using System;
using COMMON;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Machine
{
    public class Work : Part
    {
        public enum sc
        {
            Idle    = 0,
            Work       ,
            MAX_SEQ_CYCLE
        };

        public struct TWorkInfo
        {
            public double dAbsFeed    ; //"Abs피딩"
            public double dRotPos     ; //"로테이션"    
            public double dCog        ; //"칼날진입각도"
            public double dCogUp      ; //"가시세움각도"
            public double dTableWork  ; //"스테이지높이"
            public double dTableWait  ; //"스테이지대기"
            public double dSwitchWork ; //"방향선택"    
            public double dBladBfWork ; //"칼날작업대기"
            public double dBladWork   ; //"칼날작업"
        }
        public List<TWorkInfo> lsWorkList = new List<TWorkInfo> ();

        public CCycleTimer WorkTimer = new CCycleTimer();
        public CCycleTimer CutTimer  = new CCycleTimer();

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
        protected String m_sCheckSafeMsg;

        public CTimer[] m_CycleTime;

        public Work(int _iPartId)
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

            WorkTimer.Clear();
            CutTimer.Clear();

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
                    Step.iToStart++;
                    return false;

                case 11:
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
                    Step.iToStop++;
                    return false;

                case 11:
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
                bool isCycleWork   = !OM.CmnOptn.bRewindMode && OM.DevOptn.iLotWorkCount > OM.EqpStat.iWorkCount ;
                bool isCycleEnd  = !isCycleWork ;

                if (ER_IsErr()) return false;

                //Normal Decide Step.
                     if (isCycleWork    ) { Step.eSeq = sc.Work      ;  }
                else if (isCycleEnd     ) { Stat.bWorkEnd = true; return true; }
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
                default         : Trace("default End"); 
                                  Step.eSeq = sc.Idle;   return false;
                case sc.Idle    :                        return false;
                case sc.Work    : if (!CycleWork    ())  return false; break ;
            }

            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;

        }
        //인터페이스 상속 끝.==================================================
        #endregion        
        
        Random Ran = new Random();
        public void MakeWorkList()
        {
            lsWorkList.Clear();
            //List<TWorkInfo> lsTempList = new List<TWorkInfo>();

            double dBladeOfs  =  OM.DevOptn.dLeftRightDist / 2.0 ;

            //Left->Right , Right->Left 이렇게 2개가 있는데 절차상 모두 2번이다.
            //나중에 L->R->L 이런거 나오면 분기하여 3으로 세팅 해야함.
            List<int> lsWorkDir = new List<int>();
            string sPattern = OM.DevOptn.iPattern.ToString();

            //패턴 스트링 쪼개서 하나씩 넣는다.
            for(int i = 0 ; i < sPattern.Length ; i++){
                lsWorkDir.Add(CConfig.StrToIntDef(sPattern[i].ToString() , 0));
            }


            double dFeedPos = 0.0 ;
            double dRotPos  = 0.0 ;
            bool   bRotPlus = true ; //로테이션 방향이 정방향으로 가고 있는지 역방향으로 가고 있는지.
            TWorkInfo Info = new TWorkInfo();
            for(int j = 0 ; j < OM.DevOptn.iRepeatCnt ; j++)
            {
                //리피트 간 오프셑.
                if(j!=0)dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedRepeatOfs) ; 
                for(int i = 0 ; i < lsWorkDir.Count ; i++)
                {                    
                    dRotPos  = 0.0 ;

                    if(lsWorkDir[i] == 0)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad0Cog   );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad0CogUp );
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit0Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl0Work  );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl0Wait  );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad0BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad0Work  );



                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed0SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed0CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed0CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed0CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos ;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit0Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed0CogLengOfs) ;
                    }

                    else if(lsWorkDir[i] == 1)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad1Cog   );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad1CogUp );
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit1Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl1Work  );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl1Wait  );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad1BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad1Work  );



                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed1SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed1CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed1CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed1CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos ;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit1Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed1CogLengOfs) ;
                    }
                    else if(lsWorkDir[i] == 2)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad2Cog  );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad2CogUp);
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit2Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl2Work );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl2Wait );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad2BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad2Work  );

                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed2SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed2CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed2CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed2CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit2Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed2CogLengOfs) ;
                    }
                    else if(lsWorkDir[i] == 3)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad3Cog  );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad3CogUp);
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit3Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl3Work );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl3Wait );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad3BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad3Work  );

                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed3SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed3CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed3CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed3CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit3Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed3CogLengOfs) ;
                    }
                    else if(lsWorkDir[i] == 4)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad4Cog  );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad4CogUp);
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit4Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl4Work );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl4Wait );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad4BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad4Work  );

                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed4SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed4CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed4CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed4CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit4Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed4CogLengOfs) ;
                    }
                    else if(lsWorkDir[i] == 5)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad5Cog  );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad5CogUp);
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit5Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl5Work );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl5Wait );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad5BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad5Work  );

                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed5SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed5CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed5CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed5CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit5Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed5CogLengOfs) ;
                    }

                    else if(lsWorkDir[i] == 6)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad6Cog  );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad6CogUp);
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit6Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl6Work );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl6Wait );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad6BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad6Work  );

                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed6SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed6CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed6CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed6CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit6Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed6CogLengOfs) ;
                    }
                    else if(lsWorkDir[i] == 7)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad7Cog  );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad7CogUp);
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit7Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl7Work );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl7Wait );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad7BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad7Work  );

                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed7SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed7CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed7CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed7CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit7Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed7CogLengOfs) ;
                    }
                    else if(lsWorkDir[i] == 8)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad8Cog  );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad8CogUp);
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit8Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl8Work );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl8Wait );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad8BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad8Work  );

                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed8SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed8CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed8CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed8CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit8Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed8CogLengOfs) ;
                    }
                    else if(lsWorkDir[i] == 9)
                    {   
                        Info.dCog        = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad9Cog  );
                        Info.dCogUp      = PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad9CogUp);
                        Info.dSwitchWork = OM.DevOptn.iWRK_TSwit9Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                        Info.dTableWork  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl9Work );
                        Info.dTableWait  = PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl9Wait );
                        Info.dBladBfWork = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad9BfWork);
                        Info.dBladWork   = PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad9Work  );

                        //먼저 대기 오프셑값 넣고.
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed9SpareOfs) ; 

                        //작업 피치 계산 해서 넣어준다.
                        if (PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed9CogGapOfs) > 0)
                        {
                            for (double dCogPos = 0.0; dCogPos <= PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed9CogLengOfs); dCogPos += PM.GetValue(mi.WRK_TFeed, pv.WRK_TFeed9CogGapOfs))
                            {
                                Info.dRotPos = dRotPos;//+ PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd);
                                if (OM.DevOptn.iCogRotation == 0) //단방향 이동
                                {
                                    dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                    if (dRotPos > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                    {
                                        dRotPos = 0.0;
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 1) //왕복 이동
                                {
                                    if (bRotPlus)
                                    {//정방향.
                                        if (dRotPos + PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) > PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd))
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = false;
                                        }
                                        else
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                    else
                                    {//역방향.
                                        if (dRotPos - PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs) < 0)
                                        {
                                            dRotPos += PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                            bRotPlus = true;
                                        }
                                        else
                                        {
                                            dRotPos -= PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveOfs);
                                        }
                                    }
                                }
                                else if (OM.DevOptn.iCogRotation == 2) //랜덤 이동
                                {
                                    dRotPos = Ran.Next((int)PM.GetValue(mi.WRK_YRott, pv.WRK_YRottMoveEnd));
                                }

                                //if(dCogPos != 0.0){
                                //    dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedLCogGapOfs) ;
                                //}
                                Info.dAbsFeed = dFeedPos + dCogPos;
                                Info.dAbsFeed += OM.DevOptn.iWRK_TSwit9Work == 0 ? dBladeOfs : -dBladeOfs;
                                lsWorkList.Add(Info);
                            }
                        }
                        dFeedPos += PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed9CogLengOfs) ;
                    }

                    else
                    {
                    }               
                }
            }

            lsWorkList.Sort(delegate(TWorkInfo _tA , TWorkInfo _tB){
                return _tA.dAbsFeed.CompareTo(_tB.dAbsFeed);
            });
        }

        //위에 부터 작업.
        public bool FindChip(int _iId, out int _iC, out int _iR, cs _iChipStat) 
        {
            
            //DM.ARAY[_iId].FindFrstRowCol(_iChipStat, ref _iC, ref _iR);
            //return (_iC >= 0 && _iR >= 0) ? true : false;
            _iC = 0; _iR = 0;
            return false ;
        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 30000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Trace(sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Trace(sTemp);
            }

            PreStep.iHome = Step.iHome ;
           
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    //if(Step.iHome != PreStep.iHome)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true ;
            
                case 10:
                    //MT_SetHomeDone(mi.OUT_TRelB , true);
                    //MT_SetHomeDone(mi.OUT_TRelT , true);
                    //MT_GoHome(mi.OUT_YGuid);

                    MT_SetHomeDone(mi.WRK_TFeed , false);
                    MT_SetHomeDone(mi.WRK_YRott , false);
                    MT_SetHomeDone(mi.WRK_ZTabl , false);
                    MT_SetHomeDone(mi.WRK_TBlad , false);
                    MT_SetHomeDone(mi.WRK_TSwit , false);
                    MT_SetHomeDone(mi.WRK_YBlad , false);

                    MT_SetHomeDone(mi.WRK_TFeed , true );
                    
                    MT_GoHome(mi.WRK_YBlad);
                    Step.iHome++;
                    return false ;

                case 11:
                    if(!MT_GetHomeDone(mi.WRK_YBlad)) return false ;
                    MT_GoAbsMan(mi.WRK_YBlad , PM_GetValue(mi.WRK_YBlad,pv.WRK_YBladWait));
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.WRK_YBlad, pv.WRK_YBladWait)) return false ;
                    MT_GoHome(mi.WRK_ZTabl);
                    Step.iHome++;
                    return false ;

                case 13:
                    if(!MT_GetHomeDone(mi.WRK_ZTabl)) return false ;
                    MT_GoAbsMan(mi.WRK_ZTabl , PM_GetValue(mi.WRK_ZTabl,pv.WRK_ZTablWait));
                    Step.iHome++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.WRK_ZTabl, pv.WRK_ZTablWait)) return false ;
                    MT_GoHome(mi.WRK_YRott);
                    MT_GoHome(mi.WRK_TBlad);
                    MT_GoHome(mi.WRK_TSwit);
                    Step.iHome++;
                    return false ;

                case 15:
                    if(!MT_GetHomeDone(mi.WRK_YRott)) return false ;
                    if(!MT_GetHomeDone(mi.WRK_TBlad)) return false ;
                    if(!MT_GetHomeDone(mi.WRK_TSwit)) return false ;

                    MT_GoAbsMan(mi.WRK_YRott , PM_GetValue(mi.WRK_YRott,pv.WRK_YRottWait));
                    MT_GoAbsMan(mi.WRK_TBlad , PM_GetValue(mi.WRK_TBlad,pv.WRK_TBladWait));
                    MT_GoAbsMan(mi.WRK_TSwit , PM_GetValue(mi.WRK_TSwit,pv.WRK_TSwitWait));
                    Step.iHome++;
                    return false;

                case 16:
                    if (!MT_GetStopPos(mi.WRK_YRott,pv.WRK_YRottWait)) return false;
                    if (!MT_GetStopPos(mi.WRK_TBlad,pv.WRK_TBladWait)) return false;
                    if (!MT_GetStopPos(mi.WRK_TSwit,pv.WRK_TSwitWait)) return false;
                    Step.iHome = 0;
                    return true;
            }
        }

        //public void RepeatGuide()
        //{
        //    if(MT_GetStop(mi.OUT_YGuid)){
        //        if(MT_GetCmdPos(mi.OUT_YGuid) == PM_GetValue(mi.OUT_YGuid , pv.OUT_YGuidFrnt))
        //        {
        //            MoveMotr(mi.OUT_YGuid , pv.OUT_YGuidRear);
        //            OM.EqpStat.bOutGuideToRear = true ;
        //        }
        //        else if(MT_GetCmdPos(mi.OUT_YGuid) == PM_GetValue(mi.OUT_YGuid , pv.OUT_YGuidRear))
        //        {
        //            MoveMotr(mi.OUT_YGuid , pv.OUT_YGuidFrnt);
        //            OM.EqpStat.bOutGuideToRear = false ;
        //        }
        //        else if(OM.EqpStat.bOutGuideToRear)
        //        {
        //            MoveMotr(mi.OUT_YGuid , pv.OUT_YGuidRear);
        //            OM.EqpStat.bOutGuideToRear = true ;
        //        }
        //        else 
        //        {
        //            MoveMotr(mi.OUT_YGuid , pv.OUT_YGuidFrnt);
        //            OM.EqpStat.bOutGuideToRear = false ;
        //        }
        //    }
        //}

        bool bLeft = false ;
        public bool CycleWork()//여기부터
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
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
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    OM.EqpStat.dCutTime = CutTimer.CheckTime_s();
                    CutTimer.Clear();

                    bLeft = lsWorkList[OM.EqpStat.iWorkStep].dSwitchWork == PM_GetValue(mi.WRK_TSwit, pv.WRK_TSwitLWork);
                    Program.SendListMsg("WorkStep=" + (bLeft?"Left ":"Right ") + OM.EqpStat.iWorkStep.ToString() + "/" + (lsWorkList.Count-1).ToString());

                     
                    MT_GoAbsRun(mi.WRK_ZTabl, lsWorkList[OM.EqpStat.iWorkStep].dTableWait);
                    //MoveMotr(mi.WRK_ZTabl , pv.WRK_ZTablWait);
                    Step.iCycle++;
                    return false ;

                case 11: 
                    if(!MT_GetStop(mi.WRK_ZTabl)) return false ;
                    MoveMotr(mi.WRK_YBlad , pv.WRK_YBladWait);
                    Step.iCycle++;
                    return false ;

                case 12: 
                    if(!MT_GetStopPos(mi.WRK_YBlad , pv.WRK_YBladWait)) return false ;

                    if(OM.CmnOptn.bUseYBfPos)Step.iCycle=50 ;
                    else                     Step.iCycle=20 ;
                    return false ;


                case 20:
                    MT_GoAbsRun(mi.WRK_TBlad, lsWorkList[OM.EqpStat.iWorkStep].dCog);
                    MT_GoAbsRun(mi.WRK_TSwit, lsWorkList[OM.EqpStat.iWorkStep].dSwitchWork);
                    //MoveMotr(mi.WRK_TBlad, pv.WRK_TBlad1Cog);
                    //MoveMotr(mi.WRK_TBlad, pv.WRK_TBlad2Cog); 
                    //if(bLeft) {
                    //    
                    //    MoveMotr(mi.WRK_TSwit , pv.WRK_TSwitLWork ) ;
                    //}
                    //else {
                    //    
                    //    MoveMotr(mi.WRK_TSwit , pv.WRK_TSwitRWork ) ;
                    //}
                    MT_GoAbsRun(mi.WRK_YRott , lsWorkList[OM.EqpStat.iWorkStep].dRotPos );
                    MT_GoAbsRun(mi.WRK_TFeed , lsWorkList[OM.EqpStat.iWorkStep].dAbsFeed);
                    Step.iCycle++;
                    return false ;

                case 21: 
                    if(!MT_GetStop(mi.WRK_TBlad )) return false ;
                    if(!MT_GetStop(mi.WRK_TSwit )) return false ;
                    if(!MT_GetStop(mi.WRK_YRott )) return false ;
                    if(!MT_GetStop(mi.WRK_TFeed )) return false ;
                    MT_GoAbsRun(mi.WRK_ZTabl, lsWorkList[OM.EqpStat.iWorkStep].dTableWork);
                    //if(bLeft) {
                    //    MoveMotr(mi.WRK_ZTabl , pv.WRK_ZTabl1Work  ) ; 
                    //}
                    //else {
                    //    MoveMotr(mi.WRK_ZTabl , pv.WRK_ZTabl2Work  ) ; 
                    //}

                    //MoveMotr(mi.WRK_YBlad, pv.WRK_YBladWork); 
                    Step.iCycle++;
                    return false ;

                case 22:
                    if(!MT_GetStop(mi.WRK_ZTabl )) return false ;
                    //MoveMotr(mi.WRK_YBlad , pv.WRK_YBlad1Work  ) ; //uph 향상목적으로 미리 올려 논다.
                    MT_GoAbsRun(mi.WRK_YBlad, lsWorkList[OM.EqpStat.iWorkStep].dBladWork);
                    Step.iCycle++;
                    return false ;

                case 23:
                    //if(!MT_GetStop(mi.WRK_YBlad)) return false ;
                    if(!MT_GetStop(mi.WRK_YBlad )) return false ;
                    MT_GoAbsRun(mi.WRK_TBlad, lsWorkList[OM.EqpStat.iWorkStep].dCogUp);
                    //if(bLeft) {
                    //    MoveMotr(mi.WRK_TBlad , pv.WRK_TBlad1CogUp  ) ; 
                    //}
                    //else {
                    //    MoveMotr(mi.WRK_TBlad , pv.WRK_TBlad2CogUp  ) ; 
                    //}
                    Step.iCycle++;
                    return false ;

                case 24:
                    if(!MT_GetStop(mi.WRK_TBlad)) return false ;

                    MT_GoAbsRun(mi.WRK_TBlad, lsWorkList[OM.EqpStat.iWorkStep].dCog);
                    //if(bLeft) {
                    //    MoveMotr(mi.WRK_TBlad , pv.WRK_TBlad1Cog  ) ; 
                    //}
                    //else {
                    //    MoveMotr(mi.WRK_TBlad , pv.WRK_TBlad2Cog  ) ; 
                    //}
                    MoveMotr(mi.WRK_YBlad , pv.WRK_YBladWait  ) ;
                    //MoveMotr(mi.WRK_ZTabl, pv.WRK_ZTablWait);
                    Step.iCycle++;
                    return false ;

                case 25:
                    if(!MT_GetStop(mi.WRK_TBlad)) return false ;
                    if(!MT_GetStop(mi.WRK_YBlad)) return false ;
                    MT_GoAbsRun(mi.WRK_ZTabl, lsWorkList[OM.EqpStat.iWorkStep].dTableWait);
                    //MoveMotr(mi.WRK_ZTabl , pv.WRK_ZTablWait);
                    Step.iCycle++;
                    return false;

                case 26:
                    if (!MT_GetStop(mi.WRK_ZTabl)) return false;
                    OM.EqpStat.iWorkStep++;
                    if(OM.EqpStat.iWorkStep == lsWorkList.Count){
                        MT_GoIncSlow(mi.WRK_TFeed , PM_GetValue(mi.WRK_TFeed,pv.WRK_TFeedEndOfs));
                    }
                    Step.iCycle++;
                    return false ;

                case 27:  
                    if(!MT_GetStop(mi.WRK_TFeed)) return false ;
                    if(OM.EqpStat.iWorkStep >= lsWorkList.Count) {
                        OM.EqpStat.iWorkCount++;
                        SPC.LOT.Data.WorkCnt++;

                        Program.SendListMsg("WorkCount=" + OM.EqpStat.iWorkCount.ToString() + "/" + OM.DevOptn.iLotWorkCount.ToString());
                        OM.EqpStat.iWorkStep = 0 ;
                        MT_SetPos(mi.WRK_TFeed , 0.0);
                        
                        OM.EqpStat.dWorkTime = WorkTimer.CheckTime_s() ;
                        OM.EqpStat.dUPH      = 3600/OM.EqpStat.dWorkTime ;
                        WorkTimer.Clear();
                    }
                    Step.iCycle = 0;
                    return true;


                case 50:
                    MT_GoAbsRun(mi.WRK_TBlad, lsWorkList[OM.EqpStat.iWorkStep].dCog);
                    MT_GoAbsRun(mi.WRK_TSwit, lsWorkList[OM.EqpStat.iWorkStep].dSwitchWork);
                    //MoveMotr(mi.WRK_TBlad, pv.WRK_TBlad1Cog);
                    //MoveMotr(mi.WRK_TBlad, pv.WRK_TBlad2Cog); 
                    //if(bLeft) {
                    //    
                    //    MoveMotr(mi.WRK_TSwit , pv.WRK_TSwitLWork ) ;
                    //}
                    //else {
                    //    
                    //    MoveMotr(mi.WRK_TSwit , pv.WRK_TSwitRWork ) ;
                    //}
                    MT_GoAbsRun(mi.WRK_YRott , lsWorkList[OM.EqpStat.iWorkStep].dRotPos );
                    MT_GoAbsRun(mi.WRK_TFeed , lsWorkList[OM.EqpStat.iWorkStep].dAbsFeed);
                    Step.iCycle++;
                    return false ;

                case 51: 
                    if(!MT_GetStop(mi.WRK_TBlad )) return false ;
                    if(!MT_GetStop(mi.WRK_TSwit )) return false ;
                    if(!MT_GetStop(mi.WRK_YRott )) return false ;
                    if(!MT_GetStop(mi.WRK_TFeed )) return false ;

                    //MoveMotr(mi.WRK_YBlad , pv.WRK_YBlad1BfWork  ) ; //테스트용 포지션 2개의 퀄리티가 똑같이 나오지 않아 먼저 평평한데까지 가서 쓰는 걸로.
                    MT_GoAbsRun(mi.WRK_YBlad , lsWorkList[OM.EqpStat.iWorkStep].dBladBfWork );
                    
                    Step.iCycle++;
                    return false ;

                case 52:
                    if(!MT_GetStop(mi.WRK_YBlad)) return false ;                    
                    MT_GoAbsRun(mi.WRK_ZTabl, lsWorkList[OM.EqpStat.iWorkStep].dTableWork);
                    //if(bLeft) {
                    //    MoveMotr(mi.WRK_ZTabl , pv.WRK_ZTabl1Work  ) ; 
                    //}
                    //else {
                    //    MoveMotr(mi.WRK_ZTabl , pv.WRK_ZTabl2Work  ) ; 
                    //}

                    //MoveMotr(mi.WRK_YBlad, pv.WRK_YBladWork); 
                    Step.iCycle++;
                    return false ;

                case 53:
                    if(!MT_GetStop(mi.WRK_ZTabl )) return false ;
                    //MoveMotr(mi.WRK_YBlad , pv.WRK_YBlad1Work  ) ; //uph 향상목적으로 미리 올려 논다.
                    MT_GoAbsRun(mi.WRK_YBlad , lsWorkList[OM.EqpStat.iWorkStep].dBladWork );
                    Step.iCycle++;
                    return false ;

                case 54:
                    if(!MT_GetStop(mi.WRK_YBlad)) return false ;
                    MT_GoAbsRun(mi.WRK_TBlad, lsWorkList[OM.EqpStat.iWorkStep].dCogUp);
                    //if(bLeft) {
                    //    MoveMotr(mi.WRK_TBlad , pv.WRK_TBlad1CogUp  ) ; 
                    //}
                    //else {
                    //    MoveMotr(mi.WRK_TBlad , pv.WRK_TBlad2CogUp  ) ; 
                    //}
                    Step.iCycle++;
                    return false ;

                case 55:
                    if(!MT_GetStop(mi.WRK_TBlad)) return false ;

                    MT_GoAbsRun(mi.WRK_TBlad, lsWorkList[OM.EqpStat.iWorkStep].dCog);
                    //if(bLeft) {
                    //    MoveMotr(mi.WRK_TBlad , pv.WRK_TBlad1Cog  ) ; 
                    //}
                    //else {
                    //    MoveMotr(mi.WRK_TBlad , pv.WRK_TBlad2Cog  ) ; 
                    //}
                    MoveMotr(mi.WRK_YBlad , pv.WRK_YBladWait  ) ;
                    //MoveMotr(mi.WRK_ZTabl, pv.WRK_ZTablWait);
                    Step.iCycle++;
                    return false ;

                case 56:
                    if(!MT_GetStop(mi.WRK_TBlad)) return false ;
                    if(!MT_GetStop(mi.WRK_YBlad)) return false ;
                    MT_GoAbsRun(mi.WRK_ZTabl, lsWorkList[OM.EqpStat.iWorkStep].dTableWait);
                    //MoveMotr(mi.WRK_ZTabl , pv.WRK_ZTablWait);
                    Step.iCycle++;
                    return false;

                case 57:
                    if (!MT_GetStop(mi.WRK_ZTabl)) return false;
                    OM.EqpStat.iWorkStep++;
                    if(OM.EqpStat.iWorkStep == lsWorkList.Count){
                        MT_GoIncRun(mi.WRK_TFeed , PM_GetValue(mi.WRK_TFeed,pv.WRK_TFeedEndOfs));
                    }
                    Step.iCycle++;
                    return false ;

                case 58:  
                    if(!MT_GetStop(mi.WRK_TFeed)) return false ;
                    if(OM.EqpStat.iWorkStep >= lsWorkList.Count) {
                        OM.EqpStat.iWorkCount++;
                        SPC.LOT.Data.WorkCnt++;

                        Program.SendListMsg("WorkCount=" + OM.EqpStat.iWorkCount.ToString() + "/" + OM.DevOptn.iLotWorkCount.ToString());
                        OM.EqpStat.iWorkStep = 0 ;
                        MT_SetPos(mi.WRK_TFeed , 0.0);
                        
                        OM.EqpStat.dWorkTime = WorkTimer.CheckTime_s() ;
                        OM.EqpStat.dUPH      = 3600/OM.EqpStat.dWorkTime ;
                        WorkTimer.Clear();
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleManMove(double _dDist)//여기부터
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 20000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
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
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.WRK_ZTabl , pv.WRK_ZTablWait);
                    Step.iCycle++;
                    return false ;

                case 11: 
                    if(!MT_GetStopPos(mi.WRK_ZTabl , pv.WRK_ZTablWait)) return false ;
                    MoveMotr(mi.WRK_YBlad , pv.WRK_YBladWait);
                    Step.iCycle++;
                    return false ;

                case 12: 
                    if(!MT_GetStopPos(mi.WRK_YBlad , pv.WRK_YBladWait)) return false ;

                    MT_GoIncRun(mi.WRK_TFeed , _dDist);

                    Step.iCycle++;
                    return false ;

                case 13: 
                    if(!MT_GetStop(mi.WRK_TFeed )) return false ;

                    MT_SetPos(mi.WRK_TFeed, lsWorkList[OM.EqpStat.iWorkStep].dAbsFeed);
              
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            //if(_eActr == ci.INDX_DoorClOp){
            //    if(_eFwd == fb.Fwd) {
            //        if(CL_Complete(ci.INDX_TrayFeedFwBw, fb.Fwd))
            //        {
            //            sMsg = "Feeder가 전진해 있습니다.";
            //            bRet = false ;
            //        }
            //    }
            //}

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(CL_GetName(_eActr), sMsg);
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

            if(_eMotr == mi.WRK_TFeed){
            }
            else if(_eMotr == mi.WRK_YRott){
            }
            else if(_eMotr == mi.WRK_ZTabl){
            }
            else if(_eMotr == mi.WRK_TBlad){
            }
            else if(_eMotr == mi.WRK_TSwit){
            }
            else if(_eMotr == mi.WRK_YBlad){
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
            if (!MT_GetStop(mi.WRK_TFeed)) return false;
            if (!MT_GetStop(mi.WRK_YRott)) return false;
            if (!MT_GetStop(mi.WRK_ZTabl)) return false;
            if (!MT_GetStop(mi.WRK_TBlad)) return false;
            if (!MT_GetStop(mi.WRK_TSwit)) return false;
            if (!MT_GetStop(mi.WRK_YBlad)) return false;

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
