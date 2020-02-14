using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using COMMON;
using SpinnakerNET;
using SpinnakerNET.GenApi;

namespace Machine
{
    class ImageEventListener : ManagedImageEvent
    {
        private string deviceSerialNumber;
        GrabCallback CallBack = null;
        

        // The constructor retrieves the serial number and initializes the
        // image counter to 0.
        public ImageEventListener(IManagedCamera cam)
        {
            // Retrieve device serial number
            INodeMap nodeMap = cam.GetTLDeviceNodeMap();

            deviceSerialNumber = "";

            IString iDeviceSerialNumber = nodeMap.GetNode<IString>("DeviceSerialNumber");
            if (iDeviceSerialNumber != null && iDeviceSerialNumber.IsReadable)
            {
                deviceSerialNumber = iDeviceSerialNumber.Value;
            }

            Timer.Start();
        }

        // This method defines an image event. In it, the image that
        // triggered the event is converted and saved before incrementing
        // the count. Please see Acquisition_CSharp example for more
        // in-depth comments on the acquisition of images.
        
        public double    dFrame    = 0.0 ;
        Stopwatch Timer = Stopwatch.StartNew();
        public int iFrameCnt = 0 ;
        //IManagedImage convertedImage ;
        override protected void OnImageEvent(ManagedImage _miImg)
        {
            iFrameCnt++;

            
            //1초에 한번씩 갱신.
            //double Time = Timer.ElapsedTicks / (double)Stopwatch.Frequency ;
            //if(Time > 1)
            //{
            //    Timer.Restart();
            //    dFrame = 1/(Time /iFrameCnt);
            //    iFrameCnt=0;
            //    
            //}

            //_miImg.

            double Time = Timer.ElapsedMilliseconds ;
            if(Time > 2000)
            {
                Timer.Restart();
                dFrame = 1000/(Time /iFrameCnt);
                iFrameCnt=0;
                
            }

            
            if (_miImg.IsIncomplete)
            {
                int i =0 ;
                i++;
                //Console.WriteLine("Image incomplete with image status {0}...\n", _miImg.ImageStatus);
            }
            else
            {
                if(CallBack != null)
                { 
                    // Convert image
                    if(_miImg.PixelFormat == PixelFormatEnums.BayerRG8)
                    {//4665600 , 1555200 1440*1080

                        CallBack((int)_miImg.Width, (int)_miImg.Height , 8 , EPixelFormat.BayerRG8, _miImg.DataPtr);           
                    }
                    else //if(_miImg.PixelFormat == PixelFormatEnums.Mono8)
                    {
                        CallBack((int)_miImg.Width, (int)_miImg.Height , 8, EPixelFormat.Gray, _miImg.DataPtr);
                    }
                }
            }           

            // Must manually release the image to prevent buffers on the camera stream from filling up
            _miImg.Release();

            return ;

        }

        public void SetCallback(GrabCallback _fpCallback)
        {
            CallBack = _fpCallback ;
        }
    }

    public class CFlir : CErrorObject, ICamera
    {
        [Serializable, TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUPara
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Exposure"  )] public uint   Exposure  { get; set; }
            [CategoryAttribute("UPara"), DisplayNameAttribute("Gain"      )] public double Gain      { get; set; }
            [CategoryAttribute("UPara"), DisplayNameAttribute("Gamma"     )] public double Gamma     { get; set; }
            [CategoryAttribute("UPara"), DisplayNameAttribute("BlackLevel")] public double BlackLevel{ get; set; }
        }
        [Serializable, TypeConverter(typeof(ExpandableObjectConverter))]
        public class CPara
        {
            [CategoryAttribute("Para"), DisplayNameAttribute("PhysicalAdd"  )] public uint PhysicalAdd   { get; set; }
            [CategoryAttribute("Para"), DisplayNameAttribute("HWTrigger"    )] public bool HWTrigger     { get; set; }
        }
        //Static.
        //======================================
        static uint iFlirCnt = 0; //현재 메모리상 카메라 갯수.
        static uint iTotalCnt = 0; //API에서 받아온 FLIR 카메라 갯수.
        //=======================================
        CPara        Para         = new CPara(); //UI에서 보이지 않는 파라임 
        CUPara       UParaSet     = new CUPara(); //최종으로 세팅된 유저파라 기억하고 있어야 함. 세이브 필요 없음.
        //GrabCallback CallBack     = null;
        Stopwatch    GrabTime     = new Stopwatch();

        static ManagedSystem System = new ManagedSystem();
        static IList<IManagedCamera> lsCamaras ;

        IManagedCamera Camera ;
        ImageEventListener EventListener ;
        INodeMap NodeMap ;

        public CFlir(string _sName) : base(_sName)
        {

        }

        public bool Init()
        {
            bool bRet = true ;
            if (iFlirCnt == 0)
            {
                LibraryVersion spinVersion = System.GetLibraryVersion();
                lsCamaras = System.GetCameras();
                iTotalCnt = (uint)lsCamaras.Count ;
                if (iTotalCnt == 0)
                {
                    sError = "Camera Count Over(Cam Count from Api:" + 0 + ")";
                    bRet = false ;
                }
            }

            if(iTotalCnt <= Para.PhysicalAdd || Para.PhysicalAdd < 0 )
            {
                sError = "Invalid PhysicalAdd(Cam Count from Api:" + iTotalCnt + ")";
                bRet = false ;
            }
            

            Camera = lsCamaras[(int)Para.PhysicalAdd];

            try
            {
                // Retrieve TL device nodemap and print device information
                //INodeMap nodeMapTLDevice = Camera.GetTLDeviceNodeMap();
                //result = PrintDeviceInfo(nodeMapTLDevice);

                // Initialize camera
                NodeMap = Camera.Init();
                

                //Camera.();

                // Retrieve GenICam nodemap
                NodeMap = Camera.GetNodeMap();

                EventListener = new ImageEventListener(Camera);      
                Camera.RegisterEvent(EventListener);

                //이렇게 해야 강제종료한상태에서 넘어가짐.
                Camera.BeginAcquisition();
                Camera.EndAcquisition();

                //User set 0 Loading.
                #region LoadUserSet0
                IEnum iUserSetSelector = NodeMap.GetNode<IEnum>("UserSetSelector");
                if (iUserSetSelector == null || !iUserSetSelector.IsWritable)
                {
                    sError = "Unable to Select UserSetSelector";
                    bRet = false;
                }
                

                //Load UserSet.
                ICommand iUserSetLoad = NodeMap.GetNode<ICommand>("UserSetLoad");
                if (iUserSetLoad == null || !iUserSetLoad.IsWritable)
                {
                    sError = "Unable to Excute UserSetLoad";
                    bRet = false;
                }                
                iUserSetLoad.Execute();
                #endregion

                // Set acquisition mode to continuous
                #region SetAcquisitionModeToContinuous
                IEnum iAcquisitionMode = NodeMap.GetNode<IEnum>("AcquisitionMode");
                if (iAcquisitionMode == null || !iAcquisitionMode.IsWritable)
                {
                    sError = "Unable to set acquisition mode to continuous (node retrieval). Aborting...";
                    bRet = false;
                }
                
                IEnumEntry iAcquisitionModeContinuous = iAcquisitionMode.GetEntryByName("Continuous");
                if (iAcquisitionModeContinuous == null || !iAcquisitionMode.IsReadable)
                {
                    sError = "Unable to set acquisition mode to continuous (enum entry retrieval). Aborting...";
                    bRet = false;
                }                
                iAcquisitionMode.Value = iAcquisitionModeContinuous.Symbolic;
                #endregion

                //Buffer Stream Count Mode   StreamBufferCountMode	Auto
                #region iStreamBufferCountMode
                INodeMap StreamNodeMap = Camera.GetTLStreamNodeMap();
                IEnum iStreamBufferHandlingMode = StreamNodeMap.GetNode<IEnum>("StreamBufferHandlingMode");
                if (iStreamBufferHandlingMode == null || !iStreamBufferHandlingMode.IsWritable)
                {
                    sError = "Unable to set StreamBufferHandlingMode to Auto (node retrieval). Aborting...";
                    bRet = false;
                }
                
                IEnumEntry iNewestOnly = iStreamBufferHandlingMode.GetEntryByName("NewestOnly");
                //IEnumEntry iNewestOnly = iStreamBufferHandlingMode.GetEntryByName("NewestFirst");
                if (iNewestOnly == null || !iNewestOnly.IsReadable)
                {
                    sError = "Unable to set StreamBufferCountMode to Auto (enum entry retrieval). Aborting...";
                    bRet = false;
                }                
                iStreamBufferHandlingMode.Value = iNewestOnly.Symbolic;
                #endregion

                //나중에 이더넷카메라 사용시 깜박거리면 버퍼사이즈 메뉴얼모드에서 50으로 바꿔줘야함.
                #region SetBufferSize
                //if (!CameraFunctionCheck(spinCameraGetTLStreamNodeMap(pAS->m_hCamera, &pAS->m_hTLStreamNode), "Unable to retrieve GenICam TLStream nodemap. Aborting with error"))
                //    return FALSE;
                //
                //
                //int64Value = iBufferCount + 5;
                //if (!CameraFunctionCheck(spinNodeMapGetNode(pAS->m_hTLStreamNode, "StreamDefaultBufferCountMax", &hCaptionNode), "Cameara param get fail ==> StreamDefaultBufferCountMax Node"))
                //    return FALSE;
                //if (!CameraFunctionCheck(spinIntegerSetValue(hCaptionNode, int64Value), "Cameara param set fail ==> StreamDefaultBufferCountMax Value"))
                //    return FALSE;
                //
                //int64Value = iBufferCount;
                //if (!CameraFunctionCheck(spinNodeMapGetNode(pAS->m_hTLStreamNode, "StreamDefaultBufferCount", &hCaptionNode), "Cameara param get fail ==> StreamDefaultBufferCount Node"))
                //    return FALSE;
                //if (!CameraFunctionCheck(spinIntegerSetValue(hCaptionNode, int64Value), "Cameara param set fail ==> StreamDefaultBufferCount Value"))
                //    return FALSE;
                #endregion

                // Begin acquiring images
                Camera.BeginAcquisition();

                //Para.HWTrigger = true ;
                if (!SetModeHwTrigger(true)) { bRet = false ;}                
                
            }
            catch (SpinnakerException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                sError = ex.Message;
                bRet = false ;
            }

                  

            
            
            iFlirCnt++;
            return bRet;
        }
        
        public bool Close()
        {
            iFlirCnt--;

            // End acquisition
            Camera.EndAcquisition();
                          
            // Deinitialize camera
            Camera.DeInit();

            if (iFlirCnt == 0) {
                System.Dispose();
                lsCamaras.Clear();
            }

            return true;
        }

        public bool Grab()
        {
            GrabTime.Restart();
            
            try{
                // Execute software trigger
                ICommand iTriggerSoftware = NodeMap.GetNode<ICommand>("TriggerSoftware");
                if (iTriggerSoftware == null || !iTriggerSoftware.IsWritable)
                {
                    sError = "Trigger Failed!";
                    return false;
                }
                
                iTriggerSoftware.Execute();
            }
            catch(SpinnakerException _e)
            {
                sError = _e.Message ;
                return false ;
            }

            return true;
        }

        public void SetGrabFunc(GrabCallback _pFunc)
        {
            if (EventListener==null)return ;
            EventListener.SetCallback(_pFunc);
        }

        public double GetGrabTime()
        {
            return GrabTime.ElapsedMilliseconds;
        }

        public double GetFrameFrq()
        {
            if(EventListener == null) return 0.0;
            return EventListener.dFrame ;
        }

        public bool LoadSavePara(bool _bLoad, string _sParaFilePath)
        {
            string sFilePath = _sParaFilePath; // + "NeptunePara.xml";
            if (_bLoad)
            {
                if (!CXml.LoadXml(sFilePath, ref Para)) { return false; }
            }
            else
            {
                if (!CXml.SaveXml(sFilePath, ref Para)) { return false; }
            }

            return true;
        }

        public bool SetExposure(uint _uiValue)
        {
            try
            {
                

                IFloat iExposureTime = NodeMap.GetNode<IFloat>("ExposureTime");
                if (iExposureTime == null || !iExposureTime.IsWritable)
                {
                    sError = "Unable to set exposure time. Aborting...";
                    return false;
                }

                // Ensure desired exposure time does not exceed the maximum
                double dValue ;
                dValue = (_uiValue > iExposureTime.Max ? iExposureTime.Max : _uiValue);
                dValue = (dValue   < iExposureTime.Min ? iExposureTime.Min :   dValue);

                iExposureTime.Value = dValue ; 

                //Console.WriteLine("Exposure time set to {0} us...\n", iExposureTime.Value);
            }
            catch (SpinnakerException ex)
            {
                sError = ex.Message;
                return false ;
            }

            return true;
        }

        bool SetGain(double _dValue)
        {
            try
            {
                IFloat iGain = NodeMap.GetNode<IFloat>("Gain");
                if (iGain == null || !iGain.IsWritable)
                {
                    sError = "Unable to set Gain. Aborting...";
                    return false;
                }

                // Ensure desired exposure time does not exceed the maximum
                double dValue = _dValue ;
                dValue = (dValue  > iGain.Max ? iGain.Max : dValue );
                dValue = (dValue  < iGain.Min ? iGain.Min : dValue );

                iGain.Value = dValue ;

                //Console.WriteLine("Exposure time set to {0} us...\n", iExposureTime.Value);
            }
            catch (SpinnakerException ex)
            {
                sError = ex.Message;
                return false;
            }

            return true;
        }
        
        bool SetGamma(double _dValue)
        {
            try
            {
                IFloat iGamma = NodeMap.GetNode<IFloat>("Gamma");
                if (iGamma == null || !iGamma.IsWritable)
                {
                    sError = "Unable to set Gamma. Aborting...";
                    return false;
                }

                double dValue = _dValue;
                dValue = (dValue > iGamma.Max ? iGamma.Max : dValue);
                dValue = (dValue < iGamma.Min ? iGamma.Min : dValue);

                iGamma.Value = dValue ;

                //Console.WriteLine("Exposure time set to {0} us...\n", iExposureTime.Value);
            }
            catch (SpinnakerException ex)
            {
                sError = ex.Message;
                return false;
            }

            return true;
        }

        bool SetBlackLevel(double _dValue)
        {
            try
            {
                IFloat iBlackLevel = NodeMap.GetNode<IFloat>("BlackLevel");
                if (iBlackLevel == null || !iBlackLevel.IsWritable)
                {
                    sError = "Unable to set Gamma. Aborting...";
                    return false;
                }

                double dValue = _dValue ;
                dValue = (dValue > iBlackLevel.Max ? iBlackLevel.Max : dValue);
                dValue = (dValue < iBlackLevel.Min ? iBlackLevel.Min : dValue);

                iBlackLevel.Value = dValue ;

                //Console.WriteLine("Exposure time set to {0} us...\n", iExposureTime.Value);
            }
            catch (SpinnakerException ex)
            {
                sError = ex.Message;
                return false;
            }

            return true;
        }

        public bool ApplyPara(object _oUPara)
        {
            CUPara UserPara;
            try
            {
                UserPara = (CUPara)_oUPara;
            }
            catch (InvalidCastException e)
            {
                sError = e.Message;
                return false;
            }

            bool bRet = true;
            if(UParaSet.Exposure != UserPara.Exposure && !SetExposure(UserPara.Exposure)) bRet = false ;
            UParaSet.Exposure = UserPara.Exposure;

            if(UParaSet.Gain != UserPara.Gain && !SetGain(UserPara.Gain)) bRet = false ;
            UParaSet.Gain = UserPara.Gain;

            if(UParaSet.Gamma != UserPara.Gamma && !SetGamma(UserPara.Gamma)) bRet = false ;
            UParaSet.Gamma = UserPara.Gamma;

            if(UParaSet.BlackLevel != UserPara.BlackLevel && !SetBlackLevel(UserPara.BlackLevel)) bRet = false ;
            UParaSet.BlackLevel = UserPara.BlackLevel;

            return bRet ; 
        }

        public bool SetModeHwTrigger(bool _bOn)
        {
            try
            {
                //Load UserSet.
                ICommand Stop = NodeMap.GetNode<ICommand>("AcquisitionStop");
                if (Stop == null || !Stop.IsWritable)
                {
                    sError = "Unable to Excute UserSetLoad";
                    return false;
                }                
                Stop.Execute();

                //
                // Ensure trigger mode off
                //
                // *** NOTES ***
                // The trigger must be disabled in order to configure whether 
                // the source is software or hardware.
                //
                IEnum iTriggerMode = NodeMap.GetNode<IEnum>("TriggerMode");
                if (iTriggerMode == null || !iTriggerMode.IsWritable)
                {
                    sError = "Unable to disable trigger mode (enum retrieval). Aborting..." ;
                    return false;
                }
                IEnumEntry iTriggerModeOff = iTriggerMode.GetEntryByName("Off");
                if (iTriggerModeOff == null || !iTriggerModeOff.IsReadable)
                {
                    sError = "Unable to disable trigger mode (entry retrieval). Aborting...";
                    return false;
                }
                //Trigger mode disabled...
                iTriggerMode.Value = iTriggerModeOff.Value;

                //
                // Select trigger source
                //
                // *** NOTES ***
                // The trigger source must be set to hardware or software while 
                // trigger mode is off.
                //
                IEnum iTriggerSource = NodeMap.GetNode<IEnum>("TriggerSource");
                if (iTriggerSource == null || !iTriggerSource.IsWritable)
                {
                    sError = "Unable to set trigger mode (enum retrieval). Aborting..." ;
                    return false;
                }

                if (_bOn)
                {
                    IEnumEntry iTriggerSourceHardware = iTriggerSource.GetEntryByName("Line0");
                    if (iTriggerSourceHardware == null || !iTriggerSourceHardware.IsReadable)
                    {                        sError = "Unable to set hardware trigger mode (entry retrieval). Aborting...";
                        return false;
                    }

                    iTriggerSource.Value = iTriggerSourceHardware.Value;
                }                
                else
                {
                    // Set trigger mode to software
                    IEnumEntry iTriggerSourceSoftware = iTriggerSource.GetEntryByName("Software");
                    if (iTriggerSourceSoftware == null || !iTriggerSourceSoftware.IsReadable)
                    {
                        sError = "Unable to set software trigger mode (entry retrieval). Aborting...";
                        return false;
                    }

                    iTriggerSource.Value = iTriggerSourceSoftware.Value;
                }

				IEnumEntry iTriggerModeOn = iTriggerMode.GetEntryByName("On");
                if (iTriggerModeOn == null || !iTriggerModeOn.IsReadable)
                {
                    sError = "Unable to enable trigger mode (entry retrieval). Aborting...";
                    return false;
                }
				iTriggerMode.Value = iTriggerModeOn.Value;

                ICommand Start = NodeMap.GetNode<ICommand>("AcquisitionStart");
                if (Start == null || !Start.IsWritable)
                {
                    sError = "Unable to Excute UserSetLoad";
                    return false;
                }                
                Start.Execute();

            }
            catch (SpinnakerException ex)
            {
                sError  = ex.Message;
                return false ;
            }

            return true;
        
        }

        public bool ShowSettingDialog()
        {
            //if (m_hCamHandle != IntPtr.Zero)
                // show camera control dialog
                //NeptuneC.ntcShowControlDialog(m_hCamHandle);

            return true ;

        }

        public string GetError()
        {
            return sError ;
        }

        public Type GetParaType()
        {
            return typeof(CUPara);
        }


    }
}
