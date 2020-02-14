using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MotionInterface;
using COMMON;
//using Paix_MotionControler;
using Paix_MotionController;

namespace MotionNMC2
{
    public class CDio : IDio
    {
        public CDio() { }

        //하드웨어 IP No.
        //short m_nDevId ;

        int m_iMaxIn  = 0;
        int m_iMaxOut = 0;

        public struct TModuleInfo
        {
            public int     iCntIn       ;
            public int     iCntOut      ;
            public short   nNmcNo       ;
            public short[] nDataIn      ; //64, 128
            public short[] nDataOutGet  ; //64, 128
            public short[] nDataOutSet  ;
            public short[] nDataOutNo   ;
        }
        //short[] ReadStatIn  = new short[128];//= new short[64]; 64도 호환되는거 같아서 128로만함.
        //short[] ReadStatOut = new short[128];//= new short[64]; 안되면 밑에서 64따로 128 따로 해야됨.
        public TModuleInfo[] m_aModuleInfo;
        int m_iModuleCnt = 0;

        public void QuickSort(int[] _array, int left, int right)
        {
            int i = left;   // 왼쪽
            int j = right;  // _array.Length;
            int pivot = _array[(left + right) / 2];
            while (true)
            {
                while (_array[i] < pivot) i++;// 큰값 찾기
                while (_array[j] > pivot) j--;  // 작은 값 찾기
                if (i > j) break;
                int temp = _array[i];
                _array[i] = _array[j];
                _array[j] = temp;
                i++;
                j--;
            }

            if (left < j)
                QuickSort(_array, left, j);
            if (right > i)
                QuickSort(_array, i, right);
        }

        public bool Init()
        {
            short[] nIp = new short [] { 192,168,0,255 }; // 192.168.0.XXX로 되는 모든 주소로 장치 검색
            //무조건 첫모듈은 1번이여야 하고 순서대로 설정해야 함. 1,2,3,4,5,6,
            //이함수에서 모듈을 선착순 순서로 리턴해서 순서가 렌덤임.
            int nCount = NMC2.nmc_GetEnumList(nIp,out NMC2.NMCEQUIPLIST NmcEquipList);

            if(nCount <= 0 ) 
            {
                Log.ShowMessage("Dio", "Module Count is 0, plz Check IP Add");
                return false;
            }

            for(int i=0; i<nCount; i++) 
            {
                if(NmcEquipList.nDioType[i] != 0) m_iModuleCnt++;
            }
            m_aModuleInfo = new TModuleInfo[m_iModuleCnt];
            
            m_iModuleCnt = 0;
            //string sTemp = "";
            for(int j=1; j<nCount+1; j++)
            { 
                for(int i = 0; i < nCount; i++)//2개이상 모듈을 사용할때 IP작은순으로 들어오지 않고 선착순으로 들어옴....
                {
                    if (NmcEquipList.lModelType[i] == NMC2.NMC2_UDIO)
                    {
                        //int i1 = NmcEquipList.lIp[0]     & 0xff;
                        //int i2 = NmcEquipList.lIp[0]>>8  & 0xff;
                        //int i3 = NmcEquipList.lIp[0]>>16 & 0xff;

                        int i4 = NmcEquipList.lIp[i] >> 24 & 0xff;
                        if (i4 != j) continue;

                        //sTemp = sTemp + i4.ToString();
                        m_aModuleInfo[m_iModuleCnt].nNmcNo = (short)i4;
                        if (NMC2.nmc_OpenDevice(m_aModuleInfo[m_iModuleCnt].nNmcNo) != 0)
                        {
                            Log.ShowMessage("Dio", "Paiz NMC2 IO Module Port Open Fail");
                            return false;
                        }

                        NMC2.nmc_GetDIOInfo(m_aModuleInfo[m_iModuleCnt].nNmcNo, out short iCntIn, out short iCntOut);
                        m_aModuleInfo[m_iModuleCnt].iCntIn = iCntIn;
                        m_aModuleInfo[m_iModuleCnt].iCntOut = iCntOut;
                        m_aModuleInfo[m_iModuleCnt].nDataIn = new short[128];
                        m_aModuleInfo[m_iModuleCnt].nDataOutGet = new short[128];
                        m_aModuleInfo[m_iModuleCnt].nDataOutSet = new short[128];//new short[iCntOut]; 16개나 128개나 시간상 똑같아서 128
                        m_aModuleInfo[m_iModuleCnt].nDataOutNo = new short[128];//new short[iCntOut];
                        m_iMaxIn += iCntIn;
                        m_iMaxOut += iCntOut;
                        for (int k = 0; k < 128; k++)
                        {
                            m_aModuleInfo[m_iModuleCnt].nDataOutNo[k] = (short)k;
                        }
                        m_iModuleCnt++;
                    }
                }
                
            }


            //for(int y = 0; y < m_iMaxOut; y++)
            //{
            //    SetOut(y,GetOut(y));
            //}

            for (int i = 0; i < m_iModuleCnt; i++)
            {
                NMC2.nmc_GetDIOOutput128(m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataOutSet);
                NMC2.nmc_GetDIOOutput128(m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataOutGet);
                NMC2.nmc_GetDIOInput128(m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataIn);
            }
            //Log.ShowMessage(sTemp, sTemp);

            /*
            short nRet;
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
            */

            //NMC2E-UDIO-CPU, NMC2E-UD16&UDO16 사용시
            //int plDeviceType;
            //NMC2.nmc_GetDeviceType(m_nDevId, out plDeviceType);
            //short pnInCount, pnOutCount ;
            //int iRet = NMC2.nmc_GetDIOInfo(m_nDevId, out pnInCount, out pnOutCount);
            //m_iMaxIn  = pnInCount ;
            //m_iMaxOut = pnOutCount;
            //if(iRet != 0) {
            //    m_iMaxIn = 0 ; m_iMaxOut = 0 ; 
            //}
            //ReadStatIn  = new short[m_iMaxIn ];
            //ReadStatOut = new short[m_iMaxOut];

            return true;
        }

        public bool Close()
        {

            MotionNMC2.CModule.CloseDevice();
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

        public bool SetOut(int _iNo, bool _bOn, bool _bDirect = false)
        {
            //Check Error.
            if (_iNo >= m_iMaxOut || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;

            GetInfoOutput(_iNo, out iModule, out iModuleDp , out iBitAddr);

            short nOn = _bOn ? (short)1 : (short)0;

            if (_bDirect)
            {
                NMC2.nmc_SetDIOOutPin(m_aModuleInfo[iModule].nNmcNo,(short)iBitAddr,nOn);
            }
            else{
                m_aModuleInfo[iModule].nDataOutSet[iBitAddr] = nOn ;
            }

            return true;

        }

        public bool GetOut(int _iNo, bool _bDirect = false)
        {
            //Check Error.
            if (_iNo >= m_iMaxOut || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;

            GetInfoOutput(_iNo, out iModule, out iModuleDp, out iBitAddr);

            bool _bOn;
            //if (_bDirect) NMC2.nmc_GetDIOOutput (m_aModuleInfo[iModule].nNmcNo, m_aModuleInfo[iModule].nDataOutGet);

            if (m_aModuleInfo[iModule].nDataOutGet[iBitAddr] == 0) { _bOn = false; }
            else                                                   { _bOn = true ; }

            return _bOn;

        }

        public bool GetIn(int _iNo, bool _bDirect = false)
        {
            //Check Error.
            if (_iNo >= m_iMaxIn || _iNo < 0) { return false; }

            //Get Addr.
            int iBitAddr;
            int iModule;
            int iModuleDp;

            GetInfoInput(_iNo, out iModule, out iModuleDp, out iBitAddr);

            bool _bOn;
            //if (_bDirect) NMC2.nmc_GetDIOInput (m_aModuleInfo[iModule].nNmcNo, m_aModuleInfo[iModule].nDataIn);

            if (m_aModuleInfo[iModule].nDataIn[iBitAddr] == 0) { _bOn = false; }
            else                                               { _bOn = true;  }

            return _bOn;
        }
        /*
        public void Update()
        {
            //NMC2.nmc_GetMDIOOutput(m_nDevId, ReadStatOut);
            //NMC2.nmc_GetMDIOInput (m_nDevId, ReadStatIn );
            for(int i=0; i < m_iModuleCnt; i++)
            {
                NMC2.nmc_SetDIOOutPins(m_aModuleInfo[i].nNmcNo, (short)m_aModuleInfo[i].nDataOutSet.Length , m_aModuleInfo[i].nDataOutNo, m_aModuleInfo[i].nDataOutSet);

                //short s = m_aModuleInfo[i].nDataOutSet[0] ;
                //NMC2.nmc_SetDIOOutPin(m_aModuleInfo[i].nNmcNo , 0 , s);

                //NMC2.nmc_GetDIOOutput (m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataOutGet);
                //NMC2.nmc_GetDIOInput  (m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataIn    );
                NMC2.nmc_GetDIOOutput128 (m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataOutGet);
                NMC2.nmc_GetDIOInput128  (m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataIn    );
                
                
            }
            
            
            
        }
        */
        public void Update()
        {
            //NMC2.nmc_GetMDIOOutput(m_nDevId, ReadStatOut);
            //NMC2.nmc_GetMDIOInput (m_nDevId, ReadStatIn );
            bool bDisconnected = false ;
            for(int i=0; i < m_iModuleCnt; i++)
            {
                if(NMC2.nmc_SetDIOOutPins(m_aModuleInfo[i].nNmcNo, (short)m_aModuleInfo[i].nDataOutSet.Length , m_aModuleInfo[i].nDataOutNo, m_aModuleInfo[i].nDataOutSet) == NMC2.NMC_NOTCONNECT) bDisconnected = true ;
                

                //short s = m_aModuleInfo[i].nDataOutSet[0] ;
                //NMC2.nmc_SetDIOOutPin(m_aModuleInfo[i].nNmcNo , 0 , s);

                //NMC2.nmc_GetDIOOutput (m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataOutGet);
                //NMC2.nmc_GetDIOInput  (m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataIn    );
                if(NMC2.nmc_GetDIOOutput128 (m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataOutGet) == NMC2.NMC_NOTCONNECT) bDisconnected = true ;
                if(NMC2.nmc_GetDIOInput128  (m_aModuleInfo[i].nNmcNo, m_aModuleInfo[i].nDataIn    ) == NMC2.NMC_NOTCONNECT) bDisconnected = true ;
                
                
            }

            //이건 아예 첨부터 장비 파워 안들어갔을때. 중간에 장비 전원 껐다켰을때.
            if (m_iModuleCnt==0 || bDisconnected)
            {
                short[] nIp = new short[] { 192, 168, 0, 255 }; // 192.168.0.XXX로 되는 모든 주소로 장치 검색
                int nCount = NMC2.nmc_GetEnumList(nIp, out NMC2.NMCEQUIPLIST NmcEquipList);
                if(nCount != 0)
                {
                    Init();
                }
            }

            
        }

    }
}
