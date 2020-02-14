using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class Loader : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
            public bool bReqStop   ;
            public bool bSupplied  ; //한번 공급 했음.
            public void Clear()
            {
                bWorkEnd    = false ;
                bReqStop    = false ;
                bSupplied   = false ;
            }
        };   

        public enum sc
        {
            Idle    = 0,
            Supply     ,
            Move       ,
            Out        ,
            OutPush    ,
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

        protected String      m_sPartName;
        protected int         m_iPartId  ;
        //Timer.
        protected CDelayTimer m_tmMain   ;
        protected CDelayTimer m_tmCycle  ;
        protected CDelayTimer m_tmHome   ;
        protected CDelayTimer m_tmToStop ;
        protected CDelayTimer m_tmToStart;
        protected CDelayTimer m_tmDelay  ;        

        protected SStat Stat;
        protected SStep Step, PreStep;

        protected double m_dLastIdxPos;
        protected string m_sCheckSafeMsg;

        public CTimer[] m_CycleTime;

        public Loader(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();

            m_CycleTime   = new CTimer[(int)sc.MAX_SEQ_CYCLE];

            for(int i = 0 ; i < (int)sc.MAX_SEQ_CYCLE ; i++)
            {
                m_CycleTime [i]  = new CTimer();
            }

            m_iPartId = _iPartId;

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
            Step.iToStart = 10;
            //Ok.
            return true;

        }
        override public bool ToStart() //스타트를 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 5000))
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
                    Step.iToStart = 0;
                    return true;

                case 10:
                    Stat.bSupplied = false;
                    MoveCyl(ci.LDR_OutPshrFtRr , fb.Bwd);
                    Step.iToStart++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.LDR_OutPshrFtRr , fb.Bwd))return false ;
                    Step.iToStart++;
                    return false;

                case 12:

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
                    Step.iToStop = 0;
                    return true;

                case 10:
                    MT_Stop(mi.LDR_XBelt);
                    MoveMotr(mi.LDR_YBelt, pv.LDR_YBeltWorkStart);
                    Step.iToStop++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.LDR_YBelt, pv.LDR_YBeltWorkStart)) return false;
                    //if(!CL_Complete(ci.PREB_StprUpDn , fb.Fwd)) return false ;
                    Step.iToStop++;
                    return false;

                case 12:

                    Step.iToStop++;
                    return false;
                
                case 13: 

                    Step.iToStop = 0;
                    return true;
            }


        }

        override public int GetHomeStep   () { return      Step.iHome    ; } override public int GetPreHomeStep   () { return PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return      Step.iToStart ; } override public int GetPreToStartStep() { return PreStep.iToStart ; }
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

                if (OM.CmnOptn.bAutoQc){ Stat.bWorkEnd = true; return true; }

                //사이클
                bool isSupply  =  DM.ARAY[ri.LDR].CheckAllStat(cs.None   )  && !IO_GetX(xi.LDR_WorkRackExist) &&  IO_GetX(xi.LDR_InNotArrival    ) && !Stat.bSupplied;
                //bool isIn      =  DM.ARAY[ri.LDR].CheckAllStat(cs.None   )  && !IO_GetX(xi.LDR_WorkRackExist) && !IO_GetX(xi.LDR_InNotArrival    ) ;
                //bool isMove    = !DM.ARAY[ri.LDR].CheckAllStat(cs.Shake  )  &&  DM.ARAY[ri.SUT].CheckAllStat(cs.None   ) && DM.ARAY[ri.PKR].CheckAllStat(cs.None   ) && IO_GetX(xi.LDR_WorkRackExist) ;
                bool isMove    = (DM.ARAY[ri.LDR].CheckAllStat(cs.WorkEnd)|| DM.ARAY[ri.LDR].CheckAllStat(cs.Empty)) &&  IO_GetX(xi.LDR_WorkRackExist);
                bool isOut     = (DM.ARAY[ri.LDR].CheckAllStat(cs.WorkEnd)|| DM.ARAY[ri.LDR].CheckAllStat(cs.Empty)) &&  IO_GetX(xi.LDR_OutNotArrival) && !IO_GetX(xi.LDR_WorkRackExist);
                bool isOutPush = !IO_GetX(xi.LDR_OutNotArrival) ;
                bool isWorkEnd =  DM.ARAY[ri.LDR].CheckAllStat(cs.None   )  && !IO_GetX(xi.LDR_WorkRackExist) && IO_GetX(xi.LDR_OutNotArrival   ) &&Stat.bSupplied ;
                //bSupplied는 Cycle


                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.
                     if (isOutPush) { DM.ARAY[ri.LDR].Trace(m_sPartName); Step.eSeq = sc.OutPush ; }
                else if (isOut    ) { DM.ARAY[ri.LDR].Trace(m_sPartName); Step.eSeq = sc.Out     ; }
                else if (isMove   ) { DM.ARAY[ri.LDR].Trace(m_sPartName); Step.eSeq = sc.Move    ; }
                //else if (isIn     ) { DM.ARAY[ri.LDR].Trace(m_sPartName); Step.eSeq = sc.In      ; }
                else if (isSupply ) { DM.ARAY[ri.LDR].Trace(m_sPartName); Step.eSeq = sc.Supply  ; }
                else if (isWorkEnd) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle){
                    Trace(Step.eSeq.ToString() +" Start");
                    Log.TraceListView(this.ToString() + Step.eSeq.ToString() + " 동작 시작");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }
            
            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default           : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case (sc.Idle    ): return false;
                case (sc.OutPush ): if (!CycleOutPush()) return false; break;
                case (sc.Out     ): if (!CycleOut    ()) return false; break;
                case (sc.Move    ): if (!CycleMove   ()) return false; break;
              //case (sc.In      ): if (!CycleIn     ()) return false; break;
                case (sc.Supply  ): if (!CycleSupply ()) return false; break;

            }
            Trace(sCycle+" End");
            Log.TraceListView(this.ToString() + sCycle + " 동작 종료"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        //위에 부터 작업.
        //인덱스 데이터에서 작업해야 되는 컬로우를 뽑아서 리턴.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            return DM.ARAY[_iId].FindLastColFrstRow(ref c, ref r , _iChip);
        }

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 10000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_HomeTO ,sTemp);
                Trace(sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Trace(sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    return true ;

                case 10:
                    MT_SetServo(mi.LDR_XBelt, true);
                    //MT_SetServo(mi.LDR_XBelt, false);


                    MoveCyl(ci.LDR_OutPshrFtRr  , fb.Bwd);
                    MoveCyl(ci.LDR_WorkStprRrFt , fb.Bwd);
                    MT_Stop(mi.LDR_XBelt);
                    MT_GoHome(mi.LDR_YBelt);
                    
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!CL_Complete(ci.LDR_OutPshrFtRr  , fb.Bwd)) return false ;
                    if(!CL_Complete(ci.LDR_WorkStprRrFt , fb.Bwd)) return false ;
                    if(!MT_GetHomeDone(mi.LDR_YBelt))return false ;

                    MT_GoAbsMan(mi.LDR_YBelt , pv.LDR_YBeltWorkStart);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetStop(mi.LDR_YBelt)) return false ;

                    MT_SetHomeDone(mi.LDR_XBelt , true);
                    MT_SetPos(mi.LDR_XBelt , 0.0);
                    Step.iHome++;
                    return false;

                case 13: 
                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CycleOutPush()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                //IO_SetY(yi.RAIL_FeedingAC3,false);
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

            int r, c = 0;
            Enum pvCheck = new pv();
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    if(!IO_GetX(xi.LDR_OutNotFull))
                    {
                        ER_SetErr(ei.LDR_UnloaderFull , "배출부에 튜브렉을 치워주세요.");
                        return true ;
                    }
                    MoveCyl(ci.LDR_OutPshrFtRr , fb.Fwd);
                    Step.iCycle++;
                    return false;
                    
                case 11:
                    if (!IO_GetX(xi.LDR_OutNotFull))
                    {
                        ER_SetErr(ei.LDR_UnloaderFull, "배출부에 튜브렉을 치워주세요.");
                        MoveCyl(ci.LDR_OutPshrFtRr, fb.Bwd);
                        return true;
                    }
                    if (!CL_Complete(ci.LDR_OutPshrFtRr  ,fb.Fwd)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!m_tmDelay.OnDelay(500))return false ;
                    MoveCyl(ci.LDR_OutPshrFtRr , fb.Bwd);
                    Step.iCycle++;
                    return false;
                    
                case 13: 
                    if(!CL_Complete(ci.LDR_OutPshrFtRr  ,fb.Bwd)) return false;
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CycleOut()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 8000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                //IO_SetY(yi.RAIL_FeedingAC3,false);
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.LDR_OutPshrFtRr , fb.Bwd);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.LDR_OutPshrFtRr  ,fb.Bwd)) return false;
                    MT_JogP(mi.LDR_XBelt);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(m_tmDelay.OnDelay(7000))
                    {
                        ER_SetErr(ei.LDR_UnloaderFull, "배출부에 튜브렉이 진입 못합니다.");
                        MT_Stop(mi.LDR_XBelt);
                        return true;
                    }
                    if(IO_GetX(xi.LDR_OutNotArrival))return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    MT_Stop(mi.LDR_XBelt);
                    DM.ARAY[ri.LDR].SetStat(cs.None);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleMove()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                //IO_SetY(yi.RAIL_FeedingAC3,false);
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.LDR_WorkStprRrFt , fb.Bwd);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.LDR_WorkStprRrFt  ,fb.Bwd)) return false;
                    MT_GoIncRun(mi.LDR_XBelt , PM.GetValue(mi.LDR_XBelt , pv.LDR_XBeltPitch));
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStop(mi.LDR_XBelt))return false ;

                    if (IO_GetX(xi.LDR_WorkRackExist))
                    {
                        if (IO_GetX(xi.LDR_WorkTubeExist))
                        {
                            DM.ARAY[ri.LDR].SetStat(cs.Shake);
                        }
                        else
                        {
                            DM.ARAY[ri.LDR].SetStat(cs.Empty);
                        }
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }


        public bool CycleSupply()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                MoveMotr(mi.LDR_YBelt, pv.LDR_YBeltWorkStart);
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

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    MoveMotr(mi.LDR_YBelt, pv.LDR_YBeltWorkStart);
                    return true;

                case 10:
                    MoveMotr (mi.LDR_YBelt , pv.LDR_YBeltWorkStart);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!MT_GetStopPos(mi.LDR_YBelt  , pv.LDR_YBeltWorkStart)) return false;
                    MoveMotrSlow(mi.LDR_YBelt , pv.LDR_YBeltWorkEnd);
                    Step.iCycle++;
                    return false;
                    
                case 12: 
                    if(!IO_GetX(xi.LDR_InNotArrival))
                    {
                        MT_Stop(mi.LDR_YBelt);
                        Step.iCycle++;
                    }
                    if(!MT_GetStopPos(mi.LDR_YBelt  , pv.LDR_YBeltWorkEnd)) return false;
                    if (IO_GetX(xi.LDR_InNotArrival))
                    {

                        MoveMotr(mi.LDR_YBelt, pv.LDR_YBeltWorkStart);
                        //자제 없고 한번 공급 동작 한것에 대한 플레그 세움.
                        Stat.bSupplied = true;
                        Step.iCycle = 0;
                        return true;
                    }
                        
                    Step.iCycle++;
                    return false ;
                    
                    //위에서 씀.
                case 13: //정상 도착
                    if(!MT_GetStop(mi.LDR_YBelt)) return false;
                    //뒤로 1미리 빼서 안끼게 한다.
                    MT_GoIncRun(mi.LDR_YBelt, -0.1);//  
                    Step.iCycle++;
                    return false ;

                case 14:
                    if (!MT_GetStopInpos(mi.LDR_YBelt)) return false;
                    //
                    Step.iCycle++;
                    return false ;
                case 15:
                    MoveCyl(ci.LDR_WorkStprRrFt, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!CL_Complete(ci.LDR_WorkStprRrFt, fb.Fwd)) return false;
                    MT_JogP(mi.LDR_XBelt);
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!IO_GetX(xi.LDR_WorkRackExist)) return false;
                    //로딩 Y뒤로 뺌.
                    MoveMotr(mi.LDR_YBelt, pv.LDR_YBeltWorkStart);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!m_tmDelay.OnDelay(2000)) return false;
                    MT_Stop(mi.LDR_XBelt);
                    if (!MT_GetStopPos(mi.LDR_YBelt, pv.LDR_YBeltWorkStart)) return false;

                    if (IO_GetX(xi.LDR_WorkTubeExist))
                    {
                        DM.ARAY[ri.LDR].SetStat(cs.Shake);
                    }
                    else
                    {
                        DM.ARAY[ri.LDR].SetStat(cs.Empty);
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.LDR_OutPshrFtRr)
            {

            }
            else if (_eActr == ci.LDR_WorkStprRrFt)
            {

            }
            else if (!_bChecked)
            {
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

        public bool CheckSafe(mi _eMotr, pv _ePstn, double _dOfsPos = 0)
        {
            if (OM.MstOptn.bDebugMode) return true;
            double dDstPos = PM_GetValue(_eMotr, _ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";

            if (_eMotr == mi.LDR_XBelt)
            {
            
            }
            else if(_eMotr == mi.LDR_YBelt)
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

        public bool JogCheckSafe(mi _eMotr, EN_JOG_DIRECTION _eDir, EN_UNIT_TYPE _eType, double _dDist)
        {
            if (OM.MstOptn.bDebugMode) return true;
            bool bRet = true;
            string sMsg = "";

            if (_eMotr == mi.LDR_XBelt)
            {
                if (_eDir == EN_JOG_DIRECTION.Neg) //아래
                {
            
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
            
                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
                    {
            
                    }
                }
                else //위
                {
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
            
                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
                    {
            
                    }
                }
            }
            else if (_eMotr == mi.LDR_YBelt)
            {
                if (_eDir == EN_JOG_DIRECTION.Neg) //아래
                {
            
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
            
                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
                    {
            
                    }
                }
                else //위
                {
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
            
                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
                    {
            
                    }
                }
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
        public bool MoveMotr(mi _eMotr , pv _ePstn ,  double _dOfsPos=0, bool _bLink = true)
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
            if( !MT_GetStop(mi.LDR_XBelt)) return false;
            if( !MT_GetStop(mi.LDR_YBelt)) return false;
            
            if (!CL_Complete(ci.LDR_WorkStprRrFt)) return false;
            if (!CL_Complete(ci.LDR_OutPshrFtRr )) return false;

            if(m_tmDelay.GetUsing())return false ;

            return true ;
        }

        public void Trace(string _name, params bool[] _val)
        {
            string sLog = _name + " = ";
            for(int i=0; i<_val.Length; i++) sLog += _val[i].ToString() + ",";
            Trace(sLog);
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
