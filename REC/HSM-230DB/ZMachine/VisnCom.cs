using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using COMMON;
using SML2;

namespace Machine
{
    public class VisnCom
    {
        //Commend Id
        public enum ci{
            DIEALIGN = 0, 
            SUBSALIGN   , 
            RIGHTDIST   , 
            LEFTDIST    ,
            BTMALIGN    ,
            DISPENSE    ,
            MAX_COMMAND_ID
        }        

        public enum vs {
            Insp      = 0 ,
            Reset         ,
            Command       ,
            JobChange     ,
            MAX_VISN_SEND
        }
        public struct TPara
        {
            public string sVisnPcName     ; //파일저장시에 파일명에 삽입.
            public string sVisnFolder     ; //파일저장 하는 폴더.
            public xi     xVisn_Ready     ; 
            public xi     xVisn_Busy      ; 
            public xi     xVisn_InspOk    ; 
                          
            public yi     yVisn_InspStart ;
            public yi     yVisn_Reset     ;
            public yi     yVisn_Command   ;
            public yi     yVisn_JobChange ;
            public yi     yVisn_Live      ;
        }
        public struct TRslt {
            public double dMainX ;
            public double dMainY ;
            public double dSubX  ;
            public double dSubY  ;
        }
        struct TCycle{
            public int         iStep     ;
            public int         iPreStep  ;
            public CDelayTimer tmStep    ;
            public CDelayTimer tmDelay   ;
            public int         iResetCnt ;
        };

        TPara    Para ;
        TCycle[] VisnCycle = new TCycle [(int)vs.MAX_VISN_SEND];
        bool     InspOk;

        public void Init(TPara _tPara)
        {
            Para = _tPara ;

            //c:\\Control\\VisnRB_JobChange.dat
            string sPath = Para.sVisnFolder ;  //Dir value to save       
            DirectoryInfo di = new DirectoryInfo(sPath);    
            if (!di.Exists) 
            {  
                di.Create(); 
            }

            for(int i = 0 ; i < (int)vs.MAX_VISN_SEND ; i++)
            {
                VisnCycle[i].tmDelay = new CDelayTimer();
                VisnCycle[i].tmStep  = new CDelayTimer();
            }
        }

        public void Close()
        {
        }

        private bool SaveFile(string _sFileName , string _sData )
        {     
            string sPath = Para.sVisnFolder ;  //Dir value to save       
            string sFilePath = sPath + "\\" + Para.sVisnPcName + "_" + _sFileName ; 
            FileInfo fi = new FileInfo(sFilePath);
            if(fi.Exists)
            {
                fi.Delete();
            }
             
            FileStream   fs = fi.Create();
            StreamWriter sw = new StreamWriter(fs,System.Text.Encoding.Default); 
            sw.BaseStream.Seek(0, SeekOrigin.End); 
            //sw.WriteLine (_sData); 
            sw.Write(_sData); 
            sw.Flush(); 
            sw.Close(); 
            return true ;
        }

        private bool LoadFile(string _sFileName  , ref string _sData )
        {     
            string sPath = Para.sVisnFolder ;  //Dir value to save       
            string sFilePath = sPath + "\\" + Para.sVisnPcName + "_" + _sFileName ; 

            FileStream fs = new FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Read); 
            StreamReader sr = new StreamReader(fs,System.Text.Encoding.Default); 
            sr.BaseStream.Seek(0, SeekOrigin.Begin); 
            _sData = sr.ReadLine();
            //while (sr.Peek() > -1) { 
            //    sr.ReadLine();
            //} 
            sr.Close(); 

            return true ;
        }

        public bool SaveJobFile(string _sDevice)
        {     
            return SaveFile("JobChange.dat" , _sDevice );

        }
        public bool SaveLotNo(string _sLotNo)
        {
            return SaveFile("LotNo.dat" , _sLotNo );
        }
        public bool SaveCommand(string _sCommand)
        {
            return SaveFile("Command.dat" , _sCommand );
        }

        public bool LoadRsltDieAlign(ref string _sData)
        {
            bool bRet = LoadFile("DieAlign.dat"  , ref _sData );

            return bRet ;
        }
        public bool LoadRsltSubsAlign(ref string _sData)
        {
            bool bRet = LoadFile("SubsAlign.dat"  , ref _sData );

            return bRet ;
        }
        public bool LoadRsltBtmAlign(ref string _sData)
        {
            bool bRet = LoadFile("BtmAlign.dat"  , ref _sData );

            return bRet ;
        }
        public bool LoadRsltRightDist(ref string _sData)
        {
            bool bRet = LoadFile("RightDist.dat"  , ref _sData );

            return bRet ;
        }
        public bool LoadRsltLeftDist(ref string _sData)
        {
            bool bRet = LoadFile("LeftDist.dat"  , ref _sData );

            return bRet ;
        }

        private bool DeleteFile(string _sFileName)
        {
            string sPath = Para.sVisnFolder ;  //Dir value to save       
            string sFilePath = sPath + "\\" + Para.sVisnPcName + "_" + _sFileName ; 
            try{
                File.Delete(sFilePath);
            }
            catch{
                return false ;
            }
            return true ;
        }

        public bool DeleteRsltDieAlign()
        {
            return DeleteFile("DieAlign.dat");            
        }

        public bool DeleteRsltSubsAlign(ref string _sData)
        {
            return DeleteFile("SubsAlign.dat");   
        }
        public bool DeleteRsltBtmAlign(ref string _sData)
        {
            return DeleteFile("BtmAlign.dat");
        }
        public bool DeleteRsltRightDist(ref string _sData)
        {
            return DeleteFile("RightDist.dat");
        }
        public bool DeleteRsltLeftDist(ref string _sData)
        {
            return DeleteFile("LeftDist.dat");
        }

        static public bool GetRsltFromString(string _sRslt , out TRslt _tRslt)
        {
            _tRslt.dMainX = 0 ;
            _tRslt.dMainY = 0 ;
            _tRslt.dSubX  = 0 ;
            _tRslt.dSubY  = 0 ;

            if (_sRslt == null) return false;

            char sp = ',';
            string[] saSplitRslt = _sRslt.Split(sp);
            string sValue ;

            if(saSplitRslt.Length != 4) {
                Log.Trace("Err",_sRslt + " Data Cnt is not 4");
                return false ;
            }

            //MainX
            if(saSplitRslt[0].IndexOf("X")!=0){
                Log.Trace("Err",_sRslt + " doesn't have Data Main X");
                return false ;
            }
            sValue = saSplitRslt[0].Substring(1);
            _tRslt.dMainX = double.Parse(sValue);

            //MainY
            if(saSplitRslt[1].IndexOf("Y")!=0){
                Log.Trace("Err",_sRslt + " doesn't have Data Main Y");
                return false ;
            }
            sValue = saSplitRslt[1].Substring(1);
            _tRslt.dMainY = double.Parse(sValue);

            //SubX
            if(saSplitRslt[2].IndexOf("x")!=0){
                Log.Trace("Err",_sRslt + " doesn't have Data Sub x");
                return false ;
            }
            sValue = saSplitRslt[2].Substring(1);
            _tRslt.dSubX = double.Parse(sValue);

            //SubY
            if(saSplitRslt[3].IndexOf("y")!=0){
                Log.Trace("Err",_sRslt + " doesn't have Data Sub y");
                return false ;
            }
            sValue = saSplitRslt[3].Substring(1);
            _tRslt.dSubY = double.Parse(sValue);

            return true ;
        }

        //One Cycle.
        //여기부터.
        bool CycleInsp(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;

            if (_tCycle.tmDelay.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 5000))
            {
                SM.ER_SetNeedShowErr(true );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(Para.yVisn_InspStart, false);
                _tCycle.iStep = 0;
                return true;
            }

            if (_tCycle.iStep != _tCycle.iPreStep)
            {
                sTemp = string.Format("Step={0:00}", _tCycle.iStep);
                Log.Trace("Vision Communction", sTemp);
            }

            _tCycle.iPreStep = _tCycle.iStep;

            //Cycle.
            switch (_tCycle.iStep) {           
                default : 
                    sTemp = string.Format("DEFAILT END STATUS Step={0:00} , PreStep={0:00}", _tCycle.iStep , _tCycle.iPreStep); 
                    SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                    Log.Trace("Vision Communction", sTemp);           
                    SM.IO_SetY(Para.yVisn_InspStart , false) ;           
                    _tCycle.iStep = 0 ;
                    return true ;
           
                case 10: 
                    SM.IO_SetY(Para.yVisn_InspStart , false) ;
                    _tCycle.iStep++;
                    return false ;
           
                case 11: 
                    if(SM.IO_GetX(Para.xVisn_Busy)) return false ;
                    SM.IO_SetY(Para.yVisn_InspStart , true) ;
                    _tCycle.iStep++;
                    return false ;
           
                case 12: 
                    if(!SM.IO_GetX(Para.xVisn_Busy)) return false ;
                    SM.IO_SetY(Para.yVisn_InspStart , false ) ;
                    _tCycle.iStep++;
                    return false ;
           
                case 13: 
                    if(SM.IO_GetX(Para.xVisn_Busy)) return false ;
                    InspOk = SM.IO_GetX(xi.VISN_InspOk);
                    InspOk = SM.IO_GetX(Para.xVisn_InspOk);
                    _tCycle.iStep = 0 ;
                    return true;
            }
        }
        bool CycleReset(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;

            if (_tCycle.tmDelay.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 5000))
            {
                //SM.ER_SetNeedShowErr(true );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                //SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(Para.yVisn_Reset, false);
                _tCycle.iStep = 0;
                return true;
            }

            if (_tCycle.iStep != _tCycle.iPreStep)
            {
                sTemp = string.Format("Step={0:00}", _tCycle.iStep);
                Log.Trace("Vision Communction", sTemp);
            }

            _tCycle.iPreStep = _tCycle.iStep;

            //Cycle.
            switch (_tCycle.iStep) {
           
                default : 
                    sTemp = string.Format("DEFAILT END STATUS Step={0:00} , PreStep={0:00}", _tCycle.iStep , _tCycle.iPreStep); 
                    SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                    Log.Trace("Vision Communction", sTemp);           
                    SM.IO_SetY(Para.yVisn_Reset , false) ;           
                    _tCycle.iStep = 0 ;
                    return true ;

                case 10:  
                    SM.IO_SetY(Para.yVisn_Reset, false);
                    _tCycle.iResetCnt = 0;
                    _tCycle.iStep++;
                    return false;

                case 11:
                    SM.IO_SetY(Para.yVisn_Reset, true);
                    //Program.SendListMsg("Reset On");
                    _tCycle.tmDelay.Clear();
                    _tCycle.iStep++;
                    return false;
                    

                case 12:  
                    if(!SM.IO_GetX(Para.xVisn_Busy)) return false;
                    //Program.SendListMsg("Busy On");
                    SM.IO_SetY(Para.yVisn_Reset, false);
                    _tCycle.iStep++;
                    return false ;

                case 13:
                    //if (_tCycle.tmDelay.OnDelay(true, 3500))
                    //{
                    //    SM.IO_SetY(Para.yVisn_Reset, false);
                    //}
                    //if (_tCycle.tmDelay.OnDelay(true, 4000))
                    //{
                    //    sTemp = string.Format("VISION RESET RETRY Step={0:00}", _tCycle.iStep); 
                    //    Log.Trace("Vision Communction", sTemp);          
                    //    _tCycle.iStep = 11;
                    //    _tCycle.iResetCnt++;
                    //}

                    if (SM.IO_GetX(Para.xVisn_Busy)) return false;
                    //Program.SendListMsg("Busy Off");
                    //
                    _tCycle.iStep = 0;
                    return true;
            }
        }
        bool CycleCommand(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;

            if (_tCycle.tmDelay.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 14000))
            {
                SM.ER_SetNeedShowErr(true );
                if(_tCycle.iStep==13) sTemp = "13번스텝에서 커멘드파일을 비전에서 못읽은듯 합니다.";
                else                  sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(Para.yVisn_Command, false);
                _tCycle.iStep = 0;
                return true;
            }

            if (_tCycle.iStep != _tCycle.iPreStep)
            {
                sTemp = string.Format("Step={0:00}", _tCycle.iStep);
                Log.Trace("Vision Communction", sTemp);
            }

            _tCycle.iPreStep = _tCycle.iStep;

            //Cycle.
            switch (_tCycle.iStep) {
           
                default : 
                    sTemp = string.Format("DEFAILT END STATUS Step={0:00} , PreStep={0:00}", _tCycle.iStep , _tCycle.iPreStep); 
                    SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                    Log.Trace("Vision Communction", sTemp);           
                    SM.IO_SetY(Para.yVisn_Command , false) ;
                    _tCycle.iStep = 0;
                    return true;

                case 10:
                    SM.IO_SetY(Para.yVisn_Command, false);
                    _tCycle.iStep++;
                    return false;

                case 11: 
                    if (SM.IO_GetX(Para.xVisn_Busy)) return false;
                    SM.IO_SetY(Para.yVisn_Command, true);
                    //Program.SendListMsg("Command On");
                    _tCycle.iStep++;
                    return false;

                case 12: 
                    if (!SM.IO_GetX(Para.xVisn_Busy)) return false;
                    SM.IO_SetY(Para.yVisn_Command, false);
                    //Program.SendListMsg("Command Off");
                    _tCycle.iStep++;
                    return false;

                case 13: //타임아웃에서 씀
                    if (SM.IO_GetX(Para.xVisn_Busy)) return false;
                    //Program.SendListMsg("Busy Off");
                    _tCycle.iStep = 0;
                    return true;
            }

        }
        bool CycleJobChange(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;
            if (_tCycle.tmDelay.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 5000))
            {
                SM.ER_SetNeedShowErr(true );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(Para.yVisn_JobChange, false);
                _tCycle.iStep = 0;
                return true;
            }

            if (_tCycle.iStep != _tCycle.iPreStep)
            {
                sTemp = string.Format("Step={0:00}", _tCycle.iStep);
                Log.Trace("Vision Communction", sTemp);
            }

            _tCycle.iPreStep = _tCycle.iStep;

            //Cycle.
            switch (_tCycle.iStep) {
           
                default : 
                    sTemp = string.Format("DEFAILT END STATUS Step={0:00} , PreStep={0:00}", _tCycle.iStep , _tCycle.iPreStep); 
                    SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                    Log.Trace("Vision Communction", sTemp);           
                    SM.IO_SetY(Para.yVisn_JobChange , false) ;
                    _tCycle.iStep = 0;
                    return true;

                case 10:
                    SM.IO_SetY(Para.yVisn_JobChange, false);
                    _tCycle.iStep++;
                    return false;

                case 11: 
                    if (SM.IO_GetX(Para.xVisn_Busy)) return false;
                    SM.IO_SetY(Para.yVisn_JobChange, true);
                    _tCycle.iStep++;
                    return false;

                case 12: 
                    if (!SM.IO_GetX(Para.xVisn_Busy)) return false;
                    SM.IO_SetY(Para.yVisn_JobChange, false);
                    _tCycle.iStep++;
                    return false;

                case 13: 
                    if (SM.IO_GetX(Para.xVisn_Busy)) return false;
                    _tCycle.iStep = 0;
                    Log.ShowMessage("Visn Comm","JobChange End");
                    return true;
            }
        }

        public void Update()
        {
            if(VisnCycle[(int)vs.Insp     ].iStep != 0) CycleInsp     (ref VisnCycle[(int)vs.Insp     ]) ;
            if(VisnCycle[(int)vs.Reset    ].iStep != 0) CycleReset    (ref VisnCycle[(int)vs.Reset    ]) ;
            if(VisnCycle[(int)vs.Command  ].iStep != 0) CycleCommand  (ref VisnCycle[(int)vs.Command  ]) ;
            if(VisnCycle[(int)vs.JobChange].iStep != 0) CycleJobChange(ref VisnCycle[(int)vs.JobChange]) ;
        }

        public void Reset()
        {
            SM.IO_SetY(Para.yVisn_InspStart, false);
            SM.IO_SetY(Para.yVisn_Reset    , false);
            SM.IO_SetY(Para.yVisn_Command  , false);
            SM.IO_SetY(Para.yVisn_JobChange, false);
        }

        public bool SendInsp()
        {
            if (!SM.IO_GetX(Para.xVisn_Ready)) return false;
            VisnCycle[(int)vs.Insp].iStep = 10;
            return true;
        }
        public bool SendReset()
        {
            if (!SM.IO_GetX(Para.xVisn_Ready)) return false;
            VisnCycle[(int)vs.Reset].iStep = 10;
            return true;
        }
        public bool SendCommand(String _sCommand)
        {
            if (!SM.IO_GetX(Para.xVisn_Ready)) return false;
            SaveCommand(_sCommand);
            VisnCycle[(int)vs.Command].iStep = 10;
            return true;
        }
        public bool SendJobChange(String _sJobName)
        {
            //if (!SM.IO_GetX(Para.xVisn_Ready)) return false;
            SaveJobFile(_sJobName);
            VisnCycle[(int)vs.JobChange].iStep = 10;
            return true;
        }

        public bool GetSendCycleEnd(vs _eVisnSend)
        {
            return VisnCycle[(int)_eVisnSend].iStep == 0;
        }
     
        public bool GetInspOk()
        {
            return InspOk ;
        }



    }
}
