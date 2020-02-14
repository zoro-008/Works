using System;
using COMMON;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using SML;

namespace Machine
{
    public class PreRail : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
            public bool bReqStop   ;
            public void Clear()
            {
                bWorkEnd    = false ;
                bReqStop    = false ;
                bSupply     = false ;
            }
            public bool bSupply    ;
        };   

        public enum sc
        {
            Idle    = 0,
            Reload     ,
            Supply     ,
            LiftUp     ,
            LiftDown   ,
            //Clean      ,
                
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

        public PreRail(int _iPartId = 0)
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
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 5000))
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
                    return true;

                case 10:
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
                    return true;

                case 10:
                    MT_Stop(mi.PRER_X);
                    MoveCyl(ci.PRER_RollerUpDn, fb.Fwd);

                    Step.iToStop++;
                    return false;

                case 11:
                    if(!MT_GetStop(mi.PRER_X)) return false;
                    if(!CL_Complete(ci.PRER_RollerUpDn, fb.Fwd)) return false;

                    Step.iToStop = 0;
                    return true;
            }
        }

        override public int GetHomeStep   () { return      Step.iHome    ; } override public int GetPreHomeStep   () { return      PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return      Step.iToStart ; } override public int GetPreToStartStep() { return      PreStep.iToStart ; }
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
                bool bLODRNone = DM.ARAY[ri.LODR].CheckAllStat(cs.None    );
                bool bPRERNone = DM.ARAY[ri.PRER].CheckAllStat(cs.None    );
                bool bSupply   = DM.ARAY[ri.LODR].CheckAllStat(cs.Unknown );
                bool bLiftUp   = DM.ARAY[ri.PRER].CheckAllStat(cs.LiftUp  );
                bool bLiftDown = DM.ARAY[ri.PRER].CheckAllStat(cs.Empty   );
                //bool bClean    = DM.ARAY[ri.PRER].CheckAllStat(cs.Cleaning) && DM.ARAY[ri.PSTR].CheckAllStat(cs.None);
                bool bRailSsr1 = IO_GetX(xi.PRER_TrayDetect1);
                //bool bRailSsr2 = IO_GetX(xi.PRER_TrayDetect2);
                
                //사이클
                bool isReload   =  bLODRNone &&  IO_GetXDryRun(xi.PRER_TrayDetect1, OM.MstOptn.bIdleRun);
                bool isSupply   =  bSupply   && bPRERNone;
                bool isLiftUp   =  bLiftUp  ;
                bool isLiftDown =  bLiftDown; 
                //bool isClean    =  bClean   ;
                bool isEnd      =  bLODRNone && bPRERNone; 

                //모르는 자재 에러
                //if(bLODRNone && bRailSsr1) ER_SetErr(ei.PKG_Unknwn,"Loading have no data found, but rail sensor detected") ;
                //if(bPRERNone && bRailSsr2) ER_SetErr(ei.PKG_Unknwn,"Pre Rail have no data found, but rail sensor detected") ;
                
                //자재 사라짐
                //if(!bLODRNone && !bRailSsr1) ER_SetErr(ei.PKG_Dispr  ,"Loading have data, but rail sensor not detected") ;
                //if(!bPRERNone && !bRailSsr2) ER_SetErr(ei.PKG_Dispr  ,"Pre Rail have data, but rail sensor not detected") ;

                if (ER_IsErr()) return false;

                //Normal Decide Step.
                     if (isReload  ) { DM.ARAY[ri.LODR].Trace(m_iPartId);                                    Step.eSeq = sc.Reload  ; }
                else if (isSupply  ) { DM.ARAY[ri.LODR].Trace(m_iPartId); DM.ARAY[ri.PRER].Trace(m_iPartId); Step.eSeq = sc.Supply  ; }
                else if (isLiftUp  ) { DM.ARAY[ri.PRER].Trace(m_iPartId); DM.ARAY[ri.LPCK].Trace(m_iPartId); Step.eSeq = sc.LiftUp  ; }
                else if (isLiftDown) { DM.ARAY[ri.PRER].Trace(m_iPartId);                                    Step.eSeq = sc.LiftDown; }
                //else if (isClean   ) { DM.ARAY[ri.PRER].Trace(m_iPartId); DM.ARAY[ri.PSTR].Trace(m_iPartId); Step.eSeq = sc.Clean   ; }
                else if (isEnd     ) { Stat.bWorkEnd = true; return true; }
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
                default          : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case sc.Idle     : return false;
                case sc.Reload   : if (!CycleReload  ()) return false; break;
                case sc.Supply   : if (!CycleSupply  ()) return false; break;
                case sc.LiftUp   : if (!CycleLiftUp  ()) return false; break;
                case sc.LiftDown : if (!CycleLiftDown()) return false; break;
                //case sc.Clean    : if (!CycleClean   ()) return false; break;
            }
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        //public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        //{
        //    c=0 ; r=0 ;
        //    
        //    return DM.ARAY[(int)_iId].FindFrstColLastRow( ref c, ref r , _iChip);
        //}    

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_HomeTO ,sTemp);
                Trace(sTemp);
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
                    IO_SetY(yi.PSTR_IonBlwrTop, false);
                    IO_SetY(yi.PSTR_IonBlwrBtm, false);
                    IO_SetY(yi.PSTR_AirBlwrTop1, false);
                    IO_SetY(yi.PSTR_AirBlwrBtm1, false);
                    IO_SetY(yi.PSTR_AirBlwrTop2, false);
                    IO_SetY(yi.PSTR_AirBlwrBtm2, false);
                    MT_Stop(mi.PRER_X);
                    CL_Move(ci.PRER_RollerUpDn   , fb.Fwd);
                    CL_Move(ci.PRER_PreStprUpDn  , fb.Fwd);
                    CL_Move(ci.PRER_SttnStprUpDn , fb.Fwd);
                    CL_Move(ci.PRER_TrayAlignFwBw, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 11:
                    if(!MT_GetStop(mi.PRER_X)) return false;
                    if(!CL_Complete(ci.PRER_RollerUpDn   , fb.Fwd)) return false;
                    if(!CL_Complete(ci.PRER_PreStprUpDn  , fb.Fwd)) return false;
                    if(!CL_Complete(ci.PRER_SttnStprUpDn , fb.Fwd)) return false;
                    if(!CL_Complete(ci.PRER_TrayAlignFwBw, fb.Bwd)) return false;
                    
                    CL_Move(ci.PRER_SttnClampClOp , fb.Bwd);

                    Step.iHome++;
                    return false ;
            
                case 12:
                    if(!CL_Complete(ci.PRER_SttnClampClOp , fb.Bwd)) return false;
                    CL_Move(ci.PRER_SttnUpDn, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 13:
                    if (!CL_Complete(ci.PRER_SttnUpDn, fb.Bwd)) return false;
                    MT_SetPos(mi.PRER_X, 0.0);
                    MT_SetHomeDone(mi.PRER_X, true);

                    Step.iHome = 0;
                    return true ;
            }
        }

        public bool CycleReload()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                MT_Stop(mi.PRER_X);
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
                    
                    DM.ARAY[ri.LODR].SetStat(cs.Unknown);

                    Step.iCycle = 0;
                    return true;
            }
        }
        
        public bool CycleSupply()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                MT_Stop(mi.PRER_X);
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

            double dRailNorSpeed = SM.MTR.Para[(int)mi.PRER_X].dRunSpeed;
            bool bIdleRun = OM.MstOptn.bIdleRun;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    if (!IO_GetXDryRun(xi.PRER_TrayDetect1, bIdleRun))
                    {
                        ER_SetErr(ei.PKG_Dispr, "Loading have data, but rail sensor not detected");
                        return true;
                    }
                    OM.EqpStat.dSttTime = DateTime.Now.ToOADate();
                    MoveCyl(ci.PRER_SttnClampClOp, fb.Bwd);
                    MoveCyl(ci.PRER_SttnUpDn     , fb.Bwd);
                    MoveCyl(ci.PRER_PreStprUpDn  , fb.Fwd);
                    MoveCyl(ci.PRER_SttnStprUpDn , fb.Fwd);
                    MoveCyl(ci.PRER_TrayAlignFwBw, fb.Bwd);
                    MoveCyl(ci.PRER_RollerUpDn   , fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.PRER_SttnClampClOp, fb.Bwd)) return false;
                    if(!CL_Complete(ci.PRER_SttnUpDn     , fb.Bwd)) return false;
                    if(!CL_Complete(ci.PRER_PreStprUpDn  , fb.Fwd)) return false;
                    if(!CL_Complete(ci.PRER_SttnStprUpDn , fb.Fwd)) return false;
                    if(!CL_Complete(ci.PRER_TrayAlignFwBw, fb.Bwd)) return false;
                    if(!CL_Complete(ci.PRER_RollerUpDn   , fb.Fwd)) return false;

                    Step.iCycle++;
                    return false;

                case 12:
                    MT_JogVel(mi.PRER_X, dRailNorSpeed);
                    MoveCyl(ci.PRER_PreStprUpDn  , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.PRER_PreStprUpDn  , fb.Bwd)) return false;
                    if(!IO_GetXDryRun(xi.PRER_TrayDetect2, bIdleRun)) return false;
                    MT_Stop(mi.PRER_X);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStop(mi.PRER_X)) return false;
                    MoveCyl(ci.PRER_PreStprUpDn  , fb.Fwd);
                    
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!CL_Complete(ci.PRER_PreStprUpDn  , fb.Fwd)) return false;
                    DM.ARAY[ri.LODR].SetStat(cs.None  );
                    DM.ARAY[ri.PRER].SetStat(cs.LiftUp);

                    Step.iCycle = 0;
                    return true;

               
            }
        }

        public bool CycleLiftUp()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 7000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                MT_Stop(mi.PRER_X);
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

            bool r, c ;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    if (OM.MstOptn.bIdleRun && !DM.ARAY[ri.LPCK].CheckAllStat(cs.None)) return false;
                    MoveCyl(ci.PRER_TrayAlignFwBw, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.PRER_TrayAlignFwBw, fb.Fwd)) return false;
                    

                    Step.iCycle++;
                    return false;

                case 12:
                    
                    MoveCyl(ci.PRER_SttnUpDn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.PRER_SttnUpDn, fb.Fwd)) return false;
                    MoveCyl(ci.PRER_SttnClampClOp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.PRER_SttnClampClOp, fb.Fwd)) return false;
                    MoveCyl(ci.PRER_TrayAlignFwBw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.PRER_TrayAlignFwBw, fb.Bwd)) return false;
                    DM.ARAY[ri.LPCK].SetStat(cs.Pick);
                    DM.ARAY[ri.PRER].SetStat(cs.Exist);

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
                MT_Stop(mi.PRER_X);
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

            bool r, c ;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.PRER_TrayAlignFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.PRER_TrayAlignFwBw, fb.Bwd)) return false;
                    MoveCyl(ci.PRER_SttnClampClOp, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PRER_SttnClampClOp, fb.Bwd)) return false;
                    MoveCyl(ci.PRER_SttnUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.PRER_SttnUpDn, fb.Bwd)) return false;
                    DM.ARAY[ri.PRER].SetStat(cs.Cleaning);
                    
                    Step.iCycle = 0;
                    return true;

            }
        }

        //public double dPos = 0;
        //public bool CycleClean()
        //{
        //    String sTemp;
        //    if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
        //    {
        //        sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
        //        sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
        //        MT_Stop(mi.PRER_X);
        //        IO_SetY(yi.PRER_IonBlowerTop, false);
        //        IO_SetY(yi.PRER_IonBlowerBtm, false);
        //        IO_SetY(yi.PRER_AirBlowerTop, false);
        //        IO_SetY(yi.PRER_AirBlowerBtm, false);
        //        ER_SetErr(ei.ETC_CycleTO, sTemp);
        //        Trace(sTemp);
        //        return true;
        //    }
        //
        //    if (Step.iCycle != PreStep.iCycle)
        //    {
        //        sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
        //        Trace(sTemp);
        //    }
        //
        //    PreStep.iCycle = Step.iCycle;
        //
        //    if (Stat.bReqStop)
        //    {
        //        //return true ;
        //    }
        //
        //    int r,c = 0;
        //    //double dPos = 0;
        //
        //    switch (Step.iCycle)
        //    {
        //
        //        default: 
        //            sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
        //            return true;
        //
        //        case 10:
        //            MoveCyl(ci.PRER_RollerUpDn, fb.Bwd);
        //
        //            Step.iCycle++;
        //            return false;
        //
        //        case 11:
        //            if(!CL_Complete(ci.PRER_RollerUpDn, fb.Bwd)) return false;
        //
        //            MT_JogVel(mi.PRER_X, OM.DevInfo.dRailClnSpeed);
        //            MT_JogVel(mi.PSTR_X, OM.DevInfo.dRailClnSpeed);
        //            Step.iCycle++;
        //            return false;
        //
        //        case 12:
        //            if (!OM.CmnOptn.bPRER_IonBtmOff) IO_SetY(yi.PRER_IonBlowerBtm, true);
        //            if (!OM.CmnOptn.bPRER_AirBtmOff) IO_SetY(yi.PRER_AirBlowerBtm, true);
        //            
        //            if (!OM.CmnOptn.bPRER_IonTopOff) IO_SetY(yi.PRER_IonBlowerTop, true);
        //            if (!OM.CmnOptn.bPRER_AirTopOff) IO_SetY(yi.PRER_AirBlowerTop, true);
        //            
        //
        //            Step.iCycle++;
        //            return false;
        //
        //        case 13:
        //            if (!OM.CmnOptn.bPRER_IonBtmOff && !IO_GetY(yi.PRER_IonBlowerBtm)) return false;
        //            if (!OM.CmnOptn.bPRER_AirBtmOff && !IO_GetY(yi.PRER_AirBlowerBtm)) return false;
        //            
        //            if (!OM.CmnOptn.bPRER_IonTopOff && !IO_GetY(yi.PRER_IonBlowerTop)) return false;
        //            if (!OM.CmnOptn.bPRER_AirTopOff && !IO_GetY(yi.PRER_AirBlowerTop)) return false;
        //            
        //
        //            SEQ.PSTR.MoveCyl(ci.PSTR_SttnStprUpDn, fb.Fwd);
        //            MoveCyl(ci.PRER_SttnStprUpDn, fb.Bwd);
        //
        //            Step.iCycle++;
        //            return false;
        //
        //        case 14:
        //            if(!CL_Complete(ci.PSTR_SttnStprUpDn, fb.Fwd)) return false;
        //            if(!CL_Complete(ci.PRER_SttnStprUpDn, fb.Bwd)) return false;
        //
        //            if(!IO_GetX(xi.PRER_TrayDetect2)) DM.ARAY[ri.PRER].SetStat(cs.None);
        //
        //            if(!IO_GetX(xi.PSTR_TrayDetect2)) return false;
        //
        //            MT_Stop(mi.PRER_X);
        //            MT_Stop(mi.PSTR_X);
        //            
        //            Step.iCycle++;
        //            return false;
        //
        //        case 15:
        //            if(!MT_GetStop(mi.PRER_X)) return false;
        //            if(!MT_GetStop(mi.PSTR_X)) return false;
        //            IO_SetY(yi.PRER_IonBlowerTop, false);
        //            IO_SetY(yi.PRER_IonBlowerBtm, false);
        //            IO_SetY(yi.PRER_AirBlowerTop, false);
        //            IO_SetY(yi.PRER_AirBlowerBtm, false);
        //
        //            //DM.ARAY[ri.PRER].SetStat(cs.None  );
        //            DM.ARAY[ri.PSTR].SetStat(cs.LiftUp);
        //
        //            Step.iCycle=0;
        //            return true;
        //    }
        //}

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            //bool bMov = !MT_GetStop(mi.LODR_ZClmp) || !MT_GetStop(mi.LODR_YClmp) ;

            if(_eActr == ci.PRER_RollerUpDn)
            {
                //if(bMov) {sMsg = "Loader motor is moving"; bRet = false;}
            }
            else if (_eActr == ci.PRER_TrayAlignFwBw)
            {
                //if (!SEQ._bRun && Step.iCycle == 0)
                //{
                //    if(_eFwd == fb.Bwd && (IO_GetX(xi.LODR_MgzDetect1) || IO_GetX(xi.LODR_MgzDetect2)) )
                //    {
                //        if (Log.ShowMessageModal("Confirm", "Mgz sensor is detected , Open the Mgz?") != System.Windows.Forms.DialogResult.Yes) return false;
                //    }
                //}
            }
            else if (_eActr == ci.PRER_PreStprUpDn)
            {
                //if (!SEQ._bRun && Step.iCycle == 0)
                //{
                //    if(_eFwd == fb.Bwd && (IO_GetX(xi.LODR_MgzDetect1) || IO_GetX(xi.LODR_MgzDetect2)) )
                //    {
                //        if (Log.ShowMessageModal("Confirm", "Mgz sensor is detected , Open the Mgz?") != System.Windows.Forms.DialogResult.Yes) return false;
                //    }
                //}
            }
            else if (_eActr == ci.PRER_SttnStprUpDn)
            {
                //if (!SEQ._bRun && Step.iCycle == 0)
                //{
                //    if(_eFwd == fb.Bwd && (IO_GetX(xi.LODR_MgzDetect1) || IO_GetX(xi.LODR_MgzDetect2)) )
                //    {
                //        if (Log.ShowMessageModal("Confirm", "Mgz sensor is detected , Open the Mgz?") != System.Windows.Forms.DialogResult.Yes) return false;
                //    }
                //}
            }
            else if (_eActr == ci.PRER_SttnClampClOp)
            {
                //if (!SEQ._bRun && Step.iCycle == 0)
                //{
                //    if(_eFwd == fb.Bwd && (IO_GetX(xi.LODR_MgzDetect1) || IO_GetX(xi.LODR_MgzDetect2)) )
                //    {
                //        if (Log.ShowMessageModal("Confirm", "Mgz sensor is detected , Open the Mgz?") != System.Windows.Forms.DialogResult.Yes) return false;
                //    }
                //}
            }
            else if (_eActr == ci.PRER_SttnUpDn)
            {
                //if (!SEQ._bRun && Step.iCycle == 0)
                //{
                //    if(_eFwd == fb.Bwd && (IO_GetX(xi.LODR_MgzDetect1) || IO_GetX(xi.LODR_MgzDetect2)) )
                //    {
                //        if (Log.ShowMessageModal("Confirm", "Mgz sensor is detected , Open the Mgz?") != System.Windows.Forms.DialogResult.Yes) return false;
                //    }
                //}
            }

            else if(!_bChecked){
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

        public bool CheckSafe(mi _eMotr, pv _ePstn ,  double _dOfsPos=0)
        {
            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";

            if (_eMotr == mi.PRER_X)
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

            if (_eMotr == mi.PRER_X)
            {
                if (_eDir == EN_JOG_DIRECTION.Neg)
                {
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
                        
                    }
                    else if(_eType == EN_UNIT_TYPE.utMove)
                    {

                    }
                }
                //Fwd
                else
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
            if (!MT_GetStop(mi.PRER_X)) return false;
            
            if (!CL_Complete(ci.PRER_RollerUpDn    )) return false;
            if (!CL_Complete(ci.PRER_TrayAlignFwBw )) return false;
            if (!CL_Complete(ci.PRER_PreStprUpDn)) return false;
            if (!CL_Complete(ci.PRER_SttnStprUpDn)) return false;
            if (!CL_Complete(ci.PRER_SttnClampClOp  )) return false;
            if (!CL_Complete(ci.PRER_SttnUpDn       )) return false;

            return true;
        }

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
