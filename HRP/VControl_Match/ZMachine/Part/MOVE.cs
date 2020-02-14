using System;
using COMMON;
using System.Runtime.CompilerServices;
using VDll;

namespace Machine
{
    public class Move : Part
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
            Work       ,
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

        public const int iWLDRGetOfs = 1;
        public const int iWSTGGrprBwdOfs = -2;

        public bool m_bRun = false;


        public Move(int _iPartId = 0)
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
                Trace(sTemp);
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

                //if( DM.ARAY[ri.SPLR].CheckAllStat(cs.None) && IO_GetX(xi.RAIL_TrayDtct1)) {ER_SetErr(ei.PKG_Unknwn , "Supplyer Unknwn PKG Error."   ); return false;}
                //if(!OM.MstOptn.bIdleRun && !DM.ARAY[ri.SPLR].CheckAllStat(cs.None) &&!IO_GetX(xi.RAIL_TrayDtct1)) {ER_SetErr(ei.PKG_Dispr  , "Supplyer Disappear PKG Error."); return false;}

                //int iFWorkCol = DM.ARAY[ri.IDXF].FindFrstCol(cs.Good);
                //int iRWorkCol = DM.ARAY[ri.IDXR].FindFrstCol(cs.Good);

                //작업열을 세팅하여 미리서플라이 가능하게 함.
                //bool bIdxFCanSply = (iFWorkCol!=-1) && DM.ARAY[ri.IDXF].GetMaxCol() - iFWorkCol >= OM.DevOptn.iIdxCanSplyCol ;
                //bool bIdxRCanSply = (iRWorkCol!=-1) && DM.ARAY[ri.IDXR].GetMaxCol() - iRWorkCol >= OM.DevOptn.iIdxCanSplyCol ;
                //bool bIdxFSplyPos  = !DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && ML.MT_GetCmdPos(mi.IDXF_XFrnt) > OM.CmnOptn.dIdxFSplyPos;
                //bool bIdxRSplyPos  = !DM.ARAY[ri.IDXR].CheckAllStat(cs.None) && ML.MT_GetCmdPos(mi.IDXR_XRear) > OM.CmnOptn.dIdxRSplyPos;
                //
                ////인덱스 비어있음.
                
                //bool bIdxFNone     = DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && !IO_GetX(xi.RAIL_TrayDtct1);
                //bool bIdxRNone     = DM.ARAY[ri.IDXR].CheckAllStat(cs.None) && !IO_GetX(xi.RAIL_TrayDtct1);
                //bool bIdxFSplyStat = DM.ARAY[ri.IDXF].GetCntStat(cs.Empty) == DM.ARAY[ri.IDXF].GetMaxCol() * DM.ARAY[ri.IDXF].GetMaxRow() - DM.ARAY[ri.MASK].GetCntStat(cs.None) || DM.ARAY[ri.IDXF].GetCntStat(cs.Good) != 0 ; 
                //bool bIdxRSplyStat = DM.ARAY[ri.IDXR].GetCntStat(cs.Empty) == DM.ARAY[ri.IDXR].GetMaxCol() * DM.ARAY[ri.IDXR].GetMaxRow() - DM.ARAY[ri.MASK].GetCntStat(cs.None) || DM.ARAY[ri.IDXR].GetCntStat(cs.Good) != 0 ;                

                //int  iTopCoverCnt  = 1 ;
                //int  iBtmCoverCnt  = 
                bool isCycleWork = m_bRun;

                bool isCycleEnd = !m_bRun;                                   
                
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleWork  ) { Step.eSeq = sc.Work ;  }
                else if (isCycleEnd   ) { Stat.bWorkEnd = true; return true; }
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
                default         :                      Trace("default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle  ):                                                                                                      return false;
                case (sc.Work  ): if (CycleWork  ()) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        

        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            DM.ARAY[_iId].FindFrstColLastRow((cs)_iChip, ref c, ref r);
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
            return (c >= 0 && r >= 0) ? true : false;

            //return false;

        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
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
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;
            
                case 10:
                    MT_GoHome(mi.MOVE_X);
                    MT_GoHome(mi.MOVE_Y);
                    
                    Step.iHome++;
                    return false ;

                case 11:
                    if (!MT_GetHomeDone(mi.MOVE_X)) return false;
                    if (!MT_GetHomeDone(mi.MOVE_Y)) return false;

                    MoveMotr(mi.MOVE_X, pv.MOVE_XWait);
                    MoveMotr(mi.MOVE_Y, pv.MOVE_YWait);
                    Step.iHome++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.MOVE_X, pv.MOVE_XWait)) return false;
                    if (!MT_GetStopPos(mi.MOVE_Y, pv.MOVE_YWait)) return false;
                    //MT_GoHome(mi.MOVE_Y);
                    Step.iHome=0;
                    return true ;
            }
        }

        //public int iLDRSplyCnt = 0;
        public bool CycleWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 10000))
            {
                MT_ResetTrgPos(mi.MOVE_X);
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                ML.MT_SetY(mi.MOVE_X, 3, false);
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

            int r,c = -1;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    VL.Autorun(true);
                    VL.Ready(0);
                    
                    MoveMotr(mi.MOVE_Y, pv.MOVE_YWork);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.MOVE_Y, pv.MOVE_YWork)) return false;
                    //MoveMotr(mi.MOVE_X, pv.MOVE_XStart);
                    Step.iCycle++;
                    return false;

                case 12:
                    MoveMotr(mi.MOVE_X, pv.MOVE_XStart, -PM.GetValue(mi.MOVE_X, pv.MOVE_XTrgOfs));
                    
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.MOVE_X, pv.MOVE_XStart, -PM.GetValue(mi.MOVE_X, pv.MOVE_XTrgOfs))) return false;

                    //레이져 킴.
                    ML.MT_SetY(mi.MOVE_X, 3, true);

                    double dTrgDist = PM.GetValue(mi.MOVE_X , pv.MOVE_XTrgDist);
                    MT_ResetTrgPos(mi.MOVE_X);
                    //모터 25mm 검사영역 2.1mm/s 트리거500방
                    //아임아이 카메라 설정중에 트리거 필터를 50->0으로 해줘야 트리거Width 를 2로 넣을수 있음.
                    //50일땐 트리거 23까지는 안터지고 24부터는 터지긴 하나 놓침 25가 안전빵인듯
                    //어떻게 해도 속도 2.1mm/s 를 넘기지 못함.
                    MT_SetTrgBlock(mi.MOVE_X, PM.GetValue(mi.MOVE_X, pv.MOVE_XStart), PM.GetValue(mi.MOVE_X, pv.MOVE_XEnd)- dTrgDist, dTrgDist, 2 , false, true);//28
                    MT_GoAbsSlow(mi.MOVE_X, (PM.GetValue(mi.MOVE_X, pv.MOVE_XEnd) + PM.GetValue(mi.MOVE_X, pv.MOVE_XTrgOfs)));
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStop(mi.MOVE_X)) return false;
                    MT_ResetTrgPos(mi.MOVE_X);

                    //레이져 끔
                    ML.MT_SetY(mi.MOVE_X, 3, false);

                    m_bRun = false;

                    VL.Autorun(false);

                    Step.iCycle = 0;
                    return true;
            }
        }



        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            //if(_eActr == ci.LODR_GrpRtrCwCCw){
            //    //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
            //        //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
            //    //}
            //}
            //else if (_eActr == ci.LODR_GuideOpCl)
            //{

            //}
            //else if (_eActr == ci.LODR_PckrRtrCwCCw)
            //{

            //}
            //else if (_eActr == ci.LODR_PshrRtrCwCCw)
            //{
            //    if (MT_GetCmdPos(mi.LODR_XPshr) > PM.GetValue(mi.LODR_XPshr , pv.LODR_XPshrWait) + 10) 
            //    { 
            //        sMsg = MT_GetName(mi.LODR_XPshr) + "is not Wait Position";
            //        bRet = false;
            //    }
            //}
            //else if (_eActr == ci.LODR_RngGrpFwBw)
            //{

            //}
            //else if (_eActr == ci.LODR_PckrFwBw)
            //{
            //    if (_eFwd == fb.Fwd && CL_GetCmd(ci.LODR_RngJigFwBw) != fb.Fwd) 
            //    { 
            //        sMsg = CL_GetName(ci.LODR_RngJigFwBw) + "is not Fwd";
            //        bRet = false;
            //    }

            //}
            //else if (_eActr == ci.LODR_RngJigFwBw)
            //{
            //    if (_eFwd == fb.Bwd && CL_GetCmd(ci.LODR_PckrFwBw) != fb.Bwd)
            //    {
            //        sMsg = CL_GetName(ci.LODR_PckrFwBw) + "is Fwd";
            //        bRet = false;
            //    }

            //    if (CL_GetCmd(ci.LODR_GuideOpCl) != fb.Bwd)
            //    {
            //        sMsg = CL_GetName(ci.LODR_GuideOpCl) + "is not Bwd";
            //        bRet = false;
            //    }
            //}
            //else if(!_bChecked){
            //    sMsg = "Cylinder " + CL_GetName(_eActr) + " is Not this parts.";
            //    bRet = false;
            //}

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

            if (_eMotr == mi.MOVE_X)
            {
                //if (!MT_GetStopInpos(mi.WSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.WSTG_YGrpr) + "is moving.";
                //    bRet = false;
                //}
            }
            else if (_eMotr == mi.MOVE_Y)
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
            if (!MT_GetStop(mi.MOVE_X)) return false;
            if (!MT_GetStop(mi.MOVE_Y)) return false;

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
