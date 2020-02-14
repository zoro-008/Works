using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class Loader : Part
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
            Loading    ,
            Unloading  ,
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

        const int iOverloadStopDelay = 100 ;

        public Loader(int _iPartId = 0)
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
            if(SEQ._bRun) return ;

            //카세트 이전 작업 끝난 데이터 Unknown으로 바꾼다.
            if (DM.ARAY[ri.CST].GetCntStat(cs.Unkn) == 0 && DM.ARAY[ri.CST].GetCntStat(cs.Mask) == 0)
            {
                bool bLeft = ML.IO_GetX(xi.CassetteRight) && ML.IO_GetXUp(xi.CassetteLeft);
                bool bRight = ML.IO_GetX(xi.CassetteLeft) && ML.IO_GetXUp(xi.CassetteRight);
                bool bBoth = ML.IO_GetXUp(xi.CassetteRight) && ML.IO_GetXUp(xi.CassetteLeft);
                if (bLeft || bRight || bBoth)
                {
                    DM.ARAY[ri.CST].ChangeStat(cs.Work, cs.Unkn);
                }
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
                    Step.iToStart++;
                    return false;

                case 11:
                    Step.iToStart++;
                    return false;

                case 12:

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
                    Step.iToStop++;
                    return false;

                case 11:
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
                bool isLoading   = false;
                bool isUnloading = false;
                bool isWorkEnd   = false;


                if (ER_IsErr()) return false;

                //Normal Decide Step.
                     if (isLoading  ) { DM.ARAY[ri.CST].Trace(m_sPartName); DM.ARAY[ri.PCK].Trace(m_sPartName); DM.ARAY[ri.STG].Trace(m_sPartName); Step.eSeq = sc.Loading  ; }
                else if (isUnloading) { DM.ARAY[ri.CST].Trace(m_sPartName); DM.ARAY[ri.PCK].Trace(m_sPartName); DM.ARAY[ri.STG].Trace(m_sPartName); Step.eSeq = sc.Unloading; }
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
                case (sc.Loading  ): if (!CycleLoading  ()) return false; break;
                case (sc.Unloading): if (!CycleUnloading()) return false; break;

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
                    if (IO_GetX(xi.WaferDtSsr)){
                        ER_SetErr(ei.STG_RemoveWafer);
                        Step.iHome = 0;
                        return true;
                    }
                    MT_SetReset(mi.LDR_XPck, true);
                    MT_SetReset(mi.LDR_YPck, true);
                    MT_SetReset(mi.LDR_ZPck, true);
                    MT_SetReset(mi.LDR_ZStg, true);
                    m_tmDelay.Clear();
                    Step.iHome++;
                    return false ;

                case 11:
                    if(!m_tmDelay.OnDelay(100))return false ;//알람이 있는채로 시작 되는 경우 때문에.
                    
                    MT_SetReset(mi.LDR_XPck, false);
                    MT_SetReset(mi.LDR_YPck, false);
                    MT_SetReset(mi.LDR_ZPck, false);
                    MT_SetReset(mi.LDR_ZStg, false);
                    m_tmDelay.Clear();
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!m_tmDelay.OnDelay(500))return false;//처음 켜면 브레이크가 잡혀 있는체로 서보온 되는데 이게 시간이 조금 걸려서 이렇게 함;.

                    MT_SetServo(mi.LDR_XPck, true);
                    MT_SetServo(mi.LDR_YPck, true);
                    MT_SetServo(mi.LDR_ZPck, true);
                    MT_SetServo(mi.LDR_ZStg, true);
                    m_tmDelay.Clear();
                    Step.iHome++;
                    return false;

                case 13: 
                    if(!m_tmDelay.OnDelay(300))return false;
                    //MT_Stop(mi.LDR_XPck);
                    //MT_Stop(mi.LDR_YPck);
                    //MT_Stop(mi.LDR_ZPck);
                    //MT_Stop(mi.LDR_ZStg);
                    Step.iHome++;
                    return false;

                case 14: 
                    if(!MT_GetStop(mi.LDR_XPck)) return false;
                    if(!MT_GetStop(mi.LDR_YPck)) return false;
                    if(!MT_GetStop(mi.LDR_ZPck)) return false;
                    if(!MT_GetStop(mi.LDR_ZStg)) return false;

                    //MT_GoHome(mi.LDR_XPck);
                    MT_GoHome(mi.LDR_YPck);
                    //MT_GoHome(mi.LDR_ZPck);
                    //MT_GoHome(mi.LDR_ZStg);
                    Step.iHome++;
                    return false;

                case 15: 
                    //if(!MT_GetHomeDone(mi.LDR_XPck))return false ;
                    if(!MT_GetHomeDone(mi.LDR_YPck))return false ;
                    //if(!MT_GetHomeDone(mi.LDR_ZPck))return false ;
                    //if(!MT_GetHomeDone(mi.LDR_ZStg))return false ;
                    MT_GoHome(mi.LDR_XPck);
                    MT_GoHome(mi.LDR_ZPck);
                    MT_GoHome(mi.LDR_ZStg);

                    IO_SetY(yi.StageVacuum , false);
                    Step.iHome++;
                    return false;

                case 16: 
                    if(!MT_GetHomeDone(mi.LDR_XPck))return false ;
                    if(!MT_GetHomeDone(mi.LDR_ZPck))return false ;
                    if(!MT_GetHomeDone(mi.LDR_ZStg))return false ;
                    //MT_GoAbsMan(mi.LDR_XPck,PM.GetValue(mi.LDR_XPck,pv.LDR_XPckWait));
                    //MT_GoAbsMan(mi.LDR_YPck,PM.GetValue(mi.LDR_YPck,pv.LDR_YPckWait));
                    //MT_GoAbsMan(mi.LDR_ZPck,PM.GetValue(mi.LDR_ZPck,pv.LDR_ZPckWait));
                    //MT_GoAbsMan(mi.LDR_ZStg,PM.GetValue(mi.LDR_ZStg,pv.LDR_ZStgWait));
                    MT_GoAbsMan(mi.LDR_XPck,pv.LDR_XPckWait);
                    MT_GoAbsMan(mi.LDR_YPck,pv.LDR_YPckWait);
                    MT_GoAbsMan(mi.LDR_ZPck,pv.LDR_ZPckWait);
                    MT_GoAbsMan(mi.LDR_ZStg,pv.LDR_ZStgWait);
                    Step.iHome++;
                    return false;

                case 17: 
                    if(!MT_GetStopPos(mi.LDR_XPck,pv.LDR_XPckWait)) return false;
                    if(!MT_GetStopPos(mi.LDR_YPck,pv.LDR_YPckWait)) return false;
                    if(!MT_GetStopPos(mi.LDR_ZPck,pv.LDR_ZPckWait)) return false;
                    if(!MT_GetStopPos(mi.LDR_ZStg,pv.LDR_ZStgWait)) return false;
                    Step.iHome = 0;
                    return true;
            }
        }


        public bool CycleWait()
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
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    //MT_Stop(mi.LDR_XPck);
                    //MT_Stop(mi.LDR_YPck);
                    //MT_Stop(mi.LDR_ZPck);
                    //MT_Stop(mi.LDR_ZStg);
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.LDR_XPck)) return false;
                    if(!MT_GetStop(mi.LDR_YPck)) return false;
                    if(!MT_GetStop(mi.LDR_ZPck)) return false;
                    if(!MT_GetStop(mi.LDR_ZStg)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckWait)) return false;
                    MoveMotr(mi.LDR_XPck,pv.LDR_XPckWait);
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPckWait);
                    MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgWait);
                    Step.iCycle++;
                    return false;
                    
                case 13: 
                    if(!GetStop(mi.LDR_XPck,pv.LDR_XPckWait)) return false;
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPckWait)) return false;
                    if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgWait)) return false;
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //대충 쓰다 버릴놈들
        const int iVacuumTimeOut = 2200 ;
        double dCstWaferPos = 0.0 ;

        int iCycle    = 0 ;
        int iPreCycle = 0 ;
        public void InitCycleLoading  () { iCycle    = 10 ; iPreCycle = 0 ; }
        public void InitCycleUnLoading() { iCycle    = 10 ; iPreCycle = 0 ; }

        public bool CycleLoading()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(iCycle != 0 && iCycle == iPreCycle && CheckStop() && !OM.MstOptn.bDebugMode, 15000))
            {
                sTemp = string.Format("Time Out iCycle={0:00}", iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (iCycle != iPreCycle)
            {
                sTemp = string.Format("CycleLoading iCycle={0:00}", iCycle);
                Trace(sTemp);
            }

            iPreCycle = iCycle;

            if (Stat.bReqStop || (MM.Working() && MM.Stop) || ER_IsErr()) //에러상태일때 안멈춰서 추가함 dw
            {
                if(iCycle == 10 || iCycle == 20 || iCycle == 30 || iCycle == 40)
                {
                    //if(!OM.MstOptn.bIdleRun) return true;
                    return true;
                }
                
            }

            int r, c = 0;
            switch (iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iCycle={0:00}", iCycle);
                    return true;

                case 10:
                    Log.TraceListView("로딩 동작 시작");

                    bool bPckNone =  DM.ARAY[ri.PCK].CheckAllStat(cs.None) && !IO_GetX(xi.PickerVacuum); 
                    bool bStgNone =  DM.ARAY[ri.STG].CheckAllStat(cs.None) && !IO_GetX(xi.WaferDtSsr ); 
                    bool bCst     =  IO_GetX(xi.CassetteLeft) &&  IO_GetX(xi.CassetteRight) && DM.ARAY[ri.CST].GetCntStat(cs.Unkn) > 0; 
                    
                    //bool bStageDn = MT_CmprPos(mi.LDR_ZStg,PM.GetValue(mi.LDR_ZStg,pv.LDR_ZStgStageDown));
                    bool bStageDn = MT_CmprPos(mi.LDR_ZStg,pv.LDR_ZStgStageDown);

                    bool bPckUnkn =  DM.ARAY[ri.PCK].CheckAllStat(cs.Unkn) &&  IO_GetX(xi.PickerVacuum); 
                    
                    if(bPckNone && bStgNone && bCst) //로더 픽 -> 플레이스 -> 스테이지 다운
                    {
                        iCycle=20;
                        return false;
                    }

                    if(bPckUnkn && bStgNone) //플레이스 -> 스테이지 다운
                    {
                        iCycle=30;
                        return false;
                    }

                    if( bPckNone && !bStgNone && !bStageDn) //스테이지 다운
                    {
                        iCycle=40;
                        return false;
                    }

                    if(!bCst    ) ER_SetErr(ei.LDR_CstNoWafer);
                    if(!bPckNone) ER_SetErr(ei.PCK_Wafer     );
                    if(!bStgNone) ER_SetErr(ei.STG_Wafer     );
                    iCycle = 0;
                    return true;
                    
                case 20: //위에서 쓰니깐 조심...21번에서도 쓰는데 중간에 스탑 버튼 누르면 
                    //빈슬롯 스캔 됐을때 여기서 멈추게 함.
                    InitCycleStep();
                    iCycle++;
                    return false;

                case 21:
                    if(!CycleLoadingPick()) return false;
                    //if(ML.ER_IsErr()) //내부에서 에러로 리턴인 경우.
                    //{
                    /// iCycle = 0 ;
                    //    return true ;
                    //}
                    if(DM.ARAY[ri.PCK].CheckAllStat(cs.None)) //자제 없는 슬롯 작업시.
                    { 
                        iCycle = 20 ;
                        return false ;
                    }
                    iCycle = 30;
                    return false ;

                case 30:
                    InitCycleStep();
                    iCycle++;
                    return false;

                case 31:
                    if(!CycleLoadingPlace()) return false;
                    iCycle = 40;
                    return false;

                case 40:
                    InitCycleStep();
                    iCycle++;
                    return false;
                    
                case 41: 
                    if(!CycleStgDn()) return false;
                    Log.TraceListView("로딩 동작 종료");
                    iCycle = 0;
                    return true ;
            }
        }

        public bool bUnloadingEnd = false;
        public bool CycleUnloading()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(iCycle != 0 && iCycle == iPreCycle && CheckStop() && !OM.MstOptn.bDebugMode, 15000))
            {
                sTemp = string.Format("Time Out iCycle={0:00}", iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (iCycle != iPreCycle)
            {
                sTemp = string.Format("CycleUnloading iCycle={0:00}", iCycle);
                Trace(sTemp);
            }

            iPreCycle = iCycle;

            if (Stat.bReqStop || (MM.Working() && MM.Stop) || ER_IsErr()) //에러상태일때 안멈춰서 추가함 dw
            {
                if(iCycle == 10 || iCycle == 20 || iCycle == 30 || iCycle == 40)
                {
                    //if(!OM.MstOptn.bIdleRun) return true;
                    return true;
                }
                
            }

            int r, c = 0;
            switch (iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iCycle={0:00}", iCycle);
                    return true;

                case 10:
                    Log.TraceListView("언로딩 동작 시작");
                    bUnloadingEnd = false;

                    bool bPckNone =  DM.ARAY[ri.PCK].CheckAllStat(cs.None) && !IO_GetX(xi.PickerVacuum); 
                    bool bStgNone =  DM.ARAY[ri.STG].CheckAllStat(cs.None) && !IO_GetX(xi.WaferDtSsr ); 
                    bool bCst     =  IO_GetX(xi.CassetteLeft) &&  IO_GetX(xi.CassetteRight) && DM.ARAY[ri.CST].GetCntStat(cs.Mask) > 0; 
                    
                    //bool bStageUp = MT_CmprPos(mi.LDR_ZStg,PM.GetValue(mi.LDR_ZStg,pv.LDR_ZStgAlign));
                    bool bStageUp = MT_CmprPos(mi.LDR_ZStg,pv.LDR_ZStgAlign);
                    
                    bool bPckWork =  DM.ARAY[ri.PCK].CheckAllStat(cs.Work) &&  IO_GetX(xi.PickerVacuum); 

                    if(!bStageUp && !bStgNone && bPckNone &&  bCst) //스테이즈 업 -> 픽 -> 플레이스
                    {
                        iCycle=20;
                        return false;
                    }

                    if(!bStgNone && bPckNone && bCst) //픽 -> 플레이스
                    {
                        iCycle=30;
                        return false;
                    }

                    if( bPckWork && bCst) //플레이스
                    {
                        iCycle=40;
                        return false;
                    }

                    if (!bCst) ER_SetErr(ei.LDR_CstNoMask);
                    else       ER_SetErr(ei.LDR_NoWafer  );
                    iCycle = 0;
                    return true;
                    
                case 20:
                    InitCycleStep();
                    iCycle++;
                    return false;

                case 21:
                    if (!CycleStgUp()) return false;
                    InitCycleStep();
                    iCycle = 30;
                    return false ;

                case 30:
                    InitCycleStep();
                    iCycle++;
                    return false;

                case 31:
                    if (!CycleUnloadingPick()) return false;
                    InitCycleStep();
                    iCycle = 40;
                    return false;
                    
                case 40:
                    InitCycleStep();
                    iCycle++;
                    return false;

                case 41:
                    if (!CycleUnloadingPlace()) return false;
                    Log.TraceListView("언로딩 동작 종료");

                    bUnloadingEnd = true;
                    
                    iCycle = 0;
                    return true;
            }
        }

        //개별 동작
        public bool CycleLoadingPick()
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
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    Log.TraceListView("로딩 픽 동작 시작");
                    //bool bPckNone  =  ; 
                    //bool bCst      =   &&  IO_GetX(xi.);
                    //bool bCstWafer =   ; 

                    IO_SetY(yi.StageVacuum , false ); //여기 켜져 있으면 픽커 배큠압 안참... 라인에서는 상관 없을듯.

                    if(!DM.ARAY[ri.PCK].CheckAllStat(cs.None) )
                    {
                        ER_SetErr(ei.PCK_Wafer , "픽커에 자제 데이터가 있습니다."); //피커 자재가 있다
                        Step.iCycle = 0;
                        return true;
                    }
                    if(IO_GetX(xi.PickerVacuum))
                    {
                        ER_SetErr(ei.PCK_Wafer, "픽커 배큠 센서가 감지 중 입니다."); //피커 자재가 있다
                        Step.iCycle = 0;
                        return true;
                    }
                    if(!IO_GetX(xi.CassetteLeft))
                    {
                        ER_SetErr(ei.LDR_NoCst, "카세트 왼쪽 감지센서가 Off입니다."); //피커 자재가 있다
                        Step.iCycle = 0;
                        return true;
                    }
                    if (!IO_GetX(xi.CassetteRight))
                    {
                        ER_SetErr(ei.LDR_NoCst, "카세트 오른쪽 감지센서가 Off입니다."); //피커 자재가 있다
                        Step.iCycle = 0;
                        return true;
                    }
                    if (!DM.ARAY[ri.CST].IsExist(cs.Unkn))
                    {
                        ER_SetErr(ei.LDR_CstNoWafer, "카세트에 자제 데이터가 없습니다."); //피커 자재가 있다
                        Step.iCycle = 0;
                        return true;
                    }

                    //bool bCstAlign=  IO_GetX(xi.CassetteLeft) && !IO_GetX(xi.CassetteRight) || 
                    //                !IO_GetX(xi.CassetteLeft) &&  IO_GetX(xi.CassetteRight) ;
                    //if(bCstAlign) ER_SetErr(ei.LDR_CstLocationErr); //카세트를 잘 놓으시요!!! //메뉴얼에서 체크 함

                    r = DM.ARAY[ri.CST].FindLastRow(cs.Unkn);
                    r = OM.DevInfo.iRowCount - r - 1 ; //뒤집기 밑에서 부터 한다
                    dCstWaferPos = OM.DevInfo.dRowPitch * r * -1;

                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    

                    MoveMotr(mi.LDR_XPck,pv.LDR_XPckCst        );
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPck1stWaferBtm,dCstWaferPos);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!GetStop(mi.LDR_XPck,pv.LDR_XPckCst        )) return false;
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPck1stWaferBtm,dCstWaferPos)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckFwdCst);
                    Step.iCycle++;
                    return false;
                    
                case 13:
                    //Overload1
                    if (IO_GetX(xi.WaferOverload)) //오버로드 처리 -> 100
                    {
                        MT_EmgStop(mi.LDR_YPck);
                        ER_SetErr(ei.LDR_WaferOverload);
                        Step.iCycle=100;
                        return false;
                    }
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckFwdCst)) return false;
                    MoveMotrSlow(mi.LDR_ZPck,pv.LDR_ZPck1stWafer,dCstWaferPos);
                    Step.iCycle++;
                    return false;
                    
                case 14:
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPck1stWafer,dCstWaferPos)) return false;
                    IO_SetY(yi.PickerVacuum,true);
                    m_tmDelay1.Clear();
                    m_tmDelay2.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if(IO_GetX(xi.PickerVacuum))
                    {
                        MoveMotr(mi.LDR_YPck, pv.LDR_YPckBwd);
                        Step.iCycle++;
                        return false;
                    }
                    if(!m_tmDelay1.OnDelay(OM.DevInfo.iVacuumOn)) return false; //최소 베큠 시간
                    //if(m_tmDelay2.OnDelay(iVacuumTimeOut)) { //자재 없다고 판단 -> 200
                    //    IO_SetY(yi.WaferVacuum,false); //딜레이 있어야 할듯 자연파기임
                    //    Step.iCycle = 200;
                    //    return false;
                    //}
                    //if(!IO_GetX(xi.PickerVacuum)) return false;

                    //MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    IO_SetY(yi.PickerVacuum, false); //딜레이 있어야 할듯 자연파기임
                    Step.iCycle=200;
                    return false;

                case 16:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;

                    //카세트는 마스크로 피커는 작업전으로 셋팅
                    r = DM.ARAY[ri.CST].FindLastRow(cs.Unkn);
                    DM.ARAY[ri.PCK].SetStat(cs.Unkn);
                    DM.ARAY[ri.CST].SetStat(0,r,cs.Mask);

                    //우측으로 이동하는건 일단 안함
                    Step.iCycle= 0 ;
                    return true ;

                //Overload 처리 100~
                case 100:
                    //MT_Stop(mi.LDR_YPck);
                    Step.iCycle++;
                    return false;

                case 101:
                    if(!GetStop(mi.LDR_YPck)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 102:
                    if(!m_tmDelay.OnDelay(iOverloadStopDelay))return false ; //여기서 딜레이 없이 바로 무브 시키면 씹힘.
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;

                case 103:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    Step.iCycle= 0 ;
                    return true ;

                //베큠 감지 안됨 자재 없다고 판단 Empty처리 뒤로 빠지고 끝냄
                case 200:
                    IO_SetY(yi.PickerVacuum,false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 201:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iVacuumOff)) return false; //혹시 자재가 있었는데 못집엇는데 베큠압이 남아있어서 자재를 부슬까봐
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPck1stWaferBtm,dCstWaferPos);//밑에 자제 쓸어서 데리고 옴.
                    Step.iCycle++;
                    return false;

                case 202:
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPck1stWaferBtm,dCstWaferPos)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;

                case 203:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    r = DM.ARAY[ri.CST].FindLastRow(cs.Unkn);
                    DM.ARAY[ri.CST].SetStat(0,r,cs.Empty);
                    Step.iCycle= 0 ;
                    return true ;

            }
        }

        public bool CycleLoadingPlace()
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
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    Log.TraceListView("로딩 플레이스 동작 시작");
                    //bool bPckNone  =   DM.ARAY[ri.PCK].CheckAllStat(cs.None) || !IO_GetX(xi.WaferVacuum); 
                    //bool bStgExist =  !DM.ARAY[ri.STG].CheckAllStat(cs.None) ||  IO_GetX(xi.WaferDtSsr ); 

                    bool bPckNone  =   DM.ARAY[ri.PCK].CheckAllStat(cs.None) || !IO_GetX(xi.PickerVacuum); 
                    bool bStgExist =  !DM.ARAY[ri.STG].CheckAllStat(cs.None) ||  IO_GetX(xi.WaferDtSsr ); 
                    
                    if(bPckNone) 
                    {
                        ER_SetErr(ei.PCK_Wafer, "피커에 자제 데이터가 없거나 배큠센서가 감지 되지 않습니다."); //피커 자재가 있다
                        Step.iCycle = 0;
                        return true;
                    }
                    if(bStgExist) 
                    {
                        ER_SetErr(ei.STG_Wafer , "스테이지에 자제 데이터가 있거나 센서가 감지 되었습니다."); //스테이지에 자재가 있다
                        Step.iCycle=0;
                        return true;
                    }
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgAlign);
                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    MoveMotr(mi.LDR_XPck,pv.LDR_XPckStg);
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPckStgAlignUp); //약간 충돌 가능성 있어서 보여 서 뒤에서 하자.
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!GetStop(mi.LDR_XPck,pv.LDR_XPckStg       )) return false;
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPckStgAlignUp)) return false;
                    //MoveMotr(mi.LDR_ZPck, pv.LDR_ZPckStgAlignUp);
                    Step.iCycle++;
                    return false ;

                case 13:
                    //if (!GetStop(mi.LDR_ZPck, pv.LDR_ZPckStgAlignUp)) return false;

                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckFwdStg);
                    Step.iCycle++;
                    return false;
                    
                case 14:
                    //Overload1
                    if (IO_GetX(xi.WaferOverload)) //오버로드 처리 -> 100
                    {
                        ER_SetErr(ei.LDR_WaferOverload);
                        MT_EmgStop(mi.LDR_YPck);
                        Step.iCycle = 100;
                        return false;
                    }
                    
                    if (!GetStop(mi.LDR_YPck,pv.LDR_YPckFwdStg     )) return false;
                    if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgAlign      )) return false;
                    IO_SetY(yi.PickerVacuum,false);
                    IO_SetY(yi.StageVacuum,false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15: 
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iVacuumOff)) return false; 
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPckStgAlign); //이거 속도 퍼센트로 조정해라
                    Step.iCycle++;
                    return false;
                    
                case 16: 
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPckStgAlign)) return false;
                    IO_SetY(yi.StageVacuum,true);
                    Step.iCycle++;
                    return false;
                    
                case 17: 
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPckStgAlignDown); //이거 속도 퍼센트로 조정해라
                    Step.iCycle++;
                    return false;
                    
                case 18: 
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPckStgAlignDown)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;
                    
                case 19: 
                    if(MT_GetEncPos(mi.LDR_YPck) < PM.GetValue(mi.LDR_YPck, pv.LDR_YPckStgMoveAble))
                    {
                        MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgBeforeStage);
                        Step.iCycle++;
                        return false;
                    }
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    MoveMotr(mi.LDR_ZStg, pv.LDR_ZStgBeforeStage);
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!GetStop(mi.LDR_ZStg, pv.LDR_ZStgBeforeStage)) return false;
                    DM.ARAY[ri.PCK].SetStat(cs.None);
                    DM.ARAY[ri.STG].SetStat(cs.Unkn); //얼라인 전상태

                    //무조건 제거해야 할수도 있고 아닐수도 있어서 일단 무조건 제거하게 하지는 않고 에러 한번만 띄운다
                    //이따 밑에 언로딩 픽에서 에러 다시 띄우는 걸로 하자 
                    if (!IO_GetX(xi.StageVacuum)) //베큠이 잡히지 않아...얼라인 실패 하면 답도 없다. 자재 제거해라
                    {
                        IO_SetY(yi.StageVacuum,false);
                        ER_SetErr(ei.STG_AlignFail);
                        Step.iCycle=0;
                        return true;
                    }

                    Step.iCycle=0;
                    return true;

                //Overload 처리 100~
                case 100:
                    //MT_EmgStop(mi.LDR_YPck);
                    Step.iCycle++;
                    return false;

                case 101:
                    if (!GetStop(mi.LDR_YPck)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 102:
                    if (!m_tmDelay.OnDelay(iOverloadStopDelay)) return false; //여기서 딜레이 없이 바로 무브 시키면 씹힘.
                    MoveMotr(mi.LDR_YPck, pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;

                case 103:
                    if (!GetStop(mi.LDR_YPck, pv.LDR_YPckBwd)) return false;
                    Step.iCycle = 0;
                    return true;

            }
        }
        
        public bool CycleUnloadingPick()
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
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    Log.TraceListView("언로딩 픽 동작 시작");
                    if (!MT_CmprPos(mi.LDR_YPck, pv.LDR_YPckBwd) || !MT_CmprPos(mi.LDR_YPck, pv.LDR_YPckWait))
                    {
                        ER_SetErr(ei.PCK_NeedtoBwd);
                        Step.iCycle=0;
                        return true;
                    }

                    if(!DM.ARAY[ri.PCK].CheckAllStat(cs.None))
                    {
                        ER_SetErr(ei.PCK_Wafer , "픽커에 자제데이터가 있습니다."); //피커 자재가 있다
                        Step.iCycle = 0;
                        return true;
                    }
                    if(IO_GetX(xi.PickerVacuum)) 
                    {
                        ER_SetErr(ei.PCK_Wafer , "픽커에 배큠센서가 감지 되고 있습니다." ); //피커 자재가 있다
                        Step.iCycle = 0;
                        return true;
                    }

                    if(DM.ARAY[ri.STG].CheckAllStat(cs.None)) 
                    {
                        ER_SetErr(ei.STG_NoWafer , "스테이지에 자제 데이터가 없습니다."); //스테이지에 자재가 없다
                        Step.iCycle=0;
                        return true;
                    }
                    if(!IO_GetX(xi.WaferDtSsr))
                    {
                        ER_SetErr(ei.STG_NoWafer, "스테이지에 자제 감지 센서가 Off입니다."); //스테이지에 자재가 없다
                        Step.iCycle = 0;
                        return true;
                    }

                    IO_SetY(yi.PickerVacuum,false);
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    MoveMotr(mi.LDR_XPck,pv.LDR_XPckStg);
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPckStgAlignDown);
                    MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgAlign);
                    Step.iCycle++;
                    return false;
                    
                case 12:
                    if(!GetStop(mi.LDR_XPck,pv.LDR_XPckStg)) return false;
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPckStgAlignDown)) return false;
                    if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgAlign)) return false;
                    //if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgStage)) return false;
                    IO_SetY(yi.StageVacuum,true);
                    m_tmDelay1.Clear();
                    m_tmDelay2.Clear();
                    Step.iCycle++;
                    return false;
                    
                case 13:
                    //if(!m_tmDelay1.OnDelay(OM.DevInfo.iVacuumOn)) return false; //최소 베큠 시간
                    if( m_tmDelay2.OnDelay(iVacuumTimeOut))
                    {
                        IO_SetY(yi.StageVacuum,false);
                        ER_SetErr(ei.LDR_StageVacuum , "스테이지 배큠 센서가 감지 되지 않습니다.");
                        Step.iCycle=0;
                        return true;
                    }
                    if(!IO_GetX(xi.StageVacuum)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckFwdStg);
                    Step.iCycle++;
                    return false;
                    
                case 14:
                    //Overload1
                    if (IO_GetX(xi.WaferOverload)) //오버로드 처리 -> 100
                    {
                        ER_SetErr(ei.LDR_WaferOverload);
                        MT_EmgStop(mi.LDR_YPck);
                        Step.iCycle = 100;
                        return false;
                    }
                    if (!GetStop(mi.LDR_YPck,pv.LDR_YPckFwdStg)) return false;
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPckStgAlign);
                    Step.iCycle++;
                    return false;
                    
                case 15:
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPckStgAlign)) return false;
                    IO_SetY(yi.StageVacuum,false);
                    IO_SetY(yi.PickerVacuum,true );
                    m_tmDelay1.Clear();
                    m_tmDelay2.Clear();
                    m_tmDelay3.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!m_tmDelay1.OnDelay(OM.DevInfo.iVacuumOn )) return false;
                    if(!m_tmDelay2.OnDelay(OM.DevInfo.iVacuumOff)) return false;
                    if( m_tmDelay3.OnDelay(iVacuumTimeOut))
                    {
                        IO_SetY(yi.PickerVacuum,false);
                        ER_SetErr(ei.LDR_WaferVacuum);
                        Step.iCycle=200; //베큠 안잡힐시에 밑으로 빠져서 뒤로가야됨
                        return false;
                    }
                    if(!IO_GetX(xi.PickerVacuum)) return false;
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPckStgAlignUp);
                    Step.iCycle++;
                    return false;
                    
                case 17:
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPckStgAlignUp)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;
                    
                case 18:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;

                    DM.ARAY[ri.STG].SetStat(cs.None);
                    DM.ARAY[ri.PCK].SetStat(cs.Work);

                    Step.iCycle= 0 ;
                    return true ;

                //Overload 처리 100~
                case 100:
                    //MT_EmgStop(mi.LDR_YPck);
                    Step.iCycle++;
                    return false;

                case 101:
                    if (!GetStop(mi.LDR_YPck)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 102:
                    if (!m_tmDelay.OnDelay(iOverloadStopDelay)) return false; //여기서 딜레이 없이 바로 무브 시키면 씹힘.
                    MoveMotr(mi.LDR_YPck, pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;

                case 103:
                    if (!GetStop(mi.LDR_YPck, pv.LDR_YPckBwd)) return false;
                    Step.iCycle = 0;
                    return true;



                case 200: //베큠 실패 밑으로 갔다 빠짐
                    IO_SetY(yi.PickerVacuum,false);
                    m_tmDelay1.Clear();
                    Step.iCycle++;
                    return false;

                case 201:
                    if(!m_tmDelay1.OnDelay(OM.DevInfo.iVacuumOff)) return false;
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPckStgAlignDown);
                    Step.iCycle++;
                    return false;
                    
                case 202:
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPckStgAlignDown)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;
                    
                case 203:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CycleUnloadingPlace()
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
            switch (Step.iCycle)
            {
                
                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    Log.TraceListView("언로딩 플레이스 동작 시작");
                    if (!MT_CmprPos(mi.LDR_YPck, pv.LDR_YPckBwd) || !MT_CmprPos(mi.LDR_YPck, pv.LDR_YPckWait))
                    {
                        ER_SetErr(ei.PCK_NeedtoBwd);
                        Step.iCycle=0;
                        return true;
                    }

                    IO_SetY(yi.PickerVacuum,true);

                    if(DM.ARAY[ri.PCK].CheckAllStat(cs.None))
                    {
                        ER_SetErr(ei.PCK_NoWafer , "픽커에 자제 데이터가 없습니다.");  
                        Step.iCycle=0;
                        return true ;
                    }
                    if (!IO_GetX(xi.PickerVacuum))
                    {
                        ER_SetErr(ei.PCK_NoWafer, "픽커에 배큠센서가 Off입니다.");  
                        Step.iCycle = 0;
                        return true;
                    }
                    if (!IO_GetX(xi.CassetteLeft) || !IO_GetX(xi.CassetteRight))
                    {
                        ER_SetErr(ei.PCK_NoWafer, "메거진 감지센서가 Off입니다.");  
                        Step.iCycle = 0;
                        return true;
                    }
                    if (!DM.ARAY[ri.CST].IsExist(cs.Mask))
                    {
                        ER_SetErr(ei.PCK_NoWafer, "메거진에 마스크 슬롯이 없습니다.");  
                        Step.iCycle = 0;
                        return true;
                    }


                    r = DM.ARAY[ri.CST].FindLastRow(cs.Mask);
                    r = OM.DevInfo.iRowCount - r - 1;
                    dCstWaferPos = OM.DevInfo.dRowPitch * r * -1 ;

                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    MoveMotr(mi.LDR_XPck,pv.LDR_XPckCst);//왠지 불안해서 구분동작으로 가자.
                    MoveMotr(mi.LDR_ZPck,pv.LDR_ZPck1stWafer,dCstWaferPos);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!GetStop(mi.LDR_XPck, pv.LDR_XPckCst )) return false;
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPck1stWafer,dCstWaferPos)) return false;

                    //MoveMotr(mi.LDR_XPck, pv.LDR_XPckCst);
                    Step.iCycle++;
                    return false ;

                case 13:
                    //if (!GetStop(mi.LDR_XPck, pv.LDR_XPckCst)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckFwdCst);
                    Step.iCycle++;
                    return false;
                    
                case 14:
                    //Overload1
                    if (IO_GetX(xi.WaferOverload)) //오버로드 처리 -> 100
                    {
                        MT_EmgStop(mi.LDR_YPck);
                        ER_SetErr(ei.LDR_WaferOverload);
                        Step.iCycle=100;
                        return false;
                    }
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckFwdCst)) return false;
                    IO_SetY(yi.PickerVacuum,false);
                    m_tmDelay1.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!m_tmDelay1.OnDelay(OM.DevInfo.iVacuumOff)) return false;
                    MoveMotrSlow (mi.LDR_ZPck,pv.LDR_ZPck1stWaferBtm,dCstWaferPos);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!GetStop(mi.LDR_ZPck,pv.LDR_ZPck1stWaferBtm,dCstWaferPos)) return false;
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;
                    
                case 17: 
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    r = DM.ARAY[ri.CST].FindLastRow(cs.Mask);
                    DM.ARAY[ri.CST].SetStat(0,r,cs.Work);
                    DM.ARAY[ri.PCK].SetStat(cs.None);
                    Step.iCycle= 0 ;
                    return true ;

                //Overload 처리 100~
                case 100:
                    //MT_EmgStop(mi.LDR_YPck);
                    Step.iCycle++;
                    return false;

                case 101:
                    if(!GetStop(mi.LDR_YPck)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 102:
                    if(!m_tmDelay.OnDelay(iOverloadStopDelay))return false ; //스탑 하고 딜레이 안주면 다음 무빙 함수 씹힘.
                    MoveMotr(mi.LDR_YPck,pv.LDR_YPckBwd);
                    Step.iCycle++;
                    return false;

                case 103:
                    if(!GetStop(mi.LDR_YPck,pv.LDR_YPckBwd)) return false;
                    Step.iCycle= 0 ;
                    return true ;

            }
        }

        public bool CycleStgUp()
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
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    Log.TraceListView("스테이지 업 동작 시작");
                    if (!MT_CmprPos(mi.LDR_YPck, pv.LDR_YPckBwd) || !MT_CmprPos(mi.LDR_YPck, pv.LDR_YPckWait))
                    {
                        ER_SetErr(ei.PCK_NeedtoBwd);
                        Step.iCycle=0;
                        return true;
                    }
                    //bool bStgUnkn =  DM.ARAY[ri.STG].CheckAllStat(cs.None) &&  IO_GetX(xi.WaferDtSsr ); 
                    //bool bStgDisp = !DM.ARAY[ri.STG].CheckAllStat(cs.None) && !IO_GetX(xi.WaferDtSsr );
                    //
                    //if (bStgDisp)
                    //{
                    //    ER_SetErr(ei.STG_RemoveWafer);
                    //    Step.iCycle = 0;
                    //    return true;
                    //}
                    MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgStage);
                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgStage)) return false;
                    IO_SetY(yi.StageVacuum,true);
                    m_tmDelay1.Clear();
                    m_tmDelay2.Clear();
                    Step.iCycle++;
                    return false;
                    
                case 12:
                    if(!m_tmDelay1.OnDelay(OM.DevInfo.iVacuumOn)) return false; //최소 베큠 시간
                    if( m_tmDelay2.OnDelay(iVacuumTimeOut))
                    {
                        //IO_SetY(yi.StageVacuum,false);
                        ER_SetErr(ei.LDR_StageVacuum);
                        Step.iCycle=0;
                        return true;
                    }
                    if(!IO_GetX(xi.StageVacuum)) return false;
                    Step.iCycle++;
                    return false;

                case 13: 
                    MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgAlign);
                    Step.iCycle++;
                    return false;
                    
                case 14:
                    if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgAlign)) return false;
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CycleStgDn() //자재를 스테이지에 놓는다
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
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    Log.TraceListView("스테이지 다운 동작 시작");
                    if (!MT_CmprPos(mi.LDR_YPck, pv.LDR_YPckBwd) || !MT_CmprPos(mi.LDR_YPck, pv.LDR_YPckWait))
                    {
                        ER_SetErr(ei.PCK_NeedtoBwd);
                        Step.iCycle=0;
                        return true;
                    }

                    bool bVacuum = IO_GetY(yi.StageVacuum) ;
                    if (!bVacuum)
                    {
                        IO_SetY(yi.StageVacuum,true);
                        m_tmDelay1.Clear();
                        m_tmDelay2.Clear();
                        Step.iCycle++;
                        return false;
                    }
                    Step.iCycle=20;
                    return false;

                case 11:
                    if(!m_tmDelay1.OnDelay(OM.DevInfo.iVacuumOn)) return false;
                    if(m_tmDelay2.OnDelay(iVacuumTimeOut))
                    {
                        IO_SetY(yi.StageVacuum,false);
                        ER_SetErr(ei.STG_AlignFail);
                        Step.iCycle=0;
                        return true;
                    }

                    if (!IO_GetX(xi.StageVacuum)) return false;//베큠이 잡히지 않아...얼라인 실패 하면 답도 없다. 자재 제거해라
                    Step.iCycle=20;
                    return false;

                case 20: //베큠킨상태
                    bool bStgUnkn =  DM.ARAY[ri.STG].CheckAllStat(cs.None) && ( IO_GetX(xi.WaferDtSsr ) ||  IO_GetX(xi.StageVacuum)); //닝겐이 그냥 놧을경우로 그냥 내둬 본다
                    bool bStgDisp = !DM.ARAY[ri.STG].CheckAllStat(cs.None) && (!IO_GetX(xi.WaferDtSsr ) || !IO_GetX(xi.StageVacuum));

                    if (bStgDisp)
                    {
                        ER_SetErr(ei.STG_RemoveWafer);
                        Step.iCycle = 0;
                        return true;
                    }

                    MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgBeforeStage);
                    Step.iCycle++;
                    return false;

                case 21:
                    if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgBeforeStage)) return false;
                    MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgStage);
                    Step.iCycle++;
                    return false;

                case 22: 
                    if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgStage)) return false;
                    IO_SetY(yi.StageVacuum,false);

                    m_tmDelay1.Clear();
                    Step.iCycle++;
                    return false;

                case 23:
                    if(!m_tmDelay1.OnDelay(OM.DevInfo.iVacuumOff)) return false; 

                    MoveMotr(mi.LDR_ZStg,pv.LDR_ZStgStageDown);
                    Step.iCycle++;
                    return false;

                case 24:
                    if(!GetStop(mi.LDR_ZStg,pv.LDR_ZStgStageDown)) return false;
                    DM.ARAY[ri.STG].SetStat(cs.Work);
                    Step.iCycle= 0 ;
                    return true ;
            }
        }
        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            //if (_eActr == ci.LDR_OutPshrFtRr)
            if (!_bChecked)
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

            bool bNegDir = ML.MT_GetCmdPos(_eMotr) > dDstPos ;

            bool bRet = true;
            string sMsg = "";
            if (!IO_GetX(xi.ManualInspLimit))
            { 
                bRet = false; 
                sMsg = "Edge Inspection Unit Limit Sensor Detected"; 
            } 

            if (_eMotr == mi.LDR_XPck)
            {
                //if(!MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckBwd ) && 
                //   !MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckWait)     ) { bRet = false; sMsg = "LDR_YPck Need to Backward Position"; }
                if(ML.MT_GetCmdPos(mi.LDR_YPck) > PM_GetValue(mi.LDR_YPck, pv.LDR_YPckStgMoveAble)) { bRet = false; sMsg = "LDR_YPck Need to Backward Position"; }

            }
            else if(_eMotr == mi.LDR_YPck)
            {
                if(!MT_CmprPos(mi.LDR_XPck,pv.LDR_XPckCst) && 
                   !MT_CmprPos(mi.LDR_XPck,pv.LDR_XPckStg) && !bNegDir && _ePstn != pv.LDR_YPckWait && _ePstn != pv.LDR_YPckBwd) { bRet = false; sMsg = "LDR_XPck Need to Cassette or Stage Position"; }

                //스테이지 위에 커버에 부딪히는거 방지
                double dX_Ctr = (PM.GetValue(mi.LDR_XPck,pv.LDR_XPckStg) + PM.GetValue(mi.LDR_XPck,pv.LDR_XPckCst)) / 2.0 ;
                bool   bX_Stg = MT_GetCmdPos(mi.LDR_XPck) < dX_Ctr ;
                if (bX_Stg)
                {
                    bool bZ_StgPos = PM.GetValue(mi.LDR_ZPck,pv.LDR_ZPckStgAlignUp) - 1.0 < MT_GetCmdPos(mi.LDR_ZPck) &&
                                     MT_GetCmdPos(mi.LDR_ZPck) < PM.GetValue(mi.LDR_ZPck,pv.LDR_ZPckStgAlignDown) + 1.0 ;
                    if(!bZ_StgPos) { bRet = false; sMsg = "LDR_ZPck Need to Stage Position"; }
                }

            }
            else if(_eMotr == mi.LDR_ZPck)
            {
                double dX_Ctr = (PM.GetValue(mi.LDR_XPck,pv.LDR_XPckStg) + PM.GetValue(mi.LDR_XPck,pv.LDR_XPckCst)) / 2.0 ;
                bool   bX_Stg = MT_GetCmdPos(mi.LDR_XPck) < dX_Ctr ;
                //bool   bX_Cst = MT_GetCmdPos(mi.LDR_XPck) > dX_Ctr ;

                bool   bY_Fwd = !MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckWait) && !MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckBwd) ;

                if (bX_Stg)
                {
                    if (bY_Fwd)
                    {
                        //포지션 이동 안됨
                        if(_ePstn == pv.LDR_ZPckWait        ) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }
                        if(_ePstn == pv.LDR_ZPck1stWaferBtm ) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }
                        if(_ePstn == pv.LDR_ZPck1stWafer    ) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }
                    }
                }
                else
                {
                    if (bY_Fwd)
                    {
                        //포지션 이동 안됨
                        if(_ePstn == pv.LDR_ZPckWait         ) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }
                        if(_ePstn == pv.LDR_ZPckStgAlignUp   ) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }
                        if(_ePstn == pv.LDR_ZPckStgAlign     ) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }
                        if(_ePstn == pv.LDR_ZPckStgAlignDown ) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }

                        if(_ePstn == pv.LDR_ZPck1stWaferBtm && !MM.Working()) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }
                        if(_ePstn == pv.LDR_ZPck1stWafer    && !MM.Working()) {  bRet = false; sMsg = "LDR_YPck Need to Wait or Backward Position"; }

                    }
                }



                if(!MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckFwdCst) && 
                   !MT_CmprPos(mi.LDR_YPck,pv.LDR_YPckFwdStg) &&
                    MT_GetCmdPos(mi.LDR_YPck) > PM_GetValue(mi.LDR_YPck, pv.LDR_YPckStgMoveAble)) 
                {
                        bRet = false; sMsg = "LDR_YPck Need to Wait or Backward or Cassette or StageForward Position"; 
                        bRet = false; sMsg = "LDR_YPck Need to Cassette or Stage Forward Position";                     
                }
            }
                
            
            else if(_eMotr == mi.LDR_ZStg)
            {
                if (MT_CmprPos(mi.LDR_XPck, pv.LDR_XPckStg))
                {
                    //if(PM.GetValue(mi.LDR_YPck,pv.LDR_YPckStgMoveAble) >= MT_GetEncPos(mi.LDR_YPck))
                    if (PM.GetValue(mi.LDR_YPck, pv.LDR_YPckStgMoveAble) < MT_GetEncPos(mi.LDR_YPck))
                    {
                        bRet = false; sMsg = "LDR_YPck Need to Stage Moveable down Position"; 
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

        //이건 안쓰고 일단 축별로 걸어서 위에 체크세이프로 쓴다 이걸 언제함...개귀찮아서 다른방법 찾아야 하는거 아님?
        public bool JogCheckSafe(mi _eMotr, EN_JOG_DIRECTION _eDir, EN_UNIT_TYPE _eType, double _dDist)
        {
            if (OM.MstOptn.bDebugMode) return true;
            bool bRet = true;
            string sMsg = "";

            if (_eMotr == mi.LDR_XPck)
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
            else if (_eMotr == mi.LDR_YPck)
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
            else if (_eMotr == mi.LDR_ZPck)
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
            else if (_eMotr == mi.LDR_ZStg)
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
            else if (MM.Working()  ) MT_GoAbsRun(_eMotr , dDstPos, iSpdPer);
            else                     MT_GoAbsMan(_eMotr , dDstPos);

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
            if( !MT_GetStop(mi.LDR_XPck)) return false;
            if( !MT_GetStop(mi.LDR_YPck)) return false;
            if( !MT_GetStop(mi.LDR_ZPck)) return false;
            if( !MT_GetStop(mi.LDR_ZStg)) return false;
            
            //if (!CL_Complete(ci.LDR_WorkStprRrFt)) return false;

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
