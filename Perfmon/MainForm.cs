using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Windows.Win32;
using static Perfmon.RunStatusItem;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Perfmon
{
    public partial class MainForm : Form
    {
        private readonly PerformanceCounter? cpuTotal;
        private readonly PerformanceCounter? ramAva;
        private readonly PerformanceCounter? ramUsed;

        private static int _phyMemTotal = 0;
        private static List<RunStatusItem> _monitor = new();

        private Dictionary<uint, ProcessMonitor> _monitorTasks = new();
        private Dictionary<uint, int> _linePidMap = new();
        private int _pidsCount = 0;

        public MainForm()
        {
            InitializeComponent();
            cpuTotal = new PerformanceCounter();
            if(Environment.OSVersion.Version.Major >= 10)
            {
                cpuTotal.CategoryName = "Processor Information";
                cpuTotal.CounterName = "% Processor Utility";
            }
            else
            {
                cpuTotal.CategoryName = "Processor";
                cpuTotal.CounterName = "% Processor Time";
            }
            
            cpuTotal.InstanceName = "_Total";

            ramAva = new PerformanceCounter("Memory", "Available Bytes");
            ramUsed = new PerformanceCounter("Memory", "Committed Bytes");
            ConstructListView();
            _ = QurySystemInfo();
            _ = RefreshListView();
        }

        private void BtnShotProcess_MouseDown(object sender, MouseEventArgs e)
        {
            this.Opacity = 0;
        }

        private void BtnShotProcess_MouseUp(object sender, MouseEventArgs e)
        {
            PInvoke.GetCursorPos(out Point v);
            var Handle = PInvoke.WindowFromPoint(v);
            uint pid = 0;
            unsafe
            {
                _ = PInvoke.GetWindowThreadProcessId(Handle, &pid);
            }
            uint pid2 = pid;

            if (!_monitorTasks.ContainsKey(pid2))
            {
                ProcessMonitor monitor = new(pid2, 1000, onUpdateMonitorStatus);
                _monitorTasks.Add(pid2, monitor);
                int index = _pidsCount++;
                _linePidMap[pid2] = index;

                MonitorDetailLV.BeginUpdate();
                var lvi = new ListViewItem(new string[] {
                "0", "Input/Select Target Process", "0", "0", "0", "0", "0", "0", "0", "0"});

                MonitorDetailLV.Items.Insert(index, lvi);
                MonitorDetailLV.Items[index].Selected = true;
                MonitorDetailLV.EndUpdate();
            }

            this.Opacity = 1;
        }

        void onUpdateMonitorStatus(ref RunStatusItem status)
        {
            lock (_monitor)
            {
                _monitor.Add(status);
            }
        }

        void ConstructListView()
        {
            MonitorDetailLV.Columns.Clear();

            string[] v = new string[] { "进程ID", "进程名", "CPU使用率", "虚拟内存", "物理内存", "总内存", "上行", "下行", "流量", "状态" };
            int[] colsize = new int[] { 60, 80, 60, 100, 100, 100, 120, 120, 120, 60 };

            for (int i = 0; i < v.Length; i++)
            {
                ColumnHeader ch = new()
                {
                    Width = colsize[i],
                    TextAlign = HorizontalAlignment.Left,
                    Text = v[i]
                };
                MonitorDetailLV.Columns.Add(ch);
            }
        }

        async Task RefreshListView()
        {
            while (!IsDisposed)
            {
                List<RunStatusItem> ress;
                lock (_monitor)
                {
                    ress = new List<RunStatusItem>(_monitor.ToArray());
                    _monitor.Clear();
                }

                MonitorDetailLV.BeginUpdate();
                foreach (RunStatusItem item in ress)
                {
                    var lvi = new ListViewItem(item.info());
                    if(_linePidMap.ContainsKey(item.pid))
                    {
                        var index = _linePidMap[item.pid];
                        MonitorDetailLV.Items[index] = lvi;
                    }
                }
               
                MonitorDetailLV.EndUpdate();

                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }

        async Task QurySystemInfo()
        {
            _phyMemTotal = GetPhisicalMemory();
            var core = Environment.ProcessorCount;
            var mnam = Environment.MachineName;
            var os = Environment.OSVersion.Version.ToString();
            Process curProcess = Process.GetCurrentProcess();

            while (!IsDisposed)
            {
                StringBuilder sb = new();
                int rama = (int)((long)Math.Round(ramAva.NextValue()) >> 20);
                int ram = (int)((long)ramUsed.NextValue() >> 20) + rama;

                sb.Append($"{cpuTotal?.NextValue() :F2} % | {mnam} | {os} | {core} | ");
                sb.Append($"{ram} MB | {rama} MB | {_phyMemTotal} MB | {curProcess.Id},{curProcess.ProcessName}");

                labelCpuAndMem.Text = sb.ToString();
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }

        private void TextBoxPID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (uint.TryParse(textBoxPID.Text.ToString(), out uint pi))
                {
                    uint pid = pi;
                    if (!_monitorTasks.ContainsKey(pid))
                    {
                        ProcessMonitor monitor = new(pid, 1000, onUpdateMonitorStatus);
                        _monitorTasks.Add(pid, monitor);
                        int index = _pidsCount++;
                        _linePidMap[pid] = index;

                        MonitorDetailLV.BeginUpdate();
                        var lvi = new ListViewItem(new string[] {
                "0", "Input/Select Target Process", "0", "0", "0", "0", "0", "0", "0", "0"});

                        MonitorDetailLV.Items.Insert(index, lvi);
                        MonitorDetailLV.Items[index].Selected = true;
                        MonitorDetailLV.EndUpdate();
                    }
                }
                else
                {
                    MessageBox.Show("Bad PID, PID wrong or has been exit", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (MonitorDetailLV.SelectedIndices.Count > 0)
            {
                index = MonitorDetailLV.SelectedIndices[0];
            }
            var item = MonitorDetailLV.Items[index];
            uint pid = uint.Parse(item.Text);
            if (_monitorTasks.ContainsKey(pid))
            {
                var v = _monitorTasks[pid];
                v.Dispose();
                _monitorTasks.Remove(pid);
            }
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {

        }

        private void btnRestart_Click(object sender, EventArgs e)
        {

        }

        private void btnBreak_Click(object sender, EventArgs e)
        {

        }

        private static int GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(); //用于查询一些如系统信息的管理对象 
            searcher.Query = new SelectQuery("Win32_PhysicalMemory ", "", new[] { "Capacity" }); //设置查询条件 
            ManagementObjectCollection collection = searcher.Get(); //获取内存容量 
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties["Capacity"].Value != null)
                {
                    try
                    {
                        capacity += long.Parse(baseObj.Properties["Capacity"].Value.ToString());
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            return (int)(capacity >> 20);
        }

        private void listViewDetail_Enter(object sender, EventArgs e)
        {

        }
    }
}
