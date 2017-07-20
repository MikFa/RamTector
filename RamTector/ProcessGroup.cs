using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RamTector
{
    class ProcessGroup : IWatchedProcess
    {
        public List<IProcessGroupMember> GroupMembers { get; set; }

        public new Memory Memory { get; private set; }

        public new float CpuProcessorPercent { get; private set; }

        public string GroupName { get; set; }

        public ManualResetEvent DoneEvent { get; private set; }

        public ProcessGroup(string groupName)
        {
            this.GroupName = groupName;
            this.Memory = new Memory();
            this.CpuProcessorPercent = 0;
        }

        public void Update(object a)
        {
            try
            {
                this.Memory.ResetMemory();
                foreach (IProcessGroupMember gm in GroupMembers)
                {
                    if(!CheckIfProcessAlive(gm.InstanceName)) { gm.Reset(); continue; }
                    gm.Update();
                    this.Memory.AddMemoryValue(gm.Memory);
                    this.CpuProcessorPercent = gm.CpuProcessorPercent;
                }
            }
            catch (Exception e){throw e;}

            var re = (PassedValue)a;
            re.DoneEvent.Set();
        }

        public bool CheckIfProcessAlive(string name)
        {
            return Process.GetProcessesByName(name).Count() > 0 ? true : false;
        }

    }
}
