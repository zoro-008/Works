using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using COMMON;
using System.Threading;

namespace Machine
{
    public partial class FormVision : Form
    {
        private const string sFormText = "FormVision";
        public static bool bLive = false ;

        //이미지 핸들링 관련.
        Mat DispImg ;
        public Mat GetDispImg()
        {
            return DispImg ;
        }
        private readonly object DispImgLock = new object();

        //녹화관련.
        private VideoWriter OpenCV_video ;
        //private CMultimidiaTimer RecTimer = new CMultimidiaTimer();
        

        

        struct TStat {
            public bool bImgClick    ; //이미지 이동.
            public int  iClickX      ; //트렉커 클릭X
            public int  iClickY      ; //트렉커 클릭Y
        };
        
        TStat Stat = new TStat { bImgClick = false, iClickX = 0, iClickY = 0 };

        public struct TScaleOffset
        {
            public float fScaleX;
            public float fScaleY;

            public float fOffsetX;
            public float fOffsetY;
        }
        TScaleOffset ScaleOffset;


        public FormVision()
        {
            InitializeComponent();
            ibCam.SizeMode = PictureBoxSizeMode.Normal ;
            
            
            //sun 뻑나는데 최상위 어쩌구 저쩌구.
            //FrmTrain.Parent = this ;

            ibCam.MouseWheel += new MouseEventHandler(ibCam_MouseWheel);

            if(SEQ.bCamInit)SEQ.Cam.SetGrabFunc(GrabCallback);

            ThreadRecorder.Init();
            
            //RecTimer.Tick += tmRec_Tick ;
        }

        private void FormVision_FormClosing(object sender, FormClosingEventArgs e)
        {
            bLive = false ;
            Thread.Sleep(100); //페인트 뻑 방지.
            ThreadRecorder.Close();
        }

        struct TThreadPara
        {
            public Mat Img ;
            public string File ;
            public int    Index ;
        }

        int iGrabCnt  = 0 ;
        int iSavedIdx = 0 ;

        bool GrabCallback(int _iWidth , int _iHeight , int _iBit , EPixelFormat _ePxFormat , IntPtr _pImage )
        {
            
            

           // return true ;
            double dStart = CTimer.GetTime_us();
            int iStride = ((_iWidth * _iBit / 8 ) /4) * 4 ;
            if(((_iWidth * _iBit) /4) % 4 != 0){
                iStride++;
            }

            if(_ePxFormat != EPixelFormat.BayerRG8)
            {
                return false ;
            }

            using (Mat Img = new Emgu.CV.Mat(_iHeight, _iWidth, Emgu.CV.CvEnum.DepthType.Cv8U, 1, _pImage, iStride))
            {

                //lock (DispImgLock)
                {
                    bUsingDispImg = true ;
                    DispImg = new Emgu.CV.Mat(_iHeight, _iWidth, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
                    //CvInvoke.GetRotationMatrix2D(new Point2f(src.Width / 2, src.Height / 2), degree, 1);
                    //Img.Bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY);

                    try
                    {
                        Emgu.CV.CvInvoke.CvtColor(Img, DispImg, Emgu.CV.CvEnum.ColorConversion.BayerBg2Bgr); //존나 빠르군...

                        //2019.10.23
                        if (OM.Info.bImageSave)
                        {
                            iSavedIdx++;

                            if(iSavedIdx >= OM.Info.iImageSaveCnt)
                            { 
                                OM.Info.iImageSave++;

                                TThreadPara Para = new TThreadPara();
                                Para.File  = "D:\\Image\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + OM.Info.sImageFileName + "_" + OM.Info.iImageSave.ToString() + ".bmp" ;
                                Para.Img   = DispImg.Clone() ;
                                Para.Index = iSavedIdx ;

                                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(Para.File));
                                if(!di.Exists) di.Create();
                                iSavedIdx = 0;
                                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveImage) , Para);
                            }

                            
                        }
                        //DispImg.Bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY);
                    }
                    catch
                    {
                        bUsingDispImg = false ;
                        return false;
                    }
                    bUsingDispImg = false ;
                }
            }
            

            iGrabCnt++;
            if(iGrabCnt > 999999999) iGrabCnt = 0 ;

            PaintInvoke();

            double dEnd = CTimer.GetTime_us();
            double dGab = (dEnd - dStart)/1000.0 ; //대략 5ms 이하.

            if (bLive) {
            //    Delay(100);
                SEQ.Cam.Grab();
            }
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

        public static void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }

        bool bUsingDispImg = false ;
        private void ibCam_Paint(object sender, PaintEventArgs e)
        {
            if (DispImg == null) return;
            
            

            //lbGrabTime .Text = GrabTime.ElapsedMilliseconds.ToString() ;
            //GrabTime.Restart();
            Graphics g = e.Graphics ;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode   = PixelOffsetMode.Half ;

            


            try
            {
                if(ScaleOffset.fScaleX == 0 && ScaleOffset.fScaleY ==0)
                {
                    SetFitScaleOffset(g.ClipBounds.Width , g.ClipBounds.Height , DispImg.Width , DispImg.Height);
                }

                //lock(DispImgLock)

                {
                    bUsingDispImg = true ;
                    PaintMat(DispImg , g, ScaleOffset);
                    bUsingDispImg = false ;
                }
                
            }
            catch (Exception _e)
            {
                //sError = _e.ToString();
                bUsingDispImg = false ;
                return ;
            }

            //using (Font font = new Font("Arial", 10))
            //using (SolidBrush brush = new SolidBrush(Color.Purple))
            //{
            //    brush.Color = Color.Purple;
            //    string sFrame = SEQ.Cam.GetFrameFrq().ToString() ;
            //    g.DrawString(sFrame , font, brush, 10, 10);
            //}

            //lbScaleOffset.Text = "ScaleX="+ScaleOffset.fScaleX + " ScaleY=" + ScaleOffset.fScaleY +" OffsetX="+ScaleOffset.fOffsetX + " OffsetY=" + ScaleOffset.fOffsetY ;
        }

        public static bool PaintMat(Mat _Mat , Graphics _g , TScaleOffset _ScaleOffset , float _fMin = 0f, float _fMax = 0f )//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            Rectangle Rect = new Rectangle(0,0,(int)_g.ClipBounds.Width , (int)_g.ClipBounds.Height) ;

            
            if(_Mat.Depth == DepthType.Cv8U)
            {
                try
                {
                    _g.DrawImage(_Mat.Bitmap , 
                                 Rect , 
                                 _ScaleOffset.fOffsetX , 
                                 _ScaleOffset.fOffsetY , 
                                 _g.ClipBounds.Width  / _ScaleOffset.fScaleX , 
                                 _g.ClipBounds.Height / _ScaleOffset.fScaleY , 
                                 GraphicsUnit.Pixel);
                }
                catch(Exception _e)
                {
                    return false ;
                }
            }
            else if(_Mat.Depth == DepthType.Cv32F)
            {
                double min = _fMin;
                double max = _fMax;
                int[] minIdx = null;
                int[] maxIdx = null;
                
                if(min == 0f && max == 0f)
                {
                    CvInvoke.MinMaxIdx(_Mat, out min, out max, minIdx, maxIdx);
                }

                if(min == max) 
                {
                    return false ;
                }

                using(Mat ScaledImg = new Mat(_Mat.Rows, _Mat.Cols, DepthType.Cv8U, 4))
                {
                    double scale = 255.0 / (max - min);
                    _Mat.ConvertTo(ScaledImg, DepthType.Cv8U, scale, -min * scale);

                    try
                    {
                        _g.DrawImage(ScaledImg.Bitmap , 
                                     Rect , 
                                     _ScaleOffset.fOffsetX , 
                                     _ScaleOffset.fOffsetY , 
                                     _g.ClipBounds.Width  / _ScaleOffset.fScaleX , 
                                     _g.ClipBounds.Height / _ScaleOffset.fScaleY , 
                                     GraphicsUnit.Pixel);
                    }
                    catch(Exception _e)
                    {
                        return false ;
                    }
                }
                
            }
            else 
            {
                return false ;
            }
            

            return true ;

        }

        private void FormVision_Shown(object sender, EventArgs e)
        {

        }

        //다른쓰레드에서 화면갱신 호출할때.
        delegate void FInvalidateImageBox();
        public void PaintInvoke() //일단 외부 억세스 막고.  이벤트 방식에서 다시 바꿈.
        {
            

            if (ibCam.InvokeRequired) // Invoke가 필요하면
            {
                ibCam.Invoke(new FInvalidateImageBox(ibCam.Invalidate), new object[] { }); // 대리자를 호출
            }
            else
            { 
                ibCam.Invalidate();
            }
        }

        private void ibCam_MouseDown(object sender, MouseEventArgs e)
        {
            Stat.iClickX = e.X;
            Stat.iClickY = e.Y;
            Stat.bImgClick = true;
        }

        private float GetImgX(int _iPanelX)
        {
            if(ScaleOffset.fScaleX == 0) return 0 ;
            float dRet = _iPanelX / ScaleOffset.fScaleX + ScaleOffset.fOffsetX ;
            return dRet ;
        }

        private float GetImgY(int _iPanelY)
        {
            if(ScaleOffset.fScaleY == 0) return 0 ;
            float dRet = _iPanelY / ScaleOffset.fScaleY + ScaleOffset.fOffsetY ;
            return dRet ;
        }

        private int GetPanelX(float _dImgX)
        {
            double dRet = (_dImgX - ScaleOffset.fOffsetX) * ScaleOffset.fScaleX  ;
            return (int)dRet ;
        }

        private int GetPanelY(float _dImgY)
        {
            double dRet = (_dImgY- ScaleOffset.fOffsetY) * ScaleOffset.fScaleY  ;
            return (int)dRet ;
        }

        private void ibCam_MouseMove(object sender, MouseEventArgs e)
        {

            if (Stat.bImgClick)
            {
                //스케일 고려한 이동량.
                float fMoveX = GetImgX(e.X) - GetImgX(Stat.iClickX); 
                float fMoveY = GetImgY(e.Y) - GetImgY(Stat.iClickY); 

                Stat.iClickX = e.X ;
                Stat.iClickY = e.Y ;

                ScaleOffset.fOffsetX -= fMoveX ;
                ScaleOffset.fOffsetY -= fMoveY ;
                
                UpdateOffset(ref ScaleOffset);

                ibCam.Invalidate();
            }    
            DispPixelData(e.X , e.Y);
        }

        private void DispPixelData(int _iPanelX , int _iPanelY)
        {
            if(DispImg ==null)return ;
            //lock(GV.ImageLock[iVisionID])//lock(DispImg)//
            //{     
                       
                double dCrntImgX = GetImgX(_iPanelX);
                double dCrntImgY = GetImgY(_iPanelY);
                if (dCrntImgX >= 0 && dCrntImgX < DispImg.Width && dCrntImgY >= 0 && dCrntImgY < DispImg.Height ) 
                {
                    //색깔 보여 주기.
                    Color PixelColor = GetPixel(DispImg, (int)dCrntImgX, (int)dCrntImgY);//, DispImg.Cols, DispImg.ElementSize);

                    int iFontColorA = PixelColor.A ;
                    int iFontColorR = PixelColor.R < 128 ? 255 : 0 ;
                    int iFontColorG = PixelColor.G < 128 ? 255 : 0 ;
                    int iFontColorB = PixelColor.B < 128 ? 255 : 0 ;
                    Color FontColor = Color.FromArgb(iFontColorA, iFontColorR, iFontColorG, iFontColorB);  

                    //pnColor.BackColor = PixelColor ;
                    //lbPixel.ForeColor = FontColor  ;
                    //lbPixel.Text = "<" + DispImg.Width + "." + DispImg.Height + "." + "> " +"(" + (int)dCrntImgX + "." + (int)dCrntImgY + ")=";
                    //if (DispImg.ElementSize == 3)
                    //{
                    //    lbPixel.Text += PixelColor.R.ToString() + ", " + PixelColor.G.ToString() + ", " + PixelColor.B.ToString();
                    //}
                    //else
                    //{
                    //    lbPixel.Text += PixelColor.R.ToString();
                    //}  
                    
                }
            //}
        }

        private void ibCam_MouseUp(object sender, MouseEventArgs e)
        {

            Stat.bImgClick = false;
            ibCam.Invalidate();
        }

        protected void SetFitScaleOffset(float _fPanelWidth , float _fPanelHeight , int _iImgWidth , int _iImgHeight)
        {
            float WidthScale  = _fPanelWidth  / (float)_iImgWidth  ;
            float HeightScale = _fPanelHeight / (float)_iImgHeight ;
            //TScaleOffset ScaleOffset = new TScaleOffset();
            ScaleOffset.fScaleX  = WidthScale < HeightScale ? WidthScale : HeightScale ;
            ScaleOffset.fScaleY  = WidthScale < HeightScale ? WidthScale : HeightScale ;

            float fMaxOfsX =  _iImgWidth   - _fPanelWidth  / ScaleOffset.fScaleX ;
            float fMaxOfsY =  _iImgHeight  - _fPanelHeight / ScaleOffset.fScaleY ;
            if(fMaxOfsX < 0) fMaxOfsX /= 2.0f ;
            if(fMaxOfsY < 0) fMaxOfsY /= 2.0f ;
            if(ScaleOffset.fOffsetX < 0       ) ScaleOffset.fOffsetX = 0 ;
            if(ScaleOffset.fOffsetY < 0       ) ScaleOffset.fOffsetY = 0 ;
            if(ScaleOffset.fOffsetX > fMaxOfsX) ScaleOffset.fOffsetX = fMaxOfsX ;
            if(ScaleOffset.fOffsetY > fMaxOfsY) ScaleOffset.fOffsetY = fMaxOfsY ;
        }

        //Stopwatch GrabTime = new Stopwatch();
        

        private void ZoomIn(int _iX, int _iY)
        {
            if(DispImg==null) return ;

            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 1.25f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 1.25f;

            float fMouseScaleX = _iX * 1.25f;
            float fMouseScaleY = _iY * 1.25f;

            float fMouseMoveX = fMouseScaleX - _iX;
            float fMouseMoveY = fMouseScaleY - _iY;

            ScaleOffset.fOffsetX += (fMouseMoveX / ScaleOffset.fScaleX);
            ScaleOffset.fOffsetY += (fMouseMoveY / ScaleOffset.fScaleY);

            UpdateOffset(ref ScaleOffset);
            ibCam.Invalidate();
        }

        private void ZoomOut(int _iX, int _iY)
        {
            if(DispImg==null) return ;

            if (ScaleOffset.fScaleX * 0.8f < 0.1 || ScaleOffset.fScaleY * 0.8f < 0.1) return;

            ScaleOffset.fScaleX = ScaleOffset.fScaleX / 1.25f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY / 1.25f;

            float fMouseScaleX = _iX / 1.25f;
            float fMouseScaleY = _iY / 1.25f;

            float fMouseMoveX = fMouseScaleX - _iX;
            float fMouseMoveY = fMouseScaleY - _iY;

            ScaleOffset.fOffsetX += (fMouseMoveX / ScaleOffset.fScaleX);
            ScaleOffset.fOffsetY += (fMouseMoveY / ScaleOffset.fScaleY);

            UpdateOffset(ref ScaleOffset);

            ibCam.Invalidate();
        }

        //마우스 휠 이벤트 (Zoom In/Out)
        private void ibCam_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                ZoomOut(e.X, e.Y);
            }
            else
            {
                ZoomIn(e.X, e.Y);
                
            }
        }

        //Key Down 이벤트 (Zoom In/Out)
        private void ibCam_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Keys pressKey = e.KeyData & ~(Keys.Control);

            Point P = ibCam.PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y));

            switch (pressKey)
            {
                case Keys.Add     : ZoomIn (P.X, P.Y); break;
                case Keys.Subtract: ZoomOut(P.X, P.Y); break;
            }
        }

        public Color GetPixel(Mat mat, int x, int y)//, int cols, int elementSize)
        {
            Color color ;
            byte r = 0,g = 0,b = 0 ;  

            int c = mat.Cols ;
            int e = mat.ElementSize ;
            unsafe
            {
                byte* Data = (byte *)mat.DataPointer;               
                
                if(e == 3)
                {
                    b = *(Data + (y * c + x) * e + 0);
                    g = *(Data + (y * c + x) * e + 1);
                    r = *(Data + (y * c + x) * e + 2);
                    color = Color.FromArgb(r,g,b);
                }
                else
                {
                    r = *(Data + (y * c + x) * e);
                    color = Color.FromArgb(r,r,r);
                }
            }
            return color;
        }
        
        private void UpdateOffset(ref TScaleOffset _ScaleOffset)
        {
            //lock(GV.ImageLock[iVisionID])
            {
                if (DispImg == null) return;
                
                int iImgWidth  = DispImg.Width  ;
                int iImgHeight = DispImg.Height ;
                if (iImgWidth == 0 || iImgHeight == 0) return;
                
                int iPanelWidth  = ibCam.Width  ;
                int iPanelHeight = ibCam.Height ;                        
                float fMaxOfsX =  iImgWidth   - iPanelWidth  / _ScaleOffset.fScaleX ;
                float fMaxOfsY =  iImgHeight  - iPanelHeight / _ScaleOffset.fScaleY ;
                if(fMaxOfsX < 0) fMaxOfsX /= 2.0f ;
                if(fMaxOfsY < 0) fMaxOfsY /= 2.0f ;
                if(_ScaleOffset.fOffsetX < 0       ) _ScaleOffset.fOffsetX = 0 ;
                if(_ScaleOffset.fOffsetY < 0       ) _ScaleOffset.fOffsetY = 0 ;
                if(_ScaleOffset.fOffsetX > fMaxOfsX) _ScaleOffset.fOffsetX = fMaxOfsX ;
                if(_ScaleOffset.fOffsetY > fMaxOfsY) _ScaleOffset.fOffsetY = fMaxOfsY ;
            }
        }

        private TScaleOffset GetFitScaleOffset(Mat _Img)
        {
            float WidthScale  = ibCam.Width  / (float)_Img.Width  ;
            float HeightScale = ibCam.Height / (float)_Img.Height ;
            TScaleOffset ScaleOffset = new TScaleOffset();
            ScaleOffset.fScaleX  = WidthScale < HeightScale ? WidthScale : HeightScale ;
            ScaleOffset.fScaleY  = WidthScale < HeightScale ? WidthScale : HeightScale ;

            float fMaxOfsX =  _Img.Width   - ibCam.Width  / ScaleOffset.fScaleX ;
            float fMaxOfsY =  _Img.Height  - ibCam.Height / ScaleOffset.fScaleY ;
            if(fMaxOfsX < 0) fMaxOfsX /= 2.0f ;
            if(fMaxOfsY < 0) fMaxOfsY /= 2.0f ;
            if(ScaleOffset.fOffsetX < 0       ) ScaleOffset.fOffsetX = 0 ;
            if(ScaleOffset.fOffsetY < 0       ) ScaleOffset.fOffsetY = 0 ;
            if(ScaleOffset.fOffsetX > fMaxOfsX) ScaleOffset.fOffsetX = fMaxOfsX ;
            if(ScaleOffset.fOffsetY > fMaxOfsY) ScaleOffset.fOffsetY = fMaxOfsY ;

            return ScaleOffset;
        }

        //private void btFitSize_Click(object sender, EventArgs e)
        //{
        //IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
        //if(DispPkg == null) return;
        //Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
        //if (DispImg == null) return;
        //
        //TScaleOffset ScaleOffset = GetFitScaleOffset(DispImg);
        //
        //GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset;
        //
        //ibCam.Invalidate();
        //}

        //private void btRealSize_Click(object sender, EventArgs e)
        //{
        //if(GV.Visions[iVisionID].GetPkgDisplay()==null) return ;
        //TScaleOffset ScaleOffset ;
        //ScaleOffset.fOffsetX = 0;
        //ScaleOffset.fOffsetY = 0;
        //ScaleOffset.fScaleX  = 1.0f;
        //ScaleOffset.fScaleY  = 1.0f;
        //
        //UpdateOffset(ref ScaleOffset);
        //
        //GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset ;
        //
        //ibCam.Invalidate();
        //}

        private void FormVision_Resize(object sender, EventArgs e)
        {
            if (DispImg == null) return;

            UpdateOffset(ref ScaleOffset);
        }


        
        public void Rec(bool _bStart)
        {
            if(_bStart)ThreadRecorder.StartRec(GetDispImg , 30);
            else       ThreadRecorder.StopRec ();

        }

        public void Save()
        {
            /*
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "ImageFiles (*.bmp, *.jpg)|*.bmp;*.jpg";
            if (Openfile.ShowDialog() != DialogResult.OK) return ;
            DispImg = new Mat(Openfile.FileName, ImreadModes.Unchanged);

            ibCam.Invalidate();  
            */
            
            if(GetDispImg() == null) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bmp File|*.bmp";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;
            //ibCam.Image.Save(saveFileDialog1.FileName);
            GetDispImg().Save(saveFileDialog1.FileName);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "ImageFiles (*.bmp, *.jpg)|*.bmp;*.jpg";
            if (Openfile.ShowDialog() != DialogResult.OK) return ;
            DispImg = new Mat(Openfile.FileName, ImreadModes.Unchanged);

            ibCam.Invalidate();  

            TThreadPara Para = new TThreadPara();
            Para.File  = "D:\\Image\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + OM.Info.sImageFileName + "_" + OM.Info.iImageSave.ToString() + ".bmp" ;
            Para.Img   = DispImg.Clone() ;
            Para.Index = 0 ;
            //Para.Img.Save(Para.File);
            
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(Para.File));
            if(!di.Exists)
            {
                di.Create();
            }
            
            //Para.Img.Save(Para.File);
            //Para.Img.Save("D:\\Image\\20191104\\_0.bmp");
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveImage) , Para);
            //Para.Img.Dispose();

        }
    }
}
