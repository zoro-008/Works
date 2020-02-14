using System;
using System.Windows.Forms;
using System.Reflection;
using COMMON;
using SML;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

//using System.Runtime.InteropServices;
//using ML.CXmlBase;
//using SMLDefine;
//using SMLApp;

namespace Machine
{
    public struct TPstnValue
    {
        public string sName; //포지션 이름
        public bool   bOffset; //상대값 포지션인지 오프셑인지.
        public bool   bFix; //수정 불가 포지션
        public bool   bCommon; //전디바이스 공용.
        public double dValue; //값.
        public double dMin;
        public double dMax;
        public int    iSpdPer;
    };

    public delegate bool dgCheckSafe(mi _eMotr, pv _ePstn ,  double _dOfsPos=0);
    public delegate double dgGetCmdPos(int m);

    public class MotrPstn
    {
        uint m_uMaxPstn;
        uint m_uMotrId;
        public TPstnValue[] PstnValue;
        public int iMotrMosCellSize = 25; //모터 포지션 셀높이
        //ListView Column 사이즈
        //public int iNameWidth = 180;
        //public int iValueWidth = 105;
        //public int iGoWidth = 50;
        //public int iInputWidth = 75;
        //public int iMinMaxWidth = 75;
        public int iNameWidth       = 500; //= 305; 1280 1024
        public int iValueWidth      = 250; //= 150;
        public int iGoWidth         =  80; //=  50;
        public int iInputWidth      = 100; //=  70;
        public int iMinMaxWidthName = 100; //=  50; //Name
        public int iMinMaxWidthVal  = 150; //=  80; //Value 775
        public int iSpeedPerName    = 100; //=  80;
        public int iSpeedPerVal     =  70; //=  40;
        dgCheckSafe CheckSafe;
        //dgGetCmdPos GetCmdPos;

        public ListViewEx lvPstnView;

        TextBox tbPstnEdit;
        Button[] btPstnGo;
        Button[] btPstnInput;
        TextBox tbPstnMinEdit;
        TextBox tbPstnMaxEdit;
        TextBox tbPstnSpdEdit;

        public MotrPstn(uint _uMotrId, uint _uPstnCnt)
        {
            m_uMotrId = _uMotrId;
            m_uMaxPstn = _uPstnCnt;
            PstnValue = new TPstnValue[_uPstnCnt];

            lvPstnView = new ListViewEx();


            lvPstnView.Clear();
            lvPstnView.View = View.Details;
            lvPstnView.FullRowSelect = true;
            lvPstnView.GridLines = true;

            //셀높이 조절하기 편법.
            //ImageList dummyImage = new ImageList();
            //dummyImage.ImageSize = new System.Drawing.Size(1, iMotrMosCellSize); //(1, 30);
            //lvPstnView.SmallImageList = dummyImage;

            //1280*1024 해상도에서 Column 폭
            lvPstnView.Columns.Add("Name"    , iNameWidth,  HorizontalAlignment.Left);//150
            lvPstnView.Columns.Add("Value"   , iValueWidth, HorizontalAlignment.Left);//100
            lvPstnView.Columns.Add(""        , iGoWidth,    HorizontalAlignment.Left);//55
            lvPstnView.Columns.Add(""        , iInputWidth, HorizontalAlignment.Left);
            lvPstnView.Columns.Add("Min"     , iMinMaxWidthName, HorizontalAlignment.Left);
            lvPstnView.Columns.Add(""        , iMinMaxWidthVal, HorizontalAlignment.Left);
            lvPstnView.Columns.Add("Max"     , iMinMaxWidthName, HorizontalAlignment.Left);
            lvPstnView.Columns.Add(""        , iMinMaxWidthVal, HorizontalAlignment.Left);
            lvPstnView.Columns.Add("Speed % ", iSpeedPerName, HorizontalAlignment.Left);
            lvPstnView.Columns.Add(""        , iSpeedPerVal, HorizontalAlignment.Left);

            //1024*768 해상도에서 Column 폭
            //lvPstnView.Columns.Add("Name"    , 60, HorizontalAlignment.Left);
            //lvPstnView.Columns.Add("Value"   , 70, HorizontalAlignment.Left);
            //lvPstnView.Columns.Add("Go"      , 35, HorizontalAlignment.Left);
            //lvPstnView.Columns.Add("Input"   , 55, HorizontalAlignment.Left);
            //lvPstnView.Columns.Add("Min"     , 55, HorizontalAlignment.Left);
            //lvPstnView.Columns.Add("Max"     , 55, HorizontalAlignment.Left);

            lvPstnView.HeaderStyle = ColumnHeaderStyle.None;
            //lvPstnView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            //lvPstnView.OwnerDraw = true;
            lvPstnView.DrawColumnHeader    += new DrawListViewColumnHeaderEventHandler(lvDrawColumnHeader);
            lvPstnView.DrawSubItem         += new DrawListViewSubItemEventHandler     (lvDrawSubItem);
            lvPstnView.ColumnWidthChanging += new ColumnWidthChangingEventHandler     (lvColumnWidthChanging);

            ListViewItem[] liInput = new ListViewItem[_uPstnCnt];

            var PropInput = lvPstnView.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvPstnView, true, null);

            btPstnGo = new Button[_uPstnCnt];
            btPstnInput = new Button[_uPstnCnt];

            //Font BtnFont = new Font("굴림", 10f, FontStyle.Bold);
            Font BtnFont = new Font("Arial", 10f);//, FontStyle.Bold);


            for (int i = 0; i < _uPstnCnt; i++)
            {
                liInput[i] = new ListViewItem(string.Format("{0:000}", i));
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");

                liInput[i].UseItemStyleForSubItems = false; //요건 셀별 색깔 넣을때.
                liInput[i].UseItemStyleForSubItems = false;

                lvPstnView.Items.Add(liInput[i]);

                btPstnGo[i] = new Button();
                btPstnInput[i] = new Button();

                btPstnGo[i].Tag = i;
                btPstnInput[i].Tag = i;

                btPstnGo[i].Font = BtnFont;
                btPstnInput[i].Font = BtnFont;

                btPstnGo[i].Text = "GO";
                btPstnInput[i].Text = "INPUT";

                btPstnGo[i].BackColor = Color.Aqua;
                btPstnInput[i].BackColor = Color.Lime;

                lvPstnView.AddEmbeddedControl(btPstnGo[i], 2, i);
                lvPstnView.AddEmbeddedControl(btPstnInput[i], 3, i);

                //lvPstnView.Items[i].SubItems[1].Font = BtnFont;
                lvPstnView.Items[i].SubItems[0].BackColor = Color.Silver;
                lvPstnView.Items[i].SubItems[4].BackColor = Color.Silver;
                lvPstnView.Items[i].SubItems[5].BackColor = Color.Silver;
                lvPstnView.Items[i].SubItems[6].BackColor = Color.Silver;
                lvPstnView.Items[i].SubItems[7].BackColor = Color.Silver;
                lvPstnView.Items[i].SubItems[8].BackColor = Color.Silver;
                lvPstnView.Items[i].SubItems[9].BackColor = Color.Silver;

                btPstnGo[i].Click += new EventHandler(btGoClick);
                btPstnInput[i].Click += new EventHandler(btInputClick);


            }
            tbPstnEdit = new TextBox();
            tbPstnEdit.Visible = false;
            tbPstnEdit.BorderStyle = BorderStyle.FixedSingle;
            tbPstnEdit.Tag = 0;
            tbPstnEdit.Leave += tbPstnEdit_Leave;
            tbPstnEdit.KeyDown += tbPstnEdit_KeyDown;

            tbPstnMinEdit = new TextBox();
            tbPstnMinEdit.Visible = false;
            tbPstnMinEdit.BorderStyle = BorderStyle.FixedSingle;
            tbPstnMinEdit.Tag = 0;
            tbPstnMinEdit.Leave   += tbPstnMinEdit_Leave;
            tbPstnMinEdit.KeyDown += tbPstnMinEdit_KeyDown;

            tbPstnMaxEdit = new TextBox();
            tbPstnMaxEdit.Visible = false;
            tbPstnMaxEdit.BorderStyle = BorderStyle.FixedSingle;
            tbPstnMaxEdit.Tag = 0;
            tbPstnMaxEdit.Leave   += tbPstnMaxEdit_Leave;
            tbPstnMaxEdit.KeyDown += tbPstnMaxEdit_KeyDown;

            tbPstnSpdEdit = new TextBox();
            tbPstnSpdEdit.Visible = false;
            tbPstnSpdEdit.BorderStyle = BorderStyle.FixedSingle;
            tbPstnSpdEdit.Tag = 0;
            tbPstnSpdEdit.Leave   += tbPstnSpdEdit_Leave;
            tbPstnSpdEdit.KeyDown += tbPstnSpdEdit_KeyDown;

            lvPstnView.AddEmbeddedControl(tbPstnEdit   , 1, 0);
            lvPstnView.AddEmbeddedControl(tbPstnMinEdit, 5, 0);
            lvPstnView.AddEmbeddedControl(tbPstnMaxEdit, 7, 0);
            lvPstnView.AddEmbeddedControl(tbPstnSpdEdit, 9, 0);

            lvPstnView.Click += new EventHandler(lvPstnView_Click);

            //for (int i = 0; i < _uMotrId; i++)
            //{
            //    btPstnGo[_uPstnCnt].Click += new EventHandler(btGoClick);
            //    btPstnInput[_uPstnCnt].Click += new EventHandler(btInputClick);
            //}
            //lvPstnView.DrawItem += new DrawListViewItemEventHandler(lvPstnView_DrawItem);
            //lvPstnView.OwnerDraw = true ;

        }

        private void lvDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Silver, e.Bounds);
            e.DrawText();
        }
        
        void lvDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            //string searchTerm = "Term";
            //int index = e.SubItem.Text.IndexOf(searchTerm);
            //if (index >= 0)
            //{
            //    string sBefore = e.SubItem.Text.Substring(0, index);

            //    Size bounds = new Size(e.Bounds.Width, e.Bounds.Height);
            //    Size s1 = TextRenderer.MeasureText(e.Graphics, sBefore, lvPstnView.Font, bounds);
            //    Size s2 = TextRenderer.MeasureText(e.Graphics, searchTerm, lvPstnView.Font, bounds);

            //    Rectangle rect = new Rectangle(e.Bounds.X + s1.Width, e.Bounds.Y, s2.Width, e.Bounds.Height);
            //    e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), rect);
            //}

            Rectangle rect = new Rectangle(e.Bounds.X + 1 , e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
            if (e.ColumnIndex == 0) e.Graphics.FillRectangle(Brushes.Silver, rect);
            e.DrawText();

            //if ((e.ItemState & ListViewItemStates.Focused) > 0)
            //{
            //    e.Graphics.FillRectangle(SystemBrushes.Highlight,
            //    e.Bounds);
            //    //e.Graphics.DrawString(e.Item.Text, lvPstnView.Font,
            //    //SystemBrushes.HighlightText, e.Bounds);
            //    e.DrawText();
            //}
            //else
            //{
            //    e.DrawBackground();
            //    e.DrawText();
            //}
        }
        void lvColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView != null)
            {
                e.NewWidth = listView.Columns[e.ColumnIndex].Width;
                e.Cancel = true;
            }
        }

        private void tbPstnEdit_Leave(object sender, EventArgs e)
        {
            TextBox Tb = sender as TextBox;
            int iPosId = (int)Tb.Tag;
            Tb.Visible = false;
            double dVal;
            if (!double.TryParse(Tb.Text, out dVal)) return;
            SetValue((uint)iPosId, dVal, false);
        }

        private void tbPstnEdit_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox Tb = sender as TextBox;
            if (e.KeyCode == Keys.Enter)
            {
                double dVal;
                int iPosId = (int)Tb.Tag;
                Tb.Visible = false;
                if (!double.TryParse(Tb.Text, out dVal)) return;
                SetValue((uint)iPosId, dVal, false);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Tb.Text = "";
                Tb.Visible = false;
            }
        }

        private void tbPstnMinEdit_Leave(object sender, EventArgs e)
        {
            TextBox Tb = sender as TextBox;
            int iPosId = (int)Tb.Tag;
            Tb.Visible = false;
            double dVal;
            if (!double.TryParse(Tb.Text, out dVal)) return;
            SetValueMin((uint)iPosId, dVal, false);
        }

        private void tbPstnMinEdit_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox Tb = sender as TextBox;
            if (e.KeyCode == Keys.Enter)
            {
                double dVal;
                int iPosId = (int)Tb.Tag;
                Tb.Visible = false;
                if (!double.TryParse(Tb.Text, out dVal)) return;
                SetValueMin((uint)iPosId, dVal, false);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Tb.Text = "";
                Tb.Visible = false;
            }
        }

        private void tbPstnMaxEdit_Leave(object sender, EventArgs e)
        {
            TextBox Tb = sender as TextBox;
            int iPosId = (int)Tb.Tag;
            Tb.Visible = false;
            double dVal;
            if (!double.TryParse(Tb.Text, out dVal)) return;
            SetValueMax((uint)iPosId, dVal, false);
        }

        private void tbPstnMaxEdit_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox Tb = sender as TextBox;
            if (e.KeyCode == Keys.Enter)
            {
                double dVal;
                int iPosId = (int)Tb.Tag;
                Tb.Visible = false;
                if (!double.TryParse(Tb.Text, out dVal)) return;
                SetValueMax((uint)iPosId, dVal, false);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Tb.Text = "";
                Tb.Visible = false;
            }
        }


        private void tbPstnSpdEdit_Leave(object sender, EventArgs e)
        {
            TextBox Tb = sender as TextBox;
            int iPosId = (int)Tb.Tag;
            Tb.Visible = false;
            int iVal;
            if (!int.TryParse(Tb.Text, out iVal)) return;
            SetValueSpd((uint)iPosId, iVal, false);
        }

        private void tbPstnSpdEdit_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox Tb = sender as TextBox;
            if (e.KeyCode == Keys.Enter)
            {
                int iVal;
                int iPosId = (int)Tb.Tag;
                Tb.Visible = false;
                if (!int.TryParse(Tb.Text, out iVal)) return;
                SetValueSpd((uint)iPosId, iVal, false);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Tb.Text = "";
                Tb.Visible = false;
            }
        }

        private void lvPstnView_Click(object sender, EventArgs e)
        {
            ListView Lv = sender as ListView;

            Point mousePos = Lv.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = Lv.HitTest(mousePos);
            if (hitTest.Item == null) return;
            int row = hitTest.Item.Index;
            int col = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);

            if (col == 1)
            {
                lvPstnView.MoveEmbeddedControl(tbPstnEdit, 1, row);

                tbPstnEdit.Visible = true;
                tbPstnEdit.Text = GetValue((uint)row).ToString();
                tbPstnEdit.Tag = row;
                tbPstnEdit.Select();//Focus();
            }
            if (col == 5)
            {
                lvPstnView.MoveEmbeddedControl(tbPstnMinEdit, 5, row);

                tbPstnMinEdit.Visible = true;
                tbPstnMinEdit.Text = GetValueMin((uint)row).ToString();
                tbPstnMinEdit.Tag = row;
                tbPstnMinEdit.Select();//Focus();
            }
            if (col == 7)
            {
                lvPstnView.MoveEmbeddedControl(tbPstnMaxEdit, 7, row);

                tbPstnMaxEdit.Visible = true;
                tbPstnMaxEdit.Text = GetValueMax((uint)row).ToString();
                tbPstnMaxEdit.Tag = row;
                tbPstnMaxEdit.Select();//Focus();
            }
            if (col == 9)
            {
                lvPstnView.MoveEmbeddedControl(tbPstnSpdEdit, 9, row);

                tbPstnSpdEdit.Visible = true;
                tbPstnSpdEdit.Text = GetValueSpd((uint)row).ToString();
                tbPstnSpdEdit.Tag = row;
                tbPstnSpdEdit.Select();//Focus();
            }
        }

        private void btGoClick(object sender, EventArgs e)
        {
            //Button Bt = sender as Button;
            //uint uPstnNo = (uint)Bt.Tag;
            if(!ML.MT_GetStop(m_uMotrId)) ML.MT_Stop(m_uMotrId);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;

            bool bRet = true;
            if (Log.ShowMessageModal("Confirm", "Do you want to Part Move?") != DialogResult.Yes) return;
            //if (MessageBox.Show(new Form{TopMost = true},"Part를 이동하시겠습니까?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            if (CheckSafe != null)
            {
                if(!CheckSafe((mi)m_uMotrId , (pv)uPstnNo)) bRet = false ;
            }
            //if(m_uMotrId == (int)mi.IDX_X) bRet = SEQ.IDX.CheckSafe((mi)m_uMotrId, uPstnNo);

            if (!bRet) return;
            //ML.MT_GoAbsRun(m_uMotrId, PM.GetValue(m_uMotrId, uPstnNo), PM.GetValueSpdPer(m_uMotrId, uPstnNo) );
            ML.MT_GoAbsMan(m_uMotrId, PM.GetValue(m_uMotrId, uPstnNo));

        }
        private void btInputClick(object sender, EventArgs e)
        {
            //Button Bt = sender as Button;
            //uint uPstnNo = (uint)Bt.Tag;

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;

            bool   bServo = ML.MT_GetServo(m_uMotrId);
            
            double dPos, dInputPos;
            string sPos;

            if (bServo)
            {
                dPos      = ML.MT_GetCmdPos(m_uMotrId);
                sPos      = dPos.ToString("N4");
                dInputPos = double.Parse(sPos);
            }
            else
            {
                dPos      = ML.MT_GetEncPos(m_uMotrId);
                sPos      = dPos.ToString("N4");
                dInputPos = double.Parse(sPos);

            }

            string sText = ((Button)sender).Text;
            
            Log.Trace("Form DeviceSet Input Button Clicked ("  + ML.MT_GetName(m_uMotrId) + " " + GetName(uPstnNo).Trim() + " " +
                       GetValue(uPstnNo) + " -> " + sPos + ")", ForContext.Frm);

            //SetValue(uPstnNo , GetCmdPos((int)m_uMotrId));
            SetValue(uPstnNo, dInputPos);

        }

        public void SetProp(uint _uPstnNo, string _sName, bool _bOffset, bool _bFix, bool _bCommon)
        {
            PstnValue[_uPstnNo].sName = _sName;
            PstnValue[_uPstnNo].bOffset = _bOffset;
            PstnValue[_uPstnNo].bFix = _bFix;
            PstnValue[_uPstnNo].bCommon = _bCommon;

            if (lvPstnView.Parent != null)
            {
                //UI에 접근 하기 위한 인보크
                if (lvPstnView.InvokeRequired)
                {
                    lvPstnView.Invoke(new MethodInvoker(delegate ()
                    {
                        // code for running in ui thread
                        btPstnGo[_uPstnNo].Visible = !_bOffset;
                        btPstnInput[_uPstnNo].Visible = !_bFix && !_bOffset;
                        lvPstnView.Items[(int)_uPstnNo].SubItems[0].Text = _sName;
                        lvPstnView.Items[(int)_uPstnNo].SubItems[0].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                        lvPstnView.Items[(int)_uPstnNo].SubItems[4].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                        //lvPstnView.Items[(int)_uPstnNo].SubItems[5].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                        //lvPstnView.Items[(int)_uPstnNo].SubItems[6].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                        //lvPstnView.Items[(int)_uPstnNo].SubItems[7].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                        //lvPstnView.Items[(int)_uPstnNo].SubItems[8].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                        //lvPstnView.Items[(int)_uPstnNo].SubItems[9].BackColor = _bCommon ? Color.LightGray : Color.Silver;

                    }));
                }

                else
                {
                    btPstnGo[_uPstnNo].Visible = !_bOffset;
                    btPstnInput[_uPstnNo].Visible = !_bFix && !_bOffset;
                    lvPstnView.Items[(int)_uPstnNo].SubItems[0].Text = _sName;
                    lvPstnView.Items[(int)_uPstnNo].SubItems[0].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                    lvPstnView.Items[(int)_uPstnNo].SubItems[4].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                    //lvPstnView.Items[(int)_uPstnNo].SubItems[5].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                    //lvPstnView.Items[(int)_uPstnNo].SubItems[6].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                    //lvPstnView.Items[(int)_uPstnNo].SubItems[7].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                    //lvPstnView.Items[(int)_uPstnNo].SubItems[8].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                    //lvPstnView.Items[(int)_uPstnNo].SubItems[9].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                }

            }
        }

        public double GetValue(uint _uPstnNo)
        {
            double dTemp = PstnValue[_uPstnNo].dValue;
            return dTemp;
        }
        public string GetName(uint _uPstnNo)
        {
            return PstnValue[_uPstnNo].sName;
        }
        public double GetValueMin(uint _uPstnNo)
        {
            return PstnValue[_uPstnNo].dMin;
        }
        public double GetValueMax(uint _uPstnNo)
        {
            return PstnValue[_uPstnNo].dMax;
        }
        public int GetValueSpd(uint _uPstnNo)
        {
            return PstnValue[_uPstnNo].iSpdPer;
        }

        public void SetValue(uint _uPstnNo, double _dValue, bool _bWrite = true)
        {
            double dValue = _dValue;

            if (dValue < PstnValue[_uPstnNo].dMin) dValue = PstnValue[_uPstnNo].dMin;
            if (dValue > PstnValue[_uPstnNo].dMax) dValue = PstnValue[_uPstnNo].dMax;
            if(_bWrite) PstnValue[_uPstnNo].dValue = dValue;

            //UI에 접근 하기 위한 인보크
            if (lvPstnView.Parent != null)
            {
                if (lvPstnView.InvokeRequired)
                {
                    lvPstnView.Invoke(new MethodInvoker(delegate ()
                    {
                        // code for running in ui thread
                        lvPstnView.Items[(int)_uPstnNo].SubItems[1].Text = dValue.ToString();

                    }));
                    //lvPstnView.Items[(int)_uPstnNo].SubItems[1].Text = _dValue.ToString();
                    //}));
                }
                else
                {
                    lvPstnView.Items[(int)_uPstnNo].SubItems[1].Text = dValue.ToString();
                }
            }
        }

        public void SetValueMin(uint _uPstnNo, double _dValue, bool _bWrite = true)
        {
            if(_bWrite) PstnValue[_uPstnNo].dMin = _dValue;

            //UI에 접근 하기 위한 인보크
            if (lvPstnView.Parent != null)
            {
                if (lvPstnView.InvokeRequired)
                {
                    lvPstnView.Invoke(new MethodInvoker(delegate (){lvPstnView.Items[(int)_uPstnNo].SubItems[5].Text = _dValue.ToString();}));
                }
                else
                {
                    lvPstnView.Items[(int)_uPstnNo].SubItems[5].Text = _dValue.ToString();
                }
            }
        }

        public void SetValueMax(uint _uPstnNo, double _dValue, bool _bWrite = true)
        {
            if(_bWrite) PstnValue[_uPstnNo].dMax = _dValue;

            //UI에 접근 하기 위한 인보크
            if (lvPstnView.Parent != null)
            {
                if (lvPstnView.InvokeRequired)
                {
                    lvPstnView.Invoke(new MethodInvoker(delegate (){lvPstnView.Items[(int)_uPstnNo].SubItems[7].Text = _dValue.ToString();}));
                }
                else
                {
                    lvPstnView.Items[(int)_uPstnNo].SubItems[7].Text = _dValue.ToString();
                }
            }
        }

        public void SetValueSpd(uint _uPstnNo, int _iValue, bool _bWrite = true)
        {
            int iValue = _iValue;
            if (iValue <=  0) iValue = 100;
            if (iValue > 100) iValue = 100;
            if(_bWrite) PstnValue[_uPstnNo].iSpdPer = iValue;

            //UI에 접근 하기 위한 인보크
            if (lvPstnView.Parent != null)
            {
                if (lvPstnView.InvokeRequired)
                {
                    lvPstnView.Invoke(new MethodInvoker(delegate () { lvPstnView.Items[(int)_uPstnNo].SubItems[9].Text = iValue.ToString(); }));
                }
                else
                {
                    lvPstnView.Items[(int)_uPstnNo].SubItems[9].Text = iValue.ToString();
                }
            }
        }

        public void SetCheckSafe(dgCheckSafe _dgCheckSafe)
        {
            CheckSafe = _dgCheckSafe;

        }
        //public void SetGetCmdPos(dgGetCmdPos _dgGetCmdPos)
        //{
        //    GetCmdPos = _dgGetCmdPos;
        //}

        public void SetListviewParent(Panel _pnBase)
        {
            _pnBase.Dock = DockStyle.Fill;
            _pnBase.Width  = _pnBase.Width - 17;
            _pnBase.Height = 5 + (iMotrMosCellSize) * (int)m_uMaxPstn + 1;
            //_pnBase.Dock = DockStyle.None;
            _pnBase.AutoScroll = false;
            lvPstnView.Left = -4;
            lvPstnView.Width = _pnBase.Width;
            lvPstnView.Height = 5 + (iMotrMosCellSize) * (int)m_uMaxPstn ;

            lvPstnView.Parent = _pnBase;

            // code for running in ui thread
            for (int i = 0; i < m_uMaxPstn; i++)
            {
                btPstnGo[i].Visible = !PstnValue[i].bOffset;
                btPstnInput[i].Visible = !PstnValue[i].bFix && !PstnValue[i].bOffset;
                lvPstnView.Items[(int)i].SubItems[0].Text = PstnValue[(int)i].sName;
                lvPstnView.Items[(int)i].SubItems[0].BackColor = PstnValue[(int)i].bCommon ? Color.LightGray : Color.Silver;
                lvPstnView.Items[(int)i].SubItems[1].Text = PstnValue[(int)i].dValue.ToString();
                lvPstnView.Items[(int)i].SubItems[4].Text = "Min";
                lvPstnView.Items[(int)i].SubItems[5].Text = PstnValue[(int)i].dMin.ToString();
                lvPstnView.Items[(int)i].SubItems[6].Text = "Max";
                lvPstnView.Items[(int)i].SubItems[7].Text = PstnValue[(int)i].dMax.ToString();
                lvPstnView.Items[(int)i].SubItems[8].Text = "Speed %";
                lvPstnView.Items[(int)i].SubItems[9].Text = PstnValue[(int)i].iSpdPer.ToString();


            }

        }

    }


    public static class PM
    {
        static uint m_iMaxMotr;
        static MotrPstn[] MotrPstn;

        static uint[] PstnCnt;

        public static void Init(uint[] _uPstnCnts)
        {
            PstnCnt = _uPstnCnts;
            m_iMaxMotr = (uint)PstnCnt.Length;


            MotrPstn = new MotrPstn[m_iMaxMotr];
            for (uint i = 0; i < m_iMaxMotr; i++)
            {
                MotrPstn[i] = new MotrPstn(i, PstnCnt[i]);
            }
        }
        public static void SetProp(uint _uAxisNo, uint _uPstnNo, string _sName, bool _bOffset, bool _bFix, bool _bCommon)
        {
            MotrPstn[_uAxisNo].SetProp(_uPstnNo, _sName, _bOffset, _bFix, _bCommon);
        }

        public static void SetCheckSafe(uint _uAxisNo, dgCheckSafe _dgCheckSafe)
        {
            MotrPstn[_uAxisNo].SetCheckSafe(_dgCheckSafe);

        }
        //public static void SetGetCmdPos(uint _uAxisNo, dgGetCmdPos _dgGetCmdPos)
        //{
        //    MotrPstn[_uAxisNo].SetGetCmdPos(_dgGetCmdPos);
        //}

        public static double GetValue(uint _uAxisNo, uint _uPstnNo)
        {
            double dTemp = MotrPstn[_uAxisNo].GetValue(_uPstnNo);
            return dTemp;
        }

        public static double GetValue(mi _eAxisNo, pv _ePstnNo)
        {
            return GetValue((uint)_eAxisNo, (uint)_ePstnNo);
        }

        public static int GetValueSpdPer(uint _uAxisNo, uint _uPstnNo)
        {
            return MotrPstn[_uAxisNo].GetValueSpd(_uPstnNo);
        }

        public static int GetValueSpdPer(mi _eAxisNo, pv _ePstnNo)
        {
            return GetValueSpdPer((uint)_eAxisNo, (uint)_ePstnNo);
        }

        public static void SetValue(uint _uAxisNo, uint _uPstnNo, double _dValue)
        {
            MotrPstn[_uAxisNo].SetValue(_uPstnNo, _dValue);
        }

        public static void SetValue(mi _uAxisNo, pv _uPstnNo, double _dValue)
        {
            SetValue((uint)_uAxisNo, (uint)_uPstnNo, _dValue);
        }

        public static void Load(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevicePath = sExeFolder + "JobFile\\" + _sJobName + "\\MotrPstn.ini";
            string sCommonPath = sExeFolder + "Util\\CommonPstn.ini";
            string sMinMaxPath = sExeFolder + "Util\\LimitPstn.ini";

            CIniFile IniDevice = new CIniFile(sDevicePath);
            CIniFile IniCommon = new CIniFile(sCommonPath);
            CIniFile IniMinMax = new CIniFile(sMinMaxPath);

            string sSection;

            //Save Device.
            for (int m = 0; m < m_iMaxMotr; m++)
            {
                //Set Dir.
                sSection = "Motor" + m.ToString();
                for (int i = 0; i < MotrPstn[m].PstnValue.Length; i++)
                {
                    if (MotrPstn[m].PstnValue[i].bCommon)
                    {
                        IniCommon.Load(sSection, MotrPstn[m].PstnValue[i].sName, ref MotrPstn[m].PstnValue[i].dValue);
                    }
                    else
                    {
                        IniDevice.Load(sSection, MotrPstn[m].PstnValue[i].sName, ref MotrPstn[m].PstnValue[i].dValue);
                    }
                    IniMinMax.Load(sSection, MotrPstn[m].PstnValue[i].sName.Trim() + "_MIN", ref MotrPstn[m].PstnValue[i].dMin);
                    IniMinMax.Load(sSection, MotrPstn[m].PstnValue[i].sName.Trim() + "_MAX", ref MotrPstn[m].PstnValue[i].dMax);
                    IniMinMax.Load(sSection, MotrPstn[m].PstnValue[i].sName.Trim() + "_SPD", ref MotrPstn[m].PstnValue[i].iSpdPer);
                    
                    if (MotrPstn[m].PstnValue[i].dMax <= 0) MotrPstn[m].PstnValue[i].dMax = ML.MT_GetMaxPosition(m);
                    if (MotrPstn[m].PstnValue[i].iSpdPer <= 0 ) MotrPstn[m].PstnValue[i].iSpdPer = 100;
                    if (MotrPstn[m].PstnValue[i].iSpdPer > 100) MotrPstn[m].PstnValue[i].iSpdPer = 100;

                    //if (MotrPstn[m].PstnValue[i].dMax    <= 0 )
                    //ML.MT_GetInPosSgnl

                }
            }
        }
        public static void Save(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevicePath = sExeFolder + "JobFile\\" + _sJobName + "\\MotrPstn.ini";
            string sCommonPath = sExeFolder + "Util\\CommonPstn.ini";
            string sMinMaxPath = sExeFolder + "Util\\LimitPstn.ini";

            CIniFile IniDevice = new CIniFile(sDevicePath);
            CIniFile IniCommon = new CIniFile(sCommonPath);
            CIniFile IniMinMax = new CIniFile(sMinMaxPath);

            string sSection;

            //Save Device.
            for (int m = 0; m < m_iMaxMotr; m++)
            {
                //Set Dir.
                sSection = "Motor" + m.ToString();
                for (int i = 0; i < MotrPstn[m].PstnValue.Length; i++)
                {
                    if (MotrPstn[m].PstnValue[i].bCommon)
                    {
                        IniCommon.Save(sSection, MotrPstn[m].PstnValue[i].sName, MotrPstn[m].PstnValue[i].dValue);
                    }
                    else
                    {
                        IniDevice.Save(sSection, MotrPstn[m].PstnValue[i].sName, MotrPstn[m].PstnValue[i].dValue);
                    }
                    if (MotrPstn[m].PstnValue[i].dMax <= 0) MotrPstn[m].PstnValue[i].dMax = ML.MT_GetMaxPosition(m);
                    IniMinMax.Save(sSection, MotrPstn[m].PstnValue[i].sName.Trim() + "_MIN", MotrPstn[m].PstnValue[i].dMin);
                    IniMinMax.Save(sSection, MotrPstn[m].PstnValue[i].sName.Trim() + "_MAX", MotrPstn[m].PstnValue[i].dMax);
                    if (MotrPstn[m].PstnValue[i].iSpdPer <= 0 ) MotrPstn[m].PstnValue[i].iSpdPer = 100;
                    if (MotrPstn[m].PstnValue[i].iSpdPer > 100) MotrPstn[m].PstnValue[i].iSpdPer = 100;
                    IniMinMax.Save(sSection, MotrPstn[m].PstnValue[i].sName.Trim() + "_SPD", MotrPstn[m].PstnValue[i].iSpdPer);

                }
            }
        }


        public static void SetWindow(Panel _pnBase, int _iAxisNo)
        {
            MotrPstn[_iAxisNo].SetListviewParent(_pnBase);

        }

        public static void UpdatePstn(bool _bToTable)
        {
            if (_bToTable)
            {
                for (int m = 0; m < (int)mi.MAX_MOTR; m++)
                {
                    for (int i = 0; i < PstnCnt[m]; i++)
                    {
                        MotrPstn[m].lvPstnView.Items[i].SubItems[1].Text = MotrPstn[m].PstnValue[(int)i].sName;
                        MotrPstn[m].lvPstnView.Items[i].SubItems[1].Text = MotrPstn[m].PstnValue[(int)i].dValue.ToString();
                        MotrPstn[m].lvPstnView.Items[i].SubItems[5].Text = MotrPstn[m].PstnValue[(int)i].dMin.ToString();
                        MotrPstn[m].lvPstnView.Items[i].SubItems[7].Text = MotrPstn[m].PstnValue[(int)i].dMax.ToString();
                        MotrPstn[m].lvPstnView.Items[i].SubItems[9].Text = MotrPstn[m].PstnValue[(int)i].iSpdPer.ToString();
                    }
                }
            }

            else
            {
                for (int m = 0; m < (int)mi.MAX_MOTR; m++)
                {
                    for (int i = 0; i < PstnCnt[m]; i++)
                    {
                        MotrPstn[m].PstnValue[(int)i].sName   = MotrPstn[m].lvPstnView.Items[(int)i].SubItems[0].Text;
                        MotrPstn[m].PstnValue[(int)i].dValue  = Convert.ToDouble(MotrPstn[m].lvPstnView.Items[(int)i].SubItems[1].Text);
                        MotrPstn[m].PstnValue[(int)i].dMin    = Convert.ToDouble(MotrPstn[m].lvPstnView.Items[(int)i].SubItems[5].Text);
                        MotrPstn[m].PstnValue[(int)i].dMax    = Convert.ToDouble(MotrPstn[m].lvPstnView.Items[(int)i].SubItems[7].Text);
                        MotrPstn[m].PstnValue[(int)i].iSpdPer = Convert.ToInt32(MotrPstn[m].lvPstnView.Items[(int)i].SubItems[9].Text);
                    }
                }
            }
        }

        public static int GetMotrPosHeight()
        {
            return MotrPstn[0].iMotrMosCellSize;
        }

        public static int GetMotrPosWidth()
        {
            int iRet = MotrPstn[0].iNameWidth + MotrPstn[0].iValueWidth + MotrPstn[0].iGoWidth + MotrPstn[0].iInputWidth + (MotrPstn[0].iMinMaxWidthName * 2) + (MotrPstn[0].iMinMaxWidthVal * 2) +
                       MotrPstn[0].iSpeedPerName + MotrPstn[0].iSpeedPerVal;
            return iRet;
        }
    }

    /// <summary>
    /// 리스트뷰에 버튼 추가
    /// </summary>
    public class ListViewEx : ListView
    {
        #region Interop-Defines
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);

        // ListView messages
        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

        // Windows Messages
        private const int WM_PAINT = 0x000F;
        #endregion

        /// <summary>
        /// Structure to hold an embedded control's info
        /// </summary>
        private struct EmbeddedControl
        {
            public Control Control;
            public int Column;
            public int Row;
            public DockStyle Dock;
            public ListViewItem Item;
        }

        private ArrayList _embeddedControls = new ArrayList();

        public ListViewEx() { }

        /// <summary>
        /// Retrieve the order in which columns appear
        /// </summary>
        /// <returns>Current display order of column indices</returns>
        protected int[] GetColumnOrder()
        {
            IntPtr lPar = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);

            IntPtr res = SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lPar);
            if (res.ToInt32() == 0)	// Something went wrong
            {
                Marshal.FreeHGlobal(lPar);
                return null;
            }

            int[] order = new int[Columns.Count];
            Marshal.Copy(lPar, order, 0, Columns.Count);

            Marshal.FreeHGlobal(lPar);

            return order;
        }

        /// <summary>
        /// Retrieve the bounds of a ListViewSubItem
        /// </summary>
        /// <param name="Item">The Item containing the SubItem</param>
        /// <param name="SubItem">Index of the SubItem</param>
        /// <returns>Subitem's bounds</returns>
        protected Rectangle GetSubItemBounds(ListViewItem Item, int SubItem)
        {
            Rectangle subItemRect = Rectangle.Empty;
        
            if (Item == null)
                throw new ArgumentNullException("Item");
        
            int[] order = GetColumnOrder();
            if (order == null) // No Columns
                return subItemRect;
        
            if (SubItem >= order.Length)
                throw new IndexOutOfRangeException("SubItem " + SubItem + " out of range");
        
            // Retrieve the bounds of the entire ListViewItem (all subitems)
            Rectangle lviBounds = Item.GetBounds(ItemBoundsPortion.Entire);
            int subItemX = lviBounds.Left;
        
            // Calculate the X position of the SubItem.
            // Because the columns can be reordered we have to use Columns[order[i]] instead of Columns[i] !
            ColumnHeader col;
            int i;
            for (i = 0; i < order.Length; i++)
            {
                col = this.Columns[order[i]];
                if (col.Index == SubItem)
                    break;
                subItemX += col.Width;
            }
        
            subItemRect = new Rectangle(subItemX, lviBounds.Top, this.Columns[order[i]].Width, lviBounds.Height);
        
            return subItemRect;
        }

        /// <summary>
        /// Add a control to the ListView
        /// </summary>
        /// <param name="c">Control to be added</param>
        /// <param name="col">Index of column</param>
        /// <param name="row">Index of row</param>
        public void AddEmbeddedControl(Control c, int col, int row)
        {
            AddEmbeddedControl(c, col, row, DockStyle.Fill);
        }
        /// <summary>
        /// Add a control to the ListView
        /// </summary>
        /// <param name="c">Control to be added</param>
        /// <param name="col">Index of column</param>
        /// <param name="row">Index of row</param>
        /// <param name="dock">Location and resize behavior of embedded control</param>
        public void AddEmbeddedControl(Control c, int col, int row, DockStyle dock)
        {
            if (c == null)
                throw new ArgumentNullException();
            if (col >= Columns.Count || row >= Items.Count)
                throw new ArgumentOutOfRangeException();

            EmbeddedControl ec;
            ec.Control = c;
            ec.Column = col;
            ec.Row = row;
            ec.Dock = dock;
            ec.Item = Items[row];

            _embeddedControls.Add(ec);

            // Add a Click event handler to select the ListView row when an embedded control is clicked
            c.Click += new EventHandler(_embeddedControl_Click);

            this.Controls.Add(c);
        }

        public void MoveEmbeddedControl(Control c, int col, int row)
        {
            if (c == null)
                throw new ArgumentNullException();
            if (col >= Columns.Count || row >= Items.Count)
                throw new ArgumentOutOfRangeException();
            RemoveEmbeddedControl(c);
            AddEmbeddedControl(c, col, row);
            
        }

        /// <summary>
        /// Remove a control from the ListView
        /// </summary>
        /// <param name="c">Control to be removed</param>
        public void RemoveEmbeddedControl(Control c)
        {
            if (c == null)
                throw new ArgumentNullException();

            for (int i = 0; i < _embeddedControls.Count; i++)
            {
                EmbeddedControl ec = (EmbeddedControl)_embeddedControls[i];
                if (ec.Control == c)
                {
                    c.Click -= new EventHandler(_embeddedControl_Click);
                    this.Controls.Remove(c);
                    _embeddedControls.RemoveAt(i);
                    return;
                }
            }
            throw new Exception("Control not found!");
        }

        /// <summary>
        /// Retrieve the control embedded at a given location
        /// </summary>
        /// <param name="col">Index of Column</param>
        /// <param name="row">Index of Row</param>
        /// <returns>Control found at given location or null if none assigned.</returns>
        public Control GetEmbeddedControl(int col, int row)
        {
            foreach (EmbeddedControl ec in _embeddedControls)
                if (ec.Row == row && ec.Column == col)
                    return ec.Control;

            return null;
        }

        [DefaultValue(View.LargeIcon)]
        public new View View
        {
            get
            {
                return base.View;
            }
            set
            {
                // Embedded controls are rendered only when we're in Details mode
                foreach (EmbeddedControl ec in _embeddedControls)
                    ec.Control.Visible = (value == View.Details);

                base.View = value;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                    if (View != View.Details)
                        break;
        
                    // Calculate the position of all embedded controls
                    foreach (EmbeddedControl ec in _embeddedControls)
                    {
                        Rectangle rc = this.GetSubItemBounds(ec.Item, ec.Column);
        
                        if ((this.HeaderStyle != ColumnHeaderStyle.None) &&
                            (rc.Top < this.Font.Height)) // Control overlaps ColumnHeader
                        {
                            //ec.Control.Visible = false;
                            //continue;
                        }
                        else
                        {
                            //ec.Control.Visible = true;
                        }
        
                        switch (ec.Dock)
                        {
                            case DockStyle.Fill:
                                break;
                            case DockStyle.Top:
                                rc.Height = ec.Control.Height;
                                break;
                            case DockStyle.Left:
                                rc.Width = ec.Control.Width;
                                break;
                            case DockStyle.Bottom:
                                rc.Offset(0, rc.Height - ec.Control.Height);
                                rc.Height = ec.Control.Height;
                                break;
                            case DockStyle.Right:
                                rc.Offset(rc.Width - ec.Control.Width, 0);
                                rc.Width = ec.Control.Width;
                                break;
                            case DockStyle.None:
                                rc.Size = ec.Control.Size;
                                break;
                        }
        
                        // Set embedded control's bounds
                        ec.Control.Bounds = rc;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void _embeddedControl_Click(object sender, EventArgs e)
        {
            // When a control is clicked the ListViewItem holding it is selected
            foreach (EmbeddedControl ec in _embeddedControls)
            {
                if (ec.Control == (Control)sender)
                {
                    this.SelectedItems.Clear();
                    ec.Item.Selected = true;
                }
            }
        }
    }



}
