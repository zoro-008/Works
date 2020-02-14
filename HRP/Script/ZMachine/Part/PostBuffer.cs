using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class PostBuffer : Part
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
            Wait           ,
            In             ,
            Work           ,
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

        public PostBuffer(int _iPartId = 0)
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
                    if(!IO_GetX(xi.PSTB_PkgDetect1) && !IO_GetX(xi.PSTB_PkgDetect2))  MoveCyl(ci.PSTB_MarkStprUpDn  ,fb.Fwd);
                    //MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    MoveCyl(ci.PSTB_PusherFwBw    ,fb.Bwd);
                    MoveCyl(ci.PSTB_MarkAlignFWBw ,fb.Bwd);
                    IO_SetY(yi.RAIL_FeedingAC3,false);
                    Step.iToStart++;
                    return false;

                case 11: 
                    if(!CL_Complete(ci.PSTB_MarkStprUpDn)) return false;
                    //if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_PusherFwBw    ,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_MarkAlignFWBw ,fb.Bwd)) return false;
                    MoveCyl(ci.PSTB_MarkSttnUpDn  ,fb.Bwd);
                    //MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart);
                    //MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart);
                    
                    Step.iToStart++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTB_MarkSttnUpDn  ,fb.Bwd)) return false;
                    //if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart)) return false;
                    //if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart)) return false;
                    
                    Step.iToStart++;
                    return false;
                
                case 13: 
                    
                    Step.iToStart = 0;
                    return true;
                    
            }

        }

        private bool bStpDtt ;
        override public bool ToStop() //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 8000)) ER_SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

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
                    bStpDtt = IO_GetX(xi.PSTB_MarkDetect) || IO_GetX(xi.PSTB_PkgDetect1) || IO_GetX(xi.PSTB_PkgDetect2);
                    if(!bStpDtt) MoveCyl(ci.PSTB_MarkStprUpDn  ,fb.Fwd);

                    bWaitX = MT_CmprPos(mi.PSTB_XMark,PM.GetValue(mi.PSTB_XMark,pv.PSTB_XMarkWait));
                    bWaitY = MT_CmprPos(mi.PSTB_YMark,PM.GetValue(mi.PSTB_YMark,pv.PSTB_YMarkWait));

                    if(!bWaitX || !bWaitY) MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    MoveCyl(ci.PSTB_PusherFwBw    ,fb.Bwd);
                    MoveCyl(ci.PSTB_MarkAlignFWBw ,fb.Bwd);
                    IO_SetY(yi.RAIL_FeedingAC3,false);
                    Step.iToStop++;
                    return false;
                    
                case 11: 
                    if(!bStpDtt && !CL_Complete(ci.PSTB_MarkStprUpDn  ,fb.Fwd)) return false;
                    if((!bWaitX || !bWaitY) && !CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_PusherFwBw    ,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_MarkAlignFWBw ,fb.Bwd)) return false;
                    MoveCyl(ci.PSTB_MarkSttnUpDn  ,fb.Bwd);
                    MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWait);
                    MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWait);
                    Step.iToStop++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTB_MarkSttnUpDn  ,fb.Bwd)) return false;
                    if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWait)) return false;
                    if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWait)) return false;
                    
                    MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Fwd);
                    Step.iToStop++;
                    return false;
                
                case 13: 
                    
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Fwd)) return false;
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
                bool None   = DM.ARAY[ri.PSTB].CheckAllStat(cs.None   ) ;
                //bool Fail   = DM.ARAY[ri.PSTB].GetCntStat(cs.Rslt0,cs.Rslt1,cs.Rslt2,cs.Rslt3,cs.Rslt4,cs.Rslt5,cs.Rslt6,cs.Rslt7,cs.Rslt8,cs.Rslt9,
                                                          //cs.RsltA,cs.RsltB,cs.RsltC,cs.RsltD,cs.RsltE,cs.RsltF,cs.RsltG,cs.RsltH,cs.RsltI,cs.RsltJ,cs.RsltK,cs.RsltL) > 0;
                bool Fail   = GetFail();                                                       

                //조건
                bool bPkgDetect  =  IO_GetX(xi.PSTB_MarkDetect) ;
                bool bPkgDetect1 =  IO_GetX(xi.PSTB_PkgDetect1) ;
                bool bPkgDetect2 =  IO_GetX(xi.PSTB_PkgDetect2) ;
                bool bLiftUp     = !CL_Complete(ci.PSTB_MarkSttnUpDn,fb.Bwd) ;
                //bool bWait       = CL_Complete(ci.PSTB_MarkStprUpDn,fb.Fwd) && CL_Complete(ci.PSTB_MarkAlignFWBw,fb.Bwd) && CL_Complete(ci.PSTB_PusherFwBw,fb.Bwd) && !bLiftUp ;

                double dPos    = DM.ARAY[ri.ULDR].FindFrstRow(cs.Empty) * OM.DevInfo.dMgzPitch ;
                bool   WorkPos = MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpWorkStart) - dPos) && SEQ.ULDR.CheckStop() &&
                                 DM.ARAY[ri.ULDR].GetCntStat(cs.Empty) > 0 && SEQ.ULDR.GetSeqStep() == 0 ; //언로더 워크 포지션 이고 정지중

                //싸이클
                bool isWait  =  None && !Wait() ;
                bool isIn    = !None && !bLiftUp &&  Fail && !IO_GetY(yi.RAIL_FeedingAC3);
                bool isWork  = !None &&  bLiftUp &&  Fail ;
                bool isOut   = !None &&  WorkPos && !Fail && !IO_GetY(yi.RAIL_FeedingAC3);
                bool isEnd   =  None && !bPkgDetect && !bPkgDetect1 && !bPkgDetect2;

                //모르는 자제 에러
                if( None && bPkgDetect) ER_SetErr(ei.PKG_Unknwn,"Postbuffer have no data found, but strip sensor detected") ;
                
                //카세트 사라짐
                //if(!None && !bPkgDetect) ER_SetErr(ei.PKG_Dispr  ,"Postbuffer have data, but strip sensor not detected") ;
                if(!None && !bPkgDetect && !bPkgDetect1 && !bPkgDetect2 &&
                   !bLiftUp && !IO_GetY(yi.RAIL_FeedingAC3)) ER_SetErr(ei.PKG_Dispr  ,"Postbuffer have data, but strip sensor not detected") ;

                if (ER_IsErr())
                {
                    return false;
                } 
                //Normal Decide Step.
                     if (isWait) { DM.ARAY[ri.PSTB].Trace(m_iPartId); Step.eSeq = sc.Wait; }
                else if (isIn  ) { DM.ARAY[ri.PSTB].Trace(m_iPartId); Step.eSeq = sc.In  ; }
                else if (isWork) { DM.ARAY[ri.PSTB].Trace(m_iPartId); Step.eSeq = sc.Work; }
                else if (isOut ) { DM.ARAY[ri.PSTB].Trace(m_iPartId); Step.eSeq = sc.Out ; }
                else if (isEnd ) { Stat.bWorkEnd = true; return true; }
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
                case (sc.Wait      ): if (!CycleWait(true)) return false; break;
                case (sc.In        ): if (!CycleIn  (    )) return false; break;
                case (sc.Work      ): if (!CycleWork(    )) return false; break;
                case (sc.Out       ): if (!CycleOut (    )) return false; break;
            }
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        /// <summary>
        /// 스토퍼랑 리프트등 자재 받을수 있는 상태
        /// </summary>
        public bool Wait()
        {
            bool bWait       = false;
            bool bLiftUp     = CL_Complete(ci.PSTB_MarkSttnUpDn,fb.Fwd) ;
            bWait = CL_Complete(ci.PSTB_MarkStprUpDn,fb.Fwd) && CL_Complete(ci.PSTB_MarkAlignFWBw,fb.Bwd) && CL_Complete(ci.PSTB_PusherFwBw,fb.Bwd) && !bLiftUp ;
            return bWait;
        }

        public bool GetFail(bool bAutorun = true)
        {
            CArray ar ;

            //if(bAutorun) ar = DM.ARAY[ri.PSTB];
            //else         ar = ArayMark        ;
            ar = DM.ARAY[ri.PSTB];

            for (int r = 0; r < ar.GetMaxRow(); r++)
            {
                for (int c = 0; c < ar.GetMaxCol(); c++)
                {
                    cs CS ;
                    if(bAutorun) CS = ar.GetStat(c, r);
                    else         CS = ar.GetStat(c, r);
                    if( (CS == cs.Rslt0 && !OM.CmnOptn.bNotMark0) || (CS == cs.Rslt8 && !OM.CmnOptn.bNotMark8) || (CS == cs.RsltG && !OM.CmnOptn.bNotMarkG) || 
                        (CS == cs.Rslt1 && !OM.CmnOptn.bNotMark1) || (CS == cs.Rslt9 && !OM.CmnOptn.bNotMark9) || (CS == cs.RsltH && !OM.CmnOptn.bNotMarkH) || 
                        (CS == cs.Rslt2 && !OM.CmnOptn.bNotMark2) || (CS == cs.RsltA && !OM.CmnOptn.bNotMarkA) || (CS == cs.RsltI && !OM.CmnOptn.bNotMarkI) || 
                        (CS == cs.Rslt3 && !OM.CmnOptn.bNotMark3) || (CS == cs.RsltB && !OM.CmnOptn.bNotMarkB) || (CS == cs.RsltJ && !OM.CmnOptn.bNotMarkJ) || 
                        (CS == cs.Rslt4 && !OM.CmnOptn.bNotMark4) || (CS == cs.RsltC && !OM.CmnOptn.bNotMarkC) || (CS == cs.RsltK && !OM.CmnOptn.bNotMarkK) || 
                        (CS == cs.Rslt5 && !OM.CmnOptn.bNotMark5) || (CS == cs.RsltD && !OM.CmnOptn.bNotMarkD) || (CS == cs.RsltL && !OM.CmnOptn.bNotMarkL) || 
                        (CS == cs.Rslt6 && !OM.CmnOptn.bNotMark6) || (CS == cs.RsltE && !OM.CmnOptn.bNotMarkE) || 
                        (CS == cs.Rslt7 && !OM.CmnOptn.bNotMark7) || (CS == cs.RsltF && !OM.CmnOptn.bNotMarkF) )
                        
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 결과값들 페일로 만든담에 마킹하기용
        /// </summary>
        public CArray ArayMark = new CArray();
        public bool CopyArray()
        {
            ArayMark.SetMaxColRow(DM.ARAY[ri.PSTB].GetMaxCol(), DM.ARAY[ri.PSTB].GetMaxRow());

            for (int r = 0; r < ArayMark.GetMaxRow(); r++)
            {
                for (int c = 0; c < ArayMark.GetMaxCol(); c++)
                {
                    cs CS ;
                    CS = DM.ARAY[ri.PSTB].GetStat(c, r);
                    if( (CS == cs.Rslt0 && !OM.CmnOptn.bNotMark0) || (CS == cs.Rslt8 && !OM.CmnOptn.bNotMark8) || (CS == cs.RsltG && !OM.CmnOptn.bNotMarkG) || 
                        (CS == cs.Rslt1 && !OM.CmnOptn.bNotMark1) || (CS == cs.Rslt9 && !OM.CmnOptn.bNotMark9) || (CS == cs.RsltH && !OM.CmnOptn.bNotMarkH) || 
                        (CS == cs.Rslt2 && !OM.CmnOptn.bNotMark2) || (CS == cs.RsltA && !OM.CmnOptn.bNotMarkA) || (CS == cs.RsltI && !OM.CmnOptn.bNotMarkI) || 
                        (CS == cs.Rslt3 && !OM.CmnOptn.bNotMark3) || (CS == cs.RsltB && !OM.CmnOptn.bNotMarkB) || (CS == cs.RsltJ && !OM.CmnOptn.bNotMarkJ) || 
                        (CS == cs.Rslt4 && !OM.CmnOptn.bNotMark4) || (CS == cs.RsltC && !OM.CmnOptn.bNotMarkC) || (CS == cs.RsltK && !OM.CmnOptn.bNotMarkK) || 
                        (CS == cs.Rslt5 && !OM.CmnOptn.bNotMark5) || (CS == cs.RsltD && !OM.CmnOptn.bNotMarkD) || (CS == cs.RsltL && !OM.CmnOptn.bNotMarkL) || 
                        (CS == cs.Rslt6 && !OM.CmnOptn.bNotMark6) || (CS == cs.RsltE && !OM.CmnOptn.bNotMarkE) || 
                        (CS == cs.Rslt7 && !OM.CmnOptn.bNotMark7) || (CS == cs.RsltF && !OM.CmnOptn.bNotMarkF) )
                        
                    {
                        ArayMark.SetStat(c, r, cs.Fail);
                    }
                    else
                    {
                        ArayMark.SetStat(c, r, cs.None);
                    }
                }
            }
            return true;
        }

        
        public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            
            //bool bRet = DM.ARAY[_iId].FindFrstColRow(ref c,ref r,cs.Rslt0,cs.Rslt1,cs.Rslt2,cs.Rslt3,cs.Rslt4,cs.Rslt5,cs.Rslt6,cs.Rslt7,cs.Rslt8,cs.Rslt9,
            //             cs.RsltA,cs.RsltB,cs.RsltC,cs.RsltD,cs.RsltE,cs.RsltF,cs.RsltG,cs.RsltH,cs.RsltI,cs.RsltJ,cs.RsltK,cs.RsltL) ;
            bool bRet = ArayMark.FindFrstColRow(ref c,ref r,cs.Fail);
            
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
                    CL_Move(ci.PSTB_MarkStprUpDn  ,fb.Fwd);
                    CL_Move(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    CL_Move(ci.PSTB_PusherFwBw    ,fb.Bwd);
                    CL_Move(ci.PSTB_MarkAlignFWBw ,fb.Bwd);
                    IO_SetY(yi.RAIL_FeedingAC3,false);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!CL_Complete(ci.PSTB_MarkStprUpDn  ,fb.Fwd)) return false;
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_PusherFwBw    ,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_MarkAlignFWBw ,fb.Bwd)) return false;
                    CL_Move(ci.PSTB_MarkSttnUpDn  ,fb.Bwd);
                    MT_GoHome(mi.PSTB_XMark);
                    MT_GoHome(mi.PSTB_YMark);
                    Step.iHome++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTB_MarkSttnUpDn  ,fb.Bwd)) return false;
                    if(!MT_GetHomeDone(mi.PSTB_XMark)) return false;
                    if(!MT_GetHomeDone(mi.PSTB_YMark)) return false;
                    MT_GoAbsMan(mi.PSTB_XMark,pv.PSTB_XMarkWait);
                    MT_GoAbsMan(mi.PSTB_YMark,pv.PSTB_YMarkWait);
                    
                    Step.iHome++;
                    return false;
                
                case 13: 
                    if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWait)) return false;
                    if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWait)) return false;
                    CL_Move(ci.PSTB_MarkingPenUpDn,fb.Fwd);
                    Step.iHome++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Fwd)) return false;

                    Step.iHome = 0;
                    return true;
            }
        }
        
        //펜꼽기 _bPutIn = true;
        //펜뽑아서 스타트 위치로 Default
        private bool bWaitX , bWaitY;
        public bool CycleWait(bool _bPutIn = false)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                IO_SetY(yi.RAIL_FeedingAC3,false);
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
                    //펜마킹 넥스트 무브 포지션 클리어.
                    iManNextCol=0;
                    iManNextRow=0;

                    MoveCyl(ci.PSTB_MarkStprUpDn  ,fb.Fwd);
                    MoveCyl(ci.PSTB_PusherFwBw    ,fb.Bwd);
                    MoveCyl(ci.PSTB_MarkAlignFWBw ,fb.Bwd);
                    IO_SetY(yi.RAIL_FeedingAC3,false);

                    if(!MT_GetStop(mi.PSTB_XMark)) return false; //밑에 있어야함
                    if(!MT_GetStop(mi.PSTB_YMark)) return false;
                    bWaitX = MT_CmprPos(mi.PSTB_XMark,PM.GetValue(mi.PSTB_XMark,pv.PSTB_XMarkWait));
                    bWaitY = MT_CmprPos(mi.PSTB_YMark,PM.GetValue(mi.PSTB_YMark,pv.PSTB_YMarkWait));

                    if(_bPutIn){
                        if(!bWaitX||!bWaitY) MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                        else                 MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Fwd);
                    }
                    else
                    {
                        MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    }
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.PSTB_MarkStprUpDn  ,fb.Fwd)) return false;
                    if(_bPutIn){
                        if(!bWaitX||!bWaitY) {if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;}
                        else                 {if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Fwd)) return false;}
                    }
                    else
                    {
                        {if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;}
                    }

                    if(!CL_Complete(ci.PSTB_PusherFwBw    ,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_MarkAlignFWBw  ,fb.Bwd)) return false;
                    MoveCyl(ci.PSTB_MarkSttnUpDn  ,fb.Bwd);
                    if(_bPutIn){
                        MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWait);
                        MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWait);
                    }
                    else        {
                        MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart);                                             
                        MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart);
                    }
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTB_MarkSttnUpDn  ,fb.Bwd)) return false;
                    if(_bPutIn){
                        if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWait)) return false;
                        if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWait)) return false;
                        MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Fwd);
                    }
                    else        {
                        if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart)) return false;
                        if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart)) return false;
                    }

                    
                    Step.iCycle++;
                    return false;
                
                case 13: 
                    
                    if(_bPutIn && !CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Fwd)) return false;
                    if (!_bPutIn)
                    {
                        CopyArray();
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleReplace()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                IO_SetY(yi.RAIL_FeedingAC3,false);
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
                    MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    MoveMotr(mi.PSTB_XMark,pv.PSTB_XReplace);
                    MoveMotr(mi.PSTB_YMark,pv.PSTB_YReplace);

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTB_MarkSttnUpDn  ,fb.Bwd))return false;
                    if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XReplace)) return false;
                    if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YReplace)) return false;

                    Step.iCycle = 0;
                    return true;
            }
        }

        //메뉴올동작으로 넥스트 눌렀을때 동작.
        int iManNextCol = 0 ;
        int iManNextRow = 0 ;
        public bool CycleNext() //Manual Cycle
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                IO_SetY(yi.RAIL_FeedingAC3,false);
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
                    if(iManNextRow >= ArayMark.GetMaxRow() || iManNextCol >= ArayMark.GetMaxCol())
                    {
                        iManNextCol=0;
                        iManNextRow=0;
                    }


                    //Copy
                    MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;

                    //FindChip(0,out iCol,out iRow);
                    OM.StripPos.GetPos(iManNextCol,iManNextRow,3,out dPos1,out dPos2);
                    

                    MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart, dPos1);                                             
                    MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart, dPos2);
                    Step.iCycle++;
                    return false;

                case 12: 
                    //OM.StripPos.GetPos(iManNextCol,iManNextRow,3,out dPos1,out dPos2);
                    if(!MT_GetStop(mi.PSTB_XMark)) return false;
                    if(!MT_GetStop(mi.PSTB_YMark)) return false;


                    iManNextRow++;

                    if(iManNextRow >= ArayMark.GetMaxRow())
                    {
                        iManNextCol++;
                        iManNextRow=0;
                    }
                    if(iManNextCol >= ArayMark.GetMaxCol())
                    {
                        iManNextCol=0;
                        iManNextRow=0;
                    }

                    


                    Step.iCycle = 0;
                    return true;
            }
        }

        bool bMoveWorkStt = false;
        public bool CycleIn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                IO_SetY(yi.RAIL_FeedingAC3,false);
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

            if(Step.iCycle > 10)
            {
                if(!bMoveWorkStt && CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd))
                {
                    int iCol,iRow;
                    CopyArray();
                    if(FindChip(0,out iCol,out iRow))
                    {
                        OM.StripPos.GetPos(iCol,iRow,3,out dPos1,out dPos2);
                        MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart, dPos1);
                        MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart, dPos2); //확인은 밑에서 하고 있음
                        bMoveWorkStt = true;
                    }
                }
            }

            bool r, c ;
       
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                //초기화
                case 10:
                    if(OM.CmnOptn.bIgnrMark)
                    {
                        for (int ir = 0; ir < DM.ARAY[ri.PSTB].GetMaxRow(); ir++)
                        {
                            for (int ic = 0; ic < DM.ARAY[ri.PSTB].GetMaxCol(); ic++)
                            {
                                cs CS ;
                                CS = DM.ARAY[ri.PSTB].GetStat(ic, ir);
                                if( (CS == cs.Rslt0 ) || (CS == cs.Rslt8 ) || (CS == cs.RsltG ) || 
                                    (CS == cs.Rslt1 ) || (CS == cs.Rslt9 ) || (CS == cs.RsltH ) || 
                                    (CS == cs.Rslt2 ) || (CS == cs.RsltA ) || (CS == cs.RsltI ) || 
                                    (CS == cs.Rslt3 ) || (CS == cs.RsltB ) || (CS == cs.RsltJ ) || 
                                    (CS == cs.Rslt4 ) || (CS == cs.RsltC ) || (CS == cs.RsltK ) || 
                                    (CS == cs.Rslt5 ) || (CS == cs.RsltD ) || (CS == cs.RsltL ) || 
                                    (CS == cs.Rslt6 ) || (CS == cs.RsltE ) || 
                                    (CS == cs.Rslt7 ) || (CS == cs.RsltF ) )
                                    
                                {
                                    DM.ARAY[ri.PSTB].SetStat(ic, ir, cs.Work);
                                }
                            }
                        }
                        Step.iCycle=0;
                        return true ;
                    }
                    bMoveWorkStt = false;
                    MoveCyl(ci.PSTB_MarkStprUpDn,fb.Fwd);
                    MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);//Step change check //if(Step.iCycle > 10)
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.PSTB_MarkStprUpDn,fb.Fwd)) return false;
                    IO_SetY(yi.RAIL_FeedingAC3,true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if(m_tmDelay.OnDelay(true,4000))
                    {
                        ER_SetErr(ei.RAIL_FeedingFail,"PEN MARKING ZONE FEED FAIL");
                        return true;
                    }
                    if(!IO_GetX(xi.PSTB_MarkDetect)) return false;
                    Step.iCycle++;
                    return false;

                case 13:
                    MoveCyl(ci.PSTB_MarkSttnUpDn,fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.PSTB_MarkSttnUpDn,fb.Fwd)) return false;
                    MoveCyl(ci.PSTB_MarkAlignFWBw,fb.Fwd);
                    IO_SetY(yi.RAIL_FeedingAC3,false);
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!CL_Complete(ci.PSTB_MarkAlignFWBw,fb.Fwd)) return false;
                    //MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    
                    //int iCol,iRow;
                    //CopyArray();
                    //if(!FindChip(0,out iCol,out iRow)) return false;
                    //OM.StripPos.GetPos(iCol,iRow,3,out dPos1,out dPos2);
                    //MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart, dPos1);
                    //MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart, dPos2);

                    //MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart); //위에서 미리 가따놓아서 굳이 미리 하지 않음
                    //MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart);
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart, dPos1)) return false;
                    if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart, dPos2)) return false;
                    Step.iCycle++;
                    return false;

                case 18:

                    Step.iCycle++;
                    return false;

                case 19:

                    Step.iCycle = 0;
                    return true;
            }
        }

        private double dPos1, dPos2;
        private int    iCol , iRow ;
        public bool CycleWork()
        {
            String sTemp;
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

            if (Stat.bReqStop || (MM.Working() && MM.Stop))
            {
                if(Step.iCycle == 20)
                {
                    return true;
                }
                
            }

            int r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    if(OM.CmnOptn.bIgnrMark)
                    {
                        for (r = 0; r < DM.ARAY[ri.PSTB].GetMaxRow(); r++)
                        {
                            for (c = 0; c < DM.ARAY[ri.PSTB].GetMaxCol(); c++)
                            {
                                cs CS ;
                                CS = DM.ARAY[ri.PSTB].GetStat(c, r);
                                if( (CS == cs.Rslt0 ) || (CS == cs.Rslt8 ) || (CS == cs.RsltG ) || 
                                    (CS == cs.Rslt1 ) || (CS == cs.Rslt9 ) || (CS == cs.RsltH ) || 
                                    (CS == cs.Rslt2 ) || (CS == cs.RsltA ) || (CS == cs.RsltI ) || 
                                    (CS == cs.Rslt3 ) || (CS == cs.RsltB ) || (CS == cs.RsltJ ) || 
                                    (CS == cs.Rslt4 ) || (CS == cs.RsltC ) || (CS == cs.RsltK ) || 
                                    (CS == cs.Rslt5 ) || (CS == cs.RsltD ) || (CS == cs.RsltL ) || 
                                    (CS == cs.Rslt6 ) || (CS == cs.RsltE ) || 
                                    (CS == cs.Rslt7 ) || (CS == cs.RsltF ) )
                                    
                                {
                                    DM.ARAY[ri.PSTB].SetStat(c, r, cs.Work);
                                }
                            }
                        }
                        Step.iCycle=0;
                        return true ;
                    }


                    MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    MoveCyl(ci.PSTB_MarkSttnUpDn,fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.PSTB_MarkSttnUpDn,fb.Fwd)) return false;
                    MoveCyl(ci.PSTB_MarkAlignFWBw,fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.PSTB_MarkAlignFWBw,fb.Fwd)) return false;
                    //마킹용 어레이로 포스트 버퍼 복사.
                    
                    Step.iCycle = 15;
                    return false;

                case 15: //위 아래에서 사용 변경 유의
                    CopyArray();
                    if(!FindChip(0,out iCol,out iRow)) return false;
                    OM.StripPos.GetPos(iCol,iRow,3,out dPos1,out dPos2);
                    MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart, dPos1);
                    MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart, dPos2);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart, dPos1)) return false;
                    if(!MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart, dPos2)) return false;
                    MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Fwd)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!m_tmDelay.OnDelay(true,300)) return false;
                    MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 19:
                    if(!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    DM.ARAY[ri.PSTB].SetStat(iCol,iRow,cs.Work);
                    ArayMark.SetStat(iCol,iRow,cs.Work);
                    Step.iCycle++;
                    return false;

                case 20: //위에서 사용
                    //bool Fail   = DM.ARAY[ri.PSTB].GetCntStat(cs.Rslt0,cs.Rslt1,cs.Rslt2,cs.Rslt3,cs.Rslt4,cs.Rslt5,cs.Rslt6,cs.Rslt7,cs.Rslt8,cs.Rslt9,
                    //                                          cs.RsltA,cs.RsltB,cs.RsltC,cs.RsltD,cs.RsltE,cs.RsltF,cs.RsltG,cs.RsltH,cs.RsltI,cs.RsltJ,cs.RsltK,cs.RsltL) > 0;
                    bool Fail = GetFail(false);
                    if(Fail){
                        Step.iCycle=15;
                        return false;
                    }

                    //MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWorkStart);
                    //MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWorkStart);

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
                IO_SetY(yi.RAIL_FeedingAC3,false);
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
        
            bool b1 = MT_GetStopPos(mi.PSTB_XMark,pv.PSTB_XMarkWait);
            bool b2 = MT_GetStopPos(mi.PSTB_YMark,pv.PSTB_YMarkWait);

            if(Step.iCycle >= 12)
            {
                if(b1 && b2)
                {
                    MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Fwd);
                }
            }

            switch (Step.iCycle)
            {
        
                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;
        
                case 10:

                    if(!DM.ARAY[ri.SPC].CheckAllStat(cs.None) && DM.ARAY[ri.SPC].LotNo!="") //20190121 DM.ARAY[ri.SPC].LotNo!="" 추가 이거 없으면 메뉴얼 동작 같은것 할때 찌꺼기SPC데이터 남음.
                    {
                        //DM.CopyData()
                        for(int i = 0 ; i < (int)cs.MAX_CHIP_STAT ; i++)
                        {
                            OM.EqpStat.iRsltCnts[i] += DM.ARAY[(int)ri.SPC].GetCntStat((cs)i);
                        }
                        SPC.MAP.SaveDataMap(ri.SPC);
                        SPC.MAP.SaveDataCnt(SPC.LOT.Data.StartedAt , DM.ARAY[ri.SPC].LotNo , OM.EqpStat.iRsltCnts);


                        Trace(                    "Good AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Good ).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Good ]);
                                                                                                                
                        Trace(OM.CmnOptn.sRsltName0 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt0).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt0]);
                        Trace(OM.CmnOptn.sRsltName1 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt1).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt1]);
                        Trace(OM.CmnOptn.sRsltName2 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt2).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt2]);
                        Trace(OM.CmnOptn.sRsltName3 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt3).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt3]);
                        Trace(OM.CmnOptn.sRsltName4 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt4).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt4]);
                        Trace(OM.CmnOptn.sRsltName5 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt5).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt5]);
                        Trace(OM.CmnOptn.sRsltName6 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt6).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt6]);
                        Trace(OM.CmnOptn.sRsltName7 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt7).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt7]);
                        Trace(OM.CmnOptn.sRsltName8 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt8).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt8]);
                        Trace(OM.CmnOptn.sRsltName9 + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt9).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.Rslt9]);
                        Trace(OM.CmnOptn.sRsltNameA + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltA).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltA]);
                        Trace(OM.CmnOptn.sRsltNameB + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltB).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltB]);
                        Trace(OM.CmnOptn.sRsltNameC + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltC).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltC]);
                        Trace(OM.CmnOptn.sRsltNameD + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltD).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltD]);
                        Trace(OM.CmnOptn.sRsltNameE + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltE).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltE]);
                        Trace(OM.CmnOptn.sRsltNameF + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltF).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltF]);
                        Trace(OM.CmnOptn.sRsltNameG + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltG).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltG]);
                        Trace(OM.CmnOptn.sRsltNameH + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltH).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltH]);
                        Trace(OM.CmnOptn.sRsltNameI + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltI).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltI]);
                        Trace(OM.CmnOptn.sRsltNameJ + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltJ).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltJ]);
                        Trace(OM.CmnOptn.sRsltNameK + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltK).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltK]);
                        Trace(OM.CmnOptn.sRsltNameL + " AddedCnt = " + DM.ARAY[(int)ri.SPC].GetCntStat(cs.RsltL).ToString() + " TotalCnt = " + OM.EqpStat.iRsltCnts[(int)cs.RsltL]);


                        //Lot Count Check
                        int iFailCnt = DM.ARAY[(int)ri.SPC].GetCntStat(cs.Rslt0 ,
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
                        //Lot Info
                        OM.EqpStat.iWorkCnt += OM.DevInfo.iRowCnt * OM.DevInfo.iColCnt ;
                        OM.EqpStat.iFailCnt += iFailCnt ;
                        //Day Info
                        //일단 디스플레이 안하기 때문에 주석... 오버플로우 걱정..
                        //OM.EqpStat.iTotalWorkCnt += OM.EqpStat.iWorkCnt ;
                        OM.EqpStat.iDayWorkCnt   += OM.DevInfo.iRowCnt * OM.DevInfo.iColCnt ;

                        DM.ARAY[ri.SPC].ClearMap();
                    }




                    if(!b1 || !b2) MoveCyl(ci.PSTB_MarkingPenUpDn,fb.Bwd);
                    MoveCyl(ci.PSTB_MarkSttnUpDn  ,fb.Bwd);
                    MoveCyl(ci.PSTB_MarkAlignFWBw ,fb.Bwd);
                    MoveCyl(ci.PSTB_MarkStprUpDn  ,fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if((!b1 || !b2) &&!CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_MarkSttnUpDn  ,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_MarkAlignFWBw ,fb.Bwd)) return false;
                    if(!CL_Complete(ci.PSTB_MarkStprUpDn  ,fb.Bwd)) return false;
                    MoveMotr(mi.PSTB_XMark,pv.PSTB_XMarkWait);
                    MoveMotr(mi.PSTB_YMark,pv.PSTB_YMarkWait); //Step Change Need to Check //if(Step.iCycle >= 12)
                    IO_SetY(yi.RAIL_FeedingAC3,true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    r = IO_GetX(xi.PSTB_MarkDetect) || IO_GetX(xi.PSTB_PkgDetect1);
                    //c = IO_GetX(xi.PSTB_PkgDetect2) ;
                    if(m_tmDelay.OnDelay(true,4000))
                    {
                        IO_SetY(yi.RAIL_FeedingAC3,false);
                        ER_SetErr(ei.RAIL_FeedingFail,"Strip Out Feed Fail");
                        return true;
                    }
                    if(r) return false;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!m_tmDelay.OnDelay(500))return false ;
                    //if(r || !c) return false;
                    MoveCyl(ci.PSTB_PusherFwBw,fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.PSTB_PusherFwBw,fb.Fwd)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!m_tmDelay.OnDelay(true,500)) return false;
                    MoveCyl(ci.PSTB_PusherFwBw,fb.Bwd);
                    Step.iCycle++;
                    return false;
        
                case 16:
                    if(!CL_Complete(ci.PSTB_PusherFwBw,fb.Bwd)) return false;
                    if (IO_GetX(xi.PSTB_PkgDetect1))
                    {
                        IO_SetY(yi.RAIL_FeedingAC3,false);
                        ER_SetErr(ei.RAIL_FeedingFail,"Strip Out Feed Fail");
                        return true;
                    }
                    IO_SetY(yi.RAIL_FeedingAC3,false);

                    

                    //Data Mask
                    iRow = DM.ARAY[ri.ULDR].FindFrstRow(cs.Empty);
                    
                    DM.ARAY[ri.ULDR].LotNo = DM.ARAY[ri.PSTB].LotNo ;
                    DM.ARAY[ri.ULDR].SetStat(0,iRow,cs.Work);

                    DM.ARAY[ri.PSTB].ClearMap();
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.PSTB_MarkStprUpDn)
            {

            }
            else if (_eActr == ci.PSTB_MarkAlignFWBw)
            {

            }
            else if (_eActr == ci.PSTB_MarkingPenUpDn)
            {
                if(!MT_GetStop(mi.PSTB_XMark) || !MT_GetStop(mi.PSTB_YMark)) {
                    sMsg = "Marking Zone Motor is Moving";
                    return false;
                }
            }
            else if (_eActr == ci.PSTB_MarkSttnUpDn)
            {

            }
            else if (_eActr == ci.PSTB_PusherFwBw)
            {
                //if (IO_GetX(xi.PSTB_PkgDetect1)) {
                //    sMsg = "The Sensor is Detected In Before the end of the rail";
                //    return false;
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

        public bool CheckSafe(mi _eMotr, pv _ePstn, double _dOfsPos = 0)
        {
            if (OM.MstOptn.bDebugMode) return true;
            double dDstPos = PM_GetValue(_eMotr, _ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";

            bool bPenBwd = CL_Complete(ci.PSTB_MarkingPenUpDn,fb.Bwd);

            if (!bPenBwd)
            {
                sMsg = "Need to Pen Up Position";
                bRet = false;
            }

            if (_eMotr == mi.PSTB_XMark)
            {
            
            }
            else if (_eMotr == mi.PSTB_YMark)
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

            if (_eMotr == mi.PSTB_XMark)
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
            else if (_eMotr == mi.PSTB_YMark)
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
            if (!MT_GetStop(mi.PSTB_XMark)) return false;
            if (!MT_GetStop(mi.PSTB_YMark)) return false;

            if (!CL_Complete(ci.PSTB_MarkStprUpDn  )) return false;
            if (!CL_Complete(ci.PSTB_MarkAlignFWBw )) return false;
            if (!CL_Complete(ci.PSTB_MarkingPenUpDn)) return false;
            if (!CL_Complete(ci.PSTB_MarkSttnUpDn  )) return false;
            if (!CL_Complete(ci.PSTB_PusherFwBw    )) return false;

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
