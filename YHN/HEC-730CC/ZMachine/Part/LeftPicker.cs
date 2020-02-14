using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class LeftPicker : Part
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

        //로더에서 넣어주고 VisnZone에서 가져가기 때문에 할일이 없음.
        public enum sc
        {
            Idle    = 0,
            Pick       ,
            Clean      ,
            Place      ,
            PckrClean  ,
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

        public double dSttTime = 0;//1사이클타임 잴때 쓴다. 시작 시간
        public double dEndTime = 0;//1사이클타임 잴때 쓴다. 종료 시간

        public LeftPicker(int _iPartId = 0)
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
                    MoveCyl(ci.LPCK_PickerDnUp , fb.Bwd);
                    Step.iToStart++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.LPCK_PickerDnUp , fb.Bwd)) return false ;

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
                    MoveCyl(ci.LPCK_PickerDnUp , fb.Bwd);

                    Step.iToStop++;
                    return false;

                case 11: 
                    if(!CL_Complete(ci.LPCK_PickerDnUp , fb.Bwd)) return false ;

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
                bool bNone      = DM.ARAY[ri.LPCK].CheckAllStat(cs.None     ) && DM.ARAY[ri.PRER].CheckAllStat(cs.None);
                bool bPick      = DM.ARAY[ri.LPCK].CheckAllStat(cs.Pick     ) && DM.ARAY[ri.PRER].CheckAllStat(cs.Exist);
                bool bClean     = DM.ARAY[ri.LPCK].CheckAllStat(cs.Cleaning );
                bool bPlace     = DM.ARAY[ri.LPCK].CheckAllStat(cs.Place    ) && DM.ARAY[ri.VSTG].CheckAllStat(cs.Empty);
                bool bPckrClean = DM.ARAY[ri.LPCK].CheckAllStat(cs.Pckrclean);

                //사이클
                bool isPick      = bPick ;
                bool isClean     = bClean;
                bool isPlace     = bPlace;
                bool isPckrClean = bPckrClean; 
                bool isEnd       = bNone ; 

                //bool isCycleOut =  SEQ.HEAD.GetSeqStep() == (int)Head.sc.Move && !DM.ARAY[ri.PREB].CheckAllStat(cs.None);

                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.

                     if (isPick     ) { DM.ARAY[ri.LPCK].Trace(m_iPartId); DM.ARAY[ri.PRER].Trace(m_iPartId); Step.eSeq = sc.Pick      ; }
                else if (isClean    ) { DM.ARAY[ri.LPCK].Trace(m_iPartId);                                    Step.eSeq = sc.Clean     ; }
                else if (isPlace    ) { DM.ARAY[ri.LPCK].Trace(m_iPartId); DM.ARAY[ri.VSTG].Trace(m_iPartId); Step.eSeq = sc.Place     ; }
                else if (isPckrClean) { DM.ARAY[ri.LPCK].Trace(m_iPartId);                                    Step.eSeq = sc.PckrClean ; }
                else if (isEnd   ) { Stat.bWorkEnd = true; return true; }
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
                case (sc.Pick      ): if (!CyclePick     ()) return false; break;
                case (sc.Clean     ): if (!CycleClean    ()) return false; break;
                case (sc.Place     ): if (!CyclePlace    ()) return false; break;
                case (sc.PckrClean ): if (!CyclePckrClean()) return false; break;
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
        public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            return DM.ARAY[_iId].FindLastColFrstRow(ref c, ref r , _iChip);
        }

        public bool OverrideVelAtMultiPos(mi _eMotr, pv _ePstn, double _dOfsPos=0)
        {
            double dOverridePos1 = PM_GetValue(mi.LPCK_Y, pv.LPCK_YCleanStt);
            double dOverridePos2 = PM_GetValue(mi.LPCK_Y, pv.LPCK_YCleanEnd);
            double dOverrideVel1 = OM.DevInfo.dLPCKClnSpeed;
            double dOverrideVel2 = SM.MTR.Para[(int)mi.LPCK_Y].dRunSpeed;

            double[] aOverridePos = new double[2] { dOverridePos1, dOverridePos2 };
            double[] aOverrideVel = new double[2] { dOverrideVel1, dOverrideVel2 };

            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;

            return MT_OverrideVelAtMultiPos(_eMotr, dDstPos, aOverridePos, aOverrideVel);
        }

        public bool OverrideVelAtPos(mi _eMotr, pv _ePstn, double _dOverridePos, double _dOverrideVel,  double _dOfsPos = 0)
        {
            //double dOverridePos1 = PM_GetValue(mi.LPCK_Y, pv.LPCK_YCleanStt);
            //double dOverridePos2 = PM_GetValue(mi.LPCK_Y, pv.LPCK_YCleanEnd);
            //double dOverrideVel1 = OM.DevInfo.dLPCKClnSpeed;
            //double dOverrideVel2 = SM.MTR.Para[(int)mi.LPCK_Y].dRunSpeed;
            //
            //double[] aOverridePos = new double[2] { dOverridePos1, dOverridePos2 };
            //double[] aOverrideVel = new double[2] { dOverrideVel1, dOverrideVel2 };

            double dDstPos = PM_GetValue(_eMotr, _ePstn) + _dOfsPos;

            return MT_OverrideVelAtPos(_eMotr, dDstPos, _dOverridePos, _dOverrideVel);
        }

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 300000 )) {
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
                    IO_SetY(yi.LPCK_IonBlwrBtm, false);
                    IO_SetY(yi.LPCK_AirBlwrBtm1, false);
                    IO_SetY(yi.LPCK_AirBlwrBtm2, false);
                    CL_Move(ci.LPCK_PickerDnUp, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 11: 
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd)) return false ;
                    MT_GoHome(mi.LPCK_Y);
                    Step.iHome++;
                    return false;


                case 12: 
                    if(!MT_GetHomeDone(mi.LPCK_Y)) return false;
                    MoveMotr(mi.LPCK_Y, pv.LPCK_YPick);

                    Step.iHome++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.LPCK_Y, pv.LPCK_YPick)) return false;

                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CyclePick()
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



            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    //if (dSttTime != 0)
                    //{
                    //    dEndTime = DateTime.Now.ToOADate();
                    //}
                    //OM.EqpStat.dCycleTime = dEndTime - dSttTime;
                    //
                    //dSttTime = DateTime.Now.ToOADate();

                    MoveCyl(ci.LPCK_PickerDnUp, fb.Bwd);

                    IO_SetY(yi.LPCK_Eject1, true);
                    IO_SetY(yi.LPCK_Eject2, true);
                    IO_SetY(yi.LPCK_Eject3, true);
                    IO_SetY(yi.LPCK_Eject4, true);
                    IO_SetY(yi.LPCK_Eject5, true);
                    IO_SetY(yi.LPCK_Eject6, true);

                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd)) return false;
                    MoveMotr(mi.LPCK_Y, pv.LPCK_YPick);
                    
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.LPCK_Y, pv.LPCK_YPick)) return false;
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iLPCKBfPickDly)) return false;
                    MoveCyl(ci.LPCK_PickerDnUp, fb.Fwd);
                    
                    Step.iCycle++;
                    return false;
                
                case 13: 
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Fwd)) return false;
                    IO_SetY(yi.LPCK_Eject1, false);
                    IO_SetY(yi.LPCK_Eject2, false);
                    IO_SetY(yi.LPCK_Eject3, false);
                    IO_SetY(yi.LPCK_Eject4, false);
                    IO_SetY(yi.LPCK_Eject5, false);
                    IO_SetY(yi.LPCK_Eject6, false);

                    Step.iCycle++;
                    return false;

                case 14:
                    IO_SetY(yi.LPCK_Vacuum1, true);
                    IO_SetY(yi.LPCK_Vacuum2, true);
                    IO_SetY(yi.LPCK_Vacuum3, true);
                    IO_SetY(yi.LPCK_Vacuum4, true);
                    IO_SetY(yi.LPCK_Vacuum5, true);
                    IO_SetY(yi.LPCK_Vacuum6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iLPCKPickDly)) return false;

                    

                    MoveCyl(ci.LPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd)) return false;
                    if((!SEQ.PSTR.m_bVacSkip1 && !IO_GetX(xi.LPCK_Vacuum1)) || (!SEQ.PSTR.m_bVacSkip2 && !IO_GetX(xi.LPCK_Vacuum2)) ||
                       (!SEQ.PSTR.m_bVacSkip3 && !IO_GetX(xi.LPCK_Vacuum3)) || (!SEQ.PSTR.m_bVacSkip4 && !IO_GetX(xi.LPCK_Vacuum4)) ||
                       (!SEQ.PSTR.m_bVacSkip5 && !IO_GetX(xi.LPCK_Vacuum5)) || (!SEQ.PSTR.m_bVacSkip6 && !IO_GetX(xi.LPCK_Vacuum6)))
                    {
                        Step.iCycle = 50;
                        return false;
                    } 
                    DM.ARAY[ri.LPCK].SetStat(cs.Cleaning);
                    DM.ARAY[ri.PRER].SetStat(cs.Empty);
                    
                    Step.iCycle = 0;
                    return true;

                //위에서 씀
                //Pick Miss 패턴
                case 50:
                    MoveCyl(ci.LPCK_PickerDnUp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 51:
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Fwd)) return false;
                    IO_SetY(yi.LPCK_Vacuum1, false);
                    IO_SetY(yi.LPCK_Vacuum2, false);
                    IO_SetY(yi.LPCK_Vacuum3, false);
                    IO_SetY(yi.LPCK_Vacuum4, false);
                    IO_SetY(yi.LPCK_Vacuum5, false);
                    IO_SetY(yi.LPCK_Vacuum6, false);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 52:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    IO_SetY(yi.LPCK_Eject1, true);
                    IO_SetY(yi.LPCK_Eject2, true);
                    IO_SetY(yi.LPCK_Eject3, true);
                    IO_SetY(yi.LPCK_Eject4, true);
                    IO_SetY(yi.LPCK_Eject5, true);
                    IO_SetY(yi.LPCK_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 53:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iLPCKPlaceDly)) return false;
                    MoveCyl(ci.LPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 54:
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd)) return false;
                    IO_SetY(yi.LPCK_Eject1, false);
                    IO_SetY(yi.LPCK_Eject2, false);
                    IO_SetY(yi.LPCK_Eject3, false);
                    IO_SetY(yi.LPCK_Eject4, false);
                    IO_SetY(yi.LPCK_Eject5, false);
                    IO_SetY(yi.LPCK_Eject6, false);

                    Step.iCycle++;
                    return false;

                case 55:
                    ER_SetErr(ei.PCK_PickMiss, "Left Picker Pick up miss");
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        double m_dRunVel = 0;
        public bool CycleClean()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                IO_SetY(yi.LPCK_IonBlwrBtm, false);
                IO_SetY(yi.LPCK_AirBlwrBtm1, false);
                IO_SetY(yi.LPCK_AirBlwrBtm2, false);
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
                    MoveCyl(ci.LPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd)) return false;
                    MoveMotr(mi.LPCK_Y, pv.LPCK_YPick);

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.LPCK_Y, pv.LPCK_YPick)) return false;
                    if(!OM.CmnOptn.bLPCK_IonOff ) IO_SetY(yi.LPCK_IonBlwrBtm, true);
                    if(!OM.CmnOptn.bLPCK_Air1Off) IO_SetY(yi.LPCK_AirBlwrBtm1, true);
                    if(!OM.CmnOptn.bLPCK_Air2Off) IO_SetY(yi.LPCK_AirBlwrBtm2, true);
                    

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!OM.CmnOptn.bLPCK_IonOff && !IO_GetY(yi.LPCK_IonBlwrBtm)) return false;
                    if (!OM.CmnOptn.bLPCK_Air1Off && !IO_GetY(yi.LPCK_AirBlwrBtm1)) return false;
                    if (!OM.CmnOptn.bLPCK_Air2Off && !IO_GetY(yi.LPCK_AirBlwrBtm2)) return false;

                    m_dRunVel = MT_GetRunVel(mi.LPCK_Y); //런속도 받아오기
                    MoveMotr(mi.LPCK_Y, pv.LPCK_YPlace);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (MT_GetCmdPos(mi.LPCK_Y) > PM.GetValue(mi.LPCK_Y, pv.LPCK_YCleanStt)) return false;
                    MT_OverrideVel(mi.LPCK_Y,OM.DevInfo.dLPCKClnSpeed); //속도 오버로드
                    Step.iCycle++;
                    return false;
                    
                case 15:
                    if (MT_GetCmdPos(mi.LPCK_Y) > PM.GetValue(mi.LPCK_Y, pv.LPCK_YCleanEnd)) return false;
                    MT_OverrideVel(mi.LPCK_Y,m_dRunVel); //속도 오버로드

                    IO_SetY(yi.LPCK_IonBlwrBtm, false); //주석 가능성있어서 밑에 냅둠
                    IO_SetY(yi.LPCK_AirBlwrBtm1, false);
                    IO_SetY(yi.LPCK_AirBlwrBtm2, false);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStopPos(mi.LPCK_Y, pv.LPCK_YPlace)) return false;

                    IO_SetY(yi.LPCK_IonBlwrBtm, false);
                    IO_SetY(yi.LPCK_AirBlwrBtm1, false);
                    IO_SetY(yi.LPCK_AirBlwrBtm2, false);
                    DM.ARAY[ri.LPCK].SetStat(cs.Place);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePlace()
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
                    MoveCyl(ci.LPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd)) return false;
                    MoveMotr(mi.LPCK_Y, pv.LPCK_YPlace);

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.LPCK_Y, pv.LPCK_YPlace)) return false;
                    if(!MT_GetStopPos(mi.VSTG_X, pv.VSTG_XWorkStt))
                    {
                        ER_SetErr(ei.PKG_Dispr, "Vacuum Stage is not in Place Position.");
                        return true;
                    }
                    
                    Step.iCycle++;
                    return false;
                
                case 13: 
                    IO_SetY(yi.VSTG_Eject1, false);
                    IO_SetY(yi.VSTG_Eject2, false);
                    IO_SetY(yi.VSTG_Eject3, false);
                    IO_SetY(yi.VSTG_Eject4, false);
                    IO_SetY(yi.VSTG_Eject5, false);
                    IO_SetY(yi.VSTG_Eject6, false);

                    Step.iCycle++;
                    return false;

                case 14:
                    IO_SetY(yi.VSTG_Vacuum1, true);
                    IO_SetY(yi.VSTG_Vacuum2, true);
                    IO_SetY(yi.VSTG_Vacuum3, true);
                    IO_SetY(yi.VSTG_Vacuum4, true);
                    IO_SetY(yi.VSTG_Vacuum5, true);
                    IO_SetY(yi.VSTG_Vacuum6, true);

                    Step.iCycle++;
                    return false;

                case 15:
                    MoveCyl(ci.LPCK_PickerDnUp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Fwd)) return false;
                    IO_SetY(yi.LPCK_Vacuum1, false);
                    IO_SetY(yi.LPCK_Vacuum2, false);
                    IO_SetY(yi.LPCK_Vacuum3, false);
                    IO_SetY(yi.LPCK_Vacuum4, false);
                    IO_SetY(yi.LPCK_Vacuum5, false);
                    IO_SetY(yi.LPCK_Vacuum6, false);

                    Step.iCycle++;
                    return false;

                case 17:
                    IO_SetY(yi.LPCK_Eject1, true);
                    IO_SetY(yi.LPCK_Eject2, true);
                    IO_SetY(yi.LPCK_Eject3, true);
                    IO_SetY(yi.LPCK_Eject4, true);
                    IO_SetY(yi.LPCK_Eject5, true);
                    IO_SetY(yi.LPCK_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iLPCKPlaceDly)) return false;

                    if((!SEQ.PSTR.m_bVacSkip1 && !IO_GetX(xi.VSTG_Vacuum1)) || (!SEQ.PSTR.m_bVacSkip2 && !IO_GetX(xi.VSTG_Vacuum2)) ||
                       (!SEQ.PSTR.m_bVacSkip3 && !IO_GetX(xi.VSTG_Vacuum3)) || (!SEQ.PSTR.m_bVacSkip4 && !IO_GetX(xi.VSTG_Vacuum4)) ||
                       (!SEQ.PSTR.m_bVacSkip5 && !IO_GetX(xi.VSTG_Vacuum5)) || (!SEQ.PSTR.m_bVacSkip6 && !IO_GetX(xi.VSTG_Vacuum6)))
                    {
                        Step.iCycle = 50;
                        return false;
                    }

                    //if(!IO_GetX(xi.VSTG_Vacuum1) || !IO_GetX(xi.VSTG_Vacuum2) ||
                    //   !IO_GetX(xi.VSTG_Vacuum3) || !IO_GetX(xi.VSTG_Vacuum4) ||
                    //   !IO_GetX(xi.VSTG_Vacuum5) || !IO_GetX(xi.VSTG_Vacuum6))
                    //{
                    //    Step.iCycle = 50;
                    //    return false;
                    //}

                    MoveCyl(ci.LPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 19:
                    if(!CL_Complete(ci.LPCK_PickerDnUp , fb.Bwd)) return false;
                    IO_SetY(yi.LPCK_Eject1, false);
                    IO_SetY(yi.LPCK_Eject2, false);
                    IO_SetY(yi.LPCK_Eject3, false);
                    IO_SetY(yi.LPCK_Eject4, false);
                    IO_SetY(yi.LPCK_Eject5, false);
                    IO_SetY(yi.LPCK_Eject6, false);

                    DM.ARAY[ri.VSTG].SetStat(cs.Cleaning);

                    Step.iCycle++;
                    return false;

                case 20:
                    //MoveMotr(mi.LPCK_Y, pv.LPCK_YPick);

                    Step.iCycle++;
                    return false;

                case 21:
                    //if(!MT_GetStopPos(mi.LPCK_Y, pv.LPCK_YPick)) return false;
                    DM.ARAY[ri.LPCK].SetStat(cs.Pckrclean    );
                    

                    Step.iCycle = 0;
                    return true;

                //Use case 18
                case 50:
                    MoveCyl(ci.LPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 51:
                    if (!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd)) return false;
                    MT_Stop(mi.PRER_X);
                    ER_SetErr(ei.VSTG_VacMiss, "Stage Vacuum Miss");

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePckrClean()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                IO_SetY(yi.LPCK_AirBlwrBtm1, false);
                IO_SetY(yi.LPCK_AirBlwrBtm2, false);
                IO_SetY(yi.LPCK_IonBlwrBtm, false);
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
                    MoveCyl(ci.LPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd)) return false;
                    MoveMotr(mi.LPCK_Y, pv.LPCK_YPlace);

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.LPCK_Y, pv.LPCK_YPlace)) return false;
                    if (!OM.CmnOptn.bLPCK_IonOff ) IO_SetY(yi.LPCK_IonBlwrBtm , true);
                    if (!OM.CmnOptn.bLPCK_Air1Off) IO_SetY(yi.LPCK_AirBlwrBtm1, true);
                    if (!OM.CmnOptn.bLPCK_Air2Off) IO_SetY(yi.LPCK_AirBlwrBtm2, true);
                    

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!OM.CmnOptn.bLPCK_IonOff  && !IO_GetY(yi.LPCK_IonBlwrBtm )) return false;
                    if (!OM.CmnOptn.bLPCK_Air1Off && !IO_GetY(yi.LPCK_AirBlwrBtm1)) return false;
                    if (!OM.CmnOptn.bLPCK_Air2Off && !IO_GetY(yi.LPCK_AirBlwrBtm2)) return false;

                    m_dRunVel = MT_GetRunVel(mi.LPCK_Y); //런속도 받아오기
                    MoveMotr(mi.LPCK_Y, pv.LPCK_YPick);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (MT_GetCmdPos(mi.LPCK_Y) > PM.GetValue(mi.LPCK_Y, pv.LPCK_YCleanEnd)) return false;
                    MT_OverrideVel(mi.LPCK_Y, OM.DevInfo.dLPCKClnSpeed); //속도 오버로드
                    Step.iCycle++;
                    return false;

                case 15:
                    if (MT_GetCmdPos(mi.LPCK_Y) < PM.GetValue(mi.LPCK_Y, pv.LPCK_YCleanStt)) return false;
                    MT_OverrideVel(mi.LPCK_Y, m_dRunVel); //속도 오버로드

                    IO_SetY(yi.LPCK_IonBlwrBtm , false); //주석 가능성있어서 밑에 냅둠
                    IO_SetY(yi.LPCK_AirBlwrBtm1, false);
                    IO_SetY(yi.LPCK_AirBlwrBtm2, false);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStopPos(mi.LPCK_Y, pv.LPCK_YPick)) return false;

                    IO_SetY(yi.LPCK_IonBlwrBtm , false);
                    IO_SetY(yi.LPCK_AirBlwrBtm1, false);
                    IO_SetY(yi.LPCK_AirBlwrBtm2, false);
                    DM.ARAY[ri.LPCK].SetStat(cs.None);
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
                    if (IO_GetY(yi.LPCK_Vacuum1) || IO_GetY(yi.LPCK_Vacuum2) ||
                        IO_GetY(yi.LPCK_Vacuum3) || IO_GetY(yi.LPCK_Vacuum4) ||
                        IO_GetY(yi.LPCK_Vacuum5) || IO_GetY(yi.LPCK_Vacuum6))
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    IO_SetY(yi.LPCK_Vacuum1, true);
                    IO_SetY(yi.LPCK_Vacuum2, true);
                    IO_SetY(yi.LPCK_Vacuum3, true);
                    IO_SetY(yi.LPCK_Vacuum4, true);
                    IO_SetY(yi.LPCK_Vacuum5, true);
                    IO_SetY(yi.LPCK_Vacuum6, true);

                    Step.iCycle = 0;
                    return true;

                case 20:
                    IO_SetY(yi.LPCK_Vacuum1, false);
                    IO_SetY(yi.LPCK_Vacuum2, false);
                    IO_SetY(yi.LPCK_Vacuum3, false);
                    IO_SetY(yi.LPCK_Vacuum4, false);
                    IO_SetY(yi.LPCK_Vacuum5, false);
                    IO_SetY(yi.LPCK_Vacuum6, false);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 21:
                    if(!m_tmDelay.OnDelay(100)) return false;
                    IO_SetY(yi.LPCK_Eject1, true);
                    IO_SetY(yi.LPCK_Eject2, true);
                    IO_SetY(yi.LPCK_Eject3, true);
                    IO_SetY(yi.LPCK_Eject4, true);
                    IO_SetY(yi.LPCK_Eject5, true);
                    IO_SetY(yi.LPCK_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    IO_SetY(yi.LPCK_Eject1, false);
                    IO_SetY(yi.LPCK_Eject2, false);
                    IO_SetY(yi.LPCK_Eject3, false);
                    IO_SetY(yi.LPCK_Eject4, false);
                    IO_SetY(yi.LPCK_Eject5, false);
                    IO_SetY(yi.LPCK_Eject6, false);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.LPCK_PickerDnUp)
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

            if (_eMotr == mi.LPCK_Y)
            {
                if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd))
                {
                    sMsg = CL_GetName(ci.LPCK_PickerDnUp) + "Cylinder is Down";
                    bRet = false;
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

        public bool JogCheckSafe(mi _eMotr, EN_JOG_DIRECTION _eDir, EN_UNIT_TYPE _eType, double _dDist)
        {
            if (OM.MstOptn.bDebugMode) return true;
            bool bRet = true;
            string sMsg = "";

            if (_eMotr == mi.LPCK_Y)
            {
                if(!CL_Complete(ci.LPCK_PickerDnUp, fb.Bwd))
                {
                    sMsg = CL_GetName(ci.LPCK_PickerDnUp) + "Cylinder is Down";
                    bRet = false;
                }
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
            if( !MT_GetStop(mi.LPCK_Y)) return false;
            
            if (!CL_Complete(ci.LPCK_PickerDnUp)) return false;

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
