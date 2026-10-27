
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
    public class LotInfoRequestLogic : IPlcLogic
    {

        private readonly int TRIGGER;
        private readonly int CONF_TRIGGER;
        private readonly int CONF_TRIGGER_ACK;
        private readonly int TIMEOUT;


        public string NEXT_LOTID = "W3868";
        public string NEXT_PRODID = "W3860";
        public Int32 HOST_PRODID = 0x3120;
        public Int32 HOST_LOTID = 0x3128;

        private readonly int _LotIdLength = 8;

        private HostRequestState _state = HostRequestState.Idle;

        private DateTime _requestTime;
        private readonly Stopwatch _timer = new();
        private bool _firstScan = true;


        public LotInfoRequestLogic(int Trigger, int ConfTrigger, int ConfTriggerAck, int timeoutSeconds = 30)
        {
            TRIGGER = Trigger;
            CONF_TRIGGER = ConfTrigger;
            CONF_TRIGGER_ACK = ConfTriggerAck;
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
                if (manager.ReadWord(CONF_TRIGGER_ACK) == ConfirmAck.OK)
                {
                    string strLotID = manager.ReadStringFromMemory(HOST_LOTID, _LotIdLength);
                    manager.SetValue(NEXT_LOTID, strLotID, _LotIdLength);

                    string strProdID = manager.ReadStringFromMemory(HOST_PRODID, 5);
                    manager.SetValue(NEXT_PRODID, strProdID, 5);
                }

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
