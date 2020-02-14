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
        public static readonly string[] clGMode = { "전압유지","전압증가","전류유지","전압계승","GND","계승증가" };
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
        protected CDelayTimer m_tmData1   ;
        protected CDelayTimer m_tmData2   ;
        //protected CDelayTimer m_tnDataD   ;
        protected CDelayTimer m_tmKeep    ;
        protected CDelayTimer m_tmArc     ;
        protected CDelayTimer m_tmArc1    ;
        protected CDelayTimer m_tmArc2    ;
        protected CDelayTimer m_tmArc3    ;
        protected CDelayTimer m_tmStart   ;
        public bool       bStart     ;

        public List<Data> lst        ;
        public List<Data> lstSub     ;
        public Data       data       ;
        public int        iStep      ;
        public int        iPreStep   ;
        public bool       bStepDelay ;
        public int        iRepeatCnt ;
        public bool       bInspection;
        Stopwatch sw      = new Stopwatch();
        Stopwatch Arc     = new Stopwatch(); //아킹 검출후 2초 패스용
        Stopwatch swStart = new Stopwatch(); //아킹 검출후 2초 패스용

        

        //Rs232 GetData
        public int        iDataNo    ;
        public int        iPreDataNo ;
        public int        iDataNoD   ;
        public int        iPreDataNoD;
        public bool       bWait     ;
        public RS485_ConverTech.TStat[] Stat;
        public RS232_Daegyum_Seasoning.TStat StatD;

        #region Graph
        public delegate void Chart_AddPoints1(DateTime _dX, string _dY); public delegate void Chart_AddPoints1C(DateTime _dX, string _dY);
        public delegate void Chart_AddPoints2(DateTime _dX, string _dY); public delegate void Chart_AddPoints2C(DateTime _dX, string _dY);
        public delegate void Chart_AddPoints3(DateTime _dX, string _dY); public delegate void Chart_AddPoints3C(DateTime _dX, string _dY);
        public delegate void Chart_AddPoints4(DateTime _dX, string _dY); public delegate void Chart_AddPoints4C(DateTime _dX, string _dY);
        public delegate void Chart_AddPoints5(DateTime _dX, string _dY); public delegate void Chart_AddPoints5C(DateTime _dX, string _dY);
        public delegate void Chart_AddPoints6(DateTime _dX, string _dY); public delegate void Chart_AddPoints6C(DateTime _dX, string _dY);

        public delegate void Chart_Save1     (string _FileName); public delegate void Chart_Save1C    (string _FileName);
        public delegate void Chart_Save2     (string _FileName); public delegate void Chart_Save2C    (string _FileName);
        public delegate void Chart_Save3     (string _FileName); public delegate void Chart_Save3C    (string _FileName);
        public delegate void Chart_Save4     (string _FileName); public delegate void Chart_Save4C    (string _FileName);
        public delegate void Chart_Save5     (string _FileName); public delegate void Chart_Save5C    (string _FileName);
        public delegate void Chart_Save6     (string _FileName); public delegate void Chart_Save6C    (string _FileName);
        public delegate void Chart_Clear     ();                 
        public delegate void Chart_Begin     ();                 
        public delegate void Chart_End       ();                 

        public event Chart_AddPoints1  CAddPoints1 ; public event Chart_AddPoints1C  CAddPoints1C ;
        public event Chart_AddPoints2  CAddPoints2 ; public event Chart_AddPoints2C  CAddPoints2C ;
        public event Chart_AddPoints3  CAddPoints3 ; public event Chart_AddPoints3C  CAddPoints3C ;
        public event Chart_AddPoints4  CAddPoints4 ; public event Chart_AddPoints4C  CAddPoints4C ;
        public event Chart_AddPoints5  CAddPoints5 ; public event Chart_AddPoints5C  CAddPoints5C ;
        public event Chart_AddPoints6  CAddPoints6 ; public event Chart_AddPoints6C  CAddPoints6C ;

        public event Chart_Save1       CSave1      ; public event Chart_Save1C       CSave1C      ;
        public event Chart_Save2       CSave2      ; public event Chart_Save2C       CSave2C      ;
        public event Chart_Save3       CSave3      ; public event Chart_Save3C       CSave3C      ;
        public event Chart_Save4       CSave4      ; public event Chart_Save4C       CSave4C      ;
        public event Chart_Save5       CSave5      ; public event Chart_Save5C       CSave5C      ;
        public event Chart_Save6       CSave6      ; public event Chart_Save6C       CSave6C      ;
        public event Chart_Clear       CClear      ; 
        public event Chart_Begin       CBegin      ; 
        public event Chart_End         CEnd        ; 


        #endregion

        //Total
        private bool bReceive    = false;
        private bool bStepChange = false;

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
            
            m_tmData1    = new CDelayTimer();
            m_tmData2    = new CDelayTimer();
            //m_tnDataD    = new CDelayTimer();
            m_tmKeep     = new CDelayTimer();
            m_tmArc      = new CDelayTimer();
            m_tmArc1     = new CDelayTimer();
            m_tmArc2     = new CDelayTimer();
            m_tmArc3     = new CDelayTimer();
            m_tmStart    = new CDelayTimer();

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
            //dAddKv = lst[iNo].Anode.InckV   * 1000 * (imsec/lst[iNo].Anode.IncTime); 
            return true;
        }

        public void TimeClear()
        {
            m_tmTimeOut1.Clear(); 
            m_tmTimeOut2.Clear(); 
            m_tmCycle   .Clear(); 
            m_tmDelay   .Clear(); 
            m_tmLive    .Clear(); 
            m_tmGraph   .Clear(); 
            
            m_tmData1   .Clear(); 
            m_tmData2   .Clear(); 
            //m_tnDataD   .Clear(); 
            m_tmKeep    .Clear();
            m_tmArc     .Clear();
            m_tmArc1    .Clear();
            m_tmArc2    .Clear();
            m_tmArc3    .Clear();
            m_tmStart   .Clear();


        }
        public void Reset(bool _bStep = false)
        {
            if(_bStep) {
                ClearStep();
            }

            TimeClear();

            iStep      = 0;
            bStepDelay = false;
            

            //GetData
            iDataNo = 0;
            iDataNoD= 0;
            bWait   = false;
            for(int i=0; i<RS485_ConverTech.MAX_ARRAY; i++) Stat[i].Clear();
            StatD.Clear();

            bStart = false;
            
        }

        public void ClearStep(bool bLotEnd = true)
        {
            //현재스텝 클리어
            iStep      = 0;
            iRepeatCnt = 0;
            //진행시간
            //for(int i=0; i<lst.Count; i++) {            //스탑워치 초기화 스탑
            sw.Stop();
            sw.Reset();
            swStart.Stop ();
            swStart.Reset();
            Arc.Stop ();
            Arc.Reset();
            //SEQ.Daegyum.SendArcReset();

            if(bLotEnd) {
                LOT.LotEnd();
            }

            //lst.
            //lst.Clear();
            //Graph
            //CClear();

        }

        private bool bPreDetected = false;
        private bool bDetected    = false;
        private int  iArcTime     = 0    ;

        //bool bAStt = false; bool bADif = false; double dAPre = 0.0;
        //bool bFStt = false; bool bFDif = false; double dFPre = 0.0;
        //bool bGStt = false; bool bGDif = false; double dGPre = 0.0;

        double dAPre = 0.0;
        double dFPre = 0.0;
        double dGPre = 0.0;

        public bool CheckArc()
        {
            bool bRet0 = false;
            bool bRet1 = false;
            bool bRet2 = false;
            bool bRet3 = false;

            //bDetected = false;
            //lst[iStep].Anode.InckV
            //if(!m_tmStart.OnDelay(bStart,2000)) return false;
            //bStart = false;

            //double dStepTime = 0;
            //for(int i=0; i< iStep; i++) dStepTime += GetTime(i);
            //int iStepmsec = (int)(lst[0].Total.msec - dStepTime*1000.0) ;
            //bool bStepmsec = iStepmsec > 2000 ;

            //전압 증가일때만 아킹 검사
            //bool bInc    = false;
            //bool bKeep   = false;
            //bool bRepeat = false;
            ////clAMode = { "전압증가","전압유지","전압반복" };
            //     if(lst[iStep].Anode.Mode == clAMode[0]) bInc    = true;
            //else if(lst[iStep].Anode.Mode == clAMode[1]) bKeep   = true;
            //else                                         bRepeat = true;
            //
            //bool bSwt   = lst[iStep].Anode.Mode == clCMode[0]; //SWITCHING
            //
            //bool bAOver = Math.Abs(Stat[0].dSetVoltage - Stat[0].dVoltage) > OM.CmnOptn.dArcVoltage ;
            //bool b2     = false;

            //----------------------------------------------------------------------
            //전압증가
            //----------------------------------------------------------------------
            bool bInc = false;
            if(lst[iStep].Anode.Mode == clAMode[0]) 
            {
                bInc = Math.Abs(Stat[0].dSetVoltage - Stat[0].dVoltage) > lst[iStep].Anode.InckV + 1.0;
                if (Arc.IsRunning                  ) bInc = false;
            }
            if (Arc.ElapsedMilliseconds >= 2000) Arc.Stop();
            if(bInc) { bRet0 = true; Arc.Restart();}
            //----------------------------------------------------------------------

            bool bAOver = Math.Abs(Stat[0].dSetVoltage - Stat[0].dVoltage) > OM.CmnOptn.dArcVoltage ;
            bool bFOver = Math.Abs(Stat[1].dSetVoltage - Stat[1].dVoltage) > OM.CmnOptn.dArcVoltage1;
            bool bGOver = Math.Abs(Stat[2].dSetVoltage - Stat[2].dVoltage) > OM.CmnOptn.dArcVoltage2;

            bool bA = Stat[0].dSetVoltage == dAPre && Stat[0].dSetVoltage != 0.0;
            bool bF = Stat[1].dSetVoltage == dFPre && Stat[1].dSetVoltage != 0.0;
            bool bG = Stat[2].dSetVoltage == dGPre && Stat[2].dSetVoltage != 0.0;

            dAPre = Stat[0].dSetVoltage ;
            dFPre = Stat[1].dSetVoltage ;
            dGPre = Stat[2].dSetVoltage ;

            if (m_tmArc1.OnDelay(bA, 2000)) { if (bAOver) { bRet1 = true; } }
            if (m_tmArc2.OnDelay(bF, 2000)) { if (bFOver) { bRet2 = true; } }
            if (m_tmArc3.OnDelay(bG, 2000)) { if (bGOver) { bRet3 = true; } }
            //if(m_tmArc1 .OnDelay(bA, 2000)) { if(bAOver) { bRet1 = true; } }
            //if(m_tmArc2 .OnDelay(bF, 2000)) { if(bFOver) { bRet2 = true; } }
            //if(m_tmArc3 .OnDelay(bG, 2000)) { if(bGOver) { bRet3 = true; } }

            bool bSwt = lst[iStep].Cathode.Mode == clCMode[0]; //SWITCHING
            if(bSwt)
            {
                if(bRet1) { ER_SetErr(ei.AGG_Arc,"Anode 구간 아킹 감지 설정 : " + Stat[0].dSetVoltage.ToString() + " 출력 : " + Stat[0].dVoltage.ToString()); } 
                if(bRet2) { ER_SetErr(ei.AGG_Arc,"Focus 구간 아킹 감지 설정 : " + Stat[1].dSetVoltage.ToString() + " 출력 : " + Stat[1].dVoltage.ToString()); } 
                if(bRet3) { ER_SetErr(ei.AGG_Arc,"Gate  구간 아킹 감지 설정 : " + Stat[2].dSetVoltage.ToString() + " 출력 : " + Stat[2].dVoltage.ToString()); } 
            }

            if(bRet0 || bRet1 || bRet2 || bRet3)
            {
                OM.CmnOptn.iArcCount1++;
                
                SEQ.Daegyum.SendReset ();
                SEQ.Daegyum.SendReset1();
                SEQ.Daegyum.SendReset2();
            }
            /*
            bool b0 = bKeep   && bAOver && bSwt;
            bool b1 = bRepeat && Stat[0].dSetVoltage >= Stat[0].dVoltage && bAOver ;
            
            bool b3 = Math.Abs(Stat[1].dSetVoltage - Stat[1].dVoltage) > OM.CmnOptn.dArcVoltage1 ;
            bool b4 = Math.Abs(Stat[2].dSetVoltage - Stat[2].dVoltage) > OM.CmnOptn.dArcVoltage2 ;

            if(m_tmArc .OnDelay(b0, 2000    )) { ER_SetErr(ei.AGG_Arc,"Anode 구간 아킹 감지"); bRet = true; } 
            if(m_tmArc1.OnDelay(b1, 2000    )) { bRet = true; } //올라가는데 2초정도 걸리는거 같아서...
            //if(m_tmArc1.OnDelay(b2, 2000    )) { iArcTime = 2000; bRet = true; } 
            if(m_tmArc2.OnDelay(b3, 2000    )) { ER_SetErr(ei.AGG_Arc,"Focus 구간 아킹 감지");bRet = true; } 
            if(m_tmArc3.OnDelay(b4, 2000    )) { ER_SetErr(ei.AGG_Arc,"Gate 구간 아킹 감지" );bRet = true; } 

            if(bRet)
            {
                OM.CmnOptn.iArcCount1++;
                
                SEQ.Daegyum.SendReset ();
                SEQ.Daegyum.SendReset1();
                SEQ.Daegyum.SendReset2();
            }
            */
            /*
            if(bStepmsec && (b1 || b2) && !bStart)
            {
                //bDetected = true;
                //if(!bPreDetected) {
                    if (bInspection)
                    {
                        //ER_SetErr(ei.AGG_Arc);
                        //bRet = true;
                        //ClearStep();
                        if(m_tmArc.OnDelay(true,iArcTime))
                        {
                            OM.CmnOptn.iArcCount1++;
                            iArcTime = 2000;
                            m_tmArc.Clear();

                            SEQ.Daegyum.SendReset ();
                            SEQ.Daegyum.SendReset1();
                            SEQ.Daegyum.SendReset2();
                        }
                        
                    }
                    else
                    {
                        OM.CmnOptn.iArcCount1++;
                        ER_SetErr(ei.AGG_Arc);
                        bRet = true;
                        ClearStep();

                        SEQ.Daegyum.SendReset ();
                        SEQ.Daegyum.SendReset1();
                        SEQ.Daegyum.SendReset2();


                    }
                //    OM.CmnOptn.iArcCount2++;
                //}
            }

            bool b3 = Math.Abs(Stat[1].dSetVoltage - Stat[1].dVoltage) > OM.CmnOptn.dArcVoltage1 ;
            bool b4 = Math.Abs(Stat[2].dSetVoltage - Stat[2].dVoltage) > OM.CmnOptn.dArcVoltage2 ;

            if(bStepmsec && (b3 || b4) && !bStart)
            {
                OM.CmnOptn.iArcCount1++;
                ER_SetErr(ei.AGG_Arc);
                bRet = true;
                ClearStep();

                SEQ.Daegyum.SendReset ();
                SEQ.Daegyum.SendReset1();
                SEQ.Daegyum.SendReset2();

            }
            */
            //bPreDetected = bDetected;

            //if(OM.CmnOptn.iDetectCount > 0)
            //{
            //    ER_SetErr(ei.AGG_Arc);
            //    bRet = true;
            //    ClearStep();
            //}

            //if(StatD.iArc >0)
            //{
            //    bRet = false;
            //}
            bool bRet = bRet0 || bRet1 || bRet2 || bRet3 ;
            return bRet;
        }
        public bool bReading = false;
        public void GetData()
        {
            //clFMode = { "전압유지","전압증가","GND","OPEN" };
            //clGMode = { "전압유지","전압증가","전류유지","전압계승","GND","계승증가" };
            //clCMode = { "SWITCHING","GND","OPEN" };
            SEQ.ConverTech.SendA(0);
            //Delay(15);

            bool bFocusOpen = lst[iStep].Focus.Mode == clFMode[2] || lst[iStep].Focus.Mode == clFMode[3] ;
            bool bGateOpen  = lst[iStep].Gate .Mode == clGMode[4] ;
            if(!bFocusOpen) SEQ.ConverTech.SendA(1);
            else            SEQ.ConverTech.Stat[1].Clear(false);
            if(!bGateOpen ) SEQ.ConverTech.SendA(2);
            else            SEQ.ConverTech.Stat[2].Clear(false);

            bReading = true;
            Stat[0] = SEQ.ConverTech.Stat[0];
            Stat[1] = SEQ.ConverTech.Stat[1];
            Stat[2] = SEQ.ConverTech.Stat[2];
            bReading = false;
        }


        public void GetDataD()
        {

            //TODO :: 테스트를 위해 잠시 주석.
            //SEQ.Daegyum.SendGate  ();
            //SEQ.Daegyum.SendFocus (); //TODO :: 응답 없음으로 잠시 주석
            bool bSwt = lst[iStep].Cathode.Mode == clCMode[0] ; 
            if(bSwt) SEQ.Daegyum.SendCathod();
            
            bReading = true;
            OM.CmnOptn.iArcCount2 = SEQ.Daegyum.Stat.iArc   ; 
            
            if(bSwt) {
                StatD.dCathod         = SEQ.Daegyum.Stat.dCathod; 
                StatD.dFocus          = SEQ.Daegyum.Stat.dFocus ;  
                StatD.dGate           = SEQ.Daegyum.Stat.dGate  ; 
            }
            else     {
                StatD.dCathod         = 0.0                     ;  
                StatD.dFocus          = 0.0                     ;  
                StatD.dGate           = 0.0                     ; 
            }
            bReading = false;
            


            
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
            bool b1 = SEQ.Daegyum.dOffTime != lst[iStep].Cathode.OffTime * 10 ;
            bool b2 = SEQ.Daegyum.dOnTime  != lst[iStep].Cathode.OnTime  * 10 ;
            if(b1 || b2) SEQ.Daegyum.SendOutOnOff(false);

            if(b1)
            {
                SEQ.Daegyum.SendOffTime(lst[iStep].Cathode.OffTime*10);
            }
            if(b2)
            {
                SEQ.Daegyum.SendOnTime (lst[iStep].Cathode.OnTime *10); //1 -> 100usec
            }
            if(!SEQ.Daegyum.bOut) SEQ.Daegyum.SendOutOnOff(true);
            //Trace(SEQ.Daegyum.dOnTime.ToString() + " " + SEQ.Daegyum.dOffTime.ToString());
            
        }


        public void StopWatch()
        {
            sw     .Stop();
            swStart.Stop();
            Arc    .Stop();
            
        }

        public void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }

        public bool StartCon()
        {
            Arc.Restart(); //아킹 검출후 2초 패스용
            //swStart.Reset();
            swStart.Restart();
            iArcTime = 0; //처음에는 바로 카운트 추가하기용
            bStart     = true ; //처음에는 아킹카운트 2초간 안할라고 넣음
            bStep1Time = true ; //게이트 전류유지에서 처음에랑 스텝 넘어갈때 한번만 바로 할라고 넣음
            SEQ.ConverTech.SendB(0,0); //전류
            SEQ.ConverTech.SendB(1,0); //전류
            SEQ.ConverTech.SendB(2,0); //전류
                                             
            SEQ.ConverTech.SendC(0,60); //전압
            SEQ.ConverTech.SendC(1,50); //전압
            SEQ.ConverTech.SendC(2,50); //전압

            SEQ.ConverTech.SendD(0); //On
            SEQ.ConverTech.SendD(1); //On
            SEQ.ConverTech.SendD(2); //On

            //SEQ.Daegyum.SendOutOnOff(true);
            SEQ.Daegyum.dOnTime  = 0 ;
            SEQ.Daegyum.dOffTime = 0 ;

            SEQ.ConverTech.Stat[0].dSetVoltage = 0 ; dAPre = 0 ;
            SEQ.ConverTech.Stat[1].dSetVoltage = 0 ; dFPre = 0 ;
            SEQ.ConverTech.Stat[2].dSetVoltage = 0 ; dGPre = 0 ;
            //중간에 병신낫을때 초기화용으로 여기다 넣음
            //SEQ.ConverTech.Stat[0].bRcvEnd = false;
            //SEQ.ConverTech.Stat[1].bRcvEnd = false;
            //SEQ.ConverTech.Stat[2].bRcvEnd = false;
            SEQ.ConverTech.SendA(0);
            //Delay(15);
            SEQ.ConverTech.SendA(1);
            //Delay(15);
            SEQ.ConverTech.SendA(2);

            TimeClear();

            //dPreSetvF = 0;
            //dPreSetvG = 0;
            //bPreDetected = false;
            bReceive = false;
            return true;
        }

        public bool StopCon()
        {
            if(lst[iStep].Gate.EndmA > 0) {
                ClearStep();
            }

            StopWatch();
            
            Arc.Stop(); //아킹 검출후 2초 패스용

            SEQ.ConverTech.SendE(0); //Off
            SEQ.ConverTech.SendE(1); //Off
            SEQ.ConverTech.SendE(2); //Off
            SEQ.Daegyum.SendOutOnOff(false);

            if(OM.EqpOptn.bClearStop)
            {
                ClearStep();
            }

            //TODO :: 일단 오픈상태로 스탑하기
            //IO_SetY(yi.CathodeGND,false);
            //IO_SetY(yi.CathodeSWT,false);
            //IO_SetY(yi.GateGND   ,false);
            //IO_SetY(yi.GatePower ,false);
            //IO_SetY(yi.FocusGND  ,false);
            //IO_SetY(yi.FocusPower,false);

            //CEnd();
            return true;
        }

        bool bStep1Time = false;
        public bool Total()
        {
            if (iPreStep != iStep) Trace("Step Changed : " + iPreStep.ToString() + " -> " + iStep.ToString());
            iPreStep = iStep;
            
            int  iNo       = iStep ;
            bool bTimeOut1 = false ;
            bool bTimeOut2 = false ;
            bool bTimeOut3 = false ;
            bool bTimeOut4 = false ;
            
            //0. Error List Step Delay Check
            if (ER_IsErr (    )) return false;
            if (!CheckList(iNo)) return false;
            if (bStepDelay && !m_tmCycle.OnDelay(bStepDelay, OM.CmnOptn.iStepDelay)) {
                m_tmTimeOut1.Clear();
                return false;
            }
            
            //SendLive(); //대겸에 1초마다 신호주기
            bStepDelay = false;
            if(!sw.IsRunning) sw.Start();
            int  msec      = (int)sw.ElapsedMilliseconds ;
            lst[0].Total.msec = msec ;


            //1. Trans And Check
            if(!AnodeTrans(iNo)) bTimeOut1 = true;
            if(!GateTrans (iNo)) bTimeOut2 = true; 
            if(!FocusTrans(iNo)) bTimeOut3 = true;
            //if(!SEQ.Daegyum.bOut) {
            //    SEQ.Daegyum.SendOutOnOff(true);
            //    bTimeOut4 = true;
            //}

            if (m_tmTimeOut1.OnDelay(bTimeOut1 || bTimeOut2 || bTimeOut3 || bTimeOut4, 1000)) {
                if(bTimeOut1) ER_SetErr(ei.AGG_CathodTrans);
                if(bTimeOut2) ER_SetErr(ei.AGG_GateTrans  );
                if(bTimeOut3) ER_SetErr(ei.AGG_FocusTrans );
                return false;
            }
            if(bTimeOut1 || bTimeOut2 || bTimeOut3 || bTimeOut4) return false;

            double dStepTime = 0;

            

            //2. Next Step
            double dStepTotalTime = 0;
            dStepTime      = 0;
            //double dStepNext      = 0;
            //if(iStep + 1 < lst.Count) dStepNext = iStep +1 ;
            //else                      dStepNext = iStep    ;

            //for(int i=0; i<= iStep; i++) dStepTime += SEQ.aging.lst[i].Total.Time;
            for(int i=0; i<= iStep; i++) dStepTime += GetTime(i);
            if(msec >= dStepTime * 1000.0)
            {
                if(iStep+1 < lst.Count) //레시피 와 레시피 딜레이
                {
                    if(lst[iNo].Gate.Mode == clGMode[1]){//clGMode = { "전압유지","전압증가","전류유지","전압계승","GND" };
                        if(Math.Abs(lst[iNo].Gate.EndmA - StatD.dCathod) > 1.0 )//&& StatD.dCathod != 0)
                        {
                            ER_SetErr(ei.AGG_GateEndmA);
                            ClearStep();
                            return false;
                        } 
                    }
                    iStep++; 
                    bStep1Time = true;
                    if(lst[iStep-1].Total.No != lst[iStep].Total.No)
                    {
                        bStepDelay = true; sw.Stop(); m_tmCycle.Clear(); bReceive = false; bStepChange = true;
                        
                        //TODO :: 확인 필요
                        SEQ.ConverTech.SendB(0,0); //전압
                        SEQ.ConverTech.SendB(1,0); //전압
                        SEQ.ConverTech.SendB(2,0); //전압
                                                         
                        SEQ.ConverTech.SendE(0); //Off
                        SEQ.ConverTech.SendE(1); //Off
                        SEQ.ConverTech.SendE(2); //Off

                        SEQ.Daegyum.SendReset (); //캐소드 리밋 에러 땜에 추가
                        SEQ.Daegyum.SendReset1(); //캐소드 리밋 에러 땜에 추가
                        SEQ.Daegyum.SendReset2(); //캐소드 리밋 에러 땜에 추가



                        m_tmTimeOut1.Clear();
                        return false;
                    }
                }
                //3. End Check
                //if(iStep >= lst.Count)
                else
                {

                    return true;
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

            

            //4. Play 
            dStepTime = 0;
            //for(int i=0; i< iStep; i++) dStepTime += SEQ.aging.lst[i].Total.Time;
            for(int i=0; i< iStep; i++) dStepTime += GetTime(i);
            int iStepmsec = (int)(msec - dStepTime*1000.0) ;
            if (!bReceive) {
                //m_tmCycle.Clear();
                //Delay(10);
                SetDataD();
                Anode(iStep,iStepmsec);
                Gate (iStep,iStepmsec);
                Focus(iStep,iStepmsec);
                bReceive = true;
                bStepChange = false;
                
            }
            else {
                GetData ();
                GetDataD();
                bReceive = false;

                //Graph
                if (m_tmGraph.OnDelay(true, 100)){ //|| msec < 100) {
                    m_tmGraph.Clear();

                    
                    //GetDataD();

                    double dPreTime = 0;
                    //for(int i=0; i<iStep; i++) dPreTime += SEQ.aging.lst[i].Total.Time;
                    for(int i=0; i<iStep; i++) dPreTime += SEQ.aging.GetTime(i);
                    //string sTime = (TimeSpan.FromSeconds(dPreTime) + TimeSpan.FromMilliseconds(lst[iStep].Total.msec)).ToString(); 
                    //string sTime = (lst[iStep].Total.msec / 1000.0).ToString("N1") ;
                    //string sTime = (lst[iStep].Total.msec / 1000.0).ToString("N1") ;
                    //string sTime = (lst[iStep].Total.msec).ToString() ;
                    //string sTime = (lst[iStep].Total.msec / 1000).ToString() ;
                    
                    System.DateTime dt = new System.DateTime(1983,11,08,0,0,0);
                    //dt.AddMilliseconds(lst[iStep].Total.msec);
                    //CBegin();
                    //chart1.Series[0].Points.AddXY(x.AddSeconds(10), 34);
                    //chart1.Series[0].Points.AddXY(x.AddSeconds(30), 334);
                    CAddPoints1C(dt.AddMilliseconds(msec), Stat[0].dCurrent.ToString());
                    CAddPoints2C(dt.AddMilliseconds(msec), StatD  .dCathod .ToString());
                    CAddPoints3C(dt.AddMilliseconds(msec), Stat[1].dCurrent.ToString());
                    CAddPoints4C(dt.AddMilliseconds(msec), StatD  .dFocus  .ToString());
                    CAddPoints5C(dt.AddMilliseconds(msec), Stat[2].dCurrent.ToString());
                    CAddPoints6C(dt.AddMilliseconds(msec), StatD  .dGate   .ToString());

                    CAddPoints1 (dt.AddMilliseconds(msec), Stat[0].dSetVoltage.ToString());
                    CAddPoints2 (dt.AddMilliseconds(msec), Stat[0].dVoltage   .ToString());
                    CAddPoints3 (dt.AddMilliseconds(msec), Stat[1].dSetVoltage.ToString());
                    CAddPoints4 (dt.AddMilliseconds(msec), Stat[1].dVoltage   .ToString());
                    CAddPoints5 (dt.AddMilliseconds(msec), Stat[2].dSetVoltage.ToString());
                    CAddPoints6 (dt.AddMilliseconds(msec), Stat[2].dVoltage   .ToString());

                    
                    CEnd();
                    //Random rd = new Random();
                    //CAddPoints1(sTime, rd.Next(0,100)   .ToString());
                    //CAddPoints2(sTime, rd.Next(0,100)   .ToString());
                    //CAddPoints3(sTime, rd.Next(0,100)   .ToString());
                    //CAddPoints4(sTime, rd.Next(0,100)   .ToString());
                    //CAddPoints5(sTime, rd.Next(0,100)   .ToString());
                    //CAddPoints6(sTime, rd.Next(0,100)   .ToString());
                    //CEnd();
                    
                }

                //Limit Check
                bool b1 = StatD.dCathod    < lst[iStep].Cathode.MinmA ;
                bool b2 = StatD.dCathod    > lst[iStep].Cathode.MaxmA ;
                bool b3 = Stat[0].dCurrent < lst[iStep].Cathode.MinmA ;
                bool b4 = Stat[0].dCurrent > lst[iStep].Cathode.MaxmA ;
                //if(b1 || b2 || b3 || b4) { 
                if(swStart.ElapsedMilliseconds > 2000)
                { 
                    if(lst[iStep].Cathode.Mode == clCMode[0]){ //clCMode = { "SWITCHING","GND","OPEN" };
                        if(b1) ML.ER_SetErr(ei.AGG_CathodLimit,"Check the Min Limit (" + StatD.dCathod   .ToString() + "<" + lst[iStep].Cathode.MinmA.ToString() + ")"); 
                        if(b2) ML.ER_SetErr(ei.AGG_CathodLimit,"Check the Max Limit (" + StatD.dCathod   .ToString() + ">" + lst[iStep].Cathode.MaxmA.ToString() + ")");
                        if(b1 || b2)
                        {
                            ClearStep();
                            return false; 
                        }
                    }
                    else { 
                        if(b3) ML.ER_SetErr(ei.AGG_CathodLimit,"Check the Min Limit (" + Stat[0].dCurrent.ToString() + "<" + lst[iStep].Cathode.MinmA.ToString() + ")"); 
                        if(b4) ML.ER_SetErr(ei.AGG_CathodLimit,"Check the Max Limit (" + Stat[0].dCurrent.ToString() + ">" + lst[iStep].Cathode.MaxmA.ToString() + ")"); 
                        if(b3 || b4)
                        {
                            ClearStep();
                            return false; 
                        }
                    }
                }  
                    
                //}
            }

            if(CheckArc())
            {
                //ClearStep();
                //return false; 
            }            
            
            
            

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
            //if(!Stat[0].bRcvEnd) return;
            int    iNo  = _iNo   ;
            
            //Set
            double dSttv        = lst[iNo].Anode.StartkV ;
            double dStpv        = lst[iNo].Anode.StopkV  ;
            double dAddv        ;
            double dSetv        ;
            int    imsec        = (int)_dSec; //msec
            //public static readonly string[] clAMode = { "전압증가","전압유지","전압반복" };
            if(lst[iNo].Anode.Mode == clAMode[0]) //전압증가
            {
                if(lst[iNo].Anode.IncTime < 1) lst[iNo].Anode.IncTime = 1;
                //dAddv = lst[iNo].Anode.InckV   * 1000 * (imsec/(lst[iNo].Anode.IncTime*1000));
                //dAddv = Math.Round(lst[iNo].Anode.InckV * (imsec/(lst[iNo].Anode.IncTime*1000.0)),2); //시간에 따라 리니어 하게 증가
                dAddv = lst[iNo].Anode.InckV * (int)(imsec/(lst[iNo].Anode.IncTime*1000.0)); //시간에 따라 리니어 하게 증가
                dSetv = dSttv+dAddv;
                if(dSetv > dStpv) dSetv = dStpv;
                
                if(Stat[0].dSetVoltage != dSetv) SEQ.ConverTech.SendB(0,dSetv);
                //SEQ.ConverTech.SendD(0); //On
            }
            else if(lst[iNo].Anode.Mode == clAMode[1]) //전압유지
            {
                if(Stat[0].dSetVoltage != dSttv) SEQ.ConverTech.SendB(0,dSttv);
            }
            else if(lst[iNo].Anode.Mode == clAMode[2]) //전압반복
            {
                int AllTime = (int)((lst[iNo].Anode.OnTime + lst[iNo].Anode.OffTime)*1000.0) ;
                int  iOn = imsec % AllTime;
                bool bOn = iOn <=  lst[iNo].Anode.OnTime*1000.0 ;
                if (bOn)
                {
                    if(Stat[0].dSetVoltage != dSttv) SEQ.ConverTech.SendB(0,dSttv); //On
                    //SEQ.ConverTech.SendD(0); //On
                }
                else
                {
                    if(Stat[0].dSetVoltage != dStpv) SEQ.ConverTech.SendB(0,dStpv); //Off
                    //SEQ.ConverTech.SendE(0); //Off
                }
            }

            //전압 증가일때만 아킹 검사
            if(lst[iNo].Anode.Mode == clAMode[0]) //전압증가
            {
                bInspection = true;
            }
            else
            {
                bInspection = false;
            }               


        }

        double dPreSetvG = 0;
        public void Gate(int _iNo, double _dSec)
        {
            //if(!Stat[2].bRcvEnd) return;
            int    iNo  = _iNo   ;
            
            double dSttv        = lst[iNo].Gate.StartV ;
            double dAddv        ;
            double dSetv        = 0;
            int    imsec        = (int)_dSec;
            //public static readonly string[] clGMode = { "전압유지","전압증가","전류유지","전압계승","GND","OPEN" };
            if(lst[iNo].Gate.Mode == clGMode[0]) //전압유지
            {
                dSetv = dSttv;
                if(Stat[2].dSetVoltage != dSttv/1000) SEQ.ConverTech.SendB(2,dSttv/1000);
            }
            else if(lst[iNo].Gate.Mode == clGMode[1] || lst[iNo].Gate.Mode == clGMode[5]) //전압증가
            {
                if(lst[iNo].Gate.IncTime < 1) lst[iNo].Gate.IncTime = 1 ;
                //dAddv = lst[iNo].Gate.IncV * (imsec/(lst[iNo].Gate.IncTime*1000.0)); //시간에 따라 리니어 하게 증가
                dAddv = lst[iNo].Gate.IncV * (int)(imsec/(lst[iNo].Gate.IncTime*1000.0)); //
                
                if(dSttv < 0) {
                    lst[iNo].Gate.StartV = dPreSetvG ;
                    dSttv = lst[iNo].Gate.StartV;
                }

                dSetv = dSttv+dAddv;
                if(dSetv               >  lst[iNo].Gate.StopV) dSetv = lst[iNo].Gate.StopV;
                //if(lst[iNo].Gate.EndmA <= Stat[1].dCurrent   ) dSetv = Stat[1].dSetVoltage; 
                if(lst[iNo].Gate.EndmA != 0 && !bStepChange)
                { 
                    if(lst[iNo].Gate.EndmA <= StatD.dCathod && dPreSetvG != 0) 
                    {
                        lst[iNo].Gate.EndTime = imsec / 1000 + 1;
                        dSetv = dPreSetvG ;
                    }
                }

                //SEQ.ConverTech.SendB(2,Math.Round(dSetv/1000,3));
                if(Stat[2].dSetVoltage != dSetv/1000) SEQ.ConverTech.SendB(2,dSetv/1000);

            }
            else if(lst[iNo].Gate.Mode == clGMode[2]) //전류유지
            {
                double dTime = lst[iNo].Cathode.OnTime + lst[iNo].Cathode.OffTime ;

                if (m_tmKeep.OnDelay(true, (int)dTime) || bStep1Time){ //|| msec < 100) {
                    bStep1Time = false;
                    m_tmKeep.Clear();
                    dSetv = dPreSetvG ;
                    double dMin = lst[iNo].Gate.KeepmA - OM.CmnOptn.dKeepGateV_A ;
                    double dMax = lst[iNo].Gate.KeepmA + OM.CmnOptn.dKeepGateV_A ;
                    bool bOk = dMin <= StatD.dCathod && StatD.dCathod  <= dMax ;
                    if (!bOk) {
                        if(StatD.dCathod < lst[iNo].Gate.KeepmA) dSetv += OM.CmnOptn.dKeepGateV_P  ; 
                        if(StatD.dCathod > lst[iNo].Gate.KeepmA) dSetv -= OM.CmnOptn.dKeepGateV_M  ;
                    }
                    bool bOver = lst[iNo].Gate.StopV < dSetv || 6000 < dSetv ;
                    if(lst[iNo].Gate.StopV != 0 && bOver) {
                        ER_SetErr(ei.AGG_GateOverStop);
                        ClearStep();
                        return;
                    }

                    
                    
                    //if(Stat[0].dSetVoltage < lst[iNo].Gate.KeepmA) dSetv += (OM.CmnOptn.dKeepGateV) ; //테스트용 
                    //if(Stat[0].dSetVoltage > lst[iNo].Gate.KeepmA) dSetv -= (OM.CmnOptn.dKeepGateV) ;
                         if(dSetv >  6000) dSetv = 6000;
                    else if(dSetv <= 0   ) dSetv = 0   ;
                    if(Stat[2].dSetVoltage != dSetv/1000) SEQ.ConverTech.SendB(2,dSetv/1000);
                }
            }
            else if(lst[iNo].Gate.Mode == clGMode[3]) //전압계승
            {
                //dSetv = Stat[1].dSetVoltage; 
                dSetv = dPreSetvG ;
                if(Stat[2].dSetVoltage != dSetv/1000) SEQ.ConverTech.SendB(2,dSetv/1000); //+ a
            }

            if(dSetv != 0) dPreSetvG = dSetv;
        }

        double dPreSetvF = 0;
        public void Focus(int _iNo, double _dSec)
        {
            //if(!Stat[1].bRcvEnd) return;
            int    iNo  = _iNo   ;
            
            double dSttv        = lst[iNo].Focus.StartV ;
            double dAddv        ;
            double dSetv        = 0;
            int    imsec        = (int)_dSec;
            //public static readonly string[] clFMode = { "전압유지","전압증가","GND","OPEN" };
            if(lst[iNo].Focus.Mode == clFMode[0]) //전압유지
            {
                dSetv = dPreSetvF ;
                if(Stat[1].dSetVoltage != dSttv/1000) SEQ.ConverTech.SendB(1,dSttv/1000);
            }
            else if(lst[iNo].Focus.Mode == clFMode[1]) //전압증가
            {
                if(lst[iNo].Focus.IncTime < 1) lst[iNo].Focus.IncTime = 1 ;
                //dAddv = lst[iNo].Focus.IncV * (imsec/(lst[iNo].Focus.IncTime*1000.0)); //시간에 따라 리니어 하게 증가
                dAddv = lst[iNo].Focus.IncV * (int)(imsec/(lst[iNo].Focus.IncTime*1000.0)); //
                dSetv = dSttv+dAddv;
                if(dSetv > lst[iNo].Focus.StopV) dSetv = lst[iNo].Focus.StopV;
                //SEQ.ConverTech.SendB(1,Math.Round(dSetv/1000,3));
                if(Stat[1].dSetVoltage != dSetv/1000) SEQ.ConverTech.SendB(1,dSetv/1000);

            }
            //SetDataD();

            if(dSetv != 0) dPreSetvF = dSetv;
        }

        public bool AnodeTrans(int _iNo)
        {
            bool   bRet = false;
            int    iNo  = _iNo   ;

            //public static readonly string[] clCMode = { "SWITCHING","GND","OPEN" };
            if(lst[iNo].Cathode.Mode == clCMode[0]) //SWITCHING
            {
                if(IO_GetX(xi.CathodeSWT) && !IO_GetX(xi.CathodeGND) && Stat[0].bOutput) bRet = true;
                else
                {
                    SEQ.ConverTech.SendD(0); //On
                    SEQ.ConverTech.SendA(0); //
                    bReading = true ;
                    Stat[0].bOutput = SEQ.ConverTech.Stat[0].bOutput;
                    bReading = false;
                    IO_SetY(yi.CathodeSWT,true );
                    IO_SetY(yi.CathodeGND,false);

                }
            }
            else if(lst[iNo].Cathode.Mode == clCMode[1]) //GND
            {
                if(!IO_GetX(xi.CathodeSWT) && IO_GetX(xi.CathodeGND) && Stat[0].bOutput) bRet = true;
                else
                {
                    //SEQ.ConverTech.SendB(0,0); //전류
                    SEQ.ConverTech.SendD(0); //On
                    SEQ.ConverTech.SendA(0); //

                    Stat[0].bOutput = SEQ.ConverTech.Stat[0].bOutput;

                    IO_SetY(yi.CathodeSWT,false);
                    IO_SetY(yi.CathodeGND,true );
                }
            }
            else //OPEN
            {
                if(!IO_GetX(xi.CathodeSWT) && !IO_GetX(xi.CathodeGND) && Stat[0].bOutput) bRet = true;
                else
                {
                    //SEQ.ConverTech.SendB(0,0); //전류
                    SEQ.ConverTech.SendD(0);   //On
                    SEQ.ConverTech.SendA(0);   //

                    Stat[0].bOutput = SEQ.ConverTech.Stat[0].bOutput;

                    IO_SetY(yi.CathodeSWT,false);
                    IO_SetY(yi.CathodeGND,false);
                }

                
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
                if(!IO_GetX(xi.GatePower) && IO_GetX(xi.GateGND) && !Stat[2].bOutput) bRet = true;
                else
                {
                    SEQ.ConverTech.SendB(2,0); //전류
                    SEQ.ConverTech.SendE(2);   //Off
                    SEQ.ConverTech.SendA(2);   //

                    Stat[2].bOutput = SEQ.ConverTech.Stat[2].bOutput;

                    IO_SetY(yi.GatePower ,false);
                    IO_SetY(yi.GateGND   ,true );
                }
            }
            //else if(lst[iNo].Gate.Mode == clGMode[5]) //OPEN
            //{
            //    IO_SetY(yi.GatePower ,false);
            //    IO_SetY(yi.GateGND   ,false);

            //    if(!IO_GetX(xi.GatePower) && !IO_GetX(xi.GateGND)) bRet = true;
            //}
            else //
            {
                if(IO_GetX(xi.GatePower) && !IO_GetX(xi.GateGND) && Stat[2].bOutput) bRet = true;
                else
                {
                    SEQ.ConverTech.SendD(2);   //On
                    SEQ.ConverTech.SendA(2);   //

                    Stat[2].bOutput = SEQ.ConverTech.Stat[2].bOutput;

                    IO_SetY(yi.GatePower ,true );
                    IO_SetY(yi.GateGND   ,false);
                }
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
                if(!IO_GetX(xi.FocusPower) && IO_GetX(xi.FocusGND) && !Stat[1].bOutput) bRet = true;
                else
                {
                    SEQ.ConverTech.SendB(1,0); //전류
                    SEQ.ConverTech.SendE(1);   //Off
                    SEQ.ConverTech.SendA(1);   //

                    Stat[1].bOutput = SEQ.ConverTech.Stat[1].bOutput;

                    IO_SetY(yi.FocusPower,false);
                    IO_SetY(yi.FocusGND  ,true );
                }
            }
            else if(lst[iNo].Focus.Mode == clFMode[3]) //GND//OPEN
            {
                if(!IO_GetX(xi.FocusPower) && !IO_GetX(xi.FocusGND) && !Stat[1].bOutput) bRet = true;
                else
                {
                    SEQ.ConverTech.SendB(1,0); //전류
                    SEQ.ConverTech.SendE(1);   //Off
                    SEQ.ConverTech.SendA(1);   //

                    Stat[1].bOutput = SEQ.ConverTech.Stat[1].bOutput;

                    IO_SetY(yi.FocusPower,false);
                    IO_SetY(yi.FocusGND  ,false);
                }
            }
            else
            {
                if( IO_GetX(xi.FocusPower) && !IO_GetX(xi.FocusGND) && Stat[1].bOutput) bRet = true;
                else
                {
                    SEQ.ConverTech.SendD(1); //On
                    SEQ.ConverTech.SendA(1);   //

                    Stat[1].bOutput = SEQ.ConverTech.Stat[1].bOutput;

                    IO_SetY(yi.FocusPower,true );
                    IO_SetY(yi.FocusGND  ,false);
                }
            }

            return bRet;
        }

        public double GetTime(int _iStep)
        {
            double dTime = 0;
            if(_iStep <= SEQ.aging.lst.Count) {
                if(SEQ.aging.lst[_iStep].Gate.EndTime > 0) dTime = SEQ.aging.lst[_iStep].Gate.EndTime;
                else                                       dTime = SEQ.aging.lst[_iStep].Total.Time  ;
            }

            return dTime;
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
            //if(_iTag != -1) sTag = string.Format("{0:00}", _iTag    );
            //else            sTag = string.Format("{0:00}", m_iPartId);
            sTag = string.Format("{0:00}", m_iPartId);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            Log.SendMessage(sFullMsg);
        }

    }

    public class Data
    {
        public struct TTotal
        {
            private string no       ;
            private string name     ;
            private double time     ;
            

            public string No        { get { return no       ;} set { if(value == "") value = Aging.clNo[0]; no   = value;} }
            public string Name      { get { return name     ;} set { if(value == "") value = "NoName";      name = value;} }
            public double Time      { get { return time     ;} set { if(value<OM.CmnOptn.min11) value = OM.CmnOptn.min11; if(value>OM.CmnOptn.max11) value = OM.CmnOptn.max11; time      = value;} }
            

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
            private double rptCnt  ;
            
            public string Mode    { get { return mode   ;} set { if(value == "") value = Aging.clAMode[0]; mode = value;} }
            public double StartkV { get { return startkV;} set { if(value<OM.CmnOptn.min21) value = OM.CmnOptn.min21; if(value>OM.CmnOptn.max21) value = OM.CmnOptn.max21; startkV = value;} }
            public double StopkV  { get { return stopkV ;} set { if(value<OM.CmnOptn.min22) value = OM.CmnOptn.min22; if(value>OM.CmnOptn.max22) value = OM.CmnOptn.max22; stopkV  = value;} }
            public double InckV   { get { return inckV  ;} set { if(value<OM.CmnOptn.min23) value = OM.CmnOptn.min23; if(value>OM.CmnOptn.max23) value = OM.CmnOptn.max23; inckV   = value;} }
            public double IncTime { get { return incTime;} set { if(value<OM.CmnOptn.min24) value = OM.CmnOptn.min24; if(value>OM.CmnOptn.max24) value = OM.CmnOptn.max24; incTime = value;} }
            public double OnTime  { get { return onTime ;} set { if(value<OM.CmnOptn.min25) value = OM.CmnOptn.min25; if(value>OM.CmnOptn.max25) value = OM.CmnOptn.max25; onTime  = value;} }
            public double OffTime { get { return offTime;} set { if(value<OM.CmnOptn.min26) value = OM.CmnOptn.min26; if(value>OM.CmnOptn.max26) value = OM.CmnOptn.max26; offTime = value;} }
            public double RptCnt  { get { return rptCnt ;} set { if(value<OM.CmnOptn.min12) value = OM.CmnOptn.min12; if(value>OM.CmnOptn.max12) value = OM.CmnOptn.max12; rptCnt  = value;} }

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
            private double endTime ;

            public string Mode    { get { return mode   ;} set { if(value == "") value = Aging.clGMode[0]; mode = value;} }
            public double StartV  { get { return startV ;} set { if(value<OM.CmnOptn.min31) value = OM.CmnOptn.min31; if(value>OM.CmnOptn.max31) value = OM.CmnOptn.max31; startV  = value;} }
            public double StopV   { get { return stopV  ;} set { if(value<OM.CmnOptn.min32) value = OM.CmnOptn.min32; if(value>OM.CmnOptn.max32) value = OM.CmnOptn.max32; stopV   = value;} }
            public double IncV    { get { return incV   ;} set { if(value<OM.CmnOptn.min33) value = OM.CmnOptn.min33; if(value>OM.CmnOptn.max33) value = OM.CmnOptn.max33; incV    = value;} }
            public double IncTime { get { return incTime;} set { if(value<OM.CmnOptn.min34) value = OM.CmnOptn.min34; if(value>OM.CmnOptn.max34) value = OM.CmnOptn.max34; incTime = value;} }
            public double KeepmA  { get { return keepmA ;} set { if(value<OM.CmnOptn.min35) value = OM.CmnOptn.min35; if(value>OM.CmnOptn.max35) value = OM.CmnOptn.max35; keepmA  = value;} }
            public double EndmA   { get { return endmA  ;} set { if(value<OM.CmnOptn.min36) value = OM.CmnOptn.min36; if(value>OM.CmnOptn.max36) value = OM.CmnOptn.max36; endmA   = value;} }
            public double EndTime { get { return endTime;} set { endTime   = value;} }

            public void Clear()
            {
                endTime = 0;
            }
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
            Gate .Clear();

            Total   = _Total   ;
            Anode   = _Anode   ;
            Gate    = _Gate    ;
            Focus   = _Focus   ;
            Cathode = _Cathode ;
        }

        public void Clear()
        {
            Total.Clear();
            Gate .Clear();
        }



    }
}
