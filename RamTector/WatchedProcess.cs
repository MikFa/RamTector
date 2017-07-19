using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RamTector
{
    public class WachtedProcess : IProcessGroup
    {
        public Memory Memory { get; set; }
        public long CpuProcessPercent { get; set; }
        public decimal CpuPercent { get; set; }
        public bool Grouped { get; private set; }
        public int GroupCount { get; private set; }

        #region performance counters
        #endregion

        public WachtedProcess(Memory memory, long cpuTime)
        {
            this.Memory = memory;
            this.CpuProcessPercent = cpuTime;

        }
        public WachtedProcess(Memory memory, long cpuTime, bool grouped, int groupCount)
        {
            this.Memory = memory;
            this.CpuProcessPercent = cpuTime;
            this.Grouped = grouped;
            this.GroupCount = groupCount;
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
