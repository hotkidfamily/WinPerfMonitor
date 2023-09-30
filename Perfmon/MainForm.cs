using System.Diagnostics;
using System.Management;
using System.Text;
using Windows.Win32;
using CsvHelper;
using System.Globalization;
using Microsoft.Diagnostics.Tracing.AutomatedAnalysis;
using CsvHelper.Configuration;

namespace Perfmon
{
    public partial class MainForm : Form
    {
        private readonly PerformanceCounter cpuTotal;
        private readonly PerformanceCounter ramAva;
        private readonly PerformanceCounter ramUsed;

        private static int _phyMemTotal = 0;
        private static readonly List<RunStatusItem> _monitorResult = new();

        internal class ProcessMonitorManager
        {
            public ProcessMonitor? Monitor;
            public int LiveVideIndex;
            public CsvWriter? ResWriter;
        }

        private readonly Dictionary<uint, ProcessMonitorManager> _monitorManager = new();
        private static readonly string[] _colHeaders = new string[] { "PID", "进程名", "CPU", "虚拟内存", "物理内存", "总内存", "上行", "下行", "流量", "运行时间", "状态" };
        private static readonly string[] _colDefaultValues = new string[] { "0", "Input/Select Target Process", "0", "0", "0", "0", "0", "0", "0", "0 s", "0" };
        private static readonly int[] _colSize = new int[] { 50, 100, 40, 80, 100, 80, 100, 100, 80, 80, 60 };

        private static readonly System.Diagnostics.Process _selfProcess = System.Diagnostics.Process.GetCurrentProcess();

        public MainForm()
        {
            InitializeComponent();
            cpuTotal = new PerformanceCounter();
            if (Environment.OSVersion.Version.Major >= 10)
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
            _phyMemTotal = GetPhisicalMemory();
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

            CreateNewMonitor(pid);

            this.Opacity = 1;
        }

        void onUpdateMonitorStatus(ref RunStatusItem status)
        {
            lock (_monitorResult)
            {
                _monitorResult.Add(status);
            }
            if (_monitorManager.ContainsKey(status.Pid))
            {
                var it = _monitorManager[status.Pid];
                it.ResWriter?.WriteRecord(status);
                it.ResWriter?.NextRecord();
            }
        }

        void ConstructListView()
        {
            MonitorDetailLV.Columns.Clear();

            for (int i = 0; i < _colSize.Length; i++)
            {
                ColumnHeader ch = new()
                {
                    Width = _colSize[i],
                    TextAlign = HorizontalAlignment.Left,
                    Text = _colHeaders[i],
                };
                MonitorDetailLV.Columns.Add(ch);
            }
        }

        async Task RefreshListView()
        {
            while (!IsDisposed)
            {
                List<RunStatusItem> ress;
                lock (_monitorResult)
                {
                    ress = new List<RunStatusItem>(_monitorResult.ToArray());
                    _monitorResult.Clear();
                }

                MonitorDetailLV.BeginUpdate();
                foreach (RunStatusItem item in ress)
                {
                    if (_monitorManager.ContainsKey(item.Pid))
                    {
                        var index = _monitorManager[item.Pid].LiveVideIndex;

                        var values = item.info();
                        for (int i = 0; i < _colHeaders.Length; i++)
                        {
                            MonitorDetailLV.Items[index].SubItems[i].Text = values[i];
                        }
                        if (item.ExcuteStatus == "exit")
                        {
                            MonitorDetailLV.Items[index].BackColor = Color.Red;
                        }
                    }
                }

                MonitorDetailLV.EndUpdate();

                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }

        async Task QurySystemInfo()
        {
            var core = Environment.ProcessorCount;
            var mnam = Environment.MachineName;
            var os = Environment.OSVersion.Version.ToString();

            while (!IsDisposed)
            {
                StringBuilder sb = new();
                int rama = (int)((long)Math.Round(ramAva.NextValue()) >> 20);
                int ram = (int)((long)ramUsed.NextValue() >> 20) + rama;

                sb.Append($"{cpuTotal?.NextValue():F2} % | {mnam} | {os} | {core} | ");
                sb.Append($"{ram} MB | {rama} MB | {_phyMemTotal} MB");

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
                    CreateNewMonitor(pid);
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
            item.BackColor = Color.Black;
            item.ForeColor = Color.White;

            uint pid = uint.Parse(item.Text);
            if (_monitorManager.ContainsKey(pid))
            {
                var v = _monitorManager[pid];
                v.Monitor?.Dispose();
                _monitorManager.Remove(pid);
                v.ResWriter?.Dispose();
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
                        long.TryParse(baseObj.Properties["Capacity"].Value.ToString(), out long cap);
                        capacity += cap;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return (int)(capacity >> 20);
        }

        private void CreateNewMonitor(uint pid)
        {
            if (!_monitorManager.ContainsKey(pid))
            {
                ProcessMonitor monitor = new(pid, 1000, onUpdateMonitorStatus);
                var mainPath = Path.GetDirectoryName(_selfProcess.MainModule?.FileName);
                var csvpath = Path.Combine(mainPath??Environment.CurrentDirectory, "output");

                if (!Directory.Exists($"{csvpath}"))
                    Directory.CreateDirectory($"{csvpath}");

                var writer = new StreamWriter($"{csvpath}{Path.DirectorySeparatorChar}{pid}.{DateTime.Now.ToString("yyyyMMdd.hhmmss")}.csv");
                var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                ProcessMonitorManager monitorMgr = new();
                monitorMgr.Monitor = monitor;
                monitorMgr.ResWriter = csv;
                csv.WriteHeader<RunStatusItem>();
                csv.NextRecord();

                MonitorDetailLV.BeginUpdate();
                var lvi = new ListViewItem(_colDefaultValues);
                var it = MonitorDetailLV.Items.Add(lvi);
                MonitorDetailLV.Items[it.Index].Selected = true;
                MonitorDetailLV.EndUpdate();

                monitorMgr.LiveVideIndex = it.Index;
                _monitorManager.Add(pid, monitorMgr);
            }
        }

        private void listViewDetail_Enter(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach(var it in _monitorManager)
            {
                it.Value.Monitor?.Dispose();
                it.Value.ResWriter?.Dispose();
            }
            _monitorManager.Clear();
        }
    }
}
