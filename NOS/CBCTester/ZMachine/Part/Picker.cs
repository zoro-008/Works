using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    public class Picker : Part
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
            PickLdr    ,
            Shake      ,
            PlceSut    ,
            Barcode    ,
            PickSut    ,
            PlceLdr    ,
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

        public Picker(int _iPartId = 0)
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
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake);
                    Step.iToStart++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake)) return false ;
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
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake);
                    Step.iToStop++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake)) return false ;
                    Step.iToStop++;
                    return false;

                case 12:
                    MoveCyl(ci.PKR_PkrRrFt, fb.Fwd);
                    MoveCyl(ci.PKR_PkrShakeUpDn, fb.Bwd);
                    Step.iToStop++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.PKR_PkrRrFt , fb.Fwd))return false ;
                    if (!CL_Complete(ci.PKR_PkrShakeUpDn, fb.Bwd)) return false;
                    Step.iToStop++;
                    return false;
                
                case 14: 

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
            if(OM.CmnOptn.bAutoQc) return AutorunQC();
            else                   return AutorunNm();
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        public bool AutorunNm() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;
                   
                                  //이거 타이밍 문제 있어서 짜증남. 미리 가면 실린지 썩 할때 챔버 필링하고 있어. 니들을 못닦음.
                bool isCleaning = /*DM.ARAY[ri.CHA].CheckAllStat(cs.Clean  ) ||*/ DM.ARAY[ri.CHA].CheckAllStat(cs.None);
                bool isNone     = DM.ARAY[ri.SYR].CheckAllStat(cs.None   ) && DM.ARAY[ri.SUT].CheckAllStat(cs.None   );
                bool isWorkExist = !DM.ARAY[ri.CHA].CheckAllStat(cs.None) || !DM.ARAY[ri.SYR].CheckAllStat(cs.None) || !DM.ARAY[ri.SUT].CheckAllStat(cs.None) ;

                //사이클
                bool isPickLdr =  DM.ARAY[ri.LDR].CheckAllStat(cs.Shake  ) && DM.ARAY[ri.PKR].CheckAllStat(cs.None   ) && ((isCleaning && isNone) || OM.CmnOptn.bNotUseInsp) &&(!isWorkExist || !OM.EqpStat.bWorkOneStop) ;
                bool isShake   =  DM.ARAY[ri.PKR].CheckAllStat(cs.Shake  ) ;
                bool isPlceSut =  DM.ARAY[ri.PKR].CheckAllStat(cs.Barcode) && DM.ARAY[ri.SUT].CheckAllStat(cs.None   );
                bool isBarcode =  DM.ARAY[ri.SUT].CheckAllStat(cs.Barcode) ;
                bool isPickSut =  DM.ARAY[ri.PKR].CheckAllStat(cs.None   ) && DM.ARAY[ri.SUT].CheckAllStat(cs.WorkEnd);
                bool isPlceLdr =  DM.ARAY[ri.PKR].CheckAllStat(cs.WorkEnd) && DM.ARAY[ri.LDR].CheckAllStat(cs.Mask   );
                bool isWorkEnd =  DM.ARAY[ri.PKR].CheckAllStat(cs.None   ) && DM.ARAY[ri.SUT].CheckAllStat(cs.None   );

                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.
                     if (isPlceLdr) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.PlceLdr ; }
                else if (isPickSut) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.PickSut ; }
                else if (isBarcode) { DM.ARAY[ri.SUT].Trace(m_sPartName); Step.eSeq = sc.Barcode ; }
                else if (isPlceSut) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.PlceSut ; }
                else if (isShake  ) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.Shake   ; }
                else if (isPickLdr) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.PickLdr ; }
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
                case (sc.Idle   ): return false;
                case (sc.PickLdr): if (!CyclePickLdr()) return false; break;
                case (sc.Shake  ): if (!CycleShake  ()) return false; break;
                case (sc.PlceSut): if (!CyclePlceSut()) return false; break;
                case (sc.Barcode): if (!CycleBarcode()) return false; break;
                case (sc.PickSut): if (!CyclePickSut()) return false; break;
                case (sc.PlceLdr): if (!CyclePlceLdr()) return false; break;

            }
            Trace(sCycle+" End");
            Log.TraceListView(this.ToString() + sCycle + " 동작 종료");
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }

        public bool AutorunQC() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                    
                //사이클
                bool isShake   =  DM.ARAY[ri.PKR].CheckAllStat(cs.Shake  ) ;
                bool isPlceSut =  DM.ARAY[ri.PKR].CheckAllStat(cs.Barcode) && DM.ARAY[ri.SUT].CheckAllStat(cs.None   );
                bool isBarcode =  DM.ARAY[ri.SUT].CheckAllStat(cs.Barcode) ;
                bool isPickSut =  DM.ARAY[ri.PKR].CheckAllStat(cs.None   ) && DM.ARAY[ri.SUT].CheckAllStat(cs.Shake  );
                bool isWorkEnd =  DM.ARAY[ri.PKR].CheckAllStat(cs.None   ) && DM.ARAY[ri.SUT].CheckAllStat(cs.None   );

                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.
                     
                     if (isShake  ) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.Shake   ; }
                else if (isPlceSut) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.PlceSut ; }
                else if (isBarcode) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.Barcode ; }
                else if (isPickSut) { DM.ARAY[ri.PKR].Trace(m_sPartName); Step.eSeq = sc.PickSut ; }
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
                default          : Trace("default End"); Step.eSeq = sc.Idle; return false;
                case (sc.Idle   ): return false;
                case (sc.Shake  ): if (!CycleShake  ()) return false; break;
                case (sc.PlceSut): if (!CyclePlceSut()) return false; break;
                case (sc.Barcode): if (!CycleBarcode()) return false; break;
                case (sc.PickSut): if (!CyclePickSut()) return false; break;
                

            }
            Trace(sCycle+" End"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }

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
                    MT_SetServo(mi.PKR_ZPckr, true );
                    MT_SetServo(mi.PKR_YSutl, true );

                    MT_GoHome(mi.PKR_ZPckr);
                    Step.iHome++;
                    return false;

                case 11: 
                    if(!MT_GetHomeDone(mi.PKR_ZPckr))return false ;
                    MT_GoHome(mi.PKR_YSutl);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetHomeDone(mi.PKR_YSutl))return false ;
                    MT_GoAbsMan(mi.PKR_ZPckr , pv.PKR_ZPckrShake);
                    MT_GoAbsMan(mi.PKR_YSutl , pv.PKR_YSutlBuffer);

                    Step.iHome++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.PKR_ZPckr, pv.PKR_ZPckrShake )) return false ;
                    if(!MT_GetStopPos(mi.PKR_YSutl, pv.PKR_YSutlBuffer)) return false ;

                    CL_Move(ci.PKR_PkrShakeUpDn , fb.Bwd);
                    Step.iHome++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.PKR_PkrShakeUpDn , fb.Bwd)) return false ;
                    
                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CyclePickLdr()
        {
            string sTemp;
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
                    MoveCyl(ci.PKR_PkrClampClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.PKR_PkrClampClOp , fb.Bwd))return false ;

                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake );
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;
                    MoveCyl(ci.PKR_PkrRrFt      , fb.Bwd);
                    MoveCyl(ci.PKR_PkrShakeUpDn , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.PKR_PkrRrFt      , fb.Bwd))return false ;
                    if(!CL_Complete(ci.PKR_PkrShakeUpDn , fb.Bwd))return false ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrRackPick );
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrRackPick))return false ;
                    MoveCyl(ci.PKR_PkrClampClOp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.PKR_PkrClampClOp , fb.Fwd))return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;
                    
                case 16: 
                    if(!m_tmDelay.OnDelay(100)) return false ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake );
                    MoveMotr(mi.PKR_YSutl , pv.PKR_YSutlPicker);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;

                    DM.ARAY[ri.LDR].SetStat(cs.Mask );
                    DM.ARAY[ri.PKR].SetStat(cs.Shake);
                   
                    //바코드 찍기전엔 ?로 세팅 하여 SPC.LOT.Update에서 랏오픈만 시키고 바코드 찍으면 LotNo갱신 형태로 구성. 
                    //타이밍 문제 발생해서 일단 셔틀에 ID로 매겨 놓고 실린지 CycleSuck할때 바코드로 옮겨서 그때 랏오픈하게 한다. 그리고 
                    //OM.EqpStat.sBloodID = "?" ; 

                    MoveCyl(ci.PKR_PkrRrFt , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!CL_Complete(ci.PKR_PkrRrFt , fb.Bwd))return false ;
                    if (!MoveMotr(mi.PKR_YSutl, pv.PKR_YSutlPicker)) return false;
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        int iCrntShakeCnt = 0 ;
        public bool CycleShake()
        {
            string sTemp;
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
                    iCrntShakeCnt = 0 ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake );
                    
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;                    
                    MoveCyl(ci.PKR_PkrRrFt, fb.Fwd);
                    Step.iCycle++;
                    return false ;

                //밑에서 씀.
                case 12:
                    if (!CL_Complete(ci.PKR_PkrRrFt, fb.Fwd)) return false;
                    Step.iCycle++;
                    return false;

                case 13:
                    MoveCyl(ci.PKR_PkrShakeUpDn , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!CL_Complete(ci.PKR_PkrShakeUpDn , fb.Fwd))return false ;
                    MoveCyl(ci.PKR_PkrShakeUpDn , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.PKR_PkrShakeUpDn , fb.Bwd))return false ;
                    iCrntShakeCnt++;
                    if(iCrntShakeCnt < OM.DevInfo.iShakeCnt)
                    {
                        Step.iCycle = 12 ;
                        return false ;
                    }
                    
                    DM.ARAY[ri.PKR].SetStat(cs.Barcode);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePlceSut()
        {
            string sTemp;
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
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake );
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;
                    Step.iCycle++;
                    return false ;

                case 12:
                    MoveCyl(ci.PKR_PkrShakeUpDn , fb.Bwd);
                    MoveCyl(ci.PKR_PkrRrFt      , fb.Fwd);
                    MoveMotr(mi.PKR_YSutl , pv.PKR_YSutlPicker);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.PKR_PkrShakeUpDn , fb.Bwd))return false; 
                    if(!CL_Complete(ci.PKR_PkrRrFt      , fb.Fwd))return false ;
                    if(!MT_GetStopPos(mi.PKR_YSutl , pv.PKR_YSutlPicker))return false ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrSuttlePlce );//플레이스는 밑에 뽁뽁이를 붙이기 위해.
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.PKR_ZPckr, pv.PKR_ZPckrSuttlePlce))return false ;
                    MoveCyl(ci.PKR_PkrClampClOp , fb.Bwd);

                    

                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.PKR_PkrClampClOp , fb.Bwd))return false ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;
                    DM.ARAY[ri.PKR].SetStat(cs.None   );
                    DM.ARAY[ri.SUT].SetStat(cs.Barcode);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleBarcode()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                IO_SetY( yi.PKR_BarcodeSpin , false) ; 
                SEQ.Bar.Stop();
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
                    IO_SetY( yi.PKR_BarcodeSpin , false) ; 
                    SEQ.Bar.Stop();
                    return true;

                case 10:
                    MoveMotr(mi.PKR_YSutl , pv.PKR_YSutlBuffer );
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.PKR_YSutl , pv.PKR_YSutlBuffer))return false ;
                    
                    if(!SEQ.Bar.Read())
                    {
                        ER_SetErr(ei.PKR_Barcode , "메세지 전송을 실패하였습니다.");
                        return false ;
                    }

                    IO_SetY( yi.PKR_BarcodeSpin , true) ; 

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(m_tmDelay.OnDelay(2000))
                    {
                        IO_SetY( yi.PKR_BarcodeSpin , false) ; 
                        SEQ.Bar.Stop();
                        if(!OM.CmnOptn.bIgnrBarcode)
                        {
                            ER_SetErr(ei.PKR_Barcode, "타임아웃");
                            return true;
                        }

                        //임시로 수정
                        DM.ARAY[ri.SUT].ID = DateTime.Now.ToString("yyyyMMdd_HHmmss"); 
                        //OM.EqpStat.sBloodID = SEQ.Bar.GetReadingText();
                        DM.ARAY[ri.SUT].SetStat(cs.Work);
                        Step.iCycle = 0;
                        return true;
                    }
                    if(!SEQ.Bar.ReadEnded()) return false ;
                    IO_SetY( yi.PKR_BarcodeSpin , false) ; 


                    DM.ARAY[ri.SUT].ID = SEQ.Bar.GetReadingText();
                    DM.ARAY[ri.SUT].SetStat(cs.Work);
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CyclePickSut()
        {
            string sTemp;
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
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake );
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;
                    Step.iCycle++;
                    return false ;

                case 12:
                    MoveCyl(ci.PKR_PkrShakeUpDn , fb.Bwd);
                    MoveCyl(ci.PKR_PkrClampClOp , fb.Bwd);
                    MoveCyl(ci.PKR_PkrRrFt, fb.Fwd);
                    MoveMotr(mi.PKR_YSutl , pv.PKR_YSutlPicker);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.PKR_PkrShakeUpDn , fb.Bwd))return false; 
                    if(!CL_Complete(ci.PKR_PkrClampClOp , fb.Bwd))return false; 
                    if(!CL_Complete(ci.PKR_PkrRrFt      , fb.Fwd))return false; 
                    if(!MT_GetStopPos(mi.PKR_YSutl , pv.PKR_YSutlPicker))return false ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrSuttlePick ); //픽는 -5씩 오프셑을 둔다.
                    Step.iCycle++;
                    return false ; 

                case 14:
                    if(!MT_GetStopPos(mi.PKR_ZPckr, pv.PKR_ZPckrSuttlePick))return false ;
                    MoveCyl(ci.PKR_PkrClampClOp , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.PKR_PkrClampClOp , fb.Fwd))return false ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;
                    DM.ARAY[ri.PKR].SetStat(DM.ARAY[ri.SUT].GetStat(0,0)); //오토큐씨 모드에도 같이 쓸라고 이렇게 함.
                    DM.ARAY[ri.SUT].SetStat(cs.None);

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePlceLdr()
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

            int r, c = 0;
            Enum pvCheck = new pv();
            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    //MoveCyl(ci.PKR_PkrClampClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 11:
                    //if(!CL_Complete(ci.PKR_PkrClampClOp , fb.Bwd))return false ;

                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake );
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;
                    MoveCyl(ci.PKR_PkrRrFt      , fb.Bwd);
                    MoveCyl(ci.PKR_PkrShakeUpDn , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.PKR_PkrRrFt      , fb.Bwd))return false ;
                    if(!CL_Complete(ci.PKR_PkrShakeUpDn , fb.Bwd))return false ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrRackPlce );
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrRackPlce))return false ;
                    MoveCyl(ci.PKR_PkrClampClOp , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!CL_Complete(ci.PKR_PkrClampClOp , fb.Bwd))return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;
                    
                case 16: 
                    if(!m_tmDelay.OnDelay(100)) return false ;
                    MoveMotr(mi.PKR_ZPckr , pv.PKR_ZPckrShake );
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!MT_GetStopPos(mi.PKR_ZPckr , pv.PKR_ZPckrShake))return false ;

                    DM.ARAY[ri.LDR].SetStat(cs.WorkEnd );
                    DM.ARAY[ri.PKR].SetStat(cs.None    );

                    SPC.DAY.Data.DayWorkCnt++;


                    //MoveCyl(ci.PKR_PkrRrFt      , fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 18:
                    //if(!CL_Complete(ci.PKR_PkrRrFt , fb.Fwd))return false ;
                    Step.iCycle= 0 ;
                    return true ;
            }
        }

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.PKR_PkrClampClOp)
            {

            }
            else if (_eActr == ci.PKR_PkrRrFt)
            {

            }
            else if(_eActr == ci.PKR_PkrShakeUpDn)
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

            if (_eMotr == mi.PKR_YSutl)
            {
                if(MT_CmprPos(mi.SYR_XSyrg, PM.GetValue(mi.SYR_XSyrg, pv.SYR_XSyrgSuttle)))
                {
                    if (MT_GetCmdPos(mi.SYR_ZSyrg) > PM.GetValue(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove) + 1.0)
                    {
                        sMsg = "Motor " + MT_GetName(mi.SYR_ZSyrg) + "가 이동 포지션이 아닙니다.";
                        bRet = false;

                    }

                }
                
            }
            else if(_eMotr == mi.PKR_ZPckr)
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

            if (_eMotr == mi.PKR_YSutl)
            {
                if (MT_CmprPos(mi.SYR_XSyrg, PM.GetValue(mi.SYR_XSyrg, pv.SYR_XSyrgSuttle)))
                {
                    if (MT_GetCmdPos(mi.SYR_ZSyrg) > PM.GetValue(mi.SYR_ZSyrg, pv.SYR_ZSyrgMove) + 1.0)
                    {
                        sMsg = "Motor " + MT_GetName(mi.SYR_ZSyrg) + "가 이동 포지션이 아닙니다.";
                        bRet = false;

                    }

                }
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
            else if (_eMotr == mi.PKR_ZPckr)
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
            if (!CL_Complete(ci.PKR_PkrRrFt     )) return false;
            if (!CL_Complete(ci.PKR_PkrClampClOp)) return false;

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
