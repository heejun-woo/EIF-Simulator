using System.Windows.Forms;

namespace Equipment_Interface_Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitializeMonitorLayout(tabPage1);



            _plcMonitor.Info("PLC Connected");

            _plcMonitor.Write("B3810", true);

            _plcMonitor.Read("B3010", true);

            _plcMonitor.Read("W3878", 120);

            _plcMonitor.Event("Destination request started");

            _plcMonitor.Error("PLC Communication Timeout");
        }


        private PlcMonitorControl _plcMonitor;

        private void InitializeMonitorLayout(TabPage page)
        {
            SplitContainer _splitContainer = new SplitContainer();


            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Vertical;

            _splitContainer.SplitterDistance = _splitContainer.ClientSize.Width / 2;

            _splitContainer.IsSplitterFixed = false;

            page.Controls.Add(_splitContainer);

            // 오른쪽 PLC Monitor
            _plcMonitor = new PlcMonitorControl();
            _plcMonitor.Dock = DockStyle.Fill;

            _splitContainer.Panel2.Controls.Add(_plcMonitor);
        }
    }
}
