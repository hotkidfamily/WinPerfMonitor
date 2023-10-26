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
            LVHistory = new ListView();
            SuspendLayout();
            // 
            // LVHistory
            // 
            LVHistory.Location = new Point(11, 11);
            LVHistory.Margin = new Padding(2, 2, 2, 2);
            LVHistory.Name = "LVHistory";
            LVHistory.Size = new Size(562, 447);
            LVHistory.TabIndex = 0;
            LVHistory.UseCompatibleStateImageBehavior = false;
            LVHistory.View = View.Details;
            // 
            // HistoryForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 469);
            Controls.Add(LVHistory);
            Margin = new Padding(2, 2, 2, 2);
            Name = "HistoryForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "HistoryForm";
            ResumeLayout(false);
        }

        #endregion

        private ListView LVHistory;
    }
}