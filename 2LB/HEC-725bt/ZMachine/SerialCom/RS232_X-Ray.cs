using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace Machine
{
    public class RS232_X_Ray : CSerialPort
    {
        public RS232_X_Ray(int _iPortId , string _sName, string _sEndOfText="\r"):base(_iPortId , _sName, _sEndOfText)
        {
            /*초기화 상태.
            BaudRate=115200
            DataBit=8
            ParityBit=0
            StopBit=1           
            */
        }

        public bool SetVolt(int _iVolt)
        {
            bool bRet = SendMsg("[spm_hv"+_iVolt+"60]");
            return bRet;
        }

        public bool SetAmpere(int _iAmpere)
        {
            bool bRet = SendMsg("[spm_hv" + _iAmpere + "70]");
            return bRet;
        }

        public bool SetTime(int _iTime)
        {
            bool bRet = SendMsg("[spm_hv" + _iTime + "210]");
            return bRet;
        }

        //public bool Run()
        //{
        //    bool bRet = SendMsg("[run]");
        //    return bRet;
        //}

        public void SetXrayPara(int _iVolt, int _iAmpere, int _iTime)
        {
            //이거 안되면 사이에 딜레이 넣어보고 그래도 안되면 그냥 따로 쓰자.
            SetVolt(_iVolt);
            SetAmpere(_iAmpere);
            SetTime(_iTime);
        }

        public void SetXrayPara(string _iVolt, string _iAmpere, string _iTime)
        {
            int i1 = CConfig.StrToIntDef(_iVolt  ,0);
            int i2 = CConfig.StrToIntDef(_iAmpere,0);
            int i3 = CConfig.StrToIntDef(_iTime  ,0);
            SetVolt(i1);
            SetAmpere(i2);
            SetTime(i3);
        }

    }
}
