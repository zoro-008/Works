using COMMON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Control
{
    class CRun
    {
        //public delegate void Chart_AddPoints(dynamic _iNo, dynamic _iSeries, dynamic _dX, dynamic _dY);
        public delegate void Chart_AddPoints12(string _dX, string _dY);
        public delegate void Chart_AddPoints3 (string _dX, string _dY);
        public delegate void Chart_AddPoints4 (string _dX, string _dY);
        public delegate void Chart_Save12     (string _FileName);
        public delegate void Chart_Save3      (string _FileName);
        public delegate void Chart_Save4      (string _FileName);
        public delegate void Chart_Clear    ();
        public delegate void Chart_Begin    ();
        public delegate void Chart_End      ();

        public event Chart_AddPoints12 CAddPoints12;
        public event Chart_AddPoints3  CAddPoints3 ;
        public event Chart_AddPoints4  CAddPoints4 ;
        public event Chart_Clear       CClear  ;
        public event Chart_Begin       CBegin  ;
        public event Chart_End         CEnd    ;
        public event Chart_Save12      CSave12 ;
        public event Chart_Save3       CSave3  ;
        public event Chart_Save4       CSave4  ;


        //Timer
        protected CDelayTimer m_tmCycle;
        protected CDelayTimer m_tmTemp ;

        private int iCycle;
        private int iPreCycle;
        private int i1 , i2;
        private int iPos1 , iPos2 ;
        private int m_iMeasureCnt;
        private string sPath;
        public int  iNowCnt;
        
        //Alloc
        private const int m_iMotrNo = 0 ;
        public const int iTotalStroke   = 211;
        public const int iMeasureMaxCnt = 30;
        public static string[,] sOut1 = new string[iTotalStroke,iMeasureMaxCnt];
        public static string[,] sOut2 = new string[iTotalStroke,iMeasureMaxCnt];
        public static string[,] sOut3 = new string[iTotalStroke,iMeasureMaxCnt];
        public static string[,] sOut4 = new string[iTotalStroke,iMeasureMaxCnt];

        //Err Check
        public static string sErrMsg = "";

        //AutoRun
        private Thread MainThread ;//= new Thread(new ThreadStart(Update));
        public bool bRun     ;
        public bool bPreRun  ;
        public bool bReqStop ;

        public CRun()
        {
            m_tmCycle     = new CDelayTimer();
            m_tmTemp      = new CDelayTimer();
            MainThread = new Thread(new ThreadStart(Update));
            MainThread.Priority = ThreadPriority.Highest;
            MainThread.Start();

            sPath = "";
        }

        public void Close()
        {
            MainThread.Abort();
            MainThread.Join();
        }
        public void ClearData()
        {
            m_iMeasureCnt = 0;
            for (int i = 0; i < iTotalStroke; i++)
            {
                for (int j = 0; j < iMeasureMaxCnt; j++)
                {
                    sOut1[i,j] = "";
                    sOut2[i,j] = "";
                    sOut3[i,j] = "";
                    sOut4[i,j] = "";
                }
            }
        }

        public void GetData(int _iPos, int _iNo)
        {
            sErrMsg = "" ;
            if (_iPos < 0 || _iPos > iTotalStroke  ) { sErrMsg = "센서 데이터 값 받아오기중 포지션의 범위를 이탈"; return; }
            if (_iNo < 0  || _iNo  > iMeasureMaxCnt) { sErrMsg = "센서 데이터 값 받아오기중 최대갯수 범위를 이탈"; return; }
            LS9IF_MEASURE_DATA stMeasureData = new LS9IF_MEASURE_DATA();

            int rc = NativeMethods.LS9IF_GetMeasurementValue(ref stMeasureData);
            if (rc != 0) { sErrMsg = "센서 데이터 값 받아오기 실패"; return; }

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < stMeasureData.stMesureValue.Length; i++)
            {
                if (i == 0)
                {
                    sOut1[_iPos, _iNo] = stMeasureData.stMesureValue[i].fValue.ToString("0.##########");
                    if (stMeasureData.stMesureValue[i].fValue == -10000000000) sOut1[_iPos, _iNo] = "X";
                }
                if (i == 1)
                {
                    sOut2[_iPos, _iNo] = stMeasureData.stMesureValue[i].fValue.ToString("0.##########");
                    if (stMeasureData.stMesureValue[i].fValue == -10000000000) sOut2[_iPos, _iNo] = "X";
                }
                if (i == 2)
                {
                    sOut3[_iPos, _iNo] = stMeasureData.stMesureValue[i].fValue.ToString("0.##########");
                    if (stMeasureData.stMesureValue[i].fValue == -10000000000) sOut3[_iPos, _iNo] = "X";
                }
                if (i == 3)
                {
                    sOut4[_iPos, _iNo] = stMeasureData.stMesureValue[i].fValue.ToString("0.##########");
                    if (stMeasureData.stMesureValue[i].fValue == -10000000000) sOut4[_iPos, _iNo] = "X";
                }

            }

        }

        public void SaveCsv(string _sPath, int _iPos,int _iCnt)
        {
            sErrMsg = "" ;

            int iPos = _iPos; // 10 ;
            if (iPos < 0 || iPos  > iTotalStroke  ) { sErrMsg = "센서 데이터 저장중 포지션의 범위를 이탈"; return; }
            if (_iCnt< 0 || _iCnt > iMeasureMaxCnt) { sErrMsg = "센서 데이터 저장중 최대갯수 범위를 이탈"; return; }

            string sPath = _sPath;// @"D:\Data\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".csv";
            string sDir = Path.GetDirectoryName(sPath + "\\");
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;

            string line = "";
            if (!File.Exists(sPath))
            {
                line = "Position,";
                if (OM.CmnOptn.bOut1) line += "Out1,";
                if (OM.CmnOptn.bOut2) line += "Out2,";
                if (OM.CmnOptn.bOut3) line += "Out3,";
                if (OM.CmnOptn.bOut4) line += "Out4,";
                line += "\r\n";
            }
            FileStream fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            
            
            for (int i = 0; i < _iCnt; i++)
            {
                line += _iPos + ",";
                if (OM.CmnOptn.bOut1) line += sOut1[iPos,i] + ",";
                if (OM.CmnOptn.bOut2) line += sOut2[iPos,i] + ",";
                if (OM.CmnOptn.bOut3) line += sOut3[iPos,i] + ",";
                if (OM.CmnOptn.bOut4) line += sOut4[iPos,i] + ",";
                line += "\r\n";
            }

            //sw.WriteLine(line);
            sw.Write(line);
            sw.Close();
            fs.Close();
        }

        public void Update()
        {
            while (true)
            {
                Thread.Sleep(0);
                if(bRun && bPreRun == false)
                {
                    iCycle    = 10 ;
                    iPreCycle = 0  ;
                    bReqStop  = false;
                    sErrMsg   = "" ;
                    CClear();
                    //CBegin();

                }
                bPreRun = bRun ;

                if(bRun)
                {
                         if(OM.Mode.iMode == Mode.AutoMode1 ) { if(CycleAuto1 ()) { bRun = false; /*CEnd();*/} }
                    else if(OM.Mode.iMode == Mode.AutoMode2 ) { if(CycleAuto2 ()) { bRun = false; /*CEnd();*/} }
                    else if(OM.Mode.iMode == Mode.ManualMode) { if(CycleManual()) { bRun = false; /*CEnd();*/} }
                }

                Util.Update();
            }
        }

        public bool SetStoragePoints(int _iCnt = 400000)
        {
            byte byDepth = 0x02;
            LS9IF_TARGET_SETTING stTargetSetting = new LS9IF_TARGET_SETTING();
            stTargetSetting.byType = 0x10;
            stTargetSetting.byCategory = 0x05;
            stTargetSetting.byItem = 0x02;
            stTargetSetting.byTarget = 0x00;

            byte[] pbyDatas1 = new byte[128];
            int dwDataSize = 0;

            int rc = NativeMethods.LS9IF_GetSetting(byDepth, stTargetSetting, pbyDatas1, ref dwDataSize);
            if (rc != (int)Rc.Ok) { sErrMsg = "검사 데이터 갯수 받아오기 실패"; return false; }
            
            int storagePoints = BitConverter.ToInt32(pbyDatas1, 0);
            if(storagePoints != _iCnt)
            {
                storagePoints = _iCnt;
                byte[] pbyDatas2 = BitConverter.GetBytes(storagePoints);

                uint pdwError = 0;

                rc = NativeMethods.LS9IF_SetSetting(byDepth, stTargetSetting, pbyDatas2.Length, pbyDatas2, ref pdwError);
                if (rc != (int)Rc.Ok) { sErrMsg = "검사 데이터 갯수 설정 실패"; return false; }

                if (pdwError == 0)
                {
                    return true ; //result= "OK";
                }
                else
                {
                    sErrMsg = "검사 데이터 갯수 설정 실패"; return false; //result = "Error code갌0x" + pdwError.ToString("x8");
                }

            }
            return true;
        }

        public bool StorageClear()
        {
            int rc = NativeMethods.LS9IF_ClearMemory();
            if (rc != (int)Rc.Ok) { sErrMsg = "검사 데이터 클리어 실패"; return false; }
            return true;
        }

        public bool StorageStart()
        {
            int rc = NativeMethods.LS9IF_StartStorage();
            if (rc != (int)Rc.Ok) { sErrMsg = "검사 시작 실패"; return false; }
            return true;
        }

        public bool StorageStop()
        {
            int rc = NativeMethods.LS9IF_StopStorage();
            if (rc != (int)Rc.Ok) { sErrMsg = "검사 중지 실패"; return false; }
            return true;
        }

        public int GetStorageCnt()
        {
            LS9IF_STORAGE_INFO stStorageInfo = new LS9IF_STORAGE_INFO();

            int rc = NativeMethods.LS9IF_GetStorageStatus(ref stStorageInfo);
            if (rc != (int)Rc.Ok) { sErrMsg = "현재 검사중인 데이터 갯수 받아오기 실패"; return 0; }

            return (int)stStorageInfo.dwStorageCnt;
            //StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.AppendLine("Status:" + stStorageInfo.byStatus.ToString());
            //stringBuilder.AppendLine("Version:" + stStorageInfo.wStrageVer.ToString());
            //stringBuilder.AppendLine("Number:" + stStorageInfo.dwStorageCnt.ToString());

            //OutPutResult(_grpDataStorage.Text, _btnGetStorageStatus.Text, stringBuilder.ToString());
        }

        public void GetData_SaveCsv(string _sPath, int _iPos, int _iNo, int _iCnt)
        {
            iNowCnt = 0;
            CBegin();
            sErrMsg = "" ;
            if (_iPos < 0 || _iPos > iTotalStroke  ) { sErrMsg = "센서 데이터 값 받아오기중 포지션의 범위를 이탈"; return; }
            if (_iNo < 0  || _iNo  >=iMeasureMaxCnt) { sErrMsg = "센서 데이터 값 받아오기중 최대갯수 범위를 이탈"; return; }

            ///
            string sPath = _sPath + "\\" + string.Format("{0:000}",_iPos) + "_" + string.Format("{0:00}",_iNo + 1) + ".csv";
            string sPathJpg12 = _sPath + "\\" + string.Format("{0:000}",_iPos) + "_" + string.Format("{0:00}",_iNo + 1) + "_OUT12.Jpg";
            string sPathJpg3  = _sPath + "\\" + string.Format("{0:000}",_iPos) + "_" + string.Format("{0:00}",_iNo + 1) + "_OUT3.Jpg";
            string sPathJpg4  = _sPath + "\\" + string.Format("{0:000}",_iPos) + "_" + string.Format("{0:00}",_iNo + 1) + "_OUT4.Jpg";
            string sDir  = Path.GetDirectoryName(sPath) + "\\" ;//
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) { sErrMsg = "결과값 폴더 생성 실패"; return; }

            string line = "";
            if (!File.Exists(sPath))
            {
                line = "Time,";
                if (OM.CmnOptn.bOut1) line += "OUT1,";
                if (OM.CmnOptn.bOut2) line += "OUT2,";
                if (OM.CmnOptn.bOut3) line += "OUT3,";
                if (OM.CmnOptn.bOut4) line += "OUT4,";
                line += ",,OUT3,OUT4,";
                line += "\r\n";
            }

            FileStream fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            //sw.WriteLine(line);
            sw.Write(line);

            ///
            uint iCnt         = (uint)_iCnt;
            uint readStart    = 0          ;
            uint storageCount = 4500       ;
            uint readIndex    = 0          ;

            LS9IF_STORAGE_DATA[] stStorageData = new LS9IF_STORAGE_DATA[storageCount];

            //int rc = NativeMethods.LS9IF_GetStorageData(readStart, storageCount, stStorageData);
            //if (rc != 0) { sErrMsg = "센서 데이터 값 받아오기 실패"; return; }

            List<string> lst  =  new List<string>();
            ///평균값, 최대값, 최소값, CV, 표준분포
            List<float> lst3 =  new List<float>();
            List<float> lst4 =  new List<float>();
            string line1, line2, line3, line4, line5;
            ///
            bool b1 = iCnt % storageCount > 0 ;
            uint i1 = iCnt / storageCount     ;
            if(iCnt > storageCount && b1) i1 += 1 ;
            if(i1 ==0) i1 = 1;

            bool bOut1  = false;
            bool bFirst = true ;
            for (uint i = 0; i < i1; i++)
            {
                readStart = storageCount * i ;
                
                if(b1 && i == i1 - 1)
                {
                    storageCount = iCnt % storageCount ;
                }
                int rc = NativeMethods.LS9IF_GetStorageData(readStart, storageCount, stStorageData);
                if (rc != 0) { sErrMsg = "센서 데이터 값 받아오기 실패"; return; }

                for (uint j = 0; j < storageCount; j++)
                {
                    iNowCnt = (int)readStart + (int)j ;
                    //readIndex = readStart + j ;
                    readIndex = j ;
                    string dateTimeString;
                    try
                    {
                        DateTime datetime = new DateTime(2000 + stStorageData[readIndex].byYear, stStorageData[readIndex].byMonth, stStorageData[readIndex].byDay, 
                            stStorageData[readIndex].byHour, stStorageData[readIndex].byMinute, stStorageData[readIndex].bySecond, stStorageData[readIndex].byMillsecond);
                        dateTimeString = datetime.ToString("MM/dd/yyyy hh:mm:ss.fff tt", CultureInfo.InvariantCulture) ;//datetime.ToString();
                    }
                    catch (Exception)
                    {
                        dateTimeString = "Invalid Date";
                    }

                    //if(bFirst)
                    //{
                        line = dateTimeString + ",";
                        bOut1= false;

                        for (int k = 0; k < stStorageData[readIndex].stStorageValue.Length; k++)
                        {
                            //if (stStorageData[readIndex].stStorageValue[k].fValue != -10000000000)
                            //{
                                if (k == 0)
                                {
                                    sOut1[_iPos, _iNo] = stStorageData[readIndex].stStorageValue[k].fValue.ToString("0.##########");
                                    //if (stStorageData[readIndex].stStorageValue[k].fValue == -10000000000) sOut1[_iPos, _iNo] = "X";
                                    if (OM.CmnOptn.bOut1) {
                                        line += sOut1[_iPos,_iNo] + ",";
                                        if (stStorageData[readIndex].stStorageValue[k].fValue != -10000000000)
                                        {
                                            //CAddPoints(2,0,dateTimeString,sOut1[_iPos,_iNo]);
                                            bOut1 = true;
                                            //CAddPoints(dateTimeString,sOut1[_iPos,_iNo]);
                                        }
                                    }
                                }
                                if (k == 1)
                                {
                                    sOut2[_iPos, _iNo] = stStorageData[readIndex].stStorageValue[k].fValue.ToString("0.##########");
                                    //if (stStorageData[readIndex].stStorageValue[k].fValue == -10000000000) sOut2[_iPos, _iNo] = "X";
                                    if (OM.CmnOptn.bOut2) {
                                        line += sOut2[_iPos,_iNo] + ",";
                                        if (stStorageData[readIndex].stStorageValue[k].fValue != -10000000000)
                                        {                                
                                            if(bOut1) CAddPoints12(sOut1[_iPos,_iNo],sOut2[_iPos,_iNo]);
                                            //CAddPoints(dateTimeString,sOut2[_iPos,_iNo]);
                                        }
                                    }
                                }
                                if (k == 2)
                                {
                                    sOut3[_iPos, _iNo] = stStorageData[readIndex].stStorageValue[k].fValue.ToString("0.##########");
                                    //if (stStorageData[readIndex].stStorageValue[k].fValue == -10000000000) sOut3[_iPos, _iNo] = "X";
                                    if (OM.CmnOptn.bOut3) {
                                        line += sOut3[_iPos,_iNo] + ",";
                                        if (stStorageData[readIndex].stStorageValue[k].fValue != -10000000000) {
                                            CAddPoints3(dateTimeString,sOut3[_iPos,_iNo]);
                                            lst3.Add(stStorageData[readIndex].stStorageValue[k].fValue);
                                        }
                                    }
                                }
                                if (k == 3)
                                {
                                    sOut4[_iPos, _iNo] = stStorageData[readIndex].stStorageValue[k].fValue.ToString("0.##########");
                                    //if (stStorageData[readIndex].stStorageValue[k].fValue == -10000000000) sOut4[_iPos, _iNo] = "X";
                                    if (OM.CmnOptn.bOut4) {
                                        line += sOut4[_iPos,_iNo] + ",";
                                        if (stStorageData[readIndex].stStorageValue[k].fValue != -10000000000) {
                                            CAddPoints4(dateTimeString,sOut4[_iPos,_iNo]);
                                            lst4.Add(stStorageData[readIndex].stStorageValue[k].fValue);
                                        }
                                    }
                                }
                            //}
                        }

                        if(j == 0) line1 = line ;
                        if(j == 1) line2 = line ;
                        if(j == 2) line3 = line ;
                        if(j == 3) line4 = line ;
                        if(j == 4) line5 = line ;
                        //line += "\r\n";
                        lst.Add(line);
                        //sw.Write(line);
                        
                        //bFirst = false;
                    //}
                }
            }
                      
            //차트 이미지 업데이트
            CEnd(); 

            //차트 이미지 저장하기
            CSave12(sPathJpg12);
            CSave3 (sPathJpg3 );
            CSave4 (sPathJpg4 );

            //최대최소평균표준편차cv구하기
            StreamWriter swsub = new StreamWriter(fs, Encoding.UTF8);

            float dMin3 = 0f, dMin4 = 0f;
            float dMax3 = 0f, dMax4 = 0f;
            float dAvr3 = 0f, dAvr4 = 0f;
            float dCnt3 = 0f, dCnt4 = 0f;
            float dStd3 = 0f, dStd4 = 0f;
            float dCv3  = 0f, dCv4  = 0f;
            float dVar  = 0f;
            if(lst3.Count > 0 && lst4.Count > 0)
            { 
                dMin3 = lst3.Min();
                dMin4 = lst4.Min();

                dMax3 = lst3.Max();
                dMax4 = lst4.Max();
                
                dAvr3 = lst3.Average();
                dAvr4 = lst4.Average();

                dCnt3  = lst3.Count;
                dCnt4  = lst4.Count;

                dVar = 0f;
                for(int i = 0; i < dCnt3; i++) dVar += (float)Math.Pow(lst3[i] - dAvr3, 2);
                if(dCnt3 > 0) dStd3 = (float)Math.Sqrt(dVar / dCnt3);
                dVar = 0f;
                for(int i = 0; i < dCnt4; i++) dVar += (float)Math.Pow(lst4[i] - dAvr4, 2);
                if(dCnt4 > 0) dStd4 = (float)Math.Sqrt(dVar / dCnt4);
                
                if(dAvr3 != 0) dCv3 = dStd3 /dAvr3*100 ;
                if(dAvr4 != 0) dCv4 = dStd4 /dAvr4*100 ;
            }

            if(lst.Count > 0)
            {
                //수정
                lst[0] += ",MIN," + dMin3.ToString() + "," + dMin4.ToString();
                lst[1] += ",MAX," + dMax3.ToString() + "," + dMax4.ToString();
                lst[2] += ",AVR," + string.Format("{0:0.000}",dAvr3) + "," + string.Format("{0:0.000}",dAvr4);
                lst[3] += ",STD," + string.Format("{0:0.000}",dStd3) + "," + string.Format("{0:0.000}",dStd4);
                lst[4] += ",CV,"  + string.Format("{0:0.000}",dCv3 ) + "," + string.Format("{0:0.000}",dCv4 ); 
                
                //저장 위에서 \r\n 붙여서
                for(int i = 0; i < lst.Count; i++) sw.WriteLine(lst[i]);
            }
            //종료
            sw.Close();
            fs.Close();
      
            
        }

        public bool CycleAuto1()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(iCycle != 0 && iCycle == iPreCycle && CheckStop() , 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", iCycle);
                Log.ShowMessage("Error",sTemp);
                return true;
            }

            if (iCycle != iPreCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", iCycle);
                Log.Trace(sTemp);
            }

            if(sErrMsg != "")
            {
                Util.MT.StopAll();
                Log.ShowMessage("Error",sErrMsg);
                iCycle = 0 ;
                return true;
            }

            if(bReqStop)
            {
                StorageStop();
                Util.MT.Stop(0);
                iCycle = 0 ;
                return true;
            }

            iPreCycle = iCycle;
            sErrMsg   = "";
            bReqStop  = false;

            switch (iCycle)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", iCycle);
                    Log.Trace(sTemp);
                    return true;

                case 10:
                    sPath = @"D:\Data\" + DateTime.Now.ToString("yyyy-MM-dd") +@"\" + DateTime.Now.ToString("hh-mm-ss") ;
                    Util.MT.StopAll();
                    //1.0.0.1
                    //iPos1 = (OM.Auto1.iAStrokeStart - OM.Auto1.iAStrokeEnd);
                    iPos1 = (OM.Auto1.iAStrokeEnd - OM.Auto1.iAStrokeStart);
                    if(iPos1 <= 0                 ) sErrMsg = "Check the Stroke Start , End Position";
                    if(OM.Auto1.iAInterval <= 0 || 
                       OM.Auto1.iAInterval > iPos1) sErrMsg = "Check the Interval Value";

                    i1 = 0;
                    i2 = (iPos1 / OM.Auto1.iAInterval) + 1 ; //i1~i2
                    m_iMeasureCnt = 0; //현재 검사수.
                    ClearData();

                    SetStoragePoints();
                    //StorageClear();

                    iCycle++;
                    return false;

                case 11:
                    if(!Util.MT.GetStop(m_iMotrNo)) return false;
                    //1.0.0.1
                    //iPos2 = OM.Auto1.iAStrokeStart - OM.Auto1.iAInterval*i1; //이동할 포지션.
                    iPos2 = OM.Auto1.iAStrokeStart + OM.Auto1.iAInterval*i1; //이동할 포지션.
                    Util.MT.GoAbsRun(m_iMotrNo, iPos2*10);
                    iCycle++;
                    return false;

                case 12:
                    if(!Util.MT.GetStop(m_iMotrNo)) return false;
                    m_tmTemp.Clear();
                    iCycle++;
                    return false;

                case 13:
                    if(!m_tmTemp.OnDelay(OM.Auto1.iAResidenceTime)) return false;
                    iCycle++;
                    return false;

                case 14:
                    iCycle++;
                    return false;

                case 15:
                    StorageClear(); 
                    StorageStart();
                    m_tmTemp.Clear();

                    iCycle++;
                    return false;

                case 16: 
                    iCycle++;
                    return false;

                case 17: 
                    if(!m_tmTemp.OnDelay(OM.Auto1.iAMeasureTime)) {
                        iCycle = 16;
                        return false;
                    }
                    StorageStop();
                    if(OM.Auto1.iAMeasureTime > 0)
                    {
                        GetData_SaveCsv(sPath,iPos2, m_iMeasureCnt,GetStorageCnt());
                    }
                    m_iMeasureCnt++;

                    if(m_iMeasureCnt < OM.Auto1.iAMeasureCnt)
                    {
                        iCycle = 15;
                        return false;
                    }
                    m_iMeasureCnt = 0;
                    //SaveCsv(sPath, iPos2, OM.Auto1.iAMeasureCnt);
                    
                    i1 ++;
                    if (i1 < i2)
                    {
                        iCycle = 11;
                        return false;

                    }
                    iCycle = 0 ;
                    return true;

            }
        }

        public bool CycleAuto2()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(iCycle != 0 && iCycle == iPreCycle && CheckStop() , 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", iCycle);
                Log.ShowMessage("Error",sTemp);
                return true;
            }

            if (iCycle != iPreCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", iCycle);
                Log.Trace(sTemp);
            }

            if(sErrMsg != "")
            {
                StorageStop();
                Util.MT.StopAll();
                Log.ShowMessage("Error",sErrMsg);
                iCycle = 0 ;
                return true;
            }

            if(bReqStop)
            {
                Util.MT.Stop(0);
                iCycle = 0 ;
                return true;
            }

            iPreCycle = iCycle;
            sErrMsg   = "";
            bReqStop  = false;
            switch (iCycle)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", iCycle);
                    Log.Trace(sTemp);
                    return true;

                case 10:
                    sPath = @"D:\Data\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\" + DateTime.Now.ToString("hh-mm-ss") ;
                    Util.MT.StopAll();
                    //1.0.0.1
                    //iPos1 = iTotalStroke;
                    //iPos2 = 0;
                    //i1 = iTotalStroke - 1;
                    iPos1 = 0;
                    iPos2 = iTotalStroke;
                    i1 = iPos1;

                    m_iMeasureCnt = 0; //현재 검사수.
                    ClearData();

                    SetStoragePoints();
                    //StorageClear();

                    iCycle++;
                    return false;

                case 11:
                    if(!Util.MT.GetStop(m_iMotrNo)) return false;
                    //갈지말지 선택하기
                    if(OM.Auto2[i1].iBTime == 0 && OM.Auto2[i1].iATime == 0)
                    { //안가겟어
                        //if (i1 < iPos2) i1++;
                        //1.0.0.1
                        //if (i1 > 0) i1--;
                        if (i1 < iTotalStroke - 1) i1++;
                        else
                        {
                            iCycle = 0;
                            return true;
                        }
                    }
                    iCycle++;
                    return false;

                case 12:
                    if(!Util.MT.GetStop(m_iMotrNo)) return false;
                    if(OM.Auto2[i1].iBTime == 0 && OM.Auto2[i1].iATime == 0)
                    { 
                        iCycle = 11;
                        return false;
                    }
                    iCycle = 20;
                    return false;

                case 20:
                    if(!Util.MT.GetStop(m_iMotrNo)) return false;
                    //1.0.0.1
                    //iPos1 -= i1 ;
                    iPos1 += i1 ;
                    Util.MT.GoAbsRun(m_iMotrNo, i1 * 10);
                    iCycle++;
                    return false;

                case 21:
                    if(!Util.MT.GetStop(m_iMotrNo)) return false;
                    m_tmTemp.Clear();
                    iCycle++;
                    return false;

                case 22:
                    if(!m_tmTemp.OnDelay(OM.Auto2[i1].iBTime)) return false;
                    iCycle++;
                    return false;

                case 23:
                    iCycle++;
                    return false;

                case 24:
                    iCycle++;
                    return false;

                case 25:
                    
                    StorageClear(); 
                    StorageStart();
                    m_tmTemp.Clear();
                    iCycle++;
                    return false;

                case 26: 
                    iCycle++;
                    return false;

                case 27: 
                    if(!m_tmTemp.OnDelay(OM.Auto2[i1].iATime)) {
                        iCycle = 26;
                        return false;
                    }
                    StorageStop();
                    if(OM.Auto2[i1].iATime > 0) {
                        GetData_SaveCsv(sPath,i1, m_iMeasureCnt,GetStorageCnt());
                    }
                    m_iMeasureCnt++;

                    if(m_iMeasureCnt < OM.Auto2[i1].iCount)
                    {
                        iCycle = 25;
                        return false;
                    }
                    m_iMeasureCnt = 0;
                    
                    //1.0.0.1
                    //i1 --;
                    //if (i1 > iPos2)
                    //{
                    //    iCycle = 11;
                    //    return false;
                    //}
                    i1++;
                    if (i1 < iPos2 - 1)
                    {
                        iCycle = 11;
                        return false;
                    }
                    iCycle = 0 ;
                    return true;

            }
        }

        public bool CycleManual()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(iCycle != 0 && iCycle == iPreCycle && CheckStop() , 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", iCycle);
                Log.ShowMessage("Error",sTemp);
                return true;
            }

            if (iCycle != iPreCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", iCycle);
                Log.Trace(sTemp);
            }

            if(sErrMsg != "")
            {
                Util.MT.StopAll();
                Log.ShowMessage("Error",sErrMsg);
                iCycle = 0 ;
                return true;
            }

            if(bReqStop)
            {
                StorageStop();
                Util.MT.Stop(0);
                iCycle = 0 ;
                return true;
            }

            iPreCycle = iCycle;
            sErrMsg   = "";
            bReqStop  = false;
            
            switch (iCycle)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", iCycle);
                    Log.Trace(sTemp);
                    return true;

                case 10:
                    sPath = @"D:\Data\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\" + DateTime.Now.ToString("hh-mm-ss") ;
                    Util.MT.StopAll();
                    iPos1 = OM.Manual.iMPosition;
                    m_iMeasureCnt = 0; //현재 검사수.
                    ClearData();

                    //iMPosition    
                    //iMResidenceTim
                    //iMMeasureCnt  
                    //iMMeasureTime 

                    SetStoragePoints();
                    //StorageClear();

                    iCycle++;
                    return false;

                case 11:
                    if(!Util.MT.GetStop(m_iMotrNo)) return false;
                    Util.MT.GoAbsRun(m_iMotrNo, iPos1*10);
                    iCycle++;
                    return false;

                case 12:
                    if(!Util.MT.GetStop(m_iMotrNo)) return false;
                    m_tmTemp.Clear();
                    iCycle++;
                    return false;

                case 13:
                    if(!m_tmTemp.OnDelay(OM.Manual.iMResidenceTime)) return false;
                    m_tmTemp.Clear();
                    iCycle++;
                    return false;

                case 14:
                    iCycle++;
                    return false;

                case 15:
                    
                    StorageClear(); 
                    StorageStart();
                    m_tmTemp.Clear();
                    iCycle++;
                    return false;

                case 16: 
                    iCycle++;
                    return false;

                case 17: 
                    if(!m_tmTemp.OnDelay(OM.Manual.iMMeasureTime)) {
                        iCycle = 16;
                        return false;
                    }
                    StorageStop();
                    if(OM.Manual.iMMeasureCnt > 0) {
                        GetData_SaveCsv(sPath,iPos1, m_iMeasureCnt,GetStorageCnt());
                    }
                    m_iMeasureCnt++;
                    if(m_iMeasureCnt < OM.Manual.iMMeasureCnt)
                    {
                        iCycle = 15;
                        return false;
                    }
                    m_iMeasureCnt = 0;
                    
                    iCycle = 0 ;
                    return true;

            }
        }


        public bool CheckStop()
        {
            if (!Util.MT.GetStop(0)) return false;
            return true ;
        }

    }
}
