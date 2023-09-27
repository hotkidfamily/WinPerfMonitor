using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Windows.Win32;
using System.Management;


namespace Perfmon
{
    public partial class MainForm : Form
    {
        private readonly PerformanceCounter? cpuTotal;
        private readonly PerformanceCounter? ramAva;
        private readonly PerformanceCounter? ramUsed;

        class RunStatusItem
        {
            public uint pid = 0;
            public string procName = "undefined";
            public float cpu = 0;
            public float vMem = 0;
            public float phyMem = 0;
            public float totalMem = 0;
            public float downLink = 0;
            public float upLink = 0;
            public float totoalLinkFlow = 0;
            public uint excuteStatus = 0;
        }

        private readonly RunStatusItem _mproc = new();
        private static int _phyMemTotal = 0;

        
        public MainForm()
        {
            InitializeComponent();
            cpuTotal = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramAva = new PerformanceCounter("Memory", "Available Bytes");
            ramUsed = new PerformanceCounter("Memory", "Committed Bytes");
            _ = MachineUpdate();
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
            var s = System.Diagnostics.Process.GetProcessById((int)pid);

            _mproc.pid = pid;
            _mproc.procName = s.ProcessName;
            UpdateListView();

            this.Opacity = 1;
        }

        void UpdateListView()
        {
            listViewDetail.Columns.Clear();

            string[] v = new string[] { "进程ID", "进程名", "CPU使用率", "虚拟内存", "物理内存", "总内存", "下行（KB/s）", "上行（KB/s）", "流量", "状态" };
            int[] colsize = new int[] { 60, 80, 60, 100, 100, 100, 120, 120, 120, 60 };

            for (int i = 0; i < v.Length; i++)
            {
                ColumnHeader ch = new()
                {
                    Width = colsize[i],
                    TextAlign = HorizontalAlignment.Left,
                    Text = v[i]
                };
                listViewDetail.Columns.Add(ch);
            }

            listViewDetail.BeginUpdate();
            for (int i = 0; i < 10; i++)
            {
                var lvi = new ListViewItem(new string[] {
                    _mproc.pid.ToString(),
                    _mproc.procName, _mproc.vMem.ToString(),_mproc.phyMem.ToString(),_mproc.totalMem.ToString()});

                listViewDetail.Items.Add(lvi);
            }
            listViewDetail.EndUpdate();

            listViewDetail.Items[0].Selected = true;
        }

        async Task MachineUpdate()
        {
            _phyMemTotal = GetPhisicalMemory();
            var core = Environment.ProcessorCount;
            var mnam = Environment.MachineName;
            var os = Environment.OSVersion.Version.ToString();
            Process curProcess = Process.GetCurrentProcess();

            while (!IsDisposed)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                StringBuilder sb = new();
                int rama = (int)((long)Math.Round(ramAva.NextValue()) >> 20);
                int ram = (int)((long)ramUsed.NextValue() >> 20) + rama;

                sb.Append($"{cpuTotal?.NextValue():F2} % | {mnam} | {os} | {core} | ");
                sb.Append($"{ram} MB | {rama} MB | {_phyMemTotal} MB | {curProcess.Id},{curProcess.ProcessName}");

                labelCpuAndMem.Text = sb.ToString();
            }
        }

        private void TextBoxPID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (uint.TryParse(textBoxPID.Text.ToString(), out uint pi))
                {
                    _mproc.pid = pi;
                    var s = System.Diagnostics.Process.GetProcessById((int)pi);
                    _mproc.procName = s.ProcessName;
                    UpdateListView();
                }
                else
                {
                    MessageBox.Show("Bad PID, PID wrong or has been exit", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

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

    }
}
