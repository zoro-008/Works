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
using COMMON;
using SML2;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

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
        public  static int             m_iInspNo; 
        public  static int             m_iMovePos; 

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

            m_iInspNo = 0;

            

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

            //Check Alarm.
            //if (SM.ER.IsErr()) { Init(); return false; } //아 밑에 처리 하는 애 때문에 잠시 이렇게 함.          //test

            //    if(!IO_GetX(xETC_MainPower) ) {FM_MsgOk("ERR","Power On Plz");      return false ;} //test
            //if (!SM.IO.GetX((int)EN_INPUT_ID.xETC_MainAirChk)) { MessageBox.Show("Check Main Air!", "ERROR"); return false; } //test
            m_bStdCalPick = false;                                                                                                            //   mcLDR_FHome
            //   mcLDR_RHome
            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    //   mcIDX_Home
            m_iManNo = _iNo;     
            
            //   mcLTL_Home
            //   mcRTL_Home

            //SM.ER.SetDisp(true);

            bool bRet = true;                                                                                                                //   mcSTG_Home 
            //   mcULD_Home
            /********************/
            /* 프리프로세서     */
            /********************/
            if      (m_iManNo == mc.NoneCycle          ) { bRet = false; }
            else if (m_iManNo == mc.AllHome            ) { }
            else if (m_iManNo == mc.IDX_Home           ) { }
                                                       
                                                       
            else if (m_iManNo == mc.IDX_CycleWork      ) { }
            else if (m_iManNo == mc.IDX_CycleOut       ) { }
            //else if (m_iManNo == mc.IDX_CycleSupply    ) { }
            else if (m_iManNo == mc.IDX_CycleManSttWait) { }

            else if (m_iManNo == mc.IDX_HolderFwd      ) { }  
            else if (m_iManNo == mc.IDX_HolderBwd      ) { }
            else if (m_iManNo == mc.IDX_CutterFwd      ) { }
            else if (m_iManNo == mc.IDX_CutterBwd      ) { }

            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)
            {
                SM.MT_SetServoAll(true);
                SEQ.IDX.InitHomeStep();   
                //SEQ.PCK.InitHomeStep();
                //LTL.InitHomeStep();
                //RTL.InitHomeStep();
                //STG.InitHomeStep();
                //ULD.InitHomeStep();
            }
            else if (m_iManNo == mc.IDX_CycleWork      ) {SEQ.IDX.InitCycleStep(); }
            else if (m_iManNo == mc.IDX_CycleOut       ) {SEQ.IDX.InitCycleStep(); }
            else if (m_iManNo == mc.IDX_CycleManSttWait) {SEQ.IDX.InitCycleStep(); }

            else if (m_iManNo == mc.IDX_HolderFwd      ) {SM.CL_Move(ci.IDX_Hold1UpDn , fb.Fwd);  
                                                          SM.CL_Move(ci.IDX_Hold2UpDn , fb.Fwd); 
                                                          SM.CL_Move(ci.IDX_TwstLtDnUp, fb.Fwd); 
                                                          SM.CL_Move(ci.IDX_TwstRtDnUp, fb.Fwd); 
                                                         }
            else if (m_iManNo == mc.IDX_HolderBwd      ) {SM.CL_Move(ci.IDX_Hold1UpDn , fb.Bwd);  
                                                          SM.CL_Move(ci.IDX_Hold2UpDn , fb.Bwd); 
                                                          SM.CL_Move(ci.IDX_TwstLtDnUp, fb.Bwd); 
                                                          SM.CL_Move(ci.IDX_TwstRtDnUp, fb.Bwd); 
                                                         }
            else if (m_iManNo == mc.IDX_CutterFwd      ) {SM.CL_Move(ci.IDX_CutLtFwBw  , fb.Fwd); 
                                                          SM.CL_Move(ci.IDX_CutRtFwBw  , fb.Fwd); 
                                                          SM.CL_Move(ci.IDX_CutterDnUp , fb.Fwd);
                                                          SM.CL_Move(ci.IDX_CutBaseUpDn, fb.Fwd);
                                                         }
            else if (m_iManNo == mc.IDX_CutterBwd      ) {SM.CL_Move(ci.IDX_CutLtFwBw  , fb.Bwd); 
                                                          SM.CL_Move(ci.IDX_CutRtFwBw  , fb.Bwd); 
                                                          SM.CL_Move(ci.IDX_CutterDnUp , fb.Bwd);
                                                          SM.CL_Move(ci.IDX_CutBaseUpDn, fb.Bwd);
                                                         }

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

            if (m_iManNo != mc.AllHome && (m_iManNo != mc.IDX_Home)) SEQ.InspectHomeDone();
            
            SEQ.InspectLightGrid();

            bool r1, r2;//, r3, r4, r5, r6, r7, r8, r9;
            r1 = r2 = false;// r3 = r4 = r5 = r6 = r7 = r8 = r9 = false;

            //Check Alarm.
            if (SML.ER.IsErr()) { Init(); return; }             //test
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                r1 = SEQ.IDX.CycleHome();

                if (!r1) return;

                m_iManNo = (int)mc.NoneCycle;
                
                Log.ShowMessage("Confirm", "All Home Finished!");

                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.IDX_Home            ) { if (SEQ.IDX.CycleHome      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDX_CycleWork       ) { if (SEQ.IDX.CycleWork      ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDX_CycleOut        ) { if (SEQ.IDX.CycleOut       ()) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.IDX_CycleManSttWait ) { if (SEQ.IDX.CycleManSttWait()) m_iManNo = mc.NoneCycle; }

            else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

            //Ok.
            return;
        }
    }
}
