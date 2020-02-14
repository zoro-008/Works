using COMMON;
using Emgu.CV;
using Emgu.CV.CvEnum;
using SML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormCam : Form
    {
        private const string sFormText = "Form Cam";
        
        public string SaveFolder { get; set; } = "d:\\CamRec\\";

        private int pnWidth ;
        private int pnHeight;

        private bool bRecord ;

        //EMGU CV
        private Mat         matBgr      ; //
        private VideoWriter videoWriter ;

        public FormCam()
        {
            InitializeComponent();

            pnWidth  = ibCam.Width ;
            pnHeight = ibCam.Height;

            
        }

        static public object lockObject = new object();
        private void ibCam_Paint(object sender, PaintEventArgs e)
        {
            lock(lockObject)
            { 
            //CvInvoke.Resize(mat,Display,new Size(pnWidth,pnHeight));
            //ibCam.Image = Display  ;
            if (matBgr == null) return ;

            //원본이미지와 페널이미지의 비율.
            TScaleOffset scaleOffset ;
            scaleOffset.fOffsetX = 0 ;
            scaleOffset.fOffsetY = 0 ;
            //scaleOffset.fScaleX  = pnHeight / (float)matBgra.Height ;
            //scaleOffset.fScaleY  = pnWidth  / (float)matBgra.Width  ;
            scaleOffset.fScaleX  = pnWidth  / (float)matBgr.Width  ;
            scaleOffset.fScaleY  = pnHeight / (float)matBgr.Height ;

            //일단 이미지를 그리고.
            Graphics g = e.Graphics ;
            PaintMat(matBgr , g , scaleOffset);

            //트레커 그림.
            //Tracker.GetRectangle
            //Tracker.Paint(g,scaleOffset);
            //const int iFontSize = 10;
            //
            //float fLeft   = GetScaleOffsetValX(Tracker.GetRectangle().Left , scaleOffset)  ;
            //float fTop    = GetScaleOffsetValY(Tracker.GetRectangle().Top  , scaleOffset)  ;

            //using (Pen pen = new Pen(Color.Yellow, 1))
            //using (SolidBrush brush = new SolidBrush(Color.Yellow))
            //using (Font font = new Font("Arial", iFontSize))
            //{
            //    g.DrawString(iAvr.ToString(), font, brush,fLeft ,fTop  - 15);
            //}
            }
        }

        public static bool PaintMat(Mat _Mat, Graphics _g, TScaleOffset _ScaleOffset, float _fMin = 0f, float _fMax = 0f)//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            Rectangle Rect = new Rectangle(0, 0, (int)_g.ClipBounds.Width, (int)_g.ClipBounds.Height);


            if (_Mat.Depth == DepthType.Cv8U)
            {
                try
                {
                    _g.DrawImage(_Mat.Bitmap,
                                 Rect,
                                 _ScaleOffset.fOffsetX,
                                 _ScaleOffset.fOffsetY,
                                 _g.ClipBounds.Width / _ScaleOffset.fScaleX,
                                 _g.ClipBounds.Height / _ScaleOffset.fScaleY,
                                 GraphicsUnit.Pixel);
                }
                catch (Exception _e)
                {
                    return false;
                }
            }
            //else if(_Mat.Depth == DepthType.Cv32F)
            //{
            //    double min = _fMin;
            //    double max = _fMax;
            //    int[] minIdx = null;
            //    int[] maxIdx = null;

            //    if(min == 0f && max == 0f)
            //    {
            //        CvInvoke.MinMaxIdx(_Mat, out min, out max, minIdx, maxIdx);
            //    }

            //    if(min == max) 
            //    {
            //        return false ;
            //    }

            //    using(Mat ScaledImg = new Mat(_Mat.Rows, _Mat.Cols, DepthType.Cv8U, 4))
            //    {
            //        double scale = 255.0 / (max - min);
            //        _Mat.ConvertTo(ScaledImg, DepthType.Cv8U, scale, -min * scale);

            //        try
            //        {
            //            _g.DrawImage(ScaledImg.Bitmap , 
            //                         Rect , 
            //                         _ScaleOffset.fOffsetX , 
            //                         _ScaleOffset.fOffsetY , 
            //                         _g.ClipBounds.Width  / _ScaleOffset.fScaleX , 
            //                         _g.ClipBounds.Height / _ScaleOffset.fScaleY , 
            //                         GraphicsUnit.Pixel);
            //        }
            //        catch(Exception _e)
            //        {
            //            return false ;
            //        }
            //    }

            //}
            else
            {
                return false;
            }
            return true;
        }

        float GetScaleOffsetValX(float _fOriX , TScaleOffset _ScaleOffset)
        {
            float fRet = (_fOriX - _ScaleOffset.fOffsetX)  * _ScaleOffset.fScaleX ;
            return fRet ;
        }

        float GetScaleOffsetValY(float _fOriY , TScaleOffset _ScaleOffset)
        {
            float fRet = (_fOriY - _ScaleOffset.fOffsetY)  * _ScaleOffset.fScaleY ;
            return fRet ;
        }


        private void btPlay_Click(object sender, EventArgs e)
        {

        }

        private void btRec_Click(object sender, EventArgs e)
        {

        }

        private void btStop_Click(object sender, EventArgs e)
        {

        }

        private void rbSave_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if(matBgr == null) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bmp File|*.bmp";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;
            //ibCam.Image.Save(saveFileDialog1.FileName);
            matBgr.Save(saveFileDialog1.FileName);
        }


        public void Rec(string _sPath = "",int _iw = 640, int _ih = 480)
        {
            int iw = _iw;
            int ih = _ih;

            string sPath = @"D:\CameraRec\" + System.DateTime.Now.ToString("yyyyMMdd") + @"\" +  
                           System.DateTime.Now.ToString("hh.mm.ss.avi");

            if (_sPath != "") sPath = _sPath;

            if (Directory.Exists(Path.GetDirectoryName(sPath)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(sPath));

            Size size = new Size(iw,ih);
            
            videoWriter = new VideoWriter(sPath, VideoWriter.Fourcc('X','V','I','D'), 30, size , true);

            bRecord = true;
        }
        static public object lockVideo = new object();
        public void RecStop()
        {
            lock (lockVideo) { 
            if(videoWriter != null) {
                videoWriter.Dispose();
                bRecord = false;
            }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            lock (lockVideo) { 
            if(videoWriter != null && videoWriter.IsOpened)
            {
                videoWriter.Write(matBgr);
            }
            }
            btRec. Enabled = !bRecord ;
            btStop.Enabled =  bRecord ;
            timer1.Enabled =  true    ;
        }

        private void FormCam_VisibleChanged(object sender, EventArgs e)
        {
            pnTop.Height = 30 ;
        }
    }
}
