using System;
using COMMON;
using System.Runtime.CompilerServices;

namespace Machine
{
    public class TurnTable : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop;
            public void Clear()
            {
                bWorkEnd  = false ;
                bReqStop  = false ;
            }
        };   

        public enum sc
        {
            Idle    = 0,
            Move       ,
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

        public CTimer   m_TickTime ;
        public double   m_dTickTime;

        public CTimer[] m_CycleTime;

        public int r, c;

        //Barcode
        private bool bTriggerSensor ; //센서 확인용
        private int  iBarcodeErrCnt ; //횟수 초과시 에러용
        private bool bRet1          ; //결과 확인용 
        private bool bRet2          ; //결과 확인용 
        private bool bRet3          ; //바코드 품질 등급 확인용

        private int  _iWorkCount     ; //테이블 위치 확인 0~6
        public  int   iWorkCount
        {
            get
            {
                     if(_iWorkCount < 0) _iWorkCount = 0;
                //else if(_iWorkCount > 6) _iWorkCount = 6;
                
                return _iWorkCount;
            }
            set
            {
                _iWorkCount = value;
            }
        }     
        private int   iWorkCountTemp;

        public TurnTable(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();

            m_TickTime    = new CTimer();
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
                    //1.0.1.3
                    if(!OM.CmnOptn.bIgnrBarcode) SEQ.BarCodeReader.CycleReading(false); //LOFF

                    iBarcodeErrCnt = 0 ;

                    if (CL_Complete(ci.LODR_RngGrpFwBw, fb.Fwd) || CL_Complete(ci.VISN_TurnGrpFwBw, fb.Fwd))
                    {
                        MoveCyl(ci.TBLE_Grpr1FwBw, fb.Fwd);
                        MoveCyl(ci.TBLE_Grpr2FwBw, fb.Fwd);
                        MoveCyl(ci.TBLE_Grpr3FwBw, fb.Fwd);
                        MoveCyl(ci.TBLE_Grpr4FwBw, fb.Fwd);
                        MoveCyl(ci.TBLE_Grpr5FwBw, fb.Fwd);
                        MoveCyl(ci.TBLE_Grpr6FwBw, fb.Fwd);
                        Step.iToStart++;
                        return false;
                    }

                    Step.iToStart = 0;
                    return true;


                case 11:
                    if (!CL_Complete(ci.TBLE_Grpr1FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr2FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr3FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr4FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr5FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr6FwBw, fb.Fwd)) return false;
                    Step.iToStart++;
                    return false;

                case 12:
                    //if(!MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait , PM.GetValue(mi.TBLE_TTble ,  pv.TBLE_TTbleWorkPitch) * iWorkCount )) return false;
                    Step.iToStart++;
                    return false;

                case 13:
                    //if(!MT_GetStopPos(mi.TBLE_TTble, pv.TBLE_TTbleWait , PM.GetValue(mi.TBLE_TTble ,  pv.TBLE_TTbleWorkPitch) * iWorkCount)) return false;

                    //if (!CL_Complete(ci.IDXR_ClampUpDn, fb.Bwd)) return false;
                    //if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd)) return false ;
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
                    //1.0.1.3
                    if(!OM.CmnOptn.bIgnrBarcode) SEQ.BarCodeReader.CycleReading(false); //LOFF
                    
                    //iBarcodeErrCnt = 0 ;
                    //MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    Step.iToStop++;
                    return false;

                case 11: 
                    //if(!MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait , PM.GetValue(mi.TBLE_TTble ,  pv.TBLE_TTbleWorkPitch) * iWorkCount )) return false;
                    //if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd)) return false ;
                    //MoveMotr(mi.SSTG_YGrpr, pv.SSTG_YGrprWait);
                    
                    Step.iToStop++;
                    return false;

                case 12:
                    //if(!MT_GetStopPos(mi.TBLE_TTble, pv.TBLE_TTbleWait , PM.GetValue(mi.TBLE_TTble ,  pv.TBLE_TTbleWorkPitch) * iWorkCount)) return false;
                    ////혹시라도 처박으면..
                    //if (IO_GetX(xi.SSTG_GrprOverload))
                    //{
                    //    MT_EmgStop(mi.SSTG_YGrpr);
                    //    ER_SetErr(ei.PRT_OverLoad, "Substrate");
                    //    return true;
                    //}
                    //if(!MT_GetStop(mi.SSTG_YGrpr)) return false ;
                    ////오버로드 에러가 없거나 있어도 Substrate Stage께 아닌경우에만
                    //bool bOverLoad  = ER_GetErrOn(ei.PRT_OverLoad) && ER_GetErrSubMsg(ei.PRT_OverLoad)!="Substrate";
                    //bool bMissed    = ER_GetErrOn(ei.PRT_Missed  ) && ER_GetErrSubMsg(ei.PRT_Missed  )!="Substrate";
                    //bool bBarcodeNG = ER_GetErrOn(ei.PRT_Barcode ) && ER_GetErrSubMsg(ei.PRT_Barcode )!="Substrate";
                    //if(!bOverLoad && !bMissed && !bBarcodeNG){
                    //    MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevWait);
                    //}

                    Step.iToStop++;
                    return false;
                
                case 13: 
                    //if(!MT_GetStopPos(mi.SLDR_ZElev,pv.SLDR_ZElevWait)) return false ;
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

        //public bool GetTrayAllGood()
        //{
        //    for (int c = 0; c < OM.DevInfo.iTRAY_PcktCntX; c++)
        //    {
        //        for (int r = 0; r < OM.DevInfo.iTRAY_PcktCntY; r++)
        //        {
        //            if (DM.ARAY[ri.MASK].GetStat(c, r) == cs.None) continue ;
        //            if (DM.ARAY[ri.IDXR].GetStat(c, r) != cs.Good) return false ;
        //        }
        //    }
        //    return true ;
        //}

        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;              

                //if (IO_GetX(xi.IDXR_Overload))
                //{
                //    ER_SetErr(ei.PRT_OverLoad, "Rear Index OverLoad Error.");
                //}
                bool isTBLEAllNone= DM.ARAY[ri.TLDR].CheckAllStat(cs.None) && DM.ARAY[ri.TVSN].CheckAllStat(cs.None) &&
                                    DM.ARAY[ri.TMRK].CheckAllStat(cs.None) && DM.ARAY[ri.TULD].CheckAllStat(cs.None) &&
                                    DM.ARAY[ri.TRJM].CheckAllStat(cs.None) && DM.ARAY[ri.TRJV].CheckAllStat(cs.None) ;
                bool isTBLELODROk =(DM.ARAY[ri.PLDR].CheckAllStat(cs.None) || DM.ARAY[ri.TLDR].CheckAllStat(cs.Unknown)) ||
                                   (DM.ARAY[ri.PLDR].GetCntStat(cs.Unknown) == 0 && SEQ.LODR.GetSeqStep() != (int)Loader.sc.Push && DM.ARAY[ri.TLDR].CheckAllStat(cs.None)) || 
                                   (DM.ARAY[ri.PLDR].CheckAllStat(cs.Unknown) && DM.ARAY[ri.TLDR].CheckAllStat(cs.Unknown)) ;
                bool isTBLEVISNOk = DM.ARAY[ri.TVSN].CheckAllStat(cs.None) || DM.ARAY[ri.TVSN].CheckAllStat(cs.Good)   || DM.ARAY[ri.TVSN].CheckAllStat(cs.NGVisn) ;
                bool isTBLEMARKOk = DM.ARAY[ri.TMRK].CheckAllStat(cs.None) || DM.ARAY[ri.TMRK].CheckAllStat(cs.Good)   || DM.ARAY[ri.TMRK].CheckAllStat(cs.NGVisn)  ||  DM.ARAY[ri.TMRK].CheckAllStat(cs.NGMark);
                bool isTBLEULDROk = DM.ARAY[ri.TULD].CheckAllStat(cs.None) || DM.ARAY[ri.TULD].CheckAllStat(cs.NGMark) || DM.ARAY[ri.TULD].CheckAllStat(cs.NGVisn) && SEQ.PULD.GetSeqStep() != (int)PreUnloader.sc.Work;
                bool isTBLEREJMOk = DM.ARAY[ri.TRJM].CheckAllStat(cs.None) || DM.ARAY[ri.TRJM].CheckAllStat(cs.NGVisn) ;
                bool isTBLEREJVOk = DM.ARAY[ri.TRJV].CheckAllStat(cs.None) ;

                bool isCycleMove   = !isTBLEAllNone && 
                                      isTBLELODROk  && 
                                      isTBLEVISNOk  && 
                                      isTBLEMARKOk  &&
                                      isTBLEULDROk  &&
                                      isTBLEREJMOk  &&
                                      isTBLEREJVOk  ;
                                     
                bool isCycleEnd     = isTBLEAllNone ;
                                          
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleMove    ) { Step.eSeq = sc.Move        ;  }
                else if (isCycleEnd     ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                //if( DM.ARAY[ri.IDXR].CheckAllStat(cs.None) && IO_GetX(xi.IDXR_TrayDtct) && !isCycleGet) {ER_SetErr(ei.PKG_Unknwn , "RearIndex Unknwn PKG Error."   ); return false;}
                //if(!DM.ARAY[ri.IDXR].CheckAllStat(cs.None) &&!IO_GetX(xi.IDXR_TrayDtct)               ) {ER_SetErr(ei.PKG_Dispr  , "RearIndex Disappear PKG Error."); return false;}

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
                default           :                          Trace("default End");                                     Step.eSeq = sc.Idle;   return false;
                case (sc.Idle    ):                                                                                                           return false;
                case (sc.Move    ): if (CycleMove     ())   { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;

            }
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        //위에 부터 작업.
        public bool FindChip(int _iId, out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c = 0;
            r = 0;
            
            DM.ARAY[_iId].FindLastColFrstRow((cs)_iChip, ref c, ref r);
            return true ;
        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 30000 )) {
                sTemp = string.Format("Time Out Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Trace(sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Trace(sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if(Stat.bReqStop) {
                //return true ;
            }

            //FindChip(out iAray , out iC , out iR);
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;
            
                case 10:
                    iWorkCount = 0; //현재 위치를 웨이트 플러스 피치 곱하기 0으로 함.
                    CL_Move(ci.TBLE_Grpr1FwBw, fb.Bwd);
                    CL_Move(ci.TBLE_Grpr2FwBw, fb.Bwd);
                    CL_Move(ci.TBLE_Grpr3FwBw, fb.Bwd);
                    CL_Move(ci.TBLE_Grpr4FwBw, fb.Bwd);
                    CL_Move(ci.TBLE_Grpr5FwBw, fb.Bwd);
                    CL_Move(ci.TBLE_Grpr6FwBw, fb.Bwd);
                    Step.iHome++;
                    return false ;
            
                case 11:
                    if (!CL_Complete(ci.TBLE_Grpr1FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr2FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr3FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr4FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr5FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr6FwBw, fb.Bwd)) return false;
                    CL_Move(ci.LODR_RngGrpFwBw  , fb.Bwd);
                    CL_Move(ci.VISN_TurnGrpFwBw , fb.Bwd);
                    CL_Move(ci.MARK_AlgnFwBw    , fb.Bwd);
                    CL_Move(ci.MARK_AlgnPinFwBw , fb.Bwd);
                    Step.iHome++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.LODR_RngGrpFwBw  , fb.Bwd)) return false;
                    if (!CL_Complete(ci.VISN_TurnGrpFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.MARK_AlgnFwBw    , fb.Bwd)) return false;
                    if (!CL_Complete(ci.MARK_AlgnPinFwBw , fb.Bwd)) return false;
                    MT_GoHome(mi.TBLE_TTble);
           
                    Step.iHome++;
                    return false ;

                case 13:
                    if (!MT_GetHomeDone(mi.TBLE_TTble)) return false;
                    MT_GoAbsRun(mi.TBLE_TTble, pv.TBLE_TTbleWait);
                    
                    Step.iHome++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.TBLE_TTble , pv.TBLE_TTbleWait))return false;
                    CL_Move(ci.TBLE_Grpr1FwBw, fb.Fwd);
                    CL_Move(ci.TBLE_Grpr2FwBw, fb.Fwd);
                    CL_Move(ci.TBLE_Grpr3FwBw, fb.Fwd);
                    CL_Move(ci.TBLE_Grpr4FwBw, fb.Fwd);
                    CL_Move(ci.TBLE_Grpr5FwBw, fb.Fwd);
                    CL_Move(ci.TBLE_Grpr6FwBw, fb.Fwd);
                    Step.iHome++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.TBLE_Grpr1FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr2FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr3FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr4FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr5FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr6FwBw, fb.Fwd)) return false;
                    Step.iHome = 0;
                    return true;
                    
                   
            }
        }

        public bool CycleMove()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 20000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "" + Step.eSeq.ToString() + "" + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
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
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    OM.EqpStat.dWorkTime = SEQ.bWorkTimer ? SEQ.m_cyWorktime.CheckTime_s() : 0;
                    OM.EqpStat.dWorkUPH  = SEQ.bWorkTimer ? 3600 / OM.EqpStat.dWorkTime : 0 ;
                    SEQ.m_cyWorktime.Clear();
                    SEQ.bWorkTimer = true;
                    MoveCyl(ci.TBLE_Grpr1FwBw, fb.Bwd);
                    MoveCyl(ci.TBLE_Grpr2FwBw, fb.Bwd);
                    MoveCyl(ci.TBLE_Grpr3FwBw, fb.Bwd);
                    MoveCyl(ci.TBLE_Grpr4FwBw, fb.Bwd);
                    MoveCyl(ci.TBLE_Grpr5FwBw, fb.Bwd);
                    MoveCyl(ci.TBLE_Grpr6FwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.TBLE_Grpr1FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr2FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr3FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr4FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr5FwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr6FwBw, fb.Bwd)) return false;
                    if(IO_GetX(xi.TBLE_LODRClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "LODR Clamp Cylinder Is Not Bwd"); return true ;}
                    if(IO_GetX(xi.TBLE_VISNClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "Visn Clamp Cylinder Is Not Bwd"); return true ;}
                    if(IO_GetX(xi.TBLE_MARKClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "MARK Clamp Cylinder Is Not Bwd"); return true ;}
                    if(IO_GetX(xi.TBLE_ULDRClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "ULDR Clamp Cylinder Is Not Bwd"); return true ;}
                    if(IO_GetX(xi.TBLE_RJEVClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "RJEV Clamp Cylinder Is Not Bwd"); return true ;}
                    if(IO_GetX(xi.TBLE_RJEMClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "RJEM Clamp Cylinder Is Not Bwd"); return true ;}
                    MoveCyl(ci.LODR_RngGrpFwBw  , fb.Bwd);
                    MoveCyl(ci.VISN_TurnGrpFwBw , fb.Bwd);
                    if (OM.MstOptn.bMarkAlgin)
                    {
                        MoveCyl(ci.MARK_AlgnFwBw    , fb.Bwd);
                        MoveCyl(ci.MARK_AlgnPinFwBw , fb.Bwd);
                    }
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.LODR_RngGrpFwBw  , fb.Bwd))return false;
                    if(!CL_Complete(ci.VISN_TurnGrpFwBw , fb.Bwd))return false;
                    if(OM.MstOptn.bMarkAlgin && !CL_Complete(ci.MARK_AlgnFwBw    , fb.Bwd))return false;
                    if(OM.MstOptn.bMarkAlgin && !CL_Complete(ci.MARK_AlgnPinFwBw , fb.Bwd))return false;

                    //1.0.1.3
                    //현재 위치로 일단 보내고
                    //바코드 위치 사용 안할때는 되돌린다.
                    if (!OM.CmnOptn.bStopBarcRead)
                    {
                        MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait, PM.GetValue(mi.TBLE_TTble, pv.TBLE_TTbleWorkPitch) * iWorkCount);
                    }
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.TBLE_TTble, pv.TBLE_TTbleWait , PM.GetValue(mi.TBLE_TTble ,  pv.TBLE_TTbleWorkPitch) * iWorkCount )) return false;
                    Step.iCycle = 15;
                    return false;

                case 15:
                    //1.0.1.3 바코드 추가
                    //초기화
                    bRet1 = false; //바코드 결과값 초기화 에러일때 없을때
                    bRet2 = false; //바코드 결과값 초기화 입력한 바코드랑 다를때
                    bRet3 = false; //바코드 결과값 초기화 등급이 A,B가 아닐때

                    iWorkCountTemp = iWorkCount + 1;
                    if (iWorkCountTemp > 6) iWorkCountTemp = 1;

                    //1.0.1.5
                    bool bExist = !DM.ARAY[ri.TMRK].CheckAllStat(cs.None);
                    bool bMarkSkip = DM.ARAY[ri.TMRK].SubStep == 1;
                    if (!bExist /*|| (bExist && bMarkSkip)*/ && OM.CmnOptn.bIgnrBarcode) //바코드 사용 안할때.
                    {
                        MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait, PM.GetValue(mi.TBLE_TTble, pv.TBLE_TTbleWorkPitch) * (iWorkCountTemp));
                        Step.iCycle = 30; //바코드 사용안할시 기존 스텝
                        return false;
                    }
                    else //바코드 사용할때.
                    {
                        //바코드 초기 상태 확인후 접속하고 안되면 에러.
                        if (!SEQ.BarCodeReader.GetReady())
                        {
                            if (!SEQ.BarCodeReader.ConnectToServer())
                            {
                                ML.ER_SetErr(ei.PRT_Barcode, SEQ.BarCodeReader.ErrMsg);
                                Step.iCycle = 0;
                                return true;
                            }
                        }

                        if (OM.CmnOptn.bStopBarcRead)
                        {
                            double dBarcodePosOfs = PM.GetValue(mi.TBLE_TTble, pv.TBLE_TTbleBarcRead);
                            MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait, (PM.GetValue(mi.TBLE_TTble, pv.TBLE_TTbleWorkPitch) * iWorkCount) + dBarcodePosOfs);
                        }
                        else
                        {
                            MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait, PM.GetValue(mi.TBLE_TTble, pv.TBLE_TTbleWorkPitch) * (iWorkCountTemp));
                        }
                        
                    }
                    
                    Step.iCycle = 20; 
                    return false;

                //바코드 읽음
                case 20:
                    if (!MT_GetStopInpos(mi.TBLE_TTble)) return false;
                    if (OM.CmnOptn.bStopBarcRead) SEQ.BarCodeReader.CycleReading(true); //이니셜 하면서 바코드ON
                    Step.iCycle++;
                    return false;

                case 21:
                    //아닐때는 센서로 트리거 바코드 검사
                    //한스텝 돌고 체크
                    if (SEQ.BarCodeReader.ErrMsg != "") //에러 체크.
                    {
                        if (!OM.CmnOptn.bStopBarcRead)
                        {
                            MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait, PM.GetValue(mi.TBLE_TTble, pv.TBLE_TTbleWorkPitch) * iWorkCount); //되돌리고
                        }
                        ML.ER_SetErr(ei.PRT_Barcode, SEQ.BarCodeReader.ErrMsg); //에러
                        Step.iCycle = 0;
                        return true;
                    }
                    //if(!SEQ.BarCodeReader.CycleReading()) return false ;
                    if (!SEQ.BarCodeReader.Read()) return false;

                    Step.iCycle++;
                    return false;

                //바코드 결과 확인
                case 22:
                         if (SEQ.BarCodeReader.GetBarcode() == "ERROR" ||
                             SEQ.BarCodeReader.GetBarcode() == "")                                  bRet1 = true;
                    else if (SEQ.BarCodeReader.GetBarcode() != SEQ.BarCodeReader.GetInputBarcode()) bRet2 = true;
                         if (SEQ.BarCodeReader.GetGradeError())                                     bRet3 = true;

                    if (bRet1 || bRet2 || bRet3)
                    {
                        iBarcodeErrCnt++;
                        if (OM.DevInfo.iBarcodeErrCnt != 0 && iBarcodeErrCnt >= OM.DevInfo.iBarcodeErrCnt)
                        {
                            iBarcodeErrCnt = 0;
                                 if (bRet1) ER_SetErr(ei.PRT_Barcode, "Barcode Reading failed." );
                            else if (bRet2) ER_SetErr(ei.PRT_Barcode, "Barcode Matching failed.");
                            else            ER_SetErr(ei.PRT_Barcode, "Barcode Quality is Raw." );

                            if (!OM.CmnOptn.bStopBarcRead)
                            {
                                MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait, PM.GetValue(mi.TBLE_TTble, pv.TBLE_TTbleWorkPitch) * iWorkCount);
                            }
                            Step.iCycle = 0;
                            return true;
                        }
                    }
                    if (OM.CmnOptn.bStopBarcRead)
                    {
                        //테이블 다음 위치로 돌리기
                        MoveMotr(mi.TBLE_TTble, pv.TBLE_TTbleWait, PM.GetValue(mi.TBLE_TTble, pv.TBLE_TTbleWorkPitch) * (iWorkCountTemp));
                    }
                    Step.iCycle = 30;
                    return false;

                //바코드 사용안할시 기존 스텝
                case 30:
                    if (!MT_GetStopInpos(mi.TBLE_TTble)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;
                    
                case 31: //알수없음 기존에 
                    if(!m_tmDelay.OnDelay(50))return false;
                    Step.iCycle++;
                    return false;

                case 32:
                    //기존에 인크리멘탈로 돌리던것 수정.
                    iWorkCount++;
                    if(iWorkCount >= 6) iWorkCount = 0 ;

                    //데이터 마스킹
                    DM.ShiftData(ri.TRJM, ri.TRJV);
                    DM.ShiftData(ri.TULD, ri.TRJM);
                    DM.ShiftData(ri.TMRK, ri.TULD);
                    DM.ShiftData(ri.TVSN, ri.TMRK); 
                    //Marking 위치가 굿으로 바뀌어있으면 Unkwon으로 바꾸자.
                    if (DM.ARAY[ri.TMRK].CheckAllStat(cs.Good)) DM.ARAY[ri.TMRK].SetStat(cs.Unknown);
                    DM.ShiftData(ri.TLDR, ri.TVSN);
                    if(!DM.ARAY[ri.TVSN].CheckAllStat(cs.None)) MoveCyl(ci.VISN_TurnGrpFwBw , fb.Fwd);
                    else                                        MoveCyl(ci.VISN_TurnGrpFwBw , fb.Bwd);

                    //1.0.1.3
                    //바코드 리더기로 읽엇을때와 입력한 바코드값이 다를때
                    if (!OM.CmnOptn.bIgnrBarcode) //바코드를 사용할때
                    {
                        if (!DM.ARAY[ri.TULD].CheckAllStat(cs.None)) //자재가 있으면
                        {
                            if ((bRet1 || bRet2 || bRet3) && !DM.ARAY[ri.TULD].CheckAllStat(cs.NGVisn)) //비전 에러가 우선임
                            {
                                DM.ARAY[ri.TULD].SetStat(cs.NGMark);
                            }
                        }
                    }

                    Step.iCycle++;
                    return false;

                case 33:
                    if (!DM.ARAY[ri.TVSN].CheckAllStat(cs.None) &&!CL_Complete(ci.VISN_TurnGrpFwBw, fb.Fwd))return false;
                    if ( DM.ARAY[ri.TVSN].CheckAllStat(cs.None) &&!CL_Complete(ci.VISN_TurnGrpFwBw, fb.Bwd))return false;

                    //if (!SEQ.BarCodeReader.CycleRead(true)) return false;

                    Step.iCycle++;
                    return false;

                case 34:

                    MoveCyl(ci.TBLE_Grpr1FwBw, fb.Fwd);
                    MoveCyl(ci.TBLE_Grpr2FwBw, fb.Fwd);
                    MoveCyl(ci.TBLE_Grpr3FwBw, fb.Fwd);
                    MoveCyl(ci.TBLE_Grpr4FwBw, fb.Fwd);
                    MoveCyl(ci.TBLE_Grpr5FwBw, fb.Fwd);
                    MoveCyl(ci.TBLE_Grpr6FwBw, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 35:
                    if (!CL_Complete(ci.TBLE_Grpr1FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr2FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr3FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr4FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr5FwBw, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TBLE_Grpr6FwBw, fb.Fwd)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 36:
                    if(!m_tmDelay.OnDelay(100))return false;
                    if(!IO_GetX(xi.TBLE_LODRClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "LODR Clamp Cylinder Is Not Fwd"); return true ;}
                    if(!IO_GetX(xi.TBLE_VISNClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "Visn Clamp Cylinder Is Not Fwd"); return true ;}
                    if(!IO_GetX(xi.TBLE_MARKClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "MARK Clamp Cylinder Is Not Fwd"); return true ;}
                    if(!IO_GetX(xi.TBLE_ULDRClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "ULDR Clamp Cylinder Is Not Fwd"); return true ;}
                    if(!IO_GetX(xi.TBLE_RJEVClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "RJEV Clamp Cylinder Is Not Fwd"); return true ;}
                    if(!IO_GetX(xi.TBLE_RJEMClmpDtct)){ER_SetErr(ei.ATR_TimeOut , "RJEM Clamp Cylinder Is Not Fwd"); return true ;}
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.TBLE_Grpr1FwBw){
                //if(_eFwd == fb.Fwd) {
                //    if(CL_Complete(ci.IDXR_ClampClOp, fb.Fwd))
                //    {
                //        sMsg = "Rear Index is Close";
                //        bRet = false ;
                //    }
                //}
            }
            else if (_eActr == ci.TBLE_Grpr2FwBw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.TBLE_Grpr2FwBw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.TBLE_Grpr3FwBw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.TBLE_Grpr4FwBw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.TBLE_Grpr5FwBw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.TBLE_Grpr6FwBw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if(_eActr == ci.LODR_RngGrpFwBw )
            {

            }
            else if(_eActr == ci.VISN_TurnGrpFwBw)
            {

            }
            else if (_eActr == ci.MARK_AlgnFwBw)
            {

            }
            else if (_eActr == ci.MARK_AlgnPinFwBw)
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

            if (_eMotr == mi.TBLE_TTble)
            {
                if(CL_GetCmd(ci.TBLE_Grpr1FwBw  ) != fb.Bwd){sMsg = "Grpper 1번 동작 중!"    ; bRet = false ;}
                if(CL_GetCmd(ci.TBLE_Grpr2FwBw  ) != fb.Bwd){sMsg = "Grpper 2번 동작 중!"    ; bRet = false ;}
                if(CL_GetCmd(ci.TBLE_Grpr3FwBw  ) != fb.Bwd){sMsg = "Grpper 3번 동작 중!"    ; bRet = false ;}
                if(CL_GetCmd(ci.TBLE_Grpr4FwBw  ) != fb.Bwd){sMsg = "Grpper 4번 동작 중!"    ; bRet = false ;}
                if(CL_GetCmd(ci.TBLE_Grpr5FwBw  ) != fb.Bwd){sMsg = "Grpper 5번 동작 중!"    ; bRet = false ;}
                if(CL_GetCmd(ci.TBLE_Grpr6FwBw  ) != fb.Bwd){sMsg = "Grpper 6번 동작 중!"    ; bRet = false ;}
                if(CL_GetCmd(ci.LODR_RngGrpFwBw ) != fb.Bwd){sMsg = "Loader Gripper 동작 중!"; bRet = false ;}
                if(CL_GetCmd(ci.VISN_TurnGrpFwBw) != fb.Bwd){sMsg = "Vision Gripper 동작 중!"; bRet = false ;}
                if(CL_GetCmd(ci.MARK_AlgnFwBw   ) != fb.Bwd){sMsg = "Mark Align 동작 중!"    ; bRet = false ;}
                if(CL_GetCmd(ci.MARK_AlgnPinFwBw) != fb.Bwd){sMsg = "Mark Align Pin 동작 중!"; bRet = false ;}
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
            if (!MT_GetStop(mi.TBLE_TTble)) return false;

            if (!CL_Complete(ci.TBLE_Grpr1FwBw  )) return false;
            if (!CL_Complete(ci.TBLE_Grpr2FwBw  )) return false;
            if (!CL_Complete(ci.TBLE_Grpr3FwBw  )) return false;
            if (!CL_Complete(ci.TBLE_Grpr4FwBw  )) return false;
            if (!CL_Complete(ci.TBLE_Grpr5FwBw  )) return false;
            if (!CL_Complete(ci.TBLE_Grpr6FwBw  )) return false;
            if (!CL_Complete(ci.LODR_RngGrpFwBw )) return false;
            if (!CL_Complete(ci.VISN_TurnGrpFwBw)) return false;
            if (!CL_Complete(ci.MARK_AlgnFwBw   )) return false;
            if (!CL_Complete(ci.MARK_AlgnPinFwBw)) return false;
            
            return true ;
        }
        //public void SaveLotName(string _sLotName){
        //    string sLotDataPath = "C:\\Data\\LotName.ini";
        //    CIniFile IniLotDatadSave = new CIniFile(sLotDataPath);

        //    IniLotDatadSave.Save("LotName", "LotName", _sLotName);

        //}

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
