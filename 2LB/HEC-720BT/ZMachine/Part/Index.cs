using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMMON;
using SML2;
using System.IO;



namespace Machine
{
    //SLD == Solder
    public class Index : Part
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
            Idle     = 0,
            Tension  = 1,
            Work     = 2, //가시매선 커팅
            Move     = 3,
            Out      = 4, //배출.
            //메뉴얼 삽입 버튼.
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
                iHome    = 0;
                iToStart = 0;
                eSeq     = sc.Idle;
                iCycle   = 0;
                iToStop  = 0;
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

        public int iCttrCnt;
        public int iNodeCnt;
        public int iWorkCnt;

        public struct TNodePos
        {

            public double WrkSttPos ; //작업할 가시 위치.
            public int    iDirection; //Left/Right Tool 선택
            public double dDegree   ; //실 돌려주는 위치 
            public int    iWrkCnt   ; //반복작업 갯수
            public double dWrkPitch ; //반복작업 피치.
        }
        TNodePos WorkPos;

        //소팅 인포=============================================================

        public Index()
        {
            m_sPartName = "Index ";

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

            iCttrCnt = OM.EqpStat.iCttrCnt;
            iNodeCnt = OM.EqpStat.iNodeCnt;
            iWorkCnt = OM.EqpStat.iWorkCnt;
            //InitCycleName();
            //InitCycleTime();                        
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


        //PartInterface 부분.
        //인터페이스 상속.====================================================================================================================
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
                ER_SetErr(ei.ETC_ToStartTO, m_sPartName); 
            }
        
            String sTemp = String.Format("Step.iToStart={0:00}", Step.iToStart);
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
                    if ((OM.CmnOptn.bUsedLine1 && IO_GetX(xi.IDX_Detect1)) || (OM.CmnOptn.bUsedLine2 && IO_GetX(xi.IDX_Detect2)) ||
                        (OM.CmnOptn.bUsedLine3 && IO_GetX(xi.IDX_Detect3)) || (OM.CmnOptn.bUsedLine4 && IO_GetX(xi.IDX_Detect4)) ||
                        (OM.CmnOptn.bUsedLine5 && IO_GetX(xi.IDX_Detect5)))
                    {
                        ER_SetErr(ei.PRT_Missed, "자재가 감지되지 않습니다.");
                    }

                    if (DM.ARAY[(int)ri.IDX].CheckAllStat(cs.Empty))
                    {
                        if (OM.CmnOptn.bUsedLine1 && !IO_GetX(xi.IDX_Detect1)) DM.ARAY[(int)ri.IDX].SetStat(0, 0, cs.Unkwn);
                        
                        if (OM.CmnOptn.bUsedLine2 && !IO_GetX(xi.IDX_Detect2)) DM.ARAY[(int)ri.IDX].SetStat(0, 1, cs.Unkwn);
                        if (OM.CmnOptn.bUsedLine3 && !IO_GetX(xi.IDX_Detect3)) DM.ARAY[(int)ri.IDX].SetStat(0, 2, cs.Unkwn);
                        if (OM.CmnOptn.bUsedLine4 && !IO_GetX(xi.IDX_Detect4)) DM.ARAY[(int)ri.IDX].SetStat(0, 3, cs.Unkwn);
                        if (OM.CmnOptn.bUsedLine5 && !IO_GetX(xi.IDX_Detect5)) DM.ARAY[(int)ri.IDX].SetStat(0, 4, cs.Unkwn);
                    }

                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Bwd);
                    MoveCyl(ci.IDX_CutterDnUp , fb.Bwd);
                    Step.iToStart++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.IDX_CutBaseUpDn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.IDX_CutterDnUp , fb.Bwd)) return false;
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
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 15000))
            {
                ER_SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);
            }
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
                    //MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTStopWait); //이거 주석처리 되있었는데 문제되면 걷어내야함. 진섭
                    Step.iToStop++;
                    return false;

                case 11:
                    //if (!MT_GetStopPos(mi.IDX_XOUT, pv.IDX_XOUTStopWait)) return false;
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

        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            //Check Cycle Time Out.
            String sTemp;
            sTemp = String.Format("%s Step.iCycle={0:00}", "Autorun", Step.iCycle);
            if (Step.eSeq != PreStep.eSeq)
            {
                Log.Trace(m_sPartName, sTemp);
            }


            PreStep.eSeq = Step.eSeq;

            string sCycle = m_sCycleName[(int)Step.eSeq] ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                bool bExistWork  = !IO_GetX(xi.IDX_Detect1) || !IO_GetX(xi.IDX_Detect2) || !IO_GetX(xi.IDX_Detect3) || !IO_GetX(xi.IDX_Detect4) || !IO_GetX(xi.IDX_Detect5);
                bool bExistEmpty =  IO_GetX(xi.IDX_Detect1) &&  IO_GetX(xi.IDX_Detect2) &&  IO_GetX(xi.IDX_Detect3) &&  IO_GetX(xi.IDX_Detect4) &&  IO_GetX(xi.IDX_Detect5);
                
                bool isCycleWork = bExistWork &&
                                   DM.ARAY[(int)ri.IDX].CheckAllStat(cs.Unkwn) || 
                                   DM.ARAY[(int)ri.IDX].GetCntStat(cs.Work ) == 0 &&
                                   DM.ARAY[(int)ri.IDX].GetCntStat(cs.None ) == 0 &&
                                   !DM.ARAY[(int)ri.IDX].CheckAllStat(cs.Empty) ;
                bool isCycleMove = DM.ARAY[ri.IDX].GetCntStat(cs.Move) != 0;
                bool isCycleOut  = bExistWork && DM.ARAY[(int)ri.IDX].GetCntStat(cs.Work) != 0; //|| 
                //bool isWorkEnd = !bExistWork && (DM.ARAY[(int)ri.IDX].CheckAllStat(cs.Empty) || DM.ARAY[(int)ri.IDX].CheckAllStat(cs.None));
                bool isWorkEnd = DM.ARAY[(int)ri.IDX].CheckAllStat(cs.None);
                //여기부터 조건 잡자.

                if ((isCycleWork || isCycleOut) && bExistEmpty)
                {
                    ER_SetErr(ei.PRT_Missed, "자재가 감지되지 않습니다.");
                }
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleOut    ) { Step.eSeq = sc.Out               ; }
                else if (isCycleMove   ) { Step.eSeq = sc.Move              ; }
                else if (isCycleWork   ) { Step.eSeq = sc.Work              ; } 
                else if (isWorkEnd     ) { Stat.bWorkEnd = true; return true; }

                Stat.bWorkEnd = false;

                if (Step.eSeq != sc.Idle)
                {
                    Log.Trace(m_sPartName, Step.eSeq.ToString() + " Start");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }

            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default           :                         Log.Trace(m_sPartName, "default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle    ):                                                                                                                          return false;
                case (sc.Work    ): if (CycleWork     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Move    ): if (CycleMove     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Out     ): if (CycleOut      ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                
            }
        }

        //인터페이스 상속 끝.==================================================
        public double LengthToDegree(double _dDegree)
        {
            double dRet = 0.0;
            //dRet = (OM.DevOptn.dWorkDiameter * 3.14) / _dDegree; 
            dRet = (OM.DevOptn.dWorkDiameter * 3.14) / (360/_dDegree);
            return dRet;
        }

        public int WorkCount(double _dPitch)
        {
            int iRet = 0;
            //dRet = (OM.DevOptn.dWorkDiameter * 3.14) / _dDegree; 
            iRet = (int)((OM.DevOptn.dWorkDist - (OM.DevOptn.dSttEmpty + OM.DevOptn.dEndEmpty)) / _dPitch);
            return iRet;
        }
        public enum EN_FINDCHIP : int
        {
            FrstRowCol     = 0 ,
            FrstColRow     = 1 ,
            LastRowCol     = 2 ,
            FrstRowLastCol = 3 ,
            LastRowFrstCol = 4 ,
            LastColFrstRow = 5 ,
            FrstColLastRow = 6 ,
            LastColRow     = 7 , 
            MAX_FINDCHIP      
        };

        public EN_FINDCHIP enFindChip;

        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail)
        { 
            r = 0;
            c = 0;
            DM.ARAY[_iId].FindFrstRowCol(_iChip, ref c, ref r);
            return (c >= 0 && r >= 0) ? true : false;
        }
        protected struct TWorkInfo {
            public int iCol ;
            public int iRow ;
            public cs eStat ;
        } ;//오토런에서 스테이지에서 정보를 가져다 담아 놓고 Cycle에서 이것을 쓴다....

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() &&!OM.MstOptn.bDebugMode, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
                //Step.iHome = 0 ;
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

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iHome);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iHome = 0 ;
                    return false ;

                case 10: 
                    CL_Move(ci.IDX_CutterDnUp , fb.Bwd);
                    CL_Move(ci.IDX_CutBaseUpDn, fb.Bwd);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!CL_Complete(ci.IDX_CutterDnUp , fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_CutBaseUpDn, fb.Bwd))return false;
                    CL_Move(ci.IDX_TwstLtDnUp, fb.Bwd);
                    CL_Move(ci.IDX_TwstRtDnUp, fb.Bwd);
                    CL_Move(ci.IDX_CutLtFwBw , fb.Bwd);
                    CL_Move(ci.IDX_CutRtFwBw , fb.Bwd);
                    CL_Move(ci.IDX_OutDnUp   , fb.Bwd);
                    Step.iHome++;
                    return false ;
            
                case 12:
                    if(!CL_Complete(ci.IDX_TwstLtDnUp, fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_TwstRtDnUp, fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_CutLtFwBw , fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_CutRtFwBw , fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_OutDnUp   , fb.Bwd))return false;
                    MT_GoHome(mi.IDX_TTRN);
                    MT_GoHome(mi.IDX_XCUT);
                    MT_GoHome(mi.IDX_XOUT);
                    Step.iHome++;
                    return false;

                case 13: 
                    if (!MT_GetHomeDone(mi.IDX_TTRN)) return false;
                    if (!MT_GetHomeDone(mi.IDX_XCUT)) return false;
                    if (!MT_GetHomeDone(mi.IDX_XOUT)) return false;
                    MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTStopWait);
                    Step.iHome++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.IDX_XOUT, pv.IDX_XOUTStopWait)) return false;
                    Step.iHome = 0;
                    return true ;
            }
        }

        int iSttCnt   = 0;
        int iShiftCnt = 0; //작업하는 길이가 100mm넘으면 옮겨서 하자.
        double dOutShiftPos = 0.0;
        public bool CycleWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }

            int r;
            int c;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Cycle Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    iNodeCnt = OM.EqpStat.iNodeCnt ;
                    iCttrCnt = OM.EqpStat.iCttrCnt ;

                    //iShiftCnt = 0 ;
                    iSttCnt   = 0 ;
                    dOutShiftPos = 0.0;

                    MoveCyl(ci.IDX_Hold1UpDn , fb.Fwd);
                    MoveCyl(ci.IDX_Hold2UpDn , fb.Fwd);
                    MoveCyl(ci.IDX_CutLtFwBw , fb.Bwd);
                    MoveCyl(ci.IDX_CutRtFwBw , fb.Bwd);
                    if (iCttrCnt == 0)
                    {
                        MoveCyl(ci.IDX_TwstLtDnUp, fb.Bwd);
                        MoveCyl(ci.IDX_TwstRtDnUp, fb.Bwd);
                    }
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.IDX_Hold1UpDn , fb.Fwd)) return false;
                    if (!CL_Complete(ci.IDX_Hold2UpDn , fb.Fwd)) return false;
                    if (!CL_Complete(ci.IDX_CutLtFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.IDX_CutRtFwBw , fb.Bwd)) return false;
                    if (iCttrCnt == 0)
                    {
                        if (!CL_Complete(ci.IDX_TwstLtDnUp, fb.Bwd)) return false;
                        if (!CL_Complete(ci.IDX_TwstRtDnUp, fb.Bwd)) return false;
                        MoveMotr(mi.IDX_TTRN, pv.IDX_TTRNWait);
                    }
                    if (iCttrCnt == 0 && iNodeCnt == 0)
                    {
                        MoveMotr(mi.IDX_XCUT, pv.IDX_XCUTWorkStt); 
                    }
                    
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStop(mi.IDX_TTRN)) return false;
                    if (!MT_GetStop(mi.IDX_XCUT)) return false;
                    MoveCyl(ci.IDX_TwstLtDnUp, fb.Fwd);
                    MoveCyl(ci.IDX_TwstRtDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!CL_Complete(ci.IDX_TwstLtDnUp, fb.Fwd)) return false;
                    if (!CL_Complete(ci.IDX_TwstRtDnUp, fb.Fwd)) return false;
                    if (OM.CmnOptn.bIgnrWork)
                    {
                        Step.iCycle = 21;
                        return false;
                    }
                    Step.iCycle++;
                    return false;

                //아래서 씀.
                case 14:
                    double dWorkPos = 0.0;

                    
                    if (OM.NodePos[iNodeCnt].iDirection == 0)
                    {
                        double dTemp = (OM.NodePos[iNodeCnt].dWrkSttPos + (OM.DevOptn.dCutLtRtDist / 2)) + (OM.NodePos[iNodeCnt].dWrkPitch * iCttrCnt);
                        dWorkPos = PM.GetValue(mi.IDX_XCUT, pv.IDX_XCUTWorkStt) - dTemp - OM.DevOptn.dSttEmpty;

                        double dCheckPos = PM.GetValue(mi.IDX_XCUT, pv.IDX_XCUTWorkStt) - (dWorkPos - (OM.DevOptn.dCutLtRtDist / 2));

                        if (dCheckPos - (iShiftCnt * OM.DevOptn.dShiftWorkPos) >= OM.MstOptn.dMAXWorkDist || dWorkPos + (iShiftCnt * OM.DevOptn.dShiftWorkPos) < 0)
                        {
                            if ((OM.CmnOptn.bUsedLine1 && IO_GetX(xi.IDX_Detect1)) || (OM.CmnOptn.bUsedLine2 && IO_GetX(xi.IDX_Detect2)) ||
                                (OM.CmnOptn.bUsedLine3 && IO_GetX(xi.IDX_Detect3)) || (OM.CmnOptn.bUsedLine4 && IO_GetX(xi.IDX_Detect4)) ||
                                (OM.CmnOptn.bUsedLine5 && IO_GetX(xi.IDX_Detect5)))
                            {
                                ER_SetErr(ei.PRT_Missed, "자재가 감지되지 않습니다.");
                                return true;
                            }
                            
                            if (!IO_GetX(xi.IDX_Detect1) && OM.CmnOptn.bUsedLine1) DM.ARAY[(int)ri.IDX].SetStat(0,0, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,0, cs.Empty);
                            if (!IO_GetX(xi.IDX_Detect2) && OM.CmnOptn.bUsedLine2) DM.ARAY[(int)ri.IDX].SetStat(0,1, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,1, cs.Empty);
                            if (!IO_GetX(xi.IDX_Detect3) && OM.CmnOptn.bUsedLine3) DM.ARAY[(int)ri.IDX].SetStat(0,2, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,2, cs.Empty);
                            if (!IO_GetX(xi.IDX_Detect4) && OM.CmnOptn.bUsedLine4) DM.ARAY[(int)ri.IDX].SetStat(0,3, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,3, cs.Empty);
                            if (!IO_GetX(xi.IDX_Detect5) && OM.CmnOptn.bUsedLine5) DM.ARAY[(int)ri.IDX].SetStat(0,4, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,4, cs.Empty);

                            Step.iCycle = 0;
                            return true;
                            //FindChip(ri.IDX, out c, out r, cs.Unkwn);
                            //DM.ARAY[ri.IDX].SetStat(c, r, cs.Move);
                            //Step.iCycle = 0;
                            //return true;
                        }
                        
                    }
                    else
                    {
                        double dTemp = (OM.NodePos[iNodeCnt].dWrkSttPos - (OM.DevOptn.dCutLtRtDist / 2)) + (OM.NodePos[iNodeCnt].dWrkPitch * iCttrCnt);
                        dWorkPos = PM.GetValue(mi.IDX_XCUT, pv.IDX_XCUTWorkStt) - dTemp - OM.DevOptn.dSttEmpty;

                        double dCheckPos = PM.GetValue(mi.IDX_XCUT, pv.IDX_XCUTWorkStt) - (dWorkPos + (OM.DevOptn.dCutLtRtDist / 2));

                        if (dCheckPos - (iShiftCnt * OM.DevOptn.dShiftWorkPos) >= OM.MstOptn.dMAXWorkDist || dWorkPos + dWorkPos + (iShiftCnt * OM.DevOptn.dShiftWorkPos) < 0)
                        {
                            if ((OM.CmnOptn.bUsedLine1 && IO_GetX(xi.IDX_Detect1)) || (OM.CmnOptn.bUsedLine2 && IO_GetX(xi.IDX_Detect2)) ||
                                (OM.CmnOptn.bUsedLine3 && IO_GetX(xi.IDX_Detect3)) || (OM.CmnOptn.bUsedLine4 && IO_GetX(xi.IDX_Detect4)) ||
                                (OM.CmnOptn.bUsedLine5 && IO_GetX(xi.IDX_Detect5)))
                            {
                                ER_SetErr(ei.PRT_Missed, "자재가 감지되지 않습니다.");
                                return true;
                            }
                            
                            if (!IO_GetX(xi.IDX_Detect1) && OM.CmnOptn.bUsedLine1) DM.ARAY[(int)ri.IDX].SetStat(0,0, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,0, cs.Empty);
                            if (!IO_GetX(xi.IDX_Detect2) && OM.CmnOptn.bUsedLine2) DM.ARAY[(int)ri.IDX].SetStat(0,1, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,1, cs.Empty);
                            if (!IO_GetX(xi.IDX_Detect3) && OM.CmnOptn.bUsedLine3) DM.ARAY[(int)ri.IDX].SetStat(0,2, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,2, cs.Empty);
                            if (!IO_GetX(xi.IDX_Detect4) && OM.CmnOptn.bUsedLine4) DM.ARAY[(int)ri.IDX].SetStat(0,3, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,3, cs.Empty);
                            if (!IO_GetX(xi.IDX_Detect5) && OM.CmnOptn.bUsedLine5) DM.ARAY[(int)ri.IDX].SetStat(0,4, cs.Move );
                            else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,4, cs.Empty);

                            Step.iCycle = 0;
                            return true;
                            //Step.iCycle = 100;
                            //return false;
                        }
                    }
                    
                    if (iShiftCnt != 0)
                    {
                        //dWorkPos += OM.DevOptn.dShiftWorkPos;
                            //SM.MT.GoIncRun((int)mi.IDX_XOUT, OM.DevOptn.dShiftWorkPos);

                        //Test
                        dWorkPos = dWorkPos + dOutShiftPos;
                        
                    }

                    if (MT_GetCmdPos(mi.IDX_XCUT) > SML.MT.Para[(int)mi.IDX_XCUT].dMaxPos)
                    {
                        ER_SetErr(ei.PRT_OverLoad, "Index Max Position Over");
                        return true;
                    }
                    else if (MT_GetCmdPos(mi.IDX_XCUT) < SML.MT.Para[(int)mi.IDX_XCUT].dMinPos)
                    {
                        ER_SetErr(ei.PRT_OverLoad, "Index Min Position Over");
                        return true;
                    }

                    if (iShiftCnt == 0) MT_GoAbsRun(mi.IDX_XCUT, dWorkPos);
                    else               MT_GoAbsRun(mi.IDX_XCUT, dWorkPos+(iShiftCnt * OM.DevOptn.dShiftOfs));
                    Log.Trace("Move Pos " + iNodeCnt.ToString(), MT_GetCmdPos((int)mi.IDX_XCUT).ToString());
                    //MT_GoAbsRun(mi.IDX_TTRN, PM.GetValue(mi.IDX_TTRN, pv.IDX_TTRNWait) + OM.NodePos[iNodeCnt].dDegree);
                    MT_GoAbsRun(mi.IDX_TTRN, PM.GetValue(mi.IDX_TTRN, pv.IDX_TTRNWait) + LengthToDegree(OM.NodePos[iNodeCnt].dDegree));
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MT_GetStop(mi.IDX_XCUT)) return false;
                    if (!MT_GetStop(mi.IDX_TTRN)) return false;
                    if (OM.NodePos[iNodeCnt].iDirection == 0) MoveCyl(ci.IDX_CutLtFwBw, fb.Fwd);
                    else                                      MoveCyl(ci.IDX_CutRtFwBw, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (OM.NodePos[iNodeCnt].iDirection == 0 && !CL_Complete(ci.IDX_CutLtFwBw, fb.Fwd)) return false;
                    if (OM.NodePos[iNodeCnt].iDirection == 1 && !CL_Complete(ci.IDX_CutRtFwBw, fb.Fwd)) return false;
                    if (OM.NodePos[iNodeCnt].iDirection == 0) MT_GoIncRun((int)mi.IDX_XCUT, -OM.DevOptn.dLeftCutMovePitch );
                    else                                      MT_GoIncRun((int)mi.IDX_XCUT,  OM.DevOptn.dRightCutMovePitch);
                    Log.Trace("Move Pos " + iNodeCnt.ToString(), MT_GetCmdPos((int)mi.IDX_XCUT).ToString());
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iWorkDelay)) return false;
                    if (!MT_GetStop(mi.IDX_XCUT)) return false;
                    Step.iCycle++;
                    return false;  
                                   
                case 18:
                    if (OM.NodePos[iNodeCnt].iDirection == 0) MT_GoIncRun((int)mi.IDX_XCUT,  OM.DevOptn.dCutBwdOfs);
                    else                                      MT_GoIncRun((int)mi.IDX_XCUT, -OM.DevOptn.dCutBwdOfs);
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!MT_GetStop(mi.IDX_XCUT)) return false;
                    MoveCyl(ci.IDX_CutLtFwBw, fb.Bwd);
                    MoveCyl(ci.IDX_CutRtFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!CL_Complete(ci.IDX_CutLtFwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.IDX_CutRtFwBw, fb.Bwd)) return false;
                    if (ER_IsErr())
                    {
                        return true;
                    }
                    if (OM.NodePos[iNodeCnt].iWrkCnt - 1 <= iCttrCnt)
                    {
                        iCttrCnt = 0;
                        iNodeCnt++;
                        
                        OM.EqpStat.iNodeCnt = iNodeCnt;
                        OM.EqpStat.iCttrCnt = iCttrCnt;

                    }
                    else
                    {
                        iCttrCnt++;
                        OM.EqpStat.iCttrCnt = iCttrCnt;
                    }

                    if ((OM.NodePos[iNodeCnt].dWrkSttPos == 0 && OM.NodePos[iNodeCnt].dDegree == 0 &&
                         OM.NodePos[iNodeCnt].iWrkCnt == 0 && OM.NodePos[iNodeCnt].dWrkPitch == 0) ||
                         OM.MAX_NODE_POS < iNodeCnt)
                    {
                        Step.iCycle = 21;
                        return false;
                    }
                    //if ((OM.NodePos[iNodeCnt].dWrkSttPos == 0 && OM.NodePos[iNodeCnt].dDegree == 0 &&
                    //     OM.NodePos[iNodeCnt].iWrkCnt == 0 && OM.NodePos[iNodeCnt].dWrkPitch == 0) ||
                    //     OM.MAX_NODE_POS < iNodeCnt)
                    //{
                    //    Step.iCycle = 21;
                    //    return false;
                    //}

                    //OM.EqpStat.iNodeCnt = iNodeCnt ;
                    //OM.EqpStat.iCttrCnt = iCttrCnt ;
                    
                    Step.iCycle = 0;
                    return true;

                case 21:
                    if ((OM.CmnOptn.bUsedLine1 && IO_GetX(xi.IDX_Detect1)) || (OM.CmnOptn.bUsedLine2 && IO_GetX(xi.IDX_Detect2)) ||
                        (OM.CmnOptn.bUsedLine3 && IO_GetX(xi.IDX_Detect3)) || (OM.CmnOptn.bUsedLine4 && IO_GetX(xi.IDX_Detect4)) ||
                        (OM.CmnOptn.bUsedLine5 && IO_GetX(xi.IDX_Detect5)))
                    {
                        ER_SetErr(ei.PRT_Missed, "자재가 감지되지 않습니다.");
                        return true;
                    }

                    if (!IO_GetX(xi.IDX_Detect1) && OM.CmnOptn.bUsedLine1) DM.ARAY[(int)ri.IDX].SetStat(0,0, cs.Work );
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,0, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect2) && OM.CmnOptn.bUsedLine2) DM.ARAY[(int)ri.IDX].SetStat(0,1, cs.Work );
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,1, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect3) && OM.CmnOptn.bUsedLine3) DM.ARAY[(int)ri.IDX].SetStat(0,2, cs.Work );
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,2, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect4) && OM.CmnOptn.bUsedLine4) DM.ARAY[(int)ri.IDX].SetStat(0,3, cs.Work );
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,3, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect5) && OM.CmnOptn.bUsedLine5) DM.ARAY[(int)ri.IDX].SetStat(0,4, cs.Work );
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,4, cs.Empty);

                    OM.EqpStat.iNodeCnt  = 0 ;
                    OM.EqpStat.iCttrCnt  = 0 ;

                    Step.iCycle = 0;
                    return true;

            }
        }

        //자재 길이가 100mm 이상 넘어갈때 실 옮겨서 작업하는 사이클
        public bool CycleMove()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }

            int r = 0, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Cycle Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                
                //길이가 Option값을 넘어갔다.
                case 10:
                    MoveCyl(ci.IDX_Hold1UpDn  , fb.Fwd);
                    MoveCyl(ci.IDX_Hold2UpDn  , fb.Fwd);
                    MoveCyl(ci.IDX_TwstLtDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutterDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Bwd);
                    if (iShiftCnt == 0)
                    {
                        MoveCyl(ci.IDX_OutDnUp, fb.Bwd);
                    }
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.IDX_Hold1UpDn  , fb.Fwd)) return false;
                    if (!CL_Complete(ci.IDX_Hold2UpDn  , fb.Fwd)) return false;
                    if (!CL_Complete(ci.IDX_TwstLtDnUp , fb.Bwd)) return false;
                    if (iShiftCnt == 0 && !CL_Complete(ci.IDX_OutDnUp, fb.Bwd)) return false;
                    if (!CL_Complete(ci.IDX_CutterDnUp , fb.Bwd)) return false;
                    if (!CL_Complete(ci.IDX_CutBaseUpDn, fb.Bwd)) return false;

                    if (iShiftCnt == 0)
                    {
                        MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTClamp);
                    }
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.IDX_XOUT, pv.IDX_XOUTClamp)) return false;
                    MoveCyl(ci.IDX_OutDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!CL_Complete(ci.IDX_OutDnUp, fb.Fwd)) return false;
                    MoveCyl(ci.IDX_Hold1UpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.IDX_Hold1UpDn, fb.Bwd)) return false;
                    MoveCyl(ci.IDX_Hold2UpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.IDX_Hold2UpDn, fb.Bwd)) return false;
                    iShiftCnt++;
                    if (OM.DevOptn.dWorkDist > (MT_GetCmdPos(mi.IDX_XOUT) + OM.DevOptn.dShiftWorkPos) + PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTTensnOfs))
                    {
                        dOutShiftPos = (OM.DevOptn.dShiftWorkPos * iShiftCnt) + PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTTensnOfs);
                        MT_GoIncRun(mi.IDX_XOUT, OM.DevOptn.dShiftWorkPos);
                    }
                    else
                    {
                        dOutShiftPos = OM.DevOptn.dWorkDist;
                        MT_GoIncRun(mi.IDX_XOUT, OM.DevOptn.dWorkDist - (MT_GetCmdPos(mi.IDX_XOUT)) - PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTTensnOfs));
                    }
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_Hold1UpDn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 17:
                    if (!CL_Complete(ci.IDX_Hold1UpDn, fb.Fwd)) return false;
                    MT_GoIncRun(mi.IDX_XOUT, PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTTensnOfs));

                    Step.iCycle++;
                    return false;

                case 18:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_Hold2UpDn , fb.Fwd);
                    MoveCyl(ci.IDX_TwstLtDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!CL_Complete(ci.IDX_Hold2UpDn , fb.Fwd)) return false;
                    if (!CL_Complete(ci.IDX_TwstLtDnUp, fb.Fwd)) return false;

                    if ((OM.CmnOptn.bUsedLine1 && IO_GetX(xi.IDX_Detect1)) || (OM.CmnOptn.bUsedLine2 && IO_GetX(xi.IDX_Detect2)) ||
                        (OM.CmnOptn.bUsedLine3 && IO_GetX(xi.IDX_Detect3)) || (OM.CmnOptn.bUsedLine4 && IO_GetX(xi.IDX_Detect4)) ||
                        (OM.CmnOptn.bUsedLine5 && IO_GetX(xi.IDX_Detect5)))
                    {
                        ER_SetErr(ei.PRT_Missed, "자재가 감지되지 않습니다.");
                        return true;
                    }

                    if (!IO_GetX(xi.IDX_Detect1) && OM.CmnOptn.bUsedLine1) DM.ARAY[(int)ri.IDX].SetStat(0,0, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,0, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect2) && OM.CmnOptn.bUsedLine2) DM.ARAY[(int)ri.IDX].SetStat(0,1, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,1, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect3) && OM.CmnOptn.bUsedLine3) DM.ARAY[(int)ri.IDX].SetStat(0,2, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,2, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect4) && OM.CmnOptn.bUsedLine4) DM.ARAY[(int)ri.IDX].SetStat(0,3, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,3, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect5) && OM.CmnOptn.bUsedLine5) DM.ARAY[(int)ri.IDX].SetStat(0,4, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0,4, cs.Empty);


                    Step.iCycle = 0;
                    return true;

            }
        }
        
        public bool CycleOut    ()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Cycle Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 10:
                    iWorkCnt = OM.EqpStat.iWorkCnt;
                    if(dOutShiftPos == OM.DevOptn.dWorkDist)
                    {
                        Step.iCycle = 100;
                        return false;
                    }
                    
                    Step.iCycle++;
                    return false;

                //아래서 씀.
                case 11:
                    //MoveActr(ai.IDX_ShiftUpDn, fb.Fwd);
                    
                    Step.iCycle++;
                    return false;

                case 12:
                    MoveCyl(ci.IDX_Hold1UpDn  , fb.Fwd);
                    MoveCyl(ci.IDX_Hold2UpDn  , fb.Fwd);
                    MoveCyl(ci.IDX_TwstLtDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_TwstRtDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutterDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Bwd);
                    if (iShiftCnt == 0) MoveCyl(ci.IDX_OutDnUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.IDX_Hold1UpDn  , fb.Fwd))return false;
                    if(!CL_Complete(ci.IDX_Hold2UpDn  , fb.Fwd))return false;
                    if(!CL_Complete(ci.IDX_TwstLtDnUp , fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_TwstRtDnUp , fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_CutterDnUp , fb.Bwd)) return false;
                    if(!CL_Complete(ci.IDX_CutBaseUpDn, fb.Bwd)) return false;
                    if (iShiftCnt == 0 && !CL_Complete(ci.IDX_OutDnUp, fb.Bwd)) return false;

                    if (iShiftCnt == 0) MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTClamp);
                    //MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTClamp);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_OutDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!CL_Complete(ci.IDX_OutDnUp, fb.Fwd))return false;
                    MoveCyl(ci.IDX_Hold1UpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!CL_Complete(ci.IDX_Hold1UpDn, fb.Bwd)) return false;
                    MoveCyl(ci.IDX_Hold2UpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!CL_Complete(ci.IDX_Hold2UpDn, fb.Bwd)) return false;
                    if (OM.CmnOptn.bIgnrWork) MT_GoAbsRun(mi.IDX_XOUT, OM.DevOptn.dWorkDist);
                    else                      MT_GoAbsRun(mi.IDX_XOUT, OM.DevOptn.dWorkDist - PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTTensnOfs));
                    Step.iCycle++;
                    return false;

                
                case 18:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_Hold1UpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!CL_Complete(ci.IDX_Hold1UpDn, fb.Fwd)) return false;
                    if (!OM.CmnOptn.bIgnrWork) MT_GoIncRun(mi.IDX_XOUT, PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTTensnOfs));
                    Step.iCycle++;
                    return false;
                //아래서 씀.
                case 20:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_Hold2UpDn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 21:
                    if (!CL_Complete(ci.IDX_Hold2UpDn, fb.Fwd)) return false;
                    MT_GoIncRun(mi.IDX_XOUT, -PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTRvrsOfs));

                    Step.iCycle++;
                    return false;

                case 22:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Fwd) ;

                    Step.iCycle++;
                    return false;

                case 23:
                    if (!CL_Complete(ci.IDX_CutBaseUpDn, fb.Fwd)) return false;
                    MoveCyl(ci.IDX_CutterDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 24:
                    if (!CL_Complete(ci.IDX_CutterDnUp, fb.Fwd)) return false;
                    MoveCyl(ci.IDX_CutterDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 25:
                    if (!CL_Complete(ci.IDX_CutterDnUp , fb.Bwd)) return false;
                    if (!CL_Complete(ci.IDX_CutBaseUpDn, fb.Bwd)) return false;
                    MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTBin);
                    Step.iCycle++;
                    return false;

                case 26:
                    if (!MT_GetStopPos(mi.IDX_XOUT, pv.IDX_XOUTBin)) return false;
                    MoveCyl(ci.IDX_OutDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 27: 
                    if(!CL_Complete(ci.IDX_OutDnUp , fb.Bwd))return false;
                    MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTWait);
                    iWorkCnt++;
                    OM.EqpStat.iWorkCnt = iWorkCnt;
                    iShiftCnt = 0;
                    if (iWorkCnt == OM.DevOptn.iTargetCnt)
                    {
                        DM.ARAY[(int)ri.IDX].SetStat(cs.None);
                        Step.iCycle = 0;
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 28:
                    if ((OM.CmnOptn.bUsedLine1 && IO_GetX(xi.IDX_Detect1)) || (OM.CmnOptn.bUsedLine2 && IO_GetX(xi.IDX_Detect2)) ||
                        (OM.CmnOptn.bUsedLine3 && IO_GetX(xi.IDX_Detect3)) || (OM.CmnOptn.bUsedLine4 && IO_GetX(xi.IDX_Detect4)) ||
                        (OM.CmnOptn.bUsedLine5 && IO_GetX(xi.IDX_Detect5)))
                    {
                        ER_SetErr(ei.PRT_Missed, "자재가 감지되지 않습니다.");
                        return true;
                    }
                
                    if (!IO_GetX(xi.IDX_Detect1) && OM.CmnOptn.bUsedLine1) DM.ARAY[(int)ri.IDX].SetStat(0, 0, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0, 0, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect2) && OM.CmnOptn.bUsedLine2) DM.ARAY[(int)ri.IDX].SetStat(0, 1, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0, 1, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect3) && OM.CmnOptn.bUsedLine3) DM.ARAY[(int)ri.IDX].SetStat(0, 2, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0, 2, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect4) && OM.CmnOptn.bUsedLine4) DM.ARAY[(int)ri.IDX].SetStat(0, 3, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0, 3, cs.Empty);
                    if (!IO_GetX(xi.IDX_Detect5) && OM.CmnOptn.bUsedLine5) DM.ARAY[(int)ri.IDX].SetStat(0, 4, cs.Unkwn);
                    else                                                   DM.ARAY[(int)ri.IDX].SetStat(0, 4, cs.Empty);
                
                    Step.iCycle = 0;
                    return true;
                    

                case 100:
                    MoveCyl(ci.IDX_Hold1UpDn  , fb.Fwd);
                    MoveCyl(ci.IDX_Hold2UpDn  , fb.Fwd);
                    MoveCyl(ci.IDX_TwstLtDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_TwstRtDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutterDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 101:
                    if(!CL_Complete(ci.IDX_Hold1UpDn  , fb.Fwd))return false;
                    if(!CL_Complete(ci.IDX_Hold2UpDn  , fb.Fwd))return false;
                    if(!CL_Complete(ci.IDX_TwstLtDnUp , fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_TwstRtDnUp , fb.Bwd))return false;
                    if(!CL_Complete(ci.IDX_CutterDnUp , fb.Bwd)) return false;
                    if(!CL_Complete(ci.IDX_CutBaseUpDn, fb.Bwd)) return false;
                    MoveCyl(ci.IDX_OutDnUp , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 102:
                    if(!CL_Complete(ci.IDX_OutDnUp, fb.Fwd))return false;
                    Step.iCycle++;
                    return false;

                case 103:
                    Step.iCycle = 20;
                    return false;
            }
        }

        //시작준비하는 매뉴얼사이클
        public bool CycleManSttWait()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;


            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Cycle Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:
                    return false ;

                case 10:
                    iShiftCnt = 0;
                    OM.EqpStat.iNodeCnt = 0;
                    OM.EqpStat.iCttrCnt = 0;
                    //iWorkCnt  = 0;
                    Step.iCycle++;
                    return false;

                case 11:
                    MoveCyl(ci.IDX_Hold1UpDn  , fb.Bwd);
                    MoveCyl(ci.IDX_Hold2UpDn  , fb.Fwd);
                    MoveCyl(ci.IDX_CutLtFwBw  , fb.Bwd);
                    MoveCyl(ci.IDX_CutRtFwBw  , fb.Bwd);
                    MoveCyl(ci.IDX_TwstLtDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_TwstRtDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutterDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.IDX_Hold1UpDn  , fb.Bwd)) return false;
                    if(!CL_Complete(ci.IDX_Hold2UpDn  , fb.Fwd)) return false;
                    if(!CL_Complete(ci.IDX_CutLtFwBw  , fb.Bwd)) return false;
                    if(!CL_Complete(ci.IDX_CutRtFwBw  , fb.Bwd)) return false;
                    if(!CL_Complete(ci.IDX_TwstLtDnUp , fb.Bwd)) return false;
                    if(!CL_Complete(ci.IDX_TwstRtDnUp , fb.Bwd)) return false;
                    if(!CL_Complete(ci.IDX_CutterDnUp , fb.Bwd)) return false;
                    if(!CL_Complete(ci.IDX_CutBaseUpDn, fb.Bwd)) return false;
                    
                    Step.iCycle++;
                    return false;

                case 13:
                    MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTClamp); 
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.IDX_XOUT, pv.IDX_XOUTClamp)) return false;
                    MoveCyl(ci.IDX_OutDnUp, fb.Fwd);
                    
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.IDX_OutDnUp, fb.Fwd)) return false;
                    MoveCyl(ci.IDX_Hold2UpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 16:
                    if (!CL_Complete(ci.IDX_Hold2UpDn, fb.Bwd)) return false;
                    MT_GoIncRun(mi.IDX_XOUT, OM.DevOptn.dWorkDist);
                    
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_Hold1UpDn, fb.Fwd);
                    
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!CL_Complete(ci.IDX_Hold1UpDn, fb.Fwd)) return false;
                    MT_GoIncRun(mi.IDX_XOUT, PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTTensnOfs));
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_Hold2UpDn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 20:
                    if (!CL_Complete(ci.IDX_Hold2UpDn, fb.Fwd)) return false;
                    MT_GoIncRun(mi.IDX_XOUT, -PM.GetValue(mi.IDX_XOUT, pv.IDX_XOUTRvrsOfs));

                    Step.iCycle++;
                    return false;

                case 21:
                    if (!MT_GetStop(mi.IDX_XOUT)) return false;
                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!CL_Complete(ci.IDX_CutBaseUpDn, fb.Fwd)) return false;
                    MoveCyl(ci.IDX_CutterDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 23:
                    if (!CL_Complete(ci.IDX_CutterDnUp, fb.Fwd)) return false;
                    MoveCyl(ci.IDX_CutterDnUp , fb.Bwd);
                    MoveCyl(ci.IDX_CutBaseUpDn, fb.Bwd);
                    Step.iCycle ++;
                    return false;

                case 24:
                    if (!CL_Complete(ci.IDX_CutterDnUp , fb.Bwd)) return false;
                    if (!CL_Complete(ci.IDX_CutBaseUpDn, fb.Bwd)) return false;
                    MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTBin);
                    Step.iCycle++;
                    return false;

                case 25:
                    if (!MT_GetStopPos(mi.IDX_XOUT, pv.IDX_XOUTBin)) return false;
                    MoveCyl(ci.IDX_OutDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 26:
                    if (!CL_Complete(ci.IDX_OutDnUp, fb.Bwd)) return false;
                    MoveMotr(mi.IDX_XOUT, pv.IDX_XOUTWait);
                    Step.iCycle++;
                    return false;

                case 27:
                    if (!MT_GetStopPos(mi.IDX_XOUT, pv.IDX_XOUTWait)) return false;
                    Step.iCycle = 0;
                    return true;

            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;
            
            String sMsg = "";
            bool bRet = true;
            

            if (_eActr == ci.IDX_Hold1UpDn)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = AnsiString("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.IDX_CutLtFwBw)
            {
            
            }
            else if (_eActr == ci.IDX_CutRtFwBw)
            {

            }
            else if (_eActr == ci.IDX_TwstLtDnUp)
            {

            }
            else if (_eActr == ci.IDX_TwstRtDnUp)
            {

            }
            else if (_eActr == ci.IDX_Hold2UpDn)
            {

            }
            else if (_eActr == ci.IDX_CutBaseUpDn)
            {

            }
            else if (_eActr == ci.IDX_OutDnUp)
            {

            }
            else if (_eActr == ci.IDX_CutterDnUp)
            {

            }
            else
            {
                sMsg = "Cylinder " + CL_GetName(_eActr) + " is Not this parts.";
                bRet = false;
            }
            
            
            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(CL_GetName(_eActr), sMsg);
                if (Step.iCycle == 0) Log.ShowMessage(CL_GetName(_eActr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }
            
            return bRet;
        }

        public bool CheckSafe(mi _eMotr, pv _ePstn, double _dOfsPos = 0)
        {
            double dDstPos = PM_GetValue(_eMotr, _ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";
            
            if (_eMotr == mi.IDX_XCUT)
            {
                //if (SM.IO.GetX((int)xi.IDX_CutBaseUp) || SM.IO.GetX((int)xi.IDX_CutterDn))
                //{
                //    sMsg = m_sPartName + " 커터 파트가 닫혀있습니다. 커터 파트를 확인해주세요.";
                //    bRet = false;
                //}
            }

            else if (_eMotr == mi.IDX_XOUT)
            {
                if (IO_GetX(xi.IDX_CutBaseUp) || IO_GetX(xi.IDX_CutterDn))
                {
                    sMsg = m_sPartName + " 커터 파트가 닫혀있습니다. 커터 파트를 확인해주세요.";
                    bRet = false;
                }
            }

            else if (_eMotr == mi.IDX_TTRN)
            {
                //if (SM.IO.GetX((int)xi.IDX_CutBaseUp) || SM.IO.GetX((int)xi.IDX_CutterDn))
                //{
                //    sMsg = m_sPartName + " 커터 파트가 닫혀있습니다. 커터 파트를 확인해주세요.";
                //    bRet = false;
                //}
            }
            else
            {
                sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
                bRet = false;
            }
            
            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(MT_GetName(_eMotr), sMsg);
                //메뉴얼 동작일때.
                if (Step.eSeq == 0) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }
            
            return bRet;
        }
        

        public bool MoveMotrSlow(mi _eMotr, pv _ePstn, double _dOfsPos = 0)
        {
            if (!CheckSafe(_eMotr, _ePstn, _dOfsPos)) return false;

            double dDstPos = PM_GetValue(_eMotr, _ePstn) + _dOfsPos;
            MT_GoAbsSlow(_eMotr, dDstPos);
            return true;

        }


        //무브함수들의 리턴이 Done을 의미 한게 아니고 명령 전달이 됐는지 여부로 바꿈.
        //Done 확인을 위해서는 GetStop을 써야함.
        public bool MoveMotr(mi _eMotr, pv _ePstn, double _dOfsPos = 0)
        {
            if (!CheckSafe(_eMotr, _ePstn, _dOfsPos)) return false;

            double dDstPos = PM_GetValue(_eMotr, _ePstn) + _dOfsPos;
            if (Step.iCycle != 0) MT_GoAbsRun(_eMotr, dDstPos);
            else MT_GoAbsMan(_eMotr, dDstPos);
            return true;       
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
            if (!MT_GetStop(mi.IDX_XCUT)) return false;
            if (!MT_GetStop(mi.IDX_XOUT)) return false;
            if (!MT_GetStop(mi.IDX_TTRN)) return false;

            return true;
        }       

        public double WorkCenterCal()
        {
            double dWorkPos     = 100;                     //구할 거리.
            double dTotalDist   = OM.MstOptn.dCutIdxDist;  //커터에서 실린더 까지의 길이. 
            double dSttPos      = 0.0;                     //처음 작업 시작 위치.
            double dPreWorkPos  = 0.0;                     //검색 길이. 
            double dWorkDist    = OM.DevOptn.dWorkDist  ;  //순수 작업 길이.
            int iWorkDistCnt = (int)dTotalDist / (int)dWorkDist;  //전체 길이에서 몇개 작업 가능 한지.

            for (int i = 0; i < iWorkDistCnt; i++)
            {
                dSttPos = OM.DevOptn.dWorkDist * i;
                //dPreWorkPos = Math.Abs((OM.MstOptn.dCutIdxDist - dSttPos) - OM.MstOptn.dHomeTwstCntr - (OM.MstOptn.dTwstCntr - OM.MstOptn.dHomeTwstCntr));
                dPreWorkPos = (dTotalDist - OM.MstOptn.dTwstCntr) - (dSttPos + (dWorkDist / 2));
                if (dTotalDist > dSttPos + OM.DevOptn.dWorkDist + OM.MstOptn.dTwstCntr - OM.MstOptn.dHomeTwstCntr && Math.Abs(dPreWorkPos) < Math.Abs(dWorkPos))
                {
                    dWorkPos = dPreWorkPos;
                }

            }
            return OM.MstOptn.dHomeTwstCntr - dWorkPos + OM.MstOptn.dTnsnOfs;
        } 
    };
}
