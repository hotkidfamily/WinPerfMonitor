namespace PerfMonitor
{
    partial class HistoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if ( disposing && (components != null) )
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
        private void InitializeComponent ()
        {
            components = new System.ComponentModel.Container();
            LVHistory = new ListView();
            HistoryMenuStrip = new ContextMenuStrip(components);
            OpenToolStripMenuItem = new ToolStripMenuItem();
            DeleteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            ModifyMarkerToolStripMenuItem = new ToolStripMenuItem();
            HistoryMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // LVHistory
            // 
            LVHistory.FullRowSelect = true;
            LVHistory.GridLines = true;
            LVHistory.Location = new Point(11, 11);
            LVHistory.Margin = new Padding(2);
            LVHistory.Name = "LVHistory";
            LVHistory.Size = new Size(562, 447);
            LVHistory.TabIndex = 0;
            LVHistory.UseCompatibleStateImageBehavior = false;
            LVHistory.View = View.Details;
            LVHistory.MouseClick += LVHistory_MouseClick;
            // 
            // HistoryMenuStrip
            // 
            HistoryMenuStrip.Items.AddRange(new ToolStripItem[] { OpenToolStripMenuItem, DeleteToolStripMenuItem, toolStripSeparator1, ModifyMarkerToolStripMenuItem });
            HistoryMenuStrip.Name = "contextMenuStrip1";
            HistoryMenuStrip.Size = new Size(125, 76);
            // 
            // OpenToolStripMenuItem
            // 
            OpenToolStripMenuItem.Image = Properties.Resources.floppy;
            OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            OpenToolStripMenuItem.Size = new Size(124, 22);
            OpenToolStripMenuItem.Text = "打开";
            OpenToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // DeleteToolStripMenuItem
            // 
            DeleteToolStripMenuItem.Image = Properties.Resources.remove;
            DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            DeleteToolStripMenuItem.Size = new Size(124, 22);
            DeleteToolStripMenuItem.Text = "删除";
            DeleteToolStripMenuItem.Click += DeleteToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(121, 6);
            // 
            // ModifyMarkerToolStripMenuItem
            // 
            ModifyMarkerToolStripMenuItem.Image = Properties.Resources.marker;
            ModifyMarkerToolStripMenuItem.Name = "ModifyMarkerToolStripMenuItem";
            ModifyMarkerToolStripMenuItem.Size = new Size(124, 22);
            ModifyMarkerToolStripMenuItem.Text = "修改备注";
            ModifyMarkerToolStripMenuItem.Click += ModifyMarkerToolStripMenuItem_Click;
            // 
            // HistoryForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 469);
            Controls.Add(LVHistory);
            Margin = new Padding(2);
            Name = "HistoryForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "HistoryForm";
            HistoryMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView LVHistory;
        private ContextMenuStrip HistoryMenuStrip;
        private ToolStripMenuItem OpenToolStripMenuItem;
        private ToolStripMenuItem DeleteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem ModifyMarkerToolStripMenuItem;
    }
}