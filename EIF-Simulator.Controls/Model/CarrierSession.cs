using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.Controls
{
    public class CarrierSession
    {
        public CarrierControl Carrier { get; }

        public NodeControl? Destination { get; set; }

        public Route? Route { get; set; }

        public CarrierSession(CarrierControl carrier)
        {
            Carrier = carrier;
        }
        public int RouteIndex { get; set; } = 0;

        public bool IsRunning { get; set; }
    }


}
