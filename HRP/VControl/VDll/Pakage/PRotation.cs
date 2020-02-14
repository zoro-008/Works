using COMMON;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VDll.Pakage
{
    class PRotation: CPkgObject , IPkg
    {
        readonly TProp Prop = new TProp{bImgGrabber = false ,
                                        bHaveDialog = false ,
                                        bHaveImage  = true  ,
                                        bHavePosRef = false };

        public class CMstPara 
        {           
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Image"    )] public string   PkgNameOfImage    {get;set;} public IPkg PkgImage ;                

            public IPkg PkgNext;
        }        

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUsrPara 
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Angle"            )] public uint         Angle          {get;set;}                          
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

        Mat RunImg   = new Mat();

        class TRslt
        {
            public int WorkTimems ;
        }
        TRslt Rslt = new TRslt();

        public PRotation(string _sName , int _iVisionID):base(_sName , _iVisionID)
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
            return RunImg ;            
        }

        //public Mat GetTrainFormImage ()
        //{
        //    //if (MorphImg == null) return null;
        //    return RunImg  ;
        //}
        //
        //public Mat GetTrainImage ()
        //{
        //    return TrainImg ;      
        //}

        public TPos GetPos()
        {
            return Pos ;
        }

        public bool Train()
        {
            
            return true ;
        }

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
            
            if(RunImg != null)  RunImg.Dispose();
            RunImg = MstPara.PkgImage.GetImage().Clone();

            if (RunImg.Width  <= Rect.Right ) {sError = "Tracker Right is out of image"; return false; }
            if (RunImg.Height <= Rect.Bottom) {sError = "Tracker Bottom is out of image"; return false; }


            try
            {
                using (Mat RotMat = new Mat())
                using (Mat Roi = new Mat(RunImg, Rect))
                {
                    lock (GV.ImageLock[iVisionID]) //lock (Rslt.Peaks)
                    {
                        PointF pt = new PointF((float)(Roi.Cols / 2.0), (float)(Roi.Rows / 2.0));                        
                        CvInvoke.GetRotationMatrix2D(pt, UsrPara.Angle, 1.0, RotMat);
                        CvInvoke.WarpAffine(Roi, Roi, RotMat, new Size(Roi.Cols, Roi.Rows));
                    }
                }
            }
            catch (Exception _e)
            {
                sError = _e.Message ;
                WorkTime.Stop();
                Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds ;
                return false ;
            }

            WorkTime.Stop();
            Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds ;

            //OK NG에 따라 분기문....
            _tRet.bEnded  = true ;
            _tRet.NextPkg = null ;

            _tRet.NextPkg = MstPara.PkgNext;

            return true ;
        }


        public bool Paint(Graphics _g , bool _bTrain)
        {
            if(tScaleOffset.fScaleX == 0 && tScaleOffset.fScaleY ==0)
            {
                SetFitScaleOffset(_g.ClipBounds.Width , _g.ClipBounds.Height , GetImage().Width , GetImage().Height);
            }

            return PaintScOf(_g , _bTrain , tScaleOffset);
            
        }

        public bool PaintScOf  (Graphics _g , bool _bTrain , TScaleOffset _ScaleOffset)
        {
            try
            {
                RunImg.Paint(_g, tScaleOffset);
            }
            catch (Exception _e)
            {
                sError = _e.ToString();
                return false;
            }

            if (_bTrain)
            {
                foreach (CTracker Tracker in lsTracker)
                {
                    Tracker.Paint(_g, tScaleOffset);
                }
                _g.DrawString("MultiProfile WorkTime : " + Rslt.WorkTimems.ToString(), new Font("헤움봄비152", 20), Brushes.Yellow, 0, 0);
            }

            return true ;
        }

        //public bool PaintTrain (Graphics _g)
        //{
        //    if (!Paint(_g)) return false;
        //
        //    
        //    return true;
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
                        RunImg = CvInvoke.Imread(MorphImgPath, ImreadModes.Unchanged);
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
                if(RunImg  != null)RunImg .Save(MorphImgPath );
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
