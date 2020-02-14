using COMMON;
using System;
using System.Collections.Generic;

namespace Machine
{
    //PCK == Picker
    public class Tool : Part
    {
               //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop;
            public void Clear()
            {
                bWorkEnd = false;
                bReqStop = false;
            }
        };   
        public enum sc
        {
            Idle             = 0,
            EmptyVisn           , //빈 트레이 검사 해보자.
            Vision              , //비젼 검사 겸 바코드 스캔
            NGPick              , //픽 동작
            NGPlace             , //NG 자재 플레이스 동작
            EmptyCheck          , //픽커로 배큠확인 하여 NG stat을 Empty로 전환.
            GoodPick            ,
            GoodPlace           ,
            MAX_SEQ_CYCLE
        };

        public struct SStep
        {
            public int  iHome;
            public int  iToStart;
            public sc   eSeq;
            public int  iCycle;
            public int  iToStop;
            public sc   eLastSeq;
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
  
        //비전 검사 
        protected CCycleTimer m_ct1Cycle   ;  

        protected SStat Stat;
        protected SStep Step, PreStep;

        protected double m_dLastIdxPos;
        protected String m_sCheckSafeMsg;

        public CTimer[] m_CycleTime;

        //VisnCom.TRslt RsltDieAlign     ;
        //VisnCom.TRslt RsltSubsAlign    ;
        //VisnCom.TRslt RsltBtmAlign     ;
        //VisnCom.TRslt RsltRightDist    ;
        //VisnCom.TRslt RsltLeftDist     ;

        //bool bIDXRWork;
        //bool bIDXFWork;
        const int iVisnDelay  = 100 ;
        const int iVisnMargin = 10  ;//비젼 검사할때 시작할때랑 마지막에 마진 집어넣는다.

        public Tool()
        {
            m_sPartName = this.GetType().Name;

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();

            m_ct1Cycle    = new CCycleTimer();

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

            int a = 0; 
            a++;

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
                default: Step.iToStart = 0;
                    return true;

                case 10:
            
                    Step.iToStart++;
                    return false;
                
                case 11: 
                   
                    Step.iToStart++;
                    return false ;

                case 12:
                    
                    Step.iToStart++;
                    return false ;

                case 13:
                    
                    Step.iToStart++;
                    return false;

                case 14:
                    
                    Step.iToStart++;
                    return false;

                case 15:
                    
                    Step.iToStart = 0;
                    return true ;

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
                default: Step.iToStop = 0;
                    return true;

                case 10:
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait);
                    Step.iToStop++;
                    return false;
                
                case 11:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;
                    if (!MT_GetStopPos(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait)) return false;
                    //MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolWait);
                    MoveMotr(mi.TOOL_XRjct, pv.TOOL_XRjctWait);
                    Step.iToStop++;
                    return false ;

                case 12:
                    //if (!MT_GetStopPos(mi.TOOL_YTool, pv.TOOL_YToolWait)) return false;
                    if (!MT_GetStopPos(mi.TOOL_XRjct, pv.TOOL_XRjctWait)) return false;
                    Step.iToStop = 0;
                    return true ;
            }


        }

        override public int GetHomeStep   () { return Step.iHome    ; } override public int GetPreHomeStep   () { return PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return Step.iToStart ; } override public int GetPreToStartStep() { return PreStep.iToStart ; }
        override public int GetSeqStep    () { return (int)Step.eSeq; } override public int GetPreSeqStep    () { return (int)PreStep.eSeq; }
        override public int GetCycleStep  () { return Step.iCycle   ; } override public int GetPreCycleStep  () { return PreStep.iCycle   ; } override public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; }
        override public int GetToStopStep () { return Step.iToStop  ; } override public int GetPreToStopStep () { return PreStep.iToStop  ; }

        override public string GetCrntCycleName(         ) { return Step.eSeq.ToString();}
        override public String GetCycleName    (int _iSeq) { return ((sc)_iSeq).ToString(); }
        override public double GetCycleTime    (int _iSeq) { return m_CycleTime[_iSeq].Duration; }
        override public String GetPartName     (         ) { return m_sPartName; }

        override public int GetCycleMaxCnt() { return (int)sc.MAX_SEQ_CYCLE; }
        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            //Check Cycle Time Out.
            String sTemp;
            sTemp = String.Format("%s Step.iCycle={0:00}", "Autorun", Step.iCycle);
            if (Step.eSeq != PreStep.eSeq) {
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                if(!OM.CmnOptn.bIdleRun &&  DM.ARAY[ri.PCKR].CheckAllStat(cs.None) && IO_GetX(xi.TOOL_PckrVac)) {
                    ER_SetErr(ei.PKG_Unknwn , "Picker Unknwn PKG Error-Check " + SM.IO_GetXName(xi.TOOL_PckrVac) ); 
                    return false;
                }
                if(!OM.CmnOptn.bIdleRun && !DM.ARAY[ri.PCKR].CheckAllStat(cs.None) &&!IO_GetX(xi.TOOL_PckrVac)) {
                    ER_SetErr(ei.PKG_Dispr  , "Picker Disappear PKG Error-Check"+SM.IO_GetXName(xi.TOOL_PckrVac)); 
                    return false;
                }

                //현재 작업중인 인덱스.
                int iWorkCol , iWorkRow  ;
                cs  iWorkStat;
                bool bFlying ;
                int IDX = ri.IDXF;
                
               if(SEQ.IDXR.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying, OM.CmnOptn.bGoldenTray)){
                    IDX = ri.IDXR ;
                }
                else if (SEQ.IDXF.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying, OM.CmnOptn.bGoldenTray)){
                    IDX = ri.IDXF ;
                }

                bool bFailChip = (//DM.ARAY[IDX].GetCntStat(cs.NG0 ) != 0 || Vision result empty
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG1  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG2  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG3  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG4  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG5  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG6  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG7  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG8  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG9  ||
                                  DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG10 );

                bool bUnknownChip = DM.ARAY[IDX].GetCntStat(cs.Unknown) != 0;
                                 
                //Inspection, Pick, Place5
                bool isCycleEmptyVisn   =(OM.GetSupplyCnt() == 1 || (OM.DevOptn.bUseBtmCover && OM.GetSupplyCnt() == OM.DevInfo.iTRAY_StackingCnt)) && //첫째장이거나 마지막장 사용시 마지막장일때.
                                          DM.ARAY[IDX].GetCntStat(cs.Vision) != 0 &&  //빈트레이는 바코트에서 홀랑 엠프티이다.
                                          !OM.CmnOptn.bGoldenTray; 
                bool isCycleVision      =  bFlying ;
                bool isCycleNGPick      = (bFailChip/*||bUnknownChip*/) && DM.ARAY[IDX].GetCntStat(cs.Empty) == 0 && DM.ARAY[ri.PCKR].CheckAllStat(cs.None) && !OM.CmnOptn.bGoldenTray;
                bool isCycleNGPlace     = cs.NG0 <= DM.ARAY[ri.PCKR].GetStat(0, 0) && DM.ARAY[ri.PCKR].GetStat(0, 0) <= cs.NG10 && !DM.ARAY[ri.TRYF].CheckAllStat(cs.None) && 
                                          DM.ARAY[ri.TRYF].GetCntStat(cs.Empty) != 0;
                bool isCycleGoodPick    = DM.ARAY[ri.PCKR].CheckAllStat(cs.None) && DM.ARAY[ri.TRYG].GetCntStat(cs.Good) > 0 && iWorkStat == cs.Empty && 
                                         !DM.ARAY[ri.TRYG].CheckAllStat(cs.None) && !DM.ARAY[IDX].CheckAllStat(cs.Empty);
                bool isCycleGoodPlace   = DM.ARAY[ri.PCKR].CheckAllStat(cs.Good) && iWorkStat == cs.Empty ;//개별 확인 비전 시퀜스.
                bool isCycleEmptyCheck  = DM.ARAY[ri.PCKR].CheckAllStat(cs.None) && iWorkStat == cs.NG0 ; //일단 비전 엠프티 는 NG0번
                bool isCycleEnd         = DM.ARAY[ri.PCKR].CheckAllStat(cs.None) && 
                                        ((DM.ARAY[ri.IDXR].CheckAllStat(cs.None) && DM.ARAY[ri.IDXF].CheckAllStat(cs.None)) || 
                                         (OM.CmnOptn.bGoldenTray && DM.ARAY[ri.IDXR].GetCntStat(cs.Vision) == 0 && DM.ARAY[ri.IDXF].GetCntStat(cs.Vision) == 0));
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleEmptyVisn ) { Step.eSeq  = sc.EmptyVisn  ; }
                else if (isCycleVision    ) { Step.eSeq  = sc.Vision     ; }
                else if (isCycleNGPick    ) { Step.eSeq  = sc.NGPick     ; }
                else if (isCycleNGPlace   ) { Step.eSeq  = sc.NGPlace    ; }
                else if (isCycleGoodPick  ) { Step.eSeq  = sc.GoodPick   ; }
                else if (isCycleGoodPlace ) { Step.eSeq  = sc.GoodPlace  ; }
                else if (isCycleEmptyCheck) { Step.eSeq  = sc.EmptyCheck ; }
                else if (isCycleEnd       ) { Stat.bWorkEnd = true; return true; }

                Stat.bWorkEnd = false;

                if(DM.ARAY[ri.TRYF].GetCntStat(cs.Empty) == 0)
                {
                    ER_SetErr(ei.PRT_RemoveTray, "Fail Tray Full");
                }
                if (DM.ARAY[ri.TRYG].GetCntStat(cs.Good) == 0)
                {
                    ER_SetErr(ei.PRT_FullTray , "Good Tray Empty");
                }

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
                case sc.Idle      :                                                                                                                           return false;
                case sc.EmptyVisn : if (CycleEmptyVisn ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Vision    : if (CycleVision    ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.NGPick    : if (CycleNGPick    ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.NGPlace   : if (CycleNGPlace   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.GoodPick  : if (CycleGoodPick  ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.GoodPlace : if (CycleGoodPlace ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.EmptyCheck: if (CycleEmptyCheck()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }                               
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            r = 0 ;
            c = 0 ;
            if (_iId == ri.TRYG || _iId == ri.TRYF)DM.ARAY[_iId].FindLastRowFrstCol(_iChip, ref c, ref r);
            else                                   DM.ARAY[_iId].FindLastColFrstRow(_iChip, ref c, ref r);
            return (c <= 0 && r >= OM.DevInfo.iTRAY_PcktCntY - 1) ? true : false;
        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 25000))
            {
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
                //Step.iHome = 0;
                //return true ;
            }
            
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    if(Step.iHome != PreStep.iHome)Log.Trace(m_sPartName, sTemp);
                    return true ;
            
                case 10:
                    MT_GoHome(mi.TOOL_ZPckr);
                    MT_GoHome(mi.TOOL_ZVisn); 

                    Step.iHome++;
                    return false ;

                case 11:
                    if(!MT_GetHomeDone(mi.TOOL_ZPckr)) return false;
                    if(!MT_GetHomeDone(mi.TOOL_ZVisn)) return false;
                    MT_GoHome(mi.TOOL_YTool);
                    MT_GoHome(mi.TOOL_XRjct);

                    Step.iHome++;
                    return false ;

                case 12:
                    if (!MT_GetHomeDone(mi.TOOL_YTool)) return false;
                    if (!MT_GetHomeDone(mi.TOOL_XRjct)) return false;
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait);
                    Step.iHome++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait))return false;
                                       
                    Step.iHome = 0;
                    return true ;
            }
        }

        string GetS(double _dVal)
        {
            return string.Format("{0:0.000}", _dVal);
        }

        
        
        public delegate bool FMoveMotr(mi _eMotr , pv _ePstn ,  double _dOfsPos=0);        
        int       iVisnIndx = 0 , iVisnRtry = 0;
        int       iWorkCol , iWorkRow  ;
        cs        iWorkStat;
        bool      bFlying ;
        int       IDX = ri.IDXF;
        FMoveMotr MoveMotrIdx = SEQ.IDXF.MoveMotr;
        mi        IDX_X = mi.IDXF_XFrnt;
        double    dYSttPosOfs = 0.0 ;
        double    dYEndPosOfs = 0.0 ;
        int       iInvWorkCol = 0   ;
        pv        IDX_XVsnStt   = pv.IDXF_XFrntVsnStt1 ;
        pv        IDX_XVsnStt1  = pv.IDXF_XFrntVsnStt1 ;
        pv        IDX_XVsnStt2  = pv.IDXF_XFrntVsnStt2 ;
        pv        IDX_XVsnStt3  = pv.IDXF_XFrntVsnStt3 ;
        pv        IDX_XVsnStt4  = pv.IDXF_XFrntVsnStt4 ;
        pv        TOOL_YVsnStt  = pv.TOOL_YToolVsnStt1 ;
        pv        TOOL_ZVsnWork = pv.TOOL_ZVisnWork1   ;
        pv        IDX_XWorkStt  = pv.IDXF_XFrntWorkStt ;
        xi        IDX_TrayDtct  = xi.IDXF_TrayDtct     ;
        bool      bRearToFrnt   = false ;
        
        //나중에 오라클에서 써야할것들.
        //반복문 수정.
        public bool CycleEmptyVisn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 10000))
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
            //const int iTotalInspCnt = 1 ; //진섭아 옵션추가. 1~4까지 설정 가능 하게 콤보박스. 한패키지 검사 횟수. 제일큰건 4방까지 가능.
            double dToolYVsnEndOffset = OM.DevInfo.dTRAY_PcktPitchY * (OM.DevInfo.iTRAY_PcktCntY-1) ;
            switch (Step.iCycle)
            {
                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                //초기화 패턴. 여기는 반복되지 않는다.
                case 10:
                    iVisnIndx = 0;
                    if (SEQ.IDXR.FindChip(out iWorkCol, out iWorkRow, out iWorkStat, out bFlying))
                    {
                        IDX = ri.IDXR;
                        MoveMotrIdx = SEQ.IDXR.MoveMotr;
                        IDX_X = mi.IDXR_XRear;
                        IDX_XVsnStt1 = pv.IDXR_XRearVsnStt1; 
                        IDX_XVsnStt2 = pv.IDXR_XRearVsnStt2; 
                        IDX_XVsnStt3 = pv.IDXR_XRearVsnStt3; 
                        IDX_XVsnStt4 = pv.IDXR_XRearVsnStt4; 
                        IDX_TrayDtct = xi.IDXR_TrayDtct ;
                    }
                    else if (SEQ.IDXF.FindChip(out iWorkCol, out iWorkRow, out iWorkStat, out bFlying))
                    {
                        IDX = ri.IDXF;
                        MoveMotrIdx = SEQ.IDXF.MoveMotr;
                        IDX_X = mi.IDXF_XFrnt;
                        IDX_XVsnStt1 = pv.IDXF_XFrntVsnStt1; 
                        IDX_XVsnStt2 = pv.IDXF_XFrntVsnStt2; 
                        IDX_XVsnStt3 = pv.IDXF_XFrntVsnStt3; 
                        IDX_XVsnStt4 = pv.IDXF_XFrntVsnStt4; 
                        IDX_TrayDtct = xi.IDXF_TrayDtct ;
                    }


                    ////그냥 한줄만.
                    //Step.iCycle = 200;
                    //return false;


                    if (OM.CmnOptn.iInspCrvrTray == (int)vi.One)
                    {
                        Step.iCycle = 100;
                        return false;
                    }
                    else if(OM.CmnOptn.iInspCrvrTray == (int)vi.Col)
                    {
                        Step.iCycle = 200;
                        return false;
                    }
                    else
                    {
                        Step.iCycle = 300;
                        return false;
                    }
                    
                case 100:
                    iInvWorkCol = OM.DevInfo.iTRAY_PcktCntX - iWorkCol - 1;
                    SEQ.Visn.SendManMode(true);
                    Log.Trace("VISION", "SendManMode true" );
                    Step.iCycle++;
                    return false;

                case 101:
                    if(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.ManMode))return false ;
                    Step.iCycle++;
                    return false ;

                case 102:
                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt1, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt2, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt3, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt4, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);

                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork1);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork2);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork3);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork4);
                    
                    if (iVisnIndx == 0) MoveMotrIdx(IDX_X , IDX_XVsnStt1 , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX );
                    if (iVisnIndx == 1) MoveMotrIdx(IDX_X , IDX_XVsnStt2 , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX );
                    if (iVisnIndx == 2) MoveMotrIdx(IDX_X , IDX_XVsnStt3 , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX );
                    if (iVisnIndx == 3) MoveMotrIdx(IDX_X , IDX_XVsnStt4 , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX );   
                 


                    SEQ.Visn.DeleteRslt();//검사전에 결과값 지워준다.
                    SEQ.Visn.SendIndex(iVisnIndx);
                    Log.Trace("VISION", "SendIndex" + iVisnIndx.ToString());
                    
                    
                    Step.iCycle++;
                    return false;

                //using later step.
                case 103:
                    if(!MT_GetStopInpos(mi.TOOL_ZVisn))return false;
                    if (!MT_GetStopInpos(mi.TOOL_YTool)) return false;
                    if (!MT_GetStop(IDX_X)) return false;
                    if(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Command))return false;
                    
                    if (!OM.CmnOptn.bIdleRun && !IO_GetX(xi.RAIL_TrayDtct2) && iWorkCol != 0)//sensor zone is over when col 0
                    {
                        ER_SetErr(ei.PRT_Missed , IO_GetXName(xi.RAIL_TrayDtct2) +" is Not Detected!");
                        return true ;
                    }
                    
                    Step.iCycle++;
                    return false;

                case 104:
                    SEQ.Visn.SendManInsp();
                    Log.Trace("VISION", "SendManInsp" );
                    Step.iCycle++;
                    return false;

                case 105:
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Insp)) return false;
                    if(!SEQ.Visn.FindRsltFile())return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 106:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    VisnCom.TRslt RsltOne = new VisnCom.TRslt();

                    SEQ.Visn.LoadManRslt(ref RsltOne);

                    if(RsltOne.Empty         == 0 &&
                       RsltOne.MixDevice     == 0 &&
                       RsltOne.UnitID        == null &&
                       RsltOne.UnitDMC1      == null &&
                       RsltOne.UnitDMC2      == null &&
                       RsltOne.GlobtopLeft   == 0 &&
                       RsltOne.GlobtopTop    == 0 &&
                       RsltOne.GlobtopRight  == 0 &&
                       RsltOne.GlobtopBottom == 0 &&
                       RsltOne.MatchingError == 0 &&
                       RsltOne.UserDefine    == 0 ) return false;
                    
                    //바코드들은 "0"이면 있는데 못읽음. "1"바코드가 검사에 없음. "바코드문자" 읽기 성공.
                    if(RsltOne.Empty        ==0   ){
                        DM.ARAY[IDX].SetStat(cs.Empty);
                        Step.iCycle = 0;
                        return true;
                    }

                    iVisnIndx++;
                    if(iVisnIndx<=OM.DevOptn.iTotalInspCnt) {
                        Step.iCycle = 102 ;
                        return false ;
                    }            


                    ER_SetErr(ei.PCK_CoverTray, "Corver Tray is not Empty");
                    Step.iCycle = 0;
                    return true;
                    


                case 200:

                    DM.ARAY[ri.INSP].SetStat(cs.Good);
                    if(!OM.CmnOptn.bIdleRun)SEQ.Visn.SendManMode(false);
                    Log.Trace("VISION", "SendManMode false" );
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);
                    Step.iCycle++;
                    return false ;
                
                case 201:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait)) return false;
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.ManMode) && !OM.CmnOptn.bIdleRun) return false ;
                    Step.iCycle++;
                    return false;

                //아래에서 씀.
                case 202:
                    if(IDX_X == mi.IDXR_XRear){
                        if(iVisnIndx == 0){IDX_XVsnStt = pv.IDXR_XRearVsnStt1 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt1 ;}
                        if(iVisnIndx == 1){IDX_XVsnStt = pv.IDXR_XRearVsnStt2 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt2 ;}
                        if(iVisnIndx == 2){IDX_XVsnStt = pv.IDXR_XRearVsnStt3 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt3 ;}
                        if(iVisnIndx == 3){IDX_XVsnStt = pv.IDXR_XRearVsnStt4 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt4 ;}
                    }
                    else {
                        if (iVisnIndx == 0) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt1; TOOL_YVsnStt = pv.TOOL_YToolVsnStt1;}
                        if (iVisnIndx == 1) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt2; TOOL_YVsnStt = pv.TOOL_YToolVsnStt2;}
                        if (iVisnIndx == 2) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt3; TOOL_YVsnStt = pv.TOOL_YToolVsnStt3;}
                        if (iVisnIndx == 3) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt4; TOOL_YVsnStt = pv.TOOL_YToolVsnStt4;}
                    }
                    
                    //검사1방 혹은 3방 짜리는 열마다 스타트 방향이 지그제그 이고 
                    iInvWorkCol = OM.DevInfo.iTRAY_PcktCntX - iWorkCol -1 ;
                    bRearToFrnt = (iVisnIndx + iInvWorkCol) % 2 == 0 ;//검사인덱스 와 검사열을 더해서 2로 나누면 뒤에서 앞으로 인지 앞에서 뒤로 인지 나옴.
                    if (bRearToFrnt) 
                    { //장비 앞면에서 뒤로 가는 플라잉.
                        dYSttPosOfs = -iVisnMargin ;
                        dYEndPosOfs = (OM.DevInfo.iTRAY_PcktCntY-1) * OM.DevInfo.dTRAY_PcktPitchY + iVisnMargin ;                    
                    }
                    else
                    {
                        dYSttPosOfs = (OM.DevInfo.iTRAY_PcktCntY-1) * OM.DevInfo.dTRAY_PcktPitchY + iVisnMargin ;    
                        dYEndPosOfs = -iVisnMargin ;
                    }

                    MoveMotrIdx(IDX_X , IDX_XVsnStt , iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX );
                    MoveMotr   (mi.TOOL_YTool, TOOL_YVsnStt, dYSttPosOfs);

                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork1);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork2);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork3);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork4);
                   
                    Step.iCycle++;
                    return false ;

                case 203:
                    if (!MT_GetStopPos(IDX_X        , IDX_XVsnStt ,iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX )) return false;
                    if (!MT_GetStopPos(mi.TOOL_YTool, TOOL_YVsnStt, dYSttPosOfs)) return false;
                    if (!MT_GetStop(mi.TOOL_ZVisn))return false;

                    


                    Log.Trace("VISION", "SendIndex" + iVisnIndx.ToString());
                    if(!SEQ.Visn.SendIndex(iVisnIndx) && !OM.CmnOptn.bIdleRun)
                    {
                        //SendIndex 함수 안에서 에러 띄운다. 비전 준비안됨.
                        return true;   
                    }

                    if(!OM.CmnOptn.bIdleRun) SEQ.Visn.DeleteRslt();//검사전에 결과값 지워준다.

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 204:
                    if(!m_tmDelay.OnDelay(true, 50))return false;
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Command) && !OM.CmnOptn.bIdleRun) return false;
                    SM.MT_ResetTrgPos(mi.TOOL_YTool);
                    Step.iCycle++;
                    return false;

                case 205:                    
                    double dSttPos = PM_GetValue(mi.TOOL_YTool, TOOL_YVsnStt);
                    double[] dTrgPos = new double[OM.DevInfo.iTRAY_PcktCntY];
                    for (int i = 0; i < OM.DevInfo.iTRAY_PcktCntY ; i++)
                    {
                        if(bRearToFrnt) dTrgPos[i                                ] = dSttPos + (i * OM.DevInfo.dTRAY_PcktPitchY) + OM.DevOptn.dTrgOfs;
                        else            dTrgPos[OM.DevInfo.iTRAY_PcktCntY - i - 1] = dSttPos + (i * OM.DevInfo.dTRAY_PcktPitchY) + OM.DevOptn.dTrgOfs;
                    }
                    SM.MT_SetTrgPos(mi.TOOL_YTool, dTrgPos, 1000, true, true);

                    double dVsnInspEnd = PM_GetValue(mi.TOOL_YTool, TOOL_YVsnStt) + dYEndPosOfs;
                    MT_GoAbsVel(mi.TOOL_YTool, dVsnInspEnd, OM.DevOptn.iInspSpeed);

                    Step.iCycle++;
                    return false ;

                case 206:
                    if (!MT_GetStop(mi.TOOL_YTool)) return false;                   

                    SM.MT_ResetTrgPos(mi.TOOL_YTool);
                    
                    Step.iCycle++;
                    return false;

                case 207:
                    if (IO_GetX(xi.VISN_Busy)) return false; //인스펙션 종료 확인        
                    if (!SEQ.Visn.FindRsltFile() && !OM.CmnOptn.bIdleRun) return false;
                    //iVisnIndx++;
                    
                    //여기 나중에 미리 보내는 것 넣자. 진섭아.
                    //if(iVisnIndx>=iTotalInspCnt) 
                    VisnCom.TRslt RsltCol = new VisnCom.TRslt();
                    
                    int iIndex = 0;
                    for(int i = 0 ; i < OM.DevInfo.iTRAY_PcktCntY ; i++){
                        if(bRearToFrnt) {
                            iIndex = i;
                            SEQ.Visn.LoadRslt(i, ref RsltCol);
                        }
                        else { //비전쪽은 첨찍은게 인덱스가 0이다.
                            iIndex = OM.DevInfo.iTRAY_PcktCntY - i - 1;
                            SEQ.Visn.LoadRslt(i, ref RsltCol);
                        }
                        
                        if(RsltCol.Empty ==0 ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG0);} 
                        
                        if(DM.ARAY[ri.MASK].GetStat(iWorkCol, iWorkRow) == cs.None)
                        {
                            DM.ARAY[ri.INSP].SetStat(0, iWorkRow, cs.None);
                        }
                    }

                    iVisnIndx++;
                    if(iVisnIndx<=OM.DevOptn.iTotalInspCnt) {
                        Step.iCycle = 202 ;
                        return false ;
                    }  

                    //for(int r = 0 ; r < OM.DevInfo.iTRAY_PcktCntY ; r++){
                    //    if (DM.ARAY[IDX].GetStat(iWorkCol, r) != cs.None)
                    //    {
                    //        DM.ARAY[IDX].SetStat(iWorkCol, r, DM.ARAY[ri.INSP].GetStat(0, r));
                    //    }
                    //}
                    if(DM.ARAY[ri.INSP].GetCntStat(cs.NG0) == OM.DevInfo.iTRAY_PcktCntY)
                    {
                        DM.ARAY[IDX].SetStat(cs.Empty);
                        Step.iCycle = 0;
                        return true;
                    }
                    else
                    {
                        ER_SetErr(ei.PCK_CoverTray, "Corver Tray is not Empty");
                        Step.iCycle = 0;
                        return true;
                    }


                //이것은 안쓰기도 하고 검증하기 귀찮아서 화면에서 설정 못하게 막음.
                //만약 다시 쓰려면 Vision Index를 이용해서 멀티 검사 하는 부분 확인 해야 함.
                case 300:
                    if (SEQ.IDXR.FindChip(out iWorkCol, out iWorkRow, out iWorkStat, out bFlying))
                    {
                        IDX = ri.IDXR;
                        MoveMotrIdx = SEQ.IDXR.MoveMotr;
                        IDX_X = mi.IDXR_XRear;
                        if (iVisnIndx == 0) { IDX_XVsnStt = pv.IDXR_XRearVsnStt1; TOOL_YVsnStt = pv.TOOL_YToolVsnStt1; }
                        if (iVisnIndx == 1) { IDX_XVsnStt = pv.IDXR_XRearVsnStt2; TOOL_YVsnStt = pv.TOOL_YToolVsnStt2; }
                        if (iVisnIndx == 2) { IDX_XVsnStt = pv.IDXR_XRearVsnStt3; TOOL_YVsnStt = pv.TOOL_YToolVsnStt3; }
                        if (iVisnIndx == 3) { IDX_XVsnStt = pv.IDXR_XRearVsnStt4; TOOL_YVsnStt = pv.TOOL_YToolVsnStt4; }
                    }
                    else if (SEQ.IDXF.FindChip(out iWorkCol, out iWorkRow, out iWorkStat, out bFlying))
                    {
                        IDX = ri.IDXF;
                        MoveMotrIdx = SEQ.IDXF.MoveMotr;
                        IDX_X = mi.IDXF_XFrnt;
                        if (iVisnIndx == 0) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt1; TOOL_YVsnStt = pv.TOOL_YToolVsnStt1; }
                        if (iVisnIndx == 1) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt2; TOOL_YVsnStt = pv.TOOL_YToolVsnStt2; }
                        if (iVisnIndx == 2) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt3; TOOL_YVsnStt = pv.TOOL_YToolVsnStt3; }
                        if (iVisnIndx == 3) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt4; TOOL_YVsnStt = pv.TOOL_YToolVsnStt4; }
                    }
                    DM.ARAY[ri.INSP].SetStat(cs.Good);
                    if(!OM.CmnOptn.bIdleRun)SEQ.Visn.SendManMode(false);
                    Log.Trace("VISION", "SendManMode false" );
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);
                    Step.iCycle++;
                    return false ;
                
                case 301:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait)) return false;
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.ManMode) && !OM.CmnOptn.bIdleRun) return false ;
                    Step.iCycle++;
                    return false;

                //아래에서 씀.
                case 302:
                    if(SEQ.IDXR.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying)){
                        IDX = ri.IDXR ;
                        MoveMotrIdx = SEQ.IDXR.MoveMotr ;
                        IDX_X = mi.IDXR_XRear ;
                        if(iVisnIndx == 0){IDX_XVsnStt = pv.IDXR_XRearVsnStt1 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt1 ;}
                        if(iVisnIndx == 1){IDX_XVsnStt = pv.IDXR_XRearVsnStt2 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt2 ;}
                        if(iVisnIndx == 2){IDX_XVsnStt = pv.IDXR_XRearVsnStt3 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt3 ;}
                        if(iVisnIndx == 3){IDX_XVsnStt = pv.IDXR_XRearVsnStt4 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt4 ;}
                    }
                    else if (SEQ.IDXF.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying)){
                        IDX = ri.IDXF ;
                        MoveMotrIdx = SEQ.IDXF.MoveMotr ;
                        IDX_X = mi.IDXF_XFrnt ;
                        if (iVisnIndx == 0) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt1; TOOL_YVsnStt = pv.TOOL_YToolVsnStt1;}
                        if (iVisnIndx == 1) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt2; TOOL_YVsnStt = pv.TOOL_YToolVsnStt2;}
                        if (iVisnIndx == 2) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt3; TOOL_YVsnStt = pv.TOOL_YToolVsnStt3;}
                        if (iVisnIndx == 3) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt4; TOOL_YVsnStt = pv.TOOL_YToolVsnStt4;}
                    }
                    
                    //검사1방 혹은 3방 짜리는 열마다 스타트 방향이 지그제그 이고 
                    iInvWorkCol = OM.DevInfo.iTRAY_PcktCntX - iWorkCol -1 ;
                    bRearToFrnt = (iVisnIndx + iInvWorkCol) % 2 == 0 ;//검사인덱스 와 검사열을 더해서 2로 나누면 뒤에서 앞으로 인지 앞에서 뒤로 인지 나옴.
                    if (bRearToFrnt) 
                    { //장비 앞면에서 뒤로 가는 플라잉.
                        dYSttPosOfs = -iVisnMargin ;
                        dYEndPosOfs = (OM.DevInfo.iTRAY_PcktCntY-1) * OM.DevInfo.dTRAY_PcktPitchY + iVisnMargin ;                    
                    }
                    else
                    {
                        dYSttPosOfs = (OM.DevInfo.iTRAY_PcktCntY-1) * OM.DevInfo.dTRAY_PcktPitchY + iVisnMargin ;    
                        dYEndPosOfs = -iVisnMargin ;
                    }

                    MoveMotrIdx(IDX_X , IDX_XVsnStt , iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX );
                    MoveMotr   (mi.TOOL_YTool, TOOL_YVsnStt, dYSttPosOfs);

                    //if (iVisnIndx == 0) MoveMotr   (mi.TOOL_ZVisn, pv.TOOL_ZVisnWork1);
                    //if (iVisnIndx == 1) MoveMotr   (mi.TOOL_ZVisn, pv.TOOL_ZVisnWork2);
                    //if (iVisnIndx == 2) MoveMotr   (mi.TOOL_ZVisn, pv.TOOL_ZVisnWork3);
                    //if (iVisnIndx == 3) MoveMotr   (mi.TOOL_ZVisn, pv.TOOL_ZVisnWork4);

                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork1);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork2);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork3);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork4);
                   
                    Step.iCycle++;
                    return false ;

                case 303:
                    if (!MT_GetStopPos(IDX_X        , IDX_XVsnStt ,iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX )) return false;
                    if (!MT_GetStopPos(mi.TOOL_YTool, TOOL_YVsnStt, dYSttPosOfs)) return false;
                    if (!MT_GetStop(mi.TOOL_ZVisn))return false;
                    Log.Trace("VISION", "SendIndex" + iVisnIndx.ToString());
                    if(!SEQ.Visn.SendIndex(iVisnIndx) && !OM.CmnOptn.bIdleRun)
                    {
                        return true;   
                    }

                    if(!OM.CmnOptn.bIdleRun) SEQ.Visn.DeleteRslt();//검사전에 결과값 지워준다.

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 304:
                    if(!m_tmDelay.OnDelay(true, 50))return false;
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Command) && !OM.CmnOptn.bIdleRun) return false;
                    SM.MT_ResetTrgPos(mi.TOOL_YTool);
                    Step.iCycle++;
                    return false;

                case 305:                    
                    //double 
                    dSttPos = PM_GetValue(mi.TOOL_YTool, TOOL_YVsnStt);
                    //double[] 
                    dTrgPos = new double[OM.DevInfo.iTRAY_PcktCntY];
                    for (int i = 0; i < OM.DevInfo.iTRAY_PcktCntY ; i++)
                    {
                        if(bRearToFrnt) dTrgPos[i                                ] = dSttPos + (i * OM.DevInfo.dTRAY_PcktPitchY) + OM.DevOptn.dTrgOfs;
                        else            dTrgPos[OM.DevInfo.iTRAY_PcktCntY - i - 1] = dSttPos + (i * OM.DevInfo.dTRAY_PcktPitchY) + OM.DevOptn.dTrgOfs;
                    }
                    SM.MT_SetTrgPos(mi.TOOL_YTool, dTrgPos, 1000, true, true);

                    dVsnInspEnd = PM_GetValue(mi.TOOL_YTool, TOOL_YVsnStt) + dYEndPosOfs;
                    MT_GoAbsVel(mi.TOOL_YTool, dVsnInspEnd, OM.DevOptn.iInspSpeed);

                    Step.iCycle++;
                    return false ;

                case 306:
                    if (!MT_GetStop(mi.TOOL_YTool)) return false;                   

                    SM.MT_ResetTrgPos(mi.TOOL_YTool);
                    
                    Step.iCycle++;
                    return false;

                case 307:
                    if (IO_GetX(xi.VISN_Busy)) return false; //인스펙션 종료 확인        
                    if (!SEQ.Visn.FindRsltFile() && !OM.CmnOptn.bIdleRun) return false;
                    //iVisnIndx++;
                    
                    //여기 나중에 미리 보내는 것 넣자. 진섭아.
                    //if(iVisnIndx>=iTotalInspCnt) 
                    VisnCom.TRslt RsltAll = new VisnCom.TRslt();
                    
                    for(int i = 0 ; i < OM.DevInfo.iTRAY_PcktCntY ; i++){
                        if(bRearToFrnt) {
                            iIndex = i;
                            SEQ.Visn.LoadRslt(i, ref RsltAll);
                        }
                        else { //비전쪽은 첨찍은게 인덱스가 0이다.
                            iIndex = OM.DevInfo.iTRAY_PcktCntY - i - 1;
                            SEQ.Visn.LoadRslt(i, ref RsltAll);
                        }

                        if (RsltAll.Empty == 0) { DM.ARAY[ri.INSP].SetStat(0, iIndex, cs.NG0); } 
                        
                        if(DM.ARAY[ri.MASK].GetStat(iWorkCol, iWorkRow) == cs.None)
                        {
                            DM.ARAY[ri.INSP].SetStat(0, iWorkRow, cs.None);
                        }
                    }

                     for(int r = 0 ; r < OM.DevInfo.iTRAY_PcktCntY ; r++){
                        if (DM.ARAY[IDX].GetStat(iWorkCol, r) != cs.None)
                        {
                            DM.ARAY[IDX].SetStat(iWorkCol, r, DM.ARAY[ri.INSP].GetStat(0, r));
                            
                            if(DM.ARAY[IDX].GetStat(iWorkCol, r) != cs.NG0)
                            {
                               ER_SetErr(ei.PCK_CoverTray, "Corver Tray is not Empty");
                               
                               //강제로 변환시켜야 겠다.
                               DM.ARAY[IDX].SetStat(cs.Vision);
                               Step.iCycle = 0;
                               return true;
                            }
                        }
                    }
                    if(DM.ARAY[IDX].GetCntStat(cs.Vision) != 0) {
                        DM.ARAY[IDX].ChangeStat(cs.NG0, cs.Good);
                        Step.iCycle = 300;
                        return false;
                    }
                    DM.ARAY[IDX].SetStat(cs.Empty);
                    Step.iCycle = 0;
                    return true;                    
            }
        }
        //진섭아 Z포지션 추가 해서
        public bool CycleVision()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 10000))
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
            //const int iTotalInspCnt = 1 ; //진섭아 옵션추가. 1~4까지 설정 가능 하게 콤보박스. 한패키지 검사 횟수. 제일큰건 4방까지 가능.
            double dToolYVsnEndOffset = OM.DevInfo.dTRAY_PcktPitchY * (OM.DevInfo.iTRAY_PcktCntY-1) ;
            switch (Step.iCycle)
            {
                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                //초기화 패턴. 여기는 반복되지 않는다.
                case 10:
                    DM.ARAY[ri.INSP].SetStat(cs.Good);
                    for(int r = 0 ; r < DM.ARAY[ri.INSP].GetMaxRow() ; r++)
                    {
                        DM.ARAY[ri.INSP].Chip[0,r].sUnitID = "1"   ;
                        DM.ARAY[ri.INSP].Chip[0,r].sDMC1   = "1"   ;
                        DM.ARAY[ri.INSP].Chip[0,r].sDMC2   = "1"   ;
                    }
                    iVisnIndx = 0 ;

                    if(!OM.CmnOptn.bIdleRun)SEQ.Visn.SendManMode(false);
                    Log.Trace("VISION", "SendManMode false" );
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);
                    Step.iCycle++;
                    return false ;
                
                case 11:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait)) return false;
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.ManMode) && !OM.CmnOptn.bIdleRun) return false ;
                    Step.iCycle++;
                    return false;

                //아래에서 씀.
                case 12:
                    if(SEQ.IDXR.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying, OM.CmnOptn.bGoldenTray)){
                        IDX = ri.IDXR ;
                        MoveMotrIdx = SEQ.IDXR.MoveMotr ;
                        IDX_X = mi.IDXR_XRear ;
                        if(iVisnIndx == 0){IDX_XVsnStt1 = pv.IDXR_XRearVsnStt1 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt1 ;}
                        if(iVisnIndx == 1){IDX_XVsnStt2 = pv.IDXR_XRearVsnStt2 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt2 ;}
                        if(iVisnIndx == 2){IDX_XVsnStt3 = pv.IDXR_XRearVsnStt3 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt3 ;}
                        if(iVisnIndx == 3){IDX_XVsnStt4 = pv.IDXR_XRearVsnStt4 ; TOOL_YVsnStt = pv.TOOL_YToolVsnStt4 ;}
                        IDX_TrayDtct = xi.IDXR_TrayDtct ;
                    }
                    else if (SEQ.IDXF.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying, OM.CmnOptn.bGoldenTray)){
                        IDX = ri.IDXF ;
                        MoveMotrIdx = SEQ.IDXF.MoveMotr ;
                        IDX_X = mi.IDXF_XFrnt ;
                        if (iVisnIndx == 0) { IDX_XVsnStt1 = pv.IDXF_XFrntVsnStt1; TOOL_YVsnStt = pv.TOOL_YToolVsnStt1;}
                        if (iVisnIndx == 1) { IDX_XVsnStt2 = pv.IDXF_XFrntVsnStt2; TOOL_YVsnStt = pv.TOOL_YToolVsnStt2;}
                        if (iVisnIndx == 2) { IDX_XVsnStt3 = pv.IDXF_XFrntVsnStt3; TOOL_YVsnStt = pv.TOOL_YToolVsnStt3;}
                        if (iVisnIndx == 3) { IDX_XVsnStt4 = pv.IDXF_XFrntVsnStt4; TOOL_YVsnStt = pv.TOOL_YToolVsnStt4;}
                        IDX_TrayDtct = xi.IDXF_TrayDtct ;
                    }

                    //가끔 픽사이클에서 WorkChip 세팅 하고 에러 났을때 문제가 됨.
                    DM.ARAY[IDX].ClearWorkChip();
                    
                    //검사1방 혹은 3방 짜리는 열마다 스타트 방향이 지그제그 이고 
                    iInvWorkCol = OM.DevInfo.iTRAY_PcktCntX - iWorkCol -1 ;
                    bRearToFrnt = (iVisnIndx + iInvWorkCol) % 2 == 0 ;//검사인덱스 와 검사열을 더해서 2로 나누면 뒤에서 앞으로 인지 앞에서 뒤로 인지 나옴.
                    if (bRearToFrnt) 
                    { //장비 앞면에서 뒤로 가는 플라잉.
                        dYSttPosOfs = -iVisnMargin ;
                        dYEndPosOfs = (OM.DevInfo.iTRAY_PcktCntY-1) * OM.DevInfo.dTRAY_PcktPitchY + iVisnMargin ;                    
                    }
                    else
                    {
                        dYSttPosOfs = (OM.DevInfo.iTRAY_PcktCntY-1) * OM.DevInfo.dTRAY_PcktPitchY + iVisnMargin ;    
                        dYEndPosOfs = -iVisnMargin ;
                    }

                    MoveMotrIdx(IDX_X , IDX_XVsnStt , iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX );
                    MoveMotr   (mi.TOOL_YTool, TOOL_YVsnStt, dYSttPosOfs);

                    //if (iVisnIndx == 0) MoveMotr   (mi.TOOL_ZVisn, pv.TOOL_ZVisnWork1);
                    //if (iVisnIndx == 1) MoveMotr   (mi.TOOL_ZVisn, pv.TOOL_ZVisnWork2);
                    //if (iVisnIndx == 2) MoveMotr   (mi.TOOL_ZVisn, pv.TOOL_ZVisnWork3);
                    //if (iVisnIndx == 3) MoveMotr   (mi.TOOL_ZVisn, pv.TOOL_ZVisnWork4);

                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork1);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork2);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork3);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork4);
                   
                    Step.iCycle++;
                    return false ;

                case 13:
                    if (!MT_GetStopPos(IDX_X        , IDX_XVsnStt ,iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX )) return false;
                    if (!MT_GetStopPos(mi.TOOL_YTool, TOOL_YVsnStt, dYSttPosOfs)) return false;
                    if (!MT_GetStop(mi.TOOL_ZVisn))return false;

                    //20180305 여기에 요청으로 센싱해서 알람 띄울것인데.
                    //로더 밑으로 인덱스가 들어가면 로더 상단에 자제 올라가 있는 것이 감지 되는 경우 있어서 조심스러워 옵션 처리함.
                    if(OM.CmnOptn.bIdxDetectVisnZone && !OM.CmnOptn.bIdleRun)
                    {
                        if (!IO_GetX(IDX_TrayDtct) && iInvWorkCol == 0) //괜히 에러 늘리지 말고 맨 처음 한번만 해보자.
                        {
                            ER_SetErr(ei.PKG_Dispr, "Vision Zone Index tray detect sensor Not detected.");
                            return true;
                        }
                    }



                    Log.Trace("VISION", "SendIndex" + iVisnIndx.ToString());
                    if(!SEQ.Visn.SendIndex(iVisnIndx) && !OM.CmnOptn.bIdleRun)
                    {
                        //SendIndex 함수 내부에서 레디 안되어 있으면 에러.
                        return true;   //Why Return true??
                        //return false ;
                    }

                    if (!OM.CmnOptn.bIdleRun && !IO_GetX(xi.RAIL_TrayDtct2) && iWorkCol != 0)//sensor zone is over when col 0
                    {
                        ER_SetErr(ei.PRT_Missed , IO_GetXName(xi.RAIL_TrayDtct2) +" is Not Detected!");
                        return true ;
                    }

                    if(!OM.CmnOptn.bIdleRun) {
                        if(!SEQ.Visn.DeleteRslt()){//검사전에 결과값 지워준다.     2018 0221 혹시 문제 소지가 있을듯 해서 추가.                        
                            ER_SetErr(ei.VSN_ComErr , "Failed to delete VisionResultFile!");
                            return true ;
                        }
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!m_tmDelay.OnDelay(true, 50))return false;
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Command) && !OM.CmnOptn.bIdleRun) return false;
                    SM.MT_ResetTrgPos(mi.TOOL_YTool);
                    Step.iCycle++;
                    return false;

                case 15:                    
                    double dSttPos = PM_GetValue(mi.TOOL_YTool, TOOL_YVsnStt);
                    double[] dTrgPos = new double[OM.DevInfo.iTRAY_PcktCntY];
                    for (int i = 0; i < OM.DevInfo.iTRAY_PcktCntY ; i++)
                    {
                        if(bRearToFrnt) dTrgPos[i                                ] = dSttPos + (i * OM.DevInfo.dTRAY_PcktPitchY) + OM.DevOptn.dTrgOfs;
                        else            dTrgPos[OM.DevInfo.iTRAY_PcktCntY - i - 1] = dSttPos + (i * OM.DevInfo.dTRAY_PcktPitchY) + OM.DevOptn.dTrgOfs;
                    }
                    SM.MT_SetTrgPos(mi.TOOL_YTool, dTrgPos, 1000, true, true);

                    double dVsnInspEnd = PM_GetValue(mi.TOOL_YTool, TOOL_YVsnStt) + dYEndPosOfs;
                    MT_GoAbsVel(mi.TOOL_YTool, dVsnInspEnd, OM.DevOptn.iInspSpeed);

                    Step.iCycle++;
                    return false ;

                case 16:
                    if (!MT_GetStop(mi.TOOL_YTool)) return false;                   

                    SM.MT_ResetTrgPos(mi.TOOL_YTool);
                    m_tmDelay.Clear();
                    
                    Step.iCycle++;
                    return false;

                case 17:
                    if (m_tmDelay.OnDelay(9000))
                    {
                        ER_SetErr(ei.VSN_ComErr, "Vision Program Busy");
                        return true ;
                    }
                    if (IO_GetX(xi.VISN_Busy)) return false; //인스펙션 종료 확인        
                    if (!SEQ.Visn.FindRsltFile() && !OM.CmnOptn.bIdleRun) return false; //이상하게 비지를 내리기 전에 파일을 쓰고 내리는데 파일을 못읽는 경우가 있어서..
                    
                    //20180221 오라클에 자꾸 UnitID  UnitDMC1 UnitDMC2 가 1 , 1, 1 로 남는 경우가 생겨서. 확인 차 로그 남기고 딜레이 50미리 주고 다시 남기고 에러 띄움.
                    VisnCom.TRslt RsltTemp = new VisnCom.TRslt();
                    int iIndexTemp = 0;
                    for(int i = 0 ; i < OM.DevInfo.iTRAY_PcktCntY ; i++){
                        if(bRearToFrnt) {
                            iIndexTemp = i;
                            SEQ.Visn.LoadRslt(i , ref RsltTemp);
                        }
                        else { //비전쪽은 첨찍은게 인덱스가 0이다.
                            iIndexTemp = OM.DevInfo.iTRAY_PcktCntY - i - 1;
                            SEQ.Visn.LoadRslt(i , ref RsltTemp);
                        }

                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.Empty         = " + RsltTemp.Empty        .ToString());
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.UnitID        = " + RsltTemp.UnitID                  );
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.UnitDMC1      = " + RsltTemp.UnitDMC1                );
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.UnitDMC2      = " + RsltTemp.UnitDMC2                );
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.GlobtopLeft   = " + RsltTemp.GlobtopLeft  .ToString());
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.GlobtopTop    = " + RsltTemp.GlobtopTop   .ToString());
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.GlobtopRight  = " + RsltTemp.GlobtopRight .ToString());
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.GlobtopBottom = " + RsltTemp.GlobtopBottom.ToString());
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.MatchingError = " + RsltTemp.MatchingError.ToString());
                        Log.Trace("17_VisionResult"+iIndexTemp.ToString() , "RsltTemp.UserDefine    = " + RsltTemp.UserDefine   .ToString());
                    }

                    //==============================================================================================================
                    m_tmDelay.Clear();
                    
                    Step.iCycle++;
                    return false;
                    

                case 18:
                    if (!m_tmDelay.OnDelay(20)) return false ;

                    VisnCom.TRslt Rslt = new VisnCom.TRslt();
                    int iIndex = 0;
                    bool bExist111 = false ;

                    //로그 2번째.==========================================================
                    for(int i = 0 ; i < OM.DevInfo.iTRAY_PcktCntY ; i++){
                        if(bRearToFrnt) {
                            iIndexTemp = i;
                            SEQ.Visn.LoadRslt(i , ref Rslt);
                        }
                        else { //비전쪽은 첨찍은게 인덱스가 0이다.
                            iIndexTemp = OM.DevInfo.iTRAY_PcktCntY - i - 1;
                            SEQ.Visn.LoadRslt(i , ref Rslt);
                        }

                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.Empty         = " + Rslt.Empty        .ToString());
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.UnitID        = " + Rslt.UnitID                  );
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.UnitDMC1      = " + Rslt.UnitDMC1                );
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.UnitDMC2      = " + Rslt.UnitDMC2                );
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.GlobtopLeft   = " + Rslt.GlobtopLeft  .ToString());
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.GlobtopTop    = " + Rslt.GlobtopTop   .ToString());
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.GlobtopRight  = " + Rslt.GlobtopRight .ToString());
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.GlobtopBottom = " + Rslt.GlobtopBottom.ToString());
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.MatchingError = " + Rslt.MatchingError.ToString());
                        Log.Trace("18_1_VisionResult"+iIndexTemp.ToString() , "Rslt.UserDefine    = " + Rslt.UserDefine   .ToString());

                        if(Rslt.Empty        !=0 &&  
                           //Rslt.MixDevice    !=0 &&
                           Rslt.GlobtopLeft  !=0 && 
                           Rslt.GlobtopTop   !=0 && 
                           Rslt.GlobtopRight !=0 && 
                           Rslt.GlobtopBottom!=0 &&  
                           Rslt.MatchingError!=0 &&  
                           Rslt.UserDefine   !=0 ){
                            if(Rslt.UnitID =="1"  && Rslt.UnitDMC1 == "1"  && Rslt.UnitDMC2 == "1")
                            {
                                bExist111 = true ;
                            }

                        }                        
                    }

                    if(bExist111) 
                    {
                        ER_SetErr(ei.VSN_InspNG , "UnitID is 1 , UnitDMC1 is 1 , UnitDMC2 is 1");
                        return true ;

                    }

                    //==================================================================================================











                    for(int i = 0 ; i < OM.DevInfo.iTRAY_PcktCntY ; i++){
                        if(bRearToFrnt) {
                            iIndex = i;
                            SEQ.Visn.LoadRslt(i , ref Rslt);
                        }
                        else { //비전쪽은 첨찍은게 인덱스가 0이다.
                            iIndex = OM.DevInfo.iTRAY_PcktCntY - i - 1;
                            SEQ.Visn.LoadRslt(i , ref Rslt);
                        }

                        //3차 로그 실재로 쓰는 로그.
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.Empty         = " + Rslt.Empty        .ToString());
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.UnitID        = " + Rslt.UnitID                  );
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.UnitDMC1      = " + Rslt.UnitDMC1                );
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.UnitDMC2      = " + Rslt.UnitDMC2                );
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.GlobtopLeft   = " + Rslt.GlobtopLeft  .ToString());
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.GlobtopTop    = " + Rslt.GlobtopTop   .ToString());
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.GlobtopRight  = " + Rslt.GlobtopRight .ToString());
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.GlobtopBottom = " + Rslt.GlobtopBottom.ToString());
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.MatchingError = " + Rslt.MatchingError.ToString());
                        Log.Trace("18_2_VisionResult"+iIndex.ToString() , "Rslt.UserDefine    = " + Rslt.UserDefine   .ToString());


                             if(Rslt.Empty        ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG0 );} 
                        //else if(Rslt.MixDevice    ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG1 );} //이것은 비전에서 쓰는 것이 아니고 핸들러에서 오라클 관련 에러 일때 쓰는 것이다.
                        else if(Rslt.UnitID       =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG2 );}//바코드들은 "0"이면 있는데 못읽음. "1"바코드가 검사에 없음. "바코드문자" 읽기 성공.
                        else if(Rslt.UnitDMC1     =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG3 );}
                        else if(Rslt.UnitDMC2     =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG4 );}
                        else if(Rslt.GlobtopLeft  ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG5 );}
                        else if(Rslt.GlobtopTop   ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG6 );}
                        else if(Rslt.GlobtopRight ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG7 );}
                        else if(Rslt.GlobtopBottom==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG8 );} 
                        else if(Rslt.MatchingError==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG9 );} 
                        else if(Rslt.UserDefine   ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG10);}

                        if (!OM.CmnOptn.bIdleRun)
                        {
                            if (!OM.CmnOptn.bOracleNotUse)//칩에
                            {
                                if (Rslt.UnitID   != "1")DM.ARAY[ri.INSP].Chip[0,iIndex].sUnitID = Rslt.UnitID   ; 
                                if (Rslt.UnitDMC1 != "1")DM.ARAY[ri.INSP].Chip[0,iIndex].sDMC1   = Rslt.UnitDMC1 ; 
                                if (Rslt.UnitDMC2 != "1")DM.ARAY[ri.INSP].Chip[0,iIndex].sDMC2   = Rslt.UnitDMC2 ; 
                            }
                        }

                        if(OM.CmnOptn.bIdleRun){
                            DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.Good);
                        }
  
                        if(DM.ARAY[ri.MASK].GetStat(iWorkCol, iWorkRow) == cs.None)
                        {
                            DM.ARAY[ri.INSP].SetStat(0, iWorkRow, cs.None);
                        }
                    }

                    iVisnIndx++;
                    if(iVisnIndx<=OM.DevOptn.iTotalInspCnt) {
                        Step.iCycle = 12 ;
                        return false ;
                    }                                     

                    if (!OM.CmnOptn.bIdleRun)
                    {
                        if (!OM.CmnOptn.bOracleNotUse)
                        {
                            string sErrMsgs = "";
                            string sErrMsg  = "";
                            string sFullMsgs = "";
                            int iRow = 0 ;
                            int iCol = 0 ;
                            int iPocketID = 0;
                            cs InspStat = cs.Good ;
                            for(int r = 0 ; r < OM.DevInfo.iTRAY_PcktCntY ; r++){                                
                                if (DM.ARAY[ri.MASK].GetStat(iWorkCol, r) != cs.None)//마스킹 부분 제끼고.
                                {
                                    iCol = iWorkCol ;
                                    iRow = r        ;
                                    sErrMsg  = "";
                                    if (!CheckOracleNG(ri.INSP, 0, iRow, ref sErrMsg))
                                    {
                                        if (!OM.CmnOptn.bGoldenTray)
                                        {
                                            sErrMsgs += iRow.ToString() + "-" + sErrMsg +"\r\n";
                                        }
                                        //이함수 안에서 오라클 에러 난 상태
                                    }

                                    iPocketID = GetPocketID (iCol , iRow) ;
                                    InspStat = DM.ARAY[ri.INSP].GetStat(0, r) ;
                                    if (InspStat == cs.Good)//여기서는 굿만 담아야 된다.
                                    {
                                        string sQuery ="";
                                        if(!SEQ.Oracle.GetUnitInspectionQuery(iPocketID.ToString() , DM.ARAY[ri.INSP].Chip[0 , iRow].sUnitID , DM.ARAY[ri.INSP].Chip[0 , iRow].sDMC1 , DM.ARAY[ri.INSP].Chip[0 , iRow].sDMC2 , InspStat ,OM.CmnOptn.sMachinID , ref sQuery)){
                                            SM.ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                                        }
                                        else
                                        {
                                            DM.ARAY[ri.INSP].Chip[0, r].sQuery = sQuery ;
                                        }
                                        //if(!SEQ.Oracle.AddUnitInspectionQuery(iPocketID.ToString() , DM.ARAY[ri.INSP].Chip[0 , iRow].sUnitID , DM.ARAY[ri.INSP].Chip[0 , iRow].sDMC1 , DM.ARAY[ri.INSP].Chip[0 , iRow].sDMC2 , InspStat ,OM.CmnOptn.sMachinID )){
                                        //    //이렇게 되면 트레이 다시 돌려야 되네..
                                        //    SM.ER_SetErr(ei.ETC_Oracle , "AddUnitInspectionQuery Failed - " + SEQ.Oracle.GetLastMsg());
                                        //}
                                        sFullMsgs += iRow.ToString() + "-OK\r\n";
                                    }
                                    else if(InspStat == cs.NG0 ){sFullMsgs += iRow.ToString() + "-OK\r\n";}
                                    else if(InspStat == cs.NG1 ){sFullMsgs += iRow.ToString() + "-"+sErrMsg+"\r\n";}
                                    else if(InspStat == cs.NG2 ){sFullMsgs += iRow.ToString() + "-V_UnitID        \r\n";}
                                    else if(InspStat == cs.NG3 ){sFullMsgs += iRow.ToString() + "-V_UnitDMC1      \r\n";}
                                    else if(InspStat == cs.NG4 ){sFullMsgs += iRow.ToString() + "-V_UnitDMC2      \r\n";}
                                    else if(InspStat == cs.NG5 ){sFullMsgs += iRow.ToString() + "-V_GlobtopLeft   \r\n";}
                                    else if(InspStat == cs.NG6 ){sFullMsgs += iRow.ToString() + "-V_GlobtopTop    \r\n";}
                                    else if(InspStat == cs.NG7 ){sFullMsgs += iRow.ToString() + "-V_GlobtopRight  \r\n";}
                                    else if(InspStat == cs.NG8 ){sFullMsgs += iRow.ToString() + "-V_GlobtopBottom \r\n";}
                                    else if(InspStat == cs.NG9 ){sFullMsgs += iRow.ToString() + "-V_MatchingError \r\n";}
                                    else if(InspStat == cs.NG10){sFullMsgs += iRow.ToString() + "-V_UserDefine    \r\n";}
                                    
                                }
                            }
                            if(sErrMsgs != "")ER_SetErr(ei.ETC_Oracle, sFullMsgs);
                        }
                    }


                    for(int r = 0 ; r < OM.DevInfo.iTRAY_PcktCntY ; r++){

                        if (DM.ARAY[IDX].GetStat(iWorkCol, r) != cs.None)
                        {
                            //일단 리젝에서 빼온것은 카운팅 하지 않고 순수하게 해당 랏 트레이에서만 카운팅을 한다.
                            //나중에 카운팅 다시 해야 하면 SPC.LOT.Data.WorkCnt는 아웃 할때마다 60개씩 더해 주고
                            //
                            //여기서 카운팅 하면 재검시에 굿으로 떨어지면 난처하므로.
                            //픽 할때와 엠티 체크 할때 카운팅 하는것으로.
                            DM.ARAY[IDX].SetStat(iWorkCol, r, DM.ARAY[ri.INSP].GetStat(0, r));
                            DM.ARAY[IDX].Chip[iWorkCol,r].sQuery = DM.ARAY[ri.INSP].Chip[0, r].sQuery  ;

                            SPC.LOT.Data.WorkCnt +=1 ;
                        }
                    }
                   
                    Step.iCycle = 0;
                    return true;

            }
        }

        //return false  : 에러발생상태.
        //retrun true   : 에러 없는 상태.
        public bool CheckOracleNG(int _iArayID , int _iCol , int _iRow , ref string  _sErrMsg)
        {
            string sUnitID = DM.ARAY[_iArayID].Chip[_iCol,_iRow].sUnitID ;
            string sDMC1   = DM.ARAY[_iArayID].Chip[_iCol,_iRow].sDMC1   ;
            string sDMC2   = DM.ARAY[_iArayID].Chip[_iCol,_iRow].sDMC2   ;

            bool bNeedCheckUnitID = sUnitID != "1" && sUnitID != "0" ; //1은 검사가 없어서 그냥 굿. 0은 검사가 있는데 못읽음 그냥페일 "ㄴㅇㄹㅇㄴㄹㄴ"는 읽음.
            bool bNeedChecksDMC1  = sDMC1   != "1" && sDMC1   != "0" && OM.DevOptn.bDMC1Grouping ; //1은 검사가 없어서 그냥 굿. 0은 검사가 있는데 못읽음 그냥페일 "ㄴㅇㄹㅇㄴㄹㄴ"는 읽음.
            bool bNeedChecksDMC2  = sDMC2   != "1" && sDMC2   != "0" ; //1은 검사가 없어서 그냥 굿. 0은 검사가 있는데 못읽음 그냥페일 "ㄴㅇㄹㅇㄴㄹㄴ"는 읽음.

            SEQ.Oracle.SendMsg("Check UnitID:"+sUnitID + " DMC1:"+sDMC1 + " DMC2:" + sDMC2);

           

            //
            if (bNeedChecksDMC1)
            {
                string sSupplyerCode = "";
                if (sDMC1.Length < 2) {    
                    _sErrMsg = "Short DMC1-"+sDMC1 ;
                    DM.ARAY[_iArayID].SetStat(_iCol,_iRow,cs.NG1 );
                    return false ;
                }
                else { 
                    sSupplyerCode = sDMC1.Substring(0,2);
                }

                if(SEQ.Oracle.Stat.lsUnitInspection_DMC1.Contains(sDMC1)){
                    _sErrMsg = "Duplicate DMC1-"+sDMC1 ;
                    DM.ARAY[_iArayID].SetStat(_iCol,_iRow,cs.NG1 );
                    return false ;
                }
            
                if (!SEQ.Oracle.Stat.sTrayInfomation_DMC1.Contains(sSupplyerCode)) {
                    _sErrMsg = "Supplyer Code:"+sSupplyerCode + " is not in "+ SEQ.Oracle.Stat.sTrayInfomation_DMC1 ;
                    DM.ARAY[_iArayID].SetStat(_iCol,_iRow,cs.NG1 );
                    return false ;
                }
            }

            if(bNeedChecksDMC2)
            {
                if(OM.DevOptn.iDMC2CheckMathod == 1) { //문자열 검사방식.
                    if (!SEQ.Oracle.CheckDMC2CharacterCompare(sDMC2)) { 
                        _sErrMsg = SEQ.Oracle.GetLastMsg();
                        DM.ARAY[_iArayID].SetStat(_iCol,_iRow,cs.NG1 );
                        return false ;
                    }
                }
                else if (OM.DevOptn.iDMC2CheckMathod == 2){ //스펙비교방식.
                    if (!SEQ.Oracle.CheckDMC2_Spec(sDMC2)) { 
                        _sErrMsg = SEQ.Oracle.GetLastMsg();
                        DM.ARAY[_iArayID].SetStat(_iCol,_iRow,cs.NG1 );
                        return false ;
                    }
                }
            }

            if (bNeedCheckUnitID)
            {
                if (sUnitID.Length < 8) {    
                    _sErrMsg = "Short UnitID-"+sUnitID ; 
                    DM.ARAY[_iArayID].SetStat(_iCol,_iRow,cs.NG1 );
                    return false ;
                }
                string sPanelID = sUnitID.Substring(0,2) + sUnitID.Substring(4,4);

                if (!SEQ.Oracle.Stat.lsProbeFile_ProbeFile.Contains(sPanelID)){
                    _sErrMsg = "PanelID:"+ sPanelID + " dosn't exist in ProbeFileList";
                    DM.ARAY[_iArayID].SetStat(_iCol,_iRow,cs.NG1 );
                    return false ;
                }
            }
            
            return true ;
        }

        int GetPocketID (int _iCol , int _iRow)
        {
            int iPockID = 0 ;
            for (int r = OM.DevInfo.iTRAY_PcktCntY - 1; r >= 0; r--)
            {
                for(int c = 0 ; c < OM.DevInfo.iTRAY_PcktCntX ; c++)
                {
                    if(DM.ARAY[ri.MASK].GetStat(iWorkCol, iWorkRow) != cs.None)iPockID++;
                    if(_iCol == c && _iRow == r)return iPockID ;
                }
            }
            return iPockID ;
        }

        public int iNGPickCnt;
        public bool CycleNGPick()
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
                    iVisnIndx = 0;
                    iVisnRtry = 0;
                    SEQ.Visn.DeleteRslt();// ManRslt();//검사전에 결과값 지워준다.
                    

                    if(SEQ.IDXR.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying)){
                        IDX = ri.IDXR ;
                        MoveMotrIdx = SEQ.IDXR.MoveMotr ;
                        IDX_X = mi.IDXR_XRear ;
                        if(iVisnIndx == 0){IDX_XVsnStt = pv.IDXR_XRearVsnStt1 ; }
                        if(iVisnIndx == 1){IDX_XVsnStt = pv.IDXR_XRearVsnStt2 ; }
                        if(iVisnIndx == 2){IDX_XVsnStt = pv.IDXR_XRearVsnStt3 ; }
                        if(iVisnIndx == 3){IDX_XVsnStt = pv.IDXR_XRearVsnStt4 ; }

                        IDX_XWorkStt = pv.IDXR_XRearWorkStt ; 
                    }
                    else if (SEQ.IDXF.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying)){
                        IDX = ri.IDXF ;
                        MoveMotrIdx = SEQ.IDXF.MoveMotr ;
                        IDX_X = mi.IDXF_XFrnt ;
                        if (iVisnIndx == 0) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt1; }
                        if (iVisnIndx == 1) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt2; }
                        if (iVisnIndx == 2) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt3; }
                        if (iVisnIndx == 3) { IDX_XVsnStt = pv.IDXF_XFrntVsnStt4; }

                        IDX_XWorkStt = pv.IDXF_XFrntWorkStt ;
                    }
                    
                    iInvWorkCol = OM.DevInfo.iTRAY_PcktCntX - iWorkCol - 1;
                    SEQ.Visn.SendManMode(true);
                    Log.Trace("VISION", "SendManMode true" );
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.ManMode))return false ;
                    //미리 굿으로 때려 놓고.
                    //멀티샷 검사 일때 페일 난것만 덧씌운다.
                    //일단 비전쪽에서 티칭 안한경우는 굿으로 빼니깐 초기값은 1
                    DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sUnitID = "1" ;
                    DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC1   = "1" ;
                    DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC2   = "1" ;
                    
                    Step.iCycle++;
                    return false ;

                //using later step.
                case 12:
                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt1, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt2, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt3, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt4, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);

                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork1);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork2);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork3);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork4);


                    MoveMotrIdx(IDX_X , IDX_XVsnStt , iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX );

                    if(!SEQ.Visn.DeleteRslt()){//검사전에 결과값 지워준다.     2018 0221 혹시 문제 소지가 있을듯 해서 추가.                        
                        ER_SetErr(ei.VSN_ComErr , "Failed to delete VisionResultFile!");
                        return true ;
                    }
                    

                    SEQ.Visn.SendIndex(iVisnIndx);
                    Log.Trace("VISION", "SendIndex" + iVisnIndx.ToString());
                    
                    
                    Step.iCycle++;
                    return false;

                
                case 13:
                    if(!MT_GetStopInpos(mi.TOOL_ZVisn))return false;
                    if (!MT_GetStopInpos(mi.TOOL_YTool)) return false;
                    if (!MT_GetStop(IDX_X)) return false;
                    if(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Command))return false;
                    
                    if (!OM.CmnOptn.bIdleRun && !IO_GetX(xi.RAIL_TrayDtct2) && iWorkCol != 0)//sensor zone is over when col 0
                    {
                        ER_SetErr(ei.PRT_Missed , IO_GetXName(xi.RAIL_TrayDtct2) +" is Not Detected!");
                        return true ;
                    }
                    
                    Step.iCycle++;
                    return false;


                case 14: 
                    
                    SEQ.Visn.SendManInsp();
                    Log.Trace("VISION", "SendManInsp" );
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Insp)) return false;
                    if(!SEQ.Visn.FindRsltFile())return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    VisnCom.TRslt Rslt = new VisnCom.TRslt();
                    
                    SEQ.Visn.LoadManRslt(ref Rslt);

                    if(Rslt.Empty         == 0 &&
                       Rslt.MixDevice     == 0 &&
                       Rslt.UnitID        == null &&
                       Rslt.UnitDMC1      == null &&
                       Rslt.UnitDMC2      == null &&
                       Rslt.GlobtopLeft   == 0 &&
                       Rslt.GlobtopTop    == 0 &&
                       Rslt.GlobtopRight  == 0 &&
                       Rslt.GlobtopBottom == 0 &&
                       Rslt.MatchingError == 0 &&
                       Rslt.UserDefine    == 0 ) return false;
                    

                    DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , cs.Good) ;
                    
                    //멀티샷검사시에 검사영역이 아니면 1이 날라와서 
                    if(Rslt.UnitID       !="1" ) DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sUnitID = Rslt.UnitID   ;
                    if(Rslt.UnitDMC1     !="1" ) DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC1   = Rslt.UnitDMC1 ;
                    if(Rslt.UnitDMC2     !="1" ) DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC2   = Rslt.UnitDMC2 ;

                    //바코드들은 "0"이면 있는데 못읽음. "1"바코드가 검사에 없음. "바코드문자" 읽기 성공.
                         if(Rslt.Empty        ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG0 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG0);} 
                    //else if(Rslt.MixDevice    ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG1 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG1);}
                    else if(Rslt.UnitID       =="0" ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG2 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG2);}
                    else if(Rslt.UnitDMC1     =="0" ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG3 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG3);}
                    else if(Rslt.UnitDMC2     =="0" ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG4 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG4);}
                    else if(Rslt.GlobtopLeft  ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG5 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG5);}
                    else if(Rslt.GlobtopTop   ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG6 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG6);}
                    else if(Rslt.GlobtopRight ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG7 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG7);}
                    else if(Rslt.GlobtopBottom==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG8 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG8);} 
                    else if(Rslt.MatchingError==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG9 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG8);} 
                    else if(Rslt.UserDefine   ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG10);}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG8);} 


                    //2018 02 21 자꾸 1 1 1 나오는 것 때문에
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.Empty         = " + Rslt.Empty        .ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.UnitID        = " + Rslt.UnitID                  );
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.UnitDMC1      = " + Rslt.UnitDMC1                );
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.UnitDMC2      = " + Rslt.UnitDMC2                );
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.GlobtopLeft   = " + Rslt.GlobtopLeft  .ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.GlobtopTop    = " + Rslt.GlobtopTop   .ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.GlobtopRight  = " + Rslt.GlobtopRight .ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.GlobtopBottom = " + Rslt.GlobtopBottom.ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.MatchingError = " + Rslt.MatchingError.ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.UserDefine    = " + Rslt.UserDefine   .ToString());




                    
                    //멀티샷 검사일경우.
                    if (iVisnIndx < OM.DevOptn.iTotalInspCnt) //combobox index + 1
                    {
                        iVisnIndx++;
                        
                        Step.iCycle = 12;
                        return false;
                    }//여기까지가 멀티샷 검사 끝내고 오라클용 스트링 세팅 끝난상황.

                    //비전 페일시 리트라이.
                    if (iVisnRtry < OM.DevOptn.iNgInspCnt && DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow) != cs.Good)
                    {
                        iVisnRtry++;
                        iVisnIndx = 0;
                        Step.iCycle = 12;
                        return false;
                    }
                                    
                    if (!OM.CmnOptn.bIdleRun && !OM.CmnOptn.bOracleNotUse)
                    {
                        int iPocketID = 0;
                        cs InspStat = cs.Good ;                             
                        string sErrMsg = "";
                        if (!CheckOracleNG(IDX, iWorkCol, iWorkRow , ref sErrMsg))
                        {
                            //이함수 안에서 믹스빈 에러(SetErr) 난 상태 안에서 마스키 하고 에러 띄우고 해서.
                            Log.Trace("CheckOracleNG NG" , sErrMsg);
                        }

                        iPocketID = GetPocketID (iWorkCol, iWorkRow) ;
                        InspStat = DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow) ;

                        if (InspStat == cs.Good)//재검 했는데 굿으로 되었음.
                        {
                            string sQuery ="";
                            if(!SEQ.Oracle.GetUnitInspectionQuery(iPocketID.ToString() , 
                                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sUnitID , 
                                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC1   , 
                                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC2   , 
                                                                  cs.Good ,OM.CmnOptn.sMachinID , ref sQuery)){
                                SM.ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                                DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , iWorkStat);
                                return true ;
                            }
                            else
                            {
                                DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sQuery = sQuery ;
                            }         
                            return true ;
                        }
                        else//재검 했는데 페일임.
                        {
                            if(!SEQ.Oracle.InsertUnitInspectionQuery(iPocketID.ToString() , 
                                DM.ARAY[IDX].Chip[iWorkCol , iWorkRow].sUnitID     , 
                                DM.ARAY[IDX].Chip[iWorkCol , iWorkRow].sDMC1       , 
                                DM.ARAY[IDX].Chip[iWorkCol , iWorkRow].sDMC2       , 
                                InspStat ,OM.CmnOptn.sMachinID)){//Get Query And Insert 
                                
                                DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , iWorkStat) ; //쿼리 에러났으니깐 원래 맨처음 상태로 되돌리고 에러 띄우면 다시 검사 하기에 ...
                                SM.ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                                Step.iCycle=0;
                                return true;
                            }
                        }                        
                    }

                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG0 ){SPC.LOT.Data.Empty         += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG1 ){SPC.LOT.Data.MixDevice     += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG2 ){SPC.LOT.Data.UnitID        += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG3 ){SPC.LOT.Data.UnitDMC1      += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG4 ){SPC.LOT.Data.UnitDMC2      += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG5 ){SPC.LOT.Data.GlobtopLeft   += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG6 ){SPC.LOT.Data.GlobtopTop    += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG7 ){SPC.LOT.Data.GlobtopRight  += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG8 ){SPC.LOT.Data.GlobtopBottom += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG9 ){SPC.LOT.Data.MatchingError += 1 ;}
                    if(DM.ARAY[IDX].GetStat(iWorkCol , iWorkRow) == cs.NG10){SPC.LOT.Data.UserDefine    += 1 ;}

                    if (DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow) == cs.Good) { 
                        return true ;
                    }
                    
                    Log.Trace("VISION", "SendManMode" + false.ToString() );
                    Step.iCycle++;
                    return false;

                case 17:
                    SEQ.Visn.SendManMode(false);
                    MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait);
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!MT_GetStopPos(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait)) return false;
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;
                    if(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.ManMode))return false ;
                    MoveMotrIdx(IDX_X, IDX_XWorkStt   , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX);
                    MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolIdxWorkStt, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);

                    Step.iCycle++;
                    return false;

                
                case 19:
                    if (!MT_GetStopPos(IDX_X, IDX_XWorkStt , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX)) return false;
                    if (!MT_GetStopPos(mi.TOOL_YTool, pv.TOOL_YToolIdxWorkStt, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY)) return false;
                    iNGPickCnt = 0 ;
                    Step.iCycle++;
                    return false;

                //아래서씀. Pick Miss 발생
                case 20:
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrIdxPick);
                  
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrIdxPick)) return false;
                    IO_SetY(yi.TOOL_PckrVac, true);
                    DM.ARAY[IDX].SetWorkChip(iWorkCol, iWorkRow);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPickDelay)) return false;
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 23:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;

                    if (!IO_GetX(xi.TOOL_PckrVac))
                    {
                        iNGPickCnt++;
                        if (iNGPickCnt < OM.CmnOptn.iPickRtryCnt) //이거 나중에 옵션으로 바꿔야할수도 있음. 사양서에 Pick 3번으로 명시돼있음
                        {
                            Step.iCycle = 20;
                            return false;
                        }
                        else
                        {
                            ER_SetErr(ei.PCK_PickMiss, "Reject Picker Pick Miss Error");
                            DM.ARAY[ri.PCKR].SetStat(cs.None);
                            return true;
                        }
                        
                    }

                    Step.iCycle++;
                    return false;

                case 24:
                    DM.ARAY[ri.PCKR].SetStat(DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow));
                    DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.Empty);

                    

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleNGPlace()
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

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;
            FindChip(ri.TRYF, out c, out r, cs.Empty);

            double dOfsX = (DM.ARAY[ri.TRYF].GetMaxCol() - c - 1) * OM.DevInfo.dTRAY_PcktPitchX;
            double dOfsY = r * OM.DevInfo.dTRAY_PcktPitchY;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:

                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;
                    MoveMotr(mi.TOOL_XRjct, pv.TOOL_XRjctWrkStt     , dOfsX);
                    MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolNgTWorkStt , dOfsY);

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.TOOL_XRjct, pv.TOOL_XRjctWrkStt    , dOfsX)) return false;
                    if (!MT_GetStopPos(mi.TOOL_YTool, pv.TOOL_YToolNgTWorkStt, dOfsY)) return false;
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrNgTWork);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrNgTWork)) return false;
                    IO_SetY(yi.TOOL_PckrVac, false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;
                    if (IO_GetX(xi.TOOL_PckrVac))
                    {
                        ER_SetErr(ei.PRT_VacErr, "Picker Vacuum Sensor On Error");
                        return true;
                    }
                    DM.ARAY[ri.TRYF].SetStat(c, r, DM.ARAY[ri.PCKR].GetStat(0,0));
                    DM.ARAY[ri.PCKR].SetStat(cs.None);
                    Step.iCycle = 0;
                    return true;
              
            }
        }

        public bool CycleEmptyCheck()
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
                    if(SEQ.IDXR.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying)){
                        IDX = ri.IDXR ;
                        MoveMotrIdx = SEQ.IDXR.MoveMotr ;
                        IDX_X = mi.IDXR_XRear ;
                        IDX_XWorkStt = pv.IDXR_XRearWorkStt ; 
                    }
                    else if (SEQ.IDXF.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying)){
                        IDX = ri.IDXF ;
                        MoveMotrIdx = SEQ.IDXF.MoveMotr ;
                        IDX_X = mi.IDXF_XFrnt ;
                        IDX_XWorkStt = pv.IDXF_XFrntWorkStt ;
                    }
                    
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    iInvWorkCol = OM.DevInfo.iTRAY_PcktCntX - iWorkCol - 1;

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;

                    MoveMotrIdx(IDX_X, IDX_XWorkStt   , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX);
                    MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolIdxWorkStt, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);

                    Step.iCycle++;
                    return false;

                
                case 12:
                    if (!MT_GetStopPos(IDX_X, IDX_XWorkStt , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX)) return false;
                    if (!MT_GetStopPos(mi.TOOL_YTool, pv.TOOL_YToolIdxWorkStt, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY)) return false;
                    Step.iCycle++;
                    return false;

                //아래서씀. Pick Miss 발생
                case 13:
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrIdxPick);
                  
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrIdxPick)) return false;
                    IO_SetY(yi.TOOL_PckrVac, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPickDelay)) return false;
                    if (!IO_GetX(xi.TOOL_PckrVac))
                    {
                        DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , cs.Empty);      
                        SPC.LOT.Data.Empty         += 1 ;
                    }
                    else if(IO_GetX(xi.TOOL_PckrVac) && OM.CmnOptn.iEmptyCheckPrcs == 0) //엠티포켓 체크 하고 
                    {
                        Step.iCycle = 50;
                        return false;
                    }
                    else if (IO_GetX(xi.TOOL_PckrVac) && OM.CmnOptn.iEmptyCheckPrcs == 1)
                    {
                        DM.ARAY[ri.PCKR].SetStat(DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow));
                        DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.Empty);
                        
                        
                        //if(DM.ARAY[ri.PCKR].GetStat(0, 0) == cs.NG0){SPC.LOT.Data.Empty         += 1 ;}
                    }

                    
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;

                    Step.iCycle = 0;
                    return true;

                //Up Used
                case 50:
                    IO_SetY(yi.TOOL_PckrVac, false);
                    Step.iCycle++;
                    return false;

                case 51:
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 52:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;
                    DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.Good);//After Vision Change
                    ER_SetErr(ei.PRT_Detect, "Vaccum Sensor detected during empty check.");
                    return true;
            }
        }

        int iGoodPickCnt;
        int iGoodPickMiss;
        public bool CycleGoodPick()
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

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;
            FindChip(ri.TRYG, out c, out r, cs.Good);

            double dOfsX = (DM.ARAY[ri.TRYG].GetMaxCol() - c - 1) * OM.DevInfo.dTRAY_PcktPitchX;
            double dOfsY = r * OM.DevInfo.dTRAY_PcktPitchY;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    iGoodPickCnt = 0;
                    iGoodPickMiss = 0;
                    if (IO_GetX(xi.TOOL_PckrVac))
                    {
                        ER_SetErr(ei.PRT_VacErr, "Picker Vacuum Sensor On Error");
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                //아래서 씀.
                case 11:
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;
                    MoveMotr(mi.TOOL_XRjct, pv.TOOL_XRjctWrkStt    , dOfsX);
                    MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolGdTWorkStt, dOfsY);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.TOOL_XRjct, pv.TOOL_XRjctWrkStt    , dOfsX)) return false;
                    if (!MT_GetStopPos(mi.TOOL_YTool, pv.TOOL_YToolGdTWorkStt, dOfsY)) return false;
                    Step.iCycle++;
                    return false;

                case 14:
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrGdTWork);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrGdTWork)) return false;
                    IO_SetY(yi.TOOL_PckrVac, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPickDelay)) return false;
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;
                    if (!IO_GetX(xi.TOOL_PckrVac))
                    {
                        iGoodPickCnt++;
                        if (iGoodPickCnt < OM.CmnOptn.iPickRtryCnt) //이거 나중에 옵션으로 바꿔야할수도 있음. 사양서에 Pick 3번으로 명시돼있음
                        {
                            
                            Step.iCycle = 13;
                            return false;
                        }
                        else
                        {
                            iGoodPickCnt = 0;
                            iGoodPickMiss += 1;
                            DM.ARAY[ri.TRYG].SetStat(c, r, cs.Empty);

                            if (iGoodPickMiss <= OM.CmnOptn.iGoodPickMissCnt)
                            {
                                Step.iCycle = 11;
                                return false;
                            }
                            else
                            {
                                ER_SetErr(ei.PCK_PickMiss, "Reject Picker Pick Miss Error");
                                return true;
                            }
                            
                        }
                        
                    }
                    Step.iCycle++;
                    return false;

                case 18:
                    DM.ARAY[ri.PCKR].SetStat(DM.ARAY[ri.TRYG].GetStat(c, r));
                    DM.ARAY[ri.TRYG].SetStat(c, r, cs.Empty);
                    
                    Step.iCycle = 0;
                    return true;

            }
        }

        cs RsltStat ;
        int iGoodPlaceVisnNgCnt = 0 ;
        public bool CycleGoodPlace()
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
                    iVisnIndx = 0;
                    iVisnRtry = 0;
                    if(SEQ.IDXR.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying)){
                        IDX = ri.IDXR ;
                        MoveMotrIdx = SEQ.IDXR.MoveMotr ;
                        IDX_X = mi.IDXR_XRear ;
                        IDX_XVsnStt1 = pv.IDXR_XRearVsnStt1; 
                        IDX_XVsnStt2 = pv.IDXR_XRearVsnStt2; 
                        IDX_XVsnStt3 = pv.IDXR_XRearVsnStt3; 
                        IDX_XVsnStt4 = pv.IDXR_XRearVsnStt4; 
                        IDX_XWorkStt = pv.IDXR_XRearWorkStt ; 
                    }
                    else if (SEQ.IDXF.FindChip(out iWorkCol , out iWorkRow , out iWorkStat , out bFlying)){
                        IDX = ri.IDXF ;
                        MoveMotrIdx = SEQ.IDXF.MoveMotr ;
                        IDX_X = mi.IDXF_XFrnt ;
                        IDX_XVsnStt1 = pv.IDXF_XFrntVsnStt1; 
                        IDX_XVsnStt2 = pv.IDXF_XFrntVsnStt2; 
                        IDX_XVsnStt3 = pv.IDXF_XFrntVsnStt3; 
                        IDX_XVsnStt4 = pv.IDXF_XFrntVsnStt4; 
                        IDX_XWorkStt = pv.IDXF_XFrntWorkStt ;
                    }
                    iInvWorkCol = OM.DevInfo.iTRAY_PcktCntX - iWorkCol - 1;
                    SEQ.Visn.SendManMode(true);
                    Log.Trace("VISION", "SendManMode true" );

                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);

                    Step.iCycle++;
                    return false;

                case 11:
                    if(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.ManMode)) return false ;
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;

                    MoveMotrIdx(IDX_X, IDX_XWorkStt   , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX);
                    MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolIdxWorkStt, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);

                    Step.iCycle++;
                    return false;

                
                case 12:
                    if (!MT_GetStopPos(IDX_X, IDX_XWorkStt , iInvWorkCol * OM.DevInfo.dTRAY_PcktPitchX)) return false;
                    if (!MT_GetStopPos(mi.TOOL_YTool, pv.TOOL_YToolIdxWorkStt, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY)) return false;
                    if (!OM.CmnOptn.bIdleRun && !IO_GetX(xi.RAIL_TrayDtct2) && iWorkCol != 0)//sensor zone is over when col 0
                    {
                        ER_SetErr(ei.PRT_Missed , IO_GetXName(xi.RAIL_TrayDtct2) +" is Not Detected!");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 13:
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrIdxPlace);
                    
                    Step.iCycle++;
                    return false;

                case 14:
                    if(MT_GetCmdPos(mi.TOOL_ZPckr) > PM_GetValue(mi.TOOL_ZPckr , pv.TOOL_ZPckrIdxPick)-1.0) { //1미리 전에 배큠오프.
                        IO_SetY(yi.TOOL_PckrVac, false);
                    }

                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrIdxPlace)) return false;
                    IO_SetY(yi.TOOL_PckrVac, false);
                    //IO_SetY(yi.TOOL_Pckr, false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;                    
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove);
                    
                    
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove)) return false;
                    //내려 놨으니 하나 놓고.
                    DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , cs.Vision) ;
                    RsltStat = cs.Good;
                    //일단 비전쪽에서 티칭 안한경우는 굿으로 빼니깐 초기값은 1
                    DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sUnitID = "1" ;
                    DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC1   = "1" ;
                    DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC2   = "1" ;



                    Step.iCycle++;
                    return false ;

                    //using later step
                case 17:
                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt1, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt2, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt3, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_YTool, pv.TOOL_YToolVsnStt4, iWorkRow * OM.DevInfo.dTRAY_PcktPitchY);

                    if (iVisnIndx == 0) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork1);
                    if (iVisnIndx == 1) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork2);
                    if (iVisnIndx == 2) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork3);
                    if (iVisnIndx == 3) MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWork4);
                    
                    if (iVisnIndx == 0) MoveMotrIdx(IDX_X , IDX_XVsnStt1 , iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX );
                    if (iVisnIndx == 1) MoveMotrIdx(IDX_X , IDX_XVsnStt2 , iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX );
                    if (iVisnIndx == 2) MoveMotrIdx(IDX_X , IDX_XVsnStt3 , iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX );
                    if (iVisnIndx == 3) MoveMotrIdx(IDX_X , IDX_XVsnStt4 , iInvWorkCol *OM.DevInfo.dTRAY_PcktPitchX );






                    SEQ.Visn.SendIndex(iVisnIndx);
                    Log.Trace("VISION", "SendIndex" +  iVisnIndx.ToString() );
                    
                    if(!SEQ.Visn.DeleteRslt()){//검사전에 결과값 지워준다.     2018 0221 혹시 문제 소지가 있을듯 해서 추가.                        
                        ER_SetErr(ei.VSN_ComErr , "Failed to delete VisionResultFile!");
                        return true ;
                    }
                    
                    Step.iCycle++;
                    return false;

                
                case 18:
                    if (!MT_GetStop(mi.TOOL_YTool)) return false;
                    if (!MT_GetStop(mi.TOOL_ZVisn))return false;
                    if (!MT_GetStop(IDX_X))return false;
                    if(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Command))return false;      
              
                    
                    Step.iCycle++;
                    return false;

                case 19:
                    
                    SEQ.Visn.SendManInsp();
                    Log.Trace("VISION", "SendManInsp" );
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.Insp)) return false;
                    Log.Trace("VISION", "SendManInsp END" );
                    if(!SEQ.Visn.FindRsltFile())return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 21:
                    if(!m_tmDelay.OnDelay(200)) return false;
                    VisnCom.TRslt Rslt = new VisnCom.TRslt();

                    SEQ.Visn.LoadManRslt(ref Rslt);

                    if(Rslt.Empty         == 0 &&
                       Rslt.MixDevice     == 0 &&
                       Rslt.UnitID        == null &&
                       Rslt.UnitDMC1      == null &&
                       Rslt.UnitDMC2      == null &&
                       Rslt.GlobtopLeft   == 0 &&
                       Rslt.GlobtopTop    == 0 &&
                       Rslt.GlobtopRight  == 0 &&
                       Rslt.GlobtopBottom == 0 &&
                       Rslt.MatchingError == 0 &&
                       Rslt.UserDefine    == 0 ) return false; //그냥 기다리는 건가???





                    //DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , cs.Good) ;
                    
                    //멀티샷검사시에 검사영역이 아니면 1이 날라와서 
                    if(Rslt.UnitID       !="1" ) DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sUnitID = Rslt.UnitID   ;
                    if(Rslt.UnitDMC1     !="1" ) DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC1   = Rslt.UnitDMC1 ;
                    if(Rslt.UnitDMC2     !="1" ) DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC2   = Rslt.UnitDMC2 ;

                    //바코드들은 "0"이면 있는데 못읽음. "1"바코드가 검사에 없음. "바코드문자" 읽기 성공.
                         if(Rslt.Empty        ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG0 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG0);} 
                    //else if(Rslt.MixDevice    ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG1 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG1);}
                    else if(Rslt.UnitID       =="0" ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG2 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG2);}
                    else if(Rslt.UnitDMC1     =="0" ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG3 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG3);}
                    else if(Rslt.UnitDMC2     =="0" ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG4 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG4);}
                    else if(Rslt.GlobtopLeft  ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG5 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG5);}
                    else if(Rslt.GlobtopTop   ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG6 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG6);}
                    else if(Rslt.GlobtopRight ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG7 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG7);}
                    else if(Rslt.GlobtopBottom==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG8 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG8);} 
                    else if(Rslt.MatchingError==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG9 );}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG8);} 
                    else if(Rslt.UserDefine   ==0   ){DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, cs.NG10);}//DM.ARAY[ri.INSP].SetStat(0,iWorkRow,cs.NG8);} 


                    //2018 02 21 자꾸 1 1 1 나오는 것 때문에
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.Empty         = " + Rslt.Empty        .ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.UnitID        = " + Rslt.UnitID                  );
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.UnitDMC1      = " + Rslt.UnitDMC1                );
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.UnitDMC2      = " + Rslt.UnitDMC2                );
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.GlobtopLeft   = " + Rslt.GlobtopLeft  .ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.GlobtopTop    = " + Rslt.GlobtopTop   .ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.GlobtopRight  = " + Rslt.GlobtopRight .ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.GlobtopBottom = " + Rslt.GlobtopBottom.ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.MatchingError = " + Rslt.MatchingError.ToString());
                    Log.Trace("VisionResult "+iWorkCol.ToString() +" "+iWorkRow.ToString(), "Rslt.UserDefine    = " + Rslt.UserDefine   .ToString());






                    
                    //멀티샷 검사일경우.
                    iVisnIndx++;
                    if (iVisnIndx < OM.DevOptn.iTotalInspCnt) //combobox index + 1
                    {                        
                        Step.iCycle = 17;
                        return false;
                    }//여기까지가 멀티샷 검사 끝내고 오라클용 스트링 세팅 끝난상황.

                    //비전 페일시 리트라이.
                    //if (iVisnRtry < OM.DevOptn.iNgInspCnt && DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow) != cs.Good)
                    //{
                    //    iVisnRtry++;
                    //    iVisnIndx = 0;
                    //    Step.iCycle = 12;
                    //    return false;
                    //}

                    if (!OM.CmnOptn.bIdleRun && !OM.CmnOptn.bOracleNotUse)
                    {
                        int iPocketID = 0;
                        cs InspStat = cs.Good ;                             
                        string sErrMsg = "";
                        if (!CheckOracleNG(IDX, iWorkCol, iWorkRow , ref sErrMsg))
                        {
                            ER_SetErr(ei.ETC_Oracle , sErrMsg);
                            //이함수 안에서 믹스빈 에러(SetErr) 난 상태 안에서 마스키 하고 에러 띄우고 해서.
                        }

                        iPocketID = GetPocketID (iWorkCol, iWorkRow) ;
                        InspStat = DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow) ;

                        if (InspStat == cs.Vision)//재검 했는데 굿으로 되었음.
                        {
                            string sQuery ="";
                            if(!SEQ.Oracle.GetUnitInspectionQuery(iPocketID.ToString() , 
                                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sUnitID , 
                                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC1   , 
                                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC2   , 
                                                                  cs.Good ,OM.CmnOptn.sMachinID , ref sQuery)){
                                SM.ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                                DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , iWorkStat);
                                return true ;
                            }
                            else
                            {
                                DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sQuery = sQuery ;
                            }         
                        }
                        else//재검 했는데 페일임. 여기서 페일판정 나면 어차피 NG픽에서 작업을 하기 때문에 여기선 안한다. 여기서 하면 오라클에 데이터 2개 들어감.
                        {
                            //if(!SEQ.Oracle.InsertUnitInspectionQuery(iPocketID.ToString() , 
                            //    DM.ARAY[IDX].Chip[iWorkCol , iWorkRow].sUnitID     , 
                            //    DM.ARAY[IDX].Chip[iWorkCol , iWorkRow].sDMC1       , 
                            //    DM.ARAY[IDX].Chip[iWorkCol , iWorkRow].sDMC2       , 
                            //    InspStat ,OM.CmnOptn.sMachinID)){//Get Query And Insert 
                                
                            //    DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , iWorkStat) ; //쿼리 에러났으니깐 원래 맨처음 상태로 되돌리고 에러 띄우면 다시 검사 하기에 ...
                            //    SM.ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                            //    Step.iCycle=0;
                            //    return true;
                            //}
                        }                        
                    }
                                    
                    //if (!OM.CmnOptn.bIdleRun)
                    //{
                    //    if (!OM.CmnOptn.bOracleNotUse)
                    //    {
                    //        int iPocketID = 0;
                    //        cs InspStat = cs.Good ;                             

                    //        if (!CheckOracleNG(IDX, iWorkCol, iWorkRow,true))
                    //        {
                    //            //이함수 안에서 오라클 에러(SetErr) 난 상태
                    //        }

                    //        iPocketID = GetPocketID (iWorkCol, iWorkRow) ;
                    //        InspStat = DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow) ; 

                    //        string sQuery ="";
                    //        if(DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow) == cs.Good){//굿인 경우만 쿼리넣고 페일인 경우는 NG 픽에서 처리.
                    //            if(!SEQ.Oracle.GetUnitInspectionQuery(iPocketID.ToString() , 
                    //                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sUnitID , 
                    //                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC1   , 
                    //                                                  DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sDMC2 , 
                    //                                                  cs.Good ,OM.CmnOptn.sMachinID , ref sQuery)){
                    //                SM.ER_SetErr(ei.ETC_Oracle , SEQ.Oracle.GetLastMsg());
                    //                DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , cs.NG1);
                    //                return true ;
                    //            }
                    //            else
                    //            {
                    //                DM.ARAY[IDX].Chip[iWorkCol, iWorkRow].sQuery = sQuery ;
                    //            }                             
                    //        }
                    //    }
                    //}


                    if (DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow)==cs.Vision || OM.DevOptn.iVisnNGCntErr==0) {
                        iGoodPlaceVisnNgCnt = 0; 
                    }
                    else {
                        iGoodPlaceVisnNgCnt++;
                        if (iGoodPlaceVisnNgCnt >= OM.DevOptn.iVisnNGCntErr)
                        {
                            ER_SetErr(ei.VSN_InspNG , "Place Vision Serial Fail Count Error");
                            iGoodPlaceVisnNgCnt = 0;
                        }
                    }
                                    
                    
                    MoveMotr(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait);
                    SEQ.Visn.SendManMode(false);    
                    Step.iCycle++;
                    return false;

                case 22:
                    if(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.ManMode)) return false ;
                    if (!MT_GetStopPos(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait)) return false;
                    //DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , cs.Vision);    //일단 굿으로 하고 시퀜스 돌리면서 만들자.         
                    //DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow, DM.ARAY[ri.INSP].GetStat(0, iWorkRow));
                         
                     Log.Trace("VISION", "SendManMode false" );
                    DM.ARAY[ri.PCKR].SetStat(0       , 0        , cs.None);    //일단 여기까지만 하고. 
                    if(DM.ARAY[IDX].GetStat(iWorkCol, iWorkRow)==cs.Vision)DM.ARAY[IDX].SetStat(iWorkCol, iWorkRow , cs.Good);
                    DM.ARAY[IDX].ClearWorkChip();
                    Step.iCycle = 0;
                    return true;
            }
        }


        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.IDXR_ClampClOp){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }

            else if (_eActr == ci.IDXR_ClampUpDn)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }

            else if (_eActr == ci.IDXF_ClampClOp)
            {
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.IDXF_ClampUpDn)
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

            //TOOL_ZVisn


            if (_eMotr == mi.TOOL_ZPckr)
            {
                //if (!MT_GetStopInpos(mi.TOOL_YGent))
                //{
                //    sMsg = MT_GetName(mi.TOOL_YGent) + " is moving.";
                //    bRet = false;
                //}
                //
                //if (!MT_GetStopInpos(mi.TOOL_XLeft))
                //{
                //    sMsg = MT_GetName(mi.TOOL_XLeft) + " is moving.";
                //    bRet = false;
                //}
            
            }
            
            else if (_eMotr == mi.TOOL_YTool)
            {
                if (!MT_GetStopInpos(mi.TOOL_ZPckr))
                {
                    sMsg = MT_GetName(mi.TOOL_ZPckr) + " is moving.";
                    bRet = false;
                }
                
                if (MT_GetCmdPos(mi.TOOL_ZPckr) > PM_GetValue(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove))
                {
                    sMsg = MT_GetName(mi.TOOL_ZPckr) + " is lower than Picker Z Move Position.";
                    bRet = false;
                }
            }
            
            else if (_eMotr == mi.TOOL_XRjct)
            {
                if (!MT_GetStopInpos(mi.TOOL_ZPckr))
                {
                    sMsg = MT_GetName(mi.TOOL_ZPckr) + "is moving.";
                    bRet = false;
                }

                if (MT_GetCmdPos(mi.TOOL_ZPckr) > PM_GetValue(mi.TOOL_ZPckr, pv.TOOL_ZPckrMove))
                {
                    sMsg = MT_GetName(mi.TOOL_ZPckr) + " is lower than Picker Z Move Position.";
                    bRet = false;
                }
                
            }

            else if (_eMotr == mi.TOOL_ZVisn)
            {
                //if (!MT_GetStopInpos(mi.SSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.SSTG_YGrpr) + "is moving.";
                //    bRet = false;
                //}
                //
                //if (dDstPos > PM_GetValue(mi.SSTG_YGrpr, pv.SSTG_YGrprPickWait))
                //{
                //    sMsg = MT_GetName(mi.SSTG_YGrpr) + "Crash Position with " + MT_GetName(_eMotr);
                //    bRet = false;
                //}
                //
                //if (IO_GetX(xi.SLDR_SstOutCheck))
                //{
                //    sMsg = IO_GetXName(xi.SLDR_SstOutCheck) + "is Checking.";
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
                if (Step.eSeq == 0) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
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
            if (!MT_GetStop(mi.TOOL_XRjct)) return false;
            if (!MT_GetStop(mi.TOOL_ZVisn)) return false;
            if (!MT_GetStop(mi.TOOL_ZPckr)) return false;
            if (!MT_GetStop(mi.TOOL_YTool)) return false;

            //if (!MT_GetStop(mi.TOOL_XLeft)) return false;
            //if (!MT_GetStop(mi.TOOL_YVisn)) return false;
            //if (!MT_GetStop(mi.TOOL_YGent)) return false;
            return true;
        }
    }; 

   
    
}
