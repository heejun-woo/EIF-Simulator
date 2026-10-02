using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.Controls
{
    public enum NodeState
    {
        Run,
        Wait,
        Trouble,
        Stop
    }

    public enum CarrierHistoryType
    {
        Created,
        DestinationSet,
        Arrived,
        CellTransfer,
        Removed,

        HostRequest,
        HostResponse,
        HostTimeout,
        HostRetry
    }
}
