using PerfMonitor.Library;
using System.Diagnostics;
using static PerfMonitor.MainForm;

namespace PerfMonitor
{
    public partial class HistoryForm : Form
    {
        private readonly string[] _columns = new string[] { "备注", "PID", "日期", "结果" };
        private readonly int[] _columnsWidth = new int[] { 100, 100, 200, 200 };
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

            _history = (HistoryController) history;
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
            var item = LVHistory.FocusedItem;
            if ( item != null )
            {
                HistoryItem v = (HistoryItem)item.Tag;

                ProcessStartInfo psi = new()
                {
                    FileName = v.ResPath,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void DeleteToolStripMenuItem_Click (object sender, EventArgs e)
        {
            var item = LVHistory.FocusedItem;
            if ( item != null )
            {
                HistoryItem v = (HistoryItem)item.Tag;
                _history.RemoveItem(v);
                LVHistory.Items.Remove(item);
            }
        }

        private void ModifyMarkerToolStripMenuItem_Click (object sender, EventArgs e)
        {
            var item = LVHistory.FocusedItem;
            item?.BeginEdit();
        }

        private void LVHistory_AfterLabelEdit (object sender, LabelEditEventArgs e)
        {
            if ( e.Label != null )
            {
                string editedText = e.Label;

                var item = LVHistory.FocusedItem;
                var ctx = (HistoryItem)item.Tag;
                if ( ctx != null )
                {
                    ctx.Marker = editedText;
                    _history.Write();
                }
            }
        }

        private void LVHistory_KeyDown (object sender, KeyEventArgs e)
        {
            var item = LVHistory.FocusedItem;
            if ( e.KeyCode == Keys.F2 && item != null )
            {
                item.BeginEdit();
            }
            else if ( e.KeyCode == Keys.F5 )
            {
                LVHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                LVHistory.BeginUpdate();
                for ( int i = 0; i <= LVHistory.Columns.Count - 1; i++ )
                {
                    LVHistory.Columns[i].Width += 20;
                }
                LVHistory.EndUpdate();
            }
        }

        private void FreshToolStripMenuItem_Click (object sender, EventArgs e)
        {
            LVHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            LVHistory.BeginUpdate();
            for ( int i = 0; i <= LVHistory.Columns.Count - 1; i++ )
            {
                LVHistory.Columns[i].Width += 20;
            }
            LVHistory.EndUpdate();
        }
    }
}