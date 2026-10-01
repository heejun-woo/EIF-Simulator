using EIF_Simulator.Core;
using EIF_Simulator.McProtocol.Plc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.Controls.Manager
{
    public class EquipmentPlc
    {
  
        public string Name => Config.Name;
        public int Port => Config.McPort;
        public EquipmentPlcConfig Config { get; }

        public PlcContext Context { get; }

        public PlcScanEngine ScanEngine => Context.ScanEngine;
        public PlcMemory Memory => Context.Memory;
        public PlcBindingManager BindingManager => Context.BindingManager;
        public McProtocolServer Server => Context.Server;
        public NodeRegistry Nodes => Context.Nodes;
        public CarrierRegistry Carriers => Context.Carriers;
        public CarrierManager CarrierManager => Context.CarrierManager;
        public NodeManager NodeManager => Context.NodeManager;
        public CarrierHistoryManager CarrierHistory => Context.CarrierHistory;
        public CarrierRouteManager RouteManager => Context._routeManager;

        public CarrierStateManager CarrierStateManager => Context.CarrierStateManager;

        public EquipmentPlc(EquipmentPlcConfig config)
        {
            Config = config;

            Context = new PlcContext();
        }

        public void Start()
        {
            Context.Start(Config.McPort);
        }

        public void Stop()
        {
            Context.Stop();
        }
    }

    public class PlcBitGridItem : INotifyPropertyChanged
    {
        public string Address { get; set; }

        private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PlcWordGridItem : INotifyPropertyChanged
    {
        public string Address { get; set; }

        private ushort _value;
        public ushort Value
        {
            get => _value;
            set
            {
                if (_value == value) return;

                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public string Description { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}


