using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class Adjust : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
            public bool bReqStop   ;
            public bool bSupplied  ; //한번 공급 했음.
            public void Clear()
            {
                bWorkEnd    = false ;
                bReqStop    = false ;
                bSupplied   = false ;
            }
        };   

        public enum sc
        {
            Idle    = 0,
            Run        ,
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
        protected CDelayTimer m_tmDelay1 ;        
        protected CDelayTimer m_tmDelay2 ;        
        protected CDelayTimer m_tmDelay3 ;        

        protected SStat Stat;
        protected SStep Step, PreStep;

        protected double m_dLastIdxPos;
        protected string m_sCheckSafeMsg;

        public CTimer[] m_CycleTime;

        //데이터 디스플레이용 최대 최소 토크값 및 결과값
        public RS485_SeTechYD5010.CResult tRst0;
        public RS485_SeTechYD5010.CResult tRst1;
        public RS485_SeTechYD5010.CResult tRst2;
        public RS485_SeTechYD5010.CResult tRst3;


        public Adjust(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();
            m_tmDelay1    = new CDelayTimer();
            m_tmDelay2    = new CDelayTimer();
            m_tmDelay3    = new CDelayTimer();


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
            m_tmDelay1 .Clear();
            m_tmDelay2 .Clear();
            m_tmDelay3 .Clear();
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

        public bool bAutorun = false;
        public bool bNG      = false;
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
                    //if (OM.EqpStat.iCrntWorkCount + 1 > OM.EqpStat.iTotalWorkCount)
                    //{
                    //    ER_SetErr(ei.ADJ_WorkCountOver, "현재 작업 수량이 설정 작업 수량보다 많습니다.");
                    //    return true;
                    //}
                    //나중에 하던가 말던가 이것만 해놈 시작하기 전에만 체크
                    
                    //if (ML.IO_GetX(xi.AreaSensor))
                    //{
                    //    ML.ER_SetErr(ei.ETC_Emergency, "전방 안전 센서가 감지 중 입니다..");
                    //    Step.iToStart = 0;
                    //    return true;
                    //}
                    if (!ML.IO_GetX(xi.HSensorReady))
                    {
                        ML.ER_SetErr(ei.ADJ_HghtCheck, "높이 측정기가 준비 상태가 아닙니다.");
                        Step.iToStart = 0;
                        return true;
                    }

                    bool bAllNone = DM.ARAY[ri.PRE].CheckAllStat(cs.None) && DM.ARAY[ri.ADJ].CheckAllStat(cs.None) && DM.ARAY[ri.PST].CheckAllStat(cs.None);
                    
                    if (bAllNone && (OM.EqpStat.iCrntWorkCount + 1 > OM.EqpStat.iTotalWorkCount))
                    {
                        DM.ARAY[ri.PRE].SetStat(cs.None);
                        ER_SetErr(ei.ADJ_WorkCountOver, "현재 작업 수량이 설정 작업 수량보다 많습니다.");
                        iCycle_PreC = 0;
                        return true;
                    }

                    InitCycleRun();
                    SEQ.rsNut.SetReset(yi.NutReset, true);

                    m_tmDelay.Clear();
                    Step.iToStart++;
                    return false;

                case 11:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    SEQ.rsNut.SetReset(yi.NutReset, false);

                    Step.iToStart++;
                    return false;

                case 12:
                    int iGetCh = SEQ.rsNut.GetCh();
                    if (SEQ.rsNut.GetCh() != OM.DevInfo.iChannelNo)
                    {
                        SEQ.rsNut.SetCh(OM.DevInfo.iChannelNo);
                    }

                    Step.iToStart++;
                    return false;

                case 13:
                    if(!SEQ.rsNut.ReadEnded()) return false;
                    if (SEQ.rsNut.GetCh() != OM.DevInfo.iChannelNo)
                    {
                        ER_SetErr(ei.ADJ_Communication, "너트런너 채널이 변경되지 않았습니다.");
                        Step.iToStart = 0;
                        return true;
                    }

                    MoveCyl(ci.ADJ_PreStageUpDn , fb.Bwd);
                    MoveCyl(ci.ADJ_PostStageUpDn, fb.Bwd);
                    MoveCyl(ci.ADJ_AdjustUpDn   , fb.Fwd);

                    Step.iToStart++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.ADJ_PreStageUpDn , fb.Bwd)) return false;
                    if (!CL_Complete(ci.ADJ_PostStageUpDn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.ADJ_AdjustUpDn   , fb.Fwd)) return false;

                    bAutorun = true;
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
            //Move Home.
            switch (Step.iToStop)
            {
                default: 
                    Step.iToStop = 0;
                    return true;

                case 10:
                    MoveCyl(ci.ADJ_AdjustUpDn   , fb.Fwd);
                    MoveCyl(ci.ADJ_PreStageUpDn , fb.Bwd);
                    MoveCyl(ci.ADJ_PostStageUpDn, fb.Bwd);

                    Step.iToStop++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.ADJ_AdjustUpDn   , fb.Fwd)) return false;
                    if(!CL_Complete(ci.ADJ_PreStageUpDn , fb.Bwd)) return false;
                    if(!CL_Complete(ci.ADJ_PostStageUpDn, fb.Bwd)) return false;

                    Step.iToStop++;
                    return false;

                case 12:

                    Step.iToStop++;
                    return false;
                
                case 13: 

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

                //사이클
                bool isRun       = bAutorun ;//&& !bNG;
                bool isWorkEnd   = !bAutorun;//bWorkEnd;


                if (ER_IsErr()) return false;

                //Normal Decide Step.
                     if (isRun      ) { DM.ARAY[ri.PRE].Trace(m_sPartName); DM.ARAY[ri.ADJ].Trace(m_sPartName); DM.ARAY[ri.PST].Trace(m_sPartName); Step.eSeq = sc.Run  ; }
                else if (isWorkEnd  ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle){
                    Trace(Step.eSeq.ToString() +" Start");
                    Log.TraceListView(this.ToString() + Step.eSeq.ToString() + " 동작 시작");
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
                case (sc.Run      ): if (!CycleRun  ()) return false; break;

            }
            Trace(sCycle+" End");
            Log.TraceListView(this.ToString() + sCycle + " 동작 종료"); 
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

        public bool bHomeEnd = false;
        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 8000 )) {
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
                    bHomeEnd = false;
                    CL_Move(ci.ADJ_AdjustUpDn   , fb.Fwd);
                    CL_Move(ci.ADJ_PreStageUpDn , fb.Bwd);
                    CL_Move(ci.ADJ_PostStageUpDn, fb.Bwd);
                    
                    Step.iHome++;
                    return false;

                case 11: 
                    if (!CL_Complete(ci.ADJ_AdjustUpDn   , fb.Fwd)) return false;
                    if (!CL_Complete(ci.ADJ_PreStageUpDn , fb.Bwd)) return false;
                    if (!CL_Complete(ci.ADJ_PostStageUpDn, fb.Bwd)) return false;

                    CL_Move(ci.ADJ_AdjTransferFwBw, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.ADJ_AdjTransferFwBw, fb.Bwd)) return false;

                    if (!IO_GetX(xi.ADJ_TurnTableHome))
                    {
                        CL_Move(ci.ADJ_AdjTransferFwBw, fb.Bwd);
                    }
                    Step.iHome++;
                    return false;

                case 13:
                    if (!IO_GetX(xi.ADJ_TurnTableHome)) return false;

                    //IO_SetY(yi.HSensorClear, true);
                    //IO_SetY(yi.HSensorZero , true);
                    IO_SetY(yi.NutReset    , true);

                    Step.iHome++;
                    return false;

                case 14:
                    //if(!IO_GetY(yi.HSensorClear)) return false;
                    //if(!IO_GetY(yi.HSensorZero )) return false;
                    if(!IO_GetY(yi.NutReset    )) return false;

                    //IO_SetY(yi.HSensorClear, false);
                    //IO_SetY(yi.HSensorZero , false);
                    IO_SetY(yi.NutReset    , false);

                    Step.iHome++;
                    return false;

                case 15:
                    //if(IO_GetY(yi.HSensorClear)) return false;
                    //if(IO_GetY(yi.HSensorZero )) return false;
                    if(IO_GetY(yi.NutReset    )) return false;

                    bHomeEnd = true;

                    Step.iHome = 0;
                    return true;
            }
        }

        //대충 쓰다 버릴놈들

        int iCycle    = 0 ;
        int iPreCycle = 0 ;
        public void InitCycleRun  () { iCycle    = 10 ; iPreCycle = 0 ; }

        bool bPreUnkn = false;
        bool bAdjUnkn = false;
        bool bPstUnkn = false;

        //const int iPreCylUpStep = 14;//프리체크에서 실린더 업 하는 스텝
        //const int iPreStartStep = 16;//프리체크에서 높이측정 시작 하는 스텝
        //const int iPstCylUpStep = 12;//포스트체크에서 실린더 업 하는 스텝
        //const int iPstStartStep = 14;//포스트체크에서 높이측정 시작 하는 스텝

        public bool CycleRun()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(iCycle != 0 && iCycle == iPreCycle && CheckStop() && !IO_GetX(xi.NutBusy) && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out iCycle={0:00}", iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (iCycle != iPreCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", iCycle);
                Trace(sTemp);
            }

            iPreCycle = iCycle;

            //if (Stat.bReqStop || (MM.Working() && MM.Stop))
            //{
            //    //if(Step.iCycle == 10 || Step.iCycle == 20 || Step.iCycle == 30 || Step.iCycle == 40)
            //    //{
            //        return true;
            //    //}
            //    
            //}

            int r, c = 0;
            switch (iCycle)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear iCycle={0:00}", iCycle);
                    return true;

                case 10:
                    Log.TraceListView("Run 동작 시작");
                    
                    OM.EqpStat.dSttCycleTime = DateTime.Now.ToOADate();

                    MoveCyl(ci.ADJ_PreStageUpDn , fb.Bwd);
                    MoveCyl(ci.ADJ_AdjustUpDn   , fb.Fwd);
                    MoveCyl(ci.ADJ_PostStageUpDn, fb.Bwd);

                    iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.ADJ_PreStageUpDn , fb.Bwd)) return false;
                    if(!CL_Complete(ci.ADJ_AdjustUpDn   , fb.Fwd)) return false;
                    if(!CL_Complete(ci.ADJ_PostStageUpDn, fb.Bwd)) return false;
                    if (OM.DevInfo.bUseAdjTransfer) MoveCyl(ci.ADJ_AdjTransferFwBw, fb.Fwd);
                    else                            MoveCyl(ci.ADJ_AdjTransferFwBw, fb.Bwd);

                    InitCycleStep();

                    iCycle++;
                    return false;

                case 12:
                    if (OM.DevInfo.bUseAdjTransfer)
                    {
                        if(!CL_Complete(ci.ADJ_AdjTransferFwBw, fb.Fwd)) return false;
                    }
                    else
                    {
                        if(!CL_Complete(ci.ADJ_AdjTransferFwBw, fb.Bwd)) return false; 
                    }
                    //bUnknown이 true이면 턴테이블 안돌림
                    //false이면 돌린다.
                    bool bUnknown = DM.ARAY[ri.PRE].CheckAllStat(cs.Unkn) || DM.ARAY[ri.ADJ].CheckAllStat(cs.Unkn) || DM.ARAY[ri.PST].CheckAllStat(cs.Unkn);

                    if(!bUnknown)
                    {
                        if(!CycleTableTurn()) return false;
                    }

                    iCycle++;
                    return false;

                case 13:
                    InitCyclePreCheck ();
                    InitCycleAdjust   ();
                    InitCyclePostCheck();

                    bPreUnkn = !DM.ARAY[ri.PRE].CheckAllStat(cs.None);
                    bAdjUnkn =  DM.ARAY[ri.ADJ].CheckAllStat(cs.Unkn);
                    bPstUnkn = !DM.ARAY[ri.PST].CheckAllStat(cs.None);// && NG 카운팅을 유형별로 쪼개서 해야하는데 여기저기서 하면 헷갈려서 PostCheck에서 다 할거라 지움 
                               //!DM.ARAY[ri.PST].CheckAllStat(cs.NG1)  &&
                               //!DM.ARAY[ri.PST].CheckAllStat(cs.NG2)  &&
                               //!DM.ARAY[ri.PST].CheckAllStat(cs.NG3)    ;

                    iCycle++;
                    return false;

                case 14:
                    bool c1 = false;
                    bool c2 = false;
                    bool c3 = false;
                    
                    if (bPreUnkn) { c1 = CyclePreCheck (); }
                    if (bAdjUnkn) { c2 = CycleAdjust   (); }
                    if (bPstUnkn) { c3 = CyclePostCheck(); }
                    
                    if (bPreUnkn && !c1) return false;
                    if (bAdjUnkn && !c2) return false;
                    if (bPstUnkn && !c3) return false;
                    
                    bAutorun = false;

                    OM.EqpStat.dEndCycleTime = DateTime.Now.ToOADate();

                    
                    OM.EqpStat.dCycleTime = OM.EqpStat.dEndCycleTime - OM.EqpStat.dSttCycleTime;


                    Log.TraceListView("Run 동작 종료");
                    iCycle = 0  ;
                    return true ;
            }
        }

        //개별 동작
        public bool CycleTableTurn()
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
                    Log.TraceListView("Cycle TurnTable 동작 시작");
                    IO_SetY(yi.OKLamp, false);
                    IO_SetY(yi.NGLamp, false);
                    if (!OM.MstOptn.bDebugMode && MM.GetManNo() == mc.TableTurn)
                    {
                        bool bUnknown = DM.ARAY[ri.PRE].CheckAllStat(cs.Unkn) || DM.ARAY[ri.ADJ].CheckAllStat(cs.Unkn) || DM.ARAY[ri.PST].CheckAllStat(cs.Unkn);
                        if (bUnknown)
                        {
                            Log.ShowMessage("Error", "작업 전 데이터가 남아있는 파트가 있어 턴테이블을 돌릴 수 없습니다.");
                            return true;
                        }
                    }
                    DM.ARAY[ri.PRE].ID = "";
                    MoveCyl(ci.ADJ_PreStageUpDn , fb.Bwd);
                    MoveCyl(ci.ADJ_AdjustUpDn   , fb.Fwd);
                    MoveCyl(ci.ADJ_PostStageUpDn, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.ADJ_PreStageUpDn , fb.Bwd)) return false;
                    if(!CL_Complete(ci.ADJ_PostStageUpDn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.ADJ_AdjustUpDn   , fb.Fwd)) return false;

                    //턴테이블 이니셜
                    if (IO_GetX(xi.ADJ_TurnTableHome)) MoveCyl(ci.ADJ_TurnTableCw, fb.Bwd); //IO_SetY(yi.ADJ_TurnTableInit, true);
                                                                                            //MoveCyl(ci.ADJ_TurnTableCw, fb.Bwd);
                    Step.iCycle++;
                    return false;
                case 12:
                    if (!IO_GetX(xi.ADJ_TurnTableReady))return false;
                    IO_SetY(yi.ADJ_TurnTableInit, false);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    if(IO_GetY(yi.ADJ_TurnTableInit)) return false;
                    MoveCyl(ci.ADJ_TurnTableCw, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.ADJ_TurnTableCw, fb.Fwd)) return false;
                    //if (DM.ARAY[ri.PST].CheckAllStat(cs.Good))
                    //{
                    //    IO_SetY(yi.OKLamp, true );
                    //    IO_SetY(yi.NGLamp, false);
                    //}
                    //else if (DM.ARAY[ri.PST].CheckAllStat(cs.NG))
                    //{
                    //    IO_SetY(yi.OKLamp, false);
                    //    IO_SetY(yi.NGLamp, true );
                    //}
                    //else
                    //{
                    //    IO_SetY(yi.OKLamp, false);
                    //    IO_SetY(yi.NGLamp, false);
                    //}

                    //마지막 검사하고 처음위치에 보내서 확인
                    DM.ShiftData(ri.PST, ri.STT); //초기값넣는게 있어야 겟네

                    switch (DM.ARAY[ri.STT].GetStat(0, 0))
                    {
                        default     : IO_SetY(yi.ETC_HghtNGLp, false); IO_SetY(yi.ETC_HighTqNGLp, false); IO_SetY(yi.ETC_LowTqNGLp, false); break;
                        case cs.NG1 : IO_SetY(yi.ETC_HghtNGLp, true ); IO_SetY(yi.ETC_HighTqNGLp, false); IO_SetY(yi.ETC_LowTqNGLp, false); break; //높이NG
                        case cs.NG2 : IO_SetY(yi.ETC_HghtNGLp, false); IO_SetY(yi.ETC_HighTqNGLp, true ); IO_SetY(yi.ETC_LowTqNGLp, false); break; //상한토크NG
                        case cs.NG3 : IO_SetY(yi.ETC_HghtNGLp, false); IO_SetY(yi.ETC_HighTqNGLp, false); IO_SetY(yi.ETC_LowTqNGLp, true ); break; //하한토크NG
                    }

                         if (DM.ARAY[ri.ADJ].CheckAllStat(cs.None)) { DM.ShiftData(ri.ADJ, ri.PST); DM.ARAY[ri.PST].SetStat(cs.None); }
                    else if (DM.ARAY[ri.ADJ].CheckAllStat(cs.NG1 )) { DM.ShiftData(ri.ADJ, ri.PST); DM.ARAY[ri.PST].SetStat(cs.NG1 ); }
                    else if (DM.ARAY[ri.ADJ].CheckAllStat(cs.NG2 )) { DM.ShiftData(ri.ADJ, ri.PST); DM.ARAY[ri.PST].SetStat(cs.NG2 ); }
                    else if (DM.ARAY[ri.ADJ].CheckAllStat(cs.NG3 )) { DM.ShiftData(ri.ADJ, ri.PST); DM.ARAY[ri.PST].SetStat(cs.NG3 ); }
                    else                                            { DM.ShiftData(ri.ADJ, ri.PST); DM.ARAY[ri.PST].SetStat(cs.Unkn); }

                    if (DM.ARAY[ri.PRE].CheckAllStat(cs.None)) { DM.ShiftData(ri.PRE, ri.ADJ); DM.ARAY[ri.ADJ].SetStat(cs.None); }
                    else                                       { DM.ShiftData(ri.PRE, ri.ADJ); DM.ARAY[ri.ADJ].SetStat(cs.Unkn); }

                    DM.ARAY[ri.PRE].ClearMap();
                    DM.ARAY[ri.PRE].SetStat(cs.Unkn);


                    if (DM.ARAY[ri.STT].CheckAllStat(cs.Good))
                    {
                        OM.EqpStat.iGoodCount++;
                        OM.EqpStat.iCrntWorkCount++;
                    }
                    else if (DM.ARAY[ri.STT].CheckAllStat(cs.NG1)) //토크 NG 외에 너트런너에서 에러띄워서 세운거 전부 높이NG로 때림
                    {
                        OM.EqpStat.iHghtNG++;
                    }
                    else if (DM.ARAY[ri.STT].CheckAllStat(cs.NG2)) //상한토크 NG
                    {
                        OM.EqpStat.iHighTqNG++;
                    }
                    else if (DM.ARAY[ri.STT].CheckAllStat(cs.NG3)) //하한토크 NG
                    {
                        OM.EqpStat.iLowTqNG++;
                    }

                    ////클리어는 SEQ.InspectLightGrid()
                    //if(DM.ARAY[ri.STT].CheckAllStat(cs.NG1) || DM.ARAY[ri.STT].CheckAllStat(cs.NG2) || DM.ARAY[ri.STT].CheckAllStat(cs.NG3))
                    //{
                    //    bNG = true;
                    //}

                    //bShowResult = true ; //!DM.ARAY[ri.STT].CheckAllStat(cs.None);

                    //if (DM.ARAY[ri.ADJ].CheckAllStat(cs.Work))
                    //{
                    //    DM.ARAY[ri.PST].Step = DM.ARAY[ri.ADJ].Step;
                    //    DM.ARAY[ri.PST].Data = DM.ARAY[ri.ADJ].Data;
                    //    DM.ARAY[ri.PST].SetStat(cs.Unkn);
                    //}
                    //    
                    //if (DM.ARAY[ri.PRE].CheckAllStat(cs.Work))
                    //{
                    //    DM.ARAY[ri.ADJ].Step = DM.ARAY[ri.PRE].Step;
                    //    DM.ARAY[ri.ADJ].Data = DM.ARAY[ri.PRE].Data;
                    //    OM.EqpStat.dAdjustHeight = OM.EqpStat.dPreCheckGap; //Step에 높이측정 편차 데이터 저장
                    //    DM.ARAY[ri.ADJ].SetStat(cs.Unkn);
                    //}
                    //    
                    //DM.ARAY[ri.PRE].SetStat(cs.Unkn);

                    Step.iCycle = 0;
                    return true;
            }
        }

        //개별 동작
        //public int iCrntWorkCount; //자재 감지센서가 없어서 PreCheck에서 측정 하고 자재 있으면 카운팅해야한다. LotOpen했을때 입력한 카운팅 넘어갔을때 에러도 여기서 띄움

        int iCycle_PreC    = 0;
        int iPreCycle_PreC = 0;
        public void InitCyclePreCheck() { iCycle_PreC = 10; iPreCycle_PreC = 0; }

        bool bFstCheck = false;

        double dEmptyHeight1 = 0.0 ;
        double dEmptyHeight2 = 0.0 ;
        public bool CyclePreCheck()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(iCycle_PreC != 0 && iCycle_PreC == iPreCycle_PreC && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                //sTemp = string.Format("Time Out Step.iCycle={0:00}", iCycle_PreC);
                sTemp = string.Format("Time Out iCycle_PreC={0:00}", iCycle_PreC);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                MoveCyl(ci.ADJ_PreStageUpDn, fb.Bwd);
                Trace(sTemp);
                return true;
            }

            if (iCycle_PreC != iPreCycle_PreC)
            {
                sTemp = string.Format("Cycle iCycle_PreC={0:00}", iCycle_PreC);
                Trace(sTemp);
            }

            iPreCycle_PreC = iCycle_PreC;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r, c = 0;
            switch (iCycle_PreC)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iCycle_PreC={0:00}", iCycle_PreC);
                    return true;

                case 10:
                    Log.TraceListView("조절 전 높이측정 동작 시작");
                    SEQ.rsHeight.Init();

                    m_tmDelay.Clear();
                    iCycle_PreC++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(200)) return false;
                    if (IO_GetX(xi.HSensorReady)) SEQ.rsHeight.Start();
                    iCycle_PreC++;
                    return false;

                case 12:
                    if (SEQ.rsHeight.sErr != "")
                    {
                        ER_SetErr(ei.ADJ_HghtCheck, SEQ.rsHeight.sErr);
                        MoveCyl(ci.ADJ_PreStageUpDn, fb.Bwd);
                        return true;
                    }
                    if (!SEQ.rsHeight.End()) return false;

                    //자제 없음 감지용.
                    dEmptyHeight1 = SEQ.rsHeight.sHeight.dLeft1 + 0.1 ;
                    dEmptyHeight2 = SEQ.rsHeight.sHeight.dLeft2 + 0.1 ;


                    MoveCyl(ci.ADJ_PreStageUpDn , fb.Bwd);
                    OM.EqpStat.dPreCheckGap = 0;

                    iCycle_PreC++;
                    return false;
                    
                case 13:
                    if(!CL_Complete(ci.ADJ_PreStageUpDn , fb.Bwd)) return false;
                    SEQ.rsHeight.Init();

                    m_tmDelay.Clear();
                    iCycle_PreC++;
                    return false ;

                //프리체크 포스트체크 실린더 업 타이밍때문에 여기 스텝 올때까지 기다린다.
                case 14:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    MoveCyl(ci.ADJ_PreStageUpDn, fb.Fwd);
                   
                    iCycle_PreC++;
                    return false;
                    
                case 15:
                    if(!CL_Complete(ci.ADJ_PreStageUpDn, fb.Fwd)) return false;
                    
                    m_tmDelay.Clear();
                    iCycle_PreC++;
                    return false;
                
                //프리체크, 포스트 체크 실린더 업과 높이 측정 Start 타이밍 잡기 위해 스텝 추가.
                //이 스텝 올때까지 기다리고 동시 작업 시킨다.
                case 16:
                    if(!m_tmDelay.OnDelay(100)) return false;

                    iCycle_PreC++;
                    return false;

                case 17:
                    if(!IO_GetX(xi.HSensorReady)) return false ; //레디를 보면서 혹시 체결후 검사와 체결전 검사가 겹쳤을때 상황을 본다.
                       
                    m_tmDelay.Clear();
                    iCycle_PreC++;
                    return false ;

                case 18:
                    if(!m_tmDelay.OnDelay(30))return false;//체결후 검사와 체결전 검사 혹시라도 겹칠까봐 일단 통신송수신 속도인 20ms보다 좀더 준다.

                    SEQ.rsHeight.Start();
                    iCycle_PreC++;
                    return false;

                case 19:
                    if(SEQ.rsHeight.sErr != "") 
                    {
                        ER_SetErr(ei.ADJ_HghtCheck, SEQ.rsHeight.sErr);
                        MoveCyl(ci.ADJ_PreStageUpDn, fb.Bwd);
                        return true;
                    }
                    if(!SEQ.rsHeight.End()) return false;
                    double dLeft1  = SEQ.rsHeight.sHeight.dLeft1;
                    double dLeft2  = SEQ.rsHeight.sHeight.dLeft2;

                    //센서값은 볼트가 높을수록 플러스.
                    DM.ARAY[ri.PRE].Data.dData1 = SEQ.rsHeight.sHeight.dLeft1;
                    DM.ARAY[ri.PRE].Data.dData2 = SEQ.rsHeight.sHeight.dLeft2;

                    //자제 없음.
                    //처음에 or 조건으로 되어있었으나
                    //자재 있고 한쪽만 터치 안됐을 경우에 작업자가 모니터를 안봐서
                    //OK로 빠진건지 None인 상태로 그냥 흘러온건지 확인을 안해서
                    //그냥 한쪽이라도 감지 되면 작업 가능하도록 수정
                    if (dLeft1 < dEmptyHeight1 && dLeft2 < dEmptyHeight2)
                    {
                        iCycle_PreC = 30;
                        return false;
                    }

                    //OM.EqpStat.dPreCheckGap = -dLeft2 + dLeft1;
                    OM.EqpStat.dPreCheckGap = dLeft2 -dLeft1;


                    Log.TraceListView("dLeft1" + dLeft1.ToString());
                    Log.TraceListView("dLeft2" + dLeft2.ToString());
                    Log.TraceListView(OM.EqpStat.dPreCheckGap.ToString());
                    m_tmDelay.Clear();
                    iCycle_PreC++;
                    return false;

                case 20:
                    if (!m_tmDelay.OnDelay(500)) return false;

                    MoveCyl(ci.ADJ_PreStageUpDn, fb.Bwd);

                    iCycle_PreC++;
                    return false;

                case 21:
                    if(!CL_Complete(ci.ADJ_PreStageUpDn, fb.Bwd)) return false;
                    
                    DM.ARAY[ri.PRE].SetStat(cs.Work);

                    //고객사에서 양품 수량이 토탈 카운트에 다다르면 끝낸다고 해서 수정
                    //카운팅을 턴테이블 돌리는데서 할꺼임
                    //if (OM.EqpStat.iCrntWorkCount + 1 > OM.EqpStat.iTotalWorkCount)
                    //{
                    //    DM.ARAY[ri.PRE].SetStat(cs.None);
                    //    ER_SetErr(ei.ADJ_WorkCountOver, "현재 작업 수량이 설정 작업 수량보다 많습니다.");
                    //    iCycle_PreC = 0;
                    //    return true;
                    //}
                    //OM.EqpStat.iCrntWorkCount++;// = iCrntWorkCount;
                    //DM.ARAY[ri.PRE].Step = OM.EqpStat.iCrntWorkCount;

                    iCycle_PreC = 0;
                    return true;

                //위에서 씀
                //측정후 계산 데이터가 0이면
                case 30:
                    MoveCyl(ci.ADJ_PreStageUpDn, fb.Bwd);
                    
                    iCycle_PreC++;
                    return false;

                case 31:
                    if(!CL_Complete(ci.ADJ_PreStageUpDn, fb.Bwd)) return false;
                    DM.ARAY[ri.PRE].SetStat(cs.None);

                    iCycle_PreC = 0 ;
                    return true ;
            }
        }

        int iCycle_Adjust = 0;
        int iPreCycle_Adjust = 0;
        public void InitCycleAdjust() { iCycle_Adjust = 10; iPreCycle_Adjust = 0; }

        //추가 작업하려고 사용
        int iAddWorkCnt = 0;
        public bool CycleAdjust()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(iCycle_Adjust != 0 && iCycle_Adjust == iPreCycle_Adjust && CheckStop() && !IO_GetX(xi.NutBusy) && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out iCycle_Adjust={0:00}", iCycle_Adjust);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);
                Trace(sTemp);
                return true;
            }

            if (iCycle_Adjust != iPreCycle_Adjust)
            {
                sTemp = string.Format("Cycle iCycle_Adjust={0:00}", iCycle_Adjust);
                Trace(sTemp);
            }

            iPreCycle_Adjust = iCycle_Adjust;

            if (Stat.bReqStop)
            {
                //return true ;
            }
            int r, c = 0;
            switch (iCycle_Adjust)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iCycle_Adjust={0:00}", iCycle_Adjust);
                    return true;

                case 10:
                    Log.TraceListView("높이조절 동작 시작");
                    
                    //두번하면 우측 가서 검사하면서 마이크로미터 부러짐
                    DM.ARAY[ri.ADJ].SetStat(cs.NG1);
                    //Init
                    iAddWorkCnt = 0;

                    //토크 에러 발생 시 체결토크 0이면 정상, 0이 아니면 토크 에러 발생한 것으로 사용
                    DM.ARAY[ri.ADJ].Data.dLowTq = 0;

                    //슬라이드 자제 검증.
                    OM.EqpStat.dAdjustHeight = -OM.DevInfo.dWorkOfs - (DM.ARAY[ri.ADJ].Data.dData1 - DM.ARAY[ri.ADJ].Data.dData2);
                    
                    //OM.EqpStat.dAdjustHeight = -OM.DevInfo.dWorkOfs - (DM.ARAY[ri.ADJ].Data.dData1 - DM.ARAY[ri.ADJ].Data.dData2);
                    //OM.EqpStat.dAdjustHeight = (DM.ARAY[ri.ADJ].Data.dData2 - DM.ARAY[ri.ADJ].Data.dData1)+ OM.DevInfo.dWorkOfs ;

                    MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);
                    iCycle_Adjust++;
                    return false;
                    
                case 11:
                    if(!CL_Complete(ci.ADJ_AdjustUpDn, fb.Fwd)) return false;
                 

                    iCycle_Adjust++;
                    return false;

                case 12:

                    if (OM.DevInfo.bUseAdjTransfer) MoveCyl(ci.ADJ_AdjTransferFwBw, fb.Fwd);
                    else                            MoveCyl(ci.ADJ_AdjTransferFwBw, fb.Bwd);

                    iCycle_Adjust++;
                    return false;

                case 13:
                    if (OM.DevInfo.bUseAdjTransfer)
                    {
                        if(!CL_Complete(ci.ADJ_AdjTransferFwBw, fb.Fwd)) return false;
                    }
                    else
                    {
                        if(!CL_Complete(ci.ADJ_AdjTransferFwBw, fb.Bwd)) return false; 
                    }

                    MoveCyl(ci.ADJ_AdjustUpDn, fb.Bwd);

                    iCycle_Adjust++;
                    return false ;

                case 14:
                    if (IO_GetX(xi.ADJ_AdjustUpDnBuff)) IO_SetY(yi.ADJ_AdjustUpDnBuff, true);
                    if (!CL_Complete(ci.ADJ_AdjustUpDn, fb.Bwd)) return false;
                    IO_SetY(yi.ADJ_AdjustUpDnBuff, false);

                    iCycle_Adjust++;
                    return false;

                case 15:
                    if(OM.DevInfo.iBoltFindOptn == 0) //본체결 +30도 -60도 +30도 사용하는 옵션
                    {
                        iCycle_Adjust = 30;
                        return false;
                    }
                    
                    //FirstStage 사용하는 옵션
                    iCycle_Adjust = 20;
                    return false;

                //위에서씀
                //FirstStage 사용하는 옵션
                case 20:

                    //본체결 방향에 맞추어 가체결.
                    SEQ.rsNut.CycleFirstStage(true , OM.EqpStat.dAdjustHeight < 0 );

                    iCycle_Adjust++;
                    return false;

                case 21:
                    if (!SEQ.rsNut.CycleFirstStage()) return false;
                    if (SEQ.rsNut.GetErr() != "")
                    {
                        ER_SetErr(ei.ADJ_Communication, SEQ.rsNut.GetErr());
                        MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);
                        //DM.ARAY[ri.ADJ].SetStat(cs.NG2);
                        //DM.ARAY[ri.ADJ].Data.sResultNg = SEQ.rsNut.GetErr();
                        iCycle_Adjust = 0;
                        return true;
                    }

                    iCycle_Adjust = 40;
                    return false;

                //위에서 씀
                //옵션체결 +30도 -60도 +30도 사용하는 옵션 30도 안되서
                //+61도 -61도
                case 30:
                    //사용안할때는 맥스 토크 100으로 넣음 
                    //SEQ.rsNut.CycleSecondStage(true,(OM.EqpStat.dAdjustHeight > 0) ? 300 : -300,0,0,1000);
                    SEQ.rsNut.CycleSecondStage(true, (OM.EqpStat.dAdjustHeight > 0) ? (int)(OM.CmnOptn.dFindBoltWork1 * 10) : -(int)(OM.CmnOptn.dFindBoltWork1 * 10), 0, 0, 1000);
                    iCycle_Adjust++;
                    return false;

                case 31:
                    if (!SEQ.rsNut.CycleSecondStage()) return false;
                    if (SEQ.rsNut.GetErr() != "")
                    {
                        //ER_SetErr(ei.ADJ_Communication, SEQ.rsNut.GetErr());
                        //MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);
                        //DM.ARAY[ri.ADJ].SetStat(cs.NG2);
                        //DM.ARAY[ri.ADJ].Data.sResultNg = SEQ.rsNut.GetErr();
                        iCycle_Adjust = 100;
                        return false;
                    }
                    iCycle_Adjust++;
                    return false;

                case 32:
                    //SEQ.rsNut.CycleSecondStage(true, (OM.EqpStat.dAdjustHeight > 0) ? -450 : 450,0,0,1000);
                    SEQ.rsNut.CycleSecondStage(true, (OM.EqpStat.dAdjustHeight > 0) ? -(int)(OM.CmnOptn.dFindBoltWork2 * 10) : (int)(OM.CmnOptn.dFindBoltWork2 * 10), 0, 0, 1000);
                    
                    iCycle_Adjust++;
                    return false;

                case 33:
                    if (!SEQ.rsNut.CycleSecondStage()) return false;
                    if (SEQ.rsNut.GetErr() != "")
                    {
                        //ER_SetErr(ei.ADJ_Communication, SEQ.rsNut.GetErr());
                        //MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);
                        //DM.ARAY[ri.ADJ].SetStat(cs.NG2);
                        //DM.ARAY[ri.ADJ].Data.sResultNg = SEQ.rsNut.GetErr();
                        iCycle_Adjust = 100;
                        return false;
                    }

                    iCycle_Adjust++;
                    return false;

                case 34:
                    //SEQ.rsNut.CycleSecondStage(true, (OM.EqpStat.dAdjustHeight > 0) ? 450 : -450,0,0,1000);
                    SEQ.rsNut.CycleSecondStage(true, (OM.EqpStat.dAdjustHeight > 0) ? (int)(OM.CmnOptn.dFindBoltWork3 * 10) : -(int)(OM.CmnOptn.dFindBoltWork3 * 10), 0, 0, 1000);

                    iCycle_Adjust++;
                    return false;
                
                case 35:
                    if (!SEQ.rsNut.CycleSecondStage()) return false;
                    if (SEQ.rsNut.GetErr() != "")
                    {
                        //ER_SetErr(ei.ADJ_Communication, SEQ.rsNut.GetErr());
                        //MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);
                        //DM.ARAY[ri.ADJ].SetStat(cs.NG2);
                        //DM.ARAY[ri.ADJ].Data.sResultNg = SEQ.rsNut.GetErr();
                        iCycle_Adjust = 100;
                        return false;
                    }
                
                    iCycle_Adjust = 40;
                    return false;
                
                //본체결==============================================================================
                case 40:
                    int iTWork = 0;
                    int iRWork = 0;//(int)(OM.DevInfo.dNutLastMotn * 100);
                    int iMin   = 0;
                    int iMax   = 0;
                    if (iAddWorkCnt == 0)
                    {
                        //각도 데이터가 소수점 첫째자리까지만 표현됨
                        //만약 99.3도를 돌리고 싶으면 993을 날려주면 된다.
                        double dTWork1 = (OM.EqpStat.dAdjustHeight / OM.DevInfo.dBoltPitch) * 360;
                        iTWork = (int)(dTWork1 * 10);
                        //고객사 요청
                        //본체결에서 토크가 6이상 나오는 자재가 많아 작업 자체가 안되는 경우가 많음
                        //본체결 시 너트런너가 사용 가능한 최대 토크로 돌려버리고
                        //토크 측정은 옵션 체결에서 하도록 요청하여 수정
                        iMax = (int)(OM.DevInfo.dBfMaxTq * 10);
                        //if (OM.EqpStat.dAdjustHeight > 0) iTWork = (int)(dTWork1 * 10) + (int)(OM.DevInfo.dNutLastMotn * 10);
                        //else                             iTWork = (int)(dTWork1 * 10) - (int)(OM.DevInfo.dNutLastMotn * 10);
                    }
                    else{
                        if (OM.DevInfo.bAddWork)
                        {
                            if (iAddWorkCnt == 1) iTWork = (int)(OM.DevInfo.dNutWorkOptn1 * 10);
                            if (iAddWorkCnt == 2) iTWork = (int)(OM.DevInfo.dNutWorkOptn2 * 10);
                            if (iAddWorkCnt == 3) iTWork = (int)(OM.DevInfo.dNutWorkOptn3 * 10);
                            //고객사 요청
                            //본체결에서 토크가 6이상 나오는 자재가 많아 작업 자체가 안되는 경우가 많음
                            //본체결 시 너트런너가 사용 가능한 최대 토크로 돌려버리고
                            //토크 측정은 옵션 체결에서 하도록 요청하여 수정
                            iMax = (int)(OM.DevInfo.dMaxTq * 10);
                            //if(OM.DevInfo.dNutWorkOptn3 > 0) iTWork = (int)(OM.DevInfo.dNutWorkOptn3 * 10) + (int)(OM.DevInfo.dNutLastMotn * 10);
                            //else                             iTWork = (int)(OM.DevInfo.dNutWorkOptn3 * 10) - (int)(OM.DevInfo.dNutLastMotn * 10);
                            //iTWork = (int)(OM.DevInfo.dNutWorkOptn3 * 10) + (int)(OM.DevInfo.dNutLastMotn * 10);
                            //}
                        }
                    }
                    if (!OM.DevInfo.bAddWork)
                    {
                        iRWork = (int)(OM.DevInfo.dNutLastMotn * 10);
                    }
                    else
                    {
                        if (iAddWorkCnt == 3)
                        {
                            iRWork = (int)(OM.DevInfo.dNutLastMotn * 10);
                        }
                    }
                    iMin = (int)(OM.DevInfo.dMinTq * 10);
                    SEQ.rsNut.CycleSecondStage(true, iTWork, iRWork, iMin, iMax);

                    iCycle_Adjust++;
                    return false;

                case 41:
                    if(!SEQ.rsNut.CycleSecondStage()) return false;
                    //tRst0 = SEQ.rsNut.sResult;

                         if (iAddWorkCnt == 0) tRst0 = SEQ.rsNut.sResult;
                    else if (iAddWorkCnt == 1) tRst1 = SEQ.rsNut.sResult;
                    else if (iAddWorkCnt == 2) tRst2 = SEQ.rsNut.sResult;
                    else if (iAddWorkCnt == 3) tRst3 = SEQ.rsNut.sResult;

                    if (iAddWorkCnt == 0) DM.ARAY[ri.ADJ].Data.dTgtTq0 = tRst0.iTorque;
                    if (iAddWorkCnt == 0) DM.ARAY[ri.ADJ].Data.dMaxTq0 = tRst0.iMaxTorque;
                    if (iAddWorkCnt == 1) DM.ARAY[ri.ADJ].Data.dTgtTq1 = tRst1.iTorque;
                    if (iAddWorkCnt == 1) DM.ARAY[ri.ADJ].Data.dMaxTq1 = tRst1.iMaxTorque;
                    if (iAddWorkCnt == 2) DM.ARAY[ri.ADJ].Data.dTgtTq2 = tRst2.iTorque;
                    if (iAddWorkCnt == 2) DM.ARAY[ri.ADJ].Data.dMaxTq2 = tRst2.iMaxTorque;
                    if (iAddWorkCnt == 3) DM.ARAY[ri.ADJ].Data.dTgtTq3 = tRst3.iTorque;
                    if (iAddWorkCnt == 3) DM.ARAY[ri.ADJ].Data.dMaxTq3 = tRst3.iMaxTorque;

                    if (SEQ.rsNut.GetErr() != "")
                    {
                        //ER_SetErr(ei.ADJ_Communication, SEQ.rsNut.GetErr());
                        //MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);
                        //if(DM.ARAY[ri.ADJ].Data.dMaxTq0/1000 >= OM.DevInfo.dMaxTq/10)
                        //{
                        //    DM.ARAY[ri.ADJ].SetStat(cs.NG2);
                        //}
                        //else if(DM.ARAY[ri.ADJ].Data.dTgtTq0/1000 < OM.DevInfo.dMinTq/10)
                        //{
                        //    DM.ARAY[ri.ADJ].SetStat(cs.NG3);
                        //}
                        
                        //0번 본체결에서 High Torque로 NG 발생했을때 처리하는게 없어서 높이 NG로 뺀다.
                        bool bHghtNG   = DM.ARAY[ri.ADJ].Data.dMaxTq0 / 100 >= OM.DevInfo.dBfMaxTq;
                        //여기는 1~3번까지 측정 체결에서 High/Low Torque 처리
                        bool bHighTqNG = DM.ARAY[ri.ADJ].Data.dMaxTq1 / 100 >= OM.DevInfo.dMaxTq || DM.ARAY[ri.ADJ].Data.dMaxTq2 / 100 >= OM.DevInfo.dMaxTq ||
                                         DM.ARAY[ri.ADJ].Data.dMaxTq3 / 100 >= OM.DevInfo.dMaxTq;
                        bool bLowTqNG  = DM.ARAY[ri.ADJ].Data.dTgtTq1 / 100 <= OM.DevInfo.dMinTq || DM.ARAY[ri.ADJ].Data.dTgtTq2 / 100 <= OM.DevInfo.dMinTq ||
                                         DM.ARAY[ri.ADJ].Data.dTgtTq3 / 100 <= OM.DevInfo.dMinTq;

                        //토크 에러 발생 시 체결토크 0이면 정상, 0이 아니면 토크 에러 발생한 것으로 사용
                        if (iAddWorkCnt == 1) { DM.ARAY[ri.ADJ].Data.dLowTq = DM.ARAY[ri.ADJ].Data.dTgtTq1 ; }
                        if (iAddWorkCnt == 2) { DM.ARAY[ri.ADJ].Data.dLowTq = DM.ARAY[ri.ADJ].Data.dTgtTq2 ; }
                        if (iAddWorkCnt == 3) { DM.ARAY[ri.ADJ].Data.dLowTq = DM.ARAY[ri.ADJ].Data.dTgtTq3 ; }

                        if (bHghtNG)
                        {
                            DM.ARAY[ri.ADJ].SetStat(cs.NG1);
                        }
                        else if (bHighTqNG)
                        {
                            DM.ARAY[ri.ADJ].SetStat(cs.NG2);
                        }
                        else if (bLowTqNG)
                        {
                            DM.ARAY[ri.ADJ].SetStat(cs.NG3);
                        }

                        //DM.ARAY[ri.ADJ].Data.sResultNg = SEQ.rsNut.GetErr();
                        iCycle_Adjust = 100;
                        return false;
                    }

                    //m_tmDelay.Clear();
                    iCycle_Adjust++;
                    return false;

                case 42:
                    //if(!m_tmDelay.OnDelay(1000)) return false; //체결이 다 끝나기 전에 Complete 들어와서 돌리면서 실린더 올려서 딜레이 집어넣음. 진섭

                    if (iAddWorkCnt == 0)
                    {
                        OM.EqpStat.dSStgFastenTorq = SEQ.rsNut.GetResult().iTorque   /100.0;//데이터가 소수점 둘째자리로 표현되서 나누기100함
                        OM.EqpStat.dSStgMaxTorq    = SEQ.rsNut.GetResult().iMaxTorque/100.0;//데이터가 소수점 둘째자리로 표현되서 나누기100함
                    }
                    else
                    {
                        if (OM.DevInfo.bAddWork)
                        {
                            if (iAddWorkCnt == 1) { OM.EqpStat.dOptnFastenTorq1 = SEQ.rsNut.sResult.iTorque / 100.0; OM.EqpStat.dOptnMaxTorq1 = SEQ.rsNut.sResult.iMaxTorque / 100.0; }
                            if (iAddWorkCnt == 2) { OM.EqpStat.dOptnFastenTorq2 = SEQ.rsNut.sResult.iTorque / 100.0; OM.EqpStat.dOptnMaxTorq2 = SEQ.rsNut.sResult.iMaxTorque / 100.0; }
                            if (iAddWorkCnt == 3) { OM.EqpStat.dOptnFastenTorq3 = SEQ.rsNut.sResult.iTorque / 100.0; OM.EqpStat.dOptnMaxTorq3 = SEQ.rsNut.sResult.iMaxTorque / 100.0; }
                        }
                    }
                    iAddWorkCnt++;

                    if(iAddWorkCnt > 3 || !OM.DevInfo.bAddWork)
                    {
                        iCycle_Adjust=80;
                        return false;
                    }
                    iCycle_Adjust=40;
                    return false;


                //위에서 씀
                //마무리
                case 80:
                    MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);

                    iCycle_Adjust++;
                    return false;

                case 81:
                    if(!CL_Complete(ci.ADJ_AdjustUpDn, fb.Fwd)) return false;
                    DM.ARAY[ri.ADJ].SetStat(cs.Work);

                    iCycle_Adjust=0;
                    return true;

                //위에서씀
                //너트런너 에러 발생 시 처리
                case 100:
                    SEQ.rsNut.CycleSecondStage(true, 0, (int)(OM.DevInfo.dNutLastMotn * 10), 0, 1000);

                    iCycle_Adjust++;
                    return false;

                case 101:
                    if(!SEQ.rsNut.CycleSecondStage()) return false;
                    MoveCyl(ci.ADJ_AdjustUpDn, fb.Fwd);

                    iCycle_Adjust++;
                    return false;

                case 102:
                    if(!CL_Complete(ci.ADJ_AdjustUpDn, fb.Fwd)) return false;

                    DM.ARAY[ri.ADJ].Data.sResultNg = SEQ.rsNut.GetErr();
                    iCycle_Adjust = 0;
                    return true;

            }
        }

        //public int iGoodCount;
        //public int iNGCount  ;

        int iCycle_PostC = 0;
        int iPreCycle_PostC = 0;
        public void InitCyclePostCheck() { iCycle_PostC = 10; iPreCycle_PostC = 0; }

        public bool CyclePostCheck()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(iCycle_PostC != 0 && iCycle_PostC == iPreCycle_PostC && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out iCycle_PostC={0:00}", iCycle_PostC);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                MoveCyl(ci.ADJ_PostStageUpDn, fb.Bwd);
                Trace(sTemp);
                return true;
            }

            if (iCycle_PostC != iPreCycle_PostC)
            {
                sTemp = string.Format("Cycle iCycle_PostC={0:00}", iCycle_PostC);
                Trace(sTemp);
            }

            iPreCycle_PostC = iCycle_PostC;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r, c = 0;
            switch (iCycle_PostC)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iCycle_PostC={0:00}", iCycle_PostC);
                    return true;

                case 10:
                    Log.TraceListView("조절 후 높이측정 동작 시작");
                    //NG 처리
                    if(DM.ARAY[ri.PST].CheckAllStat(cs.NG1) || DM.ARAY[ri.PST].CheckAllStat(cs.NG2) || DM.ARAY[ri.PST].CheckAllStat(cs.NG3))
                    {
                        iCycle_PostC = 50;
                        return false;
                    }
                    
                    MoveCyl(ci.ADJ_PostStageUpDn, fb.Bwd);
                    OM.EqpStat.dPostCheckGap = 0;

                    iCycle_PostC++;
                    return false;
                    
                case 11:
                    if(!CL_Complete(ci.ADJ_PostStageUpDn, fb.Bwd)) return false;
                    SEQ.rsHeight.Init();

                    m_tmDelay.Clear();
                    iCycle_PostC++;
                    return false;

                //프리체크 포스트체크 실린더 업 타이밍때문에 여기 스텝 올때까지 기다린다.
                case 12:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    MoveCyl(ci.ADJ_PostStageUpDn, fb.Fwd);

                    iCycle_PostC++;
                    return false;
                    
                case 13:
                    if (!CL_Complete(ci.ADJ_PostStageUpDn, fb.Fwd)) return false;

                    m_tmDelay.Clear();
                    iCycle_PostC++;
                    return false;

                //프리체크, 포스트 체크 실린더 업과 높이 측정 Start 타이밍 잡기 위해 스텝 추가.
                //이 스텝 올때까지 기다리고 동시 작업 시킨다.
                case 14:
                    if(!m_tmDelay.OnDelay(100)) return false;
                    
                    iCycle_PostC++;
                    return false;
                
                case 15:
                    if (!IO_GetX(xi.HSensorReady)) return false ;
                    
                    SEQ.rsHeight.Start();

                    iCycle_PostC++;
                    return false;
                    
                case 16:
                    if ( SEQ.rsHeight.sErr != "") {
                        ER_SetErr(ei.ADJ_HghtCheck, SEQ.rsHeight.sErr);
                        MoveCyl(ci.ADJ_PostStageUpDn, fb.Bwd);
                        return true;
                    }
                    if (!SEQ.rsHeight.End     ()) return false;
                    double dRight1  = SEQ.rsHeight.sHeight.dRight1;
                    double dRight2  = SEQ.rsHeight.sHeight.dRight2;

                    DM.ARAY[ri.PST].Data.dData3 = SEQ.rsHeight.sHeight.dRight1;
                    DM.ARAY[ri.PST].Data.dData4 = SEQ.rsHeight.sHeight.dRight2;

                    double dGap = dRight2-dRight1;
                    Log.TraceListView(dGap.ToString());
                    //양품 수량이 총 수량과 같아질때 Lot End 시켜달라고 해서 여기서
                    //양품/NG카운팅 할 이유가 없어짐
                    //턴테이블에서 카운팅함(STT 어레이에 있는 데이터로 확인)
                    if(!DM.ARAY[ri.PST].CheckAllStat(cs.None))
                    {
                        if(Math.Abs(dGap) <= OM.DevInfo.dCheckTolerance) {
                            DM.ARAY[ri.PST].SetStat(cs.Good);
                            //OM.EqpStat.iGoodCount++;
                        }
                        else {
                            DM.ARAY[ri.PST].SetStat(cs.NG1);
                            //OM.EqpStat.iHghtNG++;
                        }
                    }

                    //OM.EqpStat.dPostCheckGap = dGap;
                    //Log.TraceListView(OM.EqpStat.dPostCheckGap.ToString());
                    m_tmDelay.Clear();

                    iCycle_PostC++;
                    return false;
                    
                case 17:
                    if(!m_tmDelay.OnDelay(500)) return false;
                    MoveCyl(ci.ADJ_PostStageUpDn, fb.Bwd);
                    
                    iCycle_PostC++;
                    return false;

                case 18:
                    if(!CL_Complete(ci.ADJ_PostStageUpDn, fb.Bwd)) return false;

                    iCycle_PostC = 50;
                    return false;

                //위에서씀
                //너트런너에서 NG로 넘어온거 카운팅 및 처리
                case 30:
                    //if (DM.ARAY[ri.PST].CheckAllStat(cs.NG1)) //토크 NG 외에 너트런너에서 에러띄워서 세운거 전부 높이NG로 때림
                    //{
                    //    OM.EqpStat.iHghtNG++;
                    //}
                    //else if (DM.ARAY[ri.PST].CheckAllStat(cs.NG2)) //상한토크 NG
                    //{
                    //    OM.EqpStat.iHighTqNG++;
                    //}
                    //else if (DM.ARAY[ri.PST].CheckAllStat(cs.NG3)) //하한토크 NG
                    //{
                    //    OM.EqpStat.iLowTqNG++;
                    //}

                    iCycle_PostC = 50;
                    return false;



                //위에서 씀
                //마지막 데이터 저장
                case 50:
                    if (!DM.ARAY[ri.PST].CheckAllStat(cs.None)) {
                        //SPC.LOT.SaveDataIni(true); //Lot End 전에 안보이는 것 땜에 넣음 업데이트로 감.
                        SPC.SUB.SaveData(SPC.LOT.Data.StartedAt, LOT.GetLotNo(), new CSpcSubUnit.TLotData()
                        {
                            WorkNo         = DM.ARAY[ri.PST].Step                               ,
                            Result         = DM.ARAY[ri.PST].CheckAllStat(cs.Good) ? "OK" : "NG",
                            ResultNg       = DM.ARAY[ri.PST].Data.sResultNg,
                            //TargetToque    = DM.ARAY[ri.PST].Data.dTgtTq0 / 1000  ,
                            TargetToque    = DM.ARAY[ri.PST].Data.dLowTq == 0 ? DM.ARAY[ri.PST].Data.dTgtTq3 / 1000 : DM.ARAY[ri.PST].Data.dLowTq / 1000,
                            MaxToque       = DM.ARAY[ri.PST].Data.dMaxTq0 / 1000  ,
                            PostCheckGap   = Math.Truncate((DM.ARAY[ri.PST].Data.dData3 - DM.ARAY[ri.PST].Data.dData4) * 10000) / 10000,
                            //TargetToque1   = DM.ARAY[ri.PST].Data.dTgtTq1 / 1000  ,
                            //MaxToque1      = DM.ARAY[ri.PST].Data.dMaxTq1 / 1000  ,
                            //TargetToque2   = DM.ARAY[ri.PST].Data.dTgtTq2 / 1000  ,
                            //MaxToque2      = DM.ARAY[ri.PST].Data.dMaxTq2 / 1000  ,
                            //TargetToque3   = DM.ARAY[ri.PST].Data.dTgtTq3 / 1000  ,
                            //MaxToque3      = DM.ARAY[ri.PST].Data.dMaxTq3 / 1000  ,
                            //HeightData1    = DM.ARAY[ri.PST].Data.dData1   ,
                            //HeightData2    = DM.ARAY[ri.PST].Data.dData2   ,  
                            HeightData3    = DM.ARAY[ri.PST].Data.dData3   ,  
                            HeightData4    = DM.ARAY[ri.PST].Data.dData4   ,  
                        });
                    }
                    iCycle_PostC = 0 ;
                    return true ;

                
            }
        }

        //매뉴얼 사이클
        public bool CycleNutStart()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                //sTemp = string.Format("Time Out Step.iCycle={0:00}", iCycle_PreC);
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

            int r, c = 0;
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    //double dTemp = 0.0;
                    //double.TryParse(tbDegree.Text, out dTemp);
                    //if (dTemp != 0)
                    //{
                    //    if (dTemp < 0)
                    //    {
                    //        SEQ.rsNut.SendWrite(129, 0);//반시계방향
                    //    }
                    //    else if (dTemp > 0)
                    //    {
                    //        SEQ.rsNut.SendWrite(129, 1);//시계방향
                    //    }
                    //    int iTemp = (int)(OM.DevInfo.dNutWorkOptn3 * 10);
                    //    SEQ.rsNut.SendWrite(149, iTemp);//회전량    
                    //}
                    //
                    //ML.IO_SetY(yi.NutSStart, true);




                    Log.TraceListView("너트런너 정지 시작");
                    IO_SetY(yi.NutSStart, false);

                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if(IO_GetY(yi.NutSStart)) return false;
                    IO_SetY(yi.NutStop, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 12:
                    if (!IO_GetY(yi.NutStop)) return false;
                    IO_SetY(yi.NutStop, false);

                    Step.iCycle++;
                    return false;
                
                case 13:
                    if (IO_GetY(yi.NutStop)) return false;

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleNutCylDown()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                //sTemp = string.Format("Time Out Step.iCycle={0:00}", iCycle_PreC);
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

            int r, c = 0;
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.ADJ_AdjustUpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (IO_GetX(xi.ADJ_AdjustUpDnBuff)) IO_SetY(yi.ADJ_AdjustUpDnBuff, true);
                    if (!CL_Complete(ci.ADJ_AdjustUpDn, fb.Bwd)) return false;
                    IO_SetY(yi.ADJ_AdjustUpDnBuff, false);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleNutStop()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                //sTemp = string.Format("Time Out Step.iCycle={0:00}", iCycle_PreC);
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

            int r, c = 0;
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    Log.TraceListView("너트런너 정지 시작");
                    IO_SetY(yi.NutFStart, false);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (IO_GetY(yi.NutFStart)) return false;
                    IO_SetY(yi.NutSStart, false);

                    Step.iCycle++;
                    return false;
                    
                case 12:
                    if(IO_GetY(yi.NutSStart)) return false;
                    IO_SetY(yi.NutStop, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 13:
                    if (!IO_GetY(yi.NutStop)) return false;
                    IO_SetY(yi.NutStop, false);

                    Step.iCycle++;
                    return false;
                
                case 14:
                    if (IO_GetY(yi.NutStop)) return false;

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.ADJ_TurnTableCw)
            {
                if (!CL_Complete(ci.ADJ_AdjustUpDn, fb.Fwd))
                {
                    sMsg = "너트러너 실린더 위치가 내려가 있어 테이블 회전 시 충돌 위험이 있습니다.";
                    bRet = false;
                }
                if (!CL_Complete(ci.ADJ_PreStageUpDn, fb.Bwd))
                {
                    sMsg = "프리 스테이지 실린더 위치가 올라가 있어 테이블 회전 시 충돌 위험이 있습니다.";
                    bRet = false;
                }
                if (!CL_Complete(ci.ADJ_PostStageUpDn, fb.Bwd))
                {
                    sMsg = "포스트 스테이지 실린더 위치가 올라가 있어 테이블 회전 시 충돌 위험이 있습니다.";
                    bRet = false;
                }
            }
            else if (_eActr == ci.ADJ_PreStageUpDn)
            {

            }
            else if (_eActr == ci.ADJ_AdjustUpDn)
            {
                
            }
            else if (_eActr == ci.ADJ_PostStageUpDn)
            {

            }
            else if (_eActr == ci.ADJ_AdjTransferFwBw)
            {
                if(!CL_Complete(ci.ADJ_AdjustUpDn, fb.Fwd))
                {
                    sMsg = "너트러너 실린더 위치가 내려가 있어 충돌 위험이 있습니다.";
                    bRet = false;
                }
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
            //if (IO_GetX(xi.ManualInspLimit))
            //{ 
            //    bRet = false; 
            //    sMsg = "Edge Inspection Unit Limit Sensor Detected"; 
            //} 

            //if (_eMotr == mi.LDR_XPck)
            //{
            //    if(!MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckBwd ) && 
            //       !MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckWait)     ) { bRet = false; sMsg = "LDR_YPck Need to Backward Position"; }
            //
            //}
            //else if(_eMotr == mi.LDR_YPck)
            //{
            //    if(!MT_CmprPos(mi.LDR_XPck,pv.LDR_XPckCst) && 
            //       !MT_CmprPos(mi.LDR_XPck,pv.LDR_XPckStg)     ) { bRet = false; sMsg = "LDR_XPck Need to Cassette or Stage Position"; }
            //}
            //else if(_eMotr == mi.LDR_ZPck)
            //{
            //    if(!MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckFwdCst) && 
            //       !MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckFwdStg) 
            //       ) { bRet = false; sMsg = "LDR_YPck Need to Cassette or Stage Forward Position"; }
            //    if(!MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckWait  ) && 
            //       !MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckBwd   ) 
            //       ) { bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }
            //}
            //else if(_eMotr == mi.LDR_ZStg)
            //{
            //    if (MT_CmprPos(mi.LDR_XPck, pv.LDR_XPckStg))
            //    {
            //        if(PM.GetValue(mi.LDR_YPck,pv.LDR_YPckStgMoveAble) >= MT_GetEncPos(mi.LDR_YPck))
            //        {
            //            bRet = false; sMsg = "LDR_YPck Need to Stage Moveable down Position"; 
            //        }
            //    }
            //}

            //else
            //{
            //    sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
            //    bRet = false;
            //}

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

        //이건 안쓰고 일단 축별로 걸어서 위에 체크세이프로 쓴다 이걸 언제함...개귀찮아서 다른방법 찾아야 하는거 아님?
        public bool JogCheckSafe(mi _eMotr, EN_JOG_DIRECTION _eDir, EN_UNIT_TYPE _eType, double _dDist)
        {
            if (OM.MstOptn.bDebugMode) return true;
            bool bRet = true;
            string sMsg = "";

            //if (_eMotr == mi.LDR_XPck)
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
            //else if (_eMotr == mi.LDR_YPck)
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
            //else if (_eMotr == mi.LDR_ZPck)
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
            //else if (_eMotr == mi.LDR_ZStg)
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
            //{
            //    sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
            //    bRet = false;
            //}

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

        //동작 확인
        public bool GetStop(mi _eMotr , pv _ePstn ,  double _dOfsPos=0, bool _bLink = true)
        {
            return MT_GetStopPos(_eMotr,_ePstn,_dOfsPos);
        }

        public bool GetStop(mi _eMotr)
        {
            return MT_GetStopInpos(_eMotr);
        }

        public bool GetStop(ci _eActr, fb _eFwd)
        {
            return CL_Complete(_eActr,_eFwd);
        }

        public bool GetStop(ci _eActr)
        {
            return CL_Complete(_eActr);
        }

        public bool CheckStop()
        {
            if (!CL_Err(ci.ADJ_AdjTransferFwBw) && !CL_Complete(ci.ADJ_AdjTransferFwBw)) return false;
            if (!CL_Err(ci.ADJ_AdjustUpDn     ) && !CL_Complete(ci.ADJ_AdjustUpDn     )) return false;
            if (!CL_Err(ci.ADJ_PostStageUpDn  ) && !CL_Complete(ci.ADJ_PostStageUpDn  )) return false;
            if (!CL_Err(ci.ADJ_PreStageUpDn   ) && !CL_Complete(ci.ADJ_PreStageUpDn   )) return false;
            if (!CL_Err(ci.ADJ_TurnTableCw    ) && !CL_Complete(ci.ADJ_TurnTableCw    )) return false;

            //GetStop에서 혹은 Cycle안에 이함수를 클리어 하면서 넣으면 시퀜스에서 타임아웃 체크 할 필요가 없음.
            //if(m_tmDelay.GetUsing())return false ; 

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
            Log.SendSerilog(sFullMsg);
        }        
        
    };

    
}
