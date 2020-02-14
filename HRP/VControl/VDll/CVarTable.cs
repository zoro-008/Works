using COMMON;
using Emgu.CV;
using SplitAndMerge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDll
{
    class CVarTable
    {
        Dictionary<string,CValue> dtList ;
        List<string> ltName ;
        public  string  FileName      {get{return sFileName;} set{sFileName = value;}}
        private string sFileName;
        public CVarTable(string _sName)
        {
            dtList = new Dictionary<string,CValue>();
            sFileName = _sName ;
            Load();
        }

        //~CVarTable()
        //{
        //    Save();
        //}

        public bool AddValue(string _sItem, CValue _Value)
        {
            if(dtList.ContainsKey(_sItem)) return false;
            dtList[_sItem] = _Value;
            return true;
        }

        public void Clear()
        {
            dtList.Clear();
        }

        public void RemoveValue(string _sItem)
        {
            dtList.Remove(_sItem);
        }

        public int GetCount()
        {
            return dtList.Count;
        }

        public string GetName(int _iItem)
        {
            string sName = "";
            ltName = dtList.Keys.ToList<string>();
            if(ltName.Count > _iItem) sName = ltName[_iItem] ;

            return sName;
        }

        public CValue GetValue(string _sItem)
        {
            CValue Value;
            if(dtList.TryGetValue(_sItem,out Value))
            {
                return Value;
            }
            return null;
        }

        public void RenameValue(string _fromName, string _toName)
        {
            CValue value = dtList[_fromName];
            dtList.Remove(_fromName);
            dtList[_toName] = value;
        }

        public void SetValue(string _fromKey, CValue _Value)
        {
            dtList[_fromKey] = _Value;
        }

        public bool ValueExists(string _sItem)
        {
            return dtList.ContainsKey(_sItem);
        }

        public Type GetType(string _sType)
        {
            Type tType = null;
            string sText = _sType;
            switch(sText)
            {
                default       : tType = typeof(string); break;
                case "String" : tType = typeof(string); break;
                case "Double" : tType = typeof(double); break;
                case "Int32"  : tType = typeof(int   ); break;
                case "Uint32" : tType = typeof(uint  ); break;
                case "Boolean": tType = typeof(bool  ); break;
                case "Image"  : tType = typeof(Mat   ); break;
            }
            return tType;
        }

        #region LoadSave
        public bool Load(string _sPath = "")
        {
            CConfig Config = new CConfig(); 

            string sFullPath ;
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;

            if(_sPath == "") sFullPath  = GV.Para.UtilFolder + "\\" + sFileName + ".ini";
            else             sFullPath  = sExeFolder + _sPath;

            //Clear Dictionary
            Clear();

            //CValue
            int iIdx = 0;
            string sName,sComment,sType,sValue;
            double dMin,dMax;

            Config.Load(sFullPath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            Config.GetValue("Index", "Count", out iIdx);
            for(int i=0; i<iIdx; i++)
            {
                Config.GetValue(i.ToString(), "Name"   , out sName           );
                Config.GetValue(i.ToString(), "Comment", out sComment        );
                Config.GetValue(i.ToString(), "Min    ", out dMin            );
                Config.GetValue(i.ToString(), "Max    ", out dMax            );
                Config.GetValue(i.ToString(), "Type   ", out sType           );
                Config.GetValue(i.ToString(), "Value  ", out sValue          );
                AddValue(sName, new CValue(sName,sComment,dMin,dMax,GetType(sType)));
            }

            return true;
        }


        public bool Save(string _sPath = "")
        {
            CConfig Config = new CConfig(); 

            string sFullPath ;
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            
            if(_sPath == "") sFullPath  = GV.Para.UtilFolder + "\\" + sFileName + ".ini";
            else             sFullPath  = sExeFolder + _sPath;

            if (File.Exists(sFullPath)) File.Delete(sFullPath);

            int iIdx = 0;
            Config.SetValue("Index", "Count", dtList.Count);
            foreach(KeyValuePair<string,CValue> items in dtList)
            {
                Config.SetValue(iIdx.ToString(), "Name"   , items.Value.Name           );
                Config.SetValue(iIdx.ToString(), "Comment", items.Value.Comment        );
                Config.SetValue(iIdx.ToString(), "Min    ", items.Value.Min            );
                Config.SetValue(iIdx.ToString(), "Max    ", items.Value.Max            );
                Config.SetValue(iIdx.ToString(), "Type   ", items.Value.Type.ToString());
                Config.SetValue(iIdx.ToString(), "Value  ", items.Value.Value          );
                iIdx++;
            }
            Config.Save(sFullPath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            return true;
        }
        #endregion
    }

    class CValue
    {
        public CValue(//int    _iNo      ,
                      string _sName    = ""  ,
                      string _sComment = ""  ,
                      double _dMin     = 0   ,
                      double _dMax     = 0   ,
                      Type   _tType    = null,
                      string _sValue   = ""  ,
                      Mat    _Mat      = null)
        {
            //iNo      = _iNo      ;
            sName    = _sName    ;
            sComment = _sComment ; 
            dMin     = _dMin     ;
            dMax     = _dMax     ;
            tType    = _tType    ;
            sValue   = _sValue   ;
            iMat     = _Mat      ;
        }
          
        //private int    iNo      ;
        private string sName    ;
        private string sComment ;
        private double dMin     ;
        private double dMax     ;
        private Type   tType    ;
        private string sValue   ;
        private Mat    iMat     ;

        //public int    No        {get{return iNo     ;} set{iNo      = value;}}  
        public string Name      {get{return sName   ;} set{sName    = value;}}
        public string Comment   {get{return sComment;} set{sComment = value;}}
        public double Min       {get{return dMin    ;} set{dMin     = value;}}
        public double Max       {get{return dMax    ;} set{dMax     = value;}}
        public Type   Type      {get{return tType   ;} set{tType    = value;}}
        public string Value     {get{return sValue  ;} set{sValue   = value;}} 
        public Mat    Mat       {get{return iMat    ;} set{iMat     = value;}} 
        

    }


}
