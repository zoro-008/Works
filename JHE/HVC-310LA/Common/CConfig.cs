﻿using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace COMMON
{
	public sealed class CConfig: CDisposable //이 클래스는 마지막 클래스이므로 더 이상 상속을 할 수 없다.
    {
        #region Disposable
        private bool m_bDisposed = false;

        protected override void Dispose(bool isDisposing)
        {
            if(m_bDisposed)return;

            if(isDisposing)
            {
                // TODO : Free managed resources here 
                Table.Dispose();
            }
            // TODO : Free unmanaged resources here 

            base.Dispose(isDisposing);
            m_bDisposed = true;
        }
        #endregion

        #region Declarations

        private DataTable Table; //this is the main Table
		private string m_sFileName = ""; //this is the filename that was loaded

		public enum EN_CONFIG_FILE_TYPE //this specifies if the file is an xml or an ini
		{
			ftIni, 
            ftXml
		}

		#endregion

		#region Public

		public CConfig() //creates the settings
		{
			initializeDataTable();
		}

		public void Load(string _sFile, EN_CONFIG_FILE_TYPE _eFileType = EN_CONFIG_FILE_TYPE.ftIni) //loads settings from a file (xml or ini)
		{
			m_sFileName = Path.GetFullPath(_sFile); //saves the filename for future use

			if (_eFileType == EN_CONFIG_FILE_TYPE.ftIni)
				LoadFromIni();
			else
				LoadFromXml();



            
		}

        public void SetValue(string _sSection, string _sKey, bool   _bValue, bool _bOverWrite = false) { SetValue(_sSection, _sKey, _bValue.ToString(), _bOverWrite);}
        public void SetValue(string _sSection, string _sKey, int    _iValue, bool _bOverWrite = false) { SetValue(_sSection, _sKey, _iValue.ToString(), _bOverWrite);}
        public void SetValue(string _sSection, string _sKey, uint   _iValue, bool _bOverWrite = false) { SetValue(_sSection, _sKey, _iValue.ToString(), _bOverWrite);}
        public void SetValue(string _sSection, string _sKey, float  _fValue, bool _bOverWrite = false) { SetValue(_sSection, _sKey, _fValue.ToString(), _bOverWrite);}
        public void SetValue(string _sSection, string _sKey, double _dValue, bool _bOverWrite = false) { SetValue(_sSection, _sKey, _dValue.ToString(), _bOverWrite);}
		public void SetValue(string _sSection, string _sKey, string _sValue, bool _bOverWrite = false) //adds a new setting to the table
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();
            _sValue   = _sValue.Trim();

            
            //foreach (DataRow row in Table.Rows.Cast<DataRow>().Where(row => (string)row[0] == _sSection && (string)row[1] == _sKey))
            //{
            //    row[2] = _sValue;
            //    return;
            //}

            //DataRow drRow = new DataRow();

            //drRow = Table.Rows.Add(_sSection, _sKey, _sValue);

            if (_bOverWrite)
            {
                foreach (DataRow row in Table.Rows.Cast<DataRow>().Where(row => (string) row[0] == _sSection && (string) row[1] == _sKey))
                {
                    row[2] = _sValue;
                    return;
                }

                Table.Rows.Add(_sSection, _sKey, _sValue);
            }
            else
                Table.Rows.Add(_sSection, _sKey, _sValue);
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

        //예외처리 일단 스킵.
        public bool GetValue(string _sSection, string _sKey, out bool   _bValue) { string _sValue; bool bRet = GetValue(_sSection, _sKey, out _sValue); _bValue = StrToBoolDef  (_sValue,false); return bRet; }
        public bool GetValue(string _sSection, string _sKey, out int    _iValue) { string _sValue; bool bRet = GetValue(_sSection, _sKey, out _sValue); _iValue = StrToIntDef   (_sValue,0    ); return bRet; }
        public bool GetValue(string _sSection, string _sKey, out uint   _iValue) { string _sValue; bool bRet = GetValue(_sSection, _sKey, out _sValue); _iValue = StrToUintDef  (_sValue,0    ); return bRet; }
        public bool GetValue(string _sSection, string _sKey, out float  _fValue) { string _sValue; bool bRet = GetValue(_sSection, _sKey, out _sValue); _fValue = StrToFloatDef (_sValue,0    ); return bRet; }
        public bool GetValue(string _sSection, string _sKey, out double _dValue) { string _sValue; bool bRet = GetValue(_sSection, _sKey, out _sValue); _dValue = StrToDoubleDef(_sValue,0.0  ); return bRet; }
		public bool GetValue(string _sSection, string _sKey, out string _sValue) //gets a value or returns a default value
		{
            _sSection = _sSection.Trim();
            _sKey     = _sKey.Trim();


			foreach (DataRow row in Table.Rows.Cast<DataRow>().Where(row => (string)row[0] == _sSection && (string)row[1] == _sKey))
			{
                _sValue = ((string)row[2]).Trim();
                return true; 
			}
            _sValue = "";
			return false;
		}

        public void Save(string file, EN_CONFIG_FILE_TYPE _ftFileType=EN_CONFIG_FILE_TYPE.ftIni) //saves the file to a file
        {
            m_sFileName = Path.GetFullPath(file); //saves the filename for future use
			//sorts the table for saving

            if (m_sFileName == "")
            {
                Log.ShowMessage("Error", "The file name was not defined");
            } 

			DataView dv = Table.DefaultView;
            dv.Sort = "Section asc";
			DataTable sortedDT = dv.ToTable();

			if (_ftFileType == EN_CONFIG_FILE_TYPE.ftXml)
				sortedDT.WriteXml(m_sFileName);
			else
			{
				StreamWriter sw = new StreamWriter(m_sFileName);

				string lastCategory ="";

				foreach (DataRow row in sortedDT.Rows)
				{
					if ((string) row[0] != lastCategory)
					{
						lastCategory = (string) row[0];
						sw.WriteLine("[" + lastCategory + "]");
					}

					sw.WriteLine((string) row[1] + "=" + (string)row[2]);
				}

				sw.Close();
			}
		}


		#endregion

		#region Private

        private bool MakeFilePathFolder(string _sFileName)
        {
            string sFileFolderPath = Path.GetDirectoryName(_sFileName);

            DirectoryInfo _diPath = new DirectoryInfo(sFileFolderPath);

            // 해당 경로에 해당 하는 폴더가 없으면 만들어 줌 
            if (!_diPath.Exists)
            {
                try
                {
                    _diPath.Create();
                }
                catch (Exception e)
                {
                    Log.ShowMessage("Exception", e.Message);
                    throw new FileNotFoundException(e.Message);
                }
                return true;
            }

            return true;
        }

		private void LoadFromIni() //loads settings from ini
		{
			if (!File.Exists(m_sFileName))return;

            Table.Clear();

			StreamReader sr = new StreamReader(m_sFileName); //stream reader that will read the settings

			string currentCategory = ""; //holds the category we're at

			while (!sr.EndOfStream) //goes through the file
			{
				string currentLine = sr.ReadLine(); //reads the current file

				if (currentLine.Length < 3) continue; //checks that the line is usable

				if (currentLine.StartsWith("[") && currentLine.EndsWith("]")) //checks if the line is a category marker
				{
					currentCategory = currentLine.Substring(1, currentLine.Length - 2);
					continue;
				}

				if (!currentLine.Contains("=")) continue; //or an actual setting

				string currentKey = currentLine.Substring(0, currentLine.IndexOf("=", StringComparison.Ordinal));

				string currentValue = currentLine.Substring(currentLine.IndexOf("=", StringComparison.Ordinal) + 1);

				SetValue(currentCategory, currentKey, currentValue, true);
			}

			sr.Close(); //closes the stream
		}

		private void LoadFromXml() //loads the settings from an xml file
		{
            Table.Clear();
			Table.ReadXml(m_sFileName);
		}

		private void initializeDataTable() //re-initializes the table with the proper columns
		{
			Table = new DataTable {TableName = "Settings"};

			Table.Columns.Add("Section", typeof(string));
			Table.Columns.Add("Key", typeof(string));
			Table.Columns.Add("Value", typeof(string));
		}

		#endregion

	}

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
            if (_sValue == null) _sValue = "";
            _sValue   = _sValue.Trim();

            long lRet = WritePrivateProfileString(_sSection, _sKey, _sValue, m_sFilePath);

            return (lRet > 0);
		}


        








    }

}


