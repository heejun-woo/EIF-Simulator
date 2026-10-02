using System;
using System.Windows;
using System.Windows.Data;

namespace EIF_Simulator.OpcUa
{
    public static class OpcUaBindingExtensions
    {
        public static void Bind(
            this OpcUaBindingManager manager,
            DependencyObject target,
            DependencyProperty targetProperty,
            string tagName,
            BindingMode mode = BindingMode.TwoWay)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (string.IsNullOrWhiteSpace(tagName))
                throw new ArgumentException(
                    "Tag name cannot be empty.",
                    nameof(tagName));

            if (!manager.Contains(tagName))
            {
                throw new InvalidOperationException(
                    $"OPC UA tag is not registered: {tagName}");
            }

            var source =
                new OpcUaBindingSource(
                    manager,
                    tagName);

            var binding =
                new Binding(nameof(OpcUaBindingSource.Value))
                {
                    Source = source,
                    Mode = mode,
                    UpdateSourceTrigger =
                        UpdateSourceTrigger.PropertyChanged
                };

            BindingOperations.SetBinding(
                target,
                targetProperty,
                binding);
        }
    }
}