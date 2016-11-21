using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS.Commands
{
    /// <summary>
    /// 登录响应
    /// </summary>
    public class LoginResponse : ODSResponse
    {
        private static IDictionary<string, string> STATUSDESC = new Dictionary<string, string>()
        {
            { "0", "登录成功" },
            { "51002", "此次连线已经登录过" },
            { "51005", "用户名或密码不正确" },
            { "51010", "此账号正被其他连线使用中" }
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
