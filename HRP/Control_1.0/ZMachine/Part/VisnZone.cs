using System;
using COMMON;
using System.Runtime.CompilerServices;

namespace Machine
{
    //VISN = visionZone
    public class VisionZone : Part
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
            Work                , //
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

        //VisnCom.TRslt RsltDieAlign     ;
        //VisnCom.TRslt RsltSubsAlign    ;
        //VisnCom.TRslt RsltBtmAlign     ;
        //VisnCom.TRslt RsltRightDist    ;
        //VisnCom.TRslt RsltLeftDist     ;
        LOT.TLot LotData;

        double m_dDiePosOfsX ;
        double m_dDiePosOfsY ;

        const int iVisnDelay  = 100 ;
        const double dDispCheckOfs = -5.0;

        public VisionZone(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;
            
            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();

            m_CycleTime   = new CTimer[(int)sc.MAX_SEQ_CYCLE];
            LotData = new LOT.TLot();

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
                Trace(sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: Step.iToStart = 0;
                    return true;

                case 10:
                    MoveCyl(ci.VISN_FixRtrCwCCw , fb.Bwd);
                    MoveCyl(ci.VISN_GrpRtrCwCCw , fb.Bwd);
                    MoveCyl(ci.VISN_TurnGrpFwBw , fb.Bwd);
            
                    Step.iToStart++;
                    return false;
                
                case 11: 
                    if(!CL_Complete(ci.VISN_FixRtrCwCCw , fb.Bwd))return false;
                    if(!CL_Complete(ci.VISN_GrpRtrCwCCw , fb.Bwd))return false;
                    if(!CL_Complete(ci.VISN_TurnGrpFwBw , fb.Bwd))return false;
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
                Trace(sTemp);
            }

            PreStep.iToStop = Step.iToStop;
            Stat.bReqStop = false;            

            //Move Home.
            switch (Step.iToStop)
            {
                default: Step.iToStop = 0;
                    return true;

                case 10:
                    //MoveCyl(ci.BARZ_YPckrFwBw, fb.Bwd);
                    Step.iToStop++;
                    return false;
                
                case 11:
                    //if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Bwd)) return false;
                    //MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iToStop++;
                    return false ;

                case 12:
                    //if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove)) return false;
                    //MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrWait);
                    Step.iToStop++;
                    return false ;

                case 13:
                    //if (!MT_GetStopPos(mi.BARZ_XPckr, pv.BARZ_XPckrWait)) return false;
                    Step.iToStop = 0;
                    return true;

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

                //if( DM.ARAY[ri.TVSN].CheckAllStat(cs.Unknown) && IO_GetX(xi.TBLE_VISNPkgDtct)) {ER_SetErr(ei.PKG_Dispr   , "Vision Disappear PKG Error."); return false;}
                //if(!DM.ARAY[ri.TVSN].CheckAllStat(cs.None   ) &&!IO_GetX(xi.TBLE_VISNPkgDtct)) {ER_SetErr(ei.PKG_Unknwn  , "Vision Unknown PKG Error.  "); return false;}
                
                bool isCycleWork     = DM.ARAY[ri.TVSN].CheckAllStat(cs.Unknown) && SEQ.TTBL.GetSeqStep() == (int)TurnTable.sc.Idle;
                bool isCycleEnd      =!DM.ARAY[ri.TVSN].CheckAllStat(cs.Unknown) && DM.ARAY[ri.TVSN].CheckAllStat(cs.None);
                                              
                                            
                                   
                if (ER_IsErr()) return false;
                     if (isCycleWork        ) { Step.eSeq  = sc.Work    ; }
                else if (isCycleEnd         ) { Stat.bWorkEnd = true; return true; }

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
                default            : Trace("default End"); Step.eSeq = sc.Idle;   return false;
                case sc.Idle       : return false;
                case sc.Work       : if (!CycleWork    ()) return false; break;
            }                
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;                           
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            r = 0 ;
            c = 0 ;
            DM.ARAY[_iId].FindFrstRowCol(_iChip, ref c , ref r);             
            return (c >= 0 && r >= 0) ? true : false;
        }

        int iRepeatCnt = 0;
        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 25000))
            {
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
                //Step.iHome = 0;
                //return true ;
            }
            
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    if(Step.iHome != PreStep.iHome) Trace(sTemp);
                    return true ;
            
                case 10:
                    iRepeatCnt = 0;
                    //CL_Move(ci.VISN_FixRtrCwCCw , fb.Bwd);
                    //CL_Move(ci.VISN_GrpRtrCwCCw , fb.Bwd);
                    //CL_Move(ci.VISN_TurnGrpFwBw , fb.Bwd);
                    //CL_Move(ci.VISN_TurnRtrCwCCw, fb.Bwd);
                    Step.iHome++;
                    return false ;

                case 11:
                    //if (!CL_Complete(ci.VISN_FixRtrCwCCw , fb.Bwd)) return false;
                    //if (!CL_Complete(ci.VISN_GrpRtrCwCCw , fb.Bwd)) return false;
                    //if (!CL_Complete(ci.VISN_TurnGrpFwBw , fb.Bwd)) return false;
                    //if (!CL_Complete(ci.VISN_TurnRtrCwCCw, fb.Bwd)) return false;
                    MT_GoHome(mi.VISN_ZGrpr);
                    
                    Step.iHome++;
                    return false ;

                case 12:
                    if (!MT_GetHomeDone(mi.VISN_ZGrpr)) return false;
                    MT_GoAbsRun(mi.VISN_ZGrpr, PM.GetValue(mi.VISN_ZGrpr, pv.VISN_ZGrprWait));
                    Step.iHome++;
                    return false;

                case 13:
                    if (!MT_GetStopInpos(mi.VISN_ZGrpr)) return false;
                    MoveCyl(ci.VISN_TurnRtrCwCCw, fb.Bwd);
                    Step.iHome++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.VISN_TurnRtrCwCCw, fb.Bwd)) return false;
                    MoveCyl(ci.VISN_TurnRtrCwCCw, fb.Fwd);
                    Step.iHome++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.VISN_TurnRtrCwCCw, fb.Fwd)) return false;
                    if (iRepeatCnt < 3)
                    {
                        iRepeatCnt++;
                        Step.iHome = 13;
                        return false;
                    }
                    Step.iHome = 0;
                    return true;
                    
            }
        }

        
        public bool CycleWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
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

            int r = 0, c = 0;


            const int iMaxInspCnt = 4 ;

            string sVsnRecvMsg;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //MoveCyl(ci.VISN_TurnGrpFwBw  , fb.Bwd);
                    if(!IO_GetX(xi.TBLE_VISNPkgDtct)) {
                        ER_SetErr(ei.PKG_Dispr   , "Vision Disappear PKG Error.");
                        Step.iCycle = 0;
                        return true;
                    }
                    if (!OM.CmnOptn.bVisnSkip) VC.SendVisnMsg(VC.sm.Ready);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!OM.CmnOptn.bVisnSkip && !VC.IsEndSendMsg()) return false;
                    if (!OM.CmnOptn.bVisnSkip && VC.GetVisnRecvErrMsg() != "")
                    {
                        ER_SetErr(ei.VSN_ComErr , VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }
                    if (!OM.CmnOptn.bVisnSkip && VC.GetVisnSendMsg() != "OK")
                    {
                        ER_SetErr(ei.VSN_ComErr , "Vision Not Ready");
                        Step.iCycle=0 ;
                        return true ;
                    }
                    if(!OM.CmnOptn.bVisnSkip) VC.ClearRecvData();
                    MoveCyl(ci.VISN_GrpRtrCwCCw  , fb.Bwd);
                    MoveCyl(ci.VISN_FixRtrCwCCw  , fb.Bwd);
                    //MoveCyl(ci.VISN_TurnRtrCwCCw , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    //if(!CL_Complete(ci.VISN_TurnGrpFwBw , fb.Bwd))return false;
                    if(!CL_Complete(ci.VISN_GrpRtrCwCCw , fb.Bwd))return false;
                    if(!CL_Complete(ci.VISN_FixRtrCwCCw , fb.Bwd))return false;
                    //if(!CL_Complete(ci.VISN_TurnRtrCwCCw))return false;
                    MoveCyl(ci.VISN_TurnGrpFwBw, fb.Fwd);
                    
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.VISN_TurnGrpFwBw , fb.Fwd))return false;
                    MoveCyl(ci.VISN_GrpRtrCwCCw , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.VISN_GrpRtrCwCCw , fb.Fwd))return false;
                    MoveMotr(mi.VISN_ZGrpr, pv.VISN_ZGrprWork);
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!MT_GetStopInpos(mi.VISN_ZGrpr))return false;
                    MoveCyl(ci.VISN_FixRtrCwCCw , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.VISN_FixRtrCwCCw , fb.Fwd))return false;
                    if(!OM.CmnOptn.bVisnSkip) VC.SendVisnMsg(VC.sm.Insp);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!OM.CmnOptn.bVisnSkip && VC.GetVisnSendMsg() == "") return false;
                    Step.iCycle++;
                    return false;

                case 18:
                    CL_Move(ci.VISN_FixRtrCwCCw , fb.Bwd);
                    
                    Step.iCycle++;
                    return false;

                case 19:
                    if(!CL_Complete(ci.VISN_FixRtrCwCCw , fb.Bwd))return false;
                    //if (!OM.CmnOptn.bVisnSkip) VC.SendVisnMsg(VC.sm.Insp);
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!OM.CmnOptn.bVisnSkip)
                    {
                        sVsnRecvMsg = VC.GetVisnSendMsg();

                        if (sVsnRecvMsg == "1")
                        {
                            Step.iCycle = 24;
                            return false;
                        }
                        //if (sVsnRecvMsg == "00002")
                        //{
                        //    Step.iCycle = 24;
                        //    return false;
                        //}
                        
                    }
                    if (OM.CmnOptn.bVisnSkip){
                        Step.iCycle = 24;
                        return false;
                    }
                    Step.iCycle++;
                    return false;

                case 21:
                    CL_Move(ci.VISN_GrpRtrCwCCw , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!CL_Complete(ci.VISN_GrpRtrCwCCw, fb.Bwd)) return false;
                    //CL_Move(ci.VISN_TurnGrpFwBw , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 23:
                    //if(!CL_Complete(ci.VISN_TurnGrpFwBw , fb.Bwd))return false;

                    sVsnRecvMsg = VC.GetVisnSendMsg();
                    //검사 Good일때
                    if (sVsnRecvMsg == "0") {
                        DM.ARAY[ri.TVSN].SetStat(cs.Good);
                    }
                    //검사 실패시.
                    else if(sVsnRecvMsg == "2"){
                        DM.ARAY[ri.TVSN].SetStat(cs.NGVisn);
                    }
                    if (OM.CmnOptn.bVisnSkip) DM.ARAY[ri.TVSN].SetStat(cs.Good);
                    Step.iCycle = 0;
                    return true;

                case 24:
                    MoveMotr(mi.VISN_ZGrpr , pv.VISN_ZGrprWait);
                    Step.iCycle++;
                    return false;

                case 25:
                    if(!MT_GetStopInpos(mi.VISN_ZGrpr))return false;
                    if(CL_GetCmd(ci.VISN_TurnRtrCwCCw) == fb.Bwd){
                        CL_Move(ci.VISN_TurnRtrCwCCw , fb.Fwd);
                    }
                    else{
                        CL_Move(ci.VISN_TurnRtrCwCCw , fb.Bwd);
                    }
                    
                    Step.iCycle++;
                    return false;

                case 26:
                    //여기는 한번 확인.
                    if(!CL_Complete(ci.VISN_TurnRtrCwCCw))return false;
                    CL_Move(ci.VISN_GrpRtrCwCCw , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 27:
                    if (!CL_Complete(ci.VISN_GrpRtrCwCCw, fb.Bwd)) return false;
                    DM.ARAY[ri.TVSN].SetStat(cs.Good);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.VISN_FixRtrCwCCw){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            
            else if(_eActr == ci.VISN_GrpRtrCwCCw){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            
            else if(_eActr == ci.VISN_TurnGrpFwBw){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else if(_eActr == ci.VISN_TurnRtrCwCCw){
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

            //TOOL_ZVisn


            if (_eMotr == mi.VISN_ZGrpr)
            {
                
            
            }
            
            //else if (_eMotr == mi.BARZ_ZPckr)
            //{
            //    //if (CL_GetCmd(ci.BARZ_YPckrFwBw)==fb.Fwd)
            //    //{
            //    //    sMsg = CL_GetName(ci.BARZ_YPckrFwBw) + " is Fwd.";
            //    //    bRet = false;
            //    //}
                
            //}
            
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
            if (!MT_GetStop(mi.VISN_ZGrpr)) return false;

            if (!CL_Complete(ci.VISN_FixRtrCwCCw )) return false;
            if (!CL_Complete(ci.VISN_GrpRtrCwCCw )) return false;
            if (!CL_Complete(ci.VISN_TurnGrpFwBw )) return false;
            if (!CL_Complete(ci.VISN_TurnRtrCwCCw)) return false;

            return true;
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
