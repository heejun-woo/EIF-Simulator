using EIF_Simulator.OpcUa;
using EIF_Simulator.OpcUa.EIF_Simulator.OpcUa;
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

namespace ProcessControlSimulatorOpcUa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private OpcUaContext? _opcUa;

        private async void StartOpcUa_Click(
            object sender,
            RoutedEventArgs e)
        {
            _opcUa = new OpcUaContext();

            _opcUa.BindingManager.AddTag(
                "LGES.CommMng.Check.E_CommCheck",
                (short)0);

            _opcUa.BindingManager.AddTag(
                "LGES.CommMng.Check.H_CommCheck",
                false);

            _opcUa.BindingManager.AddTag(
                "LGES.LotMng.Current.E_LotID",
                string.Empty);

            _opcUa.BindingManager.AddTag(
                "LGES.ProcState.Eqp01.E_VisionState",
                new bool[2]);

            _opcUa.BindingManager.AddTag(
                "LGES.LotMng.Current.E_LotID",
                string.Empty);

            _opcUa.BindingManager.AddTag(
                "LGES.CommMng.Check.H_CommCheck",
                false);



            await _opcUa.StartAsync(4840);


            _ = _opcUa.StartAsync();

            ConnectUi();
        }


        private void ConnectUi()
        {
            if (_opcUa == null)
                return;

            var manager =
                _opcUa.BindingManager;

            manager.Bind(
                LotIdTextBox,
                TextBox.TextProperty,
                "LGES.LotMng.Current.E_LotID");

            manager.Bind(
                CommCheckBox,
                CheckBox.IsCheckedProperty,
                "LGES.CommMng.Check.H_CommCheck");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Simulator → OPC UA Client
            _opcUa.BindingManager.SetValue(
                "LGES.LotMng.Current.E_LotID",
                "LOT001");

            _opcUa.BindingManager.SetValue(
                "LGES.CommMng.Check.E_CommCheck",
                (short)123);


            bool value =
                _opcUa.BindingManager.GetValue<bool>(
                    "LGES.CommMng.Check.H_CommCheck");

            int aa = _opcUa.BindingManager.GetValue<int>(
                "LGES.CommMng.Check.E_CommCheck");

            string str = "";
        }
    }
}