using System;
using COMMON;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;

namespace Machine
{
    public class Righ : Part
    {
        public delegate void CamVoid  (); //델리게이트 선언
        public delegate void CamString(string _sPath); //델리게이트 선언
        public event CamVoid   CStart; //델리게이트 이벤트 선언
        public event CamVoid   CStop ; //델리게이트 이벤트 선언
        public event CamString CRec  ; //델리게이트 이벤트 선언
        public void Start() { CStart(); }
        public void Stop () { CStop(); CStart(); }
        public void Rec  (string _sPath = "") {
            string sPath = _sPath ;
            if(sPath == "") sPath = @OM.CmnOptn.sLeftFolder + @"\" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".avi";
            CRec(sPath);
        }
        
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop;
            public void Clear()
            {
                bWorkEnd  = false ;
                bReqStop  = false ;
            }
        };   

        public enum sc
        {
            Idle    = 0,
            Wait       ,
            Zero       ,
            WorkH      ,
            WorkW      ,
            WorkP      ,
            //WorkG      ,
            Once       ,
            MAX_SEQ_CYCLE
        };

        public struct SStep
        {
            public int iHome   ;
            public int iToStart;
            public sc  eSeq    ;
            public int iCycle  ;
            public int iToStop ;
            public sc  eLastSeq;
            public void Clear()
            {
                iHome    = 0;
                iToStart = 0;
                eSeq     = sc.Idle;
                iCycle   = 0;
                iToStop  = 0;
                eLastSeq = sc.Idle;
            }
        };

        protected string      m_sPartName;
        protected int         m_iPartId  ;
        //Timer.
        protected CDelayTimer m_tmMain   ;
        protected CDelayTimer m_tmCycle  ;
        protected CDelayTimer m_tmHome   ;
        protected CDelayTimer m_tmToStop ;
        protected CDelayTimer m_tmToStart;
        protected CDelayTimer m_tmDelay  ;
        protected CDelayTimer m_tmDelay1 ;        

        public    SStat Stat;
        protected SStep Step, PreStep;

        protected double m_dLastIdxPos;
        protected string m_sCheckSafeMsg;

        public CTimer[] m_CycleTime;

        public int    iWorkCnt  ;
        public double dKg       ;
        public double dZero     ;
        public double dZero1    ;
        public double dZero2    ;

        public bool   bUseUp    ;
        public bool   bUseDn    ;

        public bool   bToStart  ;
        public bool   bPull     ;
        public int    iMode     ;

        public int    iUsbCnt   ;
        public int    iCount1   ;
        //이거는 테스트 해보고 바꺼 줘야 할거.
        public double dMinLoad;
        public const int iTime0 = 300; //Zero
        public const int iTime1 = 300; //Weight
        public const int iTime2 = 30 ; //Fast 
        //public const double dMinLoad1 = 0.2;
        //public const double dMinLoad2 = 0.5;
        //public const double dMinInc1 = 0.01 ;
        //public const double dMinInc2 = 0.001;

        string sLotNo      ;//= DateTime.Now.ToString("hhmmss");

        public double GetLoadCell(int _iNo)
        {
            double Rst = 0.0;

                 if(_iNo == 1) Rst = SEQ.AIO_GetX(ax.ETC_LoadCell1) ;
            else if(_iNo == 2) Rst = SEQ.AIO_GetX(ax.ETC_LoadCell2) ;
            else if(_iNo == 3) Rst = SEQ.AIO_GetX(ax.ETC_LoadCell3) ;

            //Rst = Rst * 3;
            
            return Rst;
        }

        public Righ(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;
            
            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();
            m_tmDelay1    = new CDelayTimer();

            m_CycleTime   = new CTimer[(int)sc.MAX_SEQ_CYCLE];

            for(int i = 0 ; i < (int)sc.MAX_SEQ_CYCLE ; i++)
            {
                m_CycleTime [i]  = new CTimer();
            }

            m_iPartId = _iPartId;

            iUsbCnt   =0;
            iWorkCnt  =0;
            dKg       =0;
            dZero     =0;
            bToStart  =false;

            dZero1    =0;
            dZero2    =0;

            bUseUp    =false;
            bUseDn    =false;

            bPull     =false;
            iMode     =0;
            Reset();                  
        }
        protected double GetLastCmd(mi _iMotr)
        {
            double dLastIdxPos = 0.0;

            return dLastIdxPos;
        }
        protected void ResetTimer()
        {
            //Clear Timer.
            m_tmMain   .Clear();
            m_tmCycle  .Clear();
            m_tmHome   .Clear();
            m_tmToStop .Clear();
            m_tmToStart.Clear();
            m_tmDelay  .Clear();
        }

        public SStep GetStep() { return Step; }
        #endregion 

        //PartInterface 부분.
        //인터페이스 상속.====================================================================================================================
        #region Interface
        override public void Reset() //리셑 버튼 눌렀을때 타는 함수.
        {
            m_tmMain   .Clear();
            m_tmCycle  .Clear();
            m_tmHome   .Clear();
            m_tmToStop .Clear();
            m_tmToStart.Clear();
            m_tmDelay  .Clear();

            ResetTimer();

            Stat.Clear();
            Step.Clear();
            PreStep.Clear();
        }

        //Running Functions.
        override public void Update()
        {

        }
        override public bool ToStopCon() //스탑을 하기 위한 조건을 보는 함수.
        {
            Stat.bReqStop = true;
            //During the auto run, do not stop.
            if (Step.eSeq != sc.Idle) return false;

            Step.iToStop = 10;
            //Ok.
            return true;


        }
        override public bool ToStartCon() //스타트를 하기 위한 조건을 보는 함수.
        {
            Stat.bReqStop = false;
            Step.iToStart = 10;
            //Ok.
            return true;

        }
        override public bool ToStart() //스타트를 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 6000))
            {
                ER_SetErr(ei.ETC_ToStartTO, m_sPartName );
            }

            String sTemp = String.Format("Step.iToStart={0:00}", Step.iToStart);

            if (Step.iToStart != PreStep.iToStart)
            {
                Trace(sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: 
                    return true;

                case 10: 
                    bToStart = true;
                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    Step.iToStart++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    //MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    //MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    Step.iToStart++;
                    return false;

                case 12:
                    //if(!MT_GetStopPos(mi.L_UP_ZLift,pv.L_UP_ZLiftWait)) return false;
                    //if(!MT_GetStopPos(mi.L_DN_ZLift,pv.L_DN_ZLiftWait)) return false;

                    Step.iToStart = 0;
                    return true;


            }

        }
        override public bool ToStop() //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 10000)) ER_SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

            String sTemp;
            sTemp = string.Format("Step.iToStop={0:00}", Step.iToStop);
            if (Step.iToStop != PreStep.iToStop)
            {
                Trace(sTemp);
            }

            PreStep.iToStop = Step.iToStop;

            Stat.bReqStop = false;

            //Move Home.
            switch (Step.iToStop)
            {
                default: 
                    return true;

                case 10: 
                    MT_Stop(mi.R_UP_ZLift);
                    //Stop();
                    Step.iToStop++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    if (OM.DevInfo.iR_Mode == 0) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait );
                    if (OM.DevInfo.iR_Mode == 1) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait );
                    if (OM.DevInfo.iR_Mode == 2) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP);

                    //if (bPull) MoveMotr(mi.R_UP_ZLift, pv.R_UP_ZLiftWaitP);
                    //else       MT_GoAbsRun(mi.R_UP_ZLift, 0.0);
                    Step.iToStop++;
                    return false;

                case 12:
                    if (OM.DevInfo.iR_Mode == 0) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait )) return false; }
                    if (OM.DevInfo.iR_Mode == 1) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait )) return false; }
                    if (OM.DevInfo.iR_Mode == 2) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP)) return false; }

                    //if ( bPull && !MT_GetStopPos(mi.R_UP_ZLift, pv.R_UP_ZLiftWaitP)) return false;
                    //if (!bPull && !MT_GetStop(mi.R_UP_ZLift)) return false;
                    Step.iToStop = 0;
                    return true;
            }


        }

        override public int GetHomeStep   () { return      Step.iHome    ; } override public int GetPreHomeStep   () { return      PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return      Step.iToStart ; } override public int GetPreToStartStep() { return      PreStep.iToStart ; }
        override public int GetSeqStep    () { return (int)Step.eSeq     ; } override public int GetPreSeqStep    () { return (int)PreStep.eSeq     ; }
        override public int GetCycleStep  () { return      Step.iCycle   ; } override public int GetPreCycleStep  () { return      PreStep.iCycle   ; } override public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; }
        override public int GetToStopStep () { return      Step.iToStop  ; } override public int GetPreToStopStep () { return      PreStep.iToStop  ; }

        override public string GetCrntCycleName(         ) { return Step.eSeq.ToString();}
        override public String GetCycleName    (int _iSeq) { return ((sc)_iSeq).ToString(); }
        override public double GetCycleTime    (int _iSeq) { return m_CycleTime[_iSeq].Duration; }
        override public String GetPartName     (         ) { return m_sPartName; }

        override public int GetCycleMaxCnt() { return (int)sc.MAX_SEQ_CYCLE; }

        override public int  GetPartId(        ) { return m_iPartId; }
        override public void SetPartId(int _iId) { m_iPartId = _iId; }

        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;
            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;
                if (!OM.CmnOptn.bUse_R_Part) return true;
                dKg = 0;
                if(OM.DevInfo.iR_Mode == (int)Mode.Weight) dKg = OM.DevInfo.dR_W_Weight ;

                bool isCycleZero   =   OM.DevInfo.iR_Mode != (int)Mode.Pull_Dest && bToStart ;//(iWorkCnt == 0 || bToStart)                      ;//&& iWorkCnt != -1;
                bool isCycleWorkH  =   OM.DevInfo.iR_Mode == (int)Mode.Height    && iWorkCnt < OM.DevInfo.iR_H_Count ;//&& iWorkCnt != -1;
                bool isCycleWorkK  =   OM.DevInfo.iR_Mode == (int)Mode.Weight    && iWorkCnt < OM.DevInfo.iR_W_Count ;//&& iWorkCnt != -1;
                bool isCycleWorkP  =   OM.DevInfo.iR_Mode == (int)Mode.Pull_Dest && iWorkCnt < OM.DevInfo.iR_P_Count ;//&& iWorkCnt != -1;
                //bool isCycleWorkG  =   OM.DevInfo.iR_Mode == (int)Mode.GripH     && iWorkCnt < OM.DevInfo.iR_G_Count ;//&& iWorkCnt != -1;

                bool isCycleEnd    = !isCycleWorkH && !isCycleWorkK && !isCycleZero;
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleZero  ) { Step.eSeq = sc.Zero   ;  }
                else if (isCycleWorkH ) { Step.eSeq = sc.WorkH  ;  }
                else if (isCycleWorkK ) { Step.eSeq = sc.WorkW  ;  }
                else if (isCycleWorkP ) { Step.eSeq = sc.WorkP  ;  }
                //else if (isCycleWorkG ) { Step.eSeq = sc.WorkG  ;  }
                else if (isCycleEnd   ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle){
                    Trace(Step.eSeq.ToString() +" Start");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }

            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default         :                        Trace("default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle  ):                                                                                                        return false;
                case (sc.Zero  ): if (CycleZero (dKg                  )) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.WorkH ): if (CycleWorkH(OM.DevInfo.iR_H_Count)) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.WorkW ): if (CycleWorkW(OM.DevInfo.iR_W_Count)) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.WorkP ): if (CycleWorkP(OM.DevInfo.iR_P_Count)) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                //case (sc.WorkG ): if (CycleWorkG(OM.DevInfo.iR_G_Count)) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        

        //밑에 부터 작업.
        public bool FindChip(out int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            return false;

        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 6000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Trace(sTemp);
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Trace(sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if(Stat.bReqStop) {
                //return true ;
            }
            
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;
            
                case 10: 
                    MT_Stop(mi.R_UP_ZLift);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    MT_GoHome(mi.R_UP_ZLift);
                    Step.iHome++;
                    return false ;

                case 12:
                    if (!MT_GetHomeDone(mi.R_UP_ZLift)) return false;
                    if (OM.DevInfo.iR_Mode == 0) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait );
                    if (OM.DevInfo.iR_Mode == 1) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait );
                    if (OM.DevInfo.iR_Mode == 2) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP);
                    //if (OM.DevInfo.iR_Mode == 3) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG);

                    Step.iHome++;
                    return false;

                case 13:
                    if (OM.DevInfo.iR_Mode == 0) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait )) return false; }
                    if (OM.DevInfo.iR_Mode == 1) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait )) return false; }
                    if (OM.DevInfo.iR_Mode == 2) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP)) return false; }
                    //if (OM.DevInfo.iR_Mode == 3) { if(!MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG)) return false; }

                    Step.iHome = 0;
                    return true ;
            }
        }

        public bool CycleWait()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 6000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.RIGH_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            if(!CheckOver(3,Step.iCycle)) return true;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10: 
                    MT_Stop(mi.R_UP_ZLift);
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    if (OM.DevInfo.iR_Mode == 0) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait );
                    if (OM.DevInfo.iR_Mode == 1) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait );
                    if (OM.DevInfo.iR_Mode == 2) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP);
                    //if (OM.DevInfo.iR_Mode == 3) MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG);

                    Step.iCycle++;
                    return false;

                case 12:
                    if (OM.DevInfo.iR_Mode == 0) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait )) return false; }
                    if (OM.DevInfo.iR_Mode == 1) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait )) return false; }
                    if (OM.DevInfo.iR_Mode == 2) { if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP)) return false; }
                    //if (OM.DevInfo.iR_Mode == 3) { if(!MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG)) return false; }

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleZero(double dKg = 0)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.RIGH_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                MT_Stop(mi.R_UP_ZLift);
                return true ;
            }

            if(!CheckOver(3,Step.iCycle)) return true;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //if (OM.DevInfo.iR_Mode != (int)Mode.Weight)
                    //{
                    //    Step.iCycle = 0;
                    //    return true;
                    //}

                    MT_Stop(mi.R_UP_ZLift);
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait)) return false;
                    IO_SetY(yi.ETC_LoadZero3, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero3, false);
                    Step.iCycle = 15;
                    return false;

                case 15:
                    //IO_SetY(yi.ETC_LoadZero3, false);
                    MoveMotr(mi.R_UP_ZLift, pv.R_UP_ZLiftTchB);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftTchB)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!m_tmDelay.OnDelay(true, 1000)) return false;
                    if (SEQ.AIO_GetX(ax.ETC_LoadCell3) > 1.0)
                    {
                        ER_SetErr(ei.ETC_LoadCellOver3);
                        return true;
                    }

                    IO_SetY(yi.ETC_LoadZero3, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero3, false);
                    Step.iCycle++;
                    return false;

                case 19://Upside Motor Touch Zero Move Down
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    
                    //MT_GoIncRun(mi.R_UP_ZLift,dMinInc1);
                    Step.iCycle=20;
                    return false;

                case 20://Upside Motor Touch Zero Move Down
                    //if (dKg == 0)
                    //{
                    //    //dKg = 0.5;
                    //    Step.iCycle++;
                    //    return false;
                    //}
                    m_tmDelay.Clear();
                    Step.iCycle=21;
                    return false;

                case 21:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    if (!m_tmDelay1.OnDelay(true, OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (!m_tmDelay.OnDelay(MoveKg(ax.ETC_LoadCell3, dKg, true) == 1, iTime2)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 22:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    if (!m_tmDelay1.OnDelay(true,OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (!m_tmDelay.OnDelay(MoveKg(ax.ETC_LoadCell3, dKg) == 1, iTime0)) return false;
                    dZero = MT_GetEncPos(mi.R_UP_ZLift) - OM.DevInfo.dR_H_ZeroOfs1 ;
                    PM.SetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA, dZero);
                    Step.iCycle++;
                    return false;

                case 23:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    MoveMotr(mi.R_UP_ZLift, pv.R_UP_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 24:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;

                    bToStart = false;
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleWorkH(int _iCnt = 0)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.RIGH_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                MT_Stop(mi.R_UP_ZLift);
                return true ;
            }

            if(!CheckOver(3,Step.iCycle)) return true;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //iWorkCnt = 0;
                    MT_Stop(mi.R_UP_ZLift);
                    //Start();
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait)) return false;
                    //if (PM.GetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA) == 0)
                    //{
                    //    ER_SetErr(ei.RIGH_NeedZeroCheck);
                    //    return true;
                    //}
                    //MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftTchH);
                    Step.iCycle++;
                    return false;

                case 13:
                    //if(!CheckOver(3,Step.iCycle,3)) return true;
                    //if(!MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftTchH)) return false;
                    //if(SEQ.AIO_GetX(ax.ETC_LoadCell3) > 3) { ER_SetErr(ei.RIGH_NeedZeroCheck); return true; }
                    IO_SetY(yi.ETC_LoadZero3, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero3, false);
                    //Step.iCycle = 15;
                    //return false;
                    //IO_SetY(yi.ETC_LoadZero3, false);
                    //Rec();
                    //if (!OM.CmnOptn.bUse_R_Zero)
                    //{//메뉴얼로 지정한 값으로 내려 감
                        //dZero1 = PM.GetValue(mi.R_UP_ZLift,pv.R_UP_ZLiftTchH) + OM.DevInfo.dR_H_Height ;
                    //}
                    //else
                    //{//측정한 제로 값으로 내려감
                        dZero1 = PM.GetValue(mi.R_UP_ZLift,pv.R_UP_ZLiftTchA) + OM.DevInfo.dR_H_Height ;
                    //}
                    MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait);

                    Step.iCycle = 15;
                    return false;

                case 15:
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    MT_GoAbs(mi.R_UP_ZLift,dZero1,OM.DevInfo.dR_H_Vel,OM.DevInfo.dR_H_Acc,OM.DevInfo.dR_H_Dcc);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iR_H_Time)) return false;

                    if (OM.CmnOptn.bUse_R_Dark)
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    Step.iCycle = 30;
                    return false;


                case 20:
                    SEQ.MCR.CycleInit(1, (iWorkCnt + 1).ToString());
                    Step.iCycle++;
                    return false;

                case 21:
                    if (SEQ.MCR.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.MCR.GetErrCode());
                        return true;
                    }
                    if (!SEQ.MCR.Cycle(1)) return false;
                    Step.iCycle = 30;
                    return false;

                case 30:
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell3));

                    MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 31:
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 32: //대기시간
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iR_H_Wait)) return false;

                    iWorkCnt++;
                    SPC.LOT.Data.WorkCnt2++;
                    if (_iCnt == 0)
                    {
                        if (iWorkCnt < OM.DevInfo.iR_H_Count)
                        {
                            Step.iCycle = 15;
                            return false;
                        }
                    }
                    else
                    {
                        if (iWorkCnt < _iCnt)
                        {
                            Step.iCycle = 15;
                            return false;
                        }

                    }
                    //if(OM.DevInfo.iR_H_Manual == 0) PM.SetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA, 0);
                    //Stop();
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleWorkW(int _iCnt = 0)
        {
            String sTemp;
            //if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 6000))
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.RIGH_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                MT_Stop(mi.R_UP_ZLift);
                return true ;
            }

            if(!CheckOver(3,Step.iCycle)) return false;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //iWorkCnt = 0;
                    MT_Stop(mi.R_UP_ZLift);
                    //Start();
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWait)) return false;
                    if (PM.GetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA) == 0)
                    {
                        ER_SetErr(ei.RIGH_NeedZeroCheck);
                        return true;
                    }

                    //Rec();
                    //if (!OM.CmnOptn.bUse_R_Zero)
                    //{//메뉴얼로 지정한 값으로 내려 감
                        //dZero1 = PM.GetValue(mi.R_UP_ZLift,pv.R_UP_ZLiftTchW) ;//+ OM.DevInfo.dL_H_Height ;
                    //}
                    //else
                    //{//측정한 제로 값으로 내려감
                        dZero1 = PM.GetValue(mi.R_UP_ZLift,pv.R_UP_ZLiftTchA) ;//+ OM.DevInfo.dL_H_Height ;
                    //}
                    dKg = OM.DevInfo.dR_W_Weight;
                    Step.iCycle = 15;
                    return false;

                case 15: //뒤에서 사용
                    MT_GoAbs(mi.R_UP_ZLift,dZero1,OM.DevInfo.dR_W_Vel,OM.DevInfo.dR_W_Acc,OM.DevInfo.dR_W_Dcc);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    //추가
                    if (OM.CmnOptn.bUse_R_Cort)
                    {
                        iCount1 = 0;
                        Step.iCycle=20;
                        return false;
                    }
                    m_tmDelay.Clear();
                    Step.iCycle = 30;
                    return false;

                case 20: //뒤에서 사용
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay1.OnDelay(true,OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (!m_tmDelay.OnDelay(MoveKg(ax.ETC_LoadCell3, dKg) == 1, iTime1)) return false;
                    Step.iCycle = 30;
                    return false;

                case 30:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 31:
                    if (!m_tmDelay1.OnDelay(true,OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (OM.CmnOptn.bUse_R_Cort) { MoveKg(ax.ETC_LoadCell3, dKg); }
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iR_W_Time)) return false;
                    if (OM.CmnOptn.bUse_R_Dark)
                    {
                        Step.iCycle = 40;
                        return false;
                    }
                    Step.iCycle = 50;
                    return false;


                case 40:
                    SEQ.MCR.CycleInit(1, (iWorkCnt + 1).ToString());
                    Step.iCycle++;
                    return false;

                case 41:
                    if (SEQ.MCR.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.MCR.GetErrCode());
                        return true;
                    }
                    if (!SEQ.MCR.Cycle(1)) return false;
                    Step.iCycle = 50;
                    return false;

                case 50:
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell3));
                    Step.iCycle++;
                    return false;

                case 51:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 52:
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 53:
                    if (!m_tmDelay.OnDelay(true,OM.DevInfo.iR_W_Wait)) return false;

                    iWorkCnt++;
                    SPC.LOT.Data.WorkCnt2++;
                    if (_iCnt == 0)
                    {
                        if (iWorkCnt < OM.DevInfo.iR_W_Count)
                        {
                            Step.iCycle = 15;
                            return false;
                        }
                    }
                    else
                    {
                        if (iWorkCnt < _iCnt)
                        {
                            Step.iCycle = 15;
                            return false;
                        }
                    }
                    //Stop();
                    if(OM.DevInfo.iR_W_Manual == 0) PM.SetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA, 0);
                    
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleWorkP(int _iCnt = 0)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 6000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.RIGH_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                MT_Stop(mi.R_UP_ZLift);
                return true ;
            }

            if(!CheckOver(3,Step.iCycle)) return false;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //iWorkCnt = 0;
                    MT_Stop(mi.R_UP_ZLift);
                    //Start();
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP)) return false;
                    //IO_SetY(yi.ETC_LoadZero3, true);
                    //Rec();
                    dZero1 = PM.GetValue(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP) - OM.DevInfo.dR_P_Height ;

                    IO_SetY(yi.ETC_LoadZero3, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero3, false);
                    Step.iCycle = 15;
                    return false;

                case 15:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iR_P_Wait)) return false;
                    MT_GoAbs(mi.R_UP_ZLift,dZero1,OM.DevInfo.dR_P_Vel,OM.DevInfo.dR_P_Acc,OM.DevInfo.dR_P_Dcc);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iR_P_Time)) return false;
                    if (OM.CmnOptn.bUse_R_Dark)
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    Step.iCycle = 30;
                    return false;

                case 20:
                    SEQ.MCR.CycleInit(1, (iWorkCnt + 1).ToString());
                    Step.iCycle++;
                    return false;

                case 21:
                    if (SEQ.MCR.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.MCR.GetErrCode());
                        return true;
                    }
                    if (!SEQ.MCR.Cycle(1)) return false;
                    Step.iCycle = 30;
                    return false;

                case 30:
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell3));
                    iWorkCnt++;
                    SPC.LOT.Data.WorkCnt2++;

                    MT_GoAbs(mi.R_UP_ZLift, PM.GetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftWaitP), OM.DevInfo.dR_P_Vel, OM.DevInfo.dR_P_Acc, OM.DevInfo.dR_P_Dcc);
                    if (_iCnt == 0)
                    {
                        if (iWorkCnt < OM.DevInfo.iR_P_Count)
                        {
                            m_tmDelay.Clear();
                            Step.iCycle = 15;
                            return false;
                        }
                    }
                    else
                    {
                        if (iWorkCnt < _iCnt)
                        {
                            m_tmDelay.Clear();
                            Step.iCycle = 15;
                            return false;
                        }
                    }

                    Step.iCycle++;
                    return false;

                case 31:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    //Stop();
                    Step.iCycle = 0;
                    return true;

                //case 18:
                //    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                //    m_tmDelay.Clear();
                //    Step.iCycle++;
                //    return false;

                //case 19:
                //    if(!m_tmDelay.OnDelay(OM.DevInfo.iR_P_Time)) return false;
                //    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell3));
                //    iWorkCnt++;
                //    SPC.LOT.Data.WorkCnt2++;
                //    if (_iCnt == 0)
                //    {
                //        if (iWorkCnt < OM.DevInfo.iR_P_Count)
                //        {
                //            Step.iCycle = 15;
                //            return false;
                //        }
                //    }
                //    else
                //    {
                //        if (iWorkCnt < _iCnt)
                //        {
                //            Step.iCycle = 15;
                //            return false;
                //        }
                //    }                    
                //    //Stop();
                //    Step.iCycle = 0;
                //    return true;
            }
        }
        /*
        public bool CycleWorkG(int _iCnt = 0)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 6000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.RIGH_CycleTO, sTemp);
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                MT_Stop(mi.R_UP_ZLift);
                return true ;
            }

            if(!CheckOver(3,Step.iCycle)) return false;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //iWorkCnt = 0;
                    MT_Stop(mi.R_UP_ZLift);
                    //Start();
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    if(!MT_CmprPos(mi.R_UP_ZLift, PM.GetValue(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG)))
                    {
                        ER_SetErr(ei.RIGH_GripWaitCheck);
                        return true;
                    }
                    if(SEQ.AIO_GetX(ax.ETC_LoadCell3) > 3) { ER_SetErr(ei.RIGH_NeedZeroCheck); return true; }

                    dZero1 = PM.GetValue(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG) - OM.DevInfo.dR_G_Height ;
                    dZero2 = PM.GetValue(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG) + OM.DevInfo.dR_G_Height ;
                    //MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG);
                    Step.iCycle++;
                    return false;

                case 12:
                    //if(!MT_GetStopPos(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG)) return false;

                    IO_SetY(yi.ETC_LoadZero3, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero3, false);
                    Step.iCycle = 15;
                    return false;

                case 15: //위로 이동
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;

                    MT_GoAbs(mi.R_UP_ZLift,dZero1,OM.DevInfo.dR_G_Vel,OM.DevInfo.dR_G_Acc,OM.DevInfo.dR_G_Dcc);
                    Step.iCycle++;
                    return false;

                case 16: //위로 이동 체크
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17: //대기 시간
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iR_G_Time)) return false;
                    if (OM.CmnOptn.bUse_R_Dark)
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    Step.iCycle = 30;
                    return false;

                case 20: //매크로
                    SEQ.MCR.CycleInit(1, (iWorkCnt + 1).ToString() + "_1");
                    Step.iCycle++;
                    return false;

                case 21: //매크로
                    if (SEQ.MCR.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.MCR.GetErrCode());
                        return true;
                    }
                    if (!SEQ.MCR.Cycle(1)) return false;
                    Step.iCycle = 30;
                    return false;

                case 30: //저장
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell3),"_1");
                    Step.iCycle++;
                    return false;

                    //개귀찮 두번행!!! 밑으로 이동
                case 31: //밑으로 이동
                    MT_GoAbs(mi.R_UP_ZLift,dZero2,OM.DevInfo.dR_G_Vel,OM.DevInfo.dR_G_Acc,OM.DevInfo.dR_G_Dcc);
                    Step.iCycle++;
                    return false;

                case 32: //밑으로 이동
                    if(!MT_GetStop(mi.R_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 33: //유지시간
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iR_G_Time)) return false;
                    if (OM.CmnOptn.bUse_R_Dark)
                    {
                        Step.iCycle = 40;
                        return false;
                    }
                    Step.iCycle = 50;
                    return false;

                case 40:
                    SEQ.MCR.CycleInit(1, (iWorkCnt + 1).ToString() + "_2");
                    Step.iCycle++;
                    return false;

                case 41:
                    if (SEQ.MCR.GetErrCode() != "")
                    {
                        ER_SetErr(ei.ETC_MacroErr, SEQ.MCR.GetErrCode());
                        return true;
                    }
                    if (!SEQ.MCR.Cycle(1)) return false;
                    Step.iCycle = 50;
                    return false;

                case 50:
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell3),"_2");

                    Step.iCycle=60;
                    return false;

                case 60:

                    iWorkCnt++;
                    SPC.LOT.Data.WorkCnt2++;

                    
                    if (_iCnt == 0)
                    {
                        if (iWorkCnt < OM.DevInfo.iR_G_Count)
                        {
                            m_tmDelay.Clear();
                            Step.iCycle = 15;
                            return false;
                        }
                    }
                    else
                    {
                        if (iWorkCnt < _iCnt)
                        {
                            m_tmDelay.Clear();
                            Step.iCycle = 15;
                            return false;
                        }
                    }

                    MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG);
                    Step.iCycle++;
                    return false;

                case 61:
                    if (!MT_GetStop(mi.R_UP_ZLift)) return false;
                    //Stop();
                    Step.iCycle = 0;
                    return true;

            }
        }
        */
        public int MoveKg(ax _id, double dKg = 0 , bool bFast = false)
        {        
            double Load = 0;
            int iType = 0; //slow
            if (dKg == 0)
            {
                //dKg = 1.0;
                if(_id == ax.ETC_LoadCell1) dKg = OM.DevInfo.dL_H_Weight;//1.0;
                if(_id == ax.ETC_LoadCell2) dKg = OM.DevInfo.dL_H_Weight;//1.0;
                if(_id == ax.ETC_LoadCell3) dKg = OM.DevInfo.dR_H_Weight;//1.0;
                iType = 1;
            }

            if(OM.CmnOptn.dLS1 > 0.1) OM.CmnOptn.dLS1 = 0.1;
            if(OM.CmnOptn.dLS2 < 20 ) OM.CmnOptn.dLS2 = 20 ;
            if(OM.CmnOptn.dLS3 < 30 ) OM.CmnOptn.dLS3 = 30 ;
            if(OM.CmnOptn.dLS4 < 40 ) OM.CmnOptn.dLS4 = 40 ;
            if(OM.CmnOptn.dLS5 < 50 ) OM.CmnOptn.dLS5 = 50 ;

            if(OM.CmnOptn.dLS2 > 1000 ) OM.CmnOptn.dLS2 = 1000 ;
            if(OM.CmnOptn.dLS3 > 1000 ) OM.CmnOptn.dLS3 = 1000 ;
            if(OM.CmnOptn.dLS4 > 1000 ) OM.CmnOptn.dLS4 = 1000 ;
            if(OM.CmnOptn.dLS5 > 1000 ) OM.CmnOptn.dLS5 = 1000 ;

            double dInc   = 0;
            int    iClass = 0;
            double dLoadCell = SEQ.AIO_GetX(_id);
            double dAbs      = Math.Abs(dLoadCell - dKg);
            if (!bFast)
            { 
                     if (dAbs > 5.0 ) { dInc = 0.1;                 iClass = 4; }
                else if (dAbs > 3.0 ) { dInc = dAbs/OM.CmnOptn.dLS2;iClass = 3; }
                else if (dAbs > 2.0 ) { dInc = dAbs/OM.CmnOptn.dLS3;iClass = 3; }
                else if (dAbs > 1.0 ) { dInc = dAbs/OM.CmnOptn.dLS4;iClass = 3; }
                else if (dAbs > 0.1 ) { dInc = dAbs/OM.CmnOptn.dLS5;iClass = 2; }
                else if (dAbs > 0.09) { dInc = 0.0009;              iClass = 1; }
                else if (dAbs > 0.08) { dInc = 0.0008;              iClass = 1; }
                else if (dAbs > 0.07) { dInc = 0.0007;              iClass = 1; }
                else if (dAbs > 0.06) { dInc = 0.0006;              iClass = 1; }
                else if (dAbs > 0.05) { dInc = 0.0005;              iClass = 1; }
                else if (dAbs > 0.04) { dInc = 0.0004;              iClass = 1; }
                else if (dAbs > 0.03) { dInc = 0.0003;              iClass = 1; }
                else if (dAbs > 0.02) { dInc = 0.0002;              iClass = 1; }
                else if (dAbs > 0.01) { dInc = 0.0001;              iClass = 1; }
                else                  { dInc = 0.0001;              iClass = 1; }
            }
            else
            {
                     if (dAbs > 0.5 ) { dInc = OM.CmnOptn.dLF1 ;   iClass = 3; }
                else if (dAbs > 0.4 ) { dInc = OM.CmnOptn.dLF2 ;   iClass = 2; }
                else if (dAbs > 0.3 ) { dInc = OM.CmnOptn.dLF3 ;   iClass = 2; }
                else if (dAbs > 0.2 ) { dInc = OM.CmnOptn.dLF4 ;   iClass = 1; }
                else if (dAbs > 0.1 ) { dInc = OM.CmnOptn.dLF5 ;   iClass = 1; }

            }
            if(dInc < 0.0001) dInc = 0.0001 ;
            if(dInc > 0.1   ) dInc = 0.1    ;

            //Add 1.0.0.8
            if(OM.CmnOptn.dLoadRange < 0.1) OM.CmnOptn.dLoadRange = 0.1 ;
            if(OM.CmnOptn.dLoadRange > 0.9) OM.CmnOptn.dLoadRange = 0.9 ;

            if(dAbs >= OM.CmnOptn.dLoadRange) iClass = 1;

            if (_id == ax.ETC_LoadCell1)
            {
                if (!MT_GetStop(mi.L_UP_ZLift)) return iClass;
                if (MT_GetCmdPos(mi.L_UP_ZLift) >= PM.GetValueMax(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA))
                {
                    ER_SetErr(ei.LEFT_ZeroCheckUpFail);
                    return 0;
                }
                if (iClass != 0)
                {
                    if (dLoadCell > dKg + Load) MT_GoIncSlow(mi.L_UP_ZLift, -dInc);
                    else MT_GoIncSlow(mi.L_UP_ZLift, dInc);
                }
            }
            if (_id == ax.ETC_LoadCell2)
            {
                if (!MT_GetStop(mi.L_DN_ZLift)) return iClass;
                if (MT_GetCmdPos(mi.L_DN_ZLift) >= PM.GetValueMax(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA))
                {
                    ER_SetErr(ei.LEFT_ZeroCheckDnFail);
                    return 0;
                }
                if (iClass != 0)
                {
                    if (dLoadCell > dKg + Load) MT_GoIncSlow(mi.L_DN_ZLift, -dInc);
                    else MT_GoIncSlow(mi.L_DN_ZLift, dInc);
                }
            }
            if (_id == ax.ETC_LoadCell3)
            {
                if (!MT_GetStop(mi.R_UP_ZLift)) return iClass;
                if (MT_GetCmdPos(mi.R_UP_ZLift) >= PM.GetValueMax(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA))
                {
                    ER_SetErr(ei.RIGH_ZeroCheckFail);
                    return 0;
                }
                if (iClass != 0)
                {
                    if (dLoadCell > dKg + Load) MT_GoIncSlow(mi.R_UP_ZLift, -dInc);
                    else MT_GoIncSlow(mi.R_UP_ZLift, dInc);
                }
            }

            return iClass;
        }

        public void SaveCsv(int _iWorkCnt, double _dAxVel, string _sSub = "")
        {
            //string sPath = @OM.CmnOptn.sRighFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + LOT.GetLotNo() + "_2.csv";
            if(_iWorkCnt == 0) sLotNo = DateTime.Now.ToString("hhmmss");
            string sPath = @OM.CmnOptn.sRighFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + LOT.GetLotNo() + @"\" + sLotNo + "_2.csv";
            string sDir  = Path.GetDirectoryName(sPath + "\\");
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;
            string line  = "";
            string sMode = "" ;
            int iWorkCnt = _iWorkCnt + 1;
            if (OM.DevInfo.iR_Mode == (int)Mode.Height) sMode = "Height" ;
            if (OM.DevInfo.iR_Mode == (int)Mode.Weight) sMode = "Weight" ;
            if (OM.DevInfo.iR_Mode == (int)Mode.Pull_Dest) sMode = "Pulling";
            if (OM.DevInfo.iR_Mode == (int)Mode.GripH ) sMode = "Grip Height";

            if (!File.Exists(sPath))
            {
                line =
                "Time,"             + 
                "Work Count,"       +
                "Load,"             +
                "Motor_Up Pos,"     +
                sMode +      ","    +
                "Acc(mm/sec^2),"    +
                "Velocity(mm/sec)," +
                "Dcc(mm/sec^2),"    +
                "Remain Time(ms),"  +
                "Count,"            +
                "OverLoad,"         +
                "Usb Count \r\n"    ;
                

            }

            FileStream   fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            string sSub = "" ;
            if(_sSub != "") sSub = iWorkCnt + _sSub ;
            else            sSub = iWorkCnt.ToString() ;

            line +=
            DateTime.Now.ToString("yyyyMMdd-hhmmss") + "," +
            sSub                                     + "," +
            _dAxVel                                  + "," +
            MT_GetCmdPos(mi.R_UP_ZLift)              + "," ;

            if (OM.DevInfo.iR_Mode == (int)Mode.Height)
            {
                line += OM.DevInfo.dR_H_Height.ToString() + "," + OM.DevInfo.dR_H_Acc.ToString() + "," + OM.DevInfo.dR_H_Vel.ToString() + "," + OM.DevInfo.dR_H_Dcc.ToString() + "," +
                        OM.DevInfo.iR_H_Time.ToString() + "," + OM.DevInfo.iR_H_Count.ToString() + "," + OM.DevInfo.dR_H_Over.ToString() + ",";
            }
            if (OM.DevInfo.iR_Mode == (int)Mode.Weight) 
            {
                line += OM.DevInfo.dR_W_Weight.ToString() + "," + OM.DevInfo.dR_W_Acc.ToString() + "," + OM.DevInfo.dR_W_Vel.ToString() + "," + OM.DevInfo.dR_W_Dcc.ToString() + "," +
                        OM.DevInfo.iR_W_Time.ToString() + "," + OM.DevInfo.iR_W_Count.ToString() + "," + OM.DevInfo.dR_W_Over.ToString() + ",";
            }
            if (OM.DevInfo.iR_Mode == (int)Mode.Pull_Dest)
            {
                line += OM.DevInfo.dR_P_Height.ToString() + "," + OM.DevInfo.dR_P_Acc.ToString() + "," + OM.DevInfo.dR_P_Vel.ToString() + "," + OM.DevInfo.dR_P_Dcc.ToString() + "," +
                        OM.DevInfo.iR_P_Time.ToString() + "," + OM.DevInfo.iR_P_Count.ToString() + "," + OM.DevInfo.dR_P_Over.ToString() + ",";
            }
            line += iUsbCnt.ToString();
            sw.WriteLine(line);
            sw.Close();
            fs.Close();
        }

        public bool CheckOver(int _iNo, int _iCycle, double _dOver = 0)
        {
            if (_iCycle == 10)
            {
                if (_iNo == 1 && !IO_GetX(xi.ETC_LoadCellOK1)) ER_SetErr(ei.ETC_LoadCellOk1);
                if (_iNo == 2 && !IO_GetX(xi.ETC_LoadCellOK2)) ER_SetErr(ei.ETC_LoadCellOk2);
                if (_iNo == 3 && !IO_GetX(xi.ETC_LoadCellOK3)) ER_SetErr(ei.ETC_LoadCellOk3);
                //iWorkCnt = 0 ; //초기화를 여기서 쑤셔 넣음.
            }

            double dNow  = GetLoadCell(_iNo);
            double dOver = 0;

            if (_iNo == 1 || _iNo == 2)
            {
                if (OM.DevInfo.iL_Mode == (int)Mode.Height) dOver = OM.DevInfo.dL_H_Over;
                if (OM.DevInfo.iL_Mode == (int)Mode.Weight) dOver = OM.DevInfo.dL_W_Over;
                if (OM.DevInfo.iL_Mode == (int)Mode.Pull_Dest) dOver = OM.DevInfo.dL_D_Over;
                if (OM.DevInfo.iL_Mode == (int)Mode.GripH    ) dOver = OM.DevInfo.dL_G_Over;
            }
            else
            {
                if (OM.DevInfo.iR_Mode == 0) dOver = OM.DevInfo.dR_H_Over;
                if (OM.DevInfo.iR_Mode == 1) dOver = OM.DevInfo.dR_W_Over;
                if (OM.DevInfo.iR_Mode == 2) dOver = OM.DevInfo.dR_P_Over;
            }

            if(_dOver != 0) dOver = _dOver;
            //TODO :: 인장일시에는 작은걸로 비교 해야 하는건가 테스트 해보고 넣으자.
            if (Math.Abs(dNow) > Math.Abs(dOver))
            {
                MT_EmgStopAll();
                if (_iNo == 1) ER_SetErr(ei.ETC_LoadCellOver1);
                if (_iNo == 2) ER_SetErr(ei.ETC_LoadCellOver2);
                if (_iNo == 3) ER_SetErr(ei.ETC_LoadCellOver3);
                return false;
            }
            
            return true;
        }

        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.MAX_ACTR){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            else {
                sMsg = "Cylinder " + CL_GetName(_eActr) + " is Not this parts.";
                bRet = false;
            }
            
            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Trace(CL_GetName(_eActr) + " " + sMsg);
                if (Step.iCycle==0) Log.ShowMessage(CL_GetName(_eActr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool CheckSafe(mi _eMotr, pv _ePstn ,  double _dOfsPos=0)
        {
            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";

            if (_eMotr == mi.R_UP_ZLift)
            {
                //if (!MT_GetStopInpos(mi.WSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.WSTG_YGrpr) + "is moving.";
                //    bRet = false;
                //}
            }
            else
            {
                sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
                bRet = false;
            }
            
            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Trace(MT_GetName(_eMotr) + " " + sMsg);
                //메뉴얼 동작일때.
                if (Step.eSeq == sc.Idle) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool MoveMotrSlow(mi _eMotr , pv _ePstn ,  double _dOfsPos=0)
        {
            if (!CheckSafe(_eMotr, _ePstn , _dOfsPos)) return false;

            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            MT_GoAbsSlow(_eMotr, dDstPos); 
            return true ;

        }

        //무브함수들의 리턴이 Done을 의미 한게 아니고 명령 전달이 됐는지 여부로 바꿈.
        //Done 확인을 위해서는 GetStop을 써야함.
        public bool MoveMotr(mi _eMotr , pv _ePstn ,  double _dOfsPos=0)
        {
            if (!CheckSafe(_eMotr, _ePstn , _dOfsPos)) return false;

            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            int    iSpdPer = PM_GetValueSpdPer(_eMotr, _ePstn);

            if (Step.iCycle!=0) MT_GoAbsRun(_eMotr , dDstPos, iSpdPer);
            else                MT_GoAbsMan(_eMotr , dDstPos);
            return true ;        
        }


        //무브함수들의 리턴이 Done을 의미 한게 아니고 명령 전달이 됐는지 여부로 바꿈.
        //Done 확인을 위해서는 GetStop을 써야함.
        public bool MoveCyl(ci _eActr, fb _eFwd)
        {
            if (!CheckSafe(_eActr, _eFwd)) return false;

            CL_Move(_eActr, _eFwd);
            
            return true;
        }

        public bool CheckStop()
        {
            if (!MT_GetStop(mi.R_UP_ZLift)) return false;

            return true;
        }

        public void Trace(string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = m_sPartName.Replace(",", "");
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", m_iPartId);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            Log.SendMessage(sFullMsg);
        }

    };

    

   
    
}
