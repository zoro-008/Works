using COMMON;
using OpenCvSharp;
using SML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormCam : Form
    {
        CvCapture capture;
        IplImage src;
        CvVideoWriter OpenCV_video;
        bool isLoad ;
        public FormCam(Panel _pnBase)
        {
            InitializeComponent();
            this.TopLevel = false;
            this.Parent = _pnBase;
            this.Dock = DockStyle.Fill;

            MakeDoubleBuffered(toolStrip1,true);

            isLoad = false;
            if(!OM.CmnOptn.bIgnrCam) CamLoad();

        }

        public void CamLoad()
        {
            try
            {
                capture = CvCapture.FromCamera(CaptureDevice.DShow, 0);
                capture.SetCaptureProperty(CaptureProperty.FrameWidth, 680);
                capture.SetCaptureProperty(CaptureProperty.FrameHeight, 480);
                isLoad = true;
                //OpenCV_video = new CvVideoWriter(@"D:\Test.avi", "XVID", 15, Cv.GetSize(src));
            }
            catch
            {
                isLoad = false;
                //timer1.Enabled = false;
            }
        }
        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }
        

        private void bt1_Click(object sender, EventArgs e)
        {
            //Start();
        }

        private void bt2_Click(object sender, EventArgs e)
        {
            //Stop();
        }

        public void Start()
        {
            timer1.Enabled = true;
        }

        public void Stop()
        {
            timer1.Enabled = false;
            RecStop();
            //timer2.Enabled = false;
            //pMediaControl.Stop();
        }

        private void bt3_Click(object sender, EventArgs e)
        {
            //Rec();
        }

        public void Rec(string _sPath = "",int _iW = 640, int _iH = 480)
        {
            if(OM.CmnOptn.bIgnrCam) return ;

            string sPath = "";
            int iW = _iW;
            int iH = _iH;

            if (_sPath == "") sPath = "D:\\Test.avi";
            else              sPath = _sPath;

            if (Directory.Exists(Path.GetDirectoryName(sPath)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(sPath));

            //if(OpenCV_video != null) OpenCV_video.Dispose();
            OpenCV_video = new CvVideoWriter(sPath, "XVID", 30, Cv.GetSize(src));
            timer1.Enabled = true;
            timer2.Enabled = true;
        }
        public void RecStop()
        {
            timer2.Enabled = false;
            if(OpenCV_video != null) OpenCV_video.Dispose();

        }

        private void FormCam_Load(object sender, EventArgs e)
        {
            Start();
        }

        public struct CCamInfo
        {
            public int iInfo1;
            public int iInfo2;
            public int iInfo3;
            public string sInfo1;
        }
        //public static sTag Tag;
        public static CCamInfo CamInfo;
        public static void SaveInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sInfoPath = sExeFolder + @"Util\CamInfo.ini";
            //string sInfoPath = LogInfo.sLogPath + "Option\\LogInfo.ini";
            CAutoIniFile.SaveStruct<CCamInfo>(sInfoPath, "CamInfo", ref CamInfo);
        }

        public static void LoadInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sInfoPath = sExeFolder + @"Util\CamInfo.ini";
            //string sInfoPath = LogInfo.sLogPath + "Option\\LogInfo.ini";
            CAutoIniFile.LoadStruct<CCamInfo>(sInfoPath, "CamInfo", ref CamInfo);
        }

        bool bPreCam ;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            bool bNowCam = OM.CmnOptn.bIgnrCam ;
            if(!bNowCam && bPreCam != bNowCam)
            {
                if(capture != null && capture.IsEnabledDispose) capture.Dispose();
                CamLoad();
            }
            bPreCam = bNowCam ;


            if (Eqp.bDeviceCng2 && !OM.CmnOptn.bIgnrCam)
            {
                if(capture != null && capture.IsEnabledDispose) capture.Dispose();
                CamLoad();
                Eqp.bDeviceCng2 = false;
            }


            if(isLoad && !Eqp.bDeviceCng2 && !OM.CmnOptn.bIgnrCam)
            {
                if(OpenCV_video != null)
                { 
                    if (OpenCV_video.IsDisposed) bt3.Enabled = true ;
                    else                         bt3.Enabled = false;
                }
                if(capture.IsEnabledDispose)
                {
                    try { 
                        src = capture.QueryFrame();
                        pictureBoxIpl1.ImageIpl = src;
                    }
                    catch
                    {

                    }
                }
            }

            timer1.Enabled = true ;
        }

        private void FormCam_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible) timer1.Enabled = true;
        }
           
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            //if(!timer1.Enabled) return;
            if(isLoad && !Eqp.bDeviceCng2 && !OM.CmnOptn.bIgnrCam) OpenCV_video.WriteFrame(src);

            timer2.Enabled = true;
        }

        private void bt1_MouseDown(object sender, MouseEventArgs e)
        {
            Start();
        }

        private void bt2_MouseDown(object sender, MouseEventArgs e)
        {
            Stop();
        }

        private void bt3_MouseDown(object sender, MouseEventArgs e)
        {
            bt3.Enabled = false;
            Rec();
        }

        private void bt4_MouseDown(object sender, MouseEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg File|*.jpg";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;
            Cv.SaveImage(saveFileDialog1.FileName, src);
        }

        private void FormCam_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cv.ReleaseImage(src);
            src.Dispose();
        }

        private void saveToJpg(IntPtr Source, int Size, int height, int width)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg File|*.jpg";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;

            //15강에서는 stride 가 4에서 3으로 바뀌었습니다
            int stride = -3 * width;

            IntPtr Scan0 = (IntPtr)(((int)Source) + (Size - (3 * width)));
            //픽셀포맷역시 15강에서는 14강과 틀립니다 IBasicVideo.GetCurrentImage와 데이타 형식이 틀리기 때문입니다
            Bitmap img = new Bitmap(width, height, stride, PixelFormat.Format24bppRgb, Scan0);
            img.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);

            img.Dispose();
        }
    }
}
