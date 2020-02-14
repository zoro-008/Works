using COMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDll.Pakage;

namespace VDll
{
    public class VL
    {
        //GV에 있어야 하나.. 여기에 있어야 하나..
        //sun sdfgfdg
        public struct TPara
        {   
            public string   UtilFolder   ;
            public string   DeviceFolder ;
        
            public int      VisionCnt    ; 
            public string[] VisionNames  ;
        }
        //테스트트트

        public struct TUtil
        {
            public int             CameraCnt   ; 
            public string[]        CameraNames ; 
            public ECameraType[]   CameraTypes ;
            
            public int             LightCnt    ; 
            public string[]        LightNames  ; 
            public ELightType[]    LightTypes  ;
        }
        static TUtil Util;

        static public FormRecipe.dgLoadRecipe LoadRecipe;

        public static bool Init(TPara _tVisnPara)
        {
            //현재 프로젝트에 있는 PKG 리스트 만들어 놓기.
            GV.DynamicPkgMaker = new CDynamic<IPkg>();

            GV.Para = _tVisnPara ;

            //하드웨어들 구성 갯수 및 이름... 로딩.
            if(!LoadUtil(GV.Para.UtilFolder)) {
                MessageBox.Show("Vision Util File Loading Failed!");
                return false ;
            }
   
            //잡파일 패스 생성.
            if(!CIniFile.MakeFilePathFolder(_tVisnPara.DeviceFolder)) {
                MessageBox.Show("Vision Device Folder Making Failed!");
                return false ;
            }

            //카메라 갯수 만큼 생성.
            GV.Cameras = new Camera.ICamera[Util.CameraCnt];
            for(int i = 0 ; i< Util.CameraCnt ; i++){
                if(Util.CameraTypes[i] == ECameraType.Neptune) GV.Cameras[i] = new Camera.CNeptune(Util.CameraNames[i]);
                else {
                    MessageBox.Show("Undefined Camera Type!" , "Camera"+i.ToString());
                    return false ;
                }

                bool bRet = false ;
                
                try
                {
                    bRet = !GV.Cameras[i].Init() ;
                }
                catch(Exception _e)
                {
                    MessageBox.Show(_e.ToString() , "Camera"+i.ToString());
                }

                if(bRet)
                {
                    MessageBox.Show("Camera Init Failed!" , "Camera"+i.ToString());
                    //sun 나중에 고민해 보자꾸나.
                    //return false ;
                }
            }

            //조명 갯수 만큼 생성.
            GV.Lights = new Light.ILight [Util.LightCnt];
            for(int i = 0 ; i< Util.LightCnt ; i++){
                if(Util.LightTypes[i] == ELightType.Deakyum) GV.Lights[i] = new Light.CDeakyum(Util.LightNames[i]);
                else {
                    MessageBox.Show("Undefined Light Type!" , "Light"+i.ToString());
                    return false ;
                }

                if(GV.Lights[i].Init())
                {
                    MessageBox.Show("Light Init Failed!" , "Light"+i.ToString());
                    //sun 나중에 고민해 보자꾸나.
                    //return false ;
                }
            }
                        
            //전역변수 테이블 1개...전비전 이용테이블.
            GV.GlobalTables   = new CVarTable("VAR_Global");

            //각 비전별 변수 테이블 생성.
            GV.VarTables      = new CVarTable[GV.Para.VisionCnt];
            GV.PaintCallbacks = new GV.FPaintCallback[GV.Para.VisionCnt];
            GV.ImageLock      = new object[GV.Para.VisionCnt];
            
            GV.Visions        = new Vision[GV.Para.VisionCnt];
            //GV.VisnStats      = new TVisnStat[GV.Para.VisionCnt];
            GV.calibration    = new Calibration[GV.Para.VisionCnt];

            for(int i = 0 ; i< GV.Para.VisionCnt ; i++){
                GV.VarTables  [i] = new CVarTable("VAR_"+GV.Para.VisionNames[i]);
                GV.Visions    [i] = new Vision(GV.Para.VisionNames[i] , i);
                GV.ImageLock  [i] = new object();
                GV.Visions    [i].Init();
                GV.VarTables  [i].Load();
                GV.calibration[i] = new Calibration(i);
            }
            LoadRecipe = LoadDevice;
            
            if(!LoadSaveLastDeviceName(true , GV.Para.UtilFolder)) 
            {
                MessageBox.Show("Load LastDeviceName Failed!" , GV.Para.UtilFolder);
                //sun 나중에 고민해 보자꾸나.
                //return false ;
            }


            if(!LoadDevice(GV.DeviceName)) 
            {
                MessageBox.Show("Load Device Failed!" , GV.DeviceName);
                //sun 나중에 고민해 보자꾸나.
                //return false ;
            }

            //요안에 Camera접근하는 것이 있어 LoadDevice뒤에 있어야함.
            GV.FrmVisions = new FormVision[GV.Para.VisionCnt];
            for(int i = 0 ; i< GV.Para.VisionCnt ; i++){
                GV.FrmVisions[i] = new FormVision(i);
            }

            return true ;
        }
        public static string RecipeName(string _RecipeName = "")
        {
            if(_RecipeName != "") GV.DeviceName = _RecipeName;
            return GV.DeviceName;
        }

        public static bool SetVisonForm(int _iVisnID , ref Panel _pnBase)
        {
            if(_iVisnID <  0                   ) { MessageBox.Show("Vision ID is less than 0");    return false ; } 
            if(_iVisnID >= GV.FrmVisions.Length) { MessageBox.Show("Vision ID is over than "+ (GV.FrmVisions.Length -1).ToString());   return false ; } 

            GV.FrmVisions[_iVisnID].TopLevel = false ;
            GV.FrmVisions[_iVisnID].Parent   = _pnBase ;
            GV.FrmVisions[_iVisnID].Show();
            GV.FrmVisions[_iVisnID].Dock = DockStyle.Fill ;
            return true ;
        }

        public static bool LoadDevice(string _sDeviceName)
        {
            bool bRet = true ;
            LoadSaveLastDeviceName(false , GV.Para.UtilFolder);
            for(int i = 0 ; i< GV.Para.VisionCnt ; i++)
            {
                if(!GV.Visions[i].LoadSave(true , GV.Para.DeviceFolder + GV.DeviceName + "\\Vision"+i.ToString())) bRet = false ;
            }
            return bRet ;
        }

        public static void Autorun(bool _bOn)
        {
            GV.AutoRun = _bOn ;
            for(int i = 0 ; i< GV.Para.VisionCnt ; i++)
            {
                GV.Visions[i].Autorun(_bOn);
            }
        }

        public static void Ready(int _iVisnID)
        {
            GV.Visions[_iVisnID].Ready();

        }

        public static bool GetTraining(int _iVisnID)
        {
            return GV.Visions[_iVisnID].VisnStats.Trainings;
        }



        public static void Close()
        {
            //각 비전별 폼 생성.
            for(int i = 0 ; i< GV.Para.VisionCnt ; i++){
                GV.FrmVisions[i].Close();
            }

            for(int i = 0 ; i< Util.CameraCnt ; i++){
                GV.Cameras[i].Close();
            }

            for(int i = 0 ; i< Util.LightCnt ; i++){
                GV.Lights[i].Close();
            }

            //비전갯수만큼 생성.
            for(int i = 0 ; i< GV.Para.VisionCnt ; i++){
                GV.Visions[i].Close();
            }

            //각 비전별 변수 테이블 생성.   
            for(int i = 0 ; i< GV.Para.VisionCnt ; i++){
                //GV.VarTables[i] = new CVarTable();
            }
            
            //전역변수 테이블.
            //GV.GlobalTables. = new CVarTable();
        }

        static bool LoadUtil(string _sVisnSettingFolder , bool _bResave = true)
        {
            string sPropFilePath = _sVisnSettingFolder + "\\Util.ini";
            CIniFile Ini = new CIniFile(sPropFilePath) ;
            Ini.Load("Count" , "CameraCnt" , out Util.CameraCnt); if(Util.CameraCnt==0)Util.CameraCnt=1;
            Ini.Load("Count" , "LightCnt"  , out Util.LightCnt ); 
            uint uiTemp = 0 ;

            //조명들..
            Util.LightNames = new string     [Util.LightCnt];
            Util.LightTypes = new ELightType [Util.LightCnt];
            for(int i = 0 ; i < Util.LightCnt ; i++) 
            {
                Ini.Load("Light"+i.ToString() , "LightName" , out Util.LightNames[i]);
                Ini.Load("Light"+i.ToString() , "LightType" , out uiTemp            ); Util.LightTypes[i] = (ELightType)uiTemp ;
            }
            
            //카메라들.
            Util.CameraNames = new string   [Util.CameraCnt];
            Util.CameraTypes = new ECameraType [Util.CameraCnt];
            for(int i = 0 ; i < Util.CameraCnt ; i++) 
            {
                Ini.Load("Camera"+i.ToString() , "CameraName" , out Util.CameraNames[i]);
                Ini.Load("Camera"+i.ToString() , "CameraType" , out uiTemp             ); Util.CameraTypes[i] = (ECameraType)uiTemp ;
            }

            if(_bResave) {
                Ini.Save("Count" , "CameraCnt" , Util.CameraCnt);
                Ini.Save("Count" , "LightCnt"  , Util.LightCnt );

                for(int i = 0 ; i < Util.LightCnt ; i++) 
                {
                    Ini.Save("Light"+i.ToString() , "LightName" ,      Util.LightNames[i]);
                    Ini.Save("Light"+i.ToString() , "LightType" , (int)Util.LightTypes[i]);
                }
                for(int i = 0 ; i < Util.CameraCnt ; i++) 
                {
                    Ini.Save("Camera"+i.ToString() , "CameraName" ,      Util.CameraNames[i]);
                    Ini.Save("Camera"+i.ToString() , "CameraType" , (int)Util.CameraTypes[i]);
                }
            }
            return true ;
        }

        static bool LoadSaveLastDeviceName(bool _bLoad , string _sVisnSettingFolder)
        {
            string sPropFilePath = _sVisnSettingFolder + "\\LastDevice.ini";
            CIniFile Ini = new CIniFile(sPropFilePath) ;

            if(_bLoad) Ini.Load ("LastDevice" , "Name" , out GV.DeviceName); 
            else       Ini.Save ("LastDevice" , "Name" ,     GV.DeviceName); 

            return true ;
        }

        //public static ECameraType EN_CAM_SEL { get; set; }
    }
}
