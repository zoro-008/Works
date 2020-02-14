using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using COMMON;
using SML2;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection;
using DWORD = System.UInt32;
using System.Diagnostics;

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

        static public Index    IDX = new Index   ();

        static public Part[] m_Part = new Part[(int)pi.MAX_PART];

        static CDelayTimer m_tmToStop   ;
        static CDelayTimer m_tmToStrt   ;
        static CDelayTimer m_tmFlickOn  ;
        static CDelayTimer m_tmFlickOff ;
        static CDelayTimer m_tmCloseDoor;
        static CDelayTimer m_tmTemp     ;       

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

        static public EN_SEQ_STEP m_iStep; //Sequence Step.
        static public EN_SEQ_STAT m_iSeqStat; //Current Sequence Status.


        static public bool m_bTempTrigger;
        static public bool m_bTriggerOne;

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

        static public siCom[] Com = new siCom[(int)si.MAX_RS232];


        public static void Init()
        {
            SML.TPara Para;
            
            Para.sParaFolderPath = Directory.GetCurrentDirectory() + "\\Util\\";
            Para.iWidth          = 1280 ;
            Para.iHeight         = 863  ;
            Para.bTabHides       = new bool[6];
            Para.bUseErrPic      = true ;
            Para.iCntErr         = 60;
            Para.iCntDIn         = 32;
            Para.iCntDOut        = 32;
            Para.iCntCylinder    = 9;
            Para.iCntMotr        = 3;
            Para.eLanSel         = EN_LAN_SEL.English;
            Para.eDio            = EN_DIO_SEL.AXL;
            Para.eMotors         = new EN_MOTR_SEL[Para.iCntMotr];

            Para.eMotors[0] = EN_MOTR_SEL.AXL ;
            Para.eMotors[1] = EN_MOTR_SEL.AXL ;
            Para.eMotors[2] = EN_MOTR_SEL.AXL ;
            
            SML.Init(Para);
            DM .Init();
            OM .Init();
            LOT.Init();
            SPC.Init();
            PM.Init(PM.PstnCnt);
            
            MainThread.Priority = ThreadPriority.Highest;
            //MainThread.Priority = ThreadPriority.Normal;
            MainThread.Start();

            m_tmToStop      = new CDelayTimer ();
            m_tmToStrt      = new CDelayTimer ();
            m_tmFlickOn     = new CDelayTimer ();
            m_tmFlickOff    = new CDelayTimer ();
            m_tmCloseDoor   = new CDelayTimer ();
            m_tmTemp        = new CDelayTimer ();

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
            m_bRunEnd = false;

            m_Part[(int)pi.IDX] = IDX;
        }
        public static void Close()
        {
            MainThread.Abort();
            MainThread.Join();
            
            SML  .Close();
            OM  .Close();
            LOT .Close();
            SPC .Close();
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
				SML.Update(m_iSeqStat);

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
                //InspectCrash();

                //Update ErrorProc.
                UpdateButton();

                //Check Button.
                UpdateSeqState();

                //Update Motor State (Input)
                MM.Update();

                //SPC
                SPC.Update(m_iSeqStat);
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
            SM.ER_Clear();
            
            //Manual.
            MM.Reset();
            
            //Part.
            for(int i=0; i < (int)pi.MAX_PART; i++){
                m_Part[i].Reset();
            }
            
            //Lot End Flag Reset.
            LOT.Reset();
            //FM_CloseMsgOk();
            
            m_iSeqStat = EN_SEQ_STAT.Stop;

            //System.
            //if(!EM_IsErr()) return; //20150801 선계원 홈잡을때 리셑 누르면 홈스텝이 날라가서 처박았음.
            //SM.MT.ResetAll();
            SM.MT_ResetAll();
            SM.MT_SetServoAll(true);
        }

        

         private static void UpdateButton  ()
         {
             //if (!OM.CmnOptn.bIgnrDoor) InspectDoor();

             if(m_iStep != EN_SEQ_STEP.Idle) 
             {
                 InspectHomeDone () ;
             }
         
             //Local Var.
             bool isErr     = SML.ER.IsErr() ;
             bool isHomeEnd = SM.MT_GetHomeDoneAll();
         
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
         
             bool bStartSw   = SM.IO_GetXUp(xi.ETC_StartSw) || m_bBtnStart ; 
             bool bStopSw    = SM.IO_GetXUp(xi.ETC_StopSw ) || m_bBtnStop  ; 
             bool bResetSw   = SM.IO_GetXUp(xi.ETC_ResetSw) || m_bBtnReset ; 
             bool bAirSw     = SM.IO_GetXUp(xi.ETC_AirSw  ) || m_bBtnAir   ;
             //bool bInitSw    = SM.IO_GetXUp(xi.ETC_LInitSw ) || SM.IO_GetXUp(xi.ETC_RInitSw )                ;
         
             if(SM.IO_GetXUp(xi.ETC_StartSw))
             {
                 Log.Trace("IO_GetXUp(xETC_StartSw)","true");
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
         
                 if(MM.GetManNo() != (int)mc.NoneCycle) {
                     Log.Trace("ManCycle",string.Format(MM.GetManNo().ToString()));
                     bStartSw = false;
                 }
             }
         
             //Update Switch's Lamp
             bool bStopBtnFlick = (m_iStep == EN_SEQ_STEP.ToStopCon || m_iStep == EN_SEQ_STEP.ToStop) && m_bFlick ;
             
             
             //버튼 클릭시나
             SM.IO_SetY(yi.ETC_StartLp  , SM.IO_GetX(xi.ETC_StartSw ) ||  m_bRun                                      );
             SM.IO_SetY(yi.ETC_StopLp   , SM.IO_GetX(xi.ETC_StopSw  ) || !m_bRun || bStopBtnFlick                     );
             SM.IO_SetY(yi.ETC_ResetLp  , SM.IO_GetX(xi.ETC_ResetSw ) || (m_bFlick && isErr)                          );
             //SM.IO.SetY((int)EN_OUTPUT_ID.yETC_LAirLp    , SM.IO.GetX((int)EN_INPUT_ID.xETC_LAirSw   ) || SM.IO.GetY((int)EN_OUTPUT_ID.yETC_MainAirSol));
             //SM.IO.SetY((int)EN_OUTPUT_ID.yETC_LInitLp   , SM.IO.GetX((int)EN_INPUT_ID.xETC_LInitSw  )                                                 );
             
             //SM.IO.SetY((int)EN_OUTPUT_ID.yETC_RStartLp  , SM.IO.GetX((int)EN_INPUT_ID.xETC_RStartSw ) ||  m_bRun                                      );
             //SM.IO.SetY((int)EN_OUTPUT_ID.yETC_RStopLp   , SM.IO.GetX((int)EN_INPUT_ID.xETC_RStopSw  ) || !m_bRun || bStopBtnFlick                     );
             //SM.IO.SetY((int)EN_OUTPUT_ID.yETC_RResetLp  , SM.IO.GetX((int)EN_INPUT_ID.xETC_RResetSw ) || (m_bFlick && isErr)                          );
             //SM.IO.SetY((int)EN_OUTPUT_ID.yETC_RAirLp    , SM.IO.GetX((int)EN_INPUT_ID.xETC_RAirSw   ) || SM.IO.GetY((int)EN_OUTPUT_ID.yETC_MainAirSol));
             //SM.IO.SetY((int)EN_OUTPUT_ID.yETC_RInitLp   , SM.IO.GetX((int)EN_INPUT_ID.xETC_RInitSw  )                                                 );
             
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
                 if(!LOT.GetLotOpen     ()) { Log.ShowMessage("Error" , "장비 랏오픈을 해주세요."); bStartSw = false ; }
                 if(!InspectStripDispr  ()) { m_bInspDispr = true ; bStartSw = false ; }
                 if(!InspectStripUnknown()) { m_bInspUnkwn = true ; bStartSw = false ; }

                 if (m_iSeqStat == EN_SEQ_STAT.WorkEnd || m_iSeqStat == EN_SEQ_STAT.RunWarn) Reset();
             }

             //if (bInitSw)
             //{
             //    MM.SetManCycle(mc.AllHome);
             //}
             
             //Air Switch.
             if(bAirSw && !m_bRun && m_iSeqStat != EN_SEQ_STAT.Init)
             {
                 SM.IO_SetY(yi.ETC_MainAirSol, !SM.IO_GetY(yi.ETC_MainAirSol));
             }
             
             //Buzzer Off.
             if (isErr && bStopSw) SML.TL.SetBuzzOff(true);
             
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
                 SML.TL.SetBuzzOff(false); 
                 SML.ER.SetDisp(true );
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
             
             if (isResetCon && (m_iStep == EN_SEQ_STEP.Idle)) Reset()           ;
             
             if (m_tmToStrt.OnDelay(m_iStep == EN_SEQ_STEP.ToStartCon || m_iStep == EN_SEQ_STEP.ToStart , 30000))
             {
                 //Trace Log.
                 string Msg ;
                 Msg = string.Format("ToStrtTimeOut : m_iStep=%d" ,m_iStep );
                 Log.Trace  ("SEQ",Msg);
                 SML.ER.SetErr ((int)ei.ETC_ToStartTO);
                 m_iStep = EN_SEQ_STEP.Idle;
                 m_bRun  = false;
             }
             
             //CDelayTimer StopBtn = null;
             //StopBtn = new CDelayTimer();
             //if(m_iStep == EN_SEQ_STEP.scToStopCon)
             //{
             //    if(StopBtn.OnDelay(SM.IO.GetX((int)IP.xETC_StopSw)||SM.IO.GetX((int)IP.xETC_StopSw) , 5000))
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
             
                 //ToStop();
                 SML.ER.SetErr((int)ei.ETC_ToStopTO);
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
                                                       Log.Trace("SEQ", "scRun LotEnd");
                                                       m_bRunEnd = true;
                                                       m_iStep = EN_SEQ_STEP.ToStopCon;
                                                       ////랏엔드 상황.
                                                       //LOT.LotEnd();
                                                       //Log.Trace("SEQ","scRun LotEnd");
                                                       //m_iStep = EN_SEQ_STEP.ToStopCon ;
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

            //if(!SM.IO_GetY(yi.ETC_MainAirSol)) isOk = false ;
            if (!SM.IO_GetX(xi.ETC_MainAir)) isOk = false;

            //LOL if (!isOk && m_iSeqStat != EN_SEQ_STAT.ssError) SML.ER.SetErrMsg((int)ei.ETC_MainAir, "Cheked Main Air");
             
            return isOk ;
        }
        public static bool  InspectTemp()
        {
            bool isOk = true ;

            //if(IO_GetX(xPSB_TempAlrm)){EM_SetErr(eiPRB_TempZ1Alrm); isOk = false ;}
            
            return isOk ;
        }
        
        public static bool  InspectEmergency()
        {
            bool isOk = true ;
        
            //Check Emergency
            if (SM.IO_GetX(xi.ETC_EmgSw))
            {
                SM.MT_EmgStopAll();
                SM.ER_SetErr(ei.ETC_Emergency, "Emergency Switch가 눌렸습니다.");
                //SM.MT.SetServoAll(false);
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

            //if (!OM.CmnOptn.bIgnrDoor && m_iStep == EN_SEQ_STEP.Run)
            //{
            //    if (!SM.IO_GetX(xi.ETC_FtDoor)) { SM.ER_SetErr(ei.ETC_Door, "앞문 열림."); isOk = false; }
            //    if (!SM.IO_GetX(xi.ETC_RrDoor)) { SM.ER_SetErr(ei.ETC_Door, "앞문 열림."); isOk = false; }
            //}

            //Ok.
            return isOk;
        }
                                                
        public static bool  InspectActuator()
        {
            //Local Var.
            bool isOk = true;
            bool isErr = false;

            //Inspect.
            //for (ci i = 0; i < ci.MAX_ACTR; i++)
            //{
            //    isErr = SM.CL_Err(i);
            //    if (isErr) { SM.ER_SetErr(ei.ATR_TimeOut, SM.CL_GetName(i)); isOk = false; }
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
                if (SM.MT_GetAlarmSgnl(i)                  ){SM.ER_SetErr(ei.MTR_Alarm  , SM.MT_GetName(i)); isOk = false; }
                if (SM.MT_GetHomeDone(i))
                {
                    if (SM.MT_GetNLimSnsr(i)) { SML.ER.SetErrMsg((int)ei.MTR_NegLim, SM.MT_GetName(i)); isOk = false; }
                    if (SM.MT_GetPLimSnsr(i)) { SML.ER.SetErrMsg((int)ei.MTR_PosLim, SM.MT_GetName(i)); isOk = false; }
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
                if (!SM.MT_GetHomeDone(i)){SM.ER_SetErr(ei.MTR_HomeEnd , SM.MT_GetName(i)); isOk = false; }
            }
          
          //Ok.
            return isOk;
        }
                                                 
        //public static bool  InspectCrash()
        //{
        //    //LeftX
        //    double dLTL_XGenRrEnc = SM.MT.GetEncPos((int)EN_MOTR_ID.miSLD_XMotr);
        //    double dLTL_XGenRrTrg = SM.MT.GetTrgPos((int)EN_MOTR_ID.miSLD_XMotr);
        //    double dLTL_XGenRrPos = dLTL_XGenRrEnc < dLTL_XGenRrTrg ? dLTL_XGenRrTrg : dLTL_XGenRrEnc ;
        //    //RightX
        //    //double dRTL_XGenRrEnc = SM.MT.GetEncPos((int)EN_MOTR_ID.miRTL_XGenRr );
        //    //double dRTL_XGenRrTrg = SM.MT.GetTrgPos((int)EN_MOTR_ID.miRTL_XGenRr );
        //    //double dRTL_XGenRrPos = dRTL_XGenRrEnc < dRTL_XGenRrTrg ? dRTL_XGenRrTrg : dRTL_XGenRrEnc ;
        //    //
        //    ////LeftYVisn
        //    //double dLTL_YVisnEnc  = SM.MT.GetEncPos((int)EN_MOTR_ID.miLTL_YVisn  );
        //    //double dLTL_YVisnTrg  = SM.MT.GetTrgPos((int)EN_MOTR_ID.miLTL_YVisn  );
        //    //double dLTL_YVisnPos  = dLTL_YVisnEnc < dLTL_YVisnTrg ? dLTL_YVisnTrg : dLTL_YVisnEnc ;
        //    ////LeftYVisn
        //    //double dLTL_YDispEnc  = SM.MT.GetEncPos((int)EN_MOTR_ID.miLTL_YDispr );
        //    //double dLTL_YDispTrg  = SM.MT.GetTrgPos((int)EN_MOTR_ID.miLTL_YDispr );
        //    //double dLTL_YDispPos  = dLTL_YDispEnc < dLTL_YDispTrg ? dLTL_YDispTrg : dLTL_YDispEnc ;
        //    //
        //    ////RightYTranfer
        //    //double dRTL_YTrsprEnc = SM.MT.GetEncPos((int)EN_MOTR_ID.miRTL_YTrspr );
        //    //double dRTL_YTrsprTrg = SM.MT.GetTrgPos((int)EN_MOTR_ID.miRTL_YTrspr );
        //    //double dRTL_YTrsprPos = dRTL_YTrsprEnc < dRTL_YTrsprTrg ? dRTL_YTrsprTrg : dRTL_YTrsprEnc ;
        //    ////RightYVisn
        //    //double dRTL_YVisnEnc  = SM.MT.GetEncPos((int)EN_MOTR_ID.miRTL_YVisn  );
        //    //double dRTL_YVisnTrg  = SM.MT.GetTrgPos((int)EN_MOTR_ID.miRTL_YVisn  );
        //    //double dRTL_YVisnPos  = dRTL_YVisnEnc < dRTL_YVisnTrg ? dRTL_YVisnTrg : dRTL_YVisnEnc ;
        //    //
        //    ////Left Right
        //    //if(dLTL_XGenRrPos + dRTL_XGenRrPos > COptnMan.MstOptn.dLTL_RTL_Stroke)
        //    //{
        //    //    if(ManualMan.GetManNo() != EN_MANUAL_CYCLE.mcAllHome)
        //    //    { //하드웨어 충돌 알람시에 올홈으로 풀수 있다.
        //    //        if(SM.MT.GetServo((int)EN_MOTR_ID.miLTL_XGenRr) || SM.MT.GetServo((int)EN_MOTR_ID.miRTL_XGenRr))
        //    //        {
        //    //            SM.MT.SetServo((int)EN_MOTR_ID.miLTL_XGenRr,false);
        //    //            SM.MT.SetServo((int)EN_MOTR_ID.miRTL_XGenRr,false);
        //    //            SM.ER.SetErrMsg((int)EN_ERR_ID.eiPRT_Crash , "레프트툴 라이트툴이 충돌하려 합니다. 홈을 잡아주세요.");
        //    //            return false ;
        //    //        }
        //    //    }
        //    //}
        //    //
        //    ////Left Y
        //    //if(dLTL_YVisnPos + dLTL_YDispPos > COptnMan.MstOptn.dLTL_VISN_DISP_Strke)
        //    //{
        //    //    if(ManualMan.GetManNo() != EN_MANUAL_CYCLE.mcAllHome)
        //    //    { //하드웨어 충돌 알람시에 올홈으로 풀수 있다.
        //    //        if(SM.MT.GetServo((int)EN_MOTR_ID.miLTL_YDispr) || SM.MT.GetServo((int)EN_MOTR_ID.miRTL_YVisn))
        //    //        {
        //    //            SM.MT.SetServo((int)EN_MOTR_ID.miLTL_YDispr,false);
        //    //            SM.MT.SetServo((int)EN_MOTR_ID.miRTL_YVisn,false);
        //    //            SM.ER.SetErrMsg((int)EN_ERR_ID.eiPRT_Crash , "레프트 디스펜서 와 레프트 비젼이 충돌하려 합니다. 홈을 잡아주세요.");
        //    //            return false ;
        //    //        }
        //    //    }
        //    //}
        //    //
        //    ////Right Y
        //    //if(dRTL_YTrsprPos + dRTL_YVisnPos > COptnMan.MstOptn.dRTL_TRNS_VISN_Strke){
        //    //    if(ManualMan.GetManNo() != EN_MANUAL_CYCLE.mcAllHome){ //하드웨어 충돌 알람시에 올홈으로 풀수 있다.
        //    //        if(SM.MT.GetServo((int)EN_MOTR_ID.miRTL_YTrspr) || SM.MT.GetServo((int)EN_MOTR_ID.miRTL_YVisn)){
        //    //            SM.MT.SetServo((int)EN_MOTR_ID.miRTL_YTrspr,false);
        //    //            SM.MT.SetServo((int)EN_MOTR_ID.miRTL_YVisn ,false);
        //    //            SM.ER.SetErrMsg((int)EN_ERR_ID.eiPRT_Crash , "라이트 트렌스퍼 와 레프트 비젼이 충돌하려 합니다. 홈을 잡아주세요.");
        //    //            return false ;
        //    //        }
        //    //    }
        //    //}
        //  
        //    return true ;
        //}
                                                 
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
            //Call ToStop.
            
            
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
                if(!bRet[i]) return false ;
            }
            
            return true ;
        }

        public static void UpdateSeqState() 
        {
            bool isInit    =  MM.GetManNo() == mc.AllHome ;
            bool isError   =  SML.ER.IsErr()              ;                 
            bool isRunning =  m_bRun                      ;
            bool isRunWarn =  m_bRunEnd                   ;     
            bool isStop    = !m_bRun && !m_bRunEnd        ;                
            bool isLotEnd  =  LOT.GetLotEnd()             ;
            
            //Flicking Timer.
            if (m_bFlick) { m_tmFlickOn .Clear(); if (m_tmFlickOff.OnDelay( m_bFlick , 500)) m_bFlick = false; }
            else          { m_tmFlickOff.Clear(); if (m_tmFlickOn .OnDelay(!m_bFlick , 500)) m_bFlick = true ; }
            
            //Set Sequence State. Tower Lamp
                 if (isInit   ) { m_iSeqStat = EN_SEQ_STAT.Init       ;}
            else if (isLotEnd ) { m_iSeqStat = EN_SEQ_STAT.WorkEnd    ;}
            else if (isError  ) { m_iSeqStat = EN_SEQ_STAT.Error      ;}
            else if (isRunning) { m_iSeqStat = EN_SEQ_STAT.Running    ;}
            else if (isStop   ) { m_iSeqStat = EN_SEQ_STAT.Stop       ;}
            else if (isRunWarn) {
                                    m_iSeqStat = EN_SEQ_STAT.RunWarn;
                                    Log.ShowMessage("Warning", "Work End");
                                }
            else                { }
        }
    }
}
