using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON;
using SMDll2;



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

        public enum EN_SEQ_CYCLE
        {
            Idle   = 0,
            Supply = 1,
            Work   = 2,
            Out    = 3,
            MAX_SEQ_CYCLE
        };

        public struct SStep
        {
            public int iHome;
            public int iToStart;
            public EN_SEQ_CYCLE iSeq;
            public int iCycle;
            public int iToStop;
            public EN_SEQ_CYCLE iLastSeq;
            public void Clear()
            {
                iHome = 0;
                iToStart = 0;
                iSeq = EN_SEQ_CYCLE.Idle;
                iCycle = 0;
                iToStop = 0;
                iLastSeq = EN_SEQ_CYCLE.Idle;
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

            m_sCycleName  = new string[(int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE];
            m_CycleTime   = new CTimer[(int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE];

            for(int i = 0 ; i < (int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE ; i++)
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
            if (Step.iSeq != EN_SEQ_CYCLE.Idle) return false;

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
        public int GetSeqStep    () { return (int)Step.iSeq; } public int GetPreSeqStep    () { return (int)PreStep.iSeq; }
        public int GetCycleStep  () { return Step.iCycle   ; } public int GetPreCycleStep  () { return PreStep.iCycle   ; } public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; }
        public int GetToStopStep () { return Step.iToStop  ; } public int GetPreToStopStep () { return PreStep.iToStop  ; }
        public SStep GetStep     () { return Step; }

        public String GetCycleName(int _iSeq) { return m_sCycleName[_iSeq]; }
        public double GetCycleTime(int _iSeq) { return m_CycleTime [_iSeq].Duration; }
        public String GetPartName (         ) { return m_sPartName        ; }

        public int GetCycleMaxCnt() { return (int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE; }       

        public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            //Check Cycle Time Out.
            String sTemp;
            sTemp = String.Format("%s Step.iCycle={0:00}", "Autorun", Step.iCycle);
            if (Step.iSeq != PreStep.iSeq)
            {
                Log.Trace(m_sPartName, sTemp);
            }


            PreStep.iSeq = Step.iSeq;

            string sCycle = m_sCycleName[(int)Step.iSeq] ;

            //Check Error & Decide Step.
            if (Step.iSeq == EN_SEQ_CYCLE.Idle)
            {
                if (Stat.bReqStop) return false;

                int c = 1, r = 0; 
                if(DM.ARAY[(int)ri.WRK].GetCntStat(cs.Working) > 0) FindChip(ref c, ref r, cs.Working, ri.WRK);
                else                                                FindChip(ref c, ref r, cs.Unkwn  , ri.WRK);
                double dXPos = PM.GetValue(mi.IDX_X, pv.IDX_XPickStt) + ((OM.DevInfo.iTrayColCnt - c - 1) * OM.DevInfo.dTrayColPitch);

                bool bIDXWorkPos  = SM.MT.GetCmdPos((int)mi.IDX_X) >= dXPos - 0.1 &&
                                    SM.MT.GetCmdPos((int)mi.IDX_X) <= dXPos + 0.1 ;

                bool isCycleSupply = DM.ARAY[(int)ri.WRK].CheckAllStat(cs.None)    && DM.ARAY[(int)ri.PRI].CheckAllStat(cs.Unkwn) && GetX(xi.IDX_Pri) && !GetX(xi.IDX_PriOut);
                bool isCycleWork   =(DM.ARAY[(int)ri.WRK].GetCntStat(cs.Unkwn) > 0 || DM.ARAY[(int)ri.WRK].GetCntStat(cs.Working) > 0)&& SEQ.PCK.GetSeqStep() == 0 && !bIDXWorkPos;
                bool isCycleOut    = DM.ARAY[(int)ri.WRK].GetCntStat(cs.Unkwn) <=0 && DM.ARAY[(int)ri.WRK].GetCntStat(cs.Working) <=0 && !DM.ARAY[(int)ri.WRK].CheckAllStat(cs.None) &&
                                     !GetX(xi.IDX_Uld) ;
                bool isCycleEnd    = DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None) && DM.ARAY[(int)ri.WRK].CheckAllStat(cs.None);

                if (SEQ.LDR.GetSeqStep() == (int)EN_SEQ_CYCLE.Idle && DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None) && GetX(xi.IDX_Pri))
                {
                    SetErr(ei.PKG_Unknwn, "PreIndex Detected Tray.");
                }

                //여기부터 조건 잡자.


                if (SM.ER.IsErr())
                {
                    MoveActr(ai.LDR_TrayFixClOp, fb.Bwd);
                    return false;
                } 

                //Normal Decide Step.
                     if (isCycleSupply    ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Supply ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isCycleWork      ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Work   ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isCycleOut       ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Out    ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isCycleEnd       ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;
            }

            //Cycle.
            Step.iLastSeq = Step.iSeq;
            switch (Step.iSeq)
            {
                default                   :                         Log.Trace(m_sPartName, "default End");                                    Step.iSeq = EN_SEQ_CYCLE.Idle;   return false;
                case (EN_SEQ_CYCLE.Idle  ):                                                                                                                                    return false;
                case (EN_SEQ_CYCLE.Supply): if (CycleSupply   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Work  ): if (CycleWork     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Out   ): if (CycleOut      ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                
            }
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
        //protected CArray[] Aray;
        
        public bool FindChip(ref int c, ref int r, cs _iChip, ri _iId) //이거 되는지 확인 해야함
        {
            r = 0;
            c = DM.ARAY[(int)_iId].FindLastCol(_iChip); 
            
            return (c >= 0 && r >= 0) ? false : true;
        }

        public double GetMotrPos (mi _iMotr , pv _iPstnValue )
        {
            return PM.GetValue((uint)_iMotr , (uint)_iPstnValue);
        }
        
        public void InitCycleName(){
            m_sCycleName = new String[(int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE];
            m_sCycleName[(int)EN_SEQ_CYCLE.Idle     ]="Idle"       ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Supply   ]="Supply"     ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Work     ]="Work"       ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Out      ]="Out"        ;
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
                    SM.CL.Move((int)ai.IDX_IdxUpDn, EN_CYLINDER_POS.cpBwd);
                    Step.iHome++;
                    return false ;
            
                case 11:
                    if(!SM.CL.Complete((int)ai.IDX_IdxUpDn, EN_CYLINDER_POS.cpBwd))return false;
                    SM.MT.GoHome((int)mi.IDX_X);
                    Step.iHome++;
                    return false;

                case 12:
                    if (!SM.MT.GetHomeDone((int)mi.IDX_X)) return false;
                    if(!DM.ARAY[(int)ri.WRK].CheckAllStat(cs.None))
                        MoveMotr(mi.IDX_X, GetMotrPos(mi.IDX_X, pv.IDX_XPickStt) - 10);
                    Step.iHome++;
                    return false;

                case 13:
                    if (!GetStop(mi.IDX_X)) return false;
                    Step.iHome = 0;
                    return true ;
            }
        }
        public bool CycleSupply()
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
                    return false;

                case 10: 
                    MoveActr(ai.IDX_IdxUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!GetActrStop(ai.IDX_IdxUpDn, fb.Bwd))return false;
                    MoveMotr(mi.IDX_X, pv.IDX_XWait);
                    Step.iCycle++;
                    return false;

                
                case 12: 
                    if(!GetStop(mi.IDX_X))return false;
                    MoveActr(ai.IDX_IdxUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!GetActrStop(ai.IDX_IdxUpDn, fb.Fwd))return false;
                    if(!OM.CmnOptn.bAirBlwrSkip) SetY(yi.IDX_AlgnBlw, true);
                    MoveMotr(mi.IDX_X, pv.IDX_XPickStt);
                    Step.iCycle++;
                    return false ;

                case 14: 
                    if(!GetStop(mi.IDX_X))return false;
                    SetY(yi.IDX_AlgnBlw, false);
                    DM.ShiftData((int)ri.PRI , (int)ri.WRK);
                    Step.iCycle = 0;
                    return true;
            }
        }
        
        public bool CycleWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.ETC_AllHomeTO, sTemp);
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
                         return false;


                case 10:
                    //MoveActr(ai.IDX_IdxUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    //if (!GetActrStop(ai.IDX_IdxUpDn, fb.Bwd)) return false;
                    //dXPos = PM.GetValue(mi.IDX_X, pv.IDX_XPickStt) - 10;
                    //MoveMotr(mi.IDX_X, dXPos);
                    Step.iCycle++;
                    return false;

                case 12:
                    //if (!GetStop(mi.IDX_X)) return false;
                    MoveActr(ai.IDX_IdxUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!GetActrStop(ai.IDX_IdxUpDn, fb.Fwd))return false;
                    if(DM.ARAY[(int)ri.WRK].GetCntStat(cs.Working) > 0) FindChip(ref c, ref r, cs.Working, ri.WRK);
                    else                                                FindChip(ref c, ref r, cs.Unkwn  , ri.WRK);
                    dXPos = PM.GetValue(mi.IDX_X, pv.IDX_XPickStt) + ((OM.DevInfo.iTrayColCnt - c - 1) * OM.DevInfo.dTrayColPitch);
                    MoveMotr(mi.IDX_X, dXPos);
                    Step.iCycle++;
                    return false;

                case 14: 
                    if(!GetStop(mi.IDX_X))return false;
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
                SetErr(ei.ETC_AllHomeTO, sTemp);
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
            FindChip(ref c, ref r, cs.Unkwn, ri.WRK);

            switch (Step.iCycle)
            {

                default: sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    if (GetX(xi.IDX_UldOver))
                    {
                        SetErr(ei.PRT_Detect, "UnLoader Tray Count Over");
                        Step.iCycle = 0;
                        return true;
                    }
                    MoveActr(ai.IDX_StockUpDn, fb.Bwd);
                    MoveActr(ai.IDX_IdxUpDn  , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ai.IDX_StockUpDn, fb.Bwd)) return false;
                    if (!GetActrStop(ai.IDX_IdxUpDn  , fb.Fwd)) return false;
                    MoveMotr(mi.IDX_X, pv.IDX_XOut);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!GetStop(mi.IDX_X)) return false;
                    MoveActr(ai.IDX_StockUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!GetX(xi.IDX_StockUp))return false;
                    MoveActr(ai.IDX_IdxUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    MoveMotr(mi.IDX_X, pv.IDX_XWait);
                    MoveActr(ai.IDX_StockUpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 15:
                    if (!GetActrStop(ai.IDX_IdxUpDn, fb.Bwd)) return false;
                    if(!GetActrStop(ai.IDX_StockUpDn, fb.Bwd))return false;
                    
                    //여기서 SPC 저장.
                    SPC.LOT.AddWorkCntData(DM.ARAY[(int)ri.WRK].GetMaxCol() * DM.ARAY[(int)ri.WRK].GetMaxRow());
                    SPC.LOT.AddGoodCntData(DM.ARAY[(int)ri.WRK].GetCntStat(cs.Good));
                    SPC.LOT.LoadSaveLotIni(false);
                    DM.ARAY[(int)ri.WRK].SetStat(cs.None);
                    Step.iCycle++;
                    return false;

                case 16:
                    MoveActr(ai.IDX_StockUpDn, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!GetActrStop(ai.IDX_StockUpDn, fb.Bwd)) return false;
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!GetStop(mi.IDX_X)) return false;
                    Step.iCycle = 0;
                    return true;
            }
        }
        
        public bool CheckSafe(mi _iMotr, double _dPos)
        {
            //if(!SM.MT.CmprPos((int)_iMotr, _dPos))return false;
            
            bool bRet = true;
            string sMsg = "";

            if (_iMotr == mi.IDX_X)
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
                sMsg = "Motor " + SM.MT.GetName((int)_iMotr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(SM.MT.GetName((int)_iMotr), sMsg);
                if (Step.iSeq == 0) Log.ShowMessage(SM.MT.GetName((int)_iMotr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        //Lee.
        public bool CheckSafe(ai _iActr, EN_CYLINDER_POS _bFwd)
        {
            if (SM.CL.Complete((int)_iActr, _bFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_iActr == ai.IDX_IdxUpDn){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = AnsiString("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_iActr == ai.IDX_StockUpDn)
            {

            }
            else {
                sMsg = "Cylinder " + SM.CL.GetName((int)_iActr) + " is Not this parts.";
                bRet = false;
            }


            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(SM.CL.GetName((int)_iActr), sMsg);
                if (Step.iCycle==0) Log.ShowMessage(SM.CL.GetName((int)_iActr), sMsg);
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

            if(_bSlow) {SM.MT.GoAbsSlow((int)_iMotr, _dPos); return false;}
            else { 
                if(Step.iCycle != 0) { SM.MT.GoAbsRun((int)_iMotr, _dPos); }
                else                 { SM.MT.GoAbsMan((int)_iMotr, _dPos); }
            }

            return GetStop(_iMotr)  ;         
        }

        public bool MoveMotr(mi _iMotr , pv _iPosId , bool _bSlow=false)
        {
            return MoveMotr(_iMotr , GetMotrPos(_iMotr,_iPosId) , _bSlow);      
        }

        public void SetY(yi _YAdd , bool _bVal)
        {
            SM.IO.SetY((int)_YAdd , _bVal);
        }

        public bool GetY(yi _YAdd)
        {
            return SM.IO.GetY((int)_YAdd);
        }

        public bool GetX(xi _XAdd)
        {
            return SM.IO.GetX((int)_XAdd);
        }

        public bool GetStop(mi _iMotr)
        {
            return SM.MT.GetStopInpos((int)_iMotr);

        }
        public void SetErr(ei _eErrNo, string _sMsg = "")
        {
            if (_sMsg == "") SM.ER.SetErr((int)_eErrNo);
            SM.ER.SetErrMsg((int)_eErrNo, _sMsg);
        }
        
        public bool MoveActr(ai _iActr, fb _bFwd)
        {
            EN_CYLINDER_POS m_bFwd = (EN_CYLINDER_POS)_bFwd;
            if (!CheckSafe(_iActr, m_bFwd)) return false;

            SM.CL.Move((int)_iActr, m_bFwd);
            
            return true;
        }

        public bool GetActrStop(ai _iActr, fb _bFwd)
        {
            if(SM.CL.Complete((int)_iActr, (EN_CYLINDER_POS)_bFwd)){
                return true;
            }
            return false;
        }

        public bool CheckStop()
        {
            if (!SM.MT.GetStop((int)mi.IDX_X         ))return false;

            if (!SM.CL.Complete((int)ai.IDX_IdxUpDn  ))return false;
            if (!SM.CL.Complete((int)ai.IDX_StockUpDn))return false;

            return true;
        }
    };

    

   
    
}
