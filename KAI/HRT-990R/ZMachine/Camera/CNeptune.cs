using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NeptuneC_Interface;
using COMMON;

namespace VDll.Camera
{
    //////////////////////////////
    //IMI Neptune Ver4109 64bit //
    /// //////////////////////////



    public class CNeptune : CErrorObject, ICamera
    {
        [Serializable, TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUPara
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Brightness")] public uint Brightness { get; set; }
            [CategoryAttribute("UPara"), DisplayNameAttribute("Sharpness")] public uint Sharpness { get; set; }
            [CategoryAttribute("UPara"), DisplayNameAttribute("Gamma")] public uint Gamma { get; set; }
            [CategoryAttribute("UPara"), DisplayNameAttribute("Shutter")] public uint Shutter { get; set; }
            [CategoryAttribute("UPara"), DisplayNameAttribute("Gain")] public uint Gain { get; set; }
        }
        [Serializable, TypeConverter(typeof(ExpandableObjectConverter))]
        public class CPara
        {
            [CategoryAttribute("Para"), DisplayNameAttribute("PhysicalAdd")] public uint PhysicalAdd { get; set; }

            //이건그냥 넵튠프로그램에서 하자.
            //    [CategoryAttribute("Para" ), DisplayNameAttribute("VideoFormat" )]public int VideoFormat{set{if(value < 0)value=0 ; if(value>6 )value=6 ;}} 
            //    [CategoryAttribute("Para" ), DisplayNameAttribute("VideoMode"   )]public int VideoMode  {set{if(value < 5)value=5 ; if(value>12)value=12;}} 
            //    [CategoryAttribute("Para" ), DisplayNameAttribute("FrameRate"   )]public int FrameRate  {set{if(value < 1)value=1 ; if(value>8 )value=8 ;}} 
        }
        //Static.
        //======================================
        static uint iNeptCnt = 0; //현재 메모리상 넵튠 카메라 갯수.
        static uint iTotalCnt = 0; //넵튠에서 받아온 카메라 갯수.
        static NEPTUNE_CAM_INFO[] NeptCamInfoList;
        //=======================================

        IntPtr m_hCamHandle = IntPtr.Zero;
        NeptuneC_Interface.NeptuneCFrameCallback NeptuneCFrameCallbackInstance;
        CPara Para = new CPara(); //UI에서 보이지 않는 파라임 
        CUPara UParaSet = new CUPara(); //최종으로 세팅된 유저파라 기억하고 있어야 함. 세이브 필요 없음.
        GrabCallback CallBack = null;
        Stopwatch GrabTime = new Stopwatch();
        ENeptuneError eRet;
        ENeptunePixelFormat ePxFormat;

        public CNeptune(string _sName) : base(_sName)
        {
        }

        public bool Init()
        {
            //eRet = NeptuneC.ntcUninit();


            eRet = NeptuneC.ntcInit();
            if (eRet != ENeptuneError.NEPTUNE_ERR_Success) {
                sError = "API Init Failed";
                return false;
            }

            if (iNeptCnt == 0) {
                eRet = NeptuneC.ntcGetCameraCount(ref iTotalCnt);
                if (eRet != ENeptuneError.NEPTUNE_ERR_Success) {
                    sError = "GetCameraCount Failed";
                    return false;
                }
                if (iTotalCnt > 0) {
                    NeptCamInfoList = new NEPTUNE_CAM_INFO[iTotalCnt];
                    IntPtr pCamInfo = NeptuneC.MarshalArrtoIntPtr<NEPTUNE_CAM_INFO>(NeptCamInfoList);
                    NeptCamInfoList = new NEPTUNE_CAM_INFO[iTotalCnt];
                    eRet = NeptuneC.ntcGetCameraInfo(pCamInfo, iTotalCnt);
                    if (eRet == ENeptuneError.NEPTUNE_ERR_Success) {
                        NeptuneC.UnmarshalIntPtrToArr<NEPTUNE_CAM_INFO>(pCamInfo, ref NeptCamInfoList);
                    }
                    Marshal.FreeHGlobal(pCamInfo);
                }
            }
            if (iNeptCnt >= iTotalCnt) {
                sError = "Camera Count Over(Cam Count from Api:" + iTotalCnt + ")";
                return false;
            }

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sFilePath = sExeFolder + "VisnUtil\\CamParaNeptune" + iNeptCnt.ToString() + ".xml";
            LoadSavePara(true, sFilePath);

            eRet = NeptuneC.ntcOpen(NeptCamInfoList[Para.PhysicalAdd].strCamID, ref m_hCamHandle, ENeptuneDevAccess.NEPTUNE_DEV_ACCESS_EXCLUSIVE);
            if (eRet != ENeptuneError.NEPTUNE_ERR_Success)
            {
                sError = "Camera Open Failed!-code(" + eRet.ToString() + ")";
                return false;
            }

            //로딩시에 유저셑 체널 5번을 로딩한다.
            //F2F1011C 레지스터 500000 으로 세팅 하고 전원오프시에 5번채널을 로딩하라는 설정임.
            //세팅 변경시에 Userset 5번 채널 로딩 해서 바꾸고 해야 함.
            NEPTUNE_USERSET UserSet ;
            UserSet.SupportUserSet = 0 ;
            UserSet.UserSetIndex   = ENeptuneUserSet.NEPTUNE_USERSET_5 ;
            UserSet.Command        = ENeptuneUserSetCommand.NEPTUNE_USERSET_CMD_LOAD ;
            eRet = NeptuneC.ntcSetUserSet(m_hCamHandle, UserSet );
            if (eRet != ENeptuneError.NEPTUNE_ERR_Success)
            {
                sError = "SetUserSet Failed!-code(" + eRet.ToString() + ")";
                return false;
            }



            //Callback 등록.
            NeptuneCFrameCallbackInstance = new NeptuneCFrameCallback(FrameCallback);
            if (NeptuneC.ntcSetFrameCallback(m_hCamHandle, NeptuneCFrameCallbackInstance, IntPtr.Zero) != ENeptuneError.NEPTUNE_ERR_Success)
            {
                sError = "Camera Callback Setting Failed!";
                return false;
            }

            eRet = NeptuneC.ntcSetAcquisition(m_hCamHandle, ENeptuneBoolean.NEPTUNE_BOOL_TRUE);
            if (eRet != ENeptuneError.NEPTUNE_ERR_Success)
            {
                sError = "SetAcquisition Failed!-code(" + eRet.ToString() + ")";
                return false;
            }




            /*유저 버퍼세팅 나중에 해보자.
             * 이미지 큰 카메라 일땐 이걸 쓰는게 좋음.
             _uint32_t nSize;
             _int8_t* pBuffer = NULL;
             ntcSetPixelFormat(hCamHandle, Mono8);
             ntcGetBufferSize(hCamHandle, &nSize);
             pBuffer = new _int8_t[nSize]; 
             ntcSetUserBuffer(hCamHandle, pBuffer, nSize, 5);
             ntcSetAcquisition(hCamHandle, NEPTUNE_BOOL_TRUE); 
             */

            NEPTUNE_TRIGGER Trigger;
            Trigger.Mode = ENeptuneTriggerMode.NEPTUNE_TRIGGER_MODE_0;
            Trigger.Polarity = ENeptunePolarity.NEPTUNE_POLARITY_RISINGEDGE;
            Trigger.OnOff = ENeptuneBoolean.NEPTUNE_BOOL_TRUE;//NEPTUNE_BOOL_TRUE
            Trigger.Source = ENeptuneTriggerSource.NEPTUNE_TRIGGER_SOURCE_SW;
            Trigger.nParam = 1;
            if (NeptuneC.ntcSetTrigger(m_hCamHandle, Trigger) != ENeptuneError.NEPTUNE_ERR_Success)
            {
                sError = "Camera Callback Setting Failed!";
                return false;
            }


            if (NeptuneC.ntcGetPixelFormat(m_hCamHandle, ref ePxFormat) != ENeptuneError.NEPTUNE_ERR_Success)
            {
                sError = "Camera PixelFormat Getting Failed!";
                return false;
            }

            iNeptCnt++;
            return true;
        }

        public bool Close()
        {
            iNeptCnt--;

            if (m_hCamHandle != IntPtr.Zero)
            {
                NeptuneC.ntcClose(m_hCamHandle);
                m_hCamHandle = IntPtr.Zero;
            }

            if (iNeptCnt == 0) NeptuneC.ntcUninit();

            return true;
        }


        private bool FrameCallback(ref NEPTUNE_IMAGE _pImage, IntPtr _pContext)
        {
            GrabTime.Stop();
            //외부 콜백함수 등록 안됌.
            if (CallBack == null) return true;

            const bool bUseNeptuneConv = false;
            if (ePxFormat == ENeptunePixelFormat.YUV422Packed)
            {

                if (bUseNeptuneConv)
                {
                    //이렇게 쓰면 메모리 누수되어서 안됌. 재활용 방식으로 해야함.
                    //UInt32 nRGBSize = _pImage.uiWidth * _pImage.uiHeight * 3;
                    //Byte[] RGBArr = new Byte[nRGBSize];
                    //IntPtr pRGBBuf = Marshal.AllocHGlobal(Marshal.SizeOf(RGBArr[0]) * (int)nRGBSize);
                    //NeptuneC.ntcGetRGBData(m_hCamHandle, pRGBBuf, nRGBSize);
                    //
                    //CallBack((int)_pImage.uiWidth , (int)_pImage.uiHeight , 24 , EPixelFormat.Bgr888 , pRGBBuf);
                }
                else
                {
                    CallBack((int)_pImage.uiWidth, (int)_pImage.uiHeight, (int)_pImage.uiBitDepth, EPixelFormat.Yuv422, _pImage.pData);
                }
            }
            else
            {
                CallBack((int)_pImage.uiWidth, (int)_pImage.uiHeight, (int)_pImage.uiBitDepth, EPixelFormat.Gray, _pImage.pData);
            }



            return true;
        }

        //public object GetUserPara()
        //{
        //    return UserPara ;
        //}

        public bool Grab()
        {
            GrabTime.Restart();
            if (m_hCamHandle == IntPtr.Zero)
            {
                sError = "Camera handle is null";
                GrabTime.Stop();
                return false;
            }

            if (NeptuneC.ntcRunSWTrigger(m_hCamHandle) != ENeptuneError.NEPTUNE_ERR_Success) {
                sError = "API Grab Function Failed!";
                return false;
            }
            return true;
        }

        public void SetGrabFunc(GrabCallback _pFunc)
        {
            CallBack = _pFunc;
        }

        public double GetGrabTime()
        {
            return GrabTime.ElapsedMilliseconds;
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

            NEPTUNE_FEATURE FeatureInfo = new NEPTUNE_FEATURE();

            //이거 다수행 하면 344ms 정도 걸림....
            //그랩 마다 세팅 하고 싶은데 힘들것 같음.
            bool bRet = true;
            if (UParaSet.Brightness != UserPara.Brightness) {
                NeptuneC.ntcGetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_BLACKLEVEL, ref FeatureInfo);
                FeatureInfo.Value = (int)UserPara.Brightness;
                if (NeptuneC.ntcSetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_BLACKLEVEL, FeatureInfo) != ENeptuneError.NEPTUNE_ERR_Success) {
                    sError = "Failed Set Brightness " + UserPara.Brightness.ToString();
                    bRet = false;
                }
                else UParaSet.Brightness = UserPara.Brightness;
            }

            if (UParaSet.Sharpness != UserPara.Sharpness) {
                NeptuneC.ntcGetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_SHARPNESS, ref FeatureInfo);
                FeatureInfo.Value = (int)UserPara.Sharpness;
                ENeptuneError Error = NeptuneC.ntcSetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_SHARPNESS, FeatureInfo);
                if (Error != ENeptuneError.NEPTUNE_ERR_Success) {
                    sError = "Failed Set Sharpness " + UserPara.Sharpness.ToString();
                    bRet = false;
                }
                else UParaSet.Sharpness = UserPara.Sharpness;
            }

            if (UParaSet.Gamma != UserPara.Gamma) {
                NeptuneC.ntcGetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_GAMMA, ref FeatureInfo);
                FeatureInfo.Value = (int)UserPara.Gamma;
                if (NeptuneC.ntcSetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_GAMMA, FeatureInfo) != ENeptuneError.NEPTUNE_ERR_Success) {
                    sError = "Failed Set Gamma " + UserPara.Gamma.ToString();
                    bRet = false;
                }
                else UParaSet.Gamma = UserPara.Gamma;
            }

            if (UParaSet.Shutter != UserPara.Shutter) {
                NeptuneC.ntcGetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_SHUTTER, ref FeatureInfo);
                FeatureInfo.Value = (int)UserPara.Shutter;
                if (NeptuneC.ntcSetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_SHUTTER, FeatureInfo) != ENeptuneError.NEPTUNE_ERR_Success) {
                    sError = "Failed Set Shutter_" + UserPara.Shutter.ToString();
                    bRet = false;
                }
                else UParaSet.Shutter = UserPara.Shutter;
            }

            if (UParaSet.Gain != UserPara.Gain) {
                NeptuneC.ntcGetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_GAIN, ref FeatureInfo);
                FeatureInfo.Value = (int)UserPara.Gain;
                if (NeptuneC.ntcSetFeature(m_hCamHandle, ENeptuneFeature.NEPTUNE_FEATURE_GAIN, FeatureInfo) != ENeptuneError.NEPTUNE_ERR_Success) {
                    sError = "Failed Set Gain_" + UserPara.Gain.ToString();
                    bRet = false;
                }
                else UParaSet.Gain = UserPara.Gain;
            }
            return bRet;
        }

        public bool SetModeHwTrigger(bool _bOn)
        {
            NEPTUNE_TRIGGER Trigger = new NEPTUNE_TRIGGER();

            if (NeptuneC.ntcGetTrigger(m_hCamHandle, ref Trigger) != ENeptuneError.NEPTUNE_ERR_Success)
            {
                sError = "Camera Get Trigger Info Failed!";
                return false;
            }

            if (_bOn) Trigger.Source = ENeptuneTriggerSource.NEPTUNE_TRIGGER_SOURCE_LINE1;
            else      Trigger.Source = ENeptuneTriggerSource.NEPTUNE_TRIGGER_SOURCE_SW   ;

            if (NeptuneC.ntcSetTrigger(m_hCamHandle, Trigger) != ENeptuneError.NEPTUNE_ERR_Success)
            {
                sError = "Camera Set Trigger Info Failed!";
                return false;
            }

            return true;
        }

        public bool ShowSettingDialog()
        {
            if (m_hCamHandle != IntPtr.Zero)
                // show camera control dialog
                NeptuneC.ntcShowControlDialog(m_hCamHandle);

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
