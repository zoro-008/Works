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
using System.Diagnostics;
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

        public string SaveFolder { get; set; } = "d:\\CamSave\\";
        public bool   bInspection = false;

        private int pnWidth ;
        private int pnHeight;
        
        //EMGU CV
        private Mat         matBgra     ; //카메라에서 Bgra로 들어옴.
        //private Mat         matBgr      ; //Bgra=>Bgr로 변환
        private Mat         matInsp     ; //영상 검사용
        private VideoWriter videoWriter ;

        public int ArkCnt { get { return iArkCnt;} set { iArkCnt = value;} }
        //Inspection        
        private int         iAvr        ; //트레커의 사각형의 R영역 밝기 평균 OM.CmnOptn.iMinA 보다 큰놈만..
        private int         iArkCnt     ; //아킹으로 설정값 이하로 떨어졌을경우.
        private const int   iArkFrmCnt = 10 ; //평균값이 처음에 떨어지고 10프레임 
        public  const int   iAvrQueMax = 100; //큐 맥스 사이즈
        Queue<int>          inspAvrQue = new Queue<int>();

        //public  int Avr
        public  CTracker    Tracker = new CTracker();



                //검사 관련.
        //private bool        bthreshold  ;
        //private Mat         threshold   ;

        //private bool        bPreDetected;
        //private bool        bDetected   ;


        public FormCam_XNB()
        {
            

            InitializeComponent();

            if(OM.CmnOptn.iMinA     <  0          ) OM.CmnOptn.iMinA     = 0             ;
            if(OM.CmnOptn.iMinA     >= 255        ) OM.CmnOptn.iMinA     = 255           ;
            if(OM.CmnOptn.iCheckGap <  1          ) OM.CmnOptn.iCheckGap = 1             ;            
            if(OM.CmnOptn.iCheckGap >= iAvrQueMax ) OM.CmnOptn.iCheckGap = iAvrQueMax - 1;
            //Load
            numericUpDown1.Value = OM.CmnOptn.iThreshold    ;
            numericUpDown2.Value = OM.CmnOptn.iMinA         ;
            numericUpDown3.Value = OM.CmnOptn.iCheckGap     ;

            //Init
            string sMsg1 = OpenCamera();
            if(sMsg1 != "") Log.ShowMessage("Error" , "Camera Open Failed! - "+ sMsg1);
            //Delay(1000);
            string sMsg2 = PlayCamera();
            if(sMsg2 != "") Log.ShowMessage("Error" , "PlayCamera Failed! - " + sMsg2);

            //CallBack
            try { 
                sdkWindow.SetVideoRawDataReceived(hMediaSource);
            }
            catch
            {
                Log.Trace("Error sdkWindow");
            }

            iAvr = 0;
            pnWidth  = ibCam.Width ;
            pnHeight = ibCam.Height;

            //Tracker 초기화

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath      = sExeFolder + "Util\\Tracker.ini";

            Tracker.Init();
            Tracker.visible = true  ;
            Tracker.enabled = false ;
            Tracker.trackerType = CTracker.ETrackerType.ttRect ;
            Tracker.LoadSave(true , sPath);

            matBgra= new Mat(480,640, DepthType.Cv8U,3);
            ////마지막 이미지 로딩.
            //try
            //{ 
            //    matBgra= new Mat(SaveFolder + "LastImg.bmp");
            //}
            //catch(Exception _e)
            //{
            //    matBgra= new Mat(480,640, DepthType.Cv8U,3);
            //}         
            
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
            try
            {
                int iErrorCode = 0 ;
                iErrorCode = sdkCamera.Initialize();
                
                
                if(iErrorCode != 0)
                {
                    return "sdkCamera.Initialize Failed - " + sdkCamera.GetErrorString(iErrorCode);
                
                }
                
                bool b1 = sdkCamera.GetDeviceStatus(hDevice) == 1;

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
            }
            catch
            {
                Log.Trace("Error sdkWindow");
            }

            return "";
        }

        private string PlayCamera()
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while(sdkCamera.GetDeviceStatus(hDevice) != 1)
                {
                    if (sw.ElapsedMilliseconds > 6000)
                    {
                        //System.Windows.Forms.Application.DoEvents();
                        return "Not connected" ;
                        //break;
                    }
                
                }
                
                
                //if (sdkCamera.GetDeviceStatus(hDevice) != ((int)XConnect.XDEVICE_STATUS_CONNECTED))
                //{
                //    return "Not connected" ;
                //}
                
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
            }
            catch
            {
                Log.Trace("Error sdkWindow");
            }
           
            return "";
        }

        private string StopCamera()
        {
            try
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
            }
            catch
            {
                Log.Trace("Error sdkWindow");
            }

            return "";

        }

        //static public object lockVideo = new object();
        private bool Play()
        {
            try
            {
                lock (lockVideo) { 
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
                }
            }
            catch
            {
                Log.Trace("Error sdkWindow");
            }
            return true;
        }

        private bool Stop()
        {
            try
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
            }
            catch
            {
                Log.Trace("Error sdkWindow");
            }
            return false;
        }

        private string CloseCamera()
        {
            try
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
            }
            catch
            {
                Log.Trace("Error sdkWindow");
            }
            return "";
        }

        private void FormCam_XNB_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                
                //마지막 이미지 로딩.
                //matBgra.Save(SaveFolder + "LastImg.bmp");
                
                
                
                //RecStop();
                string sTemp= CloseCamera();
                if(sTemp != "")
                {
                    Log.ShowMessage("Error","CloseCamera Failed! - "+sTemp);
                    //MessageBox.Show("CloseCamera Failed! - "+sTemp);
                }
                Tracker.Close(); //클로즈 카메라 뒤에 있어야 합니당...스톱하고 뒤에 Close.
            }
            catch
            {
                Log.Trace("Error sdkWindow");
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
            try
            {
                lock (lockVideo) { 

                bRecord = true;

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
                
                
                
                
                }
            }
            catch
            {
                Log.Trace("Error videoWriter");
            }
        }

        static public object lockVideo = new object();
        public void RecStop()
        {
            try
            {
                lock (lockVideo) { 
                if(videoWriter != null) {
                    videoWriter.Dispose();
                    bRecord = false;
                }
                }
            }
            catch
            {
                Log.Trace("Error videoWriter");
            }
        }

        private void btStop_CheckedChanged(object sender, EventArgs e)
        {

        }

        //3채널에서 R영역만 분리해서 그래이에 집어넣음.
        //private bool GetRedChannel(Mat source3ChMat , ref Mat dest1chMat)
        //{
        //    if(source3ChMat.Depth != DepthType.Cv8U) return false ;
        //    if(source3ChMat.NumberOfChannels != 3)   return false ;
        //    //Red 영역을 추출하여 검사용 Gray이미지로 사용.
        //    CvInvoke.cvSetImageCOI(source3ChMat , 3); //관심 채널 3번세팅 R.
        //    CvInvoke.cvCopy       (source3ChMat , dest1chMat , IntPtr.Zero); //matR에 3번채널 복사
        //    CvInvoke.cvSetImageCOI(source3ChMat , 0); //관심 채널 해재.
        //    return true ;
        //}

        bool bDownning  = false ;
        Stopwatch sw = new Stopwatch();
        //int  iGrabCount = 0     ;
        static public object lockObject = new object();
        private void sdkWindow_OnVideoRawDataReceived(object sender, AxXNSSDKWINDOWLib._DXnsSdkWindowEvents_OnVideoRawDataReceivedEvent e)
        {
            try
            {
                lock(lockObject)
                { 
                //Log.Trace("-1", ti.Frm);
                IntPtr pVideoData = new IntPtr(e.pVideoData);
                
                if(matBgra==null || matBgra.Width != e.nWidth || matBgra.Height != e.nHeight) //아직 널이거나 사이즈 안맞으면.
                {
                    matBgra = new Mat(e.nHeight,e.nWidth, DepthType.Cv8U,4,pVideoData,e.nWidth*4);
                    //Log.Trace("matBgra", ti.Frm);
                }
                else
                {
                    //matBgra.Dispose();
                    matBgra = new Mat(e.nHeight,e.nWidth, DepthType.Cv8U,4,pVideoData,e.nWidth*4);
                    //matInsp = new Mat(e.nHeight,e.nWidth, DepthType.Cv8U,4,pVideoData,e.nWidth*4);
                    matBgra = matBgra.Clone();
                    //matInsp = matBgra.Clone();
                    pVideoData = IntPtr.Zero;
                    //CvInvoke.cvCopy(pVideoData , matBgra.DataPointer , IntPtr.Zero);
                    //Log.Trace("matBgra", ti.Frm);
                }              
                }
            }
            catch
            {
                Log.Trace("Error matBgra");
            }
            //CvInvoke.CvtColor(matBgra , matBgr ,Emgu.CV.CvEnum.ColorConversion.Bgra2Bgr);            
            
                //iGrabCount++;
            //if(iGrabCount > 100000) iGrabCount = 0;
            
            //matBgr  = matInsp.Clone();
            


            try
            {
                double dAvr = 0 ;
                if(CheckThreshold(matBgra , Tracker.GetRectangle() , OM.CmnOptn.iMinA , ref dAvr))
                { 
                    iAvr = (int)dAvr;
                
                    inspAvrQue.Enqueue(iAvr);
                    while(inspAvrQue.Count >= iAvrQueMax)
                    {
                        inspAvrQue.Dequeue();
                    }
                    
                    if(inspAvrQue.Count > iArkFrmCnt)
                    { 
                        int[] inspAvrArray = new int[inspAvrQue.Count * 2]; //어레이로 변환시 2배로 사이즈를 해줘야함.
                        inspAvrArray = inspAvrQue.ToArray();
                        if(inspAvrArray[inspAvrQue.Count-1-iArkFrmCnt] - inspAvrArray[inspAvrQue.Count-1] > OM.CmnOptn.iCheckGap)
                        {
                            if(!bDownning)
                            {
                                bDownning = true ;
                                iArkCnt++;
                                if(iArkCnt > 100000) iArkCnt = 0;
                                if(SEQ.aging.bInspection && SEQ._bRun) //전압 상승 구간
                                {
                                    OM.CmnOptn.iDetectCount++;
                                }
                            }
                        }
                        else
                        {
                            bDownning = false ;
                        }
                    }
                }
            }
            catch
            {
                Log.Trace("Error CheckThreshold");
            }

            //화면 갱신.
            ibCam.Invalidate();
            
        }

        private bool CheckThreshold(Mat im , Rectangle inspRect , int minThreshold , ref double retThreshold)
        {
            if(im==null) return false ;
            if(im.Depth != DepthType.Cv8U)return false ; //8비트언사인드만 검사.
            if(im.NumberOfChannels != 4)return false ;//한픽셀 4채널 즉 32비트만 검사. BGRA타입.
            if(inspRect.Width<1 )return false ;
            if(inspRect.Height<1)return false ;
            
            double dSum=0;
            double dAvr=0;
            int   iPxCnt = 0 ;
            unsafe //픽셀을 바로접근하기 위해 씀... 속도때문에.
            {
                byte* Data = (byte *)im.DataPointer;   
                byte  CrntPx = 0 ;  //현재 픽셀 밝기.
                byte  tempR ;
                byte  tempG ;
                byte  tempB ;
                byte  tempA ;
                int   stride = im.Step ;
                for(int y = inspRect.Top ; y < inspRect.Bottom ; y+=3)
                {
                    for(int x = inspRect.Left ; x < inspRect.Right ; x+=3)
                    {
                        tempA = *(Data + (y * stride) + (x*im.NumberOfChannels)+3) ;//BGR이여서 R채널껏만 검사한다. 이미지가 레드계열임.
                        tempR = *(Data + (y * stride) + (x*im.NumberOfChannels)+2) ;//BGR이여서 R채널껏만 검사한다. 이미지가 레드계열임.
                        tempG = *(Data + (y * stride) + (x*im.NumberOfChannels)+1) ;//BGR이여서 R채널껏만 검사한다. 이미지가 레드계열임.
                        tempB = *(Data + (y * stride) + (x*im.NumberOfChannels)+0) ;//BGR이여서 R채널껏만 검사한다. 이미지가 레드계열임.
                        CrntPx = tempR ;        
            
                        if(minThreshold < CrntPx)
                        {
                            dSum += CrntPx ;
                            iPxCnt++;
                        }
                    }
                }
            }
            if(iPxCnt > 0) dAvr = dSum / (double)iPxCnt;
            else           dAvr = 0 ;
            retThreshold = dAvr ;

            return true ;
        }

        private int iPreAvr = 0;
        private bool DetectBlobs(Mat im, int _minA, int _maxA, bool bShow = false)
        {
            //lock(lockObject)
            //{ 
            if(im == null) return false;
            int minA = _minA; // Minimum area in pixels
            int maxA = _maxA; // Maximum area in pixels
            if (minA < 1) minA = 1;

            int iSum = 0;
            int iCnt = 0;
            //iAvr = 0;
            int iAvrTemp = 0;
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

            if(iCnt > 0) {
                    iAvrTemp = iSum / iCnt;
                    iAvr     = iAvrTemp;
            }
            if(iPreAvr < iAvrTemp)
            {
                if(iAvrTemp > maxA) return true;
            }
            iPreAvr = iAvrTemp;
            
            return false;
            //}
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

        private bool CheckArkDown(Mat im, int _minA, int _maxA, bool bShow = false)
        {
            //lock(lockObject)
            //{ 
            if(im == null) return false;
            int minA = _minA; // Minimum area in pixels
            int maxA = _maxA; // Maximum area in pixels
            if (minA < 1) minA = 1;

            int iSum = 0;
            int iCnt = 0;
            //iAvr = 0;
            int iAvrTemp = 0;
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

            if(iCnt > 0) {
                    iAvrTemp = iSum / iCnt;
                    iAvr     = iAvrTemp;
            }
            if(iPreAvr < iAvrTemp)
            {
                if(iAvrTemp > maxA) return true;
            }
            iPreAvr = iAvrTemp;
            
            return false;
            //}
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

            if(matBgra == null) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bmp File|*.bmp";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;
            //ibCam.Image.Save(saveFileDialog1.FileName);
            matBgra.Save(saveFileDialog1.FileName);
        }

        private void btRst_Click(object sender, EventArgs e)
        {
            
        }
        
        private bool bFirst = true;
        private void sdkCamera_OnDeviceStatusChanged(object sender, AxXNSSDKDEVICELib._DXnsSdkDeviceEvents_OnDeviceStatusChangedEvent e)
        {
            try
            {
                if((e.nErrorCode == (int)XErrorCode.ERR_SUCCESS) && e.nDeviceStatus == 1 && !bFirst)
                {
                    Log.Trace("Error sdkCamera_OnDeviceStatusChanged");
                    if (sdkCamera.GetDeviceStatus(hDevice) == 1)
                    {
                        Log.Trace("Error sdkCamera_OnDeviceStatusChanged");
                        //sdkCamera.OpenMedia(hDevice, nControlID, ((int)XMediaType.XMEDIA_LIVE), 0, 0, ref hMediaSource);
                        hMediaSource = NULL;
                        Play();
                    }
                }
                bFirst = false;
            }
            catch
            {
                Log.Trace("Error sdkCamera_OnDeviceStatusChanged");
            }
        }

        private void btOri_Click(object sender, EventArgs e)
        {
            //bthreshold = false;
            ibCam.Invalidate();
        }

        private void btThr_Click(object sender, EventArgs e)
        {
            //bthreshold = true;
            ibCam.Invalidate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            OM.CmnOptn.iThreshold = (int)numericUpDown1.Value;
        }

        private void btInsp_Click(object sender, EventArgs e)
        {
            double dAvr = 0 ;
            if(CheckThreshold(matBgra , Tracker.GetRectangle() , OM.CmnOptn.iMinA , ref dAvr)) iAvr = (int)dAvr ;
            ibCam.Invalidate();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            OM.CmnOptn.iMinA = (int)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            OM.CmnOptn.iCheckGap = (int)numericUpDown3.Value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            lbCount .Text     = OM.CmnOptn.iDetectCount.ToString() ;
            lbArcCnt.Text     = iArkCnt    .ToString() ;

            //pnWidth  = ibCam.Width ;
            //pnHeight = ibCam.Height;

            //lbGrabCount.Text  = iGrabCount .ToString();
            //lbPaintCount.Text = iPaintCount.ToString();

            //녹화
            try
            {
                lock (lockVideo) { 
                    //if(!sw.IsRunning) sw.Start();
                    //if(sw.ElapsedMilliseconds >= 33)
                    //{
                    //    sw.Restart();
                        if(videoWriter != null && videoWriter.IsOpened)
                        {
                            Mat matBgr = new Mat();
                            CvInvoke.CvtColor(matBgra , matBgr ,Emgu.CV.CvEnum.ColorConversion.Bgra2Bgr);            
                            videoWriter.Write(matBgr);
                        }
                    //}
                }
            }
            catch
            {
                Log.Trace("Error videoWriter");
            }


            ////녹화
            //if(videoWriter != null && videoWriter.IsOpened)
            //{
            //    Mat matBgr = new Mat();
            //    CvInvoke.CvtColor(matBgra , matBgr ,Emgu.CV.CvEnum.ColorConversion.Bgra2Bgr);            
            //    videoWriter.Write(matBgr);
            //}

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

            matBgra = new Mat(Openfile.FileName);
            
            //CvInvoke.CvtColor(matBgr, matBgr, ColorConversion.Bgr2Gray);        


            //CvInvoke.Threshold(imLoad,imLoad,OM.CmnOptn.iThreshold,255,Emgu.CV.CvEnum.ThresholdType.Binary);
            
            //DetectBlobs(matBgr,OM.CmnOptn.iMinA,OM.CmnOptn.iMaxA,true);
            ibCam.Invalidate();

        }

        private void btSetting_Click(object sender, EventArgs e)
        {
            pnTitle1.Visible = !pnTitle1.Visible;
            Tracker.enabled  = pnTitle1.Visible ;
            Tracker.selected = pnTitle1.Visible ;
            ibCam.Invalidate();

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
            //pnWidth  = ibCam.Width ;
            //pnHeight = ibCam.Height;
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

        //private int iPaintCount = 0;
        private void ibCam_Paint(object sender, PaintEventArgs e)
        {
            lock(lockObject)
            { 
            //CvInvoke.Resize(mat,Display,new Size(pnWidth,pnHeight));
            //ibCam.Image = Display  ;
            if (matBgra == null) return ;

            //원본이미지와 페널이미지의 비율.
            TScaleOffset scaleOffset ;
            scaleOffset.fOffsetX = 0 ;
            scaleOffset.fOffsetY = 0 ;
            //scaleOffset.fScaleX  = pnHeight / (float)matBgra.Height ;
            //scaleOffset.fScaleY  = pnWidth  / (float)matBgra.Width  ;
            scaleOffset.fScaleX  = pnWidth  / (float)matBgra.Width  ;
            scaleOffset.fScaleY  = pnHeight / (float)matBgra.Height ;

            //일단 이미지를 그리고.
            Graphics g = e.Graphics ;
            PaintMat(matBgra , g , scaleOffset);

            //트레커 그림.
            //Tracker.GetRectangle
            Tracker.Paint(g,scaleOffset);
            const int iFontSize = 10;

            float fLeft   = GetScaleOffsetValX(Tracker.GetRectangle().Left , scaleOffset)  ;
            float fTop    = GetScaleOffsetValY(Tracker.GetRectangle().Top  , scaleOffset)  ;

            using (Pen pen = new Pen(Color.Yellow, 1))
            using (SolidBrush brush = new SolidBrush(Color.Yellow))
            using (Font font = new Font("Arial", iFontSize))
            {
                g.DrawString(iAvr.ToString(), font, brush,fLeft ,fTop  - 15);
            }

            //iPaintCount++;
            //if(iPaintCount > 100000) iPaintCount = 0;

                //TScaleOffset ScaleOffset = GV.Visions[iVisionID].GetPkgActivate().ScaleOffset ;
                //
                //Graphics g = e.Graphics ;
                //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                //g.PixelOffsetMode   = PixelOffsetMode.Half ;
                //
                //lock(GV.ImageLock[iVisionID])
                //{
                //    GV.Visions[iVisionID].GetPkgActivate().PaintTrain(g);
                //    if (ScaleOffset.fScaleX > 29 && ScaleOffset.fScaleY > 29)
                //    {
                //        ViewerDisplayPixel(g);
                //    }
                //}
            }
        }

        private void ibCam_MouseDown(object sender, MouseEventArgs e)
        {
            if (matBgra == null) return ;

            TScaleOffset scaleOffset ;
            scaleOffset.fOffsetX = 0 ;
            scaleOffset.fOffsetY = 0 ;
            //scaleOffset.fScaleX  = pnHeight / (float)matBgra.Height ;
            //scaleOffset.fScaleY  = pnWidth  / (float)matBgra.Width  ;
            scaleOffset.fScaleX  = pnWidth  / (float)matBgra.Width  ;
            scaleOffset.fScaleY  = pnHeight / (float)matBgra.Height ;
            if(Tracker.MouseDown(Control.ModifierKeys , e,scaleOffset))
            {
                ibCam.Invalidate();
            }  
        }

        private void ibCam_MouseMove(object sender, MouseEventArgs e)
        {
            if (matBgra == null) return ;

            TScaleOffset scaleOffset ;
            scaleOffset.fOffsetX = 0 ;
            scaleOffset.fOffsetY = 0 ;
            //scaleOffset.fScaleX  = pnHeight / (float)matBgra.Height ;
            //scaleOffset.fScaleY  = pnWidth  / (float)matBgra.Width  ;
            scaleOffset.fScaleX  = pnWidth  / (float)matBgra.Width  ;
            scaleOffset.fScaleY  = pnHeight / (float)matBgra.Height ;

            if(Tracker.MouseMove(Control.ModifierKeys , e,scaleOffset))
            {
                ibCam.Invalidate();
            }     
        }

        private void ibCam_MouseUp(object sender, MouseEventArgs e)
        {
            if (matBgra == null) return ;

            if(Tracker.MouseUp(Control.ModifierKeys , e))
            {
                ibCam.Invalidate();
            }  
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            OM.SaveCmnOptn();
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath      = sExeFolder + "Util\\Tracker.ini";
            Tracker.LoadSave(false , sPath);
        }

        private void btInsp_Click_1(object sender, EventArgs e)
        {


        }

        private void ibCam_SizeChanged(object sender, EventArgs e)
        {
            pnWidth  = ibCam.Width ;
            pnHeight = ibCam.Height;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}

