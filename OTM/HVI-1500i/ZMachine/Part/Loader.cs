using System;
using COMMON;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
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
            public void Clear()
            {
                bWorkEnd    = false ;
                bReqStop    = false ;
                bSupply     = false ;
            }
            public bool bSupply    ;
        };   

        public enum sc
        {
            Idle    = 0,
            Supply     ,
            Pick       ,
            Work       ,
            Drop       ,
                
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
                    return true;

                case 10:
                    if(IO_GetX(xi.PREB_StrpDetect) &&  DM.ARAY[ri.PREB].CheckAllStat(cs.None) ) //맨마지막 작업했던 자재 데이터로 랏넘버 및 아이디 씌운다. 맨마지막 자제 데이터맵은 날라감.
                    {
                        if(OM.EqpStat.iReinputCnt>99)OM.EqpStat.iReinputCnt=0;
                        DM.ARAY[ri.PREB].SetStat(cs.Unknown);
                        DM.ARAY[ri.PREB].LotNo = OM.CmnOptn.sLdrPreLotNo ;
                        DM.ARAY[ri.PREB].ID    = (9000 + OM.EqpStat.iReinputCnt).ToString() ; //이거 형변환 잘되는지 찾아보자 ...
                        Trace("PRE BUFFER ADD LOT NO" + OM.CmnOptn.sLdrPreLotNo);
                        Trace("PRE BUFFER ADD LOT ID" + OM.CmnOptn.sLdrPreLotId);
                    }
                    Step.iToStart = 0;
                    return true;


            }

        }
        override public bool ToStop() //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 5000)) ER_SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

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

                if(OM.DevInfo.iMgzCntPerLot < 1) OM.DevInfo.iMgzCntPerLot = 1; //1이상이여야 함 다른데서 까먹을거 같아서 여기 넣어놈.
                //자재 상태
                bool None   = DM.ARAY[ri.LODR].CheckAllStat(cs.None   ) ;
                bool Unkwn  = DM.ARAY[ri.LODR].GetCntStat  (cs.Unknown) > 0;
                bool Empty  = DM.ARAY[ri.LODR].CheckAllStat(cs.Empty  ) ;
                bool bMgzSsr1 = IO_GetX(xi.LODR_MgzDetect1) ;
                bool bMgzSsr2 = IO_GetX(xi.LODR_MgzDetect2) ;

                //조건
                bool Supply   = (!IO_GetX(xi.LODR_MgzIn) && !Stat.bSupply) ; //매거진에 구멍땜에 안들어와있을경우 반영 , 매거진 있을때라고 생각
                bool Pick     = !Supply;
                bool PreRdy   = DM.ARAY[ri.PREB].CheckAllStat(cs.None) && SEQ.VSNZ.GetSeqStep() != (int)VisnZone.sc.Move ;
                bool bLot     = LOT.GetNextMgz() != "" ;
                bool bLotWait = LOT.GetLotNo() != DM.ARAY[ri.LODR].LotNo ; //랏변경시에 레일을 비우고 시작한다.
                bool bLotMove = DM.ARAY[ri.PREB].CheckAllStat(cs.None) && 
                                DM.ARAY[ri.VSN1].CheckAllStat(cs.None) && 
                                DM.ARAY[ri.VSN2].CheckAllStat(cs.None) && 
                                DM.ARAY[ri.VSN3].CheckAllStat(cs.None) &&
                                DM.ARAY[ri.PSTB].CheckAllStat(cs.None) ;
                    
                //사이클
                bool isSupply =  None  && Supply && bLot && !OM.CmnOptn.bLoadStop;
                bool isPick   =  None  && Pick   && bLot && !OM.CmnOptn.bLoadStop; 
                bool isWork   =  Unkwn && PreRdy         && !OM.CmnOptn.bLoadStop && (!bLotWait || bLotMove); 
                bool isDrop   =  Empty ;
                bool isEnd    =  None  && LOT.GetNextMgz() == "" ; 

                //모르는 카세트 에러
                if( None && (  bMgzSsr1 ||  bMgzSsr2)) ER_SetErr(ei.PKG_Unknwn,"Loader have no data found, but mgz sensor detected") ;
                
                //카세트 사라짐
                if(!None && ( !bMgzSsr1 && !bMgzSsr2)) ER_SetErr(ei.PKG_Dispr  ,"Loader have data, but mgz sensor not detected") ;

                if (ER_IsErr()) return false;

                //Normal Decide Step.
                     if (isSupply) { DM.ARAY[ri.LODR].Trace(m_iPartId); Step.eSeq = sc.Supply; }
                else if (isPick  ) { DM.ARAY[ri.LODR].Trace(m_iPartId); Step.eSeq = sc.Pick  ; }
                else if (isWork  ) { DM.ARAY[ri.LODR].Trace(m_iPartId); Step.eSeq = sc.Work  ; }
                else if (isDrop  ) { DM.ARAY[ri.LODR].Trace(m_iPartId); Step.eSeq = sc.Drop  ; }
                else if (isEnd   ) { Stat.bWorkEnd = true; return true; }
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
                default         : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case sc.Idle    : return false;
                case sc.Supply  : if (!CycleSupply()) return false; break;
                case sc.Pick    : if (!CyclePick  ()) return false; break;
                case sc.Work    : if (!CycleWork  ()) return false; break;
                case sc.Drop    : if (!CycleDrop  ()) return false; break;
            }
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        //public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        //{
        //    c=0 ; r=0 ;
        //    
        //    return DM.ARAY[(int)_iId].FindFrstColLastRow( ref c, ref r , _iChip);
        //}    

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_HomeTO ,sTemp);
                Trace(sTemp);
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
            
                case 10: //홈잡을때 자재 들고 있으면 물어봐야 함 Y축 부터 잡게 되어 있음
                    IO_SetY(yi.LODR_MgzInAC , false);
                    IO_SetY(yi.LODR_MgzOutAC, false);
                    IO_SetY(yi.RAIL_FeedingAC1, false);
                    Step.iHome++;
                    return false;

                case 11:
                    CL_Move(ci.LODR_PusherFwBw, fb.Bwd);
                    Step.iHome++;
                    return false ;
            
                case 12:
                    if(!CL_Complete(ci.LODR_PusherFwBw,fb.Bwd)) return false;
                    MT_GoHome(mi.LODR_YClmp);
                    Step.iHome++;
                    return false;

                case 13:
                    if(!MT_GetHomeDone(mi.LODR_YClmp)) return false;
                    MT_GoHome(mi.LODR_ZClmp);
                    Step.iHome++;
                    return false;

                case 14:
                    if(!MT_GetHomeDone(mi.LODR_ZClmp)) return false;
                    MT_GoAbsMan(mi.LODR_YClmp,PM.GetValue(mi.LODR_YClmp,pv.LODR_YClmpWait));
                    MT_GoAbsMan(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpWait));
                    Step.iHome++;
                    return false;
                      

                case 15:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpWait)) return false;
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpWait)) return false;

                    Step.iHome = 0;
                    return true ;
            }
        }
        
        public bool CycleWait()
        {
            String sTemp;
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

            int r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.LODR_PusherFwBw,fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.LODR_PusherFwBw,fb.Bwd)) return false;
                    Step.iCycle++;
                    return false;

                case 12:
                    MoveMotr(mi.LODR_YClmp,pv.LODR_YClmpWait);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpWait)) return false;
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpWait);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpWait)) return false;
                    Step.iCycle = 0;
                    return true;

               
            }
        }

        public bool CycleSupply()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 7000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                IO_SetY(yi.LODR_MgzInAC ,false);
                IO_SetY(yi.LODR_MgzOutAC,false);
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

            bool r, c ;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    IO_SetY(yi.LODR_MgzInAC ,true );
                    IO_SetY(yi.LODR_MgzOutAC,false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 11:
                    r = m_tmDelay.OnDelay(true,6000);
                    c = IO_GetX(xi.LODR_MgzIn)      ;
                    if( r) {
                        IO_SetY(yi.LODR_MgzInAC ,false);
                        IO_SetY(yi.LODR_MgzOutAC,false);
                        ER_SetErr(ei.LODR_SupplyFail);
                        return true;
                    }
                    if(!c) return false; 
                    Step.iCycle++;
                    return false;

                case 12:
                    IO_SetY(yi.LODR_MgzInAC ,false);
                    //IO_SetY(yi.LODR_MgzOutAC,false);
                    Stat.bSupply = true;

                    Step.iCycle = 0;
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
                IO_SetY(yi.LODR_MgzInAC ,false);
                IO_SetY(yi.LODR_MgzOutAC,false);
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

            bool r, c ;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    if(IO_GetX(xi.LODR_MgzDetect1) || IO_GetX(xi.LODR_MgzDetect2))
                    {
                        ER_SetErr(ei.LODR_SupplyFail , "Loader magazine sensor is detected");
                        return true;
                    }
                    MoveCyl(ci.LODR_ClampUpDn ,fb.Bwd);
                    MoveCyl(ci.LODR_PusherFwBw,fb.Bwd);
                    IO_SetY(yi.LODR_MgzInAC ,true );
                    IO_SetY(yi.LODR_MgzOutAC,false);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.LODR_ClampUpDn ,fb.Bwd)) return false;
                    if(!CL_Complete(ci.LODR_PusherFwBw,fb.Bwd)) return false;
                    MoveMotr(mi.LODR_YClmp,pv.LODR_YClmpWork);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpWork)) return false;
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpPickFwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpPickFwd)) return false;
                    MoveMotr(mi.LODR_YClmp,pv.LODR_YClmpPick);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpPick)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    r = IO_GetX(xi.LODR_MgzDetect1) || IO_GetX(xi.LODR_MgzDetect2) ; //둘다 보는게 좋긴한데 하나로 일단 감
                    if(m_tmDelay.OnDelay(4000))
                    {
                        IO_SetY(yi.LODR_MgzInAC ,false);
                        IO_SetY(yi.LODR_MgzOutAC,false);
                        MT_GoAbsMan(mi.LODR_YClmp,pv.LODR_YClmpWork);
                        ER_SetErr(ei.LODR_SupplyFail,"The magazine sensor of the loader is not detected.");
                        return true;
                    }
                    if(!r) return false;
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpClampOn);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpClampOn)) return false;
                    MoveCyl(ci.LODR_ClampUpDn ,fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!CL_Complete(ci.LODR_ClampUpDn ,fb.Fwd)) return false;
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpPickBwd);
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpPickBwd)) return false;
                    IO_SetY(yi.LODR_MgzInAC ,false);
                    MoveMotr(mi.LODR_YClmp,pv.LODR_YClmpWork);
                    Step.iCycle++;
                    return false;

                case 19:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpWork)) return false;

                    Stat.bSupply = false;

                    //여기서는 그냥 메거진정보꺼내서 lotno에 입히기만 하고 WorkCycle에서 랏오픈.
                    string sLot = LOT.PopMgz() ; //이안에서 유저가 입력한 랏넘버에 숫자를 뒤에 입힌다.
                    
                    
                    //실제 시간은 어쩔수 없이 여기서 나우로 가져오지면 실제 SPC에서 데이터 남기는 랏스타트 시간과 약간의 딜레이가 있지만 크게 문제 없다.
                    //다만 이시점에 폴더 확인 하고 랏오픈시점에 다음날로 넘어가면 
                    //IA10010000 을 19일날 두번째 돌리기 시작 했으면 20일날 데이터가 IA10010000_0이 아닌 IA10010000_1로 남게됨.
                    
                    //메거진 2개씩 돌리면 픽2번해서 랏이 따로따로 쪼개짐.. ㅜㅠ
                    //랏넘버 입력 할때로 옮김.
                    //string sLotNo = SPC.MAP.GetLotNo(sLot) ;

                    DM.ARAY[ri.LODR].LotNo = sLot;//sLotNo ;
                    DM.ARAY[ri.LODR].SetStat(cs.Unknown);                    
                    DM.ARAY[ri.LODR].ID    = LOT.GetWorkMgzCnt().ToString() ;

                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpWorkStart);
                    Step.iCycle++;
                    return false;

                case 20:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpWorkStart)) return false;
                    
                    Step.iCycle = 0;
                    return true;

            }
        }

        public double dPos = 0;
        public bool CycleWork()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                IO_SetY(yi.LODR_MgzInAC   ,false);
                IO_SetY(yi.LODR_MgzOutAC  ,false);
                IO_SetY(yi.PREB_AirBlower ,false);
                IO_SetY(yi.RAIL_FeedingAC1,false);
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

            int r,c = 0;
            //double dPos = 0;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    dPos = (DM.ARAY[ri.LODR].GetMaxRow() - DM.ARAY[ri.LODR].FindLastRow(cs.Unknown) -1) * OM.DevInfo.dMgzPitch ;
                    double dWorkStt = PM.GetValue(mi.LODR_ZClmp,pv.ULDR_ZClmpWorkStart) + dPos;
                    if(!MT_CmprPos(mi.LODR_ZClmp,dWorkStt) && IO_GetX(xi.PREB_PkgInDetect))
                    {
                        ER_SetErr(ei.LODR_SupplyFail , "Pre Buffer In Sensor Checked");
                        return true;
                    }

                    MoveCyl(ci.LODR_ClampUpDn ,fb.Fwd);
                    MoveCyl(ci.LODR_PusherFwBw,fb.Bwd);
                    IO_SetY(yi.LODR_MgzInAC ,false);
                    IO_SetY(yi.LODR_MgzOutAC,false);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.LODR_ClampUpDn ,fb.Fwd)) return false;
                    if(!CL_Complete(ci.LODR_PusherFwBw,fb.Bwd)) return false;
                    MoveMotr(mi.LODR_YClmp,pv.LODR_YClmpWork);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpWork)) return false;
                    dPos = (DM.ARAY[ri.LODR].GetMaxRow() - DM.ARAY[ri.LODR].FindLastRow(cs.Unknown) -1) * OM.DevInfo.dMgzPitch ;
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpWorkStart,dPos);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpWorkStart,dPos)) return false;
                    MoveCyl(ci.LODR_PusherFwBw,fb.Fwd);
                    IO_SetY(yi.RAIL_FeedingAC1 , true);
                    Step.iCycle++;
                    return false;

                case 14:
                    if (IO_GetX(xi.LODR_PushOverload))
                    {
                        MoveCyl(ci.LODR_PusherFwBw,fb.Bwd);
                        ER_SetErr(ei.LODR_PushOverload);
                        return true;
                    }
                    if(!CL_Complete(ci.LODR_PusherFwBw,fb.Fwd)) return false;
                    if (!IO_GetX(xi.PREB_PkgInDetect)) //Have no strip
                    { 
                        MoveCyl(ci.LODR_PusherFwBw,fb.Bwd);
                        r = DM.ARAY[ri.LODR].FindLastRow(cs.Unknown);
                        DM.ARAY[ri.LODR].SetStat(0,r,cs.Empty);
                        Step.iCycle=20;
                        return false;
                    }

                    //요기서 살짝 씀
                    SEQ.PREB.MoveCyl(ci.PREB_StprUpDn,fb.Fwd);
                    IO_SetY(yi.PREB_AirBlower,true);

                    MoveCyl(ci.LODR_PusherFwBw,fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 20: //Have no strip case
                    if(!CL_Complete(ci.LODR_PusherFwBw,fb.Bwd)) return false;
                    IO_SetY(yi.RAIL_FeedingAC1 , false);

                    Step.iCycle=0;
                    return true;

                case 15:
                    if(!CL_Complete(ci.PREB_StprUpDn  ,fb.Fwd)) return false;
                    //IO_SetY(yi.RAIL_FeedingAC1,true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (m_tmDelay.OnDelay(4000))
                    {
                        IO_SetY(yi.RAIL_FeedingAC1,false);
                        IO_SetY(yi.PREB_AirBlower ,false);
                        ER_SetErr(ei.RAIL_FeedingFail,"Loader Pusher Feeding Fail");
                        return true;
                    }
                    if(!IO_GetX(xi.PREB_StrpDetect)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!m_tmDelay.OnDelay(100))return false ; //자꾸 튕겨서 센서밖으로 나가서.



                    IO_SetY(yi.RAIL_FeedingAC1,false);
                    IO_SetY(yi.PREB_AirBlower ,false);
                    
                    //?? 쉬프트로 하면 안됌.//DM.ShiftData(ri.LODR,ri.PREB);
                    r = DM.ARAY[ri.LODR].FindLastRow(cs.Unknown);
                    DM.ARAY[ri.LODR].SetStat(0,r,cs.Empty);
                    DM.ARAY[ri.PREB].SetStat(cs.Unknown);
                    DM.ARAY[ri.PREB].LotNo = DM.ARAY[ri.LODR].LotNo ;
                    DM.ARAY[ri.PREB].ID    = (LOT.GetWorkMgzCnt() * 100 + r).ToString(); //100자리는 메거진카운트 10자리까진 슬롯.
                    OM.CmnOptn.sLdrPreLotNo = DM.ARAY[ri.PREB].LotNo ;
                    OM.CmnOptn.sLdrPreLotId = DM.ARAY[ri.PREB].ID    ;

                    if(LOT.GetLotNo() != DM.ARAY[ri.PREB].LotNo)
                    {
                        LOT.LotOpen(DM.ARAY[ri.PREB].LotNo);
                        OM.EqpStat.iPreRsltCnts = (int [])OM.EqpStat.iRsltCnts.Clone() ;
                        System.Array.Clear(OM.EqpStat.iRsltCnts,0,OM.EqpStat.iRsltCnts.Length);
                        //OM.EqpStat.iRsltCnts.Initialize();
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!m_tmDelay.OnDelay(1000))return false ; //자꾸 튕겨서 센서밖으로 나가서.
                    if(IO_GetX(xi.PREB_StrpDetect)) 
                    {
                        Step.iCycle=0;
                        return true;
                    }
                    IO_SetY(yi.RAIL_FeedingAC1,true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 19:
                    if (!m_tmDelay.OnDelay(300))return false ; //자꾸 튕겨서 센서밖으로 나가서.
                    IO_SetY(yi.RAIL_FeedingAC1,false);

                    
                    Step.iCycle=0;
                    return true;
            }
        }

        public bool CycleDrop()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 6000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                IO_SetY(yi.LODR_MgzInAC ,false);
                IO_SetY(yi.LODR_MgzOutAC,false);
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

            int r,c = 0;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;


                case 10:
                    MoveCyl(ci.LODR_ClampUpDn ,fb.Fwd);
                    MoveCyl(ci.LODR_PusherFwBw,fb.Bwd);
                    IO_SetY(yi.LODR_MgzOutAC,true );
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.LODR_ClampUpDn ,fb.Fwd)) return false;
                    if(!CL_Complete(ci.LODR_PusherFwBw,fb.Bwd)) return false;
                    MoveMotr(mi.LODR_YClmp,pv.LODR_YClmpWork);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpWork)) return false;
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpPlaceFwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpPlaceFwd)) return false;

                    if (IO_GetX(xi.LODR_MgzOutFull))
                    {
                        IO_SetY(yi.LODR_MgzOutAC,false);
                        ER_SetErr(ei.PRT_MgzFull,"Loader Magazine Full");
                        return true;
                    }

                    IO_SetY(yi.LODR_MgzOutAC,false);

                    MoveMotr(mi.LODR_YClmp,pv.LODR_YClmpPlace);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpPlace)) return false;
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpClampOff);
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpClampOff)) return false;
                    MoveCyl(ci.LODR_ClampUpDn ,fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.LODR_ClampUpDn ,fb.Bwd)) return false;
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpPlaceBwd);
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.LODR_ZClmp,pv.LODR_ZClmpPlaceBwd)) return false;
                    IO_SetY(yi.LODR_MgzOutAC,true);
                    MoveMotr(mi.LODR_YClmp,pv.LODR_YClmpWork);
                    
                    DM.ARAY[ri.LODR].ClearMap();

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18:
                    
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iLdrOutDelay)) return false;
                    IO_SetY(yi.LODR_MgzOutAC,false);
                    Step.iCycle++;
                    return false ;

                case 19:
                    if(!MT_GetStopPos(mi.LODR_YClmp,pv.LODR_YClmpWork)) return false;


                    
                    MoveMotr(mi.LODR_ZClmp,pv.LODR_ZClmpPickFwd);
                    Step.iCycle = 0;
                    return true;             
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            bool bMov = !MT_GetStop(mi.LODR_ZClmp) || !MT_GetStop(mi.LODR_YClmp) ;

            if(_eActr == ci.LODR_PusherFwBw)
            {
                if(bMov) {sMsg = "Loader motor is moving"; bRet = false;}
            }
            else if (_eActr == ci.LODR_ClampUpDn)
            {
                if (!SEQ._bRun && Step.iCycle == 0)
                {
                    if(_eFwd == fb.Bwd && (IO_GetX(xi.LODR_MgzDetect1) || IO_GetX(xi.LODR_MgzDetect2)) )
                    {
                        if (Log.ShowMessageModal("Confirm", "Mgz sensor is detected , Open the Mgz?") != System.Windows.Forms.DialogResult.Yes) return false;
                    }
                }
            }

            else if(!_bChecked){
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

            bool bZPickFwd  = MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpPickFwd )) ;
            bool bZPlaceFwd = MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpPlaceFwd)) ;
            bool bZPickBwd  = MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpPickBwd )) ;
            bool bZPlaceBwd = MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpPlaceBwd)) ;

            bool bYWork     = MT_CmprPos(mi.LODR_YClmp,PM.GetValue(mi.LODR_YClmp,pv.LODR_YClmpWork)) ;
            bool bYWait     = MT_CmprPos(mi.LODR_YClmp,PM.GetValue(mi.LODR_YClmp,pv.LODR_YClmpWait)) ;
            bool bYNotW     = !bYWork && !bYWait ;

            bool bYFwd1     = MT_CmprPos(mi.LODR_YClmp,PM.GetValue(mi.LODR_YClmp,pv.LODR_YClmpPick )) &&
                             (MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpPickBwd )) ||
                              MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpPickFwd )) ||
                              MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpClampOn )) );

            bool bYFwd2     = MT_CmprPos(mi.LODR_YClmp,PM.GetValue(mi.LODR_YClmp,pv.LODR_YClmpPlace )) &&
                             (MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpPlaceBwd )) ||
                              MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpPlaceFwd )) ||
                              MT_CmprPos(mi.LODR_ZClmp,PM.GetValue(mi.LODR_ZClmp,pv.LODR_ZClmpClampOff )) );

            if(!CL_Complete(ci.LODR_PusherFwBw,fb.Bwd)) { sMsg = "Need to Loader Pusher Bwd Position"; bRet = false;} 
            if(IO_GetX(xi.PREB_PkgInDetect))            { sMsg = "Pre Buffer In Sensor Detected"; bRet = false;} 

            if (_eMotr == mi.LODR_YClmp)
            {
                if(_ePstn == pv.LODR_YClmpPick ) { if(!bZPickFwd  ) { sMsg = "Need to Loor Z Pick Fwd Position" ; bRet = false;} }
                if(_ePstn == pv.LODR_YClmpPlace) { if(!bZPlaceFwd ) { sMsg = "Need to Loor Z Place Fwd Position"; bRet = false;} }
                if(_ePstn == pv.LODR_YClmpWait ) { if(!bYWork && !bZPickBwd && !bZPlaceBwd) { sMsg = "Need to Loor Z Pick or Place Bwd Position"; bRet = false;} }
                if(_ePstn == pv.LODR_YClmpWork ) { if(!bYWait && !bZPickBwd && !bZPlaceBwd) { sMsg = "Need to Loor Z Pick or Place Bwd Position"; bRet = false;} }
            }
            else if (_eMotr == mi.LODR_ZClmp)
            {
                if(_ePstn == pv.LODR_ZClmpPickBwd  ) {if(!bYFwd1) { sMsg = "Need to Loor Y Z Pick Position" ; bRet = false;} }
                if(_ePstn == pv.LODR_ZClmpPickFwd  ) {if( bYNotW && !bYFwd1) { sMsg = "Need to Loor Wait or Work Position"; bRet = false;} }
                if(_ePstn == pv.LODR_ZClmpClampOn  ) {if(!bYFwd1) { sMsg = "Need to Loor Y Z Pick Position" ; bRet = false;} }
                                                     
                if(_ePstn == pv.LODR_ZClmpPlaceBwd ) {if(!bYFwd2) { sMsg = "Need to Loor Y Z Place Position"; bRet = false;} }
                if(_ePstn == pv.LODR_ZClmpPlaceFwd ) {if( bYNotW && !bYFwd2) { sMsg = "Need to Loor Wait or Work Position"; bRet = false;} }
                if(_ePstn == pv.LODR_ZClmpClampOff ) {if(!bYFwd2) { sMsg = "Need to Loor Y Z Place Position"; bRet = false;} }
                                                     
                if(_ePstn == pv.LODR_ZClmpWait     ) {if( bYNotW) { sMsg = "Need to Loor Wait or Work Position"; bRet = false;} }
                if(_ePstn == pv.LODR_ZClmpWorkStart) {if( bYNotW) { sMsg = "Need to Loor Wait or Work Position"; bRet = false;} }

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

            if(!CL_Complete(ci.LODR_PusherFwBw,fb.Bwd)) { sMsg = "Need to Loor Pusher Bwd Position"; bRet = false;} 
            if(IO_GetX(xi.PREB_PkgInDetect))            { sMsg = "Rail In Sensor Detected"; bRet = false;} 

            if (_eMotr == mi.LODR_YClmp)
            {
                if (_eDir == EN_JOG_DIRECTION.Neg)
                {
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
                        
                    }
                    else if(_eType == EN_UNIT_TYPE.utMove)
                    {

                    }
                }
                //Fwd
                else
                {
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {

                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
                    {

                    }
                }
             
            }
            else if (_eMotr == mi.LODR_ZClmp)
            {
                //Bwd
                if (_eDir == EN_JOG_DIRECTION.Neg)
                {
                    if (_eType == EN_UNIT_TYPE.utJog)
                    {
                
                    }
                    else if (_eType == EN_UNIT_TYPE.utMove)
                    {
                
                    }
                }
                //Fwd
                else 
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
            if (!MT_GetStop(mi.LODR_YClmp)) return false;
            if (!MT_GetStop(mi.LODR_ZClmp)) return false;

            if (!CL_Complete(ci.LODR_PusherFwBw)) return false;
            if (!CL_Complete(ci.LODR_ClampUpDn )) return false;

            return true;
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
