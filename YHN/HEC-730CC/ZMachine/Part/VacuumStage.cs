using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;
using System.Collections.Generic;

namespace Machine
{
    public class VacuumStage : Part
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
            Ready      ,
            Clean      ,
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
        //public VisnCom[] VisnComs = new VisnCom[(int)vi.MAX_VI];

        public VacuumStage(int _iPartId = 0)
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
        //bool bFirstReset = true ;
        override public void Reset() //리셑 버튼 눌렀을때 타는 함수.
        {

            ResetTimer();

            Stat.Clear();
            Step.Clear();
            PreStep.Clear();

            //if (!bFirstReset)
            //{
            //    for (int i = 0; i < (int)vi.MAX_VI; i++)
            //    {
            //        if (!OM.VsSkip((vi)i) && VisnComs[i].EndCmd()) VisnComs[i].SendCmd(VisnCom.vc.Reset);
            //    }
            //    
            //}
            //bFirstReset = false ;
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
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 10000)) {
                ER_SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);
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

                    Step.iToStop++;
                    return false;

                case 11:

                    Step.iToStop++;
                    return false;

                case 12:
                    Step.iToStop++;
                    return false ;

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

                //조건
                bool bEmpty = DM.ARAY[ri.VSTG].CheckAllStat(cs.Empty   );
                bool bReady = DM.ARAY[ri.VSTG].CheckAllStat(cs.WorkEnd );
                bool bClean = DM.ARAY[ri.VSTG].CheckAllStat(cs.Cleaning);

                //사이클
                bool isReady = bReady;
                bool isClean = bClean;
                bool isEnd   = bEmpty;


                //모르는 자제 에러
                //if( PrebNone && bPrbCheck && !IO_GetY(yi.RAIL_FeedingAC1)) ER_SetErr(ei.PKG_Unknwn,"Prebuffer have no data found, but strip sensor detected") ;
                //if( Vsn1None && bVs1Check) ER_SetErr(ei.PKG_Unknwn,"VisnZone1 have no data found, but strip sensor detected") ;
                //if( Vsn2None && bVs2Check) ER_SetErr(ei.PKG_Unknwn,"VisnZone2 have no data found, but strip sensor detected") ;
                //if( Vsn3None && bVs3Check) ER_SetErr(ei.PKG_Unknwn,"VisnZone3 have no data found, but strip sensor detected") ;
                
                //카세트 사라짐
                //if(!PrebNone && !bPrbCheck) ER_SetErr(ei.PKG_Dispr  ,"Prebuffer have data, but strip sensor not detected") ;
                //if(!Vsn1None && !bVs1Check) ER_SetErr(ei.PKG_Dispr  ,"VisnZone1 have data, but strip sensor not detected") ;
                //if(!Vsn2None && !bVs2Check) ER_SetErr(ei.PKG_Dispr  ,"VisnZone2 have data, but strip sensor not detected") ;
                //if(!Vsn3None && !bVs3Check) ER_SetErr(ei.PKG_Dispr  ,"VisnZone3 have data, but strip sensor not detected") ;

                if (ER_IsErr())
                {
                    return false;
                }
                     if (isReady) { DM.ARAY[ri.VSTG].Trace(); Step.eSeq = sc.Ready ; } 
                else if (isClean) { DM.ARAY[ri.VSTG].Trace(); Step.eSeq = sc.Clean ; } 
                else if (isEnd  ) { Stat.bWorkEnd = true; return true; }
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
                case (sc.Ready): if (!CycleReady()) return false; break;
                case (sc.Clean): if (!CycleClean()) return false; break;
            }
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        //위에 부터 작업.
        //인덱스 데이터에서 작업해야 되는 컬로우를 뽑아서 리턴.
        public bool FindChip(out int c, out int r, cs _iChip=cs.RetFail) 
        {
            //if(DM.ARAY[ri.VSN1].CheckAllStat(cs.None)){c=-1 ; r =-1 ; return false ;}
            //if(DM.ARAY[ri.VSN2].CheckAllStat(cs.None)){c=-1 ; r =-1 ; return false ;}
            //if(DM.ARAY[ri.VSN3].CheckAllStat(cs.None)){c=-1 ; r =-1 ; return false ;}

            //int r1 ;
            //int c1 ;
            //int r2 ;
            //int c2 ;
            //int r3 ;
            //int c3 ;
            //
            ////if(HEAD_INSP_DIRECTION == idLeftTop) {
            //r = DM.ARAY[ri.VSN1].GetMaxRow() ;
            //c = DM.ARAY[ri.VSN1].GetMaxCol() ;
            //
            //r1 = DM.ARAY[ri.VSN1].GetMaxRow() ;
            //c1 = DM.ARAY[ri.VSN1].GetMaxCol() ;
            //r2 = DM.ARAY[ri.VSN2].GetMaxRow() ;
            //c2 = DM.ARAY[ri.VSN2].GetMaxCol() ;
            //r3 = DM.ARAY[ri.VSN3].GetMaxRow() ;
            //c3 = DM.ARAY[ri.VSN3].GetMaxCol() ;
            //
            ////3존 중에 가장 먼저 있는 자제를 한다.
            //if(!DM.ARAY[ri.VSN1].CheckAllStat(cs.None)){r1 = DM.ARAY[ri.VSN1].FindFrstRow(_iChip); r = r1 ; c = c1 ;}
            //if(!DM.ARAY[ri.VSN2].CheckAllStat(cs.None)){r2 = DM.ARAY[ri.VSN2].FindFrstRow(_iChip); if(r > r2) { r = r2 ; c = c2 ;}}
            //if(!DM.ARAY[ri.VSN3].CheckAllStat(cs.None)){r3 = DM.ARAY[ri.VSN3].FindFrstRow(_iChip); if(r > r3) { r = r3 ; c = c3 ;}}           
            //
            ////if(r == -1 && c == -1) return false ;
            //if(r == -1) return false ;
            
            r = 0;
            c = 0;

            return true ;
        }

        public bool OverrideVelAtMultiPos(mi _eMotr, pv _ePstn, double[] _aOverridePos, double[] _aOverrideVel, double _dOfsPos=0)
        {
            //double dOverridePos1 = PM_GetValue(mi.VSTG_X, pv.VSTG_XCleanStt);
            //double dOverridePos2 = PM_GetValue(mi.VSTG_X, pv.VSTG_XCleanEnd);
            //double dOverrideVel1 = OM.DevInfo.dVSTGClnSpeed;
            //double dOverrideVel2 = SM.MTR.Para[(int)mi.VSTG_X].dRunSpeed;
            //
            //double[] aOverridePos = new double[2] { dOverridePos1, dOverridePos2 };
            //double[] aOverrideVel = new double[2] { dOverrideVel1, dOverrideVel2 };

            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;

            return MT_OverrideVelAtMultiPos(_eMotr, dDstPos, _aOverridePos, _aOverrideVel);
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

            switch (Step.iHome)
            {
                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    Trace(sTemp);
                    return true ;

                case 10:
                    MT_GoHome(mi.VSTG_X);

                    Step.iHome++;
                    return false;

                case 11:
                    if(!MT_GetHomeDone(mi.VSTG_X)) return false;
                    MoveMotr(mi.VSTG_X, pv.VSTG_XWorkStt);

                    Step.iHome++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.VSTG_X, pv.VSTG_XWorkStt)) return false;

                    Step.iHome = 0;
                    return true;
            }
        }
    
        public bool CycleReady()
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

            int c,r;
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    IO_SetY(yi.VSTG_Vacuum1, false);
                    IO_SetY(yi.VSTG_Vacuum2, false);
                    IO_SetY(yi.VSTG_Vacuum3, false);
                    IO_SetY(yi.VSTG_Vacuum4, false);
                    IO_SetY(yi.VSTG_Vacuum5, false);
                    IO_SetY(yi.VSTG_Vacuum6, false);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    IO_SetY(yi.VSTG_Eject1, true);
                    IO_SetY(yi.VSTG_Eject2, true);
                    IO_SetY(yi.VSTG_Eject3, true);
                    IO_SetY(yi.VSTG_Eject4, true);
                    IO_SetY(yi.VSTG_Eject5, true);
                    IO_SetY(yi.VSTG_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(200)) return false;
                    IO_SetY(yi.VSTG_Eject1, false);
                    IO_SetY(yi.VSTG_Eject2, false);
                    IO_SetY(yi.VSTG_Eject3, false);
                    IO_SetY(yi.VSTG_Eject4, false);
                    IO_SetY(yi.VSTG_Eject5, false);
                    IO_SetY(yi.VSTG_Eject6, false);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!OM.CmnOptn.bVSTG_IonOff ) IO_SetY(yi.VSTG_IonBlwrTop, true);
                    if (!OM.CmnOptn.bVSTG_Air1Off) IO_SetY(yi.VSTG_AirBlwrTop1, true);
                    if (!OM.CmnOptn.bVSTG_Air2Off) IO_SetY(yi.VSTG_AirBlwrTop2, true);
                    

                    Step.iCycle++;
                    return false;

                case 14:
                    if (!OM.CmnOptn.bVSTG_IonOff && !IO_GetY(yi.VSTG_IonBlwrTop)) return false;
                    if (!OM.CmnOptn.bVSTG_Air1Off && !IO_GetY(yi.VSTG_AirBlwrTop1)) return false;
                    if (!OM.CmnOptn.bVSTG_Air2Off && !IO_GetY(yi.VSTG_AirBlwrTop2)) return false;


                    m_dRunVel = MT_GetRunVel(mi.VSTG_X); //런속도 받아오기
                    MoveMotr(mi.VSTG_X, pv.VSTG_XWorkStt);

                    Step.iCycle++;
                    return false;

                case 15:
                    if (MT_GetCmdPos(mi.VSTG_X) < PM.GetValue(mi.VSTG_X, pv.VSTG_XCleanEnd)) return false;
                    //MT_OverrideVel(mi.VSTG_X, OM.DevInfo.dVSTGClnSpeed); //속도 오버로드
                    MT_OverrideVel(mi.VSTG_X, SM.MTR.GetSlowVel((int)mi.VSTG_X)); //속도 오버로드
                    Step.iCycle++;
                    return false;

                case 16:
                    if (MT_GetCmdPos(mi.VSTG_X) < PM.GetValue(mi.VSTG_X, pv.VSTG_XCleanStt)) return false;
                    MT_OverrideVel(mi.VSTG_X, m_dRunVel); //속도 오버로드

                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.VSTG_X, pv.VSTG_XWorkStt)) return false;
                    
                    IO_SetY(yi.VSTG_IonBlwrTop, false);
                    IO_SetY(yi.VSTG_AirBlwrTop1, false);
                    IO_SetY(yi.VSTG_AirBlwrTop2, false);

                    DM.ARAY[ri.VSTG].SetStat(cs.Empty);

                    Step.iCycle = 0;
                    return true;
                
                
            }
        }
        
        double m_dRunVel = 0;
        public bool CycleClean()
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
                    MoveMotr(mi.VSTG_X, pv.VSTG_XWorkStt);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.VSTG_X, pv.VSTG_XWorkStt)) return false;
                    if (!OM.CmnOptn.bVSTG_IonOff ) IO_SetY(yi.VSTG_IonBlwrTop, true);
                    if (!OM.CmnOptn.bVSTG_Air1Off) IO_SetY(yi.VSTG_AirBlwrTop1, true);
                    if (!OM.CmnOptn.bVSTG_Air2Off) IO_SetY(yi.VSTG_AirBlwrTop2, true);
                    
                        
                    

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!OM.CmnOptn.bVSTG_IonOff  && !IO_GetY(yi.VSTG_IonBlwrTop )) return false;
                    if (!OM.CmnOptn.bVSTG_Air1Off && !IO_GetY(yi.VSTG_AirBlwrTop1)) return false;
                    if (!OM.CmnOptn.bVSTG_Air2Off && !IO_GetY(yi.VSTG_AirBlwrTop2)) return false;
                    m_dRunVel = MT_GetRunVel(mi.VSTG_X); //런속도 받아오기
                    MoveMotr(mi.VSTG_X, pv.VSTG_XWorkEnd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (MT_GetCmdPos(mi.VSTG_X) > PM.GetValue(mi.VSTG_X, pv.VSTG_XCleanStt)) return false;
                    MT_OverrideVel(mi.VSTG_X,OM.DevInfo.dVSTGClnSpeed); //속도 오버로드
                    Step.iCycle++;
                    return false;
                    
                case 14:
                    if (MT_GetCmdPos(mi.VSTG_X) > PM.GetValue(mi.VSTG_X, pv.VSTG_XCleanEnd)) return false;
                    MT_OverrideVel(mi.VSTG_X,m_dRunVel); //속도 오버로드
                    
                    IO_SetY(yi.VSTG_IonBlwrTop, false); //주석 가능성있어서 밑에 냅둠
                    IO_SetY(yi.VSTG_AirBlwrTop1, false);
                    IO_SetY(yi.VSTG_AirBlwrTop2, false);

                    Step.iCycle++;
                    return false;

                case 15:
                    if(!MT_GetStopPos(mi.VSTG_X, pv.VSTG_XWorkEnd)) return false;
                    IO_SetY(yi.VSTG_IonBlwrTop, false);
                    IO_SetY(yi.VSTG_AirBlwrTop1, false);
                    IO_SetY(yi.VSTG_AirBlwrTop2, false);
                    DM.ARAY[ri.VSTG].SetStat(cs.Clean);
                    DM.ARAY[ri.RPCK].SetStat(cs.Pick);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleManVacuumOnOff()
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
                    if (IO_GetY(yi.VSTG_Vacuum1) || IO_GetY(yi.VSTG_Vacuum2) ||
                        IO_GetY(yi.VSTG_Vacuum3) || IO_GetY(yi.VSTG_Vacuum4) ||
                        IO_GetY(yi.VSTG_Vacuum5) || IO_GetY(yi.VSTG_Vacuum6))
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    IO_SetY(yi.VSTG_Vacuum1, true);
                    IO_SetY(yi.VSTG_Vacuum2, true);
                    IO_SetY(yi.VSTG_Vacuum3, true);
                    IO_SetY(yi.VSTG_Vacuum4, true);
                    IO_SetY(yi.VSTG_Vacuum5, true);
                    IO_SetY(yi.VSTG_Vacuum6, true);

                    Step.iCycle = 0;
                    return true;

                case 20:
                    IO_SetY(yi.VSTG_Vacuum1, false);
                    IO_SetY(yi.VSTG_Vacuum2, false);
                    IO_SetY(yi.VSTG_Vacuum3, false);
                    IO_SetY(yi.VSTG_Vacuum4, false);
                    IO_SetY(yi.VSTG_Vacuum5, false);
                    IO_SetY(yi.VSTG_Vacuum6, false);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    IO_SetY(yi.VSTG_Eject1, true);
                    IO_SetY(yi.VSTG_Eject2, true);
                    IO_SetY(yi.VSTG_Eject3, true);
                    IO_SetY(yi.VSTG_Eject4, true);
                    IO_SetY(yi.VSTG_Eject5, true);
                    IO_SetY(yi.VSTG_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    IO_SetY(yi.VSTG_Eject1, false);
                    IO_SetY(yi.VSTG_Eject2, false);
                    IO_SetY(yi.VSTG_Eject3, false);
                    IO_SetY(yi.VSTG_Eject4, false);
                    IO_SetY(yi.VSTG_Eject5, false);
                    IO_SetY(yi.VSTG_Eject6, false);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;


            //     if (_eActr == ci.RAIL_Vsn1AlignFwBw){ }
            //else if (_eActr == ci.RAIL_Vsn2AlignFwBw){ }
            //else if (_eActr == ci.RAIL_Vsn3AlignFwBw){ }
            //else if (_eActr == ci.RAIL_Vsn1StprUpDn ){ }
            //else if (_eActr == ci.RAIL_Vsn2StprUpDn ){ }
            //else if (_eActr == ci.RAIL_Vsn3StprUpDn ){ }
            //else if (_eActr == ci.RAIL_Vsn1SttnUpDn ){ }
            //else if (_eActr == ci.RAIL_Vsn2SttnUpDn ){ }
            //else if (_eActr == ci.RAIL_Vsn3SttnUpDn ){ }
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

            bool bRet = true;
            string sMsg = "";

            if (_eMotr == mi.VSTG_X)
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

            if (_eMotr == mi.VSTG_X)
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
            if (!MT_GetStop(mi.VSTG_X)) return false;

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
