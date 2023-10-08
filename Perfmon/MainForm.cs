using CsvHelper;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using Windows.Win32;

namespace Perfmon
{
    public partial class MainForm : Form
    {
        private PerformanceCounter cpuTotal = default!;
        private PerformanceCounter ramAva = default!;
        private PerformanceCounter ramUsed = default!;

        private static int _phyMemTotal = 0;
        private static readonly List<RunStatusItem> _monitorResult = new();

        internal class ProcessMonitorManager
        {
            public ProcessMonitor? Monitor;
            public int LiveVideIndex = 0;
            public CsvWriter? ResWriter;
            public string? ResPath;
            public Thread? VisualThread;
        }

        private readonly Dictionary<uint, ProcessMonitorManager> _monitorManager = new();

        private static readonly string[] _colHeaders_zh_hans
            = new string[] { "PID", "进程名", "CPU", "虚拟内存", "物理内存", "总内存", "上行", "下行", "流量", "运行时间", "状态" };

        private static readonly string[] _colHeaders_en = new string[] { "PID", "Name", "CPU", "Virtual Memory", "Physical Memory", "Total Memory", "Up Link", "Down Link", "Link Flow", "Time", "Status" };
        private static readonly string[] _colDefaultValues = new string[] { "0", "Attaching Process", "0", "0", "0", "0", "0", "0", "0", "0 s", "0" };

        private readonly string[] _colHeaders = default!;
        private static readonly int[] _colSize = new int[] { 50, 100, 40, 80, 100, 80, 100, 100, 80, 80, 60 };

        private static readonly Process _selfProcess = Process.GetCurrentProcess();

        private double _sysCpu = 0;

        public MainForm()
        {
            CultureInfo current = Thread.CurrentThread.CurrentUICulture;
            if (current.TwoLetterISOLanguageName != "zh")
            {
                CultureInfo newCulture = CultureInfo.CreateSpecificCulture("en-US");
                Thread.CurrentThread.CurrentUICulture = newCulture;
                Thread.CurrentThread.CurrentCulture = newCulture;
                _colHeaders = _colHeaders_en;
            }
            else
            {
                CultureInfo newCulture = CultureInfo.CreateSpecificCulture("zh-CN");
                Thread.CurrentThread.CurrentUICulture = newCulture;
                Thread.CurrentThread.CurrentCulture = newCulture;
                _colHeaders = _colHeaders_zh_hans;
            }

            InitializeComponent();
            ConstructListView();

            ConstructSystemMonitor();
            _phyMemTotal = GetPhisicalMemory();
            _ = QurySystemInfo();
            _ = RefreshListView();
        }

        private void ConstructSystemMonitor()
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                try
                {
                    cpuTotal = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
                    float usage = cpuTotal?.NextValue() ?? 0;
                }
                catch (Exception)
                {
                    cpuTotal = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                }
            }
            else
            {
                cpuTotal = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
            ramAva = new PerformanceCounter("Memory", "Available Bytes");
            ramUsed = new PerformanceCounter("Memory", "Committed Bytes");
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

        private void OnUpdateMonitorStatus(ref RunStatusItem status)
        {
            lock (_monitorResult)
            {
                _monitorResult.Add(status);
            }
            if (_monitorManager.ContainsKey(status.Pid))
            {
                var it = _monitorManager[status.Pid];
                status.SysCpu = _sysCpu;
                it.ResWriter?.WriteRecord(status);
                it.ResWriter?.NextRecord();
                it.ResWriter?.FlushAsync();
            }
        }

        private void ConstructListView()
        {
            MonitorDetailLV.Columns.Clear();

            for (int i = 0; i < _colSize.Length; i++)
            {
                ColumnHeader ch = new()
                {
                    Width = _colSize[i],
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Left,
                    Text = _colHeaders?[i],
                };
                MonitorDetailLV.Columns.Add(ch);
            }
        }

        private async Task RefreshListView()
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

                        var values = item.Info();
                        for (int i = 0; i < _colHeaders?.Length; i++)
                        {
                            MonitorDetailLV.Items[index].SubItems[i].Text = values[i];
                        }
                        if (item.ExcuteStatus == "exit")
                        {
                            MonitorDetailLV.Items[index].BackColor = Color.Red;
                            if (_monitorManager.ContainsKey(item.Pid))
                            {
                                var v = _monitorManager[item.Pid];
                                v.Monitor?.Dispose();
                                _ = v.ResWriter?.DisposeAsync();
                                v.Monitor = null;
                                v.ResWriter = null;
                            }
                        }
                    }
                }

                MonitorDetailLV.EndUpdate();

                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }

        private async Task QurySystemInfo()
        {
            var core = Environment.ProcessorCount;
            var mnam = Environment.MachineName;
            var os = Environment.OSVersion.Version.ToString();

            while (!IsDisposed)
            {
                _selfProcess.Refresh();
                int rama = (int)((long)Math.Round(ramAva.NextValue()) >> 20);
                int ram = (int)((long)ramUsed.NextValue() >> 20) + rama;
                int pVRam = (int)(_selfProcess.VirtualMemorySize64 >> 30);
                int pPhyRam = (int)(_selfProcess.PrivateMemorySize64 >> 20);
                _sysCpu = cpuTotal?.NextValue() ?? 0;

                var sb = $"{_sysCpu:F2}%, {mnam}, {os}, {core} C, {ram}MB, {rama}MB, {_phyMemTotal}GB, {pVRam}GB,{pPhyRam}MB";

                labelCpuAndMem.Text = sb;
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

        private void BtnStop_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (MonitorDetailLV.SelectedIndices.Count > 0)
            {
                index = MonitorDetailLV.SelectedIndices[0];
            }
            var item = MonitorDetailLV.Items[index];
            uint pid = uint.Parse(item.Text);
            if (_monitorManager.ContainsKey(pid))
            {
                var v = _monitorManager[pid];
                v.Monitor?.Dispose();
                _ = (v.ResWriter?.DisposeAsync());
                v.Monitor = null;
                v.ResWriter = null;

                item.BackColor = Color.Black;
                item.ForeColor = Color.White;
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (MonitorDetailLV.SelectedIndices.Count > 0)
            {
                index = MonitorDetailLV.SelectedIndices[0];
            }
            else
            {
                return;
            }
            var item = MonitorDetailLV.Items[index];
            uint pid = uint.Parse(item.Text);
            if (_monitorManager.ContainsKey(pid))
            {
                var v = _monitorManager[pid];
                if (v.Monitor == null)
                {
                    _monitorManager.Remove(pid);
                    item.BackColor = Color.White;
                    item.ForeColor = Color.Red;
                }

                //MonitorDetailLV.Items.RemoveAt(index);
            }
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (MonitorDetailLV.SelectedIndices.Count > 0)
            {
                index = MonitorDetailLV.SelectedIndices[0];
            }
            else
            {
                return;
            }
            var item = MonitorDetailLV.Items[index];
            uint pid = uint.Parse(item.Text);
            if (!_monitorManager.ContainsKey(pid))
            {
                CreateNewMonitor(pid);
            }
        }

        private void BtnBreak_Click(object sender, EventArgs e)
        {
        }

        private static int GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new()
            {
                Query = new SelectQuery("Win32_PhysicalMemory ", "", new[] { "Capacity" })
            };
            ManagementObjectCollection collection = searcher.Get();
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties["Capacity"].Value != null)
                {
                    try
                    {
                        _ = long.TryParse(baseObj.Properties["Capacity"].Value.ToString(), out long cap);
                        capacity += cap;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return (int)(capacity / (1024 * 1024 * 1000));
        }

        private void CreateNewMonitor(uint pid)
        {
            if (!_monitorManager.ContainsKey(pid))
            {
                Process p = Process.GetProcessById((int)pid);
                string name = p.ProcessName;
                ProcessMonitor monitor = new(pid, 1000, OnUpdateMonitorStatus);
                var mainPath = Path.GetDirectoryName(_selfProcess.MainModule?.FileName);
                var csvpath = Path.Combine(mainPath ?? Environment.CurrentDirectory, "output");

                if (!Directory.Exists($"{csvpath}"))
                    Directory.CreateDirectory($"{csvpath}");

                string resPath = $"{csvpath}{Path.DirectorySeparatorChar}{name}({pid}).{DateTime.Now:yyyy.MMdd.HHmm.ss}.csv";
                var writer = new StreamWriter(resPath);
                var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                ProcessMonitorManager monitorMgr = new()
                {
                    Monitor = monitor,
                    ResWriter = csv,
                    ResPath = resPath
                };
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var it in _monitorManager)
            {
                it.Value.Monitor?.Dispose();
                it.Value.ResWriter?.Dispose();
            }
            _monitorManager.Clear();
            cpuTotal.Dispose();
            ramAva.Dispose();
            ramUsed.Dispose();
        }

        private void BtnOpenFloder_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (MonitorDetailLV.SelectedIndices.Count > 0)
            {
                index = MonitorDetailLV.SelectedIndices[0];
            }
            else
            {
                return;
            }
            var item = MonitorDetailLV.Items[index];
            if (uint.TryParse(item.Text, out uint pid))
            {
                if (_monitorManager.ContainsKey(pid))
                {
                    var monitor = _monitorManager[pid];
                    var path = Path.GetDirectoryName(monitor.ResPath);
                    if (path != null)
                    {
                        ProcessStartInfo psi = new()
                        {
                            FileName = path,
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                }
            }
        }

        private void BtnAnalysis_Click(object sender, EventArgs e)
        {
        }

        private void BtnOpenResult_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (MonitorDetailLV.SelectedIndices.Count > 0)
            {
                index = MonitorDetailLV.SelectedIndices[0];
            }
            else
            {
                return;
            }
            var item = MonitorDetailLV.Items[index];
            if (uint.TryParse(item.Text, out uint pid))
            {
                if (_monitorManager.ContainsKey(pid))
                {
                    var monitor = _monitorManager[pid];
                    var path = monitor.ResPath;
                    if (path != null)
                    {
                        ProcessStartInfo psi = new()
                        {
                            FileName = path,
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                }
            }
        }

        private void MonitorDetailLV_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = MonitorDetailLV.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;
            if (uint.TryParse(item.Text, out uint pid))
            {
                if (_monitorManager.ContainsKey(pid))
                {
                    var it = _monitorManager[pid];
                    string path = it.ResPath ?? "";
                    if (
                        path != null
                        && ((it.VisualThread == null) || !it.VisualThread.IsAlive) // fix: already running form
                        && (it.LiveVideIndex == info.Item.Index) // fix: a monitor recapture after removed, then item be double cliked
                        )
                    {
                        var helpThread = new Thread(new ThreadStart(() =>
                        {
                            string desc = it.Monitor?.Descriptor() ?? "invalid";
                            using var visual = new VisualForm(path, desc);
                            visual.ShowDialog();
                        }));
                        helpThread.SetApartmentState(ApartmentState.STA);
                        helpThread.Start();
                        it.VisualThread = helpThread;
                    }
                }
            }
        }

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            using var setting = new SettingForm();
            setting.ShowDialog();
        }
    }
}