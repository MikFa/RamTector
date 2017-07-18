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

    #10 Get github repository. 
  Create a "stats" method that supplies the helper functions with processes.Custom struct or object (can I use the built in process class?)
 */
namespace RamTector
{
    class Program
    {        
        static void Main(string[] args)
        {
            ProgramManager pm = new ProgramManager();
            pm.MainLoop();
        }

    }
}
