using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;
using System.Collections.Generic;

namespace Machine
{
    public class VisnZone : Part
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
            Idle    = 0,
            Move       ,
            Insp       ,
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

        public int iWorkC ;
        public int iWorkR ;

        // OM에 있는걸로 같이 쓴다. ArrayPos ArrayPosition = new ArrayPos();

        //비전 통신 관련.
        public VisnCom[] VisnComs = new VisnCom[(int)vi.MAX_VI];

        public VisnZone(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();

            m_CycleTime   = new CTimer[(int)sc.MAX_SEQ_CYCLE];

            for (int i = 0; i < (int)sc.MAX_SEQ_CYCLE; i++)
            {
                m_CycleTime[i] = new CTimer();
            }


            VisnCom.TPara Para ; 

            Para.Id          = vi.Vs1L ;
            Para.yiLotStart  = yi.HEAD_Vsn1_LLotStart  ;
            Para.yiReset     = yi.HEAD_Vsn1_LReset     ;
            Para.yiJobStart  = yi.HEAD_Vsn1_LJobChange ;
            Para.xiVisnReady = xi.HEAD_Vsn1_LReady     ;
            Para.xiVisnBusy  = xi.HEAD_Vsn1_LBusy      ;
            Para.xiVisnEnd   = xi.HEAD_Vsn1_LEnd       ;
            VisnComs[(int)Para.Id] = new VisnCom(Para) ;

            Para.Id          = vi.Vs1R ;
            Para.yiLotStart  = yi.HEAD_Vsn1_RLotStart  ;
            Para.yiReset     = yi.HEAD_Vsn1_RReset     ;
            Para.yiJobStart  = yi.HEAD_Vsn1_RJobChange ;
            Para.xiVisnReady = xi.HEAD_Vsn1_RReady     ;
            Para.xiVisnBusy  = xi.HEAD_Vsn1_RBusy      ;
            Para.xiVisnEnd   = xi.HEAD_Vsn1_REnd       ;
            VisnComs[(int)Para.Id] = new VisnCom(Para) ;

            Para.Id          = vi.Vs2L ;
            Para.yiLotStart  = yi.HEAD_Vsn2_LLotStart  ;
            Para.yiReset     = yi.HEAD_Vsn2_LReset     ;
            Para.yiJobStart  = yi.HEAD_Vsn2_LJobChange ;
            Para.xiVisnReady = xi.HEAD_Vsn2_LReady     ;
            Para.xiVisnBusy  = xi.HEAD_Vsn2_LBusy      ;
            Para.xiVisnEnd   = xi.HEAD_Vsn2_LEnd       ;
            VisnComs[(int)Para.Id] = new VisnCom(Para) ;

            Para.Id          = vi.Vs2R ;
            Para.yiLotStart  = yi.HEAD_Vsn2_RLotStart  ;
            Para.yiReset     = yi.HEAD_Vsn2_RReset     ;
            Para.yiJobStart  = yi.HEAD_Vsn2_RJobChange ;
            Para.xiVisnReady = xi.HEAD_Vsn2_RReady     ;
            Para.xiVisnBusy  = xi.HEAD_Vsn2_RBusy      ;
            Para.xiVisnEnd   = xi.HEAD_Vsn2_REnd       ;
            VisnComs[(int)Para.Id] = new VisnCom(Para) ;

            Para.Id          = vi.Vs3L ;
            Para.yiLotStart  = yi.HEAD_Vsn3_LLotStart  ;
            Para.yiReset     = yi.HEAD_Vsn3_LReset     ;
            Para.yiJobStart  = yi.HEAD_Vsn3_LJobChange ;
            Para.xiVisnReady = xi.HEAD_Vsn3_LReady     ;
            Para.xiVisnBusy  = xi.HEAD_Vsn3_LBusy      ;
            Para.xiVisnEnd   = xi.HEAD_Vsn3_LEnd       ;
            VisnComs[(int)Para.Id] = new VisnCom(Para) ;

            Para.Id          = vi.Vs3R ;
            Para.yiLotStart  = yi.HEAD_Vsn3_RLotStart  ;
            Para.yiReset     = yi.HEAD_Vsn3_RReset     ;
            Para.yiJobStart  = yi.HEAD_Vsn3_RJobChange ;
            Para.xiVisnReady = xi.HEAD_Vsn3_RReady     ;
            Para.xiVisnBusy  = xi.HEAD_Vsn3_RBusy      ;
            Para.xiVisnEnd   = xi.HEAD_Vsn3_REnd       ;
            VisnComs[(int)Para.Id] = new VisnCom(Para) ;

            

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
        bool bFirstReset = true ;
        override public void Reset() //리셑 버튼 눌렀을때 타는 함수.
        {

            ResetTimer();

            Stat.Clear();
            Step.Clear();
            PreStep.Clear();

            if (!bFirstReset)
            {
                for (int i = 0; i < (int)vi.MAX_VI; i++)
                {
                    if (!OM.VsSkip((vi)i) && VisnComs[i].EndCmd()) VisnComs[i].SendCmd(VisnCom.vc.Reset);
                }
                
            }
            bFirstReset = false ;
        }

        //Running Functions.
        override public void Update()
        {
            for(int i = 0 ; i < (int)vi.MAX_VI ; i++)
            {
                VisnComs[i].Update();
            }
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
                    Step.iToStart = 0;
                    return true;

                case 10:
                    if(!OM.VsSkip(vi.Vs1L)) { if(!VisnComs[(int)vi.Vs1L].EndCmd()) return false; }
                    if(!OM.VsSkip(vi.Vs1R)) { if(!VisnComs[(int)vi.Vs1R].EndCmd()) return false; }
                    if(!OM.VsSkip(vi.Vs2L)) { if(!VisnComs[(int)vi.Vs2L].EndCmd()) return false; }
                    if(!OM.VsSkip(vi.Vs2R)) { if(!VisnComs[(int)vi.Vs2R].EndCmd()) return false; }
                    if(!OM.VsSkip(vi.Vs3L)) { if(!VisnComs[(int)vi.Vs3L].EndCmd()) return false; }
                    if(!OM.VsSkip(vi.Vs3R)) { if(!VisnComs[(int)vi.Vs3R].EndCmd()) return false; }

                    if(!OM.VsSkip(vi.Vs1L)) { VisnComs[(int)vi.Vs1L].SendCmd(VisnCom.vc.Reset); }
                    if(!OM.VsSkip(vi.Vs1R)) { VisnComs[(int)vi.Vs1R].SendCmd(VisnCom.vc.Reset); }
                    if(!OM.VsSkip(vi.Vs2L)) { VisnComs[(int)vi.Vs2L].SendCmd(VisnCom.vc.Reset); }
                    if(!OM.VsSkip(vi.Vs2R)) { VisnComs[(int)vi.Vs2R].SendCmd(VisnCom.vc.Reset); }
                    if(!OM.VsSkip(vi.Vs3L)) { VisnComs[(int)vi.Vs3L].SendCmd(VisnCom.vc.Reset); }
                    if(!OM.VsSkip(vi.Vs3R)) { VisnComs[(int)vi.Vs3R].SendCmd(VisnCom.vc.Reset); }


                    m_tmDelay.Clear();
                    //검사시에 타임아웃이나 결과값 못일어서 에러난경우는 언노운으로 재마스킹 해서 다시 검사하게.
                    iManWorkC = 0 ; 
                    iManWorkR = 0 ;
                    bManRight = true ;

                    DM.ARAY[ri.VSN1].ChangeStat(cs.Wait , cs.Unknown);
                    DM.ARAY[ri.VSN2].ChangeStat(cs.Wait , cs.Unknown);
                    DM.ARAY[ri.VSN3].ChangeStat(cs.Wait , cs.Unknown);

                    MoveCyl(ci.RAIL_Vsn1AlignFwBw, fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw, fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw, fb.Bwd);

                    //MoveCyl(ci.RAIL_Vsn1SttnUpDn, fb.Bwd);
                    //MoveCyl(ci.RAIL_Vsn2SttnUpDn, fb.Bwd);
                    //MoveCyl(ci.RAIL_Vsn3SttnUpDn, fb.Bwd);

                    Step.iToStart++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw, fb.Bwd)) return false ;

                    //if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn, fb.Bwd)) return false ;
                    //if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn, fb.Bwd)) return false ;
                    //if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn, fb.Bwd)) return false ;

                    MoveCyl(ci.RAIL_Vsn1StprUpDn, fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2StprUpDn, fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3StprUpDn, fb.Fwd);

                    MoveCyl(ci.RAIL_Vsn1SttnUpDn, fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2SttnUpDn, fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3SttnUpDn, fb.Bwd);

                    Step.iToStart++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.RAIL_Vsn1StprUpDn, fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2StprUpDn, fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3StprUpDn, fb.Fwd)) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn, fb.Bwd)) return false ;

                    if(!m_tmDelay.OnDelay(true,1000)) return false;
                    if(!OM.VsSkip(vi.Vs1L) && !VisnComs[(int)vi.Vs1L].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 1 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs1R) && !VisnComs[(int)vi.Vs1R].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 2 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs2L) && !VisnComs[(int)vi.Vs2L].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 3 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs2R) && !VisnComs[(int)vi.Vs2R].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 4 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs3L) && !VisnComs[(int)vi.Vs3L].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 5 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs3R) && !VisnComs[(int)vi.Vs3R].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 6 Not Ready"); return true ;}

                    Step.iToStart = 0;
                    return true;
                    
            }

        }
        override public bool ToStop() //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 5000)) {
                ER_SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);
                IO_SetY(yi.RAIL_FeedingAC1,false);
                IO_SetY(yi.RAIL_FeedingAC2,false);
                IO_SetY(yi.RAIL_FeedingAC3,false);
            }

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

                    MT_ResetTrgPos(mi.HEAD_XVisn);

                    MoveCyl(ci.RAIL_Vsn1AlignFwBw, fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw, fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw, fb.Bwd);

                    MoveCyl(ci.RAIL_Vsn1SttnUpDn, fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2SttnUpDn, fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3SttnUpDn, fb.Bwd);

                    Step.iToStop++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw, fb.Bwd)) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn, fb.Bwd)) return false ;

                    MoveCyl(ci.RAIL_Vsn1StprUpDn, fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2StprUpDn, fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3StprUpDn, fb.Fwd);

                    Step.iToStop++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.RAIL_Vsn1StprUpDn, fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2StprUpDn, fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3StprUpDn, fb.Fwd)) return false ;

                    if(!MT_GetStop(mi.HEAD_XVisn)) return false;
                    if(!MT_GetStop(mi.HEAD_YVisn)) return false;
                    MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnWait);
                    MoveMotr(mi.HEAD_YVisn,pv.HEAD_YVisnWait);
                    Step.iToStop++;
                    return false ;

                case 13: 
                    if(!MT_GetStopPos(mi.HEAD_XVisn,pv.HEAD_XVisnWait)) return false ;
                    if(!MT_GetStopPos(mi.HEAD_YVisn,pv.HEAD_YVisnWait)) return false ;
                    IO_SetY(yi.RAIL_FeedingAC1,false);
                    IO_SetY(yi.RAIL_FeedingAC2,false);
                    IO_SetY(yi.RAIL_FeedingAC3,false);
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

                //자재 상태
                bool PrebNone   = DM.ARAY[ri.PREB].CheckAllStat(cs.None   )     ;
                bool Vsn1None   = DM.ARAY[ri.VSN1].CheckAllStat(cs.None   )     ;
                bool Vsn1Unkwn  = DM.ARAY[ri.VSN1].GetCntStat  (cs.Unknown) > 0 ;
                bool Vsn1Wait   = DM.ARAY[ri.VSN1].GetCntStat  (cs.Wait   ) > 0 ;
                bool Vsn2None   = DM.ARAY[ri.VSN2].CheckAllStat(cs.None   )     ;
                bool Vsn2Unkwn  = DM.ARAY[ri.VSN2].GetCntStat  (cs.Unknown) > 0 ;
                bool Vsn2Wait   = DM.ARAY[ri.VSN2].GetCntStat  (cs.Wait   ) > 0 ;
                bool Vsn3None   = DM.ARAY[ri.VSN3].CheckAllStat(cs.None   )     ;
                bool Vsn3Unkwn  = DM.ARAY[ri.VSN3].GetCntStat  (cs.Unknown) > 0 ;
                bool Vsn3Wait   = DM.ARAY[ri.VSN3].GetCntStat  (cs.Wait   ) > 0 ;
                bool PstbNone   = DM.ARAY[ri.PSTB].CheckAllStat(cs.None   )     ;

                bool bPrbCheck = IO_GetX(xi.PREB_StrpDetect) ;
                bool bVs1Check = IO_GetX(xi.RAIL_Vsn1Detect) ;
                bool bVs2Check = IO_GetX(xi.RAIL_Vsn2Detect) ;
                bool bVs3Check = IO_GetX(xi.RAIL_Vsn3Detect) ;

                string sLDR_NextStripLotNo = "";
                if(!DM.ARAY[ri.LODR].CheckAllStat(cs.None) ) sLDR_NextStripLotNo = DM.ARAY[ri.LODR].LotNo ;
                else                                         sLDR_NextStripLotNo = LOT.GetNextMgz()       ;
                bool bEnd = LOT.GetNextMgz() == "" && DM.ARAY[ri.LODR].GetCntStat(cs.Unknown) == 0 ;

                //CycleMove관련.
                bool LodrWorking = SEQ.LODR.GetSeqStep() == (int)Loader.sc.Work ; //워크가 아닐때만 작업을 함. 
                bool PrebReady   =(!PrebNone || bEnd || sLDR_NextStripLotNo != LOT.GetLotNo()) && !LodrWorking ; //로더에서 랏체인지시에 레일에 자제 없게 메거진을 잡고 있는다.
                bool Vsn1Ready   = Vsn1None || (!Vsn1Unkwn && !Vsn1Wait) ;
                bool Vsn2Ready   = Vsn2None || (!Vsn2Unkwn && !Vsn2Wait) ;
                bool Vsn3Ready   = Vsn3None || (!Vsn3Unkwn && !Vsn3Wait) ;
                bool PstbReady   = PstbNone  && SEQ.PSTB.Wait(); //스토퍼랑 리프트등 자재 받을수 있는 상태

                //사이클
                bool isCycleInsp = Vsn1Unkwn || Vsn2Unkwn || Vsn3Unkwn ;
                bool isCycleMove = PrebReady && Vsn1Ready && Vsn2Ready && Vsn3Ready && PstbReady && 
                                 (!PrebNone || !Vsn1None || !Vsn2None || !Vsn3None);
                bool isEnd       = Vsn1None  && Vsn2None  && Vsn3None  ; 


                //모르는 자제 에러
                if( PrebNone && bPrbCheck && !IO_GetY(yi.RAIL_FeedingAC1)) ER_SetErr(ei.PKG_Unknwn,"Prebuffer have no data found, but strip sensor detected") ;
                if( Vsn1None && bVs1Check) ER_SetErr(ei.PKG_Unknwn,"VisnZone1 have no data found, but strip sensor detected") ;
                if( Vsn2None && bVs2Check) ER_SetErr(ei.PKG_Unknwn,"VisnZone2 have no data found, but strip sensor detected") ;
                if( Vsn3None && bVs3Check) ER_SetErr(ei.PKG_Unknwn,"VisnZone3 have no data found, but strip sensor detected") ;
                
                //카세트 사라짐
                if(!PrebNone && !bPrbCheck) ER_SetErr(ei.PKG_Dispr  ,"Prebuffer have data, but strip sensor not detected") ;
                if(!Vsn1None && !bVs1Check) ER_SetErr(ei.PKG_Dispr  ,"VisnZone1 have data, but strip sensor not detected") ;
                if(!Vsn2None && !bVs2Check) ER_SetErr(ei.PKG_Dispr  ,"VisnZone2 have data, but strip sensor not detected") ;
                if(!Vsn3None && !bVs3Check) ER_SetErr(ei.PKG_Dispr  ,"VisnZone3 have data, but strip sensor not detected") ;

                if (ER_IsErr())
                {
                    return false;
                }
                     if (isCycleInsp      ) { DM.ARAY[ri.VSN1].Trace();DM.ARAY[ri.VSN2].Trace();DM.ARAY[ri.VSN3].Trace(); Step.eSeq = sc.Insp ; } 
                else if (isCycleMove      ) { DM.ARAY[ri.VSN1].Trace();DM.ARAY[ri.VSN2].Trace();DM.ARAY[ri.VSN3].Trace(); Step.eSeq = sc.Move ; } 
                else if (isEnd            ) { Stat.bWorkEnd = true; return true; }
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
                default        : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case (sc.Idle ): return false;
                case (sc.Insp ): if (!CycleInsp ()) return false; break;
                case (sc.Move ): if (!CycleMove ()) return false; break;
            }
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        public void JobChange()
        { 
            VisnComs[(int)vi.Vs1L].SendCmd(VisnCom.vc.JobChange,OM.GetCrntDev());
            VisnComs[(int)vi.Vs1R].SendCmd(VisnCom.vc.JobChange,OM.GetCrntDev());
            VisnComs[(int)vi.Vs2L].SendCmd(VisnCom.vc.JobChange,OM.GetCrntDev());
            VisnComs[(int)vi.Vs2R].SendCmd(VisnCom.vc.JobChange,OM.GetCrntDev());
            VisnComs[(int)vi.Vs3L].SendCmd(VisnCom.vc.JobChange,OM.GetCrntDev());
            VisnComs[(int)vi.Vs3R].SendCmd(VisnCom.vc.JobChange,OM.GetCrntDev());
        }

        //위에 부터 작업.
        //인덱스 데이터에서 작업해야 되는 컬로우를 뽑아서 리턴.
        public bool FindChip(out int c, out int r, cs _iChip=cs.RetFail) 
        {
            //if(DM.ARAY[ri.VSN1].CheckAllStat(cs.None)){c=-1 ; r =-1 ; return false ;}
            //if(DM.ARAY[ri.VSN2].CheckAllStat(cs.None)){c=-1 ; r =-1 ; return false ;}
            //if(DM.ARAY[ri.VSN3].CheckAllStat(cs.None)){c=-1 ; r =-1 ; return false ;}



            int r1 ;
            int c1 ;
            int r2 ;
            int c2 ;
            int r3 ;
            int c3 ;
            
            //if(HEAD_INSP_DIRECTION == idLeftTop) {
            r = DM.ARAY[ri.VSN1].GetMaxRow() ;
            c = DM.ARAY[ri.VSN1].GetMaxCol() ;
            
            r1 = DM.ARAY[ri.VSN1].GetMaxRow() ;
            c1 = DM.ARAY[ri.VSN1].GetMaxCol() ;
            r2 = DM.ARAY[ri.VSN2].GetMaxRow() ;
            c2 = DM.ARAY[ri.VSN2].GetMaxCol() ;
            r3 = DM.ARAY[ri.VSN3].GetMaxRow() ;
            c3 = DM.ARAY[ri.VSN3].GetMaxCol() ;

            //3존 중에 가장 먼저 있는 자제를 한다.
            if(!DM.ARAY[ri.VSN1].CheckAllStat(cs.None)){r1 = DM.ARAY[ri.VSN1].FindFrstRow(_iChip); r = r1 ; c = c1 ;}
            if(!DM.ARAY[ri.VSN2].CheckAllStat(cs.None)){r2 = DM.ARAY[ri.VSN2].FindFrstRow(_iChip); if(r > r2) { r = r2 ; c = c2 ;}}
            if(!DM.ARAY[ri.VSN3].CheckAllStat(cs.None)){r3 = DM.ARAY[ri.VSN3].FindFrstRow(_iChip); if(r > r3) { r = r3 ; c = c3 ;}}           
            
            //if(r == -1 && c == -1) return false ;
            if(r == -1) return false ;
            
            return true ;
        }

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                IO_SetY(yi.RAIL_FeedingAC2,false);
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

            switch (Step.iHome)
            {
                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    Trace(sTemp);
                    return true ;

                case 10:
                    

                    IO_SetY(yi.RAIL_FeedingAC1,false);
                    IO_SetY(yi.RAIL_FeedingAC2,false);
                    IO_SetY(yi.RAIL_FeedingAC3,false);

                    MT_GoHome(mi.HEAD_XVisn) ;
                    MT_GoHome(mi.HEAD_YVisn) ;

                    MT_GoHome(mi.HEAD_XCvr1) ;
                    MT_GoHome(mi.HEAD_XCvr2) ;
                    MT_GoHome(mi.HEAD_XCvr3) ;

                    CL_Move(ci.RAIL_Vsn1AlignFwBw,fb.Bwd);
                    CL_Move(ci.RAIL_Vsn2AlignFwBw,fb.Bwd);
                    CL_Move(ci.RAIL_Vsn3AlignFwBw,fb.Bwd);
                    
                    CL_Move(ci.RAIL_Vsn1SttnUpDn ,fb.Bwd);
                    CL_Move(ci.RAIL_Vsn2SttnUpDn ,fb.Bwd);
                    CL_Move(ci.RAIL_Vsn3SttnUpDn ,fb.Bwd);

                    Step.iHome++;
                    return false;

                case 11:
                    if(!MT_GetHomeDone(mi.HEAD_XVisn)) return false ;
                    if(!MT_GetHomeDone(mi.HEAD_YVisn)) return false ;

                    if(!MT_GetHomeDone(mi.HEAD_XCvr1)) return false ;
                    if(!MT_GetHomeDone(mi.HEAD_XCvr2)) return false ;
                    if(!MT_GetHomeDone(mi.HEAD_XCvr3)) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw,fb.Bwd)) return false;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw,fb.Bwd)) return false;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw,fb.Bwd)) return false;

                    if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn,fb.Bwd)) return false;
                    if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn,fb.Bwd)) return false;
                    if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn,fb.Bwd)) return false;

                    CL_Move(ci.RAIL_Vsn1StprUpDn ,fb.Fwd);
                    CL_Move(ci.RAIL_Vsn2StprUpDn ,fb.Fwd);
                    CL_Move(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);

                    MT_GoAbsMan(mi.HEAD_XVisn,PM.GetValue(mi.HEAD_XVisn,pv.HEAD_XVisnWait));
                    MT_GoAbsMan(mi.HEAD_YVisn,PM.GetValue(mi.HEAD_YVisn,pv.HEAD_YVisnWait));

                    MT_GoAbsMan(mi.HEAD_XCvr1,PM.GetValue(mi.HEAD_XCvr1,pv.HEAD_XCvr1Wait));
                    MT_GoAbsMan(mi.HEAD_XCvr2,PM.GetValue(mi.HEAD_XCvr2,pv.HEAD_XCvr2Wait));
                    MT_GoAbsMan(mi.HEAD_XCvr3,PM.GetValue(mi.HEAD_XCvr3,pv.HEAD_XCvr3Wait));
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw,fb.Bwd)) return false;
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw,fb.Bwd)) return false;
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw,fb.Bwd)) return false;

                    if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_XVisnWait)) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_YVisnWait)) return false ;
                                  
                    if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_XCvr1Wait)) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_XCvr2Wait)) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_XCvr3Wait)) return false ;

                    if(!OM.VsSkip(vi.Vs1L)) VisnComs[(int)vi.Vs1L].SendCmd(VisnCom.vc.Reset);
                    if(!OM.VsSkip(vi.Vs1R)) VisnComs[(int)vi.Vs1R].SendCmd(VisnCom.vc.Reset);
                    if(!OM.VsSkip(vi.Vs2L)) VisnComs[(int)vi.Vs2L].SendCmd(VisnCom.vc.Reset);
                    if(!OM.VsSkip(vi.Vs2R)) VisnComs[(int)vi.Vs2R].SendCmd(VisnCom.vc.Reset);
                    if(!OM.VsSkip(vi.Vs3L)) VisnComs[(int)vi.Vs3L].SendCmd(VisnCom.vc.Reset);
                    if(!OM.VsSkip(vi.Vs3R)) VisnComs[(int)vi.Vs3R].SendCmd(VisnCom.vc.Reset);

                    Step.iHome++;
                    return false ;

                case 13:
                    if(!OM.VsSkip(vi.Vs1L) && !VisnComs[(int)vi.Vs1L].EndCmd()) return false ;
                    if(!OM.VsSkip(vi.Vs1R) && !VisnComs[(int)vi.Vs1R].EndCmd()) return false ;
                    if(!OM.VsSkip(vi.Vs2L) && !VisnComs[(int)vi.Vs2L].EndCmd()) return false ;
                    if(!OM.VsSkip(vi.Vs2R) && !VisnComs[(int)vi.Vs2R].EndCmd()) return false ;
                    if(!OM.VsSkip(vi.Vs3L) && !VisnComs[(int)vi.Vs3L].EndCmd()) return false ;
                    if(!OM.VsSkip(vi.Vs3R) && !VisnComs[(int)vi.Vs3R].EndCmd()) return false ;
                    
                    if(!OM.VsSkip(vi.Vs1L) && VisnComs[(int)vi.Vs1L].GetErrMsg() != "") ER_SetErr(ei.VSN_ComErr , VisnComs[(int)vi.Vs1L].GetErrMsg());
                    if(!OM.VsSkip(vi.Vs1R) && VisnComs[(int)vi.Vs1R].GetErrMsg() != "") ER_SetErr(ei.VSN_ComErr , VisnComs[(int)vi.Vs1R].GetErrMsg());
                    if(!OM.VsSkip(vi.Vs2L) && VisnComs[(int)vi.Vs2L].GetErrMsg() != "") ER_SetErr(ei.VSN_ComErr , VisnComs[(int)vi.Vs2L].GetErrMsg());
                    if(!OM.VsSkip(vi.Vs2R) && VisnComs[(int)vi.Vs2R].GetErrMsg() != "") ER_SetErr(ei.VSN_ComErr , VisnComs[(int)vi.Vs2R].GetErrMsg());
                    if(!OM.VsSkip(vi.Vs3L) && VisnComs[(int)vi.Vs3L].GetErrMsg() != "") ER_SetErr(ei.VSN_ComErr , VisnComs[(int)vi.Vs3L].GetErrMsg());
                    if(!OM.VsSkip(vi.Vs3R) && VisnComs[(int)vi.Vs3R].GetErrMsg() != "") ER_SetErr(ei.VSN_ComErr , VisnComs[(int)vi.Vs3R].GetErrMsg()); 

                    Step.iHome = 0;
                    return true;
            }
        }
    
        //카메라 한쪽이 고장났을때.
        //오른쪽이 고장났을때는 그냥 라이트스킵만 한고 돌리면 되지만.
        //왼쪽이 고장났을때는 오른쪽카메라 띠어다 왼쪽으로 교체 하던가.
        //오른쪽카메라 기준으로 워크스타트 위치를 변경해야함.
        private List<double> GetTrgPosX(bool _bMinFirst ,double _dStartPos ,  double _dTrgOfs ,  bool _bHalfStroke = true)
        {
            double iCrntInspCnt = 0 ;
            double dCrntInspPos = 0.0 ;
            double dPitch = 0.0;
            double dNxtColPos = 0.0;
            double dX , dY ;
            List<double> TrgPos = new List<double>();
            //VsnColPos.Clear();
            int iTrgColCnt = 0;
            if (_bHalfStroke)
            {
                iTrgColCnt = OM.DevInfo.iColCnt / 2  + ((OM.DevInfo.iColCnt % 2 != 0) ? 1 : 0) ; //반나눠서 안떨어지면 한개 추가.
            }
            else
            {
                iTrgColCnt = OM.DevInfo.iColCnt ;
            }

            //if(iTr)

            for(int c = 0  ; c < iTrgColCnt ; c++ )
            {
                OM .StripPos.GetPos(c , 0 , 4 , out dX , out dY);
                if(iCrntInspCnt==0)dCrntInspPos = dX ;
                iCrntInspCnt++;
                
                if(c+1<iTrgColCnt)//마지막 자제가 아니면 
                {
                    OM .StripPos.GetPos(c+1 , 0 , 4 , out dNxtColPos , out dY); //다음자제 피치를 보고 같이 검사 가능한지 파악.
                    dPitch = dNxtColPos - dX ;
                    //배정도 실수라 안떨어짐.
                    
                    if((int)(Math.Round(dPitch*1000)) != (int)(OM.DevInfo.dColPitch*1000) || iCrntInspCnt == OM.DevInfo.iColInspCnt)
                    {
                        TrgPos.Add(dCrntInspPos); //VsnColPos.Add(c);
                        iCrntInspCnt=0;
                    }
                    continue ;
                }
                else //맨마지막자제이니 그냥 등록.
                {
                    TrgPos.Add(dCrntInspPos); //VsnColPos.Add(c);
                    iCrntInspCnt=0;
                }
            }
            if(_bMinFirst)
            {
                TrgPos.Sort((double x, double y) => x.CompareTo(y));
                for(int i = 0 ; i < TrgPos.Count ; i++)
                {
                    TrgPos[i] += _dStartPos ;
                    TrgPos[i] -= _dTrgOfs ;
                }
            }
            else
            {
                TrgPos.Sort((double x, double y) => y.CompareTo(x));
                for(int i = 0 ; i < TrgPos.Count ; i++)
                {
                    TrgPos[i] += _dStartPos ;
                    TrgPos[i] += _dTrgOfs ;
                }
            }

            return TrgPos ;
        }

        //메뉴얼 넥스트 에서 Y축 계산 할때 사용.
        private List<double> GetTrgPosY(bool _bMinFirst ,double _dStartPos )
        {
            double iCrntInspCnt = 0 ;
            double dCrntInspPos = 0.0 ;
            double dPitch = 0.0;
            double dNxtRowPos = 0.0;
            double dX , dY ;
            List<double> TrgPos = new List<double>(); 

            int iTrgRowCnt = OM.DevInfo.iRowCnt ;

            for(int r = 0  ; r < iTrgRowCnt ; r++ )
            {
                OM .StripPos.GetPos(0 , r , 4 , out dX , out dY);
                if(iCrntInspCnt==0)dCrntInspPos = dY ;
                iCrntInspCnt++;
                
                if(r+1<OM.DevInfo.iColCnt)//마지막 자제가 아니면 
                {
                    OM .StripPos.GetPos(0 , r+1 , 4 , out dX , out dNxtRowPos); //다음자제 피치를 보고 같이 검사 가능한지 파악.
                    dPitch = dNxtRowPos - dY ;
                    //배정도 실수라 안떨어짐.
                    
                    if((int)(Math.Round(dPitch*1000)) != (int)(OM.DevInfo.dRowPitch*1000) || iCrntInspCnt == OM.DevInfo.iRowInspCnt)
                    {
                        TrgPos.Add(dCrntInspPos); //VsnColPos.Add(c);
                        iCrntInspCnt=0;
                    }
                    continue ;
                }
                else //맨마지막자제이니 그냥 등록.
                {
                    TrgPos.Add(dCrntInspPos); //VsnColPos.Add(c);
                    iCrntInspCnt=0;
                }
            }

            if(_bMinFirst)
            {
                TrgPos.Sort((double x, double y) => x.CompareTo(y));
                for(int i = 0 ; i < TrgPos.Count ; i++)
                {
                    TrgPos[i] += _dStartPos ;
                }
            }
            else
            {
                TrgPos.Sort((double x, double y) => y.CompareTo(x));
                for(int i = 0 ; i < TrgPos.Count ; i++)
                {
                    TrgPos[i] += _dStartPos ;
                }
            }

            return TrgPos ;
        }

        public bool CycleWait()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    Trace(sTemp);
                    return true;

                case 10:
                    IO_SetY(yi.RAIL_FeedingAC2 , false);

                    MoveMotr(mi.HEAD_XCvr1,pv.HEAD_XCvr1Work );
                    MoveMotr(mi.HEAD_XCvr2,pv.HEAD_XCvr2Work );
                    MoveMotr(mi.HEAD_XCvr3,pv.HEAD_XCvr3Work );

                    MoveMotr(mi.HEAD_YVisn,pv.HEAD_YVisnWait);
                    MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnWait);

                    MoveCyl(ci.RAIL_Vsn1StprUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2StprUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);

                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd);

                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.HEAD_XCvr1,pv.HEAD_XCvr1Work )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XCvr2,pv.HEAD_XCvr2Work )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XCvr3,pv.HEAD_XCvr3Work )) return false ;

                    if(!MT_GetStopPos(mi.HEAD_YVisn,pv.HEAD_YVisnWait )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XVisn,pv.HEAD_XVisnWait )) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1StprUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2StprUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3StprUpDn ,fb.Fwd)) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd)) return false ;
                    
                    MoveCyl(ci.RAIL_Vsn1SttnUpDn ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2SttnUpDn ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3SttnUpDn ,fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn ,fb.Bwd)) return false ;

                    Step.iCycle=0;
                    return true ;
            }
        }

        public bool CycleWorkStart()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                IO_SetY(yi.RAIL_FeedingAC2 , false);
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    Trace(sTemp);
                    return true;

                case 10:
                    iManWorkC = 0 ;
                    iManWorkR = 0 ;
                    bManRight = true;

                    IO_SetY(yi.RAIL_FeedingAC2 , false);

                    MoveMotr(mi.HEAD_XCvr1,pv.HEAD_XCvr1Work );
                    MoveMotr(mi.HEAD_XCvr2,pv.HEAD_XCvr2Work );
                    MoveMotr(mi.HEAD_XCvr3,pv.HEAD_XCvr3Work );



                    MoveMotr(mi.HEAD_YVisn,pv.HEAD_YVisnWorkStart);
                    if(OM.DevInfo.bVsL_NotUse)
                    {
                        MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnRWorkStart);
                    }
                    else
                    {
                        MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnWorkStart);
                    }

                    MoveCyl(ci.RAIL_Vsn1StprUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2StprUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);

                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd);

                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.HEAD_XCvr1,pv.HEAD_XCvr1Work )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XCvr2,pv.HEAD_XCvr2Work )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XCvr3,pv.HEAD_XCvr3Work )) return false ;

                    if(!MT_GetStopPos(mi.HEAD_YVisn,pv.HEAD_YVisnWorkStart )) return false ;
                    if(OM.DevInfo.bVsL_NotUse)
                    {
                        if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_XVisnRWorkStart))return false ;
                    }
                    else
                    {
                        if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_XVisnWorkStart))return false ;
                    }


                    if(!CL_Complete(ci.RAIL_Vsn1StprUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2StprUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3StprUpDn ,fb.Fwd)) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd)) return false ;
                    
                    MoveCyl(ci.RAIL_Vsn1SttnUpDn ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2SttnUpDn ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3SttnUpDn ,fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn ,fb.Bwd)) return false ;
                    IO_SetY(yi.RAIL_FeedingAC2 , true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!m_tmDelay.OnDelay(true,1000)) return false;
                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Fwd);

                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Fwd)) return false ;
                    MoveCyl(ci.RAIL_Vsn1SttnUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2SttnUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3SttnUpDn ,fb.Fwd);

                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn ,fb.Fwd)) return false ;
                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd);

                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd)) return false ;

                    IO_SetY(yi.RAIL_FeedingAC2 , false);
                    //DM.ARAY[ri.VSN1].SetStat(cs.Unknown);

                    Step.iCycle=0;
                    return true ;
            }
        }

        int iManWorkR = 0 ;
        int iManWorkC = 0 ;
        bool bManRight = true ;

        public bool CycleNext()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                IO_SetY(yi.RAIL_FeedingAC2 , false);
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    Trace(sTemp);
                    return true;

                case 10:

                    List<double> TrgPosY = GetTrgPosY(true , PM.GetValue(mi.HEAD_YVisn , pv.HEAD_YVisnWorkStart));
                    List<double> TrgPosX ; 
                    if( OM.DevInfo.bVsL_NotUse)
                    {
                        TrgPosX = GetTrgPosX(true , PM.GetValue(mi.HEAD_XVisn , pv.HEAD_XVisnRWorkStart) , 0 , !OM.DevInfo.bVsL_NotUse && !OM.DevInfo.bVsR_NotUse);
                    }
                    else
                    {
                        TrgPosX = GetTrgPosX(true , PM.GetValue(mi.HEAD_XVisn , pv.HEAD_XVisnWorkStart) , 0 , !OM.DevInfo.bVsL_NotUse && !OM.DevInfo.bVsR_NotUse);
                    }
                    
                    

                    if(bManRight)
                    {
                        iManWorkC++;
                        if(iManWorkC >= TrgPosX.Count)
                        {
                            iManWorkC=TrgPosX.Count-1;
                            iManWorkR++;
                            bManRight = false ;
                        }                        
                    }
                    else
                    {
                        iManWorkC--;
                        if(iManWorkC < 0)
                        {
                            iManWorkC=0;
                            iManWorkR++;
                            bManRight = true ;
                        }                        
                    }
                    if(iManWorkR >= TrgPosY.Count)
                    {
                        iManWorkC = 0 ;
                        iManWorkR = 0 ;
                        bManRight = true ;
                    }

                    MoveMotr(mi.HEAD_XCvr1 , pv.HEAD_XCvr1Work);
                    MoveMotr(mi.HEAD_XCvr2 , pv.HEAD_XCvr2Work);
                    MoveMotr(mi.HEAD_XCvr3 , pv.HEAD_XCvr3Work);


                    MT_GoAbsMan(mi.HEAD_XVisn , TrgPosX[iManWorkC]); 
                    MT_GoAbsMan(mi.HEAD_YVisn , TrgPosY[iManWorkR]); 

                    Step.iCycle++;
                    return false;

                    

                case 11:
                    if(!MT_GetStopPos(mi.HEAD_XCvr1,pv.HEAD_XCvr1Work )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XCvr2,pv.HEAD_XCvr2Work )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XCvr3,pv.HEAD_XCvr3Work )) return false ;

                    if(!MT_GetStop(mi.HEAD_YVisn)) return false ;
                    if(!MT_GetStop(mi.HEAD_XVisn)) return false ;

                    Step.iCycle=0;
                    return true ;
            }
        }

        double dWorkLeftXPos ;
        double dWorkYPos ;
        double dWorkRightXPos ;
        bool   bRight = true ;

        bool bVsn1Strip = false;
        bool bVsn2Strip = false;
        bool bVsn3Strip = false;
        public bool CycleInsp()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                MT_ResetTrgPos(mi.HEAD_XVisn);
                IO_SetY(yi.RAIL_FeedingAC2,false);
                ER_SetErr(ei.ETC_CycleTO, "Check Vision Device " + sTemp);
                Trace(sTemp);

                goto ReturnTrue;
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

            int c,r;
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    Trace(sTemp);
                    goto ReturnTrue;

                //바인딩
                case 10:
                    if(OM.CmnOptn.bIgnrVisn)
                    {
                        DM.ARAY[ri.RLT1].SetStat(cs.Good);
                        if(!DM.ARAY[ri.VSN1].CheckAllStat(cs.None))DM.ARAY[ri.VSN1].SetStat(cs.Work);

                        DM.ARAY[ri.RLT2].SetStat(cs.Good);
                        if(!DM.ARAY[ri.VSN2].CheckAllStat(cs.None))DM.ARAY[ri.VSN2].SetStat(cs.Work);

                        DM.ARAY[ri.RLT3].SetStat(cs.Good);
                        if(!DM.ARAY[ri.VSN3].CheckAllStat(cs.None))DM.ARAY[ri.VSN3].SetStat(cs.Work);

                        Step.iCycle = 0;
                        goto ReturnTrue;
                    }
                    
                    
                    if(!OM.VsSkip(vi.Vs1L) && !VisnComs[(int)vi.Vs1L].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 1 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs1R) && !VisnComs[(int)vi.Vs1R].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 2 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs2L) && !VisnComs[(int)vi.Vs2L].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 3 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs2R) && !VisnComs[(int)vi.Vs2R].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 4 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs3L) && !VisnComs[(int)vi.Vs3L].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 5 Not Ready"); return true ;}
                    if(!OM.VsSkip(vi.Vs3R) && !VisnComs[(int)vi.Vs3R].GetIOReady()){ ER_SetErr(ei.VSN_ComErr , "Vision 6 Not Ready"); return true ;}

                    
                    MoveMotr(mi.HEAD_XCvr1,pv.HEAD_XCvr1Work );
                    MoveMotr(mi.HEAD_XCvr2,pv.HEAD_XCvr2Work );
                    MoveMotr(mi.HEAD_XCvr3,pv.HEAD_XCvr3Work );

                    //미리보내놓음. 확인은 나중에 함.
                    MoveMotr(mi.HEAD_YVisn,pv.HEAD_YVisnWorkStart);
                    if(OM.DevInfo.bVsL_NotUse)MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnRWorkStart,-5);
                    else                      MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnWorkStart ,-5);

                    MoveCyl(ci.RAIL_Vsn1StprUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2StprUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);

                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.HEAD_XCvr1,pv.HEAD_XCvr1Work )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XCvr2,pv.HEAD_XCvr2Work )) return false ;
                    if(!MT_GetStopPos(mi.HEAD_XCvr3,pv.HEAD_XCvr3Work )) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1StprUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2StprUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3StprUpDn ,fb.Fwd)) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd)) return false ;

                    IO_SetY(yi.RAIL_FeedingAC2 , true);
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!m_tmDelay.OnDelay(300)) return false ;

                    //Manual
                    if(MM.Working())
                    {
                        bool b1 = DM.ARAY[ri.VSN1].CheckAllStat(cs.None);
                        bool b2 = DM.ARAY[ri.VSN2].CheckAllStat(cs.None);
                        bool b3 = DM.ARAY[ri.VSN3].CheckAllStat(cs.None);
                        bVsn1Strip = b1 && IO_GetX(xi.RAIL_Vsn1Detect) ;
                        bVsn2Strip = b2 && IO_GetX(xi.RAIL_Vsn2Detect) ;
                        bVsn3Strip = b3 && IO_GetX(xi.RAIL_Vsn3Detect) ;

                        if(bVsn1Strip) DM.ARAY[ri.VSN1].SetStat(cs.Unknown);
                        if(bVsn2Strip) DM.ARAY[ri.VSN2].SetStat(cs.Unknown);
                        if(bVsn3Strip) DM.ARAY[ri.VSN3].SetStat(cs.Unknown);

                        if((!b1&&!IO_GetX(xi.RAIL_Vsn1Detect)) && (!b2 && !IO_GetX(xi.RAIL_Vsn2Detect)) && (!b3 && !IO_GetX(xi.RAIL_Vsn3Detect)))
                        {
                            IO_SetY(yi.RAIL_FeedingAC2 , false);
                            ER_SetErr(ei.PKG_Dispr  ,"Vision Zone has no strips") ;
                            return true;
                        }

                        if(!IO_GetX(xi.RAIL_Vsn1Detect) && !IO_GetX(xi.RAIL_Vsn2Detect) && !IO_GetX(xi.RAIL_Vsn3Detect))
                        {
                            IO_SetY(yi.RAIL_FeedingAC2 , false);
                            ER_SetErr(ei.PKG_Dispr  ,"Vision Zone has no strips") ;
                            return true;
                        }
                    }

                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Fwd);

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Fwd)) return false ;

                    MoveCyl(ci.RAIL_Vsn1SttnUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2SttnUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3SttnUpDn ,fb.Fwd);


                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn ,fb.Fwd)) return false ;

                    IO_SetY(yi.RAIL_FeedingAC2 , false);
                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd)) return false ;
                    //여기까지 바인딩 끝~~~~
                    Step.iCycle=20;
                    return false ;

                //여기부터 검사.========================================================================================================================================
                //검사 초기화.
                case 20:                          
                    //OM .StripPos.SetPara(PosPara);
                    //혹시 찌꺼기 자제 있어도 어차피 비전에서 1스트립 트리거 다들어와야 되기때문에 강제 마스킹 하고 시작함.
                    if(!DM.ARAY[ri.VSN1].CheckAllStat(cs.None))DM.ARAY[ri.VSN1].SetStat(cs.Unknown);
                    if(!DM.ARAY[ri.VSN2].CheckAllStat(cs.None))DM.ARAY[ri.VSN2].SetStat(cs.Unknown);
                    if(!DM.ARAY[ri.VSN3].CheckAllStat(cs.None))DM.ARAY[ri.VSN3].SetStat(cs.Unknown);

                    //초기화.
                    DM.ARAY[ri.RLT1].SetStat(cs.Good);
                    DM.ARAY[ri.RLT2].SetStat(cs.Good);
                    DM.ARAY[ri.RLT3].SetStat(cs.Good);

                    //처음에 반전하면서 시작해서 일단 펄스로 놓으나 스캔시작전에 한번 반전 한다.
                    bRight = false ;

                    FindChip(out c,out r,cs.Unknown);

                    int iTrgColCnt = 0;
                    if (!OM.DevInfo.bVsL_NotUse && !OM.DevInfo.bVsR_NotUse) //둘다 사용시.
                    {
                        iTrgColCnt = OM.DevInfo.iColCnt / 2  + ((OM.DevInfo.iColCnt % 2 != 0) ? 1 : 0) ; //반나눠서 안떨어지면 한개 추가.
                    }
                    else //한쪽만 사용시.
                    {
                        iTrgColCnt = OM.DevInfo.iColCnt ;
                    }

                    //비전 검사 범위를 감안하여 실제 트리거 필요한 왼쪽위 기준자제의 위치를 확인.
                    //검사갯수로 나눈다음 나머지가 있으면 한샷을 더하게 한다.
                    //  21           (42         / 2                     ) * 2 ;
                    int iTrgEndCol = (iTrgColCnt / OM.DevInfo.iColInspCnt) * OM.DevInfo.iColInspCnt ;
                    if(iTrgColCnt % OM.DevInfo.iColInspCnt != 0)iTrgEndCol+=OM.DevInfo.iColInspCnt;
                    //한샷에서 실제 필요한 포지션은 왼쪽 위에 있는 포지션이여서 검사영역중에 왼쪽포지션으로 가게 한다.
                    iTrgEndCol -= (OM.DevInfo.iColInspCnt-1);
                    
                    
                    //왼쪽 작업 포지션 
                    
                    //iWorkR = DM.ARAY[ri.VSN1].FindFrstRow(cs.Unknown); //어차피 VS 1,2,3 다 똑같아서 1번으로 함.
                    iWorkR = r ;//DM.ARAY[ri.VSN1].FindFrstRow(cs.Unknown); //어차피 VS 1,2,3 다 똑같아서 1번으로 함.
                    if(iWorkR==-1)
                    {
                        //ER_SetErr(ei.PKG_Dispr, "There is no Strips!");
                        //goto ReturnTrue;

                    }

                    if (!OM .StripPos.GetPos(0, iWorkR, 4, out dWorkLeftXPos, out dWorkYPos))
                    {
                        ER_SetErr(ei.MTR_PosCal, OM .StripPos.Error);
                        goto ReturnTrue;
                    }
                    //Y축데이터때문에 OM.StripPos.GetPos(0, iWorkR, 4, out dWorkLeftXPos, out dWorkYPos))를
                    //21번스텝에서 한번 더 하는데 그거 땜에 dWorkLeftXPos를 여기서 셋팅해도 계속 0으로 들어옴
                    //그래서 밑에서 dWorkLeftXPos -= 5; 한번 더 하는데 여기꺼 지워야 할지 판단이 안되서 내비둠. 진섭
                    dWorkLeftXPos -= 5;//가속오프셑 3mm로 박음.
                    //오른쪽 작업 포지션.
                    //if (!OM .StripPos.GetPos(iTrgEndCol - 1, iWorkR , 4, out dWorkRightXPos , out dWorkYPos))

                    if (!OM .StripPos.GetPos(iTrgEndCol -1, iWorkR , 4, out dWorkRightXPos , out dWorkYPos))
                    {
                        ER_SetErr(ei.MTR_PosCal, OM .StripPos.Error);
                        goto ReturnTrue;
                    }
                    dWorkRightXPos += 5; //감속 오프셑.

                    Step.iCycle++;
                    return false ;


                //밑에서 씀.
                case 21:
                    FindChip(out c,out r,cs.Unknown);
                    bRight = !bRight ;
                    iWorkR = r;//DM.ARAY[ri.VSN1].FindFrstRow(cs.Unknown); //어차피 VS 1,2,3 다 똑같아서 1번으로 함.
                    if (!OM .StripPos.GetPos(0, iWorkR, 4, out dWorkLeftXPos, out dWorkYPos))//Y축 데이터 뽑기위해.
                    {
                        ER_SetErr(ei.MTR_PosCal, OM .StripPos.Error);
                        goto ReturnTrue;
                    }
                    //위에서 셋팅하는데 Y축 데이터 뽑을때 다시 한번 타면서 dWorkLeftXPos가 0으로 다시 셋팅되서
                    //여기서 한번 더 셋팅해줌. 진섭
                    dWorkLeftXPos -= 5;//가속오프셑 3mm로 박음.

                    //Y축 보내고.
                    MoveMotr(mi.HEAD_YVisn,pv.HEAD_YVisnWorkStart , dWorkYPos );
                    if(OM.DevInfo.bVsL_NotUse)MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnRWorkStart , bRight ? dWorkLeftXPos : dWorkRightXPos);
                    else                      MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnWorkStart  , bRight ? dWorkLeftXPos : dWorkRightXPos);

                    Step.iCycle++;
                    return false;

                case 22:
                    if(!MT_GetStopInpos(mi.HEAD_XVisn)) return false ; 
                    if(!MT_GetStopInpos(mi.HEAD_YVisn)) return false ;


                    //카메라 한쪽이 고장났을때.
                    //오른쪽이 고장났을때는 그냥 라이트스킵만 한고 돌리면 되지만.
                    //왼쪽이 고장났을때는 오른쪽카메라 띠어다 왼쪽으로 교체 하던가.
                    //오른쪽카메라 기준으로 워크스타트 위치를 변경해야함. 자제가 작아서 못가면 카메라를 옮겨 달아야 함.
                    List<double> lsTrgPos ;
                    if(OM.DevInfo.bVsL_NotUse)lsTrgPos = GetTrgPosX(bRight , PM.GetValue(mi.HEAD_XVisn , pv.HEAD_XVisnRWorkStart) , OM.CmnOptn.dTrgOfs ,!OM.DevInfo.bVsL_NotUse && !OM.DevInfo.bVsR_NotUse);
                    else                      lsTrgPos = GetTrgPosX(bRight , PM.GetValue(mi.HEAD_XVisn , pv.HEAD_XVisnWorkStart ) , OM.CmnOptn.dTrgOfs ,!OM.DevInfo.bVsL_NotUse && !OM.DevInfo.bVsR_NotUse);
                    MT_SetTrgPos(mi.HEAD_XVisn , lsTrgPos.ToArray() , 1000 , true , true );

                    if(OM.DevInfo.bVsL_NotUse)MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnRWorkStart , bRight ? dWorkRightXPos : dWorkLeftXPos);
                    else                      MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnWorkStart  , bRight ? dWorkRightXPos : dWorkLeftXPos);
                    Step.iCycle++;
                    return false;

                //검사 한사이클 끝.
                case 23:
                    if(!MT_GetStopInpos(mi.HEAD_XVisn)) return false ; 
                    //검사 한줄 마스킹 
                    double dPreRowPos = 0.0 ;
                    for(int iR = 0 ; iR < OM.DevInfo.iRowInspCnt ; iR++)
                    {
                        if(iWorkR + iR >= OM.DevInfo.iRowCnt) break ;

                        OM.StripPos.GetPos(0,iWorkR + iR , 4 ,out double dPosX , out double dPosY);
                        double dGap=dPosY - dPreRowPos ;
                        if(iR==0 || Math.Round(dGap*1000) == (int)(OM.DevInfo.dRowPitch*1000))
                        { 
                            if(!DM.ARAY[ri.VSN1].CheckAllStat(cs.None))DM.ARAY[ri.VSN1].RangeSetStat(0,iWorkR + iR,OM.DevInfo.iColCnt-1 , iWorkR + iR , cs.Wait);
                            if(!DM.ARAY[ri.VSN2].CheckAllStat(cs.None))DM.ARAY[ri.VSN2].RangeSetStat(0,iWorkR + iR,OM.DevInfo.iColCnt-1 , iWorkR + iR , cs.Wait);
                            if(!DM.ARAY[ri.VSN3].CheckAllStat(cs.None))DM.ARAY[ri.VSN3].RangeSetStat(0,iWorkR + iR,OM.DevInfo.iColCnt-1 , iWorkR + iR , cs.Wait);
                        }
                        else
                        {
                            break ;
                        }

                        dPreRowPos = dPosY ;
                    }
                    FindChip(out c,out r,cs.Unknown);
                    //if(DM.ARAY[ri.VSN1].GetCntStat(cs.Unknown)>0)
                    if(r>0)
                    {
                        Step.iCycle = 21 ;
                        return false ;
                    }

                    //미리보내놓음.
                    MoveCyl(ci.RAIL_Vsn1SttnUpDn ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2SttnUpDn ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3SttnUpDn, fb.Bwd);

                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd);

                    //미리보내놓음. 확인은 나중에 함.
                    MoveMotr(mi.HEAD_YVisn,pv.HEAD_YVisnWorkStart);
                    if(OM.DevInfo.bVsL_NotUse)MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnRWorkStart,-5);
                    else                      MoveMotr(mi.HEAD_XVisn,pv.HEAD_XVisnWorkStart ,-5);

                    Step.iCycle++;
                    return false ;

                case 24:
                    if(!OM.VsSkip(vi.Vs1L) && !VisnComs[(int)vi.Vs1L].GetIOEnd())return false ;
                    if(!OM.VsSkip(vi.Vs1R) && !VisnComs[(int)vi.Vs1R].GetIOEnd())return false ;
                    if(!OM.VsSkip(vi.Vs2L) && !VisnComs[(int)vi.Vs2L].GetIOEnd())return false ;
                    if(!OM.VsSkip(vi.Vs2R) && !VisnComs[(int)vi.Vs2R].GetIOEnd())return false ;
                    if(!OM.VsSkip(vi.Vs3L) && !VisnComs[(int)vi.Vs3L].GetIOEnd())return false ;
                    if(!OM.VsSkip(vi.Vs3R) && !VisnComs[(int)vi.Vs3R].GetIOEnd())return false ;

                    if(!OM.VsSkip(vi.Vs1L))
                    {
                        if(!VisnComs[(int)vi.Vs1L].ReadResult(DM.ARAY[ri.RLT1]))
                        {
                            ER_SetErr(ei.VSN_ComErr, VisnComs[(int)vi.Vs1L].GetErrMsg());
                            goto ReturnTrue;
                        }
                    }
                    if(!OM.VsSkip(vi.Vs1R))
                    {
                        if(!VisnComs[(int)vi.Vs1R].ReadResult(DM.ARAY[ri.RLT1]))
                        {
                            ER_SetErr(ei.VSN_ComErr, VisnComs[(int)vi.Vs1R].GetErrMsg());
                            goto ReturnTrue;
                        }
                    }
                    if(!DM.ARAY[ri.VSN1].CheckAllStat(cs.None))DM.ARAY[ri.VSN1].SetStat(cs.Work);

                    


                    if(!OM.VsSkip(vi.Vs2L))
                    {
                        if(!VisnComs[(int)vi.Vs2L].ReadResult(DM.ARAY[ri.RLT2]))
                        {
                            ER_SetErr(ei.VSN_ComErr, VisnComs[(int)vi.Vs2L].GetErrMsg());
                            goto ReturnTrue;
                        }
                    }
                    if(!OM.VsSkip(vi.Vs2R))
                    {
                        if(!VisnComs[(int)vi.Vs2R].ReadResult(DM.ARAY[ri.RLT2]))
                        {
                            ER_SetErr(ei.VSN_ComErr, VisnComs[(int)vi.Vs2R].GetErrMsg());
                            goto ReturnTrue;
                        }
                    }
                    if(!DM.ARAY[ri.VSN2].CheckAllStat(cs.None))DM.ARAY[ri.VSN2].SetStat(cs.Work);


                    if(!OM.VsSkip(vi.Vs3L))
                    {
                        if(!VisnComs[(int)vi.Vs3L].ReadResult(DM.ARAY[ri.RLT3]))
                        {
                            ER_SetErr(ei.VSN_ComErr, VisnComs[(int)vi.Vs3L].GetErrMsg());
                            goto ReturnTrue;
                        }
                    }
                    if(!OM.VsSkip(vi.Vs3R))
                    {
                        if(!VisnComs[(int)vi.Vs3R].ReadResult(DM.ARAY[ri.RLT3]))
                        {
                            ER_SetErr(ei.VSN_ComErr, VisnComs[(int)vi.Vs3R].GetErrMsg());
                            goto ReturnTrue;
                        }
                    }
                    if(!DM.ARAY[ri.VSN3].CheckAllStat(cs.None))DM.ARAY[ri.VSN3].SetStat(cs.Work);

                    bool bError = false ;
                    if (!OM.VsSkip(vi.Vs1L) || !OM.VsSkip(vi.Vs1R))
                    {
                        if (OM.CmnOptn.iVsLim0 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt0) > OM.CmnOptn.iVsLim0) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName0 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt0).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim1 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt1) > OM.CmnOptn.iVsLim1) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName1 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt1).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim2 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt2) > OM.CmnOptn.iVsLim2) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName2 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt2).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim3 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt3) > OM.CmnOptn.iVsLim3) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName3 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt3).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim4 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt4) > OM.CmnOptn.iVsLim4) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName4 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt4).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim5 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt5) > OM.CmnOptn.iVsLim5) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName5 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt5).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim6 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt6) > OM.CmnOptn.iVsLim6) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName6 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt6).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim7 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt7) > OM.CmnOptn.iVsLim7) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName7 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt7).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim8 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt8) > OM.CmnOptn.iVsLim8) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName8 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt8).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim9 != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt9) > OM.CmnOptn.iVsLim9) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltName9 + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.Rslt9).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimA != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltA) > OM.CmnOptn.iVsLimA) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameA + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltA).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimB != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltB) > OM.CmnOptn.iVsLimB) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameB + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltB).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimC != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltC) > OM.CmnOptn.iVsLimC) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameC + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltC).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimD != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltD) > OM.CmnOptn.iVsLimD) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameD + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltD).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimE != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltE) > OM.CmnOptn.iVsLimE) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameE + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltE).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimF != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltF) > OM.CmnOptn.iVsLimF) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameF + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltF).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimG != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltG) > OM.CmnOptn.iVsLimG) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameG + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltG).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimH != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltH) > OM.CmnOptn.iVsLimH) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameH + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltH).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimI != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltI) > OM.CmnOptn.iVsLimI) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameI + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltI).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimJ != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltJ) > OM.CmnOptn.iVsLimJ) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameJ + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltJ).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimK != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltK) > OM.CmnOptn.iVsLimK) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameK + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltK).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimL != 0 && DM.ARAY[ri.RLT1].GetCntStat(cs.RsltL) > OM.CmnOptn.iVsLimL) {ER_SetErr(ei.VSN_SameFailCnt, "VSN1" + OM.CmnOptn.sRsltNameL + " Failed over " + DM.ARAY[ri.RLT1].GetCntStat(cs.RsltL).ToString());bError = true;}
                    }                                                                                                                                                                                                                                 
                    if (!OM.VsSkip(vi.Vs2L) || !OM.VsSkip(vi.Vs2R))                                                                                                                                                                                   
                    {                                                                                                                                                                                                                                 
                        if (OM.CmnOptn.iVsLim0 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt0) > OM.CmnOptn.iVsLim0) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName0 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt0).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim1 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt1) > OM.CmnOptn.iVsLim1) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName1 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt1).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim2 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt2) > OM.CmnOptn.iVsLim2) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName2 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt2).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim3 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt3) > OM.CmnOptn.iVsLim3) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName3 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt3).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim4 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt4) > OM.CmnOptn.iVsLim4) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName4 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt4).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim5 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt5) > OM.CmnOptn.iVsLim5) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName5 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt5).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim6 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt6) > OM.CmnOptn.iVsLim6) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName6 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt6).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim7 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt7) > OM.CmnOptn.iVsLim7) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName7 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt7).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim8 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt8) > OM.CmnOptn.iVsLim8) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName8 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt8).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim9 != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt9) > OM.CmnOptn.iVsLim9) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName9 + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.Rslt9).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimA != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltA) > OM.CmnOptn.iVsLimA) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameA + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltA).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimB != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltB) > OM.CmnOptn.iVsLimB) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameB + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltB).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimC != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltC) > OM.CmnOptn.iVsLimC) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameC + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltC).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimD != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltD) > OM.CmnOptn.iVsLimD) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameD + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltD).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimE != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltE) > OM.CmnOptn.iVsLimE) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameE + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltE).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimF != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltF) > OM.CmnOptn.iVsLimF) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameF + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltF).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimG != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltG) > OM.CmnOptn.iVsLimG) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameG + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltG).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimH != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltH) > OM.CmnOptn.iVsLimH) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameH + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltH).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimI != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltI) > OM.CmnOptn.iVsLimI) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameI + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltI).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimJ != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltJ) > OM.CmnOptn.iVsLimJ) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameJ + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltJ).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimK != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltK) > OM.CmnOptn.iVsLimK) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameK + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltK).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimL != 0 && DM.ARAY[ri.RLT2].GetCntStat(cs.RsltL) > OM.CmnOptn.iVsLimL) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameL + " Failed over " + DM.ARAY[ri.RLT2].GetCntStat(cs.RsltL).ToString());bError = true;}
                    }                                                                                                                                                                                                                             
                    if (!OM.VsSkip(vi.Vs3L) || !OM.VsSkip(vi.Vs3R))                                                                                                                                                                               
                    {                                                                                                                                                                                                                             
                        if (OM.CmnOptn.iVsLim0 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt0) > OM.CmnOptn.iVsLim0) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName0 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt0).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim1 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt1) > OM.CmnOptn.iVsLim1) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName1 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt1).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim2 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt2) > OM.CmnOptn.iVsLim2) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName2 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt2).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim3 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt3) > OM.CmnOptn.iVsLim3) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName3 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt3).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim4 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt4) > OM.CmnOptn.iVsLim4) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName4 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt4).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim5 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt5) > OM.CmnOptn.iVsLim5) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName5 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt5).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim6 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt6) > OM.CmnOptn.iVsLim6) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName6 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt6).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim7 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt7) > OM.CmnOptn.iVsLim7) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName7 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt7).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim8 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt8) > OM.CmnOptn.iVsLim8) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName8 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt8).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLim9 != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt9) > OM.CmnOptn.iVsLim9) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltName9 + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.Rslt9).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimA != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltA) > OM.CmnOptn.iVsLimA) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameA + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltA).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimB != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltB) > OM.CmnOptn.iVsLimB) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameB + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltB).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimC != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltC) > OM.CmnOptn.iVsLimC) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameC + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltC).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimD != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltD) > OM.CmnOptn.iVsLimD) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameD + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltD).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimE != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltE) > OM.CmnOptn.iVsLimE) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameE + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltE).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimF != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltF) > OM.CmnOptn.iVsLimF) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameF + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltF).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimG != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltG) > OM.CmnOptn.iVsLimG) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameG + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltG).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimH != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltH) > OM.CmnOptn.iVsLimH) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameH + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltH).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimI != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltI) > OM.CmnOptn.iVsLimI) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameI + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltI).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimJ != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltJ) > OM.CmnOptn.iVsLimJ) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameJ + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltJ).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimK != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltK) > OM.CmnOptn.iVsLimK) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameK + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltK).ToString());bError = true;}
                        if (OM.CmnOptn.iVsLimL != 0 && DM.ARAY[ri.RLT3].GetCntStat(cs.RsltL) > OM.CmnOptn.iVsLimL) {ER_SetErr(ei.VSN_SameFailCnt, "VSN2" + OM.CmnOptn.sRsltNameL + " Failed over " + DM.ARAY[ri.RLT3].GetCntStat(cs.RsltL).ToString());bError = true;}
                    }

                    if(bError)
                    {
                        if(!DM.ARAY[ri.VSN1].CheckAllStat(cs.None))DM.ARAY[ri.VSN1].SetStat(cs.Unknown);
                        if(!DM.ARAY[ri.VSN2].CheckAllStat(cs.None))DM.ARAY[ri.VSN2].SetStat(cs.Unknown);
                        if(!DM.ARAY[ri.VSN3].CheckAllStat(cs.None))DM.ARAY[ri.VSN3].SetStat(cs.Unknown);
                    }
                    //에러 마스킹 처리.
                    Step.iCycle++;
                    return false ;

                case 25:
                    if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn ,fb.Bwd)) return false ;

                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd)) return false ;
                    //if(OM.DevInfo.bVsL_NotUse)
                    //{
                    //    if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_XVisnRWorkStart))return false ;
                    //}
                    //else
                    //{
                    //    if(!MT_GetStopPos(mi.HEAD_XVisn , pv.HEAD_XVisnWorkStart))return false ;
                    //}

                    Step.iCycle = 0;
                    goto ReturnTrue;
            }

            ReturnTrue:
            if(MM.Working())
            {
                if(bVsn1Strip) DM.ARAY[ri.VSN1].SetStat(cs.None);
                if(bVsn2Strip) DM.ARAY[ri.VSN2].SetStat(cs.None);
                if(bVsn3Strip) DM.ARAY[ri.VSN3].SetStat(cs.None);

                MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd);
                MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd);
                MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd);

                MoveCyl(ci.RAIL_Vsn1SttnUpDn ,fb.Bwd);
                MoveCyl(ci.RAIL_Vsn2SttnUpDn ,fb.Bwd);
                MoveCyl(ci.RAIL_Vsn3SttnUpDn ,fb.Bwd);
            }
            return true;


        }

        public int GetChipLevel(cs _csStat)
        {

            switch (_csStat)
            {
                default       :return -1 ;
                case cs.Rslt0 :return OM.CmnOptn.iRsltLevel0 ;
                case cs.Rslt1 :return OM.CmnOptn.iRsltLevel1 ;
                case cs.Rslt2 :return OM.CmnOptn.iRsltLevel2 ;
                case cs.Rslt3 :return OM.CmnOptn.iRsltLevel3 ;
                case cs.Rslt4 :return OM.CmnOptn.iRsltLevel4 ;
                case cs.Rslt5 :return OM.CmnOptn.iRsltLevel5 ;
                case cs.Rslt6 :return OM.CmnOptn.iRsltLevel6 ;
                case cs.Rslt7 :return OM.CmnOptn.iRsltLevel7 ;
                case cs.Rslt8 :return OM.CmnOptn.iRsltLevel8 ;
                case cs.Rslt9 :return OM.CmnOptn.iRsltLevel9 ;
                case cs.RsltA :return OM.CmnOptn.iRsltLevelA ;
                case cs.RsltB :return OM.CmnOptn.iRsltLevelB ;
                case cs.RsltC :return OM.CmnOptn.iRsltLevelC ;
                case cs.RsltD :return OM.CmnOptn.iRsltLevelD ;
                case cs.RsltE :return OM.CmnOptn.iRsltLevelE ;
                case cs.RsltF :return OM.CmnOptn.iRsltLevelF ;
                case cs.RsltG :return OM.CmnOptn.iRsltLevelG ;
                case cs.RsltH :return OM.CmnOptn.iRsltLevelH ;
                case cs.RsltI :return OM.CmnOptn.iRsltLevelI ;
                case cs.RsltJ :return OM.CmnOptn.iRsltLevelJ ;
                case cs.RsltK :return OM.CmnOptn.iRsltLevelK ;
                case cs.RsltL :return OM.CmnOptn.iRsltLevelL ;
            }
        }

        public void ArrayMergeFail(CArray _Src , CArray _Dst)
        {
            for(int r = 0 ; r < OM.DevInfo.iRowCnt ; r++)
            {
                for(int c = 0 ; c < OM.DevInfo.iColCnt ; c++)
                {
                    if(GetChipLevel(_Src.GetStat(c,r)) > GetChipLevel(_Dst.GetStat(c,r)))
                    {
                        _Dst.SetStat(c,r , _Src.GetStat(c,r));
                    }
                }
            }
        }

        bool bCanMovePrb = false ;
        bool bCanMove1   = false ;
        bool bCanMove2   = false ;
        bool bCanMove3   = false ;

        //비전 세팅시에 자제 그냥 찔러 넣고 넥스트무브 메뉴얼 동작 하는 경우가 있는데
        //데이터가 없으면 안가기 때문에 센서만 보고 동작 하기 위해
        bool bManCanMovePrb = false ;
        bool bManCanMove1   = false ;
        bool bManCanMove2   = false ;
        bool bManCanMove3   = false ;

        public bool CycleMove()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                MT_ResetTrgPos(mi.HEAD_XVisn);
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                
                IO_SetY(yi.RAIL_FeedingAC1 , false);
                IO_SetY(yi.RAIL_FeedingAC2 , false);
                IO_SetY(yi.RAIL_FeedingAC3 , false);
                if(Step.iCycle == 20)
                {
                    if(bCanMove3  ) ER_SetErr(ei.RAIL_FeedingFail,"Vision3 zone to Mark zone feed fail");
                    if(bCanMove2  ) ER_SetErr(ei.RAIL_FeedingFail,"Vision2 zone to Vision3 zone feed fail");
                    if(bCanMove1  ) ER_SetErr(ei.RAIL_FeedingFail,"Vision1 zone to Vision2 zone feed fail");
                    if(bCanMovePrb) ER_SetErr(ei.RAIL_FeedingFail,"Buffer zone to Vision1 zone feed fail");
                }
                else
                {
                    ER_SetErr(ei.ETC_CycleTO, sTemp);
                }

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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    Trace(sTemp);
                    return true;

             
                case 10:   
                    if(MM.Working())//메뉴얼일땐 그냥 스트립 넣고 비전 테스트 하는 경우가 많음.
                    {
                        bCanMovePrb = !DM.ARAY[ri.PREB].CheckAllStat(cs.None) || IO_GetX(xi.PREB_StrpDetect);//오토런에서 이미 cs.Work인지 확인 하고 들어온 상황.
                        bCanMove1   = !DM.ARAY[ri.VSN1].CheckAllStat(cs.None) || IO_GetX(xi.RAIL_Vsn1Detect);
                        bCanMove2   = !DM.ARAY[ri.VSN2].CheckAllStat(cs.None) || IO_GetX(xi.RAIL_Vsn2Detect);
                        bCanMove3   = !DM.ARAY[ri.VSN3].CheckAllStat(cs.None) || IO_GetX(xi.RAIL_Vsn3Detect);

                        //bManCanMovePrb = DM.ARAY[ri.PREB].CheckAllStat(cs.None) && IO_GetX(xi.PREB_StrpDetect);
                        //bManCanMove1   = DM.ARAY[ri.VSN1].CheckAllStat(cs.None) && IO_GetX(xi.RAIL_Vsn1Detect);
                        //bManCanMove2   = DM.ARAY[ri.VSN2].CheckAllStat(cs.None) && IO_GetX(xi.RAIL_Vsn2Detect);
                        //bManCanMove3   = DM.ARAY[ri.VSN3].CheckAllStat(cs.None) && IO_GetX(xi.RAIL_Vsn3Detect);

                    }
                    else
                    {
                        bCanMovePrb = !DM.ARAY[ri.PREB].CheckAllStat(cs.None) ;//오토런에서 이미 cs.Work인지 확인 하고 들어온 상황.
                        bCanMove1   = !DM.ARAY[ri.VSN1].CheckAllStat(cs.None) ;
                        bCanMove2   = !DM.ARAY[ri.VSN2].CheckAllStat(cs.None) ;
                        bCanMove3   = !DM.ARAY[ri.VSN3].CheckAllStat(cs.None) ;//이미 오토런에서 포스트 버퍼 없거나 비전3번이 없는 경우로 되어 있음.

                    }
                    

                    MoveCyl(ci.RAIL_Vsn1StprUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn2StprUpDn ,fb.Fwd);
                    MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);
                    

                    MoveCyl(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd);
                    MoveCyl(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.RAIL_Vsn1StprUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2StprUpDn ,fb.Fwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3StprUpDn ,fb.Fwd)) return false ;
                    

                    if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw ,fb.Bwd)) return false ;
                    if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw ,fb.Bwd)) return false ;

                    IO_SetY(yi.RAIL_FeedingAC1 , true);
                    IO_SetY(yi.RAIL_FeedingAC2 , true);
                    IO_SetY(yi.RAIL_FeedingAC3 , true);

                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 12://여기부터 스토퍼 내리기 생각......  OM.EqpStat.sVsn1LastLot = DM.ARAY[ri.VSN1].LotNo ;
                    //if(!m_tmDelay.OnDelay(300)) return false ; //혹시나 싶어서 스토퍼에 밀착시키기위해 딜레이.

                    if(bCanMove3)
                    {
                        ArrayMergeFail(DM.ARAY[ri.RLT3] , DM.ARAY[ri.WRK3]);//결과값 합쳐서.
                        DM.ShiftData(ri.WRK3 , ri.PSTB);//포스트버퍼로 넘김.
                        
                        
                        DM.ARAY[ri.PSTB].ChangeStat(cs.Unknown , cs.Good);
                        DM.CopyData (ri.PSTB , ri.SPC );//SPC용으로 저장해놓음.
                        DM.ARAY[ri.SPC].Name = "SPC";

                        DM.ARAY[ri.VSN3].ClearMap();
                        
                        //이거 로더에서 랏오픈할때 함.
                        //OM.EqpStat.iPreRsltCnts = (int [])OM.EqpStat.iRsltCnts.Clone() ;
                        //OM.EqpStat.iRsltCnts.Initialize();
                        
                        
                        
                        MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Bwd);
                    }
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 13: 
                    if(!IO_GetX(xi.RAIL_Vsn3Detect)) MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);
                    if(bCanMove3 && !m_tmDelay.OnDelay(OM.DevInfo.iStprDnDelay)) return false ;

                    if(bCanMove2)
                    {
                        ArrayMergeFail(DM.ARAY[ri.RLT2] , DM.ARAY[ri.WRK2]);//결과값 합쳐서.
                        DM.ShiftData(ri.WRK2 , ri.WRK3);//포스트버퍼로 넘김.
                        DM.ARAY[ri.VSN3].SetStat(cs.Unknown);
                        DM.ARAY[ri.VSN3].LotNo=DM.ARAY[ri.WRK3].LotNo ;
                        DM.ARAY[ri.VSN3].ID   =DM.ARAY[ri.WRK3].ID    ;
                        DM.ARAY[ri.VSN2].ClearMap();
                        MoveCyl(ci.RAIL_Vsn2StprUpDn ,fb.Bwd);
                        //만약 새로운 랏이면 랏오픈.
                        if(DM.ARAY[ri.WRK3].LotNo != OM.EqpStat.sVsn3LastLot)
                        {
                            if(!OM.VsSkip(vi.Vs3L))VisnComs[(int)vi.Vs3L].SendCmd(VisnCom.vc.LotStart,DM.ARAY[ri.WRK3].LotNo);
                            if(!OM.VsSkip(vi.Vs3R))VisnComs[(int)vi.Vs3R].SendCmd(VisnCom.vc.LotStart,DM.ARAY[ri.WRK3].LotNo);
                            OM.EqpStat.sVsn3LastLot = DM.ARAY[ri.WRK3].LotNo ;
                        }
                    }
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 14: 
                    if(!IO_GetX(xi.RAIL_Vsn3Detect)) MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);
                    if(!IO_GetX(xi.RAIL_Vsn2Detect)) MoveCyl(ci.RAIL_Vsn2StprUpDn ,fb.Fwd);
                    if(bCanMove2 && !m_tmDelay.OnDelay(OM.DevInfo.iStprDnDelay)) return false ;

                    if(bCanMove1)
                    {
                        ArrayMergeFail(DM.ARAY[ri.RLT1] , DM.ARAY[ri.WRK1]);//결과값 합쳐서.
                        DM.ShiftData(ri.WRK1 , ri.WRK2);
                        DM.ARAY[ri.VSN2].SetStat(cs.Unknown);
                        DM.ARAY[ri.VSN2].LotNo=DM.ARAY[ri.WRK2].LotNo ;
                        DM.ARAY[ri.VSN2].ID   =DM.ARAY[ri.WRK2].ID    ;
                        DM.ARAY[ri.VSN1].ClearMap();
                        MoveCyl(ci.RAIL_Vsn1StprUpDn ,fb.Bwd);
                        //만약 새로운 랏이면 랏오픈.
                        if(DM.ARAY[ri.WRK2].LotNo != OM.EqpStat.sVsn2LastLot)
                        {
                            if(!OM.VsSkip(vi.Vs2L))VisnComs[(int)vi.Vs2L].SendCmd(VisnCom.vc.LotStart,DM.ARAY[ri.WRK2].LotNo);
                            if(!OM.VsSkip(vi.Vs2R))VisnComs[(int)vi.Vs2R].SendCmd(VisnCom.vc.LotStart,DM.ARAY[ri.WRK2].LotNo);
                            OM.EqpStat.sVsn2LastLot = DM.ARAY[ri.WRK2].LotNo ;
                        }
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!IO_GetX(xi.RAIL_Vsn3Detect)) MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);
                    if(!IO_GetX(xi.RAIL_Vsn2Detect)) MoveCyl(ci.RAIL_Vsn2StprUpDn ,fb.Fwd);
                    if(!IO_GetX(xi.RAIL_Vsn1Detect)) MoveCyl(ci.RAIL_Vsn1StprUpDn ,fb.Fwd);
                    if(bCanMove1 && !m_tmDelay.OnDelay(OM.DevInfo.iStprDnDelay)) return false ;

                    if(bCanMovePrb)
                    {
                        DM.ShiftData(ri.PREB , ri.WRK1);//포스트버퍼로 넘김.
                        DM.ARAY[ri.VSN1].SetStat(cs.Unknown);
                        DM.ARAY[ri.VSN1].LotNo=DM.ARAY[ri.WRK1].LotNo ;
                        DM.ARAY[ri.VSN1].ID   =DM.ARAY[ri.WRK1].ID    ;

                        SEQ.PREB.MoveCyl(ci.PREB_StprUpDn ,fb.Bwd);
                        //만약 새로운 랏이면 랏오픈.
                        if(DM.ARAY[ri.WRK1].LotNo != OM.EqpStat.sVsn1LastLot)
                        {
                            if(!OM.VsSkip(vi.Vs1L))VisnComs[(int)vi.Vs1L].SendCmd(VisnCom.vc.LotStart,DM.ARAY[ri.WRK1].LotNo);
                            if(!OM.VsSkip(vi.Vs1R))VisnComs[(int)vi.Vs1R].SendCmd(VisnCom.vc.LotStart,DM.ARAY[ri.WRK1].LotNo);
                            OM.EqpStat.sVsn1LastLot = DM.ARAY[ri.WRK1].LotNo ;
                        }
                    }

                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!IO_GetX(xi.RAIL_Vsn3Detect)) MoveCyl(ci.RAIL_Vsn3StprUpDn ,fb.Fwd);
                    if(!IO_GetX(xi.RAIL_Vsn2Detect)) MoveCyl(ci.RAIL_Vsn2StprUpDn ,fb.Fwd);
                    if(!IO_GetX(xi.RAIL_Vsn1Detect)) MoveCyl(ci.RAIL_Vsn1StprUpDn ,fb.Fwd);
                    if(!IO_GetX(xi.PREB_StrpDetect)) SEQ.PREB.MoveCyl(ci.PREB_StprUpDn     ,fb.Fwd);

                    //CL_GetCmd(ci.RAIL_Vsn3StprUpDn)==fb.Fwd 이걸 봐야 자제 빠져 나가고 다음꺼 들어와서 감지 된지 알수 있음.
                    if(bCanMove3   && CL_GetCmd(ci.RAIL_Vsn3StprUpDn)!=fb.Fwd)return false ;
                    if(bCanMove2   && CL_GetCmd(ci.RAIL_Vsn2StprUpDn)!=fb.Fwd)return false ;
                    if(bCanMove1   && CL_GetCmd(ci.RAIL_Vsn1StprUpDn)!=fb.Fwd)return false ;
                    if(bCanMovePrb && CL_GetCmd(ci.PREB_StprUpDn    )!=fb.Fwd)return false ;

                    if(!OM.VsSkip(vi.Vs1L)&&!VisnComs[(int)vi.Vs1L].EndCmd()) return false;
                    if(!OM.VsSkip(vi.Vs1R)&&!VisnComs[(int)vi.Vs1R].EndCmd()) return false;
                    if(!OM.VsSkip(vi.Vs2L)&&!VisnComs[(int)vi.Vs2L].EndCmd()) return false;
                    if(!OM.VsSkip(vi.Vs2R)&&!VisnComs[(int)vi.Vs2R].EndCmd()) return false;
                    if(!OM.VsSkip(vi.Vs3L)&&!VisnComs[(int)vi.Vs3L].EndCmd()) return false;
                    if(!OM.VsSkip(vi.Vs3R)&&!VisnComs[(int)vi.Vs3R].EndCmd()) return false;

                    Step.iCycle=20;
                    return false ;

                case 20:
                    if(bCanMove3   && !IO_GetX(xi.PSTB_MarkDetect)) return false ;
                    if(bCanMove2   && !IO_GetX(xi.RAIL_Vsn3Detect)) return false ;
                    if(bCanMove1   && !IO_GetX(xi.RAIL_Vsn2Detect)) return false ;
                    if(bCanMovePrb && !IO_GetX(xi.RAIL_Vsn1Detect)) return false ;
                      
                    IO_SetY(yi.RAIL_FeedingAC1 , false);
                    IO_SetY(yi.RAIL_FeedingAC2 , false);
                    IO_SetY(yi.RAIL_FeedingAC3 , false);

                    if (!DM.ARAY[(int)ri.PSTB].CheckAllStat(cs.None) && OM.CmnOptn.iVsLimT !=0)
                    {
                        int iTotalCnt = DM.ARAY[(int)ri.PSTB].GetCntStat(cs.Rslt0 ,
                                                                         cs.Rslt1 ,
                                                                         cs.Rslt2 ,
                                                                         cs.Rslt3 ,
                                                                         cs.Rslt4 ,
                                                                         cs.Rslt5 ,
                                                                         cs.Rslt6 ,
                                                                         cs.Rslt7 ,
                                                                         cs.Rslt8 ,
                                                                         cs.Rslt9 ,
                                                                         cs.RsltA ,
                                                                         cs.RsltB ,
                                                                         cs.RsltC ,
                                                                         cs.RsltD ,
                                                                         cs.RsltE ,
                                                                         cs.RsltF ,
                                                                         cs.RsltG ,
                                                                         cs.RsltH ,
                                                                         cs.RsltI ,
                                                                         cs.RsltJ ,
                                                                         cs.RsltK ,
                                                                         cs.RsltL );
                        if(iTotalCnt >= OM.CmnOptn.iVsLimT)
                        {
                            ER_SetErr(ei.VSN_TotalFailCnt , "Total Fail Count Over ("+iTotalCnt.ToString() + ") in PostBuffer Zone ");
                            return true ;
                        }
                    }

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;


                 if (_eActr == ci.RAIL_Vsn1AlignFwBw){ }
            else if (_eActr == ci.RAIL_Vsn2AlignFwBw){ }
            else if (_eActr == ci.RAIL_Vsn3AlignFwBw){ }
            else if (_eActr == ci.RAIL_Vsn1StprUpDn ){ }
            else if (_eActr == ci.RAIL_Vsn2StprUpDn ){ }
            else if (_eActr == ci.RAIL_Vsn3StprUpDn ){ }
            else if (_eActr == ci.RAIL_Vsn1SttnUpDn ){ }
            else if (_eActr == ci.RAIL_Vsn2SttnUpDn ){ }
            else if (_eActr == ci.RAIL_Vsn3SttnUpDn ){ }
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

            if (_eMotr == mi.HEAD_XVisn)
            {
            
            }
            else if (_eMotr == mi.HEAD_YVisn)
            {

            }
            else if (_eMotr == mi.HEAD_XCvr1)
            {

            }
            else if (_eMotr == mi.HEAD_XCvr2)
            {

            }
            else if (_eMotr == mi.HEAD_XCvr3)
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

            if (_eMotr == mi.HEAD_XVisn)
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
            else if (_eMotr == mi.HEAD_YVisn)
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
            else if (_eMotr == mi.HEAD_XCvr1)
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
            else if (_eMotr == mi.HEAD_XCvr2)
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
            else if (_eMotr == mi.HEAD_XCvr3)
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
        public bool MoveMotr(mi _eMotr , pv _ePstn ,  double _dOfsPos=0, bool _bLink = true)
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
            if (!MT_GetStop(mi.HEAD_XVisn)) return false;
            if (!MT_GetStop(mi.HEAD_YVisn)) return false;
            if (!MT_GetStop(mi.HEAD_XCvr1)) return false;
            if (!MT_GetStop(mi.HEAD_XCvr2)) return false;
            if (!MT_GetStop(mi.HEAD_XCvr3)) return false;

            if(!CL_Complete(ci.RAIL_Vsn1AlignFwBw)) return false;
            if(!CL_Complete(ci.RAIL_Vsn2AlignFwBw)) return false;
            if(!CL_Complete(ci.RAIL_Vsn3AlignFwBw)) return false;
            if(!CL_Complete(ci.RAIL_Vsn1StprUpDn )) return false;
            if(!CL_Complete(ci.RAIL_Vsn2StprUpDn )) return false;
            if(!CL_Complete(ci.RAIL_Vsn3StprUpDn )) return false;
            if(!CL_Complete(ci.RAIL_Vsn1SttnUpDn )) return false;
            if(!CL_Complete(ci.RAIL_Vsn2SttnUpDn )) return false;
            if(!CL_Complete(ci.RAIL_Vsn3SttnUpDn )) return false;

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
