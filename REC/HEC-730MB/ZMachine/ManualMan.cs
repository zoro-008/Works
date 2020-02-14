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
            if (SEQ._bRun && _iNo != mc.RIGH_Rest            ) { Log.ShowMessage("ERROR", "Autorunning"       ); return false; }
            //if (!SEQ.InspectHomeDone()                       ) {                                                 return false; }
            //if (_iNo > mc.STCK_Home && !SEQ.InspectHomeDone()) {                                                 return false; }

            //LOL if (!SML.IO.GetX((int)xi.ETC_MainAir)            ) { Log.ShowMessage("ERROR", "Main Air is Not Supply"); return false; }

            //Check Alarm.
            //LOL if (SML.ER.IsErr()) { Init(); return false; } //아 밑에 처리 하는 애 때문에 잠시 이렇게 함.          //test

            //    if(!IO_GetX(xETC_MainPower) ) {FM_MsgOk("ERR","Power On Plz");      return false ;} //test
            //if (!ML.IO.GetX((int)EN_INPUT_ID.xETC_MainAirChk)) { MessageBox.Show("Check Main Air!", "ERROR"); return false; } //test
            //   mcLDR_RHome
            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    //   mcIDX_Home
            m_iManNo = _iNo;

            bool bRet = true;   

            if (m_iManNo != mc.AllHome   &&
                m_iManNo != mc.LEFT_Home &&
                m_iManNo != mc.RIGH_Home &&
                !SEQ.InspectHomeDone()) { bRet = false; }
            //   mcLTL_Home
            //   mcRTL_Home

            //ML.ER.SetDisp(true);jinseop

                                                                                                             //   mcSTG_Home 
            //   mcULD_Home
            /********************/ 
            /********************/
            if      (m_iManNo == mc.NoneCycle         ) { bRet = false; }
            else if (m_iManNo == mc.AllHome           ) { }
            else if (m_iManNo == mc.LEFT_Home         ) { }
            else if (m_iManNo == mc.RIGH_Home         ) { }                        

            else if (m_iManNo == mc.LEFT_Wait         ) { }                                            
            else if (m_iManNo == mc.LEFT_Zero         ) { }                                            
            else if (m_iManNo == mc.LEFT_Once         ) { }                                            
            else if (m_iManNo == mc.LEFT_Work         ) { }                                            
            else if (m_iManNo == mc.LEFT_Rest         ) { }                                            

            else if (m_iManNo == mc.RIGH_Wait         ) { }                                            
            else if (m_iManNo == mc.RIGH_Zero         ) { }                                            
            else if (m_iManNo == mc.RIGH_Once         ) { }                                            
            else if (m_iManNo == mc.RIGH_Work         ) { }                                            
            else if (m_iManNo == mc.RIGH_Rest         ) { }                                            


            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/
            SEQ.LEFT.Stat.bReqStop = false;
            SEQ.RIGH.Stat.bReqStop = false;

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                ML.MT_SetServoAll(true);
                SEQ.LEFT.InitHomeStep();
                SEQ.RIGH.InitHomeStep();
            }
            else if (m_iManNo == mc.LEFT_Home         ) {SEQ.LEFT.InitHomeStep(); }
            else if (m_iManNo == mc.RIGH_Home         ) {SEQ.RIGH.InitHomeStep(); }

            else if (m_iManNo == mc.LEFT_Wait         ) {SEQ.LEFT.InitCycleStep(); } 
            else if (m_iManNo == mc.LEFT_Zero         ) {SEQ.LEFT.InitCycleStep(); } 
            else if (m_iManNo == mc.LEFT_Once         ) {SEQ.LEFT.InitCycleStep(); } 
            else if (m_iManNo == mc.LEFT_Work         ) {SEQ.LEFT.InitCycleStep(); } 
            else if (m_iManNo == mc.LEFT_Rest         ) {SEQ.LEFT.InitCycleStep(); } 

            else if (m_iManNo == mc.RIGH_Wait         ) {SEQ.RIGH.InitCycleStep(); } 
            else if (m_iManNo == mc.RIGH_Zero         ) {SEQ.RIGH.InitCycleStep(); } 
            else if (m_iManNo == mc.RIGH_Once         ) {SEQ.RIGH.InitCycleStep(); } 
            else if (m_iManNo == mc.RIGH_Work         ) {SEQ.RIGH.InitCycleStep(); } 
            else if (m_iManNo == mc.RIGH_Rest         ) {SEQ.RIGH.InitCycleStep(); }                                       

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
            double dKg1 = 0;
            double dKg2 = 0;
            if (OM.DevInfo.iL_Mode == (int)Mode.Weight) dKg1 = OM.DevInfo.dL_W_Weight;
            if (OM.DevInfo.iR_Mode == (int)Mode.Weight) dKg2 = OM.DevInfo.dR_W_Weight;
            //Check Alarm.
            if (ML.ER_IsErr()) { Init(); return; }
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                r1 = SEQ.LEFT.CycleHome();
                r2 = SEQ.RIGH.CycleHome();

                if (!r1 || !r2) { return; }                
                m_iManNo = (int)mc.NoneCycle;
                
                Log.ShowMessage("Confirm", "All Home Finished!");

                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.LEFT_Home         ) { if (SEQ.LEFT.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.RIGH_Home         ) { if (SEQ.RIGH.CycleHome      ()) m_iManNo = mc.NoneCycle; }
             
            else if (m_iManNo == mc.LEFT_Wait         ) { if (SEQ.LEFT.CycleWait      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LEFT_Zero         ) { if (SEQ.LEFT.CycleZero      (dKg1)) m_iManNo = mc.NoneCycle; }
            
            else if (m_iManNo == mc.LEFT_Once         )
            {
                if (OM.DevInfo.iL_Mode == (int)Mode.Height) { if (SEQ.LEFT.CycleWorkH(1)) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iL_Mode == (int)Mode.Weight) { if (SEQ.LEFT.CycleWorkW(1)) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iL_Mode == (int)Mode.Pull_Dest) { if (SEQ.LEFT.CycleWorkD()) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iL_Mode == (int)Mode.GripH    ) { if (SEQ.LEFT.CycleWorkG(1)) m_iManNo = mc.NoneCycle; }
            }
            else if (m_iManNo == mc.LEFT_Work         )
            {
                if (OM.DevInfo.iL_Mode == (int)Mode.Height) { if (SEQ.LEFT.CycleWorkH(OM.DevInfo.iL_H_Count)) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iL_Mode == (int)Mode.Weight) { if (SEQ.LEFT.CycleWorkW(OM.DevInfo.iL_W_Count)) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iL_Mode == (int)Mode.Pull_Dest) { if (SEQ.LEFT.CycleWorkD()) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iL_Mode == (int)Mode.GripH ) { if (SEQ.LEFT.CycleWorkG(OM.DevInfo.iL_G_Count)) m_iManNo = mc.NoneCycle; }
            }
            else if (m_iManNo == mc.LEFT_Rest         ) { SEQ.LEFT.iWorkCnt = 0; m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.RIGH_Wait         ) { if (SEQ.RIGH.CycleWait      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.RIGH_Zero         ) { if (SEQ.RIGH.CycleZero      (dKg2)) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.RIGH_Once         ) 
            {
                if (OM.DevInfo.iR_Mode == (int)Mode.Height) { if (SEQ.RIGH.CycleWorkH(1)) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iR_Mode == (int)Mode.Weight) { if (SEQ.RIGH.CycleWorkW(1)) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iR_Mode == (int)Mode.Pull_Dest) { if (SEQ.RIGH.CycleWorkP(1)) m_iManNo = mc.NoneCycle; }
                //if (OM.DevInfo.iR_Mode == (int)Mode.GripH ) { if (SEQ.RIGH.CycleWorkG(1)) m_iManNo = mc.NoneCycle; }
            }
            else if (m_iManNo == mc.RIGH_Work         ) 
            {
                if (OM.DevInfo.iR_Mode == (int)Mode.Height) { if (SEQ.RIGH.CycleWorkH(OM.DevInfo.iR_H_Count)) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iR_Mode == (int)Mode.Weight) { if (SEQ.RIGH.CycleWorkW(OM.DevInfo.iR_W_Count)) m_iManNo = mc.NoneCycle; }
                if (OM.DevInfo.iR_Mode == (int)Mode.Pull_Dest) { if (SEQ.RIGH.CycleWorkP(OM.DevInfo.iR_P_Count)) m_iManNo = mc.NoneCycle; }
                //if (OM.DevInfo.iR_Mode == (int)Mode.GripH ) { if (SEQ.RIGH.CycleWorkG(OM.DevInfo.iR_G_Count)) m_iManNo = mc.NoneCycle; }
            }
            else if (m_iManNo == mc.RIGH_Rest         ) { SEQ.RIGH.iWorkCnt = 0; m_iManNo = mc.NoneCycle; }

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
