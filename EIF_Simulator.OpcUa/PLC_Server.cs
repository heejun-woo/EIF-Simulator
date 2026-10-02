using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Configuration;

namespace EIF_Simulator.OpcUa
{
    public class PLC_Server : StandardServer
    {
        public List<INodeManager> nodeManagers;

        public PLC_Server(ITelemetryContext telemetry) : base(telemetry)
        {
        }

        protected override MasterNodeManager CreateMasterNodeManager(
            IServerInternal server,
            ApplicationConfiguration configuration)
        {
            nodeManagers = new List<INodeManager>
        {
            new NodeManager(server, configuration)
        };

            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }
    }
}