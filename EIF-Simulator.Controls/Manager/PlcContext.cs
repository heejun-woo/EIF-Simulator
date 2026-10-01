using EIF_Simulator.Core;
using EIF_Simulator.McProtocol.Plc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.Controls.Manager
{
    public class PlcContext
    {
        public NodeRegistry Nodes { get; } 
        public CarrierRegistry Carriers { get; }
        public CarrierManager CarrierManager { get; } 
        public NodeManager NodeManager { get; } 

        public CarrierHistoryManager CarrierHistory { get; } 
        public CarrierRouteManager _routeManager { get; }

        public PlcMemory Memory { get; }
        public PlcBindingManager BindingManager { get; }
        public PlcScanEngine ScanEngine { get; }
        public McProtocolServer Server { get; }

        public CarrierStateManager CarrierStateManager { get; }
        
        public PlcContext()
        {
            Memory = new PlcMemory();
            BindingManager = new PlcBindingManager(Memory);
            ScanEngine = new PlcScanEngine(BindingManager);

            Nodes = new NodeRegistry();
            Carriers = new CarrierRegistry();
            CarrierManager = new CarrierManager();
            NodeManager = new NodeManager();
            CarrierHistory = new CarrierHistoryManager();
            _routeManager = new CarrierRouteManager(CarrierManager, Nodes);
            Server = new McProtocolServer(Memory, BindingManager);
            CarrierStateManager = new CarrierStateManager(this);
        }


        public void Start(int port)
        {
            _ = Server.StartAsync(port);
            ScanEngine.Start();
        }

        public void Stop()
        {
            _ = ScanEngine.StopAsync();
            Server.Stop();
        }
    }
}
