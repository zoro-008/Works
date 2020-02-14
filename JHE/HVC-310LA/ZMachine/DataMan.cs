using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON;
using System.Windows.Forms;
using System.IO;

namespace Machine
{
    public class DM
    {
         public static CArray[] ARAY = new CArray[(int)ri.MAX_ARAY];

        public static void Init()
        {

            for(int i = 0; i < (int)ri.MAX_ARAY; i++)
            {
                ARAY[i] = new CArray();
            }
        }

        public static void LoadMap()//여기 뻑나는거 해야함 0704
        {
            //Read&Write.
            CConfig Config = new CConfig();
            String sPath;

            sPath = Directory.GetCurrentDirectory();
            sPath = sPath + "\\SeqData\\ArrayData.INI";

            Config.Load(sPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);

            for (int i = 0 ; i < (int)ri.MAX_ARAY ; i++)
                ARAY[i].Load(Config , true);
        }

        public static void SaveMap()
        {
            //Read&Write.
            CConfig Config = new CConfig();
            String sPath;

            sPath = Directory.GetCurrentDirectory();
            sPath = sPath + "\\SeqData\\ArrayData.INI";

            

            //Read&Write.
            for (int i = 0; i < (int)ri.MAX_ARAY; i++)
                ARAY[i].Load(Config , false);

            Config.Save(sPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        }
        
    }
}
