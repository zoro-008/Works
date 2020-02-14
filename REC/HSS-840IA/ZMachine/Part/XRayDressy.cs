using System;
using COMMON;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;

namespace Machine
{
    public class XRayDressy : Part
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
            Ready        ,//Dressy 전용
            Work         ,//Dressy 전용
            Analyze      ,//Dressy 전용
            Check        ,//Dressy 전용
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

        Dressy.SSetting MacroSet = new Dressy.SSetting();

        public XRayDressy(int _iPartId = 0)
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
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 10000)) ER_SetErr(ei.ETC_ToStopTO, m_sPartName);

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
                bool bWork    = DM.ARAY[ri.INDX].GetCntStat(cs.Unknown) != 0 ||
                                DM.ARAY[ri.INDX].GetCntStat(cs.Ready)   != 0 ||
                                DM.ARAY[ri.INDX].GetCntStat(cs.Work)    != 0 ||
                                DM.ARAY[ri.INDX].GetCntStat(cs.Analyze) != 0;

                bool isCycleConnect = (!bNone && !bBarcode && bWork &&
                                      DM.ARAY[ri.INDX].GetCntStat(cs.Check) == 0)  &&
                                      ML.CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Bwd) && 
                                      ML.CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd); //USB가 커넥팅 안되있으면 무조건 탄다.
                bool isCycleReady   = DM.ARAY[ri.INDX].GetCntStat(cs.Ready  ) != 0 ;
                bool isCycleWork    = DM.ARAY[ri.INDX].GetCntStat(cs.Work   ) != 0 ;
                bool isCycleAnalyze = DM.ARAY[ri.INDX].GetCntStat(cs.Analyze) != 0 ;
                bool isCycleCheck   = DM.ARAY[ri.INDX].GetCntStat(cs.Check  ) != 0;
                bool isCycleEnd     = DM.ARAY[ri.INDX].CheckAllStat(cs.None);
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleConnect ) { Step.eSeq     = sc.Connect       ; }
                else if (isCycleReady   ) { Step.eSeq     = sc.Ready         ; }
                else if (isCycleWork    ) { Step.eSeq     = sc.Work          ; }
                else if (isCycleAnalyze ) { Step.eSeq     = sc.Analyze       ; }
                else if (isCycleCheck   ) { Step.eSeq     = sc.Check         ; }
                else if (isCycleEnd     ) { Stat.bWorkEnd = true; return true; }
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
                case sc.Connect  : if (!CycleConnect  ())  return false; break ;
                case sc.Ready    : if (!CycleReady    ())  return false; break ;
                case sc.Work     : if (!CycleWork     ())  return false; break ;
                case sc.Analyze  : if (!CycleAnalyze  ())  return false; break ;
                case sc.Check    : if (!CycleCheck    ())  return false; break ;
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
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop()/* &&!OM.MstOptn.bDebugMode*/, 30000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
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

            //MacroSet.sGain = "1";
            MacroSet.sBind = "1"; //1 : 1x1, 2 : 2x2
            
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

                    if (iWorkStat == cs.Analyze) iC = c;
                    if (iWorkStat == cs.Work   ) iC = c;
                    if (iWorkStat == cs.Ready  ) iC = c;
                    if (iWorkStat == cs.Unknown) iC = c;
                    if (iWorkStat == cs.Empty) iC = c;
                    
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
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.Ready);
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
                        DM.ARAY[ri.INDX].SetStat(c, r, cs.Ready);
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

        public int iWorkStep;
        double dStartTime = 0.0;
        public bool CycleReady()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !SEQ.Mcr.CycleIng && !OM.MstOptn.bDebugMode, 20000))
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

            DM.ARAY[ri.INDX].Step = 0;

            MacroSet.smA        = OM.Dressy[iWorkStep].dXmA     .ToString();
            MacroSet.sKvp       = OM.Dressy[iWorkStep].dXKvp    .ToString();
            MacroSet.sTime      = OM.Dressy[iWorkStep].dXTime   .ToString();
            MacroSet.sFileName1 = OM.Dressy[iWorkStep].sFileName.ToString();
            MacroSet.sBind      = OM.Dressy[iWorkStep].iBind    .ToString();

            MacroSet.sAreaUp = "0";
            MacroSet.sAreaDn = "0";
            MacroSet.sAreaLt = "0";
            MacroSet.sAreaRt = "0";

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    OM.EqpStat.dStartTime = 0;
                    OM.EqpStat.dEndTime = 0;
                    dStartTime = DateTime.Now.ToOADate();
                    OM.EqpStat.dStartTime = dStartTime;
                    OM.EqpStat.sSerialList = "";
                    OM.EqpStat.sWaferNo    = "";
                    OM.EqpStat.sFosNo      = "";
                    OM.EqpStat.sBdNo       = "";
                    OM.EqpStat.sVReset     = "";
                    OM.EqpStat.sVolt       = "";
                    OM.EqpStat.sTemp       = "";
                    OM.EqpStat.sHumid      = "";

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
                    SEQ.Mcr.CycleDressyInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ML.ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    if (iWorkStat == cs.Ready)
                    {
                        if (OM.DevInfo.iMacroType == 0 && SEQ.Mcr.Dr1.bDetectSerial)
                        {
                            if (FindSerialNo())
                            {
                                Step.iCycle++;
                                return false;
                            }
                            else if (!FindSerialNo() && OM.CmnOptn.bIgnrSerialErr)//시리얼 매칭 무시 사용하면 자재 시리얼만 입력하고 다음 자재
                            {
                                Step.iCycle = 80;
                                return false;
                            }
                            else if (!FindSerialNo() && !OM.CmnOptn.bIgnrSerialErr)//시리얼넘비 매칭 에러 사용 시 여기 탄다.
                            {
                                Step.iCycle = 90;
                                return false;
                            }
                        }
                    }

                    Step.iCycle++;
                    return false;
                  
                case 12:
                    if (!SEQ.Mcr.CycleDressy(DressyMac.Setting))
                    {
                        Step.iCycle = 11;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 13:
                    if(iWorkStat == cs.Ready) DM.ARAY[ri.INDX].SetStat(c, r, cs.Work);
                    SEQ.Mcr.Dr1.bDetectSerial = false;
                    iWorkStep = 0;
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

                    OM.EqpStat.sSerialList += "(Serail No Fail)";
                    dEndTime = DateTime.Now.ToOADate();
                    OM.EqpStat.dEndTime = dEndTime;
                    
                    Step.iCycle++;
                    return false;

                case 83:
                    SaveCsv(LOT.GetLotNo());
                    MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Bwd);
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 84:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;

                    DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                    SEQ.Mcr.Dr1.bDetectSerial = false;
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

                    SEQ.Mcr.Dr1.bDetectSerial = false;
                    IO_SetY(yi.XRAY_XRayOn, false);
                    ER_SetErr(ei.ETC_MacroErr, "시리얼 넘버가 일치하는 라인이 없습니다.");
                    return true;
            }
        }


        //Aray Stat == Work 일때 타다.
        //만약 검사 단계가 25개이라고 치면
        //처음 Idx가 Work로 변할때 iStep = 0 으로 초기화 되고.
        //메크로 0번 작업이 끝나면 iStep = 1 으로 증가 되고
        //매번 검사가끝났을때 맨마지막 스텝에서 iStep++이 되고 
        //if(iStep >= 25)이면 Stat==Analize로 바뀜.
        int iPreBind = 0;
        int iBind = 0;
        int iPreFilter  = 0;
        int iCrntFilter = 0;
        public bool CycleWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !SEQ.Mcr.CycleIng && !OM.MstOptn.bDebugMode, 210000))
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
                return true ;
            }

            int c, r;
            SEQ.INDX.FindChip(ri.INDX, out c, out r, out iWorkStat);

            MacroSet.smA        = OM.Dressy[iWorkStep].dXmA     .ToString();
            MacroSet.sKvp       = OM.Dressy[iWorkStep].dXKvp    .ToString();
            MacroSet.sTime      = OM.Dressy[iWorkStep].dXTime   .ToString();
            MacroSet.sFileName1 = OM.Dressy[iWorkStep].sFileName.ToString();
            //MacroSet.sGain      = "1";
            MacroSet.sBind      = OM.Dressy[iWorkStep].iBind    .ToString();


            MacroSet.iAcq1 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq1, 0);
            MacroSet.iAcq2 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq2, 0);
            MacroSet.iAcq3 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq3, 0);
            MacroSet.iAcq4 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq4, 0);
            MacroSet.iAcq5 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq5, 0);
            MacroSet.iAcq6 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq6, 0);
            MacroSet.iAcq7 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq7, 0);
            

            MacroSet.sAreaUp = "0";
            MacroSet.sAreaDn = "0";
            MacroSet.sAreaLt = "0";
            MacroSet.sAreaRt = "0";

            MacroSet.sPath1 = OM.DressyInfo.sRsltPath;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    OM.EqpStat.iLastWorkStep = iWorkStep ;
                    
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
                    if (OM.Dressy[iWorkStep].iFilter == 0)
                    {
                        if (OM.MstOptn.bIdleRun)
                        {
                            Step.iCycle = 30;
                            return false;
                        }
                        Step.iCycle = 17;
                        return false;
                    }
                    else if (OM.Dressy[iWorkStep].iFilter == 1) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr1Work); //해당 필터 넘버 받아서 그쪽으로 모터 넘김
                    else if (OM.Dressy[iWorkStep].iFilter == 2) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr2Work);
                    else if (OM.Dressy[iWorkStep].iFilter == 3) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr3Work);
                    else if (OM.Dressy[iWorkStep].iFilter == 4) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr4Work);
                    else if (OM.Dressy[iWorkStep].iFilter == 5) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr5Work);
                    else if (OM.Dressy[iWorkStep].iFilter == 6) MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr6Work);
                    else                                        MoveMotr(mi.XRAY_XFltr, pv.XRAY_XFltr7Work);

                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStop(mi.XRAY_XFltr)) return false;
                         if (OM.Dressy[iWorkStep].iFilter == 1) MoveCyl(ci.XRAY_Filter1Dn, fb.Fwd); //해당 필터 넘버 받아서 그쪽으로 모터 넘김
                    else if (OM.Dressy[iWorkStep].iFilter == 2) MoveCyl(ci.XRAY_Filter2Dn, fb.Fwd);
                    else if (OM.Dressy[iWorkStep].iFilter == 3) MoveCyl(ci.XRAY_Filter3Dn, fb.Fwd);
                    else if (OM.Dressy[iWorkStep].iFilter == 4) MoveCyl(ci.XRAY_Filter4Dn, fb.Fwd);
                    else if (OM.Dressy[iWorkStep].iFilter == 5) MoveCyl(ci.XRAY_Filter5Dn, fb.Fwd);
                    else if (OM.Dressy[iWorkStep].iFilter == 6) MoveCyl(ci.XRAY_Filter6Dn, fb.Fwd);
                    else                                        MoveCyl(ci.XRAY_Filter7Dn, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.XRAY_Filter1Dn)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn)) return false;

                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork);//이거 Xray 파라 바꾸면서 내릴 시간 충분하면 위로 옮긴다. 진섭

                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork)) return false;
                    if (OM.MstOptn.bIdleRun)
                    {
                        Step.iCycle = 30;
                        return false;
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                //위에서 씀
                case 17:
                    if (!m_tmDelay.OnDelay(200)) return false;
                    if (!OM.MstOptn.bUseSwTrg)
                    {
                        if (SEQ.Mcr.GetErrCode() != "")
                        {
                            IO_SetY(yi.XRAY_XRayOn, false);
                            ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                            return true;
                        }
                    }
                    IO_SetY(yi.XRAY_XRayOn, true);

                    SEQ.Mcr.CycleDressyInit(MacroSet);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    MacroSet.iAcq1 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq1, 0);
                    MacroSet.iAcq2 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq2, 0);
                    MacroSet.iAcq3 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq3, 0);
                    MacroSet.iAcq4 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq4, 0);
                    MacroSet.iAcq5 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq5, 0);
                    MacroSet.iAcq6 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq6, 0);
                    MacroSet.iAcq7 = CConfig.StrToIntDef(OM.Dressy[iWorkStep].sAcq7, 0);

                    //트리거 에러 났을때 에러 안띄우고 다음 자재 작업하러...
                    //1.0.1.7
                    //Cal 공정 작업 중 편차 확인에서 Pass 안되고 계속 NG 일때
                    //다음자재 작업하러....
                    //마지막에 Report에 남길때 Calibration Fail/Trigger Fail 문구만 달라지고
                    //그외에 모든게 같아서 같은 사이클 타게 함.
                    bool bTrgErr = OM.Dressy[iWorkStep].sType == "1" && OM.DressyInfo.iTrgErrProc == 0;
                    bool bCalErr = OM.Dressy[iWorkStep].sType == "2" && OM.DressyInfo.iCalRptErrProc == 0;
                    if ((bTrgErr || bCalErr) && SEQ.Mcr.GetErrCode() != "")
                    {
                        Step.iCycle = 60;
                        return false;
                    }
                    
                    if (!OM.MstOptn.bUseSwTrg)
                    {
                        if (SEQ.Mcr.GetErrCode() != "")
                        {
                            IO_SetY(yi.XRAY_XRayOn, false);
                            ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                            return true;
                        }
                    }
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iCoolingTime)) return false; //X-Ray 조사기 쿨링 타임
                    if (iPreBind == 1 && iBind == 2)
                    {
                        MacroSet.sAreaUp = "0";
                        MacroSet.sAreaDn = "0";
                        MacroSet.sAreaLt = "0";
                        MacroSet.sAreaRt = "0";
                        if (!SEQ.Mcr.CycleDressy(DressyMac.Configuration)) return false;
                    }
                    
                    if(OM.Dressy[iWorkStep].sType == "1")
                    {
                        if (!SEQ.Mcr.CycleDressy(DressyMac.Trigger)) return false;
                    }
                    else if (OM.Dressy[iWorkStep].sType == "2")
                    {
                        if (!SEQ.Mcr.CycleDressy(DressyMac.Calibration)) return false;
                        //OK이면 가야될 스텝은 여기서 바꾼다.
                        if (SEQ.Mcr.Dr1.bPass && OM.Dressy[iWorkStep].sStep != "")
                        {
                            iWorkStep = int.Parse(OM.Dressy[iWorkStep].sStep) - 1;
                            Step.iCycle = 0;
                            return true;
                        }
                        
                    }
                    else if (OM.Dressy[iWorkStep].sType == "3")
                    {
                        if (!SEQ.Mcr.CycleDressy(DressyMac.Generate)) return false;
                    }
                    else if (OM.Dressy[iWorkStep].sType == "4")
                    {
                        string sFullName = OM.Dressy[iWorkStep].sFileName;
                        int iNameLength = sFullName.Length;
                        int iStartString = iNameLength - 8;
                        
                        string sName = sFullName.Substring(iStartString, 4);
                        
                        MacroSet.iMin = int.Parse(sName);
                        MacroSet.iMax = int.Parse(sName) + 100;

                        if (!SEQ.Mcr.CycleDressy(DressyMac.BPM4P)) return false;
                    }
                    else if (OM.Dressy[iWorkStep].sType == "5")
                    {
                        if (!SEQ.Mcr.CycleDressy(DressyMac.CalUpdate)) return false;
                    }
                    else if (OM.Dressy[iWorkStep].sType == "6")
                    {
                        if (!SEQ.Mcr.CycleDressy(DressyMac.Acquisition)) return false;
                    }
                    else if (OM.Dressy[iWorkStep].sType == "7")
                    {
                        if (!SEQ.Mcr.CycleDressy(DressyMac.FolderCopy)) return false;
                    }
                    else if (OM.Dressy[iWorkStep].sType == "8")
                    {
                        if (!SEQ.Mcr.CycleDressy(DressyMac.Aging)) return false;
                    }
                    else if (OM.Dressy[iWorkStep].sType == "9")
                    {
                        if(OM.Dressy[iWorkStep].sStep != "")
                        {
                            iWorkStep = int.Parse(OM.Dressy[iWorkStep].sStep) - 1;
                            Step.iCycle = 0;
                            return true;
                        }
                    }

                    iPreBind = OM.Dressy[iWorkStep].iBind; //전에 리스트에서 바인드 확인
                    iPreFilter = OM.Dressy[iWorkStep].iFilter;

                    if (!SEQ.Mcr.Dr1.bRework) //편차 NG 상태일때 그 스텝에서 편차 재확인 후 옮겨야되는데 이거땜에 +1되서 편차 NG일때는 스텝 안올린다.
                    {
                        iWorkStep++;
                    }

                    iBind = OM.Dressy[iWorkStep].iBind; //현재 리스트에서 바인드 확인
                    iCrntFilter = OM.Dressy[iWorkStep].iFilter;

                    if(OM.Dressy[iWorkStep].dXmA      == 0  &&
                       OM.Dressy[iWorkStep].dXKvp     == 0  && 
                       OM.Dressy[iWorkStep].dXTime    == 0  && 
                       OM.Dressy[iWorkStep].sFileName == "" &&
                       OM.Dressy[iWorkStep].sType     == "" &&
                       OM.Dressy[iWorkStep].iBind     == 0 ) //리스트 항목에 있는거에 아무것도 입력 안되어있으면 끝
                    {
                        iPreBind = 0;
                        iBind = 0;
                        iPreFilter = 0;
                        iCrntFilter = 0;
                        Step.iCycle = 50;
                        return false;
                    }

                    if ((iPreFilter != 0 && iCrntFilter != 0) && iPreFilter == iCrntFilter)
                    {
                        Step.iCycle = 16;
                        return false;
                    }
                    
                    Step.iCycle = 0;
                    return true;

                //아이들 런
                case 30:
                    iWorkStep++;
                    if(OM.Dressy[iWorkStep].dXmA      == 0  &&
                       OM.Dressy[iWorkStep].dXKvp     == 0  && 
                       OM.Dressy[iWorkStep].dXTime    == 0  && 
                       OM.Dressy[iWorkStep].sFileName == "" &&
                       OM.Dressy[iWorkStep].sType     == "" &&
                       OM.Dressy[iWorkStep].iBind     == 0 ) //리스트 항목에 있는거에 아무것도 입력 안되어있으면 끝
                    {
                        Step.iCycle = 50;
                        return false;
                    }
                    Step.iCycle = 0;
                    return true;
                    
                //매크로 끝나고 정리하는 부분
                case 50:
                    IO_SetY(yi.XRAY_XRayOn, false);
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
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    
                    MacroSet.sAreaUp = OM.DressyInfo.s8;
                    MacroSet.sAreaDn = OM.DressyInfo.s9;
                    MacroSet.sAreaLt = OM.DressyInfo.sA;
                    MacroSet.sAreaRt = OM.DressyInfo.sB;
                    SEQ.Mcr.CycleDressyInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 53:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    if (!SEQ.Mcr.CycleDressy(DressyMac.Configuration)) return false;

                    Step.iCycle++;
                    return false;

                case 54:
                    if(iWorkStat == cs.Work) DM.ARAY[ri.INDX].SetStat(c, r, cs.Analyze);

                    Step.iCycle = 0;
                    return true;

                //20mm 필터 쓸때 X-Ray 조사 Repeat Count까지 쐈는데 트리거 못받으면 여기 탄다. 진섭
                //1.0.1.7 Calibration 편차계산 반복 에러일때 다음 자재 작업하는 패턴. 진섭
                case 60:
                    IO_SetY(yi.XRAY_XRayOn, false);
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    Step.iCycle++;
                    return false;

                case 61:
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

                case 62:
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

                case 63:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;

                    if(OM.Dressy[iWorkStep].sType == "1") OM.EqpStat.sSerialList += "(Trigger Fail)";
                    else                                  OM.EqpStat.sSerialList += "(Calibration Fail)";
                    dEndTime = DateTime.Now.ToOADate();
                    OM.EqpStat.dEndTime = dEndTime;

                    //SEQ.Mcr.Dr1.bErrDevice = false;
                    if (iWorkStat == cs.Work) DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);

                    SaveCsv(LOT.GetLotNo());
                    Step.iCycle = 0;
                    return true;

                
            }
        }
        public bool CycleAnalyze()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode &&
                                  !SEQ.Mcr.CycleIng , 20000))
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

            MacroSet.sPath1 = OM.DressyInfo.sRsltPath;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    //소프트웨어트리거 사용 || 아이들런 사용 || CycleAnalyze 무시 사용 할때 CycleCheck로 넘김
                    if (OM.MstOptn.bUseSwTrg || OM.MstOptn.bIdleRun || OM.DressyInfo.bIgnrCycleAnalyze)
                    {
                        if(iWorkStat == cs.Analyze)
                        {
                            DM.ARAY[ri.INDX].SetStat(c, r, cs.Check);
                            Step.iCycle = 0;
                            return true;
                        }
                    }

                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    if (!CL_Complete(ci.XRAY_Filter1Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter2Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter3Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter4Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter5Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter6Dn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_Filter7Dn, fb.Bwd)) return false;
                    if (OM.DevInfo.iMacroType == 0)//Dressy
                    {
                        Step.iCycle = 30;
                        return false;
                    }
                    
                    Step.iCycle++;
                    return false;

                case 30:
                    SEQ.Mcr.CycleDressyInit(MacroSet); 

                    Step.iCycle++;
                    return false;

                case 31:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    if (!SEQ.Mcr.CycleDressy(DressyMac.VIET)) return false;
                    
                    Step.iCycle++;
                    return false;

                case 32:
                    SEQ.Mcr.CycleDressyInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 33:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    if(!SEQ.Mcr.CycleDressy(DressyMac.Write)) return false;

                    Step.iCycle++;
                    return false;

                case 34:
                    SEQ.Mcr.CycleDressyInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 35:
                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                        return true;
                    }
                    if (!SEQ.Mcr.CycleDressy(DressyMac.FolderCopy)) return false;

                    Step.iCycle++;
                    return false;

                case 36:
                    if (iWorkStat == cs.Analyze) DM.ARAY[ri.INDX].SetStat(c, r, cs.Check);
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        double dEndTime = 0.0;
        public double dCycleTime = 0.0;
        public bool CycleCheck() //드레시 할때 USB 한번 뺐다 꼽는 부분
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !SEQ.Mcr.CycleIng && !OM.MstOptn.bDebugMode, 20000))
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
                    if (OM.DressyInfo.bIgnrCycleCheck) //CycleCheck 무시 사용하면 종료
                    {
                        Step.iCycle = 52;
                        return false;
                    }
                    
                    MoveMotr(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove);
                    MoveCyl(ci.XRAY_LeftUSBFwBw , fb.Bwd);
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)) return false;
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw , fb.Bwd)) return false;
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

                    if (iWorkStat == cs.Check)
                    {
                        SEQ.INDX.MoveMotr(mi.INDX_XRail, pv.INDX_XRailWorkStt, c * OM.DevInfo.dTRAY_PcktPitchX);//검사 진행 방향은 왼쪽 자재가 1번
                    }
                    

                    Step.iCycle++;
                    return false;

                case 13:
                    if (iWorkStat == cs.Check)
                    {
                        if (!MT_GetStopPos(mi.INDX_XRail, pv.INDX_XRailWorkStt, c * OM.DevInfo.dTRAY_PcktPitchX)) return false;
                    }
                    
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
                        Step.iCycle = 40;
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStop(mi.XRAY_XCnct)) return false;

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
                        if (!OM.MstOptn.bIdleRun && !bLeftCnct)
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
                        if (!OM.MstOptn.bIdleRun && !bRightCnct)
                        {
                            MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                            ER_SetErr(ei.ETC_UsbDisConnect, "우측 USB 연결확인이 되지 않았습니다.");
                            return true;
                        }
                    }
                    
                    Step.iCycle = 50;
                    return false;

                //레프트 꼽았을때 문제있으면 라이트 꼽는 부분
                case 40:
                    MoveMotr(mi.XRAY_XCnct, pv.XRAY_XCnctLeftWork);

                    Step.iCycle++;
                    return false;

                case 41:
                    if (!MT_GetStopPos(mi.XRAY_XCnct, pv.XRAY_XCnctLeftWork)) return false;
                    MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Fwd);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 42:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Fwd)) return false;
                    if (!m_tmDelay.OnDelay(1500)) return false;
                    if (bLeftCnct) //레프트 USB 연결 되면 매크로 돌리고, 연결안되면 다음 사이클로 넘어간다.
                    {
                        Step.iCycle = 50;
                        return false;
                    }
                    Step.iCycle++;
                    return false;

                case 43:
                    MoveCyl(ci.XRAY_LeftUSBFwBw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 44:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Bwd)) return false;
                    MoveMotr(mi.XRAY_XCnct, pv.XRAY_XCnctRightWork);

                    Step.iCycle++;
                    return false;

                case 45:
                    if (!MT_GetStopPos(mi.XRAY_XCnct, pv.XRAY_XCnctRightWork)) return false;
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Fwd);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 46:
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Fwd)) return false;
                    if (!m_tmDelay.OnDelay(3000)) return false;
                    if (!OM.MstOptn.bIdleRun && !bRightCnct) //라이트 USB 연결안되면 에러, 연결되면 매크로 돌린다. 사이클로 넘어간다.
                    {
                        ER_SetErr(ei.ETC_UsbDisConnect, "좌측과 우측 USB 연결 확인이 되지 않습니다.");
                        return true;
                    }
                    
                    Step.iCycle = 50;
                    return false;

                //매크로 돌리고 USB 빼서 끝내는 부분
                case 50:
                    if (OM.MstOptn.bIdleRun || OM.MstOptn.bUseSwTrg)
                    {
                        m_tmDelay.Clear();
                        Step.iCycle = 52;
                        return false;
                    }
                    SEQ.Mcr.CycleDressyInit(MacroSet);

                    Step.iCycle++;
                    return false;

                case 51:
                    if (!OM.MstOptn.bUseSwTrg)
                    {
                        if (SEQ.Mcr.GetErrCode() != "")
                        {
                            ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                            return true;
                        }
                    }
                    
                    if (!SEQ.Mcr.CycleDressy(DressyMac.FileCheck)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 52:
                    if (!m_tmDelay.OnDelay(1500)) return false;
                    
                    MoveCyl(ci.XRAY_LeftUSBFwBw , fb.Bwd);
                    MoveCyl(ci.XRAY_RightUSBFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 53:
                    if (!CL_Complete(ci.XRAY_LeftUSBFwBw , fb.Bwd)) return false;
                    if (!CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd)) return false;
                    if (bLeftCnct || bRightCnct)
                    {
                        ER_SetErr(ei.ETC_UsbDisConnect, "USB Connector가 인식된 상태입니다.");
                        return true;
                    }

                    dEndTime = DateTime.Now.ToOADate();
                    OM.EqpStat.dEndTime = dEndTime;
                    dCycleTime = OM.EqpStat.dEndTime - OM.EqpStat.dStartTime;
                    SaveCsv(LOT.GetLotNo());

                    if (iWorkStat == cs.Check) DM.ARAY[ri.INDX].SetStat(c, r, cs.WorkEnd);
                    
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
        
        //드레시 전용
        public string[] sVolt ;
        public string[] sTemp ;
        public string[] sHumid;
        public string[] sWaferNo;
        public string[] sFosNo;
        public string[] sBdNo;
        public string[] sVReset;
        public bool FindSerialNo()
        {
            int iCnt = 300;
        
            sVolt  = new string[iCnt];
            sTemp  = new string[iCnt];
            sHumid = new string[iCnt];
        
            sWaferNo = new string[iCnt];
            sFosNo   = new string[iCnt];
            sBdNo    = new string[iCnt];
            sVReset  = new string[iCnt];
        
            StreamReader sr = new StreamReader(OM.CmnOptn.sDressyPath, Encoding.GetEncoding("euc-kr"));
        
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                string[] temp = s.Split(',');        // Split() 메서드를 이용하여 ',' 구분하여 잘라냄
        
                if (temp[0] == OM.EqpStat.sCrntSerial)
                {
                    OM.EqpStat.sSerialList = temp[0];
                    
                    OM.EqpStat.sWaferNo = temp[1];
                    OM.EqpStat.sFosNo   = temp[2];
                    OM.EqpStat.sBdNo    = temp[3];
                    OM.EqpStat.sVReset  = temp[4];
                    OM.EqpStat.sVolt    = temp[5];
                    OM.EqpStat.sTemp    = temp[6];
                    OM.EqpStat.sHumid   = temp[7];
                    
                    sr.Close();
                    return true;
                }
            }

            OM.EqpStat.sSerialList = OM.EqpStat.sCrntSerial;
            sr.Close();
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
            if(OM.DevInfo.iMacroType == 0) SaveCsvDr(sFileName);
        }

        public void SaveCsvDr(string sFileName)
        {
            

            if(sFileName == "") sFileName = DateTime.Now.ToString("hhmmss");
            
            string sPath = "";
            string s2 = OM.EqpStat.sYear + "-" + OM.EqpStat.sMonth + "-" + OM.EqpStat.sDay;
            sPath = OM.DressyInfo.sRsltPath + "\\" + s2 + "\\"  + sFileName + ".csv";

            FileInfo file = new FileInfo(sPath);
            if (file.Attributes == FileAttributes.ReadOnly) return;

            string sDir = Path.GetDirectoryName(sPath) + "\\";
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;

            
            string line = "";
            if (!File.Exists(sPath))
            {
                line =
                "Serial No"                     + "," +
                "Dielectricity"                 + "," +
                "Temperature"                   + "," +
                "Humid"                         + "," +
                "Blemish_a"                     + "," + 
                "Row_d"                         + "," + 
                "Column_d"                      + "," + 
                "A_col"                         + "," + 
                "A_row"                         + "," + 
                "Tot_blemish"                   + "," + 
                "Offset"                        + "," + 
                "Trigger"                       + "," + 
                "S_R60/100"                     + "," + 
                "S_R70/100"                     + "," + 
                "S_R60/400"                     + "," + 
                "S_R70/400"                     + "," + 
                "S_C60/100"                     + "," + 
                "S_C70/100"                     + "," + 
                "S_C60/400"                     + "," + 
                "S_C70/400"                     + "," + 
                "Signal_1x1"                    + "," + 
                "Noise_1x1"                     + "," + 
                "Sensitivity"                   + "," + 
                "Lf_non_uni1"                   + "," + 
                "Uniform"                       + "," + 
                "Lf_non_uni2"                   + "," + 
                "M_1x1_5lp"                     + "," + 
                "M_1x1_8lp"                     + "," + 
                "M_1x1_12lp"                    + "," + 
                "DQE"                           + "," + 
                "S_dose"                        + "," + 
                "Lin_err"                       + "," + 
                "SNR_2x2"                       + "," + 
                "Signal_2x2"                    + "," + 
                "Noise_2x2"                     + "," + 
                "M_2x2_8lp"                     + "," +
                "지그 BCR"                      + "," +
                "StartTime"                     + "," +
                "EndTime"                       + "\r\n" +
                "Spec"                          + "," + 
                ""                              + "," +
                ""                              + "," +
                ""                              + "," +
                "0~16"                          + "," + 
                "0~10"                          + "," + 
                "0~10"                          + "," + 
                "0"                             + "," + 
                "0"                             + "," + 
                "0~1"                           + "," + 
                "0~250"                         + "," + 
                "1~5"                           + "," + 
                "≥45"                          + "," + 
                "≥45"                          + "," + 
                "≥75"                          + "," + 
                "≥85"                          + "," + 
                "≥45"                          + "," + 
                "≥45"                          + "," + 
                "≥95"                          + "," + 
                "≥105"                         + "," + 
                "ADC"                           + "," + 
                "ADC"                           + "," + 
                "5.5~6.5"                       + "," + 
                "0~5"                           + "," + 
                "0~5"                           + "," + 
                "0~15"                          + "," + 
                "20~100"                        + "," + 
                "5~100"                         + "," + 
                "1~100"                         + "," + 
                "10~100"                        + "," + 
                "≥1600"                        + "," + 
                "0~5"                           + "," + 
                "≥50"                          + "," + 
                "ADC"                           + "," + 
                "ADC"                           + "," + 
                "5~100"                         + "\r\n";
            }

            if (File.Exists(sPath))
            {
                if (IsFileLocked(file)) return; //Report 파일 열려있으면 return 시키고 안열려있으면 저장
            }
            FileStream fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("euc-kr") /*Encoding.UTF8*/);
            line +=
            OM.EqpStat.sSerialList                                        + "," +
            OM.EqpStat.sVolt                                              + "," +
            OM.EqpStat.sTemp                                              + "," +
            OM.EqpStat.sHumid                                             + "," +
            SEQ.Mcr.Dr1.Result.Blemish_amount                             + "," +           
            SEQ.Mcr.Dr1.Result.Row_defects                                + "," +   
            SEQ.Mcr.Dr1.Result.Column_defects                             + "," +   
            SEQ.Mcr.Dr1.Result.Adjacent_columns                           + "," +   
            SEQ.Mcr.Dr1.Result.Adjacent_rows                              + "," +   
            SEQ.Mcr.Dr1.Result.Total_blemish_count                        + "," +   
            SEQ.Mcr.Dr1.Result.Offset                                     + "," +   
            SEQ.Mcr.Dr1.Result.Trigger_sensitivity                        + "," +            
            SEQ.Mcr.Dr1.Result.SNR_R60_100                                + "," +            
            SEQ.Mcr.Dr1.Result.SNR_R70_100                                + "," +            
            SEQ.Mcr.Dr1.Result.SNR_R60_400                                + "," +            
            SEQ.Mcr.Dr1.Result.SNR_R70_400                                + "," +            
            SEQ.Mcr.Dr1.Result.SNR_C60_100                                + "," +            
            SEQ.Mcr.Dr1.Result.SNR_C70_100                                + "," +            
            SEQ.Mcr.Dr1.Result.SNR_C60_400                                + "," +            
            SEQ.Mcr.Dr1.Result.SNR_C70_400                                + "," +            
            SEQ.Mcr.Dr1.Result.Signal_1x1_mode                            + "," +            
            SEQ.Mcr.Dr1.Result.Noise_1x1_mode                             + "," +            
            SEQ.Mcr.Dr1.Result.Sensitivity                                + "," +            
            SEQ.Mcr.Dr1.Result.Low_frequency_non_uniformity1              + "," +            
            SEQ.Mcr.Dr1.Result.Uniformity                                 + "," +           
            SEQ.Mcr.Dr1.Result.Low_frequency_non_uniformity2              + "," +           
            SEQ.Mcr.Dr1.Result.MTF_mode_1x1_at_5_lp_mm                    + "," +           
            SEQ.Mcr.Dr1.Result.MTF_mode_1x1_at_8_lp_mm                    + "," +           
            SEQ.Mcr.Dr1.Result.MTF_mode_1x1_at_12_lp_mm                   + "," +           
            SEQ.Mcr.Dr1.Result.DQE                                        + "," +           
            SEQ.Mcr.Dr1.Result.Saturation_dose                            + "," +           
            SEQ.Mcr.Dr1.Result.Linearity_error                            + "," +           
            SEQ.Mcr.Dr1.Result.SNR_2x2_mode                               + "," +           
            SEQ.Mcr.Dr1.Result.Signal_2x2_mode                            + "," +           
            SEQ.Mcr.Dr1.Result.Noise_2x2_mode                             + "," +           
            SEQ.Mcr.Dr1.Result.MTF_mode_2x2_at_8_lp_mm                    + "," +
            OM.EqpStat.sBarcode                                           + "," +
            DateTime.FromOADate(OM.EqpStat.dStartTime).ToString("HH:mm:ss") + "," +
            DateTime.FromOADate(OM.EqpStat.dEndTime)  .ToString("HH:mm:ss") ;

            sw.WriteLine(line, Encoding.GetEncoding("euc-kr"));
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
