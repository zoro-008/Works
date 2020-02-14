using System;
using System.IO;
using System.Reflection;

namespace COMMON
{
    public class CAutoConfig
    {
        public CAutoConfig() { }

        /// <summary>
        /// 클래스 인스턴스를 받아서 자동으로 ini파일로 세이브 해준다.
        /// 배열은 일단 1차배열까지만 지원되고
        /// 지원 자료형은 int,double,bool,string
        /// </summary>
        /// <typeparam name="T">클래스형</typeparam>
        /// <param name="_bLoad">true일경우 Load,false일경우 Save</param>
        /// <param name="_sFilePath">저장혹은 읽기를 할 ini파일의 FullPass</param>
        /// <param name="_oObj">저장 혹은 읽기를 할 클래스의 인스턴스</param>
        /// <returns>읽기의 경우 파일이 없으면 false</returns>
        public bool LoadSave<T>(bool _bLoad,string _sFilePath,ref T _oObj) //박싱 언박싱용. 이렇게 안하면 스트럭쳐는 안된다. Call By Value...
        {
            bool bRet;
            using(CConfig Config = new CConfig())
            {
                if(_bLoad)
                {
                    if(!File.Exists(_sFilePath)) return false;

                    Log.Trace("Load Obj Start",_sFilePath);
                    //파일에서 읽어서 테이블 만듬.
                    Config.Load(_sFilePath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
                    object obj = _oObj;
                    bRet = LoadObj(Config,_oObj);
                    _oObj = (T)obj;
                    Log.Trace("Load Obj End",_sFilePath);

                    return true;


                }
                else
                {
                    Log.Trace("Save Obj Start",_sFilePath);
                    object obj = _oObj;
                    bRet = SaveObj(Config,_oObj);
                    _oObj = (T)obj;
                    Config.Save(_sFilePath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
                    Log.Trace("Save Obj End",_sFilePath);
                }
            }
            return bRet;

        }

        private bool LoadObj(CConfig _cConfig,object _oObj)
        {
            Type ObjType = _oObj.GetType();// this.GetType();//.GetType();
            //PropertyInfo[] ObjPropInfo = ObjType.GetProperties();
            FieldInfo[] ObjFieldInfo = ObjType.GetFields();

            string sMemName;
            string sMemType;
            Type   MemType;

            string sValue;
            int    iTemp;
            double dTemp;
            string sTemp;
            bool   bTemp;
            foreach(FieldInfo MemInfo in ObjFieldInfo)
            {
                sMemName = MemInfo.Name;
                sMemType = MemInfo.FieldType.FullName;
                MemType = MemInfo.FieldType;

                

                if(MemType == typeof(int))
                {
                    _cConfig.GetValue(ObjType.Name,sMemName,out sValue);
                    //struct는 콜바잉벨류라 박싱 언박싱을 해줘서 콜바이 레퍼런스로 교체.
                    iTemp = CConfig.StrToIntDef(sValue,0);
                    MemInfo.SetValue(_oObj,iTemp);
                    Log.Trace(sMemName,sValue);

                }
                else if(MemType == typeof(bool))
                {
                    _cConfig.GetValue(ObjType.Name,sMemName,out sValue);
                    bTemp = CConfig.StrToBoolDef(sValue,false);
                    MemInfo.SetValue(_oObj,bTemp);
                    Log.Trace(sMemName,sValue);

                }
                else if(MemType == typeof(double))
                {
                    _cConfig.GetValue(ObjType.Name,sMemName,out sValue);
                    dTemp = CConfig.StrToDoubleDef(sValue,0.0);
                    MemInfo.SetValue(_oObj,dTemp);
                    Log.Trace(sMemName,sValue);

                }
                else if(MemType == typeof(string))
                {
                    _cConfig.GetValue(ObjType.Name,sMemName,out sValue);
                    sTemp = sValue;
                    MemInfo.SetValue(_oObj,sTemp);
                    Log.Trace(sMemName,sValue);
                }
                else if(MemType == typeof(int[]))
                {
                    int[] iAray = (int[])MemInfo.GetValue(_oObj);

                    string sIndex;
                    for(int i = 0;i < iAray.Length;i++)
                    {
                        sIndex = string.Format("{0}({1})",sMemName,i);
                        _cConfig.GetValue(ObjType.Name,sIndex,out sValue);
                        iTemp = CConfig.StrToIntDef(sValue,0);
                        iAray[i] = iTemp; 
                        
                        Log.Trace(sIndex,iAray[i].ToString());
                    }
                }
                else if(MemType == typeof(bool[]))
                {
                    bool[] bAray = (bool[])MemInfo.GetValue(_oObj);

                    string sIndex;
                    for(int i = 0;i < bAray.Length;i++)
                    {
                        sIndex = string.Format("{0}({1})",sMemName,i);
                        _cConfig.GetValue(ObjType.Name,sIndex,out sValue);
                        bTemp = CConfig.StrToBoolDef(sValue,false);
                        bAray[i] = bTemp;

                        Log.Trace(sIndex,bAray[i].ToString());
                    }
                }
                else if(MemType == typeof(double[]))
                {
                    double[] dAray = (double[])MemInfo.GetValue(_oObj);

                    string sIndex;
                    for(int i = 0;i < dAray.Length;i++)
                    {
                        sIndex = string.Format("{0}({1})",sMemName,i);
                        _cConfig.GetValue(ObjType.Name,sIndex,out sValue);
                        dTemp = CConfig.StrToDoubleDef(sValue,0);
                        dAray[i] = dTemp;

                        Log.Trace(sIndex,dAray[i].ToString());
                    }
                }
                else if(MemType == typeof(string[]))
                {
                    string[] sAray = (string[])MemInfo.GetValue(_oObj);

                    string sIndex;
                    for(int i = 0;i < sAray.Length;i++)
                    {
                        sIndex = string.Format("{0}({1})",sMemName,i);
                        _cConfig.GetValue(ObjType.Name,sIndex,out sValue);
                        //MemInfo.SetValue(_oObj,sValue);
                        sAray[i] = sValue;

                        Log.Trace(sIndex,sAray[i].ToString());
                    }
                }


                

            }


            return true;
        }

        private bool SaveObj(CConfig _cConfig,object _oObj)
        {

            Type ObjType = _oObj.GetType();
            FieldInfo[] ObjFieldInfo = ObjType.GetFields();

            string sMemName;
            string sMemType;
            Type   MemType;
            object oTemp;

            string sValue=string.Empty;
            foreach(FieldInfo MemInfo in ObjFieldInfo)
            {
                sMemName = MemInfo.Name;
                sMemType = MemInfo.FieldType.FullName;
                MemType  = MemInfo.FieldType;

                if(MemType == typeof(int)||
                   MemType == typeof(bool)||
                   MemType == typeof(double)||
                   MemType == typeof(string))
                {
                    oTemp = MemInfo.GetValue(_oObj);

                    if(oTemp!=null)
                    {
                        sValue = MemInfo.GetValue(_oObj).ToString();
                    }
                    else
                    {
                        sValue = "";
                    }

                    _cConfig.SetValue(ObjType.Name,sMemName,sValue);
                    Log.Trace(sMemName,sValue);

                }
                else if(MemType == typeof(int[]))
                {
                    int[] iAray = (int[])MemInfo.GetValue(_oObj);

                    string sIndex;
                    for(int i = 0;i < iAray.Length;i++)
                    {
                        sIndex = string.Format("{0}({1})",sMemName,i);
                        _cConfig.SetValue(ObjType.Name,sIndex,iAray[i]);
                        Log.Trace(sIndex,iAray[i].ToString());
                    }
                }
                else if(MemType == typeof(bool[]))
                {
                    bool[] bAray = (bool[])MemInfo.GetValue(_oObj);

                    string sIndex;
                    for(int i = 0;i < bAray.Length;i++)
                    {
                        sIndex = string.Format("{0}({1})",sMemName,i);
                        _cConfig.SetValue(ObjType.Name,sIndex,bAray[i]);
                        Log.Trace(sIndex,bAray[i].ToString());
                    }
                }
                else if(MemType == typeof(double[]))
                {
                    double[] dAray = (double[])MemInfo.GetValue(_oObj);

                    string sIndex;
                    for(int i = 0;i < dAray.Length;i++)
                    {
                        sIndex = string.Format("{0}({1})",sMemName,i);
                        _cConfig.SetValue(ObjType.Name,sIndex,dAray[i]);
                        Log.Trace(sIndex,dAray[i].ToString());
                    }
                }
                else if(MemType == typeof(string[]))
                {
                    string[] sAray = (string[])MemInfo.GetValue(_oObj);

                    string sIndex;
                    for(int i = 0;i < sAray.Length;i++)
                    {
                        sIndex = string.Format("{0}({1})",sMemName,i);
                        _cConfig.SetValue(ObjType.Name,sIndex,sAray[i]);
                        Log.Trace(sIndex,sAray[i].ToString());
                    }
                }

            }
            return true;
        }
    }
}
