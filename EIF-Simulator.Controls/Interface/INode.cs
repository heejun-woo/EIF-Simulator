using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EIF_Simulator.Controls;

namespace EIF_Simulator.Controls
{
    public interface INode
    {
        string Id { get; }

        Point GetPosition();

        bool IsOccupied { get; }

        CarrierControl? Carrier { get; }


        bool TryEnter(CarrierControl carrier);

        void Leave();
    }
}
