using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON;
using SMDll2;

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

        public enum EN_SEQ_CYCLE
        {
            Idle   = 0,
            Supply = 1,
            Work   = 2,
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


        public Loader()
        {
            m_sPartName = "Loader";

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
                    MoveActr(ai.LDR_TrayFixClOp, fb.Bwd);
                    //iTrayWorkCnt = 0;
                    Step.iToStop++;
                    return false;
                
                case 11:
                    if (!MoveActr(ai.LDR_TrayFixClOp, fb.Bwd)) return false;
                    iTrayWorkCnt = 0;
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

                bool bCycleSupply = !GetX(xi.LDR_TrayPstn) && DM.ARAY[(int)ri.LDR].CheckAllStat(cs.None) && GetX(xi.LDR_TrayDetect);
                bool bCycleWork   =  GetX(xi.LDR_TrayPstn) && DM.ARAY[(int)ri.PRI].CheckAllStat(cs.None) && 
                                    !GetX(xi.IDX_Pri)      &&!DM.ARAY[(int)ri.LDR].CheckAllStat(cs.None) && 
                                                              DM.ARAY[(int)ri.LDR].GetCntStat(cs.Unkwn) > 0;

                bool isCycleEnd = DM.ARAY[(int)ri.LDR].CheckAllStat(cs.None) && !GetX(xi.LDR_TrayDetect);


                //여기부터 조건 잡자.


                if (SM.ER.IsErr())
                {
                    MoveActr(ai.LDR_TrayFixClOp, fb.Bwd);
                    return false;
                } 
                    
                //Normal Decide Step.
                     if (bCycleSupply) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Supply ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (bCycleWork  ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Work   ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); } 
                else if (isCycleEnd  ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;
            }

            //Cycle.
            Step.iLastSeq = Step.iSeq;
            switch (Step.iSeq)
            {
                default                   :                       Log.Trace(m_sPartName, "default End");                                    Step.iSeq = EN_SEQ_CYCLE.Idle;   return false;
                case (EN_SEQ_CYCLE.Idle)  :                                                                                                                                    return false;
                case (EN_SEQ_CYCLE.Supply): if (CycleSupply ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Work  ): if (CycleWork   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                
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
            m_sCycleName[(int)EN_SEQ_CYCLE.Idle     ]="Idle"        ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Supply   ]="Supply"      ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Work     ]="Work  "      ;
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

        public static int iTrayWorkCnt = 0;
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
                    SM.CL.Move((int)ai.LDR_GripDnUp, EN_CYLINDER_POS.cpBwd);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!SM.CL.Move((int)ai.LDR_GripDnUp, EN_CYLINDER_POS.cpBwd))return false;
                    SM.CL.Move((int)ai.LDR_GripRtLt, EN_CYLINDER_POS.cpBwd);
                    Step.iHome++;
                    return false;

                case 12: 
                    if(!SM.CL.Move((int)ai.LDR_GripRtLt, EN_CYLINDER_POS.cpBwd)) return false;
                    SM.CL.Move((int)ai.LDR_TrayFixClOp, EN_CYLINDER_POS.cpBwd);
                    Step.iHome++;
                    return false;
                //로더에 실린더 하나 추가되면서 추가함 진섭.
                case 13:
                    if (!SM.CL.Move((int)ai.LDR_TrayFixClOp, EN_CYLINDER_POS.cpBwd)) return false;
                    SM.MT.GoHome((int)mi.LDR_Z);
                    Step.iHome++;
                    return false ;
            
                case 14:
                    if (!SM.MT.GetHomeDone((int)mi.LDR_Z)) return false;
                    iTrayWorkCnt = 0;
                    Step.iHome = 0;
                    return true ;
            }
        }

        //모터 포지션 확인후 ERR띄우기.

        
        public static int iTrayImgNo = 0;
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

            const int iTrayMaxCnt = 20;
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
                        return false;
                    }
                    MoveActr(ai.LDR_GripClOp, fb.Bwd);
                    MoveActr(ai.LDR_TrayFixClOp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ai.LDR_GripClOp, fb.Bwd)) return false;
                    if (!GetActrStop(ai.LDR_TrayFixClOp, fb.Bwd)) return false;
                    MoveActr(ai.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                
                case 12:
                    if (!GetActrStop(ai.LDR_GripDnUp, fb.Bwd)) return false;
                    MoveActr(ai.LDR_GripRtLt, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if (!GetActrStop(ai.LDR_GripRtLt, fb.Bwd)) return false;
                    Step.iCycle++;
                    return false ;

                //아래서 씀.
                case 14:
                    //MoveMotr(mi.LDR_Z, pv.LDR_ZWorkStt);
                    MoveActr(ai.LDR_TrayFixClOp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MoveActr(ai.LDR_TrayFixClOp, fb.Bwd)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iLDRTrayCheckTime)) return false;
                    if (!GetStop(mi.LDR_Z)) return false;
                    if (GetX(xi.LDR_TrayOver))
                    {
                        SetErr(ei.PRT_TrayErr, "Tray Position Over. Remove Tray Plz.");
                        Step.iCycle = 0;
                        return false;
                    }
                    if(SM.MT.GetCmdPos((int)mi.LDR_Z) < (PM.GetValue(mi.LDR_Z, pv.LDR_ZWorkStt) + (OM.DevInfo.dTrayHeight * iTrayMaxCnt) - 0.5)){
                        dLDRZTrayPos = PM.GetValue(mi.LDR_Z, pv.LDR_ZWorkStt) + (OM.DevInfo.dTrayHeight * iTrayWorkCnt);
                        MoveMotr(mi.LDR_Z, dLDRZTrayPos);
                    }
                    else{
                        SetErr(ei.PRT_TrayErr, "Checked Loader Tray.");
                        MoveMotr((int)mi.LDR_Z, pv.LDR_ZWait);
                        iTrayWorkCnt = 0;
                        Step.iCycle = 0;
                        return false;
                    }
                    
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!GetStop(mi.LDR_Z))return false;
                    //MoveActr(ai.LDR_TrayFixClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 18:
                    //if (!MoveActr(ai.LDR_TrayFixClOp, fb.Fwd)) return false;
                    if(!GetX(xi.LDR_TrayPstn)){
                        iTrayWorkCnt++;
                        m_tmDelay.Clear();
                        Step.iCycle = 14;
                        return false;
                    }

                    if (GetX(xi.LDR_TrayDir)) iTrayImgNo = 1;
                    else                      iTrayImgNo = 0;

                    //ARAY Data 입력.
                    DM.ARAY[(int)ri.LDR].ID      = iTrayWorkCnt.ToString() ;
                    DM.ARAY[(int)ri.LDR].LotNo   = LOT.m_sLotNo;
                    DM.ARAY[(int)ri.LDR].Step    = 0;
                    DM.ARAY[(int)ri.LDR].SubStep = iTrayImgNo;

                    DM.ARAY[(int)ri.LDR].SetStat(cs.Unkwn);
                    Step.iCycle = 0;
                    return true;
                //    SM.MT.GoIncVel((int)mi.LDR_Z, 10, 10);
                //    Step.iCycle++;
                //    return false;

                
                //case 15:
                //    if(!GetX(xi.LDR_TrayPstn)){
                //        Step.iCycle = 14;
                //        return false;
                //    }
                //    SM.MT.Stop((int)mi.LDR_Z);
                //    Step.iCycle++;
                //    return false;

                //case 16:
                //    if(!GetStop(mi.LDR_Z))return false;
                //    Step.iCycle++;
                //    return false ;

                ////아래서 씀.
                //case 17:
                //    SM.MT.GoIncVel((int)mi.LDR_Z, -1, 5);
                //    Step.iCycle++;
                //    return false ;

                //case 18:
                //    if(GetX(xi.LDR_TrayPstn)){
                //        Step.iCycle = 17;
                //        return false ;
                //    }
                //    SM.MT.Stop((int)mi.LDR_Z);
                //    Step.iCycle++;
                //    return false;

                //case 19:
                //    if (!GetStop(mi.LDR_Z)) return false;
                //    Step.iCycle++;
                //    return false;

                //case 20:
                //    //위에 까지만 쓸수도 있다.
                //    SM.MT.GoIncVel((int)mi.LDR_Z, 1, 1);
                //    Step.iCycle++;
                //    return false ;

                //case 21:
                //    if(!GetX(xi.LDR_TrayPstn)){
                //        Step.iCycle = 20;
                //        return false;
                //    }
                //    SM.MT.Stop((int)mi.LDR_Z);
                //    Step.iCycle++;
                //    return false;

                //case 22:
                //    if (!GetStop(mi.LDR_Z)) return false;
                //    DM.ARAY[(int)ri.LDR].SetStat(cs.Unkwn);
                //    Step.iCycle = 0;
                //    return true ;
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

            switch (Step.iCycle)
            {

                default: sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                         Step.iCycle = 0;
                         return true;

                case 0:
                         return false;


                case 10:
                    MoveActr(ai.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ai.LDR_GripDnUp, fb.Bwd)) return false;
                    MoveActr(ai.LDR_GripRtLt, fb.Bwd);
                    MoveActr(ai.LDR_GripClOp, fb.Bwd);
                    MoveActr(ai.LDR_TrayFixClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!GetActrStop(ai.LDR_GripRtLt, fb.Bwd)) return false;
                    if (!GetActrStop(ai.LDR_GripClOp, fb.Bwd)) return false;
                    if (!GetActrStop(ai.LDR_TrayFixClOp, fb.Fwd)) return false;
                    MoveActr(ai.LDR_GripDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetActrStop(ai.LDR_GripDnUp, fb.Fwd)) return false;
                    MoveActr(ai.LDR_GripClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!GetActrStop(ai.LDR_GripClOp, fb.Fwd)) return false;
                    MoveActr(ai.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!GetActrStop(ai.LDR_GripDnUp, fb.Bwd)) return false;
                    MoveActr(ai.LDR_GripRtLt, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!GetActrStop(ai.LDR_GripRtLt, fb.Fwd)) return false;
                    MoveActr(ai.LDR_GripDnUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!GetActrStop(ai.LDR_GripDnUp, fb.Fwd)) return false;
                    MoveActr(ai.LDR_GripClOp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!GetActrStop(ai.LDR_GripClOp, fb.Bwd)) return false;
                    MoveActr(ai.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!GetActrStop(ai.LDR_GripDnUp, fb.Bwd)) return false;
                    MoveActr(ai.LDR_GripRtLt, fb.Bwd);
                    if (!GetX(xi.IDX_Pri))
                    {
                        SetErr(ei.PRT_Missed, "Tray Supply Miss");
                        Step.iCycle = 0;
                        return true;
                    }
                    
                    
                    DM.ARAY[(int)ri.PRI].SetStat(cs.Unkwn);

                    DM.ARAY[(int)ri.PRI].ID      = DM.ARAY[(int)ri.LDR].ID     ;
                    DM.ARAY[(int)ri.PRI].LotNo   = DM.ARAY[(int)ri.LDR].LotNo  ;
                    DM.ARAY[(int)ri.PRI].Step    = DM.ARAY[(int)ri.LDR].Step   ;
                    DM.ARAY[(int)ri.PRI].SubStep = DM.ARAY[(int)ri.LDR].SubStep;

                    DM.ARAY[(int)ri.LDR].SetStat(cs.None);
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleTraySupply()
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

            switch (Step.iCycle)
            {

                default: sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iCycle);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;


                case 10:
                    MoveActr(ai.LDR_GripDnUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetActrStop(ai.LDR_GripDnUp, fb.Bwd)) return false;
                    MoveActr(ai.LDR_GripRtLt, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!GetActrStop(ai.LDR_GripRtLt, fb.Bwd)) return false;
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

            if(_iActr == ai.LDR_GripClOp){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = AnsiString("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_iActr == ai.LDR_GripDnUp)
            {

            }
            else if (_iActr == ai.LDR_GripRtLt)
            {

            }
            else if (_iActr == ai.LDR_TrayFixClOp)
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
            if (!SM.MT.GetStop((int)mi.LDR_Z        ))return false;

            if (!SM.CL.Complete((int)ai.LDR_GripClOp))return false;
            if (!SM.CL.Complete((int)ai.LDR_GripDnUp))return false;
            if (!SM.CL.Complete((int)ai.LDR_GripRtLt))return false;

            return true;
        }
    }
}
 