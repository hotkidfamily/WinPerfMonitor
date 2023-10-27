using PerfMonitor.Library;
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
        private readonly string[] _columns = new string[] { "PID", "日期", "备注", "结果"};
        private readonly int[] _columnsWidth = new int[] { 100, 100, 200, 200};
        private readonly HistoryController _history;

        public HistoryForm (object history)
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

            _history = (HistoryController)history;
            LVHistory.BeginUpdate();
            foreach ( var res in _history.History )
            {
                var lvi = new ListViewItem(res.Info())
                {
                    Tag = res
                };
                LVHistory.Items.Add(lvi);
            }
            LVHistory.EndUpdate();
        }

        private void LVHistory_MouseClick (object sender, MouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Right )
            {
                var item = LVHistory.FocusedItem;
                if ( item != null && item.Bounds.Contains(e.Location) )
                {
                    HistoryMenuStrip.Show(Cursor.Position);
                }
            }
        }

        private void OpenToolStripMenuItem_Click (object sender, EventArgs e)
        {

        }

        private void DeleteToolStripMenuItem_Click (object sender, EventArgs e)
        {

        }

        private void ModifyMarkerToolStripMenuItem_Click (object sender, EventArgs e)
        {

        }
    }
}
