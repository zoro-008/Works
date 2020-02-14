﻿//using System.Windows.Forms;

using MotionInterface;
using COMMON;

namespace MotionAXL
{
    public class CDio:IDio
    {
        public CDio() {}

        public struct TModuleInfo
        {
            public int iCntIn;
            public int iCntOut;
        };

        //하드웨어상의 모듈갯수. 
        public TModuleInfo[] m_aModuleInfo;
        int m_iModuleCnt;

        //모듈의 접점 카운트
        int m_iMaxIn = 0;
        int m_iMaxOut= 0;


        //상속된것들.
        public bool Init()
        {
            //통합 보드 초기화 부분.
            if (CAXL.AxlIsOpened() == 0)
            {				// 통합 라이브러리가 사용 가능하지 (초기화가 되었는지) 확인
                if (CAXL.AxlOpenNoReset(7) != 0)
                {			// 통합 라이브러리 초기화
                    Log.ShowMessage("Dio", "AJIN AXL Lib Loading Error");
                    return false;
                }
            }

            uint uiStatus = 0;
            uint uiRet = CAXD.AxdInfoIsDIOModule(ref uiStatus);
            if (uiRet != 0)
            {
                if (uiStatus == 0)
                {
                    Log.ShowMessage("Dio", "AJIN AXL No Exist IO Module");
                }
                else
                {
                    Log.ShowMessage("Dio", "AJIN AXL Init Failed");
                }

                return false;
            }
            
            

            //모듈 정보 확인 부분.
            //InPut OutPut Count Set
            CAXD.AxdInfoGetModuleCount(ref m_iModuleCnt);


            //Get Max I Module , O Module Count.
            int iInputCnt = 0 , iOutputCnt = 0;


            m_aModuleInfo = new TModuleInfo[m_iModuleCnt];
            for (int i = 0; i < m_iModuleCnt; i++)
            {
                CAXD.AxdInfoGetInputCount (i, ref iInputCnt ); 
                CAXD.AxdInfoGetOutputCount(i, ref iOutputCnt);

                m_iMaxIn += iInputCnt; 
                m_iMaxOut += iOutputCnt;

                m_aModuleInfo[i].iCntIn = iInputCnt;
                m_aModuleInfo[i].iCntOut = iOutputCnt;

            }
            
            return true;
        }

        public bool Close()
        {
            CAXL.AxlClose();
            return true;

        }
                
        public bool GetInfoInput(int _iNo, out int _iModuleNo, out int _iModuleNoDp, out int _iOffset)
        {
            _iModuleNo = 0;
            _iModuleNoDp = 0;
            _iOffset = _iNo;

            int iOffset = _iNo;
            int iDpNo = 0;

            for (int i = 0; i < m_iModuleCnt; i++)
            {
                if ((iOffset - m_aModuleInfo[i].iCntIn < 0))
                {
                    _iModuleNo = i;
                    _iModuleNoDp = iDpNo;
                    _iOffset = iOffset;
                    return true;
                }
                iOffset -= m_aModuleInfo[i].iCntIn;
                if (m_aModuleInfo[i].iCntIn > 0) iDpNo++;
            }

            return false;

        }
        public bool GetInfoOutput(int _iNo, out int _iModuleNo, out int _iModuleNoDp, out int _iOffset)
        {
            _iModuleNo = 0;
            _iModuleNoDp = 0;
            _iOffset = _iNo;

            int iOffset = _iNo;
            int iDpNo = 0;

            for (int i = 0; i < m_iModuleCnt; i++)
            {
                if ((iOffset - m_aModuleInfo[i].iCntOut < 0))
                {
                    _iModuleNo = i;
                    _iModuleNoDp = iDpNo;
                    _iOffset = iOffset;
                    return true;
                }
                iOffset -= m_aModuleInfo[i].iCntOut;
                if (m_aModuleInfo[i].iCntOut > 0) iDpNo++;
            }

            return false;
        }
        public bool SetOut(int _iNo, bool _bOn)
        {
            //Check Error.
            if (_iNo >= m_iMaxOut || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;
            

            GetInfoOutput(_iNo, out iModule, out iModuleDp , out iBitAddr);

            //OutPut
            uint uiOn = _bOn ? (uint)1 : (uint)0 ;

            CAXD.AxdoWriteOutportBit(iModule, iBitAddr, uiOn);
            return true;

        }
        public bool GetOut(int _iNo)
        {
            //Check Error.
            if (_iNo >= m_iMaxOut || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;


            GetInfoOutput(_iNo, out iModule, out iModuleDp, out iBitAddr);

            //OutPut
            uint uiOn = 0;
            CAXD.AxdoReadOutportBit(iModule, iBitAddr, ref uiOn);

            return uiOn == 0 ? false : true;

        }
        public bool GetIn(int _iNo)
        {
            //Check Error.
            if (_iNo >= m_iMaxIn || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;


            GetInfoInput(_iNo, out iModule, out iModuleDp, out iBitAddr);

            //OutPut
            uint uiOn = 0;
            CAXD.AxdiReadInportBit(iModule, iBitAddr, ref uiOn);

            return uiOn == 0 ? false : true;
        }
    }
}
