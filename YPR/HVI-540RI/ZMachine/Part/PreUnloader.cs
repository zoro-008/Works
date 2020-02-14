using System;
using COMMON;
using System.Runtime.CompilerServices;

namespace Machine
{
    //PULD == PreUnloader
    public class PreUnloader : Part
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
            Idle              = 0,
            Work                 , //

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

        int    iWorkCnt ;

        public PreUnloader(int _iPartId = 0)
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
                    //CL_Move(ci.STCK_StackOpCl, fb.Fwd);
                    Step.iToStart++;
                    return false;
                
                case 11: 
                    //if (!CL_Complete(ci.STCK_StackOpCl)) return false;

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
                    
                    Step.iToStop++;
                    return false;
                
                case 11: 
                   
                    Step.iToStop++;
                    return false ;

                case 12:
                  
                    Step.iToStop++;
                    return false ;

                case 13:
                   
                    Step.iToStop++;
                    return false;

                case 14:
                   
                    Step.iToStop++;
                    return false;

                case 15:

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

                //if(SEQ.IDXF.GetSeqStep() != (int)IndexFront.sc.Out && SEQ.IDXR.GetSeqStep() != (int)IndexRear.sc.Out){
                //    if( DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) && IO_GetX(xi.RAIL_TrayDtct4)) {ER_SetErr(ei.PKG_Unknwn , "Picker Unknwn PKG Error."   ); return false;}
                //    if(!DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) &&!IO_GetX(xi.RAIL_TrayDtct4)) {ER_SetErr(ei.PKG_Dispr  , "Picker Disappear PKG Error."); return false;}
                //}
                //if( DM.ARAY[ri.TULD].CheckAllStat(cs.Unknown) &&!IO_GetX(xi.TBLE_ULDRPkgDtct)) {ER_SetErr(ei.PKG_Dispr   , "PreUnLoader Disappear PKG Error."); return false;}
                //if(!DM.ARAY[ri.TULD].CheckAllStat(cs.None   ) && IO_GetX(xi.TBLE_ULDRPkgDtct)) {ER_SetErr(ei.PKG_Unknwn  , "PreUnLoader Unknown PKG Error.  "); return false;}

                //if(DM.ARAY[ri.ULDR].CheckAllStat(cs.WorkEnd) && (!DM.ARAY[ri.TLDR].CheckAllStat(cs.None) ||
                //                                                 !DM.ARAY[ri.TVSN].CheckAllStat(cs.None) ||
                //                                                 !DM.ARAY[ri.TMRK].CheckAllStat(cs.None) ||
                //                                                 !DM.ARAY[ri.TULD].CheckAllStat(cs.None))){
                //                                                     ER_SetErr(ei.PKG_Supply, "UnLoader Bar Clear Plz.");
                //                                                     return false;
                //}
                //Inspection, Pick, Place
                bool isCycleWork           = DM.ARAY[ri.TULD].CheckAllStat(cs.Good) && DM.ARAY[ri.PULD].CheckAllStat(cs.None) && SEQ.TTBL.GetSeqStep() == (int)TurnTable.sc.Idle ;//&& OM.DevInfo.iWorkEndCnt > OM.EqpStat.iWorkCnt;
                bool isCycleEnd            = DM.ARAY[ri.TULD].CheckAllStat(cs.None) ;//DM.ARAY[ri.OUTZ].CheckAllStat(cs.None) && (DM.ARAY[ri.STCK].CheckAllStat(cs.None) || DM.ARAY[ri.STCK].CheckAllStat(cs.Empty));
                                   
                if (ER_IsErr()) return false;

                //Normal Decide Step.
                if (isCycleWork   ) { Step.eSeq  = sc.Work   ; }
                else if (isCycleEnd    ) { Stat.bWorkEnd = true; return true; }

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
                default         :                     Trace("default End");                                    Step.eSeq = sc.Idle;   return false;
                case sc.Idle    :                                                                                                     return false;
                case sc.Work    : if (CycleWork ()) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                
            }                               
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            r = 0 ;
            c = 0 ;
            DM.ARAY[_iId].FindFrstColLastRow((cs)_iChip, ref c, ref r);
            return (c >= 0 && r >= 0) ? true : false;
        }       

        public bool CycleHome()
        {
            string sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("Time Out Home Step.iHome={0:00}", Step.iHome);
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
                    CL_Move(ci.ULDR_GrpFwBw , fb.Fwd);
                    Step.iHome++;
                    return false ;

                case 11:
                    if (!CL_Complete  (ci.ULDR_GrpFwBw  , fb.Fwd)) return false;
                    CL_Move(ci.ULDR_RngGrpFwBw   , fb.Bwd);
                    Step.iHome++;
                    return false ;

                case 12:
                    if (!CL_Complete  (ci.ULDR_RngGrpFwBw  , fb.Bwd)) return false;
                    CL_Move(ci.ULDR_GrpRtrCwCCw  , fb.Bwd);
                    Step.iHome++;
                    return false;

                case 13:
                    if (!CL_Complete  (ci.ULDR_GrpRtrCwCCw , fb.Bwd)) return false;
                    Step.iHome++;
                    return false;

                case 14:
                    Step.iHome = 0;
                    return true ;
            }
        }

        public bool CycleWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + "" + Step.eSeq.ToString() + "" + sTemp;
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

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    if(!IO_GetX(xi.TBLE_ULDRPkgDtct)) {
                        ER_SetErr(ei.PKG_Dispr   , "PreUnLoader Disappear PKG Error.");
                        Step.iCycle = 0;
                        return true;
                    }
                    MoveCyl (ci.ULDR_GrpRtrCwCCw  , fb.Bwd   );
                    MoveCyl (ci.ULDR_RngGrpFwBw   , fb.Bwd   );
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.ULDR_GrpRtrCwCCw, fb.Bwd)) return false;
                    if (!CL_Complete(ci.ULDR_RngGrpFwBw , fb.Bwd)) return false;
                    MoveCyl(ci.ULDR_GrpFwBw , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CL_Complete(ci.ULDR_GrpFwBw , fb.Fwd))return false;
                    MoveCyl(ci.ULDR_RngGrpFwBw , fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!CL_Complete(ci.ULDR_RngGrpFwBw, fb.Fwd)) return false;
                    MoveCyl(ci.ULDR_GrpRtrCwCCw , fb.Fwd);
                    MoveCyl(ci.ULDR_GrpFwBw     , fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!CL_Complete(ci.ULDR_GrpRtrCwCCw, fb.Fwd))return false;
                    if(!CL_Complete(ci.ULDR_GrpFwBw    , fb.Bwd))return false;
                    DM.ShiftData((int)ri.TULD , (int)ri.PULD);
                    Step.iCycle = 0;
                    return true;                
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.ULDR_GrpFwBw){
                if(CL_GetCmd(ci.ULDR_RngGrpFwBw) != fb.Bwd && _eFwd == fb.Fwd) 
                {
                    sMsg = CL_GetName(ci.ULDR_RngGrpFwBw) + "is not Open";
                    bRet = false;
                }
            }
            
            else if(_eActr == ci.ULDR_GrpRtrCwCCw){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            
            else if (_eActr == ci.ULDR_RngGrpFwBw)
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

            //TOOL_ZVisn


            //if (_eMotr == mi.ULDR_XNzzl)
            //{

            
            //}
            //else if(_eMotr == mi.ULDR_XGrpr)
            //{

            //}
            //else if(_eMotr == mi.ULDR_YIndx)
            //{
            //    if(MT_GetCmdPos(mi.ULDR_ZNzzl) > PM.GetValue(mi.ULDR_ZNzzl , pv.ULDR_ZNzzlWait) + 10)
            //    {
            //        sMsg = MT_GetName(mi.ULDR_ZNzzl) + "is not wait Position";
            //        bRet = false;
            //    }
            //    if (CL_GetCmd(ci.ULDR_OutPshrFwBw) != fb.Bwd)
            //    {
            //        sMsg = CL_GetName(ci.ULDR_OutPshrFwBw) + "is not Bwd";
            //        bRet = false;
            //    }
            //}
            //else if(_eMotr == mi.ULDR_ZNzzl)
            //{

            //}
            //else if(_eMotr == mi.ULDR_ZPckr)
            //{

            //}
            //else
            //{
            //    sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
            //    bRet = false;
            //}
            
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
            if (!CL_Complete(ci.ULDR_RngGrpFwBw  )) return false;
            if (!CL_Complete(ci.ULDR_GrpRtrCwCCw )) return false;
            if (!CL_Complete(ci.ULDR_GrpFwBw     )) return false;



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
