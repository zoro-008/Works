using COMMON;
using SML2;
using System;
using System.IO;
//
namespace Machine
{
    public class VisnCom
    {

        public enum vs {
            Insp      = 0 ,
            Reset         ,
            Command       ,
            JobChange     ,
            ManMode       ,
            LotStart      ,
            MAX_VISN_SEND
        };

        struct TCycle{
            public int         iStep     ;
            public int         iPreStep  ;
            public CDelayTimer tmStep    ;
            public CDelayTimer tmCycle   ;
            public CDelayTimer tmDelay   ;
            public int         iResetCnt ;
        }
        public struct TRslt {
            public int    MixDevice     ;
            public string UnitID        ;
            public string UnitDMC1      ;
            public string UnitDMC2      ;
            public int    GlobtopLeft   ;
            public int    GlobtopTop    ;
            public int    GlobtopRight  ;
            public int    GlobtopBottom ;
            public int    Empty         ;
            public int    MatchingError ;
            public int    UserDefine    ;
        }
        

        public struct TPara //if struct AXL Board are Starange.
        {
            public string sVisnPcName ; //파일저장시에 파일명에 삽입.
            public string sVisnFolder ; //파일저장 하는 폴더.
            //public xi xVisn_Ready     ;
            //public xi xVisn_Busy      ;
             
            //public yi yVisn_Command   ;
            //public yi yVisn_JobChange ;
            //public yi yVisn_Reset     ;
            //public yi yVisn_ManMode   ;
            //public yi yVisn_ManInsp   ;
        };

        TPara    Para ;
        TCycle[] VisnCycle = new TCycle [(int)vs.MAX_VISN_SEND];
        bool     InspOk;
        //bool     bManMode ;

        public void Init(ref TPara _tPara)
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
                VisnCycle[i].tmCycle = new CDelayTimer();
                VisnCycle[i].tmStep  = new CDelayTimer();
                VisnCycle[i].tmDelay = new CDelayTimer();
            }
        }

        public void Close()
        {
        }

        public void LoadRslt(int _iIdx , ref TRslt _Rslt)
        {
            string sVisnPath = "C:\\Data\\Visn.ini";
            CAutoIniFile.LoadStruct<TRslt>(sVisnPath, _iIdx.ToString(), ref _Rslt);                
        }

        public void LoadManRslt(ref TRslt _Rslt)
        {
            string sVisnManPath = "C:\\Data\\Visn.ini";

            CAutoIniFile.LoadStruct<TRslt>(sVisnManPath, "0", ref _Rslt);
        }

        private void SaveCommand(int _iIndx)
        {
            string sCommandPath = "C:\\Data\\VisnCommand.ini";
            CIniFile IniCommandSave = new CIniFile(sCommandPath);

            IniCommandSave.Save("Command", "Index", _iIndx);
        }

        private void SaveManMode(bool _bMode)
        {
            string sCommandPath = "C:\\Data\\VisnManMode.ini";
            CIniFile IniCommandSave = new CIniFile(sCommandPath);
            int iManMode = _bMode ? 1 : 0 ;
            IniCommandSave.Save("ManMode", "ManMode", iManMode);
        }

        private void SaveJobFile(string _sDevice)
        {
            string sCommandPath = "C:\\Data\\VisnJobChange.ini";
            CIniFile IniCommandSave = new CIniFile(sCommandPath);

            IniCommandSave.Save("JobChange", "Device", OM.GetCrntDev());
        }

        private void SaveLotStart(string _sLotNo)
        {
            string sLotDataPath = "C:\\Data\\LotName.ini";
            CIniFile IniLotDatadSave = new CIniFile(sLotDataPath);

            IniLotDatadSave.Save("LotName", "LotName", _sLotNo);
        }

        private bool DeleteFile(string _sFileName)
        {
            string sPath = Para.sVisnFolder ;  //Dir value to save       
            string sFilePath = sPath + "\\" + _sFileName ; 
            try{
                File.Delete(sFilePath);
            }
            catch{
                return false ;
            }
            return true ;
        }

        public bool DeleteRslt()
        {
            return DeleteFile("Visn.ini");
        }
        public bool DeleteManRslt()
        {
            return DeleteFile("VisnMan.ini");
        }
        public bool FindRsltFile()
        {
            string sPath = Para.sVisnFolder ;  //Dir value to save       
            string sFilePath = sPath + "\\Visn.ini" ; 
            return File.Exists(sFilePath);
        }
        //One Cycle.
        //여기부터.
        bool CycleInsp(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;
        
            if (_tCycle.tmCycle.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 8000))
            {
                SM.ER_SetNeedShowErr(false );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
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
                    _tCycle.iStep = 0 ;
                    return true ;
           
                case 10: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    //SML.MT.OneShotTrg((int)mi.TOOL_YTool,true,1000);
                    //SM.IO_SetY(Para.yVisn_ManInsp, true);
                    SM.IO_SetY(yi.VISN_ManInsp, true);
                    Log.Trace("Vision Shot", "ON");      
                    _tCycle.tmDelay.Clear();
                    _tCycle.iStep++;
                    return false ;
           
                case 11: 
                    //if(_tCycle.tmDelay.OnDelay(true, 10))return false;
                    if(!SM.IO_GetX(xi.VISN_Busy)) return false ;
                    //SM.IO_SetY(Para.yVisn_ManInsp, false);
                    SM.IO_SetY(yi.VISN_ManInsp, false);
                    Log.Trace("Vision Shot", "OFF"); 
                    _tCycle.tmDelay.Clear();
                    _tCycle.iStep++;
                    return false;

                case 12:
                    //if(_tCycle.tmDelay.OnDelay(1000))return false;
                    //SM.IO_SetY(Para.yVisn_ManInsp, false);
                    _tCycle.tmDelay.Clear();
                    _tCycle.iStep++;
                    return false ;
           
                case 13: 
                    //if(_tCycle.tmDelay.OnDelay(1000))return false;
                    //if(SM.IO_GetY(Para.yVisn_ManInsp))
                    //{
                        //SM.IO_SetY(yi.VISN_ManInsp, false);
                        Log.Trace("Vision Shot", "OFF2"); 
                        //return false;
                    //}
                    if(SM.IO_GetX(xi.VISN_Busy)) return false ;
                    Log.Trace("Vision Shot", "END");
                    _tCycle.iStep = 0 ;
                    return true;
            }
        }
        bool CycleReset(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;

            if (_tCycle.tmCycle.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 1000))
            {
                SM.ER_SetNeedShowErr(false );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(yi.VISN_Reset, false);
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
                    SM.IO_SetY(yi.VISN_Reset , false) ;           
                    _tCycle.iStep = 0 ;
                    return true ;

                case 10:  
                    SM.IO_SetY(yi.VISN_Reset, false);  
                    SM.IO_SetY(yi.VISN_ManInsp, false);  
                    _tCycle.iStep++;
                    return false;

                case 11:
                    SM.IO_SetY(yi.VISN_Reset, true);
                    _tCycle.tmDelay.Clear();
                    _tCycle.iStep++;
                    return false;

                case 12:  
                    if (_tCycle.tmDelay.OnDelay(true, 100))
                    {
                        SM.IO_SetY(yi.VISN_Reset, false);
                        _tCycle.iStep = 0 ;
                        return true ;
                    }
                    if (!SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_Reset, false);
                    _tCycle.tmDelay.Clear();
                    _tCycle.iStep++;
                    return false;

                case 13:
                    if (_tCycle.tmDelay.OnDelay(true, 100))
                    {
                        SM.IO_SetY(yi.VISN_Reset, false);
                        _tCycle.iStep = 0 ;
                        return true ;
                    }
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;

                    
                    _tCycle.iStep = 0;
                    return true;
            }
        }
        bool CycleCommand(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;

            if (_tCycle.tmCycle.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 8000))
            {
                SM.ER_SetNeedShowErr(false );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(yi.VISN_Command, false);
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
                    SM.IO_SetY(yi.VISN_Command , false) ;
                    _tCycle.iStep = 0;
                    return true;

                case 10:
                    SM.IO_SetY(yi.VISN_Command, false);
                    _tCycle.iStep++;
                    return false;

                case 11: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_Command, true);
                    _tCycle.iStep++;
                    return false;

                case 12: 
                    if (!SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_Command, false);
                    _tCycle.iStep++;
                    return false;

                case 13: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    _tCycle.iStep = 0;
                    return true;
            }
        }

        bool CycleManMode(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;

            if (_tCycle.tmCycle.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 8000))
            {
                SM.ER_SetNeedShowErr(false );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(yi.VISN_ManMode, false);
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
                    SM.IO_SetY(yi.VISN_ManMode , false) ;
                    _tCycle.iStep = 0;
                    return true;

                case 10:
                    SM.IO_SetY(yi.VISN_ManMode, false);
                    _tCycle.iStep++;
                    return false;

                case 11: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_ManMode, true);
                    _tCycle.iStep++;
                    return false;

                case 12: 
                    if (!SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_ManMode, false);
                    _tCycle.iStep++;
                    return false;

                case 13: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    _tCycle.iStep = 0;
                    return true;
            }
        }
        bool CycleJobChange(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;
            if (_tCycle.tmCycle.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 8000))
            {
                SM.ER_SetNeedShowErr(false );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(yi.VISN_Change, false);
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
                    SM.IO_SetY(yi.VISN_Change , false) ;
                    _tCycle.iStep = 0;
                    return true;

                case 10:
                    SM.IO_SetY(yi.VISN_Change, false);
                    _tCycle.iStep++;
                    return false;

                case 11: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_Change, true);
                    _tCycle.iStep++;
                    return false;

                case 12: 
                    if (!SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_Change, false);
                    _tCycle.iStep++;
                    return false;

                case 13: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    _tCycle.iStep = 0;
                    return true;
            }
        }

        bool CycleLotStart(ref TCycle _tCycle)
        {
            //Check Cycle Time Out.
            string sTemp;
            if (_tCycle.tmCycle.OnDelay(_tCycle.iStep == _tCycle.iPreStep && !OM.MstOptn.bDebugMode, 8000))
            {
                SM.ER_SetNeedShowErr(false );
                sTemp = string.Format("VisionTimeOut Step={0:00}", _tCycle.iStep);
                SM.ER_SetErr(ei.VSN_ComErr ,sTemp);
                SM.IO_SetY(yi.VISN_Change, false);
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
                    SM.IO_SetY(yi.VISN_Change , false) ;
                    _tCycle.iStep = 0;
                    return true;

                case 10:
                    SM.IO_SetY(yi.VISN_LotStart, false);
                    _tCycle.iStep++;
                    return false;

                case 11: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_LotStart, true);
                    _tCycle.iStep++;
                    return false;

                case 12: 
                    if (!SM.IO_GetX(xi.VISN_Busy)) return false;
                    SM.IO_SetY(yi.VISN_LotStart, false);
                    _tCycle.iStep++;
                    return false;

                case 13: 
                    if (SM.IO_GetX(xi.VISN_Busy)) return false;
                    _tCycle.iStep = 0;
                    return true;
            }
        }

        public void Update()
        {
            if(VisnCycle[(int)vs.Reset    ].iStep != 0) CycleReset    (ref VisnCycle[(int)vs.Reset    ]) ;
            if(VisnCycle[(int)vs.Command  ].iStep != 0) CycleCommand  (ref VisnCycle[(int)vs.Command  ]) ;
            if(VisnCycle[(int)vs.JobChange].iStep != 0) CycleJobChange(ref VisnCycle[(int)vs.JobChange]) ;
            if(VisnCycle[(int)vs.ManMode  ].iStep != 0) CycleManMode  (ref VisnCycle[(int)vs.ManMode  ]) ;
            if(VisnCycle[(int)vs.Insp     ].iStep != 0) CycleInsp     (ref VisnCycle[(int)vs.Insp     ]) ;
            if(VisnCycle[(int)vs.LotStart ].iStep != 0) CycleLotStart (ref VisnCycle[(int)vs.LotStart ]) ;
        }

        public void Reset()
        {
            SM.IO_SetY(yi.VISN_Command  , false);
            SM.IO_SetY(yi.VISN_Change   , false);
            SM.IO_SetY(yi.VISN_Reset    , false);
            SM.IO_SetY(yi.VISN_ManMode  , false);
            SM.IO_SetY(yi.VISN_ManInsp  , false);
            SM.IO_SetY(yi.VISN_LotStart , false);
        }

        public bool SendManInsp()
        {
            //if (!SM.IO_GetX(xi.VISN_Busy)) return false;
            VisnCycle[(int)vs.Insp].iStep = 10;
            return true;
        }
        public bool SendReset()
        {
            //if (!SM.IO_GetX(Para.xVisn_Ready)) return false;
            VisnCycle[(int)vs.Reset].iStep = 10;
            return true;
        }
        public bool SendIndex(int _iIndx)
        {
            SaveCommand(_iIndx);
            if (!SM.IO_GetX(xi.VISN_Ready))
            {
                SM.ER_SetErr(ei.VSN_ComErr, "Vision not ready");
                return false;
            }
                
            
            VisnCycle[(int)vs.Command].iStep = 10;
            return true;
        }
        public bool SendJobChange(String _sJobName)
        {
            SaveJobFile(_sJobName);
            if (!SM.IO_GetX(xi.VISN_Ready)) return false;
            
            VisnCycle[(int)vs.JobChange].iStep = 10;
            return true;
        }

        public bool SendManMode(bool _bManMode)
        {
            //if (!SM.IO_GetX(xi.VISN_Ready)) return false;
            //bManMode = _bManMode ;
            SaveManMode(_bManMode);
            VisnCycle[(int)vs.ManMode].iStep = 10;
            return true;
        }

        public bool SendLotStart(string _sLotNo)
        {
            //if (!SM.IO_GetX(xi.VISN_Ready)) return false;
            //bManMode = _bManMode ;
            SaveLotStart(_sLotNo);
            VisnCycle[(int)vs.LotStart].iStep = 10;
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
