﻿using COMMON;

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
            if (SEQ._bRun && _iNo != mc.LODR_ManLotSupply    ) { Log.ShowMessage("ERROR", "Autorunning"       ); return false; }
            if (_iNo > mc.AllHome && !SEQ.InspectHomeDone()) {                                                 return false; }

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
            else if (m_iManNo == mc.LODR_Home         ) { }
            else if (m_iManNo == mc.TTBL_Home         ) { }
            else if (m_iManNo == mc.VISN_Home         ) { }
            else if (m_iManNo == mc.MARK_Home         ) { }
            else if (m_iManNo == mc.ULDR_Home         ) { }
            else if (m_iManNo == mc.REJM_Home         ) { }
            else if (m_iManNo == mc.REJV_Home         ) { }

            else if (m_iManNo == mc.LODR_CycleHold    ) { }
            else if (m_iManNo == mc.LODR_CyclePush    ) { }
            else if (m_iManNo == mc.LODR_CyclePick    ) { }
            else if (m_iManNo == mc.LODR_ManLotSupply ) { }
            else if (m_iManNo == mc.LODR_PshrWaitPos  ) { }
                

            else if (m_iManNo == mc.TTBL_CycleMove    ) { }
            else if (m_iManNo == mc.TTBL_CLAllFwd     ) {if(!ML.MT_GetStop(mi.TBLE_TTble)) bRet = false; }
            else if (m_iManNo == mc.TTBL_CLAllBwd     ) {if(!ML.MT_GetStop(mi.TBLE_TTble)) bRet = false; }

            else if (m_iManNo == mc.VISN_CycleWork    ) { }
            else if (m_iManNo == mc.VISN_ManSupply    ) { }
            else if (m_iManNo == mc.VISN_ManPlace     ) { }
            else if (m_iManNo == mc.VISN_ManReverse   ) { }

            else if (m_iManNo == mc.MARK_CycleWork    ) { }
            else if (m_iManNo == mc.MARK_CycleManChage) { }

            else if (m_iManNo == mc.ULDR_CycleMove    ) { }
            else if (m_iManNo == mc.ULDR_CycleDlvr    ) { }
            else if (m_iManNo == mc.ULDR_CyclePick    ) { }
            else if (m_iManNo == mc.ULDR_CyclePlce    ) { }
            else if (m_iManNo == mc.ULDR_CyclePaint   ) { }
            else if (m_iManNo == mc.ULDR_YIdxWaitPos  ) { if(ML.CL_GetCmd(ci.ULDR_OutPshrFwBw) != fb.Bwd) bRet = false;}

            else if (m_iManNo == mc.RJEM_CycleWork    ) { }

            else if (m_iManNo == mc.RJEV_CycleWork    ) { }

            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                ML.MT_SetServoAll(true);
                SEQ.LODR.InitHomeStep();
                SEQ.TTBL.InitHomeStep();
                SEQ.VISN.InitHomeStep();
                SEQ.MARK.InitHomeStep();
                SEQ.ULDR.InitHomeStep();
                SEQ.REJM.InitHomeStep();
                SEQ.REJV.InitHomeStep();
            }
            else if (m_iManNo == mc.LODR_Home          ) {ML.MT_SetServoAll(true);SEQ.LODR.InitHomeStep(); }
            else if (m_iManNo == mc.TTBL_Home          ) {ML.MT_SetServoAll(true);SEQ.TTBL.InitHomeStep(); }
            else if (m_iManNo == mc.VISN_Home          ) {ML.MT_SetServoAll(true);SEQ.VISN.InitHomeStep(); }
            else if (m_iManNo == mc.MARK_Home          ) {ML.MT_SetServoAll(true);SEQ.MARK.InitHomeStep(); }
            else if (m_iManNo == mc.ULDR_Home          ) {ML.MT_SetServoAll(true);SEQ.ULDR.InitHomeStep(); }
            else if (m_iManNo == mc.REJM_Home          ) {ML.MT_SetServoAll(true);SEQ.REJM.InitHomeStep(); }
            else if (m_iManNo == mc.REJV_Home          ) {ML.MT_SetServoAll(true);SEQ.REJV.InitHomeStep(); }
                                                       
            else if (m_iManNo == mc.LODR_CycleHold     ) {SEQ.LODR.InitCycleStep();                        }
            else if (m_iManNo == mc.LODR_CyclePush     ) {SEQ.LODR.InitCycleStep();                        }
            else if (m_iManNo == mc.LODR_CyclePick     ) {SEQ.PLDR.InitCycleStep();                        }
            else if (m_iManNo == mc.LODR_ManLotSupply  ) {SEQ.LODR.InitCycleStep();                        }
            else if (m_iManNo == mc.LODR_PshrWaitPos   ) {ML.MT_GoAbsMan(mi.LODR_XPshr, PM.GetValue(mi.LODR_XPshr, pv.LODR_XPshrWait)); m_iManNo = mc.NoneCycle;}
                                                       
            else if (m_iManNo == mc.TTBL_CycleMove     ) {SEQ.TTBL.InitCycleStep();                        }
            else if (m_iManNo == mc.TTBL_CLAllFwd      ) {ML.CL_Move(ci.TBLE_Grpr1FwBw , fb.Fwd); 
                                                          ML.CL_Move(ci.TBLE_Grpr2FwBw , fb.Fwd);
                                                          ML.CL_Move(ci.TBLE_Grpr3FwBw , fb.Fwd);
                                                          ML.CL_Move(ci.TBLE_Grpr4FwBw , fb.Fwd);
                                                          ML.CL_Move(ci.TBLE_Grpr5FwBw , fb.Fwd);
                                                          ML.CL_Move(ci.TBLE_Grpr6FwBw , fb.Fwd);
                                                          m_iManNo = mc.NoneCycle;
                                                         }
            else if (m_iManNo == mc.TTBL_CLAllBwd      ) {ML.CL_Move(ci.TBLE_Grpr1FwBw , fb.Bwd); 
                                                          ML.CL_Move(ci.TBLE_Grpr2FwBw , fb.Bwd);
                                                          ML.CL_Move(ci.TBLE_Grpr3FwBw , fb.Bwd);
                                                          ML.CL_Move(ci.TBLE_Grpr4FwBw , fb.Bwd);
                                                          ML.CL_Move(ci.TBLE_Grpr5FwBw , fb.Bwd);
                                                          ML.CL_Move(ci.TBLE_Grpr6FwBw , fb.Bwd);
                                                          m_iManNo = mc.NoneCycle;
                                                         }
                                                       
            else if (m_iManNo == mc.VISN_CycleWork     ) {SEQ.VISN.InitCycleStep();                        }
            else if (m_iManNo == mc.VISN_ManSupply     ) {SEQ.VISN.InitCycleStep();                        }
            else if (m_iManNo == mc.VISN_ManPlace      ) {SEQ.VISN.InitCycleStep();                        }
            else if (m_iManNo == mc.VISN_ManReverse    ) {SEQ.VISN.InitCycleStep();                        }
                                                       
            else if (m_iManNo == mc.MARK_CycleWork     ) {SEQ.MARK.InitCycleStep();                        }
            else if (m_iManNo == mc.MARK_CycleManChage ) {SEQ.MARK.InitCycleStep();                        }
                                                       
            else if (m_iManNo == mc.ULDR_CycleMove     ) {SEQ.ULDR.InitCycleStep();                        }
            else if (m_iManNo == mc.ULDR_CycleDlvr     ) {SEQ.PULD.InitCycleStep();                        }
            else if (m_iManNo == mc.ULDR_CyclePick     ) {SEQ.ULDR.InitCycleStep();                        }
            else if (m_iManNo == mc.ULDR_CyclePlce     ) {SEQ.ULDR.InitCycleStep();                        }
            else if (m_iManNo == mc.ULDR_CyclePaint    ) {SEQ.ULDR.InitCycleStep();                        }
            else if (m_iManNo == mc.ULDR_YIdxWaitPos   ) { ML.MT_GoAbsMan(mi.ULDR_YIndx, PM.GetValue(mi.ULDR_YIndx, pv.ULDR_YIndxWait)); m_iManNo = mc.NoneCycle;}
                                           
            else if (m_iManNo == mc.RJEM_CycleWork     ) {SEQ.REJM.InitCycleStep();                        }
                                                       
            else if (m_iManNo == mc.RJEV_CycleWork     ) {SEQ.REJV.InitCycleStep();                        }
            
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

            bool r1, r2 , r3, r4, r5, r6 , r7;
            r1 = r2 = r3 = r4 = r5 =r6 = r7 = false;

            //Check Alarm.
            if (ML.ER_IsErr()) { Init(); return; }
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                r2 = SEQ.TTBL.CycleHome();
                if (!r2) return ;
                r1 = SEQ.LODR.CycleHome();
                r3 = SEQ.VISN.CycleHome();
                r4 = SEQ.MARK.CycleHome();
                r5 = SEQ.ULDR.CycleHome();
                r6 = SEQ.REJM.CycleHome();
                r7 = SEQ.REJV.CycleHome();

                if (!r1 || !r2 || !r3 || !r4 || !r5 || !r6 || !r7) { return; }                
                m_iManNo = (int)mc.NoneCycle;
              
                Log.ShowMessage("Confirm", "All Home Finished!");

                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.LODR_Home          ) { if (SEQ.LODR.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TTBL_Home          ) { if (SEQ.TTBL.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VISN_Home          ) { if (SEQ.VISN.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.MARK_Home          ) { if (SEQ.MARK.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_Home          ) { if (SEQ.ULDR.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.REJM_Home          ) { if (SEQ.REJM.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.REJV_Home          ) { if (SEQ.REJV.CycleHome      ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.LODR_CycleHold     ) { if (SEQ.LODR.CycleHold      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LODR_CyclePush     ) { if (SEQ.LODR.CyclePush      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LODR_CyclePick     ) { if (SEQ.PLDR.CycleWork      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LODR_ManLotSupply  ) { if (SEQ.LODR.CycleManSupply ()) m_iManNo = mc.NoneCycle; }
            
            else if (m_iManNo == mc.TTBL_CycleMove     ) { if (SEQ.TTBL.CycleMove      ()) m_iManNo = mc.NoneCycle; }
            
            else if (m_iManNo == mc.VISN_CycleWork     ) { if (SEQ.VISN.CycleWork      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VISN_ManSupply     ) { if (SEQ.VISN.CycleManSupply ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VISN_ManPlace      ) { if (SEQ.VISN.CycleManPlace  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VISN_ManReverse    ) { if (SEQ.VISN.CycleReverse   ()) m_iManNo = mc.NoneCycle; }
                                                                      
            else if (m_iManNo == mc.MARK_CycleWork     ) { if (SEQ.MARK.CycleWork      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.MARK_CycleManChage ) { if (SEQ.MARK.CycleManChange ()) m_iManNo = mc.NoneCycle; }
            
            else if (m_iManNo == mc.ULDR_CycleMove     ) { if (SEQ.ULDR.CycleMove      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_CycleDlvr     ) { if (SEQ.PULD.CycleWork      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_CyclePick     ) { if (SEQ.ULDR.CyclePick      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_CyclePlce     ) { if (SEQ.ULDR.CyclePlce      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_CyclePaint    ) { if (SEQ.ULDR.CyclePaint     ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.RJEM_CycleWork     ) { if (SEQ.REJM.CycleReject    ()) m_iManNo = mc.NoneCycle; }
            
            else if (m_iManNo == mc.RJEV_CycleWork     ) { if (SEQ.REJV.CycleReject    ()) m_iManNo = mc.NoneCycle; }                               

            //else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

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
    //            ML.ER.SetErrMsg((int)ei.ETC_AllHomeTO, sTemp);
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
