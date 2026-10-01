using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EIF_Simulator.Controls
{
    public class NodeModel
    {
        public string Id { get; set; }

        public List<NodeModel> NextNodes { get; set; } = new();
    }
}
