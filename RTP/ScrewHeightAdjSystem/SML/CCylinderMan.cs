using System.Collections.Generic;
using COMMON;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace SML
{
    public class CCylinder
    {

        public CCylinder(){}

        //public const int MAX_X_CNT = 10;//실린더에 한쪽 센서가 여러게 달려 있는 경우.
        public struct TPara 
        {
            public string            sEnum       ;
            public string            sName       ;
            public string            sComment    ;
            public bool              bFwdXAnd    ;//센서 여러개 일때 센서들 엔드조건인지 오어 조건 인지.
            public bool              bBwdXAnd    ;
            public bool              bActrSync   ;
            public int               iFwdXAdd    ;
            public int               iBwdXAdd    ;
            public int               iFwdYAdd    ;
            public int               iBwdYAdd    ;
            public int               iFwdOnDelay ;
            public int               iBwdOnDelay ;
            public int               iFwdTimeOut ;
            public int               iBwdTimeOut ;
            public int               iActrSync   ;
            public int               iRptDelay   ;
            public EN_MOVE_DIRECTION eDirType    ; //EN_ACTR_DIRECTION
        };

        public struct TStat 
        {
            public EN_CYL_POS  eActPos ; //true == Fwd ;
            public EN_CYL_POS  eCmdPos ;
            public bool             bErr    ;
        };

        public TPara Para ;
        TStat Stat ;

        CDelayTimer TimeOutFwd=new CDelayTimer();
        CDelayTimer TimeOutBwd=new CDelayTimer();
        CDelayTimer DelayFwd  =new CDelayTimer();
        CDelayTimer DelayBwd  =new CDelayTimer();

        //EN_LAN_SEL  m_eLangSel       ; //Languge Selection.
        CDioMan     DIO              ;

        public bool GetFwdX()
        {
            if(Para.bFwdXAnd)//모든 센서가 불이 들어와야 하는 옵션.
            {
                if(Para.iFwdXAdd >= 0)
                {
                    if(!DIO.GetX(Para.iFwdXAdd)) return false ;
                }
                return true ;
            }
            else //여러개 센서중에 하나만 들어오면 되는 옵션.
            {
                if (Para.iFwdXAdd >= 0)
                {
                    if (DIO.GetX(Para.iFwdXAdd)) return true;
                }
                else if (Para.iFwdXAdd == -1)  return !GetBwdX();
                return false ;
            }
            
        }
        public bool GetBwdX()
        {
           if(Para.bBwdXAnd)//모든 센서가 불이 들어와야 하는 옵션.
            {
                if(Para.iBwdXAdd >= 0)
                {
                    if(!DIO.GetX(Para.iBwdXAdd)) return false ;
                }
                
                return true ;
            }
            else //여러개 센서중에 하나만 들어오면 되는 옵션.
            {
                if(Para.iBwdXAdd >= 0)
                {
                    if(DIO.GetX(Para.iBwdXAdd)) return true ;
                }
                else if (Para.iBwdXAdd == -1)   return true ;
                return false; 

            }
        }
 
        //public bool Init(EN_LAN_SEL _eLanSel,ref CDioMan _Dio)
        public bool Init(ref CDioMan _Dio)
        {
            //m_eLangSel = _eLanSel;

            DIO = _Dio ;

            //Para.iFwdXAdd = new int[MAX_X_CNT];
            //Para.iBwdXAdd = new int[MAX_X_CNT];

            Reset();

            //LoadSave(true);

            return true;
        }
        public bool Close()
        {
            return true;
        }

        public void Reset()
        {            
            TimeOutFwd.Clear();
            TimeOutBwd.Clear();
            
            //Set exist ID.
            bool bExistFwdX = (Para.iFwdXAdd >=  0) ;
            bool bExistBwdX = (Para.iBwdXAdd >=  0) ;
            bool bExistFwdY = (Para.iFwdYAdd >=  0) ;
            bool bExistBwdY = (Para.iBwdYAdd >=  0) ;
            
            bool bFwdX = false; 
            bool bBwdX = false; 
            bool bFwdY = false; 
            bool bBwdY = false; 
            
            bool bVtrlFwdX = false;
            bool bVtrlBwdX = false;
            //bool bVtrlFwdY = false;
            //bool bVtrlBwdY = false;
            
            //I/O.
            if (bExistFwdX) bFwdX = GetFwdX(); //IO.GetX(Para.iFwdXAdd,true); //Detect X/Y Val.
            if (bExistBwdX) bBwdX = GetBwdX(); //IO.GetX(Para.iBwdXAdd,true);
            if (bExistFwdY) bFwdY = DIO.GetY(Para.iFwdYAdd);
            if (bExistBwdY) bBwdY = DIO.GetY(Para.iBwdYAdd);
            
            //Virtual Input Sensor
            if      (bExistFwdX) bVtrlFwdX =  bFwdX ;
            else if (bExistBwdX) bVtrlFwdX = !bBwdX ;
            else if (bExistFwdY) bVtrlFwdX =  bFwdY ;
            else if (bExistBwdY) bVtrlFwdX = !bBwdY ;
            
            
            if      (bExistBwdX) bVtrlBwdX =  bBwdX ;
            else if (bExistFwdX) bVtrlBwdX = !bFwdX ;
            else if (bExistBwdY) bVtrlBwdX =  bBwdY ;
            else if (bExistFwdY) bVtrlBwdX = !bFwdY ;
            
            
            if(bVtrlFwdX) Stat.eCmdPos = EN_CYL_POS.Fwd ;
            if(bVtrlBwdX) Stat.eCmdPos = EN_CYL_POS.Bwd ;
            
        }

        public string GetName  ()
        {
            return Para.sName ;
        }
        public bool Complete (EN_CYL_POS _bCmd)
        {
            if(Stat.eCmdPos != Stat.eActPos) return false ;
            return Stat.eCmdPos == _bCmd ;
        }
        public bool Complete()
        {
            return Stat.eCmdPos == Stat.eActPos ;
        }
        public EN_CYL_POS GetCmd()
        {
            return Stat.eCmdPos ;
        }

        public EN_CYL_POS GetAct()
        {
            return Stat.eActPos ;
        }

        public bool Err()
        {
            return Stat.bErr ;
        }
        public bool Move(EN_CYL_POS _bCmd)
        {
            if(Stat.eCmdPos == _bCmd) Complete(_bCmd);

            Log.Trace(Para.sEnum + " Cyl Move",_bCmd == EN_CYL_POS.Fwd ? "Fwd" : "Bwd");

            //처음에 에러떠서...업데이트에서 하는걸로.
            //Stat.eCmdPos = _bCmd;

            if( _bCmd == EN_CYL_POS.Fwd) {
                if (Para.iFwdYAdd >= 0 && Para.iBwdYAdd >= 0)
                {
                    if (Para.iFwdYAdd >= 0) DIO.SetY(Para.iFwdYAdd, true);
                    if (Para.iBwdYAdd >= 0) DIO.SetY(Para.iBwdYAdd, false);
                }
                else
                {
                    if (Para.iFwdYAdd != -1) DIO.SetY(Para.iFwdYAdd, true);
                    if (Para.iBwdYAdd != -1) DIO.SetY(Para.iBwdYAdd, false);
                }
            }
            else {
                if (Para.iFwdYAdd >= 0 && Para.iBwdYAdd >= 0)
                {
                    if (Para.iFwdYAdd >= 0) DIO.SetY(Para.iFwdYAdd, false);
                    if (Para.iBwdYAdd >= 0) DIO.SetY(Para.iBwdYAdd, true);
                }
                else
                {
                    if (Para.iFwdYAdd != -1) DIO.SetY(Para.iFwdYAdd, false);
                    if (Para.iBwdYAdd != -1) DIO.SetY(Para.iBwdYAdd, true);
                }
            }

            return Complete(_bCmd);        
        }

        public void Update()
        {
            //Local Var.
            bool bExistFwdX;
            bool bExistBwdX;
            bool bExistFwdY;
            bool bExistBwdY;
            int  iFwdX;
            int  iBwdX;
            int  iFwdY;
            int  iBwdY;

            bool bFwdX = false;
            bool bBwdX = false;
            bool bFwdY = false;
            bool bBwdY = false;

            bool bVtrlFwdX;
            bool bVtrlBwdX;
            bool bVtrlFwdY;
            bool bVtrlBwdY;

            bool bDelayedFwd;
            bool bDelayedBwd;


            //Set I/O ID.
            iFwdX = Para.iFwdXAdd;
            iBwdX = Para.iBwdXAdd;
            iFwdY = Para.iFwdYAdd;
            iBwdY = Para.iBwdYAdd;

            //Check SKIP.
            if(Para.iFwdYAdd < 0 && Para.iBwdYAdd < 0)
            {
                Reset();
                return;
            }

            //Set exist ID.
            bExistFwdX = (iFwdX >=  0);
            bExistBwdX = (iBwdX >=  0);
            bExistFwdY = (iFwdY >=  0);
            bExistBwdY = (iBwdY >=  0);


            //I/O.
            if(bExistFwdX) bFwdX = GetFwdX();//IO.GetX(iFwdX); //Detect X/Y Val.
            if(bExistBwdX) bBwdX = GetBwdX();//IO.GetX(iBwdX);
            if(bExistFwdY) bFwdY = DIO.GetY(iFwdY);
            if(bExistBwdY) bBwdY = DIO.GetY(iBwdY);


            //Virtual Input Sensor
                 if(bExistFwdX) bVtrlFwdX =  bFwdX;
            else if(bExistBwdX) bVtrlFwdX = !bBwdX;
            else if(bExistFwdY) bVtrlFwdX =  bFwdY;
            else if(bExistBwdY) bVtrlFwdX = !bBwdY;
            else return;

                 if(bExistBwdX) bVtrlBwdX =  bBwdX;
            else if(bExistFwdX) bVtrlBwdX = !bFwdX;
            else if(bExistBwdY) bVtrlBwdX =  bBwdY;
            else if(bExistFwdY) bVtrlBwdX = !bFwdY;
            else return;

            //Virtual Output Sensor
            if(bExistFwdY) bVtrlFwdY =  bFwdY;
            else           bVtrlFwdY = !bBwdY;
            if(bExistBwdY) bVtrlBwdY =  bBwdY;
            else           bVtrlBwdY = !bFwdY;


            bDelayedFwd = DelayFwd.OnDelay(bVtrlFwdX,Para.iFwdOnDelay);
            bDelayedBwd = DelayBwd.OnDelay(bVtrlBwdX,Para.iBwdOnDelay);

            if(bDelayedFwd) Stat.eActPos = EN_CYL_POS.Fwd;
            if(bDelayedBwd) Stat.eActPos = EN_CYL_POS.Bwd;

            if(Stat.eCmdPos == Stat.eActPos)
            {
                TimeOutFwd.Clear();
                TimeOutBwd.Clear();
            }

            if ( bVtrlFwdY && !bVtrlBwdY) Stat.eCmdPos = EN_CYL_POS.Fwd;
            if (!bVtrlFwdY &&  bVtrlBwdY) Stat.eCmdPos = EN_CYL_POS.Bwd;

            bool bFwdErr = TimeOutFwd.OnDelay(Para.iFwdTimeOut) ;
            bool bBwdErr = TimeOutBwd.OnDelay(Para.iBwdTimeOut) ;

            Stat.bErr = bFwdErr || bBwdErr ;

            //TimeOutFwd.OnDelay(Stat.eCmdPos == EN_CYL_POS.cpFwd && Stat.eActPos != EN_CYL_POS.cpFwd,Para.iFwdTimeOut);
            //TimeOutBwd.OnDelay(Stat.eCmdPos == EN_CYL_POS.cpBwd && Stat.eActPos != EN_CYL_POS.cpBwd,Para.iBwdTimeOut);


            //if ( bVtrlFwdX && !bVtrlBwdX) Stat.eCmdPos = EN_CYL_POS.cpFwd;
            //if (!bVtrlFwdX &&  bVtrlBwdX) Stat.eCmdPos = EN_CYL_POS.cpBwd;

            //OnDelay Timer.
            //TimeOutFwd.OnDelay(Stat.eCmdPos == EN_CYL_POS.cpFwd && Stat.eActPos != EN_CYL_POS.cpFwd,Para.iFwdTimeOut);
            //TimeOutBwd.OnDelay(Stat.eCmdPos == EN_CYL_POS.cpBwd && Stat.eActPos != EN_CYL_POS.cpBwd,Para.iBwdTimeOut);

            


        }



        //public bool LoadSave(bool _bLoad, int _iCylNo , ref CConfig _Config)
        //{
        //    string sSelLan;
        //    string sCylNo;
        //
        //    switch(m_eLangSel)
        //    {
        //        default                  : sSelLan = "E_"; break;
        //        case EN_LAN_SEL.lsEnglish: sSelLan = "E_"; break;
        //        case EN_LAN_SEL.lsKorean : sSelLan = "K_"; break;
        //        case EN_LAN_SEL.lsChinese: sSelLan = "C_"; break;
        //    }
        //
        //    if(_bLoad)
        //    {
        //
        //        int iTemp;
        //
        //        sCylNo = string.Format("CYLINDER({0:000})",_iCylNo);
        //
        //
        //        _Config.GetValue(sCylNo,"sEnum      ",out Para.sEnum      );
        //        _Config.GetValue(sCylNo,"sName      ",out Para.sName      );
        //        _Config.GetValue(sCylNo,sSelLan+"sComment   ",out Para.sComment   );
        //        _Config.GetValue(sCylNo,sSelLan+"bFwdXAnd   ",out Para.bFwdXAnd   );
        //        _Config.GetValue(sCylNo,"bBwdXAnd   ",out Para.bBwdXAnd   );
        //        for(int j = 0;j < MAX_X_CNT;j++)
        //        {
        //            _Config.GetValue(sCylNo,"iFwdXAdd" + j.ToString(),out Para.iFwdXAdd[j]);
        //            _Config.GetValue(sCylNo,"iBwdXAdd" + j.ToString(),out Para.iBwdXAdd[j]);
        //
        //        }
        //        _Config.GetValue(sCylNo,"iFwdYAdd   ",out Para.iFwdYAdd   );
        //        _Config.GetValue(sCylNo,"iBwdYAdd   ",out Para.iBwdYAdd   );
        //        _Config.GetValue(sCylNo,"iFwdOnDelay",out Para.iFwdOnDelay);
        //        _Config.GetValue(sCylNo,"iBwdOnDelay",out Para.iBwdOnDelay);
        //        _Config.GetValue(sCylNo,"iFwdTimeOut",out Para.iFwdTimeOut);
        //        _Config.GetValue(sCylNo,"iBwdTimeOut",out Para.iBwdTimeOut);
        //        _Config.GetValue(sCylNo,"eDirType   ",out iTemp           ); Para.eDirType = (EN_MOVE_DIRECTION)iTemp;
        //        
        //    }
        //    else
        //    {
        //        sCylNo = string.Format("CYLINDER({0:000})",_iCylNo);
        //
        //
        //        _Config.SetValue(sCylNo,"sEnum      ", Para.sEnum      );
        //        _Config.SetValue(sCylNo,"sName      ", Para.sName      );
        //        _Config.SetValue(sCylNo,sSelLan+"sComment   ", Para.sComment   );
        //        _Config.SetValue(sCylNo,sSelLan+"bFwdXAnd   ", Para.bFwdXAnd   );
        //        _Config.SetValue(sCylNo,"bBwdXAnd   ", Para.bBwdXAnd   );
        //        for(int j = 0;j < MAX_X_CNT;j++)
        //        {
        //            _Config.SetValue(sCylNo,"iFwdXAdd"+j.ToString(), Para.iFwdXAdd[j]);
        //            _Config.SetValue(sCylNo,"iBwdXAdd"+j.ToString(), Para.iBwdXAdd[j]);
        //
        //        }
        //        _Config.SetValue(sCylNo,"iFwdYAdd   ", Para.iFwdYAdd);
        //        _Config.SetValue(sCylNo,"iBwdYAdd   ", Para.iBwdYAdd);
        //        _Config.SetValue(sCylNo,"iFwdOnDelay", Para.iFwdOnDelay);
        //        _Config.SetValue(sCylNo,"iBwdOnDelay", Para.iBwdOnDelay);
        //        _Config.SetValue(sCylNo,"iFwdTimeOut", Para.iFwdTimeOut);
        //        _Config.SetValue(sCylNo,"iBwdTimeOut", Para.iBwdTimeOut);
        //        _Config.SetValue(sCylNo,"eDirType   ", (int)Para.eDirType); 
        //    }
        //    return true;
        //}

    }


    public class CCylinderMan//========================================================================================================================
    {

        public CCylinderMan() { }


        public struct TRepeat//왕복 테스트용. 
        {
            public bool        bActivate;
            public int         iDelay;
            public List<int>   lCylinderList;//왕복할 애들..
            public CDelayTimer DelayRepeat;
            public int         iFrstActr;
            public int         iScndActr;
            public bool        bRepeatFwd;
        }

        int         m_iMaxCylinder   ;
        string      m_sParaFolderPath;
        public CCylinder[] m_aCylinder      ;
        public TRepeat     Repeat           ;

        CDioMan     DIO              ;
        EN_LAN_SEL m_eLangSel; //Languge Selection.

        public int _iMaxCylinder { get { return m_iMaxCylinder; } }

        bool CheckRangeOk(int _iCylNo)
        {
            if(_iCylNo < 0 || _iCylNo >= m_iMaxCylinder) return false;
            return true;
        }

        public bool Init(EN_LAN_SEL _eLanSel,string _sParaFolderPath,Enum _eCyl,CDioMan _Dio)
        {
            m_eLangSel = _eLanSel ;
            m_sParaFolderPath = _sParaFolderPath;
            Repeat.DelayRepeat = new CDelayTimer();

            Type type = _eCyl.GetType();
            Array arrayTemp = Enum.GetValues(type);
            m_iMaxCylinder = arrayTemp.Length -1 ;
            if(m_iMaxCylinder < 0) m_iMaxCylinder = 0 ;

            m_aCylinder       = new CCylinder[m_iMaxCylinder];

            DIO = _Dio ;

            for(int i = 0 ; i < m_iMaxCylinder ; i++) 
            {
                m_aCylinder[i] = new CCylinder();
                
                
            }
            LoadSave(true);
            for (int i = 0; i < m_iMaxCylinder; i++)
            {
                m_aCylinder[i].Init(ref _Dio);
                m_aCylinder[i].Para.sEnum = arrayTemp.GetValue(i).ToString();
            }

            

            Repeat.DelayRepeat.Clear();

            return true;
        }
        public bool Close()
        {
            return true;
        }

        public void Reset()
        {
            for(int i = 0;i < m_iMaxCylinder;i++)
            {
                m_aCylinder[i].Reset();
            }
        }


        //진섭 추가 20160405
        public CCylinder.TPara GetCylinderPara(int _iCylNo)
        {
            return m_aCylinder[_iCylNo].Para;
        }

        public CCylinderMan.TRepeat GetCylRptPara(int _iCylNo)
        {
            return Repeat;
        }

        public void SetCylinderPara(int _iCylNo, CCylinder.TPara _tPara)
        {
            m_aCylinder[_iCylNo].Para = _tPara;
        }



        public string GetName(int _iCylNo)
        {
            if (_iCylNo < 0 || _iCylNo >= m_iMaxCylinder) Log.ShowMessageFunc(string.Format("CYL : {0} is not in between 0 and MaxCylinder:{1}", _iCylNo, m_iMaxCylinder));
            return m_aCylinder[_iCylNo].GetName();
        }
        public bool Complete(int _iCylNo, EN_CYL_POS _bCmd)
        {
            if (_iCylNo < 0 || _iCylNo >= m_iMaxCylinder) Log.ShowMessageFunc(string.Format("CYL : {0} is not in between 0 and MaxCylinder:{1}", _iCylNo, m_iMaxCylinder));
            return m_aCylinder[_iCylNo].Complete(_bCmd);
        }
        public bool Complete(int _iCylNo)
        {
            if (_iCylNo < 0 || _iCylNo >= m_iMaxCylinder) Log.ShowMessageFunc(string.Format("CYL : {0} is not in between 0 and MaxCylinder:{1}", _iCylNo, m_iMaxCylinder));
            return m_aCylinder[_iCylNo].Complete();
        }
        public EN_CYL_POS GetCmd(int _iCylNo)
        {
            if (_iCylNo < 0 || _iCylNo >= m_iMaxCylinder) Log.ShowMessageFunc(string.Format("CYL : {0} is not in between 0 and MaxCylinder:{1}", _iCylNo, m_iMaxCylinder));
            return m_aCylinder[_iCylNo].GetCmd();
        }

        public EN_CYL_POS GetAct(int _iCylNo)
        {
            if (_iCylNo < 0 || _iCylNo >= m_iMaxCylinder) Log.ShowMessageFunc(string.Format("CYL : {0} is not in between 0 and MaxCylinder:{1}", _iCylNo, m_iMaxCylinder));
            return m_aCylinder[_iCylNo].GetAct();
        }

        public bool Err(int _iCylNo)
        {
            if (_iCylNo < 0 || _iCylNo >= m_iMaxCylinder) Log.ShowMessageFunc(string.Format("CYL : {0} is not in between 0 and MaxCylinder:{1}", _iCylNo, m_iMaxCylinder));
            return m_aCylinder[_iCylNo].Err();
        }
        public bool Move(int _iCylNo, EN_CYL_POS _bCmd)
        {
            if (_iCylNo < 0 || _iCylNo >= m_iMaxCylinder) Log.ShowMessageFunc(string.Format("CYL : {0} is not in between 0 and MaxCylinder:{1}", _iCylNo, m_iMaxCylinder));
            return m_aCylinder[_iCylNo].Move(_bCmd);
        }

        public void Update ()
        {
            //m_aCylinder[0].Update();
            for(int i = 0 ; i < m_iMaxCylinder ; i++)
            {
                m_aCylinder[i].Update();
            }
            if (Repeat.bActivate) RptActr(); 
        }

        //public bool LoadSave(bool _bLoad)
        //{
        //    CConfig Config = new CConfig();
        //
        //    //LoadSave(bool _bLoad, int _iCylNo , ref CConfig _Config)
        //    if(_bLoad)
        //    {
        //        Config.Load(m_sParaFolderPath + "Cylinder.ini",CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        //        for(int i = 0;i < m_iMaxCylinder;i++)
        //        {
        //            m_aCylinder[i].LoadSave(true,i,ref Config);
        //        }
        //    }
        //    else
        //    {
        //       
        //        for(int i = 0;i < m_iMaxCylinder;i++)
        //        {
        //            m_aCylinder[i].LoadSave(false,i,ref Config);
        //        }
        //        Config.Save(m_sParaFolderPath + "Cylinder.ini",CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        //    }
        //    return true;
        //}

        public bool LoadSave(bool _bLoad)
        {
            CConfig Config = new CConfig();

            string sSelLan;
            string sCylNo ;
            string sPath  ;

            switch (m_eLangSel)
            {
                default: sSelLan = "_E"; break;
                case EN_LAN_SEL.English: sSelLan = "_E"; break;
                case EN_LAN_SEL.Korean : sSelLan = "_K"; break;
                case EN_LAN_SEL.Chinese: sSelLan = "_C"; break;
            }
            sPath = m_sParaFolderPath + "Cylinder"  + sSelLan + ".ini";

            if (_bLoad)
            {
                Config.Load(sPath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);

                int iDirType = 0;

                for (int i = 0; i < m_iMaxCylinder; i++)
                {
                    sCylNo = string.Format("Cylinder({0:000})", i);
                    //Config.GetValue("sEnum"       , sCylNo, out m_aCylinder[i].Para.sEnum      );
                    Config.GetValue(sCylNo, "sName"       , out m_aCylinder[i].Para.sName      );
                    Config.GetValue(sCylNo, "sComment"    , out m_aCylinder[i].Para.sComment   );
                    Config.GetValue(sCylNo, "iFwdXAdd"    , out m_aCylinder[i].Para.iFwdXAdd   );
                    Config.GetValue(sCylNo, "iBwdXAdd"    , out m_aCylinder[i].Para.iBwdXAdd   );
                    Config.GetValue(sCylNo, "iFwdYAdd"    , out m_aCylinder[i].Para.iFwdYAdd   );
                    Config.GetValue(sCylNo, "iBwdYAdd"    , out m_aCylinder[i].Para.iBwdYAdd   );
                    Config.GetValue(sCylNo, "iFwdOnDelay" , out m_aCylinder[i].Para.iFwdOnDelay);
                    Config.GetValue(sCylNo, "iBwdOnDelay" , out m_aCylinder[i].Para.iBwdOnDelay);
                    Config.GetValue(sCylNo, "iFwdTimeOut" , out m_aCylinder[i].Para.iFwdTimeOut);
                    Config.GetValue(sCylNo, "iBwdTimeOut" , out m_aCylinder[i].Para.iBwdTimeOut);
                    Config.GetValue(sCylNo, "eDirType"    , out iDirType                       );
                    Config.GetValue(sCylNo, "bActrSync"   , out m_aCylinder[i].Para.bActrSync  );
                    Config.GetValue(sCylNo, "iActrSync"   , out m_aCylinder[i].Para.iActrSync  );
                    Config.GetValue(sCylNo, "iRptDelay"   , out Repeat.iDelay   );
                    
                    m_aCylinder[i].Para.eDirType = (EN_MOVE_DIRECTION)iDirType;
                }

            }
            else
            {
                for (int i = 0; i < m_iMaxCylinder; i++)
                {
                    sCylNo = string.Format("Cylinder({0:000})", i);
                    Config.SetValue(sCylNo, "sEnum"       ,     m_aCylinder[i].Para.sEnum      );
                    Config.SetValue(sCylNo, "sName"       ,     m_aCylinder[i].Para.sName      );
                    Config.SetValue(sCylNo, "sComment"    ,     m_aCylinder[i].Para.sComment   );
                    Config.SetValue(sCylNo, "iFwdXAdd"    ,     m_aCylinder[i].Para.iFwdXAdd   );
                    Config.SetValue(sCylNo, "iBwdXAdd"    ,     m_aCylinder[i].Para.iBwdXAdd   );
                    Config.SetValue(sCylNo, "iFwdYAdd"    ,     m_aCylinder[i].Para.iFwdYAdd   );
                    Config.SetValue(sCylNo, "iBwdYAdd"    ,     m_aCylinder[i].Para.iBwdYAdd   );
                    Config.SetValue(sCylNo, "iFwdOnDelay" ,     m_aCylinder[i].Para.iFwdOnDelay);
                    Config.SetValue(sCylNo, "iBwdOnDelay" ,     m_aCylinder[i].Para.iBwdOnDelay);
                    Config.SetValue(sCylNo, "iFwdTimeOut" ,     m_aCylinder[i].Para.iFwdTimeOut);
                    Config.SetValue(sCylNo, "iBwdTimeOut" ,     m_aCylinder[i].Para.iBwdTimeOut);
                    Config.SetValue(sCylNo, "eDirType"    ,(int)m_aCylinder[i].Para.eDirType   );
                    Config.SetValue(sCylNo, "bActrSync"   ,     m_aCylinder[i].Para.bActrSync  );
                    Config.SetValue(sCylNo, "iActrSync"   ,     m_aCylinder[i].Para.iActrSync  );
                    Config.SetValue(sCylNo, "iRptDelay"   ,     Repeat.iDelay  );
                }
                Config.Save(sPath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
   
            }
            /*
            if (_bLoad)
            {
                CIniFile CylConfig = new CIniFile(m_sParaFolderPath + "Cylinder.ini");

                int iDirType = 0;

                for (int i = 0; i < m_iMaxCylinder; i++)
                {
                    sCylNo = string.Format("Cylinder({0:000})", i);
                    CylConfig.Load(          "sEnum"       , sCylNo, out m_aCylinder[i].Para.sEnum      );
                    CylConfig.Load(sSelLan + "sName"       , sCylNo, out m_aCylinder[i].Para.sName      );
                    CylConfig.Load(sSelLan + "sComment"    , sCylNo, out m_aCylinder[i].Para.sComment   );
                    CylConfig.Load(          "iFwdXAdd"    , sCylNo, out m_aCylinder[i].Para.iFwdXAdd   );
                    CylConfig.Load(          "iBwdXAdd"    , sCylNo, out m_aCylinder[i].Para.iBwdXAdd   );
                    CylConfig.Load(          "iFwdYAdd"    , sCylNo, out m_aCylinder[i].Para.iFwdYAdd   );
                    CylConfig.Load(          "iBwdYAdd"    , sCylNo, out m_aCylinder[i].Para.iBwdYAdd   );
                    CylConfig.Load(          "iFwdOnDelay" , sCylNo, out m_aCylinder[i].Para.iFwdOnDelay);
                    CylConfig.Load(          "iBwdOnDelay" , sCylNo, out m_aCylinder[i].Para.iBwdOnDelay);
                    CylConfig.Load(          "iFwdTimeOut" , sCylNo, out m_aCylinder[i].Para.iFwdTimeOut);
                    CylConfig.Load(          "iBwdTimeOut" , sCylNo, out m_aCylinder[i].Para.iBwdTimeOut);
                    CylConfig.Load(          "eDirType"    , sCylNo, out iDirType                       );
                    CylConfig.Load(          "bActrSync"   , sCylNo, out m_aCylinder[i].Para.bActrSync  );
                    CylConfig.Load(          "iActrSync"   , sCylNo, out m_aCylinder[i].Para.iActrSync  );
                    CylConfig.Load(          "iRptDelay"   , sCylNo, out Repeat.iDelay   );
                    
                    m_aCylinder[i].Para.eDirType = (EN_MOVE_DIRECTION)iDirType;
                }

            }
            else
            {
                CIniFile CylConfig = new CIniFile(m_sParaFolderPath + "Cylinder.ini");

                for (int i = 0; i < m_iMaxCylinder; i++)
                {
                    sCylNo = string.Format("Cylinder({0:000})", i);
                    CylConfig.Save(          "sEnum"       , sCylNo,       m_aCylinder[i].Para.sEnum      );
                    CylConfig.Save(sSelLan + "sName"       , sCylNo,       m_aCylinder[i].Para.sName      );
                    CylConfig.Save(sSelLan + "sComment"    , sCylNo,       m_aCylinder[i].Para.sComment   );
                    CylConfig.Save(          "iFwdXAdd"    , sCylNo,       m_aCylinder[i].Para.iFwdXAdd   );
                    CylConfig.Save(          "iBwdXAdd"    , sCylNo,       m_aCylinder[i].Para.iBwdXAdd   );
                    CylConfig.Save(          "iFwdYAdd"    , sCylNo,       m_aCylinder[i].Para.iFwdYAdd   );
                    CylConfig.Save(          "iBwdYAdd"    , sCylNo,       m_aCylinder[i].Para.iBwdYAdd   );
                    CylConfig.Save(          "iFwdOnDelay" , sCylNo,       m_aCylinder[i].Para.iFwdOnDelay);
                    CylConfig.Save(          "iBwdOnDelay" , sCylNo,       m_aCylinder[i].Para.iBwdOnDelay);
                    CylConfig.Save(          "iFwdTimeOut" , sCylNo,       m_aCylinder[i].Para.iFwdTimeOut);
                    CylConfig.Save(          "iBwdTimeOut" , sCylNo,       m_aCylinder[i].Para.iBwdTimeOut);
                    CylConfig.Save(          "eDirType"    , sCylNo,  (int)m_aCylinder[i].Para.eDirType   );
                    CylConfig.Save(          "bActrSync"   , sCylNo,       m_aCylinder[i].Para.bActrSync  );
                    CylConfig.Save(          "iActrSync"   , sCylNo,       m_aCylinder[i].Para.iActrSync  );
                    CylConfig.Save(          "iRptDelay"   , sCylNo,       Repeat.iDelay  );
                }
   
            }*/
            return true;
        }

        public EN_MOVE_DIRECTION GetDirType(int _iCylNo)
        {
            return m_aCylinder[_iCylNo].Para.eDirType;
        }

        public void StopRpt ()
        {
            Repeat.bActivate = false ;
            Reset();
        }

        public void GoRpt(int _iDelay , int _iActr1No , int _iActr2No = -1)
        {
            if (_iActr1No < 0  || m_iMaxCylinder <= _iActr1No) { Log.ShowMessage("Error", "실린더 범위가 벗어났습니다."); return; }
            if (_iActr2No == -1) { _iActr2No = _iActr1No; }

            Repeat.iDelay = _iDelay;
            Repeat.iFrstActr = _iActr1No;
            Repeat.iScndActr = _iActr2No;
            Repeat.bActivate = true;
            if (GetCmd(_iActr1No) == EN_CYL_POS.Fwd && GetCmd(_iActr2No) == EN_CYL_POS.Fwd) Repeat.bRepeatFwd = true;
            if (GetCmd(_iActr1No) == EN_CYL_POS.Bwd && GetCmd(_iActr2No) == EN_CYL_POS.Bwd) Repeat.bRepeatFwd = false;

        }

        public void RptActr ()
        {
            if (Repeat.iScndActr == Repeat.iFrstActr)
            { //실린더 1개 맞출때
                //if (Repeat.DelayRepeat.OnDelay(GetCmd(Repeat.iFrstActr) == EN_CYL_POS.cpFwd, Repeat.iDelay))
                if (Repeat.DelayRepeat.OnDelay(true, Repeat.iDelay) && Repeat.bRepeatFwd)
                {
                    Move(Repeat.iFrstActr, EN_CYL_POS.Bwd);
                    Repeat.DelayRepeat.Clear();
                    Repeat.bRepeatFwd = false;
                }
                //if (Repeat.DelayRepeat.OnDelay(GetCmd(Repeat.iFrstActr) == EN_CYL_POS.cpBwd, Repeat.iDelay))
                if (Repeat.DelayRepeat.OnDelay(true, Repeat.iDelay) && !Repeat.bRepeatFwd)
                {
                    Move(Repeat.iFrstActr, EN_CYL_POS.Fwd);
                    Repeat.DelayRepeat.Clear();
                    Repeat.bRepeatFwd = true;
                }
            }
            else 
            {                      //실린더 2개 같이 맞출때
                //if (Repeat.DelayRepeat.OnDelay(GetCmd(Repeat.iFrstActr) == EN_CYL_POS.cpFwd, Repeat.iDelay))
                if (Repeat.DelayRepeat.OnDelay(true, Repeat.iDelay) && Repeat.bRepeatFwd)
                {
                    Move(Repeat.iFrstActr, EN_CYL_POS.Bwd);
                    Move(Repeat.iScndActr, EN_CYL_POS.Bwd);
                    Repeat.DelayRepeat.Clear();
                    Repeat.bRepeatFwd = false;
                }
                //if (Repeat.DelayRepeat.OnDelay(GetCmd(Repeat.iFrstActr) == EN_CYL_POS.cpBwd, Repeat.iDelay))
                if (Repeat.DelayRepeat.OnDelay(true, Repeat.iDelay) && !Repeat.bRepeatFwd)
                {
                    Move(Repeat.iFrstActr, EN_CYL_POS.Fwd);
                    Move(Repeat.iScndActr, EN_CYL_POS.Fwd);
                    Repeat.DelayRepeat.Clear();
                    Repeat.bRepeatFwd = true;
                }
            }
        }

        public void DisplayStatus(int _iActrNo , Label _lbFwdStat , Label _lbBwdStat , Label _lbAlarm)
        {
            //Check Null.
            if ((_lbFwdStat == null) || (_lbBwdStat == null) || (_lbAlarm == null)) return;
        
            //Display.
            if (m_aCylinder[_iActrNo].Complete(EN_CYL_POS.Fwd)) _lbFwdStat .BackColor = Color.Lime;
            else                                                       _lbFwdStat .BackColor = Color.Gray;
            if (m_aCylinder[_iActrNo].Complete(EN_CYL_POS.Bwd)) _lbBwdStat .BackColor = Color.Lime;
            else                                                       _lbBwdStat .BackColor = Color.Gray;
            if(m_aCylinder[_iActrNo].Para.iFwdXAdd != -1 || m_aCylinder[_iActrNo].Para.iBwdXAdd != -1 /*|| 
               m_aCylinder[_iActrNo].Para.iFwdYAdd != -1 || m_aCylinder[_iActrNo].Para.iBwdYAdd != -1*/)
            {
                if (m_aCylinder[_iActrNo].Err()                          ) _lbAlarm   .BackColor = Color.Red ;
                else                                                       _lbAlarm   .BackColor = Color.Gray;
            }
            
        
        }
     
    }
}
