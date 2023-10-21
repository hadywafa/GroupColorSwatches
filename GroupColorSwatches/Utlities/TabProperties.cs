using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupColorSwatches.Utlities
{
    public class TabProperties : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            var server = Grasshopper.Instances.ComponentServer;
            server.AddCategoryShortName("Hady Wafa", "HW");
            server.AddCategorySymbolName("Hady Wafa", 'H');
            server.AddCategoryIcon("Hady Wafa", Properties.Resources.HW);
            return GH_LoadingInstruction.Proceed;
        }
    }
}
