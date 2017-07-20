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
        public event UpdateHandler  Update; //here to remind me to make update "event" instead of looping. But one thing at a time! #Learning
        public delegate void UpdateHandler(object sender, EventArgs e);
        private float totalCpuUtilization;
        private PerformanceCounter totalCpuCounter;
        #endregion

        public ProcessCalculator()
        {
            WatchedProcesses = new Dictionary<string, IWatchedProcess>();
            TotalCpuUtilization = 0;
            refreshTime = new TimeSpan(0, 0, 0, 0, 1000);


            totalCpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        #region properties
        public Dictionary<string, IWatchedProcess> WatchedProcesses { get; set; }

        public TimeSpan RefreshTime { get { return refreshTime; } set { refreshTime = value; } }

        public float TotalCpuUtilization { get => totalCpuUtilization; set => totalCpuUtilization = value; }

        public TimeSpan previousTotalCpuTime { get; private set; }
        #endregion

        #region methods
        public bool UpdateWatchedProcesses()
        {
            var processes = Process.GetProcesses();
            ManualResetEvent[] doneEvents = new ManualResetEvent[WatchedProcesses.Count];
            UpdateTotalCpu();
            int index = 0; 
            foreach (var group in WatchedProcesses)
            {
                doneEvents[index] = new ManualResetEvent(false);
                PassedValue p = new PassedValue(doneEvents[index]);
                index++;
                //Process[] newProcess = group.Value.GroupMembers.Count < 1 ? processes.Where(c => c.ProcessName.ToLower().Contains(group.Key.ToLower())).ToArray() : processes.Where(c => c.ProcessName.ToLower() == group.Key.ToLower()).ToArray();
                    
                ThreadPool.QueueUserWorkItem(new WaitCallback(group.Value.Update), p);
            }
            if (doneEvents.Length > 0) { WaitHandle.WaitAll(doneEvents); }            
            return true;
        }

        public StatusCode AddProcess(string groupName, bool group = false)
        {
            var processes = group ? Process.GetProcesses().Where(c => c.ProcessName.ToLower().Contains(groupName.ToLower())).ToArray() : Process.GetProcessesByName(groupName);
            if (processes.Length <= 0)
            {
                return StatusCode.UnableToFindProcess;
            }

            if (WatchedProcesses.ContainsKey(groupName)) { return StatusCode.AlreadyExsists; }
            
            WatchedProcesses.Add(groupName, NewIWatchedProcess(group, groupName));

            return StatusCode.Success;
        }

        public IWatchedProcess NewIWatchedProcess(bool group, string name)
        {
            if (group) { return new ProcessGroup(name); } else { return new WachtedProcess(name); }
        }

        private void UpdateTotalCpu()
        {
            try
            {
                this.TotalCpuUtilization = totalCpuCounter.NextValue();
            }
            catch (System.ComponentModel.Win32Exception) { }
            catch (Exception e ) { throw e; }
        }
        #endregion
    }

    public struct PassedValue
    {        
        public ManualResetEvent DoneEvent { get; set; }

        public PassedValue(ManualResetEvent doneEvent)
        {
            this.DoneEvent = doneEvent;
        }
    }


}
