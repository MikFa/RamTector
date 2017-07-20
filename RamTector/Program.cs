using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
  Todo: 
    #1 Inorder to get the cpu use from each application I need to keep a list of each and their previous "times" inorder to calculate the current use. 
        - Not working Figure out how, I don't want to use performance counter 

    (Y)#2 https://stackoverflow.com/questions/1837962/asynchronous-delegates-vs-thread-threadpool
            Use threadpool to update the processes when we reach a certain number 

    #3 Implement so you can group multiple processes together. 
        -Skype has multiple smaller processes so they might want to be bundled 
        -allow for generic bundling
        -Show a list that helps pick the correct names. (with logos if possible) 
    #3.5 Allow custom naming for individual processes

    #4 Sort by name/ram/cpu

    #5 Add groups with different names, simply change process calculator .addprocess

    HP!#6 Make it so it only updates all processes once and then the groups gather from the pool what they need
    #7 Make it so that the update happens during an event called update. Make it async. 

    (y)#10 Get github repository. 
  Create a "stats" method that supplies the helper functions with processes.Custom struct or object (can I use the built in process class?)
 */
namespace RamTector
{
    class Program
    {        
        static void Main(string[] args)
        {
            //var counter = new PerformanceCounter();
            //counter.CategoryName = "Process";
            //counter.CounterName = "Working Set - Private";
            //var processes = Process.GetProcessesByName("chrome");
            //for (int i = 0; i < processes.Length - 1; i++)
            //{
            //    counter.InstanceName = processes[i].ProcessName;                
            //    Console.WriteLine("{1} {0}K", counter.RawValue / 1024, counter.InstanceName);
            //}
            //counter.InstanceName = "chrome";
            //Console.WriteLine("{1} {0}K", counter.RawValue / 1024, counter.InstanceName);

            //counter.InstanceName = "chrome#1";
            //Console.WriteLine("{1} {0}K", counter.RawValue / 1024, counter.InstanceName);
            //Console.ReadLine();

            ProgramManager pm = new ProgramManager();
            pm.MainLoop();
        }

    }
}
