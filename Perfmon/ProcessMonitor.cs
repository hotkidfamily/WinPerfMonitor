using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace Perfmon
{
    delegate void UpdateMonitorStatusDelegate(ref RunStatusItem status);

    internal class RunStatusItem
    {
        public uint pid = 0;
        public string procName = "undefined";
        public double cpu = 0;
        public double vMem = 0;
        public double phyMem = 0;
        public double totalMem = 0;
        public double downLink = 0;
        public double upLink = 0;
        public double totalLinkFlow = 0;
        public long excuteSeconds = 0;
        public string excuteStatus = "no exist";
        
        public string[] info()
        {
            string uposfix = " Kbps";
            string dposfix = " Kbps";
            double total = totalLinkFlow / 1024.0f;
            double up = upLink * 8;
            double down = downLink * 8;

            if(up > 1 << 10)
            {
                up /= 1024.0f;
                uposfix = " Mbps";
            }

            if (down > 1 << 10)
            {
                down /= 1024.0f;
                dposfix = " Mbps";
            }

            return new string[] {
                $"{pid}",
                procName,
                $"{cpu :F2}",
                $"{vMem/1024 :F2} GB",
                $"{phyMem :F2} MB",
                $"{totalMem/1024 :F2} GB",
                $"{up :F2}{uposfix}",
                $"{down :F2}{dposfix}",
                $"{total :F2} MB",
                $"{TimeSpan.FromSeconds(excuteSeconds).ToString()} s",
                $"{excuteStatus}"
            };
        }
    }

    internal class NetspeedTrace {
        public long send = 0;
        public long received = 0;
    };

    internal class ProcessMonitor : IDisposable
    {
        private readonly uint _pid = 0;

        private readonly int _interval = 1000;
        private bool _endTask = true;
        private readonly Task? _task;

        private RunStatusItem _onceRes = new();

        private readonly Process? _process;

        private readonly UpdateMonitorStatusDelegate? _updateMonitorStatus;

        private TraceEventSession? _netTraceSession;
        private NetspeedTrace _netspeedDetail = new();
        private NetspeedTrace _netspeedDetailOld = new();

        void ProcessExitEventHandler(object? sender, EventArgs e)
        {
            _endTask = true;
            _task?.Wait();
            _onceRes.excuteStatus = "exit";

            _updateMonitorStatus?.Invoke(ref _onceRes);
        }

        public ProcessMonitor(uint pid, int interval, UpdateMonitorStatusDelegate UpdateHandle) 
        {
            _pid = pid;
            _interval = interval;
            _onceRes.excuteStatus = "running";

            try
            {
                _process = Process.GetProcessById((int)pid);
            }
            catch (ArgumentException)
            {
                _onceRes.excuteStatus = "no exist";
            }
            if (_process != null)
            {
                _endTask = false;
                _onceRes.pid = _pid;
                _onceRes.procName = _process.ProcessName;

                _updateMonitorStatus = UpdateHandle;
                _process.EnableRaisingEvents = true;
                _process.Exited += new EventHandler(ProcessExitEventHandler);

                _task = new Task(() =>
                {
                    long firstMonitorTicks = Environment.TickCount64;
                    long lastMonitorTicks = 0;
                    double lastProcessorTime = 0;
                    double cores = 100.0f / Environment.ProcessorCount;
                    NetspeedTrace netspeedTracer = new();

                    while (!_endTask)
                    {
                        _onceRes.vMem = _process.VirtualMemorySize64 / 1048576.0f;
                        _onceRes.phyMem = _process.WorkingSet64 / 1048576.0f;
                        _onceRes.totalMem = _onceRes.vMem + _onceRes.phyMem;

                        double nowProcessorTime = _process.TotalProcessorTime.TotalMilliseconds;
                        long nowTicks = Environment.TickCount64;
                        if (lastMonitorTicks == 0)
                        {
                            lastMonitorTicks = nowTicks - 1000;
                            lastProcessorTime = nowProcessorTime;
                        }
                    
                        _onceRes.cpu = Math.Round((nowProcessorTime - lastProcessorTime) * cores / (nowTicks - lastMonitorTicks), 2);
                        _onceRes.excuteSeconds = (nowTicks - firstMonitorTicks)/1000;
                        lastMonitorTicks = nowTicks;
                        lastProcessorTime = nowProcessorTime;

                        {
                            netspeedTracer.send = _netspeedDetail.send;
                            netspeedTracer.received = _netspeedDetail.received;

                            _onceRes.upLink = (netspeedTracer.send - _netspeedDetailOld.send) / 1024.0f;
                            _onceRes.downLink = (netspeedTracer.received - _netspeedDetailOld.received) / 1024.0f;
                            _onceRes.totalLinkFlow = (netspeedTracer.send + _netspeedDetailOld.received) / 1024.0f;

                            _netspeedDetailOld.send = netspeedTracer.send;
                            _netspeedDetailOld.received = netspeedTracer.received;
                        }

                        _updateMonitorStatus?.Invoke(ref _onceRes);

                        Thread.Sleep(TimeSpan.FromMilliseconds(_interval));
                    }
                });

                _task.Start();

                Task.Run(() => { StartEtwSession(); });
            }
        }

        private void StartEtwSession()
        {
            try
            {
                var processId = _pid;

                using (_netTraceSession = new TraceEventSession("Perfmon_KernelAndClrEventsSession"))
                {
                    _netTraceSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);

                    _netTraceSession.Source.Kernel.TcpIpRecv += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            _netspeedDetail.received += data.size;
                        }
                    };

                    _netTraceSession.Source.Kernel.TcpIpSend += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            _netspeedDetail.send += data.size;
                        }
                    };

                    _netTraceSession.Source.Kernel.UdpIpRecv += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            _netspeedDetail.received += data.size;
                        }
                    };

                    _netTraceSession.Source.Kernel.UdpIpSend += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            _netspeedDetail.send += data.size;
                        }
                    };

                    _netTraceSession.Source.Process();
                }
            }
            catch
            {
                _netspeedDetail.send = 0;
                _netspeedDetail.received = 0;
            }
        }

        public void Dispose()
        {
            _endTask = true;
            _task?.Wait();
            _netTraceSession?.Dispose();
        }
    }
}
