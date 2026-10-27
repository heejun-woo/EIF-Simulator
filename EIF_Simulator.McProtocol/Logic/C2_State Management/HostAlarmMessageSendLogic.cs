using EIF_Simulator.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EIF_Simulator.McProtocol.Logic
{
    public class HostAlarmMessageSendLogic : IPlcLogic
    {
        private readonly int TRIGGER;

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
        public event Action<int, int, string> ReceiveHostMessage ;

        public string AutoMode;
        public string LangType;


        PlcBindingManager manager;

        private bool _TriggerValue = false;

        public void Scan(PlcBindingManager manager)
        {
            if (manager.ReadBit(TRIGGER) && !_TriggerValue)
            {
                // 발생
                ReceiveHostMessage?.Invoke(3, 20, "Processing");

                _TriggerValue = true;
                return;
            }
            else if (manager.ReadBit(TRIGGER) == false && _TriggerValue)
            {
                _TriggerValue = false;
            }
        }

        public HostAlarmMessageSendLogic(PlcBindingManager _manager, string _AutoMode = "B380B", string _LangType = "W3900")
        {
            manager = _manager;
            AutoMode = _AutoMode;
            LangType = _LangType; 
        }
    }
}
