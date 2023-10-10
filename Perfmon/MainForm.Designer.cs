﻿using System.Security.Cryptography;

namespace PerfMonitor
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
            toolTip1 = new ToolTip(components);
            BtnSetting = new Button();
            BtnOpenFloder = new Button();
            btnShotProcess = new Button();
            BtnHistory = new Button();
            ItemContextMenuStrip = new ContextMenuStrip(components);
            openToolStripMenuItem = new ToolStripMenuItem();
            stopToolStripMenuItem = new ToolStripMenuItem();
            restartCaptureToolStripMenuItem = new ToolStripMenuItem();
            deleteCaptureToolStripMenuItem = new ToolStripMenuItem();
            freshToolStripMenuItem = new ToolStripMenuItem();
            flowLayoutPanel1 = new FlowLayoutPanel();
            ItemContextMenuStrip.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // labelCpuAndMem
            // 
            labelCpuAndMem.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelCpuAndMem.BorderStyle = BorderStyle.FixedSingle;
            labelCpuAndMem.Location = new Point(13, 343);
            labelCpuAndMem.Margin = new Padding(1);
            labelCpuAndMem.Name = "labelCpuAndMem";
            labelCpuAndMem.ReadOnly = true;
            labelCpuAndMem.Size = new Size(903, 23);
            labelCpuAndMem.TabIndex = 5;
            labelCpuAndMem.TextAlign = HorizontalAlignment.Right;
            toolTip1.SetToolTip(labelCpuAndMem, "本机状态");
            // 
            // MonitorDetailLV
            // 
            MonitorDetailLV.Activation = ItemActivation.OneClick;
            MonitorDetailLV.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MonitorDetailLV.BorderStyle = BorderStyle.FixedSingle;
            MonitorDetailLV.Columns.AddRange(new ColumnHeader[] { PID, procName, cpuUsage, vMem, phyMem, totalMem, downLink, upLink, totalLink, runningSeconds, monitorStatus });
            MonitorDetailLV.FullRowSelect = true;
            MonitorDetailLV.GridLines = true;
            MonitorDetailLV.HideSelection = true;
            MonitorDetailLV.Location = new Point(13, 98);
            MonitorDetailLV.MultiSelect = false;
            MonitorDetailLV.Name = "MonitorDetailLV";
            MonitorDetailLV.Size = new Size(903, 222);
            MonitorDetailLV.TabIndex = 3;
            MonitorDetailLV.UseCompatibleStateImageBehavior = false;
            MonitorDetailLV.View = View.Details;
            MonitorDetailLV.MouseClick += MonitorDetailLV_MouseClick;
            MonitorDetailLV.MouseDoubleClick += MonitorDetailLV_MouseDoubleClick;
            MonitorDetailLV.MouseDown += MonitorDetailLV_MouseDown;
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
            downLink.Text = "下行";
            downLink.Width = 100;
            // 
            // upLink
            // 
            upLink.Text = "上行";
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
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 34);
            label1.Name = "label1";
            label1.Size = new Size(96, 17);
            label1.TabIndex = 4;
            label1.Text = "输入 PID 开始：";
            // 
            // textBoxPID
            // 
            textBoxPID.Location = new Point(115, 31);
            textBoxPID.Name = "textBoxPID";
            textBoxPID.Size = new Size(100, 23);
            textBoxPID.TabIndex = 1;
            toolTip1.SetToolTip(textBoxPID, "输入PID");
            textBoxPID.KeyPress += TextBoxPID_KeyPress;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 76);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 6;
            label2.Text = "详细情况：";
            // 
            // BtnSetting
            // 
            BtnSetting.BackgroundImage = Properties.Resources.setting;
            BtnSetting.BackgroundImageLayout = ImageLayout.Zoom;
            BtnSetting.Cursor = Cursors.Hand;
            BtnSetting.FlatStyle = FlatStyle.Popup;
            BtnSetting.ImageKey = "(无)";
            BtnSetting.ImeMode = ImeMode.NoControl;
            BtnSetting.Location = new Point(44, 4);
            BtnSetting.Margin = new Padding(2);
            BtnSetting.Name = "BtnSetting";
            BtnSetting.Size = new Size(36, 36);
            BtnSetting.TabIndex = 14;
            toolTip1.SetToolTip(BtnSetting, "设置");
            BtnSetting.UseVisualStyleBackColor = true;
            BtnSetting.Click += BtnSetting_Click;
            // 
            // BtnOpenFloder
            // 
            BtnOpenFloder.BackgroundImage = Properties.Resources.folder;
            BtnOpenFloder.BackgroundImageLayout = ImageLayout.Zoom;
            BtnOpenFloder.Cursor = Cursors.Hand;
            BtnOpenFloder.FlatStyle = FlatStyle.Popup;
            BtnOpenFloder.ImageKey = "(无)";
            BtnOpenFloder.ImeMode = ImeMode.NoControl;
            BtnOpenFloder.Location = new Point(4, 4);
            BtnOpenFloder.Margin = new Padding(2);
            BtnOpenFloder.Name = "BtnOpenFloder";
            BtnOpenFloder.Size = new Size(36, 36);
            BtnOpenFloder.TabIndex = 10;
            toolTip1.SetToolTip(BtnOpenFloder, "打开数据文件夹");
            BtnOpenFloder.UseVisualStyleBackColor = true;
            BtnOpenFloder.Click += BtnOpenFloder_Click;
            // 
            // btnShotProcess
            // 
            btnShotProcess.BackgroundImage = Properties.Resources.target;
            btnShotProcess.BackgroundImageLayout = ImageLayout.Zoom;
            btnShotProcess.Cursor = Cursors.Cross;
            btnShotProcess.FlatStyle = FlatStyle.Popup;
            btnShotProcess.ImageKey = "(无)";
            btnShotProcess.ImeMode = ImeMode.NoControl;
            btnShotProcess.Location = new Point(243, 24);
            btnShotProcess.Margin = new Padding(1);
            btnShotProcess.Name = "btnShotProcess";
            btnShotProcess.Size = new Size(36, 36);
            btnShotProcess.TabIndex = 0;
            toolTip1.SetToolTip(btnShotProcess, "抓取进程");
            btnShotProcess.UseVisualStyleBackColor = true;
            btnShotProcess.MouseDown += BtnShotProcess_MouseDown;
            btnShotProcess.MouseUp += BtnShotProcess_MouseUp;
            // 
            // BtnHistory
            // 
            BtnHistory.BackgroundImage = Properties.Resources.history;
            BtnHistory.BackgroundImageLayout = ImageLayout.Zoom;
            BtnHistory.Cursor = Cursors.Hand;
            BtnHistory.FlatStyle = FlatStyle.Popup;
            BtnHistory.ImageKey = "(无)";
            BtnHistory.ImeMode = ImeMode.NoControl;
            BtnHistory.Location = new Point(102, 4);
            BtnHistory.Margin = new Padding(20, 2, 2, 2);
            BtnHistory.Name = "BtnHistory";
            BtnHistory.Size = new Size(36, 36);
            BtnHistory.TabIndex = 15;
            toolTip1.SetToolTip(BtnHistory, "打开历史");
            BtnHistory.UseVisualStyleBackColor = true;
            // 
            // ItemContextMenuStrip
            // 
            ItemContextMenuStrip.ImageScalingSize = new Size(24, 24);
            ItemContextMenuStrip.Items.AddRange(new ToolStripItem[] { openToolStripMenuItem, stopToolStripMenuItem, restartCaptureToolStripMenuItem, deleteCaptureToolStripMenuItem, freshToolStripMenuItem });
            ItemContextMenuStrip.Name = "contextMenuStrip1";
            ItemContextMenuStrip.Size = new Size(146, 154);
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Image = Properties.Resources.floppy;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(145, 30);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // stopToolStripMenuItem
            // 
            stopToolStripMenuItem.Image = Properties.Resources.stop;
            stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            stopToolStripMenuItem.Size = new Size(145, 30);
            stopToolStripMenuItem.Text = "Stop";
            stopToolStripMenuItem.Click += StopToolStripMenuItem_Click;
            // 
            // restartCaptureToolStripMenuItem
            // 
            restartCaptureToolStripMenuItem.Image = Properties.Resources.reloading;
            restartCaptureToolStripMenuItem.Name = "restartCaptureToolStripMenuItem";
            restartCaptureToolStripMenuItem.Size = new Size(145, 30);
            restartCaptureToolStripMenuItem.Text = "ReCapture";
            restartCaptureToolStripMenuItem.Click += RestartCaptureToolStripMenuItem_Click;
            // 
            // deleteCaptureToolStripMenuItem
            // 
            deleteCaptureToolStripMenuItem.Image = Properties.Resources.remove;
            deleteCaptureToolStripMenuItem.Name = "deleteCaptureToolStripMenuItem";
            deleteCaptureToolStripMenuItem.Size = new Size(145, 30);
            deleteCaptureToolStripMenuItem.Text = "Delete";
            deleteCaptureToolStripMenuItem.Click += DeleteCaptureToolStripMenuItem_Click;
            // 
            // freshToolStripMenuItem
            // 
            freshToolStripMenuItem.Image = Properties.Resources.refresh;
            freshToolStripMenuItem.Name = "freshToolStripMenuItem";
            freshToolStripMenuItem.Size = new Size(145, 30);
            freshToolStripMenuItem.Text = "Fresh";
            freshToolStripMenuItem.Click += FreshToolStripMenuItem_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(BtnOpenFloder);
            flowLayoutPanel1.Controls.Add(BtnSetting);
            flowLayoutPanel1.Controls.Add(BtnHistory);
            flowLayoutPanel1.Location = new Point(768, 24);
            flowLayoutPanel1.Margin = new Padding(1);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(2);
            flowLayoutPanel1.Size = new Size(148, 44);
            flowLayoutPanel1.TabIndex = 7;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(928, 377);
            Controls.Add(btnShotProcess);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(label2);
            Controls.Add(textBoxPID);
            Controls.Add(label1);
            Controls.Add(MonitorDetailLV);
            Controls.Add(labelCpuAndMem);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(1);
            MinimumSize = new Size(940, 406);
            Name = "MainForm";
            Text = "PerfMonitor";
            FormClosing += MainForm_FormClosing;
            ItemContextMenuStrip.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
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
        private ToolTip toolTip1;
        private ContextMenuStrip ItemContextMenuStrip;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private ToolStripMenuItem restartCaptureToolStripMenuItem;
        private ToolStripMenuItem deleteCaptureToolStripMenuItem;
        private ToolStripMenuItem freshToolStripMenuItem;
        private Button BtnSetting;
        private Button BtnOpenFloder;
        private Button btnShotProcess;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button BtnHistory;
    }
}