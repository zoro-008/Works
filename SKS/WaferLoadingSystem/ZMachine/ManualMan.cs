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

            if (_iNo     < 0                                 ) { Log.ShowMessage("ERROR", "Wrong Manual No"   ,3000); return false; }
            if (_iNo     >= mc.MAX_MANUAL_CYCLE              ) { Log.ShowMessage("ERROR", "Wrong Manual No"   ,3000); return false; }
            if (m_iManNo != mc.NoneCycle                     ) { Log.ShowMessage("ERROR", "Doing Manual Cycle",3000); return false; }
            if (SEQ._bRun                                    ) { Log.ShowMessage("ERROR", "Autorunning"       ,3000); return false; }
            if (ML.ER_IsErr()                                ) { return false; }

            bool bPartHome = mc.LDR_CycleHome == _iNo ;
            bool bAllHome  = mc.AllHome       == _iNo ;

            if ((!bAllHome && !bPartHome) && !SEQ.InspectHomeDone()) { return false; }

            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    //   mcIDX_Home
            m_iManNo = _iNo;     
            
            //   mcLTL_Home
            //   mcRTL_Home

            //ML.ER.SetDisp(true);jinseop

            bool bRet = true;                                                                                                                //   mcSTG_Home 

            /********************/
            /********************/
            if (!ML.IO_GetX(xi.ManualInspLimit))
            {
                ML.ER_SetErr(ei.LDR_ManualInspLimit);
                bRet = false;
            }

            if      (m_iManNo == mc.NoneCycle         ) { bRet = false; }
            //자재 상태 확인
            else if (m_iManNo == mc.CycleWait         ) { }
            else if (m_iManNo == mc.CycleLoading      ) {if(!CheckStatus())                               bRet = false; } //전부 검사
            else if (m_iManNo == mc.CycleUnLoading    ) {if(!CheckStatus())                               bRet = false; } //전부 검사
            else if (m_iManNo == mc.LoadingPick       ) {if(!CheckStatus(ri.CST) || !CheckStatus(ri.PCK)) bRet = false; }
            else if (m_iManNo == mc.LoadingPlace      ) {if(!CheckStatus(ri.STG) || !CheckStatus(ri.PCK)) bRet = false; }
            else if (m_iManNo == mc.StageDown         ) { }
            else if (m_iManNo == mc.StageUp           ) { }
            else if (m_iManNo == mc.UnloadingPick     ) {if(!CheckStatus(ri.STG) || !CheckStatus(ri.PCK)) bRet = false; }
            else if (m_iManNo == mc.UnloadingPlace    ) {if(!CheckStatus(ri.CST) || !CheckStatus(ri.PCK)) bRet = false; }
            else if (m_iManNo == mc.CycleIdle         ) {if(!OM.MstOptn.bIdleRun)                         bRet = false; }
                    
            else if (m_iManNo == mc.AllHome             ) { }


            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                ML.MT_SetServoAll(true);

                SEQ.LDR.InitHomeStep();

            }

            else if (m_iManNo == mc.LDR_CycleHome     ) { SEQ.LDR.InitHomeStep ();}

            else if (m_iManNo == mc.CycleWait         ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.CycleLoading      ) { SEQ.LDR.InitCycleLoading  ();}
            else if (m_iManNo == mc.CycleUnLoading    ) { SEQ.LDR.InitCycleUnLoading();}
            else if (m_iManNo == mc.LoadingPick       ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.LoadingPlace      ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.StageDown         ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.StageUp           ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.UnloadingPick     ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.UnloadingPlace    ) { SEQ.LDR.InitCycleStep();}
            else if (m_iManNo == mc.CycleIdle         ) { SEQ.LDR.InitCycleLoading(); SEQ.LDR.InitCycleUnLoading();}

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

        public static bool bPreStatus = false;
        public static void Update()
        {
            if (m_iManNo == mc.NoneCycle) {
                Stop = false;
                return;
            }
            if (m_bManSetting) return;

            //if (m_iManNo != mc.AllHome && m_iManNo != mc.SLD_Home ) SEQ.InspectHomeDone();   

            if(!OM.MstOptn.bDebugMode) SEQ.InspectLightGrid();

            //Check Alarm. //에러 띄우고 좀더 돌려야 해서 제거 함
            //if (ML.ER_IsErr()) { Init(); return; }

            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                if(!SEQ.LDR.CycleHome()) return ;

                if (ML.ER_IsErr()) { Init(); return; }

                m_iManNo = (int)mc.NoneCycle;
                Log.ShowMessage("Confirm", "All Homing Finished!",3000);
                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.LDR_CycleHome        ) { if (SEQ.LDR.CycleHome         ()) m_iManNo = mc.NoneCycle; }
            //else if (m_iManNo == mc.LDR_CycleLoading     ) { if( SEQ.LDR.CycleLoading      ()) m_iManNo = mc.NoneCycle; }
            //else if (m_iManNo == mc.LDR_CycleUnLoading   ) { if( SEQ.LDR.CycleUnloading    ()) m_iManNo = mc.NoneCycle; }
            //else if (m_iManNo == mc.STG_Up               ) { if( SEQ.LDR.CycleStgUp        ()) m_iManNo = mc.NoneCycle; }
            //else if (m_iManNo == mc.STG_Down             ) { if( SEQ.LDR.CycleStgDn        ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.CycleWait           ) { if( SEQ.LDR.CycleWait          ()) m_iManNo = mc.NoneCycle; }     
            else if (m_iManNo == mc.CycleLoading        ) { 
                if( SEQ.LDR.CycleLoading       ()) {
                    m_iManNo = mc.NoneCycle; 
                    if(OM.MstOptn.bIdleRun && !Stop) {SetManCycle(mc.CycleIdle); bPreStatus = true; }
                }
            }     
            else if (m_iManNo == mc.CycleUnLoading      ) { 
                if(SEQ.LDR.CycleUnloading     ()) 
                {
                    m_iManNo = mc.NoneCycle;
                    if (OM.CmnOptn.bLoadAtUnload && SEQ.LDR.bUnloadingEnd && DM.ARAY[ri.CST].GetCntStat(cs.Unkn) > 0)
                    {
                        SetManCycle(mc.CycleLoading);
                    }
                    if(OM.MstOptn.bIdleRun && !Stop) {
                        SetManCycle(mc.CycleIdle); bPreStatus = false;
                        if (DM.ARAY[ri.CST].CheckAllStat(cs.Work))
                        {
                            DM.ARAY[ri.CST].SetStat(cs.Unkn);
                        }
                    }  
                }
            }
                    
            else if (m_iManNo == mc.LoadingPick         ) { if( SEQ.LDR.CycleLoadingPick   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.LoadingPlace        ) { if( SEQ.LDR.CycleLoadingPlace  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.StageDown           ) { if( SEQ.LDR.CycleStgDn         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.StageUp             ) { if( SEQ.LDR.CycleStgUp         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.UnloadingPick       ) { if( SEQ.LDR.CycleUnloadingPick ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.UnloadingPlace      ) { if( SEQ.LDR.CycleUnloadingPlace()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.CycleIdle           ) { 
                if(ML.ER_IsErr()) m_iManNo = mc.NoneCycle;
                if(  bPreStatus) { m_iManNo = mc.NoneCycle; SetManCycle(mc.CycleUnLoading); }
                else             { m_iManNo = mc.NoneCycle; SetManCycle(mc.CycleLoading  ); }
            }
            

            else   { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

            

            //Ok.
            return;
        }

        public static bool CheckStatus(int iId = -1)
        {
            //자재 보여주기
            bool bPckNone =  DM.ARAY[ri.PCK].CheckAllStat(cs.None) ;//&& !ML.IO_GetX(xi.WaferVacuum); 
            bool bStgNone =  DM.ARAY[ri.STG].CheckAllStat(cs.None) ;//&& !ML.IO_GetX(xi.WaferDtSsr ); 
            bool bCstNone =  DM.ARAY[ri.CST].CheckAllStat(cs.None) ;//ML.IO_GetX(xi.CassetteLeft) &&  ML.IO_GetX(xi.CassetteRight); 

            bool bDisp1 = !bPckNone && !ML.IO_GetX(xi.PickerVacuum) ;
            bool bUnkn1 =  bPckNone &&  ML.IO_GetX(xi.PickerVacuum) ;

            bool bDisp2 = !bStgNone && !ML.IO_GetX(xi.WaferDtSsr ) ;
            bool bUnkn2 =  bStgNone &&  ML.IO_GetX(xi.WaferDtSsr ) ;

            bool bDisp3 = !bCstNone && (!ML.IO_GetX(xi.CassetteLeft) || !ML.IO_GetX(xi.CassetteRight)) ;
            bool bUnkn3 =  bCstNone && ( ML.IO_GetX(xi.CassetteLeft) ||  ML.IO_GetX(xi.CassetteRight)) ;

            bool bRet = true;
            if(iId == (int)ri.PCK || iId == -1)
            {
                if(bDisp1) { ML.ER_SetErr(ei.PKG_Dispr ,"피커에 자재가 있는데 베큠 센서 감지 안됨"           ) ; bRet = false; }
                if(bUnkn1) { ML.ER_SetErr(ei.PKG_Unknwn,"피커에 자재가 없는데 베큠 센서 감지됨"              ) ; bRet = false; }
            }
            if(iId == (int)ri.STG || iId == -1)
            {
                if(bDisp2) { ML.ER_SetErr(ei.PKG_Dispr ,"스테이지에 자재가 있는데 웨이퍼 감지 센서 감지 안됨") ; bRet = false; }
                if(bUnkn2) { ML.ER_SetErr(ei.PKG_Unknwn,"스테이지에 자재가 없는데 웨이퍼 감지 센서 감지됨"   ) ; bRet = false; }
            }
            if(iId == (int)ri.CST || iId == -1)
            {
                if(bDisp3) { ML.ER_SetErr(ei.PKG_Dispr ,"카세트가 있는데 카세트 감지 센서 감지 안됨"         ) ; bRet = false; }
                if(bUnkn3) { ML.ER_SetErr(ei.PKG_Unknwn,"카세트가 없는데 카세트 감지 센서 감지됨"            ) ; bRet = false; }

                //카세트 삐딱체크
                bool bCstAlign=  ML.IO_GetX(xi.CassetteLeft) && !ML.IO_GetX(xi.CassetteRight) || 
                                !ML.IO_GetX(xi.CassetteLeft) &&  ML.IO_GetX(xi.CassetteRight) ;
                if(bCstAlign) { ML.ER_SetErr(ei.LDR_CstLocationErr); bRet = false; }//카세트를 잘 놓으시요!!! 
            }

            return bRet;
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
