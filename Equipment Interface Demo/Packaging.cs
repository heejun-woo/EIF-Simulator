using Equipment_Interface_Demo.Popup;
using Microsoft.VisualBasic.ApplicationServices;
using System.Reflection.PortableExecutable;
using System.Security.Policy;
using System.Windows.Forms;
using static System.Windows.Forms.Design.AxImporter;

namespace Equipment_Interface_Demo
{
    public partial class Packaging : Form
    {
        bool _bLotRunning = false;
        bool _bItPass = false;

        public Packaging()
        {
            InitializeComponent();

            InitializeCommChkLayout(tabCommChk);
            Initialize_plcEqpStatLayout(tabEqpStat);
            InitializeAlarmLayout(tabAlarm);
            InitializeHostMsgLayout(tabHostMsg);

            InitializeCarrierLayout(tabMtrl);
            InitializeCarrierRWLayout(tabCarrierRU);
            InitializeCarrierRLLayout(tabCarrierRL);

            InitializeLotControlLayout(tabLotControl);
            InitializeUBMLayout(tabUbm);

        }


        #region 7.1.1 [C1] Communication Management
        private PlcMonitorControl _plcCommChk;

        private bool _bCommChk = false;

        private void InitializeCommChkLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();


            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _plcCommChk = new PlcMonitorControl();
            _plcCommChk.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_plcCommChk);
            TimerCommchk.Enabled = true;
        }

        private void btnCommStep1_Click(object sender, EventArgs e)
        {
            _IEqpCommChkTimeOut = 0;

            _plcCommChk.HostWrite("Host Comm Check", true);
            _plcCommChk.EqpWrite("Host Comm Check Confirm", true);

            if (!_bCommChk)
            {
                _IEqpCommChkTimeOut += 1;
                _plcCommChk.EqpWrite("COMM ON", true);
                _plcCommChk.EqpWrite("COMM OFF", false);
            }

            btnlbCommChk(true);

            Thread.Sleep(1000);

            _plcCommChk.HostWrite("Host Comm Check", false);
            _plcCommChk.EqpWrite("Host Comm Check Confirm", false);
        }

        private void btnlbCommChk(bool stat)
        {
            _bCommChk = stat;

            if (stat)
            {
                lbCommChk.BackColor = Color.Green;
                lbCommChk.Text = "COMM ON";
            }
            else
            {
                lbCommChk.BackColor = Color.Red;
                lbCommChk.Text = "COMM OFF";
            }
        }

        int _iEqpCommChk = 0;
        int _IEqpCommChkTimeOut = 0;
        private void TimerCommchk_Tick(object sender, EventArgs e)
        {
            if (_iEqpCommChk == 999) _iEqpCommChk = 0;
            _iEqpCommChk += 1;
            _plcCommChk.EqpWrite("EQP Comm Check", _iEqpCommChk);

            if (_bCommChk)
            {
                _IEqpCommChkTimeOut += 1;

                if (_IEqpCommChkTimeOut > 30)
                {
                    btnlbCommChk(false);

                    _plcCommChk.EqpWrite("COMM ON", false);
                    _plcCommChk.EqpWrite("COMM OFF", true);

                    _plcCommChk.Info("If no communication check occurs for 30 seconds");
                    _plcCommChk.Info(" it switches to OFF and a heavy alarm is triggered.");
                    _plcCommChk.Event("Heavy alarm triggered - (Code:100)");
                    AddAlarmSetCode(100);

                }
            }
        }
        #endregion

        #region 7.1.2.1 [C2-1] Equipment State Change Report
        private PlcMonitorControl _plcEqpStat;


        private void Initialize_plcEqpStatLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();
            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _plcEqpStat = new PlcMonitorControl();
            _plcEqpStat.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_plcEqpStat);
        }

        int _EqpStat = 0;
        int _AlarmCode = 0;
        int _SubStat = 0;

        bool bFixStat = false;


        private void btnEqpStep1_Click(object sender, EventArgs e)
        {
            if (_AlarmCode != 0)
            {
                _plcEqpStat.Info("You cannot start while the alarm is going off.");
                return;
            }

            if (TimerUserStop.Enabled)
            {
                _plcEqpStat.Info("You need to select SubState first.");
                return;
            }
            if (_bLotRunning) SetEqpStat(1);
            else
            {
                _plcEqpStat.Info("It's a wait because the lot is not running.");
                SetEqpStat(2);
            }

        }

        private void SetEqpStat(int EqpStat, int SubStat = 0)
        {
            if (bFixStat)
            {
                _EqpStat = (int)EqpStat;
                _SubStat = (int)SubStat;
                return;
            }

            switch (EqpStat)
            {
                case 1:
                    lbEqpStat.Text = "RUN";
                    lbEqpStat.BackColor = Color.Green;
                    break;
                case 2:
                    lbEqpStat.Text = "WAIT";
                    lbEqpStat.BackColor = Color.Yellow;
                    break;
                case 4:
                    lbEqpStat.Text = "TROUBLE";
                    lbEqpStat.BackColor = Color.Red;
                    break;
                case 8:
                default:

                    if (SubStat == 104)
                    {
                        lbEqpStat.Text = "공사";
                    }
                    else
                        lbEqpStat.Text = "USER STOP";
                    lbEqpStat.BackColor = Color.Blue;

                    break;

            }

            if (_EqpStat != EqpStat)
            {
                _EqpStat = (int)EqpStat;
                _plcEqpStat.EqpWrite("Equipment State", _EqpStat);
            }
            if (_SubStat != SubStat)
            {
                _SubStat = (int)SubStat;
                _plcEqpStat.EqpWrite("Equipment Substate", _SubStat);

                if (SubStat == 104)
                {
                    bFixStat = true;
                    _plcEqpStat.Info("The equipment status is fixed and does not change.");
                }
            }

        }

        private void SetAlarm(int alarmCode)
        {
            if (_AlarmCode != alarmCode)
            {
                if (alarmCode == 0)
                {
                    _plcEqpStat.Event($"Alarm Reset Report - (Code:{_AlarmCode})");
                }
                else
                {
                    _plcEqpStat.Event($"Alarm Set Report - (Code:{alarmCode})");
                }

                _AlarmCode = (int)alarmCode;
                _plcEqpStat.EqpWrite("Trouble Code", _AlarmCode);
            }
        }

        private void btnEqpStep2_Click(object sender, EventArgs e)
        {
            if (_EqpStat > 2) return;

            SetEqpStat(8);

            if (numDelayTime.Value == 0)
            {
                _plcEqpStat.Info("The User Stop Delay Time is set to 0, so the popup activates immediately.");
                using (var popup = new SubStat())
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                        SetEqpStat(_EqpStat, popup.iSelectValue);


                    TimerUserStop.Enabled = false;
                }
            }
            else
                TimerUserStop.Enabled = true;

        }

        private void btnEqpStep3_Click(object sender, EventArgs e)
        {

            AddAlarmSetCode(1234);

        }

        private void btnEqpStep4_Click(object sender, EventArgs e)
        {
            if (_EqpStat == 4)
            {
                _plcEqpStat.Info("All alarms have cleared, and the status switches to User Stop (BM:118).");
                SetEqpStat(8, 118);
            }

            SetAlarm(0);
        }

        private void lbEqpStat_Click(object sender, EventArgs e)
        {
            if (bFixStat)
            {
                bFixStat = false;

                _plcEqpStat.EqpWrite("Equipment State", _EqpStat);
                _plcEqpStat.EqpWrite("Equipment Substate", _SubStat);

                _plcEqpStat.Info("The lock state has been released, and the current state is being reported.");

                SetEqpStat(_EqpStat, _SubStat);
                return;
            }

            if (_EqpStat == 8)
            {
                using (var popup = new SubStat())
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                        SetEqpStat(_EqpStat, popup.iSelectValue);

                    TimerUserStop.Enabled = false;
                }
            }
        }

        int iUserStopDelayChk = 0;
        private void TimerUserStop_Tick(object sender, EventArgs e)
        {
            iUserStopDelayChk += 1;

            if (numDelayTime.Value * 60 < iUserStopDelayChk)
            {
                TimerUserStop.Enabled = false;

                _plcEqpStat.Event("The popup activates as the User Stop Delay Time has elapsed.");
                using (var popup = new SubStat())
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                        SetEqpStat(_EqpStat, popup.iSelectValue);

                }
            }
        }
        #endregion

        #region Alarm Set/Reset Report
        private PlcMonitorControl _plcAlarm;

        private bool _T3_Trigger = false;

        private void InitializeAlarmLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();


            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _plcAlarm = new PlcMonitorControl();
            _plcAlarm.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_plcAlarm);
            TimerCommchk.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (numAlarmSetCode.Value == 0) return;

            AddAlarmSetCode(Convert.ToInt32(numAlarmSetCode.Value));
        }

        private void AddAlarmSetCode(int code)
        {
            if (_EqpStat < 4)
            {
                _plcEqpStat.Info("The equipment stops due to an alarm and switches to Trouble.");
                SetEqpStat(4, _SubStat);
            }

            if (lstAlarmSet.Items.Contains(code) == false)
            {
                lstAlarmSet.Items.Add(code);

                _plcAlarm.Event($"Alarm (Code : {code}) triggered");
                SetAlarm(code);
            }
        }

        int iReasonCode = 0;
        private void TimerAlarmSet_Tick(object sender, EventArgs e)
        {
            if (lstAlarmSet.Items.Count > 0)
            {
                if (!_bCommChk)
                {
                    if (iReasonCode != 1)
                    {
                        iReasonCode = 1;
                        _plcAlarm.Error($"Condition not met - [ COMM OFF ]");
                    }

                    return;
                }
                else if (_bItPass)
                {
                    if (iReasonCode != 2)
                    {
                        iReasonCode = 2;
                        _plcAlarm.Error($"Condition not met - [ IT PASS ]");
                    }

                    return;
                }
                else if (_T3_Trigger)
                {
                    if (iReasonCode != 3)
                    {
                        iReasonCode = 3;
                        _plcAlarm.Error($"Condition not met - [ Trigger On ]");
                    }

                    return;
                }

                AlarmSetReport(Convert.ToInt32(lstAlarmSet.Items[0]));
            }
            else if (lstAlarmReset.Items.Count > 0)
            {
                if (!_bCommChk)
                {
                    if (iReasonCode != 1)
                    {
                        iReasonCode = 1;
                        _plcAlarm.Error($"Condition not met - [ COMM OFF ]");
                    }

                    return;
                }
                else if (_bItPass)
                {
                    if (iReasonCode != 2)
                    {
                        iReasonCode = 2;
                        _plcAlarm.Error($"Condition not met - [ IT PASS ]");
                    }

                    return;
                }
                else if (_T3_Trigger)
                {
                    if (iReasonCode != 3)
                    {
                        iReasonCode = 3;
                        _plcAlarm.Error($"Condition not met - [ Trigger On ]");
                    }

                    return;
                }

                AlarmReSetReport(Convert.ToInt32(lstAlarmReset.Items[0]));
            }
        }


        private void AlarmSetReport(int code)
        {
            _plcAlarm.EqpWrite("Set Alarm ID", code);
            _plcAlarm.EqpWrite("Alarm Set Report", true);
            _plcAlarm.Info($"Confirm signal waiting");

            lstAlarmSet.Items.Remove(code);
            Thread.Sleep(1000);

            _plcAlarm.HostWrite("Alarm Set Confirm", true);
            _plcAlarm.Info($"Actions after receiving the host signal");
            _plcAlarm.EqpWrite("Alarm Set Report", false);
            _plcAlarm.EqpWrite("Set Alarm ID", 0);

            Thread.Sleep(1000);

            _plcAlarm.HostWrite("Alarm Reet Confirm", false);
            _plcAlarm.Info($"Host Report Completed");
        }

        private void AlarmReSetReport(int code)
        {
            _plcAlarm.EqpWrite("Reset Alarm ID", code);
            _plcAlarm.EqpWrite("Alarm Reset Report", true);
            _plcAlarm.Info($"Confirm signal waiting");

            lstAlarmReset.Items.Remove(code);
            Thread.Sleep(1000);

            _plcAlarm.HostWrite("Alarm Reset Confirm", true);
            _plcAlarm.Info($"Actions after receiving the host signal");
            _plcAlarm.EqpWrite("Alarm Reset Report", false);
            _plcAlarm.EqpWrite("Reset Alarm ID", 0);

            Thread.Sleep(1000);

            _plcAlarm.HostWrite("Alarm Reset Confirm", false);
            _plcAlarm.Info($"Host Report Completed");
        }

        private void btnAlarmStep2_Click(object sender, EventArgs e)
        {
            if (numAlarmResetCode.Value == 0) return;

            AddAlarmResetCode(Convert.ToInt32(numAlarmResetCode.Value));
        }
        private void AddAlarmResetCode(int code)
        {
            if (lstAlarmReset.Items.Contains(code) == false)
            {
                lstAlarmReset.Items.Add(code);
                _plcAlarm.Event($"Alarm (Code : {code}) triggered");
            }
        }

        #endregion

        #region Host Message Send
        private PlcMonitorControl _plcHostMsg;

        private void InitializeHostMsgLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();


            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 10 * 6;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _plcHostMsg = new PlcMonitorControl();
            _plcHostMsg.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_plcHostMsg);

            AddHostMsg(1, "(12345) Auto Length Detect Cycle Alarm");
            AddHostMsg(0, "Lot Infomation Validation Error(H2DKJAJSSD)");
        }

        private void AddHostMsg(int System, string message)
        {
            if (gridHostMsg.InvokeRequired)
            {
                gridHostMsg.BeginInvoke(
                    new Action(() => AddHostMsg(System, message)));
                return;
            }

            string strSystem = string.Empty;
            switch (System)
            {
                case 0:
                    strSystem = "MES(RMS)";
                    break;
                case 1:
                    strSystem = "APC";
                    break;
                case 2:
                    strSystem = "FDC";
                    break;
                case 3:
                    strSystem = "SPC+";
                    break;
                default:
                    return;
            }

            gridHostMsg.Rows.Insert(
                0,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                strSystem,
                message
            );

            if (gridHostMsg.Rows.Count > 100)
                gridHostMsg.Rows.RemoveAt(100);
        }

        private void btnHostMsgStep1_Click(object sender, EventArgs e)
        {

            _plcHostMsg.HostWrite("Alarm Send System", NumSystem.Value);
            _plcHostMsg.HostWrite("EQP Processing Stop Type", numStopType.Value);
            _plcHostMsg.HostWrite("Host Alarm Msg", txtHostMessage.Text);
            _plcHostMsg.HostWrite("Host Alarm Msg Send", true);


            AddHostMsg(Convert.ToInt32(NumSystem.Value), txtHostMessage.Text);

            if (numStopType.Value != 0)
            {
                _plcHostMsg.Info("A heavy alarm is triggered because of the Stop Type");

                switch (NumSystem.Value)
                {
                    case 0:
                        _plcHostMsg.Event("MES(RMS) Heavy alarm triggered - (Code: 101)");
                        AddAlarmSetCode(101);
                        break;
                    case 1:
                        _plcHostMsg.Event("APC Heavy alarm triggered - (Code: 102)");
                        AddAlarmSetCode(102);
                        break;
                    case 2:
                        _plcHostMsg.Event("FDC Heavy alarm triggered - (Code: 103)");
                        AddAlarmSetCode(103);
                        break;
                    case 3:
                        _plcHostMsg.Event("SPC+ Heavy alarm triggered - (Code: 104)");
                        AddAlarmSetCode(104);
                        break;
                    default:
                        return;
                }

            }

            Thread.Sleep(500);
            _plcHostMsg.HostWrite("Host Alarm Msg Send", false);

        }
        #endregion

        #region Material Management
        private PlcMonitorControl _PlcMtrl;


        private void InitializeCarrierLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();

            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _PlcMtrl = new PlcMonitorControl();
            _PlcMtrl.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_PlcMtrl);

            rbMtrlOutType1.Checked = true;
        }

        #region Mtrl
        private void button1_Click(object sender, EventArgs e)
        {
            string CSTID = txtMtrlID.Text;

            if (GetStat(lbMtrlJobStat))
            {
                _plcAlarm.Error($"You can report after the job end.");
                return;
            }

            string TRIGER = "Material ID Confirm Request_01";
            string CONF = "Material ID Confirm Send_01";
            string ACK = "Material ID Confirm Ack_01";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcMtrl.EqpWrite("Confirm Material ID_01", CSTID);
            _PlcMtrl.EqpWrite("Material Location_01", 11);
            _PlcMtrl.EqpWrite(TRIGER, true);
            _PlcMtrl.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckMtrl_TimeOut.Checked)
            {
                _PlcMtrl.Info($"If there is no response for 30 seconds, it times out.");

                _PlcMtrl.EqpWrite(TRIGER, false);
                _PlcMtrl.Info($"{TRIGER} Report Completed");

                _PlcMtrl.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 6.08]");
                AddAlarmSetCode(10000);
                return;
            }

            if (ckMtrl_NG.Checked)
            {
                _PlcMtrl.HostWrite(ACK, 11);
                _PlcMtrl.HostWrite(CONF, true);

                _PlcMtrl.Info($"Since it is an ACK-11 NG, clear data");

                _PlcMtrl.EqpWrite("Confirm Material ID_01", "");
                _PlcMtrl.EqpWrite("Material Location_01", "0");

                _PlcMtrl.EqpWrite(TRIGER, false);

                txtMtrlID.Text = "";
                txtMtrlWip.Text = "";
                SetLabelBackColor(lbMtrlIdStat, false);
                SetLabelBackColor(lbMtrlJobStat, false);

                Thread.Sleep(100);

                _PlcMtrl.HostWrite(CONF, false);
                _PlcMtrl.Info($"{TRIGER} Report Completed");

                return;

                //using (var popup = new NgSelect())
                //{
                //    if (popup.ShowDialog() == DialogResult.OK)
                //    {
                //        ckMtrl_NG.Checked = false;

                //        switch (popup.iSelectValue)
                //        {
                //            case 1:
                //                txtMtrlID.Text = popup.strValue;
                //                button1_Click(null, null);
                //                break;
                //            case 2:
                //                button1_Click(null, null);
                //                break;
                //        }

                //    }
                //}
            }
            else
            {
                //DATA
                _PlcMtrl.HostWrite("Material Initial WIP Qty_01", 5000);
                _PlcMtrl.HostWrite(ACK, 10);
                _PlcMtrl.HostWrite(CONF, true);

                _PlcMtrl.Info($"Actions after receiving the host signal");
                _PlcMtrl.EqpWrite("Initial WIP Qty", 5000);
                _PlcMtrl.EqpWrite(TRIGER, false);


                txtMtrlID.Text = CSTID;
                txtMtrlWip.Text = "5000";
                SetLabelBackColor(lbMtrlIdStat, true);
                SetLabelBackColor(lbMtrlJobStat, false);

                Thread.Sleep(100);

                _PlcMtrl.HostWrite(CONF, false);
                _PlcMtrl.Info($"{TRIGER} Report Completed");

                Mtrl_JobStart();

            }

        }

        private void Mtrl_JobStart()
        {
            string TRIGER = "Material Job Start Report_01";
            string CONF = "Material Job Start Confirm_01";
            string ACK = "Material Job Start Confirm Ack_01";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            _PlcMtrl.EqpWrite(TRIGER, true);
            _PlcMtrl.Info($"Confirm signal waiting");

            if (ckMtrl_TimeOut.Checked)
            {
                _PlcMtrl.Info($"If there is no response for 30 seconds, it times out.");

                _PlcMtrl.EqpWrite(TRIGER, false);
                _PlcMtrl.Info($"{TRIGER} Report Completed");

                _PlcMtrl.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 6.09]");
                AddAlarmSetCode(10000);

                return;
            }

            Thread.Sleep(100);

            _PlcMtrl.HostWrite(ACK, 10);
            _PlcMtrl.HostWrite(CONF, true);

            _PlcMtrl.Info($"Actions after receiving the host signal");
            _PlcMtrl.EqpWrite(TRIGER, false);

            SetLabelBackColor(lbMtrlJobStat, true);

            Thread.Sleep(100);

            _PlcMtrl.HostWrite(CONF, false);
            _PlcMtrl.Info($"{TRIGER} Report Completed");

            txtMtrlID.ReadOnly = true;
        }

        private void UW1_JobEnd(int EndType = 1)
        {
            if (EndLotType != 0) EndType = EndLotType;

            string TRIGER = "Material Job End Report_01";
            string CONF = "Material Job End Confirm_01";

            _PlcMtrl.EqpWrite("Material Work Qty", 1234);
            _PlcMtrl.EqpWrite("Material Job End Type", EndType);

            if (EndType == 1)
            {
                _PlcMtrl.EqpWrite("Material Remaining Qty", 0);
            }
            else
            {
                _PlcMtrl.EqpWrite("Material Remaining Qty", "Initial WIP Qty - Work Qty");
            }

            _PlcMtrl.EqpWrite(TRIGER, true);
            _PlcMtrl.Info($"Confirm signal waiting");

            if (ckMtrl_TimeOut.Checked)
            {
                _PlcMtrl.Info($"If there is no response for 30 seconds, it times out.");

                _PlcMtrl.EqpWrite(TRIGER, false);
                _PlcMtrl.Info($"{TRIGER} Report Completed");

                _PlcMtrl.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 6.10]");
                AddAlarmSetCode(10000);

                return;
            }

            Thread.Sleep(100);

            _PlcMtrl.HostWrite(CONF, true);
            _PlcMtrl.Info($"Actions after receiving the host signal");
            _PlcMtrl.EqpWrite(TRIGER, false);

            SetLabelBackColor(lbMtrlIdStat, false);
            SetLabelBackColor(lbMtrlJobStat, false);

            Thread.Sleep(100);

            _PlcMtrl.HostWrite(CONF, false);
            _PlcMtrl.Info($"{TRIGER} Report Completed");

            txtMtrlID.ReadOnly = false;

        }

        private void MTRL_Output()
        {
            string TRIGER = "Material Output Report_01";
            string CONF = "Carrier Output Confirm_01";

            _PlcMtrl.EqpWrite(TRIGER, true);
            _PlcMtrl.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckMtrl_TimeOut.Checked)
            {
                _PlcMtrl.Info($"If there is no response for 30 seconds, it times out.");

                _PlcMtrl.EqpWrite(TRIGER, false);
                _PlcMtrl.Info($"{TRIGER} Report Completed");

                _PlcMtrl.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 6.11]");
                AddAlarmSetCode(10000);
                return;
            }

            _PlcMtrl.Info($"Actions after receiving the host signal");

            _PlcMtrl.HostWrite(CONF, true);
            _PlcMtrl.EqpWrite(TRIGER, false);

            _PlcMtrl.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcMtrl.EqpWrite("Material ID", "");
            _PlcMtrl.EqpWrite("Material Work Qty", "");
            _PlcMtrl.EqpWrite("Material Initial WIP Qty", 0);
            _PlcMtrl.EqpWrite("Material Work Qty", 0);
            _PlcMtrl.EqpWrite("Material Fixed Loss", 0);
            _PlcMtrl.EqpWrite("Material Output Type", 0);

            _PlcMtrl.EqpWrite(TRIGER, false);

            txtMtrlID.Text = "";
            txtMtrlWip.Text = "";
            SetLabelBackColor(lbMtrlIdStat, false);
            SetLabelBackColor(lbMtrlJobStat, false);

            Thread.Sleep(100);

            _PlcMtrl.HostWrite(CONF, false);
            _PlcMtrl.Info($"{TRIGER} Report Completed");
            txtMtrlID.ReadOnly = false;
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            _PlcMtrl.Info("It is not triggered by HMI operation, but occurs when the specified conditions are met.");
        }

        private void btnMtrlStep3_Click(object sender, EventArgs e)
        {
            if (GetStat(lbMtrlJobStat))
            {
                UW1_JobEnd(rbMtrlOutType1.Checked ? 0 : 1);
            }

        }

        private void btnMtrlStep4_Click(object sender, EventArgs e)
        {
            MTRL_Output();
        }
        #endregion

        private void SetLabelBackColor(Label lb, bool val)
        {
            if (!val) lb.BackColor = Color.Gray;
            else lb.BackColor = Color.YellowGreen;
        }

        private bool GetStat(Label lb)
        {
            return lb.BackColor == Color.YellowGreen ? true : false;
        }

        #endregion

        #region Carrier - IN
        private PlcMonitorControl _PlcRUCarrier;

        bool RU1_ID, RU2_ID;

        private void InitializeCarrierRWLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();

            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _PlcRUCarrier = new PlcMonitorControl();
            _PlcRUCarrier.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_PlcRUCarrier);

        }

        private void btnRWCarrier1_Click(object sender, EventArgs e)
        {
            string CSTID = txtInCarrierID.Text;

            string TRIGER = "Carrier ID Report_01";
            string CONF = "Carrier ID Confirm_01";
            string ACK = "Carrier ID Confirm Ack_01";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcRUCarrier.EqpWrite("LOC-101 Carrier ID", CSTID);
            _PlcRUCarrier.EqpWrite("LOC-101 Carrier ID Reading Type", 4);
            _PlcRUCarrier.EqpWrite(TRIGER, true);
            _PlcRUCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckRU1_TimeOut.Checked)
            {
                _PlcRUCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcRUCarrier.EqpWrite(TRIGER, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

                _PlcRUCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.00]");
                AddAlarmSetCode(10001);

                return;
            }

            if (ckRU1_NG.Checked)
            {
                _PlcRUCarrier.HostWrite(ACK, 11);
                _PlcRUCarrier.HostWrite(CONF, true);

                _PlcRUCarrier.Info($"Since it is an ACK-11 NG, clear data");
                _PlcRUCarrier.EqpWrite("LOC-101 Carrier ID", "");
                _PlcRUCarrier.EqpWrite("LOC-101 Initial WIP Qty", 0);
                _PlcRUCarrier.EqpWrite("LOC-101 Carrier-Lot ID", "");
                _PlcRUCarrier.EqpWrite(TRIGER, false);

                txtInCarrierID.Text = "";
                txtInCarrierLot.Text = "";
                txtInCarrierWork.Text = "0";
                txtInCarrierWip.Text = "0";
                RU1_ID = false;
                SetLabelBackColor(lbInCarrierIdStat, RU1_ID);

                Thread.Sleep(100);

                _PlcRUCarrier.HostWrite(CONF, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

                using (var popup = new NgSelect())
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        ckRU1_NG.Checked = false;

                        switch (popup.iSelectValue)
                        {
                            case 1:
                                txtInCarrierID.Text = popup.strValue;
                                btnRWCarrier1_Click(null, null);
                                break;
                            case 2:
                                btnRWCarrier1_Click(null, null);
                                break;
                            case 3:
                                InCarrier_JobEnd(4);
                                break;
                        }

                    }
                }
            }
            else
            {
                //DATA

                string carrlot = "INCARR" + RANDOM;
                string wip = "2";
                _PlcRUCarrier.HostWrite("LOC-101 Initial WIP Qty", wip);
                _PlcRUCarrier.HostWrite("LOC-101 Carrier-Lot ID", carrlot);

                _PlcRUCarrier.HostWrite(ACK, 10);
                _PlcRUCarrier.HostWrite(CONF, true);

                _PlcRUCarrier.Info($"Actions after receiving the host signal");

                _PlcRUCarrier.EqpWrite("LOC-101 Carrier-Lot ID", false);
                _PlcRUCarrier.EqpWrite("LOC-101 Initial WIP Qty", 2);

                txtInCarrierLot.Text = carrlot;
                txtInCarrierWork.Text = "0";
                txtInCarrierWip.Text = wip;

                _PlcRUCarrier.EqpWrite(TRIGER, false);

                SetLabelBackColor(lbInCarrierIdStat, true);

                Thread.Sleep(100);

                _PlcRUCarrier.HostWrite(CONF, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

                IN_Carrier_JobStart();

            }
        }

        private void IN_Carrier_JobStart()
        {
            string TRIGER = "Carrier Job Start Report_01";
            string CONF = "Carrier Job Start Confirm_01";
            string ACK = "Carrier Job Start Confirm Ack_01";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            _PlcRUCarrier.EqpWrite(TRIGER, true);
            _PlcRUCarrier.Info($"Confirm signal waiting");

            if (ckRU1_TimeOut.Checked)
            {
                _PlcRUCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcRUCarrier.EqpWrite(TRIGER, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

                _PlcRUCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.08]");
                AddAlarmSetCode(10000);

                return;
            }

            Thread.Sleep(100);

            _PlcRUCarrier.HostWrite(ACK, 10);
            _PlcRUCarrier.HostWrite(CONF, true);

            _PlcRUCarrier.Info($"Actions after receiving the host signal");
            _PlcRUCarrier.EqpWrite(TRIGER, false);

            SetLabelBackColor(lbInCarrierJobStat, true);

            Thread.Sleep(100);

            _PlcRUCarrier.HostWrite(CONF, false);
            _PlcRUCarrier.Info($"{TRIGER} Report Completed");

            txtInCarrierID.ReadOnly = true;
        }


        private void btnInCarrier2_Click(object sender, EventArgs e)
        {
            if (GetStat(lbInCarrierJobStat))
            {
                InCarrier_JobEnd(InCarrierOutType);
            }
        }

        private void InCarrier_JobEnd(int EndType = 1)
        {
            if (EndLotType != 0) EndType = EndLotType;

            string TRIGER = "Carrier Job End Report_01";
            string CONF = "Carrier Job End Confirm_01";

            _PlcRUCarrier.EqpWrite("LOC-101 Work Qty", 1234);
            _PlcRUCarrier.EqpWrite("LOC-101 Job End Type", EndType);

            if (EndType == 1)
            {
                _PlcRUCarrier.EqpWrite("LOC-101 Remaining Qty", 0);
                _PlcRUCarrier.EqpWrite("LOC-101 Previous Input Loss", "Initial WIP Qty - Input Qty - Fixed Loss Qty");
            }
            else
            {
                _PlcRUCarrier.EqpWrite("LOC-101 Remaining Qty", "Initial WIP Qty - Work Qty");
                _PlcRUCarrier.EqpWrite("LOC-101 Previous Input Loss", 0);
            }

            _PlcRUCarrier.EqpWrite(TRIGER, true);
            _PlcRUCarrier.Info($"Confirm signal waiting");

            if (ckMtrl_TimeOut.Checked)
            {
                _PlcRUCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcRUCarrier.EqpWrite(TRIGER, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

                _PlcRUCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 6.10]");
                AddAlarmSetCode(10000);

                return;
            }

            Thread.Sleep(100);

            _PlcRUCarrier.HostWrite(CONF, true);
            _PlcRUCarrier.Info($"Actions after receiving the host signal");
            _PlcRUCarrier.EqpWrite(TRIGER, false);

            SetLabelBackColor(lbInCarrierIdStat, false);
            SetLabelBackColor(lbInCarrierJobStat, false);

            Thread.Sleep(100);

            _PlcRUCarrier.HostWrite(CONF, false);
            _PlcRUCarrier.Info($"{TRIGER} Report Completed");

            txtInCarrierID.ReadOnly = false;

        }

        int InCarrierOutType = 0;

        private void lbInCarrierOutType2_Click(object sender, EventArgs e)
        {
            if (sender is not Label lb)
                return;

            Panel panel = lb.Parent as Panel;

            if (panel == null)
                return;

            foreach (Label label in panel.Controls.OfType<Label>())
            {
                label.BackColor = Color.Yellow;
                label.ForeColor = Color.Black;
            }

            lb.BackColor = Color.Black;
            lb.ForeColor = Color.Yellow;

            InCarrierOutType = Convert.ToInt32(lb.Tag);
        }

        private void btnInCarrier3_Click(object sender, EventArgs e)
        {
            InCarrier_Out();
        }


        private void InCarrier_Out()
        {
            string TRIGER = "Carrier Output Report_01";
            string CONF = "Carrier Output Confirm_01";

            _PlcRUCarrier.EqpWrite(TRIGER, true);
            _PlcRUCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcRUCarrier.Info($"Actions after receiving the host signal");

            _PlcRUCarrier.HostWrite(CONF, true);
            _PlcRUCarrier.EqpWrite(TRIGER, false);

            _PlcRUCarrier.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcRUCarrier.EqpWrite("LOC-101 Carrier ID", "");
            _PlcRUCarrier.EqpWrite("LOC-101 Product ID", "");
            _PlcRUCarrier.EqpWrite("LOC-101 Carrier-Lot ID", "");
            _PlcRUCarrier.EqpWrite("LOC-101 Initial WIP Qty", 0);
            _PlcRUCarrier.EqpWrite("LOC-101 Work Qty", 0);
            _PlcRUCarrier.EqpWrite("LOC-101 Remaining Qty", 0);
            _PlcRUCarrier.EqpWrite("LOC-101 Previous Input Loss", 0);
            _PlcRUCarrier.EqpWrite("LOC-101 Job End Type", 0);

            _PlcRUCarrier.EqpWrite(TRIGER, false);

            txtInCarrierID.Text = "";
            txtInCarrierLot.Text = "";
            RU1_ID = false;
            SetLabelBackColor(lbInCarrierIdStat, RU1_ID);

            Thread.Sleep(100);

            _PlcRUCarrier.HostWrite(CONF, false);
            _PlcRUCarrier.Info($"{TRIGER} Report Completed");

            txtInCarrierID.ReadOnly = false;
        }
        #endregion

        #region Carrier - RL
        private PlcMonitorControl _PlcOutCarrier;

        private void InitializeCarrierRLLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();

            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _PlcOutCarrier = new PlcMonitorControl();
            _PlcOutCarrier.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_PlcOutCarrier);

        }


        #endregion

        #region Lot Control
        private PlcMonitorControl _PlcLotControl;


        private void InitializeLotControlLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();

            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _PlcLotControl = new PlcMonitorControl();
            _PlcLotControl.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_PlcLotControl);
        }

        bool bPassPopup = false;
        private void btnLotStart_Click(object sender, EventArgs e)
        {

            #region 조건 체크
            if (_bLotRunning)
            {
                _PlcLotControl.Error("Lot Running, so I can't start Lot.");
                return;
            }


            #endregion


            if (bPassPopup)
            {
                _PlcLotControl.EqpWrite("Current Work Type", 1);
                txtWorkType.Text = "1";
            }
            else
            {
                using (var popup = new CurrentWorkType())
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        _PlcLotControl.EqpWrite("Current Work Type", popup.iSelectValue);
                        txtWorkType.Text = popup.iSelectValue.ToString();
                    }
                    else return;
                }
            }

            string CSTID = txtRL1Carrier.Text;

            string TRIGER = "Lot Start Report";
            string CONF = "Lot Start Confirm";
            string ACK = "Lot Start Confirm ACK";
            string RANDOM = DateTime.Now.ToString("HHmmss");


            _PlcLotControl.Event("##### LOT START REPORT STRART #####");
            _PlcLotControl.EqpWrite(TRIGER, true);
            _PlcLotControl.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckOutCarrier_TimeOut.Checked)
            {
                _PlcLotControl.Info($"If there is no response for 30 seconds, it times out.");

                _PlcLotControl.EqpWrite(TRIGER, false);
                _PlcLotControl.Info($"{TRIGER} Report Completed");

                _PlcLotControl.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 1.02]");
                AddAlarmSetCode(10004);

                return;
            }

            if (ckOutCarrier_NG.Checked)
            {
                _PlcLotControl.HostWrite(ACK, 11);
                _PlcLotControl.HostWrite(CONF, true);

                _PlcLotControl.Info($"Since it is an ACK-11 NG, clear data");
                _PlcLotControl.EqpWrite(TRIGER, false);

                Thread.Sleep(100);

                _PlcLotControl.HostWrite(CONF, false);
                _PlcLotControl.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA

                _PlcLotControl.HostWrite(ACK, 10);

                _PlcLotControl.HostWrite("Product ID", "PROD" + RANDOM);
                _PlcLotControl.HostWrite("Upper Lot ID", "UPPER" + RANDOM);
                _PlcLotControl.HostWrite("Lower Lot ID", "LOWER" + RANDOM);
                _PlcLotControl.HostWrite("QA Material", true);
                _PlcLotControl.HostWrite(CONF, true);

                _PlcLotControl.Info($"Actions after receiving the host signal");


                if (rbRL1.Checked)
                {
                    _PlcLotControl.EqpWrite("RL1 Carrier-Lot ID", "LOWER" + RANDOM);
                    txtRL1CarrierLot.Text = "LOWER" + RANDOM;
                }
                else
                {
                    _PlcLotControl.EqpWrite("RL2 Carrier-Lot ID", "LOWER" + RANDOM);
                    txtRL2CarrierLot.Text = "LOWER" + RANDOM;
                }
                txtLowerLot.Text = "LOWER" + RANDOM;

                _bLotRunning = true;
                SetLabelBackColor(lbLotRunning, _bLotRunning);

                ckQA.Checked = true;


                _PlcLotControl.EqpWrite(TRIGER, false);


                Thread.Sleep(100);

                _PlcLotControl.HostWrite(CONF, false);
                _PlcLotControl.Info($"{TRIGER} Report Completed");

                bPassPopup = false;

            }
        }


        private void LotEnd(bool bCut = false)
        {

            if (txtWorkType.Text.Equals("1"))
            {
                using (var popup = new EndWorkType())
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        _PlcLotControl.EqpWrite("Lot End Work Type", popup.iSelectValue);
                        txtWorkType.Text = popup.iSelectValue.ToString();
                    }
                    else return;
                }
            }

            string CSTID = txtRL1Carrier.Text;

            string TRIGER = "Lot End Report";
            string CONF = "Lot End Confirm";
            string ACK = "Lot End Confirm ACK";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            _PlcLotControl.EqpWrite($"Cut", bCut);
            _PlcLotControl.EqpWrite(TRIGER, true);
            _PlcLotControl.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckOutCarrier_TimeOut.Checked)
            {
                _PlcLotControl.Info($"If there is no response for 30 seconds, it times out.");

                _PlcLotControl.EqpWrite(TRIGER, false);
                _PlcLotControl.Info($"{TRIGER} Report Completed");

                _PlcLotControl.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 1.03]");
                AddAlarmSetCode(10004);

                return;
            }

            //DATA

            _PlcLotControl.HostWrite(CONF, true);

            _PlcLotControl.Info($"Actions after receiving the host signal");


            txtUpperLot.Text = "";
            txtLowerLot.Text = "";
            txtWorkType.Text = "";

            _bLotRunning = false;
            SetLabelBackColor(lbLotRunning, _bLotRunning);
            ckQA.Checked = false;

            _PlcLotControl.EqpWrite(TRIGER, false);


            Thread.Sleep(100);

            _PlcLotControl.HostWrite(CONF, false);
            _PlcLotControl.Info($"{TRIGER} Report Completed");


            EndLotType = 0;

        }

        private void btnLotEnd3_Click(object sender, EventArgs e)
        {
            if (_bLotRunning == false)
            {
                _PlcLotControl.Error("The lot is not in running state.");
                return;
            }


            _PlcLotControl.Event("##### LOT END REPORT STRART #####");
            LotEnd();
        }

        private void btnLotEnd2_Click(object sender, EventArgs e)
        {
            #region 조건체크
            if (_bLotRunning == false)
            {
                _PlcLotControl.Error("The lot is not in running state.");
                return;
            }

            #endregion

            using (var popup = new LotEndTypeCut())
            {
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    _PlcLotControl.EqpWrite("Lot End Output Type", popup.iSelectValue);
                    EndLotType = popup.iSelectValue;
                }
                else return;
            }

            _PlcLotControl.Event("##### LOT END REPORT STRART #####");
            LotEnd(true);


            _PlcLotControl.Event("##### RU Core Change #####");


            _PlcLotControl.Event("##### RL Core Change #####");
            if (rbRL1.Checked) rbRL2.Checked = true;
            else rbRL1.Checked = true;

            _PlcLotControl.Event("##### UW ID Report #####");
            button1_Click(null, null);

            bPassPopup = true;
            _PlcLotControl.Event("##### Next Lot Start #####");
            btnLotStart_Click(btnLotStart, null);

        }
        int EndLotType = 0;
        private void btnLotEnd1_Click(object sender, EventArgs e)
        {
            #region 조건체크
            if (_bLotRunning == false)
            {
                _PlcLotControl.Error("The lot is not in running state.");
                return;
            }
            #endregion

            using (var popup = new LotEndTypeCut())
            {
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    _PlcLotControl.EqpWrite("Lot End Output Type", popup.iSelectValue);
                    EndLotType = popup.iSelectValue;
                }
                else return;
            }



            _PlcLotControl.Event("##### LOT END REPORT STRART #####");
            LotEnd(true);


            _PlcLotControl.Event("##### RU Core Change #####");

            _PlcLotControl.Event("##### RL Core Change #####");
            if (rbRL1.Checked) rbRL2.Checked = true;
            else rbRL1.Checked = true;

            bPassPopup = true;
            _PlcLotControl.Event("##### Next Lot Start #####");
            btnLotStart_Click(btnLotStart, null);
        }
        #endregion

        #region UBM
        private PlcMonitorControl _plcUBM;

        private void InitializeUBMLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();


            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _plcUBM = new PlcMonitorControl();
            _plcUBM.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_plcUBM);

        }



        private void btnUbmStep1_Click(object sender, EventArgs e)
        {
            string bcr = txtUbmID.Text;

            if (string.IsNullOrWhiteSpace(bcr) == false)
            {
                _plcUBM.Error($"UBM Parts is already in use");
                return;
            }

            string TRIGER = "UBM Parts Input Report";
            string CONF = "UBM Parts Input Confirm";
            string ACK = "UBM Parts Input Confirm Ack";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            _plcUBM.EqpWrite("UBM Parts ID", txtUBM_Bcr.Text);
            _plcUBM.EqpWrite("UBM Parts Input Location", 11);
            _plcUBM.EqpWrite(TRIGER, true);
            _plcUBM.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckUbmTimeOut.Checked)
            {
                _plcUBM.Info($"If there is no response for 30 seconds, it times out.");

                _plcUBM.EqpWrite(TRIGER, false);
                _plcUBM.Info($"{TRIGER} Report Completed");

                _plcUBM.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 2.06]");
                AddAlarmSetCode(10004);

                return;
            }

            if (ckUbmNg.Checked)
            {
                _plcUBM.HostWrite(ACK, 11);
                _plcUBM.HostWrite(CONF, true);

                _plcUBM.Info($"Since it is an ACK-11 NG, clear data");
                _plcUBM.EqpWrite("UBM Parts ID", "");
                _plcUBM.EqpWrite(TRIGER, false);

                txtUbmID.Text = "";
                txtUbmLimitQty.Text = "0";
                txtUbmUseQty.Text = "0";
                txtUbmWanningQty.Text = "0";

                Thread.Sleep(100);

                _plcUBM.HostWrite(CONF, false);
                _plcUBM.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA
                _plcUBM.HostWrite("UBM Parts ID", "UBM" + RANDOM);
                _plcUBM.HostWrite("Limit Usage Count", 10000);
                _plcUBM.HostWrite("Usage Count", 1234);
                _plcUBM.HostWrite("Limit Repair Count", 200);
                _plcUBM.HostWrite("Repair Count", 34);
                _plcUBM.HostWrite("UBM Parts Status", 1);
                _plcUBM.HostWrite("UBM Parts Model ID", "MODEL" + RANDOM);
                _plcUBM.HostWrite("Warning Usage Count", 9000);
                _plcUBM.HostWrite(ACK, 10);
                _plcUBM.HostWrite(CONF, true);

                _plcUBM.Info($"Actions after receiving the host signal");

                _plcUBM.EqpWrite("Current UBM Parts ID_01", "UBM" + RANDOM);
                _plcUBM.EqpWrite("Limit Usage Count", 10000);
                _plcUBM.EqpWrite("Current UBM Parts Usage Count_01", 1234);
                _plcUBM.EqpWrite(TRIGER, false);

                txtUbmID.Text = "UBM" + RANDOM;
                txtUbmLimitQty.Text = "10000";
                txtUbmUseQty.Text = "1234";
                txtUbmWanningQty.Text = "9000";

                Thread.Sleep(100);

                _plcUBM.HostWrite(CONF, false);
                _plcUBM.Info($"{TRIGER} Report Completed");

            }

        }

        private void btnUbmStep2_Click(object sender, EventArgs e)
        {
            if (txtUbmUseQty.Text.Equals("1234"))
            {
                txtUbmUseQty.Text = "9000";
                _plcUBM.Event("A light alarm is triggered when the warning quantity is reached");
            }
            else if (txtUbmUseQty.Text.Equals("9000"))
            {
                txtUbmUseQty.Text = "10000";
                _plcUBM.Event("A heavy alarm will be triggered if usage exceeds the limit");
            }
            //else txtUbmUseQty.Text = "1234";
        }

        int ubmOutType = 0;
        private void lbUbmOutType1_Click(object sender, EventArgs e)
        {
            if (sender is not Label lb)
                return;

            Panel panel = lb.Parent as Panel;

            if (panel == null)
                return;

            foreach (Label label in panel.Controls.OfType<Label>())
            {
                label.BackColor = Color.Yellow;
                label.ForeColor = Color.Black;
            }

            lb.BackColor = Color.Black;
            lb.ForeColor = Color.Yellow;

            ubmOutType = Convert.ToInt32(lb.Tag);
        }

        private void btnUbmStep3_Click(object sender, EventArgs e)
        {
            string bcr = txtUbmID.Text;

            if (string.IsNullOrWhiteSpace(bcr))
            {
                _plcUBM.Error($"The UBM parts are not installed.");
                return;
            }

            string TRIGER = "UBM Parts Output Report";
            string CONF = "UBM Parts Output Confirm";
            string ACK = "UBM Parts Output Confirm Ack";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            _plcUBM.EqpWrite("History UBM Parts ID", bcr);
            _plcUBM.EqpWrite("History Usage Count", txtUbmUseQty.Text);
            _plcUBM.EqpWrite("Reason Code", ubmOutType);
            _plcUBM.EqpWrite("UBM Parts Output Location", "11");
            _plcUBM.EqpWrite(TRIGER, true);
            _plcUBM.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckUbmTimeOut.Checked)
            {
                _plcUBM.Info($"If there is no response for 30 seconds, it times out.");

                _plcUBM.EqpWrite(TRIGER, false);
                _plcUBM.Info($"{TRIGER} Report Completed");

                _plcUBM.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 2.07]");
                AddAlarmSetCode(10004);

                return;
            }

            if (ckUbmNg.Checked)
            {
                _plcUBM.HostWrite(ACK, 11);
                _plcUBM.HostWrite(CONF, true);


                _plcUBM.EqpWrite(TRIGER, false);

                Thread.Sleep(100);

                _plcUBM.HostWrite(CONF, false);
                _plcUBM.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA
                _plcUBM.HostWrite(ACK, 10);
                _plcUBM.HostWrite(CONF, true);

                _plcUBM.Info($"Actions after receiving the host signal");

                _plcUBM.EqpWrite("Current UBM Parts ID_01", "");
                _plcUBM.EqpWrite("Limit Usage Count", "");
                _plcUBM.EqpWrite("Current UBM Parts Usage Count_01", "");
                _plcUBM.EqpWrite(TRIGER, false);

                txtUbmID.Text = "";
                txtUbmLimitQty.Text = "0";
                txtUbmUseQty.Text = "0";
                txtUbmWanningQty.Text = "0";

                Thread.Sleep(100);

                _plcUBM.HostWrite(CONF, false);
                _plcUBM.Info($"{TRIGER} Report Completed");

            }
        }

        #endregion


        private void Out_Carrier_JobStart()
        {
            string TRIGER = "Carrier Job Start Report_03";
            string CONF = "Carrier Job Start Confirm_03";
            string ACK = "Carrier Job Start Confirm Ack_03";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            _PlcOutCarrier.EqpWrite(TRIGER, true);
            _PlcOutCarrier.Info($"Confirm signal waiting");

            if (ckRU1_TimeOut.Checked)
            {
                _PlcOutCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcOutCarrier.EqpWrite(TRIGER, false);
                _PlcOutCarrier.Info($"{TRIGER} Report Completed");

                _PlcOutCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.10]");
                AddAlarmSetCode(10000);

                return;
            }

            Thread.Sleep(100);

            _PlcOutCarrier.HostWrite(ACK, 10);
            _PlcOutCarrier.HostWrite(CONF, true);

            _PlcOutCarrier.Info($"Actions after receiving the host signal");
            _PlcOutCarrier.EqpWrite(TRIGER, false);

            SetLabelBackColor(lbOutCarrierJobStat, true);

            Thread.Sleep(100);

            _PlcOutCarrier.HostWrite(CONF, false);
            _PlcOutCarrier.Info($"{TRIGER} Report Completed");

            txtOutCarrierID.ReadOnly = true;
        }

        private void OutCarrier_JobEnd(int EndType = 1)
        {
            if (EndLotType != 0) EndType = EndLotType;

            string TRIGER = "Carrier Job End Report_03";
            string CONF = "Carrier Job End Confirm_03";

            _PlcOutCarrier.EqpWrite("LOC-103 Work Qty", 1234);
            _PlcOutCarrier.EqpWrite("LOC-103 Job End Type", EndType);


            _PlcOutCarrier.EqpWrite(TRIGER, true);
            _PlcOutCarrier.Info($"Confirm signal waiting");

            if (ckOutCarrier_TimeOut.Checked)
            {
                _PlcOutCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcOutCarrier.EqpWrite(TRIGER, false);
                _PlcOutCarrier.Info($"{TRIGER} Report Completed");

                _PlcOutCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 4.08]");
                AddAlarmSetCode(10000);

                return;
            }

            Thread.Sleep(100);

            _PlcOutCarrier.HostWrite(CONF, true);
            _PlcOutCarrier.Info($"Actions after receiving the host signal");
            _PlcOutCarrier.EqpWrite(TRIGER, false);

            SetLabelBackColor(lbOutCarrierIdStat, false);
            SetLabelBackColor(lbOutCarrierJobStat, false);

            Thread.Sleep(100);

            _PlcOutCarrier.HostWrite(CONF, false);
            _PlcOutCarrier.Info($"{TRIGER} Report Completed");

            txtOutCarrierID.ReadOnly = false;

        }

        int OutCarrierOutType = 0;

        private void OutCarrier_Out()
        {
            string TRIGER = "Carrier Output Report_03";
            string CONF = "Carrier Output Confirm_03";

            _PlcOutCarrier.EqpWrite(TRIGER, true);
            _PlcOutCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcOutCarrier.Info($"Actions after receiving the host signal");

            _PlcOutCarrier.HostWrite(CONF, true);
            _PlcOutCarrier.EqpWrite(TRIGER, false);

            _PlcOutCarrier.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcOutCarrier.EqpWrite("LOC-103 Carrier ID", "");
            _PlcOutCarrier.EqpWrite("LOC-103 Product ID", "");
            _PlcOutCarrier.EqpWrite("LOC-103 Carrier-Lot ID", "");
            _PlcOutCarrier.EqpWrite("LOC-103 Work Qty", 0);
            _PlcOutCarrier.EqpWrite("LOC-103 Job End Type", 0);

            _PlcOutCarrier.EqpWrite(TRIGER, false);

            txtOutCarrierID.Text = "";
            txtOutCarrierLot.Text = "";
            txtOutCarrierWorkQty.Text = "0";

            SetLabelBackColor(lbOutCarrierIdStat, false);

            Thread.Sleep(100);

            _PlcOutCarrier.HostWrite(CONF, false);
            _PlcOutCarrier.Info($"{TRIGER} Report Completed");

            txtOutCarrierID.ReadOnly = false;
        }



        private void btnOutCarrier1_Click(object sender, EventArgs e)
        {
            string CSTID = txtOutCarrierID.Text;

            string TRIGER = "Carrier ID Report_03";
            string CONF = "Carrier ID Confirm_03";
            string ACK = "Carrier ID Confirm Ack_03";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcOutCarrier.EqpWrite("LOC-103 Carrier ID", CSTID);
            _PlcOutCarrier.EqpWrite("LOC-103 Carrier ID Reading Type", 4);
            _PlcOutCarrier.EqpWrite(TRIGER, true);
            _PlcOutCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckRU1_TimeOut.Checked)
            {
                _PlcOutCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcOutCarrier.EqpWrite(TRIGER, false);
                _PlcOutCarrier.Info($"{TRIGER} Report Completed");

                _PlcOutCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.03]");
                AddAlarmSetCode(10001);

                return;
            }

            if (ckRU1_NG.Checked)
            {
                _PlcOutCarrier.HostWrite(ACK, 11);
                _PlcOutCarrier.HostWrite(CONF, true);

                _PlcOutCarrier.Info($"Since it is an ACK-11 NG, clear data");
                _PlcOutCarrier.EqpWrite("LOC-103 Carrier ID", "");
                _PlcOutCarrier.EqpWrite("LOC-103 Carrier-Lot ID", ""); 
                _PlcOutCarrier.EqpWrite(TRIGER, false);

                txtOutCarrierID.Text = "";
                txtOutCarrierLot.Text = "";
                txtOutCarrierWorkQty.Text = "0";

                SetLabelBackColor(lbInCarrierIdStat, false);

                Thread.Sleep(100);

                _PlcOutCarrier.HostWrite(CONF, false);
                _PlcOutCarrier.Info($"{TRIGER} Report Completed");

                using (var popup = new NgSelect())
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        ckRU1_NG.Checked = false;

                        switch (popup.iSelectValue)
                        {
                            case 1:
                                txtOutCarrierID.Text = popup.strValue;
                                btnOutCarrier1_Click(null, null);
                                break;
                            case 2:
                                btnOutCarrier1_Click(null, null);
                                break;
                            case 3:
                                OutCarrier_JobEnd(4);
                                break;
                        }

                    }
                }
            }
            else
            {
                //DATA
                _PlcOutCarrier.HostWrite("LOC-103 Carrier-Lot ID", "OUTCARR" + RANDOM);

                _PlcOutCarrier.HostWrite(ACK, 10);
                _PlcOutCarrier.HostWrite(CONF, true);

                _PlcOutCarrier.Info($"Actions after receiving the host signal");

                txtOutCarrierLot.Text = "OUTCARR" + RANDOM;
                txtOutCarrierWorkQty.Text = "0";

                _PlcOutCarrier.EqpWrite(TRIGER, false);

                SetLabelBackColor(lbOutCarrierIdStat, true);

                Thread.Sleep(100);

                _PlcOutCarrier.HostWrite(CONF, false);
                _PlcOutCarrier.Info($"{TRIGER} Report Completed");

                Out_Carrier_JobStart();

            }
        }

        private void lbOutCarrierOutType5_Click(object sender, EventArgs e)
        {
            if (sender is not Label lb)
                return;

            Panel panel = lb.Parent as Panel;

            if (panel == null)
                return;

            foreach (Label label in panel.Controls.OfType<Label>())
            {
                label.BackColor = Color.Yellow;
                label.ForeColor = Color.Black;
            }

            lb.BackColor = Color.Black;
            lb.ForeColor = Color.Yellow;

            OutCarrierOutType = Convert.ToInt32(lb.Tag);
        }

        private void btnOutCarrier2_Click(object sender, EventArgs e)
        {
            if (GetStat(lbOutCarrierJobStat))
            {
                OutCarrier_JobEnd(OutCarrierOutType);
            }
        }

        private void btnOutCarrier3_Click(object sender, EventArgs e)
        {
            OutCarrier_Out();
        }
    }


}
