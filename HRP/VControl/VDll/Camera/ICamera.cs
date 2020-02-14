using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDll.Camera
{
    public enum EPixelFormat
    {
        Gray   ,
        Rgb888 ,
        Bgr888 ,
        Yuv422 ,
    }

    public delegate bool GrabCallback(int _iWidth , int _iHeiht , int _iBit , EPixelFormat _ePxFormat , IntPtr _pImage );
    interface ICamera 
    {
        bool   Init();
        bool   Close();
        bool   Grab();
        void   SetGrabFunc(GrabCallback _pFunc);
        double GetGrabTime();
        bool   LoadSavePara(bool _bLoad , string _sParaFilePath);
        bool   ApplyPara(object _UserPara);
        bool   SetModeHwTrigger(bool _bOn);
        bool   ShowSettingDialog();
        string GetError();

        Type   GetParaType();
    }
}
