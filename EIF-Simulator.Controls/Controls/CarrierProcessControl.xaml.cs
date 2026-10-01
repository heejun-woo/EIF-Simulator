using EIF_Simulator.Controls.Manager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EIF_Simulator.Controls
{
    /// <summary>
    /// CarrierProcessControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CarrierProcessControl : NodeControl
    {
        public NodeRegistry Nodes => Context.Nodes;
        private CarrierRouteManager RouteManager => Context._routeManager;
        private CarrierManager CarrierManager => Context.CarrierManager;

        // =====================================================
        // Enable
        // =====================================================

        public static readonly DependencyProperty IsProcessEnabledProperty =
    DependencyProperty.Register(
        nameof(IsProcessEnabled),
        typeof(bool),
        typeof(CarrierProcessControl),
        new PropertyMetadata(
            true,
            OnProcessEnabledChanged));

        public bool IsProcessEnabled
        {
            get => (bool)GetValue(IsProcessEnabledProperty);
            set => SetValue(IsProcessEnabledProperty, value);
        }

        // =====================================================
        // Set Empty
        // true  = Carrier Empty
        // false = Carrier Loaded
        // =====================================================

        public static readonly DependencyProperty
            SetEmptyProperty =
                DependencyProperty.Register(
                    nameof(SetEmpty),
                    typeof(bool),
                    typeof(CarrierProcessControl),
                    new PropertyMetadata(true));

        public bool SetEmpty
        {
            get =>
                (bool)GetValue(
                    SetEmptyProperty);

            set =>
                SetValue(
                    SetEmptyProperty,
                    value);
        }

        // =====================================================
        // Next Node Id
        // =====================================================

        public static readonly DependencyProperty
            NextNodeIdProperty =
                DependencyProperty.Register(
                    nameof(NextNodeId),
                    typeof(string),
                    typeof(CarrierProcessControl),
                    new PropertyMetadata(""));

        public string NextNodeId
        {
            get =>
                (string)GetValue(
                    NextNodeIdProperty);

            set =>
                SetValue(
                    NextNodeIdProperty,
                    value);
        }

        // =====================================================
        // UI 표시
        // =====================================================

        public string EmptyText =>
            SetEmpty
                ? "EMPTY"
                : "LOAD";

        // =====================================================
        // Constructor
        // =====================================================

        public CarrierProcessControl()
        {
            InitializeComponent();
        }

        // =====================================================
        // Process
        // =====================================================

        public async Task ProcessAsync(
            CarrierControl carrier)
        {
            if (carrier == null)
                return;

            if (!IsProcessEnabled)
                return;

            // Process 위치에 도착한 상태에서 잠시 대기
            await Task.Delay(500);

            // -----------------------------------------
            // Carrier Empty 상태 변경
            // -----------------------------------------

            if (!SetEmpty)
                carrier.CurrentCellCount = carrier.MaxCellCount;
            else carrier.CurrentCellCount = 0;

            // 상태 변경도 눈에 보이도록 잠시 대기
            await Task.Delay(300);

            // -----------------------------------------
            // 다음 Node
            // -----------------------------------------

            if (string.IsNullOrWhiteSpace(
                    NextNodeId))
            {
                return;
            }

            var nextNode =
                Nodes.Get(NextNodeId);

            if (nextNode == null)
                return;

            RouteManager.SetDestination(carrier, nextNode.Id);
        }



        private static void OnProcessEnabledChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
        {
            if (d is not CarrierProcessControl control)
                return;

            bool enabled = (bool)e.NewValue;

            // OFF → ON일 때만
            if (!enabled)
                return;

            control.ProcessWaitingCarrier();
        }

        private void ProcessWaitingCarrier()
        {
            var session =
                CarrierManager.Sessions
                    .FirstOrDefault(x =>
                        x.Carrier.CurrentNode == this);

            if (session == null)
                return;

            ProcessAsync(session.Carrier);
        }
    }
}
