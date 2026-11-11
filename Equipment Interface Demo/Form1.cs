using Equipment_Interface_Demo.Popup;
using Microsoft.VisualBasic.ApplicationServices;
using System.Reflection.PortableExecutable;
using System.Security.Policy;
using System.Windows.Forms;
using static System.Windows.Forms.Design.AxImporter;

namespace Equipment_Interface_Demo
{
    public partial class Form1 : Form
    {
        bool _bLotRunning = false;
        bool _bItPass = false;

        public Form1()
        {
            InitializeComponent();

            InitializeCommChkLayout(tabCommChk);
            Initialize_plcEqpStatLayout(tabEqpStat);
            InitializeAlarmLayout(tabAlarm);
            InitializeHostMsgLayout(tabHostMsg);

            InitializeCarrierLayout(tabCarrier);
            InitializeCarrierRWLayout(tabCarrierRU);
            InitializeCarrierRLLayout(tabCarrierRL);

            InitializeLotControlLayout(tabLotControl);

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

        #region Carrier - UW
        private PlcMonitorControl _PlcUWCarrier;

        bool UW1_ID, UW1_JOB, UW2_ID, UW2_JOB;

        private void InitializeCarrierLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();

            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _PlcUWCarrier = new PlcMonitorControl();
            _PlcUWCarrier.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_PlcUWCarrier);

            rbUW1_1.Checked = true;
        }

        #region UW1
        private void button1_Click(object sender, EventArgs e)
        {
            string CSTID = txtUW1Carrier.Text;

            string TRIGER = "Carrier ID Report_01";
            string CONF = "Carrier ID Confirm_01";
            string ACK = "Carrier ID Confirm Ack_01";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcUWCarrier.EqpWrite("UW1 Carrier ID", CSTID);
            _PlcUWCarrier.EqpWrite("UW1 Carrier ID Reading Type", 3);
            _PlcUWCarrier.EqpWrite(TRIGER, true);
            _PlcUWCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckUW1_TimeOut.Checked)
            {
                _PlcUWCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcUWCarrier.EqpWrite(TRIGER, false);
                _PlcUWCarrier.Info($"{TRIGER} Report Completed");


                _PlcUWCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.00]");
                AddAlarmSetCode(10000);
            }

            if (ckUW1_NG.Checked)
            {
                _PlcUWCarrier.HostWrite(ACK, 11);
                _PlcUWCarrier.HostWrite(CONF, true);

                _PlcUWCarrier.Info($"Since it is an ACK-11 NG, clear data");

                _PlcUWCarrier.EqpWrite("UW1 Carrier ID", "");
                _PlcUWCarrier.EqpWrite("UW1 Product ID", "");
                _PlcUWCarrier.EqpWrite("UW1 Carrier-Lot ID", "");
                _PlcUWCarrier.EqpWrite("UW1 Initial WIP Qty", 0);
                _PlcUWCarrier.EqpWrite("UW1 Work Qty", 0);
                _PlcUWCarrier.EqpWrite("UW1 Fixed Loss", 0);
                _PlcUWCarrier.EqpWrite("UW1 Job End Type", 0);

                _PlcUWCarrier.EqpWrite(TRIGER, false);

                txtUW1Carrier.Text = "";
                txtUW1CarrierLot.Text = "";
                UW1_ID = false;
                UW1_JOB = false;
                SetLabelBackColor(lbUW1_ID_stat, UW1_ID);
                SetLabelBackColor(lbUW1_JOB_stat, UW1_JOB);

                Thread.Sleep(100);

                _plcAlarm.HostWrite(CONF, false);
                _plcAlarm.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA
                _PlcUWCarrier.HostWrite("Lot Type", "P");
                _PlcUWCarrier.HostWrite("UW1 Initial WIP Qty", 5000);
                _PlcUWCarrier.HostWrite("UW1 Product ID", "PROD" + RANDOM);
                _PlcUWCarrier.HostWrite("UW1 Carrier-Lot ID", "CARRLOT" + RANDOM);
                _PlcUWCarrier.HostWrite(ACK, 10);
                _PlcUWCarrier.HostWrite(CONF, true);

                _PlcUWCarrier.Info($"Actions after receiving the host signal");
                _PlcUWCarrier.EqpWrite("UW1 Product ID", "PROD" + RANDOM);
                _PlcUWCarrier.EqpWrite("UW1 Carrier-Lot ID", "CARRLOT" + RANDOM);
                _PlcUWCarrier.EqpWrite(TRIGER, false);


                txtUW1CarrierLot.Text = "CARRLOT" + RANDOM;
                UW1_ID = true;
                UW1_JOB = false;
                SetLabelBackColor(lbUW1_ID_stat, UW1_ID);
                SetLabelBackColor(lbUW1_JOB_stat, UW1_JOB);

                Thread.Sleep(100);

                _PlcUWCarrier.HostWrite(CONF, false);
                _PlcUWCarrier.Info($"{TRIGER} Report Completed");

                if (rbUW1_1.Checked)
                {
                    _PlcUWCarrier.Event(" Since UW1 is active, Job Start Report occurs");
                    UW1_JobStart();
                }

            }

        }

        private void UW1_JobStart()
        {
            string TRIGER = "Carrier Job Start Report_01";
            string CONF = "Carrier Job Start Confirm_01";
            string ACK = "Carrier Job Start Confirm Ack_01";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            _PlcUWCarrier.EqpWrite(TRIGER, true);
            _PlcUWCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(ACK, 10);
            _PlcUWCarrier.HostWrite(CONF, true);

            _PlcUWCarrier.Info($"Actions after receiving the host signal");
            _PlcUWCarrier.EqpWrite(TRIGER, false);

            UW1_JOB = true;
            SetLabelBackColor(lbUW1_JOB_stat, UW1_JOB);

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(CONF, false);
            _PlcUWCarrier.Info($"{TRIGER} Report Completed");

            if (_bLotRunning) txtUW1Carrier.ReadOnly = true;
        }

        private void UW1_JobEnd(int EndType = 1)
        {
            if (EndLotType != 0) EndType = EndLotType;

            string TRIGER = "Carrier Job End Report_01";
            string CONF = "Carrier Job End Confirm_01";

            _PlcUWCarrier.EqpWrite("UW1 Work Qty", 1234);
            _PlcUWCarrier.EqpWrite("UW1 Job End Type", EndType);

            if (EndType == 1)
            {
                _PlcUWCarrier.EqpWrite("UW1 Remaining Qty", 0);
                _PlcUWCarrier.EqpWrite("UW1 Previous Input Loss", "Initial WIP Qty - Input Qty - Fixed Loss Qty");
            }
            else
            {
                _PlcUWCarrier.EqpWrite("UW1 Remaining Qty", "Initial WIP Qty - Work Qty");
                _PlcUWCarrier.EqpWrite("UW1 Previous Input Loss", 0);
            }

            _PlcUWCarrier.EqpWrite("UW1 Auto Carrier Change State", ckUWAutosplicing.Checked);

            _PlcLotControl.Info("UW1 Job End Data Move - UW1 Data >> UW Input Lot 01");
            _PlcLotControl.EqpWrite("UW Input Lot Qty", 1);
            _PlcLotControl.EqpWrite("UW Input Lot-01 Carrier ID", txtUW1Carrier.Text);
            _PlcLotControl.EqpWrite("UW Input Lot-01 Carrier-Lot ID", txtUW1CarrierLot.Text);
            _PlcLotControl.EqpWrite("UW Input Lot-01 End Core AB", rbUW1_1.Checked ? true : false);
            _PlcLotControl.EqpWrite("UW Input Lot-01 Initial WIP Qty", "UW1 Initial WIP Qty Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Work Qty", "UW1 Work Qty Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Remaining Qty", "UW1 Work Remaining Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Previous Input Loss", "UW1 Previous Input Loss Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Fixed Loss", "UW1 Fixed Loss Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Job End Type", "UW1 Job End Type");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Working Direction", "UW1 Working Direction");

            _PlcUWCarrier.EqpWrite(TRIGER, true);
            _PlcUWCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(CONF, true);
            _PlcUWCarrier.Info($"Actions after receiving the host signal");
            _PlcUWCarrier.EqpWrite(TRIGER, false);

            UW1_ID = false;
            UW1_JOB = false;
            SetLabelBackColor(lbUW1_ID_stat, UW1_ID);
            SetLabelBackColor(lbUW1_JOB_stat, UW1_JOB);


            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(CONF, false);
            _PlcUWCarrier.Info($"{TRIGER} Report Completed");

            txtUW1Carrier.ReadOnly = false;
        }

        private void UW1_Output()
        {
            string TRIGER = "Carrier Output Report_01";
            string CONF = "Carrier Output Confirm_01";

            _PlcUWCarrier.EqpWrite(TRIGER, true);
            _PlcUWCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcUWCarrier.Info($"Actions after receiving the host signal");

            _PlcUWCarrier.HostWrite(CONF, true);
            _PlcUWCarrier.EqpWrite(TRIGER, false);

            _PlcUWCarrier.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcUWCarrier.EqpWrite("UW1 Carrier ID", "");
            _PlcUWCarrier.EqpWrite("UW1 Product ID", "");
            _PlcUWCarrier.EqpWrite("UW1 Carrier-Lot ID", "");
            _PlcUWCarrier.EqpWrite("UW1 Initial WIP Qty", 0);
            _PlcUWCarrier.EqpWrite("UW1 Work Qty", 0);
            _PlcUWCarrier.EqpWrite("UW1 Fixed Loss", 0);
            _PlcUWCarrier.EqpWrite("UW1 Job End Type", 0);

            _PlcUWCarrier.EqpWrite(TRIGER, false);

            txtUW1Carrier.Text = "";
            txtUW1CarrierLot.Text = "";
            UW1_ID = false;
            UW1_JOB = false;
            SetLabelBackColor(lbUW1_ID_stat, UW1_ID);
            SetLabelBackColor(lbUW1_JOB_stat, UW1_JOB);

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(CONF, false);
            _PlcUWCarrier.Info($"{TRIGER} Report Completed");
            txtUW1Carrier.ReadOnly = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (UW1_JOB) UW1_JobEnd(ckUWAutosplicing.Checked ? 1 : 2);

            UW1_Output();

        }
        #endregion

        private void SetLabelBackColor(Label lb, bool val)
        {
            if (!val) lb.BackColor = Color.Gray;
            else lb.BackColor = Color.YellowGreen;
        }

        #region UW2

        private void btnUWCarrier2_Click(object sender, EventArgs e)
        {
            string CSTID = txtUW2Carrier.Text;

            string TRIGER = "Carrier ID Report_02";
            string CONF = "Carrier ID Confirm_02";
            string ACK = "Carrier ID Confirm Ack_02";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcUWCarrier.EqpWrite("UW2 Carrier ID", CSTID);
            _PlcUWCarrier.EqpWrite("UW2 Carrier ID Reading Type", 3);
            _PlcUWCarrier.EqpWrite(TRIGER, true);
            _PlcUWCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckUW1_TimeOut.Checked)
            {
                _PlcUWCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcUWCarrier.EqpWrite(TRIGER, false);
                _PlcUWCarrier.Info($"{TRIGER} Report Completed");

                _PlcUWCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.01]");
                AddAlarmSetCode(10001);
            }

            if (ckUW2_NG.Checked)
            {
                _PlcUWCarrier.HostWrite(ACK, 11);
                _PlcUWCarrier.HostWrite(CONF, true);

                _PlcUWCarrier.Info($"Since it is an ACK-11 NG, clear data");

                _PlcUWCarrier.EqpWrite("UW2 Carrier ID", "");
                _PlcUWCarrier.EqpWrite("UW2 Product ID", "");
                _PlcUWCarrier.EqpWrite("UW2 Carrier-Lot ID", "");
                _PlcUWCarrier.EqpWrite("UW2 Initial WIP Qty", 0);
                _PlcUWCarrier.EqpWrite("UW2 Work Qty", 0);
                _PlcUWCarrier.EqpWrite("UW2 Fixed Loss", 0);
                _PlcUWCarrier.EqpWrite("UW2 Job End Type", 0);

                _PlcUWCarrier.EqpWrite(TRIGER, false);

                txtUW2Carrier.Text = "";
                txtUW2CarrierLot.Text = "";
                UW2_ID = false;
                UW2_JOB = false;
                SetLabelBackColor(lbUW2_ID_stat, UW2_ID);
                SetLabelBackColor(lbUW2_JOB_stat, UW2_JOB);

                Thread.Sleep(100);

                _PlcUWCarrier.HostWrite(CONF, false);
                _PlcUWCarrier.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA
                _PlcUWCarrier.HostWrite("Lot Type", "P");
                _PlcUWCarrier.HostWrite("UW2 Initial WIP Qty", 5000);
                _PlcUWCarrier.HostWrite("UW2 Product ID", "PROD" + RANDOM);
                _PlcUWCarrier.HostWrite("UW2 Carrier-Lot ID", "CARRLOT" + RANDOM);
                _PlcUWCarrier.HostWrite(ACK, 10);
                _PlcUWCarrier.HostWrite(CONF, true);

                _PlcUWCarrier.Info($"Actions after receiving the host signal");
                _PlcUWCarrier.EqpWrite("UW2 Product ID", "PROD" + RANDOM);
                _PlcUWCarrier.EqpWrite("UW2 Carrier-Lot ID", "CARRLOT" + RANDOM);
                _PlcUWCarrier.EqpWrite(TRIGER, false);


                txtUW2CarrierLot.Text = "CARRLOT" + RANDOM;
                UW2_ID = true;
                UW2_JOB = false;
                SetLabelBackColor(lbUW2_ID_stat, UW2_ID);
                SetLabelBackColor(lbUW2_JOB_stat, UW2_JOB);

                Thread.Sleep(100);

                _PlcUWCarrier.HostWrite(CONF, false);
                _PlcUWCarrier.Info($"{TRIGER} Report Completed");

                if (rbUW1_2.Checked)
                {
                    _PlcUWCarrier.Event(" Since UW1 is active, Job Start Report occurs");
                    UW2_JobStart();
                }

            }
        }

        private void UW2_JobStart()
        {
            string TRIGER = "Carrier Job Start Report_02";
            string CONF = "Carrier Job Start Confirm_02";
            string ACK = "Carrier Job Start Confirm Ack_02";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            _PlcUWCarrier.EqpWrite(TRIGER, true);
            _PlcUWCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(ACK, 10);
            _PlcUWCarrier.HostWrite(CONF, true);

            _PlcUWCarrier.Info($"Actions after receiving the host signal");
            _PlcUWCarrier.EqpWrite(TRIGER, false);

            UW2_JOB = true;
            SetLabelBackColor(lbUW2_JOB_stat, UW2_JOB);

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(CONF, false);
            _PlcUWCarrier.Info($"{TRIGER} Report Completed");

            if (_bLotRunning) txtUW2Carrier.ReadOnly = true;
        }

        private void UW2_JobEnd(int EndType = 1)
        {

            if (EndLotType != 0) EndType = EndLotType;
            string TRIGER = "Carrier Job End Report_02";
            string CONF = "Carrier Job End Confirm_02";

            _PlcUWCarrier.EqpWrite("UW2 Work Qty", 1234);
            _PlcUWCarrier.EqpWrite("UW2 Job End Type", EndType);

            if (EndType == 1)
            {
                _PlcUWCarrier.EqpWrite("UW2 Remaining Qty", 0);
                _PlcUWCarrier.EqpWrite("UW2 Previous Input Loss", "Initial WIP Qty - Input Qty - Fixed Loss Qty");
            }
            else
            {
                _PlcUWCarrier.EqpWrite("UW2 Remaining Qty", "Initial WIP Qty - Work Qty");
                _PlcUWCarrier.EqpWrite("UW2 Previous Input Loss", 0);
            }

            _PlcUWCarrier.EqpWrite("UW2 Auto Carrier Change State", ckUWAutosplicing.Checked);

            _PlcLotControl.Info("UW2 Job End Data Move - UW1 Data >> UW Input Lot 01");
            _PlcLotControl.EqpWrite("UW Input Lot Qty", 1);
            _PlcLotControl.EqpWrite("UW Input Lot-01 Carrier ID", txtUW2Carrier.Text);
            _PlcLotControl.EqpWrite("UW Input Lot-01 Carrier-Lot ID", txtUW2CarrierLot.Text);
            _PlcLotControl.EqpWrite("UW Input Lot-01 End Core AB", rbUW1_1.Checked ? true : false);
            _PlcLotControl.EqpWrite("UW Input Lot-01 Initial WIP Qty", "UW2 Initial WIP Qty Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Work Qty", "UW2 Work Qty Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Remaining Qty", "UW2 Work Remaining Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Previous Input Loss", "UW2 Previous Input Loss Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Fixed Loss", "UW2 Fixed Loss Value");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Job End Type", "UW2 Job End Type");
            _PlcLotControl.EqpWrite("UW Input Lot-01 Working Direction", "UW2 Working Direction");

            _PlcUWCarrier.EqpWrite(TRIGER, true);
            _PlcUWCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(CONF, true);
            _PlcUWCarrier.Info($"Actions after receiving the host signal");
            _PlcUWCarrier.EqpWrite(TRIGER, false);

            UW2_ID = false;
            UW2_JOB = false;
            SetLabelBackColor(lbUW2_ID_stat, UW2_ID);
            SetLabelBackColor(lbUW2_JOB_stat, UW2_JOB);

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(CONF, false);
            _PlcUWCarrier.Info($"{TRIGER} Report Completed");

            txtUW2Carrier.ReadOnly = false;
        }

        private void UW2_Output()
        {
            string TRIGER = "Carrier Output Report_02";
            string CONF = "Carrier Output Confirm_02";

            _PlcUWCarrier.EqpWrite(TRIGER, true);
            _PlcUWCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcUWCarrier.Info($"Actions after receiving the host signal");

            _PlcUWCarrier.HostWrite(CONF, true);
            _PlcUWCarrier.EqpWrite(TRIGER, false);

            _PlcUWCarrier.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcUWCarrier.EqpWrite("UW2 Carrier ID", "");
            _PlcUWCarrier.EqpWrite("UW2 Product ID", "");
            _PlcUWCarrier.EqpWrite("UW2 Carrier-Lot ID", "");
            _PlcUWCarrier.EqpWrite("UW2 Initial WIP Qty", 0);
            _PlcUWCarrier.EqpWrite("UW2 Work Qty", 0);
            _PlcUWCarrier.EqpWrite("UW2 Fixed Loss", 0);
            _PlcUWCarrier.EqpWrite("UW2 Job End Type", 0);

            _PlcUWCarrier.EqpWrite(TRIGER, false);

            txtUW2Carrier.Text = "";
            txtUW2CarrierLot.Text = "";
            UW2_ID = false;
            UW2_JOB = false;
            SetLabelBackColor(lbUW2_ID_stat, UW2_ID);
            SetLabelBackColor(lbUW2_JOB_stat, UW2_JOB);

            Thread.Sleep(100);

            _PlcUWCarrier.HostWrite(CONF, false);
            _PlcUWCarrier.Info($"{TRIGER} Report Completed");

            txtUW2Carrier.ReadOnly = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (UW2_JOB) UW2_JobEnd(ckUWAutosplicing.Checked ? 1 : 2);

            UW2_Output();
        }
        #endregion

        private void rbUW1_1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUW1_1.Checked)
            {
                if (UW1_ID && !UW1_JOB) UW1_JobStart();

                if (UW2_JOB) UW2_JobEnd(ckUWAutosplicing.Checked ? 1 : 2);
            }
            else
            {
                if (UW2_ID && !UW2_JOB) UW2_JobStart();

                if (UW1_JOB) UW1_JobEnd(ckUWAutosplicing.Checked ? 1 : 2);

            }
        }
        #endregion

        #region Carrier - RU
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

            rbRU1.Checked = true;
        }

        #region RU1

        private void btnRWCarrier1_Click(object sender, EventArgs e)
        {
            string CSTID = txtRU1Carrier.Text;

            string TRIGER = "Carrier ID Report_03";
            string CONF = "Carrier ID Confirm_03";
            string ACK = "Carrier ID Confirm Ack_03";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcRUCarrier.EqpWrite("RU1 Carrier ID", CSTID);
            _PlcRUCarrier.EqpWrite("RU1 Carrier ID Reading Type", 3);
            _PlcRUCarrier.EqpWrite(TRIGER, true);
            _PlcRUCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckRU1_TimeOut.Checked)
            {
                _PlcRUCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcRUCarrier.EqpWrite(TRIGER, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

                _PlcRUCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.02]");
                AddAlarmSetCode(10001);
            }

            if (ckRU1_NG.Checked)
            {
                _PlcRUCarrier.HostWrite(ACK, 11);
                _PlcRUCarrier.HostWrite(CONF, true);

                _PlcRUCarrier.Info($"Since it is an ACK-11 NG, clear data");
                _PlcRUCarrier.EqpWrite("RU1 Carrier ID", "");
                _PlcRUCarrier.EqpWrite(TRIGER, false);

                txtRU1Carrier.Text = "";
                txtRU1CarrierLot.Text = "";
                RU1_ID = false;
                SetLabelBackColor(lbRU1_ID_stat, RU1_ID);

                Thread.Sleep(100);

                _PlcRUCarrier.HostWrite(CONF, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA
                _PlcRUCarrier.HostWrite(ACK, 10);
                _PlcRUCarrier.HostWrite(CONF, true);

                _PlcRUCarrier.Info($"Actions after receiving the host signal");
                _PlcRUCarrier.EqpWrite(TRIGER, false);

                RU1_ID = true;
                SetLabelBackColor(lbRU1_ID_stat, RU1_ID);

                Thread.Sleep(100);

                _PlcRUCarrier.HostWrite(CONF, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            RU1_Output();
        }

        private void RU1_Output()
        {
            string TRIGER = "Carrier Output Report_03";
            string CONF = "Carrier Output Confirm_03";

            _PlcRUCarrier.EqpWrite(TRIGER, true);
            _PlcRUCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcRUCarrier.Info($"Actions after receiving the host signal");

            _PlcRUCarrier.HostWrite(CONF, true);
            _PlcRUCarrier.EqpWrite(TRIGER, false);

            _PlcRUCarrier.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcRUCarrier.EqpWrite("RU1 Carrier ID", "");
            _PlcRUCarrier.EqpWrite("RU1 Product ID", "");
            _PlcRUCarrier.EqpWrite("RU1 Carrier-Lot ID", "");
            _PlcRUCarrier.EqpWrite("RU1 Work Qty", 0);
            _PlcRUCarrier.EqpWrite("RU1 Output Type", 0);
            _PlcRUCarrier.EqpWrite("RU1 Fixed Loss", 0);
            _PlcRUCarrier.EqpWrite("RU1 Vision Trigger Count", 0);

            _PlcRUCarrier.EqpWrite(TRIGER, false);

            txtRU1Carrier.Text = "";
            txtRU1CarrierLot.Text = "";
            RU1_ID = false;
            SetLabelBackColor(lbRU1_ID_stat, RU1_ID);

            Thread.Sleep(100);

            _PlcRUCarrier.HostWrite(CONF, false);
            _PlcRUCarrier.Info($"{TRIGER} Report Completed");

            txtRU1Carrier.ReadOnly = false;
        }

        #endregion

        #region RU2

        private void btnRUCarrier2_Click(object sender, EventArgs e)
        {
            string CSTID = txtRU1Carrier.Text;

            string TRIGER = "Carrier ID Report_04";
            string CONF = "Carrier ID Confirm_04";
            string ACK = "Carrier ID Confirm Ack_04";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcRUCarrier.EqpWrite("RU2 Carrier ID", CSTID);
            _PlcRUCarrier.EqpWrite("RU2 Carrier ID Reading Type", 3);
            _PlcRUCarrier.EqpWrite(TRIGER, true);
            _PlcRUCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckRU1_TimeOut.Checked)
            {
                _PlcRUCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcRUCarrier.EqpWrite(TRIGER, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

                _PlcRUCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.03]");
                AddAlarmSetCode(10001);
            }

            if (ckRU2_NG.Checked)
            {
                _PlcRUCarrier.HostWrite(ACK, 11);
                _PlcRUCarrier.HostWrite(CONF, true);

                _PlcRUCarrier.Info($"Since it is an ACK-11 NG, clear data");
                _PlcRUCarrier.EqpWrite("RU2 Carrier ID", "");
                _PlcRUCarrier.EqpWrite(TRIGER, false);

                txtRU2Carrier.Text = "";
                txtRU2CarrierLot.Text = "";
                RU2_ID = false;
                SetLabelBackColor(lbRU2_ID_stat, RU2_ID);

                Thread.Sleep(100);

                _PlcRUCarrier.HostWrite(CONF, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA
                _PlcRUCarrier.HostWrite(ACK, 10);
                _PlcRUCarrier.HostWrite(CONF, true);

                _PlcRUCarrier.Info($"Actions after receiving the host signal");
                _PlcRUCarrier.EqpWrite(TRIGER, false);

                RU2_ID = true;
                SetLabelBackColor(lbRU2_ID_stat, RU2_ID);

                Thread.Sleep(100);

                _PlcRUCarrier.HostWrite(CONF, false);
                _PlcRUCarrier.Info($"{TRIGER} Report Completed");

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            RU2_Output();
        }

        private void RU2_Output()
        {
            string TRIGER = "Carrier Output Report_04";
            string CONF = "Carrier Output Confirm_04";

            _PlcRUCarrier.EqpWrite(TRIGER, true);
            _PlcRUCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcRUCarrier.Info($"Actions after receiving the host signal");

            _PlcRUCarrier.HostWrite(CONF, true);
            _PlcRUCarrier.EqpWrite(TRIGER, false);

            _PlcRUCarrier.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcRUCarrier.EqpWrite("RU2 Carrier ID", "");
            _PlcRUCarrier.EqpWrite("RU2 Product ID", "");
            _PlcRUCarrier.EqpWrite("RU2 Carrier-Lot ID", "");
            _PlcRUCarrier.EqpWrite("RU2 Work Qty", 0);
            _PlcRUCarrier.EqpWrite("RU2 Output Type", 0);
            _PlcRUCarrier.EqpWrite("RU2 Fixed Loss", 0);
            _PlcRUCarrier.EqpWrite("RU2 Vision Trigger Count", 0);

            _PlcRUCarrier.EqpWrite(TRIGER, false);

            txtRU2Carrier.Text = "";
            txtRU2CarrierLot.Text = "";
            RU2_ID = false;
            SetLabelBackColor(lbRU2_ID_stat, RU2_ID);

            Thread.Sleep(100);

            _PlcRUCarrier.HostWrite(CONF, false);
            _PlcRUCarrier.Info($"{TRIGER} Report Completed");

            txtRU2Carrier.ReadOnly = false;
        }
        #endregion
        #endregion

        #region Carrier - RL
        private PlcMonitorControl _PlcRLCarrier;

        bool RL1_ID, RL2_ID;

        private void InitializeCarrierRLLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();

            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 3;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _PlcRLCarrier = new PlcMonitorControl();
            _PlcRLCarrier.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_PlcRLCarrier);

            rbRL1.Checked = true;
        }

        #region RL1

        private void btnRLCarrier1_Click(object sender, EventArgs e)
        {
            string CSTID = txtRL1Carrier.Text;

            string TRIGER = "Carrier ID Report_05";
            string CONF = "Carrier ID Confirm_05";
            string ACK = "Carrier ID Confirm Ack_05";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcRLCarrier.EqpWrite("RL1 Carrier ID", CSTID);
            _PlcRLCarrier.EqpWrite("RL1 Carrier ID Reading Type", 3);
            _PlcRLCarrier.EqpWrite(TRIGER, true);
            _PlcRLCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckRL1_TimeOut.Checked)
            {
                _PlcRLCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcRLCarrier.EqpWrite(TRIGER, false);
                _PlcRLCarrier.Info($"{TRIGER} Report Completed");

                _PlcRLCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.04]");
                AddAlarmSetCode(10004);
            }

            if (ckRL1_NG.Checked)
            {
                _PlcRLCarrier.HostWrite(ACK, 11);
                _PlcRLCarrier.HostWrite(CONF, true);

                _PlcRLCarrier.Info($"Since it is an ACK-11 NG, clear data");
                _PlcRLCarrier.EqpWrite("RL1 Carrier ID", "");
                _PlcRLCarrier.EqpWrite(TRIGER, false);

                txtRL1Carrier.Text = "";
                txtRL1CarrierLot.Text = "";
                RL1_ID = false;
                SetLabelBackColor(lbRL1_ID_stat, RL1_ID);

                Thread.Sleep(100);

                _PlcRLCarrier.HostWrite(CONF, false);
                _PlcRLCarrier.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA
                _PlcRLCarrier.HostWrite(ACK, 10);
                _PlcRLCarrier.HostWrite(CONF, true);

                _PlcRLCarrier.Info($"Actions after receiving the host signal");
                _PlcRLCarrier.EqpWrite(TRIGER, false);

                RL1_ID = true;
                SetLabelBackColor(lbRL1_ID_stat, RL1_ID);

                Thread.Sleep(100);

                _PlcRLCarrier.HostWrite(CONF, false);
                _PlcRLCarrier.Info($"{TRIGER} Report Completed");

            }
        }


        private void btnRLCarrier1Out_Click(object sender, EventArgs e)
        {
            RL1_Output();
        }

        private void RL1_Output()
        {
            string TRIGER = "Carrier Output Report_05";
            string CONF = "Carrier Output Confirm_05";

            _PlcRLCarrier.EqpWrite(TRIGER, true);
            _PlcRLCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcRLCarrier.Info($"Actions after receiving the host signal");

            _PlcRLCarrier.HostWrite(CONF, true);
            _PlcRLCarrier.EqpWrite(TRIGER, false);

            _PlcRLCarrier.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcRLCarrier.EqpWrite("RL1 Carrier ID", "");
            _PlcRLCarrier.EqpWrite("RL1 Product ID", "");
            _PlcRLCarrier.EqpWrite("RL1 Carrier-Lot ID", "");
            _PlcRLCarrier.EqpWrite("RL1 Work Qty", 0);
            _PlcRLCarrier.EqpWrite("RL1 Output Type", 0);
            _PlcRLCarrier.EqpWrite("RL1 Fixed Loss", 0);
            _PlcRLCarrier.EqpWrite("RL1 Vision Trigger Count", 0);

            _PlcRLCarrier.EqpWrite(TRIGER, false);

            txtRL1Carrier.Text = "";
            txtRL1CarrierLot.Text = "";
            RL1_ID = false;
            SetLabelBackColor(lbRL1_ID_stat, RL1_ID);

            Thread.Sleep(100);

            _PlcRLCarrier.HostWrite(CONF, false);
            _PlcRLCarrier.Info($"{TRIGER} Report Completed");

            txtRL1Carrier.ReadOnly = false;
        }

        #endregion

        #region RL2

        private void btnRLCarrier2_Click(object sender, EventArgs e)
        {
            string CSTID = txtRL1Carrier.Text;

            string TRIGER = "Carrier ID Report_06";
            string CONF = "Carrier ID Confirm_06";
            string ACK = "Carrier ID Confirm Ack_06";
            string RANDOM = DateTime.Now.ToString("HHmmss");

            if (string.IsNullOrWhiteSpace(CSTID)) CSTID = "NOREAD";

            _PlcRLCarrier.EqpWrite("RL2 Carrier ID", CSTID);
            _PlcRLCarrier.EqpWrite("RL2 Carrier ID Reading Type", 3);
            _PlcRLCarrier.EqpWrite(TRIGER, true);
            _PlcRLCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            if (ckRL1_TimeOut.Checked)
            {
                _PlcRLCarrier.Info($"If there is no response for 30 seconds, it times out.");

                _PlcRLCarrier.EqpWrite(TRIGER, false);
                _PlcRLCarrier.Info($"{TRIGER} Report Completed");

                _PlcRLCarrier.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 3.05]");
                AddAlarmSetCode(10005);
            }

            if (ckRL2_NG.Checked)
            {
                _PlcRLCarrier.HostWrite(ACK, 11);
                _PlcRLCarrier.HostWrite(CONF, true);

                _PlcRLCarrier.Info($"Since it is an ACK-11 NG, clear data");
                _PlcRLCarrier.EqpWrite("RL2 Carrier ID", "");
                _PlcRLCarrier.EqpWrite(TRIGER, false);

                txtRL2Carrier.Text = "";
                txtRL2CarrierLot.Text = "";
                RL2_ID = false;
                SetLabelBackColor(lbRL2_ID_stat, RL2_ID);

                Thread.Sleep(100);

                _PlcRLCarrier.HostWrite(CONF, false);
                _PlcRLCarrier.Info($"{TRIGER} Report Completed");
            }
            else
            {
                //DATA
                _PlcRLCarrier.HostWrite(ACK, 10);
                _PlcRLCarrier.HostWrite(CONF, true);

                _PlcRLCarrier.Info($"Actions after receiving the host signal");
                _PlcRLCarrier.EqpWrite(TRIGER, false);

                RL2_ID = true;
                SetLabelBackColor(lbRL2_ID_stat, RL2_ID);

                Thread.Sleep(100);

                _PlcRLCarrier.HostWrite(CONF, false);
                _PlcRLCarrier.Info($"{TRIGER} Report Completed");

            }
        }

        private void btnRLCarrier2Out_Click(object sender, EventArgs e)
        {
            RL2_Output();
        }

        private void RL2_Output()
        {
            string TRIGER = "Carrier Output Report_04";
            string CONF = "Carrier Output Confirm_04";

            _PlcRLCarrier.EqpWrite(TRIGER, true);
            _PlcRLCarrier.Info($"Confirm signal waiting");

            Thread.Sleep(100);

            _PlcRLCarrier.Info($"Actions after receiving the host signal");

            _PlcRLCarrier.HostWrite(CONF, true);
            _PlcRLCarrier.EqpWrite(TRIGER, false);

            _PlcRLCarrier.Info($"Since the CARRIER has been emitted, it must be deleted.");

            _PlcRLCarrier.EqpWrite("RL2 Carrier ID", "");
            _PlcRLCarrier.EqpWrite("RL2 Product ID", "");
            _PlcRLCarrier.EqpWrite("RL2 Carrier-Lot ID", "");
            _PlcRLCarrier.EqpWrite("RL2 Work Qty", 0);
            _PlcRLCarrier.EqpWrite("RL2 Output Type", 0);
            _PlcRLCarrier.EqpWrite("RL2 Fixed Loss", 0);
            _PlcRLCarrier.EqpWrite("RL2 Vision Trigger Count", 0);

            _PlcRLCarrier.EqpWrite(TRIGER, false);

            txtRL2Carrier.Text = "";
            txtRL2CarrierLot.Text = "";
            RL2_ID = false;
            SetLabelBackColor(lbRL2_ID_stat, RL2_ID);

            Thread.Sleep(100);

            _PlcRLCarrier.HostWrite(CONF, false);
            _PlcRLCarrier.Info($"{TRIGER} Report Completed");

            txtRL2Carrier.ReadOnly = false;
        }
        #endregion
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

            if (rbUW1_1.Checked)
            {
                if (UW1_JOB == false)
                {
                    _PlcLotControl.Error("UW is not ready, so I can't start Lot.");
                    return;
                }
            }
            else
            {
                if (UW2_JOB == false)
                {
                    _PlcLotControl.Error("UW is not ready, so I can't start Lot.");
                    return;
                }
            }

            if (rbRU1.Checked)
            {
                if (RU1_ID == false)
                {
                    _PlcLotControl.Error("RU is not ready, so I can't start Lot.");
                    return;
                }
            }
            else
            {
                if (RU2_ID == false)
                {
                    _PlcLotControl.Error("RU is not ready, so I can't start Lot.");
                    return;
                }
            }

            if (rbRL1.Checked)
            {
                if (RL1_ID == false)
                {
                    _PlcLotControl.Error("RL is not ready, so I can't start Lot.");
                    return;
                }
            }
            else
            {
                if (RL2_ID == false)
                {
                    _PlcLotControl.Error("RL is not ready, so I can't start Lot.");
                    return;
                }
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

            if (ckRL1_TimeOut.Checked)
            {
                _PlcLotControl.Info($"If there is no response for 30 seconds, it times out.");

                _PlcLotControl.EqpWrite(TRIGER, false);
                _PlcLotControl.Info($"{TRIGER} Report Completed");

                _PlcLotControl.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 1.02]");
                AddAlarmSetCode(10004);
            }

            if (ckRL1_NG.Checked)
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

                if (rbRU1.Checked)
                {
                    _PlcLotControl.EqpWrite("RU1 Carrier-Lot ID", "UPPER" + RANDOM);
                    txtRU1CarrierLot.Text = "UPPER" + RANDOM;
                }
                else
                {
                    _PlcLotControl.EqpWrite("RU2 Carrier-Lot ID", "UPPER" + RANDOM);
                    txtRU2CarrierLot.Text = "UPPER" + RANDOM;
                }
                txtUpperLot.Text = "UPPER" + RANDOM;

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

            if (ckRL1_TimeOut.Checked)
            {
                _PlcLotControl.Info($"If there is no response for 30 seconds, it times out.");

                _PlcLotControl.EqpWrite(TRIGER, false);
                _PlcLotControl.Info($"{TRIGER} Report Completed");

                _PlcLotControl.Event("EHxxxx(Alarm ID) : Host Communication Timeout[Tag 1.03]");
                AddAlarmSetCode(10004);
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

            if (rbRL1.Checked)
            {
                RL1_ID = false;
                SetLabelBackColor(lbRL1_ID_stat, RL1_ID);
            }
            else
            {
                RL2_ID = false;
                SetLabelBackColor(lbRL2_ID_stat, RL2_ID);
            }

            if (rbRU1.Checked)
            {
                RU1_ID = false;
                SetLabelBackColor(lbRU1_ID_stat, RU1_ID);
            }
            else
            {
                RU2_ID = false;
                SetLabelBackColor(lbRU2_ID_stat, RU2_ID);
            }


            EndLotType = 0;

        }

        private void btnLotEnd3_Click(object sender, EventArgs e)
        {
            if (_bLotRunning == false)
            {
                _PlcLotControl.Error("The lot is not in running state.");
                return;
            }

            _PlcLotControl.Event("##### UW JOB END REPORT STRART #####");
            if (UW1_JOB) UW1_JobEnd(2);
            else UW2_JobEnd(2);

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

            if (rbRU1.Checked)
            {
                if (RU2_ID == false)
                {
                    _PlcLotControl.Error("RU is not ready, so I can't Lot division.");
                    return;
                }
            }
            else
            {
                if (RU1_ID == false)
                {
                    _PlcLotControl.Error("RU is not ready, so I can't Lot division.");
                    return;
                }
            }

            if (rbRL1.Checked)
            {
                if (RL2_ID == false)
                {
                    _PlcLotControl.Error("RL is not ready, so I can't Lot division.");
                    return;
                }
            }
            else
            {
                if (RL1_ID == false)
                {
                    _PlcLotControl.Error("RL is not ready, so I can't Lot division.");
                    return;
                }
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

            _PlcLotControl.Event("##### UW JOB END REPORT STRART #####");
            if (UW1_JOB) UW1_JobEnd();
            else UW2_JobEnd();

            _PlcLotControl.Event("##### LOT END REPORT STRART #####");
            LotEnd(true);


            _PlcLotControl.Event("##### RU Core Change #####");

            if (rbRU1.Checked) rbRU2.Checked = true;
            else rbRU1.Checked = true;

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
            if (rbUW1_1.Checked)
            {
                if (UW2_ID == false)
                {
                    _PlcLotControl.Error("UW is not ready, so I can't Lot End.");
                    return;
                }
            }
            else
            {
                if (UW1_ID == false)
                {
                    _PlcLotControl.Error("UW is not ready, so I can't Lot End.");
                    return;
                }

            }
            if (rbRU1.Checked)
            {
                if (RU2_ID == false)
                {
                    _PlcLotControl.Error("RU is not ready, so I can't Lot End.");
                    return;
                }
            }
            else
            {
                if (RU1_ID == false)
                {
                    _PlcLotControl.Error("RU is not ready, so I can't Lot End.");
                    return;
                }
            }

            if (rbRL1.Checked)
            {
                if (RL2_ID == false)
                {
                    _PlcLotControl.Error("RL is not ready, so I can't Lot End.");
                    return;
                }
            }
            else
            {
                if (RL1_ID == false)
                {
                    _PlcLotControl.Error("RL is not ready, so I can't Lot End.");
                    return;
                }
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

            _PlcLotControl.Event("##### UW Core Change #####");
            _PlcLotControl.Event("##### UW JOB END REPORT AND NEXT UW JOB REPORT STRART #####");
            if (rbUW1_1.Checked) rbUW1_2.Checked = true;
            else rbUW1_1.Checked = true;

            _PlcLotControl.Event("##### LOT END REPORT STRART #####");
            LotEnd(true);


            _PlcLotControl.Event("##### RU Core Change #####");

            if (rbRU1.Checked) rbRU2.Checked = true;
            else rbRU1.Checked = true;

            _PlcLotControl.Event("##### RL Core Change #####");
            if (rbRL1.Checked) rbRL2.Checked = true;
            else rbRL1.Checked = true;

            bPassPopup = true;
            _PlcLotControl.Event("##### Next Lot Start #####");
            btnLotStart_Click(btnLotStart, null);
        } 
        #endregion
    }





}
