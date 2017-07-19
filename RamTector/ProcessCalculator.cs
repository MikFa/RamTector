using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RamTector
{
    public class ProcessCalculator
    {
        #region fields
        public static TimeSpan refreshTime;
        public event UpdateHandler Update;
        public delegate void UpdateHandler(object sender, EventArgs e);
        //private Stopwatch cpuWatch;
        private long totalCpuUtilization;
        //private TimeSpan totalCpuTime;
        //private PerformanceCounter ramCounter;
        //private PerformanceCounter cpuCounter;
        private PerformanceCounter totalCpuCounter;
        #endregion

        public ProcessCalculator()
        {
            WatchedProcesses = new Dictionary<string, WachtedProcess>();
            TotalCpuUtilization = 0;
            refreshTime = new TimeSpan(0, 0, 0, 0, 1000);


            totalCpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        #region properties
        public Dictionary<string, WachtedProcess> WatchedProcesses { get; private set; }

        public TimeSpan RefreshTime { get { return refreshTime; } set { refreshTime = value; } }

        public long TotalCpuUtilization { get => totalCpuUtilization; set => totalCpuUtilization = value; }

        public TimeSpan previousTotalCpuTime { get; private set; }
        #endregion

        #region methods
        public bool UpdateWatchedProcesses()
        {
            var processes = Process.GetProcesses();
            UpdateTotalCpu();
            foreach (var watchedProcess in WatchedProcesses)
            {
                Process[] newProcess = watchedProcess.Value.Grouped ? processes.Where(c => c.ProcessName.ToLower().Contains(watchedProcess.Key.ToLower())).ToArray() : processes.Where(c => c.ProcessName.ToLower() == watchedProcess.Key.ToLower()).ToArray();
                PassedValue p = new PassedValue(watchedProcess.Value, newProcess);

                ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWatchedProcess), p);
            }
            return true;
        }

        private void UpdateWatchedProcess(object a)
        {
            PassedValue pV = (PassedValue)a;
            pV.p.Memory.SetNewMemoryValue(CalculateProcessRam(pV.ps));
            pV.p.CpuProcessPercent = CalculateProcessCpuTime(pV.ps);
        }

        public StatusCode AddProcess(string applicationName, bool group = false)
        {
            var processes = group ? Process.GetProcesses().Where(c => c.ProcessName.ToLower().Contains(applicationName.ToLower())).ToArray() : Process.GetProcessesByName(applicationName);
            if (processes.Length <= 0)
            {
                return StatusCode.UnableToFindProcess;
            }

            if (WatchedProcesses.ContainsKey(applicationName)) { return StatusCode.AlreadyExsists; }

            WatchedProcesses.Add(applicationName, new WachtedProcess(CalculateProcessRam(processes), CalculateProcessCpuTime(processes), group, processes.Count()));

            return StatusCode.Success;
        }

        private Memory CalculateProcessRam(Process[] processes)
        {
            Memory m = new Memory();
            try
            {
                var ramCounter = new PerformanceCounter();
                ramCounter.CategoryName = "Process";
                ramCounter.CounterName = "Working Set - Private";
                long totalRam = 0;
                if (processes.Length > 1)
                {
                    foreach (var item in processes)
                    {
                        ramCounter.InstanceName = item.ProcessName;
                        totalRam += ramCounter.RawValue;
                    }
                }
                else
                {
                    ramCounter.InstanceName = processes[0].ProcessName;
                    totalRam = ramCounter.RawValue;
                }
                m = new Memory(totalRam, BytePrefix.Byte);
                //m.ConvertPrefix(BytePrefix.KiloBytes);
            }
            catch (System.ComponentModel.Win32Exception e) { }
            catch (Exception e) { throw e; }
            return m;
        }

        private long CalculateProcessCpuTime(Process[] processes)
        {
            long totalCpu = 0;
            try
            {
                var cpuCounter = new PerformanceCounter();
                cpuCounter.CategoryName = "Process";
                cpuCounter.CounterName = "% Processor Time";
                if (processes.Length > 1)
                {
                    foreach (var item in processes)
                    {
                        cpuCounter.InstanceName = item.ProcessName;
                        totalCpu += cpuCounter.RawValue;
                    }
                }
                else
                {
                    cpuCounter.InstanceName = processes[0].ProcessName;
                    totalCpu = cpuCounter.RawValue;
                }
            }
            catch (System.ComponentModel.Win32Exception) { }
            catch (Exception e) { throw e; }

            return totalCpu;
        }

        private void UpdateTotalCpu()
        {
            try
            {
                this.TotalCpuUtilization = totalCpuCounter.RawValue;
            }
            catch (System.ComponentModel.Win32Exception e) { }
            catch (Exception) { throw; }
        }
        #endregion
    }

    public struct PassedValue
    {
        public WachtedProcess p { get; private set; }
        public Process[] ps { get; private set; }

        public PassedValue(WachtedProcess p, Process[] ps)
        {
            this.p = p;
            this.ps = ps;
        }

    }


}
