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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnShotProcess = new Button();
            labelCpuAndMem = new TextBox();
            listView1 = new ListView();
            procName = new ColumnHeader();
            cpu = new ColumnHeader();
            vMem = new ColumnHeader();
            phyMem = new ColumnHeader();
            GPU = new ColumnHeader();
            label1 = new Label();
            textBoxPID = new TextBox();
            label2 = new Label();
            SuspendLayout();
            // 
            // btnShotProcess
            // 
            btnShotProcess.BackgroundImage = Properties.Resources.game_game_shooting1;
            btnShotProcess.BackgroundImageLayout = ImageLayout.Zoom;
            btnShotProcess.Cursor = Cursors.Cross;
            btnShotProcess.DialogResult = DialogResult.OK;
            btnShotProcess.FlatStyle = FlatStyle.Popup;
            btnShotProcess.ImageKey = "(无)";
            btnShotProcess.Location = new Point(573, 14);
            btnShotProcess.Margin = new Padding(2);
            btnShotProcess.Name = "btnShotProcess";
            btnShotProcess.Size = new Size(41, 30);
            btnShotProcess.TabIndex = 0;
            btnShotProcess.UseVisualStyleBackColor = true;
            btnShotProcess.Click += btnShotProcess_Click;
            btnShotProcess.MouseDown += btnShotProcess_MouseDown;
            btnShotProcess.MouseUp += btnShotProcess_MouseUp;
            // 
            // labelCpuAndMem
            // 
            labelCpuAndMem.BorderStyle = BorderStyle.FixedSingle;
            labelCpuAndMem.Location = new Point(13, 405);
            labelCpuAndMem.Margin = new Padding(2);
            labelCpuAndMem.Name = "labelCpuAndMem";
            labelCpuAndMem.ReadOnly = true;
            labelCpuAndMem.Size = new Size(726, 23);
            labelCpuAndMem.TabIndex = 1;
            labelCpuAndMem.TextAlign = HorizontalAlignment.Right;
            // 
            // listView1
            // 
            listView1.BorderStyle = BorderStyle.FixedSingle;
            listView1.Columns.AddRange(new ColumnHeader[] { procName, cpu, vMem, phyMem, GPU });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(13, 98);
            listView1.Name = "listView1";
            listView1.Size = new Size(726, 280);
            listView1.TabIndex = 3;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 21);
            label1.Name = "label1";
            label1.Size = new Size(96, 17);
            label1.TabIndex = 4;
            label1.Text = "输入 PID 开始：";
            // 
            // textBoxPID
            // 
            textBoxPID.Location = new Point(115, 18);
            textBoxPID.Name = "textBoxPID";
            textBoxPID.Size = new Size(100, 23);
            textBoxPID.TabIndex = 5;
            textBoxPID.KeyPress += textBoxPID_KeyPress;
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
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(751, 439);
            Controls.Add(label2);
            Controls.Add(textBoxPID);
            Controls.Add(label1);
            Controls.Add(listView1);
            Controls.Add(labelCpuAndMem);
            Controls.Add(btnShotProcess);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnShotProcess;
        private TextBox labelCpuAndMem;
        private ListView listView1;
        private ColumnHeader procName;
        private ColumnHeader cpu;
        private ColumnHeader vMem;
        private ColumnHeader phyMem;
        private ColumnHeader GPU;
        private Label label1;
        private TextBox textBoxPID;
        private Label label2;
    }
}