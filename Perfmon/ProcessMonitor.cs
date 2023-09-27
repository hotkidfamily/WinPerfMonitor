using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System.Diagnostics;

namespace Perfmon
{
    delegate void UpdateMonitorStatusDelegate(uint pid, ref RunStatusItem status);

    internal class RunStatusItem
    {
        public uint pid = 0;
        public string procName = "undefined";
        public float cpu = 0;
        public long vMem = 0;
        public long phyMem = 0;
        public long totalMem = 0;
        public float downLink = 0;
        public float upLink = 0;
        public float totoalLinkFlow = 0;
        public uint excuteStatus = 0;

        public string[] info()
        {
            return new string[] {
                $"{pid}",
                procName,
                $"{cpu:F2}",
                $"{vMem}",
                $"{phyMem}",
                $"{totalMem}",
                $"{upLink}",
                $"{downLink}",
                $"{totoalLinkFlow}",
                $"{excuteStatus}"
            };
        }
    }

    internal class TcpipTrace {
        public long received = 0;
        public long send = 0;
    };

    internal class ProcessMonitor: IDisposable
    {
        private readonly uint _pid = 0;
        private readonly int _interval = 1000;

        private RunStatusItem _onceRes = new();

        private Process _process;
        private PerformanceCounter _cpuProc;
        private PerformanceCounter _vRamUsed;
        private PerformanceCounter _phyRamUsed;

        private UpdateMonitorStatusDelegate _updateMonitorStatus;

        private bool _endTask = true;
        private readonly Task _task;
        private TraceEventSession? _netTraceSession;
        private TcpipTrace _netTraceDetail = new();

        public ProcessMonitor(uint pid, int interval, UpdateMonitorStatusDelegate UpdateHandle) 
        {
            _pid = pid;
            _interval = interval;
            _process = System.Diagnostics.Process.GetProcessById((int)pid);

            _endTask = false;
            _onceRes.pid = _pid;
            _onceRes.procName = _process.ProcessName;

            _cpuProc = new PerformanceCounter();
            _vRamUsed = new PerformanceCounter();
            _phyRamUsed = new PerformanceCounter();

            _updateMonitorStatus = UpdateHandle;

            _task = new Task(() =>
            {
                TimeSpan lastCpuTime = new();
                while (!_endTask)
                {
                    _onceRes.vMem = _process.VirtualMemorySize64 >> 20;
                    _onceRes.phyMem = _process.WorkingSet64 >> 20;
                    _onceRes.totalMem = _onceRes.vMem + _onceRes.phyMem;
                    _onceRes.cpu = (_process.TotalProcessorTime - lastCpuTime).Milliseconds / 10;

                    lock(_netTraceDetail)
                    {
                        _onceRes.upLink = _netTraceDetail.send;
                        _onceRes.downLink = _netTraceDetail.received;
                    }
                    
                    _updateMonitorStatus(_pid, ref _onceRes);

                    Thread.Sleep(TimeSpan.FromMilliseconds(_interval));

                    System.Console.WriteLine("monitor running...");
                }
            });

            _task.Start();

            Task.Run(() => { StartEtwSession(); });
        }
        private void StartEtwSession()
        {
            try
            {
                var processId = Process.GetCurrentProcess().Id;
                lock (_netTraceDetail)
                {
                    _netTraceDetail.send = 0;
                    _netTraceDetail.received = 0;
                }

                using (_netTraceSession = new TraceEventSession("Perfmon_KernelAndClrEventsSession"))
                {
                    _netTraceSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);

                    _netTraceSession.Source.Kernel.TcpIpRecv += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            lock (_netTraceDetail)
                            {
                                _netTraceDetail.received += data.size;
                            }
                        }
                    };

                    _netTraceSession.Source.Kernel.TcpIpSend += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            lock (_netTraceDetail)
                            {
                                _netTraceDetail.send += data.size;
                            }
                        }
                    };

                    _netTraceSession.Source.Process();
                }
            }
            catch
            {
                lock (_netTraceDetail)
                {
                    _netTraceDetail.send = 0;
                    _netTraceDetail.received = 0;
                }
            }
        }

        public void Dispose()
        {
            _endTask = true;
            _task.Wait();
            _netTraceSession?.Dispose();
        }
    }
}
