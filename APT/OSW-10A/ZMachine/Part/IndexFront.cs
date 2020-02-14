using COMMON;
using System;
using SML2;



namespace Machine
{
    public class IndexFront : Part
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
            Get        ,
            Barcode    ,
            Out        , 
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

        public IndexFront()
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
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: 
                    Step.iToStart = 0;
                    return true;

                case 10:
                    if (DM.ARAY[ri.IDXF].CheckAllStat(cs.None))
                    {
                        MoveCyl(ci.IDXF_ClampClOp, fb.Bwd);
                        Step.iToStart++;
                        return false;
                    }
                    Step.iToStart = 0;
                    return true;
                    

                case 11: 
                    if(!CL_Complete(ci.IDXF_ClampClOp, fb.Bwd))return false;
                    MoveCyl(ci.IDXF_ClampUpDn, fb.Bwd);
                    Step.iToStart++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.IDXF_ClampUpDn, fb.Bwd))return false;
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
                Log.Trace(m_sPartName, sTemp);
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
               
                    
                    OM.EqpStat.dLastIDXFPos = SM.MT_GetCmdPos(mi.IDXF_XFrnt);
                    Step.iToStop++;
                    return false;

                case 11: 

                    Step.iToStop++;
                    return false;

                case 12:
                    Step.iToStop++;
                    return false;
                
                case 13: 
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

        //public bool GetTrayAllGood()
        //{
        //    for (int c = 0; c < OM.DevInfo.iTRAY_PcktCntX; c++)
        //    {
        //        for (int r = 0; r < OM.DevInfo.iTRAY_PcktCntY; r++)
        //        {
        //            if (DM.ARAY[ri.MASK].GetStat(c, r) == cs.None) continue ;
        //            if (DM.ARAY[ri.IDXF].GetStat(c, r) != cs.Good) return false ;
        //        }
        //    }
        //    return true ;
        //}
        

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

            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                if (IO_GetX(xi.IDXF_Overload))
                {
                    ER_SetErr(ei.PRT_OverLoad, "Front Index OverLoad Error.");
                }

                
                

                bool isCycleGet     = DM.ARAY[ri.SPLR].CheckAllStat(cs.Unknown) && DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && SEQ.IDXR.GetStep().eSeq != IndexRear.sc.Get && !OM.CmnOptn.bIdxFSkip;
                bool isCycleBarcode = DM.ARAY[ri.IDXF].CheckAllStat(cs.Unknown) &&  
                                     //(DM.ARAY[ri.IDXR].CheckAllStat(cs.None)    ||  SEQ.IDXR.GetStep().eSeq == IndexRear.sc.Out );
                                     (DM.ARAY[ri.IDXR].CheckAllStat(cs.None)    ||  (!IO_GetX(xi.RAIL_TrayDtct2 ) && SEQ.IDXR.GetStep().eSeq == IndexRear.sc.Out));
                bool isCycleOut     = DM.ARAY[ri.IDXF].CheckAllStat(cs.Good) || DM.ARAY[ri.IDXF].CheckAllStat(cs.Empty) && DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) && (DM.ARAY[ri.IDXR].CheckAllStat(cs.None) || DM.ARAY[ri.IDXR].CheckAllStat(cs.Unknown))&& !OM.CmnOptn.bGoldenTray;
                bool isCycleEnd     =(DM.ARAY[ri.IDXF].CheckAllStat(cs.None) ||(!DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && OM.CmnOptn.bGoldenTray)) ;//&& LOT.LotList.Count == 0;


                //문제가 많음 로더쪽 위도 감지되는 경우 있고 이리 저리 에러 뜸.
                //싸이클에 넣자.
                //if(isCycleGet || isCycleBarcode || isCycleOut) {
                //    if( DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && IO_GetX(xi.IDXF_TrayDtct) ) {ER_SetErr(ei.PKG_Unknwn , "FrontIndex Unknwn PKG Error."   ); return false;}  //CycleGet To Error.
                //    if(!DM.ARAY[ri.IDXF].CheckAllStat(cs.None) &&!IO_GetX(xi.IDXF_TrayDtct) ) {ER_SetErr(ei.PKG_Dispr  , "FrontIndex Disappear PKG Error."); return false;}                    
                //}

                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleGet     ) { Step.eSeq = sc.Get      ;  }
                else if (isCycleBarcode ) { Step.eSeq = sc.Barcode  ;  }
                else if (isCycleOut     ) { Step.eSeq = sc.Out      ;  }
                else if (isCycleEnd     ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                //
                //

                if(Step.eSeq != sc.Idle){
                    Log.Trace(m_sPartName, Step.eSeq.ToString() +" Start");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }

            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default           :                          Log.Trace(m_sPartName, "default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle    ):                                                                                                                           return false;
                case (sc.Get     ): if (CycleGet     ())   { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Barcode ): if (CycleBarcode ())   { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Out     ): if (CycleOut     ())   { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        //위에 부터 작업.
        //인덱스 데이터에서 작업해야 되는 컬로우를 뽑아서 리턴.
        public bool FindChip(out int _iC, out int _iR,out cs _iChipStat , out bool _bFlying , bool _bGldTry = false) 
        {
            int ColWork   = -1 ; int RowWork   = -1 ;
            int ColVision = -1 ; int RowVision = -1 ;
            int ColEmpty  = -1 ; int RowEmpty  = -1 ;
            int ColNG0    = -1 ; int RowNG0    = -1 ;
            int ColNG1    = -1 ; int RowNG1    = -1 ;
            int ColNG2    = -1 ; int RowNG2    = -1 ;
            int ColNG3    = -1 ; int RowNG3    = -1 ;
            int ColNG4    = -1 ; int RowNG4    = -1 ;
            int ColNG5    = -1 ; int RowNG5    = -1 ;
            int ColNG6    = -1 ; int RowNG6    = -1 ;
            int ColNG7    = -1 ; int RowNG7    = -1 ;
            int ColNG8    = -1 ; int RowNG8    = -1 ;
            int ColNG9    = -1 ; int RowNG9    = -1 ;
            int ColNG10   = -1 ; int RowNG10   = -1 ;
            int ColUkwn   = -1 ; int RowUkwn   = -1 ;

            //현재 작업중인 인덱스의 작업중인 컬럼.              
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.Vision  ,ref ColVision , ref RowVision);
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.Empty   ,ref ColEmpty  , ref RowEmpty );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG0     ,ref ColNG0    , ref RowNG0   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG1     ,ref ColNG1    , ref RowNG1   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG2     ,ref ColNG2    , ref RowNG2   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG3     ,ref ColNG3    , ref RowNG3   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG4     ,ref ColNG4    , ref RowNG4   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG5     ,ref ColNG5    , ref RowNG5   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG6     ,ref ColNG6    , ref RowNG6   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG7     ,ref ColNG7    , ref RowNG7   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG8     ,ref ColNG8    , ref RowNG8   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG9     ,ref ColNG9    , ref RowNG9   );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.NG10    ,ref ColNG10   , ref RowNG10  );
            DM.ARAY[ri.IDXF].FindLastColFrstRow(cs.Unknown ,ref ColUkwn   , ref RowUkwn  );

            if(ColWork <  ColVision) {ColWork = ColVision ; RowWork = RowVision ;} else if(ColWork == ColVision) {if(RowWork > RowVision) RowWork = RowVision ;}
            if(ColWork <  ColEmpty ) {ColWork = ColEmpty  ; RowWork = RowEmpty  ;} else if(ColWork == ColEmpty ) {if(RowWork > RowEmpty ) RowWork = RowEmpty  ;}
            if(ColWork <  ColNG0   ) {ColWork = ColNG0    ; RowWork = RowNG0    ;} else if(ColWork == ColNG0   ) {if(RowWork > RowNG0   ) RowWork = RowNG0    ;}
            if(ColWork <  ColNG1   ) {ColWork = ColNG1    ; RowWork = RowNG1    ;} else if(ColWork == ColNG1   ) {if(RowWork > RowNG1   ) RowWork = RowNG1    ;}
            if(ColWork <  ColNG2   ) {ColWork = ColNG2    ; RowWork = RowNG2    ;} else if(ColWork == ColNG2   ) {if(RowWork > RowNG2   ) RowWork = RowNG2    ;}
            if(ColWork <  ColNG3   ) {ColWork = ColNG3    ; RowWork = RowNG3    ;} else if(ColWork == ColNG3   ) {if(RowWork > RowNG3   ) RowWork = RowNG3    ;}
            if(ColWork <  ColNG4   ) {ColWork = ColNG4    ; RowWork = RowNG4    ;} else if(ColWork == ColNG4   ) {if(RowWork > RowNG4   ) RowWork = RowNG4    ;}
            if(ColWork <  ColNG5   ) {ColWork = ColNG5    ; RowWork = RowNG5    ;} else if(ColWork == ColNG5   ) {if(RowWork > RowNG5   ) RowWork = RowNG5    ;}
            if(ColWork <  ColNG6   ) {ColWork = ColNG6    ; RowWork = RowNG6    ;} else if(ColWork == ColNG6   ) {if(RowWork > RowNG6   ) RowWork = RowNG6    ;}
            if(ColWork <  ColNG7   ) {ColWork = ColNG7    ; RowWork = RowNG7    ;} else if(ColWork == ColNG7   ) {if(RowWork > RowNG7   ) RowWork = RowNG7    ;}
            if(ColWork <  ColNG8   ) {ColWork = ColNG8    ; RowWork = RowNG8    ;} else if(ColWork == ColNG8   ) {if(RowWork > RowNG8   ) RowWork = RowNG8    ;}
            if(ColWork <  ColNG9   ) {ColWork = ColNG9    ; RowWork = RowNG9    ;} else if(ColWork == ColNG9   ) {if(RowWork > RowNG9   ) RowWork = RowNG9    ;}
            if(ColWork <  ColNG10  ) {ColWork = ColNG10   ; RowWork = RowNG10   ;} else if(ColWork == ColNG10  ) {if(RowWork > RowNG10  ) RowWork = RowNG10   ;}
            //if(ColWork <  ColUkwn  ) {ColWork = ColUkwn   ; RowWork = RowUkwn   ;} else if(ColWork == ColUkwn  ) {if(RowWork > RowUkwn  ) RowWork = RowUkwn   ;}
            if (_bGldTry)
            {
                _iC = ColVision;
                _iR = RowVision;
            }
            else
            {
                _iC = ColWork ; 
                _iR = RowWork ;
            }

            if((_iC < 0 || _iR < 0)) {
                _iChipStat = cs.RetFail ;                
                _bFlying   = false ;
                return false ;
            }

            _iChipStat = DM.ARAY[ri.IDXF].GetStat(_iC , _iR);
            _bFlying   = DM.ARAY[ri.IDXF].GetCntColStat(_iC, cs.Vision) != 0;

            return true;
        } 

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 30000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                Log.Trace(m_sPartName, sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if(Stat.bReqStop) {
                //return true ;
            }

            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;

                case 10:
                    CL_Move(ci.IDXF_ClampClOp, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Bwd)) return false;
                    CL_Move(ci.IDXF_ClampUpDn, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.IDXF_ClampUpDn, fb.Bwd)) return false;
                    MT_GoHome(mi.IDXF_XFrnt);

                    Step.iHome++;
                    return false;

                case 13:
                    if (!MT_GetHomeDone(mi.IDXF_XFrnt)) return false;
                    if (DM.ARAY[ri.IDXF].CheckAllStat(cs.None))//자제 없으면 웨이트로 감.
                    {
                        Step.iHome = 50;
                        return false;
                    }
                    Step.iHome++;
                    return false;

                case 14:
                    MT_GoAbsSlow(mi.IDXF_XFrnt, OM.EqpStat.dLastIDXFPos);
                    Step.iHome++;
                    return false;

                case 15:
                    if (!MT_GetStop(mi.IDXF_XFrnt)) return false;
                    
                    CL_Move(ci.IDXF_ClampUpDn, fb.Fwd);
                    Step.iHome++;
                    return false;

                case 16:
                    if (!CL_Complete(ci.IDXF_ClampUpDn, fb.Fwd)) return false;
                    CL_Move(ci.IDXF_ClampClOp, fb.Fwd);
                    Step.iHome++;
                    return false;

                case 17:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Fwd)) return false;

                    Step.iHome = 0;
                    return true;

                //위에서 씀.
                case 50:
                    MT_GoAbsRun(mi.IDXF_XFrnt, PM.GetValue(mi.IDXF_XFrnt, pv.IDXF_XFrntWait));
                    Step.iHome++;
                    return false;

                case 51:
                    if(!MT_GetStopInpos(mi.IDXF_XFrnt))return false;
                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CycleGet()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 20000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            

            if (Stat.bReqStop)
            {
                //return true ;
            }

            if (IO_GetX(xi.IDXF_Overload))
            {
                MT_Stop(mi.IDXF_XFrnt);
                ER_SetErr(ei.PRT_OverLoad, "Front Index Overload Error");
                return true;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:                  
                    
                    MoveCyl(ci.IDXF_ClampClOp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Bwd)) return false;
                    MoveCyl(ci.IDXF_ClampUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.IDXF_ClampUpDn, fb.Bwd)) return false;
                    MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntClamp);
                    MoveCyl (ci.IDXF_ClampClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntClamp)) return false;
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Fwd)) return false;
                    MoveCyl (ci.IDXF_ClampClOp, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Bwd)) return false;
                    MoveCyl(ci.IDXF_ClampUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.IDXF_ClampUpDn, fb.Fwd)) return false;
                    //MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntClamp, -5);
                    Step.iCycle++;
                    return false;

                case 16:
                    //if (!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntClamp, -5)) return false;
                    if (!IO_GetX(xi.IDXF_TrayDtct) && !OM.CmnOptn.bIdleRun)
                    {
                        ER_SetErr(ei.PKG_Dispr, "Front Index tray detect sensor Not detected.");
                        return true;
                    }
                    MoveCyl(ci.IDXF_ClampClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Fwd)) return false;
                    
                    DM.ARAY[ri.IDXF].SetStat(cs.Unknown);
                    for (int c = 0; c < OM.DevInfo.iTRAY_PcktCntX; c++)
                    {
                        for (int r = 0; r < OM.DevInfo.iTRAY_PcktCntY; r++)
                        {
                            if (DM.ARAY[ri.MASK].GetStat(c, r) == cs.None)
                            {
                                DM.ARAY[ri.IDXF].SetStat(c, r, cs.None);
                            }
                        }
                    }
                    DM.ARAY[ri.SPLR].SetStat(cs.None);

                    Step.iCycle = 0;
                    return true;
            }
        }

        //public bool bNeedToBarcode;
        public bool bTrayWorkTimer;
        public bool CycleBarcode()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                Log.Trace(m_sPartName, sTemp);
            }

            if (IO_GetX(xi.IDXF_Overload))
            {
                MT_Stop(mi.IDXF_XFrnt);
                ER_SetErr(ei.PRT_OverLoad, "Front Index is Overload Error");
                return true;
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            LOT.TLot Lot ;
            
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    if (!OM.CmnOptn.bIdleRun && IO_GetX(xi.RAIL_TrayDtct2)&&!MT_CmprPos(mi.IDXF_XFrnt, PM_GetValue(mi.IDXF_XFrnt,pv.IDXF_XFrntBarcode)))
                    {
                        //ER_SetErr(ei.PRT_Detect , IO_GetXName(xi.RAIL_TrayDtct2) +" is Something Detected!");
                        //return true ;
                    }
                    OM.EqpStat.sTraySttTime = DateTime.Now.ToString("HH:mm:ss");
                    OM.EqpStat.dTrayWorkTime = SEQ.bTrayWorkTimer ? SEQ.m_cyTrayWorktime.CheckTime_s() : 0;
                    OM.EqpStat.dTrayUPH = SEQ.bTrayWorkTimer ? 3600 / (OM.EqpStat.dTrayWorkTime / (DM.ARAY[ri.IDXF].GetMaxCol() * DM.ARAY[ri.IDXF].GetMaxRow() - DM.ARAY[ri.IDXF].GetCntStat(cs.None))) : 0;
                    SEQ.m_cyTrayWorktime.Clear();
                    SEQ.bTrayWorkTimer = true;

                    if(OM.CmnOptn.bIdleRun){
                         Step.iCycle=100;
                         return false;
                    }

                    //이조건이면 바코드 안찍는다.
                    if (!OM.CmnOptn.bIdleRun) { //아이들 런이면 첫장부터 작업 한다.
                        if(OM.GetSupplyCnt() == 1 || (OM.DevOptn.bUseBtmCover && OM.GetSupplyCnt() == OM.DevInfo.iTRAY_StackingCnt) || OM.CmnOptn.bGoldenTray ){
                             Step.iCycle=50;
                             return false;
                        }
                    }
                    Step.iCycle++;
                    return false;

                case 11:        
                    //그냥 오토런에서만 본다.
                    if (!IO_GetX(xi.IDXF_TrayDtct) && !OM.CmnOptn.bIdleRun)
                    {
                        ER_SetErr(ei.PKG_Dispr, "Front Index tray detect sensor Not detected.");
                        return true;
                    }

                    SEQ.BarcordLODR.SendScanOn();
                    MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntBarcode);                    
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntBarcode)) return false;

                    if (!OM.CmnOptn.bIdleRun && !IO_GetX(xi.RAIL_TrayDtct2))//sensor zone is over when col 0
                    {
                        ER_SetErr(ei.PRT_Missed , IO_GetXName(xi.RAIL_TrayDtct2) +" is Not Detected!");
                        return true ;
                    }

                    

                    MoveCyl(ci.IDXF_ClampClOp, fb.Bwd); //레일 초기 진입시에 크리퍼랑 레일이랑 찐따지는것 방지용. 한번 풀었다 잡는다.
                    Step.iCycle++;
                    return false ;

                case 13:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Bwd)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:

                    if(m_tmDelay.OnDelay(3000)) {//바코드 못읽었을때 상황.
                        if(SEQ.BarcordLODR.GetText() == "") {
                            ER_SetErr(ei.PRT_Barcode, "Barcode Reading Failed");
                            return true;
                        }
                    }
                    if(SEQ.BarcordLODR.GetText() == "")return false ;

                    if(SPC.LOT.Data.TrayNo == "") {
                        SPC.LOT.Data.TrayNo = SEQ.BarcordLODR.GetText() ;                   
                    }
                    OM.EqpStat.sTrayLabel = SEQ.BarcordLODR.GetText() ;


                    if (!OM.CmnOptn.bOracleNotUse)
                    {
                        if (!SEQ.Oracle.ProcessTrayLabel(OM.EqpStat.sTrayLabel))
                        {
                            SM.ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg() );
                            Log.Trace("Oracle Fail Message" , SEQ.Oracle.GetLastMsg());
                            return true ;
                        }

                        //쓰레드로 돌린다 PanelID가 많을때 10만개 정도...
                        SEQ.Oracle.ThreadMakePanelIDList();
                    }

                    Step.iCycle++;
                    return false ;

                case 15:
                    if (!OM.CmnOptn.bOracleNotUse)
                    {
                        //쓰레드로 돌린다 PanelID가 많을때 10만개 정도...
                        if(COracle.bMakingPanelList)return false ;
                        if (!COracle.bMakePanelListRet)
                        {
                            ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                            return true ;
                        }
                    }
                    MoveCyl(ci.IDXF_ClampClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Fwd)) return false;
                    MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntVsnStt1);
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntVsnStt1)) return false;
                    DM.ARAY[ri.IDXF].ChangeStat(cs.Unknown, cs.Vision);
                    Step.iCycle = 0;
                    return true;




                    
                    
                //골든디바이스 및 빈트레이일때 위에서 씀.
                case 50 :
                    MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntVsnStt1);
                    Step.iCycle++;
                    return false ;

                case 51:
                    if(!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntVsnStt1)) return false;
                    MoveCyl(ci.IDXF_ClampClOp, fb.Bwd); //레일 초기 진입시에 크리퍼랑 레일이랑 찐따지는것 방지용. 한번 풀었다 잡는다.
                    Step.iCycle++;
                    return false ;

                case 52:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Bwd)) return false;
                    MoveCyl(ci.IDXF_ClampClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 53:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Fwd)) return false;
                    

                    //골드트레이가 아니고서.
                    if(!OM.CmnOptn.bGoldenTray){
                        DM.ARAY[ri.IDXF].ChangeStat(cs.Unknown, cs.Empty);
                        if (OM.CmnOptn.iInspCrvrTray == (int)vi.One) {
                            DM.ARAY[ri.IDXF].SetStat(OM.DevInfo.iTRAY_PcktCntX - 1, 0, cs.Vision);
                        }
                        else if (OM.CmnOptn.iInspCrvrTray == (int)vi.Col) {
                            for (int i = 0; i < OM.DevInfo.iTRAY_PcktCntY; i++) {
                                DM.ARAY[ri.IDXF].SetStat(OM.DevInfo.iTRAY_PcktCntX - 1, i, cs.Vision);
                            }
                        }
                        else if (OM.CmnOptn.iInspCrvrTray == (int)vi.All) {
                            DM.ARAY[ri.IDXF].SetStat(cs.Vision) ;
                        }
                        else {
                            DM.ARAY[ri.IDXF].SetStat(cs.Vision) ;
                        }
                    }
                    else {//골든디바이스
                        DM.ARAY[ri.IDXF].ChangeStat(cs.Unknown, cs.Vision);

                    }

                    Step.iCycle=0;
                    return true ;

                case 100:
                    MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntBarcode);
                    Step.iCycle++;
                    return false ;

                case 101:
                    if (!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntBarcode)) return false;
                    MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntVsnStt1);
                    Step.iCycle++;
                    return false;

                case 102:
                    if(!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntVsnStt1)) return false;
                    DM.ARAY[ri.IDXF].ChangeStat(cs.Unknown, cs.Vision);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleOut() 
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 20000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "-" + GetCrntCycleName() + sTemp;
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            if (IO_GetX(xi.IDXF_Overload))
            {
                MT_Stop(mi.IDXF_XFrnt);
                ER_SetErr(ei.PRT_OverLoad, "Front Index is Overload Error");
                return true;
            }

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    if (!IO_GetX(xi.IDXF_TrayDtct) && !OM.CmnOptn.bIdleRun)
                    {
                        ER_SetErr(ei.PKG_Dispr, "Front Index tray detect sensor Not detected.");
                        return true;
                    }



                    MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntUld);
                    if (!OM.CmnOptn.bOracleNotUse && !OM.CmnOptn.bIdleRun) {
                        if (DM.ARAY[ri.IDXF].CheckAllStat(cs.Good)) { 
                            //if(!OM.CmnOptn.bOracleNotWriteInsp){
                            //    for (int r = 0; r < DM.ARAY[ri.IDXF].GetMaxRow(); r++)
                            //    {
                            //        for (int c = 0; c < DM.ARAY[ri.IDXF].GetMaxCol(); c++) { 
                            //            if (!SEQ.Oracle.Insert("insert " + DM.ARAY[ri.IDXF].Chip[c,r].sQuery))
                            //            {
                            //                ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                            //            }
                            //        }
                            //    }
                            //}
                            for (int r = DM.ARAY[ri.IDXF].GetMaxRow()-1; r >= 0 ; r--)
                            {
                                for (int c = 0; c < DM.ARAY[ri.IDXF].GetMaxCol(); c++) { 
                                    if (!SEQ.Oracle.Insert("insert " + DM.ARAY[ri.IDXF].Chip[c,r].sQuery))
                                    {
                                        ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                                    }
                                }
                            }

                            if (!OM.CmnOptn.bOracleNotWriteVIT) { 
                                if (!SEQ.Oracle.InsertVIT(OM.CmnOptn.sMachinID , 
                                                          LOT.CrntLotData.sEmployeeID , //20180125 SML.FrmLogOn.GetId() ,
                                                          OM.EqpStat.sTraySttTime , 
                                                          DateTime.Now.ToString("HH:mm:ss")))
                                {
                                    ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                                }
                            }
                        }                        
                    }
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntUld)) return false;
                    //if (!IO_GetX(xi.RAIL_TrayDtct4))
                    //{
                    //    ER_SetErr(ei.PKG_Dispr, "Tray Out Sensor Not Detected");
                    //    return true;
                    //}

                    

                    MoveCyl(ci.IDXF_ClampClOp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.IDXF_ClampClOp, fb.Bwd)) return false;
                    MoveCyl(ci.IDXF_ClampUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!CL_Complete(ci.IDXF_ClampUpDn, fb.Bwd)) return false;
                    MoveMotr(mi.IDXF_XFrnt, pv.IDXF_XFrntWait);
                    //if (SEQ.LODR.iLDRSplyCnt == OM.DevInfo.iTRAY_StackingCnt)
                    //{
                    //    if (OM.DevOptn.bUseTopEmptyTray)
                    //    {
                    //        SEQ.LODR.iLDRSplyCnt -= 2;
                    //    }
                    //    else
                    //    {
                    //        SEQ.LODR.iLDRSplyCnt -= 1;
                    //    }
                    //}
                    DM.ARAY[ri.IDXF].SetStat(cs.None);
                    DM.ARAY[ri.OUTZ].SetStat(cs.Good);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.IDXF_XFrnt, pv.IDXF_XFrntWait)) return false;
                    Step.iCycle++;
                    return false;

                case 15:
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.IDXF_ClampUpDn)
            {
                if(_eFwd == fb.Fwd) {
                    if(CL_Complete(ci.IDXF_ClampClOp, fb.Fwd))
                    {
                        sMsg = "Front Index is Close";
                        bRet = false ;
                    }
                }
            }
            else if (_eActr == ci.IDXF_ClampClOp)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            
            else {
                sMsg = "Cylinder " + CL_GetName(_eActr) + " is Not this parts.";
                bRet = false;
            }
            
            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(CL_GetName(_eActr), sMsg);
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

            if (_eMotr == mi.IDXF_XFrnt)
            {
                //if (!MT_GetStopInpos(mi.SSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.SSTG_YGrpr) + "is moving.";
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
            if(Step.iCycle!=0) MT_GoAbsRun(_eMotr , dDstPos);
            else               MT_GoAbsMan(_eMotr , dDstPos);
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
            if (!MT_GetStop(mi.IDXF_XFrnt))return false;

            if (!CL_Complete(ci.IDXF_ClampUpDn))return false;
            if (!CL_Complete(ci.IDXF_ClampClOp))return false;

            return true ;
        }
        //public void SaveLotName(string _sLotName)
        //{
        //    string sLotDataPath = "C:\\Data\\LotName.ini";
        //    CIniFile IniLotDatadSave = new CIniFile(sLotDataPath);

        //    IniLotDatadSave.Save("LotName", "LotName", _sLotName);

        //}
        
        
    };

    
}
