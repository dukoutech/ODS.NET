using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dukou.ODS
{
    /// <summary>
    /// ODS数据
    /// </summary>
    public abstract class ODSData
    {
        private SortedDictionary<string, string> data = new SortedDictionary<string, string>();

        private bool dataChanged = false;

        private string dataString = null;

        /// <summary>
        /// 获取指定KEY值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string Get(string key)
        {
            if (data.ContainsKey(key))
            {
                return data[key];
            }

            return null;
        }

        /// <summary>
        /// 设置指定KEY值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void Set(string key, string value)
        {
            data[key] = value ?? string.Empty;
            dataChanged = true;
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes($"{ToString()}\r\n");
        }

        /// <summary>
        /// 加载文本
        /// </summary>
        /// <param name="text"></param>
        public void Load(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                data.Clear();
                dataChanged = true;
                dataString = null;

                foreach (var kvStr in text.TrimEnd('\r', '\n').Split('|'))
                {
                    var kv = kvStr.Split('=');
                    data[kv[0]] = kv[1];
                }
            }
        }

        public override string ToString()
        {
            if (dataChanged || string.IsNullOrEmpty(dataString))
            {
                dataString = string.Join("|", data.Select(x => $"{x.Key}={x.Value}"));
                dataChanged = false;
            }
            return dataString.ToString();
        }
    }
}
