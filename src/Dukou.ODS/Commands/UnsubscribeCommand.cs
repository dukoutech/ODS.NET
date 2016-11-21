using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS.Commands
{
    public class UnsubscribeCommand : ODSCommand
    {
        public UnsubscribeCommand()
        {
            Set("0", "Unsubscribe");
            Set("1", "1002");
        }

        public UnsubscribeCommand(string instrument) : this()
        {
            Set("10", instrument);
        }
    }
}
