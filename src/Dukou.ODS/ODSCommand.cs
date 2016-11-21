using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS
{
    public abstract class ODSCommand : ODSData
    {
        public string Command
        {
            get
            {
                return Get("0");
            }
            set
            {
                Set("0", value);
            }
        }

        public string Tag
        {
            get
            {
                return Get("1");
            }
            set
            {
                Set("1", value);
            }
        }
    }
}
