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
        //public struct TLot{
        //    public string sLotNo       ;
        //    public int    iCnt         ;
        //
        //}

        static string LastPopLot  ; //맨마지막 메거진 뺀것.
        static int    iWorkMgzCnt = 0 ;         //메거진 작업 갯수. 첫번째 메거진 찝을때 1 , 두번째 메거진 찝을때 2로 
        static string CrntLotData ; //현재 작업중인 랏 정보.
        static bool LotOpened = false ;
        static bool LotEnded  = false ;
        
        //얘는 랏오픈창에서 핸들링 하기 좋게 퍼블릭.
        public static List<string> LotList = new List<string> ();



        //Queue<string> qLotNo = new Queue<string>();       
        //Work Mode
        //protected const int IN     = 0; //Input-WorkZone까지 1Cycle
        //protected const int OUT    = 1; //WorkZone-Output까지 1Cycle

        public static void Init()
        {
            //LotList.Clear();
            LoadSave(true);
        }

        public static void Close()
        {
            LoadSave(false);
        }

        public static bool LoadSave(bool _bLoad)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLotInfo = sExeFolder + "SeqData\\LotInfo.ini";  
                    
            CIniFile IniLotInfo = new CIniFile(sLotInfo);
            string sLot = "";
            int  LotCount  = 0     ;
            if(_bLoad) 
            {
                IniLotInfo.Load("Total ", "LotCount"    , ref LotCount    );
                IniLotInfo.Load("Total ", "LotOpened"   , ref LotOpened   );
                IniLotInfo.Load("Total ", "LotEnded"    , ref LotEnded    );
                IniLotInfo.Load("Total ", "iWorkMgzCnt" , ref iWorkMgzCnt );
                IniLotInfo.Load("Total ", "CrntLotData" , ref CrntLotData );
                IniLotInfo.Load("Total ", "LastPopLot"  , ref LastPopLot  ); 
                
                LotList.Clear();
                for(int i = 0 ; i < LotCount ; i++){
                    IniLotInfo.Load("LotList ", i.ToString()  , ref sLot  );
                    LotList.Add(sLot);
                }
            }
            else 
            {
                LotCount = LotList.Count ;
                IniLotInfo.Save("Total ", "LotCount" , LotCount );
                IniLotInfo.Save("Total ", "LotOpened", LotOpened);
                IniLotInfo.Save("Total ", "LotEnded" , LotEnded );    
                IniLotInfo.Save("Total ", "iWorkMgzCnt" , iWorkMgzCnt );
                IniLotInfo.Save("Total ", "CrntLotData" , CrntLotData );
                IniLotInfo.Save("Total ", "LastPopLot"  , LastPopLot  );

                for(int i = 0 ; i < LotCount ; i++){
                    sLot = LotList[i];
                    IniLotInfo.Save("LotList ", i.ToString()  , sLot  );                
                }
            }
            return true;
        }
        
        //Lot Processing.
        public static void LotOpen(string _sLot)//로더에서 프리버퍼로 공급할때 처리.
        {
            Log.Trace("SEQ",("Lot Open : " + _sLot ).ToString()  );
            LotOpened  = true ;
            //iWorkMgzCnt = 0;
            CrntLotData = _sLot ;
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
            Log.Trace("SEQ",CrntLotData + " Lot Finished"   );
           
            //Reset Lot Flag.
            CrntLotData = "";
            LotOpened   = false;
            LotEnded    = true ;
            iWorkMgzCnt = 0;

            //MM.SetManCycle(mc.LODR_ManLotSupply);
            //SEQ.LODR.iLDRSplyCnt = 0;
        }
        
        public static bool GetLotEnd ()
        {
            //Check already opened Lot.
            return LotEnded ;
        }

        public static string GetLotNo()
        {
            return CrntLotData;
        }

        //다음 메거진의 랏넘버를 빼어냄.
        static public string PopMgz()
        {

            if(iWorkMgzCnt == 0 || iWorkMgzCnt >= OM.DevInfo.iMgzCntPerLot)
            {
                if(LotList.Count != 0)
                {
                    LotOpened = true;
                    string sLotNo = LotList[0] ;

                    //처음엔 로더픽에서 하니 메거진마다 랏이 쪼게지고.
                    //두번째 랏오픈창에 Apply버튼에 넣을려니 버튼누를때 마다 언더바 숫자가 붙고.
                    //여기서 하는게 맞는듯.
                    string sNumberedLotNo = SPC.MAP.GetLotNo(sLotNo); //언더바 숫자 가져오기.
                    LastPopLot = sNumberedLotNo ;
                    LotList.RemoveAt(0);
                    iWorkMgzCnt = 1 ;
                    return LastPopLot ;
                }
                else
                {
                    return "" ;
                }                
            }
            else
            {
                iWorkMgzCnt++;
                return LastPopLot ;
            }
        }

        static public int GetWorkMgzCnt()
        {
            return iWorkMgzCnt ;
        }

        //다음 메거진의 랏넘버를 확인만 함.
        static public string GetNextMgz()
        {
            if(OM.DevInfo.iMgzCntPerLot == 1)
            {
                if(LotList.Count != 0) return LotList[0];
                else                   return ""    ;
            }
            else
            {
                if(iWorkMgzCnt == 0 || iWorkMgzCnt >= OM.DevInfo.iMgzCntPerLot)
                {
                    if(LotList.Count != 0) return LotList[0];
                    else                   return ""    ;
                }
                else
                {
                    return LastPopLot ;
                }
            }
        }

        //static public bool CheckLotChange()
        //{
        //    string sNextMgz = GetNextMgz() ;
        //    if(sNextMgz!="")
        //    {
        //        if(sNextMgz != 
        //    }
        //}
        //public static void RemoveAt(int i = 0)
        //{
        //    if(LotList.Count > 0) {
        //        if(i > 0) LotList.RemoveAt(0);
        //    }
        //}

    }
        
        
  







}
