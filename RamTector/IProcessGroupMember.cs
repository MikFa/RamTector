using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RamTector
{
    public interface IProcessGroupMember : IWatchedProcess
    {
        string InstanceName { get;}
        void Update();
        void InitializeCounters();
        void Reset();
    }
}
