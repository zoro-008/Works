using COMMON;
using System;



namespace Machine
{
    public class Loader : Part
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
            Supply     ,
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
        public const int iWSTGGrprBwdOfs = -2;


        public Loader()
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
                 
                    Step.iToStart++;
                    return false;

                case 11: 
              
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
              
                    Step.iToStop++;
                         return false;

                case 11: 
                    
                    Step.iToStop++;
                    return false ;

                case 12:
                    
                    Step.iToStop++;
                    return false;

                case 13:
                   
                    Step.iToStop++;
                    return false;
                
                case 14: 
                    
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

        //현재 장비안에 있는 트레이 갯수.
        public int GetCrntTrayCnt()
        {
            int iSTCKCnt = DM.ARAY[ri.STCK].GetCntStat  (cs.Good) ;
            int iPSTCCnt = DM.ARAY[ri.PSTC].GetCntStat  (cs.Good) ;
            int iOUTZCnt = DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) ? 0 : 1 ;
            int iIDXRCnt = DM.ARAY[ri.IDXR].CheckAllStat(cs.None) ? 0 : 1 ;
            int iIDXFCnt = DM.ARAY[ri.IDXF].CheckAllStat(cs.None) ? 0 : 1 ;
            int iSPLRCnt = DM.ARAY[ri.SPLR].CheckAllStat(cs.None) ? 0 : 1 ;

            int iTotalCnt = iSTCKCnt + iPSTCCnt + iOUTZCnt + iIDXRCnt + iIDXFCnt + iSPLRCnt ;
            return iTotalCnt ;

        }

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

                //if( DM.ARAY[ri.SPLR].CheckAllStat(cs.None) && IO_GetX(xi.RAIL_TrayDtct1)) {ER_SetErr(ei.PKG_Unknwn , "Supplyer Unknwn PKG Error."   ); return false;}
                if(!OM.CmnOptn.bIdleRun && !DM.ARAY[ri.SPLR].CheckAllStat(cs.None) &&!IO_GetX(xi.RAIL_TrayDtct1)) {ER_SetErr(ei.PKG_Dispr  , "Supplyer Disappear PKG Error."); return false;}

                //공급 부족 에러.
                if (!OM.CmnOptn.bIdleRun && !OM.CmnOptn.bGoldenTray) { 
                    if ((!IO_GetX(xi.LODR_TrayDtct) && OM.GetSupplyCnt() < OM.DevInfo.iTRAY_StackingCnt) && LOT.LotOpened && !OM.EqpStat.bWrapingEnd)
                    {
                        ER_SetErr(ei.PRT_NeedTraySupply, "Supply Tray");
                    }
                }
                //int iFWorkCol = DM.ARAY[ri.IDXF].FindFrstCol(cs.Good);
                //int iRWorkCol = DM.ARAY[ri.IDXR].FindFrstCol(cs.Good);

                //작업열을 세팅하여 미리서플라이 가능하게 함.
                //bool bIdxFCanSply = (iFWorkCol!=-1) && DM.ARAY[ri.IDXF].GetMaxCol() - iFWorkCol >= OM.DevOptn.iIdxCanSplyCol ;
                //bool bIdxRCanSply = (iRWorkCol!=-1) && DM.ARAY[ri.IDXR].GetMaxCol() - iRWorkCol >= OM.DevOptn.iIdxCanSplyCol ;
                bool bIdxFSplyPos  = !DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && SM.MT_GetCmdPos(mi.IDXF_XFrnt) > OM.CmnOptn.dIdxFSplyPos;
                bool bIdxRSplyPos  = !DM.ARAY[ri.IDXR].CheckAllStat(cs.None) && SM.MT_GetCmdPos(mi.IDXR_XRear) > OM.CmnOptn.dIdxRSplyPos;

                //인덱스 비어있음.
                bool bIdxFNone     = DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && !IO_GetX(xi.RAIL_TrayDtct1);
                bool bIdxRNone     = DM.ARAY[ri.IDXR].CheckAllStat(cs.None) && !IO_GetX(xi.RAIL_TrayDtct1);
                bool bIdxFSplyStat = DM.ARAY[ri.IDXF].GetCntStat(cs.Empty) == DM.ARAY[ri.IDXF].GetMaxCol() * DM.ARAY[ri.IDXF].GetMaxRow() - DM.ARAY[ri.MASK].GetCntStat(cs.None) || DM.ARAY[ri.IDXF].GetCntStat(cs.Good) != 0 ; 
                bool bIdxRSplyStat = DM.ARAY[ri.IDXR].GetCntStat(cs.Empty) == DM.ARAY[ri.IDXR].GetMaxCol() * DM.ARAY[ri.IDXR].GetMaxRow() - DM.ARAY[ri.MASK].GetCntStat(cs.None) || DM.ARAY[ri.IDXR].GetCntStat(cs.Good) != 0 ;                

                //int  iTopCoverCnt  = 1 ;
                //int  iBtmCoverCnt  = 
                bool isCycleSupply =  DM.ARAY[ri.SPLR].CheckAllStat(cs.None) && !OM.CmnOptn.bLoadingStop && !OM.EqpStat.bWrapingEnd &&
                                     (IO_GetX(xi.LODR_TrayDtct)|| OM.CmnOptn.bIdleRun) && //로더에 자제 확인 하여.
                                     ((OM.GetSupplyCnt() < OM.DevInfo.iTRAY_StackingCnt && !OM.CmnOptn.bGoldenTray) || OM.CmnOptn.bIdleRun || (OM.GetSupplyCnt() == 0 && OM.CmnOptn.bGoldenTray))&&
                                     ((bIdxRNone && bIdxFSplyStat && bIdxFSplyPos) || (bIdxFNone && bIdxRSplyStat && bIdxRSplyPos) || (bIdxRNone && bIdxFNone)) ;


                bool isCycleEnd    = DM.ARAY[ri.SPLR].CheckAllStat(cs.None) && (OM.EqpStat.bWrapingEnd|| OM.CmnOptn.bGoldenTray) ;//;

                

                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleSupply) { Step.eSeq = sc.Supply ;  }
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
                default         :                      Log.Trace(m_sPartName, "default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle  ):                                                                                                                       return false;
                case (sc.Supply): if (CycleSupply()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        

        //밑에 부터 작업.
        public bool FindChip(out int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            //if(-1 != DM.ARAY[ri.WLDT].FindLastRow(_iChip)){
            //    _iId = ri.WLDT ;
            //    r  = DM.ARAY[ri.WLDT].FindLastRow(_iChip) ;
            //    return true ;
            //}
            //else if(-1 != DM.ARAY[ri.WLDB].FindLastRow(_iChip)){
            //    _iId = ri.WLDB ;
            //    r  = DM.ARAY[ri.WLDB].FindLastRow(_iChip) ;
            //    return true ;
            //}
            //if(_iChip == cs.RetFail) {
            //    DM.ARAY[ri.WSTG].FindLastRowCol(cs.Empty , ref c , ref r);
            //    _iId = ri.WSTG ;
            //    if(r<0 || c<0){
            //        r=0;
            //        c=0;
            //    }
            //    else {                    
            //        if (c+1 <= OM.DevInfo.iWFER_DieCntX)
            //        {
            //            if(r < OM.DevInfo.iWFER_DieCntY-1) {
            //                r++;
            //            } 
            //            else {
            //                r = OM.DevInfo.iWFER_DieCntY-1;
            //            }
            //            
            //            c = 0;
            //        }
            //        else
            //        {
            //            c++;
            //        }
            //    }
            //    return true ;
            //}
            
            //나머지는 해당 칩을 찾아서 리턴.
            //DM.ARAY[ri.WSTG].FindLastRowCol(_iChip , ref c , ref r);
            //_iId = ri.WSTG ;
            //return (c >= 0 && r >= 0) ? true : false;
            return false;

        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
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
                    CL_Move(ci.LODR_SperatorUpDn, fb.Bwd);
                    Step.iHome++;
                    return false ;
            
                case 11:
                    if (!CL_Complete(ci.LODR_SperatorUpDn, fb.Bwd)) return false;
                    CL_Move(ci.LODR_ClampClOp, fb.Bwd);
                    
                    Step.iHome++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.LODR_ClampClOp, fb.Bwd)) return false;
                    MT_GoHome(mi.LODR_ZLift);
                    
                    Step.iHome++;
                    return false ;

                case 13:
                    if (!MT_GetHomeDone(mi.LODR_ZLift)) return false;
                    MT_GoAbsRun(mi.LODR_ZLift, PM.GetValue(mi.LODR_ZLift, pv.LODR_ZLiftWait));
                    Step.iHome++;
                    return false;
                    //if(DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && DM.ARAY[ri.IDXR].CheckAllStat(cs.None) &&
                    //   DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) && DM.ARAY[ri.SPLR].CheckAllStat(cs.None))
                    //{
                    //    iLDRSplyCnt = 0;   
                    //}
                    //else
                    //{
                    //    iLDRSplyCnt = OM.EqpStat.iLDRSplyCnt;
                    //}

                case 14:
                    if(!MT_GetStopInpos(mi.LODR_ZLift))return false;
                    Step.iHome = 0;
                    return true ;
            }
        }

        //public int iLDRSplyCnt = 0;
        public bool CycleSupply()
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

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;
                case 10:
                    //if (!IO_GetX(xi.LODR_TrayDtct) && LOT.LotList.Count != 0 && !OM.MstOptn.bIdleRun)
                    if (!IO_GetX(xi.LODR_TrayDtct) && !OM.CmnOptn.bIdleRun)
                    {
                        ER_SetErr(ei.PKG_Supply, "There is No Tray in the Loader");
                        return true;
                    }

                    MoveMotr(mi.LODR_ZLift, pv.LODR_ZLiftWait);
                    MoveCyl (ci.LODR_ClampClOp, fb.Bwd       );
                    MoveCyl (ci.LODR_SperatorUpDn, fb.Bwd    );

                    if(DM.ARAY[ri.IDXF].CheckAllStat(cs.None)) SEQ.IDXF.MoveCyl(ci.IDXF_ClampUpDn , fb.Bwd);
                    if(DM.ARAY[ri.IDXR].CheckAllStat(cs.None)) SEQ.IDXR.MoveCyl(ci.IDXR_ClampUpDn , fb.Bwd);


                    Step.iCycle++;
                    return false ;

                case 11:
                    if (!MT_GetStopPos(mi.LODR_ZLift, pv.LODR_ZLiftWait)) return false;
                    if (!CL_Complete  (ci.LODR_ClampClOp, fb.Bwd       )) return false;
                    if (!CL_Complete  (ci.LODR_SperatorUpDn, fb.Bwd    )) return false;
                    MoveMotr(mi.LODR_ZLift, pv.LODR_ZLiftPick);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.LODR_ZLift, pv.LODR_ZLiftPick)) return false;
                    MoveCyl(ci.LODR_ClampClOp, fb.Fwd);
                    MoveCyl(ci.LODR_SperatorUpDn, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.LODR_ClampClOp, fb.Fwd)) return false;
                    if(!CL_Complete(ci.LODR_SperatorUpDn, fb.Fwd)) return false ;
                    MoveMotr(mi.LODR_ZLift, pv.LODR_ZLiftSperate);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.LODR_ZLift, pv.LODR_ZLiftSperate)) return false;
                    MoveCyl(ci.LODR_SperatorUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.LODR_SperatorUpDn, fb.Bwd)) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!m_tmDelay.OnDelay(true, 1000))return false;
                    MoveMotr(mi.LODR_ZLift, pv.LODR_ZLiftPlace);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if (!MT_GetStopPos(mi.LODR_ZLift, pv.LODR_ZLiftPlace)) return false;
                    MoveCyl (ci.LODR_ClampClOp, fb.Bwd       );
                    Step.iCycle++;
                    return false ;

                case 18:
                    if (!CL_Complete  (ci.LODR_ClampClOp, fb.Bwd       )) return false;
                    MoveMotr(mi.LODR_ZLift, pv.LODR_ZLiftWait);
                    
                    Step.iCycle++;
                    return false;

                case 19:
                    
                    if (!MT_GetStopPos(mi.LODR_ZLift, pv.LODR_ZLiftWait)) return false;
                    m_tmDelay.Clear();

                    
                    DM.ARAY[ri.SPLR].SetStat(cs.Unknown);

                    //OM.EqpStat.iLDRSplyCnt++; 

                    Step.iCycle++;
                    return false;

                case 20:
                    if(!m_tmDelay.OnDelay(true, 1000))return false;



                    if (!IO_GetX(xi.RAIL_TrayDtct1) && !OM.CmnOptn.bIdleRun)
                    {
                        ER_SetErr(ei.PKG_Supply, "The Tray On the Suppyer is Slanted");
                        return true;
                    }
                    
                    

                    Step.iCycle++;
                    return false;

                case 21:

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.LODR_ClampClOp){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if (_eActr == ci.LODR_SperatorUpDn)
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

            if (_eMotr == mi.LODR_ZLift)
            {
                //if (!MT_GetStopInpos(mi.WSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.WSTG_YGrpr) + "is moving.";
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
            if (!MT_GetStop(mi.LODR_ZLift)) return false;

            if (!CL_Complete(ci.LODR_ClampClOp   )) return false;
            if (!CL_Complete(ci.LODR_SperatorUpDn)) return false;
            //if (!MT_GetStop(mi.WSTG_TTble)) return false;
            //if (!MT_GetStop(mi.TOOL_XEjtL)) return false;
            //if (!MT_GetStop(mi.TOOL_XEjtR)) return false;
            //if (!MT_GetStop(mi.TOOL_YEjtr)) return false;
            //if (!MT_GetStop(mi.TOOL_ZEjtr)) return false;
            //if (!MT_GetStop(mi.WSTG_ZExpd)) return false;
            //if (!MT_GetStop(mi.WSTG_YGrpr)) return false;
            //if (!MT_GetStop(mi.WLDR_ZElev)) return false;
            return true;
        }
    };

    

   
    
}
