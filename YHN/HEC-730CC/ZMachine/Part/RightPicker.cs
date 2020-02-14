using System;
using COMMON;
using SML;
using System.Runtime.CompilerServices;

namespace Machine
{
    //PULD == PreUnloader
    public class RightPicker : Part
    {
               //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
            public bool bReqStop   ;
            public void Clear()
            {
                bWorkEnd    = false;
                bReqStop    = false;
                bSupply     = false;
            }
            public bool bSupply    ;
        };   
        public enum sc
        {
            Idle    = 0,
            Pick       ,
            Move       ,
            Place      ,
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
        public double dUldrStartTime = 0.0;
        public double dUldrEndTime = 0.0;

        public RightPicker(int _iPartId = 0)
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
            //if (!SEQ.InspectLightGrid()) Stat.bSafetyStop = true;
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
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Bwd);

                    Step.iToStart++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;

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
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Bwd);
                    
                    Step.iToStop++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;
                    Step.iToStop = 0;
                    return true;
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

                //조건
                bool bNone  = DM.ARAY[ri.RPCK].CheckAllStat(cs.None    ) && DM.ARAY[ri.PSTR].CheckAllStat(cs.None) && DM.ARAY[ri.VSTG].CheckAllStat(cs.Empty);
                bool bPick  = DM.ARAY[ri.RPCK].CheckAllStat(cs.Pick    ) && DM.ARAY[ri.VSTG].CheckAllStat(cs.Clean);
                bool bMove  = DM.ARAY[ri.RPCK].CheckAllStat(cs.Move);
                bool bPlace = DM.ARAY[ri.RPCK].CheckAllStat(cs.Place   ) && DM.ARAY[ri.PSTR].CheckAllStat(cs.Empty);
                    
                //사이클
                bool isPick   = bPick ;
                bool isMove   = bMove ;
                bool isPlace  = bPlace;
                bool isEnd    = bNone ;

                //모르는 카세트 에러
                //if(!isPick){
                //    if(None && (bMgzSsr1 || bMgzSsr2)) ER_SetErr(ei.PKG_Unknwn,"Unloader have no data found, but mgz sensor detected") ;
                //}
                //카세트 사라짐
                //if(!isDrop){
                //    if(!None && ( !bMgzSsr1 && !bMgzSsr2)) ER_SetErr(ei.PKG_Dispr,"Unloader have data, but mgz sensor not detected") ;
                //}
                //카세트 필요
                //if(  None && !IO_GetX(xi.ULDR_MgzIn) && Stat.bSupply && !NonePsb) ER_SetErr(ei.PRT_NeedMgz,"Unloader need to magazine") ;
                //카세트 꽉참
                //if(  None && !IO_GetX(xi.ULDR_MgzIn) && Stat.bSupply && !NonePsb) ER_SetErr(ei.PRT_NeedMgz,"Unloader need to magazine") ;


                if (ER_IsErr())
                {
                    return false;
                }

                //Normal Decide Step.
                     if (isPick   ) { DM.ARAY[ri.RPCK].Trace(m_iPartId); DM.ARAY[ri.VSTG].Trace(m_iPartId); Step.eSeq = sc.Pick ; }
                else if (isMove   ) { DM.ARAY[ri.RPCK].Trace(m_iPartId);                                    Step.eSeq = sc.Move ; }
                else if (isPlace  ) { DM.ARAY[ri.RPCK].Trace(m_iPartId); DM.ARAY[ri.PSTR].Trace(m_iPartId); Step.eSeq = sc.Place; }
                else if (isEnd    ) { Stat.bWorkEnd = true; return true; }

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
                default         : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case sc.Idle    : return false;
                case sc.Pick    : if (!CyclePick ()) return false; break;
                case sc.Move    : if (!CycleMove ()) return false; break;
                case sc.Place   : if (!CyclePlace()) return false; break;
                
            }       
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;                                    
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            r = 0 ;
            c = 0 ;
            DM.ARAY[_iId].FindFrstColLastRow( ref c, ref r , _iChip);
            return (c >= 0 && r >= 0) ? true : false;
        }       

        public bool CycleHome()
        {
            string sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
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
                    if(Step.iHome != PreStep.iHome) Trace(sTemp);
                    return true ;
            
                case 10:
                    CL_Move(ci.RPCK_PickerDnUp, fb.Bwd);
                    
                    Step.iHome++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;
                    MT_GoHome(mi.RPCK_Y);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetHomeDone(mi.RPCK_Y)) return false;
                    MoveMotr(mi.RPCK_Y, pv.RPCK_YPick);

                    Step.iHome++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.RPCK_Y, pv.RPCK_YPick)) return false;

                    Step.iHome = 0;
                    return true ;
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

            bool r, c ;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;
                    MoveMotr(mi.RPCK_Y, pv.RPCK_YPick);

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.RPCK_Y, pv.RPCK_YPick)) return false;
                    if(!MT_GetStopPos(mi.VSTG_X, pv.VSTG_XWorkEnd))
                    {
                        ER_SetErr(ei.PKG_Dispr, "Vacuum Stage is not in Pick Position.");
                        return true;
                    }
                    
                    Step.iCycle++;
                    return false;
                
                case 13: 
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Fwd)) return false;
                    //IO_SetY(yi.RPCK_Eject1, false);
                    //IO_SetY(yi.RPCK_Eject2, false);
                    //IO_SetY(yi.RPCK_Eject3, false);
                    //IO_SetY(yi.RPCK_Eject4, false);
                    //IO_SetY(yi.RPCK_Eject5, false);
                    //IO_SetY(yi.RPCK_Eject6, false);

                    Step.iCycle++;
                    return false;

                case 15:
                    IO_SetY(yi.RPCK_Vacuum1, true);
                    IO_SetY(yi.RPCK_Vacuum2, true);
                    IO_SetY(yi.RPCK_Vacuum3, true);
                    IO_SetY(yi.RPCK_Vacuum4, true);
                    IO_SetY(yi.RPCK_Vacuum5, true);
                    IO_SetY(yi.RPCK_Vacuum6, true);

                    Step.iCycle++;
                    return false;

                case 16:
                    
                    IO_SetY(yi.VSTG_Vacuum1, false);
                    IO_SetY(yi.VSTG_Vacuum2, false);
                    IO_SetY(yi.VSTG_Vacuum3, false);
                    IO_SetY(yi.VSTG_Vacuum4, false);
                    IO_SetY(yi.VSTG_Vacuum5, false);
                    IO_SetY(yi.VSTG_Vacuum6, false);

                    Step.iCycle++;
                    return false;

                case 17:
                    IO_SetY(yi.VSTG_Eject1, true);
                    IO_SetY(yi.VSTG_Eject2, true);
                    IO_SetY(yi.VSTG_Eject3, true);
                    IO_SetY(yi.VSTG_Eject4, true);
                    IO_SetY(yi.VSTG_Eject5, true);
                    IO_SetY(yi.VSTG_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iRPCKPickDly)) return false;

                    
                    IO_SetY(yi.VSTG_Eject1, false);
                    IO_SetY(yi.VSTG_Eject2, false);
                    IO_SetY(yi.VSTG_Eject3, false);
                    IO_SetY(yi.VSTG_Eject4, false);
                    IO_SetY(yi.VSTG_Eject5, false);
                    IO_SetY(yi.VSTG_Eject6, false);

                    MoveCyl(ci.RPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 19:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;
                    if ((!SEQ.PSTR.m_bVacSkip1 && !IO_GetX(xi.RPCK_Vacuum1)) || (!SEQ.PSTR.m_bVacSkip2 && !IO_GetX(xi.RPCK_Vacuum2)) ||
                        (!SEQ.PSTR.m_bVacSkip3 && !IO_GetX(xi.RPCK_Vacuum3)) || (!SEQ.PSTR.m_bVacSkip4 && !IO_GetX(xi.RPCK_Vacuum4)) ||
                        (!SEQ.PSTR.m_bVacSkip5 && !IO_GetX(xi.RPCK_Vacuum5)) || (!SEQ.PSTR.m_bVacSkip6 && !IO_GetX(xi.RPCK_Vacuum6)))
                    {
                        Step.iCycle = 50;
                        return false;
                    }
                    DM.ARAY[ri.RPCK].SetStat(cs.Move);
                    DM.ARAY[ri.VSTG].SetStat(cs.WorkEnd);

                    Step.iCycle = 0;
                    return true;

                //위에서 씀
                //Pick Miss 패턴
                case 50:
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 51:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Fwd)) return false;
                    IO_SetY(yi.RPCK_Vacuum1, false);
                    IO_SetY(yi.RPCK_Vacuum2, false);
                    IO_SetY(yi.RPCK_Vacuum3, false);
                    IO_SetY(yi.RPCK_Vacuum4, false);
                    IO_SetY(yi.RPCK_Vacuum5, false);
                    IO_SetY(yi.RPCK_Vacuum6, false);

                    Step.iCycle++;
                    return false;

                case 52:
                    IO_SetY(yi.RPCK_Eject1, true);
                    IO_SetY(yi.RPCK_Eject2, true);
                    IO_SetY(yi.RPCK_Eject3, true);
                    IO_SetY(yi.RPCK_Eject4, true);
                    IO_SetY(yi.RPCK_Eject5, true);
                    IO_SetY(yi.RPCK_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 53:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iRPCKPlaceDly)) return false;
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 54:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;
                    IO_SetY(yi.RPCK_Eject1, false);
                    IO_SetY(yi.RPCK_Eject2, false);
                    IO_SetY(yi.RPCK_Eject3, false);
                    IO_SetY(yi.RPCK_Eject4, false);
                    IO_SetY(yi.RPCK_Eject5, false);
                    IO_SetY(yi.RPCK_Eject6, false);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 55:
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

                case 56:
                    if (!m_tmDelay.OnDelay(200)) return false;
                    IO_SetY(yi.VSTG_Eject1, false);
                    IO_SetY(yi.VSTG_Eject2, false);
                    IO_SetY(yi.VSTG_Eject3, false);
                    IO_SetY(yi.VSTG_Eject4, false);
                    IO_SetY(yi.VSTG_Eject5, false);
                    IO_SetY(yi.VSTG_Eject6, false);
                    ER_SetErr(ei.PCK_PickMiss, "Right Picker Pick up miss");
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleMove()
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

            //bool r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;
                    MoveMotr(mi.RPCK_Y, pv.RPCK_YPlace);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.RPCK_Y, pv.RPCK_YPlace)) return false;
                    DM.ARAY[ri.RPCK].SetStat(cs.Place);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePlace()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 6000))
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

            //bool r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;
                    MoveMotr(mi.RPCK_Y, pv.RPCK_YPlace);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.RPCK_Y, pv.RPCK_YPlace)) return false;
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Fwd)) return false;
                    IO_SetY(yi.RPCK_Vacuum1, false);
                    IO_SetY(yi.RPCK_Vacuum2, false);
                    IO_SetY(yi.RPCK_Vacuum3, false);
                    IO_SetY(yi.RPCK_Vacuum4, false);
                    IO_SetY(yi.RPCK_Vacuum5, false);
                    IO_SetY(yi.RPCK_Vacuum6, false);


                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    IO_SetY(yi.RPCK_Eject1, true);
                    IO_SetY(yi.RPCK_Eject2, true);
                    IO_SetY(yi.RPCK_Eject3, true);
                    IO_SetY(yi.RPCK_Eject4, true);
                    IO_SetY(yi.RPCK_Eject5, true);
                    IO_SetY(yi.RPCK_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iRPCKPlaceDly)) return false;
                    MoveCyl(ci.RPCK_PickerDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd)) return false;
                    
                    IO_SetY(yi.RPCK_Eject1, false);
                    IO_SetY(yi.RPCK_Eject2, false);
                    IO_SetY(yi.RPCK_Eject3, false);
                    IO_SetY(yi.RPCK_Eject4, false);
                    IO_SetY(yi.RPCK_Eject5, false);
                    IO_SetY(yi.RPCK_Eject6, false);

                    DM.ARAY[ri.PSTR].SetStat(cs.Clean);

                    Step.iCycle++;
                    return false;

                case 17:
                    MoveMotr(mi.RPCK_Y, pv.RPCK_YPick);

                    Step.iCycle++;
                    return false;

                case 18:
                    if(!MT_GetStopPos(mi.RPCK_Y, pv.RPCK_YPick)) return false;
                    DM.ARAY[ri.RPCK].SetStat(cs.None   );
                    

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
                    if (IO_GetY(yi.RPCK_Vacuum1) || IO_GetY(yi.RPCK_Vacuum2) ||
                        IO_GetY(yi.RPCK_Vacuum3) || IO_GetY(yi.RPCK_Vacuum4) ||
                        IO_GetY(yi.RPCK_Vacuum5) || IO_GetY(yi.RPCK_Vacuum6))
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    IO_SetY(yi.RPCK_Vacuum1, true);
                    IO_SetY(yi.RPCK_Vacuum2, true);
                    IO_SetY(yi.RPCK_Vacuum3, true);
                    IO_SetY(yi.RPCK_Vacuum4, true);
                    IO_SetY(yi.RPCK_Vacuum5, true);
                    IO_SetY(yi.RPCK_Vacuum6, true);

                    Step.iCycle = 0;
                    return true;

                case 20:
                    IO_SetY(yi.RPCK_Vacuum1, false);
                    IO_SetY(yi.RPCK_Vacuum2, false);
                    IO_SetY(yi.RPCK_Vacuum3, false);
                    IO_SetY(yi.RPCK_Vacuum4, false);
                    IO_SetY(yi.RPCK_Vacuum5, false);
                    IO_SetY(yi.RPCK_Vacuum6, false);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    IO_SetY(yi.RPCK_Eject1, true);
                    IO_SetY(yi.RPCK_Eject2, true);
                    IO_SetY(yi.RPCK_Eject3, true);
                    IO_SetY(yi.RPCK_Eject4, true);
                    IO_SetY(yi.RPCK_Eject5, true);
                    IO_SetY(yi.RPCK_Eject6, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    IO_SetY(yi.RPCK_Eject1, false);
                    IO_SetY(yi.RPCK_Eject2, false);
                    IO_SetY(yi.RPCK_Eject3, false);
                    IO_SetY(yi.RPCK_Eject4, false);
                    IO_SetY(yi.RPCK_Eject5, false);
                    IO_SetY(yi.RPCK_Eject6, false);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.RPCK_PickerDnUp)
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

        public bool CheckSafe(mi _eMotr, pv _ePstn ,  double _dOfsPos=0)
        {
            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";
            
            if (_eMotr == mi.RPCK_Y)
            {
                if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd))
                {
                    sMsg = CL_GetName(ci.RPCK_PickerDnUp) + "Cylinder is Down";
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
                if (Step.eSeq == 0) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
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

            if (_eMotr == mi.RPCK_Y)
            {
                if(!CL_Complete(ci.RPCK_PickerDnUp, fb.Bwd))
                {
                    sMsg = CL_GetName(ci.RPCK_PickerDnUp) + "Cylinder is Down";
                    bRet = false;
                }
                //Bwd
                if (_eDir == EN_JOG_DIRECTION.Neg)
                {

                    if (_eType == EN_UNIT_TYPE.utJog)
                    {

                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
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
            if (!MT_GetStop(mi.RPCK_Y)) return false;

            if (!CL_Complete(ci.RPCK_PickerDnUp)) return false;

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
