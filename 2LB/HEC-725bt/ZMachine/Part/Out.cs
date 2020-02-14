using System;
using COMMON;
using System.Runtime.CompilerServices;

namespace Machine
{
    public class Out : Part
    {
        public enum sc
        {
            Idle    = 0,
            Wind       , 
            Rewind     ,
            MAX_SEQ_CYCLE
        };

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

        public Out(int _iPartId)
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
                    Step.iToStart = 0;
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
                    Step.iToStop = 0;
                    return true;

                case 10:
                    Step.iToStop++;
                    return false;

                case 11:
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


        CDelayTimer DryRunTimer = new CDelayTimer();
        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;     
                
                bool bDryWind = false ;

                if(DryRunTimer.OnDelay(1000)){
                    bDryWind = OM.MstOptn.bIdleRun ;
                    DryRunTimer.Clear();

                }
                bool isCycleRewind =  OM.CmnOptn.bRewindMode ; //&& (GetFrntSensor() || GetRearSensor());
                bool isCycleWind   = GetFrntSensor() || GetRearSensor() || bDryWind ;
                bool isCycleEnd    = !isCycleWind  && !isCycleRewind ;
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleRewind  ) { Step.eSeq = sc.Rewind  ;  }
                else if (isCycleWind    ) { Step.eSeq = sc.Wind    ;  }
                else if (isCycleEnd      ) { Stat.bWorkEnd = true; return true; }
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
                default         : Trace("default End"); 
                                  Step.eSeq = sc.Idle;   return false;
                case sc.Idle    :                        return false;
                case sc.Rewind  : if (!CycleRewind  ())  return false; break ;
                case sc.Wind    : if (!CycleWind    ())  return false; break ;
            }

            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;

        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        //위에 부터 작업.
        public bool FindChip(int _iId, out int _iC, out int _iR, cs _iChipStat) 
        {
            
            //DM.ARAY[_iId].FindFrstRowCol(_iChipStat, ref _iC, ref _iR);
            //return (_iC >= 0 && _iR >= 0) ? true : false;
            _iC = 0; _iR = 0;
            return false ;
        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 30000 )) {
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
           
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;
            
                case 10:
                    MT_SetHomeDone(mi.OUT_TRelB , true);
                    MT_SetHomeDone(mi.OUT_TRelT , true);
                    MT_SetHomeDone(mi.OUT_YGuid , true);
                    MT_GoHome(mi.OUT_YGuid);
                    Step.iHome++;
                    return false ;

                case 11:
                    if(!MT_GetHomeDone(mi.OUT_YGuid)) return false ;
                    Step.iHome = 0;
                    return true;
            }
        }

        public bool GetFrntSensor()
        {
            if(OM.CmnOptn.bUseFrnt) return IO_GetX(xi.OUT_BtmSnsrF) ;
            return false ;
        }

        public bool GetRearSensor()
        {
            if(OM.CmnOptn.bUseRear) return IO_GetX(xi.OUT_TopSnsrR) ;
            return false ;
        }

        //가이드및 와인딩 동작.
        CDelayTimer StopDelay = new CDelayTimer();
        public void RunGuid(bool _bInit=false)
        {   
            if(_bInit) StopDelay.Clear();
            if (!MT_GetStop(mi.OUT_YGuid))
            {
                StopDelay.Clear();
                return;
            }

            double Rear = PM.GetValue(mi.OUT_YGuid,pv.OUT_YGuidRear);
            double Frnt = PM.GetValue(mi.OUT_YGuid,pv.OUT_YGuidFrnt);
            
            //스탑일때도 
            //bool bDelayStart = MT_GetStop(mi.OUT_YGuid) && (MT_CmprPos(mi.OUT_YGuid , PM_GetValue(mi.OUT_YGuid,pv.OUT_YGuidFrnt)) ||MT_CmprPos(mi.OUT_YGuid , PM_GetValue(mi.OUT_YGuid,pv.OUT_YGuidRear)));
            if(MT_CmprPos(mi.OUT_YGuid , PM_GetValue(mi.OUT_YGuid,pv.OUT_YGuidFrnt))){
                if(!StopDelay.OnDelay(OM.DevOptn.iGuidStopDealy)) return ;
                //MoveMotr(mi.OUT_YGuid,pv.OUT_YGuidRear);
                if(!OM.CmnOptn.bRewindMode) MT_GoAbsRun(mi.OUT_YGuid,pv.OUT_YGuidRear);
                else                        MT_GoAbsVel(mi.OUT_YGuid,Rear,OM.CmnOptn.dRewindYVel);
                OM.EqpStat.bOutGuideToRear = true ;
            }
            else if(MT_CmprPos(mi.OUT_YGuid , PM_GetValue(mi.OUT_YGuid,pv.OUT_YGuidRear))){//MT_GetCmdPos(mi.OUT_YGuid) == PM_GetValue(mi.OUT_YGuid,pv.OUT_YGuidRear)) {
                if(!StopDelay.OnDelay(OM.DevOptn.iGuidStopDealy)) return ;
                //MoveMotr(mi.OUT_YGuid,pv.OUT_YGuidFrnt);
                if(!OM.CmnOptn.bRewindMode) MT_GoAbsRun(mi.OUT_YGuid,pv.OUT_YGuidFrnt);
                else                        MT_GoAbsVel(mi.OUT_YGuid,Frnt,OM.CmnOptn.dRewindYVel);
                OM.EqpStat.bOutGuideToRear = false ;
            }
            else if(OM.EqpStat.bOutGuideToRear){
                //MoveMotr(mi.OUT_YGuid,pv.OUT_YGuidRear);
                if(!OM.CmnOptn.bRewindMode) MT_GoAbsRun(mi.OUT_YGuid,pv.OUT_YGuidRear);
                else                        MT_GoAbsVel(mi.OUT_YGuid,Rear,OM.CmnOptn.dRewindYVel);
            }
            else if(!OM.EqpStat.bOutGuideToRear){
                //MoveMotr(mi.OUT_YGuid,pv.OUT_YGuidFrnt);
                if(!OM.CmnOptn.bRewindMode) MT_GoAbsRun(mi.OUT_YGuid,pv.OUT_YGuidFrnt);
                else                        MT_GoAbsVel(mi.OUT_YGuid,Frnt,OM.CmnOptn.dRewindYVel);
            }
            
    }
        public void StopGuid()
        {
            MT_Stop(mi.OUT_YGuid);
        }

        CDelayTimer FrntDelay = new CDelayTimer();
        CDelayTimer RearDelay = new CDelayTimer();
        public bool CycleWind()//OUT_BtmSnsrF
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Trace(sTemp);

                MT_Stop(mi.OUT_TRelB);
                MT_Stop(mi.OUT_TRelT);

                StopGuid();
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
                MT_Stop(mi.OUT_TRelB);
                MT_Stop(mi.OUT_TRelT);

                StopGuid();
                return true ;
            }

            

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //RunGuid(true);
                    FrntDelay.Clear();
                    RearDelay.Clear();

                    if (OM.MstOptn.bIdleRun)
                    {
                        Step.iCycle = 20;
                        return false;

                    }

                    Step.iCycle++;
                    return false ;

                case 11:
                    


                    //감지 되어 있으면 켜고
                    if(GetFrntSensor()&&MT_GetStop(mi.OUT_TRelB)) {MT_JogP(mi.OUT_TRelB); }
                    if(GetRearSensor()&&MT_GetStop(mi.OUT_TRelT)) {MT_JogP(mi.OUT_TRelT); }

                    //감지 해제 되었으면 끄고.
                    if(!GetFrntSensor()&&!MT_GetStop(mi.OUT_TRelB)) {MT_Stop(mi.OUT_TRelB); }
                    if(!GetRearSensor()&&!MT_GetStop(mi.OUT_TRelT)) {MT_Stop(mi.OUT_TRelT); }

                    if(FrntDelay.OnDelay(GetFrntSensor(),5000))
                    {
                        ER_SetErr(ei.PKG_Dispr, "프론트 감기센서가 5초이상 계속 감지되었습니다.");
                        StopGuid();
                        MT_Stop(mi.OUT_TRelB);
                        MT_Stop(mi.OUT_TRelT);
                        Step.iCycle = 0;
                        return true;
                    }
                    if (RearDelay.OnDelay(GetRearSensor(), 5000))
                    {
                        ER_SetErr(ei.PKG_Dispr, "리어 감기센서가 5초이상 계속 감지되었습니다.");
                        StopGuid();
                        MT_Stop(mi.OUT_TRelB);
                        MT_Stop(mi.OUT_TRelT);
                        Step.iCycle = 0;
                        return true;
                    }


                    RunGuid();
                    if(GetFrntSensor() || GetRearSensor()) return false ;
                    Step.iCycle++;
                    return false ;

                case 12:
                    StopGuid();

                    //이미 스탑때리고 난후지만 확인사살.
                    MT_Stop(mi.OUT_TRelB);
                    MT_Stop(mi.OUT_TRelT);

                    Step.iCycle = 0;
                    return true;


                case 20:
                    MT_JogP(mi.OUT_TRelB);
                    MT_JogP(mi.OUT_TRelT);

                    m_tmDelay.Clear();
                    RunGuid();
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(500)) return false;

                    StopGuid();
                    //이미 스탑때리고 난후지만 확인사살.
                    MT_Stop(mi.OUT_TRelB);
                    MT_Stop(mi.OUT_TRelT);

                    Step.iCycle = 0;
                    return true;

            }
        }

        //오른쪽 실타레 -> 오른쪽 롤러 -> 왼쪽 롤러 -> 센서롤러 -> 왼쪽롤러2 -> 가이드 모터 -> 왼쪽 실타레
        public bool CycleRewind()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Trace(sTemp);
                MT_Stop(mi.OUT_YGuid);
                MT_Stop(mi.OUT_TRelT);
                MT_Stop(mi.OUT_TRelB);
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
                MT_Stop(mi.OUT_YGuid);
                MT_Stop(mi.OUT_TRelT);
                MT_Stop(mi.OUT_TRelB);
                return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //OM.CmnOptn.dRewindVel
                    //MT_goa
                    MT_JogP(mi.OUT_TRelT,OM.CmnOptn.dRewindRVel);
                    //RunGuid(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 11:
                    RunGuid();
                    return false;
                    //if (!m_tmDelay.OnDelay(5000)) return false;
                    //if(GetRearSensor() && IO_GetX(xi.OUT_TopSnsrR)) return false ;
                    Step.iCycle++;
                    return false ;

                case 12:
                    StopGuid();
                    MT_Stop(mi.OUT_TRelT);
                    MT_Stop(mi.OUT_YGuid);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            //if(_eActr == ci.INDX_DoorClOp){
            //    if(_eFwd == fb.Fwd) {
            //        if(CL_Complete(ci.INDX_TrayFeedFwBw, fb.Fwd))
            //        {
            //            sMsg = "Feeder가 전진해 있습니다.";
            //            bRet = false ;
            //        }
            //    }
            //}

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

            if(_eMotr == mi.OUT_TRelB){
            }
            else if(_eMotr == mi.OUT_TRelT){
            }
            else if(_eMotr == mi.OUT_YGuid){
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

            if (!MT_GetStop(mi.OUT_TRelB)) return false;
            if (!MT_GetStop(mi.OUT_TRelT)) return false;
            if (!MT_GetStop(mi.OUT_YGuid)) return false;

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
