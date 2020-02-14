using COMMON;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDll.Pakage;

namespace VDll
{
    public partial class FormCalibration : Form
    {
        struct TStat {
            public bool bImgClick    ; //이미지 이동.
            public int  iClickX      ; //트렉커 클릭X
            public int  iClickY      ; //트렉커 클릭Y
        };
        TStat        Stat        ;
        TScaleOffset ScaleOffset ;

        int id;
        Mat display = new Mat();
        //Calibration cal ; //너와 나의 연결고리

        public FormCalibration(int id)
        {
            InitializeComponent();
            
            GV.calibration[id].PaintCallbacks += new Calibration.FInvalidate(PaintInvoke);
            GV.calibration[id].InitPaint();
            //Calibration.PaintCallbacks += new Calibration.FInvalidate(PaintInvoke);
            //cal = Calibration.Instance;


            //Init
            this.id = id ;
            Stat = new TStat { bImgClick = false, iClickX = 0, iClickY = 0 };
            ScaleOffset = new TScaleOffset { fOffsetX = 0, fOffsetY = 0, fScaleX = 1.0f, fScaleY = 1.0f };

            InitPkgList();
            UpdatePkgList();

            Stretch();

            ibCam.MouseWheel += new MouseEventHandler(ibCam_MouseWheel);

        }

        #region Init
        private void InitPkgList()
        {
            lvPkg.Clear();
            lvPkg.Columns.Add("No"    , 35 , HorizontalAlignment.Left);
            lvPkg.Columns.Add("Name"  , 290, HorizontalAlignment.Left);
        }

        private void UpdatePkgList()
        {
            lvPkg.Items.Clear();
            int iListCnt = 4;
            ListViewItem[] liPkg = new ListViewItem[iListCnt];
            for (int i = 0; i < iListCnt ; i++) liPkg[i] = new ListViewItem(i.ToString());

            liPkg[0].SubItems.Add("Homography");
            liPkg[1].SubItems.Add("Homography_Result");
            liPkg[2].SubItems.Add("Calibration");
            liPkg[3].SubItems.Add("Calibration_Result");

            for (int i = 0; i < iListCnt ; i++) lvPkg.Items.Add(liPkg[i]);

            liPkg[0].Selected = true;
            
            UpdatePkg();
        }
        #endregion

        private void UpdatePkg()
        {
            if (lvPkg.SelectedIndices.Count <= 0) return;
            int iSel = lvPkg.SelectedIndices[0];
            
            pgParaCommon.SelectedObject = GV.calibration[id].common.para ;

            if(iSel == 2 && GV.calibration[id].homography.para.Use == Calibration.USE_METHOD.Use) GV.calibration[id].calibrate.ori = GV.calibration[id].homography.rst2.Clone();

                 if(iSel == 0) { pgPara.SelectedObject = GV.calibration[id].homography.para  ; display = GV.calibration[id].homography.ori  ; }
            else if(iSel == 1) { pgPara.SelectedObject = null                                ; display = GV.calibration[id].homography.rst2 ; }
            else if(iSel == 2) { pgPara.SelectedObject = GV.calibration[id].calibrate.para   ; display = GV.calibration[id].calibrate.ori   ; }
            else if(iSel == 3) { pgPara.SelectedObject = null                                ; display = GV.calibration[id].calibrate.rst2  ; }
            
            //Tracker
                 if(iSel == 0) { GV.calibration[id].common.Tracker = GV.calibration[id].homography.Tracker ; GV.calibration[id].common.bTrackerDisplay = true ;}
            else if(iSel == 1) { GV.calibration[id].common.Tracker = GV.calibration[id].homography.Tracker ; GV.calibration[id].common.bTrackerDisplay = false;}
            else if(iSel == 2) { GV.calibration[id].common.Tracker = GV.calibration[id].calibrate.Tracker  ; GV.calibration[id].common.bTrackerDisplay = true ;}
            else if(iSel == 3) { GV.calibration[id].common.Tracker = GV.calibration[id].calibrate.Tracker  ; GV.calibration[id].common.bTrackerDisplay = false;}


            //ibTrainCam.Image = mat;

            ibCam.Invalidate();
        }
        
        private void UpdateOffset()
        {
            if (display == null) return;

            int iImgWidth  = display.Width  ;
            int iImgHeight = display.Height ;
            if (iImgWidth == 0 || iImgHeight == 0) return;

            if(rbStratch.Checked)
            {
                ScaleOffset.fOffsetX = 0;
                ScaleOffset.fOffsetY = 0;
                ScaleOffset.fScaleX = ibCam.Width /  (float)iImgWidth;
                ScaleOffset.fScaleY = ibCam.Height / (float)iImgHeight;
            }

            int iPanelWidth  = ibCam.Width  ;
            int iPanelHeight = ibCam.Height ;                        
            float fMaxOfsX =  iImgWidth   - iPanelWidth  / ScaleOffset.fScaleX ;
            float fMaxOfsY =  iImgHeight  - iPanelHeight / ScaleOffset.fScaleY ;
            if(fMaxOfsX < 0) fMaxOfsX /= 2.0f ;
            if(fMaxOfsY < 0) fMaxOfsY /= 2.0f ;
            if(ScaleOffset.fOffsetX < 0       ) ScaleOffset.fOffsetX = 0 ;
            if(ScaleOffset.fOffsetY < 0       ) ScaleOffset.fOffsetY = 0 ;
            if(ScaleOffset.fOffsetX > fMaxOfsX) ScaleOffset.fOffsetX = fMaxOfsX ;
            if(ScaleOffset.fOffsetY > fMaxOfsY) ScaleOffset.fOffsetY = fMaxOfsY ;
        }

        private void Stretch()
        {
            ScaleOffset.fOffsetX = 0;
            ScaleOffset.fOffsetY = 0;
            ScaleOffset.fScaleX = ibCam.Width  / (float)display.Width;
            ScaleOffset.fScaleY = ibCam.Height / (float)display.Height;
            UpdateOffset();
        }

        delegate void FInvalidate();
        public void PaintInvoke(Mat mat) //일단 외부 억세스 막고.  이벤트 방식에서 다시 바꿈.
        {
            display = mat.Clone();
            if (ibCam.InvokeRequired) // Invoke가 필요하면
            {
                try //라이브 상태에서 카메라 그랩 콜백이 남은 상태에서 종료 해버리면 ibTrainCam가 삭제 되었다고 Exception뜸.
                {
                     ibCam.Invoke(new FInvalidate(ibCam.Invalidate), new object[] { }); // 대리자를 호출
                }
                catch(Exception _e)
                {

                }
            }
            else
            { 
                ibCam.Invalidate();
            }
        }

        #region GetImage GetPanel
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
        #endregion

        #region MouseEvent
        private void ibTrainCam_MouseDown(object sender, MouseEventArgs e)
        {
            if (display.Bitmap == null) return;

            bool bTracker = GV.calibration[id].common.Tracker.MouseDown(Control.ModifierKeys , e , ScaleOffset);
            if(!bTracker) 
            {
                Stat.iClickX = e.X ;
                Stat.iClickY = e.Y ;
                Stat.bImgClick = true;
            }
        }

        private void ibTrainCam_MouseMove(object sender, MouseEventArgs e)
        {
            if (display.Bitmap == null) return;

            bool bTracker = GV.calibration[id].common.Tracker.MouseMove(Control.ModifierKeys , e , ScaleOffset);
            if(bTracker) ibCam.Invalidate();

            if (Stat.bImgClick)
            {
                //스케일 고려한 이동량.
                float fMoveX = GetImgX(e.X) - GetImgX(Stat.iClickX); 
                float fMoveY = GetImgX(e.Y) - GetImgX(Stat.iClickY); 

                Stat.iClickX = e.X ;
                Stat.iClickY = e.Y ;
                
                ScaleOffset.fOffsetX -= fMoveX ;
                ScaleOffset.fOffsetY -= fMoveY ;
                UpdateOffset();
                ibCam.Invalidate();
            }
            
            DispPixelData(e.X , e.Y);
        }


        private void DispPixelData(int _iPanelX , int _iPanelY)
        {
            Mat DispImg = display;
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
                lbPixel.Text = "<" + DispImg.Width + "." + DispImg.Height + "." + "> " +"(" + (int)dCrntImgX + "." + (int)dCrntImgY + ")\n";
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

        private void ibTrainCam_MouseUp(object sender, MouseEventArgs e)
        {
            if (display.Bitmap == null) return;

            GV.calibration[id].common.Tracker.MouseUp(Control.ModifierKeys , e);

            Stat.bImgClick = false;
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

        private void ZoomIn(int _iX, int _iY)
        {
            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 1.25f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 1.25f;

            float fMouseScaleX = _iX * 1.25f;
            float fMouseScaleY = _iY * 1.25f;

            float fMouseMoveX = fMouseScaleX - _iX;
            float fMouseMoveY = fMouseScaleY - _iY;

            ScaleOffset.fOffsetX += (fMouseMoveX / ScaleOffset.fScaleX);
            ScaleOffset.fOffsetY += (fMouseMoveY / ScaleOffset.fScaleY);

            UpdateOffset();
            ibCam.Invalidate();
        }

        private void ZoomOut(int _iX, int _iY)
        {
            if (ScaleOffset.fScaleX * 0.8f < 0.1 || ScaleOffset.fScaleY * 0.8f < 0.1) return;

            ScaleOffset.fScaleX = ScaleOffset.fScaleX / 1.25f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY / 1.25f;

            float fMouseScaleX = _iX / 1.25f;
            float fMouseScaleY = _iY / 1.25f;

            float fMouseMoveX = fMouseScaleX - _iX;
            float fMouseMoveY = fMouseScaleY - _iY;

            ScaleOffset.fOffsetX += (fMouseMoveX / ScaleOffset.fScaleX);
            ScaleOffset.fOffsetY += (fMouseMoveY / ScaleOffset.fScaleY);

            UpdateOffset();
            ibCam.Invalidate();
        }
        #endregion

        #region display
        private void ibTrainCam_Paint(object sender, PaintEventArgs e)
        {
            if (display.Bitmap == null) return;
            Graphics g = e.Graphics ;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode   = PixelOffsetMode.Half ;

            Display(g, ScaleOffset);

            if(GV.calibration[id].common.bTrackerDisplay) GV.calibration[id].common.Tracker.Paint(g , ScaleOffset);

            if(ScaleOffset.fScaleX > 25 && ScaleOffset.fScaleY > 25)
            {
                ViewerDisplayPixel(g);
            }
        }

        public bool Display(Graphics _g , TScaleOffset _ScaleOffset)//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            Rectangle Rect = new Rectangle(0,0,(int)_g.ClipBounds.Width , (int)_g.ClipBounds.Height) ;             
            try {
                if(display != null) 
                {
                    lock(display)
                    {
                        _g.DrawImage(display.Bitmap, Rect , 
                                     _ScaleOffset.fOffsetX , _ScaleOffset.fOffsetY , 
                                     _g.ClipBounds.Width  / _ScaleOffset.fScaleX , _g.ClipBounds.Height / _ScaleOffset.fScaleY , 
                                     GraphicsUnit.Pixel);
                    }
                }
            }
            catch(Exception _e){
                return false ;
            }

            return true ;
        }

        //뷰어 전체에 픽셀값 보여주는 함수
        public void ViewerDisplayPixel(Graphics _g)
        {
            Mat DispImg = display;            
            if (DispImg == null) return;

            float dDispImgStartX = GetImgX(0);
            float dDispImgStartY = GetImgY(0);
            float dDispImgEndX   = GetImgX((int)_g.ClipBounds.Width );
            float dDispImgEndY   = GetImgY((int)_g.ClipBounds.Height);     

            int iDispImgStartX = (int)dDispImgStartX;
            int iDispImgStartY = (int)dDispImgStartY;
            int iDispImgEndX   = (int)Math.Ceiling((double)dDispImgEndX  );
            int iDispImgEndY   = (int)Math.Ceiling((double)dDispImgEndY  );

            int iFontSize = 10;
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
                            sText = "R:" + iR.ToString() + " G:" + iG.ToString() + " B:" + iB.ToString();
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
        #endregion

        #region event
        private void FormCalibration_Shown(object sender, EventArgs e)
        {
            rbStratch.Checked = true;
            rbStratch.Focus();
            tmUpdate.Enabled  = true;
        }

        private void lvPkg_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePkg();
        }

        private void btLoadImg_Click(object sender, EventArgs e)
        {
            if (lvPkg.SelectedIndices.Count <= 0) return;
            int iSel = lvPkg.SelectedIndices[0];

            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "ImageFiles (*.bmp, *.jpg)|*.bmp;*.jpg";
            if (Openfile.ShowDialog() != DialogResult.OK) return ;

                 if(iSel == 0) { GV.calibration[id].homography.ori  = new Mat(Openfile.FileName); display = GV.calibration[id].homography.ori; GV.calibration[id].homography.para.Path = Openfile.FileName; }
            else if(iSel == 2) { GV.calibration[id].calibrate.ori   = new Mat(Openfile.FileName); display = GV.calibration[id].calibrate.ori;  GV.calibration[id].calibrate.para.Path  = Openfile.FileName; }
            
            this.Refresh();
            //ibTrainCam.Image = mat;
            //ibCam.Invalidate(true);
            //ibCam.Refresh();
        }

        private void btSaveImg_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Filter = "Bitmap Files|*.bmp";
            if (SaveFile.ShowDialog() != DialogResult.OK) return;
            display.Save(SaveFile.FileName);
            ibCam.Invalidate();
        }
        
        private void btZoomOut_Click(object sender, EventArgs e)
        {
            if(ScaleOffset.fScaleX * 0.8f < 0.1 || ScaleOffset.fScaleY * 0.8f < 0.1) return ;
            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 0.8f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 0.8f;         
            UpdateOffset();   
            ibCam.Invalidate();
        }

        private void btZoomIn_Click(object sender, EventArgs e)
        {
            ScaleOffset.fScaleX  = ScaleOffset.fScaleX * 1.25f;
            ScaleOffset.fScaleY  = ScaleOffset.fScaleY * 1.25f;       
            UpdateOffset();
            ibCam.Invalidate();
        }
        private void rbStratch_CheckedChanged(object sender, EventArgs e)
        {
            if(rbRealRatio.Checked)
            {
                ScaleOffset.fOffsetX = 0;
                ScaleOffset.fOffsetY = 0;
                ScaleOffset.fScaleX = 1.0f;
                ScaleOffset.fScaleY = 1.0f;                
            }
            else 
            {
                ScaleOffset.fOffsetX = 0;
                ScaleOffset.fOffsetY = 0;
                ScaleOffset.fScaleX = ibCam.Width  / (float)display.Width ;
                ScaleOffset.fScaleY = ibCam.Height / (float)display.Height;
            }
            UpdateOffset ();
            //ibCam.Refresh();
            ibCam.Invalidate();
        }

        private void btTrain_Click(object sender, EventArgs e)
        {
            if (lvPkg.SelectedIndices.Count <= 0) return;
            int iSel = lvPkg.SelectedIndices[0];

            if (display.Bitmap == null) return;

            bool bRet = false;
                 if(iSel == 0) { bRet = GV.calibration[id].FindHomography(GV.calibration[id].homography.ori ); }
            else if(iSel == 2) { bRet = GV.calibration[id].FindCalibrate (GV.calibration[id].calibrate.ori  ); }

            //Error check
            string sErr = GV.calibration[id].sErr;
            if(sErr == "") sErr = "Calibration Failed";
            if(!bRet) MessageBox.Show(sErr, "Error");
            
            ibCam.Invalidate();
        }

        private void btSavePara_Click(object sender, EventArgs e)
        { 
            btSavePara.Enabled = false;
            GV.calibration[id].Load(false);
            btSavePara.Enabled = true ;
        }
                
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
        #endregion

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

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            if(rbStratch.Checked)
            {
                btZoomIn.Enabled  = false;
                btZoomOut.Enabled = false;
            }
            else
            {
                btZoomIn.Enabled  = true;
                btZoomOut.Enabled = true;
            }
        }

        private void FormCalibration_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        private void FormCalibration_Move(object sender, EventArgs e)
        {
            ibCam.Invalidate();

        }

        private void btGrabCam_Click(object sender, EventArgs e)
        {

        }

        private void rbRealRatio_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btProfile_Click(object sender, EventArgs e)
        {

        }
    }

}
