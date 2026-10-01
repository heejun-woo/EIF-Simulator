
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EIF_Simulator.Core
{
    public interface IPlcBindable
    {
        ObservableCollection<PlcBinding> PlcBindings { get; }
    }
}