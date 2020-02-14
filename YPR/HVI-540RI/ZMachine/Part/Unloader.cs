using System;
using COMMON;
using System.Runtime.CompilerServices;

namespace Machine
{
    //ULDR == UnLoader
    public class Unloader : Part
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
            Move                 ,
            Pick                 , //
            Plce                 , //
            Paint                ,

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

        public int iPshrCycle ;

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

        int    iWorkCnt ;

        public Unloader(int _iPartId = 0)
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

            iPshrCycle = 0;
        }

        //Running Functions.
        override public void Update()
        {

            if (DM.ARAY[ri.PSHR].CheckAllStat(cs.Good) && iPshrCycle == 0 && MT_GetStop(mi.ULDR_YIndx)) iPshrCycle = 10;
            if (DM.ARAY[ri.PSHR].CheckAllStat(cs.Good) && iPshrCycle != 0 && MT_GetStop(mi.ULDR_YIndx) && OM.DevInfo.iWorkEndCnt > OM.EqpStat.iULDRCnt) CyclePusher();

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
                Trace(sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: Step.iToStart = 0;
                    return true;

                case 10:
                    MoveCyl(ci.ULDR_OutPshrFwBw, fb.Bwd);
                    Step.iToStart++;
                    return false;
                
                case 11:
                    if (!CL_Complete(ci.ULDR_OutPshrFwBw, fb.Bwd)) return false;

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
                Trace(sTemp);
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

                //if(SEQ.IDXF.GetSeqStep() != (int)IndexFront.sc.Out && SEQ.IDXR.GetSeqStep() != (int)IndexRear.sc.Out){
                //    if( DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) && IO_GetX(xi.RAIL_TrayDtct4)) {ER_SetErr(ei.PKG_Unknwn , "Picker Unknwn PKG Error."   ); return false;}
                //    if(!DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) &&!IO_GetX(xi.RAIL_TrayDtct4)) {ER_SetErr(ei.PKG_Dispr  , "Picker Disappear PKG Error."); return false;}
                //}
                //if( DM.ARAY[ri.TULD].CheckAllStat(cs.Unknown) &&!IO_GetX(xi.TBLE_ULDRPkgDtct)) {ER_SetErr(ei.PKG_Dispr   , "UnLoader Disappear PKG Error."); return false;}
                //if(!DM.ARAY[ri.TULD].CheckAllStat(cs.None   ) && IO_GetX(xi.TBLE_ULDRPkgDtct)) {ER_SetErr(ei.PKG_Unknwn  , "UnLoader Unknown PKG Error.  "); return false;}

                if(DM.ARAY[ri.ULDR].CheckAllStat(cs.WorkEnd) && (!DM.ARAY[ri.TLDR].CheckAllStat(cs.None) ||
                                                                 !DM.ARAY[ri.TVSN].CheckAllStat(cs.None) ||
                                                                 !DM.ARAY[ri.TMRK].CheckAllStat(cs.None) ||
                                                                 !DM.ARAY[ri.TULD].CheckAllStat(cs.None))){
                                                                     ER_SetErr(ei.PKG_Supply, "UnLoader Bar Clear Plz.");
                                                                     return false;
                }
                //Inspection, Pick, Place
                bool isCycleMove           = DM.ARAY[ri.ULDR].GetCntStat(cs.Work) == 0 && DM.ARAY[ri.ULDR].GetCntStat(cs.Empty) !=0 ;
                bool isCyclePick           =!DM.ARAY[ri.PULD].CheckAllStat(cs.None)    && SEQ.PULD.GetSeqStep() != (int)PreUnloader.sc.Work && DM.ARAY[ri.PICK].CheckAllStat(cs.None);
                bool isCyclePlce           = DM.ARAY[ri.ULDR].GetCntStat(cs.Work) != 0 && DM.ARAY[ri.PSHR].CheckAllStat(cs.None) && OM.DevInfo.iWorkEndCnt > OM.EqpStat.iULDRCnt && DM.ARAY[ri.PICK].CheckAllStat(cs.Unknown);
                bool isCyclePaint          = DM.ARAY[ri.ULDR].GetCntStat(cs.Work) != 0 && OM.DevInfo.iWorkEndCnt <= OM.EqpStat.iULDRCnt;
                bool isCycleEnd            = DM.ARAY[ri.PULD].CheckAllStat(cs.None) && DM.ARAY[ri.PSHR].CheckAllStat(cs.None) && DM.ARAY[ri.PICK].CheckAllStat(cs.None) ;//DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) && (DM.ARAY[ri.STCK].CheckAllStat(cs.None) || DM.ARAY[ri.STCK].CheckAllStat(cs.Empty));
                                   
                if (ER_IsErr()) return false;

                //Normal Decide Step.
                     if (isCycleMove   ) { Step.eSeq  = sc.Move   ; }
                else if (isCyclePick   ) { Step.eSeq  = sc.Pick   ; }
                else if (isCyclePlce   ) { Step.eSeq  = sc.Plce   ; }
                else if (isCyclePaint  ) { Step.eSeq  = sc.Paint  ; }
                else if (isCycleEnd    ) { Stat.bWorkEnd = true; return true; }

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
                default         :                     Trace("default End");                                    Step.eSeq = sc.Idle;   return false;
                case sc.Idle    :                                                                                                     return false;
                case sc.Move    : if (CycleMove ()) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Pick    : if (CyclePick ()) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Plce    : if (CyclePlce ()) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Paint   : if (CyclePaint()) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                
            }                               
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            r = 0 ;
            c = 0 ;
            DM.ARAY[_iId].FindFrstColLastRow((cs)_iChip, ref c, ref r);
            return (c >= 0 && r >= 0) ? true : false;
        }       

        public bool CycleHome()
        {
            string sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("Time Out Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Trace(sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Trace(sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if(Stat.bReqStop) {
                //Step.iHome = 0;
                //return true ;
            }
            
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    if(Step.iHome != PreStep.iHome) Trace(sTemp);
                    return true ;
            
                case 10:
                    CL_Move(ci.ULDR_GrpRtrCwCCw  , fb.Bwd);
                    CL_Move(ci.ULDR_OutPshrFwBw  , fb.Bwd);
                    CL_Move(ci.ULDR_ReGrRtrCwCCw , fb.Bwd);
                    CL_Move(ci.ULDR_RngGrpFwBw   , fb.Bwd);
                    CL_Move(ci.ULDR_RngReGrFwBw  , fb.Bwd);
                    Step.iHome++;
                    return false ;

                case 11:
                    if (!CL_Complete  (ci.ULDR_GrpRtrCwCCw , fb.Bwd)) return false;
                    if (!CL_Complete  (ci.ULDR_OutPshrFwBw , fb.Bwd)) return false;
                    if (!CL_Complete  (ci.ULDR_ReGrRtrCwCCw, fb.Bwd)) return false;
                    if (!CL_Complete  (ci.ULDR_RngGrpFwBw  , fb.Bwd)) return false;
                    if (!CL_Complete  (ci.ULDR_RngReGrFwBw , fb.Bwd)) return false;
                    MT_GoHome(mi.ULDR_XGrpr);
                    MT_GoHome(mi.ULDR_ZNzzl);
                    MT_GoHome(mi.ULDR_ZPckr);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetHomeDone(mi.ULDR_XGrpr))return false;
                    if(!MT_GetHomeDone(mi.ULDR_ZNzzl))return false;
                    if(!MT_GetHomeDone(mi.ULDR_ZPckr))return false;
                    MT_GoAbsRun(mi.ULDR_XGrpr , pv.ULDR_XGrprWait);
                    MT_GoAbsRun(mi.ULDR_ZNzzl , pv.ULDR_ZNzzlWait);
                    MT_GoAbsRun(mi.ULDR_ZPckr , pv.ULDR_ZPckrWait);
                    MT_GoHome(mi.ULDR_XNzzl);
                    MT_GoHome(mi.ULDR_YIndx);
                    Step.iHome++;
                    return false;

                case 13:
                    if(!MT_GetHomeDone(mi.ULDR_XNzzl))return false;
                    if(!MT_GetHomeDone(mi.ULDR_YIndx))return false;
                    if(!MT_GetStopInpos(mi.ULDR_XGrpr))return false;
                    if(!MT_GetStopInpos(mi.ULDR_ZNzzl))return false;
                    if(!MT_GetStopInpos(mi.ULDR_ZPckr))return false;
                    MT_GoAbsRun(mi.ULDR_XNzzl , pv.ULDR_XNzzlWait);
                    MT_GoAbsRun(mi.ULDR_YIndx , pv.ULDR_YIndxWait);
                    Step.iHome++;
                    return false;

                case 14:
                    if(!MT_GetStopInpos(mi.ULDR_XNzzl))return false;
                    if(!MT_GetStopInpos(mi.ULDR_YIndx))return false;
                    Step.iHome = 0;
                    return true ;
            }
        }

        string GetS(double _dVal)
        {
            return string.Format("{0:0.000}", _dVal);
        }

        public bool CycleMove()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "" + Step.eSeq.ToString() + "" + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
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

            int c, r;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.ULDR_XNzzl  , pv.ULDR_XNzzlWait);
                    MoveMotr(mi.ULDR_ZNzzl  , pv.ULDR_ZNzzlWait);
                    Step.iCycle++;
                    return false ;

                case 11:
               
                    if (!MT_GetStopPos(mi.ULDR_XNzzl , pv.ULDR_XNzzlWait)) return false;
                    if (!MT_GetStopPos(mi.ULDR_ZNzzl , pv.ULDR_ZNzzlWait)) return false;
                    FindChip(ri.ULDR , out c , out r, cs.Empty);
                    //MoveMotr(mi.LODR_YIndx, pv.LODR_YIndxWork, (OM.DevInfo.iLODR_BarCnt - r - 1) * OM.DevInfo.dLODR_BarPitch);
                    MoveMotr(mi.ULDR_YIndx, pv.ULDR_YIndxWork, (OM.DevInfo.iULDR_BarCnt - r - 1) * OM.DevInfo.dULDR_BarPitch);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopInpos(mi.ULDR_YIndx))return false;
                    FindChip(ri.ULDR , out c , out r, cs.Empty);
                    DM.ARAY[ri.ULDR].SetStat(c, r, cs.Work);
                    if (r != OM.EqpStat.iULDRWorkNo) OM.EqpStat.iULDRCnt = 0;
                    OM.EqpStat.iULDRWorkNo = r;
                    Step.iCycle = 0;
                    return true ;                    
            }
        }

        public bool CyclePick()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "" + Step.eSeq.ToString() + "" + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
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

            int c, r;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveCyl (ci.ULDR_RngReGrFwBw  , fb.Bwd   );
                    MoveCyl (ci.ULDR_ReGrRtrCwCCw , fb.Bwd   );
                    //MoveCyl (ci.ULDR_OutPshrFwBw  , fb.Bwd   );
                    MoveMotr(mi.ULDR_XGrpr  , pv.ULDR_XGrprPick , 10);
                    //MoveMotr(mi.ULDR_YIndx  , pv.ULDR_YIndxWork);
                    MoveMotr(mi.ULDR_ZPckr  , pv.ULDR_ZPckrPick);
                    MoveMotr(mi.ULDR_ZNzzl  , pv.ULDR_ZNzzlWait);
                    
                    Step.iCycle++;
                    return false ;

                case 11:
                    if (!CL_Complete  (ci.ULDR_RngReGrFwBw  )) return false;
                    if (!CL_Complete  (ci.ULDR_ReGrRtrCwCCw )) return false;
                    //if (!CL_Complete  (ci.ULDR_OutPshrFwBw  )) return false;
                    if (!MT_GetStopInpos(mi.ULDR_XGrpr)) return false;
                    if (!MT_GetStopPos(mi.ULDR_ZPckr, pv.ULDR_ZPckrPick)) return false;
                    if (!MT_GetStopPos(mi.ULDR_ZNzzl, pv.ULDR_ZNzzlWait)) return false;
                    FindChip(ri.ULDR , out c , out r, cs.Work);
                    MoveMotr(mi.ULDR_YIndx, pv.ULDR_YIndxWork, (OM.DevInfo.iULDR_BarCnt - r - 1) * OM.DevInfo.dULDR_BarPitch);
                    //MoveMotr(mi.ULDR_XGrpr, pv.ULDR_XGrprPick, 10);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopInpos(mi.ULDR_YIndx)) return false;
                    if (!MT_GetStopInpos(mi.ULDR_XGrpr)) return false;    
                    //MoveCyl(ci.ULDR_RngGrpFwBw  , fb.Fwd);
                    MoveCyl(ci.ULDR_GrpRtrCwCCw , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 13:
                    //if (!CL_Complete(ci.ULDR_RngGrpFwBw  , fb.Fwd)) return false;
                    if (!CL_Complete(ci.ULDR_GrpRtrCwCCw , fb.Fwd)) return false;
                    MoveMotr(mi.ULDR_XGrpr, pv.ULDR_XGrprPick);
                    //MoveMotr(mi.ULDR_XGrpr , pv.ULDR_XGrprPick , - 10);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.ULDR_XGrpr , pv.ULDR_XGrprPick))return false;
                    MoveCyl(ci.ULDR_RngReGrFwBw, fb.Fwd);
                    
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.ULDR_RngReGrFwBw, fb.Fwd)) return false;
                    MoveCyl(ci.ULDR_RngGrpFwBw , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!CL_Complete(ci.ULDR_RngGrpFwBw, fb.Bwd)) return false;
                    MoveMotr(mi.ULDR_XGrpr, pv.ULDR_XGrprPick, 10);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!MT_GetStopInpos(mi.ULDR_XGrpr)) return false;
                    MoveCyl(ci.ULDR_ReGrRtrCwCCw, fb.Fwd);
                    MoveCyl(ci.ULDR_GrpRtrCwCCw, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!CL_Complete(ci.ULDR_ReGrRtrCwCCw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.ULDR_GrpRtrCwCCw , fb.Bwd)) return false;
                    MoveCyl(ci.ULDR_GrpFwBw, fb.Fwd);
                    DM.ARAY[ri.PULD].SetStat(cs.None);
                    DM.ARAY[ri.PICK].SetStat(cs.Unknown);
                    Step.iCycle = 0;
                    return true;

            }
        }
        public bool CyclePlce()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "" + Step.eSeq.ToString() + "" + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
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

            int c, r;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.ULDR_ZPckr, pv.ULDR_ZPckrPlace);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopInpos(mi.ULDR_ZPckr)) return false;
                    MoveMotr(mi.ULDR_XGrpr, pv.ULDR_XGrprPlace);
                    //MoveCyl(ci.ULDR_GrpFwBw, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.ULDR_XGrpr, pv.ULDR_XGrprPlace)) return false;
                    MoveMotr(mi.ULDR_ZPckr, pv.ULDR_ZPckrPlace);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.ULDR_ZPckr, pv.ULDR_ZPckrPlace)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    MoveCyl(ci.ULDR_RngReGrFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.ULDR_RngReGrFwBw, fb.Bwd)) return false;
                    //MoveMotr(mi.ULDR_YIndx  , pv.ULDR_YIndxWork);
                    MoveMotr(mi.ULDR_ZPckr  , pv.ULDR_ZPckrPlace , 10);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStopInpos(mi.ULDR_ZPckr)) return false;
                    MoveCyl(ci.ULDR_ReGrRtrCwCCw, fb.Bwd);
                    DM.ARAY[ri.PICK].SetStat(cs.None);
                    DM.ARAY[ri.PSHR].SetStat(cs.Good);
                    Step.iCycle++;
                    return false;

                case 17:
                    //if (!CL_Complete(ci.ULDR_RngReGrFwBw, fb.Bwd)) return false;
                    //if (!CL_Complete(ci.ULDR_GrpFwBw, fb.Fwd)) return false;
                    
                    if (!CL_Complete(ci.ULDR_ReGrRtrCwCCw, fb.Bwd)) return false;
                    MoveMotr(mi.ULDR_ZPckr, pv.ULDR_ZPckrPick);
                    MoveMotr(mi.ULDR_XGrpr, pv.ULDR_XGrprPick , 10);
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!MT_GetStopPos(mi.ULDR_ZPckr , pv.ULDR_ZPckrPick))return false;
                    if(!MT_GetStopInpos(mi.ULDR_XGrpr)) return false;
                    //MoveCyl(ci.ULDR_OutPshrFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 19:
                    
                    OM.EqpStat.iWorkCnt++;
                    OM.EqpStat.iULDRCnt++;
                    Step.iCycle = 0;
                    return true ;                    
            }
        }

        public bool CyclePusher()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(iPshrCycle != 0 && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = CL_GetName(ci.ULDR_OutPshrFwBw); //string.Format("Cycle Step.iCycle={0:00}", iPshrCycle);
                sTemp = m_sPartName + "" + Step.eSeq.ToString() + "" + sTemp;
                ER_SetErr(ei.ATR_TimeOut, sTemp);
                DM.ARAY[ri.PSHR].SetStat(cs.None);
                Trace(sTemp);
                return true;
            }

            switch (iPshrCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveCyl(ci.ULDR_OutPshrFwBw, fb.Fwd);
                    iPshrCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.ULDR_OutPshrFwBw, fb.Fwd)) return false;
                    MoveCyl(ci.ULDR_OutPshrFwBw, fb.Bwd);
                    iPshrCycle++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.ULDR_OutPshrFwBw, fb.Bwd)) return false;
                    DM.ARAY[ri.PSHR].SetStat(cs.None);
                    iPshrCycle = 0;
                    return true;
            }
        }

        int m_iPaintRepeatCnt = 0;
        public bool CyclePaint()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "" + Step.eSeq.ToString() + "" + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
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

            int c, r;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.ULDR_ZNzzl, pv.ULDR_ZNzzlWork   );
                    MoveMotr(mi.ULDR_XNzzl, pv.ULDR_XNzzlWorkStt , OM.DevInfo.iPaintColorNo * OM.DevInfo.dNozzelXPitch);
                    MoveCyl(ci.ULDR_OutPshrFwBw, fb.Fwd);
                    m_iPaintRepeatCnt = 0;
                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if (!MT_GetStopPos  (mi.ULDR_ZNzzl, pv.ULDR_ZNzzlWork   )) return false;
                    if (!MT_GetStopInpos(mi.ULDR_XNzzl)) return false;
                    if (!CL_Complete(ci.ULDR_OutPshrFwBw, fb.Fwd)) return false;
                    if (!OM.CmnOptn.bPaintSkip)
                    {
                             if (OM.DevInfo.iPaintColorNo == 0) IO_SetY(yi.ETC_Manual1, true);
                        else if (OM.DevInfo.iPaintColorNo == 1) IO_SetY(yi.ETC_Manual2, true);
                        else if (OM.DevInfo.iPaintColorNo == 2) IO_SetY(yi.ETC_Manual3, true);
                        else if (OM.DevInfo.iPaintColorNo == 3) IO_SetY(yi.ETC_Manual4, true);
                        else if (OM.DevInfo.iPaintColorNo == 4) IO_SetY(yi.ETC_Manual5, true);
                    }
                    m_tmDelay.Clear();
                    if (m_iPaintRepeatCnt >= OM.DevInfo.iNozzelRptCnt)
                    {
                        Step.iCycle = 13;
                        return false;
                    }
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iNozzelDelay)) return false;
                    MoveMotr(mi.ULDR_XNzzl, pv.ULDR_XNzzlWorkEnd, OM.DevInfo.iPaintColorNo * OM.DevInfo.dNozzelXPitch);
                    m_iPaintRepeatCnt++;
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopInpos(mi.ULDR_XNzzl)) return false;
                    if (m_iPaintRepeatCnt < OM.DevInfo.iNozzelRptCnt)
                    {
                        MoveMotr(mi.ULDR_XNzzl, pv.ULDR_XNzzlWorkStt, OM.DevInfo.iPaintColorNo * OM.DevInfo.dNozzelXPitch);
                        m_iPaintRepeatCnt++;
                        Step.iCycle = 11;
                        return false;
                    }
                    IO_SetY(yi.ETC_Manual1 , false);
                    IO_SetY(yi.ETC_Manual2 , false);
                    IO_SetY(yi.ETC_Manual3 , false);
                    IO_SetY(yi.ETC_Manual4 , false);
                    IO_SetY(yi.ETC_Manual5 , false);
                    MoveMotr(mi.ULDR_ZNzzl, pv.ULDR_ZNzzlWait);
                    MoveMotr(mi.ULDR_XNzzl, pv.ULDR_XNzzlWait);
                    MoveCyl(ci.ULDR_OutPshrFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.ULDR_ZNzzl , pv.ULDR_ZNzzlWait))return false;
                    if(!MT_GetStopPos(mi.ULDR_XNzzl , pv.ULDR_XNzzlWait))return false;
                    if (!CL_Complete(ci.ULDR_OutPshrFwBw, fb.Bwd)) return false;
                    DM.ARAY[ri.ULDR].ChangeStat(cs.Work , cs.WorkEnd);
                    DM.ARAY[ri.PSHR].SetStat(cs.None);
                    iPshrCycle = 0;
                    //OM.EqpStat.iULDRCnt = 0;
                    Step.iCycle = 0;
                    return true ;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.ULDR_GrpFwBw){
                if(CL_GetCmd(ci.ULDR_RngGrpFwBw) != fb.Bwd && _eFwd == fb.Fwd) 
                {
                    sMsg = CL_GetName(ci.ULDR_RngGrpFwBw) + "is not Open";
                    bRet = false;
                }
            }
            
            else if(_eActr == ci.ULDR_GrpRtrCwCCw){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            
            else if(_eActr == ci.ULDR_OutPshrFwBw){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.ULDR_ReGrRtrCwCCw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.ULDR_RngGrpFwBw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.ULDR_RngReGrFwBw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
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

        public bool CheckSafe(mi _eMotr, pv _ePstn ,  double _dOfsPos=0)
        {
            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";

            //TOOL_ZVisn


            if (_eMotr == mi.ULDR_XNzzl)
            {

            
            }
            else if(_eMotr == mi.ULDR_XGrpr)
            {

            }
            else if(_eMotr == mi.ULDR_YIndx)
            {
                if(MT_GetCmdPos(mi.ULDR_ZNzzl) > PM.GetValue(mi.ULDR_ZNzzl , pv.ULDR_ZNzzlWait) + 10)
                {
                    sMsg = MT_GetName(mi.ULDR_ZNzzl) + "is not wait Position";
                    bRet = false;
                }
                if (CL_GetCmd(ci.ULDR_OutPshrFwBw) != fb.Bwd)
                {
                    sMsg = CL_GetName(ci.ULDR_OutPshrFwBw) + "is not Bwd";
                    bRet = false;
                }
            }
            else if(_eMotr == mi.ULDR_ZNzzl)
            {

            }
            else if(_eMotr == mi.ULDR_ZPckr)
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
            if (!MT_GetStop(mi.ULDR_ZPckr)) return false;
            if (!MT_GetStop(mi.ULDR_XGrpr)) return false;
            if (!MT_GetStop(mi.ULDR_YIndx)) return false;
            if (!MT_GetStop(mi.ULDR_ZNzzl)) return false;
            if (!MT_GetStop(mi.ULDR_XNzzl)) return false;

            
            if (!CL_Complete(ci.ULDR_ReGrRtrCwCCw)) return false;
            if (!CL_Complete(ci.ULDR_RngReGrFwBw )) return false;
            if (!CL_Complete(ci.ULDR_OutPshrFwBw )) return false;
            if (!CL_Complete(ci.ULDR_RngGrpFwBw  )) return false;
            if (!CL_Complete(ci.ULDR_GrpRtrCwCCw )) return false;
            if (!CL_Complete(ci.ULDR_GrpFwBw     )) return false;



            return true;
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
