using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using Emgu.CV;
using COMMON;
using VDll.Camera;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using Emgu.CV.CvEnum;
using System.Reflection;

namespace VDll.Pakage
{
    
    public class PCamera : CPkgObject , IPkg
    {
        readonly TProp Prop = new TProp{bHaveTrain  = true  ,
                                        bHaveDialog = true  ,
                                        bHaveImage  = true  ,
                                        bHavePosRef = false };

        public enum EMode
        {
            Shape     = 0 ,
            Template      ,
            Search         
        }

        public class CMstPara 
        {
            [CategoryAttribute("MPara"), DisplayNameAttribute("Camera ID"        )] public uint   CameraID       {get;set;} 
            [CategoryAttribute("MPara"), DisplayNameAttribute("Light ID"         )] public uint   LightID        {get;set;} 

            [CategoryAttribute("MPara"), DisplayNameAttribute("Image Load"       )] public bool   ImageLoad      {get;set;} 
            [EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            [CategoryAttribute("MPara"), DisplayNameAttribute("Image Load Folder")] public string ImageLoadFolder    {get;set;} 

            [CategoryAttribute("MPara"), DisplayNameAttribute("Image Save"       )] public bool   ImageSave      {get;set;} 
            [EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            [CategoryAttribute("MPara"), DisplayNameAttribute("Image Save Folder")] public string ImageSaveFolder{get;set;} 

            [CategoryAttribute("MPara"), DisplayNameAttribute("Use Hw Trigger"   )] public bool   UseHwTrigger   {get;set;} 


            public IPkg PkgNext    ; 

            //public uint   CameraID       {get;set;} 
            //public uint   LightID        {get;set;} 
            //public bool   ImageLoad      {get;set;} 
            //public string ImageFolder    {get;set;} 
        }        

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUsrPara 
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Temp"           )] public bool    Temp  {get;set;} 
            [CategoryAttribute("UPara"), DisplayNameAttribute("EMode"          )] public EMode   EMode {get;set;}             
        }

        private CMultiObjectSelector.CustomObjectType oRet;

        //저장 값들....
        CMstPara MstPara = new CMstPara();
        CUsrPara UsrPara = new CUsrPara();  
        object   CamPara ;                  //카메라 파라. 이놈은 MPara에 Camera ID에 의해 객체가 바뀌기 때문에 미리 만들어 놓지 못함.
        object   LgtPara ;                  //조명 파라.   이놈은 MPara에 Camera ID에 의해 객체가 바뀌기 때문에 미리 만들어 놓지 못함.
        //====================================

        List<CTracker> lsTracker = new List<CTracker>();

        TPos RefPos = new TPos{dRefX   = 0.0 , //카메라는 기준포지션 없음.
                               dRefY   = 0.0 ,
                               dRefT   = 0.0 ,
                               dInspX  = 0.0 ,
                               dInspY  = 0.0 ,
                               dInspT  = 0.0 };




        //sun 이거 나중에 LinkedList 랑 비교 해서 어떤게 좋은지 확인하자.
        List<Mat> lsImage = new List<Mat>(); //오토런 검사시에 이미지를 넣어놓는 리스트 이고 이 리스트에 이미지가 1개 이상이 있으면 쓰레드에서 검사를 시작 한다.
        Mat ActiveImage = new Mat(); //오토런이 아닐때에는 여기에 이미지를 담고, 오토런일때는 lsImage에 이미지를 Push 하고 Run()에서 lsImage=>ActiveImage로 
        Mat TrainImage  = new Mat(); //트레인시에 카메라 페키지에서 Train을 누르면 ActiveImage => TrainImage



        //Virtual Cam 파일폴더에서 이미지 로딩할때 쓰는 것.
        Camera.CImgLoader ImgLoader ;//= new Camera.CVirtual();

        public PCamera(string _sName , int _iVisionID):base(_sName , _iVisionID)
        {
            
        }

        #region IPkg
        public bool Init()
        {
            ImgLoader = new Camera.CImgLoader(sName+"_ImgLoader");

            CamPara = Activator.CreateInstance(GV.Cameras[MstPara.CameraID].GetParaType());
            LgtPara = Activator.CreateInstance(GV.Lights [MstPara.LightID ].GetParaType());

            return true ;
        }

        public bool Close()
        {

            return true ;
        }

        //외부에서 보는 현재 패키지 특성.
        public TProp GetProp()
        {            
            return Prop ;
        }
        
        public object MPara 
        {
            get {return MstPara;} 
            set {MstPara = value as CMstPara;}
        }
        
        public object UPara
        {
            //
            //public UPara()
            //{
            //
            //}
            
            get
            {
                //sun 나중에 여기 널일때 문제 없는지 확인.
                //타입이 다를때 기존 메모리 CreateInstance로 생성 된것 관리가 되는지 확인.
                //타입 바꾸는 것이 능사인지 모르겠음.
                //CMultiObjectSelector.CustomObjectType oRet ;
                //if(bFirst)
                //{
                if(oRet == null)
                {
                    List<CCustomProperty> lsPara = new List<CCustomProperty>();
                    CMultiObjectSelector.GetData(ref CamPara, ref lsPara, "Camera");
                    CMultiObjectSelector.GetData(ref LgtPara, ref lsPara, "Light" );
                    CMultiObjectSelector.GetData(ref UsrPara, ref lsPara, "User"  );

                    oRet = new CMultiObjectSelector.CustomObjectType { props = lsPara };
                }
                return oRet;
            }

            set
            {
                CMultiObjectSelector.SetProperty(ref CamPara, value , "Camera");
                CMultiObjectSelector.SetProperty(ref LgtPara, value , "Light" );
                CMultiObjectSelector.SetProperty(ref UsrPara, value , "User"  );
            }
        }

        public string GetName ()  {return sName;  }
        public string GetError()  {return sError; }

        //트레커 외부에서 클릭 처리 해야함.
        public List<CTracker> Trackers {get { return lsTracker ;}}
        
        public Mat GetImage ()
        {
            //if(GV.Trainings[iVisionID]) return TrainImage ;
            return ActiveImage ;
        }

        public Mat GetTrainFormImage()
        {
            return GetImage () ;
        }

        public Mat GetTrainImage ()
        {
            return TrainImage ;
        }


        public TPos GetPos()
        {
            return RefPos ;
        }

        public bool Train()
        {
            if(ActiveImage == null) {
                sError = "ActiveImage is null!";
                return false ;
            }

            if(TrainImage != null){
                //sun 이게 이렇게 쓰는 건지 모르겠음. EmguCV 에서 따옴. 
                //When working with large image, it is recommend to call the Dispose() method to explicitly release the object.
                TrainImage.Dispose(); 
                TrainImage = null ;
                //GC.SuppressFinalize(ActiveImage);
            }
            lock(GV.ImageLock[iVisionID])//lock(ActiveImage) 
            {
                //Create(int rows, int cols, DepthType type, int channels);
                //TrainImage.Create(ActiveImage.Rows , ActiveImage.Cols , ActiveImage.Depth , ActiveImage.NumberOfChannels);
                //ActiveImage.CopyTo(TrainImage);
                TrainImage = ActiveImage.Clone();
            }

            return true ;
        }

        struct TThreadPara
        {
            public Mat Img ;
            public string File ;
            public int    Index ;
        }

        //return 비정상 종료 여부 리턴.
        
        public bool Run(TRunPara _tPara , ref TRunRet _tRet)
        {
            _tRet.bEnded = false ;
            _tRet.NextPkg = this ;

            if(_tPara.bNeedPopImg) 
            {
                //하드웨어 트리거 방식이고 이미지로더기능쓸때는 이미지 떨어지면 트리거 쏨.
                //존나 잘 맞아 떨어졌네 ... 난감했는데.
                if(MstPara.UseHwTrigger && MstPara.ImageLoad) 
                {
                    if(lsImage.Count <= 0) Grab(true);
                }   
                
                if(lsImage.Count <= 0){
                                      
                    return true ;//이미지가 아직 안들어 온거라 리턴 true;.
                }
                lock(GV.ImageLock[iVisionID])
                //lock(lsImage)
                { //여기랑 Display랑 서로 라킹 안걸면 서로 침해함.
                    if(ActiveImage != null){
                        //sun 이게 이렇게 쓰는 건지 모르겠음. EmguCV 에서 따옴. 
                        //When working with large image, it is recommend to call the Dispose() method to explicitly release the object.
                        ActiveImage.Dispose(); 
                        //ActiveImage = null ;
                    }
                    
                    
                    ActiveImage = lsImage[0] ;                    
                    if(MstPara.ImageSave)
                    {
                        string sFileName = MstPara.ImageSaveFolder + "//" + DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff") + ".bmp" ;
                        try{
                            const bool bUseThread = true ;
                            TThreadPara Para ;
                            

                            Para.File  = MstPara.ImageSaveFolder + "//" + iSavedIdx.ToString()+".bmp"; //sFileName ;
                            Para.Img   = ActiveImage.Clone() ;
                            Para.Index = iSavedIdx ;

                            iSavedIdx++;

                            if(bUseThread) ThreadPool.QueueUserWorkItem(new WaitCallback(SaveImage) , Para);
                            else           ActiveImage.Save(sFileName);
                        }
                        catch(Exception _e)
                        {
                            sError = sFileName + " saving failed" ;
                            return false ;
                        }    
                    }
                    try{
                        lsImage.RemoveAt(0);
                    }
                    catch(Exception _e)
                    {
                        sError = "lsImage(Image List) Remove Failed - " + _e.ToString() ;
                        return false ;
                    }                        
                     
                } 
            }
         
            _tRet.bEnded  = true ;
            _tRet.NextPkg = MstPara.PkgNext ;
            
            return true ;
        }

        private void SaveImage(object _oObj)
        {
            TThreadPara Para = (TThreadPara)_oObj  ;
            //iSavedIdx = Para.Index ;
            string sFileName = Para.File ;
            Para.Img.Save(sFileName);
            Para.Img.Dispose();
        }

        //static object LockDisp = new object();
        int iDispCnt = 0;
        int iFrameCnt = 0;
        int iSavedIdx = 0 ; //마지막 세이브된 인덱스.
        public bool Paint(Graphics _g)
        {
            if (ActiveImage == null) return false;

            try
            {
                iDispCnt++;
                if (iDispCnt > 100000) iDispCnt = 0;

                if(tScaleOffset.fScaleX == 0 && tScaleOffset.fScaleY ==0)
                {
                    SetFitScaleOffset(_g.ClipBounds.Width , _g.ClipBounds.Height , ActiveImage.Width , ActiveImage.Height);
                }

                ActiveImage.Paint(_g, tScaleOffset);
            }
            catch (Exception _e)
            {
                sError = _e.ToString();
                return false;
            }
            

            _g.DrawString("FrameCnt : " + iFrameCnt.ToString()    , new Font("헤움봄비152", 20), Brushes.Yellow, 0, 20);
            _g.DrawString("DispCnt  : " + iDispCnt .ToString()    , new Font("헤움봄비152", 20), Brushes.Yellow, 0, 40);
            _g.DrawString("ListCnt  : " + lsImage.Count.ToString(), new Font("헤움봄비152", 20), Brushes.Yellow, 0, 60);
            return true ;
        }

        public bool PaintTrain (Graphics _g)
        {
            if (ActiveImage == null) return false;

            try
            {

                iDispCnt++;
                if (iDispCnt > 100000) iDispCnt = 0;

                if (tScaleOffset.fScaleX == 0 && tScaleOffset.fScaleY == 0)
                {
                    SetFitScaleOffset(_g.ClipBounds.Width , _g.ClipBounds.Height , ActiveImage.Width , ActiveImage.Height);
                }

                ActiveImage.Paint(_g, tScaleOffset);
            }
            catch (Exception _e)
            {
                sError = _e.ToString();
                return false;
            }
            

            _g.DrawString("FrameCnt : " + iFrameCnt.ToString()    , new Font("헤움봄비152", 20), Brushes.Yellow, 0, 20);
            _g.DrawString("DispCnt  : " + iDispCnt .ToString()    , new Font("헤움봄비152", 20), Brushes.Yellow, 0, 40);
            _g.DrawString("ListCnt  : " + lsImage.Count.ToString(), new Font("헤움봄비152", 20), Brushes.Yellow, 0, 60);
            return true ;
        }
        
        public TScaleOffset ScaleOffset
        {
            get {return tScaleOffset ;} 
            set {tScaleOffset = value;}
        }

        public bool ShowDialog() //bHaveDialog 가 on인 페키지만 Show 해줌.
        {
            return true ;

        }

        //오토런 전환시 수행할 것들함.
        public bool Autorun(bool _bAutorun)
        {
            UseImgList = true;
            if (_bAutorun && MstPara.UseHwTrigger) return GV.Cameras[MstPara.CameraID].SetModeHwTrigger(true );
            else                                   return GV.Cameras[MstPara.CameraID].SetModeHwTrigger(false);

        }

        //오토런중에 뭔가 미리 준비 해야 될 것들 있으면 한다.
        public bool Ready()
        {
            bool bRet = true ;

            foreach(Mat Img in lsImage)
            {
                Img.Dispose();
            }

            lsImage.Clear();


            ImgLoader.ClearImgIdx();
             
            iFrameCnt = 0;
            iDispCnt = 0;
            iSavedIdx = 0 ;
            GV.Cameras[MstPara.CameraID].ApplyPara(CamPara);

            //그랩시에 프레임콜백이 들어오기때문에 
            //하드웨어 트리거시에 콜백등록이 안되어 있어서 
            //레디시에 일단 미리 등록하는 걸로....
            GV.Cameras[MstPara.CameraID].SetGrabFunc(GrabCallback);

            //경로 없으면 만들기.
            if(MstPara.ImageLoad && !Directory.Exists(MstPara.ImageLoadFolder ))
            {
                try
                {
                    if(!CFolderMaker.MakeFolder(MstPara.ImageLoadFolder))// MakeFolder(MstPara.ImageLoadFolder))
                    {
                        sError = MstPara.ImageLoadFolder + " Make Folder failed!" ;
                        bRet = false ;
                    }
                }
                catch(Exception _e)
                {
                    sError = _e.ToString() ;
                    bRet = false ;
                }
            }
            
            if(MstPara.ImageSave && !Directory.Exists(MstPara.ImageSaveFolder ))
            {
                try
                {   
                    if(!CFolderMaker.MakeFolder(MstPara.ImageSaveFolder))
                    {
                        sError = MstPara.ImageSaveFolder + " Make Folder failed!" ;
                        bRet = false ;
                    }
                }
                catch(Exception _e)
                {
                    sError = _e.ToString() ;
                    bRet = false ;
                }
            }

            return bRet;
        }

        public bool MouseDown(MouseEventArgs _e)
        {
            return false ;
        }
        public bool MouseMove(MouseEventArgs _e)
        {
            return false ;
        }
        public bool MouseUp  (MouseEventArgs _e)
        {
            return false ;
        }

        
        //한비전단위로 로드세이브 되기에 세이브시에 PKG이름이나 갯수 변경사항 찌꺼기 남는 것을 방지 하기 위해.
        //해당 비전을 한번 다 지우고 해야 함.
        public bool LoadSave (bool _bLoad , string _sPath)
        {
            bool bRet = true ;
            string FilePath      = _sPath+"\\"+sName+"_"+GetType().Name +".ini";
            string TrainImgPath  = _sPath+"\\"+sName+"_"+GetType().Name +"_TrainImg.bmp";
            string ActiveImgPath = _sPath+"\\"+sName+"_"+GetType().Name +"_ActiveImg.bmp";
            if (_bLoad)
            {
                CAutoIniFile.LoadStruct(FilePath,"MasterPara",ref MstPara ); 
                CAutoIniFile.LoadStruct(FilePath,"UserPara"  ,ref UsrPara ); 

                if(CamPara == null || CamPara.GetType() != GV.Cameras[MstPara.CameraID].GetParaType()){
                    CamPara = Activator.CreateInstance(GV.Cameras[MstPara.CameraID].GetParaType());
                }
                if(LgtPara == null || LgtPara.GetType() != GV.Lights[MstPara.LightID].GetParaType()){
                    LgtPara = Activator.CreateInstance(GV.Lights[MstPara.LightID].GetParaType());
                }
                CAutoIniFile.LoadStruct(FilePath,"CameraPara",ref CamPara); 
                CAutoIniFile.LoadStruct(FilePath,"LightPara" ,ref LgtPara); 


                //밑에서 로딩이 안되면 그냥 널인 상태로 남아있어서 로딩이 안됨.
                //if(TrainImage != null)
                //{
                //    TrainImage.Dispose();
                //    TrainImage = null ;
                //}
                //if(ActiveImage != null)
                //{
                //    ActiveImage.Dispose();
                //    ActiveImage = null ;
                //}

                if(File.Exists(TrainImgPath ))
                {
                    try
                    {
                        TrainImage  = CvInvoke.Imread(TrainImgPath , ImreadModes.Unchanged );
                    }
                    catch(Exception _e)
                    {
                        sError = _e.ToString() ;
                        bRet = false ;
                    }
                }

                if(File.Exists(ActiveImgPath ))
                {
                    try
                    {
                        ActiveImage= CvInvoke.Imread(ActiveImgPath, ImreadModes.Unchanged);
                    }
                    catch(Exception _e)
                    {
                        sError = _e.ToString() ;
                        bRet = false ;
                    }
                }
            }
            else
            {
                //Vision에서 폴더 지움.if(File.Exists(FilePath)) File.Delete(FilePath);
                CAutoIniFile.SaveStruct(FilePath,"MasterPara",ref MstPara); 
                CAutoIniFile.SaveStruct(FilePath,"UserPara"  ,ref UsrPara);                 
                CAutoIniFile.SaveStruct(FilePath,"CameraPara",ref CamPara); 
                CAutoIniFile.SaveStruct(FilePath,"LightPara" ,ref LgtPara); 

                if(TrainImage  != null)TrainImage .Save(TrainImgPath );
                if(ActiveImage != null)
                {
                    lock (GV.ImageLock[iVisionID])
                    {
                        ActiveImage.Save(ActiveImgPath);
                    }
                }
            }

            MstPara.PkgNext = null ;
            for(int i = 0 ; i < GV.Visions[iVisionID].Pkgs.Count ; i++)
            {
                if(GV.Visions[iVisionID].Pkgs[i] == this)
                {
                    if(GV.Visions[iVisionID].Pkgs.Count > i+1)
                    {
                        MstPara.PkgNext = GV.Visions[iVisionID].Pkgs[i+1];
                        break ;
                    }
                }
            }

            //경로 없으면 만들기.
            if(MstPara.ImageLoad && !Directory.Exists(MstPara.ImageLoadFolder ))
            {
                try
                {
                    if(!CFolderMaker.MakeFolder(MstPara.ImageLoadFolder))// MakeFolder(MstPara.ImageLoadFolder))
                    {
                        sError = MstPara.ImageLoadFolder + " Make Folder failed!" ;
                        bRet = false ;
                    }
                }
                catch(Exception _e)
                {
                    sError = _e.ToString() ;
                    bRet = false ;
                }
            }
            
            if(MstPara.ImageSave && !Directory.Exists(MstPara.ImageSaveFolder ))
            {
                try
                {   
                    if(!CFolderMaker.MakeFolder(MstPara.ImageSaveFolder))
                    {
                        sError = MstPara.ImageSaveFolder + " Make Folder failed!" ;
                        bRet = false ;
                    }
                }
                catch(Exception _e)
                {
                    sError = _e.ToString() ;
                    bRet = false ;
                }
            }

            return bRet ;
        }

        #endregion

        //=====================================================================================================================================
        #region General
        EventWaitHandle WaitForSingleObject = new EventWaitHandle(false ,EventResetMode.ManualReset);
        bool UseImgList = false; 
        
        public int GetImageCnt()
        {
            return lsImage.Count ;
        }

        public Mat GetAcitveImage()
        {
            return ActiveImage;
        }

        public bool LoadActiveImage(string _sPath)
        {
            lock(GV.ImageLock[iVisionID])
            { //여기랑 Display랑 서로 라킹 안걸면 서로 침해함.
                if(ActiveImage != null){
                    //sun 이게 이렇게 쓰는 건지 모르겠음. EmguCV 에서 따옴. 
                    //When working with large image, it is recommend to call the Dispose() method to explicitly release the object.
                    ActiveImage.Dispose(); 
                    ActiveImage = null ;
                    //GC.SuppressFinalize(ActiveImage);
                }
                try
                {

                    ActiveImage = new Mat(_sPath , ImreadModes.Unchanged);
                }
                catch (Exception _e)
                {
                    sError = _e.ToString();
                    return false;
                }
            }

            return true ;
        }
        public bool SaveActiveImage(string _sPath)
        {
            lock(GV.ImageLock[iVisionID])
            { 
                ActiveImage.Save(_sPath);
            }

            return true ;
        }

        public bool LoadTrainImage(string _sPath)
        {
            lock(TrainImage)
            { //여기랑 Display랑 서로 라킹 안걸면 서로 침해함.
                if(TrainImage != null){
                    //sun 이게 이렇게 쓰는 건지 모르겠음. EmguCV 에서 따옴. 
                    //When working with large image, it is recommend to call the Dispose() method to explicitly release the object.
                    TrainImage.Dispose(); 
                    TrainImage = null ;
                    //GC.SuppressFinalize(ActiveImage);
                }
                TrainImage = new Mat(_sPath, ImreadModes.Unchanged);
            }

            return true ;
        }
        public bool SaveTrainImage(string _sPath)
        {
            lock(TrainImage)
            { 
                TrainImage.Save(_sPath);
            }

            return true ;
        }


        public bool Grab(bool _bUseImgList = false)
        {
            
            WaitForSingleObject.Reset();

            UseImgList = _bUseImgList ;

            if(MstPara.ImageLoad){
                if(MstPara.ImageLoadFolder =="")
                {
                    sError = "MstPara.ImageFolder is Empty";
                    return false;
                }
                if (!CFolderMaker.MakeFilePathFolder(MstPara.ImageLoadFolder)) 
                {
                    sError = "MstPara.ImageFolder(" + MstPara.ImageLoadFolder + ") is not Exist";
                    return false;
                }
                Camera.CImgLoader.CUPara Para = new Camera.CImgLoader.CUPara();
                Para.FolderPath = MstPara.ImageLoadFolder ;
                ImgLoader.ApplyPara(Para);

                ImgLoader.SetGrabFunc(GrabCallback);
                if(!ImgLoader.Grab())
                {
                    sError = ImgLoader.GetError();
                    return false ;
                }
            }
            else {
                //카메라 인덱스 세팅 오버 되었을때.
                if(MstPara.CameraID >= GV.Cameras.Length) 
                {
                    sError = "Camera Index Overflow" ;
                    return false ;
                }
                //세팅
                GV.Cameras[MstPara.CameraID].ApplyPara(CamPara);

                //그랩전에 콜백 함수 세팅 하고 그랩.
                GV.Cameras[MstPara.CameraID].SetGrabFunc(GrabCallback);
                if(!GV.Cameras[MstPara.CameraID].Grab())
                {
                    sError = GV.Cameras[MstPara.CameraID].GetError();
                    return false ;
                }
            }

            try {
                if(!WaitForSingleObject.WaitOne(1000)){
                    
                    sError = "Grab TimeOut";
                    return false ;
                }
            }
            catch(Exception _e){
                sError = _e.ToString();
                return false ;
            }           
            
            
            

            return true ;
        }

        bool GrabCallback(int _iWidth , int _iHeight , int _iBit , EPixelFormat _ePxFormat , IntPtr _pImage )
        {
            int iStride = ((_iWidth * _iBit / 8 ) /4) * 4 ;
            if(((_iWidth * _iBit) /4) % 4 != 0){
                iStride++;
            }

            iFrameCnt++;
            if(iFrameCnt > 100000) iFrameCnt = 0;

            Mat Img ;
            if(_ePxFormat == Camera.EPixelFormat.Gray) {
                //이렇게 하는것은 데이터 까지 홀랑 복사가 이뤄지지 않고 MAT 해더만 복사생성된다.
                Img = new Mat(_iHeight , _iWidth , Emgu.CV.CvEnum.DepthType.Cv8U , 1 , _pImage , iStride);
            }
            else if(_ePxFormat == Camera.EPixelFormat.Bgr888) {
                Img = new Mat(_iHeight , _iWidth , Emgu.CV.CvEnum.DepthType.Cv8U , 3 , _pImage , iStride);
            }
            else if(_ePxFormat == Camera.EPixelFormat.Rgb888) {
                using(Mat TempImg = new Mat(_iHeight , _iWidth , Emgu.CV.CvEnum.DepthType.Cv8U , 3 , _pImage , iStride))
                {
                    Img = new Mat(_iHeight , _iWidth , Emgu.CV.CvEnum.DepthType.Cv8U , 3 , _pImage , iStride);
                    try{                    
                        CvInvoke.CvtColor(TempImg , Img ,Emgu.CV.CvEnum.ColorConversion.Rgb2Bgr , 3);                    
                    }
                    catch(Exception _e)
                    {
                        sError = _e.ToString();
                        return false ;
                    }
                }
            }
            else if(_ePxFormat == Camera.EPixelFormat.Yuv422) {  
                int iStrideBgr = ((_iWidth * _iBit / 8 ) /4) * 4 ;
                if(((_iWidth * _iBit) /4) % 4 != 0){
                    iStrideBgr++;
                }
                using(Mat TempImg = new Mat(_iHeight , _iWidth , Emgu.CV.CvEnum.DepthType.Cv8U , 2 , _pImage , iStride))
                {
                    Img = new Mat(_iHeight , _iWidth , Emgu.CV.CvEnum.DepthType.Cv8U , 3 , _pImage , iStride);
                    try{                    
                        CvInvoke.CvtColor(TempImg , Img ,Emgu.CV.CvEnum.ColorConversion.Yuv2RgbY422 , 3);                    
                    }
                    catch(Exception _e)
                    {
                        sError = _e.ToString();
                        return false ;
                    }
                }
            }
            else 
            {
                sError = "Unknown Format Image";
                return false ;
            }

            //sun 여기서 Autorun 상태에 따라 리스트에 넣던 
            if(UseImgList)
            {
                //lock(lsImage)
                {
                    lsImage.Add(Img.Clone());
                }
                Img.Dispose();
                Img = null ;
            }            
            else
            {
                lock(GV.ImageLock[iVisionID]) //lock(ActiveImage)//
                { //여기랑 Display랑 서로 라킹 안걸면 서로 침해함.
                    if(ActiveImage != null)
                    {
                        //sun 이게 이렇게 쓰는 건지 모르겠음. EmguCV 에서 따옴. 
                        //When working with large image, it is recommend to call the Dispose() method to explicitly release the object.                    
                        ActiveImage.Dispose();                         
                        ActiveImage = null ;                    
                        //GC.Collect();                        
                    }                    
                    //여기서 통복사본을 만듬.
                    ActiveImage = Img.Clone(); 
                }
                Img.Dispose();
                Img = null ;
            }
            
            
            WaitForSingleObject.Set();


            //일단 삭제 하고 Vision에서 호출 하는 걸로.
            //if (MstPara.UseHwTrigger && GV.AutoRun && GV.PaintCallbacks[iVisionID] != null)
            //{
            //    GV.PaintCallbacks[iVisionID]();
            //}

            return true ;
        }
        #endregion
    }
}
