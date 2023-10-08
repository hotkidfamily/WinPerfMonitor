namespace Perfmon
{
    partial class VisualForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualForm));
            formsPlotProcCPU = new ScottPlot.FormsPlot();
            formsPlotProcMem = new ScottPlot.FormsPlot();
            formsPlotUpLink = new ScottPlot.FormsPlot();
            formsPlotSysCpu = new ScottPlot.FormsPlot();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // formsPlotProcCPU
            // 
            formsPlotProcCPU.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotProcCPU.Location = new Point(4, 3);
            formsPlotProcCPU.Margin = new Padding(4, 3, 4, 3);
            formsPlotProcCPU.Name = "formsPlotProcCPU";
            formsPlotProcCPU.Size = new Size(768, 163);
            formsPlotProcCPU.TabIndex = 0;
            // 
            // formsPlotProcMem
            // 
            formsPlotProcMem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotProcMem.Location = new Point(4, 172);
            formsPlotProcMem.Margin = new Padding(4, 3, 4, 3);
            formsPlotProcMem.Name = "formsPlotProcMem";
            formsPlotProcMem.Size = new Size(768, 163);
            formsPlotProcMem.TabIndex = 1;
            // 
            // formsPlotUpLink
            // 
            formsPlotUpLink.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotUpLink.Location = new Point(4, 341);
            formsPlotUpLink.Margin = new Padding(4, 3, 4, 3);
            formsPlotUpLink.Name = "formsPlotUpLink";
            formsPlotUpLink.Size = new Size(768, 163);
            formsPlotUpLink.TabIndex = 2;
            // 
            // formsPlotSysCpu
            // 
            formsPlotSysCpu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotSysCpu.Location = new Point(4, 510);
            formsPlotSysCpu.Margin = new Padding(4, 3, 4, 3);
            formsPlotSysCpu.Name = "formsPlotSysCpu";
            formsPlotSysCpu.Size = new Size(768, 164);
            formsPlotSysCpu.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(formsPlotProcCPU, 0, 0);
            tableLayoutPanel1.Controls.Add(formsPlotSysCpu, 0, 3);
            tableLayoutPanel1.Controls.Add(formsPlotProcMem, 0, 1);
            tableLayoutPanel1.Controls.Add(formsPlotUpLink, 0, 2);
            tableLayoutPanel1.Location = new Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(776, 677);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // VisualForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 701);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(816, 740);
            Name = "VisualForm";
            Text = "VisualForm";
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScottPlot.FormsPlot formsPlotProcCPU;
        private ScottPlot.FormsPlot formsPlotProcMem;
        private ScottPlot.FormsPlot formsPlotUpLink;
        private ScottPlot.FormsPlot formsPlotSysCpu;
        private TableLayoutPanel tableLayoutPanel1;
    }
}