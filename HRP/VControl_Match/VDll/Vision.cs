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

        struct TStat
        {
            public bool   NeedGrabInsp ;
            public bool   NeedGrab     ;
            public bool   NeedInsp     ;
                                       
            public bool   Grabbing     ;
            public bool   Inspecting   ;

            public bool   bNeedDispRslt ;
        }
        TStat Stat ;



        public void NeedGrab()
        {
            pkDisplay = pkCamera ;
            Stat.NeedGrab = true ;
            Stat.Grabbing = true ;
        }
        public void NeedInsp()
        {
            Stat.NeedInsp = true ;
            Stat.Inspecting = true ;
        }
        public void NeedGrabInsp()
        {
            Stat.NeedGrabInsp = true ;
            Stat.Grabbing   = true ; //사진찍고 움직이면서 검사할때 확인.
            Stat.Inspecting = true ; //그자리에서 그냥 찍고 검사할때 확인.
        }
        public bool GetGrabbing()
        {
            return Stat.Grabbing ;
        }

        public bool GetInspecting()
        {
            return Stat.Inspecting ;
        }
        public void UpdateStat()
        {
            //PCamera에서 일단 그랩을 하게 만들어 놨는데.
            //이부분에 관하여.. 고민 해야 하고.
            //봐서 원복 해야 될지도 모름.

            //PCamera에 Use Ex Trigger랑 연동 하여 관리를 해야함.

            while(true)
            {
                Thread.Sleep(0);
                if(pkCamera == null) continue ; 


                if(Cycle == ECycle.Idle)
                {
                    bool bCycleGrabInsp = Stat.NeedGrabInsp ;
                    bool bCycleGrab     = Stat.NeedGrab     ;
                    bool bCycleInsp     = Stat.NeedInsp     ;
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
                    Stat.NeedGrabInsp = false ; 

                    pkCamera.Grab(true );   
                    Stat.Grabbing = false ;

                    Stat.bNeedDispRslt = true ;
                    RunPkgs(true) ;
                    Stat.Inspecting = false ;

                    
                    Cycle = ECycle.Idle ;
                }
                else if(Cycle == ECycle.Grab) 
                {
                    Stat.NeedGrab = false ;

                    Stat.bNeedDispRslt = false ;

                    pkCamera.Grab(false );
                    Stat.Grabbing = false ;
                    GV.PaintCallbacks[iVisionID]();
                    Cycle = ECycle.Idle ;
                }
                else if(Cycle == ECycle.Insp) 
                {
                    Stat.NeedInsp = false ;


                    Stat.bNeedDispRslt = true;
                    RunPkgs(false) ;
                    Stat.Inspecting = false ;
                    Cycle = ECycle.Idle ;
                }                
                else if(Cycle == ECycle.PopInsp)
                {
                    //오토런중이라 이미지 리스트에 이미지 있음.. 얼렁얼렁 이미지리스트에서 ActiveImage로 빼서 검사하자..
                    Stat.Inspecting = true ;
                    Stat.bNeedDispRslt = true;
                    RunPkgs(true);

                    Stat.Inspecting = false ;
                    Cycle = ECycle.Idle ;
                }
                
            }
        }


        //모든 리스트에는 무조건 1번이 카메라.
        List<IPkg> lsPkg = new List<IPkg>();      
        PCamera    pkCamera   ;
        IPkg       pkActivate ;
        IPkg       pkDisplay  ;

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

        public IPkg GetPkgDisplay()
        {
            return pkDisplay ;
        }

        public IPkg GetPkgCamera()
        {
            return pkDisplay ;
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
            TRunRet  RunRet  = new TRunRet();

            if(_bUseImgList)
            {
                RunPara.bNeedPopImg = true ;
            }

            pkActivate = lsPkg[0] ;

            tmTimeOut.Clear();
            while(!bRqstStop)
            {
                try
                {
                    bRunRet = pkActivate.Run(RunPara , ref RunRet);
                }            
                catch(Exception _e)
                {
                    sError = pkActivate.GetName() + "_" + pkActivate.GetError();
                    Log.ShowMessage(pkActivate.GetName() , pkActivate.GetError());
                    pkActivate = null ;
                    return false ;
                }
                
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
                    if(pkActivate is PDisplay) {
                        pkDisplay = pkActivate as PDisplay ;
                    }

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


        //===================================================================================================
        

        //public Mat GetDisplayImage()
        //{
        //    if(pkDisplay == null) return null ;
        //    pkDisplay.Get
        //    
        //}

        //public TScaleOffset GetDisplayScaleOffset()
        //{
        //    IPkg Pkg = GetDisplayPkg() ; 
        //    if(Pkg == null) return new TScaleOffset() ;
        //
        //    return Pkg.ScaleOffset;
        //}

        //public void SetDisplayScaleOffset(TScaleOffset _tScaleOffset)
        //{
        //    IPkg Pkg = GetDisplayPkg();
        //    if (Pkg == null) return;
        //    Pkg.ScaleOffset = _tScaleOffset;
        //}

        public bool LoadActiveImage(string _sPath)
        {
            Stat.bNeedDispRslt = false ;
            pkDisplay = pkCamera ;
            return pkCamera.LoadActiveImage(_sPath) ;
        }

        public bool SaveActiveImage(string _sPath)
        {
            return pkCamera.SaveActiveImage(_sPath);
        }

        //일단 미완인데 현재 보여지고 있는 이미지를 저장 하는 것을 만들자.....
        //public bool SaveDisplayImage(string _sPath)
        //{
        //    if(pkDisplay == null) {
        //        sError = "Pkg Display is null";
        //        return false ;
        //    }
        //    return pkDisplay.SaveImage(_sPath);
        //}

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
                }
                catch (Exception e)
                {
                    Console.WriteLine(_sPath + "delete failed: {0}", e.Message);
                }

                TempDir.MoveTo(_sPath +"\\");
            }

            pkCamera = null;
            pkDisplay = null;
            for(int i = 0 ; i < lsPkg.Count ; i++)
            {
                if(pkCamera == null && lsPkg[i] is PCamera) {
                    pkCamera = lsPkg[i] as PCamera ;
                }
                if(/*pkDisplay == null && */lsPkg[i] is PDisplay) {
                    pkDisplay = lsPkg[i] as PDisplay ;
                }                
            }
            return true ;
        }
    }
}
