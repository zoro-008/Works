using System;
using COMMON;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;

namespace Machine
{
    public class Left : Part
    {
        public delegate void CamVoid  (); //델리게이트 선언
        public delegate void CamString(string _sPath, int _iW, int _iH); //델리게이트 선언
        public event CamVoid   CStart; //델리게이트 이벤트 선언
        public event CamVoid   CStop ; //델리게이트 이벤트 선언
        public event CamString CRec  ; //델리게이트 이벤트 선언
        public void Start() { CStart(); }
        public void Stop () { CStop(); CStart(); }
        public void Rec  (string _sPath,int _iW,int _iH) {
            string sPath = _sPath ;
            if(sPath == "") sPath = @OM.CmnOptn.sLeftFolder + @"\" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".avi";
            CRec(sPath, _iW, _iH);
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
            WorkD      ,
            WorkG      ,
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
        protected CDelayTimer m_tmDelay2 ;        

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

        public int    iUsbCnt   ;
        public int    iCount1   ;
        public int    iCount2   ;
        public bool   bToStart  ;

        public int    iDestCnt  ;
        //이거는 테스트 해보고 바꺼 줘야 할거.
        public double dMinLoad ;
        public const int iTime0 = 300 ; //Zero
        public const int iTime1 = 300 ; //Weight
        public const int iTime2 = 30  ; //Fast 
        //public const double dMinLoad1 = 0.2   ;
        //public const double dMinLoad2 = 0.5   ;
        //public const double dMinInc1  = 0.01  ;
        //public const double dMinInc2  = 0.001 ;
        //public const double dMinInc3  = 0.0005;

        public double dCheckLoad1 ;
        public double dCheckLoad2 ;

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

        public Left(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;
            
            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();
            m_tmDelay1    = new CDelayTimer();
            m_tmDelay2    = new CDelayTimer();

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
                    //iWorkCnt = 0;
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
                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    //Stop();
                    Step.iToStop++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    dCheckLoad1 = SEQ.AIO_GetX(ax.ETC_LoadCell1);
                    dCheckLoad2 = SEQ.AIO_GetX(ax.ETC_LoadCell2);
                    if (OM.DevInfo.iL_Mode == 3) {
                        MoveMotrSlow(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG);
                        MT_GoAbsSlow(mi.L_DN_ZLift, 0.0);
                    }
                    else {
                        MT_GoAbsSlow(mi.L_UP_ZLift, 0.0);
                        MT_GoAbsSlow(mi.L_DN_ZLift, 0.0);
                    }

                    Step.iToStop++;
                    return false;

                case 12:
                    if(!CheckOver(1,Step.iToStop,Math.Abs(dCheckLoad1) + 2)) return true;
                    if(!CheckOver(2,Step.iToStop,Math.Abs(dCheckLoad2) + 2)) return true;
                    
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
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
                if (!OM.CmnOptn.bUse_L_Part) return true;
                dKg = 0;
                if(OM.DevInfo.iL_Mode == (int)Mode.Weight   ) dKg = OM.DevInfo.dL_W_Weight ;
                if(OM.DevInfo.iL_Mode == (int)Mode.Pull_Dest) dKg = 1 ;

                bool isCycleZero   =   OM.DevInfo.iL_Mode != (int)Mode.GripH  && bToStart ;//(iWorkCnt == 0 || bToStart)      ;//&& iWorkCnt != -1;
                bool isCycleWorkH  =   OM.DevInfo.iL_Mode == (int)Mode.Height && iWorkCnt < OM.DevInfo.iL_H_Count ;//&& iWorkCnt != -1;
                bool isCycleWorkW  =   OM.DevInfo.iL_Mode == (int)Mode.Weight && iWorkCnt < OM.DevInfo.iL_W_Count ;//&& iWorkCnt != -1;
                bool isCycleWorkD  =   OM.DevInfo.iL_Mode == (int)Mode.Pull_Dest   && iWorkCnt < 1 ;
                bool isCycleWorkG  =   OM.DevInfo.iL_Mode == (int)Mode.GripH       && iWorkCnt < OM.DevInfo.iL_G_Count ;//&& iWorkCnt != -1;

                bool isCycleEnd    = !isCycleWorkH && !isCycleWorkW && !isCycleZero;
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleZero  ) { Step.eSeq = sc.Zero   ;  }
                else if (isCycleWorkH ) { Step.eSeq = sc.WorkH  ;  }
                else if (isCycleWorkW ) { Step.eSeq = sc.WorkW  ;  }
                else if (isCycleWorkD ) { Step.eSeq = sc.WorkD  ;  }
                else if (isCycleWorkG ) { Step.eSeq = sc.WorkG  ;  }
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
                default         :                       Trace("default End");                                    Step.eSeq = sc.Idle;   return false;
                case (sc.Idle  ):                                                                                                       return false;
                case (sc.Zero  ): if (CycleZero (dKg                   )) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.WorkH ): if (CycleWorkH(OM.DevInfo.iL_H_Count )) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.WorkW ): if (CycleWorkW(OM.DevInfo.iL_W_Count )) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.WorkD ): if (CycleWorkD(                      )) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case (sc.WorkG ): if (CycleWorkG(OM.DevInfo.iL_G_Count )) { Trace(sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
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
                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    dCheckLoad1 = SEQ.AIO_GetX(ax.ETC_LoadCell1);
                    dCheckLoad2 = SEQ.AIO_GetX(ax.ETC_LoadCell2);
                    MT_GoHome(mi.L_UP_ZLift);
                    MT_GoHome(mi.L_DN_ZLift);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!CheckOver(1,Step.iHome,Math.Abs(dCheckLoad1) + 2)) return true;
                    if(!CheckOver(2,Step.iHome,Math.Abs(dCheckLoad2) + 2)) return true;
                    if (!MT_GetHomeDone(mi.L_UP_ZLift)) return false;
                    if (!MT_GetHomeDone(mi.L_DN_ZLift)) return false;
                    //MT_GoAbsRun(mi.L_UP_ZLift, PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftWait));
                    //MT_GoAbsRun(mi.L_DN_ZLift, PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftWait));
                    if (OM.DevInfo.iL_Mode == 3) {
                        MoveMotrSlow(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG);
                        MT_GoAbsSlow(mi.L_DN_ZLift, 0.0);
                    }
                    else {
                        MT_GoAbsSlow(mi.L_UP_ZLift, 0.0);
                        MT_GoAbsSlow(mi.L_DN_ZLift, 0.0);
                    }

                    Step.iHome++;
                    return false;

                case 13:
                    if(!CheckOver(1,Step.iHome,Math.Abs(dCheckLoad1) + 2)) return true;
                    if(!CheckOver(2,Step.iHome,Math.Abs(dCheckLoad2) + 2)) return true;

                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    //if (OM.DevInfo.iL_Mode == 3) {
                    //    if(!MT_GetStopPos(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG)) return false;
                    //    if(!MT_GetStopPos(mi.L_DN_ZLift,pv.L_DN_ZLiftWait )) return false;
                    //}
                    //else {
                    //    if(!MT_GetStopPos(mi.L_UP_ZLift,pv.L_UP_ZLiftWait )) return false;
                    //    if(!MT_GetStopPos(mi.L_DN_ZLift,pv.L_DN_ZLiftWait )) return false;
                    //}
                    //if(!MT_GetStopPos(mi.L_UP_ZLift,pv.L_UP_ZLiftWait))return false;
                    //if(!MT_GetStopPos(mi.L_DN_ZLift,pv.L_DN_ZLiftWait))return false;
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
                ER_SetErr(ei.LEFT_CycleTO, sTemp);
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

            if(!CheckOver(1,Step.iCycle)) return true;
            if(!CheckOver(2,Step.iCycle)) return true;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10: 
                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    dCheckLoad1 = SEQ.AIO_GetX(ax.ETC_LoadCell1);
                    dCheckLoad2 = SEQ.AIO_GetX(ax.ETC_LoadCell2);
                    //MT_GoAbsRun(mi.L_UP_ZLift, 0.0);
                    //MT_GoAbsRun(mi.L_DN_ZLift, 0.0);
                    //MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    //MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    if (OM.DevInfo.iL_Mode == 3) {
                        MoveMotrSlow(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG);
                        MT_GoAbsSlow(mi.L_DN_ZLift, 0.0);
                    }
                    else {
                        MT_GoAbsSlow(mi.L_UP_ZLift, 0.0);
                        MT_GoAbsSlow(mi.L_DN_ZLift, 0.0);
                    }

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CheckOver(1,Step.iCycle,Math.Abs(dCheckLoad1) + 2)) return true;
                    if(!CheckOver(2,Step.iCycle,Math.Abs(dCheckLoad2) + 2)) return true;
                    
                    //if(!CheckOver(1,Step.iCycle,3)) return true;
                    //if(!CheckOver(2,Step.iCycle,3)) return true;
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    //if(!MT_GetStopPos(mi.L_UP_ZLift,pv.L_UP_ZLiftWait)) return false;
                    //if(!MT_GetStopPos(mi.L_DN_ZLift,pv.L_DN_ZLiftWait)) return false;
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
                ER_SetErr(ei.LEFT_CycleTO, sTemp);
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
                MT_Stop(mi.L_UP_ZLift);
                MT_Stop(mi.L_DN_ZLift);
                return true ;
            }

            if(!CheckOver(1,Step.iCycle)) return true;
            if(!CheckOver(2,Step.iCycle)) return true;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    if (OM.DevInfo.iL_Mode == (int)Mode.GripH)
                    {
                        Step.iCycle = 0;
                        return true;
                    }

                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    dCheckLoad1 = SEQ.AIO_GetX(ax.ETC_LoadCell1);
                    dCheckLoad2 = SEQ.AIO_GetX(ax.ETC_LoadCell2);
                    MoveMotrSlow(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    MoveMotrSlow(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CheckOver(1,Step.iCycle,Math.Abs(dCheckLoad1) + 2)) return true;
                    if(!CheckOver(2,Step.iCycle,Math.Abs(dCheckLoad2) + 2)) return true;

                    if(!MT_GetStopPos(mi.L_UP_ZLift,pv.L_UP_ZLiftWait)) return false;
                    if(!MT_GetStopPos(mi.L_DN_ZLift,pv.L_DN_ZLiftWait)) return false;
                    IO_SetY(yi.ETC_LoadZero1, true);
                    IO_SetY(yi.ETC_LoadZero2, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero1, false);
                    IO_SetY(yi.ETC_LoadZero2, false);
                    if (OM.DevInfo.iL_Motr == 0)
                    {
                        Step.iCycle = 15;
                        return false;
                    }
                    if(OM.DevInfo.iL_Motr == 1)
                    {
                        Step.iCycle = 15;
                        return false;
                    }
                    Step.iCycle = 30;
                    return false;

                case 15:
                    //ML.IO_SetY(yi.ETC_LoadZero1, true );
                    //ML.IO_SetY(yi.ETC_LoadZero1, false);
                    MoveMotr(mi.L_UP_ZLift, pv.L_UP_ZLiftTchB);
                    
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStopPos(mi.L_UP_ZLift,pv.L_UP_ZLiftTchB)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!m_tmDelay.OnDelay(true, 1000)) return false;
                    if (SEQ.AIO_GetX(ax.ETC_LoadCell1) > 1.0)
                    {
                        ER_SetErr(ei.ETC_LoadCellOver1);
                        return true;
                    }
                    IO_SetY(yi.ETC_LoadZero1, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero1, false);
                    //iCount1 = MoveKg(ax.ETC_LoadCell1, dKg);
                    Step.iCycle=20;
                    return false;

                case 20:
                    //if (dKg == 0)
                    //{
                    //    //dKg = 0.5;
                    //    Step.iCycle++;
                    //    return false;
                    //}
                    m_tmDelay.Clear();
                    Step.iCycle = 21;
                    return false;

                case 21://0일때
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!m_tmDelay1.OnDelay(true, OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (!m_tmDelay.OnDelay(MoveKg(ax.ETC_LoadCell1, dKg, true) == 1, iTime2)) return false;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 22://0이 아닐때
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!m_tmDelay1.OnDelay(true,OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (!m_tmDelay.OnDelay(MoveKg(ax.ETC_LoadCell1, dKg) == 1,iTime0)) return false;
                    dZero = MT_GetEncPos(mi.L_UP_ZLift) - OM.DevInfo.dL_H_ZeroOfs1 ;
                    PM.SetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftTchA,dZero);
                    Step.iCycle++;
                    return false;

                case 23:
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    Trace(dZero.ToString());
                    Trace(PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftTchA).ToString());

                    if (OM.DevInfo.iL_Motr == 0)
                    {
                        Step.iCycle = 30;
                        return false;
                    }
                    bToStart    = false;
                    Step.iCycle = 0;
                    return true;

                case 30://하단부 모터도 제로점 잡기
                    //ML.IO_SetY(yi.ETC_LoadZero1, true );
                    //ML.IO_SetY(yi.ETC_LoadZero1, false);
                    MoveMotr(mi.L_DN_ZLift, pv.L_DN_ZLiftTchB);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 31:
                    if (!MT_GetStopPos(mi.L_DN_ZLift, pv.L_DN_ZLiftTchB)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 32:
                    if (!m_tmDelay.OnDelay(true, 1000)) return false;
                    if (SEQ.AIO_GetX(ax.ETC_LoadCell2) > 1.0)
                    {
                        ER_SetErr(ei.ETC_LoadCellOver2);
                        return true;
                    }

                    IO_SetY(yi.ETC_LoadZero2, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 33:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero2, false);
                    Step.iCycle=40;
                    return false;

                case 40:
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    //if (dKg == 0)
                    //{
                    //    //dKg = 0.5;
                    //    Step.iCycle++;
                    //    return false;
                    //}
                    m_tmDelay.Clear();
                    Step.iCycle=41;
                    return false;

                case 41:
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    if (!m_tmDelay1.OnDelay(true,OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (!m_tmDelay.OnDelay(MoveKg(ax.ETC_LoadCell2, dKg, true) == 1, iTime2)) return false;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 42:
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    if (!m_tmDelay1.OnDelay(true,OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (!m_tmDelay.OnDelay(MoveKg(ax.ETC_LoadCell2, dKg) == 1, iTime0)) return false;
                    dZero = MT_GetEncPos(mi.L_DN_ZLift) - OM.DevInfo.dL_H_ZeroOfs2 ;
                    PM.SetValue(mi.L_DN_ZLift,pv.L_DN_ZLiftTchA,dZero);
                    Step.iCycle++;
                    return false;

                case 43:
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 44:
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;

                    bToStart    = false;
                    Step.iCycle = 0;
                    return true;
/*

                    MoveKg(ax.ETC_LoadCell1,dKg);

                    MT_GoIncRun(mi.L_UP_ZLift,dMinInc1);
                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18://Upside Motor Touch Zero Move Down
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    //if (!m_tmDelay.OnDelay(100)) return false;
                    if(MT_GetCmdPos(mi.L_UP_ZLift) >= PM.GetValueMax(mi.L_UP_ZLift,pv.L_UP_ZLiftTchA))
                    {
                        ER_SetErr(ei.LEFT_ZeroCheckUpFail);
                        return true;
                    }
                    if (dKg == 0) dMinLoad = dMinLoad2;
                    else          dMinLoad = dMinLoad1;
                    if(GetLoadCell(1) < dKg + dMinLoad)
                    {
                        Step.iCycle = 17;
                        return false;
                    }
                    Step.iCycle++;
                    return false;

                case 19://Upside Motor Touch Zero Move Up
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    //MT_GoIncRun(mi.L_UP_ZLift, -dMinInc3);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 20://Upside Motor Touch Zero Move Up
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!m_tmDelay.OnDelay(500)) return false;
                    if(MT_GetCmdPos(mi.L_UP_ZLift) <= PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftWait))
                    {
                        ER_SetErr(ei.LEFT_ZeroCheckUpFail);
                        return true;
                    }
                    if(GetLoadCell(1) > dKg)
                    {
                        MT_GoIncRun(mi.L_UP_ZLift, -dMinInc3);
                        Step.iCycle = 19;
                        return false;
                    }

                    dZero = MT_GetEncPos(mi.L_UP_ZLift);
                    PM.SetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftTchA,dZero);

                    MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    Trace(dZero.ToString());
                    Trace(PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftTchA).ToString());

                    if (OM.DevInfo.iL_Motr == 0)
                    {
                        Step.iCycle = 25;
                        return false;
                    }
                    bToStart    = false;
                    Step.iCycle = 0;
                    return true;

                case 25:
                    //ML.IO_SetY(yi.ETC_LoadZero2, true );
                    //ML.IO_SetY(yi.ETC_LoadZero2, false);
                    MoveMotr(mi.L_DN_ZLift, pv.L_DN_ZLiftTchB);
                    Step.iCycle++;
                    return false;

                case 26:
                    if(!MT_GetStopPos(mi.L_DN_ZLift,pv.L_DN_ZLiftTchB)) return false;
                    Step.iCycle++;
                    return false;

                case 27://Motor Touch Zero Move Down
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    MT_GoIncRun(mi.L_DN_ZLift,dMinInc1);
                    Step.iCycle++;
                    return false;

                case 28://Motor Touch Zero Move Down
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    if(MT_GetCmdPos(mi.L_DN_ZLift) >= PM.GetValueMax(mi.L_DN_ZLift,pv.L_DN_ZLiftTchA))
                    {
                        ER_SetErr(ei.LEFT_ZeroCheckDnFail);
                        return true;
                    }
                    if (dKg == 0) dMinLoad = dMinLoad2;
                    else          dMinLoad = dMinLoad1;
                    if(GetLoadCell(2) < dKg + dMinLoad)
                    {
                        Step.iCycle = 27;
                        return false;
                    }
                    Step.iCycle++;
                    return false;

                case 29://Motor Touch Zero Move Up
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    MT_GoIncRun(mi.L_DN_ZLift,-dMinInc2);
                    Step.iCycle++;
                    return false;

                case 30://Motor Touch Zero Move Up
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    if(MT_GetCmdPos(mi.L_DN_ZLift) <= PM.GetValue(mi.L_DN_ZLift,pv.L_DN_ZLiftWait))
                    {
                        ER_SetErr(ei.LEFT_ZeroCheckDnFail);
                        return true;
                    }
                    if(GetLoadCell(2) > dKg)
                    {
                        Step.iCycle = 29;
                        return false;
                    }
                    dZero = MT_GetEncPos(mi.L_DN_ZLift);
                    PM.SetValue(mi.L_DN_ZLift,pv.L_DN_ZLiftTchA,dZero);
                    MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);

                    bToStart    = false;
                    Step.iCycle = 0;
                    return true;
 */
            }
        }

        public bool CycleWorkH(int _iCnt = 0)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.LEFT_CycleTO, sTemp);
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
                MT_Stop(mi.L_UP_ZLift);
                MT_Stop(mi.L_DN_ZLift);
                return true;
            }

            if (!CheckOver(1, Step.iCycle)) return true;
            if (!CheckOver(2, Step.iCycle)) return true;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //iWorkCnt = 0;
                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    //Start();
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    dCheckLoad1 = SEQ.AIO_GetX(ax.ETC_LoadCell1);
                    dCheckLoad2 = SEQ.AIO_GetX(ax.ETC_LoadCell2);
                    MoveMotrSlow(mi.L_UP_ZLift, pv.L_UP_ZLiftWait);
                    MoveMotrSlow(mi.L_DN_ZLift, pv.L_DN_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CheckOver(1,Step.iCycle,Math.Abs(dCheckLoad1) + 2)) return true;
                    if(!CheckOver(2,Step.iCycle,Math.Abs(dCheckLoad2) + 2)) return true;
                    if (!MT_GetStopPos(mi.L_UP_ZLift, pv.L_UP_ZLiftWait)) return false;
                    if (!MT_GetStopPos(mi.L_DN_ZLift, pv.L_DN_ZLiftWait)) return false;
                    if (OM.DevInfo.iL_Motr == 0)
                    {
                        if (PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA) == 0 || PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }
                        bUseUp = true;
                        bUseDn = true;
                    }
                    else if (OM.DevInfo.iL_Motr == 1)
                    {
                        if (PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }
                        bUseUp = true;
                        bUseDn = false;
                    }
                    else
                    {
                        if (PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }
                        bUseUp = false;
                        bUseDn = true;
                    }
                    //if (bUseUp) MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftTchH);
                    //if (bUseDn) MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftTchH);
                    Step.iCycle++;
                    return false;

                case 13:
                    //if(!CheckOver(1,Step.iCycle,3)) return true;
                    //if(!CheckOver(2,Step.iCycle,3)) return true;
                    //if (bUseUp && !MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftTchH)) return false;
                    //if (bUseDn && !MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftTchH)) return false;

                    //if(bUseUp && SEQ.AIO_GetX(ax.ETC_LoadCell1) > 3) { ER_SetErr(ei.LEFT_NeedZeroCheck); return true; }
                    //if(bUseDn && SEQ.AIO_GetX(ax.ETC_LoadCell2) > 3) { ER_SetErr(ei.LEFT_NeedZeroCheck); return true; }
                    
                    IO_SetY(yi.ETC_LoadZero1, true);
                    IO_SetY(yi.ETC_LoadZero2, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero1, false);
                    IO_SetY(yi.ETC_LoadZero2, false);
                    //ML.IO_SetY(yi.ETC_LoadZero1, false);
                    //ML.IO_SetY(yi.ETC_LoadZero2, false);
                    //Rec();
                    //if (!OM.CmnOptn.bUse_L_Zero)
                    //{//메뉴얼로 지정한 값으로 내려 감
                    //    dZero1 = PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchH) + OM.DevInfo.dL_H_Height;
                    //    dZero2 = PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchH) + OM.DevInfo.dL_H_Height;
                    //}
                    //else
                    //{//측정한 제로 값으로 내려감
                        dZero1 = PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA) + OM.DevInfo.dL_H_Height;
                        dZero2 = PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA) + OM.DevInfo.dL_H_Height;
                    //}

                    MoveMotr(mi.L_UP_ZLift, pv.L_UP_ZLiftWait);
                    MoveMotr(mi.L_DN_ZLift, pv.L_DN_ZLiftWait);
                    Step.iCycle = 15;
                    return false;

                case 15: //다운 , 돌림
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    if (bUseUp) MT_GoAbs(mi.L_UP_ZLift, dZero1, OM.DevInfo.dL_H_Vel, OM.DevInfo.dL_H_Acc, OM.DevInfo.dL_H_Dcc);
                    if (bUseDn) MT_GoAbs(mi.L_DN_ZLift, dZero2, OM.DevInfo.dL_H_Vel, OM.DevInfo.dL_H_Acc, OM.DevInfo.dL_H_Dcc);
                    Step.iCycle++;
                    return false;

                case 16: //다운체크
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17: //유지시간
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iL_H_Time)) return false;
                    if (OM.CmnOptn.bUse_L_Dark)
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    Step.iCycle = 30;
                    return false;

                case 20: //매크로
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
                    Step.iCycle=30;
                    return false;

                case 30: //저장
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell1), SEQ.AIO_GetX(ax.ETC_LoadCell2));

                    if (bUseUp) MoveMotr(mi.L_UP_ZLift, pv.L_UP_ZLiftWait);
                    if (bUseDn) MoveMotr(mi.L_DN_ZLift, pv.L_DN_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 31: //대기포지션
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 32: //대기시간
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iL_H_Wait)) return false;

                    iWorkCnt++;
                    SPC.LOT.Data.WorkCnt1++;
                    if (_iCnt == 0)
                    {
                        if (iWorkCnt < OM.DevInfo.iL_H_Count)
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
                    //MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    //MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    //Stop();
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleWorkW(int _iCnt = 0)
        {
            String sTemp;
            bool bRet1, bRet2;
            //if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 6000))
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.LEFT_CycleTO, sTemp);
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
                MT_Stop(mi.L_UP_ZLift);
                MT_Stop(mi.L_DN_ZLift);
                return true ;
            }

            if(!CheckOver(1,Step.iCycle)) return true;
            if(!CheckOver(2,Step.iCycle)) return true;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //iWorkCnt = 0;
                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    //Start();
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    dCheckLoad1 = SEQ.AIO_GetX(ax.ETC_LoadCell1);
                    dCheckLoad2 = SEQ.AIO_GetX(ax.ETC_LoadCell2);
                    MoveMotrSlow(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    MoveMotrSlow(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CheckOver(1,Step.iCycle,Math.Abs(dCheckLoad1) + 2)) return true;
                    if(!CheckOver(2,Step.iCycle,Math.Abs(dCheckLoad2) + 2)) return true;
                    if(!MT_GetStopPos(mi.L_UP_ZLift,pv.L_UP_ZLiftWait)) return false;
                    if(!MT_GetStopPos(mi.L_DN_ZLift,pv.L_DN_ZLiftWait)) return false;
                    //Rec();
                    //if (!OM.CmnOptn.bUse_L_Zero)
                    //{//메뉴얼로 지정한 값으로 내려 감
                    //    dZero1 = PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftTchW) ;//+ OM.DevInfo.dL_H_Height ;
                    //    dZero2 = PM.GetValue(mi.L_DN_ZLift,pv.L_DN_ZLiftTchW) ;//+ OM.DevInfo.dL_H_Height ;
                    //}
                    //else
                    //{//측정한 제로 값으로 내려감
                        dZero1 = PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftTchA) ;//+ OM.DevInfo.dL_H_Height ;
                        dZero2 = PM.GetValue(mi.L_DN_ZLift,pv.L_DN_ZLiftTchA) ;//+ OM.DevInfo.dL_H_Height ;
                    //}

                    if (OM.DevInfo.iL_Motr == 0)
                    {
                        bUseUp = true ;
                        bUseDn = true ;
                        if (PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA) == 0 || PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }
                    }
                    else if(OM.DevInfo.iL_Motr == 1)
                    {
                        bUseUp = true ;
                        bUseDn = false;
                        if (PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }
                    }
                    else
                    {
                        if (PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }

                        bUseUp = false;
                        bUseDn = true ;
                    }
                    dKg = OM.DevInfo.dL_W_Weight;
                    Step.iCycle = 15;
                    return false;

                case 15: //다운
                    if(bUseUp) MT_GoAbs(mi.L_UP_ZLift,dZero1,OM.DevInfo.dL_W_Vel,OM.DevInfo.dL_W_Acc,OM.DevInfo.dL_W_Dcc);
                    if(bUseDn) MT_GoAbs(mi.L_DN_ZLift,dZero2,OM.DevInfo.dL_W_Vel,OM.DevInfo.dL_W_Acc,OM.DevInfo.dL_W_Dcc);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (OM.CmnOptn.bUse_L_Cort)
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    Step.iCycle = 30;
                    return false;

                case 20: //뒤에서 사용
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 21:
                    bRet1 = true;
                    bRet2 = true;
                    
                    //if (!m_tmDelay1.OnDelay(true, OM.CmnOptn.iLoadTime)) return false;
                    //m_tmDelay1.Clear();
                    if (bUseUp) {
                        if (!m_tmDelay1.OnDelay(true, OM.CmnOptn.iLoadTime)) return false;
                        m_tmDelay1.Clear();
                        bRet1 = MoveKg(ax.ETC_LoadCell1, dKg) == 1; 
                    }
                    //if (!m_tmDelay1.OnDelay(true, 30)) return false;
                    //m_tmDelay1.Clear();
                    if (bUseDn) {
                        if (!m_tmDelay2.OnDelay(true, OM.CmnOptn.iLoadTime)) return false;
                        m_tmDelay2.Clear(); 
                        bRet2 = MoveKg(ax.ETC_LoadCell2, dKg) == 1;
                    }
                    //if (!m_tmDelay1.OnDelay(true,30)) return false;
                    //m_tmDelay1.Clear();
                    if (!m_tmDelay.OnDelay(bRet1 && bRet2, iTime1)) return false;
                    Step.iCycle = 30;
                    return false;                    

                case 30:
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 31:
                    if (!m_tmDelay1.OnDelay(true,OM.CmnOptn.iLoadTime)) return false;
                    m_tmDelay1.Clear();
                    if (bUseUp && OM.CmnOptn.bUse_L_Cort) { MoveKg(ax.ETC_LoadCell1, dKg);}

                    if (bUseDn && OM.CmnOptn.bUse_L_Cort) {
                        //if (!m_tmDelay2.OnDelay(true, 30)) return false;
                        //m_tmDelay2.Clear();
                        MoveKg(ax.ETC_LoadCell2, dKg);
                    }
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iL_W_Time)) return false; //유지시간

                    if (OM.CmnOptn.bUse_L_Dark)
                    {
                        Step.iCycle = 40;
                        return false;
                    }
                    Step.iCycle = 50;
                    return false;


                case 40: //매크로
                    SEQ.MCR.CycleInit(1, (iWorkCnt+1).ToString());
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

                case 50: //저장
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell1), SEQ.AIO_GetX(ax.ETC_LoadCell2));
                    Step.iCycle++;
                    return false;

                case 51:
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    if(bUseUp) MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    if(bUseDn) MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 52:
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 53: //대기시간
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iL_W_Wait)) return false; 
                    iWorkCnt++;
                    SPC.LOT.Data.WorkCnt1++;
                    if (_iCnt == 0)
                    {
                        if (iWorkCnt < OM.DevInfo.iL_W_Count)
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
                    //MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWait);
                    //MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    PM.SetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA, 0);
                    PM.SetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA, 0);
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleWorkD(int _iCnt = 0)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.LEFT_CycleTO, sTemp);
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
                MT_Stop(mi.L_UP_ZLift);
                MT_Stop(mi.L_DN_ZLift);
                return true;
            }
            bool b1, b2;
            if (!CheckOver(1, Step.iCycle)) return true;
            if (!CheckOver(2, Step.iCycle)) return true;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    iWorkCnt = 0;
                    iDestCnt = 0;
                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    //Start();
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    dCheckLoad1 = SEQ.AIO_GetX(ax.ETC_LoadCell1);
                    dCheckLoad2 = SEQ.AIO_GetX(ax.ETC_LoadCell2);
                    MoveMotrSlow(mi.L_UP_ZLift, pv.L_UP_ZLiftWait);
                    MoveMotrSlow(mi.L_DN_ZLift, pv.L_DN_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!CheckOver(1,Step.iCycle,Math.Abs(dCheckLoad1) + 2)) return true;
                    if(!CheckOver(2,Step.iCycle,Math.Abs(dCheckLoad2) + 2)) return true;
                    if (!MT_GetStopPos(mi.L_UP_ZLift, pv.L_UP_ZLiftWait)) return false;
                    if (!MT_GetStopPos(mi.L_DN_ZLift, pv.L_DN_ZLiftWait)) return false;
                    IO_SetY(yi.ETC_LoadZero1, true);
                    IO_SetY(yi.ETC_LoadZero2, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero1, false);
                    IO_SetY(yi.ETC_LoadZero2, false);
                    //ML.IO_SetY(yi.ETC_LoadZero1, false);
                    //ML.IO_SetY(yi.ETC_LoadZero2, false);
                    //Rec();
                    //if (!OM.CmnOptn.bUse_L_Zero)
                    //{//메뉴얼로 지정한 값으로 내려 감
                    //    dZero1 = PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchH) + OM.DevInfo.dL_H_Height;
                    //    dZero2 = PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchH) + OM.DevInfo.dL_H_Height;
                    //}
                    //else
                    //{//측정한 제로 값으로 내려감
                    //dZero1 = PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA) - 0.2; //테스트 필요
                    //dZero2 = PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA) - 0.2;

                    dZero1 = PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA); //테스트 필요
                    dZero2 = PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA);
                    //}

                    if (OM.DevInfo.iL_Motr == 0)
                    {
                        if (PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA) == 0 || PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }
                        bUseUp = true;
                        bUseDn = true;
                    }
                    else if (OM.DevInfo.iL_Motr == 1)
                    {
                        if (PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }
                        bUseUp = true;
                        bUseDn = false;
                    }
                    else
                    {
                        if (PM.GetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA) == 0)
                        {
                            ER_SetErr(ei.LEFT_NeedZeroCheck);
                            return true;
                        }
                        bUseUp = false;
                        bUseDn = true;
                    }
                    Step.iCycle = 15;
                    return false;

                case 15:
                    if (bUseUp) MT_GoAbsRun(mi.L_UP_ZLift, dZero1);
                    if (bUseDn) MT_GoAbsRun(mi.L_DN_ZLift, dZero2);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if (!MT_GetStop(mi.L_DN_ZLift)) return false;
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iL_D_Time)) return false;
                    if (OM.CmnOptn.bUse_L_Dark)
                    {
                        Step.iCycle = 20;
                        return false;
                    }
                    Step.iCycle = 30;
                    return false;

                case 20:
                    iDestCnt++;
                    SEQ.MCR.CycleInit(1,iDestCnt.ToString());
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
                    SaveCsv(iDestCnt-1, SEQ.AIO_GetX(ax.ETC_LoadCell1), SEQ.AIO_GetX(ax.ETC_LoadCell2));
                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 31:
                    b1 = false; 
                    b2 = false; 
                    if (bUseUp && SEQ.AIO_GetX(ax.ETC_LoadCell1) < OM.DevInfo.dL_D_Weight) { dZero1 += OM.DevInfo.dL_D_Height; b1 = true; }
                    if (bUseDn && SEQ.AIO_GetX(ax.ETC_LoadCell2) < OM.DevInfo.dL_D_Weight) { dZero2 += OM.DevInfo.dL_D_Height; b2 = true; }
                    if(b1 || b2)
                    {
                        Step.iCycle = 15;
                        return false;
                    }
                    MoveMotr(mi.L_UP_ZLift, pv.L_UP_ZLiftWait);
                    MoveMotr(mi.L_DN_ZLift, pv.L_DN_ZLiftWait);
                    Step.iCycle++;
                    return false;

                case 32:
                    if (!MT_GetStopPos(mi.L_UP_ZLift, pv.L_UP_ZLiftWait)) return false;
                    if (!MT_GetStopPos(mi.L_DN_ZLift, pv.L_DN_ZLiftWait)) return false;
                    iWorkCnt++;
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleWorkG(int _iCnt = 0)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 6000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.LEFT_CycleTO, sTemp);
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
                MT_Stop(mi.L_UP_ZLift);
                return true ;
            }

            if(!CheckOver(1,Step.iCycle)) return false;
            if(!CheckOver(2,Step.iCycle)) return false;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //iWorkCnt = 0;
                    MT_Stop(mi.L_UP_ZLift);
                    MT_Stop(mi.L_DN_ZLift);
                    //Start();
                    Step.iCycle++;
                    return false;

                case 11: 
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;
                    if(!MT_CmprPos(mi.L_UP_ZLift, PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG)))
                    {
                        ER_SetErr(ei.LEFT_GripWaitCheck);
                        return true;
                    }
                    if(SEQ.AIO_GetX(ax.ETC_LoadCell1) > 3) { ER_SetErr(ei.LEFT_NeedZeroCheck); return true; }

                    dZero1 = PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG) - OM.DevInfo.dL_G_Height1 ;
                    dZero2 = PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG) + OM.DevInfo.dL_G_Height2 ;
                    //MoveMotr(mi.L_DN_ZLift,pv.L_DN_ZLiftWait);
                    MT_GoAbsRun(mi.L_DN_ZLift, 0.0);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStop(mi.L_DN_ZLift)) return false;

                    IO_SetY(yi.ETC_LoadZero1, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    IO_SetY(yi.ETC_LoadZero1, false);
                    Step.iCycle = 15;
                    return false;

                case 15: //위로 이동
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;

                    MT_GoAbs(mi.L_UP_ZLift,dZero1,OM.DevInfo.dL_G_Vel,OM.DevInfo.dL_G_Acc,OM.DevInfo.dL_G_Dcc);
                    Step.iCycle++;
                    return false;

                case 16: //위로 이동 체크
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17: //대기 시간
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iL_G_Time)) return false;
                    if (OM.CmnOptn.bUse_L_Dark)
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
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell1), SEQ.AIO_GetX(ax.ETC_LoadCell2),"_1");
                    if(OM.DevInfo.iL_G_Wait1 != 0) MT_GoAbs(mi.L_UP_ZLift,PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG),OM.DevInfo.dL_G_Vel,OM.DevInfo.dL_G_Acc,OM.DevInfo.dL_G_Dcc);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                    //개귀찮 두번행!!! 밑으로 이동
                case 31: //밑으로 이동
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iL_G_Wait1)) return false;
                    MT_GoAbs(mi.L_UP_ZLift,dZero2,OM.DevInfo.dL_G_Vel,OM.DevInfo.dL_G_Acc,OM.DevInfo.dL_G_Dcc);
                    Step.iCycle++;
                    return false;

                case 32: //밑으로 이동
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 33: //유지시간
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iL_G_Time)) return false;
                    if (OM.CmnOptn.bUse_L_Dark)
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
                    SaveCsv(iWorkCnt, SEQ.AIO_GetX(ax.ETC_LoadCell1), SEQ.AIO_GetX(ax.ETC_LoadCell2),"_2");
                    if(OM.DevInfo.iL_G_Wait2 != 0) MT_GoAbs(mi.L_UP_ZLift,PM.GetValue(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG),OM.DevInfo.dL_G_Vel,OM.DevInfo.dL_G_Acc,OM.DevInfo.dL_G_Dcc);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                    //개귀찮 두번행!!! 밑으로 이동
                case 51: //밑으로 이동
                    if(!MT_GetStop(mi.L_UP_ZLift)) return false;
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iL_G_Wait2)) return false;

                    Step.iCycle=60;
                    return false;

                case 60:

                    iWorkCnt++;
                    SPC.LOT.Data.WorkCnt1++;

                    
                    if (_iCnt == 0)
                    {
                        if (iWorkCnt < OM.DevInfo.iL_G_Count)
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

                    MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG);
                    Step.iCycle++;
                    return false;

                case 61:
                    if (!MT_GetStop(mi.L_UP_ZLift)) return false;
                    //Stop();
                    Step.iCycle = 0;
                    return true;

            }
        }

        public int MoveKg(ax _id, double dKg = 0, bool bFast = false)
        {
            double Load = 0;
            int iType = 0; //slow
            if (dKg == 0)
            {
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

            if(dAbs <= OM.CmnOptn.dLoadRange) iClass = 1;

            if(_id == ax.ETC_LoadCell1)
            {
                if (!MT_GetStop(mi.L_UP_ZLift)) return iClass;
                if(MT_GetCmdPos(mi.L_UP_ZLift) >= PM.GetValueMax(mi.L_UP_ZLift,pv.L_UP_ZLiftTchA)) 
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
            if(_id == ax.ETC_LoadCell2)
            {
                if (!MT_GetStop(mi.L_DN_ZLift)) return iClass;
                if(MT_GetCmdPos(mi.L_DN_ZLift) >= PM.GetValueMax(mi.L_DN_ZLift,pv.L_DN_ZLiftTchA)) 
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
            if(_id == ax.ETC_LoadCell3)
            {
                if (!MT_GetStop(mi.R_UP_ZLift)) return iClass;
                if(MT_GetCmdPos(mi.R_UP_ZLift) >= PM.GetValueMax(mi.R_UP_ZLift,pv.R_UP_ZLiftTchA)) 
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

        public void SaveCsv(int _iWorkCnt, double _dAxVel1 , double _dAxVel2 = 0, string _sSub = "")
        {
            if(_iWorkCnt == 0) sLotNo = DateTime.Now.ToString("hhmmss");
            string sPath = @OM.CmnOptn.sLeftFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + LOT.GetLotNo() + @"\" + sLotNo + "_1.csv";
            string sDir  = Path.GetDirectoryName(sPath + "\\");
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;
            string line     = "";
            string sMode    = "" ;
            int    iWorkCnt = _iWorkCnt + 1 ;

            if (OM.DevInfo.iL_Mode == (int)Mode.Weight) sMode = "Weight";
            else                                        sMode = "Height";
            if (!File.Exists(sPath))
            {
                line =
                "Time,"             + 
                "Work Count,"       +
                "Load_Up,"          +
                "Load_Dn,"          +
                "Motor_Up Pos,"     +
                "Motor_Dn Pos,"     +
                sMode +       ","   +
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
            sSub + "," +
            _dAxVel1.ToString() + "," +
            _dAxVel2.ToString() + "," +
            MT_GetCmdPos(mi.L_UP_ZLift) + "," +
            MT_GetCmdPos(mi.R_UP_ZLift) + "," ;
            if (OM.DevInfo.iL_Mode == (int)Mode.Height) {
                line += OM.DevInfo.dL_H_Height.ToString() + "," + OM.DevInfo.dL_H_Acc.ToString() + "," + OM.DevInfo.dL_H_Vel.ToString() + "," + OM.DevInfo.dL_H_Dcc.ToString() + "," +
                        OM.DevInfo.iL_H_Time.ToString() + "," + OM.DevInfo.iL_H_Count.ToString() + "," + OM.DevInfo.dL_H_Over.ToString() + ",";
            }
            else if(OM.DevInfo.iL_Mode == (int)Mode.Weight) 
            {
                line += OM.DevInfo.dL_W_Weight.ToString() + "," + OM.DevInfo.dL_W_Acc.ToString() + "," + OM.DevInfo.dL_W_Vel.ToString() + "," + OM.DevInfo.dL_W_Dcc.ToString() + "," +
                        OM.DevInfo.iL_W_Time.ToString() + "," + OM.DevInfo.iL_W_Count.ToString() + "," + OM.DevInfo.dL_W_Over.ToString() + ",";
            }
            else if(OM.DevInfo.iL_Mode == (int)Mode.Pull_Dest) 
            {
                line += OM.DevInfo.dL_D_Weight.ToString() + "," + "-" + "," + "-" + "," + "-" + "," +
                        OM.DevInfo.iL_D_Time.ToString() + "," + "-" + "," + OM.DevInfo.dL_D_Over.ToString() + ",";
            }
            else 
            {
                line += OM.DevInfo.dL_G_Height1.ToString() + "_" +OM.DevInfo.dL_G_Height2.ToString() + "," + OM.DevInfo.dL_G_Acc.ToString() + "," + OM.DevInfo.dL_G_Vel.ToString() + "," + OM.DevInfo.dL_G_Dcc.ToString() + "," +
                        OM.DevInfo.iL_G_Time.ToString() + "," + OM.DevInfo.iL_G_Count.ToString() + "," + OM.DevInfo.dL_G_Over.ToString() + ",";
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
                if (OM.DevInfo.iR_Mode == (int)Mode.Height) dOver = OM.DevInfo.dR_H_Over;
                if (OM.DevInfo.iR_Mode == (int)Mode.Weight) dOver = OM.DevInfo.dR_W_Over;
                if (OM.DevInfo.iR_Mode == (int)Mode.Pull_Dest) dOver = OM.DevInfo.dR_P_Over;
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

            if (_eMotr == mi.L_UP_ZLift)
            {
                //if (!MT_GetStopInpos(mi.WSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.WSTG_YGrpr) + "is moving.";
                //    bRet = false;
                //}
            }
            else if (_eMotr == mi.L_DN_ZLift)
            {

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
            if (!MT_GetStop(mi.L_UP_ZLift)) return false;
            if (!MT_GetStop(mi.L_DN_ZLift)) return false;

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
