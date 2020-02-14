using COMMON;

namespace Machine
{
    class MM
    {
        private static int             m_iManStep ; //, m_iPreManStep ;
        
        private static mc m_iManNo   ;

        private static CDelayTimer     m_tmCycle   = new CDelayTimer() ; //메뉴얼 싸이클 타임아웃용.
        private static CDelayTimer     m_tmDelay   = new CDelayTimer(); //메뉴얼 싸이클

        private static bool            m_bManSetting ; //쎄팅중임.
        public  static int             m_iWorkNo; 

        public static mc m_iCrntManNo   ;

        public static void Init()
        {
            
            m_iManStep    = 0;
            //m_iPreManStep = 0;
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
            if (_iNo > mc.STCK_Home && !SEQ.InspectHomeDone()) {                                                 return false; }

            //   mcLDR_RHome
            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    //   mcIDX_Home
            m_iManNo = _iNo;     
            
            //   mcLTL_Home
            //   mcRTL_Home

            //SM.ER.SetDisp(true);jinseop

            bool bRet = true;                                                                                                                //   mcSTG_Home 
            //   mcULD_Home
            /********************/ 
            /********************/
            if      (m_iManNo == mc.NoneCycle             ) { bRet = false; }
            else if (m_iManNo == mc.AllHome               ) { }
            else if (m_iManNo == mc.LODR_Home             ) { }
            else if (m_iManNo == mc.TOOL_Home             ) { }
            else if (m_iManNo == mc.BARZ_Home             ) { }
            else if (m_iManNo == mc.IDXR_Home             ) { }
            else if (m_iManNo == mc.IDXF_Home             ) { }
            else if (m_iManNo == mc.STCK_Home             ) { }

            else if (m_iManNo == mc.LODR_CycleSply        ) {if(SM.CL_GetCmd(ci.IDXR_ClampUpDn) == fb.Fwd && SM.MT_GetCmdPos(mi.IDXR_XRear) < OM.CmnOptn.dIdxRSplyPos){
                                                                 Log.ShowMessage("Warring", "Rear Index Clamp is Up!");
                                                                 bRet = false;            
                                                             }
                                                             if(SM.CL_GetCmd(ci.IDXF_ClampUpDn) == fb.Fwd && SM.MT_GetCmdPos(mi.IDXF_XFrnt) < OM.CmnOptn.dIdxFSplyPos){
                                                                 Log.ShowMessage("Warring", "Front Index Clamp is Up!");
                                                                 bRet = false;            
                                                             }
                                                            }
                                                          
            else if (m_iManNo == mc.IDXF_CycleGet         ) {if(!SM.MT_CmprPos(mi.IDXR_XRear , SM.PM_GetValue(mi.IDXR_XRear, pv.IDXR_XRearWait ) , 1.0)){
                                                                Log.ShowMessage("Warring", "Rear Index is not in wait Position.");
                                                                bRet = false;
                                                             }
                                                            }
            else if (m_iManNo == mc.IDXF_CycleBarcode     ) {if(!SM.MT_CmprPos(mi.IDXR_XRear , SM.PM_GetValue(mi.IDXR_XRear, pv.IDXR_XRearWait ) , 1.0)){
                                                                Log.ShowMessage("Warring", "Rear Index is not in wait Position.");
                                                                bRet = false;
                                                             }
                                                             if(!DM.ARAY[ri.IDXR].CheckAllStat(cs.None)){
                                                                 Log.ShowMessage("Warring", "Rear IndexData Exist!");
                                                                 bRet = false;
                                                             }
                                                             if(SM.CL_GetCmd(ci.IDXR_ClampUpDn) == fb.Fwd ){
                                                                 Log.ShowMessage("Warring", "Rear Index Clamp is Up!");
                                                                 bRet = false ;
                                                             }
                                                            }
            else if (m_iManNo == mc.IDXF_CycleOut         ) {if(!SM.MT_CmprPos(mi.IDXR_XRear , SM.PM_GetValue(mi.IDXR_XRear, pv.IDXR_XRearWait ) , 1.0)){
                                                                Log.ShowMessage("Warring", "Rear Index is not in wait Position.");
                                                                bRet = false;
                                                             }
                                                             if(!DM.ARAY[ri.OUTZ].CheckAllStat(cs.None)){
                                                                 Log.ShowMessage("Warring", "OutZone Data Exist!");
                                                                 bRet = false;
                                                             }  
                
                                                             if(!DM.ARAY[ri.IDXR].CheckAllStat(cs.None)){
                                                                 Log.ShowMessage("Warring", "Rear IndexData Exist!");
                                                                 bRet = false;
                                                             }
                                                             if(SM.CL_GetCmd(ci.IDXR_ClampUpDn) == fb.Fwd ){
                                                                 Log.ShowMessage("Warring", "Rear Index Clamp is Up!");
                                                                 bRet = false ;
                                                             }
                                                            }
            

            else if (m_iManNo == mc.IDXR_CycleGet         ) {if(!SM.MT_CmprPos(mi.IDXF_XFrnt , SM.PM_GetValue(mi.IDXF_XFrnt, pv.IDXF_XFrntWait ) , 1.0)){
                                                                Log.ShowMessage("Warring", "Front Index is not in wait Position.");
                                                                bRet = false;
                                                             }
                                                            }
            else if (m_iManNo == mc.IDXR_CycleBarcode     ) {if(!SM.MT_CmprPos(mi.IDXF_XFrnt , SM.PM_GetValue(mi.IDXF_XFrnt, pv.IDXF_XFrntWait ) , 1.0)){
                                                                Log.ShowMessage("Warring", "Front Index is not in wait Position.");
                                                                bRet = false;
                                                             }
                                                           
                                                             if(!DM.ARAY[ri.IDXF].CheckAllStat(cs.None)){
                                                                 Log.ShowMessage("Warring", "Front IndexData Exist!");
                                                                 bRet = false;
                                                             }
                                                             if(SM.CL_GetCmd(ci.IDXF_ClampUpDn) == fb.Fwd ){
                                                                 Log.ShowMessage("Warring", "Front Index Clamp is Up!");
                                                                 bRet = false ;
                                                             }
                                                            }
            else if (m_iManNo == mc.IDXR_CycleOut         ) {if(!SM.MT_CmprPos(mi.IDXF_XFrnt , SM.PM_GetValue(mi.IDXF_XFrnt, pv.IDXF_XFrntWait ) , 1.0)){
                                                                Log.ShowMessage("Warring", "Front Index is not in wait Position.");
                                                                bRet = false;
                                                             }
                                                             if(!DM.ARAY[ri.OUTZ].CheckAllStat(cs.None)){
                                                                 Log.ShowMessage("Warring", "OutZone Data Exist!");
                                                                 bRet = false;
                                                             }               
                
                                                             if(!DM.ARAY[ri.IDXF].CheckAllStat(cs.None)){
                                                                 Log.ShowMessage("Warring", "Front IndexData Exist!");
                                                                 bRet = false;
                                                             }
                                                             if(SM.CL_GetCmd(ci.IDXF_ClampUpDn) == fb.Fwd ){
                                                                 Log.ShowMessage("Warring", "Front Index Clamp is Up!");
                                                                 bRet = false ;
                                                             }
                                                            }
            else if (m_iManNo == mc.TOOL_CycleVisn        ) {}
            else if (m_iManNo == mc.TOOL_CycleNGPick      ) {}
            else if (m_iManNo == mc.TOOL_CycleNGPlace     ) {}
            else if (m_iManNo == mc.TOOL_CycleGoodPick    ) {}
            else if (m_iManNo == mc.TOOL_CycleGoodPlace   ) {}
                                                          
            else if (m_iManNo == mc.STCK_CycleToStack     ) {}
            else if (m_iManNo == mc.STCK_CycleStack       ) {}
            else if (m_iManNo == mc.STCK_CycleOut         ) {}

            else if (m_iManNo == mc.BARZ_CycleBarPick     ) {}
            else if (m_iManNo == mc.BARZ_CycleBarPlace    ) {}
            else if (m_iManNo == mc.BARZ_CycleOut         ) {}
            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                SM.MT_SetServoAll(true);
                SEQ.LODR.InitHomeStep();
                SEQ.TOOL.InitHomeStep();
                SEQ.BARZ.InitHomeStep();
                SEQ.IDXR.InitHomeStep();
                SEQ.IDXF.InitHomeStep();
                SEQ.STCK.InitHomeStep();
            }
            else if (m_iManNo == mc.LODR_Home             ) {SM.MT_SetServoAll(true);SEQ.LODR.InitHomeStep(); }
            else if (m_iManNo == mc.TOOL_Home             ) {SM.MT_SetServoAll(true);SEQ.TOOL.InitHomeStep(); }
            else if (m_iManNo == mc.BARZ_Home             ) {SM.MT_SetServoAll(true);SEQ.BARZ.InitHomeStep(); }
            else if (m_iManNo == mc.IDXR_Home             ) {SM.MT_SetServoAll(true);SEQ.IDXR.InitHomeStep(); }
            else if (m_iManNo == mc.IDXF_Home             ) {SM.MT_SetServoAll(true);SEQ.IDXF.InitHomeStep(); }
            else if (m_iManNo == mc.STCK_Home             ) {SM.MT_SetServoAll(true);SEQ.STCK.InitHomeStep(); }
                                                          
            else if (m_iManNo == mc.LODR_CycleSply        ) {SEQ.LODR.InitCycleStep();                        }
                                                          
            else if (m_iManNo == mc.IDXF_CycleGet         ) {SEQ.IDXF.InitCycleStep();                        }
            else if (m_iManNo == mc.IDXF_CycleBarcode     ) {SEQ.IDXF.InitCycleStep();                        }
            else if (m_iManNo == mc.IDXF_CycleOut         ) {SEQ.IDXF.InitCycleStep();                        }
            else if (m_iManNo == mc.IDXR_CycleGet         ) {SEQ.IDXR.InitCycleStep();                        }
            else if (m_iManNo == mc.IDXR_CycleBarcode     ) {SEQ.IDXR.InitCycleStep();                        }
            else if (m_iManNo == mc.IDXR_CycleOut         ) {SEQ.IDXR.InitCycleStep();                        }
                                                          
            else if (m_iManNo == mc.TOOL_CycleVisn        ) {SEQ.TOOL.InitCycleStep();                        }
            else if (m_iManNo == mc.TOOL_CycleNGPick      ) {SEQ.TOOL.InitCycleStep();                        }
            else if (m_iManNo == mc.TOOL_CycleNGPlace     ) {SEQ.TOOL.InitCycleStep();                        }
            else if (m_iManNo == mc.TOOL_CycleGoodPick    ) {SEQ.TOOL.InitCycleStep();                        }
            else if (m_iManNo == mc.TOOL_CycleGoodPlace   ) {SEQ.TOOL.InitCycleStep();                        }
                                                          
            else if (m_iManNo == mc.STCK_CycleToStack     ) {SEQ.STCK.InitCycleStep();                        }
            else if (m_iManNo == mc.STCK_CycleStack       ) {SEQ.STCK.InitCycleStep();                        }
            else if (m_iManNo == mc.STCK_CycleOut         ) {SEQ.STCK.InitCycleStep();                        }

            else if (m_iManNo == mc.BARZ_CycleBarPick     ) {SEQ.BARZ.InitCycleStep();                        }
            else if (m_iManNo == mc.BARZ_CycleBarPlace    ) {SEQ.BARZ.InitCycleStep();                        }
            else if (m_iManNo == mc.BARZ_CycleOut         ) {SEQ.BARZ.InitCycleStep();                        }
            
            //else if (m_iManNo == mBARZ_CycleBarPick  c.TOOL_SubsAlignVisn ) { SEQ.TOOL.InitCycleStep(); }
            //else if (m_iManNo == mBARZ_CycleBarPlace c.TOOL_WafrAlignVisn ) { SEQ.TOOL.InitCycleStep(); }
            //else if (m_iManNo == mBARZ_CycleOut      c.WSTG_WaferGet      ) { SEQ.WSTG.InitCycleStep(); }
            //else if (m_iManNo == mc.TOOL_Eject         ) { SEQ.TOOL.InitCycleStep(); }
            //else if (m_iManNo == mc.TOOL_DispCheck     ) { SEQ.TOOL.InitCycleStep(); }
            //else if (m_iManNo == mc.TOOL_HghtCheck     ) { SEQ.TOOL.InitCycleStep(); }
            //else if (m_iManNo == mc.WSTG_ExpdWork      ) { SML.MT.GoAbsRun((int)mi.WSTG_ZExpd, PM.GetValue(mi.WSTG_ZExpd, pv.WSTG_ZExpdWork));}
            //else if (m_iManNo == mc.SSTG_SubsRailConv  ) { SEQ.SSTG.InitCycleStep(); }
            //else if (m_iManNo == mc.SSTG_WafrRailConv  ) { SEQ.WSTG.InitCycleStep(); }
            //else if (m_iManNo == mc.SSTG_SubsRailHome  ) { SML.MT.GoHome((int)mi.SSTG_XRail); }
            

            //else if (m_iManNo == mc.PCK_CyclePick    ) {SEQ.PCK.InitCycleStep(); }
            //else if (m_iManNo == mc.PCK_CycleVisn    ) {SEQ.PCK.InitCycleStep(); }
            //else if (m_iManNo == mc.PCK_CyclePrnt    ) {SEQ.PCK.InitCycleStep(); }
            //else if (m_iManNo == mc.PCK_CyclePlce    ) {SEQ.PCK.InitCycleStep(); }

            //else if (m_iManNo == mc.IDX_CycleSupply  ) {SEQ.IDX.InitCycleStep(); }
            //else if (m_iManNo == mc.IDX_CycleWork    ) {SEQ.IDX.InitCycleStep(); }
            //else if (m_iManNo == mc.IDX_CycleOut     ) {SEQ.IDX.InitCycleStep(); }
            //else if (m_iManNo == mc.IDX_LtPitchMove  ) {SML.MT.GoIncMan((int)mi.IDX_X,  OM.DevInfo.dTrayColPitch);}
            //else if (m_iManNo == mc.IDX_RtPitchMove  ) {SML.MT.GoIncMan((int)mi.IDX_X, -OM.DevInfo.dTrayColPitch);}


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
            //if (SM.ER_IsErr()) { Init(); return; }
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                r1 = SEQ.TOOL.CycleHome();
                if (!r1) return;
                r2 = SEQ.BARZ.CycleHome();
                r3 = SEQ.LODR.CycleHome();
                r4 = SEQ.IDXR.CycleHome();
                r5 = SEQ.IDXF.CycleHome();
                r6 = SEQ.STCK.CycleHome();

                if (!r1 || !r2 || !r3 || !r4 || !r5 || !r6) { return; }                
                m_iManNo = (int)mc.NoneCycle;
                
                if(SM.MT_GetHomeDoneAll()){
                    Log.ShowMessage("Confirm", "All Home Finished!");
                }

                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.LODR_Home             ) { if (SEQ.LODR.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_Home             ) { if (SEQ.TOOL.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.BARZ_Home             ) { if (SEQ.BARZ.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDXR_Home             ) { if (SEQ.IDXR.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDXF_Home             ) { if (SEQ.IDXF.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.STCK_Home             ) { if (SEQ.STCK.CycleHome      ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.LODR_CycleSply        ) { if (SEQ.LODR.CycleSupply    ()) m_iManNo = mc.NoneCycle; }
                                                                                          
            else if (m_iManNo == mc.IDXF_CycleGet         ) { if (SEQ.IDXF.CycleGet       ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDXF_CycleBarcode     ) { if (SEQ.IDXF.CycleBarcode   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDXF_CycleOut         ) { if (SEQ.IDXF.CycleOut       ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDXR_CycleGet         ) { if (SEQ.IDXR.CycleGet       ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDXR_CycleBarcode     ) { if (SEQ.IDXR.CycleBarcode   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDXR_CycleOut         ) { if (SEQ.IDXR.CycleOut       ()) m_iManNo = mc.NoneCycle; }
                                                                                          
            else if (m_iManNo == mc.TOOL_CycleVisn        ) { if (SEQ.TOOL.CycleVision    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_CycleNGPick      ) { if (SEQ.TOOL.CycleNGPick    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_CycleNGPlace     ) { if (SEQ.TOOL.CycleNGPlace   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_CycleGoodPick    ) { if (SEQ.TOOL.CycleGoodPick  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_CycleGoodPlace   ) { if (SEQ.TOOL.CycleGoodPlace ()) m_iManNo = mc.NoneCycle; }
                                                          
            else if (m_iManNo == mc.STCK_CycleToStack     ) { if (SEQ.STCK.CycleToStack   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.STCK_CycleStack       ) { if (SEQ.STCK.CycleStack     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.STCK_CycleOut         ) { if (SEQ.STCK.CycleOut       ()) m_iManNo = mc.NoneCycle; }
                                                                                          
            else if (m_iManNo == mc.BARZ_CycleBarPick     ) { if (SEQ.BARZ.CycleBarPick   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.BARZ_CycleBarPlace    ) { if (SEQ.BARZ.CycleBarPlace  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.BARZ_CycleOut         ) { if (SEQ.BARZ.CycleOut       ()) m_iManNo = mc.NoneCycle; }
             

            else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

            //Ok.
            return;
        }
    //    public static bool CycleManTrayPlce()
    //    {
    //        String sTemp;
    //        if (m_tmCycle.OnDelay(m_iManStep != 0 && m_iManStep == m_iPreManStep , 5000))
    //        {
    //            sTemp = string.Format("m_iManStep ={0:00}", m_iManStep);
    //            sTemp = "Manual Man" + sTemp;
    //            SM.ER.SetErrMsg((int)ei.ETC_AllHomeTO, sTemp);
    //            Log.Trace("Manual Man", sTemp);
    //            m_iManStep = 0;
    //            return true;
    //        }

    //        if (m_iManStep != m_iPreManStep)
    //        {
    //            sTemp = string.Format("Home m_iManStep={0:00}", m_iManStep);
    //            Log.Trace("MAnual Man", sTemp);
    //        }

    //        m_iPreManStep = m_iManStep;

    //        switch (m_iManStep) {
        
    //            default: sTemp = string.Format("Cycle Default Clear Manual m_iPreManStep ={0:00}", m_iPreManStep);
    //                     m_iManStep = 0 ;
    //                     return true ;
        
    //            case 10: 
    //                //SEQ.IDX.MoveActr(ai.IDX_StockUpDn , fb.Bwd);
    //                //SEQ.IDX.MoveActr(ai.IDX_IdxUpDn   , fb.Bwd);
    //                m_iManStep++;
    //                return false ;
        
    //            case 11: 
    //                //if(!SEQ.IDX.GetActrStop(ai.IDX_StockUpDn, fb.Bwd))return false;
    //                if(!SEQ.IDX.GetActrStop(ai.IDX_IdxUpDn  , fb.Bwd))return false;
    //                SEQ.PCK.MoveMotr(mi.PCK_Z , pv.PCK_ZMove);
    //                m_iManStep++;
    //                return false ;
        
    //            case 12: 
    //                if(!SEQ.PCK.GetStop(mi.PCK_Z))return false;
    //                SEQ.PCK.MoveMotr(mi.PCK_Y , pv.PCK_YPlce);
    //                m_iManStep++;
    //                return false ;
        
    //            case 13: 
    //                if(!SEQ.PCK.GetStop(mi.PCK_Y))return false;
    //                m_iManStep = 0 ;
    //               return true ;
    //        }
    //    }
    }


}
