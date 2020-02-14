using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using COMMON;

namespace SMDll2
{
    
    public partial class FormDllMain: Form //UserControl
    {
        readonly int iIoVtrValCol = 3;
        readonly int iIoActValCol = 4;

        public string m_sId;
        public int m_iType;

        public FormDllMain(int _iWidth , int _iHeight , bool [] _bTabHides)
        {
            InitializeComponent();


            /*
             //Control Scale 변경
            float widthRatio = 1024f / 1280f;
            float heightRatio = 607f / 863f;
            SizeF scale = new SizeF(widthRatio, heightRatio);
            this.Scale(scale);

            foreach (Control control in this.Controls)
            {
                control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            }

            //TapControl Header 크기 변경
            this.tcMain.ItemSize = new Size(168, 25);

            this.Width  = 1288;
            this.Height = 904;


            InitTabErr ();
            InitTabDio ();
            //InitTabTLamp();
            //InitTabCylinder();
            InitTabMotr();

            //스케일 줄였더니 폰트 크기만 안바껴서 여기서 바꿈
            FontFamily ff = label2.Font.FontFamily;
            label2.Font = new Font(ff, 10);
             */

            //Control Scale 변경
            //시발 화면해상도에 리밋이 있어서 그냥 코딩으로 해야됌.
            //시발시발.
            const int  iWidth  = 1280;
            const int  iHeight = 863;


            float widthRatio  = _iWidth   / (float)iWidth;// this.ClientSize.Width;//1280f;
            float heightRatio = _iHeight  / (float)iHeight;//.ClientSize.Height; //863f ;

            SizeF scale = new SizeF(widthRatio, heightRatio);
            //this.Scale(scale);

            foreach (Control control in this.Controls)
            {
                control.Scale(scale); 
                //control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            }

            //TapControl Header 크기 변경
            this.tcMain.ItemSize = new Size((int)(this.tcMain.ItemSize.Width*widthRatio), (int)(this.tcMain.ItemSize.Height*heightRatio));


            //this.Width  = 1280;
            //this.Height = 904 ;

            InitTabErr ();
            InitTabDio ();
            InitTabTLamp();
            InitTabCylinder();
            InitTabMotr();

            if (_bTabHides.Length == 6) 
            {    
                if(_bTabHides[0])tcMain.TabPages.Remove(tpError    );
                if(_bTabHides[1])tcMain.TabPages.Remove(tpDio      );
                if(_bTabHides[2])tcMain.TabPages.Remove(tpTowerLamp);
                if(_bTabHides[3])tcMain.TabPages.Remove(tpCylinder );
                if(_bTabHides[4])tcMain.TabPages.Remove(tpMotor    );
                if(_bTabHides[5])tcMain.TabPages.Remove(tpMaster   );
            }
        }

        private void InitTabErr()
        {
            //Error ListView
            lvError.Clear();
            lvError.View = View.Details;
            lvError.LabelEdit = true;
            lvError.AllowColumnReorder = true;
            lvError.FullRowSelect = true;
            lvError.GridLines = true;
            lvError.Sorting = SortOrder.Ascending;

            lvError.Columns.Add("No"    , 40 , HorizontalAlignment.Left);
            lvError.Columns.Add("Enum"  , 100, HorizontalAlignment.Left);
            lvError.Columns.Add("Name"  , 215, HorizontalAlignment.Left);
            lvError.Columns.Add("Action", 370, HorizontalAlignment.Left);

            ListViewItem[] liError = new ListViewItem[SM.ER._iMaxErrCnt];
            for (int i = 0; i < SM.ER._iMaxErrCnt; i++)
            {
                liError[i] = new ListViewItem(string.Format("{0:000}", i));
                liError[i].SubItems.Add("");
                liError[i].SubItems.Add("");
                liError[i].SubItems.Add("");

                liError[i].UseItemStyleForSubItems = false;
                liError[i].UseItemStyleForSubItems = false;

                lvError.Items.Add(liError[i]);
            }

            var PropError = lvError.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropError.SetValue(lvError, true, null);

            
            DispErr();

            m_tErrTracker.Rect.X = 100;
            m_tErrTracker.Rect.Y = 100;
            m_tErrTracker.Rect.Width = 200;
            m_tErrTracker.Rect.Height = 200;



        }

        private void InitTabDio()
        {
            //Input ListView
            lvInput.Clear();
            lvInput.View = View.Details;
            lvInput.FullRowSelect = true;
            lvInput.GridLines = true;

            lvInput.Columns.Add("No"      , 35 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("Add"     , 60 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("HexAdd"  , 60 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("VVal"    , 40, HorizontalAlignment.Left);
            lvInput.Columns.Add("AVal"    , 40, HorizontalAlignment.Left);
            lvInput.Columns.Add("Name"    , 150,HorizontalAlignment.Left);
            lvInput.Columns.Add("Inv"     , 35 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("Comment" , 170,HorizontalAlignment.Left);
            lvInput.Columns.Add("OnDelay" , 55 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("OffDelay", 55, HorizontalAlignment.Left);
            
            

            ListViewItem [] liInput = new ListViewItem [SM.IO._iMaxIn];
            for (int i = 0; i < SM.IO._iMaxIn; i++)
            {
                liInput[i] = new ListViewItem(string.Format("{0:000}",i));
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");

                liInput[i].UseItemStyleForSubItems = false;
                liInput[i].UseItemStyleForSubItems = false;

                lvInput.Items.Add(liInput[i]);
            }

            var PropInput = lvInput.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvInput, true, null);

            //lvInput.SetDoubleBuffered(true);


            //Output ListView
            lvOutput.Clear();
            lvOutput.View = View.Details;
            lvOutput.FullRowSelect = true;
            lvOutput.GridLines = true;

            lvOutput.Columns.Add("No"      , 35 , HorizontalAlignment.Left);
            lvOutput.Columns.Add("Add"     , 60 , HorizontalAlignment.Left);
            lvOutput.Columns.Add("HexAdd"  , 60 , HorizontalAlignment.Left);
            lvOutput.Columns.Add("VVal", 40, HorizontalAlignment.Left);
            lvOutput.Columns.Add("AVal", 40, HorizontalAlignment.Left);
            lvOutput.Columns.Add("Name"    , 150, HorizontalAlignment.Left);
            lvOutput.Columns.Add("Inv"     , 35 , HorizontalAlignment.Left);
            lvOutput.Columns.Add("Comment" , 170, HorizontalAlignment.Left);
            lvOutput.Columns.Add("OnDelay" , 55 , HorizontalAlignment.Left);
            lvOutput.Columns.Add("OffDelay", 55 , HorizontalAlignment.Left);
            

            ListViewItem[] liOutput = new ListViewItem[SM.IO._iMaxOut];
            for (int i = 0; i < SM.IO._iMaxOut; i++)
            {
                liOutput[i] = new ListViewItem(string.Format("{0:000}",i));
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");

                liOutput[i].UseItemStyleForSubItems = false;
                liOutput[i].UseItemStyleForSubItems = false;

                lvOutput.Items.Add(liOutput[i]);
            }  

            
    
            var PropOutput = lvOutput.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropOutput.SetValue(lvOutput, true, null);
        }

        //타워램프 이니셜
        private void InitTabTLamp()
        {
            //TowerLamp ListView
            lvTLamp.Clear();
            lvTLamp.View = View.Details;
            lvTLamp.FullRowSelect = true;
            lvTLamp.GridLines = true;
        
            lvTLamp.Columns.Add("STATUS" , 100, HorizontalAlignment.Left);
            lvTLamp.Columns.Add("RED"    , 100, HorizontalAlignment.Left);
            lvTLamp.Columns.Add("YELLOW" , 100, HorizontalAlignment.Left);
            lvTLamp.Columns.Add("GREEN"  , 100, HorizontalAlignment.Left);
            lvTLamp.Columns.Add("SOUND"  , 100, HorizontalAlignment.Left);
        
            ListViewItem[] liTLamp = new ListViewItem[SM.TL._ListViewCnt];
        
            for (int i = 0; i < SM.TL._ListViewCnt; i++)
            {
                liTLamp[i] = new ListViewItem(string.Format("", i));
                liTLamp[i].SubItems.Add("");
                liTLamp[i].SubItems.Add("");
                liTLamp[i].SubItems.Add("");
                liTLamp[i].SubItems.Add("");
                
            
                liTLamp[i].UseItemStyleForSubItems = false;
                liTLamp[i].UseItemStyleForSubItems = false;
            
                lvTLamp.Items.Add(liTLamp[i]);
            }
        
            lvTLamp.Items[0].SubItems[0].Text = "Init"    ;
            lvTLamp.Items[1].SubItems[0].Text = "Warning" ;
            lvTLamp.Items[2].SubItems[0].Text = "Error"   ;
            lvTLamp.Items[3].SubItems[0].Text = "Running" ;
            lvTLamp.Items[4].SubItems[0].Text = "Stop"    ;
            lvTLamp.Items[5].SubItems[0].Text = "Maint"   ;
            lvTLamp.Items[6].SubItems[0].Text = "RunWarn" ;
            lvTLamp.Items[7].SubItems[0].Text = "LotEnd"  ;
        
            var PropTLamp = lvTLamp.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropTLamp.SetValue(lvTLamp, true, null);
        }

        //실린더 이니셜
        private void InitTabCylinder()
        {
            //Cylinder ListView
            lvCylinder.Clear();
            lvCylinder.View = View.Details;
            lvCylinder.FullRowSelect = true;
            lvCylinder.GridLines = true;
        
            lvCylinder.Columns.Add("No"      , 100, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("Name"    , 150, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("Comment" , 220, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("xFwdID"  , 100, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("xBwdID"  , 100, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("yFwdID"  , 100, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("yBwdID"  , 100, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("FwdOD"   , 100, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("BwdOD"   , 100, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("FwdTO"   , 100, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("BwdTO"   , 100, HorizontalAlignment.Left);
        
            ListViewItem[] liCylinder = new ListViewItem[SM.CL._iMaxCylinder];
        
            for (int i = 0; i < SM.CL._iMaxCylinder; i++)
            {
                liCylinder[i] = new ListViewItem(string.Format("{0:000}", i));
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
                liCylinder[i].SubItems.Add("");
        
                
              
                liCylinder[i].UseItemStyleForSubItems = false;
                liCylinder[i].UseItemStyleForSubItems = false;
        
                lvCylinder.Items.Add(liCylinder[i]);
            }
        
            
            var PropCylinder = lvCylinder.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropCylinder.SetValue(lvCylinder, true, null);
        }

        private void InitTabMotr()
        {
            cbMotrSel.Items.Clear();
            for (int i = 0; i < SM.MT._iMaxMotr; i++)
            {
                if (SM.MT.Para[i].sMotrName == null)
                {
                    cbMotrSel.Items.Add(string.Format("Motor{0:00}", i));
                }
                else
                {
                    cbMotrSel.Items.Add(SM.MT.Para[i].sMotrName);
                }

            }

            if (SM.MT._iMaxMotr > 0)
            {
                cbMotrSel.SelectedIndex = 0;
            }
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            int iPageSel = tcMain.SelectedIndex;
            
            //Error
            if (iPageSel == 0)
            {

            }
                
            //IO
            else if(iPageSel == 1)
            {
                for (int i = 0; i < SM.IO._iMaxIn; i++)
                {
                    lvInput.Items[i].SubItems[iIoVtrValCol].Text = SM.IO.GetX(i) ? "ON" : "OFF";
                    lvInput.Items[i].SubItems[iIoVtrValCol].BackColor = SM.IO.GetX(i) ? Color.Lime : Color.Silver;

                    lvInput.Items[i].SubItems[iIoActValCol].Text = SM.IO.GetX(i, true) ? "ON" : "OFF";
                    lvInput.Items[i].SubItems[iIoActValCol].BackColor = SM.IO.GetX(i, true) ? Color.Lime : Color.Silver;

                }

                for (int i = 0; i < SM.IO._iMaxOut; i++)
                {
                    lvOutput.Items[i].SubItems[iIoVtrValCol].Text      = SM.IO.m_aOut[i].Stat.bVtrVal ? "ON" : "OFF";
                    lvOutput.Items[i].SubItems[iIoVtrValCol].BackColor = SM.IO.m_aOut[i].Stat.bVtrVal ? Color.Lime : Color.Silver;
                
                    lvOutput.Items[i].SubItems[iIoActValCol].Text = SM.IO.GetY(i, true) ? "ON" : "OFF";
                    lvOutput.Items[i].SubItems[iIoActValCol].BackColor = SM.IO.GetY(i, true) ? Color.Lime : Color.Silver;
                }      
 
                if (lvInput.SelectedIndices.Count <= 0) return;
                int iIOSel = 0 ;

                if (tsOutput.SelectedIndex == 0)
                {
                    //iIOSel = lvInput.SelectedIndices[0];
                    //pnUpEdge  .BackColor = SM.IO.GetXUp((int)iIOSel) ? Color.Lime : Color.Gray ;
                    //pnDownEdge.BackColor = SM.IO.GetXDn((int)iIOSel) ? Color.Lime : Color.Gray ;

                }
                else {
                    //lvOutput. 
                    //iIOSel = lvOutput.SelectedIndices[0];
                    //pnUpEdge  .BackColor = SM.IO.GetYUp((int)iIOSel) ? Color.Lime : Color.Gray ;
                    //pnDownEdge.BackColor = SM.IO.GetYDn((int)iIOSel) ? Color.Lime : Color.Gray ;
                }

                


                


            }

            //TowerLamp
            else if (iPageSel == 2)
            {

            }

            //Cylinder
            else if (iPageSel == 3)
            {

            }

            //Motor
            else if (iPageSel == 4)
            {
                if(m_iPreCrntLevel != FormPassword.m_iCrntLevel)  DispMotr();

                int iMotrSel = cbMotrSel.SelectedIndex;

                lbStop    .BackColor = SM.MT.GetStop      (iMotrSel) ? Color.Lime:Color.Silver;
                lbInpos   .BackColor = SM.MT.GetInPosSgnl (iMotrSel) ? Color.Lime:Color.Silver;
                lbServo   .BackColor = SM.MT.GetServo     (iMotrSel) ? Color.Lime:Color.Silver;
                lbHomeDone.BackColor = SM.MT.GetHomeDone  (iMotrSel) ? Color.Lime:Color.Silver;
                lbBreakOff.BackColor = SM.MT.GetBreakOff  (iMotrSel) ? Color.Lime:Color.Silver;
                lbAlram   .BackColor = SM.MT.GetAlarmSgnl (iMotrSel) ? Color.Lime:Color.Silver;
                lbDirP    .BackColor = SM.MT.Stat[iMotrSel].bMovingP ? Color.Lime:Color.Silver;
                lbDirN    .BackColor = SM.MT.Stat[iMotrSel].bMovingN ? Color.Lime:Color.Silver;
                lbZPhase  .BackColor = SM.MT.GetZphaseSgnl(iMotrSel) ? Color.Lime:Color.Silver;
                lbLimitP  .BackColor = SM.MT.GetPLimSnsr  (iMotrSel) ? Color.Lime:Color.Silver;
                lbHome    .BackColor = SM.MT.GetHomeSnsr  (iMotrSel) ? Color.Lime:Color.Silver;
                lbLimitN  .BackColor = SM.MT.GetNLimSnsr  (iMotrSel) ? Color.Lime:Color.Silver;

                lbCmdPos  .Text      = SM.MT.GetCmdPos(iMotrSel).ToString();
                lbEncPos  .Text      = SM.MT.GetEncPos(iMotrSel).ToString();
                lbTrgPos  .Text      = SM.MT.GetTrgPos(iMotrSel).ToString();
            }

            //Master
            else if (iPageSel == 5)
            {

            }
        }

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iPageSel = tcMain.SelectedIndex;
            DispForm(iPageSel);
            cbDioTestMode.Checked = false;
            SM.IO.SetTestMode(false);
            
            
        }

        private void DispForm(int _iPageIndex)
        {
            
            
            //Error
            if (_iPageIndex == 0)
            {
                DispErr();
            }
            //IO
            else if (_iPageIndex == 1)
            {
                DispDio();
            }
            //TowerLamp
            else if (_iPageIndex == 2)
            {
                int iMotrSel = cbMotrSel.SelectedIndex;

                DispMotr();
                DispTLamp();
            }

            //Cylinder
            else if (_iPageIndex == 3)
            {
                DispCylinder();
            }

            //Motor
            else if (_iPageIndex == 4)
            {
                
            }

            //Master
            else if (_iPageIndex == 5)
            {

            }
        }


        private void DispErr()
        {
            for (int i = 0; i < SM.ER._iMaxErrCnt; i++)
            {
                lvError.Items[i].SubItems[1].Text = SM.ER.GetErrConfig(i).sEnum;
                lvError.Items[i].SubItems[2].Text = SM.ER.GetErrConfig(i).sName;
                lvError.Items[i].SubItems[3].Text = SM.ER.GetErrConfig(i).sAction;
            }           
        }

        private void DispErrProp(int _iSelNo)
        {
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);

            tbErrNo.Text   = _iSelNo.ToString();
            tbErrEnum.Text = SM.ER.GetErrConfig(_iSelNo).sEnum;
            tbErrName.Text = SM.ER.GetErrConfig(_iSelNo).sName;
            tbErrAct.Text  = SM.ER.GetErrConfig(_iSelNo).sAction;
            tbErrImg.Text  = SM.ER.GetErrConfig(_iSelNo).sImgPath;

            m_tErrTracker.Rect.X      = (int)SM.ER.GetErrConfig(_iSelNo).dRectLeft;
            m_tErrTracker.Rect.Y      = (int)SM.ER.GetErrConfig(_iSelNo).dRectTop;
            m_tErrTracker.Rect.Width  = (int)SM.ER.GetErrConfig(_iSelNo).dRectWidth;
            m_tErrTracker.Rect.Height = (int)SM.ER.GetErrConfig(_iSelNo).dRectHeight;

            if (tbErrImg.Text.Trim() == "")
            {
                pbErrImg.Image = null;
                return;
            }

            FileInfo Info = new FileInfo(tbErrImg.Text);

            if (!Info.Exists)
            {
                Log.ShowMessage("File Not Exist", tbErrImg.Text);
                return;
            }

            try
            {
                pbErrImg.Image = Image.FromFile(tbErrImg.Text);
            }
            catch (Exception e)
            {
                Log.ShowMessage("Exception", e.Message);
                throw new FileNotFoundException(e.Message);
            }

            this.pbErrImg.SizeMode = PictureBoxSizeMode.StretchImage;

            Graphics g = Graphics.FromImage(pbErrImg.Image);

            pbErrImg.Refresh();
        }

        private void lvError_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvError.SelectedIndices.Count <= 0) return;
            int iErrorSel = lvError.SelectedIndices[0];
            DispErrProp(iErrorSel);
        }

        private void btSaveErr_Click(object sender, EventArgs e)
        {
            SM.ER.LoadSave(false);
        }

        private void btErrApply_Click(object sender, EventArgs e)
        {
            if (tbErrNo.Text != "")
            {
                int iSelNo = Convert.ToInt32(tbErrNo.Text);

                CErrMan.TPara Para = SM.ER.GetErrConfig(iSelNo);
                Para.sEnum = tbErrEnum.Text;
                Para.sName = tbErrName.Text;
                Para.sAction = tbErrAct.Text;
                Para.sImgPath = tbErrImg.Text;

                Para.dRectLeft = m_tErrTracker.Rect.X;
                Para.dRectTop = m_tErrTracker.Rect.Y;
                Para.dRectWidth = m_tErrTracker.Rect.Width;
                Para.dRectHeight = m_tErrTracker.Rect.Height;

                SM.ER.SetErrConfig(iSelNo, Para);

                DispErr();
            }
        }

        private void btOpenImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenImg = new OpenFileDialog();
            OpenImg.InitialDirectory = @"./";
            OpenImg.Filter = "그림 파일 (*.jpg, *.gif, *.bmp) | *.jpg; *.gif; *.bmp; | 모든 파일 (*.*) | *.*";
            OpenImg.FilterIndex = 1;
            OpenImg.RestoreDirectory = true;
            if (OpenImg.ShowDialog() == DialogResult.OK)
            {
                string fileFullName = OpenImg.FileName;
                tbErrImg.Text = fileFullName;

                pbErrImg.ImageLocation = fileFullName;

                this.pbErrImg.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void DispDio()
        {
            for (int i = 0; i < SM.IO._iMaxIn; i++)
            {
                lvInput.Items[i].SubItems[1].Text = string.Format("X{0:000}", SM.IO.GetInputPara(i).iAdd);
                lvInput.Items[i].SubItems[2].Text = string.Format("X{0:X3}", SM.IO.GetInputPara(i).iAdd);
                lvInput.Items[i].SubItems[5].Text = SM.IO.GetInputPara(i).sName;
                lvInput.Items[i].SubItems[6].Text = SM.IO.GetInputPara(i).bInv ? "O" : "X";
                lvInput.Items[i].SubItems[7].Text = SM.IO.GetInputPara(i).SComt;
                lvInput.Items[i].SubItems[8].Text = string.Format("{0:0}", SM.IO.GetInputPara(i).iOnDelay);
                lvInput.Items[i].SubItems[9].Text = string.Format("{0:0}", SM.IO.GetInputPara(i).iOffDelay);
            }

            for (int i = 0; i < SM.IO._iMaxOut; i++)
            {
                //sItem = string.Format("X{0:000}", SM.IO.GetInputPara(i).iAdd);
                //lvInput.Items[i].SubItems[1].Text = sItem;
                lvOutput.Items[i].SubItems[1].Text = string.Format("X{0:000}", SM.IO.GetOutputPara(i).iAdd);
                lvOutput.Items[i].SubItems[2].Text = string.Format("X{0:X3}", SM.IO.GetOutputPara(i).iAdd);
                lvOutput.Items[i].SubItems[5].Text = SM.IO.GetOutputPara(i).sName;
                lvOutput.Items[i].SubItems[6].Text = SM.IO.GetOutputPara(i).bInv ? "O" : "X";
                lvOutput.Items[i].SubItems[7].Text = SM.IO.GetOutputPara(i).SComt;
                lvOutput.Items[i].SubItems[8].Text = string.Format("{0:0}", SM.IO.GetOutputPara(i).iOnDelay);
                lvOutput.Items[i].SubItems[9].Text = string.Format("{0:0}", SM.IO.GetOutputPara(i).iOffDelay);
            }
        }

        private void DispDioInputProp(int _iSelNo)
        {
            tbDioNo.Text = _iSelNo.ToString();
            tbDioEnum.Text = SM.IO.GetInputPara(_iSelNo).sEnum;
            tbDioName.Text = SM.IO.GetInputPara(_iSelNo).sName;
            tbDioAdd.Text = SM.IO.GetInputPara(_iSelNo).iAdd.ToString();
            tbDioComment.Text = SM.IO.GetInputPara(_iSelNo).SComt;
            tbDioOnDelay.Text = SM.IO.GetInputPara(_iSelNo).iOnDelay.ToString();
            tbDioOffDelay.Text = SM.IO.GetInputPara(_iSelNo).iOffDelay.ToString();
            cbDioInv.Checked = SM.IO.GetInputPara(_iSelNo).bInv;
        }

        private void DispDioOutputProp(int _iSelNo)
        {
            tbDioNo.Text = _iSelNo.ToString();
            tbDioEnum.Text = SM.IO.GetOutputPara(_iSelNo).sEnum;
            tbDioName.Text = SM.IO.GetOutputPara(_iSelNo).sName;
            tbDioAdd.Text = SM.IO.GetOutputPara(_iSelNo).iAdd.ToString();
            tbDioComment.Text = SM.IO.GetOutputPara(_iSelNo).SComt;
            tbDioOnDelay.Text = SM.IO.GetOutputPara(_iSelNo).iOnDelay.ToString();
            tbDioOffDelay.Text = SM.IO.GetOutputPara(_iSelNo).iOffDelay.ToString();
            cbDioInv.Checked = SM.IO.GetOutputPara(_iSelNo).bInv;
        }

        private void lvInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvInput.SelectedIndices.Count <= 0) return;
            int iInputSel = lvInput.SelectedIndices[0];
            DispDioInputProp(iInputSel);
        }

        private void lvOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvOutput.SelectedIndices.Count <= 0) return;
            int iOutputSel = lvOutput.SelectedIndices[0];
            DispDioOutputProp(iOutputSel);
        }

        private void btDioApply_Click(object sender, EventArgs e)
        {
            if (tbDioNo.Text != "")
            {
                int iSelNo = Convert.ToInt32(tbDioNo.Text);

                if (tsOutput.SelectedIndex == 0)
                {
                    //Para.iAdd = CIniFile.StrToIntDef(tbDioAdd.Text, 0);
                    CDioMan.TPara InPara = SM.IO.GetInputPara(iSelNo);
                    InPara.iAdd = Convert.ToInt32(tbDioAdd.Text);
                    InPara.iOffDelay = Convert.ToInt32(tbDioOffDelay.Text);
                    InPara.iOnDelay = Convert.ToInt32(tbDioOnDelay.Text);
                    InPara.SComt = tbDioComment.Text;
                    InPara.sName = tbDioName.Text;
                    InPara.bInv = cbDioInv.Checked;

                    SM.IO.SetInputPara(iSelNo, InPara);
                }
                if (tsOutput.SelectedIndex == 1)
                {
                    CDioMan.TPara OutPara = SM.IO.GetOutputPara(iSelNo);
                    OutPara.iAdd = Convert.ToInt32(tbDioAdd.Text);
                    OutPara.iOffDelay = Convert.ToInt32(tbDioOffDelay.Text);
                    OutPara.iOnDelay = Convert.ToInt32(tbDioOnDelay.Text);
                    OutPara.SComt = tbDioComment.Text;
                    OutPara.sName = tbDioName.Text;
                    OutPara.bInv = cbDioInv.Checked;

                    SM.IO.SetOutputPara(iSelNo, OutPara);

                }
                DispDio();
            }

        }

        private void btSaveIO_Click(object sender, EventArgs e)
        {
            SM.IO.LoadSave(false);
        }


        //타워램프 리스트뷰 디스플레이
        private void DispTLamp()
        {
            for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
            {
                lvTLamp.Items[i].SubItems[1].Text = SM.TL.GetTLampInfo((EN_SEQ_STAT)i).iRed.ToString();
                lvTLamp.Items[i].SubItems[2].Text = SM.TL.GetTLampInfo((EN_SEQ_STAT)i).iYel.ToString();
                lvTLamp.Items[i].SubItems[3].Text = SM.TL.GetTLampInfo((EN_SEQ_STAT)i).iGrn.ToString();
                lvTLamp.Items[i].SubItems[4].Text = SM.TL.GetTLampInfo((EN_SEQ_STAT)i).iBuzz.ToString();
            }
        }

        //타워램프 프로퍼티 디스플레이
        private void DispTLampProp(EN_SEQ_STAT _eStat)
        {
            _eStat = (EN_SEQ_STAT)lvTLamp.SelectedIndices[0];
        
            cbRedSeq.SelectedIndex = (int)SM.TL.GetTLampInfo(_eStat).iRed;
            cbYelSeq.SelectedIndex = (int)SM.TL.GetTLampInfo(_eStat).iYel;
            cbGrnSeq.SelectedIndex = (int)SM.TL.GetTLampInfo(_eStat).iGrn;
            cbSndSeq.SelectedIndex = (int)SM.TL.GetTLampInfo(_eStat).iBuzz;
        
            tbRedAdd.Text = SM.TL.GetTLampPara().iRedAdd.ToString();
            tbYelAdd.Text = SM.TL.GetTLampPara().iYelAdd.ToString();
            tbGrnAdd.Text = SM.TL.GetTLampPara().iGrnAdd.ToString();
            tbSntAdd.Text = SM.TL.GetTLampPara().iSndAdd.ToString();
        }

        //타워램프 리스트뷰 인덱스 선택하면 실행

        //타워램프 Apply 버튼 클릭이벤트
        private void btTLampApply_Click(object sender, EventArgs e)
        {
            if (tbRedAdd.Text != "")
            {
                EN_SEQ_STAT eStat = (EN_SEQ_STAT)lvTLamp.SelectedIndices[0];

                CTowerLampMan.TPara Para = SM.TL.GetTLampPara();
                CTowerLampMan.TLampInfo Info = SM.TL.GetTLampInfo(eStat);

                Para.iRedAdd = Convert.ToInt32(tbRedAdd.Text);
                Para.iYelAdd = Convert.ToInt32(tbYelAdd.Text);
                Para.iGrnAdd = Convert.ToInt32(tbGrnAdd.Text);
                Para.iSndAdd = Convert.ToInt32(tbSntAdd.Text);

                SM.TL.SetTLampPara(Para);

                Info.iRed = (CTowerLampMan.EN_LAMP_OPER)cbRedSeq.SelectedIndex;
                Info.iYel = (CTowerLampMan.EN_LAMP_OPER)cbYelSeq.SelectedIndex;
                Info.iGrn = (CTowerLampMan.EN_LAMP_OPER)cbGrnSeq.SelectedIndex;
                Info.iBuzz = (CTowerLampMan.EN_LAMP_OPER)cbSndSeq.SelectedIndex;

                SM.TL.SetTLampInfo(eStat, Info);

                DispTLamp();
            }
        }

        private void btSaveTLamp_Click(object sender, EventArgs e)
        {
            SM.TL.LoadSave(false);
        }

        private void cbTestMode_CheckedChanged(object sender, EventArgs e)
        {
            SM.TL.SetTestMode(true);
        }

        //실린더 리스트뷰 디스플레이
        private void DispCylinder()
        {
            for (int i = 0; i < SM.CL._iMaxCylinder; i++)
            {
                lvCylinder.Items[i].SubItems[1] .Text = SM.CL.GetCylinderPara(i).sName;
                lvCylinder.Items[i].SubItems[2] .Text = SM.CL.GetCylinderPara(i).sComment;
                lvCylinder.Items[i].SubItems[3] .Text = string.Format("{0:0}", SM.CL.GetCylinderPara(i).iFwdXAdd   );
                lvCylinder.Items[i].SubItems[4] .Text = string.Format("{0:0}", SM.CL.GetCylinderPara(i).iBwdXAdd   );
                lvCylinder.Items[i].SubItems[5] .Text = string.Format("{0:0}", SM.CL.GetCylinderPara(i).iFwdYAdd   );
                lvCylinder.Items[i].SubItems[6] .Text = string.Format("{0:0}", SM.CL.GetCylinderPara(i).iBwdYAdd   );
                lvCylinder.Items[i].SubItems[7] .Text = string.Format("{0:0}", SM.CL.GetCylinderPara(i).iFwdOnDelay);
                lvCylinder.Items[i].SubItems[8] .Text = string.Format("{0:0}", SM.CL.GetCylinderPara(i).iBwdOnDelay);
                lvCylinder.Items[i].SubItems[9] .Text = string.Format("{0:0}", SM.CL.GetCylinderPara(i).iFwdTimeOut);
                lvCylinder.Items[i].SubItems[10].Text = string.Format("{0:0}", SM.CL.GetCylinderPara(i).iBwdTimeOut);
            }
        }

        //실린더 프로퍼티 디스플레이
        private void DispCylinderProp(int _iSelNo)
        {
            //string sTemp = "";
        
            tbCylNo      .Text = _iSelNo.ToString();
            tbCylEnum    .Text = SM.CL.GetCylinderPara(_iSelNo).sEnum;
            tbCylName    .Text = SM.CL.GetCylinderPara(_iSelNo).sName;
            tbCylComt    .Text = SM.CL.GetCylinderPara(_iSelNo).sComment;
            tbCylAddrIF  .Text = SM.CL.GetCylinderPara(_iSelNo).iFwdXAdd.ToString();
            tbCylAddrIB  .Text = SM.CL.GetCylinderPara(_iSelNo).iBwdXAdd.ToString();
            tbCylAddrOF  .Text = SM.CL.GetCylinderPara(_iSelNo).iFwdYAdd   .ToString();
            tbCylAddrOB  .Text = SM.CL.GetCylinderPara(_iSelNo).iBwdYAdd   .ToString();
            tbCylOnDelayF.Text = SM.CL.GetCylinderPara(_iSelNo).iFwdOnDelay.ToString();
            tbCylOnDelayB.Text = SM.CL.GetCylinderPara(_iSelNo).iBwdOnDelay.ToString();
            tbCylTimeOutF.Text = SM.CL.GetCylinderPara(_iSelNo).iFwdTimeOut.ToString();
            tbCylTimeOutB.Text = SM.CL.GetCylinderPara(_iSelNo).iBwdTimeOut.ToString();
            cbDirc.SelectedIndex = (int)SM.CL.GetCylinderPara(_iSelNo).eDirType;
            //tbCylAddrIF_.Text = sTemp.sprintf("DI%d_%02d", iIOModuleNo + 1, iIOOffset);
            //tbCylAddrIB_.Text = SM.IO.GetInputPara(_iSelNo).iBwdTimeOut.ToString();
        
        
        }

        //실린더 리스트뷰 인덱스 선택시 실행
        private void lvCylinder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvCylinder.SelectedIndices.Count <= 0) return;
            int iCylSel = lvCylinder.SelectedIndices[0];
            DispCylinderProp(iCylSel);
        }

        //실린더 Apply 버튼 클릭 이벤트
        private void btCylApply_Click(object sender, EventArgs e)
        {
            //string sXAdd = "";
            if (tbCylNo.Text != "")
            {
                int iSelNo = Convert.ToInt32(tbCylNo.Text);

                CCylinder.TPara Para = SM.CL.GetCylinderPara(iSelNo);
                Para.sEnum = tbCylEnum.Text;
                Para.sName = tbCylName.Text;
                Para.sComment = tbCylComt.Text;
                Para.iFwdXAdd = Convert.ToInt32(tbCylAddrIF.Text);
                Para.iBwdXAdd = Convert.ToInt32(tbCylAddrIB.Text);
                Para.iFwdYAdd = Convert.ToInt32(tbCylAddrOF.Text);
                Para.iBwdYAdd = Convert.ToInt32(tbCylAddrOB.Text);
                Para.iFwdOnDelay = Convert.ToInt32(tbCylOnDelayF.Text);
                Para.iBwdOnDelay = Convert.ToInt32(tbCylOnDelayB.Text);
                Para.iFwdTimeOut = Convert.ToInt32(tbCylTimeOutF.Text);
                Para.iBwdTimeOut = Convert.ToInt32(tbCylTimeOutB.Text);
                Para.eDirType = (EN_MOVE_DIRECTION)cbDirc.SelectedIndex;

                SM.CL.SetCylinderPara(iSelNo, Para);

                DispCylinder();
            }
               
        }

        private void btSaveCyl_Click(object sender, EventArgs e)
        {
            SM.CL.LoadSave(false);
        }

        private EN_LEVEL m_iPreCrntLevel = EN_LEVEL.lvOperator;

        private void DispMotr()
        {
            //ComboBox.ObjectCollection MotrItems ;
            //MotrItems = new ComboBox.ObjectCollection();
            if (SM.MT._iMaxMotr < 1) return;

            int iMotrSel = cbMotrSel.SelectedIndex;

            pgMotrPara   .SelectedObject = SM.MT.Para[iMotrSel];
            pgMotrParaSub.SelectedObject = SM.MT.Mtr [iMotrSel].GetPara();

            if (FormPassword.m_iCrntLevel == EN_LEVEL.lvMaster)
            {
                pgMotrPara   .Enabled = true;
                pgMotrParaSub.Enabled = true;
            }
            else
            {
                pgMotrPara   .Enabled = false;
                pgMotrParaSub.Enabled = false;
            }
            m_iPreCrntLevel = FormPassword.m_iCrntLevel;
        }

        private void cbMotrSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            DispMotr();
        }

        private void btSaveMotr_Click(object sender, EventArgs e)
        {
            SM.MT.ApplyParaAll();
            SM.MT.LoadSaveAll(false);
        }

        private void FormDllMain_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }

        private void lbEncPos_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            lbStop    .BackColor = SM.MT.GetStop      (iMotrSel) ? Color.Lime:Color.Silver;
            lbInpos   .BackColor = SM.MT.GetInPosSgnl (iMotrSel) ? Color.Lime:Color.Silver;
            lbServo   .BackColor = SM.MT.GetServo     (iMotrSel) ? Color.Lime:Color.Silver;
            lbHomeDone.BackColor = SM.MT.GetHomeDone  (iMotrSel) ? Color.Lime:Color.Silver;
            lbBreakOff.BackColor = SM.MT.GetBreakOff  (iMotrSel) ? Color.Lime:Color.Silver;
            lbAlram   .BackColor = SM.MT.GetAlarmSgnl (iMotrSel) ? Color.Lime:Color.Silver;
            lbDirP    .BackColor = SM.MT.Stat[iMotrSel].bMovingP ? Color.Lime:Color.Silver;
            lbDirN    .BackColor = SM.MT.Stat[iMotrSel].bMovingN ? Color.Lime:Color.Silver;
            lbZPhase  .BackColor = SM.MT.GetZphaseSgnl(iMotrSel) ? Color.Lime:Color.Silver;
            lbLimitP  .BackColor = SM.MT.GetPLimSnsr  (iMotrSel) ? Color.Lime:Color.Silver;
            lbHome    .BackColor = SM.MT.GetHomeSnsr  (iMotrSel) ? Color.Lime:Color.Silver;
            lbLimitN  .BackColor = SM.MT.GetNLimSnsr  (iMotrSel) ? Color.Lime:Color.Silver;

            lbCmdPos  .Text      = SM.MT.GetCmdPos(iMotrSel).ToString();
            lbEncPos  .Text      = SM.MT.GetEncPos(iMotrSel).ToString();
            lbTrgPos  .Text      = SM.MT.GetTrgPos(iMotrSel).ToString();
        }

        //private void btJobN_Click(object sender, EventArgs e)
        //{
            
        //}

        private void btJogP_MouseDown(object sender, MouseEventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.JogP(iMotrSel);
        }

        private void btJogP_MouseUp(object sender, MouseEventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.Stop(iMotrSel);
        }

        private void btJobN_MouseDown(object sender, MouseEventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.JogN(iMotrSel);
        }

        private void btJobN_MouseUp(object sender, MouseEventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.Stop(iMotrSel);
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.Stop(iMotrSel);
        }

        private void btServoOn_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
             
            SM.MT.SetServo(iMotrSel,true);
        }

        private void btServoOff_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetServo(iMotrSel, false);
        }

        private void btAllStop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SM.MT._iMaxMotr; i++ )
                SM.MT.Stop(i);
        }

        private void btServoOnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SM.MT._iMaxMotr; i++)
                SM.MT.SetServo(i,true);
        }

        private void btServoOffAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SM.MT._iMaxMotr; i++)
                SM.MT.SetServo(i, false);
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.GoHome(iMotrSel);
        }

        private void btClearPos_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetPos(iMotrSel, 0.0);
        }

        private void btGo1stPos_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.GoAbsRepeatFst(iMotrSel);
        }

        private void btGo2ndPos_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.GoAbsRepeatScd(iMotrSel);
        }

        private void btStop2_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.Stop(iMotrSel);
        }

        private void btRepeat_Click(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.StartRepeat(iMotrSel);
        }

        private void btReset_MouseDown(object sender, MouseEventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetReset(iMotrSel, true);

        }

        private void btReset_MouseUp(object sender, MouseEventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetReset(iMotrSel, false);
        }

        

        private struct TErrTracker
        {
            public const int iThickness = 6 ;
            public Rectangle Rect      ;
            public bool      bDown     ;
            public EClickPos eClickPos ;
             
            public int       iPosX     ;
            public int       iPosY     ;
            public int       iPrePosX  ;
            public int       iPrePosY  ;
        }

        TErrTracker m_tErrTracker ;

        bool CheckRectIn(int _iLeftX, int _iTopY,int _iRightX, int _iBottomY, int _iX, int _iY) //사각포인트 중간크기를 넘김.
        {
            return _iX > _iLeftX && _iX <= _iRightX &&
                   _iY > _iTopY  && _iY <= _iBottomY;
        }
        bool CheckRectShapeIn(Rectangle _tRect , int _iX, int _iY, double _dPntHfSize)
        {
            Rectangle OutRect = new Rectangle ();
            Rectangle InRect  = new Rectangle ();

            OutRect.X      = _tRect.X - (int)_dPntHfSize;
            OutRect.Y      = _tRect.Y - (int)_dPntHfSize ;
            OutRect.Width  = _tRect.Width  + (int)_dPntHfSize * 2 ;
            OutRect.Height = _tRect.Height + (int)_dPntHfSize * 2 ;

            InRect .X      = _tRect.X + (int)_dPntHfSize ;
            InRect .Y      = _tRect.Y + (int)_dPntHfSize ;
            InRect .Width  = _tRect.Width  - (int)_dPntHfSize * 2 ;
            InRect .Height = _tRect.Height - (int)_dPntHfSize * 2 ;

            if (!CheckRectIn(OutRect.Left, OutRect.Top, OutRect.Right, OutRect.Bottom, _iX, _iY))
            {
                 return false ;
            }

            if (CheckRectIn(InRect.Left, InRect.Top, InRect.Right, InRect.Bottom, _iX, _iY))
            {
                return false ;
            }

            return true ;

        }

        private void pbErrImg_MouseDown(object sender, MouseEventArgs e)
        {
            m_tErrTracker.bDown = true;

            if (m_tErrTracker.Rect.IsEmpty)
            {
                m_tErrTracker.Rect.X = e.X ;
                m_tErrTracker.Rect.Y = e.Y ;
            }

            int iCntX = (m_tErrTracker.Rect.Left+m_tErrTracker.Rect.Right )/2 ;
            int iCntY = (m_tErrTracker.Rect.Top +m_tErrTracker.Rect.Bottom)/2 ;
            const int iThick = TErrTracker.iThickness ;

                 if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, m_tErrTracker.Rect.Top    - iThick, m_tErrTracker.Rect.Left  + iThick, m_tErrTracker.Rect.Top    + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.cpLeftTop;
            else if (CheckRectIn(iCntX                    - iThick, m_tErrTracker.Rect.Top    - iThick, iCntX                    + iThick, m_tErrTracker.Rect.Top    + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.cpTop;
            else if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, m_tErrTracker.Rect.Top    - iThick, m_tErrTracker.Rect.Right + iThick, m_tErrTracker.Rect.Top    + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.cpRightTop;
            else if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, iCntY                     - iThick, m_tErrTracker.Rect.Right + iThick, iCntY                     + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.cpRight;
            else if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, m_tErrTracker.Rect.Bottom - iThick, m_tErrTracker.Rect.Right + iThick, m_tErrTracker.Rect.Bottom + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.cpRightBottom;
            else if (CheckRectIn(iCntX                    - iThick, m_tErrTracker.Rect.Bottom - iThick, iCntX                    + iThick, m_tErrTracker.Rect.Bottom + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.cpBottom;
            else if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, m_tErrTracker.Rect.Bottom - iThick, m_tErrTracker.Rect.Left  + iThick, m_tErrTracker.Rect.Bottom + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.cpLeftBottom;
            else if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, iCntY                     - iThick, m_tErrTracker.Rect.Left  + iThick, iCntY                     + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.cpLeft;
            else if (CheckRectShapeIn(m_tErrTracker.Rect, e.X, e.Y, TErrTracker.iThickness / 2.0)) m_tErrTracker.eClickPos = EClickPos.cpMove;

            else if (!CheckRectShapeIn(m_tErrTracker.Rect, e.X, e.Y, TErrTracker.iThickness / 2.0)) m_tErrTracker.eClickPos = EClickPos.cpNone;

            m_tErrTracker.iPrePosX = e.X ;
            m_tErrTracker.iPrePosY = e.Y ;
        }

        private void pbErrImg_MouseMove(object sender, MouseEventArgs e)
        {
            
            int x, y;

            x = e.X - m_tErrTracker.iPrePosX;
            y = e.Y - m_tErrTracker.iPrePosY;

            if(m_tErrTracker.bDown)
            {
                if(m_tErrTracker.eClickPos == EClickPos.cpMove)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.cpLeftTop)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width - x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.cpTop)
                {
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.cpRightTop)
                {
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.cpRight)
                {
                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                  

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.cpRightBottom)
                {
                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.cpBottom)
                {
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.cpLeftBottom)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;


                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width - x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.cpLeft)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;


                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width - x;


                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }
            }
            pbErrImg.Refresh();
        }

        private void pbErrImg_MouseUp(object sender, MouseEventArgs e)
        {
            m_tErrTracker.bDown = false;
        }

        private void pbErrImg_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Pen RP = new Pen(Color.Red, 6);
            g.DrawRectangle(RP, m_tErrTracker.Rect.X, m_tErrTracker.Rect.Y, m_tErrTracker.Rect.Width, m_tErrTracker.Rect.Height);
            RP.Dispose();
            //g.Dispose();
        }

        private void lvOutput_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SM.IO.GetTestMode()) SM.IO.SetYTestMode(lvOutput.SelectedItems[0].Index, !SM.IO.GetY(lvOutput.SelectedItems[0].Index));
            else                     SM.IO.SetY(lvOutput.SelectedItems[0].Index, !SM.IO.GetY(lvOutput.SelectedItems[0].Index), cbDioDirect.Checked);

            for (int i = 0; i < SM.IO._iMaxOut; i++)
            {
                lvOutput.Items[i].SubItems[iIoVtrValCol].Text      = SM.IO.m_aOut[i].Stat.bVtrVal ? "ON" : "OFF";
                lvOutput.Items[i].SubItems[iIoVtrValCol].BackColor = SM.IO.m_aOut[i].Stat.bVtrVal ? Color.Lime : Color.Silver;

                lvOutput.Items[i].SubItems[iIoActValCol].Text      = SM.IO.GetY(i, true) ? "ON" : "OFF";
                lvOutput.Items[i].SubItems[iIoActValCol].BackColor = SM.IO.GetY(i, true) ? Color.Lime : Color.Silver;
            }

        }

        private void lvTLamp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvTLamp.SelectedIndices.Count <= 0) return;
            EN_SEQ_STAT eStat = (EN_SEQ_STAT)lvTLamp.SelectedIndices[0];
            DispTLampProp(eStat);
        }

        private void cbDioTestMode_CheckedChanged(object sender, EventArgs e)
        {
            SM.IO.SetTestMode(cbDioTestMode.Checked);
        }

        

        //public string OnGetAge()
        //{
        //   //sQuery "CompanyName", "FileDescription", "FileVersion", "InternalName", "LegalCopyright", "LegalTradeMarks", "OriginalFileName", "ProductName", "ProductVersion", "Comments"
        //
        //   string filename = Application->ExeName;
        //   string filepath = "";
        //   string dllname  = "";
        //
        //   while(filename.Pos("\\")){
        //       filepath += filename.SubString(1,filename.Pos("\\")) ;
        //       filename.Delete(1,filename.Pos("\\"));
        //   }
        //
        //   dllname = filepath + "SMDll.dll" ;
        //
        //
        //   int Age = FileAge(dllname);
        //   string Date = FileDateToDateTime(Age).FormatString("''yyyy'. 'm'. 'd'. 'AM/PM' 'hh': 'nn'");
        //
        //   return Date ;
        //}
    }
}
