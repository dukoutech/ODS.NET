using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS
{
    /// <summary>
    /// ODS响应
    /// </summary>
    public class ODSResponse : ODSData
    {
        public string Status
        {
            get
            {
                return Get("5");
            }
        }

        public string Tag
        {
            get
            {
                return Get("1");
            }
        }

        public virtual string GetDescription()
        {
            return string.Empty;
        }
    }
}
