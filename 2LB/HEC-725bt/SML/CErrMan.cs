using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Drawing;
using COMMON;

namespace SML
{

    public class CErrMan
    {
        public FormErr FrmErr;

        /// <summary>
        /// 에러에 관한 디스크로딩값.
        /// </summary>
        public struct TPara
        {
            public string sEnum;
            public string sName;
            public string sAction;
            public string sImgPath;
            public string sErrMsg;

            public double dRectLeft;
            public double dRectTop;
            public double dRectWidth;
            public double dRectHeight;

            public int    iErrorLevel;
        }
        

        /// <summary>
        /// 에러에 관한 현재 상황.
        /// 저장값이 아닌 Update 갱신값 과 에러띄울때 SubMsg사용값이 세팅됨.
        /// </summary>
        public struct TStat
        {
            public string sSubMsg;
            public bool bOn;
            public bool bOnUp;
            public bool bOnDn;

            public bool bPreOn;
        }

        public struct TErr
        {
            public TPara Para;
            public TStat Stat;
        }

        TErr[] m_aErr = null;


        bool         m_bUseErrPic ; //true 일때만 사진 띄어준다.
        EN_LAN_SEL   m_eLangSel   ; //Languge Selection.
        string       m_sParaFolderPath ;
       
        public int          m_iMaxErr    ; //총에러 개수.
        int          m_iLastErr   ; //가장 마지막에 온된 에러.
        int          m_iErrOnCnt  ; //현재 온되어 있는 에러 갯수.
        bool         m_bNeedShowForm = true ; //true일때만 에러창을 띄운다.

        public int _iMaxErrCnt { get { return m_iMaxErr; } }


        public FormErr m_FrmErr;

        /// <summary>
        /// 에러리스트 클래스 생성자 입니다.
        /// </summary>
        /// <param name="_iMaxErrCnt">에러카운트의 갯수 입니다.</param>
        /// 
        public bool Init(EN_LAN_SEL _eLanSel,string _sParaFolderPath,int _iMaxErrCnt,bool _bUseErrPic)
        {
            //LoadSavePara(false);
            m_eLangSel = _eLanSel;
            m_sParaFolderPath = _sParaFolderPath;
            m_bUseErrPic = _bUseErrPic;

            m_FrmErr = new FormErr();

            

            m_aErr = new TErr[_iMaxErrCnt];

            m_iMaxErr = _iMaxErrCnt; 
            for (int i = 0; i < _iMaxErrCnt; i++)
            {
                //m_aErr[i].Para = new TErrConfig();
                m_aErr[i].Para.sEnum = i.ToString();
                m_aErr[i].Para.sName = i.ToString();
                m_aErr[i].Para.sAction = i.ToString();
                m_aErr[i].Para.sImgPath = i.ToString();

                m_aErr[i].Para.dRectLeft   = 0;
                m_aErr[i].Para.dRectTop    = 0;
                m_aErr[i].Para.dRectHeight = 0;
                m_aErr[i].Para.dRectWidth  = 0;

                m_aErr[i].Para.iErrorLevel = 0;

                m_aErr[i].Stat.sSubMsg = "";
                m_aErr[i].Stat.bOn   = false;
                m_aErr[i].Stat.bOnUp = false;
                m_aErr[i].Stat.bOnDn = false;
                m_aErr[i].Stat.bPreOn= false;

            }
            LoadSave(true);
            return true;
        }

        public bool Close()
        {
            return true;
        }

        public void Update()
        {
            
            for (int i = 0; i < m_iMaxErr; i++)
            {
                if (m_aErr[i].Stat.bOn != m_aErr[i].Stat.bPreOn)
                {
                    m_aErr[i].Stat.bOnDn = !m_aErr[i].Stat.bOn;
                    m_aErr[i].Stat.bOnUp =  m_aErr[i].Stat.bOn;

                }
                else
                {
                    m_aErr[i].Stat.bOnDn = false;
                    m_aErr[i].Stat.bOnUp = false;
                }
                    
            }
        }

        

        public void Clear()
        {
           
            for (int i = 0; i < m_iMaxErr; i++) m_aErr[i].Stat.bOn = false;

            m_FrmErr.HideErr();
            //HideErr();

        }

        public void SetErr(int _iNo , string _sMsg="",[CallerMemberName] string memberName = "",[CallerLineNumber] int sourceLineNumber = 0)
        {
            if (_iNo < 0 || _iNo >= m_iMaxErr) Log.ShowMessageFunc(string.Format("{0} is not in between 0 and MaxErr:{1}", _iNo, m_iMaxErr));
            if (m_aErr[_iNo].Stat.bOn) return;
            m_iLastErr = _iNo;

            m_aErr[_iNo].Stat.sSubMsg = _sMsg;
            m_aErr[_iNo].Stat.bOn = true;

            if (m_bNeedShowForm)
            {
                m_FrmErr.ShowErr(_iNo);
            }

            

            
            //m_iLastErr = _iNo;
            //
            //if (!m_bShowForm)
            //{
            //    m_FrmErr.ShowErr(_iNo);
            //}
           
            Log.Trace(String.Format("ERROR({0:000})", _iNo), String.Format("FUNC : {0} LINE : {1}", memberName, sourceLineNumber), ti.Sts);
        }

        public bool GetErr(int _iNo)
        {
            return m_aErr[_iNo].Stat.bOn;
        }

        public void SetNeedShowErr(bool _bOn)
        {
            m_bNeedShowForm = _bOn;
        }
        public TStat GetErrStat(int _iNo)
        {
            if(_iNo < 0 || _iNo >= m_iMaxErr) Log.ShowMessageFunc(string.Format("ERR:{0} is not in between 0 and MaxErr:{1}",_iNo,m_iMaxErr));
            return m_aErr[_iNo].Stat;
        }
        public TPara GetErrConfig(int _iNo)
        {
            if(_iNo < 0 || _iNo >= m_iMaxErr) Log.ShowMessageFunc(string.Format("ERR:{0} is not in between 0 and MaxErr:{1}",_iNo,m_iMaxErr));
            return m_aErr[_iNo].Para;
        }
        public void SetErrConfig(int _iNo,TPara _tErrConfig)
        {
            if(_iNo < 0 || _iNo >= m_iMaxErr) Log.ShowMessageFunc(string.Format("ERR:{0} is not in between 0 and MaxErr:{1}",_iNo,m_iMaxErr));

            m_aErr[_iNo].Para = _tErrConfig;
        }
        public bool GetErrOn(int _iNo)
        {
            if(_iNo < 0 || _iNo >= m_iMaxErr) Log.ShowMessageFunc(string.Format("ERR:{0} is not in between 0 and MaxErr:{1}",_iNo,m_iMaxErr));
            return m_aErr[_iNo].Stat.bOn;
        }
        public string GetErrSubMsg(int _iNo)
        {
            if(_iNo < 0 || _iNo >= m_iMaxErr) Log.ShowMessageFunc(string.Format("ERR:{0} is not in between 0 and MaxErr:{1}",_iNo,m_iMaxErr));
            return m_aErr[_iNo].Stat.sSubMsg;
        }
        
        //진섭 추가
        public int GetLastErr() //에러 넘버
        {
            return m_iLastErr;
        }

        public string GetErrName(int _iNo)
        {
            return m_aErr[_iNo].Para.sName;
        }

        public void SetErrMsg(int _iNo , string _sErrMsg)
        {
            if(_iNo < 0 || _iNo >= m_iMaxErr) {Log.ShowMessage("Error", "Err Range is Over"); return ; }
        
            string sErrNo  = "ERROR_"+ _iNo.ToString() ;
        
            if (m_aErr[_iNo].Stat.bOn) return;

            m_iLastErr = _iNo;

            if (m_bNeedShowForm /* || (_iNo != m_iLastErr)*/)
            {
                m_FrmErr.ShowErr(_iNo); //FrmErr -> m_FrmErr로 바꿈
            }           
        
            m_aErr[_iNo].Para.sErrMsg = _sErrMsg;
            m_aErr[_iNo].Stat.bOn = true ;        
            Log.Trace(sErrNo.ToString(),(m_aErr[_iNo].Para.sName +" Msg:"+ _sErrMsg).ToString());
        }

        public string GetErrMsg(int _iNo)
        {
            return m_aErr[_iNo].Para.sErrMsg;
        }

        public bool IsErr()
        {
            for (int i = 0; i < m_iMaxErr; i++) if (m_aErr[i].Stat.bOn) return true;

            return false;
        }

        public EN_ERR_LEVEL GetErrLevel(int _iNo)
        {
            return (EN_ERR_LEVEL) m_aErr[_iNo].Para.iErrorLevel;
        }


        public bool LoadSave(bool _bLoad)
        {
            CConfig Config = new CConfig();

            string sSelLan;
            string sErrNo;


            switch(m_eLangSel)
            {
                default: sSelLan = "E_"; break;
                case EN_LAN_SEL.English: sSelLan = "E_"; break;
                case EN_LAN_SEL.Korean: sSelLan = "K_"; break;
                case EN_LAN_SEL.Chinese: sSelLan = "C_"; break;
            }


            if(_bLoad)
            {
                CIniFile ErrorConfig = new CIniFile(m_sParaFolderPath + "Error.ini");

                for (int i = 0; i < m_iMaxErr; i++)
                {
                    sErrNo = string.Format("ERR({0:000})", i);
                    ErrorConfig.Load("sEnum"                , sErrNo, out m_aErr[i].Para.sEnum);
                    ErrorConfig.Load(sSelLan + "sName"      , sErrNo, out m_aErr[i].Para.sName);
                    ErrorConfig.Load(sSelLan + "sAction"    , sErrNo, out m_aErr[i].Para.sAction);
                    ErrorConfig.Load("sImgPath"             , sErrNo, out m_aErr[i].Para.sImgPath);


                    ErrorConfig.Load("dRectLeft  ", sErrNo, out  m_aErr[i].Para.dRectLeft);
                    ErrorConfig.Load("dRectTop   ", sErrNo, out  m_aErr[i].Para.dRectTop);
                    ErrorConfig.Load("dRectWidth ", sErrNo, out  m_aErr[i].Para.dRectWidth);
                    ErrorConfig.Load("dRectHeight", sErrNo, out  m_aErr[i].Para.dRectHeight);

                    ErrorConfig.Load("iErrorLevel", sErrNo, out  m_aErr[i].Para.iErrorLevel);
                }

            }
            else
            {
                CIniFile ErrorConfig = new CIniFile(m_sParaFolderPath + "Error.ini");

                for (int i = 0; i < m_iMaxErr; i++)
                {
                    sErrNo = string.Format("ERR({0:000})", i);
                    ErrorConfig.Save("sEnum"            , sErrNo, m_aErr[i].Para.sEnum);
                    ErrorConfig.Save(sSelLan + "sName"  , sErrNo, m_aErr[i].Para.sName);
                    ErrorConfig.Save(sSelLan + "sAction", sErrNo, m_aErr[i].Para.sAction);
                    ErrorConfig.Save("sImgPath"         , sErrNo, m_aErr[i].Para.sImgPath);


                    ErrorConfig.Save("dRectLeft  ", sErrNo, m_aErr[i].Para.dRectLeft);
                    ErrorConfig.Save("dRectTop   ", sErrNo, m_aErr[i].Para.dRectTop);
                    ErrorConfig.Save("dRectWidth ", sErrNo, m_aErr[i].Para.dRectWidth);
                    ErrorConfig.Save("dRectHeight", sErrNo, m_aErr[i].Para.dRectHeight);

                    ErrorConfig.Save("iErrorLevel", sErrNo, m_aErr[i].Para.iErrorLevel);
                }
            }
            return true;
        }
        
        public FileInfo fi;
        public void DisplayErrForm(TextBox _tbName, TextBox _tbEnum, TextBox _tbAction, TextBox _tbErrMsg, PictureBox _pbErr)
        {
            //Local Var.
            string     Temp,Str  ;
            int        iRect     ;
            int        Cnt,iCount;
            char       CChar     ;
            int        iErrNo    ;
            int        iPicWidth ;
            int        iPicHeight;
        
            iPicWidth  = _pbErr.Width  ;
            iPicHeight = _pbErr.Height ;

            iErrNo = GetLastErr() ;
            
            
            if (m_aErr[iErrNo].Para.sImgPath != "") { fi = new FileInfo(m_aErr[iErrNo].Para.sImgPath); }
        
            //Set Caption.
            Temp = string.Format("ERR{0:000}", iErrNo );
            //_tbErrNo . Text = Temp;
            
            _tbName  . Text = m_aErr[iErrNo].Para.sName;
            _tbEnum  . Text = m_aErr[iErrNo].Para.sEnum;
            _tbAction. Text = m_aErr[iErrNo].Para.sAction;
            _tbErrMsg. Text = m_aErr[iErrNo].Stat.sSubMsg ;
        
            //Display Solution
            Temp = m_aErr[iErrNo].Para.sAction;
            Cnt = 0;
            CChar = '*';
            //for(register int i = 1 ; i <= Temp.Length() ; i++) if (Temp[i] == CChar) iCount++;
            //Cnt = iCount;
           
            _tbAction.Text = Temp; 

            Temp = m_aErr[iErrNo].Para.sImgPath ;
            if (m_aErr[iErrNo].Para.sImgPath == "" || !fi.Exists) {
                _pbErr.Visible = false ;
            }
            else if(fi.Exists) {
                _pbErr.Visible  = true ;
                _pbErr.Image    = Image.FromFile(Temp);
            }


            //m_aErr[iErrNo].Para.sErrMsg = "";
        }

    }

}
