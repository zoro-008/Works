using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON;
using SML2;



namespace Machine
{
    public class SubstrateStage : Part
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
            Move       , //2개씩 있는자제 피치 이동.
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

        public UVWStage UVW  ;

        //Ver 1.0.5.0
        //바코드 리딩한 횟수
        //iBarcReadCnt랑 Row 갯수랑 비교해서 일치하지 않으면 계속 바코드 리딩 돌린다.
        public int iBarcReadCnt;

        //public const int iSLDRGetOfs = 1;
        public const double dSLDRGetOfs = 0.2;
        public const int iSGrprOutOfs = 1;
        public const int iSSTGGrprBwdOfs = -2;

        public SubstrateStage()
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

            //eStgAng : 1 , dX1Ang :135 , dX2Ang : 225 , dYAbg : 45 , dRotRad : 212.13203435596425732
            TUVWPara UvwPara ;// = new TUVWPara();
            UvwPara.eStgAng = UVW_ANG.AngCW90 ;
            UvwPara.dX1Ang  = 135 ;
            UvwPara.dX2Ang  = 225 ;
            UvwPara.dYAng   = 45  ;
            UvwPara.dRotRad = 212.13203435596425732 ;
            UvwPara.iX1     = (int)mi.SSTG_YRght ;
            UvwPara.iX2     = (int)mi.SSTG_YLeft ;
            UvwPara.iY      = (int)mi.SSTG_XFrnt ;
            UVW = new UVWStage(UvwPara);

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
        bool ActivatedError = false ;
        override public void Update()
        {
            //20180509 1.0.3.1 엔코더 값 밀리는 현상으로 테스트 추가.
            if(!ActivatedError) {
                if(MT_GetStop(mi.SLDR_ZElev))
                {
                    double dEncPos = MT_GetEncPos(mi.SLDR_ZElev) ;
                    double dCmdPos = MT_GetCmdPos(mi.SLDR_ZElev) ;
                    double dTrgPos = MT_GetTrgPos(mi.SLDR_ZElev) ;
                
                    if(Math.Abs( dEncPos - dCmdPos) > 1.0) 
                    {
                        string sTemp = string.Format("EncPos:{0:F6} CmdPos:{1:F6}" , dEncPos , dCmdPos);
                        Log.Trace(MT_GetName(mi.SLDR_ZElev) , sTemp);
                        ER_SetErr(ei.MTR_Alarm , sTemp);
                        ActivatedError = true ;
                    }
                }
            }
            else {
                double dEncPos = MT_GetEncPos(mi.SLDR_ZElev) ;
                double dCmdPos = MT_GetCmdPos(mi.SLDR_ZElev) ;
                double dTrgPos = MT_GetTrgPos(mi.SLDR_ZElev) ;

                if(Math.Abs( dEncPos - dCmdPos) < 1.0) 
                {
                    ActivatedError = false ;
                }
                    

            }
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
                    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    Step.iToStart++;
                    return false;

                case 11: 
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd)) return false ;

                    if(DM.ARAY[ri.SSTG].CheckAllStat(cs.None)) {
                        Step.iToStart = 0 ;
                        return true ;
                    }


                    IO_SetY(yi.SSTG_SubsVac, true);
                    Step.iToStart++;
                    return false;
                                 
                case 12:
                    MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailDown);
                    Step.iToStart++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailDown))return false ;
                    
                    //가끔 WorkEnd로 마스킹 하여 스킵 하고 싶은경우 에러가뜨는 경우가 있음. 
                    //if(!IO_GetX(xi.SSTG_BoatVac)) 
                    //{
                    //    ER_SetErr(ei.PRT_VaccSensor, "스테이지 배큠 센서가 감지되지 않습니다.");
                    //    return true;
                    //}
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
                    IO_SetY(yi.SSTG_IonizerOn , false);
                    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    Step.iToStop++;
                    return false;

                case 11: 
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd)) return false ;
                    //if(DM.ARAY[ri.SSTG].CheckAllStat(cs.None)) {//서브스트레이트에 자제 없는 경우.
                        MoveMotr(mi.SSTG_YGrpr, pv.SSTG_YGrprWait);
                    //}
                    //else {//자제 있는경우.
                    //    MoveMotr(mi.SSTG_YGrpr, pv.SSTG_YGrprPickWait);
                    //   
                    //}
                    Step.iToStop++;
                    return false;

                case 12:
                    //혹시라도 처박으면..
                    if (IO_GetX(xi.SSTG_GrprOverload))
                    {
                        MT_EmgStop(mi.SSTG_YGrpr);
                        ER_SetErr(ei.PRT_OverLoad, "Substrate");
                        return true;
                    }
                    if(!MT_GetStop(mi.SSTG_YGrpr)) return false ;
                    //오버로드 에러가 없거나 있어도 Substrate Stage께 아닌경우에만
                    bool bOverLoad  = ER_GetErrOn(ei.PRT_OverLoad) && ER_GetErrSubMsg(ei.PRT_OverLoad)!="Substrate";
                    bool bMissed    = ER_GetErrOn(ei.PRT_Missed  ) && ER_GetErrSubMsg(ei.PRT_Missed  )!="Substrate";
                    bool bBarcodeNG = ER_GetErrOn(ei.PRT_Barcode ) && ER_GetErrSubMsg(ei.PRT_Barcode )!="Substrate";
                    
                    //오성철과장 요청.
                    //if(!bOverLoad && !bMissed && !bBarcodeNG){
                    //    MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevWait);
                    //}

                    bool bVacErr = ER_GetErrOn(ei.PRT_VaccSensor) && ER_GetErrSubMsg(ei.PRT_VaccSensor)!="스테이지 배큠 센서가 감지되지 않습니다.";

                    if(bVacErr){
                        IO_SetY(yi.SSTG_SubsVac, false);
                        MoveMotr(mi.SSTG_ZRail, pv.SSTG_ZRailWait);
                    }
                    Step.iToStop++;
                    return false;
                
                case 13: 
                    if(!MT_GetStop(mi.SSTG_ZRail)) return false ;
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

                //if( DM.ARAY[ri.SLDR].CheckAllStat(cs.None) && IO_GetX(xi.SLDR_MgzCheck)) {ER_SetErr(ei.PKG_Unknwn , "Substrate Loader Unknwn PKG Error."   ); return false;}
                //if(!DM.ARAY[ri.SLDR].CheckAllStat(cs.None) &&!IO_GetX(xi.SLDR_MgzCheck)) {ER_SetErr(ei.PKG_Dispr  , "Substrate Loader Disappear PKG Error."); return false;}

                int r=0;
                //r = DM.ARAY[ri.SSTG].FindLastRow(cs.WorkEnd);
                //if(r<0){
                //    r=0;
                //}
                //else{
                //    r++;
                //}
                r = DM.ARAY[ri.SSTG].FindLastRow(cs.WorkEnd);
                if(r < 0){
                    r=OM.DevInfo.iSBOT_PcktCnt-1;
                }
                else{
                    r--;
                }

                bool isCycleGet  = DM.ARAY[ri.SSTG].CheckAllStat(cs.None   ) && ( DM.ARAY[ri.SLDT].IsExist(cs.Unknown) || DM.ARAY[ri.SLDB].IsExist(cs.Unknown));
                bool isCycleMove = r != OM.EqpStat.iSSTGStep && OM.EqpStat.iSSTGStep != 0;
                bool isCycleOut  = DM.ARAY[ri.SSTG].CheckAllStat(cs.WorkEnd  ) && ( DM.ARAY[ri.SLDT].IsExist(cs.Mask   ) ||  DM.ARAY[ri.SLDB].IsExist(cs.Mask   ) );
                bool isCycleEnd  = DM.ARAY[ri.SSTG].CheckAllStat(cs.None   ) && (!DM.ARAY[ri.SLDT].IsExist(cs.Unknown) && !DM.ARAY[ri.SLDB].IsExist(cs.Unknown) );

                //에러 띄우고 그냥 랏엔드는 거의 안하다 시피 한다.
                //어차피 랏데이터도 자제별로 남겨서 의미 없음.
                bool bNoWorkChip = DM.ARAY[ri.WSTG].GetCntStat(cs.EjectAlign) == 0 &&
                                   DM.ARAY[ri.WSTG].GetCntStat(cs.Eject     ) == 0 &&
                                   DM.ARAY[ri.WSTG].GetCntStat(cs.Align     ) == 0 &&
                                   DM.ARAY[ri.WSTG].GetCntStat(cs.Pick      ) == 0 ;
                if(isCycleEnd) {
                    if(SEQ.TOOL.m_bContiWork) { //계속작업모드면
                        ER_SetErr(ei.ETC_NeedPkg,"서브스트레이트 로더에 웨이퍼를 공급하고 마스킹해주세요.");
                        return false ;
                    }
                    else { //작업엔드 모드면
                        if(!DM.ARAY[ri.WSTG].CheckAllStat(cs.None)) DM.ARAY[ri.WSTG].SetStat(cs.Empty); //서브스트레이트 스테이지에 있는 자제 다 워크엔드 시키고.
                        DM.ARAY[ri.WLDB].ChangeStat(cs.Unknown , cs.WorkEnd);  //로더에 언노운 있던것을 다 WorkEnd시킨다.
                        DM.ARAY[ri.WLDT].ChangeStat(cs.Unknown , cs.WorkEnd);
                    }   
                    
                }



                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleGet   ) { Step.eSeq = sc.Get ;  }
                else if (isCycleMove  ) { Step.eSeq = sc.Move;  }
                else if (isCycleOut   ) { Step.eSeq = sc.Out ;  }
                else if (isCycleEnd   ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

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
                default         :                        Log.Trace(m_sPartName, "default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle  ):                                                                                                                         return false;
                case (sc.Get   ): if (CycleGet     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Move  ): if (CycleMove    ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.Out   ): if (CycleOut     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        //위에 부터 작업.
        public bool FindChip(out int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            c = 0 ; r = 0 ;
            _iId = 0;
            ////디폴트값으로 들어오면 현재 작업중인 칩을 리턴 한다.
            //if(_iChip == cs.RetFail) {
            //    DM.ARAY[_iId].FindLastRowCol(cs.WorkEnd , ref c , ref r);
            //    if(r<0 || c<0){
            //        r=0;
            //        c=0;
            //    }
            //    else {
            //        r++;
            //        c = 0;
            //    }
            //    return true ;
            //}
            //
            ////나머지는 해당 칩을 찾아서 리턴.
            //DM.ARAY[_iId].FindLastRowCol(_iChip , ref c , ref r);
            //return (c >= 0 && r >= 0) ? true : false;
            if(_iChip != cs.RetFail) { 
                if(-1 != DM.ARAY[ri.SLDT].FindFrstRow(_iChip)){
                    _iId = ri.SLDT ;
                    r  = DM.ARAY[ri.SLDT].FindFrstRow(_iChip) ;
                    return true ;
                }
                else if(-1 != DM.ARAY[ri.SLDB].FindFrstRow(_iChip)){
                    _iId = ri.SLDB ;
                    r  = DM.ARAY[ri.SLDB].FindFrstRow(_iChip) ;
                    return true ;
                }
            }
            if(_iChip == cs.RetFail) {
                DM.ARAY[ri.SSTG].FindLastRowCol(cs.WorkEnd , ref c , ref r);
                _iId = ri.SSTG ;
                if(r<0 || c<0){
                    r=OM.DevInfo.iSBOT_PcktCnt - 1;
                    c=0;
                }
                else {
                    r--;
                    c = 0;
                }
                return true ;
            }
                
                //나머지는 해당 칩을 찾아서 리턴.
            DM.ARAY[ri.SSTG].FindLastRowCol(_iChip , ref c , ref r);
            _iId = ri.SSTG ;
            return (c >= 0 && r >= 0) ? true : false;
            
            
            //return false ;
        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 8000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Log.Trace(m_sPartName, sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if(Stat.bReqStop) {
                //return true ;
            }


            int iAray ;
            int iR ;
            int iC ;

            FindChip(out iAray , out iC , out iR);
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;
            
                case 10:
                    CL_Move(ci.SSTG_GrprClOp , fb.Bwd);
                    Step.iHome++;
                    return false ;
            
                case 11:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd)) return false ;
                    
                    //레일 컨버전은 메뉴얼 동작 컨버젼으로 홈잡는다===
                    MT_SetHomeDone(mi.SSTG_XRail , true);
                    MT_SetPos(mi.SSTG_XRail,pv.SSTG_XRailWork);
                    //=======================
                    MT_GoHome(mi.SSTG_YGrpr);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetHomeDone(mi.SSTG_YGrpr)) return false;
                    MT_GoHome(mi.SSTG_ZRail);
                    MT_GoHome(mi.SLDR_ZElev);
                    Step.iHome++;
                    return false ;

                case 13:
                    if(!MT_GetHomeDone(mi.SSTG_ZRail)) return false ;
                    if(!MT_GetHomeDone(mi.SLDR_ZElev)) return false;
                    //MT_GoHome(mi.SSTG_XFrnt);
                    //MT_GoHome(mi.SSTG_YLeft);
                    //MT_GoHome(mi.SSTG_YRght);
                    UVW.GoHome();

                    Step.iHome++;
                    return false;

                case 14:
                    if(!UVW.GetHomeDone()) return false ;
                    //if(!MT_GetHomeDone(mi.SSTG_XFrnt)) return false ;
                    //if(!MT_GetHomeDone(mi.SSTG_YLeft)) return false;
                    //if(!MT_GetHomeDone(mi.SSTG_YRght)) return false;

                    MT_GoAbsMan(mi.SSTG_YGrpr,pv.SSTG_YGrprWait);        
                    MT_GoAbsMan(mi.SSTG_ZRail,pv.SSTG_ZRailWait);        
                    MT_GoAbsMan(mi.SSTG_XFrnt,pv.SSTG_XFrntWait);        
                    MT_GoAbsMan(mi.SSTG_YLeft,pv.SSTG_YLeftWait);        
                    MT_GoAbsMan(mi.SSTG_YRght,pv.SSTG_YRghtWait);
                    MT_GoAbsMan(mi.SLDR_ZElev,pv.SLDR_ZElevWait);    

                    Step.iHome++;
                    return false;

                case 15:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr,pv.SSTG_YGrprWait))return false ;    
                    if(!MT_GetStopPos(mi.SSTG_ZRail,pv.SSTG_ZRailWait))return false ;    
                    if(!MT_GetStopPos(mi.SSTG_XFrnt,pv.SSTG_XFrntWait))return false ;    
                    if(!MT_GetStopPos(mi.SSTG_YLeft,pv.SSTG_YLeftWait))return false ;    
                    if(!MT_GetStopPos(mi.SSTG_YRght,pv.SSTG_YRghtWait))return false ;
                    if(!MT_GetStopPos(mi.SLDR_ZElev,pv.SLDR_ZElevWait))return false ;
                    Step.iHome = 0;
                    return true ;
            }
        }

        
        public bool CycleGet()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                IO_SetY(yi.SSTG_IonizerOn , false);
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
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
                //return true ;
            }

            //int r = 0, c = 0;

            if(CL_Complete(ci.SSTG_GrprClOp , fb.Fwd))
            {
                if(!MT_GetStop(mi.SSTG_YGrpr))
                {
                    //이것은 어차피 놓치는 거라 그냥 움직이고 나서 확인 하는 것으로 한다.
                }                
            }
            
            int iAray;
            int iR ;
            int iC ;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //일단 스텝 초기화.
                    //OM.EqpStat.iSSTGStep = 0 ;
                    iBarcReadCnt = 0;
                    OM.EqpStat.iSSTGStep = OM.DevInfo.iSBOT_PcktCnt - 1 ;

                    MoveMotr(mi.SSTG_XFrnt , pv.SSTG_XFrntWait);
                    MoveMotr(mi.SSTG_YLeft , pv.SSTG_YLeftWait);
                    MoveMotr(mi.SSTG_YRght , pv.SSTG_YRghtWait);
                    //MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprWait);
                    MoveMotr(mi.SSTG_XRail , pv.SSTG_XRailWork); //보트 클램핑 할때 레일 X축으로 하여 넣어줌. 진섭
                    MoveCyl (ci.SSTG_BoatClampClOp , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.SSTG_XFrnt , pv.SSTG_XFrntWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_YLeft , pv.SSTG_YLeftWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_YRght , pv.SSTG_YRghtWait))return false ;
                    //if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_XRail , pv.SSTG_XRailWork))return false ;
                    if(!CL_Complete(ci.SSTG_BoatClampClOp , fb.Bwd))return false ;

                    MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailWait);
                    Step.iCycle++;
                    return false ;

                case 12: 
                    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailWait))return false ;                  
                    

                    MoveCyl (ci.SSTG_GrprClOp , fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd))return false;     
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprPickWait);               
                    
                    //이상함.......20180308 오성철 과장이 랏오픈하고 Empty로 Top을 다 드레그 했는데도 Top의 맨위에 가서 한번 작업 을 하고 
                    //                       주욱 내려와서 Btm을 작업 한다고 함. 근데 Mask는 왜 조건을 확인 하는지 모르겠음.
                    //     if (FindChip(out iAray , out iC , out iR, cs.Mask   ) && iAray == ri.SLDT) MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevTopWorkStt, iR * OM.DevInfo.dSLDR_SlotPitch);
                    //else if (FindChip(out iAray , out iC , out iR, cs.Unknown) && iAray == ri.SLDT) MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevTopWorkStt, iR * OM.DevInfo.dSLDR_SlotPitch);                 
                    //
                    //     if (FindChip(out iAray , out iC , out iR, cs.Mask   ) && iAray == ri.SLDB) MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevBtmWorkStt, iR  * OM.DevInfo.dSLDR_SlotPitch);
                    //else if (FindChip(out iAray , out iC , out iR, cs.Unknown) && iAray == ri.SLDB) MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevBtmWorkStt, iR  * OM.DevInfo.dSLDR_SlotPitch);

                         if (FindChip(out iAray , out iC , out iR, cs.Unknown) && iAray == ri.SLDT) MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevTopWorkStt, iR * OM.DevInfo.dSLDR_SlotPitch);
                    else if (FindChip(out iAray , out iC , out iR, cs.Unknown) && iAray == ri.SLDB) MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevBtmWorkStt, iR * OM.DevInfo.dSLDR_SlotPitch);                         
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr,pv.SSTG_YGrprPickWait))return false;
                    if (!MT_GetStop(mi.SLDR_ZElev)) return false;
                    if(IO_GetX(xi.SSTG_GrprPkgCheck)){
                        ER_SetErr(ei.PKG_Dispr , "자재가 감지중입니다. 로더를 확인해주세요");
                        return true ;
                    }
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprPick);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprPick)) return false;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!m_tmDelay.OnDelay(1000)) return false ;
                    if(!IO_GetX(xi.SSTG_GrprPkgCheck)) {//자제 없는경우.
                        Step.iCycle = 50 ;
                        return false ;
                    }
                    Step.iCycle++;
                    return false;

                //자제 있는경우.
                case 17: 
                    MoveCyl(ci.SSTG_GrprClOp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Fwd)) return false ;
                    //MT_GoIncRun(mi.WLDR_ZElev, -iWLDRGetOfs);

                    Step.iCycle++;
                    return false;

                case 19:
                    //if (!MT_GetStop(mi.WLDR_ZElev)) return false;
                    MT_GoIncRun(mi.SSTG_YGrpr, iSSTGGrprBwdOfs);
                    Step.iCycle++;
                    return false;

                case 20:
                    if(!MT_GetStop(mi.SSTG_YGrpr)) return false;
                    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 21:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd)) return false ;
                    //MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprPick);
                    MT_GoAbsVel(mi.SSTG_YGrpr, PM_GetValue(mi.SSTG_YGrpr, pv.SSTG_YGrprPick), 10);
                    Step.iCycle++;
                    return false ;

                case 22:
                    if(!MT_GetStop(mi.SSTG_YGrpr)) return false;
                    MoveCyl(ci.SSTG_GrprClOp , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 23:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Fwd)) return false;
                    IO_SetY(yi.SSTG_IonizerOn , true);
                    Step.iCycle = 30;
                    return false;

                //Ver. 1.0.5.0
                //20180724 바코드를 자재에 부착한다고 하여 개별로 스캔하도록 수정. 진섭
                case 30:
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprBarcode, -(OM.DevInfo.dSBOT_PcktPitch * iBarcReadCnt));
                    Step.iCycle++;
                    return false ;

                
                case 31:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprBarcode, -(OM.DevInfo.dSBOT_PcktPitch * iBarcReadCnt))) return false ;
                    
                    Step.iCycle++;
                    return false ;

                case 32:
                    
                    if(!OM.CmnOptn.bSubBarcodeSkip) {
                        //바코드 읽기.
                        SEQ.BarcordSSTG.SendScanOn();
                    } 

                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 33:
                    if(!OM.CmnOptn.bSubBarcodeSkip) {
                        if(m_tmDelay.OnDelay(3000)) {//바코드 못읽었을때 상황.
                            DM.ARAY[ri.SSTG].Chip[0, iBarcReadCnt].Data = "";
                        }
                        if(SEQ.BarcordSSTG.GetText() == "")return false ;
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 34:
                    if(!m_tmDelay.OnDelay(100))return false ;
                    if(!OM.CmnOptn.bSubBarcodeSkip) {
                        DM.ARAY[ri.SSTG].Chip[0, iBarcReadCnt].Data = ""; //여기서 초기화 한번 하고 데이터 집어넣는다.
                        DM.ARAY[ri.SSTG].Chip[0, iBarcReadCnt].Data = SEQ.BarcordSSTG.GetText();
                        Program.SendListMsg(OM.EqpStat.sSSTGBarcode);
                    }
                    else {
                        //연월일시간분초 입력
                        DM.ARAY[ri.SSTG].LotNo = DateTime.Now.ToString("yyyyMMddhhmmss"); //ID에 넣는게 맞는지 확인해야함 Jinseop
                    }

                    //여기서 카운트랑 Row 갯수 비교해서 카운트가 적으면 다시 위로 올려서 바코드 리딩
                    if (iBarcReadCnt < OM.DevInfo.iSBOT_PcktCnt-1)
                    {
                        iBarcReadCnt++;
                        Step.iCycle = 30;
                        return false;
                    }

                    iBarcReadCnt = 0;
                    Step.iCycle = 40;
                    return false;

                //WorkStart로 이동
                case 40:
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprWorkStt);
                    Step.iCycle++;
                    return false ;

                case 41:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWorkStt)) return false ;
                    IO_SetY(yi.SSTG_IonizerOn , false);
                    if(!IO_GetX(xi.SSTG_GrprPkgCheck)) {//자제 없는경우.
                        ER_SetErr(ei.PRT_Missed, "서브스트레이트가 정확히 이송되지 않았습니다. 서브스트레이트를 로더에 다시 넣어주세요.");
                        Step.iCycle = 80;
                        return false ;
                    }

                    MoveCyl(ci.SSTG_BoatClampClOp, fb.Fwd);
                    //MT_GoIncSlow(mi.SSTG_XRail, OM.DevOptn.dBoutClampOfs);

                    Step.iCycle++;
                    return false;

                case 42:
                    if(!CL_Complete(ci.SSTG_BoatClampClOp, fb.Fwd)) return false;
                    //나중에 맵을 확인 해서 하는 걸로 수정.
                    

                    FindChip(out iAray , out iC, out iR , cs.Unknown);
                    DM.ARAY[iAray].SetStat(0,iR,cs.Mask);
                    DM.ARAY[ri.SSTG].SetStat(cs.Align);

                    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 43:
                    if(!CL_Complete(ci.SSTG_GrprClOp,fb.Bwd))return false ;

                    MT_GoIncRun(mi.SSTG_ZRail, -iSGrprOutOfs);
                    Step.iCycle++;
                    return false;

                case 44:
                    if(!MT_GetStop(mi.SSTG_ZRail)) return false;
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprWait);
                    Step.iCycle++;
                    return false ;

                case 45:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWait)) return false ; 
                    IO_SetY(yi.SSTG_SubsVac, true);
                    Step.iCycle++;
                    return false;
                                 
                case 46:
                    MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailDown);
                    Step.iCycle++;
                    return false ;

                case 47:
                    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailDown))return false ;
                    if(!IO_GetX(xi.SSTG_BoatVac)) 
                    {
                        ER_SetErr(ei.PRT_VaccSensor, "스테이지 배큠 센서가 감지되지 않습니다.");
                        return true;
                    }
                    
                    Step.iCycle=0;
                    return true;


                //메거진슬롯에 서브스트레이트가 없을경우.
                case 50:
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprPickWait);
                    Step.iCycle++;
                    return false ;

                case 51:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprPickWait)) return false ;
                    
                    FindChip(out iAray , out iC, out iR , cs.Unknown);
                    DM.ARAY[iAray].SetStat(0,iR,cs.Empty);
                    Step.iCycle=0;
                    return true;


                ////바코드 못읽었을때 상황.
                //case 60:               
                //    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                //    Step.iCycle++;
                //    return false ;
                //
                //case 61:
                //    if(CL_Complete(ci.SSTG_GrprClOp,fb.Bwd))return false ;
                //    ER_SetErr(ei.PRT_Barcode , "Substrate에서 Barcode를 읽지못했습니다.");
                //    Step.iCycle=0;
                //    return true;

                ////배큠에러시 상황.
                //case 70:
                //    IO_SetY(yi.SSTG_SubsVac, false);
                //    MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailWait);
                //    Step.iCycle++;
                //    return false ;

                //case 33:
                //    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailWait))return false ;
                //    Step.iCycle=0;
                //    return true ;

            }
        }

        public bool CycleMove()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
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
                //return true ;
            }

            int r = 0, c = 0;
            r = DM.ARAY[ri.SSTG].FindLastRow(cs.WorkEnd);
            //if(r<0){
            //    r=OM.DevInfo.iSBOT_PcktCnt - 1;
            //}
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    IO_SetY(yi.SSTG_SubsVac, false);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd))return false ;
                    MoveMotr(mi.SSTG_XFrnt , pv.SSTG_XFrntWait);
                    MoveMotr(mi.SSTG_YLeft , pv.SSTG_YLeftWait);
                    MoveMotr(mi.SSTG_YRght , pv.SSTG_YRghtWait);
                    MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailWait);
                    Step.iCycle++;
                    return false ;

                case 12: 
                    if(!MT_GetStopPos(mi.SSTG_XFrnt , pv.SSTG_XFrntWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_YLeft , pv.SSTG_YLeftWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_YRght , pv.SSTG_YRghtWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailWait))return false ;
                    MoveCyl(ci.SSTG_BoatClampClOp, fb.Bwd);
                    
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.SSTG_BoatClampClOp, fb.Bwd)) return false;
                    MT_GoIncRun(mi.SSTG_ZRail, -iSGrprOutOfs);
                    Step.iCycle++;
                    return false;

                case 14:
                    //if(!GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprPickWait))return false ;
                    if(!MT_GetStop(mi.SSTG_ZRail)) return false;
                    //MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprWorkStt , (OM.DevInfo.dSBOT_PcktPitch * ((OM.DevInfo.iSBOT_PcktCnt-1) - r)) + 5.0 ); //조금 밀어서 집어야 한다.
                    MoveMotr(mi.SSTG_YGrpr, pv.SSTG_YGrprWorkStt , (OM.DevInfo.dSBOT_PcktPitch * ((OM.DevInfo.iSBOT_PcktCnt-1) - r)));
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr, pv.SSTG_YGrprWorkStt , (OM.DevInfo.dSBOT_PcktPitch * ((OM.DevInfo.iSBOT_PcktCnt-1) - r)))) return false;
                    MT_GoIncVel(mi.SSTG_YGrpr, 5.0, 5);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(IO_GetX(xi.SSTG_GrprOverload)){
                        ER_SetErr(ei.PRT_OverLoad , "Substrate");
                        MT_EmgStop(mi.SSTG_YGrpr);
                        return true ;
                    }
                    //if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWorkStt , (OM.DevInfo.dSBOT_PcktPitch * ((OM.DevInfo.iSBOT_PcktCnt-1) - r)) + 5.0))return false ;
                    if(!MT_GetStop(mi.SSTG_YGrpr)) return false;
                    MoveCyl(ci.SSTG_GrprClOp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Fwd))return false ;
                    
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprWorkStt , OM.DevInfo.dSBOT_PcktPitch); //조금 밀어서 집어야 한다.
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWorkStt , OM.DevInfo.dSBOT_PcktPitch)) return false;
                    MoveCyl(ci.SSTG_BoatClampClOp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 19:
                    if(!CL_Complete(ci.SSTG_BoatClampClOp, fb.Fwd)) return false;
                    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 20:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd))return false ;
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprWait);
                    IO_SetY(yi.SSTG_SubsVac, true);
                    Step.iCycle++;
                    return false ;

                case 21:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWait))return false ;
                    MoveMotr(mi.SSTG_ZRail, pv.SSTG_ZRailDown);
                    Step.iCycle++;
                    return false;

                case 22:
                    if(!MT_GetStopPos(mi.SSTG_ZRail, pv.SSTG_ZRailDown))return false ;
                    DM.ARAY[ri.SSTG].Step = DM.ARAY[ri.SSTG].GetCntStat(cs.WorkEnd);
                    r--;
                    OM.EqpStat.iSSTGStep=r;//20170619 진섭
                    Step.iCycle=0;
                    return true;
            }
        }

        public bool CycleOut()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                IO_SetY(yi.SSTG_IonizerOn , false);
                //Step.iCycle = 0;
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
                //return true ;
            }

            //int r = 0, c = 0;

            int iAray = 0;
            int iC;
            int iR;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    IO_SetY(yi.SSTG_SubsVac, false);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd))return false ;
                    MoveMotr(mi.SSTG_XFrnt , pv.SSTG_XFrntWait);
                    MoveMotr(mi.SSTG_YLeft , pv.SSTG_YLeftWait);
                    MoveMotr(mi.SSTG_YRght , pv.SSTG_YRghtWait);
                    MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailWait);
                    Step.iCycle++;
                    return false ;

                case 12: 
                    if(!MT_GetStopPos(mi.SSTG_XFrnt , pv.SSTG_XFrntWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_YLeft , pv.SSTG_YLeftWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_YRght , pv.SSTG_YRghtWait))return false ;
                    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailWait))return false ;
                    MoveMotr(mi.SSTG_XRail, pv.SSTG_XRailWork);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.SSTG_XRail, pv.SSTG_XRailWork))return false ;
                    if (FindChip(out iAray , out iC , out iR, cs.Mask   ))
                    {
                        if     (iAray == ri.SLDT) MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevTopWorkStt, iR * OM.DevInfo.dSLDR_SlotPitch);
                        else if(iAray == ri.SLDB) MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevBtmWorkStt, iR * OM.DevInfo.dSLDR_SlotPitch);
                    }

                    //if (FindChip(out iAray , out iC , out iR, cs.Mask   ) && iAray == ri.WLDT) MoveMotr(mi.WLDR_ZElev, pv.WLDR_ZElevTopWorkStt, -((OM.DevInfo.iWLDR_SlotCnt-1)-iR) * OM.DevInfo.dWLDR_SlotPitch);
                    //if (FindChip(out iAray , out iC , out iR, cs.Mask   ) && iAray == ri.WLDB) MoveMotr(mi.WLDR_ZElev, pv.WLDR_ZElevBtmWorkStt, -((OM.DevInfo.iWLDR_SlotCnt-1)-iR) * OM.DevInfo.dWLDR_SlotPitch);


                    MoveCyl(ci.SSTG_BoatClampClOp, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 14:
                    //if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprPickWait))return false ;
                    
                    if(!CL_Complete(ci.SSTG_BoatClampClOp, fb.Bwd)) return false;
                    if (!MT_GetStop(mi.SLDR_ZElev)) return false;
                    MT_GoIncRun(mi.SLDR_ZElev, -dSLDRGetOfs);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MT_GetStop(mi.SLDR_ZElev)) return false;
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprPick);
                    IO_SetY(yi.SSTG_IonizerOn , true);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(IO_GetX(xi.SSTG_GrprOverload)){
                        ER_SetErr(ei.PRT_OverLoad , "Substrate");
                        MT_EmgStop(mi.SSTG_YGrpr);
                        IO_SetY(yi.SSTG_IonizerOn , false);
                        return true ;
                    }
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprPick))return false ;
                    IO_SetY(yi.SSTG_IonizerOn , false);

                    //여기부터 데이터 넣고 핸들링.
                    FindChip(out iAray , out iC, out iR , cs.Mask);
                    DM.ARAY[iAray].SetStat(0,iR,cs.WorkEnd);
                    DM.ARAY[ri.SSTG].SetStat(cs.None);
                    MT_GoIncRun(mi.SLDR_ZElev, dSLDRGetOfs);

                    Step.iCycle++;
                    return false;

                case 17:
                    if (!MT_GetStop(mi.SLDR_ZElev)) return false;
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprPickWait);
                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprPickWait))return false ;
                    Step.iCycle=0;
                    return true;
            }
        }

        //레일 컨버젼 매뉴얼 사이클
        public bool CycleSubsRailConv() 
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                //Step.iCycle = 0;
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
                //return true ;
            }

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    if(IO_GetX(xi.SSTG_BoatCheck)) {
                        Log.ShowMessage("Warning", "레일 위에 자재가 있습니다");
                        return true;
                    }

                    if(IO_GetX(xi.SSTG_BoatRailIn)) {
                        Log.ShowMessage("Warning", "레일 투입구에 자재가 있습니다.");
                        return true;
                    }

                    if(IO_GetX(xi.SLDR_SstOutCheck)) {
                        Log.ShowMessage("Warning", "매거진에서 보트가 나와있습니다.");
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 11:
                    MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd))return false ;
                    MoveMotr(mi.SSTG_YGrpr , pv.SSTG_YGrprWait);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWait)) return false;
                    MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailWait);
                    Step.iCycle++;
                    return false ;

                case 14: 
                    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailWait))return false ;
                    MT_GoHome(mi.SSTG_XRail);
                    
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!MT_GetHomeDone(mi.SSTG_XRail)) return false ;
                    MoveMotr(mi.SSTG_XRail, pv.SSTG_XRailWork);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStopPos(mi.SSTG_XRail, pv.SSTG_XRailWork)) return false;
                    MoveMotr(mi.SSTG_YLeft, pv.SSTG_YLeftWait);
                    MoveMotr(mi.SSTG_YRght, pv.SSTG_YRghtWait);
                    MoveMotr(mi.SSTG_XFrnt, pv.SSTG_XFrntWait);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!MT_GetStopPos(mi.SSTG_YLeft, pv.SSTG_YLeftWait)) return false;
                    if (!MT_GetStopPos(mi.SSTG_YRght, pv.SSTG_YRghtWait)) return false;
                    if (!MT_GetStopPos(mi.SSTG_XFrnt, pv.SSTG_XFrntWait)) return false;
              
                    Step.iCycle = 0;
                    return true ;

            }
        }



        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.SSTG_GrprClOp){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if(_eActr == ci.SSTG_BoatClampClOp) {

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

            if (_eMotr == mi.SLDR_ZElev)
            {
                //if (!MT_GetStopInpos(mi.SSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.SSTG_YGrpr) + "is moving.";
                //    bRet = false;
                //}

                if (dDstPos > PM_GetValue(mi.SSTG_YGrpr, pv.SSTG_YGrprPickWait))
                {
                    sMsg = MT_GetName(mi.SSTG_YGrpr) + "Crash Position with " + MT_GetName(_eMotr);
                    bRet = false;
                }

                if (IO_GetX(xi.SLDR_SstOutCheck))
                {
                    sMsg = IO_GetXName(xi.SLDR_SstOutCheck) + "is Checking.";
                    bRet = false;
                }
            }

            else if (_eMotr == mi.SSTG_YGrpr){
                if (!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailWait)) {
                    if (!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailWait, -iSGrprOutOfs)) {
                        sMsg = MT_GetName(mi.SSTG_ZRail) + "is Down.";
                        bRet = false;
                    }
                }
            }
            else if (_eMotr == mi.SSTG_XRail){}
            else if (_eMotr == mi.SSTG_ZRail){
                if (!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWait)) {
                    sMsg = MT_GetName(mi.SSTG_ZRail) + "is Not in Wait Pos.";
                    bRet = false;
                }
            }
            else if (_eMotr == mi.SSTG_YLeft){}
            else if (_eMotr == mi.SSTG_YRght){}
            else if (_eMotr == mi.SSTG_XFrnt){}
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
            if (!MT_GetStop(mi.SSTG_YGrpr))return false;
            if (!MT_GetStop(mi.SSTG_XRail))return false;
            if (!MT_GetStop(mi.SSTG_ZRail))return false;
            if (!MT_GetStop(mi.SSTG_YLeft))return false;
            if (!MT_GetStop(mi.SSTG_YRght))return false;
            if (!MT_GetStop(mi.SSTG_XFrnt))return false;
            if (!MT_GetStop(mi.SLDR_ZElev))return false;

            return true ;
        }
        
    };

    
}
