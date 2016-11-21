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
        public bool Success
        {
            get
            {
                return Status == "0";
            }
        }
    }
}
