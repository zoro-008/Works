using System;
using System.Threading;
using System.IO;
using COMMON;
using SML;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace Machine
{

    public sealed class SEQ
    {
        static Thread MainThread = new Thread(new ThreadStart(Update));
        static double m_dMainThreadCycleTime ; public static double _dMainThreadCycleTime{get{return m_dMainThreadCycleTime ;}}

        //static CDelayTimer m_tmToStop   ;
        //static CDelayTimer m_tmToStrt   ;
        static CDelayTimer m_tmFlickOn  = new CDelayTimer ();
        static CDelayTimer m_tmFlickOff = new CDelayTimer ();
        static CDelayTimer m_tmCloseDoor= new CDelayTimer ();
        static CDelayTimer m_tmTemp     = new CDelayTimer ();

        static private bool m_bBtnReset ;
        static private bool m_bBtnStart ;
        static private bool m_bBtnStop  ;
        static private bool m_bBtnCool  ;
        static private bool m_bBtnHome  ;

        static private bool m_bRun      ; //Run Flag. (Latched)
        static private bool m_bReqStop  ;
        static private bool m_bFlick    ; //Flicking Flags.


        static private CDelayTimer m_tmCycle  = new CDelayTimer ();
        static private CDelayTimer m_tmToStop = new CDelayTimer ();
        static private CDelayTimer m_tmToStart= new CDelayTimer ();
        static private CDelayTimer m_tmDelay  = new CDelayTimer (); 

        static private EN_SEQ_STEP m_iStep   ; //Sequence Step.
        static private EN_SEQ_STAT m_iSeqStat; //Current Sequence Status.

       

        //FGetCodeCallback GetCode = null ;

        public struct SStep
        {
            public int iHome;
            public int iToStart;
            public int iToStop;
            public void Clear()
            {
                iHome    = 0;
                iToStart = 0;
                iToStop  = 0;
            }
        };
        static private SStep  Step , PreStep   ;

        //Flir 카메라.
        static public CFlir Cam ; //new CFlir("Camera0");

        //Heater controller.
        static public RS485_TK4S Heater ;


        static public bool m_bRunEnd;
    
        //Property.
        static public bool _bBtnReset { set{ m_bBtnReset = value; } }
        static public bool _bBtnStart { set{ m_bBtnStart = value; } }
        static public bool _bBtnStop  { set{ m_bBtnStop  = value; } }
        static public bool _bBtnCool  { set{ m_bBtnCool  = value; } }
        static public bool _bBtnHome  { set{ m_bBtnHome  = value; } }

        static public bool _bRun      { get{ return m_bRun      ; } }
        static public bool _bReqStop  { get{ return m_bReqStop  ; } }
        static public bool _bFlick    { get{ return m_bFlick    ; } }
    
        static public EN_SEQ_STEP  _iStep    { get {return m_iStep   ;}}
        static public EN_SEQ_STAT  _iSeqStat { get {return m_iSeqStat;}}

        //현재 활성화 되어 있는 코드.
        static private sc activeSeqCode = sc.Run ;
        static public  sc ActiveSeqCode {get {return sc.Run ;} set {activeSeqCode = value;}}
        //코드들..
        static public CCode[] Codes = new CCode[(int)sc.MAX_SEQUENCE_CODE];

        static public bool bCamInit = false ;

        public static void Init()
        {
            //Common
            SM.TPara Para;            
            Para.sParaFolderPath = Directory.GetCurrentDirectory() + "\\Util\\";
            Para.iWidth    = 1280;
            Para.iHeight   = 863;
            Para.bTabHides = new bool[6];

            switch(Eqp.sLanguage)
            {
                default       : CLanguage.ChangeLanguage("en"     ); Para.eLanSel = EN_LAN_SEL.English; break;
                case "English": CLanguage.ChangeLanguage("en"     ); Para.eLanSel = EN_LAN_SEL.English; break;
                case "Korean" : CLanguage.ChangeLanguage("ko"     ); Para.eLanSel = EN_LAN_SEL.Korean ; break;
                case "Chinese": CLanguage.ChangeLanguage("zh-Hans"); Para.eLanSel = EN_LAN_SEL.Chinese; break;
            }
            
            //Error         
            Para.Err.bUseErrPic    = true    ;
            Para.Err.eErr          = new ei();
            Para.bTabHides[0]      = false   ;
            //D IO           
            Para.Dio.eDioSel       = EN_DIO_SEL.AXL;
            Para.Dio.eX            = new xi();
            Para.Dio.eY            = new yi();
            Para.bTabHides[1]      = false   ;
            //A IO
            Para.Aio.eAioSel       = EN_AIO_SEL.AXL;
            Para.Aio.eX            = new ax();
            Para.Aio.eY            = new ay();
            Para.Aio.iRangeAMin    = 0       ;
            Para.Aio.iRangeAMax    = 0       ;
            Para.bTabHides[2]      = true    ;

            //TowerLamp
            Para.bTabHides[3]      = true ;

            //Cylinder
            Para.Cyl.eCyl          = new ci();
            Para.bTabHides[4]      = false ;

            //Motor          
            Para.Mtr.eMtrSel = new EN_MTR_SEL[(int)mi.MAX_MOTR];
            for(int i=0; i<(int)mi.MAX_MOTR; i++)
            {
                Para.Mtr.eMtrSel[i] = EN_MTR_SEL.AXL;
            }
            Para.Mtr.eMtr = new mi();
            Para.bTabHides[5]     = false ;
            
            SM .Init(Para);
            OM .Init();
            DM .Init();
    

            for(int i = 0 ; i < Codes.Length ; i++)
            {
                Codes[i] = new CCode();
            }

            Heater = new RS485_TK4S(1,"Heater",1);
            
            MainThread.Priority = ThreadPriority.Highest;
            //MainThread.Priority = ThreadPriority.Lowest;
            MainThread.Start();
            Reset();
            try
            {
                Cam = new CFlir("Camera0");
                bCamInit = Cam.Init() ;
            }
            catch(Exception _e)
            {
                MessageBox.Show(_e.ToString() , "Camera0");
            }
            
            if(!bCamInit)
            {
                MessageBox.Show("Camera Init Failed!", "Error");
            }
            else
            {
                //Cam.SetModeHwTrigger(false);
            }

            

        }

        public static void Close()
        {
            MainThread.Abort();
            MainThread.Join();
            
            SM  .Close();
            OM  .Close();

        }

        //public static bool Compile(sc _eSeq , string[] _sLines)
        //{
        //    Eqp.ClearMsg();
        //    Eqp.AddMsg("컴파일 시작-------------------------------------------");
        //    bool bRet = Codes[(int)_eSeq].Compile(_sLines) ;
        //    if(!bRet)
        //    {
        //        for(int i = 0 ; i < Codes[(int)_eSeq].CompileErrs.Count ; i++)
        //        {
        //            Eqp.AddMsg(Codes[(int)_eSeq].CompileErrs[i]);
        //        }
        //        Eqp.AddMsg("컴파일 실패-------------------------------------------");
        //        return false ;
        //    }
        //
        //    Eqp.AddMsg("컴파일 완료-------------------------------------------");
        //    return true ;
        //}
        
        public static void SetActiveSeq(sc _eSeq)
        {
            activeSeqCode = _eSeq ;
            
        }

        public static void Update()
        {
            double dPreTime  = CTimer.GetTime_us();
            double dCrntTime;
            while (true)
            {
                dCrntTime = CTimer.GetTime_us();
                m_dMainThreadCycleTime = (dCrntTime - dPreTime) / 1000.0;
                dPreTime = dCrntTime;
                Thread.Sleep(0);

                //Motion Dll Update
                SM.Update(m_iSeqStat);

                //Inspection
                InspectMainAir();     
                InspectEmergency();   
                InspectActuator();    
                InspectMotor();       
                InspectCrash();       
                InspectTemp();        

                //Update ErrorProc.
                UpdateButton();       

                //Check Button.
                UpdateSeqState();             

                if(Step.iHome != 0)
                {
                    if(CycleHome())
                    {
                        bool bAllDone = ML.MT_GetHomeDoneAll() ;
                        if(bAllDone)
                        {
                            Eqp.AddMsg("All Homing Finished!");
                            Log.ShowMessage("Confirm", "All Homing Finished!");
                        }
                        else  
                        {
                            Eqp.AddMsg("All Homing Failed!");
                            Log.ShowMessage("Confirm", "All Homing Failed!"  );
                        }
                    }
                }

                Heater.Update();

                //Thread.Sleep(1);
                
            }

        }

        public static void Reset()
        {
            //Check running flag.
            if (m_bRun                               ) return;
            if (m_iSeqStat   == EN_SEQ_STAT.Init     ) return;
            if (m_iStep      == EN_SEQ_STEP.ToStopCon) return;
            if (m_iStep      == EN_SEQ_STEP.ToStop   ) return;
            
            Log.Trace("Seq","Reset");
            
            m_tmFlickOn  .Clear();
            m_tmFlickOff .Clear();
            m_tmTemp     .Clear();
            m_tmCloseDoor.Clear();


            m_tmCycle  .Clear();
            m_tmToStop .Clear();
            m_tmToStart.Clear();
            m_tmDelay  .Clear();

            Step.Clear();
            PreStep.Clear();


            
            //Init. Var.
            m_bBtnReset  = false ;
            m_bBtnStart  = false ;
            m_bBtnStop   = false ;

            m_bRunEnd    = false ;
            m_bRun       = false ;
            m_iStep      = EN_SEQ_STEP.Idle;
            
            //Error.
            ML.ER_Clear();

            //Lot End Flag Reset.
            Log.CloseForm();
            
            m_iSeqStat = EN_SEQ_STAT.Stop;

            for(int i = 0 ; i < (int)sc.MAX_SEQUENCE_CODE ; i++)
            {
                Codes[(int)i].Reset();
            }

            ML.MT_ResetAll();
          
            ML.CL_Reset();

            //2019.10.23
            OM.Info = new OM.CInfo();

        }

        private static void UpdateButton  ()
        {
            //Check Inspect.
            if(m_iStep != EN_SEQ_STEP.Idle) 
            {
                InspectHomeDone () ;
                if(!OM.CmnOptn.bIgnrDoor) InspectDoor();
            }

            if(ML.IO_GetXUp(xi.StartSw)) activeSeqCode = sc.Run ;
            if(ML.IO_GetXUp(xi.CoolSw )) activeSeqCode = sc.Cool;

            //Local Var.
            bool isErr      = ML.ER_IsErr() ;
            bool isHomeEnd  = ML.MT_GetHomeDoneAll();
            bool bStartSw   = ML.IO_GetXUp(xi.StartSw) || m_bBtnStart;
            bool bStopSw    = ML.IO_GetXUp(xi.StopSw ) || m_bBtnStop ;
            bool bHomeSw    = ML.IO_GetXUp(xi.HomeSw ) || m_bBtnHome ;
            bool bCoolSw    = ML.IO_GetXUp(xi.CoolSw ) || m_bBtnCool ;
            bool bResetSw   = m_bBtnReset;
            //bool bResetSw   = ML.IO_GetXUp(xi.ResetSw) || m_bBtnReset;

            

            //Button Flag Clear.
            m_bBtnStart = false ;
            m_bBtnStop  = false ;
            m_bBtnReset = false ;
            m_bBtnHome  = false ;
            m_bBtnCool  = false ;


            //Update Switch's Lamp
            bool bStopBtnFlick  = (m_iStep == EN_SEQ_STEP.ToStopCon || m_iStep == EN_SEQ_STEP.ToStop) && m_bFlick ;            
            bool bResetBtnFlick = (isErr && m_bFlick) ;
            //버튼 클릭시나
            ML.IO_SetY(yi.StartLamp   , ML.IO_GetX(xi.StartSw) ||  m_bRun                             );
            ML.IO_SetY(yi.StopLamp    , ML.IO_GetX(xi.StopSw ) ||  bResetBtnFlick                     );
            ML.IO_SetY(yi.HomeLamp    , ML.IO_GetX(xi.HomeSw )                                        );
            ML.IO_SetY(yi.CoolingLamp , ML.IO_GetX(xi.CoolSw )                                        );
            //ML.IO_SetY(yi.ResetLamp, ML.IO_GetX(xi.ResetSw) || (m_bFlick && isErr));
            //ML.IO_SetY(yi.ResetLamp, ML.IO_GetX(xi.ResetSw) || (m_bFlick && isErr)                 );                        

            //Init. Button Flags.

            if(ML.IO_GetXUp(xi.StartSw))
            {
                SetActiveSeq(sc.Run    ); 
            }
            if (bStartSw)
            {
                Eqp.ClearMsg();
                Log.Trace("bStartSw","버튼 눌림  활성화코드:"+ activeSeqCode.ToString());
                if(!isHomeEnd )
                {
                    Log.ShowMessage("Error" , "장비 홈을 잡아주세요."  );
                    bStartSw = false ;
                }               

                bool bRet = Codes[(int)activeSeqCode].RunInit();
                if(!bRet)
                {
                    Eqp.AddMsg(Codes[(int)activeSeqCode].RunError);
                    Log.ShowMessage("Error" , Codes[(int)activeSeqCode].RunError  );
                    //ML.ER_SetErr(ei.RUN_Error , Codes[(int)activeSeqCode].RunError);
                    bStartSw = false ;
                }
            }

            if(bHomeSw) 
            {
                SEQ.GoHome();
            }
            if(bCoolSw) 
            {
                SetActiveSeq(sc.Cool); 
                Eqp.ClearMsg();
                Log.Trace("bCoolSw","버튼 눌림");
                if(!isHomeEnd )
                {
                    Log.ShowMessage("Error" , "장비 홈을 잡아주세요."  );
                    bCoolSw = false ;
                }                

                bool bRet = Codes[(int)activeSeqCode].RunInit();
                if(!bRet)
                {
                    Eqp.AddMsg(Codes[(int)activeSeqCode].RunError);
                    Log.ShowMessage("Error" , Codes[(int)activeSeqCode].RunError );
                    //ML.ER_SetErr(ei.RUN_Error , Codes[(int)activeSeqCode].RunError);
                    bCoolSw = false ;
                }
            }

           
            
            
            
            
            
            
            
            
            //Buzzer Off.
            if (isErr && bStopSw) ML.TL_SetBuzzOff(true);
            
            bool isStopCon  = bStopSw  || (isErr  && !m_bReqStop &&  m_bRun) ;
            bool isRunCon   = (bStartSw || bCoolSw) && !isErr  /*&& ManualMan.GetManNo() == mcNone*/ ;
            bool isResetCon = (bResetSw && !m_bRun) || (bStopSw && !m_bRun)  ;

            //bool isResetCon = bStopSw && !m_bRun;//Reset버튼 없어서 Stop 버튼에 엮음. 진섭


            //Run.
            if (isRunCon && (m_iStep == EN_SEQ_STEP.Idle)) 
            {
                m_iStep = EN_SEQ_STEP.ToStartCon ;
                ML.TL_SetBuzzOff(false);
                ML.ER_SetDisp(true );                
            }
            if (isStopCon  &&  (m_iStep != EN_SEQ_STEP.Idle)) //스타트버튼 안눌리는것 때문에 테스트.
            { 
                Log.Trace("isStopCon  &&  m_iStep" , string.Format(m_iStep.ToString()));
                Log.Trace("bStopSw"                , bStopSw    ? "True" : "False");
                Log.Trace("isErr"                  , isErr      ? "True" : "False");
                Log.Trace("m_bReqStop"             , m_bReqStop ? "True" : "False");
                Log.Trace("m_bRun"                 , m_bRun     ? "True" : "False");

                m_bReqStop = true;
            }
            if (isResetCon && (m_iStep == EN_SEQ_STEP.Idle)) Reset() ;
            
            //ToStart함수안 에서도 체크함.
            //ToStart TimeOut.
            if (m_tmToStart.OnDelay(m_iStep == EN_SEQ_STEP.ToStartCon || m_iStep == EN_SEQ_STEP.ToStart , 10000))
            {
                //Trace Log.
                string Msg ;
                Msg = string.Format("ToStrtTimeOut : m_iStep=%d" ,m_iStep );
                Log.Trace  ("SEQ",Msg);
                ML.ER_SetErr (ei.ETC_ToStartTO);
                m_iStep = EN_SEQ_STEP.Idle;
                m_bRun  = false;
            }
            
            //ToStop TimeOut.
            if (m_tmToStop.OnDelay(m_iStep == EN_SEQ_STEP.ToStopCon || m_iStep == EN_SEQ_STEP.ToStop , 10000))      //  20000)) {
            {
                //Trace Log.
                string Msg;
                Msg = string.Format("ToStopTimeOut : m_iStep=%d", m_iStep  );
                Log.Trace("SEQ",Msg);
                m_bRun = false ;
                
                ML.ER_SetErr(ei.ETC_ToStopTO);
                m_iStep    = EN_SEQ_STEP.Idle;
                m_bReqStop = false;
            }

            EN_SEQ_STEP iPreStep = m_iStep ;
            if(iPreStep != m_iStep) 
            {
                string sMsg = "" ;
                sMsg = "Step Changed" + string.Format(iPreStep.ToString()) + " -> " + string.Format(m_iStep.ToString()) ;
                Log.Trace("SEQ",sMsg);
            }
            iPreStep = m_iStep ;
            
            
            //Running Step.
            switch (m_iStep) {
                case EN_SEQ_STEP.Idle       : return;            
                case EN_SEQ_STEP.ToStartCon : if(!ToStartCon())//에러나면 그냥 세움.
                                              {
                                                  if(ML.ER_IsErr())
                                                  {
                                                      m_iStep = EN_SEQ_STEP.Idle ;                                                       
                                                  }
                                                  return ;
                                              }

                                              

                                              Step.iToStart = 10 ;
                                              m_iStep = EN_SEQ_STEP.ToStart;
                                              Log.Trace("SEQ","scToStartCon END");
                                              return ;            
                case EN_SEQ_STEP.ToStart    : if(!ToStart())//에러나면 그냥 세움.
                                              {
                                                  if(ML.ER_IsErr())
                                                  {
                                                      m_iStep = EN_SEQ_STEP.Idle ;                                                       
                                                  }
                                                  return ;
                                              }
                                              m_bRun = true ;                                             
                                              m_iStep = EN_SEQ_STEP.Run ;
                                              Log.Trace("SEQ","scToStart END");
                                              return ;            
                case EN_SEQ_STEP.Run        : if(!m_bReqStop) 
                                              {
                                                  if(Autorun()) 
                                                  {
                                                      //Log.ShowMessage("Check", "Work Ended.");
                            
                                                      m_bRunEnd = true;
                                                      m_iStep = EN_SEQ_STEP.ToStopCon ;
                                                  }
                                                  return ;
                                              }
                                              m_bReqStop = false ;
                                              m_iStep = EN_SEQ_STEP.ToStopCon ;
                                              Log.Trace("SEQ","scRun END");
                                              return ;            
                case EN_SEQ_STEP.ToStopCon  : if(!ToStopCon()) 
                                              {
                                                  //if(Autorun())
                                                  //{
                                                  //    Log.Trace("SEQ","scToStopCon LotEnd");
                                                  //}
                                                  return ;
                                              }
                                              Step.iToStop = 10 ;
                                              m_bRun = false ;
                                              m_iStep = EN_SEQ_STEP.ToStop;
                                              Log.Trace("SEQ","scToStopCon END");
                                              return ;            
                case EN_SEQ_STEP.ToStop    :  if (!ToStop()) return ;
                                              m_iStep = EN_SEQ_STEP.Idle ;
                                              m_bReqStop = false ;
                                              
                                              Log.Trace("SEQ","scToStop END");
                                              
                                              return;
            }
        }




        //Functions.
            //Inspection Machine Status.
        public static bool InspectMainAir()
        {
            bool isOk = true ;

            //if(!ML.IO_GetY(yi.ETC_MainAirSol)) isOk = false ;
            //if(!ML.IO_GetX(xi.ETC_MainAir   )) isOk = false ;

            //LOL if (!isOk && m_iSeqStat != EN_SEQ_STAT.ssError) SML.ER.SetErrMsg((int)ei.ETC_MainAir, "Cheked Main Air");
             
            return isOk ;
        }

        public static bool InspectTemp()
        {
            bool isOk = true ;
            //if(ML.IO_GetY(yi.SSTG_HeaterOn)) {
            //    if (Temp.GetCrntTemp(1) > OM.DevOptn.iSStgTemp + 30) { 
            //        ML.IO_SetY(yi.SSTG_HeaterOn, false); 
            //        Log.ShowMessage("Warning", "스테이지 온도가 설정 온도 보다 30이상 높아서 강제 Off합니다."); 
            //    }
            //}
            //else { //켜는것은 껏다키거나 IO로 누르자.
            //    //if(SEQ.Temp.GetCrntTemp(1) < OM.DevOptn.iSStgTemp     ) {
            //    //    ML.IO_SetY(yi.SSTG_HeaterOn,true); 
            //    //    Log.ShowMessage("Warning","온도가 안정권으로 복귀되어 On합니다.",1000);
            //    //}
            //}
            //
            //if(Temp.GetSetTemp(0)!=OM.DevOptn.iSStgTemp) Temp.SetTemp(0,OM.DevOptn.iSStgTemp);

            return isOk ;
        }
        
        public static bool  InspectEmergency()
        {
            bool isOk = true ;
        
            //Check Emergency
            if(ML.IO_GetXUp(xi.EmgSw))
            {
                ML.IO_SetY(yi.LaserOnOff,false);
                ML.MT_EmgStopAll();
                ML.MT_SetServoAll(false);
                ML.ER_SetErr(ei.ETC_Emergency);
                
                isOk = false ;
            }
            
            return isOk ;
        }
                                     
        public static bool  InspectDoor()
        {
            //Local Var.
            bool isOk = true;
            
            if (!ML.IO_GetX(xi.Door1Left )){ML.ER_SetErr(ei.ETC_Door, "Left Door Opened"  );isOk= false;}
            if (!ML.IO_GetX(xi.Door2Front)){ML.ER_SetErr(ei.ETC_Door, "Front Door Opened" );isOk= false;}
            if (!ML.IO_GetX(xi.Door3Right)){ML.ER_SetErr(ei.ETC_Door, "Right Door Opened" );isOk= false;}
            if (!ML.IO_GetX(xi.Door4Rear )){ML.ER_SetErr(ei.ETC_Door, "Rear Door Opened"  );isOk= false;}

            //Ok.
            return isOk;
            
        }
        

        public static bool  InspectActuator()
        {
            //Local Var.
            bool isOk  = true ;
            bool isErr = false;
            
            //Inspect.
            for(int i = 0 ; i < (int)ci.MAX_ACTR; i++) 
            {
                isErr = ML.CL_Err((ci)i) ;
                if (isErr) { ML.ER_SetErr(ei.ATR_TimeOut, ML.CL_GetName((ci)i)); isOk = false; }
            }
            
            //Ok.
            return isOk;
        }
                                                
        public static bool  InspectMotor()
        {
            //Local Var.
            bool isOk  = true;
            
            for(int i = 0 ; i < (int)mi.MAX_MOTR; i++) 
            {
                if (ML.MT_GetAlarmSgnl(i) ){ML.ER_SetErr(ei.MTR_Alarm  , ML.MT_GetName(i)); isOk = false; }
                if (ML.MT_GetHomeDone(i)){
                    if (ML.MT_GetNLimSnsr (i)) {ML.ER_SetErr(ei.MTR_NegLim , ML.MT_GetName(i));isOk = false;}
                    if (ML.MT_GetPLimSnsr (i)) {ML.ER_SetErr(ei.MTR_PosLim , ML.MT_GetName(i));isOk = false;}
                }
            }    
            
            //Ok.
            return isOk;
        }
                                                
        public static bool  InspectHomeDone()   
        {
            //Local Var.
            bool isOk = true;
            
            //Inspect.
            for(int i = 0 ; i < (int)mi.MAX_MOTR; i++) {
                if (!ML.MT_GetHomeDone(i)){ML.ER_SetErr(ei.MTR_HomeEnd , ML.MT_GetName(i)); isOk = false; }
            }
            
            //Ok.
            return isOk;
        }

        public static bool InspectCrash()
        {
            bool bCrashed = false;
            return !bCrashed;
        }

            //Running Functions.
        public static bool ToStartCon()   //스타트를 하기 위한 조건을 보는 함수.
        {
            
            
            
            
            

            

            return true ;
        } 

        public static bool CheckStop()     //스탑을 하기 위한 조건을 보는 함수.
        {
            for(int i = 0 ; i < (int)ci.MAX_ACTR ; i++)
            {
                if(!ML.CL_Complete((ci)i)) return false ;
            }

            for(int i = 0 ; i < (int)mi.MAX_MOTR ; i++)
            {
                if(!ML.MT_GetStop((mi)i)) return false ;
            }
            return true ;
        } 

        public static bool ToStopCon()     //스탑을 하기 위한 조건을 보는 함수.
        {
            //TODO :: 일단 없애고 투스탑에서 멈추는걸로 하려함
            //return CheckStop() ;
            ThreadRecorder.StopRec ();

            return true;
        } 
        
        
        public static bool ToStart()      //스타트를 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 5000)) ML.ER_SetErr(ei.ETC_ToStartTO); //EM_SetErr(eiLDR_ToStartTO);

            String sTemp;
            sTemp = String.Format("Step.iToStart={0:00}", Step.iToStart);
            if (Step.iToStart != PreStep.iToStart)
            {
                Log.Trace("", sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: Step.iToStart = 0;
                    return true;

                case 10:
                    //2019.10.23
                    OM.Info = new OM.CInfo();

                    ML.CL_StopRpt();
                    FormVision.bLive = false;
                    //if (EmbededExe.GetHPSHandle() < 0) return false;

                    Step.iToStart++;
                    return false;

                case 11:

                    ML.CL_Move(ci.CoverUD , fb.Fwd);
                    ML.CL_Move(ci.HeatrUD , fb.Bwd);
                    Step.iToStart++;
                    return false;

                case 12:
                    if(!ML.CL_Complete(ci.CoverUD , fb.Fwd)) return false ;
                    if(!ML.CL_Complete(ci.HeatrUD , fb.Bwd)) return false ;
                    for(int i = 0 ; i < (int)mi.MAX_MOTR ; i++)
                    {
                        ML.MT_Stop(i);
                    }
                    
                    ML.CL_Move(ci.CoverFB , fb.Bwd);
                    ML.MT_GoAbsMan(mi.LaserX  , 0.0);
                    ML.MT_GoAbsMan(mi.MagnetX , 0.0);
                    //ML.MT_GoAbsMan(mi.RotorT  , 0.0);
                    Step.iToStart++;
                    return false;

                case 13: 
                    if(!ML.CL_Complete(ci.CoverFB , fb.Bwd)) return false ;
                    if(!ML.MT_GetStopInpos(mi.LaserX )) return false ;
                    if(!ML.MT_GetStopInpos(mi.MagnetX)) return false ;
                    //if(!ML.MT_GetStop(mi.RotorT )) return false ;
                    Step.iToStart = 0;
                    return true;
            }
        }

        public static bool ToStop()       //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 5000)) ML.ER_SetErr(ei.ETC_ToStopTO); //EM_SetErr(eiLDR_ToStopTO);

            String sTemp;
            sTemp = String.Format("Step.iToStop={0:00}", Step.iToStop);
            if (Step.iToStop != PreStep.iToStop)
            {
                Log.Trace("", sTemp);
            }

            PreStep.iToStop = Step.iToStop;

            //Move Home.
            switch (Step.iToStop)
            {
                default:
                    Step.iToStop = 0;
                    return true;

                case 10:
                    //2019.10.23
                    OM.Info = new OM.CInfo();

                    FormVision.bLive = false;
                    for(int i = 0 ; i < (int)mi.MAX_MOTR ; i++)
                    {
                        ML.MT_Stop(i);
                    }
                    ML.IO_SetY(yi.LaserOnOff  , false);
                    ML.IO_SetY(yi.SepctroLight, false);
                    Step.iToStop++;
                    return false;

                case 11:
                    //정지 확인
                    for(int i = 0 ; i < (int)mi.MAX_MOTR ; i++)
                    {
                        if(!ML.MT_GetStop((mi)i)) return false ;
                    }
                    for(int i = 0 ; i < (int)ci.MAX_ACTR ; i++)
                    {
                        if(!ML.CL_Complete((ci)i)) return false ;
                    }

                    ML.CL_Move(ci.CoverUD, fb.Fwd);
                    ML.CL_Move(ci.HeatrUD, fb.Bwd);
                    Step.iToStop++;
                    return false;

                case 12:
                    if (!ML.CL_Complete(ci.CoverUD, fb.Fwd)) return false;
                    if (!ML.CL_Complete(ci.HeatrUD, fb.Bwd)) return false;
                    ML.CL_Move(ci.CoverFB, fb.Bwd);

                    ML.MT_GoAbsMan(mi.LaserX , 0.0);
                    ML.MT_GoAbsMan(mi.MagnetX, 0.0);
                    //ML.MT_GoAbsMan(mi.RotorT, 0.0);
                    Step.iToStop++;
                    return false;

                case 13:
                    if (!ML.CL_Complete(ci.CoverFB, fb.Bwd)) return false;
                    if (!ML.MT_GetStopInpos(mi.LaserX)) return false;
                    if (!ML.MT_GetStopInpos(mi.MagnetX)) return false;
                    if (!ML.MT_GetStop(mi.RotorT)) return false;
                    Step.iToStop = 0;
                    return true;
            }
        }

        

        public static void GoHome()
        {
            if(m_bRun || Step.iHome != 0) return;

            Step.iHome = 10 ;
        }
        public static bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                ML.ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(sTemp);
                Step.iHome=0;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Log.Trace(sTemp);
            }
            
            PreStep.iHome = Step.iHome ;

            if (ML.ER_IsErr()) {
                Step.iHome = 0 ;
                return false;
            }

            switch (Step.iHome)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    Step.iHome = 0 ;
                    return true ;
            
                case 10:
                    ML.IO_SetY(yi.LaserOnOff  , false);
                    ML.IO_SetY(yi.LightOnOff  , false);
                    ML.IO_SetY(yi.SepctroLight, false);
                    ML.CL_Move(ci.CoverUD , fb.Fwd);
                    ML.CL_Move(ci.HeatrUD , fb.Bwd);
                    Step.iHome++;
                    return false;

                case 11:
                    if(!ML.CL_Complete(ci.CoverUD , fb.Fwd)) return false ;
                    if(!ML.CL_Complete(ci.HeatrUD , fb.Bwd)) return false ;
                    ML.CL_Move(ci.CoverFB , fb.Bwd);
                    ML.MT_GoHome(mi.LaserX );
                    ML.MT_GoHome(mi.MagnetX);
                    ML.MT_GoHome(mi.RotorT );
                    Step.iHome++;
                    return false;

                case 12: 
                    if(!ML.CL_Complete(ci.CoverFB , fb.Bwd)) return false ;
                    if(!ML.MT_GetHomeDone(mi.LaserX )) return false ;
                    if(!ML.MT_GetHomeDone(mi.MagnetX)) return false ;
                    if(!ML.MT_GetHomeDone(mi.RotorT )) return false ;

                    Step.iHome=0;
                    return true ;
            }
        }
        
        public static bool Autorun()      //오토런닝시에 계속 타는 함수. 
        {
            bool bRet = Codes[(int)activeSeqCode].Run();
            string sErrMsg = Codes[(int)activeSeqCode].RunError ;
            if(sErrMsg != "") ML.ER_SetErr(ei.RUN_Error , sErrMsg);
            return bRet ;            
        }
          
        public static string CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            String sMsg = "";
            if (ML.CL_Complete(_eActr, _eFwd)) return sMsg;

            if(_eActr == ci.CoverUD){
            }
            else if(_eActr == ci.CoverFB){
                if(!ML.CL_Complete(ci.CoverUD,fb.Fwd)) sMsg = "충돌 감지 커버 실린더 다운상태";
            }
            else if(_eActr == ci.HeatrUD){
                //if(!ML.CL_Complete(ci.CoverUD,fb.Fwd)) sMsg = "충돌 감지 커버 실린더 다운상태";
                if(_eFwd == fb.Fwd)
                {
                    if(ML.MT_GetCmdPos(mi.MagnetX) > OM.CmnOptn.dMagnetCheck)
                    {
                        sMsg = "충돌 감지 Magnet X axis 전진 상태";
                    }
                }
            }

            if(sMsg != "")
            {
                if(_bRun) Log.Trace      (ML.CL_GetName(_eActr).ToString() + sMsg);
                else      Log.ShowMessage(ML.CL_GetName(_eActr).ToString() , sMsg);
            }

            return sMsg;
        }

        public static string CheckSafe(mi _eMotr)
        {
            string sMsg = "";

            if (_eMotr == mi.RotorT) { }
            else if (_eMotr == mi.LaserX) {
            }
            else if (_eMotr == mi.MagnetX) { 
                if (!ML.CL_Complete(ci.HeatrUD, fb.Bwd)) sMsg = "충돌 감지 히터 실린더 업상태";
            }

            if(sMsg != "")
            {
                if(_bRun) Log.Trace      (ML.MT_GetName(_eMotr).ToString() + sMsg);
                else      Log.ShowMessage(ML.MT_GetName(_eMotr).ToString() , sMsg);
            }

            return sMsg;
        }
        
        static private EN_SEQ_STAT iPreStat = EN_SEQ_STAT.MAX_SEQ_STAT;
        static private EN_SEQ_STAT iStat;
        public static void UpdateSeqState() 
        {            
            bool isInit    =  Step.iHome != 0; 
            bool isError   =  ML.ER_IsErr() ;                 
            bool isRunning =  m_bRun     ;
            bool isRunEnd  =  m_bRunEnd  ;
            bool isStop    = !m_bRun && !m_bRunEnd;
            bool isToStart =  m_iStep == EN_SEQ_STEP.ToStartCon || m_iStep == EN_SEQ_STEP.ToStart ;
            bool isToStop  =  m_iStep == EN_SEQ_STEP.ToStopCon  || m_iStep == EN_SEQ_STEP.ToStop  ;
            
            //Flicking Timer.
            if (m_bFlick) { m_tmFlickOn .Clear(); if (m_tmFlickOff.OnDelay( m_bFlick , 500)) m_bFlick = false; }
            else          { m_tmFlickOff.Clear(); if (m_tmFlickOn .OnDelay(!m_bFlick , 500)) m_bFlick = true ; }
            
            //Set Sequence State. Tower Lamp
                 if (isInit   ) { m_iSeqStat = EN_SEQ_STAT.Init       ;}
            else if (isError  ) { m_iSeqStat = EN_SEQ_STAT.Error      ;}
            else if (isToStart) { m_iSeqStat = EN_SEQ_STAT.ToStart    ;}
            else if (isToStop ) { m_iSeqStat = EN_SEQ_STAT.ToStop     ;}
            else if (isRunning) { m_iSeqStat = EN_SEQ_STAT.Running    ;}
            else if (isStop   ) { m_iSeqStat = EN_SEQ_STAT.Stop       ;}
            else if (isRunEnd ) { m_iSeqStat = EN_SEQ_STAT.RunWarn    ;}
                                  //Log.ShowMessage("Warning", "Work End");}
            else                { }

            //Save Status Log.
            string sStatText,sSub;
            if (m_iSeqStat != iPreStat)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0) { sSub = " STATUS END"  ; iStat = iPreStat  ; }
                    else        { sSub = " STATUS START"; iStat = m_iSeqStat; }

                    switch (iStat)
                    {
                        default: sStatText = "Program Start"; break;
                        case EN_SEQ_STAT.Init   : sStatText = "INIT"    + sSub; break;
                        case EN_SEQ_STAT.Warning: sStatText = "WARNING" + sSub; break;
                        case EN_SEQ_STAT.Error  : sStatText = "ERROR"   + sSub; break;
                        case EN_SEQ_STAT.Running: sStatText = "RUNNING" + sSub; break;
                        case EN_SEQ_STAT.Stop   : sStatText = "STOP"    + sSub; break;
                        case EN_SEQ_STAT.RunWarn: sStatText = "RUNWARN" + sSub; break;
                        case EN_SEQ_STAT.WorkEnd: sStatText = "LOTEND"  + sSub; break;
                        case EN_SEQ_STAT.Manual : sStatText = "MANUAL"  + sSub; break;
                        case EN_SEQ_STAT.ToStart: sStatText = "TO START"+ sSub; break;
                        case EN_SEQ_STAT.ToStop : sStatText = "TO STOP" + sSub; break;
                    }
                    Log.Trace(sStatText, ti.Sts);
                }

                iPreStat = m_iSeqStat;
            }

        }        





    }
}
