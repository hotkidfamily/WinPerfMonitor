using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Diagnostics;

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
        public uint excuteStatus = 0;

        public string[] info()
        {
            string uposfix = " Kbps";
            string dposfix = " Kbps";
            double total = totalLinkFlow /= 1024.0f;
            double up = upLink;
            double down = downLink;

            if(upLink > 1 << 10)
            {
                up /= 1024.0f;
                uposfix = " Mbps";
            }

            if (downLink > 1 << 10)
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
                $"{total :F2} Mb",
                $"{excuteStatus}"
            };
        }
    }

    internal class TcpipTrace {
        public long send = 0;
        public long received = 0;
    };

    internal class ProcessMonitor: IDisposable
    {
        private readonly uint _pid = 0;
        private readonly int _interval = 1000;

        private RunStatusItem _onceRes = new();

        private Process _process;

        private UpdateMonitorStatusDelegate _updateMonitorStatus;

        private bool _endTask = true;
        private readonly Task _task;

        private TraceEventSession? _netTraceSession;
        private TcpipTrace _netTraceDetail = new();
        private TcpipTrace _netTraceOld = new();

        public ProcessMonitor(uint pid, int interval, UpdateMonitorStatusDelegate UpdateHandle) 
        {
            _pid = pid;
            _interval = interval;
            _process = System.Diagnostics.Process.GetProcessById((int)pid);

            _endTask = false;
            _onceRes.pid = _pid;
            _onceRes.procName = _process.ProcessName;

            _updateMonitorStatus = UpdateHandle;

            _task = new Task(() =>
            {
                long lastMonitorTicks = 0;
                double lastProcessorTime = 0;
                double cores = 100.0f / Environment.ProcessorCount;
                TcpipTrace tcpipTrace = new TcpipTrace();

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

                    lastMonitorTicks = nowTicks;
                    lastProcessorTime = nowProcessorTime;

                    {
                        tcpipTrace.send = _netTraceDetail.send;
                        tcpipTrace.received = _netTraceDetail.received;

                        _onceRes.upLink = (tcpipTrace.send - _netTraceOld.send) / 1024.0f;
                        _onceRes.downLink = (tcpipTrace.received - _netTraceOld.received) / 1024.0f;
                        _onceRes.totalLinkFlow = (tcpipTrace.send + _netTraceOld.received) / 1024.0f;

                        _netTraceOld.send = tcpipTrace.send;
                        _netTraceOld.received = tcpipTrace.received;
                    }
                    
                    _updateMonitorStatus(ref _onceRes);
                    

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
                var processId = _pid;

                using (_netTraceSession = new TraceEventSession("Perfmon_KernelAndClrEventsSession"))
                {
                    _netTraceSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);

                    _netTraceSession.Source.Kernel.TcpIpRecv += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            _netTraceDetail.received += data.size;
                        }
                    };

                    _netTraceSession.Source.Kernel.TcpIpSend += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            _netTraceDetail.send += data.size;
                        }
                    };

                    _netTraceSession.Source.Kernel.UdpIpRecv += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            _netTraceDetail.received += data.size;
                        }
                    };

                    _netTraceSession.Source.Kernel.UdpIpSend += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            _netTraceDetail.send += data.size;
                        }
                    };
                    _netTraceSession.Source.Process();
                }
            }
            catch
            {
                _netTraceDetail.send = 0;
                _netTraceDetail.received = 0;
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
