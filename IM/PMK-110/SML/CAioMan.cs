using COMMON;
using MotionInterface;
using MotionAXL;
using System;
using System.Collections.Generic;

namespace SML
{
    public class CAioMan
    {

        public CAioMan() { }

        //접점하나에 관한 파라.
        public struct TPara
        {
            //Para.
            public int    iAdd;  //주소
            public string sEnum; //이넘
            public string sName; //이름
            public string SComt; //설명
        }

        //접점 하나에 관한 스탯.
        public struct TStat
        {
            //Value.
            public double dAtrVal ; //딜레이가 가미된 가상의 값 프로그램에서 이값으로 쓴다.
            public double dAtrValA; //실제센서 값
            public double dAtrValD; //실제센서 값
        };

        public struct TAIo
        {
           
            public TPara Para;
            public TStat Stat;

            public CDelayTimer DelayOn ;//= new CDelayTimer();
            public CDelayTimer DelayOff;//= new CDelayTimer();

        }

        public TAIo[] m_aIn;
        public TAIo[] m_aOut;


        EN_LAN_SEL m_eLangSel = EN_LAN_SEL.English; //Languge Selection.
        string     m_sParaFolderPath;
        bool       m_bTestMode;
        int        m_iMaxIn ; public int _iMaxIn  { get { return m_iMaxIn;  } }
        int        m_iMaxOut; public int _iMaxOut { get { return m_iMaxOut; } }

        EN_AIO_SEL AioSel;

        IAio Aio = null;
        
        public bool Init(EN_LAN_SEL _eLanSel,string _sParaFolderPath,EN_AIO_SEL _eAioSel,Enum _eX,Enum _eY,int _iRangeAMin,int _iRangeAMax)
        {
            m_eLangSel = _eLanSel ;
            m_sParaFolderPath = _sParaFolderPath;
            AioSel = _eAioSel;

            Type type1 = _eX.GetType();
            Array arrayTemp1 = Enum.GetValues(type1);
            m_iMaxIn  = arrayTemp1.Length - 1;
            if (m_iMaxIn < 0) m_iMaxIn = 0;

            Type type2 = _eY.GetType();
            Array arrayTemp2 = Enum.GetValues(type2);
            m_iMaxOut = arrayTemp2.Length - 1;
            if(m_iMaxOut < 0) m_iMaxOut = 0 ;

            m_aIn  = new TAIo[m_iMaxIn ];
            m_aOut = new TAIo[m_iMaxOut];

            if(_eAioSel == EN_AIO_SEL.AXL)
            {
                Aio = new MotionAXL.CAio(_iRangeAMin,_iRangeAMax);   
            }
            else if(_eAioSel == EN_AIO_SEL.None)
            {
                return false;
            }
            else
            {
                Log.ShowMessage("Err", "_eAioSel is not defined");
                return false;
            }

            Aio.Init();

            //int iCnt = 0 ;
            //foreach ( Object obj in arrayTemp1 )  {
                //m_aIn[iCnt].Para.sEnum = obj.ToString();
            for(int i=0; i<m_iMaxIn; i++) { 
                m_aIn[i].Para.sEnum = arrayTemp1.GetValue(i).ToString();
            }
            for(int i=0; i<m_iMaxOut; i++) {
                m_aOut[i].Para.sEnum = arrayTemp2.GetValue(i).ToString();
            }

            LoadSave(true);
            //LoadSave(false);


            //for (int i = 0 ; i < m_iMaxIn ; i++) 
            //{
            //    m_aIn[i].DelayOn  = new CDelayTimer();
            //    m_aIn[i].DelayOff = new CDelayTimer();
            //}
        
            //for (int i = 0 ; i < m_iMaxOut ; i++) 
            //{
            //    m_aOut[i].DelayOn  = new CDelayTimer();
            //    m_aOut[i].DelayOff = new CDelayTimer();

            //    m_aOut[i].Stat.dAtrValA = GetY(i, true);
            //    m_aOut[i].Stat.bVtrVal = m_aOut[i].Stat.bAtrVal;
            //}

            Log.Trace("SMDLL","Init Finished");

            return true;
        }
        public bool Close()
        {
            if(AioSel != EN_AIO_SEL.None) Aio.Close();
            return true;
        }

        public void Update()
        {    
            //Output.
            for (int i = 0 ; i < m_iMaxOut ; i++) {

                //Set Output.
                Aio.SetOut(m_aOut[i].Para.iAdd , m_aOut[i].Stat.dAtrVal);
                m_aOut[i].Stat.dAtrValA = Aio.GetOut(m_aOut[i].Para.iAdd);
                //m_aOut[i].Stat.dAtrValD = Aio.GetOut(m_aOut[i].Para.iAdd, true);
            }
        
            for (int i = 0 ; i < m_iMaxIn ; i++) {
                m_aIn[i].Stat.dAtrValA = Aio.GetIn(m_aIn[i].Para.iAdd);
                m_aIn[i].Stat.dAtrValD = Aio.GetIn(m_aIn[i].Para.iAdd,true);
            }
        }

        public void SetYTestMode(int _iNo , double _dVal)
        {
            if (_iNo < 0 || _iNo >= m_iMaxOut)
            {
                Log.ShowMessageFunc(string.Format("ERR_A OUT_A{0:000} is not Between 0 and {1}", _iNo, m_iMaxOut));
                return;
            }

            m_aOut[_iNo].Stat.dAtrVal = _dVal;
            Log.Trace(string.Format("OUTPUT_A({0:000}) {1} is {2:#} (Direct)", _iNo, m_aOut[_iNo].Para.sEnum, _dVal ), ForContext.Sts);
            Aio.SetOut(m_aOut[_iNo].Para.iAdd, _dVal);
            
        }
        
        public void SetY(int _iNo , double _dVal) 
        {
            if (m_bTestMode) return;
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR_A OUT_A({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                return ; 
            }
        
            m_aOut[_iNo].Stat.dAtrVal = _dVal ;
            Aio.SetOut(m_aOut[_iNo].Para.iAdd , _dVal);
        }
        
        public double GetY(int _iNo)
        {
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR_A OUT({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                return 0.0; 
            }
            return m_aOut[_iNo].Stat.dAtrValA;
            //return Aio.GetOut(m_aOut[_iNo].Para.iAdd);
        }
        
        public double GetX(int _iNo, bool _bDigit = false)
        {
            if( _iNo < 0 || _iNo >= m_iMaxIn) 
            {
                Log.ShowMessageFunc(string.Format("ERR_A IN({0:000}) is not Between 0 and {1}",_iNo,m_iMaxIn));
                return 0.0; 
            }
            if (_bDigit)
            {
                return m_aIn[_iNo].Stat.dAtrValD;
            }
            return m_aIn[_iNo].Stat.dAtrValA;
            //return m_aIn[_iNo].Stat.dAtrValD;
            //return Aio.GetIn(m_aIn[_iNo].Para.iAdd,_bDigit);
         }
        
        public bool GetModleInfoX (int _iNo , out int _iModuleNo , out int _iModuleNoDp , out int _iOffset)
        {

            if( _iNo < 0 || _iNo >= m_iMaxIn) 
            {
                Log.ShowMessageFunc(string.Format("ERR_A IN({0:000}) is not Between 0 and {1}",_iNo,m_iMaxIn));
                _iModuleNo   = 0 ;
                _iModuleNoDp = 0 ;
                _iOffset = 0;
                return false; 
            }
            return Aio.GetInfoInput(m_aIn[_iNo].Para.iAdd , out _iModuleNo , out _iModuleNoDp, out _iOffset ) ;
        }

        public bool GetModuleInfoY(int _iNo, out int _iModuleNo, out int _iModuleNoDp, out int _iOffset)
        {
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR_A OUT({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                _iModuleNo = 0;
                _iModuleNoDp = 0;
                _iOffset = 0;
                return false; 
            }

            return Aio.GetInfoOutput(m_aOut[_iNo].Para.iAdd, out _iModuleNo, out _iModuleNoDp, out _iOffset);
        }

        public void SetTestMode(bool _bOn)
        {
            m_bTestMode = _bOn;
        }

        public bool GetTestMode()
        {
            return m_bTestMode;
        }

        public bool LoadSave(bool _bLoad)
        {
            CConfig Config1 = new CConfig(); // Input
            CConfig Config2 = new CConfig(); // Output

            string sSelLan;
            string sIndex ;
            string sPath1 ;
            string sPath2 ;

            switch (m_eLangSel)
            {
                default: sSelLan = "_E"; break;
                case EN_LAN_SEL.English: sSelLan = "_E"; break;
                case EN_LAN_SEL.Korean : sSelLan = "_K"; break;
                case EN_LAN_SEL.Chinese: sSelLan = "_C"; break;
            }
            sPath1 = m_sParaFolderPath + "AInput"  + sSelLan + ".ini";
            sPath2 = m_sParaFolderPath + "AOutput" + sSelLan + ".ini";

            if (_bLoad)
            {
                Config1.Load(sPath1,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    Config1.GetValue(sIndex, "Address  ", out m_aIn[i].Para.iAdd );
                    Config1.GetValue(sIndex, "Enum     ", out m_aIn[i].Para.sEnum);
                    Config1.GetValue(sIndex, "Name     ", out m_aIn[i].Para.sName);
                    Config1.GetValue(sIndex, "Comment  ", out m_aIn[i].Para.SComt);
                }        
                                                                           
                Config2.Load(sPath2,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
                for (int i = 0; i < m_iMaxOut ; i++)
                {
                    sIndex = string.Format("Output({0:000})", i);
                    Config2.GetValue(sIndex, "Address  ", out m_aOut[i].Para.iAdd );
                    Config2.GetValue(sIndex, "Enum     ", out m_aOut[i].Para.sEnum);
                    Config2.GetValue(sIndex, "Name     ", out m_aOut[i].Para.sName);
                    Config2.GetValue(sIndex, "Comment  ", out m_aOut[i].Para.SComt);
                }
            }
            else
            {
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    Config1.SetValue(sIndex, "Address  ", m_aIn[i].Para.iAdd );
                    Config1.SetValue(sIndex, "Enum     ", m_aIn[i].Para.sEnum);
                    Config1.SetValue(sIndex, "Name     ", m_aIn[i].Para.sName);
                    Config1.SetValue(sIndex, "Comment  ", m_aIn[i].Para.SComt);
                }
                Config1.Save(sPath1,CConfig.EN_CONFIG_FILE_TYPE.ftIni);

                for (int i = 0; i < m_iMaxOut ; i++)
                {
                    sIndex = string.Format("Output({0:000})", i);
                    Config2.SetValue(sIndex, "Address  ", m_aOut[i].Para.iAdd );
                    Config2.SetValue(sIndex, "Enum     ", m_aOut[i].Para.sEnum);
                    Config2.SetValue(sIndex, "Name     ", m_aOut[i].Para.sName);
                    Config2.SetValue(sIndex, "Comment  ", m_aOut[i].Para.SComt);
                }
                Config2.Save(sPath2,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            }
            /*
            if (_bLoad)
            {
                CIniFile InputConfig = new CIniFile(m_sParaFolderPath + "Input_A.ini");
                
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    InputConfig.Load(          "Address  ", sIndex, out m_aIn[i].Para.iAdd     );
                    InputConfig.Load(          "Enum     ", sIndex, out m_aIn[i].Para.sEnum    );
                    InputConfig.Load(sSelLan + "Name     ", sIndex, out m_aIn[i].Para.sName    );
                    InputConfig.Load(sSelLan + "Comment  ", sIndex, out m_aIn[i].Para.SComt    );
                }

                CIniFile OutputConfig = new CIniFile(m_sParaFolderPath + "Output_A.ini");
                
                for (int i = 0; i < m_iMaxOut ; i++)
                {
                    sIndex = string.Format("Output({0:000})", i);
                    OutputConfig.Load(          "Address  ", sIndex, out m_aOut[i].Para.iAdd     );
                    OutputConfig.Load(          "Enum     ", sIndex, out m_aOut[i].Para.sEnum    );
                    OutputConfig.Load(sSelLan + "Name     ", sIndex, out m_aOut[i].Para.sName    );
                    OutputConfig.Load(sSelLan + "Comment  ", sIndex, out m_aOut[i].Para.SComt    );
                }
            }
            else
            {
                CIniFile InputConfig = new CIniFile(m_sParaFolderPath + "Input_A.ini");
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    InputConfig.Save(          "Address  ", sIndex, m_aIn[i].Para.iAdd);
                    InputConfig.Save(          "Enum     ", sIndex, m_aIn[i].Para.sEnum);
                    InputConfig.Save(sSelLan + "Name     ", sIndex, m_aIn[i].Para.sName);
                    InputConfig.Save(sSelLan + "Comment  ", sIndex, m_aIn[i].Para.SComt);
                }

                CIniFile OutputConfig = new CIniFile(m_sParaFolderPath + "Output_A.ini");
                for (int i = 0; i < m_iMaxOut ; i++)
                {
                    sIndex = string.Format("Output({0:000})", i);
                    OutputConfig.Save(          "Address  ", sIndex, m_aOut[i].Para.iAdd);
                    OutputConfig.Save(          "Enum     ", sIndex, m_aOut[i].Para.sEnum);
                    OutputConfig.Save(sSelLan + "Name     ", sIndex, m_aOut[i].Para.sName);
                    OutputConfig.Save(sSelLan + "Comment  ", sIndex, m_aOut[i].Para.SComt);
                }
             
            }
            */
            return true;
        }
        public TPara GetInputPara(int _iNo)
        {
            return m_aIn[_iNo].Para;

        }
        public TPara GetOutputPara(int _iNo)
        {
            return m_aOut[_iNo].Para;
        }

        public void SetInputPara(int _iNo , TPara _tPara)
        {
            m_aIn[_iNo].Para = _tPara;

        }
        public void SetOutputPara(int _iNo , TPara _tPara)
        {
            m_aOut[_iNo].Para = _tPara;
        }

        public string GetYName(int _iNo)
        {
            return m_aOut[_iNo].Para.sName;
        }
        public string GetXName(int _iNo)
        {
            return m_aIn[_iNo].Para.sName;
        }
    }


}
