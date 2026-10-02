
using Opc.Ua;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EIF_Simulator.OpcUa
{
    public class CommonLogic
    {
        private ApplicationInstance _app;
        private PLC_Server PLC;
        private NodeManager Tags;

        public CommonLogic(string strPort)
        {
            #region PLC_Server Start
            Opc.Ua.ApplicationConfiguration config = new Opc.Ua.ApplicationConfiguration()
            {
                ApplicationName = "FakePLC",
                ApplicationType = ApplicationType.Server,
                ServerConfiguration = new ServerConfiguration
                {
                    BaseAddresses = { $"opc.tcp://localhost:{strPort}" }
                },
                SecurityConfiguration = new SecurityConfiguration
                {
                    AutoAcceptUntrustedCertificates = true,
                    AddAppCertToTrustedStore = true,
                    ApplicationCertificate = new CertificateIdentifier()
                },

                TraceConfiguration = new TraceConfiguration(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                DisableHiResClock = false
            };

            config.ServerConfiguration.SecurityPolicies.Clear();
            config.ServerConfiguration.SecurityPolicies.Add(new ServerSecurityPolicy
            {
                SecurityMode = MessageSecurityMode.None,
                SecurityPolicyUri = SecurityPolicies.None
            });

            config.Validate(ApplicationType.Server).Wait();

            _app = new ApplicationInstance(config);
            var result = _app.CheckApplicationInstanceCertificate(false, CertificateFactory.DefaultKeySize);

            PLC = new PLC_Server();
            _app.Start(PLC);

            Tags = PLC.nodeManagers[0] as NodeManager;
            #endregion

            #region Variable 선언

            #region 7.1 [C] Equipment Common Management

            #region 7.1.1.1 [C1-1] EQP Communication Check 
            AddVariable("LGES.CommMng.Check.E_CommCheck", DataTypeIds.Int16, 0);
            #endregion

            #region 7.1.1.2 [C1-2] Host Communication Check 
            AddVariable("LGES.CommMng.Check.H_CommCheck", DataTypeIds.Boolean, false);
            AddVariable("LGES.CommMng.Check.E_CommCheckConfirm", DataTypeIds.Boolean, false);
            #endregion

            #region 7.1.1.3 [C1-3] Communication State Change Report 
            AddVariable("LGES.CommMng.State.E_CommOn", DataTypeIds.Boolean, false);
            AddVariable("LGES.CommMng.State.E_CommOff", DataTypeIds.Boolean, false);
            #endregion

            #region 7.1.1.4 [C1-4] Date and Time Set Request
            AddVariable("LGES.CommMng.Time.H_DateTimeSetRequest", DataTypeIds.Boolean, false);
            AddVariable("LGES.CommMng.Time.H_DateAndTime", DataTypeIds.Int16, new short[6] { 0, 0, 0, 0, 0, 0 });
            #endregion

            #region 7.1.2.1 [C2-1] Equipment State Change Report 
            AddVariable("LGES.EqpState.Eqp01.E_EqpState", DataTypeIds.Int16, 8);
            AddVariable("LGES.EqpState.Eqp01.E_EqpSubstate", DataTypeIds.Int16, 0);
            AddVariable("LGES.EqpState.Eqp01.E_TroubleCode", DataTypeIds.Int16, 0);
            AddVariable("LGES.EqpState.Eqp01.E_UserStopPopUpDelayTime", DataTypeIds.Int16, 0);
            AddVariable("LGES.LotMng.Common.E_LotRunning", DataTypeIds.Boolean, false);
            AddVariable("LGES.LotMng.Current.E_LotID", DataTypeIds.String, string.Empty);
            AddVariable("LGES.LotMng.Current.E_ProductID", DataTypeIds.String, string.Empty);
            #endregion

            #region 7.1.2.3 [C2-3] Host Alarm Message Send
            AddVariable("LGES.HostMsg.Eqp01.H_Send", DataTypeIds.Boolean, false);
            AddVariable("LGES.HostMsg.Eqp01.H_SendSystem", DataTypeIds.Int16, 0);
            AddVariable("LGES.HostMsg.Eqp01.H_EQPStopType", DataTypeIds.Int16, 0);
            AddVariable("LGES.HostMsg.Eqp01.H_DisplayType", DataTypeIds.Int16, 0);
            AddVariable("LGES.HostMsg.Eqp01.H_Msg", DataTypeIds.String, new string[2] { "", "" });
            #endregion

            #region 7.1.2.4 [C2-4] Equipment Operation Mode Change Report
            AddVariable("LGES.OprtMode.Eqp01.E_AutoMode", DataTypeIds.Boolean, false);
            AddVariable("LGES.OprtMode.Eqp01.E_ITBypass", DataTypeIds.Boolean, false);
            AddVariable("LGES.OprtMode.Eqp01.E_ReworkMode", DataTypeIds.Boolean, false);
            AddVariable("LGES.OprtMode.Eqp01.E_HMILanguageType", DataTypeIds.Int16, new short[1] { 1 });
            AddVariable("LGES.OprtMode.Eqp01.E_RunEnable", DataTypeIds.Boolean, false);
            #endregion

            #region 7.1.2.5 [C2-5] Processing State Change Report
            AddVariable("LGES.LotMng.Common.E_LotRunning", DataTypeIds.Boolean, false);
            AddVariable("LGES.LotMng.Common.E_LotEndBusy", DataTypeIds.Boolean, false);
            AddVariable("LGES.ProcState.Eqp01.E_VisionState", DataTypeIds.Boolean, new bool[2] { false, false });
            #endregion

            #region 7.1.2.6 [C2-6] Remote Command Send
            AddVariable("LGES.RemoteCmd.Eqp01.H_Send", DataTypeIds.Boolean, false);
            AddVariable("LGES.RemoteCmd.Eqp01.H_Code", DataTypeIds.Int16, 0);
            AddVariable("LGES.RemoteCmd.Eqp01.E_Confirm", DataTypeIds.Boolean, false);
            AddVariable("LGES.RemoteCmd.Eqp01.E_ConfirmACK", DataTypeIds.Int16, 0);
            #endregion

            #region 7.1.2.8 [C2-8] Alarm Set Report
            AddVariable("LGES.EqpState.Eqp01.H_AlarmSetConfirm", DataTypeIds.Boolean, false);
            AddVariable("LGES.EqpState.Eqp01.E_AlarmSetReport", DataTypeIds.Boolean, false);
            AddVariable("LGES.EqpState.Eqp01.E_SetAlarm", DataTypeIds.Int16, 0);
            #endregion

            #region 7.1.2.9 [C2-9] Alarm Reset Report
            AddVariable("LGES.EqpState.Eqp01.H_AlarmResetConfirm", DataTypeIds.Boolean, false);
            AddVariable("LGES.EqpState.Eqp01.E_AlarmResetReport", DataTypeIds.Boolean, false);
            AddVariable("LGES.EqpState.Eqp01.E_ResetAlarm", DataTypeIds.Int16, 0);
            #endregion 
            #endregion

            #endregion

            DefineVariable();

            //PLC 로직 동작
            StartLoop();
        }

        #region Common Logic

        protected virtual void DefineVariable()
        {

        }

        public static string GetVariableName(string name)
        {
            List<string> lst = name.Split('.').ToList();

            string str = string.Empty;
            foreach (string s in lst)
            {
                str += "\"" + s + "\".";
            }
            return str.Substring(0, str.Length - 1);
        }

        public object GetValue(string name)
        {
            var node = Tags.findNode(GetVariableName(name));
            if (node != null)
            {
                return Tags._memory[node.DisplayName.Text];
            }
            return null;
        }
        public object SetValue(string name, object value)
        {
            var node = Tags.findNode(GetVariableName(name));
            if (node != null)
            {
                Tags._memory[node.DisplayName.Text] = value;

                OnTagChanged(node);
                return true;
            }
            return false;
        }
        public object SetValue(string name, int arryIndex, object value)
        {
            var node = Tags.findNode(GetVariableName(name));
            if (node != null)
            {

                if (Tags._memory[node.DisplayName.Text] is Array arr && arryIndex >= 0 && arryIndex < arr.Length)
                {
                    if (arr.GetType().Name == "String[]")
                    {
                        value = Convert.ToString(value);
                    }
                    else if (arr.GetType().Name == "Int16[]")
                    {
                        value = Convert.ToInt16(value);
                    }
                    else if (arr.GetType().Name == "Boolean[]")
                    {
                        value = Convert.ToBoolean(value);
                    }

                    arr.SetValue(value, arryIndex);
                    Tags._memory[node.DisplayName.Text] = arr;
                }
            }

            Tags._memory[$"{GetVariableName(name)}[{arryIndex}]"] = value;

            return false;
        }

        public object AddValue(string name, int value)
        {
            var node = Tags.findNode(GetVariableName(name));
            if (node != null)
            {
                int x = Convert.ToInt32(Tags._memory[node.DisplayName.Text]) + value;
                Tags._memory[node.DisplayName.Text] = x;

                OnTagChanged(node);
                return true;
            }
            return false;
        }

        public object MoveValue(string name, string name2)
        {
            var node = Tags.findNode(GetVariableName(name));
            var node2 = Tags.findNode(GetVariableName(name2));

            if (node != null)
            {
                Tags._memory[node2.DisplayName.Text] = Tags._memory[node.DisplayName.Text];
                OnTagChanged(node2);

                ClearValue(name);
                return true;
            }
            return false;
        }


        public object ClearValue(string name)
        {
            var node = Tags.findNode(GetVariableName(name));

            if (node != null)
            {
                switch (Tags._memory[node.DisplayName.Text].GetType().Name.ToString())
                {
                    case "String":
                        Tags._memory[node.DisplayName.Text] = string.Empty;
                        break;

                    case "Int32":
                    case "Int16":
                        Tags._memory[node.DisplayName.Text] = 0;
                        break;

                    default:
                        break;
                }

                OnTagChanged(node);
                return true;
            }
            return false;
        }

        public bool ContainKey(string Key)
        {
            if (Tags._memory.ContainsKey(Key)) return true;
            return false;
        }
        public void AddVariable(string name, NodeId dataType, object defaultValue)
        {
            BaseDataVariableState? var = Tags.AddVariable(GetVariableName(name), dataType, defaultValue);
            if (var != null) var.OnSimpleWriteValue = OnWriteValue;
        }

        // 외부에서 변경시 발생하는 이벤트
        private ServiceResult OnWriteValue(ISystemContext ctx, NodeState node, ref object val)
        {
            Console.WriteLine($"{node.DisplayName} = {val}");
            Tags._memory[node.DisplayName.Text] = val;
            string text = node.DisplayName.Text.Replace("\"", "");

            switch (text)
            {
                case "LGES.CommMng.Check.H_CommCheck":
                    _DtComCheck = DateTime.Now;
                    SetValue("LGES.CommMng.State.E_CommOn", true);
                    SetValue("LGES.CommMng.State.E_CommOff", false);
                    SetValue("LGES.CommMng.Check.E_CommCheckConfirm", val);
                    break;

                case "LGES.HostMsg.Eqp01.H_Send":
                    if ((bool)val) 
                        PrintHostMessage();
                    break;

                case "LGES.RemoteCmd.Eqp01.H_Send":
                    if ((bool)val)
                    {
                        RemoteCommand();
                    }
                    else
                        SetValue("LGES.RemoteCmd.Eqp01.E_Confirm", false);

                    break;

                default:
                    OnWriteValueEvent(ctx, node, ref val);
                    break;
            }


            return ServiceResult.Good;
        }


        protected virtual void OnWriteValueEvent(ISystemContext ctx, NodeState node, ref object val)
        {

        }

        // 내부에서 변경시 발생하는 이벤트
        private void OnTagChanged(NodeState node)
        {
            string text = node.DisplayName.Text.Replace("\"", "");
            object val = Tags._memory[node.DisplayName.Text];

            switch (text)
            {
                default:
                    OnTagChangedEvent(node);
                    break;
            }

        }
        protected virtual void OnTagChangedEvent(NodeState node)
        {

        }


        #endregion

        DateTime _DtComCheck;
        public void StartLoop()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    #region 7.1.1.1 [C1-1] EQP Communication Check
                    int E_CommCheck = Convert.ToInt16(GetValue("LGES.CommMng.Check.E_CommCheck")) + 1;
                    if (E_CommCheck > 9999) E_CommCheck = 0;
                    SetValue("LGES.CommMng.Check.E_CommCheck", E_CommCheck);
                    #endregion

                    #region 7.1.1.2 [C1-2] Host Communication Check 
                    if ((DateTime.Now - _DtComCheck).TotalSeconds > 30)
                    {
                        SetValue("LGES.CommMng.State.E_CommOn", false);
                        SetValue("LGES.CommMng.State.E_CommOff", true);
                    }
                    #endregion

                    await Task.Delay(1000);
                }
            });
        }

        #region 7.1.2.3 [C2-3] Host Alarm Message Send
        public void PrintHostMessage()
        {
            Task.Run(() =>
            {
                var msg = GetValue("LGES.HostMsg.Eqp01.H_Msg");
                if (msg.GetType().Name == "String[]")
                {
                    PrintHostMessageEvent(Convert.ToInt32(GetValue("LGES.HostMsg.Eqp01.H_SendSystem")), string.Join("", msg as string[]));
                }
            });

            if (Convert.ToInt32(GetValue("LGES.HostMsg.Eqp01.H_EQPStopType")) > 0)
            {
                int alarm = Convert.ToInt32(GetValue("LGES.HostMsg.Eqp01.H_SendSystem")) * 100 +
                   Convert.ToInt32(GetValue("LGES.HostMsg.Eqp01.H_EQPStopType"));
                AddAlarm(alarm);
            }
        }
        protected virtual void PrintHostMessageEvent(int system, string str)
        {
        }
        #endregion

        #region 7.1.2.6 [C2-6] Remote Command Send
        public void RemoteCommand()
        {
            int ack = 11;
            switch (Convert.ToInt16(GetValue("LGES.RemoteCmd.Eqp01.H_Code")))
            {
                case 12:
                    if ((bool)GetValue("LGES.OprtMode.Eqp01.E_ITBypass"))
                    {
                        SetValue("LGES.OprtMode.Eqp01.E_ITBypass", false);
                        ack = 10;
                    }
                    break;
                case 21:

                    if (Convert.ToInt32(GetValue("LGES.EqpState.Eqp01.E_EqpState")) < 4)
                    {
                        SetValue("LGES.EqpState.Eqp01.E_EqpState", 8);
                        SetValue("LGES.EqpState.Eqp01.E_EqpSubstate", 118);
                        ack = 10;
                    }
                    break;
            }

            SetValue("LGES.RemoteCmd.Eqp01.E_ConfirmACK", ack);
            SetValue("LGES.RemoteCmd.Eqp01.E_Confirm", true);

        }
        #endregion

        #region 7.1.2.8 [C2-8] Alarm Set Report
        Queue<int> alarmQueue = new Queue<int>();
        public void AddAlarm(int alarmCode)
        {
            if (Convert.ToInt32(GetValue("LGES.EqpState.Eqp01.E_EqpState")) < 4)
            {
                SetValue("LGES.EqpState.Eqp01.E_EqpState", 4);
            }
            SetValue("LGES.EqpState.Eqp01.E_TroubleCode", alarmCode);


            SetValue("LGES.EqpState.Eqp01.E_SetAlarm", alarmCode);
            SetValue("LGES.EqpState.Eqp01.E_AlarmSetReport", true);

            alarmQueue.Enqueue(alarmCode);

            Task.Run(async () =>
            {
                DateTime On = DateTime.Now;
                while (true)
                {
                    await Task.Delay(1000);

                    if ((DateTime.Now - On).TotalSeconds > 30)
                    {
                        SetValue("LGES.EqpState.Eqp01.E_AlarmSetReport", false);
                        setEqpAlarm(1001);

                        break;
                    }
                    else
                    {
                        if ((bool)GetValue("LGES.EqpState.Eqp01.H_AlarmSetConfirm"))
                        {
                            SetValue("LGES.EqpState.Eqp01.E_SetAlarm", 0);
                            SetValue("LGES.EqpState.Eqp01.E_AlarmSetReport", false);

                            break;
                        }
                    }

                }
            });
        }
        public void setEqpAlarm(int alarmCode, bool isLight = false)
        {
            if (isLight)
            {
                AddAlarm(alarmCode);
            }
            else
            {
                if (Convert.ToInt32(GetValue("LGES.EqpState.Eqp01.E_EqpState")) < 4)
                {
                    SetValue("LGES.EqpState.Eqp01.E_EqpState", 4);
                    SetValue("LGES.EqpState.Eqp01.E_TroubleCode", alarmCode);
                }
                else
                {
                    SetValue("LGES.EqpState.Eqp01.E_TroubleCode", alarmCode);
                }
            }
        }
        #endregion

        #region 7.1.2.9 [C2-9] Alarm Reset Report
        object lockResetAlarm = new object();
        public void ResetAlarm()
        {
            alarmQueue.Enqueue(0);

            bool isProcess = false;

            while (alarmQueue.Count > 0)
            {
                if (isProcess)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                lock (lockResetAlarm)
                {
                    isProcess = true;

                    int alarm = alarmQueue.Dequeue();

                    SetValue("LGES.EqpState.Eqp01.E_ResetAlarm", alarm);
                    SetValue("LGES.EqpState.Eqp01.E_AlarmResetReport", true);
                }

                Task.Run(async () =>
                {
                    DateTime On = DateTime.Now;
                    while (true)
                    {
                        await Task.Delay(100);

                        if ((DateTime.Now - On).TotalSeconds > 30)
                        {
                            SetValue("LGES.EqpState.Eqp01.E_AlarmResetReport", false);
                            setEqpAlarm(1002);

                            lock (lockResetAlarm)
                            {
                                isProcess = false;
                            }
                            break;
                        }
                        else
                        {
                            if ((bool)GetValue("LGES.EqpState.Eqp01.H_AlarmResetConfirm"))
                            {
                                SetValue("LGES.EqpState.Eqp01.E_ResetAlarm", 0);
                                SetValue("LGES.EqpState.Eqp01.E_AlarmResetReport", false);

                                lock (lockResetAlarm)
                                {
                                    isProcess = false;
                                }
                                break;
                            }
                        }
                    }
                });

            }

            if (Convert.ToInt32(GetValue("LGES.EqpState.Eqp01.E_EqpState")) == 4)
            {
                SetValue("LGES.EqpState.Eqp01.E_EqpState", 8);
                SetValue("LGES.EqpState.Eqp01.E_EqpSubstate", 118);
                SetValue("LGES.EqpState.Eqp01.E_TroubleCode", 0);
            }
        } 
        #endregion
    }
}
