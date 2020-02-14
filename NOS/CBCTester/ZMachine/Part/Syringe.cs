using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class Syringe : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
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
            Suck       ,
            Supply     ,
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

        public Syringe(int _iPartId = 0)
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
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );
                    Step.iToStart++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove)) return false ;
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgClean);
                    Step.iToStart++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgClean)) return false ;

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
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );
                    Step.iToStop++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove)) return false ;
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgClean);
                    Step.iToStop++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgClean)) return false ;

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
                //bool bNeedFillTank    = IO_GetX(xi.CP_1NotFull) || IO_GetX(xi.CP_2NotFull) || IO_GetX(xi.CP_3NotFull) || 
                //                        IO_GetX(xi.CS_FNotFull) || IO_GetX(xi.CS_RNotFull) || IO_GetX(xi.SULFNotFull) || 
                //                        IO_GetX(xi.FBNotFull  ) || IO_GetX(xi.FDLNotFull ) || IO_GetX(xi.RETNotFull ) ||
                //                        IO_GetX(xi.NRNotFull  ) ;

                //사이클     DM.ARAY[ri.SUT].CheckAllStat(cs.Work )가 cs.Barcode일때 하고 싶지만 미리 썩 하면 별로 안좋을듯.
                bool isSuck    =  DM.ARAY[ri.SUT].CheckAllStat(cs.Work ) && DM.ARAY[ri.SYR].CheckAllStat(cs.None ) && (DM.ARAY[ri.CHA].CheckAllStat(cs.None) || DM.ARAY[ri.CHA].CheckAllStat(cs.Ready));
                bool isSupply  =  DM.ARAY[ri.SYR].CheckAllStat(cs.Work ) && DM.ARAY[ri.CHA].IsExist     (cs.Ready ) ; //&& SEQ.CHA.GetSeqStep() == 0  ; //이게 
                //bool isClean   =  DM.ARAY[ri.CHA].CheckAllStat(cs.None ) && DM.ARAY[ri.SYR].CheckAllStat(cs.Waste) ;
                bool isClean   =  DM.ARAY[ri.SYR].CheckAllStat(cs.Clean) ;
                bool isWorkEnd =  DM.ARAY[ri.SUT].CheckAllStat(cs.None ) && DM.ARAY[ri.SYR].CheckAllStat(cs.None )&& DM.ARAY[ri.CHA].IsExist     (cs.None);

                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.
                     if (isClean  ) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.Clean  ; }
                else if (isSupply ) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.Supply ; }
                else if (isSuck   ) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.Suck   ; }
                else if (isWorkEnd) { Stat.bWorkEnd = true; return true; }             
                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle){
                    Trace(Step.eSeq.ToString() +" Start");
                    Log.TraceListView(Step.eSeq.ToString() + " 동작 시작");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }
            
            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default         : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case (sc.Idle  ): return false;
                case (sc.Suck  ): if (!CycleSuck  ()) return false; break;
                case (sc.Supply): if (!CycleSupply()) return false; break;
                case (sc.Clean ): if (!CycleClean ()) return false; break;


            }
            Trace(sCycle+" End");
            Log.TraceListView(sCycle + " 동작 종료");
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
            string sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 10000 )) {
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
                    MT_SetServo(mi.SYR_XSyrg, true);
                    MT_SetServo(mi.SYR_ZSyrg, true);
                    MT_GoHome(mi.SYR_ZSyrg);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!MT_GetHomeDone(mi.SYR_ZSyrg))return false ;
                    MT_GoHome(mi.SYR_XSyrg);
                    SEQ.CHA.MoveCyl(ci.CHA_MixCoverOpCl , fb.Fwd);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetHomeDone(mi.SYR_XSyrg))return false ;
                    if(!CL_Complete(ci.CHA_MixCoverOpCl , fb.Fwd))return false ;
                    MT_GoAbsMan(mi.SYR_XSyrg, pv.SYR_XSyrgClean);
                    Step.iHome++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.SYR_XSyrg, pv.SYR_XSyrgClean)) return false ;


                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CycleSuck()
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
                    return true;

                case 10:
                    //LotOpen
                    OM.EqpStat.sBloodID = DM.ARAY[ri.SUT].ID ;

                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );

                    //UPH측정용.=> 국가과제 검수용.
                    if(OM.CmnOptn.bNotUseInsp)
                    {
                        Step.iCycle = 50;
                        return false ;
                    }

                    
                    
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;

                    //기포 없애기=============================================================================
                    MoveMotr(mi.SYR_XSyrg, pv.SYR_XSyrgClean);
                    SEQ.PKR.MoveMotr(mi.PKR_YSutl , pv.PKR_YSutlSyinge );

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.SYR_XSyrg, pv.SYR_XSyrgClean)) return false;
                    if(!MT_GetStopPos(mi.PKR_YSutl , pv.PKR_YSutlSyinge))return false ;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(500)) return false;
                    MoveMotr(mi.SYR_ZSyrg, pv.SYR_ZSyrgChamber);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.SYR_ZSyrg, pv.SYR_ZSyrgChamber)) return false;

                    SEQ.CHA.WastClean(true);
                    SEQ.SyringePump.AbsMove((int)PumpID.Blood, VALVE_POS.Output, 0, 15);
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 15:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false;

                    //약간 남은 피 없애기 위해 조금더(100) 배출.
                    SEQ.SyringePump.PickupAndDispInc((int)PumpID.Blood, VALVE_POS.Input, VALVE_POS.Output, OM.DevInfo.iBloodPreCutVol*2, 10);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false;
                    SEQ.CHA.WastClean(false);

                    Step.iCycle++;
                    return false;

                case 17:
                    MoveMotrSlow(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove);

                    Step.iCycle++;
                    return false;

                case 18:
                    if (!MT_GetStopPos(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove)) return false;

                    //피빨기================================================================================
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgSuttle   );
                    
                    Step.iCycle++;
                    return false ;

                case 19:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgSuttle))return false ;
                    
                    //MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgSuttleLid );
                    Step.iCycle++;
                    return false ;

                case 20:
                    //if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgSuttleLid))return false ;
                    MoveMotrSlow(mi.SYR_ZSyrg , pv.SYR_ZSyrgSuttle );
                    Step.iCycle++;
                    return false ;

                case 21:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgSuttle))return false ;
                    int iVol = OM.DevInfo.iCmb1BloodVol + 
                               OM.DevInfo.iCmb2BloodVol + 
                               (OM.CmnOptn.bNotUseFB   ? 0 : OM.DevInfo.iCmb3BloodVol) +
                               (OM.CmnOptn.bNotUse4DLS ? 0 : OM.DevInfo.iCmb4BloodVol) + 
                               (OM.CmnOptn.bNotUseRet  ? 0 : OM.DevInfo.iCmb5BloodVol) + 
                               (OM.CmnOptn.bNotUseNr   ? 0 : OM.DevInfo.iCmb6BloodVol) + 
                               OM.DevInfo.iBloodPreCutVol;
                    iVol += (int)(iVol * 0.1) ; //10프로마진.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.Blood , VALVE_POS.Output, iVol , 10);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 22:
                    //펌프 다 빨기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.Blood ))return false ;
                    SEQ.CHA.NiddleClean(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 23:
                    if (!m_tmDelay.OnDelay(3000)) return false;
                    MoveMotrSlow(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 24:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    SEQ.CHA.NiddleClean(false);
                    //위의 함수안에서 핸들링 하나 빠는쪽은 조금더 켜놓고 싶음.                    
                    IO_SetY(yi.NidlOut_OtSt, true); //자꾸 밖으로 물이 튀어 나옴.

                    //셔틀은 필요 없으니 먼져 보냄.
                    DM.ARAY[ri.SUT].SetStat(cs.WorkEnd);
                    

                    //혈액 프리컷=========================================================================
                    MoveMotr(mi.SYR_XSyrg, pv.SYR_XSyrgClean);

                    Step.iCycle++;
                    return false;

                case 25:
                    if (!MT_GetStopPos(mi.SYR_XSyrg, pv.SYR_XSyrgClean)) return false;

                    IO_SetY(yi.NidlOut_OtSt, false); //자꾸 밖으로 물이 튀어 나옴.
                    //SEQ.CHA.NiddleClean(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 26:
                    if (!m_tmDelay.OnDelay(500)) return false;
                    MoveMotr(mi.SYR_ZSyrg, pv.SYR_ZSyrgChamber);
                    Step.iCycle++;
                    return false;

                case 27:
                    if (!MT_GetStopPos(mi.SYR_ZSyrg, pv.SYR_ZSyrgChamber)) return false;
                    //SEQ.CHA.NiddleClean(false);
                    SEQ.CHA.WastClean(true);
                    SEQ.SyringePump.DispIncPos((int)PumpID.Blood, VALVE_POS.Output, OM.DevInfo.iBloodPreCutVol, 10);
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 28:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false;
                    SEQ.CHA.WastClean(false);
                    //SEQ.CHA.NiddleClean(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 29:
                    if (!m_tmDelay.OnDelay(500)) return false;
                    MoveMotrSlow(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false;

                case 30:
                    if (!MT_GetStopPos(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove)) return false;
                    //SEQ.CHA.NiddleClean(false);
                    Step.iCycle++;
                    return false ;

                case 31:
                    //if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    DM.ARAY[ri.SYR].SetStat(cs.Work   );
                    Step.iCycle= 0 ;
                    return true ;

                //UPH용.
                //===================================================================================================================
                case 50:
                    MoveMotrSlow(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false;

                case 51:
                    if (!MT_GetStopPos(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove)) return false;
                    //피빨기================================================================================
                    MoveMotr(mi.SYR_XSyrg, pv.SYR_XSyrgSuttle);
                    SEQ.PKR.MoveMotr(mi.PKR_YSutl, pv.PKR_YSutlSyinge);

                    Step.iCycle++;
                    return false;

                case 52:
                    if (!MT_GetStopPos(mi.SYR_XSyrg, pv.SYR_XSyrgSuttle))return false ;
                    if (!MT_GetStopPos(mi.PKR_YSutl, pv.PKR_YSutlSyinge)) return false;
                    //MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgSuttleLid );
                    Step.iCycle++;
                    return false ;

                case 53:
                    //if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgSuttleLid))return false ;
                    MoveMotrSlow(mi.SYR_ZSyrg , pv.SYR_ZSyrgSuttle );
                    Step.iCycle++;
                    return false ;

                case 54:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgSuttle))return false ;
                    iVol = OM.DevInfo.iCmb1BloodVol + 
                           OM.DevInfo.iCmb2BloodVol + 
                           (OM.CmnOptn.bNotUseFB   ? 0 : OM.DevInfo.iCmb3BloodVol) +
                           (OM.CmnOptn.bNotUse4DLS ? 0 : OM.DevInfo.iCmb4BloodVol) + 
                           (OM.CmnOptn.bNotUseRet  ? 0 : OM.DevInfo.iCmb5BloodVol) + 
                           (OM.CmnOptn.bNotUseNr   ? 0 : OM.DevInfo.iCmb6BloodVol) + 
                           OM.DevInfo.iBloodPreCutVol;
                    SEQ.SyringePump.PickupIncPos((int)PumpID.Blood , VALVE_POS.Output, iVol , 10);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 55:
                    //펌프 다 빨기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.Blood ))return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 56:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    MoveMotrSlow(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 57:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    //위의 함수안에서 핸들링 하나 빠는쪽은 조금더 켜놓고 싶음.                    
                    //셔틀은 필요 없으니 먼져 보냄.
                    DM.ARAY[ri.SUT].SetStat(cs.WorkEnd);
                    DM.ARAY[ri.SYR].SetStat(cs.Work   );
                    Step.iCycle=0;
                    return true ;

                    
                    
            }
        }

        int iWorkCol = 0;
        int iBloodVol = 0 ;
        public bool CycleSupply()
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
                    return true;

                case 10:
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    SEQ.CHA.MoveCyl(ci.CHA_MixCoverOpCl , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.CHA_MixCoverOpCl , fb.Fwd)) return false ;
                    
                    //iWorkCol = DM.ARAY[ri.CHA].FindFrstCol(cs.Ready);

                    //* 타이밍 차트상으로 혈액 투입 순서는 2,1,4,5,6,3
                    //iWorkCol = DM.ARAY[ri.CHA].FindFrstCol(cs.None);
                         if(DM.ARAY[ri.CHA].GetStat(1,0) == cs.Ready)  {iWorkCol = 1 ; iBloodVol = OM.DevInfo.iCmb2BloodVol;}
                    else if(DM.ARAY[ri.CHA].GetStat(0,0) == cs.Ready)  {iWorkCol = 0 ; iBloodVol = OM.DevInfo.iCmb1BloodVol;}
                    else if(DM.ARAY[ri.CHA].GetStat(3,0) == cs.Ready)  {iWorkCol = 3 ; iBloodVol = OM.DevInfo.iCmb4BloodVol;}
                    else if(DM.ARAY[ri.CHA].GetStat(4,0) == cs.Ready)  {iWorkCol = 4 ; iBloodVol = OM.DevInfo.iCmb5BloodVol;}
                    else if(DM.ARAY[ri.CHA].GetStat(5,0) == cs.Ready)  {iWorkCol = 5 ; iBloodVol = OM.DevInfo.iCmb6BloodVol;}
                    else if(DM.ARAY[ri.CHA].GetStat(2,0) == cs.Ready)  {iWorkCol = 2 ; iBloodVol = OM.DevInfo.iCmb3BloodVol;}

                    if (OM.CmnOptn.bNotUseFB && iWorkCol == 2)
                    {
                        //임시로 주석 모션만 체크
                        DM.ARAY[ri.CHA].SetStat(iWorkCol, 0, cs.Fill);

                        //마지막 챔버일시에 실린지 clean 
                        if (DM.ARAY[ri.CHA].CheckAllStat(cs.Fill))
                        {
                            DM.ARAY[ri.SYR].SetStat(cs.Clean);
                            //SEQ.CHA.Stat.bRqstFill = true;
                        }
                        Step.iCycle = 0;
                        return true;
                    }
                    if (OM.CmnOptn.bNotUse4DLS && iWorkCol == 3)
                    {
                        //임시로 주석 모션만 체크
                        DM.ARAY[ri.CHA].SetStat(iWorkCol, 0, cs.Fill);

                        //마지막 챔버일시에 실린지 clean 
                        if (DM.ARAY[ri.CHA].CheckAllStat(cs.Fill))
                        {
                            DM.ARAY[ri.SYR].SetStat(cs.Clean);
                            //SEQ.CHA.Stat.bRqstFill = true;
                        }
                        Step.iCycle = 0;
                        return true;
                    }

                    if (OM.CmnOptn.bNotUseRet&& iWorkCol == 4)
                    {
                        //임시로 주석 모션만 체크
                        DM.ARAY[ri.CHA].SetStat(iWorkCol, 0, cs.Fill);

                        //마지막 챔버일시에 실린지 clean 
                        if (DM.ARAY[ri.CHA].CheckAllStat(cs.Fill))
                        {
                            DM.ARAY[ri.SYR].SetStat(cs.Clean);
                            //SEQ.CHA.Stat.bRqstFill = true;
                        }
                        Step.iCycle=0;
                        return true;
                    }
                    if(OM.CmnOptn.bNotUseNr && iWorkCol == 5)
                    {
                        //임시로 주석 모션만 체크
                        DM.ARAY[ri.CHA].SetStat(iWorkCol, 0, cs.Fill);

                        //마지막 챔버일시에 실린지 clean 
                        if (DM.ARAY[ri.CHA].CheckAllStat(cs.Fill))
                        {
                            DM.ARAY[ri.SYR].SetStat(cs.Clean);
                            //SEQ.CHA.Stat.bRqstFill = true;
                        }
                        Step.iCycle=0;
                        return true;
                    }
                        
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgChamberStt , PM.GetValue(mi.SYR_XSyrg , pv.SYR_XSyrgChamberPitch) * iWorkCol  );
                    //SEQ.CHA.NiddleClean(true);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgChamberStt , PM.GetValue(mi.SYR_XSyrg , pv.SYR_XSyrgChamberPitch) * iWorkCol ))return false ;
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamber );

                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamber))return false ;
                    //SEQ.CHA.NiddleClean(false);
                    SEQ.SyringePump.DispIncPos((int)PumpID.Blood , VALVE_POS.Output , iBloodVol , OM.DevInfo.iBloodChamberSpd);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 15:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(200)) return false;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false ;


                    //SEQ.CHA.NiddleClean(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(1000)) return false;


                    MoveMotrSlow(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;

                    //SEQ.CHA.NiddleClean(false);

                    //임시로 주석 모션만 체크
                    DM.ARAY[ri.CHA].SetStat(iWorkCol , 0 , cs.Fill);
                    
                    //마지막 챔버일시에 실린지 clean 
                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.Fill))
                    {
                        DM.ARAY[ri.SYR].SetStat(cs.Clean); //시점이 썩하기 전에 하는게 좋아서 옮김.
                        //SEQ.CHA.Stat.bRqstFill = true;
                    }
                    
                    

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CycleClean()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                SEQ.CHA.WastClean(false);
                SEQ.CHA.NiddleClean(false);
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
                    SEQ.CHA.WastClean(false);
                    SEQ.CHA.NiddleClean(false);
                    return true;

                case 10:
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgClean );
                    
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgClean ))return false ;

                    SEQ.CHA.NiddleClean(true);
                    m_tmDelay.Clear();
                    
                    

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!m_tmDelay.OnDelay(1000))return false  ;
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamber );
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamber))return false ;
                    SEQ.CHA.NiddleClean(false);


                    SEQ.CHA.WastClean(true);
                    SEQ.SyringePump.AbsMove((int)PumpID.Blood , VALVE_POS.Output , 0 , 15);
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 15:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false ;


                    //약간 남은 피 없애기 위해 조금더(100) 배출.
                    int iVol = OM.DevInfo.iCmb1BloodVol + OM.DevInfo.iCmb2BloodVol + OM.DevInfo.iCmb3BloodVol + OM.DevInfo.iCmb4BloodVol + OM.DevInfo.iCmb5BloodVol + OM.DevInfo.iCmb6BloodVol + OM.DevInfo.iBloodPreCutVol;
                    iVol = OM.CmnOptn.bNotUseInsp ? 500 : 3000 ;//(int)(iVol * 0.3); //20프로마진.
                    SEQ.SyringePump.PickupAndDispInc((int)PumpID.Blood, VALVE_POS.Input , VALVE_POS.Output, iVol, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false;

                    SEQ.CHA.NiddleClean(true);
                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    //if(!m_tmDelay.OnDelay(500))return false ;
                    MoveMotrSlow(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    SEQ.CHA.NiddleClean(false);
                    SEQ.CHA.WastClean(false);

                    //위의 두개 함수안에서 핸들링 하나 빠는쪽은 조금더 켜놓고 싶음.
                    IO_SetY(yi.NidlOut_OtSt, true); //자꾸 밖으로 물이 튀어 나옴.
                    IO_SetY(yi.ClenOut_OtSt, true); //자꾸 밖으로 물이 튀어 나옴.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 19:
                    if (!m_tmDelay.OnDelay(2000)) return false;

                    IO_SetY(yi.NidlOut_OtSt, false);
                    IO_SetY(yi.ClenOut_OtSt, false); //자꾸 밖으로 물이 튀어 나옴.

                    DM.ARAY[ri.SYR].SetStat(cs.None);

                    //임시로 오토런을 위해 함.
                    //DM.ARAY[ri.CHA].SetStat(cs.None);

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        //CP2 채우는 동작.
        int iRepeatCnt = 0;
        public bool CycleManReadyBlood()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                IO_SetY(yi.ClenOut_OtSt, false);
                SEQ.CHA.NiddleClean(false);
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
                    IO_SetY(yi.ClenOut_OtSt, false);
                    SEQ.CHA.NiddleClean(false);
                    return true;

                case 10:
                    iRepeatCnt = 0;
                    MoveMotr(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove)) return false;
                    MoveMotr(mi.SYR_XSyrg, pv.SYR_XSyrgClean);
                    

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.SYR_XSyrg, pv.SYR_XSyrgClean)) return false;
                    SEQ.CHA.NiddleClean(true);
                    MoveMotr(mi.SYR_ZSyrg, pv.SYR_ZSyrgChamber);
                    //sun 펌프로 밀기.
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.SYR_ZSyrg, pv.SYR_ZSyrgChamber)) return false;
                    SEQ.CHA.NiddleClean(false);
                    IO_SetY(yi.ClenOut_OtSt, true);
                    SEQ.SyringePump.AbsMove((int)PumpID.Blood, VALVE_POS.Output, 0, 15);

                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

               
                //밑에서씀.
                case 14:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false;

                    //3000까지 빨기.
                    IO_SetY(yi.ClenOut_OtSt, false);
                    SEQ.SyringePump.AbsMove((int)PumpID.Blood, VALVE_POS.Input, 3000, 15);
                    Step.iCycle++;
                    m_tmDelay.Clear();
                    return false;

                case 15:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false;
                    IO_SetY(yi.ClenOut_OtSt, true);
                    SEQ.SyringePump.AbsMove((int)PumpID.Blood, VALVE_POS.Output, 0, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false;
                    IO_SetY(yi.ClenOut_OtSt, false);
                    if (iRepeatCnt < OM.DevInfo.iBloodReadyCnt-1)
                    {
                        iRepeatCnt++;
                        Step.iCycle=14;
                        return false;
                    }


                    MoveMotrSlow(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove);
                    SEQ.CHA.NiddleClean(true);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!MT_GetStopPos(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove)) return false;

                    SEQ.CHA.NiddleClean(false);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            //if (_eActr == ci.SYR_MixCoverClOp)
            //{
            //
            //}
            //else if (_eActr == ci.PKR_PkrFtRr)
            //{
            //
            //}
            //else if (!_bChecked)
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

            if (_eMotr == mi.SYR_XSyrg)
            {
                if(MT_GetCmdPos(mi.SYR_ZSyrg) > PM.GetValue(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove)+1.0)
                {
                    sMsg = "Motor " + MT_GetName(mi.SYR_ZSyrg) + "가 이동 포지션이 아닙니다.";
                    bRet = false;

                }
            }
            else if(_eMotr == mi.SYR_ZSyrg)
            {
               


                //if(MT_GetCmdPos(mi.SYR_XSyrg) > PM.GetValue(mi.SYR_XSyrg , pv.SYR_XSyrgClean)+1.0)
                //{
                //    sMsg = "Motor " + MT_GetName(mi.SYR_ZSyrg) + "가 이동 포지션이 아닙니다.";
                //    bRet = false;
                //
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

            if (_eMotr == mi.SYR_XSyrg)
            {

                    if (MT_GetCmdPos(mi.SYR_ZSyrg) > PM.GetValue(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove) + 1.0)
                    {
                        sMsg = "Motor " + MT_GetName(mi.SYR_ZSyrg) + "가 이동 포지션이 아닙니다.";
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
            else if (_eMotr == mi.SYR_ZSyrg)
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
            if( !MT_GetStop(mi.SYR_XSyrg)) return false;
            if( !MT_GetStop(mi.SYR_ZSyrg)) return false;
            
            //if (!CL_Complete(ci.PKR_PkrShakeUpDn)) return false;
            //if (!CL_Complete(ci.PKR_PkrRrFt     )) return false;
            //if (!CL_Complete(ci.PKR_PkrClampClOp)) return false;

            if(m_tmDelay.GetUsing())return false ;

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
