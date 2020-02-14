using COMMON;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using VDll.Pakage;
using Emgu.CV;

namespace VDll
{
    class Vision : CErrorObject
    {
        enum ECycle
        {
            Idle     = 0 , //아무것도 하지 않는 상태..
            GrabInsp     , //이미지 리스트 사용 하지 않고 그랩과 한번하여 ActiveImage 세팅 및 검사 함. 메뉴얼로 사용.
            Grab         , //이미지 리스트 사용 하지 않고 그랩만 한번하여 ActiveImage 세팅.  메뉴얼로 라이브 혹은 그랩 할 때 씀.
            Insp         , //이미지 리스트 사용 하지 않고 ActiveImage 검사 
            PopInsp        //이미지 리스트에 있는 이미지를 하나 뽑아와서 검사.
        }
        ECycle Cycle ;

        public struct TVisnStat
        {
            //요청
            public bool NeedGrabInsp;
            public bool NeedGrab;
            public bool NeedInsp;

            //상태
            public bool Grabbing;
            public bool Inspecting;
            public bool bNeedDispRslt;

            //티칭중.
            public bool Trainings   ;
        }     
        public TVisnStat VisnStats ;

        public void NeedGrab()
        {
            VisnStats.NeedGrab = true ;
            VisnStats.Grabbing = true ;
        }
        public void NeedInsp()
        {
            VisnStats.NeedInsp   = true ;
            VisnStats.Inspecting = true ;
        }
        public void NeedGrabInsp()
        {
            VisnStats.NeedGrabInsp = true ;
            VisnStats.Grabbing     = true ; //사진찍고 움직이면서 검사할때 확인.
            VisnStats.Inspecting   = true ; //그자리에서 그냥 찍고 검사할때 확인.
        }
        public bool GetGrabbing()
        {
            return VisnStats.Grabbing ;
        }

        public bool GetInspecting()
        {
            return VisnStats.Inspecting ;
        }
        public void UpdateStat()
        {
            while(true)
            {
                Thread.Sleep(1);
                if(pkCamera == null) continue ; 

                if(Cycle == ECycle.Idle)
                {
                    bool bCycleGrabInsp = VisnStats.NeedGrabInsp ;
                    bool bCycleGrab     = VisnStats.NeedGrab     ;
                    bool bCycleInsp     = VisnStats.NeedInsp     ;
                    bool bCyclePopInsp  = pkCamera != null && pkCamera.GetImageCnt() > 0;                    

                         if(bCycleGrabInsp) Cycle = ECycle.GrabInsp ;
                    else if(bCycleGrab    ) Cycle = ECycle.Grab     ;
                    else if(bCycleInsp    ) Cycle = ECycle.Insp     ;
                    else if(bCyclePopInsp ) Cycle = ECycle.PopInsp  ;//이조건은 플라잉 방식일때의 시작 조건......
                }
                
                
                if(Cycle == ECycle.Idle)
                {
                    continue ;
                }
                else if(Cycle == ECycle.GrabInsp) 
                {
                    VisnStats.NeedGrabInsp = false ; 

                    pkCamera.Grab(true );   
                    VisnStats.Grabbing = false ;

                    VisnStats.bNeedDispRslt = true ;
                    RunPkgs(true) ;
                    VisnStats.Inspecting = false ;                    
                    Cycle = ECycle.Idle ;
                }
                else if(Cycle == ECycle.Grab) 
                {
                    VisnStats.NeedGrab = false ;
                    VisnStats.bNeedDispRslt = false ;

                    pkCamera.Grab(false );
                    VisnStats.Grabbing = false ;
                    GV.PaintCallbacks[iVisionID]();
                    Cycle = ECycle.Idle ;
                }
                else if(Cycle == ECycle.Insp) 
                {
                    VisnStats.NeedInsp = false ;

                    VisnStats.bNeedDispRslt = true;
                    RunPkgs(false) ;
                    VisnStats.Inspecting = false ;
                    Cycle = ECycle.Idle ;
                }                
                else if(Cycle == ECycle.PopInsp)
                {
                    //오토런중이라 이미지 리스트에 이미지 있음.. 얼렁얼렁 이미지리스트에서 ActiveImage로 빼서 검사하자..
                    VisnStats.Inspecting = true ;
                    VisnStats.bNeedDispRslt = true;
                    RunPkgs(true);

                    VisnStats.Inspecting = false ;
                    Cycle = ECycle.Idle ;
                }                
            }
        }

        //모든 리스트에는 무조건 1번이 카메라.
        List<IPkg> lsPkg = new List<IPkg>();      
        PCamera    pkCamera   ;//한비전엔 일단 1개의 카메라만 있음.
        IPkg       pkActivate ;//현재 진행중인 pkg
        PDisplay   pkDisplay  ;//화면에 뿌려주는 PKG

        Thread     trStat     ;
        int        iVisionID  ;

        public Vision(string _sName , int _iVisionID):base(_sName)
        {
            iVisionID = _iVisionID ;
        }

        public bool Init()
        {
            trStat = new Thread(UpdateStat); 
            trStat.Priority = ThreadPriority.Normal ;
            trStat.Start();
            return true ;
        }

        public bool Close()
        {            
            trStat.Abort();
            trStat.Join();
            //원래 있던 페키지들 지워야 함.....
            for(int i = 0 ; i < lsPkg.Count ; i++)
            {
                //sun IDisposible 상속 받아서 구현 하자.
                lsPkg[i].Close();
                //lsPkg[i].Dispose();
            }
            lsPkg.Clear();
            pkCamera = null;
            pkDisplay = null;

            return true ;
        }

        public bool SetPkgActivate(IPkg _pkActivate)
        {
            pkActivate = _pkActivate;
            return true ;
        }

        public IPkg GetPkgActivate()
        {
            return pkActivate ;
        }

        public PDisplay GetPkgDisplay()
        {
            return pkDisplay ;
        }

        public PCamera GetPkgCamera()
        {
            return pkCamera ;
        }


        public Mat GetImage()
        {
            Mat Image = new Mat();

                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) Image = pkDisplay .GetImage();
            else if(VisnStats.Trainings     && pkActivate != null) Image = pkActivate.GetImage();
            else if(                           pkCamera   != null) Image = pkCamera  .GetImage();
            
            return Image;
        }

        public TScaleOffset GetScaleOffset()
        {
            TScaleOffset ScaleOffset = new TScaleOffset(0,0,0,0);

                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) ScaleOffset = pkDisplay .ScaleOffset;
            else if(VisnStats.Trainings     && pkActivate != null) ScaleOffset = pkActivate.ScaleOffset;
            else if(                           pkCamera   != null) ScaleOffset = pkCamera  .ScaleOffset;

            return ScaleOffset;
        }

        public void SetScaleOffset(TScaleOffset _ScaleOffset)
        {
                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) pkDisplay .ScaleOffset = _ScaleOffset;
            else if(VisnStats.Trainings     && pkActivate != null) pkActivate.ScaleOffset = _ScaleOffset;
            else if(                           pkCamera   != null) pkCamera  .ScaleOffset = _ScaleOffset;

        }

        public TProp GetProp()
        {
            TProp prop = new TProp();

                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) prop = pkDisplay .GetProp();
            else if(VisnStats.Trainings     && pkActivate != null) prop = pkActivate.GetProp();
            else if(                           pkCamera   != null) prop = pkCamera  .GetProp();

            return prop;
        }

        public object GetMPara()
        {
            object prara = new object();

                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) prara = pkDisplay .MPara ;
            else if(VisnStats.Trainings     && pkActivate != null) prara = pkActivate.MPara ;
            else if(                           pkCamera   != null) prara = pkCamera  .MPara ;

            return prara;
        }

        public object GetUPara()
        {
            object prara = new object();

                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) prara = pkDisplay .UPara ;
            else if(VisnStats.Trainings     && pkActivate != null) prara = pkActivate.UPara ;
            else if(                           pkCamera   != null) prara = pkCamera  .UPara ;

            return prara;
        }

        public void SetMPara(object _para)
        {
                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) pkDisplay .MPara = _para ;
            else if(VisnStats.Trainings     && pkActivate != null) pkActivate.MPara = _para ;
            else if(                           pkCamera   != null) pkCamera  .MPara = _para ;
        }

        public void SetUPara(object _para)
        {
                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) pkDisplay .UPara = _para ;
            else if(VisnStats.Trainings     && pkActivate != null) pkActivate.UPara = _para ;
            else if(                           pkCamera   != null) pkCamera  .UPara = _para ;
        }

        public bool Run(TRunPara _tPara , ref TRunRet _tRet)
        {
            bool bRet = false;

                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) bRet = pkDisplay .Run(_tPara,ref _tRet);
            else if(VisnStats.Trainings     && pkActivate != null) bRet = pkActivate.Run(_tPara,ref _tRet);
            else if(                           pkCamera   != null) bRet = pkCamera  .Run(_tPara,ref _tRet);

            return bRet;
        }

        public bool Paint (Graphics _g , bool _bTrain)
        {
            bool bRet = false;
                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) bRet = pkDisplay .Paint(_g,_bTrain);
            else if(VisnStats.Trainings     && pkActivate != null) bRet = pkActivate.Paint(_g,_bTrain);
            else if(                           pkCamera   != null) bRet = pkCamera  .Paint(_g,_bTrain);

            return bRet;
        }

        public bool PaintScOf  (Graphics _g , bool _bTrain , TScaleOffset _ScaleOffset)
        {
            bool bRet = false;
                 if(VisnStats.bNeedDispRslt && pkDisplay  != null) bRet = pkDisplay .PaintScOf(_g,_bTrain,_ScaleOffset);
            else if(VisnStats.Trainings     && pkActivate != null) bRet = pkActivate.PaintScOf(_g,_bTrain,_ScaleOffset);
            else if(                           pkCamera   != null) bRet = pkCamera  .PaintScOf(_g,_bTrain,_ScaleOffset);

            return bRet;
        }

        public void SetFitScaleOffset(float _fPanelWidth , float _fPanelHeight , int _iImgWidth , int _iImgHeight)
        {
            if(pkCamera != null) pkCamera.SetFitScaleOffset(_fPanelWidth,_fPanelHeight,_iImgWidth,_iImgHeight);

        }
        //=================================================================================================== 
        public bool LoadActiveImage(string _sPath)
        {
            VisnStats.bNeedDispRslt = false ;
            //pkDisplay = pkCamera ;
            
            return pkCamera.LoadActiveImage(_sPath) ;
        }

        public bool SaveActiveImage(string _sPath)
        {
            return pkCamera.SaveActiveImage(_sPath);
        }

        public void Autorun(bool _bOn)
        {
            if(_bOn) bRqstStop = true ;

            foreach (IPkg Pkg in lsPkg)
            {
                if (!Pkg.Autorun(_bOn))
                {
                    
                }
            }
        }

        public void Ready()
        {
            foreach (IPkg Pkg in lsPkg)
            {
                if (!Pkg.Ready())
                {

                }
            }
        }


        bool bRqstStop = false ;
        CDelayTimer tmTimeOut = new CDelayTimer();
        public bool RunPkgs(bool _bUseImgList)
        {
            if(lsPkg.Count <= 0) return false ;   
            bRqstStop = false ;

            bool     bRunRet = true ;
            TRunPara RunPara = new TRunPara();
            TRunRet  RunRet  = new TRunRet ();

            if(_bUseImgList)
            {
                RunPara.bNeedPopImg = true ;
            }

            pkActivate = lsPkg[0] ;

            tmTimeOut.Clear();
            while(!bRqstStop)
            {
                //try
                {
                    bRunRet = pkActivate.Run(RunPara , ref RunRet);
                }            
                //catch(Exception _e)
                //{
                //    sError = pkActivate.GetName() + "_" + pkActivate.GetError();
                //    Log.ShowMessage(pkActivate.GetName() , pkActivate.GetError());
                //    pkActivate = null ;
                //    return false ;
                //}
                
                if(!bRunRet) //함수 수행 이상.
                {
                    sError = pkActivate.GetError();
                    pkActivate = null ;
                    return false ;
                }

                //현재는 카메라에서만 이미지 기다리는 용도로 쓴다. 
                if(RunRet.bEnded) 
                {
                    //디스플레이면 등록시켜놔서 화면 그려줄때쓰게 한다.
                    //처음은 로드세이브에서 등록 시키고 그다음에 멀티 Display일때를 감안하여
                    //다음 자제가 Display이면 갱신 시켜서 중간에 화면을 그리더라도 문제 없게.
                    //if(pkActivate is PDisplay) {
                    //    pkDisplay = pkActivate as PDisplay ;
                    //}
                    //일단 Display없애보자.

                    if(RunRet.NextPkg == null)
                    {
                        //모든 페키지 러닝완료.
                        pkActivate = null ;
                        return true ; 
                    }
                    //RunRet.bEnded == true 면 다음PKG로 이동.
                    pkActivate = RunRet.NextPkg ;                     
                }
                else 
                {
                    //나중에 타임 아웃은 외부로 빼야 될듯 함...
                    //if(tmTimeOut.OnDelay(2000))
                    //{
                    //    sError = CrntPkg.GetName() + "Pkg TimeOut";
                    //    return false ; 
                    //}
                }
            }

            bRqstStop = false;
            sError = "Request Stop!";
            pkActivate = null ;
            return false ; 
        }
        
        public List<IPkg> Pkgs { 
            get { return lsPkg ;}            
        }
        public PCamera Camera {
            get { return pkCamera ;}
        }

        public bool LoadSave(bool _bLoad , string _sPath)
        {
            string sPkgsFile     = _sPath +  "\\Packages.ini";
            string sTempPkgsFile = _sPath +"_Temp\\Packages.ini";
            if(_bLoad)
            {
                int iPkgCnt ;
                CIniFile Ini = new CIniFile(sPkgsFile) ;
                Ini.Load("Total" , "PkgCnt" , out iPkgCnt);
                string sName ;
                string sType ;
                IPkg Pkg ;
                
                //원래 있던 페키지들 지워야 함.....
                for(int i = 0 ; i < lsPkg.Count ; i++)
                {
                    //sun IDisposible 상속 받아서 구현 하자.
                    lsPkg[i].Close();
                    //lsPkg[i].Dispose();
                }
                lsPkg.Clear();

                for(int i = 0 ; i < iPkgCnt ; i++)
                {                    
                    Ini.Load("Pkg"+i.ToString() , "Name" , out sName );
                    Ini.Load("Pkg"+i.ToString() , "Type" , out sType );
                    Pkg = GV.DynamicPkgMaker.New(sType , new object[] {sName , iVisionID});
                    if(Pkg == null) continue ;
                    Pkg.Init();      

                    lsPkg.Add(Pkg);
                }

                for(int i = 0 ; i < iPkgCnt ; i++)
                {                      
                    try
                    {
                        lsPkg[i].LoadSave(_bLoad , _sPath);
                    }
                    catch(Exception _e)
                    {
                        _e.ToString();
                    }
                }
            }
            else 
            {
                //일단 템프로 저장하고 완료 되면 리네임 하는 방식.                
                DirectoryInfo TempDir = new DirectoryInfo(_sPath +"_Temp\\");
                try
                {
                    if(TempDir.Exists) TempDir.Delete(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(_sPath + "delete failed: {0}", e.Message);
                }

                CIniFile Ini = new CIniFile(sTempPkgsFile) ;
                Ini.Save("Total" , "PkgCnt" , lsPkg.Count);
                for(int i = 0 ; i < lsPkg.Count ; i++)
                {
                    Ini.Save("Pkg"+i.ToString() , "Name" , lsPkg[i].GetName()    );
                    Ini.Save("Pkg"+i.ToString() , "Type" , lsPkg[i].GetType().Name);

                    try
                    {
                        lsPkg[i].LoadSave(_bLoad , _sPath +"_Temp\\");
                    }
                    catch(Exception _e)
                    {
                        _e.ToString();
                    }
                }                

                DirectoryInfo Dir = new DirectoryInfo(_sPath +"\\");
                try
                {
                    if(Dir.Exists) Dir.Delete(true);
                    TempDir.MoveTo(_sPath +"\\");
                }
                catch (Exception e)
                {
                    Log.ShowMessage("Folder Delete Error" , e.Message);
                    Console.WriteLine(_sPath + "delete failed: {0}", e.Message);
                }

                
            }

            pkCamera = null;
            pkDisplay = null;
            for(int i = 0 ; i < lsPkg.Count ; i++)
            {
                if(pkCamera == null && lsPkg[i] is PCamera) 
                {
                    pkCamera = lsPkg[i] as PCamera ;
                }        
                if(lsPkg[i] is PDisplay) 
                {
                    pkDisplay = lsPkg[i] as PDisplay ;
                }
            }
            return true ;
        }
    }
}
