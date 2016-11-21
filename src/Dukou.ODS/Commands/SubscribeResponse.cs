using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS.Commands
{
    public class SubscribeResponse : ODSResponse
    {
        private static IDictionary<string, string> STATUSDESC = new Dictionary<string, string>()
        {
            { "0", "订阅成功" },
            { "50101", "无效的 Symbol或这个账号没有此 Symbol 的订阅权限" },
            { "50111", "参数有问题" },
            { "50112", "此账号没有使用CONFLATIONINTERVAL [1052]、CONFLATIONMODE [1052]的权限" }
        };

        public bool Success
        {
            get
            {
                return Status == "0";
            }
        }

        public override string GetDescription()
        {
            if (STATUSDESC.ContainsKey(Status))
            {
                return STATUSDESC[Status];
            }

            return base.GetDescription();
        }
    }
}
