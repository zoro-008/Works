using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMMON;
using SMDll2;



namespace Machine
{
    public enum EN_SEQ_CYCLE
    {
        Idle        =  0,
        VisnLens    =  1, //Lens쪽 Vision 작업
        Pick        =  2, //Lens Pick
        Align       =  3, //Align(Array만 Align으로 변경)
        VisnHldr    =  4, //Holder쪽 Vision 작업
        Place       =  5, //Screw Type 체결
        Place2      =  6, //Lock Type 체결
        AfterVisn   =  7, //락 타입은 체결후 검사 할 수 있음.
        Out         =  8, //작업 끝나고 트레이 아웃
        PlaceGood   =  9, //재수없게 픽커가 집었는데 홀더검사가 패일 나서 채울데가 없을경우 다시 굳트레이에 놓는다.
        ToolCalib   = 10, //Picker Calibration


        UnlockVisn  = 11, //Unlock 시 Holder 족 Lens 검사(Screw Type)
        UnlockVisn2 = 12, //Unlock 시 Holder 쪽 Lens/Holder 검사(Lock Type)
        Unlock      = 13, //Unlock 작업(Screw Type)
        Unlock2     = 14, //Unlock 작업(Lock Type)
        UnlockAlign = 15, //Unlock 시 Align(Array만 Align으로 변경)
        UnlockPlace = 16, //Unlock 시 푼 렌즈 트레이로 이동
        MAX_SEQ_CYCLE
    };
    //SLD == Solder
    public partial class Stage:PartInterface
    {
        //public-------------------------------------------------------------------
        public struct SStat
        {
            public bool bWorkEnd;
            public bool bReqStop;
            public void Clear()
            {
                bWorkEnd = false;
                bReqStop = false;
            }

        };    //sun Clear When LOT Open. and every 30Sec in autorun()

        //public enum EN_SEQ_CYCLE
        //{
        //    Idle      = 0,
        //    VisnLens  = 1, //렌즈찝기전 비젼검사.
        //    Pick      = 2, //렌즈 찝기.
        //    Align     = 3, //얼라인 한번 치기.
        //    VisnHldr  = 4, //홀더 내려놓기전 비젼 검사.
        //    Place     = 5, //홀더 채결.
        //    ToolCalib = 6, //Picker calibration
        //    PCKZWork  = 7,
        //    MAX_SEQ_CYCLE
        //};

        public struct SStep
        {
            public int iHome;
            public int iToStart;
            public EN_SEQ_CYCLE iSeq;
            public int iCycle;
            public int iToStop;
            public EN_SEQ_CYCLE iLastSeq;
            public void Clear()
            {
                iHome    = 0;
                iToStart = 0;
                iSeq     = EN_SEQ_CYCLE.Idle;
                iCycle   = 0;
                iToStop  = 0;
                iLastSeq = EN_SEQ_CYCLE.Idle;
            }
        };
        //protected-------------------------------------------------------------------
        protected String      m_sPartName;
        //protected CCycleTimer m_tmDispr  ;
        //Timer.
        protected CDelayTimer m_tmMain   ;
        protected CDelayTimer m_tmCycle  ;
        protected CDelayTimer m_tmHome   ;
        protected CDelayTimer m_tmToStop ;
        protected CDelayTimer m_tmToStart;
        protected CDelayTimer m_tmDelay  ;        

        protected SStat Stat;
        protected SStep Step, PreStep;

        protected TWorkInfo WorkInfo;

        protected double m_dLastIdxPos;
        protected String m_sCheckSafeMsg;

        //For Master Form
        protected bool m_bManAutorun = false;
        
        //Ver1.0.1.0
        //Work Mode 추가
        protected const int ScrewType   = 0; //높이 센서로만 체결하는 옵션 CyclePlace
        protected const int LockType    = 1; //포지션으로 돌리고 렌즈가 들어갔는지 Height Sensor로 체크하는 옵션 CyclePlace2

        public string[] m_sCycleName;
        public CTimer[] m_CycleTime;

        public void SetManAutorun(bool _bVal) { m_bManAutorun = _bVal; }

        private struct VisnRslt
        {
            public double dX ;//=0.0;
            public double dY ;//=0.0;
            public double dT ;//=0.0;
        }

        VisnRslt RsltLensL;
        VisnRslt RsltLensR;
        VisnRslt RsltHldrL;
        VisnRslt RsltHldrR;

        VisnRslt RsltLens; //렌즈 캘리브레이션.
        VisnRslt RsltHldr; //홀더 캘리브레이션용.
        
        private struct TSortInfo
        {
            //Input.GetWork태우기 전에 세팅해야하는것들.
            public ri   eTool      ;
            public ri   eTray      ;
            public bool bPick      ;
            public cs   eTrayChip  ;//픽일때 Tray쪽 찝을놈.  플레이스일때 Tray쪽 놓을놈.
            public cs   ePickChip  ;//픽일때 픽커쪽 빈놈  .  플레이스일때 픽커쪽 달려있는놈. 

            //Output GetWork태우고 나면 결과값으로 세팅되는 것들.
            public bool bLeftWork  ;
            public bool bRightWork ;
            public int  iToolShift ; //소팅작업할때 첫번째픽커부터 첫번째 작업해야 하는 픽커까지의 갯수차.
            public int  iTrayC     ;
            public int  iTrayR     ;            
        }
        TSortInfo SortInfo ;

        //리트라이 관련 카운터.
        int iPickVisnRetryCnt  ;
        int iPickRetryCnt      ;
        int iPlaceVisnRetryCnt ;
        int iPlaceRetryCnt     ;

        //소팅 인포=============================================================
        //struct TSortInfo{
        //    public bool         bFindOk     ; //작업가능여부.
        //    public bool         isPick      ; //픽용인지 플레이스 용인지.
        //    public int          iCol        ; //iARAY의 작업할 영역의 왼쪽.
        //    public int          iRow        ; //iARAY의 작업할 영역의 제일위쪽.
        //    public EN_CHIP_STAT iTargetStat ; //목적 칩 상태.
        //    public EN_ARAY_ID   iAray       ; //작업할 어레이
        //    public EN_ARAY_ID   iTool       ; //피커툴 어레이.
        //    public int          iToolShft   ; //툴의 첫번째 컬럼 부터 작업이 안될경우 쉬프트 된다.
        //    public int          iDnCnt      ; //찝거나 내려놓는 칩 갯수.
        //    public bool         bDn[MAX_TOOL_SOCK_COL] ;
        //};
        //TSortInfo SortInfo ;
        //트레이에서 찝거나 트레이에 놓을때 사용 하는 함수.
        //bool GetTraySortInfo  (bool _bPick , EN_ARAY_ID _iTool , EN_ARAY_ID _iAray , TSortInfo & _tInfo);
        //bool ShiftTraySortInfo(TSortInfo & _tInfo , bool _bVacErr1 = false , bool _bVacErr2 = false ,bool _bVacErr3 = false ,bool _bVacErr4 = false );


        

        public Stage()
        {
            m_sPartName = "Stage ";

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();
            
               

            m_sCycleName  = new string[(int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE];
            m_CycleTime   = new CTimer[(int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE];


            for(int i = 0 ; i < (int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE ; i++)
            {
                m_CycleTime [i]  = new CTimer();
            }

            Reset();
            
            InitCycleName();
            InitCycleTime();                        
        }


        //PartInterface 부분.
        //인터페이스 상속.====================================================================================================================
        public void Reset() //리셑 버튼 눌렀을때 타는 함수.
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
        public void Update()
        {
            if (m_bManAutorun)
            {
                Autorun();
                m_bManAutorun = false;
            }
        }
        public bool ToStopCon() //스탑을 하기 위한 조건을 보는 함수.
        {
            Stat.bReqStop = true;
            //During the auto run, do not stop.
            if (Step.iSeq != EN_SEQ_CYCLE.Idle) return false;

            Step.iToStop = 10;
            //Ok.
            return true;


        }
        public bool ToStartCon() //스타트를 하기 위한 조건을 보는 함수.
        {
            Step.iToStart = 10;
            //Ok.
            return true;

        }
        public bool ToStart() //스타트를 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 5000)) SetErr(ei.ETC_ToStartTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

            String sTemp;
            sTemp = String.Format("Step.iToStart={0:00}", Step.iToStart);
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
                    //if (EmbededExe.GetHPSHandle() < 0) return false;

                    Step.iToStart++;
                    return false;

                case 11:
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    Step.iToStart++;
                    return false;

                case 12:
                    if(!GetStop(mi.PCK_ZL)) return false ;
                    if(!GetStop(mi.PCK_ZR)) return false ;

                    Step.iToStart++;
                    return false;

                case 13: 
                    SetY(yi.STG_VacLenOn, true);

                    iPickVisnRetryCnt  = 0 ;
                    iPickRetryCnt      = 0 ;
                    iPlaceVisnRetryCnt = 0 ;
                    iPlaceRetryCnt     = 0 ;

                    iRptCnt = 0;
                    Step.iToStart = 0;
                    return true;


            }

        }
        public bool ToStop() //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 10000)) SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

            String sTemp;
            sTemp = string.Format("Step.iToStop={0:00}", Step.iToStop);
            if (Step.iToStop != PreStep.iToStop)
            {
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iToStop = Step.iToStop;

            Stat.bReqStop = false;

            int c = 0;
            int r = 0;

            //Move Home.
            switch (Step.iToStop)
            {
                default: Step.iToStop = 0;
                    return true;

                case 10: 
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    SetY(yi.PCK_EjtLtOn, false);
                    SetY(yi.PCK_EjtRtOn, false);
                    Step.iToStop++;
                    return false;

                case 11: 
                    if(!GetStop(mi.PCK_ZL)) return false ;
                    if(!GetStop(mi.PCK_ZR)) return false ;

                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);

                    Step.iToStop = 0;
                    return true;
            }


        }

        public int GetHomeStep   () { return Step.iHome    ; } public int GetPreHomeStep   () { return PreStep.iHome    ; } public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        public int GetToStartStep() { return Step.iToStart ; } public int GetPreToStartStep() { return PreStep.iToStart ; }
        public int GetSeqStep    () { return (int)Step.iSeq; } public int GetPreSeqStep    () { return (int)PreStep.iSeq; }
        public int GetCycleStep  () { return Step.iCycle   ; } public int GetPreCycleStep  () { return PreStep.iCycle   ; } public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; }
        public int GetToStopStep () { return Step.iToStop  ; } public int GetPreToStopStep () { return PreStep.iToStop  ; }
        public SStep GetStep     () { return Step; }

        public String GetCycleName(int _iSeq) { return m_sCycleName[_iSeq]; }
        public double GetCycleTime(int _iSeq) { return m_CycleTime [_iSeq].Duration; }
        public String GetPartName (         ) { return m_sPartName        ; }

        public int GetCycleMaxCnt() { return (int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE; }       

        public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            //Check Cycle Time Out.
            String sTemp;
            sTemp = String.Format("%s Step.iCycle={0:00}", "Autorun", Step.iCycle);
            if (Step.iSeq != PreStep.iSeq)
            {
                Log.Trace(m_sPartName, sTemp);
            }


            PreStep.iSeq = Step.iSeq;

            string sCycle = m_sCycleName[(int)Step.iSeq] ;

            //Check Error & Decide Step.
            if (Step.iSeq == EN_SEQ_CYCLE.Idle)
            {
                if (Stat.bReqStop) return false;

                //int c = 1, r = 0; 
                bool bEndWork      = DM.ARAY[(int)ri.LENS].CheckAllStat(cs.Empty);
                                   
                bool bExistPlace   = DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn ) != 0 || 
                                     DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn ) != 0 ;
                                   
                //언노운없고 
                bool bFullHdrWork  = DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.LastVisn) == 0 && 
                                     DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 0 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.LastVisn) == 0 ;

                bool bFullHdrNone  = DM.ARAY[(int)ri.REAR].CheckAllStat(cs.None) && 
                                     DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.None);
                                   
                bool bUnlockWork   = DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) != 0 ||
                                     DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) != 0;
                                   
                bool bUnlockVisn   = DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) != 0 ||
                                     DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) != 0;

                bool bUnlockEndWrk = DM.ARAY[(int)ri.REAR].CheckAllStat(cs.Empty) &&
                                     DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.Empty);

                bool bToolEmpty    = DM.ARAY[(int)ri.PICK].CheckAllStat(cs.Empty) || (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty) + DM.ARAY[(int)ri.PICK].GetCntStat(cs.None)==2);//툴1개만 사용시 고려.

                bool bUnkwnWork    = DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) != 0||
                                     DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) != 0;

                bool bOneRearWork  = DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipFrnt;
                bool bOneFrntWork  = DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && OM.CmnOptn.bSkipRear;
                bool bOneFullWork  = DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0 && (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0);

                int iTemp = DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) ;

                //Normal
                //20181102 바이오프요청 다끝나고 한번에 검사.
                //bool isNorVisnLens = DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) != 0 && (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) == 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && bUnkwnWork && !OM.MstOptn.bUnlock  &&  // 기본동작;
                //                    (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) - DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) > 0 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) - DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) > 0)&&
                //                     DM.ARAY[(int)ri.FRNT].GetCntStat(cs.AtVisn)==0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.AtVisn)==0;
                //bool isNorPick     = DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) != 0 && (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty)!= 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && !OM.MstOptn.bUnlock;
                //bool isNorAlign    = (DM.ARAY[(int)ri.PICK].CheckAllStat(cs.Visn) || (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0)) ||
                //                     (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0 && (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) - DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0 || DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) - DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 0)) &&
                //                     !OM.MstOptn.bUnlock;
                //bool isNorVisnHldr = !bExistPlace && (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) != 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && !OM.MstOptn.bUnlock;// 기본동작;
                //bool isNorPlace    =  bExistPlace && (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align)!= 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && !OM.MstOptn.bUnlock;
                //bool isNorAfterVisn= (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.AtVisn)>0 || DM.ARAY[(int)ri.REAR].GetCntStat(cs.AtVisn)>0) && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) == 0;
                //bool isNorOut      = (((DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0)|| bFullWork) && (DM.ARAY[(int)ri.PICK].CheckAllStat(cs.Empty) && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0)&& !OM.MstOptn.bUnlock) ||
                //                      ((DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.None)) && (DM.ARAY[(int)ri.PICK].CheckAllStat(cs.Empty) && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && !OM.MstOptn.bUnlock) ||
                //                      ((DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].CheckAllStat(cs.None)) && (DM.ARAY[(int)ri.PICK].CheckAllStat(cs.Empty) && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && !OM.MstOptn.bUnlock);
                //     isNorOut      = isNorOut && DM.ARAY[(int)ri.REAR].GetCntStat(cs.AtVisn) == 0 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.AtVisn) == 0 ;

                //마지막검사 작업완료 하고 한번에 하기.
                bool isNorVisnLens = DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) != 0 && (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) == 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && bUnkwnWork && !OM.MstOptn.bUnlock  &&  // 기본동작;
                                    (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) - DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) > 0 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) - DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) > 0);
                bool isNorPick     = DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) != 0 && (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty)!= 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && !OM.MstOptn.bUnlock;

                bool isNorAlign    = (DM.ARAY[(int)ri.PICK].CheckAllStat(cs.Visn) || (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0)) ||
                                     (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0 && (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) - DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0 || DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) - DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 0)) &&
                                     !OM.MstOptn.bUnlock;
                bool bExistHldrUnkn = DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn)>0 || DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn)>0 ;
                bool isNorVisnHldr = !bExistPlace && (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) != 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && bExistHldrUnkn && !OM.MstOptn.bUnlock;// 기본동작;
                bool isNorPlace    =  bExistPlace && (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align)!= 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 0) && !OM.MstOptn.bUnlock;
                

                //에프터비전 있고 피커에 없고 비전이나 얼라인 없을때.
                bool isNorAfterVisn= (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.LastVisn)>0 || DM.ARAY[(int)ri.REAR].GetCntStat(cs.LastVisn)>0) && 
                                      DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) == 0 && 
                                     !bUnkwnWork && !bExistPlace ;

                //렌즈와 트레이가 동시에 작업이 끝나면 렌즈 오아 조건 때문에  AfterVisn검사 안하고 튀어 나옴...
                bool bHdrNeedAfterVsn= (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn   ) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn   )) == 0 &&
                                       (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.LastVisn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.LastVisn)) > 0 ;
                bool isNorOutHolder  = bFullHdrWork ; //홀더쪽에 언노운 , 비전 , 라스트비전 할게 없고.                                       
                bool isNorOutLens    = DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0 && !bHdrNeedAfterVsn; //작업할께 없을때.
                bool isNorOutEtcAnd  = bToolEmpty         && //픽커에 자제 없고. 
                                      !OM.MstOptn.bUnlock ; //언락모드가 아닐때.                
                bool isNorOut        = (isNorOutHolder || isNorOutLens) && isNorOutEtcAnd ;
                bool isNorPlaceGood  = DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align)!= 0 && !bExistHldrUnkn && !bExistPlace ;


                //End
                bool isEndAlign     = bEndWork && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn ) != 0 && !OM.MstOptn.bUnlock;


                //툴 1개만 사용
                bool isOneVisnLens = (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty) == 1 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 1) &&
                                      DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) != 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0 && !OM.MstOptn.bUnlock;
                bool isOnePick     =  DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty ) == 1 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 1 && !OM.MstOptn.bUnlock;
                bool isOneAlign    =  DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn ) == 1 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 1 && !OM.MstOptn.bUnlock;
                bool isOneVisnHldr = (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) == 1 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 1) && bUnkwnWork && !OM.MstOptn.bUnlock;
                bool isOnePlace    = (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) == 1 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.None) == 1) && bExistPlace  && !OM.MstOptn.bUnlock;

                

                bool isVisnLens  = isNorVisnLens || isOneVisnLens;
                bool isPick      = isNorPick     || isOnePick    ;
                bool isAlign     = isNorAlign    || isEndAlign    || isOneAlign   ;
                bool isVisnHldr  = isNorVisnHldr || isOneVisnHldr;
                bool isPlace     =(isNorPlace    || isOnePlace   ) && OM.DevOptn.iWorkMode == ScrewType ;
                bool isPlace2    =(isNorPlace    || isOnePlace   ) && OM.DevOptn.iWorkMode == LockType  ;
                bool isAfterVisn = isNorAfterVisn ;
                bool isOut       = isNorOut       ;
                bool isPlaceGood = isNorPlaceGood ; //홀더비전 재수없게 페일나서 채결할 수 없을때 다시 트레이에 놓는다.

                
                bool iWorkEnd = (DM.ARAY[(int)ri.LENS].CheckAllStat(cs.None) || bFullHdrNone) ||
                                (DM.ARAY[(int)ri.REAR].CheckAllStat(cs.None) && OM.CmnOptn.bSkipFrnt) ||
                                (DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.None) && OM.CmnOptn.bSkipRear);


                bool isUnlockVisn  = bUnlockWork && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) == 0 && OM.MstOptn.bUnlock && OM.DevOptn.iWorkMode == ScrewType;
                bool isUnlockVisn2 = bUnlockWork && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) == 0 && OM.MstOptn.bUnlock && OM.DevOptn.iWorkMode == LockType ;
                bool isUnlock      = (bUnlockVisn && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty) != 0 && OM.MstOptn.bUnlock ||
                                     ((DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 1 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) == 1) && 
                                     !DM.ARAY[(int)ri.REAR].CheckAllStat(cs.Work) && !DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.Work))) && OM.DevOptn.iWorkMode == ScrewType;
                bool isUnlock2     = (bUnlockVisn && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty) != 0 && OM.MstOptn.bUnlock ||
                                     ((DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 1 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) == 1) && 
                                     !DM.ARAY[(int)ri.REAR].CheckAllStat(cs.Work) && !DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.Work))) && OM.DevOptn.iWorkMode == LockType;
                bool isUnlockAlign = DM.ARAY[(int)ri.PICK].CheckAllStat(cs.Visn)     || (bOneRearWork || bOneFrntWork || bOneFullWork) && OM.MstOptn.bUnlock;
                bool isUnlockPlace = DM.ARAY[(int)ri.LENS].GetCntStat(cs.Empty) != 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Align) != 0 && OM.MstOptn.bUnlock;

                if (OM.MstOptn.bUnlock)
                {
                    if ((bToolEmpty && bFullHdrWork) || (bToolEmpty && OM.CmnOptn.bSkipFrnt && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 0) ||
                        (bToolEmpty && OM.CmnOptn.bSkipRear && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 0) || DM.ARAY[(int)ri.LENS].CheckAllStat(cs.Unkwn))
                    {
                        //Log.ShowMessage("Tray", "Unlock Work End");
                        return true;
                    }
                    
                }
                //여기부터 조건 잡자.


                if (SM.ER.IsErr()) return false;
                //Normal Decide Step.
                     if (isOut        ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Out       ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isPlaceGood  ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.PlaceGood ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isAfterVisn  ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.AfterVisn ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isPlace      ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Place     ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isPlace2     ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Place2    ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isVisnHldr   ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.VisnHldr  ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); } 
                else if (isAlign      ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Align     ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); } 
                else if (isPick       ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Pick      ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); } 
                else if (isVisnLens   ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.VisnLens  ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                
                else if (isUnlockPlace) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.UnlockPlace; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isUnlock     ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Unlock     ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isUnlock2    ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.Unlock2    ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isUnlockAlign) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.UnlockAlign; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isUnlockVisn ) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.UnlockVisn ; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (isUnlockVisn2) { Log.Trace(m_sPartName, sCycle +" Stt"); Step.iSeq = EN_SEQ_CYCLE.UnlockVisn2; InitCycleStep(); m_CycleTime[(int)Step.iSeq].Start(); }
                else if (iWorkEnd     ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;
            }

            //Cycle.
            Step.iLastSeq = Step.iSeq;
            switch (Step.iSeq)
            {
                default                        :                           Log.Trace(m_sPartName, "default End");                                    Step.iSeq = EN_SEQ_CYCLE.Idle;   return false;
                case (EN_SEQ_CYCLE.Idle       ):                                                                                                                                      return false;
                case (EN_SEQ_CYCLE.VisnLens   ): if (CycleVisnLens   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Pick       ): if (CyclePick       ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Align      ): if (CycleAlign      ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.VisnHldr   ): if (CycleVisnHldr   ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Place      ): if (CyclePlace      ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Place2     ): if (CyclePlace2     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.AfterVisn  ): if (CycleAfterVisn  ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.PlaceGood  ): if (CyclePlaceGood  ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Out        ): if (CycleOut        ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;

                case (EN_SEQ_CYCLE.UnlockVisn ): if (CycleUnlockVisn ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.UnlockVisn2): if (CycleUnlockVisn2()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Unlock     ): if (CycleUnlock     ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.Unlock2    ): if (CycleUnlock2    ()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.UnlockAlign): if (CycleUnlockAlign()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
                case (EN_SEQ_CYCLE.UnlockPlace): if (CycleUnlockPlace()) { Log.Trace(m_sPartName, sCycle+" End"); m_CycleTime[(int)Step.iSeq].End(); Step.iSeq = EN_SEQ_CYCLE.Idle; } return false;
            }
        }

        //인터페이스 상속 끝.==================================================

        protected double GetLastCmd(mi _iMotr)
        {
            double dLastIdxPos = 0.0;

            //sunsun
                //if (!SM.MT.GetAlarmSgnl((int)_iMotr) && !SM.MT.GetPLimSnsr((int)_iMotr)) dLastIdxPos = SM.MT.GetCmdPos((int)_iMotr);
                //else dLastIdxPos = GetMotrPos(_iMotr, (EN_PSTN_VALUE)0);

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
        //protected CArray[] Aray;
        
        public enum EN_FINDCHIP : int
        {
            FrstRowCol     = 0 ,
            FrstColRow     = 1 ,
            LastRowCol     = 2 ,
            FrstRowLastCol = 3 ,
            LastRowFrstCol = 4 ,
            LastColFrstRow = 5 ,
            FrstColLastRow = 6 ,
            LastColRow     = 7 , 
            MAX_FINDCHIP      
        };

        public EN_FINDCHIP enFindChip;

        public bool FindChip(ref int c, ref int r, cs _iChip, ri _iId) //이거 되는지 확인 해야함
        { 
            if(_iChip == cs.LastVisn)
            {
                r =DM.ARAY[(int)_iId].FindFrstRow(cs.LastVisn);
                if(r % 2 == 0)
                {
                    return DM.ARAY[(int)_iId].FindFrstRowCol(_iChip, ref c, ref r);
                }
                else
                {
                    return DM.ARAY[(int)_iId].FindFrstRowLastCol(_iChip, ref c, ref r);
                }

            }
            else
            {
                return DM.ARAY[(int)_iId].FindFrstRowCol(_iChip, ref c, ref r);
                
            }


            

        }
        protected struct TWorkInfo {
            public int iCol ;
            public int iRow ;
            public cs eStat ;
        } ;//오토런에서 스테이지에서 정보를 가져다 담아 놓고 Cycle에서 이것을 쓴다....

        public double GetMotrPos (mi _iMotr , pv _iPstnValue )
        {
            return PM.GetValue((uint)_iMotr , (uint)_iPstnValue);
        }
        
        public void InitCycleName(){
            m_sCycleName = new String[(int)EN_SEQ_CYCLE.MAX_SEQ_CYCLE] ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Idle       ]="Idle       " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.VisnLens   ]="VisnLens   " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Pick       ]="Pick       " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Align      ]="Align      " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.VisnHldr   ]="VisnHldr   " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Place      ]="Place      " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Place2     ]="Place2     " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.AfterVisn  ]="AfterVisn  " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.PlaceGood  ]="PlaceGood  " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Out        ]="Out        " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.ToolCalib  ]="ToolCalib  " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.UnlockVisn ]="UnlockVisn " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.UnlockVisn2]="UnlockVisn2" ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Unlock     ]="Unlock     " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.Unlock2    ]="Unlock2    " ;
            m_sCycleName[(int)EN_SEQ_CYCLE.UnlockAlign]="UnlockAlign" ;
            m_sCycleName[(int)EN_SEQ_CYCLE.UnlockPlace]="UnlockPlace" ;
        }

        public void InitCycleTime()
        {
            
        }

        public void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() &&!OM.MstOptn.bDebugMode, 5000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                SetErr(ei.ETC_AllHomeTO ,sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iHome = 0 ;
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
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iHome);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iHome = 0 ;
                    return false ;

                case  0:
                    return false ;

                case 10:
                    //if (EmbededExe.GetHPSHandle() < 0) return false;

                    Step.iHome++;
                    return false;

                case 11:
                    SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                    SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                    SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                    SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                    SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                    SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );


                    //20181018 세타모터 홈잡을때 자꾸 알람이 떠서 원래있던 패턴을 주석처리 하고 요것저것 해본다.
                    //이것도 왜 홈던을 트루 시키고 하는지 모르겠음.
                    //SM.MT.SetServo   ((int)mi.PCK_TL, false);
                    //SM.MT.SetHomeDone((int)mi.PCK_TL, true );
                    //SM.MT.SetReset   ((int)mi.PCK_TL, true );
                    //SM.MT.SetServo   ((int)mi.PCK_TL, true );
                    //
                    //SM.MT.SetServo   ((int)mi.PCK_TR, false);
                    //SM.MT.SetHomeDone((int)mi.PCK_TR, true );
                    //SM.MT.SetReset   ((int)mi.PCK_TR, true );
                    //SM.MT.SetServo   ((int)mi.PCK_TR, true );

                    //홈던깨기.
                    //SM.MT.SetServo((int)mi.PCK_TL, false);
                    //Delay(50);
                    //SM.MT.SetHomeDone((int)mi.PCK_TL, false);
                    //SM.MT.GoHome((int)mi.PCK_TL);
                    //Delay(50);
                    //SM.MT.SetServo((int)mi.PCK_TL, true);
                    //Delay(50);
                    //
                    ////홈던깨기
                    //SM.MT.SetServo((int)mi.PCK_TR, false);
                    //Delay(50);
                    //SM.MT.SetHomeDone((int)mi.PCK_TR, false);
                    //SM.MT.GoHome((int)mi.PCK_TR);
                    //Delay(50);
                    //SM.MT.SetServo((int)mi.PCK_TR, true);
                    //Delay(50);

                    SM.MT.SetHomeDone((int)mi.PCK_TL, false);
                    SM.MT.SetHomeDone((int)mi.PCK_TR, false);
                    Step.iHome++;
                    return false;

                case 12:
                    SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 16);
                    SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 16);
                    SM.MT.GoHome((int)mi.PCK_ZL);
                    SM.MT.GoHome((int)mi.PCK_ZR);
                    Step.iHome++;
                    return false ;
            
                case 13: 
                    
                    
                    
                    
                    
                    
                    Step.iHome++;
                    return false;

                case 14:
                    if (!SM.MT.GetHomeDone((int)mi.PCK_ZL)) return false;
                    if (!SM.MT.GetHomeDone((int)mi.PCK_ZR)) return false;
                    SM.MT.GoHome((int)mi.PCK_X);
                    SM.MT.GoHome((int)mi.PCK_TL);
                    SM.MT.GoHome((int)mi.PCK_TR);
                    SM.MT.GoHome((int)mi.STG_Y);


                    Step.iHome++;
                    return false;

                case 15: 
                    if (!SM.MT.GetHomeDone((int)mi.STG_Y)) return false;
                    if (!SM.MT.GetHomeDone((int)mi.PCK_X)) return false;

                    Step.iHome++;
                    return false;

                case 16: 
                    MoveMotr(mi.PCK_X, pv.PCK_XWait);
                    MoveMotr(mi.STG_Y, pv.STG_YWait);
                    Step.iHome++;
                    return false;

                case 17: 
                    if(!GetStop(mi.PCK_X)) return false;
                    if(!GetStop(mi.STG_Y)) return false;

                    //통신타입이라 좀 늦게 확인 한다.
                    if (!SM.MT.GetHomeDone((int)mi.PCK_TL)) return false;
                    if (!SM.MT.GetHomeDone((int)mi.PCK_TR)) return false;

                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);
                    Step.iHome++;
                    return false;

                case 18: 
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;
                    if (!GetStop(mi.PCK_TL, true)) return false;
                    if (!GetStop(mi.PCK_TR, true)) return false;
                    Step.iHome = 0;
                    return true ;
            }
        }

        private bool GetSortInfo(bool _bUseMulti, ref TSortInfo _SortInfo)
        {
            _SortInfo.iToolShift = 0 ;
            _SortInfo.bLeftWork  = false ;
            _SortInfo.bRightWork = false ;
            _SortInfo.iTrayC     = 0 ;
            _SortInfo.iTrayR     = 0 ;

            int r = 0 ;
            int c = 0 ;

            if (DM.ARAY[(int)_SortInfo.eTool].GetStat(0, 0) == _SortInfo.ePickChip)//왼쪽툴에 작업 할것이 있을때.
            {
                if (!FindChip(ref c, ref r, _SortInfo.eTrayChip, _SortInfo.eTray))
                {
                    return false;
                }

                _SortInfo.bLeftWork = true;
                _SortInfo.iTrayC = c;
                _SortInfo.iTrayR = r;
                _SortInfo.iToolShift = 0;

                return true;
            }

            //Picker 2개 동시에 같이 내리는 부분
            //if(DM.ARAY[(int)_SortInfo.eTool].GetStat(0,0)==_SortInfo.ePickChip)//왼쪽툴에 작업 할것이 있을때.
            //{                
            //    if(!FindChip(ref c , ref r , _SortInfo.eTrayChip , _SortInfo.eTray))
            //    {
            //        return false ;                    
            //    }

            //    _SortInfo.bLeftWork  = true ;
            //    _SortInfo.iTrayC = c ;
            //    _SortInfo.iTrayR = r ;
            //    _SortInfo.iToolShift = 0 ;

            //    //오른쪽 툴 확인.
            //                          //5가 들어감. 
            //    if(_SortInfo.iTrayC + OM.DevOptn.iPCKGapCnt < DM.ARAY[(int)_SortInfo.eTray].GetMaxCol()){
            //        if(DM.ARAY[(int)_SortInfo.eTray].GetStat(_SortInfo.iTrayC + OM.DevOptn.iPCKGapCnt , r) == _SortInfo.eTrayChip)
            //        {
            //            //_SortInfo.bRightWork = _bUseMulti ;
            //            _SortInfo.bRightWork = _bUseMulti;
            //        }
            //    }

            //    return true ;
            //}


            //왼쪽 툴에 작업 할 것이 없고 오른쪽 툴에만 작업 할것이 잇을때.
            else if(DM.ARAY[(int)_SortInfo.eTool].GetStat(1,0)==_SortInfo.ePickChip)
            {
                if(!FindChip(ref c , ref r , _SortInfo.eTrayChip , _SortInfo.eTray))
                {
                    return false ;                    
                }
                _SortInfo.bRightWork = false ;
                _SortInfo.iToolShift = 1 ;
                _SortInfo.iTrayC = c ;
                _SortInfo.iTrayR = r ;
                _SortInfo.bRightWork = true ;

                return true ;
            }
            else 
            {
                
                return false ;
            }
        }

        private bool GetVisnRslt(string _sMsg , ref double _dX , ref double _dY , ref double _dT)
        {
            Log.Trace("VisinRsltParsing", _sMsg);

            if (_sMsg == "") return false;
            //sData == "X3.001,Y1.002,T1.030";
            string [] sVisnDatas = new string[3];
            string sTemp = _sMsg;// "";
            sVisnDatas = _sMsg.Split(';');
            
            //X값.
            //if(_sMsg.Substring(0,1)!="X") return false ;
            //sTemp = _sMsg.Substring(1,5);
            //if (CConfig.StrToDoubleDef(sTemp, -9.999) != CConfig.StrToDoubleDef(sTemp, 9.999)) return false ;
            if (sVisnDatas[0] == "NG") return false;
            sTemp = sVisnDatas[0].Substring(1, sVisnDatas[0].Length - 1);
            _dX = CConfig.StrToDoubleDef(sTemp, 0);
            

            //Y값.
            //if(_sMsg.Substring(7,1)!="Y") return false ;
            //sTemp = _sMsg.Substring(8,5);
            //if (CConfig.StrToDoubleDef(sTemp, -9.999) != CConfig.StrToDoubleDef(sTemp, 9.999)) return false ;
            //_dY = CConfig.StrToDoubleDef(sTemp, 0);
            if (sVisnDatas[1] == "NG") return false;
            sTemp = sVisnDatas[1].Substring(1, sVisnDatas[1].Length - 1);
            _dY = CConfig.StrToDoubleDef(sTemp, 0);

            //T값.
            //if(_sMsg.Substring(14,1)!="T") return false ;
            //sTemp = _sMsg.Substring(15,5);
            //if (CConfig.StrToDoubleDef(sTemp, -9.999) != CConfig.StrToDoubleDef(sTemp, 9.999)) return false ;
            //_dT = CConfig.StrToDoubleDef(sTemp, 0);
            if (sVisnDatas[2] == "NG") return false;
            sTemp = sVisnDatas[2].Substring(1, sVisnDatas[2].Length - 1);
            _dT = CConfig.StrToDoubleDef(sTemp, 0);

            return true ;
        }

        public int GetPckStat(cs _eStat)
        {
            int iRet = 0;
            iRet = DM.ARAY[(int)ri.PICK].GetCntStat(_eStat);


            return iRet;
        }

        public bool CycleVisnLens ()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }

            int r = 0 ; 
            int c = 0 ;

            switch (Step.iCycle)
            {
                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:                    
                    return false ;

                case 10:
                    SetY(yi.PCK_EjtLtOn, false);
                    SetY(yi.PCK_EjtRtOn, false);
                    if (OM.CmnOptn.bUseMultiHldr)
                    {
                        if (!FindChip(ref c, ref r, cs.Unkwn, ri.LENS))
                        {
                            Step.iCycle = 0;
                            return true;
                        }

                        if (DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 1)
                        {
                            if (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 1)
                            {
                                Step.iCycle = 0;
                                return true;
                            }
                            
                        }
                        
                    }

                    if (FindChip(ref c, ref r, cs.Visn, ri.LENS))
                    {
                        if ((DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 1 && OM.CmnOptn.bSkipRear) ||
                        (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 1 && OM.CmnOptn.bSkipFrnt) ||
                        (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 1 && DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.Work)) ||
                        (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 1 && DM.ARAY[(int)ri.REAR].CheckAllStat(cs.Work)))
                        {
                            Step.iCycle = 0;
                            return true;
                        }
                    }

                    if (DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 1)
                    {
                        if ((DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 1 && OM.CmnOptn.bSkipRear) ||
                            (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 1 && OM.CmnOptn.bSkipFrnt) ||
                            (DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 1 && DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.Work)) ||
                            (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 1 && DM.ARAY[(int)ri.REAR].CheckAllStat(cs.Work)))
                        {
                            DM.ARAY[(int)ri.PICK].ChangeStat(cs.Visn, cs.Align);
                            Step.iCycle = 0;
                            return true;
                        }
                    }

                    Step.iCycle++;
                    return false;

                case 11:
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    MoveMotr(mi.PCK_TL , pv.PCK_TLVisnZero);
                    MoveMotr(mi.PCK_TR , pv.PCK_TRVisnZero);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;
                    Step.iCycle++;
                    return false;

                case 13: 
                    Step.iCycle++;
                    return false;

                case 14: 
                    Step.iCycle++;
                    return false;

                case 15:                     
                    FindChip(ref c,ref r,cs.Unkwn,ri.LENS);//X는 오른쪽이홈 Y는 rear쪽이홈.
                    MoveMotr(mi.PCK_X , GetMotrPos(mi.PCK_X,pv.PCK_XVisnLensStt) - c * OM.DevInfo.dLensColPitch);
                    MoveMotr(mi.STG_Y , GetMotrPos(mi.STG_Y,pv.STG_YVisnLensStt) - r * OM.DevInfo.dLensRowPitch);
                    //나중엔 비젼 통신이 이리로 와야 함.
                    VC.SendVisnMsg(VC.sm.Ready);
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    //나중에 통신 확인 여기.
                    if(!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr,"Vision Not Ready");
                        Step.iCycle=0 ;
                        return true ;
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                    if (!FindChip(ref c, ref r, cs.Visn, ri.LENS) && DM.ARAY[(int)ri.PICK].GetStat(0, 0) == cs.Empty && !OM.CmnOptn.bIgnrLeftPck)//X는 오른쪽이홈 Y는 앞쪽이홈.
                    {
                        RsltLensL.dX = 0.0;
                        RsltLensL.dY = 0.0;
                        RsltLensL.dT = 0.0;
                    }
                    
                    if ((FindChip(ref c, ref r, cs.Visn, ri.LENS) && DM.ARAY[(int)ri.PICK].GetStat(1, 0) == cs.Empty) ||
                        (!FindChip(ref c, ref r, cs.Visn, ri.LENS) && !OM.CmnOptn.bIgnrRightPck))
                    {
                        RsltLensR.dX = 0.0;
                        RsltLensR.dY = 0.0;
                        RsltLensR.dT = 0.0;
                    }

                    VC.SendVisnMsg(VC.sm.Insp,"0");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                    Step.iCycle++;
                    return false;

                case 18:  
                    if(!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        FindChip(ref c, ref r, cs.Unkwn, ri.LENS);//X는 오른쪽이홈 Y는 rear쪽이홈.
                        if (iPickVisnRetryCnt < OM.CmnOptn.iPickVisnRetryCnt)
                        {
                            DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Unkwn);
                            iPickVisnRetryCnt++;
                            Step.iCycle = 0;
                            return true;
                        }
                        else
                        {
                            DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Fail);
                            iPickVisnRetryCnt = 0;
                            //if (OM.CmnOptn.bUseMultiHldr && GetPckStat(cs.Empty) == 2)
                            //{
                            //    if (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn)
                            //        - DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) > 0)
                            //    {
                            //        Step.iCycle = 10;
                            //        return false;
                            //    }
                            //}
                            Step.iCycle = 0;
                            return true;

                        }
                    }
                    iPlaceVisnRetryCnt = 0;

                    //첫번째 비전 이면.
                    if (!OM.CmnOptn.bUseMultiHldr)
                    {
                        if (!FindChip(ref c, ref r, cs.Visn, ri.PICK) && !OM.CmnOptn.bIgnrLeftPck)//X는 오른쪽이홈 Y는 앞쪽이홈.
                        //if(OM.CmnOptn.bUseLeftPck)
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensL.dX, ref RsltLensL.dY, ref RsltLensL.dT))
                            {
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong Or Failed!");
                                DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                Step.iCycle = 0;
                                return true;
                            }

                            FindChip(ref c, ref r, cs.Unkwn, ri.LENS);
                            DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Visn);
                            iPickVisnRetryCnt=0;

                            if (OM.DevOptn.dLensVisnXTol != 0)
                            {
                                if (RsltLensL.dX < -OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision X=" + RsltLensL.dX + " Range Over Spec(" + OM.DevOptn.dLensVisnXTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltLensL.dX > OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision X=" + RsltLensL.dX + " Range Over Spec(" + OM.DevOptn.dLensVisnXTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dLensVisnYTol != 0)
                            {
                                if (RsltLensL.dY < -OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Y=" + RsltLensL.dY + " Range Over Spec(" + OM.DevOptn.dLensVisnYTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltLensL.dY > OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Y=" + RsltLensL.dY + " Range Over Spec(" + OM.DevOptn.dLensVisnYTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                        }
                        else if (FindChip(ref c, ref r, cs.Visn, ri.PICK) || (!FindChip(ref c, ref r, cs.Visn, ri.PICK) && !OM.CmnOptn.bIgnrRightPck))//두번째 비전이면.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensR.dX, ref RsltLensR.dY, ref RsltLensR.dT))
                            {
                                DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong Or Failed!");
                                Step.iCycle = 0;
                                return true;
                            }

                            FindChip(ref c, ref r, cs.Unkwn, ri.LENS);
                            DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Visn);
                            iPickVisnRetryCnt=0;

                            if (OM.DevOptn.dLensVisnXTol != 0)
                            {
                                if (RsltLensR.dX < -OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision X=" + RsltLensR.dX + " Range Over Spec(" + OM.DevOptn.dLensVisnXTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltLensR.dX > OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision X=" + RsltLensR.dX + " Range Over Spec(" + OM.DevOptn.dLensVisnXTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dLensVisnYTol != 0)
                            {
                                if (RsltLensR.dY < -OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Y=" + RsltLensR.dY + " Range Over Spec(" + OM.DevOptn.dLensVisnYTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltLensR.dY > OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Y=" + RsltLensR.dY + " Range Over Spec(" + OM.DevOptn.dLensVisnYTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }
                        }
                    }

                    if (OM.CmnOptn.bUseMultiHldr)
                    {
                        if (!FindChip(ref c, ref r, cs.Visn, ri.LENS) && GetPckStat(cs.Visn) == 0)//X는 오른쪽이홈 Y는 앞쪽이홈.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensL.dX, ref RsltLensL.dY, ref RsltLensL.dT))
                            {
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong Or Failed!");
                                DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                Step.iCycle = 0;
                                return true;
                            }

                            FindChip(ref c, ref r, cs.Unkwn, ri.LENS);
                            DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Visn);
                            iPickVisnRetryCnt=0;


                            if (GetPckStat(cs.Empty) == 2)
                            {
                                if (DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) 
                                    - DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) > 0)
                                {
                                    Step.iCycle = 10;
                                    return false;
                                }
                            }

                            if ((DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 1 && OM.CmnOptn.bSkipFrnt) ||
                                DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 1)
                            {
                                Step.iCycle = 0;
                                return true;
                            }
                            

                            if (OM.DevOptn.dLensVisnXTol != 0)
                            {
                                if (RsltLensL.dX < -OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision X=" + RsltLensL.dX + " Range Over Spec(" + OM.DevOptn.dLensVisnXTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltLensL.dX > OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision X=" + RsltLensL.dX + " Range Over Spec(" + OM.DevOptn.dLensVisnXTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dLensVisnYTol != 0)
                            {
                                if (RsltLensL.dY < -OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Y=" + RsltLensL.dY + " Range Over Spec(" + OM.DevOptn.dLensVisnYTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltLensL.dY > OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Y=" + RsltLensL.dY + " Range Over Spec(" + OM.DevOptn.dLensVisnYTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                        }
                        else if (FindChip(ref c, ref r, cs.Visn, ri.LENS) || GetPckStat(cs.Visn) == 1)//두번째 비전이면.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensR.dX, ref RsltLensR.dY, ref RsltLensR.dT))
                            {
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong Or Failed!");
                                DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                Step.iCycle = 0;
                                return true;
                            }

                            FindChip(ref c, ref r, cs.Unkwn, ri.LENS);
                            DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Visn);
                            iPickVisnRetryCnt=0;

                            if (OM.DevOptn.dLensVisnXTol != 0)
                            {
                                if (RsltLensR.dX < -OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision X=" + RsltLensR.dX + " Range Over Spec(" + OM.DevOptn.dLensVisnXTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltLensR.dX > OM.DevOptn.dLensVisnXTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision X=" + RsltLensR.dX + " Range Over Spec(" + OM.DevOptn.dLensVisnXTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dLensVisnYTol != 0)
                            {
                                if (RsltLensR.dY < -OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Y=" + RsltLensR.dY + " Range Over Spec(" + OM.DevOptn.dLensVisnYTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltLensR.dY > OM.DevOptn.dLensVisnYTol)
                                {
                                    DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Y=" + RsltLensR.dY + " Range Over Spec(" + OM.DevOptn.dLensVisnYTol + ")");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }
                        }
                    }
                    
                    
                    Step.iCycle = 0;
                    return true;
              
                    
            }
        }
        
        public bool CyclePick()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }

            int r = 0, c = 0;

            
            double dPckXPos = 0;
            double dPckYPos = 0;
            double dPckTPos = 0;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:
                    return false ;

                case 10:
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;
                    
                    FindChip(ref c, ref r, cs.Visn, ri.LENS);

                    //트레이쪽 오프셑.
                    dPckXPos = GetMotrPos(mi.PCK_X,pv.PCK_XVisnLensStt) ;
                    dPckYPos = GetMotrPos(mi.STG_Y,pv.STG_YVisnLensStt) ;
                    dPckXPos -= c * OM.DevInfo.dLensColPitch;
                    dPckYPos -= r * OM.DevInfo.dLensRowPitch;
                    
                    //픽커쪽 오프셑.
                    FindChip(ref c,ref r,cs.Empty,ri.PICK);
                    if(c==0){//픽커 1번 찝을때
                        dPckXPos -= RsltLensL.dX;
                        dPckYPos -= RsltLensL.dY;
                        dPckXPos += GetMotrPos(mi.PCK_X,pv.PCK_XVisnPck1Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y,pv.STG_YVisnPck1Ofs);

                        dPckTPos  = GetMotrPos(mi.PCK_TL, pv.PCK_TLVisnZero);
                        dPckTPos += RsltLensL.dT;

                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableLeftX(dPckTPos);
                            dPckYPos += LookUpTable.GetLookUpTableLeftY(dPckTPos);
                            dPckTPos += LookUpTable.GetLookUpTableLeftT(dPckTPos);
                        }

                        MoveMotr(mi.PCK_TL , dPckTPos);
                    }
                    else {//픽커 2번 찝을때.
                        dPckXPos -= RsltLensR.dX;
                        dPckYPos -= RsltLensR.dY;
                        dPckXPos += GetMotrPos(mi.PCK_X,pv.PCK_XVisnPck2Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y,pv.STG_YVisnPck2Ofs);

                        dPckTPos  = GetMotrPos(mi.PCK_TR, pv.PCK_TRVisnZero);
                        dPckTPos += RsltLensR.dT;

                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableRightX(dPckTPos);
                            dPckYPos += LookUpTable.GetLookUpTableRightY(dPckTPos);
                            dPckTPos += LookUpTable.GetLookUpTableRightT(dPckTPos);
                        }

                        MoveMotr(mi.PCK_TR , dPckTPos);
                    }
                    
                    //XY 움직이고.
                    MoveMotr(mi.PCK_X , dPckXPos);
                    MoveMotr(mi.STG_Y , dPckYPos);

                    //T를 계산.
                    Step.iCycle++;
                    return false ;



                case 12: 
                    if(!GetStop(mi.PCK_X )) return false ;
                    if(!GetStop(mi.STG_Y )) return false ;
                    
                    FindChip(ref c,ref r,cs.Empty,ri.PICK);
                    if(c==0){//픽커 1번 찝을때

                        if (!GetStop(mi.PCK_TL, true)) return false;
                        MoveMotr(mi.PCK_ZL,pv.PCK_ZLPick);
                        SetY(yi.PCK_VacLtOn,false);
                        SetY(yi.PCK_EjtLtOn,false);
                    }
                    else {//픽커 2번 찝을때.
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        MoveMotr(mi.PCK_ZR,pv.PCK_ZRPick);
                        SetY(yi.PCK_VacRtOn,false);
                        SetY(yi.PCK_EjtRtOn,false);
                    }
                    Step.iCycle++;
                    return false;

                case 13: 
                    if(!GetStop(mi.PCK_ZL)) return false ;
                    if(!GetStop(mi.PCK_ZR)) return false ;
                    FindChip(ref c,ref r,cs.Empty,ri.PICK);
                    if(c==0){//픽커 1번 찝을때
                        SetY(yi.PCK_VacLtOn,true);
                    }
                    else {//픽커 2번 찝을때.
                        SetY(yi.PCK_VacRtOn,true);
                    }
                    
                    m_tmDelay.Clear();
                    Step.iCycle ++;
                    return false;
                
                case 14:
                    FindChip(ref c,ref r,cs.Empty,ri.PICK);
                    if(c==0){//픽커 1번 찝을때
                        if (GetX(xi.PCK_VacLt))
                        {
                            Step.iCycle++;
                            return false ;
                        }
                    }
                    else {//픽커 2번 찝을때.
                        if (GetX(xi.PCK_VacRt))
                        {
                            Step.iCycle++;
                            return false ;
                        }
                    }
                    
                    if(!m_tmDelay.OnDelay(1000))return false ;
                    
                    Step.iCycle = 50 ;
                    return false ;

                //배큠성공시.
                case 15:
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 16: 
                    if(!GetStop(mi.PCK_ZL)) return false ;
                    if(!GetStop(mi.PCK_ZR)) return false ;

                    FindChip(ref c,ref r,cs.Empty,ri.PICK);
                    if(c==0){//픽커 1번 찝을때
                        MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    }
                    else {//픽커 2번 찝을때.
                        MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);
                    }  

                    //여기서 배큠 확인 하면 실물과 다를때 예외 처리 해야 되서 안함.
                    FindChip(ref c,ref r,cs.Empty,ri.PICK);
                    DM.ARAY[(int)ri.PICK].SetStat(c,r,cs.Visn);                   
                    

                    FindChip(ref c,ref r,cs.Visn,ri.LENS);
                    DM.ARAY[(int)ri.LENS].SetStat(c,r,cs.Empty);

                    Step.iCycle=0;
                    return true;
                    //정상종료 끝.

                //위에서 씀.
                //배큠에러시에.
                case 50: 
                    FindChip(ref c,ref r,cs.Empty,ri.PICK);
                    if(c==0){//픽커 1번 찝을때
                        SetY(yi.PCK_VacLtOn , false);
                        SetY(yi.PCK_EjtLtOn , true );
                    }
                    else {//픽커 2번 찝을때.
                        SetY(yi.PCK_VacRtOn , false);
                        SetY(yi.PCK_EjtRtOn , true );
                    }
                    m_tmDelay.Clear();
                    
                    Step.iCycle++;
                    return false;

                case 51: 
                    if(!m_tmDelay.OnDelay(300))return false ;
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 52: 
                    if(!GetStop(mi.PCK_ZL)) return false ;
                    if(!GetStop(mi.PCK_ZR)) return false ;
                    SetY(yi.PCK_EjtLtOn, false);
                    SetY(yi.PCK_EjtRtOn, false);
                    FindChip(ref c,ref r,cs.Empty,ri.PICK);  
                    //DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);

                    if(c==0){//픽커 1번 찝을때
                        
                        MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                        
                    }
                    else {//픽커 2번 찝을때.
                        MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);
                    }                
                    
                    //리트라이 카운트.
                    if(iPickRetryCnt < OM.CmnOptn.iPickRetryCnt)
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Unkwn);
                        iPickRetryCnt++;
                    }
                    else
                    {
                        //if(c==0)
                        //{//픽커 1번 찝을때
                        //    SetErr(ei.PRT_VacErr , "Left Picker Pickup Vaccum Error");
                        //}
                        //else
                        //{
                        //     SetErr(ei.PRT_VacErr , "Right Picker Pickup Vaccum Error");
                        //}
                        FindChip(ref c, ref r, cs.Visn, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Fail);
                        //DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Fail);
                        iPickRetryCnt=0;
                    }
                    Step.iCycle=0;
                    return true;
            }
        }
        
        public bool CycleAlign()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:
                    return false ;

                case 10:
                    DM.ARAY[(int)ri.PICK].ChangeStat(cs.Visn, cs.Align);     
                    
                    Step.iCycle = 0;
                    return true;
            }
        }


        int iR = 0 ;
        int iC = 0 ;
        ri  iTray = ri.NONE ;
        double dMoveX = 0.0 ;
        double dMoveY = 0.0 ;
        public bool CycleVisnHldr ()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;
            

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:
                    
                    return false ;

                case 10: 
                    if(FindChip(ref c, ref r, cs.Unkwn, ri.REAR))//X는 오른쪽이홈 Y는 rear쪽이홈.)
                    {
                        FindChip(ref c, ref r, cs.Unkwn, ri.REAR);//X는 오른쪽이홈 Y는 rear쪽이홈.
                        dMoveX=GetMotrPos(mi.PCK_X,pv.PCK_XVisnRearStt) - c * OM.DevInfo.dRearColPitch;
                        dMoveY=GetMotrPos(mi.STG_Y,pv.STG_YVisnRearStt) - r * OM.DevInfo.dRearRowPitch;
                        iC = c ;
                        iR = r ;
                        iTray = ri.REAR ;
                                               
                    }
                    else if (FindChip(ref c, ref r, cs.Unkwn, ri.FRNT))
                    {
                        dMoveX=GetMotrPos(mi.PCK_X,pv.PCK_XVisnFrntStt) - c * OM.DevInfo.dFrntColPitch;
                        dMoveY=GetMotrPos(mi.STG_Y,pv.STG_YVisnFrntStt) - r * OM.DevInfo.dFrntRowPitch;
                        iC = c ;
                        iR = r ;  
                        iTray = ri.FRNT ;
                    }   

                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;
                    Step.iCycle++;
                    return false;

                case 12: 
                    
                    
                    Step.iCycle++;
                    return false;

                case 13: //일단은 여기서 레디 확인 하나 나중에 디버깅 끝나면 옮기자 렉타임 없게.                
                    
                    Step.iCycle++;
                    return false;

                case 14:       
                    MoveMotr(mi.PCK_X , dMoveX);
                    MoveMotr(mi.STG_Y , dMoveY);
                    VC.SendVisnMsg(VC.sm.Ready);
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;
                    if(!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr,"Vision Not Ready");
                        Step.iCycle=0 ;
                        return true ;
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                    if (OM.CmnOptn.bUseMultiHldr)
                    {
                        if (!FindChip(ref c, ref r, cs.Visn, iTray))
                        {
                            RsltHldrL.dX = 0.0;
                            RsltHldrL.dY = 0.0;
                            RsltHldrL.dT = 0.0;
                        }
                        else
                        {
                            RsltHldrR.dX = 0.0;
                            RsltHldrR.dY = 0.0;
                            RsltHldrR.dT = 0.0;
                        }
                    }
                    else
                    {
                        if (DM.ARAY[(int)ri.REAR].GetStat(0, 0) == cs.Align)
                        {
                            RsltHldrL.dX = 0.0;
                            RsltHldrL.dY = 0.0;
                            RsltHldrL.dT = 0.0;
                        }
                        if (DM.ARAY[(int)ri.REAR].GetStat(1, 0) == cs.Align)
                        {
                            RsltHldrR.dX = 0.0;
                            RsltHldrR.dY = 0.0;
                            RsltHldrR.dT = 0.0;
                        }
                    }

                    

                    VC.SendVisnMsg(VC.sm.Insp,"2");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                    //VC.SendVisnMsg(VC.sm.Insp,"1");
                    Step.iCycle++;
                    return false;

                case 17:  
                    if(!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        if (iPlaceVisnRetryCnt < OM.CmnOptn.iPlaceVisnRetryCnt)
                        {
                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Unkwn);                            
                            iPlaceVisnRetryCnt++;
                            Step.iCycle = 10;
                            return false;

                            //Step.iCycle = 0;
                            //return true;
                        }
                        else
                        {
                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Fail);
                            iPlaceVisnRetryCnt = 0;
                            Step.iCycle = 0;
                            return true;
                            
                        }                        
                    }
                    iPlaceVisnRetryCnt = 0;

                    if (!OM.CmnOptn.bUseMultiHldr)
                    {
                        //첫번째 비전 이면.
                        if (DM.ARAY[(int)ri.PICK].GetStat(0, 0) == cs.Align && !OM.CmnOptn.bIgnrLeftPck)//X는 오른쪽이홈 Y는 앞쪽이홈.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldrL.dX, ref RsltHldrL.dY, ref RsltHldrL.dT))
                            {
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong Or Failed!");
                                DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                Step.iCycle = 0;
                                return true;
                                
                            }

                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataX = RsltHldrL.dX;
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataY = RsltHldrL.dY;
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataT = RsltHldrL.dT;

                            if (OM.DevOptn.dHldrVisnXTol != 0)
                            {
                                if (RsltHldrL.dX < -OM.DevOptn.dHldrVisnXTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltHldrL.dX > OM.DevOptn.dHldrVisnXTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dHldrVisnYTol != 0)
                            {
                                if (RsltHldrL.dY < -OM.DevOptn.dHldrVisnYTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltHldrL.dY > OM.DevOptn.dHldrVisnYTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                        }
                        else if (DM.ARAY[(int)ri.PICK].GetStat(1, 0) == cs.Align && !OM.CmnOptn.bIgnrRightPck)//두번째 비전이면.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldrR.dX, ref RsltHldrR.dY, ref RsltHldrR.dT))
                            {
                                DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong Or Failed!");
                                Step.iCycle = 0;
                                return true;
                            }

                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataX = RsltHldrR.dX;
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataY = RsltHldrR.dY;
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataT = RsltHldrR.dT;

                            if (OM.DevOptn.dHldrVisnXTol != 0)
                            {
                                if (RsltHldrR.dX < -OM.DevOptn.dHldrVisnXTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltHldrR.dX > OM.DevOptn.dHldrVisnXTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dHldrVisnYTol != 0)
                            {
                                if (RsltHldrR.dY < -OM.DevOptn.dHldrVisnYTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltHldrR.dY > OM.DevOptn.dHldrVisnYTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }
                        }
                    }


                    if (OM.CmnOptn.bUseMultiHldr)
                    {
                        //첫번째 비전 이면.
                        if (!FindChip(ref c, ref r, cs.Visn, iTray) && 
                            DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) < 1)//X는 오른쪽이홈 Y는 앞쪽이홈.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldrL.dX, ref RsltHldrL.dY, ref RsltHldrL.dT))
                            {
                                DM.ARAY[(int)iTray].SetStat(iC,iR, cs.Unkwn);
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong Or Failed!");
                                Step.iCycle = 0;
                                return true;
                            }

                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataX = RsltHldrL.dX;
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataY = RsltHldrL.dY;
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataT = RsltHldrL.dT;

                            if (OM.DevOptn.dHldrVisnXTol != 0)
                            {
                                if (RsltHldrL.dX < -OM.DevOptn.dHldrVisnXTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltHldrL.dX > OM.DevOptn.dHldrVisnXTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dHldrVisnYTol != 0)
                            {
                                if (RsltHldrL.dY < -OM.DevOptn.dHldrVisnYTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltHldrL.dY > OM.DevOptn.dHldrVisnYTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (GetPckStat(cs.Align) == 2 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) + DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) < 2)
                            {
                                Step.iCycle = 10;
                                return false;
                            }

                            if ((DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 1 && OM.CmnOptn.bSkipFrnt) ||
                                DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 1)
                            {
                                Step.iCycle = 0;
                                return true;
                            }
                        }
                        else//두번째 비전이면.
                        {
                            if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldrR.dX, ref RsltHldrR.dY, ref RsltHldrR.dT))
                            {
                                DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong Or Failed!");
                                Step.iCycle = 0;
                                return true;                                
                            }

                            DM.ARAY[(int)iTray].SetStat(iC, iR, cs.Visn);
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataX = RsltHldrR.dX;
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataY = RsltHldrR.dY;
                            DM.ARAY[(int)iTray].Chip[iC, iR].dDataT = RsltHldrR.dT;

                            if (OM.DevOptn.dHldrVisnXTol != 0)
                            {
                                if (RsltHldrR.dX < -OM.DevOptn.dHldrVisnXTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                                if (RsltHldrR.dX > OM.DevOptn.dHldrVisnXTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }

                            if (OM.DevOptn.dHldrVisnYTol != 0)
                            {
                                if (RsltHldrR.dY < -OM.DevOptn.dHldrVisnYTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }

                                if (RsltHldrR.dY > OM.DevOptn.dHldrVisnYTol)
                                {
                                    DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                                    DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    SetErr(ei.VSN_InspRangeOver, "Vision Tolerance Range Over");
                                    Step.iCycle = 0;
                                    return true;
                                }
                            }
                        }
                    }
                    Step.iCycle++;
                    return false;

                case 18:
                    Step.iCycle=0;
                    return true ;

            }
        }


        public double dLtSttTime     = 0;
        public double dLtEndTime     = 0;
        public double dRtSttTime     = 0;
        public double dRtEndTime     = 0;
        public double dLtTotalTime   = 0;
        public double dRtTotalTime   = 0;
        public double dAssyTotalTime = 0;

        public int    iRptCnt = 0;
        public bool CyclePlace()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Change Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            double dPckXPos = 0;
            double dPckYPos = 0;


            //Device Option.
            double dTWork          ;
            double dHldrPitch      ; 
            double dThetaWorkSpeed ;
            double dThetaWorkAcc   ; 

            //하드웨어 픽스된 변수들 하드웨어 변경시에 바꿔줘야함.
            const double dTUnitPerRot = 360;//Degree
            const double dZUnitPerRot = 2;//mm
            const double dZ_TRotRatio = dZUnitPerRot / dTUnitPerRot;
            double dHldrPitch_ZRotRatio;


            double dZMovePos ;
            double dZSpeed  ;
            double dZAcc;

            //bool bLeftTool = false; //Sensor에 의해 멈췄는지 포지션으로 멈췄는지 확인 용
            //bool bRightTool = false;
            
            double dTemp;
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Cycle Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:
                    return false ;

                case 10: //소팅정보 확인 하고 Z축은 위로 올림.
                    SortInfo.bPick = false ;
                    SortInfo.eTool = ri.PICK ;
                    if(FindChip(ref c, ref r , cs.Visn,ri.REAR))SortInfo.eTray = ri.REAR ;
                    else                                        SortInfo.eTray = ri.FRNT ;
                    SortInfo.ePickChip = cs.Align ;
                    SortInfo.eTrayChip = cs.Visn  ;
                    if (!GetSortInfo(OM.CmnOptn.bUseMultiHldr, ref SortInfo))
                    {
                        Step.iCycle = 0 ;
                        return true ;
                    }

                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);

                    Step.iCycle++;
                    return false;

                case 11://올라온거 확인 하고 혹시 세타 모터 알람 있는지 확인.
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;

                    if(SM.MT.GetAlarmSgnl((int)mi.PCK_TL) || SM.MT.GetAlarmSgnl((int)mi.PCK_TR))
                    {
                        SM.ER.SetErrMsg((int)ei.MTR_Alarm, "Theta Motor Alarm");
                        Step.iCycle=0;
                        return true ;
                    }

                    Step.iCycle++;
                    return false;

                case 12://비전값 보정하여 포지션에 넣고 이동.
                    if (SortInfo.eTray == ri.REAR)
                    {
                        dPckXPos = GetMotrPos(mi.PCK_X,pv.PCK_XVisnRearStt) ;
                        dPckYPos = GetMotrPos(mi.STG_Y,pv.STG_YVisnRearStt) ;
                        dPckXPos += SortInfo.iToolShift * OM.DevOptn.iPCKGapCnt * OM.DevInfo.dRearColPitch ;
                        dPckXPos -= SortInfo.iTrayC * OM.DevInfo.dRearColPitch;//X는 오른쪽이홈 Y는 앞쪽이홈.
                        dPckYPos -= SortInfo.iTrayR * OM.DevInfo.dRearRowPitch;
                    }
                    else
                    {
                        dPckXPos = GetMotrPos(mi.PCK_X,pv.PCK_XVisnFrntStt) ;
                        dPckYPos = GetMotrPos(mi.STG_Y,pv.STG_YVisnFrntStt) ;
                        dPckXPos += SortInfo.iToolShift * OM.DevOptn.iPCKGapCnt * OM.DevInfo.dFrntColPitch ;
                        dPckXPos -= SortInfo.iTrayC * OM.DevInfo.dFrntColPitch;//X는 오른쪽이홈 Y는 앞쪽이홈.
                        dPckYPos -= SortInfo.iTrayR * OM.DevInfo.dFrntRowPitch;
                    }
                    
                    FindChip(ref c,ref r,cs.Align,ri.PICK);

                    if(c==0){//픽커 1번 찝을때
                        dPckXPos -= RsltHldrL.dX;
                        dPckYPos -= RsltHldrL.dY;
                        dPckXPos += GetMotrPos(mi.PCK_X,pv.PCK_XVisnPck1Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y,pv.STG_YVisnPck1Ofs);
                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableLeftX(GetMotrPos(mi.PCK_TL,pv.PCK_TLVisnZero));
                            dPckYPos += LookUpTable.GetLookUpTableLeftY(GetMotrPos(mi.PCK_TL,pv.PCK_TLVisnZero));
                        }
                    }
                    else {//픽커 2번 찝을때.
                        dPckXPos -= RsltHldrR.dX;
                        dPckYPos -= RsltHldrR.dY;
                        dPckXPos += GetMotrPos(mi.PCK_X,pv.PCK_XVisnPck2Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y,pv.STG_YVisnPck2Ofs);
                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableRightX(GetMotrPos(mi.PCK_TR, pv.PCK_TRVisnZero));
                            dPckYPos += LookUpTable.GetLookUpTableRightY(GetMotrPos(mi.PCK_TR, pv.PCK_TRVisnZero));
                        }
                    }

                    MoveMotr(mi.PCK_X , dPckXPos);
                    MoveMotr(mi.STG_Y , dPckYPos);

                    Step.iCycle++;
                    return false ;

                case 13: //정지 확인 하고 Z축 내려놓는다.
                    if(!GetStop(mi.PCK_X)) return false;
                    if(!GetStop(mi.STG_Y)) return false;

                    if (SortInfo.bLeftWork)
                    {
                        if(SortInfo.eTray == ri.FRNT) DM.ARAY[(int)ri.FRNT].Chip[SortInfo.iTrayC,SortInfo.iTrayR].iTag = 0;
                        else                          DM.ARAY[(int)ri.REAR].Chip[SortInfo.iTrayC,SortInfo.iTrayR].iTag = 0; 
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLPlce);
                    }
                    if (SortInfo.bRightWork)
                    {
                        if(SortInfo.eTray == ri.FRNT) DM.ARAY[(int)ri.FRNT].Chip[SortInfo.iTrayC,SortInfo.iTrayR].iTag = 1;
                        else                          DM.ARAY[(int)ri.REAR].Chip[SortInfo.iTrayC,SortInfo.iTrayR].iTag = 1; 
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRPlce);
                    }

                    Step.iCycle++;
                    return false ;

                case 14: //Z축 멈춘거 확인 하고 역방향 돌리기 시작.
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;

                    
                    if (SortInfo.bLeftWork)
                    {
                        //SM.IO.SetY((int)yi.PCK_VacLtOn, false);
                        dLtSttTime = DateTime.Now.ToOADate();
                        SM.MT.GoIncRun((int)mi.PCK_TL, -(PM.GetValue(mi.PCK_TL, pv.PCK_TLRvrsWork)));
                    }
                    if (SortInfo.bRightWork)
                    {
                        //SM.IO.SetY((int)yi.PCK_VacRtOn, false);
                        dRtSttTime = DateTime.Now.ToOADate();
                        SM.MT.GoIncRun((int)mi.PCK_TR, -(PM.GetValue(mi.PCK_TR, pv.PCK_TRRvrsWork)));
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 15://통신 딜레이 좀 먹이고.
                    if (!m_tmDelay.OnDelay(100)) return false;
                    Step.iCycle++;
                    return false;

                case 16: //멈췄는지 확인 하고 토크설정값 세팅.                    
                    if(SortInfo.bLeftWork)
                    {
                        Log.Trace("TL Bf Cmd", SM.MT.GetCmdPos((int)mi.PCK_TL).ToString());
                        if (!GetStop(mi.PCK_TL, true)) return false;
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax"      , OM.DevOptn.dTorqueMax);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.DevOptn.dTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime" , OM.DevOptn.dTorqueTime);

                        SM.MT.SetServo   ((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true );
                        SM.MT.SetServo   ((int)mi.PCK_TL, true );
                    }
                    if(SortInfo.bRightWork)
                    {
                        Log.Trace("TR Bf Cmd", SM.MT.GetCmdPos((int)mi.PCK_TR).ToString());
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.DevOptn.dTorqueMax);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.DevOptn.dTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.DevOptn.dTorqueTime);

                        SM.MT.SetServo   ((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true );
                        SM.MT.SetServo   ((int)mi.PCK_TR, true );
                    }
                    
                    Step.iCycle ++;
                    return false;

                case 17: //Z축과 T축의 포지션 계산하여 정방향 구동.
                    //Device Option.
                    dTWork          = PM.GetValue(mi.PCK_TL,pv.PCK_TLWorkOfs) ; //T1440 Z0.4
                    dHldrPitch      = OM.DevOptn.dHldrPitch      ;
                    dThetaWorkSpeed = OM.DevOptn.dThetaWorkSpeed ; //초당 2바퀴.
                    dThetaWorkAcc   = OM.DevOptn.dThetaWorkAcc   ; //

                    dHldrPitch_ZRotRatio = dHldrPitch ;//하우징 나사산 피치_회전 비
                    dZMovePos = (dTWork / dTUnitPerRot) * dHldrPitch_ZRotRatio;

                    dZSpeed = (dZMovePos / dTWork) * dThetaWorkSpeed ;
                    dZAcc   = (dZMovePos / dTWork) * dThetaWorkAcc   ;

                    if(SortInfo.bLeftWork)
                    {
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TL);
                        SM.MT.GoInc((int)mi.PCK_TL,dTWork   ,dThetaWorkSpeed,dThetaWorkAcc,dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZL,dZMovePos,dZSpeed        ,dZAcc        ,dZAcc        );
                        double ddTemp = dTemp;
                    }
                    if(SortInfo.bRightWork)
                    {
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TR);
                        SM.MT.GoInc((int)mi.PCK_TR,dTWork   ,dThetaWorkSpeed,dThetaWorkAcc,dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZR,dZMovePos,dZSpeed        ,dZAcc        ,dZAcc        );
                        double ddTemp = dTemp;
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 18://Z축 감지센서가 센싱 해제 되면 스탑 시킴.
                    //왼쪽툴 작업.
                    if(SortInfo.bLeftWork)
                    {
                        if (!GetX(xi.PCK_HghtLt))
                        {
                            //20161115 펌웨어추가 
                            if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;
                            SM.MT.Stop((int)mi.PCK_TL);                            
                            SM.MT.Stop((int)mi.PCK_ZL);
                            //bLeftTool = true; //작업완료 추후에 쓰기위해 셑해놓음.
                        }
                        else //센싱해제안되고  bLeftTool 이거빼고 조건추가.
                        {
                            //모터는 다멈췄는데 알람상태도 아니면 센서가 낑기거나 렌즈가 제대로 안찝혔거나.
                            if(GetStop(mi.PCK_ZL) && GetStop(mi.PCK_TL, true)&&!SM.MT.GetAlarmSgnl((int)mi.PCK_TL))
                            {
                                Log.Trace("Sensor Unchecked and Motor normal finshed TL At Cmd", SM.MT.GetCmdPos((int)mi.PCK_TL).ToString());
                                SetErr(ei.PRT_LensPut , "Check the LEFT height sensor or lens");

                                iRptCnt = 0;
                                
                                SetY(yi.PCK_VacLtOn, false);
                                //SetY(yi.PCK_EjtLtOn, true);

                                //ToStop에서함.
                                MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                                MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                            }
                        }
                        
                        //T축 멈췄는지 확인 하고 바로 Z축 멈춤.
                        if (!GetStop(mi.PCK_TL, true)) return false;
                        else                           SM.MT.Stop((int)mi.PCK_ZL);                       
                        //Z축 동시에 멈췄는지 확인.
                        if (!GetStop(mi.PCK_ZL)) return false;  

                        //토크에러 체크안함 옵션에 토크에러 발생 시                        
                        if(SM.MT.GetAlarmSgnl((int)mi.PCK_TL))
                        {
                            if (!OM.CmnOptn.bTorqChck)//토크 체크 모드가 아닐때 토크에러시 끌러서 렌즈트레이에 렌즈를 다시 담음.
                            {
                                if (iRptCnt < OM.DevOptn.iWrkRptCnt) //토크 페일 리핏 카운트가 0보다 크면
                                {
                                    SM.MT.Stop((int)mi.PCK_ZL);
                                    if (SortInfo.eTray == ri.REAR)
                                    {
                                        DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);//홀더를 다시 트라이 하기 위해 언노운으로 마스킹
                                    }
                                    else
                                    {
                                        DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    }    
                                    iRptCnt++;//재시도 카운트 증가.
                                }
                                else
                                {
                                    SM.MT.Stop((int)mi.PCK_ZL);
                                    if (SortInfo.eTray == ri.REAR)
                                    {
                                        FindChip(ref c, ref r, cs.Visn, ri.REAR);
                                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.TorqueFail);
                                    }
                                    else
                                    {
                                        FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.TorqueFail);
                                    }
                                    iRptCnt = 0;//카운트 리셑
                                }

                                //역회전하여 렌즈를 풀른후 렌즈트레이에 담는 패턴.
                                Step.iCycle = 50;
                                return false;
                            }
                            //토크 체크 모드에서의 토크에러 발생 시 토크에러시 역회전 안하고 Z축만 올림.
                            else //if (OM.CmnOptn.bTorqChck) 
                            {
                                SM.MT.Stop((int)mi.PCK_ZL);
                                if (SortInfo.eTray == ri.REAR)
                                {
                                    FindChip(ref c, ref r, cs.Visn, ri.REAR);
                                    DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.TorqueFail);
                                }
                                else
                                {
                                    FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                                    DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.TorqueFail);
                                }
                            
                                //토크에러시에 그냥 렌즈 넵두고 올라오는 패턴.
                                Step.iCycle = 100;
                                return false;
                            }

                        }
                        

                        /*20181015 선계원.
                        //위에 센서 확인 하는곳에 else로 에러띄우게 수정.
                        //높이센서가 아직 다 안내려갔는데 알람도 안뜨고 세타 완료된 경우.
                        //if (!bLeftTool && !SM.MT.GetAlarmSgnl((int)mi.PCK_TL))
                        //{
                        //    //dTemp1 = SM.MT.GetCmdPos((int)mi.PCK_TL);
                        //    Log.Trace("TL At Cmd", SM.MT.GetCmdPos((int)mi.PCK_TL).ToString());
                        //    iRptCnt++;
                        //    if (OM.DevOptn.iWrkRptCnt == 0 || OM.DevOptn.iWrkRptCnt == iRptCnt) //토크 페일 리핏 카운트가 0이면
                        //    {
                        //        if (SortInfo.eTray == ri.REAR)
                        //        {
                        //            FindChip(ref c, ref r, cs.Visn, ri.REAR);
                        //            DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.SensorFail);
                        //        }
                        //        else
                        //        {
                        //            FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                        //            DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.SensorFail);
                        //        }
                        //        iRptCnt = 0;
                        //    }
                        //
                        //    else if (OM.DevOptn.iWrkRptCnt > iRptCnt) //토크 페일 리핏 카운트가 0보다 크면
                        //    {
                        //        if (SortInfo.eTray == ri.REAR)
                        //        {
                        //            DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                        //        }
                        //        else
                        //        {
                        //            DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                        //        }
                        //    }
                        //
                        //    Step.iCycle = 50;
                        //    return false;
                        //}*/

                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax", OM.MstOptn.iMaxTorqueMax);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime", OM.MstOptn.iMaxTorqueTime);

                        SM.MT.SetServo((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true);
                        SM.MT.SetServo((int)mi.PCK_TL, true);
                    }

                    //오른쪽툴 작업.
                    if(SortInfo.bRightWork)
                    {
                        if (!GetX(xi.PCK_HghtRt))
                        {
                            //20161115 펌웨어추가
                            if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;
                            SM.MT.Stop((int)mi.PCK_TR);                            
                            SM.MT.Stop((int)mi.PCK_ZR);
                            //bRightTool = true;
                        }
                        else//센싱해제안되고  bLeftTool 이거빼고 조건추가.
                        {
                            //모터는 다멈췄는데 알람상태도 아니면 센서가 낑기거나 렌즈가 제대로 안찝혔거나.
                            if(GetStop(mi.PCK_ZR) && GetStop(mi.PCK_TR, true)&&!SM.MT.GetAlarmSgnl((int)mi.PCK_TR))
                            {
                                Log.Trace("Sensor Unchecked and Motor normal finshed TR At Cmd", SM.MT.GetCmdPos((int)mi.PCK_TR).ToString());

                                SetErr(ei.PRT_LensPut , "Check the RIGHT height sensor or lens");

                                iRptCnt = 0;
                                
                                SetY(yi.PCK_VacRtOn, false);
                                //SetY(yi.PCK_EjtLtOn, true);

                                //ToStop에서함.
                                MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                                MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                            }
                        }

                        //T축 멈췄는지 확인 하고 바로 Z축 멈춤.
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        else                           SM.MT.Stop((int)mi.PCK_ZR);
                        //Z축 동시에 멈췄는지 확인.
                        if (!GetStop(mi.PCK_ZR)) return false;
                      

                        //토크에러 체크안함 옵션에 토크에러 발생 시                        
                        if(SM.MT.GetAlarmSgnl((int)mi.PCK_TR))
                        {
                            if (!OM.CmnOptn.bTorqChck)//토크 체크 모드가 아닐때 토크에러시 끌러서 렌즈트레이에 렌즈를 다시 담음.
                            {
                                if (iRptCnt < OM.DevOptn.iWrkRptCnt) //토크 페일 리핏 카운트가 0보다 크면
                                {
                                    SM.MT.Stop((int)mi.PCK_ZR);
                                    if (SortInfo.eTray == ri.REAR)
                                    {
                                        DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);//홀더를 다시 트라이 하기 위해 언노운으로 마스킹
                                    }
                                    else
                                    {
                                        DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    }    
                                    iRptCnt++;//재시도 카운트 증가.
                                }
                                else
                                {
                                    SM.MT.Stop((int)mi.PCK_ZR);
                                    if (SortInfo.eTray == ri.REAR)
                                    {
                                        FindChip(ref c, ref r, cs.Visn, ri.REAR);
                                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.TorqueFail);
                                    }
                                    else
                                    {
                                        FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.TorqueFail);
                                    }
                                    iRptCnt = 0;//카운트 리셑
                                }

                                //역회전하여 렌즈를 풀른후 렌즈트레이에 담는 패턴.
                                Step.iCycle = 50;
                                return false;
                            }
                            //토크 체크 모드에서의 토크에러 발생 시 토크에러시 역회전 안하고 Z축만 올림.
                            else //if (OM.CmnOptn.bTorqChck) 
                            {
                                SM.MT.Stop((int)mi.PCK_ZR);
                                if (SortInfo.eTray == ri.REAR)
                                {
                                    FindChip(ref c, ref r, cs.Visn, ri.REAR);
                                    DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.TorqueFail);
                                }
                                else
                                {
                                    FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                                    DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.TorqueFail);
                                }
                            
                                //토크에러시에 그냥 렌즈 넵두고 올라오는 패턴.
                                Step.iCycle = 100;
                                return false;
                            }
                        }

                        //Z축 감지 센서 에러 발생 시
                        //if (!bRightTool && !SM.MT.GetAlarmSgnl((int)mi.PCK_TR))
                        //{
                        //    //dTemp1 = SM.MT.GetCmdPos((int)mi.PCK_TR);
                        //    Log.Trace("TR At Cmd", SM.MT.GetCmdPos((int)mi.PCK_TR).ToString());
                        //    iRptCnt++;
                        //    if (OM.DevOptn.iWrkRptCnt == 0 || OM.DevOptn.iWrkRptCnt == iRptCnt) //토크 페일 리핏 카운트가 0이면
                        //    {
                        //        if (SortInfo.eTray == ri.REAR)
                        //        {
                        //            FindChip(ref c, ref r, cs.Visn, ri.REAR);
                        //            DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.SensorFail);
                        //        }
                        //        else
                        //        {
                        //            FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                        //            DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.SensorFail);
                        //        }
                        //        iRptCnt = 0;
                        //    }
                        //
                        //    else if (OM.DevOptn.iWrkRptCnt > iRptCnt) //토크 페일 리핏 카운트가 0보다 크면
                        //    {
                        //        if (SortInfo.eTray == ri.REAR)
                        //        {
                        //            //FindChip(ref c, ref r, cs.Visn, ri.REAR);
                        //            //DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.Unkwn);
                        //            DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                        //        }
                        //        else
                        //        {
                        //            //FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                        //            //DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.Unkwn);
                        //            DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                        //        }
                        //    }
                        //
                        //    Step.iCycle = 50;
                        //    return false;
                        //}

                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                        SM.MT.SetServo((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true);
                        SM.MT.SetServo((int)mi.PCK_TR, true);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 19:
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;
                    if(SortInfo.bLeftWork)
                    {
                        SM.MT.GoIncRun((int)mi.PCK_TL, -OM.DevOptn.dThetaBackPos);
                    }
                    if(SortInfo.bRightWork)
                    {
                        SM.MT.GoIncRun((int)mi.PCK_TR, -OM.DevOptn.dThetaBackPos);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(10)) return false;
                    Step.iCycle++;
                    return false;

                case 22:
                    if(SortInfo.bLeftWork)
                    {
                        if (!GetStop(mi.PCK_TL, true)) return false;
                        dLtEndTime = DateTime.Now.ToOADate();
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true);
                    }
                    if(SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        dRtEndTime = DateTime.Now.ToOADate();
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                    }
                    m_tmDelay.Clear();
                    
                    Step.iCycle++;
                    return false;

                case 23:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;
                    if (SortInfo.bLeftWork)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    if (SortInfo.bRightWork)
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }

                    double dLtTime = 0;
                    double dRtTime = 0;

                    dLtTime   = dLtEndTime - dLtSttTime;
                    dRtTime   = dRtEndTime - dRtSttTime;
                    //dAssyTime = dLtTime + dRtTime;
                    //double dAssyTime = dRtEndTime + dLtSttTime;

                    dLtTotalTime += dLtTime;
                    dRtTotalTime += dRtTime;

                    if (SortInfo.bLeftWork)
                    {
                        dAssyTotalTime += dLtTime;
                    }
                    if (SortInfo.bRightWork)
                    {
                        dAssyTotalTime += dRtTime;
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 24:
                    if (!m_tmDelay.OnDelay(200)) return false;

                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL,"EncoderOffset", 4001);
                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR,"EncoderOffset", 4001);
                    }

                    Step.iCycle++;
                    return false;

                case 25:
                    
                    if (SortInfo.bLeftWork)
                    {
                        if(!GetStop(mi.PCK_ZL)) return false;
                        
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    if (SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        
                        SetY(yi.PCK_EjtRtOn, false);
                    
                    }
                    Step.iCycle++;
                    return false;

                case 26:
                    if (SortInfo.eTray == ri.REAR)
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.REAR);
                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.Work);
                        FindChip(ref c, ref r, cs.Align, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    else
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.Work);
                        FindChip(ref c, ref r, cs.Align, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    iRptCnt = 0;

                    Step.iCycle = 0;
                    return true;
                //==========================================================================
                //여기까지 정상종료.

                //위에서 씀  모터토그 에러시에 반대로 풀러서 렌즈트레이에 넣는 패턴.              
                case 50: //일단 토크 설정 최대치로 세팅 하고 다시 서보온.
                    if(SortInfo.bLeftWork)
                    {
                        //SM.IO.SetY((int)yi.PCK_VacLtOn, true);
                        DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Fail);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true );
                        SM.MT.SetReset   ((int)mi.PCK_TL, true );
                        SM.MT.SetServo   ((int)mi.PCK_TL, true );
                    }
                    if(SortInfo.bRightWork)
                    {
                        //SM.IO.SetY((int)yi.PCK_VacRtOn, true);
                        DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Fail);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true );
                        SM.MT.SetReset   ((int)mi.PCK_TR, true );
                        SM.MT.SetServo   ((int)mi.PCK_TR, true );
                    }
                    Step.iCycle++;
                    return false;

                case 51://Z축을 언락위치로 이동.
                    if(SortInfo.bLeftWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 16);
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLUnlock);
                    }

                    if (SortInfo.bRightWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 16);
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRUnlock);
                    }

                    Step.iCycle++;
                    return false;

                case 52://Z축 동작 멈춘거 확인 하고 계산하여 Z축 T축 동기구동하여 끌름.
                    //Device Option.
                    dTWork = PM.GetValue(mi.PCK_TL, pv.PCK_TLUnlockWork); //T1440 Z0.4
                    dHldrPitch = OM.DevOptn.dHldrPitch;
                    dThetaWorkSpeed = OM.DevOptn.dThetaWorkSpeed; //초당 2바퀴.
                    dThetaWorkAcc = OM.DevOptn.dThetaWorkAcc; //

                    //하드웨어 픽스된 변수들 하드웨어 변경시에 바꿔줘야함.
                    dHldrPitch_ZRotRatio = dHldrPitch;


                    dZMovePos = (dTWork / dTUnitPerRot) * dHldrPitch_ZRotRatio;
                    dZSpeed   = (dZMovePos / dTWork) * dThetaWorkSpeed;
                    dZAcc     = (dZMovePos / dTWork) * dThetaWorkAcc;

                    //FindChip(ref c, ref r, cs.Empty, ri.PICK);
                    if(SortInfo.bLeftWork)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TL);
                        SM.MT.GoInc((int)mi.PCK_TL, -dTWork, dThetaWorkSpeed, dThetaWorkAcc, dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZL, -dZMovePos, dZSpeed, dZAcc, dZAcc);
                    }
                    if (SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TR);
                        SM.MT.GoInc((int)mi.PCK_TR, -dTWork, dThetaWorkSpeed, dThetaWorkAcc, dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZR, -dZMovePos, dZSpeed, dZAcc, dZAcc);
                    }

                    Step.iCycle++;
                    return false;

                case 53://T축 동작 멈춤 확인 하고.
                    if (SortInfo.bLeftWork && GetStop(mi.PCK_TL, true)) return false;
                    if (SortInfo.bRightWork && GetStop(mi.PCK_TR, true)) return false;
                    Step.iCycle++;
                    return false;

                case 54://Z축 동작멈춤 확인 하고 Z축 대기위치로 올림.
                    if(SortInfo.bLeftWork)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        if (!GetStop(mi.PCK_TL, true)) return false;
                        
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    if(SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }
                  
                    Step.iCycle++;
                    return false;

                case 55://렌즈 트레이쪽으로 이동.
                    FindChip(ref c, ref r, cs.Empty, ri.LENS);//X는 오른쪽이홈 Y는 rear쪽이홈.
                    dMoveX = GetMotrPos(mi.PCK_X, pv.PCK_XVisnLensStt) - c * OM.DevInfo.dLensColPitch;
                    dMoveY = GetMotrPos(mi.STG_Y, pv.STG_YVisnLensStt) - r * OM.DevInfo.dLensRowPitch;
                    
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        dMoveX += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck1Ofs);
                        dMoveY += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck1Ofs);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        dMoveX += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck2Ofs);
                        dMoveY += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck2Ofs);
                    }

                    MoveMotr(mi.PCK_X, dMoveX);
                    MoveMotr(mi.STG_Y, dMoveY);

                    Step.iCycle++;
                    return false;

                case 56://XY 스탑 확인하고 Z축 픽위치로 이동.
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLPick);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRPick);
                    }

                    Step.iCycle++;
                    return false;

                case 57: //배큠 끄고.
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                    }                    
                    Step.iCycle++;
                    return false;

                case 58://이젝터 끄고.                    
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        SetY(yi.PCK_EjtRtOn, false);
                    }
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 59://딜레이 먹이고 Z축 올림.
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 60:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 0);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 0);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 61://데이터 마스킹 하고
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        FindChip(ref c, ref r, cs.Empty, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Fail);
                        FindChip(ref c, ref r, cs.Fail, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    else
                    {
                        FindChip(ref c, ref r, cs.Empty, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Fail);
                        FindChip(ref c, ref r, cs.Fail, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 62 ://엔코더 오프셑 4001을 왜 먹이지????
                    if (!m_tmDelay.OnDelay(200)) return false;
                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL,"EncoderOffset", 4001);
                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR,"EncoderOffset", 4001);
                    }
                    
                    Step.iCycle = 0;
                    return true;

                //위에서 씀
                //토크 체크 모드에서 설정 토크 이상 사용 시
                case 100:
                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax", OM.MstOptn.iMaxTorqueMax);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime", OM.MstOptn.iMaxTorqueTime);

                        SM.MT.SetServo((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true);
                        SM.MT.SetReset((int)mi.PCK_TL, true);
                        SM.MT.SetServo((int)mi.PCK_TL, true);

                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax", OM.MstOptn.iMaxTorqueMax);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime", OM.MstOptn.iMaxTorqueTime);

                        SM.MT.SetServo((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true);
                        SM.MT.SetReset((int)mi.PCK_TR, true);
                        SM.MT.SetServo((int)mi.PCK_TR, true);
                    }

                    Step.iCycle++;
                    return false;

                case 101:
                    if (SortInfo.bLeftWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 16);
                        SM.IO.SetY((int)yi.PCK_VacLtOn, false);
                        SM.IO.SetY((int)yi.PCK_EjtLtOn, true );
                    }

                    if (SortInfo.bRightWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 16);
                        SM.IO.SetY((int)yi.PCK_VacRtOn, false);
                        SM.IO.SetY((int)yi.PCK_EjtRtOn, true);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 102:
                    if (SortInfo.bLeftWork)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    if (SortInfo.bRightWork)
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }

                    Step.iCycle++;
                    return false;

                case 103:
                    if (SortInfo.bLeftWork)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        SM.IO.SetY((int)yi.PCK_EjtLtOn, false);
                    }
                    if (SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        SM.IO.SetY((int)yi.PCK_EjtRtOn, false);
                    }
                    Step.iCycle++;
                    return false;

                case 104:
                    if (SortInfo.bLeftWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 0);
                    }
                    if (SortInfo.bRightWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 0);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 105:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Empty);
                    }
                    else
                    {
                        DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Empty);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 106:
                    if (!m_tmDelay.OnDelay(100)) return false;

                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "EncoderOffset", 4001);
                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "EncoderOffset", 4001);
                    }

                    Step.iCycle = 0;
                    return true;
            }
        }
        //20181016
        //끼우는 렌즈방식용.
        double dBfTLeftPos  = 0.0; //렌즈 체결 전 T축 Left 포지션 저장
        double dBfTRightPos = 0.0; //렌즈 체결 전 T축 Right 포지션 저장

        public double dBfThetaEncPos = 0.0;
        public double dAtThetaEncPos = 0.0;
        public double dMovePos       = 0.0;
        public bool CyclePlace2()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Change Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;
            double dPckXPos = 0;
            double dPckYPos = 0;
            double dPckTPos = 0;

            //Device Option.
            double dTWork          ;
            double dHldrPitch      ; 
            double dThetaWorkSpeed ;
            double dThetaWorkAcc   ; 

            //하드웨어 픽스된 변수들 하드웨어 변경시에 바꿔줘야함.
            const double dTUnitPerRot = 360;//Degree
            const double dZUnitPerRot = 2;//mm
            const double dZ_TRotRatio = dZUnitPerRot / dTUnitPerRot;
            double dHldrPitch_ZRotRatio;


            double dZMovePos ;
            double dZSpeed  ;
            double dZAcc;

            //bool bLeftTool = false; //Sensor에 의해 멈췄는지 포지션으로 멈췄는지 확인 용
            //bool bRightTool = false;
            
            double dTemp;
            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Cycle Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:
                    
                    return false ;

                case 10: //소팅정보 확인 하고 Z축은 위로 올림.
                    SortInfo.bPick = false ;
                    SortInfo.eTool = ri.PICK ;
                    if(FindChip(ref c, ref r , cs.Visn,ri.REAR))SortInfo.eTray = ri.REAR ;
                    else                                        SortInfo.eTray = ri.FRNT ;
                    SortInfo.ePickChip = cs.Align ;
                    SortInfo.eTrayChip = cs.Visn  ;
                    if (!GetSortInfo(OM.CmnOptn.bUseMultiHldr, ref SortInfo))
                    {
                        Step.iCycle = 0 ;
                        return true ;
                    }
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);

                    Step.iCycle++;
                    return false;

                case 11://올라온거 확인 하고 혹시 세타 모터 알람 있는지 확인.
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;

                    if(SM.MT.GetAlarmSgnl((int)mi.PCK_TL) || SM.MT.GetAlarmSgnl((int)mi.PCK_TR))
                    {
                        SM.ER.SetErrMsg((int)ei.MTR_Alarm, "Theta Motor Alarm");
                        Step.iCycle=0;
                        return true ;
                    }

                    Step.iCycle++;
                    return false;

                case 12://비전값 보정하여 포지션에 넣고 이동.
                    if (SortInfo.eTray == ri.REAR)
                    {
                        dPckXPos = GetMotrPos(mi.PCK_X,pv.PCK_XVisnRearStt) ;
                        dPckYPos = GetMotrPos(mi.STG_Y,pv.STG_YVisnRearStt) ;
                        dPckXPos += SortInfo.iToolShift * OM.DevOptn.iPCKGapCnt * OM.DevInfo.dRearColPitch ;
                        dPckXPos -= SortInfo.iTrayC * OM.DevInfo.dRearColPitch;//X는 오른쪽이홈 Y는 앞쪽이홈.
                        dPckYPos -= SortInfo.iTrayR * OM.DevInfo.dRearRowPitch;
                    }
                    else
                    {
                        dPckXPos = GetMotrPos(mi.PCK_X,pv.PCK_XVisnFrntStt) ;
                        dPckYPos = GetMotrPos(mi.STG_Y,pv.STG_YVisnFrntStt) ;
                        dPckXPos += SortInfo.iToolShift * OM.DevOptn.iPCKGapCnt * OM.DevInfo.dFrntColPitch ;
                        dPckXPos -= SortInfo.iTrayC * OM.DevInfo.dFrntColPitch;//X는 오른쪽이홈 Y는 앞쪽이홈.
                        dPckYPos -= SortInfo.iTrayR * OM.DevInfo.dFrntRowPitch;
                    }
                    
                    FindChip(ref c,ref r,cs.Align,ri.PICK);

                    if(c==0){//픽커 1번 끼울때
                        RsltHldrL.dX = RsltHldrL.dX;
                        RsltHldrL.dY = RsltHldrL.dY;
                        dPckXPos -= DM.ARAY[(int)SortInfo.eTray].Chip[SortInfo.iTrayC , SortInfo.iTrayR].dDataX;
                        dPckYPos -= DM.ARAY[(int)SortInfo.eTray].Chip[SortInfo.iTrayC , SortInfo.iTrayR].dDataY;

                        dPckXPos += GetMotrPos(mi.PCK_X,pv.PCK_XVisnPck1Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y,pv.STG_YVisnPck1Ofs);

                        //렌즈 체결 전 집어넣는 위치
                        dPckTPos  = GetMotrPos(mi.PCK_TL, pv.PCK_TLVisnZero);
                        dPckTPos += GetMotrPos(mi.PCK_TL, pv.PCK_TLHolderPutOfs);
                        //dPckTPos += RsltHldrL.dT;
                        dPckTPos += DM.ARAY[(int)SortInfo.eTray].Chip[SortInfo.iTrayC, SortInfo.iTrayR].dDataT;

                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableLeftX(dPckTPos);
                            dPckYPos += LookUpTable.GetLookUpTableLeftY(dPckTPos);
                            dPckTPos += LookUpTable.GetLookUpTableLeftT(dPckTPos);
                        }
                        //Ver1010 끼우는방식. 나사체결방식은 비전결과 0으로 온다.
                        //일단 해보면서 하기위해 단순 오프셑 방식으로 넣어서 테스트.
                        //MoveMotr(mi.PCK_TL, GetMotrPos(mi.PCK_TL, pv.PCK_TLVisnZero)+ GetMotrPos(mi.PCK_TL, pv.PCK_TLHolderPutOfs) + RsltHldrL.dT);
                        MoveMotr(mi.PCK_TL, dPckTPos);
                    }
                    else {//픽커 2번 끼울때.
                        //dPckXPos -= RsltHldrR.dX;
                        //dPckYPos -= RsltHldrR.dY;
                        RsltHldrR.dX = RsltHldrR.dX;
                        RsltHldrR.dY = RsltHldrR.dY;

                        dPckXPos -= DM.ARAY[(int)SortInfo.eTray].Chip[SortInfo.iTrayC, SortInfo.iTrayR].dDataX;
                        dPckYPos -= DM.ARAY[(int)SortInfo.eTray].Chip[SortInfo.iTrayC, SortInfo.iTrayR].dDataY;
                        dPckXPos += GetMotrPos(mi.PCK_X,pv.PCK_XVisnPck2Ofs);
                        dPckYPos += GetMotrPos(mi.STG_Y,pv.STG_YVisnPck2Ofs);

                        //렌즈 체결 전 집어넣는 위치
                        dPckTPos  = GetMotrPos(mi.PCK_TR, pv.PCK_TRVisnZero);
                        dPckTPos += GetMotrPos(mi.PCK_TR, pv.PCK_TRHolderPutOfs);
                        dPckTPos += DM.ARAY[(int)SortInfo.eTray].Chip[SortInfo.iTrayC, SortInfo.iTrayR].dDataT;
                        //dPckTPos += RsltHldrR.dT;

                        if (OM.MstOptn.bUseEccntrCorr)
                        {
                            dPckXPos += LookUpTable.GetLookUpTableRightX(dPckTPos);
                            dPckYPos += LookUpTable.GetLookUpTableRightY(dPckTPos);
                            dPckTPos += LookUpTable.GetLookUpTableRightT(dPckTPos);
                        }
                        //Ver1010 끼우는방식.
                        MoveMotr(mi.PCK_TR, dPckTPos);
                    }

                    MoveMotr(mi.PCK_X , dPckXPos);
                    MoveMotr(mi.STG_Y , dPckYPos);

                    Step.iCycle++;
                    return false ;

                case 13: //정지 확인 하고 Z축 내려놓는다.
                    if(!GetStop(mi.PCK_X )) return false;
                    if(!GetStop(mi.STG_Y )) return false;
                    if(!GetStop(mi.PCK_TL)) return false;
                    if(!GetStop(mi.PCK_TR)) return false;

                    //1단계 위치 
                    dBfTLeftPos  = SM.MT.GetCmdPos((int)mi.PCK_TL);//렌즈 체결 전 위치 저장
                    dBfTRightPos = SM.MT.GetCmdPos((int)mi.PCK_TR);//렌즈 체결 전 위치 저장



                    if (SortInfo.bLeftWork)
                    {
                        if(SortInfo.eTray == ri.FRNT) DM.ARAY[(int)ri.FRNT].Chip[SortInfo.iTrayC,SortInfo.iTrayR].iTag = 0;
                        else                          DM.ARAY[(int)ri.REAR].Chip[SortInfo.iTrayC,SortInfo.iTrayR].iTag = 0; 
                        dBfThetaEncPos = SM.MT.GetEncPos((int)mi.PCK_TL);
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLPlce);
                    }
                    if (SortInfo.bRightWork)
                    {
                        if(SortInfo.eTray == ri.FRNT) DM.ARAY[(int)ri.FRNT].Chip[SortInfo.iTrayC,SortInfo.iTrayR].iTag = 1;
                        else                          DM.ARAY[(int)ri.REAR].Chip[SortInfo.iTrayC,SortInfo.iTrayR].iTag = 1; 

                        dBfThetaEncPos = SM.MT.GetEncPos((int)mi.PCK_TR);
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRPlce);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14: //Z축 멈춘거 확인 하고 
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;

                    //Z Axis Down Delay
                    if (!m_tmDelay.OnDelay(500)) return false;

                    //끼우는 모드에서 렌즈 높이센서를 들어갔는지 체크하는 것으로 씀.
                    if ((SortInfo.bLeftWork  && !GetX(xi.PCK_HghtLt)) ||
                        (SortInfo.bRightWork && !GetX(xi.PCK_HghtRt)))
                    {
                        //센서가 감지 안되면 들어갔음.     
                        //높이센서 안보면 30번 스텝으로
                        dBfTLeftPos  = SM.MT.GetCmdPos((int)mi.PCK_TL);//렌즈 체결 전 위치 저장
                        dBfTRightPos = SM.MT.GetCmdPos((int)mi.PCK_TR);//렌즈 체결 전 위치 저장
                        Step.iCycle = 30;
                        return false;
                    }

                    //끼우는 방식 안들어갔을때 좌우로 흔들어줌.                    
                    Step.iCycle++;
                    return false;

                //안들어갔을때 패턴.
                case 15: //정방향 0.2도 
                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.GoIncRun((int)mi.PCK_TL, 0.5);
                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.GoIncRun((int)mi.PCK_TR, 0.5);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    if (!GetStop(mi.PCK_TL)) return false;
                    if (!GetStop(mi.PCK_TR)) return false;

                    
                    if ((SortInfo.bLeftWork  && !GetX(xi.PCK_HghtLt)) ||
                        (SortInfo.bRightWork && !GetX(xi.PCK_HghtRt)))
                    {
                        //센서가 감지 안되면 들어갔음.     
                        //원위치 원복 패턴인데 어차피 들어간곳을 기준각도로 하기로 생각하여 안돌림.
                        //if (SortInfo.bLeftWork)
                        //{
                        //    SM.MT.GoIncRun((int)mi.PCK_TL, 0.1);
                        //}
                        //if (SortInfo.bRightWork)
                        //{
                        //    SM.MT.GoIncRun((int)mi.PCK_TR, 0.1);
                        //}

                        //높이센서 센서 오프 되면 들어간것으로 보고 30번 스텝으로
                        dBfTLeftPos  = SM.MT.GetCmdPos((int)mi.PCK_TL);//렌즈 체결 전 위치 저장
                        dBfTRightPos = SM.MT.GetCmdPos((int)mi.PCK_TR);//렌즈 체결 전 위치 저장
                        Step.iCycle = 30;
                        return false;
                    }

                    //안들어갔음 0.4 -방향으로
                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.GoIncRun((int)mi.PCK_TL, -1.0);
                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.GoIncRun((int)mi.PCK_TR, -1.0);
                    }
                    m_tmDelay.Clear();
                   
                    Step.iCycle++;
                    return false;

                case 17: //멈춘것 확인하고.
                    if (!m_tmDelay.OnDelay(1000)) return false;
                    if (!GetStop(mi.PCK_TL)) return false;
                    if (!GetStop(mi.PCK_TR)) return false;

                    if ((SortInfo.bLeftWork  && !GetX(xi.PCK_HghtLt)) ||
                        (SortInfo.bRightWork && !GetX(xi.PCK_HghtRt)))
                    {
                        //센서가 감지 안되면 들어갔음.     
                        //원위치 원복 패턴인데 어차피 들어간곳을 기준각도로 하기로 생각하여 안돌림.
                        //if (SortInfo.bLeftWork)
                        //{
                        //    SM.MT.GoIncRun((int)mi.PCK_TL, 0.1);
                        //}
                        //if (SortInfo.bRightWork)
                        //{
                        //    SM.MT.GoIncRun((int)mi.PCK_TR, 0.1);
                        //}

                        //높이센서 센서 오프 되면 들어간것으로 보고 30번 스텝으로
                        dBfTLeftPos  = SM.MT.GetCmdPos((int)mi.PCK_TL);//렌즈 체결 전 위치 저장
                        dBfTRightPos = SM.MT.GetCmdPos((int)mi.PCK_TR);//렌즈 체결 전 위치 저장
                        Step.iCycle = 30;
                        return false;
                    }

                    //최종 결론 안들어감...
                    Log.Trace(SortInfo.bLeftWork ? "Left ":"Right "+"Lens Putting Failed At Cmd", SM.MT.GetCmdPos((int)mi.PCK_TL).ToString());

                    if(iPlaceRetryCnt < OM.CmnOptn.iPlaceRetryCnt)
                    {
                        iPlaceRetryCnt++;
                        
                        //ToStop에서함.
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                        FindChip(ref c, ref r, cs.Visn, SortInfo.eTray);//리트라이 위해 그냥 마스킹 하지 않고 빠져나감.
                        //DM.ARAY[(int)SortInfo.eTray].SetStat(c, r, cs.Unkwn);//홀더를 다시 트라이 하기 위해 언노운으로 마스킹
                        
                    }
                    else
                    {
                        //SetErr(ei.PRT_LensPut , SortInfo.bLeftWork ? "Left " : "Right " + "Lens Putting Failed At Cmd");
                        Log.Trace("Failed", SortInfo.bLeftWork ? "Left " : "Right " + "Lens Putting Failed At Cmd");
                        iPlaceRetryCnt=0;

                        FindChip(ref c, ref r, cs.Visn, SortInfo.eTray);
                        DM.ARAY[(int)SortInfo.eTray].SetStat(c, r, cs.SensorFail);//홀더를 센서페일로 마스킹.
                        Step.iCycle = 50; //들어올려 트레이에 가져다놓는패턴.
                        return false;
                    }

                    Step.iCycle++;
                    return false;

                case 18:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    Step.iCycle=0;
                    return true;

                    //최종 인입 페일 결과.
                    //=============================================================================================================
                
                //위에서씀.
                case 30://정상적인 패턴임.

                    iPlaceRetryCnt=0;

                    if(SortInfo.bLeftWork)
                    {
                        Log.Trace("TL Bf Cmd", SM.MT.GetCmdPos((int)mi.PCK_TL).ToString());
                        if (!GetStop(mi.PCK_TL, true)) return false;
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax"      , OM.DevOptn.dTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.DevOptn.dTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime" , OM.DevOptn.dTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true );
                        SM.MT.SetServo   ((int)mi.PCK_TL, true );
                    }
                    if(SortInfo.bRightWork)
                    {
                        Log.Trace("TR Bf Cmd", SM.MT.GetCmdPos((int)mi.PCK_TR).ToString());
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.DevOptn.dTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.DevOptn.dTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.DevOptn.dTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true );
                        SM.MT.SetServo   ((int)mi.PCK_TR, true );
                    }
                    
                    Step.iCycle ++;
                    return false;

                case 31: //Z축과 T축의 포지션 계산하여 정방향 구동.
                    //Device Option.
                    dTWork          = PM.GetValue(mi.PCK_TL,pv.PCK_TLWorkOfs) ; //T1440 Z0.4
                    Log.Trace("dTWork", dTWork.ToString());
                    dHldrPitch      = OM.DevOptn.dHldrPitch      ; //끼우는방식은 노멀 0
                    dThetaWorkSpeed = OM.DevOptn.dThetaWorkSpeed ; //초당 2바퀴.
                    dThetaWorkAcc   = OM.DevOptn.dThetaWorkAcc   ; //

                    dHldrPitch_ZRotRatio = dHldrPitch ;//하우징 나사산 피치_회전 비
                    dZMovePos = (dTWork / dTUnitPerRot) * dHldrPitch_ZRotRatio;

                    dZSpeed = (dZMovePos / dTWork) * dThetaWorkSpeed ;
                    dZAcc   = (dZMovePos / dTWork) * dThetaWorkAcc   ;

                    if(SortInfo.bLeftWork)
                    {
                        dLtSttTime = DateTime.Now.ToOADate();
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TL);
                        SM.MT.GoInc((int)mi.PCK_TL,dTWork   ,dThetaWorkSpeed,dThetaWorkAcc,dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZL,dZMovePos,dZSpeed        ,dZAcc        ,dZAcc        );
                        double ddTemp = dTemp;
                    }
                    if(SortInfo.bRightWork)
                    {
                        dRtSttTime = DateTime.Now.ToOADate();
                        dTemp = SM.MT.GetCmdPos((int)mi.PCK_TR);
                        SM.MT.GoInc((int)mi.PCK_TR,dTWork   ,dThetaWorkSpeed,dThetaWorkAcc,dThetaWorkAcc);
                        SM.MT.GoInc((int)mi.PCK_ZR,dZMovePos,dZSpeed        ,dZAcc        ,dZAcc        );
                        double ddTemp = dTemp;
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 32://Z축 감지센서가 센싱 해제 되면 스탑 시킴.
                    if (!m_tmDelay.OnDelay(100)) return false;
                    //왼쪽툴 작업.
                    if (SortInfo.bLeftWork)
                    {
                        //T축 멈췄는지 확인 하고 바로 Z축 멈춤.
                        if (!GetStop(mi.PCK_TL, true)) return false;
                        else                           SM.MT.Stop((int)mi.PCK_ZL);                       
                        //Z축 동시에 멈췄는지 확인.
                        if (!GetStop(mi.PCK_ZL)) return false;

                        dAtThetaEncPos = SM.MT.GetEncPos((int)mi.PCK_TL);
                                              
                        if(SM.MT.GetAlarmSgnl((int)mi.PCK_TL))
                        {
                            //토크에러 체크안함 옵션에 토크에러 발생 시  
                            if (!OM.CmnOptn.bTorqChck)//토크 체크 모드가 아닐때 토크에러시 끌러서 렌즈트레이에 렌즈를 다시 담음.
                            {
                                if (iRptCnt < OM.DevOptn.iWrkRptCnt) //토크 페일 리핏 카운트가 0보다 크면
                                {
                                    SM.MT.Stop((int)mi.PCK_ZL);
                                    if (SortInfo.eTray == ri.REAR)
                                    {
                                        DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);//홀더를 다시 트라이 하기 위해 언노운으로 마스킹
                                    }
                                    else
                                    {
                                        DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    }    
                                    iRptCnt++;//재시도 카운트 증가.
                                }
                                else
                                {
                                    SM.MT.Stop((int)mi.PCK_ZL);
                                    if (SortInfo.eTray == ri.REAR)
                                    {
                                        FindChip(ref c, ref r, cs.Visn, ri.REAR);
                                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.TorqueFail);
                                    }
                                    else
                                    {
                                        FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.TorqueFail);
                                    }
                                    iRptCnt = 0;//카운트 리셑
                                }

                                //역회전하여 렌즈를 풀른후 렌즈트레이에 담는 패턴.
                                Step.iCycle = 50;
                                return false;
                            }
                            //토크 체크 모드에서의 토크에러 발생 시 토크에러시 역회전 안하고 Z축만 올림.
                            else //if (OM.CmnOptn.bTorqChck) 
                            {
                                SM.MT.Stop((int)mi.PCK_ZL);
                                if (SortInfo.eTray == ri.REAR)
                                {
                                    FindChip(ref c, ref r, cs.Visn, ri.REAR);
                                    DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.TorqueFail);
                                }
                                else
                                {
                                    FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                                    DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.TorqueFail);
                                }
                            
                                //토크에러시에 그냥 렌즈 넵두고 올라오는 패턴.
                                Step.iCycle = 100;
                                return false;
                            }

                        }  

                        //토크다시 맥스치로 세팅.
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true );
                        SM.MT.SetServo   ((int)mi.PCK_TL, true );
                    }

                    //오른쪽툴 작업.
                    if(SortInfo.bRightWork)
                    {
                        //T축 멈췄는지 확인 하고 바로 Z축 멈춤.
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        else                           SM.MT.Stop((int)mi.PCK_ZR);
                        //Z축 동시에 멈췄는지 확인.
                        if (!GetStop(mi.PCK_ZR)) return false;

                        dAtThetaEncPos = SM.MT.GetEncPos((int)mi.PCK_TR);

                        //토크에러 발생 시                        
                        if (SM.MT.GetAlarmSgnl((int)mi.PCK_TR))
                        {
                            if (!OM.CmnOptn.bTorqChck)//토크 체크 모드가 아닐때 토크에러시 끌러서 렌즈트레이에 렌즈를 다시 담음.
                            {
                                if (iRptCnt < OM.DevOptn.iWrkRptCnt) //토크 페일 리핏 카운트가 0보다 크면
                                {
                                    SM.MT.Stop((int)mi.PCK_ZR);
                                    if (SortInfo.eTray == ri.REAR)
                                    {
                                        DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);//홀더를 다시 트라이 하기 위해 언노운으로 마스킹
                                    }
                                    else
                                    {
                                        DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                                    }    
                                    iRptCnt++;//재시도 카운트 증가.
                                }
                                else
                                {
                                    SM.MT.Stop((int)mi.PCK_ZR);
                                    if (SortInfo.eTray == ri.REAR)
                                    {
                                        FindChip(ref c, ref r, cs.Visn, ri.REAR);
                                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.TorqueFail);
                                    }
                                    else
                                    {
                                        FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.TorqueFail);
                                    }
                                    iRptCnt = 0;//카운트 리셑
                                }

                                //역회전하여 렌즈를 풀른후 렌즈트레이에 담는 패턴.
                                Step.iCycle = 50;
                                return false;
                            }
                            //토크 체크 모드에서의 토크에러 발생 시 토크에러시 역회전 안하고 Z축만 올림.
                            else //if (OM.CmnOptn.bTorqChck) 
                            {
                                SM.MT.Stop((int)mi.PCK_ZR);
                                if (SortInfo.eTray == ri.REAR)
                                {
                                    FindChip(ref c, ref r, cs.Visn, ri.REAR);
                                    DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.TorqueFail);
                                }
                                else
                                {
                                    FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                                    DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.TorqueFail);
                                }
                            
                                //토크에러시에 그냥 렌즈 넵두고 올라오는 패턴.
                                Step.iCycle = 100;
                                return false;
                            }
                        }

                        //토크 다시 맥스치로 설정.
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true );
                        SM.MT.SetServo   ((int)mi.PCK_TR, true );
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 33:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    if(SortInfo.bLeftWork)
                    {
                        SM.MT.GoIncRun((int)mi.PCK_TL, -OM.DevOptn.dThetaBackPos);
                    }
                    if(SortInfo.bRightWork)
                    {
                        SM.MT.GoIncRun((int)mi.PCK_TR, -OM.DevOptn.dThetaBackPos);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 34:
                    if (!m_tmDelay.OnDelay(50)) return false;
                    Step.iCycle++;
                    return false;

                case 35:
                    if(SortInfo.bLeftWork)
                    {
                        if (!GetStop(mi.PCK_TL, true)) return false;
                        dLtEndTime = DateTime.Now.ToOADate();
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true);
                    }
                    if(SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        dRtEndTime = DateTime.Now.ToOADate();
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                    }
                    m_tmDelay.Clear();
                    
                    Step.iCycle++;
                    return false;

                case 36:
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;
                    if (SortInfo.bLeftWork)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                        SetY(yi.PCK_EjtLtOn, false);
                        
                    }
                    if (SortInfo.bRightWork)
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                        SetY(yi.PCK_EjtRtOn, false);
                    }

                    double dLtTime = 0;
                    double dRtTime = 0;

                    dLtTime   = dLtEndTime - dLtSttTime;
                    dRtTime   = dRtEndTime - dRtSttTime;

                    dLtTotalTime += dLtTime;
                    dRtTotalTime += dRtTime;

                    if (SortInfo.bLeftWork)
                    {
                        dAssyTotalTime += dLtTime;
                    }
                    if (SortInfo.bRightWork)
                    {
                        dAssyTotalTime += dRtTime;
                    }                    

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;
                case 37:
                    if (!m_tmDelay.OnDelay(200)) return false;

                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL,"EncoderOffset", 4001);
                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR,"EncoderOffset", 4001);
                    }

                    Step.iCycle++;
                    return false;

                case 38:                    
                    if (SortInfo.bLeftWork)
                    {
                        if(!GetStop(mi.PCK_ZL)) return false;
                        
                        
                    }
                    if (SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        
                    
                    }
                    Step.iCycle++;
                    return false;

                //After Vision Inspection 사용하면 150번
                case 39:
                    Log.Trace("dBfThetaEncPos", dBfThetaEncPos.ToString());
                    Log.Trace("dAtThetaEncPos", dAtThetaEncPos.ToString());
                    dMovePos = dAtThetaEncPos - dBfThetaEncPos;
                    Log.Trace("dMovePos", dMovePos.ToString());
                    //if (SortInfo.eTray == ri.REAR)
                    //{
                    //    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    //    DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    //}
                    //else
                    //{
                    //    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    //    DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    //}

                    Step.iCycle++;
                    return false;

                case 40:
                    if (SortInfo.eTray == ri.REAR)
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.REAR);
                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.LastVisn);
                        
                        FindChip(ref c, ref r, cs.Align, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    else
                    {
                        FindChip(ref c, ref r, cs.Visn, ri.FRNT);
                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.LastVisn);

                        FindChip(ref c, ref r, cs.Align, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    iRptCnt = 0;
                    iPlaceVisnRetryCnt=0;
                    Step.iCycle = 0;
                    return true;
                    //=======================================================================================
                    //정상종료패턴끝

                //위에서 씀  모터토그 에러시에 반대로 풀러서 렌즈트레이에 넣는 패턴.              
                case 50: //일단 토크 설정 최대치로 세팅 하고 다시 서보온.
                    if(SortInfo.bLeftWork)
                    {
                        //SM.IO.SetY((int)yi.PCK_VacLtOn, true);
                        DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Fail);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                        //알람리셑.
                        SM.MT.SetServo   ((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true );
                        SM.MT.SetReset   ((int)mi.PCK_TL, true );
                        SM.MT.SetServo   ((int)mi.PCK_TL, true );
                    }
                    if(SortInfo.bRightWork)
                    {
                        //SM.IO.SetY((int)yi.PCK_VacRtOn, true);
                        DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Fail);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true );
                        SM.MT.SetReset   ((int)mi.PCK_TR, true );
                        SM.MT.SetServo   ((int)mi.PCK_TR, true );
                    }
                    Step.iCycle++;
                    return false;

                case 51://Z축을 언락위치로 이동.
                    //T축을 렌즈 체결 전 위치로 이동한다.
                    
                    if(SortInfo.bLeftWork)
                    {
                        MoveMotr(mi.PCK_TL, dBfTLeftPos);
                    }
                    
                    if (SortInfo.bRightWork)
                    {
                        MoveMotr(mi.PCK_TR, dBfTRightPos);
                    }

                    Step.iCycle++;
                    return false;

                case 52://Z축 동작 멈춘거 확인 하고 계산하여 Z축 T축 동기구동하여 끌름.
                    //Z축 올리고
                    if (SortInfo.bLeftWork)
                    {
                        if(!GetStop(mi.PCK_TL, true)) return false;
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }

                    if (SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_TR, true)) return false;
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }


                    Step.iCycle++;
                    return false;

                case 53://T축 동작 멈춤 확인 하고.
                    //배큠센서 확인해서 렌즈 빠져있으면 에러
                    
                    
                  
                    //FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    //DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);//20181212왜 여태 이걸 몰랐지?

                    //FindChip(ref c, ref r, cs.Align , ri.PICK);
                    //DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Fail);
                    

                    if (SortInfo.bLeftWork)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        if (!SM.IO.GetX((int)xi.PCK_VacLt))
                        {
                            SM.IO.SetY((int)yi.PCK_VacLtOn, false);
                            SM.ER.SetErr((int)ei.PRT_VaccSensor, "Left Picker does not have a lens.");
                            return true;
                        }
                    }

                    if (SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        if (!SM.IO.GetX((int)xi.PCK_VacRt))
                        {
                            SM.IO.SetY((int)yi.PCK_VacRtOn, false);
                            
                            SM.ER.SetErr((int)ei.PRT_VaccSensor, "Right Picker does not have a lens.");
                            return true;
                        }
                    }

                    Step.iCycle++;
                    return false;

                case 54://Z축 동작멈춤 확인 하고 Z축 대기위치로 올림.
                    
                  
                    Step.iCycle++;
                    return false;

                case 55://렌즈 트레이쪽으로 이동.
                    FindChip(ref c, ref r, cs.Empty, ri.LENS);//X는 오른쪽이홈 Y는 rear쪽이홈.
                    dMoveX = GetMotrPos(mi.PCK_X, pv.PCK_XVisnLensStt) - c * OM.DevInfo.dLensColPitch;
                    dMoveY = GetMotrPos(mi.STG_Y, pv.STG_YVisnLensStt) - r * OM.DevInfo.dLensRowPitch;
                    
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        dMoveX += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck1Ofs);
                        dMoveY += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck1Ofs);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        dMoveX += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck2Ofs);
                        dMoveY += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck2Ofs);
                    }

                    MoveMotr(mi.PCK_X, dMoveX);
                    MoveMotr(mi.STG_Y, dMoveY);

                    Step.iCycle++;
                    return false;

                case 56://XY 스탑 확인하고 Z축 픽위치로 이동.
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLPick);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRPick);
                    }

                    Step.iCycle++;
                    return false;

                case 57: //배큠 끄고.
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                    }                    
                    Step.iCycle++;
                    return false;

                case 58://이젝터 끄고.                    
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        SetY(yi.PCK_EjtRtOn, false);
                    }
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 59://딜레이 먹이고 Z축 올림.
                    if (!m_tmDelay.OnDelay(OM.DevOptn.iPlceDelay)) return false;
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 60:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 0);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 0);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 61://데이터 마스킹 하고
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Fail, ri.PICK);
                    if (c == 0)
                    {
                        FindChip(ref c, ref r, cs.Empty, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Fail);
                        FindChip(ref c, ref r, cs.Fail, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    else
                    {
                        FindChip(ref c, ref r, cs.Empty, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Fail);
                        FindChip(ref c, ref r, cs.Fail, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 62 ://엔코더 오프셑 4001을 왜 먹이지????
                    if (!m_tmDelay.OnDelay(200)) return false;
                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL,"EncoderOffset", 4001);
                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR,"EncoderOffset", 4001);
                    }
                    
                    Step.iCycle = 0;
                    return true;

                //위에서 씀
                //토크 체크 모드에서 설정 토크 이상 사용 시
                case 100:
                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax",       OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime",  OM.MstOptn.iMaxTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true);
                        SM.MT.SetReset   ((int)mi.PCK_TL, true);
                        SM.MT.SetServo   ((int)mi.PCK_TL, true);

                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.MstOptn.iMaxTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.MstOptn.iMaxTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true );
                        SM.MT.SetReset   ((int)mi.PCK_TR, true );
                        SM.MT.SetServo   ((int)mi.PCK_TR, true );
                    }

                    Step.iCycle++;
                    return false;

                case 101:
                    if (SortInfo.bLeftWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 16);
                        SM.IO.SetY((int)yi.PCK_VacLtOn, false);
                        SM.IO.SetY((int)yi.PCK_EjtLtOn, true );
                    }

                    if (SortInfo.bRightWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 16);
                        SM.IO.SetY((int)yi.PCK_VacRtOn, false);
                        SM.IO.SetY((int)yi.PCK_EjtRtOn, true);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 102:
                    if (SortInfo.bLeftWork)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    if (SortInfo.bRightWork)
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }

                    Step.iCycle++;
                    return false;

                case 103:
                    if (SortInfo.bLeftWork)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        SM.IO.SetY((int)yi.PCK_EjtLtOn, false);
                    }
                    if (SortInfo.bRightWork)
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        SM.IO.SetY((int)yi.PCK_EjtRtOn, false);
                    }
                    Step.iCycle++;
                    return false;

                case 104:
                    if (SortInfo.bLeftWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "InputEnable", 0);
                    }
                    if (SortInfo.bRightWork)
                    {
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "InputEnable", 0);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 105:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Empty);
                    }
                    else
                    {
                        DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Empty);
                    }

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 106:
                    if (!m_tmDelay.OnDelay(100)) return false;

                    if (SortInfo.bLeftWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "EncoderOffset", 4001);
                    }
                    if (SortInfo.bRightWork)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "EncoderOffset", 4001);
                    }

                    Step.iCycle = 0;
                    return true;
            }
        }


        public bool CycleAfterVisn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Change Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            switch (Step.iCycle)
            {

                default: 
                    sTemp = string.Format("Cycle Default Clear Cycle Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:                    
                    return false ;

                //렌즈 체결 후 비젼 검사 패턴
                case 10:
                    //SortInfo.bPick = false ;
                    //SortInfo.eTool = ri.PICK ;
                    //if(FindChip(ref c, ref r , cs.Visn,ri.REAR))SortInfo.eTray = ri.REAR ;
                    //else                                        SortInfo.eTray = ri.FRNT ;
                    //SortInfo.ePickChip = cs.Align ;
                    //SortInfo.eTrayChip = cs.Visn  ;
                    //if (!GetSortInfo(OM.CmnOptn.bUseMultiHldr, ref SortInfo))
                    //{
                    //    Step.iCycle = 0 ;
                    //    return true ;
                    //}

                    if(!OM.CmnOptn.bUseAtInsp)
                    {
                        DM.ARAY[(int)ri.FRNT].ChangeStat(cs.LastVisn , cs.Work);
                        DM.ARAY[(int)ri.REAR].ChangeStat(cs.LastVisn , cs.Work);
                        Step.iCycle = 0;
                        return true ;
                    }
                    
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);

                    VC.SendVisnMsg(VC.sm.Ready);

                    Step.iCycle++;
                    return false;

                case 11: //일단은 여기서 레디 확인 하나 나중에 디버깅 끝나면 옮기자 렉타임 없게.
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Not Ready");
                        Step.iCycle = 0;
                        return true;
                    }

                    Step.iCycle++;
                    return false;

                case 12:
                    if (FindChip(ref c, ref r, cs.LastVisn, ri.REAR))
                    {
                        MoveMotr(mi.PCK_X, GetMotrPos(mi.PCK_X, pv.PCK_XVisnRearStt) - c * OM.DevInfo.dRearColPitch);
                        MoveMotr(mi.STG_Y, GetMotrPos(mi.STG_Y, pv.STG_YVisnRearStt) - r * OM.DevInfo.dRearRowPitch);
                    }

                    else if(FindChip(ref c, ref r, cs.LastVisn, ri.FRNT))
                    {
                        MoveMotr(mi.PCK_X, GetMotrPos(mi.PCK_X, pv.PCK_XVisnFrntStt) - c * OM.DevInfo.dFrntColPitch);
                        MoveMotr(mi.STG_Y, GetMotrPos(mi.STG_Y, pv.STG_YVisnFrntStt) - r * OM.DevInfo.dFrntRowPitch);
                    }
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;
                    //나중에 통신 확인 여기.
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                    RsltLens.dX = 0.0;
                    RsltLens.dY = 0.0;
                    RsltLens.dT = 0.0;

                    VC.SendVisnMsg(VC.sm.Insp, "0");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }

                    if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLens.dX, ref RsltLens.dY, ref RsltLens.dT))
                    {
                        if (FindChip(ref c, ref r, cs.LastVisn, ri.REAR))
                        {
                            DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.LastVisnFail);
                        }

                        else if (FindChip(ref c, ref r, cs.LastVisn, ri.FRNT))
                        {
                            DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.LastVisnFail);
                        }
                            
                        //SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong or NG!");
                        Step.iCycle = 0;
                        return true;
                    }

                    Step.iCycle++;
                    return false;

                case 16:
                    double dBfWorkT = 0.0;
                    double dAtWorkT = 0.0;
                    double dWorkOfs = 0.0;


                    //if (FindChip(ref c, ref r, cs.AtVisn, ri.REAR))
                    //{
                    //    MoveMotr(mi.PCK_X, GetMotrPos(mi.PCK_X, pv.PCK_XVisnRearStt) - c * OM.DevInfo.dRearColPitch);
                    //    MoveMotr(mi.STG_Y, GetMotrPos(mi.STG_Y, pv.STG_YVisnRearStt) - r * OM.DevInfo.dRearRowPitch);
                    //}
                    //
                    //else if(FindChip(ref c, ref r, cs.AtVisn, ri.FRNT))
                    //{
                    //    MoveMotr(mi.PCK_X, GetMotrPos(mi.PCK_X, pv.PCK_XVisnFrntStt) - c * OM.DevInfo.dFrntColPitch);
                    //    MoveMotr(mi.STG_Y, GetMotrPos(mi.STG_Y, pv.STG_YVisnFrntStt) - r * OM.DevInfo.dFrntRowPitch);
                    //}
                    if (FindChip(ref c, ref r, cs.LastVisn, ri.REAR))
                    {
                        if (DM.ARAY[(int)ri.REAR].Chip[c, r].iTag == 0) //Tool ID (Left)
                        {
                            dWorkOfs = GetMotrPos(mi.PCK_TL, pv.PCK_TLWorkOfs);
                            //dBfWorkT = GetMotrPos(mi.PCK_TL, pv.PCK_TLHolderPutOfs) + RsltHldrL.dT;
                            dBfWorkT = GetMotrPos(mi.PCK_TL, pv.PCK_TLHolderPutOfs) + DM.ARAY[(int)ri.REAR].Chip[c, r].dDataT;//RsltHldrL.dT;
                            dAtWorkT = dBfWorkT + dWorkOfs;
                        }
                        else if(DM.ARAY[(int)ri.REAR].Chip[c, r].iTag == 1)//Tool ID (Right)
                        {
                            dWorkOfs = GetMotrPos(mi.PCK_TR, pv.PCK_TRWorkOfs);
                            //dBfWorkT = GetMotrPos(mi.PCK_TR, pv.PCK_TRHolderPutOfs) + RsltHldrR.dT;
                            dBfWorkT = GetMotrPos(mi.PCK_TR, pv.PCK_TRHolderPutOfs) + DM.ARAY[(int)ri.REAR].Chip[c, r].dDataT;
                            dAtWorkT = dBfWorkT + dWorkOfs;
                        }
                    }
                    else if (FindChip(ref c, ref r, cs.LastVisn, ri.FRNT))
                    {
                        if (DM.ARAY[(int)ri.FRNT].Chip[c, r].iTag == 0) //Tool ID (Left)
                        {   //10
                            dWorkOfs = GetMotrPos(mi.PCK_TL, pv.PCK_TLWorkOfs);//10
                            //75       //8.6                                          //67.184
                            dBfWorkT = GetMotrPos(mi.PCK_TL, pv.PCK_TLHolderPutOfs) + DM.ARAY[(int)ri.FRNT].Chip[c, r].dDataT;//RsltHldrL.dT;
                            dAtWorkT = dBfWorkT + dWorkOfs;
                        }
                        else if (DM.ARAY[(int)ri.FRNT].Chip[c, r].iTag == 1)//Tool ID (Right)
                        {
                            dWorkOfs = GetMotrPos(mi.PCK_TR, pv.PCK_TRWorkOfs);
                            dBfWorkT = GetMotrPos(mi.PCK_TR, pv.PCK_TRHolderPutOfs) + DM.ARAY[(int)ri.FRNT].Chip[c, r].dDataT;
                            dAtWorkT = dBfWorkT + dWorkOfs;
                        }
                    }
                    double dRsltLensT = RsltLens.dT;

                    Log.Trace("dAtWorkT", dAtWorkT.ToString());
                    Log.Trace("dRsltLensT", dRsltLensT.ToString());

                    if ((dAtWorkT - dRsltLensT) > OM.DevOptn.dAtVisnTTol || (dAtWorkT - dRsltLensT) < -OM.DevOptn.dAtVisnTTol)
                    {
                        if (FindChip(ref c, ref r, cs.LastVisn, ri.REAR))
                        {
                            DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.LastVisnFail);
                        }
                        else if(FindChip(ref c, ref r, cs.LastVisn, ri.FRNT))
                        {
                            DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.LastVisnFail);
                        }//
                        //SM.ER.SetErr((int)ei.VSN_InspRangeOver, "After Inspection Tolerance Over");
                        //SetErr(ei.VSN_InspRangeOver, "Vision T=" + (dAtWorkT - dRsltLensT) + " Range Over Spec(" + OM.DevOptn.dAtVisnTTol + ")");
                        Step.iCycle = 0;
                        return true;
                    }

                    

                    if (FindChip(ref c, ref r, cs.LastVisn, ri.REAR))
                    {
                        FindChip(ref c, ref r, cs.LastVisn, ri.REAR);
                        DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.Work);
                    }
                    else if(FindChip(ref c, ref r, cs.LastVisn, ri.FRNT))
                    {
                        FindChip(ref c, ref r, cs.LastVisn, ri.FRNT);
                        DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.Work);
                    }
                    iRptCnt = 0;

                    Step.iCycle = 0;
                    return true;
            }
        }


        public bool CycleOut()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }

            switch (Step.iCycle)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    MoveMotr(mi.STG_Y, pv.STG_YChange);

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!GetStop(mi.STG_Y)) return false;
                    //렌즈 트레이에 작업이 없어야 함.
                    if(DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) == 0)
                    {
                        SetY(yi.STG_VacLenOn, false);
                    }


                    bool bRearWork = DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Visn) == 0;
                    bool bFrntWork = DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Visn) == 0;

                    if (DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0 &&
                        DM.ARAY[(int)ri.PICK].CheckAllStat(cs.Empty) && (!bRearWork || !bFrntWork))
                    {
                        SEQ._bBtnStop = true;
                        //SetErr(ei.PKG_WorkEnd, "Lens Work End");
                        //DM.ARAY[(int)ri.LENS].SetStat(cs.None);
                    }

                    if ((DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty) != 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0) && bRearWork && OM.CmnOptn.bSkipFrnt)
                    {
                        SEQ._bBtnStop = true;
                        //SetErr(ei.PKG_WorkEnd, "Holder Work End");
                        //DM.ARAY[(int)ri.REAR].SetStat(cs.None);
                    }

                    if ((DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty) != 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) != 0) && bFrntWork && OM.CmnOptn.bSkipRear)
                    {
                        SEQ._bBtnStop = true;
                        //SetErr(ei.PKG_WorkEnd, "Holder Work End");
                        //DM.ARAY[(int)ri.FRNT].SetStat(cs.None);
                    }

                    if ((DM.ARAY[(int)ri.PICK].GetCntStat(cs.Empty) != 0 && DM.ARAY[(int)ri.PICK].GetCntStat(cs.Visn) == 0) && bFrntWork && bRearWork)
                    {
                        SEQ._bBtnStop = true;
                        //SetErr(ei.PKG_WorkEnd, "Holder Work End");
                        //DM.ARAY[(int)ri.FRNT].SetStat(cs.None);
                        //DM.ARAY[(int)ri.REAR].SetStat(cs.None);
                    }

                    if (DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) == 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Visn) == 0 && 
                        bFrntWork && bRearWork)
                    {
                        SEQ._bBtnStop = true;
                        //SetErr(ei.PKG_WorkEnd, "All Tray Work End");
                        //DM.ARAY[(int)ri.FRNT].SetStat(cs.None);
                        //DM.ARAY[(int)ri.REAR].SetStat(cs.None);
                        //DM.ARAY[(int)ri.LENS].SetStat(cs.None);
                    }

                    //SEQ._bBtnStop = true;
                    iRptCnt = 0;

                    Step.iCycle = 0;
                    return true;
            }
        }


        public bool CheckSafe(mi _iMotr, double _dPos)
        {
            bool bRet = true;
            if (_iMotr == mi.PCK_X || _iMotr == mi.STG_Y)
            {
                if (SM.MT.GetCmdPos((int)mi.PCK_ZL) > PM.GetValue(mi.PCK_ZL, pv.PCK_ZLWait))
                {
                    m_sCheckSafeMsg = "PCK_ZL is Lower than Wait Pos";
                    bRet = false;
                }
                if (SM.MT.GetCmdPos((int)mi.PCK_ZR) > PM.GetValue(mi.PCK_ZR, pv.PCK_ZRWait))
                {
                    m_sCheckSafeMsg = "PCK_ZR is Lower than Wait Pos";
                    bRet = false;
                }   
            }

            if(!bRet && !SEQ._bRun)
            {
                Log.ShowMessage("Error", m_sCheckSafeMsg);
                return bRet;
            }

            m_sCheckSafeMsg = "";
            return bRet;

        }

        public bool CheckSafeJog(mi _iMotr, bool _bPos)
        {
            bool bRet = true;
            if (_iMotr == mi.PCK_X || _iMotr == mi.STG_Y)
            {
                if (SM.MT.GetCmdPos((int)mi.PCK_ZL) > PM.GetValue(mi.PCK_ZL, pv.PCK_ZLWait))
                {
                    m_sCheckSafeMsg = "PCK_ZL is Lower than Wait Pos";
                    bRet = false;
                }
                if (SM.MT.GetCmdPos((int)mi.PCK_ZR) > PM.GetValue(mi.PCK_ZR, pv.PCK_ZRWait))
                {
                    m_sCheckSafeMsg = "PCK_ZR is Lower than Wait Pos";
                    bRet = false;
                }   
            }

            if(!bRet && !SEQ._bRun)
            {
                Log.ShowMessage("Error", m_sCheckSafeMsg);
                return bRet;
            }

            m_sCheckSafeMsg = "";
            return bRet;

        }

        public bool CheckSafe(ai _iActr, EN_CYLINDER_POS _bFwd)
        {
            return true;
        }

        public bool MoveMotr(mi _iMotr , double _dPos , bool _bSlow = false)
        {
            if(!CheckSafe(_iMotr, _dPos))return false ;

            if(_bSlow) {SM.MT.GoAbsSlow((int)_iMotr, _dPos); return false;}
            else { 
                if(Step.iCycle != 0) { SM.MT.GoAbsRun((int)_iMotr, _dPos); }
                else                 { SM.MT.GoAbsMan((int)_iMotr, _dPos); }
            }

            return GetStop(_iMotr)  ;         
        }

        public bool MoveMotr(mi _iMotr , pv _iPosId , bool _bSlow=false)
        {
            return MoveMotr(_iMotr , GetMotrPos(_iMotr,_iPosId) , _bSlow);      
        }

        public void MoveMotr3Axis(mi _iMotrX , mi _iMotrY , mi _iMotrZ , double _dPosX , double _dPosY , double _dPosZ , double _dVel)
        {
            int    [] Motrs = new int[3];
            double [] Poses = new double[3];

            //순서가 모션보드 오름 차순.
            Motrs[0] = (int)_iMotrY ;
            Motrs[1] = (int)_iMotrX ;
            Motrs[2] = (int)_iMotrZ ;

            Poses[0] = _dPosY ;
            Poses[1] = _dPosX ;
            Poses[2] = _dPosZ ;

            SM.MT.GoMultiAbs(Motrs , Poses ,_dVel);
        }

        public void SetY(yi _YAdd , bool _bVal)
        {
            SM.IO.SetY((int)_YAdd , _bVal);
        }

        public bool GetY(yi _YAdd)
        {
            return SM.IO.GetY((int)_YAdd);
        }

        public bool GetX(xi _XAdd)
        {
            return SM.IO.GetX((int)_XAdd);
        }
        

        public bool GetStop(mi _iMotr, bool _bNotUseEnc = false)
        {
            if (_bNotUseEnc) { return SM.MT.GetStop((int)_iMotr); }
            return SM.MT.GetStopInpos((int)_iMotr);

        }

        public void SetErr(ei _eErrNo , string _sMsg = "")
        {
            if(_sMsg == "") SM.ER.SetErr((int)_eErrNo);
            SM.ER.SetErrMsg((int)_eErrNo,_sMsg);
        }
            

        public bool MoveActr(ai _iActr, EN_CYLINDER_POS _bFwd)
        {
            if (!CheckSafe(_iActr, _bFwd)) return false;

            SM.CL.Move((int)_iActr, _bFwd);
            
            return true;
        }

        public bool CheckStop()
        {
            if (!SM.MT.GetStop((int)mi.STG_Y )) return false;
            if (!SM.MT.GetStop((int)mi.PCK_X )) return false;
            if (!SM.MT.GetStop((int)mi.PCK_ZL)) return false;
            if (!SM.MT.GetStop((int)mi.PCK_ZR)) return false;
            if (!SM.MT.GetStop((int)mi.PCK_TL)) return false;
            if (!SM.MT.GetStop((int)mi.PCK_TR)) return false;

            return true;
        }

        double dPckXLtPos = 0.0;
        double dPckXRtPos = 0.0;
        double dPckYLtPos = 0.0;
        double dPckYRtPos = 0.0;
        bool   bLeftCal = false;
        //double dVisnYLtPos = 0.0;
        //double dVisnYRtPos = 0.0;
        public bool CycleToolCalib()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.ETC_ManCycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }


            //dVisnYLtPos = PM.GetValue(mi.STG_Y, pv.STG_YAlgn) + PM.GetValue(mi.STG_Y, pv.STG_YVisnPck1Ofs);
            //dVisnYRtPos = PM.GetValue(mi.STG_Y, pv.STG_YAlgn) + PM.GetValue(mi.STG_Y, pv.STG_YVisnPck2Ofs);

            switch (Step.iCycle)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    //MoveMotr(mi.PCK_X, pv.PCK_XWait);
                    //MoveMotr(mi.STG_Y, pv.STG_YWait);
                    SM.MT.GoAbsRun((int)mi.PCK_TL, 0.0);
                    SM.MT.GoAbsRun((int)mi.PCK_TR, 0.0);
                    
                    Step.iCycle++;
                    return false;

                case 12:
                    //if (!GetStop(mi.PCK_X )) return false;
                    //if (!GetStop(mi.STG_Y )) return false;
                    if (!GetStop(mi.PCK_TL, true)) return false;
                    if (!GetStop(mi.PCK_TR, true)) return false;

                    if (OM.CmnOptn.bIgnrLeftPck)
                    {
                        bLeftCal = false;
                    }

                    else
                    {
                        bLeftCal = true;
                    }
                    
                    Step.iCycle++;
                    return false;

                //여기서 Left, Right 분기
                case 13:
                    //Calibration은 일단 Align 포지션을 가져다 쓴다.
                    dPckXLtPos = GetMotrPos(mi.PCK_X, pv.PCK_XAlgn) + GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck1Ofs);
                    dPckXRtPos = GetMotrPos(mi.PCK_X, pv.PCK_XAlgn) + GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck2Ofs);// -OM.DevOptn.iPCKGapCnt * OM.DevInfo.dLensColPitch;
                    dPckYLtPos = GetMotrPos(mi.STG_Y, pv.STG_YAlgn) + GetMotrPos(mi.STG_Y, pv.STG_YVisnPck1Ofs);
                    dPckYRtPos = GetMotrPos(mi.STG_Y, pv.STG_YAlgn) + GetMotrPos(mi.STG_Y, pv.STG_YVisnPck2Ofs);

                    if (bLeftCal)
                    {
                        MoveMotr(mi.PCK_X, dPckXLtPos,true);
                        MoveMotr(mi.STG_Y, dPckYLtPos,true);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_X, dPckXRtPos,true);
                        MoveMotr(mi.STG_Y, dPckYRtPos,true);
                    }
                    
                    Step.iCycle++;
                    return false;
                 
                case 14:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    if (bLeftCal) MoveMotr(mi.PCK_ZL, pv.PCK_ZLAlgnPlce , true);
                    else          MoveMotr(mi.PCK_ZR, pv.PCK_ZRAlgnPlce , true);
                    Step.iCycle++;
                    return false;
                    
                case 15:
                    if ( bLeftCal && !GetStop(mi.PCK_ZL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_ZR)) return false;
                    if (bLeftCal)
                    {
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true );
                    }
                    else {
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(true, 500)) return false;
                    if (bLeftCal)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait , true);
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait , true);
                        SetY(yi.PCK_EjtRtOn, false);
                    }
                    Step.iCycle++;
                    return false;

                case 17:
                    if ( bLeftCal && !GetStop(mi.PCK_ZL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_ZR)) return false;
                    
                    MoveMotr(mi.PCK_X, pv.PCK_XAlgn,true);
                    MoveMotr(mi.STG_Y, pv.STG_YAlgn,true);
                    
                    VC.SendVisnMsg(VC.sm.Ready);
                    Step.iCycle++;
                    return false;

                case 18: 
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Not Ready");
                        Step.iCycle = 0;
                        return true;
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 19:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                    VC.SendVisnMsg(VC.sm.Insp, "0");
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                        Step.iCycle = 0;
                        return true;
                    }

                    //Init RsltLens
                    //RsltLens.dX = 0.0;
                    //RsltLens.dY = 0.0;
                    //RsltLens.dT = 0.0;
                    RsltLensL.dX = 0.0;
                    RsltLensL.dY = 0.0;
                    RsltLensL.dT = 0.0;

                    RsltLensR.dX = 0.0;
                    RsltLensR.dY = 0.0;
                    RsltLensR.dT = 0.0;

                    //if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLens.dX, ref RsltLens.dY, ref RsltLens.dT))
                    //{
                    //    SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                    //    Step.iCycle = 0;
                    //    return true;
                    //}

                    if (bLeftCal)
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensL.dX, ref RsltLensL.dY, ref RsltLensL.dT))
                        {
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        PM.SetValue(mi.PCK_X , pv.PCK_XVisnPck1Ofs, GetMotrPos(mi.PCK_X , pv.PCK_XVisnPck1Ofs) + RsltLensL.dX);
                        PM.SetValue(mi.STG_Y , pv.STG_YVisnPck1Ofs, GetMotrPos(mi.STG_Y , pv.STG_YVisnPck1Ofs) + RsltLensL.dY);
                        PM.SetValue(mi.PCK_TL, pv.PCK_TLVisnZero  , -(RsltLensL.dT));
                    }
                    else 
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensR.dX, ref RsltLensR.dY, ref RsltLensR.dT))
                        {
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        PM.SetValue(mi.PCK_X , pv.PCK_XVisnPck2Ofs, GetMotrPos(mi.PCK_X , pv.PCK_XVisnPck2Ofs) + RsltLensR.dX);
                        PM.SetValue(mi.STG_Y , pv.STG_YVisnPck2Ofs, GetMotrPos(mi.STG_Y , pv.STG_YVisnPck2Ofs) + RsltLensR.dY);
                        PM.SetValue(mi.PCK_TR, pv.PCK_TRVisnZero  , -(RsltLensR.dT));
                    }
                    PM.Save(OM.GetCrntDev());

                    Step.iCycle++;
                    return false;

                case 21:
                    if (bLeftCal) 
                    {
                        MoveMotr(mi.PCK_X, dPckXLtPos,true);
                        MoveMotr(mi.STG_Y, dPckYLtPos,true);
                    }
                    else           
                    { 
                        MoveMotr(mi.PCK_X, dPckXRtPos,true);
                        MoveMotr(mi.STG_Y, dPckYRtPos,true);
                    }

                    Step.iCycle++;
                    return false;

                case 22:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    if (bLeftCal) MoveMotr(mi.PCK_ZL, pv.PCK_ZLAlgnPick,true);
                    else          MoveMotr(mi.PCK_ZR, pv.PCK_ZRAlgnPick,true);

                    Step.iCycle++;
                    return false;

                case 23:
                    if (bLeftCal && !GetStop(mi.PCK_ZL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_ZR)) return false;

                    if (bLeftCal)
                    {
                        SetY(yi.PCK_EjtLtOn, false);
                        SetY(yi.PCK_VacLtOn, true);
                    }
                    else
                    {
                        SetY(yi.PCK_EjtRtOn, false);
                        SetY(yi.PCK_VacRtOn, true);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 24:
                    if (!m_tmDelay.OnDelay(true, 500)) return false;
                    if (bLeftCal)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait,true);
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait,true);
                        SetY(yi.PCK_EjtRtOn, false);
                    }

                    Step.iCycle++;
                    return false;

                case 25:
                    if (bLeftCal && !GetStop(mi.PCK_ZL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_ZR)) return false;

                    if (bLeftCal)
                    {
                        if (OM.CmnOptn.bIgnrRightPck)
                        {
                            Step.iCycle = 0;
                            return true;
                        }

                        else
                        {
                            bLeftCal = false;
                            Step.iCycle = 13;
                            return false;
                        }
                    }
                   
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleHoldrCalib ()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;           

            switch (Step.iCycle)
            {
                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:                    
                    return false ;

                case 10: 
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 11:
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;
                    Step.iCycle++;
                    return false;

                case 12:                                        
                    Step.iCycle++;
                    return false;

                case 13: //일단은 여기서 레디 확인 하나 나중에 디버깅 끝나면 옮기자 렉타임 없게.                    
                    Step.iCycle++;
                    return false;

                case 14:       
                    if(OM.CmnOptn.bSkipRear)
                    {
                        MoveMotr(mi.PCK_X , pv.PCK_XVisnFrntStt,true);
                        MoveMotr(mi.STG_Y , pv.STG_YVisnFrntStt,true);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_X , pv.PCK_XVisnRearStt,true);
                        MoveMotr(mi.STG_Y , pv.STG_YVisnRearStt,true);
                    }
                                        
                    VC.SendVisnMsg(VC.sm.Ready); 
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;
                    //나중에 통신 확인 여기.

                    if (!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr,"Vision Not Ready");
                        Step.iCycle=0 ;
                        return true ;
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                    VC.SendVisnMsg(VC.sm.Insp,"0");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                    Step.iCycle++;
                    return false;

                case 17: //렌즈먼저 검사. 
                    if(!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }

                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr,"Vision Inspection Failed!");
                        Step.iCycle=0 ;
                        return true ;
                    }

                    if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLens.dX, ref RsltLens.dY, ref RsltLens.dT))
                    {
                        SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                        Step.iCycle = 0;
                        return true;
                    }
                    Step.iCycle++;
                    return false ;

                case 18:
                    VC.SendVisnMsg(VC.sm.Insp,"1");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                    Step.iCycle++;
                    return false;

                case 19:  //홀더 검사.
                    if(!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }

                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr,"Vision Inspection Failed!");
                        Step.iCycle=0 ;
                        return true ;
                    }

                    if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltHldr.dX, ref RsltHldr.dY, ref RsltHldr.dT))
                    {
                        SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                        Step.iCycle = 0;
                        return true;
                    }

                    //렌즈와 홀더간의 오프셑 계산하여 
                    //홀더에서 T값을 얻게되면 
                    PM.SetValue(mi.PCK_TL, pv.PCK_TLHolderPutOfs  , RsltLens.dT - RsltHldr.dT);
                    PM.SetValue(mi.PCK_TR, pv.PCK_TLHolderPutOfs  , RsltLens.dT - RsltHldr.dT);

                    //MoveMotr(mi.PCK_X , pv.PCK_XWait);
                    //MoveMotr(mi.STG_Y , pv.STG_YWait);
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;
                    PM.Save(OM.GetCrntDev());

                    Step.iCycle=0;
                    return true ;

            }
        }




        public bool bLeftTorque = false;
        public bool CycleTorqueCheck()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                //SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax", OM.MstOptn.iMaxTorqueMax);
                //SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                //SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime", OM.MstOptn.iMaxTorqueTime);
                //
                //SM.MT.SetServo((int)mi.PCK_TL, false);
                //SM.MT.SetHomeDone((int)mi.PCK_TL, true);
                ////SM.MT.SetReset   ((int)mi.PCK_TL, true );
                //SM.MT.SetServo((int)mi.PCK_TL, true);
                //
                //
                //SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax", OM.MstOptn.iMaxTorqueMax);
                //SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                //SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime", OM.MstOptn.iMaxTorqueTime);
                //
                //SM.MT.SetServo((int)mi.PCK_TR, false);
                //SM.MT.SetHomeDone((int)mi.PCK_TR, true);
                ////SM.MT.SetReset   ((int)mi.PCK_TR, true );
                //SM.MT.SetServo((int)mi.PCK_TR, true);

                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.ETC_ManCycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Cycle Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

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
                    


                    //Device Option.
                    double dTWork = PM.GetValue(mi.PCK_TL, pv.PCK_TLWorkOfs); //T1440 Z0.4
                    //double dHldrPitch = OM.DevOptn.dHldrPitch;
                    double dThetaWorkSpeed = OM.DevOptn.dThetaWorkSpeed; //초당 2바퀴.
                    double dThetaWorkAcc = OM.DevOptn.dThetaWorkAcc; //

                    //하드웨어 픽스된 변수들 하드웨어 변경시에 바꿔줘야함.

                    double dSttTime = CTimer.GetTime_us();
                    if (bLeftTorque)
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax"      , OM.DevOptn.dTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.DevOptn.dTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime" , OM.DevOptn.dTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true );
                        //SM.MT.SetReset   ((int)mi.PCK_TL, true );
                        SM.MT.SetServo   ((int)mi.PCK_TL, true );

                        SM.MT.GoInc((int)mi.PCK_TL, dTWork, dThetaWorkSpeed, dThetaWorkAcc, dThetaWorkAcc);
                    }
                    else
                    {
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax"      , OM.DevOptn.dTorqueMax  );
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.DevOptn.dTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime" , OM.DevOptn.dTorqueTime );

                        SM.MT.SetServo   ((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true );
                        //SM.MT.SetReset   ((int)mi.PCK_TR, true );
                        SM.MT.SetServo   ((int)mi.PCK_TR, true );

                        SM.MT.GoInc((int)mi.PCK_TR, dTWork, dThetaWorkSpeed, dThetaWorkAcc, dThetaWorkAcc);
                    }
                    double dEndTime = CTimer.GetTime_us();

                    Log.ShowMessage("Time", (dEndTime - dSttTime).ToString());

                    Step.iCycle++;
                    return false;

                case 14:
                    if (bLeftTorque)
                    {
                        if (!GetStop(mi.PCK_TL, true)) return false;

                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueMax", OM.MstOptn.iMaxTorqueMax);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TL, "TorqueLockTime", OM.MstOptn.iMaxTorqueTime);

                        SM.MT.SetServo((int)mi.PCK_TL, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TL, true);
                        //SM.MT.SetReset   ((int)mi.PCK_TL, true );
                        SM.MT.SetServo((int)mi.PCK_TL, true);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_TR, true)) return false;

                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueMax", OM.MstOptn.iMaxTorqueMax);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockLimit", OM.MstOptn.iMaxTorqueLimit);
                        SM.MT.SetDoublePara((int)mi.PCK_TR, "TorqueLockTime", OM.MstOptn.iMaxTorqueTime);

                        SM.MT.SetServo((int)mi.PCK_TR, false);
                        SM.MT.SetHomeDone((int)mi.PCK_TR, true);
                        //SM.MT.SetReset   ((int)mi.PCK_TR, true );
                        SM.MT.SetServo((int)mi.PCK_TR, true);
                    }

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool bconsecutively = false;        
        public bool CycleManInsp(int _iInspType)
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() &&!OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.ETC_ManCycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }

            int r = 0 ; 
            int c = 0 ;

            switch (Step.iCycle)
            {
                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case  0:
                    return false ;

                case 10:
                    if (bconsecutively)
                    {
                        if ((_iInspType == 0 && DM.ARAY[(int)ri.LENS].GetCntStat(cs.Unkwn) == 0) ||
                            (_iInspType == 1 && DM.ARAY[(int)ri.REAR].GetCntStat(cs.Unkwn) == 0) ||
                            (_iInspType == 2 && DM.ARAY[(int)ri.FRNT].GetCntStat(cs.Unkwn) == 0))
                        {
                                 if (_iInspType == 0) DM.ARAY[(int)ri.LENS].ChangeStat(cs.Visn, cs.Unkwn);
                            else if (_iInspType == 1) DM.ARAY[(int)ri.REAR].ChangeStat(cs.Visn, cs.Unkwn);
                            else                      DM.ARAY[(int)ri.FRNT].ChangeStat(cs.Visn, cs.Unkwn);
                            Step.iCycle = 0;
                            return true;
                        }
                    }
                    Step.iCycle++;
                    return false;

                case 11: 
                    MoveMotr(mi.PCK_ZL , pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR , pv.PCK_ZRWait);
                    MoveMotr(mi.PCK_TL , pv.PCK_TLWait);
                    MoveMotr(mi.PCK_TR , pv.PCK_TRWait);
                    Step.iCycle++;
                    return false;

                case 12:
                    if(!GetStop(mi.PCK_ZL)) return false;
                    if(!GetStop(mi.PCK_ZR)) return false;
                    if (!GetStop(mi.PCK_TL, true)) return false;
                    if(!GetStop(mi.PCK_TR, true)) return false;
                    Step.iCycle++;
                    return false;

                case 13: 
                    VC.SendVisnMsg(VC.sm.Ready);
                    
                    Step.iCycle++;
                    return false;

                case 14: //일단은 여기서 레디 확인 하나 나중에 디버깅 끝나면 옮기자 렉타임 없게.
                    if(!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr,"Vision Not Ready");
                        Step.iCycle=0 ;
                        return true ;
                    }
                    
                    Step.iCycle++;
                    return false;

                case 15:
                    if (_iInspType == 0)
                    {
                        FindChip(ref c,ref r,cs.Unkwn,ri.LENS);//X는 오른쪽이홈 Y는 rear쪽이홈.
                        MoveMotr(mi.PCK_X , GetMotrPos(mi.PCK_X,pv.PCK_XVisnLensStt) - c * OM.DevInfo.dLensColPitch);
                        MoveMotr(mi.STG_Y , GetMotrPos(mi.STG_Y,pv.STG_YVisnLensStt) - r * OM.DevInfo.dLensRowPitch);
                    }

                    else if (_iInspType == 1)
                    {
                        FindChip(ref c, ref r, cs.Unkwn, ri.REAR);//X는 오른쪽이홈 Y는 rear쪽이홈.
                        MoveMotr(mi.PCK_X, GetMotrPos(mi.PCK_X, pv.PCK_XVisnRearStt) - c * OM.DevInfo.dRearColPitch);
                        MoveMotr(mi.STG_Y, GetMotrPos(mi.STG_Y, pv.STG_YVisnRearStt) - r * OM.DevInfo.dRearRowPitch);
                    }
                    
                    else {
                        FindChip(ref c, ref r, cs.Unkwn, ri.FRNT);
                        MoveMotr(mi.PCK_X , GetMotrPos(mi.PCK_X, pv.PCK_XVisnFrntStt) - c * OM.DevInfo.dFrntColPitch);
                        MoveMotr(mi.STG_Y , GetMotrPos(mi.STG_Y, pv.STG_YVisnFrntStt) - r * OM.DevInfo.dFrntRowPitch);
                    }
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;
                    //나중에 통신 확인 여기.
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 17:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                    RsltLensL.dX = 0.0;
                    RsltLensL.dY = 0.0;
                    RsltLensL.dT = 0.0;

                         if (_iInspType == 0)                    VC.SendVisnMsg(VC.sm.Insp, "0");//0:렌즈검사 1:홀더검사. 2:홀더&렌즈유무검사.(1번은 렌즈있어도 OK 2번은 렌즈 있으면 NG)
                    else if (_iInspType == 1 || _iInspType == 2) VC.SendVisnMsg(VC.sm.Insp, "2");//VC.SendVisnMsg(VC.sm.Insp,"1");//
                    Step.iCycle++;
                    return false;

                case 18:
                    if (!VC.IsEndSendMsg()) return false ;


                    if(!VC.IsEndSendMsg()) return false ;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr,VC.GetVisnSendErrMsg());
                        Step.iCycle=0 ;
                        return true ;
                    }

                    if (_iInspType == 0)
                    {
                        FindChip(ref c, ref r, cs.Unkwn, ri.LENS);
                        if (VC.GetVisnSendMsg() == "NG")
                        {                        
                            DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Fail);
                        }
                        else DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Visn);
                    }
                    else if (_iInspType == 1)
                    {
                        FindChip(ref c, ref r, cs.Unkwn, ri.REAR);
                        if (VC.GetVisnSendMsg() == "NG")
                        {                        
                            DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.Fail);
                        }
                        else DM.ARAY[(int)ri.REAR].SetStat(c, r, cs.Visn);
                    }
                    else
                    {
                        FindChip(ref c, ref r, cs.Unkwn, ri.FRNT);
                        if (VC.GetVisnSendMsg() == "NG")
                        {                        
                            DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.Fail);
                        }
                        else DM.ARAY[(int)ri.FRNT].SetStat(c, r, cs.Visn);
                    }
                    if (bconsecutively)
                    {
                        Step.iCycle = 10;
                        return false;
                    }
                    else
                    {
                        Step.iCycle = 0;
                        return true;
                    }
            }
        }

        

        public bool CycleTrayIn()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.ETC_ManCycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }

            switch (Step.iCycle)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    MoveMotr(mi.STG_Y, pv.STG_YVisnLensStt);

                    Step.iCycle++;
                    return false;

                case 12:
                    if (!GetStop(mi.STG_Y)) return false;
                    SetY(yi.STG_VacLenOn, true);

                    if (OM.CmnOptn.bSkipFrnt)
                    {
                        DM.ARAY[(int)ri.FRNT].SetStat(cs.None);
                    }

                    if (OM.CmnOptn.bSkipRear)
                    {
                        DM.ARAY[(int)ri.REAR].SetStat(cs.None);
                    }

                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CyclePlaceGood()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.PRT_CycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iHome={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iHome = 0;
                //return true ;
            }

            int r = 0, c = 0;

            double dPckXPos = 0;
            double dPckYPos = 0;
            double dPckTPos = 0;

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    //if (!GetSortInfo(OM.DevOptn.bUseMultiHldr, ref SortInfo))
                    //{
                    //    Step.iCycle = 0;
                    //    return true;
                    //}


                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    //MoveMotr(mi.PCK_TL , pv.PCK_TLWait);
                    //MoveMotr(mi.PCK_TR , pv.PCK_TRWait);
                    //MoveMotr(mi.PCK_TL, pv.PCK_TLVisnZero);
                    //MoveMotr(mi.PCK_TR, pv.PCK_TRVisnZero);

                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    

                    Step.iCycle++;
                    return false;

                case 12:
                    FindChip(ref c, ref r, cs.Empty, ri.LENS);//X는 오른쪽이홈 Y는 rear쪽이홈.
                    dMoveX = GetMotrPos(mi.PCK_X, pv.PCK_XVisnLensStt) - c * OM.DevInfo.dLensColPitch;
                    dMoveY = GetMotrPos(mi.STG_Y, pv.STG_YVisnLensStt) - r * OM.DevInfo.dLensRowPitch;

                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        dMoveX += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck1Ofs);
                        dMoveY += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck1Ofs);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        dMoveX += GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck2Ofs);
                        dMoveY += GetMotrPos(mi.STG_Y, pv.STG_YVisnPck2Ofs);
                    }

                    MoveMotr(mi.PCK_X, dMoveX);
                    MoveMotr(mi.STG_Y, dMoveY);

                    Step.iCycle++;
                    return false;

                case 13:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLPick);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRPick);
                    }

                    Step.iCycle++;
                    return false;
                case 14:
                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true);
                        
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                        
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!m_tmDelay.OnDelay(100)) return false;
                    SetY(yi.PCK_EjtLtOn, false);
                    SetY(yi.PCK_EjtRtOn, false);

                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    }

                    Step.iCycle++;
                    return false;

                case 16:
                    FindChip(ref c, ref r, cs.Align, ri.PICK);
                    if (c == 0)
                    {
                        if (!GetStop(mi.PCK_ZL)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TL, "EncoderOffset", 4001);
                        FindChip(ref c, ref r, cs.Empty, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Unkwn);
                        FindChip(ref c, ref r, cs.Align, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    else
                    {
                        if (!GetStop(mi.PCK_ZR)) return false;
                        //SM.MT.SetDoublePara((int)mi.PCK_TR, "EncoderOffset", 4001);
                        FindChip(ref c, ref r, cs.Empty, ri.LENS);
                        DM.ARAY[(int)ri.LENS].SetStat(c, r, cs.Unkwn);
                        FindChip(ref c, ref r, cs.Align, ri.PICK);
                        DM.ARAY[(int)ri.PICK].SetStat(c, r, cs.Empty);
                    }
                    
                    

                    Step.iCycle = 0;
                    return true;
            }
        }

        public double dPckXLtSttPos = 0.0;
        public double dPckYLtSttPos = 0.0;
        public double dPckXRtSttPos = 0.0;
        public double dPckYRtSttPos = 0.0;

        //bool bLeftInsp = false;
        public int iEccentricInspCnt = 0;
        //double dVisnYLtPos = 0.0;
        //double dVisnYRtPos = 0.0;
        public bool CycleToolEccentric()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + sTemp;
                SetErr(ei.ETC_ManCycleTO, sTemp);
                Log.Trace(m_sPartName, sTemp);
                Step.iCycle = 0;
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("TIMEOUT Step.iCycle={0:00}", Step.iCycle);
                Log.Trace(m_sPartName, sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //Step.iCycle = 0;
                //return true ;
            }


            //dVisnYLtPos = PM.GetValue(mi.STG_Y, pv.STG_YAlgn) + PM.GetValue(mi.STG_Y, pv.STG_YVisnPck1Ofs);
            //dVisnYRtPos = PM.GetValue(mi.STG_Y, pv.STG_YAlgn) + PM.GetValue(mi.STG_Y, pv.STG_YVisnPck2Ofs);

            switch (Step.iCycle)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:000}", Step.iCycle);
                    Log.ShowMessage(m_sPartName, sTemp);
                    Step.iCycle = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait);
                    MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait);
                    Step.iCycle++;
                    return false;

                case 11:
                    if (!GetStop(mi.PCK_ZL)) return false;
                    if (!GetStop(mi.PCK_ZR)) return false;
                    //MoveMotr(mi.PCK_X, pv.PCK_XWait);
                    //MoveMotr(mi.STG_Y, pv.STG_YWait);
                    SM.MT.GoAbsRun((int)mi.PCK_TL, 0.0);
                    SM.MT.GoAbsRun((int)mi.PCK_TR, 0.0);
                    
                    Step.iCycle++;
                    return false;

                case 12:
                    //if (!GetStop(mi.PCK_X )) return false;
                    //if (!GetStop(mi.STG_Y )) return false;
                    if (!GetStop(mi.PCK_TL, true)) return false;
                    if (!GetStop(mi.PCK_TR, true)) return false;

                    if (OM.CmnOptn.bIgnrLeftPck)
                    {
                        bLeftCal = false;
                    }

                    else
                    {
                        bLeftCal = true;
                    }
                    
                    Step.iCycle++;
                    return false;

                //여기서 Left, Right 분기
                case 13:
                    iEccentricInspCnt = 0;

                    Step.iCycle++;
                    return false;

                case 14:
                    //Calibration은 일단 Align 포지션을 가져다 쓴다.
                    dPckXLtSttPos = GetMotrPos(mi.PCK_X, pv.PCK_XAlgn) + GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck1Ofs);
                    dPckXRtSttPos = GetMotrPos(mi.PCK_X, pv.PCK_XAlgn) + GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck2Ofs);// -OM.DevOptn.iPCKGapCnt * OM.DevInfo.dLensColPitch;
                    dPckYLtSttPos = GetMotrPos(mi.STG_Y, pv.STG_YAlgn) + GetMotrPos(mi.STG_Y, pv.STG_YVisnPck1Ofs);
                    dPckYRtSttPos = GetMotrPos(mi.STG_Y, pv.STG_YAlgn) + GetMotrPos(mi.STG_Y, pv.STG_YVisnPck2Ofs);

                    if (bLeftCal)
                    {
                        MoveMotr(mi.PCK_X, dPckXLtSttPos,true);
                        MoveMotr(mi.STG_Y, dPckYLtSttPos,true);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_X, dPckXRtSttPos,true);
                        MoveMotr(mi.STG_Y, dPckYRtSttPos,true);
                    }
                    
                    Step.iCycle++;
                    return false;
                 
                case 15:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    Step.iCycle = 20;
                    return false;

                //360도 다 돌때까지 반복
                case 20:
                    if (bLeftCal)
                    {
                        SM.MT.GoAbsRun((int)mi.PCK_TL, iEccentricInspCnt * (360 / OM.MAX_TABLE));
                    }
                    else
                    {
                        SM.MT.GoAbsRun((int)mi.PCK_TR, iEccentricInspCnt * (360 / OM.MAX_TABLE));
                    }

                    Step.iCycle++;
                    return false;

                case 21:
                    if ( bLeftCal && !GetStop(mi.PCK_TL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_TR)) return false;

                    if (bLeftCal) MoveMotr(mi.PCK_ZL, pv.PCK_ZLAlgnPlce , true);
                    else          MoveMotr(mi.PCK_ZR, pv.PCK_ZRAlgnPlce , true);
                    Step.iCycle++;
                    return false;
                    
                case 22:
                    if ( bLeftCal && !GetStop(mi.PCK_ZL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_ZR)) return false;
                    if (bLeftCal)
                    {
                        SetY(yi.PCK_VacLtOn, false);
                        SetY(yi.PCK_EjtLtOn, true );
                    }
                    else {
                        SetY(yi.PCK_VacRtOn, false);
                        SetY(yi.PCK_EjtRtOn, true);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 23:
                    if (!m_tmDelay.OnDelay(true, 500)) return false;
                    if (bLeftCal)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait , true);
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait , true);
                        SetY(yi.PCK_EjtRtOn, false);
                    }
                    Step.iCycle++;
                    return false;

                case 24:
                    if ( bLeftCal && !GetStop(mi.PCK_ZL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_ZR)) return false;
                    
                    MoveMotr(mi.PCK_X, pv.PCK_XAlgn,true);
                    MoveMotr(mi.STG_Y, pv.STG_YAlgn,true);
                    
                    VC.SendVisnMsg(VC.sm.Ready);
                    Step.iCycle++;
                    return false;

                case 25: 
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() != "OK")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Not Ready");
                        Step.iCycle = 0;
                        return true;
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 26:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVisnBfDelay)) return false;
                    VC.SendVisnMsg(VC.sm.Insp, "0");

                    

                    Step.iCycle++;
                    return false;

                case 27:
                    if (!VC.IsEndSendMsg()) return false;
                    if (VC.GetVisnRecvErrMsg() != "")
                    {
                        SetErr(ei.VSN_ComErr, VC.GetVisnSendErrMsg());
                        Step.iCycle = 0;
                        return true;
                    }
                    if (VC.GetVisnSendMsg() == "NG")
                    {
                        SetErr(ei.VSN_ComErr, "Vision Inspection Failed!");
                        Step.iCycle = 0;
                        return true;
                    }

                    //Init RsltLens
                    //RsltLens.dX = 0.0;
                    //RsltLens.dY = 0.0;
                    //RsltLens.dT = 0.0;
                    RsltLensL.dX = 0.0;
                    RsltLensL.dY = 0.0;
                    RsltLensL.dT = 0.0;

                    RsltLensR.dX = 0.0;
                    RsltLensR.dY = 0.0;
                    RsltLensR.dT = 0.0;

                    //if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLens.dX, ref RsltLens.dY, ref RsltLens.dT))
                    //{
                    //    SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                    //    Step.iCycle = 0;
                    //    return true;
                    //}

                    if (bLeftCal)
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensL.dX, ref RsltLensL.dY, ref RsltLensL.dT))
                        {
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }

                        OM.LeftTable[iEccentricInspCnt].dX =  RsltLensL.dX;
                        OM.LeftTable[iEccentricInspCnt].dY = -RsltLensL.dY;//-(204.5  + -224.7 - 17*20)
                        OM.LeftTable[iEccentricInspCnt].dT =  RsltLensL.dT + PM.GetValue(mi.PCK_TL, pv.PCK_TLVisnZero);//비전제로값을 상쇄.
                        OM.LeftTable[iEccentricInspCnt].dT =  OM.LeftTable[iEccentricInspCnt].dT - iEccentricInspCnt * (360 / OM.MAX_TABLE);//모터각도뺌.
                        OM.LeftTable[iEccentricInspCnt].dT =  OM.LeftTable[iEccentricInspCnt].dT < -180 ? OM.LeftTable[iEccentricInspCnt].dT + 360 : OM.LeftTable[iEccentricInspCnt].dT;//비전결과값이 기 비전
                        OM.LeftTable[iEccentricInspCnt].dT = -OM.LeftTable[iEccentricInspCnt].dT; //오프셑으로 변환.
                    }
                    else 
                    {
                        if (!GetVisnRslt(VC.GetVisnSendMsg(), ref RsltLensR.dX, ref RsltLensR.dY, ref RsltLensR.dT))
                        {
                            SetErr(ei.VSN_ComErr, "Vision Result Msg is Wrong!");
                            Step.iCycle = 0;
                            return true;
                        }
                        
                        OM.RightTable[iEccentricInspCnt].dX = RsltLensR.dX;
                        OM.RightTable[iEccentricInspCnt].dY =-RsltLensR.dY;
                        OM.RightTable[iEccentricInspCnt].dT = RsltLensR.dT + PM.GetValue(mi.PCK_TR, pv.PCK_TRVisnZero);//비전제로값을 상쇄.
                        OM.RightTable[iEccentricInspCnt].dT = OM.RightTable[iEccentricInspCnt].dT - iEccentricInspCnt * (360 / OM.MAX_TABLE);//모터각도뺌.
                        OM.RightTable[iEccentricInspCnt].dT = OM.RightTable[iEccentricInspCnt].dT < -180 ? OM.RightTable[iEccentricInspCnt].dT + 360 : OM.LeftTable[iEccentricInspCnt].dT;//비전결과값이 기 비전
                        OM.RightTable[iEccentricInspCnt].dT = -OM.RightTable[iEccentricInspCnt].dT; //오프셑으로 변환.
                    }

                    iEccentricInspCnt++;

                    Step.iCycle++;
                    return false;

                case 28:
                    //dPckXLtSttPos = GetMotrPos(mi.PCK_X, pv.PCK_XAlgn) + GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck1Ofs) + RsltLensL.dX;
                    //dPckXRtSttPos = GetMotrPos(mi.PCK_X, pv.PCK_XAlgn) + GetMotrPos(mi.PCK_X, pv.PCK_XVisnPck2Ofs) + RsltLensR.dX;// -OM.DevOptn.iPCKGapCnt * OM.DevInfo.dLensColPitch;
                    //dPckYLtSttPos = GetMotrPos(mi.STG_Y, pv.STG_YAlgn) + GetMotrPos(mi.STG_Y, pv.STG_YVisnPck1Ofs) + RsltLensL.dY;
                    //dPckYRtSttPos = GetMotrPos(mi.STG_Y, pv.STG_YAlgn) + GetMotrPos(mi.STG_Y, pv.STG_YVisnPck2Ofs) + RsltLensR.dY;

                    if (bLeftCal) 
                    {
                        MoveMotr(mi.PCK_X, dPckXLtSttPos, true);
                        MoveMotr(mi.STG_Y, dPckYLtSttPos, true);
                    }
                    else           
                    { 
                        MoveMotr(mi.PCK_X, dPckXRtSttPos, true);
                        MoveMotr(mi.STG_Y, dPckYRtSttPos, true);
                    }

                    Step.iCycle++;
                    return false;

                case 29:
                    if (!GetStop(mi.PCK_X)) return false;
                    if (!GetStop(mi.STG_Y)) return false;

                    if (bLeftCal) MoveMotr(mi.PCK_ZL, pv.PCK_ZLAlgnPick,true);
                    else          MoveMotr(mi.PCK_ZR, pv.PCK_ZRAlgnPick,true);

                    Step.iCycle++;
                    return false;

                case 30:
                    if (bLeftCal && !GetStop(mi.PCK_ZL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_ZR)) return false;

                    if (bLeftCal)
                    {
                        SetY(yi.PCK_EjtLtOn, false);
                        SetY(yi.PCK_VacLtOn, true);
                    }
                    else
                    {
                        SetY(yi.PCK_EjtRtOn, false);
                        SetY(yi.PCK_VacRtOn, true);
                    }
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 31:
                    if (!m_tmDelay.OnDelay(true, 500)) return false;
                    if (bLeftCal)
                    {
                        MoveMotr(mi.PCK_ZL, pv.PCK_ZLWait,true);
                        SetY(yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        MoveMotr(mi.PCK_ZR, pv.PCK_ZRWait,true);
                        SetY(yi.PCK_EjtRtOn, false);
                    }

                    Step.iCycle++;
                    return false;

                case 32:
                    if (bLeftCal && !GetStop(mi.PCK_ZL)) return false;
                    if (!bLeftCal && !GetStop(mi.PCK_ZR)) return false;

                    if(iEccentricInspCnt == OM.MAX_TABLE)
                    {
                        Step.iCycle = 40;
                        return false;
                    }

                    Step.iCycle = 20;
                    return false;

                case 40:
                    if (bLeftCal)
                    {
                        if (OM.CmnOptn.bIgnrRightPck)
                        {
                            Step.iCycle = 0;
                            return true;
                        }
                        else
                        {
                            bLeftCal = false;
                            Step.iCycle = 13;
                            return false;
                        }
                    }
                   
                    Step.iCycle = 0;
                    return true;
            }
        }

    };
}

