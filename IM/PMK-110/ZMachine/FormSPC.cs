using System;
using System.Windows.Forms;
using COMMON;
using System.IO;

namespace Machine
{
    public partial class FormSPC : Form
    {
        public static string LOT_FOLDER = "d:\\LotLog\\" + OM.EqpOptn.sModelName + "\\";   //LOT_FOLDER
        public static string WRK_FOLDER = "d:\\WrkLog\\" + OM.EqpOptn.sModelName + "\\";   //WRK_FOLDER

        //public SPC SPC = new SPC();
        //public LotData LOT = new LotData();
        //public ErrData ERR = new ErrData();
        //public DayData DAY = new DayData();
        
        

        //LotData.TData tData;

        public FormSPC(Panel _pnBase)
        {
            InitializeComponent();

            //string sDate = lvLotDate.Items[Convert.ToInt32(lvLotDate.Items)].SubItems[1].ToString() ;
            this.TopLevel = false;
            this.Parent = _pnBase;

            SPC.LOT.DispDateList(lvLotDate);

            //string sDate = .Items[0].SubItems[1].ToString();

            //LOT.DispLotList(sDate, lvLotInfo);

            
        }

        public void DispMgzAndSlotList(string _sDate , string _sLotId , ListView _lvMgz , ListView _lvSlot )
        {
            int iMgzCnt = 0 ;
            int iSlotCnt = 0 ;

            string sPath = LOT_FOLDER + _sDate + "\\" + _sLotId + ".ini";

            DirectoryInfo Info = new DirectoryInfo(sPath);
            int FileCount = Info.GetFiles().Length;
        
        
            
        
            //작업 메거진 카운트 갱신.
            CIniFile IniLoadMgz = new CIniFile(sPath);
        
            IniLoadMgz.Load("ETC", "MgzCnt" , ref iMgzCnt );
            IniLoadMgz.Load("ETC", "SlotCnt", ref iSlotCnt);

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

            SPC.LOT.DispLotList(sDate, lvLotInfo);
        }


        public void DispStripMap(string _sDate , string _sLotId , int _iMgz , int _iSlot , ListView _lvMap)
        {
            string sPath  ;
        
            int iColCnt = 0 ;
            int iRowCnt = 0 ;
        
        
            sPath = LOT_FOLDER + _sDate + "//" + _sLotId + ".ini" ;

            CIniFile IniLoadMap = new CIniFile(sPath);
            //작업 메거진 카운트 갱신.
            IniLoadMap.Load("ETC", "ColCnt", ref iColCnt);
            IniLoadMap.Load("ETC", "RowCnt", ref iRowCnt);

            iRowCnt = _lvMap.Items.Count - 1;
            iColCnt = _lvMap.Columns.Count-1  ;

            //_lvMap.FixedCols = 1;
            //_lvMap.FixedRows = 1;
        
        
            //맵 저장.
            string sRowData = "";
            string sCaption ;
            string sIndex   ;
            string sData    ;
            sCaption = _iMgz.ToString() + "_" + _iSlot ;
            for(int r = 0 ; r < iRowCnt ; r++) {
                _lvMap.Items[r+1].SubItems[0].Text = r.ToString() ;
                sIndex = string.Format("Row{0:00}", r);
                IniLoadMap.Load(sCaption, sIndex, ref sRowData);
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

        private void btErrDataView_Click(object sender, EventArgs e)
        {
            if (rbErrNo.Checked)
            {
                SPC.ERR.DispErrData(edErrSttTime.Value, edErrEndTime.Value, lvErrData, true);
            }
            else if(rbErrTime.Checked)
            {
                SPC.ERR.DispErrData(edErrSttTime.Value, edErrEndTime.Value, lvErrData, false);
            }

        }
        public void ShowUpdate()
        {
            SPC.LOT.DispDateList(lvLotDate);
            Refresh();
        }

        private void FormSPC_Shown(object sender, EventArgs e)
        {
            edErrSttTime.Value = DateTime.Now ;
            edErrEndTime.Value = DateTime.Now ;
        }
    }
}
