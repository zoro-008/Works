using System;
using COMMON;
using SML;



namespace Machine
{
    //IDX == Index
    public class Index : PartInterface
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
            Idle    = 0,
            ToMark  = 1,
            Mark    = 2,
            ToBar   = 3,
            Barcode = 4,
            Out     = 5,
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

        protected double m_dLastIdxPos;
        protected String m_sCheckSafeMsg;

        public string[] m_sCycleName;
        public CTimer[] m_CycleTime;

        public Index()
        {
            m_sPartName = "Index";

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
                    Step.iToStop++;
                    return false;
                
                case 11: 
                    Step.iToStop++;
                    return false;

                case 12: 
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
        public string GetSeqName() { return Step.eSeq.ToString(); }

        public string GetCrntCycleName     () { return Step.eSeq.ToString(); }
        public String GetCycleName(int _iSeq) { return m_sCycleName[_iSeq]; }
        public double GetCycleTime(int _iSeq) { return m_CycleTime [_iSeq].Duration; }
        public String GetPartName (         ) { return m_sPartName        ; }

        public int GetCycleMaxCnt() { return (int)sc.MAX_SEQ_CYCLE; }       

        public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            //Check Cycle Time Out.
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName();

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;
                                  
                bool isCycleToMark  = DM.ARAY[(int)ri.MRK].CheckAllStat(cs.None)  &&  DM.ARAY[(int)ri.PRI].CheckAllStat(cs.Unkwn)  && DM.ARAY[(int)ri.BAR].CheckAllStat(cs.None);
                bool isCycleMark    = DM.ARAY[(int)ri.MRK].CheckAllStat(cs.Unkwn) ;
                bool isCycleToBar   = DM.ARAY[(int)ri.BAR].CheckAllStat(cs.None)  && DM.ARAY[(int)ri.MRK].CheckAllStat(cs.Mark);
                bool isCycleBarcode = DM.ARAY[(int)ri.BAR].CheckAllStat(cs.Mark)  ; 
                bool isCycleOut     = DM.ARAY[(int)ri.BAR].CheckAllStat(cs.Good)  ; // ;
                bool isCycleEnd     = DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None)  && 
                                      DM.ARAY[(int)ri.MRK].CheckAllStat(cs.None)  &&
                                      DM.ARAY[(int)ri.BAR].CheckAllStat(cs.None)  ;


                //에러 추가.
                //if()
                if (SEQ.LDR.GetSeqStep() == (int)sc.Idle && DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None) && GetX(xi.IDX_Pri))
                {
                    SetErr(ei.PKG_Unknwn, "Unknown Tray Detected in PreIndex Zone");
                }
                if (!DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None) && !GetX(xi.IDX_Pri))
                {
                    SetErr(ei.PKG_Dispr , "Tray Needed in PreIndex Zone");
                }

                if (DM.ARAY[(int)ri.MRK].CheckAllStat(cs.None) && GetX(xi.IDX_PriOut))
                {
                    SetErr(ei.PKG_Unknwn, "Unknown Tray Detected in PreIndex Zone");
                }
                if (!DM.ARAY[(int)ri.MRK].CheckAllStat(cs.None) && !GetX(xi.IDX_PriOut))
                {
                    SetErr(ei.PKG_Dispr, "Tray Needed in PreIndex Zone");
                }


                if (SM.ERR.IsErr())
                {
                    MoveActr(ci.LDR_TrayFixClOp, fb.Bwd);
                    return false;
                } 

                //Normal Decide Step.
                     if (isCycleToMark    ) { Step.eSeq = sc.ToMark ; }
                else if (isCycleMark      ) { Step.eSeq = sc.Mark   ; }
                else if (isCycleToBar     ) { Step.eSeq = sc.ToBar  ; }
                else if (isCycleBarcode   ) { Step.eSeq = sc.Barcode; }
                else if (isCycleOut       ) { Step.eSeq = sc.Out    ; }
                else if (isCycleEnd       ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle)
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
                default          :                         Log.Trace(m_sPartName, "default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle   ):                                                                                                                          return false;
                case (sc.ToMark ): if (CycleToMark   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Mark   ): if (CycleMark     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.ToBar  ): if (CycleToBar    ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Barcode): if (CycleBarcode  ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Out    ): if (CycleOut      ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                
            }
        }

        //인터페이스 상속 끝.==================================================

        protected double GetLastCmd(mi _iMotr)
        {
            double dLastIdxPos = 0.0;

            //sunsun
                //if (!SM.MTR.GetAlarmSgnl((int)_iMotr) && !SM.MTR.GetPLimSnsr((int)_iMotr)) dLastIdxPos = SM.MTR.GetCmdPos((int)_iMotr);
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
        //protected CArray[] Aray;
        
        public bool FindChip(ref int c, ref int r, cs _iChip, int _iId) //이거 되는지 확인 해야함
        {
            r = 0;
            c = DM.ARAY[_iId].FindLastCol(_iChip); 
            
            return (c >= 0 && r >= 0) ? false : true;
        }

        public double GetMotrPos (mi _iMotr , pv _iPstnValue )
        {
            return PM.GetValue((uint)_iMotr , (uint)_iPstnValue);
        }
        
        public void InitCycleName()
        {
            m_sCycleName = new String[(int)sc.MAX_SEQ_CYCLE];
            m_sCycleName[(int)sc.Idle     ]="Idle"       ;
            m_sCycleName[(int)sc.ToMark   ]="Supply"     ;
            m_sCycleName[(int)sc.Mark     ]="Mark"       ;
            m_sCycleName[(int)sc.Out      ]="Out"        ;
        }

        public void InitCycleTime()
        {
            
        }
        
        
        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                SetErr(ei.ETC_AllHomeTO ,sTemp);
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
                         //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                         Step.iHome = 0 ;
                         return true ;
            
                case 10: 
                    SM.CYL.Move((int)ci.IDX_IdxUpDn, EN_CYL_POS.Bwd);
                    Step.iHome++;
                    return false ;
            
                case 11:
                    if(!SM.CYL.Complete((int)ci.IDX_IdxUpDn, EN_CYL_POS.Bwd))return false;
                    SM.MTR.GoHome((int)mi.IDX_X);
                    Step.iHome++;
                    return false;

                case 12:
                    if (!SM.MTR.GetHomeDone((int)mi.IDX_X)) return false;
                    if(!DM.ARAY[(int)ri.MRK].CheckAllStat(cs.None))
                    MoveMotr(mi.IDX_X, GetMotrPos(mi.IDX_X, pv.IDX_XMark) - 10);
                    Step.iHome++;
                    return false;

                case 13:
                    if (!GetStop(mi.IDX_X)) return false;
                    Step.iHome = 0;
                    return true ;
            }
        }
        public bool CycleToMark()
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

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return true;

                case 10:
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 11: 
                    
                    //시간 걸리므로 미리 함.
                    if(OM.DevOptn.bUseSerialDMC)
                    {
                        if(m_tmDelay.OnDelay(9000))
                        {
                            SetErr(ei.LSR_ComNG , "Laser " + SEQ.Laser.GetCycle().ToString() + " Cycle is Still Doing.");
                        }
                        if(!SEQ.Laser.GetCycleEnded())return false ;
                        SEQ.Laser.SetCycle(RS232_DominoDynamark3.Cycle.SetSerialNo);
                    }
                    


                    MoveActr(ci.IDX_IdxUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 12: 
                    if(!GetActrStop(ci.IDX_IdxUpDn, fb.Bwd))return false;
                    MoveMotr(mi.IDX_X, pv.IDX_XWait);
                    Step.iCycle++;
                    return false;

                
                case 13: 
                    if(!GetStop(mi.IDX_X))return false;
                    MoveActr(ci.IDX_IdxUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!GetActrStop(ci.IDX_IdxUpDn, fb.Fwd))return false;
                    //if(!OM.CmnOptn.bAirBlwrSkip) SetY(yi.IDX_AlgnBlw, true);
                    MoveMotr(mi.IDX_X, pv.IDX_XMark , true);
                    Step.iCycle++;
                    return false ;

                case 15: 
                    if(!GetStop(mi.IDX_X))return false;
                    //SetY(yi.IDX_AlgnBlw, false);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if (OM.DevOptn.bUseSerialDMC)
                    {
                        if (!SEQ.Laser.GetCycleEnded()) return false ;
                        if (SEQ.Laser.GetErr() != "")
                        {
                            SetErr(ei.LSR_ComNG, SEQ.Laser.GetErr());
                            //Step.iCycle=0;
                            //return true ;
                        }
                    }

                    DM.ShiftData(ri.PRI , ri.MRK);
                    Step.iCycle = 0;
                    return true;
            }
        }
        
        public bool CycleMark()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 251000))
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

            int r = 0, c = 0;
            
            
            switch (Step.iCycle)
            {

                default: sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                         //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                         Step.iCycle = 0;
                         return true;
                case 0:
                    return true;

                case 10:
                    if(OM.CmnOptn.bMarkSkip)
                    {
                        DM.ARAY[ri.MRK].SetStat(cs.Mark);
                        Step.iCycle = 0 ;
                        return true ;

                    }
                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 11:
                    //if(!m_tmDelay.OnDelay(10000))return false ;
                    //레이저 관련.
                    SEQ.Laser.SetCycle(RS232_DominoDynamark3.Cycle.Mark);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!SEQ.Laser.GetCycleEnded()) return false ;
                    if( SEQ.Laser.GetErr() != "")
                    {
                        SetErr(ei.LSR_ComNG , SEQ.Laser.GetErr());
                        Step.iCycle=0;
                        return true ;
                    }

                    DM.ARAY[ri.MRK].SetStat(cs.Mark);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleToBar()
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

            int r = 0, c = 0;


            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    Step.iCycle = 0;
                    return true;
                case 0:
                    return true;

                case 10:

                    MoveActr(ci.IDX_IdxUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ci.IDX_IdxUpDn, fb.Fwd)) return false;
                    MoveMotr(mi.IDX_X, pv.IDX_XBarcode);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!GetStop(mi.IDX_X)) return false;
                    DM.ARAY[ri.MRK].SetStat(cs.None);
                    DM.ARAY[ri.BAR].SetStat(cs.Mark);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleBarcode()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 8000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SEQ.Barcode.Stop();
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

            int r = 0, c = 0;


            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    Step.iCycle = 0;
                    return true;
                case 0:
                    return true;

                case 10:
                    if (OM.CmnOptn.bBarSkip)
                    {
                        DM.ARAY[ri.MRK].SetStat(cs.None);
                        DM.ARAY[ri.BAR].SetStat(cs.Good);
                        Step.iCycle = 0;
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 11:
                    //바코드
                    SEQ.Barcode.Read();
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if(m_tmDelay.OnDelay(6000))
                    {
                        SetErr(ei.BAR_Ng, "Barcode Reading Failed!");
                        SEQ.Barcode.Stop();
                        Step.iCycle = 0;
                        return true;
                    }
                    if (!SEQ.Barcode.ReadEnded()) return false;
                    if (LOT.m_sLotNo != SEQ.Barcode.GetReadingText().Substring(0, LOT.m_sLotNo.Length))
                    {
                        SetErr(ei.BAR_Ng, "Barcode(" + SEQ.Barcode.GetReadingText() + ") is diffrent from LotNo(" + LOT.m_sLotNo + ")");
                        Step.iCycle = 0;
                        return true;
                    }

                    DM.ARAY[ri.BAR].SetStat(cs.Good);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleOut()
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

            switch (Step.iCycle)
            {

                default: sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    Step.iCycle = 0;
                    return true;

                case 0:
                    
                    return true;

                case 10:
                    if (GetX(xi.IDX_Uld))
                    {
                        SetErr(ei.PRT_Detect, "Index Unloader StockZone Sensor Detected");
                        Step.iCycle = 0;
                        return true;
                    }
                    if (GetX(xi.IDX_UldOver)) 
                    {
                        SetErr(ei.PRT_Detect, "UnLoader Tray Count Over");
                        Step.iCycle = 0;
                        return true;
                    }
                    Step.iCycle++;
                    return false ;

                case 11:
                    if (SM.MTR.GetCmdPos((int)mi.IDX_X) < PM.GetValue(mi.IDX_X, pv.IDX_XBarcode) - 1)
                    {   //로더에서 바코드 존에 자제 있을때는 내려놓게 하는데 이때 홈잡고 스타트 하면 병신 됨.
                        //인덱스 핑거 올라오고 로더는 플레이스 하면서 처박음.
                        MoveActr(ci.IDX_IdxUpDn, fb.Bwd);
                        Step.iCycle++;
                        return false;

                    }

                    Step.iCycle = 20;
                    return false;

                case 12:
                    if (!GetActrStop(ci.IDX_IdxUpDn, fb.Bwd)) return false;
                    MoveMotr(mi.IDX_X, pv.IDX_XMark);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.IDX_X)) return false;
                    MoveActr(ci.IDX_IdxUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!GetActrStop(ci.IDX_IdxUpDn, fb.Fwd)) return false;
                    MoveMotr(mi.IDX_X, pv.IDX_XBarcode);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!GetStop(mi.IDX_X)) return false;
                    Step.iCycle = 20 ;
                    return false ;

                //위에서 씀. 여기부턴 원래 플로우 시에 타는 것.
                case 20 :
                    MoveActr(ci.IDX_StockUpDn, fb.Bwd);
                    MoveActr(ci.IDX_IdxUpDn  , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!GetActrStop(ci.IDX_StockUpDn, fb.Bwd)) return false;
                    if (!GetActrStop(ci.IDX_IdxUpDn  , fb.Fwd)) return false;
                    MoveMotr(mi.IDX_X, pv.IDX_XOut);
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!GetStop(mi.IDX_X)) return false;
                    MoveActr(ci.IDX_StockUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 23:
                    if(!GetX(xi.IDX_StockUp))return false;
                    MoveActr(ci.IDX_IdxUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 24:
                    MoveMotr(mi.IDX_X, pv.IDX_XWait);
                    MoveActr(ci.IDX_StockUpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 25:
                    if (!GetActrStop(ci.IDX_IdxUpDn, fb.Bwd)) return false;
                    if(!GetActrStop(ci.IDX_StockUpDn, fb.Bwd))return false;
                    
                    //여기서 SPC 저장.
                    SPC.LOT.AddWorkCntData(OM.DevInfo.iTrayColCnt * OM.DevInfo.iTrayRowCnt);
                    SPC.LOT.AddGoodCntData(OM.DevInfo.iTrayColCnt * OM.DevInfo.iTrayRowCnt);
                    
                    DM.ARAY[(int)ri.BAR].SetStat(cs.None);
                    Step.iCycle++;
                    return false;

                case 26:
                    MoveActr(ci.IDX_StockUpDn, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 27:
                    if (!GetActrStop(ci.IDX_StockUpDn, fb.Bwd)) return false;
                    Step.iCycle++;
                    return false;

                case 28:
                    if (!GetStop(mi.IDX_X)) return false;
                    Step.iCycle = 0;
                    return true;
            }
        }
        
        public bool CheckSafe(mi _iMotr, double _dPos)
        {
            //if(!SM.MTR.CmprPos((int)_iMotr, _dPos))return false;
            
            bool bRet = true;
            string sMsg = "";

            if (_iMotr == mi.IDX_X)
            {
                //if (SM.MTR.GetCmdPos((int)mi.IDX_X) > _dPos)
                //{
                //    if (SM.CYL.GetCmd((int)ci.IDX_IdxUpDn) == EN_CYL_POS.Fwd)
                //    {
                //        sMsg = "Index up/dn Cylinder must be down When need to move negative direction";
                //        bRet = false;
                //    }
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

            if(_iActr == ci.IDX_IdxUpDn){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = AnsiString("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_iActr == ci.IDX_StockUpDn)
            {

            }
            else {
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
            if (!SM.MTR.GetStop((int)mi.IDX_X         ))return false;

            if (!SM.CYL.Complete((int)ci.IDX_IdxUpDn  ))return false;
            if (!SM.CYL.Complete((int)ci.IDX_StockUpDn))return false;

            return true;
        }
    };

    

   
    
}
