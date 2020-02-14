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
        public static bool Stop;
        //private double          m_dVisnRsltGapX ;
        //private double          m_dVisnRsltGapY ;
        //private double          m_dVisnRsltGapT ;

        public static void Init()
        {
            m_iManStep    = 0;
            m_iPreManStep = 0;
            m_iManNo      = mc.NoneCycle;

            m_bManSetting = false;
            Stop          = false;
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
            if (_iNo > mc.VSTG_Home && !SEQ.InspectHomeDone()) {                                                 return false; }

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
            if      (m_iManNo == mc.NoneCycle           ) { bRet = false; }
            else if (m_iManNo == mc.AllHome             ) { }
            else if (m_iManNo == mc.PRER_Home           ) { }
            else if (m_iManNo == mc.PSTR_Home           ) { }
            else if (m_iManNo == mc.LPCK_Home           ) { }
            else if (m_iManNo == mc.RPCK_Home           ) { }
            else if (m_iManNo == mc.VSTG_Home           ) { }
            
            else if (m_iManNo == mc.PRER_Reload         ) { }
            else if (m_iManNo == mc.PRER_Supply         ) { }
            else if (m_iManNo == mc.PRER_LiftUp         ) { } 
            else if (m_iManNo == mc.PRER_LiftDown       ) { }
            
            else if (m_iManNo == mc.PSTR_Clear          ) { }
            else if (m_iManNo == mc.PSTR_Clean          ) { } 
            else if (m_iManNo == mc.PSTR_LiftUp         ) { } 
            else if (m_iManNo == mc.PSTR_LiftDown       ) { } 
            else if (m_iManNo == mc.PSTR_Out            ) { } 
            
            else if (m_iManNo == mc.LPCK_Pick           ) { if(ML.IO_GetX(xi.LPCK_Vacuum1) || ML.IO_GetX(xi.LPCK_Vacuum2) ||
                                                               ML.IO_GetX(xi.LPCK_Vacuum3) || ML.IO_GetX(xi.LPCK_Vacuum4) ||
                                                               ML.IO_GetX(xi.LPCK_Vacuum5) || ML.IO_GetX(xi.LPCK_Vacuum6)) {Log.ShowMessage("Error" , "Picker is not empty"); bRet = false; } } 
            else if (m_iManNo == mc.LPCK_Clean          ) { } 
            else if (m_iManNo == mc.LPCK_Place          ) { } 
            
            else if (m_iManNo == mc.RPCK_Pick           ) { if(ML.IO_GetX(xi.RPCK_Vacuum1) || ML.IO_GetX(xi.RPCK_Vacuum2) ||
                                                               ML.IO_GetX(xi.RPCK_Vacuum3) || ML.IO_GetX(xi.RPCK_Vacuum4) ||
                                                               ML.IO_GetX(xi.RPCK_Vacuum5) || ML.IO_GetX(xi.RPCK_Vacuum6)) {Log.ShowMessage("Error" , "Picker is not empty"); bRet = false; }  } 
            else if (m_iManNo == mc.RPCK_Move           ) { } 
            else if (m_iManNo == mc.RPCK_Place          ) { }
            
            else if (m_iManNo == mc.VSTG_Ready          ) { } 
            else if (m_iManNo == mc.VSTG_Clean          ) { } 

            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                ML.MT_SetServoAll(true);
                SEQ.PRER.InitHomeStep();
                SEQ.PSTR.InitHomeStep();
                SEQ.LPCK.InitHomeStep();
                SEQ.RPCK.InitHomeStep();
                SEQ.VSTG.InitHomeStep();
            }
            else if (m_iManNo == mc.PRER_Home           ) {ML.MT_SetServoAll(true);SEQ.PRER.InitHomeStep(); }
            else if (m_iManNo == mc.PSTR_Home           ) {ML.MT_SetServoAll(true);SEQ.PSTR.InitHomeStep(); }
            else if (m_iManNo == mc.LPCK_Home           ) {ML.MT_SetServoAll(true);SEQ.LPCK.InitHomeStep(); }
            else if (m_iManNo == mc.RPCK_Home           ) {ML.MT_SetServoAll(true);SEQ.RPCK.InitHomeStep(); }
            else if (m_iManNo == mc.VSTG_Home           ) {ML.MT_SetServoAll(true);SEQ.VSTG.InitHomeStep(); }

            else if (m_iManNo == mc.PRER_Reload         ) {SEQ.PRER.InitCycleStep(); }
            else if (m_iManNo == mc.PRER_Supply         ) {SEQ.PRER.InitCycleStep(); }
            else if (m_iManNo == mc.PRER_LiftUp         ) {SEQ.PRER.InitCycleStep(); }
            else if (m_iManNo == mc.PRER_LiftDown       ) {SEQ.PRER.InitCycleStep(); }
            
            else if (m_iManNo == mc.PSTR_Clear          ) {SEQ.PSTR.InitCycleStep(); }
            else if (m_iManNo == mc.PSTR_Clean          ) {SEQ.PSTR.InitCycleStep(); }
            else if (m_iManNo == mc.PSTR_LiftUp         ) {SEQ.PSTR.InitCycleStep(); }
            else if (m_iManNo == mc.PSTR_LiftDown       ) {SEQ.PSTR.InitCycleStep(); }
            else if (m_iManNo == mc.PSTR_Out            ) {SEQ.PSTR.InitCycleStep(); }

            else if (m_iManNo == mc.LPCK_Pick           ) {SEQ.LPCK.InitCycleStep(); }
            else if (m_iManNo == mc.LPCK_Clean          ) {SEQ.LPCK.InitCycleStep(); }
            else if (m_iManNo == mc.LPCK_Place          ) {SEQ.LPCK.InitCycleStep(); }
            else if (m_iManNo == mc.LPCK_PckrClean      ) {SEQ.LPCK.InitCycleStep(); }

            else if (m_iManNo == mc.RPCK_Pick           ) {SEQ.RPCK.InitCycleStep(); }
            else if (m_iManNo == mc.RPCK_Move           ) {SEQ.RPCK.InitCycleStep(); }
            else if (m_iManNo == mc.RPCK_Place          ) {SEQ.RPCK.InitCycleStep(); }

            else if (m_iManNo == mc.VSTG_Ready          ) {SEQ.VSTG.InitCycleStep(); }
            else if (m_iManNo == mc.VSTG_Clean          ) {SEQ.VSTG.InitCycleStep(); }

            else if (m_iManNo == mc.PRER_CleanerOnOff   ) {ML.IO_SetY(yi.PSTR_AirBlwrBtm1, !ML.IO_GetY(yi.PSTR_AirBlwrBtm1));
                                                           ML.IO_SetY(yi.PSTR_AirBlwrBtm2, !ML.IO_GetY(yi.PSTR_AirBlwrBtm2));
                                                           ML.IO_SetY(yi.PSTR_AirBlwrTop1, !ML.IO_GetY(yi.PSTR_AirBlwrTop1));
                                                           ML.IO_SetY(yi.PSTR_AirBlwrTop2, !ML.IO_GetY(yi.PSTR_AirBlwrTop2));
                                                           ML.IO_SetY(yi.PSTR_IonBlwrTop , !ML.IO_GetY(yi.PSTR_IonBlwrTop ));
                                                           ML.IO_SetY(yi.PSTR_IonBlwrBtm , !ML.IO_GetY(yi.PSTR_IonBlwrBtm ));}
            else if (m_iManNo == mc.LPCK_CleanerOnOff   ) {ML.IO_SetY(yi.LPCK_AirBlwrBtm1, !ML.IO_GetY(yi.LPCK_AirBlwrBtm1));
                                                           ML.IO_SetY(yi.LPCK_AirBlwrBtm2, !ML.IO_GetY(yi.LPCK_AirBlwrBtm2));
                                                           ML.IO_SetY(yi.LPCK_IonBlwrBtm , !ML.IO_GetY(yi.LPCK_IonBlwrBtm ));}
            else if (m_iManNo == mc.VSTG_CleanerOnOff   ) {ML.IO_SetY(yi.VSTG_AirBlwrTop1, !ML.IO_GetY(yi.VSTG_AirBlwrTop1));
                                                           ML.IO_SetY(yi.VSTG_AirBlwrTop2, !ML.IO_GetY(yi.VSTG_AirBlwrTop2));
                                                           ML.IO_SetY(yi.VSTG_IonBlwrTop , !ML.IO_GetY(yi.VSTG_IonBlwrTop ));}
            
            else if (m_iManNo == mc.LPCK_VacuumOnOff    ) {SEQ.LPCK.InitCycleStep(); }
            else if (m_iManNo == mc.LPCK_EjectOnOff     ) {ML.IO_SetY(yi.LPCK_Eject1, !ML.IO_GetY(yi.LPCK_Eject1));
                                                           ML.IO_SetY(yi.LPCK_Eject2, !ML.IO_GetY(yi.LPCK_Eject2));
                                                           ML.IO_SetY(yi.LPCK_Eject3, !ML.IO_GetY(yi.LPCK_Eject3));
                                                           ML.IO_SetY(yi.LPCK_Eject4, !ML.IO_GetY(yi.LPCK_Eject4));
                                                           ML.IO_SetY(yi.LPCK_Eject5, !ML.IO_GetY(yi.LPCK_Eject5));
                                                           ML.IO_SetY(yi.LPCK_Eject6, !ML.IO_GetY(yi.LPCK_Eject6));}
            else if (m_iManNo == mc.RPCK_VacuumOnOff    ) {SEQ.RPCK.InitCycleStep(); }
            else if (m_iManNo == mc.RPCK_EjectOnOff     ) {ML.IO_SetY(yi.RPCK_Eject1, !ML.IO_GetY(yi.RPCK_Eject1));
                                                           ML.IO_SetY(yi.RPCK_Eject2, !ML.IO_GetY(yi.RPCK_Eject2));
                                                           ML.IO_SetY(yi.RPCK_Eject3, !ML.IO_GetY(yi.RPCK_Eject3));
                                                           ML.IO_SetY(yi.RPCK_Eject4, !ML.IO_GetY(yi.RPCK_Eject4));
                                                           ML.IO_SetY(yi.RPCK_Eject5, !ML.IO_GetY(yi.RPCK_Eject5));
                                                           ML.IO_SetY(yi.RPCK_Eject6, !ML.IO_GetY(yi.RPCK_Eject6));}
            else if (m_iManNo == mc.VSTG_VacuumOnOff    ) {SEQ.VSTG.InitCycleStep(); }
            else if (m_iManNo == mc.VSTG_EjectOnOff     ) {ML.IO_SetY(yi.VSTG_Eject1, !ML.IO_GetY(yi.VSTG_Eject1));
                                                           ML.IO_SetY(yi.VSTG_Eject2, !ML.IO_GetY(yi.VSTG_Eject2));
                                                           ML.IO_SetY(yi.VSTG_Eject3, !ML.IO_GetY(yi.VSTG_Eject3));
                                                           ML.IO_SetY(yi.VSTG_Eject4, !ML.IO_GetY(yi.VSTG_Eject4));
                                                           ML.IO_SetY(yi.VSTG_Eject5, !ML.IO_GetY(yi.VSTG_Eject5));
                                                           ML.IO_SetY(yi.VSTG_Eject6, !ML.IO_GetY(yi.VSTG_Eject6));}
            else if(m_iManNo == mc.RAIL_RailRun)          { m_iManStep = 10;}
            
            m_bManSetting = false; //m_bManSetting 중요함 리턴전에 꼭 펄스 시켜야함. 쓰레드가 달라서. ::Update에서 m_iManNo=0이 되므로 주의.
            return true;
         }

        public static bool Working()
        {
            if(m_iManNo == mc.NoneCycle) return false;
            else                         return true ;
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
            if (m_iManNo == mc.NoneCycle) {
                Stop = false;
                return;
            }
            if (m_bManSetting) return;

            //if (m_iManNo != mc.AllHome && m_iManNo != mc.SLD_Home ) SEQ.InspectHomeDone();   

            if(!OM.MstOptn.bDebugMode) SEQ.InspectLightGrid();

            //Check Alarm.
            if (ML.ER_IsErr()) { Init(); return; }
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                bool r1 = SEQ.PRER.CycleHome();
                bool r2 = SEQ.PSTR.CycleHome();
                bool r3 = SEQ.LPCK.CycleHome();
                bool r4 = SEQ.RPCK.CycleHome();
                if(!r3 || !r4) return;
                bool r5 = SEQ.VSTG.CycleHome();

                if(!r1 || !r2 || !r3 || !r4 || !r5  ) return ;
                if (ML.ER_IsErr()) { Init(); return; }
                m_iManNo = (int)mc.NoneCycle;
                Log.ShowMessage("Confirm", "All Homing Finished!");
                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.PRER_Home           ) { if (SEQ.PRER.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTR_Home           ) { if (SEQ.PSTR.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LPCK_Home           ) { if (SEQ.LPCK.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.RPCK_Home           ) { if (SEQ.RPCK.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VSTG_Home           ) { if (SEQ.VSTG.CycleHome      ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.PRER_Reload         ) { if( SEQ.PRER.CycleReload    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PRER_Supply         ) { if( SEQ.PRER.CycleSupply    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PRER_LiftUp         ) { if( SEQ.PRER.CycleLiftUp    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PRER_LiftDown       ) { if( SEQ.PRER.CycleLiftDown  ()) m_iManNo = mc.NoneCycle; }
                                                                                      
            else if (m_iManNo == mc.PSTR_Clear          ) { if( SEQ.PSTR.CycleClear     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTR_Clean          ) { if( SEQ.PSTR.CycleClean     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTR_LiftUp         ) { if( SEQ.PSTR.CycleLiftUp    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTR_LiftDown       ) { if( SEQ.PSTR.CycleLiftDown  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTR_Out            ) { if( SEQ.PSTR.CycleOut       ()) m_iManNo = mc.NoneCycle; }
                                                                                        
            else if (m_iManNo == mc.LPCK_Pick           ) { if( SEQ.LPCK.CyclePick      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LPCK_Clean          ) { if( SEQ.LPCK.CycleClean     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LPCK_Place          ) { if( SEQ.LPCK.CyclePlace     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LPCK_PckrClean      ) { if( SEQ.LPCK.CyclePckrClean ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.RPCK_Pick           ) { if( SEQ.RPCK.CyclePick      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.RPCK_Move           ) { if( SEQ.RPCK.CycleMove      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.RPCK_Place          ) { if( SEQ.RPCK.CyclePlace     ()) m_iManNo = mc.NoneCycle; }
                                                                                        
            else if (m_iManNo == mc.VSTG_Ready          ) { if( SEQ.VSTG.CycleReady     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VSTG_Clean          ) { if( SEQ.VSTG.CycleClean     ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.LPCK_VacuumOnOff    ) { if( SEQ.LPCK.CycleManVacuumOnOff()) m_iManNo = mc.NoneCycle;}
            else if (m_iManNo == mc.RPCK_VacuumOnOff    ) { if (SEQ.RPCK.CycleManVacuumOnOff()) m_iManNo = mc.NoneCycle;}
            else if (m_iManNo == mc.VSTG_VacuumOnOff    ) { if (SEQ.VSTG.CycleManVacuumOnOff()) m_iManNo = mc.NoneCycle;}

            else if (m_iManNo == mc.RAIL_RailRun        ) { if(CycleManRailRun              ()) m_iManNo = mc.NoneCycle;}

            else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

            //Ok.
            return;
        }
        public static bool CycleManRailRun()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(m_iManStep != 0 && m_iManStep == m_iPreManStep , 5000))
            {
                sTemp = string.Format("m_iManStep ={0:00}", m_iManStep);
                sTemp = "Manual Man" + sTemp;
                ML.ER_SetErr((int)ei.ETC_HomeTO, sTemp);
                Log.Trace("Manual Man", sTemp);
                m_iManStep = 0;
                return true;
            }
        
            if (m_iManStep != m_iPreManStep)
            {
                sTemp = string.Format("Home m_iManStep={0:00}", m_iManStep);
                Log.Trace("MAnual Man", sTemp);
            }
        
            m_iPreManStep = m_iManStep;
        
            switch (m_iManStep) {
        
                default: sTemp = string.Format("Cycle Default Clear Manual m_iPreManStep ={0:00}", m_iPreManStep);
                         m_iManStep = 0 ;
                         return true ;
        
                case 10:
                    if (ML.MT_GetStop(mi.PRER_X) && ML.MT_GetStop(mi.PSTR_X))
                    {
                        m_iManStep = 20;
                        return false;
                    }
                    if (!ML.MT_GetStop(mi.PRER_X) || !ML.MT_GetStop(mi.PSTR_X)) 
                    {
                        ML.MT_Stop(mi.PRER_X);
                        ML.MT_Stop(mi.PSTR_X);
                    }

                    m_iManStep++;
                    return false ;
        
                case 11: 
                    if(ML.MT_GetStop(mi.PRER_X)) return false;
                    if(ML.MT_GetStop(mi.PSTR_X)) return false;


                    m_iManStep = 0;
                    return true;

                case 20:
                    ML.MT_JogVel(mi.PRER_X, ML.MT_GetRunVel(mi.PRER_X));
                    ML.MT_JogVel(mi.PSTR_X, ML.MT_GetRunVel(mi.PSTR_X));

                    m_iManStep = 0 ;
                    return true ;
            }
        }
    }


}
