using COMMON;
using SML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public class Aging : ML
    {
        public static readonly string[] clNo    = { "1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20" };
        public static readonly string[] clAMode = { "전압증가","전압유지","전압반복" };
        public static readonly string[] clGMode = { "전압유지","전압증가","전류유지","전압계승","GND" };
        public static readonly string[] clFMode = { "전압유지","전압증가","GND","OPEN" };
        public static readonly string[] clCMode = { "SWITCHING","GND","OPEN" };

        protected String      m_sPartName ;
        protected int         m_iPartId   ;
        protected CDelayTimer m_tmTimeOut1;
        protected CDelayTimer m_tmTimeOut2;
        protected CDelayTimer m_tmCycle   ;
        protected CDelayTimer m_tmDelay   ;
        protected CDelayTimer m_tmLive    ;
        protected CDelayTimer m_tmGraph   ;
        protected CDelayTimer m_tmData    ;
        protected CDelayTimer m_tnDataD   ;


        public List<Data> lst        ;
        public List<Data> lstSub     ;
        public Data       data       ;
        public int        iStep      ;
        public int        iPreStep   ;
        public bool       bStepDelay ;
        public int        iRepeatCnt ;

        Stopwatch sw = new Stopwatch();

        //Rs232 GetData
        public int        iDataNo    ;
        public int        iPreDataNo ;
        public int        iDataNoD   ;
        public int        iPreDataNoD;
        public bool       bWait     ;
        public RS485_ConverTech.TStat[] Stat;
        public RS232_Daegyum_Seasoning.TStat StatD;

        #region Graph
        public delegate void Chart_AddPoints1(string _dX, string _dY);
        public delegate void Chart_AddPoints2(string _dX, string _dY);
        public delegate void Chart_AddPoints3(string _dX, string _dY);
        public delegate void Chart_AddPoints4(string _dX, string _dY);
        public delegate void Chart_AddPoints5(string _dX, string _dY);
        public delegate void Chart_AddPoints6(string _dX, string _dY);
        public delegate void Chart_Save1     (string _FileName);
        public delegate void Chart_Save2     (string _FileName);
        public delegate void Chart_Save3     (string _FileName);
        public delegate void Chart_Save4     (string _FileName);
        public delegate void Chart_Save5     (string _FileName);
        public delegate void Chart_Save6     (string _FileName);
        public delegate void Chart_Clear     ();
        public delegate void Chart_Begin     ();
        public delegate void Chart_End       ();

        public event Chart_AddPoints1  CAddPoints1 ;
        public event Chart_AddPoints2  CAddPoints2 ;
        public event Chart_AddPoints3  CAddPoints3 ;
        public event Chart_AddPoints4  CAddPoints4 ;
        public event Chart_AddPoints5  CAddPoints5 ;
        public event Chart_AddPoints6  CAddPoints6 ;

        public event Chart_Save1       CSave1      ;
        public event Chart_Save2       CSave2      ;
        public event Chart_Save3       CSave3      ;
        public event Chart_Save4       CSave4      ;
        public event Chart_Save5       CSave5      ;
        public event Chart_Save6       CSave6      ;

        public event Chart_Clear       CClear      ;
        public event Chart_Begin       CBegin      ;
        public event Chart_End         CEnd        ;


        #endregion

        public Aging(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;
            m_iPartId   = _iPartId; //Log 기록용
            m_tmTimeOut1 = new CDelayTimer();
            m_tmTimeOut2 = new CDelayTimer();
            m_tmCycle    = new CDelayTimer();
            m_tmDelay    = new CDelayTimer();
            m_tmLive     = new CDelayTimer();
            m_tmGraph    = new CDelayTimer();
            
            m_tmData     = new CDelayTimer();
            m_tnDataD    = new CDelayTimer();

            lst    = new List<Data>();
            lstSub = new List<Data>();
            
            //Rs485 컨버테크 받아온 데이터 저장용.
            Stat  = new RS485_ConverTech.TStat[RS485_ConverTech.MAX_ARRAY];
            StatD = new RS232_Daegyum_Seasoning.TStat();
            
            //Reset Data
            Reset();

        }

        public bool CheckList(int _iNo)
        {
            bool bRet = true;
            int iNo = _iNo;

            if(lst.Count < 1    ) { bRet = false; }
            if(lst.Count <= _iNo) { bRet = false; }

            if(!bRet) {
                ER_SetErr(ei.AGG_CheckRecipe,"Step = " + (iNo+1).ToString()); 
                return false;
            }

            //IncTime 0일때 처리 필요
            //dAddKv = lst[iNo].Anode.InckV   * 1000 * (iSec/lst[iNo].Anode.IncTime); 
            return true;
        }

        public void Reset(bool _bStep = false)
        {
            if(_bStep) {
                ClearStep();
            }

            m_tmTimeOut1.Clear();
            m_tmTimeOut2.Clear();
            m_tmCycle.Clear();
            m_tmDelay.Clear();
            m_tmLive .Clear();
            m_tmGraph.Clear();
            iStep      = 0;
            bStepDelay = false;
            

            //GetData
            iDataNo = 0;
            iDataNoD= 0;
            bWait   = false;
            for(int i=0; i<RS485_ConverTech.MAX_ARRAY; i++) Stat[i].Clear();
            StatD.Clear();
            
        }

        public void ClearStep()
        {
            //현재스텝 클리어
            iStep      = 0;
            iRepeatCnt = 0;
            //진행시간
            //for(int i=0; i<lst.Count; i++) {
            //스탑워치 초기화 스탑
            sw.Stop();
            sw.Reset();

            SEQ.Daegyum.SendArcReset();
            //Graph
            //CClear();

        }

        private bool bPreDetected = false;
        private bool bDetected    = false;

        public void CheckArc()
        {
            bDetected = false;
            if(Math.Abs(Stat[0].dSetVoltage - Stat[0].dVoltage) > OM.CmnOptn.dArcVoltage)
            {
                bDetected = true;
                if(!bPreDetected) OM.CmnOptn.iArcCount2++;
            }
            bPreDetected = bDetected;
        }

        public void GetData()
        {
            //if(iDataNo > 2) iDataNo = 0;
            if (m_tmData.OnDelay(bWait, 3000)) {
                ER_SetErr(ei.ETC_CvtCom);
                return;
            } 

            if(!bWait) { SEQ.ConverTech.SendA(iDataNo); bWait = true; }
            if( bWait && SEQ.ConverTech.IsReceiveEnd(iDataNo)) { 
                Stat[iDataNo] = SEQ.ConverTech.Stat[iDataNo]; 
                bWait = false; 
            }
            
        }

        public void SendLive()
        {
            if(m_tmLive.OnDelay(true,1000))
            {
                SEQ.Daegyum.SendLive();
                m_tmLive.Clear();
            }
            
        }

        public void ListLog()
        {
            if(lst.Count < 1) return;

            for(int i =0; i<lst.Count; i++)
            {
                StructLog(ref lst[i].Total  ,i+1);
                StructLog(ref lst[i].Anode  ,i+1);
                StructLog(ref lst[i].Focus  ,i+1);
                StructLog(ref lst[i].Gate   ,i+1);
                StructLog(ref lst[i].Cathode,i+1);

            }
        }

        public void ListLogSub()
        {
            if(lstSub.Count < 1) return;

            for(int i =0; i<lstSub.Count; i++)
            {
                StructLog(ref lstSub[i].Total  ,i+1);
                StructLog(ref lstSub[i].Anode  ,i+1);
                StructLog(ref lstSub[i].Focus  ,i+1);
                StructLog(ref lstSub[i].Gate   ,i+1);
                StructLog(ref lstSub[i].Cathode,i+1);

            }
        }

        public void StructLog<T>(ref T _oStruct, int _iStep)
        {
            Type type = _oStruct.GetType();

            string sKey     ;
            string sValue   ;

            FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
            string sName = type.Name;
            for(int i = 0 ; i < f.Length ; i++){
                sKey   = f[i].Name ;
                sValue = f[i].GetValue(_oStruct).ToString();  
                Trace("Step " + _iStep.ToString() + "_" + sName + "_" + sKey + " : " + sValue,ti.Dev);
            }
        }

        public void SetDataD()
        {
            SEQ.Daegyum.SendOnTime (lst[iStep].Cathode.OnTime *100); //1 -> 100usec
            SEQ.Daegyum.SendOffTime(lst[iStep].Cathode.OffTime*100);
        }

        public void GetDataD()
        {
            if (m_tmData.OnDelay(!SEQ.Daegyum.IsReceiveEnd(), 3000)) {
                ER_SetErr(ei.ETC_Daegyum);
                return;
            } 

            if(iDataNoD > 3) iDataNo = 0;

                 if(iDataNoD == 0) { SEQ.Daegyum.SendArc   (); iDataNoD++; }
            else if(iDataNoD == 1) { if(SEQ.Daegyum.IsReceiveEnd()) { OM.CmnOptn.iArcCount1 = SEQ.Daegyum.Stat.iArc; iDataNoD++; } }
            else if(iDataNoD == 2) { SEQ.Daegyum.SendAll(); iDataNoD++; }
            else if(iDataNoD == 3) { //TODO ::
                if(SEQ.Daegyum.IsReceiveEnd()) { 
                    StatD.dCathod = SEQ.Daegyum.Stat.dCathod; 
                    StatD.dFocus  = SEQ.Daegyum.Stat.dFocus ;  
                    StatD.dGate   = SEQ.Daegyum.Stat.dGate  ; 
                    iDataNoD++; 
                } 
            }
        }

        public void StopWatch()
        {
            sw.Stop();
            
        }

        public bool StartCon()
        {
            CBegin();
            SEQ.ConverTech.SendD(0);
            SEQ.ConverTech.SendD(1);
            SEQ.ConverTech.SendD(2);
            SEQ.Daegyum.SendOutOnOff(true);
            return true;
        }

        public bool StopCon()
        {
            StopWatch();
            SEQ.ConverTech.SendE(0);
            SEQ.ConverTech.SendE(1);
            SEQ.ConverTech.SendE(2);
            SEQ.Daegyum.SendOutOnOff(false);

            //TODO :: 일단 오픈상태로 스탑하기
            IO_SetY(yi.CathodeGND,false);
            IO_SetY(yi.CathodeSWT,false);
            IO_SetY(yi.GateGND   ,false);
            IO_SetY(yi.GatePower ,false);
            IO_SetY(yi.FocusGND  ,false);
            IO_SetY(yi.FocusPower,false);

            CEnd();
            return true;
        }

        public bool Total()
        {
            if (iPreStep != iStep) Log.Trace("Step Changed : " + iPreStep.ToString() + " -> " + iStep.ToString());
            iPreStep = iStep;
            
            int  iNo       = iStep ;
            bool bTimeOut1 = false ;
            bool bTimeOut2 = false ;
            bool bTimeOut3 = false ;
            
            //0. Error List Step Delay Check
            if (ER_IsErr (    )) return false;
            if (!CheckList(iNo)) return false;
            if (!m_tmCycle.OnDelay(bStepDelay, OM.CmnOptn.iStepDelay)) return false;
            SendLive(); //대겸에 1초마다 신호주기
            bStepDelay = false;
            if(!sw.IsRunning) sw.Start();
            int  msec      = (int)sw.ElapsedMilliseconds ;
            lst[iStep].Total.msec = msec ;


            //1. Trans And Check
            if(!AnodeTrans(iNo)) bTimeOut1 = true;
            if(!GateTrans (iNo)) bTimeOut2 = true; 
            if(!FocusTrans(iNo)) bTimeOut3 = true;

            if (m_tmTimeOut1.OnDelay(bTimeOut1 || bTimeOut2 || bTimeOut3, 300)) {
                if(bTimeOut1) ER_SetErr(ei.AGG_CathodTrans);
                if(bTimeOut2) ER_SetErr(ei.AGG_GateTrans  );
                if(bTimeOut3) ER_SetErr(ei.AGG_FocusTrans );
                return false;
            }
            if(bTimeOut1 || bTimeOut2 || bTimeOut3) return false;

            //2. Next Step
            if(msec >= lst[iNo].Total.Time)
            {
                iStep++; 
                if(iStep < lst.Count) //레시피 와 레시피 딜레이
                {
                    if(lst[iStep-1].Total.No != lst[iStep].Total.No)
                    {
                        bStepDelay = true; sw.Stop();
                        return false;
                    }
                }
                //if(lst[iStep].Total.RepeatCnt != 0 && iRepeatCnt < lst[iStep].Total.RepeatCnt)
                //{
                //    iRepeatCnt++; bStepDelay = true; sw.Stop();
                //    return false;
                //    //lst[iStep].Total.dSttTime = DateTime.Now;
                //}
                //iStep++; bStepDelay = true; sw.Stop();
                //iRepeatCnt = 0; 
                //if(iStep < lst.Count) lst[iStep].Total.dSttTime = DateTime.Now;
            }

            //3. End Check
            if(iStep >= lst.Count)
            {
                return true;
            }

            //4. Play 
            if (m_tmCycle.OnDelay(true, 100)) {
                m_tmCycle.Clear();
                Anode(iNo,msec);
                Gate (iNo,msec);
                Focus(iNo,msec);
            }
            else {
                GetData ();
                GetDataD();

                //Graph
                if (m_tmGraph.OnDelay(true, 1000)) {
                    m_tmGraph.Clear();

                    double dPreTime = 0;
                    for(int i=0; i<iStep; i++) dPreTime += SEQ.aging.lst[i].Total.Time;
                    string sTime = (TimeSpan.FromSeconds(dPreTime) + TimeSpan.FromMilliseconds(lst[iStep].Total.msec)).ToString(); 
                    
                    CAddPoints1(sTime, Stat[0].dSetVoltage.ToString());
                    CAddPoints2(sTime, StatD.dCathod      .ToString());
                    CAddPoints3(sTime, Stat[1].dSetVoltage.ToString());
                    CAddPoints4(sTime, StatD.dFocus       .ToString());
                    CAddPoints5(sTime, Stat[2].dSetVoltage.ToString());
                    CAddPoints6(sTime, StatD.dGate        .ToString());
                }

                //Limit Check
                bool b1 = StatD.dCathod < lst[iStep].Cathode.MinmA ;
                bool b2 = StatD.dCathod > lst[iStep].Cathode.MaxmA ;
                if(b1 || b2) { 
                    if(b1) ML.ER_SetErr(ei.AGG_CathodLimit,"Check the Min Limit (" + lst[iStep].Cathode.MinmA.ToString() + ")"); 
                    if(b2) ML.ER_SetErr(ei.AGG_CathodLimit,"Check the Max Limit (" + lst[iStep].Cathode.MaxmA.ToString() + ")");
                    ClearStep();
                    return false; 
                }
            }

            CheckArc();
            
            //GetData Time Out
            //if (m_tmTimeOut2.OnDelay(iDataNo == iPreDataNo, 300)) {
            //    ER_SetErr(ei.ETC_Daegyum);
            //    return false;
            //}            
            //iPreDataNo = iDataNo;

            return false;

        }

        public void Anode(int _iNo, double _dSec)
        {
            int    iNo  = _iNo   ;
            
            //Set
            double dSttv        = lst[iNo].Anode.StartkV * 1000 ;
            double dAddv        ;
            double dSetv        ;
            int    iSec         = (int)_dSec;
            //public static readonly string[] clAMode = { "전압증가","전압유지","전압반복" };
            if(lst[iNo].Anode.Mode == clAMode[0]) //전압증가
            {
                if(lst[iNo].Anode.IncTime < 1) lst[iNo].Anode.IncTime = 1;
                dAddv = lst[iNo].Anode.InckV   * 1000 * (iSec/lst[iNo].Anode.IncTime);
                dSetv = dSttv+dAddv;
                if(dSetv > lst[iNo].Anode.StopkV*1000) dSetv = lst[iNo].Anode.StopkV*1000;
                SEQ.ConverTech.SendB(0,dSetv);
            }
            else if(lst[iNo].Anode.Mode == clAMode[1]) //전압유지
            {
                SEQ.ConverTech.SendB(0,dSttv);
            }
            else if(lst[iNo].Anode.Mode == clAMode[2]) //전압반복
            {
                int AllTime = (int)(lst[iNo].Anode.OnTime + lst[iNo].Anode.OffTime) ;
                int  iOn = iSec % AllTime;
                bool bOn = iOn <=  lst[iNo].Anode.OnTime ;
                if (bOn)
                {
                    SEQ.ConverTech.SendB(0,dSttv); //On
                    SEQ.ConverTech.SendD(0); //On
                }
                else
                {
                    SEQ.ConverTech.SendB(0,0); //On
                    SEQ.ConverTech.SendE(0); //Off
                }
            }
        }

        double dPreSetvG = 0;
        public void Gate(int _iNo, double _dSec)
        {
            int    iNo  = _iNo   ;
            
            double dSttv        = lst[iNo].Gate.StartV ;
            double dAddv        ;
            double dSetv        = 0;
            int    iSec         = (int)_dSec;
            //public static readonly string[] clGMode = { "전압유지","전압증가","전류유지","전압계승","GND","OPEN" };
            if(lst[iNo].Gate.Mode == clGMode[0]) //전압유지
            {
                SEQ.ConverTech.SendB(2,dSttv);
            }
            else if(lst[iNo].Gate.Mode == clGMode[1]) //전압증가
            {
                if(lst[iNo].Gate.IncTime < 1) lst[iNo].Gate.IncTime = 1 ;
                dAddv = lst[iNo].Gate.IncV * (iSec/lst[iNo].Gate.IncTime);
                dSetv = dSttv+dAddv;
                if(dSetv               >  lst[iNo].Gate.StopV) dSetv = lst[iNo].Gate.StopV;
                //if(lst[iNo].Gate.EndmA <= Stat[1].dCurrent   ) dSetv = Stat[1].dSetVoltage; 
                if(lst[iNo].Gate.EndmA <= StatD.dGate        ) dSetv = dPreSetvG ;

                SEQ.ConverTech.SendB(2,dSetv);

            }
            else if(lst[iNo].Gate.Mode == clGMode[2]) //전류유지
            {
                dSetv = dPreSetvG ;
                if(StatD.dCathod < lst[iNo].Gate.KeepmA) dSetv += 10 ; //TODO :: 테스트 필요
                if(StatD.dCathod > lst[iNo].Gate.KeepmA) dSetv -= 10 ;
                SEQ.ConverTech.SendB(2,dSetv);
            }
            else if(lst[iNo].Gate.Mode == clGMode[3]) //전압계승
            {
                //dSetv = Stat[1].dSetVoltage; 
                dSetv = dPreSetvG ;
                SEQ.ConverTech.SendB(2,dSetv); //+ a
            }

            if(dSetv != 0) dPreSetvG = dSetv;
        }

        double dPreSetvF = 0;
        public void Focus(int _iNo, double _dSec)
        {
            int    iNo  = _iNo   ;
            
            double dSttv        = lst[iNo].Focus.StartV ;
            double dAddv        ;
            double dSetv        = 0;
            int    iSec          = (int)_dSec;
            //public static readonly string[] clFMode = { "전압유지","전압증가","GND","OPEN" };
            if(lst[iNo].Focus.Mode == clFMode[0]) //전압유지
            {
                dSetv = dPreSetvF ;
                SEQ.ConverTech.SendB(1,dSttv);
            }
            else if(lst[iNo].Focus.Mode == clFMode[1]) //전압증가
            {
                if(lst[iNo].Focus.IncTime < 1) lst[iNo].Focus.IncTime = 1 ;
                dAddv = lst[iNo].Focus.IncV * (iSec/lst[iNo].Focus.IncTime);
                dSetv = dSttv+dAddv;
                if(dSetv > lst[iNo].Focus.StopV) dSetv = lst[iNo].Focus.StopV;
                SEQ.ConverTech.SendB(1,dSetv);

            }
            SetDataD();

            if(dSetv != 0) dPreSetvF = dSetv;
        }

        public bool AnodeTrans(int _iNo)
        {
            bool   bRet = false;
            int    iNo  = _iNo   ;

            //public static readonly string[] clCMode = { "SWITCHING","GND","OPEN" };
            if(lst[iNo].Cathode.Mode == clCMode[0]) //SWITCHING
            {
                IO_SetY(yi.CathodeSWT,true );
                IO_SetY(yi.CathodeGND,false);

                if(IO_GetX(xi.CathodeSWT) && !IO_GetX(xi.CathodeGND)) bRet = true;
            }
            else if(lst[iNo].Cathode.Mode == clCMode[1]) //GND
            {
                IO_SetY(yi.CathodeSWT,false);
                IO_SetY(yi.CathodeGND,true );

                if(!IO_GetX(xi.CathodeSWT) && IO_GetX(xi.CathodeGND)) bRet = true;
            }
            else //OPEN
            {
                IO_SetY(yi.CathodeSWT,false);
                IO_SetY(yi.CathodeGND,false);

                if(!IO_GetX(xi.CathodeSWT) && !IO_GetX(xi.CathodeGND)) bRet = true;
            }

            return bRet;
        }

        public bool GateTrans(int _iNo)
        {
            bool   bRet = false;
            int    iNo  = _iNo   ;

            //public static readonly string[] clGMode = { "전압유지","전압증가","전류유지","전압계승","GND" };
            if(lst[iNo].Gate.Mode == clGMode[4]) //GND
            {
                IO_SetY(yi.GatePower ,false);
                IO_SetY(yi.GateGND   ,true );

                if(!IO_GetX(xi.GatePower) && IO_GetX(xi.GateGND)) bRet = true;
            }
            //else if(lst[iNo].Gate.Mode == clGMode[5]) //OPEN
            //{
            //    IO_SetY(yi.GatePower ,false);
            //    IO_SetY(yi.GateGND   ,false);

            //    if(!IO_GetX(xi.GatePower) && !IO_GetX(xi.GateGND)) bRet = true;
            //}
            else //
            {
                IO_SetY(yi.GatePower ,true );
                IO_SetY(yi.GateGND   ,false);

                if(IO_GetX(xi.GatePower) && !IO_GetX(xi.GateGND)) bRet = true;
            }

            return bRet;
        }

        public bool FocusTrans(int _iNo)
        {
            bool   bRet = false;
            int    iNo  = _iNo   ;

            //public static readonly string[] clFMode = { "전압유지","전압증가","GND","OPEN" };
            if(lst[iNo].Focus.Mode == clFMode[2]) //GND
            {
                IO_SetY(yi.FocusPower,false);
                IO_SetY(yi.FocusGND  ,true );

                if(!IO_GetX(xi.FocusPower) && IO_GetX(xi.FocusGND)) bRet = true;
            }
            else if(lst[iNo].Focus.Mode == clFMode[3]) //GND//OPEN
            {
                IO_SetY(yi.FocusPower,false);
                IO_SetY(yi.FocusGND  ,false);

                if(!IO_GetX(xi.FocusPower) && !IO_GetX(xi.FocusGND)) bRet = true;
            }
            else
            {
                IO_SetY(yi.FocusPower,true );
                IO_SetY(yi.FocusGND  ,false);

                if( IO_GetX(xi.FocusPower) && !IO_GetX(xi.FocusGND)) bRet = true;
            }

            return bRet;
        }
               
        //Log
        public void Trace(string _name, params bool[] _val)
        {
            string sLog = _name + " = ";
            for(int i=0; i<_val.Length; i++) sLog += _val[i].ToString() + ",";
            Trace(sLog);
        }

        public void Trace(string _sMsg, int _iTag = -1, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = m_sPartName.Replace(",", "");
            string sMsg = _sMsg.Replace(",", "");
            //string sTag = string.Format("{0:00}", m_iPartId);
            string sTag = "";
            if(_iTag != -1) sTag = string.Format("{0:00}", _iTag    );
            else            sTag = string.Format("{0:00}", m_iPartId);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            Log.SendMessage(sFullMsg);
        }

    }

    public class Data
    {
        public struct TTotal
        {
            private string no       ;
            private double time     ;
            private double repeatCnt;

            public string No        { get { return no       ;} set { if(value == "") value = Aging.clNo[0]; no = value;} }
            public double Time      { get { return time     ;} set { if(value<OM.CmnOptn.min11) value = OM.CmnOptn.min11; if(value>OM.CmnOptn.max11) value = OM.CmnOptn.max11; time      = value;} }
            public double RepeatCnt { get { return repeatCnt;} set { if(value<OM.CmnOptn.min12) value = OM.CmnOptn.min12; if(value>OM.CmnOptn.max12) value = OM.CmnOptn.max12; repeatCnt = value;} }

            //public DateTime dSttTime;
            public int    msec      ;

            public void Clear()
            {
                //dSttTime = DateTime.MinValue ;
                msec = 0;
            }
        }
        public TTotal Total;
        
        public struct TAnode
        {
            private string mode    ;
            private double startkV ;
            private double stopkV  ;
            private double inckV   ;
            private double incTime ;
            private double onTime  ;
            private double offTime ;
            
            public string Mode    { get { return mode   ;} set { if(value == "") value = Aging.clAMode[0]; mode = value;} }
            public double StartkV { get { return startkV;} set { if(value<OM.CmnOptn.min21) value = OM.CmnOptn.min21; if(value>OM.CmnOptn.max21) value = OM.CmnOptn.max21; startkV = value;} }
            public double StopkV  { get { return stopkV ;} set { if(value<OM.CmnOptn.min22) value = OM.CmnOptn.min22; if(value>OM.CmnOptn.max22) value = OM.CmnOptn.max22; stopkV  = value;} }
            public double InckV   { get { return inckV  ;} set { if(value<OM.CmnOptn.min23) value = OM.CmnOptn.min23; if(value>OM.CmnOptn.max23) value = OM.CmnOptn.max23; inckV   = value;} }
            public double IncTime { get { return incTime;} set { if(value<OM.CmnOptn.min24) value = OM.CmnOptn.min24; if(value>OM.CmnOptn.max24) value = OM.CmnOptn.max24; incTime = value;} }
            public double OnTime  { get { return onTime ;} set { if(value<OM.CmnOptn.min25) value = OM.CmnOptn.min25; if(value>OM.CmnOptn.max25) value = OM.CmnOptn.max25; onTime  = value;} }
            public double OffTime { get { return offTime;} set { if(value<OM.CmnOptn.min26) value = OM.CmnOptn.min26; if(value>OM.CmnOptn.max26) value = OM.CmnOptn.max26; offTime = value;} }

        }
        public TAnode Anode;

        public struct TGate
        {
            private string mode    ;
            private double startV  ;
            private double stopV   ;
            private double incV    ;
            private double incTime ;
            private double keepmA  ;
            private double endmA   ;

            public string Mode    { get { return mode   ;} set { if(value == "") value = Aging.clGMode[0]; mode = value;} }
            public double StartV  { get { return startV ;} set { if(value<OM.CmnOptn.min31) value = OM.CmnOptn.min31; if(value>OM.CmnOptn.max31) value = OM.CmnOptn.max31; startV  = value;} }
            public double StopV   { get { return stopV  ;} set { if(value<OM.CmnOptn.min32) value = OM.CmnOptn.min32; if(value>OM.CmnOptn.max32) value = OM.CmnOptn.max32; stopV   = value;} }
            public double IncV    { get { return incV   ;} set { if(value<OM.CmnOptn.min33) value = OM.CmnOptn.min33; if(value>OM.CmnOptn.max33) value = OM.CmnOptn.max33; incV    = value;} }
            public double IncTime { get { return incTime;} set { if(value<OM.CmnOptn.min34) value = OM.CmnOptn.min34; if(value>OM.CmnOptn.max34) value = OM.CmnOptn.max34; incTime = value;} }
            public double KeepmA  { get { return keepmA ;} set { if(value<OM.CmnOptn.min35) value = OM.CmnOptn.min35; if(value>OM.CmnOptn.max35) value = OM.CmnOptn.max35; keepmA  = value;} }
            public double EndmA   { get { return endmA  ;} set { if(value<OM.CmnOptn.min36) value = OM.CmnOptn.min36; if(value>OM.CmnOptn.max36) value = OM.CmnOptn.max36; endmA   = value;} }

        }
        public TGate Gate;

        public struct TFocus
        {
            private string mode    ;
            private double startV  ;
            private double stopV   ;
            private double incV    ;
            private double incTime ;

            public string Mode    { get { return mode   ;} set { if(value == "") value = Aging.clFMode[0]; mode = value;} }
            public double StartV  { get { return startV ;} set { if(value<OM.CmnOptn.min41) value = OM.CmnOptn.min41; if(value>OM.CmnOptn.max41) value = OM.CmnOptn.max41; startV  = value;} }
            public double StopV   { get { return stopV  ;} set { if(value<OM.CmnOptn.min42) value = OM.CmnOptn.min42; if(value>OM.CmnOptn.max42) value = OM.CmnOptn.max42; stopV   = value;} }
            public double IncV    { get { return incV   ;} set { if(value<OM.CmnOptn.min43) value = OM.CmnOptn.min43; if(value>OM.CmnOptn.max43) value = OM.CmnOptn.max43; incV    = value;} }
            public double IncTime { get { return incTime;} set { if(value<OM.CmnOptn.min44) value = OM.CmnOptn.min44; if(value>OM.CmnOptn.max44) value = OM.CmnOptn.max44; incTime = value;} }


        }
        public TFocus Focus;

        public struct TCathode
        {
            private string mode    ;
            private double onTime  ;
            private double offTime ;
            private double minmA   ;
            private double maxmA   ;

            public string Mode     { get { return mode   ;} set { if(value == "") value = Aging.clCMode[0]; mode = value;} }
            public double OnTime   { get { return onTime ;} set { if(value<OM.CmnOptn.min51) value = OM.CmnOptn.min51; if(value>OM.CmnOptn.max51) value = OM.CmnOptn.max51; onTime  = value;} }
            public double OffTime  { get { return offTime;} set { if(value<OM.CmnOptn.min52) value = OM.CmnOptn.min52; if(value>OM.CmnOptn.max52) value = OM.CmnOptn.max52; offTime = value;} }
            public double MinmA    { get { return minmA  ;} set { if(value<OM.CmnOptn.min53) value = OM.CmnOptn.min53; if(value>OM.CmnOptn.max53) value = OM.CmnOptn.max53; minmA   = value;} }
            public double MaxmA    { get { return maxmA  ;} set { if(value<OM.CmnOptn.min54) value = OM.CmnOptn.min54; if(value>OM.CmnOptn.max54) value = OM.CmnOptn.max54; maxmA   = value;} }


        }
        public TCathode Cathode;

        public Data ()
        {

        }

        public Data (TTotal _Total, TAnode _Anode ,TGate _Gate,TFocus _Focus, TCathode _Cathode)
        {
            Total.Clear();
            Total   = _Total   ;
            Anode   = _Anode   ;
            Gate    = _Gate    ;
            Focus   = _Focus   ;
            Cathode = _Cathode ;
        }

        public void Clear()
        {
            Total.Clear();
        }



    }
}
