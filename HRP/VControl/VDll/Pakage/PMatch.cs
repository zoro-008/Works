using COMMON;
using Emgu.CV;
using Emgu.CV.CvEnum;
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
using VDll.Algorithm;
//using VDll.Algorithm;

namespace VDll.Pakage
{
    public class PMatch : CPkgObject , IPkg
    {
        readonly TProp Prop = new TProp{bImgGrabber = false ,
                                        bHaveDialog = true  ,
                                        bHaveImage  = false ,
                                        bHavePosRef = true  };

        public enum EMatchMode
        {
            Shape     = 0 ,
            Template      
        }

        public class CMstPara 
        {
            [CategoryAttribute("MPara"), DisplayNameAttribute("Match Mode"          )] public EMatchMode   MatchMode         {get;set;} = EMatchMode.Shape ;
            [CategoryAttribute("MPara"), DisplayNameAttribute("Dual Mode"           )] public bool         DualMode          {get;set;} = false ;
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Image"    )] public string       PkgNameOfImage    {get;set;} public IPkg PkgImage ;            
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Pos"      )] public string       PkgNameOfPos      {get;set;} public IPkg PkgPos   ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Ok Result")] public string       PkgNameOfOk       {get;set;} public IPkg PkgOk    ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Ng Result")] public string       PkgNameOfNg       {get;set;} public IPkg PkgNg    ; 
        }        

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUsrPara 
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Inspection Area X"   )] public uint         InspectionAreaX  {get;set;} = 10 ;
            [CategoryAttribute("UPara"), DisplayNameAttribute("Inspection Area Y"   )] public uint         InspectionAreaY  {get;set;} = 10 ;
        }

        //UI에서 여러파라들을 한번에 보여줄수 있게 하는것.
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

        Mat TemplateImg1 = new Mat();

        public PMatch(string _sName , int _iVisionID):base(_sName , _iVisionID)
        {
            //현재 세팅 패키지와 가장 가까운 이미지를 가지고 있는 PKG로 초기화 세팅.
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                TProp Prop =  Pkg.GetProp() ;
                if(Prop.bHaveImage) MstPara.PkgNameOfImage = Pkg.GetName() ;
            }
        }

        class TRslt
        {
            //public List<PointF> Peaks = new List<PointF>();
            public double  dCntX  ;
            public double  dCntY  ;
            public double  dLeft  ;
            public double  dTop   ;
            public double  dWidth ;
            public double  dHeight;
            public double  dScore ;
            public double  dAngle ;

            public int     WorkTimems ;
        }
        TRslt Rslt = new TRslt();

        #region IPkg
        public bool Init()
        {
            CTracker Tracker1 = new CTracker();
            Tracker1.Init();
            Tracker1.caption = "Main" ;
            lsTracker.Add(Tracker1);

            CTracker Tracker2 = new CTracker();
            Tracker2.Init();
            Tracker2.caption = "Sub" ;
            Tracker2.visible = false ;
            lsTracker.Add(Tracker2);

            //나중에 알고리즘 바꿀때는 AlgPara = Activator.CreateInstance( /*여기 생각좀 하자...*/); //sun
            //AlgPara = Activator.CreateInstance(typeof(CTempMatch.CUPara));

            //처음엔 그냥 TempPara로 생성.
            SetAlgPara(typeof(CTempMatch.CUPara));

            return true ;
        }

        public void SetAlgPara(Type _AlgoType)
        {
            bool bNewAlgoPara = false ;
            if(AlgPara == null || AlgPara.GetType() != _AlgoType)
            {
                AlgPara = Activator.CreateInstance(_AlgoType);
                bNewAlgoPara = true ;
            }

            if(oRet == null || bNewAlgoPara) //처음 이거나 알고파라가 새로 할당된경우.
            {
                List<CCustomProperty> lsPara = new List<CCustomProperty>();
                CMultiObjectSelector.GetData(ref AlgPara, ref lsPara , "Algo");
                CMultiObjectSelector.GetData(ref UsrPara, ref lsPara , "User");
                
                oRet = new CMultiObjectSelector.CustomObjectType { props = lsPara };
            }          
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
            get 
            {
                //if(oRet == null)
                //{
                //    List<CCustomProperty> lsPara = new List<CCustomProperty>();
                //    CMultiObjectSelector.GetData(ref AlgPara, ref lsPara , "Algo");
                //    CMultiObjectSelector.GetData(ref UsrPara, ref lsPara , "User");
                //
                //    oRet = new CMultiObjectSelector.CustomObjectType { props = lsPara };
                //}
                return oRet;
            }
            set 
            {
                CMultiObjectSelector.SetProperty(ref AlgPara, value , "Algo");
                CMultiObjectSelector.SetProperty(ref UsrPara, value , "User");
            }
        }

        public string GetName ()  {return sName;  }
        public string GetError()  {return sError; }

        //트레커 외부에서 클릭 처리 해야함.
        public List<CTracker> Trackers {get { return lsTracker ;}}        
        public Mat GetImage ()
        {
            if (MstPara.PkgImage == null) return null;
            return MstPara.PkgImage.GetImage()  ;
        }

        public TPos GetPos()
        {
            return Pos ;
        }

        public bool Train()
        {
            if(GetImage() == null) 
            {
                sError = "Image is Null";
                return false ;
            }

            if (TemplateImg1 != null)
            {
                TemplateImg1.Dispose();
                TemplateImg1 = null ;
            }
            try
            {
                TemplateImg1 = new Mat(GetImage(),lsTracker[0].GetRectangle()).Clone();
                CvInvoke.Imshow("Template", TemplateImg1);
            }
            catch(Exception _e)
            {
                sError = _e.Message ;
                return false ;
            }

            return true ;           
        }
        public bool Run(TRunPara _tPara , ref TRunRet _tRet)
        {
            _tRet.bEnded  = false ;
            _tRet.NextPkg = this  ;
            sError = "";

            Stopwatch WorkTime = new Stopwatch() ;
            //WorkTime.Reset();
            WorkTime.Restart();

            //이미지 프로세싱.
            Rectangle Rect = lsTracker[0].GetRectangle();
            Mat Img = GetImage();
            if(Img == null) {sError = "PkgImage is null"; return false; }
            if (Img.Width  <= Rect.Right ) {sError = "Tracker Right is out of image"; return false; }
            if (Img.Height <= Rect.Bottom) {sError = "Tracker Bottom is out of image"; return false; }            
            
            Rect.X -= (int)UsrPara.InspectionAreaX ;
            Rect.Y -= (int)UsrPara.InspectionAreaY ;
            Rect.Width  += (int)(UsrPara.InspectionAreaX * 2) ;
            Rect.Height += (int)(UsrPara.InspectionAreaY * 2) ;

            Img = new Mat(GetImage() , Rect);

            int iWidth  = Img.Width;
            int iHeight = Img.Height-1;

            
            lock(GV.ImageLock[iVisionID]) //lock (Rslt.Peaks)
            {
                if (MstPara.MatchMode == EMatchMode.Template)
                {
                    CTempMatch.TRslt TempRslt = new CTempMatch.TRslt() ;
                    try
                    {
                        if (!CTempMatch.Find(Img, TemplateImg1, AlgPara as CTempMatch.CUPara, ref TempRslt))
                        {
                            sError = "FindMatch was Failed";
                            WorkTime.Stop();
                            Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds;
                            return false;
                        }
                    }
                    catch (Exception _e)
                    {
                        sError = _e.Message;
                        WorkTime.Stop();
                        Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds;
                        return false;
                    }
                    WorkTime.Stop();
                    Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds;

                    Rslt.dLeft = Rect.Left + TempRslt.dX;
                    Rslt.dTop = Rect.Top + TempRslt.dY;
                    Rslt.dWidth = TemplateImg1.Width;
                    Rslt.dHeight = TemplateImg1.Height;
                    Rslt.dCntX = Rect.Left + TemplateImg1.Width / 2.0;
                    Rslt.dCntY = Rect.Right + TemplateImg1.Height / 2.0;
                    Rslt.dScore = TempRslt.dScore;
                    Rslt.dAngle = TempRslt.dImgAngle;
                }
                else if (MstPara.MatchMode == EMatchMode.Shape)
                {
                    CShapeMatch.TRslt TempRslt = new CShapeMatch.TRslt() ;
                    try
                    {
                        
                        if (!CShapeMatch.Find(Img, TemplateImg1, AlgPara as CShapeMatch.CUPara, ref TempRslt , out Mat Result))
                        {
                            sError = "FindMatch was Failed";
                            WorkTime.Stop();
                            Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds;
                            return false;
                        }

                        CvInvoke.Imshow("Result", Result);

                    }
                    catch (Exception _e)
                    {
                        sError = _e.Message;
                        WorkTime.Stop();
                        Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds;
                        return false;
                    }
                    WorkTime.Stop();
                    Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds;

                    Rslt.dLeft = Rect.Left + TempRslt.dX;
                    Rslt.dTop = Rect.Top + TempRslt.dY;
                    Rslt.dWidth = TemplateImg1.Width;
                    Rslt.dHeight = TemplateImg1.Height;
                    Rslt.dCntX = Rect.Left + TemplateImg1.Width / 2.0;
                    Rslt.dCntY = Rect.Right + TemplateImg1.Height / 2.0;
                    Rslt.dScore = TempRslt.dScore;
                    Rslt.dAngle = TempRslt.dImgAngle;
                }
            }
            

            //이미지 프로세싱.
            _tRet.bEnded  = true ;
            _tRet.NextPkg = null ;
            for(int i = 0 ; i < GV.Visions[iVisionID].Pkgs.Count ; i++)
            {
                if(GV.Visions[iVisionID].Pkgs[i] == this)
                {
                    if(GV.Visions[iVisionID].Pkgs.Count > i+1)
                    {
                        _tRet.NextPkg = GV.Visions[iVisionID].Pkgs[i+1];
                    }
                }
            }

            GV.GlobalTables.AddValue("")

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
                if (_bTrain)GetImage().Paint(_g, _ScaleOffset);//나중에 Paint 없애고 _g.TranslateTransform() _g.ScaleTransform() 이용하여 해보자.
            
                using(Pen pen = new Pen(Color.Yellow , 1 ))
                {
                    

                    pen.Color = Color.Aqua ;

                    //ScaledPeak2.X = ((Rslt.Peaks[i  ].X - tScaleOffset.fOffsetX) * tScaleOffset.fScaleX);
                    //ScaledPeak2.Y = ((Rslt.Peaks[i  ].Y - tScaleOffset.fOffsetY) * tScaleOffset.fScaleY);

                    Rectangle Rect = new Rectangle();
                    Rect.X     = (int)((Rslt.dLeft - _ScaleOffset.fOffsetX) * _ScaleOffset.fScaleX);  
                    Rect.Y     = (int)((Rslt.dTop  - _ScaleOffset.fOffsetY) * _ScaleOffset.fScaleY);  
                    Rect.Width = (int)( Rslt.dWidth  * _ScaleOffset.fScaleX                         );  
                    Rect.Height= (int)( Rslt.dHeight * _ScaleOffset.fScaleY                         );  
                    _g.DrawRectangle(pen,Rect);
                    
                    //Brush brush = new Brush();//= new Brush() ;
                    //using(Brush brush = )
                    using (Font font = new Font("Arial", 10))
                    using (SolidBrush brush = new SolidBrush(Color.Red))
                    {
                        _g.DrawString("Time:"  + Rslt.WorkTimems.ToString() +"ms", font , brush , new PointF((float)Rect.X ,(float)(Rect.Y-font.Height-1)));
                        _g.DrawString("Score:" + Rslt.dScore.ToString()          , font , brush , new PointF((float)Rect.X ,(float)(Rect.Y+Rect.Height)));
                        _g.DrawString("Angle:" + Rslt.dAngle.ToString()          , font , brush , new PointF((float)Rect.X ,(float)(Rect.Y+Rect.Height+font.Height+1)));
                    }
                    //_g.DrawLine(pen , )
                }
            }
            catch (Exception _e)
            {
                sError = _e.ToString();
                return false;
            }

            if (_bTrain)
            {
                using (Pen pen = new Pen(Color.Yellow, 1))
                {
                    foreach (CTracker Tracker in lsTracker)
                    {
                        if (Tracker.visible) //Tracker ROI 그리기.
                        {
                            Rectangle Rect = Tracker.GetRectangle();
                            Rectangle Roi = new Rectangle();
                            Roi.X = (int)(Rect.Left - UsrPara.InspectionAreaX);
                            Roi.Y = (int)(Rect.Top - UsrPara.InspectionAreaY);
                            Roi.Width = (int)(Rect.Width + UsrPara.InspectionAreaX * 2);
                            Roi.Height = (int)(Rect.Height + UsrPara.InspectionAreaY * 2);

                            Roi.X     = (int)((Roi.X - tScaleOffset.fOffsetX) * tScaleOffset.fScaleX);  
                            Roi.Y     = (int)((Roi.Y  - tScaleOffset.fOffsetY) * tScaleOffset.fScaleY);  
                            Roi.Width = (int)( Roi.Width  * tScaleOffset.fScaleX                         );  
                            Roi.Height= (int)( Roi.Height * tScaleOffset.fScaleY                         );  

                            _g.DrawRectangle(pen, Roi);
                            

                        }

                        //_g.DrawRectangle(pen , Tracker.GetRectangle());
                        Tracker.Paint(_g, tScaleOffset);
                    }
                }
            }



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
            bool bRlst = false ;
            bool bRet  = false ;
            foreach (CTracker Tracker in lsTracker)
            {
                bRlst = Tracker.MouseDown(Control.ModifierKeys, _e, tScaleOffset);
                if (bRlst == true) bRet = true;
            }
            return bRet ;
        }
        public bool MouseMove(MouseEventArgs _e)
        {
            bool bRlst = false;
            bool bRet = false;
            foreach (CTracker Tracker in lsTracker)
            {
                bRlst = Tracker.MouseMove(Control.ModifierKeys, _e, tScaleOffset);
                if (bRlst == true) bRet = true;
            }
            return bRet;
        }
        public bool MouseUp  (MouseEventArgs _e)
        {
            bool bRlst = false;
            bool bRet = false;
            foreach (CTracker Tracker in lsTracker)
            {
                bRlst = Tracker.MouseUp(Control.ModifierKeys, _e);
                if (bRlst == true) bRet = true;
            }
            return bRet;
        }

        //VisionJobFile//VisionName//PkgName 폴더까지 들어와야함.
        //한비전단위로 로드세이브 되기에 세이브시에 PKG이름이나 갯수 변경사항 찌꺼기 남는 것을 방지 하기 위해.
        //해당 비전을 한번 다 지우고 해야 함.
        public bool LoadSave (bool _bLoad , string _sPath)
        {
            bool bRet = true ;
            string FilePath = _sPath+"\\"+sName+"_"+GetType().Name +".ini";
            string TrainImgPath     = _sPath+"\\TrainImg.bmp";
            string TemplateImgPath  = _sPath+"\\TemplateImg.bmp";

            //마스터 옵션.
            if(_bLoad)
            {
                CAutoIniFile.LoadStruct(FilePath,"MasterPara",ref MstPara );
            }
            else
            {
                CAutoIniFile.SaveStruct(FilePath,"MasterPara",ref MstPara); 
            }

            //마스터 옵션에 따라.
            if (MstPara.MatchMode == EMatchMode.Template)
            {
                SetAlgPara(typeof(CTempMatch.CUPara));
            }
            else if(MstPara.MatchMode == EMatchMode.Shape)
            {
                SetAlgPara(typeof(CShapeMatch.CUPara));
            }
            else
            {
                sError = "MstPara.MatchMode is out of range.";
                bRet = false ;
            }


            //나머지.
            if (_bLoad)
            {
                CAutoIniFile.LoadStruct(FilePath,"UserPara"  ,ref UsrPara ); 
                CAutoIniFile.LoadStruct(FilePath,"AlgoPara"  ,ref AlgPara ); 

                try
                {
                    TemplateImg1 = new Mat(TemplateImgPath , ImreadModes.Unchanged);
                }
                catch (Exception _e)
                {
                    sError = _e.ToString();
                    bRet = false;
                }
            }
            else
            {
                //세이브 할때 트레인도 같이함. 나중에 마스크 설정 하는것 Setting버트으로 창띄워서 할껀데 Setting 버튼 누를때도 Train해야함.
                Train();

                //Vision에서 폴더 지움.if(File.Exists(_sPath)) File.Delete(_sPath);
                CAutoIniFile.SaveStruct(FilePath,"UserPara"  ,ref UsrPara);
                CAutoIniFile.SaveStruct(FilePath,"AlgoPara"  ,ref AlgPara); 
                try
                {
                    TemplateImg1.Save(TemplateImgPath );
                }
                catch (Exception _e)
                {
                    sError = _e.ToString();
                    bRet = false ;
                }
            }

            //Tracker Load Save.
            CIniFile Ini = new CIniFile(FilePath);
            foreach (CTracker Tracker in lsTracker)
            {
                Tracker.LoadSave(_bLoad, Tracker.caption , Ini);
            }


            //PKG Link
            MstPara.PkgImage = null ;
            MstPara.PkgPos   = null ;
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                if(Pkg.GetName() == MstPara.PkgNameOfImage ) MstPara.PkgImage = Pkg ;
                if(Pkg.GetName() == MstPara.PkgNameOfPos   ) MstPara.PkgPos   = Pkg ;
            }
            return bRet ;
        }
        #endregion
    }
}
