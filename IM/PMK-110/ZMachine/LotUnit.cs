using COMMON;

namespace Machine
{
    public class LOT
    {
        public static string m_sLotNo     ;
        public static string m_sJobName;

        public static bool m_bLotOpen;
        public static bool m_bLotEnded;

        public static bool m_bRqstLotEnd; //랏엔드 요청

        public static void Init()
        {
            m_sLotNo      = ""   ;
            m_sJobName    = ""   ;
            Log.Trace("SEQ", "Lot Init");
            m_bLotOpen    = false;
            m_bLotEnded   = false;
            m_bRqstLotEnd = false;
        
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

    
                    
            //Current Lot Informations.
            if(_bLoad) 
            {
                CIniFile IniLotInfo = new CIniFile(sLotInfo);

                Log.Trace("SEQ","Lot LoadSave"   );
                IniLotInfo.Load("Member ", "m_bLotOpen    ", ref m_bLotOpen);
                IniLotInfo.Load("Member ", "m_bLotEnded   ", ref m_bLotEnded);
                IniLotInfo.Load("Member ", "m_bRqstLotEnd ", ref m_bRqstLotEnd);
                IniLotInfo.Load("Member ", "m_sLotNo      ", ref m_sLotNo);
                IniLotInfo.Load("Member ", "m_sJobName    ", ref m_sJobName);
            }
            else 
            {
                CIniFile IniLotInfo = new CIniFile(sLotInfo);

                IniLotInfo.Save("Member ", "m_bLotOpen    ", m_bLotOpen);
                IniLotInfo.Save("Member ", "m_bLotEnded   ", m_bLotEnded);
                IniLotInfo.Save("Member ", "m_bRqstLotEnd ", m_bRqstLotEnd);
                IniLotInfo.Save("Member ", "m_sLotNo      ", m_sLotNo);
                IniLotInfo.Save("Member ", "m_sJobName    ", m_sJobName);
            }
            return true;

        }
        
        //Lot Processing.
        public static void LotOpen(string _sLotNo , string _sJobName)
        {
            Log.Trace("SEQ",("Lot Open : " + _sLotNo).ToString()  );
            m_bLotOpen  = true ;
        
            m_sLotNo      = _sLotNo       ;
            m_sJobName    = _sJobName     ;
        }
        
        public static bool GetLotOpen()
        {
            return m_bLotOpen ;
        }
        
        public static void Reset()
        {
            m_bLotEnded = false ;
        
        }
        
        public static void LotEnd()
        {
            //Check already opened Lot.
            Log.Trace("SEQ","Lot Finished"   );
            //if (!m_bLotOpen) return;
            //Trace("SEQ","Lot Finished2"   );
        
            //Reset Lot Flag.
            m_bLotOpen    = false;
            m_bRqstLotEnd = false ;
            m_bLotEnded   = true ;

            //MessageBox.Show("LOT IS FINISHED", "Confirm");
        
        }
        
        public static bool GetLotEnd ()
        {
            //Check already opened Lot.
            return m_bLotEnded ;
        
        }

        public static string GetLotNo()
        {
            return m_sLotNo; 
        }
    }
        
        
  







}
