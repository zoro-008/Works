using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMMON;
using SMDll2;
using System.IO;
using System.Reflection;

namespace Machine
{
    public partial class FormSPC : Form
    {
        public FormMain FrmMain;

        public static string LOT_FOLDER = "d:\\LotLog\\" + OM.EqpOptn.sModelName + "\\";   //LOT_FOLDER
        public static string WRK_FOLDER = "d:\\WrkLog\\" + OM.EqpOptn.sModelName + "\\";   //WRK_FOLDER

        public int iNo = 1;
        

        LotData.TData tData;

        public FormSPC(Panel _pnBase)
        {
            InitializeComponent();

            //string sDate = lvLotDate.Items[Convert.ToInt32.lvLotDate.Items].SubItems[1].ToString() ;
            this.TopLevel = false;
            this.Parent = _pnBase;

            //FrmMain = _FrmMain;
            DispDateList();

            //ng sDate = Convert.ToString(lvLotDate.Items[0].SubItems[1]);

            //DispLotList(sDate, lvLotInfo);

            
        }

        


        public void DispLotList(string _sDate, ListView _lvLotInfo)
        {
            //TSearchRec sr          ;
            string sPath = LOT_FOLDER + _sDate;
 
            string sSerchFile = sPath + "\\*.ini";

            
            DateTime tTime;

            tData.Clear();

            _lvLotInfo.Clear();
            _lvLotInfo.View               = View.Details;
            _lvLotInfo.LabelEdit          = true;
            _lvLotInfo.AllowColumnReorder = true;
            _lvLotInfo.FullRowSelect      = true;
            _lvLotInfo.GridLines          = true;
            _lvLotInfo.Sorting            = SortOrder.Descending;
            _lvLotInfo.Scrollable         = true;
            
            _lvLotInfo.Columns.Add("No"       , 40 , HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("LotId"    , 100, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("WorkCount", 210, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("UPEH"     , 210, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("UPH"      , 210, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("Start"    , 210, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("End  "    , 210, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("WorkTime" , 210, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("TotalTime", 210, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("JobFile"  , 210, HorizontalAlignment.Left);

            //LotCount확인
            _lvLotInfo.Items.Clear();

            DirectoryInfo Info = new DirectoryInfo(sPath);
            int FileCount = Info.GetFiles().Length;

            ListViewItem[] liitem = new ListViewItem[FileCount];

            for (int i = 0; i < FileCount; i++)
            {
                liitem[i] = new ListViewItem(string.Format("{0}", iNo));
                liitem[i].SubItems.Add("");
                liitem[i].SubItems.Add("");
                liitem[i].SubItems.Add("");
                liitem[i].SubItems.Add("");
                liitem[i].SubItems.Add("");
                liitem[i].SubItems.Add("");
                liitem[i].SubItems.Add("");
                liitem[i].SubItems.Add("");
                liitem[i].SubItems.Add("");
                
                liitem[i].UseItemStyleForSubItems = false;
                liitem[i].UseItemStyleForSubItems = false;

                _lvLotInfo.Items.Add(liitem[i]);
                iNo++;
            }

            

            //LotData
            string sTemp;
            if (Info.Exists)
            {
                tData.dUPEH = 0;
                tData.dUPH = 0;
                foreach (FileInfo info in Info.GetFiles())
                {
                    //sIniPath = sPath + "\\" + sr.Name;
                    CIniFile IniLoadLotList = new CIniFile(sPath);

                    //랏 정보 블러오기.

                    IniLoadLotList.Load("Data", "sLotId    ", out tData.sLotId    ); //_sgLotList->Cells[1][iLotCnt + 1] = tData.sLotId;
                    IniLoadLotList.Load("Data", "iWorkCnt  ", out tData.iWorkCnt  ); //_sgLotList->Cells[2][iLotCnt + 1] = tData.iWorkCnt;

                    IniLoadLotList.Load("Data", "dUPEH     ", out tData.dUPEH     ); //_sgLotList->Cells[3][iLotCnt + 1] = sTemp;

                    IniLoadLotList.Load("Data", "dUPH      ", out tData.dUPH      ); //_sgLotList->Cells[4][iLotCnt + 1] = sTemp;
                    IniLoadLotList.Load("Data", "dSttTime  ", out tData.dSttTime  ); //tTime.Val = tData.dSttTime; _sgLotList->Cells[5][iLotCnt + 1] = tTime.FormatString("hh:nn:ss");
                    IniLoadLotList.Load("Data", "dEndTime  ", out tData.dEndTime  ); //tTime.Val = tData.dEndTime; _sgLotList->Cells[6][iLotCnt + 1] = tTime.FormatString("hh:nn:ss");
                    IniLoadLotList.Load("Data", "dWorkTime ", out tData.dWorkTime ); //tTime.Val = tData.dWorkTime; _sgLotList->Cells[7][iLotCnt + 1] = tTime.FormatString("hh:nn:ss");
                    IniLoadLotList.Load("Data", "dTotalTime", out tData.dTotalTime); //tTime.Val = tData.dTotalTime; _sgLotList->Cells[8][iLotCnt + 1] = tTime.FormatString("hh:nn:ss");
                    IniLoadLotList.Load("Data", "sJobFile  ", out tData.sJobFile  ); //_sgLotList->Cells[9][iLotCnt + 1] = tData.sJobFile;
                    IniLoadLotList.Load("Data", "dErrTime  ", out tData.dErrTime  ); //디스플레이 안함.
                    IniLoadLotList.Load("Data", "dStopTime ", out tData.dStopTime ); //디스플레이 안함.
                }

                for (int i = 0; i < FileCount; i++)
                {
                    lvLotInfo.Items[i].SubItems[1].Text = tData.sLotId;
                    lvLotInfo.Items[i].SubItems[2].Text = tData.iWorkCnt.ToString();
                    sTemp                               = string.Format("%0.3f", tData.dUPEH);
                    lvLotInfo.Items[i].SubItems[3].Text = sTemp;
                    sTemp                               = string.Format("%0.3f", tData.dUPH);
                    lvLotInfo.Items[i].SubItems[4].Text = sTemp;
                    lvLotInfo.Items[i].SubItems[5].Text = tData.dSttTime.ToString();
                    lvLotInfo.Items[i].SubItems[6].Text = tData.dEndTime.ToString();
                    lvLotInfo.Items[i].SubItems[7].Text = tData.dWorkTime.ToString();
                    lvLotInfo.Items[i].SubItems[8].Text = tData.dTotalTime.ToString();
                    lvLotInfo.Items[i].SubItems[9].Text = tData.sJobFile;

                } 
            }
         
            
            
            DateTime tCmprTime1;
            DateTime tCmprTime2;

            for (int i = 1; i < lvLotInfo.Items.Count; i++) 
            {
                if (lvLotInfo.Items[9].SubItems[i] == null) continue;//null일때 문제 없는지 확인해야함 진섭

                for (int j = i + 1; j < lvLotInfo.Items.Count; j++)
                {
                    if (lvLotInfo.Items[9].SubItems[j] == null) continue;
                    tCmprTime1 = Convert.ToDateTime(lvLotInfo.Items[9].SubItems[i]);
                    tCmprTime2 = Convert.ToDateTime(lvLotInfo.Items[9].SubItems[j]);
                    tCmprTime1 = Convert.ToDateTime(string.Format("fffff.ffffffffff"));
                    tCmprTime2 = Convert.ToDateTime(string.Format("fffff.ffffffffff"));
                    if (tCmprTime1 > tCmprTime2)
                    {
                        for (int x = 1; x < lvLotInfo.Columns.Count; x++)
                        {
                            lvLotInfo.Items[x].SubItems[lvLotInfo.Items.Count] = lvLotInfo.Items[x].SubItems[i];
                            lvLotInfo.Items[x].SubItems[i]                     = lvLotInfo.Items[x].SubItems[j];
                            lvLotInfo.Items[x].SubItems[j]                     = lvLotInfo.Items[x].SubItems[lvLotInfo.Items.Count];
                        }
                    }
                }
            }
        }

        public void DispDateList()
        {
            lvLotDate.Clear();
            lvLotDate.View               = View.Details;
            lvLotDate.LabelEdit          = true;
            lvLotDate.AllowColumnReorder = true;
            lvLotDate.FullRowSelect      = true;
            lvLotDate.GridLines          = true;
            lvLotDate.Sorting            = SortOrder.Ascending;

            lvLotDate.Columns.Add("No"       , 40 , HorizontalAlignment.Left);
            lvLotDate.Columns.Add("File Name", 200, HorizontalAlignment.Left);
        
            //Init. Grid Data.
            for (int i = 0 ;i < lvLotDate.Items.Count ; i++) lvLotDate.Items.Clear();
       
            //ListView Display
            DirectoryInfo Info = new DirectoryInfo(LOT_FOLDER);
            if (Info.Exists)
            {
                DirectoryInfo[] Dir = Info.GetDirectories();

                int FolderCount = Info.GetDirectories().Length;

                int iNo = 1;

                ListViewItem[] liLotDate = new ListViewItem[FolderCount];

                for (int i = FolderCount - 1 ; i > 0; i--)
                {
                    liLotDate[i] = new ListViewItem(string.Format("{0:00}", iNo));
                    liLotDate[i].SubItems.Add("");

                    lvLotDate.Items.Add(liLotDate[i]);
                    
                    iNo++;

                    int iItemNo = FolderCount - 1 - i; //다시 뒤집는다. 
                    lvLotDate.Items[iItemNo].SubItems[1].Text = Dir[i].Name.ToString();
                }
            }


        }



        //public void DispWorkList(String _sFilePath , ListView _lvWorkList)
        //{
        //    DispData.LoadFromCsv(WRK_FOLDER + _sFilePath);
        //    _lvWorkList->ColCount = DispData.GetMaxCol();
        //    _lvWorkList->RowCount = DispData.GetMaxRow();
        //    _lvWorkList->ColWidths[0] = 30;
        //    for (int c = 0; c < _lvWorkList->ColCount; c++)
        //    {
        //        for (int r = 0; r < _lvWorkList->RowCount; r++)
        //        {
        //            _lvWorkList->Cells[c][r] = DispData.GetCell(c, r);
        //        }
        //    }
        //
        //    //ColWidth
        //    int iTextCnt ;
        //    const int iTextPxRatio = 12 ;
        //    for (int c = 0; c < _lvWorkList->ColCount; c++)
        //    {
        //        iTextCnt = 0 ;
        //        for (int r = 0; r < _lvWorkList->RowCount; r++)
        //        {
        //            if (iTextCnt < _lvWorkList->Cells[c][r].Length()) iTextCnt = _lvWorkList.Items[r].SubItems[c].Length();
        //        }
        //        _lvWorkList->ColWidths[c] = iTextCnt * iTextPxRatio;
        //    }
        //}

        public void DispMgzAndSlotList(string _sDate , string _sLotId , ListView _lvMgz , ListView _lvSlot )
        {
            int iMgzCnt ;
            int iSlotCnt ;

            string sPath = LOT_FOLDER + _sDate + "\\" + _sLotId + ".ini";

            DirectoryInfo Info = new DirectoryInfo(sPath);
            int FileCount = Info.GetFiles().Length;
        
        
            
        
            //작업 메거진 카운트 갱신.
            CIniFile IniLoadMgz = new CIniFile(sPath);
        
            IniLoadMgz.Load("ETC", "MgzCnt" , out iMgzCnt );
            IniLoadMgz.Load("ETC", "SlotCnt", out iSlotCnt);

            iMgzCnt  = _lvMgz .Items.Count;
            iSlotCnt = _lvSlot.Items.Count;
        
            for(int m = 0 ; m < iMgzCnt ; m++)
            {
                _lvMgz.Items[m].SubItems[0].Text = m.ToString() ;
                _lvMgz.Items[m].SubItems[1].Text = m.ToString() ;
            }
        
            for(int s = 0 ; s < iSlotCnt ; s++)
            {
                _lvSlot.Items[s].SubItems[0].Text = s.ToString() ;
                _lvSlot.Items[s].SubItems[1].Text = s.ToString() ;
            }
        
        }

        private void lvLotDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sDate = lvLotDate.FocusedItem.SubItems[1].Text;
            
            DispLotList(sDate, lvLotInfo);
        }


        public void DispStripMap(string _sDate , string _sLotId , int _iMgz , int _iSlot , ListView _lvMap)
        {
            string sPath  ;
        
            int iColCnt ;
            int iRowCnt ;
        
        
            sPath = LOT_FOLDER + _sDate + "//" + _sLotId + ".ini" ;

            CIniFile IniLoadMap = new CIniFile(sPath);
            //작업 메거진 카운트 갱신.
            IniLoadMap.Load("ETC", "ColCnt", out iColCnt);
            IniLoadMap.Load("ETC", "RowCnt", out iRowCnt);

            iRowCnt = _lvMap.Items.Count - 1;
            iColCnt = _lvMap.Columns.Count-1  ;

            //_lvMap.FixedCols = 1;
            //_lvMap.FixedRows = 1;
        
        
            //맵 저장.
            string sRowData ;
            string sCaption ;
            string sIndex   ;
            string sData    ;
            sCaption = _iMgz.ToString() + "_" + _iSlot ;
            for(int r = 0 ; r < iRowCnt ; r++) {
                _lvMap.Items[r+1].SubItems[0].Text = r.ToString() ;
                sIndex = string.Format("Row{0:00}", r);
                IniLoadMap.Load(sCaption, sIndex, out sRowData);
                for(int c = 0 ; c < iColCnt ; c++) {
                    if (r == 0) _lvMap.Items[0].SubItems[c + 1].Text = c.ToString();
                    sData = sRowData.Substring(1,2) ;
                    //sRowData.Delete(1,3) ;
                    _lvMap.Items[r + 1].SubItems[c + 1].Text = sData;
                }
        
            }
        }

        private void FormSPC_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }
        
        
        
        
        
        


    }
}
