using EIF_Simulator.Controls.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EIF_Simulator.Controls
{
    public static class PlcContextProvider
    {
        public static readonly DependencyProperty PlcContextProperty =
            DependencyProperty.RegisterAttached(
                "PlcContext",
                typeof(PlcContext),
                typeof(PlcContextProvider),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static void SetPlcContext(
            DependencyObject element,
            PlcContext value)
        {
            element.SetValue(PlcContextProperty, value);
        }

        public static PlcContext GetPlcContext(
            DependencyObject element)
        {
            return (PlcContext)element.GetValue(
                PlcContextProperty);
        }
    }
}
