using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RamTector
{
    public interface IWatchedProcess
    {
        Memory Memory { get; }
        float CpuProcessorPercent { get; }        
        void Update(object a);        
    }
}
