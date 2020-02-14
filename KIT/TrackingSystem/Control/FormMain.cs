using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Control
{
    public partial class FormMain : Form
    {
        public FormMotion FrmMotion;
        public FormKeyence FrmKeyence;
        public FormGraph FrmGraph12;
        public FormGraph FrmGraph3;
        public FormGraph FrmGraph4;
        private CRun Run;
        ListViewItem.ListViewSubItem SelectedLSI;
        //TextBox TxtEdit;
       
        private const string sFormText = "Form Main ";
        public bool bDown1;
        public bool bDown2;

        public FormMain()
        {
            InitializeComponent();

            DispDevice();

            Util.Init();
            OM.Init();

            Run        = new CRun();
            FrmMotion  = new FormMotion();
            FrmKeyence = new FormKeyence(this);
            FrmGraph12 = new FormGraph(SeriesChartType.FastPoint,"OUT1,2",true);
            FrmGraph3  = new FormGraph(SeriesChartType.FastLine, "OUT3");
            FrmGraph4  = new FormGraph(SeriesChartType.FastLine, "OUT4");

            Run.CAddPoints12 += new CRun.Chart_AddPoints12 (FrmGraph12.Chart_AddPoints);
            Run.CAddPoints3  += new CRun.Chart_AddPoints3  (FrmGraph3.Chart_AddPoints );
            Run.CAddPoints4  += new CRun.Chart_AddPoints4  (FrmGraph4.Chart_AddPoints );
            Run.CClear       += new CRun.Chart_Clear       (FrmGraph12.Chart_Clear    );
            Run.CClear       += new CRun.Chart_Clear       (FrmGraph3.Chart_Clear     );
            Run.CClear       += new CRun.Chart_Clear       (FrmGraph4.Chart_Clear     );
            Run.CBegin       += new CRun.Chart_Begin       (FrmGraph12.Begin          );
            Run.CBegin       += new CRun.Chart_Begin       (FrmGraph3.Begin           );
            Run.CBegin       += new CRun.Chart_Begin       (FrmGraph4.Begin           );
            Run.CEnd         += new CRun.Chart_End         (FrmGraph12.End            );
            Run.CEnd         += new CRun.Chart_End         (FrmGraph3.End             );
            Run.CEnd         += new CRun.Chart_End         (FrmGraph4.End             );
            Run.CSave12      += new CRun.Chart_Save12      (FrmGraph12.Save           );
            Run.CSave3       += new CRun.Chart_Save3       (FrmGraph3.Save            );
            Run.CSave4       += new CRun.Chart_Save4       (FrmGraph4.Save            );



            propertyGrid1.SelectedObject = OM.Auto1;
            propertyGrid2.SelectedObject = OM.Manual;
            propertyGrid3.SelectedObject = OM.Mode;

            lbText1.BackColor = Color.Transparent;
            lbText2.BackColor = Color.Transparent;
            pnMove.BackColor = Color.Transparent;
            //ListView 관련
            InitListView();
            UpdateLsv(true);

            var PropLotInfo = LsvDisp.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(LsvDisp, true, null);

            PropLotInfo = pnMove.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(pnMove, true, null);

            PropLotInfo = panel5.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(panel5, true, null);

            PropLotInfo = btStart.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(btStart, true, null);

            PropLotInfo = btStop.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(btStop, true, null);

            PropLotInfo = btLiftUp.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(btLiftUp, true, null);

            PropLotInfo = btLiftDn.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(btLiftDn, true, null);

            for (int i=0; i<lvDevice.Items.Count; i++)
            {
                if(lvDevice.Items[i].SubItems[1].Text == OM.GetCrntDev())
                {
                    //lvDevice.Focus();
                    //lvDevice.HideSelection = false;
                    lvDevice.Items[i].Selected = true;
                    lvDevice.Select();
                    //lvDevice.Items[i].EnsureVisible();
                }
            }
            //MainThread = new Thread(new ThreadStart(Update));

            Log.StartLogMan();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Util.MT.StopAll();
            Run.Close();
            Log.EndLogMan();      
        }

        public void DispDevice()
        {
            lvDevice.Items.Clear();

            InitDevice();
        }

        #region Init
        private void InitDevice()
        {
            //Device ListView
            lvDevice.Clear();
            lvDevice.View = View.Details;
            lvDevice.HeaderStyle = ColumnHeaderStyle.None;
            lvDevice.FullRowSelect = true;
            lvDevice.GridLines = true;

            lvDevice.Columns.Add("", 25, HorizontalAlignment.Left);
            lvDevice.Columns.Add("", 210, HorizontalAlignment.Left);
            lvDevice.Columns.Add("", 170, HorizontalAlignment.Left);

            string sPath;
            sPath = Application.StartupPath + "\\JobFile\\";
            DirectoryInfo Info = new DirectoryInfo(sPath);

            if (Info.Exists)
            {
                DirectoryInfo[] Dir = Info.GetDirectories();


                int iNo = 1;

                foreach (DirectoryInfo info in Dir)
                {

                    ListViewItem item = new ListViewItem(string.Format("{0}", iNo));
                    item.SubItems.Add(info.Name);
                    item.SubItems.Add(info.LastWriteTime.ToString());
                    lvDevice.Items.Add(item);
                    iNo++;

                }
            }

            var PropDevice = lvDevice.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropDevice.SetValue(lvDevice, true, null);

            //tmUpdate.Enabled = true;
        }

        private void InitListView()
        {
            LvAuto2.Clear();
            LvAuto2.View = View.Details;
            //Lsv.LabelEdit = true;
            //Lsv.AllowColumnReorder = true;
            LvAuto2.FullRowSelect = true;
            LvAuto2.GridLines = true;
            //Lsv.Sorting = SortOrder.Descending;
            //Lsv.Scrollable = true;
            //Need to Find
            LvAuto2.MultiSelect = false;
            //Lsv.HideSelection = false;

            LvAuto2.Columns.Add("POS", 50, HorizontalAlignment.Left); //Day Time
            LvAuto2.Columns.Add("Residence time" , 230, HorizontalAlignment.Left); //Day Time
            LvAuto2.Columns.Add("Measuring count", 230, HorizontalAlignment.Left); //Day Time
            LvAuto2.Columns.Add("Measuring time" , 230, HorizontalAlignment.Left); //Day Time

            LvAuto2.MouseWheel += new MouseEventHandler(Lsv_MouseWheel);

            LsvDisp.Clear();
            LsvDisp.View = View.Details;
            LsvDisp.FullRowSelect = true;
            LsvDisp.GridLines = true;
            LsvDisp.MultiSelect = false;
            LsvDisp.Columns.Add("Pos", 40, HorizontalAlignment.Left); //Day Time
            string sTemp ;
            for(int i=0; i<CRun.iMeasureMaxCnt; i++)
            {
                sTemp = (i + 1).ToString() ;
                LsvDisp.Columns.Add(sTemp, 90, HorizontalAlignment.Left); //Day Time
            }

            //TxtEdit = new TextBox();
            //TxtEdit.Leave += new EventHandler   (TxtEdit_Leave);
            //TxtEdit.KeyUp += new KeyEventHandler(TxtEdit_KeyUp);
            ////TxtEdit.SendToBack();
            //TxtEdit.BringToFront();
        }
        #endregion

        #region Menu 
        private void mOTIONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sText = mOTIONToolStripMenuItem.Text;
            Log.Trace(sFormText + sText + " Menu Click", 1);

            //FormKeyence FrmKeyence = new FormKeyence();
            FrmKeyence.Show();
        }

        private void motionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string sText = motionToolStripMenuItem1.Text;
            Log.Trace(sFormText + sText + " Menu Click", 1);

            //FormMotion    FrmMotion = new FormMotion ();
            FrmMotion.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sText = exitToolStripMenuItem.Text;
            Log.Trace(sFormText + sText + " Menu Click", 1);
            
            Close();
        }
        #endregion

        #region Device

        public void UpdateLsv(bool _bToTable)
        {
            if (_bToTable)
            {
                LvAuto2.Items.Clear();
                LvAuto2.BeginUpdate();
                ListViewItem item;
                for (int j = 0; j < OM.m_iMaxArray; j++)
                //for (int j = OM.m_iMaxArray - 1; j >= 0; j--)
                {
                    item = new ListViewItem((j).ToString());
                    //item.SubItems.Add(OM.Auto2[OM.m_iMaxArray - 1 - j].iBTime.ToString());
                    //item.SubItems.Add(OM.Auto2[OM.m_iMaxArray - 1 - j].iCount.ToString());
                    //item.SubItems.Add(OM.Auto2[OM.m_iMaxArray - 1 - j].iATime.ToString());
                    item.SubItems.Add(OM.Auto2[j].iBTime.ToString());
                    item.SubItems.Add(OM.Auto2[j].iCount.ToString());
                    item.SubItems.Add(OM.Auto2[j].iATime.ToString());

                    //1.0.0.1
                    //LvAuto2.Items.Insert(0, item);
                    LvAuto2.Items.Insert(LvAuto2.Items.Count, item);
                }
                LvAuto2.EndUpdate();
            }
            else
            {
                for (int i = 0; i < LvAuto2.Items.Count; i++)
                {
                    ListViewItem item = LvAuto2.Items[i];
                    if (OM.m_iMaxArray - 1 - i >= 0)
                    {

                        //OM.Auto2[OM.m_iMaxArray - 1 - i].iBTime = CConfig.StrToIntDef(item.SubItems[1].Text,0); CheckValue(ref OM.Auto2[OM.m_iMaxArray - 1 - i].iBTime , 0, 6000  );
                        //OM.Auto2[OM.m_iMaxArray - 1 - i].iCount = CConfig.StrToIntDef(item.SubItems[2].Text,0); CheckValue(ref OM.Auto2[OM.m_iMaxArray - 1 - i].iCount , 0, 30    );
                        //OM.Auto2[OM.m_iMaxArray - 1 - i].iATime = CConfig.StrToIntDef(item.SubItems[3].Text,0); CheckValue(ref OM.Auto2[OM.m_iMaxArray - 1 - i].iATime , 0, 20000 );

                        //1.0.0.1
                        OM.Auto2[i].iBTime = CConfig.StrToIntDef(item.SubItems[1].Text, 0); CheckValue(ref OM.Auto2[i].iBTime, 0, 6000 );
                        OM.Auto2[i].iCount = CConfig.StrToIntDef(item.SubItems[2].Text, 0); CheckValue(ref OM.Auto2[i].iCount, 0, 30   );
                        OM.Auto2[i].iATime = CConfig.StrToIntDef(item.SubItems[3].Text, 0); CheckValue(ref OM.Auto2[i].iATime, 0, 20000);

                        //OM.Auto2[i].iBTime = CConfig.StrToIntDef(item.SubItems[1].Text, 0);
                        //OM.Auto2[i].iCount = CConfig.StrToIntDef(item.SubItems[2].Text, 0);
                        //OM.Auto2[i].iATime = CConfig.StrToIntDef(item.SubItems[3].Text, 0);

                    }
                }
                UpdateLsv(true);
            }      

        }

        public void CheckValue(ref int _tbControl, int _iMin, int _iMax)
        {
            int iVal = _tbControl;// CConfig.StrToIntDef(_tbControl.Text, 0);
            if (iVal < _iMin) iVal = _iMin;
            if (iVal > _iMax) iVal = _iMax;
            _tbControl = iVal;
        }

        public void CheckValue(ref double _tbControl, double _dMin, double _dMax)
        {
            double dVal = _tbControl;// CConfig.StrToDoubleDef(_tbControl.Text, 0);
            if (dVal < _dMin) dVal = _dMin;
            if (dVal > _dMax) dVal = _dMax;
            _tbControl = dVal;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Click", 1);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
            UpdateLsv(false);

            OM.SaveJobFile(OM.GetCrntDev());
            //OM.SaveDevInfo(OM.GetCrntDev());
            //OM.SavePara(OM.GetCrntDev());
            propertyGrid1.SelectedObject = OM.Auto1;
            propertyGrid2.SelectedObject = OM.Manual;
            propertyGrid3.SelectedObject = OM.Mode;

        }

        private void Lsv_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = LvAuto2.HitTest(e.X, e.Y);
            SelectedLSI = i.SubItem;
            if (SelectedLSI == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);
            if (col == 0) {
                ListViewItem item = LvAuto2.Items[row];
                TxtEdit.Text = item.SubItems[col].Text ;
                return;
            }

            int border = 0;
            switch (LvAuto2.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = SelectedLSI.Bounds.Width;
            int CellHeight = SelectedLSI.Bounds.Height;
            int CellLeft = border + LvAuto2.Left + i.SubItem.Bounds.Left;
            int CellTop = LvAuto2.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = LvAuto2.Columns[0].Width;
            TxtEdit.Location = new Point(CellLeft, CellTop);
            TxtEdit.Size = new Size(CellWidth, CellHeight);
            TxtEdit.Visible = true;
            TxtEdit.BringToFront();
            TxtEdit.Text = i.SubItem.Text;
            TxtEdit.Select();
            TxtEdit.SelectAll();
        }

        private void HideTextEditor()
        {
            TxtEdit.Visible = false;
            if (SelectedLSI != null)
            {
                SelectedLSI.Text = TxtEdit.Text;
            }
            SelectedLSI = null;
            TxtEdit.Text = "";
        }

        private void TxtEdit_Leave(object sender, EventArgs e)
        {
            HideTextEditor();
        }
        private void TxtEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                HideTextEditor();
        }

        private void Lsv_MouseDown(object sender, MouseEventArgs e)
        {
            HideTextEditor();
        }
        private void Lsv_MouseWheel(object sender, MouseEventArgs e)
        {
            HideTextEditor();
        }

        private void lvDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDevice.SelectedIndices.Count <= 0) return;
            int iDeviceSel = lvDevice.SelectedIndices[0];

            lbSelDevice.Text = lvDevice.Items[iDeviceSel].SubItems[1].Text;
            tbFromName.Text = lvDevice.Items[iDeviceSel].SubItems[1].Text;
        }

        private void btNew_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Click", 1);

            string sPath;
            sPath = Application.StartupPath + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sPath);
            if (di.Exists == false)
            {
                //if (Log.ShowMessageModal("Confirm", "JOB FILE을 생성하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want to Create JOB FILE?") != DialogResult.Yes) return;
                di.Create();
            }

            if (tbToName.Text == "")
            {
                //Log.ShowMessage("ERROR", "입력 된 JOB FILE 이름이 없습니다.");
                Log.ShowMessage("ERROR", "Enter JOB FILE name is missing.");
                return;
            }

            if (di.Exists == true)
            {
                //Log.ShowMessage("Warning", "같은 이름의 File이 존재합니다.");
                Log.ShowMessage("Warning", "File with the same name exists.");
                return;
            }

            string sPathInfo = Application.StartupPath + "\\JobFile\\";
            DirectoryInfo Info = new DirectoryInfo(sPathInfo);

            int iNo = lvDevice.Items.Count + 1;
            ListViewItem item = new ListViewItem(string.Format("{0}", iNo));
            item.SubItems.Add(tbToName.Text);
            item.SubItems.Add(Info.LastWriteTime.ToString());
            lvDevice.Items.Add(item);

            DispDevice();
        }

        private void btCopy_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Click", 1);

            if (tbFromName.Text == "")
            {
                //Log.ShowMessage("Error", "선택된 JOB FILE 이름이 없습니다.");
                Log.ShowMessage("Error", "The selected JOB FILE name is missing.");
                return;
            }

            if (tbToName.Text == "")
            {
                //Log.ShowMessage("Error", "입력된 JOB FILE 이름이 없습니다.");
                Log.ShowMessage("Error", "Enter JOB FILE name is missing.");
                return;
            }

            string sFromPath;
            string sToPath;
            sFromPath = Application.StartupPath + "\\JobFile\\" + tbFromName.Text;
            sToPath = Application.StartupPath + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sToPath);

            if (di.Exists == false)
            {
                //if (Log.ShowMessageModal("Confirm", tbToName.Text + " 을(를) " + tbFromName.Text + " 로 부터 복사 생성 하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want copy " + "to " + tbToName.Text + " From " + tbFromName.Text + "?") != DialogResult.Yes) return;
                CopyFolder(sFromPath, sToPath);
            }

            if (di.Exists == true)
            {
                //Log.ShowMessage("Warning", "같은 이름의 JOB FILE이 존재합니다.");
                Log.ShowMessage("Warning", "File with the same name exists.");
                return;
            }

            //COptnMan.SetCrntDev(tbToName.Text);

            string sPathInfo = Application.StartupPath + "\\JobFile\\";
            DirectoryInfo Info = new DirectoryInfo(sPathInfo);

            int iNo = lvDevice.Items.Count + 1;
            ListViewItem item = new ListViewItem(string.Format("{0}", iNo));
            item.SubItems.Add(tbToName.Text);
            item.SubItems.Add(Info.LastWriteTime.ToString());
            lvDevice.Items.Add(item);

            DispDevice();

        }

        private void btRename_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Click", 1);

            if (tbFromName.Text == "")
            {
                Log.ShowMessage("Error", "The selected JOB FILE name is missing.");
                return;
            }

            if (tbToName.Text == "")
            {
                Log.ShowMessage("Error", "Enter JOB FILE name is missing.");
                return;
            }

            string sFromPath;
            string sToPath;
            sFromPath = Application.StartupPath + "\\JobFile\\" + tbFromName.Text;
            sToPath = Application.StartupPath + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sToPath);

            if (di.Exists == false)
            {
                //if (Log.ShowMessageModal("Confirm", "잡파일 이름을 변경하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want to change the JOB FILE Name?") != DialogResult.Yes) return;
                RenameFolder(sFromPath, sToPath);
            }

            if (di.Exists == true)
            {
                //Log.ShowMessage("Warning", "같은 이름의 JOB FILE이 존재합니다.");
                Log.ShowMessage("Warning", "File with the same name exists.");
                return;
            }

            OM.SetCrntDev(tbToName.Text);
            DispDevice();

        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Click", 1);

            //Check None Name.
            if (tbFromName.Text == "")
            {   //아무것도 선택되지 않은 경우.
                //Log.ShowMessage("Error", "선택된 JOB FILE 이름이 없습니다." );
                Log.ShowMessage("Error", "The selected JOB FILE name is missing.");
                return;
            }

            string sPath;

            sPath = Application.StartupPath + "\\JobFile\\" + tbFromName.Text;

            DirectoryInfo di = new DirectoryInfo(sPath);

            if (di.Exists == true)
            {
                //if (Log.ShowMessageModal("Confirm", "선택된 JOB FILE을 삭제하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want to delete the selected JOB FILE?") != DialogResult.Yes) return;
                DeleteFolder(sPath);
            }

            if (tbFromName.Text == OM.GetCrntDev())
            {
                //Log.ShowMessage("Warning", "현재 사용중인 JOB FILE은 삭제할 수 없습니다." );
                Log.ShowMessage("Warning", "JOB FILE currently in use cannot be deleted.");
                return;
            }

            DispDevice();

            //Trace Log.
            //Log.Trace("JobFile", (tbFromName.Text + " is Deleted"));
        }

        private void btDownload_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Click", 1);

            if (tbFromName.Text == "") return;

            //if(OM.DevInfo.sMrkData != "")SEQ.Com[0].SendMsg(OM.DevInfo.sMrkData);
            if (lvDevice.SelectedIndices.Count <= 0) return;
            int iDeviceSel = lvDevice.SelectedIndices[0];

            string sName = lvDevice.Items[iDeviceSel].SubItems[1].Text;
            string sTemp = "DOWNLOAD JOB FILE. (" + lbSelDevice.Text + ")";

            if (OM.GetCrntDev() == sName)
            {
                //Log.ShowMessage( "ERROR", "현재 잡파일과 같은 잡파일 입니다.");
                Log.ShowMessage("ERROR", "This is the current file with the same file.");
                return;
            }
            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            OM.LoadJobFile(sName);
            UpdateLsv(true);
            propertyGrid1.SelectedObject = OM.Auto1;
            propertyGrid2.SelectedObject = OM.Manual;
            propertyGrid3.SelectedObject = OM.Mode;


            lbSelDevice.Text = "";
            tbFromName.Text = "";
            lbCrntDevice.Text = sName;

        }
        
        //Folder Copy하는 함수
        public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            foreach (string folder in folders)
            {
                string name = tbToName.Text;
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        /// <summary>
        /// FOLDER DELETE
        /// </summary>
        /// <param name="path">삭제할 폴더 경로</param>
        /// <returns>true,false반환</returns>
        public static bool DeleteFolder(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                    file.Attributes = FileAttributes.Normal;
                Directory.Delete(path, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// FOLDER NAME CHANGE
        /// </summary>
        /// <param name="path">이름변경할 폴더 경로, ex)C:\a\b.txt</param>
        /// <param name="changepath">이름변경하는 폴더 이름, ex)C:\a\a.txt</param>
        /// <returns>0은 성공 1은 실패</returns>
        public static int RenameFolder(string path, string changepath)
        {
            try
            {
                DirectoryInfo Dir = new DirectoryInfo(path);
                DirectoryInfo CDir = new DirectoryInfo(changepath);
                if (CDir.Exists)
                {
                    DeleteFolder(changepath);
                }
                if (Dir.Exists)
                {
                    Dir.MoveTo(changepath);
                    Dir = new DirectoryInfo(changepath);
                }
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }
        #endregion

        #region Operation Button

        private void btStart_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", 1);

            btStart.Enabled = false;
            if (Util.MT.GetAlarmSgnl(0))
            {
                Log.ShowMessage("Error", "Motor Alarm (Click the reset button)",1000);
            }

            if (!FrmKeyence.Start()) { return; }
            if (!FrmKeyence.Reset()) { return; }

            Run.bRun = true;
            
        }

        private void btLightOnOff_MouseDown(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Down", 1);

            Util.MT.JogN(0);
        }

        private void btLightOnOff_MouseUp(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Up", 1);

            Util.MT.Stop(0);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Down", 1);

            Util.MT.JogP(0);
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Up", 1);

            Util.MT.Stop(0);
        }

        private void btZero_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", 1);

            if(!Util.MT.GetStop(0)) return;

            if (!Util.MT.GetHomeDone(0)) Util.MT.GoHome(0);
            else                         Util.MT.GoAbsMan(0, 0);
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", 1);

            Run.bReqStop = true;

            if (!Run.bRun) Util.MT.StopAll();
        }

        #endregion


        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            lbCrntDevice.Text = OM.GetCrntDev();
            //lbPara.Text = "PARAMETER_" + OM.GetCrntDev();

            bool bLimN = Util.MT.GetNLimSnsr(0);
            bool bLimP = Util.MT.GetPLimSnsr(0);
            //Error
            if (Util.MT.GetHomeDone(0)) {
                if (bLimN) { Log.ShowMessageFunc("Motor - Limit Sensor touched Move the motor + direction(UpSide)"); }
                if (bLimP) { Log.ShowMessageFunc("Motor + Limit Sensor touched Move the motor - direction(DownSide)"); }
            }

            //Lift Up / Dn
            if (Util.MT.GetHomeDone(0))
            {
                if (!Run.bRun)
                {
                    //1.0.0.1
                    //if (Util.MT.GetEncPos(0) > Util.MT.GetMinPosition(0))
                    //{
                    //    if (Util.MT.GetX(0, 3) && Util.MT.GetStop(0) && !bLimN) { Util.MT.JogN(0); bDown1 = true; }
                    //}
                    //if (Util.MT.GetEncPos(0) < Util.MT.GetMaxPosition(0))
                    //{
                    //    if (Util.MT.GetX(0, 4) && Util.MT.GetStop(0) && !bLimP) { Util.MT.JogP(0); bDown2 = true; }
                    //}
                    //if (!Util.MT.GetX(0, 3) && bDown1) { Util.MT.Stop(0); bDown1 = false; }
                    //if (!Util.MT.GetX(0, 4) && bDown2) { Util.MT.Stop(0); bDown2 = false; }

                    if (Util.MT.GetEncPos(0) > Util.MT.GetMinPosition(0))
                    {
                        if (Util.MT.GetX(0, 4) && Util.MT.GetStop(0) && !bLimN) { Util.MT.JogN(0); bDown1 = true; }
                    }
                    if (Util.MT.GetEncPos(0) < Util.MT.GetMaxPosition(0))
                    {
                        if (Util.MT.GetX(0, 3) && Util.MT.GetStop(0) && !bLimP) { Util.MT.JogP(0); bDown2 = true; }
                    }
                    if (!Util.MT.GetX(0, 4) && bDown1) { Util.MT.Stop(0); bDown1 = false; }
                    if (!Util.MT.GetX(0, 3) && bDown2) { Util.MT.Stop(0); bDown2 = false; }

                    //     if (Util.MT.GetX(0, 3) && Util.MT.GetStop(0) && !bLimN && !bLimP) { Util.MT.GoIncMan(0,-10); bDown1 = true; }
                    //else if (Util.MT.GetX(0, 4) && Util.MT.GetStop(0) && !bLimN && !bLimP) { Util.MT.GoIncMan(0, 10); bDown2 = true; }


                }
            }

            //Status Check
            if (Run.bRun)
            {
                     if(OM.Mode.iMode == Mode.AutoMode1 ) btStatus.Text = "AUTO MODE1 RUN";
                else if(OM.Mode.iMode == Mode.AutoMode2 ) btStatus.Text = "AUTO MODE2 RUN";
                else if(OM.Mode.iMode == Mode.ManualMode) btStatus.Text = "MANUAL RUN";
            }
            else
            {
                btStatus.Text = "STOP";
            }

            //Button Enable Change
            btStart.Enabled    = !Run.bRun && Util.MT.GetHomeDoneAll();
            btZero.Enabled     = !Run.bRun;
            btLiftUp.Enabled   = !Run.bRun && Util.MT.GetHomeDoneAll();
            btLiftDn.Enabled   = !Run.bRun && Util.MT.GetHomeDoneAll();
            btDownload.Enabled = !Run.bRun;
            btRename.Enabled   = !Run.bRun;
            btDelete.Enabled   = !Run.bRun;
            btSave.Enabled     = !Run.bRun;

            FrmKeyence.btSaveMotr.Enabled = !Run.bRun;
            FrmMotion.btSaveMotr.Enabled  = !Run.bRun;

            //Image Move
            //105, 60 ~250
            double dPos    = Util.MT.GetCmdPos(0)/10; //dTemp;//
            //1.0.0.1
            //double dHeight = 250 - (dPos/210 * 190) ;
            double dHeight = 60 + (dPos / 210 * 190);
            pnMove.Location = new Point(109,(int)dHeight);
            //if(dTemp > 210) dTemp = 0;
            //else dTemp++;

            //Motion Check
            lbAxisName.Text = "[" + 0 + "] " + Util.MT.GetName(0);

            lbStat1.ForeColor = Util.MT.GetNLimSnsr  (0) ? Color.Red : Color.Silver;
            lbStat2.ForeColor = Util.MT.GetHomeSnsr  (0) ? Color.Red : Color.Silver;
            lbStat3.ForeColor = Util.MT.GetPLimSnsr  (0) ? Color.Red : Color.Silver;
            lbStat4.ForeColor = Util.MT.GetHomeDone  (0) ? Color.Red : Color.Silver;
            lbStat5.ForeColor = Util.MT.GetAlarmSgnl (0) ? Color.Red : Color.Silver;
            lbStat6.ForeColor = Util.MT.GetStop      (0) ? Color.Red : Color.Silver;
            lbStat7.ForeColor = Util.MT.GetInPosSgnl (0) ? Color.Red : Color.Silver;  
            lbStat8.ForeColor = Util.MT.GetServo     (0) ? Color.Red : Color.Silver;
            
            string sTemp = string.Format("{0:0.0000}", Util.MT.GetCmdPos(0) / 10d);
            lbCmdPos.Text     = sTemp;

            if (Run.bRun)
            {
                //LsvDisp.EnsureVisible(CConfig.StrToIntDef(sTemp,0) );
                double dTemp = CConfig.StrToDoubleDef(sTemp, 0);
                //1.0.0.1
                //int iNo = OM.m_iMaxArray - 1 - (int)dTemp ;
                int iNo = (int)dTemp;
                if(iNo >= 0 && iNo < LsvDisp.Items.Count) LsvDisp.EnsureVisible(iNo);
                //LsvDisp.TopItem = LsvDisp.Items[iNo];
            }

            //Inspection Count Check
            LS9IF_STORAGE_INFO stStorageInfo = new LS9IF_STORAGE_INFO();

            int rc = NativeMethods.LS9IF_GetStorageStatus(ref stStorageInfo);
            if (rc == (int)Rc.Ok)
            { 
                //if(stStorageInfo.byStatus == 0x00) lbCount.Text = stStorageInfo.dwStorageCnt.ToString();
                lbCount.Text = stStorageInfo.dwStorageCnt.ToString();
            }
            else
            {
                lbCount.Text = "NOT";
            }

            lbNowCnt.Text = Run.iNowCnt.ToString();
            //stringBuilder.AppendLine("Status:" + stStorageInfo.byStatus.ToString());
            //stringBuilder.AppendLine("Version:" + stStorageInfo.wStrageVer.ToString());
            //stringBuilder.AppendLine("Number:" + stStorageInfo.dwStorageCnt.ToString());

            timer1.Enabled = true;
        }
        static double dTemp = 0 ;
        
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;

            LsvDisp.Refresh();

            timer2.Enabled = true;
        }

        private void LsvDisp_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem lvi = new ListViewItem(); 	// create a listviewitem object

            string sTemp = "";

            //1.0.0.1
            //int i = OM.m_iMaxArray - e.ItemIndex - 1;
            int i = e.ItemIndex;

            int iCnt = 0;
            lvi = new ListViewItem((i).ToString());
            for (int j = 0; j < CRun.iMeasureMaxCnt; j++)
            {
                     if (OM.CmnOptn.bOut1) { sTemp = CRun.sOut1[i, j] ; iCnt = 1;}
                else if (OM.CmnOptn.bOut2) { sTemp = CRun.sOut2[i, j] ; iCnt = 2;}
                else if (OM.CmnOptn.bOut3) { sTemp = CRun.sOut3[i, j] ; iCnt = 3;}
                else if (OM.CmnOptn.bOut4) { sTemp = CRun.sOut4[i, j] ; iCnt = 4;}

                //if (OM.CmnOptn.bOut1            ) sTemp += "," + CRun.sOut1[i, j] ;
                if (OM.CmnOptn.bOut2 && iCnt < 2) sTemp += " , " + CRun.sOut2[i, j] ;
                if (OM.CmnOptn.bOut3 && iCnt < 3) sTemp += " , " + CRun.sOut3[i, j] ;
                if (OM.CmnOptn.bOut4            ) sTemp += " , " + CRun.sOut4[i, j] ;
                
                lvi.SubItems.Add(sTemp);
            }

            e.Item = lvi; 		// assign item to event argument's item-property
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = CConfig.StrToIntDef(textBox1.Text,0) ;
            int j = CConfig.StrToIntDef(textBox2.Text,0) ;
            CRun.sOut1[i, j] = textBox3.Text ;
            CRun.sOut2[i, j] = textBox3.Text ;
            CRun.sOut3[i, j] = textBox3.Text ;
            CRun.sOut4[i, j] = textBox3.Text ;
        }

        private void btLiftUp_Click(object sender, EventArgs e)
        {

        }

        private void LvAuto2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Are you sure you want to clear the list?") != DialogResult.Yes) return;
            for(int i=0; i< OM.m_iMaxArray; i++)
            {
                OM.Auto2[i].iATime = 0;
                OM.Auto2[i].iBTime = 0;
                OM.Auto2[i].iCount = 0;
            }
            UpdateLsv(true);

        }

        private void gRAPHToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void oUT12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sText = gRAPHToolStripMenuItem.Text;
            Log.Trace(sFormText + sText + " Menu Click", 1);

            FrmGraph12.Show();
        }

        private void oUT3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sText = gRAPHToolStripMenuItem.Text;
            Log.Trace(sFormText + sText + " Menu Click", 1);

            FrmGraph3.Show();

        }

        private void oUT4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sText = gRAPHToolStripMenuItem.Text;
            Log.Trace(sFormText + sText + " Menu Click", 1);

            FrmGraph4.Show();

        }

        private void FormMain_QueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e)
        {

        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            FrmGraph12.Show(512,318);
            FrmGraph12.Visible = false;
            FrmGraph3.Show();
            FrmGraph3.Visible = false;
            FrmGraph4.Show();
            FrmGraph4.Visible = false;
        }




        //private const int WM_HSCROLL = 0x114;
        //private const int WM_VSCROLL = 0x115;
        //private const int MOUSEWHEEL = 0x020A;
        //private const int KEYDOWN = 0x0100;

        //public event EventHandler Scroll;

        //protected void OnScroll()
        //{
        //   if (this.Scroll != null)
        //      this.Scroll(this, EventArgs.Empty);

        //   HideTextEditor();
        //}

        //protected override void WndProc(ref System.Windows.Forms.Message m)
        //{
        //   base.WndProc(ref m);
        //   if (m.Msg == MOUSEWHEEL || m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL)
        //   {
        //       this.OnScroll();
        //   }
        //}

        //public void SetValue(double _dValue)
        //{
        //    double dValue = _dValue;

        //    //UI에 접근 하기 위한 인보크
        //    if (Lsv.Parent != null)
        //    {
        //        if (Lsv.InvokeRequired)
        //        {
        //            Lsv.Invoke(new MethodInvoker(delegate ()
        //            {
        //                // code for running in ui thread
        //                Lsv.Items.SubItems[1].Text = dValue.ToString();

        //            }));
        //            //Lsv.Items[(int)_uPstnNo].SubItems[1].Text = _dValue.ToString();
        //            //}));
        //        }
        //        else
        //        {
        //            Lsv.Items.SubItems[1].Text = dValue.ToString();
        //        }
        //    }
        //}




        //---- 
        //END
        //----
    }


}
