using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.Controls
{
    public class NodeTreeItem
    {
        public string Name { get; set; } = "";

        public string Type { get; set; } = "";

        public NodeControl? Control { get; set; }
    }
}
