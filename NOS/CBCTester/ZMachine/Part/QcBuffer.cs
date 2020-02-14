using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class QcBuffer : Part
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
            }
        };   

        public enum sc
        {
            Idle      = 0,
            MainPumpOn   ,
            RtPickFrg    ,
            RtPlceBfr    ,
            UnFreeze     ,
            LtPickBfr    ,
            LtPlceSut    ,
            LtPickSut    ,
            LtPlceBfr    ,
            RtPickBfr    ,
            RtPlceFrg    ,
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



        protected CDelayTimer m_tmUnFreeze = new CDelayTimer() ;      

        public QcBuffer(int _iPartId = 0)
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
                    Step.iToStart = 0;
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Bwd);

                    Step.iToStart++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd)) return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd)) return false ;

                    MoveCyl(ci.QCB_RtPckCmpLtRt , fb.Bwd);
                    Step.iToStart++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Bwd)) return false ;
                    MoveCyl(ci.QCB_LtPckCmpLtRt , fb.Bwd);
                    Step.iToStart++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_LtPckCmpLtRt , fb.Bwd)) return false ;

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
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Bwd);

                    Step.iToStop++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd)) return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd)) return false ;

                    MoveCyl(ci.QCB_RtPckCmpLtRt , fb.Bwd);
                    Step.iToStop++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Bwd)) return false ;
                    MoveCyl(ci.QCB_LtPckCmpLtRt , fb.Bwd);
                    Step.iToStop++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_LtPckCmpLtRt , fb.Bwd)) return false ;

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

        CDelayTimer PumpChargingTimer = new CDelayTimer();
        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                

                //오토큐씨 아닐땐 안돔.
                if (!OM.CmnOptn.bAutoQc) { Stat.bWorkEnd = true; return true; }

                //bool isCleaning = DM.ARAY[ri.CHA].CheckAllStat(cs.Clean) || DM.ARAY[ri.CHA].CheckAllStat(cs.None);
                //bool isNone = DM.ARAY[ri.SYR].CheckAllStat(cs.None) && DM.ARAY[ri.SUT].CheckAllStat(cs.None);
                //
                ////사이클
                //bool isPickLdr = DM.ARAY[ri.LDR].CheckAllStat(cs.Shake) && DM.ARAY[ri.PKR].CheckAllStat(cs.None) && isCleaning && isNone;
                //bool isShake = DM.ARAY[ri.PKR].CheckAllStat(cs.Shake);
                                 //이거 타이밍 문제 있어서 짜증남. 미리 가면 실린지 썩 할때 챔버 필링하고 있어. 니들을 못닦음.
                bool isCleaning = /*DM.ARAY[ri.CHA].CheckAllStat(cs.Clean) ||*/ DM.ARAY[ri.CHA].CheckAllStat(cs.None);
                bool isNone = DM.ARAY[ri.SYR].CheckAllStat(cs.None) && DM.ARAY[ri.SUT].CheckAllStat(cs.None);

                //사이클
                //     툴 동작 목적지
                bool isMainPump  =  IO_GetY(yi.ETC_MainPumpOff);
                bool isRtPickFrg =  DM.ARAY[ri.FRG].IsExist     (cs.Freeze ) && DM.ARAY[ri.RPK].CheckAllStat(cs.None)      ;
                bool isRtPlceBfr =  DM.ARAY[ri.RPK].CheckAllStat(cs.Freeze ) && DM.ARAY[ri.BFR].IsExist     (cs.None)      ;
                bool isUnFreeze  =  DM.ARAY[ri.BFR].IsExist     (cs.Freeze ) &&!DM.ARAY[ri.FRG].IsExist     (cs.Freeze )   && !DM.ARAY[ri.RPK].IsExist(cs.Freeze) && m_tmUnFreeze.OnDelay(OM.CmnOptn.iUnFreezeMin * 60*1000);                
                bool isLtPickBfr =  DM.ARAY[ri.BFR].IsExist     (cs.Shake  ) && DM.ARAY[ri.LPK].CheckAllStat(cs.None)      && DM.ARAY[ri.PKR].CheckAllStat(cs.None) && isCleaning && isNone ;
                bool isLtPlceSut =  DM.ARAY[ri.LPK].CheckAllStat(cs.Shake  ) && DM.ARAY[ri.SUT].CheckAllStat(cs.None)      ;
                bool isLtPickSut =  DM.ARAY[ri.LPK].CheckAllStat(cs.None   ) && DM.ARAY[ri.SUT].CheckAllStat(cs.WorkEnd)   ;
                bool isLtPlceBfr =  DM.ARAY[ri.LPK].CheckAllStat(cs.WorkEnd) && DM.ARAY[ri.BFR].IsExist     (cs.None)      ;
                bool isRtPickBfr =  DM.ARAY[ri.RPK].CheckAllStat(cs.None   ) && DM.ARAY[ri.BFR].IsExist     (cs.WorkEnd)   ;
                bool isRtPlceFrg =  DM.ARAY[ri.RPK].CheckAllStat(cs.WorkEnd) && DM.ARAY[ri.FRG].IsExist     (cs.None)      ;

                bool isWorkEnd   =  DM.ARAY[ri.LPK].CheckAllStat(cs.None   )     && DM.ARAY[ri.SUT].CheckAllStat(cs.None   ) &&
                                    DM.ARAY[ri.RPK].CheckAllStat(cs.None   )     && DM.ARAY[ri.BFR].CheckAllStat(cs.None   ) && !DM.ARAY[ri.FRG].IsExist     (cs.Freeze , cs.Shake ); 

                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.
                     if (isRtPlceFrg) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.RtPlceFrg ; }
                else if (isRtPickBfr) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.RtPickBfr ; }
                else if (isLtPlceBfr) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.LtPlceBfr ; }
                else if (isLtPickSut) { DM.ARAY[ri.SUT].Trace(m_sPartName); Step.eSeq = sc.LtPickSut ; }
                else if (isLtPlceSut) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.LtPlceSut ; }
                else if (isLtPickBfr) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.LtPickBfr ; }
                else if (isUnFreeze ) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.UnFreeze  ; }
                else if (isRtPlceBfr) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.RtPlceBfr ; }
                else if (isRtPickFrg) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.RtPickFrg ; }
                else if (isWorkEnd  ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if (Step.eSeq != sc.Idle){
                    Trace(Step.eSeq.ToString() +" Start");
                    Log.TraceListView(Step.eSeq.ToString() + " 동작 시작");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }
            
            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default            : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case (sc.Idle     ): return false;
                case (sc.RtPickFrg): if (!CycleRtPickFrg()) return false; break;
                case (sc.RtPlceBfr): if (!CycleRtPlceBfr()) return false; break;
                case (sc.UnFreeze ): if (!CycleUnFreeze ()) return false; break;
                case (sc.LtPickBfr): if (!CycleLtPickBfr()) return false; break;
                case (sc.LtPlceSut): if (!CycleLtPlceSut()) return false; break;
                case (sc.LtPickSut): if (!CycleLtPickSut()) return false; break;
                case (sc.LtPlceBfr): if (!CycleLtPlceBfr()) return false; break;
                case (sc.RtPickBfr): if (!CycleRtPickBfr()) return false; break;
                case (sc.RtPlceFrg): if (!CycleRtPlceFrg()) return false; break;
            }
            Trace(sCycle+" End");
            Log.TraceListView(sCycle + " 동작 종료");
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

       

        //위에 부터 작업.
        //인덱스 데이터에서 작업해야 되는 컬로우를 뽑아서 리턴.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            return DM.ARAY[_iId].FindLastColFrstRow(ref c, ref r , _iChip);
        }

        public bool CycleHome()
        {
            string sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 10000 )) {
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
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Bwd);

                    Step.iHome++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd)) return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd)) return false ;

                    MoveCyl(ci.QCB_RtPckCmpLtRt , fb.Bwd);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Bwd)) return false ;
                    MoveCyl(ci.QCB_LtPckCmpLtRt , fb.Bwd);
                    Step.iHome++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_LtPckCmpLtRt , fb.Bwd)) return false ;

                    Step.iHome = 0;
                    return true;
            }
        }

        int iWorkR = 0 ;
        //냉장고에서 찝기
        public bool CycleRtPickFrg()
        {
            string sTemp;
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpClOp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpClOp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpLtRt, fb.Bwd);

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Bwd))return false ;
                    iWorkR = DM.ARAY[ri.FRG].FindFrstRow(cs.Freeze);
                    if(iWorkR == 0)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr , fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr , fb.Fwd);
                    }
                    else if(iWorkR == 1)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr , fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr , fb.Bwd);
                    }
                    else
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr , fb.Bwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr , fb.Bwd);
                    }
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.QCB_BfStg1stFtRr))return false ;
                    if(!CL_Complete(ci.QCB_BfStg2ndFtRr))return false ;
                    if (iWorkR == 0)
                    {
                        if (!IO_GetX(xi.QCB_BfStgFrnt)) return false;
                    }
                    else if (iWorkR == 1)
                    {
                        if (!IO_GetX(xi.QCB_BfStgMidl)) return false;
                    }
                    else
                    {
                        if (!IO_GetX(xi.QCB_BfStgRear)) return false;
                    }
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpClOp, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!CL_Complete(ci.QCB_RtPckCmpClOp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.FRG].SetStat(0,iWorkR,cs.None  );
                    DM.ARAY[ri.RPK].SetStat(cs.Freeze);

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //버퍼에 내려놓기.
        public bool CycleRtPlceBfr()
        {
            string sTemp;
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpLtRt, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.QCB_LtPckCmpLtRt , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpLtRt, fb.Fwd);

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Fwd))return false ;                    
                    iWorkR = DM.ARAY[ri.BFR].FindFrstRow(cs.None);
                    if (iWorkR == 0)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Fwd);
                    }
                    else if (iWorkR == 1)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    else
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Bwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.QCB_BfStg1stFtRr))return false ;
                    if(!CL_Complete(ci.QCB_BfStg2ndFtRr))return false ;
                    if (iWorkR == 0)
                    {
                        if (!IO_GetX(xi.QCB_BfStgFrnt)) return false;
                    }
                    else if (iWorkR == 1)
                    {
                        if (!IO_GetX(xi.QCB_BfStgMidl)) return false;
                    }
                    else
                    {
                        if (!IO_GetX(xi.QCB_BfStgRear)) return false;
                    }
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpClOp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!CL_Complete(ci.QCB_RtPckCmpClOp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.BFR].SetStat(0,iWorkR,cs.Freeze  );
                    DM.ARAY[ri.RPK].SetStat(cs.None);
                    MoveCyl(ci.QCB_RtPckCmpLtRt, fb.Bwd);

                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Bwd))return false ;         

                    //해동시간 스타트... 기존에 하고 있었으면 리셑. 3개만드는것은 오바인듯 함.
                    m_tmUnFreeze.Clear();

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //해동완료.
        public bool CycleUnFreeze()
        {
            string sTemp;
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.BFR].ChangeStat(cs.Freeze , cs.Shake);
                    
                    Step.iCycle= 0 ;
                    return true ;
            }
        }
        
        //버퍼에서 찝기
        public bool CycleLtPickBfr()
        {
            string sTemp;
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpLtRt, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpLtRt, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpClOp, fb.Bwd);

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_LtPckCmpLtRt , fb.Bwd))return false ;                    
                    if(!CL_Complete(ci.QCB_LtPckCmpClOp , fb.Bwd))return false ;                    
                    iWorkR = DM.ARAY[ri.BFR].FindFrstRow(cs.Shake);
                    if(iWorkR == -1)
                    {
                        if(SEQ._bRun)
                        {
                            ER_SetErr(ei.PKG_Dispr , "Shake 상태의 시료가 버퍼에 존재하지 않습니다.");
                            return true ;
                        }
                        else
                        {
                            Log.ShowMessage("Error" , "Shake 상태의 시료가 버퍼에 존재하지 않습니다.");
                        }
                    }
                    if (iWorkR == 0)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Fwd);
                    }
                    else if (iWorkR == 1)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    else
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Bwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }

                    

                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.QCB_BfStg1stFtRr))return false ;
                    if(!CL_Complete(ci.QCB_BfStg2ndFtRr))return false ;
                    if (iWorkR == 0)
                    {
                        if (!IO_GetX(xi.QCB_BfStgFrnt)) return false;
                    }
                    else if (iWorkR == 1)
                    {
                        if (!IO_GetX(xi.QCB_BfStgMidl)) return false;
                    }
                    else
                    {
                        if (!IO_GetX(xi.QCB_BfStgRear)) return false;
                    }
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpClOp, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!CL_Complete(ci.QCB_LtPckCmpClOp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.BFR].SetStat(0,iWorkR,cs.None  );
                    DM.ARAY[ri.LPK].SetStat(cs.Shake);
                      

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //셔틀에 내려놓기
        public bool CycleLtPlceSut()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);

                IO_SetY(yi.PKR_BarcodeSpin,false);
                SEQ.Bar.Stop();

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
                    IO_SetY(yi.PKR_BarcodeSpin,false);
                    SEQ.Bar.Stop();
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    if (iWorkR == 0)
                    {
                        if (!IO_GetX(xi.QCB_BfStgFrnt)) return false;
                    }
                    else if (iWorkR == 1)
                    {
                        if (!IO_GetX(xi.QCB_BfStgMidl)) return false;
                    }
                    else
                    {
                        if (!IO_GetX(xi.QCB_BfStgRear)) return false;
                    }
                    MoveCyl(ci.QCB_LtPckCmpLtRt, fb.Fwd);
                    SEQ.PKR.MoveMotr(mi.PKR_YSutl , pv.PKR_YSutlBuffer );

                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete  (ci.QCB_LtPckCmpLtRt , fb.Fwd            ))return false ;                    
                    if(!MT_GetStopPos(mi.PKR_YSutl        , pv.PKR_YSutlBuffer))return false ;                  
                    
                    //혹시 실수로 시험관 있을까봐 한번 바코드로 찍어봄.
                    //if(!SEQ.Bar.Read())
                    //{
                    //    ER_SetErr(ei.PKR_Barcode , "메세지 전송을 실패하였습니다.");
                    //    return false ;
                    //}
                    //
                    //IO_SetY( yi.PKR_BarcodeSpin , true) ; 

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 13:
                    //if(SEQ.Bar.ReadEnded()) 
                    //{
                    //    ER_SetErr(ei.PKR_Barcode , "자제 바코드가 감지 되었습니다. 자제를 빼주세요.");
                    //    IO_SetY( yi.PKR_BarcodeSpin , false) ; 
                    //    SEQ.Bar.Stop();
                    //    return false ;
                    //}
                    
                    //if(!m_tmDelay.OnDelay(2000))return false ;
                    //SEQ.Bar.Stop();
                    //IO_SetY( yi.PKR_BarcodeSpin , false) ; 

                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpClOp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.QCB_LtPckCmpClOp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.SUT].SetStat(cs.Shake);
                    DM.ARAY[ri.LPK].SetStat(cs.None);
                      

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //셔틀에서 찝기
        public bool CycleLtPickSut()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);

                IO_SetY(yi.PKR_BarcodeSpin,false);
                SEQ.Bar.Stop();

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
                    IO_SetY(yi.PKR_BarcodeSpin,false);
                    SEQ.Bar.Stop();
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;

                    MoveCyl(ci.QCB_LtPckCmpLtRt, fb.Fwd);
                    MoveCyl(ci.QCB_LtPckCmpClOp, fb.Bwd);
                    SEQ.PKR.MoveMotr(mi.PKR_YSutl , pv.PKR_YSutlBuffer );

                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete  (ci.QCB_LtPckCmpLtRt , fb.Fwd            ))return false ;                    
                    if(!CL_Complete  (ci.QCB_LtPckCmpClOp , fb.Bwd            ))return false ;                    
                    if(!MT_GetStopPos(mi.PKR_YSutl        , pv.PKR_YSutlBuffer))return false ;                  
                    
                    //혹시 돌고 있을까봐.
                    IO_SetY( yi.PKR_BarcodeSpin , false) ; 

                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpClOp, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.QCB_LtPckCmpClOp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.SUT].SetStat(cs.None);
                    DM.ARAY[ri.LPK].SetStat(cs.WorkEnd);

                    //MoveCyl(ci.QCB_LtPckCmpLtRt , fb.Bwd); 버퍼에 이미 있는 애들하고 충돌 우려 있음.
                    Step.iCycle++;
                    return false ;

                case 16:
                    //if(!MoveCyl(ci.QCB_LtPckCmpLtRt , fb.Bwd)) return false ;
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //버퍼에 내려놓기.
        public bool CycleLtPlceBfr()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);

                IO_SetY(yi.PKR_BarcodeSpin,false);
                SEQ.Bar.Stop();

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
                    IO_SetY(yi.PKR_BarcodeSpin,false);
                    SEQ.Bar.Stop();
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;

                    MoveCyl(ci.QCB_LtPckCmpLtRt, fb.Bwd);


                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete  (ci.QCB_LtPckCmpLtRt , fb.Bwd            ))return false ;         
                    iWorkR = DM.ARAY[ri.BFR].FindFrstRow(cs.None);
                    if (iWorkR == 0)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Fwd);
                    }
                    else if (iWorkR == 1)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    else
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Bwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_BfStg1stFtRr))return false ;
                    if(!CL_Complete(ci.QCB_BfStg2ndFtRr))return false ;
                    if (iWorkR == 0)
                    {
                        if (!IO_GetX(xi.QCB_BfStgFrnt)) return false;
                    }
                    else if (iWorkR == 1)
                    {
                        if (!IO_GetX(xi.QCB_BfStgMidl)) return false;
                    }
                    else
                    {
                        if (!IO_GetX(xi.QCB_BfStgRear)) return false;
                    }
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpClOp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.QCB_LtPckCmpClOp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpDnUp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.BFR].SetStat(0,iWorkR,cs.WorkEnd);
                    DM.ARAY[ri.LPK].SetStat(cs.None);

                    MoveCyl(ci.QCB_LtPckCmpLtRt , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!MoveCyl(ci.QCB_LtPckCmpLtRt , fb.Fwd)) return false ;
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //버퍼에서 찝기
        public bool CycleRtPickBfr()
        {
            string sTemp;
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_LtPckCmpLtRt, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.QCB_LtPckCmpLtRt , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpLtRt, fb.Fwd);
                    MoveCyl(ci.QCB_RtPckCmpClOp, fb.Bwd);

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Fwd))return false ;   
                    if(!CL_Complete(ci.QCB_RtPckCmpClOp , fb.Bwd))return false ;   
                    iWorkR = DM.ARAY[ri.BFR].FindFrstRow(cs.WorkEnd);
                    if (iWorkR == 0)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Fwd);
                    }
                    else if (iWorkR == 1)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    else
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Bwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.QCB_BfStg1stFtRr))return false ;
                    if(!CL_Complete(ci.QCB_BfStg2ndFtRr))return false ;
                    if (iWorkR == 0)
                    {
                        if (!IO_GetX(xi.QCB_BfStgFrnt)) return false;
                    }
                    else if (iWorkR == 1)
                    {
                        if (!IO_GetX(xi.QCB_BfStgMidl)) return false;
                    }
                    else
                    {
                        if (!IO_GetX(xi.QCB_BfStgRear)) return false;
                    }
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpClOp, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!CL_Complete(ci.QCB_RtPckCmpClOp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.BFR].SetStat(0,iWorkR,cs.None  );
                    DM.ARAY[ri.RPK].SetStat(cs.WorkEnd);
                    //MoveCyl(ci.QCB_RtPckCmpLtRt, fb.Bwd); 먼저하면 2번째꺼부터 처박음.

                    Step.iCycle++;
                    return false ;

                case 18:
                    //if(!CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Bwd))return false ;         
                    m_tmUnFreeze.Clear();
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //냉장고에 놓기.
        public bool CycleRtPlceFrg()
        {
            string sTemp;
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    //MoveCyl(ci.QCB_RtPckCmpClOp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    //if(!CL_Complete(ci.QCB_RtPckCmpClOp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpDnUp, fb.Bwd);
                    MoveCyl(ci.QCB_LtPckCmpDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    if(!CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Bwd))return false ;
                    
                    iWorkR = DM.ARAY[ri.FRG].FindFrstRow(cs.None);
                    if (iWorkR == 0)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Fwd);
                    }
                    else if (iWorkR == 1)
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Fwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    else
                    {
                        MoveCyl(ci.QCB_BfStg1stFtRr, fb.Bwd);
                        MoveCyl(ci.QCB_BfStg2ndFtRr, fb.Bwd);
                    }
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.QCB_BfStg1stFtRr))return false ;
                    if(!CL_Complete(ci.QCB_BfStg2ndFtRr))return false ;
                    if (iWorkR == 0)
                    {
                        if (!IO_GetX(xi.QCB_BfStgFrnt)) return false;
                    }
                    else if (iWorkR == 1)
                    {
                        if (!IO_GetX(xi.QCB_BfStgMidl)) return false;
                    }
                    else
                    {
                        if (!IO_GetX(xi.QCB_BfStgRear)) return false;
                    }
                    MoveCyl(ci.QCB_RtPckCmpLtRt, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.QCB_RtPckCmpLtRt, fb.Bwd)) return false;
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Fwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpClOp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!CL_Complete(ci.QCB_RtPckCmpClOp , fb.Bwd))return false ;
                    MoveCyl(ci.QCB_RtPckCmpDnUp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Bwd))return false ;
                    DM.ARAY[ri.FRG].SetStat(0,iWorkR,cs.WorkEnd  );
                    DM.ARAY[ri.RPK].SetStat(cs.None);

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;


                 if (_eActr == ci.QCB_RtPckCmpClOp){}
            else if (_eActr == ci.QCB_RtPckCmpDnUp){}
            else if (_eActr == ci.QCB_RtPckCmpLtRt)
            {
                if(CL_Complete(ci.QCB_RtPckCmpDnUp , fb.Fwd))
                {
                    sMsg = CL_GetName(ci.QCB_RtPckCmpDnUp) + " 실린더가 다운 되어 있습니다." ;
                    bRet = false ;
                }
                if(CL_Complete(ci.QCB_LtPckCmpLtRt , fb.Bwd)&&_eFwd == fb.Fwd)
                {
                    sMsg = CL_GetName(ci.QCB_RtPckCmpDnUp) + " 실린더가 버퍼 위치에 있어 충돌되어 못 움직입니다." ;
                    bRet = false ;
                }
            }
            else if (_eActr == ci.QCB_LtPckCmpClOp){}
            else if (_eActr == ci.QCB_LtPckCmpDnUp){}
            else if (_eActr == ci.QCB_LtPckCmpLtRt)
            {
                if(CL_Complete(ci.QCB_LtPckCmpDnUp , fb.Fwd))
                {
                    sMsg = CL_GetName(ci.QCB_RtPckCmpDnUp) + " 실린더가 다운 되어 있습니다." ;
                    bRet = false ;
                }
                if(CL_Complete(ci.QCB_RtPckCmpLtRt , fb.Fwd)&&_eFwd == fb.Bwd)
                {
                    sMsg = CL_GetName(ci.QCB_RtPckCmpLtRt) + " 실린더가 버퍼 위치에 있어 충돌되어 못 움직입니다." ;
                    bRet = false ;
                }
            }
            else if (_eActr == ci.QCB_BfStg1stFtRr){}
            else if (_eActr == ci.QCB_BfStg2ndFtRr){}
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

            //if (_eMotr == mi.PKR_YSutl)
            //{
            //
            //}
            //else if(_eMotr == mi.PKR_ZPckr)
            //{
            //
            //}
            //else
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

            //if (_eMotr == mi.PKR_YSutl)
            //{
            //    if (_eDir == EN_JOG_DIRECTION.Neg) //아래
            //    {
            //
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //    else //위
            //    {
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //}
            //else if (_eMotr == mi.PKR_ZPckr)
            //{
            //    if (_eDir == EN_JOG_DIRECTION.Neg) //아래
            //    {
            //
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //    else //위
            //    {
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //}
            //else
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
            //if( !MT_GetStop(mi.PKR_ZPckr)) return false;
            //if( !MT_GetStop(mi.PKR_YSutl)) return false;
            
            if (!CL_Complete(ci.QCB_RtPckCmpClOp)) return false;
            if (!CL_Complete(ci.QCB_RtPckCmpDnUp)) return false;
            if (!CL_Complete(ci.QCB_RtPckCmpLtRt)) return false;
            if (!CL_Complete(ci.QCB_LtPckCmpClOp)) return false;
            if (!CL_Complete(ci.QCB_LtPckCmpDnUp)) return false;
            if (!CL_Complete(ci.QCB_LtPckCmpLtRt)) return false;
            if (!CL_Complete(ci.QCB_BfStg1stFtRr)) return false;
            if (!CL_Complete(ci.QCB_BfStg2ndFtRr)) return false;

            if(m_tmDelay.GetUsing())return false ;

            return true ;
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
