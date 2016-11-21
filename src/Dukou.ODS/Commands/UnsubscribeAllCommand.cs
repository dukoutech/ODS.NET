using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS.Commands
{
    public class UnsubscribeAllCommand : ODSCommand
    {
        public UnsubscribeAllCommand()
        {
            Set("0", "UnsubscribeAll");
            Set("1", "1003");
        }
    }
}
