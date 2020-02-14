using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    class Util
    {
        static public CMotrMan      MT;

        public static void Init()
        {
            MT = new CMotrMan();
            string sPath = Directory.GetCurrentDirectory() + "\\Util\\";
            int iCnt = 1;

            MT.Init(sPath, iCnt);
        }

        public static void Update()
        {
            MT.Update();
        }
    }
}
