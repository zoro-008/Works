using System;
using System.Threading;
using System.IO;
using COMMON;
using SML;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Machine
{

    public sealed class SEQ
    {
        
        [DllImport("user32", EntryPoint = "FindWindow")]
        static private extern IntPtr FindWindow(string IPClassName, String IpWindowName);
        [DllImport("User32", EntryPoint = "SetForegroundWindow")]
        static private extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static public  extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_SHOWMAXVIEW = 3;


        static Thread MainThread = new Thread(new ThreadStart(Update));
        static double m_dMainThreadCycleTime ; public static double _dMainThreadCycleTime{get{return m_dMainThreadCycleTime ;}}

        static public Macro         Mcr = new Macro          (7);
        static public Index         INDX = new Index         ((int)pi.INDX + ti.Max);
        static public XRayDressy    XRYD = new XRayDressy    ((int)pi.XRYD + ti.Max);
        static public XRayEz        XRYE = new XRayEz        ((int)pi.XRYE + ti.Max);


        static public Part[] m_Part = new Part[(int)pi.MAX_PART];

        
        static public USBSerialNumber USB = new USBSerialNumber();
        //static public UsbNotification USBdetect = new UsbNotification();

        //static public VisnCom   Visn = new VisnCom() ;

        static CDelayTimer m_tmToStop   ;
        static CDelayTimer m_tmToStrt   ;
        static CDelayTimer m_tmFlickOn  ;
        static CDelayTimer m_tmFlickOff ;
        static CDelayTimer m_tmCloseDoor;
        static CDelayTimer m_tmTemp     ;
        static CDelayTimer m_tmBuzzer   ;       
        
        static public CCycleTimer m_cyTrayWorktime ;
        

        static private bool m_bBtnReset ;
        static private bool m_bBtnStart ;
        static private bool m_bBtnStop  ;
        static private bool m_bBtnAir   ;

        static bool m_bInspDispr;
        static bool m_bInspUnkwn;

        static private bool m_bRun      ; //Run Flag. (Latched)
        static private bool m_bRunEdge  ;
        static private bool m_bReqStop  ;
        static private bool m_bFlick    ; //Flicking Flags.

        static private EN_SEQ_STEP m_iStep   ; //Sequence Step.
        static private EN_SEQ_STAT m_iSeqStat; //Current Sequence Status.

        static public bool m_bTempTrigger;
        static public bool m_bTriggerOne;
        static public bool bTrayWorkTimer;

        static public bool m_bRunEnd;
    
        //Property.
        static public bool _bBtnReset { set{ m_bBtnReset = value; } }
        static public bool _bBtnStart { set{ m_bBtnStart = value; } }
        static public bool _bBtnStop  { set{ m_bBtnStop  = value; } }
        static public bool _bBtnAir   { set{ m_bBtnAir   = value; } }

        static public bool _bRun      { get{ return m_bRun      ; } }
        static public bool _bRunEdge  { get{ return m_bRunEdge  ; } }
        static public bool _bReqStop  { get{ return m_bReqStop  ; } }
        static public bool _bFlick    { get{ return m_bFlick    ; } }
                      
        static public bool _bInspDispr { get {return m_bInspDispr;} set{m_bInspDispr = value;} }
        static public bool _bInspUnkwn { get {return m_bInspUnkwn;} set{m_bInspUnkwn = value;} }
    
        static public EN_SEQ_STEP  _iStep    { get {return m_iStep   ;}}
        static public EN_SEQ_STAT  _iSeqStat { get {return m_iSeqStat;}}

        //Serial통신
        static public RS232_3310g         Barcord = new RS232_3310g        (4, "Barcord"   );  
        static public RS232_X_Ray         XrayCom = new RS232_X_Ray        (3, "XrayCom"   );

        //Ver 1.0.3.0 화면 녹화 기능 랏엔드할때 녹화 종료하려고 여기에서 생성
        //static public Recorder ScreenRecord = new Recorder();

        public static void Init()
        {
            SM.TPara Para;            
            Para.sParaFolderPath = Directory.GetCurrentDirectory() + "\\Util\\";
            Para.iWidth = 1280;
            Para.iHeight = 863;
            Para.bTabHides = new bool[6];
            //Para.bTabHides[2] = true;
            //Para.bTabHides[3] = true;
            //Para.bTabHides[5] = true;
            Para.bUseErrPic = true;
            Para.iCntErr = 60;
            Para.iCntDIn = 96;
            Para.iCntDOut = 64;
            Para.iCntCylinder = (int)ci.MAX_ACTR;
            Para.iCntMotr = (int)mi.MAX_MOTR;
            Para.eLanSel = EN_LAN_SEL.English;
            Para.eDio = EN_DIO_SEL.AXL;
            Para.eMotors = new EN_MOTR_SEL[Para.iCntMotr];

            Para.eMotors[0] = EN_MOTR_SEL.AXL;
            Para.eMotors[1] = EN_MOTR_SEL.AXL;
            Para.eMotors[2] = EN_MOTR_SEL.AXL;
            Para.eMotors[3] = EN_MOTR_SEL.AXL;
            Para.eMotors[4] = EN_MOTR_SEL.AXL;

            //AI TAB 2
            Para.eAio = EN_AIO_SEL.AXL;
            Para.iCntAIn    = 0;
            Para.iCntAOut   = 0;
            Para.iRangeAMin = 0;
            Para.iRangeAMax = 0;
            Para.bTabHides[2] = true;

            SM.Init(Para);
            OM .Init();
            DM .Init();
            LOT.Init();
            SPC.Init();
            PM .Init(PM.PstnCnt);
       

            //VisnPara.xVisn_Ready     = xi.VISN_Ready   ; 
            //VisnPara.xVisn_Busy      = xi.VISN_Busy    ;

            //VisnPara.yVisn_Command   = yi.VISN_Command ;
            //VisnPara.yVisn_JobChange = yi.VISN_Change  ;
            //VisnPara.yVisn_Reset     = yi.VISN_Reset   ;
            //VisnPara.yVisn_ManMode   = yi.VISN_ManMode ;
            //VisnPara.yVisn_ManInsp   = yi.VISN_ManInsp ;
        
            
            MainThread.Priority = ThreadPriority.Highest;
            //MainThread.Priority = ThreadPriority.Normal;
            MainThread.Start();

            m_tmToStop      = new CDelayTimer ();
            m_tmToStrt      = new CDelayTimer ();
            m_tmFlickOn     = new CDelayTimer ();
            m_tmFlickOff    = new CDelayTimer ();
            m_tmCloseDoor   = new CDelayTimer ();
            m_tmTemp        = new CDelayTimer ();
            m_tmBuzzer      = new CDelayTimer ();

            m_cyTrayWorktime = new CCycleTimer();

            m_bBtnReset     = false;
            m_bBtnStart     = false;
            m_bBtnStop      = false;
            m_bBtnAir       = false;

            m_bRun          = false;
            m_bRunEdge      = false;
            m_bFlick        = false;
            m_iStep         = EN_SEQ_STEP.Idle;
            m_iSeqStat      = EN_SEQ_STAT.Stop;

            //Run End Buzzer.
            m_bRunEnd       = false;

            m_Part[(int)pi.INDX] = INDX; //m_Part[(int)pi.INDX].SetPartId((int)pi.INDX + ti.Max);
            m_Part[(int)pi.XRYD] = XRYD; //m_Part[(int)pi.RGPY].SetPartId((int)pi.RGPY + ti.Max);
            m_Part[(int)pi.XRYE] = XRYE; //m_Part[(int)pi.RGPY].SetPartId((int)pi.RGPY + ti.Max);

            //Mcr.CycleInit();
            //LoadCell   .PortOpen();
            //Dispr      .PortOpen();
            //HeightSnsr .PortOpen();
            Barcord.PortOpen();
            //XrayCom.PortOpen();
            //Temp       .PortOpen();

            //ML.IO_SetY(yi.SSTG_HeaterOn,true); 


            //DateTime Time =  DateTime.Now ;
            //bool bConnect = SEQ.Oracle.OpenDB(OM.CmnOptn.sOracleIP , OM.CmnOptn.sOraclePort , OM.CmnOptn.sOracleID , OM.CmnOptn.sOraclePassword);//"192.168.1.77" , "1521" , "hr","hr"
            //TimeSpan Span =  DateTime.Now - Time ;
            //double dVal = Span.TotalMilliseconds ;


            //if(!OM.CmnOptn.bOracleNotUse && !bConnect) Log.ShowMessage("Oracle","DB Connection Error");
            XRYD.iWorkStep = OM.EqpStat.iLastWorkStep;

            
            //Add Event
            //XRYD.MacReset += new Machine.XRayDressy.CMacReset(Mcr.Reset);
            //XRYE.MacReset += new Machine.XRayEz    .CMacReset(Mcr.Reset);


        }

        public static void Close()
        {
            ML.MT_SetServoAll(false);

            MainThread.Abort();
            MainThread.Join();
            
            SM  .Close();
            OM  .Close();
            LOT .Close();
            SPC .Close();

            //ScreenRecord.Close(); //녹화기능 Dispose

            //for (int i = 1; i < (int)si.MAX_RS232; i++)
            //{
            //    Com[i].PortClose();
            //}

            //ML.IO_SetY(yi.SSTG_HeaterOn,false); 
            //SEQ.Oracle.CloseDB();
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
                SM.Update(m_iSeqStat);

                //Part
                for (int i = 0; i < (int)pi.MAX_PART; i++)
                {
                    m_Part[i].Update();
                }

                //Inspection
                InspectMainAir();
                InspectEmergency();
                InspectActuator();
                InspectMotor();
                InspectCrash();
                InspectTemp();
                InspectFlowMeter();
                InspectMainServo();
                InspectDoor();
                InspectBuzzer();


                //Update ErrorProc.
                UpdateButton();

                //Check Button.
                UpdateSeqState();

                //Update Motor State (Input)
                MM.Update();

                //SPC
                SPC.Update(LOT.CrntLotData.sEmployeeID , LOT.CrntLotData.sLotNo , OM.GetCrntDev() , m_iSeqStat , OM.EqpStat.bMaint);

                //모니터 화면 녹화
                
                //RecUpdate(OM.CmnOptn.bUseRecord);

            }

        }
        
        public static void RecUpdate(bool _bRecord = false)
        {
            //if (!_bRecord) {
            //    if(ScreenRecord.state.bRecording) ScreenRecord.StopRecording();
            //    return;
            //}
            //switch (SEQ._iSeqStat)
            //{
            //    case EN_SEQ_STAT.Running: ScreenRecord.StartRecording(1); break;
            //    case EN_SEQ_STAT.Error  : ScreenRecord.PauseRecording(); break;
            //    case EN_SEQ_STAT.Stop   : ScreenRecord.PauseRecording(); break;
            //    case EN_SEQ_STAT.WorkEnd: ScreenRecord.StopRecording (); break;
            //
            //}
            //public static int iTemp = 0;
            //switch (iTemp)
            //{
            //    case 0: ScreenRecord.StartRecording(1); break;
            //    case 1: ScreenRecord.PauseRecording( ); break;
            //    case 2: ScreenRecord.PauseRecording( ); break;
            //    case 3: ScreenRecord.StopRecording ( ); break;
            //
            //}
        }


        //    CPartInterface * m_pPart[MAX_PART] ;

        //--------------------------------------------------------------------------------------------------------

        public static void Reset()
        {
            //Check running flag.
            if (m_bRun                             ) return;
            if (m_iSeqStat == EN_SEQ_STAT.Init     ) return;
            if (m_iStep    == EN_SEQ_STEP.ToStopCon) return;
            if (m_iStep    == EN_SEQ_STEP.ToStop   ) return;
            
            LOT.Reset();
            
            Log.Trace("Seq","Reset");
            
            m_tmToStop   .Clear();
            m_tmToStrt   .Clear();
            m_tmFlickOn  .Clear();
            m_tmFlickOff .Clear();
            m_tmTemp     .Clear();
            m_tmBuzzer   .Clear();
            m_tmCloseDoor.Clear();
            
            //Init. Var.
            m_bBtnReset  = false ;
            m_bBtnStart  = false ;
            m_bBtnStop   = false ;

            m_bRunEnd    = false ;
            m_bRun       = false ;
            m_iStep      = EN_SEQ_STEP.Idle;
            
            //Error.
            ML.ER_Clear();
            
            //Manual.
            MM.Reset();
            
            //ML.
            for(int i=0; i < (int)pi.MAX_PART; i++){
                m_Part[i].Reset();
            }
            
            //Lot End Flag Reset.
            LOT.Reset();
            Mcr.Reset();
            Log.CloseForm();
            //FM_CloseMsgOk();

            m_iSeqStat = EN_SEQ_STAT.Stop;

            //System.
            //if(!EM_IsErr()) return; //20150801 선계원 홈잡을때 리셑 누르면 홈스텝이 날라가서 처박았음.
            //ML.MT_ResetAll();
            ML.MT_ResetAll();
            ML.MT_SetServoAll(true);
     
        }

        

         private static void UpdateButton  ()
         {
             //Check Inspect.
             //if (!OM.CmnOptn.bIgnrDoor) 
             InspectDoor();

             if(m_iStep != EN_SEQ_STEP.Idle) 
             {
                 InspectHomeDone () ;
                 
             }

             if (m_iStep == EN_SEQ_STEP.Idle) InspectLightGrid();
             //Local Var.
             bool isErr     = ML.ER_IsErr() ;
             bool isHomeEnd = ML.MT_GetHomeDoneAll();
         
             //vision manual button.
             //CDelayTimer tmVisnCycle ;
             //if(IO_GetX(xETC_LStopSw)&& m_iSeqStat == ssStop ){
                 //if(tmVisnCycle.OnDelay(true , 1000)) {
                 //    tmVisnCycle.Clear();
                 //    if(MM.GetManNo() == mcNoneCycle) {
                 //        MM.SetManCycle(mcVSN_CycleWork);
                 //    }
                 //}
             //}
             //else {
                 //tmVisnCycle.Clear();
             //}

             if (ML.IO_GetXUp(xi.ETC_TrayLiftSw) && SEQ._iSeqStat == EN_SEQ_STAT.Stop) 
             {
                 INDX.MoveMotr(mi.LODR_ZElev, pv.LODR_ZElevRefill);
             }
         
             bool bStartSw   = ML.IO_GetXUp(xi.ETC_StartSw) || m_bBtnStart;// || ML.IO_GetXUp(xi.ETC_StartSwR) || m_bBtnStart ; 
             bool bStopSw    = ML.IO_GetXUp(xi.ETC_StopSw ) || m_bBtnStop ;// || ML.IO_GetXUp(xi.ETC_StopSwR ) || m_bBtnStop  ; 
             bool bResetSw   = ML.IO_GetXUp(xi.ETC_ResetSw) || m_bBtnReset;// || ML.IO_GetXUp(xi.ETC_ResetSwR) || m_bBtnReset ; 
             bool bAirSw     = ML.IO_GetXUp(xi.ETC_AirSw  ) || m_bBtnAir ;
             bool bInitSw    = ML.IO_GetXUp(xi.ETC_InitSw ) ;
         
             if(m_bBtnStart)
             {
                 Log.Trace("m_bBtnStart","true");
             }         
             if(bStartSw)
             {
                 if (!LOT.LotOpened)
                 {
                     ML.ER_SetErr(ei.ETC_LotOpen, "Lot Open이 되지 않았습니다.");
                     return;
                 }
                 Log.Trace("bStartSw","Started");
         
                 if(m_iSeqStat == EN_SEQ_STAT.Init   ) {Log.Trace("SeqStat","ssInit   ");}
                 if(m_iSeqStat == EN_SEQ_STAT.WorkEnd) {Log.Trace("SeqStat","ssWorkEnd");}
                 if(m_iSeqStat == EN_SEQ_STAT.Error  ) {Log.Trace("SeqStat","ssError  ");}
                 if(m_iSeqStat == EN_SEQ_STAT.Running) {Log.Trace("SeqStat","ssRunning");}
                 if(m_iSeqStat == EN_SEQ_STAT.Stop   ) {Log.Trace("SeqStat","ssStop   ");}
                 if(m_iSeqStat == EN_SEQ_STAT.RunWarn) {Log.Trace("SeqStat","ssRunWarn");}

                //if (OM.CmnOptn.bUseRecord)
                //{
                //    ScreenRecord.StartRecording();
                //}
         
                 if(MM.GetManNo() != (int)mc.NoneCycle) {
                     Log.Trace("ManCycle",string.Format(MM.GetManNo().ToString()));
                     bStartSw = false;
                 }
             }
         
         
             //Update Switch's Lamp
             bool bStopBtnFlick = (m_iStep == EN_SEQ_STEP.ToStopCon || m_iStep == EN_SEQ_STEP.ToStop) && m_bFlick ;
             
             
             //버튼 클릭시나
             ML.IO_SetY(yi.ETC_StartLp  , ML.IO_GetX(xi.ETC_StartSw ) ||  m_bRun                             );
             ML.IO_SetY(yi.ETC_StopLp   , ML.IO_GetX(xi.ETC_StopSw  ) || !m_bRun || bStopBtnFlick            );
             ML.IO_SetY(yi.ETC_ResetLp  , ML.IO_GetX(xi.ETC_ResetSw ) || (m_bFlick && isErr)                 );
             ML.IO_SetY(yi.ETC_AirLp    , ML.IO_GetX(xi.ETC_AirSw   ) || ML.IO_GetY(yi.ETC_MainAirSol       ));
             ML.IO_SetY(yi.ETC_InitLp   , ML.IO_GetX(xi.ETC_InitSw  )                                        );
             
           //  ML.IO_SetY(yi.ETC_StartLpR  , ML.IO_GetX(xi.ETC_StartSwR ) ||  m_bRun                             );
           //  ML.IO_SetY(yi.ETC_StopLpR   , ML.IO_GetX(xi.ETC_StopSwR  ) || !m_bRun || bStopBtnFlick            );
           //  ML.IO_SetY(yi.ETC_ResetLpR  , ML.IO_GetX(xi.ETC_ResetSwR ) || (m_bFlick && isErr)                 );
           ////ML.IO_SetY(yi.ETC_AirLp     , ML.IO_GetX(xi.ETC_AirSw    ) || ML.IO_GetY(yi.ETC_MainAirSol     ));
           //  ML.IO_SetY(yi.ETC_InitLpR   , ML.IO_GetX(xi.ETC_InitSwR  )                                        );
             
             //Center Man Button
             m_bBtnStart = false ;
             m_bBtnStop  = false ;
             m_bBtnReset = false ;
             m_bBtnAir   = false ;
             

             //Init. Button Flags.
             if (bStartSw)
             {
                 //bool bAllArayNone = DM.ARAY[riLSP].CheckAllStat(csNone) && DM.ARAY[riLDR].CheckAllStat(csNone) && DM.ARAY[riLST].CheckAllStat(csNone) &&
                 //                    DM.ARAY[riPSB].CheckAllStat(csNone) && DM.ARAY[riULD].CheckAllStat(csNone) && DM.ARAY[riVSN].CheckAllStat(csNone)  ;
                 if(!isHomeEnd            ) { Log.ShowMessage("Error" , "장비 홈을 잡아주세요."  ); bStartSw = false ; }
                 //if(!LOT.GetLotOpen     ()) { Log.ShowMessage("Error" , "장비 랏오픈을 해주세요."); bStartSw = false ; }
                 if(!InspectStripDispr  ()) { m_bInspDispr = true ; bStartSw = false ; }
                 if(!InspectStripUnknown()) { m_bInspUnkwn = true ; bStartSw = false ; }

                 if (m_iSeqStat == EN_SEQ_STAT.WorkEnd || m_iSeqStat == EN_SEQ_STAT.RunWarn) Reset();
             }

             if (bInitSw)
             {
                 MM.SetManCycle(mc.AllHome);
             }
             
             //Air Switch.
             if(bAirSw && !m_bRun && m_iSeqStat != EN_SEQ_STAT.Init)
             {
                 ML.IO_SetY(yi.ETC_MainAirSol , !ML.IO_GetY(yi.ETC_MainAirSol )) ;
             }
             
             //Buzzer Off.
             if (isErr && bStopSw) ML.TL_SetBuzzOff(true);
             
             //Set Condition Flags.
             if( bStartSw)   //스타트버튼 안눌리는것 때문에 테스트.
             { 
                 Log.Trace("isErr" , isErr ? "true":"false");
                 Log.Trace("ManualMan.GetManNo()", string.Format(MM.GetManNo().ToString()));
             }
             if (bStopSw)
             {
                 Log.Trace("Stop", "Stop");
             }
             
             bool isStopCon  = bStopSw  || (isErr  && !m_bReqStop &&  m_bRun) ;
             bool isRunCon   = bStartSw && !isErr  /*&& ManualMan.GetManNo() == mcNone*/ ;
             bool isResetCon = bResetSw && !m_bRun ;
             
             //Run.
             if (isRunCon && (m_iStep == EN_SEQ_STEP.Idle)) 
             {
                 m_iStep = EN_SEQ_STEP.ToStartCon ;
                 ML.TL_SetBuzzOff(false);
                 ML.ER_SetDisp(true );
                 
             }
             if( isRunCon && (m_iStep == EN_SEQ_STEP.Idle)) //스타트버튼 안눌리는것 때문에 테스트.
             { 
                 Log.Trace("isRunCon && m_iStep" , string.Format(m_iStep.ToString()));
             }
             if( isStopCon  &&  (m_iStep != EN_SEQ_STEP.Idle)) //스타트버튼 안눌리는것 때문에 테스트.
             { 
                 Log.Trace("isStopCon  &&  m_iStep" , string.Format(m_iStep.ToString()));
                 Log.Trace("bStopSw"                , bStopSw    ? "True" : "False");
                 Log.Trace("isErr"                  , isErr      ? "True" : "False");
                 Log.Trace("m_bReqStop"             , m_bReqStop ? "True" : "False");
                 Log.Trace("m_bRun"                 , m_bRun     ? "True" : "False");

                 m_bReqStop = true;
             }
             
             if (isResetCon && (m_iStep == EN_SEQ_STEP.Idle)) Reset() ;
             
             if (m_tmToStrt.OnDelay(m_iStep == EN_SEQ_STEP.ToStartCon || m_iStep == EN_SEQ_STEP.ToStart , 30000))
             {
                 //Trace Log.
                 string Msg ;
                 Msg = string.Format("ToStrtTimeOut : m_iStep=%d" ,m_iStep );
                 Log.Trace  ("SEQ",Msg);
                 ML.ER_SetErr (ei.ETC_ToStartTO);
                 m_iStep = EN_SEQ_STEP.Idle;
                 m_bRun  = false;
             }
             
             //CDelayTimer StopBtn = null;
             //StopBtn = new CDelayTimer();
             //if(m_iStep == EN_SEQ_STEP.scToStopCon)
             //{
             //    if(StopBtn.OnDelay(ML.IO_GetX((int)IP.xETC_StopSw)||ML.IO_GetX((int)IP.xETC_StopSw) , 5000))
             //    {
             //        Log.Trace("SEQ","Forced Stop");
             //        m_bRun = false ;
             //        m_iStep    = EN_SEQ_STEP.scIdle;
             //        m_bReqStop = false;
             //    }
             //}
             //else 
             //{
             //    StopBtn.Clear();
             //}
             
             
             if (m_tmToStop.OnDelay(m_iStep == EN_SEQ_STEP.ToStopCon || m_iStep == EN_SEQ_STEP.ToStop , 30000))      //  20000)) {
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
             
             
             //이상하게 중간에 랏엔드가 되는 현상 발견해서 넣어둠.
             bool bPreLotEnd = LOT.GetLotEnd() ;
             if(LOT.GetLotEnd() != bPreLotEnd) 
             {
                 Log.Trace("SEQ",LOT.GetLotEnd() ? "LotEnd True" : "LotEnd False");
             }
             bPreLotEnd = LOT.GetLotEnd() ;
             
             
             
             //Running Step.
             switch (m_iStep) {
                 case EN_SEQ_STEP.Idle       : return;
             
                 case EN_SEQ_STEP.ToStartCon : if(!ToStartCon()) return ;
                                                 m_iStep = EN_SEQ_STEP.ToStart;
                                                 Log.Trace("SEQ","scToStartCon END");
                                                 return ;
             
                 case EN_SEQ_STEP.ToStart    : if(!ToStart()) return ;
                                                 m_bRun = true ;
                                                 m_bRunEdge = true;                                                 
                                                 m_iStep = EN_SEQ_STEP.Run ;
                                                 Log.Trace("SEQ","scToStart END");
                                                 return ;
             
                 case EN_SEQ_STEP.Run        : if(!m_bReqStop) 
                                                 {
                                                     if(Autorun()) 
                                                     {
                                                         //랏엔드 상황.
                                                         //LOT.LotEnd();
                                                         Log.ShowMessage("Checked", "Lot Ended.");
                                                         Log.Trace("SEQ",LOT.GetLotNo() + "LotEnd");
                                                         m_bRunEnd = true;
                                                         m_iStep = EN_SEQ_STEP.ToStopCon ;
                                                     }
                                                     return ;
                                                 }
                                                 m_bReqStop = false ;
                                                 m_iStep = EN_SEQ_STEP.ToStopCon ;
                                                 Log.Trace("SEQ","scRun END");
                                                 return ;
             
                 case EN_SEQ_STEP.ToStopCon :  if(!ToStopCon()) 
                                                 {
                                                     if(Autorun())
                                                     {
                                                         //랏엔드 상황.
                                                         //LOT.LotEnd();
                                                         Log.Trace("SEQ","scToStopCon LotEnd");
                                                     }
                                                     return ;
                                                 }
                                                 m_bRun = false ;
                                                 m_iStep = EN_SEQ_STEP.ToStop;
                                                 Log.Trace("SEQ","scToStopCon END");
                                                 return ;
             
                 case EN_SEQ_STEP.ToStop    :  if (!ToStop()) return ;
                                                 m_iStep = EN_SEQ_STEP.Idle ;
                                                 m_bReqStop = false ;
                                                 
                                                 DM.SaveMap(); 
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
            if(!ML.IO_GetX(xi.ETC_MainAir   )) isOk = false ;

            //LOL if (!isOk && m_iSeqStat != EN_SEQ_STAT.ssError) SML.ER.SetErrMsg((int)ei.ETC_MainAir, "Cheked Main Air");
             
            return isOk ;
        }

        public static bool  InspectTemp()
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
            if (ML.IO_GetX(xi.ETC_FtLtEmgSw) ||
                ML.IO_GetX(xi.ETC_FtRtEmgSw) ||
                ML.IO_GetX(xi.ETC_RrEmgSw  )) 
            {
                ML.MT_EmgStopAll();
                ML.MT_SetServoAll(false);
                if(ML.IO_GetX(xi.ETC_FtLtEmgSw)) ML.ER_SetErr(ei.ETC_Emergency,"전면 좌측 Emergency Switch 가 눌렸습니다.");
                if(ML.IO_GetX(xi.ETC_FtRtEmgSw)) ML.ER_SetErr(ei.ETC_Emergency,"전면 우측 Emergency Switch 가 눌렸습니다.");
                if(ML.IO_GetX(xi.ETC_RrEmgSw  )) ML.ER_SetErr(ei.ETC_Emergency,"뒷면 Emergency Switch 가 눌렸습니다.");

                isOk = false ;
            }
            
            return isOk ;
        }
        
        public static bool  InspectLightGrid()
        {
            bool isOk = true ;
            return isOk ;
        }
                                                
        public static bool  InspectDoor()
        {
            //Local Var.
            bool isOk = true;

            if (/*!OM.CmnOptn.bIgnrDoor &&*/ m_iStep == EN_SEQ_STEP.Run)
            {
                if (!OM.CmnOptn.bIgnrDoor)
                {
                    if (!ML.IO_GetX(xi.ETC_FtOutDoor)) { ML.ER_SetErr(ei.ETC_Door, "앞문 열림."  ); isOk = false; }
                    if (!ML.IO_GetX(xi.ETC_LtOutDoor)) { ML.ER_SetErr(ei.ETC_Door, "좌측문 열림."); isOk = false; }
                    if (!ML.IO_GetX(xi.ETC_RrOutDoor)) { ML.ER_SetErr(ei.ETC_Door, "뒷문 열림."  ); isOk = false; }
                    if (!ML.IO_GetX(xi.ETC_RtOutDoor)) { ML.ER_SetErr(ei.ETC_Door, "우측문 열림."); isOk = false; }
                }
                
                
                if (!ML.IO_GetX(xi.ETC_FtInDoor  )) { ML.ER_SetErr(ei.ETC_Door, "차폐함 앞문 열림."); isOk = false; }
                if (!ML.IO_GetX(xi.ETC_RrInDoor  )) { ML.ER_SetErr(ei.ETC_Door, "차폐함 우측문 열림."   ); isOk = false; }
                if (!ML.IO_GetX(xi.ETC_RrInDoor  )) { ML.ER_SetErr(ei.ETC_Door, "차폐함 뒷문 열림."     ); isOk = false; }
            }

            //Ok.
            return isOk;
            
        }
        public static bool  InspectFlowMeter()
        {
            bool isOk = true ;
            //if( ML.IO_GetY(yi.BARZ_Blower)) {
            //    m_tmTemp.Clear();
            //}
            //if(!ML.IO_GetY(yi.BARZ_Blower) && !ML.IO_GetX(xi.ETC_FlowMeter) && m_tmTemp.OnDelay(500)) {
            //    ML.ER_SetErr(ei.ETC_FlowMeter, "Flow Meter Sensor Checked");
            //    isOk = false;
            //}
            
            return isOk ;
        }

        public static bool InspectMainServo()
        {
            bool isOk = true;

            if (!ML.IO_GetX(xi.ETC_MainServo))                
            {
            
                ML.MT_SetServoAll(false);
                
                isOk = false;
            }

            return isOk;
        }
                                                
        public static bool  InspectActuator()
        {
            //Local Var.
            bool isOk  = true ;
            bool isErr = false;
            
            //Inspect.
            for(ci i = 0 ; i < ci.MAX_ACTR ; i++) 
            {
                isErr = ML.CL_Err(i) ;
                if (isErr) { ML.ER_SetErr(ei.ATR_TimeOut, ML.CL_GetName(i)); isOk = false; }
            }
            
            //Ok.
            return isOk;
        }
                                                
        public static bool  InspectMotor()
        {
            //Local Var.
            bool isOk  = true;
            
            for(mi i = 0 ; i < mi.MAX_MOTR ; i++) 
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
            for(mi i = 0 ; i < mi.MAX_MOTR ; i++) {
                if (!ML.MT_GetHomeDone(i)){ML.ER_SetErr(ei.MTR_HomeEnd , ML.MT_GetName(i)); isOk = false; }
            }
            
            //Ok.
            return isOk;
        }

        public static bool InspectCrash()
        {
            bool bCrashed = false;
            //if (ML.IO_GetXUp(xi.TOOL_Crash)) {
            //    if (MM.GetManNo() != mc.AllHome){ //하드웨어 충돌 알람시에 올홈으로 풀수 있다.
            //        if (!ML.MT_GetStop(mi.TOOL_XLeft))
            //        {
            //            ML.MT_EmgStop(mi.TOOL_XLeft);
            //            ML.ER_SetErr(ei.PRT_Crash, "툴 하드웨어 충돌 센서가 감지 되었습니다.");
            //            bCrashed = true;
            //            
            //        }
            //        if (!ML.MT_GetStop(mi.TOOL_XRght))
            //        {
            //            ML.MT_EmgStop(mi.TOOL_XRght);
            //            ML.ER_SetErr(ei.PRT_Crash, "툴 하드웨어 충돌 센서가 감지 되었습니다.");
            //            bCrashed = true;
            //        }
            //    }
            //}
            //
            //if (ML.IO_GetXUp(xi.WSTG_EjtCrash)) {
            //    if (MM.GetManNo() != mc.AllHome){ //하드웨어 충돌 알람시에 올홈으로 풀수 있다.
            //        if (!ML.MT_GetStop(mi.TOOL_XEjtR))
            //        {
            //            ML.MT_EmgStop(mi.TOOL_XEjtR);
            //            ML.ER_SetErr(ei.PRT_Crash, "이젝터간 하드웨어 충돌 센서가 감지 되었습니다.");
            //            bCrashed = true;
            //        }
            //        if (!ML.MT_GetStop(mi.TOOL_XEjtL))
            //        {
            //            ML.MT_EmgStop(mi.TOOL_XEjtL);
            //            ML.ER_SetErr(ei.PRT_Crash, "이젝터간 하드웨어 충돌 센서가 감지 되었습니다.");
            //            bCrashed = true;
            //        }
            //    }
            //}
            //
            //if (ML.IO_GetXUp(xi.WSTG_EjtShortRt)) {
            //    if (MM.GetManNo() != mc.AllHome){ //하드웨어 충돌 알람시에 올홈으로 풀수 있다.
            //        if (!ML.MT_GetStop(mi.TOOL_XEjtR))
            //        {
            //            ML.MT_EmgStop(mi.TOOL_XEjtR);
            //            ML.ER_SetErr(ei.PRT_Crash, "오른쪽 이젝터와 웨이퍼링 충돌 센서가 감지 되었습니다.");
            //            bCrashed = true;
            //        }
            //    }
            //}
            //
            //if (ML.IO_GetXUp(xi.WSTG_EjtShortLt)) {
            //    if (MM.GetManNo() != mc.AllHome){ //하드웨어 충돌 알람시에 올홈으로 풀수 있다.
            //        if (!ML.MT_GetStop(mi.TOOL_XEjtL))
            //        {
            //            ML.MT_EmgStop(mi.TOOL_XEjtL);
            //            ML.ER_SetErr(ei.PRT_Crash, "왼쪽 이젝터와 웨이퍼링 충돌 센서가 감지 되었습니다.");
            //            bCrashed = true;
            //        }
            //    }
            //}
            //
            //double dLeftCmdPos = ML.MT_GetCmdPos(mi.TOOL_XLeft);
            //double dLeftTrgPos = ML.MT_GetTrgPos(mi.TOOL_XLeft);
            //double dLeftPos    =  dLeftCmdPos > dLeftTrgPos ? dLeftCmdPos : dLeftTrgPos ;
            //
            //double dRghtCmdPos = ML.MT_GetCmdPos(mi.TOOL_XRght);
            //double dRghtTrgPos = ML.MT_GetTrgPos(mi.TOOL_XRght);
            //double dRghtPos    =  dRghtCmdPos > dRghtTrgPos ? dRghtCmdPos : dRghtTrgPos ;
            //
            //
            //
            //if(dLeftPos + dRghtPos > OM.DevOptn.dToolCrashDist) {
            //    if(!ML.MT_GetStop(mi.TOOL_XLeft))ML.MT_Stop(mi.TOOL_XLeft);
            //    if(!ML.MT_GetStop(mi.TOOL_XRght))ML.MT_Stop(mi.TOOL_XRght);
            //    ML.ER_SetErr(ei.PRT_Crash, "툴들의 타겟 포지션이 툴간 충돌거리세팅 보다 큽니다.");
            //    bCrashed = true;
            //}




            return !bCrashed;
        }
                                                 
        public static bool  InspectStripDispr()
        {
            bool isOk  = true;

        //    if(DM.ARAY[riWRK].GetCntExist() && !IO_GetX(xWRK_Detect)  && !OM.MstOptn.bDryRun ) isOk = false ;
        //    if(DM.ARAY[riPSB].GetCntExist() &&  !OM.MstOptn.bDryRun) {
        //        if(!IO_GetX(xPSB_LDetect) && !IO_GetX(xPSB_RDetect) && !IO_GetX(xPSB_Pkg1) && !IO_GetX(xPSB_Pkg2) && !IO_GetX(xPSB_PkgOut1) && !IO_GetX(xPSB_PkgOut2)) isOk = false ;
        //    }
        //
        //    if (OM.EqpOptn.bExistLoader  ) { if(!DM.ARAY[riLDR].CheckAllStat(csNone) && ( !IO_GetX(xLDR_MgzDetect1) && !IO_GetX(xLDR_MgzDetect2) ) ) isOk = false ; }
        //    if (OM.EqpOptn.bExistUnLoader) { if(!DM.ARAY[riULD].CheckAllStat(csNone) && ( !IO_GetX(xULD_MgzDetect1) && !IO_GetX(xULD_MgzDetect2) ) ) isOk = false ; }
        //
            return isOk ;
        }
        
        public static void InspectBuzzer()
        {
            int iBuzzOffTime = OM.CmnOptn.iBuzzOffTime * 1000; //초단위로 바꾼다.

            if(iBuzzOffTime > 0)
            {
                if (ML.IO_GetY(yi.ETC_TwBzz) && m_tmBuzzer.OnDelay(iBuzzOffTime))
                {
                    m_tmTemp.Clear();
                    ML.TL_SetBuzzOff(true);
                }
            }
        }

        public static bool  InspectStripUnknown()
        {
        //    if (DM.ARAY[riPR2].CheckAllStat(csNone) && DM.ARAY[riPR1].CheckAllStat(csNone) && IO_GetX(xPRB_3Pkg) ) return false ;
            return true ;
        }
        

        

            //Running Functions.
        public static bool ToStartCon()   //스타트를 하기 위한 조건을 보는 함수.
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];

            //for(int i=0; i < (int)pi.MAX_PART; i++){
            //    bRet[i] = m_Part[i].ToStartCon();
            //}
            //
            //for(int i = 0 ; i < (int)pi.MAX_PART ;  i++) {
            //    if(!bRet[i]) return false ;
            //}

            bRet[0] = m_Part[0].ToStartCon();
                 if (OM.DevInfo.iMacroType == 0) bRet[1] = m_Part[1].ToStartCon(); //드레시
            else if (OM.DevInfo.iMacroType == 1) bRet[2] = m_Part[2].ToStartCon(); //이지센서

                 if (!bRet[0]                              ) return false;
                 if (OM.DevInfo.iMacroType == 0 && !bRet[1]) return false;
            else if (OM.DevInfo.iMacroType == 1 && !bRet[2]) return false;
            
            return true ;
        } 

        public static bool ToStopCon()     //스탑을 하기 위한 조건을 보는 함수.
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
            
            //for(int i=0; i < (int)pi.MAX_PART; i++){
            //    bRet[i] = m_Part[i].ToStopCon();
            //}
            //for(int i = 0 ; i < (int)pi.MAX_PART ; i++) {
            //    if(!bRet[i]) return false ;
            //}
            bRet[0] = m_Part[0].ToStopCon();
                 if (OM.DevInfo.iMacroType == 0) bRet[1] = m_Part[1].ToStopCon(); //드레시
            else if (OM.DevInfo.iMacroType == 1) bRet[2] = m_Part[2].ToStopCon(); //이지센서

                 if (!bRet[0]                              ) return false;
                 if (OM.DevInfo.iMacroType == 0 && !bRet[1]) return false;
            else if (OM.DevInfo.iMacroType == 1 && !bRet[2]) return false;
            
            return true ;
        } 
        
        public static bool ToStart()      //스타트를 하기 위한 함수.
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
            //Call ToStop.
            //for(int i=0; i < (int)pi.MAX_PART; i++){
            //    bRet[i] = m_Part[i].ToStart();
            //}           

            bRet[0] = m_Part[0].ToStart();
                 if (OM.DevInfo.iMacroType == 0) bRet[1] = m_Part[1].ToStart(); //드레시
            else if (OM.DevInfo.iMacroType == 1) bRet[2] = m_Part[2].ToStart(); //이지센서

            //for(int i = 0 ; i < (int)pi.MAX_PART ; i++) {
            //    if(!bRet[i]) return false ;
            //}

                 if (!bRet[0]                              ) return false;
                 if (OM.DevInfo.iMacroType == 0 && !bRet[1]) return false;
            else if (OM.DevInfo.iMacroType == 1 && !bRet[2]) return false;
            
            return true ;
        } 
        
        public static bool ToStop()       //스탑을 하기 위한 함수.
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
            //Call ToStop.
            //for(int i=0; i < (int)pi.MAX_PART; i++){
            //    bRet[i] = m_Part[i].ToStop();
            //}

            bRet[0] = m_Part[0].ToStop();
                 if (OM.DevInfo.iMacroType == 0) bRet[1] = m_Part[1].ToStop(); //드레시
            else if (OM.DevInfo.iMacroType == 1) bRet[2] = m_Part[2].ToStop(); //이지센서
            
            //for(int i = 0 ; i < (int)pi.MAX_PART ; i++) {
            //    if(!bRet[i]) return false ;
            //}
                 if (!bRet[0]                              ) return false;
                 if (OM.DevInfo.iMacroType == 0 && !bRet[1]) return false;
            else if (OM.DevInfo.iMacroType == 1 && !bRet[2]) return false;
            
            return true ;
        }
        
        public static bool Autorun()      //오토런닝시에 계속 타는 함수. 
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
            //Call ToStop.
            //for(int i=0; i < (int)pi.MAX_PART; i++){
            //    bRet[i] = m_Part[i].Autorun();
            //}
            bRet[0] = m_Part[0].Autorun();
                 if (OM.DevInfo.iMacroType == 0) bRet[1] = m_Part[1].Autorun(); //드레시
            else if (OM.DevInfo.iMacroType == 1) bRet[2] = m_Part[2].Autorun(); //이지센서

            //for(int i = 0 ; i < (int)pi.MAX_PART ; i++) {
            //    if (!bRet[i]) return false ;
            //}
                 if (!bRet[0]                              ) return false;
                 if (OM.DevInfo.iMacroType == 0 && !bRet[1]) return false;
            else if (OM.DevInfo.iMacroType == 1 && !bRet[2]) return false;

            //ML.ER.SetErrMsg((int)ei.PRT_TrayErr, "Have Not Tray");
            
            return true ;
        }
        
        static private EN_SEQ_STAT iPreStat = EN_SEQ_STAT.MAX_SEQ_STAT;
        static private EN_SEQ_STAT iStat;
        public static void UpdateSeqState() 
        {
            
            bool isInit    =  MM.GetManNo() == mc.AllHome ;
            bool isError   =  ML.ER_IsErr() ;                 
            bool isRunning =  m_bRun     ;
            bool isRunWarn =  m_bRunEnd  ;
            bool isManual  = MM.GetManNo() > mc.AllHome;
            bool isStop    = !m_bRun && !m_bRunEnd;                 
            
            bool isLotEnd  =  LOT.GetLotEnd() ;
            
            //Flicking Timer.
            if (m_bFlick) { m_tmFlickOn .Clear(); if (m_tmFlickOff.OnDelay( m_bFlick , 500)) m_bFlick = false; }
            else          { m_tmFlickOff.Clear(); if (m_tmFlickOn .OnDelay(!m_bFlick , 500)) m_bFlick = true ; }
            
            //Set Sequence State. Tower Lamp
               //  if (isMaint  ) { m_iSeqStat = EN_SEQ_STAT.Maint      ;}
                 if (isInit   ) { m_iSeqStat = EN_SEQ_STAT.Init       ;}
            else if (isLotEnd ) { m_iSeqStat = EN_SEQ_STAT.WorkEnd    ;}
            else if (isError  ) { m_iSeqStat = EN_SEQ_STAT.Error      ;}
            else if (isRunning) { m_iSeqStat = EN_SEQ_STAT.Running    ;}
            else if (isManual ) { m_iSeqStat = EN_SEQ_STAT.Manual     ;}
            else if (isStop   ) { m_iSeqStat = EN_SEQ_STAT.Stop       ;}
            else if (isRunWarn) {  
                                    m_iSeqStat = EN_SEQ_STAT.RunWarn    ;
                                    Log.ShowMessage("Warning", "Work End");
                                }
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
                    }
                    Log.Trace(sStatText, ti.Sts);
                }

                iPreStat = m_iSeqStat;
            }

        }        
    }
}
