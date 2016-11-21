using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS.Commands
{
    public class SubscribeCommand : ODSCommand
    {
        public SubscribeCommand()
        {
            Set("0", "Subscribe");
            Set("1", "1001");
        }

        public SubscribeCommand(string instrument) : this()
        {
            Set("10", instrument);
        }
    }
}
