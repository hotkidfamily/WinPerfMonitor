using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfMonitor
{
    public partial class HistoryForm : Form
    {
        internal class HistoryTask
        {
            private string resPath;
            private string mark;

            public string ResPath { get => resPath; set => resPath = value; }
            public string Mark { get => mark; set => mark = value; }
        }

        private HistoryTask result = default!;

        internal HistoryTask Result { get => result; set => result = value; }

        private readonly string[] _columns = new string[] { "PID", "结果", "备注"};
        private readonly int[] _columnsWidth = new int[] { 100, 200, 100};

        public HistoryForm (string jsonConfig)
        {
            InitializeComponent();
            LVHistory.Columns.Clear();

            for ( int i = 0; i < _columns?.Length; i++ )
            {
                ColumnHeader ch = new()
                {
                    Width = _columnsWidth[i],
                    TextAlign = HorizontalAlignment.Left,
                    Text = _columns?[i],
                };
                LVHistory.Columns.Add(ch);
            }
        }
    }
}
