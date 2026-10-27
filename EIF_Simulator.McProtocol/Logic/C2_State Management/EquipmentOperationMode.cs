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
    public class EquipmentOperationMode : IPlcLogic
    {        
        public bool AUTO_MODE
        {
            get => manager.GetValue<bool>(AutoMode);
            set => manager.SetValue(AutoMode, value);
        }
        public int LANGUAGE_TYPE
        {
            get => manager.GetValue<int>(LangType);
            set => manager.SetValue(LangType, value);
        }


        public string AutoMode;
        public string LangType;


        PlcBindingManager manager;

        public void Scan(PlcBindingManager manager)
        {
        }

        public EquipmentOperationMode(PlcBindingManager _manager, string _AutoMode = "B380B", string _LangType = "W3900")
        {
            manager = _manager;
            AutoMode = _AutoMode;
            LangType = _LangType; 
        }
    }
}
