using System.Security.Cryptography;

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
            btnRestart = new Button();
            BtnRemove = new Button();
            btnBreak = new Button();
            BtnOpenFloder = new Button();
            BtnOpenResult = new Button();
            BtnAnalysis = new Button();
            BtnSetting = new Button();
            toolTip1 = new ToolTip(components);
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnShotProcess
            // 
            resources.ApplyResources(btnShotProcess, "btnShotProcess");
            btnShotProcess.BackgroundImage = Properties.Resources.target;
            btnShotProcess.Cursor = Cursors.Cross;
            btnShotProcess.Name = "btnShotProcess";
            toolTip1.SetToolTip(btnShotProcess, resources.GetString("btnShotProcess.ToolTip"));
            btnShotProcess.UseVisualStyleBackColor = true;
            btnShotProcess.MouseDown += BtnShotProcess_MouseDown;
            btnShotProcess.MouseUp += BtnShotProcess_MouseUp;
            // 
            // labelCpuAndMem
            // 
            resources.ApplyResources(labelCpuAndMem, "labelCpuAndMem");
            labelCpuAndMem.BorderStyle = BorderStyle.FixedSingle;
            labelCpuAndMem.Name = "labelCpuAndMem";
            labelCpuAndMem.ReadOnly = true;
            toolTip1.SetToolTip(labelCpuAndMem, resources.GetString("labelCpuAndMem.ToolTip"));
            // 
            // MonitorDetailLV
            // 
            resources.ApplyResources(MonitorDetailLV, "MonitorDetailLV");
            MonitorDetailLV.Activation = ItemActivation.OneClick;
            MonitorDetailLV.BorderStyle = BorderStyle.FixedSingle;
            MonitorDetailLV.Columns.AddRange(new ColumnHeader[] { PID, procName, cpuUsage, vMem, phyMem, totalMem, downLink, upLink, totalLink, runningSeconds, monitorStatus });
            MonitorDetailLV.FullRowSelect = true;
            MonitorDetailLV.GridLines = true;
            MonitorDetailLV.HideSelection = true;
            MonitorDetailLV.MultiSelect = false;
            MonitorDetailLV.Name = "MonitorDetailLV";
            toolTip1.SetToolTip(MonitorDetailLV, resources.GetString("MonitorDetailLV.ToolTip"));
            MonitorDetailLV.UseCompatibleStateImageBehavior = false;
            MonitorDetailLV.View = View.Details;
            MonitorDetailLV.MouseDoubleClick += MonitorDetailLV_MouseDoubleClick;
            // 
            // PID
            // 
            resources.ApplyResources(PID, "PID");
            // 
            // procName
            // 
            resources.ApplyResources(procName, "procName");
            // 
            // cpuUsage
            // 
            resources.ApplyResources(cpuUsage, "cpuUsage");
            // 
            // vMem
            // 
            resources.ApplyResources(vMem, "vMem");
            // 
            // phyMem
            // 
            resources.ApplyResources(phyMem, "phyMem");
            // 
            // totalMem
            // 
            resources.ApplyResources(totalMem, "totalMem");
            // 
            // downLink
            // 
            resources.ApplyResources(downLink, "downLink");
            // 
            // upLink
            // 
            resources.ApplyResources(upLink, "upLink");
            // 
            // totalLink
            // 
            resources.ApplyResources(totalLink, "totalLink");
            // 
            // runningSeconds
            // 
            resources.ApplyResources(runningSeconds, "runningSeconds");
            // 
            // monitorStatus
            // 
            resources.ApplyResources(monitorStatus, "monitorStatus");
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            toolTip1.SetToolTip(label1, resources.GetString("label1.ToolTip"));
            // 
            // textBoxPID
            // 
            resources.ApplyResources(textBoxPID, "textBoxPID");
            textBoxPID.Name = "textBoxPID";
            toolTip1.SetToolTip(textBoxPID, resources.GetString("textBoxPID.ToolTip"));
            textBoxPID.KeyPress += TextBoxPID_KeyPress;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            toolTip1.SetToolTip(label2, resources.GetString("label2.ToolTip"));
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(btnShotProcess);
            flowLayoutPanel1.Controls.Add(btnStop);
            flowLayoutPanel1.Controls.Add(btnRestart);
            flowLayoutPanel1.Controls.Add(BtnRemove);
            flowLayoutPanel1.Controls.Add(btnBreak);
            flowLayoutPanel1.Controls.Add(BtnOpenFloder);
            flowLayoutPanel1.Controls.Add(BtnOpenResult);
            flowLayoutPanel1.Controls.Add(BtnAnalysis);
            flowLayoutPanel1.Controls.Add(BtnSetting);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            toolTip1.SetToolTip(flowLayoutPanel1, resources.GetString("flowLayoutPanel1.ToolTip"));
            // 
            // btnStop
            // 
            resources.ApplyResources(btnStop, "btnStop");
            btnStop.BackgroundImage = Properties.Resources.stop;
            btnStop.Cursor = Cursors.Hand;
            btnStop.Name = "btnStop";
            toolTip1.SetToolTip(btnStop, resources.GetString("btnStop.ToolTip"));
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += BtnStop_Click;
            // 
            // btnRestart
            // 
            resources.ApplyResources(btnRestart, "btnRestart");
            btnRestart.BackgroundImage = Properties.Resources.reloading;
            btnRestart.Cursor = Cursors.Hand;
            btnRestart.Name = "btnRestart";
            toolTip1.SetToolTip(btnRestart, resources.GetString("btnRestart.ToolTip"));
            btnRestart.UseVisualStyleBackColor = true;
            btnRestart.Click += BtnRestart_Click;
            // 
            // BtnRemove
            // 
            resources.ApplyResources(BtnRemove, "BtnRemove");
            BtnRemove.BackgroundImage = Properties.Resources.remove;
            BtnRemove.Cursor = Cursors.Hand;
            BtnRemove.Name = "BtnRemove";
            toolTip1.SetToolTip(BtnRemove, resources.GetString("BtnRemove.ToolTip"));
            BtnRemove.UseVisualStyleBackColor = true;
            BtnRemove.Click += BtnRemove_Click;
            // 
            // btnBreak
            // 
            resources.ApplyResources(btnBreak, "btnBreak");
            btnBreak.BackgroundImage = Properties.Resources.pause;
            btnBreak.Cursor = Cursors.Hand;
            btnBreak.Name = "btnBreak";
            toolTip1.SetToolTip(btnBreak, resources.GetString("btnBreak.ToolTip"));
            btnBreak.UseVisualStyleBackColor = true;
            btnBreak.Click += BtnBreak_Click;
            // 
            // BtnOpenFloder
            // 
            resources.ApplyResources(BtnOpenFloder, "BtnOpenFloder");
            BtnOpenFloder.BackgroundImage = Properties.Resources.folder;
            BtnOpenFloder.Cursor = Cursors.Hand;
            BtnOpenFloder.Name = "BtnOpenFloder";
            toolTip1.SetToolTip(BtnOpenFloder, resources.GetString("BtnOpenFloder.ToolTip"));
            BtnOpenFloder.UseVisualStyleBackColor = true;
            BtnOpenFloder.Click += BtnOpenFloder_Click;
            // 
            // BtnOpenResult
            // 
            resources.ApplyResources(BtnOpenResult, "BtnOpenResult");
            BtnOpenResult.BackgroundImage = Properties.Resources.floppy;
            BtnOpenResult.Cursor = Cursors.Hand;
            BtnOpenResult.Name = "BtnOpenResult";
            toolTip1.SetToolTip(BtnOpenResult, resources.GetString("BtnOpenResult.ToolTip"));
            BtnOpenResult.UseVisualStyleBackColor = true;
            BtnOpenResult.Click += BtnOpenResult_Click;
            // 
            // BtnAnalysis
            // 
            resources.ApplyResources(BtnAnalysis, "BtnAnalysis");
            BtnAnalysis.BackgroundImage = Properties.Resources.analysis;
            BtnAnalysis.Cursor = Cursors.Hand;
            BtnAnalysis.Name = "BtnAnalysis";
            toolTip1.SetToolTip(BtnAnalysis, resources.GetString("BtnAnalysis.ToolTip"));
            BtnAnalysis.UseVisualStyleBackColor = true;
            BtnAnalysis.Click += BtnAnalysis_Click;
            // 
            // BtnSetting
            // 
            resources.ApplyResources(BtnSetting, "BtnSetting");
            BtnSetting.BackgroundImage = Properties.Resources.setting;
            BtnSetting.Cursor = Cursors.Hand;
            BtnSetting.Name = "BtnSetting";
            toolTip1.SetToolTip(BtnSetting, resources.GetString("BtnSetting.ToolTip"));
            BtnSetting.UseVisualStyleBackColor = true;
            BtnSetting.Click += BtnSetting_Click;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(flowLayoutPanel1);
            Controls.Add(label2);
            Controls.Add(textBoxPID);
            Controls.Add(label1);
            Controls.Add(MonitorDetailLV);
            Controls.Add(labelCpuAndMem);
            Name = "MainForm";
            toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            FormClosing += MainForm_FormClosing;
            flowLayoutPanel1.ResumeLayout(false);
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
        private Button BtnRemove;
        private Button btnRestart;
        private ToolTip toolTip1;
        private Button BtnOpenFloder;
        private Button BtnAnalysis;
        private Button BtnOpenResult;
        private Button BtnSetting;
    }
}