using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using COMMON;
using SML2;



namespace Machine
{
    public class WaferStage : Part
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

        public const int iWLDRGetOfs = 1;
        public const int iWSTGGrprBwdOfs = -5;


        public WaferStage()
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
                    return true;

                case 10: 
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    if(OM.DevInfo.iWaferSize == 0) {//0: 8인치 , 1:12인치.
                        CL_Move(ci.WSTG_RailCvsLtFwBw,fb.Fwd);
                        CL_Move(ci.WSTG_RailCvsRtFwBw,fb.Fwd);
                    }
                    else {
                        CL_Move(ci.WSTG_RailCvsLtFwBw,fb.Bwd);
                        CL_Move(ci.WSTG_RailCvsRtFwBw,fb.Bwd);
                    }
                    Step.iToStart++;
                    return false ;
            
                case 11:
                    if(!CL_Complete(ci.WSTG_RailCvsLtFwBw)) return false ;
                    if(!CL_Complete(ci.WSTG_RailCvsRtFwBw)) return false ;
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Bwd)) return false ;

                    if(DM.ARAY[ri.WSTG].CheckAllStat(cs.None)) {
                        Step.iToStart=0;
                        return true ;
                    }
                    MoveMotr(mi.WSTG_ZExpd , pv.WSTG_ZExpdWork);
                    Step.iToStart++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.WSTG_ZExpd , pv.WSTG_ZExpdWork))return false ;
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
                    return true;

                case 10:
                    IO_SetY(yi.WSTG_HeatGunOn , false);
                    IO_SetY(yi.WSTG_IonizerOn , false);
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iToStop++;
                         return false;

                case 11: 
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Bwd)) return false ;
                    //일단 설계쪽 이상희 차장과 협의를 함.
                    //그리퍼가 웨이퍼 이상 포지션으로 나와있으려면 웨이퍼스테이지 세타가 Wait포지션에 있다.
                    //위의 것을 전제로 구성해본다.
                    if (DM.ARAY[ri.WSTG].CheckAllStat(cs.None)){
                        MoveMotr(mi.WSTG_TTble, pv.WSTG_TTbleWait);
                    }
                    
                    Step.iToStop++;
                    return false ;

                case 12:
                    if (!MT_GetStop(mi.WSTG_TTble)) return false;
                    //if (DM.ARAY[ri.WSTG].CheckAllStat(cs.None))
                    //{
                    //    MoveMotr(mi.WSTG_YGrpr, pv.WSTG_YGrprPickWait);
                    //}
                    //else
                    //{
                        MoveMotr(mi.WSTG_YGrpr, pv.WSTG_YGrprWait    );
                    //}
                    Step.iToStop++;
                    return false;

                case 13:
                    //혹시라도 처박으면..
                    if (IO_GetX(xi.WSTG_GrprOverload))
                    {
                        MT_EmgStop(mi.WSTG_YGrpr);
                        ER_SetErr(ei.PRT_OverLoad, "Wafer");
                        return true;
                    }
                    if (!MT_GetStop(mi.WSTG_YGrpr)) return false;
                    //오버로드 에러가 없거나 있어도 Wafer Stage께 아닌경우에만 ER_SetErr(ei.PRT_Missed , "Wafer를 Stage에 가져오는 동안 Gripper에서 빠졌습니다.");
                    bool bOverLoad  = ER_GetErrOn(ei.PRT_OverLoad) && ER_GetErrSubMsg(ei.PRT_OverLoad)!="Wafer" ;
                    bool bMissed    = ER_GetErrOn(ei.PRT_Missed  ) && ER_GetErrSubMsg(ei.PRT_Missed  )!="Wafer";
                    bool bBarcodeNG = ER_GetErrOn(ei.PRT_Barcode ) && ER_GetErrSubMsg(ei.PRT_Barcode )!="Wafer";
                    //if(!bOverLoad && !bMissed && !bBarcodeNG){
                    //    MoveMotr(mi.WLDR_ZElev, pv.WLDR_ZElevWait);
                    //}
                    Step.iToStop++;
                    return false;
                
                case 14: 
                    //if(!MT_GetStopPos(mi.WLDR_ZElev,pv.WLDR_ZElevWait)) return false ;

                    Step.iToStop = 0;
                    return true;
            }


        }

        override public int GetHomeStep   () { return      Step.iHome    ; } override public int GetPreHomeStep   () { return      PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return      Step.iToStart ; } override public int GetPreToStartStep() { return      PreStep.iToStart ; }
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

                //if( DM.ARAY[ri.WLDR].CheckAllStat(cs.None) && IO_GetX(xi.WLDR_MgzCheck)) {ER_SetErr(ei.PKG_Unknwn , "Wafer Loader Unknwn PKG Error."   ); return false;}
                //if(!DM.ARAY[ri.WLDR].CheckAllStat(cs.None) &&!IO_GetX(xi.WLDR_MgzCheck)) {ER_SetErr(ei.PKG_Dispr  , "Wafer Loader Disappear PKG Error."); return false;}
                bool bNoWorkChip = DM.ARAY[ri.WSTG].GetCntStat(cs.EjectAlign) == 0 &&
                                   DM.ARAY[ri.WSTG].GetCntStat(cs.Eject     ) == 0 &&
                                   DM.ARAY[ri.WSTG].GetCntStat(cs.Align     ) == 0 &&
                                   DM.ARAY[ri.WSTG].GetCntStat(cs.Pick      ) == 0 ;

                bool bToolLongWork = DM.ARAY[ri.PCKR].CheckStat(0 , 0 , cs.Distance) && DM.ARAY[ri.PCKR].CheckStat(0 , 0 , cs.Attach) ;

                bool isCycleGet =!bToolLongWork &&
                                  DM.ARAY[ri.WSTG].CheckAllStat(cs.None ) && ( DM.ARAY[ri.WLDT].IsExist(cs.Unknown) ||  DM.ARAY[ri.WLDB].IsExist(cs.Unknown));
                bool isCycleOut = bNoWorkChip                             && ( DM.ARAY[ri.WLDT].IsExist(cs.Mask   ) ||  DM.ARAY[ri.WLDB].IsExist(cs.Mask   ));
                bool isCycleEnd = DM.ARAY[ri.WSTG].CheckAllStat(cs.None ) && (!DM.ARAY[ri.WLDT].IsExist(cs.Unknown) && !DM.ARAY[ri.WLDB].IsExist(cs.Unknown));
                                   

                //에러 띄우고 그냥 랏엔드는 거의 안하다 시피 한다.
                //어차피 랏데이터도 자제별로 남겨서 의미 없음.
                int iSStgR = 0      ; 
                int iSStgC = 0      ;
                int iSAray = ri.SSTG;
                SEQ.SSTG.FindChip(out iSAray, out iSStgC, out iSStgR);                

                //SSTG : Align->Height->Disp    ->DispVisn->Attach                                                ->EndVisn->EndHeight->WorkEnd
                //WSTG :                                            Eject ->Align->Pick  ->Empty   
                //PCKR :                                            None         ->BtmVisn  -> Attach
                bool bSSTGWorkEnd  =  DM.ARAY[ri.PCKR].CheckAllStat(cs.None) &&
                                     !DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.BltHeight ) && //끝나고 높이 측정하여 BLT 측정.
                                     !DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.EndVisn   ) ;  //끝나고 좌측 우측 모두 거리 측정 하기.

                //웨이퍼는 할일 없고 툴은 자제가 없으며 섭스스테이지는 1자제의 비전검사까지 끝났을때.
                if(isCycleEnd && bSSTGWorkEnd) {
                    if(SEQ.TOOL.m_bContiWork) { //계속작업모드면
                        ER_SetErr(ei.ETC_NeedPkg,"웨이퍼로더에 웨이퍼를 공급하고 마스킹해주세요.");
                        return false ;
                    }
                    else { //작업엔드 모드면
                        if(!DM.ARAY[ri.SSTG].CheckAllStat(cs.None)) DM.ARAY[ri.SSTG].SetStat(cs.WorkEnd); //서브스트레이트 스테이지에 있는 자제 다 워크엔드 시키고.
                        DM.ARAY[ri.SLDB].ChangeStat(cs.Unknown , cs.WorkEnd);  //로더에 언노운 있던것을 다 WorkEnd시킨다.
                        DM.ARAY[ri.SLDT].ChangeStat(cs.Unknown , cs.WorkEnd);
                    }                    
                }
                   

                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleGet   ) { Step.eSeq = sc.Get ;  }
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
                case (sc.Out   ): if (CycleOut     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;

            }
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        

        //밑에 부터 작업.
        public bool FindChip(out int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            ////디폴트값으로 들어오면 현재 작업중인 칩을 리턴 한다.
            //if(_iChip == cs.RetFail) {
            //    DM.ARAY[_iId].FindLastRowCol(cs.Empty , ref c , ref r);
            //    if(r<0 || c<0){
            //        r=0;
            //        c=0;
            //    }
            //    else {                    
            //        if (c+1 <= OM.DevInfo.iWFER_DieCntX)
            //        {
            //            r++;
            //            c = 0;
            //        }
            //        else
            //        {
            //            c++;
            //        }
            //    }
            //    return true ;
            //}
            //
            //DM.ARAY[_iId].FindLastRowCol(_iChip , ref c , ref r);
            //return (c >= 0 && r >= 0) ? true : false;
            if(_iChip != cs.RetFail) { //로더쪽.
                if(-1 != DM.ARAY[ri.WLDB].FindLastRow(_iChip)){
                    _iId = ri.WLDB ;
                    r  = DM.ARAY[ri.WLDB].FindLastRow(_iChip) ;
                    return true ;
                }            
                else if(-1 != DM.ARAY[ri.WLDT].FindLastRow(_iChip)){
                    _iId = ri.WLDT ;
                    r  = DM.ARAY[ri.WLDT].FindLastRow(_iChip) ;
                    return true ;
                }
            }
                
            if(_iChip == cs.RetFail) {//작업열 위치 가져 오기.
                int iC = 0 ;
                int iR = 0 ;                
                if(DM.ARAY[ri.WSTG].FindFrstColRow(cs.Pick , ref iC , ref iR)){
                    c = iC ; 
                    r = iR ;
                    _iId = ri.WSTG ;
                    return true ;
                }
                else if(DM.ARAY[ri.WSTG].FindFrstColRow(cs.Align , ref iC , ref iR)){
                    c = iC ; 
                    r = iR ;
                    _iId = ri.WSTG ;
                    return true ;
                }
                else if(DM.ARAY[ri.WSTG].FindFrstColRow(cs.Eject , ref iC , ref iR)){
                    c = iC ; 
                    r = iR ;
                    _iId = ri.WSTG ;
                    return true ;
                }
                else if(DM.ARAY[ri.WSTG].FindFrstColRow(cs.EjectAlign , ref iC , ref iR)){
                    c = iC ; 
                    r = iR ;
                    _iId = ri.WSTG ;
                    return true ;
                }
                return false ;
            }
            
            //나머지는 해당 칩을 찾아서 리턴.
            DM.ARAY[ri.WSTG].FindFrstRowCol(_iChip , ref c , ref r);
            _iId = ri.WSTG ;
            return (c >= 0 && r >= 0) ? true : false;

            /*
             public bool FindChip(int _iId , ref int c, ref int r, cs _iChip = cs.RetFail) 
        {
            

            //나머지는 해당 칩을 찾아서 리턴.
            DM.ARAY[ri.WSTG].FindFrstRowCol(_iChip , ref c , ref r);
            return (c >= 0 && r >= 0) ? true : false;
        }       
             */
        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 8000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.PRT_CycleTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
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
            
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;
            
                case 10:
                    if(OM.DevInfo.iWaferSize == 0) {//0: 8인치 , 1:12인치.
                        CL_Move(ci.WSTG_RailCvsLtFwBw,fb.Fwd);
                        CL_Move(ci.WSTG_RailCvsRtFwBw,fb.Fwd);
                    }
                    else {
                        CL_Move(ci.WSTG_RailCvsLtFwBw,fb.Bwd);
                        CL_Move(ci.WSTG_RailCvsRtFwBw,fb.Bwd);
                    }
                    CL_Move(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iHome++;
                    return false ;
            
                case 11:
                    if(!CL_Complete(ci.WSTG_RailCvsLtFwBw)) return false ;
                    if(!CL_Complete(ci.WSTG_RailCvsRtFwBw)) return false ;
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Bwd)) return false ;

                    MT_GoHome(mi.WSTG_TTble);
                    Step.iHome++;
                    return false;

                case 12:
                    if (!MT_GetHomeDone(mi.WSTG_TTble)) return false;
                    MT_GoAbsMan(mi.WSTG_TTble , pv.WSTG_TTbleWait);
                    Step.iHome++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.WSTG_TTble , pv.WSTG_TTbleWait))return false ;
                    MT_GoHome(mi.WSTG_ZExpd);
                    MT_GoHome(mi.TOOL_ZEjtr);
                    Step.iHome++;
                    return false ;

                case 14:
                    if (!MT_GetHomeDone(mi.WSTG_ZExpd)) return false;
                    if (!MT_GetHomeDone(mi.TOOL_ZEjtr)) return false;
                    MT_GoHome(mi.WSTG_YGrpr);
                    MT_GoHome(mi.TOOL_XEjtL);
                    MT_GoHome(mi.TOOL_XEjtR);
                    MT_GoHome(mi.TOOL_YEjtr);
                    Step.iHome++;
                    return false ;

                case 15:
                    if (!MT_GetHomeDone(mi.WSTG_YGrpr)) return false;
                    if (!MT_GetHomeDone(mi.TOOL_XEjtL)) return false;
                    if (!MT_GetHomeDone(mi.TOOL_XEjtR)) return false;
                    if (!MT_GetHomeDone(mi.TOOL_YEjtr)) return false;
                    MT_GoHome(mi.WLDR_ZElev);
                    Step.iHome++;
                    return false;

                case 16:
                    if (!MT_GetHomeDone(mi.WLDR_ZElev)) return false;
                    MT_GoAbsMan(mi.WSTG_YGrpr,pv.WSTG_YGrprWait);        
                    MT_GoAbsMan(mi.WSTG_ZExpd,pv.WSTG_ZExpdWait);        
                    MT_GoAbsMan(mi.TOOL_ZEjtr,pv.TOOL_ZEjtrWait);        
                    MT_GoAbsMan(mi.TOOL_XEjtL,pv.WSTG_XEjtLWait);        
                    MT_GoAbsMan(mi.TOOL_XEjtR,pv.WSTG_XEjtRWait);
                    MT_GoAbsMan(mi.TOOL_YEjtr,pv.TOOL_YEjtrWait);
                    MT_GoAbsMan(mi.WLDR_ZElev,pv.WLDR_ZElevWait);    
                    Step.iHome++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr,pv.WSTG_YGrprWait))return false ;    
                    if(!MT_GetStopPos(mi.WSTG_ZExpd,pv.WSTG_ZExpdWait))return false ;    
                    if(!MT_GetStopPos(mi.TOOL_ZEjtr,pv.TOOL_ZEjtrWait))return false ;    
                    if(!MT_GetStopPos(mi.TOOL_XEjtL,pv.WSTG_XEjtLWait))return false ;    
                    if(!MT_GetStopPos(mi.TOOL_XEjtR,pv.WSTG_XEjtRWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_YEjtr,pv.TOOL_YEjtrWait))return false ;
                    if(!MT_GetStopPos(mi.WLDR_ZElev,pv.WLDR_ZElevWait))return false ;
                    Step.iHome = 0;
                    return true ;
            }
        }

        public bool SetMapFile(int _iAray , string _sBarcode)
        {
            //기존에 있던것들 지우기.
            DirectoryInfo di = new DirectoryInfo(OM.CmnOptn.sMapFileFolder);
            if (!di.Exists) {
                di.Create();
                Program.SendListMsg("Map file 폴더가 없어서 만들었습니다.");
                return false ;
            }

            string sLotId = "" ;
            string sWfrNo = "" ;
            for(int i = 0 ; i < _sBarcode.Length ; i++ )
            {                
                if(OM.CmnOptn.sBarLotIDMask.Length > i && OM.CmnOptn.sBarLotIDMask.Substring(i,1)=="*"){
                    sLotId += _sBarcode.Substring(i,1);
                }

                if(OM.CmnOptn.sBarWfrNoMask.Length > i && OM.CmnOptn.sBarWfrNoMask.Substring(i,1)=="*"){
                    sWfrNo += _sBarcode.Substring(i,1);
                }
            }

            int iStart  = -1 ;
            int iLength = 0 ; 
            for(int i = 0 ; i < OM.CmnOptn.sFileLotIDMask.Length ; i++){
                if(iStart == -1){//시작
                    if(OM.CmnOptn.sFileLotIDMask.Length > i &&  OM.CmnOptn.sFileLotIDMask.Substring(i,1)=="*"){
                        iStart = i ;
                    }
                }
                else {//끝 및 길이.
                    if(OM.CmnOptn.sFileLotIDMask.Length > i &&  OM.CmnOptn.sFileLotIDMask.Substring(i,1)!="*"){
                        iLength = i - iStart ;
                    }
                }
            }
            if (iLength == 0)
            {
                iLength = OM.CmnOptn.sFileLotIDMask.Length - iStart ;
            }

            bool bFoundWaferLot = false ;
            string sLine = "" ;
            string sChipStat ="" ;
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension != ".csv") continue;
                if(fi.Name.Length < iStart + iLength)continue ;
                if (fi.Name.Substring(iStart , iLength) != sLotId) continue;
                try 
                { 
                    StreamReader sr = new StreamReader(OM.CmnOptn.sMapFileFolder + "\\" + fi.Name , Encoding.GetEncoding("utf-8")); 
                    while(!sr.EndOfStream) 
                    { 
                        sLine = sr.ReadLine(); 
                        if(!bFoundWaferLot && !sLine.Contains("Wafer Lot")) continue ;

                        //위아래 똑같은 맵이 2개 있어서 중간에 "Wafer Lot"이 한번 나오고 나서 맵을 사용 해야 함.
                        //두개의 맵이 똑같긴 한데 일단 협의를 밑에꺼로 쓴다고 함.
                        if(!bFoundWaferLot) { 
                            string[] sData = sLine.Split(',');
                            if(sData.Length < 2) {
                                Program.SendListMsg("Wafer Lot Reading 실패");
                                return false ;
                            }
                            if(sData[1] != sLotId) {
                                Program.SendListMsg("Wafer의 랏넘버와 Mapfile의 WaferLot이 다릅니다.");
                                return false ;
                            }
                            bFoundWaferLot = true ;
                        }

                        if(!sLine.Contains("#"+sWfrNo)) continue ;
                        
                        Program.SendListMsg("#"+sWfrNo + " 로딩");
                        Program.SendListMsg(sLine);
                        string[] sChips = sLine.Split(','); 
                        
                        int iDataCnt = 0 ;

                        List<string> lsChips = new List<string>();
                        for (int i = 0 ; i < sChips.Length ; i++) 
                        {
                            if(i==0) continue ;//칩번호가 들어있음 ex) #01
                            if(sChips[i] == "F" || sChips[i] == "P" || sChips[i] =="X"){
                                lsChips.Add(sChips[i]);
                            }
                            else if (sChips[i] == "")
                            {

                            }
                            else
                            {
                                lsChips.Add("X");
                            }
                        }
                        int iMaxDataCnt = lsChips.Count() ;

                        if(!OM.DevOptn.bRvsWafer){
                            for(int c = 0 ; c < DM.ARAY[_iAray].GetMaxCol() ; c++){
                                for (int r = 0; r < DM.ARAY[_iAray].GetMaxRow(); r++){
                                    if(iDataCnt >= iMaxDataCnt) continue ;
                                    sChipStat = lsChips[iDataCnt];
                                    iDataCnt++;
                                         if(sChipStat=="P") DM.ARAY[_iAray].SetStat(c,r,cs.EjectAlign) ;
                                    else if(sChipStat=="F") DM.ARAY[_iAray].SetStat(c,r,cs.Fail      ) ;
                                    else if(sChipStat=="X") DM.ARAY[_iAray].SetStat(c,r,cs.Empty     ) ;
                                    else                    DM.ARAY[_iAray].SetStat(c,r,cs.Fail      ) ;     
                                }
                            }                            
                        }
                        else {
                            for(int c = DM.ARAY[_iAray].GetMaxCol() - 1 ; c >= 0 ; c--){
                                for (int r = DM.ARAY[_iAray].GetMaxRow() -1; r >= 0 ; r--){
                                    if(iDataCnt >= iMaxDataCnt) continue ;
                                    sChipStat = lsChips[iDataCnt];
                                    iDataCnt++;
                                         if(sChipStat=="P") DM.ARAY[_iAray].SetStat(c,r,cs.EjectAlign) ;
                                    else if(sChipStat=="F") DM.ARAY[_iAray].SetStat(c,r,cs.Fail      ) ;
                                    else if(sChipStat=="X") DM.ARAY[_iAray].SetStat(c,r,cs.Empty     ) ;
                                    else                    DM.ARAY[_iAray].SetStat(c,r,cs.Fail      ) ; 
                                }
                            }  
                        }

                        return true ;
                    }  
                    Program.SendListMsg("Map file에 해당 웨이퍼의 정보가 없습니다.");
                    sr.Close();
                    sr.Dispose();
                    return false ;

                } 
                catch (Exception e) 
                { 
                    Program.SendListMsg(e.Message);
                    return false ;
                }
            }         
            return false ;
        }
        
        public bool CycleGet()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                IO_SetY(yi.WSTG_HeatGunOn , false);
                IO_SetY(yi.WSTG_IonizerOn , false);
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
            int iAray;
            int iC;
            int iR;
            

            if(CL_Complete(ci.WSTG_GrprClOp , fb.Fwd))
            {
                if(!MT_GetStop(mi.WSTG_YGrpr))
                {
                    //이것은 어차피 놓치는 거라 그냥 움직이고 나서 확인 하는 것으로 한다.
                }                
            }
            
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;
                case 10:
                    MoveMotr(mi.WSTG_TTble , pv.WSTG_TTbleWait);
                    MoveMotr(mi.TOOL_ZEjtr , pv.TOOL_ZEjtrWait);
                    MoveMotr(mi.WSTG_ZExpd , pv.WSTG_ZExpdWait);
                    
                    if(OM.DevOptn.iGetHeatDelay != 0) {//예열
                        IO_SetY(yi.WSTG_HeatGunOn , true);
                    }
                    Step.iCycle++;
                    return false ;

                case 11: 
                    if(!MT_GetStopPos(mi.WSTG_TTble , pv.WSTG_TTbleWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZEjtr , pv.TOOL_ZEjtrWait))return false ;
                    if(!MT_GetStopPos(mi.WSTG_ZExpd , pv.WSTG_ZExpdWait))return false ;                   
                    
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Bwd))return false;
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPickWait);

                         if (FindChip(out iAray , out iC , out iR, cs.Unknown) && iAray == ri.WLDB) MoveMotr(mi.WLDR_ZElev, pv.WLDR_ZElevBtmWorkStt, (iR - (OM.DevInfo.iWLDR_SlotCnt -1)) * OM.DevInfo.dWLDR_SlotPitch);
                    else if (FindChip(out iAray , out iC , out iR, cs.Unknown) && iAray == ri.WLDT) MoveMotr(mi.WLDR_ZElev, pv.WLDR_ZElevTopWorkStt, (iR - (OM.DevInfo.iWLDR_SlotCnt -1)) * OM.DevInfo.dWLDR_SlotPitch);
                    else {
                        Step.iCycle=0 ;
                        return true ;
                    }

                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr,pv.WSTG_YGrprPickWait))return false;
                    if(!MT_GetStop(mi.WLDR_ZElev))return false;
                    if(IO_GetX(xi.WSTG_GrprPkgCheck)){
                        ER_SetErr(ei.PKG_Dispr , "웨이퍼가 감지중입니다. 로더를 확인해주세요");
                        return true ;
                    }
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPick);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprPick)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!m_tmDelay.OnDelay(200)) return false ;
                    if(!IO_GetX(xi.WSTG_GrprPkgCheck)) {//자제 없는경우.

                        Step.iCycle = 50 ;
                        return false ;
                    }
                    Step.iCycle++;
                    return false;

                //자제 있는경우.
                case 16: 
                    MoveCyl(ci.WSTG_GrprClOp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Fwd)) return false ;
                    //MT_GoIncRun(mi.WLDR_ZElev, -iWLDRGetOfs);

                    Step.iCycle++;
                    return false;

                case 18:
                    //if (!MT_GetStop(mi.WLDR_ZElev)) return false;
                    MT_GoIncRun(mi.WSTG_YGrpr, iWSTGGrprBwdOfs);
                    Step.iCycle++;
                    return false;

                case 19:
                    if(!MT_GetStop(mi.WSTG_YGrpr)) return false;
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 20:
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Bwd)) return false ;
                    //MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPick);
                    MT_GoAbsVel(mi.WSTG_YGrpr, PM_GetValue(mi.WSTG_YGrpr, pv.WSTG_YGrprPick), 10);
                    Step.iCycle++;
                    return false ;

                case 21:
                    if(!MT_GetStop(mi.WSTG_YGrpr)) return false;
                    MoveCyl(ci.WSTG_GrprClOp , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 22:
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Fwd)) return false;
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprBarcode);
                    IO_SetY(yi.WSTG_IonizerOn , true);
                    Step.iCycle++;
                    return false ;

                case 23:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprBarcode)) return false ;

                    Step.iCycle++;
                    return false ;

                case 24:
                    
                    if(!OM.CmnOptn.bWfrBarcodeSkip) {
                        //바코드 읽기.
                        SEQ.BarcordWSTG.SendScanOn();
                    } 
                    
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 25:
                    if(!OM.CmnOptn.bWfrBarcodeSkip) {
                        if(m_tmDelay.OnDelay(3000)) {//바코드 못읽었을때 상황.
                            Step.iCycle=60;
                            return false;
                        }
                        if(SEQ.BarcordWSTG.GetText()=="")return false ;
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 26:
                    if(!m_tmDelay.OnDelay(100)) return false ;
                      
                    if(!OM.CmnOptn.bWfrBarcodeSkip) {
                        OM.EqpStat.sWSTGBarcode = SEQ.BarcordWSTG.GetText();
                        int iTemp = 0;
                        for(int r = 0;r < DM.ARAY[ri.WSTG].MaxRow; r++)
                        {
                            for (int c = 0; c < DM.ARAY[ri.WSTG].MaxCol; c++)
                            {
                                DM.ARAY[ri.WSTG].Chip[c, r].Data = SEQ.BarcordWSTG.GetText() + "_" + iTemp.ToString() ;
                                iTemp++;
                            }
                        }

                        Program.SendListMsg("WSTG BAR:"+OM.EqpStat.sWSTGBarcode);    

                        //나중에 맵을 확인 해서 하는 걸로 수정.
                        //DM.ARAY[ri.WSTG].SetStat(cs.EjectAlign);
                        if (!SetMapFile(ri.WSTG , SEQ.BarcordWSTG.GetText()))
                        {
                            Step.iCycle=70;
                            return false;
                        }

                        //맵파일 로딩 했는데 굿이 하나도 없을때.
                        if (DM.ARAY[ri.WSTG].GetCntStat(cs.EjectAlign)==0)
                        {
                            Step.iCycle=90;
                            return false;
                        }

                        FindChip(out iAray , out iC, out iR , cs.Unknown);
                        DM.ARAY[iAray].SetStat(0,iR,cs.Mask);
                    }
                    else {
                        //연월일시간분초 입력
                        DM.ARAY[ri.WSTG].LotNo = DateTime.Now.ToString("yyyyMMddhhmmss"); //ID에 넣는게 맞는지 확인해야함 Jinseop

                        //나중에 맵을 확인 해서 하는 걸로 수정.
                        DM.ARAY[ri.WSTG].SetStat(cs.EjectAlign);
                        FindChip(out iAray , out iC, out iR , cs.Unknown);
                        DM.ARAY[iAray].SetStat(0,iR,cs.Mask);
                    }
                    
                    //예열 끝.
                    IO_SetY(yi.WSTG_HeatGunOn , false);

                    //바코드 리딩 성공.
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPlace);
                    Step.iCycle++;
                    return false ;

                case 27:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprPlace)) return false ;
                    IO_SetY(yi.WSTG_IonizerOn , false);

                    


                    ////나중에 맵을 확인 해서 하는 걸로 수정.
                    //DM.ARAY[ri.WSTG].SetStat(cs.EjectAlign);

                    //FindChip(out iAray , out iC, out iR , cs.Unknown);
                    //DM.ARAY[iAray].SetStat(0,iR,cs.Mask);

                    if(!IO_GetX(xi.WSTG_GrprPkgCheck)) {//자제 없는경우.
                        ER_SetErr(ei.PRT_Missed , "웨이퍼가 정확히 이송되지 않았습니다. 웨이퍼를 스테이지에 넣어주세요");
                        Step.iCycle = 80;
                        return false ;
                    }

                    MT_GoIncVel(mi.WSTG_YGrpr, -2.0, 10);
                    Step.iCycle++;
                    return false;

                case 28:
                    if(!MT_GetStop(mi.WSTG_YGrpr)) return false;
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 29:
                    if(!CL_Complete(ci.WSTG_GrprClOp,fb.Bwd))return false ;
                    

                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprWait);
                    

                    Step.iCycle++;
                    return false ;

                case 30:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprWait)) return false ;

                    if(OM.DevOptn.iGetHeatDelay != 0) {//예열
                        IO_SetY(yi.WSTG_HeatGunOn , true);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 31:
                    if(!m_tmDelay.OnDelay(OM.DevOptn.iGetHeatDelay))return false ;
                    IO_SetY(yi.WSTG_HeatGunOn , false);
                    MoveMotr(mi.WSTG_ZExpd , pv.WSTG_ZExpdWork);
                    Step.iCycle++;
                    return false ;

                case 32:
                    if(!MT_GetStopPos(mi.WSTG_ZExpd , pv.WSTG_ZExpdWork))return false ;
                    MoveMotr(mi.WSTG_TTble , pv.WSTG_TTbleWork);
                    Step.iCycle++;
                    return false;

                case 33:
                    if(!MT_GetStopPos(mi.WSTG_TTble , pv.WSTG_TTbleWork))return false ;

                    //처음 웨이퍼를 받아오면 기준 세팅값을 WfrWork에 넣어준다.
                    //비전 범위 아웃 에러 나면 오퍼 화면에서 오퍼레이터가 WfrWork를 조절 한다.
                    //즉 Work는 기준값 WfrWork는 해당 웨이퍼의 보정된 값이다.
                    PM.SetValue(mi.WSTG_TTble , pv.WSTG_TTbleWfrWork , PM.GetValue(mi.WSTG_TTble , pv.WSTG_TTbleWork));
                    PM.Save(OM.GetCrntDev());

                    Step.iCycle=0;
                    return true;


                //메거진슬롯에 웨이퍼가 없을경우.
                case 50:
                    IO_SetY(yi.WSTG_HeatGunOn , false);
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPickWait);
                    Step.iCycle++;
                    return false ;

                case 51:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprPickWait)) return false ;
                    
                    FindChip(out iAray , out iC, out iR , cs.Unknown);
                    DM.ARAY[iAray].SetStat(0,iR,cs.Empty);

                    Step.iCycle=0;
                    return true ;


                //바코드 못읽었을때 상황.
                case 60:               
                    IO_SetY(yi.WSTG_HeatGunOn , false);
                    IO_SetY(yi.WSTG_IonizerOn , false);
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 61:
                    if(CL_Complete(ci.WSTG_GrprClOp,fb.Bwd))return false ;
                    ER_SetErr(ei.PRT_Barcode , "Wafer");
                    Step.iCycle=0;
                    return true ;

                //맵파일 못읽었을때 상황.
                case 70:               
                    IO_SetY(yi.WSTG_HeatGunOn , false);
                    IO_SetY(yi.WSTG_IonizerOn , false);
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 71:
                    if(CL_Complete(ci.WSTG_GrprClOp,fb.Bwd))return false ;
                    ER_SetErr(ei.PRT_Barcode , "MapFile을 읽기 에러.");
                    Step.iCycle=0;
                    return true ;

                //웨이퍼를 놓쳤을때 상황.
                case 80:
                    IO_SetY(yi.WSTG_HeatGunOn , false);
                    IO_SetY(yi.WSTG_IonizerOn , false);
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprWait);
                    Step.iCycle++;
                    return false ;

                case 81:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprWait)) return false ;
                    Step.iCycle = 0;
                    return true ;

                //맵파일 읽었는데 작업할 칩이 없는경우.
                case 90:
                    IO_SetY(yi.WSTG_HeatGunOn , false);
                    IO_SetY(yi.WSTG_IonizerOn , false);
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 91:
                    if(CL_Complete(ci.WSTG_GrprClOp,fb.Bwd))return false ;

                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPick);
                    Step.iCycle++;
                    return false ;

                case 92:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprPick)) return false ;
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPickWait);
                    Step.iCycle++;
                    return false ;

                case 93:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprPickWait))return false ;
                    DM.ARAY[ri.WSTG].SetStat(cs.None);
                    
                    FindChip(out iAray , out iC, out iR , cs.Unknown);
                    DM.ARAY[iAray].SetStat(0,iR,cs.WorkEnd);

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
                SM.IO_SetY(yi.WSTG_HeatGunOn , false);
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
            int iAray;
            int iC;
            int iR;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Bwd))return false ;
                    MoveMotr(mi.WSTG_ZExpd , pv.WSTG_ZExpdWait);
                    MoveMotr(mi.TOOL_ZEjtr , pv.TOOL_ZEjtrWait);
                    MoveMotr(mi.WSTG_TTble , pv.WSTG_TTbleWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.WSTG_ZExpd , pv.WSTG_ZExpdWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZEjtr , pv.TOOL_ZEjtrWait))return false ;
                    if(!MT_GetStopPos(mi.WSTG_TTble , pv.WSTG_TTbleWait))return false ;

                    if(OM.DevOptn.iOutHeatDelay != 0) {
                        SM.IO_SetY(yi.WSTG_HeatGunOn , true);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!m_tmDelay.OnDelay(OM.DevOptn.iOutHeatDelay))return false ;
                    SM.IO_SetY(yi.WSTG_HeatGunOn , false);
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPlace);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprPlace))return false ;
                    //     if(FindChip(ri.WLDR , out c, out r , cs.Mask   )) MoveMotr(mi.WLDR_ZElev,pv.WLDR_ZElevBtmWorkStt,-r*OM.DevInfo.dWLDR_SlotPitch);
                    //else if(FindChip(ri.WLDR , out c, out r , cs.Unknown)) MoveMotr(mi.WLDR_ZElev,pv.WLDR_ZElevBtmWorkStt,-r*OM.DevInfo.dWLDR_SlotPitch);

                         if (FindChip(out iAray , out iC , out iR, cs.Mask   ) && iAray == ri.WLDB) MoveMotr(mi.WLDR_ZElev, pv.WLDR_ZElevBtmWorkStt, -((OM.DevInfo.iWLDR_SlotCnt-1)-iR) * OM.DevInfo.dWLDR_SlotPitch);
                    else if (FindChip(out iAray , out iC , out iR, cs.Mask   ) && iAray == ri.WLDT) MoveMotr(mi.WLDR_ZElev, pv.WLDR_ZElevTopWorkStt, -((OM.DevInfo.iWLDR_SlotCnt-1)-iR) * OM.DevInfo.dWLDR_SlotPitch);
                    else {
                        Step.iCycle=0 ;
                        return true ;
                    }
                    
                    


                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MT_GetStop(mi.WLDR_ZElev)) return false;
                    MT_GoIncRun(mi.WLDR_ZElev, -iWLDRGetOfs);

                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStop(mi.WLDR_ZElev)) return false;
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPick);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(IO_GetX(xi.WSTG_GrprOverload)){
                        ER_SetErr(ei.PRT_OverLoad , "Wafer");
                        MT_EmgStop(mi.WSTG_YGrpr);
                        return true ;
                    }
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprPick))return false ;

                    //여기부터 데이터 넣고 핸들링.
                    FindChip(out iAray , out iC, out iR , cs.Mask);
                    DM.ARAY[iAray].SetStat(0,iR,cs.WorkEnd);
                    MT_GoIncRun(mi.WLDR_ZElev, iWLDRGetOfs);

                    Step.iCycle++;
                    return false;

                case 18:
                    if (!MT_GetStop(mi.WLDR_ZElev)) return false;
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprPickWait);
                    Step.iCycle++;
                    return false ;

                case 19:
                    if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprPickWait))return false ;
                    DM.ARAY[ri.WSTG].SetStat(cs.None);
                    Step.iCycle=0;
                    return true;
            }
        }

        //레일 컨버젼 매뉴얼 사이클
        public bool CycleWafrRailConv() 
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
                    if(IO_GetX(xi.WSTG_GrprPkgCheck)) {
                        Log.ShowMessage("Warning", "그립퍼에 자재가 감지중입니다.");
                        return true;
                    }

                    if(IO_GetX(xi.WLDR_WfrOutCheck)) {
                        Log.ShowMessage("Warning", "매거진에서 웨이퍼가 나와있습니다.");
                        return true;
                    }
                    Step.iCycle++;
                    return false;

                case 11:
                    MoveCyl(ci.WSTG_GrprClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.WSTG_GrprClOp , fb.Bwd))return false ;
                    MoveMotr(mi.WSTG_YGrpr , pv.WSTG_YGrprWait);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.SSTG_YGrpr , pv.SSTG_YGrprWait)) return false;
                    MoveMotr(mi.WSTG_ZExpd , pv.WSTG_ZExpdWait);
                    Step.iCycle++;
                    return false ;

                case 14: 
                    if(!MT_GetStopPos(mi.WSTG_ZExpd , pv.WSTG_ZExpdWait)) return false ;
                    MoveMotr(mi.WSTG_TTble, pv.WSTG_TTbleWait);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!MT_GetStopPos(mi.WSTG_TTble, pv.WSTG_TTbleWait)) return false ;

                    if(OM.DevInfo.iWaferSize == 0) {//0: 8인치 , 1:12인치.
                        CL_Move(ci.WSTG_RailCvsLtFwBw,fb.Fwd);
                        CL_Move(ci.WSTG_RailCvsRtFwBw,fb.Fwd);
                    }
                    else {
                        CL_Move(ci.WSTG_RailCvsLtFwBw,fb.Bwd);
                        CL_Move(ci.WSTG_RailCvsRtFwBw,fb.Bwd);
                    }

                    Step.iCycle++;
                    return false;

                case 16:
                    if (!CL_Complete(ci.WSTG_RailCvsLtFwBw)) return false;
                    if (!CL_Complete(ci.WSTG_RailCvsRtFwBw)) return false;
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.WSTG_GrprClOp){
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

            if (_eMotr == mi.WLDR_ZElev)
            {
                //if (!MT_GetStopInpos(mi.WSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.WSTG_YGrpr) + "is moving.";
                //    bRet = false;
                //}

                if (dDstPos > PM_GetValue(mi.WSTG_YGrpr, pv.WSTG_YGrprPickWait))
                {
                    sMsg = MT_GetName(mi.WSTG_YGrpr) + "Crash Position with " + MT_GetName(_eMotr);
                    bRet = false;
                }

                if (IO_GetX(xi.WLDR_WfrOutCheck))
                {
                    sMsg = IO_GetXName(xi.WLDR_WfrOutCheck) + "is Checking.";
                    bRet = false;
                }
            }

            else if (_eMotr == mi.WSTG_YGrpr)
            {
                if (!MT_GetStop(mi.WLDR_ZElev)) {sMsg = MT_GetName(mi.WSTG_YGrpr) + "is moving.";bRet = false;}
                if (!MT_GetStop(mi.WSTG_TTble)) {sMsg = MT_GetName(mi.WSTG_TTble) + "is moving.";bRet = false;}
                if (!MT_GetStop(mi.TOOL_XEjtL)) {sMsg = MT_GetName(mi.TOOL_XEjtL) + "is moving.";bRet = false;}
                if (!MT_GetStop(mi.TOOL_XEjtR)) {sMsg = MT_GetName(mi.TOOL_XEjtR) + "is moving.";bRet = false;}
                if (!MT_GetStop(mi.TOOL_YEjtr)) {sMsg = MT_GetName(mi.TOOL_YEjtr) + "is moving.";bRet = false;}
                if (!MT_GetStop(mi.TOOL_ZEjtr)) {sMsg = MT_GetName(mi.TOOL_ZEjtr) + "is moving.";bRet = false;}
                if (!MT_GetStop(mi.WSTG_ZExpd)) {sMsg = MT_GetName(mi.WSTG_ZExpd) + "is moving.";bRet = false;}

                if (dDstPos > PM_GetValue(mi.WSTG_YGrpr, pv.WSTG_YGrprPick))
                {
                    sMsg = MT_GetName(mi.WSTG_YGrpr) + "Crash Position with " + MT_GetName(_eMotr);
                    bRet = false;
                }

                //if (IO_GetX(xi.WLDR_WfrOutCheck))
                //{
                //    sMsg = IO_GetXName(xi.WLDR_WfrOutCheck) + "is Checking.";
                //    bRet = false;
                //}
            }
            else if(_eMotr == mi.WSTG_TTble){if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprWait)) {sMsg = MT_GetName(mi.WSTG_YGrpr) + "is not in WaitPos.";bRet = false;}}
            else if(_eMotr == mi.WSTG_ZExpd){if(!MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprWait)) {sMsg = MT_GetName(mi.WSTG_YGrpr) + "is not in WaitPos.";bRet = false;}}
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
            if (!MT_GetStop(mi.WSTG_TTble)) return false;
            if (!MT_GetStop(mi.TOOL_XEjtL)) return false;
            if (!MT_GetStop(mi.TOOL_XEjtR)) return false;
            if (!MT_GetStop(mi.TOOL_YEjtr)) return false;
            if (!MT_GetStop(mi.TOOL_ZEjtr)) return false;
            if (!MT_GetStop(mi.WSTG_ZExpd)) return false;
            if (!MT_GetStop(mi.WSTG_YGrpr)) return false;
            if (!MT_GetStop(mi.WLDR_ZElev)) return false;
            return true;
        }
    };

    

   
    
}
