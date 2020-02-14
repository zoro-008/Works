using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Machine
{
    public class CObject
    {
        protected string sName ;
        public string Name 
        {
            get{return sName ;}     
        }

        public CObject(string _sName)
        {
            sName = _sName ;
        }
    }

    public class CRegisteredObject : CObject
    {
        static Dictionary<string,CRegisteredObject> dtNameObjects = new Dictionary<string,CRegisteredObject>();
        static CRegisteredObject FindObject(string _sName)
        {
            return dtNameObjects[_sName];
        }
        public CRegisteredObject(string _sName) : base(_sName)
        {
            sName = _sName ;
            try
            {
                dtNameObjects.Add(sName , this);
            }
            catch(Exception _e)
            {
                MessageBox.Show(_e.ToString());
            }
        }
        ~CRegisteredObject()
        {
            dtNameObjects.Remove(sName);
        }
    }   

    public class CErrorObject : CObject
    {
        public CErrorObject(string _sName):base(_sName)
        {    
            
        }

        protected string sError ;
        public string Error 
        {
            get{string sRet = sError; sError = ""; return sRet ;}
        }
    }

    public enum EPixelFormat
    {
        Gray    ,
        Rgb888  ,
        Bgr888  ,
        Yuv422  ,
        BayerRG8 
    }

    public delegate bool GrabCallback(int _iWidth , int _iHeiht , int _iBit , EPixelFormat _ePxFormat , IntPtr _pImage );
    interface ICamera 
    {
        bool   Init();
        bool   Close();
        bool   Grab();
        void   SetGrabFunc(GrabCallback _pFunc);
        double GetGrabTime();
        double GetFrameFrq();
        bool   LoadSavePara(bool _bLoad , string _sParaFilePath);
        bool   ApplyPara(object _UserPara);
        bool   SetModeHwTrigger(bool _bOn);
        bool   ShowSettingDialog();
        string GetError();

        Type   GetParaType();
    }
}
