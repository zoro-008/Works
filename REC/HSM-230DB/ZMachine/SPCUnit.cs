using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection;
using COMMON;
using SML2;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

using System.Globalization;

//using System.Runtime.InteropServices;
//using SML.CXmlBase;
//using SMLDefine;
//using SMLApp;

namespace Machine
{
    //const  EN_ARAY_ID DATA_ARAY = riSTG ;

    public class ErrData
    {
        public struct TData
        {
            public int    iErrNo   ;
            public string sErrName ;
            public double dSttTime ;
            public string sErrMsg  ;
            public string sLotId   ;
        };

        public static int    m_iLastErr    ;
        public static string m_sLastErrMsg ;
        public TData         m_tErrData    ;
        

        public string ERR_FOLDER = "d:\\ErrLog\\" + OM.EqpOptn.sModelName + "\\";   //ERR_FOLDER

        public void Init()
        {
            LoadSaveLastErr(true);
        }

        public void Close()
        {
            LoadSaveLastErr(false);
        }

        public void LoadSaveLastErr(bool _bLoad)
        {
            //Set Dir.
            //ERR_FOLDER    ;
            string sPath = ERR_FOLDER + "LastErr.ini";
        
            if(_bLoad) {
                CIniFile IniLastErrInfo = new CIniFile(sPath);

                IniLastErrInfo.Load("LastInfo" , "m_iLastErr"    , out m_iLastErr   );
                IniLastErrInfo.Load("LastInfo" , "m_sLastErrMsg" , out m_sLastErrMsg);
            }
            else {
                CIniFile IniLastErrInfo = new CIniFile(sPath);

                //IniLastErrInfo.ClearFile(sPath) ;
                IniLastErrInfo.Save("LastInfo" , "m_iLastErr"    , m_iLastErr   );
                IniLastErrInfo.Save("LastInfo" , "m_sLastErrMsg" , m_sLastErrMsg);
            }
        }

        public void SaveErrIni (TData _tData)
        {
            string sPath  ;
        
            string sCaption ;
            DateTime  tErrDateTime ;

            tErrDateTime = DateTime.FromOADate(_tData.dSttTime);
        
            sPath = ERR_FOLDER + DateTime.Now.ToString("yyyyMMdd") + ".ini" ;
        
            int iCnt ;
            CIniFile IniSaveErr = new CIniFile(sPath);

            IniSaveErr.Load("ETC", "ErrCnt", out iCnt);
        
            sCaption = iCnt.ToString() ;

            IniSaveErr.Save(sCaption, "iErrNo  ", _tData.iErrNo);
            IniSaveErr.Save(sCaption, "sErrName", _tData.sErrName);
            IniSaveErr.Save(sCaption, "dSttTime", _tData.dSttTime);
            IniSaveErr.Save(sCaption, "sErrMsg ", _tData.sErrMsg);
            IniSaveErr.Save(sCaption, "sLotId  ", _tData.sLotId);
        
            iCnt++;
            IniSaveErr.Save("ETC", "ErrCnt", iCnt);
        }

        

        public int GetErrCnt  (DateTime _tSttData , DateTime _tEndData)
        {
            string sPath  ;
        
            int iCnt = 0 ;
            int iCntSum = 0 ;

            string SearchDate;

            for (DateTime d = _tSttData; d <= _tEndData; d = d.AddDays(1))
            {
                SearchDate = d.ToString();
                sPath = ERR_FOLDER + d.ToString("yyyyMMdd") + ".ini";

                CIniFile IniGetErrCnt = new CIniFile(sPath);

                IniGetErrCnt.Load("ETC", "ErrCnt", out iCnt);
                iCntSum += iCnt;
            }
            return iCntSum;
        }

        public bool LoadErrIni(DateTime _tSttData , DateTime _tEndData , TData[] _tData)
        {
            string sPath  ;
        
            string sCaption ;
            int iErrCnt = 0 ;
            int iMaxErrDayCnt = 0 ;

            string sTemp;
            for (DateTime d = _tSttData; d <= _tEndData; d = d.AddDays(1))
            {
                DateTime SearchDate = d;
                sTemp = d.ToString();
                sPath = ERR_FOLDER + d.ToString("yyyyMMdd") + ".ini";
                iMaxErrDayCnt = GetErrCnt(SearchDate, SearchDate);
                for (int c = 0; c < iMaxErrDayCnt; c++)
                {
                    CIniFile IniLoadErr = new CIniFile(sPath);
                    sCaption = c.ToString();

                    IniLoadErr.Load(sCaption, "iErrNo  ", out _tData[iErrCnt].iErrNo);
                    IniLoadErr.Load(sCaption, "sErrName", out _tData[iErrCnt].sErrName);
                    IniLoadErr.Load(sCaption, "dSttTime", out _tData[iErrCnt].dSttTime);
                    IniLoadErr.Load(sCaption, "sErrMsg ", out _tData[iErrCnt].sErrMsg);
                    IniLoadErr.Load(sCaption, "sLotId  ", out _tData[iErrCnt].sLotId);
                    iErrCnt++;
                }
            }
            return true;
        }

        public void SortErrData(bool _bNumberTime , int _iDataCnt , TData[] _tData)
        {
            TData _tTempData ;
        
            if(_bNumberTime) { //숫자 우선.
                for(int i = 0; i < _iDataCnt; i++) { //버블버블 버블팝 버블버블 팝팝!!
                   for(int j = 0; j < _iDataCnt -i - 1; j++) {
                      if(_tData[j].iErrNo > _tData[j + 1].iErrNo) {
                         _tTempData = _tData[j];
                         _tData[j] = _tData [j + 1];
                         _tData[j + 1] = _tTempData;
                      }
                   }
                }
            }
            else { //시간 우선.
                for(int i = 0; i < _iDataCnt; i++) { //버블버블 버블팝 버블버블 팝팝!!
                   for(int j = 0; j < _iDataCnt -i - 1; j++) {
                      if(_tData[j].dSttTime > _tData[j + 1].dSttTime) {
                         _tTempData = _tData[j];
                         _tData[j] = _tData [j + 1];
                         _tData[j + 1] = _tTempData;
                      }
                   }
                }
            }
        }
        
        public bool SetErr(int _iErrNo , string _sErrName , string _sErrMsg , string _sLotId)
        {
            //최근에 뜬 에러일시 팅겨냄...
            if(m_iLastErr == _iErrNo && m_sLastErrMsg == _sErrMsg) return false;
            
            //기존에 있던것들 지우기.
            DirectoryInfo di = new DirectoryInfo(ERR_FOLDER);        
            if(!di.Exists) di.Create();            
            foreach (FileInfo fi in di.GetFiles())
            {
                //if (fi.Extension != ".log") continue;

                // 12개월 이전 로그를 삭제합니다.
                if (fi.CreationTime <= DateTime.Now.AddMonths(-12))
                {
                    fi.Delete();
                }
            }
        
            m_tErrData.iErrNo   = _iErrNo   ;
            m_tErrData.sErrName = _sErrName ;
            m_tErrData.dSttTime = DateTime.Now.ToOADate();
            m_tErrData.sErrMsg  = _sErrMsg  ;
            m_tErrData.sLotId   = _sLotId   ;
        
            SaveErrIni(m_tErrData);
        
            m_iLastErr    = _iErrNo  ;
            m_sLastErrMsg = _sErrMsg ;
        
            return true;
        }
        
        //여기는 FormSPC 완료 되면 할꺼
        public void DispErrData (DateTime _tSttData , DateTime _tEndData , ListView _lvErrDate , bool _bNumberTime) //true면 에러넘버 , false면 시간.
        {
            if(_lvErrDate == null) return ;
            int iErrCnt = GetErrCnt(_tSttData , _tEndData) ;
            TData[] Datas = new TData[iErrCnt];
            LoadErrIni(_tSttData , _tEndData , Datas);
            
            SortErrData(_bNumberTime , iErrCnt , Datas) ;

            string sPath = ERR_FOLDER;//+ OM.EqpOptn.sModelName;

            string sSerchFile = sPath + "\\*.ini";

            _lvErrDate.Clear();
            _lvErrDate.View               = View.Details;
            _lvErrDate.LabelEdit          = true;
            _lvErrDate.AllowColumnReorder = true;
            _lvErrDate.FullRowSelect      = true;
            _lvErrDate.GridLines          = true;
            _lvErrDate.Sorting            = SortOrder.Descending;
            _lvErrDate.Scrollable         = true;

            _lvErrDate.Columns.Add("No"      , 40 , HorizontalAlignment.Left);
            _lvErrDate.Columns.Add("Err No"  , 100, HorizontalAlignment.Left);
            _lvErrDate.Columns.Add("Err Name", 210, HorizontalAlignment.Left);
            _lvErrDate.Columns.Add("Time"    , 210, HorizontalAlignment.Left);
            _lvErrDate.Columns.Add("Err Msg" , 210, HorizontalAlignment.Left);
            _lvErrDate.Columns.Add("Lot Id"  , 210, HorizontalAlignment.Left);

            //LotCount확인
            _lvErrDate.Items.Clear();

            DirectoryInfo Info = new DirectoryInfo(sPath);
            int FileCount = Info.GetFiles().Length;

            ListViewItem[] liitem = new ListViewItem[iErrCnt];

            for (int i = 0; i < iErrCnt; i++)
            {
                liitem[i] = new ListViewItem(string.Format("{0}", iErrCnt));
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

                _lvErrDate.Items.Add(liitem[i]);
            }

            DateTime Time;

            for (int r = 0; r < iErrCnt; r++)
            {
                _lvErrDate.Items[r].SubItems[0].Text = r.ToString();
                _lvErrDate.Items[r].SubItems[1].Text = Datas[r].iErrNo.ToString();
                _lvErrDate.Items[r].SubItems[2].Text = Datas[r].sErrName;
                Time = DateTime.FromOADate(Datas[r].dSttTime);
                //Time = Convert.ToDateTime(Datas[r].dSttTime);
                _lvErrDate.Items[r].SubItems[3].Text = Time.ToString("yyyy-MM-dd hh:mm:ss");
                _lvErrDate.Items[r].SubItems[4].Text = Datas[r].sErrMsg;
                _lvErrDate.Items[r].SubItems[5].Text = Datas[r].sLotId;
            }

            //delete[] Datas;

        }
        
        public void Update(string _sCrntLotNo)
        {
            //Err Log
            bool bPreErr = false ;
            bool isErr = SML.ER.IsErr();
            if(isErr && !bPreErr) {
                SetErr(SML.ER.GetLastErr(), SML.ER.GetErrName(SML.ER.GetLastErr()), SML.ER.GetErrMsg(SML.ER.GetLastErr()), _sCrntLotNo);
            }
            bPreErr = isErr ;
        }
    }













    public class WrkData
    {
        public struct TData
        {
            public string LotNo     ;
            public string Device    ;
            public double StartTime ;
            public double EndTime   ;
            public double LeftGapX  ;
            public double LeftGapY  ;
            public double RghtGapX  ;
            public double RghtGapY  ;
            public string SubHeight ;
            public string EndHeight ;
            public string GapHeight ;
            public string BltHeight ;
            public string SubBarcode; //Ver 1.0.5.0 2018.08.09 서브스트레이트 바코드
            public string WfrBarcode; //Ver 1.0.5.0 2018.08.09 웨이퍼 CMOS 바코드+넘버
        };
        public TData Data    ;
        public void DataClear()
        {
            Data.LotNo     = "" ;
            Data.Device    = "" ;
            Data.StartTime = 0.0;
            Data.EndTime   = 0.0;
            Data.LeftGapX  = 0.0;
            Data.LeftGapY  = 0.0;
            Data.RghtGapX  = 0.0;
            Data.RghtGapY  = 0.0;
            Data.SubHeight = "" ;
            Data.EndHeight = "" ;
            Data.GapHeight = "" ;
            Data.BltHeight = "" ;
            Data.SubBarcode  = "" ;
            Data.WfrBarcode  = "" ;
        }

        
        

        public string WRK_FOLDER = "d:\\WrkLog\\" + OM.EqpOptn.sModelName + "\\";   //ERR_FOLDER

        public void Init()
        {
            LoadSaveLastData(true);
        }

        public void Close()
        {
            LoadSaveLastData(false);
        }
        public void LoadSaveLastData(bool _bLoad)
        {
            //Set Dir.
            //ERR_FOLDER    ;
            string sPath = WRK_FOLDER + "LastWrkData.ini";
            if(_bLoad) CAutoIniFile.LoadStruct<TData>(sPath,"Data",ref Data);   
            else       CAutoIniFile.SaveStruct<TData>(sPath,"Data",ref Data);   
        }

        public void SaveDataIni ()
        {
            string sPath  ;
        
            DateTime  tDateTime ;

            tDateTime = DateTime.FromOADate(Data.StartTime);
        
            sPath = WRK_FOLDER + tDateTime.ToString("yyyyMMdd") + ".ini" ;

            //기존에 있던것들 지우기.
            DirectoryInfo di = new DirectoryInfo(WRK_FOLDER);
            if (!di.Exists) di.Create();
            foreach (FileInfo fi in di.GetFiles())
            {
                //if (fi.Extension != ".log") continue;
                // 12개월 이전 로그를 삭제합니다.
                if (fi.CreationTime <= DateTime.Now.AddMonths(-12))                {
                    fi.Delete();
                }
            }

            //카운트 저장.
            int iCnt = 0 ;
            CIniFile IniGetErrCnt = new CIniFile(sPath);
            IniGetErrCnt.Load("ETC", "DataCnt", out iCnt);
            int iAddedCnt=iCnt+1;
            IniGetErrCnt.Save("ETC", "DataCnt", iAddedCnt);

            //데이터 저장.
            CAutoIniFile.SaveStruct<TData>(sPath , iCnt.ToString(),ref Data);   
        }        

        public int GetDataCnt  (DateTime _tSttData , DateTime _tEndData)
        {
            string sPath  ;
        
            int iCnt = 0 ;
            int iCntSum = 0 ;

            string SearchDate;

            for (DateTime d = _tSttData; d <= _tEndData; d = d.AddDays(1))
            {
                SearchDate = d.ToString();
                sPath = WRK_FOLDER + d.ToString("yyyyMMdd") + ".ini";

                CIniFile IniGetCnt = new CIniFile(sPath);

                IniGetCnt.Load("ETC", "DataCnt", out iCnt);
                iCntSum += iCnt;
            }
            return iCntSum;
        }

        public bool LoadDataIni(DateTime _tSttData , DateTime _tEndData , TData[] _tData)
        {
            string sPath  ;

            int iDataCnt = 0 ;
            int iMaxDayCnt = 0 ;

            string sTemp;
            for (DateTime d = _tSttData; d <= _tEndData; d = d.AddDays(1))
            {
                DateTime SearchDate = d;
                sTemp = d.ToString();
                sPath = WRK_FOLDER + d.ToString("yyyyMMdd") + ".ini";
                //sPath = "D:\WrkLog\HSM_230DB\20170726.ini";
                iMaxDayCnt = GetDataCnt(SearchDate, SearchDate);
                //TData Data = new TData() ;
                for (int c = 0; c < iMaxDayCnt; c++)
                {
                    CAutoIniFile.LoadStruct<TData>(sPath , c.ToString(),ref _tData[iDataCnt]);   
                    //CAutoIniFile.LoadStruct<TData>(sPath , c.ToString(),ref Data);   
                    iDataCnt++;
                }
            }
            return true;
        }

        //일단 안만듬.
        //public void SortErrData(bool _bNumberTime , int _iDataCnt , TData[] _tData)
        //{
        //    TData _tTempData ;
        
        //    if(_bNumberTime) { //숫자 우선.
        //        for(int i = 0; i < _iDataCnt; i++) { //버블버블 버블팝 버블버블 팝팝!!
        //           for(int j = 0; j < _iDataCnt -i - 1; j++) {
        //              if(_tData[j].iErrNo > _tData[j + 1].iErrNo) {
        //                 _tTempData = _tData[j];
        //                 _tData[j] = _tData [j + 1];
        //                 _tData[j + 1] = _tTempData;
        //              }
        //           }
        //        }
        //    }
        //    else { //시간 우선.
        //        for(int i = 0; i < _iDataCnt; i++) { //버블버블 버블팝 버블버블 팝팝!!
        //           for(int j = 0; j < _iDataCnt -i - 1; j++) {
        //              if(_tData[j].dSttTime > _tData[j + 1].dSttTime) {
        //                 _tTempData = _tData[j];
        //                 _tData[j] = _tData [j + 1];
        //                 _tData[j + 1] = _tTempData;
        //              }
        //           }
        //        }
        //    }
        //}
        
        //여기는 FormSPC 완료 되면 할꺼
        public void DispData (DateTime _tSttData , DateTime _tEndData , ListView _lvTable) //true면 에러넘버 , false면 시간.
        {
            if(_lvTable == null) return ;
            int iDatsCnt = GetDataCnt(_tSttData , _tEndData) ;
            TData[] Datas = new TData[iDatsCnt];
            LoadDataIni(_tSttData , _tEndData , Datas);
            
            //SortErrData(_bNumberTime , iErrCnt , Datas) ;

            string sPath = WRK_FOLDER;
            string sSerchFile = sPath + "\\*.ini";

            _lvTable.Clear();
            _lvTable.View               = View.Details;
            _lvTable.LabelEdit          = true;
            _lvTable.AllowColumnReorder = true;
            _lvTable.FullRowSelect      = true;
            _lvTable.GridLines          = true;
            _lvTable.Sorting            = SortOrder.Descending;
            _lvTable.Scrollable         = true;

            Type type = typeof(TData);
            int iCntOfItem = type.GetProperties().Length;
            FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
            
            //컬럼추가 하고 이름을 넣는다.
            _lvTable.Columns.Add("No" , 100 , HorizontalAlignment.Left);
            for(int c = 0 ; c < f.Length ; c++){
                _lvTable.Columns.Add(f[c].Name , 100 , HorizontalAlignment.Left);
            }
            

            _lvTable.Items.Clear();
            string sValue ="";
            double dValue =0.0;
            string sName  ="";
            DateTime dTime ;
            ListViewItem[] liitem = new ListViewItem[iDatsCnt];
            for (int r = 0; r < iDatsCnt; r++){
                liitem[r] = new ListViewItem(string.Format("{0}", r));
                for(int c = 0 ; c < f.Length ; c++){
                    sName = f[c].Name;
                    if(sName.IndexOf("Time") >= 0){
                        dValue = (double)f[c].GetValue(Datas[r]);
                        dTime = DateTime.FromOADate(dValue);
                        sValue = dTime.ToString("yyyy-MM-dd hh:mm:ss");
                    }
                    else {
                        sValue = f[c].GetValue(Datas[r]).ToString();
                    }
                    liitem[r].SubItems.Add(sValue);
                }
                liitem[r].UseItemStyleForSubItems = false;
                _lvTable.Items.Add(liitem[r]);
            }

            _lvTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

        }
        
        public void Update(string _sCrntLotNo)
        {
            //Err Log
            
        }
    }





























    public class LotData
    {
        public struct TData
        {
            public string sLotId;
            public int iWorkCnt;
            public int iGoodCnt;

            public double dSttTime;
            public double dEndTime;
            public double dWorkTime;
            public double dErrTime;
            public double dStopTime;
            public double dTotalTime;
            public double dUPEH;
            public double dUPH;
            public double dCycleTime;
            public string sJobFile;
            public void Clear()
            {
                sLotId     = "ID" ;
                iWorkCnt   = 0  ;
                iGoodCnt   = 0  ;

                dSttTime   = 0.0;
                dEndTime   = 0.0;
                dWorkTime  = 0.0;
                dErrTime   = 0.0;
                dStopTime  = 0.0;
                dTotalTime = 0.0;
                dUPEH      = 0  ;
                dUPH       = 0  ;
                dCycleTime  = 0.0;
                sJobFile   = "" ;
            }
        };

        public TData Data;
        public CCycleTimer m_tmTick;
        public double dTickTime;
        public string LOT_FOLDER = "d:\\LotLog\\" + OM.EqpOptn.sModelName + "\\";   //LOT_FOLDER
        public string DAY_FOLDER = "d:\\DayLog\\" + OM.EqpOptn.sModelName + "\\";   //DAY_FOLDER
        public string WRK_FOLDER = "d:\\WrkLog\\" + OM.EqpOptn.sModelName + "\\";   //WRK_FOLDER
        public int iNo = 1;
        

        public void Init()
        {
            m_tmTick = new CCycleTimer();
            dTickTime = 0;
            LoadSaveLotIni(true);
        }

        public void Close()
        {
            LoadSaveLotIni(false);
        }

        //public bool SaveArrayData(TData _tData, int _iMgzNo, int _iSlotNo , CArray _rData)
        //{
        //    string sPath;
        //
        //    DateTime tDateTime;
        //
        //    tDateTime = Convert.ToDateTime(_tData.dSttTime);
        //
        //    sPath = LOT_FOLDER + tDateTime.ToString("yyyymmdd") + _tData.sLotId + ".ini";
        //    
        //
        //    //기존에 있던것들 지우기.
        //    Log.DeleteLogFile(LOT_FOLDER); //90일.
        //
        //
        //    //작업 메거진 카운트 갱신.
        //    int iMgzCnt = 0;
        //
        //    CIniFile IniLoadMgz = new CIniFile(sPath);
        //    IniLoadMgz.Load("ETC", "MgzCnt", out iMgzCnt);
        //
        //    CIniFile IniSaveMgz = new CIniFile(sPath);
        //    if (_iMgzNo + 1 >= iMgzCnt) IniSaveMgz.Save("ETC", "MgzCnt", _iMgzNo + 1);
        //
        //    //맥스 슬랏 카운트 갱신.
        //    int iSlotCnt = 0;
        //
        //    CIniFile IniLoadSlot = new CIniFile(sPath);
        //    IniLoadSlot.Load("ETC", "SlotCnt", out iSlotCnt);
        //
        //    CIniFile IniSaveSlot = new CIniFile(sPath);
        //    if (_iSlotNo + 1 >= iSlotCnt) IniSaveSlot.Save("ETC", "SlotCnt", _iSlotNo + 1);
        //
        //    //IniSaveSlot.Save("ETC", "ColCnt", _rData.GetMaxCol());
        //    //IniSaveSlot.Save("ETC", "RowCnt", _rData.GetMaxRow());
        //
        //
        //    //랏 정보 저장.
        //    CIniFile IniSaveLotInfo = new CIniFile(sPath);
        //    IniSaveLotInfo.Save("Data", "sLotId    ", _tData.sLotId    );
        //    IniSaveLotInfo.Save("Data", "iWorkCnt  ", _tData.iWorkCnt  );
        //
        //    IniSaveLotInfo.Save("Data", "dSttTime  ", _tData.dSttTime  );
        //    IniSaveLotInfo.Save("Data", "dEndTime  ", _tData.dEndTime  );
        //    IniSaveLotInfo.Save("Data", "dWorkTime ", _tData.dWorkTime );
        //    IniSaveLotInfo.Save("Data", "dErrTime  ", _tData.dErrTime  );
        //    IniSaveLotInfo.Save("Data", "dStopTime ", _tData.dStopTime );
        //    IniSaveLotInfo.Save("Data", "dTotalTime", _tData.dTotalTime);
        //    IniSaveLotInfo.Save("Data", "dUPEH     ", _tData.dUPEH     );
        //    IniSaveLotInfo.Save("Data", "dUPH      ", _tData.dUPH      );
        //    IniSaveLotInfo.Save("Data", "sJobFile  ", _tData.sJobFile  );
        //
        //
        //    //맵 저장.
        //    string sRowData ;
        //    string sCaption ;
        //    string sIndex   ;
        //    string sTemp    ;
        //    sCaption = _iMgzNo.ToString() + "_" + _iSlotNo;
        //    for (int r = 0; r < _rData.GetMaxRow(); r++)
        //    {
        //        CIniFile IniSaveMap = new CIniFile(sPath);
        //        sRowData = "";
        //        sIndex = string.Format("Row{0:00}", r);// sIndex. ("Row{0:00}", r);
        //        for (int c = 0; c < _rData.GetMaxCol(); c++)
        //        {
        //            //sTemp.sprintf("r=%02d,c=%02d", r,c);
        //            //Trace("Aray", sTemp.c_str());
        //            sTemp = string.Format("{0:00}", (int)_rData.GetStat(r, c));
        //            if (c < _rData.GetMaxCol() - 1) sTemp += "_";
        //            sRowData += sTemp;
        //        }
        //        IniSaveMap.Save(sCaption, sIndex, sRowData);
        //    }
        //}

        public void LoadSaveLotIni(bool _bLoad)
        {
       
           
            string sPath  ;

            sPath = LOT_FOLDER + LOT.GetLotNo();// +"\\LotInfo.ini";

            DirectoryInfo di = new DirectoryInfo(sPath);

            if (di.Exists == false) di.Create();

            sPath += "\\LotInfo.ini"; 
        
            if(_bLoad) 
            {
                CIniFile IniLoadLotInfo = new CIniFile(sPath);
              
                //if (m_tData.sLotId == "") m_tData.sLotId = DateTime.Now.ToString("HHmmss");
                IniLoadLotInfo.Load("Data", "sLotId    ", out Data.sLotId    );
                IniLoadLotInfo.Load("Data", "iWorkCnt  ", out Data.iWorkCnt  );
                IniLoadLotInfo.Load("Data", "iGoodCnt  ", out Data.iGoodCnt  );
                                                           
                IniLoadLotInfo.Load("Data", "dSttTime  ", out Data.dSttTime  );
                IniLoadLotInfo.Load("Data", "dEndTime  ", out Data.dEndTime  );
                IniLoadLotInfo.Load("Data", "dWorkTime ", out Data.dWorkTime );
                IniLoadLotInfo.Load("Data", "dErrTime  ", out Data.dErrTime  );
                IniLoadLotInfo.Load("Data", "dStopTime ", out Data.dStopTime );
                IniLoadLotInfo.Load("Data", "dTotalTime", out Data.dTotalTime);
                IniLoadLotInfo.Load("Data", "dUPEH     ", out Data.dUPEH     );
                IniLoadLotInfo.Load("Data", "dUPH      ", out Data.dUPH      );
                IniLoadLotInfo.Load("Data", "sJobFile  ", out Data.sJobFile  );
            }                                                                   
            else                                                                
            {                                                                   
                CIniFile IniSaveLotInfo = new CIniFile(sPath);

                //if (m_tData.sLotId == "") m_tData.sLotId = DateTime.Now.ToString("HHmmss");
                IniSaveLotInfo.Save("Data", "sLotId    ", Data.sLotId        );
                IniSaveLotInfo.Save("Data", "iWorkCnt  ", Data.iWorkCnt      );
                IniSaveLotInfo.Save("Data", "iGoodCnt  ", Data.iGoodCnt      );
                IniSaveLotInfo.Save("Data", "dSttTime  ", Data.dSttTime      );
                IniSaveLotInfo.Save("Data", "dEndTime  ", Data.dEndTime      );
                IniSaveLotInfo.Save("Data", "dWorkTime ", Data.dWorkTime     );
                IniSaveLotInfo.Save("Data", "dErrTime  ", Data.dErrTime      );
                IniSaveLotInfo.Save("Data", "dStopTime ", Data.dStopTime     );
                IniSaveLotInfo.Save("Data", "dTotalTime", Data.dTotalTime    );
                IniSaveLotInfo.Save("Data", "dUPEH     ", Data.dUPEH         );
                IniSaveLotInfo.Save("Data", "dUPH      ", Data.dUPH          );
                IniSaveLotInfo.Save("Data", "sJobFile  ", Data.sJobFile      );
            }
        
        }

        public void ClearData()
        {
            Data.Clear();
            //memset(&m_tData , 0 , sizeof(TData)); 이방식으로 하면 String 에서 메모리 누수 생김.
        }

        public void AddGoodCntData(int _iGoodCnt)
        {
            Data.iGoodCnt += _iGoodCnt;
        }

        public void AddWorkCntData(int _iWorkCnt)
        {
            Data.iWorkCnt += _iWorkCnt;
        }
        public void DispLotList(string _sDate, ListView _lvLotInfo)
        {
            //TSearchRec sr          ;
            string sPath = LOT_FOLDER + _sDate;

            string sSerchFile = sPath + "\\*.ini";


            DateTime tTime;

            Data.Clear();

            _lvLotInfo.Clear();
            _lvLotInfo.View = View.Details;
            _lvLotInfo.LabelEdit = true;
            _lvLotInfo.AllowColumnReorder = true;
            _lvLotInfo.FullRowSelect = true;
            _lvLotInfo.GridLines = true;
            _lvLotInfo.Sorting = SortOrder.Descending;
            _lvLotInfo.Scrollable = true;

            _lvLotInfo.Columns.Add("No"       , 40 , HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("LotId"    , 100, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("WorkCount", 210, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("GoodCount", 210, HorizontalAlignment.Left);
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
                liitem[i].SubItems.Add("");

                liitem[i].UseItemStyleForSubItems = false;
                liitem[i].UseItemStyleForSubItems = false;

                _lvLotInfo.Items.Add(liitem[i]);
                //iNo++;
            }



            //LotData
            string sTemp;
            if (Info.Exists)
            {
                Data.dUPEH = 0;
                Data.dUPH = 0;
                foreach (FileInfo info in Info.GetFiles())
                {
                    //sIniPath = sPath + "\\" + sr.Name;
                    CIniFile IniLoadLotList = new CIniFile(sPath + "\\LotInfo.ini");

                    //랏 정보 블러오기.

                    IniLoadLotList.Load("Data", "sLotId    ", out Data.sLotId    ); //_sgLotList->Cells[1][iLotCnt + 1] = tData.sLotId;
                    IniLoadLotList.Load("Data", "iWorkCnt  ", out Data.iWorkCnt  ); //_sgLotList->Cells[2][iLotCnt + 1] = tData.iWorkCnt;
                    IniLoadLotList.Load("Data", "iGoodCnt  ", out Data.iGoodCnt); //_sgLotList->Cells[2][iLotCnt + 1] = tData.iWorkCnt;
                                                                                    
                    IniLoadLotList.Load("Data", "dUPEH     ", out Data.dUPEH     ); //_sgLotList->Cells[3][iLotCnt + 1] = sTemp;
                                                                                    
                    IniLoadLotList.Load("Data", "dUPH      ", out Data.dUPH      ); //_sgLotList->Cells[4][iLotCnt + 1] = sTemp;
                    IniLoadLotList.Load("Data", "dSttTime  ", out Data.dSttTime  ); //tTime.Val = tData.dSttTime; _sgLotList->Cells[5][iLotCnt + 1] = tTime.FormatString("hh:nn:ss");
                    IniLoadLotList.Load("Data", "dEndTime  ", out Data.dEndTime  ); //tTime.Val = tData.dEndTime; _sgLotList->Cells[6][iLotCnt + 1] = tTime.FormatString("hh:nn:ss");
                    IniLoadLotList.Load("Data", "dWorkTime ", out Data.dWorkTime ); //tTime.Val = tData.dWorkTime; _sgLotList->Cells[7][iLotCnt + 1] = tTime.FormatString("hh:nn:ss");
                    IniLoadLotList.Load("Data", "dTotalTime", out Data.dTotalTime); //tTime.Val = tData.dTotalTime; _sgLotList->Cells[8][iLotCnt + 1] = tTime.FormatString("hh:nn:ss");
                    IniLoadLotList.Load("Data", "sJobFile  ", out Data.sJobFile  ); //_sgLotList->Cells[9][iLotCnt + 1] = tData.sJobFile;
                    IniLoadLotList.Load("Data", "dErrTime  ", out Data.dErrTime  ); //디스플레이 안함.
                    IniLoadLotList.Load("Data", "dStopTime ", out Data.dStopTime ); //디스플레이 안함.
                }

                for (int i = 0; i < FileCount; i++)
                {
                    _lvLotInfo.Items[i].SubItems[ 1].Text = Data.sLotId;
                    _lvLotInfo.Items[i].SubItems[ 2].Text = Data.iWorkCnt.ToString();
                    _lvLotInfo.Items[i].SubItems[ 3].Text = Data.iGoodCnt.ToString();
                    sTemp = string.Format("{0:000}", Data.dUPEH);
                    _lvLotInfo.Items[i].SubItems[ 4].Text = sTemp;
                    sTemp = string.Format("{0:000}", Data.dUPH);
                    _lvLotInfo.Items[i].SubItems[ 5].Text = sTemp;
                    _lvLotInfo.Items[i].SubItems[ 6].Text = Data.dSttTime.ToString();
                    _lvLotInfo.Items[i].SubItems[ 7].Text = Data.dEndTime.ToString();
                    _lvLotInfo.Items[i].SubItems[ 8].Text = Data.dWorkTime.ToString();
                    _lvLotInfo.Items[i].SubItems[ 9].Text = Data.dTotalTime.ToString();
                    _lvLotInfo.Items[i].SubItems[10].Text = Data.sJobFile;

                }
            }



            DateTime tCmprTime1;
            DateTime tCmprTime2;

            for (int i = 1; i < _lvLotInfo.Items.Count; i++)
            {
                if (_lvLotInfo.Items[9].SubItems[i] == null) continue;//null일때 문제 없는지 확인해야함 진섭

                for (int j = i + 1; j < _lvLotInfo.Items.Count; j++)
                {
                    if (_lvLotInfo.Items[9].SubItems[j] == null) continue;
                    tCmprTime1 = Convert.ToDateTime(_lvLotInfo.Items[10].SubItems[i]);
                    tCmprTime2 = Convert.ToDateTime(_lvLotInfo.Items[10].SubItems[j]);
                    tCmprTime1 = Convert.ToDateTime(string.Format("fffff.ffffffffff"));
                    tCmprTime2 = Convert.ToDateTime(string.Format("fffff.ffffffffff"));
                    if (tCmprTime1 > tCmprTime2)
                    {
                        for (int x = 1; x < _lvLotInfo.Columns.Count; x++)
                        {
                            _lvLotInfo.Items[x].SubItems[_lvLotInfo.Items.Count] = _lvLotInfo.Items[x].SubItems[i];
                            _lvLotInfo.Items[x].SubItems[i] = _lvLotInfo.Items[x].SubItems[j];
                            _lvLotInfo.Items[x].SubItems[j] = _lvLotInfo.Items[x].SubItems[_lvLotInfo.Items.Count];
                        }
                    }
                }
            }
        }

        //public void DispWorkList(String _sFilePath , ListView _lvWorkList)
        //{
        //    CCsvFile.ToCsvFields(WRK_FOLDER + _sFilePath);
        //    _lvWorkList.Columns.Count = DispData.GetMaxCol();
        //    _lvWorkList.RowCount = DispData.GetMaxRow();
        //    _lvWorkList.ColWidths[0] = 30;
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

        public void SaveWorkListCSV(string _sFilePath)
        {
            //20150823 레이언스 FOS때문에 추가...
            //자제 하나하나의 리스트를 보여 줘야 한다.
            //DispData.SaveToCsv(_sFilePath);
        }

        public void SaveWorkListXLS(string _sFilePath)
        {
            //20150823 레이언스 FOS때문에 추가...
            //자제 하나하나의 리스트를 보여 줘야 한다.
            //DispData.SaveToXls(_sFilePath);
        }

        public void Update(EN_SEQ_STAT Stat)
        {
            //타임 인포 갱신.
            //DateTime tDateTime = DateTime.Now;
            //
            ////string sDateTime = DateTime.Now.ToString();
            //
            //DateTime tPreTime = tDateTime;
            //DateTime tCrntTime  = DateTime.Now;
            //DateTime tCycleTime  = tCrntTime - tPreTime ;

            //시간 단위가 너무 커서 컴파일 에러 남 진섭
            double dStartTime = DateTime.Now.ToOADate();
            
            double dPreTime = dStartTime;
            double dCrntTime = DateTime.Now.ToOADate();
            double dCycleTime = dCrntTime - dPreTime ;


            switch(Stat) {
                case EN_SEQ_STAT.Init     : Data.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Warning  : Data.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Error    : Data.dErrTime  += dCycleTime ; break ;
                case EN_SEQ_STAT.Running  : Data.dWorkTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Stop     : Data.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Maint    : Data.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.RunWarn  : Data.dWorkTime += dCycleTime ; break ;
                case EN_SEQ_STAT.WorkEnd  : Data.dStopTime += dCycleTime ; break ;
            }
            Data.dTotalTime += dCycleTime ;
            //m_tData.dTickTime = STG.GetTickTime() / 1000;
            dPreTime = dCrntTime ;
        
            //잡파일 체인지 할때 어레이 동적 할당 다시 하기때문에 돌려줘야 한다.
            //이장비는 툴만 보기 때문에 상관 없지만 그냥 이렇게 내둔다.
            if(Stat == EN_SEQ_STAT.Stop ) return ;
            //if(Stat != ssRunning && Stat != ssRunWarn) return ;
            ///////////////////////매우 중요.............
        
            string sLotId ;
            sLotId = LOT.GetLotNo() ; //LDR까지 스캔해보고 있으면 미리 랏오픈을 한다.
            if(sLotId != Data.sLotId && sLotId != "") { //어맛! 새로운 랏.
                Data.Clear();
                //memset(&m_tData , 0 , sizeof(TData)); 메모리 누수.
        
                Data.sLotId    = LOT.GetLotNo() ;
                Data.dSttTime  = dCrntTime ;
            }

            //Array 완성되면 마저 해야함 진섭
            //bool bPreWorkEndAll = DM.ARAY[riSTG].CheckAllStat(csWorkEnd);
            //bool bWorkEndAll    = DM.ARAY[riSTG].CheckAllStat(csWorkEnd);
            //if(!bPreWorkEndAll && bWorkEndAll){
            //    m_tData.iWorkCnt += 1;
            //}
            //bPreWorkEndAll = bWorkEndAll;
        
            const int iSlotNo = 0 ;
            const int iMgzNo  = 0 ;
        
            Data.dEndTime  = dCrntTime ;
            Data.dUPEH     = Data.dTotalTime == 0 ? 0.0 : Data.iWorkCnt  / (Data.dTotalTime * 24) ;
            Data.dUPH      = Data.dWorkTime  == 0 ? 0.0 : Data.iWorkCnt  / (Data.dWorkTime  * 24) ;
            Data.sJobFile  = OM.GetCrntDev() ;
        
            //Array 완성되면 마저 해야함 진섭
            //bool bPreSTGExist = !DM.ARAY[riSTG].CheckAllStat(csNone);
            //bool bSTGExist    = !DM.ARAY[riSTG].CheckAllStat(csNone);
        
            //if(!bPreSTGExist && bSTGExist)
            //{
            //    m_tmTick.Clear();
            //}
            //if(bSTGExist){
            //    m_tData.dTickTime = m_tmTick.OnCheckCycle() / 1000;
            //}
            //bPreSTGExist = bSTGExist ;
        }

        public void DispLotInfo(ListView _lvLotInfo) //오퍼레이션 창용.
        {
            DateTime tDateTime;

            string sTemp;
        
            //LotId
            _lvLotInfo.Items[0].SubItems[0].Text = "LotId";
            _lvLotInfo.Items[0].SubItems[1].Text = Data.sLotId;
        
            //WorkCnt
            _lvLotInfo.Items[1].SubItems[0].Text = "WorkCnt  ";
            _lvLotInfo.Items[1].SubItems[1].Text = Data.iWorkCnt.ToString();

            //WorkCnt
            _lvLotInfo.Items[2].SubItems[0].Text = "GoodCnt  ";
            _lvLotInfo.Items[2].SubItems[1].Text = Data.iGoodCnt.ToString();
        
            //UPH
            sTemp = string.Format("{0:0.0}", Data.dUPH);
            _lvLotInfo.Items[3].SubItems[0].Text = "UPH      ";
            _lvLotInfo.Items[3].SubItems[1].Text = sTemp;
        
            //SttTime
            _lvLotInfo.Items[4].SubItems[0].Text = "SttTime  ";
            tDateTime = DateTime.FromOADate(Data.dSttTime);
            _lvLotInfo.Items[4].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");

            //TickTime
            sTemp = string.Format("{0:0.00}", dTickTime);
            _lvLotInfo.Items[5].SubItems[0].Text = "TackTime ";
            _lvLotInfo.Items[5].SubItems[1].Text = sTemp;
            
        
            //EndTime
            //_lvLotInfo.Items[5].SubItems[0].Text = "EndTime  ";
            //tDateTime = DateTime.FromOADate(m_tData.dEndTime);
            //_lvLotInfo.Items[5].SubItems[1].Text = tDateTime.ToString("hh:mm:ss");
        
            //WorkTime
            //_lvLotInfo.Items[6].SubItems[0].Text = "WorkTime ";
            //tDateTime = DateTime.FromOADate(m_tData.dWorkTime);
            //_lvLotInfo.Items[6].SubItems[1].Text = tDateTime.ToString("hh:mm:ss");
        
            //TotalTime
            //_lvLotInfo.Items[7].SubItems[0].Text = "TotalTime";
            //tDateTime = DateTime.FromOADate(m_tData.dTotalTime);
            //_lvLotInfo.Items[7].SubItems[1].Text = tDateTime.ToString("hh:mm:ss");
        
            //TickTime
            //_lvLotInfo.Items[8].SubItems[0].Text = "CycleTime";
            //_lvLotInfo.Items[8].SubItems[1].Text = m_tData.dTickTime + "sec";
        
            //UPH
            //sTemp = string.Format("{0:0.0000}", Convert.ToBoolean(m_tData.dUPH) ? 1 / m_tData.dUPH : 0);  //bool형으로 Converting 되는지 확인 할 것
            //_lvLotInfo.Items[9].SubItems[0].Text = "HPU      ";
            //_lvLotInfo.Items[9].SubItems[1].Text = sTemp + " hour";
        }
        
        public void WriteLotInfo()
        {
            //칩 카운트 갱신.
            //int iSlotNo ;
            //int iMgzNo  ;
            //double dCrntTime = Now().Val ;
            //m_tData.iWorkCnt  += DM.ARAY[DATA_ARAY].GetMaxCol() * DM.ARAY[DATA_ARAY].GetMaxRow() - DM.ARAY[DATA_ARAY].GetCntStat(csEmpty); //Empty 빼준다...없는것이므로.JS
            //m_tData.dEndTime  = dCrntTime ;
            //m_tData.dUPEH     = m_tData.dTotalTime == 0 ? 0.0 : m_tData.iWorkCnt  / (m_tData.dTotalTime * 24) ;
            //m_tData.sJobFile  = OM.GetCrntDev() ;
            //
            //iSlotNo = DM.ARAY[DATA_ARAY].GetID().ToIntDef(0)%100 ;
            //iMgzNo  = DM.ARAY[DATA_ARAY].GetID().ToIntDef(0)/100 ;
            //SaveArrayData(m_tData , iMgzNo , iSlotNo , DM.ARAY[DATA_ARAY]);
        }
        
        public string GetCrntLotNo()
        {
        
        
        
        
            //if(DM.ARAY[riSRT].GetCntStat(csRslt1)
        
        
        //     return "";
            return LOT.GetLotNo();

        }
    }

    public class DayData
    {
        public struct TData
        {
            public  double dWorkTime;
            public  double dStopTime;
            public  double dErrTime;
            public  double dTotalTime;
            public  double dCycleTime;
            public  double dUPEH;
            public  double dUPH;
            public  int    iWorkCnt;
            public  int    iLotCnt;
            public double dPreTime;
            public void TimeClear()
            {
                dWorkTime  = 0.0;
                dStopTime  = 0.0;
                dErrTime   = 0.0;
                dTotalTime = 0.0;
            }
            public void UPHClear()
            {
                dUPEH      = 0.0;
                dUPH       = 0.0;
            }
            public void CntClear()
            {
                iWorkCnt   = 0  ;
                iLotCnt    = 0  ;
            }
        };

        public string LOT_FOLDER = "d:\\LotLog\\" + OM.EqpOptn.sModelName + "\\";   //LOT_FOLDER
        public string DAY_FOLDER = "d:\\DayLog\\" + OM.EqpOptn.sModelName + "\\";   //DAY_FOLDER

        public TData Data;

        public void Init()
        {
            Data.dPreTime = 0;
            LoadSaveDayIni(true , ref Data) ;
            if(Data.dPreTime == 0) Data.dPreTime = DateTime.Now.ToOADate();
        }
        
        public void Close()
        {
            LoadSaveDayIni(false , ref Data) ;
        }

        //static double dPreTime = DateTime.Now.ToOADate();
        static int iPreWorkCnt;
        public void Update(EN_SEQ_STAT Stat)
        {
            //string sLotId ;
            double dCrntTime = DateTime.Now.ToOADate();                    
            
            double dCycleTime = dCrntTime - Data.dPreTime ;
        
            //Time Info.
            switch(Stat) {
                case EN_SEQ_STAT.Init     : Data.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Warning  : Data.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Error    : Data.dErrTime  += dCycleTime ; break ;
                case EN_SEQ_STAT.Running  : Data.dWorkTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Stop     : Data.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Maint    : Data.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.RunWarn  : Data.dWorkTime += dCycleTime ; break ;
                case EN_SEQ_STAT.WorkEnd  : Data.dStopTime += dCycleTime ; break ;
            }
            Data.dTotalTime += dCycleTime ;
        
        
        
            //Day Log 랏 데이터는 저장 할때 랏오픈된 날짜에 저장 하지만 Day는 Lot하고 상관 없는 개념이라 그냥 바로바로 Day에 저장 한다.
            //Lot과 데이터 연동 하고 싶으면 랏업데이트에 있는 저장 패턴 복사해서 쓴다.
            bool bDateChanged = ((int)dCrntTime) != ((int)Data.dPreTime) ; //소수점 이하는 시간 데이터.
            if(bDateChanged) { //날자 바뀜.
                ClearData();
            }
            Data.dPreTime = dCrntTime ;
        
        
            //string sPreLotNo = _sCrntLotNo ;
            //if(sPreLotNo != _sCrntLotNo ) { //앗 랏이 바뀌었네!!!!
            //    m_tData.iLotCnt++;
            //}
            //sPreLotNo = _sCrntLotNo ;
        
        
        
            //m_tData.dErrTime = dCrntTime;
            //m_tData.dWorkTime = ;
            //m_tData.dStopTime;
            //m_tData.dTotalTime;
            //m_tData.dUPH;
            //m_tData.iWorkCnt;
            

            Data.dUPEH     = Data.dTotalTime == 0 ? 0.0 : Data.iWorkCnt / (Data.dTotalTime * 24) ;
            Data.dUPH      = Data.dWorkTime  == 0 ? 0.0 : Data.iWorkCnt / (Data.dWorkTime  * 24) ;
        
            //잡파일 체인지 할때 어레이 동적 할당 다시 하기때문에 돌려줘야 한다.
            //이장비는 툴만 보기 때문에 상관 없지만 그냥 이렇게 내둔다.
            if(Stat == EN_SEQ_STAT.Stop) return ;
            ///////////////////////매우 중요.............
            int iWorkCnt;
            if(SEQ._bRun){
                iWorkCnt = DM.ARAY[ri.SSTG].GetCntStat(cs.WorkEnd);
                if( iPreWorkCnt != iWorkCnt && iWorkCnt != 0)
                {
                        Data.iWorkCnt += 1;
                        iPreWorkCnt = iWorkCnt;
                }
            }



        }

        public void DispDayInfo(ListView _lvDayInfo) //오퍼레이션 창용.
        {
            DateTime tDateTime;

            string sTemp;
            _lvDayInfo.Items[0].SubItems[0].Text = "WorkTime ";
            tDateTime = DateTime.FromOADate(Data.dWorkTime);
            _lvDayInfo.Items[0].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");


            _lvDayInfo.Items[1].SubItems[0].Text = "StopTime ";
            tDateTime = DateTime.FromOADate(Data.dStopTime);
            _lvDayInfo.Items[1].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");


            _lvDayInfo.Items[2].SubItems[0].Text = "ErrTime  ";
            tDateTime = DateTime.FromOADate(Data.dErrTime);
            _lvDayInfo.Items[2].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");


            _lvDayInfo.Items[3].SubItems[0].Text = "TotalTime";
            tDateTime = DateTime.FromOADate(Data.dTotalTime);
            _lvDayInfo.Items[3].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");

            sTemp = string.Format("{0:0.0000}", Data.dUPH);
            _lvDayInfo.Items[4].SubItems[0].Text = "UPH";
            _lvDayInfo.Items[4].SubItems[1].Text = sTemp;


            _lvDayInfo.Items[5].SubItems[0].Text = "WorkCnt  ";
            _lvDayInfo.Items[5].SubItems[1].Text = Data.iWorkCnt.ToString();
        }
        
        public void WriteDayInfo()
        {
            DateTime tDateTime = DateTime.Now;
            //int iFailCnt = DataMan.ARAY[DATA_ARAY].GetCntStat  (csRslt1) +
            //               DM.ARAY[DATA_ARAY].GetCntStat  (csRslt2) +
            //               DM.ARAY[DATA_ARAY].GetCntStat  (csRslt3) +
            //               DM.ARAY[DATA_ARAY].GetCntStat  (csRslt4) +
            //               DM.ARAY[DATA_ARAY].GetCntStat  (csRslt5) +
            //               DM.ARAY[DATA_ARAY].GetCntStat  (csRslt6) +
            //               DM.ARAY[DATA_ARAY].GetCntStat  (csRslt7) +
            //               DM.ARAY[DATA_ARAY].GetCntStat  (csRslt8) ;
//            m_tData.iWorkCnt += DM.ARAY[(int)ri.SLD].GetMaxCol() * DM.ARAY[(int)ri.SLD].GetMaxRow();   //Array 만들어지면 해야함 진섭
            Data.dUPEH     = Data.dTotalTime == 0 ? 0.0 : Data.iWorkCnt / (Data.dTotalTime * 24) ;
            Data.dUPH      = Data.dWorkTime  == 0 ? 0.0 : Data.iWorkCnt / (Data.dWorkTime  * 24) ;
        
            SaveDayData(Data , tDateTime);
        
        }
        public void LoadSaveDayIni(bool _bLoad , ref TData _tData)
        {
            //Make Dir.
            string sPath = DAY_FOLDER + "DayInfo.ini";
        
            if(_bLoad) {
                CIniFile IniLoadDayInfo = new CIniFile(sPath);

                IniLoadDayInfo.Load("Data", "dWorkTime ", out _tData.dWorkTime );
                IniLoadDayInfo.Load("Data", "dStopTime ", out _tData.dStopTime );
                IniLoadDayInfo.Load("Data", "dErrTime  ", out _tData.dErrTime  );
                IniLoadDayInfo.Load("Data", "dTotalTime", out _tData.dTotalTime);
                IniLoadDayInfo.Load("Data", "dUPEH     ", out _tData.dUPEH     );
                IniLoadDayInfo.Load("Data", "dUPH      ", out _tData.dUPH      );
                IniLoadDayInfo.Load("Data", "iWorkCnt  ", out _tData.iWorkCnt  );
                IniLoadDayInfo.Load("Data", "iLotCnt   ", out _tData.iLotCnt   );
                IniLoadDayInfo.Load("Data", "dPreTime  ", out _tData.dPreTime  ); //Ver1.0.5.0 전날 장비 끄고 가면 다음날 Day쪽 관련 시간들 초기화가 안되서 저장한다음에 날짜 다르면 초기화
            }
            else {
                CIniFile IniSaveDayInfo = new CIniFile(sPath);

                IniSaveDayInfo.Save("Data", "dWorkTime ", _tData.dWorkTime );
                IniSaveDayInfo.Save("Data", "dStopTime ", _tData.dStopTime );
                IniSaveDayInfo.Save("Data", "dErrTime  ", _tData.dErrTime  );
                IniSaveDayInfo.Save("Data", "dTotalTime", _tData.dTotalTime);
                IniSaveDayInfo.Save("Data", "dUPEH     ", _tData.dUPEH     );
                IniSaveDayInfo.Save("Data", "dUPH      ", _tData.dUPH      );
                IniSaveDayInfo.Save("Data", "iWorkCnt  ", _tData.iWorkCnt  );
                IniSaveDayInfo.Save("Data", "iLotCnt   ", _tData.iLotCnt   );
                IniSaveDayInfo.Save("Data", "dPreTime  ", _tData.dPreTime  ); //Ver1.0.5.0 전날 장비 끄고 가면 다음날 Day쪽 관련 시간들 초기화가 안되서 저장한다음에 날짜 다르면 초기화
            }                                                        
        
        
        }
        
        public bool SaveDayData(TData _tData , DateTime _tDateTime) //혹시 나중에 Lot하고 동기화 할까봐 TDateTime 인자 넣음.
        {
            //최근에 뜬 에러일시 팅겨냄...
            DirectoryInfo di = new DirectoryInfo(LOT_FOLDER);

            if (!di.Exists) di.Create();

            //기존에 있던것들 지우기.
            foreach (FileInfo fi in di.GetFiles())
            {
                //if (fi.Extension != ".log") continue;

                // 3개월 이전 로그를 삭제합니다.
                if (fi.CreationTime <= DateTime.Now.AddMonths(-3))
                {
                    fi.Delete();
                }
            }

            string sPath = LOT_FOLDER + _tDateTime.ToString("yyyyMMdd") + ".ini" ;

            CIniFile IniSaveDayDate = new CIniFile(sPath);

            IniSaveDayDate.Save("Data", "dWorkTime ", _tData.dWorkTime );
            IniSaveDayDate.Save("Data", "dStopTime ", _tData.dStopTime );
            IniSaveDayDate.Save("Data", "dErrTime  ", _tData.dErrTime  );
            IniSaveDayDate.Save("Data", "dTotalTime", _tData.dTotalTime);
            IniSaveDayDate.Save("Data", "dUPEH     ", _tData.dUPEH     );
            IniSaveDayDate.Save("Data", "dUPH      ", _tData.dUPH      );
            IniSaveDayDate.Save("Data", "iWorkCnt  ", _tData.iWorkCnt  );
            IniSaveDayDate.Save("Data", "iLotCnt   ", _tData.iLotCnt   );

            return true;
        }
        
        public void ClearData()
        {
            Data.TimeClear();
            Data.UPHClear ();
            Data.CntClear ();
            //memset(&m_tData , 0 , sizeof(TData));
            
        
        }
        
        public void DispDayList (ListView _lvDateList)
        {
            //SPC에 탭 추가 하고 구현 해야하는데.
            //일단 안씀.
        }
    }

    public static class SPC
    {
        public static ErrData ERR = new ErrData();
        public static WrkData WRK = new WrkData();
        public static LotData LOT = new LotData();
        public static DayData DAY = new DayData();

        public static void Init()
        {
            ERR.Init();
            WRK.Init();
            LOT.Init();
            DAY.Init();
        }
        
        public static void Close()
        {
            ERR.Close();
            WRK.Close();
            LOT.Close();
            DAY.Close();
        }
        
        public static void Update(EN_SEQ_STAT Stat)
        {
            //LOT = new CLotData() ;
            ERR.Update(LOT.GetCrntLotNo()     );
            WRK.Update(LOT.GetCrntLotNo() );


            LOT.Update(Stat);
            //if (LOT.GetCrntLotNo() != null)
            DAY.Update(Stat);
        }

    }
     
        
  







}
   