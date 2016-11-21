using Common.Logging;
using Dukou.ODS.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Dukou.ODS
{
    public class ODSLevel1QuoteDataFeed : IDisposable
    {
        private static ILog logger = LogManager.GetLogger("ODS");

        private static string ODSLEVEL1QUOTEDATAQUEUEKEY = "ODSLEVEL1QUOTEDATA";

        private static char[] PACKAGEENDCHARS = new char[] { '\n' };

        private TcpClient client = null;

        private IDictionary<string, AutoResetEvent> CmdAutoResetEvents = new Dictionary<string, AutoResetEvent>()
        {
            { ODSCommandTags.Login, new AutoResetEvent(false) },
            { ODSCommandTags.Subscribe, new AutoResetEvent(false) },
            { ODSCommandTags.Unsubscribe, new AutoResetEvent(false) },
            { ODSCommandTags.UnsubscribeAll, new AutoResetEvent(false) },
        };

        private ISet<string> Instruments = new HashSet<string>();

        private ConcurrentDictionary<string, ConcurrentQueue<string>> ODSResponseTextQueues = null;

        /// <summary>
        /// ODS服务器IP
        /// </summary>
        public string ODSServerIP { get; private set; }

        /// <summary>
        /// ODS服务器端口
        /// </summary>
        public int ODSServerPort { get; private set; }

        /// <summary>
        /// ODS用户名
        /// </summary>
        public string ODSUserName { get; private set; }

        /// <summary>
        /// ODS用户密码
        /// </summary>
        public string ODSUserPassword { get; private set; }

        /// <summary>
        /// 是否已启动
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// 重启时间
        /// </summary>
        public int RestartTimes { get; set; }

        public ODSLevel1QuoteDataFeed(string ip, int port, string username, string password)
        {
            ODSServerIP = ip;
            ODSServerPort = port;
            ODSUserName = username;
            ODSUserPassword = password;

            ODSResponseTextQueues = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            ODSResponseTextQueues[ODSCommandTags.Login] = new ConcurrentQueue<string>();
            ODSResponseTextQueues[ODSCommandTags.Subscribe] = new ConcurrentQueue<string>();
            ODSResponseTextQueues[ODSCommandTags.Unsubscribe] = new ConcurrentQueue<string>();
            ODSResponseTextQueues[ODSCommandTags.UnsubscribeAll] = new ConcurrentQueue<string>();
            ODSResponseTextQueues["ODSLevel1Data"] = new ConcurrentQueue<string>();

            RestartTimes = 120;
        }

        public ODSLevel1QuoteData Dequeue()
        {
            string responseText = null;
            if (ODSResponseTextQueues[ODSLEVEL1QUOTEDATAQUEUEKEY].TryDequeue(out responseText))
            {
                var data = new ODSLevel1QuoteData();
                data.Load(responseText);

                return data;
            }

            return null;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            try
            {
                var cmd = new LoginCommand(ODSUserName, ODSUserPassword);

                var responseText = SendCmd(ODSCommandTags.Login, cmd);
                if (!string.IsNullOrEmpty(responseText))
                {
                    var response = new LoginResponse();
                    response.Load(responseText);
                    return response.Success;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public bool Subscribe(string instrument)
        {
            try
            {
                if (!Instruments.Contains(instrument) && Instruments.Add(instrument))
                {
                    var cmd = new SubscribeCommand(instrument);

                    var responseText = SendCmd(ODSCommandTags.Subscribe, cmd);
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        var response = new SubscribeResponse();
                        response.Load(responseText);
                        return response.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public bool Unsubscribe(string instrument)
        {
            try
            {
                if (Instruments.Remove(instrument))
                {
                    var cmd = new UnsubscribeCommand(instrument);

                    var responseText = SendCmd(ODSCommandTags.Unsubscribe, cmd);
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        var response = new UnsubscribeResponse();
                        response.Load(responseText);

                        return response.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// 取消所有订阅
        /// </summary>
        /// <returns></returns>
        public bool UnsubscribeAll()
        {
            try
            {
                Instruments.Clear();

                var cmd = new UnsubscribeAllCommand();

                var responseText = SendCmd(ODSCommandTags.UnsubscribeAll, cmd);
                if (!string.IsNullOrEmpty(responseText))
                {
                    var response = new UnsubscribeAllResponse();
                    response.Load(responseText);

                    return response.Success;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            UnsubscribeAll();

            foreach (var item in ODSResponseTextQueues)
            {
                if (item.Value.Count > 0)
                {
                    while (item.Value.TryDequeue(out string result) && item.Value.Count > 0)
                    {
                        logger.Info($"移除数据{result}");
                    }
                }
            }

            Started = false;

            CloseTcpClient();
        }

        /// <summary>
        /// 关闭 TcpClient
        /// </summary>
        private void CloseTcpClient()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }

        /// <summary>
        /// 确认已连接
        /// </summary>
        private void EnsureConnected()
        {
            if (client == null)
            {
                client = new TcpClient()
                {
                    ReceiveTimeout = 10000
                };
            }
            if (!client.Connected)
            {
                lock (this)
                {
                    if (!!client.Connected)
                    {
                        Started = true;
                        client.Connect(ODSServerIP, ODSServerPort);
                        new Thread(Receive).Start();
                    }
                }
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        private void Receive()
        {
            try
            {
                var buffer = new byte[1024];
                var bufferSize = 1024;
                var lastReceiveTime = DateTime.Now;
                var sb = new StringBuilder();

                ODSResponse response = null;

                while (ShouldRun())
                {
                    if (ShouldSleep())
                    {
                        Thread.Sleep(10);
                    }

                    if (ShouldReceive())
                    {
                        do
                        {
                            int len = client.GetStream().Read(buffer, 0, bufferSize);
                            sb.Append(Encoding.UTF8.GetString(buffer, 0, len));
                        } while (ShouldReceive());
                    }
                    if (sb.Length > 0)
                    {
                        var responseText = sb.ToString();
                        foreach (var item in responseText.Split(PACKAGEENDCHARS, StringSplitOptions.RemoveEmptyEntries))
                        {
                            response = new ODSResponse();
                            response.Load(item);

                            if (!string.IsNullOrEmpty(response.Tag) && ODSResponseTextQueues.ContainsKey(response.Tag))
                            {
                                ODSResponseTextQueues[response.Tag].Enqueue(item);
                                CmdAutoResetEvents[response.Tag].Set();
                            }
                            else
                            {
                                ODSResponseTextQueues[ODSLEVEL1QUOTEDATAQUEUEKEY].Enqueue(item);
                                logger.Info($"ODSLevel1QuoteData:{item}");
                            }
                        }

                        lastReceiveTime = DateTime.Now;
                    }

                    if (lastReceiveTime.AddSeconds(RestartTimes) < DateTime.Now)
                    {
                        logger.Info($"超过{RestartTimes}秒未接收到数据");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Info("接收数据退出循环");

            if (Started)
            {
                var instruments = Instruments.Select(x => x).ToList();

                CloseTcpClient();

                Login();

                UnsubscribeAll();

                foreach (var item in instruments)
                {
                    Subscribe(item);
                }

                logger.Info("重新接收数据");
            }
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private string SendCmd(string tag, ODSCommand cmd)
        {
            var responseText = string.Empty;
            var data = cmd.GetBytes();
            EnsureConnected();
            logger.Info($"{tag}发送报文:{cmd}");
            client.GetStream().Write(data, 0, data.Length);
            CmdAutoResetEvents[tag].WaitOne(60000);
            if (ODSResponseTextQueues[tag].TryDequeue(out responseText))
            {
                logger.Info($"{tag}响应报文:{responseText}");
                return responseText;
            }

            return null;
        }

        /// <summary>
        /// 应该接收数据
        /// </summary>
        /// <returns></returns>
        private bool ShouldReceive()
        {
            return ShouldRun() && client.Available > 0;
        }

        /// <summary>
        /// 应该运行
        /// </summary>
        /// <returns></returns>
        private bool ShouldRun()
        {
            return Started && client != null;
        }

        /// <summary>
        /// 应该睡眠
        /// </summary>
        /// <returns></returns>
        public bool ShouldSleep()
        {
            return ShouldRun() && client.Available == 0;
        }

    }
}
