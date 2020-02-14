using COMMON;
using MotionLink;
using SML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormOperation : Form
    {
        [DllImport("user32")]
        public static extern Int32 GetCursorPos(out Point pt);

        //public static FormOperation FrmOperation;
        //public static FormMain      FrmMain     ;
        public FormVision    FrmVision   ;

        public        FraMotr          []  FraMotr      ;
        public        FrameCylinderAPT []  FraCylAPT    ;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 6;        

        //Output 0~5
        //FrameOutputAPT[] FraOutputAPT;
        //private string sFormText ;
        private const string sFormText = "Form Operation ";

        //Stack<string> UndoListRun     = new Stack<string>();
        //Stack<string> UndoListCool    = new Stack<string>();
        //Stack<string> UndoListExample = new Stack<string>();
        //Stack<string> UndoListTest    = new Stack<string>();

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();
            this.TopLevel = false;
            this.Parent = _pnBase;

            FrmVision = new FormVision();
            FrmVision.TopLevel = false ;
            FrmVision.Parent = pnCam ;            
            FrmVision.Dock = DockStyle.Fill ;
            FrmVision.Show();

            FraMotr = new FraMotr[(int)mi.MAX_MOTR];
            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                Control[] Ctrl = pnMtr.Controls.Find("pnMotrJog" +  m.ToString(), true);

                MOTION_DIR eDir = ML.MT_GetDirType((mi)m);
                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, eDir);
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            }

            //여기 AP텍에서만 쓰는거
            //실린더 버튼 AP텍꺼
            FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] CtrlAP = pnCyl.Controls.Find("C" + i.ToString(), true);

                FraCylAPT[i] = new FrameCylinderAPT();
                FraCylAPT[i].TopLevel = false;
                FraCylAPT[i].SetConfig((ci)i, ML.CL_GetName(i).ToString(), ML.CL_GetDirType((ci)i), CtrlAP[0]);
                FraCylAPT[i].Show();
            }



            //현재 스크립트 로드.
            LoadScript();

            //lbAutoComp
            lbAutoComp.Parent = this;
            lbAutoComp.BringToFront();

            //From Cam Setting
            //if(!Eqp.bIgnrCam)
            //{
            //    FrmCam_XNB = new FormCam_XNB();
            //    FrmCam_XNB.TopLevel = false;
            //    FrmCam_XNB.Parent   = pnCam;
            //    FrmCam_XNB.Dock     = DockStyle.Fill;
            //    FrmCam_XNB.Show();
            //}
            Eqp.AddMsg     = this.AddMsg    ;
            Eqp.ClearMsg   = this.ClearMsg  ;
            Eqp.ChangeScrt = this.LoadScript;

            //SEQ.Codes[(int)sc.Run].SetCode(ref rtRun.Lines);
            
            //메뉴얼 포지션 이동 에디터 최소 최대값 설정
            edMt0.Minimum = (int)ML.MT_GetMinPosition(mi.RotorT );
            edMt0.Maximum = (int)ML.MT_GetMaxPosition(mi.RotorT );
            edMt1.Minimum = (int)ML.MT_GetMinPosition(mi.LaserX );
            edMt1.Maximum = (int)ML.MT_GetMaxPosition(mi.LaserX );
            edMt2.Minimum = (int)ML.MT_GetMinPosition(mi.MagnetX);
            edMt2.Maximum = (int)ML.MT_GetMaxPosition(mi.MagnetX);

            gbM1.Text += " (" + ML.MT_GetMinPosition(mi.LaserX ).ToString() + " ~ " + ML.MT_GetMaxPosition(mi.LaserX )+"mm)";
            gbM2.Text += " (" + ML.MT_GetMinPosition(mi.MagnetX).ToString() + " ~ " + ML.MT_GetMaxPosition(mi.MagnetX)+"mm)";

        }

        public void LoadScript()
        {
            //스크립트 로드.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "JobFile\\" + OM.GetCrntDev() + "\\" ;
            try
            {
                rtRun.Clear();
                rtRun    .LoadFile(sPath+"ScriptRun.txt" , RichTextBoxStreamType.PlainText);
            }
            catch(Exception _e)
            {
                Log.Trace(_e.Message);
            }

            sPath = sExeFolder + "JobFile\\" ;
            try
            {         
                rtCool.Clear();
                rtCool.LoadFile(sPath + "ScriptCool.txt", RichTextBoxStreamType.PlainText);                
            }
            catch (Exception _e)
            {
                Log.Trace(_e.Message);
            }

            try
            {         
                rtExample.Clear();
                rtExample.LoadFile(sPath + "ScriptEx.txt", RichTextBoxStreamType.PlainText);                
            }
            catch (Exception _e)
            {
                Log.Trace(_e.Message);
            }

            try
            {
                rtTest1.Clear();
                rtTest1.LoadFile(sPath + "ScriptTest1.txt", RichTextBoxStreamType.PlainText);
            }
            catch (Exception _e)
            {
                Log.Trace(_e.Message);
            }

            try
            {
                rtTest2.Clear();
                rtTest2.LoadFile(sPath + "ScriptTest2.txt", RichTextBoxStreamType.PlainText);
            }
            catch (Exception _e)
            {
                Log.Trace(_e.Message);
            }

        }

        private static Point FindLocation(Control ctrl)
        {
            Point p;
            for (p = ctrl.Location; ctrl.Parent != null; ctrl = ctrl.Parent)
                p.Offset(ctrl.Parent.Location);
            return p;
        }

        public int getWidth(RichTextBox tb)
        {
            int w = 25;
            // get total lines of richTextBox1
            int line = tb.Lines.Length;

            if (line <= 99)
            {
                w = 20 + (int)tb.Font.Size;
            }
            else if (line <= 999)
            {
                w = 30 + (int)tb.Font.Size;
            }
            else
            {
                w = 50 + (int)tb.Font.Size;
            }

            return w;
        }
        public void AddLineNumbers()
        {
            RichTextBox tb      = rtRun   ;
            RichTextBox LineBox = LineNum1;

            int iPage = tcSequence.SelectedIndex ;

                 if(iPage == 1) { tb = rtCool   ; LineBox = LineNum2 ; }
            else if(iPage == 2) { tb = rtExample; LineBox = LineNum3 ; }
            else if(iPage == 3) { tb = rtTest1  ; LineBox = LineNum4 ; }
            else if(iPage == 4) { tb = rtTest2  ; LineBox = LineNum5 ; }

            LineBox.Clear();

            // create & set Point pt to (0,0)
            Point pt = new Point(0, 1);
            // get First Index & First Line from richTextBox1
            int First_Index = tb.GetCharIndexFromPosition(pt);
            int First_Line = tb.GetLineFromCharIndex(First_Index);
            // set X & Y coordinates of Point pt to ClientRectangle Width & Height respectively
            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;
            // get Last Index & Last Line from richTextBox1
            int Last_Index = tb.GetCharIndexFromPosition(pt);
            int Last_Line = tb.GetLineFromCharIndex(Last_Index);
            // set Center alignment to LineNumberTextBox
            LineBox.SelectionAlignment = HorizontalAlignment.Center;
            // set LineNumberTextBox text to null & width to getWidth() function value
            LineBox.Text = "";
            LineBox.Width = getWidth(tb);
            // now add each line number to LineNumberTextBox upto last line
            for (int i = First_Line; i <= Last_Line + 2; i++)
            {
                LineBox.Text += i + "\n";
            }
            LineBox.Invalidate();
        }

        //화면 메세지 박스에 메세지 추가.
        delegate void VoidDelegate();
        public void AddMsg(string _sMsg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(delegate() 
                                             {
                                                 lbMsg.Items.Add(_sMsg);
                                                 Log.Trace("AddMsg" , _sMsg);
                                                 if(lbMsg.Items.Count > 100)
                                                 {
                                                     lbMsg.Items.RemoveAt(0);
                                                     
                                                 }
                                                 lbMsg.TopIndex = lbMsg.Items.Count -1 ;
                                             } 
                                             ) );
            }
            else
            {
                lbMsg.Items.Add(_sMsg);
                lbMsg.TopIndex = lbMsg.Items.Count -1 ;
            }
        }
        public void ClearMsg()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(delegate() {lbMsg.Items.Clear();} ) );
            }
            else
            {
                lbMsg.Items.Clear();
            }
        }
        

        private void btOperator_Click(object sender, EventArgs e)
        {
            //pnPassWord.Visible = true;
            //if (FrmLogOn.m_iCrntLogIn == (int)EN_LOGIN.LogOut)
            //{
            //    FrmLogOn.ShowPage();
            //}
            SM.FrmLogOn.Show();
        }

        //[DllImport("Kernel32.dll")]
        //public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);
        //double    dPreTime  = 0.0 ;
        //double    dCrntTime = 0.0 ;
        //public double    dFrame    = 0.0 ;
        //Stopwatch Timer = Stopwatch.StartNew();
        private void timer1_Tick(object sender, EventArgs e)
        {
            //dCrntTime = Timer.ElapsedTicks ; //0.000,000,1 CTimer.GetTime_us();  10000000/
            //lbKeyword.Text = keyword ;
            //lbSelectionStart.Text = rtTest.SelectionStart.ToString() + "/" + rtTest.Text.Length ;

            //Timer.Restart() ;
            if(!SEQ.bCamInit) return ;
            lbFrame.Text = ((int)SEQ.Cam.GetFrameFrq()).ToString();


            //Add Err List
            string Str;
            int iPreErrCnt = 0;
            int iCrntErrCnt = 0;
            for (int i = 0; i < ML.ER_MaxCount(); i++)
            {
                if (ML.ER_GetErr((ei)i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt)
            {
                ErrList.Items.Clear();
                int iErrNo = ML.ER_GetLastErr();
                for (int i = 0; i < ML.ER_MaxCount(); i++)
                {
                    if (ML.ER_GetErr((ei)i))
                    {
                        Str = string.Format("[ERR{0:000}]", i);
                        Str += ML.ER_GetErrName(i) + " " + ML.ER_GetErrSubMsg((ei)i);
                        ErrList.Items.Add(Str);
                    }
                }
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error)
            {
                ErrList.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt;

            //HomeDone Check
            if (!ML.MT_GetHomeDoneAll())
            {
                btHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else
            {
                btHome.ForeColor = Color.Black;
            }

            cbLive.Enabled = !SEQ._bRun;
            btSave.Enabled = !SEQ._bRun;
            btRun .Enabled = !SEQ._bRun;
            pnMtr .Enabled = !SEQ._bRun && ML.MT_GetHomeDoneAll();
            pnCyl .Enabled = !SEQ._bRun && ML.MT_GetHomeDoneAll();

            btO4 .ForeColor = ML.IO_GetY(yi.LightOnOff  ) ? Color.Green : Color.Black;
            btO8 .ForeColor = ML.IO_GetY(yi.LaserOnOff  ) ? Color.Green : Color.Black;
            btO10.ForeColor = ML.IO_GetY(yi.SepctroLight) ? Color.Green : Color.Black;

            //Mouse
            GetCursorPos(out Point pt);
            lbMouseX.Text = pt.X.ToString();
            lbMouseY.Text = pt.Y.ToString();

            lbTemp.Text = SEQ.Heater.GetCrntTemp(0).ToString();            
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            SEQ._bBtnStop = true;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            SEQ._bBtnReset = true;
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ.SetActiveSeq(sc.Run    ); 
            //SEQ.Compile(sc.Run    , rtRun    .Lines) ;
            SEQ._bBtnStart = true ;
        }
        private void btHome_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            SEQ._bBtnHome = true ;
        }

        private void btCool_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.Compile(sc.Cool    , rtCool    .Lines) ;
            SEQ._bBtnCool = true ;
        }

        private void btAllHome_Click(object sender, EventArgs e)
        {
            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "홈동작을 수행 하시겠습니까?") != DialogResult.Yes) return;
                //MM.SetManCycle(mc.AllHome);
            }
            else
            {
                DialogResult Rslt ;
                Rslt = MessageBox.Show(new Form(){TopMost = true}, "홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                if (Rslt == DialogResult.Yes)
                {
                    ML.MT_SetServoAll(true);
                    Thread.Sleep(100);
                    for (int i = 0; i < (int)mi.MAX_MOTR; i++)
                    {
                        ML.MT_SetHomeDone(i, true);
                    }
                }
                else if(Rslt == DialogResult.No)
                {
                    //MM.SetManCycle(mc.AllHome);
                }
            }            
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            FrmVision.Close();
            DM.SaveMap();

        }

        private void btRun_Click(object sender, EventArgs e)
        {
            switch(tcSequence.SelectedIndex)
            {
                case 0: /*SEQ.Compile(sc.Run    , rtRun    .Lines) ; */SEQ.SetActiveSeq(sc.Run    ); break ;
                case 1: /*SEQ.Compile(sc.Cool   , rtCool   .Lines) ; */SEQ.SetActiveSeq(sc.Cool   ); break ;
                case 2: /*SEQ.Compile(sc.Example, rtExample.Lines) ; */SEQ.SetActiveSeq(sc.Example); break ;
                case 3: /*SEQ.Compile(sc.Test   , rtTest   .Lines) ; */SEQ.SetActiveSeq(sc.Test1  ); break ;
                case 4: /*SEQ.Compile(sc.Test   , rtTest   .Lines) ; */SEQ.SetActiveSeq(sc.Test2  ); break ;
            }
            SEQ._bBtnStart = true ;
        }

        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = Visible ;

            SEQ.Codes[(int)sc.Run    ].SetCode(rtRun.Lines);
            SEQ.Codes[(int)sc.Cool   ].SetCode(rtCool.Lines);
            SEQ.Codes[(int)sc.Example].SetCode(rtExample.Lines);
            SEQ.Codes[(int)sc.Test1  ].SetCode(rtTest1.Lines);
            SEQ.Codes[(int)sc.Test2  ].SetCode(rtTest2.Lines);

            if (Visible) AddLineNumbers();
        }

        private void cbLive_CheckedChanged(object sender, EventArgs e)
        {
            string sText = ((CheckBox)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            FormVision.bLive = cbLive.Checked ;
            if(FormVision.bLive)
            {
                SEQ.Cam.SetModeHwTrigger(false);
                SEQ.Cam.Grab();
            }
            else
            {
                SEQ.Cam.SetModeHwTrigger(true);
            }
        }
        
        private void cbRec_CheckedChanged(object sender, EventArgs e)
        {
            FrmVision.Rec(cbRec.Checked);
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            int iUnit = Convert.ToInt32(((RadioButton)sender).Tag);

            double dUserDefine = 0.0;
            if (!double.TryParse(tbUserUnit.Text, out dUserDefine)) dUserDefine = 0.0;

            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                switch (iUnit)
                {
                    default: FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog , 0d         ); break;
                    case 1 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, 1d         ); break;
                    case 2 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, 0.5d       ); break;
                    case 3 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, 0.1d       ); break;
                    case 4 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, 0.05d      ); break;
                    case 5 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, dUserDefine); break;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "JobFile\\" + OM.GetCrntDev() + "\\" ; 
            string sPath2 = sExeFolder + "JobFile\\"; 

            switch(tcSequence.SelectedIndex)
            {
                case 0: rtRun    .SaveFile(sPath +"ScriptRun.txt"  , RichTextBoxStreamType.PlainText); break ;
                case 1: rtCool   .SaveFile(sPath2+"ScriptCool.txt" , RichTextBoxStreamType.PlainText); break ;
                case 2: rtExample.SaveFile(sPath2+"ScriptEx.txt"   , RichTextBoxStreamType.PlainText); break ;
                case 3: rtTest1  .SaveFile(sPath2+"ScriptTest1.txt", RichTextBoxStreamType.PlainText); break ;
                case 4: rtTest2  .SaveFile(sPath2+"ScriptTest2.txt", RichTextBoxStreamType.PlainText); break ;
            }

            ClearMsg();
            int iIdx = tcSequence.SelectedIndex;
            bool bRet = SEQ.Codes[iIdx].RunInit();
            if(!bRet)
            {
                AddMsg(SEQ.Codes[iIdx].RunError);
                Log.ShowMessage("Error" , SEQ.Codes[iIdx].RunError  );
            }
        }

        List<string> GetContainList(List<Type> types , string _sWord)
        {
            if(types == null) return null ;
            List<string> lsTypeNames = new List<string>();
            foreach (Type type in types)
            {
                //대문자로 변환하여 대문자 소문자 구분없이 비교.
                if(type.Name.ToUpper().Contains(_sWord.ToUpper())) lsTypeNames.Add(type.Name);
            }
            if(lsTypeNames.Count == 0)
            {
                foreach (Type type in types)
                {
                    lsTypeNames.Add(type.Name);
                }
            }
            return lsTypeNames ;
        }

        string keyword = "";
        private void rtRun_KeyPress(object sender, KeyPressEventArgs e)
        {
            RichTextBox rtAct = sender as RichTextBox ;
            //int pos = rtAct.GetLineFromCharIndex(rtAct.SelectionStart); 

            if (!lbAutoComp.Visible)
            {
                //밑에서 코딩할때 TextBox에 가려져서 Parent를 페이지컨트롤상단 Panel로 한다.
                //lbAutoComp.Parent = rtAct ;
                if (Control.ModifierKeys == Keys.Control && e.KeyChar == ' ') //Space
                {
                    keyword="";
                    char cCharictor ;
                    for(int i = 1 ; i < rtAct.SelectionStart ; i++)
                    {
                        cCharictor = rtAct.Text[rtAct.SelectionStart-i];
                             if(cCharictor >= '0' && cCharictor <= '9') keyword = cCharictor + keyword;
                        else if(cCharictor >= 'a' && cCharictor <= 'z') keyword = cCharictor + keyword;
                        else if(cCharictor >= 'A' && cCharictor <= 'Z') keyword = cCharictor + keyword;
                        else if(cCharictor == '_'                     ) keyword = cCharictor + keyword;
                        else                                            break ;
                    }
                    List<string> typeNames = GetContainList(CFunction.GetFunctions() , keyword);
                    lbAutoComp.Items.Clear();
                    if(typeNames != null)
                    {
                        foreach (string typeName in typeNames)
                        {
                            lbAutoComp.Items.Add(typeName);
                        }
                        lbAutoComp.SelectedIndex = lbAutoComp.FindString(keyword);
                        Point point = rtAct.GetPositionFromCharIndex(rtAct.SelectionStart);
                        point.Y += (int)Math.Ceiling(rtAct.Font.GetHeight()) + 5; //10 is the .y postion of the richtectbox
                        point.X  = point.X + ((int)Math.Ceiling(rtAct.Font.GetHeight()/2) * keyword.Length); //105 is the .x postion of the richtectbox
                        point.Y += 60 ; //이건 페어런트가 누구냐에따라 조절... 원래는 글로벌로 좌표 따서 해야 될듯.
                        //point.X += rtAct.Left ;
                        //point.Y += 
                        Point p = FindLocation(rtAct);
                        p.X += point.X;
                        p.Y += point.Y - 165;
                        lbAutoComp.Location = p;     
                        
                        lbAutoComp.Show();
                    }
                    
                    rtAct.Focus();
                    e.Handled = true;            
                }
            }
            else
            {
                if (e.KeyChar == '\b') //backspace시엔 키워드 줄인다.
                {
                    if(keyword.Length > 0)
                    { 
                        keyword = keyword.Remove(keyword.Length - 1);
                    }
                    else
                    {
                        lbAutoComp.Visible = false;
                        rtAct.Focus();
                        e.Handled = true;

                    }
                    List<string> typeNames = GetContainList(CFunction.GetFunctions() , keyword);
                    lbAutoComp.Items.Clear();
                    if(typeNames != null)
                    {
                        foreach (string typeName in typeNames)
                        {
                            lbAutoComp.Items.Add(typeName);
                        }
                        lbAutoComp.SelectedIndex = lbAutoComp.FindString(keyword);
                    }
                    
                }
                else if (e.KeyChar == '\r')//Enter시에는 리스트박스에 선택되어진 글짜를 리치텍스트박스에 완성시킨다.
                {
                    lbAutoComp.Visible = false;

                    //rtAct.Text.TrimEnd('\n');

                    string functionName = lbAutoComp.Text;
                    string AddingText   = "";
                    
                    
                    
                    //여기부터
                    rtAct.Focus();
                    int iStartCursorPos = rtAct.SelectionStart ;
                    //if(keyword.Length>0) 
                    {
                        rtAct.Text = rtAct.Text.Remove(iStartCursorPos - keyword.Length , keyword.Length);//-1은 엔터시에 \n이 이미 들어있음.
                        iStartCursorPos -= keyword.Length ;
                    }
                    

                    //if(keyword.Length>0) rtAct.Text = rtAct.Text.Remove(rtAct.Text.Length - keyword.Length-1);//-1은 엔터시에 \n이 이미 들어있음.

                    AddingText += functionName;
                    int iParaCnt = CFunction.GetParaCount(functionName);
                    AddingText += "(";
                    int iCursorPos =  iStartCursorPos + AddingText.Length; //rtAct.Text.Length + AddingText.Length;
                    
                    for(int i = 1 ; i < iParaCnt ; i++)
                    {
                        AddingText += "," ;
                    }
                    AddingText += ");";
                    AddingText += CFunction.GetComment(functionName);
                    int iTextLength = rtAct.Text.Length ;
                    rtAct.Text = rtAct.Text.Insert(iStartCursorPos , AddingText);
                    //if(iTextLength <= iStartCursorPos) rtAct.Text+=AddingText;
                    //else                               rtAct.Text = rtAct.Text.Insert(iStartCursorPos , AddingText);

                    rtAct.Focus();//커서
                    if(iParaCnt==0) iCursorPos = iStartCursorPos + AddingText.Length ;
                    rtAct.SelectionStart = iCursorPos ;
                    e.Handled = true;
                }
                else if (e.KeyChar == '\u001b') //Esc시엔 취소
                {
                    lbAutoComp.Visible = false;
                    rtAct.Focus();
                    e.Handled = true;
                }
                else//나머지는 그냥 글짜를 붙인다.
                {
                    keyword += e.KeyChar;
                    List<string> typeNames = GetContainList(CFunction.GetFunctions() , keyword);
                    lbAutoComp.Items.Clear();
                    if(typeNames != null)
                    {
                        foreach (string typeName in typeNames)
                        {
                            lbAutoComp.Items.Add(typeName);
                        }
                        lbAutoComp.SelectedIndex = lbAutoComp.FindString(keyword);
                    }
                    rtAct.Focus();
                }
            }

        }

        private void rtRun_KeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox rtAct = sender as RichTextBox;
            if (lbAutoComp.Visible)
            {
                if(e.KeyCode == Keys.Up)
                {
                    if(0 < lbAutoComp.SelectedIndex-1)lbAutoComp.SelectedIndex-- ;
                    e.Handled = true ;
                }
                else if(e.KeyCode == Keys.Down)
                {
                    if(lbAutoComp.Items.Count > lbAutoComp.SelectedIndex+1)lbAutoComp.SelectedIndex++ ;      
                    e.Handled = true ;
                }
                else if(e.KeyCode == Keys.Enter) 
                {
                    int    iSel  = rtAct.SelectionStart ;
                    string sTemp = rtAct.Text ;
                    e.Handled = true ;
                }
                else if(e.Control && e.KeyCode == Keys.Space)
                {
                    lbAutoComp.Visible = false;
                    rtAct.Focus();
                    e.Handled = true;
                }
            }
            else
            {
                
            }

            //undo.
            if(e.KeyCode == Keys.Z && (e.Control))
            {
                string sPop = "" ;

                if(rtAct.CanUndo)
                {
                    rtAct.Undo();
                }


                //switch(tcSequence.SelectedIndex)
                //{
                //    case 0: if(UndoListRun    .Count>0)sPop = UndoListRun    .Pop(); break ;
                //    case 1: if(UndoListCool   .Count>0)sPop = UndoListCool   .Pop(); break ;
                //    case 2: if(UndoListExample.Count>0)sPop = UndoListExample.Pop(); break ;
                //    case 3: if(UndoListTest   .Count>0)sPop = UndoListTest   .Pop(); break ;
                //}
                //switch(tcSequence.SelectedIndex)
                //{
                //    case 0: rtRun     .Text = sPop; break ;
                //    case 1: rtCool    .Text = sPop; break ;
                //    case 2: rtExample .Text = sPop; break ;
                //    case 3: rtTest    .Text = sPop; break ;
                //}
            }
            //AddLineNumbers();
        }

        private void rtAutoComp_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void rtRun_TextChanged(object sender, EventArgs e)
        {
            
            SEQ.Codes[(int)sc.Run].SetCode(rtRun.Lines);
        }

        private void rtCool_TextChanged(object sender, EventArgs e)
        {
            SEQ.Codes[(int)sc.Cool].SetCode(rtCool.Lines);
        }

        private void rtExample_TextChanged(object sender, EventArgs e)
        {
            SEQ.Codes[(int)sc.Example].SetCode(rtExample.Lines);
        }

        private void rtTest_TextChanged(object sender, EventArgs e)
        {
            SEQ.Codes[(int)sc.Test1].SetCode(rtTest1.Lines);
        }

        private void pnCam_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            //SEQ.Reset();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            FrmVision.Save();
        }

        private void btO4_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            string sTag = ((Button)sender).Tag.ToString();
            int    iTag = 0;
            int.TryParse(sTag,out iTag);
            ML.IO_SetY(iTag,!ML.IO_GetY(iTag));
            

        }

        private void btMv0_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            string sTag = ((Button)sender).Tag.ToString();
            int    iTag = 0;
            int.TryParse(sTag,out iTag);

            double dPos = 0.0;
                 if(iTag == 0) dPos = (double)edMt0.Value;
            else if(iTag == 1) dPos = (double)edMt1.Value;
            else if(iTag == 2) dPos = (double)edMt2.Value;

            ML.MT_GoAbsMan(iTag,dPos);
        }

        private void FormOperation_Load(object sender, EventArgs e)
        {
            AddLineNumbers();
        }

        private void rtRun_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void rtRun_VScroll(object sender, EventArgs e)
        {
            AddLineNumbers();
        }

        private void tcSequence_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddLineNumbers();
        }

        private void btTest1Run_Click(object sender, EventArgs e)
        {
            SEQ.SetActiveSeq(sc.Test1); 
            SEQ._bBtnStart = true ;
        }

        private void btTest2Run_Click(object sender, EventArgs e)
        {
            SEQ.SetActiveSeq(sc.Test2); 
            SEQ._bBtnStart = true ;
        }

        private void rtTest2_TextChanged(object sender, EventArgs e)
        {
            SEQ.Codes[(int)sc.Test2].SetCode(rtTest2.Lines);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool bRet = SEQ.Codes[(int)sc.Run].RunInit();
            if(!bRet)
            {
                Eqp.AddMsg(SEQ.Codes[(int)sc.Run].RunError);
                Log.ShowMessage("Error" , SEQ.Codes[(int)sc.Run].RunError  );
                //ML.ER_SetErr(ei.RUN_Error , Codes[(int)activeSeqCode].RunError);
                //bStartSw = false ;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            bool bRet = SEQ.Codes[(int)sc.Run].Run();
            string sErrMsg = SEQ.Codes[(int)sc.Run].RunError ;
            if(sErrMsg != "") ML.ER_SetErr(ei.RUN_Error , sErrMsg);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormSpectroMain.frmSpectrometer.SpectroSaveWave("asdf", 2);
        }







        //public class DoubleBuffer : Panel
        //{
        //    public DoubleBuffer()
        //    {
        //        this.SetStyle(ControlStyles.ResizeRedraw         , true);
        //        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        //        this.SetStyle(ControlStyles.UserPaint            , true);
        //        this.SetStyle(ControlStyles.AllPaintingInWmPaint , true);
        //
        //        this.UpdateStyles();
        //    }
        //}
        //public class DoubleBufferP : PictureBox
        //{
        //    public DoubleBufferP()
        //    {
        //        this.DoubleBuffered = true;
        //        this.SetStyle(ControlStyles.UserPaint |
        //          ControlStyles.AllPaintingInWmPaint  |
        //          ControlStyles.ResizeRedraw          |
        //          ControlStyles.ContainerControl      |
        //          ControlStyles.OptimizedDoubleBuffer |
        //          ControlStyles.SupportsTransparentBackColor
        //          , true);
        //    }
        //}
        //


    }
}