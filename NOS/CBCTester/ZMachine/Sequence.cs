﻿using System;
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
        
        [DllImport("user32", EntryPoint = "FindWindow")]
        static private extern IntPtr FindWindow(string IPClassName, String IpWindowName);
        [DllImport("User32", EntryPoint = "SetForegroundWindow")]
        static private extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static public  extern bool ShowWindow(IntPtr hWnd, int nCmdShow); 
        private const int SW_SHOWMAXVIEW = 3;

        static Thread MainThread = new Thread(new ThreadStart(Update));
        static double m_dMainThreadCycleTime ; public static double _dMainThreadCycleTime{get{return m_dMainThreadCycleTime ;}}


        static public Loader   LDR = new Loader   ();
        static public Picker   PKR = new Picker   ();
        static public QcBuffer QCB = new QcBuffer ();
        static public Syringe  SYR = new Syringe  ();
        static public Chamber  CHA = new Chamber  ();
        static public Part[] m_Part = new Part[(int)pi.MAX_PART];

        //static public VisnCom   Visn = new VC() ;

        static CDelayTimer m_tmToStop   ;
        static CDelayTimer m_tmToStrt   ;
        static CDelayTimer m_tmFlickOn  ;
        static CDelayTimer m_tmFlickOff ;
        static CDelayTimer m_tmCloseDoor;
        static CDelayTimer m_tmTemp     ;

        static public CCycleTimer m_cyWorktime;
        

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
        static public bool bWorkTimer;

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
    
        static public EN_SEQ_STEP  _iStep    { get {return m_iStep   ;}}
        static public EN_SEQ_STAT  _iSeqStat { get {return m_iSeqStat;}}

        //static public string m_sBarcode ;

        static public RS232_DataMan50 Bar = new RS232_DataMan50(5,"Barcode"); //sun
        static public USB_Spectrometer Spec = new USB_Spectrometer();
        //1:펌프 , 2: , 3:바코드 
        static public RS485_HamiltonSyringePump SyringePump = new RS485_HamiltonSyringePump(3, "SylingePump", 6);

        static public string sFcmIp = "192.168.1.100" ;
        static public int    iFcmPort   = 25534 ;

        static public TCPIP_NewOpticsFCM FcmTester = new TCPIP_NewOpticsFCM(sFcmIp, iFcmPort);// ("127.0.0.1",25534);//("192.168.0.99" , 25534);

        public static void Init(int _iWidth, int _iHeight)
        {
            Log.StartLogMan();
            //Common
            SM.TPara Para;
            Para.sParaFolderPath = Directory.GetCurrentDirectory() + "\\Util\\";
            //Para.iWidth    = 1280;
            //Para.iHeight   = 863;
            Para.iWidth    = _iWidth ;
            Para.iHeight   = _iHeight;
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
            Para.Dio.eDioSel       = EN_DIO_SEL.NMC2;
            //Para.Dio.eDioSel       = EN_DIO_SEL.AXL;
            Para.Dio.eX            = new xi();
            Para.Dio.eY            = new yi();
            Para.bTabHides[1]      = false   ;
            //A IO
            Para.Aio.eAioSel       = EN_AIO_SEL.None;
            Para.Aio.eX            = new ax();
            Para.Aio.eY            = new ay();
            Para.Aio.iRangeAMin    = 0       ;
            Para.Aio.iRangeAMax    = 0       ;
            Para.bTabHides[2]      = true    ;

            //TowerLamp
            Para.bTabHides[3]   = false;

            //Cylinder
            Para.Cyl.eCyl          = new ci();
            Para.bTabHides[4]      = false ;

            //Motor          
            Para.Mtr.eMtrSel = new EN_MTR_SEL[(int)mi.MAX_MOTR];
            for(int i=0; i<(int)mi.MAX_MOTR; i++)
            {
                Para.Mtr.eMtrSel[i] = EN_MTR_SEL.UiRobot;
            }
            Para.Mtr.eMtr = new mi();
            Para.bTabHides[5]     = false ;
            
            SM .Init(Para);
            OM .Init();
            DM .Init();
            SPC.Init();
            PM .Init(Pstn.Cnt);
            

            //ArrayPos.TPara PosPara ;//= new ArrayPos.TPara();
            //PosPara.dColGrGap  = OM.DevInfo.dColGrGap  ;
            //PosPara.iColCnt    = OM.DevInfo.iColCnt    ;
            //PosPara.iRowCnt    = OM.DevInfo.iRowCnt    ;
            //PosPara.dColPitch  = OM.DevInfo.dColPitch  ;
            //PosPara.dRowPitch  = OM.DevInfo.dRowPitch  ;
            //PosPara.iColGrCnt  = OM.DevInfo.iColGrCnt  ;
            //PosPara.iRowGrCnt  = OM.DevInfo.iRowGrCnt  ;
            //PosPara.dColGrGap  = OM.DevInfo.dColGrGap  ;
            //PosPara.dRowGrGap  = OM.DevInfo.dRowGrGap  ;
            //
            //PosPara.iColSbGrCnt= OM.DevInfo.iColSbGrCnt;
            //PosPara.iRowSbGrCnt= OM.DevInfo.iRowSbGrCnt;
            //
            //PosPara.dRowSbGrGap= OM.DevInfo.dRowSbGrGap;
            //PosPara.dColSbGrGap= OM.DevInfo.dColSbGrGap;
            //if(!OM .StripPos.SetPara(PosPara))
            //{
            //    Log.ShowMessage("Strip Position Err" , OM .StripPos.Error);
            //}

            //ML.IO_SetY(yi.ETC_MainAirOnOff , true) ;

            MainThread.Priority = ThreadPriority.Highest;
            MainThread.Start();

            m_tmToStop      = new CDelayTimer ();
            m_tmToStrt      = new CDelayTimer ();
            m_tmFlickOn     = new CDelayTimer ();
            m_tmFlickOff    = new CDelayTimer ();
            m_tmCloseDoor   = new CDelayTimer ();
            m_tmTemp        = new CDelayTimer ();

            m_cyWorktime = new CCycleTimer();

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

            

            m_Part[(int)pi.LDR] = LDR; 
            m_Part[(int)pi.PKR] = PKR; 
            m_Part[(int)pi.QCB] = QCB; 
            m_Part[(int)pi.SYR] = SYR; 
            m_Part[(int)pi.CHA] = CHA; 
        }

        public static void Close()
        {
            MainThread.Abort();
            MainThread.Join();

            

            SM  .Close();
            OM  .Close();
            //LOT .Close();
            SPC .Close();
        }

        //static CDelayTimer PumpChargingTimer = new CDelayTimer();
        public static void Update()
        {
            List<double> lst = new List<double>();
            double dPreTime  = CTimer.GetTime_us();
            double dCrntTime;
            double d1 = 0;
            while (true)
            {
                dCrntTime = CTimer.GetTime_us();
                m_dMainThreadCycleTime = (dCrntTime - dPreTime) / 1000.0;
                dPreTime = dCrntTime;
                Thread.Sleep(0);

                //Motion Dll Update
                SM.Update(m_iSeqStat);
                
                //Main Air pump On/Off
                //기본적으로 오토런시에만 메인펌프를 켜는데 메뉴얼 동작시에도 켜야 되서.
                bool bMovingCylinder = false ;
                for(int i = 0 ; i < (int)ci.MAX_ACTR ; i++)
                {
                    if(!ML.CL_Complete(i ))
                    {
                        bMovingCylinder = true ;
                    }
                }

                //if(bMovingCylinder || m_iStep != EN_SEQ_STEP.Idle)
                //{
                //    ML.IO_SetY(yi.ETC_MainPumpOn , true);
                //}
                //else
                //{
                //    ML.IO_SetY(yi.ETC_MainPumpOn , false);
                //}
                for(yi y = yi.TankCP1_InSt; y <= yi.Sol76; y++)
                {

                }


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
                InspectLightGrid();

                //Update ErrorProc.
                UpdateButton();       

                //Check Button.
                UpdateSeqState();     

                //Update Motor State (Input)
                MM.Update();         


                //SPC
                //SPC.Update(LOT.GetLotNo() , OM.GetCrntDev() , m_iSeqStat , OM.EqpStat.bMaint);
                SPC.Update(OM.EqpStat.sBloodID , OM.GetCrntDev() , m_iSeqStat , false);

                SyringePump.Update();

                //오토큐씨 모드 일때 기다렸다 스타트 하는부분.
                if(OM.CmnOptn.bAutoQc && _iStep == EN_SEQ_STEP.Idle )
                {
                    //if(bNeedStartQC)
                    DateTime dtNow       = DateTime.Now;
                    DateTime dtTime      = new DateTime(dtNow.Year , dtNow.Month , dtNow.Day , dtNow.Hour , dtNow.Minute , dtNow.Second) ;
                    DateTime dtQCStart   = new DateTime(dtNow.Year , dtNow.Month , dtNow.Day , OM.CmnOptn.iQcStartHour , OM.CmnOptn.iQcStartMin    , 0) ; 

                    
                    DateTime dtPumpStart = new DateTime(dtNow.Year , dtNow.Month , dtNow.Day , OM.CmnOptn.iQcStartHour , OM.CmnOptn.iQcStartMin    , 0) ;
                    dtPumpStart = dtPumpStart.AddMinutes(-1);//1분전에 미리 펌프온.

                    //펌프 스타트.
                    if(dtTime >= dtPumpStart && dtTime <= dtQCStart)
                    {
                        Part.IO_SetY(yi.ETC_MainPumpOff , false );
                    }

                    //1초가 넘어가면 동작 안함.
                    if(dtTime == dtQCStart ) //1초안에 안들어오면 재수없으면 안될수 있는데 귀찮아서 이렇게함. 원래는 엣지변수 하나 둬서 잡아야함.
                    {
                        Part.IO_SetY(yi.ETC_MainPumpOff, false);//혹시 몰라서 한번 더 함.
                        DM.ARAY[ri.FRG].SetStat(cs.Freeze);
                        m_bBtnStart = true ;
                        

                    }
                }
            }
        }

        //    CPartInterface * m_pPart[MAX_PART] ;

        //--------------------------------------------------------------------------------------------------------

        public static void Reset()
        {
            //Check running flag.
            if (m_bRun                               ) return;
            if (m_iSeqStat   == EN_SEQ_STAT.Init     ) return;
            if (m_iStep      == EN_SEQ_STEP.ToStopCon) return;
            if (m_iStep      == EN_SEQ_STEP.ToStop   ) return;
            if(MM.GetManNo() != mc.NoneCycle         ) return;
            //LOT.Reset();
            
            Log.Trace("Seq","Reset");
            
            m_tmToStop   .Clear();
            m_tmToStrt   .Clear();
            m_tmFlickOn  .Clear();
            m_tmFlickOff .Clear();
            m_tmTemp     .Clear();
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
            for(int i=0; i < (int)pi.MAX_PART; i++)
            {
                m_Part[i].Reset();
            }

            //Cylinder Reset , Need to First Time
            ML.CL_Reset();
            
            //Lot End Flag Reset.
            //LOT.Reset();
            Log.CloseForm();
            
            m_iSeqStat = EN_SEQ_STAT.Stop;

            //System.
            ML.MT_ResetAll();
            ML.MT_SetServoAll(true);

            //VSNZ존에서 처리함.
            //for(int i = 0 ; i < (int)vi.MAX_VI; i++)
            //{
            //    if(!OM.VsSkip((vi)i)) VSNZ.VisnComs[i].SendCmd(VisnCom.vc.Reset);
            //}
        }

        

        private static void UpdateButton  ()
        {
            
            if(m_iStep != EN_SEQ_STEP.Idle) 
            {
                InspectHomeDone () ;
                if(!OM.CmnOptn.bIgnrDoor) InspectDoor();
            }

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
        
            bool bStartSw   = m_bBtnStart; //ML.IO_GetXUp(xi.ETC_StartSw)  ||m_bBtnStart;
            bool bStopSw    = m_bBtnStop ; //ML.IO_GetXUp(xi.ETC_StopSw )  ||m_bBtnStop ;
            bool bResetSw   = m_bBtnReset; //ML.IO_GetXUp(xi.ETC_ResetSw)  ||m_bBtnReset;
            bool bAirSw     = m_bBtnAir  ; //ML.IO_GetXUp(xi.ETC_AirSw  )  ||m_bBtnAir  ;
            //bool bInitSw    = ML.IO_GetXUp(xi.ETC_InitSw ) ;
            
                                                                                   
            if(bStartSw)
            {
                Log.Trace("bStartSw","Started");
        
                
            }
        
            //Update Switch's Lamp
            bool bStopBtnFlick = (m_iStep == EN_SEQ_STEP.ToStopCon || m_iStep == EN_SEQ_STEP.ToStop) && m_bFlick ;
            
            //버튼 클릭시나
            //ML.IO_SetY(yi.ETC_StartLp  , ML.IO_GetX(xi.ETC_StartSw ) ||  m_bRun                        );
            //ML.IO_SetY(yi.ETC_StopLp   , ML.IO_GetX(xi.ETC_StopSw  ) || !m_bRun || bStopBtnFlick       );
            //ML.IO_SetY(yi.ETC_ResetLp  , ML.IO_GetX(xi.ETC_ResetSw ) || (m_bFlick && isErr)            );
            //ML.IO_SetY(yi.ETC_AirLp    , ML.IO_GetX(xi.ETC_AirSw   ) || ML.IO_GetY(yi.ETC_MainAirOnOff));
            //ML.IO_SetY(yi.ETC_InitLp   , ML.IO_GetX(xi.ETC_InitSw  )                                   );
            
            //Center Man Button
            m_bBtnStart = false ;
            m_bBtnStop  = false ;
            m_bBtnReset = false ;
            m_bBtnAir   = false ;
            

            //Init. Button Flags.
            if (bStartSw)
            {
                if(MM.GetManNo() != (int)mc.NoneCycle) 
                {
                    Log.ShowMessage("Error","메뉴얼 동작중 입니다.");
                    bStartSw = false;
                }
                if(!isHomeEnd        ) { Log.ShowMessage("Error" , "Machine Needed Initial"  ); bStartSw = false ; }
                //if(!LOT.GetLotOpen() && LOT.GetNextMgz()=="") { Log.ShowMessage("Error" , "Machine Need to Lot "  ); bStartSw = false ; } //기존 랏오픈 패턴
                //if(LOT.GetNextMgz()=="") { Log.ShowMessage("Error" , "Machine Needed Lot List Registration"  ); bStartSw = false ; } //기존 랏오픈 패턴

                if (m_iSeqStat == EN_SEQ_STAT.WorkEnd || m_iSeqStat == EN_SEQ_STAT.RunWarn) Reset();
            }

            //if (bInitSw)
            //{
            //    if (!ML.IO_GetY(yi.ETC_MainAirOnOff))
            //    {
            //        ML.ER_SetErr(ei.ETC_MainAir, "Check Main Air");
            //        return;
            //    }
            //    
            //    MM.SetManCycle(mc.AllHome);
            //}
            
            //Air Switch.
            //if(bAirSw && !m_bRun && m_iSeqStat != EN_SEQ_STAT.Init)
            //{
            //    ML.IO_SetY(yi.ETC_MainAirOnOff , !ML.IO_GetY(yi.ETC_MainAirOnOff )) ;
            //}
            
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
                //진섭
                //if (!_bRun && !MM.Working())
                //{
                //    ML.IO_SetY(yi.RAIL_FeedingAC1,false);
                //    ML.IO_SetY(yi.RAIL_FeedingAC2,false);
                //    ML.IO_SetY(yi.RAIL_FeedingAC3,false);
                //}
                MM.Stop = true;
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
            
            if (m_tmToStrt.OnDelay(m_iStep == EN_SEQ_STEP.ToStartCon || m_iStep == EN_SEQ_STEP.ToStart , 10000))
            {
                //Trace Log.
                string Msg ;
                Msg = string.Format("ToStrtTimeOut : m_iStep=" ,m_iStep.ToString() );
                Log.Trace  ("SEQ",Msg);
                ML.ER_SetErr (ei.ETC_ToStartTO);
                m_iStep = EN_SEQ_STEP.Idle;
                m_bRun  = false;
            }

            bool bStopping = m_iStep == EN_SEQ_STEP.ToStopCon || m_iStep == EN_SEQ_STEP.ToStop;
            bool bChaRun = CHA.GetStep().eSeq != Chamber.sc.Idle;
            bool bSylWorking = SyringePump.GetBusy((int)PumpID.Blood) ||
                               SyringePump.GetBusy((int)PumpID.DC   ) ||
                               SyringePump.GetBusy((int)PumpID.FCM  ) ||
                               SyringePump.GetBusy((int)PumpID.FDS  ) ||
                               SyringePump.GetBusy((int)PumpID.NR   ) ||
                               SyringePump.GetBusy((int)PumpID.RET  ) ;

            bool bChaWorking = CHA.GetSeqStep() != (int)Chamber.sc.Idle ;


            if (m_tmToStop.OnDelay(bStopping && !bSylWorking && !bChaWorking, 30000))    
            {
                //Trace Log.
                string Msg;
                Msg = string.Format("ToStopTimeOut : m_iStep=", m_iStep.ToString()  );
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
                case EN_SEQ_STEP.Idle :
                    return;

                case EN_SEQ_STEP.ToStartCon :
                    if (!ToStartCon()) return ;
                    m_iStep = EN_SEQ_STEP.ToStart;
                    Log.Trace("SEQ","scToStartCon END");
                    return ;
            
                case EN_SEQ_STEP.ToStart :
                    if (!ToStart()) return ;
                    m_bRun = true ;
                    m_bRunEdge = true;                                                 
                    m_iStep = EN_SEQ_STEP.Run ;
                    Log.Trace("SEQ","scToStart END");
                    return ;
            
                case EN_SEQ_STEP.Run :
                    if (!m_bReqStop) 
                    {
                        if(Autorun()) 
                        {
                            //랏엔드 상황.
                            //LOT.LotEnd();
                            //Log.Trace("SEQ",LOT.GetLotNo() + "LotEnd");
                            Log.ShowMessage("Checked", "작업 종료");

                            OM.EqpStat.sBloodID = "" ;
                            
                            m_bRunEnd = true;
                            m_iStep = EN_SEQ_STEP.ToStopCon ;
                        }
                        return ;
                    }
                    m_bReqStop = false ;
                    m_iStep = EN_SEQ_STEP.ToStopCon ;
                    Log.Trace("SEQ","scRun END");
                    return ;
            
                case EN_SEQ_STEP.ToStopCon :
                    if (!ToStopCon()) 
                    {
                        if(Autorun())
                        {
                            //랏엔드 상황.
                            //LOT.LotEnd();
                            Log.Trace("SEQ", "정지중 작업 종료");

                            OM.EqpStat.sBloodID = "" ;
                        }
                        return ;
                    }
                    m_bRun = false ;
                    m_iStep = EN_SEQ_STEP.ToStop;
                    Log.Trace("SEQ","scToStopCon END");
                    return ;
            
                case EN_SEQ_STEP.ToStop    :
                    if (!ToStop()) return ;
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
        
            //if(!ML.IO_GetX(xi.ETC_MainAir   ))
            //{
            //    ML.ER_SetErr((int)ei.ETC_MainAir, "Cheked Main Air");
            //    isOk = false ;
            //}
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
            if (ML.IO_GetX(xi.ETC_EmgSw) )
            {
                ML.MT_EmgStopAll();
                ML.MT_SetServoAll(false);
                ML.ER_SetErr(ei.ETC_Emergency,"Emergency Switch pushed."   );
            
                isOk = false ;
            }
            
            return isOk ;
        }
        
        public static bool InspectLightGrid()
        {
            bool isOk = true ;
        
        
            return isOk ;
        }
                                                
        public static bool InspectDoor()
        {
            //Local Var.
            bool isOk = true;
            
            //if (!ML.IO_GetX(xi.ETC_DoorFt)){ML.ER_SetErr(ei.ETC_Door, "Front Door Opened");isOk= false;}
            //if (!ML.IO_GetX(xi.ETC_DoorLt)){ML.ER_SetErr(ei.ETC_Door, "Left Door Opened" );isOk= false;}
            //if (!ML.IO_GetX(xi.ETC_DoorRr)){ML.ER_SetErr(ei.ETC_Door, "Rear Door Opened");isOk= false;}
            //if (!ML.IO_GetX(xi.ETC_DoorRt)){ML.ER_SetErr(ei.ETC_Door, "Right Door Opened" );isOk= false;}
        
            //Ok.
            return isOk;
            
        }
        
        public static bool InspectActuator()
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
                if (ML.MT_GetAlarmSgnl(i) ){
                    if(ML.MT_GetServo(i)) ML.MT_SetServo(i, false);
                    ML.ER_SetErr(ei.MTR_Alarm  , ML.MT_GetName(i));
                    isOk = false;
                }
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
            return !bCrashed;
        }
                                                
        
            //Running Functions.
        public static bool ToStartCon()   //스타트를 하기 위한 조건을 보는 함수.
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
        
            for(int i=0; i < (int)pi.MAX_PART; i++){
                bRet[i] = m_Part[i].ToStartCon();
            }
            
            for(int i = 0 ; i < (int)pi.MAX_PART ;  i++) {
                if(!bRet[i]) return false ;
            }
            
            return true ;
        } 
        
        public static bool ToStopCon()     //스탑을 하기 위한 조건을 보는 함수.
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
            
            for(int i=0; i < (int)pi.MAX_PART; i++){
                bRet[i] = m_Part[i].ToStopCon();
            }
            for(int i = 0 ; i < (int)pi.MAX_PART ; i++) {
                if(!bRet[i]) return false ;
            }
            
            return true ;
        } 
        
        public static bool ToStart()      //스타트를 하기 위한 함수.
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
            //Call ToStop.
            for(int i=0; i < (int)pi.MAX_PART; i++){
                bRet[i] = m_Part[i].ToStart();
            }           
            
            for(int i = 0 ; i < (int)pi.MAX_PART ; i++) {
                if(!bRet[i]) return false ;
            }
            
            return true ;
        } 
        
        public static bool ToStop()       //스탑을 하기 위한 함수.
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
            //Call ToStop.
            for(int i=0; i < (int)pi.MAX_PART; i++){
                bRet[i] = m_Part[i].ToStop();
            }
            
            for(int i = 0 ; i < (int)pi.MAX_PART ; i++) {
                if(!bRet[i]) return false ;
            }
            
            return true ;
        }
        
        public static bool Autorun()      //오토런닝시에 계속 타는 함수. 
        {
            bool[] bRet = new bool[(int)pi.MAX_PART];
            //Call ToStop.
            //for(int i=0; i < (int)pi.MAX_PART; i++){
            //    bRet[i] = m_Part[i].Autorun();
            //}
            for(int i=(int)pi.MAX_PART-1; i >= 0; i--){
                bRet[i] = m_Part[i].Autorun();
            }
            
            for(int i = 0 ; i < (int)pi.MAX_PART ; i++) {
                if (!bRet[i]) return false ;
            }
        
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
            bool isRunWarn =  false ; //m_bRunEnd  ; 일단 랏이 없어 랏앤드로 쓰자.
            bool isManual  = MM.GetManNo() > mc.AllHome;
            bool isStop    = !m_bRun && !m_bRunEnd;
            bool isToStart =  m_iStep == EN_SEQ_STEP.ToStartCon || m_iStep == EN_SEQ_STEP.ToStart ;
            bool isToStop  =  m_iStep == EN_SEQ_STEP.ToStopCon  || m_iStep == EN_SEQ_STEP.ToStop  ;
            bool isLotEnd  =  m_bRunEnd ; //LOT.GetLotEnd() ;
            
            //Flicking Timer.
            if (m_bFlick) { m_tmFlickOn .Clear(); if (m_tmFlickOff.OnDelay( m_bFlick , 500)) m_bFlick = false; }
            else          { m_tmFlickOff.Clear(); if (m_tmFlickOn .OnDelay(!m_bFlick , 500)) m_bFlick = true ; }
            
            //Set Sequence State. Tower Lamp
               //  if (isMaint  ) { m_iSeqStat = EN_SEQ_STAT.Maint      ;}
                 if (isInit   ) { m_iSeqStat = EN_SEQ_STAT.Init       ;}
            else if (isLotEnd ) { m_iSeqStat = EN_SEQ_STAT.WorkEnd    ;}
            else if (isError  ) { m_iSeqStat = EN_SEQ_STAT.Error      ;}
            else if (isToStart) { m_iSeqStat = EN_SEQ_STAT.ToStart    ;}
            else if (isToStop ) { m_iSeqStat = EN_SEQ_STAT.ToStop     ;}
            else if (isRunning) { m_iSeqStat = EN_SEQ_STAT.Running    ;}
            else if (isManual ) { m_iSeqStat = EN_SEQ_STAT.Manual     ;}
            else if (isStop   ) { m_iSeqStat = EN_SEQ_STAT.Stop       ;}
            else if (isRunWarn) { m_iSeqStat = EN_SEQ_STAT.RunWarn    ;Log.ShowMessage("Warning", "Work End");}
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
                    Log.Trace(sStatText, ForContext.Sts);
                }
        
                iPreStat = m_iSeqStat;
            }
        
        }        
    }
}
