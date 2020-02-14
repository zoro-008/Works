using System;
using COMMON;
using SML;

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

            if (_iNo     < 0                                ) { Log.ShowMessage("ERROR", "Wrong Manual No"   ); return false; }
            if (_iNo     >= mc.MAX_MANUAL_CYCLE             ) { Log.ShowMessage("ERROR", "Wrong Manual No"   ); return false; }
            if (m_iManNo != mc.NoneCycle                    ) { Log.ShowMessage("ERROR", "Doing Manual Cycle"); return false; }
            if (SEQ._bRun                                   ) { Log.ShowMessage("ERROR", "Autorunning"       ); return false; }
            if (_iNo != mc.AllHome && !SEQ.InspectHomeDone()) {                                                 return false; }

            if (!SM.DIO.GetX((int)xi.ETC_MainAir)            ) { Log.ShowMessage("ERROR", "Wrong Main Air"    ); return false; }

            //Check Alarm.
            if (SM.ERR.IsErr()) { Init(); return false; } //아 밑에 처리 하는 애 때문에 잠시 이렇게 함.          //test

            //    if(!IO_GetX(xETC_MainPower) ) {FM_MsgOk("ERR","Power On Plz");      return false ;} //test
            //if (!SM.IO.GetX((int)EN_INPUT_ID.xETC_MainAirChk)) { MessageBox.Show("Check Main Air!", "ERROR"); return false; } //test
            m_bStdCalPick = false;                                                                                                            //   mcLDR_FHome
            //   mcLDR_RHome
            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    //   mcIDX_Home
            m_iManNo = _iNo;     
            
            //   mcLTL_Home
            //   mcRTL_Home

            SM.ERR.SetNeedShowErr(true);

            bool bRet = true;                                                                                                                //   mcSTG_Home 
            //   mcULD_Home
            /********************/
            /* 프리프로세서     */
            /********************/
            if      (m_iManNo == mc.NoneCycle       ) { bRet = false; }
            else if (m_iManNo == mc.AllHome         ) { }
            else if (m_iManNo == mc.LDR_Home        ) { }
            else if (m_iManNo == mc.IDX_Home        ) { }

            else if (m_iManNo == mc.LDR_CycleSupply ) { }
            else if (m_iManNo == mc.LDR_CycleWork   ) { }
            else if (m_iManNo == mc.LDR_TraySupply  ) { }
                                                    
            else if (m_iManNo == mc.IDX_CycleSupply ) { }
            else if (m_iManNo == mc.IDX_CycleWork   ) { }
            else if (m_iManNo == mc.IDX_CycleOut    ) { }

            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)
            {
                SM.MTR.SetServoAll(true);
                SEQ.LDR.InitHomeStep();
                SEQ.IDX.InitHomeStep();
            }
            else if (m_iManNo == mc.LDR_Home         ) {SEQ.LDR.InitHomeStep(); }
            else if (m_iManNo == mc.IDX_Home         ) {SEQ.IDX.InitHomeStep(); }

            else if (m_iManNo == mc.LDR_CycleSupply  ) {SEQ.LDR.InitCycleStep(); }
            else if (m_iManNo == mc.LDR_CycleWork    ) {SEQ.LDR.InitCycleStep(); }
            else if (m_iManNo == mc.LDR_TraySupply   ) {SEQ.LDR.InitCycleStep(); }

            else if (m_iManNo == mc.IDX_CycleSupply  ) {SEQ.IDX.InitCycleStep(); }
            else if (m_iManNo == mc.IDX_CycleWork    ) {SEQ.IDX.InitCycleStep(); }
            else if (m_iManNo == mc.IDX_CycleOut     ) {SEQ.IDX.InitCycleStep(); }

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

//            if (m_iManNo != mc.AllHome && m_iManNo != mc.SLD_Home ) SEQ.InspectHomeDone();
            


            SEQ.InspectLightGrid();

            bool r1, r2, r3;//, r4, r5, r6, r7, r8, r9;
            r1 = r2 = r3 = false; //r4 = r5 = r6 = r7 = r8 = r9 = false;

            //Check Alarm.
            if (SM.ERR.IsErr()) { Init(); return; }             //test
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                r1 = SEQ.LDR.CycleHome();
                r2 = SEQ.IDX.CycleHome();

                if(!r1 || !r2)return ;
                
                m_iManNo = (int)mc.NoneCycle;
                
                Log.ShowMessage("Confirm", "All Home Finished!");



                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.LDR_Home         ) {if(SEQ.LDR.CycleHome      ()) m_iManNo = mc.NoneCycle ;}
            else if (m_iManNo == mc.IDX_Home         ) {if(SEQ.IDX.CycleHome      ()) m_iManNo = mc.NoneCycle ;}
                                                                                    
            else if (m_iManNo == mc.LDR_CycleSupply  ) {if(SEQ.LDR.CyclePick    ()) m_iManNo = mc.NoneCycle ;}
            else if (m_iManNo == mc.LDR_CycleWork    ) {if(SEQ.LDR.CyclePlace      ()) m_iManNo = mc.NoneCycle ;}
            else if (m_iManNo == mc.LDR_TraySupply   ) {if(SEQ.LDR.CyclePckrRight()) m_iManNo = mc.NoneCycle ;}
                                                                                    
            else if (m_iManNo == mc.IDX_CycleSupply  ) {if(SEQ.IDX.CycleToMark    ()) m_iManNo = mc.NoneCycle ;}
            else if (m_iManNo == mc.IDX_CycleWork    ) {if(SEQ.IDX.CycleMark      ()) m_iManNo = mc.NoneCycle ;}
            else if (m_iManNo == mc.IDX_CycleOut     ) {if(SEQ.IDX.CycleOut       ()) m_iManNo = mc.NoneCycle ;}
            

            else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

            //Ok.
            return;
        }
        public static bool CycleManTrayPlce()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iManStep != 0 && m_iManStep == m_iPreManStep , 5000))
            {
                sTemp = string.Format("m_iManStep ={0:00}", m_iManStep);
                sTemp = "Manual Man" + sTemp;
                SM.ERR.SetErrMsg((int)ei.ETC_AllHomeTO, sTemp);
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

                    m_iManStep++;
                    return false ;
        
                case 11: 

                    m_iManStep++;
                    return false ;
        
                case 12: 

                    m_iManStep++;
                    return false ;
        
                case 13: 
                    m_iManStep = 0 ;
                   return true ;
            }
        }
    }


}
