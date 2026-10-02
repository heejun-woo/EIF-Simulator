using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.OpcUa
{
    public static class OpcUaCommonTags
    {
        public static void Register(OpcUaBindingManager manager)
        {
            manager.AddTag("LGES.CommMng.Check.E_CommCheck", (short)0);
            manager.AddTag("LGES.CommMng.Check.H_CommCheck", false);

            manager.AddTag("LGES.CommMng.Check.E_CommCheckConfirm", false);
            manager.AddTag("LGES.CommMng.State.E_CommOn", false);

            manager.AddTag("LGES.CommMng.State.E_CommOff", false);

            manager.AddTag("LGES.CommMng.Time.H_DateTimeSetRequest", false);
            manager.AddTag("LGES.CommMng.Time.H_DateAndTime", new short[6]);

            manager.AddTag("LGES.EqpState.Eqp01.E_EqpState", (short)8);
            manager.AddTag("LGES.LotMng.Current.E_LotID", string.Empty);

            manager.AddTag("LGES.HostMsg.Eqp01.H_Msg", new string[2]);
            manager.AddTag("LGES.OprtMode.Eqp01.E_AutoMode", false);

            manager.AddTag("LGES.ProcState.Eqp01.E_VisionState", new bool[2]);
        }
    }
}
