using COMMON;
using MotionInterface;

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

        public bool Init(EN_LAN_SEL _eLanSel,string _sParaFolderPath,int _iMaxIn,int _iMaxOut,EN_DIO_SEL _eDioSel)
        {
            m_eLangSel = _eLanSel ;
            m_sParaFolderPath = _sParaFolderPath;
            m_aIn  = new TDIo[_iMaxIn];
            m_aOut = new TDIo[_iMaxOut];

            m_iMaxIn = _iMaxIn ;
            m_iMaxOut= _iMaxOut;


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
                //Check Delay.
                if(m_aOut[i].Stat.bVtrVal) 
                {
                    m_aOut[i].DelayOff.Clear();
                    if(m_aOut[i].Para.iOnDelay > 0) 
                    {
                        if(!m_aOut[i].DelayOn.OnDelay (m_aOut[i].Stat.bVtrVal != m_aOut[i].Stat.bAtrVal , m_aOut[i].Para.iOnDelay ))
                        {
                            continue ;
                        }
                    }
                }
                else 
                {
                    m_aOut[i].DelayOn.Clear();
                    if(m_aOut[i].Para.iOffDelay > 0)
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
                        Log.Trace(string.Format("OUTPUT({0:000}) {1} is {2} with Delay {3}", i, m_aOut[i].Para.sEnum, m_aOut[i].Stat.bVtrVal ? "On" : "Off", m_aOut[i].Stat.bVtrVal ? m_aOut[i].Para.iOnDelay : m_aOut[i].Para.iOffDelay),ti.Sts);
                    }
                }
                else {
                    m_aOut[i].Stat.bUpEdge = false ;
                    m_aOut[i].Stat.bDnEdge = false ;
                }
        
                //Set Output.
                Dio.SetOut(m_aOut[i].Para.iAdd , m_aOut[i].Stat.bVtrVal);
                m_aOut[i].Stat.bPreVal = m_aOut[i].Stat.bAtrVal;
                m_aOut[i].Stat.bAtrVal = Dio.GetOut(m_aOut[i].Para.iAdd);
                if (!m_aOut[i].Para.bNotLog && m_aOut[i].Stat.bPreVal != m_aOut[i].Stat.bAtrVal)
                {
                    Log.Trace(string.Format("OUTPUT({0:000}) {1} is {2} with Delay {3}", i, m_aOut[i].Para.sEnum, m_aOut[i].Stat.bAtrVal ? "On" : "Off", m_aOut[i].Stat.bAtrVal ? m_aOut[i].Para.iOnDelay : m_aOut[i].Para.iOffDelay), ti.Sts);
                }

            }
        
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
                        Log.Trace(string.Format("INPUT({0:000}) {1} is {2} with Delay {3}", i, m_aIn[i].Para.sEnum, m_aIn[i].Stat.bAtrVal ? "On" : "Off", m_aIn[i].Stat.bAtrVal ? m_aIn[i].Para.iOnDelay : m_aIn[i].Para.iOffDelay), ti.Sts);
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
            Log.Trace(string.Format("OUTPUT({0:000}) {1} is {2} (Direct)", _iNo, m_aOut[_iNo].Para.sEnum, _bVal ? "On" : "Off"), ti.Sts);
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
                Log.Trace(string.Format("OUTPUT({0:000}) {1} is {2} (Direct)",_iNo,m_aOut[_iNo].Para.sEnum , _bVal ? "On":"Off"), ti.Sts);
                Dio.SetOut(m_aOut[_iNo].Para.iAdd , _bVal);
            }
        }
        
        public bool GetY(int _iNo , bool _bDirect=false)
        {
            if( _iNo < 0 || _iNo >= m_iMaxOut) 
            {
                Log.ShowMessageFunc(string.Format("ERR OUT({0:000}) is not Between 0 and {1}",_iNo,m_iMaxOut));
                return false; 
            }

            if(_bDirect) return Dio.GetOut(m_aOut[_iNo].Para.iAdd);

            
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
        
            if(_bDirect) return Dio.GetIn(m_aIn[_iNo].Para.iAdd);

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
            string sSelLan;
            string sIndex;

            switch (m_eLangSel)
            {
                default: sSelLan = "E_"; break;
                case EN_LAN_SEL.English: sSelLan = "E_"; break;
                case EN_LAN_SEL.Korean : sSelLan = "K_"; break;
                case EN_LAN_SEL.Chinese: sSelLan = "C_"; break;
            }

            if (_bLoad)
            {
                CIniFile InputConfig = new CIniFile(m_sParaFolderPath + "Input.ini");
                
                for (int i = 0; i < m_iMaxIn ; i++)
                {
                    sIndex = string.Format("Input({0:000})", i);
                    InputConfig.Load(          "Address  ", sIndex, out m_aIn[i].Para.iAdd     );
                    InputConfig.Load(          "Invert   ", sIndex, out m_aIn[i].Para.bInv     );
                    InputConfig.Load(          "Enum     ", sIndex, out m_aIn[i].Para.sEnum    );
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
                    OutputConfig.Load(          "Enum     ", sIndex, out m_aOut[i].Para.sEnum    );
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
