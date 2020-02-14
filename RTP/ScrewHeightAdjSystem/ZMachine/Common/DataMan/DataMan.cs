using System;
using COMMON;
using System.IO;

namespace Machine
{
    public class DM
    {
        public static CArray[] ARAY = new CArray[(int)ri.MAX_ARAY];
        
        //인덱서.
        //public CArray this[ri _eIndex]
        //{
        //    get
        //    {
        //        return ARAY[(int)_eIndex];
        //    }
        //    set
        //    {
        //        ARAY[(int)_eIndex] = value;
        //    }
        //}

        public static void Init()
        {
            for(int i = 0; i < (int)ri.MAX_ARAY; i++)
            {
                ARAY[i] = new CArray();
            }
        }

        public static void LoadMap()
        {
            //Read&Write.
            CConfig Config = new CConfig();
            string sPath = Directory.GetCurrentDirectory();
            string sPath1 = sPath + "\\SeqData\\ArrayPara.INI";
            string sPath2 = sPath + "\\SeqData\\ArrayData.INI";

            Config.Load(sPath1, CConfig.EN_CONFIG_FILE_TYPE.ftIni);

            for (int i = 0 ; i < (int)ri.MAX_ARAY ; i++) {
                ARAY[i].Load    (Config , true );
                ARAY[i].LoadData(sPath2 , true );
            }
        }

        public static void SaveMap()
        {
            //Read&Write.
            CConfig Config = new CConfig();
            string sPath  = Directory.GetCurrentDirectory();
            string sPath1 = sPath + "\\SeqData\\ArrayPara.INI";
            string sPath2 = sPath + "\\SeqData\\ArrayData.INI";

            //Read&Write.
            for (int i = 0; i < (int)ri.MAX_ARAY; i++) { 
                ARAY[i].Load    (Config , false);
                ARAY[i].LoadData(sPath2 , false);
            }

            Config.Save(sPath1, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        }
        public static void ShiftData(int _iFrom, int _iTo)
        {
            DM.ARAY[_iTo].SetMaxColRow(DM.ARAY[_iFrom].GetMaxCol(), DM.ARAY[_iFrom].GetMaxRow());

            DM.ARAY[_iTo].ID      = DM.ARAY[_iFrom].ID     ;
            DM.ARAY[_iTo].LotNo   = DM.ARAY[_iFrom].LotNo  ;
            DM.ARAY[_iTo].Step    = DM.ARAY[_iFrom].Step   ;
            DM.ARAY[_iTo].SubStep = DM.ARAY[_iFrom].SubStep;

            DM.ARAY[_iTo].Data    = DM.ARAY[_iFrom].Data   ;

            for (int r = 0; r < DM.ARAY[_iFrom].GetMaxRow(); r++)
            {
                //memcpy(CHPS[r],_cArray.CHPS[r],sizeof(CChip)*m_iMaxCol);
                for (int c = 0; c < DM.ARAY[_iFrom].GetMaxCol(); c++)
                {
                    DM.ARAY[_iTo].SetStat(c, r, DM.ARAY[_iFrom].GetStat(c, r));
                }
            }
            DM.ARAY[_iFrom].ClearMap();
        }
        public static void CopyData(int _iFrom, int _iTo)
        {
            DM.ARAY[_iTo].SetMaxColRow(DM.ARAY[_iFrom].GetMaxCol(), DM.ARAY[_iFrom].GetMaxRow());

            DM.ARAY[_iTo].ID      = DM.ARAY[_iFrom].ID     ;
            DM.ARAY[_iTo].LotNo   = DM.ARAY[_iFrom].LotNo  ;
            DM.ARAY[_iTo].Step    = DM.ARAY[_iFrom].Step   ;
            DM.ARAY[_iTo].SubStep = DM.ARAY[_iFrom].SubStep;

            for (int r = 0; r < DM.ARAY[_iFrom].GetMaxRow(); r++)
            {
                //memcpy(CHPS[r],_cArray.CHPS[r],sizeof(CChip)*m_iMaxCol);
                for (int c = 0; c < DM.ARAY[_iFrom].GetMaxCol(); c++)
                {
                    DM.ARAY[_iTo].SetStat(c, r, DM.ARAY[_iFrom].GetStat(c, r));
                }
            }
        }
    }
}
