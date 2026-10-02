using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Server;

namespace EIF_Simulator.OpcUa
{
    public class OpcUaServer : StandardServer
    {
        private readonly OpcUaBindingManager _bindingManager;

        public OpcUaNodeManager? NodeManager { get; private set; }

        public OpcUaServer(
            ITelemetryContext telemetry,
            OpcUaBindingManager bindingManager)
            : base(telemetry)
        {
            _bindingManager = bindingManager;
        }

        protected override MasterNodeManager CreateMasterNodeManager(
            IServerInternal server,
            ApplicationConfiguration configuration)
        {
            NodeManager =
                new OpcUaNodeManager(
                    server,
                    configuration,
                    _bindingManager);

            INodeManager[] nodeManagers =
            {
                NodeManager
            };

            return new MasterNodeManager(
                server,
                configuration,
                null,
                nodeManagers);
        }
    }
}
