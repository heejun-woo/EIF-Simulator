using EIF_Simulator.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.McProtocol.Logic
{
    public class CommunicationCheckLogic
      : IPlcLogic
    {
        private bool _lastRequest;
        private DateTime _lastRequestTime;

        private readonly Stopwatch _timer = new();
        private bool _firstScan = true;


        public string EQP_Comm_Check = "W3800";
        public Int32 Host_Comm_Check = 0x3000;
        public Int32 Host_Comm_Check_conf = 0x3800;
        public Int32 COMM_ON = 0x3808;
        public Int32 COMM_OFF = 0x3809;


        int count = 0;
        public void Scan(PlcBindingManager manager)
        {
            #region 1초 주기로 실행
            if (!_firstScan && _timer.ElapsedMilliseconds < 100)
            {
                return;
            }

            count += 1;
            _firstScan = false;
            _timer.Restart();
            #endregion

            if (count > 9)
            {
                short check = manager.GetValue<short>(EQP_Comm_Check);

                if (manager.GetValue<short>(EQP_Comm_Check) == 9999)
                {
                    manager.SetValue(EQP_Comm_Check, (short)1);
                }
                else manager.SetValue(EQP_Comm_Check, manager.GetValue<short>(EQP_Comm_Check) + 1);

                count = 0;
            }

            bool request = manager.ReadBit(Host_Comm_Check);

            // Rising Edge
            if (request != _lastRequest)
            {
                manager.WriteBit(Host_Comm_Check_conf, request);


                manager.WriteBit(COMM_ON, true);
                manager.WriteBit(COMM_OFF, false);

                _lastRequestTime = DateTime.Now;
                _lastRequest = request;
            }
            else
            {
                // Timeout
                if ((DateTime.Now - _lastRequestTime).TotalSeconds > 30)
                {
                    manager.WriteBit(COMM_ON, false);
                    manager.WriteBit(COMM_OFF, true);
                }
            }
        }
    }
}
