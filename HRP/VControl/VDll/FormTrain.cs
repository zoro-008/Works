﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDll.Pakage;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

namespace VDll
{
    public partial class FormTrain : Form
    {
        struct TStat
        {
            public bool bImgClick ; //이미지 클릭.
            public int  iImgClickX; //이미지 클릭X
            public int  iImgClickY; //이미지 클릭Y
        };

        int iVisionID;
        TStat Stat = new TStat { iImgClickX = 0, iImgClickY = 0 , bImgClick = false};
        //TScaleOffset ScaleOffset = new TScaleOffset{fOffsetX = 0 , fOffsetY = 0 , fScaleX = 1.0f , fScaleY = 1.0f };

        FormProfile FrmProfile;
        CTracker    Tracker = new CTracker();

        //IPkg       Vision;
        Vision vision;

        public FormTrain(int _iVisionID)
        {
            InitializeComponent();

            iVisionID = _iVisionID ;
            vision    = GV.Visions[iVisionID];
            //Camera = vision.GetPkgCamera() ;
            //Vision = vision.GetPkgActivate();

            InitPkgList();
            UpdatePkgList();

            ibTrainCam.MouseWheel += new MouseEventHandler(ibTrainCam_MouseWheel);

            //Profile용 트래커 이건 그냥 생성자에 넣는게 낫겟다.
            Tracker.Init();
            Tracker.visible = false;
            Tracker.trackerType = CTracker.ETrackerType.ttLine;

            
            //rbRealRatio.Checked = true;
            rbStratch_CheckedChanged();
        }

        private void FormTrain_Load(object sender, EventArgs e)
        {
            rbStratch_CheckedChanged();
        }

        private void InitPkgList()
        {
            lvPkg.Clear();
            lvPkg.View = View.Details;
            lvPkg.AllowColumnReorder = false;
            lvPkg.FullRowSelect = true;
            lvPkg.MultiSelect = false ;
            lvPkg.GridLines = true;
            lvPkg.Sorting = SortOrder.None;

            lvPkg.Columns.Add("No"    , 25 , HorizontalAlignment.Left);
            lvPkg.Columns.Add("Name"  , 140, HorizontalAlignment.Left);
            lvPkg.Columns.Add("Type"  , 140, HorizontalAlignment.Left);
        }

        private void UpdatePkgList()
        {
            lvPkg.Items.Clear();

            int iPkgCnt = vision.Pkgs.Count ;
            ListViewItem[] liPkg = new ListViewItem[iPkgCnt];
            for (int i = 0; i < liPkg.Length ; i++)
            {
                liPkg[i] = new ListViewItem(i.ToString());
                liPkg[i].SubItems.Add(((CErrorObject)vision.Pkgs[i]).Name);
                liPkg[i].SubItems.Add(vision.Pkgs[i].GetType().Name);
                lvPkg.Items.Add(liPkg[i]);
            }

            if (liPkg.Length > 0)
            {
                lvPkg.Items[0].Focused  = true;
                lvPkg.Items[0].Selected = true;

                UpdatePropertyGrid();
            }
        }
        
        private void lvPkg_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePropertyGrid();
            
            TProp PkgProp = vision.GetProp();

            btLoadImg.Visible = PkgProp.bImgGrabber ;
            btSaveImg.Visible = PkgProp.bImgGrabber ;
            btSetting.Visible = PkgProp.bHaveDialog ;
            btCapture.Visible = PkgProp.bImgGrabber ;
        }

        private void UpdatePropertyGrid()
        {
            //여기 셀렉트 해제 되면 타서 SelectedIndices 하나도 세팅 안되는 경우 있음.
            int iSel = 0 ;
            try
            {
                iSel = lvPkg.SelectedIndices[0] ;
            }
            catch(Exception _e)
            {
                return ;
            }

            if(rbUserPara.Checked) pgPara.SelectedObject = vision.Pkgs[iSel].UPara ;
            else                   pgPara.SelectedObject = vision.Pkgs[iSel].MPara ;

            vision.SetPkgActivate(vision.Pkgs[iSel]);

            TRunRet  Ret  = new TRunRet ();
            TRunPara Para = new TRunPara();
            Para.bNeedPopImg = false;
            vision.Run(Para, ref Ret);

            ibTrainCam.Invalidate();
        }
        delegate void FInvalidateImageBox();
        public void PaintInvoke() //일단 외부 억세스 막고.  이벤트 방식에서 다시 바꿈.
        {
            if (ibTrainCam.InvokeRequired) // Invoke가 필요하면
            {
                try //라이브 상태에서 카메라 그랩 콜백이 남은 상태에서 종료 해버리면 ibTrainCam가 삭제 되었다고 Exception뜸.
                {
                     ibTrainCam.Invoke(new FInvalidateImageBox(ibTrainCam.Invalidate), new object[] { }); // 대리자를 호출
                }
                catch(Exception _e)
                {

                }
            }
            else
            { 
                ibTrainCam.Invalidate();
            }
        }

        private void FormTrain_Shown(object sender, EventArgs e)
        {
            //카메라 없을때 일단 확인 안하고 주석 처리함.
            //if(vision.Camera == null) return ;
            
            vision.VisnStats.Trainings = true ;
            GV.PaintCallbacks[iVisionID]= PaintInvoke ;

            //TScaleOffset SO = new TScaleOffset(0,0,0,0);

            //화면에 맞춰다시 스케일 지정하기 위해 초기화 하고
            //Paint함수에서 자동으로 Fit View로 맞춘다.
            foreach(IPkg Pkg in vision.Pkgs)
            {
                Pkg.ScaleOffset = new TScaleOffset(0,0,0,0);
            }



            tmLive.Enabled = true ;
            tmUpdate.Enabled = true ;


        }
        private void FormTrain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmLive.Enabled = false ;
            tmUpdate.Enabled = false ;
            vision.VisnStats.Trainings = false ;
            vision.SetPkgActivate(null);
        }

        private void FormTrain_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void FormTrain_Resize(object sender, EventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            UpdateOffset(ref ScaleOffset);
            vision.SetScaleOffset(ScaleOffset) ;

        }

        //마우스 휠 이벤트 (Zoom In/Out)
        private void ibTrainCam_MouseWheel(object sender, MouseEventArgs e)
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

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            
        }

        private void tmLive_Tick(object sender, EventArgs e)
        {
            vision.NeedGrab();
        }

        private void ibTrainCam_MouseDown(object sender, MouseEventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            bool bTracker = Tracker.MouseDown(Control.ModifierKeys , e , ScaleOffset );
            if (bTracker)
            {

            }
            //else if (vision.GetPkgActivate().MouseDown(e))
            //{
            //    //Stat.bTrkClick = true ; //sun 나중에 보고  bTrkClick 이거 자체를 빼버리자 Tracker 내부에 클릭 관련 변수 있어서. 필요 없을 수도 있음.
            //}
            else 
            {
                Stat.iImgClickX = e.X ;
                Stat.iImgClickY = e.Y ;
                Stat.bImgClick = true;
            }
            ibTrainCam.Invalidate();
        }

        //int iTemp = 0 ;
        private void ibTrainCam_MouseMove(object sender, MouseEventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();

            if(Tracker.MouseMove(Control.ModifierKeys , e , ScaleOffset))
            {
                ibTrainCam.Invalidate();
                FrmProfile.ProfilePaintInvoke();
            }
            //else if(vision.GetPkgActivate().MouseMove(e))
            //{
            //    ibTrainCam.Invalidate();
            //}          
            else if (Stat.bImgClick)
            {
                //스케일 고려한 이동량.
                float fMoveX = GetImgX(e.X) - GetImgX(Stat.iImgClickX); 
                float fMoveY = GetImgY(e.Y) - GetImgY(Stat.iImgClickY); 

                Stat.iImgClickX = e.X ;
                Stat.iImgClickY = e.Y ;

                ScaleOffset.fOffsetX -= fMoveX ;
                ScaleOffset.fOffsetY -= fMoveY ;                

                UpdateOffset(ref ScaleOffset);
                vision.SetScaleOffset(ScaleOffset) ;
                ibTrainCam.Invalidate();

                //iTemp++;
                //if(iTemp > 100000) iTemp = 0 ;
                //lbMousePoint.Text = iTemp.ToString() + " " + ScaleOffset.fOffsetX.ToString();
            }    
            DispPixelData(e.X , e.Y);            
        }

        private void ibTrainCam_MouseUp(object sender, MouseEventArgs e)
        {
            if(Tracker.MouseUp(Control.ModifierKeys , e))
            {

            }
            else if(vision.GetPkgActivate().MouseUp(e))
            {

            }

            Stat.bImgClick = false;
            ibTrainCam.Invalidate();
        }

        private void pgPara_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //if (rbUserPara.Checked) vision.GetPkgActivate().UPara = pgPara.SelectedObject ;
            //else                    vision.GetPkgActivate().MPara = pgPara.SelectedObject ;
            if (rbUserPara.Checked) vision.SetUPara(pgPara.SelectedObject) ;
            else                    vision.SetMPara(pgPara.SelectedObject) ;

            TRunRet  Ret  = new TRunRet ();
            TRunPara Para = new TRunPara();
            Para.bNeedPopImg = false;
            //vision.GetPkgActivate().Run(Para, ref Ret);
            vision.Run(Para, ref Ret);

            ibTrainCam.Invalidate();
        }
                       
        private float GetImgX(int _iPanelX)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            if (ScaleOffset.fScaleX == 0) return 0;
            float dRet = _iPanelX / ScaleOffset.fScaleX + ScaleOffset.fOffsetX;
            return dRet;
        }

        private float GetImgY(int _iPanelY)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            if (ScaleOffset.fScaleY == 0) return 0;
            float dRet = _iPanelY / ScaleOffset.fScaleY + ScaleOffset.fOffsetY;
            return dRet;
        }

        private int GetPanelX(float _dImgX)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            double dRet = (_dImgX - ScaleOffset.fOffsetX) * ScaleOffset.fScaleX;
            return (int)dRet;
        }

        private int GetPanelY(float _dImgY)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            double dRet = (_dImgY - ScaleOffset.fOffsetY) * ScaleOffset.fScaleY;
            return (int)dRet;
        }

        private void DispPixelData(int _iPanelX , int _iPanelY)
        {
            lock(GV.ImageLock[iVisionID])//lock(DispImg)//
            {     
                Mat DispImg = vision.GetImage() ;
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
                    lbPixel.Text = "<" + DispImg.Width + "." + DispImg.Height+"> " +"(" + (int)dCrntImgX + "." + (int)dCrntImgY + ")=";
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

        private void UpdateOffset(ref TScaleOffset _ScaleOffset)
        {
            //lock(GV.ImageLock[iVisionID])
            {
                int iImgWidth  = vision.GetImage().Width  ;
                int iImgHeight = vision.GetImage().Height ;
                if (iImgWidth == 0 || iImgHeight == 0) return;
                
                if(rbStratch.Checked)
                {
                    _ScaleOffset.fOffsetX = 0;
                    _ScaleOffset.fOffsetY = 0;
                    _ScaleOffset.fScaleX = ibTrainCam.Width /  (float)iImgWidth;
                    _ScaleOffset.fScaleY = ibTrainCam.Height / (float)iImgHeight;
                }

                int iPanelWidth  = ibTrainCam.Width  ;
                int iPanelHeight = ibTrainCam.Height ;                        
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
            float WidthScale  = ibTrainCam.Width  / (float)_Img.Width  ;
            float HeightScale = ibTrainCam.Height / (float)_Img.Height ;

            TScaleOffset ScaleOffset = new TScaleOffset();

            ScaleOffset.fScaleX  = WidthScale < HeightScale ? WidthScale : HeightScale ;
            ScaleOffset.fScaleY  = WidthScale < HeightScale ? WidthScale : HeightScale ;


            float fMaxOfsX =  _Img.Width   - ibTrainCam.Width  / ScaleOffset.fScaleX ;
            float fMaxOfsY =  _Img.Height  - ibTrainCam.Height / ScaleOffset.fScaleY ;
            if(fMaxOfsX < 0) fMaxOfsX /= 2.0f ;
            if(fMaxOfsY < 0) fMaxOfsY /= 2.0f ;
            if(ScaleOffset.fOffsetX < 0       ) ScaleOffset.fOffsetX = 0 ;
            if(ScaleOffset.fOffsetY < 0       ) ScaleOffset.fOffsetY = 0 ;
            if(ScaleOffset.fOffsetX > fMaxOfsX) ScaleOffset.fOffsetX = fMaxOfsX ;
            if(ScaleOffset.fOffsetY > fMaxOfsY) ScaleOffset.fOffsetY = fMaxOfsY ;

            return ScaleOffset;
        }

        private void ibTrainCam_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics ;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode   = PixelOffsetMode.Half ;

            TScaleOffset ScaleOffset = vision.GetScaleOffset();
            //if(ScaleOffset.fScaleX == 0 && ScaleOffset.fScaleY ==0)
            //{
            //    vision.SetFitScaleOffset(g.ClipBounds.Width , g.ClipBounds.Height , vision.GetImage().Width, vision.GetImage().Height);
            //}

            lock(GV.ImageLock[iVisionID])
            {
                //vision.GetPkgActivate().Paint(g , true);
                //vision.PaintScOf(g, true , ScaleOffset);                    
                vision.Paint(g, true);
                if (ScaleOffset.fScaleX > 29 && ScaleOffset.fScaleY > 29)
                {
                    ViewerDisplayPixel(g);
                }
                
                if(vision.GetPkgActivate().GetError() != "")
                {
                    using (Font font = new Font("Arial", 12))
                    using (SolidBrush brush = new SolidBrush(Color.Red))
                    {
                        g.DrawString(vision.GetPkgActivate().GetError()  , font , brush , new PointF(5,5));
                    }

                }
                Tracker.Paint(g, ScaleOffset);
            }

        }

        //뷰어 전체에 픽셀값 보여주는 함수
        public void ViewerDisplayPixel(Graphics _g)
        {
            Mat DispImg = vision.GetImage();
            if (DispImg == null) return;

            float dDispImgStartX = GetImgX(0);
            float dDispImgStartY = GetImgY(0);
            float dDispImgEndX = GetImgX((int)_g.ClipBounds.Width);
            float dDispImgEndY = GetImgY((int)_g.ClipBounds.Height);

            int iDispImgStartX = (int)dDispImgStartX;
            int iDispImgStartY = (int)dDispImgStartY;
            int iDispImgEndX = (int)Math.Ceiling((double)dDispImgEndX);
            int iDispImgEndY = (int)Math.Ceiling((double)dDispImgEndY);

            const int iFontSize = 8;
            Color color;
            int iR, iG, iB;
            string sText = "";

            using (Font font = new Font("Arial", iFontSize))
            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                for (int x = iDispImgStartX; x < iDispImgEndX; x++)
                {
                    for (int y = iDispImgStartY; y < iDispImgEndY; y++)
                    {
                        color = GetPixel(DispImg, x, y);//, DispImg.Cols, DispImg.ElementSize);
                        iR = color.R;
                        iG = color.G;
                        iB = color.B;

                        if (DispImg.ElementSize == 3)
                        {
                            sText = "R:" + iR.ToString() + "\r\n" + "G:" + iG.ToString() + "\r\n" + "B:" + iB.ToString();
                        }
                        else
                        {
                            sText = iR.ToString();
                        }

                        float Tempx = GetPanelX(x);
                        float Tempy = GetPanelY(y);
                        _g.DrawString(sText, font, brush, Tempx, Tempy);
                    }
                }
            }
        }
        
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


            ibTrainCam.Invalidate();

            /*
             if(vision.GetDisplayPkg()==null) return ;
            TScaleOffset ScaleOffset = vision.GetPkgActivate().ScaleOffset ;

            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 1.25f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 1.25f;

            float fMouseScaleX = _iX * 1.25f;
            float fMouseScaleY = _iY * 1.25f;

            float fMouseMoveX = fMouseScaleX - _iX;
            float fMouseMoveY = fMouseScaleY - _iY;

            ScaleOffset.fOffsetX += (fMouseMoveX / ScaleOffset.fScaleX);
            ScaleOffset.fOffsetY += (fMouseMoveY / ScaleOffset.fScaleY);

            UpdateOffset(ref ScaleOffset);
            vision.GetPkgActivate().ScaleOffset(ScaleOffset) ;


            ibCam.Invalidate();
             */
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


            ibTrainCam.Invalidate();
        }

        public Color GetPixel(Mat mat, int x, int y)//, int cols, int elementSize)
        {
            Color color;
            byte r = 0, g = 0, b = 0;

            int c = mat.Cols ;
            int e = mat.ElementSize ;
            unsafe
            {
                byte* Data = (byte*)mat.DataPointer;

                if (e == 3)
                {
                    b = *(Data + (y * c + x) * e + 0);
                    g = *(Data + (y * c + x) * e + 1);
                    r = *(Data + (y * c + x) * e + 2);
                    color = Color.FromArgb(r, g, b);
                }
                else
                {
                    r = *(Data + (y * c + x) * e);
                    color = Color.FromArgb(r, r, r);
                }
            }
            return color;
        }
        





        #region Button Event

        private void rbStratch_CheckedChanged() 
        { 
            //IPkg ActPkg = vision.GetPkgActivate() ;
            //if(ActPkg==null) return ;

            //Mat DispImg ;            
            //PCamera CamPkg = ActPkg as PCamera;
            
            //이렇게 분기 안해주면 CamPkg는 티칭시에 티칭이미지를 GetImage에서 리턴해서 보여주는 놈하고 스케일이 다르게 된다.
            //처음 PKG 추가 하고 나면 Capture이미지가 없어서 디스플레이가 안됌.
            //if(CamPkg == null)
            //{
            //    DispImg = ActPkg. GetImage();
            //}
            //else
            //{
            //    DispImg = CamPkg.GetAcitveImage();
            //}
            //
            //if (DispImg == null) return;
            //---------------------------------------------------------
            //TScaleOffset ScaleOffset = GetFitScaleOffset(DispImg);
            TScaleOffset ScaleOffset = vision.GetScaleOffset();

            if(rbRealRatio.Checked)
            {
                ScaleOffset.fOffsetX = 0;
                ScaleOffset.fOffsetY = 0;
                ScaleOffset.fScaleX = 1.0f;
                ScaleOffset.fScaleY = 1.0f;                
            }
            else if(rbFit.Checked)
            {
                ScaleOffset = GetFitScaleOffset(vision.GetImage());
            }
            else 
            {
                ScaleOffset.fOffsetX = 0;
                ScaleOffset.fOffsetY = 0;
                //ScaleOffset.fScaleX = ibTrainCam.Width  / (float)DispImg.Width ;
                //ScaleOffset.fScaleY = ibTrainCam.Height / (float)DispImg.Height;
                ScaleOffset.fScaleX = ibTrainCam.Width  / (float)vision.GetImage().Width ;
                ScaleOffset.fScaleY = ibTrainCam.Height / (float)vision.GetImage().Height;
            }
            UpdateOffset(ref ScaleOffset);
            //ActPkg.ScaleOffset=ScaleOffset ;
            vision.SetScaleOffset(ScaleOffset);
            ibTrainCam.Invalidate();
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
            vision.SetScaleOffset(ScaleOffset);

            ibTrainCam.Invalidate();
        }

        private void btZoomOut_Click(object sender, EventArgs e)
        {
            TScaleOffset ScaleOffset = vision.GetScaleOffset();

            if(ScaleOffset.fScaleX * 0.8f < 0.1 || ScaleOffset.fScaleY * 0.8f < 0.1) return ;
            ScaleOffset.fScaleX = ScaleOffset.fScaleX * 0.8f;
            ScaleOffset.fScaleY = ScaleOffset.fScaleY * 0.8f;

            UpdateOffset(ref ScaleOffset);
            vision.SetScaleOffset(ScaleOffset);

            ibTrainCam.Invalidate();
        }

        private void btLoadImg_Click(object sender, EventArgs e)
        {
            if(vision.Camera == null) return ;
            
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "ImageFiles (*.bmp, *.jpg)|*.bmp;*.jpg";
            if (Openfile.ShowDialog() != DialogResult.OK) return ;
            vision.Camera.LoadActiveImage(Openfile.FileName);

            //라이브를 멈춤 안그럼 갱신되어 버림.
            tmLive.Enabled = false ;
            ibTrainCam.Invalidate();
        }

        private void btSaveImg_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Filter = "ImageFiles (*.bmp, *.jpg)|*.bmp;*.jpg";
            if (SaveFile.ShowDialog() != DialogResult.OK) return;
            vision.SaveActiveImage(SaveFile.FileName);
            ibTrainCam.Invalidate();
        }

        private void btProfile_Click(object sender, EventArgs e)
        {
            Mat Image = vision.GetImage();

            FrmProfile = new FormProfile(Image, Tracker);
            FrmProfile.Show();
        }

        private void btPkgEdit_Click(object sender, EventArgs e)
        {
            using(FormPkg FrmPkg = new FormPkg(iVisionID))
            {
                FrmPkg.ShowDialog();
            }

            UpdatePkgList();

            //요건 일단 세이브 하자.---------
            if(!vision.LoadSave(false , GV.Para.DeviceFolder + GV.DeviceName + "\\Vision"+iVisionID.ToString())) 
            {
                MessageBox.Show(GV.Para.DeviceFolder + "\\" + GV.DeviceName + "\\" + " - Save Failed" , "Error");
            }
            ibTrainCam.Invalidate();
            //-------------------------------
        }

        private void btPre_Click(object sender, EventArgs e)
        {
            //여기 셀렉트 해제 되면 타서 SelectedIndices 하나도 세팅 안되는 경우 있음.
            lvPkg.Focus();
            int iSel = 0 ;
            try
            {
                iSel = lvPkg.SelectedIndices[0] ;
            }
            catch(Exception _e)
            {
                return ;
            }

            if(iSel <= 0)  return ;
            iSel--;
            lvPkg.Items[iSel].Focused = true;
            lvPkg.Items[iSel].Selected = true;     
            //UpdatePropertyGrid(); rbUserPara_CheckedChanged로 호출됨.
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            //여기 셀렉트 해제 되면 타서 SelectedIndices 하나도 세팅 안되는 경우 있음.
            lvPkg.Focus();
            int iSel = 0 ;
            try
            {
                iSel = lvPkg.SelectedIndices[0] ;
            }
            catch(Exception _e)
            {
                return ;
            }

            if(iSel >= vision.Pkgs.Count-1)  return ;

            iSel++;
            lvPkg.Items[iSel].Focused = true;
            lvPkg.Items[iSel].Selected = true;          

            //UpdatePropertyGrid(); rbUserPara_CheckedChanged로 호출됨.
        }

        private void btCapture_Click(object sender, EventArgs e)
        {
            PCamera Pkg = vision.GetPkgActivate() as PCamera ;
            if(Pkg == null)return ;
            Pkg.CaptureImg();
            
        }

        private void btSetting_Click(object sender, EventArgs e)
        {
        }

        private void rbUserPara_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePropertyGrid();
        }

        private void rbMasterPara_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if(!vision.LoadSave(false , GV.Para.DeviceFolder + GV.DeviceName + "\\Vision"+iVisionID.ToString())) 
            {
                MessageBox.Show(GV.Para.DeviceFolder + "\\" + GV.DeviceName + "\\" + " - Save Failed" , "Error");
            }

            //if()

            ibTrainCam.Invalidate();
        }

        private void btLocalValue_Click(object sender, EventArgs e)
        {
            using (FormValue FrmValue = new FormValue(iVisionID))
            {
                //FrmValue.LoadTable();
                FrmValue.ShowDialog();
            }
        }

        private void btGlobalValue_Click(object sender, EventArgs e)
        {
            using (FormValue FrmValue = new FormValue(0,true))
            {
                //FrmValue.LoadTable();
                FrmValue.ShowDialog();
            }
        }

        private void cbLiveCam_CheckedChanged(object sender, EventArgs e)
        {
            tmLive.Enabled = cbLiveCam.Checked;
        }

    }
    #endregion
}
