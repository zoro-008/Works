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
        public  static int             m_iWorkNo; 

        
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

            bool bPartHome = mc.LDR_CycleHome == _iNo ||
                             mc.PKR_CycleHome == _iNo ||
                             mc.QCB_CycleHome == _iNo ||
                             mc.SYR_CycleHome == _iNo ||
                             mc.CHA_CycleHome == _iNo ;
            bool bAllHome  = mc.AllHome == _iNo ;

            if ((!bAllHome && !bPartHome) && !SEQ.InspectHomeDone()) { return false; }

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


            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                ML.MT_SetServoAll(true);

                SEQ.LDR.InitHomeStep();
                SEQ.PKR.InitHomeStep();
                SEQ.QCB.InitHomeStep();
                SEQ.SYR.InitHomeStep();
                SEQ.CHA.InitHomeStep();

            }

            else if (m_iManNo == mc.LDR_CycleHome        ) { SEQ.LDR.InitHomeStep ();}
            else if (m_iManNo == mc.LDR_CycleOutPush     ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.LDR_CycleOut         ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.LDR_CycleMove        ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.LDR_CycleSupply      ) { SEQ.LDR.InitCycleStep();}                                                                                        
                                                                     
            else if (m_iManNo == mc.PKR_CycleHome        ) { SEQ.PKR.InitHomeStep ();}
            else if (m_iManNo == mc.PKR_CyclePickLdr     ) { SEQ.PKR.InitCycleStep();}
            else if (m_iManNo == mc.PKR_CycleShake       ) { SEQ.PKR.InitCycleStep();}
            else if (m_iManNo == mc.PKR_CyclePlceSut     ) { SEQ.PKR.InitCycleStep();}                                                                                       
            else if (m_iManNo == mc.PKR_CycleBarcode     ) { SEQ.PKR.InitCycleStep();}
            else if (m_iManNo == mc.PKR_CyclePickSut     ) { SEQ.PKR.InitCycleStep();}
            else if (m_iManNo == mc.PKR_CyclePlceLdr     ) { SEQ.PKR.InitCycleStep();}
                                                                    
            else if (m_iManNo == mc.QCB_CycleHome        ) { SEQ.QCB.InitHomeStep ();}
            else if (m_iManNo == mc.QCB_CycleRtPickFrg   ) { SEQ.QCB.InitCycleStep();}
            else if (m_iManNo == mc.QCB_CycleRtPlceBfr   ) { SEQ.QCB.InitCycleStep();}
            else if (m_iManNo == mc.QCB_CycleUnFreeze    ) { SEQ.QCB.InitCycleStep();}
            else if (m_iManNo == mc.QCB_CycleLtPickBfr   ) { SEQ.QCB.InitCycleStep();}
            else if (m_iManNo == mc.QCB_CycleLtPlceSut   ) { SEQ.QCB.InitCycleStep();}
            else if (m_iManNo == mc.QCB_CycleLtPickSut   ) { SEQ.QCB.InitCycleStep();}
            else if (m_iManNo == mc.QCB_CycleLtPlceBfr   ) { SEQ.QCB.InitCycleStep();}
            else if (m_iManNo == mc.QCB_CycleRtPickBfr   ) { SEQ.QCB.InitCycleStep();}
            else if (m_iManNo == mc.QCB_CycleRtPlceFrg   ) { SEQ.QCB.InitCycleStep();}
                                                                    
            else if (m_iManNo == mc.SYR_CycleHome        ) { SEQ.SYR.InitHomeStep ();}
            else if (m_iManNo == mc.SYR_CycleSuck        ) { SEQ.SYR.InitCycleStep();}
            else if (m_iManNo == mc.SYR_CycleSupply      ) { SEQ.SYR.InitCycleStep();}
            else if (m_iManNo == mc.SYR_CycleClean       ) { SEQ.SYR.InitCycleStep();}            
            else if (m_iManNo == mc.SYR_CycleReadyBlood  ) { SEQ.SYR.InitCycleStep();}            
                                                                
            else if (m_iManNo == mc.CHA_CycleHome        ) { SEQ.CHA.InitHomeStep ();}
            else if (m_iManNo == mc.CHA_CycleFillTank    ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleFillChamber ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleInspChamber ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleEmptyChamber) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleCleanChamber) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleReadyDC     ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleReadyFCM    ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleReadyNR     ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleReadyRET    ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleReady4DL    ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleTimeSupply  ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleCleanDcc    ) { SEQ.CHA.InitCycleStep();}
            else if (m_iManNo == mc.CHA_CycleInspDC      ) { SEQ.CHA.InitCycleStep();}


            else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.
            
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

                bool r1 = SEQ.PKR.CycleHome();
                bool r2 = SEQ.SYR.CycleHome();

                if(!r1 || !r2) return ;
                bool r3 = SEQ.LDR.CycleHome();
                bool r4 = SEQ.QCB.CycleHome();
                bool r5 = SEQ.CHA.CycleHome();


                if(!r3 || !r4 || !r5) return ;
                if (ML.ER_IsErr()) { Init(); return; }
                m_iManNo = (int)mc.NoneCycle;
                Log.ShowMessage("Confirm", "All Homing Finished!");
                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.LDR_CycleHome        ) { if (SEQ.LDR.CycleHome         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LDR_CycleOutPush     ) { if( SEQ.LDR.CycleOutPush      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LDR_CycleOut         ) { if( SEQ.LDR.CycleOut          ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LDR_CycleMove        ) { if( SEQ.LDR.CycleMove         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LDR_CycleSupply      ) { if( SEQ.LDR.CycleSupply       ()) m_iManNo = mc.NoneCycle; }                                                                                        
                                                                                           
            else if (m_iManNo == mc.PKR_CycleHome        ) { if( SEQ.PKR.CycleHome         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PKR_CyclePickLdr     ) { if( SEQ.PKR.CyclePickLdr      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PKR_CycleShake       ) { if( SEQ.PKR.CycleShake        ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PKR_CyclePlceSut     ) { if( SEQ.PKR.CyclePlceSut      ()) m_iManNo = mc.NoneCycle; }                                                                                       
            else if (m_iManNo == mc.PKR_CycleBarcode     ) { if( SEQ.PKR.CycleBarcode      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PKR_CyclePickSut     ) { if( SEQ.PKR.CyclePickSut      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.PKR_CyclePlceLdr     ) { if( SEQ.PKR.CyclePlceLdr      ()) m_iManNo = mc.NoneCycle; }
                                                                                           
            else if (m_iManNo == mc.QCB_CycleHome        ) { if( SEQ.QCB.CycleHome         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleRtPickFrg   ) { if( SEQ.QCB.CycleRtPickFrg    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleRtPlceBfr   ) { if( SEQ.QCB.CycleRtPlceBfr    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleUnFreeze    ) { if( SEQ.QCB.CycleUnFreeze     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleLtPickBfr   ) { if( SEQ.QCB.CycleLtPickBfr    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleLtPlceSut   ) { if( SEQ.QCB.CycleLtPlceSut    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleLtPickSut   ) { if( SEQ.QCB.CycleLtPickSut    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleLtPlceBfr   ) { if( SEQ.QCB.CycleLtPlceBfr    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleRtPickBfr   ) { if( SEQ.QCB.CycleRtPickBfr    ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.QCB_CycleRtPlceFrg   ) { if( SEQ.QCB.CycleRtPlceFrg    ()) m_iManNo = mc.NoneCycle; }
                                                                                           
            else if (m_iManNo == mc.SYR_CycleHome        ) { if( SEQ.SYR.CycleHome         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.SYR_CycleSuck        ) { if( SEQ.SYR.CycleSuck         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.SYR_CycleSupply      ) { if( SEQ.SYR.CycleSupply       ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.SYR_CycleClean       ) { if( SEQ.SYR.CycleClean        ()) m_iManNo = mc.NoneCycle; }            
            else if (m_iManNo == mc.SYR_CycleReadyBlood  ) { if( SEQ.SYR.CycleManReadyBlood()) m_iManNo = mc.NoneCycle; }            
                                                                                  
            else if (m_iManNo == mc.CHA_CycleHome        ) { if( SEQ.CHA.CycleHome         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleFillTank    ) { if( SEQ.CHA.CycleFillTank     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleFillChamber ) { if( SEQ.CHA.CycleFillChamber  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleInspChamber ) { if( SEQ.CHA.CycleInspChamber  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleEmptyChamber) { if( SEQ.CHA.CycleEmptyChamber ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleCleanChamber) { if (SEQ.CHA.CycleCleanChamber ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleReadyDC     ) { if( SEQ.CHA.CycleManReadyDC   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleReadyFCM    ) { if( SEQ.CHA.CycleManReadyFCM  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleReadyNR     ) { if( SEQ.CHA.CycleManReadyNR   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleReadyRET    ) { if( SEQ.CHA.CycleManReadyRET  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleReady4DL    ) { if( SEQ.CHA.CycleManReady4DL  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleTimeSupply  ) { if( SEQ.CHA.CycleManTimeSupply()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleCleanDcc    ) { if( SEQ.CHA.CycleCleanChamber ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CHA_CycleInspDC      ) { if( SEQ.CHA.CycleInspChamber  ()) m_iManNo = mc.NoneCycle; }

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
