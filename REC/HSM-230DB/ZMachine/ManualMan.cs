using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using SML2;
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
            if (_iNo > mc.SSTG_Home && !SEQ.InspectHomeDone()) {                                                 return false; }

            //LOL if (!SML.IO.GetX((int)xi.ETC_MainAir)            ) { Log.ShowMessage("ERROR", "Main Air is Not Supply"); return false; }

            //Check Alarm.
            //LOL if (SML.ER.IsErr()) { Init(); return false; } //아 밑에 처리 하는 애 때문에 잠시 이렇게 함.          //test

            //    if(!IO_GetX(xETC_MainPower) ) {FM_MsgOk("ERR","Power On Plz");      return false ;} //test
            //if (!SM.IO.GetX((int)EN_INPUT_ID.xETC_MainAirChk)) { MessageBox.Show("Check Main Air!", "ERROR"); return false; } //test
            //   mcLDR_RHome
            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    //   mcIDX_Home
            m_iManNo = _iNo;     
            
            //   mcLTL_Home
            //   mcRTL_Home

            //ER_SetDisp(true);jinseop

            bool bRet = true;                                                                                                                //   mcSTG_Home 
            //   mcULD_Home
            /********************/ 
            /********************/
            if      (m_iManNo == mc.NoneCycle         ) { bRet = false; }
            else if (m_iManNo == mc.AllHome           ) { }
            else if (m_iManNo == mc.TOOL_Home         ) { }
            else if (m_iManNo == mc.WSTG_Home         ) { }
            else if (m_iManNo == mc.SSTG_Home         ) { }
            else if (m_iManNo == mc.TOOL_SubsAlignVisn) { }
            else if (m_iManNo == mc.TOOL_HghtCheck    ) { }
            else if (m_iManNo == mc.TOOL_PckrCheck    ) { }
            else if (m_iManNo == mc.ETC_ConvPos       ) { }

            else if (m_iManNo == mc.WSTG_WaferOut      ) { if(DM.ARAY[ri.WLDB].GetCntStat(cs.Mask)==0 && DM.ARAY[ri.WLDT].GetCntStat(cs.Mask)==0) {Log.ShowMessage("Error" ,"웨이퍼로더에 마스크 슬롯이 없습니다."); bRet=false;}}
            else if (m_iManNo == mc.SSTG_SubsOut       ) { if(DM.ARAY[ri.SLDB].GetCntStat(cs.Mask)==0 && DM.ARAY[ri.SLDT].GetCntStat(cs.Mask)==0) {Log.ShowMessage("Error" ,"서브스트레이터로더에 마스크 슬롯이 없습니다."); bRet=false;}}

            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)            {
                SML.MT.SetServoAll(true);
                SEQ.WSTG.InitHomeStep();
                SEQ.SSTG.InitHomeStep();
                SEQ.TOOL.InitHomeStep();
            }
            else if (m_iManNo == mc.TOOL_Home          ) {SML.MT.SetServoAll(true);SEQ.TOOL.InitHomeStep(); }
            else if (m_iManNo == mc.WSTG_Home          ) {SML.MT.SetServoAll(true);SEQ.WSTG.InitHomeStep(); }
            else if (m_iManNo == mc.SSTG_Home          ) {SML.MT.SetServoAll(true);SEQ.SSTG.InitHomeStep(); }
            else if (m_iManNo == mc.TOOL_SubsAlignVisn ) { SEQ.TOOL.InitCycleStep(); }
            else if (m_iManNo == mc.TOOL_WafrAlignVisn ) { SEQ.TOOL.InitCycleStep(); }
            else if (m_iManNo == mc.WSTG_WaferGet      ) { SEQ.WSTG.InitCycleStep(); }
            else if (m_iManNo == mc.TOOL_Eject         ) { SEQ.TOOL.InitCycleStep(); }
            else if (m_iManNo == mc.TOOL_DispCheck     ) { SEQ.TOOL.InitCycleStep(); }
            else if (m_iManNo == mc.TOOL_HghtCheck     ) { SEQ.TOOL.InitCycleStep(); }
            else if (m_iManNo == mc.WSTG_ExpdWork      ) { SML.MT.GoAbsRun((int)mi.WSTG_ZExpd, PM.GetValue(mi.WSTG_ZExpd, pv.WSTG_ZExpdWork));}
            else if (m_iManNo == mc.SSTG_SubsRailConv  ) { SEQ.SSTG.InitCycleStep(); }
            else if (m_iManNo == mc.SSTG_WafrRailConv  ) { SEQ.WSTG.InitCycleStep(); }
            else if (m_iManNo == mc.TOOL_PckrCheck     ) { SEQ.TOOL.InitCycleStep(); }
            else if (m_iManNo == mc.ETC_ConvPos        ) { SEQ.TOOL.InitCycleStep(); }

            else if (m_iManNo == mc.WSTG_WaferOut      ) { SEQ.WSTG.InitCycleStep(); }
            else if (m_iManNo == mc.SSTG_SubsOut       ) { SEQ.SSTG.InitCycleStep(); }
            
            

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

            bool r1, r2 , r3;
            r1 = r2 = r3 = false;

            //Check Alarm.
            if (SML.ER.IsErr()) { Init(); return; }
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                r1 = SEQ.TOOL.CycleHome();
                if(!r1) return ;
                r2 = SEQ.WSTG.CycleHome();
                r3 = SEQ.SSTG.CycleHome();

                if (!r1 || !r2 || !r3) { return; }                
                m_iManNo = (int)mc.NoneCycle;
                
                Log.ShowMessage("Confirm", "All Home Finished!");

                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.TOOL_Home          ) { if (SEQ.TOOL.CycleHome()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.WSTG_Home          ) { if (SEQ.WSTG.CycleHome()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.SSTG_Home          ) { if (SEQ.SSTG.CycleHome()) m_iManNo = mc.NoneCycle; }
                                                       
            else if (m_iManNo == mc.TOOL_SubsAlignVisn ) { if (SEQ.TOOL.CycleSubsAlignVisn ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_WafrAlignVisn ) { if (SEQ.TOOL.CycleDieAlignVisn  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.WSTG_WaferGet      ) { if (SEQ.WSTG.CycleGet           ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_Eject         ) { if (SEQ.TOOL.CycleEject         ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_DispCheck     ) { if (SEQ.TOOL.CycleDispCheck     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_HghtCheck     ) { if (SEQ.TOOL.CycleHghtCheck     ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.SSTG_SubsRailConv  ) { if (SEQ.SSTG.CycleSubsRailConv  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.SSTG_WafrRailConv  ) { if (SEQ.WSTG.CycleWafrRailConv  ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.TOOL_PckrCheck     ) { if (SEQ.TOOL.CyclePickerCheck   ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ETC_ConvPos        ) { if (SEQ.TOOL.CycleConvPos       ()) m_iManNo = mc.NoneCycle; }

            else if (m_iManNo == mc.WSTG_WaferOut      ) { if (SEQ.WSTG.CycleOut           ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.SSTG_SubsOut       ) { if (SEQ.SSTG.CycleOut           ()) m_iManNo = mc.NoneCycle; }
            
            //else if (m_iManNo == mc.IDX_Home         ) {if(SEQ.IDX.CycleHome      ()) m_iManNo = mc.NoneCycle ;}
            //else if (m_iManNo == mc.PCK_Home         ) {if(SEQ.PCK.CycleHome      ()) m_iManNo = mc.NoneCycle ;}
                                                                                    
            //else if (m_iManNo == mc.PCK_CyclePick    ) {if(SEQ.PCK.CyclePick      ()) m_iManNo = mc.NoneCycle ;}
            //else if (m_iManNo == mc.PCK_CycleVisn    ) {if(SEQ.PCK.CycleVsn       ()) m_iManNo = mc.NoneCycle ;}
            //else if (m_iManNo == mc.PCK_CyclePrnt    ) {if(SEQ.PCK.CyclePrnt      ()) m_iManNo = mc.NoneCycle ;}
            //else if (m_iManNo == mc.PCK_CyclePlce    ) {if(SEQ.PCK.CyclePlce      ()) m_iManNo = mc.NoneCycle ;}
                                                                                    
            //else if (m_iManNo == mc.IDX_CycleWork    ) {if(SEQ.IDX.CycleWork      ()) m_iManNo = mc.NoneCycle ;}
            //else if (m_iManNo == mc.IDX_CycleOut     ) {if(SEQ.IDX.CycleOut       ()) m_iManNo = mc.NoneCycle ;}
            

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
