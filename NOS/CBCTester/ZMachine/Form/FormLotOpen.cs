using System;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using COMMON;
using SML;
using System.Drawing;
using System.Text;
using System.IO;

namespace Machine
{
    public partial class FormLotOpen : Form
    {
        public static FormLotOpen FrmLotOpen;
        private const string sFormText = "Form Operation ";

        public FormLotOpen()
        {
            InitializeComponent();
            
            tbSelDevice.Text = OM.GetCrntDev();
            tbLotNo    .Text = "";

            tbLotNo.Focus();
            
        }

        private void UpdateLotList()
        {
            if(LOT.LotList.Count < 1) return;

            DgView.Rows.Clear();
            
            for(int i=0; i<LOT.LotList.Count; i++)
            {
                DgView.Rows.Add();
                DgView.SetString("LOTNAME",i,LOT.LotList[i]);
            }
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //if (tbLotNo.Text == "") return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");

            string LotNo  = tbLotNo.Text.Trim();
            string Device = tbSelDevice.Text.Trim();

            //LOT.LotOpen(LotNo);

            //Update LotList
            LOT.LotList.Clear();
            for(int i=0; i<DgView.Rows.Count-1; i++)
            {
                string sOriLotNo = DgView.GetString("LOTNAME",i); 

                //시부래 해놓고 생각해보니 Apply버튼 누를때 마다 언더바 숫자가 붙게 되네;;;;
                //다시 PopMgz으로 가서 해보자.

                //string sLotNo = SPC.MAP.GetLotNo(sOriLotNo) ;     //중복작업인지 DataMap폴더의 데이터가 있는지 확인 한후 재가동 숫자를 붙임.
                //for(int j = 0 ; j < i ; j++) //이번엔 랏리스트에 혹시 또라이마냥 또 똑같은걸 넣는 상황을 위해 랏리스트 확인.
                //{
                //    string sListLotNo = DgView.GetString("LOTNAME",j);
                //    if(sOriLotNo == sListLotNo)
                //    {
                //        Log.ShowMessage("Error" , )
                //    }
                //}
                LOT.LotList.Add(sOriLotNo);
            }

            Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            Close();   
        }

        private void FormLotOpen_Shown(object sender, EventArgs e)
        {
            UpdateLotList();
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //if (LOT.LotList.Count != 0 && lvLot.SelectedIndices[0] != 0)
            //{
            //    LOT.LotList.Remove(LOT.LotList[lvLot.SelectedIndices[0]]);
            //}


            UpdateLotList();
        }

        private void tbMaterialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                tbLotNo.Focus();
            }
        }

        private void tbLotNo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btLotOpen.Focus();
            }
        }

        private void tbTargetBin_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                //btAdd_Click(sender , e);
                //tbMaterialNo.Focus();
                btLotOpen.Focus();
            }
        }

        private void FormLotOpen_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                if(e.RowIndex == senderGrid.Rows.Count - 1)
                {
                    senderGrid.Rows.Add();
                }
                if(e.ColumnIndex == 1 && e.RowIndex < senderGrid.Rows.Count - 1) senderGrid.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void DgView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            //String rowIdx = (e.RowIndex + 1).ToString("X4");
            String rowIdx = (e.RowIndex + 1).ToString();
            
            StringFormat centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            
            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top,grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText,headerBounds,centerFormat);
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            DgView.Rows.Clear();
        }
    }
    //컬럼 이름으로 값을 가져오기 위한 Extention Class
    public static class DataGridViewExtenstions
    {
        //public static object GetCellValueFromColumnHeader(this DataGridViewCellCollection CellCollection, string HeaderText)
        //{
        //    //return CellCollection.Cast<DataGridViewCell>().First(c => c.OwningColumn.HeaderText == HeaderText).Value;            
        //
        //    foreach(DataGridViewColumn Col in )
        //
        //
        //
        //}

        //public static object GetDataByName(this DataGridViewCellCollection CellCollection, string HeaderText)
        //{
        //    //return CellCollection.Cast<DataGridViewCell>().First(c => c.OwningColumn.HeaderText == HeaderText).Value;            
        //
        //    foreach(DataGridViewColumn Col in )
        //
        //
        //
        //}

        public static string GetString(this DataGridView GridView ,  string _sName , int _iRow)
        {
            string sRet = "";
            for(int c = 0 ; c < GridView.ColumnCount ; c++)
            {
                if(GridView.Columns[c].Name == _sName)
                {
                    try
                    {
                        sRet = GridView[c,_iRow].Value.ToString();
                    }
                    catch
                    {
                        sRet = "";
                    }
                    return sRet ;
                    
                }
            }
            return sRet;
        }

        //이거 되려나....
        public static bool SetString(this DataGridView GridView , string _sName , int _iRow , string _sValue)
        {
            for(int c = 0 ; c < GridView.ColumnCount ; c++)
            {
                if(GridView.Columns[c].Name == _sName) {
                    GridView[c,_iRow].Value = _sValue ;
                    return true ;
                }
            }
            return false ;

        }

        public static void CopyToClipboard(this DataGridView GridView)
        {
            DataObject obj = GridView.GetClipboardContent();
            if(obj != null)
            {
                Clipboard.SetDataObject(obj);
            }
        }

        public static void PasteClipboard(this DataGridView GridView)
        {
            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            int iRow = GridView.CurrentCell.RowIndex   ;
            int iCol = GridView.CurrentCell.ColumnIndex;
            DataGridViewCell oCell;
            try//if(true)//iRow + lines.Length > gvPara.Rows.Count-1)
            {
                bool bFlag = false;
                foreach(string sEmpty in lines) 
                { 
                    if(sEmpty == "")
                    {
                        bFlag = true;
                    }
                }
                int iNewRows = iRow + lines.Length - GridView.Rows.Count;
                if(iNewRows >=0)
                {
                    if(bFlag) GridView.Rows.Add(iNewRows);
                    else      GridView.Rows.Add(iNewRows+1);
                }
                else
                {
                    //gvPara.Rows.Add(iNewRows+1);
                }
                foreach(string line in lines)
                {
                    if(iRow < GridView.RowCount && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');
                        for(int i=0;i<sCells.GetLength(0);++i)
                        {
                            if(iCol+i<GridView.ColumnCount)
                            {
                                oCell = GridView[iCol+i,iRow];
                                oCell.Value = Convert.ChangeType(sCells[i].Replace("\r",""),oCell.ValueType);
                            }
                            else
                            {
                                break;
                            }
                        }
                            iRow++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch(FormatException)
            {
                return;
            }
            //Clipboard.Clear();
        }

        [Serializable]
        public sealed class ColumnInfo
        {
            public string Name { get; set; }
            public int DisplayIndex { get; set; }
            public int Width { get; set; }
            public bool Visible { get; set; }
        }
        public static bool Load(this DataGridView GridView, string fileName)
        {
            GridView.Rows.Clear();

            string [] sDatas ;
            try
            {
                sDatas= System.IO.File.ReadAllLines(fileName, Encoding.Default);
            }
            catch(Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return false ;
            }
                

            //해더만 있어도 리턴.
            if(sDatas.Length < 2) return false ;

            //0번은 해더.
            GridView.RowCount = sDatas.Length ;
            for (int r = 1; r < sDatas.Length ; r++)
            {
                string [] RowData = sDatas[r].Split(',');
                if (RowData.Length != GridView.ColumnCount) return false;
                for(int c = 0 ; c < RowData.Length ; c++)
                {
                    GridView[c,r-1].Value = RowData[c];
                }

                //GridView.Rows.Add(RowData);
            }
            return true ;
        }

        public static bool Save(this DataGridView GridView, string fileName)
        {

            try
            {
                if (GridView.ColumnCount <= 0) return false;
                using (System.IO.FileStream csvFileWriter = new FileStream(fileName,FileMode.OpenOrCreate))
                {
                    //Header
                    string columnHeaderText = "";
                    for (int c = 0; c < GridView.ColumnCount; c++)
                    {
                        columnHeaderText += GridView.Columns[c].Name;
                        if (c != GridView.ColumnCount - 1) columnHeaderText += ',';
                    }
                    Byte[] Bytes = Encoding.GetEncoding("ks_c_5601-1987").GetBytes(columnHeaderText+"\n");
                    csvFileWriter.Write(Bytes, 0, Bytes.Length);

                    if (GridView.RowCount <= 0) return false;
                    //Data
                    string RowData = "";
                    for (int r = 0; r < GridView.RowCount - 1; r++)
                    {
                        RowData = "";
                        for (int c = 0; c < GridView.ColumnCount; c++)
                        {
                            //if(GridView.Columns[c] is DataGridViewComboBoxColumn)
                            //{
                            //    RowData +=uint.Parse(GridView.Rows[r].Cells[c].Value);
                            //}
                            //else
                            //{
                            //    RowData += GridView[c, r].Value.ToString();
                            //}

                            if(GridView[c, r].Value == null) RowData +=  "" ;
                            else                             RowData += GridView[c, r].Value.ToString();

                            if (c != GridView.ColumnCount - 1) RowData += ",";

                        }
                        Bytes = Encoding.GetEncoding("ks_c_5601-1987").GetBytes(RowData+"\n");
                        //csvFileWriter.Write(Bytes, 0, Bytes.Length);
                        //csvFileWriter.wr
                        csvFileWriter.Write(Bytes, 0, Bytes.Length);
                    }
                    csvFileWriter.Close();
                }


            }
            catch (Exception exceptionObject)
            {
                MessageBox.Show(exceptionObject.ToString());
                return false;
            }
            return true;

        }
    }

}
