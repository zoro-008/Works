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
        public static mc m_iCrntManNo   ;

        public static void Init()
        {
            
            m_iManStep    = 0;
            m_iPreManStep = 0;
            m_iManNo      = mc.NoneCycle;

            m_bManSetting = false;

            m_tmCycle.Clear();
            m_tmDelay.Clear();
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
            if (_iNo > mc.HomeEnd  && !SEQ.InspectHomeDone() ) {                                                 return false; }

            //   mcLDR_RHome
            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    
            m_iManNo = _iNo;     
           

            bool bRet = true;                                                                                                        
            //   mcULD_Home
            /********************/ 
            /********************/
            if      (m_iManNo == mc.NoneCycle         ) { bRet = false; }
            else if (m_iManNo == mc.AllHome           ) { }

            

            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                ML.MT_SetServoAll(true);
                SEQ.WRK.InitHomeStep();
                SEQ.OUT.InitHomeStep();
                
            }
            else if (m_iManNo == mc.StepCut           ) {m_iManStep = 10 ; }
            else if (m_iManNo == mc.Move5mm           ) {m_iManStep = 10 ; }
            else if (m_iManNo == mc.Move100mm         ) {m_iManStep = 10 ; }
                            
            //else if (m_iManNo == mc.XRAY_CycleAging       ) {SEQ.XRAY.InitCycleStep();                        }
                                               
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
                r1 = SEQ.WRK.CycleHome();
                r2 = SEQ.OUT.CycleHome();
                if (!r1 || !r2 ) { return; }                
                m_iManNo = (int)mc.NoneCycle;
                
                Log.ShowMessage("Confirm", "All Home Finished!");

                m_iCrntManNo = m_iManNo;
            }

                                                            

            else if (m_iManNo == mc.StepCut       ) { if (CycleManWork   (   )) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.Move5mm       ) { if (CycleManMove   (5  )) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.Move100mm     ) { if (CycleManMove   (10 )) m_iManNo = mc.NoneCycle; }

            else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

            //Ok.
            return;
        }

        public static bool CycleManWork()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(m_iManStep != 0 && m_iManStep == m_iPreManStep && !OM.MstOptn.bDebugMode, 10000))
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

            
            bool bRet1 = false ;
            bool bRet2 = false ;

            m_iPreManStep = m_iManStep;

            switch (m_iManStep) {
      
                default: sTemp = string.Format("Cycle Default Clear Manual m_iPreManStep ={0:00}", m_iPreManStep);
                         m_iManStep = 0 ;
                         return true ;


                case 10:
                    SEQ.WRK.InitCycleStep();
                    SEQ.OUT.InitCycleStep();
                    m_iManStep++;
                    return false ;

                case 11:
                    bRet1 = SEQ.WRK.CycleWork();
                    bRet2 = SEQ.OUT.CycleWind();

                    if(!bRet1 || !bRet2) return false ;

                    m_iManStep = 0;
                    return true;


            }
        }


        public static bool CycleManMove(double _dDist)
        {
            string sTemp;
            if (m_tmCycle.OnDelay(m_iManStep != 0 && m_iManStep == m_iPreManStep && !OM.MstOptn.bDebugMode, 10000))
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

            
            bool bRet1 = false ;
            bool bRet2 = false ;

            m_iPreManStep = m_iManStep;

            switch (m_iManStep) {
      
                default: sTemp = string.Format("Cycle Default Clear Manual m_iPreManStep ={0:00}", m_iPreManStep);
                         m_iManStep = 0 ;
                         return true ;


                case 10:
                    SEQ.WRK.InitCycleStep();
                    SEQ.OUT.InitCycleStep();
                    m_iManStep++;
                    return false ;

                case 11:
                    bRet1 = SEQ.WRK.CycleManMove(_dDist);
                    bRet2 = SEQ.OUT.CycleWind();

                    if(!bRet1 || !bRet2) return false ;

                    m_iManStep = 0;
                    return true;


            }
        }




    }


}
