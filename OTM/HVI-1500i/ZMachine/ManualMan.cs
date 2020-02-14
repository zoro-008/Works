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
            if (_iNo > mc.ULDR_Home && !SEQ.InspectHomeDone()) {                                                 return false; }

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
            else if (m_iManNo == mc.LODR_Home           ) { }
            else if (m_iManNo == mc.PREB_Home           ) { }
            else if (m_iManNo == mc.HEAD_Home           ) { }
            else if (m_iManNo == mc.PSTB_Home           ) { }
            else if (m_iManNo == mc.ULDR_Home           ) { }

            else if (m_iManNo == mc.LODR_Wait           ) { }
            else if (m_iManNo == mc.LODR_Supply         ) { }
            else if (m_iManNo == mc.LODR_Pick           ) { } 
            else if (m_iManNo == mc.LODR_Work           ) { }
            else if (m_iManNo == mc.LODR_Drop           ) { } 

            else if (m_iManNo == mc.VSNZ_Wait           ) { } 
            else if (m_iManNo == mc.VSNZ_Stt            ) { } 
            else if (m_iManNo == mc.VSNZ_Move           ) { } 
            else if (m_iManNo == mc.VSNZ_Insp           ) { } 
            else if (m_iManNo == mc.VSNZ_Next           ) { } 
            
            else if (m_iManNo == mc.PSTB_Wait           ) { } 
            else if (m_iManNo == mc.PSTB_Stt            ) { } 
            else if (m_iManNo == mc.PSTB_In             ) { } 
            else if (m_iManNo == mc.PSTB_Work           ) {SEQ.PSTB.CopyArray(); if(SEQ.PSTB.ArayMark.GetCntStat(cs.Fail) == 0) {Log.ShowMessage("Error" , "There is no Fail on PostBufferZone"); bRet = false; } }
            else if (m_iManNo == mc.PSTB_Out            ) { } 
            else if (m_iManNo == mc.PSTB_Next           ) { } 
            else if (m_iManNo == mc.PSTB_Replace        ) { } 
            
            
            else if (m_iManNo == mc.ULDR_Wait           ) { } 
            else if (m_iManNo == mc.ULDR_Supply         ) { } 
            else if (m_iManNo == mc.ULDR_Pick           ) { } 
            else if (m_iManNo == mc.ULDR_Work           ) { } 
            else if (m_iManNo == mc.ULDR_Drop           ) { } 

            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                ML.MT_SetServoAll(true);
                SEQ.LODR.InitHomeStep();
                SEQ.PREB.InitHomeStep();
                SEQ.VSNZ.InitHomeStep();
                SEQ.PSTB.InitHomeStep();
                SEQ.ULDR.InitHomeStep();
            }
            else if (m_iManNo == mc.LODR_Home           ) {ML.MT_SetServoAll(true);SEQ.LODR.InitHomeStep(); }
            else if (m_iManNo == mc.PREB_Home           ) {ML.MT_SetServoAll(true);SEQ.PREB.InitHomeStep(); }
            else if (m_iManNo == mc.HEAD_Home           ) {ML.MT_SetServoAll(true);SEQ.VSNZ.InitHomeStep(); }
            else if (m_iManNo == mc.PSTB_Home           ) {ML.MT_SetServoAll(true);SEQ.PSTB.InitHomeStep(); }
            else if (m_iManNo == mc.ULDR_Home           ) {ML.MT_SetServoAll(true);SEQ.ULDR.InitHomeStep(); }

            else if (m_iManNo == mc.LODR_Wait           ) {SEQ.LODR.InitCycleStep(); }
            else if (m_iManNo == mc.LODR_Supply         ) {SEQ.LODR.InitCycleStep(); }
            else if (m_iManNo == mc.LODR_Pick           ) {SEQ.LODR.InitCycleStep(); }
            else if (m_iManNo == mc.LODR_Work           ) {SEQ.LODR.InitCycleStep(); }
            else if (m_iManNo == mc.LODR_Drop           ) {SEQ.LODR.InitCycleStep(); } 

            else if (m_iManNo == mc.VSNZ_Wait           ) {SEQ.VSNZ.InitCycleStep(); }
            else if (m_iManNo == mc.VSNZ_Stt            ) {SEQ.VSNZ.InitCycleStep(); }
            else if (m_iManNo == mc.VSNZ_Move           ) {SEQ.VSNZ.InitCycleStep(); }
            else if (m_iManNo == mc.VSNZ_Insp           ) {SEQ.VSNZ.InitCycleStep(); }
            else if (m_iManNo == mc.VSNZ_Next           ) {SEQ.VSNZ.InitCycleStep(); }
            
            else if (m_iManNo == mc.PSTB_Wait           ) {SEQ.PSTB.InitCycleStep(); }
            else if (m_iManNo == mc.PSTB_Stt            ) {SEQ.PSTB.InitCycleStep(); }
            else if (m_iManNo == mc.PSTB_In             ) {SEQ.PSTB.InitCycleStep(); }
            else if (m_iManNo == mc.PSTB_Work           ) {SEQ.PSTB.InitCycleStep(); }
            else if (m_iManNo == mc.PSTB_Out            ) {SEQ.PSTB.InitCycleStep(); }
            else if (m_iManNo == mc.PSTB_Next           ) {SEQ.PSTB.InitCycleStep(); }
            else if (m_iManNo == mc.PSTB_Replace        ) {SEQ.PSTB.InitCycleStep(); }
            
            else if (m_iManNo == mc.ULDR_Wait           ) {SEQ.ULDR.InitCycleStep(); }
            else if (m_iManNo == mc.ULDR_Supply         ) {SEQ.ULDR.InitCycleStep(); }
            else if (m_iManNo == mc.ULDR_Pick           ) {SEQ.ULDR.InitCycleStep(); }
            else if (m_iManNo == mc.ULDR_Work           ) {SEQ.ULDR.InitCycleStep(); }
            else if (m_iManNo == mc.ULDR_Drop           ) {SEQ.ULDR.InitCycleStep(); }
            
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
                bool r1 = SEQ.LODR.CycleHome();
                bool r2 = SEQ.PREB.CycleHome();
                bool r3 = SEQ.VSNZ.CycleHome();
                bool r4 = SEQ.PSTB.CycleHome();
                bool r5 = SEQ.ULDR.CycleHome();

                if(!r1 || !r2 || !r3 || !r4 || !r5  ) return ;
                if (ML.ER_IsErr()) { Init(); return; }
                m_iManNo = (int)mc.NoneCycle;
                Log.ShowMessage("Confirm", "All Homing Finished!");
                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.LODR_Home           ) { if (SEQ.LODR.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PREB_Home           ) { if (SEQ.PREB.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.HEAD_Home           ) { if (SEQ.VSNZ.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTB_Home           ) { if (SEQ.PSTB.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_Home           ) { if (SEQ.ULDR.CycleHome      ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.LODR_Wait           ) { if( SEQ.LODR.CycleWait()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LODR_Supply         ) { if( SEQ.LODR.CycleSupply()    ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LODR_Pick           ) { if( SEQ.LODR.CyclePick()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LODR_Work           ) { if( SEQ.LODR.CycleWork()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LODR_Drop           ) { if( SEQ.LODR.CycleDrop()      ) m_iManNo = mc.NoneCycle; } 
                                                                                       
            else if (m_iManNo == mc.VSNZ_Wait           ) { if( SEQ.VSNZ.CycleWait()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VSNZ_Stt            ) { if( SEQ.VSNZ.CycleWorkStart() ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VSNZ_Move           ) { if( SEQ.VSNZ.CycleMove()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VSNZ_Insp           ) { if( SEQ.VSNZ.CycleInsp()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.VSNZ_Next           ) { if( SEQ.VSNZ.CycleNext()      ) m_iManNo = mc.NoneCycle; }
                                                                                       
            else if (m_iManNo == mc.PSTB_Wait           ) { if( SEQ.PSTB.CycleWait(true)  ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTB_Stt            ) { if( SEQ.PSTB.CycleWait(false) ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTB_In             ) { if( SEQ.PSTB.CycleIn()        ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTB_Work           ) { if( SEQ.PSTB.CycleWork()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTB_Out            ) { if( SEQ.PSTB.CycleOut()       ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTB_Next           ) { if( SEQ.PSTB.CycleNext()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PSTB_Replace        ) { if( SEQ.PSTB.CycleReplace()   ) m_iManNo = mc.NoneCycle; }
            
            else if (m_iManNo == mc.ULDR_Wait           ) { if( SEQ.ULDR.CycleWait()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_Supply         ) { if( SEQ.ULDR.CycleSupply()    ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_Pick           ) { if( SEQ.ULDR.CyclePick()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_Work           ) { if( SEQ.ULDR.CycleWork()      ) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ULDR_Drop           ) { if( SEQ.ULDR.CycleDrop()      ) m_iManNo = mc.NoneCycle; }

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
