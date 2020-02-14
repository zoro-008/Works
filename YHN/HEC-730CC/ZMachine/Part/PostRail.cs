using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class PostRail : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop   ;
            public void Clear()
            {
                bWorkEnd    = false ;
                bReqStop    = false ;
            }
        };   

        public enum sc
        {
            Idle    = 0    ,
            Clear          ,
            Clean          ,
            LiftUp         ,
            LiftDown       ,
            Out            ,
            MAX_SEQ_CYCLE
        };

        public struct SStep
        {
            public int iHome   ;
            public int iToStart;
            public sc  eSeq    ;
            public int iCycle  ;
            public int iToStop ;
            public sc  eLastSeq;
            public void Clear()
            {
                iHome    = 0;
                iToStart = 0;
                eSeq     = sc.Idle;
                iCycle   = 0;
                iToStop  = 0;
                eLastSeq = sc.Idle;
            }
        };

        protected String      m_sPartName;
        protected int         m_iPartId  ;
        //Timer.
        protected CDelayTimer m_tmMain   ;
        protected CDelayTimer m_tmCycle  ;
        protected CDelayTimer m_tmHome   ;
        protected CDelayTimer m_tmToStop ;
        protected CDelayTimer m_tmToStart;
        protected CDelayTimer m_tmDelay  ;        

        protected SStat Stat;
        protected SStep Step, PreStep;

        protected double m_dLastIdxPos;
        protected String m_sCheckSafeMsg;

        public CTimer[] m_CycleTime;

        public bool m_bVacSkip1 = false;
        public bool m_bVacSkip2 = false;
        public bool m_bVacSkip3 = false;
        public bool m_bVacSkip4 = false;
        public bool m_bVacSkip5 = false;
        public bool m_bVacSkip6 = false;

        public PostRail(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();

            m_CycleTime   = new CTimer[(int)sc.MAX_SEQ_CYCLE];

            for(int i = 0 ; i < (int)sc.MAX_SEQ_CYCLE ; i++)
            {
                m_CycleTime [i]  = new CTimer();
            }

            m_iPartId = _iPartId;

            Reset();                  
        }
        protected double GetLastCmd(mi _iMotr)
        {
            double dLastIdxPos = 0.0;

            return dLastIdxPos;
        }
        protected void ResetTimer()
        {
            //Clear Timer.
            m_tmMain   .Clear();
            m_tmCycle  .Clear();
            m_tmHome   .Clear();
            m_tmToStop .Clear();
            m_tmToStart.Clear();
            m_tmDelay  .Clear();
        }

        public SStep GetStep() { return Step; }
        #endregion 

        //PartInterface 부분.
        //인터페이스 상속.====================================================================================================================
        #region Interface
        override public void Reset() //리셑 버튼 눌렀을때 타는 함수.
        {

            ResetTimer();

            Stat.Clear();
            Step.Clear();
            PreStep.Clear();
        }

        //Running Functions.
        override public void Update()
        {

        }
        override public bool ToStopCon() //스탑을 하기 위한 조건을 보는 함수.
        {
            Stat.bReqStop = true;
            //During the auto run, do not stop.
            if (Step.eSeq != sc.Idle) return false;

            Step.iToStop = 10;
            //Ok.
            return true;


        }
        override public bool ToStartCon() //스타트를 하기 위한 조건을 보는 함수.
        {
            Step.iToStart = 10;
            //Ok.
            return true;

        }
        override public bool ToStart() //스타트를 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 8000))
            {
                ER_SetErr(ei.ETC_ToStartTO, m_sPartName );
            }

            String sTemp = String.Format("Step.iToStart={0:00}", Step.iToStart);

            if (Step.iToStart != PreStep.iToStart)
            {
                Trace(sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: 
                    Step.iToStart = 0;
                    return true;

                case 10:
                    //if(!IO_GetX(xi.PSTB_PkgDetect1) && !IO_GetX(xi.PSTB_PkgDetect2))  MoveCyl(ci.PSTB_MarkStprUpDn  ,fb.Fwd);
                    ////MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    //MoveCyl(ci.PSTB_PusherFwBw    ,fb.Bwd);
                    //MoveCyl(ci.PSTB_MarkAlignFWBw ,fb.Bwd);
                    //IO_SetY(yi.RAIL_FeedingAC3,false);
                    Step.iToStart++;
                    return false;

                case 11: 
                    //if(!CL_Complete(ci.PSTB_MarkStprUpDn)) return false;
                    ////if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    //if(!CL_Complete(ci.PSTB_PusherFwBw    ,fb.Bwd)) return false;
                    //if(!CL_Complete(ci.PSTB_MarkAlignFWBw ,fb.Bwd)) return false;
                    //MoveCyl(ci.PSTB_MarkSttnUpDn  ,fb.Bwd);
                    ////MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart);
                    ////MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart);
                    
                    Step.iToStart++;
                    return false;

                case 12:
                    //if(!CL_Complete(ci.PSTB_MarkSttnUpDn  ,fb.Bwd)) return false;
                    //if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart)) return false;
                    //if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart)) return false;
                    
                    Step.iToStart++;
                    return false;
                
                case 13: 
                    
                    Step.iToStart = 0;
                    return true;
                    
            }

        }

        override public bool ToStop() //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 10000)) ER_SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

            String sTemp;
            sTemp = string.Format("Step.iToStop={0:00}", Step.iToStop);
            if (Step.iToStop != PreStep.iToStop)
            {
                Trace(sTemp);
            }

            PreStep.iToStop = Step.iToStop;

            Stat.bReqStop = false;

            //Move Home.
            switch (Step.iToStop)
            {
                default: 
                    Step.iToStop = 0;
                    return true;

                case 10:
                    MT_Stop(mi.PSTR_X);

                    Step.iToStop++;
                    return false;
                    
                case 11: 
                    if(!MT_GetStop(mi.PSTR_X)) return false;

                    Step.iToStop = 0;
                    return true;
            }


        }

        override public int GetHomeStep   () { return      Step.iHome    ; } override public int GetPreHomeStep   () { return PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return      Step.iToStart ; } override public int GetPreToStartStep() { return PreStep.iToStart ; }
        override public int GetSeqStep    () { return (int)Step.eSeq     ; } override public int GetPreSeqStep    () { return (int)PreStep.eSeq     ; }
        override public int GetCycleStep  () { return      Step.iCycle   ; } override public int GetPreCycleStep  () { return      PreStep.iCycle   ; } override public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; }
        override public int GetToStopStep () { return      Step.iToStop  ; } override public int GetPreToStopStep () { return      PreStep.iToStop  ; }
        
        override public string GetCrntCycleName(         ) { return Step.eSeq.ToString();}
        override public String GetCycleName    (int _iSeq) { return ((sc)_iSeq).ToString(); }
        override public double GetCycleTime    (int _iSeq) { return m_CycleTime[_iSeq].Duration; }
        override public String GetPartName     (         ) { return m_sPartName; }

        override public int GetCycleMaxCnt() { return (int)sc.MAX_SEQ_CYCLE; }

        override public int  GetPartId(        ) { return m_iPartId; }
        override public void SetPartId(int _iId) { m_iPartId = _iId; }

        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                //조건
                bool bULDRNone = DM.ARAY[ri.ULDR].CheckAllStat(cs.None   );
                bool bPSTRNone = DM.ARAY[ri.PSTR].CheckAllStat(cs.None   );
                bool bClean    = DM.ARAY[ri.PRER].CheckAllStat(cs.Cleaning) && DM.ARAY[ri.PSTR].CheckAllStat(cs.None);
                bool bLiftUp   = DM.ARAY[ri.PSTR].CheckAllStat(cs.LiftUp );
                bool bLiftDown = DM.ARAY[ri.PSTR].CheckAllStat(cs.Clean  );
                bool bOut      = DM.ARAY[ri.PSTR].CheckAllStat(cs.WorkEnd) && DM.ARAY[ri.ULDR].CheckAllStat(cs.None);
                bool bEnd      = DM.ARAY[ri.LODR].CheckAllStat(cs.None)&& DM.ARAY[ri.PRER].CheckAllStat(cs.None) && DM.ARAY[ri.PSTR].CheckAllStat(cs.None) && 
                                (DM.ARAY[ri.ULDR].CheckAllStat(cs.WorkEnd) || DM.ARAY[ri.ULDR].CheckAllStat(cs.None));
                //bool bRailSsr1 = IO_GetX(xi.PSTR_TrayDetect1);
                bool bRailSsr2 = IO_GetX(xi.PSTR_TrayDetect2);
                bool bRailSsr3 = IO_GetX(xi.PSTR_TrayDetect3);

                //싸이클
                bool isClear     = DM.ARAY[ri.ULDR].CheckAllStat(cs.WorkEnd) && !IO_GetX(xi.PSTR_TrayDetect3);
                bool isClean     = bClean   ;
                bool isLiftUp    = bLiftUp   ; 
                bool isLiftDown  = bLiftDown ; 
                bool isOut       = bOut      ; 
                bool isEnd       = bPSTRNone && bEnd && !OM.MstOptn.bIdleRun; //여기에 레일 마지막단 센서 감지 안될때 조건 넣어야 할 수도 있음.

                //모르는 자제 에러
                //if(bPSTRNone && bRailSsr2) ER_SetErr(ei.PKG_Unknwn,"Post Rail have no data found, but rail sensor detected") ;
                //if(bULDRNone && bRailSsr3) ER_SetErr(ei.PKG_Unknwn,"Unloading have no data found, but rail sensor detected") ;
                
                //카세트 사라짐
                //if(!bPSTRNone && !bRailSsr2) ER_SetErr(ei.PKG_Dispr,"Post Rail have data, but rail sensor not detected") ;
                //if(!bULDRNone && !bRailSsr3) ER_SetErr(ei.PKG_Dispr,"Unloading have data, but rail sensor not detected") ;

                if (ER_IsErr())
                {
                    return false;
                } 
                //Normal Decide Step.
                     if (isClear    ) { DM.ARAY[ri.ULDR].Trace(m_iPartId);                                    Step.eSeq = sc.Clear    ; }
                else if (isClean    ) { DM.ARAY[ri.PRER].Trace(m_iPartId); DM.ARAY[ri.PSTR].Trace(m_iPartId); Step.eSeq = sc.Clean    ; }
                else if (isLiftUp   ) { DM.ARAY[ri.PSTR].Trace(m_iPartId); DM.ARAY[ri.RPCK].Trace(m_iPartId); Step.eSeq = sc.LiftUp   ; }
                else if (isLiftDown ) { DM.ARAY[ri.PSTR].Trace(m_iPartId);                                    Step.eSeq = sc.LiftDown ; }
                else if (isOut      ) { DM.ARAY[ri.PSTR].Trace(m_iPartId); DM.ARAY[ri.ULDR].Trace(m_iPartId); Step.eSeq = sc.Out      ; }
                else if (isEnd      ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle){
                    Trace(Step.eSeq.ToString() +" Start");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }
            
            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default             : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case (sc.Idle      ): return false;
                case (sc.Clear     ): if (!CycleClear()) return false; break;
                case (sc.Clean     ): if (!CycleClean    ()) return false; break;
                case (sc.LiftUp    ): if (!CycleLiftUp   ()) return false; break;
                case (sc.LiftDown  ): if (!CycleLiftDown ()) return false; break;
                case (sc.Out       ): if (!CycleOut      ()) return false; break;
            }
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion        
        
        public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            
            ////bool bRet = DM.ARAY[_iId].FindFrstColRow(ref c,ref r,cs.Rslt0,cs.Rslt1,cs.Rslt2,cs.Rslt3,cs.Rslt4,cs.Rslt5,cs.Rslt6,cs.Rslt7,cs.Rslt8,cs.Rslt9,
            ////             cs.RsltA,cs.RsltB,cs.RsltC,cs.RsltD,cs.RsltE,cs.RsltF,cs.RsltG,cs.RsltH,cs.RsltI,cs.RsltJ,cs.RsltK,cs.RsltL) ;
            //bool bRet = ArayMark.FindFrstColRow(ref c,ref r,cs.Fail);
            bool bRet = false;
            return bRet;
        }

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_HomeTO ,sTemp);
                Trace(sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Trace(sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    return true ;

                case 10:
                    MT_Stop(mi.PSTR_X);
                    CL_Move(ci.PSTR_SttnStprUpDn , fb.Fwd);
                    CL_Move(ci.PSTR_TrayAlignFwBw, fb.Bwd);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.PSTR_X)) return false;
                    if(!CL_Complete(ci.PSTR_SttnStprUpDn , fb.Fwd)) return false;
                    if(!CL_Complete(ci.PSTR_TrayAlignFwBw, fb.Bwd)) return false;
                    
                    CL_Move(ci.PSTR_SttnClampClOp , fb.Bwd);
                    Step.iHome++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTR_SttnClampClOp , fb.Bwd)) return false;
                    CL_Move(ci.PSTR_SttnUpDn, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 13:
                    if (!CL_Complete(ci.PSTR_SttnUpDn, fb.Bwd)) return false;
                    MT_SetPos(mi.PSTR_X, 0.0);
                    MT_SetHomeDone(mi.PSTR_X, true);
                    
                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CycleClear()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                MT_Stop(mi.PSTR_X);
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    DM.ARAY[ri.ULDR].SetStat(cs.None);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleClean()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                MT_Stop(mi.PRER_X);
                IO_SetY(yi.PSTR_IonBlwrTop, false);
                IO_SetY(yi.PSTR_IonBlwrBtm, false);
                IO_SetY(yi.PSTR_AirBlwrTop1, false);
                IO_SetY(yi.PSTR_AirBlwrBtm1, false);
                IO_SetY(yi.PSTR_AirBlwrTop2, false);
                IO_SetY(yi.PSTR_AirBlwrBtm2, false);
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r, c = 0;
            //double dPos = 0;
            bool bIdleRun = OM.MstOptn.bIdleRun;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    OM.EqpStat.dPreSttTime = OM.EqpStat.dSttTime;
                    SEQ.PRER.MoveCyl(ci.PRER_RollerUpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.PRER_RollerUpDn, fb.Bwd)) return false;

                    MT_JogVel(mi.PRER_X, OM.DevInfo.dRailClnSpeed);
                    MT_JogVel(mi.PSTR_X, OM.DevInfo.dRailClnSpeed);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!OM.CmnOptn.bPSTR_IonBtmOff ) IO_SetY(yi.PSTR_IonBlwrBtm, true);
                    if (!OM.CmnOptn.bPSTR_Air1BtmOff) IO_SetY(yi.PSTR_AirBlwrBtm1, true);
                    if (!OM.CmnOptn.bPSTR_Air2BtmOff) IO_SetY(yi.PSTR_AirBlwrBtm2, true);
                    
                    if (!OM.CmnOptn.bPSTR_IonTopOff) IO_SetY(yi.PSTR_IonBlwrTop, true);
                    if (!OM.CmnOptn.bPSTR_Air1TopOff) IO_SetY(yi.PSTR_AirBlwrTop1, true);
                    if (!OM.CmnOptn.bPSTR_Air2TopOff) IO_SetY(yi.PSTR_AirBlwrTop2, true);
                    

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!OM.CmnOptn.bPSTR_IonBtmOff && !IO_GetY(yi.PSTR_IonBlwrBtm)) return false;
                    if (!OM.CmnOptn.bPSTR_Air1BtmOff && !IO_GetY(yi.PSTR_AirBlwrBtm1) && !IO_GetY(yi.PSTR_AirBlwrBtm2)) return false;
                    if (!OM.CmnOptn.bPSTR_Air2BtmOff && !IO_GetY(yi.PSTR_AirBlwrBtm1) && !IO_GetY(yi.PSTR_AirBlwrBtm2)) return false;

                    if (!OM.CmnOptn.bPSTR_IonTopOff && !IO_GetY(yi.PSTR_IonBlwrTop)) return false;
                    if (!OM.CmnOptn.bPSTR_Air1TopOff && !IO_GetY(yi.PSTR_AirBlwrTop1)) return false;
                    if (!OM.CmnOptn.bPSTR_Air2TopOff && !IO_GetY(yi.PSTR_AirBlwrTop2)) return false;


                    MoveCyl(ci.PSTR_SttnStprUpDn, fb.Fwd);
                    SEQ.PRER.MoveCyl(ci.PRER_SttnStprUpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.PSTR_SttnStprUpDn, fb.Fwd)) return false;
                    if (!CL_Complete(ci.PRER_SttnStprUpDn, fb.Bwd)) return false;

                    Step.iCycle++;
                    return false;

                case 15:
                    if (IO_GetXDn(xi.PRER_TrayOutSnsr) || OM.MstOptn.bIdleRun)
                    {
                        DM.ARAY[ri.PRER].SetStat(cs.None);
                        MT_Stop(mi.PRER_X);
                    }
                    
                    if (!IO_GetXDryRun(xi.PSTR_TrayDetect2, bIdleRun)) return false;

                    //MT_Stop(mi.PRER_X);
                    MT_Stop(mi.PSTR_X);

                    Step.iCycle++;
                    return false;

                case 16:
                    //if (!MT_GetStop(mi.PRER_X)) return false;
                    if (!MT_GetStop(mi.PSTR_X)) return false;
                    IO_SetY(yi.PSTR_IonBlwrTop, false);
                    IO_SetY(yi.PSTR_IonBlwrBtm, false);
                    IO_SetY(yi.PSTR_AirBlwrTop1, false);
                    IO_SetY(yi.PSTR_AirBlwrBtm1, false);
                    IO_SetY(yi.PSTR_AirBlwrTop2, false);
                    IO_SetY(yi.PSTR_AirBlwrBtm2, false);

                    //DM.ARAY[ri.PRER].SetStat(cs.None  );
                    DM.ARAY[ri.PSTR].SetStat(cs.LiftUp);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleLiftUp()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                //IO_SetY(yi.RAIL_FeedingAC3,false);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r, c = 0;
            Enum pvCheck = new pv();
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.PSTR_TrayAlignFwBw, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.PSTR_TrayAlignFwBw, fb.Fwd)) return false;

                    Step.iCycle++;
                    return false;

                case 12:
                    
                    MoveCyl(ci.PSTR_SttnUpDn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.PSTR_SttnUpDn, fb.Fwd)) return false;
                    MoveCyl(ci.PSTR_SttnClampClOp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.PSTR_SttnClampClOp, fb.Fwd)) return false;
                    MoveCyl(ci.PSTR_TrayAlignFwBw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.PSTR_TrayAlignFwBw, fb.Bwd)) return false;
                    DM.ARAY[ri.PSTR].SetStat(cs.Empty);
                    

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleLiftDown()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                //IO_SetY(yi.RAIL_FeedingAC3,false);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r, c = 0;
            Enum pvCheck = new pv();
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.PSTR_TrayAlignFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.PSTR_TrayAlignFwBw, fb.Bwd)) return false;
                    MoveCyl(ci.PSTR_SttnClampClOp, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTR_SttnClampClOp, fb.Bwd)) return false;
                    MoveCyl(ci.PSTR_SttnUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.PSTR_SttnUpDn, fb.Bwd)) return false;
                    DM.ARAY[ri.PSTR].SetStat(cs.WorkEnd);
                    if (IO_GetX(xi.PSTR_TrayDetect3))
                    {
                        ER_SetErr(ei.PKG_Unknwn, "Unloading sensor detected.");
                        return true;
                    }

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleOut()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                //IO_SetY(yi.RAIL_FeedingAC3,false);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }
            
            bool r, c ;

            double dRailNorSpeed = SM.MTR.Para[(int)mi.PSTR_X].dRunSpeed;
       
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                //초기화
                case 10:
                    MoveCyl(ci.PSTR_TrayAlignFwBw, fb.Bwd);
                    MoveCyl(ci.PSTR_SttnClampClOp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.PSTR_TrayAlignFwBw, fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTR_SttnClampClOp, fb.Bwd)) return false;
                    MoveCyl(ci.PSTR_SttnUpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTR_SttnUpDn, fb.Bwd)) return false;
                    MT_JogVel(mi.PSTR_X, dRailNorSpeed);
                    
                    Step.iCycle++;
                    return false;

                case 13:
                    MoveCyl(ci.PSTR_SttnStprUpDn , fb.Bwd);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.PSTR_SttnStprUpDn , fb.Bwd)) return false;
                    if(!OM.MstOptn.bIdleRun && !IO_GetXDn(xi.PSTR_TrayDetect2)) return false;

                    Step.iCycle++;
                    return false;

                case 15:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iPSTR_OutDelay)) return false;
                    MoveCyl(ci.PSTR_SttnStprUpDn, fb.Fwd);
                    MT_Stop(mi.PSTR_X);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.PSTR_SttnStprUpDn, fb.Fwd)) return false;
                    if(!MT_GetStop(mi.PSTR_X)) return false;
                    if (!OM.MstOptn.bIdleRun)
                    {
                        m_bVacSkip1 = false;
                        m_bVacSkip2 = false;
                        m_bVacSkip3 = false;
                        m_bVacSkip4 = false;
                        m_bVacSkip5 = false;
                        m_bVacSkip6 = false;
                    }
                    
                    DM.ARAY[ri.PSTR].SetStat(cs.None   );
                    DM.ARAY[ri.ULDR].SetStat(cs.WorkEnd);
                    OM.EqpStat.iDayWorkCnt += 1; //일단 트레이 갯수로 카운팅하고 추후에 자재 갯수로 바꿔야되면 1트레이당 자재 갯수 옵션 처리해서 곱하기 한다.
                    OM.EqpStat.dEndTime = DateTime.Now.ToOADate();
                    OM.EqpStat.dCycleTime = OM.EqpStat.dEndTime - OM.EqpStat.dPreSttTime;

                    Step.iCycle = 0;
                    return true;
            }
        }
        
        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.PSTR_TrayAlignFwBw)
            {

            }
            else if (_eActr == ci.PSTR_SttnStprUpDn)
            {

            }
            else if (_eActr == ci.PSTR_SttnClampClOp)
            {
                //if(!MT_GetStop(mi.PSTB_XMark) || !MT_GetStop(mi.PSTB_YMark)) {
                //    sMsg = "Marking Zone Motor is Moving";
                //    return false;
                //}
            }
            else if (_eActr == ci.PSTR_SttnUpDn)
            {

            }
            
            else if (!_bChecked)
            {
                sMsg = "Cylinder " + CL_GetName(_eActr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Trace(CL_GetName(_eActr) + " " + sMsg);
                if (Step.iCycle==0) Log.ShowMessage(CL_GetName(_eActr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool CheckSafe(mi _eMotr, pv _ePstn, double _dOfsPos = 0)
        {
            if (OM.MstOptn.bDebugMode) return true;
            double dDstPos = PM_GetValue(_eMotr, _ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";

            //bool bPenBwd = CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd);

            //if (!bPenBwd)
            //{
            //    sMsg = "Need to Pen Up Position";
            //    bRet = false;
            //}

            if (_eMotr == mi.PSTR_X)
            {
            
            }
            else
            {
                sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Trace(MT_GetName(_eMotr) + " " + sMsg);
                //메뉴얼 동작일때.
                if (Step.eSeq == sc.Idle) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool JogCheckSafe(mi _eMotr, EN_JOG_DIRECTION _eDir, EN_UNIT_TYPE _eType, double _dDist)
        {
            if (OM.MstOptn.bDebugMode) return true;
            bool bRet = true;
            string sMsg = "";

            if (_eMotr == mi.PSTR_X)
            {
                if (_eDir == EN_JOG_DIRECTION.Neg) //아래
                {
            
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
            
                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
                    {
            
                    }
                }
                else //위
                {
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
            
                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
                    {
            
                    }
                }
            }
            else
            {
                sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Trace(MT_GetName(_eMotr) + " " + sMsg);
                //메뉴얼 동작일때.
                if (Step.eSeq == sc.Idle) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool MoveMotrSlow(mi _eMotr , pv _ePstn ,  double _dOfsPos=0)
        {
            if (!CheckSafe(_eMotr, _ePstn , _dOfsPos)) return false;

            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            MT_GoAbsSlow(_eMotr, dDstPos); 
            return true ;

        }

        //무브함수들의 리턴이 Done을 의미 한게 아니고 명령 전달이 됐는지 여부로 바꿈.
        //Done 확인을 위해서는 GetStop을 써야함.
        public bool MoveMotr(mi _eMotr , pv _ePstn ,  double _dOfsPos=0)
        {
            if (!CheckSafe(_eMotr, _ePstn , _dOfsPos)) return false;

            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            int    iSpdPer = PM_GetValueSpdPer(_eMotr, _ePstn);

            if (Step.iCycle!=0) MT_GoAbsRun(_eMotr , dDstPos, iSpdPer);
            else                MT_GoAbsMan(_eMotr , dDstPos);

            return true ;        
        }

        //무브함수들의 리턴이 Done을 의미 한게 아니고 명령 전달이 됐는지 여부로 바꿈.
        //Done 확인을 위해서는 GetStop을 써야함.
        public bool MoveCyl(ci _eActr, fb _eFwd)
        {
            if (!CheckSafe(_eActr, _eFwd)) return false;

            CL_Move(_eActr, _eFwd);
            
            return true;
        }

        public bool CheckStop()
        {
            if (!MT_GetStop(mi.PSTR_X)) return false;

            if (!CL_Complete(ci.PSTR_TrayAlignFwBw )) return false;
            if (!CL_Complete(ci.PSTR_SttnStprUpDn)) return false;
            if (!CL_Complete(ci.PSTR_SttnClampClOp  )) return false;
            if (!CL_Complete(ci.PSTR_SttnUpDn       )) return false;

            return true ;
        }
        //public void SaveLotName(string _sLotName)
        //{
        //    string sLotDataPath = "C:\\Data\\LotName.ini";
        //    CIniFile IniLotDatadSave = new CIniFile(sLotDataPath);

        //    IniLotDatadSave.Save("LotName", "LotName", _sLotName);

        //}

        public void Trace(string _name, params bool[] _val)
        {
            string sLog = _name + " = ";
            for(int i=0; i<_val.Length; i++) sLog += _val[i].ToString() + ",";
            Trace(sLog);
        }
        public void Trace(string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = m_sPartName.Replace(",", "");
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", m_iPartId);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            Log.SendMessage(sFullMsg);
        }        
        
    };

    
}
