using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Paix_MotionControler;
using COMMON;

namespace MotionNMC2
{
    //멀티 모듈이 적용 안되고 오직 1개만 쓸듯 해서 1개전용 으로 만듬.
    static public class CModule
    {
        static bool bOpened = false;

        //Fixed address....... change when you needed ..
        const short m_nModuleIP = 11;
        static public bool OpenDevice(out short _nModuleIP)
        {
            _nModuleIP = m_nModuleIP;
            if (bOpened)
            {                
                return true;
            }

            short nRet;

            if (NMC2.nmc_PingCheck(m_nModuleIP, 200) != 0)
            {
                Log.ShowMessage("PAIX", "Ping Test Error 192.168.0." + m_nModuleIP.ToString());
                return false;
            }

            nRet = NMC2.nmc_OpenDevice(m_nModuleIP);

            if (nRet == 0)
            {
                bOpened = true;
            }
            else
            {
                Log.ShowMessage("PAIX", "NMC2 Open Failed");
                return false;
            }

            return bOpened;
        }

        static public void CloseDevice()
        {
            if (!bOpened) return;
            NMC2.nmc_CloseDevice(m_nModuleIP);
            bOpened = false;
        }

        static public bool ReOpenDevice()
        {
            NMC2.nmc_CloseDevice(m_nModuleIP);

            if(NMC2.nmc_PingCheck(m_nModuleIP, 200) != 0){
                Log.Trace("PAIX", "ReOpen Ping Test Error 192.168.0." + m_nModuleIP.ToString());
                return false;
            }

            short nRet = NMC2.nmc_OpenDevice(m_nModuleIP);

            if(nRet == 0)
            {
                Log.ShowMessage("PAIX", "ReOpen Success! 192.168.0." + m_nModuleIP.ToString());
                return true;
            }
            Log.Trace("PAIX", "ReOpen Failed! 192.168.0." + m_nModuleIP.ToString());
            return false ;
        }
    }

    
}
