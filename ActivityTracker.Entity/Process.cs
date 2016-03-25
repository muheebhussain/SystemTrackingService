using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityTracker.Entity
{
    public class SystemProcess
    {
        public int? BasePriority { get; set; }
        public bool? EnableRaisingEvents { get; set; }
        public int? ExitCode { get; set; }
        public DateTime? ExitTime { get; set; }
        public int? HandleCount { get; set; }
        public bool? HasExited { get; set; }
        public int? Id { get; set; }
        public string MachineName { get; set; }
        public string MainWindowTitle { get; set; }
        public int NonpagedSystemMemorySize { get; set; }
        public long NonpagedSystemMemorySize64 { get; set; }
        public int PagedMemorySize { get; set; }
        public long PagedMemorySize64 { get; set; }
        public int PagedSystemMemorySize { get; set; }
        public long PagedSystemMemorySize64 { get; set; }
        public int PeakPagedMemorySize { get; set; }
        public long PeakPagedMemorySize64 { get; set; }
        public int PeakVirtualMemorySize { get; set; }
        public long PeakVirtualMemorySize64 { get; set; }
        public int PeakWorkingSet { get; set; }
        public long PeakWorkingSet64 { get; set; }
        public bool PriorityBoostEnabled { get; set; }
        public int PrivateMemorySize { get; set; }
        public long PrivateMemorySize64 { get; set; }
        public TimeSpan PrivilegedProcessorTime { get; set; }
        public string ProcessName { get; set; }
        public bool Responding { get; set; }
        public int? SessionId { get; set; }
        public StreamReader StandardError { get; set; }
        public StreamWriter StandardInput { get; set; }
        public StreamReader StandardOutput { get; set; }
        public DateTime? StartTime { get; set; }
        public TimeSpan? TotalProcessorTime { get; set; }
        public TimeSpan? UserProcessorTime { get; set; }
        public int VirtualMemorySize { get; set; }
        public long VirtualMemorySize64 { get; set; }
        public int WorkingSet { get; set; }
        public long WorkingSet64 { get; set; }

        public override string ToString()
        {
            //return $"{BasePriority} { HandleCount} " +
            //        $"{HasExited} {Id} { MachineName} {MainWindowTitle} {ProcessName } {SessionId } {StartTime } {TotalProcessorTime } { UserProcessorTime}";

            return $"Id: {Id} WindowTitle: {MainWindowTitle} ProcessName: {ProcessName }";

        }
    }
}