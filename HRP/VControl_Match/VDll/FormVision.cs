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
        
        TStat Stat = new TStat { bImgClick = false, iClickX = 0, iClickY = 0 };
        int iVisionID = 0;
        CTracker Tracker = new CTracker();
        FormProfile FrmProfile;

        public FormVision(int _iVisionID)
        {
            InitializeComponent();
            ibCam.SizeMode = PictureBoxSizeMode.Normal ;

            iVisionID = _iVisionID ;

            GV.PaintCallbacks[iVisionID]= PaintInvoke ;
            
            Tracker.Init();
            //sun 뻑나는데 최상위 어쩌구 저쩌구.
            //FrmTrain.Parent = this ;

            ibCam.MouseWheel += new MouseEventHandler(ibCam_MouseWheel);
        }

        private void FormVision_Shown(object sender, EventArgs e)
        {

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

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            //lbGrabTime .Text = GV.Cameras[0].GetGrabTime().ToString();
            //lbZoomScale.Text = ibCam.ZoomScale.ToString() ; 
        }

        private void btLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "ImageFiles (*.bmp, *.jpg)|*.bmp;*.jpg";
            if (Openfile.ShowDialog() != DialogResult.OK) return ;
            GV.Visions[iVisionID].LoadActiveImage(Openfile.FileName);

            TScaleOffset ScaleOffset = GetFitScaleOffset( GV.Visions[iVisionID].GetPkgCamera().GetImage() ); 
            GV.Visions[iVisionID].GetPkgCamera().ScaleOffset= ScaleOffset ;

            ibCam.Invalidate();            
        }

        private void btGrab_Click(object sender, EventArgs e)
        {
            GV.Visions[iVisionID].NeedGrab() ;
        }
        
        private void btTrain_Click(object sender, EventArgs e)
        {
            tmLive.Enabled = false ;
            cbLive.Checked = false ;
            tmGrabInsp.Enabled = false;
            cbGrabInsp.Checked = false;

            using(FormTrain FrmTrain = new FormTrain(iVisionID))
            {
                FrmTrain.ShowDialog();
            }

            TScaleOffset SO = new TScaleOffset();
            SO.fScaleX  = 0 ;
            SO.fScaleY  = 0 ;
            SO.fOffsetX = 0 ;
            SO.fOffsetY = 0 ;
            //화면에 맞춰다시 스케일 지정하기 위해 초기화 하고
            //Paint함수에서 자동으로 Fit View로 맞춘다.
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                Pkg.ScaleOffset = SO ;
            }

            GV.PaintCallbacks[iVisionID]= PaintInvoke ;
            ibCam.Invalidate();   
        }

        private void tmLive_Tick(object sender, EventArgs e)
        {
            GV.Visions[iVisionID].NeedGrab();
        }

        private void ibCam_MouseDown(object sender, MouseEventArgs e)
        {
            IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
            if(DispPkg == null) return;
            Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
            if (DispImg == null) return;

            if(Tracker.MouseDown(Control.ModifierKeys , e , DispPkg.ScaleOffset ))
            {
            }
            else 
            {
                Stat.iClickX = e.X ;
                Stat.iClickY = e.Y ;
                Stat.bImgClick = true;
            }
        }

        private float GetImgX(int _iPanelX)
        {
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;
            if(ScaleOffset.fScaleX == 0) return 0 ;
            float dRet = _iPanelX / ScaleOffset.fScaleX + ScaleOffset.fOffsetX ;
            return dRet ;
        }

        private float GetImgY(int _iPanelY)
        {
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;
            if(ScaleOffset.fScaleY == 0) return 0 ;
            float dRet = _iPanelY / ScaleOffset.fScaleY + ScaleOffset.fOffsetY ;
            return dRet ;
        }

        private int GetPanelX(float _dImgX)
        {
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;
            double dRet = (_dImgX - ScaleOffset.fOffsetX) * ScaleOffset.fScaleX  ;
            return (int)dRet ;
        }

        private int GetPanelY(float _dImgY)
        {
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;
            double dRet = (_dImgY- ScaleOffset.fOffsetY) * ScaleOffset.fScaleY  ;
            return (int)dRet ;
        }

        private void ibCam_MouseMove(object sender, MouseEventArgs e)
        {
            IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
            if(DispPkg == null) return;

            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;
            if(Tracker.MouseMove(Control.ModifierKeys , e , ScaleOffset))
            {
                ibCam.Invalidate();
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

                GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset ;

                ibCam.Invalidate();
            }    
            DispPixelData(e.X , e.Y);
        }

        private void DispPixelData(int _iPanelX , int _iPanelY)
        {
            lock(GV.ImageLock[iVisionID])//lock(DispImg)//
            {     
                IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
                if(DispPkg == null) return;
                Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
                if (DispImg == null) return;
                       
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

                    pnColor.BackColor = PixelColor ;
                    lbPixel.ForeColor = FontColor  ;
                    lbPixel.Text = "<" + DispImg.Width + "." + DispImg.Height + "." + "> " +"(" + (int)dCrntImgX + "." + (int)dCrntImgY + ")=";
                    if (DispImg.ElementSize == 3)
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

        private void ibCam_MouseUp(object sender, MouseEventArgs e)
        {
            if(Tracker.MouseUp(Control.ModifierKeys , e))
            {

            }

            Stat.bImgClick = false;
            ibCam.Invalidate();
        }

        Stopwatch GrabTime = new Stopwatch();
        private void ibCam_Paint(object sender, PaintEventArgs e)
        {
            IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
            if(DispPkg == null) return;
            Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
            if (DispImg == null) return;

            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;

            lbGrabTime .Text = GrabTime.ElapsedMilliseconds.ToString() ;
            GrabTime.Restart();
            Graphics g = e.Graphics ;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode   = PixelOffsetMode.Half ;

            lbScaleOffset.Text = "ScaleX="+ScaleOffset.fScaleX + " ScaleY=" + ScaleOffset.fScaleY +" OffsetX="+ScaleOffset.fOffsetX + " OffsetY=" + ScaleOffset.fOffsetY ;
            
            lock(GV.ImageLock[iVisionID])
            {
                GV.Visions[iVisionID].GetPkgDisplay().Paint(g);
                if(ScaleOffset.fScaleX > 29 && ScaleOffset.fScaleY > 29)
                {
                    ViewerDisplayPixel(g);
                }
            }
            

            Tracker.Paint(g , ScaleOffset);
        }

        //뷰어 전체에 픽셀값 보여주는 함수
        public void ViewerDisplayPixel(Graphics _g)
        {
            IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
            if(DispPkg == null) return;
            Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
            if (DispImg == null) return;

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
                        color = GetPixel(DispImg, x, y);// , DispImg.Cols, DispImg.ElementSize);
                        iR = color.R;
                        iG = color.G;
                        iB = color.B;

                        if (DispImg.ElementSize == 3)
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

        private void btGrabInsp_Click(object sender, EventArgs e)
        {
            GV.Visions[iVisionID].Ready();
            GV.Visions[iVisionID].NeedGrabInsp();
        }

        private void btMsg_Click(object sender, EventArgs e)
        {
            if(tbMsg.Text == "") Tracker.message = "";
            else                 Tracker.AddMessage(tbMsg.Text);
        }

        private void tmGrabInsp_Tick(object sender, EventArgs e)
        {
            GV.Visions[iVisionID].NeedGrabInsp();
        }


        private void cbLive_CheckedChanged(object sender, EventArgs e)
        {
            tmLive.Enabled = cbLive.Checked;
        }

        private void cbGrabInsp_CheckedChanged(object sender, EventArgs e)
        {
            tmGrabInsp.Enabled = cbGrabInsp.Checked;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
            if(DispPkg == null) return;
            Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
            if (DispImg == null) return;

            

            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Filter = "Bitmap Files|*.bmp";
            if (SaveFile.ShowDialog() != DialogResult.OK) return;
            DispImg.Save(SaveFile.FileName);
            ibCam.Invalidate();
        }

        private void btMap_Click(object sender, EventArgs e)
        {
            btMap.Enabled = false;
            bool bRvs = false;
            Map map = new Map(bRvs);
            map.LoadMat(GV.Visions[iVisionID].GetPkgDisplay().GetImage());
            map.window.Run(1.0/30.0);
            //Map.Instance.LoadBmp(GV.Visions[iVisionID].Camera.GetImage().Bitmap);
            //Map.Instance.window.Run(1.0/30.0);
            //Map.Instance.window.Visible = true;
            
            btMap.Enabled = true;
        }





        private void btInsp_Click(object sender, EventArgs e)
        {
            
            GV.Visions[iVisionID].NeedInsp();
        }

        private void ZoomIn(int _iX, int _iY)
        {
            if(GV.Visions[iVisionID].GetPkgDisplay()==null) return ;
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;

            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 1.25f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 1.25f;

            float fMouseScaleX = _iX * 1.25f;
            float fMouseScaleY = _iY * 1.25f;

            float fMouseMoveX = fMouseScaleX - _iX;
            float fMouseMoveY = fMouseScaleY - _iY;

            ScaleOffset.fOffsetX += (fMouseMoveX / ScaleOffset.fScaleX);
            ScaleOffset.fOffsetY += (fMouseMoveY / ScaleOffset.fScaleY);

            UpdateOffset(ref ScaleOffset);
            GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset ;


            ibCam.Invalidate();
        }

        private void ZoomOut(int _iX, int _iY)
        {
            if(GV.Visions[iVisionID].GetPkgDisplay()==null) return ;
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;

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
            GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset ;


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


        private void btProfile_Click(object sender, EventArgs e)
        {
            IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
            if(DispPkg == null) return;
            Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
            if (DispImg == null) return;

            FrmProfile = new FormProfile(DispImg, Tracker);
            FrmProfile.Show();
        }
        
        private void UpdateOffset(ref TScaleOffset _ScaleOffset)
        {
            //lock(GV.ImageLock[iVisionID])
            {
                IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
                if(DispPkg == null) return;
                Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
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

        private void btZoomIn_Click(object sender, EventArgs e)
        {
            if(GV.Visions[iVisionID].GetPkgDisplay()==null) return ;
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;

            ScaleOffset.fScaleX  = ScaleOffset.fScaleX * 1.25f;
            ScaleOffset.fScaleY  = ScaleOffset.fScaleY * 1.25f;
          
            UpdateOffset(ref ScaleOffset);

            GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset ;

            ibCam.Invalidate();
        }

        private void btZoomOut_Click(object sender, EventArgs e)
        {
            if(GV.Visions[iVisionID].GetPkgDisplay()==null) return ;
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset ;

            if(ScaleOffset.fScaleX * 0.8f < 0.1 || ScaleOffset.fScaleY * 0.8f < 0.1) return ;
            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 0.8f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 0.8f;

            UpdateOffset(ref ScaleOffset);

            GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset ;
            ibCam.Invalidate();
        }

        

        private void btFitSize_Click(object sender, EventArgs e)
        {
            IPkg DispPkg = GV.Visions[iVisionID].GetPkgDisplay();
            if(DispPkg == null) return;
            Mat DispImg = GV.Visions[iVisionID].GetPkgDisplay().GetImage() ;
            if (DispImg == null) return;

            TScaleOffset ScaleOffset = GetFitScaleOffset(DispImg);
            
            GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset;

            ibCam.Invalidate();
        }

        private void btRealSize_Click(object sender, EventArgs e)
        {
            if(GV.Visions[iVisionID].GetPkgDisplay()==null) return ;
            TScaleOffset ScaleOffset ;
            ScaleOffset.fOffsetX = 0;
            ScaleOffset.fOffsetY = 0;
            ScaleOffset.fScaleX  = 1.0f;
            ScaleOffset.fScaleY  = 1.0f;

            UpdateOffset(ref ScaleOffset);

            GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset ;

            ibCam.Invalidate();
        }

        private void FormVision_Resize(object sender, EventArgs e)
        {
            if(GV.Visions[iVisionID].GetPkgDisplay() == null) return ;
            TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset;
            UpdateOffset(ref ScaleOffset);
            GV.Visions[iVisionID].GetPkgDisplay().ScaleOffset = ScaleOffset ;
        }

    }
}
