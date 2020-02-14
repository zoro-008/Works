using System;
using COMMON;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;

namespace Machine
{
    public class XRayEz : Part
    {
        //Part Event 
        public delegate void CMacReset  (); //델리게이트 선언
        public event CMacReset MacReset   ; //델리게이트 이벤트 선언       
        
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
            Idle      = 0,
            Connect      ,//Dressy, Ez 공용
            Entering     ,//Ez 전용
            Aging        ,//Ez 전용
            MTFNPS       ,//Ez 전용
            Calibration  ,//Ez 전용
            Skull        ,//Ez 전용
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
        public bool     bDeviceChange;
        public bool     bLeftCnct     ;
        public bool     bRightCnct    ;

        public EzSensor.SSetting MacroSet = new EzSensor.SSetting();

        public XRayEz(int _iPartId = 0)
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

            MacReset += new CMacReset(SEQ.Mcr.Reset);

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

            iGetImgStep = 0;
            iWorkStep = 0;
            
            MacReset();
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
                    iWorkStep = 0;
                    MoveMotr(mi.XRAY_ZXRay , pv.XRAY_ZXRayFltrMove);
                    IO_SetY(yi.XRAY_XRayOn, false);
                    Step.iToStop++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);
                    
                    Step.iToStop++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

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

                bool bNone    = DM.ARAY[ri.INDX].CheckAllStat(cs.None   );
                bool bBarcode = DM.ARAY[ri.INDX].CheckAllStat(cs.Barcode);
                bool bWork    = DM.ARAY[ri.INDX].GetCntStat(cs.Unknown       ) != 0 ||
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
                                
                bool isCycleConnect    = (!bNone && !bBarcode && bWork &&
                                         DM.ARAY[ri.INDX].GetCntStat(cs.Check) == 0)  &&
                                         ML.CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Bwd) && 
                                         ML.CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd); //USB가 커넥팅 안되있으면 무조건 탄다.
                bool isCycleEntering   = DM.ARAY[ri.INDX].GetCntStat(cs.Entering1x1   ) != 0 ||
                                         DM.ARAY[ri.INDX].GetCntStat(cs.Entering2x2   ) != 0 ;
                bool isCycleAging      = DM.ARAY[ri.INDX].GetCntStat(cs.Aging1x1      ) != 0 ||
                                         DM.ARAY[ri.INDX].GetCntStat(cs.Aging2x2      ) != 0 ;
                bool isCycleMTFNPS     = DM.ARAY[ri.INDX].GetCntStat(cs.MTFNPS1x1     ) != 0 ||
                                         DM.ARAY[ri.INDX].GetCntStat(cs.MTFNPS2x2     ) != 0 ;
                bool isCycleCalibration= DM.ARAY[ri.INDX].GetCntStat(cs.Calibration1x1) != 0 ||
                                         DM.ARAY[ri.INDX].GetCntStat(cs.Calibration2x2) != 0 ;
                bool isCycleSkull      = DM.ARAY[ri.INDX].GetCntStat(cs.Skull1x1      ) != 0 ||
                                         DM.ARAY[ri.INDX].GetCntStat(cs.Skull2x2      ) != 0 ;
                bool isCycleEnd     = DM.ARAY[ri.INDX].CheckAllStat(cs.None);
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleConnect    ) { Step.eSeq     = sc.Connect           ; }
                else if (isCycleEntering   ) { Step.eSeq     = sc.Entering          ; }
                else if (isCycleAging      ) { Step.eSeq     = sc.Aging             ; }
                else if (isCycleMTFNPS     ) { Step.eSeq     = sc.MTFNPS            ; }
                else if (isCycleCalibration) { Step.eSeq     = sc.Calibration       ; }
                else if (isCycleSkull      ) { Step.eSeq     = sc.Skull             ; }
                else if (isCycleEnd        ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

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
                case sc.Connect      : if (!CycleConnect    ())  return false; break ;
                case sc.Entering     : if (!CycleEntering   ())  return false; break ;
                case sc.Aging        : if (!CycleAging      ())  return false; break ;
                case sc.MTFNPS       : if (!CycleMTFNPS     ())  return false; break ;
                case sc.Calibration  : if (!CycleCalibration())  return false; break ;
                case sc.Skull        : if (!CycleSkull      ())  return false; break ;
            }
            
            Trace(sCycle + " End");
            m_CycleTime[(int)Step.eSeq].End();
            Step.eSeq = sc.Idle;
            return false;
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
            int ColNG     = -1 ; int RowNG     = -1 ;
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

            _iChipStat = DM.ARAY[ri.INDX].GetStat(_iC , _iR);
            _bFlying   = DM.ARAY[ri.INDX].GetCntColStat(_iC, cs.Barcode) != 0;

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

            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    return true ;

                case 10:
                    CL_Move(ci.XRAY_LeftUSBFwBw , fb.Bwd);
                    CL_Move(ci.XRAY_RightUSBFwBw, fb.Bwd);
                    MT_GoHome(mi.XRAY_ZXRay);

                    Step.iHome++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;
                    if (!MT_GetHomeDone(mi.XRAY_ZXRay)) return false;
                    CL_Move(ci.XRAY_Filter1Dn, fb.Bwd);
                    CL_Move(ci.XRAY_Filter2Dn, fb.Bwd);
                    CL_Move(ci.XRAY_Filter3Dn, fb.Bwd);
                    CL_Move(ci.XRAY_Filter4Dn, fb.Bwd);
                    CL_Move(ci.XRAY_Filter5Dn, fb.Bwd);
                    CL_Move(ci.XRAY_Filter6Dn, fb.Bwd);
                    CL_Move(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;
                    MT_GoHome(mi.XRAY_XFltr);
                    MT_GoHome(mi.XRAY_XCnct);

                    Step.iHome++;
                    return false;

                case 13:
                    if (!MT_GetHomeDone(mi.XRAY_XFltr)) return false;
                    if (!MT_GetHomeDone(mi.XRAY_XCnct)) return false;
                    MoveMotr(mi.XRAY_XCnct, pv.XRAY_XCnctLeftWork);
                    MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work   );
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);

                    Step.iHome++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.XRAY_XCnct, pv.XRAY_XCnctLeftWork)) return false;
                    if (!MT_GetStopPos(mi.XRAY_XFltr, pv.XRAY_XFltr1Work   )) return false;
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;

                    Step.iHome = 0;
                    return true;
            }
        }




        public cs iWorkStat;
        int iC = 0;
        public bool CycleConnect()
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
            
            int c, r;

            SEQ.INDX.FindChip(ri.INDX, out c, out r, out iWorkStat);

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    

                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Bwd);
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;

                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);
                
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    if (iWorkStat == cs.Entering1x1   ) iC = c;
                    if (iWorkStat == cs.Aging1x1      ) iC = c;
                    if (iWorkStat == cs.MTFNPS1x1     ) iC = c;
                    if (iWorkStat == cs.Calibration1x1) iC = c;
                    if (iWorkStat == cs.Skull1x1      ) iC = c;
                    if (iWorkStat == cs.Entering2x2   ) iC = c;
                    if (iWorkStat == cs.Aging2x2      ) iC = c;
                    if (iWorkStat == cs.MTFNPS2x2     ) iC = c;
                    if (iWorkStat == cs.Calibration2x2) iC = c;
                    if (iWorkStat == cs.Skull2x2      ) iC = c;
                    if (iWorkStat == cs.Unknown       ) iC = c;
                    if (iWorkStat == cs.Empty         ) iC = c;
                    
                    SEQ.INDX.MoveMotr(mi.INDX_XRail, pv.INDX_XRailWorkStt,  iC * OM.DevInfo.dTRAY_PcktPitchX);//검사 진행 방향은 왼쪽 자재가 1번

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailWorkStt, iC * OM.DevInfo.dTRAY_PcktPitchX)) return false;
                    
                    
                    if (OM.DevInfo.iUseUSBOptn == 0) //Left Only
                    {
                        MoveMotr(mi.XRAY_XCnct, pv.XRAY_XCnctLeftWork);
                    }
                    else if (OM.DevInfo.iUseUSBOptn == 1) //Right Only
                    {
                        MoveMotr(mi.XRAY_XCnct, pv.XRAY_XCnctRightWork);
                    }
                    else //Left -> Right
                    {
                        Step.iCycle = 30;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStop   (mi.XRAY_XCnct                    )) return false;
                  
                    if (OM.DevInfo.iUseUSBOptn == 0) //Left Only
                    {
                        MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Fwd);
                    }
                    else if (OM.DevInfo.iUseUSBOptn == 1) //Right Only
                    {
                        MoveCyl(ci.XRAY_RightUSBFwBw, fb.Fwd);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if (OM.DevInfo.iUseUSBOptn == 0) //Left Only
                    {
                        if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Fwd)) return false;
                        if (!m_tmDelay.OnDelay(3000)) return false;
                        if (OM.CmnOptn.bIgnrCnctErr && !bLeftCnct)
                        {
                            m_tmDelay.Clear();
                            Step.iCycle = 20;
                            return false;
                        }
                        else if (!OM.CmnOptn.bIgnrCnctErr && !bLeftCnct)
                        {
                            MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Bwd);
                            ER_SetErr(ei.ETC_UsbDisConnect, "좌측 USB 연결확인이 되지 않았습니다.");
                            return true;
                        }
                        
                    }
                    else if (OM.DevInfo.iUseUSBOptn == 1)
                    {
                        if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Fwd)) return false;
                        if (!m_tmDelay.OnDelay(3000)) return false;
                        if (OM.CmnOptn.bIgnrCnctErr && !bRightCnct)
                        {
                            m_tmDelay.Clear();
                            Step.iCycle = 20;
                            return false;
                        }
                        else if (!OM.CmnOptn.bIgnrCnctErr && !bRightCnct)
                        {
                            MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                            ER_SetErr(ei.ETC_UsbDisConnect, "우측 USB 연결확인이 되지 않았습니다.");
                            return true;
                        }
                    }
                    if (iWorkStat == cs.Unknown)
                    {
                        if (OM.EzSensorInfo.iImgSize == 0 || OM.EzSensorInfo.iImgSize == 2)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Entering1x1);
                        }
                        else if (OM.EzSensorInfo.iImgSize == 1)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Entering2x2);
                        }
                        
                    }
                    

                    Step.iCycle = 0; //매크로 돌리는 부분
                    return true;

                //Left Only, Right Only 때 Connect Error Skip 할때 쓰는 부분
                case 20:
                    if (OM.DevInfo.iUseUSBOptn == 0) //Left Only
                    {
                        if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Fwd)) return false;
                        if (!m_tmDelay.OnDelay(3000)) return false;
                        if (!bLeftCnct)
                        {
                            MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Bwd);
                        }
                    }
                    else if (OM.DevInfo.iUseUSBOptn == 1)
                    {
                        if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Fwd)) return false;
                        if (!m_tmDelay.OnDelay(3000)) return false;
                        if (!bRightCnct)
                        {
                            MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                        }
                    }
                    if (iWorkStat == cs.Unknown)
                    {
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.Empty);
                    }
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;

                    Step.iCycle = 0; 
                    return true;

                //레프트 꼽았을때 문제있으면 라이트 꼽는 부분
                case 30:
                    MoveMotr(mi.XRAY_XCnct, pv.XRAY_XCnctLeftWork);

                    Step.iCycle++;
                    return false;

                case 31:
                    if (!MT_GetStopPos(mi.XRAY_XCnct, pv.XRAY_XCnctLeftWork)) return false;
                    MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 32:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Fwd)) return false;
                    if (!m_tmDelay.OnDelay(2000)) return false;
                    if (bLeftCnct) //레프트 USB 연결 되면 매크로 돌리고, 연결안되면 다음 사이클로 넘어간다.
                    {
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.Ready);
                        Step.iCycle = 0;
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 33:
                    MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 34:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Bwd)) return false;
                    MoveMotr(mi.XRAY_XCnct, pv.XRAY_XCnctRightWork);

                    Step.iCycle++;
                    return false;

                case 35:
                    if (!MT_GetStopPos(mi.XRAY_XCnct, pv.XRAY_XCnctRightWork)) return false;
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 36:
                    
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Fwd)) return false;
                    if (!m_tmDelay.OnDelay(1500)) return false;
                    if (OM.CmnOptn.bIgnrCnctErr && !bRightCnct)
                    {
                        Step.iCycle++;
                        return false;
                    }
                    if (!OM.CmnOptn.bIgnrCnctErr && !bRightCnct) //라이트 USB 연결안되면 에러, 연결되면 매크로 돌린다. 사이클로 넘어간다.
                    {
                        MoveCyl(ci.XRAY_LeftUSBFwBw , fb.Bwd);
                        MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                        ER_SetErr(ei.ETC_UsbDisConnect, "좌측과 우측 USB 연결 확인이 되지 않습니다.");
                        return true;
                    }
                    if(iWorkStat == cs.Unknown)
                    {
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.Entering1x1);
                    }
                        
                    Step.iCycle = 0;
                    return true;

                case 37:
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Fwd)) return false;
                    if (!m_tmDelay.OnDelay(2000)) return false;
                    if (!bRightCnct) //라이트 USB 연결안되면 에러, 연결되면 매크로 돌린다. 사이클로 넘어간다.
                    {
                        MoveCyl(ci.XRAY_LeftUSBFwBw , fb.Bwd);
                        MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                    }
                    if(iWorkStat == cs.Unknown)
                    {
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.Empty);
                    }
                    Step.iCycle++;
                    return false;

                case 38:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;

                    Step.iCycle = 0;
                    return true;

            }
        }

        double dStartTime1x1 = 0.0;
        double dStartTime2x2 = 0.0;
        public bool bFindSerialEnd = false;
        public bool CycleEntering()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
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
                SEQ.Mcr.Stop();
                return true;
            }
            
            int c, r;
            SEQ.INDX.FindChip(ri.INDX, out c, out r, out iWorkStat);

            DM.ARAY[ri.INDX].Step = 0;

            
            MacroSet.smA   = OM.EzSensor[0].dEzEntrXmA  .ToString()   ;
            MacroSet.sKvp  = OM.EzSensor[0].dEzEntrXKvp .ToString()   ;
            MacroSet.sTime = OM.EzSensor[0].dEzEntrXTime.ToString()   ;
            
           
            if(iWorkStat == cs.Entering1x1)
            {
                MacroSet.iWidth   = OM.EzSensorInfo.iEntr1x1Width;
                MacroSet.iHeight  = OM.EzSensorInfo.iEntr1x1Hght ;
            
                MacroSet.iBinning = OM.EzSensorInfo.iBinning1x1  ;
                MacroSet.sCalFolder = "CAL_A";
                MacroSet.sSaveFolder = "1x1";
                MacroSet.iCutoffbevel = OM.EzSensorInfo.iCutoffbevel1x1;
            }
            else if(iWorkStat == cs.Entering2x2)
            {
                MacroSet.iWidth   = OM.EzSensorInfo.iEntr2x2Width;
                MacroSet.iHeight  = OM.EzSensorInfo.iEntr2x2Hght ;
            
                MacroSet.iBinning = OM.EzSensorInfo.iBinning2x2  ;
                MacroSet.sCalFolder = "CAL";
                MacroSet.sSaveFolder = "2x2";
                MacroSet.iCutoffbevel = OM.EzSensorInfo.iCutoffbevel2x2;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    OM.EqpStat.dStartTime   = 0;
                    OM.EqpStat.dEndTime     = 0;
                    OM.EqpStat.sSerialList  = "";
                    OM.EqpStat.sWaferNo     = "";
                    OM.EqpStat.sFosNo       = "";
                    OM.EqpStat.sBdNo        = "";
                    OM.EqpStat.sDarkOfs     = "";
                    bFindSerialEnd          = false; //시리얼넘버 찾는거 완료 됐는지 확인하는 플래그
                    if (iWorkStat == cs.Entering1x1 && (OM.EzSensorInfo.iImgSize == 0 || OM.EzSensorInfo.iImgSize == 2))
                    {
                        dStartTime1x1              = DateTime.Now.ToOADate();
                        OM.EqpStat.dStartTime1x1   = dStartTime1x1;
                    }
                    if (iWorkStat == cs.Entering2x2 && OM.EzSensorInfo.iImgSize == 1)
                    {
                        dStartTime2x2              = DateTime.Now.ToOADate();
                        OM.EqpStat.dStartTime2x2   = dStartTime2x2;
                    }

                    if (OM.CmnOptn.bSkipEntr)
                    {
                        if (iWorkStat == cs.Entering1x1)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Aging1x1);
                        }
                        else if (iWorkStat == cs.Entering2x2)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Aging2x2);
                        }
                        Step.iCycle = 0;
                        return true;
                    }
                   
                    if (OM.MstOptn.bIdleRun)
                    {
                        if (iWorkStat == cs.Ready) DM.ARAY[ri.INDX].SetStat(c, r, cs.Work);
                        Step.iCycle = 0;
                        return true;
                    }
                    if (!bLeftCnct && !bRightCnct)
                    {
                        ER_SetErr(ei.ETC_UsbDisConnect, "USB 연결 확인이 되지 않습니다.");
                        return true;
                    }

                    Step.iCycle++;
                    return false;

                case 11:
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    Step.iCycle++;
                    return false;

                case 14:
                         if (OM.EzSensor[0].iEzEntrFltr == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work); //해당 필터 넘버 받아서 그쪽으로 모터 넘김
                    else if (OM.EzSensor[0].iEzEntrFltr == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    else if (OM.EzSensor[0].iEzEntrFltr == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    else if (OM.EzSensor[0].iEzEntrFltr == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    else if (OM.EzSensor[0].iEzEntrFltr == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    else if (OM.EzSensor[0].iEzEntrFltr == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    else if (OM.EzSensor[0].iEzEntrFltr == 7) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MT_GetStop(mi.XRAY_XFltr)) return false;
                         if (OM.EzSensor[0].iEzEntrFltr == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd); //해당 필터 넘버 받아서 그쪽으로 모터 넘김
                    else if (OM.EzSensor[0].iEzEntrFltr == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    else if (OM.EzSensor[0].iEzEntrFltr == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    else if (OM.EzSensor[0].iEzEntrFltr == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    else if (OM.EzSensor[0].iEzEntrFltr == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    else if (OM.EzSensor[0].iEzEntrFltr == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    else if (OM.EzSensor[0].iEzEntrFltr == 7) MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn)) return false;

                    Step.iCycle++;
                    return false;

                case 17:
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay,  pv.XRAY_ZXRayWork)) return false;
                    if (!m_tmDelay.OnDelay(OM.EzSensorInfo.iBfMacDelay)) return false;
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    IO_SetY(yi.XRAY_XRayOn, true);
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    Step.iCycle++;
                    return false;
                case 19:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        IO_SetY(yi.XRAY_XRayOn, false);
                        ML.ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    if (iWorkStat == cs.Entering1x1 || iWorkStat == cs.Entering2x2)
                    {
                        if (OM.DevInfo.iMacroType == 1 && SEQ.Mcr.Ez1.bDetectSerial)
                        {
                            if (FindSerialNo())
                            {
                                Step.iCycle++;
                                return false;
                            }
                            else if (!FindSerialNo() && OM.CmnOptn.bIgnrSerialErr)//시리얼 매칭 에러 무시 사용하면 자재 시리얼만 입력하고 다음 자재
                            {
                                Step.iCycle = 80;
                                return false;
                            }
                            else if (!FindSerialNo() && !OM.CmnOptn.bIgnrSerialErr) //시리얼넘비 매칭 에러 사용 시 여기 탄다.
                            {
                                Step.iCycle = 90;
                                return false;
                            }
                        }
                    }

                    Step.iCycle++;
                    return false;
                  
                case 20:
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.Entering))
                    {
                        Step.iCycle = 19;
                        return false;
                    }
                    IO_SetY(yi.XRAY_XRayOn, false);
                    Step.iCycle++;
                    return false;

                case 21:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCoolingTime)) return false;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 22:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 23://Trigger
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;
                    
                    Step.iCycle++;
                    return false;

                case 24:
                    if(OM.EzSensor[0].iEzTrFltr == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work);
                    if(OM.EzSensor[0].iEzTrFltr == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    if(OM.EzSensor[0].iEzTrFltr == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    if(OM.EzSensor[0].iEzTrFltr == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    if(OM.EzSensor[0].iEzTrFltr == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    if(OM.EzSensor[0].iEzTrFltr == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    if(OM.EzSensor[0].iEzTrFltr == 7) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 25:
                    if(!MT_GetStop(mi.XRAY_XFltr)) return false;
                    if(OM.EzSensor[0].iEzTrFltr == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzTrFltr == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzTrFltr == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzTrFltr == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzTrFltr == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzTrFltr == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzTrFltr == 7) MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 26:
                    if(!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn)) return false;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);

                    Step.iCycle++;
                    return false;

                case 27:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork)) return false;

                    Step.iCycle++;
                    return false;

                case 28://Trigger
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }

                    MacroSet.smA   = OM.EzSensor[0].dEzTrXmA  .ToString()   ;
                    MacroSet.sKvp  = OM.EzSensor[0].dEzTrXKvp .ToString()   ;
                    MacroSet.sTime = OM.EzSensor[0].dEzTrXTime.ToString()   ;

                    IO_SetY(yi.XRAY_XRayOn, true);
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    Step.iCycle++;
                    return false;

                case 29:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    if (SEQ.Mcr.Ez1.bErrDevice)
                    {
                        Step.iCycle = 100;
                        return false;
                    }
                    Step.iCycle++;
                    return false;

                case 30:
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.Trigger))
                    {
                        Step.iCycle = 29;
                        return false;
                    }
                    IO_SetY(yi.XRAY_XRayOn, false);
                    Step.iCycle++;
                    return false;

                case 31:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCoolingTime)) return false;
                    if(iWorkStat == cs.Entering1x1) DM.ARAY[ri.INDX].SetStat(c, r, cs.Aging1x1);
                    if(iWorkStat == cs.Entering2x2) DM.ARAY[ri.INDX].SetStat(c, r, cs.Aging2x2);
                    SEQ.Mcr.Ez1.bDetectSerial = false;
                    
                    Step.iCycle = 0;
                    return true;
                
                //시리얼넘버 매칭 안되면 여기 탄다. 진섭
                case 80:
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);

                    Step.iCycle++;
                    return false;

                case 81:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 82:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    OM.EqpStat.sSerialList += "(Serial No. Fail)";

                    Step.iCycle++;
                    return false;

                case 83:
                    //Ver 2019.10.23.6 여기서 매크로 사이클 다 탔는지 확인하고 USB 제거
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.Entering)) return false;

                    if (iWorkStat == cs.Entering1x1)
                    {
                        dEndTime1x1 = DateTime.Now.ToOADate();
                        OM.EqpStat.dEndTime1x1 = dEndTime1x1;
                    }
                    if (iWorkStat == cs.Entering2x2)
                    {
                        dEndTime2x2 = DateTime.Now.ToOADate();
                        OM.EqpStat.dEndTime2x2 = dEndTime2x2;
                    }

                    Step.iCycle++;
                    return false;

                case 84:
                    SaveCsv(LOT.GetLotNo());
                    MoveCyl(ci.XRAY_LeftUSBFwBw , fb.Bwd);
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 85:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;

                    DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                    SEQ.Mcr.Ez1.bDetectSerial = false;
                    IO_SetY(yi.XRAY_XRayOn, false);
                    Step.iCycle = 0;
                    return true;

                //Serial Matching 에러 떴을때 타는 사이클
                case 90:
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);

                    Step.iCycle++;
                    return false;

                case 91:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 92:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    SEQ.Mcr.Ez1.bDetectSerial = false;
                    IO_SetY(yi.XRAY_XRayOn, false);
                    ER_SetErr(ei.ETC_MacroErr, "시리얼 넘버가 일치하는 라인이 없습니다.");
                    return true;

                //20mm 필터 썼을 때 Trigger 안터지면 여기 탄다
                case 100:
                    IO_SetY(yi.XRAY_XRayOn, false);
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 101:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 102:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;
                    MoveCyl(ci.XRAY_LeftUSBFwBw , fb.Bwd);
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 103:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;

                    SEQ.Mcr.Ez1.bErrDevice = false;
                    OM.EqpStat.sSerialList += "(Trigger Fail)";

                    if (iWorkStat == cs.Entering1x1 && OM.EzSensorInfo.iImgSize == 0) //1x1만 작업
                    {
                        dEndTime1x1 = DateTime.Now.ToOADate();
                        OM.EqpStat.dEndTime1x1 = dEndTime1x1;
                        dCycleTime1x1 = OM.EqpStat.dEndTime1x1 - OM.EqpStat.dStartTime1x1;
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                    }
                    else if (iWorkStat == cs.Entering2x2) //2x2, 1x1 -> 2x2 작업
                    {
                        dEndTime2x2 = DateTime.Now.ToOADate();
                        OM.EqpStat.dEndTime2x2 = dEndTime2x2;
                        dCycleTime2x2 = OM.EqpStat.dEndTime2x2 - OM.EqpStat.dStartTime2x2;
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                    }
                    SaveCsv(LOT.GetLotNo());

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleAging()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 16000))
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
                SEQ.Mcr.Stop();
                return true;
            }

            int c, r;
            SEQ.INDX.FindChip(ri.INDX, out c, out r, out iWorkStat);

            if(iWorkStat == cs.Aging1x1)
            {
                MacroSet.iBinning = OM.EzSensorInfo.iBinning1x1  ;
                                       
                MacroSet.iWidth  = OM.EzSensorInfo.iAg1x1Width;
                MacroSet.iHeight = OM.EzSensorInfo.iAg1x1Hght ;
                MacroSet.sCalFolder = "CAL_A";
                MacroSet.sDarkImgPath = OM.EzSensorInfo.sDarkImgPath1x1;
                MacroSet.sSaveFolder = "1x1";
                MacroSet.iAcqMaxFrame = OM.EzSensorInfo.iAcqMaxFrame1x1;
                MacroSet.iAcqInterval = OM.EzSensorInfo.iAcqInterval1x1;
            }
            else if(iWorkStat == cs.Aging2x2)
            {
                MacroSet.iBinning = OM.EzSensorInfo.iBinning2x2  ;

                MacroSet.iWidth  = OM.EzSensorInfo.iAg2x2Width;
                MacroSet.iHeight = OM.EzSensorInfo.iAg2x2Hght ;
                MacroSet.sCalFolder = "CAL";
                MacroSet.sDarkImgPath = OM.EzSensorInfo.sDarkImgPath2x2;
                MacroSet.sSaveFolder = "2x2";
                MacroSet.iAcqMaxFrame = OM.EzSensorInfo.iAcqMaxFrame2x2;
                MacroSet.iAcqInterval = OM.EzSensorInfo.iAcqInterval2x2;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    if (OM.CmnOptn.bSkipAging)
                    {
                        if (iWorkStat == cs.Aging1x1)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.MTFNPS1x1);
                        }
                        else if (iWorkStat == cs.Aging2x2)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.MTFNPS2x2);
                        }
                        Step.iCycle = 0;
                        return true;
                    }
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;
                    
                    Step.iCycle++;
                    return false;

                case 13:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }

                    Step.iCycle++;
                    return false;

                case 15:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.Aging))
                    {
                        Step.iCycle = 14;
                        return false;
                    }
                    Step.iCycle++;
                    return false;
                    
                case 16:
                    if(iWorkStat == cs.Aging1x1) DM.ARAY[ri.INDX].SetStat(c, r, cs.MTFNPS1x1);
                    if(iWorkStat == cs.Aging2x2) DM.ARAY[ri.INDX].SetStat(c, r, cs.MTFNPS2x2);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public int iGetImgStep = 0; //Get Bright, Get Img 작업 횟수 카운트
        public bool CycleMTFNPS()
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
                SEQ.Mcr.Stop();
                return true;
            }

            int c, r;
            SEQ.INDX.FindChip(ri.INDX, out c, out r, out iWorkStat);

            if(iWorkStat == cs.MTFNPS1x1)
            {
                MacroSet.iBinning = OM.EzSensorInfo.iBinning1x1  ;
                                       
                MacroSet.iWidth  = OM.EzSensorInfo.iAg1x1Width;
                MacroSet.iHeight = OM.EzSensorInfo.iAg1x1Hght ;
                MacroSet.sCalFolder = "CAL_A";
                MacroSet.sSaveFolder = "1x1";
                MacroSet.iDimWidth   = OM.EzSensorInfo.iDimWidth1x1;
                MacroSet.iDimHght    = OM.EzSensorInfo.iDimHght1x1;
                MacroSet.dPixelPitch = OM.EzSensorInfo.dPixelPitch1x1;

                MacroSet.iNPSLeft = OM.EzSensorInfo.iNPSLeft1x1;
                MacroSet.iNPSTop  = OM.EzSensorInfo.iNPSTop1x1 ; 
                MacroSet.iNPSW    = OM.EzSensorInfo.iNPSW1x1   ;   
                MacroSet.iNPSH    = OM.EzSensorInfo.iNPSH1x1   ;   
                                                            
                MacroSet.iMTFLeft = OM.EzSensorInfo.iMTFLeft1x1;
                MacroSet.iMTFTop  = OM.EzSensorInfo.iMTFTop1x1 ; 
                MacroSet.iMTFW    = OM.EzSensorInfo.iMTFW1x1   ;
                MacroSet.iMTFH    = OM.EzSensorInfo.iMTFH1x1   ;   

                if(OM.EzSensorInfo.iEzType == 0)
                {
                    MacroSet.sFlatPath   = "1x1\\SNR\\1.raw"   ;
                    MacroSet.sObjtPath   = "1x1\\MTF\\MTF1.raw";
                }
                else if(OM.EzSensorInfo.iEzType == 1)
                {
                    MacroSet.sFlatPath   = "1x1\\SNR";
                    MacroSet.sObjtPath   = "1x1\\MTF";
                }
            }
            else if(iWorkStat == cs.MTFNPS2x2)
            {
                MacroSet.iBinning = OM.EzSensorInfo.iBinning2x2  ;

                MacroSet.iWidth  = OM.EzSensorInfo.iAg2x2Width;
                MacroSet.iHeight = OM.EzSensorInfo.iAg2x2Hght ;
                MacroSet.sCalFolder = "CAL";
                MacroSet.sSaveFolder = "2x2";
                MacroSet.iDimWidth   = OM.EzSensorInfo.iDimWidth2x2  ;
                MacroSet.iDimHght    = OM.EzSensorInfo.iDimHght2x2   ;
                MacroSet.dPixelPitch = OM.EzSensorInfo.dPixelPitch2x2;

                MacroSet.iNPSLeft = OM.EzSensorInfo.iNPSLeft2x2;
                MacroSet.iNPSTop  = OM.EzSensorInfo.iNPSTop2x2 ; 
                MacroSet.iNPSW    = OM.EzSensorInfo.iNPSW2x2   ;   
                MacroSet.iNPSH    = OM.EzSensorInfo.iNPSH2x2   ;   
                                                            
                MacroSet.iMTFLeft = OM.EzSensorInfo.iMTFLeft2x2;
                MacroSet.iMTFTop  = OM.EzSensorInfo.iMTFTop2x2 ; 
                MacroSet.iMTFW    = OM.EzSensorInfo.iMTFW2x2   ;
                MacroSet.iMTFH    = OM.EzSensorInfo.iMTFH2x2   ; 

                if(OM.EzSensorInfo.iEzType == 0)
                {
                    MacroSet.sFlatPath   = "2x2\\SNR\\1.raw"   ;
                    MacroSet.sObjtPath   = "2x2\\MTF\\MTF1.raw";
                }
                else if(OM.EzSensorInfo.iEzType == 1)
                {
                    MacroSet.sFlatPath   = "2x2\\SNR";
                    MacroSet.sObjtPath   = "2x2\\MTF";
                }
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    if (OM.CmnOptn.bSkipMTF)
                    {
                        if (iWorkStat == cs.MTFNPS1x1)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Calibration1x1);
                        }
                        else if (iWorkStat == cs.MTFNPS2x2)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Calibration2x2);
                        }
                        Step.iCycle = 0;
                        return true;
                    }
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;
                
                case 12:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    Step.iCycle++;
                    return false;

                case 14:
                    if(OM.EzSensor[0].iEzGbFltr1 == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work);
                    if(OM.EzSensor[0].iEzGbFltr1 == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    if(OM.EzSensor[0].iEzGbFltr1 == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    if(OM.EzSensor[0].iEzGbFltr1 == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    if(OM.EzSensor[0].iEzGbFltr1 == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    if(OM.EzSensor[0].iEzGbFltr1 == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    if(OM.EzSensor[0].iEzGbFltr1 == 7) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 15:
                    if(!MT_GetStop(mi.XRAY_XFltr)) return false;
                    if(OM.EzSensor[0].iEzGbFltr1 == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr1 == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr1 == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr1 == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr1 == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr1 == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr1 == 7) MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn)) return false;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);

                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork)) return false;
                    Step.iCycle++;
                    return false;

                case 18://Get Bright1 3회
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    MacroSet.smA   = OM.EzSensor[0].dEzGbXmA1  .ToString()   ;
                    MacroSet.sKvp  = OM.EzSensor[0].dEzGbXKvp1 .ToString()   ;
                    MacroSet.sTime = OM.EzSensor[0].dEzGbXTime1.ToString()   ;
                    IO_SetY(yi.XRAY_XRayOn, true);
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    Step.iCycle++;
                    return false;

                case 19:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 20:
                    
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.GetBright))
                    {
                        Step.iCycle = 19;
                        return false;
                    }
                    IO_SetY(yi.XRAY_XRayOn, false);
                    Step.iCycle++;
                    return false;

                case 21:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCoolingTime)) return false;

                    Step.iCycle++;
                    return false;

                case 22:

                    Step.iCycle++;
                    return false;

                case 23://Tolerance 비교

                    Step.iCycle++;
                    return false;

                case 24:
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 25:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 26://Calibration Generate
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.CalGen))
                    {
                        Step.iCycle = 25;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 27:
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    Step.iCycle++;
                    return false;

                case 28:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 29://Preprocess Generate
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.PreGen))
                    {
                        Step.iCycle = 28;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 30:
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 31:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 32:
                    if(!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    Step.iCycle++;
                    return false;

                case 33:
                    if(OM.EzSensor[0].iEzGiFltr1 == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work);
                    if(OM.EzSensor[0].iEzGiFltr1 == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    if(OM.EzSensor[0].iEzGiFltr1 == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    if(OM.EzSensor[0].iEzGiFltr1 == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    if(OM.EzSensor[0].iEzGiFltr1 == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    if(OM.EzSensor[0].iEzGiFltr1 == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    if(OM.EzSensor[0].iEzGiFltr1 == 7) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 34:
                    if(!MT_GetStop(mi.XRAY_XFltr)) return false;
                    if(OM.EzSensor[0].iEzGiFltr1 == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr1 == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr1 == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr1 == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr1 == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr1 == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr1 == 7) MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 35:
                    if(!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn)) return false;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);

                    Step.iCycle++;
                    return false;

                case 36:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork)) return false;
                    IO_SetY(yi.XRAY_XRayOn, true);

                    Step.iCycle++;
                    return false;

                case 37://Get Image1 3회
                         if(iWorkStat == cs.MTFNPS1x1) MacroSet.sGetImgFdName = "1x1\\SNR";
                    else if(iWorkStat == cs.MTFNPS2x2) MacroSet.sGetImgFdName = "2x2\\SNR";
                    if (OM.EzSensorInfo.iEzType == 0) //EzSensor
                    {
                             if(iGetImgStep == 0) MacroSet.sFileName   = "1";
                        else if(iGetImgStep == 1) MacroSet.sFileName   = "2";
                        else if(iGetImgStep == 2) MacroSet.sFileName   = "3";
                    }
                    else if (OM.EzSensorInfo.iEzType == 1) //BI
                    {
                             if(iGetImgStep == 0) MacroSet.sFileName   = "SNR_6mA_1";
                        else if(iGetImgStep == 1) MacroSet.sFileName   = "SNR_6mA_2";
                        else if(iGetImgStep == 2) MacroSet.sFileName   = "SNR_6mA_3";
                    }

                    MacroSet.smA   = OM.EzSensor[0].dEzGiXmA1  .ToString()   ;
                    MacroSet.sKvp  = OM.EzSensor[0].dEzGiXKvp1 .ToString()   ;
                    MacroSet.sTime = OM.EzSensor[0].dEzGiXTime1.ToString()   ;

                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    
                    Step.iCycle++;
                    return false;

                case 38:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 39:
                    
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.GetImage))
                    {
                        Step.iCycle = 38;
                        return false;
                    }
                    iGetImgStep++;
                    Step.iCycle++;
                    return false;

                case 40://Get Image 3회 작업
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCoolingTime)) return false;
                    if (iGetImgStep < 3)
                    {
                        Step.iCycle = 37;
                        return false;
                    }
                    iGetImgStep = 0;
                    IO_SetY(yi.XRAY_XRayOn, false);
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 41:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 42:
                    if(!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    SEQ.Mcr.CycleEzSensorInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 43://EzP 자재일때

                    Step.iCycle++;
                    return false;

                case 44:

                    Step.iCycle++;
                    return false;

                case 45:
                    if(OM.EzSensor[0].iEzGbFltr2 == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work);
                    if(OM.EzSensor[0].iEzGbFltr2 == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    if(OM.EzSensor[0].iEzGbFltr2 == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    if(OM.EzSensor[0].iEzGbFltr2 == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    if(OM.EzSensor[0].iEzGbFltr2 == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    if(OM.EzSensor[0].iEzGbFltr2 == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    if(OM.EzSensor[0].iEzGbFltr2 == 7) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 46:
                    if(!MT_GetStop(mi.XRAY_XFltr)) return false;
                    if(OM.EzSensor[0].iEzGbFltr2 == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr2 == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr2 == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr2 == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr2 == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr2 == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr2 == 7) MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 47:
                    if(!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn)) return false;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);

                    Step.iCycle++;
                    return false;

                case 48:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork)) return false;
                    MacroSet.smA   = OM.EzSensor[0].dEzGbXmA2  .ToString()   ;
                    MacroSet.sKvp  = OM.EzSensor[0].dEzGbXKvp2 .ToString()   ;
                    MacroSet.sTime = OM.EzSensor[0].dEzGbXTime2.ToString()   ;
                    IO_SetY(yi.XRAY_XRayOn, true);
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    Step.iCycle++;
                    return false;

                case 49:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 50:
                    
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.GetBright))
                    {
                        Step.iCycle = 49;
                        return false;
                    }
                    IO_SetY(yi.XRAY_XRayOn, false);
                    Step.iCycle++;
                    return false;

                case 51:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCoolingTime)) return false;

                    Step.iCycle++;
                    return false;

                case 52:

                    Step.iCycle++;
                    return false;

                case 53://Tolerance 비교

                    Step.iCycle++;
                    return false;

                case 54:
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 55:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 56://Calibration Generate
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.CalGen))
                    {
                        Step.iCycle = 55;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 57:
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    Step.iCycle++;
                    return false;

                case 58:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 59://PreProcess Generate
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.PreGen))
                    {
                        Step.iCycle = 58;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 60:
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 61:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 62:
                    if(!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    if(OM.EzSensor[0].iEzGiFltr2 == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work);
                    if(OM.EzSensor[0].iEzGiFltr2 == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    if(OM.EzSensor[0].iEzGiFltr2 == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    if(OM.EzSensor[0].iEzGiFltr2 == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    if(OM.EzSensor[0].iEzGiFltr2 == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    if(OM.EzSensor[0].iEzGiFltr2 == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    if(OM.EzSensor[0].iEzGiFltr2 == 7) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 63:
                    if(!MT_GetStop(mi.XRAY_XFltr)) return false;
                    if(OM.EzSensor[0].iEzGiFltr2 == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr2 == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr2 == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr2 == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr2 == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr2 == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGiFltr2 == 7) MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 64:
                    if(!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn)) return false;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);

                    Step.iCycle++;
                    return false;

                case 65:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork)) return false;
                         if(iWorkStat == cs.MTFNPS1x1) MacroSet.sGetImgFdName = "1x1\\MTF";
                    else if(iWorkStat == cs.MTFNPS2x2) MacroSet.sGetImgFdName = "2x2\\MTF";

                    IO_SetY(yi.XRAY_XRayOn, true);
                    if (OM.EzSensorInfo.iEzType == 0) //EzSensor
                    {
                             if(iGetImgStep == 0) MacroSet.sFileName   = "MTF1";
                        else if(iGetImgStep == 1) MacroSet.sFileName   = "MTF2";
                        else if(iGetImgStep == 2) MacroSet.sFileName   = "MTF3";
                             
                    }
                    else if (OM.EzSensorInfo.iEzType == 1) //BI
                    {
                             if(iGetImgStep == 0) MacroSet.sFileName   = "MTF_1";
                        else if(iGetImgStep == 1) MacroSet.sFileName   = "MTF_2";
                        else if(iGetImgStep == 2) MacroSet.sFileName   = "MTF_3";
                    }
                    

                    MacroSet.smA   = OM.EzSensor[0].dEzGiXmA2  .ToString()   ;
                    MacroSet.sKvp  = OM.EzSensor[0].dEzGiXKvp2 .ToString()   ;
                    MacroSet.sTime = OM.EzSensor[0].dEzGiXTime2.ToString()   ;

                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    
                    Step.iCycle++;
                    return false;

                case 66:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 67:
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.GetImage))
                    {
                        Step.iCycle = 66;
                        return false;
                    }
                    iGetImgStep++;
                    Step.iCycle++;
                    return false;

                case 68://Get Image 3회 작업
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCoolingTime)) return false;
                    if (iGetImgStep < 3)
                    {
                        Step.iCycle = 65;
                        return false;
                    }
                    iGetImgStep = 0;
                    IO_SetY(yi.XRAY_XRayOn ,false);
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 69:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 70:
                    if(!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    Step.iCycle = 100; //DQE 평가로 이동
                    return false;
                
                //DQE by IEC62220 돌리는 부분
                case 100:
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    
                    Step.iCycle++;
                    return false;

                case 101:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 102:
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.DQE1))
                    {
                        Step.iCycle = 101;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 103:
                    
                    Step.iCycle++;
                    return false;

                case 104:
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    
                    Step.iCycle++;
                    return false;

                case 105:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 106:
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.DQE2))
                    {
                        Step.iCycle = 105;
                        return false;
                    }

                    if(iWorkStat == cs.MTFNPS1x1) DM.ARAY[ri.INDX].SetStat(c, r, cs.Calibration1x1);
                    if(iWorkStat == cs.MTFNPS2x2) DM.ARAY[ri.INDX].SetStat(c, r, cs.Calibration2x2);

                    Step.iCycle = 0;
                    return true;
            }
        }

        
        int iPreFilter  = 0;
        int iCrntFilter = 0;
        public int iWorkStep = 0;
        public bool bGetBrightEnd = false;
        public bool CycleCalibration() //
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
                SEQ.Mcr.Stop();
                return true;
            }

            int c, r;
            SEQ.INDX.FindChip(ri.INDX, out c, out r, out iWorkStat);

            if(iWorkStat == cs.Calibration1x1)
            {
                MacroSet.iBinning = OM.EzSensorInfo.iBinning1x1  ;
                                       
                MacroSet.iWidth  = OM.EzSensorInfo.iAg1x1Width;
                MacroSet.iHeight = OM.EzSensorInfo.iAg1x1Hght ;
                MacroSet.sCalFolder = "CAL_A";
                MacroSet.sSaveFolder = "1x1";

            }
            else if(iWorkStat == cs.Calibration2x2)
            {
                MacroSet.iBinning = OM.EzSensorInfo.iBinning2x2  ;

                MacroSet.iWidth  = OM.EzSensorInfo.iAg2x2Width;
                MacroSet.iHeight = OM.EzSensorInfo.iAg2x2Hght ;
                MacroSet.sCalFolder = "CAL";
                MacroSet.sSaveFolder = "2x2";
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    iGetImgStep = 0;
                    if (OM.CmnOptn.bSkipCalib)
                    {
                        if (iWorkStat == cs.Calibration1x1)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Skull1x1);
                        }
                        else if (iWorkStat == cs.Calibration2x2)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Skull2x2);
                        }
                        Step.iCycle = 0;
                        return true;
                    }
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;
                
                    Step.iCycle = 20;
                    return false;
             
                //Get Birhgt
                case 20:
                    if(OM.EzSensor[0].iEzGbFltr3 == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work);
                    if(OM.EzSensor[0].iEzGbFltr3 == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    if(OM.EzSensor[0].iEzGbFltr3 == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    if(OM.EzSensor[0].iEzGbFltr3 == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    if(OM.EzSensor[0].iEzGbFltr3 == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    if(OM.EzSensor[0].iEzGbFltr3 == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    if(OM.EzSensor[0].iEzGbFltr3 == 7) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 21:
                    if(!MT_GetStop(mi.XRAY_XFltr)) return false;
                    if(OM.EzSensor[0].iEzGbFltr3 == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr3 == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr3 == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr3 == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr3 == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr3 == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzGbFltr3 == 7) MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 22:
                    if(!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn)) return false;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);

                    Step.iCycle++;
                    return false;

                case 23:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork)) return false;

                    Step.iCycle++;
                    return false;

                case 24:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }

                    MacroSet.smA   = OM.EzSensor[iWorkStep].dEzGbXmA3  .ToString()   ;
                    MacroSet.sKvp  = OM.EzSensor[iWorkStep].dEzGbXKvp3 .ToString()   ;
                    MacroSet.sTime = OM.EzSensor[iWorkStep].dEzGbXTime3.ToString()   ;

                    IO_SetY(yi.XRAY_XRayOn, true);
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    Step.iCycle++;
                    return false;

                case 25:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    
                    Step.iCycle++;
                    return false;

                case 26:
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.Calibration1))
                    {
                        Step.iCycle = 25;
                        return false;
                    }
                    iGetImgStep++;
                    Step.iCycle++;
                    return false;

                case 27: 
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCoolingTime)) return false;

                    IO_SetY(yi.XRAY_XRayOn, false);

                    if (iGetImgStep < OM.EzSensorInfo.iEzGbCnt)
                    {
                        Step.iCycle = 24;
                        return false;
                    }
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 28:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 29://Tolerance 비교
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.Calibration2))
                    {
                        Step.iCycle = 28;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 30:
                    if (SEQ.Mcr.Ez1.bReWork)
                    {
                        iGetImgStep--;
                        SEQ.Mcr.Ez1.bReWork = false;
                        Step.iCycle = 24;
                        return false;
                    }
                    iGetImgStep = 0;
                    iWorkStep++;
                    //OM.EqpStat.iLastWorkStep = iWorkStep;

                    Step.iCycle++;
                    return false;

                case 31:
                    if(OM.EzSensor[iWorkStep].dEzGbXmA3      == 0  &&
                       OM.EzSensor[iWorkStep].dEzGbXKvp3     == 0  && 
                       OM.EzSensor[iWorkStep].dEzGbXTime3    == 0  )
                    {
                        iPreFilter = 0;
                        iCrntFilter = 0;
                        Step.iCycle = 50;
                        return false;
                    }
                    
                    Step.iCycle = 0;
                    return true;

                //작업 조건이 다 0이면 여기 탄다.
                case 50:
                    iWorkStep = 0;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    
                    Step.iCycle++;
                    return false;

                case 51:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 52:
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    SEQ.Mcr.CycleEzSensorInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 53:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 54://Calibration Generate
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.CalGen))
                    {
                        Step.iCycle = 53;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 55:
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    Step.iCycle++;
                    return false;

                case 56:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 57://PreProcess Generate
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.PreGen))
                    {
                        Step.iCycle = 56;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 58:
                    if(iWorkStat == cs.Calibration1x1) DM.ARAY[ri.INDX].SetStat(c, r, cs.Skull1x1);
                    if(iWorkStat == cs.Calibration2x2) DM.ARAY[ri.INDX].SetStat(c, r, cs.Skull2x2);

                    Step.iCycle = 0;
                    return false;
            }
        }

        double dEndTime1x1 = 0.0;
        double dEndTime2x2 = 0.0;
        public double dCycleTime1x1 = 0.0;
        public double dCycleTime2x2 = 0.0;
        public bool CycleSkull() //드레시 할때 USB 한번 뺐다 꼽는 부분
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
                SEQ.Mcr.Stop();
                return true;
            }

            int c, r;
            SEQ.INDX.FindChip(ri.INDX, out c, out r, out iWorkStat);

            if(iWorkStat == cs.Skull1x1)
            {
                MacroSet.iBinning = OM.EzSensorInfo.iBinning1x1  ;
                                       
                MacroSet.iWidth  = OM.EzSensorInfo.iAg1x1Width;
                MacroSet.iHeight = OM.EzSensorInfo.iAg1x1Hght ;
                MacroSet.sCalFolder = "CAL_A";
                MacroSet.sSaveFolder = "1x1";
            }
            else if(iWorkStat == cs.Skull2x2)
            {
                MacroSet.iBinning = OM.EzSensorInfo.iBinning2x2  ;

                MacroSet.iWidth  = OM.EzSensorInfo.iAg2x2Width;
                MacroSet.iHeight = OM.EzSensorInfo.iAg2x2Hght ;
                MacroSet.sCalFolder = "CAL";
                MacroSet.sSaveFolder = "2x2";
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    if (OM.CmnOptn.bSkipSkull)
                    {
                        if (iWorkStat == cs.Skull1x1)
                        {
                            if (OM.EzSensorInfo.iImgSize == 0)
                            {
                                //DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                                Step.iCycle = 100;
                                return false;
                            }
                            else if (OM.EzSensorInfo.iImgSize == 2)
                            {
                                DM.ARAY[ri.INDX].SetStat(c, r, cs.Entering2x2);
                                Step.iCycle = 0;
                                return true;
                            }
                        }
                        else if (iWorkStat == cs.Skull2x2)
                        {
                            //DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                            Step.iCycle = 100;
                            return false;
                        }
                    }

                    Step.iCycle++;
                    return false;

                case 11:
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    Step.iCycle++;
                    return false;

                case 14:
                    if(OM.EzSensor[0].iEzSkFltr == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work);
                    if(OM.EzSensor[0].iEzSkFltr == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    if(OM.EzSensor[0].iEzSkFltr == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    if(OM.EzSensor[0].iEzSkFltr == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    if(OM.EzSensor[0].iEzSkFltr == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    if(OM.EzSensor[0].iEzSkFltr == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    if(OM.EzSensor[0].iEzSkFltr == 7) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 15:
                    if(!MT_GetStop(mi.XRAY_XFltr)) return false;
                    if(OM.EzSensor[0].iEzSkFltr == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzSkFltr == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzSkFltr == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzSkFltr == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzSkFltr == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzSkFltr == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    if(OM.EzSensor[0].iEzSkFltr == 7) MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn)) return false;
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);

                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork)) return false;
                    IO_SetY(yi.XRAY_XRayOn, true);

                    Step.iCycle++;
                    return false;

                case 18://Get Image1 3회
                         if(iWorkStat == cs.Skull1x1) MacroSet.sGetImgFdName = "1x1\\SKULL";
                    else if(iWorkStat == cs.Skull2x2) MacroSet.sGetImgFdName = "2x2\\SKULL";

                    MacroSet.sFileName   = "skull";

                    MacroSet.smA   = OM.EzSensor[0].dEzSkXmA  .ToString()   ;
                    MacroSet.sKvp  = OM.EzSensor[0].dEzSkXKvp .ToString()   ;
                    MacroSet.sTime = OM.EzSensor[0].dEzSkXTime.ToString()   ;

                    SEQ.Mcr.CycleEzSensorInit(MacroSet);
                    
                    Step.iCycle++;
                    return false;

                case 19:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 20:
                    
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.Skull))
                    {
                        Step.iCycle = 19;
                        return false;
                    }
                    IO_SetY(yi.XRAY_XRayOn, false);

                    Step.iCycle = 100;
                    return false;

                //Get Image 다 끝나고 마무리 하는 부분
                case 100:
                    
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 101:
                    if(!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    MoveCyl(ci.XRAY_Filter1Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter2Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter3Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter4Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter5Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter6Dn, fb.Bwd);
                    MoveCyl(ci.XRAY_Filter7Dn, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 102:
                    if(!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if(!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;

                    Step.iCycle++; //마무리
                    return false;

                case 103:
                    if (iWorkStat == cs.Skull1x1) 
                    {
                        if(OM.EzSensorInfo.iImgSize == 2) //1x1 -> 2x2 작업
                        {
                            dEndTime1x1 = DateTime.Now.ToOADate();
                            OM.EqpStat.dEndTime1x1 = dEndTime1x1;
                            dCycleTime1x1 = OM.EqpStat.dEndTime1x1 - OM.EqpStat.dStartTime1x1;
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Entering2x2);
                            Step.iCycle = 0;
                            return true;
                        }
                    }
                    MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Bwd);
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 104:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;
                    if (bLeftCnct || bRightCnct)
                    {
                        ER_SetErr(ei.ETC_UsbDisConnect, "USB Connector가 인식된 상태입니다.");
                        return true;
                    }

                    if (iWorkStat == cs.Skull1x1 && OM.EzSensorInfo.iImgSize == 0) //1x1만 작업
                    {
                        dEndTime1x1 = DateTime.Now.ToOADate();
                        OM.EqpStat.dEndTime1x1 = dEndTime1x1;
                        dCycleTime1x1 = OM.EqpStat.dEndTime1x1 - OM.EqpStat.dStartTime1x1;
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                    }
                    else if (iWorkStat == cs.Skull2x2) //2x2, 1x1 -> 2x2 작업
                    {
                        dEndTime2x2 = DateTime.Now.ToOADate();
                        OM.EqpStat.dEndTime2x2 = dEndTime2x2;
                        dCycleTime2x2 = OM.EqpStat.dEndTime2x2 - OM.EqpStat.dStartTime2x2;
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                    }
                    SaveCsv(LOT.GetLotNo());

                    Step.iCycle++;
                    return false;

                //이미지 저장 폴더를 결과 폴더에 카피
                case 105:
                    SEQ.Mcr.CycleEzSensorInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 106:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 107:
                    if (!SEQ.Mcr.CycleEzSensor(EzMac.ResultCopy))
                    {
                        Step.iCycle = 106;
                        return false;
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 108:
                    if (!m_tmDelay.OnDelay(OM.EzSensorInfo.iAtMacDelay)) return false;
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.XRAY_Filter1Dn)
            {
                if(_eFwd == fb.Fwd) {
                    if(MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork))
                    {
                        sMsg = "X-Ray Z축 모터가 하강 상태입니다.";
                        bRet = false ;
                    }
                }
            }
            else if (_eActr == ci.XRAY_Filter2Dn)
            {
                if (_eFwd == fb.Fwd)
                {
                    if (MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork))
                    {
                        sMsg = "X-Ray Z축 모터가 하강 상태입니다.";
                        bRet = false;
                    }
                }
            }
            else if (_eActr == ci.XRAY_Filter3Dn)
            {
                if (_eFwd == fb.Fwd)
                {
                    if (MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork))
                    {
                        sMsg = "X-Ray Z축 모터가 하강 상태입니다.";
                        bRet = false;
                    }
                }
            }
            else if (_eActr == ci.XRAY_Filter4Dn)
            {
                if (_eFwd == fb.Fwd)
                {
                    if (MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork))
                    {
                        sMsg = "X-Ray Z축 모터가 하강 상태입니다.";
                        bRet = false;
                    }
                }
            }
            else if (_eActr == ci.XRAY_Filter5Dn)
            {
                if (_eFwd == fb.Fwd)
                {
                    if (MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork))
                    {
                        sMsg = "X-Ray Z축 모터가 하강 상태입니다.";
                        bRet = false;
                    }
                }
            }
            else if (_eActr == ci.XRAY_Filter6Dn)
            {
                if (_eFwd == fb.Fwd)
                {
                    if (MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork))
                    {
                        sMsg = "X-Ray Z축 모터가 하강 상태입니다.";
                        bRet = false;
                    }
                }
            }
            else if (_eActr == ci.XRAY_Filter7Dn)
            {
                if (_eFwd == fb.Fwd)
                {
                    if (MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork))
                    {
                        sMsg = "X-Ray Z축 모터가 하강 상태입니다.";
                        bRet = false;
                    }
                }
            }

            else if (_eActr == ci.XRAY_LeftUSBFwBw)
            {
                //if (_eFwd == fb.Fwd)
                //{
                //    if (!MT_GetStop(mi.INDX_XRail))
                //    {
                //        sMsg = "인덱스가 이동 중 입니다.";
                //        bRet = false;
                //    }
                //}
            }

            else if (_eActr == ci.XRAY_RightUSBFwBw)
            {
                //if (_eFwd == fb.Fwd)
                //{
                //    if (!MT_GetStop(mi.INDX_XRail))
                //    {
                //        sMsg = "인덱스가 이동 중 입니다.";
                //        bRet = false;
                //    }
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

            if (_eMotr == mi.XRAY_XFltr)
            {
                if (CL_Complete(ci.XRAY_Filter1Dn, fb.Fwd) ||
                    CL_Complete(ci.XRAY_Filter2Dn, fb.Fwd) ||
                    CL_Complete(ci.XRAY_Filter3Dn, fb.Fwd) ||
                    CL_Complete(ci.XRAY_Filter4Dn, fb.Fwd) ||
                    CL_Complete(ci.XRAY_Filter5Dn, fb.Fwd) ||
                    CL_Complete(ci.XRAY_Filter6Dn, fb.Fwd) ||
                    CL_Complete(ci.XRAY_Filter7Dn, fb.Fwd))
                {
                    sMsg = "필터 실린더가 하강 상태입니다.";
                    bRet = false;
                }
            }
            else if (_eMotr == mi.XRAY_ZXRay)
            {
                if (_ePstn == pv.XRAY_ZXRayWork)
                {
                    //if (CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd) ||
                    //    CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd) ||
                    //    CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd) ||
                    //    CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd) ||
                    //    CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd) ||
                    //    CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd) ||
                    //    CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd))
                    //{
                    //    sMsg = "필터 실린더가 상승 상태입니다.";
                    //    bRet = false;
                    //}
                }
                
            }

            else if (_eMotr == mi.XRAY_XCnct)
            {

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
            if (!MT_GetStop(mi.XRAY_XFltr))return false;
            if (!MT_GetStop(mi.XRAY_ZXRay))return false;

            if (!CL_Complete(ci.XRAY_Filter1Dn)) return false;
            if (!CL_Complete(ci.XRAY_Filter2Dn)) return false;
            if (!CL_Complete(ci.XRAY_Filter3Dn)) return false;
            if (!CL_Complete(ci.XRAY_Filter4Dn)) return false;
            if (!CL_Complete(ci.XRAY_Filter5Dn)) return false;
            if (!CL_Complete(ci.XRAY_Filter6Dn)) return false;
            if (!CL_Complete(ci.XRAY_Filter7Dn)) return false;
       

            return true ;
        }
        
        public string[] sWaferNo;
        public string[] sFosNo;
        public string[] sBdNo;
        public string[] sVReset;
        public bool FindSerialNo()
        {
            int iCnt = 300;
        
            sWaferNo = new string[iCnt];
            sFosNo   = new string[iCnt];
            sBdNo    = new string[iCnt];
            sVReset  = new string[iCnt];
        
            StreamReader sr = new StreamReader(OM.CmnOptn.sEzSensorPath, Encoding.GetEncoding("euc-kr"));
        
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                string[] temp = s.Split(',');        // Split() 메서드를 이용하여 ',' 구분하여 잘라냄
        
                if (temp[0] == OM.EqpStat.sCrntSerial)
                {
                    OM.EqpStat.sSerialList = temp[0];
                    
                    OM.EqpStat.sWaferNo    = temp[1];
                    OM.EqpStat.sFosNo      = temp[2];
                    OM.EqpStat.sBdNo       = temp[3];
                    OM.EqpStat.sDarkOfs    = temp[4];
                    
                    sr.Close();
                    bFindSerialEnd = true; //Ver 2019.10.23.6 이 함수 다 탔는지 확인하는 플래그. 함수 리턴값으로 확인하려 했으나 리턴값에 따라 시퀀스가 바뀌어서 계속 뺑뺑이 돌릴수가 없음
                    return true;
                }
            }
            OM.EqpStat.sSerialList = OM.EqpStat.sCrntSerial;
            sr.Close();
            bFindSerialEnd = true; //Ver 2019.10.23.6 이 함수 다 탔는지 확인하는 플래그. 함수 리턴값으로 확인하려 했으나 리턴값에 따라 시퀀스가 바뀌어서 계속 뺑뺑이 돌릴수가 없음
            return false ;
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is: 
                //still being written to 
                //or being processed by another thread 
                //or does not exist (has already been processed) 
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            //file is not locked 
            return false;
        }

        public void SaveCsv(string sFileName)
        {
            if(OM.DevInfo.iMacroType == 1) SaveCsvEz(sFileName);
        }

        public void SaveCsvEz(string sFileName)
        {
            if(sFileName == "") sFileName = DateTime.Now.ToString("hhmmss");
            
            string sPath = "";
            string s2 = OM.EqpStat.sYear + "-" + OM.EqpStat.sMonth + "-" + OM.EqpStat.sDay;
            sPath = OM.EzSensorInfo.sRsltPath + "\\" + s2 + "\\"  + sFileName + ".csv";

            FileInfo file = new FileInfo(sPath);
            
            string sDir = Path.GetDirectoryName(sPath) + "\\";
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;

            if (file.Attributes == FileAttributes.ReadOnly) return;

            string sCrop = OM.EzSensorInfo.iAgCropTop  .ToString() + "-" +
                           OM.EzSensorInfo.iAgCropLeft .ToString() + "-" +
                           OM.EzSensorInfo.iAgCropRight.ToString() + "-" +
                           OM.EzSensorInfo.iAgCropBtm  .ToString() ;

            string sDate1x1 = DateTime.FromOADate(OM.EqpStat.dStartTime1x1).ToString("HH:mm:ss");
            string sST1x1   = DateTime.FromOADate(dCycleTime1x1           ).ToString("HH:mm:ss");
            string sDate2x2 = DateTime.FromOADate(OM.EqpStat.dStartTime2x2).ToString("HH:mm:ss");
            string sST2x2   = DateTime.FromOADate(dCycleTime2x2           ).ToString("HH:mm:ss");

            string sEntr  = OM.CmnOptn.bSkipEntr  ? "X" : "O";
            string sAging = OM.CmnOptn.bSkipAging ? "X" : "O";
            string sMTF   = OM.CmnOptn.bSkipMTF   ? "X" : "O";
            string sCalib = OM.CmnOptn.bSkipCalib ? "X" : "O";
            string sSkull = OM.CmnOptn.bSkipSkull ? "X" : "O";

            string line = "";
            if (!File.Exists(sPath))
            {
                line =
                "SerialNo"        + "," + //1x1
                "WaferNo"         + "," + 
                "FosNo"           + "," +
                "BdNo"            + "," + 
                "VReset"          + "," + 
                "Median"          + "," + 
                "Min"             + "," + 
                "Max"             + "," + 
                "Fluc"            + "," + 
                "TotalNoise"      + "," +
                "LineNoise"       + "," + 
                "DataNoise"       + "," + 
                "DynamicRange"    + "," + 
                "Sensitivity"     + "," +
                "SNR"             + "," +
                "3lpmm"           + "," +
                "6lpmm"           + "," +
                "8lpmm"           + "," +
                "Crop"            + "," +
                "Date"            + "," +
                "ST"              + "," +
                "입고공정"        + "," +
                "Aging공정"       + "," +
                "MTF/NPS공정"     + "," +
                "Calibration공정" + "," +
                "Skull공정"       + "," +
                "지그 BCR"        + "," +
                "SerialNo"        + "," + //2x2 
                "WaferNo"         + "," + 
                "FosNo"           + "," + 
                "BdNo"            + "," +
                "VReset"          + "," + 
                "Median"          + "," + 
                "Min"             + "," + 
                "Max"             + "," + 
                "Fluc"            + "," + 
                "TotalNoise"      + "," +
                "LineNoise"       + "," + 
                "DataNoise"       + "," + 
                "DynamicRange"    + "," + 
                "Sensitivity"     + "," +
                "SNR"             + "," +
                "3lpmm"           + "," +
                "6lpmm"           + "," +
                "8lpmm"           + "," +
                "Crop"            + "," +
                "Date"            + "," +
                "ST"              + "," +
                "입고공정"        + "," +
                "Aging공정"       + "," +
                "MTF/NPS공정"     + "," +
                "Calibration공정" + "," +
                "Skull공정"       + "," +
                "지그 BCR"        + "\r\n";
            }

            if (File.Exists(sPath))
            {
                if (IsFileLocked(file)) return; //Report 파일 열려있으면 return 시키고 안열려있으면 저장
            }
            FileStream fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("euc-kr"));
            line +=
            OM.EqpStat.sSerialList             + "," + //1x1
            OM.EqpStat.sWaferNo                + "," + 
            OM.EqpStat.sFosNo                  + "," + 
            OM.EqpStat.sBdNo                   + "," + 
            OM.EqpStat.sDarkOfs                + "," + 
            SEQ.Mcr.Ez1.Result.Median1x1       + "," + 
            SEQ.Mcr.Ez1.Result.Min1x1          + "," + 
            SEQ.Mcr.Ez1.Result.Max1x1          + "," + 
            SEQ.Mcr.Ez1.Result.Fluc1x1         + "," + 
            SEQ.Mcr.Ez1.Result.TotalNoise1x1   + "," +
            SEQ.Mcr.Ez1.Result.LineNoise1x1    + "," + 
            SEQ.Mcr.Ez1.Result.DataNoise1x1    + "," + 
            SEQ.Mcr.Ez1.Result.DynamicRange1x1 + "," +
            SEQ.Mcr.Ez1.Result.Sens1x1         + "," +
            SEQ.Mcr.Ez1.Result.SNR1x1          + "," +
            SEQ.Mcr.Ez1.Result.s3lpmm1x1       + "," + //"3lpmm"                        
            SEQ.Mcr.Ez1.Result.s6lpmm1x1       + "," + //"6lpmm"                        
            SEQ.Mcr.Ez1.Result.s8lpmm1x1       + "," + //"8lpmm"                        
            sCrop                              + "," + //"Crop"                         
            sDate1x1                           + "," + //"Date"  
            sST1x1                             + "," + //"ST"                         
            sEntr                              + "," + //"입고공정"                     
            sAging                             + "," + //"Aging공정"                    
            sMTF                               + "," + //"MTF/NPS공정"                  
            sCalib                             + "," + //"Calibration공정"              
            sSkull                             + "," + //"Skull공정" 
            OM.EqpStat.sBarcode                + "," + //"지그 BCR"   
            OM.EqpStat.sSerialList             + "," + //2x2
            OM.EqpStat.sWaferNo                + "," + 
            OM.EqpStat.sFosNo                  + "," + 
            OM.EqpStat.sBdNo                   + "," + 
            OM.EqpStat.sDarkOfs                + "," + 
            SEQ.Mcr.Ez1.Result.Median2x2       + "," + 
            SEQ.Mcr.Ez1.Result.Min2x2          + "," + 
            SEQ.Mcr.Ez1.Result.Max2x2          + "," + 
            SEQ.Mcr.Ez1.Result.Fluc2x2         + "," + 
            SEQ.Mcr.Ez1.Result.TotalNoise2x2   + "," +
            SEQ.Mcr.Ez1.Result.LineNoise2x2    + "," + 
            SEQ.Mcr.Ez1.Result.DataNoise2x2    + "," + 
            SEQ.Mcr.Ez1.Result.DynamicRange2x2 + "," +
            SEQ.Mcr.Ez1.Result.Sens2x2         + "," +
            SEQ.Mcr.Ez1.Result.SNR2x2          + "," +
            SEQ.Mcr.Ez1.Result.s3lpmm2x2       + "," +//"3lpmm"                        
            SEQ.Mcr.Ez1.Result.s6lpmm2x2       + "," +//"6lpmm"                        
            SEQ.Mcr.Ez1.Result.s8lpmm2x2       + "," +//"8lpmm"                        
            sCrop                              + "," +//"Crop"                         
            sDate2x2                           + "," +//"Date"  
            sST2x2                             + "," +//"ST"                         
            sEntr                              + "," +//"입고공정"                     
            sAging                             + "," +//"Aging공정"                    
            sMTF                               + "," +//"MTF/NPS공정"                  
            sCalib                             + "," +//"Calibration공정"              
            sSkull                             + "," +//"Skull공정" 
            OM.EqpStat.sBarcode                ;      //"지그 BCR"       

            sw.WriteLine(line);
            sw.Close();
            fs.Close();
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
