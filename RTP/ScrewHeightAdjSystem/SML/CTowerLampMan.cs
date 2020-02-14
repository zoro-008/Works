using COMMON;

namespace SML
{
    public class CTowerLampMan
    {
        public CTowerLampMan() { }

        public enum EN_LAMP_OPER : uint
        {
            //loLampOff   = 0 ,
            //loLampOn        ,
            //loLampFlick     ,
            
            ON = 0,
            OFF,
            FLICK,
        
            MAX_LAMP_OPER
        };

        public struct TLampInfo 
        {
            public EN_LAMP_OPER  iRed    ;
            public EN_LAMP_OPER  iYel    ;
            public EN_LAMP_OPER  iGrn    ;
            public EN_LAMP_OPER  iBuzz   ;
        };

        public struct TStat
        {
            public bool bRedOn;
            public bool bYelOn;
            public bool bGrnOn;
            public bool bBuzOn;
            
        }
        
        public struct TPara 
        {
            public int  iRedAdd ;
            public int  iYelAdd ;
            public int  iGrnAdd ;
            public int  iSndAdd ;
            public TLampInfo[] LampInfo;
            public bool bUse    ;
        };

        public TPara  Para;
        TStat         Stat;

        //TPara[] m_aAdd = null;
        
        EN_LAN_SEL    m_eLangSel = EN_LAN_SEL.English; //Languge Selection.
        string        m_sParaFolderPath;
        bool          m_bFlick;

        bool          m_bTestMode;
        bool          m_bTestLampBuss;
        EN_SEQ_STAT   m_iSeqStat;
        EN_SEQ_STAT   m_eTestStat;

        CDioMan       DIO;

        CDelayTimer   FlickTimer = new CDelayTimer();

        //public int _ListViewCnt = 8;

        bool          m_bBuzzOff;
        //bool          m_bUse    ;
        public bool Init(EN_LAN_SEL _eLanSel,string _sParaFolderPath,CDioMan _Dio,bool _bUse = true)
        {
            m_eLangSel = _eLanSel ;
            m_sParaFolderPath = _sParaFolderPath;
            DIO    = _Dio;
            //m_bUse = _bUse;
            Para.LampInfo = new TLampInfo[(int)EN_SEQ_STAT.MAX_SEQ_STAT];

            //m_aAdd = new TPara[(int)EN_SEQ_STAT.MAX_SEQ_STAT];

            LoadSave(true);
            if(!_bUse) Para.bUse = _bUse;

            return true;

            
        }

        public bool Close()
        {
            return true;
        }

        public void Update(EN_SEQ_STAT _eStat)
        {
            //m_iSeqStat = _eStat;

            //Local Var.
            EN_SEQ_STAT eStat = m_bTestLampBuss ? m_eTestStat : _eStat;
            int         iStat = (int)eStat;

            if(FlickTimer.OnDelay(500))
            {
                m_bFlick = !m_bFlick;
                FlickTimer.Clear();
            }

            if (DIO.GetTestMode()) return;
            if (!Para.bUse) return;


            
            //switch(Para.LampInfo[iStat].iRed)
            //{
            //    case EN_LAMP_OPER.loLampOff  : DIO.SetY(Para.iRedAdd,false   ); break;
            //    case EN_LAMP_OPER.loLampOn   : DIO.SetY(Para.iRedAdd,true    ); break;
            //    case EN_LAMP_OPER.loLampFlick: DIO.SetY(Para.iRedAdd,m_bFlick); break;
            //}
            //
            //switch(Para.LampInfo[iStat].iGrn)
            //{
            //    case EN_LAMP_OPER.loLampOff  : DIO.SetY(Para.iGrnAdd,false   ); break;
            //    case EN_LAMP_OPER.loLampOn   : DIO.SetY(Para.iGrnAdd,true    ); break;
            //    case EN_LAMP_OPER.loLampFlick: DIO.SetY(Para.iGrnAdd,m_bFlick); break;
            //}
            //
            //switch(Para.LampInfo[iStat].iYel)
            //{
            //    case EN_LAMP_OPER.loLampOff  : DIO.SetY(Para.iRedAdd,false   ); break;
            //    case EN_LAMP_OPER.loLampOn   : DIO.SetY(Para.iRedAdd,true    ); break;
            //    case EN_LAMP_OPER.loLampFlick: DIO.SetY(Para.iRedAdd,m_bFlick); break;
            //}
            //
            //switch(Para.LampInfo[iStat].iBuzz)
            //{
            //    case EN_LAMP_OPER.loLampOff  : DIO.SetY(Para.iSndAdd,false   ); break;
            //    case EN_LAMP_OPER.loLampOn   : DIO.SetY(Para.iSndAdd,true    ); break;
            //    case EN_LAMP_OPER.loLampFlick: DIO.SetY(Para.iSndAdd,m_bFlick); break;
            //}

            switch (Para.LampInfo[iStat].iRed)
            {
                case EN_LAMP_OPER.OFF  : if(Para.iRedAdd != -1) {DIO.SetY(Para.iRedAdd, false   );} break;
                case EN_LAMP_OPER.ON   : if(Para.iRedAdd != -1) {DIO.SetY(Para.iRedAdd, true    );} break;
                case EN_LAMP_OPER.FLICK: if(Para.iRedAdd != -1) {DIO.SetY(Para.iRedAdd, m_bFlick);} break;
            }

            switch (Para.LampInfo[iStat].iGrn)
            {
                case EN_LAMP_OPER.OFF  : if(Para.iGrnAdd != -1) {DIO.SetY(Para.iGrnAdd, false   );} break;
                case EN_LAMP_OPER.ON   : if(Para.iGrnAdd != -1) {DIO.SetY(Para.iGrnAdd, true    );} break;
                case EN_LAMP_OPER.FLICK: if(Para.iGrnAdd != -1) {DIO.SetY(Para.iGrnAdd, m_bFlick);} break;
            }                                                                                     
                                                                                                  
            switch (Para.LampInfo[iStat].iYel)                                                    
            {                                                                                     
                case EN_LAMP_OPER.OFF  : if(Para.iYelAdd != -1) {DIO.SetY(Para.iYelAdd, false   );} break;
                case EN_LAMP_OPER.ON   : if(Para.iYelAdd != -1) {DIO.SetY(Para.iYelAdd, true    );} break;
                case EN_LAMP_OPER.FLICK: if(Para.iYelAdd != -1) {DIO.SetY(Para.iYelAdd, m_bFlick);} break;
            }                                                                                     
                                                                                                  
            if(!m_bBuzzOff){                                               
                switch (Para.LampInfo[iStat].iBuzz)                                                   
                {                                                                                     
                    case EN_LAMP_OPER.OFF  : if(Para.iSndAdd != -1) {DIO.SetY(Para.iSndAdd, false   );} break;
                    case EN_LAMP_OPER.ON   : if(Para.iSndAdd != -1) {DIO.SetY(Para.iSndAdd, true    );} break;
                    case EN_LAMP_OPER.FLICK: if(Para.iSndAdd != -1) {DIO.SetY(Para.iSndAdd, m_bFlick);} break;
                }
            }
            else {
                DIO.SetY( Para.iSndAdd, false   );
            }

        }

        public TLampInfo GetTLampInfo(EN_SEQ_STAT _eStat)
        {
            return Para.LampInfo[(int)_eStat];
        }

        public TPara GetTLampPara()
        {
            //return m_aAdd[(int)_eStat];
            return Para;

        }

        public void SetTLampInfo(EN_SEQ_STAT _eStat, TLampInfo _tLampInfo)
        {
            Para.LampInfo[(int)_eStat] = _tLampInfo;
        }

        public void SetTLampPara(TPara _tLampPara)
        {
            //m_aAdd[(int)_eStat] = _tLampPara;
            Para.iRedAdd = _tLampPara.iRedAdd;
            Para.iYelAdd = _tLampPara.iYelAdd;
            Para.iGrnAdd = _tLampPara.iGrnAdd;
            Para.iSndAdd = _tLampPara.iSndAdd;
            Para.bUse    = _tLampPara.bUse   ;

        }

        public void SetTestMode (bool IsTest) 
        { 
            m_bTestMode = IsTest; 
        }

        public bool LoadSave(bool _bLoad)
        {              
            CConfig Config = new CConfig();

            string sIndex ;
            string sPath  ;
            int    iTemp  ;

            //string sSelLan;
            //switch (m_eLangSel)
            //{
            //    default: sSelLan = "_E"; break;
            //    case EN_LAN_SEL.English: sSelLan = "_E"; break;
            //    case EN_LAN_SEL.Korean : sSelLan = "_K"; break;
            //    case EN_LAN_SEL.Chinese: sSelLan = "_C"; break;
            //}
            //sPath = m_sParaFolderPath + "TowerLamp"  + sSelLan + ".ini";
            sPath = m_sParaFolderPath + "TowerLamp.ini";

            if (_bLoad)
            {
                Config.Load(sPath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
                for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
                {
                    sIndex = string.Format("TOWER_LAMP({0:0000})", i);
                    
                    Config.GetValue(sIndex, "iRed ", out iTemp); Para.LampInfo[i].iRed  = (EN_LAMP_OPER)iTemp;
                    Config.GetValue(sIndex, "iYel ", out iTemp); Para.LampInfo[i].iYel  = (EN_LAMP_OPER)iTemp;
                    Config.GetValue(sIndex, "iGrn ", out iTemp); Para.LampInfo[i].iGrn  = (EN_LAMP_OPER)iTemp;
                    Config.GetValue(sIndex, "iBuzz", out iTemp); Para.LampInfo[i].iBuzz = (EN_LAMP_OPER)iTemp;
                }

                sIndex = "m_aAdd";
                Config.GetValue(sIndex, "iRedAdd", out Para.iRedAdd);
                Config.GetValue(sIndex, "iYelAdd", out Para.iYelAdd);
                Config.GetValue(sIndex, "iGrnAdd", out Para.iGrnAdd);
                Config.GetValue(sIndex, "iSndAdd", out Para.iSndAdd);
                Config.GetValue(sIndex, "bUse   ", out Para.bUse   );

            }
            else
            {
                for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
                {
                    sIndex = string.Format("TOWER_LAMP({0:0000})", i);
                    Config.SetValue(sIndex, "iRed ", (int)Para.LampInfo[i].iRed );
                    Config.SetValue(sIndex, "iYel ", (int)Para.LampInfo[i].iYel );
                    Config.SetValue(sIndex, "iGrn ", (int)Para.LampInfo[i].iGrn );
                    Config.SetValue(sIndex, "iBuzz", (int)Para.LampInfo[i].iBuzz);
                }

                sIndex = "m_aAdd";
                Config.SetValue(sIndex, "iRedAdd", Para.iRedAdd);
                Config.SetValue(sIndex, "iYelAdd", Para.iYelAdd);
                Config.SetValue(sIndex, "iGrnAdd", Para.iGrnAdd);
                Config.SetValue(sIndex, "iSndAdd", Para.iSndAdd);
                Config.SetValue(sIndex, "bUse   ", Para.bUse   );

                Config.Save(sPath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            }
            /*
            if (_bLoad)
            {
                CIniFile TLampConfig = new CIniFile(m_sParaFolderPath + "TowerLamp.ini");


                int iTemp;
                
                for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
                {
                    sIndex = string.Format("TOWER_LAMP({0:0000})", i);
                    
                    TLampConfig.Load("iRed ", sIndex, out iTemp); Para.LampInfo[i].iRed  = (EN_LAMP_OPER)iTemp;
                    TLampConfig.Load("iYel ", sIndex, out iTemp); Para.LampInfo[i].iYel  = (EN_LAMP_OPER)iTemp;
                    TLampConfig.Load("iGrn ", sIndex, out iTemp); Para.LampInfo[i].iGrn  = (EN_LAMP_OPER)iTemp;
                    TLampConfig.Load("iBuzz", sIndex, out iTemp); Para.LampInfo[i].iBuzz = (EN_LAMP_OPER)iTemp;
                }

                sIndex = "m_aAdd";
                TLampConfig.Load(sIndex, "iRedAdd", out Para.iRedAdd);
                TLampConfig.Load(sIndex, "iYelAdd", out Para.iYelAdd);
                TLampConfig.Load(sIndex, "iGrnAdd", out Para.iGrnAdd);
                TLampConfig.Load(sIndex, "iSndAdd", out Para.iSndAdd);

            }
            else
            {
                CIniFile TLampConfig = new CIniFile(m_sParaFolderPath + "TowerLamp.ini");

                for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
                {
                    sIndex = string.Format("TOWER_LAMP({0:0000})", i);
                    TLampConfig.Save("iRed ", sIndex, (int)Para.LampInfo[i].iRed);
                    TLampConfig.Save("iYel ", sIndex, (int)Para.LampInfo[i].iYel);
                    TLampConfig.Save("iGrn ", sIndex, (int)Para.LampInfo[i].iGrn);
                    TLampConfig.Save("iBuzz", sIndex, (int)Para.LampInfo[i].iBuzz);
                }

                sIndex = "m_aAdd";
                TLampConfig.Save(sIndex, "iRedAdd", Para.iRedAdd);
                TLampConfig.Save(sIndex, "iYelAdd", Para.iYelAdd);
                TLampConfig.Save(sIndex, "iGrnAdd", Para.iGrnAdd);
                TLampConfig.Save(sIndex, "iSndAdd", Para.iSndAdd);
            }
            */
            return true;
        }

        public void SetBuzzOff(bool _bValue)
        {
            m_bBuzzOff = _bValue ;
        }
    }
}
