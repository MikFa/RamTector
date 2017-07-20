using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RamTector
{
    public class WachtedProcess : IProcessGroupMember
    {
        public new Memory Memory { get; private set; }
        public string InstanceName { get; private set; }


        #region performance counters
        private List<PerformanceCounter> ramCounter;
        private List<PerformanceCounter> cpuCounter;
        private List<PerformanceCounter> CpuCounter { get; set; }
        private List<PerformanceCounter> RamCounter { get; set; }

        public ManualResetEvent DoneEvent { get; private set; }

        public new float CpuProcessorPercent { get; private set; }
        #endregion

        public WachtedProcess(string instanceName)
        {
            this.InstanceName = instanceName;
            this.Memory = new Memory();
            this.CpuProcessorPercent = 0;
            InitializeCounters();
        }

        public void InitializeCounters()
        {
            PerformanceCounterCategory categories = new PerformanceCounterCategory("Process");
            var processes = categories.GetInstanceNames().Where(c => c.ToLower().Contains(InstanceName.ToLower())).ToArray();
            cpuCounter = new List<PerformanceCounter>(processes.Count());
            ramCounter = new List<PerformanceCounter>(processes.Count());
            for (int i = 0; i < processes.Count(); i++)
            {
                ramCounter.Add(new PerformanceCounter("Process", "Working Set - Private", processes[i], true));
                cpuCounter.Add(new PerformanceCounter("Process", "% Processor Time", processes[i], true));
            }
        }

        public void Update()
        {
            try
            {
                float totalRam = 0, totalCpu = 0;
                Process[] processes = Process.GetProcessesByName(this.InstanceName);
                for (int i = 0; i < processes.Length - 1; i++)
                {
                    //this.RamCounter.InstanceName = this.InstanceName + "#" + i;
                    //this.CpuCounter.InstanceName = this.InstanceName + "#" + i;
                    totalRam += ramCounter[i].NextValue();
                    totalCpu += cpuCounter[i].NextValue();
                }
                this.Memory.SetNewMemoryValue(totalRam, BytePrefix.Byte);
                this.CpuProcessorPercent = totalCpu;
            }
            catch (System.ComponentModel.Win32Exception) { }
            catch (Exception e) { throw e; }
            this.DoneEvent.Set();
        }

        public void Update(object a)
        {
            try
            {
                float totalRam = 0, totalCpu = 0;
                Process[] processes = Process.GetProcessesByName(InstanceName);

                if (!CheckIfProcessAlive(InstanceName)) { Reset(); return; }
                RefreshCounterSize(processes.Count());
                for (int i = 0; i < processes.Count() ; i++)
                {                    
                    totalRam += ramCounter[i].NextValue();
                    totalCpu += cpuCounter[i].NextValue();
                }
                this.Memory.SetNewMemoryValue(totalRam, BytePrefix.Byte);
                this.CpuProcessorPercent = totalCpu;
            }
            catch (System.ComponentModel.Win32Exception) { }
            catch (Exception e) { throw e; }

            var re = (PassedValue)a;
            re.DoneEvent.Set();
        }

        public void RefreshCounterSize(int count )
        {
            if (count > ramCounter.Count())
            {
                for (int i = ramCounter.Count(); i < count; i++)
                {
                    ramCounter.Add(new PerformanceCounter("Process", "Working Set - Private", this.InstanceName + i, true));
                    cpuCounter.Add(new PerformanceCounter("Process", "% Processor Time", this.InstanceName + i, true));
                }
            }
        }

        public bool CheckIfProcessAlive(string name)
        {
            return Process.GetProcessesByName(name).Count() > 0 ? true : false;
        }

        public void Reset()
        {
            this.Memory.ResetMemory();
            this.CpuProcessorPercent = 0;
        }
    }
}
