using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RamTector
{
    class ProgramManager
    {
        ProcessCalculator processCalc;
        bool _showCpu = true;
        public ProgramManager()
        {
            processCalc = new ProcessCalculator();
            processCalc.RefreshTime = new TimeSpan(0, 0, 0, 2, 0);
            processCalc.AddProcess("Chrome");
            //processCalc.AddProcess("Skype", true); // Fix if you added something that is not there so that if it is there then it will be there and if you close a application it no crash
            //processCalc.AddProcess("ramtector");
            //processCalc.AddProcess("steam");
            //processCalc.AddProcess("Firefox");
            processCalc.AddProcess("Notepad");
            //processCalc.AddProcess("Microsoft.StickyNotes");
            //processCalc.AddProcess("Calculator");
            //processCalc.AddProcess("MSIAfterBurner");
            //processCalc.AddProcess("Taskmgr");
            //processCalc.AddProcess("Ressource Monitor");
            //processCalc.AddProcess("powershell");
            //processCalc.AddProcess("skype");
            //processCalc.AddProcess("Windows Explorer");
            //processCalc.AddProcess("texstudio");
            //processCalc.AddProcess("Discord");
            //processCalc.AddProcess("devenv");
            //processCalc.AddProcess("TeamViewer");
            //processCalc.AddProcess("EthDcrMiner64");
            //processCalc.AddProcess("SpaceEngineers");
        }

        public void MainLoop()
        {
            while (true)
            {
                Console.Clear();
                var res = processCalc.UpdateWatchedProcesses();
                Console.WriteLine($"{(res ? "All processes was refreshed successully" : "Something went wrong during the refresh")} Refreshing every {processCalc.RefreshTime.Seconds} second{(processCalc.RefreshTime.Seconds> 1 ? "s" : "" )} \n");
                PrintResults(processCalc.WatchedProcesses);
                Thread.Sleep(processCalc.RefreshTime);
            }
        }
        private void PrintResults(Dictionary<string, IWatchedProcess> dict)
        {            
            foreach (var item in dict)
            {
                Console.WriteLine($"{item.Key} " +
                    $"Ram: {item.Value.Memory.MemValue} {item.Value.Memory.Prefix} " +
                    $"{(_showCpu ? $"CPU: {Math.Round(item.Value.CpuProcessorPercent, 2)}" : "")} " +
                    $"{(item.Value is IProcessGroup ? $"Group Size {((IProcessGroup)item.Value).GroupMembers.Count()}" : "")}");
            }
            Console.WriteLine($"{(_showCpu ? $"\n\nTotal Cpu: {processCalc.TotalCpuUtilization}": "")}");
        }
    }
}
