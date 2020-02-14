using CMDLL;
using COMMON;
using MotionInterface;
using Cmmsdk = CMDLL.SafeNativeMethods;

namespace MotionComizoa
{
    public class CDio : IDio
    {
        public CDio() { }

        public struct TModuleInfo
        {
            public int   iCntIn     ;
            public int   iCntOut    ;
            public uint	 iDataIn    ;
            public uint	 iDataOutGet;
            public int   iDataOutSet;
        };

        //하드웨어상의 모듈갯수. 
        //public TModuleInfo[] m_aModuleInfo;
        public TModuleInfo m_aModuleInfo;
        int m_iInModuleCnt; //Input Module Count
        int m_iOutModuleCnt;//Output Module Count
        int m_iModuleCnt;
        
        //모듈의 접점 카운트
        int m_iMaxIn = 0;
        int m_iMaxOut = 0;



        //Display용 모듈 순서
        /// <summary>
        /// 초기화함수
        /// </summary>
        /// <returns>성공했는지 실패했는지 여부</returns>
        public bool Init()
        {
            // Load Motion & DIO devices //
            int nNumAxes = 0; 
            if (Cmmsdk.cmmGnDeviceLoad((int)MotnDefines._TCmBool.cmTRUE, ref nNumAxes) != MotnDefines.cmERR_NONE)
            {
                Log.ShowMessage("Dio", "Comizoa Device Loading Error");
                return false;
            }

            //Input 전체 채널 개수
            Cmmsdk.cmmAdvGetNumAvailDioChan((int)MotnDefines._TCmBool.cmTRUE, ref m_iMaxIn); //현재 사용 가능한 Input채널 갯수를 찾는다.(첫번째 인자가 False이면 Output)

            //Output 전체 채널 개수
            Cmmsdk.cmmAdvGetNumAvailDioChan((int)MotnDefines._TCmBool.cmFALSE, ref m_iMaxOut); //현재 사용 가능한 Input채널 갯수를 찾는다.(첫번째 인자가 False이면 Output)

            m_aModuleInfo = new TModuleInfo();
            m_aModuleInfo.iCntIn  = m_iMaxIn ;
            m_aModuleInfo.iCntOut = m_iMaxOut;

            return true;

            //Input모듈 개수 찾는곳
            //for (int i = 0; i < m_iMaxIn; i++)
            //{
            //    //이거 안될수도 있음 확인해야함. 진섭.
            //    //해당 함수는 IsInputChannel  인자로 선택 된 Input 혹은 Output의 Axis 인자로 선택 된 채널의 장치 Instance를 반환합니다.
            //    //Instance는 같은 종류의 장치(ex SD404 2장)을 사용할 경우 각 장치를 구분하기 위한 숫자로 0번부터 시작합니다.
            //    Cmmsdk.cmmAdvGetDioDevInstance(i, (int)MotnDefines._TCmBool.cmTRUE, ref m_iInModuleCnt); //Axis: DevInstance를 확인할 채널
            //                                                                                          //IsInputChannel : input, output 선택( 1 : Input, 0 : Output)
            //                                                                                          //DevInstance: 선택 된 채널의 장치의 Instance
            //}

            //Output 모듈 개수 찾는곳
            //for (int o = 0; o < m_iMaxOut; o++)
            //{
            //    //이거 안될수도 있음 확인해야함. 진섭.
            //    //해당 함수는 IsInputChannel  인자로 선택 된 Input 혹은 Output의 Axis 인자로 선택 된 채널의 장치 Instance를 반환합니다.
            //    //Instance는 같은 종류의 장치(ex SD404 2장)을 사용할 경우 각 장치를 구분하기 위한 숫자로 0번부터 시작합니다.
            //    Cmmsdk.cmmAdvGetDioDevInstance(o, (int)MotnDefines._TCmBool.cmFALSE, ref m_iOutModuleCnt); //Axis: DevInstance를 확인할 채널
            //                                                                                          //IsInputChannel : input, output 선택( 1 : Input, 0 : Output)
            //                                                                                          //DevInstance: 선택 된 채널의 장치의 Instance
            //}

            //m_iModuleCnt = 0;
            //m_iModuleCnt = m_iInModuleCnt + m_iOutModuleCnt;
            //m_aModuleInfo = new TModuleInfo[m_iModuleCnt];

        }

        /// <summary>
        /// 닫기 함수.
        /// </summary>
        /// <returns>성공 했는지 실패했는지 여부</returns>
        public bool Close()
        {
            Cmmsdk.cmmGnDeviceUnload();
            return true;
        }

        /// <summary>
        /// IO번호에 따른 해당 모듈의 정보 리턴.
        /// </summary>
        /// <param name="_iNo">정보를 가져올 IO번호</param>
        /// <param name="_iModuleNo">IO번호에 따른 IO모듈넘버</param>
        /// <param name="_iModuleNoDp">IO번호에 따른 Display용 모듈넘버</param>
        /// <param name="_iOffset">해당 모듈에서의 IO번호.</param>
        /// <returns>성공 여부.</returns>
        public bool GetInfoInput(int _iNo, out int _iModuleNo, out int _iModuleNoDp, out int _iOffset)
        {
            _iModuleNo = 0;
            _iModuleNoDp = 0;

            _iOffset = _iNo;

            int iOffset = _iNo;
            int iDpNo = 0;

            for (int i = 0; i < m_iModuleCnt; i++)
            {
                if ((iOffset - m_aModuleInfo.iCntIn < 0))
                {
                    _iModuleNo = i;
                    _iModuleNoDp = iDpNo;
                    _iOffset = iOffset;
                    return true;
                }
                iOffset -= m_aModuleInfo.iCntIn;
                if (m_aModuleInfo.iCntIn > 0) iDpNo++;
            }

            return false;
        } 
        /// <summary>
        /// IO번호에 따른 해당 모듈의 정보 리턴.
        /// </summary>
        /// <param name="_iNo">정보를 가져올 IO번호</param>
        /// <param name="_iModuleNo">IO번호에 따른 IO모듈넘버</param>
        /// <param name="_iModuleNoDp">IO번호에 따른 Display용 모듈넘버</param>
        /// <param name="_iOffset">해당 모듈에서의 IO번호.</param>
        /// <returns>성공 여부.</returns>
        public bool GetInfoOutput(int _iNo, out int _iModuleNo, out int _iModuleNoDp, out int _iOffset)
        {
            _iModuleNo = 0;
            _iModuleNoDp = 0;
            _iOffset = _iNo;

            int iOffset = _iNo;
            int iDpNo = 0;

            for (int i = 0; i < m_iModuleCnt; i++)
            {
                if ((iOffset - m_aModuleInfo.iCntOut < 0))
                {
                    _iModuleNo = i;
                    _iModuleNoDp = iDpNo;
                    _iOffset = iOffset;
                    return true;
                }
                iOffset -= m_aModuleInfo.iCntOut;
                if (m_aModuleInfo.iCntOut > 0) iDpNo++;
            }

            return false;
        }

        /// <summary>
        /// IO출력
        /// </summary>
        /// <param name="_iNo">IO번호</param>
        /// <param name="_bOn">true=ON , false=OFF</param>
        /// <returns>성공여부</returns>
        public bool SetOut(int _iNo, bool _bOn, bool _bDirect = false)
        {
            //Check Error.
            if (_iNo >= m_iMaxOut || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;


            GetInfoOutput(_iNo, out iModule, out iModuleDp, out iBitAddr);

            //OutPut
            if (_bDirect)
            {
                int iOn = _bOn ? 1 : 0;
                Cmmsdk.cmmDoPutOne(_iNo, iOn);
                return true;
            }
            else//여기 꼭 확인해야함. 안될 가능성이 99.999999999%임
            {
                int iOn = 0;
                if   (_bOn) m_aModuleInfo.iDataOutSet |=  (((iOn >> iBitAddr) | 0x01) << iBitAddr);
                else        m_aModuleInfo.iDataOutSet &= ~(((iOn >> iBitAddr) | 0x01) << iBitAddr);
                return true;
            }
        }
        /// <summary>
        /// IO 출력 상태 가져오기.
        /// </summary>
        /// <param name="_iNo">IO번호</param>
        /// <returns>IO상태</returns>
        public bool GetOut(int _iNo, bool _bDirect = false)
        {
            //Check Error.
            if (_iNo >= m_iMaxOut || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;


            GetInfoOutput(_iNo, out iModule, out iModuleDp, out iBitAddr);

            //OutPut
            if (_bDirect)
            {
                int iOn = 0;
                Cmmsdk.cmmDoGetOne(_iNo, ref iOn);
                return iOn == 0 ? false : true;
            }
            else
            {
                bool bRet = ((m_aModuleInfo.iDataOutGet >> iBitAddr) & 0x01) == 0x01;
                return bRet;
            }
        }
        /// <summary>
        /// IO입력 상태 가져오기.
        /// </summary>
        /// <param name="_iNo">IO번호</param>
        /// <returns>IO상태</returns>
        public bool GetIn(int _iNo, bool _bDirect = false)
        {
            //Check Error.
            if (_iNo >= m_iMaxIn || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;

            GetInfoInput(_iNo, out iModule, out iModuleDp, out iBitAddr);

            //OutPut
            if (_bDirect)
            {
                int iOn = 0;
                Cmmsdk.cmmDiGetOne(_iNo, ref iOn);
                return iOn == 0 ? false : true;
            }
            else
            {
                bool bRet = ((m_aModuleInfo.iDataIn >> iBitAddr) & 0x01) == 0x01;
                return bRet;
            }
        }
        /// <summary>
        /// 시그널을 받아 놓는다.
        /// </summary>
        public void Update()
        {
            //for (int i = 0; i < m_iModuleCnt; i++)
            //{
                Cmmsdk.cmmDoPutMulti(0, m_aModuleInfo.iCntOut ,    m_aModuleInfo.iDataOutSet) ;
                Cmmsdk.cmmDoGetMulti(0, m_aModuleInfo.iCntOut ,ref m_aModuleInfo.iDataOutGet) ;
                Cmmsdk.cmmDiGetMulti(0, m_aModuleInfo.iCntIn  ,ref m_aModuleInfo.iDataIn    ) ;
            //}
        }
    }



}
