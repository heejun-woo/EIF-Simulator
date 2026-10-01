using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.Controls
{
    public class MoveCommand
    {
        public NodeControl Destination { get; }

        public MoveCommand(NodeControl destination)
        {
            Destination = destination;
        }
    }
}
