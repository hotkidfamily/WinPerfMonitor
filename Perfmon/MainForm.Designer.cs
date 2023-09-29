using System.Security.Cryptography;

namespace Perfmon
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnShotProcess = new Button();
            labelCpuAndMem = new TextBox();
            MonitorDetailLV = new ListView();
            PID = new ColumnHeader();
            procName = new ColumnHeader();
            cpuUsage = new ColumnHeader();
            vMem = new ColumnHeader();
            phyMem = new ColumnHeader();
            totalMem = new ColumnHeader();
            downLink = new ColumnHeader();
            upLink = new ColumnHeader();
            totalLink = new ColumnHeader();
            runningSeconds = new ColumnHeader();
            monitorStatus = new ColumnHeader();
            label1 = new Label();
            textBoxPID = new TextBox();
            label2 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnStop = new Button();
            btnDisable = new Button();
            btnRestart = new Button();
            btnBreak = new Button();
            flowLayoutPanel2 = new FlowLayoutPanel();
            toolTip1 = new ToolTip(components);
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // btnShotProcess
            // 
            btnShotProcess.BackgroundImage = Properties.Resources.shot_icon;
            btnShotProcess.BackgroundImageLayout = ImageLayout.Zoom;
            btnShotProcess.Cursor = Cursors.Cross;
            btnShotProcess.FlatStyle = FlatStyle.Popup;
            btnShotProcess.ImageKey = "(无)";
            btnShotProcess.Location = new Point(256, 13);
            btnShotProcess.Margin = new Padding(2);
            btnShotProcess.Name = "btnShotProcess";
            btnShotProcess.Size = new Size(41, 45);
            btnShotProcess.TabIndex = 0;
            toolTip1.SetToolTip(btnShotProcess, "Shot");
            btnShotProcess.UseVisualStyleBackColor = true;
            btnShotProcess.MouseDown += BtnShotProcess_MouseDown;
            btnShotProcess.MouseUp += BtnShotProcess_MouseUp;
            // 
            // labelCpuAndMem
            // 
            labelCpuAndMem.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelCpuAndMem.BorderStyle = BorderStyle.FixedSingle;
            labelCpuAndMem.Location = new Point(13, 343);
            labelCpuAndMem.Margin = new Padding(2);
            labelCpuAndMem.Name = "labelCpuAndMem";
            labelCpuAndMem.ReadOnly = true;
            labelCpuAndMem.Size = new Size(903, 23);
            labelCpuAndMem.TabIndex = 1;
            labelCpuAndMem.TextAlign = HorizontalAlignment.Right;
            toolTip1.SetToolTip(labelCpuAndMem, "LocalMachineStatus");
            // 
            // MonitorDetailLV
            // 
            MonitorDetailLV.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MonitorDetailLV.BorderStyle = BorderStyle.FixedSingle;
            MonitorDetailLV.Columns.AddRange(new ColumnHeader[] { PID, procName, cpuUsage, vMem, phyMem, totalMem, downLink, upLink, totalLink, runningSeconds, monitorStatus });
            MonitorDetailLV.FullRowSelect = true;
            MonitorDetailLV.GridLines = true;
            MonitorDetailLV.Location = new Point(13, 98);
            MonitorDetailLV.MultiSelect = false;
            MonitorDetailLV.Name = "MonitorDetailLV";
            MonitorDetailLV.Size = new Size(903, 222);
            MonitorDetailLV.TabIndex = 3;
            MonitorDetailLV.UseCompatibleStateImageBehavior = false;
            MonitorDetailLV.View = View.Details;
            MonitorDetailLV.Enter += listViewDetail_Enter;
            // 
            // PID
            //
            PID.Text = "进程ID";
            PID.Width = 50;
            // 
            // procName
            // 
            procName.Text = "进程名";
            procName.Width = 100;
            // 
            // cpuUsage
            // 
            cpuUsage.Text = "CPU使用率";
            cpuUsage.Width = 40;
            // 
            // vMem
            // 
            vMem.Text = "虚拟内存";
            vMem.Width = 80;
            // 
            // phyMem
            // 
            phyMem.Text = "物理内存";
            phyMem.Width = 100;
            // 
            // totalMem
            // 
            totalMem.Text = "进程总内存";
            totalMem.Width = 80;
            // 
            // downLink
            // 
            downLink.Text = "下行（KB/s）";
            downLink.Width = 100;
            // 
            // upLink
            // 
            upLink.Text = "上行（KB/s）";
            upLink.Width = 100;
            // 
            // totalLink
            // 
            totalLink.Text = "总流量";
            totalLink.Width = 80;
            // 
            // runningSeconds
            // 
            runningSeconds.Text = "运行时间";
            runningSeconds.Width = 80;
            // 
            // monitorStatus
            // 
            monitorStatus.Text = "状态";
            monitorStatus.Width = 60;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 27);
            label1.Name = "label1";
            label1.Size = new Size(96, 17);
            label1.TabIndex = 4;
            label1.Text = "输入 PID 开始：";
            // 
            // textBoxPID
            // 
            textBoxPID.Location = new Point(115, 25);
            textBoxPID.Name = "textBoxPID";
            textBoxPID.Size = new Size(100, 23);
            textBoxPID.TabIndex = 5;
            toolTip1.SetToolTip(textBoxPID, "输入PID");
            textBoxPID.KeyPress += TextBoxPID_KeyPress;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 78);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 6;
            label2.Text = "详细情况：";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnStop);
            flowLayoutPanel1.Controls.Add(btnDisable);
            flowLayoutPanel1.Controls.Add(btnRestart);
            flowLayoutPanel1.Location = new Point(323, 8);
            flowLayoutPanel1.Margin = new Padding(2);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(2);
            flowLayoutPanel1.Size = new Size(141, 54);
            flowLayoutPanel1.TabIndex = 7;
            // 
            // btnStop
            // 
            btnStop.BackgroundImage = Properties.Resources.stop_icon;
            btnStop.BackgroundImageLayout = ImageLayout.Zoom;
            btnStop.Cursor = Cursors.Hand;
            btnStop.FlatStyle = FlatStyle.Popup;
            btnStop.ImageKey = "(无)";
            btnStop.Location = new Point(4, 4);
            btnStop.Margin = new Padding(2);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(41, 45);
            btnStop.TabIndex = 8;
            toolTip1.SetToolTip(btnStop, "Stop");
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnDisable
            // 
            btnDisable.BackgroundImage = Properties.Resources.disable_icon;
            btnDisable.BackgroundImageLayout = ImageLayout.Zoom;
            btnDisable.Cursor = Cursors.Hand;
            btnDisable.FlatStyle = FlatStyle.Popup;
            btnDisable.ImageKey = "(无)";
            btnDisable.Location = new Point(49, 4);
            btnDisable.Margin = new Padding(2);
            btnDisable.Name = "btnDisable";
            btnDisable.Size = new Size(41, 45);
            btnDisable.TabIndex = 10;
            toolTip1.SetToolTip(btnDisable, "Remove");
            btnDisable.UseVisualStyleBackColor = true;
            btnDisable.Click += btnDisable_Click;
            // 
            // btnRestart
            // 
            btnRestart.BackgroundImage = Properties.Resources.restart_icon;
            btnRestart.BackgroundImageLayout = ImageLayout.Zoom;
            btnRestart.Cursor = Cursors.Hand;
            btnRestart.FlatStyle = FlatStyle.Popup;
            btnRestart.ImageKey = "(无)";
            btnRestart.Location = new Point(94, 4);
            btnRestart.Margin = new Padding(2);
            btnRestart.Name = "btnRestart";
            btnRestart.Size = new Size(41, 45);
            btnRestart.TabIndex = 11;
            toolTip1.SetToolTip(btnRestart, "Restart");
            btnRestart.UseVisualStyleBackColor = true;
            btnRestart.Click += btnRestart_Click;
            // 
            // btnBreak
            // 
            btnBreak.BackgroundImage = Properties.Resources.break_icon;
            btnBreak.BackgroundImageLayout = ImageLayout.Zoom;
            btnBreak.Cursor = Cursors.Hand;
            btnBreak.FlatStyle = FlatStyle.Popup;
            btnBreak.ImageKey = "(无)";
            btnBreak.Location = new Point(4, 4);
            btnBreak.Margin = new Padding(2);
            btnBreak.Name = "btnBreak";
            btnBreak.Size = new Size(41, 45);
            btnBreak.TabIndex = 9;
            toolTip1.SetToolTip(btnBreak, "暂停");
            btnBreak.UseVisualStyleBackColor = true;
            btnBreak.Click += btnBreak_Click;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(btnBreak);
            flowLayoutPanel2.Location = new Point(524, 8);
            flowLayoutPanel2.Margin = new Padding(2);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Padding = new Padding(2);
            flowLayoutPanel2.Size = new Size(115, 54);
            flowLayoutPanel2.TabIndex = 8;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(928, 377);
            Controls.Add(btnShotProcess);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(label2);
            Controls.Add(textBoxPID);
            Controls.Add(label1);
            Controls.Add(MonitorDetailLV);
            Controls.Add(labelCpuAndMem);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MinimumSize = new Size(944, 416);
            Name = "MainForm";
            Text = "PerfMonitor";
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnShotProcess;
        private TextBox labelCpuAndMem;
        private ListView MonitorDetailLV;
        private Label label1;
        private TextBox textBoxPID;
        private Label label2;
        private ColumnHeader procName;
        private ColumnHeader cpuUsage;
        private ColumnHeader vMem;
        private ColumnHeader phyMem;
        private ColumnHeader totalMem;
        private ColumnHeader downLink;
        private ColumnHeader upLink;
        private ColumnHeader totalLink;
        private ColumnHeader PID;
        private ColumnHeader runningSeconds;
        private ColumnHeader monitorStatus;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnStop;
        private Button btnBreak;
        private Button btnDisable;
        private Button btnRestart;
        private FlowLayoutPanel flowLayoutPanel2;
        private ToolTip toolTip1;
    }
}