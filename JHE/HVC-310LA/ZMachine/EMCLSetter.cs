using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EraeMotionApi;
using COMMON;

namespace Machine
{
    class EMCLSetter
    {
        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        /// Motrid (1~2)
        static public bool SetTorque(int _iMotrId , double _dTorque)
        {
            //통합 보드 초기화 부분.                
            //9600
            //14400
            //19200
            //38400
            //57600
            //115200
            const int iBaudRate = 38400;
            const int iPortId = 3;

            //ERAETech_EMCL_OpenComm
            if (!EMCL.ERAETech_EMCL_OpenComm(iPortId, iBaudRate))
            {
                return false;
            }
            int iMaxMotr = EMCL.ERAETech_EMCL_GetNodeCount(iPortId, 2);//포트에 몇개의 모터가 달려 있는지...확인 10개까지 확인해 본다.
            if (iMaxMotr != 2)
            {
                return false;
            }

            EMCL.ERAETech_EMCL_CloseComm(-1);//해당포트를 닫는다 -1 이면 모든 포트 닫음.
            return true;
        }
    }
}
