using COMMON;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
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
    public partial class FormCam_XNB : Form
    {
        private const string sFormText = "Form Cam_XNB ";

        // Device Information
        private const string LOGIN_ID     = "admin";
        private const string PASSWORD = "gksfkwjdalf5%";
        //private const string PASSWORD     = "rlaehddn5%";
        //private const string IP_ADDRESS   = "169.254.232.254";  
        private const string IP_ADDRESS = "192.168.1.100";
        private const int    PORT_NUM     = 0;
        //private const int    HTTPPORT_NUM = 10006;
        private const int HTTPPORT_NUM = 80;
        private const string MODEL_NAME   = "Samsung Network Camera/Encoder";
        private const int    MODEL_INDEX  = 1 ;

        private const int TRUE            = 1;
        private const int FALSE           = 0;
        private const int NULL            = 0;
        private const int XADDRESS_IP     = 1;
        private const int SPEED_1         = 1;
                                          
        private int  hDevice              = 0;
        private int  hMediaSource         = 0;
        //private int hFileReader          = 0;
        private int  nControlID           = 0;
        private bool bRecord ;
        
        //EMGU CV
        private Mat         mat         ;
        private Mat         temp        ;
        private VideoWriter videoWriter ;

        private bool        bthreshold  ;
        private Mat         threshold   ;

        private bool        bPreDetected;
        private bool        bDetected   ;

        private Mat         Display     ;

        //Inspection
        private int         iAvr        ;

        public string SaveFolder { get; set; } = "d:\\CamSave\\";

        public bool   bInspection = false;

        private int pnWidth ;
        private int pnHeight;

        //public  int Avr


        public FormCam_XNB()
        {
            InitializeComponent();

            if(OM.CmnOptn.iMinA <  0  ) OM.CmnOptn.iMinA = 0;
            if(OM.CmnOptn.iMaxA <  0  ) OM.CmnOptn.iMaxA = 0;
            if(OM.CmnOptn.iMinA >= 255) OM.CmnOptn.iMinA = 255;
            if(OM.CmnOptn.iMaxA >= 255) OM.CmnOptn.iMaxA = 255;
            //Load
            numericUpDown1.Value = OM.CmnOptn.iThreshold ;
            numericUpDown2.Value = OM.CmnOptn.iMinA      ;
            numericUpDown3.Value = OM.CmnOptn.iMaxA      ;

            //Init
            if(OpenCamera() != "") Log.ShowMessage("Error" , "Camera Open Failed! - "+ OpenCamera());
            Delay(5000);
            if(PlayCamera() != "") Log.ShowMessage("Error" , "PlayCamera Failed! - "+ OpenCamera());
            
            //CallBack
            sdkWindow.SetVideoRawDataReceived(hMediaSource);

            iAvr = 0;
            pnWidth  = ibCam.Width ;
            pnHeight = ibCam.Height;

            Display = new Mat();
            
        }

        public void Delay(int ms)
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

        private string OpenCamera()
        {
            int iErrorCode = 0 ;
            iErrorCode = sdkCamera.Initialize();

            if(iErrorCode != 0)
            {
                return "sdkCamera.Initialize Failed - " + sdkCamera.GetErrorString(iErrorCode);
            }

            iErrorCode = sdkWindow.Initialize(0, 0);
            if(iErrorCode != 0)
            {
                return "sdkWindow.Initialize Failed - " + sdkCamera.GetErrorString(iErrorCode);
            }

            string strModel     = MODEL_NAME ;
            string strIpAddress = IP_ADDRESS ;
            int nPort           = PORT_NUM ;
            int nHttpPort       = HTTPPORT_NUM ;
            string strID        = LOGIN_ID ;
            string strPW        = PASSWORD ;
            int nDeviceProtocol = (int)XDeviceProtocolType.XDEVICE_PROTOCOL_SUNAPI;

            int device_id = sdkCamera.CreateDeviceEx();
            hDevice = sdkCamera.GetDeviceHandle(device_id);
            if (hDevice == NULL)
            {
                return "sdkCamera.CreateDevice() fail";
            }

            
            

            sdkCamera.SetConnectionInfo2(
                   hDevice,            // [in] Device handle. This value is returned from CreateDevice().
                   "Samsung",          // [in] Fixed as 'Samsung'. The maximum length allowed is 126-byte.
                   strModel,           // [in] Name of model to connect to. The maximum length allowed is 126-byte.
                   XADDRESS_IP,        // [in] Address type
                   strIpAddress,       // [in] Actual address according to nAddressType. 
                   nPort,              // [in] Port number
                   nHttpPort,          // [in] Port number for web access
                   strID,              // [in] Login ID. The maximum length allowed is 62-byte
                   strPW,              // [in] Login password. The maximum length allowed is 18-byte
                   nDeviceProtocol,
                   0);
            

            iErrorCode = sdkCamera.ConnectNonBlock(
                hDevice,            // [in] Device handle. This value is returned from CreateDevice(). 
                FALSE,              // [in] Flag to decide where to forcibly log in or not.
                FALSE);             // [in] When the device is disconnected, 
                                    //      you can use this flag to decide if to connect again automatically. 
                                    //      If this value is 1, try to connect again.

            if (iErrorCode != 0)
            {
                return "sdkCamera.ConnectNonBlock Failed - " + sdkCamera.GetErrorString(iErrorCode);
            }

            return "";
        }

        private string PlayCamera()
        {
            if (sdkCamera.GetDeviceStatus(hDevice) != ((int)XConnect.XDEVICE_STATUS_CONNECTED))
            {
                return "Not connected" ;
            }

            if ((sdkCamera.GetControlType(hDevice, 1) == ((int)XModelType.XCTL_NETCAM)) ||
                 (sdkCamera.GetControlType(hDevice, 1) == ((int)XModelType.XCTL_ENCODER)))
            {
                nControlID = sdkCamera.GetVideoControlID(hDevice, 1, 2);
            }
            else
            {
                nControlID = sdkCamera.GetVideoControlID(hDevice, 1, 1);
            }

            // [ XNS ACTIVEX HELP ]
            // -----------------------------------------------------------------------
            // When called, it will start getting media streams from the device.
            // The receiving media streams will, then, be forwarded to the XnsSdkWindow 
            // component that will play the streams after decoding.
            // phMediaSource is needed to link the stream data with XnsSdkWindow. 
            // The value can be obtained from a parameter (out-parameter) of OpenMedia(). 
            // When XnsSdkWindow receives this value, it can get stream data from the device.
            // phMediaSource is also used for controlling playback of multimedia files. 
            // As a result, the application should keep this value at all times.
            // -----------------------------------------------------------------------
            int iErrorCode = sdkCamera.OpenMedia(hDevice, nControlID, ((int)XMediaType.XMEDIA_LIVE), 0, 0, ref hMediaSource);
            if (iErrorCode != (int)XErrorCode.ERR_SUCCESS)
            {
                return "sdkCamera.OpenMedia Failed - " + sdkCamera.GetErrorString(iErrorCode);
            }

            // [ XNS ACTIVEX HELP ]
            // -----------------------------------------------------------------------
            // Adds the media source handle to XnsSdkWindow. 
            // The media source handle is created by XnsSdkDevice. 
            // If the application calls XnsSdkDevice::OpenMedia(), 
            // it will receive media stream from the device and return the MediaSource 
            // handle. The application uses uses Start() to forward the mediasource 
            // handle to XnsSdkWindow so that XnsSdkWindow can obtain stream data. 
            // -----------------------------------------------------------------------
            iErrorCode = sdkWindow.Start(hMediaSource);
            if (iErrorCode != (int)XErrorCode.ERR_SUCCESS)
            {
                return "sdkWindow.Start Failed" ;
            }
            return "";
        }

        private string StopCamera()
        {
            hMediaSource = sdkWindow.Stop();

            // [ XNS ACTIVEX HELP ]
            // -----------------------------------------------------------------------
            // Terminates transferring media stream data from the device. The media 
            // stream data will be separated by hMediaSource 
            // (i.e., phMediaSource of OpenMedia()).
            // -----------------------------------------------------------------------
            int iErrorCode = sdkCamera.CloseMedia(hDevice, hMediaSource);
            if (iErrorCode != (int)XErrorCode.ERR_SUCCESS)
            {
                return "sdkWindow.Stop Failed" ;
            }

            return "";

        }

        private bool Play()
        {
            //if(sdkCamera.GetControlCapability(hDevice,2,(int)XUnitCap.XCTL_CAP_LIVE) == 1)
            //{ 
            if(hMediaSource == NULL)
            { 
                sdkCamera.OpenMedia(hDevice, nControlID, ((int)XMediaType.XMEDIA_LIVE), 0, 0, ref hMediaSource);
                sdkWindow.SetVideoRawDataReceived(hMediaSource);
                sdkWindow.Start(hMediaSource);
                //Delay(300);
            }
            //}
            return true;
        }

        private bool Stop()
        {
            if (hMediaSource != NULL && sdkWindow.IsMedia() == TRUE)
            {
                sdkWindow.UnSetVideoRawDataReceived(hMediaSource);
                Delay(100);
                hMediaSource = sdkWindow.Stop();
                sdkCamera.CloseMedia(hDevice, hMediaSource);
                hMediaSource = NULL;
                return true;
            }
            return false;
        }

        private string CloseCamera()
        {
            if (sdkWindow.IsMedia() == TRUE)
            {
                sdkWindow.UnSetVideoRawDataReceived(hMediaSource);
                Delay(100);
                hMediaSource = sdkWindow.Stop();
            }

            if (hMediaSource != 0)
            {
                sdkCamera.CloseMedia(hDevice, hMediaSource);

                int iErrorCode = sdkCamera.Disconnect(hDevice);
                if (iErrorCode != (int)XErrorCode.ERR_SUCCESS)
                {
                    return "sdkCamera.Disconnect failed" ;
                    
                }

                sdkCamera.ReleaseDevice(hDevice);
            }
            return "";

        }

        private void FormCam_XNB_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;

            OM.SaveCmnOptn();
            //RecStop();
            string sTemp= CloseCamera();
            if(sTemp != "")
            {
                Log.ShowMessage("Error","CloseCamera Failed! - "+sTemp);
                //MessageBox.Show("CloseCamera Failed! - "+sTemp);
            }
        }

        private void rbPlay_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbRec_CheckedChanged(object sender, EventArgs e)
        {


        }

        public void Rec(string _sPath = "",int _iW = 640, int _iH = 480)
        {
            string sPath = "";
            int iW = _iW;
            int iH = _iH;

            if (_sPath == "") sPath = SaveFolder + System.DateTime.Now.ToString("yyyyMMdd_hhmmss.avi");
            else              sPath = _sPath;

            if (Directory.Exists(Path.GetDirectoryName(sPath)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(sPath));

            Size size = new Size(iW,iH);
            //if(videoWriter != null) videoWriter.Dispose();
            
            //videoWriter = new VideoWriter(sPath, VideoWriter.Fourcc('X','2','6','4'), 30, size , true);
            videoWriter = new VideoWriter(sPath, VideoWriter.Fourcc('X','V','I','D'), 30, size , true);
            //videoWriter = new VideoWriter(sPath, VideoWriter.Fourcc('M','P','E','G'), 30, size , true);
            //videoWriter = new VideoWriter(sPath, 5, 30, size , true);
            //videoWriter = new VideoWriter(sPath, -1, 30, new Size(iW,iH), true);
            //videoWriter.Set(VideoWriter.WriterProperty.Framebytes,30);
            //videoWriter.Set(VideoWriter.WriterProperty.NStripes,30);
            //videoWriter.Set(VideoWriter.WriterProperty.Quality,30);

            

            bRecord = true;
        }

        public void RecStop()
        {
            if(videoWriter != null) {
                videoWriter.Dispose();
                bRecord = false;
            }
        }

        private void btStop_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void sdkWindow_OnVideoRawDataReceived(object sender, AxXNSSDKWINDOWLib._DXnsSdkWindowEvents_OnVideoRawDataReceivedEvent e)
        {
            IntPtr pVideoData = new IntPtr(e.pVideoData);
            temp      = new Mat(e.nHeight,e.nWidth, Emgu.CV.CvEnum.DepthType.Cv8U,4,pVideoData,e.nWidth*4);
            mat       = new Mat(e.nHeight,e.nWidth, Emgu.CV.CvEnum.DepthType.Cv8U,3);
            //mat       = new Mat(pnHeight,pnWidth, Emgu.CV.CvEnum.DepthType.Cv8U,3);
            threshold = new Mat(e.nHeight,e.nWidth, Emgu.CV.CvEnum.DepthType.Cv8U,3);
            CvInvoke.CvtColor(temp , mat ,Emgu.CV.CvEnum.ColorConversion.Bgra2Bgr);                    
            
            //녹화
            if(videoWriter != null && videoWriter.IsOpened) videoWriter.Write(mat);
            //영상 이진화
            CvInvoke.CvtColor(mat, threshold, ColorConversion.Bgr2Gray);        
            //CvInvoke.Threshold(threshold,threshold,OM.CmnOptn.iThreshold,255,Emgu.CV.CvEnum.ThresholdType.Binary);
            
            if(bthreshold) {
                CvInvoke.Resize(threshold,Display,new Size(pnWidth,pnHeight));
                ibCam.Image = Display;
            }
            else           {
                CvInvoke.Resize(mat,Display,new Size(pnWidth,pnHeight));
                ibCam.Image = Display  ;
            }

            
            if (bInspection) { 
                bDetected = false;
                if(DetectBlobs(threshold,OM.CmnOptn.iMinA,OM.CmnOptn.iMaxA))
                {
                    bDetected = true;
                    if(!bPreDetected) OM.CmnOptn.iDetectCount++;
                }
                bPreDetected = bDetected;
            }
            
            
        }

        private int iPreAvr = 0;
        private bool DetectBlobs(Mat im, int _minA, int _maxA, bool bShow = false)
        {
            if(im == null) return false;
            int minA = _minA; // Minimum area in pixels
            int maxA = _maxA; // Maximum area in pixels
            if (minA < 1) minA = 1;

            int iSum = 0;
            int iCnt = 0;
            iAvr = 0;
            for(int i=0; i<im.Width; i++)
            {
                for(int j=0; j<im.Height; j++)
                {
                    Color cl = GetPixel(im,i,j);
                    if(cl.R > minA)
                    {
                        iSum += cl.R;
                        iCnt ++;
                    }
                }
            }

            if(iCnt > 0) iAvr = iSum / iCnt;
            if(iPreAvr < iAvr)
            {
                if(iAvr > maxA) return true;
            }
            iPreAvr = iAvr;
            
            return false;

            /*
            SimpleBlobDetectorParams param = new SimpleBlobDetectorParams();
            param.FilterByCircularity = false;
            param.FilterByConvexity   = false;
            param.FilterByInertia     = false;
            param.FilterByColor       = false;
            param.MinArea = minA;
            param.MaxArea = maxA;

            SimpleBlobDetector detector = new SimpleBlobDetector(param);
            VectorOfKeyPoint keyPoints = new VectorOfKeyPoint();
            detector = new SimpleBlobDetector(param);
            detector.DetectRaw(im, keyPoints);
            //detector.Detect(im, keyPoints);

            int iBlobSize = 0;
            if(bShow)
            { 
                Mat im_with_keypoints = new Mat();
                Features2DToolbox.DrawKeypoints(im, keyPoints, im_with_keypoints, new Bgr(255, 0, 0), Features2DToolbox.KeypointDrawType.DrawRichKeypoints);
                
                // Show blobs
                //CvInvoke.Imwrite("keypoints1.jpg", im_with_keypoints);
                CvInvoke.Imshow("Blob Detector " + keyPoints.Size, im_with_keypoints);
            }
            //float i1 = keyPoints[0].Size;
            //float i2 = keyPoints[1].Size;
            //Result
            if(keyPoints.Size > 0) return true;
            return false;
            */
        }

        private void btPlay_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            Play();
        }

        private void btRec_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //OpenCamera();
            //Log.Trace("btRec_Click 1");
            //Play();
            //Log.Trace("btRec_Click 2");
            //if(!bRecord) Rec("",pnWidth,pnHeight);
            if(!bRecord) Rec();
            //Log.Trace("btRec_Click 3");
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if(bRecord) RecStop();
            
            //Log.Trace("btStop_Click 1");
            //Stop();
            //Log.Trace("btStop_Click 2");
            
            //Log.Trace("btStop_Click 3");
            
        }

        private void rbSave_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bmp File|*.bmp";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;
            ibCam.Image.Save(saveFileDialog1.FileName);
        }

        private void btRst_Click(object sender, EventArgs e)
        {
            
        }

        private bool bFirst = true;
        private void sdkCamera_OnDeviceStatusChanged(object sender, AxXNSSDKDEVICELib._DXnsSdkDeviceEvents_OnDeviceStatusChangedEvent e)
        {
            if(e.nDeviceStatus == 1 && !bFirst)
            {
                if(sdkCamera.GetDeviceStatus(hDevice) == 1)
                {
                    //sdkCamera.OpenMedia(hDevice, nControlID, ((int)XMediaType.XMEDIA_LIVE), 0, 0, ref hMediaSource);
                    hMediaSource = NULL;
                    Play();
                }
            }
            bFirst = false;
        }

        private void btOri_Click(object sender, EventArgs e)
        {
            bthreshold = false;
        }

        private void btThr_Click(object sender, EventArgs e)
        {
            bthreshold = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            OM.CmnOptn.iThreshold = (int)numericUpDown1.Value;
        }

        private void btInsp_Click(object sender, EventArgs e)
        {
            btInsp.Enabled = false;
            DetectBlobs(threshold,OM.CmnOptn.iMinA,OM.CmnOptn.iMaxA,true);
            btInsp.Enabled = true ;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            OM.CmnOptn.iMinA = (int)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            OM.CmnOptn.iMaxA = (int)numericUpDown3.Value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            lbCount.Text = OM.CmnOptn.iDetectCount.ToString() ;
            lbAvr.Text   = iAvr.ToString() ;

            btRec. Enabled = !bRecord ;
            btStop.Enabled =  bRecord ;
            timer1.Enabled =  true    ;
            //
        }

        private void btLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "ImageFiles (*.bmp, *.jpg)|*.bmp;*.jpg";
            if (Openfile.ShowDialog() != DialogResult.OK) return ;

            Mat imLoad = new Mat(Openfile.FileName);
            
            CvInvoke.CvtColor(imLoad, imLoad, ColorConversion.Bgr2Gray);        
            //CvInvoke.Threshold(imLoad,imLoad,OM.CmnOptn.iThreshold,255,Emgu.CV.CvEnum.ThresholdType.Binary);
            
            DetectBlobs(imLoad,OM.CmnOptn.iMinA,OM.CmnOptn.iMaxA,true);

        }

        private void btSetting_Click(object sender, EventArgs e)
        {
            pnTitle1.Visible = !pnTitle1.Visible;
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

        private void FormCam_XNB_Shown(object sender, EventArgs e)
        {
            pnWidth  = ibCam.Width ;
            pnHeight = ibCam.Height;
        }
    }
}

