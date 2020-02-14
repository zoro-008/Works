using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log
{
    public partial class LogOption : Form
    {
        LogMain LM;
        public LogOption(LogMain _LogMain)
        {
            InitializeComponent();
            this.TopMost = true;
            LM = _LogMain;
        }

        private void LogOption_Shown(object sender, EventArgs e)
        {
            UpdateOption(true);
        }

        public void UpdateOption(bool _bToTable)
        {
            int iRet;
            if (_bToTable == true)
            {
                tbLogPath.Text = OM.LogInfo.sLogPath;
                tbIgnrSameMsgLine.Text = OM.LogInfo.iIgnrSameMsgLine.ToString();
                tbSaveMaxDate.Text = OM.LogInfo.iSaveMaxMonths.ToString();
                textBoxCnt.Text = OM.LogInfo.iTagCnt.ToString();
                tbLogBuPath.Text = OM.LogInfo.sLogBuPath;

                textBox1.Text  = OM.GetTag( 1); CConfig.ValToCon(checkBox1,  ref OM.LogInfo.bWithAll1);
                textBox2.Text  = OM.GetTag( 2); CConfig.ValToCon(checkBox2,  ref OM.LogInfo.bWithAll2);
                textBox3.Text  = OM.GetTag( 3); CConfig.ValToCon(checkBox3,  ref OM.LogInfo.bWithAll3);
                textBox4.Text  = OM.GetTag( 4); CConfig.ValToCon(checkBox4,  ref OM.LogInfo.bWithAll4);
                textBox5.Text  = OM.GetTag( 5); CConfig.ValToCon(checkBox5,  ref OM.LogInfo.bWithAll5);
                textBox6.Text  = OM.GetTag( 6); CConfig.ValToCon(checkBox6,  ref OM.LogInfo.bWithAll6);
                textBox7.Text  = OM.GetTag( 7); CConfig.ValToCon(checkBox7,  ref OM.LogInfo.bWithAll7);
                textBox8.Text  = OM.GetTag( 8); CConfig.ValToCon(checkBox8,  ref OM.LogInfo.bWithAll8);
                textBox9.Text  = OM.GetTag( 9); CConfig.ValToCon(checkBox9,  ref OM.LogInfo.bWithAll9);
                textBox10.Text = OM.GetTag(10); CConfig.ValToCon(checkBox10, ref OM.LogInfo.bWithAll10);
                textBox11.Text = OM.GetTag(11); CConfig.ValToCon(checkBox11, ref OM.LogInfo.bWithAll11);
                textBox12.Text = OM.GetTag(12); CConfig.ValToCon(checkBox12, ref OM.LogInfo.bWithAll12);
                textBox13.Text = OM.GetTag(13); CConfig.ValToCon(checkBox13, ref OM.LogInfo.bWithAll13);
                textBox14.Text = OM.GetTag(14); CConfig.ValToCon(checkBox14, ref OM.LogInfo.bWithAll14);
                textBox15.Text = OM.GetTag(15); CConfig.ValToCon(checkBox15, ref OM.LogInfo.bWithAll15);
                //textBox16.Text = OM.LogInfo.sTag9; CConfig.ValToCon(checkBox16, ref OM.LogInfo.bWithAll16);
                
            }
            else
            {
                OM.LogInfo.sLogPath = tbLogPath.Text;
                OM.LogInfo.iIgnrSameMsgLine = int.TryParse(tbIgnrSameMsgLine.Text, out iRet) ? iRet : 0;
                OM.LogInfo.iSaveMaxMonths = int.TryParse(tbSaveMaxDate.Text, out iRet) ? iRet : 0;
                OM.LogInfo.iTagCnt = int.TryParse(textBoxCnt.Text, out iRet) ? iRet : 0;
                OM.LogInfo.sLogBuPath = tbLogBuPath.Text;

                OM.LogInfo.sTag1  = textBox1.Text;  CConfig.ConToVal(checkBox1,  ref OM.LogInfo.bWithAll1);
                OM.LogInfo.sTag2  = textBox2.Text;  CConfig.ConToVal(checkBox2,  ref OM.LogInfo.bWithAll2);
                OM.LogInfo.sTag3  = textBox3.Text;  CConfig.ConToVal(checkBox3,  ref OM.LogInfo.bWithAll3);
                OM.LogInfo.sTag4  = textBox4.Text;  CConfig.ConToVal(checkBox4,  ref OM.LogInfo.bWithAll4);
                OM.LogInfo.sTag5  = textBox5.Text;  CConfig.ConToVal(checkBox5,  ref OM.LogInfo.bWithAll5);
                OM.LogInfo.sTag6  = textBox6.Text;  CConfig.ConToVal(checkBox6,  ref OM.LogInfo.bWithAll6);
                OM.LogInfo.sTag7  = textBox7.Text;  CConfig.ConToVal(checkBox7,  ref OM.LogInfo.bWithAll7);
                OM.LogInfo.sTag8  = textBox8.Text;  CConfig.ConToVal(checkBox8,  ref OM.LogInfo.bWithAll8);
                OM.LogInfo.sTag9  = textBox9.Text;  CConfig.ConToVal(checkBox9,  ref OM.LogInfo.bWithAll9);
                OM.LogInfo.sTag10 = textBox10.Text; CConfig.ConToVal(checkBox10, ref OM.LogInfo.bWithAll10);
                OM.LogInfo.sTag11 = textBox11.Text; CConfig.ConToVal(checkBox11, ref OM.LogInfo.bWithAll11);
                OM.LogInfo.sTag12 = textBox12.Text; CConfig.ConToVal(checkBox12, ref OM.LogInfo.bWithAll12);
                OM.LogInfo.sTag13 = textBox13.Text; CConfig.ConToVal(checkBox13, ref OM.LogInfo.bWithAll13);
                OM.LogInfo.sTag14 = textBox14.Text; CConfig.ConToVal(checkBox14, ref OM.LogInfo.bWithAll14);
                OM.LogInfo.sTag15 = textBox15.Text; CConfig.ConToVal(checkBox15, ref OM.LogInfo.bWithAll15);
                //OM.LogInfo.sTag16 = textBox16.Text; CConfig.ConToVal(checkBox16, ref OM.LogInfo.bWithAll16);

                UpdateOption(true);
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            UpdateOption(false);
            OM.SaveLogInfo();

            LM.ListViewPageText();
            LM.ListViewDispInit(OM.LogInfo.iTagCnt);
        }

        private void tbLogPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void LogOption_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }

    //---------------------------------------------------------------------------
    public static class OM
    {
        public struct CLogInfo
        {
            public string sTag1;  public bool bWithAll1;
            public string sTag2;  public bool bWithAll2;
            public string sTag3;  public bool bWithAll3;
            public string sTag4;  public bool bWithAll4;
            public string sTag5;  public bool bWithAll5;
            public string sTag6;  public bool bWithAll6;
            public string sTag7;  public bool bWithAll7;
            public string sTag8;  public bool bWithAll8;
            public string sTag9;  public bool bWithAll9;
            public string sTag10; public bool bWithAll10;
            public string sTag11; public bool bWithAll11;
            public string sTag12; public bool bWithAll12;
            public string sTag13; public bool bWithAll13;
            public string sTag14; public bool bWithAll14;
            public string sTag15; public bool bWithAll15;
            //public string sTag16; public bool bWithAll16;

            //static public string[] sTag = new string[10];

            private String _sLogPath;
            public String sLogPath
            {
                get
                {
                    if (_sLogPath == "") { return @"d:\log"; }
                        return _sLogPath;
                }
                set
                {
                    _sLogPath = value; // System.Text.RegularExpressions.Regex.Replace(value, "[/*?\"<>|]", ""); 
                }
            }
            
            private String _sLogBuPath;
            public String sLogBuPath
            {
                get
                {
                    if (_sLogBuPath == "") { return System.AppDomain.CurrentDomain.BaseDirectory; }
                    return _sLogBuPath;
                }
                set
                {
                    _sLogBuPath = value;
                }
            }

            private int _iIgnrSameMsgLine;
            public int iIgnrSameMsgLine
            {
                get
                {
                    if (_iIgnrSameMsgLine < 0) _iIgnrSameMsgLine = 0;
                    if (_iIgnrSameMsgLine > 100) _iIgnrSameMsgLine = 100;
                    return _iIgnrSameMsgLine;
                }
                set
                {
                    _iIgnrSameMsgLine = value;
                }
            }

            public int _iSaveMaxMonths;
            public int iSaveMaxMonths
            {
                get
                {
                    if (_iSaveMaxMonths < 1 ) _iSaveMaxMonths = 1 ;
                    if (_iSaveMaxMonths > 25) _iSaveMaxMonths = 24;
                    return _iSaveMaxMonths;
                }
                set
                {
                    _iSaveMaxMonths = value;
                }
            }

            private int _iTagCnt;
            public int iTagCnt
            {
                get {
                    if (_iTagCnt < 0) _iTagCnt = 0;
                    if (_iTagCnt > 15) _iTagCnt = 15;
                    return _iTagCnt;
                }
                set
                {
                    _iTagCnt = value;
                }
            }
        }

        public static CLogInfo LogInfo;
        //public static sTag Tag;

        public static void SaveLogInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sInfoPath = sExeFolder + @"Log\LogInfo.ini";
            //string sInfoPath = LogInfo.sLogPath + "Option\\LogInfo.ini";
            CAutoIniFile.SaveStruct<CLogInfo>(sInfoPath, "LogInfo", ref LogInfo);
        }

        public static void LoadLogInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sInfoPath = sExeFolder + @"Log\LogInfo.ini";
            //string sInfoPath = LogInfo.sLogPath + "Option\\LogInfo.ini";
            CAutoIniFile.LoadStruct<CLogInfo>(sInfoPath, "LogInfo", ref LogInfo);
        }

        static OM()
        {
            LoadLogInfo();
        }

        public static string GetTag(int _iNo)
        {
            string sRst;

            if (_iNo == 1) { sRst = LogInfo.sTag1; if (sRst == "") sRst = "#1"; }
            else if (_iNo == 2 ) { sRst = LogInfo.sTag2;  if (sRst == "") sRst = "#2"; }
            else if (_iNo == 3 ) { sRst = LogInfo.sTag3;  if (sRst == "") sRst = "#3"; }
            else if (_iNo == 4 ) { sRst = LogInfo.sTag4;  if (sRst == "") sRst = "#4"; }
            else if (_iNo == 5 ) { sRst = LogInfo.sTag5;  if (sRst == "") sRst = "#5"; }
            else if (_iNo == 6 ) { sRst = LogInfo.sTag6;  if (sRst == "") sRst = "#6"; }
            else if (_iNo == 7 ) { sRst = LogInfo.sTag7;  if (sRst == "") sRst = "#7"; }
            else if (_iNo == 8 ) { sRst = LogInfo.sTag8;  if (sRst == "") sRst = "#8"; }
            else if (_iNo == 9 ) { sRst = LogInfo.sTag9;  if (sRst == "") sRst = "#9"; }
            else if (_iNo == 10) { sRst = LogInfo.sTag10; if (sRst == "") sRst = "#A"; }
            else if (_iNo == 11) { sRst = LogInfo.sTag11; if (sRst == "") sRst = "#B"; }
            else if (_iNo == 12) { sRst = LogInfo.sTag12; if (sRst == "") sRst = "#C"; }
            else if (_iNo == 13) { sRst = LogInfo.sTag13; if (sRst == "") sRst = "#D"; }
            else if (_iNo == 14) { sRst = LogInfo.sTag14; if (sRst == "") sRst = "#E"; }
            else if (_iNo == 15) { sRst = LogInfo.sTag15; if (sRst == "") sRst = "#F"; }
            else { sRst = ""; }

            return sRst;
        }

        //public static bool CheckWithAll()
        //{
        //    bool bRst = false;
        //    for (int i = 0; i < 10; i++)
        //    {
        //        bRst |= GetWithAll(i);
        //    }
        //    return bRst;
        //}

        public static bool GetWithAll(int _iNo)
        {
            bool bRst = false;

                 if (_iNo == 1 ) { bRst = LogInfo.bWithAll1; }
            else if (_iNo == 2 ) { bRst = LogInfo.bWithAll2; }
            else if (_iNo == 3 ) { bRst = LogInfo.bWithAll3; }
            else if (_iNo == 4 ) { bRst = LogInfo.bWithAll4; }
            else if (_iNo == 5 ) { bRst = LogInfo.bWithAll5; }
            else if (_iNo == 6 ) { bRst = LogInfo.bWithAll6; }
            else if (_iNo == 7 ) { bRst = LogInfo.bWithAll7; }
            else if (_iNo == 8 ) { bRst = LogInfo.bWithAll8; }
            else if (_iNo == 9 ) { bRst = LogInfo.bWithAll9; }
            else if (_iNo == 10) { bRst = LogInfo.bWithAll10; }
            else if (_iNo == 11) { bRst = LogInfo.bWithAll11; }
            else if (_iNo == 12) { bRst = LogInfo.bWithAll12; }
            else if (_iNo == 13) { bRst = LogInfo.bWithAll13; }
            else if (_iNo == 14) { bRst = LogInfo.bWithAll14; }
            else if (_iNo == 15) { bRst = LogInfo.bWithAll15; }
            else { bRst = false; }

            return bRst;
        }
    }
}
