using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using COMMON;
using System.Threading;


namespace VDll.Camera
{

    class CImgLoader : CErrorObject 
    {
        [Serializable]
        public class CUPara
        {   
            [EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            [CategoryAttribute("UPara" ), DisplayNameAttribute("FolderPath" )] public string FolderPath {get;set;} 
        } 
        [Serializable]
        public class CPara
        {   
        //    [CategoryAttribute("Para" ), DisplayNameAttribute("PhysicalAdd" )]public int PhysicalAdd{get{return PhysicalAdd;}set{if(value < 0)value=0 ;  }}  
        } 

        
        IntPtr       m_hCamHandle = IntPtr.Zero;
        CPara        Para         = new CPara();
        CUPara       UParaSet     = new CUPara(); //최종으로 세팅된 유저파라 기억하고 있어야 함.
        GrabCallback CallBack     = null;
        Stopwatch    GrabTime     = new Stopwatch();
        
        public CImgLoader(string _sName) : base(_sName)
        {
        }

        public bool Init()
        {
            return true ;
        }

        public bool Close()
        {
            
            return true ;
        }
        ~CImgLoader()
        {

        }

        public bool Grab()
        {
            GrabTime.Start(); 
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadImage));
            return true ;
        }

        public void ClearImgIdx()
        {
            iImgIdx = 0 ;
        }
        int iImgIdx = 0 ;
        void LoadImage(object _oObj)
        {
            string [] sFileNames = Directory.GetFiles(UParaSet.FolderPath);
            if(sFileNames.Length == 0) {
                sError = "No Files in the FolderPath" ;
                return ;
            }
            
            if(iImgIdx >= sFileNames.Length) iImgIdx = 0 ;            
            try{
                using(Mat mtUnknwn = new Mat(sFileNames[iImgIdx] , ImreadModes.Unchanged))
                {
                    int iChanel = mtUnknwn.NumberOfChannels ;    
                    GrabTime.Stop();                  
                    if(CallBack != null) {
                             if(iChanel == 3)CallBack(mtUnknwn.Width, mtUnknwn.Height , iChanel * 8 , EPixelFormat.Bgr888 , mtUnknwn.DataPointer);        
                        else if(iChanel == 1)CallBack(mtUnknwn.Width, mtUnknwn.Height , iChanel * 8 , EPixelFormat.Gray   , mtUnknwn.DataPointer);     
                        else                 return  ;
                    }
                }
               
            }
            catch(Exception e){
                GrabTime.Stop(); 
                sError = "("+sFileNames[iImgIdx]+")"+ e.Message ;
                return ;
            }
            iImgIdx++;

        }

        public void SetGrabFunc(GrabCallback _pFunc)
        {
            CallBack = _pFunc ;
        }

        public double GetGrabTime()
        {
            return GrabTime.ElapsedMilliseconds ;
        }

        public bool LoadSavePara(bool _bLoad , string _sParaFilePath)
        {
            string sFilePath = _sParaFilePath ; // + "NeptunePara.xml";
            if (_bLoad)
            {
                if (!CXml.LoadXml(sFilePath, ref Para)) { return false; }
            }
            else
            {
                if (!CXml.SaveXml(sFilePath, ref Para)) { return false; }
            }

            return true ;
        }


        public bool ApplyPara(object _oUPara)
        {
            CUPara UserPara ; 
            try
            {
                UserPara = (CUPara)_oUPara ;
            }
            catch (InvalidCastException e)
            {
                sError = e.Message;
                return false ;
            }

            if(UserPara.FolderPath != UParaSet.FolderPath) {
                if(!CIniFile.MakeFilePathFolder(UserPara.FolderPath)){
                    sError = "Make("+UserPara.FolderPath+") Folder Failed!" ;
                    return false ;
                }
                UParaSet.FolderPath = UserPara.FolderPath ;
            }
            
            return true ;
        }

        public bool ShowSettingDialog()
        {

            return true ;

        }

        public string GetError()
        {
            return sError ;
        }

        public Type GetParaType()
        {
            return typeof(CUPara);
        }

    }
}
