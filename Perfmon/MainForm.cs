using CsvHelper;
using PerfMonitor.Library;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using Windows.Win32;

namespace PerfMonitor
{
    public partial class MainForm : Form
    {
        private static int _phyMemTotal = 0;
        private static readonly List<RunStatusItem> _monitorResult = new();
        private readonly HistoryController _historyController;

        internal enum MonitorStatus : uint
        {
            MonitorStatusMonitoring = 0,
            MonitorStatusStopped = 1,
            MonitorStatusRemoved = 2,
        };

        internal class ProcessMonitorContext : IDisposable
        {
            public ProcessMonitor? Monitor;
            public int LiveVideIndex = 0;
            public CsvWriter? ResWriter;
            public string? ResPath;
            public Thread? VisualThread;
            public MonitorStatus MntStatus;
            public HistoryItem? history;
            public bool IsDisposed = false;

            public void Dispose ()
            {
                IsDisposed = true;
                Monitor?.Dispose();
                ResWriter?.Dispose();
                Monitor = null;
                ResWriter = null;
                MntStatus = MonitorStatus.MonitorStatusRemoved;
            }

            public void Stop ()
            {
                Monitor?.Dispose();
                ResWriter?.Dispose();
                Monitor = null;
                ResWriter = null;
                MntStatus = MonitorStatus.MonitorStatusStopped;
            }

            public bool IsStop ()
            {
                return MntStatus == MonitorStatus.MonitorStatusStopped;
            }
        }

        private readonly Dictionary<uint, ProcessMonitorContext> _monitorManager = new();

        private static readonly string[] _colHeaders_zh_hans
            = new string[] { "PID", "进程名", "CPU", "虚拟内存", "物理内存", "总内存", "上行", "下行", "流量", "运行时间", "状态", "备注" };

        private static int _markColumnIndex = 11; // "备注" as the last element to show

        private static readonly string[] _colDefaultValues = new string[] { "0", "Attaching Process", "0", "0", "0", "0", "0", "0", "0", "0 s", "0", "" };

        private readonly string[] _colHeaders = default!;
        private static readonly int[] _colSize = new int[] { 100, 140, 80, 100, 100, 100, 100, 100, 100, 120, 100, 200 };

        private static readonly Process _proc = Process.GetCurrentProcess();

        private double _sysCpu = 0;
        private string _taskList = string.Empty;

        private static string LogFolder
        {
            get
            {
                string? oPath = null;
                try
                {
                    var s = _proc.MainModule?.FileName;
                    if ( s != null )
                    {
                        oPath = Path.GetDirectoryName(s);
                    }
                }
                catch ( Exception ) { oPath = null; }

                var tPath = Path.Combine(Path.GetTempPath(), "PerfMonitor");
                if ( !Directory.Exists(tPath) )
                {
                    Directory.CreateDirectory(tPath);
                }

                oPath ??= tPath;

                var output = Path.Combine(oPath, "output");

                if ( !Directory.Exists($"{output}") )
                    Directory.CreateDirectory($"{output}");

                return output;
            }
        }

        private static string ConfigFolder
        {
            get
            {
                string? oPath = null;
                try
                {
                    var s = _proc.MainModule?.FileName;
                    if ( s != null )
                    {
                        oPath = Path.GetDirectoryName(s);
                    }
                }
                catch ( Exception ) { oPath = null; }

                var tPath = Environment.CurrentDirectory;
                oPath ??= tPath;

                var output = Path.Combine(oPath, "Config");

                if ( !Directory.Exists($"{output}") )
                    Directory.CreateDirectory($"{output}");

                return output;
            }
        }

        public MainForm ()
        {
            _colHeaders = _colHeaders_zh_hans;
            _markColumnIndex = Array.IndexOf(_colHeaders, "备注");
            InitializeComponent();
            ConstructListView();

            _phyMemTotal = GetPhisicalMemory();
            Task.Run(QuerySystemInfo);
            _ = RefreshListView();
            labelCpuAndMem.Text = "loading...";
            _taskList = Path.Combine(ConfigFolder + "\\tasks.json");
            _historyController = new(_taskList);
            _historyController.Read();
        }

        private void BtnShotProcess_MouseDown (object sender, MouseEventArgs e)
        {
            this.Opacity = 0;
        }

        private void BtnShotProcess_MouseUp (object sender, MouseEventArgs e)
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

        private void OnUpdateMonitorStatus (ref RunStatusItem status)
        {
            lock ( _monitorResult )
            {
                _monitorResult.Add(status);
            }
            if ( _monitorManager.ContainsKey(status.Pid) )
            {
                var it = _monitorManager[status.Pid];
                status.SysCpu = _sysCpu;
                try
                {
                    it.ResWriter?.WriteRecord(status);
                    it.ResWriter?.NextRecord();
                    it.ResWriter?.Flush();
                }
                catch ( Exception )
                {
                    // ignore file
                }
            }
        }

        private void ConstructListView ()
        {
            MonitorDetailLV.Columns.Clear();

            for ( int i = 0; i < _colSize.Length; i++ )
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

        private async Task RefreshListView ()
        {
            while ( !IsDisposed )
            {
                List<RunStatusItem> ress;
                lock ( _monitorResult )
                {
                    ress = new List<RunStatusItem>(_monitorResult.ToArray());
                    _monitorResult.Clear();
                }
                if ( ress.Count > 0 )
                {
                    MonitorDetailLV.BeginUpdate();
                    foreach ( RunStatusItem res in ress )
                    {
                        if ( _monitorManager.ContainsKey(res.Pid) )
                        {
                            var ctx = _monitorManager[res.Pid];
                            var index = ctx.LiveVideIndex;

                            var values = res.Info();
                            var item = MonitorDetailLV.Items[index];
                            for ( int i = 0; i < _markColumnIndex; i++ )
                            {
                                item.SubItems[i].Text = values[i];
                            }

                            if ( ctx.history != null)
                                item.SubItems[_markColumnIndex].Text = ctx.history.Marker;

                            if ( res.ExcuteStatus == "exit" )
                            {
                                item.BackColor = Color.Red;
                                var v = _monitorManager[res.Pid];
                                v.Stop();
                            }
                        }
                    }
                    MonitorDetailLV.EndUpdate();
                }
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }

        private async Task QuerySystemInfo ()
        {
            var core = Environment.ProcessorCount;
            var mnam = Environment.MachineName;
            var os = Environment.OSVersion.Version.ToString();
            using PerformanceCounter ramAva = new("Memory", "Available Bytes");
            using PerformanceCounter ramUsed = new("Memory", "Committed Bytes");

            string strQuery;
            if ( Environment.OSVersion.Version.Major >= 10 )
            {
                strQuery = "\\Processor Information(_Total)\\% Processor Utility";
            }
            else
            {
                strQuery = "\\Processor Information(_Total)\\% Processor Time";
            }

            using PerfQuery cpuTotal = new(strQuery);
            var sw = Stopwatch.StartNew();
            while ( !IsDisposed )
            {
                _proc.Refresh();
                int rama = (int)((long)Math.Round(ramAva?.NextValue() ?? 0) / Units.MB);
                int ram = (int)((long)(ramUsed?.NextValue() ?? 0) / Units.MB) + rama;
                double pVRam = _proc.VirtualMemorySize64 * 1.0 / Units.GB;
                int pPhyRam = (int)(_proc.WorkingSet64 / Units.MB);
                _sysCpu = cpuTotal.NextValue();

                var sb = $"{_sysCpu:F2}%, {ram}MB, {rama}MB | {core} C, {mnam}, {os}, {_phyMemTotal}GB | {pVRam:F2}GB, {pPhyRam}MB";

                labelCpuAndMem.Invoke(new Action(() =>
                {
                    labelCpuAndMem.Text = sb;
                })
                );

                var q = sw.ElapsedMilliseconds;
                var d = 1000 - (q % 1000);
                await Task.Delay(TimeSpan.FromMilliseconds(d));
            }

            cpuTotal?.Dispose();
        }

        private void TextBoxPID_KeyPress (object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
            {
                if ( uint.TryParse(textBoxPID.Text, out uint pi) )
                {
                    CreateNewMonitor(pi);
                }
                else
                {
                    MessageBox.Show("Bad PID, PID wrong or has been exit", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
        }

        private static int GetPhisicalMemory ()
        {
            ManagementObjectSearcher searcher = new()
            {
                Query = new SelectQuery("Win32_PhysicalMemory ", "", new[] { "Capacity" })
            };
            ManagementObjectCollection collection = searcher.Get();
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while ( em.MoveNext() )
            {
                ManagementBaseObject baseObj = em.Current;
                if ( baseObj.Properties["Capacity"].Value != null )
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
            return (int) (capacity / Units.GiB);
        }

        private void CreateNewMonitor (uint pid)
        {
            if ( !_monitorManager.ContainsKey(pid) )
            {
                Process? p = null;
                try
                {
                    p = Process.GetProcessById((int) pid);
                }
                catch { return; }

                string name = p.ProcessName;
                ProcessMonitor monitor = new(pid, 1000, OnUpdateMonitorStatus);

                string resPath = $"{LogFolder}{Path.DirectorySeparatorChar}{name}({pid}).{DateTime.Now:yyyy.MMdd.HHmm.ss}.csv";
                var writer = new StreamWriter(resPath);
                var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                ProcessMonitorContext ctx = new()
                {
                    Monitor = monitor,
                    ResWriter = csv,
                    ResPath = resPath,
                    MntStatus = MonitorStatus.MonitorStatusMonitoring,
                };
                csv.WriteHeader<RunStatusItem>();
                csv.NextRecord();

                MonitorDetailLV.BeginUpdate();
                var lvi = new ListViewItem(_colDefaultValues)
                {
                    Tag = ctx
                };
                var it = MonitorDetailLV.Items.Add(lvi);
                MonitorDetailLV.Items[it.Index].Selected = true;
                ctx.LiveVideIndex = it.Index;
                MonitorDetailLV.EndUpdate();

                _monitorManager.Add(pid, ctx);
                var his = _historyController.AddItem(pid, resPath, "No Marker...");
                ctx.history = his;
            }
        }

        private void MainForm_FormClosing (object sender, FormClosingEventArgs e)
        {
            foreach ( var it in _monitorManager )
            {
                it.Value.Dispose();
            }
            _monitorManager.Clear();

            _proc.Dispose();
        }

        private void BtnOpenFloder_Click (object sender, EventArgs e)
        {
            if ( LogFolder != null )
            {
                ProcessStartInfo psi = new()
                {
                    FileName = LogFolder,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void MonitorDetailLV_MouseDoubleClick (object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = MonitorDetailLV.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;
            if ( uint.TryParse(item.Text, out uint pid) )
            {
                if ( _monitorManager.ContainsKey(pid) )
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
                            var visual = new VisualForm(path, desc);
                            visual.ShowDialog();
                        }));
                        helpThread.SetApartmentState(ApartmentState.STA);
                        helpThread.Start();
                        it.VisualThread = helpThread;
                    }
                }
            }
        }

        private void BtnSetting_Click (object sender, EventArgs e)
        {
            //using var setting = new SettingForm();
            //setting.ShowDialog();
        }

        private void MonitorDetailLV_MouseClick (object sender, MouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Right )
            {
                var item = MonitorDetailLV.FocusedItem;
                if ( item != null && item.Bounds.Contains(e.Location) )
                {
                    ItemContextMenuStrip.Show(Cursor.Position);
                }
            }
        }

        private void OpenToolStripMenuItem_Click (object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if ( item != null && uint.TryParse(item.Text, out uint pid) && _monitorManager.ContainsKey(pid) )
            {
                var path = _monitorManager[pid].ResPath;
                if ( path != null )
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

        private void StopToolStripMenuItem_Click (object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if ( item != null && uint.TryParse(item.Text, out uint pid) && _monitorManager.ContainsKey(pid) )
            {
                var v = _monitorManager[pid];
                v.Stop();

                item.BackColor = Color.Black;
                item.ForeColor = Color.White;
            }
        }

        private void RestartCaptureToolStripMenuItem_Click (object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if ( item != null && uint.TryParse(item.Text, out uint pid) && _monitorManager.ContainsKey(pid) )
            {
                ProcessMonitorContext v = (ProcessMonitorContext)item.Tag;
                if ( v != null && v.IsStop() )
                {
                    v.Dispose();

                    _monitorManager.Remove(pid);
                    item.Tag = null;
                    MonitorDetailLV.Items.RemoveAt(item.Index);
                    for ( int i = 0; i < MonitorDetailLV.Items.Count; i++ )
                    {
                        ProcessMonitorContext v2 = (ProcessMonitorContext)MonitorDetailLV.Items[i].Tag;
                        v2.LiveVideIndex = i;
                    }
                    CreateNewMonitor(pid);
                }
            }
        }

        private void DeleteCaptureToolStripMenuItem_Click (object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if ( item != null && uint.TryParse(item.Text, out uint pid) && _monitorManager.ContainsKey(pid) )
            {
                var v = _monitorManager[pid];
                if ( v.IsStop() )
                {
                    v.Dispose();
                    _monitorManager.Remove(pid);
                    MonitorDetailLV.Items.RemoveAt(item.Index);
                    for ( int i = 0; i < MonitorDetailLV.Items.Count; i++ )
                    {
                        ProcessMonitorContext v2 = (ProcessMonitorContext)MonitorDetailLV.Items[i].Tag;
                        v2.LiveVideIndex = i;
                    }
                }
            }
        }

        private void FreshToolStripMenuItem_Click (object sender, EventArgs e)
        {
            MonitorDetailLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            MonitorDetailLV.BeginUpdate();
            for ( int i = 0; i <= MonitorDetailLV.Columns.Count - 1; i++ )
            {
                MonitorDetailLV.Columns[i].Width += 20;
            }
            MonitorDetailLV.EndUpdate();
        }

        private void MonitorDetailLV_MouseDown (object sender, MouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Middle )
            {
                var item = MonitorDetailLV.FocusedItem;
                if ( item != null && item.Bounds.Contains(e.Location)
                    && uint.TryParse(item.Text, out uint pid) && _monitorManager.ContainsKey(pid) )
                {
                    var monitor = _monitorManager[pid];
                    if ( monitor.ResPath != null )
                    {
                        ProcessStartInfo psi = new()
                        {
                            FileName = monitor.ResPath,
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                }
            }
        }

        private void MarkerToolStripMenuItem_Click (object sender, EventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if ( item != null && uint.TryParse(item.Text, out uint pid) && _monitorManager.ContainsKey(pid) )
            {
                item.BeginEdit();
            }
        }

        private void MonitorDetailLV_AfterLabelEdit (object sender, LabelEditEventArgs e)
        {
            if ( e.Label != null )
            {
                string editedText = e.Label;

                var item = MonitorDetailLV.FocusedItem;
                var ctx = (ProcessMonitorContext)item.Tag;
                if ( ctx != null && ctx.Monitor != null )
                {
                    ctx.Monitor.Mark = editedText;
                    ctx.history!.Marker = editedText;
                }
            }
        }

        private void BtnHistory_Click (object sender, EventArgs e)
        {
            using var history = new HistoryForm(_historyController);
            if ( history.ShowDialog() == DialogResult.OK )
            {
            }
        }

        private void MonitorDetailLV_KeyDown (object sender, KeyEventArgs e)
        {
            var item = MonitorDetailLV.FocusedItem;
            if ( e.KeyCode == Keys.F2 && item != null )
            {
                item.BeginEdit();
            }
            else if ( e.KeyCode == Keys.F5 )
            {
                MonitorDetailLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                MonitorDetailLV.BeginUpdate();
                for ( int i = 0; i <= MonitorDetailLV.Columns.Count - 1; i++ )
                {
                    MonitorDetailLV.Columns[i].Width += 20;
                }
                MonitorDetailLV.EndUpdate();
            }
        }
    }
}