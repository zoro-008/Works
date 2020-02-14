using System.Collections.Generic;
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
            public string sMaterialNo  ;
            public string sEmployeeID  ;
            public string sLotNo       ;
            public string sTargetBin   ;
        }
        
        //public static List<TLot> LotList = new List<TLot> ();
        public static TLot CrntLotData = new TLot();
        public static bool     LotOpened = false ;
        public static bool     LotEnded  = false ;

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
                IniLotInfo.Load("Total ", "LotOpened", out LotOpened);
                IniLotInfo.Load("Total ", "LotEnded" , out LotEnded );
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
            //LOT.LotList.Remove(LOT.LotList[0]);
        }
        public static void LotOpen(string _sLotId)
        {
            Log.Trace("SEQ", ("Lot Open : " + _sLotId).ToString());
            LotOpened = true;

            CrntLotData.sLotNo = _sLotId;
            //LOT.LotList.Remove(LOT.LotList[0]);
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
           
            //Reset Lot Flag.
            CrntLotData.sLotNo = "";
            LotOpened  = false;
            LotEnded   = true ;

            //업체요청 지움
            //SEQ.LEFT.iWorkCnt = 0;
            //SEQ.RIGH.iWorkCnt = 0;
            //SEQ.LODR.iLDRSplyCnt = 0;
        
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
