using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using COMMON;


//using System.Runtime.InteropServices;
//using SML.CXmlBase;
//using SMLDefine;
//using SMLApp;

namespace Machine
{
    public class LOT
    {
        public struct TLot{
            public string sLotNo       ;
        }
        
        //public static List<TLot> LotList = new List<TLot> ();
        public static TLot CrntLotData = new TLot();
        public static bool LotOpened = false ;
        public static bool LotEnded  = false ;

        public static void Init()
        {
            //LotList.Clear();
        
            LoadSave(true);
        }

        public static void Close()
        {
            LoadSave(false);
        }

        /*
         public static void LoadEqpStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";
            CAutoIniFile.LoadStruct<CEqpStat>(sEqpOptnPath,"EqpStat",ref EqpStat);
        }
        public static void SaveEqpStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";
            CAutoIniFile.SaveStruct<CEqpStat>(sEqpOptnPath,"EqpStat",ref EqpStat);
        }
         */
       
        public static bool LoadSave(bool _bLoad)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLotInfo = sExeFolder + "SeqData\\LotInfo.ini";  
                    
            //Current Lot Informations.
            TLot Data = new TLot();
            CIniFile IniLotInfo = new CIniFile(sLotInfo);
            //int iCount = LotList.Count ;
            if(_bLoad) 
            {
                //IniLotInfo.Load("Total ", "Count"    , out iCount   );
                IniLotInfo.Load("Total ", "LotOpened", ref LotOpened);
                IniLotInfo.Load("Total ", "LotEnded" , ref LotEnded );
                CAutoIniFile.LoadStruct<TLot>(sLotInfo,"CrntLotData",ref CrntLotData);

                //LotList.Clear();
                //for(int i = 0 ; i < iCount ; i++){
                //    CAutoIniFile.LoadStruct<TLot>(sLotInfo,i.ToString(),ref Data);
                //    LotList.Add(Data);
                //}
                
                
            }
            else 
            {
                //IniLotInfo.Save("Total ", "Count"    , iCount   );
                IniLotInfo.Save("Total ", "LotOpened", LotOpened);
                IniLotInfo.Save("Total ", "LotEnded" , LotEnded );         
                CAutoIniFile.SaveStruct<TLot>(sLotInfo,"CrntLotData",ref CrntLotData);

                //for(int i = 0 ; i < iCount ; i++){
                //    Data = LotList[i];
                //    CAutoIniFile.SaveStruct<TLot>(sLotInfo,i.ToString(),ref Data);                    
                //}
                
            }
            return true;

        }
        
        //Lot Processing.
        public static void LotOpen(TLot _tLotData)
        {
            Log.Trace("SEQ",("Lot Open : " + _tLotData.sLotNo ).ToString()  );
            LotOpened  = true ;

            CrntLotData = _tLotData ;

            //char[] Special = {'\\', '/', ':', '*', '?', '"', '<', '>' , '|' };

            //특수 문자 제거 
            CrntLotData.sLotNo = RemoveSpecialCharacters(CrntLotData.sLotNo);
            Log.Trace("SEQ", ("Lot Opened : " + CrntLotData.sLotNo).ToString());
            OM.EqpStat.iWorkCnt = 0;
            OM.EqpStat.iULDRCnt = 0;
            OM.EqpStat.iRJCMCnt = 0;
            OM.EqpStat.iRJCVCnt = 0;
            //LOT.LotList.Remove(LOT.LotList[0]);
        }

        public static string RemoveSpecialCharacters(string str)
        {
            char[] buffer = new char[str.Length];
            int idx = 0;

            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z')
                    || (c >= 'a' && c <= 'z') || (c == '.') || (c == '_'))
                {
                    buffer[idx] = c;
                    idx++;
                }
            }

            return new string(buffer, 0, idx);
        }

        public static bool GetLotOpen()
        {
            return LotOpened ;
        }
        
        public static void Reset()
        {
            //LotOpened = false ;
            LotEnded  = false ;
        }
        
        public static void LotEnd()
        {
            //Check already opened Lot.
            Log.Trace("SEQ","Lot Finished"   );
           
            //그래프 저장하기
            string sPath = "d:\\SpcLog\\"+ Eqp.sEqpName + "\\LotLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + LOT.GetLotNo() + "\\ANODE(SET kV).jpg";
            string sDir  = Path.GetDirectoryName(sPath + "\\");
            
            Log.Trace("Check Save", sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;
            
            sPath = "d:\\SpcLog\\"+ Eqp.sEqpName + "\\LotLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + LOT.GetLotNo() + "\\";
            //FormOperation.FrmGraph1.Save(sPath+"ANODE(Set kV).jpg"); FormOperation.FrmGraph1C.Save(sPath+"ANODE(Pow mA).jpg");
            //FormOperation.FrmGraph3.Save(sPath+"FOCUS(Set kV).jpg"); FormOperation.FrmGraph3C.Save(sPath+"FOCUS(Pow mA).jpg");
            //FormOperation.FrmGraph5.Save(sPath+"GATE(Set kV).jpg" ); FormOperation.FrmGraph5C.Save(sPath+"GATE(Pow mA).jpg" );
            //FormOperation.FrmGraph2.Save(sPath+"ANODE(kV).jpg"    ); FormOperation.FrmGraph2C.Save(sPath+"CATHOD(mA).jpg"   );
            //FormOperation.FrmGraph4.Save(sPath+"FOCUS(kV).jpg"    ); FormOperation.FrmGraph4C.Save(sPath+"FOCUS(mA).jpg"    );
            //FormOperation.FrmGraph6.Save(sPath+"GATE(kV).jpg"     ); FormOperation.FrmGraph6C.Save(sPath+"GATE(mA).jpg"     );
            
            //BackUp
            string sPath1 = "c:\\SpcLog\\"+ Eqp.sEqpName + "\\LotLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + LOT.GetLotNo() + "\\";
            Log.Trace("Check Save", sPath1);
            //DeleteFolder(sPath1);
            CopyFolder(sPath,sPath1);

            Log.Trace("Check Save", "End");
            //Reset Lot Flag.
            CrntLotData.sLotNo = "";
            LotOpened  = false;
            LotEnded   = true ;

            //SEQ.aging.Reset(true);

            //MM.SetManCycle(mc.LODR_ManLotSupply);

            
            //SEQ.LODR.iLDRSplyCnt = 0;
        
        }

        //Folder Copy하는 함수
        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest,true);
            }

            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }
        
        /// <summary>
        /// FOLDER DELETE
        /// </summary>
        /// <param name="path">삭제할 폴더 경로</param>
        /// <returns>true,false반환</returns>
        public static bool DeleteFolder(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                    file.Attributes = FileAttributes.Normal;
                Directory.Delete(path, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool GetLotEnd ()
        {
            //Check already opened Lot.
            return LotEnded ;
        
        }

        public static string GetLotNo()
        {
            return CrntLotData.sLotNo ; 
        }
    }
        
        
  







}
