using EIF_Simulator.Controls;
using EIF_Simulator.Controls.Manager;
using EIF_Simulator.Core;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProcessControlSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LayoutManager _layoutManager;
        private EquipmentPlcWindow _equipmentPlcWindow;
        private readonly SimulatorConfig _config;

        public MainWindow()
        {
            InitializeComponent();

            _layoutManager = new LayoutManager();
            _layoutManager.LoadComplete += LayoutManager_LoadComplete;

            _config = SimulatorConfigLoader.Load();

            InitializeEquipmentPlc();
            OpenEquipmentPlc();

        }


        private void LayoutManager_LoadComplete()
        {

        }

        private void OpenEquipmentPlc()
        {

            if (_equipmentPlcWindow == null)
            {
                InitializeEquipmentPlc();

                _equipmentPlcWindow.Closed += (s, e) =>
                {
                    _equipmentPlcWindow = null;
                };
                _equipmentPlcWindow.Show();
            }
            else
            {
                _equipmentPlcWindow.Activate();
                _equipmentPlcWindow.Show();
            }
        }

        private void InitializeEquipmentPlc()
        {
            var config = _config.EquipmentPlcs.FirstOrDefault(x => x.Name == "EQ01");

            if (config == null) return;

            _equipmentPlcWindow = new EquipmentPlcWindow(config);
        }
    }
}