using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Simulator.Core
{
    public interface IPlcLogic
    {
        void Scan(PlcBindingManager manager);
    }
}
