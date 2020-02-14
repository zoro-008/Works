using System;
using COMMON;
using System.Runtime.CompilerServices;

namespace Machine
{
    public class Index : Part
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

        public Index(int _iPartId = 0)
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
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 10000))
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
                    
                    Step.iToStart = 0;
                    return true;


                case 11:
                    
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
                    Step.iToStop++;
                    return false;

                case 11:
                    MoveCyl(ci.INDX_TrayClampClOp, fb.Bwd);
                    
                    Step.iToStop++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.INDX_TrayClampClOp, fb.Bwd)) return false;
                    MoveCyl(ci.INDX_TrayClampBwFw, fb.Bwd);
                    MoveCyl(ci.INDX_TrayFeedFwBw , fb.Bwd);
                    

                    Step.iToStop++;
                    return false;
                
                case 13:
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.INDX_TrayFeedFwBw, fb.Bwd)) return false;
                    MoveCyl(ci.INDX_TrayClampUpDn, fb.Bwd);
                    Step.iToStop++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.INDX_TrayClampUpDn, fb.Bwd)) return false;

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

                bool bDrWork    = DM.ARAY[ri.INDX].GetCntStat(cs.Unknown) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Ready)   != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Work)    != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Analyze) != 0;

                bool bEzWork    = DM.ARAY[ri.INDX].GetCntStat(cs.Unknown       ) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Entering1x1   ) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Aging1x1      ) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.MTFNPS1x1     ) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Calibration1x1) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Skull1x1      ) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Entering2x2   ) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Aging2x2      ) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.MTFNPS2x2     ) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Calibration2x2) != 0 ||
                                  DM.ARAY[ri.INDX].GetCntStat(cs.Skull2x2      ) != 0;
                

                bool isCycleGet     = DM.ARAY[ri.INDX].CheckAllStat(cs.None   ) && DM.ARAY[ri.LODR].IsExist     (cs.Unknown);
                bool isCycleBarcode = DM.ARAY[ri.INDX].CheckAllStat(cs.Barcode);
                bool isCycleOut     = (DM.ARAY[ri.INDX].GetCntStat(cs.WorkEnd) != 0 ||
                                       DM.ARAY[ri.INDX].GetCntStat(cs.Empty) != 0)  && 
                                       DM.ARAY[ri.INDX].GetCntStat(cs.Check) == 0   && (!bDrWork && !bEzWork) &&
                                       DM.ARAY[ri.LODR].IsExist     (cs.Mask   );
                bool isCycleEnd     = DM.ARAY[ri.INDX].CheckAllStat(cs.None   ) && 
                                      (DM.ARAY[ri.LODR].GetCntStat(cs.WorkEnd) != 0 ||
                                       DM.ARAY[ri.LODR].GetCntStat(cs.Empty) != 0) ;
                                          
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleGet     ) { Step.eSeq = sc.Get      ;  }
                else if (isCycleBarcode ) { Step.eSeq = sc.Barcode  ;  }
                else if (isCycleOut     ) { Step.eSeq = sc.Out      ;  }
                else if (isCycleEnd     ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if (DM.ARAY[ri.INDX].CheckAllStat(cs.None) && IO_GetX(xi.INDX_TrayDtct1)) { ER_SetErr(ei.PKG_Unknwn, "인덱스에 알 수 없는 트레이가 감지되었습니다."); return false; }
                if (IO_GetX(xi.INDX_LdrTrayDtct)                                        ) { ER_SetErr(ei.PKG_Unknwn, "트레이 진입구가 감지 중입니다."              ); return false; }
                

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
                default          : Trace("default End"); 
                                   Step.eSeq = sc.Idle;    return false;
                case sc.Idle     :                         return false;
                case sc.Get      : if (!CycleGet      ())  return false; break ;
                case sc.Barcode  : if (!CycleBarcode  ())  return false; break ;
                case sc.Out      : if (!CycleOut      ())  return false; break ;
            }
            
            Trace(sCycle + " End");
            m_CycleTime[(int)Step.eSeq].End();
            Step.eSeq = sc.Idle;
            return false;
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        //위에 부터 작업.
        public bool FindChip(int _iId, out int _iC, out int _iR, out cs _iChipStat) 
        {
            
            _iC = 0; _iR = 0;
            _iChipStat = 0;

            if (_iId == ri.INDX && OM.DevInfo.iMacroType == 0)
            {
                int Col    = -1 ;
                int Row    =  0 ;
                int ColUnknown = -1;
                int ColReady   = -1;
                int ColWork    = -1;
                int ColAnalyze = -1;
                int ColCheck   = -1;

                DM.ARAY[_iId].FindFrstRowCol(cs.Unknown, ref ColUnknown, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Ready  , ref ColReady  , ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Work   , ref ColWork   , ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Analyze, ref ColAnalyze, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Check  , ref ColCheck  , ref Row);

                     if (Col < ColCheck  ) { Col = ColCheck  ; } 
                else if (Col < ColAnalyze) { Col = ColAnalyze; } 
                else if (Col < ColWork   ) { Col = ColWork   ; } 
                else if (Col < ColReady  ) { Col = ColReady  ; } 
                else if (Col < ColUnknown) { Col = ColUnknown; } 

                _iC = Col;
                _iR = 0;

                if ((_iC < 0))
                {
                    _iChipStat = cs.RetFail;
                    return false;
                }
                
                _iChipStat = DM.ARAY[ri.INDX].GetStat(_iC, _iR);
                return true;
            }

            else if (_iId == ri.INDX && OM.DevInfo.iMacroType == 1)
            {
                int Col         = -1 ;
                int Row         =  0 ;
                int ColUnknown  = -1 ;
                int ColEnter1x1 = -1 ;
                int ColAging1x1 = -1 ;
                int ColMTF1x1   = -1 ;
                int ColCalib1x1 = -1 ;
                int ColSkull1x1 = -1 ;
                int ColEnter2x2 = -1 ;
                int ColAging2x2 = -1 ;
                int ColMTF2x2   = -1 ;
                int ColCalib2x2 = -1 ;
                int ColSkull2x2 = -1 ;

                DM.ARAY[_iId].FindFrstRowCol(cs.Unknown       , ref ColUnknown , ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Entering1x1   , ref ColEnter1x1, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Aging1x1      , ref ColAging1x1, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.MTFNPS1x1     , ref ColMTF1x1  , ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Calibration1x1, ref ColCalib1x1, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Skull1x1      , ref ColSkull1x1, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Entering2x2   , ref ColEnter2x2, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Aging2x2      , ref ColAging2x2, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.MTFNPS2x2     , ref ColMTF2x2  , ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Calibration2x2, ref ColCalib2x2, ref Row);
                DM.ARAY[_iId].FindFrstRowCol(cs.Skull2x2      , ref ColSkull2x2, ref Row);
               

                     if (Col < ColSkull2x2) { Col = ColSkull2x2; } 
                else if (Col < ColCalib2x2) { Col = ColCalib2x2; } 
                else if (Col < ColMTF2x2  ) { Col = ColMTF2x2  ; } 
                else if (Col < ColAging2x2) { Col = ColAging2x2; }
                else if (Col < ColEnter2x2) { Col = ColEnter2x2; } 
                else if (Col < ColSkull1x1) { Col = ColSkull1x1; } 
                else if (Col < ColCalib1x1) { Col = ColCalib1x1; } 
                else if (Col < ColMTF1x1  ) { Col = ColMTF1x1  ; } 
                else if (Col < ColAging1x1) { Col = ColAging1x1; }
                else if (Col < ColEnter1x1) { Col = ColEnter1x1; }
                else if (Col < ColUnknown ) { Col = ColUnknown ; } 

                _iC = Col;
                _iR = 0;

                if ((_iC < 0))
                {
                    _iChipStat = cs.RetFail;
                    return false;
                }
                
                _iChipStat = DM.ARAY[ri.INDX].GetStat(_iC, _iR);
                return true;
            }

            if (_iId == ri.LODR)
            {
                int Col    =  0 ;
                int Row    = -1 ;
                int RowUnknown = -1;
                int RowMask    = -1;
                int RowEmpty   = -1;
                int RowWorkEnd = -1;
               
            
                DM.ARAY[_iId].FindFrstRowCol(cs.Unknown, ref Col, ref RowUnknown);
                DM.ARAY[_iId].FindFrstRowCol(cs.Mask   , ref Col, ref RowMask   );
                DM.ARAY[_iId].FindFrstRowCol(cs.Empty  , ref Col, ref RowEmpty  );
               
            
                     if (Row < RowMask   ) { Row = RowMask   ; }
                else if (Row < RowUnknown) { Row = RowUnknown; } 
                else if (Row < RowEmpty  ) { Row = RowEmpty  ; } 
            
                _iC = 0;
                _iR = Row;
            
                if ((/*_iC < 0 || */_iR < 0))
                {
                    _iChipStat = cs.RetFail;
                    return false;
                }
                
                _iChipStat = DM.ARAY[ri.LODR].GetStat(_iC, _iR);
                return true;
            }
            return true;
        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 30000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Trace(sTemp);
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

            if(IO_GetX(xi.INDX_LdrTrayDtct))//false 인지 true인지 확인 후 바꿀것
            {
                ER_SetErr(ei.PRT_OverLoad, "로더 입구에 자재가 감지되었습니다.");
                return true;
            }

            int c, r;
            cs iWorkStat;

            FindChip(ri.INDX, out c, out r, out iWorkStat);

            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    return true ;
            
                case 10:
                    CL_Move(ci.INDX_TrayClampClOp, fb.Bwd);
                    
                    Step.iHome++;
                    return false ;
            
                case 11:
                    if (!CL_Complete(ci.INDX_TrayClampClOp, fb.Bwd)) return false;
                    CL_Move(ci.INDX_TrayClampBwFw, fb.Bwd);
                    CL_Move(ci.INDX_TrayFeedFwBw , fb.Bwd);
                    
                    Step.iHome++;
                    return false ;

                case 12:
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.INDX_TrayFeedFwBw , fb.Bwd)) return false;
                    CL_Move(ci.INDX_TrayClampUpDn, fb.Bwd);
           
                    Step.iHome++;
                    return false ;

                case 13:
                    if (!CL_Complete(ci.INDX_TrayClampUpDn, fb.Bwd)) return false;
                    MT_GoHome(mi.LODR_ZElev);
                    MT_GoHome(mi.INDX_XRail);
                   
                    Step.iHome++;
                    return false;

                case 14:
                    if (!MT_GetHomeDone(mi.LODR_ZElev)) return false;
                    if (!MT_GetHomeDone(mi.INDX_XRail)) return false;
                    MoveMotr(mi.LODR_ZElev, pv.LODR_ZElevWorkStt);
                    if (IO_GetX(xi.INDX_TrayDtct2))
                    {
                        if (iWorkStat != cs.None || iWorkStat != cs.WorkEnd)
                        {
                            SEQ.INDX.MoveMotr(mi.INDX_XRail, pv.INDX_XRailWorkStt, c * OM.DevInfo.dTRAY_PcktPitchX);
                        }
                    }
                    else
                    {
                        MoveMotr(mi.INDX_XRail, pv.INDX_XRailGet);
                    }
                    
                    Step.iHome++;
                    return false;

                case 15:
                    if (!MT_GetStopPos(mi.LODR_ZElev, pv.LODR_ZElevWorkStt)) return false;
                    if (IO_GetX(xi.INDX_TrayDtct2))
                    {
                        if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailWorkStt, c * OM.DevInfo.dTRAY_PcktPitchX)) return false ;
                    }
                    else
                    {
                        if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailGet)) return false;
                    }
                
                    Step.iHome = 0;
                    return true;
            }
        }

        cs iLdrWorkStat = 0;
        public bool CycleGet()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 20000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
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

            int c , r ;

            FindChip(ri.LODR, out c, out r, out iLdrWorkStat);

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.INDX_TrayClampClOp, fb.Bwd);
                    MoveCyl(ci.INDX_TrayFixClOp  , fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.INDX_TrayClampClOp, fb.Bwd)) return false;
                    if (!CL_Complete(ci.INDX_TrayFixClOp  , fb.Bwd)) return false;
                    MoveCyl(ci.INDX_TrayClampBwFw, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                
                case 12:
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Bwd)) return false;
                    IO_SetY(yi.ETC_IonizerPower, true);
                    IO_SetY(yi.ETC_IonizerSol  , true);

                    Step.iCycle++;
                    return false;

                //아래서 씀
                case 13:
                    if (iLdrWorkStat == cs.Unknown)
                    {
                        MoveMotr(mi.LODR_ZElev, pv.LODR_ZElevWorkStt, (r * OM.DevInfo.dLODR_SlotPitch) + 2);
                    }
                    
                    MoveMotr(mi.INDX_XRail, pv.INDX_XRailGet                                    );
                    Step.iCycle++;
                    return false;

                case 14:
                    if (iLdrWorkStat == cs.Unknown)
                    {
                        if (!MT_GetStopPos(mi.LODR_ZElev, pv.LODR_ZElevWorkStt, (r * OM.DevInfo.dLODR_SlotPitch) + 2)) return false;
                    }
                    if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailGet                                   )) return false;
                    MoveCyl(ci.INDX_DoorClOp     , fb.Bwd);
                    MoveCyl(ci.INDX_TrayClampUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.INDX_DoorClOp     , fb.Bwd)) return false;
                    if (!CL_Complete(ci.INDX_TrayClampUpDn, fb.Fwd)) return false;
                    MoveCyl(ci.INDX_TrayFeedFwBw, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (IO_GetX(xi.INDX_Overload))
                    {
                        MoveCyl(ci.INDX_TrayFeedFwBw, fb.Bwd);
                        ER_SetErr(ei.PRT_OverLoad, "Feeder 이동 중 Overload 센서가 감지되었습니다.");
                        return true;
                    }
                    if (!CL_Complete(ci.INDX_TrayFeedFwBw, fb.Fwd)) return false;
                    MoveCyl(ci.INDX_TrayClampBwFw, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 17:
                    if (IO_GetX(xi.INDX_Overload))
                    {
                        MoveCyl(ci.INDX_TrayClampBwFw, fb.Bwd);
                        ER_SetErr(ei.PRT_OverLoad, "클램프 전진 중 Overload 센서가 감지되었습니다.");
                        return true;
                    }
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Fwd)) return false;
                    
                    if (IO_GetX(xi.INDX_FeedTrayDtct))
                    {
                        MoveCyl(ci.INDX_TrayClampClOp, fb.Fwd);
                    }
                    else
                    {
                        //로더에 찝으러 갔는데 트레이가 없다.
                        Step.iCycle = 50;
                        return false;
                        
                    }
                   
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!CL_Complete(ci.INDX_TrayClampClOp, fb.Fwd)) return false;
                    MoveCyl(ci.INDX_TrayFeedFwBw, fb.Bwd);
                    if (iLdrWorkStat == cs.Unknown)
                    {
                        DM.ARAY[ri.LODR].SetStat(c, r, cs.Mask);
                    }
                    
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!CL_Complete(ci.INDX_TrayFeedFwBw, fb.Bwd)) return false;
                    MoveCyl(ci.INDX_TrayClampClOp, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!CL_Complete(ci.INDX_TrayClampClOp, fb.Bwd)) return false;
                    IO_SetY(yi.ETC_IonizerPower, false);
                    IO_SetY(yi.ETC_IonizerSol  , false);
                    MoveCyl(ci.INDX_TrayClampBwFw, fb.Bwd);
                    if (!OM.MstOptn.bIdleRun && !IO_GetX(xi.INDX_TrayDtct2))
                    {
                        ER_SetErr(ei.PKG_Dispr, "인덱스에 트레이가 감지되지 않습니다.");
                        return true;
                    }
                    
                    DM.ARAY[ri.INDX].ChangeStat(cs.None, cs.Barcode);
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Bwd)) return false;
                    MoveCyl(ci.INDX_TrayClampUpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 22:
                    if (!CL_Complete(ci.INDX_TrayClampUpDn, fb.Bwd)) return false;
                    MoveCyl(ci.INDX_TrayFixClOp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 23:
                    if (!CL_Complete(ci.INDX_TrayFixClOp, fb.Fwd)) return false;
                    MoveCyl(ci.INDX_DoorClOp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 24:
                    if (!CL_Complete(ci.INDX_DoorClOp, fb.Fwd)) return false;

                    Step.iCycle = 0;
                    return true;
                
                //로더에 찝으러 갔는데 없다
                case 50:
                    if (iLdrWorkStat == cs.Unknown)
                    {
                        DM.ARAY[ri.LODR].SetStat(c, r, cs.Empty);
                    }

                    MoveCyl(ci.INDX_TrayClampBwFw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 51:
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Bwd)) return false;
                    Step.iCycle = 0;
                    return true;

            }
        }

        public bool bTrayWorkTimer ;
        public bool CycleBarcode()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
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
            
            LOT.TLot Lot ;



            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    OM.EqpStat.sBarcode = "";
                    if (OM.CmnOptn.bBarcodeSkip || OM.MstOptn.bIdleRun)
                    {
                        DM.ARAY[ri.INDX].SetStat(cs.Unknown);
                        OM.EqpStat.sBarcode = "";
                        Step.iCycle = 0;
                        return true;
                    }
                    MoveMotr(mi.INDX_XRail, pv.INDX_XRailGet);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailGet)) return false;
                    MoveMotr(mi.INDX_XRail, pv.INDX_XRailBarcode);
                    
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailBarcode)) return false;
                    SEQ.Barcord.SendScanOn();
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 13:
                    if (m_tmDelay.OnDelay(3000))
                    {//바코드 못읽었을때 상황.
                        Step.iCycle = 60;
                        return false;
                    }
                    if (SEQ.Barcord.GetText() == "") return false;
                    
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!m_tmDelay.OnDelay(100)) return false ;
                   
                    OM.EqpStat.sBarcode = SEQ.Barcord.GetText();
                    SPC.LOT.Data.Barcode = SEQ.Barcord.GetText();
                    MoveMotr(mi.INDX_XRail, pv.INDX_XRailWorkStt);
                                   
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailWorkStt)) return false;
                    DM.ARAY[ri.INDX].SetStat(cs.Unknown);
                    Step.iCycle = 0;
                    return true;

                //바코드 못읽었을때 상황.
                case 60:
                    OM.EqpStat.sBarcode  = "Not Barcode";
                    SPC.LOT.Data.Barcode = "Not Barcode";

                    Step.iCycle++;
                    return false;

                case 61:
                    MoveMotr(mi.INDX_XRail, pv.INDX_XRailWorkStt);

                    Step.iCycle++;
                    return false;

                case 62:
                    if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailWorkStt)) return false;
                    DM.ARAY[ri.INDX].SetStat(cs.Unknown);

                    Step.iCycle = 0;
                    return true;

                    
                //아이들 러닝용 위애서 씀.
                case 100:
                    Step.iCycle++;
                    return false ;

                case 101:
                    Step.iCycle++;
                    return false;

                case 102:
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
                sTemp = m_sPartName + sTemp;
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

            int c, r;
            FindChip(ri.LODR, out c, out r, out iLdrWorkStat);
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.INDX_TrayClampClOp, fb.Bwd);
                    MoveCyl(ci.INDX_TrayClampBwFw, fb.Bwd);
                    MoveCyl(ci.INDX_TrayFeedFwBw , fb.Bwd);
                    MoveCyl(ci.INDX_DoorClOp     , fb.Bwd);
                    SEQ.XRYD.MoveCyl(ci.XRAY_LeftUSBFwBw  , fb.Bwd);
                    SEQ.XRYD.MoveCyl(ci.XRAY_RightUSBFwBw , fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.INDX_TrayClampClOp, fb.Bwd)) return false;
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.INDX_TrayFeedFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.INDX_DoorClOp     , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw  , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw , fb.Bwd)) return false;
                    
                    MoveCyl(ci.INDX_TrayClampUpDn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.INDX_TrayClampUpDn, fb.Bwd)) return false;
                    if (iLdrWorkStat == cs.Mask)
                    {
                        MoveMotr(mi.LODR_ZElev, pv.LODR_ZElevWorkStt, (r * OM.DevInfo.dLODR_SlotPitch) - 1);
                    }
                    MoveMotr(mi.INDX_XRail, pv.INDX_XRailGet);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (iLdrWorkStat == cs.Mask)
                    {
                        if (!MT_GetStopPos(mi.LODR_ZElev, pv.LODR_ZElevWorkStt, (r * OM.DevInfo.dLODR_SlotPitch) - 1)) return false;
                    }
                    
                    if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailGet                                          )) return false;

                    MoveCyl(ci.INDX_TrayFixClOp, fb.Bwd);

                    Step.iCycle++;
                    return false ;

                case 14:
                    if (!CL_Complete(ci.INDX_TrayFixClOp, fb.Bwd)) return false;
                    MoveCyl(ci.INDX_TrayClampUpDn, fb.Fwd);
                    
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.INDX_TrayClampUpDn, fb.Fwd)) return false;
                    MoveCyl(ci.INDX_TrayClampBwFw, fb.Fwd);
                    
                    Step.iCycle++;
                    return false;


                case 16:
                    if(IO_GetX(xi.INDX_Overload))
                    {
                        MoveCyl(ci.INDX_TrayClampBwFw, fb.Bwd);
                        ER_SetErr(ei.PRT_OverLoad, "클램프 전진 중 Overload가 감지되었습니다.");
                        return true;
                    }
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Fwd)) return false;
                    MoveCyl(ci.INDX_TrayClampClOp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!CL_Complete(ci.INDX_TrayClampClOp, fb.Fwd)) return false;
                    MoveCyl(ci.INDX_TrayFeedFwBw, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 18:
                    if(IO_GetX(xi.INDX_Overload))
                    {
                        IO_SetY(yi.INDX_TrayFeedFw, false);
                        ER_SetErr(ei.PRT_OverLoad, "Feeder Overload가 감지되었습니다.");
                        return true;
                    }
                    if (!CL_Complete(ci.INDX_TrayFeedFwBw, fb.Fwd)) return false;
                    DM.ARAY[ri.INDX].SetStat(cs.None);
                    MoveCyl(ci.INDX_TrayClampClOp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!CL_Complete(ci.INDX_TrayClampClOp, fb.Bwd)) return false;
                    //그냥 Bwd하면 트레이 딸려나와서 올려줌
                    if (iLdrWorkStat == cs.Mask)
                    {
                        MoveMotr(mi.LODR_ZElev, pv.LODR_ZElevWorkStt, (r * OM.DevInfo.dLODR_SlotPitch) +2);
                    }

                    Step.iCycle++;
                    return false;

                case 20:
                    if (iLdrWorkStat == cs.Mask)
                    {
                        if (!MT_GetStopPos(mi.LODR_ZElev, pv.LODR_ZElevWorkStt, (r * OM.DevInfo.dLODR_SlotPitch) + 2)) return false;
                    }

                    MoveCyl(ci.INDX_TrayClampBwFw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 21:
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Bwd)) return false;
                    if (iLdrWorkStat == cs.Mask)
                    {
                        DM.ARAY[ri.LODR].SetStat(c, r, cs.WorkEnd);
                    }
                    if (DM.ARAY[ri.LODR].GetStat(c, OM.DevInfo.iLODR_SlotCnt - 1) == cs.WorkEnd ||
                        DM.ARAY[ri.LODR].GetStat(c, OM.DevInfo.iLODR_SlotCnt - 1) == cs.Empty )
                    {
                        Step.iCycle = 30;
                        return false;
                    }
                    
                    Step.iCycle = 0;
                    return true;

                case 30:
                    MoveCyl(ci.INDX_TrayFeedFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 31:
                    if (!CL_Complete(ci.INDX_TrayFeedFwBw, fb.Bwd)) return false;
                    MoveCyl(ci.INDX_TrayClampUpDn, fb.Bwd);
                    MoveMotr(mi.LODR_ZElev, pv.LODR_ZElevRefill);
                    Step.iCycle++;
                    return false;

                case 32:
                    if (!CL_Complete(ci.INDX_TrayClampUpDn, fb.Bwd)) return false;
                    if (!MT_GetStopPos(mi.LODR_ZElev, pv.LODR_ZElevRefill)) return false;

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.INDX_DoorClOp){
                if(_eFwd == fb.Fwd) {
                    if(CL_Complete(ci.INDX_TrayFeedFwBw, fb.Fwd))
                    {
                        sMsg = "Feeder가 전진해 있습니다.";
                        bRet = false ;
                    }
                }
            }
            else if (_eActr == ci.INDX_TrayClampClOp)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.INDX_TrayClampBwFw)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.INDX_TrayClampUpDn)
            {
                if(_eFwd == fb.Fwd) 
                {
                    if (!CL_Complete(ci.INDX_TrayClampBwFw, fb.Bwd))
                    {
                        sMsg = "클램프 상승 시 충돌 위험이 있습니다.";
                        bRet = false;
                    }
                }
            }
            else if (_eActr == ci.INDX_TrayFeedFwBw)
            {
                if (_eFwd == fb.Fwd)
                {
                    if (CL_Complete(ci.INDX_DoorClOp, fb.Fwd))
                    {
                        sMsg = "Feeding Door가 닫혀있습니다.";
                        bRet = false;
                    }
                }
            }
            else if (_eActr == ci.INDX_TrayFixClOp)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
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

            if (_eMotr == mi.LODR_ZElev)
            {
                //if (!CL_Complete(ci.INDX_TrayClampUpDn, fb.Bwd))
                //{
                //    sMsg = MT_GetName(mi.INDX_XRail) + "모터와 클램프가 충돌 위치에 있습니다.";
                //    bRet = false;
                //}
            }

            else if (_eMotr == mi.INDX_XRail)
            {
                if (!CL_Complete(ci.INDX_TrayClampUpDn, fb.Bwd))
                {
                    sMsg = MT_GetName(mi.INDX_XRail) + "모터와 클램프가 충돌 위치에 있습니다.";
                    bRet = false;
                }
                if (CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Fwd))
                {
                    sMsg = "Left USB Connector가 전진 상태입니다.";
                    bRet = false;
                }
                if (CL_Complete(ci.XRAY_RightUSBFwBw, fb.Fwd))
                {
                    sMsg = "Right USB Connector가 전진 상태입니다.";
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
            if (!MT_GetStop(mi.INDX_XRail)) return false;

            if (!CL_Complete(ci.INDX_TrayFixClOp  )) return false;
            if (!CL_Complete(ci.INDX_TrayClampClOp)) return false;
            if (!CL_Complete(ci.INDX_TrayClampUpDn)) return false;
            if (!CL_Complete(ci.INDX_TrayClampBwFw)) return false;
            if (!CL_Complete(ci.INDX_TrayFeedFwBw )) return false;
            if (!CL_Complete(ci.INDX_DoorClOp     )) return false;

            return true;
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
