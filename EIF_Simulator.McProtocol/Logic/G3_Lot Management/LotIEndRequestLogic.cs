
using EIF_Simulator.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
namespace EIF_Simulator.McProtocol.Logic
{
    public class LotIEndRequestLogic : IPlcLogic
    {

        private readonly int TRIGGER;
        private readonly int CONF_TRIGGER;
        private readonly int TIMEOUT;

        private HostRequestState _state = HostRequestState.Idle;

        private DateTime _requestTime;
        private readonly Stopwatch _timer = new();
        private bool _firstScan = true;


        public LotIEndRequestLogic(int Trigger, int ConfTrigger, int timeoutSeconds = 30)
        {
            TRIGGER = Trigger;
            CONF_TRIGGER = ConfTrigger;

            TIMEOUT = timeoutSeconds;
        }

        public void Scan(PlcBindingManager manager)
        {
            if (manager.ReadBit(TRIGGER) == false)
            {
                _state = HostRequestState.Idle;
                return;
            }
            else if (_state == HostRequestState.Idle)
            {
                _requestTime = DateTime.Now;
                _state = HostRequestState.Waiting;
            }

            #region 1초 주기로 실행
            if (!_firstScan && _timer.ElapsedMilliseconds < 1000)
            {
                return;
            }

            _firstScan = false;
            _timer.Restart();
            #endregion

            switch (_state)
            {
                case HostRequestState.Idle:
                    break;


                case HostRequestState.Waiting:
                    CheckResponse(manager);
                    break;


                case HostRequestState.Alarm:

                    manager.WriteBit(TRIGGER, false);
                    _state = HostRequestState.Idle;

                    break;
            }
        }



        private void CheckResponse(PlcBindingManager manager)
        {
            bool response = manager.ReadBit(CONF_TRIGGER);

            if (response)
            {
                manager.WriteBit(TRIGGER, false);
                return;
            }

            if ((DateTime.Now - _requestTime).TotalSeconds >= TIMEOUT)
            {
                SetAlarm(true);
                
                _state = HostRequestState.Alarm;
                manager.WriteBit(TRIGGER, false);
            }

        }


        private void SetAlarm(bool value)
        {

        }


    }
}
