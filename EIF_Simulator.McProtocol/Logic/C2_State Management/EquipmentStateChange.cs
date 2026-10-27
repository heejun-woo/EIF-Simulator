using EIF_Simulator.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EIF_Simulator.McProtocol.Logic
{
    public class EquipmentStateChange : IPlcLogic
    {

        public int EQUIPMENT_STATE
        {
            get => manager.GetValue<int>(EquipmentState);
            set => manager.SetValue(EquipmentState, value);
        }        
        public int SUBSTATE
        {
            get => manager.GetValue<int>(SubState);
            set => manager.SetValue(SubState, value);
        }
        public Int32 TROUBLE_CODE
        {
            get => manager.GetValue<Int32>(TroubleCode);
            set => manager.SetValue(SubState, value);
        }


        public string EquipmentState;
        public string SubState;
        public string TroubleCode;


        PlcBindingManager manager;

        public void Scan(PlcBindingManager manager)
        {
        }

        public EquipmentStateChange(PlcBindingManager _manager, string _EquipmentState = "W3810", string _SubState = "W3812", string _TroubleCode = "W380D")
        {
            manager = _manager;     
            EquipmentState = _EquipmentState;
            SubState = _SubState; 
            TroubleCode = _TroubleCode;
        }

        public void SetAlarm(int Code)
        {
            if (EQUIPMENT_STATE < 4)
            {
                EQUIPMENT_STATE = 4;
            }
        }

        public void AlarmClear()
        {
            if (EQUIPMENT_STATE == 4)
            {
                EQUIPMENT_STATE = 8;
                SUBSTATE = 118;
            }
            TROUBLE_CODE = 0;
        }
    }
}
