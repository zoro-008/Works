using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection;
using COMMON;
using SMDll2;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

//using System.Runtime.InteropServices;
//using SMDll2.CXmlBase;
//using SMD2Define;
//using SMDll2App;

namespace Machine

{
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

    public struct TPstnValue
    {
        public string        sName   ; //포지션 이름
        public bool          bOffset ; //상대값 포지션인지 오프셑인지.
        public bool          bFix    ; //수정 불가 포지션
        public bool          bCommon ; //전디바이스 공용.
        public double        dValue  ; //값.
    };

    public delegate bool   dgCheckSafe(uint m ,uint i);
    public delegate double dgGetCmdPos(int  m);

    public class MotrPstn
    {
        uint m_uMaxPstn ;
        uint m_uMotrId  ;
        public TPstnValue[] PstnValue;

        dgCheckSafe   CheckSafe  ;
        dgGetCmdPos   GetCmdPos  ;

        public ListViewEx    lvPstnView ;
        TextBox       tbPstnEdit ;
        Button     [] btPstnGo   ;
        Button     [] btPstnInput;
        public MotrPstn(uint _uMotrId , uint _uPstnCnt)
        {
            m_uMotrId  = _uMotrId  ; 
            m_uMaxPstn = _uPstnCnt ;
            PstnValue = new TPstnValue[_uPstnCnt];

            lvPstnView  = new ListViewEx();

         
            lvPstnView.Clear();
            lvPstnView.View = View.Details;
            lvPstnView.FullRowSelect = true;
            lvPstnView.GridLines     = true;

            //셀높이 조절하기 편법.
            ImageList dummyImage = new ImageList();
            dummyImage.ImageSize = new System.Drawing.Size(1,30);
            lvPstnView.SmallImageList = dummyImage;

            //1280*1024 해상도에서 Column 폭
            lvPstnView.Columns.Add("Name"  , 150 ,HorizontalAlignment.Left);
            lvPstnView.Columns.Add("Value" , 100 ,HorizontalAlignment.Left);
            lvPstnView.Columns.Add("Go"    ,  55, HorizontalAlignment.Left);
            lvPstnView.Columns.Add("Input" , 75, HorizontalAlignment.Left);

            //1024*768 해상도에서 Column 폭
            //lvPstnView.Columns.Add("Name"    , 60, HorizontalAlignment.Left);
            //lvPstnView.Columns.Add("Value"   , 70, HorizontalAlignment.Left);
            //lvPstnView.Columns.Add("Go"      , 35, HorizontalAlignment.Left);
            //lvPstnView.Columns.Add("Input"   , 55, HorizontalAlignment.Left);

            lvPstnView.HeaderStyle = ColumnHeaderStyle.None ;

            ListViewItem [] liInput = new ListViewItem [_uPstnCnt];

            var PropInput = lvPstnView.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvPstnView, true, null);

            btPstnGo    = new Button[_uPstnCnt];
            btPstnInput = new Button[_uPstnCnt];
 
            for (int i = 0; i < _uPstnCnt; i++)
            {
                liInput[i] = new ListViewItem(string.Format("{0:000}",i));
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");

                liInput[i].UseItemStyleForSubItems = false; //요건 셀별 색깔 넣을때.
                liInput[i].UseItemStyleForSubItems = false;

                lvPstnView.Items.Add(liInput[i]);

                btPstnGo   [i] = new Button();
                btPstnInput[i] = new Button();

                btPstnGo   [i].Tag = i;
                btPstnInput[i].Tag = i; 

                btPstnGo   [i].Text = "GO";
                btPstnInput[i].Text = "INPUT";

                btPstnGo   [i].BackColor = Color.Aqua ;
                btPstnInput[i].BackColor = Color.Lime ;

                lvPstnView.AddEmbeddedControl(btPstnGo   [i] , 2,i);
                lvPstnView.AddEmbeddedControl(btPstnInput[i] , 3, i);

                lvPstnView.Items[i].SubItems[0].BackColor = Color.Silver;

                btPstnGo[i].Click    += new EventHandler(btGoClick);
                btPstnInput[i].Click += new EventHandler(btInputClick);


            }
            tbPstnEdit  = new TextBox();
            
            tbPstnEdit.Visible = false ;
            tbPstnEdit.BorderStyle = BorderStyle.FixedSingle ;
            tbPstnEdit.Tag     = 0 ;
            tbPstnEdit.Leave += tbPstnEdit_Leave ;
            
            tbPstnEdit.KeyDown += tbPstnEdit_KeyDown ;


            lvPstnView.AddEmbeddedControl(tbPstnEdit , 1,0);

            lvPstnView.Click    += new EventHandler(lvPstnView_Click);
            
            //for (int i = 0; i < _uMotrId; i++)
            //{
            //    btPstnGo[_uPstnCnt].Click += new EventHandler(btGoClick);
            //    btPstnInput[_uPstnCnt].Click += new EventHandler(btInputClick);
            //}
            //lvPstnView.DrawItem += new DrawListViewItemEventHandler(lvPstnView_DrawItem);
            //lvPstnView.OwnerDraw = true ;

        } 

        
        private void tbPstnEdit_Leave(object sender, EventArgs e)
        {
            TextBox Tb = sender as TextBox ;
            int iPosId = (int)Tb.Tag  ;

            //lvPstnView.RemoveEmbeddedControl(tbPstnEdit);
            //Tb
            Tb.Visible = false ;

            double dVal;
            if(!double.TryParse(Tb.Text,out dVal)) 
            {
                return ;
            }
            SetValue((uint)iPosId , dVal );
        }

        private void tbPstnEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                double dVal ;
                int iPosId = (int)tbPstnEdit.Tag ;
                tbPstnEdit.Visible = false;
                if (!double.TryParse(tbPstnEdit.Text, out dVal))
                {
                    return;
                }
                SetValue((uint)iPosId, dVal);
            }
            else if(e.KeyCode == Keys.Escape)
            {
                tbPstnEdit.Text = "";
                tbPstnEdit.Visible = false ;
            }

        }



        private void lvPstnView_Click(object sender, EventArgs e)
		{
			ListView Lv = sender as ListView ;
            
            Point mousePos = Lv.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = Lv.HitTest(mousePos);
            if(hitTest.Item == null) return ;
            int row = hitTest.Item.Index ;
            int col = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);

            if(col == 1)
            {
                lvPstnView.MoveEmbeddedControl(tbPstnEdit , 1,row);

                tbPstnEdit.Visible = true ;
                tbPstnEdit.Text = GetValue((uint)row).ToString();
                tbPstnEdit.Tag = row ;
                tbPstnEdit.Select();;//Focus();
                
            }
        }

        private void btGoClick(object sender, EventArgs e)
		{
            //Button Bt = sender as Button;
            //uint uPstnNo = (uint)Bt.Tag;

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;
            if (Log.ShowMessageModal("Confirm", "Do you want to Part Move?") != DialogResult.Yes) return;
            //if (MessageBox.Show(new Form{TopMost = true},"Part를 이동하시겠습니까?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            if (CheckSafe != null)
            {
                if(!CheckSafe(m_uMotrId , uPstnNo)) return ;
            }

            SM.MT.GoAbsRun((int)m_uMotrId, PM.GetValue(m_uMotrId, uPstnNo));
           
		}
        private void btInputClick(object sender, EventArgs e)
		{
			//Button Bt = sender as Button;
            //uint uPstnNo = (uint)Bt.Tag;

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;

            if (GetCmdPos != null)
            {
                SetValue(uPstnNo , GetCmdPos((int)m_uMotrId));
            }
		}

        public void SetProp(uint _uPstnNo , string _sName , bool _bOffset , bool _bFix , bool _bCommon)
        {
            PstnValue[_uPstnNo].sName   = _sName   ; 
            PstnValue[_uPstnNo].bOffset = _bOffset ;
            PstnValue[_uPstnNo].bFix    = _bFix    ;
            PstnValue[_uPstnNo].bCommon = _bCommon ;

            if (lvPstnView.Parent != null)
            {
                //UI에 접근 하기 위한 인보크
                if (lvPstnView.InvokeRequired)
                {
                    lvPstnView.Invoke(new MethodInvoker(delegate()
                    {
                        // code for running in ui thread
                        btPstnGo[_uPstnNo].Visible = !_bOffset;
                        btPstnInput[_uPstnNo].Visible = !_bFix && !_bOffset;
                        lvPstnView.Items[(int)_uPstnNo].SubItems[0].Text = _sName;
                        lvPstnView.Items[(int)_uPstnNo].SubItems[0].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                    }));
                }

                else
                {
                    btPstnGo[_uPstnNo].Visible = !_bOffset;
                    btPstnInput[_uPstnNo].Visible = !_bFix && !_bOffset;
                    lvPstnView.Items[(int)_uPstnNo].SubItems[0].Text = _sName;
                    lvPstnView.Items[(int)_uPstnNo].SubItems[0].BackColor = _bCommon ? Color.LightGray : Color.Silver;
                }
                
            }
        }

        public double GetValue(uint _uPstnNo)
        {
            return PstnValue[_uPstnNo].dValue ;
        }

        public void SetValue(uint _uPstnNo , double _dValue )
        {
            PstnValue[_uPstnNo].dValue = _dValue ;
            //UI에 접근 하기 위한 인보크
            if (lvPstnView.Parent != null)
            {
                if(lvPstnView.InvokeRequired)
                {
                    lvPstnView.Invoke(new MethodInvoker(delegate()
                    {
                    // code for running in ui thread
                    lvPstnView.Items[(int)_uPstnNo].SubItems[1].Text = _dValue.ToString();
                    }));
                }
                else
                {
                    lvPstnView.Items[(int)_uPstnNo].SubItems[1].Text = _dValue.ToString();

                }
            }
        }

        public void SetCheckSafe(dgCheckSafe _dgCheckSafe)
        {
            CheckSafe = _dgCheckSafe ;

        }
        public void SetGetCmdPos(dgGetCmdPos _dgGetCmdPos)
        {
            GetCmdPos = _dgGetCmdPos ;
        }

        public void SetListviewParent(Panel _pnBase)
        {
            lvPstnView.Parent = _pnBase ;
            lvPstnView.Dock = DockStyle.Fill ;

            // code for running in ui thread
            for (int i = 0; i < m_uMaxPstn; i++)
            {
                btPstnGo    [i].Visible = !PstnValue[i].bOffset ;
                btPstnInput [i].Visible = !PstnValue[i].bFix && !PstnValue[i].bOffset  ;
                lvPstnView.Items[(int)i].SubItems[0].Text      = PstnValue[(int)i].sName;
                lvPstnView.Items[(int)i].SubItems[0].BackColor = PstnValue[(int)i].bCommon ? Color.LightGray : Color.Silver;
                lvPstnView.Items[(int)i].SubItems[1].Text      = PstnValue[(int)i].dValue.ToString();

            }

        }

    }

  
    public static class PM
    {
        static uint        m_iMaxMotr ;
        static MotrPstn[] MotrPstn   ;

        public static uint[] PstnCnt = { (uint)pv.MAX_PSTN_MOTR0, (uint)pv.MAX_PSTN_MOTR1, (uint)pv.MAX_PSTN_MOTR2, (uint)pv.MAX_PSTN_MOTR3 };

        public static void Init(uint [] _uPstnCnts)          
        {
            m_iMaxMotr = (uint)_uPstnCnts.Length ;


            MotrPstn  = new MotrPstn  [m_iMaxMotr] ;
            for(uint i = 0 ; i < m_iMaxMotr ;i++)
            {
                MotrPstn[i] = new MotrPstn(i ,_uPstnCnts[i]);
            }
        }
        public static void SetProp(uint _uAxisNo , uint _uPstnNo , string _sName , bool _bOffset , bool _bFix , bool _bCommon)
        {
            MotrPstn[_uAxisNo].SetProp(_uPstnNo , _sName , _bOffset , _bFix , _bCommon);
        }

        public static void SetCheckSafe(uint _uAxisNo , dgCheckSafe _dgCheckSafe)
        {
             MotrPstn[_uAxisNo].SetCheckSafe(_dgCheckSafe) ;

        }
        public static void SetGetCmdPos(uint _uAxisNo , dgGetCmdPos _dgGetCmdPos)
        {
            MotrPstn[_uAxisNo].SetGetCmdPos(_dgGetCmdPos) ;
        }

        public static double GetValue(uint _uAxisNo , uint _uPstnNo)
        {
            return MotrPstn[_uAxisNo].GetValue(_uPstnNo);
        }

        public static double GetValue(mi _eAxisNo , pv _ePstnNo)
        {
            return GetValue((uint)_eAxisNo ,(uint)_ePstnNo);
        }

        public static void SetValue(uint _uAxisNo , uint _uPstnNo , double _dValue )
        {
            MotrPstn[_uAxisNo].SetValue(_uPstnNo , _dValue);
        }

        public static void SetValue(mi _uAxisNo , pv _uPstnNo , double _dValue )
        {
            SetValue((uint) _uAxisNo , (uint) _uPstnNo , _dValue );
        }

        public static void Load(string _sJobName)
        {
            string sExeFolder  = System.AppDomain.CurrentDomain.BaseDirectory; 
            string sDevicePath = sExeFolder + "JobFile\\" + _sJobName+"\\MotrPstn.ini";
            string sCommonPath = sExeFolder + "Util\\CommonPstn.ini" ;

            CIniFile IniDevice = new CIniFile(sDevicePath);
            CIniFile IniCommon = new CIniFile(sCommonPath);

            string sSection ;
            
            //Save Device.
            for(int m = 0 ; m < m_iMaxMotr ; m++) {
                //Set Dir.
                sSection = "Motor"+m.ToString() ;
                for(int i = 0 ; i < MotrPstn[m].PstnValue.Length ; i++)
                {
                    if(MotrPstn[m].PstnValue[i].bCommon)
                        IniCommon.Load(sSection , MotrPstn[m].PstnValue[i].sName , out MotrPstn[m].PstnValue[i].dValue);
                    else 
                        IniDevice.Load(sSection , MotrPstn[m].PstnValue[i].sName , out MotrPstn[m].PstnValue[i].dValue);
                }
            }
        }
        public static void Save(string _sJobName)
        {
            string sExeFolder  = System.AppDomain.CurrentDomain.BaseDirectory; 
            string sDevicePath = sExeFolder + "JobFile\\" + _sJobName+"\\MotrPstn.ini";
            string sCommonPath = sExeFolder + "Util\\CommonPstn.ini" ;

            CIniFile IniDevice = new CIniFile(sDevicePath);
            CIniFile IniCommon = new CIniFile(sCommonPath);

            string sSection ;
            
            //Save Device.
            for(int m = 0 ; m < m_iMaxMotr ; m++) {
                //Set Dir.
                sSection = "Motor"+m.ToString() ;
                for(int i = 0 ; i < MotrPstn[m].PstnValue.Length ; i++)
                {
                    if(MotrPstn[m].PstnValue[i].bCommon)
                        IniCommon.Save(sSection , MotrPstn[m].PstnValue[i].sName , MotrPstn[m].PstnValue[i].dValue);
                    else 
                        IniDevice.Save(sSection , MotrPstn[m].PstnValue[i].sName , MotrPstn[m].PstnValue[i].dValue);
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
                for (uint m = 0; m < (uint)mi.MAX_MOTR; m++)
                {
                    for (int i = 0; i < PstnCnt[m]; i++)
                    {
                        MotrPstn[m].lvPstnView.Items[i].SubItems[1].Text = MotrPstn[m].PstnValue[(int)i].sName;
                        MotrPstn[m].lvPstnView.Items[i].SubItems[1].Text = GetValue(m, (uint)i).ToString();
                    }
                }
            }
        
            else
            {
                for (uint m = 0; m < (uint)mi.MAX_MOTR; m++)
                {
                    for (uint i = 0; i < PstnCnt[m]; i++)
                    {
                        MotrPstn[m].PstnValue[(int)i].sName  = MotrPstn[m].lvPstnView.Items[(int)i].SubItems[0].Text;
                        MotrPstn[m].PstnValue[(int)i].dValue = Convert.ToDouble(MotrPstn[m].lvPstnView.Items[(int)i].SubItems[1].Text);
                    }
                }
            }
        
            
        }

    }

}
