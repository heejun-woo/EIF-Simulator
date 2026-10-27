using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.Core
{
    public static class TrigStatus
    {
        public const bool TRIG_ON = true;
        public const bool TRIG_OFF = false;
    }

    public static class ConfirmAck
    {
        public const ushort CLEAR = 00;
        public const ushort OK = 10;
        public const ushort NG = 11;
    }
}
