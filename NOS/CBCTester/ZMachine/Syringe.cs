﻿using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class Syringe : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
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
            Suck       ,
            Supply     ,
            Clean      ,
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

        public Syringe(int _iPartId = 0)
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
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );
                    Step.iToStart++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove)) return false ;
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgTube);
                    Step.iToStart++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgTube)) return false ;

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
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );
                    Step.iToStop++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove)) return false ;
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgTube);
                    Step.iToStop++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgTube)) return false ;

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
                    
                //사이클
                bool isSuck    =  DM.ARAY[ri.SUT].CheckAllStat(cs.Work ) && DM.ARAY[ri.SYL].CheckAllStat(cs.None);
                bool isSupply  =  DM.ARAY[ri.SYL].CheckAllStat(cs.Work ) && DM.ARAY[ri.CHA].IsExist     (cs.None);
                bool isClean   =  DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd ) ;
                bool isWorkEnd =  DM.ARAY[ri.SUT].CheckAllStat(cs.None ) && DM.ARAY[ri.SYL].CheckAllStat(cs.None)&& DM.ARAY[ri.CHA].IsExist     (cs.None);

                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.
                     if (isClean  ) { DM.ARAY[ri.PKR].Trace(m_iPartId); Step.eSeq = sc.Clean  ; }
                else if (isSupply ) { DM.ARAY[ri.PKR].Trace(m_iPartId); Step.eSeq = sc.Supply ; }
                else if (isSuck   ) { DM.ARAY[ri.PKR].Trace(m_iPartId); Step.eSeq = sc.Suck   ; }
                else if (isWorkEnd) { Stat.bWorkEnd = true; return true; }             
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
                default           : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case (sc.Idle   ): return false;
                case (sc.Suck  ): if (!CycleSuck  ()) return false; break;
                case (sc.Supply): if (!CycleSupply()) return false; break;
                case (sc.Clean ): if (!CycleClean ()) return false; break;


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
            
            return DM.ARAY[_iId].FindLastColFrstRow(ref c, ref r , _iChip);
        }

        public bool CycleHome()
        {
            string sTemp ;
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
                    MT_GoHome(mi.SYR_ZSyrg);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!MT_GetHomeDone(mi.SYR_ZSyrg))return false ;
                    MT_GoHome(mi.SYR_ZSyrg);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetHomeDone(mi.SYR_ZSyrg))return false ;
                    
                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CycleSuck()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);

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
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgTube   );
                    SEQ.PKR.MoveMotr(mi.PKR_YSutl , pv.PKR_YSutlSyinge );
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgTube  ))return false ;
                    if(!MT_GetStopPos(mi.PKR_YSutl , pv.PKR_YSutlSyinge))return false ;
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgTube );
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgTube))return false ;
                    //sun 펌프로 빨기.

                    Step.iCycle++;
                    return false ;

                case 14:
                    //펌프 다 빨기 기다림.
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;

                    DM.ARAY[ri.SUT].SetStat(cs.WorkEnd);
                    DM.ARAY[ri.SYL].SetStat(cs.Work   );

                    

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        int iWorkCol = 0;
        public bool CycleSupply()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);

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
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    SEQ.CHA.MoveCyl(ci.SYR_MixCoverClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.SYR_MixCoverClOp , fb.Bwd)) return false ;
                    iWorkCol = DM.ARAY[ri.CHA].FindFrstCol(cs.None);
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgChamberStt , PM.GetValue(mi.SYR_XSyrg , pv.SYR_XSyrgChamberPitch) * iWorkCol  );
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgChamberStt , PM.GetValue(mi.SYR_XSyrg , pv.SYR_XSyrgChamberPitch) * iWorkCol ))return false ;
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamer );
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamer))return false ;
                    //sun 펌프로 밀기.

                    Step.iCycle++;
                    return false ;

                case 15:
                    //펌프 다 밀기 기다림.
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;

                    DM.ARAY[ri.CHA].SetStat(iWorkCol , 0 , cs.Work);

                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.Work))
                    {
                        DM.ARAY[ri.SYL].SetStat(cs.WorkEnd);
                    }

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CycleClean()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);

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
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove );
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    MoveMotr(mi.SYR_XSyrg , pv.SYR_XSyrgClean );

                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.SYR_XSyrg , pv.SYR_XSyrgClean ))return false ;

                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamer );
                    //sun 펌프로 밀기.
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamer))return false ;
                    

                    Step.iCycle++;
                    return false ;

                case 14:
                    
                    MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    //펌프 끔.
                    DM.ARAY[ri.PKR].SetStat(cs.None);

                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            //if (_eActr == ci.SYR_MixCoverClOp)
            //{
            //
            //}
            //else if (_eActr == ci.PKR_PkrFtRr)
            //{
            //
            //}
            //else if (!_bChecked)
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

            if (_eMotr == mi.SYR_XSyrg)
            {
                if(MT_GetCmdPos(mi.SYR_ZSyrg) > PM.GetValue(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove)+1.0)
                {
                    sMsg = "Motor " + MT_GetName(mi.SYR_ZSyrg) + "가 이동 포지션이 아닙니다.";
                    bRet = false;

                }
            }
            else if(_eMotr == mi.SYR_ZSyrg)
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

            if (_eMotr == mi.SYR_XSyrg)
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
            else if (_eMotr == mi.SYR_ZSyrg)
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
            if( !MT_GetStop(mi.PKR_ZPckr)) return false;
            if( !MT_GetStop(mi.PKR_YSutl)) return false;
            
            if (!CL_Complete(ci.PKR_PkrShakeUpDn)) return false;
            if (!CL_Complete(ci.PKR_PkrFtRr     )) return false;
            if (!CL_Complete(ci.PKR_PkrClampClOp)) return false;

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
