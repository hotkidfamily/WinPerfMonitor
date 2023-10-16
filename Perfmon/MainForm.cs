using CsvHelper;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Windows.Forms;
using Windows.Win32;

namespace PerfMonitor
{
    public partial class MainForm : Form
    {
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
        private static readonly int[] _colSize = new int[] { 100, 140, 80, 100, 100, 100, 100, 100, 100, 120, 100 };

        private static readonly Process _selfProcess = Process.GetCurrentProcess();

        private double _sysCpu = 0;

        private static string LogFolder
        {
            get
            {
                string? oPath = null;
                try
                {
                    var s = _selfProcess.MainModule?.FileName;
                    if (s != null)
                    {
                        oPath = Path.GetDirectoryName(s);
                    }
                }
                catch (Exception) { oPath = null; }

                var tPath = Path.Combine(Path.GetTempPath(), "PerfMonitor");
                if (!Directory.Exists(tPath))
                {
                    Directory.CreateDirectory(tPath);
                }

                oPath ??= tPath;

                var output = Path.Combine(oPath, "output");

                if (!Directory.Exists($"{output}"))
                    Directory.CreateDirectory($"{output}");

                return output;
            }
        }

        public MainForm()
        {
            _colHeaders = _colHeaders_zh_hans;

            InitializeComponent();
            ConstructListView();

            _phyMemTotal = GetPhisicalMemory();
            _ = QuerySystemInfo();
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
                it.ResWriter?.Flush();
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
                    TextAlign = HorizontalAlignment.Left,
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
                if (ress.Count > 0)
                {
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
                                    v.ResWriter?.Dispose();
                                    v.Monitor = null;
                                    v.ResWriter = null;
                                }
                            }
                        }
                    }

                    MonitorDetailLV.EndUpdate();
                }
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }

        private async Task QuerySystemInfo()
        {
            var core = Environment.ProcessorCount;
            var mnam = Environment.MachineName;
            var os = Environment.OSVersion.Version.ToString();
            using PerformanceCounter ramAva = new("Memory", "Available Bytes");
            using PerformanceCounter ramUsed = new("Memory", "Committed Bytes");

            PerformanceCounter? cpuTotal = null;
            try
            {
                cpuTotal = new("Processor Information", "% Processor Utility", "_Total");
                _ = cpuTotal?.NextValue();
            }
            catch (Exception) { cpuTotal?.Dispose(); cpuTotal = null; }
            cpuTotal ??= new PerformanceCounter("Processor", "% Processor Time", "_Total");

            var sw = Stopwatch.StartNew();
            while (!IsDisposed)
            {
                _selfProcess.Refresh();
                int rama = (int)((long)Math.Round(ramAva?.NextValue() ?? 0) >> 20);
                int ram = (int)((long)(ramUsed?.NextValue() ?? 0) >> 20) + rama;
                int pVRam = (int)(_selfProcess.VirtualMemorySize64 >> 30);
                int pPhyRam = (int)(_selfProcess.WorkingSet64 >> 20);
                _sysCpu = cpuTotal?.NextValue() ?? 0;

                var sb = $"{_sysCpu:F2}%, {mnam}, {os}, {core} C, {ram}MB, {rama}MB, {_phyMemTotal}GB, {pVRam}GB,{pPhyRam}MB";

                labelCpuAndMem.Text = sb;

                var q = sw.ElapsedMilliseconds;
                var d = 1000 - (q % 1000);
                await Task.Delay(TimeSpan.FromMilliseconds(d));
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
                Process? p = null;
                try
                {
                    p = Process.GetProcessById((int)pid);
                }
                catch { return; }

                string name = p.ProcessName;
                ProcessMonitor monitor = new(pid, 1000, OnUpdateMonitorStatus);

                string resPath = $"{LogFolder}{Path.DirectorySeparatorChar}{name}({pid}).{DateTime.Now:yyyy.MMdd.HHmm.ss}.csv";
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
        }

        private void BtnOpenFloder_Click(object sender, EventArgs e)
        {
            if (LogFolder != null)
            {
                ProcessStartInfo psi = new()
                {
                    FileName = LogFolder,
                    UseShellExecute = true
                };
                Process.Start(psi);
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

        private void MonitorDetailLV_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var item = MonitorDetailLV.FocusedItem;
                if (item != null && item.Bounds.Contains(e.Location))
                {
                    ItemContextMenuStrip.Show(Cursor.Position);
                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if (item != null)
            {
                uint pid = uint.Parse(item.Text);
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

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if (item != null)
            {
                uint pid = uint.Parse(item.Text);
                if (_monitorManager.ContainsKey(pid))
                {
                    var v = _monitorManager[pid];
                    v.Monitor?.Dispose();
                    v.ResWriter?.Dispose();
                    v.Monitor = null;
                    v.ResWriter = null;

                    item.BackColor = Color.Black;
                    item.ForeColor = Color.White;
                }
            }
        }

        private void RestartCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if (item != null)
            {
                uint pid = uint.Parse(item.Text);
                if (!_monitorManager.ContainsKey(pid))
                {
                    CreateNewMonitor(pid);
                }
            }
        }

        private void DeleteCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if (item != null)
            {
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
                }
                //MonitorDetailLV.Items.RemoveAt(index);
            }
        }

        private void FreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MonitorDetailLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            MonitorDetailLV.BeginUpdate();
            for (int i = 0; i <= MonitorDetailLV.Columns.Count - 1; i++)
            {
                MonitorDetailLV.Columns[i].Width += 10;
            }
            MonitorDetailLV.EndUpdate();
        }

        private void MonitorDetailLV_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                var item = MonitorDetailLV.FocusedItem;
                if (item != null && item.Bounds.Contains(e.Location))
                {
                    uint pid = uint.Parse(item.Text);
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
        }
    }
}