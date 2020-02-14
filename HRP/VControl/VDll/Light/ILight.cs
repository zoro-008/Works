using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDll.Light
{
    interface ILight
    {
        bool Init();
        bool Close();
        
        bool   LoadSavePara(bool _bLoad , string _sParaFilePath);
        bool   ApplyPara(object _UserPara);
        bool   ShowSettingDialog();
        string GetError();

        Type   GetParaType();
    }
}
