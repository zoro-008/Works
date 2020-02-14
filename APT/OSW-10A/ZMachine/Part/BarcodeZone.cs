using COMMON;
using System;
using SML2;

namespace Machine
{
    //PCK == Picker
    public class BarcodeZone : Part
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
            BarPick             , //
            BarPlace            , //
            BarRead             , //바코드 에러시 다시 읽기.
            Out                 ,
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

        //double m_dDiePosOfsX ;
        //double m_dDiePosOfsY ;

        const int iVisnDelay  = 100 ;
        const double dDispCheckOfs = -5.0;

        public BarcodeZone()
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
                    IO_SetY(yi.BARZ_BrcdAC, false);
                    MoveCyl(ci.BARZ_YPckrFwBw, fb.Bwd);
                    Step.iToStop++;
                    return false;
                
                case 11:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Bwd)) return false;
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iToStop++;
                    return false ;

                case 12:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove)) return false;
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

                //if( DM.ARAY[ri.PCKR].CheckAllStat(cs.None) && IO_GetX(xi.TOOL_PckrVac)) {ER_SetErr(ei.PKG_Unknwn , "Picker Unknwn PKG Error."   ); return false;}
                //if(!DM.ARAY[ri.PCKR].CheckAllStat(cs.None) &&!IO_GetX(xi.TOOL_PckrVac)) {ER_SetErr(ei.PKG_Dispr  , "Picker Disappear PKG Error."); return false;}

                //bool isCycleBarPick     = DM.ARAY[ri.BARZ].CheckAllStat(cs.Unknown) ; //20180209 바코드 붙이기 스킵 추가 하면서 타이밍 문제 때문에 스탁존 아웃 사이클 끝날때를 기다림.
                bool isCycleBarPick     = DM.ARAY[ri.BARZ].CheckAllStat(cs.Unknown) && (SEQ.STCK.GetSeqStep() != (int)Stacker.sc.Out);
                bool isCycleBarPlace    = DM.ARAY[ri.BARZ].CheckAllStat(cs.Barcode); //IO_GetX(xi.BARZ_PckrBrcdDtct);//아.. 데이터만들기 귀찮아. //DM.ARAY[ri.BARZ].CheckAllStat(cs.Barcode);
                bool isCycleRead        = DM.ARAY[ri.BARZ].CheckAllStat(cs.BarRead);
                bool isCycleOut         = DM.ARAY[ri.BARZ].CheckAllStat(cs.WorkEnd);
                bool isCycleEnd         = DM.ARAY[ri.BARZ].CheckAllStat(cs.None   );// && IO_GetX(xi.ETC_BendingOut);
                                              
                                            
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleBarPick     ) { Step.eSeq  = sc.BarPick    ; }
                else if (isCycleBarPlace    ) { Step.eSeq  = sc.BarPlace   ; }
                else if (isCycleRead        ) { Step.eSeq  = sc.BarRead    ; }
                else if (isCycleOut         ) { Step.eSeq  = sc.Out        ; }
                else if (isCycleEnd         ) { Stat.bWorkEnd = true; return true; }

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
                default            :                         Log.Trace(m_sPartName, "default End");                                    Step.eSeq = sc.Idle;   return false;
                case sc.Idle       :                                                                                                                          return false;
                case sc.BarPick    : if (CycleBarPick  ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.BarPlace   : if (CycleBarPlace ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.BarRead    : if (CycleBarRead  ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Out        : if (CycleOut      ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }                               
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

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 25000))
            {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
                //Step.iHome = 0 ;GetCrntCycleName(         )
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
                    if (!IO_GetX(xi.BARZ_PckrVac))
                    {
                        IO_SetY(yi.BARZ_PckrVac, false);
                    }
                    IO_SetY(yi.BARZ_Blower, false);
                    CL_Move(ci.BARZ_YPckrFwBw, fb.Bwd);
                    Step.iHome++;
                    return false ;

                case 11:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Bwd)) return false;
                    MT_GoHome(mi.BARZ_ZPckr);
                    
                    Step.iHome++;
                    return false ;

                case 12:
                    if (!MT_GetHomeDone(mi.BARZ_ZPckr)) return false;
                    MT_GoAbsRun(mi.BARZ_ZPckr, PM.GetValue(mi.BARZ_ZPckr, pv.BARZ_ZPckrWait));
                    MT_GoHome(mi.BARZ_XPckr);
                    Step.iHome++;
                    return false ;

                case 13:
                    if (!MT_GetHomeDone(mi.BARZ_XPckr)) return false;
                    MT_GoAbsRun(mi.BARZ_XPckr, PM.GetValue(mi.BARZ_XPckr, pv.BARZ_XPckrWait));
                    Step.iHome++;
                    return false;

                case 14:
                    if(!MT_GetStopInpos(mi.BARZ_ZPckr))return false;
                    if(!MT_GetStopInpos(mi.BARZ_XPckr))return false;
                    Step.iHome = 0;
                    return true ;
            }
        }

        
        int iDummyBarPickCnt = 0 ;
        public bool CycleBarPick()
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
                    if(OM.CmnOptn.bSkipBarAttach)
                    {
                        DM.ARAY[ri.BARZ].SetStat(cs.WorkEnd);
                        Step.iCycle = 0 ;
                        return true ;
                    }



                    if (IO_GetX(xi.BARZ_PckrBrcdDtct))
                    {
                        MoveCyl(ci.BARZ_YPckrFwBw, fb.Bwd);
                        Step.iCycle = 200 ;
                        //ER_SetErr(ei.PRT_Detect, "Picker Detecting");
                        return false;
                    }                    
                    if(!OM.CmnOptn.bIdleRun) {
                        if(SEQ.BarcordPrnt.Stat.isPauseFlag    ){ER_SetErr(ei.ETC_BarcPrint, "Barcode Printer Pausing"    );return true;}
                        if(SEQ.BarcordPrnt.Stat.isPaperout     ){ER_SetErr(ei.ETC_BarcPrint, "Barcode Printer PaperOut"   );return true;}
                        if(SEQ.BarcordPrnt.Stat.isRibonoutFlag ){ER_SetErr(ei.ETC_BarcPrint, "Barcode Printer RibbonOut"  );return true;}
                        if(SEQ.BarcordPrnt.Stat.isHeadupFlag   ){ER_SetErr(ei.ETC_BarcPrint, "Barcode Printer HeadUp"     );return true;}
                        if(SEQ.BarcordPrnt.Stat.isLabelWaitFlag){//바코드가 나와 있으면 띠어서 버퍼에 붙이는 동작.                            
                            Step.iCycle = 100;
                            return false;
                        }      
                      
                        /*0015110851021008879110009
                        Stat.sTrayLabel_TrayLabel     = _sTrayLabel ;
                        Stat.sTrayLabel_MaterialNo    = _sTrayLabel.Substring(4,8);
                        Stat.sTrayLabel_GroupingNo    = _sTrayLabel.Substring(0,4);
                        Stat.sTrayLabel_BatchNo       = _sTrayLabel.Substring(12,10);
                        Stat.sTrayLabel_SecurityCode  = _sTrayLabel.Substring(22,3);                         
                         */    
                        int    iGrouping  = 0 ;
                        if (!int.TryParse(SEQ.Oracle.Stat.sTrayLabel_GroupingNo,out iGrouping))
                        {

                            
                        }
                        string sGrouping  = string.Format("{0:0000}",iGrouping);
                        ////int iPkgCnt = OM.DevInfo.iTRAY_PcktCntX *  OM.DevInfo.iTRAY_PcktCntX * OM.DevInfo.iTRAY_PcktCntY ;
                        //int iRealTrayCnt = OM.DevInfo.iTRAY_StackingCnt;
                        //iRealTrayCnt-- ; //Top CoverTray 
                        //if(OM.DevOptn.bUseBtmCover)iRealTrayCnt--;
                        //int iQty = OM.DevInfo.iTRAY_PcktCntX * OM.DevInfo.iTRAY_PcktCntY * iRealTrayCnt ;
                        SEQ.BarcordPrnt.PrintBar2(OM.CmnOptn.iBarcYOffset,
                                                  sGrouping                                    , 
                                                  SEQ.Oracle.Stat.sTrayLabel_MaterialNo        , 
                                                  SEQ.Oracle.Stat.sTrayLabel_BatchNo           ,  
                                                  "999"                                        ,  //여기까지는 바코드 내용.
                                                  SEQ.Oracle.Stat.sTrayInfomation_DeviceNumber , 
                                                  SEQ.Oracle.Stat.sLotTraveler_LotAlias        , 
                                                  SEQ.Oracle.Stat.sTrayInfomation_Bin          , 
                                                  OM.CmnOptn.iBarcToff.ToString()              );
                              
                        OM.EqpStat.sPrintedBarcode = sGrouping + SEQ.Oracle.Stat.sTrayLabel_MaterialNo + SEQ.Oracle.Stat.sTrayLabel_BatchNo + "999" ;


                    }

                    Step.iCycle++;
                    return false ;

                case 11:
                    MoveCyl(ci.BARZ_YPckrFwBw, fb.Bwd);
                    
                    Step.iCycle++;
                    return false ;

                case 12:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Bwd)) return false;
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove)) return false;
                    MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrPick);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!MT_GetStopPos(mi.BARZ_XPckr, pv.BARZ_XPckrPick)) return false;
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw);
                    Step.iCycle++;
                    return false;
                    
                case 15:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw)) return false;
                    MoveCyl(ci.BARZ_YPckrFwBw, fb.Fwd);
                    
                    Step.iCycle++;
                    return false ;

                case 16:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Fwd)) return false;
                    MoveMotrSlow(mi.BARZ_ZPckr, pv.BARZ_ZPckrPick);
                    
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrPick)) return false;
                    IO_SetY(yi.BARZ_PckrVac, true);
                    IO_SetY(yi.BARZ_Blower , true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 18:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iBrcdPickDelay)) return false;
                    
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw);
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw)) return false;
                    MoveCyl(ci.BARZ_YPckrFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Bwd)) return false;
                    IO_SetY(yi.BARZ_Blower , false);
                    if(OM.CmnOptn.bIdleRun){
                        IO_SetY(yi.BARZ_PckrVac , false);
                    }
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove)) return false;
                    
                    if (!OM.CmnOptn.bIdleRun && !IO_GetX(xi.BARZ_PckrVac))
                    {
                        ER_SetErr(ei.BAR_PickMiss, "Barcode Pick Missed");
                        IO_SetY(yi.BARZ_PckrVac , false);
                        IO_SetY(yi.BARZ_Blower  , false);
                        return true ;
                    }


                    Step.iCycle++;
                    return false;

                case 22:
                    DM.ARAY[ri.BARZ].SetStat(cs.Barcode);
                    if (OM.EqpStat.iBrcdRemoveCnt >= 50)
                    {
                        ER_SetErr(ei.PRT_RemovePkg, "Barcode Print Remove Count is over than 50!");
                        OM.EqpStat.iBrcdRemoveCnt = 0;
                    }
                    
                    Step.iCycle=0;
                    return true;


                //위에서 씀.
                //바코드 라벨 붙어 있을경우 제거 하여 쓴다.
                //픽에러는 무시.
               
                case 100:
                    iDummyBarPickCnt = 0 ;
                    MoveCyl(ci.BARZ_YPckrFwBw, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 101:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Bwd)) return false;
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 102:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove)) return false;
                    MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrPick);
                    Step.iCycle++;
                    return false;

                //밑에서씀 바코드픽리트라이.
                case 103:
                    if (!MT_GetStopPos(mi.BARZ_XPckr, pv.BARZ_XPckrPick)) return false;
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw);
                    Step.iCycle++;
                    return false;

                case 104:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw)) return false;
                    MoveCyl(ci.BARZ_YPckrFwBw, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 105:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Fwd)) return false;
                    MoveMotrSlow(mi.BARZ_ZPckr, pv.BARZ_ZPckrPick);

                    Step.iCycle++;
                    return false;

                case 106:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrPick)) return false;
                    IO_SetY(yi.BARZ_PckrVac, true);
                    IO_SetY(yi.BARZ_Blower, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 107:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iBrcdPickDelay)) return false;
                    
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw);
                    Step.iCycle++;
                    return false;

                case 108:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw)) return false;
                    Step.iCycle++;
                    return false ;


                case 109:
                    MoveCyl(ci.BARZ_YPckrFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                
                case 110:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Bwd)) return false;

                    if(SEQ.BarcordPrnt.Stat.isLabelWaitFlag){//바코드가 나와 있으면 띠어서 버퍼에 붙이는 동작.
                        iDummyBarPickCnt++;
                        if (iDummyBarPickCnt < 3) { 
                            Step.iCycle = 103;
                            return false;
                        }
                        SM.ER_SetErr(ei.BAR_PickMiss , "Dummy Barcode Remove Picking Failed!");
                    }   


                    IO_SetY(yi.BARZ_Blower, false);
                    //원래는 Z축 올리고 X축 가야하지만 지금 포지션상 그러면 뻘짓임...
                    MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrRemove);
                    Step.iCycle++;
                    return false;

                case 111:
                    if(!MT_GetStopPos(mi.BARZ_XPckr , pv.BARZ_XPckrRemove))return false;
                    

                    MoveMotrSlow(mi.BARZ_ZPckr, pv.BARZ_ZPckrRemove);
                    Step.iCycle++;
                    return false;

                case 112:
                    if(!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrRemove))return false;

                    IO_SetY(yi.BARZ_PckrVac, false);
                    OM.EqpStat.iBrcdRemoveCnt += 1;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 113:
                    if(!m_tmDelay.OnDelay(500))return false;
                    MoveMotr(mi.BARZ_ZPckr , pv.BARZ_ZPckrCylFw);                    
                    Step.iCycle++;
                    return false;

                case 114:
                    if(!MT_GetStopPos(mi.BARZ_ZPckr , pv.BARZ_ZPckrCylFw))return false;
                    Step.iCycle = 0;
                    return true;



                //픽커에 스티커가 있을경우.
                case 200:
                    MoveCyl(ci.BARZ_YPckrFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;
                                    
                case 201:
                    if (!CL_Complete(ci.BARZ_YPckrFwBw, fb.Bwd)) return false;
                    MoveMotr(mi.BARZ_ZPckr , pv.BARZ_ZPckrMove );
                    Step.iCycle++;
                    return false ;

                case 202:
                    if(!MT_GetStopPos(mi.BARZ_ZPckr , pv.BARZ_ZPckrMove))return false;
                    IO_SetY(yi.BARZ_Blower, false);
                    //원래는 Z축 올리고 X축 가야하지만 지금 포지션상 그러면 뻘짓임...
                    MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrRemove);
                    Step.iCycle++;
                    return false;

                case 203:
                    if(!MT_GetStopPos(mi.BARZ_XPckr , pv.BARZ_XPckrRemove))return false;                   

                    MoveMotrSlow(mi.BARZ_ZPckr, pv.BARZ_ZPckrRemove);
                    Step.iCycle++;
                    return false;

                case 204:
                    if(!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrRemove))return false;

                    IO_SetY(yi.BARZ_PckrVac, false);
                    OM.EqpStat.iBrcdRemoveCnt += 1;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 205:
                    if(!m_tmDelay.OnDelay(500))return false;
                    MoveMotr(mi.BARZ_ZPckr , pv.BARZ_ZPckrCylFw);                    
                    Step.iCycle++;
                    return false;

                case 206:
                    if(!MT_GetStopPos(mi.BARZ_ZPckr , pv.BARZ_ZPckrCylFw))return false;
                    Step.iCycle = 0;
                    return true;

                    
            }
        }

        public bool CycleBarPlace()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 20000))
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
                    //if (!IO_GetX(xi.BARZ_PckrVac))
                    //{
                    //    ER_SetErr(ei.PCK_PickMiss, "Picker Vacuum Error");
                    //    return true;
                    //}
                    if(OM.DevOptn.bUseBarcCyl) MoveCyl(ci.BARZ_BrcdTrayUpDn, fb.Fwd);
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(OM.DevOptn.bUseBarcCyl && !CL_Complete(ci.BARZ_BrcdTrayUpDn, fb.Fwd))return false;
                    if(!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove))return false;
                    MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrPlace);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.BARZ_XPckr, pv.BARZ_XPckrPlace)) return false;
                    if(!OM.CmnOptn.bIdleRun) {
                        MoveMotrSlow(mi.BARZ_ZPckr, pv.BARZ_ZPckrPlaceCheck);
                    }
                    else {
                        MoveMotrSlow(mi.BARZ_ZPckr, pv.BARZ_ZPckrPlaceCheck , -30 );
                        Step.iCycle=15;
                        return false;
                    }
                    
                    Step.iCycle++;
                    return false;

                case 13:
                    if(IO_GetX(xi.BARZ_PckrBrcdDtct)){
                        MT_Stop(mi.BARZ_ZPckr);
                        Step.iCycle= 15;
                        return false ; //여기는 감지되어서 멈춘경우.
                    }
                    if(!MT_GetStop(mi.BARZ_ZPckr))return false;
                    //여기는 감지 안되서 멈춘경우.
                    
                    MoveMotr(mi.BARZ_ZPckr , pv.BARZ_ZPckrWait);
                    Step.iCycle++;
                    return false;

                case 14: 
                    if(!MT_GetStop(mi.BARZ_ZPckr))return false;
                    ER_SetErr(ei.PRT_Barcode , "tray check sensor not detected tray.");                    
                    return true ;

                //위에서 2군데 씀.
                case 15:
                    if(!MT_GetStop(mi.BARZ_ZPckr))return false;
                    double dPlaceOfs = PM_GetValue(mi.BARZ_ZPckr, pv.BARZ_ZPckrPlaceOfs);

                    MT_GoIncSlow(mi.BARZ_ZPckr, dPlaceOfs);

                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStop(mi.BARZ_ZPckr)) return false;
                    IO_SetY(yi.BARZ_PckrVac, false);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!m_tmDelay.OnDelay(500)) return false ;
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove)) return false;
                    MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrBarc);
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!MT_GetStopPos(mi.BARZ_XPckr, pv.BARZ_XPckrBarc)) return false;
                    DM.ARAY[ri.BARZ].SetStat(cs.BarRead);
                    Step.iCycle=0;
                    return false ;
            }
        }

        public bool CycleBarRead()
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
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove)) return false;
                    MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrBarc);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!MT_GetStopPos(mi.BARZ_XPckr, pv.BARZ_XPckrBarc)) return false;
                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrBarc);
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrBarc)) return false;
                    if(!OM.CmnOptn.bIdleRun) {
                        SEQ.BarcordBARZ.SendScanOn();
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (m_tmDelay.OnDelay(2000))
                    {
                        ER_SetErr(ei.PRT_Barcode, "Barcode Scan Failed!");
                        return true;
                    }
                    if(SEQ.BarcordBARZ.GetText() == "" && !OM.CmnOptn.bIdleRun) return false ;
                    if (!OM.CmnOptn.bIdleRun && SEQ.BarcordBARZ.GetText() != OM.EqpStat.sPrintedBarcode)
                    {
                        ER_SetErr(ei.PRT_Barcode, "Barcode is not '"+ OM.EqpStat.sPrintedBarcode + "'");
                        return true;
                    }

                    MoveMotr(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove);
                    Step.iCycle++;
                    return false;

                //아이들시에 위에서씀.
                case 15:
                    if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove)) return false;
                    MoveMotr(mi.BARZ_XPckr, pv.BARZ_XPckrWait);
                    Step.iCycle++;
                    return false;

                case 16:
                    //if (!MT_GetStopPos(mi.BARZ_XPckr, pv.BARZ_XPckrWait)) return false;
                    DM.ARAY[ri.BARZ].SetStat(cs.WorkEnd);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleOut()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000 + OM.CmnOptn.iLotEndDelay))
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
                    if(IO_GetX(xi.ETC_BandingOut)){
                        ER_SetErr(ei.PKG_Unknwn , "There is a DevicePkg on the Bending Machine"   );
                        return true ;
                    }  
                    MoveCyl(ci.BARZ_BrcdTrayUpDn, fb.Bwd);
                    MoveCyl(ci.BARZ_BrcdStprUpDn, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!CL_Complete(ci.BARZ_BrcdTrayUpDn, fb.Bwd)) return false;
                    if (!CL_Complete(ci.BARZ_BrcdStprUpDn, fb.Bwd)) return false;
                    IO_SetY(yi.BARZ_BrcdAC, true);
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!IO_GetX(xi.BARZ_TrayOutDtct) && !OM.CmnOptn.bIdleRun) return false;
                    m_tmDelay.Clear();

                    OM.EqpStat.iWorkBundle++;
                    if(OM.EqpStat.iWorkBundle >= OM.DevInfo.iTRAY_BundleCnt){
                        //HZ7290XH98_006_170928_143116P
                        int iRealTrayCnt = OM.DevInfo.iTRAY_StackingCnt;
                        iRealTrayCnt-- ; //Top CoverTray 
                        if(OM.DevOptn.bUseBtmCover)iRealTrayCnt--;
                        int iQty = OM.DevInfo.iTRAY_PcktCntX * OM.DevInfo.iTRAY_PcktCntY * iRealTrayCnt ;
                        iQty *= OM.EqpStat.iWorkBundle ;
                        if (!OM.CmnOptn.bOracleNotWriteVITFile && OM.CmnOptn.sVITFolder != "") { 
                            SEQ.Oracle.WriteVIT(OM.CmnOptn.sVITFolder                  , 
                                                DateTime.Now.ToString("MM\\/dd\\/yyyy"),
                                                OM.CmnOptn.sMachinID                   ,
                                                LOT.CrntLotData.sEmployeeID            ,  //20180125 SML.FrmLogOn.GetId()                   ,
                                                OM.EqpStat.sLotSttTime                 ,
                                                DateTime.Now.ToString("HH:mm:ss")      ,
                                                iQty.ToString());
                        }
                    }

                    Step.iCycle++;
                    return false;
                
                case 13:
                    if(!m_tmDelay.OnDelay(OM.CmnOptn.iLotEndDelay))return false;
                    IO_SetY(yi.BARZ_BrcdAC, false);
                    
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!OM.CmnOptn.bIdleRun){
                        OM.EqpStat.bWrapingEnd = true ;
                    }
                    DM.ARAY[ri.BARZ].SetStat(cs.None);
                    Step.iCycle = 0;
                    return true;
            }
        }
 
        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.BARZ_BrcdStprUpDn){

            }
            
            else if(_eActr == ci.BARZ_BrcdTrayUpDn){

            }
            
            else if(_eActr == ci.BARZ_YPckrFwBw){
                if(_eFwd == fb.Fwd) {
                    if (MT_GetCmdPos(mi.BARZ_ZPckr) < SM.PM_GetValue(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw)-1)
                    {
                        sMsg = MT_GetName(mi.BARZ_ZPckr) + "'s Pos is Less than 'Picker Y Cyl Fwd'Pos";
                        bRet = false ;
                    }
                    if (MT_GetCmdPos(mi.BARZ_ZPckr) > SM.PM_GetValue(mi.BARZ_ZPckr, pv.BARZ_ZPckrPick)+1)
                    {
                        sMsg = MT_GetName(mi.BARZ_ZPckr) + "'s Pos is Bigger than 'Picker Pick'Pos";
                        bRet = false ;
                    }
                    if (MT_GetCmdPos(mi.BARZ_XPckr) < SM.PM_GetValue(mi.BARZ_XPckr, pv.BARZ_ZPckrCylFw)-1)
                    {
                        sMsg = MT_GetName(mi.BARZ_XPckr) + "'s Pos is not in 'Picker Y Cyl Fwd'Pos";
                        bRet = false ;
                    }
                    if (MT_GetCmdPos(mi.BARZ_XPckr) > SM.PM_GetValue(mi.BARZ_XPckr, pv.BARZ_ZPckrCylFw)+1)
                    {
                        sMsg = MT_GetName(mi.BARZ_XPckr) + "'s Pos is not in 'Picker Y Cyl Fwd'Pos";
                        bRet = false ;
                    }
                }
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


            if (_eMotr == mi.BARZ_XPckr)
            {
                //if (!MT_GetStopPos(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove) && MT_GetEncPos(mi.BARZ_ZPckr) > SM.PM_GetValue(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove))
                if (!SM.MT_GetStop(mi.BARZ_ZPckr))
                {
                    sMsg = MT_GetName(mi.BARZ_ZPckr) + " is moving.";
                    bRet = false;
                }
                if (MT_GetCmdPos(mi.BARZ_ZPckr) > SM.PM_GetValue(mi.BARZ_ZPckr, pv.BARZ_ZPckrMove))
                {
                    sMsg = MT_GetName(mi.BARZ_ZPckr) + "'s Pos is Bigger than 'Picker Move'Pos";
                    bRet = false;
                }
                
                if (CL_GetCmd(ci.BARZ_YPckrFwBw)==fb.Fwd)
                {
                    sMsg = CL_GetName(ci.BARZ_YPckrFwBw) + " is Fwd.";
                    bRet = false;
                }
            
            }
            
            else if (_eMotr == mi.BARZ_ZPckr)
            {
                if (CL_GetCmd(ci.BARZ_YPckrFwBw)==fb.Fwd)
                {

                    if (SM.PM_GetValue(_eMotr, _ePstn) > SM.PM_GetValue(mi.BARZ_ZPckr, pv.BARZ_ZPckrPick) + 1)
                    {
                        sMsg = CL_GetName(ci.BARZ_YPckrFwBw) + " is Fwd.";
                        bRet = false;
                    }
                    if (SM.PM_GetValue(_eMotr, _ePstn) < SM.PM_GetValue(mi.BARZ_ZPckr, pv.BARZ_ZPckrCylFw) - 1)
                    {
                        sMsg = CL_GetName(ci.BARZ_YPckrFwBw) + " is Fwd.";
                        bRet = false;
                    }
                    
                    //if (MT_GetCmdPos(mi.BARZ_ZPckr) < +1)
                        
                        
                }

                //BARZ_ZPckrMove //BARZ_ZPckrCylFw //BARZ_ZPckrPick
                
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
            if (!MT_GetStop(mi.BARZ_XPckr)) return false;
            if (!MT_GetStop(mi.BARZ_ZPckr)) return false;

            if (!CL_Complete(ci.BARZ_BrcdStprUpDn)) return false;
            if (!CL_Complete(ci.BARZ_BrcdTrayUpDn)) return false;
            if (!CL_Complete(ci.BARZ_YPckrFwBw   )) return false;            

            return true;
        }
    };
    


    

   
    
}
