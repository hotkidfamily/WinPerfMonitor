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

namespace Perfmon
{
    public partial class MainForm : Form
    {
        private readonly PerformanceCounter? cpuProc;
        private readonly PerformanceCounter? cpuTotal;
        private readonly PerformanceCounter? ramAva;
        private readonly PerformanceCounter? ramUsed;

        class ProcListItem
        {
            public uint pid;
            public string? procName;
            public float cpu;
            public float vMem;
            public float phyMem;
            public float gpu;
        }

        private readonly ProcListItem _mproc = new();
        public MainForm()
        {
            InitializeComponent();
            cpuProc = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuTotal = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramAva = new PerformanceCounter("Memory", "Available Bytes");
            ramUsed = new PerformanceCounter("Memory", "Committed Bytes");
            _ = MachineUpdate();
        }

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out Point lpPoint);

        private void BtnShotProcess_MouseDown(object sender, MouseEventArgs e)
        {
            this.Opacity = 0;
        }

        private void BtnShotProcess_MouseUp(object sender, MouseEventArgs e)
        {
            GetCursorPos(out Point v);
            var Handle = WindowFromPoint(v);
            GetWindowThreadProcessId(Handle, out uint pid);
            var s = System.Diagnostics.Process.GetProcessById((int)pid);

            _mproc.pid = pid;
            _mproc.procName = s.ProcessName;
            UpdateListView();

            this.Opacity = 1;
        }

        void UpdateListView()
        {
            listViewDetail.Columns.Clear();

            string[] v = new string[5] { "进程名", "PID", "vMem", "phyMem", "GPU" };
            for (int i = 0; i < 5; i++)
            {
                ColumnHeader ch = new()
                {
                    Width = 120,
                    TextAlign = HorizontalAlignment.Left,
                    Text = v[i]
                };
                listViewDetail.Columns.Add(ch);
            }

            listViewDetail.BeginUpdate();
            for (int i = 0; i < 10; i++)
            {
                ListViewItem lvi = new()
                {
                    Text = _mproc.procName
                };
                lvi.SubItems.Add(_mproc.pid.ToString());
                lvi.SubItems.Add(_mproc.vMem.ToString());
                lvi.SubItems.Add(_mproc.phyMem.ToString());
                lvi.SubItems.Add(_mproc.gpu.ToString());
                listViewDetail.Items.Add(lvi);
            }
            listViewDetail.EndUpdate();

            listViewDetail.Items[0].Selected = true;
        }

        async Task MachineUpdate()
        {
            while (!IsDisposed)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                var v1 = $"{cpuTotal.NextValue():F2}" + "% | ";
                var v2 = $"{(ramUsed.NextValue() + ramAva.NextValue()) / 1000 / 1000:F2}" + "MB | ";
                var v3 = $"{ramAva.NextValue() / 1000 / 1000:F2}" + "MB | " + $"{_mproc.pid},{_mproc.procName}";
                labelCpuAndMem.Text = v1 + v2 + v3;
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
    }
}
