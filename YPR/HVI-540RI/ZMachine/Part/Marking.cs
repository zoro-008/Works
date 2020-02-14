using System;
using COMMON;
using System.Runtime.CompilerServices;

namespace Machine
{
    public class Marking : Part
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

        public Marking(int _iPartId = 0)
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
                    IO_SetY(yi.MARK_Light , true);
                    MoveCyl(ci.MARK_AlgnFwBw   , fb.Bwd);
                    MoveCyl(ci.MARK_AlgnPinFwBw, fb.Bwd);
                    Step.iToStart = 0;
                    return true;
                    

                case 11:
                    if (!CL_Complete(ci.MARK_AlgnFwBw   , fb.Bwd)) return false;
                    if (!CL_Complete(ci.MARK_AlgnPinFwBw, fb.Bwd)) return false;

                    Step.iToStart++;
                    return false;

                case 12:
                    
                    //if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd)) return false ;
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
                    //MoveCyl(ci.SSTG_GrprClOp , fb.Bwd);
                    IO_SetY(yi.MARK_Light, false);
                    Step.iToStop++;
                    return false;

                case 11: 
                    //if(!CL_Complete(ci.SSTG_GrprClOp , fb.Bwd)) return false ;
                    //MoveMotr(mi.SSTG_YGrpr, pv.SSTG_YGrprWait);
                    
                    Step.iToStop++;
                    return false;

                case 12:
                    ////혹시라도 처박으면..
                    //if (IO_GetX(xi.SSTG_GrprOverload))
                    //{
                    //    MT_EmgStop(mi.SSTG_YGrpr);
                    //    ER_SetErr(ei.PRT_OverLoad, "Substrate");
                    //    return true;
                    //}
                    //if(!MT_GetStop(mi.SSTG_YGrpr)) return false ;
                    ////오버로드 에러가 없거나 있어도 Substrate Stage께 아닌경우에만
                    //bool bOverLoad  = ER_GetErrOn(ei.PRT_OverLoad) && ER_GetErrSubMsg(ei.PRT_OverLoad)!="Substrate";
                    //bool bMissed    = ER_GetErrOn(ei.PRT_Missed  ) && ER_GetErrSubMsg(ei.PRT_Missed  )!="Substrate";
                    //bool bBarcodeNG = ER_GetErrOn(ei.PRT_Barcode ) && ER_GetErrSubMsg(ei.PRT_Barcode )!="Substrate";
                    //if(!bOverLoad && !bMissed && !bBarcodeNG){
                    //    MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevWait);
                    //}

                    Step.iToStop++;
                    return false;
                
                case 13: 
                    //if(!MT_GetStopPos(mi.SLDR_ZElev,pv.SLDR_ZElevWait)) return false ;
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

        //public bool GetTrayAllGood()
        //{
        //    for (int c = 0; c < OM.DevInfo.iTRAY_PcktCntX; c++)
        //    {
        //        for (int r = 0; r < OM.DevInfo.iTRAY_PcktCntY; r++)
        //        {
        //            if (DM.ARAY[ri.MASK].GetStat(c, r) == cs.None) continue ;
        //            if (DM.ARAY[ri.IDXF].GetStat(c, r) != cs.Good) return false ;
        //        }
        //    }
        //    return true ;
        //}
        

        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                //if (IO_GetX(xi.IDXF_Overload))
                //{
                //    ER_SetErr(ei.PRT_OverLoad, "Front Index OverLoad Error.");
                //}

                bool isCycleWork    =!DM.ARAY[ri.TMRK].CheckAllStat(cs.None) && SEQ.TTBL.GetSeqStep() == (int)TurnTable.sc.Idle && DM.ARAY[ri.TMRK].Step == 0;// DM.ARAY[ri.MARK].CheckAllStat(cs.Unknown) && SEQ.TOOL.GetStep().eSeq != IndexRear.sc.Get && !OM.CmnOptn.bIdxFSkip;
                bool isCycleEnd     = DM.ARAY[ri.TMRK].CheckAllStat(cs.None) && DM.ARAY[ri.TMRK].GetCntStat(cs.Unknown) == 0 ;//(DM.ARAY[ri.IDXF].CheckAllStat(cs.None) ||(!DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && OM.CmnOptn.bGoldenTray)) ;//&& LOT.LotList.Count == 0;
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleWork    ) { Step.eSeq = sc.Work      ;  }
                else if (isCycleEnd     ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                //if( DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && IO_GetX(xi.IDXF_TrayDtct) && !isCycleGet && DM.ARAY[ri.SPLR].CheckAllStat(cs.None)) {ER_SetErr(ei.PKG_Unknwn , "FrontIndex Unknwn PKG Error."   ); return false;}  //CycleGet To Error.
                //if(!DM.ARAY[ri.IDXF].CheckAllStat(cs.None) &&!IO_GetX(xi.IDXF_TrayDtct)                                                         ) {ER_SetErr(ei.PKG_Dispr  , "FrontIndex Disappear PKG Error."); return false;}

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
                default           :                          Trace("default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle    ):                                                                                                          return false;
                case (sc.Work    ): if (CycleWork    ())   { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        //위에 부터 작업.
        //인덱스 데이터에서 작업해야 되는 컬로우를 뽑아서 리턴.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            DM.ARAY[_iId].FindLastColFrstRow((cs)_iChip, ref c, ref r);
            return true;
        }

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 30000 )) {
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
                //return true ;
            }

            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;

                case 10:
                    CL_Move(ci.MARK_AlgnFwBw   , fb.Bwd);
                    CL_Move(ci.MARK_AlgnPinFwBw, fb.Bwd);

                    Step.iHome++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.MARK_AlgnFwBw   , fb.Bwd)) return false;
                    if (!CL_Complete(ci.MARK_AlgnPinFwBw, fb.Bwd)) return false;
                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CycleWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 20000))
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
           
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:                  
                    
                    MoveCyl(ci.MARK_AlgnPinFwBw, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.MARK_AlgnPinFwBw, fb.Fwd)) return false;
                    MoveCyl(ci.MARK_AlgnFwBw, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!CL_Complete(ci.MARK_AlgnFwBw, fb.Fwd)) return false;
                    if(!OM.CmnOptn.bMarkSkip && DM.ARAY[ri.TMRK].CheckAllStat(cs.Unknown))SEQ.LaserMarking.CycleTrigger(true);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!OM.CmnOptn.bMarkSkip && !SEQ.LaserMarking.CycleTrigger() && DM.ARAY[ri.TMRK].CheckAllStat(cs.Unknown))return false;
                    if (OM.MstOptn.bMarkAlgin)
                    {
                        DM.ARAY[ri.TMRK].Step = 1;
                        if (!OM.CmnOptn.bMarkSkip) { 
                            if (SEQ.LaserMarking.GetCheckRslt()) DM.ARAY[ri.TMRK].SetStat(cs.Good);
                            else                                 DM.ARAY[ri.TMRK].SetStat(cs.NGMark);
                        }
                        else
                        {
                            if (DM.ARAY[ri.TMRK].CheckAllStat(cs.Unknown))
                            {
                                //1.0.1.5
                                //바코드 사용할때 레이져 스킵 상태이면
                                //바코드 사용 안하도록 하기 위해서 Step에 1 집어넣음.
                                DM.ARAY[ri.TMRK].SubStep = 1; 
                                DM.ARAY[ri.TMRK].SetStat(cs.Good);
                            }
                        }
                        Step.iCycle = 0;
                        return true;
                    }
                    
                    MoveCyl(ci.MARK_AlgnFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.MARK_AlgnFwBw, fb.Bwd)) return false;
                    MoveCyl(ci.MARK_AlgnPinFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.MARK_AlgnPinFwBw, fb.Bwd)) return false;
                    DM.ARAY[ri.TMRK].Step = 1;
                    if (!OM.CmnOptn.bMarkSkip && DM.ARAY[ri.TMRK].GetCntStat(cs.Unknown) != 0) { 
                        if (SEQ.LaserMarking.GetCheckRslt()) DM.ARAY[ri.TMRK].SetStat(cs.Good);
                        else                                 DM.ARAY[ri.TMRK].SetStat(cs.NGMark);
                    }
                    else
                    {
                        if (DM.ARAY[ri.TMRK].CheckAllStat(cs.Unknown))
                        {
                            DM.ARAY[ri.TMRK].SubStep = 1;
                            DM.ARAY[ri.TMRK].SetStat(cs.Good);
                        }
                            
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleManChange()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 20000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
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
           
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:                  
                    SEQ.LaserMarking.CycleProgramNo(OM.DevInfo.iProgramNo, true);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!SEQ.LaserMarking.CycleProgramNo(OM.DevInfo.iProgramNo))
                    {
                        return false;
                    }
                    else
                    {
                        Step.iCycle = 0;
                        return true;
                    }
                    
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.MARK_AlgnPinFwBw)
            {
            }
            else if (_eActr == ci.MARK_AlgnFwBw)
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

            //if (_eMotr == mi.IDXF_XFrnt)
            //{
            //    //if (!MT_GetStopInpos(mi.SSTG_YGrpr))
            //    //{
            //    //    sMsg = MT_GetName(mi.SSTG_YGrpr) + "is moving.";
            //    //    bRet = false;
            //    //}
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
            if (!CL_Complete(ci.MARK_AlgnFwBw   ))return false;
            if (!CL_Complete(ci.MARK_AlgnPinFwBw))return false;

            return true ;
        }
        //public void SaveLotName(string _sLotName)
        //{
        //    string sLotDataPath = "C:\\Data\\LotName.ini";
        //    CIniFile IniLotDatadSave = new CIniFile(sLotDataPath);

        //    IniLotDatadSave.Save("LotName", "LotName", _sLotName);

        //}

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
