using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

using VDll.Pakage;
using COMMON;
using System.Drawing.Drawing2D;

namespace VDll
{
    public partial class FormVision : Form
    {
        struct TStat {
            public bool bImgClick    ; //이미지 이동.
            public int  iClickX      ; //트렉커 클릭X
            public int  iClickY      ; //트렉커 클릭Y
        };
        
        TStat       Stat     = new TStat { bImgClick = false, iClickX = 0, iClickY = 0 };
        CTracker    Tracker  = new CTracker();
        FormProfile FrmProfile;

        int iVisionID ;
        Vision vision;

        public FormVision(int _iVisionID)
        {
            InitializeComponent();

            iVisionID = _iVisionID ;
            vision    = GV.Visions[iVisionID];

            ibCam.SizeMode = PictureBoxSizeMode.Normal ;
            GV.PaintCallbacks[iVisionID]= PaintInvoke ;
            
            //Profile용 트래커
            Tracker.Init(CTracker.ETrackerType.ttLine);
            //Tracker.Init(CTracker.ETrackerType.ttCircle);
            //Tracker.visible = false;
            //Tracker.trackerType = CTracker.ETrackerType.ttCircle;

            ibCam.MouseWheel += new MouseEventHandler(ibCam_MouseWheel);

            //Stretch, RealRatio
            //rbStratch.Checked = true;
            //rbRealRatio.Checked = true;
            //rbStratch_CheckedChanged();
        }
        private void FormVision_Load(object sender, EventArgs e)
        {
            rbStratch_CheckedChanged();
        }

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

        #region Timer
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            //lbGrabTime .Text = GV.Cameras[0].GetGrabTime().ToString();
            //lbZoomScale.Text = ibCam.ZoomScale.ToString() ; 
        }

        private void tmLive_Tick(object sender, EventArgs e)
        {
            vision.NeedGrab();
        }

        private void tmGrabInsp_Tick(object sender, EventArgs e)
        {
            vision.NeedGrabInsp();
        }
        #endregion

        private float GetImgX(int _iPanelX)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            if(ScaleOffset.fScaleX == 0) return 0 ;
            float dRet = _iPanelX / ScaleOffset.fScaleX + ScaleOffset.fOffsetX ;
            return dRet ;
        }

        private float GetImgY(int _iPanelY)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            if(ScaleOffset.fScaleY == 0) return 0 ;
            float dRet = _iPanelY / ScaleOffset.fScaleY + ScaleOffset.fOffsetY ;
            return dRet ;
        }

        private int GetPanelX(float _dImgX)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            double dRet = (_dImgX - ScaleOffset.fOffsetX) * ScaleOffset.fScaleX  ;
            return (int)dRet ;
        }

        private int GetPanelY(float _dImgY)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            double dRet = (_dImgY- ScaleOffset.fOffsetY) * ScaleOffset.fScaleY  ;
            return (int)dRet ;
        }

        #region Display
        private void DispPixelData(int _iPanelX , int _iPanelY)
        {
            lock(GV.ImageLock[iVisionID])//lock(DispImg)//
            {     
                Mat Image = vision.GetImage();
                       
                double dCrntImgX = GetImgX(_iPanelX);
                double dCrntImgY = GetImgY(_iPanelY);
                if (dCrntImgX >= 0 && dCrntImgX < Image.Width && dCrntImgY >= 0 && dCrntImgY < Image.Height ) 
                {
                    //색깔 보여 주기.
                    Color PixelColor = GetPixel(Image, (int)dCrntImgX, (int)dCrntImgY);//, DispImg.Cols, DispImg.ElementSize);

                    int iFontColorA = PixelColor.A ;
                    int iFontColorR = PixelColor.R < 128 ? 255 : 0 ;
                    int iFontColorG = PixelColor.G < 128 ? 255 : 0 ;
                    int iFontColorB = PixelColor.B < 128 ? 255 : 0 ;
                    Color FontColor = Color.FromArgb(iFontColorA, iFontColorR, iFontColorG, iFontColorB);  

                    pnColor.BackColor = PixelColor ;
                    lbPixel.ForeColor = FontColor  ;
                    lbPixel.Text = "<" + Image.Width + "." + Image.Height + "." + "> " +"(" + (int)dCrntImgX + "." + (int)dCrntImgY + ")=";
                    if (Image.ElementSize == 3)
                    {
                        lbPixel.Text += PixelColor.R.ToString() + ", " + PixelColor.G.ToString() + ", " + PixelColor.B.ToString();
                    }
                    else
                    {
                        lbPixel.Text += PixelColor.R.ToString();
                    }  
                    
                }
            }
        }

        //Stopwatch GrabTime = new Stopwatch();
        private void ibCam_Paint(object sender, PaintEventArgs e)
        {           
            //이건 뭐지??? 그랩타임인데 ㅎㅎ
            //lbGrabTime .Text = GrabTime.ElapsedMilliseconds.ToString() ;
            //GrabTime.Restart();

            Graphics g = e.Graphics ;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode   = PixelOffsetMode.Half ;

            //스케일 오프셑은 카메라껄쓴다.
            //TScaleOffset ScaleOffset = CamPkg.ScaleOffset ;
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            //if(ScaleOffset.fScaleX == 0 && ScaleOffset.fScaleY ==0)
            //{
            //    vision.SetFitScaleOffset(g.ClipBounds.Width , g.ClipBounds.Height , vision.GetImage().Width, vision.GetImage().Height);
            //}

            //검사가 끝난상태이면 PKG Diplay로 함.
            //if (vision.VisnStats.bNeedDispRslt)
            //{
            //    lock (GV.ImageLock[iVisionID])
            //    {
            //        vision.GetPkgDisplay().PaintScOf(g, false , ScaleOffset);                    
            //    }
            //}
            //else //그랩을 하거나 이미지 로드시엔 리절트가 보이면 안됨.
            //{
            //    lock (GV.ImageLock[iVisionID])
            //    {
            //        CamPkg.PaintScOf(g , false , ScaleOffset);
            //    }
            //}

            //vision.PaintScOf(g, false , ScaleOffset);                    
            vision.Paint(g, false);

            if (ScaleOffset.fScaleX > 29 && ScaleOffset.fScaleY > 29)
            {
                ViewerDisplayPixel(g);
            }
            Tracker.Paint(g, ScaleOffset);

            lbScaleOffset.Text = "ScaleX="+ScaleOffset.fScaleX + " ScaleY=" + ScaleOffset.fScaleY +" OffsetX="+ScaleOffset.fOffsetX + " OffsetY=" + ScaleOffset.fOffsetY ;
        }

        //뷰어 전체에 픽셀값 보여주는 함수
        public void ViewerDisplayPixel(Graphics _g)
        {
            Mat Image = vision.GetImage();

            float dDispImgStartX = GetImgX(0);
            float dDispImgStartY = GetImgY(0);
            float dDispImgEndX   = GetImgX((int)_g.ClipBounds.Width );
            float dDispImgEndY   = GetImgY((int)_g.ClipBounds.Height);     

            int iDispImgStartX = (int)dDispImgStartX;
            int iDispImgStartY = (int)dDispImgStartY;
            int iDispImgEndX   = (int)Math.Ceiling((double)dDispImgEndX  );
            int iDispImgEndY   = (int)Math.Ceiling((double)dDispImgEndY  );

            const int iFontSize = 8;
            Color color ;
            int iR,iG,iB ;
            string sText = "";

            using (Font font = new Font("Arial", iFontSize))
            using (SolidBrush brush = new SolidBrush(Color.Red))
            {                
                for (int x = iDispImgStartX; x < iDispImgEndX; x++)
                {
                    for (int y = iDispImgStartY; y < iDispImgEndY; y++)
                    {
                        color = GetPixel(Image, x, y);// , DispImg.Cols, DispImg.ElementSize);
                        iR = color.R;
                        iG = color.G;
                        iB = color.B;

                        if (Image.ElementSize == 3)
                        {
                            sText = "R:" + iR.ToString()+ "\r\n" + "G:" + iG.ToString() + "\r\n" + "B:" + iB.ToString();
                        }
                        else
                        {
                            sText = iR.ToString();
                        }

                        float Tempx = GetPanelX(x);
                        float Tempy = GetPanelY(y);
                        _g.DrawString(sText, font, brush, Tempx , Tempy);
                    }
                }
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
        #endregion

        private void ZoomIn(int _iX, int _iY)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();

            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 1.25f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 1.25f;

            float fMouseScaleX = _iX * 1.25f;
            float fMouseScaleY = _iY * 1.25f;

            float fMouseMoveX = fMouseScaleX - _iX;
            float fMouseMoveY = fMouseScaleY - _iY;

            ScaleOffset.fOffsetX += (fMouseMoveX / ScaleOffset.fScaleX);
            ScaleOffset.fOffsetY += (fMouseMoveY / ScaleOffset.fScaleY);

            UpdateOffset(ref ScaleOffset);
            vision.SetScaleOffset(ScaleOffset) ;

            ibCam.Invalidate();
        }

        private void ZoomOut(int _iX, int _iY)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();

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
            vision.SetScaleOffset(ScaleOffset) ;

            ibCam.Invalidate();
        }

        
        private void UpdateOffset(ref TScaleOffset _ScaleOffset)
        {
            //lock(GV.ImageLock[iVisionID])
            {
                //IPkg DispPkg = vision.GetPkgDisplay();
                //if(DispPkg == null) return;
                //Mat DispImg = vision.GetPkgDisplay().GetImage() ;
                //if (DispImg == null) return;
                //if (DispImg.Data == null) return ;

                Mat Image = vision.GetImage();
                
                int iImgWidth  = Image.Width  ;
                int iImgHeight = Image.Height ;
                if (iImgWidth == 0 || iImgHeight == 0) return;
                
                if(rbStratch.Checked)
                {
                    _ScaleOffset.fOffsetX = 0;
                    _ScaleOffset.fOffsetY = 0;
                    _ScaleOffset.fScaleX = ibCam.Width /  (float)iImgWidth;
                    _ScaleOffset.fScaleY = ibCam.Height / (float)iImgHeight;
                }

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




        #region Event
        private void rbStratch_CheckedChanged()
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            Mat Image = vision.GetImage();
            if(rbRealRatio.Checked)
            {
                ScaleOffset.fOffsetX = 0;
                ScaleOffset.fOffsetY = 0;
                ScaleOffset.fScaleX = 1.0f;
                ScaleOffset.fScaleY = 1.0f;                
            }
            else if(rbFit.Checked)
            {
                ScaleOffset = GetFitScaleOffset(Image);
            }
            else 
            {
                ScaleOffset.fOffsetX = 0;
                ScaleOffset.fOffsetY = 0;
                ScaleOffset.fScaleX = ibCam.Width  / (float)Image.Width ;
                ScaleOffset.fScaleY = ibCam.Height / (float)Image.Height;
            }
            UpdateOffset(ref ScaleOffset);
            vision.SetScaleOffset(ScaleOffset) ;
            //ibCam.Refresh();
            ibCam.Invalidate();
        }

        private void rbStratch_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                rbStratch_CheckedChanged();
            }
        }

        private void rbStratch_Click(object sender, EventArgs e)
        {
            rbStratch_CheckedChanged();
        }
        private void btZoomIn_Click(object sender, EventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();

            ScaleOffset.fScaleX  = ScaleOffset.fScaleX * 1.25f;
            ScaleOffset.fScaleY  = ScaleOffset.fScaleY * 1.25f;
          
            UpdateOffset(ref ScaleOffset);

            vision.SetScaleOffset(ScaleOffset) ;
            ibCam.Invalidate();
        }

        private void btZoomOut_Click(object sender, EventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();

            if(ScaleOffset.fScaleX * 0.8f < 0.1 || ScaleOffset.fScaleY * 0.8f < 0.1) return ;
            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 0.8f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 0.8f;

            UpdateOffset(ref ScaleOffset);

            vision.SetScaleOffset(ScaleOffset) ;
            ibCam.Invalidate();
        }

        private void btLoadImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "ImageFiles (*.bmp, *.jpg)|*.bmp;*.jpg";
            if (Openfile.ShowDialog() != DialogResult.OK) return ;
            vision.LoadActiveImage(Openfile.FileName);

            TScaleOffset ScaleOffset = GetFitScaleOffset( vision.GetPkgCamera().GetImage() ); 
            vision.SetScaleOffset(ScaleOffset) ;

            ibCam.Invalidate();      
        }

        private void btSaveImg_Click(object sender, EventArgs e)
        {
            Mat Image = vision.GetImage();

            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Filter = "Bitmap Files|*.bmp";
            if (SaveFile.ShowDialog() != DialogResult.OK) return;
            Image.Save(SaveFile.FileName);
            ibCam.Invalidate();
        }


        private void FormVision_Resize(object sender, EventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            UpdateOffset(ref ScaleOffset);
            vision.SetScaleOffset(ScaleOffset) ;
        }


        private void ibCam_MouseDown(object sender, MouseEventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();

            bool bTracker = Tracker.MouseDown(Control.ModifierKeys , e , ScaleOffset );
            if(!bTracker)
            {
                Stat.iClickX = e.X ;
                Stat.iClickY = e.Y ;
                Stat.bImgClick = true;
            }

        }

        private void ibCam_MouseMove(object sender, MouseEventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            if(Tracker.MouseMove(Control.ModifierKeys , e , ScaleOffset))
            {
                ibCam.Invalidate();
                FrmProfile.ProfilePaintInvoke();
            }
            else if (Stat.bImgClick)
            {
                //스케일 고려한 이동량.
                float fMoveX = GetImgX(e.X) - GetImgX(Stat.iClickX); 
                float fMoveY = GetImgY(e.Y) - GetImgY(Stat.iClickY); 

                Stat.iClickX = e.X ;
                Stat.iClickY = e.Y ;

                ScaleOffset.fOffsetX -= fMoveX ;
                ScaleOffset.fOffsetY -= fMoveY ;
                
                UpdateOffset(ref ScaleOffset);

                vision.SetScaleOffset(ScaleOffset);

                ibCam.Invalidate();
            }    
            DispPixelData(e.X , e.Y);
        }


        private void ibCam_MouseUp(object sender, MouseEventArgs e)
        {
            if(Tracker.MouseUp(Control.ModifierKeys , e))
            {

            }

            Stat.bImgClick = false;
            ibCam.Invalidate();
        }

        private void btMsg_Click(object sender, EventArgs e)
        {
            if(tbMsg.Text == "") Tracker.message = "";
            else                 Tracker.AddMessage(tbMsg.Text);
            ibCam.Invalidate();
        }



        private void btInsp_Click(object sender, EventArgs e)
        {
            vision.NeedInsp();
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

        private void btProfile_Click(object sender, EventArgs e)
        {
            Mat Image = vision.GetImage();

            FrmProfile = new FormProfile(Image, Tracker);
            FrmProfile.Show();
        }

        private void cbLiveCam_CheckedChanged(object sender, EventArgs e)
        {
            tmLive.Enabled = cbLiveCam.Checked;
        }

        private void btGrabCam_Click(object sender, EventArgs e)
        {
            vision.NeedGrab() ;
        }

        private void btMap3D_Click(object sender, EventArgs e)
        {
            btMap3D.Enabled = false;
            bool bRvs = false;
            Map map = new Map(bRvs);
            map.LoadMat(vision.GetImage());
            map.window.Run(1.0/30.0);
            //Map.Instance.LoadBmp(vision.Camera.GetImage().Bitmap);
            //Map.Instance.window.Run(1.0/30.0);
            //Map.Instance.window.Visible = true;
            
            btMap3D.Enabled = true;
        }


        private void btGrabInspCam_Click(object sender, EventArgs e)
        {
            vision.NeedGrabInsp();

        }

        private void btSetting_Click(object sender, EventArgs e)
        {
            tmLive       .Enabled = false ;
            cbLiveCam    .Checked = false ;
            tmGrabInsp   .Enabled = false;
            cbGrabInspCam.Checked = false;

            using(FormTrain FrmTrain = new FormTrain(iVisionID))
            {
                FrmTrain.ShowDialog();
            }

            //화면에 맞춰다시 스케일 지정하기 위해 초기화 하고
            //Paint함수에서 자동으로 Fit View로 맞춘다.
            //TScaleOffset SO = new TScaleOffset();
            //SO.fScaleX  = 0 ;
            //SO.fScaleY  = 0 ;
            //SO.fOffsetX = 0 ;
            //SO.fOffsetY = 0 ;
            foreach(IPkg Pkg in vision.Pkgs)
            {
                Pkg.ScaleOffset = new TScaleOffset();
            }

            GV.PaintCallbacks[iVisionID]= PaintInvoke ;
            rbStratch_CheckedChanged();
            //ibCam.Invalidate();  
        }

        private void cbGrabInspCam_CheckedChanged(object sender, EventArgs e)
        {
            tmGrabInsp.Enabled = cbGrabInspCam.Checked;
        }

        private void ibCam_DoubleClick(object sender, EventArgs e)
        {
            if(Log.ShowMessageModal("NOTICE","Do you want to reset the tracker?") != DialogResult.Yes) return;
            //Tracker.Init();
            Tracker.Reset();
        }

        private void CAL_Click(object sender, EventArgs e)
        {
            using(FormCalibration FrmCalibration = new FormCalibration(iVisionID))
            {
                FrmCalibration.ShowDialog();
            }
        }

        private void btRecipe_Click(object sender, EventArgs e)
        {
            using(FormRecipe FrmRecipe = new FormRecipe(GV.Para.DeviceFolder,VL.RecipeName,VL.LoadRecipe))
            {
                FrmRecipe.ShowDialog();
            }
        }


    }


    #endregion


}
