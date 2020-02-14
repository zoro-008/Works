using System;
using COMMON;
using SML;
using System.Runtime.CompilerServices;

namespace Machine
{
    //PULD == PreUnloader
    public class Unloader : Part
    {
               //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
            public bool bReqStop   ;
            public void Clear()
            {
                bWorkEnd    = false;
                bReqStop    = false;
                bSupply     = false;
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
            public int  iHome;
            public int  iToStart;
            public sc   eSeq;
            public int  iCycle;
            public int  iToStop;
            public sc   eLastSeq;
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

        int    iWorkCnt ;
        public double dUldrStartTime = 0.0;
        public double dUldrEndTime = 0.0;

        public Unloader(int _iPartId = 0)
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

            int a = 0; 
            a++;

        }
        override public bool ToStopCon() //스탑을 하기 위한 조건을 보는 함수.
        {
            Stat.bReqStop = true;
            //if (!SEQ.InspectLightGrid()) Stat.bSafetyStop = true;
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
                default: Step.iToStart = 0;
                    return true;

                case 10:
                    CL_Move(ci.PSTB_PusherFwBw,fb.Bwd);
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,false);
                    Step.iToStart++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.PSTB_PusherFwBw,fb.Bwd)) return false;
                    Step.iToStart = 0;
                    return true ;
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
                default: Step.iToStop = 0;
                    return true;

                case 10:
                    //CL_Move(ci.PSTB_PusherFwBw,fb.Bwd);
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,false);
                    Step.iToStop++;
                    return false;

                case 11:
                    //if(!CL_Complete(ci.PSTB_PusherFwBw,fb.Bwd)) return false;
                    Step.iToStop = 0;
                    return true;
            }
        }

        override public int GetHomeStep   () { return Step.iHome    ; } override public int GetPreHomeStep   () { return PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return Step.iToStart ; } override public int GetPreToStartStep() { return PreStep.iToStart ; }
        override public int GetSeqStep    () { return (int)Step.eSeq; } override public int GetPreSeqStep    () { return (int)PreStep.eSeq; }
        override public int GetCycleStep  () { return Step.iCycle   ; } override public int GetPreCycleStep  () { return PreStep.iCycle   ; } override public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; }
        override public int GetToStopStep () { return Step.iToStop  ; } override public int GetPreToStopStep () { return PreStep.iToStop  ; }

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

                //자재 상태
                bool None    =  DM.ARAY[ri.ULDR].CheckAllStat(cs.None   ) ;
                bool Work    =  DM.ARAY[ri.ULDR].CheckAllStat(cs.Work   ) ;
                bool Empty   =  DM.ARAY[ri.ULDR].GetCntStat  (cs.Empty  ) > 0 ;
                bool Strip   = !DM.ARAY[ri.PREB].CheckAllStat(cs.None   ) || !DM.ARAY[ri.VSN1].CheckAllStat(cs.None   ) || !DM.ARAY[ri.VSN2].CheckAllStat(cs.None   ) || 
                               !DM.ARAY[ri.VSN3].CheckAllStat(cs.None   ) || !DM.ARAY[ri.PSTB].CheckAllStat(cs.None   ) ;//|| !DM.ARAY[ri.ULDR].CheckAllStat(cs.None   ) ;
                bool NonePsb  = DM.ARAY[ri.PSTB].CheckAllStat(cs.None   ) ;
                bool bMgzSsr1 = IO_GetX(xi.ULDR_MgzDetect1) ;
                bool bMgzSsr2 = IO_GetX(xi.ULDR_MgzDetect2) ;

                string sLastLotNo = "";//DM.ARAY[ri.PSTB].LotNo;
                     if(!DM.ARAY[ri.PSTB].CheckAllStat(cs.None)) sLastLotNo = DM.ARAY[ri.PSTB].LotNo;
                else if(!DM.ARAY[ri.VSN3].CheckAllStat(cs.None)) sLastLotNo = DM.ARAY[ri.VSN3].LotNo;
                else if(!DM.ARAY[ri.VSN2].CheckAllStat(cs.None)) sLastLotNo = DM.ARAY[ri.VSN2].LotNo;
                else if(!DM.ARAY[ri.VSN1].CheckAllStat(cs.None)) sLastLotNo = DM.ARAY[ri.VSN1].LotNo;
                else if(!DM.ARAY[ri.PREB].CheckAllStat(cs.None)) sLastLotNo = DM.ARAY[ri.PREB].LotNo;
                else if(!DM.ARAY[ri.LODR].CheckAllStat(cs.None)) sLastLotNo = DM.ARAY[ri.LODR].LotNo;
                else                                             sLastLotNo = LOT.GetNextMgz()      ;
                              

                //조건
                double dPos    = DM.ARAY[ri.ULDR].FindFrstRow(cs.Empty) * OM.DevInfo.dMgzPitch ;
                bool   WorkPos = MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpWorkStart) - dPos);
                bool   LotCng  = DM.ARAY[ri.ULDR].LotNo != "" && DM.ARAY[ri.ULDR].LotNo != sLastLotNo ;//&& !NonePsb ;
                bool   Supply  = (!IO_GetX(xi.ULDR_MgzIn) && !Stat.bSupply) ; //매거진에 구멍땜에 안들어와있을경우 반영 , 매거진 있을때라고 생각

                //사이클
                bool isSupply  = None  &&  Strip &&  Supply;
                bool isPick    = None  &&  Strip && !Supply;
                bool isWork    = Empty && !WorkPos ;
                bool isDrop   = !None  && (Work || LotCng || (!Strip && SEQ.LODR.GetSeqStep() == 0)) ; 
                bool isEnd    =  None  && !Strip && SEQ.LODR.GetSeqStep() == 0 ; 

                //모르는 카세트 에러
                if(!isPick){
                    if(None && (bMgzSsr1 || bMgzSsr2)) ER_SetErr(ei.PKG_Unknwn,"Unloader have no data found, but mgz sensor detected") ;
                }
                //카세트 사라짐
                if(!isDrop){
                    if(!None && ( !bMgzSsr1 && !bMgzSsr2)) ER_SetErr(ei.PKG_Dispr,"Unloader have data, but mgz sensor not detected") ;
                }
                //카세트 필요
                if(  None && !IO_GetX(xi.ULDR_MgzIn) && Stat.bSupply && !NonePsb) ER_SetErr(ei.PRT_NeedMgz,"Unloader need to magazine") ;
                //카세트 꽉참
                //if(  None && !IO_GetX(xi.ULDR_MgzIn) && Stat.bSupply && !NonePsb) ER_SetErr(ei.PRT_NeedMgz,"Unloader need to magazine") ;


                if (ER_IsErr())
                {
                    return false;
                }

                //Normal Decide Step.
                     if (isSupply ) { DM.ARAY[ri.ULDR].Trace(m_iPartId); Step.eSeq = sc.Supply; }
                else if (isPick   ) { DM.ARAY[ri.ULDR].Trace(m_iPartId); Step.eSeq = sc.Pick  ; }
                else if (isWork   ) { DM.ARAY[ri.ULDR].Trace(m_iPartId); Step.eSeq = sc.Work  ; }
                else if (isDrop   ) { DM.ARAY[ri.ULDR].Trace(m_iPartId); Step.eSeq = sc.Drop  ; }
                else if (isEnd    ) { Stat.bWorkEnd = true; return true; }

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
                case sc.Supply  : if (!CycleSupply  ()) return false; break;
                case sc.Pick    : if (!CyclePick    ()) return false; break;
                case sc.Work    : if (!CycleWork    ()) return false; break;
                case sc.Drop    : if (!CycleDrop    ()) return false; break;
                
            }       
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;                                    
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            r = 0 ;
            c = 0 ;
            DM.ARAY[_iId].FindFrstColLastRow( ref c, ref r , _iChip);
            return (c >= 0 && r >= 0) ? true : false;
        }       

        public bool CycleHome()
        {
            string sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
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
                    if(Step.iHome != PreStep.iHome) Trace(sTemp);
                    return true ;
            
                case 10:
                    CL_Move(ci.PSTB_PusherFwBw,fb.Bwd);
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,false);
                    Step.iHome++;
                    return false ;

                case 11:
                    if(!CL_Move(ci.PSTB_PusherFwBw,fb.Bwd)) return false;
                    MT_GoHome(mi.ULDR_YClmp);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetHomeDone(mi.ULDR_YClmp)) return false;
                    MT_GoHome(mi.ULDR_ZClmp);
                    Step.iHome++;
                    return false ;

                case 13:
                    if(!MT_GetHomeDone(mi.ULDR_ZClmp)) return false;
                    MT_GoAbsMan(mi.ULDR_YClmp,PM.GetValue(mi.ULDR_YClmp,pv.ULDR_YClmpWait));
                    MT_GoAbsMan(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpWait));
                    Step.iHome++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpWait)) return false;
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpWait)) return false;


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

                //초기화
                case 10:
                    CL_Move(ci.PSTB_PusherFwBw,fb.Bwd);
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,false);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Move(ci.PSTB_PusherFwBw,fb.Bwd)) return false;
                    MoveMotr(mi.ULDR_YClmp,pv.ULDR_YClmpWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpWait)) return false;
                    MoveMotr(mi.ULDR_ZClmp,pv.ULDR_ZClmpWait);
                    Step.iCycle++;
                    return false;
                
                case 13:
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpWait)) return false;
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
                IO_SetY(yi.ULDR_MgzInAC ,false);
                IO_SetY(yi.ULDR_MgzOutAC,false);
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
            bool bTemp1 , bTemp2 = false ;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    IO_SetY(yi.ULDR_MgzInAC ,true );
                    IO_SetY(yi.ULDR_MgzOutAC,false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 11:
                    if (m_tmDelay.OnDelay(true,6000))
                    {
                        IO_SetY(yi.ULDR_MgzInAC ,false);
                        IO_SetY(yi.ULDR_MgzOutAC,false);
                        ER_SetErr(ei.PRT_NeedMgz);
                        return true;
                    }
                    if(!IO_GetX(xi.ULDR_MgzIn)) return false;
                    Step.iCycle++;
                    return false;

                case 12:
                    IO_SetY(yi.ULDR_MgzInAC,false);
                    Stat.bSupply = true;
                    Step.iCycle++;
                    return false;

                case 13:
                        
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
                IO_SetY(yi.ULDR_MgzInAC ,false);
                IO_SetY(yi.ULDR_MgzOutAC,false);
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
                    if(IO_GetX(xi.ULDR_MgzDetect1) || IO_GetX(xi.ULDR_MgzDetect2))
                    {
                        ER_SetErr(ei.ULDR_SupplyFail, "Unloader magazine sensor is detected");
                        return true;
                    }
                    MoveCyl(ci.ULDR_ClampUpDn,fb.Bwd);
                    MoveMotr(mi.ULDR_YClmp,pv.ULDR_YClmpWork);
                    IO_SetY(yi.ULDR_MgzInAC ,true );
                    IO_SetY(yi.ULDR_MgzOutAC,false);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.ULDR_ClampUpDn,fb.Bwd)) return false;
                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpWork)) return false;
                    MoveMotr(mi.ULDR_ZClmp,pv.ULDR_ZClmpPickFwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpPickFwd)) return false;
                    MoveMotr(mi.ULDR_YClmp,pv.ULDR_YClmpPick);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpPick)) return false;
                    r = IO_GetX(xi.ULDR_MgzDetect1) || IO_GetX(xi.ULDR_MgzDetect2) ; //둘다 보는게 좋긴한데 하나로 일단 감
                    if(m_tmDelay.OnDelay(4000))
                    {
                        IO_SetY(yi.ULDR_MgzInAC ,false);
                        IO_SetY(yi.ULDR_MgzOutAC,false);
                        MT_GoAbsMan(mi.ULDR_YClmp,pv.ULDR_YClmpWork);
                        ER_SetErr(ei.ULDR_SupplyFail,"The magazine sensor of the unloader is not detected.");
                        return true;
                    }
                    if(!r) return false;

                    MoveMotr(mi.ULDR_ZClmp,pv.ULDR_ZClmpClampOn);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpClampOn)) return false;
                    MoveCyl(ci.ULDR_ClampUpDn,fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!CL_Complete(ci.ULDR_ClampUpDn,fb.Fwd)) return false;
                    MoveMotr(mi.ULDR_ZClmp,pv.ULDR_ZClmpPickBwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpPickBwd)) return false;

                    DM.ARAY[ri.ULDR].LotNo = "";
                    DM.ARAY[ri.ULDR].SetStat(cs.Empty);

                    MoveMotr(mi.ULDR_YClmp,pv.ULDR_YClmpWork);
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,false);
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpWork)) return false;

                    Step.iCycle = 0;
                    return true;
            }
        }

        private double dPos = 0;
        public bool CycleWork()
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

            //bool r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveMotr(mi.ULDR_YClmp,pv.ULDR_YClmpWork);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpWork)) return false;
                    dPos = DM.ARAY[ri.ULDR].FindFrstRow(cs.Empty) * OM.DevInfo.dMgzPitch ;
                    MoveMotr(mi.ULDR_ZClmp,pv.ULDR_ZClmpWorkStart,-dPos);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpWorkStart,-dPos)) return false;

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleDrop()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 6000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                IO_SetY(yi.ULDR_MgzInAC ,false);
                IO_SetY(yi.ULDR_MgzOutAC,false);
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

            //bool r, c = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.ULDR_ClampUpDn,fb.Fwd);
                    MoveMotr(mi.ULDR_YClmp,pv.ULDR_YClmpWork);
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,true );
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.ULDR_ClampUpDn,fb.Fwd)) return false;
                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpWork)) return false;
                    MoveMotr(mi.ULDR_ZClmp,pv.ULDR_ZClmpPlaceFwd);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpPlaceFwd)) return false;
                    MoveMotr(mi.ULDR_YClmp,pv.ULDR_YClmpPlace);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpPlace)) return false;
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!m_tmDelay.OnDelay(true,500)) return false;
                    MoveMotr(mi.ULDR_ZClmp,pv.ULDR_ZClmpClampOff);
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpClampOff)) return false;
                    MoveCyl(ci.ULDR_ClampUpDn,fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!CL_Complete(ci.ULDR_ClampUpDn,fb.Bwd)) return false;
                    MoveMotr(mi.ULDR_ZClmp,pv.ULDR_ZClmpPlaceBwd);
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStopPos(mi.ULDR_ZClmp,pv.ULDR_ZClmpPlaceBwd)) return false;
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,true );
                    //DM.ARAY[ri.ULDR].SetStat(cs.None);
                    DM.ARAY[ri.ULDR].ClearMap();

                    MoveMotr(mi.ULDR_YClmp,pv.ULDR_YClmpWork);

                    m_tmDelay.Clear();
                    
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iUdrOutDelay)) return false;
                    IO_SetY(yi.ULDR_MgzInAC ,false);
                    IO_SetY(yi.ULDR_MgzOutAC,false);

                    Step.iCycle++;
                    return false;

                case 19: 

                    if(!MT_GetStopPos(mi.ULDR_YClmp,pv.ULDR_YClmpWork)) return false;
                    

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.ULDR_ClampUpDn)
            {
                if (!SEQ._bRun && Step.iCycle == 0)
                {
                    if(_eFwd == fb.Bwd && (IO_GetX(xi.ULDR_MgzDetect1) || IO_GetX(xi.ULDR_MgzDetect2)) )
                    {
                        if (Log.ShowMessageModal("Confirm", "Mgz sensor is detected , Open the Mgz?") != System.Windows.Forms.DialogResult.Yes) return false;
                    }
                }
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

        public bool CheckSafe(mi _eMotr, pv _ePstn ,  double _dOfsPos=0)
        {
            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";

            bool bZPickFwd  = MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpPickFwd )) ;
            bool bZPlaceFwd = MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpPlaceFwd)) ;
            bool bZPickBwd  = MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpPickBwd )) ;
            bool bZPlaceBwd = MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpPlaceBwd)) ;

            bool bYWork     = MT_CmprPos(mi.ULDR_YClmp,PM.GetValue(mi.ULDR_YClmp,pv.ULDR_YClmpWork)) ;
            bool bYWait     = MT_CmprPos(mi.ULDR_YClmp,PM.GetValue(mi.ULDR_YClmp,pv.ULDR_YClmpWait)) ;
            bool bYNotW     = !bYWork && !bYWait ;

            bool bYFwd1     = MT_CmprPos(mi.ULDR_YClmp,PM.GetValue(mi.ULDR_YClmp,pv.ULDR_YClmpPick )) &&
                             (MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpPickBwd )) ||
                              MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpPickFwd )) ||
                              MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpClampOn )) );

            bool bYFwd2     = MT_CmprPos(mi.ULDR_YClmp,PM.GetValue(mi.ULDR_YClmp,pv.ULDR_YClmpPlace )) &&
                             (MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpPlaceBwd )) ||
                              MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpPlaceFwd )) ||
                              MT_CmprPos(mi.ULDR_ZClmp,PM.GetValue(mi.ULDR_ZClmp,pv.ULDR_ZClmpClampOff )) );

            if(!CL_Complete(ci.PSTB_PusherFwBw,fb.Bwd)) { sMsg = "Need to Out Pusher Bwd Position"; bRet = false;} 
            if(IO_GetX(xi.PSTB_PkgDetect2))             { sMsg = "Out Rail Out Sensor Detected"; bRet = false;} 

            if (_eMotr == mi.ULDR_YClmp)
            {
                if(_ePstn == pv.ULDR_YClmpPick ) { if(!bZPickFwd  ) { sMsg = "Need to UnLdr Z Pick Fwd Position" ; bRet = false;} }
                if(_ePstn == pv.ULDR_YClmpPlace) { if(!bZPlaceFwd ) { sMsg = "Need to UnLdr Z Place Fwd Position"; bRet = false;} }
                if(_ePstn == pv.ULDR_YClmpWait ) { if(!bYWork && !bZPickBwd && !bZPlaceBwd) { sMsg = "Need to UnLdr Z Pick or Place Bwd Position"; bRet = false;} }
                if(_ePstn == pv.ULDR_YClmpWork ) { if(!bYWait && !bZPickBwd && !bZPlaceBwd) { sMsg = "Need to UnLdr Z Pick or Place Bwd Position"; bRet = false;} }
            }
            else if (_eMotr == mi.ULDR_ZClmp)
            {
                if(_ePstn == pv.ULDR_ZClmpPickBwd  ) {if(!bYFwd1) { sMsg = "Need to UnLdr Y Z Pick Position" ; bRet = false;} }
                if(_ePstn == pv.ULDR_ZClmpPickFwd  ) {if( bYNotW && !bYFwd1) { sMsg = "Need to UnLdr Y Wait or Work Position"; bRet = false;} }
                if(_ePstn == pv.ULDR_ZClmpClampOn  ) {if(!bYFwd1) { sMsg = "Need to UnLdr Y Z Pick Position" ; bRet = false;} }
                                                     
                if(_ePstn == pv.ULDR_ZClmpPlaceBwd ) {if(!bYFwd2) { sMsg = "Need to UnLdr Y Z Place Position"; bRet = false;} }
                if(_ePstn == pv.ULDR_ZClmpPlaceFwd ) {if( bYNotW && !bYFwd2) { sMsg = "Need to UnLdr Y Wait or Work Position"; bRet = false;} }
                if(_ePstn == pv.ULDR_ZClmpClampOff ) {if(!bYFwd2) { sMsg = "Need to UnLdr Y Z Place Position"; bRet = false;} }
                                                     
                if(_ePstn == pv.ULDR_ZClmpWait     ) {if( bYNotW) { sMsg = "Need to UnLdr Y Wait or Work Position"; bRet = false;} }
                if(_ePstn == pv.ULDR_ZClmpWorkStart) {if( bYNotW) { sMsg = "Need to UnLdr Y Wait or Work Position"; bRet = false;} }

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
                if (Step.eSeq == 0) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
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

            if (_eMotr == mi.ULDR_YClmp)
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
            else if (_eMotr == mi.ULDR_ZClmp)
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
            if (!MT_GetStop(mi.ULDR_YClmp)) return false;
            if (!MT_GetStop(mi.ULDR_ZClmp)) return false;

            if (!CL_Complete(ci.ULDR_ClampUpDn)) return false;

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
