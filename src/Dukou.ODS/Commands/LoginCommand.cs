using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS.Commands
{
    public class LoginCommand : ODSCommand
    {
        public LoginCommand()
        {
            Set("0", "Login");
            Set("1", "1000");
        }

        public LoginCommand(string username, string password) : this()
        {
            this.Username = username;
            this.Password = password;
        }

        public string Username
        {
            get
            {
                return Get("1000");
            }
            set
            {
                Set("1000", value);
            }
        }

        public string Password
        {
            get
            {
                return Get("1001");
            }
            set
            {
                Set("1001", value);
            }
        }
    }
}
