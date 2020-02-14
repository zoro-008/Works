using COMMON;

namespace Machine
{
    class MM
    {
        private static int             m_iManStep , m_iPreManStep ;
        
        private static mc m_iManNo   ;

        private static CDelayTimer     m_tmCycle   = new CDelayTimer() ; //메뉴얼 싸이클 타임아웃용.
        private static CDelayTimer     m_tmDelay   = new CDelayTimer(); //메뉴얼 싸이클

        private static bool            m_bManSetting ; //쎄팅중임.
        private static bool            m_bStdCalPick ; //Picker StdCal한 후 Pick 동작 하기 위해.
        public  static int             m_iWorkNo; 
        public  static int             m_iNodeNo;

        public  static int             m_iSolderDelay;

        public static mc m_iCrntManNo   ;

        //private double          m_dVisnRsltGapX ;
        //private double          m_dVisnRsltGapY ;
        //private double          m_dVisnRsltGapT ;

        public static void Init()
        {
            
            m_iManStep    = 0;
            m_iPreManStep = 0;
            m_iManNo      = mc.NoneCycle;

            m_bManSetting = false;

            m_tmCycle.Clear();
            m_tmDelay.Clear();

            m_iWorkNo = 0;

            

        }

        
        public static void Reset(){Init();}


        //Manual Processing.
        public static bool SetManCycle(mc _iNo)
        {
            m_iCrntManNo = m_iManNo;

            if (_iNo     < 0                                 ) { Log.ShowMessage("ERROR", "Wrong Manual No"   ); return false; }
            if (_iNo     >= mc.MAX_MANUAL_CYCLE              ) { Log.ShowMessage("ERROR", "Wrong Manual No"   ); return false; }
            if (m_iManNo != mc.NoneCycle                     ) { Log.ShowMessage("ERROR", "Doing Manual Cycle"); return false; }
            if (SEQ._bRun                                    ) { Log.ShowMessage("ERROR", "Autorunning"       ); return false; }
            if (_iNo > mc.XRAY_Home && !SEQ.InspectHomeDone()) {                                                 return false; }

            //LOL if (!SML.IO.GetX((int)xi.ETC_MainAir)            ) { Log.ShowMessage("ERROR", "Main Air is Not Supply"); return false; }

            //Check Alarm.
            //LOL if (SML.ER.IsErr()) { Init(); return false; } //아 밑에 처리 하는 애 때문에 잠시 이렇게 함.          //test

            //    if(!IO_GetX(xETC_MainPower) ) {FM_MsgOk("ERR","Power On Plz");      return false ;} //test
            //if (!ML.IO.GetX((int)EN_INPUT_ID.xETC_MainAirChk)) { MessageBox.Show("Check Main Air!", "ERROR"); return false; } //test
            //   mcLDR_RHome
            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    //   mcIDX_Home
            m_iManNo = _iNo;     
            
            //   mcLTL_Home
            //   mcRTL_Home

            //ML.ER.SetDisp(true);jinseop

            bool bRet = true;                                                                                                                //   mcSTG_Home 
            //   mcULD_Home
            /********************/ 
            /********************/
            if      (m_iManNo == mc.NoneCycle         ) { bRet = false; }
            else if (m_iManNo == mc.AllHome           ) { }
            else if (m_iManNo == mc.INDX_Home         ) { }
            else if (m_iManNo == mc.XRAY_Home         ) { }
            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                ML.MT_SetServoAll(true);
                SEQ.INDX.InitHomeStep();
                     if (OM.DevInfo.iMacroType == 0) SEQ.XRYD.InitHomeStep();
                else if (OM.DevInfo.iMacroType == 1) SEQ.XRYE.InitHomeStep();
                
            }
            else if (m_iManNo == mc.INDX_Home             ) {ML.MT_SetServoAll(true);SEQ.INDX.InitHomeStep(); }
            else if (m_iManNo == mc.XRAY_Home && OM.DevInfo.iMacroType == 0) {ML.MT_SetServoAll(true);SEQ.XRYD.InitHomeStep(); }
            else if (m_iManNo == mc.XRAY_Home && OM.DevInfo.iMacroType == 1) {ML.MT_SetServoAll(true);SEQ.XRYE.InitHomeStep(); }
                     
            else if (m_iManNo == mc.INDX_CycleGet         ) {SEQ.INDX.InitCycleStep();                        }
            else if (m_iManNo == mc.INDX_CycleBarcode     ) {SEQ.INDX.InitCycleStep();                        }
            else if (m_iManNo == mc.INDX_CycleOut         ) {SEQ.INDX.InitCycleStep();                        }

            else if (m_iManNo == mc.XRAY_CycleConnect     ) {SEQ.XRYD.InitCycleStep();                        }
            else if (m_iManNo == mc.XRAY_CycleReady       ) {SEQ.XRYD.InitCycleStep();                        }
            else if (m_iManNo == mc.XRAY_CycleWork        ) {SEQ.XRYD.InitCycleStep();                        }
            else if (m_iManNo == mc.XRAY_CycleAnalyze     ) {SEQ.XRYD.InitCycleStep();                        }
            else if (m_iManNo == mc.XRAY_CycleCheck       ) {SEQ.XRYD.InitCycleStep();                        }

            else if (m_iManNo == mc.XRAY_CycleManMacro    ) { m_iManStep = 10;                                }

            else if (m_iManNo == mc.INDX_LdrPitchUp       ) {ML.MT_GoIncMan(mi.LODR_ZElev,   OM.DevInfo.dLODR_SlotPitch);}
            else if (m_iManNo == mc.INDX_LdrPitchDn       ) {ML.MT_GoIncMan(mi.LODR_ZElev,  -OM.DevInfo.dLODR_SlotPitch);}
            else if (m_iManNo == mc.INDX_IdxPitchLeft     ) {ML.MT_GoIncMan(mi.INDX_XRail,   OM.DevInfo.dTRAY_PcktPitchX);}
            else if (m_iManNo == mc.INDX_IdxPitchRight    ) {ML.MT_GoIncMan(mi.INDX_XRail,  -OM.DevInfo.dTRAY_PcktPitchX);}

            //else if (m_iManNo == mc.XRAY_CycleAging       )SEQ.INDX.InitCycleStep(); {SEQ.XRAY.InitCycleStep();                        }
                                               
            //else if (m_iManNo == mc.USBC_CycleConnect     ) {SEQ.USBC.InitCycleStep();                        }

            m_bManSetting = false; //m_bManSetting 중요함 리턴전에 꼭 펄스 시켜야함. 쓰레드가 달라서. ::Update에서 m_iManNo=0이 되므로 주의.
            return true;
        }

        public static mc GetManNo()
        {
            return m_iManNo ;
        }

        public static int GetManStep()
        {
            return m_iManStep ;
        }


        public static void Update()
        {
            if (m_iManNo == mc.NoneCycle) return;
            if (m_bManSetting) return;

            //if (m_iManNo != mc.AllHome && m_iManNo != mc.SLD_Home ) SEQ.InspectHomeDone();   

            SEQ.InspectLightGrid();

            bool r1, r2 , r3, r4, r5, r6 ;
            r1 = r2 = r3 = r4 = r5 =r6 = false;

            //Check Alarm.
            if (ML.ER_IsErr()) { Init(); return; }
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                     if (OM.DevInfo.iMacroType == 0) r1 = SEQ.XRYD.CycleHome();
                else if (OM.DevInfo.iMacroType == 1) r1 = SEQ.XRYE.CycleHome();
                if (!r1) return;
                r2 = SEQ.INDX.CycleHome();
                
                

                if (!r1 || !r2 /*|| !r3 || !r4 || !r5 || !r6*/) { return; }                
                m_iManNo = (int)mc.NoneCycle;
                
                Log.ShowMessage("Confirm", "All Home Finished!");

                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.INDX_Home             ) { if (SEQ.INDX.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.XRAY_Home && OM.DevInfo.iMacroType == 0) { if (SEQ.XRYD.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.XRAY_Home && OM.DevInfo.iMacroType == 1) { if (SEQ.XRYE.CycleHome      ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.INDX_CycleGet         ) { if (SEQ.INDX.CycleGet       ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.INDX_CycleBarcode     ) { if (SEQ.INDX.CycleBarcode   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.INDX_CycleOut         ) { if (SEQ.INDX.CycleOut       ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.XRAY_CycleConnect     ) { if (SEQ.XRYD.CycleConnect   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.XRAY_CycleReady       ) { if (SEQ.XRYD.CycleReady     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.XRAY_CycleWork        ) { if (SEQ.XRYD.CycleWork      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.XRAY_CycleAnalyze     ) { if (SEQ.XRYD.CycleAnalyze   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.XRAY_CycleCheck       ) { if (SEQ.XRYD.CycleCheck     ()) m_iManNo = mc.NoneCycle; }
                                                              
            else if (m_iManNo == mc.XRAY_CycleManMacro    ) { if (MM.CycleManMacro  ()) m_iManNo = mc.NoneCycle; }

            //else if (m_iManNo == mc.XRAY_CycleAging       ) { if (SEQ.XRAY.CycleAging     ()) m_iManNo = mc.NoneCycle; }
                                             
            //else if (m_iManNo == mc.USBC_CycleConnect     ) { if (SEQ.USBC.CycleConnect   ()) m_iManNo = mc.NoneCycle; }

            else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

            //Ok.
            return;
        }

        static int iWorkStep = 0;
        public static bool CycleManMacro()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(m_iManStep != 0 && m_iManStep == m_iPreManStep && !OM.MstOptn.bDebugMode, 100000))
            {
                sTemp = string.Format("m_iManStep ={0:00}", m_iManStep);
                sTemp = "Manual Man" + sTemp;
                ML.ER_SetErr(ei.ETC_ManCycleTO, sTemp);
                Log.Trace("Manual Man", sTemp);
                m_iManStep = 0;
                return true;
            }

            if (m_iManStep != m_iPreManStep)
            {
                sTemp = string.Format("Home m_iManStep={0:00}", m_iManStep);
                Log.Trace("MAnual Man", sTemp);
            }

            int iMacroCycleNo = OM.MstOptn.iMacroCycle + 1;

            Dressy  .SSetting SeqSet = new Dressy  .SSetting();
            EzSensor.SSetting EzSet  = new EzSensor.SSetting();

            if (iMacroCycleNo == 1)
            {
                //SeqSet.sGain = "1";
                SeqSet.sAreaUp = "0";
                SeqSet.sAreaDn = "0";
                SeqSet.sAreaLt = "0";
                SeqSet.sAreaRt = "0";
            }

            else if (iMacroCycleNo == 2)
            {
                //SeqSet.sGain = "3";
                SeqSet.sAreaUp = "6";
                SeqSet.sAreaDn = "6";
                SeqSet.sAreaLt = "6";
                SeqSet.sAreaRt = "6";
            }

            SeqSet.smA        = OM.Dressy[iWorkStep].dXmA     .ToString();
            SeqSet.sKvp       = OM.Dressy[iWorkStep].dXKvp    .ToString();
            SeqSet.sTime      = OM.Dressy[iWorkStep].dXTime   .ToString();
            SeqSet.sFileName1 = OM.Dressy[iWorkStep].sFileName.ToString();
            SeqSet.sBind      = OM.Dressy[iWorkStep].iBind    .ToString();

            
            EzSet.iBinning   = OM.EzSensorInfo.iBinning1x1 ;
            EzSet.iWidth   = OM.EzSensorInfo.iAg1x1Width ;
            EzSet.iHeight  = OM.EzSensorInfo.iAg1x1Hght;
            if (iMacroCycleNo == 1) 
            { 
                EzSet.smA        = OM.EzSensor[0].dEzEntrXmA  .ToString();
                EzSet.sKvp       = OM.EzSensor[0].dEzEntrXKvp .ToString();
                EzSet.sTime      = OM.EzSensor[0].dEzEntrXTime.ToString();
            }

            SeqSet.sPath1 = OM.DressyInfo.sRsltPath;
            //OM.EqpStat.sSerialList = "H151OHAAC16-07501";

            m_iPreManStep = m_iManStep;

            switch (m_iManStep) {
      
                default: sTemp = string.Format("Cycle Default Clear Manual m_iPreManStep ={0:00}", m_iPreManStep);
                         m_iManStep = 0 ;
                         return true ;
      
                //case 10: 
                //    //SEQ.IDX.MoveActr(ai.IDX_StockUpDn , fb.Bwd);
                //    //SEQ.IDX.MoveActr(ai.IDX_IdxUpDn   , fb.Bwd);
                //    m_iManStep++;
                //    return false ;
                //
                //case 11: 
                //    //if(!SEQ.IDX.GetActrStop(ai.IDX_StockUpDn, fb.Bwd))return false;
                //    if(!SEQ.IDX.GetActrStop(ai.IDX_IdxUpDn  , fb.Bwd))return false;
                //    SEQ.PCK.MoveMotr(mi.PCK_Z , pv.PCK_ZMove);
                //    m_iManStep++;
                //    return false ;
                //
                //case 12: 
                //    if(!SEQ.PCK.GetStop(mi.PCK_Z))return false;
                //    SEQ.PCK.MoveMotr(mi.PCK_Y , pv.PCK_YPlce);
                //    m_iManStep++;
                //    return false ;
                //
                //case 13: 
                //    if(!SEQ.PCK.GetStop(mi.PCK_Y))return false;
                //    m_iManStep = 0 ;
                //    return true ;

                case 10:
                    if (OM.DevInfo.iMacroType == 0)
                    {
                        SEQ.Mcr.CycleDressyInit(SeqSet);
                    }
                    else if (OM.DevInfo.iMacroType == 1)
                    {
                        SEQ.Mcr.CycleEzSensorInit(EzSet);
                        
                    }
                    
                    m_iManStep++;
                    return false;

                case 11:
                    if (OM.DevInfo.iMacroType == 1 && SEQ.Mcr.Ez1.bDetectSerial)
                    {
                        if (!SEQ.XRYE.FindSerialNo()) return false;
                    }

                    if (SEQ.Mcr.GetErrCode() != "")
                    {
                        ML.ER_SetErr(ei.ETC_MacroErr, SEQ.Mcr.GetErrCode());
                    }
                    if (OM.DevInfo.iMacroType == 0)
                    {
                        
                        if (!SEQ.Mcr.CycleDressy(iMacroCycleNo)) return false;
                    }
                    else if (OM.DevInfo.iMacroType == 1)
                    {
                        if (!SEQ.Mcr.CycleEzSensor(iMacroCycleNo)) return false;
                    }

                    //SEQ.Mcr.CycleDressy(OM.MstOptn.iMacroCycle + 1);
                    m_iManStep++;
                    return false;

                case 12:
                    if (!SEQ.Mcr.CycleEnd) return false;
                    
                    //if (!SEQ.Mcr.CycleDressy(OM.MstOptn.iMacroCycle + 1)) return false;
                    //DM.ARAY[ri.INDX].SetStat(c, r, cs.Ready);
                    m_iManStep = 0;
                    return true;


            }
        }
    }


}
