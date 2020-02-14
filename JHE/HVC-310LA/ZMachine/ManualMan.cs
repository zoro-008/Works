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
using SMDll2;
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
        public  static int             m_iInspType   ;

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
            if (SM.ER.IsErr()) { Init(); return false; } //아 밑에 처리 하는 애 때문에 잠시 이렇게 함.          //test

            //    if(!IO_GetX(xETC_MainPower) ) {FM_MsgOk("ERR","Power On Plz");      return false ;} //test
            //if (!SM.IO.GetX((int)EN_INPUT_ID.xETC_MainAirChk)) { MessageBox.Show("Check Main Air!", "ERROR"); return false; } //test
            m_bStdCalPick = false;                                                                                                            //   mcLDR_FHome
            //   mcLDR_RHome
            m_bManSetting = true; //SetManCycle함수는 화면 쓰레드. 업데이트 함수에서 다른쓰레드로 들어와서 갱신하기에 플레그 걸어 막아둠.    //   mcIDX_Home
            m_iManNo = _iNo;     
            
            //   mcLTL_Home
            //   mcRTL_Home

            SM.ER.SetDisp(true);

            bool bRet = true;                                                                                                                //   mcSTG_Home 
            //   mcULD_Home
            /********************/
            /* 프리프로세서     */
            /********************/
            if      (m_iManNo == mc.NoneCycle      ) { bRet = false; }
            else if (m_iManNo == mc.AllHome        ) { }
            else if (m_iManNo == mc.ASY_Home       ) { }


            else if (m_iManNo == mc.ASY_CycleToolCalib    ) { }
            else if (m_iManNo == mc.ASY_CycleTorqueCheck  ) { }
            else if (m_iManNo == mc.ASY_CycleInsp         ) { }
            else if (m_iManNo == mc.ASY_CycleTrayOut      ) { }
            else if (m_iManNo == mc.ASY_CycleTrayIn       ) { }
            else if (m_iManNo == mc.ASY_CycleHoldrCalib   ) { }
            else if (m_iManNo == mc.ASY_CycleToolEccentric) { }

            //else if (m_iManNo == mc.SLD_Move       ) { }

            //else if (m_iManNo == mc.SLD_MotrRepeat ) { }//테스트용
            //else if (m_iManNo == mc.SLD_PbSplyOnOff) { }

            //else if (m_iManNo == mc.SLD_SolderTest) { }


            if (!bRet) Init();

            /********************/
            /* 처리..           */
            /********************/

            if (m_iManNo == mc.NoneCycle) { }
            else if (m_iManNo == mc.AllHome)
            {
                SM.MT.SetServoAll(true);
                SEQ.ASY.InitHomeStep();   
                //SEQ.IDX.InitHomeStep();
                //SEQ.PCK.InitHomeStep();
                //LTL.InitHomeStep();
                //RTL.InitHomeStep();
                //STG.InitHomeStep();
                //ULD.InitHomeStep();
            }
            else if (m_iManNo == mc.ASY_CycleToolCalib    ) { SEQ.ASY.InitCycleStep(); }
            else if (m_iManNo == mc.ASY_CycleTorqueCheck  ) { SEQ.ASY.InitCycleStep(); }
            else if (m_iManNo == mc.ASY_CycleInsp         ) { SEQ.ASY.InitCycleStep(); }
            else if (m_iManNo == mc.ASY_CycleTrayOut      ) { SEQ.ASY.InitCycleStep(); }
            else if (m_iManNo == mc.ASY_CycleTrayIn       ) { SEQ.ASY.InitCycleStep(); }
            else if (m_iManNo == mc.ASY_CycleHoldrCalib   ) { SEQ.ASY.InitCycleStep(); }
            else if (m_iManNo == mc.ASY_CycleToolEccentric) { SEQ.ASY.InitCycleStep(); }
            //else if (m_iManNo == mc.SLD_ZWork      ) { SM.MT.GoAbsMan((int)mi.SLD_ZMotr, PM.GetValue((uint)mi.SLD_ZMotr, (uint)pv.SLD_YWorkStt)); }
            //else if (m_iManNo == mc.SLD_ZWait      ) { SM.MT.GoAbsMan((int)mi.SLD_ZMotr, PM.GetValue((uint)mi.SLD_ZMotr, (uint)pv.SLD_ZWait )); }
            //else if (m_iManNo == mc.SLD_ZWaitMove  ) { SEQ.SLD.InitCycleStep(); }
            //else if (m_iManNo == mc.SLD_Move       ) { SEQ.SLD.InitCycleStep(); }
            
            //else if (m_iManNo == mc.SLD_MotrRepeat ) { SEQ.SLD.InitCycleStep(); }//테스트용
            //else if (m_iManNo == mc.SLD_PbSplyOnOff) { SM.IO.SetY((int)yi.ySLD_Soldering, !SM.IO.GetY((int)yi.ySLD_Soldering));}
               
            //else if (m_iManNo == mc.SLD_SolderTest) {SEQ.SLD.InitCycleStep(); }                                              
            //else if (m_iManNo == EN_MANUAL_CYCLE.mcULD_Home)  { /*ULD.InitHomeStep();*/ }

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

            if (m_iManNo != mc.AllHome && m_iManNo != mc.ASY_Home ) SEQ.InspectHomeDone();
            


            SEQ.InspectLightGrid();

            //bool r1, r2, r3, r4, r5, r6, r7, r8, r9;
            //r1 = r2 = r3 = r4 = r5 = r6 = r7 = r8 = r9 = false;

            //Check Alarm.
            if (SM.ER.IsErr()) { Init(); return; }             //test
            //Cycle Step.
            if (m_iManNo == mc.AllHome)
            {
                if (!SEQ.ASY.CycleHome()) return;

                m_iManNo = (int)mc.NoneCycle;
                
                Log.ShowMessage("Confirm", "All Home Finished!");

                m_iCrntManNo = m_iManNo;
            }

            else if (m_iManNo == mc.ASY_CycleToolCalib     ) { if (SEQ.ASY.CycleToolCalib    (           )) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ASY_CycleTorqueCheck   ) { if (SEQ.ASY.CycleTorqueCheck  (           )) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ASY_CycleInsp          ) { if (SEQ.ASY.CycleManInsp      (m_iInspType)) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ASY_CycleTrayOut       ) { if (SEQ.ASY.CycleOut          (           )) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ASY_CycleTrayIn        ) { if (SEQ.ASY.CycleTrayIn       (           )) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ASY_CycleHoldrCalib    ) { if (SEQ.ASY.CycleHoldrCalib   (           )) m_iManNo = mc.NoneCycle; }
            else if (m_iManNo == mc.ASY_CycleToolEccentric ) { if (SEQ.ASY.CycleToolEccentric(           )) m_iManNo = mc.NoneCycle; }
            //                                      
            //else if (m_iManNo == mc.SLD_ZWaitMove ) { if (SEQ.SLD.CycleZWaitMovePos(m_iNodeNo)) m_iManNo = mc.NoneCycle; }
            //else if (m_iManNo == mc.SLD_Move      ) { if (SEQ.SLD.CycleMovePos     (m_iNodeNo)) m_iManNo = mc.NoneCycle; }
            //
            //else if (m_iManNo == mc.SLD_MotrRepeat) { if (SEQ.SLD.CycleManMotrRepeat()) m_iManNo = mc.NoneCycle; }
            //
            //else if (m_iManNo == mc.SLD_SolderTest) { if (SEQ.SLD.CycleManSolderTest(m_iSolderDelay)) m_iManNo = mc.NoneCycle; }

            else { m_iManNo = mc.NoneCycle; } //여기서 갱신됌.

            //Ok.
            return;
        }
    }
}
