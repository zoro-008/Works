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

        static public Left         LEFT = new Left      ((int)pi.LEFT + ti.Max);
        static public Righ         RIGH = new Righ      ((int)pi.RIGH + ti.Max);
        static public Macro        MCR  = new Macro     (6);
        static public Part[] m_Part = new Part[(int)pi.MAX_PART];

        static CDelayTimer m_tmToStop   ;
        static CDelayTimer m_tmToStrt   ;
        static CDelayTimer m_tmFlickOn  ;
        static CDelayTimer m_tmFlickOff ;
        static CDelayTimer m_tmCloseDoor;
        static CDelayTimer m_tmTemp     ;

        static CDelayTimer m_tmTemp1;
        static CDelayTimer m_tmTemp2;
        static CDelayTimer m_tmTemp3;       
        
        static public CCycleTimer m_cyTrayWorktime ;
        

        static private bool m_bBtnReset ; static private bool m_bBtnReset1 ;
        static private bool m_bBtnStart ; static private bool m_bBtnStart1 ;
        static private bool m_bBtnStop  ; static private bool m_bBtnStop1  ;
        static private bool m_bBtnAir   ; static private bool m_bBtnAir1   ;

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
        static public bool _bBtnReset { set{ m_bBtnReset = value; } } static public bool _bBtnReset1 { set{ m_bBtnReset1 = value; } }
        static public bool _bBtnStart { set{ m_bBtnStart = value; } } static public bool _bBtnStart1 { set{ m_bBtnStart1 = value; } }
        static public bool _bBtnStop  { set{ m_bBtnStop  = value; } } static public bool _bBtnStop1  { set{ m_bBtnStop1  = value; } }
        static public bool _bBtnAir   { set{ m_bBtnAir   = value; } } static public bool _bBtnAir1   { set{ m_bBtnAir1   = value; } }

        static public bool _bRun      { get{ return m_bRun      ; } }
        static public bool _bRunEdge  { get{ return m_bRunEdge  ; } }
        static public bool _bReqStop  { get{ return m_bReqStop  ; } }
        static public bool _bFlick    { get{ return m_bFlick    ; } }
                      
        static public bool _bInspDispr { get {return m_bInspDispr;} set{m_bInspDispr = value;} }
        static public bool _bInspUnkwn { get {return m_bInspUnkwn;} set{m_bInspUnkwn = value;} }
    
        static public EN_SEQ_STEP  _iStep    { get {return m_iStep   ;}}
        static public EN_SEQ_STAT  _iSeqStat { get {return m_iSeqStat;}}

        //Serial통신
        //static public RS232_KBS205        LoadCell    = new RS232_KBS205       (1, "LoadCell", ""  );    
        //static public RS232_SuperSigmaCM2 Dispr       = new RS232_SuperSigmaCM2(2, "Dispensor"     );   
        //static public RS232_LK_G5001      HeightSnsr  = new RS232_LK_G5001     (3, "HeightSensor"  );    
        //static public RS485_TK4S          Temp        = new RS485_TK4S         (5, "TempControl" ,1);    
        //static public RS232_110Xi4        BarcordPrnt = new RS232_110Xi4       (0, "BarcordPrnt"   );    
        //static public RS232_3310g         BarcordLODR = new RS232_3310g        (1, "BarcordLODR"   );  
        //static public RS232_3310g         BarcordBARZ = new RS232_3310g        (2, "BarcordBARZ"   );           
        

        //static public DispensePattern DispPtrn = new DispensePattern();
        //static public HeightPattern   HghtPtrn = new HeightPattern  ();

        //static public CSerialPort[] Com = new CSerialPort[(int)si.MAX_RS232];

        public static void Init()
        {
            SM.TPara Para;            
            Para.sParaFolderPath = Directory.GetCurrentDirectory() + "\\Util\\";
            Para.iWidth     = 1280;
            Para.iHeight    = 863;
            Para.bTabHides  = new bool[6];
            
            //Para.bTabHides[3] = true;
            //Para.bTabHides[2] = true;
            Para.bUseErrPic = true;
            Para.iCntErr    = 60;

            Para.iCntDIn    = 32;
            Para.iCntDOut   = 32;
            Para.eDio       = EN_DIO_SEL.AXL;

            Para.iCntAIn    = 8;
            Para.iCntAOut   = 0;
            Para.iRangeAMin = -10 ;
            Para.iRangeAMax =  10 ;
            Para.eAio       = EN_AIO_SEL.AXL;
            Para.bTabHides[2] = false;

            Para.iCntCylinder = (int)ci.MAX_ACTR;

            Para.eLanSel = EN_LAN_SEL.English;
            
            Para.iCntMotr = (int)mi.MAX_MOTR;
            Para.eMotors = new EN_MOTR_SEL[Para.iCntMotr];
            Para.eMotors[0] = EN_MOTR_SEL.AXL;
            Para.eMotors[1] = EN_MOTR_SEL.AXL;
            Para.eMotors[2] = EN_MOTR_SEL.AXL;

            SM.Init(Para);
            OM .Init();
            DM .Init();
            LOT.Init();
            SPC.Init();
            PM .Init(PM.PstnCnt);
       
            
            //VisnCom.TPara VisnPara = new VisnCom.TPara();
            //VisnPara.sVisnPcName     = "Visn"          ; //파일저장시에 파일명에 삽입.
            //VisnPara.sVisnFolder     = "c:\\Data"      ; //파일저장 하는 폴더.
            //VisnPara.xVisn_Ready     = xi.VISN_Ready   ; 
            //VisnPara.xVisn_Busy      = xi.VISN_Busy    ;

            //VisnPara.yVisn_Command   = yi.VISN_Command ;
            //VisnPara.yVisn_JobChange = yi.VISN_Change  ;
            //VisnPara.yVisn_Reset     = yi.VISN_Reset   ;
            //VisnPara.yVisn_ManMode   = yi.VISN_ManMode ;
            //VisnPara.yVisn_ManInsp   = yi.VISN_ManInsp ;
            //Visn.Init(ref VisnPara);
            
            MainThread.Priority = ThreadPriority.Highest;
            //MainThread.Priority = ThreadPriority.Normal;
            MainThread.Start();

            m_tmToStop      = new CDelayTimer ();
            m_tmToStrt      = new CDelayTimer ();
            m_tmFlickOn     = new CDelayTimer ();
            m_tmFlickOff    = new CDelayTimer ();
            m_tmCloseDoor   = new CDelayTimer ();
            m_tmTemp        = new CDelayTimer ();
            m_tmTemp1       = new CDelayTimer ();
            m_tmTemp2       = new CDelayTimer ();
            m_tmTemp3       = new CDelayTimer ();

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

            m_Part[(int)pi.LEFT] = LEFT; //m_Part[(int)pi.LODR].SetPartId((int)pi.LODR + ti.Max);
            m_Part[(int)pi.RIGH] = RIGH; //m_Part[(int)pi.IDXR].SetPartId((int)pi.IDXR + ti.Max);
            //LoadCell   .PortOpen();
            //Dispr      .PortOpen();
            //HeightSnsr .PortOpen();
            //BarcordLODR.PortOpen();
            //BarcordBARZ.PortOpen();
            //Temp       .PortOpen();

            //ML.IO_SetY(yi.SSTG_HeaterOn,true); 


            //DateTime Time =  DateTime.Now ;
            //bool bConnect = SEQ.Oracle.OpenDB(OM.CmnOptn.sOracleIP , OM.CmnOptn.sOraclePort , OM.CmnOptn.sOracleID , OM.CmnOptn.sOraclePassword);//"192.168.1.77" , "1521" , "hr","hr"
            //TimeSpan Span =  DateTime.Now - Time ;
            //double dVal = Span.TotalMilliseconds ;


            //if(!OM.CmnOptn.bOracleNotUse && !bConnect) Log.ShowMessage("Oracle","DB Connection Error");
            

            



        }

        public static void Close()
        {
            MainThread.Abort();
            MainThread.Join();
            
            ML.MT_SetServoAll(false);
            SM  .Close();
            OM  .Close();
            LOT .Close();
            SPC .Close();

            
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
                InspectZero();
                InspectFlowMeter();

                //Update ErrorProc.
                UpdateButton();

                //Check Button.
                UpdateSeqState();

                //Update Motor State (Input)
                MM.Update();

                //SPC
                SPC.Update(LOT.CrntLotData.sEmployeeID , LOT.CrntLotData.sLotNo , OM.GetCrntDev() , m_iSeqStat , OM.EqpStat.bMaint);

                //Vision Communication
                //Visn.Update();
                //bool bTest = false;
                //if (bTest)
                //{
                //    PM.SetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA, 45);

                //}
                //SEQ.BarcordPrnt.Update();
            }

        }



        //    CPartInterface * m_pPart[MAX_PART] ;

        //--------------------------------------------------------------------------------------------------------

        public static void Reset()
        {
            //Check running flag.
            if (m_bRun                               ) return;
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
            Log.CloseForm();
            
            m_iSeqStat = EN_SEQ_STAT.Stop;

            //System.
            //if(!EM_IsErr()) return; //20150801 선계원 홈잡을때 리셑 누르면 홈스텝이 날라가서 처박았음.
            //ML.MT_ResetAll();
            ML.MT_ResetAll();
            ML.MT_SetServoAll(true);
            //Visn.SendReset();
        }

        

         private static void UpdateButton  ()
         {
             //Check Inspect.
             if (!OM.CmnOptn.bIgnrDoor)  InspectDoor();
             if(m_iStep != EN_SEQ_STEP.Idle) InspectHomeDone () ;
             if(m_iStep == EN_SEQ_STEP.Idle) InspectLightGrid();
             //Local Var.
             bool isErr     = ML.ER_IsErr() ;
             bool isHomeEnd = ML.MT_GetHomeDoneAll();
         
             bool bStartSw   = ML.IO_GetXUp(xi.ETC_StartSw_L) || ML.IO_GetXUp(xi.ETC_StartSw_R) || m_bBtnStart;// || ML.IO_GetXUp(xi.ETC_StartSwR) || m_bBtnStart ; 
             bool bStopSw    = ML.IO_GetXUp(xi.ETC_StopSw_L ) || ML.IO_GetXUp(xi.ETC_StopSw_R ) || m_bBtnStop ;// || ML.IO_GetXUp(xi.ETC_StopSwR ) || m_bBtnStop  ; 
             bool bResetSw   = ML.IO_GetXUp(xi.ETC_ResetSw_L) || ML.IO_GetXUp(xi.ETC_ResetSw_R) || m_bBtnReset;// || ML.IO_GetXUp(xi.ETC_ResetSwR) || m_bBtnReset ; 
             bool bAirSw     = false; //m_bBtnAir ;
             bool bInitSw    = false; //ML.IO_GetXUp(xi.ETC_InitSw ) ;

             //bool bStartSw1  = ML.IO_GetXUp(xi.ETC_StartSw_R) || m_bBtnStart1;// || ML.IO_GetXUp(xi.ETC_StartSwR) || m_bBtnStart ; 
             //bool bStopSw1   = ML.IO_GetXUp(xi.ETC_StopSw_R ) || m_bBtnStop1 ;// || ML.IO_GetXUp(xi.ETC_StopSwR ) || m_bBtnStop  ; 
             //bool bResetSw1  = ML.IO_GetXUp(xi.ETC_ResetSw_R) || m_bBtnReset1;// || ML.IO_GetXUp(xi.ETC_ResetSwR) || m_bBtnReset ; 
         
             bool bSt1       = ML.IO_GetXUp(xi.ETC_StartSw_L) ;
             bool bSt2       = ML.IO_GetXUp(xi.ETC_StartSw_R) ;

             if(m_iSeqStat == EN_SEQ_STAT.Running) {
                 if(bSt1) {LEFT.Stat.bReqStop = false; OM.CmnOptn.bUse_L_Part = true ; }
                 if(bSt2 && RIGH.GetSeqStep() == 0) { RIGH.Stat.bReqStop = false; RIGH.bToStart = true; OM.CmnOptn.bUse_R_Part = true ; }
                 if(bSt2 && RIGH.GetSeqStep() != 0) { RIGH.Stat.bReqStop = false; OM.CmnOptn.bUse_R_Part = true ; }

             }
             else
             {
                 if(bSt1) {LEFT.Stat.bReqStop = false; OM.CmnOptn.bUse_L_Part = true ; OM.CmnOptn.bUse_R_Part = false ;}
                 if(bSt2) {RIGH.Stat.bReqStop = false; OM.CmnOptn.bUse_R_Part = true ; OM.CmnOptn.bUse_L_Part = false ;}
             }

             if(m_bBtnStart)
             {
                 Log.Trace("m_bBtnStart","true");
             }         
             if(bStartSw)
             {
                 Log.Trace("bStartSw","Started");
         
                 if(m_iSeqStat == EN_SEQ_STAT.Init   ) {Log.Trace("SeqStat","ssInit   ");}
                 if(m_iSeqStat == EN_SEQ_STAT.WorkEnd) {Log.Trace("SeqStat","ssWorkEnd");}
                 if(m_iSeqStat == EN_SEQ_STAT.Error  ) {Log.Trace("SeqStat","ssError  ");}
                 if(m_iSeqStat == EN_SEQ_STAT.Running) {Log.Trace("SeqStat","ssRunning");}
                 if(m_iSeqStat == EN_SEQ_STAT.Stop   ) {Log.Trace("SeqStat","ssStop   ");}
                 if(m_iSeqStat == EN_SEQ_STAT.RunWarn) {Log.Trace("SeqStat","ssRunWarn");}
         
                 if(MM.GetManNo() != (int)mc.NoneCycle) {
                     Log.Trace("ManCycle",string.Format(MM.GetManNo().ToString()));
                     bStartSw = false;
                 }
             }
         
         
             //Update Switch's Lamp
             bool bStopBtnFlick = (m_iStep == EN_SEQ_STEP.ToStopCon || m_iStep == EN_SEQ_STEP.ToStop) && m_bFlick ;
             
             
             //버튼 클릭시나
             ML.IO_SetY(yi.ETC_StartLp_L  , ML.IO_GetX(xi.ETC_StartSw_L ) ||  m_bRun                             );
             ML.IO_SetY(yi.ETC_StopLp_L   , ML.IO_GetX(xi.ETC_StopSw_L  ) || !m_bRun || bStopBtnFlick            );
             ML.IO_SetY(yi.ETC_ResetLp_L  , ML.IO_GetX(xi.ETC_ResetSw_L ) || (m_bFlick && isErr)                 );

             ML.IO_SetY(yi.ETC_StartLp_R  , ML.IO_GetX(xi.ETC_StartSw_R ) || RIGH.GetSeqStep() != 0                             );
             ML.IO_SetY(yi.ETC_StopLp_R   , ML.IO_GetX(xi.ETC_StopSw_R  ) || RIGH.GetSeqStep() == 0 || bStopBtnFlick            );
             ML.IO_SetY(yi.ETC_ResetLp_R  , ML.IO_GetX(xi.ETC_ResetSw_R ) || (m_bFlick && isErr)                 );

             //ML.IO_SetY(yi.ETC_AirLp     , ML.IO_GetX(xi.ETC_AirSw    ) || ML.IO_GetY(yi.ETC_MainAirSol     ));
             //ML.IO_SetY(yi.ETC_InitLp   , ML.IO_GetX(xi.ETC_InitSw  )                                        );
             
             //Center Man Button
             m_bBtnStart = false ;
             m_bBtnStop  = false ;
             m_bBtnReset = false ;
             m_bBtnAir   = false ;

             //Init. Button Flags.
             if (bStartSw)
             {
                 if(!isHomeEnd            ) { Log.ShowMessage("Error" , "NEED TO MOTOR HOMING"  ); bStartSw = false ; }
                 if(!LOT.GetLotOpen     ()) {
                    //LOT.TLot Lot;
                    //string LotNo  = DateTime.Now.ToString("hhmmss");
                    //string Device = OM.GetCrntDev();

                    //LOT.TLot Lot;

                    //Lot.sMaterialNo = tbMaterialNo.Text.Trim();
                    //Lot.sEmployeeID = tbEmployeeID.Text.Trim();
                    string sLotNo      = DateTime.Now.ToString("hhmmss");

                    LOT.LotOpen(sLotNo);

                    //OM.EqpStat.iLDRSplyCnt = 0;
                    //Log.ShowMessage("Error" , "NEED TO LOT OPEN");
                    //bStartSw = false ;
                 }
                 if(!InspectStripDispr  ()) { m_bInspDispr = true ; bStartSw = false ; }
                 if(!InspectStripUnknown()) { m_bInspUnkwn = true ; bStartSw = false ; }

                 if(OM.DevInfo.iR_Mode == (int)Mode.Height && OM.DevInfo.iR_H_Manual == 1)
                 {
                     MM.SetManCycle(mc.RIGH_Once);
                     bStartSw = false ; 
                 }
                 if(OM.DevInfo.iR_Mode == (int)Mode.Weight && OM.DevInfo.iR_W_Manual == 1)
                 {
                     MM.SetManCycle(mc.RIGH_Once);
                     bStartSw = false ; 
                 }

                 if (m_iSeqStat == EN_SEQ_STAT.WorkEnd || m_iSeqStat == EN_SEQ_STAT.RunWarn) Reset();
             }

             if (bInitSw)
             {
                 MM.SetManCycle(mc.AllHome);
             }
             
             //Air Switch.
             if(bAirSw && !m_bRun && m_iSeqStat != EN_SEQ_STAT.Init)
             {
                 //ML.IO_SetY((int)yi.ETC_MainAirSol , !ML.IO_GetY((int)yi.ETC_MainAirSol )) ;
             }
             
             //Buzzer Off.
             if (isErr && bStopSw) ML.TL_SetBuzzOff(true);
             
             //Set Condition Flags.
             if( bStartSw)   //스타트버튼 안눌리는것 때문에 테스트.
             { 
                 Log.Trace("isErr" , isErr ? "true":"false");
                 Log.Trace("ManualMan.GetManNo()", string.Format(MM.GetManNo().ToString()));
             }

             bool bStop1       = ML.IO_GetXUp(xi.ETC_StopSw_L) ;
             bool bStop2       = ML.IO_GetXUp(xi.ETC_StopSw_R) ;
             if (bStopSw)
             {
                 //if (bStop1 && OM.CmnOptn.bUse_L_Part) {OM.CmnOptn.bUse_L_Part = false ; LEFT.bToStart = true; LEFT.Stat.bReqStop = true;}
                 if (bStop2 && OM.CmnOptn.bUse_R_Part) {OM.CmnOptn.bUse_R_Part = false ; RIGH.bToStart = true; RIGH.Stat.bReqStop = true; bStopSw = false;}
                 //if(bStop1 || bStop2) bStopSw = false;
                 Log.Trace("Stop", "Stop");
             }
             //if (!bStopSw && LEFT.Stat.bReqStop && RIGH.Stat.bReqStop) { bStopSw = true;}
             
             bool isStopCon  = bStopSw  || (isErr  && !m_bReqStop &&  m_bRun) ;
             bool isRunCon   = bStartSw && !isErr  /*&& ManualMan.GetManNo() == mcNone*/ ;
             bool isResetCon = bResetSw && !m_bRun ;
             
             //Run.
             if (isRunCon && (m_iStep == EN_SEQ_STEP.Idle)) 
             {
                 Eqp.bDeviceCng1 = true;
                 m_iStep = EN_SEQ_STEP.ToStartCon ;
                 ML.TL_SetBuzzOff(false);
                 ML.ER_SetDisp(true );
                 
             }
             if( isRunCon && (m_iStep == EN_SEQ_STEP.Idle)) //스타트버튼 안눌리는것 때문에 테스트.
             { 
                 Log.Trace("isRunCon && m_iStep" , string.Format(m_iStep.ToString()));
             }
             //if (bStopSw && (m_iSeqStat != EN_SEQ_STAT.Stop)) 
             //{
             //    LEFT.Stat.bReqStop = true;
             //    RIGH.Stat.bReqStop = true;
             //}
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
                                                         LOT.LotEnd();
                                                         Log.ShowMessage("CONFIRM", "Lot End.");
                                                         
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
                                                         LOT.LotEnd();
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

            //LOL if (!isOk && m_iSeqStat != EN_SEQ_STAT.ssError) SML.ER.SetErrMsg((int)ei.ETC_MainAir, "Cheked Main Air");
             
            return isOk ;
        }

        public static bool  InspectZero()
        {
            bool isOk = true ;


            if(m_tmTemp1.OnDelay(ML.IO_GetY(yi.ETC_LoadZero1),1000)) {m_tmTemp1.Clear(); ML.IO_SetY(yi.ETC_LoadZero1,false);}
            if(m_tmTemp2.OnDelay(ML.IO_GetY(yi.ETC_LoadZero2),1000)) {m_tmTemp2.Clear(); ML.IO_SetY(yi.ETC_LoadZero2,false);}
            if(m_tmTemp3.OnDelay(ML.IO_GetY(yi.ETC_LoadZero3),1000)) {m_tmTemp3.Clear(); ML.IO_SetY(yi.ETC_LoadZero3,false);}
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
            if (ML.IO_GetX(xi.ETC_EmgSw))
            {
                ML.MT_EmgStopAll();
                ML.MT_SetServoAll(false);
                if(ML.IO_GetX(xi.ETC_EmgSw)) ML.ER_SetErr(ei.ETC_Emergency,"Emergency Switch is Pushed.");

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

            if (!OM.CmnOptn.bIgnrDoor && m_iStep == EN_SEQ_STEP.Run)
            {
                if (!ML.IO_GetX(xi.ETC_DoorSw_F)) { ML.ER_SetErr(ei.ETC_DoorF, "Front Door Open"); isOk = false; }
                if (!ML.IO_GetX(xi.ETC_DoorSw_R)) { ML.ER_SetErr(ei.ETC_DoorR, "Rear  Door Open"); isOk = false; }
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
                                                
        public static bool  InspectActuator()
        {
            //Local Var.
            bool isOk  = true ;
            bool isErr = false;
            
            //Inspect.
            //for(ci i = 0 ; i < ci.MAX_ACTR ; i++) 
            //{
            //    isErr = ML.CL_Err(i) ;
            //    if (isErr) { ML.ER_SetErr(ei.ATR_TimeOut, ML.CL_GetName(i)); isOk = false; }
            //}
            
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

        public static double AIO_GetX(ax x,bool bDig = false)
        {
            double d1 = 1;
            double d2 = 0;
            if (x == ax.ETC_LoadCell1) {d1 = 10; d2 = -0.16  - OM.CmnOptn.dLoadOfs1 ;}//-0.12 ;}
            if (x == ax.ETC_LoadCell2) {d1 = 10; d2 =  0.0   - OM.CmnOptn.dLoadOfs2 ;}// 0.001;}
            if (x == ax.ETC_LoadCell3) {d1 = 10; d2 = -0.046 - OM.CmnOptn.dLoadOfs3 ;}//-0.03 ;}
            double dTemp = ML.AIO_GetX(x, bDig);
            dTemp = dTemp * d1 + d2;
            return dTemp;
            //return ML.AIO_GetX(x, bDig);
        }
        public static bool InspectCrash()
        {
            bool bCrashed = false;

            double Rst1 = AIO_GetX(ax.ETC_LoadCell1);//ML.AIO_GetX(ax.ETC_LoadCell1) * 10;
            double Rst2 = AIO_GetX(ax.ETC_LoadCell2);
            double Rst3 = AIO_GetX(ax.ETC_LoadCell3);

            //double dOver = 0;
            if (Rst1 > 35.0 || Rst1 < -35.0) { ML.MT_EmgStopAll(); ML.ER_SetErr(ei.ETC_LoadCellOver1); }
            if (Rst2 > 35.0 || Rst2 < -35.0) { ML.MT_EmgStopAll(); ML.ER_SetErr(ei.ETC_LoadCellOver2); }
            if (Rst3 > 35.0 || Rst3 < -35.0) { ML.MT_EmgStopAll(); ML.ER_SetErr(ei.ETC_LoadCellOver3); }


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
                                                 
        public static bool  InspectStripUnknown()
        {
        //    if (DM.ARAY[riPR2].CheckAllStat(csNone) && DM.ARAY[riPR1].CheckAllStat(csNone) && IO_GetX(xPRB_3Pkg) ) return false ;
            return true ;
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
            for(int i=0; i < (int)pi.MAX_PART; i++){
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

        public static void CheckValue(ref int _tbControl, int _iMin, int _iMax)
        {
            int iVal = _tbControl;// CConfig.StrToIntDef(_tbControl.Text, 0);
            if (iVal < _iMin) iVal = _iMin;
            if (iVal > _iMax) iVal = _iMax;
            _tbControl = iVal;
        }

        public static void CheckValue(ref double _tbControl, double _dMin, double _dMax)
        {
            double dVal = _tbControl;// CConfig.StrToDoubleDef(_tbControl.Text, 0);
            if (dVal < _dMin) dVal = _dMin;
            if (dVal > _dMax) dVal = _dMax;
            _tbControl = dVal;
        }

        public static void CheckValueMax(ref double _tbControl, double _dMin, double _dMax)
        {
            double dVal = _tbControl;// CConfig.StrToDoubleDef(_tbControl.Text, 0);
            if (dVal < _dMin) dVal = _dMax;
            if (dVal > _dMax) dVal = _dMax;
            _tbControl = dVal;
        }
    }
}
