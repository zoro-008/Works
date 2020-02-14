﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using COMMON;
using SML;

using System.Globalization;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Machine
{
    public class CMapData //: DataHandler<TLotData> 안쪽형식이 달라서 못씀.
    {
        //public int[] ChipCnts    = new int [(int)cs.MAX_CHIP_STAT]; //현재랏 카운트
        //public int[] PreChipCnts = new int [(int)cs.MAX_CHIP_STAT]; //전랏 카운트
        //public int[] SpcChipCnts = new int [(int)cs.MAX_CHIP_STAT]; //SPC디스플레이용.

        //HVI-1500i
        //포스트버퍼에 검사 끝난 결과 데이터 ini 파일로 저장
        public void SaveDataMap(int _iArayId)
        {
            //Read&Write.
            CConfig Config = new CConfig();
            string sSPCFolder = System.IO.Directory.GetParent(SPC.LOT.Folder).Parent.FullName.ToString(); //"d:\\SpcLog\\"+ Eqp.sEqpName  + "\\LotLog\\"    
            string sMapPath = "";
            //string sDataPath = "";
            string sToday = DateTime.FromOADate(SPC.LOT.Data.StartedAt).ToString("yyyyMMdd");//DateTime.Now.ToString("yyyyMMdd");

            int iMgzNo ;
            int iSlotNo;
            if(!int.TryParse(DM.ARAY[_iArayId].ID , out int iId))
            {
                iMgzNo  = 0 ; 
                iSlotNo = 0 ;
            }
            else
            {
                iMgzNo  = iId/100 ;
                iSlotNo = iId%100 ;
            }

            sMapPath  = sSPCFolder + "\\DataMap\\" + sToday + "\\" + DM.ARAY[_iArayId].LotNo + "\\" + iMgzNo.ToString() + "\\" + iSlotNo.ToString("D2") + ".INI";
            //sMapPath = sSPCFolder + "\\DataMap\\" + sToday + "\\" + DM.ARAY[_iArayId].LotNo + "\\Data.INI";

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(sMapPath));
            if (!di.Exists) di.Create();

            DM.ARAY[_iArayId].Load(Config, false);
            Config.Save(sMapPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        }

        public bool DataExist(double _dStartTime  , string _sLotNo)
        {
            string sSPCFolder = System.IO.Directory.GetParent(SPC.LOT.Folder).Parent.FullName.ToString();

            string sDataPath = "";
            string sToday = DateTime.FromOADate(_dStartTime).ToString("yyyyMMdd");//DateTime.Now.ToString("yyyyMMdd");

            sDataPath = sSPCFolder + "\\DataMap\\" + sToday + "\\" + _sLotNo +"\\";

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(sDataPath));
           
            return di.Exists ;
        }

        public string GetLotNo(string _sOriLot)
        {
            string sRetNo = "";
            int iCnt = 0 ;
            //실제 시간은 어쩔수 없이 여기서 나우로 가져오지면 실제 SPC에서 데이터 남기는 랏스타트 시간과 약간의 딜레이가 있지만 크게 문제 없다.
            //다만 이시점에 폴더 확인 하고 랏오픈시점에 다음날로 넘어가면 
            //IA10010000 을 19일날 두번째 돌리기 시작 했으면 20일날 데이터가 IA10010000_0이 아닌 IA10010000_1로 남게됨.
            while(SPC.MAP.DataExist(DateTime.Now.ToOADate() , _sOriLot + "_" + iCnt.ToString())) 
            {
                iCnt++;
            }
            sRetNo = _sOriLot + "_" + iCnt.ToString() ;
            return sRetNo ;
        }

        public void SaveDataCnt(double _dStartTime  , string _sLotNo , int [] _ChipCnts)
        {
            //if(_ChipCnts.Length != (int)cs.MAX_CHIP_STAT) return ;
            //Read&Write.
            string sSPCFolder = System.IO.Directory.GetParent(SPC.LOT.Folder).Parent.FullName.ToString();

            string sDataPath = "";
            string sToday = DateTime.FromOADate(_dStartTime).ToString("yyyyMMdd");//DateTime.Now.ToString("yyyyMMdd");

            sDataPath = sSPCFolder + "\\DataMap\\" + sToday + "\\" + _sLotNo + "\\Data.cnt";

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(sDataPath));
            if (!di.Exists) di.Create();

            CIniFile Ini = new CIniFile(sDataPath) ;
            Ini.Save("ChipCount" , "Rslt" , _ChipCnts);
        }

        public void LoadDataCnt(double _dStartTime , string _sLotNo , ref int [] _ChipCnts )
        {
            //if(_ChipCnts.Length != (int)cs.MAX_CHIP_STAT) return ;
            //Read&Write.
            string sSPCFolder = System.IO.Directory.GetParent(SPC.LOT.Folder).Parent.FullName.ToString();

            string sDataPath = "";
            string sDay = DateTime.FromOADate(_dStartTime).ToString("yyyyMMdd");//DateTime.Now.ToString("yyyyMMdd");

            sDataPath = sSPCFolder + "\\DataMap\\" + sDay + "\\" + _sLotNo + "\\Data.cnt";

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(sDataPath));
            if (!di.Exists) di.Create();

            CIniFile Ini = new CIniFile(sDataPath) ;
            Ini.Load("ChipCount" , "Rslt" , ref _ChipCnts);
        }

        

        //HVI-1500i
        //DataMap ini 파일 로드 D:\SpcLog\HVI-1500i\DataMap\20190116\ddddd\1
        public void LoadDataMap(string _sPath, CArray _Array)
        {
            //Read&Write.
            CConfig Config = new CConfig();
            string sPath = "";

            sPath = _sPath;

            Config.Load(sPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);

            _Array.Load(Config, true);
        }

    }

    //데이터를 랏넘버나 트레이 넘버로 지울 수 있어야 한다.
    //SPC에서 랏넘버랑 트레이 넘버로 검색 가능하게.
    public struct TLotData
    {
        public string LotNo          ; //업데이트세팅.

        public string Device         ; //밖에서세팅.
        public double StartedAt      ;//업데이트세팅.
        public double EndedAt        ;//업데이트세팅.

        //SPC관련.
        public double RunTime        ;//업데이트세팅. //Run시간.
        public double DownTime       ;//업데이트세팅. //Jam시간. 
        public double IdleTime       ;//업데이트세팅. //Stop상태시간.
        public double FailureTime    ;//업데이트세팅. //고장시간.
    };
    public class CLot : DataHandler<TLotData>
    {
        public CLot(string _sPath)
        {
            Folder = _sPath ;
        }

        long lPreTime = DateTime.Now.Ticks;
        string sPreLot = "";

        public void Update(string _sCrntLotNo, EN_SEQ_STAT Stat, string _sCrntDev, bool _bMaint)
        {
            long lCrntTime = DateTime.Now.Ticks;
            long lGapTime = lCrntTime - lPreTime;
            double dTimeGap = lGapTime / 10000.0f;

            if (_bMaint) //업타임 계산에서 씀
            {
                Data.FailureTime += dTimeGap;
            }
            else if (Stat == EN_SEQ_STAT.Error && ML.ER_GetErrLevel((ei)ML.ER_GetLastErr()) == EN_ERR_LEVEL.Error)
            {
                Data.DownTime += dTimeGap;
            }
            else if (Stat == EN_SEQ_STAT.Running || Stat == EN_SEQ_STAT.RunWarn)
            {
                Data.RunTime += dTimeGap;
            }
            else //Stop Init Warnning WorkEnd 
            {
                Data.IdleTime += dTimeGap;
            }

            //20190523 선계원
            //CDC장비에서는 1혈액샘플당 1랏으로 치고 픽커가 찝을때 "?"으로 랏오픈후 
            //중간에 바코드 찍으면 바코드로 랏넘버를 바꿔야 함.
            if(sPreLot == "?" && _sCrntLotNo != "?")
            {
                sPreLot = _sCrntLotNo ;
            }
            bool LotChanged = (sPreLot != _sCrntLotNo);//&& sPreLot != "" ;
            bool LotEnded = LotChanged && _sCrntLotNo == "";
            bool LotOpened = LotChanged && sPreLot == "";

            if (LotOpened) {
                //새로운랏 처리.
                DataClear();
                Data.LotNo        = _sCrntLotNo            ;
                Data.Device       = _sCrntDev              ;
                Data.StartedAt    = DateTime.Now.ToOADate();

                //OM.EqpStat.iWorkCnt  = 0 ;
                //OM.EqpStat.iFailCnt  = 0 ;
                //OM.EqpStat.dWorkUPH  = 0 ;
                //OM.EqpStat.dWorkTime = 0 ;
                //OM.EqpStat.iReinputCnt = 0 ;
            }
            else if (LotEnded){
                Data.EndedAt      = DateTime.Now.ToOADate() ;
                SaveDataIni(Data.StartedAt);
                //이위로는 기존랏 처리.
            }
            else if (LotChanged)
            {
                Data.EndedAt      = DateTime.Now.ToOADate() ;
                SaveDataIni(Data.StartedAt);
                //이위로는 기존랏 처리.

                //새로운랏 처리.
                DataClear();
                Data.LotNo        = _sCrntLotNo            ;
                Data.Device       = _sCrntDev              ;
                Data.StartedAt    = DateTime.Now.ToOADate();
                

                //OM.EqpStat.iWorkCnt  = 0 ;
                //OM.EqpStat.iFailCnt  = 0 ;
                //OM.EqpStat.dWorkUPH  = 0 ;
                //OM.EqpStat.dWorkTime = 0 ;
                //OM.EqpStat.iReinputCnt = 0 ;
            }
            lPreTime = lCrntTime ;
            //sPreLot  = "";
            sPreLot  = _sCrntLotNo;
        }
        public void DispLotInfo(ListView _lvLotInfo) //오퍼레이션 창용.
        {
            DateTime tDateTime;

            ListViewItem Item ; 
            //여기는 두줄형식.
            //LotId
            Item = new ListViewItem("BARCODE"      );Item.SubItems.Add(Data.LotNo); _lvLotInfo.Items.Add(Item);

            tDateTime = DateTime.FromOADate(Data.StartedAt);
            Item = new ListViewItem("START TIME");Item.SubItems.Add(tDateTime.ToString("HH:mm:ss")); _lvLotInfo.Items.Add(Item);

            //_lvLotInfo.Items[iStart + 0].SubItems[0].Text = "NAME";        _lvLotInfo.Items[iStart + 0].SubItems[1].Text = Data.LotNo;
            
            //여기는 한줄형식...
            ////WorkCnt
            //_lvLotInfo.Items[2].SubItems[0].Text = "WORK COUNT";
            //_lvLotInfo.Items[3].SubItems[0].Text = OM.EqpStat.iWorkCnt.ToString();
            //
            ////WorkCnt
            //_lvLotInfo.Items[4].SubItems[0].Text = "FAIL COUNT";
            //_lvLotInfo.Items[5].SubItems[0].Text = OM.EqpStat.iFailCnt.ToString();
            //
            ////Yield
            //double dYield = OM.EqpStat.iWorkCnt > 0 ? (OM.EqpStat.iWorkCnt - OM.EqpStat.iFailCnt ) / (double)OM.EqpStat.iWorkCnt : 0.0 ;
            //dYield = dYield * 100 ;
            //_lvLotInfo.Items[6].SubItems[0].Text = "YIELD";
            //_lvLotInfo.Items[7].SubItems[0].Text = dYield.ToString("N2");
            //
            ////UPH
            //OM.EqpStat.dWorkUPH = OM.EqpStat.iWorkCnt / Data.RunTime * 1000.0 * 60.0 * 60.0;
            //string sTemp = string.Format("{0:0.0}", OM.EqpStat.dWorkUPH);
            //_lvLotInfo.Items[8].SubItems[0].Text = "WORK UPH";
            //_lvLotInfo.Items[9].SubItems[0].Text = sTemp;
            //
            ////SttTime
            //tDateTime = DateTime.FromOADate(Data.StartedAt);
            //_lvLotInfo.Items[10].SubItems[0].Text = "START TIME";
            //_lvLotInfo.Items[11].SubItems[0].Text = tDateTime.ToString("HH:mm:ss");

            //TickTime
            //sTemp = string.Format("{0:0.00}", OM.EqpStat.dWorkTime);
            //_lvLotInfo.Items[12].SubItems[0].Text = "CYCLE TIME";
            //_lvLotInfo.Items[13].SubItems[0].Text = sTemp;
        }
    }

    //DayData 없어서 추가
    public struct TDayData
    {
        //public string LotNo; //업데이트세팅.

        public string Device; //밖에서세팅.
        //public double StartedAt;//업데이트세팅.
        //public double EndedAt;//업데이트세팅.

        //SPC관련.
        public double RunTime     ;//업데이트세팅. //Run시간.
        public double DownTime    ;//업데이트세팅. //Jam시간. 
        public double IdleTime    ;//업데이트세팅. //Stop상태시간.
        public double FailureTime ;//업데이트세팅. //고장시간.

        public int    DayWorkCnt  ;//하루 작업 갯수(하루 지나면 초기화)

        public string LastWrkDay;//업데이트세팅. //프로그램 실행 마지막 날짜.
        public void DataClear()
        {
            RunTime     = 0.0;
            DownTime    = 0.0;
            IdleTime    = 0.0;
            FailureTime = 0.0;
            DayWorkCnt  = 0  ;
        }

    };
    public class CDay : DataHandler<TDayData>
    {
        public CDay(string _sPath)
        {
            Folder = _sPath;
            if(Data.LastWrkDay == "") Data.LastWrkDay = DateTime.Now.ToString("MMdd");
        }

        long lPreTime = DateTime.Now.Ticks;

        public void Update(EN_SEQ_STAT Stat, string _sCrntDev, bool _bMaint)
        {
            string sCrntDay  = DateTime.Now.ToString("MMdd");
            long   lCrntTime = DateTime.Now.Ticks;
            long   lGapTime  = lCrntTime - lPreTime;
            double dTimeGap  = lGapTime / 10000.0f;

            if (_bMaint) //업타임 계산에서 씀
            {
                Data.FailureTime += dTimeGap;
            }
            else if (Stat == EN_SEQ_STAT.Error && ML.ER_GetErrLevel((ei)ML.ER_GetLastErr()) == EN_ERR_LEVEL.Error)
            {
                Data.DownTime += dTimeGap;
            }
            else if (Stat == EN_SEQ_STAT.Running || Stat == EN_SEQ_STAT.RunWarn)
            {
                Data.RunTime += dTimeGap;
            }
            else //Stop Init Warnning WorkEnd 
            {
                Data.IdleTime += dTimeGap;
            }

            Data.Device = _sCrntDev;

            
            //if(DM.ARAY[ri.OALN].CheckAllStat(cs.WorkEnd) || DM.ARAY[ri.WORK].CheckAllStat(cs.WorkEnd))
            //{
            //    Data.ToTalWorkCnt++;
            //    Data.DayWorkCnt++;
            //}

            //Data.ToTalWorkCnt = OM.EqpStat.iTotalWorkCnt; 
            //Data.DayWorkCnt   = OM.EqpStat.iDayWorkCnt  ; 
            
            if (Data.LastWrkDay != sCrntDay)
            {
                //OM.EqpStat.iDayWorkCnt = 0 ;
                //OM.EqpStat.iDayWorkUPH = 0 ;
            
                DataClear();
                Data.LastWrkDay = sCrntDay;
            }

            lPreTime = lCrntTime;
        }
        public void DispDayInfo(ListView _lvDayInfo) //오퍼레이션 창용.
        {
            DateTime tDateTime;

            string sTemp;

            ListViewItem Item ; 
            //여기는 두줄형식.
            //LotId
            Item = new ListViewItem("DEVICE"         );Item.SubItems.Add(Data.Device                          ); _lvDayInfo.Items.Add(Item);
            Item = new ListViewItem("DAY WORK COUNT" );Item.SubItems.Add(Data.DayWorkCnt.ToString()           ); _lvDayInfo.Items.Add(Item);

            double dUPH = Data.DayWorkCnt / Data.RunTime * 1000.0 * 60.0 * 60.0;
            sTemp = string.Format("{0:0.0}", dUPH);
            Item = new ListViewItem("DAY UPH"        );Item.SubItems.Add(sTemp); _lvDayInfo.Items.Add(Item);

            var RunTime = TimeSpan.FromMilliseconds(Data.RunTime);
            Item = new ListViewItem("DAY RUNTIME"    );Item.SubItems.Add(RunTime.ToString(@"hh\:mm\:ss")         ); _lvDayInfo.Items.Add(Item);

            var ErrTime = TimeSpan.FromMilliseconds(Data.DownTime);
            Item = new ListViewItem("DAY ERRTIME"    );Item.SubItems.Add(ErrTime.ToString(@"hh\:mm\:ss")         ); _lvDayInfo.Items.Add(Item);

            var IdleTime = TimeSpan.FromMilliseconds(Data.IdleTime);
            Item = new ListViewItem("DAY IDLETIME"   );Item.SubItems.Add(IdleTime.ToString(@"hh\:mm\:ss")        ); _lvDayInfo.Items.Add(Item);





            //tDateTime = DateTime.FromOADate(Data.StartedAt);
            //Item = new ListViewItem("START TIME");Item.SubItems.Add(tDateTime.ToString("HH:mm:ss")); _lvLotInfo.Items.Add(Item);

            ////LotId
            //_lvDayInfo.Items[0].SubItems[0].Text = "LotNo";
            //_lvDayInfo.Items[0].SubItems[1].Text = Data.LotNo;
            
            //Device Name
            //_lvDayInfo.Items[0].SubItems[0].Text = "DEVICE";
            //_lvDayInfo.Items[1].SubItems[0].Text = Data.Device;
            //
            ////WorkCnt
            //_lvDayInfo.Items[2].SubItems[0].Text = "DAY WORK COUNT";
            //_lvDayInfo.Items[3].SubItems[0].Text = OM.EqpStat.iDayWorkCnt.ToString();
            //
            ////UPH
            //OM.EqpStat.iDayWorkUPH = OM.EqpStat.iDayWorkCnt / Data.RunTime * 1000.0 * 60.0 * 60.0;
            //sTemp = string.Format("{0:0.0}", OM.EqpStat.iDayWorkUPH);
            //_lvDayInfo.Items[4].SubItems[0].Text = "WORK UPH";
            //_lvDayInfo.Items[5].SubItems[0].Text = sTemp;

            //Run Time
            

        }
    }

    //데이터를 랏넘버나 트레이 넘버로 지울 수 있어야 한다.
    //SPC에서 랏넘버랑 트레이 넘버로 검색 가능하게.
    public struct TErrData
    {
        //전체다 update에서 기록한다.
        public int    ErrNo    ;
        public string ErrName  ;
        public double StartedAt;
        public double EndedAt  ;
        public double ErrTime  ;
        public string ErrMsg   ;
        public string LotId    ;
    };
    public class CErr :DataHandler<TErrData>
    {

        public CErr(string _sPath)
        {
            Folder = _sPath ;
        }
        bool bPreErr = false ;
        public void Update(string _sCrntLotNo , EN_SEQ_STAT Stat)
        {
            //Err Log
            
            bool isErr = Stat == EN_SEQ_STAT.Error; // SML.ER.IsErr();
            if(isErr && !bPreErr) {
                Data.ErrNo     = ML.ER_GetLastErr() ;
                Data.ErrName   = ML.ER_GetErrName(ML.ER_GetLastErr());
                Data.StartedAt = DateTime.Now.ToOADate();
                Data.ErrMsg    = ML.ER_GetErrSubMsg((ei)ML.ER_GetLastErr())  ;
                Data.LotId     = _sCrntLotNo   ;
            }
            if(!isErr && bPreErr)
            {
                Data.EndedAt   = DateTime.Now.ToOADate();
                TimeSpan Span ;
                Span = TimeSpan.FromDays(Data.EndedAt - Data.StartedAt);
                Data.ErrTime   = Span.TotalMilliseconds ;
                SaveDataIni(Data.StartedAt);
            }
            bPreErr = isErr ;
        } 

        //시간단위.
        //MTBA = 평균 Assist (오퍼가 6분이내에 처리하여 구동) 간격 1시간 이상이여야 한다.
        //이렇게 쓰면 결국 Data로딩 Disp하면서 한번 쓰고 
        //MTBA하면서 한번 로딩 하는 방식이라.. 문제 있음.
        public double GetMTBA(DateTime _tSttData, DateTime _tEndData)
        {
            if(_tSttData > _tEndData) return 0 ;
            int iDatsCnt = GetDataCnt(_tSttData, _tEndData);
            if(iDatsCnt < 2) return 0 ; //2개이상 되어야 간격을 계산 할 수 있다.
            //TErrData[] Datas = new TErrData[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);

            List<TErrData> Datas = new List<TErrData>();
            LoadDataList(_tSttData, _tEndData, ref Datas);

            double dMs   = (Datas[iDatsCnt-1].StartedAt - Datas[0].StartedAt) / (iDatsCnt-1);
            double dSec  = dMs / 1000.0 ;
            double dMin  = dSec / 60.0 ;
            double dHour = dMin / 60.0 ;

            return dHour ;
        }

        //초단위.
        //MTTA = 평균 Assist 해결 시간. 30초이하여야한다.
        public double GetMTTA(DateTime _tSttData, DateTime _tEndData)
        {
            if(_tSttData > _tEndData) return 0 ;
            int iDatsCnt = GetDataCnt(_tSttData, _tEndData);
            if(iDatsCnt < 1) return 0 ; //1개이상 되어야 계산 할 수 있다.
            //TErrData[] Datas = new TErrData[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);

            List<TErrData> Datas = new List<TErrData>();
            LoadDataList(_tSttData, _tEndData, ref Datas);

            double dSumMs = 0 ;
            foreach (TErrData Data in Datas)
            {
                dSumMs += Data.ErrTime ;
            }

            double dMs   = dSumMs / iDatsCnt ;
            double dSec  = dMs / 1000.0 ;
            double dMin  = dSec / 60.0 ;
            double dHour = dMin / 60.0 ;

            return dSec ;
        }

        //MTBF = 평균 Failure(메인트가 붙어서 5분이상 수리) 간격  200시간 이상이여야 한다.
        //MTTR = 평균 Failure 수리 시간 1시간 이하여야 한다.
    }

    public struct TFailureData
    {
        //UI 입력.
        public string EngineerID  ;
        public string Purpose     ;

        //업데이트 입력.
        public double StartedAt   ;
        public double EndedAt     ;
        public double FailureTime ;
        public string LotId       ;
    };
    public class CFailure :DataHandler<TFailureData>
    {
        public CFailure(string _sPath)
        {
            Folder = _sPath ;
        }

        bool bFirst = true ; //메인트 상태에서 껐다켜는 경우가 있는데 이럴때 Failure가 껐다 켰을때 다시 세팅이 되는 문제가 있음.
        bool bPreFailer = false ;
        public void Update(string _sCrntLotNo , EN_SEQ_STAT Stat , bool _bMaint)
        {
            if(bFirst) {
                bPreFailer = _bMaint ;
                bFirst = false ;
            }

            //Err Log            
            bool isFailer = _bMaint; // SML.ER.IsErr();
            if(isFailer && !bPreFailer) {
                Data.StartedAt  = DateTime.Now.ToOADate();
                Data.LotId      = _sCrntLotNo   ;
            }
            if(!isFailer && bPreFailer)
            {
                Data.EndedAt       = DateTime.Now.ToOADate();
                TimeSpan Span ;
                Span = TimeSpan.FromDays(Data.EndedAt - Data.StartedAt);
                Data.FailureTime   = Span.TotalMilliseconds ;

                SaveDataIni(Data.StartedAt);
            }
            bPreFailer = isFailer ;
        } 

        //시간단위.
        //MTBF = 평균 Failure(메인트가 붙어서 5분이상 수리) 간격  200시간 이상이여야 한다.
        public double GetMTBF(DateTime _tSttData, DateTime _tEndData)
        {
            if(_tSttData > _tEndData) return 0 ;
            int iDatsCnt = GetDataCnt(_tSttData, _tEndData);
            if(iDatsCnt < 2) return 0 ; //2개이상 되어야 간격을 계산 할 수 있다.
            //TFailureData[] Datas = new TFailureData[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);

            List<TFailureData> Datas = new List<TFailureData>();
            LoadDataList(_tSttData, _tEndData, ref Datas);

            double dMs   = (Datas[iDatsCnt-1].StartedAt - Datas[0].StartedAt) / (iDatsCnt-1);
            double dSec  = dMs / 1000.0 ;
            double dMin  = dSec / 60.0 ;
            double dHour = dMin / 60.0 ;

            return dHour ;
        }

        //초단위.
        //MTTR = 평균 Failure 수리 시간 1시간 이하여야 한다.
        public double GetMTTR(DateTime _tSttData, DateTime _tEndData)
        {
            if(_tSttData > _tEndData) return 0 ;
            int iDatsCnt = GetDataCnt(_tSttData, _tEndData);
            if(iDatsCnt < 1) return 0 ; //1개이상 되어야 계산 할 수 있다.
            //TFailureData[] Datas = new TFailureData[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);
            
            List<TFailureData> Datas = new List<TFailureData>();
            LoadDataList(_tSttData, _tEndData, ref Datas);

            double dSumMs = 0 ;
            foreach (TFailureData Data in Datas)
            {
                dSumMs += Data.FailureTime ;
            }

            double dMs   = dSumMs / iDatsCnt ;
            double dSec  = dMs / 1000.0 ;
            double dMin  = dSec / 60.0 ;
            double dHour = dMin / 60.0 ;

            return dHour ;
        }
    }

    public class DataHandler<T> where T : struct//StartedAt T구성요소중에  StartedAt  은 꼭 있어야함.
    {
        public T Data;

        public string Folder ;

        public void DataClear()
        {
            Type type = Data.GetType();
            FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
            for(int i = 0 ; i < f.Length ; i++){
                     if(f[i].FieldType == typeof(bool  ))f[i].SetValueDirect(__makeref(Data), false);
                else if(f[i].FieldType == typeof(int   ))f[i].SetValueDirect(__makeref(Data), 0    );
                else if(f[i].FieldType == typeof(double))f[i].SetValueDirect(__makeref(Data), 0.0  );
                else if(f[i].FieldType == typeof(string))f[i].SetValueDirect(__makeref(Data), ""   );
            }
        }

         
        void LotData(string _sPath)
        {
            Folder = _sPath ;
        }
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
            string sPath = Folder + "LastWrkData.ini";
            if (_bLoad) CAutoIniFile.LoadStruct<T>(sPath, "Data", ref Data);
            else CAutoIniFile.SaveStruct<T>(sPath, "Data", ref Data);
        }

        public void SaveDataIni()
        {
            SaveDataIni(DateTime.Now.ToOADate());
        }
        public void SaveDataIni(double _dOaSaveDate)
        {
            SaveDataIni(_dOaSaveDate , ref Data);
        }
        public void SaveDataIni(double _dOaSaveDate , ref T _tData)
        {
            string sPath;
            DateTime tDateTime;
            tDateTime = DateTime.FromOADate(_dOaSaveDate);
            sPath = Folder + tDateTime.ToString("yyyyMMdd") + ".ini";

            ////기존에 있던것들 지우기.
            //DirectoryInfo di = new DirectoryInfo(Folder);
            //if (!di.Exists) di.Create();
            //foreach (FileInfo fi in di.GetFiles())
            //{
            //    //if (fi.Extension != ".log") continue;
            //    // 12개월 이전 로그를 삭제합니다.
            //    if (fi.CreationTime <= DateTime.Now.AddMonths(-12))
            //    {
            //        fi.Delete();
            //    }
            //}

            //기존에 있던것들 지우기.
            DirectoryInfo di = new DirectoryInfo(Folder);
            if (!di.Exists) di.Create();
            foreach (FileInfo fi in di.GetFiles())
            {
                //if (fi.Extension != ".log") continue;
                // 12개월 이전 로그를 삭제합니다.
                if (fi.CreationTime <= DateTime.Now.AddMonths(-12))
                {
                    fi.Delete();
                }
            }

            //카운트 저장.            
            int iCnt = 0;
            CIniFile IniGetErrCnt = new CIniFile(sPath);
            IniGetErrCnt.Load("ETC", "DataCnt", ref iCnt);//이거 여기서 저장하는데 Datamap에서도 가져다가 쓴다.
            int iAddedCnt = iCnt + 1;
            IniGetErrCnt.Save("ETC", "DataCnt", iAddedCnt);

            //데이터 저장.
            CAutoIniFile.SaveStruct<T>(sPath, iCnt.ToString(), ref _tData);
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
                sPath = Folder + d.ToString("yyyyMMdd") + ".ini";

                CIniFile IniGetCnt = new CIniFile(sPath);

                IniGetCnt.Load("ETC", "DataCnt", ref iCnt);
                iCntSum += iCnt;
            }
            return iCntSum;
        }
        ////이거 안쓰고 LoadDataList로 쓴다.
        //public bool LoadDataIni(DateTime _tSttData, DateTime _tEndData, T[] _tData)
        //{
        //    string sPath;

        //    int iDataCnt = 0;
        //    int iMaxDayCnt = 0;

        //    string sTemp;
        //    for (DateTime d = _tSttData; d <= _tEndData; d = d.AddDays(1))
        //    {
        //        DateTime SearchDate = d;
        //        sTemp = d.ToString();
        //        sPath = Folder + d.ToString("yyyyMMdd") + ".ini";
        //        //sPath = "D:\WrkLog\HSM_230DB\20170726.ini";
        //        iMaxDayCnt = GetDataCnt(SearchDate, SearchDate);
        //        //TData Data = new TData() ;
        //        for (int c = 0; c < iMaxDayCnt; c++)
        //        {
        //            CAutoIniFile.LoadStruct<T>(sPath, c.ToString(), ref _tData[iDataCnt]);
        //            //CAutoIniFile.LoadStruct<TData>(sPath , c.ToString(),ref Data);   
        //            iDataCnt++;
        //        }
        //    }
        //    return true;
        //}

        public bool LoadDataList(DateTime _tSttData, DateTime _tEndData, ref List<T> _lData)
        {
            string sPath;
            int iMaxDayCnt = 0;
            string sTemp;
            T Data = new T();
            for (DateTime d = _tSttData; d <= _tEndData; d = d.AddDays(1))
            {
                DateTime SearchDate = d;
                sTemp = d.ToString();
                sPath = Folder + d.ToString("yyyyMMdd") + ".ini";//sPath = "D:\WrkLog\HSM_230DB\20170726.ini";                
                iMaxDayCnt = GetDataCnt(SearchDate, SearchDate);
                for (int c = 0; c < iMaxDayCnt; c++)
                {
                    CAutoIniFile.LoadStruct<T>(sPath, c.ToString(), ref Data);
                    _lData.Add(Data);
                }
            }
            return true;
        }

        //일단 안만듬.
        public void SortErrData(bool _bNumberTime , int _iDataCnt , T[] _tData)
        {
            //TData _tTempData ;
            //if(_bNumberTime) { //숫자 우선.
            //    for(int i = 0; i < _iDataCnt; i++) { //버블버블 버블팝 버블버블 팝팝!!
            //       for(int j = 0; j < _iDataCnt -i - 1; j++) {
            //          if(_tData[j].LotNo > _tData[j + 1].LotNo) {
            //             _tTempData = _tData[j];
            //             _tData[j] = _tData [j + 1];
            //             _tData[j + 1] = _tTempData;
            //          }
            //       }
            //    }
            //}
            //else { //시간 우선.
            //    for(int i = 0; i < _iDataCnt; i++) { //버블버블 버블팝 버블버블 팝팝!!
            //       for(int j = 0; j < _iDataCnt -i - 1; j++) {
            //          if(_tData[j].StartTime > _tData[j + 1].StartTime) {
            //             _tTempData = _tData[j];
            //             _tData[j] = _tData [j + 1];
            //             _tData[j + 1] = _tTempData;
            //          }
            //       }
            //    }
            //}
        }

        //디스플레이 및 딜리트에서 같이 써서 빼놓음.
        private string GetSValue(string _sName , object _oValue)
        {
            double dValue = 0.0;
            string sValue = "";

            if (_sName.IndexOf("edAt") >= 0) //시각 StartedAt, EndedAt  delete 할때 EndedAt을 쓰니깐 조심.
            {
                dValue = (double)_oValue;
                sValue = DateTime.FromOADate(dValue).ToString("yyyy-MM-dd HH:mm:ss");
            }
            //else if (_sName.IndexOf("Span") >= 0) //날짜기준.
            //{
            //    dValue = (double)_oValue;
            //    //sValue = TimeSpan.FromDays(dValue).Hours.ToString(TimeSpan.FromDays(dValue).Hours.ToString("g"));
            //    TimeSpan Span ;
            //    try{
            //        Span = TimeSpan.FromDays(dValue);
            //    }
            //    catch(Exception ex){          
            //        Span = TimeSpan.FromMilliseconds(0);
            //    }
            //    sValue = string.Format("{0:00}.{1:00}:{2:00}:{3:00}" , Span.Days , Span.Hours , Span.Minutes , Span.Seconds);
            //    //sValue = dSpan.ToString("g");
            //}
            else if (_sName.IndexOf("Time") >= 0) //시간 
            {
                dValue = (double)_oValue;
                //sValue = TimeSpan.FromDays(dValue).Hours.ToString(TimeSpan.FromDays(dValue).Hours.ToString("g"));
                TimeSpan Span ;
                try{
                    Span = TimeSpan.FromMilliseconds(dValue);
                }
                catch(Exception ex){          
                    Span = TimeSpan.FromMilliseconds(0);
                }
                sValue = string.Format("{0:00}.{1:00}:{2:00}:{3:00}" , Span.Days , Span.Hours , Span.Minutes , Span.Seconds);
                //sValue = dSpan.ToString("g");
            }
            else
            {
                sValue = _oValue.ToString();
            }
            return sValue ;
        }

        public bool TryParse (string _sText , out TimeSpan _tsOut)
        {
            if(TimeSpan.TryParse(_sText ,out _tsOut) ) return true ;

            if (TimeSpan.TryParseExact(_sText, @"dd.HH:mm" , CultureInfo.InvariantCulture, TimeSpanStyles.None, out _tsOut))
            {
                return true ;
            }

            return false ;
        }

        public void DispData(DateTime _tSttData, DateTime _tEndData, ListView _lvTable) //true면 에러넘버 , false면 시간.
        {
            if (_lvTable == null) return;

            _lvTable.BeginUpdate();
            int iDatsCnt = GetDataCnt(_tSttData, _tEndData);
            //T[] Datas = new T[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);
            List<T> Datas = new List<T>();
            LoadDataList(_tSttData, _tEndData, ref Datas);

            //SortErrData(_bNumberTime , iErrCnt , Datas) ;

            string sPath = Folder;
            string sSerchFile = sPath + "\\*.ini";

            _lvTable.Clear();
            _lvTable.View = View.Details;
            _lvTable.LabelEdit = true;
            _lvTable.AllowColumnReorder = true;
            _lvTable.FullRowSelect = true;
            _lvTable.GridLines = true;
            //_lvTable.Sorting = SortOrder.Descending;
            _lvTable.Scrollable = true;

            Type type = typeof(T);
            int iCntOfItem = type.GetProperties().Length;
            FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //컬럼추가 하고 이름을 넣는다.
            _lvTable.Columns.Add("No", 100, HorizontalAlignment.Left);
            for (int c = 0; c < f.Length; c++)
            {
                _lvTable.Columns.Add(f[c].Name, 100, HorizontalAlignment.Left);
            }


            _lvTable.Items.Clear();
            string sValue = "";
            string sName = "";
            ListViewItem[] liitem = new ListViewItem[iDatsCnt];
            for (int r = 0; r < iDatsCnt; r++)
            {
                liitem[r] = new ListViewItem(string.Format("{0}", r));
                for (int c = 0; c < f.Length; c++)
                {
                    sName = f[c].Name;
                    sValue = GetSValue(sName , f[c].GetValue(Datas[r]));
                    liitem[r].SubItems.Add(sValue);
                }
                liitem[r].UseItemStyleForSubItems = false;
                _lvTable.Items.Add(liitem[r]);
            }
            //_lvTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            if(iDatsCnt == 0) _lvTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize   );
            else              _lvTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            _lvTable.EndUpdate();
        }

        //여기는 FormSPC 완료 되면 할꺼
        public bool DeleteSelItems(ListView _lvTable) //true면 에러넘버 , false면 시간.
        {
            if (_lvTable == null) return false;            

            //로그저장기준이 EndedAt시간이므로 
            //EndedAt의 행을 찾는다.
            int iCol = -1 ;
            for (int c = 0; c < _lvTable.Columns.Count; c++)
            {
                if (_lvTable.Columns[c].Text == "StartedAt")
                {
                    iCol = c ;
                    break ;
                }
            }
            if(iCol < 0) return false ;
 
            DateTime FirstDate = new DateTime() , Date = new DateTime();

            int iSelCnt = _lvTable.SelectedItems.Count ;
            List<T> Datas = new List<T>();
            FieldInfo[] f ; 
            string      sTable ;
            string      sValue ;
            string      sName  ;               
            bool        bSame = true ;

            //선택된 모든 아이템을 
            //여기 와일문 조건 다시 봐야 함....
            //리스트뷰로 하면 안됨.
            List<ListViewItem> SelectedData = new List<ListViewItem>();
            foreach(ListViewItem Item in _lvTable.SelectedItems)
            {
                SelectedData.Add(Item);
            }
            
            List<ListViewItem> SameDayData  = new List<ListViewItem>();
            while(SelectedData.Count>0){
                SameDayData.Clear();
                if (!DateTime.TryParse(SelectedData[0].SubItems[iCol].Text, out FirstDate)) return false ;
                //foreach(ListViewItem Item in SelectedData){
                for(int i = 0 ; i< SelectedData.Count ; i++){
                    if (!DateTime.TryParse(SelectedData[i].SubItems[iCol].Text, out Date)) return false ;
                    if (FirstDate.ToString("yyyyMMdd")  == Date.ToString("yyyyMMdd"))
                    {
                        SameDayData.Add(SelectedData[i]);
                        SelectedData.RemoveAt(i);
                    }
                }

                //선택된것들 중에 같은 날 인것들.
                Datas.Clear();
                LoadDataList(Date, Date, ref Datas);
                for (int i = 0; i < SameDayData.Count; i++)
                {
                    //데이터를 하나씩 꺼내서.
                    foreach (T Data in Datas)
                    {
                        f = Data.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        bSame = true;
                        for (int j = 0; j < f.Length; j++)
                        {
                            //sKey   = f[j].Name ;                            
                            sTable = SameDayData[i].SubItems[j + 1].Text;
                            sValue = f[j].GetValue(Data).ToString();
                            sName = f[j].Name;
                            sValue = GetSValue(sName, f[j].GetValue(Data));

                            if (sTable != sValue)
                            {
                                bSame = false;
                                break;
                            }
                        }
                        if (bSame)
                        {//같은놈 찾았으면 지우고.
                            Datas.Remove(Data);
                            break;
                        }
                    }                    
                }
                //날짜에서 데이터 빼고 여기서 다시 저장.
                //위에서 먼저 화일 지우고 다시 하면 좋지만 
                //화일 지우고 list Delete하다가 뻑나면 파일이 날라가서 안전빵으로 여기서 한다.
                string sPath = Folder + Date.ToString("yyyyMMdd") + ".ini";//sPath = "D:\WrkLog\HSM_230DB\20170726.ini";                
                File.Delete(sPath);
                foreach (T Data in Datas)
                {
                    T TempData = Data;
                    SaveDataIni(Date.ToOADate(), ref TempData);
                }
                         
            }
            return true ;
        }

        public void SaveCsv(ListView _lvTable)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "csv File|*.csv";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();

                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        string sTmp = "";
                        for (int j = 0; j < _lvTable.Columns.Count ; j++)
                            sTmp += _lvTable.Columns[j].Text + ", ";
                        sTmp += "\n";
                        Byte[] Bytes = Encoding.UTF8.GetBytes(sTmp);
                        Byte[] Dest = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("euc-kr"), Bytes); 
                        fs.Write(Dest, 0, Dest.Length);


                        for (int i = 0; i < _lvTable.Items.Count; i++)
                        {
                            sTmp = "";
                            for (int j = 0; j < _lvTable.Items[i].SubItems.Count; j++)
                                sTmp += _lvTable.Items[i].SubItems[j].Text + ", ";
                            sTmp += "\n";
                            Bytes = Encoding.UTF8.GetBytes(sTmp);
                            Dest = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("euc-kr"), Bytes); 
                            fs.Write(Dest, 0, Dest.Length);
                        }
                        break;
                }
                fs.Close();
            }
        }

        

    }

    public static class SPC
    {
        public static CErr     ERR = new CErr    ("d:\\SpcLog\\"+ Eqp.sEqpName  + "\\ErrLog\\"    );   
        public static CLot     LOT = new CLot    ("d:\\SpcLog\\"+ Eqp.sEqpName  + "\\LotLog\\"    );
        public static CDay     DAY = new CDay    ("d:\\SpcLog\\"+ Eqp.sEqpName  + "\\DayLog\\"    );
        public static CFailure FLR = new CFailure("d:\\SpcLog\\"+ Eqp.sEqpName  + "\\FailureLog\\");   

        public static CMapData MAP = new CMapData();

        public static void Init()
        {
            ERR.Init();
            LOT.Init();
            DAY.Init();
            FLR.Init();
            
        }
        
        public static void Close()
        {
            ERR.Close();
            LOT.Close();
            DAY.Close();
            FLR.Close();
        }
        
        public static void Update(string _sCrntLotNo , string _sCrntDev , EN_SEQ_STAT Stat , bool _bMaint)
        {
            ERR.Update(_sCrntLotNo,Stat);
            LOT.Update(_sCrntLotNo,Stat , _sCrntDev , _bMaint);
            DAY.Update(Stat , _sCrntDev , _bMaint);
            FLR.Update(_sCrntLotNo,Stat , _bMaint);
        }


    }
}
