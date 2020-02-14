using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON;
using SML2;
using System.IO;
using System.Xml.Serialization;

namespace Machine
{
    //PCK == Picker
    public class Tool : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop;
            public void Clear()
            {
                bWorkEnd = false;
                bReqStop = false;
            }
        };   
        public enum sc
        {
            Idle             = 0,
            SubsAlignVisn       , //SubStrate Stage에서 얼라인검사.
            Height              , //서브스트레이트 사전 높이 측정.
            Dispense            , //디스펜싱.
            DispenseVisn        , //디스펜싱후 비전검사.
            Eject               , //웨이퍼 다이 이젝팅.
            DieAlignVisn        , //웨이퍼 스테이지 얼라인측정.
            Pick                , //웨이퍼 다이 픽.
            BtmAlignVisn        , //바텀 얼라인 측정.
            DistanceVisn        , //디스턴스 측정.
            Place               , //압축 안착.
            BltHeight           , //끝나고 높이 측정하여 에폭시 층 두께 측정.
            EndDistanceVisn     , //끝나고 좌측 우측 모두 거리 측정 하기.

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

        VisnCom.TRslt RsltDieAlign     ;
        VisnCom.TRslt RsltSubsAlign    ;
        VisnCom.TRslt RsltBtmAlign     ;
        VisnCom.TRslt RsltRightDist    ;
        VisnCom.TRslt RsltLeftDist     ;

        //double OM.EqpStat.dDieVisnPosOfsX ;
        //double OM.EqpStat.dDieVisnPosOfsY ;

        const int iVisnDelay  = 100 ;
        const double dCheckOfs = -5.0;

        public Tool()
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

            LoadLists();

            Reset();                  
        }
        ~Tool() 
        {
            SaveLists();
        }

                //높이 측정값 갯수가 가변이여서 리스트를 이용.
        struct BltValue {
            public double dSub ;
            public double dDie ;
        }
        List<BltValue> BltValues = new List<BltValue>();  
        List<double> SubValues = new List<double>();
        List<double> EndValues = new List<double>();

        public void SaveLists() {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\ListValues.ini";
            CIniFile Ini = new CIniFile(sPath);

            Ini.Save("SubValues", "Count", SubValues.Count);
            for(int i = 0;i < SubValues.Count;i++) {
                Ini.Save("SubValues", i.ToString(), SubValues[i]);
            }
            
            Ini.Save("EndValues", "Count", SubValues.Count);
            for(int i = 0;i < EndValues.Count;i++) {
                Ini.Save("EndValues", i.ToString(), EndValues[i]);
            }

            //BltValue는 세이브 필요없음.
        }
        public void LoadLists() {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\ListValues.ini";
            CIniFile Ini = new CIniFile(sPath);
            double dVal = 0.0 ;
            int    iVal = 0   ;

            Ini.Load("SubValues", "Count", out iVal);
            for(int i = 0;i < iVal;i++) {
                Ini.Load("SubValues", i.ToString(), out dVal);
                SubValues.Add(dVal);
            }
            
            Ini.Save("EndValues", "Count", SubValues.Count);
            for(int i = 0;i < iVal;i++) {
                Ini.Load("EndValues", i.ToString(), out dVal);
                EndValues.Add(dVal);
            }

            //BltValue는 세이브 필요없음.
        }
        /*
        public void SaveLists() {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\SubValues.lst";
            FileStream fsFileSub = File.Create(sPath);
            XmlSerializer xsFormSub = new XmlSerializer(typeof(double));
            xsFormSub.Serialize(fsFileSub,SubValues);

            sPath = sExeFolder + "Util\\EndValues.lst";
            FileStream fsFileEnd = File.Create(sPath);
            XmlSerializer xsFormEnd = new XmlSerializer(typeof(double));
            xsFormEnd.Serialize(fsFileEnd,EndValues);

            sPath = sExeFolder + "Util\\BltValues.lst";
            FileStream fsFileBlt = File.Create(sPath);
            XmlSerializer xsFormBlt = new XmlSerializer(typeof(BltValue));
            xsFormBlt.Serialize(fsFileBlt,BltValues);
        }
        public void LoadLists() {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\SubValues.lst";
            XmlSerializer xsFormSub = new XmlSerializer(typeof(double));
            FileStream fsFileSub = new FileStream(sPath , FileMode.OpenOrCreate);
            byte[] bufferSub = new byte[fsFileSub.Length];
            fsFileSub.Read(bufferSub,0,(int)fsFileSub.Length);
            MemoryStream msMemSub = new MemoryStream(bufferSub);
            SubValues = (List<double>)xsFormSub.Deserialize(msMemSub);

            sPath = sExeFolder + "Util\\EndValues.lst";
            XmlSerializer xsFormEnd = new XmlSerializer(typeof(double));
            FileStream fsFileEnd = new FileStream(sPath , FileMode.OpenOrCreate);
            byte[] bufferEnd = new byte[fsFileEnd.Length];
            fsFileEnd.Read(bufferEnd,0,(int)fsFileEnd.Length);
            MemoryStream msMemEnd = new MemoryStream(bufferEnd);
            EndValues = (List<double>)xsFormEnd.Deserialize(msMemEnd);

            sPath = sExeFolder + "Util\\BltValues.lst";
            XmlSerializer xsFormBlt = new XmlSerializer(typeof(BltValue));
            FileStream fsFileBlt = new FileStream(sPath , FileMode.OpenOrCreate);
            byte[] bufferBlt = new byte[fsFileBlt.Length];
            fsFileBlt.Read(bufferEnd,0,(int)fsFileBlt.Length);
            MemoryStream msMemBlt = new MemoryStream(bufferBlt);
            BltValues = (List<BltValue>)xsFormBlt.Deserialize(msMemBlt);

        }*/


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

            int a = 0; 
            a++;

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
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: Step.iToStart = 0;
                    return true;

                case 10:
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    
                    Step.iToStart++;
                    return false;
                
                case 11: 
                    if(!MT_GetStopPos(mi.TOOL_ZDisp, pv.TOOL_ZDispWait)) return false ;
                    MoveCyl(ci.TOOL_DispCvFwBw,fb.Fwd);
                    Step.iToStart++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.TOOL_DispCvFwBw,fb.Fwd)) return false ;
                    MoveMotr(mi.TOOL_XRght,pv.TOOL_XRghtWait);

                    Step.iToStart++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XRght,pv.TOOL_XRghtWait)) return false ;
                    //이미 어테치 전에 스탑이면 이미 에폭시를 발라놔서 띠기 뭐하다.
                    if(DM.ARAY[ri.PCKR].CheckStat(0,0,cs.Distance) ||
                       DM.ARAY[ri.PCKR].CheckStat(0,0,cs.Attach  ) ){
                        return true ;
                    }
                    
                    MoveMotr(mi.TOOL_ZPckr,pv.TOOL_ZPckrWait);
                    MoveCyl(ci.TOOL_GuidFtDwUp, fb.Bwd);
                    MoveCyl(ci.TOOL_GuidRrDwUp, fb.Bwd);
                    Step.iToStart++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr,pv.TOOL_ZPckrWait)) return false ;
                    if(!CL_Complete(ci.TOOL_GuidFtDwUp, fb.Bwd)) return false ;
                    if(!CL_Complete(ci.TOOL_GuidRrDwUp, fb.Bwd)) return false ;
                    MoveMotr(mi.TOOL_XLeft,pv.TOOL_XLeftWait);
                    if(!DM.ARAY[ri.WSTG].CheckAllStat(cs.None)) {
                        SEQ.WSTG.MoveMotr(mi.WSTG_ZExpd , pv.WSTG_ZExpdWork);
                    }
                    Step.iToStart++;
                    return false;

                case 15:
                    if(!MT_GetStopPos(mi.TOOL_XLeft,pv.TOOL_XLeftWait)) return false ;
                    if(!MT_GetStop   (mi.WSTG_ZExpd)) return false ;
                    if(!DM.ARAY[ri.WSTG].CheckAllStat(cs.None)) {
                        SEQ.WSTG.MoveMotr(mi.WSTG_TTble , pv.WSTG_TTbleWfrWork);
                    }
                    Step.iToStart++;
                    return false ;

                case 16:
                    if(!MT_GetStop   (mi.WSTG_TTble)) return false ;

                    
                    Step.iToStart = 0;
                    return true ;

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
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iToStop = Step.iToStop;
            Stat.bReqStop = false;            

            //Move Home.
            switch (Step.iToStop)
            {
                default: Step.iToStop = 0;
                    return true;

                case 10:
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    
                    Step.iToStop++;
                    return false;
                
                case 11: 
                    if(!MT_GetStopPos(mi.TOOL_ZDisp, pv.TOOL_ZDispWait)) return false ;
                    MoveCyl(ci.TOOL_DispCvFwBw,fb.Fwd);
                    Step.iToStop++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.TOOL_DispCvFwBw,fb.Fwd)) return false ;
                    MoveMotr(mi.TOOL_XRght,pv.TOOL_XRghtWait);
                    Step.iToStop++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XRght,pv.TOOL_XRghtWait)) return false ;

                    //이미 어테치 전에 스탑이면 이미 에폭시를 발라놔서 띠기 뭐하다.
                    if(DM.ARAY[ri.PCKR].CheckStat(0,0,cs.Distance) ||
                       DM.ARAY[ri.PCKR].CheckStat(0,0,cs.Attach  ) ){
                        return true ;
                    }
                    
                    MoveMotr(mi.TOOL_ZPckr,pv.TOOL_ZPckrWait);
                    MoveCyl(ci.TOOL_GuidFtDwUp, fb.Bwd);
                    MoveCyl(ci.TOOL_GuidRrDwUp, fb.Bwd);
                    Step.iToStop++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr,pv.TOOL_ZPckrWait)) return false ;
                    if(!CL_Complete(ci.TOOL_GuidFtDwUp, fb.Bwd))return false ;
                    if(!CL_Complete(ci.TOOL_GuidRrDwUp, fb.Bwd))return false ;
                    MoveMotr(mi.TOOL_XLeft,pv.TOOL_XLeftWait);
                    Step.iToStop++;
                    return false;

                case 15:
                    if(!MT_GetStopPos(mi.TOOL_XLeft,pv.TOOL_XLeftWait)) return false ;
                    Step.iToStop = 0 ;
                    return true ;

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

        public bool m_bContiWork = true ; //로더 작업 다했을때 로더에 자제 넣으라는 에러를 띄울껀지 아니면 자동 랏마스킹 앤드를 시킬지.
        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            //Check Cycle Time Out.
            String sTemp;
            sTemp = String.Format("%s Step.iCycle={0:00}", "Autorun", Step.iCycle);
            if (Step.eSeq != PreStep.eSeq) {
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                if( DM.ARAY[ri.PCKR].CheckAllStat(cs.None) && IO_GetX(xi.TOOL_PckrVac)) {ER_SetErr(ei.PKG_Unknwn , "Picker Unknwn PKG Error."   ); return false;}
                if(!DM.ARAY[ri.PCKR].CheckAllStat(cs.None) &&!IO_GetX(xi.TOOL_PckrVac)) {ER_SetErr(ei.PKG_Dispr  , "Picker Disappear PKG Error."); return false;}

                int iWStgR = 0      ; 
                int iWStgC = 0      ;
                int iWAray = ri.WSTG; 
                SEQ.WSTG.FindChip(out iWAray, out iWStgC, out iWStgR);

                int iSStgR = 0      ; 
                int iSStgC = 0      ;
                int iSAray = ri.SSTG;
                SEQ.SSTG.FindChip(out iSAray, out iSStgC, out iSStgR);                

                //SSTG : Align->Height->Disp    ->DispVisn->Attach                                                ->EndVisn->EndHeight->WorkEnd
                //WSTG :                                            Eject ->Align->Pick  ->Empty   
                //PCKR :                                            None         ->BtmVisn  -> Attach
                bool isCycleSubsAlignVisn   = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.Align          ) &&
                                              DM.ARAY[ri.SSTG].Step >= DM.ARAY[ri.SSTG].GetCntStat(cs.WorkEnd) &&
                                              !DM.ARAY[ri.SSTG].CheckAllStat(cs.WorkEnd)                       &&
                                              SEQ.SSTG.GetSeqStep() == (int)SubstrateStage.sc.Idle; //SubStrate Stage에서 얼라인검사.
                bool isCycleHeight          = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.SubHeight) ||
                                              DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.EndHeight)  ; //서브스트레이트 사전 높이 측정 및 Die 올리고 측정.
                bool isCycleDispense        = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.Disp     ) ; //디스펜싱.
                bool isCycleDispenseVisn    = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.DispVisn ) ; //디스펜싱후 비전검사.

                bool isCycleEject           = DM.ARAY[ri.WSTG].CheckStat(iWStgC , iWStgR , cs.Eject          ) &&
                                              DM.ARAY[ri.PCKR].CheckStat(0      , 0      , cs.None           ) &&
                                              DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.Attach         ) &&
                                              SEQ.WSTG.GetSeqStep() == (int)WaferStage.sc.Idle; //웨이퍼 스테이지 얼라인측정.
                bool isCycleDieAlignVisn    =  DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.Attach         )  &&
                                              (DM.ARAY[ri.WSTG].CheckStat(iWStgC , iWStgR , cs.EjectAlign     )  || DM.ARAY[ri.WSTG].CheckStat(iWStgC , iWStgR , cs.Align          )) && //웨이퍼 스테이지 얼라인측정.
                                               DM.ARAY[ri.PCKR].CheckStat(0      , 0      , cs.None           )  &&                                               
                                               SEQ.WSTG.GetSeqStep() == (int)WaferStage    .sc.Idle              &&
                                               SEQ.SSTG.GetSeqStep() == (int)SubstrateStage.sc.Idle;

                bool isCyclePick            = DM.ARAY[ri.WSTG].CheckStat(iWStgC , iWStgR , cs.Pick           ) &&
                                              DM.ARAY[ri.PCKR].CheckStat(     0 ,      0 , cs.None           ) ; //웨이퍼 다이 픽.

                bool isCycleBtmAlignVisn    = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.Attach         ) &&
                                              DM.ARAY[ri.PCKR].CheckStat(     0 ,      0 , cs.BtmVisn        ) ; //바텀 얼라인 측정.
                bool isCycleDistanceVisn    = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.Attach         ) &&
                                              DM.ARAY[ri.PCKR].CheckStat(     0 ,      0 , cs.Distance       ) ; //디스턴스 측정.
                bool isCyclePlace           = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.Attach         ) &&
                                              DM.ARAY[ri.PCKR].CheckStat(     0 ,      0 , cs.Attach         ) ; //압축 안착.

                bool isCycleBltHeight       = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.BltHeight      ) ; //끝나고 높이 측정하여 BLT 측정.
                bool isCycleEndDistanceVisn = DM.ARAY[ri.SSTG].CheckStat(iSStgC , iSStgR , cs.EndVisn        ) ; //끝나고 좌측 우측 모두 거리 측정 하기.

                bool isCycleEnd             = DM.ARAY[ri.WSTG].CheckAllStat(cs.None) && 
                                              DM.ARAY[ri.SSTG].CheckAllStat(cs.None) &&
                                              DM.ARAY[ri.PCKR].CheckAllStat(cs.None) ;
                                   
                if (ER_IsErr()) return false;
                //Normal Decide Step.
                     if (isCycleSubsAlignVisn   ) { Step.eSeq  = sc.SubsAlignVisn    ; }
                else if (isCycleHeight          ) { Step.eSeq  = sc.Height           ; }
                else if (isCycleDispense        ) { Step.eSeq  = sc.Dispense         ; }
                else if (isCycleDispenseVisn    ) { Step.eSeq  = sc.DispenseVisn     ; }
                else if (isCycleEject           ) { Step.eSeq  = sc.Eject            ; }
                else if (isCycleDieAlignVisn    ) { Step.eSeq  = sc.DieAlignVisn     ; }
                else if (isCyclePick            ) { Step.eSeq  = sc.Pick             ; }
                else if (isCycleBtmAlignVisn    ) { Step.eSeq  = sc.BtmAlignVisn     ; }
                else if (isCycleDistanceVisn    ) { Step.eSeq  = sc.DistanceVisn     ; }
                else if (isCyclePlace           ) { Step.eSeq  = sc.Place            ; }
                else if (isCycleBltHeight       ) { Step.eSeq  = sc.BltHeight        ; }
                else if (isCycleEndDistanceVisn ) { Step.eSeq  = sc.EndDistanceVisn  ; }
                else if (isCycleEnd             ) { Stat.bWorkEnd = true; return true; }

                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle){
                    Log.Trace(m_sPartName, Step.eSeq.ToString() +" Start");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }

            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default                :                               Log.Trace(m_sPartName, "default End");                                    Step.eSeq = sc.Idle;   return false;
                case sc.Idle           :                                                                                                                                return false;
                case sc.SubsAlignVisn  : if (CycleSubsAlignVisn  ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Height         : if (CycleHeight         ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Dispense       : if (CycleDispense       ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.DispenseVisn   : if (CycleDispenseVisn   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Eject          : if (CycleEject          ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.DieAlignVisn   : if (CycleDieAlignVisn   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Pick           : if (CyclePick           ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.BtmAlignVisn   : if (CycleBtmAlignVisn   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.DistanceVisn   : if (CycleDistanceVisn   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.Place          : if (CyclePlace          ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.BltHeight      : if (CycleBltHeight      ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
                case sc.EndDistanceVisn: if (CycleEndDistanceVisn()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.eSeq].End(); Step.eSeq = sc.Idle; } return false;
            }                               
        }
        //인터페이스 상속 끝.==================================================
        #endregion

        public double GetXPosHeightFromVisn(double _dPos)
        {
            //겐트리Y와 오른쪽X는 3사분면 사용.
            double dVisnToHeightOffset = PM_GetValue(mi.TOOL_XRght , pv.TOOL_XRghtHghtCheck) - PM_GetValue(mi.TOOL_XRght , pv.TOOL_XRghtTVsnCheck);
            return _dPos + dVisnToHeightOffset ;
        }
        public double GetYPosHeightFromVisn(double _dPos)
        {
            //겐트리Y와 오른쪽X는 3사분면 사용.
            double dVisnToHeightOffset = PM_GetValue(mi.TOOL_YGent , pv.TOOL_YGentHghtCheck) - PM_GetValue(mi.TOOL_YGent , pv.TOOL_YGentTVsnCheck);
            return _dPos + dVisnToHeightOffset ;
        }     

        public double GetXPosDispFromVisn(double _dPos)
        {
            //겐트리Y와 오른쪽X는 3사분면 사용.
            double dVisnToDispOffset = PM_GetValue(mi.TOOL_XRght , pv.TOOL_XRghtDispCheck) - PM_GetValue(mi.TOOL_XRght , pv.TOOL_XRghtTVsnCheck);
            return _dPos + dVisnToDispOffset ;
        }
        public double GetYPosDispFromVisn(double _dPos)
        {
            //겐트리Y와 오른쪽X는 3사분면 사용.
            double dVisnToDispOffset = PM_GetValue(mi.TOOL_YGent , pv.TOOL_YGentDispCheck) - PM_GetValue(mi.TOOL_YGent , pv.TOOL_YGentTVsnCheck);
            return _dPos + dVisnToDispOffset ;
        }

        public double GetDispPosX(int _iNodeIdx) 
        {
            //좌표상에서 계산.
            double dStartPos    = SEQ.DispPtrn.GetScaleDispPosX(0);
            double dIncTrgPos   = SEQ.DispPtrn.GetScaleDispPosX(_iNodeIdx) - dStartPos ;

            //모터상에서 계산.
            double dSttPos  = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtDispStt);
            double dRet     = dSttPos - dIncTrgPos ;

            return dRet;
        }

        public double GetDispPosY(int _iNodeIdx) 
        {
            //좌표상에서 계산.
            double dStartPos    = SEQ.DispPtrn.GetScaleDispPosY(0);
            double dIncTrgPos   = SEQ.DispPtrn.GetScaleDispPosY(_iNodeIdx) - dStartPos ;

            //모터상에서 계산.
            double dSttPos  = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentDispStt);
            double dRet     = dSttPos + dIncTrgPos ;

            return dRet;
        }

        public double GetHghtPosX(int _iNodeIdx) 
        {
            //좌표상에서 계산.
            double dStartPos    = SEQ.HghtPtrn.GetScaleHghtPosX(0);
            double dIncTrgPos   = SEQ.HghtPtrn.GetScaleHghtPosX(_iNodeIdx) - dStartPos ;

            //모터상에서 계산.
            double dHghtSttPos  = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtHghtStt);
            double dRet         = dHghtSttPos - dIncTrgPos ;

            return dRet;
        }

        public double GetHghtPosY(int _iNodeIdx) 
        {
            //좌표상에서 계산.
            double dStartPos    = SEQ.HghtPtrn.GetScaleHghtPosY(0);
            double dIncTrgPos   = SEQ.HghtPtrn.GetScaleHghtPosY(_iNodeIdx) - dStartPos ;

            //모터상에서 계산.
            double dHghtSttPos  = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentHghtStt);
            double dRet         = dHghtSttPos + dIncTrgPos ;

            return dRet;
        }

        public double GetBltSubPosX(int _iNodeIdx) 
        {
            //좌표상에서 계산.
            double dStartPos    = SEQ.BltPtrn.GetBltPos(0).dSubPosX;
            double dIncTrgPos   = SEQ.BltPtrn.GetBltPos(_iNodeIdx).dSubPosX - dStartPos ;

            //모터상에서 계산.
            double dSttPos  = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtBltStt);
            double dRet     = dSttPos - dIncTrgPos ;

            return dRet;
        }

        public double GetBltSubPosY(int _iNodeIdx) 
        {
            //좌표상에서 계산.
            double dStartPos    = SEQ.BltPtrn.GetBltPos(0).dSubPosY;
            double dIncTrgPos   = SEQ.BltPtrn.GetBltPos(_iNodeIdx).dSubPosY - dStartPos ;

            //모터상에서 계산.
            double dSttPos  = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentBltStt);
            double dRet     = dSttPos + dIncTrgPos ;

            return dRet;
        }

        public double GetBltDiePosX(int _iNodeIdx) 
        {
            //좌표상에서 계산.
            double dStartPos    = SEQ.BltPtrn.GetBltPos(0).dSubPosX;
            double dIncTrgPos   = SEQ.BltPtrn.GetBltPos(_iNodeIdx).dDiePosX - dStartPos ;

            //모터상에서 계산.
            double dSttPos  = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtBltStt);
            double dRet     = dSttPos - dIncTrgPos ;

            return dRet;
        }

        public double GetBltDiePosY(int _iNodeIdx) 
        {
            //좌표상에서 계산.
            double dStartPos    = SEQ.BltPtrn.GetBltPos(0).dSubPosY;
            double dIncTrgPos   = SEQ.BltPtrn.GetBltPos(_iNodeIdx).dDiePosY - dStartPos ;

            //모터상에서 계산.
            double dSttPos  = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentBltStt);
            double dRet     = dSttPos + dIncTrgPos ;

            return dRet;
        }
        
        //밑에 부터 작업.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip = cs.RetFail) 
        {
            r = 0 ;
            c = 0 ;
            DM.ARAY[_iId].FindFrstRowCol(_iChip, ref c , ref r);             
            return (c >= 0 && r >= 0) ? true : false;
        }       

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 8000))
            {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
                //Step.iHome = 0 ;
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Log.Trace(m_sPartName, sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if(Stat.bReqStop) {
                //Step.iHome = 0;
                //return true ;
            }
            
            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    if(Step.iHome != PreStep.iHome)Log.Trace(m_sPartName, sTemp);
                    return true ;
            
                case 10:
                    MT_SetServo(mi.TOOL_ZDisp, true);
                    MT_SetServo(mi.TOOL_ZVisn, true);
                    MT_SetServo(mi.TOOL_ZPckr, true);
                    MT_SetServo(mi.TOOL_XRght, true);
                    MT_SetServo(mi.TOOL_XLeft, true);
                    MT_SetServo(mi.TOOL_YVisn, true);
                    //겐트리는 서보온 하고 나서 쓰레드 한바퀴 돌고 나서 서브축의 겟스탑 확인 하여 
                    //스탑이 아직 켜져 있으면 에러.
                    MT_SetServo(mi.TOOL_YGent, true);

                    MoveCyl(ci.TOOL_GuidFtDwUp, fb.Bwd);
                    MoveCyl(ci.TOOL_GuidRrDwUp, fb.Bwd);
                    Step.iHome++;
                    return false ;

                case 11:
                    if(!CL_Complete(ci.TOOL_GuidFtDwUp , fb.Bwd)) return false ;
                    if(!CL_Complete(ci.TOOL_GuidRrDwUp , fb.Bwd)) return false ;

                    if(MT_GetStop(mi.TOOL_YRsub)) {
                        ER_SetErr(ei.MTR_Alarm , "툴 Y축 겐트리 세팅이 이상합니다.");
                        return true ;
                    }                   
                    
                    MT_GoHome(mi.TOOL_ZDisp);
                    MT_GoHome(mi.TOOL_ZVisn);
                    MT_GoHome(mi.TOOL_ZPckr);
                    Step.iHome++;
                    return false ;

                case 12:
                    if(!MT_GetHomeDone(mi.TOOL_ZDisp))return false ;
                    if(!MT_GetHomeDone(mi.TOOL_ZVisn))return false ;
                    if(!MT_GetHomeDone(mi.TOOL_ZPckr))return false ;
                    MT_GoHome(mi.TOOL_XRght);
                    MT_GoHome(mi.TOOL_XLeft);
                    MT_GoHome(mi.TOOL_YVisn);
                    MT_GoHome(mi.TOOL_YGent);
                    Step.iHome++;
                    return false ;

                case 13:
                    if(!MT_GetHomeDone(mi.TOOL_XRght))return false ;
                    if(!MT_GetHomeDone(mi.TOOL_XLeft))return false ;
                    if(!MT_GetHomeDone(mi.TOOL_YVisn))return false ;
                    if(!MT_GetHomeDone(mi.TOOL_YGent))return false ;
                    MT_SetHomeDone(mi.TOOL_YRsub, true);
                    MT_SetHomeDone(mi.Spare17   , true);
                    MT_GoAbsMan(mi.TOOL_ZDisp, pv.TOOL_ZDispWait);
                    MT_GoAbsMan(mi.TOOL_ZVisn, pv.TOOL_ZVisnWait);
                    MT_GoAbsMan(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);

                    Step.iHome++;
                    return false ;
            
                case 14:
                    if (!MT_GetStopPos(mi.TOOL_ZDisp,pv.TOOL_ZDispWait)) return false;
                    if (!MT_GetStopPos(mi.TOOL_ZVisn,pv.TOOL_ZVisnWait)) return false;
                    if (!MT_GetStopPos(mi.TOOL_ZPckr,pv.TOOL_ZPckrWait)) return false;
                    MoveCyl(ci.TOOL_DispCvFwBw, fb.Bwd);
                    Step.iHome++;
                    return false;

                case 15:
                    if (!CL_Complete(ci.TOOL_DispCvFwBw, fb.Bwd)) return false;
                    MT_GoAbsMan(mi.TOOL_XRght, pv.TOOL_XRghtWait );
                    MT_GoAbsMan(mi.TOOL_XLeft, pv.TOOL_XLeftWait );
                    MT_GoAbsMan(mi.TOOL_YVisn, pv.TOOL_YVisnWait );
                    MT_GoAbsMan(mi.TOOL_YGent, pv.TOOL_YGentWait );

                    Step.iHome++;
                    return false;

                case 16:                    
                    if (!MT_GetStopPos(mi.TOOL_XRght, pv.TOOL_XRghtWait)) return false;
                    if (!MT_GetStopPos(mi.TOOL_XLeft, pv.TOOL_XLeftWait)) return false;
                    if (!MT_GetStopPos(mi.TOOL_YVisn, pv.TOOL_YVisnWait)) return false;
                    if (!MT_GetStopPos(mi.TOOL_YGent, pv.TOOL_YGentWait)) return false;

                    Step.iHome = 0;
                    return true ;
            }
        }

        string GetS(double _dVal)
        {
            return string.Format("{0:0.000}", _dVal);
        }

        int iSubsAlignInspCnt = 0 ;
        public bool CycleSubsAlignVisn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 15000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;

            const int iMaxInspCnt = 4 ;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    SPC.WRK.DataClear();
                    SPC.WRK.Data.Device    = OM.GetCrntDev();
                    SPC.WRK.Data.LotNo     = LOT.GetLotNo();
                    SPC.WRK.Data.StartTime = DateTime.Now.ToOADate();

                    iSubsAlignInspCnt = 0 ;
                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;

                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;

                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentPkPlce      );
                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnRtM);                    
                    MoveMotr(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnRtM  );
                    MoveMotr(mi.TOOL_ZVisn ,pv.TOOL_ZVisnSStgSbsWork );

                    if(!SEQ.VisnRB.SendCommand(VisnCom.ci.SUBSALIGN.ToString())){ //SubsAlign 으로 세팅.
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentPkPlce      )) return false ;
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnRtM)) return false ;                    
                    if(!MT_GetStopPos(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnRtM  )) return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn ,pv.TOOL_ZVisnSStgSbsWork )) return false ;
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Command)) return false ;
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(iVisnDelay)) return false ;
                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }

                    Step.iCycle++;
                    return false;

                case 15:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnLtM);
                    MoveMotr(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnLtM  );
                    Step.iCycle++;
                    return false;
                    
                case 16:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnLtM)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YVisnSStgVsnLtM  )) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!m_tmDelay.OnDelay(iVisnDelay)) return false ;

                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    if(!SEQ.VisnRB.GetInspOk()){ //검사
                        SM.ER_SetErr(ei.VSN_InspNG , VisnCom.ci.SUBSALIGN.ToString() + " 비전 검사 실패");
                        return true ;
                    }

                    string sRet ="";
                    if (!SEQ.VisnRB.LoadRsltSubsAlign(ref sRet) )
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.SUBSALIGN.ToString() + " Align 결과값 파일로딩 실패");
                        return true;
                    }

                    Program.SendListMsg(sRet);

                    if(!VisnCom.GetRsltFromString(sRet , out RsltSubsAlign))
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.SUBSALIGN.ToString() + " Align 결과값 파싱 실패");
                        return true;
                    }

                    double dOriAngle  = CMath.GetLineAngle(PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnRtM),   
                                                           PM_GetValue(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnRtM  ),
                                                           PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnLtM),
                                                           PM_GetValue(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnLtM  ));

                    double dInspAngle = CMath.GetLineAngle(PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnRtM) + RsltSubsAlign.dMainX ,
                                                           PM_GetValue(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnRtM  ) + RsltSubsAlign.dMainY ,
                                                           PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnLtM) + RsltSubsAlign.dSubX  ,
                                                           PM_GetValue(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnLtM  ) + RsltSubsAlign.dSubY  );
                    //왼쪽에서 오른쪽으로 가는 각도는 1도 + 일때는 1 1도- 일때는 359도 이렇게 표현되서 바꿈
                    if(dOriAngle  >315) dOriAngle  -= 360  ;
                    if(dInspAngle >315) dInspAngle -= 360  ;
                    double dVisnT = dOriAngle - dInspAngle ;

                    if(Math.Abs(RsltSubsAlign.dMainX) > OM.DevOptn.dVisnTolXY){
                        ER_SetErr(ei.VSN_InspRangeOver , "Substrate Align 비젼의 X보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }
                    if(Math.Abs(RsltSubsAlign.dMainY) > OM.DevOptn.dVisnTolXY){
                        ER_SetErr(ei.VSN_InspRangeOver , "Substrate Align 비젼의 Y보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }
                    if(Math.Abs(dVisnT) > OM.DevOptn.dVisnTolAng){
                        ER_SetErr(ei.VSN_InspRangeOver , "Substrate Align 비젼의 T보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }

                    //맨마지막은 그냥 확인만 하는것으로 함.
                    if (iMaxInspCnt-1 != iSubsAlignInspCnt)
                    {
                        // 서브스트레이트 센터에서 현재 검사 포지션까지의 세팅값 거리.
                        // Y축이 2개 달려 있어서 둘다 계산 해야함.
                        double dDistToolInspToCntrX = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtVsSStgCtr) - PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtVsSStgVsnRtM);
                        double dDistToolInspToCntrY = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentVsSStgCtr) - PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentPkPlce);
                        double dDistVisnInspToWorkY = PM_GetValue(mi.TOOL_YVisn, pv.TOOL_YVisnWork) - PM_GetValue(mi.TOOL_YVisn, pv.TOOL_YVisnSStgVsnRtM);

                        //비전값까지 같이 계산해서 넣어준다.
                        double dDistCntX = dDistToolInspToCntrX + RsltSubsAlign.dMainX;
                        double dDistCntY = dDistToolInspToCntrY + dDistVisnInspToWorkY + RsltSubsAlign.dMainY;

                        double dMoveX;
                        double dMoveY;
                        //CMath.GetRotPnt(-dVisnT , dDistCntX , dDistCntY , out dMoveX , out dMoveY);
                        CMath.GetRotPnt(-dVisnT, -dDistCntX, dDistCntY, out dMoveX, out dMoveY);
                        Program.SendListMsg("RT:" + GetS(-dVisnT) + " CX:" + GetS(-dDistCntX) + " CY:" + GetS(dDistCntY) + " MX:" + GetS(dMoveX) + "MY:" + GetS(dMoveY));
                        SEQ.SSTG.UVW.GoInc(dMoveX + RsltSubsAlign.dMainX, -dMoveY - RsltSubsAlign.dMainY, -dVisnT);
                        Program.SendListMsg("MX:" + GetS(dMoveX + RsltSubsAlign.dMainX) + " MY:" + GetS(-dMoveY - RsltSubsAlign.dMainY) + " MT:" + GetS(-dVisnT));
                    }
                    Step.iCycle++;
                    return false ;

                case 19:
                    if(!SEQ.SSTG.UVW.GetStop()) return false ;
                    iSubsAlignInspCnt++;
                    if(iMaxInspCnt > iSubsAlignInspCnt){
                        Step.iCycle=11;
                        return false ;
                    }

                    OM.EqpStat.dSstgFtX = MT_GetCmdPos(mi.SSTG_XFrnt);
                    OM.EqpStat.dSstgRtY = MT_GetCmdPos(mi.SSTG_YRght);
                    OM.EqpStat.dSstgLtY = MT_GetCmdPos(mi.SSTG_YLeft);

                    int iAray = ri.SSTG;
                    SEQ.SSTG.FindChip(out iAray , out c , out r );
                    DM.ARAY[ri.SSTG].SetStat(c,r,cs.SubHeight);

                    Step.iCycle=0;
                    return true;
            }
        }
            
        //높이 측정값 갯수가 가변이여서 리스트를 이용.
        List<double> ActValues ;
        int iCrntNode = 0 ;
        public bool CycleHeight()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;
            double dXPos = 0 ;
            double dYPos = 0 ;
            double dVal  = 0.0;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    if(DM.ARAY[ri.SSTG].GetStat(c , r)==cs.SubHeight) ActValues = SubValues ;
                    else                                              ActValues = EndValues ;

                    ActValues.Clear();

                    iCrntNode = 0 ;

                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;

                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    Step.iCycle++;
                    return false ;

                //밑에서씀.
                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;

                    dXPos = GetHghtPosX(iCrntNode);
                    dYPos = GetHghtPosY(iCrntNode);

                    MT_GoAbsRun(mi.TOOL_XRght, dXPos);
                    MT_GoAbsRun(mi.TOOL_YGent, dYPos);

                    Step.iCycle++;
                    return false; 

                case 13:
                    if(!MT_GetStop(mi.TOOL_XRght))return false ;
                    if(!MT_GetStop(mi.TOOL_YGent))return false ;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    
                    if(!m_tmDelay.OnDelay(100)) return false ;
                    SEQ.HeightSnsr.SendCheckHeight();
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!SEQ.HeightSnsr.IsReceiveEnd()) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!m_tmDelay.OnDelay(100)) return false ;//일단 야매로 딜레이 넣음 이거 없으면 높이측정값 전에 측정한값을 가져옴.
                    dVal = SEQ.HeightSnsr.GetHeight();

                    if(dVal == 99){
                        ER_SetErr(ei.HGT_RangeErr , "높이측정을 실패 했습니다.");
                        return true ;
                    }

                    ActValues.Add(dVal);

                    iCrntNode++;
                    if(iCrntNode < SEQ.HghtPtrn.GetHghtPosCnt()){
                        Step.iCycle=12;
                        return false ;
                    }

                    //제일 높은곳으로 서브스트레이트 높이 설정함.
                    double dMinVal =  999 ;
                    double dMaxVal = -999 ;
                    foreach(var Val in ActValues){
                        if(dMaxVal < Val) dMaxVal = Val ;
                        if(dMinVal > Val) dMinVal = Val ;
                    }
                    Program.SendListMsg("Height MinMaxVal:" +GetS(dMinVal) +","+ GetS(dMaxVal));

                    int iAray = ri.SSTG;
                    SEQ.SSTG.FindChip(out iAray , out c , out r); 
                    if(DM.ARAY[ri.SSTG].GetStat(c , r)==cs.SubHeight) { //서브스트레이트 측정할때
                        OM.EqpStat.dSubstrateHeight = dMaxVal;
                        if(dMaxVal - dMinVal > OM.DevOptn.dSubMinFlat){
                            ER_SetErr(ei.HGT_RangeOver , "서브스트레이트 평탄도가 설정값을 넘었습니다.");
                            return true ;
                        }
                        SPC.WRK.Data.SubHeight = "" ;
                        foreach(var Val in ActValues){
                            SPC.WRK.Data.SubHeight += string.Format("{0:0.###}", Val);
                            SPC.WRK.Data.SubHeight += " ";                            
                        }
                        SPC.WRK.Data.SubHeight.Trim();

                        DM.ARAY[ri.SSTG].SetStat(c , r , cs.Disp );
                    }
                    else {
                        if(SubValues.Count != EndValues.Count){
                            ER_SetErr(ei.HGT_RangeErr , "서브스트레이트 측정 갯수와 다이 측정갯수가 다릅니다.");
                            return true ;
                        }

                        SPC.WRK.Data.GapHeight = "";
                        for(int i = 0 ; i < SubValues.Count ; i++){
                            dVal = EndValues[i] - SubValues[i];
                            dVal -= OM.DevInfo.dWFER_Tickness ;
                            if(dVal > OM.DevOptn.dEndMinFlat){
                                ER_SetErr(ei.HGT_RangeErr , "Epoxy두께가 설정값을 넘었습니다.");
                                return true ;
                            }
                            SPC.WRK.Data.GapHeight += string.Format("{0:0.###}", dVal);
                            SPC.WRK.Data.GapHeight += " ";           
                        }

                        SPC.WRK.Data.EndHeight= "" ;
                        foreach(var Val in EndValues){
                            SPC.WRK.Data.EndHeight += string.Format("{0:0.###}", Val);
                            SPC.WRK.Data.EndHeight += " ";                            
                        }
                        SPC.WRK.Data.EndHeight.Trim();
                        DM.ARAY[ri.SSTG].SetStat(c , r , cs.BltHeight);
                    }
                    Step.iCycle = 0 ;
                    return true ;
            }
        }


        int iDispPatternIdx = 0 ;
        public bool CycleDispense()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                IO_SetY(yi.TOOL_DispOnOff , false);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;
            double dXOfs = 0 ;
            double dYOfs = 0 ;

            double dXPos = 0 ;
            double dYPos = 0 ;
            double dZPos = 0 ;

            const int iCoordId = 0;

            int [] iaMotorAxis = new int [3] ;
            iaMotorAxis[0] = (int)mi.TOOL_YGent ;
            iaMotorAxis[1] = (int)mi.TOOL_XRght ; 
            iaMotorAxis[2] = (int)mi.TOOL_ZDisp ;

            double [] daDispPos = new double [3];
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    IO_SetY(yi.TOOL_DispOnOff , false);
                    return true;

                case 10:
                    iDispPatternIdx = 1 ;

                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveCyl(ci.TOOL_DispCvFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!CL_Complete(ci.TOOL_DispCvFwBw)) return false;
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispMove);
                    Step.iCycle++;
                    return false;

                

                case 12:                    
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispMove))return false ;
                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;
                    if(OM.DevOptn.iDispShotDelay == 0) {
                        Step.iCycle = 17;
                        return false;
                    }
                    MoveMotr(mi.TOOL_XRght, pv.TOOL_XRghtWait);
                    MoveMotr(mi.TOOL_YGent, pv.TOOL_YGentWait);
                    
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.TOOL_XRght, pv.TOOL_XRghtWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent, pv.TOOL_YGentWait))return false ;
                    IO_SetY(yi.TOOL_DispOnOff, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!m_tmDelay.OnDelay(OM.DevOptn.iDispShotDelay)) return false;
                    IO_SetY(yi.TOOL_DispOnOff, false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!m_tmDelay.OnDelay(OM.DevOptn.iDispAtShotDelay)) return false;
                    Step.iCycle++;
                    return false;
                //위에서씀
                case 17:
                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtDispStt);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentDispStt);

                    MoveCyl(ci.TOOL_DispCvFwBw,fb.Bwd);

                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtDispStt)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentDispStt)) return false ;
                    if(!CL_Complete(ci.TOOL_DispCvFwBw,fb.Bwd)) return false ;

                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispCheck , SEQ.DispPtrn.GetDispPosZ(iDispPatternIdx) - OM.EqpStat.dSubstrateHeight);

                    Step.iCycle++;
                    return false;

                case 19:
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispCheck , SEQ.DispPtrn.GetDispPosZ(iDispPatternIdx) - OM.EqpStat.dSubstrateHeight)) return false ;
                    //시작 포지션 도착.
                    

                    Step.iCycle++;
                    return false ;

                //밑에서 씀.
                case 20:
                    if(!OM.CmnOptn.bNoDisp) {
                        IO_SetY(yi.TOOL_DispOnOff , SEQ.DispPtrn.GetDispOn(iDispPatternIdx));
                    }
                    else {
                        IO_SetY(yi.TOOL_DispOnOff , false);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 21:
                    if(!m_tmDelay.OnDelay(SEQ.DispPtrn.GetBfDelay(iDispPatternIdx))) return false;
                    MT_ContiSetAxisMap(mi.TOOL_YGent , iCoordId , 3 , iaMotorAxis);
                    MT_ContiSetAbsRelMode(mi.TOOL_YGent ,iCoordId, 0) ; //0:절대모드 1:상대모드

                    double dDispCntrX = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtDispStt) + SEQ.DispPtrn.GetScaleDispPosX(0);
                    double dDispCntrY = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentDispStt) - SEQ.DispPtrn.GetScaleDispPosY(0);
                    dXPos = GetDispPosX(iDispPatternIdx) ;
                    dYPos = GetDispPosY(iDispPatternIdx) ;
                    
                    dZPos  = PM_GetValue(mi.TOOL_ZDisp , pv.TOOL_ZDispCheck) ;
                    dZPos -= OM.EqpStat.dSubstrateHeight ;
                    dZPos += SEQ.DispPtrn.GetDispPosZ(iDispPatternIdx) ;

                    daDispPos[0] = dYPos ;
                    daDispPos[1] = dXPos ;
                    daDispPos[2] = dZPos ;

                    

                    

                    MT_LineMove(mi.TOOL_YGent  ,
                                iCoordId       ,
                                daDispPos      ,
                                SEQ.DispPtrn.GetSpeed(iDispPatternIdx) ,
                                SEQ.DispPtrn.GetAcc  () ,
                                SEQ.DispPtrn.GetDec  ());

                    Step.iCycle++;
                    return false; 
                    
                case 22:
                    if(!MT_GetStop(mi.TOOL_XRght)) return false ;
                    if(!MT_GetStop(mi.TOOL_YGent)) return false ;
                    if(!MT_GetStop(mi.TOOL_ZDisp)) return false ;
                    
                    m_tmDelay.Clear();
                    //에프터 딜레이 설정되어 있으면 토출 종료 하고 먹게.
                    if(SEQ.DispPtrn.GetAtDelay(iDispPatternIdx) > 0) {
                        IO_SetY(yi.TOOL_DispOnOff , false);
                    }
                    Step.iCycle++;
                    return false ;

                case 23:
                    if(!m_tmDelay.OnDelay(SEQ.DispPtrn.GetAtDelay(iDispPatternIdx))) return false; 
                    iDispPatternIdx++;
                    if(SEQ.DispPtrn.GetDispPosCnt() > iDispPatternIdx)
                    {
                        Step.iCycle = 20 ;
                        return false ;
                    }

                    IO_SetY(yi.TOOL_DispOnOff , false);
                    
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    Step.iCycle++;
                    return false ;

                case 24:
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait)) return false ;
                    MoveCyl(ci.TOOL_DispCvFwBw,fb.Fwd);
                    Step.iCycle++;
                    return false ;

                case 25:
                    if(!CL_Complete(ci.TOOL_DispCvFwBw,fb.Fwd)) return false ;
                    

                    int iAray = ri.SSTG;
                    SEQ.SSTG.FindChip(out iAray , out c , out r );
                    DM.ARAY[ri.SSTG].SetStat(c,r,cs.DispVisn);


                    Step.iCycle=0;
                    return true;
            }
        }

        public bool CycleDispenseVisn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                IO_SetY(yi.TOOL_DispOnOff , false);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
            }

            double dPosX = 0 ;
            double dPosY = 0 ;

            int r = 0, c = 0;            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    IO_SetY(yi.TOOL_DispOnOff , false);
                    return true;

                case 10:
                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork);

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn ,pv.TOOL_ZVisnSStgSbsWork)) return false ;

                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;                    
                    SEQ.VisnRB.SendCommand(VisnCom.ci.DISPENSE.ToString());                    

                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtDispVisn1);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentDispVisn1);
                    MoveMotr(mi.TOOL_YVisn , pv.TOOL_YVisnWork);

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtDispVisn1)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentDispVisn1)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YVisn ,pv.TOOL_YVisnWork     )) return false ;
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Command)) return false ;
                    //m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 14:
                    //if(!m_tmDelay.OnDelay(iVisnDelay)) return false ;
                    SEQ.VisnRB.SendInsp();
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    if(!SEQ.VisnRB.GetInspOk() && !OM.CmnOptn.bNoDisp){
                        ER_SetErr(ei.VSN_InspNG , "Dispense1 검사 NG");
                        return true ;
                    }

                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtDispVisn2);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentDispVisn2);
                    SEQ.VisnRB.SendCommand(VisnCom.ci.DISPENSE.ToString());   
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtDispVisn2)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentDispVisn2)) return false ;
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Command)) return false ;
                    //m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 17:
                    //if(!m_tmDelay.OnDelay(iVisnDelay)) return false ;
                    SEQ.VisnRB.SendInsp();
                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    if(!SEQ.VisnRB.GetInspOk()&& !OM.CmnOptn.bNoDisp){
                        ER_SetErr(ei.VSN_InspNG , "Dispense2 검사 NG");
                        return true ;
                    }

                    int iAray = ri.SSTG;
                    SEQ.SSTG.FindChip(out iAray , out c , out r );
                    DM.ARAY[ri.SSTG].SetStat(c,r,cs.Attach);


                    Step.iCycle=0;
                    return true;
            }
        }       


        public bool CycleEject()
        {
            String sTemp;
             
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true;
            }

            int r = 0, c = 0;
            int iWAray = ri.WSTG;
            SEQ.WSTG.FindChip(out iWAray, out c, out r);

            double dXOfs = c * OM.DevInfo.dWFER_DiePitchX + OM.EqpStat.dDieVisnPosOfsX;
            double dYOfs = r * OM.DevInfo.dWFER_DiePitchY + OM.EqpStat.dDieVisnPosOfsY; 
            

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    
                    MoveMotr(mi.TOOL_ZEjtr, pv.TOOL_ZEjtrWait);
                    MoveMotr(mi.TOOL_XRght, pv.TOOL_XRghtWait);
                    MoveCyl(ci.TOOL_GuidFtDwUp, fb.Bwd);
                    MoveCyl(ci.TOOL_GuidRrDwUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!MT_GetStopPos(mi.TOOL_ZEjtr, pv.TOOL_ZEjtrWait)) return false;
                    if (!MT_GetStopPos(mi.TOOL_XRght, pv.TOOL_XRghtWait)) return false;
                    if (!CL_Complete(ci.TOOL_GuidFtDwUp, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TOOL_GuidRrDwUp, fb.Bwd)) return false;
                    if (!OM.CmnOptn.bUseGuideEjct)
                    {
                        Step.iCycle = 14;
                        return false;
                    }
                    MoveMotr(mi.TOOL_XLeft, pv.TOOL_XLeftPkPickStt, dXOfs);
                    MoveMotr(mi.TOOL_YGent, pv.TOOL_YGentPkPickStt, dYOfs);
                    
                    Step.iCycle++;
                    return false ;

                case 12:
                    if (!MT_GetStopPos(mi.TOOL_XLeft, pv.TOOL_XLeftPkPickStt, dXOfs)) return false;
                    if (!MT_GetStopPos(mi.TOOL_YGent, pv.TOOL_YGentPkPickStt, dYOfs)) return false;
                    MoveCyl(ci.TOOL_GuidFtDwUp, fb.Fwd);
                    MoveCyl(ci.TOOL_GuidRrDwUp, fb.Fwd);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!CL_Complete(ci.TOOL_GuidFtDwUp, fb.Fwd)) return false;
                    if(!CL_Complete(ci.TOOL_GuidRrDwUp, fb.Fwd)) return false;

                    Step.iCycle++;
                    return false;
                //위에서씀
                case 14:
                    MoveMotr(mi.TOOL_YEjtr, pv.TOOL_YEjtrWorkStt, r * OM.DevInfo.dWFER_DiePitchY);
                    MoveMotr(mi.TOOL_XEjtL, pv.WSTG_XEjtLStart,-c * OM.DevInfo.dWFER_DiePitchX);
                    MoveMotr(mi.TOOL_XEjtR, pv.WSTG_XEjtRStart, c * OM.DevInfo.dWFER_DiePitchX);

                    Step.iCycle++;
                    return false;

                case 15:
                    if (!MT_GetStopPos(mi.TOOL_YEjtr, pv.TOOL_YEjtrWorkStt, r * OM.DevInfo.dWFER_DiePitchY)) return false;
                    if (!MT_GetStopPos(mi.TOOL_XEjtL, pv.WSTG_XEjtLStart,-c * OM.DevInfo.dWFER_DiePitchX)) return false;
                    if (!MT_GetStopPos(mi.TOOL_XEjtR, pv.WSTG_XEjtRStart, c * OM.DevInfo.dWFER_DiePitchX)) return false;                    


                    Step.iCycle++;
                    return false;

                case 16:
                    MoveMotr(mi.TOOL_ZEjtr, pv.TOOL_ZEjtrWork);                    
                    Step.iCycle++;
                    return false;

                case 17:                    
                    if (!MT_GetStopPos(mi.TOOL_ZEjtr, pv.TOOL_ZEjtrWork)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 18:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iEjectAtUpDelay)) return false;
                    //MoveMotr(mi.TOOL_XEjtL, pv.WSTG_XEjtLEnd,-c * OM.DevInfo.dWFER_DiePitchX);
                    //MoveMotr(mi.TOOL_XEjtR, pv.WSTG_XEjtREnd, c * OM.DevInfo.dWFER_DiePitchX);
                    MT_GoAbsVel(mi.TOOL_XEjtL, PM_GetValue(mi.TOOL_XEjtL, pv.WSTG_XEjtLEnd) - c * OM.DevInfo.dWFER_DiePitchX,OM.DevOptn.dEjectSpeed);
                    MT_GoAbsVel(mi.TOOL_XEjtR, PM_GetValue(mi.TOOL_XEjtR, pv.WSTG_XEjtREnd) + c * OM.DevInfo.dWFER_DiePitchX,OM.DevOptn.dEjectSpeed);
                    Step.iCycle++;
                    return false;

      
                case 19:
                    if (!MT_GetStopPos(mi.TOOL_XEjtL, pv.WSTG_XEjtLEnd,-c * OM.DevInfo.dWFER_DiePitchX)) return false;
                    if (!MT_GetStopPos(mi.TOOL_XEjtR, pv.WSTG_XEjtREnd, c * OM.DevInfo.dWFER_DiePitchX)) return false;
                    MoveMotr(mi.TOOL_ZEjtr, pv.TOOL_ZEjtrWait);
                    MoveCyl(ci.TOOL_GuidFtDwUp, fb.Bwd);
                    MoveCyl(ci.TOOL_GuidRrDwUp, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!MT_GetStopPos(mi.TOOL_ZEjtr, pv.TOOL_ZEjtrWait)) return false;
                    if(!CL_Complete(ci.TOOL_GuidFtDwUp, fb.Bwd)) return false;
                    if(!CL_Complete(ci.TOOL_GuidRrDwUp, fb.Bwd)) return false;
                    MoveMotr(mi.TOOL_YEjtr, pv.TOOL_YEjtrWait);
                    MoveMotr(mi.TOOL_XEjtL, pv.WSTG_XEjtLWait);
                    MoveMotr(mi.TOOL_XEjtR, pv.WSTG_XEjtRWait);

                    MoveMotr(mi.TOOL_XLeft, pv.TOOL_XLeftWait);
                    Step.iCycle++;
                    return false;

                case 21:
                    if(!MT_GetStopPos(mi.TOOL_YEjtr, pv.TOOL_YEjtrWait)) return false;
                    if(!MT_GetStopPos(mi.TOOL_XEjtL, pv.WSTG_XEjtLWait)) return false;
                    if(!MT_GetStopPos(mi.TOOL_XEjtR, pv.WSTG_XEjtRWait)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_XLeft, pv.TOOL_XLeftWait)) return false ;

                    DM.ARAY[ri.WSTG].SetStat(c,r,cs.Align);

                    
                    
                    Step.iCycle = 0;
                    return true;
            }
        }


        int iDieAlignInspCnt = 0 ;
        double dMasterInspPosX = 0;
        double dMasterInspPosY = 0;
        public bool CycleDieAlignVisn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;
            int iWAray = ri.WSTG;
            SEQ.WSTG.FindChip(out iWAray , out c , out r);
            const int iMaxInspCnt = 3 ;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    
                   

                    
                    if(DM.ARAY[iWAray].GetStat(c,r) == cs.EjectAlign && !OM.CmnOptn.bUseGuideEjct) {
                        DM.ARAY[ri.WSTG].SetStat(c,r,cs.Eject);
                        Step.iCycle = 0 ;
                        return true ;
                    }

                    iDieAlignInspCnt = 0 ;
                    

                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnWStgWork);

                    SEQ.WSTG.MoveMotr(mi.WSTG_TTble, pv.WSTG_TTbleWfrWork);
                    //가이드 이젝트를 안쓰면 무조건 첫검사고.
                    //가이드 이젝트를 쓰면 이젝트 얼라인일때가 처음이라 초기화
                    if(DM.ARAY[iWAray].GetStat(c,r) == cs.EjectAlign || !OM.CmnOptn.bUseGuideEjct) {                        
                        OM.EqpStat.dDieVisnPosOfsX = 0;
                        OM.EqpStat.dDieVisnPosOfsY = 0;
                    }
                    

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnWStgWork)) return false ;
                    if(!MT_GetStop   (mi.WSTG_TTble)) return false ;
                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    Step.iCycle++;
                    return false ;


                    //칩피치 X,Y 확인 하기.

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;
                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnMStt,-c*OM.DevInfo.dWFER_DiePitchX -OM.EqpStat.dDieVisnPosOfsX);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnMStt, r*OM.DevInfo.dWFER_DiePitchY +OM.EqpStat.dDieVisnPosOfsY);
                    MoveMotr(mi.TOOL_YVisn ,pv.TOOL_YVisnWork         );

                    //SEQ.VisnRB
                    if(!SEQ.VisnRB.SendCommand(VisnCom.ci.DIEALIGN.ToString())){ //다이얼라인 으로 세팅.
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnMStt,-c*OM.DevInfo.dWFER_DiePitchX -OM.EqpStat.dDieVisnPosOfsX)) return false ;                    
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnMStt, r*OM.DevInfo.dWFER_DiePitchY +OM.EqpStat.dDieVisnPosOfsY)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YVisn ,pv.TOOL_YVisnWork         )) return false ;
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Command)) return false ;

                    //마스터 검사한 위치를 저장해 놓는다.
                    dMasterInspPosX = MT_GetCmdPos(mi.TOOL_XRght);
                    dMasterInspPosY = MT_GetCmdPos(mi.TOOL_YGent);

                    Program.SendListMsg("MasterPosX="+GetS(dMasterInspPosX) + " MasterPosY="+GetS(dMasterInspPosY));

                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(iVisnDelay)) return false ;
                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }

                    Step.iCycle++;
                    return false;

                case 15:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;

                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnSStt,-c*OM.DevInfo.dWFER_DiePitchX -OM.EqpStat.dDieVisnPosOfsX);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnSStt, r*OM.DevInfo.dWFER_DiePitchY +OM.EqpStat.dDieVisnPosOfsY);
                    Step.iCycle++;
                    return false;
                    
                case 16:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_YGentVsWStgVsnSStt,-c*OM.DevInfo.dWFER_DiePitchX -OM.EqpStat.dDieVisnPosOfsX)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnSStt, r*OM.DevInfo.dWFER_DiePitchY +OM.EqpStat.dDieVisnPosOfsY)) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!m_tmDelay.OnDelay(iVisnDelay)) return false ;
                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 18:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    if(!SEQ.VisnRB.GetInspOk()){ //검사
                        SM.ER_SetErr(ei.VSN_InspNG , VisnCom.ci.DIEALIGN.ToString() + " 비전 검사 실패");
                        return true ;
                    }
                    
                    string sRet ="";
                    if (!SEQ.VisnRB.LoadRsltDieAlign(ref sRet) )
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.DIEALIGN.ToString() + " Align 결과값 파일로딩 실패");
                        return true;
                    }
                    Program.SendListMsg(sRet);
                    if(!VisnCom.GetRsltFromString(sRet , out RsltDieAlign))
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.DIEALIGN.ToString() + " Align 결과값 파싱 실패");
                        return true;
                    }

                    //좌우 검사시에 180도가 나와야 함.
                    double dOriAngle  = CMath.GetLineAngle(PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnMStt),   
                                                           PM_GetValue(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnMStt),
                                                           PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnSStt),
                                                           PM_GetValue(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnSStt));
                    
                    double dInspAngle = CMath.GetLineAngle(PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnMStt) + RsltDieAlign.dMainX ,
                                                           PM_GetValue(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnMStt) + RsltDieAlign.dMainY ,
                                                           PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnSStt) + RsltDieAlign.dSubX  ,
                                                           PM_GetValue(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnSStt) + RsltDieAlign.dSubY  );
                    //왼쪽에서 오른쪽으로 가는 각도는 1도 + 일때는 1 1도- 일때는 359도 이렇게 표현되서 바꿈
                    if(dOriAngle  >315) dOriAngle  -= 360  ;
                    if(dInspAngle >315) dInspAngle -= 360  ;
                    double dVisnT = dOriAngle - dInspAngle ;

                    if(Math.Abs(RsltDieAlign.dMainX) > OM.DevOptn.dVisnTolXY){
                        ER_SetErr(ei.VSN_InspRangeOver , "Die Align 비젼의 X보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }
                    if(Math.Abs(RsltDieAlign.dMainY) > OM.DevOptn.dVisnTolXY){
                        ER_SetErr(ei.VSN_InspRangeOver , "Die Align 비젼의 Y보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }
                    if(Math.Abs(dVisnT) > OM.DevOptn.dVisnTolAng){
                        ER_SetErr(ei.VSN_InspRangeOver , "Die Align 비젼의 T보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }                                   

                    if (iDieAlignInspCnt != iMaxInspCnt-1)
                    {
                        // 웨이퍼스테이지 센터에서 현재 검사 포지션까지의 세팅값 거리.
                        // 4사분면으로 계산.
                        double dDistToolInspToCntrX = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtVsWStgCtr) - dMasterInspPosX;//+
                        double dDistToolInspToCntrY = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentVsWStgCtr) - dMasterInspPosY;//+
                        dDistToolInspToCntrX = -dDistToolInspToCntrX;

                        //비전값까지 같이 계산해서 넣어준다.
                        double dDistCntX = dDistToolInspToCntrX + RsltDieAlign.dMainX;
                        double dDistCntY = dDistToolInspToCntrY + RsltDieAlign.dMainY;   

                        double dAngleX = 0;
                        double dAngleY = 0;  


                        CMath.GetRotPnt(-dVisnT, dDistCntX, dDistCntY, out dAngleX, out dAngleY);
                        Program.SendListMsg("dAngleX=" + GetS(dAngleX) + " dAngleY=" + GetS(dAngleY));

                        MT_GoIncRun(mi.WSTG_TTble, -dVisnT);

                        OM.EqpStat.dDieVisnPosOfsX = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtVsWStgVsnMStt) - c * OM.DevInfo.dWFER_DiePitchX - dMasterInspPosX;
                        OM.EqpStat.dDieVisnPosOfsY = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentVsWStgVsnMStt) + r * OM.DevInfo.dWFER_DiePitchY - dMasterInspPosY;
                        OM.EqpStat.dDieVisnPosOfsY = -OM.EqpStat.dDieVisnPosOfsY;

                        OM.EqpStat.dDieVisnPosOfsX += dAngleX ;
                        OM.EqpStat.dDieVisnPosOfsY += dAngleY ;                        

                        OM.EqpStat.dDieVisnPosOfsX += RsltDieAlign.dMainX;
                        OM.EqpStat.dDieVisnPosOfsY += RsltDieAlign.dMainY;

                    }
                    else {
                        OM.EqpStat.dDieVisnPosOfsX = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtVsWStgVsnMStt) - c * OM.DevInfo.dWFER_DiePitchX - dMasterInspPosX;
                        OM.EqpStat.dDieVisnPosOfsY = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentVsWStgVsnMStt) + r * OM.DevInfo.dWFER_DiePitchY - dMasterInspPosY;
                        OM.EqpStat.dDieVisnPosOfsY = -OM.EqpStat.dDieVisnPosOfsY;

                        OM.EqpStat.dDieVisnPosOfsX += RsltDieAlign.dMainX;
                        OM.EqpStat.dDieVisnPosOfsY += RsltDieAlign.dMainY;
                    }

                    Program.SendListMsg("OM.EqpStat.dDieVisnPosOfsX=" + GetS(OM.EqpStat.dDieVisnPosOfsX) + " OM.EqpStat.dDieVisnPosOfsY=" + GetS(OM.EqpStat.dDieVisnPosOfsY));
                    

                    Step.iCycle++;
                    return false ;

                case 19:
                    if(!MT_GetStop(mi.WSTG_TTble)) return false ;
                    iDieAlignInspCnt++;
                    if(iMaxInspCnt > iDieAlignInspCnt){
                        Step.iCycle=11;
                        return false ;
                    }

                    PM.SetValue(mi.WSTG_TTble,pv.WSTG_TTbleWfrWork,MT_GetCmdPos(mi.WSTG_TTble));
                    PM.Save(OM.GetCrntDev());

                    SEQ.WSTG.FindChip(out iWAray,out c,out r);
                    if(DM.ARAY[iWAray].GetStat(c,r) == cs.EjectAlign) {
                        DM.ARAY[ri.WSTG].SetStat(c, r,cs.Eject);                        
                    } 
                    else {
                        DM.ARAY[ri.WSTG].SetStat(c, r, cs.Pick);
                    }
                    

                    Step.iCycle=0;
                    return true;
            }
        
        }

        static int iPreWstgC = 0; //Ver1.0.5.0 CycleTime 계산해야되서 추가함.
        static int iPreWstgR = 0; //Ver1.0.5.0 CycleTime 계산해야되서 추가함.
        static double dPreTime = 0.0;
        public bool CyclePick()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            //int r = 0, c = 0;
            int WstgR = 0, WstgC = 0;
            int PckrR = 0, PckrC = 0;
            int iWAray = ri.WSTG;
            SEQ.WSTG.FindChip(out iWAray , out WstgC , out WstgR);

            double dXOfs = WstgC * OM.DevInfo.dWFER_DiePitchX + OM.EqpStat.dDieVisnPosOfsX;
            double dYOfs = WstgR * OM.DevInfo.dWFER_DiePitchY + OM.EqpStat.dDieVisnPosOfsY; 

            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    //Ver 1.0.5.0
                    //CycleTime 계산
                    if(dPreTime == 0.0) dPreTime = DateTime.Now.ToOADate();
                    if (iPreWstgC != WstgC || iPreWstgR != WstgR)
                    {
                        double dCrntTime = DateTime.Now.ToOADate();
                        SPC.DAY.Data.dCycleTime = dCrntTime - dPreTime;
                        dPreTime = dCrntTime;
                        iPreWstgC = WstgC;
                        iPreWstgR = WstgR;
                    }



                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtWait))return false ;

                    MoveMotr(mi.TOOL_XLeft ,pv.TOOL_XLeftPkPickStt, dXOfs);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentPkPickStt, dYOfs);
                    
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XLeft ,pv.TOOL_XLeftPkPickStt, dXOfs)) return false ;                    
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentPkPickStt, dYOfs)) return false ;
                    MoveCyl(ci.TOOL_GuidFtDwUp, fb.Fwd);
                    MoveCyl(ci.TOOL_GuidRrDwUp, fb.Fwd);

                    Step.iCycle++;
                    return false;

                case 14:
                    if (!CL_Complete(ci.TOOL_GuidFtDwUp, fb.Fwd)) return false;
                    if (!CL_Complete(ci.TOOL_GuidRrDwUp, fb.Fwd)) return false;

                    MoveMotr(mi.TOOL_ZPckr ,pv.TOOL_ZPckrPick);

                    Step.iCycle++;
                    return false ;

                case 15:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrPick)) return false;

                    IO_SetY(yi.TOOL_PckrVac, true);
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPickDelay)) return false ;
                    if (IO_GetX(xi.TOOL_PckrVac))
                    {
                        MT_GoIncVel(mi.TOOL_ZPckr, OM.DevOptn.dPickUpFrstOfs, OM.DevOptn.dPickUpFrstSpeed);
                        Step.iCycle++;
                        return false;
                    }
                    Step.iCycle++;
                    return false;

                case 17:
                    if(!MT_GetStop(mi.TOOL_ZPckr)) return false;
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);
                    Step.iCycle++;
                    return false;
                    
                case 18:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait)) return false;                    
                    if (!IO_GetX(xi.TOOL_PckrVac))
                    {
                        MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);
                        ER_SetErr(ei.PCK_PickMiss, "Die Pick-Up Miss가 발생하였습니다.");
                        return true;
                    }

                    MoveCyl(ci.TOOL_GuidFtDwUp, fb.Bwd);
                    MoveCyl(ci.TOOL_GuidRrDwUp, fb.Bwd);

                    Step.iCycle++;
                    return false;

                case 19:
                    if (!CL_Complete(ci.TOOL_GuidFtDwUp, fb.Bwd)) return false;
                    if (!CL_Complete(ci.TOOL_GuidRrDwUp, fb.Bwd)) return false;
                    
              
                    SEQ.WSTG.FindChip(out iWAray , out WstgC , out WstgR );
                    DM.ARAY[ri.WSTG].SetStat(WstgC,WstgR,cs.Empty);

                    SEQ.TOOL.FindChip(ri.PCKR , out PckrC , out PckrR, cs.None);
                    DM.ARAY[ri.PCKR].SetStat(PckrC, PckrR, cs.BtmVisn);
                    //Ver 1.0.5.0
                    //Picker에 Wafer Stage에 있는 바코드 데이터를 옮긴다.
                    DM.ARAY[ri.PCKR].Chip[PckrC, PckrR].Data = DM.ARAY[ri.WSTG].Chip[WstgC, WstgR].Data ;

                    //Ver 1.0.5.0
                    //2018.08.09 CyclePick에서 SPC에 들어갈 Wafer 바코드 내용 집어넣는다.
                    //SPC.WRK.Data.WfrBarcode = DM.ARAY[iWAray].Chip[WstgC, WstgR].Data;

                    if (!SetWorkMapFile(ri.WSTG, WstgC, WstgR, OM.EqpStat.sWSTGBarcode))
                    {
                        ER_SetErr(ei.PRT_Barcode , "맵파일 데이터 X로 마스킹을 실패 했습니다. 맵파일 상태를 확인 하세요.");
                        return true;
                    }

                    Step.iCycle=0;
                    return true;
            }
        }

        public bool CycleBtmAlignVisn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    //MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnWStgWork);

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    //if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnWStgWork)) return false ;
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtWait))return false ;
                                        
                    MoveMotr(mi.TOOL_XLeft ,pv.TOOL_XLeftPkBVsnM);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentPkBVsnM);                    
                    MoveMotr(mi.TOOL_YVisn ,pv.TOOL_YVisnWork   );
                    //MoveMotr(mi.TOOL_ZVisn ,pv.TOOL_ZVisnSStgWfrWork  );

                    if(!SEQ.VisnRB.SendCommand(VisnCom.ci.BTMALIGN.ToString())){ //다이얼라인 으로 세팅.
                        ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XLeft ,pv.TOOL_XLeftPkBVsnM)) return false ;                    
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentPkBVsnM)) return false ;                    
                    if(!MT_GetStopPos(mi.TOOL_YVisn ,pv.TOOL_YVisnWork   )) return false ;
                    //if(!MT_GetStopPos(mi.TOOL_ZVisn ,pv.TOOL_ZVisnSStgWfrWork  )) return false ;
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Command)) return false ;

                    MoveMotr(mi.TOOL_ZPckr ,pv.TOOL_ZPckrBVisn);                   

                    Step.iCycle++;
                    return false ;

                case 14:                    
                    if(!MT_GetStopPos(mi.TOOL_ZPckr ,pv.TOOL_ZPckrBVisn   )) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;


                case 15:
                    if(!m_tmDelay.OnDelay(iVisnDelay)) return false ;
                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 16:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    MoveMotr(mi.TOOL_XLeft ,pv.TOOL_XLeftPkBVsnS);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentPkBVsnS);
                    Step.iCycle++;
                    return false;
                    
                case 17:
                    if(!MT_GetStopPos(mi.TOOL_XLeft ,pv.TOOL_XLeftPkBVsnS)) return false ;                    
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentPkBVsnS)) return false ; 
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!m_tmDelay.OnDelay(iVisnDelay)) return false ;
                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 19:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    if(!SEQ.VisnRB.GetInspOk()){ //검사
                        ER_SetErr(ei.VSN_InspNG , VisnCom.ci.BTMALIGN.ToString() + " 비전 검사 실패");
                        return true ;
                    }
                    
                    string sRet ="";
                    if (!SEQ.VisnRB.LoadRsltBtmAlign(ref sRet) )
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.BTMALIGN.ToString() + " Align 결과값 파일로딩 실패");
                        return true;
                    }                    

                    if(!VisnCom.GetRsltFromString(sRet , out RsltBtmAlign))
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.BTMALIGN.ToString() + " Align 결과값 파싱 실패");
                        return true;
                    }

                    //좌우 검사시에 180도가 나와야 함.
                    double dOriAngle  = CMath.GetLineAngle(PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnMStt),   
                                                           PM_GetValue(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnMStt),
                                                           PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnSStt),
                                                           PM_GetValue(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnSStt));

                    double dInspAngle = CMath.GetLineAngle(PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnMStt) + RsltBtmAlign.dMainX ,
                                                           PM_GetValue(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnMStt) + RsltBtmAlign.dMainY ,
                                                           PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsWStgVsnSStt) + RsltBtmAlign.dSubX  ,
                                                           PM_GetValue(mi.TOOL_YGent ,pv.TOOL_YGentVsWStgVsnSStt) + RsltBtmAlign.dSubY  );
                    //왼쪽에서 오른쪽으로 가는 각도는 1도 + 일때는 1 1도- 일때는 359도 이렇게 표현되서 바꿈
                    if(dOriAngle  >315) dOriAngle  -= 360  ;
                    if(dInspAngle >315) dInspAngle -= 360  ;
                    double dVisnT = dOriAngle - dInspAngle ;

                    if(Math.Abs(RsltBtmAlign.dMainX) > OM.DevOptn.dVisnTolXY){
                        ER_SetErr(ei.VSN_InspRangeOver , "BtmAlign Align 비젼의 X보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }
                    if(Math.Abs(RsltBtmAlign.dMainY) > OM.DevOptn.dVisnTolXY){
                        ER_SetErr(ei.VSN_InspRangeOver , "BtmAlign Align 비젼의 Y보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }
                    if(Math.Abs(dVisnT) > OM.DevOptn.dVisnTolAng){
                        ER_SetErr(ei.VSN_InspRangeOver , "BtmAlign Align 비젼의 T보정값이 설정범위를 넘었습니다.");
                        Step.iCycle = 0 ;
                        return true ;
                    }

                    // 웨이퍼스테이지 센터에서 현재 검사 포지션까지의 세팅값 거리.
                    double dDistToolInspToCntrX = PM_GetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsWStgCtr) - (PM_GetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsWStgVsnMStt) +c*OM.DevInfo.dDieWidth );
                    double dDistToolInspToCntrY = PM_GetValue(mi.TOOL_YGent , pv.TOOL_YGentVsWStgCtr) - (PM_GetValue(mi.TOOL_YGent , pv.TOOL_YGentVsWStgVsnMStt) +r*OM.DevInfo.dDieHeight);

                    //비전값까지 같이 계산해서 넣어준다.
                    double dDistCntX = dDistToolInspToCntrX + RsltDieAlign.dMainX ;
                    double dDistCntY = dDistToolInspToCntrY + RsltDieAlign.dMainY ;

                    CMath.GetRotPnt(-dVisnT , dDistCntX , dDistCntY , out OM.EqpStat.dDieVisnPosOfsX , out OM.EqpStat.dDieVisnPosOfsY);
                    SEQ.SSTG.UVW.GoInc(0 ,0 , OM.DevOptn.dUVWTOfs-dVisnT);

                    MoveMotr(mi.TOOL_ZPckr ,pv.TOOL_ZPckrWait); 

                    Step.iCycle++;
                    return false ;

                case 20:
                    if(!SEQ.SSTG.UVW.GetStop()) return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait)) return false ;

                    //int iAray = ri.SSTG;
                    //SEQ.SSTG.FindChip(out iAray , out c , out r );
                    DM.ARAY[ri.PCKR].SetStat(0,0,cs.Distance);

                    Step.iCycle=0;
                    return true;
            }
        
        }
                
        //비비기랑 Vision검사 까지.
        int m_iDistVisnInspCnt = 0 ;
        const int m_iDistVisnInspCntMax = 3 ;
        public bool CycleDistanceVisn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork);

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork)) return false ;
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtWait))return false ;
                    MoveMotr(mi.TOOL_XLeft ,pv.TOOL_XLeftPkPlce);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentPkPlce);           

                    
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XLeft ,pv.TOOL_XLeftPkPlce)) return false ;                    
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentPkPlce)) return false ; 
                    
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(300)) return false ;
                    SEQ.LoadCell.SetZero();
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!m_tmDelay.OnDelay(300)) return false ;

                    SEQ.LoadCell.WeightCheck();
                    Step.iCycle++;
                    return false ;

                case 16:
                    if (!SEQ.LoadCell.IsReceiveEnd()) return false;
                    if (SEQ.LoadCell.GetLoadCellData() * 1000 > 50 ) 
                    {
                        ER_SetErr(ei.PRT_LoadCell, "로드셀의 무게가 50g 이상 감지되었습니다. 무게영점조절이 실패하였습니다.");                   
                        return true ;
                    }

                    //MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispCheck , SEQ.DispPtrn.GetDispPosZ(iDispPatternIdx) - OM.EqpStat.dSubstrateHeight)
                    Log.Trace("TOOL_ZPckrCheck Position"    , PM_GetValue(mi.TOOL_ZPckr , pv.TOOL_ZPckrCheck).ToString());
                    Log.Trace("OM.DevOptn.dShakeOffset"     , OM.DevOptn.dShakeOffset    .ToString());
                    Log.Trace("OM.EqpStat.dSubstrateHeight" , OM.EqpStat.dSubstrateHeight.ToString());
                    Log.Trace("OM.DevInfo.dWFER_Tickness"   , OM.DevInfo.dWFER_Tickness  .ToString());

                    

                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrCheck , OM.DevOptn.dShakeOffset- OM.EqpStat.dSubstrateHeight - OM.DevInfo.dWFER_Tickness);
                    //MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrPlce  , OM.DevOptn.dShakeOffset);
                    SEQ.LoadCell.WeightCheck();
                    Step.iCycle++;
                    return false ;

                case 17:
                    if (!SEQ.LoadCell.IsReceiveEnd()) return false;
                    if (SEQ.LoadCell.GetLoadCellData() * 1000 > 100 ) 
                    {
                        ER_SetErr(ei.PRT_LoadCell, "로드셀의 무게가 150g 이상 감지되었습니다. PickerZ Place포지션을 조정하세요");
                        MT_EmgStop(mi.TOOL_ZPckr);                        
                        return true ;
                    }

                    SEQ.LoadCell.WeightCheck();
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrCheck , OM.DevOptn.dShakeOffset- OM.EqpStat.dSubstrateHeight - OM.DevInfo.dWFER_Tickness)) return false ;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!m_tmDelay.OnDelay(300)) return false ;

                    SEQ.LoadCell.WeightCheck();
                    Step.iCycle++;
                    return false ;

                case 19:
                    if (!SEQ.LoadCell.IsReceiveEnd()) return false;
                    if (SEQ.LoadCell.GetLoadCellData() * 1000 > 150 ) 
                    {
                        ER_SetErr(ei.PRT_LoadCell, "로드셀의 무게가 150g 이상 감지되었습니다. PickerZ Offset포지션을 조정하세요");
                        MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                        return true ;
                    }

                    Step.iCycle++;
                    return false ;

                case 20:
                    MT_GoAbsVel(mi.TOOL_XLeft ,PM_GetValue(mi.TOOL_XLeft, pv.TOOL_XLeftPkPlce)-OM.DevOptn.dShakeRange, 2);
                    MT_GoAbsVel(mi.TOOL_YGent ,PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentPkPlce)-OM.DevOptn.dShakeRange, 2);//왼쪽 뒤
                    Step.iCycle++;
                    return false ;

                case 21:
                    if(!MT_GetStop(mi.TOOL_XLeft)) return false ;                    
                    if(!MT_GetStop(mi.TOOL_YGent)) return false ;                    
                    MT_GoAbsVel(mi.TOOL_XLeft ,PM_GetValue(mi.TOOL_XLeft, pv.TOOL_XLeftPkPlce)+OM.DevOptn.dShakeRange, 2);
                    MT_GoAbsVel(mi.TOOL_YGent ,PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentPkPlce)-OM.DevOptn.dShakeRange, 2);//오른쪽 뒤
                    Step.iCycle++;
                    return false ;
                    

                case 22:
                    if(!MT_GetStop(mi.TOOL_XLeft)) return false ;                    
                    if(!MT_GetStop(mi.TOOL_YGent)) return false ;      
                    MT_GoAbsVel(mi.TOOL_XLeft ,PM_GetValue(mi.TOOL_XLeft, pv.TOOL_XLeftPkPlce)+OM.DevOptn.dShakeRange, 2);
                    MT_GoAbsVel(mi.TOOL_YGent ,PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentPkPlce)+OM.DevOptn.dShakeRange, 2);//오른쪽 앞
                    Step.iCycle++;
                    return false ;
                    

                case 23:
                    if(!MT_GetStop(mi.TOOL_XLeft)) return false ;                    
                    if(!MT_GetStop(mi.TOOL_YGent)) return false ;
                    MT_GoAbsVel(mi.TOOL_XLeft ,PM_GetValue(mi.TOOL_XLeft, pv.TOOL_XLeftPkPlce)-OM.DevOptn.dShakeRange, 2);
                    MT_GoAbsVel(mi.TOOL_YGent ,PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentPkPlce)+OM.DevOptn.dShakeRange, 2);      
                    //MoveMotr(mi.TOOL_XLeft ,pv.TOOL_XLeftPkPlce,-1);
                    //MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentPkPlce, 1);//왼쪽 앞
                    Step.iCycle++;
                    return false ;                    

                case 24:
                    if(!MT_GetStop(mi.TOOL_XLeft)) return false ;                    
                    if(!MT_GetStop(mi.TOOL_YGent)) return false ;
                    MT_GoAbsVel(mi.TOOL_XLeft ,PM_GetValue(mi.TOOL_XLeft, pv.TOOL_XLeftPkPlce)-OM.DevOptn.dShakeRange, 2);
                    MT_GoAbsVel(mi.TOOL_YGent ,PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentPkPlce)-OM.DevOptn.dShakeRange, 2);      
                    //MoveMotr(mi.TOOL_XLeft ,pv.TOOL_XLeftPkPlce,-1);
                    //MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentPkPlce,-1);//왼쪽 앞
                    Step.iCycle++;
                    return false ;                    

                case 25:
                    if(!MT_GetStop(mi.TOOL_XLeft)) return false ;                    
                    if(!MT_GetStop(mi.TOOL_YGent)) return false ;      
                    MT_GoAbsVel(mi.TOOL_XLeft ,PM_GetValue(mi.TOOL_XLeft, pv.TOOL_XLeftPkPlce), 2);
                    MT_GoAbsVel(mi.TOOL_YGent ,PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentPkPlce), 2);
                    Step.iCycle++;
                    return false ;                    

                case 26:
               
                    if(!MT_GetStopPos(mi.TOOL_XLeft ,pv.TOOL_XLeftPkPlce)) return false ;                    
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentPkPlce)) return false ;  

                    m_iDistVisnInspCnt = 0 ;

                    Step.iCycle++;
                    return false ;
                    
                //밑에서 씀.
                case 27:

                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtM);
                    MoveMotr(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtM);
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork);
                    
                    if(!SEQ.VisnRB.SendCommand(VisnCom.ci.RIGHTDIST.ToString())){ //다이얼라인 으로 세팅.
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    
                    Step.iCycle++;
                    return false ;

                case 28:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtM)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtM  )) return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork )) return false ;
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Command)) return false ;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 29:
                    if(!m_tmDelay.OnDelay(100)) return false ;
                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 30:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtS);
                    MoveMotr(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtS  );
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgWfrWork );

                    Step.iCycle++;
                    return false ;

                case 31:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtS)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtS  )) return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgWfrWork )) return false ;

                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 32://
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    if(!SEQ.VisnRB.GetInspOk()){ //검사
                        SM.ER_SetErr(ei.VSN_InspNG , VisnCom.ci.RIGHTDIST.ToString() + " 비전 검사 실패");
                        return true ;
                    }
                    
                    string sRet ="";
                    if (!SEQ.VisnRB.LoadRsltRightDist(ref sRet) )
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.RIGHTDIST.ToString() + " 결과값 파일로딩 실패");
                        return true;
                    }                    

                    if(!VisnCom.GetRsltFromString(sRet , out RsltRightDist))
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.RIGHTDIST.ToString() + " 결과값 파싱 실패");
                        return true;
                    }
                    


                    



                    double dDistX =  RsltRightDist.dMainX - RsltRightDist.dSubX  ;
                    double dDistY =-(RsltRightDist.dMainY - RsltRightDist.dSubY) ;//여기

                    Program.SendListMsg("dDistX=" + GetS(dDistX) + "dDistY=" + GetS(dDistY));

                    if(m_iDistVisnInspCnt +1 != m_iDistVisnInspCntMax) { 
                        if(Math.Abs(RsltDieAlign.dMainX) > OM.DevOptn.dVisnTolXY){
                            ER_SetErr(ei.VSN_InspRangeOver , "Die Align 비젼의 X보정값이 설정범위를 넘었습니다.");
                            Step.iCycle = 0 ;
                            return true ;
                        }
                        if(Math.Abs(RsltDieAlign.dMainY) > OM.DevOptn.dVisnTolXY){
                            ER_SetErr(ei.VSN_InspRangeOver , "Die Align 비젼의 Y보정값이 설정범위를 넘었습니다.");
                            Step.iCycle = 0 ;
                            return true ;
                        }
                        SEQ.SSTG.UVW.GoInc(dDistX , dDistY , 0);
                    }
                    else {
                        if(Math.Abs(RsltDieAlign.dMainX) > OM.DevOptn.dRghtEndVisnTolXY){
                            ER_SetErr(ei.VSN_InspRangeOver, "엔드 비젼 우측 X 결과값이 기존 포지션 차이가" + Math.Abs(dDistX).ToString() + "입니다.");
                            Step.iCycle = 0 ;
                            return true ;
                        }
                        if(Math.Abs(RsltDieAlign.dMainY) > OM.DevOptn.dRghtEndVisnTolXY){
                            ER_SetErr(ei.VSN_InspRangeOver, "엔드 비젼 우측 Y 결과값이 기존 포지션 차이가" + Math.Abs(dDistY).ToString() + "입니다.");
                            Step.iCycle = 0 ;
                            return true ;
                        }

                    }
                    Step.iCycle++;
                    return false ;

                case 33:
                    if(!SEQ.SSTG.UVW.GetStop()) return false ;
                    m_iDistVisnInspCnt++;
                    if(m_iDistVisnInspCntMax > m_iDistVisnInspCnt) {
                        
                        Step.iCycle=27;
                        return false ;
                    }


                    DM.ARAY[ri.PCKR].SetStat(c,r,cs.Attach);

                    Step.iCycle=0;
                    return true;
            }
        }

        public bool CyclePlace()
        {
            String sTemp;
            bool bUsingDelay = Step.iCycle == 13 || Step.iCycle == 14 ;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode &&!bUsingDelay, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true;
            }

            int r = 0, c = 0;
            //SEQ.SSTG.FindChip(ri.SSTG, out c, out r);

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    if(MT_GetCmdPos(mi.TOOL_XLeft) != PM_GetValue(mi.TOOL_XLeft,pv.TOOL_XLeftPkPlce)){
                        ER_SetErr(ei.PRT_Crash , "픽커X 의 위치가 Place포지션이 아닙니다.");
                        return true ;
                    }
                    if(MT_GetCmdPos(mi.TOOL_YGent) != PM_GetValue(mi.TOOL_YGent,pv.TOOL_YGentPkPlce)){
                        ER_SetErr(ei.PRT_Crash , "픽커Y 의 위치가 Place포지션이 아닙니다.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false ;

                case 11:
                    MT_GoIncVel(mi.TOOL_ZPckr , 3.0 , OM.DevOptn.dAttachSpeed1);
                    SEQ.LoadCell.WeightCheck();
                    Step.iCycle++;
                    return false ;

                case 12:
                    if (!SEQ.LoadCell.IsReceiveEnd()) return false;
                    if (SEQ.LoadCell.GetLoadCellData() * 1000 > OM.DevOptn.dAttachForce + OM.DevOptn.dAttachForceOfs) 
                    {
                        MT_Stop(mi.TOOL_ZPckr);
                    }
                    if (!MT_GetStop(mi.TOOL_ZPckr)) {
                        SEQ.LoadCell.WeightCheck(); //로드셀 무게 달라하고 다시 12번 스텝 다시 탄다.
                        return false;
                    }
                    
                    m_tmDelay.Clear();
                    
                    Step.iCycle++;
                    return false ;

                //타임아웃 조건에서 씀.
                case 13: 
                    if (!m_tmDelay.OnDelay(true, OM.DevOptn.iAttachDelay)) return false;
                    if(!OM.CmnOptn.bEpoxyPushTest) IO_SetY(yi.TOOL_PckrVac , false );
                    m_tmDelay.Clear();
                
                    Step.iCycle++;
                    return false ;
                

                    //100mm/s 속도   1000mm/s^2 가속도  0.1 초 가속시간      =1G
                    //100mm/s 속도   2000mm/s^2 가속도  0.05초 가속시간      =2G
                    //100mm/s 속도   3000mm/s^2 가속도         가속시간      =3G
                    //400mm/s 속도  20000mm/s^2 가속도  0.02   가속시간      =5G
                //타임아웃 조건에서 씀.
                case 14: 
                    if(!m_tmDelay.OnDelay(true , OM.DevOptn.iAtAttachDelay)) return false ;  //여기서 포지션 틀어질수도 있음.
                    if(!OM.CmnOptn.bEpoxyPushTest)MT_GoInc(mi.TOOL_ZPckr , OM.DevOptn.dAtPlaceUpOfs , OM.DevOptn.dAtPlaceUpSpeed     ,   1,   1);   
                    else                          MT_GoInc(mi.TOOL_ZPckr , OM.DevOptn.dAtPlaceUpOfs , OM.DevOptn.dAtPlaceUpSpeed/10  , 0.1, 0.1);
                    Step.iCycle++;
                    return false ;

                case 15:
                    if (!MT_GetStop(mi.TOOL_ZPckr)) return false;
                    int iSAray = ri.SSTG;
                    SEQ.SSTG.FindChip(out iSAray, out c, out r);
                    if(!OM.CmnOptn.bEpoxyPushTest){
                        DM.ARAY[ri.SSTG].SetStat(c, r, cs.EndVisn);
                        DM.ARAY[ri.PCKR].SetStat(0, 0, cs.None);
                    }
                    else {
                        DM.ARAY[ri.PCKR].SetStat(0, 0, cs.Distance);
                    }

                    //Ver 1.0.5.2
                    //20180816
                    //Picker에서 데이터 옮겨주는 시점이 여기라서 여기서 저장함
                    SPC.WRK.Data.WfrBarcode = DM.ARAY[ri.PCKR].Chip[0, 0].Data;

                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);
                    Step.iCycle++;
                    return false ;

                case 16:
                    if (!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait)) return false;
                    
                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait) ;
                
                    Step.iCycle++;
                    return false ;

                case 17: 
                    if (!MT_GetStopPos(mi.TOOL_XLeft, pv.TOOL_XLeftWait)) return false;
                    if(OM.CmnOptn.bEpoxyPushTest){
                        SEQ._bBtnStop = true ;
                    }

                    Step.iCycle = 0;
                    return true;
            }

        }

        public bool CycleEndDistanceVisn() 
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int r = 0, c = 0;
            string sRet ="";
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork);

                    MT_GoAbsRun(mi.SSTG_XFrnt, OM.EqpStat.dSstgFtX);
                    MT_GoAbsRun(mi.SSTG_YRght, OM.EqpStat.dSstgRtY);
                    MT_GoAbsRun(mi.SSTG_YLeft, OM.EqpStat.dSstgLtY);

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork)) return false ;
                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    MoveMotr(mi.TOOL_YGent , pv.TOOL_YGentPkPlce);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait  ))return false ;         
                    if(!MT_GetStopPos(mi.TOOL_YGent , pv.TOOL_YGentPkPlce))return false ;         

                    if(!SEQ.VisnRB.SendCommand(VisnCom.ci.RIGHTDIST.ToString())){ //다이얼라인 으로 세팅.
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }

                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtM);
                    MoveMotr(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtM);

                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtM)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtM  )) return false ;                  

                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Command)) return false ;  
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14:      
                    if(!m_tmDelay.OnDelay(100))return false ;
                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }                   

                    Step.iCycle++;
                    return false;

                case 15:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtS);
                    MoveMotr(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtS  );
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsEndWork );
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtS)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtS  )) return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsEndWork)) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 17:      
                    if(!m_tmDelay.OnDelay(100))return false ;

                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 18://
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    if(!SEQ.VisnRB.GetInspOk()){ //검사
                        SM.ER_SetErr(ei.VSN_InspNG , VisnCom.ci.RIGHTDIST.ToString() + " 비전 검사 실패");
                        return true ;
                    }
                    
                    
                    if (!SEQ.VisnRB.LoadRsltRightDist(ref sRet) )
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.RIGHTDIST.ToString() + " 결과값 파일로딩 실패");
                        return true;
                    }                    

                    if(!VisnCom.GetRsltFromString(sRet , out RsltRightDist))
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.RIGHTDIST.ToString() + " 결과값 파싱 실패");
                        return true;
                    }

                    double dRightDistX =  RsltRightDist.dMainX - RsltRightDist.dSubX  ;
                    double dRightDistY =  RsltRightDist.dMainY - RsltRightDist.dSubY  ;
                    Program.SendListMsg("dEndRightDistX=" + GetS(dRightDistX) + "dEndRightDistY=" + GetS(dRightDistY));
                    
                    if(Math.Abs(dRightDistX)> OM.DevOptn.dRghtEndVisnTolXY) {
                        ER_SetErr(ei.VSN_InspRangeOver, "엔드 비젼 우측 X 결과값이 기존 포지션 차이가" + 
                                  Math.Abs(dRightDistX).ToString() + "입니다.");
                        return true;
                    }

                    if(Math.Abs(dRightDistY)> OM.DevOptn.dRghtEndVisnTolXY) {
                        ER_SetErr(ei.VSN_InspRangeOver, "엔드 비젼 우측 Y 결과값이 기존 포지션 차이가" + 
                                  Math.Abs(dRightDistY).ToString() + "입니다.");
                        return true;
                    }

                    SPC.WRK.Data.RghtGapX = dRightDistX ;
                    SPC.WRK.Data.RghtGapY = dRightDistY ;
                    
                    Step.iCycle++;
                    return false;

                //왼쪽 검사
                case 19:
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnLtM);
                    MoveMotr(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnLtM);
                    //MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork);
                    if(!SEQ.VisnRB.SendCommand(VisnCom.ci.LEFTDIST.ToString())){ //다이얼라인 으로 세팅.
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false ;

                case 20:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnLtM)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnLtM  )) return false ; 
                    //if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork )) return false ; 
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork);  
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Command)) return false ;  
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 21:      
                    if(!m_tmDelay.OnDelay(100))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsWork)) return false ;
                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }                   

                    Step.iCycle++;
                    return false;

                case 22:
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnLtS);
                    MoveMotr(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnLtS  );
                    MoveMotr(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsEndWork );
                    Step.iCycle++;
                    return false ;

                case 23:
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnLtS)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnLtS  )) return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZVisn , pv.TOOL_ZVisnSStgSbsEndWork )) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 24:      
                    if(!m_tmDelay.OnDelay(100))return false ;

                    if(!SEQ.VisnRB.SendInsp()){ //검사
                        SM.ER_SetErr(ei.VSN_ComErr , "비전 통신 준비 안됨.");
                        return true ;
                    }
                    Step.iCycle++;
                    return false;

                case 25://
                    if(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.Insp)) return false ;
                    if(!SEQ.VisnRB.GetInspOk()){ //검사
                        SM.ER_SetErr(ei.VSN_InspNG , VisnCom.ci.LEFTDIST.ToString() + " 비전 검사 실패");
                        return true ;
                    }
                    
                    if (!SEQ.VisnRB.LoadRsltLeftDist(ref sRet) )
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.LEFTDIST.ToString() + " 결과값 파일로딩 실패");
                        return true;
                    }                    

                    if(!VisnCom.GetRsltFromString(sRet , out RsltLeftDist))
                    {
                        ER_SetErr(ei.VSN_InspNG, VisnCom.ci.LEFTDIST.ToString() + " 결과값 파싱 실패");
                        return true;
                    }

                    double dLeftDistX =  RsltLeftDist.dMainX - RsltLeftDist.dSubX  ;
                    double dLeftDistY =  RsltLeftDist.dMainY - RsltLeftDist.dSubY ;

                    Program.SendListMsg("dEndLeftDistX=" + GetS(dLeftDistX) + "dEndLeftDistY=" + GetS(dLeftDistY));

                    if(Math.Abs(dLeftDistX) > OM.DevOptn.dLeftEndVisnTolXY) {
                        ER_SetErr(ei.VSN_InspRangeOver, "엔드 비젼 좌측 X 결과값이 기존 포지션 차이가" + 
                                  Math.Abs(dLeftDistX).ToString() + "입니다.");
                        return true;
                    }

                    if(Math.Abs(dLeftDistY) > OM.DevOptn.dLeftEndVisnTolXY) {
                        ER_SetErr(ei.VSN_InspRangeOver, "엔드 비젼 좌측 Y 결과값이 기존 포지션 차이가" + 
                                  Math.Abs(dLeftDistY).ToString() + "입니다.");
                        return true;
                    }

                    SPC.WRK.Data.LeftGapX = dLeftDistX ;
                    SPC.WRK.Data.LeftGapY = dLeftDistY ;

                    int iAray = ri.SSTG;
                    SEQ.SSTG.FindChip(out iAray , out c , out r );
                    DM.ARAY[ri.SSTG].SetStat(c,r,cs.EndHeight);

                    Step.iCycle=0;
                    return true;
            }
        }




        BltValue BltVal ;
        int iCrntBltNode = 0 ;
        public bool CycleBltHeight()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            int SstgR = 0, SstgC = 0;
            int PckrR = 0, PckrC = 0;
            //double dXOfs = 0 ;
            //double dYOfs = 0 ;
            double dXPos = 0.0;
            double dYPos = 0.0;
            double dVal = 0.0;
            
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    //if(Step.iCycle != PreStep.iCycle)Trace(m_sPartName.c_str(), sTemp.c_str());
                    return true;

                case 10:
                    BltValues.Clear();

                    iCrntBltNode = 0 ;

                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;

                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    Step.iCycle++;
                    return false ;

                //밑에서씀.
                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;

                    dXPos = GetBltSubPosX(iCrntBltNode);
                    dYPos = GetBltSubPosY(iCrntBltNode);

                    MT_GoAbsRun(mi.TOOL_XRght, dXPos);
                    MT_GoAbsRun(mi.TOOL_YGent, dYPos);
                    
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStop(mi.TOOL_XRght))return false ;
                    if(!MT_GetStop(mi.TOOL_YGent))return false ;


                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!m_tmDelay.OnDelay(100)) return false ;
                    SEQ.HeightSnsr.SendCheckHeight();
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!SEQ.HeightSnsr.IsReceiveEnd()) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!m_tmDelay.OnDelay(100)) return false ;//일단 야매로 딜레이 넣음 이거 없으면 높이측정값 전에 측정한값을 가져옴.
                    dVal = SEQ.HeightSnsr.GetHeight();
                    
                    if(dVal == 99){
                        ER_SetErr(ei.HGT_RangeErr , "높이측정을 실패 했습니다.");
                        return true ;
                    }

                    BltVal.dSub = dVal;

                    dXPos = GetBltDiePosX(iCrntBltNode);
                    dYPos = GetBltDiePosY(iCrntBltNode);

                    MT_GoAbsRun(mi.TOOL_XRght, dXPos);
                    MT_GoAbsRun(mi.TOOL_YGent, dYPos);
                    Step.iCycle++;
                    return false ;

                case 17:
                    if(!MT_GetStop(mi.TOOL_XRght))return false ;
                    if(!MT_GetStop(mi.TOOL_YGent))return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 18:
                    if(!m_tmDelay.OnDelay(100)) return false ;
                    SEQ.HeightSnsr.SendCheckHeight();
                    Step.iCycle++;
                    return false ;

                case 19:
                    if(!SEQ.HeightSnsr.IsReceiveEnd()) return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 20:
                    if(!m_tmDelay.OnDelay(100)) return false ;//일단 야매로 딜레이 넣음 이거 없으면 높이측정값 전에 측정한값을 가져옴.
                    dVal = SEQ.HeightSnsr.GetHeight();
                    
                    if(dVal == 99){
                        ER_SetErr(ei.HGT_RangeErr , "높이측정을 실패 했습니다.");
                        return true ;
                    }
                    
                    
                    BltVal.dDie = dVal;
                    BltValues .Add(BltVal);

                    iCrntBltNode++;
                    if(iCrntBltNode < SEQ.BltPtrn.GetBltPosCnt()){
                        Step.iCycle=12;
                        return false ;
                    }

                    SPC.WRK.Data.BltHeight= "" ;
                    foreach(var Val in BltValues){
                        SPC.WRK.Data.BltHeight+= string.Format("{0:0.###}", Val.dDie - Val.dSub - OM.DevInfo.dWFER_Tickness);
                        SPC.WRK.Data.BltHeight+= " ";
                    }
                    SPC.WRK.Data.BltHeight.Trim();

                    //Ver 1.0.5.0
                    //2018.08.09 CyclePlace에서 SPC에 들어갈 Substrate 바코드 내용 집어넣는다.
                    int iAray = ri.SSTG;
                    SEQ.SSTG.FindChip(out iAray , out SstgC, out SstgR);
                    SPC.WRK.Data.SubBarcode = DM.ARAY[ri.SSTG].Chip[SstgC, SstgR].Data;

                    DM.ARAY[ri.SSTG].SetStat(SstgC, SstgR, cs.WorkEnd);

                    SPC.WRK.Data.EndTime = DateTime.Now.ToOADate();
                    SPC.WRK.SaveDataIni();

                    Step.iCycle=0;
                    return true;
            }
        }

        //디스펜서 니들 높이 체크 매뉴얼 사이클
        public bool CycleDispCheck()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
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
                    MoveCyl(ci.TOOL_DispCvFwBw, fb.Bwd);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!CL_Complete(ci.TOOL_DispCvFwBw)) return false;
                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);

                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtWait);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtWait))return false ;
                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtDispCheck);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentDispCheck);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtDispCheck)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentDispCheck)) return false ;
                    MoveMotr(mi.TOOL_ZDisp, pv.TOOL_ZDispCheck, dCheckOfs);
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!MT_GetStopPos(mi.TOOL_ZDisp, pv.TOOL_ZDispCheck, dCheckOfs)) return false;
                    MT_GoIncVel(mi.TOOL_ZDisp, 10, 1.0);
                    Step.iCycle++;
                    return false ;

               
                case 16: 
                    if(!IO_GetX(xi.TOOL_NdleNotCheck)){
                        MT_Stop(mi.TOOL_ZDisp);
                    }
                    if(!MT_GetStop(mi.TOOL_ZDisp)) return false ;
                  
                    if(IO_GetX(xi.TOOL_NdleNotCheck)){
                        MT_GoIncVel(mi.TOOL_ZDisp, 3.0, 1.0);
                    }
                    Step.iCycle++;
                    return false ;
                
                case 17: 
                    if(!IO_GetX(xi.TOOL_NdleNotCheck)){
                        MT_EmgStop(mi.TOOL_ZDisp);
                    }
                    if(!MT_GetStop(mi.TOOL_ZDisp)) return false ;
                
                    //체크 완료 되면 뒤로 빼면서...
                    //체크 해제
                    MT_GoIncVel(mi.TOOL_ZDisp , -10.0 , 0.1);
                    Step.iCycle++;
                    return false ;
                
                case 18: 
                    if(IO_GetX(xi.TOOL_NdleNotCheck)) {//기존체크위치 가다가 체크되면 스탑.
                        MT_EmgStop(mi.TOOL_ZDisp);
                    }
                    if(!MT_GetStop(mi.TOOL_ZDisp)) return false ;
                    PM.SetValue(mi.TOOL_ZDisp, pv.TOOL_ZDispCheck, MT_GetCmdPos(mi.TOOL_ZDisp));
                    
                    MoveMotr(mi.TOOL_ZDisp, pv.TOOL_ZDispWait);
                    
                    Step.iCycle++;
                    return false ;
                
                case 19: 
                    if(!MT_GetStopPos(mi.TOOL_ZDisp, pv.TOOL_ZDispWait)) return false;
                    PM.Save(OM.GetCrntDev());
                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtWait);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentWait);
                
                    Step.iCycle++;
                    return false;
                    
                case 20:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtWait)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentWait)) return false ;
                    Log.ShowMessage("Warning", "터치센서 위 에폭시를 닦아주세요.");
                
                    Step.iCycle = 0;
                    return true;
            }
        }

        //높이측정기 영점 체크 매뉴얼 사이클
        public bool CycleHghtCheck()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
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
                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtWait))return false ;
                    MoveMotr(mi.TOOL_XRght ,pv.TOOL_XRghtHghtCheck);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentHghtCheck);
                    Step.iCycle++;
                    return false;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XRght ,pv.TOOL_XRghtHghtCheck)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentHghtCheck)) return false ;
                    
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(1000)) return false;
                    SEQ.HeightSnsr.SendRezero();

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 15:
                    if(!m_tmDelay.OnDelay(1000)) return false ;

                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtWait);
                    MoveMotr(mi.TOOL_YGent , pv.TOOL_YGentWait);

                    Step.iCycle++;
                    return false ;

                case 16:
                    if(!MT_GetStopPos(mi.TOOL_XRght,pv.TOOL_XRghtWait)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent,pv.TOOL_YGentWait)) return false ;

                    Step.iCycle = 0;
                    return true;
            }
        }

        //픽커의 바닥면을 터치센서를 센싱.
        public bool CyclePickerCheck()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
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
                    if(!DM.ARAY[ri.SSTG].CheckAllStat(cs.None)) {
                        ER_SetErr(ei.PRT_Detect , "서브스트레이트스테이지에 자제데이터가 있습니다.");
                        return true ;
                    }
                    if(!DM.ARAY[ri.WSTG].CheckAllStat(cs.None)) {
                        ER_SetErr(ei.PRT_Detect , "웨이퍼 스테이지에 자제데이터가 있습니다.");
                        return true ;
                    }
                    if(IO_GetX(xi.SSTG_BoatCheck)) {
                        ER_SetErr(ei.PRT_Detect , "서브스트레이트스테이지에 자제감지 센서가 감지됩니다.");
                        return true ;
                    }

                    SEQ.WSTG.MoveMotr(mi.WSTG_ZExpd , pv.WSTG_ZExpdWork);
                    SEQ.SSTG.MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailDown);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!MT_GetStopPos(mi.WSTG_ZExpd , pv.WSTG_ZExpdWork)) return false ;
                    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailDown)) return false ;



                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);

                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtWait);
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtWait))return false ;
                    MoveMotr(mi.TOOL_XLeft ,pv.TOOL_XLeftPkCheck  );
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentPckrCheck);
                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStopPos(mi.TOOL_XLeft ,pv.TOOL_XLeftPkCheck  )) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentPckrCheck)) return false ;
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrCheck, dCheckOfs);
                    Step.iCycle++;
                    return false;

                case 15:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrCheck, dCheckOfs)) return false;
                    MT_GoIncVel(mi.TOOL_ZPckr, 10, 1.0);
                    Step.iCycle++;
                    return false ;

               
                case 16: 
                    if(!IO_GetX(xi.TOOL_NdleNotCheck)){
                        MT_Stop(mi.TOOL_ZPckr);
                    }
                    if(!MT_GetStop(mi.TOOL_ZPckr)) return false ;
                  
                    if(IO_GetX(xi.TOOL_NdleNotCheck)){
                        MT_GoIncVel(mi.TOOL_ZPckr, 3.0, 1.0);
                    }
                    Step.iCycle++;
                    return false ;
                
                case 17: 
                    if(!IO_GetX(xi.TOOL_NdleNotCheck)){
                        MT_EmgStop(mi.TOOL_ZPckr);
                    }
                    if(!MT_GetStop(mi.TOOL_ZPckr)) return false ;
                
                    //체크 완료 되면 뒤로 빼면서...
                    //체크 해제
                    MT_GoIncVel(mi.TOOL_ZPckr , -10.0 , 0.1);
                    Step.iCycle++;
                    return false ;
                
                case 18: 
                    if(IO_GetX(xi.TOOL_NdleNotCheck)) {//기존체크위치 가다가 체크되면 스탑.
                        MT_EmgStop(mi.TOOL_ZPckr);
                    }
                    if(!MT_GetStop(mi.TOOL_ZPckr)) return false ;
                    PM.SetValue(mi.TOOL_ZPckr, pv.TOOL_ZPckrCheck, MT_GetCmdPos(mi.TOOL_ZPckr));
                    
                    MoveMotr(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait);
                    
                    Step.iCycle++;
                    return false ;
                
                case 19: 
                    if(!MT_GetStopPos(mi.TOOL_ZPckr, pv.TOOL_ZPckrWait)) return false;
                    PM.Save(OM.GetCrntDev());
                    MoveMotr(mi.TOOL_XLeft ,pv.TOOL_XLeftWait);
                    MoveMotr(mi.TOOL_YGent ,pv.TOOL_YGentWait);
                    SEQ.WSTG.MoveMotr(mi.WSTG_ZExpd , pv.WSTG_ZExpdWait);
                    SEQ.SSTG.MoveMotr(mi.SSTG_ZRail , pv.SSTG_ZRailWait);
                    Step.iCycle++;
                    return false;
                    
                case 20:
                    if(!MT_GetStopPos(mi.TOOL_XLeft ,pv.TOOL_XLeftWait)) return false ;
                    if(!MT_GetStopPos(mi.TOOL_YGent ,pv.TOOL_YGentWait)) return false ;
                    if(!MT_GetStopPos(mi.WSTG_ZExpd , pv.WSTG_ZExpdWait)) return false ;
                    if(!MT_GetStopPos(mi.SSTG_ZRail , pv.SSTG_ZRailWait)) return false ;

                    
                    OM.EqpStat.bNeedCheckPckr = false ;
                
                    Step.iCycle = 0;
                    return true;
            }
        }

        
        //툴하고 레일 컨버전 포지션으로 이동.
        public bool CycleConvPos()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                ER_SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
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
                    MoveMotr(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait);
                    MoveMotr(mi.TOOL_ZDisp , pv.TOOL_ZDispWait);

                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.TOOL_ZPckr , pv.TOOL_ZPckrWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_ZDisp , pv.TOOL_ZDispWait))return false ;
                    MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                    MoveMotr(mi.TOOL_XRght , pv.TOOL_XRghtWait);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!MT_GetStopPos(mi.TOOL_XLeft , pv.TOOL_XLeftWait))return false ;
                    if(!MT_GetStopPos(mi.TOOL_XRght , pv.TOOL_XRghtWait))return false ;


                    Step.iCycle++;
                    return false ;

                    
                case 13://여기서 분기 한다.
                    if(SM.MT_GetCmdPos(mi.SSTG_XRail) == 0){
                        MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftWait);
                        MoveMotr(mi.TOOL_YGent , pv.TOOL_YGentWait);
                        SEQ.SSTG.MoveMotr(mi.SSTG_XRail , pv.SSTG_XRailWork);
                    }
                    else {
                        MoveMotr(mi.TOOL_XLeft , pv.TOOL_XLeftConv);
                        MoveMotr(mi.TOOL_YGent , pv.TOOL_YGentConv);
                        SEQ.SSTG.MoveMotr(mi.SSTG_XRail , 0                );
                    }

                    Step.iCycle++;
                    return false;

                case 14:
                    if(!MT_GetStop(mi.TOOL_XLeft)) return false ;
                    if(!MT_GetStop(mi.TOOL_YGent)) return false ;
                    if(!MT_GetStop(mi.SSTG_XRail)) return false ;
                
                    Step.iCycle = 0;
                    return true;
            }
        }
        //public double GetXPosDispFromVisn(double _dPos)
        //{
        //    //겐트리Y와 오른쪽X는 3사분면 사용.
        //    double dVisnToHeightOffset = PM_GetValue(mi.TOOL_XRght , pv.TOOL_XRghtDispCheck) - PM_GetValue(mi.TOOL_XRght , pv.TOOL_XRghtTVsnCheck);
        //    return _dPos + dVisnToHeightOffset ;
        //}
        //public double GetYPosDispFromVisn(double _dPos)
        //{
        //    //겐트리Y와 오른쪽X는 3사분면 사용.
        //    double dVisnToHeightOffset = PM_GetValue(mi.TOOL_YGent , pv.TOOL_YGentDispCheck) - PM_GetValue(mi.TOOL_YGent , pv.TOOL_YGentTVsnCheck);
        //    return _dPos + dVisnToHeightOffset ;
        //}   
        public bool CheckSafe(ci _eActr, fb _eFwd)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if(_eActr == ci.TOOL_GuidFtDwUp){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }
            
            else if(_eActr == ci.TOOL_GuidRrDwUp){
                //if(_bFwd == EN_CYLINDER_POS.cpFwd) {
                    //if(!bExistSply /*|| !bSRT_ZTop*/) {sMsg = string ("Tray 센서 감지중!"); bRet = false ;}
                //}
            }

            else if(_eActr == ci.TOOL_DispCvFwBw){
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

            //TOOL_ZVisn


            if (_eMotr == mi.TOOL_ZPckr)
            {
                if (!MT_GetStopInpos(mi.TOOL_YGent))
                {
                    sMsg = MT_GetName(mi.TOOL_YGent) + " is moving.";
                    bRet = false;
                }

                if (!MT_GetStopInpos(mi.TOOL_XLeft))
                {
                    sMsg = MT_GetName(mi.TOOL_XLeft) + " is moving.";
                    bRet = false;
                }

            }

            else if (_eMotr == mi.TOOL_YGent)
            {
                if (!MT_GetStopInpos(mi.TOOL_ZPckr))
                {
                    sMsg = MT_GetName(mi.TOOL_ZPckr) + " is moving.";
                    bRet = false;
                }

                if (CL_GetCmd(ci.TOOL_GuidFtDwUp)==fb.Fwd)
                {
                    sMsg = CL_GetName(ci.TOOL_GuidFtDwUp) + " is Fwd.";
                    bRet = false;
                }
                if (CL_GetCmd(ci.TOOL_GuidRrDwUp)==fb.Fwd)
                {
                    sMsg = CL_GetName(ci.TOOL_GuidRrDwUp) + " is Fwd.";
                    bRet = false;
                }

                //if (!MT_GetStopInpos(mi.TOOL_XRght))
                //{
                //    sMsg = MT_GetName(mi.TOOL_XRght) + " is moving.";
                //    bRet = false;
                //}

                if (!MT_GetStopInpos(mi.TOOL_ZDisp))
                {
                    sMsg = MT_GetName(mi.TOOL_ZDisp) + " is moving.";
                    bRet = false;
                }
                
            }

            else if (_eMotr == mi.TOOL_YRsub)
            {
                //if (!MT_GetStopInpos(mi.SSTG_YGrpr))
                //{
                //    sMsg = MT_GetName(mi.SSTG_YGrpr) + "is moving.";
                //    bRet = false;
                //}
                //
                //if (dDstPos > PM_GetValue(mi.SSTG_YGrpr, pv.SSTG_YGrprPickWait))
                //{
                //    sMsg = MT_GetName(mi.SSTG_YGrpr) + "Crash Position with " + MT_GetName(_eMotr);
                //    bRet = false;
                //}
                //
                //if (IO_GetX(xi.SLDR_SstOutCheck))
                //{
                //    sMsg = IO_GetXName(xi.SLDR_SstOutCheck) + "is Checking.";
                //    bRet = false;
                //}
            }
            else if (_eMotr == mi.TOOL_XRght)
            {
                if (!MT_GetStopInpos(mi.TOOL_ZPckr))
                {
                    sMsg = MT_GetName(mi.TOOL_ZPckr) + " is moving.";
                    bRet = false;
                }

                //if (!MT_GetStopInpos(mi.TOOL_XLeft))
                //{
                //    sMsg = MT_GetName(mi.TOOL_XLeft) + " is moving.";
                //    bRet = false;
                //}

                //if (!MT_GetStopInpos(mi.TOOL_YGent))
                //{
                //    sMsg = MT_GetName(mi.TOOL_YGent) + " is moving.";
                //    bRet = false;
                //}

                if (!MT_GetStopInpos(mi.TOOL_ZDisp))
                {
                    sMsg = MT_GetName(mi.TOOL_ZDisp) + " is moving.";
                    bRet = false;
                }
            }
            else if (_eMotr == mi.TOOL_ZDisp)
            {
                if (!MT_GetStopInpos(mi.TOOL_YGent))
                {
                    sMsg = MT_GetName(mi.TOOL_YGent) + " is moving.";
                    bRet = false;
                }

                if (!MT_GetStopInpos(mi.TOOL_XLeft))
                {
                    sMsg = MT_GetName(mi.TOOL_XLeft) + " is moving.";
                    bRet = false;
                }
            }
            else if (_eMotr == mi.TOOL_XLeft)
            {
                if (!MT_GetStopInpos(mi.TOOL_ZPckr))
                {
                    sMsg = MT_GetName(mi.TOOL_ZPckr) + " is moving.";
                    bRet = false;
                }

                if (CL_GetCmd(ci.TOOL_GuidFtDwUp)==fb.Fwd)
                {
                    sMsg = CL_GetName(ci.TOOL_GuidFtDwUp) + " is Fwd.";
                    bRet = false;
                }
                if (CL_GetCmd(ci.TOOL_GuidRrDwUp)==fb.Fwd)
                {
                    sMsg = CL_GetName(ci.TOOL_GuidRrDwUp) + " is Fwd.";
                    bRet = false;
                }
            }
            else if (_eMotr == mi.TOOL_ZVisn)
            {
               
            }
            else if (_eMotr == mi.TOOL_YVisn)
            {
                //if (!MT_GetStopInpos(mi.TOOL_YGent))
                //{
                //    sMsg = MT_GetName(mi.TOOL_YGent) + " is moving.";
                //    bRet = false;
                //}

                //if (!MT_GetStopInpos(mi.TOOL_XLeft))
                //{
                //    sMsg = MT_GetName(mi.TOOL_XLeft) + " is moving.";
                //    bRet = false;
                //}

            }
            else if(_eMotr == mi.TOOL_XEjtL){
                //if(MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprWait)) {sMsg = MT_GetName(mi.WSTG_YGrpr) + "is not in WaitPos.";bRet = false;}
            }
            else if(_eMotr == mi.TOOL_XEjtR){
                //if(MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprWait)) {sMsg = MT_GetName(mi.WSTG_YGrpr) + "is not in WaitPos.";bRet = false;}
            }
            else if(_eMotr == mi.TOOL_YEjtr){
                //if(MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprWait)) {sMsg = MT_GetName(mi.WSTG_YGrpr) + "is not in WaitPos.";bRet = false;}
            }
            else if(_eMotr == mi.TOOL_ZEjtr){
                //if(MT_GetStopPos(mi.WSTG_YGrpr , pv.WSTG_YGrprWait)) {sMsg = MT_GetName(mi.WSTG_YGrpr) + "is not in WaitPos.";bRet = false;}
            }

            else
            {
                sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Log.Trace(MT_GetName(_eMotr), sMsg);
                //메뉴얼 동작일때.
                if (Step.eSeq == 0) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
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
            if(Step.iCycle!=0) MT_GoAbsRun(_eMotr , dDstPos);
            else               MT_GoAbsMan(_eMotr , dDstPos);
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
            if (!MT_GetStop(mi.TOOL_ZDisp)) return false;
            if (!MT_GetStop(mi.TOOL_ZVisn)) return false;
            if (!MT_GetStop(mi.TOOL_ZPckr)) return false;
            if (!MT_GetStop(mi.TOOL_XRght)) return false;
            if (!MT_GetStop(mi.TOOL_XLeft)) return false;
            if (!MT_GetStop(mi.TOOL_YVisn)) return false;
            if (!MT_GetStop(mi.TOOL_YGent)) return false;

            //툴에서 스테이지쪽을 가져다 쓰는 경우가 많아서 이렇게 함.
            if (!SEQ.WSTG.CheckStop())      return false;
            if (!SEQ.SSTG.CheckStop())      return false;
            return true;
        }




        public bool Test()
        {
            double dOriAngle  = CMath.GetLineAngle(PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnRtM),   
                                                           PM_GetValue(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnRtM  ),
                                                           PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnLtM),
                                                           PM_GetValue(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnLtM  ));

            double dInspAngle = CMath.GetLineAngle(PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnRtM) + RsltSubsAlign.dMainX ,
                                                   PM_GetValue(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnRtM  ) + RsltSubsAlign.dMainY ,
                                                   PM_GetValue(mi.TOOL_XRght ,pv.TOOL_XRghtVsSStgVsnLtM) + RsltSubsAlign.dSubX  ,
                                                   PM_GetValue(mi.TOOL_YVisn ,pv.TOOL_YVisnSStgVsnLtM  ) + RsltSubsAlign.dSubY  );
            //왼쪽에서 오른쪽으로 가는 각도는 1도 + 일때는 1 1도- 일때는 359도 이렇게 표현되서 바꿈
            if(dOriAngle  >315) dOriAngle  -= 360  ;
            if(dInspAngle >315) dInspAngle -= 360  ;
            double dVisnT = dOriAngle - dInspAngle ;

            if(Math.Abs(RsltSubsAlign.dMainX) > OM.DevOptn.dVisnTolXY){
                ER_SetErr(ei.VSN_InspRangeOver , "Substrate Align 비젼의 X보정값이 설정범위를 넘었습니다.");
                Step.iCycle = 0 ;
                return true ;
            }
            if(Math.Abs(RsltSubsAlign.dMainY) > OM.DevOptn.dVisnTolXY){
                ER_SetErr(ei.VSN_InspRangeOver , "Substrate Align 비젼의 Y보정값이 설정범위를 넘었습니다.");
                Step.iCycle = 0 ;
                return true ;
            }
            if(Math.Abs(dVisnT) > OM.DevOptn.dVisnTolAng){
                ER_SetErr(ei.VSN_InspRangeOver , "Substrate Align 비젼의 T보정값이 설정범위를 넘었습니다.");
                Step.iCycle = 0 ;
                return true ;
            }

            //맨마지막은 그냥 확인만 하는것으로 함.
            //if (iMaxInspCnt-1 != iSubsAlignInspCnt)
            {
                // 서브스트레이트 센터에서 현재 검사 포지션까지의 세팅값 거리.
                // Y축이 2개 달려 있어서 둘다 계산 해야함.
                double dDistToolInspToCntrX = PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtVsSStgCtr) - PM_GetValue(mi.TOOL_XRght, pv.TOOL_XRghtVsSStgVsnRtM);
                double dDistToolInspToCntrY = PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentVsSStgCtr) - PM_GetValue(mi.TOOL_YGent, pv.TOOL_YGentPkPlce);
                double dDistVisnInspToWorkY = PM_GetValue(mi.TOOL_YVisn, pv.TOOL_YVisnWork) - PM_GetValue(mi.TOOL_YVisn, pv.TOOL_YVisnSStgVsnRtM);

                //비전값까지 같이 계산해서 넣어준다.
                double dDistCntX = dDistToolInspToCntrX + RsltSubsAlign.dMainX;
                double dDistCntY = dDistToolInspToCntrY + dDistVisnInspToWorkY + RsltSubsAlign.dMainY;

                double dMoveX;
                double dMoveY;
                //CMath.GetRotPnt(-dVisnT , dDistCntX , dDistCntY , out dMoveX , out dMoveY);
                CMath.GetRotPnt(-dVisnT, -dDistCntX, dDistCntY, out dMoveX, out dMoveY);
                Program.SendListMsg("RT:" + GetS(-dVisnT) + " CX:" + GetS(-dDistCntX) + " CY:" + GetS(dDistCntY) + " MX:" + GetS(dMoveX) + "MY:" + GetS(dMoveY));
                SEQ.SSTG.UVW.GoInc(dMoveX + RsltSubsAlign.dMainX, -dMoveY - RsltSubsAlign.dMainY, -dVisnT);
                Program.SendListMsg("MX:" + GetS(dMoveX + RsltSubsAlign.dMainX) + " MY:" + GetS(-dMoveY - RsltSubsAlign.dMainY) + " MT:" + GetS(-dVisnT));
            }

            return true ;
        }
        /*
Wafer Lot ,,M81575,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,,,,,,,,,,,,,,,,,,,,,,,,
#01,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,,,,,,,,,,,,,,,,,,,,,,,,
#02,P,P,F,F,P,F,F,F,P,P,P,P,P,P,P,P,,,,,,,,,,,,,,,,,,,,,,,,
#03,F,P,P,P,P,P,P,P,P,P,F,P,P,P,P,P,,,,,,,,,,,,,,,,,,,,,,,,
#04,F,P,P,F,F,P,P,P,P,P,F,P,P,P,P,P,,,,,,,,,,,,,,,,,,,,,,,,
#05,P,P,P,P,P,P,P,P,P,P,P,P,P,P,F,P,,,,,,,,,,,,,,,,,,,,,,,,
#06,P,P,P,P,P,P,P,P,P,P,P,P,P,P,P,F,,,,,,,,,,,,,,,,,,,,,,,,

         */
        public bool SetWorkMapFile(int _iAray , int _iWorkC , int _iWorkR , string _sBarcode)
        {
            //기존에 있던것들 지우기.
            DirectoryInfo di = new DirectoryInfo(OM.CmnOptn.sMapFileFolder);
            if (!di.Exists) {
                di.Create();
                Program.SendListMsg("Map file 폴더가 없어서 만들었습니다.");
                return false ;
            }

            string sLotId = "" ;
            string sWfrNo = "" ;
            for(int i = 0 ; i < _sBarcode.Length ; i++ )
            {                
                if(OM.CmnOptn.sBarLotIDMask.Length > i && OM.CmnOptn.sBarLotIDMask.Substring(i,1)=="*"){
                    sLotId += _sBarcode.Substring(i,1);
                }

                if(OM.CmnOptn.sBarWfrNoMask.Length > i && OM.CmnOptn.sBarWfrNoMask.Substring(i,1)=="*"){
                    sWfrNo += _sBarcode.Substring(i,1);
                }
            }

            int iStart  = -1 ;
            int iLength = 0 ; 
            for(int i = 0 ; i < OM.CmnOptn.sFileLotIDMask.Length ; i++){
                if(iStart == -1){//시작
                    if(OM.CmnOptn.sFileLotIDMask.Length > i &&  OM.CmnOptn.sFileLotIDMask.Substring(i,1)=="*"){
                        iStart = i ;
                    }
                }
                else {//끝 및 길이.
                    if(OM.CmnOptn.sFileLotIDMask.Length > i &&  OM.CmnOptn.sFileLotIDMask.Substring(i,1)!="*"){
                        iLength = i - iStart ;
                    }
                }
            }
            if (iLength == 0)
            {
                iLength = OM.CmnOptn.sFileLotIDMask.Length - iStart ;
            }

            bool bFoundWaferLot = false ;
            string sLine = "" ;
            string sNewLine = "";
            string sFullMap = "";
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension != ".csv") continue;
                if(fi.Name.Length < iStart + iLength)continue ;
                if (fi.Name.Substring(iStart , iLength) != sLotId) continue;
                try 
                { 
                    StreamReader sr = new StreamReader(OM.CmnOptn.sMapFileFolder + "\\" + fi.Name , Encoding.GetEncoding("utf-8")); 
                    sFullMap = sr.ReadToEnd();
                    sr.BaseStream.Position = 0 ; //다시 처음으로 이동.
                    while(!sr.EndOfStream) 
                    { 
                        sLine = sr.ReadLine(); 
                        if(!bFoundWaferLot && !sLine.Contains("Wafer Lot")) continue ;

                        //"Wafer Lot"이 한번 나오고 나서 맵을 사용 해야 함. 확인 차원.
                        //두개의 맵이 똑같긴 한데 일단 협의를 밑에꺼로 쓴다고 함.
                        if(!bFoundWaferLot) { 
                            string[] sData = sLine.Split(',');
                            if(sData.Length < 2) {
                                Program.SendListMsg("Wafer Lot Reading 실패");
                                return false ;
                            }
                            if(sData[1] != sLotId) {
                                Program.SendListMsg("Wafer의 랏넘버와 Mapfile의 WaferLot이 다릅니다.");
                                return false ;
                            }
                            bFoundWaferLot = true ;
                        }

                        if(!sLine.Contains("#"+sWfrNo)) continue ;
                        break ;
                    }

                    sr.Close();
                    sr.Dispose();
                                            
                    Program.SendListMsg("#"+sWfrNo + " 로딩");
                    Program.SendListMsg(sLine);
                    string[] sChips = sLine.Split(','); 
                    int iMaxDataCnt = DM.ARAY[_iAray].GetMaxCol() * DM.ARAY[_iAray].GetMaxRow() ;
                    int iDataCnt = 0 ;

                    List<string> lsChips = new List<string>();
                    for (int i = 0 ; i < sChips.Length ; i++) 
                    {
                        if(i==0) continue ;//칩번호가 들어있음 ex) #01
                        if(sChips[i] == "F" || sChips[i] == "P" || sChips[i] =="X"){
                            lsChips.Add(sChips[i]);
                        }
                        else if (sChips[i] == "")
                        {

                        }
                        else
                        {
                            lsChips.Add("X");
                        }
                    }

                    //해당칩 데이터 X로 바꾸기.
                    if(!OM.DevOptn.bRvsWafer){
                        for(int c = 0 ; c < DM.ARAY[_iAray].GetMaxCol() ; c++){
                            for (int r = 0; r < DM.ARAY[_iAray].GetMaxRow(); r++){
                                if(iDataCnt >= iMaxDataCnt) continue ;
                                if(_iWorkC == c && _iWorkR == r){
                                    lsChips[iDataCnt]="X";
                                    //return true ;
                                }
                                iDataCnt++;                                    
                            }
                        }                            
                    }
                    else {
                        for(int c = DM.ARAY[_iAray].GetMaxCol() - 1 ; c >= 0 ; c--){
                            for (int r = DM.ARAY[_iAray].GetMaxRow() -1; r >= 0 ; r--){
                                if(iDataCnt >= iMaxDataCnt) continue ;
                                if(_iWorkC == c && _iWorkR == r){
                                    lsChips[iDataCnt]="X";
                                    //return true ;
                                }
                                iDataCnt++;
                            }
                        }  
                    }
                    sNewLine = "#"+sWfrNo ;
                    for(int j = 0 ; j < lsChips.Count ; j++){
                        sNewLine += ",";
                        sNewLine += lsChips[j] ;
                        
                    }

                    //해당 줄을 수정된 데이터로 교체.
                    int iIdx = sFullMap.IndexOf("Wafer") ;
                    iIdx = sFullMap.IndexOf(sLine) ;
                    if (iIdx==-1)
                    {
                        Program.SendListMsg("맵파일에 "+sLine+" 의 내용이 없습니다.");
                        return false ;
                    }
                    sFullMap = sFullMap.Replace(sLine , sNewLine);

                    StreamWriter sw = new StreamWriter(OM.CmnOptn.sMapFileFolder + "\\" + fi.Name ); 
                    sw.Write(sFullMap);
                    sw.Close();
                    sw.Dispose();

                } 
                catch (Exception e) 
                { 
                    Program.SendListMsg(e.Message);
                    return false ;
                }
            }         
            return true ;
        }
    };
    


    

   
    
}
