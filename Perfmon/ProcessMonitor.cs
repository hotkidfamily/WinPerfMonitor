using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Security.AccessControl;
using TraceReloggerLib;

namespace Perfmon
{
    delegate void UpdateMonitorStatusDelegate(ref RunStatusItem status);

    internal class RunStatusItem
    {
        private uint pid = 0;
        private string procName = "undefined";
        private double cpu = 0;
        private double vMem = 0;
        private double phyMem = 0;
        private double totalMem = 0;
        private double downLink = 0;
        private double upLink = 0;
        private double totalLinkFlow = 0;
        private long excuteSeconds = 0;
        private string excuteStatus = "no exist";
        private double sysCpu = 0;

        public uint Pid { get => pid; set => pid = value; }
        public string ProcName { get => procName; set => procName = value; }
        public double Cpu { get => cpu; set => cpu = value; }
        public double VMem { get => vMem; set => vMem = value; }
        public double PhyMem { get => phyMem; set => phyMem = value; }
        public double TotalMem { get => totalMem; set => totalMem = value; }
        public double DownLink { get => downLink; set => downLink = value; }
        public double UpLink { get => upLink; set => upLink = value; }
        public double TotalLinkFlow { get => totalLinkFlow; set => totalLinkFlow = value; }
        public long ExcuteSeconds { get => excuteSeconds; set => excuteSeconds = value; }
        public string ExcuteStatus { get => excuteStatus; set => excuteStatus = value; }
        public double SysCpu { get => sysCpu; set => sysCpu = value; }

        public string[] Info()
        {
            string uposfix = " Kbps";
            string dposfix = " Kbps";
            double total = TotalLinkFlow / 1024.0f;
            double up = UpLink;
            double down = DownLink;

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
                $"{Pid}",
                ProcName,
                $"{Cpu :F2}%",
                $"{VMem/1024 :F2} GB",
                $"{PhyMem :F2} MB",
                $"{TotalMem/1024 :F2} GB",
                $"{up :F2}{uposfix}",
                $"{down :F2}{dposfix}",
                $"{total :F2} MB",
                $"{TimeSpan.FromSeconds(ExcuteSeconds)} s",
                $"{ExcuteStatus}",
                $"{sysCpu :F2}%"
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
        private readonly NetspeedTrace _netspeedDetail = new();
        private readonly NetspeedTrace _netspeedDetailOld = new();
        private readonly string _desc = "invalid process desc";

        void ProcessExitEventHandler(object? sender, EventArgs e)
        {
            _endTask = true;
            _task?.Wait();
            _onceRes.ExcuteStatus = "exit";

            _updateMonitorStatus?.Invoke(ref _onceRes);
        }

        public string Descriptor()
        {
            return _desc;
        }

        public ProcessMonitor(uint pid, int interval, UpdateMonitorStatusDelegate UpdateHandle) 
        {
            _pid = pid;
            _interval = interval;
            _onceRes.ExcuteStatus = "running";
            
            try
            {
                _process = Process.GetProcessById((int)pid);
            }
            catch (ArgumentException)
            {
                _onceRes.ExcuteStatus = "no exist";
            }

            if (_process != null)
            {
                _desc = $" {_process.ProcessName}:{_pid} ";

                _endTask = false;
                _onceRes.Pid = _pid;
                _onceRes.ProcName = _process.ProcessName;

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
                        _onceRes.VMem = _process.VirtualMemorySize64 / 1048576.0f;
                        _onceRes.PhyMem = _process.WorkingSet64 / 1048576.0f;
                        _onceRes.TotalMem = _onceRes.VMem + _onceRes.PhyMem;

                        double nowProcessorTime = _process.TotalProcessorTime.TotalMilliseconds;
                        long nowTicks = Environment.TickCount64;
                        if (lastMonitorTicks == 0)
                        {
                            lastMonitorTicks = nowTicks - 1000;
                            lastProcessorTime = nowProcessorTime;
                        }
                    
                        _onceRes.Cpu = Math.Round((nowProcessorTime - lastProcessorTime) * cores / (nowTicks - lastMonitorTicks), 2);
                        _onceRes.ExcuteSeconds = (nowTicks - firstMonitorTicks)/1000;
                        lastProcessorTime = nowProcessorTime;

                        {
                            netspeedTracer.send = _netspeedDetail.send;
                            netspeedTracer.received = _netspeedDetail.received;

                            _onceRes.UpLink = (netspeedTracer.send - _netspeedDetailOld.send) * 8 / 1024.0f;
                            _onceRes.DownLink = (netspeedTracer.received - _netspeedDetailOld.received) * 8 / 1024.0f;
                            _onceRes.TotalLinkFlow = (netspeedTracer.send + _netspeedDetailOld.received) * 8 / 1024.0f;

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
