using System.Diagnostics;
using System.Management;
using System.Text;
using Windows.Win32;
using CsvHelper;
using System.Globalization;
using ScottPlot;
using ScottPlot.Renderable;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Printing;

namespace Perfmon
{
    public partial class MainForm : Form
    {
        private PerformanceCounter cpuTotal = default!;
        private PerformanceCounter ramAva = default!;
        private PerformanceCounter ramUsed = default!;

        private static int _phyMemTotal = 0;
        private static readonly List<RunStatusItem> _monitorResult = new();

        private readonly Size _formSize;
        private readonly Size _formMaxSize;

        internal class ProcessMonitorManager
        {
            public ProcessMonitor? Monitor;
            public int LiveVideIndex = 0;
            public CsvWriter? ResWriter;
            public string? ResPath;
        }

        private readonly Dictionary<uint, ProcessMonitorManager> _monitorManager = new();
        private static readonly string[] _colHeaders_zh_hans
            = new string[] { "PID", "进程名", "CPU", "虚拟内存", "物理内存", "总内存", "上行", "下行", "流量", "运行时间", "状态" };
        private static readonly string[] _colHeaders_en = new string[] { "PID", "Name", "CPU", "Virtual Memory", "Physical Memory", "Total Memory", "Up Link", "Down Link", "Link Flow", "Time", "Status" };
        private static readonly string[] _colDefaultValues = new string[] { "0", "Input/Select Target Process", "0", "0", "0", "0", "0", "0", "0", "0 s", "0" };

        private readonly string[] _colHeaders = default!;
        private static readonly int[] _colSize = new int[] { 50, 100, 40, 80, 100, 80, 100, 100, 80, 80, 60 };

        private static readonly Process _selfProcess = Process.GetCurrentProcess();

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
            ConstructTabControl();
            ConstructListView();

            ConstructSystemMonitor();
            _phyMemTotal = GetPhisicalMemory();
            _ = QurySystemInfo();
            _ = RefreshListView();
            _formSize = Size;
            _formMaxSize = new Size(Size.Width, 820);
        }

        private void ConstructSystemMonitor()
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                cpuTotal = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
                try { float usage = cpuTotal?.NextValue() ?? 0; }
                catch (Exception) {
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

        void OnUpdateMonitorStatus(ref RunStatusItem status)
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

        private void ConstructTabControl()
        {
            var tabCpu = new TabPage() { Text = "CPU" };
            var tabMem = new TabPage() { Text = "Memory" };
            var tabNetwork = new TabPage() { Text = "Network" };
            var tabSystem = new TabPage() { Text = "System" };

            Size tabSize = new Size(tabControlDataSheet.DisplayRectangle.Width, tabControlDataSheet.DisplayRectangle.Height);
            tabCpu.Controls.Add(new ScottPlot.FormsPlot
            {
                Name = "Cpu",
                Size = tabSize,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new System.Drawing.Point(0, 0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom
            });
            ScottPlot.FormsPlot plt = (FormsPlot)tabCpu.Controls["Cpu"];
            plt.Plot.SetAxisLimits(-20, 80, 0, 100);
            plt.Plot.Title("CPU usage");
            plt.Plot.XLabel("Time");
            plt.Plot.YLabel("%");

            tabMem.Controls.Add(new ScottPlot.FormsPlot
            {
                Name = "Mem",
                Size = tabSize,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new System.Drawing.Point(0, 0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom
            });
            ScottPlot.FormsPlot plt2 = (FormsPlot)tabMem.Controls["Mem"];
            plt2.Plot.SetAxisLimits(-20, 80, 0, 100);
            plt2.Plot.Title("Memory usage");
            plt2.Plot.XLabel("Time");
            plt2.Plot.YLabel("MB");

            tabNetwork.Controls.Add(new ScottPlot.FormsPlot
            {
                Name = "Net",
                Size = tabSize,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new System.Drawing.Point(0, 0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom
            });
            ScottPlot.FormsPlot plt3 = (FormsPlot)tabNetwork.Controls["Net"];
            plt3.Plot.SetAxisLimits(-20, 80, 0, 100);
            plt3.Plot.Title("Uplink Speed");
            plt3.Plot.XLabel("Time");
            plt3.Plot.YLabel("kbps");

            tabSystem.Controls.Add(new ScottPlot.FormsPlot
            {
                Name = "System",
                Size = tabSize,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new System.Drawing.Point(0, 0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom
            });
            ScottPlot.FormsPlot plt4 = (FormsPlot)tabSystem.Controls["System"];
            plt4.Plot.SetAxisLimits(-20, 80, 0, 100);
            plt4.Plot.Title("System CPU usage");
            plt4.Plot.XLabel("Time");
            plt4.Plot.YLabel("%");

            TabPage[] tabPages = { tabCpu, tabMem, tabNetwork, tabSystem };

            tabControlDataSheet.Size = new Size(MonitorDetailLV.Width, 400);
            tabControlDataSheet.TabPages.Clear();
            tabControlDataSheet.TabPages.AddRange(tabPages);
            tabControlDataSheet.SelectedIndex = 3;
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

                        var values = item.Info();
                        for (int i = 0; i < _colHeaders?.Length; i++)
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
                int pVRam = (int)(_selfProcess.VirtualMemorySize64 >> 30);
                int pPhyRam = (int)(_selfProcess.WorkingSet64 >> 20);
                float usage = cpuTotal?.NextValue() ?? 0;

                sb.Append($"{usage :F2}%, {mnam}, {os}, {core} C, ");
                sb.Append($"{ram}MB, {rama}MB, {_phyMemTotal}GB, {pVRam}GB,{pPhyRam}MB");

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
                v.ResWriter?.Dispose();
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

        private void BtnVisual_Click(object sender, EventArgs e)
        {
            if (Size == _formMaxSize)
            {
                Size = _formSize;
                tabControlDataSheet.Hide();
            }
            else
            {
                Size = _formMaxSize;
                tabControlDataSheet.Show();
            }
        }
    }
}
