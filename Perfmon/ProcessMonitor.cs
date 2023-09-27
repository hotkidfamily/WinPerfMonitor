using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Perfmon
{
    internal class ProcessMonitor
    {
        private string _processName;
        private Process _process;
        private int _pid;
        private PerformanceCounter _cpuProc;
        private PerformanceCounter _vRamUsed;
        private PerformanceCounter _phyRamUsed;
        public ProcessMonitor(int pid) 
        {
            _process = System.Diagnostics.Process.GetProcessById(pid);
            _pid = pid;
            _processName = _process.ProcessName;

            _cpuProc = new PerformanceCounter();
            _vRamUsed = new PerformanceCounter();
            _phyRamUsed = new PerformanceCounter();
        }
    }
}
