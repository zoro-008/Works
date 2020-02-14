using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace COMMON
{
    public class CIniFile
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int Size, String filePat);

        [DllImport("Kernel32.dll")]
        private static extern long WritePrivateProfileString(String Section, String Key, String val, String filePath);

        string m_sFilePath ;

        /// <summary>
        /// 해당 _sFileName으로 입력 받은 루트의 폴더생성.
        /// 다중루트 이면 모두 생성한다. 폴더만.
        /// </summary>
        /// <param name="_sFileName">생성하고자 하는 풀루트</param>
        /// <returns>생성 성공여부</returns>
        public static bool MakeFilePathFolder(string _sFileName)
        {
            string sFileFolderPath = Path.GetDirectoryName(_sFileName);
            DirectoryInfo _diPath = new DirectoryInfo(sFileFolderPath);
            // 해당 경로에 해당 하는 폴더가 없으면 만들어 줌 
            if(!_diPath.Exists)
            {
                try
                {
                    _diPath.Create();
                }
                catch(Exception e)
                {

                    Log.ShowMessage("Exception",e.Message);
                    throw new FileNotFoundException(e.Message);

                }
                return true;
            }

            return true;
        }

        
        public CIniFile(string _sFilePath) 
        {
            m_sFilePath = _sFilePath.Trim() ;
            MakeFilePathFolder(m_sFilePath);
        }

        public static bool StrToBoolDef(string _sVal,bool _bDef)
        {
            bool bRet;
            if(bool.TryParse(_sVal,out bRet)) return bRet;
            return _bDef;
        }
        public static int StrToIntDef(string _sVal , int _iDef )
        {
            int iRet;
            if(int.TryParse(_sVal,out iRet))return iRet;
            return _iDef;
        }
        public static uint StrToUintDef(string _sVal, uint _iDef)
        {
            uint iRet;
            if (uint.TryParse(_sVal, out iRet)) return iRet;
            return _iDef;
        }
        public static float StrToFloatDef(string _sVal,float _fDef)
        {
            float fRet;
            if(float.TryParse(_sVal,out fRet)) return fRet;
            return _fDef;
        }
        public static double StrToDoubleDef(string _sVal,double _dDef)
        {
            double dRet;
            if(double.TryParse(_sVal,out dRet)) return dRet;
            return _dDef;
        }

        public bool Load(string _sSection, string _sKey, ref bool   _bValue) { string _sValue=""; bool bRet = Load(_sSection, _sKey, ref _sValue); _bValue = StrToBoolDef  (_sValue,false); return bRet; }
        public bool Load(string _sSection, string _sKey, ref int    _iValue) { string _sValue=""; bool bRet = Load(_sSection, _sKey, ref _sValue); _iValue = StrToIntDef   (_sValue,0    ); return bRet; }
        public bool Load(string _sSection, string _sKey, ref uint   _iValue) { string _sValue=""; bool bRet = Load(_sSection, _sKey, ref _sValue); _iValue = StrToUintDef  (_sValue,0    ); return bRet; }
        public bool Load(string _sSection, string _sKey, ref float  _fValue) { string _sValue=""; bool bRet = Load(_sSection, _sKey, ref _sValue); _fValue = StrToFloatDef (_sValue,0    ); return bRet; }
        public bool Load(string _sSection, string _sKey, ref double _dValue) { string _sValue=""; bool bRet = Load(_sSection, _sKey, ref _sValue); _dValue = StrToDoubleDef(_sValue,0.0  ); return bRet; }
        public bool Load(string _sSection, string _sKey, ref string _sValue) //gets a value or returns a default value
		{
            if(_sSection != null)_sSection = _sSection.Trim();
            if(_sKey     != null) _sKey     = _sKey.Trim();

             StringBuilder sbTemp = new StringBuilder(1000);
            int iRet = GetPrivateProfileString(_sSection, _sKey, "", sbTemp, 1000, m_sFilePath);
            _sValue = sbTemp.ToString();
            return (iRet > 0);
		}
        public bool Load(string _sSection, string _sKey, ref bool[] _bValue)
        {
            string sValue = "";
            bool bRet = true ;

            for(int i = 0 ; i < _bValue.Length ; i++)
            {
                if(!Load(_sSection, _sKey +"["+i.ToString()+"]", ref sValue)) bRet = false; 
                _bValue[i] = StrToBoolDef  (sValue,false); 
            }
            return bRet;
        }
        public bool Load(string _sSection, string _sKey, ref int[] _iValue)
        {
            string sValue = "";
            bool bRet = true ;
            for(int i = 0 ; i < _iValue.Length ; i++)
            {
                if(!Load(_sSection, _sKey +"["+i.ToString()+"]", ref sValue)) bRet = false ;
                _iValue[i] = StrToIntDef  (sValue,0); 
            }
            return bRet;
        }
        public bool Load(string _sSection, string _sKey, ref uint[] _uiValue)
        {
            string sValue = "";
            bool bRet = true ;
            for(int i = 0 ; i < _uiValue.Length ; i++)
            {
                if(!Load(_sSection, _sKey +"["+i.ToString()+"]", ref  sValue)) bRet = false ;
                _uiValue[i] = StrToUintDef  (sValue,0); 
            }
            return bRet;
        }
        public bool Load(string _sSection, string _sKey, ref float[] _fValue)
        {
            string sValue = "";
            bool bRet = true ;
            for(int i = 0 ; i < _fValue.Length ; i++)
            {
                if(!Load(_sSection, _sKey +"["+i.ToString()+"]", ref sValue)) bRet = false ;
                _fValue[i] = StrToUintDef  (sValue,0); 
            }
            return bRet;
        }
        public bool Load(string _sSection, string _sKey, ref double[] _dValue)
        {
            string sValue = "";
            bool bRet = true ;
            for(int i = 0 ; i < _dValue.Length ; i++)
            {
                if(!Load(_sSection, _sKey +"["+i.ToString()+"]", ref sValue)) bRet = false ;
                _dValue[i] = StrToUintDef  (sValue,0); 
            }
            return bRet;
        }
        public bool Load(string _sSection, string _sKey, ref string[] _sValue)
        {
            string sValue = "";
            bool bRet = true ;
            for(int i = 0 ; i < _sValue.Length ; i++)
            {
                if(!Load(_sSection, _sKey +"["+i.ToString()+"]", ref sValue)) bRet = false ;
                _sValue[i] = sValue; 
            }
            return bRet;
        }

        public bool Save(string _sSection, string _sKey, bool   _bValue) { bool bRet = Save(_sSection, _sKey, _bValue.ToString()); return bRet; }
        public bool Save(string _sSection, string _sKey, int    _iValue) { bool bRet = Save(_sSection, _sKey, _iValue.ToString()); return bRet; }
        public bool Save(string _sSection, string _sKey, uint   _iValue) { bool bRet = Save(_sSection, _sKey, _iValue.ToString()); return bRet; }
        public bool Save(string _sSection, string _sKey, float  _fValue) { bool bRet = Save(_sSection, _sKey, _fValue.ToString()); return bRet; }
        public bool Save(string _sSection, string _sKey, double _dValue) { bool bRet = Save(_sSection, _sKey, _dValue.ToString()); return bRet; }
		public bool Save(string _sSection, string _sKey, string _sValue) //gets a value or returns a default value
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();
            if (_sValue == null) _sValue = "NULL";
            _sValue   = _sValue.Trim();

            long lRet = WritePrivateProfileString(_sSection, _sKey, _sValue, m_sFilePath);

            return (lRet > 0);
		}

        public bool Save(string _sSection, string _sKey, bool[] _bValue) //gets a value or returns a default value
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();
            bool bRet = true ;
            for(int i = 0 ; i < _bValue.Length ; i++)
            {
                if(!Save(_sSection, _sKey +"["+i.ToString()+"]", _bValue[i])) bRet = false ;
            }
            return bRet ;
        }

        public bool Save(string _sSection, string _sKey, int[] _iValue) //gets a value or returns a default value
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();
            bool bRet = true ;
            for(int i = 0 ; i < _iValue.Length ; i++)
            {
                if(!Save(_sSection, _sKey +"["+i.ToString()+"]", _iValue[i])) bRet = false ;
            }
            return bRet ;
        }

        public bool Save(string _sSection, string _sKey, uint[] _uiValue) //gets a value or returns a default value
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();
            bool bRet = true ;
            for(int i = 0 ; i < _uiValue.Length ; i++)
            {
                if(!Save(_sSection, _sKey +"["+i.ToString()+"]", _uiValue[i])) bRet = false ;
            }
            return bRet ;
        }

        public bool Save(string _sSection, string _sKey, float[] _fValue) //gets a value or returns a default value
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();
            bool bRet = true ;
            for(int i = 0 ; i < _fValue.Length ; i++)
            {
                if(!Save(_sSection, _sKey +"["+i.ToString()+"]", _fValue[i])) bRet = false ;
            }
            return bRet ;
        }

        public bool Save(string _sSection, string _sKey, double[] _dValue) //gets a value or returns a default value
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();
            bool bRet = true ;
            for(int i = 0 ; i < _dValue.Length ; i++)
            {
                if(!Save(_sSection, _sKey +"["+i.ToString()+"]", _dValue[i])) bRet = false ;
            }
            return bRet ;
        }

        public bool Save(string _sSection, string _sKey, string[] _sValue) //gets a value or returns a default value
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();
            bool bRet = true ;
            for(int i = 0 ; i < _sValue.Length ; i++)
            {
                if(!Save(_sSection, _sKey +"["+i.ToString()+"]", _sValue[i])) bRet = false ;
            }
            return bRet ;
        }



        //Ini파일에서 섹션 찾아 지우는 함수
        public void DeleteSection(String strSection)
        {
            WritePrivateProfileString(strSection, null, null, m_sFilePath);
        }
    }

    public class CAutoIniFile
    {
        public static bool LoadStruct<T>(string _sFilePath, string _sSection , ref T _oStruct)
        {
            CIniFile Ini = new CIniFile(_sFilePath) ;
            
            Type type = _oStruct.GetType();

            string sKey     ;
            string sValue  = "" ;
            int    iValue  = 0 ;
            bool   bValue  = false ;
            double dValue  = 0 ;
            Type   tType    ;

            BindingFlags Flags = BindingFlags.Public    | 
                                 BindingFlags.NonPublic | 
                                 BindingFlags.Instance  ;
            FieldInfo[] f = type.GetFields(Flags);
            for(int i = 0 ; i < f.Length ; i++){

                sKey = f[i].Name ;
                tType = f[i].FieldType;
                
                     if(tType == typeof(bool  )){Ini.Load(_sSection , sKey , ref bValue); f[i].SetValueDirect(__makeref(_oStruct), bValue);}
                else if(tType == typeof(int   )){Ini.Load(_sSection , sKey , ref iValue); f[i].SetValueDirect(__makeref(_oStruct), iValue);}
                else if(tType == typeof(double)){Ini.Load(_sSection , sKey , ref dValue); f[i].SetValueDirect(__makeref(_oStruct), dValue);}
                else if(tType == typeof(string)){Ini.Load(_sSection , sKey , ref sValue); f[i].SetValueDirect(__makeref(_oStruct), sValue);}

                else if(tType == typeof(bool[]))
                {
                    bool [] Values = (bool[])f[i].GetValue(_oStruct);
                    Ini.Load(_sSection , sKey , ref Values); f[i].SetValueDirect(__makeref(_oStruct), Values);
                }
                else if(tType == typeof(int[]))
                {
                    int [] Values = (int[])f[i].GetValue(_oStruct);
                    Ini.Load(_sSection , sKey , ref Values); f[i].SetValueDirect(__makeref(_oStruct), Values);
                }
                else if(tType == typeof(double[]))
                {
                    double [] Values = (double[])f[i].GetValue(_oStruct);
                    Ini.Load(_sSection , sKey , ref Values); f[i].SetValueDirect(__makeref(_oStruct), Values);
                }
                else if(tType == typeof(string[]))
                {
                    string [] Values = (string[])f[i].GetValue(_oStruct);//new string [tType.GetFields(Flags).Length];
                    Ini.Load(_sSection , sKey , ref Values); f[i].SetValueDirect(__makeref(_oStruct), Values);
                }

                
            }
            return true ; 
        }

        public static bool SaveStruct<T>(string _sFilePath, string _sSection , ref T _oStruct)
        {
            CIniFile Ini = new CIniFile(_sFilePath) ;

            Type type = _oStruct.GetType();

            string sKey     ;
            string sValue   ;
            bool   bRet = true ;
            Type   tType  ;

            FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for(int i = 0 ; i < f.Length ; i++){
                sKey   = f[i].Name ;
                tType  = f[i].FieldType;
                //sValue = ;  
                //if(!Ini.Save(_sSection , sKey , sValue)) bRet = false ;

                     if(tType == typeof(bool  )){Ini.Save(_sSection , sKey , f[i].GetValue(_oStruct).ToString());}
                else if(tType == typeof(int   )){Ini.Save(_sSection , sKey , f[i].GetValue(_oStruct).ToString());}
                else if(tType == typeof(double)){Ini.Save(_sSection , sKey , f[i].GetValue(_oStruct).ToString());}
                else if(tType == typeof(string)){Ini.Save(_sSection , sKey , f[i].GetValue(_oStruct).ToString());}
                else if(tType == typeof(bool[]))
                {
                    bool [] Values = (bool[])f[i].GetValue(_oStruct);
                    Ini.Save(_sSection , sKey , Values); 
                }
                else if(tType == typeof(int[]))
                {
                    int [] Values = (int[])f[i].GetValue(_oStruct);
                    Ini.Save(_sSection , sKey , Values);
                }
                else if(tType == typeof(double[]))
                {
                    double [] Values = (double[])f[i].GetValue(_oStruct);
                    Ini.Save(_sSection , sKey , Values); 
                }
                else if(tType == typeof(string[]))
                {
                    string [] Values = (string[])f[i].GetValue(_oStruct);//new string [tType.GetFields(Flags).Length];
                    Ini.Save(_sSection , sKey , Values);
                }
            }

            return bRet ;
        }
    }
}
