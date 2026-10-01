using EIF_Simulator.Controls;
using EIF_Simulator.Controls.Manager;
using EIF_Simulator.Core;
using EIF_Simulator.McProtocol.Plc;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MaterialControlSimulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static PlcContext PlcContext;  
        public static NodeRegistry Nodes => PlcContext.Nodes;        
        public static CarrierRegistry Carriers => PlcContext.Carriers;
        public static CarrierManager CarrierManager => PlcContext.CarrierManager;
        public static NodeManager NodeManager=> PlcContext.NodeManager;
        public static PlcMemory plcMemory => PlcContext.Memory;
        public static PlcBindingManager PlcBindingManager => PlcContext.BindingManager;
        public static McProtocolServer PlcServer => PlcContext.Server;
        public static PlcScanEngine scanEngine => PlcContext.ScanEngine;
        public static CarrierHistoryManager CarrierHistory => PlcContext.CarrierHistory;
        public static CarrierRouteManager _routeManager => PlcContext._routeManager;
        public static CarrierStateManager CarrierStateManager=> PlcContext.CarrierStateManager;

        public App()
        {
            InitializeComponent();
            PlcContext = new PlcContext();

        }
    }

}
