﻿using COMMON;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDll.Pakage
{
    class PMorphology : CPkgObject , IPkg
    {
        readonly TProp Prop = new TProp{bHaveTrain  = true  ,
                                        bHaveDialog = false ,
                                        bHaveImage  = true  ,
                                        bHavePosRef = false };

        //public enum EMorphlogy
        //{
        //    NotUse   = 0 ,
        //    Erosion  = 1 ,
        //    Dilation = 2 ,
        //    Opening  = 3 ,
        //    Closing  = 4 ,
        //    Gradient = 5 
        //    //TopHat
        //    //BlackHat
        //}


        public class CMstPara 
        {           
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Image"    )] public string   PkgNameOfImage    {get;set;} public IPkg PkgImage ;                

            public IPkg PkgNext;
        }        

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUsrPara 
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Iter1"            )] public uint         Iter1          {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("Morph1"           )] public MorphOp      Morph1         {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("MaskShape1"       )] public ElementShape MaskShape1     {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("SizeX1"           )] public uint         SizeX1         {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("SizeY1"           )] public uint         SizeY1         {get;set;}               

            [CategoryAttribute("UPara"), DisplayNameAttribute("Iter2"            )] public uint         Iter2          {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("Morph2"           )] public MorphOp      Morph2         {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("MaskShape2"       )] public ElementShape MaskShape2     {get;set;}    
            [CategoryAttribute("UPara"), DisplayNameAttribute("SizeX2"           )] public uint         SizeX2         {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("SizeY2"           )] public uint         SizeY2         {get;set;}    
            
            [CategoryAttribute("UPara"), DisplayNameAttribute("Iter3"            )] public uint         Iter3          {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("Morph3"           )] public MorphOp      Morph3         {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("MaskShape3"       )] public ElementShape MaskShape3     {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("SizeX3"           )] public uint         SizeX3         {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("SizeY3"           )] public uint         SizeY3         {get;set;}   
            
        }

        private CMultiObjectSelector.CustomObjectType oRet;

        //저장 값들....
        CMstPara MstPara = new CMstPara();
        CUsrPara UsrPara = new CUsrPara(); public TObj[] oUsrPara;
        //Object   AlgPara ;                 public TObj[] oAlgPara;

        //====================================
        List<CTracker> lsTracker = new List<CTracker>();
        TPos Pos = new TPos{dRefX   = 0.0 , //카메라는 기준포지션 없음.
                            dRefY   = 0.0 ,
                            dRefT   = 0.0 ,
                            dInspX  = 0.0 ,
                            dInspY  = 0.0 ,
                            dInspT  = 0.0 };

        Mat MorphImg = new Mat();
        Mat TrainImg = new Mat();

        class TRslt
        {
            public int WorkTimems ;
        }
        TRslt Rslt = new TRslt();

        public PMorphology(string _sName , int _iVisionID):base(_sName , _iVisionID)
        {
            //현재 세팅 패키지와 가장 가까운 이미지를 가지고 있는 PKG로 초기화 세팅.
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                TProp Prop =  Pkg.GetProp() ;
                if(Prop.bHaveImage) MstPara.PkgNameOfImage = Pkg.GetName() ;
            }
        }

        #region IPkg
        public bool Init()
        {
            CTracker Tracker = new CTracker();
            Tracker.Init();


            lsTracker.Add(Tracker);


            //나중에 알고리즘 바꿀때는 AlgPara = Activator.CreateInstance( /*여기 생각좀 하자...*/); //sun
            //AlgPara = Activator.CreateInstance(typeof(Algorithm.CPeak.CUPara));

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
            get {
                //sun 나중에 여기 널일때 문제 없는지 확인.
                //타입이 다를때 기존 메모리 CreateInstance로 생성 된것 관리가 되는지 확인.
                //타입 바꾸는 것이 능사인지 모르겠음.
                //List<CCustomProperty> lsPara =  new List<CCustomProperty>();
                //CMultiObjectSelector.GetData(ref AlgPara, ref oAlgPara, ref lsPara , "Peak" );
                //CMultiObjectSelector.GetData(ref UsrPara, ref oUsrPara, ref lsPara , "User" );
                //return new CMultiObjectSelector.CustomObjectType { props = lsPara };
                if(oRet == null)
                {
                    List<CCustomProperty> lsPara = new List<CCustomProperty>();
                    CMultiObjectSelector.GetData(ref UsrPara, ref lsPara, "User"  );

                    oRet = new CMultiObjectSelector.CustomObjectType { props = lsPara };
                }
                return oRet;
            }
            set {
                //CMultiObjectSelector.SetProperty(ref AlgPara, value , "Peak");
                CMultiObjectSelector.SetProperty(ref UsrPara, value , "User");
            }
        }

        public string GetName ()  {return sName;  }
        public string GetError()  {return sError; }

        //트레커 외부에서 클릭 처리 해야함.
        public List<CTracker> Trackers {get { return lsTracker ;}}
        
        public Mat GetImage ()
        {
            return MorphImg ;            
        }

        public Mat GetTrainFormImage ()
        {
            if (MorphImg == null) return null;
            return MorphImg  ;
        }

        public Mat GetTrainImage ()
        {
            return TrainImg ;      
        }

        public TPos GetPos()
        {
            return Pos ;
        }

        public bool Train()
        {
            if(MorphImg == null) {
                sError = "ActiveImage is null!";
                return false ;
            }

            if(TrainImg != null){
                TrainImg.Dispose(); 
                TrainImg = null ;
            }
            lock(GV.ImageLock[iVisionID])
            {
                TrainImg = MorphImg.Clone();
            }
            return true ;
        }

        public bool Run(TRunPara _tPara , ref TRunRet _tRet)
        {
            if (MstPara.PkgImage == null) return false;
            
            _tRet.bEnded = false ;
            _tRet.NextPkg = this ;

            Stopwatch WorkTime = new Stopwatch() ;
            //WorkTime.Reset();
            WorkTime.Restart();

            //이미지 프로세싱.
            Rectangle Rect = lsTracker[0].GetRectangle();           
            
            if(MorphImg != null)  MorphImg.Dispose();

            MorphImg = MstPara.PkgImage.GetImage().Clone();

            if (MorphImg.Width  <= Rect.Right ) {sError = "Tracker Right is out of image"; return false; }
            if (MorphImg.Height <= Rect.Bottom) {sError = "Tracker Bottom is out of image"; return false; }
            



            using (Mat Roi = new Mat(MorphImg, Rect)) 
            {
                lock (GV.ImageLock[iVisionID]) //lock (Rslt.Peaks)
                {
                    if (UsrPara.Iter1 > 0 && UsrPara.SizeX1 > 0 && UsrPara.SizeY1 > 0)
                    {
                        Mat kernel1 = CvInvoke.GetStructuringElement(UsrPara.MaskShape1, new Size((int)UsrPara.SizeX1, (int)UsrPara.SizeY1), new Point(-1, -1));
                        CvInvoke.MorphologyEx(Roi, Roi, UsrPara.Morph1, kernel1, new Point(-1, -1), (int)UsrPara.Iter1, BorderType.Constant, new MCvScalar(255, 255, 255));
                    }

                    if (UsrPara.Iter2 > 0 && UsrPara.SizeX2 > 0 && UsrPara.SizeY2 > 0)
                    {
                        Mat kernel2 = CvInvoke.GetStructuringElement(UsrPara.MaskShape2, new Size((int)UsrPara.SizeX2, (int)UsrPara.SizeY2), new Point(-1, -1));
                        CvInvoke.MorphologyEx(Roi, Roi, UsrPara.Morph2, kernel2, new Point(-1, -1), (int)UsrPara.Iter2, BorderType.Constant, new MCvScalar(255, 255, 255));
                    }

                    if (UsrPara.Iter3 > 0 && UsrPara.SizeX3 > 0 && UsrPara.SizeY3 > 0)
                    {
                        Mat kernel3 = CvInvoke.GetStructuringElement(UsrPara.MaskShape3, new Size((int)UsrPara.SizeX3, (int)UsrPara.SizeY3), new Point(-1, -1));
                        CvInvoke.MorphologyEx(Roi, Roi, UsrPara.Morph3, kernel3, new Point(-1, -1), (int)UsrPara.Iter3, BorderType.Constant, new MCvScalar(255, 255, 255));
                    }
                }
            }

            WorkTime.Stop();
            Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds ;

            //OK NG에 따라 분기문....
            _tRet.bEnded  = true ;
            _tRet.NextPkg = null ;

            _tRet.NextPkg = MstPara.PkgNext;

            return true ;
        }


        //GetImage() 에서 가져오는 이미지가 항상 쓰레드안전하지 않은 상태이므로 
        //모든 상황을 잘 따져보자.. GetImage 할때 버벅일 수 있음.
        public bool Paint(Graphics _g)// , TScaleOffset _ScaleOffset)//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            return true ;
        }

        public bool PaintTrain (Graphics _g)
        {
            try
            {
                //Mat TrainImg = MstPara.PkgImage.GetImage();
                if(tScaleOffset.fScaleX == 0 && tScaleOffset.fScaleY ==0)
                {
                    SetFitScaleOffset(_g.ClipBounds.Width , _g.ClipBounds.Height , MstPara.PkgImage.GetImage().Width , MstPara.PkgImage.GetImage().Height);
                }
                MorphImg.Paint(_g, tScaleOffset);

                foreach (CTracker Tracker in lsTracker)
                {
                    Tracker.Paint(_g, tScaleOffset);
                }

            }
            catch (Exception _e)
            {
                sError = _e.ToString();
                return false;
            }

            _g.DrawString("MultiProfile WorkTime : " + Rslt.WorkTimems.ToString() , new Font("헤움봄비152", 20), Brushes.Yellow, 0, 0);
            return true ;

        }

        public TScaleOffset ScaleOffset
        {
            get {return tScaleOffset ;} 
            set {tScaleOffset = value;}
        }

        public bool ShowDialog() //bHaveDialog 가 on인 페키지만 Show 해줌.
        {
            return false ;
        }

        //오토런 전환시 수행할 것들함.
        public bool Autorun(bool _bAutorun)
        {


            return true;
        }

        //오토런중에 뭔가 미리 준비 해야 될 것들 있으면 한다.
        public bool Ready()
        {
            return true;
        }

        public bool MouseDown(MouseEventArgs _e)
        {
            foreach( CTracker Tracker in lsTracker)
            {
                if(Tracker.MouseDown(Control.ModifierKeys , _e , tScaleOffset))
                {
                    return true ;
                }
            }
            return false ;
        }
        public bool MouseMove(MouseEventArgs _e)
        {
            foreach( CTracker Tracker in lsTracker)
            {
                if(Tracker.MouseMove(Control.ModifierKeys , _e , tScaleOffset))
                {
                    return true ;
                }
            }
            
            return false ;
        }
        public bool MouseUp(MouseEventArgs _e)
        {
            foreach( CTracker Tracker in lsTracker)
            {
                if(Tracker.MouseUp(Control.ModifierKeys , _e))
                {
                    return true ;
                }
            }
            return false ;
        }

        //VisionJobFile//VisionName//PkgName 폴더까지 들어와야함.
        //한비전단위로 로드세이브 되기에 세이브시에 PKG이름이나 갯수 변경사항 찌꺼기 남는 것을 방지 하기 위해.
        //해당 비전을 한번 다 지우고 해야 함.
        public bool LoadSave (bool _bLoad , string _sPath)
        {
            bool bRet = true ;
            string FilePath     = _sPath+"\\"+sName+"_"+GetType().Name +".ini";
            string MorphImgPath = _sPath+"\\"+sName+"_"+GetType().Name +"_MorphImg.bmp";
            string TrainImgPath = _sPath+"\\"+sName+"_"+GetType().Name +"_TrainImg.bmp";

            if (_bLoad)
            {
                CAutoIniFile.LoadStruct(FilePath,"MasterPara",ref MstPara ); 
                CAutoIniFile.LoadStruct(FilePath,"UserPara"  ,ref UsrPara ); 
                if(File.Exists(MorphImgPath ))
                {
                    try
                    {
                        MorphImg = CvInvoke.Imread(MorphImgPath, ImreadModes.Unchanged);
                    }
                    catch(Exception _e)
                    {
                        sError = _e.ToString() ;
                        bRet =  false ;
                    }

                    try
                    {
                        TrainImg = CvInvoke.Imread(TrainImgPath, ImreadModes.Unchanged);
                    }
                    catch(Exception _e)
                    {
                        sError = _e.ToString() ;
                        bRet =  false ;
                    }
                }
            }
            else
            {
                //Vision에서 폴더 지움.if(File.Exists(_sPath)) File.Delete(_sPath);
                CAutoIniFile.SaveStruct(FilePath,"MasterPara",ref MstPara); 
                CAutoIniFile.SaveStruct(FilePath,"UserPara"  ,ref UsrPara);   
                if(MorphImg  != null)MorphImg .Save(MorphImgPath );
                if(TrainImg  != null)TrainImg .Save(TrainImgPath );
            }

            CIniFile Ini = new CIniFile(FilePath);
            foreach (CTracker Tracker in lsTracker)
            {
                Tracker.LoadSave(_bLoad, "Tracker1" , Ini);
            }

            //Apply
            MstPara.PkgImage = null ;
            MstPara.PkgNext  = null ;
            for (int i = 0; i < GV.Visions[iVisionID].Pkgs.Count; i++)
            {
                if (GV.Visions[iVisionID].Pkgs[i].GetName() == MstPara.PkgNameOfImage) MstPara.PkgImage = GV.Visions[iVisionID].Pkgs[i];
                if (GV.Visions[iVisionID].Pkgs[i] == this)
                {
                    if (GV.Visions[iVisionID].Pkgs.Count > i + 1)
                    {
                        MstPara.PkgNext = GV.Visions[iVisionID].Pkgs[i + 1];
                        break;
                    }
                }
            }            

            return bRet ;
        }
        #endregion

    }
}
