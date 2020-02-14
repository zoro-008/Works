using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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

        public bool Load(string _sSection, string _sKey, out bool   _bValue) { string _sValue; bool bRet = Load(_sSection, _sKey, out _sValue); _bValue = StrToBoolDef  (_sValue,false); return bRet; }
        public bool Load(string _sSection, string _sKey, out int    _iValue) { string _sValue; bool bRet = Load(_sSection, _sKey, out _sValue); _iValue = StrToIntDef   (_sValue,0    ); return bRet; }
        public bool Load(string _sSection, string _sKey, out uint   _iValue) { string _sValue; bool bRet = Load(_sSection, _sKey, out _sValue); _iValue = StrToUintDef  (_sValue,0    ); return bRet; }
        public bool Load(string _sSection, string _sKey, out float  _fValue) { string _sValue; bool bRet = Load(_sSection, _sKey, out _sValue); _fValue = StrToFloatDef (_sValue,0    ); return bRet; }
        public bool Load(string _sSection, string _sKey, out double _dValue) { string _sValue; bool bRet = Load(_sSection, _sKey, out _sValue); _dValue = StrToDoubleDef(_sValue,0.0  ); return bRet; }
		public bool Load(string _sSection, string _sKey, out string _sValue) //gets a value or returns a default value
		{
            if(_sSection != null)_sSection = _sSection.Trim();
            if(_sKey     != null) _sKey     = _sKey.Trim();

             StringBuilder sbTemp = new StringBuilder(1000);
            int iRet = GetPrivateProfileString(_sSection, _sKey, "", sbTemp, 1000, m_sFilePath);
            _sValue = sbTemp.ToString();
            return (iRet > 0);
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
            string sValue   ;

            FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
            for(int i = 0 ; i < f.Length ; i++){
                sKey = f[i].Name ;
                Ini.Load(_sSection , sKey , out sValue);
                     if(f[i].FieldType == typeof(bool  ))f[i].SetValueDirect(__makeref(_oStruct), CIniFile.StrToBoolDef  (sValue , false));
                else if(f[i].FieldType == typeof(int   ))f[i].SetValueDirect(__makeref(_oStruct), CIniFile.StrToIntDef   (sValue , 0    ));
                else if(f[i].FieldType == typeof(double))f[i].SetValueDirect(__makeref(_oStruct), CIniFile.StrToDoubleDef(sValue , 0.0  ));
                else if(f[i].FieldType == typeof(string))f[i].SetValueDirect(__makeref(_oStruct), sValue                                 );                
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

            FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
            for(int i = 0 ; i < f.Length ; i++){
                sKey   = f[i].Name ;
                sValue = f[i].GetValue(_oStruct).ToString();  
                if(!Ini.Save(_sSection , sKey , sValue)) bRet = false ;
            }

            return bRet ;
        }
    }
}
