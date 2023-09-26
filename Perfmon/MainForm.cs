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



namespace Perfmon
{
    public partial class MainForm : Form
    {
        private PerformanceCounter? cpuProc;
        private PerformanceCounter? cpuTotal;
        private PerformanceCounter? ramAva;
        private PerformanceCounter? ramUsed;

        private uint pid = 0;


        public MainForm()
        {
            InitializeComponent();
            cpuProc = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuTotal = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramAva = new PerformanceCounter("Memory", "Available Bytes");
            ramUsed = new PerformanceCounter("Memory", "Committed Bytes");
            update();
        }

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        private void btnShotProcess_Click(object sender, EventArgs e)
        {

        }

        private void btnShotProcess_MouseDown(object sender, MouseEventArgs e)
        {
            this.Opacity = 0;
        }

        private void btnShotProcess_MouseUp(object sender, MouseEventArgs e)
        {
            this.Opacity = 1;

            Point v;
            GetCursorPos(out v);
            var Handle = WindowFromPoint(v);
            GetWindowThreadProcessId(Handle, out pid);

        }

        async Task update()
        {
            while (!IsDisposed)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                var v1 = $"{cpuTotal.NextValue():F2}" + "% | ";
                var v2 = $"{(ramUsed.NextValue() + ramAva.NextValue()) / 1000 / 1000:F2}" + "MB | ";
                var v3 = $"{ramAva.NextValue() / 1000 / 1000:F2}" + "MB | " + $"{pid}";
                labelCpuAndMem.Text = v1 + v2 + v3;
            }
        }
    }
}
