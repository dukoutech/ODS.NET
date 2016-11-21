using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS.Commands
{
    public class UnsubscribeResponse : ODSResponse
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
