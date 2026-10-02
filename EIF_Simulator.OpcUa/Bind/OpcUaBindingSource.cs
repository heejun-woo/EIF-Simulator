using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace EIF_Simulator.OpcUa
{
    internal class OpcUaBindingSource :
        INotifyPropertyChanged
    {
        private readonly OpcUaBindingManager _manager;
        private readonly string _tagName;

        public event PropertyChangedEventHandler?
            PropertyChanged;

        public object? Value
        {
            get
            {
                return _manager.GetValue(
                    _tagName);
            }

            set
            {
                if (value == null)
                    return;

                _manager.SetValue(
                    _tagName,
                    value);
            }
        }

        public OpcUaBindingSource(
            OpcUaBindingManager manager,
            string tagName)
        {
            _manager = manager;
            _tagName = tagName;

            _manager.ValueChanged +=
                OnValueChanged;
        }

        private void OnValueChanged(
            string tagName,
            object value)
        {
            if (!tagName.Equals(
                    _tagName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // OPC UA 통신 스레드에서 들어올 수 있으므로
            // UI Dispatcher에서 PropertyChanged 발생
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        OnPropertyChanged(
                            nameof(Value));
                    }));
        }

        private void OnPropertyChanged(
            [CallerMemberName]
            string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(
                    propertyName));
        }
    }
}