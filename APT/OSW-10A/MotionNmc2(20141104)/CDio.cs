using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MotionInterface;
using COMMON;
using Paix_MotionControler;

namespace MotionNMC2
{
    public class CDio : IDio
    {
        public CDio() { }

        //하드웨어 IP No.
        short m_nDevId ;

        int m_iMaxIn  = 0;
        int m_iMaxOut = 0;

        short[] ReadStat = new short[64];

        public bool Init()
        {
            short nRet;

            if (!MotionNMC2.CModule.OpenDevice(out m_nDevId)) return false;

            // DIO 입/출력 개수 정보 가져오기 ========================
            short nMotionType, nIOType, nExtIo, nMDio;
            nRet = NMC2.nmc_GetDeviceInfo(m_nDevId, out nMotionType, out nIOType, out nExtIo, out nMDio);
            //nIOType은 IO Controller 사용시.
            //switch (nIOType)
            //{
            //    case  1: m_iMaxIn = 16; m_iMaxOut = 16; break;
            //    case  2: m_iMaxIn = 32; m_iMaxOut = 32; break;
            //    case  3: m_iMaxIn = 48; m_iMaxOut = 48; break;
            //    case  4: m_iMaxIn = 64; m_iMaxOut = 64; break;
            //    default: m_iMaxIn = 0 ; m_iMaxOut = 0 ; break;
            //}
            //기본 Motion Controller 사용시는 NMDio사용.
            switch(nMDio)
            {
                case  1: m_iMaxIn = 8 ; m_iMaxOut = 8 ; break;
                default: m_iMaxIn = 0 ; m_iMaxOut = 0 ; break;
            }
      
            return true;
        }

        public bool Close()
        {

            MotionNMC2.CModule.CloseDevice();
            return true;

        }

        public bool GetInfoInput(int _iNo, out int _iModuleNo, out int _iModuleNoDp, out int _iOffset)
        {
            //이건 사용 안해서 그냥 0으로 리턴.
            _iModuleNo = 0;
            _iModuleNoDp = 0;
            _iOffset = _iNo;
            return true;
        }
        public bool GetInfoOutput(int _iNo, out int _iModuleNo, out int _iModuleNoDp, out int _iOffset)
        {
            //이건 사용 안해서 그냥 0으로 리턴.
            _iModuleNo = 0;
            _iModuleNoDp = 0;
            _iOffset = _iNo;
            return true;
        }

        public bool SetOut(int _iNo, bool _bOn)
        {
            //Check Error.
            if (_iNo >= m_iMaxOut || _iNo < 0) { return false; }

            //OutPut
            short nOn = _bOn ? (short)1 : (short)0;
            short nRet;

            //Controller 추가 연결시 사용 되는거 같다.
            //nRet = NMC2.nmc_SetDIOOutPin(m_nDevId, (short)_iNo, nOn);
            nRet = NMC2.nmc_SetMDIOOutPin(m_nDevId, (short)_iNo, nOn);

            return true;

        }
        public bool GetOut(int _iNo)
        {
            //Check Error.
            if (_iNo >= m_iMaxOut || _iNo < 0) { return false; }

            bool _bOn;
            
            short nRet;

            //Controller 추가 연결시 사용 되는거 같다.
            //nRet = NMC2.nmc_GetDIOOutput(m_nDevId, ReadStat);
            nRet = NMC2.nmc_GetMDIOOutput(m_nDevId, ReadStat);

            //OutPut
            if (ReadStat[_iNo] == 0) { _bOn = false; }
            else                     { _bOn = true ; }

            return _bOn;

        }
        public bool GetIn(int _iNo)
        {
            //Check Error.
            if (_iNo >= m_iMaxIn || _iNo < 0) { return false; }

            bool _bOn;

            //Controller 추가 연결시 사용 되는거 같다.
            //NMC2.nmc_GetDIOInput(m_nDevId, ReadStat);
            NMC2.nmc_GetMDIOInput(m_nDevId, ReadStat);

            if (ReadStat[_iNo] == 0) { _bOn = false; }
            else                     { _bOn = true;  }

            return _bOn;
        }
    }
}
