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
        private decimal totalCpuUtilization;
        private TimeSpan totalCpuTime;
        #endregion

        public ProcessCalculator()
        {
            WatchedProcesses = new Dictionary<string, WachtedProcess>();
            TotalCpuUtilization = 0;
            totalCpuTime = new TimeSpan();
            refreshTime = new TimeSpan(0, 0, 0, 0, 1000);
        }

        #region properties
        public Dictionary<string, WachtedProcess> WatchedProcesses { get; private set; }

        public TimeSpan RefreshTime { get { return refreshTime; } set { refreshTime = value; } }

        public decimal TotalCpuUtilization { get => totalCpuUtilization; set => totalCpuUtilization = value; }

        public TimeSpan previousTotalCpuTime { get; private set; }
        #endregion

        #region methods
        public bool UpdateWatchedProcesses()
        {
            var processes = Process.GetProcesses();
            UpdateTotalCpu(processes);
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
            pV.p.CalculateApplicationCPUTime(pV.ps, this.RefreshTime);
        }

        public StatusCode AddProcess(string applicationName, bool group = false)
        {
            var processes = group ? Process.GetProcesses().Where(c => c.ProcessName.ToLower().Contains(applicationName.ToLower())).ToArray() : Process.GetProcessesByName(applicationName);
            if (processes.Length < 0)
            {
                return StatusCode.UnableToFindProcess;
            }
            var processArray = Process.GetProcessesByName(applicationName);

            if (WatchedProcesses.ContainsKey(applicationName)) { return StatusCode.AlreadyExsists; }

            WatchedProcesses.Add(applicationName, new WachtedProcess(CalculateProcessRam(processArray), CalculateProcessCpuTime(processArray), group, processes.Count()));

            return StatusCode.Success;
        }

        private Memory CalculateProcessRam(Process[] processes)
        {
            try
            {
                decimal totalRam = 0;
                foreach (var item in processes)
                {
                    totalRam += item.VirtualMemorySize64 ;
                }
                Memory m = new Memory(totalRam, BytePrefix.Byte);
                m.ConvertPrefix(BytePrefix.KiloBytes);
                return m;
            }
            catch (Exception e) { throw e; }
        }

        private TimeSpan CalculateProcessCpuTime(Process[] processes)
        {
            TimeSpan totalProcessCpuTime = new TimeSpan();
            try
            {
                foreach (var item in processes)
                {
                    totalProcessCpuTime = totalProcessCpuTime.Add(item.TotalProcessorTime);
                }
            }
            catch (System.ComponentModel.Win32Exception) { }
            catch (Exception e) { throw e; }

            return totalProcessCpuTime;
        }

        private void UpdateTotalCpu(Process[] processes)
        {
            TimeSpan val = new TimeSpan();
            try
            {
                foreach (Process p in processes)
                {
                    val = val.Add(p.TotalProcessorTime);
                }
            }
            catch (System.ComponentModel.Win32Exception e) { }
            catch (Exception) { throw; }
            TotalCpuUtilization = (decimal)((val.TotalMilliseconds - previousTotalCpuTime.TotalMilliseconds) / this.RefreshTime.TotalMilliseconds) * 100;
            previousTotalCpuTime = totalCpuTime;
            totalCpuTime = val;            
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

    public class WachtedProcess
    {
        public Memory Memory { get; set; }
        public TimeSpan cpuTime { get; set; }
        public decimal CpuPercent { get; set; }
        public bool Grouped { get; private set; }
        public int GroupCount { get; private set; }

        public WachtedProcess(Memory memory, TimeSpan cpuTime, decimal cpuPercent = 0)
        {
            this.Memory = memory;
            this.cpuTime = cpuTime;
            this.CpuPercent = 0;
        }
        public WachtedProcess(Memory memory, TimeSpan cpuTime, bool grouped, int groupCount)
        {
            this.Memory = memory;
            this.cpuTime = cpuTime;
            this.CpuPercent = 0;
            this.Grouped = grouped;
            this.GroupCount = groupCount;
        }

        public void CalculateApplicationCPUTime(Process[] processes, TimeSpan refreshTime)
        {
            TimeSpan newAppCpuTime = new TimeSpan();
            try
            {
                foreach (var item in processes)
                {
                    newAppCpuTime = newAppCpuTime.Add(item.UserProcessorTime);
                }
            }
            catch (System.ComponentModel.Win32Exception) { }
            catch (Exception e) { throw e; }

            this.CpuPercent = ((decimal)(newAppCpuTime.TotalMilliseconds - this.cpuTime.TotalMilliseconds) / (decimal)refreshTime.TotalMilliseconds) * 100;
            cpuTime = newAppCpuTime;
        }

    }
}
