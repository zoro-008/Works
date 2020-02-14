﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using COMMON;
using System.Text;

namespace SML
{
    
    public partial class FormDllMain: Form //UserControl
    {
        readonly int iIoVtrValCol = 3;
        readonly int iIoActValCol = 4;

        public string m_sId;
        public int m_iType;

        public bool    m_bSetCntrlPn;
        public TextBox tbStCaption;
        public Button  btBwd;
        public Button  btFwd;

        private const string sFormText = "Form Dll Main ";

        public FormDllMain(int _iWidth , int _iHeight , bool [] _bTabHides)
        {
            InitializeComponent();

            SM.ER.pnSize.iHeight = pbErrImg.Height ;
            SM.ER.pnSize.iWidth  = pbErrImg.Width  ;
            
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
            InitTabAio ();
            InitTabTLamp();
            InitTabCylinder();
            InitTabMotr();

            if (_bTabHides.Length == 6) 
            {    
                if(_bTabHides[0])tcMain.TabPages.Remove(tpError    );
                if(_bTabHides[1])tcMain.TabPages.Remove(tpDio      );
                if(_bTabHides[2])tcMain.TabPages.Remove(tpAio      );
                if(_bTabHides[3])tcMain.TabPages.Remove(tpTowerLamp);
                if(_bTabHides[4])tcMain.TabPages.Remove(tpCylinder );
                if(_bTabHides[5])tcMain.TabPages.Remove(tpMotor    );
                
            }
        }

        private void InitTabErr()
        {
            //Error ListView
            lvError.Clear();
            lvError.View = View.Details;
            //lvError.LabelEdit = true;
            lvError.AllowColumnReorder = true;
            lvError.FullRowSelect = true;
            lvError.GridLines = true;
            lvError.Sorting = SortOrder.Ascending;

            lvError.Columns.Add("No"    , 40 , HorizontalAlignment.Left);
            lvError.Columns.Add("Enum"  , 115, HorizontalAlignment.Left);
            lvError.Columns.Add("Name"  , 170, HorizontalAlignment.Left);
            lvError.Columns.Add("Action", 280, HorizontalAlignment.Left);

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

            //this.listView1.Items[0].Selected = true; 
            if(SM.ER._iMaxErrCnt >0)lvError.Items[0].Selected = true ;
            
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
            lvInput.Columns.Add("Add"     , 45 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("Hex"     , 45 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("VVal"    , 40 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("AVal"    , 40 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("Name"    , 150,HorizontalAlignment.Left);
            lvInput.Columns.Add("Inv"     , 22 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("NotLog"  , 22 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("Comment" , 95 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("OnDelay" , 50 ,HorizontalAlignment.Left);
            lvInput.Columns.Add("OffDelay", 50 ,HorizontalAlignment.Left);
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
                liInput[i].SubItems.Add("");

                liInput[i].UseItemStyleForSubItems = false;
                liInput[i].UseItemStyleForSubItems = false;

                lvInput.Items.Add(liInput[i]);

            }

            var PropInput = lvInput.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvInput, true, null);

            if(SM.IO._iMaxIn >0)lvInput.Items[0].Selected = true ;

            //lvInput.SetDoubleBuffered(true);


            //Output ListView
            lvOutput.Clear();
            lvOutput.View = View.Details;
            lvOutput.FullRowSelect = true;
            lvOutput.GridLines = true;

            lvOutput.Columns.Add("No"      , 35 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("Add"     , 45 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("Hex"     , 45 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("VVal"    , 40 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("AVal"    , 40 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("Name"    , 150,HorizontalAlignment.Left);
            lvOutput.Columns.Add("Inv"     , 22 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("NotLog"  , 22 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("Comment" , 95 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("OnDelay" , 50 ,HorizontalAlignment.Left);
            lvOutput.Columns.Add("OffDelay", 50 ,HorizontalAlignment.Left);
            

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
                liOutput[i].SubItems.Add("");

                liOutput[i].UseItemStyleForSubItems = false;
                liOutput[i].UseItemStyleForSubItems = false;

                lvOutput.Items.Add(liOutput[i]);
            }      
            var PropOutput = lvOutput.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropOutput.SetValue(lvOutput, true, null);

            if(SM.IO._iMaxOut >0)lvOutput.Items[0].Selected = true ;
        }

        //
        private void InitTabAio()
        {
            //Input ListView
            lvInput_A.Clear();
            lvInput_A.View = View.Details;
            lvInput_A.FullRowSelect = true;
            lvInput_A.GridLines = true;
           
            
            lvInput_A.Columns.Add("No"      , 35 ,HorizontalAlignment.Left);
            lvInput_A.Columns.Add("Add"     , 45 ,HorizontalAlignment.Left);
            lvInput_A.Columns.Add("Hex"     , 45 ,HorizontalAlignment.Left);
            lvInput_A.Columns.Add("Name"    , 150,HorizontalAlignment.Left);
            lvInput_A.Columns.Add("Comment" , 225,HorizontalAlignment.Left);
            lvInput_A.Columns.Add("AVal"    , 60, HorizontalAlignment.Left);
            lvInput_A.Columns.Add("DVal"    , 60, HorizontalAlignment.Left);

            ListViewItem [] liInput = new ListViewItem [SM.AIO._iMaxIn];
            for (int i = 0; i < SM.AIO._iMaxIn; i++)
            {
                liInput[i] = new ListViewItem(string.Format("{0:000}",i));
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");

                liInput[i].UseItemStyleForSubItems = false;
                liInput[i].UseItemStyleForSubItems = false;

                lvInput_A.Items.Add(liInput[i]);

            }

            var PropInput = lvInput_A.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvInput_A, true, null);

            if(SM.AIO._iMaxIn >0) lvInput_A.Items[0].Selected = true ;

            //lvInput_A.SetDoubleBuffered(true);


            //Output ListView
            lvOutput_A.Clear();
            lvOutput_A.View = View.Details;
            lvOutput_A.FullRowSelect = true;
            lvOutput_A.GridLines = true;

            lvOutput_A.Columns.Add("No"      , 35 , HorizontalAlignment.Left);
            lvOutput_A.Columns.Add("Add"     , 50 , HorizontalAlignment.Left);
            lvOutput_A.Columns.Add("Hex"     , 50 , HorizontalAlignment.Left);
            lvOutput_A.Columns.Add("Name"    , 150, HorizontalAlignment.Left);
            lvOutput_A.Columns.Add("Comment" , 335, HorizontalAlignment.Left);

            ListViewItem[] liOutput = new ListViewItem[SM.AIO._iMaxOut];
            for (int i = 0; i < SM.AIO._iMaxOut; i++)
            {
                liOutput[i] = new ListViewItem(string.Format("{0:000}",i));
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");
                liOutput[i].SubItems.Add("");

                liOutput[i].UseItemStyleForSubItems = false;
                liOutput[i].UseItemStyleForSubItems = false;

                lvOutput_A.Items.Add(liOutput[i]);
            }      
            var PropOutput = lvOutput_A.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropOutput.SetValue(lvOutput_A, true, null);

            if(SM.AIO._iMaxOut >0)lvOutput_A.Items[0].Selected = true ;
        }

        //타워램프 이니셜
        private void InitTabTLamp()
        {
            //TowerLamp ListView
            lvTLamp.Clear();
            lvTLamp.View = View.Details;
            lvTLamp.FullRowSelect = true;
            lvTLamp.GridLines = true;
        
            lvTLamp.Columns.Add("STATUS" , 183, HorizontalAlignment.Left);
            lvTLamp.Columns.Add("RED"    , 110, HorizontalAlignment.Left);
            lvTLamp.Columns.Add("YELLOW" , 110, HorizontalAlignment.Left);
            lvTLamp.Columns.Add("GREEN"  , 110, HorizontalAlignment.Left);
            lvTLamp.Columns.Add("SOUND"  , 110, HorizontalAlignment.Left);
        
            ListViewItem[] liTLamp = new ListViewItem[(int)EN_SEQ_STAT.MAX_SEQ_STAT];
        
            for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
            {
                liTLamp[i] = new ListViewItem(string.Format("", i));
                liTLamp[i].SubItems.Add("");
                liTLamp[i].SubItems.Add("");
                liTLamp[i].SubItems.Add("");
                liTLamp[i].SubItems.Add("");
                
            
                liTLamp[i].UseItemStyleForSubItems = false;
                liTLamp[i].UseItemStyleForSubItems = false;
            
                lvTLamp.Items.Add(liTLamp[i]);
                lvTLamp.Items[i].SubItems[0].Text = ((EN_SEQ_STAT)i).ToString();
            }
       
        
            var PropTLamp = lvTLamp.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropTLamp.SetValue(lvTLamp, true, null);

            if((int)EN_SEQ_STAT.MAX_SEQ_STAT >0)lvTLamp.Items[0].Selected = true ;

        }

        //실린더 이니셜
        private void InitTabCylinder()
        {
            //Cylinder ListView
            lvCylinder.Clear();
            lvCylinder.View = View.Details;
            lvCylinder.FullRowSelect = true;
            lvCylinder.GridLines = true;
        
            lvCylinder.Columns.Add("No"      , 35 , HorizontalAlignment.Left);
            lvCylinder.Columns.Add("Name"    , 150, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("Comment" , 220, HorizontalAlignment.Left);
            lvCylinder.Columns.Add("xFwdID"  , 60 , HorizontalAlignment.Left);
            lvCylinder.Columns.Add("xBwdID"  , 60 , HorizontalAlignment.Left);
            lvCylinder.Columns.Add("yFwdID"  , 60 , HorizontalAlignment.Left);
            lvCylinder.Columns.Add("yBwdID"  , 60 , HorizontalAlignment.Left);
            lvCylinder.Columns.Add("FwdOD"   , 60 , HorizontalAlignment.Left);
            lvCylinder.Columns.Add("BwdOD"   , 60 , HorizontalAlignment.Left);
            lvCylinder.Columns.Add("FwdTO"   , 60 , HorizontalAlignment.Left);
            lvCylinder.Columns.Add("BwdTO"   , 60 , HorizontalAlignment.Left);
        
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
                liCylinder[i].SubItems.Add("");                
              
                liCylinder[i].UseItemStyleForSubItems = false;
                liCylinder[i].UseItemStyleForSubItems = false;
        
                lvCylinder.Items.Add(liCylinder[i]);
            }       
            
            var PropCylinder = lvCylinder.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropCylinder.SetValue(lvCylinder, true, null);
            if(SM.CL._iMaxCylinder >0)lvCylinder.Items[0].Selected = true ;

            m_bSetCntrlPn = false;
            tbStCaption   = null;
            btBwd         = null;
            btFwd         = null;
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
            tmUpdate.Enabled = false;

            int iPageSel = tcMain.SelectedIndex;
            string sPageSel = tcMain.TabPages[iPageSel].Name;
            
            //Error
            if (sPageSel == "tpError")
            {

            }

            //IO
            else if (sPageSel == "tpDio")
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
                    lvOutput.Items[i].SubItems[iIoVtrValCol].Text = SM.IO.m_aOut[i].Stat.bVtrVal ? "ON" : "OFF";
                    lvOutput.Items[i].SubItems[iIoVtrValCol].BackColor = SM.IO.m_aOut[i].Stat.bVtrVal ? Color.Lime : Color.Silver;

                    lvOutput.Items[i].SubItems[iIoActValCol].Text = SM.IO.GetY(i, true) ? "ON" : "OFF";
                    lvOutput.Items[i].SubItems[iIoActValCol].BackColor = SM.IO.GetY(i, true) ? Color.Lime : Color.Silver;
                }

                if (lvInput.SelectedIndices.Count <= 0) return;
                int iIOSel = 0;

                if (tsOutput.SelectedIndex == 0)
                {
                    //iIOSel = lvInput.SelectedIndices[0];
                    //pnUpEdge  .BackColor = SM.IO.GetXUp((int)iIOSel) ? Color.Lime : Color.Gray ;
                    //pnDownEdge.BackColor = SM.IO.GetXDn((int)iIOSel) ? Color.Lime : Color.Gray ;

                }
                else
                {
                    //lvOutput. 
                    //iIOSel = lvOutput.SelectedIndices[0];
                    //pnUpEdge  .BackColor = SM.IO.GetYUp((int)iIOSel) ? Color.Lime : Color.Gray ;
                    //pnDownEdge.BackColor = SM.IO.GetYDn((int)iIOSel) ? Color.Lime : Color.Gray ;
                }
            }

            //AIO
            else if (sPageSel == "tpAio")
            {
                for (int i = 0; i < SM.AIO._iMaxIn; i++)
                {
                    double dTemp1 = SM.AIO.GetX(i);
                    double dTemp2 = SM.AIO.GetX(i,true);
                    //if (dTemp != 0)
                    //{
                        lvInput_A.Items[i].SubItems[5].Text = dTemp1.ToString();
                        lvInput_A.Items[i].SubItems[5].BackColor = Color.Silver;
                      
                    //}
                    //dTemp = SM.AIO.GetX(i, true);
                    //if (dTemp != 0)
                    //{
                        lvInput_A.Items[i].SubItems[6].Text = dTemp2.ToString();
                        lvInput_A.Items[i].SubItems[6].BackColor = Color.Silver;
                    //}

                }


            }
            //TowerLamp
            else if (sPageSel == "tpTowerLamp")
            {
                lvTLamp.Refresh();
            }

            //Cylinder
            else if (sPageSel == "tpCylinder")
            {
                if (tbCylNo.Text != "")
                {
                    int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                    SM.CL.DisplayStatus(iSelNo, lbFwdStat, lbBwdStat, lbAlarm);
                }


                //string sTemp1 = tbCylAddrIF_.Text;
                //string sTemp2 = tbCylAddrIB_.Text;
                //string sTemp3 = tbCylAddrOF_.Text;
                //string sTemp4 = tbCylAddrOF_.Text;

                //if (CConfig.StrToIntDef(sTemp1,-1) != -1)
                //{
                //    for (int i = 0; i < SM.IO._iMaxIn; i++)
                //    {
                //        SM.IO.GetInModuleNo(sTemp1.ToIntDef(-1), iIOModuleNo, iIOOffset);
                //        string sTemp = string.Format("{0:000}", SM.IO.GetInputPara(i).iAdd + 1);
                //        tbCylAddrIF_.Text = sTemp;
                //    }
                //}
                //else edActrAddrIF_->Text = "None";

                //if (sTemp2.ToIntDef(-1) != -1)
                //{
                //    IO.GetInModuleNo(sTemp2.ToIntDef(-1), iIOModuleNo, iIOOffset);
                //    sTemp.sprintf("DI%d_%02d", iIOModuleNo + 1, iIOOffset);
                //    edActrAddrIB_->Text = sTemp;
                //}
                //else edActrAddrIB_->Text = "None";

                //if (sTemp3.ToIntDef(-1) != -1)
                //{
                //    IO.GetOutModuleNo(sTemp3.ToIntDef(-1), iIOModuleNo, iIOOffset);
                //    sTemp.sprintf("DO%d_%02d", iIOModuleNo + 1, iIOOffset);
                //    edActrAddrOF_->Text = sTemp;
                //}
                //else edActrAddrOF_->Text = "None";

                //if (sTemp4.ToIntDef(-1) != -1)
                //{
                //    IO.GetOutModuleNo(sTemp4.ToIntDef(-1), iIOModuleNo, iIOOffset);
                //    sTemp.sprintf("DO%d_%02d", iIOModuleNo + 1, iIOOffset);
                //    edActrAddrOB_->Text = sTemp;
                //}
                //else edActrAddrOB_->Text = "None";

            }

            //Motor
            else if (sPageSel == "tpMotor")
            {
                if (m_iPreCrntLevel != SM.FrmLogOn.GetLevel()) DispMotr();

                int iMotrSel = cbMotrSel.SelectedIndex;

                lbStop.BackColor = SM.MT.GetStop(iMotrSel) ? Color.Lime : Color.Silver;
                lbInpos.BackColor = SM.MT.GetInPosSgnl(iMotrSel) ? Color.Lime : Color.Silver;
                lbServo.BackColor = SM.MT.GetServo(iMotrSel) ? Color.Lime : Color.Silver;
                lbHomeDone.BackColor = SM.MT.GetHomeDone(iMotrSel) ? Color.Lime : Color.Silver;
                lbBreakOff.BackColor = SM.MT.GetBreakOff(iMotrSel) ? Color.Lime : Color.Silver;
                lbAlram.BackColor = SM.MT.GetAlarmSgnl(iMotrSel) ? Color.Lime : Color.Silver;
                lbDirP.BackColor = SM.MT.Stat[iMotrSel].bMovingP ? Color.Lime : Color.Silver;
                lbDirN.BackColor = SM.MT.Stat[iMotrSel].bMovingN ? Color.Lime : Color.Silver;
                lbZPhase.BackColor = SM.MT.GetZphaseSgnl(iMotrSel) ? Color.Lime : Color.Silver;
                lbLimitP.BackColor = SM.MT.GetPLimSnsr(iMotrSel) ? Color.Lime : Color.Silver;
                lbHome.BackColor = SM.MT.GetHomeSnsr(iMotrSel) ? Color.Lime : Color.Silver;
                lbLimitN.BackColor = SM.MT.GetNLimSnsr(iMotrSel) ? Color.Lime : Color.Silver;

                lbCmdPos.Text = SM.MT.GetCmdPos(iMotrSel).ToString();
                lbEncPos.Text = SM.MT.GetEncPos(iMotrSel).ToString();
                lbTrgPos.Text = SM.MT.GetTrgPos(iMotrSel).ToString();

                lbX1.BackColor = SM.MT.GetX(iMotrSel,0) ? Color.Lime : Color.Silver;
                lbX2.BackColor = SM.MT.GetX(iMotrSel,1) ? Color.Lime : Color.Silver;
                lbX3.BackColor = SM.MT.GetX(iMotrSel,2) ? Color.Lime : Color.Silver;
                lbX4.BackColor = SM.MT.GetX(iMotrSel,3) ? Color.Lime : Color.Silver;
                lbX5.BackColor = SM.MT.GetX(iMotrSel,4) ? Color.Lime : Color.Silver;

                lbY1.BackColor = SM.MT.GetY(iMotrSel,0) ? Color.Lime : Color.Silver;
                lbY2.BackColor = SM.MT.GetY(iMotrSel,1) ? Color.Lime : Color.Silver;
                lbY3.BackColor = SM.MT.GetY(iMotrSel,2) ? Color.Lime : Color.Silver;
                lbY4.BackColor = SM.MT.GetY(iMotrSel,3) ? Color.Lime : Color.Silver;
                lbY5.BackColor = SM.MT.GetY(iMotrSel,4) ? Color.Lime : Color.Silver;

            }

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iPageSel    = tcMain.SelectedIndex;
            string sPageSel = tcMain.TabPages[iPageSel].Name;
            DispForm(sPageSel);
            cbDioTestMode.Checked = false;
            SM.IO.SetTestMode(false);
            SM.AIO.SetTestMode(false);
            
        }

        private void DispForm(string _sPageIndex)
        {
            
            
            //Error
            if (_sPageIndex == "tpError")
            {
                DispErr();
            }
            //IO
            else if (_sPageIndex == "tpDio")
            {
                DispDio();
            }
            //AIO
            else if (_sPageIndex == "tpAio")
            {
                DispAio();
            }
            //TowerLamp
            else if (_sPageIndex == "tpTowerLamp")
            {
                int iMotrSel = cbMotrSel.SelectedIndex;

                DispMotr();
                DispTLamp();
            }

            //Cylinder
            else if (_sPageIndex == "tpCylinder")
            {
                DispCylinder();
            }

            //Motor
            else if (_sPageIndex == "tpMotor")
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

            if(SM.ER.GetErrLevel(_iSelNo) == EN_ERR_LEVEL.Error) rbError.Checked = true;
            else                                                 rbInfo .Checked = true;
            if (tbErrImg.Text.Trim() == "")
            {
                pbErrImg.Image = null;
                return;
            }

            FileInfo Info = new FileInfo(tbErrImg.Text);

            if (!Info.Exists)
            {
                pbErrImg.Image = null;
                //Log.ShowMessage("File Not Exist", tbErrImg.Text);
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
            btSaveErr.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SM.ER.LoadSave(false);
            btSaveErr.Enabled = true;
        }

        private void btErrApply_Click(object sender, EventArgs e)
        {
            btErrApply.Enabled = false;

            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            
            if (tbErrNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbErrNo.Text);

                CErrMan.TPara Para = SM.ER.GetErrConfig(iSelNo);
                Para.sEnum = tbErrEnum.Text;
                Para.sName = tbErrName.Text;
                Para.sAction = tbErrAct.Text;
                Para.sImgPath = tbErrImg.Text;

                Para.dRectLeft = m_tErrTracker.Rect.X;
                Para.dRectTop = m_tErrTracker.Rect.Y;
                Para.dRectWidth = m_tErrTracker.Rect.Width;
                Para.dRectHeight = m_tErrTracker.Rect.Height;

                if(rbError.Checked) Para.iErrorLevel = (int)EN_ERR_LEVEL.Error;
                else                Para.iErrorLevel = (int)EN_ERR_LEVEL.Info ;

                SM.ER.SetErrConfig(iSelNo, Para);

                DispErr();
            }

            btErrApply.Enabled = true;
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
                lvInput.Items[i].SubItems[1 ].Text = string.Format("X{0:000}", SM.IO.GetInputPara(i).iAdd);
                lvInput.Items[i].SubItems[2 ].Text = string.Format("X{0:X3}", SM.IO.GetInputPara(i).iAdd);
                lvInput.Items[i].SubItems[5 ].Text = SM.IO.GetInputPara(i).sName;
                lvInput.Items[i].SubItems[6 ].Text = SM.IO.GetInputPara(i).bInv    ? "O" : "X";
                lvInput.Items[i].SubItems[7 ].Text = SM.IO.GetInputPara(i).bNotLog ? "O" : "X";
                lvInput.Items[i].SubItems[8 ].Text = SM.IO.GetInputPara(i).SComt;
                lvInput.Items[i].SubItems[9 ].Text = string.Format("{0:0}", SM.IO.GetInputPara(i).iOnDelay);
                lvInput.Items[i].SubItems[10].Text = string.Format("{0:0}", SM.IO.GetInputPara(i).iOffDelay);
            }

            for (int i = 0; i < SM.IO._iMaxOut; i++)
            {
                //sItem = string.Format("X{0:000}", SM.IO.GetInputPara(i).iAdd);
                //lvInput.Items[i].SubItems[1].Text = sItem;
                lvOutput.Items[i].SubItems[1 ].Text = string.Format("Y{0:000}", SM.IO.GetOutputPara(i).iAdd);
                lvOutput.Items[i].SubItems[2 ].Text = string.Format("Y{0:X3}", SM.IO.GetOutputPara(i).iAdd);
                lvOutput.Items[i].SubItems[5 ].Text = SM.IO.GetOutputPara(i).sName;
                lvOutput.Items[i].SubItems[6 ].Text = SM.IO.GetOutputPara(i).bInv    ? "O" : "X";
                lvOutput.Items[i].SubItems[7 ].Text = SM.IO.GetOutputPara(i).bNotLog ? "O" : "X";
                lvOutput.Items[i].SubItems[8 ].Text = SM.IO.GetOutputPara(i).SComt;
                lvOutput.Items[i].SubItems[9 ].Text = string.Format("{0:0}", SM.IO.GetOutputPara(i).iOnDelay);
                lvOutput.Items[i].SubItems[10].Text = string.Format("{0:0}", SM.IO.GetOutputPara(i).iOffDelay);
            }
        }

        private void DispAio()
        {
            for (int i = 0; i < SM.AIO._iMaxIn; i++)
            {
                lvInput_A.Items[i].SubItems[1 ].Text = string.Format("X{0:000}", SM.AIO.GetInputPara(i).iAdd);
                lvInput_A.Items[i].SubItems[2 ].Text = string.Format("X{0:X3}", SM.AIO.GetInputPara(i).iAdd);
                lvInput_A.Items[i].SubItems[3 ].Text = SM.AIO.GetInputPara(i).sName;
                lvInput_A.Items[i].SubItems[4 ].Text = SM.AIO.GetInputPara(i).SComt;
                lvInput_A.Items[i].SubItems[5 ].Text = string.Format("{0:0}", SM.AIO.GetX(i).ToString());
                lvInput_A.Items[i].SubItems[6 ].Text = string.Format("{0:0}", SM.AIO.GetX(i,true).ToString());
            }

            for (int i = 0; i < SM.AIO._iMaxOut; i++)
            {
                //sItem = string.Format("X{0:000}", SM.IO.GetInputPara(i).iAdd);
                //lvInput.Items[i].SubItems[1].Text = sItem;
                lvOutput_A.Items[i].SubItems[1 ].Text = string.Format("Y{0:000}", SM.AIO.GetOutputPara(i).iAdd);
                lvOutput_A.Items[i].SubItems[2 ].Text = string.Format("Y{0:X3}", SM.AIO.GetOutputPara(i).iAdd);
                lvOutput_A.Items[i].SubItems[3 ].Text = SM.AIO.GetOutputPara(i).sName;
                lvOutput_A.Items[i].SubItems[4 ].Text = SM.AIO.GetOutputPara(i).SComt;
            }
        }

        private void DispDioInputProp(int _iSelNo)
        {
            tbDioNo.Text       = _iSelNo.ToString();
            tbDioEnum.Text     = SM.IO.GetInputPara(_iSelNo).sEnum;
            tbDioName.Text     = SM.IO.GetInputPara(_iSelNo).sName;
            tbDioAdd.Text      = SM.IO.GetInputPara(_iSelNo).iAdd.ToString();
            tbDioComment.Text  = SM.IO.GetInputPara(_iSelNo).SComt;
            tbDioOnDelay.Text  = SM.IO.GetInputPara(_iSelNo).iOnDelay.ToString();
            tbDioOffDelay.Text = SM.IO.GetInputPara(_iSelNo).iOffDelay.ToString();
            cbDioInv.Checked   = SM.IO.GetInputPara(_iSelNo).bInv;
            cbNotLog.Checked   = SM.IO.GetInputPara(_iSelNo).bNotLog;
        }

        private void DispDioOutputProp(int _iSelNo)
        {
            tbDioNo.Text       = _iSelNo.ToString();
            tbDioEnum.Text     = SM.IO.GetOutputPara(_iSelNo).sEnum;
            tbDioName.Text     = SM.IO.GetOutputPara(_iSelNo).sName;
            tbDioAdd.Text      = SM.IO.GetOutputPara(_iSelNo).iAdd.ToString();
            tbDioComment.Text  = SM.IO.GetOutputPara(_iSelNo).SComt;
            tbDioOnDelay.Text  = SM.IO.GetOutputPara(_iSelNo).iOnDelay.ToString();
            tbDioOffDelay.Text = SM.IO.GetOutputPara(_iSelNo).iOffDelay.ToString();
            cbDioInv.Checked   = SM.IO.GetOutputPara(_iSelNo).bInv;
            cbNotLog.Checked   = SM.IO.GetOutputPara(_iSelNo).bNotLog;
        }

        private void DispAioInputProp(int _iSelNo)
        {
            tbAiNo.Text       = _iSelNo.ToString();
            tbAiEnum.Text     = SM.AIO.GetInputPara(_iSelNo).sEnum;
            tbAiName.Text     = SM.AIO.GetInputPara(_iSelNo).sName;
            tbAiAdd.Text      = SM.AIO.GetInputPara(_iSelNo).iAdd.ToString();
            tbAiComment.Text  = SM.AIO.GetInputPara(_iSelNo).SComt;
        }

        private void DispAioOutputProp(int _iSelNo)
        {
            tbAoNo.Text       = _iSelNo.ToString();
            tbAoEnum.Text     = SM.AIO.GetOutputPara(_iSelNo).sEnum;
            tbAoName.Text     = SM.AIO.GetOutputPara(_iSelNo).sName;
            tbAoAdd.Text      = SM.AIO.GetOutputPara(_iSelNo).iAdd.ToString();
            tbAoComment.Text  = SM.AIO.GetOutputPara(_iSelNo).SComt;
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
            btDioApply.Enabled = false;

            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (tbDioNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbDioNo.Text);

                if (tsOutput.SelectedIndex == 0)
                {
                    //Para.iAdd = CIniFile.StrToIntDef(tbDioAdd.Text, 0);
                    CDioMan.TPara InPara = SM.IO.GetInputPara(iSelNo);
                    InPara.iAdd          = CConfig.StrToIntDef(tbDioAdd.Text);
                    InPara.iOffDelay     = CConfig.StrToIntDef(tbDioOffDelay.Text);
                    InPara.iOnDelay      = CConfig.StrToIntDef(tbDioOnDelay.Text);
                    InPara.SComt         = tbDioComment.Text;
                    InPara.sName         = tbDioName.Text;
                    InPara.bInv          = cbDioInv.Checked;
                    InPara.bNotLog       = cbNotLog.Checked;
                    

                    SM.IO.SetInputPara(iSelNo, InPara);
                }
                if (tsOutput.SelectedIndex == 1)
                {
                    CDioMan.TPara OutPara = SM.IO.GetOutputPara(iSelNo);
                    OutPara.iAdd          = CConfig.StrToIntDef(tbDioAdd.Text);
                    OutPara.iOffDelay     = CConfig.StrToIntDef(tbDioOffDelay.Text);
                    OutPara.iOnDelay      = CConfig.StrToIntDef(tbDioOnDelay.Text);
                    OutPara.SComt         = tbDioComment.Text;
                    OutPara.sName         = tbDioName.Text;
                    OutPara.bInv          = cbDioInv.Checked;
                    OutPara.bNotLog       = cbNotLog.Checked;

                    SM.IO.SetOutputPara(iSelNo, OutPara);

                }
                DispDio();
            }

            btDioApply.Enabled = true;
        }

        private void btSaveIO_Click(object sender, EventArgs e)
        {
            btSaveIO.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SM.IO.LoadSave(false);
            btSaveIO.Enabled = true;
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
            btTLampApply.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (tbRedAdd.Text != "")
            {
                EN_SEQ_STAT eStat = (EN_SEQ_STAT)lvTLamp.SelectedIndices[0];

                CTowerLampMan.TPara Para = SM.TL.GetTLampPara();
                CTowerLampMan.TLampInfo Info = SM.TL.GetTLampInfo(eStat);

                Para.iRedAdd = CConfig.StrToIntDef(tbRedAdd.Text);
                Para.iYelAdd = CConfig.StrToIntDef(tbYelAdd.Text);
                Para.iGrnAdd = CConfig.StrToIntDef(tbGrnAdd.Text);
                Para.iSndAdd = CConfig.StrToIntDef(tbSntAdd.Text);

                SM.TL.SetTLampPara(Para);

                Info.iRed = (CTowerLampMan.EN_LAMP_OPER)cbRedSeq.SelectedIndex;
                Info.iYel = (CTowerLampMan.EN_LAMP_OPER)cbYelSeq.SelectedIndex;
                Info.iGrn = (CTowerLampMan.EN_LAMP_OPER)cbGrnSeq.SelectedIndex;
                Info.iBuzz = (CTowerLampMan.EN_LAMP_OPER)cbSndSeq.SelectedIndex;

                SM.TL.SetTLampInfo(eStat, Info);

                DispTLamp();
            }
            btTLampApply.Enabled = true;
        }

        private void btSaveTLamp_Click(object sender, EventArgs e)
        {
            btSaveTLamp.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SM.TL.LoadSave(false);
            btSaveTLamp.Enabled = true;
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
            cbActrSync.Checked = SM.CL.GetCylinderPara(_iSelNo).bActrSync;
            tbActrSync.Text = SM.CL.GetCylinderPara(_iSelNo).iActrSync .ToString();
            tbRptDelay.Text = SM.CL.GetCylRptPara(_iSelNo).iDelay.ToString();
           

            SetCntrlPn(pnActrCnt);
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
            btCylApply.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //string sXAdd = "";
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);

                CCylinder.TPara Para = SM.CL.GetCylinderPara(iSelNo);
                Para.sEnum = tbCylEnum.Text;
                Para.sName = tbCylName.Text;
                Para.sComment = tbCylComt.Text;
                Para.iFwdXAdd = CConfig.StrToIntDef(tbCylAddrIF.Text);
                Para.iBwdXAdd = CConfig.StrToIntDef(tbCylAddrIB.Text);
                Para.iFwdYAdd = CConfig.StrToIntDef(tbCylAddrOF.Text);
                Para.iBwdYAdd = CConfig.StrToIntDef(tbCylAddrOB.Text);
                Para.iFwdOnDelay = CConfig.StrToIntDef(tbCylOnDelayF.Text);
                Para.iBwdOnDelay = CConfig.StrToIntDef(tbCylOnDelayB.Text);
                Para.iFwdTimeOut = CConfig.StrToIntDef(tbCylTimeOutF.Text);
                Para.iBwdTimeOut = CConfig.StrToIntDef(tbCylTimeOutB.Text);
                Para.eDirType = (EN_MOVE_DIRECTION)cbDirc.SelectedIndex;
                Para.bActrSync = cbActrSync.Checked;
                Para.iActrSync = CConfig.StrToIntDef(tbActrSync.Text);
                SM.CL.Repeat.iDelay = CConfig.StrToIntDef(tbRptDelay.Text);


                SM.CL.SetCylinderPara(iSelNo, Para);

                SetCntrlPn(pnActrCnt);

                DispCylinder();
            }
            btCylApply.Enabled = true;   
        }

        private void btSaveCyl_Click(object sender, EventArgs e)
        {
            btSaveCyl.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SM.CL.LoadSave(false);
            btSaveCyl.Enabled = true;
        }

        private EN_LEVEL m_iPreCrntLevel = EN_LEVEL.Operator;

        private void DispMotr()
        {
            //ComboBox.ObjectCollection MotrItems ;
            //MotrItems = new ComboBox.ObjectCollection();
            if (SM.MT._iMaxMotr < 1) return;

            int iMotrSel = cbMotrSel.SelectedIndex;

            pgMotrPara   .SelectedObject = SM.MT.Para[iMotrSel];
            pgMotrParaSub.SelectedObject = SM.MT.Mtr [iMotrSel].GetPara();

            //if(FormPassword.m_iCrntLevel == EN_LEVEL.master)
            if (SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master)
            {
                pgMotrPara.Enabled = true;
                pgMotrParaSub.Enabled = true;
            }
            else
            {
                pgMotrPara.Enabled = false;
                pgMotrParaSub.Enabled = false;
            }

            m_iPreCrntLevel = SM.FrmLogOn.GetLevel();
            
        }

        private void cbMotrSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            DispMotr();
        }

        private void btSaveMotr_Click(object sender, EventArgs e)
        {
            btSaveMotr.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SM.MT.ApplyParaAll();
            SM.MT.LoadSaveAll(false);
            btSaveMotr.Enabled = true;
        }

        private void FormDllMain_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) tmUpdate.Enabled = true;
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
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Down", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.JogP(iMotrSel);
        }

        private void btJogP_MouseUp(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Up", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.Stop(iMotrSel);
        }

        private void btJobN_MouseDown(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Down", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.JogN(iMotrSel);
        }

        private void btJobN_MouseUp(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Up", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.Stop(iMotrSel);
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.Stop(iMotrSel);
        }

        private void btServoOn_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetServo(iMotrSel,true);
        }

        private void btServoOff_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetServo(iMotrSel, false);
        }

        private void btAllStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            for (int i = 0; i < SM.MT._iMaxMotr; i++ )
                SM.MT.Stop(i);
        }

        private void btServoOnAll_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            for (int i = 0; i < SM.MT._iMaxMotr; i++)
                SM.MT.SetServo(i,true);
        }

        private void btServoOffAll_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            for (int i = 0; i < SM.MT._iMaxMotr; i++)
                SM.MT.SetServo(i, false);
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.GoHome(iMotrSel);
        }

        private void btClearPos_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetPos(iMotrSel, 0.0);
        }

        private void btGo1stPos_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.GoAbsRepeatFst(iMotrSel);
        }

        private void btGo2ndPos_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.GoAbsRepeatScd(iMotrSel);
        }

        private void btStop2_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.Stop(iMotrSel);
        }

        private void btRepeat_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.StartRepeat(iMotrSel);
        }

        private void btReset_MouseDown(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Down", ti.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetReset(iMotrSel, true);

        }

        private void btReset_MouseUp(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Up", ti.Frm);

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

                 if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, m_tErrTracker.Rect.Top    - iThick, m_tErrTracker.Rect.Left  + iThick, m_tErrTracker.Rect.Top    + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.LeftTop;
            else if (CheckRectIn(iCntX                    - iThick, m_tErrTracker.Rect.Top    - iThick, iCntX                    + iThick, m_tErrTracker.Rect.Top    + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.Top;
            else if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, m_tErrTracker.Rect.Top    - iThick, m_tErrTracker.Rect.Right + iThick, m_tErrTracker.Rect.Top    + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.RightTop;
            else if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, iCntY                     - iThick, m_tErrTracker.Rect.Right + iThick, iCntY                     + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.Right;
            else if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, m_tErrTracker.Rect.Bottom - iThick, m_tErrTracker.Rect.Right + iThick, m_tErrTracker.Rect.Bottom + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.RightBottom;
            else if (CheckRectIn(iCntX                    - iThick, m_tErrTracker.Rect.Bottom - iThick, iCntX                    + iThick, m_tErrTracker.Rect.Bottom + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.Bottom;
            else if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, m_tErrTracker.Rect.Bottom - iThick, m_tErrTracker.Rect.Left  + iThick, m_tErrTracker.Rect.Bottom + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.LeftBottom;
            else if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, iCntY                     - iThick, m_tErrTracker.Rect.Left  + iThick, iCntY                     + iThick, e.X, e.Y)) m_tErrTracker.eClickPos = EClickPos.Left;
            else if (CheckRectShapeIn(m_tErrTracker.Rect, e.X, e.Y, TErrTracker.iThickness / 2.0)) m_tErrTracker.eClickPos = EClickPos.Move;

            else if (!CheckRectShapeIn(m_tErrTracker.Rect, e.X, e.Y, TErrTracker.iThickness / 2.0)) m_tErrTracker.eClickPos = EClickPos.None;

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
                if(m_tErrTracker.eClickPos == EClickPos.Move)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.LeftTop)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width - x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.Top)
                {
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.RightTop)
                {
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.Right)
                {
                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                  

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.RightBottom)
                {
                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.Bottom)
                {
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.LeftBottom)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;


                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width - x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = e.X;
                    m_tErrTracker.iPrePosY = e.Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.Left)
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


        public void SetCntrlPn(Panel _pnBase)
        {
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                CCylinder.TPara Para = SM.CL.GetCylinderPara(iSelNo);

                if (tbStCaption == null) tbStCaption = new TextBox();
                if (btBwd       == null) btBwd       = new Button ();
                if (btFwd       == null) btFwd       = new Button ();

                tbStCaption.Parent = _pnBase;
                btBwd.Parent       = _pnBase;
                btFwd.Parent       = _pnBase;

                tbStCaption.BringToFront();
                btBwd      .BringToFront();
                btFwd      .BringToFront();

                tbStCaption.Text = Para.sName;
                tbStCaption.BorderStyle = BorderStyle.Fixed3D;
                tbStCaption.AutoSize = false;
                tbStCaption.Anchor = AnchorStyles.Top; //컨트롤 정렬하는 부분
                tbStCaption.TextAlign = HorizontalAlignment.Center;

                //모냥 찐딴데 나중에 다듬자.
                const int iMargin = 0; //마진을 주면 삐꾸나는데 마진 없이 그냥쓰자. int연산때문에 계산이 복잡하구 드러워서

                int iFontSize   = _pnBase.Height / 12;

                int iUDCellSize = _pnBase.Height / 3;

                //해더.
                tbStCaption.Left = iMargin;
                tbStCaption.Top = iMargin;
                tbStCaption.Width = _pnBase.Width - iMargin * 2;
                tbStCaption.Height = _pnBase.Height / 3 - iMargin * 2;
                //tbStCaption.Font = new Font(this.Font.FontFamily, Para.sName, (float)iFontSize);
                tbStCaption.Font = new Font(Font.FontFamily, (float)iFontSize);
                tbStCaption.Text = Para.sName;

                //위아래로 배치되는 버튼.
                int iUDBtnLeft = iMargin;
                int iUDBtnHeight = _pnBase.Height / 3 - iMargin * 2;
                int iUBtnTop = _pnBase.Height / 3 + iMargin * 3;
                int iDBtnTop = _pnBase.Height * 2 / 3 + iMargin * 5;
                int iUDBtnWidth = _pnBase.Width - iMargin * 2;

                //양옆으로 배치되는 버튼.
                int iLBtnLeft = iMargin;
                int iRBtnLeft = _pnBase.Width / 2 + iMargin * 3;
                int iLRBtnTop = _pnBase.Height / 3 + iMargin * 3;
                int iLRBtnWidth = _pnBase.Width / 2 - iMargin * 4;
                int iLRBtnHeight = _pnBase.Height * 2 / 3 + iMargin * 4;

                btBwd.Text = "BWD";
                btFwd.Text = "FWD";

                //btBwd.Font = new Font("BWD", (float)iFontSize, FontStyle.Regular);
                //btFwd.Font = new Font("FWD", (float)iFontSize, FontStyle.Regular);
                btBwd.Font = new Font(Font.FontFamily, (float)iFontSize);
                btFwd.Font = new Font(Font.FontFamily, (float)iFontSize);
                btBwd.Text = "BWD";
                btFwd.Text = "FWD";
                //btFwd.Font

                btBwd.MouseDown += new MouseEventHandler(btBwd_MouseDown);
                btFwd.MouseDown += new MouseEventHandler(btFwd_MouseDown);

                //mdLR
                //mdRL
                //mdBF
                //mdFB
                //mdUD
                //mdDU
                //mdCA
                //mdAC

                switch(Para.eDirType) 
                { 
                    case EN_MOVE_DIRECTION.LR : if(Properties.Resources.LEFT  != null) btBwd.Image = Properties.Resources.LEFT ;
                                                  if(Properties.Resources.RIGHT != null) btFwd.Image = Properties.Resources.RIGHT;

                                                  btBwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btFwd.TextImageRelation = TextImageRelation.TextBeforeImage;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleCenter;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleLeft;
                                                  

                                                  btBwd.Left   = iLBtnLeft;
                                                  btBwd.Top    = iLRBtnTop;
                                                  btBwd.Width  = iLRBtnWidth;
                                                  btBwd.Height = iLRBtnHeight;

                                                  btFwd.Left   = iRBtnLeft;
                                                  btFwd.Top    = iLRBtnTop;
                                                  btFwd.Width  = iLRBtnWidth;
                                                  btFwd.Height = iLRBtnHeight;
                                                  
                                                  break ;

                    case EN_MOVE_DIRECTION.RL : if (Properties.Resources.RIGHT != null) btBwd.Image = Properties.Resources.RIGHT;
                                                  if (Properties.Resources.LEFT  != null) btFwd.Image = Properties.Resources.LEFT ;

                                                  btBwd.TextImageRelation = TextImageRelation.TextBeforeImage;
                                                  btFwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleLeft;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleCenter;

                                                  btBwd.Left   = iRBtnLeft;
                                                  btBwd.Top    = iLRBtnTop;
                                                  btBwd.Width  = iLRBtnWidth;
                                                  btBwd.Height = iLRBtnHeight;

                                                  btFwd.Left   = iLBtnLeft;
                                                  btFwd.Top    = iLRBtnTop;
                                                  btFwd.Width  = iLRBtnWidth;
                                                  btFwd.Height = iLRBtnHeight;
                                                  break ;

                    case EN_MOVE_DIRECTION.BF : if (Properties.Resources.UP != null) btBwd.Image = Properties.Resources.UP;
                                                  if (Properties.Resources.DN != null) btFwd.Image = Properties.Resources.DN;

                                                  btBwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btFwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleCenter;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleCenter;

                                                  btBwd.Left = iUDBtnLeft;
                                                  btBwd.Top = iUBtnTop;
                                                  btBwd.Width = iUDBtnWidth;
                                                  btBwd.Height = iUDBtnHeight;

                                                  btFwd.Left = iUDBtnLeft;
                                                  btFwd.Top = iDBtnTop;
                                                  btFwd.Width = iUDBtnWidth;
                                                  btFwd.Height = iUDBtnHeight;
                                                  break ;
                
                    case EN_MOVE_DIRECTION.FB : if (Properties.Resources.DN != null) btBwd.Image = Properties.Resources.DN;
                                                  if (Properties.Resources.UP != null) btFwd.Image = Properties.Resources.UP;

                                                  btBwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btFwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleCenter;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleCenter;

                                                  btBwd.Left = iUDBtnLeft;
                                                  btBwd.Top = iDBtnTop;
                                                  btBwd.Width = iUDBtnWidth;
                                                  btBwd.Height = iUDBtnHeight;

                                                  btFwd.Left = iUDBtnLeft;
                                                  btFwd.Top = iUBtnTop;
                                                  btFwd.Width = iUDBtnWidth;
                                                  btFwd.Height = iUDBtnHeight;
                                                  break ;
                
                    case EN_MOVE_DIRECTION.UD : if (Properties.Resources.UP != null) btBwd.Image = Properties.Resources.UP;
                                                  if (Properties.Resources.DN != null) btFwd.Image = Properties.Resources.DN;

                                                  btBwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btFwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleCenter;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleCenter;

                                                  btBwd.Left = iUDBtnLeft;
                                                  btBwd.Top = iUBtnTop;
                                                  btBwd.Width = iUDBtnWidth;
                                                  btBwd.Height = iUDBtnHeight;

                                                  btFwd.Left = iUDBtnLeft;
                                                  btFwd.Top = iDBtnTop;
                                                  btFwd.Width = iUDBtnWidth;
                                                  btFwd.Height = iUDBtnHeight;
                                                  break ;
                
                    case EN_MOVE_DIRECTION.DU : if (Properties.Resources.DN != null) btBwd.Image = Properties.Resources.DN;
                                                  if (Properties.Resources.UP != null) btFwd.Image = Properties.Resources.UP;

                                                  btBwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btFwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleCenter;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleCenter;

                                                  btBwd.Left = iUDBtnLeft;
                                                  btBwd.Top = iDBtnTop;
                                                  btBwd.Width = iUDBtnWidth;
                                                  btBwd.Height = iUDBtnHeight;

                                                  btFwd.Left = iUDBtnLeft;
                                                  btFwd.Top = iUBtnTop;
                                                  btFwd.Width = iUDBtnWidth;
                                                  btFwd.Height = iUDBtnHeight;
                                                  break ;
                
                    case EN_MOVE_DIRECTION.CA : if (Properties.Resources.CW  != null) btBwd.Image = Properties.Resources.CW  ;
                                                  if (Properties.Resources.CCW != null) btFwd.Image = Properties.Resources.CCW ;

                                                  btBwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btFwd.TextImageRelation = TextImageRelation.TextBeforeImage;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleCenter;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleLeft;

                                                  btBwd.Left = iLBtnLeft;
                                                  btBwd.Top = iLRBtnTop;
                                                  btBwd.Width = iLRBtnWidth;
                                                  btBwd.Height = iLRBtnHeight;

                                                  btFwd.Left = iRBtnLeft;
                                                  btFwd.Top = iLRBtnTop;
                                                  btFwd.Width = iLRBtnWidth;
                                                  btFwd.Height = iLRBtnHeight;
                                                  break ;
                
                    case EN_MOVE_DIRECTION.AC : if (Properties.Resources.CCW != null) btBwd.Image = Properties.Resources.CCW;
                                                  if (Properties.Resources.CW  != null) btFwd.Image = Properties.Resources.CW ;

                                                  btBwd.TextImageRelation = TextImageRelation.TextBeforeImage;
                                                  btFwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleLeft;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleCenter;

                                                  btBwd.Left = iRBtnLeft;
                                                  btBwd.Top = iLRBtnTop;
                                                  btBwd.Width = iLRBtnWidth;
                                                  btBwd.Height = iLRBtnHeight;

                                                  btFwd.Left = iLBtnLeft;
                                                  btFwd.Top = iLRBtnTop;
                                                  btFwd.Width = iLRBtnWidth;
                                                  btFwd.Height = iLRBtnHeight;
                                                  break ;

                    case EN_MOVE_DIRECTION.CO : if (Properties.Resources.LEFT != null) btBwd.Image = Properties.Resources.LEFT;
                                                  if (Properties.Resources.RIGHT  != null) btFwd.Image = Properties.Resources.RIGHT ;

                                                  btBwd.TextImageRelation = TextImageRelation.TextBeforeImage;
                                                  btFwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleLeft;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleCenter;

                                                  btBwd.Left = iRBtnLeft;
                                                  btBwd.Top = iLRBtnTop;
                                                  btBwd.Width = iLRBtnWidth;
                                                  btBwd.Height = iLRBtnHeight;

                                                  btFwd.Left = iLBtnLeft;
                                                  btFwd.Top = iLRBtnTop;
                                                  btFwd.Width = iLRBtnWidth;
                                                  btFwd.Height = iLRBtnHeight;
                                                  break ;

                    case EN_MOVE_DIRECTION.OC : if (Properties.Resources.RIGHT != null) btBwd.Image = Properties.Resources.RIGHT;
                                                  if (Properties.Resources.LEFT  != null) btFwd.Image = Properties.Resources.LEFT ;

                                                  btBwd.TextImageRelation = TextImageRelation.TextBeforeImage;
                                                  btFwd.TextImageRelation = TextImageRelation.ImageBeforeText;
                                                  btBwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btFwd.TextAlign = ContentAlignment.MiddleRight;
                                                  btBwd.ImageAlign = ContentAlignment.MiddleLeft;
                                                  btFwd.ImageAlign = ContentAlignment.MiddleCenter;

                                                  btBwd.Left = iRBtnLeft;
                                                  btBwd.Top = iLRBtnTop;
                                                  btBwd.Width = iLRBtnWidth;
                                                  btBwd.Height = iLRBtnHeight;

                                                  btFwd.Left = iLBtnLeft;
                                                  btFwd.Top = iLRBtnTop;
                                                  btFwd.Width = iLRBtnWidth;
                                                  btFwd.Height = iLRBtnHeight;
                                                  break ;
                }
                
                m_bSetCntrlPn = true ;
                
            }
        }
        
        public void DelCntrlPn()
        {
        //    delete m_pStCaption;
        //    delete m_pBtnBw    ;
        //    delete m_pBtnFw    ;
        }

        public void btBwd_MouseDown(object sender, MouseEventArgs e)
        {
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                CCylinder.TPara Para = SM.CL.GetCylinderPara(iSelNo);

                Log.Trace(Para.sName, "Clicked");
                SM.CL.Move(iSelNo, EN_CYLINDER_POS.Bwd);
            }
        }

        public void btFwd_MouseDown(object sender, MouseEventArgs e)
        {
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                CCylinder.TPara Para = SM.CL.GetCylinderPara(iSelNo);

                Log.Trace(Para.sName, "Clicked");
                SM.CL.Move(iSelNo, EN_CYLINDER_POS.Fwd);
            }
        }

        private void btActrReset_Click(object sender, EventArgs e)
        {
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                CCylinder.TPara Para = SM.CL.GetCylinderPara(iSelNo);

                Log.Trace(Para.sName, "Clicked");
                SM.CL.Reset();
            }
        }

        private void btActrRpt_Click(object sender, EventArgs e)
        {
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                //CCylinder.TPara Para = SM.CL.GetCylinderPara(iSelNo);

                if (cbActrSync.Checked) SM.CL.GoRpt(CConfig.StrToIntDef(tbRptDelay.Text, 0), iSelNo, CConfig.StrToIntDef(tbActrSync.Text, -1));
                else                    SM.CL.GoRpt(CConfig.StrToIntDef(tbRptDelay.Text, 0), iSelNo                                         );  
            }
        }

        private void btActrStop_Click(object sender, EventArgs e)
        {
            SM.CL.StopRpt();
        }

        private void FormDllMain_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void lvInput_A_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvInput_A.SelectedIndices.Count <= 0) return;
            int iInputSel = lvInput_A.SelectedIndices[0];
            DispAioInputProp(iInputSel);
        }

        private void lvOutput_A_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvOutput_A.SelectedIndices.Count <= 0) return;
            int iOutputSel = lvOutput_A.SelectedIndices[0];
            DispAioOutputProp(iOutputSel);
        }

        private void btAioApply_Click(object sender, EventArgs e)
        {
            btAioApply.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (tbAiNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbAiNo.Text);

                CAioMan.TPara InPara = SM.AIO.GetInputPara(iSelNo);
                InPara.iAdd = CConfig.StrToIntDef(tbAiAdd.Text);
                InPara.SComt = tbAiComment.Text;
                InPara.sName = tbAiName.Text;


                SM.AIO.SetInputPara(iSelNo, InPara);
            }
            if (tbAoNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbAoNo.Text,0);

                CAioMan.TPara OutPara = SM.AIO.GetOutputPara(iSelNo);
                OutPara.iAdd = CConfig.StrToIntDef(tbAoAdd.Text);
                OutPara.SComt = tbAoComment.Text;
                OutPara.sName = tbAoName.Text;

                SM.AIO.SetOutputPara(iSelNo, OutPara);
            }
            DispAio();
            btAioApply.Enabled = true;
        }

        private void btSaveAIO_Click(object sender, EventArgs e)
        {
            btSaveAIO.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SM.AIO.LoadSave(false);
            btSaveAIO.Enabled = true;
        }

        private void btAoSet_Click(object sender, EventArgs e)
        {
            if (lvOutput.SelectedIndices.Count <= 0) return;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            double dVol = CConfig.StrToDoubleDef(tbAoTest.Text);
            SM.AIO.SetTestMode(true);
            SM.AIO.SetYTestMode(lvOutput.SelectedItems[0].Index,dVol);
        }

        private void lbY1_DoubleClick(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Label Clicked", ti.Frm);

            string sTag = (string)((Label)sender).Tag;
            int iTag = CConfig.StrToIntDef(sTag,0);

            int iMotrSel = cbMotrSel.SelectedIndex;
            bool bRet = SM.MT.GetY(iMotrSel,iTag);
            SM.MT.SetY(iMotrSel,iTag,!bRet);
        }

        private void lbY1_Click(object sender, EventArgs e)
        {

        }

        private void rbInfo_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lbHomeDone_DoubleClick(object sender, EventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MT.SetHomeDone(iMotrSel, !SM.MT.GetHomeDone(iMotrSel));
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
