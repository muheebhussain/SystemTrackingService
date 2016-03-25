using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ActivityTracker.Entity;

namespace ActivityTracker.Tracker
{
    public static class ActivityTracker
    {
        public static void WatchActivity()
        {
            //Get current process
            var currentProcess = Process.GetCurrentProcess();

            //Get Local Process
            var localProcess = Process.GetProcesses().OrderBy(process =>process.ProcessName);
            var processDetails = new List<SystemProcess>();
            var data = new List<string>();
            foreach (var process in localProcess)
            {
                var item = new SystemProcess()
                {
                    BasePriority = process?.BasePriority,
                    EnableRaisingEvents = process?.EnableRaisingEvents,
                    HandleCount = process?.HandleCount,
                    //HasExited = process?.HasExited,
                    Id = process?.Id,
                    MachineName = process?.MachineName,
                    MainWindowTitle = process?.MainWindowTitle,
                    ProcessName = process?.ProcessName,
                    SessionId = process?.SessionId,
                    //StartTime = process?.StartTime > DateTime.MinValue ? process.StartTime : DateTime.MinValue,
                    //TotalProcessorTime = process?.TotalProcessorTime > new TimeSpan(0,0,0) ? process.TotalProcessorTime : new TimeSpan(0,0,0),
                    //UserProcessorTime = process?.UserProcessorTime > new TimeSpan(0, 0, 0) ? process.UserProcessorTime : new TimeSpan(0, 0, 0)
                };
                data.Add(item.ToString());               
            }

            File.WriteAllLines(@"C:\CurrentProcess.txt", data.ToArray());

        }
    }
}
