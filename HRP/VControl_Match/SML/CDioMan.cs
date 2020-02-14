using COMMON;
using MotionInterface;
using System;

namespace SML
{
    public class CDioMan
    {

        public CDioMan() { }


        //접점하나에 관한 파라.
        public struct TPara
        {
            //Para.
            public int    iAdd;  //주소
            public string sEnum; //이넘
            public string sName; //이름
            public string SComt; //설명
            public bool   bInv;  //반전
            public int    iOnDelay; //지연시간  출력은 사용 안함.
            public int    iOffDelay; //지연시간
            public bool   bNotLog;  //true이면 로그 안남기도록
        }

        //접점 하나에 관한 스탯.
        public struct TStat
        {
            //Value.
            public bool bVtrVal; //딜레이가 가미된 가상의 값 프로그램에서 이값으로 쓴다.
            public bool bAtrVal; //실제센서 값
            public bool bPreVal; //로그 남기기위한 전싸이클 상태.
            public bool bUpEdge; //업엣지
            public bool bDnEdge; //다운엣지
        };

        public struct TDIo
        {
           
            public TPara Para;
            public TStat Stat;

            public CDelayTimer DelayOn ;//= new CDelayTimer();
            public CDelayTimer DelayOff;//= new CDelayTimer();

        }

        public TDIo[] m_aIn;
        public TDIo[] m_aOut;


        EN_LAN_SEL m_eLangSel = EN_LAN_SEL.Korean; //Languge Selection.
        string     m_sParaFolderPath;
        bool       m_bTestMode;
        int        m_iMaxIn ; public int _iMaxIn  { get { return m_iMaxIn;  } }
        int        m_iMaxOut; public int _iMaxOut { get { return m_iMaxOut; } }

        IDio Dio = null;

        public bool Init(EN_LAN_SEL _eLanSel,string _sParaFolderPath,EN_DIO_SEL _eDioSel,Enum _eX,Enum _eY)
        {
            m_eLangSel = _eLanSel ;
            m_sParaFolderPath = _sParaFolderPath;

            Type type1 = _eX.GetType();
            Array arrayTemp1 = Enum.GetValues(type1);
            m_iMaxIn = arrayTemp1.Length - 1;
            if(m_iMaxIn < 0) m_iMaxIn = 0 ;

            Type type2 = _eY.GetType();
            Array arrayTemp2 = Enum.GetValues(type2);
            m_iMaxOut = arrayTemp2.Length - 1;
            if(m_iMaxOut < 0) m_iMaxOut = 0 ;

            m_aIn  = new TDIo[m_iMaxIn];
            m_aOut = new TDIo[m_iMaxOut];

            if(_eDioSel == EN_DIO_SEL.AXL)
            {
                Dio = new MotionAXL.CDio();   
            }
            else if(_eDioSel == EN_DIO_SEL.NMC2)
            {
                Dio = new MotionNMC2.CDio();
            }
            else
            {
                Log.ShowMessage("Err", "_eDioSel is not defined");
                return false;
            }

            Dio.Init();

            for(int i=0; i<m_iMaxIn; i++) { 
                m_aIn[i].Para.sEnum = arrayTemp1.GetValue(i).ToString();
            }
            for(int i=0; i<m_iMaxOut; i++) { 
                m_aOut[i].Para.sEnum = arrayTemp2.GetValue(i).ToString();
            }

            LoadSave(true);
            //LoadSave(false);


            for (int i = 0 ; i < m_iMaxIn ; i++) 
            {
                m_aIn[i].DelayOn  = new CDelayTimer();
                m_aIn[i].DelayOff = new CDelayTimer();
            }
        
            for (int i = 0 ; i < m_iMaxOut ; i++) 
            {
                m_aOut[i].DelayOn  = new CDelayTimer();
                m_aOut[i].DelayOff = new CDelayTimer();

                m_aOut[i].Stat.bAtrVal = GetY(i, true);
                m_aOut[i].Stat.bVtrVal = m_aOut[i].Stat.bAtrVal;
            }

            Log.Trace("SMDLL","Init Finished");

            return true;
        }
        public bool Close()
        {
            Dio.Close();
            return true;
        }

        public void Update()
        {    
            //Output.
            for (int i = 0 ; i < m_iMaxOut ; i++) {
                m_aOut[i].Stat.bAtrVal = Dio.GetOut(m_aOut[i].Para.iAdd);
                //Check Delay.
                if(m_aOut[i].Stat.bVtrVal) 
                {
                    m_aOut[i].DelayOff.Clear();
                    //Check
                    if (m_aOut[i].Para.iOnDelay > 0 )//&& m_aOut[i].Stat.bVtrVal != m_aOut[i].Stat.bAtrVal)
                    {
                        if (!m_aOut[i].DelayOn.OnDelay(m_aOut[i].Stat.bVtrVal != m_aOut[i].Stat.bAtrVal, m_aOut[i].Para.iOnDelay))
                        {
                            continue;
                        }
                    }
                }
                else 
                {
                    m_aOut[i].DelayOn.Clear();
                    if (m_aOut[i].Para.iOffDelay > 0 )//&& m_aOut[i].Stat.bVtrVal != m_aOut[i].Stat.bAtrVal)
                    {
                        if(!m_aOut[i].DelayOff.OnDelay (m_aOut[i].Stat.bVtrVal != m_aOut[i].Stat.bAtrVal , m_aOut[i].Para.iOffDelay))
                        {
                            continue ;
                        }
                    }
                }
        
                //Edge.
                if(m_aOut[i].Stat.bVtrVal != m_aOut[i].Stat.bAtrVal ) {
                    m_aOut[i].Stat.bUpEdge = !m_aOut[i].Stat.bAtrVal ;
                    m_aOut[i].Stat.bDnEdge =  m_aOut[i].Stat.bAtrVal ;
                    
                    //노트북에서 사용시에 Actual Virtual Log problems.
                    if (!m_aOut[i].Para.bNotLog)
                    {
                        Log.Trace(string.Format("OUTPUT({0:000}) {1} is {2} with Delay {3}", i, m_aOut[i].Para.sEnum, m_aOut[i].Stat.bVtrVal ? "On" : "Off", m_aOut[i].Stat.bVtrVal ? m_aOut[i].Para.iOnDelay : m_aOut[i].Para.iOffDelay),ti.Dyi);
                    }
                }
                else {
                    m_aOut[i].Stat.bUpEdge = false ;
                    m_aOut[i].Stat.bDnEdge = false ;
                }

                Dio.SetOut(m_aOut[i].Para.iAdd, m_aOut[i].Stat.bVtrVal);

            }
                    
            //Ouput 밑에 있어야 하고 Input 위에 있어야 ...
            Dio.Update();
            
            for (int i = 0 ; i < m_iMaxIn ; i++) {
                m_aIn[i].Stat.bAtrVal = Dio.GetIn(m_aIn[i].Para.iAdd) ;
        
                //Check
                if(m_aIn[i].Para.iOnDelay > 0 && !m_aIn[i].Stat.bVtrVal)
                {
                    if(!m_aIn[i].DelayOn.OnDelay (m_aIn[i].Stat.bAtrVal != m_aIn[i].Stat.bVtrVal , m_aIn[i].Para.iOnDelay ))
                    {
                        m_aIn[i].Stat.bUpEdge = false ;
                        m_aIn[i].Stat.bDnEdge = false ;
                        continue ;
                    }
                }
        
                if(m_aIn[i].Para.iOffDelay > 0 &&  m_aIn[i].Stat.bVtrVal )
                {
                    if(!m_aIn[i].DelayOff.OnDelay (m_aIn[i].Stat.bAtrVal != m_aIn[i].Stat.bVtrVal , m_aIn[i].Para.iOffDelay))
                    {
                        m_aIn[i].Stat.bUpEdge = false ;
                        m_aIn[i].Stat.bDnEdge = false ;
                        continue ;
                    }
                }        
        
                //Edge.
                if(m_aIn[i].Stat.bVtrVal != m_aIn[i].Stat.bAtrVal) {
                    m_aIn[i].Stat.bUpEdge = !m_aIn[i].Stat.bVtrVal ;
                    m_aIn[i].Stat.bDnEdge =  m_aIn[i].Stat.bVtrVal ;
                    if (!m_aIn[i].Para.bNotLog)
                    {
                        Log.Trace(string.Format("INPUT({0:000}) {1} is {2} with Delay {3}", i, m_aIn[i].Para.sEnum, m_aIn[i].Stat.bAtrVal ? "On" : "Off", m_aIn[i].Stat.bAtrVal ? m_aIn[i].Para.iOnDelay : m_aIn[i].Para.iOffDelay), ti.Dxi);
                    }
                }
                else {
                    m_aIn[i].Stat.bUpEdge = false ;
                    m_aIn[i].Stat.bDnEdge = false ;
                }
        
                //Set Input.
                m_aIn[i].Stat.bVtrVal = m_aIn[i].Stat.bAtrVal ;


            }

            
        }

        public void SetYTestMode(int _iNo , bool _bVal)
        {
            if (_iNo < 0 || _iNo >= m_iMaxOut)
            {
                Log.ShowMessageFunc(string.Format("ERR OUT{0:000} is not Between 0 and {1}", _iNo, m_iMaxOut));
                return;
            }

            m_aOut[_iNo].Stat.bVtrVal = _bVal;
            Log.Trace(string.Format("OUTPUT({0:000}) {1} is {2} (Direct)", _iNo, m_aOut[_iNo].Para.sEnum, _bVal ? "On" : "Off"), ti.Dyi);
            Dio.SetOut(m_aOut[_iNo].Para.iAdd, _bVal);
            
        }
        
        public void SetY(int _iNo , bool _bVal , bool _bDirect=false) //다이렉트가 트루이면 테스트 모드에서도 동작함 주의.
        {
            if (m_bTestMode) return;
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR OUT({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                return ; 
            }
        
            m_aOut[_iNo].Stat.bVtrVal = _bVal ;
        
            if(_bDirect && Dio.GetOut(m_aOut[_iNo].Para.iAdd) != _bVal) {
                Log.Trace(string.Format("OUTPUT({0:000}) {1} is {2} (Direct)",_iNo,m_aOut[_iNo].Para.sEnum , _bVal ? "On":"Off"), ti.Dyi);
                Dio.SetOut(m_aOut[_iNo].Para.iAdd , _bVal , true);
            }
        }
        
        public bool GetY(int _iNo , bool _bDirect=false)
        {
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR OUT({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                return false; 
            }

            if(_bDirect) return Dio.GetOut(m_aOut[_iNo].Para.iAdd,true);

            
            return m_aOut[_iNo].Para.bInv ? !m_aOut[_iNo].Stat.bAtrVal : m_aOut[_iNo].Stat.bAtrVal;
            //return m_aOut[_iNo].Stat.bAtrVal;
        }
        
        public bool GetYDn(int _iNo )
        {
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR OUT({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                return false; 
            }

            return m_aOut[_iNo].Para.bInv ? m_aOut[_iNo].Stat.bDnEdge : m_aOut[_iNo].Stat.bUpEdge;
            //return m_aOut[_iNo].Stat.bDnEdge;
        }
        
        public bool GetYUp(int _iNo )
        {                                                        
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR OUT({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                return false; 
            }

            return m_aOut[_iNo].Para.bInv ? m_aOut[_iNo].Stat.bUpEdge : m_aOut[_iNo].Stat.bDnEdge;
            //return m_aOut[_iNo].Stat.bUpEdge ;
        }
        
        
        
        public bool GetX(int _iNo , bool _bDirect=false)
        {
            if( _iNo < 0 || _iNo >= m_iMaxIn) 
            {
                Log.ShowMessageFunc(string.Format("ERR IN({0:000}) is not Between 0 and {1}",_iNo,m_iMaxIn));
                return false; 
            }
        
            if(_bDirect) return Dio.GetIn(m_aIn[_iNo].Para.iAdd , true);

            return m_aIn[_iNo].Para.bInv ? !m_aIn[_iNo].Stat.bVtrVal : m_aIn[_iNo].Stat.bVtrVal;
            //return m_aIn[_iNo].Stat.bVtrVal ;
         }
        
        public bool GetXDn(int _iNo )
        {
            if( _iNo < 0 || _iNo >= m_iMaxIn) 
            {
                Log.ShowMessageFunc(string.Format("ERR IN({0:000}) is not Between 0 and {1}",_iNo,m_iMaxIn));
                return false; 
            }

            return m_aIn[_iNo].Para.bInv ? m_aIn[_iNo].Stat.bDnEdge : m_aIn[_iNo].Stat.bUpEdge;
            //return m_aIn[_iNo].Stat.bDnEdge ;
        }
        
        public bool GetXUp(int _iNo )
        {
            if( _iNo < 0 || _iNo >= m_iMaxIn) 
            {
                Log.ShowMessageFunc(string.Format("ERR IN({0:000}) is not Between 0 and {1}",_iNo,m_iMaxIn));
                return false; 
            }

            return m_aIn[_iNo].Para.bInv ? m_aIn[_iNo].Stat.bUpEdge : m_aIn[_iNo].Stat.bDnEdge;
            //return m_aIn[_iNo].Stat.bUpEdge ;
        }
        
        public bool GetModleInfoX (int _iNo , out int _iModuleNo , out int _iModuleNoDp , out int _iOffset)
        {

            if( _iNo < 0 || _iNo >= m_iMaxIn) 
            {
                Log.ShowMessageFunc(string.Format("ERR IN({0:000}) is not Between 0 and {1}",_iNo,m_iMaxIn));
                _iModuleNo   = 0 ;
                _iModuleNoDp = 0 ;
                _iOffset = 0;
                return false; 
            }
            return Dio.GetInfoInput(m_aIn[_iNo].Para.iAdd , out _iModuleNo , out _iModuleNoDp, out _iOffset ) ;
        }

        public bool GetModuleInfoY(int _iNo, out int _iModuleNo, out int _iModuleNoDp, out int _iOffset)
        {
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR OUT({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                _iModuleNo = 0;
                _iModuleNoDp = 0;
                _iOffset = 0;
                return false; 
            }

            return Dio.GetInfoOutput(m_aOut[_iNo].Para.iAdd, out _iModuleNo, out _iModuleNoDp, out _iOffset);
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
            sPath1 = m_sParaFolderPath + "DInput"  + sSelLan + ".ini";
            sPath2 = m_sParaFolderPath + "DOutput" + sSelLan + ".ini";

            if (_bLoad)
            {
                Config1.Load(sPath1,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
                
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    Config1.GetValue(sIndex, "Address  ", out m_aIn[i].Para.iAdd     );
                    Config1.GetValue(sIndex, "Invert   ", out m_aIn[i].Para.bInv     );
                    //Config1.GetValue(sIndex, "Enum     ", out m_aIn[i].Para.sEnum    );
                    Config1.GetValue(sIndex, "Name     ", out m_aIn[i].Para.sName    );
                    Config1.GetValue(sIndex, "Comment  ", out m_aIn[i].Para.SComt    );
                    Config1.GetValue(sIndex, "OnDelay  ", out m_aIn[i].Para.iOnDelay );
                    Config1.GetValue(sIndex, "OffDelay ", out m_aIn[i].Para.iOffDelay);
                    Config1.GetValue(sIndex, "NotLog   ", out m_aIn[i].Para.bNotLog  );
                }

                Config2.Load(sPath2,CConfig.EN_CONFIG_FILE_TYPE.ftIni);

                for (int i = 0; i < m_iMaxOut ; i++)
                {
                    sIndex = string.Format("Output({0:000})", i);
                    Config2.GetValue(sIndex, "Address  ", out m_aOut[i].Para.iAdd     );
                    Config2.GetValue(sIndex, "Invert   ", out m_aOut[i].Para.bInv     );
                    //Config2.GetValue(sIndex, "Enum     ", out m_aOut[i].Para.sEnum    );
                    Config2.GetValue(sIndex, "Name     ", out m_aOut[i].Para.sName    );
                    Config2.GetValue(sIndex, "Comment  ", out m_aOut[i].Para.SComt    );
                    Config2.GetValue(sIndex, "OnDelay  ", out m_aOut[i].Para.iOnDelay );
                    Config2.GetValue(sIndex, "OffDelay ", out m_aOut[i].Para.iOffDelay);
                    Config2.GetValue(sIndex, "NotLog   ", out m_aOut[i].Para.bNotLog  );
                }
            }
            else
            {
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    Config1.SetValue(sIndex, "Address  ", m_aIn[i].Para.iAdd     );
                    Config1.SetValue(sIndex, "Invert   ", m_aIn[i].Para.bInv     );
                    Config1.SetValue(sIndex, "Enum     ", m_aIn[i].Para.sEnum    );
                    Config1.SetValue(sIndex, "Name     ", m_aIn[i].Para.sName    );
                    Config1.SetValue(sIndex, "Comment  ", m_aIn[i].Para.SComt    );
                    Config1.SetValue(sIndex, "OnDelay  ", m_aIn[i].Para.iOnDelay );
                    Config1.SetValue(sIndex, "OffDelay ", m_aIn[i].Para.iOffDelay);
                    Config1.SetValue(sIndex, "NotLog   ", m_aIn[i].Para.bNotLog  );
                }
                Config1.Save(sPath1,CConfig.EN_CONFIG_FILE_TYPE.ftIni);

                for (int i = 0; i < m_iMaxOut ; i++)
                {
                    sIndex = string.Format("Output({0:000})", i);
                    Config2.SetValue(sIndex, "Address  ", m_aOut[i].Para.iAdd     );
                    Config2.SetValue(sIndex, "Invert   ", m_aOut[i].Para.bInv     );
                    Config2.SetValue(sIndex, "Enum     ", m_aOut[i].Para.sEnum    );
                    Config2.SetValue(sIndex, "Name     ", m_aOut[i].Para.sName    );
                    Config2.SetValue(sIndex, "Comment  ", m_aOut[i].Para.SComt    );
                    Config2.SetValue(sIndex, "OnDelay  ", m_aOut[i].Para.iOnDelay );
                    Config2.SetValue(sIndex, "OffDelay ", m_aOut[i].Para.iOffDelay);
                    Config2.SetValue(sIndex, "NotLog   ", m_aOut[i].Para.bNotLog  );
                }
                Config2.Save(sPath2,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
             
            }
            /*
            if (_bLoad)
            {
                CIniFile InputConfig = new CIniFile(m_sParaFolderPath + "Input.ini");
                
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    InputConfig.Load(          "Address  ", sIndex, out m_aIn[i].Para.iAdd     );
                    InputConfig.Load(          "Invert   ", sIndex, out m_aIn[i].Para.bInv     );
                    //InputConfig.Load(          "Enum     ", sIndex, out m_aIn[i].Para.sEnum    );
                    InputConfig.Load(sSelLan + "Name     ", sIndex, out m_aIn[i].Para.sName    );
                    InputConfig.Load(sSelLan + "Comment  ", sIndex, out m_aIn[i].Para.SComt    );
                    InputConfig.Load(          "OnDelay  ", sIndex, out m_aIn[i].Para.iOnDelay );
                    InputConfig.Load(          "OffDelay ", sIndex, out m_aIn[i].Para.iOffDelay);
                    InputConfig.Load(          "NotLog   ", sIndex, out m_aIn[i].Para.bNotLog  );
                }

                CIniFile OutputConfig = new CIniFile(m_sParaFolderPath + "Output.ini");
                
                for (int i = 0; i < m_iMaxOut ; i++)
                {
                    sIndex = string.Format("Output({0:000})", i);
                    OutputConfig.Load(          "Address  ", sIndex, out m_aOut[i].Para.iAdd     );
                    OutputConfig.Load(          "Invert   ", sIndex, out m_aOut[i].Para.bInv     );
                    //OutputConfig.Load(          "Enum     ", sIndex, out m_aOut[i].Para.sEnum    );
                    OutputConfig.Load(sSelLan + "Name     ", sIndex, out m_aOut[i].Para.sName    );
                    OutputConfig.Load(sSelLan + "Comment  ", sIndex, out m_aOut[i].Para.SComt    );
                    OutputConfig.Load(          "OnDelay  ", sIndex, out m_aOut[i].Para.iOnDelay );
                    OutputConfig.Load(          "OffDelay ", sIndex, out m_aOut[i].Para.iOffDelay);
                    OutputConfig.Load(          "NotLog   ", sIndex, out m_aOut[i].Para.bNotLog  );
                }
            }
            else
            {
                CIniFile InputConfig = new CIniFile(m_sParaFolderPath + "Input.ini");
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    InputConfig.Save(          "Address  ", sIndex, m_aIn[i].Para.iAdd);
                    InputConfig.Save(          "Invert   ", sIndex, m_aIn[i].Para.bInv);
                    InputConfig.Save(          "Enum     ", sIndex, m_aIn[i].Para.sEnum);
                    InputConfig.Save(sSelLan + "Name     ", sIndex, m_aIn[i].Para.sName);
                    InputConfig.Save(sSelLan + "Comment  ", sIndex, m_aIn[i].Para.SComt);
                    InputConfig.Save(          "OnDelay  ", sIndex, m_aIn[i].Para.iOnDelay);
                    InputConfig.Save(          "OffDelay ", sIndex, m_aIn[i].Para.iOffDelay);
                    InputConfig.Save(          "NotLog   ", sIndex, m_aIn[i].Para.bNotLog  );
                }

                CIniFile OutputConfig = new CIniFile(m_sParaFolderPath + "Output.ini");
                for (int i = 0; i < m_iMaxOut ; i++)
                {
                    sIndex = string.Format("Output({0:000})", i);
                    OutputConfig.Save(          "Address  ", sIndex, m_aOut[i].Para.iAdd);
                    OutputConfig.Save(          "Invert   ", sIndex, m_aOut[i].Para.bInv);
                    OutputConfig.Save(          "Enum     ", sIndex, m_aOut[i].Para.sEnum);
                    OutputConfig.Save(sSelLan + "Name     ", sIndex, m_aOut[i].Para.sName);
                    OutputConfig.Save(sSelLan + "Comment  ", sIndex, m_aOut[i].Para.SComt);
                    OutputConfig.Save(          "OnDelay  ", sIndex, m_aOut[i].Para.iOnDelay);
                    OutputConfig.Save(          "OffDelay ", sIndex, m_aOut[i].Para.iOffDelay);
                    OutputConfig.Save(          "NotLog   ", sIndex, m_aOut[i].Para.bNotLog  );
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
