using COMMON;
using System;

namespace Machine
{
    //PCK == Picker
    public class Stacker : Part
    {
               //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop;
            public void Clear()
            {
                bWorkEnd = false;
                bReqStop = false;
            }
        };   
        public enum sc
        {
            Idle              = 0,
            ToStack              ,
            Stack                , //
//            Barcode              , // 20180209 안씀.
            Out                  , //
            MAX_SEQ_CYCLE
        };

        public struct SStep
        {
            public int  iHome;
            public int  iToStart;
            public sc   eSeq;
            public int  iCycle;
            public int  iToStop;
            public sc   eLastSeq;
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

        VisnCom.TRslt RsltDieAlign     ;
        VisnCom.TRslt RsltSubsAlign    ;
        VisnCom.TRslt RsltBtmAlign     ;
        VisnCom.TRslt RsltRightDist    ;
        VisnCom.TRslt RsltLeftDist     ;

        double m_dDiePosOfsX ;
        double m_dDiePosOfsY ;

        const int iVisnDelay  = 100 ;
        const double dDispCheckOfs = -5.0;

        public Stacker()
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
            m_tmMain   .Clear();
            m_tmCycle  .Clear();
            m_tmHome   .Clear();
            m_tmToStop .Clear();
            m_tmToStart.Clear();
            m_tmDelay  .Clear();

            ResetTimer();

            Stat.Clear();
            Step.Clear();
            PreStep.Clear();
        }

        //Running Functions.
        override public void Update()
        {

            int a = 0; 
            a++;

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
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: Step.iToStart = 0;
                    return true;

                case 10:
                    //CL_Move(ci.STCK_StackOpCl, fb.Fwd);
                    Step.iToStart++;
                    return false;
                
                case 11: 
                    //if (!CL_Complete(ci.STCK_StackOpCl)) return false;

                    Step.iToStart = 0;
                    return true ;
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
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iToStop = Step.iToStop;
            Stat.bReqStop = false;            

            //Move Home.
            switch (Step.iToStop)
            {
                default: Step.iToStop = 0;
                    return true;

                case 10:
                    
                    Step.iToStop++;
                    return false;
                
                case 11: 
                   
                    Step.iToStop++;
                    return false ;

                case 12:
                  
                    Step.iToStop++;
                    return false ;

                case 13:
                   
                    Step.iToStop++;
                    return false;

                case 14:
                   
                    Step.iToStop++;
                    return false;

                case 15:

                    Step.iToStop = 0;
                    return true ;

            }


        }

        override public int GetHomeStep   () { return Step.iHome    ; } override public int GetPreHomeStep   () { return PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return Step.iToStart ; } override public int GetPreToStartStep() { return PreStep.iToStart ; }
        override public int GetSeqStep    () { return (int)Step.eSeq; } override public int GetPreSeqStep    () { return (int)PreStep.eSeq; }
        override public int GetCycleStep  () { return Step.iCycle   ; } override public int GetPreCycleStep  () { return PreStep.iCycle   ; } override public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; }
        override public int GetToStopStep () { return Step.iToStop  ; } override public int GetPreToStopStep () { return PreStep.iToStop  ; }

        override public string GetCrntCycleName(         ) { return Step.eSeq.ToString();}
        override public String GetCycleName    (int _iSeq) { return ((sc)_iSeq).ToString(); }
        override public double GetCycleTime    (int _iSeq) { return m_CycleTime[_iSeq].Duration; }
        override public String GetPartName     (         ) { return m_sPartName; }

        override public int GetCycleMaxCnt() { return (int)sc.MAX_SEQ_CYCLE; }

        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            //Check Cycle Time Out.
            String sTemp;
            sTemp = String.Format("%s Step.iCycle={0:00}", "Autorun", Step.iCycle);
            if (Step.eSeq != PreStep.eSeq) {
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                if(SEQ.IDXF.GetSeqStep() != (int)IndexFront.sc.Out && SEQ.IDXR.GetSeqStep() != (int)IndexRear.sc.Out){
                    if( DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) && IO_GetX(xi.RAIL_TrayDtct4)) {
                        ER_SetErr(ei.PKG_Unknwn , "Rail Outzone Unknwn Tray Error-Check "+IO_GetXName(xi.RAIL_TrayDtct4));
                        return false;
                    }
                    if(!DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) &&!IO_GetX(xi.RAIL_TrayDtct4)) {
                        ER_SetErr(ei.PKG_Dispr  , "Rail Outzone Disappear Tray Error-Check "+IO_GetXName(xi.RAIL_TrayDtct4)); 
                        return false;
                    }
                }

                //Inspection, Pick, Place
                bool isCycleToStack        = DM.ARAY[ri.OUTZ].CheckAllStat(cs.Good) && DM.ARAY[ri.STCK].GetCntStat(cs.Empty)>0 && DM.ARAY[ri.PSTC].CheckAllStat(cs.None);
                bool isCycleStack          = DM.ARAY[ri.PSTC].CheckAllStat(cs.Good) ;
                bool isCycleOut            = DM.ARAY[ri.STCK].CheckAllStat(cs.Good) ;
                bool isCycleEnd            = DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) && (DM.ARAY[ri.STCK].CheckAllStat(cs.None) || DM.ARAY[ri.STCK].CheckAllStat(cs.Empty));
                                   
                if (ER_IsErr()) return false;

                //Normal Decide Step.
                     if (isCycleToStack) { Step.eSeq  = sc.ToStack ; }
                else if (isCycleStack  ) { Step.eSeq  = sc.Stack   ; }
                else if (isCycleOut    ) { Step.eSeq  = sc.Out     ; }
                else if (isCycleEnd    ) { Stat.bWorkEnd = true; return true; }

                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle){
                    Log.Trace(m_sPartName, Step.eSeq.ToString() +" Start");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }

            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default         :                        Log.Trace(m_sPartName, "default End");                                    Step.eSeq = sc.Idle;  return false;
                case sc.Idle    :                                                                                                                        return false;
                case sc.ToStack : if (CycleToStack()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Stack   : if (CycleStack  ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Out     : if (CycleOut    ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                
            }                               
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            r = 0 ;
            c = 0 ;
            DM.ARAY[_iId].FindLastRowCol(_iChip, ref c , ref r);             
            return (c >= 0 && r >= 0) ? true : false;
        }       

        public bool CycleHome()
        {
            string sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                Log.Trace(m_sPartName, sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if(Stat.bReqStop) {
                //Step.iHome = 0;
                //return true ;
            }
            
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    if(Step.iHome != PreStep.iHome)Log.Trace(m_sPartName, sTemp);
                    return true ;
            
                case 10:
                    IO_SetY  (yi.STCK_StackAC      , false );
                    IO_SetY  (yi.BARZ_BrcdAC       , false );
                    CL_Move  (ci.STCK_RailTrayUpDn , fb.Bwd);
                    CL_Move  (ci.BARZ_BrcdTrayUpDn , fb.Bwd);
                    CL_Move  (ci.STCK_RailClOp     , fb.Fwd);
                    CL_Move  (ci.STCK_StackStprUpDn, fb.Fwd);
                    CL_Move  (ci.BARZ_BrcdStprUpDn , fb.Fwd);
                    MT_GoHome(mi.STCK_ZStck                );

                    Step.iHome++;
                    return false ;

                case 11:
                    if (!CL_Complete  (ci.STCK_RailTrayUpDn , fb.Bwd)) return false;
                    if (!CL_Complete  (ci.STCK_RailTrayUpDn , fb.Bwd)) return false;
                    if (!CL_Complete  (ci.STCK_RailClOp     , fb.Fwd)) return false;
                    if (!CL_Complete  (ci.STCK_StackStprUpDn, fb.Fwd)) return false;
                    if (!CL_Complete  (ci.BARZ_BrcdStprUpDn , fb.Fwd)) return false;
                    if(!MT_GetHomeDone(mi.STCK_ZStck                )) return false;

                    if (!IO_GetX(xi.STCK_StackTrayDtct))
                    {
                        CL_Move(ci.STCK_StackOpCl, fb.Bwd);
                    }
                    
                    Step.iHome++;
                    return false ;

                case 12:
                    if (!CL_Complete(ci.STCK_StackOpCl)) return false;
                    MT_GoAbsRun(mi.STCK_ZStck, PM.GetValue(mi.STCK_ZStck, pv.STCK_ZStckWait));
                    Step.iHome++;
                    return false;

                case 13:
                    if(!MT_GetStopInpos(mi.STCK_ZStck))return false;
                    Step.iHome = 0;
                    return true ;
            }
        }

        string GetS(double _dVal)
        {
            return string.Format("{0:0.000}", _dVal);
        }

        public bool CycleToStack()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int c, r;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveCyl (ci.STCK_StackStprUpDn, fb.Fwd   );
                    MoveCyl (ci.STCK_RailTrayUpDn , fb.Bwd   );
                    MoveCyl (ci.STCK_StackOpCl    , fb.Bwd   );
                    MoveMotr(mi.STCK_ZStck, pv.STCK_ZStckWait);

                    Step.iCycle++;
                    return false ;

                case 11:
                    if (!CL_Complete  (ci.STCK_StackStprUpDn, fb.Fwd   )) return false;
                    if (!CL_Complete  (ci.STCK_RailTrayUpDn , fb.Bwd   )) return false;
                    if (!CL_Complete  (ci.STCK_StackOpCl    , fb.Bwd   )) return false;
                    if (!MT_GetStopPos(mi.STCK_ZStck, pv.STCK_ZStckWait)) return false;
                    //if (!IO_GetX(xi.RAIL_TrayDtct4))
                    //{
                    //    ER_SetErr(ei.PKG_Dispr, "Tray Out Sensor Not Detected");
                    //    return true;
                    //}
                    MoveCyl(ci.STCK_RailTrayUpDn, fb.Fwd);
                    
                    Step.iCycle++;
                    return false ;

                case 12:
                    if (!CL_Complete(ci.STCK_RailTrayUpDn, fb.Fwd)) return false;
                    MoveCyl(ci.STCK_RailClOp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!CL_Complete(ci.STCK_RailClOp, fb.Bwd)) return false;
                    MoveCyl(ci.STCK_RailTrayUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if (!CL_Complete(ci.STCK_RailTrayUpDn, fb.Bwd)) return false;
                    DM.ARAY[ri.OUTZ].SetStat(cs.None);
                    DM.ARAY[ri.PSTC].SetStat(cs.Good);
                    MoveCyl(ci.STCK_RailClOp, fb.Fwd);
                    IO_SetY(yi.STCK_StackAC, true);
                    IO_SetY(yi.BARZ_BrcdAC , true);
                    
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!OM.CmnOptn.bIdleRun && !IO_GetX(xi.STCK_StackTrayDtct)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!m_tmDelay.OnDelay(OM.CmnOptn.bIdleRun?3000:1000)) return false;
                    IO_SetY(yi.STCK_StackAC, false);
                    IO_SetY(yi.BARZ_BrcdAC , false);
                    Step.iCycle = 0;
                    return true ;                    
            }
        }

        public bool CycleStack()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int c, r;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.STCK_ZStck, pv.STCK_ZStckWork);
                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if (!MT_GetStopPos(mi.STCK_ZStck, pv.STCK_ZStckWork)) return false;
                    DM.ARAY[ri.PSTC].SetStat(cs.None);
                    FindChip(ri.STCK, out c, out r, cs.Empty);
                    DM.ARAY[ri.STCK].SetStat(c, r, cs.Good);

                    MoveMotr(mi.STCK_ZStck, pv.STCK_ZStckWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.STCK_ZStck, pv.STCK_ZStckWait)) return false;
                    
                    Step.iCycle = 0;
                    return true ;
            }
        }


        //public bool bBarDetected = false;
        public bool CycleOut()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }
            int c,r;

            //if (!bBarDetected)
            //{
            //    if(IO_GetX(xi.BARZ_BrcdTrayDtct) || IO_GetXUp(xi.BARZ_BrcdTrayDtct) || IO_GetXDn(xi.BARZ_BrcdTrayDtct))
            //        bBarDetected = true;
            //}

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.STCK_ZStck, pv.STCK_ZStckWait);
                    MoveCyl(ci.STCK_StackOpCl    , fb.Bwd);
                    MoveCyl(ci.BARZ_BrcdStprUpDn , fb.Fwd);
                    MoveCyl(ci.STCK_StackStprUpDn, fb.Bwd);
                    MoveCyl(ci.BARZ_BrcdTrayUpDn , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.STCK_ZStck, pv.STCK_ZStckWait)) return false;
                    if (!CL_Complete  (ci.STCK_StackOpCl    , fb.Bwd)) return false;
                    if (!CL_Complete  (ci.BARZ_BrcdStprUpDn , fb.Fwd)) return false;
                    if (!CL_Complete  (ci.STCK_StackStprUpDn, fb.Bwd)) return false;
                    if (!CL_Complete  (ci.BARZ_BrcdTrayUpDn , fb.Bwd)) return false;

                    MoveMotr(mi.STCK_ZStck, pv.STCK_ZStckWork);

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.STCK_ZStck, pv.STCK_ZStckWork)) return false;
                    MoveCyl(ci.STCK_StackOpCl, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.STCK_StackOpCl, fb.Fwd))return false;
                    MoveMotr(mi.STCK_ZStck, pv.STCK_ZStckWait);
                    
                    
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.STCK_ZStck, pv.STCK_ZStckWait)) return false;
                    IO_SetY(yi.STCK_StackAC, true);
                    IO_SetY(yi.BARZ_BrcdAC , true);
                    DM.ARAY[ri.STCK].SetStat(cs.Empty);
                    DM.ARAY[ri.BARZ].SetStat(cs.Unknown);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if (m_tmDelay.OnDelay(true, 5000))
                    {
                        IO_SetY(yi.STCK_StackAC, false);
                        IO_SetY(yi.BARZ_BrcdAC , false);
                        ER_SetErr(ei.PRT_Detect, "Stacker Zone Tray Not Detect");
                        return true;
                    }
                    if(!OM.CmnOptn.bIdleRun && !IO_GetX(xi.BARZ_BrcdTrayDtct))return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!m_tmDelay.OnDelay(OM.CmnOptn.bIdleRun ? 3000 : 1000))return false;
                    MoveCyl(ci.STCK_StackOpCl, fb.Bwd);
                    if (OM.DevOptn.bUseBarcCyl)
                    {
                        MoveCyl(ci.BARZ_BrcdTrayUpDn, fb.Fwd);
                    }
                    Step.iCycle++;
                    return false;

                case 17:
                    if (OM.DevOptn.bUseBarcCyl && !CL_Complete(ci.BARZ_BrcdTrayUpDn, fb.Fwd)) return false;
                    
                    
                    IO_SetY(yi.STCK_StackAC, false);
                    IO_SetY(yi.BARZ_BrcdAC , false);
                    
                    Step.iCycle = 0;
                    return true;
            }
        }
        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.STCK_RailClOp){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            
            else if(_eActr == ci.STCK_RailTrayUpDn){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            
            else if(_eActr == ci.STCK_StackStprUpDn){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.STCK_StackOpCl)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.BARZ_BrcdStprUpDn)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.BARZ_BrcdTrayUpDn)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else {
                sMsg = "Cylinder " + CL_GetName(_eActr) + " is Not this parts.";
                bRet = false;
            }
            
            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(CL_GetName(_eActr), sMsg);
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

            //TOOL_ZVisn


            if (_eMotr == mi.STCK_ZStck)
            {
                //if (!MT_GetStopInpos(mi.TOOL_YGent))
                //{
                //    sMsg = MT_GetName(mi.TOOL_YGent) + " is moving.";
                //    bRet = false;
                //}
                //
                //if (!MT_GetStopInpos(mi.TOOL_XLeft))
                //{
                //    sMsg = MT_GetName(mi.TOOL_XLeft) + " is moving.";
                //    bRet = false;
                //}
            
            }
            
            else
            {
                sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
                bRet = false;
            }
            
            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(MT_GetName(_eMotr), sMsg);
                //메뉴얼 동작일때.
                if (Step.eSeq == 0) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
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
            if(Step.iCycle!=0) MT_GoAbsRun(_eMotr , dDstPos);
            else               MT_GoAbsMan(_eMotr , dDstPos);
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
            if (!MT_GetStop(mi.STCK_ZStck)) return false;
            
            if (!CL_Complete(ci.STCK_RailClOp     )) return false;
            if (!CL_Complete(ci.STCK_RailTrayUpDn )) return false;
            if (!CL_Complete(ci.STCK_StackOpCl    )) return false;
            if (!CL_Complete(ci.STCK_StackStprUpDn)) return false;

            return true;
        }
    };
    


    

   
    
}
