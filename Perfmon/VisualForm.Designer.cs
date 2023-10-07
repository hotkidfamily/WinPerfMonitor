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
            SuspendLayout();
            // 
            // formsPlotProcCPU
            // 
            formsPlotProcCPU.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotProcCPU.Location = new Point(13, 13);
            formsPlotProcCPU.Margin = new Padding(4, 3, 4, 3);
            formsPlotProcCPU.Name = "formsPlotProcCPU";
            formsPlotProcCPU.Size = new Size(774, 164);
            formsPlotProcCPU.TabIndex = 0;
            // 
            // formsPlotProcMem
            // 
            formsPlotProcMem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotProcMem.Location = new Point(13, 195);
            formsPlotProcMem.Margin = new Padding(4, 3, 4, 3);
            formsPlotProcMem.Name = "formsPlotProcMem";
            formsPlotProcMem.Size = new Size(774, 164);
            formsPlotProcMem.TabIndex = 1;
            // 
            // formsPlotUpLink
            // 
            formsPlotUpLink.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotUpLink.Location = new Point(13, 377);
            formsPlotUpLink.Margin = new Padding(4, 3, 4, 3);
            formsPlotUpLink.Name = "formsPlotUpLink";
            formsPlotUpLink.Size = new Size(774, 164);
            formsPlotUpLink.TabIndex = 2;
            // 
            // formsPlotSysCpu
            // 
            formsPlotSysCpu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotSysCpu.Location = new Point(13, 559);
            formsPlotSysCpu.Margin = new Padding(4, 3, 4, 3);
            formsPlotSysCpu.Name = "formsPlotSysCpu";
            formsPlotSysCpu.Size = new Size(774, 164);
            formsPlotSysCpu.TabIndex = 3;
            // 
            // VisualForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 735);
            Controls.Add(formsPlotSysCpu);
            Controls.Add(formsPlotUpLink);
            Controls.Add(formsPlotProcMem);
            Controls.Add(formsPlotProcCPU);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(816, 774);
            Name = "VisualForm";
            Text = "VisualForm";
            ResumeLayout(false);
        }

        #endregion

        private ScottPlot.FormsPlot formsPlotProcCPU;
        private ScottPlot.FormsPlot formsPlotProcMem;
        private ScottPlot.FormsPlot formsPlotUpLink;
        private ScottPlot.FormsPlot formsPlotSysCpu;
    }
}