using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormCsv : Form
    {
        string sPath ;
        public FormCsv()
        {
            InitializeComponent();

            sPath = "D:\\Temp.csv";
            listView1.Clear();
            listView1.View = View.Details;
            listView1.LabelEdit = false;
            listView1.AllowColumnReorder = true;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            //listView1.Sorting = SortOrder.Descending;
            listView1.Scrollable = true;

            //for (int c = 0; c < 20; c++)
            //{
            //    listView1.Columns.Add(c.ToString(), 100, HorizontalAlignment.Left);
            //}
            //listView1.HeaderStyle = ColumnHeaderStyle.None;
        }

        private ListViewItem[] getItems(IEnumerable lines){
            List<ListViewItem> items = new List<ListViewItem>();
            bool bFst = true;
            foreach(string line in lines){
                if(bFst) { bFst = false; continue; }
                string[] logLine = line.Split(',');
                ListViewItem dateItem = new ListViewItem(logLine[0]);
                for(int i=1 ; i<logLine.Length; i++)
                {
                    dateItem.SubItems.Add(logLine[i]);
                }
                items.Add(dateItem);
            }
            return items.ToArray();
        }

        private List<string> loadFile(string filePath){
            List<String> lines = new List<string>();
            using(StreamReader streamReader = new StreamReader(filePath)){
                while(!streamReader.EndOfStream){
                    lines.Add(streamReader.ReadLine());
                }
            }
            String[] sSplit = lines[0].Split(',');
            listView1.Clear();
            for (int i = 0; i < sSplit.Length; i++)
            {
                if(sSplit[i] != "") listView1.Columns.Add(sSplit[i], 100, HorizontalAlignment.Left);
            }
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            return lines;
        }

        public void Load(string _sPath){
            if (!File.Exists(_sPath)) return ;
            if(File.Exists(sPath)) File.Delete(sPath);
            File.Copy(_sPath,sPath);
            this.Text = _sPath ;
            listView1.BeginUpdate();
            listView1.Items.AddRange(getItems(loadFile(sPath)));
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.EndUpdate();
        }






















    }
}
