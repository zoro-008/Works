﻿using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class Rail : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop   ;
            public void Clear()
            {
                bWorkEnd    = false ;
                bReqStop    = false ;
            }
        };   

        public enum sc
        {
            Idle    = 0,
            Pick       ,
            MoveToWork ,
            InputWork  ,
            OutputWork ,
            WorkToMove ,
            Place      ,
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
        protected String m_sCheckSafeMsg;

        public CTimer[] m_CycleTime;

        public Rail(int _iPartId = 0)
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

                    Step.iToStop++;
                    return false;

                case 11: 
                    
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

                bool isCyclePick       =  false;
                bool isCycleMoveToWork =  false;
                bool isCycleInputWork  =  false;
                bool isCycleOutputWork =  false;
                bool isCycleWorkToMove =  false;
                bool isCyclePlace      =  false;
                bool isCycleEnd        =  false;
                                         

                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.

                //DM.ARAY[ri.BLDR].Trace(m_iPartId);
                //DM.ARAY[ri.BLDR].Trace(m_iPartId);
                     if (isCyclePick      ) { DM.ARAY[ri.PREB].Trace(m_iPartId); Step.eSeq = sc.Pick          ; }
                else if (isCycleMoveToWork) { DM.ARAY[ri.PREB].Trace(m_iPartId); Step.eSeq = sc.MoveToWork    ; }
                else if (isCycleInputWork ) { DM.ARAY[ri.PREB].Trace(m_iPartId); Step.eSeq = sc.InputWork     ; }
                else if (isCycleOutputWork) { DM.ARAY[ri.PREB].Trace(m_iPartId); Step.eSeq = sc.OutputWork    ; }
                else if (isCycleWorkToMove) { DM.ARAY[ri.PREB].Trace(m_iPartId); Step.eSeq = sc.WorkToMove    ; }
                else if (isCyclePlace     ) { DM.ARAY[ri.PREB].Trace(m_iPartId); Step.eSeq = sc.Place         ; }
                else if (isCycleEnd       ) { Stat.bWorkEnd = true; return true; }
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
                default             : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case (sc.Idle      ): return false;
                case (sc.Pick      ): if (!CyclePick      ()) return false; break;
                case (sc.MoveToWork): if (!CycleMoveToWork()) return false; break;
                case (sc.InputWork ): if (!CycleInputWork ()) return false; break;
                case (sc.OutputWork): if (!CycleOutputWork()) return false; break;
                case (sc.WorkToMove): if (!CycleWorkToMove()) return false; break;
                case (sc.Place     ): if (!CyclePlace     ()) return false; break;
            }
            Trace(sCycle+" End"); 
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
            
            DM.ARAY[_iId].FindLastColFrstRow(ref c, ref r , _iChip);
            return true;
        }

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 300000 )) {
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
            
            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    return true ;

                case 10:

                    Step.iHome++;
                    return false;

                case 11:

                    Step.iHome++;
                    return false;

                case 12:

                    Step.iHome++;
                    return false;

                case 13:

                    Step.iHome++;
                    return false;

                case 14:

                    Step.iHome++;
                    return false;

                case 15:

                    Step.iHome++;
                    return false;

                case 16:

                    Step.iHome++;
                    return false;

                case 17:

                    Step.iHome++;
                    return false;

                case 18:

                    Step.iHome++;
                    return false;

                case 19:

                    Step.iHome++;
                    return false;

                case 20:

                    Step.iHome++;
                    return false;

                case 21:

                    Step.iHome++;
                    return false;

                case 22:

                    Step.iHome++;
                    return false;

                case 23:

                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CyclePick()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
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

            int r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                //초기화
                case 10:

                    Step.iCycle++;
                    return false;

                case 11:

                    Step.iCycle++;
                    return false;

                case 12:

                    Step.iCycle++;
                    return false;

                case 13:

                    Step.iCycle++;
                    return false;

                case 14:

                    Step.iCycle++;
                    return false;

                case 15:

                    Step.iCycle++;
                    return false;

                case 16:

                    Step.iCycle++;
                    return false;

                case 17:

                    Step.iCycle++;
                    return false;

                case 18:

                    Step.iCycle++;
                    return false;

                case 19:

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleMoveToWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
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
                //return true;
            }

            int r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:

                    Step.iCycle++;
                    return false;

                case 11:

                    Step.iCycle++;
                    return false;

                case 12:

                    Step.iCycle++;
                    return false;

                case 13:

                    Step.iCycle++;
                    return false;

                case 14:

                    Step.iCycle++;
                    return false;

                case 15:

                    Step.iCycle++;
                    return false;

                case 16:

                    Step.iCycle++;
                    return false;

                case 17:

                    Step.iCycle++;
                    return false;

                case 18:

                    Step.iCycle++;
                    return false;

                case 19:

                    Step.iCycle++;
                    return false;

                case 20:

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleInputWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
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

            int r, c = 0;
        
            switch (Step.iCycle)
            {
        
                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;
        
                case 10:

                    Step.iCycle++;
                    return false;

                case 11:
        
                    Step.iCycle++;
                    return false;
        
                case 12:

                    Step.iCycle++;
                    return false;

                case 13:

                    Step.iCycle++;
                    return false;
        
                case 14:
                        
                    Step.iCycle++;
                    return false;
        
                case 15:
        
                    Step.iCycle++;
                    return false;
        
                case 16:

                    Step.iCycle++;
                    return false;

                case 17:

                    Step.iCycle++;
                    return false;

                case 18:

                    Step.iCycle++;
                    return false;

                case 19:

                    Step.iCycle++;
                    return false;

                case 20:

                    Step.iCycle++;
                    return false;

                case 21:

                    Step.iCycle++;
                    return false;

                case 22:
        
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleOutputWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
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

            int r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    
                    Step.iCycle++;
                    return false;

                case 11:

                    Step.iCycle++;
                    return false;

                case 12:

                    Step.iCycle++;
                    return false;

                case 13:

                    Step.iCycle++;
                    return false;

                case 14:
                        
                    Step.iCycle++;
                    return false;

                case 15:

                    Step.iCycle++;
                    return false;

                case 16:

                    Step.iCycle++;
                    return false;

                case 17:

                    Step.iCycle++;
                    return false;

                case 18:

                    Step.iCycle++;
                    return false;

                case 19:

                    Step.iCycle++;
                    return false;

                case 20:

                    Step.iCycle++;
                    return false;

                case 21:
                    
                    Step.iCycle++;
                    return false;

                case 22:

                    Step.iCycle++;
                    return false;

                case 23:

                    Step.iCycle++;
                    return false;

                case 24:

                    Step.iCycle++;
                    return false;

                case 25:

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleWorkToMove()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
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

            int r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:

                    Step.iCycle++;
                    return false;

                case 11:

                    Step.iCycle++;
                    return false;

                case 12:

                    Step.iCycle++;
                    return false;

                case 13:
                        
                    Step.iCycle++;
                    return false;

                case 14:

                    Step.iCycle++;
                    return false;

                case 15:

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePlace()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
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

            int r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:

                    Step.iCycle++;
                    return false;

                case 11:

                    Step.iCycle++;
                    return false;

                case 12:

                    Step.iCycle++;
                    return false;

                case 13:

                    Step.iCycle++;
                    return false;

                case 14:

                    Step.iCycle++;
                    return false;

                case 15:

                    Step.iCycle++;
                    return false;

                case 16:

                    Step.iCycle++;
                    return false;

                case 17:

                    Step.iCycle++;
                    return false;

                case 18:

                    Step.iCycle++;
                    return false;

                case 19:

                    Step.iCycle++;
                    return false;

                case 20:

                    Step.iCycle++;
                    return false;

                case 21:

                    Step.iCycle++;
                    return false;

                case 22:

                    Step.iCycle++;
                    return false;

                case 23:

                    Step.iCycle++;
                    return false;

                case 24:

                    Step.iCycle++;
                    return false;

                case 25:

                    Step.iCycle++;
                    return false;

                case 26:

                    Step.iCycle++;
                    return false;

                case 27:

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.RAIL_Vsn1AlignFwBw)
            {

            }
            else if (_eActr == ci.RAIL_Vsn2AlignFwBw)
            {

            }
            else if (_eActr == ci.RAIL_Vsn3AlignFwBw)
            {

            }
            else if (_eActr == ci.RAIL_MarkAlignFWBw)
            {

            }
            else if (_eActr == ci.PSTB_PusherFwBw)
            {

            }
            else if (_eActr == ci.PREB_StprUpDn)
            {

            }
            else if (_eActr == ci.RAIL_Vsn1StprUpDn)
            {

            }
            else if (_eActr == ci.RAIL_Vsn2StprUpDn)
            {

            }
            else if (_eActr == ci.RAIL_Vsn3StprUpDn)
            {

            }
            else if (_eActr == ci.RAIL_MarkStprUpDn)
            {

            }
            else if (_eActr == ci.RAIL_Vsn1SttnUpDn)
            {

            }
            else if (_eActr == ci.RAIL_Vsn2SttnUpDn)
            {

            }
            else if (_eActr == ci.RAIL_Vsn3SttnUpDn)
            {

            }
            else if (_eActr == ci.RAIL_MarkSttnUpDn)
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

            //if (_eMotr == mi.PSTB_XMark)
            //{
            //
            //}
            //else if (_eMotr == mi.PSTB_YMark)
            //{
            //
            //}
            //else
            //{
            //    sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
            //    bRet = false;
            //}

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

            //if (_eMotr == mi.PSTB_XMark)
            //{
            //    if (_eDir == EN_JOG_DIRECTION.Neg) //아래
            //    {
            //
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //    else //위
            //    {
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //}
            //else if (_eMotr == mi.PSTB_YMark)
            //{
            //    if (_eDir == EN_JOG_DIRECTION.Neg) //아래
            //    {
            //
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //    else //위
            //    {
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //}
            //else
            //{
            //    sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
            //    bRet = false;
            //}

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
            if (!CL_Complete(ci.RAIL_Vsn1AlignFwBw)) return false;
            if (!CL_Complete(ci.RAIL_Vsn2AlignFwBw)) return false;
            if (!CL_Complete(ci.RAIL_Vsn3AlignFwBw)) return false;
            if (!CL_Complete(ci.RAIL_MarkAlignFWBw )) return false;
            if (!CL_Complete(ci.RAIL_Vsn1StprUpDn  )) return false;
            if (!CL_Complete(ci.RAIL_Vsn2StprUpDn  )) return false;
            if (!CL_Complete(ci.RAIL_Vsn3StprUpDn  )) return false;
            if (!CL_Complete(ci.RAIL_MarkStprUpDn  )) return false;
            if (!CL_Complete(ci.RAIL_Vsn1SttnUpDn  )) return false;
            if (!CL_Complete(ci.RAIL_Vsn2SttnUpDn  )) return false;
            if (!CL_Complete(ci.RAIL_Vsn3SttnUpDn  )) return false;
            if (!CL_Complete(ci.RAIL_MarkSttnUpDn  )) return false;

            return true ;
        }
        //public void SaveLotName(string _sLotName)
        //{
        //    string sLotDataPath = "C:\\Data\\LotName.ini";
        //    CIniFile IniLotDatadSave = new CIniFile(sLotDataPath);

        //    IniLotDatadSave.Save("LotName", "LotName", _sLotName);

        //}

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
