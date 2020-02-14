using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using COMMON;
using System.Windows.Forms;

namespace Machine
{
    public class SPC_Sub
    {
        public static CArray SPCARAY = new CArray();

        //HVI-1500i
        //포스트버퍼에 검사 끝난 결과 데이터 csv 파일로 저장
        public void SaveDataMap(int _iArayId)
        {
            //Read&Write.
            CConfig Config = new CConfig();
            string sSPCFolder = System.IO.Directory.GetParent(SPC.LOT.Folder).Parent.FullName.ToString();
            string sPath = "";
            string sToday = DateTime.FromOADate(SPC.LOT.Data.StartedAt).ToString("yyyyMMdd");//DateTime.Now.ToString("yyyyMMdd");


            int iMgzNo  = int.Parse(DM.ARAY[ri.PREB].ID) / 100;
            int iSlotNo = int.Parse(DM.ARAY[ri.PREB].ID) % 100;
            sPath = sSPCFolder + "\\DataMap\\" + sToday + "\\" + DM.ARAY[_iArayId].LotNo + "\\" + iMgzNo.ToString() + "\\" + iSlotNo.ToString() + ".INI";
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(sPath));
            if (!di.Exists) di.Create();

            DM.ARAY[_iArayId].Load(Config, false);

            Config.Save(sPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        }

        //HVI-1500i
        //DataMap csv 파일 로드
        public void LoadDataMap(string _sPath)
        {
            //Read&Write.
            CConfig Config = new CConfig();
            string sPath = "";

            sPath = _sPath;

            Config.Load(sPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);

            SPCARAY.Load(Config, true);
        }
    }
}
