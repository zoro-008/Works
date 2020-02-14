using System;
using System.IO;
using System.Windows.Forms;
using COMMON;
using SML;

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

                IniLastErrInfo.Load("LastInfo" , "m_iLastErr"    , ref m_iLastErr   );
                IniLastErrInfo.Load("LastInfo" , "m_sLastErrMsg" , ref m_sLastErrMsg);
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
        
            int iCnt =0 ;
            CIniFile IniSaveErr = new CIniFile(sPath);

            IniSaveErr.Load("ETC", "ErrCnt", ref iCnt);
        
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

                IniGetErrCnt.Load("ETC", "ErrCnt", ref iCnt);
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

                    IniLoadErr.Load(sCaption, "iErrNo  ", ref _tData[iErrCnt].iErrNo  );
                    IniLoadErr.Load(sCaption, "sErrName", ref _tData[iErrCnt].sErrName);
                    IniLoadErr.Load(sCaption, "dSttTime", ref _tData[iErrCnt].dSttTime);
                    IniLoadErr.Load(sCaption, "sErrMsg ", ref _tData[iErrCnt].sErrMsg );
                    IniLoadErr.Load(sCaption, "sLotId  ", ref _tData[iErrCnt].sLotId  );
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
            DirectoryInfo di = new DirectoryInfo(ERR_FOLDER);

            if(m_iLastErr == _iErrNo && m_sLastErrMsg == _sErrMsg) return false;
        
            if(!di.Exists) di.Create();
        
            //기존에 있던것들 지우기.
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension != ".log") continue;

                // 3개월 이전 로그를 삭제합니다.
                if (fi.CreationTime <= DateTime.Now.AddMonths(-3))
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
            bool isErr = SM.ERR.IsErr();
            if(isErr && !bPreErr) {
                SetErr(SM.ERR.GetLastErr(), SM.ERR.GetErrName(SM.ERR.GetLastErr()), SM.ERR.GetErrMsg(SM.ERR.GetLastErr()), _sCrntLotNo);
            }
            bPreErr = isErr ;
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
            public double dTickTime;
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
                dTickTime  = 0.0;
                sJobFile   = "" ;
            }
        };

        public static TData m_tData;
        public CCycleTimer m_tmTick;
        public double dPreTime;

        public string LOT_FOLDER = "d:\\LotLog\\" + OM.EqpOptn.sModelName + "\\";   //LOT_FOLDER
        public string DAY_FOLDER = "d:\\DayLog\\" + OM.EqpOptn.sModelName + "\\";   //DAY_FOLDER
        public string WRK_FOLDER = "d:\\WrkLog\\" + OM.EqpOptn.sModelName + "\\";   //WRK_FOLDER
        public int iNo = 1;
        

        public void Init()
        {
            m_tmTick = new CCycleTimer();
            dPreTime = DateTime.Now.ToOADate();
            LoadSaveLastLotIni(true);
        }

        public void Close()
        {
            LoadSaveLastLotIni(false);
        }

        public void LoadSaveLastLotIni(bool _bLoad)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLastInfoPath = sExeFolder + "SeqData\\SpcInfo.ini";

            CIniFile IniLastInfo = new CIniFile(sLastInfoPath);

            if(_bLoad) CAutoIniFile.LoadStruct<TData>(sLastInfoPath, "SpcInfo", ref m_tData);
            else       CAutoIniFile.SaveStruct<TData>(sLastInfoPath, "SpcInfo", ref m_tData);     
        }

        public void SaveDataIni(/*double _dOaSaveDate */)
        {
            string sPath;
            DateTime tDateTime;
            tDateTime = DateTime.FromOADate(m_tData.dSttTime);
            sPath = LOT_FOLDER + tDateTime.ToString("yyyyMMdd") + ".ini";

            //기존에 있던것들 지우기.
            DirectoryInfo di = new DirectoryInfo(LOT_FOLDER);
            if (!di.Exists) di.Create();
            foreach (FileInfo fi in di.GetFiles())
            {
                //if (fi.Extension != ".log") continue;
                // 6개월 이전 로그를 삭제합니다.
                if (fi.CreationTime <= DateTime.Now.AddMonths(-6))
                {
                    fi.Delete();
                }
            }

            //카운트 저장.
            int iCnt = 0;
            CIniFile IniGetErrCnt = new CIniFile(sPath);
            IniGetErrCnt.Load("ETC", "DataCnt", ref iCnt);
            int iAddedCnt = iCnt + 1;
            IniGetErrCnt.Save("ETC", "DataCnt", iAddedCnt);

            //데이터 저장.
            CAutoIniFile.SaveStruct<TData>(sPath, iCnt.ToString(), ref m_tData);
        }

        public int GetDataCnt(DateTime _tSttData, DateTime _tEndData)
        {
            string sPath;

            int iCnt = 0;
            int iCntSum = 0;

            string SearchDate;

            for (DateTime d = _tSttData; d <= _tEndData; d = d.AddDays(1))
            {
                SearchDate = d.ToString();
                sPath = LOT_FOLDER + d.ToString("yyyyMMdd") + ".ini";

                CIniFile IniGetCnt = new CIniFile(sPath);

                IniGetCnt.Load("ETC", "DataCnt", ref iCnt);
                iCntSum += iCnt;
            }
            return iCntSum;
        }

        public void ClearData()
        {
            m_tData.Clear();
            //memset(&m_tData , 0 , sizeof(TData)); 이방식으로 하면 String 에서 메모리 누수 생김.
        }

        public void AddGoodCntData(int _iGoodCnt)
        {
            m_tData.iGoodCnt += _iGoodCnt;
        }

        public void AddWorkCntData(int _iWorkCnt)
        {
            m_tData.iWorkCnt += _iWorkCnt;
        }

        public void DispDateList(ListView _lvLotDate)
        {
            _lvLotDate.Clear();
            _lvLotDate.View = View.Details;
            _lvLotDate.LabelEdit = true;
            _lvLotDate.AllowColumnReorder = true;
            _lvLotDate.FullRowSelect = true;
            _lvLotDate.GridLines = true;
            _lvLotDate.Sorting = SortOrder.Ascending;

            _lvLotDate.Columns.Add("No", 50, HorizontalAlignment.Left);
            _lvLotDate.Columns.Add("Lot Name", 205, HorizontalAlignment.Left);

            //Init. Grid Data.
            for (int i = 0; i < _lvLotDate.Items.Count; i++) _lvLotDate.Items.Clear();

            //ListView Display
            DirectoryInfo Info = new DirectoryInfo(LOT_FOLDER);
            if (!Info.Exists) return ;

            //DirectoryInfo[] Dir = Info.GetDirectories();
            FileInfo[] Files =  Info.GetFiles();

            int iFileCnt = Files.Length;
            int iNo = 1;

            for (int i = 0; i < iFileCnt; i++)
            {
                ListViewItem liLotDate = new ListViewItem(string.Format("{0}", i + 1));
                liLotDate.SubItems.Add(Files[i].Name);
                _lvLotDate.Items.Add(liLotDate);
            }

        }

        public void DispLotList(string _sDate, ListView _lvLotInfo)
        {
            string sPath;
            sPath = LOT_FOLDER + _sDate ; //+ ".ini";

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
            _lvLotInfo.Columns.Add("Device"  , 200, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("Work", 40 , HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("Good", 40 , HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("Start"    , 100, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("End  "    , 100, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("WorkTime" , 100, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("ErrTime"  , 100, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("StopTime" , 100, HorizontalAlignment.Left);
            _lvLotInfo.Columns.Add("TotalTime", 100, HorizontalAlignment.Left);
            //_lvLotInfo.Columns.Add("UPEH"     , 210, HorizontalAlignment.Left);
            //_lvLotInfo.Columns.Add("UPH"      , 210, HorizontalAlignment.Left);
            

            //LotCount확인
            _lvLotInfo.Items.Clear();



            //카운트 저장.
            int iCnt = 0;
            CIniFile IniGetErrCnt = new CIniFile(sPath);
            IniGetErrCnt.Load("ETC", "DataCnt", ref iCnt);

            TData Data = new TData();

            for (int i = 0; i < iCnt; i++)
            {
                CAutoIniFile.LoadStruct<TData>(sPath, i.ToString(), ref Data);
                ListViewItem lvItem = new ListViewItem(string.Format("{0:000}", i + 1));
                lvItem.SubItems.Add(Data.sLotId               );
                lvItem.SubItems.Add(Data.sJobFile.ToString());
                lvItem.SubItems.Add(Data.iWorkCnt  .ToString());
                lvItem.SubItems.Add(Data.iGoodCnt  .ToString());
                //lvItem.SubItems.Add(m_tData.dUPEH     .ToString());
                //lvItem.SubItems.Add(m_tData.dUPH      .ToString());
                lvItem.SubItems.Add(DateTime.FromOADate(Data.dSttTime  ).ToString("HH:mm:ss")); 
                lvItem.SubItems.Add(DateTime.FromOADate(Data.dEndTime  ).ToString("HH:mm:ss"));
                lvItem.SubItems.Add(DateTime.FromOADate(Data.dWorkTime ).ToString("HH:mm:ss"));
                lvItem.SubItems.Add(DateTime.FromOADate(Data.dErrTime  ).ToString("HH:mm:ss"));
                lvItem.SubItems.Add(DateTime.FromOADate(Data.dStopTime ).ToString("HH:mm:ss"));
                lvItem.SubItems.Add(DateTime.FromOADate(Data.dTotalTime).ToString("HH:mm:ss"));
                
                _lvLotInfo.Items.Add(lvItem);
            }   
            
            _lvLotInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        
        public void Update(EN_SEQ_STAT Stat)
        {
            //시간 단위가 너무 커서 컴파일 에러 남 진섭
            
            double dCrntTime = DateTime.Now.ToOADate();
            //double dCycleTime = dCrntTime - m_tData.dTickTime ;
            //m_tData.dTickTime = dCrntTime;
            double dCycleTime = dCrntTime - dPreTime;

            switch (Stat) {
                case EN_SEQ_STAT.Init     : m_tData.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Warning  : m_tData.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Error    : m_tData.dErrTime  += dCycleTime ; break ;
                case EN_SEQ_STAT.Running  : m_tData.dWorkTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Stop     : m_tData.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Manual   : m_tData.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.RunWarn  : m_tData.dWorkTime += dCycleTime ; break ;
                case EN_SEQ_STAT.WorkEnd  : m_tData.dStopTime += dCycleTime ; break ;
            }
            m_tData.dTotalTime += dCycleTime ;
            //m_tData.dTickTime = STG.GetTickTime() / 1000;
            dPreTime = dCrntTime ;
        
            //잡파일 체인지 할때 어레이 동적 할당 다시 하기때문에 돌려줘야 한다.
            //이장비는 툴만 보기 때문에 상관 없지만 그냥 이렇게 내둔다.
            if(Stat == EN_SEQ_STAT.Stop ) return ;
            //if(Stat != ssRunning && Stat != ssRunWarn) return ;
            ///////////////////////매우 중요.............
        
            string sLotId ;
            sLotId = LOT.GetLotNo() ; //LDR까지 스캔해보고 있으면 미리 랏오픈을 한다.
            if(sLotId != m_tData.sLotId && sLotId != "") { //어맛! 새로운 랏.
                m_tData.Clear();
        
                m_tData.sLotId    = LOT.GetLotNo() ;
                m_tData.dSttTime  = dCrntTime ;
            }


            const int iSlotNo = 0 ;
            const int iMgzNo  = 0 ;
        
            m_tData.dEndTime  = dCrntTime ;
            m_tData.dUPEH     = m_tData.dTotalTime == 0 ? 0.0 : m_tData.iWorkCnt  / (m_tData.dTotalTime * 24) ;
            m_tData.dUPH      = m_tData.dWorkTime  == 0 ? 0.0 : m_tData.iWorkCnt  / (m_tData.dWorkTime  * 24) ;
            m_tData.sJobFile  = OM.GetCrntDev() ;

        }

        public void DispLotInfo(ListView _lvLotInfo) //오퍼레이션 창용.
        {
            DateTime tDateTime;

            string sTemp;
        
            //LotId
            _lvLotInfo.Items[0].SubItems[0].Text = "LotId";
            _lvLotInfo.Items[0].SubItems[1].Text = m_tData.sLotId;
        
            //WorkCnt
            _lvLotInfo.Items[1].SubItems[0].Text = "WorkCnt  ";
            _lvLotInfo.Items[1].SubItems[1].Text = m_tData.iWorkCnt.ToString();

            //WorkCnt
            _lvLotInfo.Items[2].SubItems[0].Text = "GoodCnt  ";
            _lvLotInfo.Items[2].SubItems[1].Text = m_tData.iGoodCnt.ToString();
        
            //UPH
            sTemp = string.Format("{0:0.0}", m_tData.dUPH);
            _lvLotInfo.Items[3].SubItems[0].Text = "UPH      ";
            _lvLotInfo.Items[3].SubItems[1].Text = sTemp;
        
            //SttTime
            _lvLotInfo.Items[4].SubItems[0].Text = "SttTime  ";
            tDateTime = DateTime.FromOADate(m_tData.dSttTime);
            _lvLotInfo.Items[4].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");

            //TickTime
            //sTemp = string.Format("{0:0.00}", dPreTime);
            _lvLotInfo.Items[5].SubItems[0].Text = "StopTime ";
            tDateTime = DateTime.FromOADate(m_tData.dStopTime);
            _lvLotInfo.Items[5].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");
        }

        public string GetCrntLotNo()
        {
            return LOT.GetLotNo();
        }


    }

    public class DayData
    {
        public struct TData
        {
            public double dWorkTime;
            public double dStopTime;
            public double dErrTime;
            public double dTotalTime;
            public double dUPEH;
            public double dUPH;
            public int    iWorkCnt;
            public int    iLotCnt;
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

        public static TData m_tData;

        public void Init()
        {
            LoadSaveDayIni(true , ref m_tData) ;
        }
        
        public void Close()
        {
            LoadSaveDayIni(false , ref m_tData) ;
        }

        static double dPreTime = DateTime.Now.ToOADate();  
        static int iPreWorkCnt = 0;
        public void Update(/*string _sCrntLotNo,*/ EN_SEQ_STAT Stat)
        {
            //string sLotId ;
            double dCrntTime = DateTime.Now.ToOADate();                    
            
            double dCycleTime = dCrntTime - dPreTime ;
        
            //Time Info.
            switch(Stat) {
                case EN_SEQ_STAT.Init     : m_tData.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Warning  : m_tData.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Error    : m_tData.dErrTime  += dCycleTime ; break ;
                case EN_SEQ_STAT.Running  : m_tData.dWorkTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Stop     : m_tData.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.Manual   : m_tData.dStopTime += dCycleTime ; break ;
                case EN_SEQ_STAT.RunWarn  : m_tData.dWorkTime += dCycleTime ; break ;
                case EN_SEQ_STAT.WorkEnd  : m_tData.dStopTime += dCycleTime ; break ;
            }
            m_tData.dTotalTime += dCycleTime ;
        
        
        
            //Day Log 랏 데이터는 저장 할때 랏오픈된 날짜에 저장 하지만 Day는 Lot하고 상관 없는 개념이라 그냥 바로바로 Day에 저장 한다.
            //Lot과 데이터 연동 하고 싶으면 랏업데이트에 있는 저장 패턴 복사해서 쓴다.
            bool bDateChanged = ((int)dCrntTime) != ((int)dPreTime) ; //소수점 이하는 시간 데이터.
            if(bDateChanged) { //날자 바뀜.
                ClearData();
            }
            dPreTime = dCrntTime ;
        
        
            //string sPreLotNo = _sCrntLotNo ;
            //if(sPreLotNo != _sCrntLotNo /*&& Stat !=ssStop*/) { //앗 랏이 바뀌었네!!!!
            //    m_tData.iLotCnt++;
            //}
            //sPreLotNo = _sCrntLotNo ;
        
        
        
            //m_tData.dErrTime = dCrntTime;
            //m_tData.dWorkTime = ;
            //m_tData.dStopTime;
            //m_tData.dTotalTime;
            //m_tData.dUPH;
            //m_tData.iWorkCnt;
            

            m_tData.dUPEH     = m_tData.dTotalTime == 0 ? 0.0 : m_tData.iWorkCnt / (m_tData.dTotalTime * 24) ;
            m_tData.dUPH      = m_tData.dWorkTime  == 0 ? 0.0 : m_tData.iWorkCnt / (m_tData.dWorkTime  * 24) ;
        
        
        
            //잡파일 체인지 할때 어레이 동적 할당 다시 하기때문에 돌려줘야 한다.
            //이장비는 툴만 보기 때문에 상관 없지만 그냥 이렇게 내둔다.
            if(Stat == EN_SEQ_STAT.Stop) return ;
            ///////////////////////매우 중요.............
            int iWorkCnt;
            if(SEQ._bRun){
                iWorkCnt= DM.ARAY[(int)ri.MRK].GetCntStat(cs.Good) + DM.ARAY[(int)ri.MRK].GetCntStat(cs.Empty);
                    if( iPreWorkCnt != iWorkCnt && iWorkCnt != 0){
                        m_tData.iWorkCnt += 8;
                        iPreWorkCnt = iWorkCnt;
                }
            }
            
           
        }

        public void DispDayInfo(ListView _lvDayInfo) //오퍼레이션 창용.
        {
            DateTime tDateTime;

            string sTemp;
            _lvDayInfo.Items[0].SubItems[0].Text = "WorkTime ";
            tDateTime = DateTime.FromOADate(m_tData.dWorkTime);
            _lvDayInfo.Items[0].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");


            _lvDayInfo.Items[1].SubItems[0].Text = "StopTime ";
            tDateTime = DateTime.FromOADate(m_tData.dStopTime);
            _lvDayInfo.Items[1].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");


            _lvDayInfo.Items[2].SubItems[0].Text = "ErrTime  ";
            tDateTime = DateTime.FromOADate(m_tData.dErrTime);
            _lvDayInfo.Items[2].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");


            _lvDayInfo.Items[3].SubItems[0].Text = "TotalTime";
            tDateTime = DateTime.FromOADate(m_tData.dTotalTime);
            _lvDayInfo.Items[3].SubItems[1].Text = tDateTime.ToString("HH:mm:ss");

            sTemp = string.Format("{0:0.0000}", m_tData.dUPH);
            _lvDayInfo.Items[4].SubItems[0].Text = "UPH";
            _lvDayInfo.Items[4].SubItems[1].Text = sTemp;


            _lvDayInfo.Items[5].SubItems[0].Text = "WorkCnt  ";
            _lvDayInfo.Items[5].SubItems[1].Text = m_tData.iWorkCnt.ToString();
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
            m_tData.dUPEH     = m_tData.dTotalTime == 0 ? 0.0 : m_tData.iWorkCnt / (m_tData.dTotalTime * 24) ;
            m_tData.dUPH      = m_tData.dWorkTime  == 0 ? 0.0 : m_tData.iWorkCnt / (m_tData.dWorkTime  * 24) ;
        
            SaveDayData(m_tData , tDateTime);
        
        }
        public void LoadSaveDayIni(bool _bLoad , ref TData _tData)
        {
            //Make Dir.
            string sPath = DAY_FOLDER + "DayInfo.ini";
        
            if(_bLoad) {
                CIniFile IniLoadDayInfo = new CIniFile(sPath);

                IniLoadDayInfo.Load("Data", "dWorkTime ", ref _tData.dWorkTime );
                IniLoadDayInfo.Load("Data", "dStopTime ", ref _tData.dStopTime );
                IniLoadDayInfo.Load("Data", "dErrTime  ", ref _tData.dErrTime  );
                IniLoadDayInfo.Load("Data", "dTotalTime", ref _tData.dTotalTime);
                IniLoadDayInfo.Load("Data", "dUPEH     ", ref _tData.dUPEH     );
                IniLoadDayInfo.Load("Data", "dUPH      ", ref _tData.dUPH      );
                IniLoadDayInfo.Load("Data", "iWorkCnt  ", ref _tData.iWorkCnt  );
                IniLoadDayInfo.Load("Data", "iLotCnt   ", ref _tData.iLotCnt   );
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
                if (fi.Extension != ".log") continue;

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
        
        public static void ClearData()
        {
            m_tData.TimeClear();
            m_tData.UPHClear ();
            m_tData.CntClear ();
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
        public static LotData LOT = new LotData();
        public static DayData DAY = new DayData();

        public static void Init()
        {
            ERR.Init();
            LOT.Init();
            DAY.Init();
        }
        
        public static void Close()
        {
            ERR.Close();
            LOT.Close();
            DAY.Close();
        }
        
        public static void Update(EN_SEQ_STAT Stat)
        {
            //LOT = new CLotData() ;
            ERR.Update(LOT.GetCrntLotNo()     );
            LOT.Update(Stat);
            //if (LOT.GetCrntLotNo() != null)
            DAY.Update(Stat);
        }










    }
        
        
  







}
