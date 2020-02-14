using COMMON;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Emgu.CV.CvEnum;

namespace VDll.Pakage
{
    class PMultiProfile : CPkgObject , IPkg
    {
        readonly TProp Prop = new TProp{bHaveTrain  = true  ,
                                        bHaveDialog = false ,
                                        bHaveImage  = true  ,
                                        bHavePosRef = false };
                  
        public class CMstPara 
        {           
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Image"    )] public string   PkgNameOfImage    {get;set;} public IPkg PkgImage ;                
            [CategoryAttribute("MPara"), DisplayNameAttribute("Process Count"       )] public int      ProcessCount      {get;set;} //토탈 이 PKG를 몇번을 수행 하는 건지에 대한 갯수.
            [CategoryAttribute("MPara"), DisplayNameAttribute("Image Copy Rows"     )] public int      ImageCopyRows     {get;set;} //한번 프로파일 이미지화 할때 몇줄을 채워넣을건지.

            public IPkg PkgNext;
        }        

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUsrPara 
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Threshold"           )] public uint         Threshold         {get;set;}        
        }

        private CMultiObjectSelector.CustomObjectType oRet;

        //저장 값들....
        CMstPara MstPara = new CMstPara();
        CUsrPara UsrPara = new CUsrPara(); 
        Object   AlgPara ;                 

        //====================================
        List<CTracker> lsTracker = new List<CTracker>();
        TPos Pos = new TPos{dRefX   = 0.0 , //카메라는 기준포지션 없음.
                            dRefY   = 0.0 ,
                            dRefT   = 0.0 ,
                            dInspX  = 0.0 ,
                            dInspY  = 0.0 ,
                            dInspT  = 0.0 };

        Mat HeightImg = new Mat();
        Mat TrainImg  = new Mat();
        //byte[] Zeros = new byte[1]; //높이 기준점. 트레인시 세팅.

        int iProcessCnt = 0 ;

        class TRslt
        {
            //public List<PointF> Peaks = new List<PointF>();
            public PointF[]     Peaks = new PointF[1];
            public int          WorkTimems ;
        }
        TRslt Rslt = new TRslt();

        public PMultiProfile(string _sName , int _iVisionID):base(_sName , _iVisionID)
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
            AlgPara = Activator.CreateInstance(typeof(Algorithm.CPeak.CUPara));

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
                if(oRet == null)
                {
                    List<CCustomProperty> lsPara = new List<CCustomProperty>();
                    CMultiObjectSelector.GetData(ref AlgPara, ref lsPara , "Peak" );
                    CMultiObjectSelector.GetData(ref UsrPara, ref lsPara , "User" );

                    oRet = new CMultiObjectSelector.CustomObjectType { props = lsPara };
                }
                return oRet;
            }
            set {
                CMultiObjectSelector.SetProperty(ref AlgPara, value , "Peak");
                CMultiObjectSelector.SetProperty(ref UsrPara, value , "User");
            }
        }

        public string GetName ()  {return sName;  }
        public string GetError()  {return sError; }

        //트레커 외부에서 클릭 처리 해야함.
        public List<CTracker> Trackers {get { return lsTracker ;}}
        
        public Mat GetImage ()
        {
            //return HeightImg;
            //if (GV.Trainings[iVisionID])
            //{
            //    if (MstPara.PkgImage == null) return null;
            //    return MstPara.PkgImage.GetImage()  ;            
            //}
            return HeightImg;            
        }

        public Mat GetTrainFormImage ()
        {
            if (MstPara.PkgImage == null) return null;
            return MstPara.PkgImage.GetTrainImage()  ;
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
            if(HeightImg == null) 
            {
                sError = "HeightImg is Null";
                return false ;
            }

            if(TrainImg != null){
                TrainImg.Dispose(); 
                TrainImg = null ;
            }
            lock(GV.ImageLock[iVisionID])
            {
                TrainImg = HeightImg.Clone();
            }

            return true ;       
        }

        
        //public void SetPixelMat(Mat mat, int x, int y, int cols, int elementSize, byte value)
        //{
        //    int c = cols;//mat.Cols ;
        //    int e = elementSize;//mat.ElementSize ;
        //    unsafe
        //    {
        //        byte* Data = (byte *)mat.DataPointer;               
        //        *(Data + (y * c + x) * e) = value;
        //    }
        //}

        public bool Run(TRunPara _tPara , ref TRunRet _tRet)
        {
            if (MstPara.PkgImage == null) return false;
            
            _tRet.bEnded = false ;
            _tRet.NextPkg = this ;
            sError = "";

            Stopwatch WorkTime = new Stopwatch() ;
            //WorkTime.Reset();
            WorkTime.Restart();

            //이미지 프로세싱.
            Rectangle Rect = lsTracker[0].GetRectangle();
            Mat Img ;

            if (GV.Trainings[iVisionID])
            {
                Img = MstPara.PkgImage.GetTrainImage();

                if (Img.Width <= Rect.Right) { sError = "Tracker Right is out of image"; return false; }
                if (Img.Height <= Rect.Bottom) { sError = "Tracker Bottom is out of image"; return false; }

                
            }
            else 
            {
                Img = MstPara.PkgImage.GetImage();

                if (Img.Width <= Rect.Right) { sError = "Tracker Right is out of image"; return false; }
                if (Img.Height <= Rect.Bottom) { sError = "Tracker Bottom is out of image"; return false; }
            }
            Img = new Mat(Img, Rect);


            int iWidth  = Img.Width;
            int iHeight = Img.Height-1;

            lock(GV.ImageLock[iVisionID]) //lock (Rslt.Peaks)
            {
                Rslt.Peaks = new PointF[iWidth];                
                Algorithm.CPeak.CUPara  PeakPara = AlgPara as Algorithm.CPeak.CUPara;
                Algorithm.CPeak.TRslt[] PeakRslt = new Algorithm.CPeak.TRslt[iWidth];
                float fTrackerHeight = lsTracker[0].GetRectangle().Height;
                float fTrackerRatio = 256f / fTrackerHeight ;
                
                if(fTrackerHeight > 0) 
                {
                    Parallel.For(0, iWidth, x =>
                    //for(int x = 0 ; x < iWidth ; x++)
                    {
                        if (!Algorithm.CPeak.FindPeakVertical(Img, x, 0, iHeight, PeakPara, ref PeakRslt[x]))
                        {
                            sError = PeakRslt[x].sError;
                            //return false ;
                        }
                        Rslt.Peaks[x].X = Rect.Left + x;
                        if (PeakRslt[x].fPos != 0)
                        {
                            Rslt.Peaks[x].Y = Rect.Top + PeakRslt[x].fPos;             
                        }
                        else
                        {
                            Rslt.Peaks[x].Y = 0;
                        }
                    
                        int   y = HeightImg.Height - (iProcessCnt + 1) * MstPara.ImageCopyRows;
                        float fHeightValue = (fTrackerHeight - PeakRslt[x].fPos) * fTrackerRatio;
                        if(MstPara.ProcessCount > iProcessCnt && !GV.Trainings[iVisionID])
                        {                        
                            for (int i = 0; i < MstPara.ImageCopyRows; i++)
                            {                            
                                HeightImg.SetValue(x, y +i, (byte)fHeightValue);
                            }
                        }
                    });
                    //}
                }
            }
            WorkTime.Stop();
            Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds ;

            //OK NG에 따라 분기문....
            _tRet.bEnded  = true ;
            _tRet.NextPkg = null ;

            _tRet.NextPkg = MstPara.PkgNext;

            iProcessCnt++;

            //if (MstPara.ProcessCount > iProcessCnt)
            //{

            //}

            return true ;
        }


        //GetImage() 에서 가져오는 이미지가 항상 쓰레드안전하지 않은 상태이므로 
        //모든 상황을 잘 따져보자.. GetImage 할때 버벅일 수 있음.
        public bool Paint(Graphics _g , bool _bTrain)// , TScaleOffset _ScaleOffset)//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            try 
            {
                int iRsltCnt = Rslt.Peaks.Length ;
                if (iRsltCnt <= 1)
                {
                    sError = "Result Count is under 2";
                    return false;
                }

                if (tScaleOffset.fScaleX == 0 && tScaleOffset.fScaleY == 0)
                {
                    SetFitScaleOffset(_g.ClipBounds.Width, _g.ClipBounds.Height, MstPara.PkgImage.GetImage().Width, MstPara.PkgImage.GetImage().Height);
                }

                using (Pen pen = new Pen(Color.Yellow, 1))
                {


                    pen.Color = Color.Aqua;
                    PointF ScaledPeak1 = new PointF(0, 0);
                    PointF ScaledPeak2 = new PointF(0, 0);
                    for (int i = 1; i < iRsltCnt; i++)
                    {
                        if (Rslt.Peaks[i].Y == 0.0 || Rslt.Peaks[i - 1].Y == 0.0) continue;
                        ScaledPeak2.X = ((Rslt.Peaks[i].X - tScaleOffset.fOffsetX) * tScaleOffset.fScaleX);
                        ScaledPeak2.Y = ((Rslt.Peaks[i].Y - tScaleOffset.fOffsetY) * tScaleOffset.fScaleY);

                        ScaledPeak1.X = ((Rslt.Peaks[i - 1].X - tScaleOffset.fOffsetX) * tScaleOffset.fScaleX);
                        ScaledPeak1.Y = ((Rslt.Peaks[i - 1].Y - tScaleOffset.fOffsetY) * tScaleOffset.fScaleY);
                        _g.DrawLine(pen, ScaledPeak1, ScaledPeak2);
                    }
                }
                
            }
            catch(Exception _e)
            {
                sError = _e.ToString();
                return false ;
            }

            if (_bTrain)
            {
                foreach (CTracker Tracker in lsTracker)
                {
                    //_g.DrawRectangle(pen , Tracker.GetRectangle());
                    Tracker.Paint(_g, tScaleOffset);
                }
                _g.DrawString("MultiProfile WorkTime : " + Rslt.WorkTimems.ToString(), new Font("헤움봄비152", 20), Brushes.Yellow, 0, 0);
            }

            return true ;
        }

        //public bool PaintTrain (Graphics _g)
        //{
        //    if(!Paint(_g)) return false ;
        //
        //    
        //
        //    _g.DrawString("MultiProfile WorkTime : " + Rslt.WorkTimems.ToString() , new Font("헤움봄비152", 20), Brushes.Yellow, 0, 0);
        //    return true ;
        //
        //}

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
            //0으로 초기화 하는 것 찾아보자.
            //ImgHeight.
            iProcessCnt = 0;
            return true;
        }

        //오토런중에 뭔가 미리 준비 해야 될 것들 있으면 한다.
        public bool Ready()
        {
            if (MstPara.PkgImage != null)
            {
                if (HeightImg.Height != MstPara.ProcessCount*MstPara.ImageCopyRows || HeightImg.Width != lsTracker[0].GetRectangle().Width)
                {
                    HeightImg.Dispose();
                    HeightImg = null;

                    Size size = new Size(lsTracker[0].GetRectangle().Width, MstPara.ProcessCount * MstPara.ImageCopyRows);
                    //ImgHeight = new Mat(MstPara.PkgImage.GetImage() , lsTracker[0].GetRectangle()).Clone();
                    HeightImg = new Mat(MstPara.ProcessCount * MstPara.ImageCopyRows, 
                                        lsTracker[0].GetRectangle().Width,
                                        Emgu.CV.CvEnum.DepthType.Cv8U ,
                                        1);
                }

                //일단 스킵. 그냥 트레커 밑단 기준으로 하자.
                //if (Zeros.Length != lsTracker[0].GetRectangle().Width)
                //{
                //    Zeros = new byte[lsTracker[0].GetRectangle().Width];
                //}
            }

            iProcessCnt = 0;
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
            string FilePath      = _sPath+"\\"+sName+"_"+GetType().Name +".ini";
            string HeightImgPath = _sPath+"\\"+sName+"_"+GetType().Name +"_HeightImg.bmp";
            string TrainImgPath  = _sPath+"\\"+sName+"_"+GetType().Name +"_TrainImg.bmp";

            if (_bLoad)
            {
                CAutoIniFile.LoadStruct(FilePath,"MasterPara",ref MstPara ); 
                CAutoIniFile.LoadStruct(FilePath,"UserPara"  ,ref UsrPara ); 
                CAutoIniFile.LoadStruct(FilePath,"AlgoPara"  ,ref AlgPara ); 

                if(File.Exists(HeightImgPath ))
                {
                    try
                    {
                        HeightImg = CvInvoke.Imread(HeightImgPath, ImreadModes.Unchanged);
                    }
                    catch(Exception _e)
                    {
                        sError = _e.ToString() ;
                        bRet =  false ;
                    }
                }

                if(File.Exists(TrainImgPath ))
                {
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
                CAutoIniFile.SaveStruct(FilePath,"AlgoPara"  ,ref AlgPara);   
                if(HeightImg  != null)HeightImg .Save(HeightImgPath );
                if(TrainImg   != null)TrainImg  .Save(TrainImgPath  );
            }

            CIniFile Ini = new CIniFile(FilePath);
            foreach (CTracker Tracker in lsTracker)
            {
                Tracker.LoadSave(_bLoad, "Tracker1" , Ini);
            }

            //Apply
            MstPara.PkgImage = null ;
            MstPara.PkgNext = null;
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
