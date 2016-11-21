using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS
{
    /// <summary>
    /// ODS数据
    /// </summary>
    public class ODSData
    {
        private SortedDictionary<string, string> data = new SortedDictionary<string, string>();

        private bool dataChanged = false;

        private string dataString = null;

        /// <summary>
        /// 获取指定KEY值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (data.ContainsKey(key))
            {
                return data[key];
            }

            return null;
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
                foreach (var kvStr in text.TrimEnd('\r', '\n').Split('|'))
                {
                    var kv = kvStr.Split('=');
                    data[kv[0]] = kv[1];
                }
                dataChanged = true;
            }
        }

        /// <summary>
        /// 设置指定KEY值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, string value)
        {
            data[key] = value ?? string.Empty;
            dataChanged = true;
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
