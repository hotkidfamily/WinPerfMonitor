﻿namespace Perfmon
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnShotProcess = new Button();
            labelCpuAndMem = new TextBox();
            SuspendLayout();
            // 
            // btnShotProcess
            // 
            btnShotProcess.BackgroundImage = Properties.Resources.game_game_shooting1;
            btnShotProcess.Cursor = Cursors.Cross;
            btnShotProcess.DialogResult = DialogResult.OK;
            btnShotProcess.FlatStyle = FlatStyle.Popup;
            btnShotProcess.Image = Properties.Resources.game_game_shooting1;
            btnShotProcess.Location = new Point(855, 12);
            btnShotProcess.Name = "btnShotProcess";
            btnShotProcess.Size = new Size(71, 64);
            btnShotProcess.TabIndex = 0;
            btnShotProcess.Text = "Find";
            btnShotProcess.UseVisualStyleBackColor = true;
            btnShotProcess.Click += btnShotProcess_Click;
            btnShotProcess.MouseDown += btnShotProcess_MouseDown;
            btnShotProcess.MouseUp += btnShotProcess_MouseUp;
            // 
            // labelCpuAndMem
            // 
            labelCpuAndMem.BorderStyle = BorderStyle.None;
            labelCpuAndMem.Location = new Point(467, 571);
            labelCpuAndMem.Name = "labelCpuAndMem";
            labelCpuAndMem.ReadOnly = true;
            labelCpuAndMem.Size = new Size(646, 23);
            labelCpuAndMem.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1180, 620);
            Controls.Add(labelCpuAndMem);
            Controls.Add(btnShotProcess);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnShotProcess;
        private TextBox labelCpuAndMem;
    }
}