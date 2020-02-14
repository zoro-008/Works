using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log
{
    public partial class LogFind : Form
    {
        LogMain LM;
        int iIdx;
        public LogFind(LogMain _LogMain)
        {
            InitializeComponent();

            this.TopMost = true;

            LM = _LogMain;
            iIdx = LM.GetTabIdx();
        }

        public void Find()
        {
            iIdx = LM.GetTabIdx();
            LM.lst[iIdx].Lsv.SelectedIndices.Clear();
            //LM.
            //iIdx = LM.
            string searchValue = textBox1.Text;

            ListViewItem item = FindItem(searchValue, 0);
            
            if (item == null)
                MessageBox.Show("No data found","FIND RESULT");
            else
                SelectItem(item);

        }

        public void FindNext()
        {
            iIdx = LM.GetTabIdx();
            string searchValue = textBox1.Text;
            int iRow = 0;

            if (radioButton1.Checked)
            {
                if (LM.lst[iIdx].Lsv.SelectedIndices.Count > 0)
                    iRow = LM.lst[iIdx].Lsv.SelectedIndices[0];
            }
            else
            {
                if (LM.lst[iIdx].Lsv.SelectedIndices.Count > 0)
                    iRow = LM.lst[iIdx].Lsv.SelectedIndices[LM.lst[iIdx].Lsv.SelectedIndices.Count - 1];
            }

            ListViewItem item = FindItem(searchValue, iRow, true);

            if (item == null)
                MessageBox.Show("No data found", "Find Result");
            else
                SelectItem(item);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void SelectItem(ListViewItem item)
        {
            //LM.lst[iIdx].Lsv.MultiSelect = false;
            //LM.lst[iIdx].Lsv.HideSelection = false;
            //item.Focused = true;
            item.Selected = true;
            item.EnsureVisible();
           
        }

        private ListViewItem FindItem(string keyword, int _startIndex, bool bNext = false)
        {
            StringComparison comp;
            if(checkBox1.Checked) comp = StringComparison.Ordinal;
            else                  comp = StringComparison.OrdinalIgnoreCase;
            int startIndex;
            if (radioButton2.Checked)
            {
                if (bNext) startIndex = _startIndex + 1;
                else startIndex = 0;
                for (int i = startIndex; i < LM.lst[iIdx].Lsv.Items.Count; i++)
                {
                    ListViewItem item = LM.lst[iIdx].Lsv.Items[i];
                    //bool isContains = item.SubItems[2].Text.Contains(keyword, comp);
                    bool isContains = item.SubItems[2].Text.IndexOf(keyword, comp) >= 1 ? true : false;
                    if (isContains)
                    {
                        return item;
                    }
                }
            }
            else
            {
                if(bNext) startIndex = _startIndex - 1;
                else      startIndex = LM.lst[iIdx].Lsv.Items.Count - 1;
                for (int i = startIndex; i >= 0; i--)
                {
                    ListViewItem item = LM.lst[iIdx].Lsv.Items[i];
                    //bool isContains = item.SubItems[2].Text.Contains(keyword, comp);
                    bool isContains = item.SubItems[2].Text.IndexOf(keyword, comp) >= 1 ? true : false;
                    if (isContains)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void LogFind_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                button2_Click(sender, e);
            }
        }

        private void LogFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            if (e.KeyCode == Keys.F3)
            {
                Find();
            }
            if (e.KeyCode == Keys.F4)
            {
                FindNext();
            }
        }

    }
}
