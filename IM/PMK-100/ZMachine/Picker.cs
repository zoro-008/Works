using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON;
using SMDll2;
using System.IO;

namespace Machine
{
    //PCK == Picker
    public class Picker : PartInterface
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
            Idle    = 0,
            Pick    = 1,
            Align   = 2,
            Marking = 3,
            Vision  = 4,
            Place   = 5,
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

        protected TWorkInfo WorkInfo;

        protected double m_dLastIdxPos;
        protected String m_sCheckSafeMsg;

        public string[] m_sCycleName;
        public CTimer[] m_CycleTime;

        public int iPickerCnt = 8;

        

        public Picker()
        {
            m_sPartName = "Picker";

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
                double dXPos = PM.GetValue(mi.IDX_X, pv.IDX_XPickStt) + (OM.DevInfo.iTrayColCnt - c - 1) * OM.DevInfo.dTrayColPitch;

                bool bIDXWorkPos  = SM.MT.GetCmdPos((int)mi.IDX_X) >= dXPos - 0.1 &&
                                    SM.MT.GetCmdPos((int)mi.IDX_X) <= dXPos + 0.1 ;

                bool bPckrVccSnsr = (GetX(xi.PCK_Vcc1) || GetX(xi.PCK_Vcc2) || GetX(xi.PCK_Vcc3) || GetX(xi.PCK_Vcc4) ||
                                     GetX(xi.PCK_Vcc5) || GetX(xi.PCK_Vcc6) || GetX(xi.PCK_Vcc7) || GetX(xi.PCK_Vcc8));

                bool isCyclePick  =  DM.ARAY[(int)ri.PCK].CheckAllStat(cs.None) && DM.ARAY[(int)ri.WRK].GetCntStat(cs.Unkwn) > 0 && 
                                     GetStop(mi.IDX_X) && bIDXWorkPos;
                //bool isCycleAlgn  =  DM.ARAY[(int)ri.PCK].GetCntStat(cs.Unkwn  ) > 0 && bPckrVccSnsr;
                bool isCyclePrnt  =  DM.ARAY[(int)ri.PCK].GetCntStat(cs.Unkwn  ) > 0 && bPckrVccSnsr;
                bool isCycleVsn   =  DM.ARAY[(int)ri.PCK].GetCntStat(cs.Marking) > 0 && bPckrVccSnsr;
                bool isCyclePlace = !DM.ARAY[(int)ri.PCK].CheckAllStat(cs.None) /*&& !isCycleAlgn */&& !isCyclePrnt && !isCycleVsn;

                bool isCycleEnd = Step.iSeq == EN_SEQ_CYCLE.Idle;


                //여기부터 조건 잡자.


                if (SM.ER.IsErr())
                {
                    MoveActr(ai.LDR_TrayFixClOp, fb.Bwd);
                    return false;
                } 

                //Normal Decide Step.
                     if (isCyclePick ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Pick   ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                //else if (isCycleAlgn ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Align  ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); } 
                else if (isCyclePrnt ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Marking; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); } 
                else if (isCycleVsn  ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Vision ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); } 
                else if (isCyclePlace) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Place  ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); } 
                else if (isCycleEnd  ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;
            }

            //Cycle.
            Step.iLastSeq = Step.iSeq;
            switch (Step.iSeq)
            {
                default                    :                    Log.Trace(m_sPartName, "default End");                                    Step.iSeq = EN_SEQ_CYCLE.Idle;   return false;
                case (EN_SEQ_CYCLE.Idle   ):                                                                                                                               return false;
                case (EN_SEQ_CYCLE.Pick   ): if (CyclePick()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                //case (EN_SEQ_CYCLE.Align  ): if (CycleAlgn()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Marking): if (CyclePrnt()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Vision ): if (CycleVsn ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Place  ): if (CyclePlce()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                
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
        
        public bool FindChip(ref int c, ref int r, cs _iChip, ri _iId, bool _bFsrt = true) //이거 되는지 확인 해야함
        {
            if(_bFsrt)
                DM.ARAY[(int)_iId].FindFrstRowLastCol(_iChip, ref c, ref r);
            else{
                DM.ARAY[(int)_iId].FindLastRowCol(_iChip, ref c, ref r);
                
            }

            return (r > -1 && c > -1) ? false : true;
        }
        protected struct TWorkInfo {
            public int iCol ;
            public int iRow ;
            public cs eStat ;
        } ;//오토런에서 스테이지에서 정보를 가져다 담아 놓고 Cycle에서 이것을 쓴다....

        public double GetMotrPos (mi _iMotr , pv _iPstnValue )
        {
            return PM.GetValue((uint)_iMotr , (uint)_iPstnValue);
        }
        
        public void InitCycleName(){
            m_sCycleName = new String[(int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE];
            m_sCycleName[(int)EN_SEQ_CYCLE.Idle      ]="Idle  "        ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Pick      ]="Solder"      ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Align     ]="Clean "       ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Marking   ]="Solder"      ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Vision    ]="Clean "       ;
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
                    SM.MT.GoHome((int)mi.PCK_Z);
                    Step.iHome++;
                    return false ;
            
                case 11:
                    if (!SM.MT.GetHomeDone((int)mi.PCK_Z)) return false;
                    SM.MT.GoHome((int)mi.PCK_Y);
                    Step.iHome++;
                    return false;

                case 12:
                    if (!SM.MT.GetHomeDone((int)mi.PCK_Y)) return false;
                    MoveMotr(mi.PCK_Y, pv.PCK_YWait);
                    Step.iHome++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_Y)) return false;
                    Step.iHome = 0;
                    return true ;
            }
        }

        public int iPckShakeCnt = 0;
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

            int c = 0, r = 0;
            double dPickPos = 0;
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    SPC.LOT.dTickTime = SPC.LOT.m_tmTick.CheckTime_s();
                    SPC.LOT.m_tmTick.Clear();
                    MoveMotr(mi.PCK_Z, pv.PCK_ZMove);
                    iPckShakeCnt = 0;
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_Z)) return false;
                    MoveMotr(mi.PCK_Y, pv.PCK_YPick);
                    Step.iCycle++;
                    return false;

                
                case 12: 
                    if(!GetStop(mi.PCK_Y)) return false;
                    if (OM.DevOptn.bUsePckShake)
                    {
                        m_tmDelay.Clear();
                        Step.iCycle = 30;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                //Down Used.
                case 13:
                    MoveMotr(mi.PCK_Z, pv.PCK_ZPick);
                    Step.iCycle++;
                    return false ;


                
                case 14:
                    if(!GetStop(mi.PCK_Z)) return false;
                    SetY(yi.PCK_Vcc1, true);
                    SetY(yi.PCK_Vcc2, true);
                    SetY(yi.PCK_Vcc3, true);
                    SetY(yi.PCK_Vcc4, true);
                    SetY(yi.PCK_Vcc5, true);
                    SetY(yi.PCK_Vcc6, true);
                    SetY(yi.PCK_Vcc7, true);
                    SetY(yi.PCK_Vcc8, true);
                    m_tmDelay.Clear();
                    
                    Step.iCycle++;
                    return false;

                
                case 15:
                    if(!m_tmDelay.OnDelay(OM.DevOptn.iPickWaitTime))return false ;
                    if(GetX(xi.PCK_Vcc1)) DM.ARAY[(int)ri.PCK].SetStat(0, 0, cs.Unkwn);
                    else                  DM.ARAY[(int)ri.PCK].SetStat(0, 0, cs.Empty);
                    if(GetX(xi.PCK_Vcc2)) DM.ARAY[(int)ri.PCK].SetStat(0, 1, cs.Unkwn);
                    else                  DM.ARAY[(int)ri.PCK].SetStat(0, 1, cs.Empty);
                    if(GetX(xi.PCK_Vcc3)) DM.ARAY[(int)ri.PCK].SetStat(0, 2, cs.Unkwn);
                    else                  DM.ARAY[(int)ri.PCK].SetStat(0, 2, cs.Empty);
                    if(GetX(xi.PCK_Vcc4)) DM.ARAY[(int)ri.PCK].SetStat(0, 3, cs.Unkwn);
                    else                  DM.ARAY[(int)ri.PCK].SetStat(0, 3, cs.Empty);
                    if(GetX(xi.PCK_Vcc5)) DM.ARAY[(int)ri.PCK].SetStat(0, 4, cs.Unkwn);
                    else                  DM.ARAY[(int)ri.PCK].SetStat(0, 4, cs.Empty);
                    if(GetX(xi.PCK_Vcc6)) DM.ARAY[(int)ri.PCK].SetStat(0, 5, cs.Unkwn);
                    else                  DM.ARAY[(int)ri.PCK].SetStat(0, 5, cs.Empty);
                    if(GetX(xi.PCK_Vcc7)) DM.ARAY[(int)ri.PCK].SetStat(0, 6, cs.Unkwn);
                    else                  DM.ARAY[(int)ri.PCK].SetStat(0, 6, cs.Empty);
                    if(GetX(xi.PCK_Vcc8)) DM.ARAY[(int)ri.PCK].SetStat(0, 7, cs.Unkwn);
                    else                  DM.ARAY[(int)ri.PCK].SetStat(0, 7, cs.Empty);
                    FindChip(ref c, ref r, cs.Unkwn, ri.WRK);

                    for (int i = 0; i < OM.DevInfo.iTrayRowCnt; i++ )
                    {
                        DM.ARAY[(int)ri.WRK].SetStat(c, i, cs.Working);
                    }

                    MoveMotr(mi.PCK_Z, pv.PCK_ZMove);
                    Step.iCycle++;
                    return false;

                
                case 16:
                    if(!GetStop(mi.PCK_Z)) return false;
                    if (DM.ARAY[(int)ri.PCK].GetCntStat(cs.Unkwn) <= 0)
                    {
                        DM.ARAY[(int)ri.WRK].ChangeStat(cs.Working, cs.Empty);
                        DM.ARAY[(int)ri.PCK].SetStat(cs.None);
                        SetErr(ei.PRT_Missed, "Picker Position Miss && Tray Work Position Miss");
                    }
                    Step.iCycle = 0;
                    return true ;

                case 30:
                    dPickPos = GetMotrPos(mi.PCK_Z, pv.PCK_ZPick) - OM.DevOptn.dPckShakeZOfs;
                    MoveMotr(mi.PCK_Z, dPickPos);
                    Step.iCycle++;
                    return false;

                //Down Used.
                case 31:
                    if (!GetStop(mi.PCK_Z)) return false;
                    if (!GetStop(mi.PCK_Y)) return false;
                    
                    double dPosShakeYOfs = GetMotrPos(mi.PCK_Y, pv.PCK_YPick) + OM.DevOptn.dPckShakeDistance;
                    MoveMotr(mi.PCK_Y, dPosShakeYOfs);
                    Step.iCycle++;
                    return false;

                case 32:
                    if (!GetStop(mi.PCK_Y)) return false;
                    MoveMotr(mi.PCK_Y, pv.PCK_YPick);
                    Step.iCycle++;
                    return false;

                case 33:
                    if (!GetStop(mi.PCK_Y)) return false;
                    double dNegShakeYOfs = GetMotrPos(mi.PCK_Y, pv.PCK_YPick) - OM.DevOptn.dPckShakeDistance;
                    MoveMotr(mi.PCK_Y, dNegShakeYOfs);
                    Step.iCycle++;
                    return false;

                case 34:
                    if (!GetStop(mi.PCK_Y)) return false;             
                    MoveMotr(mi.PCK_Y, pv.PCK_YPick);
                    iPckShakeCnt++;
                    if (iPckShakeCnt < OM.DevOptn.iPckShakeCnt)
                    {
                        Step.iCycle = 31;
                        return false;
                    }
                    Step.iCycle = 13;
                    return false;
            }
        }

        //public bool CycleAlgn()
        //{
        //    String sTemp;
        //    if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 100000))
        //    {
        //        sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
        //        sTemp = m_sPartName + sTemp;
        //        SetErr(ei.PRT_CycleTO, sTemp);
        //        Log.Trace(m_sPartName, sTemp);
        //        Step.iCycle = 0;
        //        return true;
        //    }

        //    if (Step.iCycle != PreStep.iCycle)
        //    {
        //        sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
        //        Log.Trace(m_sPartName, sTemp);
        //    }

        //    PreStep.iCycle = Step.iCycle;

        //    if (Stat.bReqStop)
        //    {
        //        //Step.iHome = 0;
        //        //return true ;
        //    }

        //    switch (Step.iCycle)
        //    {

        //        default:
        //            sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
        //            //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
        //            Step.iCycle = 0;
        //            return true;

        //        case 0:
        //            return false;


        //        case 10:
        //            MoveMotr(mi.PCK_Z, pv.PCK_ZMove);
        //            Step.iCycle++;
        //            return false;

        //        case 11:
        //            if (!GetStop(mi.PCK_Z)) return false;
        //            MoveMotr(mi.PCK_Y, pv.PCK_YAlgn);
        //            Step.iCycle++;
        //            return false;


        //        case 12:
        //            if (!GetStop(mi.PCK_Y)) return false;
        //            //왠지 여기서는 다 내리면 안될꺼 같다. 그래서 0.5mm는 올리고 베큠끄자.
        //            //MoveMotr(mi.PCK_Z, pv.PCK_ZAlgn);
        //            SM.MT.GoAbsRun((int)mi.PCK_Z, PM.GetValue(mi.PCK_Z, pv.PCK_ZAlgn) - 0.5);
        //            Step.iCycle++;
        //            return false;

        //        case 13:
        //            if (!GetStop(mi.PCK_Z)) return false;
        //            SetY(yi.PCK_Vcc1, false);
        //            SetY(yi.PCK_Vcc2, false);
        //            SetY(yi.PCK_Vcc3, false);
        //            SetY(yi.PCK_Vcc4, false);
        //            SetY(yi.PCK_Vcc5, false);
        //            SetY(yi.PCK_Vcc6, false);
        //            SetY(yi.PCK_Vcc7, false);
        //            SetY(yi.PCK_Vcc8, false);
                    
        //            for(int i = 0; i < OM.DevInfo.iTrayRowCnt; i++){
        //                DM.ARAY[(int)ri.ALN].SetStat(0, i, DM.ARAY[(int)ri.PCK].GetStat(0, i));
        //            }
                    
        //            m_tmDelay.Clear();
        //            Step.iCycle++;
        //            return false;

        //        case 14:
        //            if(!m_tmDelay.OnDelay(OM.DevOptn.iAlgnWaitTime))return false;
        //            SM.MT.GoAbsSlow((int)mi.PCK_Z, PM.GetValue(mi.PCK_Z, pv.PCK_ZAlgn));
        //            Step.iCycle++;
        //            return false;

        //        case 15:
        //            if(!GetStop(mi.PCK_Z))return false;
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,0) == cs.Unkwn) SetY(yi.PCK_Vcc1, true);
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,1) == cs.Unkwn) SetY(yi.PCK_Vcc2, true);
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,2) == cs.Unkwn) SetY(yi.PCK_Vcc3, true);
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,3) == cs.Unkwn) SetY(yi.PCK_Vcc4, true);
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,4) == cs.Unkwn) SetY(yi.PCK_Vcc5, true);
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,5) == cs.Unkwn) SetY(yi.PCK_Vcc8, true);
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,6) == cs.Unkwn) SetY(yi.PCK_Vcc6, true);
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,7) == cs.Unkwn) SetY(yi.PCK_Vcc7, true);
        //            m_tmDelay.Clear();
        //            Step.iCycle++;
        //            return false;

        //        case 16:
        //            if (!m_tmDelay.OnDelay(OM.DevOptn.iPickWaitTime)) return false;
        //            if(DM.ARAY[(int)ri.PCK].GetStat(0,0) == cs.Unkwn && GetX(xi.PCK_Vcc1) ||
        //               DM.ARAY[(int)ri.PCK].GetStat(0,1) == cs.Unkwn && GetX(xi.PCK_Vcc2) ||
        //               DM.ARAY[(int)ri.PCK].GetStat(0,2) == cs.Unkwn && GetX(xi.PCK_Vcc3) ||
        //               DM.ARAY[(int)ri.PCK].GetStat(0,3) == cs.Unkwn && GetX(xi.PCK_Vcc4) ||
        //               DM.ARAY[(int)ri.PCK].GetStat(0,4) == cs.Unkwn && GetX(xi.PCK_Vcc5) ||
        //               DM.ARAY[(int)ri.PCK].GetStat(0,5) == cs.Unkwn && GetX(xi.PCK_Vcc6) ||
        //               DM.ARAY[(int)ri.PCK].GetStat(0,6) == cs.Unkwn && GetX(xi.PCK_Vcc7) ||
        //               DM.ARAY[(int)ri.PCK].GetStat(0, 7) == cs.Unkwn && GetX(xi.PCK_Vcc8))
        //            {
        //                SetErr(ei.PRT_Missed, "Align After PickUp Miss Error.");
        //                Step.iCycle = 0;
        //                return true;
        //            }

        //            for (int i = 0; i < OM.DevInfo.iTrayRowCnt; i++)
        //            {
        //                DM.ARAY[(int)ri.PCK].SetStat(0, i, DM.ARAY[(int)ri.ALN].GetStat(0, i));
        //                DM.ARAY[(int)ri.PCK].ChangeStat(cs.Unkwn, cs.Align);
        //            }
        //            MoveMotr(mi.PCK_Z, pv.PCK_ZMove);
        //            Step.iCycle++;
        //            return false;


        //        case 17:
        //            if (!GetStop(mi.PCK_Z)) return false;
        //            Step.iCycle = 0;
        //            return true;
        //    }
        //}
        
        public bool CyclePrnt()
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

            double dPCKYPos = 0.0;
            
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    dPCKYPos = SM.MT.GetCmdPos((int)mi.PCK_Y);

                    if(dPCKYPos > PM.GetValue(mi.PCK_Y, pv.PCK_YPlce) + 0.1 ||
                       dPCKYPos > PM.GetValue(mi.PCK_Y, pv.PCK_YPlce) - 0.1)
                    {
                        MoveMotr(mi.PCK_Z, pv.PCK_ZMove) ;
                        Step.iCycle++;
                        return false;
                    }
                    Step.iCycle = 12;
                    return false;

                case 11:
                    if(!GetStop(mi.PCK_Z))return false;
                    MoveMotr(mi.PCK_Y, pv.PCK_YPlce);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!GetStop(mi.PCK_Y))return false;
                    MoveMotr(mi.PCK_Z, pv.PCK_ZPrnt);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_Z)) return false;
                    MoveMotr(mi.PCK_Y, pv.PCK_YPrnt);
                    Step.iCycle++;
                    return false;


                case 14:
                    if(GetX(xi.PCK_MrkrPkg))return false;
                    SetY(yi.PCK_PrntOn, true);
                    Step.iCycle ++;
                    return false;

                case 15:
                    if(!GetStop(mi.PCK_Y))return false;
                    SetY(yi.PCK_PrntOn , false);
                    DM.ARAY[(int)ri.PCK].ChangeStat(cs.Unkwn, cs.Marking);
                    Step.iCycle = 0;
                    return true;
            }
        }

        
        //다시 확인.JS
        public bool CycleVsn()
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

            double dPCKYPos = 0.0;
            
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                //Vision 위치가 안맞았을때 다시 움직이기 위해서
                case 10:
                    dPCKYPos = SM.MT.GetCmdPos((int)mi.PCK_Y);

                    if (dPCKYPos < PM.GetValue(mi.PCK_Y, pv.PCK_YVisn) + 20)
                    {
                        MoveMotr(mi.PCK_Z, pv.PCK_ZMove);
                        Step.iCycle++;
                        return false;
                    }
                    Step.iCycle = 12;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_Z)) return false;
                    MoveMotr(mi.PCK_Y, pv.PCK_YVisn);
                    Step.iCycle++;
                    return false;

                //Vision 위치 마추고 시작.
                case 12:
                    if (!GetStop(mi.PCK_Y)) return false;
                    MoveMotr(mi.PCK_Z, pv.PCK_ZVisn);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_Z)) return false;
                    //IO On
                    SM.MT.ResetTrgPos((int)mi.PCK_Y);
                    SetTrgPos();
                    Step.iCycle++;
                    return false;

                //아래서 씀.
                case 14:
                    if(!OM.CmnOptn.bVisnSkip) VC.SendVisnMsg(VC.sm.Ready);
                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;
                
                case 15:
                    if (!OM.CmnOptn.bVisnSkip && !VC.IsEndSendMsg()) return false;
                    if (!OM.CmnOptn.bVisnSkip && VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }
                    if (!OM.CmnOptn.bVisnSkip && VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr,"Vision Not Ready");
                        Step.iCycle=0 ;
                        return true ;
                    }
                    //에러 수정JS
                    //if (m_tmDelay.OnDelay(2000))
                    //{
                    //    SM.ER.SetErr(ei.eiPCK_VisnComErr, "Vision Communication Error(READY)");
                    //    Step.iCycle = 0;
                    //    return true;
                    //}
                    //if (!OM.MstOptn.bDebugMode) 
                    if(!OM.CmnOptn.bVisnSkip) VC.ClearRecvData();
                    MoveMotr(mi.PCK_Y, pv.PCK_YPlce);
                    Step.iCycle++;
                    return false;


                case 16:
                    if (!GetStop(mi.PCK_Y)) return false;
                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    //통신 Delay 시간 정하기.
                    //에러 수정JS
                    //if (m_tmDelay.OnDelay(1000))
                    //{
                    //    SM.ER.SetErr(ei.eiPCK_VisnComErr, "Vision Communication Error(RESULT)");
                    //    Step.iCycle = 0;
                    //    return true;
                    //}
                    //if (!VC.m_bRESULT) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if(m_tmDelay.OnDelay(true, 3000))
                    {
                        SetErr(ei.PCK_VisnComErr, "Vision Communication Error(RESULT)");
                        Step.iCycle = 0;
                        return true;
                    }
                    if (!OM.CmnOptn.bVisnSkip && VC.GetVisnRecvMsg() == "") return false;
                    if (!OM.CmnOptn.bVisnSkip)
                    { 
                        int r, c = 0;
                        string sVsnRecvMsg = VC.GetVisnRecvMsg();
                        string sResult = "";
                        string[] sVsnResult ;
                        int iResultCnt = 0;
                        
                        DM.ARAY[(int)ri.PCK].SetStat(cs.Good);
                        //if (sVsnRecvMsg == "000;")
                        //{
                        //    DM.ARAY[(int)ri.PCK].SetStat(cs.Good);
                        //}
                        if (sVsnRecvMsg != "000;") 
                        {
                            sVsnResult = sVsnRecvMsg.Split(';');
                            iResultCnt = int.Parse(sVsnResult[0].ToString());
                            for (int i = 1; i <= iResultCnt; i++) {
                                sResult = sVsnResult[i];
                                if(sResult != "" ){
                                    r = int.Parse(sResult.Substring(0, 2));
                                    c = int.Parse(sResult.Substring(2, 2));
                                    sResult = sResult.Substring(4, 1);

                                    //뒤집혀 있어서 뒤집는데
                                    //비전에서 바꿔주면 다시 바꾸기.JS
                                    //DM.ARAY[(int)ri.PCK].SetStat(c, r, cs.Fail);
                                    DM.ARAY[(int)ri.PCK].SetStat(c-1, r-1, cs.Fail);
                                }
                            }
                        }
                    }
                    else {
                        DM.ARAY[(int)ri.PCK].ChangeStat(cs.Marking, cs.Good );
                        DM.ARAY[(int)ri.PCK].ChangeStat(cs.Empty  , cs.Empty);
                    }
                    //결과처리.JS
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePlce()
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

            int c = 0, r = 0;
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;


                case 10:
                    MoveMotr(mi.PCK_Z, pv.PCK_ZMove);
                    Step.iCycle++;
                    return false;

                //Down...
                case 11:
                    if (!GetStop(mi.PCK_Z)) return false;
                    if (DM.ARAY[(int)ri.PCK].GetCntStat(cs.Fail) > 0)
                    {
                        FindChip(ref c, ref r, cs.Fail, ri.PCK, false);
                        MoveMotr(mi.PCK_Y, (PM.GetValue(mi.PCK_Y, pv.PCK_YPlce) - ((OM.DevInfo.iTrayRowCnt - r - 1)* OM.DevInfo.dTrayRowPitch)));
                        Step.iCycle++;
                        return false;
                    }
                    Step.iCycle = 15;
                    return false;
                    


                case 12:
                    if (!GetStop(mi.PCK_Y)) return false;
                    MoveMotr(mi.PCK_Z, pv.PCK_ZPlce);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_Z)) return false;
                    FindChip(ref c, ref r, cs.Fail, ri.PCK, false);
                         if(r == 0) { SetY(yi.PCK_Vcc1, false); SetY(yi.PCK_Ejt1, true); }
                    else if(r == 1) { SetY(yi.PCK_Vcc2, false); SetY(yi.PCK_Ejt2, true); }
                    else if(r == 2) { SetY(yi.PCK_Vcc3, false); SetY(yi.PCK_Ejt3, true); }
                    else if(r == 3) { SetY(yi.PCK_Vcc4, false); SetY(yi.PCK_Ejt4, true); }
                    else if(r == 4) { SetY(yi.PCK_Vcc5, false); SetY(yi.PCK_Ejt5, true); }
                    else if(r == 5) { SetY(yi.PCK_Vcc6, false); SetY(yi.PCK_Ejt6, true); }
                    else if(r == 6) { SetY(yi.PCK_Vcc7, false); SetY(yi.PCK_Ejt7, true); }
                    else if(r == 7) { SetY(yi.PCK_Vcc8, false); SetY(yi.PCK_Ejt8, true); }
                    DM.ARAY[(int)ri.PCK].SetStat(c, r, cs.Empty);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!m_tmDelay.OnDelay(OM.DevOptn.iPlceWaitTime)) return false;
                    if(GetY(yi.PCK_Ejt1)) SetY(yi.PCK_Ejt1, false);
                    if(GetY(yi.PCK_Ejt2)) SetY(yi.PCK_Ejt2, false);
                    if(GetY(yi.PCK_Ejt3)) SetY(yi.PCK_Ejt3, false);
                    if(GetY(yi.PCK_Ejt4)) SetY(yi.PCK_Ejt4, false);
                    if(GetY(yi.PCK_Ejt5)) SetY(yi.PCK_Ejt5, false);
                    if(GetY(yi.PCK_Ejt6)) SetY(yi.PCK_Ejt6, false);
                    if(GetY(yi.PCK_Ejt7)) SetY(yi.PCK_Ejt7, false);
                    if(GetY(yi.PCK_Ejt8)) SetY(yi.PCK_Ejt8, false);
                    if (DM.ARAY[(int)ri.PCK].GetCntStat(cs.Fail) > 0)
                    {
                        Step.iCycle = 11;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 15: 
                    MoveMotr(mi.PCK_Z, pv.PCK_ZPlce);
                    Step.iCycle++;
                    return false;

                case 16: 
                    if(!GetStop(mi.PCK_Z))return false;
                    if (DM.ARAY[(int)ri.PCK].GetCntStat(cs.Good) != 0)
                    {
                        MoveMotr(mi.PCK_Y, pv.PCK_YPick);
                        Step.iCycle++;
                        return false;
                    }
                    Step.iCycle = 20;
                    return false;

                case 17:
                    if (!GetStop(mi.PCK_Y)) return false;
                    MoveMotr(mi.PCK_Z, GetMotrPos(mi.PCK_Z, pv.PCK_ZPick) - 1);
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!GetStop(mi.PCK_Z)) return false;
                    SetY(yi.PCK_Vcc1, false); SetY(yi.PCK_Ejt1, true);
                    SetY(yi.PCK_Vcc2, false); SetY(yi.PCK_Ejt2, true);
                    SetY(yi.PCK_Vcc3, false); SetY(yi.PCK_Ejt3, true);
                    SetY(yi.PCK_Vcc4, false); SetY(yi.PCK_Ejt4, true);
                    SetY(yi.PCK_Vcc5, false); SetY(yi.PCK_Ejt5, true);
                    SetY(yi.PCK_Vcc6, false); SetY(yi.PCK_Ejt6, true);
                    SetY(yi.PCK_Vcc7, false); SetY(yi.PCK_Ejt7, true);
                    SetY(yi.PCK_Vcc8, false); SetY(yi.PCK_Ejt8, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 19:
                    if(!m_tmDelay.OnDelay(OM.DevOptn.iPlceWaitTime))return false;

                    SetY(yi.PCK_Ejt1, false);
                    SetY(yi.PCK_Ejt2, false);
                    SetY(yi.PCK_Ejt3, false);
                    SetY(yi.PCK_Ejt4, false);
                    SetY(yi.PCK_Ejt5, false);
                    SetY(yi.PCK_Ejt6, false);
                    SetY(yi.PCK_Ejt7, false);
                    SetY(yi.PCK_Ejt8, false);

                    MoveMotr(mi.PCK_Z, pv.PCK_ZMove);
                    Step.iCycle++;
                    return false;

                case 20:
                    if(!GetStop(mi.PCK_Z))return false;
                    FindChip(ref c, ref r, cs.Working, ri.WRK);
                    
                    for(int i = 0; i < OM.DevInfo.iVsnInspRowCnt; i++)
                    {
                        if(!OM.MstOptn.bIdleRun) DM.ARAY[(int)ri.WRK].SetStat(c, i, DM.ARAY[(int)ri.PCK].GetStat(0, i));
                        else                     DM.ARAY[(int)ri.WRK].SetStat(c, i, cs.Unkwn);
                    }

                    DM.ARAY[(int)ri.PCK].SetStat(cs.None);
                    Step.iCycle = 0;
                    return true;
            }
        }


        public void SetTrgPos()
        {
            double dSttPos = GetMotrPos(mi.PCK_Y, pv.PCK_YVisn);
            
            double[] dTrgPos = new double[iPickerCnt];

            for (int i = 0; i < iPickerCnt; i++)
            {
                dTrgPos[i] = dSttPos - (i * OM.DevInfo.dTrayColPitch) + OM.MstOptn.dTrgOfs;
            }

            SM.MT.SetTrgPos((int)mi.PCK_Y, dTrgPos, 1000, true, true);
            //MT_SetAxtTrgPos(miWRK_XVsn, iTrgCnt, dTrgPos, 1000, true, true);

            //delete[] dTrgPos;
        }

        public bool CheckSafe(mi _iMotr, double _dPos)
        {
            //if (!SM.MT.CmprPos((int)_iMotr, _dPos)) return false;

            bool bRet = true;
            string sMsg = "";

            if (_iMotr == mi.PCK_Y)
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
            else if (_iMotr == mi.PCK_Z)
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

        public bool CheckSafe(ai _iActr, EN_CYLINDER_POS _bFwd)
        {
            if (SM.CL.Complete((int)_iActr, _bFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            //if (_iActr == ai.LDR_GripClOp)
            //{
            //    //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
            //    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = AnsiString("Tray 센서 감지중!"); bRet = false ;}
            //    //}
            //}
            //else if (_iActr == ai.LDR_GripDnUp)
            //{

            //}
            //else if (_iActr == ai.LDR_GripRtLt)
            //{

            //}
            //else
            //{
                sMsg = "Cylinder " + SM.CL.GetName((int)_iActr) + " is Not this parts.";
                bRet = false;
            //}


            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(SM.CL.GetName((int)_iActr), sMsg);
                if (Step.iCycle == 0) Log.ShowMessage(SM.CL.GetName((int)_iActr), sMsg);
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
            
        public bool MoveActr(ai _iActr, EN_CYLINDER_POS _bFwd)
        {
            if (!CheckSafe(_iActr, _bFwd)) return false;

            SM.CL.Move((int)_iActr, _bFwd);
            
            return true;
        }

        public bool MoveActr(ai _iActr, fb _bFwd)
        {
            EN_CYLINDER_POS m_bFwd = (EN_CYLINDER_POS)_bFwd;
            if (!CheckSafe(_iActr, m_bFwd)) return false;

            SM.CL.Move((int)_iActr, m_bFwd);

            return true;
        }

        public bool CheckStop()
        {
            if (SM.MT.GetStop((int)mi.PCK_Y)) return false;
            if (SM.MT.GetStop((int)mi.PCK_Z)) return false;

            return true;
        }

        string sVsnData = "";
        StreamReader sr = null;
        
        
    };
    


    

   
    
}
