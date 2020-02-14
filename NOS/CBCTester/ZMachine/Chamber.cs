﻿using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    
    public class Chamber : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
            public bool bReqStop   ;
            public bool bNeedAir   ; //메인에어 필요함.

            public bool bRqstFillChamber1 ;
            public bool bRqstFillChamber2 ;
            public bool bRqstFillChamber3 ;
            public bool bRqstFillChamber4 ;
            public bool bRqstFillChamber5 ;
            public bool bRqstFillChamber6 ;

            public bool bRqstEmptyChamber1 ;
            public bool bRqstEmptyChamber2 ;
            public bool bRqstEmptyChamber3 ;
            public bool bRqstEmptyChamber4 ;
            public bool bRqstEmptyChamber5 ;
            public bool bRqstEmptyChamber6 ;

            //이건 열었다가 그냥 Stop을 안탈수도 있어 
            //오토런에서 포지션보고 판단하게 한다.
            //public bool bRqstStartClean    ; //클린벨브 열기.
            //public bool bRqstStopClean     ; //클린벨브 닫기.

            public void Clear()
            {
                bWorkEnd    = false ;
                bReqStop    = false ;
                bNeedAir    = false ;
            }
        };   

        public enum sc
        {
            Idle         = 0,

            FillChamber1    ,
            FillChamber2    ,
            FillChamber3    ,
            FillChamber4    ,
            FillChamber5    ,
            FillChamber6    ,
            
            EmptyChamber1   ,
            EmptyChamber2   ,
            EmptyChamber3   ,
            EmptyChamber4   ,
            EmptyChamber5   ,
            EmptyChamber6   ,

            Clean           ,
            
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

        public Chamber(int _iPartId = 0)
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
            bool IsFillingCP_1         = !ML.IO_GetY(yi.TankCP1_InSt    ) ;
            bool IsFillingCP_2         = !ML.IO_GetY(yi.TankCP2_InSt    ) ;
            bool IsFillingCP_3         = !ML.IO_GetY(yi.TankCP3_InSt    ) ;
            bool IsFillingCS_Front     = !ML.IO_GetY(yi.TankCSFront_InSt) ;
            bool IsFillingCS_Rear      = !ML.IO_GetY(yi.TankCSRear_InSt ) ;
            bool IsFillingSULF         = !ML.IO_GetY(yi.TankSULF_InSt   ) ;
            bool IsFillingFB           = !ML.IO_GetY(yi.TankFB_InSt     ) ;
            bool IsFilling4DL          = !ML.IO_GetY(yi.Tank4DL_InSt    ) ;
            bool IsFillingRET          = !ML.IO_GetY(yi.TankRET_InSt    ) ;
            bool IsFillingNR           = !ML.IO_GetY(yi.TankNR_InSt     ) ;

            bool IsDischargingCP_1     = !ML.IO_GetY(yi.TankCP1_OtSt    ) ;
            bool IsDischargingCP_2     = !ML.IO_GetY(yi.TankCP2_OtSt    ) ;
            bool IsDischargingCP_3     = !ML.IO_GetY(yi.TankCP3_OtSt    ) ;
            bool IsDischargingCS_Front = !ML.IO_GetY(yi.TankCSFront_OtSt) ;
            bool IsDischargingCS_Rear  = !ML.IO_GetY(yi.TankCSRear_OtSt ) ;
            bool IsDischargingSULF     = !ML.IO_GetY(yi.TankSULF_OtSt   ) ;
            bool IsDischargingFB       = !ML.IO_GetY(yi.TankFB_OtSt     ) ;
            bool IsDischarging4DL      = !ML.IO_GetY(yi.Tank4DL_OtSt    ) ;
            bool IsDischargingRET      = !ML.IO_GetY(yi.TankRET_OtSt    ) ;
            bool IsDischargingNR       = !ML.IO_GetY(yi.TankNR_OtSt     ) ;

            bool bNeedFillCP_1         = !ML.IO_GetX(xi.FullCP_1        ) ;
            bool bNeedFillCP_2         = !ML.IO_GetX(xi.FullCP_2        ) ;
            bool bNeedFillCP_3         = !ML.IO_GetX(xi.FullCP_3        ) ;
            bool bNeedFillCS_Front     = !ML.IO_GetX(xi.FullCS_Front    ) ;
            bool bNeedFillCS_Rear      = !ML.IO_GetX(xi.FullCS_Rear     ) ;
            bool bNeedFillSULF         = !ML.IO_GetX(xi.FullSULF        ) ;
            bool bNeedFillFB           = !ML.IO_GetX(xi.FullFB          ) ;
            bool bNeedFill4DL          = !ML.IO_GetX(xi.Full4DL         ) ;
            bool bNeedFillRET          = !ML.IO_GetX(xi.FullRET         ) ;
            bool bNeedFillNR           = !ML.IO_GetX(xi.FullNR          ) ;

            //탱크 채울 차람 이면 켜고.
            bool bNeedAir = false ;
            if(!IsFillingCP_1     && !IsDischargingCP_1     && bNeedFillCP_1    ){ML.IO_SetY(yi.TankCP1_InSt    ,true);ML.IO_GetY(yi.TankCP1_ArVc    ,false);bNeedAir = true ;}
            if(!IsFillingCP_2     && !IsDischargingCP_2     && bNeedFillCP_2    ){ML.IO_SetY(yi.TankCP2_InSt    ,true);ML.IO_GetY(yi.TankCP2_ArVc    ,false);bNeedAir = true ;}
            if(!IsFillingCP_3     && !IsDischargingCP_3     && bNeedFillCP_3    ){ML.IO_SetY(yi.TankCP3_InSt    ,true);ML.IO_GetY(yi.TankCP3_ArVc    ,false);bNeedAir = true ;}
            if(!IsFillingCS_Front && !IsDischargingCS_Front && bNeedFillCS_Front){ML.IO_SetY(yi.TankCSFront_InSt,true);ML.IO_GetY(yi.TankCSFront_ArVc,false);bNeedAir = true ;}
            if(!IsFillingCS_Rear  && !IsDischargingCS_Rear  && bNeedFillCS_Rear ){ML.IO_SetY(yi.TankCSRear_InSt ,true);ML.IO_GetY(yi.TankCSRear_ArVc ,false);bNeedAir = true ;}
            if(!IsFillingSULF     && !IsDischargingSULF     && bNeedFillSULF    ){ML.IO_SetY(yi.TankSULF_InSt   ,true);ML.IO_GetY(yi.TankSULF_ArVc   ,false);bNeedAir = true ;}
            if(!IsFillingFB       && !IsDischargingFB       && bNeedFillFB      ){ML.IO_SetY(yi.TankFB_InSt     ,true);ML.IO_GetY(yi.TankFB_ArVc     ,false);bNeedAir = true ;}
            if(!IsFilling4DL      && !IsDischarging4DL      && bNeedFill4DL     ){ML.IO_SetY(yi.Tank4DL_InSt    ,true);ML.IO_GetY(yi.Tank4DL_ArVc    ,false);bNeedAir = true ;}
            if(!IsFillingRET      && !IsDischargingRET      && bNeedFillRET     ){ML.IO_SetY(yi.TankRET_InSt    ,true);ML.IO_GetY(yi.TankRET_ArVc    ,false);bNeedAir = true ;}
            if(!IsFillingNR       && !IsDischargingNR       && bNeedFillNR      ){ML.IO_SetY(yi.TankNR_InSt     ,true);ML.IO_GetY(yi.TankNR_ArVc     ,false);bNeedAir = true ;}

            Stat.bNeedAir = bNeedAir ;


            //탱크 다 찻음 끄고
            if(IsFillingCP_1     && !bNeedFillCP_1    ){ML.IO_SetY(yi.TankCP1_InSt    ,false);ML.IO_GetY(yi.TankCP1_ArVc    ,true);}
            if(IsFillingCP_2     && !bNeedFillCP_2    ){ML.IO_SetY(yi.TankCP2_InSt    ,false);ML.IO_GetY(yi.TankCP2_ArVc    ,true);}
            if(IsFillingCP_3     && !bNeedFillCP_3    ){ML.IO_SetY(yi.TankCP3_InSt    ,false);ML.IO_GetY(yi.TankCP3_ArVc    ,true);}
            if(IsFillingCS_Front && !bNeedFillCS_Front){ML.IO_SetY(yi.TankCSFront_InSt,false);ML.IO_GetY(yi.TankCSFront_ArVc,true);}
            if(IsFillingCS_Rear  && !bNeedFillCS_Rear ){ML.IO_SetY(yi.TankCSRear_InSt ,false);ML.IO_GetY(yi.TankCSRear_ArVc ,true);}
            if(IsFillingSULF     && !bNeedFillSULF    ){ML.IO_SetY(yi.TankSULF_InSt   ,false);ML.IO_GetY(yi.TankSULF_ArVc   ,true);}
            if(IsFillingFB       && !bNeedFillFB      ){ML.IO_SetY(yi.TankFB_InSt     ,false);ML.IO_GetY(yi.TankFB_ArVc     ,true);}
            if(IsFilling4DL      && !bNeedFill4DL     ){ML.IO_SetY(yi.Tank4DL_InSt    ,false);ML.IO_GetY(yi.Tank4DL_ArVc    ,true);}
            if(IsFillingRET      && !bNeedFillRET     ){ML.IO_SetY(yi.TankRET_InSt    ,false);ML.IO_GetY(yi.TankRET_ArVc    ,true);}
            if(IsFillingNR       && !bNeedFillNR      ){ML.IO_SetY(yi.TankNR_InSt     ,false);ML.IO_GetY(yi.TankNR_ArVc     ,true);}
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

                if (OM.EqpStat.bAutoQCMode){ Stat.bWorkEnd = true; return true; }

                //사이클
                bool isFillChamber1  = Stat.bRqstFillChamber1  ;
                bool isFillChamber2  = Stat.bRqstFillChamber2  ;
                bool isFillChamber3  = Stat.bRqstFillChamber3  ; 
                bool isFillChamber4  = Stat.bRqstFillChamber4  ;
                bool isFillChamber5  = Stat.bRqstFillChamber5  ;
                bool isFillChamber6  = Stat.bRqstFillChamber6  ;
                                       
                bool isEmptyChamber1 = Stat.bRqstEmptyChamber1 ;
                bool isEmptyChamber2 = Stat.bRqstEmptyChamber2 ;
                bool isEmptyChamber3 = Stat.bRqstEmptyChamber3 ;
                bool isEmptyChamber4 = Stat.bRqstEmptyChamber4 ;
                bool isEmptyChamber5 = Stat.bRqstEmptyChamber5 ;
                bool isEmptyChamber6 = Stat.bRqstEmptyChamber6 ;

                bool isClean         = MT_GetTrgPos(mi.SYR_XSyrg) == PM.GetValue(mi.SYR_XSyrg , pv.SYR_XSyrgClean ) &&
                                       MT_GetTrgPos(mi.SYR_ZSyrg) == PM.GetValue(mi.SYR_ZSyrg , pv.SYR_ZSyrgChamer) ; 






                bool isMove    = !DM.ARAY[ri.LDR].CheckAllStat(cs.Shake  )  &&  DM.ARAY[ri.SUT].CheckAllStat(cs.None   ) && DM.ARAY[ri.PKR].CheckAllStat(cs.None   ) && IO_GetX(xi.LDR_WorkRackExist) ;
                bool isOut     =  DM.ARAY[ri.LDR].CheckAllStat(cs.WorkEnd)  && !IO_GetX(xi.LDR_OutArrival);
                bool isOutPush =  IO_GetX(xi.LDR_OutArrival) ;
                bool isWorkEnd =  DM.ARAY[ri.LDR].CheckAllStat(cs.None   )  && !IO_GetX(xi.LDR_WorkRackExist) && !IO_GetX(xi.LDR_OutArrival   ) &&Stat.bSupplied ;
                //bSupplied는 Cycle


                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.
                     if (isOutPush) { DM.ARAY[ri.LDR].Trace(m_iPartId); Step.eSeq = sc.OutPush ; }
                else if (isOut    ) { DM.ARAY[ri.LDR].Trace(m_iPartId); Step.eSeq = sc.Out     ; }
                else if (isMove   ) { DM.ARAY[ri.LDR].Trace(m_iPartId); Step.eSeq = sc.Move    ; }
                else if (isIn     ) { DM.ARAY[ri.LDR].Trace(m_iPartId); Step.eSeq = sc.In      ; }
                else if (isSupply ) { DM.ARAY[ri.LDR].Trace(m_iPartId); Step.eSeq = sc.Supply  ; }
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
                case (sc.Idle    ): return false;
                case (sc.OutPush ): if (!CycleOutPush()) return false; break;
                case (sc.Out     ): if (!CycleOut    ()) return false; break;
                case (sc.Move    ): if (!CycleMove   ()) return false; break;
                case (sc.In      ): if (!CycleIn     ()) return false; break;
                case (sc.Supply  ): if (!CycleSupply ()) return false; break;

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
                    MoveCyl(ci.LDR_OutPshrFtRr  , fb.Bwd);
                    MoveCyl(ci.LDR_WorkStprRrFt , fb.Bwd);
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
                    if(IO_GetX(xi.LDR_OutFull))
                    {
                        ER_SetErr(ei.LDR_UnloaderFull , "언로더에 튜브렉을 치워주세요.");
                        return true ;
                    }
                    MoveCyl(ci.LDR_OutPshrFtRr , fb.Fwd);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.LDR_OutPshrFtRr  ,fb.Fwd)) return false;
                    m_tmCycle.Clear();
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!m_tmCycle.OnDelay(500))return false ;
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
                    MoveCyl(ci.LDR_OutPshrFtRr , fb.Bwd);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.LDR_OutPshrFtRr  ,fb.Bwd)) return false;
                    MT_JogP(mi.LDR_XBelt);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!IO_GetX(xi.LDR_OutArrival))return false ;
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
                    
                    if(IO_GetX(xi.LDR_WorkTubeExist))
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

        public bool CycleIn()
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


            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    MoveCyl(ci.LDR_WorkStprRrFt , fb.Fwd);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!CL_Complete(ci.LDR_WorkStprRrFt  ,fb.Fwd)) return false;
                    MT_JogP(mi.LDR_XBelt);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!IO_GetX(xi.LDR_WorkRackExist))return false ;
                    MT_Stop(mi.LDR_XBelt);
                    if(IO_GetX(xi.LDR_WorkTubeExist))
                    {
                        DM.ARAY[ri.LDR].SetStat(cs.Shake);
                    }
                    else
                    {
                        DM.ARAY[ri.LDR].SetStat(cs.Empty);
                    }
                    Step.iCycle=0;
                    return false ;
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
                    MoveMotr(mi.LDR_YBelt , pv.LDR_YBeltWorkStart);
                    Step.iCycle++;
                    return false;
                    
                case 11: 
                    if(!MT_GetStopPos(mi.LDR_YBelt  , pv.LDR_YBeltWorkStart)) return false;
                    MoveMotr(mi.LDR_YBelt , pv.LDR_YBeltWorkEnd);
                    Step.iCycle++;
                    return false;
                    
                case 12: 
                    if(IO_GetX(xi.LDR_InArrival))
                    {
                        MT_Stop(mi.LDR_YBelt);
                        Step.iCycle++;
                    }
                    if(!MT_GetStopPos(mi.LDR_YBelt  , pv.LDR_YBeltWorkEnd)) return false;
                    
                    //자제 없고 한번 공급 동작 한것에 대한 플레그 세움.
                    Stat.bSupplied = true ;
                    Step.iCycle++;
                    return false ;
                    

                case 13: //정상 도착
                    if(!MT_GetStop(mi.LDR_YBelt)) return false;
                    MoveMotr(mi.LDR_YBelt , pv.LDR_YBeltWorkStart);
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!MT_GetStopPos(mi.LDR_YBelt , pv.LDR_YBeltWorkStart)) return false ;

                    Step.iCycle=0;
                    return false ;
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
/*
 void Update()
        {
            


            






            bool IsWork
            bool IsWork
            bool IsWork
            bool IsWork










            if(ML.IOGetX(xi. ))





        }
     */