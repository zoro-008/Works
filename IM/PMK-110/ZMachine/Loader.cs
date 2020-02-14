using System;
using COMMON;
using SML;

namespace Machine
{
    //LDR == Loader
    public class Loader : PartInterface
    {
        //public-------------------------------------------------------------------
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop;
            public void Clear()
            {
                bWorkEnd = false;
                bReqStop = false;
            }

        };    //sun Clear When LOT Open. and every 30Sec in autorun()

        public enum sc
        {
            Idle   = 0 ,
            //Supply = 1 ,
            Pick   = 1 ,
            Place  = 2 ,
            MAX_SEQ_CYCLE
        };

        public struct SStep
        {
            public int iHome;
            public int iToStart;
            public sc  eSeq;
            public int iCycle;
            public int iToStop;
            public sc  eLastSeq;
            public void Clear()
            {
                iHome = 0;
                iToStart = 0;
                eSeq = sc.Idle;
                iCycle = 0;
                iToStop = 0;
                eLastSeq = sc.Idle;
            }
        };
        //protected-------------------------------------------------------------------
        protected String      m_sPartName;
        //protected CCycleTimer m_tmDispr  ;
        //Timer.
        protected CDelayTimer m_tmMain   ;
        protected CDelayTimer m_tmCycle  ;
        protected CDelayTimer m_tmHome   ;
        protected CDelayTimer m_tmToStop ;
        protected CDelayTimer m_tmToStart;
        protected CDelayTimer m_tmDelay  ;        

        protected SStat Stat;
        protected SStep Step, PreStep;

        protected TWorkInfo WorkInfo;

        protected double m_dLastIdxPos;
        protected String m_sCheckSafeMsg;

        public string[] m_sCycleName;
        public CTimer[] m_CycleTime;


        public Loader()
        {
            m_sPartName = "Loader";

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();

            m_sCycleName  = new string[(int)sc.MAX_SEQ_CYCLE];
            m_CycleTime   = new CTimer[(int)sc.MAX_SEQ_CYCLE];

            for(int i = 0 ; i < (int)sc.MAX_SEQ_CYCLE ; i++)
            {
                m_CycleTime [i]  = new CTimer();
            }

            Reset();
            
            InitCycleName();
            InitCycleTime();                        
        }


        //PartInterface 부분.
        //인터페이스 상속.====================================================================================================================
        public void Reset() //리셑 버튼 눌렀을때 타는 함수.
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
        //
        public void Update()
        {

        }
        public bool ToStopCon() //스탑을 하기 위한 조건을 보는 함수.
        {
            Stat.bReqStop = true;
            //During the auto run, do not stop.
            if (Step.eSeq != sc.Idle) return false;

            Step.iToStop = 10;
            //Ok.
            return true;


        }
        public bool ToStartCon() //스타트를 하기 위한 조건을 보는 함수.
        {
            Step.iToStart = 10;
            //Ok.
            return true;

        }
        public bool ToStart() //스타트를 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 5000)) SetErr(ei.ETC_ToStartTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

            String sTemp;
            sTemp = String.Format("Step.iToStart={0:00}", Step.iToStart);
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
                    m_iTrayWorkCnt = 0 ;
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
        public bool ToStop() //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 10000)) SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

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
                    if(!SM.MTR.GetStop((int)mi.LDR_Z)) return false ;
                    MoveActr(ci.LDR_TrayFixClOp, fb.Bwd);
                    MoveActr(ci.LDR_GripDnUp, fb.Bwd);
                    //iTrayWorkCnt = 0;
                    Step.iToStop++;
                    return false;
                
                case 11:
                    if (!MoveActr(ci.LDR_TrayFixClOp, fb.Bwd)) return false;
                    if (!MoveActr(ci.LDR_GripDnUp   , fb.Bwd)) return false;
                    m_iTrayWorkCnt = 0;
                    MoveMotr(mi.LDR_Z, PM.GetValue(mi.LDR_Z, pv.LDR_ZWait));
                    MoveActr(ci.LDR_GripRtLt , fb.Fwd);
                    Step.iToStop++;
                    return false;

                case 12: 
                    if(!GetStop(mi.LDR_Z)) return false ;
                    if (!MoveActr(ci.LDR_GripRtLt, fb.Fwd)) return false;
                    Step.iToStop = 0;
                    return true;
            }


        }

        public int GetHomeStep   () { return Step.iHome    ; } public int GetPreHomeStep   () { return PreStep.iHome    ; } public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        public int GetToStartStep() { return Step.iToStart ; } public int GetPreToStartStep() { return PreStep.iToStart ; }
        public int GetSeqStep    () { return (int)Step.eSeq; } public int GetPreSeqStep    () { return (int)PreStep.eSeq; }
        public int GetCycleStep  () { return Step.iCycle   ; } public int GetPreCycleStep  () { return PreStep.iCycle   ; } public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; }
        public int GetToStopStep () { return Step.iToStop  ; } public int GetPreToStopStep () { return PreStep.iToStop  ; }
        public SStep GetStep     () { return Step; }
        public string GetSeqName() {  return Step.eSeq.ToString() ;}

        public string GetCrntCycleName(     ) { return Step.eSeq.ToString(); }
        public String GetCycleName(int _iSeq) { return m_sCycleName[_iSeq]; }
        public double GetCycleTime(int _iSeq) { return m_CycleTime [_iSeq].Duration; }
        public String GetPartName (         ) { return m_sPartName        ; }

        public int GetCycleMaxCnt() { return (int)sc.MAX_SEQ_CYCLE; }

        public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName();

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                //bool bInpos = false ;
                //for(int i = 0 ; i < OM.DevInfo.iTrayMaxLoading ; i++ )
                //{
                //    double dPos = PM.GetValue((uint)mi.LDR_Z , (uint)pv.LDR_ZWorkStt) * i ;
                //    double dCmd = SM.MTR.GetCmdPos((int)mi.LDR_Z) ;
                //
                //    if(dCmd - 0.1 < dPos && dPos < dCmd + 0.1)
                //    {
                //        bInpos = true ;
                //    }
                //
                //}

                bool isIdxEmpty     = DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None) && DM.ARAY[(int)ri.MRK].CheckAllStat(cs.None) ;//DM.ARAY[(int)ri.BAR].CheckAllStat(cs.None);
                //bool isCycleSupply  = DM.ARAY[(int)ri.PCK].CheckAllStat(cs.None)  && GetX(xi.LDR_TrayDetect) && (!bInpos || !GetX(xi.LDR_TrayPstn));
                bool isCyclePick    = DM.ARAY[(int)ri.PCK].CheckAllStat(cs.None ) && isIdxEmpty && !GetX(xi.LDR_TrayPstn) && GetX(xi.LDR_TrayDetect) ; // && bInpos ;
                bool isCyclePlace   = DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None ) && isIdxEmpty && //GetX(xi.LDR_TrayPstn) && !GetX(xi.IDX_Pri)
                                      DM.ARAY[(int)ri.PCK].CheckAllStat(cs.Unkwn) ;                      

                bool isCycleEnd = DM.ARAY[(int)ri.PCK].CheckAllStat(cs.None) && !GetX(xi.LDR_TrayDetect);


                //여기부터 조건 잡자.
                bool bWorkEnded = DM.ARAY[(int)ri.PCK].CheckAllStat(cs.None) &&
                                  DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None) &&
                                  DM.ARAY[(int)ri.MRK].CheckAllStat(cs.None) &&
                                  DM.ARAY[(int)ri.BAR].CheckAllStat(cs.None) &&
                                  !GetX(xi.LDR_TrayDetect);
                if (bWorkEnded)
                {
                    SetErr(ei.PRT_TrayErr, "Tray is empty, Supply Tray Plz.");
                    Step.iCycle = 0;
                    return true;
                }


                if (SM.ERR.IsErr())
                {
                    MoveActr(ci.LDR_TrayFixClOp, fb.Bwd);
                    return false;
                } 
                    
                //Normal Decide Step.
                //     if (isCycleSupply) { Step.eSeq = sc.Supply   ;  }
                //else if (isCyclePick  ) { Step.eSeq = sc.Pick     ;  } 

                     if (isCyclePick  ) { Step.eSeq = sc.Pick     ;  } 
                else if (isCyclePlace ) { Step.eSeq = sc.Place    ;  } 
                else if (isCycleEnd   ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if (Step.eSeq != sc.Idle)
                {
                    Log.Trace(Step.eSeq.ToString() + " Start");
                    Log.TraceListView(Step.eSeq.ToString() + " Cycle Started");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }

            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default         :                      return false;
                case (sc.Idle  ):                      return false;
                //case (sc.Supply): if (!CycleSupply())  return false; break ;
                case (sc.Pick  ): if (!CyclePick  ())  return false; break ;
                case (sc.Place ): if (!CyclePlace ())  return false; break ;
                
            }
            Log.Trace(sCycle + " End");
            Log.TraceListView(sCycle + " 동작 종료");
            m_CycleTime[(int)Step.eSeq].End();
            Step.eSeq = sc.Idle;
            return false;
        }

        //인터페이스 상속 끝.==================================================

        protected double GetLastCmd(mi _iMotr)
        {
            double dLastIdxPos = 0.0;

            //sunsun
                //if (!SM.MT.GetAlarmSgnl((int)_iMotr) && !SM.MT.GetPLimSnsr((int)_iMotr)) dLastIdxPos = SM.MT.GetCmdPos((int)_iMotr);
                //else dLastIdxPos = GetMotrPos(_iMotr, (EN_PSTN_VALUE)0);

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
        
        public bool FindChip(ref int c, ref int r, cs _iChip, ri _iId) //이거 되는지 확인 해야함
        { 
            //switch(OM.MstOptn.iColRowDir)
            //{
            //    case (int)EN_FINDCHIP.FrstRowCol     : DM.ARAY[(int)ri.LDR].FindFrstRowCol    (_iChip, ref c, ref r); break;
            //    case (int)EN_FINDCHIP.FrstColRow     : DM.ARAY[(int)ri.LDR].FindFrstColRow    (_iChip, ref c, ref r); break;
            //    case (int)EN_FINDCHIP.LastRowCol     : DM.ARAY[(int)ri.LDR].FindLastRowCol    (_iChip, ref c, ref r); break;
            //    case (int)EN_FINDCHIP.FrstRowLastCol : DM.ARAY[(int)ri.LDR].FindFrstRowLastCol(_iChip, ref c, ref r); break;
            //    case (int)EN_FINDCHIP.LastRowFrstCol : DM.ARAY[(int)ri.LDR].FindLastRowFrstCol(_iChip, ref c, ref r); break;
            //    case (int)EN_FINDCHIP.LastColFrstRow : DM.ARAY[(int)ri.LDR].FindLastColFrstRow(_iChip, ref c, ref r); break;
            //    case (int)EN_FINDCHIP.FrstColLastRow : DM.ARAY[(int)ri.LDR].FindFrstColLastRow(_iChip, ref c, ref r); break;
            //    case (int)EN_FINDCHIP.LastColRow     : DM.ARAY[(int)ri.LDR].FindLastColRow    (_iChip, ref c, ref r); break;
            //}
            //return OM.MstOptn.iColRowDir >= (int)EN_FINDCHIP.LastColRow ? false : true;
            return true;

        }
        protected struct TWorkInfo 
        {
            public int iCol ;
            public int iRow ;
            public cs eStat ;
        } ;//오토런에서 스테이지에서 정보를 가져다 담아 놓고 Cycle에서 이것을 쓴다....

        public double GetMotrPos (mi _iMotr , pv _iPstnValue )
        {
            return PM.GetValue((uint)_iMotr , (uint)_iPstnValue);
        }
        
        public void InitCycleName(){
            m_sCycleName = new String[(int)sc.MAX_SEQ_CYCLE];
            m_sCycleName[(int)sc.Idle     ]="Idle"        ;
            m_sCycleName[(int)sc.Pick   ]="Supply"      ;
            m_sCycleName[(int)sc.Place     ]="Work  "      ;
        }

        public void InitCycleTime()
        {
            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();
        }

        public static int m_iTrayWorkCnt = 0;
        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                SetErr(ei.ETC_AllHomeTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Log.Trace(m_sPartName, sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if(Stat.bReqStop) {
                //Step.iHome = 0;
                //return true ;
            }
            
            switch (Step.iHome) {

                default: sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                         if(Step.iHome != PreStep.iHome)Log.Trace(m_sPartName, sTemp);
                         Step.iHome = 0 ;
                         return true ;

                case 10: 
                    SM.CYL.Move((int)ci.LDR_GripDnUp, EN_CYL_POS.Bwd);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!SM.CYL.Move((int)ci.LDR_GripDnUp, EN_CYL_POS.Bwd))return false;
                    
                    Step.iHome++;
                    return false;

                case 12:
                    SM.CYL.Move((int)ci.LDR_TrayFixClOp, EN_CYL_POS.Bwd);
                    Step.iHome++;
                    return false;
                //로더에 실린더 하나 추가되면서 추가함 진섭.
                case 13:
                    if (!SM.CYL.Move((int)ci.LDR_TrayFixClOp, EN_CYL_POS.Bwd)) return false;
                    SM.MTR.GoHome((int)mi.LDR_Z);
                    Step.iHome++;
                    return false ;
            
                case 14:
                    if (!SM.MTR.GetHomeDone((int)mi.LDR_Z)) return false;
                    SM.CYL.Move((int)ci.LDR_GripRtLt, EN_CYL_POS.Fwd);
                    Step.iHome++;
                    return false ;

                case 15:
                    if (!SM.CYL.Move((int)ci.LDR_GripRtLt, EN_CYL_POS.Fwd)) return false;
                    m_iTrayWorkCnt = 0;
                    Step.iHome = 0;
                    return true ;
            }
        }

        //모터 포지션 확인후 ERR띄우기.        
        /*
        public static int iTrayImgNo = 0;
        public bool CycleSupply()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() , 100000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            double dLDRZTrayPos = 0.0;
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    if (!GetX(xi.LDR_TrayDetect))
                    {
                        SetErr(ei.PRT_TrayErr, "Tray is empty, Supply Tray Plz.");
                        Step.iCycle = 0;
                        return true;
                    }
                    
                    MoveActr(ci.LDR_GripClOp, fb.Bwd);
                    MoveActr(ci.LDR_TrayFixClOp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ci.LDR_GripClOp, fb.Bwd)) return false;
                    if (!GetActrStop(ci.LDR_TrayFixClOp, fb.Bwd)) return false;
                    MoveActr(ci.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                
                case 12:
                    if (!GetActrStop(ci.LDR_GripDnUp, fb.Bwd)) return false;
                    //MoveActr(ci.LDR_GripRtLt, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 13:
                    //if (!GetActrStop(ci.LDR_GripRtLt, fb.Bwd)) return false;
                    Step.iCycle++;
                    return false ;

                //아래서 씀.
                case 14:
                    //MoveMotr(mi.LDR_Z, pv.LDR_ZWorkStt);
                    //MoveActr(ci.LDR_TrayFixClOp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    //if (!MoveActr(ci.LDR_TrayFixClOp, fb.Bwd)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iLDRTrayCheckTime)) return false;
                    if (!GetStop(mi.LDR_Z)) return false;

                    if(SM.MTR.GetCmdPos((int)mi.LDR_Z) < )

                    if (GetX(xi.LDR_TrayOver))
                    {
                        SetErr(ei.PRT_TrayErr, "Tray is OverLoaded.");
                        Step.iCycle = 0;
                        return true;
                    }

                    dLDRZTrayPos = PM.GetValue(mi.LDR_Z, pv.LDR_ZWorkStt) + (OM.DevInfo.dTrayHeight * m_iTrayWorkCnt) ;
                    MoveMotr(mi.LDR_Z, dLDRZTrayPos);

                    Step.iCycle++;
                    return false;

                case 17:
                    if(!GetStop(mi.LDR_Z))return false;
                    Step.iCycle++;
                    return false;

                case 18:                    
                    if (!GetX(xi.LDR_TrayPstn))
                    {
                        m_iTrayWorkCnt++;
                        m_tmDelay.Clear();
                        Step.iCycle = 0;
                        return true;
                    }

                    if (GetX(xi.LDR_TrayOver))
                    {
                        m_iTrayWorkCnt=0;
                        SetErr(ei.PRT_TrayErr, "Tray is OverLoaded.");
                        Step.iCycle = 0;
                        return true;
                    }

                    //if (GetX(xi.LDR_TrayDir)) iTrayImgNo = 1;
                    //else                      iTrayImgNo = 0;

                    //ARAY Data 입력.
                    //DM.ARAY[(int)ri.PCK].ID      = m_iTrayWorkCnt.ToString() ;
                    //DM.ARAY[(int)ri.PCK].LotNo   = LOT.m_sLotNo;
                    //DM.ARAY[(int)ri.PCK].Step    = 0;
                    //DM.ARAY[(int)ri.PCK].SubStep = 0;//iTrayImgNo;

                    //DM.ARAY[(int)ri.PCK].SetStat(cs.Unkwn);
                    Step.iCycle = 0;
                    return true;
            }
        }*/

        //모터 포지션 확인후 ERR띄우기.        
        public static int iTrayImgNo = 0;
        public bool CyclePick()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 100000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            double dLDRZTrayPos = 0.0;
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    if (GetX(xi.LDR_TrayOver))
                    {
                        SetErr(ei.PRT_TrayErr, "Tray is OverLoaded.");
                        Step.iCycle = 0;
                        return true;
                    }

                    if(m_iTrayWorkCnt >= OM.DevInfo.iTrayMaxLoading)
                    {
                        SetErr(ei.PRT_TrayErr, "Loader WorkTrayCntMax is Over.");
                        Step.iCycle = 0;
                        return true;
                    }
                    MoveActr(ci.LDR_GripClOp, fb.Bwd);
                    MoveActr(ci.LDR_TrayFixClOp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ci.LDR_GripClOp, fb.Bwd)) return false;
                    if (!GetActrStop(ci.LDR_TrayFixClOp, fb.Bwd)) return false;
                    MoveActr(ci.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;


                case 12:
                    if (!GetActrStop(ci.LDR_GripDnUp, fb.Bwd)) return false;
                    MoveActr(ci.LDR_GripRtLt, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetActrStop(ci.LDR_GripRtLt, fb.Bwd)) return false;
                    Step.iCycle++;
                    return false;
                
                //여기부터 로더 올림.
                //아래서 씀.
                case 14:

                    Step.iCycle++;
                    return false;

                case 15:
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iLDRTrayCheckTime)) return false;
                    //if (!GetStop(mi.LDR_Z)) return false;

                    

                    dLDRZTrayPos = PM.GetValue(mi.LDR_Z, pv.LDR_ZWorkStt) + (OM.DevInfo.dTrayHeight * m_iTrayWorkCnt);
                    MoveMotr(mi.LDR_Z, dLDRZTrayPos);

                    Step.iCycle++;
                    return false;

                case 17:
                    if (GetX(xi.LDR_TrayOver))
                    {
                        SM.MTR.Stop((int)mi.LDR_Z);
                        m_iTrayWorkCnt = 0;
                        SetErr(ei.PRT_TrayErr, "Tray is OverLoaded.");
                        Step.iCycle = 0;
                        return true;
                    }



                    if (!GetStop(mi.LDR_Z)) return false;
                    m_iTrayWorkCnt++;

                    Step.iCycle++;
                    return false;

                case 18:
                    //if (!MoveActr(ai.LDR_TrayFixClOp, fb.Fwd)) return false;

                    if (!GetX(xi.LDR_TrayPstn))
                    {
                        Step.iCycle = 0;
                        return true;
                    }

                    

                    MoveActr(ci.LDR_TrayFixClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;


                case 19:
                   
                    if (!GetActrStop(ci.LDR_TrayFixClOp, fb.Fwd)) return false;
                    MoveActr(ci.LDR_GripDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!GetActrStop(ci.LDR_GripDnUp, fb.Fwd)) return false;
                    MoveActr(ci.LDR_GripClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!GetActrStop(ci.LDR_GripClOp, fb.Fwd)) return false;
                    MoveActr(ci.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!GetActrStop(ci.LDR_GripDnUp, fb.Bwd)) return false;

                    //ARAY Data 입력.
                    DM.ARAY[(int)ri.PCK].ID = m_iTrayWorkCnt.ToString();
                    DM.ARAY[(int)ri.PCK].LotNo = LOT.m_sLotNo;
                    DM.ARAY[(int)ri.PCK].Step = 0;
                   

                    DM.ARAY[(int)ri.PCK].SetStat(cs.Unkwn);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePlace()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }
            double dXPos = 0.0;

            switch (Step.iCycle)
            {

                default: sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                         Step.iCycle = 0;
                         return true;

                case 0:
                         return false;


                case 10:

                    MoveActr(ci.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ci.LDR_GripDnUp, fb.Bwd)) return false;
                    MoveActr(ci.LDR_GripRtLt, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!GetActrStop(ci.LDR_GripRtLt, fb.Fwd)) return false;
                    MoveActr(ci.LDR_GripDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetActrStop(ci.LDR_GripDnUp, fb.Fwd)) return false;
                    MoveActr(ci.LDR_GripClOp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!GetActrStop(ci.LDR_GripClOp, fb.Bwd)) return false;
                    MoveActr(ci.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!GetActrStop(ci.LDR_GripDnUp, fb.Bwd)) return false;
                    if (!GetX(xi.IDX_Pri))
                    {
                        SetErr(ei.PRT_Missed, "Pre Index Tray Place Failed");
                        DM.ARAY[(int)ri.PCK].SetStat(cs.None);
                        Step.iCycle = 0;
                        return true;
                    }
                    
                    
                    DM.ARAY[(int)ri.PRI].SetStat(cs.Unkwn);

                    DM.ARAY[(int)ri.PRI].ID      = DM.ARAY[(int)ri.PCK].ID     ;
                    DM.ARAY[(int)ri.PRI].LotNo   = DM.ARAY[(int)ri.PCK].LotNo  ;
                    DM.ARAY[(int)ri.PRI].Step    = DM.ARAY[(int)ri.PCK].Step   ;

                    DM.ARAY[(int)ri.PCK].SetStat(cs.None);
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePckrRight()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }
            double dXPos = 0.0;

            switch (Step.iCycle)
            {

                default: sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;


                case 10:
                    MoveActr(ci.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ci.LDR_GripDnUp, fb.Bwd)) return false;
                    MoveActr(ci.LDR_GripRtLt    , fb.Fwd);
                    MoveActr(ci.LDR_TrayFixClOp , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!GetActrStop(ci.LDR_GripRtLt   , fb.Fwd)) return false;
                    if (!GetActrStop(ci.LDR_TrayFixClOp, fb.Bwd)) return false;
                    MoveMotr(mi.LDR_Z , pv.LDR_ZWait);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.LDR_Z))return false;
                    Step.iCycle = 0;
                    return true;
            }
        }
        
        public bool CheckSafe(mi _iMotr, double _dPos)
        {
            //if(!SM.MT.CmprPos((int)_iMotr, _dPos))return false;
            
            bool bRet = true;
            string sMsg = "";

            if (_iMotr == mi.LDR_Z)
            {
                //if (IO_GetX(xLDR_Detect1))
                //{
                //    sMsg = m_sPartName + " 레일 진입부 센서 감지 상태로 미들 블럭 돌출을 확인하세요";
                //    bRet = false;
                //}

                //if (MT_GetCmdPos(miLTL_XGenRr) < PM.GetValue(miLTL_XGenRr, pvLTL_XWait))
                //{
                //    sMsg = m_sPartName + "현재 레프트툴 X축 모터 위치가 Wait 위치보다 높습니다.";
                //    bRet = false;
                //}
            }
            else
            {
                sMsg = "Motor " + SM.MTR.GetName((int)_iMotr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(SM.MTR.GetName((int)_iMotr), sMsg);
                if (Step.eSeq == 0) Log.ShowMessage(SM.MTR.GetName((int)_iMotr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        //Lee.
        public bool CheckSafe(ci _iActr, EN_CYL_POS _bFwd)
        {
            if (SM.CYL.Complete((int)_iActr, _bFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_iActr == ci.LDR_GripClOp){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = AnsiString("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_iActr == ci.LDR_GripDnUp)
            {

            }
            else if (_iActr == ci.LDR_GripRtLt)
            {
                if(SM.CYL.GetCmd((int)ci.LDR_GripDnUp)==EN_CYL_POS.Fwd)
                {
                    sMsg = "Gripper up/dn Cylinder is Down"; 
                    bRet = false;
                }
            }
            else if (_iActr == ci.LDR_TrayFixClOp)
            {

            }
            else 
            {
                sMsg = "Cylinder " + SM.CYL.GetName((int)_iActr) + " is Not this parts.";
                bRet = false;
            }


            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(SM.CYL.GetName((int)_iActr), sMsg);
                if (Step.iCycle==0) Log.ShowMessage(SM.CYL.GetName((int)_iActr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool MoveMotr(mi _iMotr , double _dPos , bool _bSlow = false)
        {
            if(!CheckSafe(_iMotr, _dPos))return false ;

            if(_bSlow) {SM.MTR.GoAbsSlow((int)_iMotr, _dPos); return false;}
            else { 
                if(Step.iCycle != 0) { SM.MTR.GoAbsRun((int)_iMotr, _dPos); }
                else                 { SM.MTR.GoAbsMan((int)_iMotr, _dPos); }
            }

            return GetStop(_iMotr)  ;         
        }

        public bool MoveMotr(mi _iMotr , pv _iPosId , bool _bSlow=false)
        {
            return MoveMotr(_iMotr , GetMotrPos(_iMotr,_iPosId) , _bSlow);      
        }

        public void SetY(yi _YAdd , bool _bVal)
        {
            SM.DIO.SetY((int)_YAdd , _bVal);
        }

        public bool GetY(yi _YAdd)
        {
            return SM.DIO.GetY((int)_YAdd);
        }

        public bool GetX(xi _XAdd)
        {
            return SM.DIO.GetX((int)_XAdd);
        }
        public bool GetStop(mi _iMotr)
        {
            return SM.MTR.GetStopInpos((int)_iMotr);

        }
        public void SetErr(ei _eErrNo, string _sMsg = "")
        {
            //if (_sMsg == "") SM.ERR.SetErr((int)_eErrNo);
            SM.ERR.SetErr((int)_eErrNo, _sMsg);
        }
        public bool MoveActr(ci _iActr, fb _bFwd)
        {
            EN_CYL_POS m_bFwd = (EN_CYL_POS)_bFwd;
            if (!CheckSafe(_iActr, m_bFwd)) return false;

            SM.CYL.Move((int)_iActr, m_bFwd);
            
            return true;
        }
        public bool GetActrStop(ci _iActr, fb _bFwd)
        {
            if(SM.CYL.Complete((int)_iActr, (EN_CYL_POS)_bFwd)){
                return true;
            }
            return false;
        }

        public bool CheckStop()
        {
            if (!SM.MTR.GetStop ((int)mi.LDR_Z       ))return false;
            if (!SM.CYL.Complete((int)ci.LDR_GripClOp))return false;
            if (!SM.CYL.Complete((int)ci.LDR_GripDnUp))return false;
            if (!SM.CYL.Complete((int)ci.LDR_GripRtLt))return false;

            return true;
        }
    }
}




