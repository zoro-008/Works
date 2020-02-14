using System;
using System.Windows.Forms;
using System.Threading;
using COMMON;
using System.IO;
using System.Reflection;
using SML;
using System.Collections;

namespace Machine
{
    public partial class FormDevice : Form
    {
        //public static FormDevice FrmDevice ;

        FormMain FrmMain;
        //CArray[] Aray;

        private const string sFormText = "Form Device ";

        public FormDevice(FormMain _FrmMain)
        {

            InitializeComponent();

            //tmUpdate.Enabled = true;

            //this.TopLevel = false;
            //this.Parent = _pnBase;
            FrmMain = _FrmMain;

            InitDevice();
        }

        public struct TPara
        {
            public string sName;
        }

        public TPara Para;
        public TPara[] m_aRow;

        public int m_iRowCount = 70;
        public string m_sParaFolderPath;

        public int _iRowCount { get { return m_iRowCount; } }

        private void InitDevice()
        {
            //Device ListView
            lvDevice.Clear();
            lvDevice.Sorting = SortOrder.None;
            lvDevice.View = View.Details;
            lvDevice.FullRowSelect = true;
            //lvDevice.AllowColumnReorder = true;
            //lvDevice.GridLines = true;
            //lvDevice.Scrollable = true;
            //lvDevice.HideSelection = false;

            lvDevice.Columns.Add("NO", 45, HorizontalAlignment.Left);
            lvDevice.Columns.Add("NAME", 380, HorizontalAlignment.Left);
            lvDevice.Columns.Add("MODIFIED DATE", 200, HorizontalAlignment.Left);

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



        public void DispDevice()
        {
            lvDevice.Items.Clear();

            InitDevice();
        }




        private void lvDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDevice.SelectedIndices.Count <= 0) return;
            int iDeviceSel = lvDevice.SelectedIndices[0];

            lbSelDevice.Text = lvDevice.Items[iDeviceSel].SubItems[1].Text;
            tbFromName.Text = lvDevice.Items[iDeviceSel].SubItems[1].Text;

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

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            lbCrntDevice.Text = OM.GetCrntDev();

            //Download 버튼 활성화 조건(나중에 확인 진섭)
            if (SEQ._iSeqStat == EN_SEQ_STAT.Stop || SEQ._iSeqStat == EN_SEQ_STAT.Error)
            {
                btDownload.Enabled = true;
            }

            //접근레벨에 따른 Setting 버튼 활성화
            //int iLevel = (int)FormPassword.GetLevel();
            //switch (iLevel)
            //{
            //    case (int)EN_LEVEL.Operator: btSetting.Enabled = false; break;
            //    case (int)EN_LEVEL.Engineer: btSetting.Enabled = true; break;
            //    case (int)EN_LEVEL.Master: btSetting.Enabled = true; break;
            //    default: break;
            //}
            int iLevel = (int)SM.FrmLogOn.GetLevel();
            switch (iLevel)
            {
                case (int)EN_LEVEL.Operator: btSetting.Enabled = false; break;
                case (int)EN_LEVEL.Engineer: btSetting.Enabled = true; break;
                case (int)EN_LEVEL.Master: btSetting.Enabled = true; break;
                default: break;
            }

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }

        private void btSetting_Click(object sender, EventArgs e)    //DeviceSet Form 띄움
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            this.Hide();
            FrmMain.FrmDeviceSet.Show();
            //FrmMain.FrmDeviceSet.UpdateDevInfo(true);
            PM.UpdatePstn(true);

            //PM.Load(OM.GetCrntDev());

        }

        private void btDownload_Click(object sender, EventArgs e)             //HRM-930B 참고
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (tbFromName.Text == "") return;

            //Check Running Status
            //            bool bAllArayNone = DM.ARAY[(int)ri.SLD].CheckAllStat(cs.None);

            if (LOT.GetLotOpen())
            {
                //Log.ShowMessage("Error", "자재나 메거진이 남아 있으면 잡파일을 바꿀수 없습니다.");
                Log.ShowMessage("Error", "Please check the status of the Lot(Need to Lot End).");
                return;
            }

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

            SaveDeviceLog(0, OM.GetCrntDev(), sName);
            //FrmMain.FrmDeviceSet.
            OM.LoadJobFile(sName); pbStatus.Minimum = 30;
            //SEQ.Visn.SendJobChange(sName); pbStatus.Minimum = 30;
            PM.Load(sName); pbStatus.Value = 70;
            CConfig Config = new CConfig();
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + sName + "\\TrayMask.ini";
            Config.Load(sDevOptnPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            //DM.ARAY[ri.MASK].Load(Config, true);

            pbStatus.Value = 100;

            //CDelayTimer TimeOut = new CDelayTimer();
            //TimeOut.Clear();
            //while (!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.JobChange))
            //{
            //    Thread.Sleep(1);
            //    if (TimeOut.OnDelay(5000))
            //    {
            //        ML.ER_SetErr(ei.VSN_ComErr, "JobFile Change TimeOut");
            //        break;
            //    }
            //}

            lbSelDevice.Text = "";
            tbFromName.Text = "";
            lbCrntDevice.Text = sName;

            PM.UpdatePstn(true);


            //OM.TrayMask.SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY);
            //OM.TrayMask.SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY);
            //OM.SaveTrayMask();


            DM.ARAY[ri.ARAY].SetMaxColRow(1, 1);

            //SEQ.VisnRB.SendJobChange(sName);

            //CDelayTimer TimeOut = new CDelayTimer();
            //TimeOut.Clear();
            //while(!SEQ.VisnRB.GetSendCycleEnd(VisnCom.vs.JobChange  )){
            //    Thread.Sleep(1);
            //    if(TimeOut.OnDelay(5000)) { 
            //        SM.ER_SetErr(ei.VSN_ComErr,"잡체인지 비전 통신 타임아웃");
            //        break;
            //    }
            //}
        }

        private void btNew_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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


            //Trace Log.
            //Log.Trace("JobFile", (tbToName.Text + " is Maked"));
            SaveDeviceLog(1, tbToName.Text);
        }

        private void btCopy_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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

            //Trace Log.
            //Log.Trace("JobFile", (tbToName.Text + " is Maked by Copy"));
            SaveDeviceLog(2, tbFromName.Text, tbToName.Text);
        }

        private void btRename_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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

            //Trace Log.
            //Log.Trace("JobFile", (tbToName.Text + " is Maked by Rename"));
            SaveDeviceLog(3, tbFromName.Text, tbToName.Text);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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
            SaveDeviceLog(4, tbFromName.Text);

            //pbStatus.Minimum = 0;
            //UserFile.GridSearchDir(ExtractFilePath(Application->ExeName) + "JobFile", sgDevice, 1, true);     // 디렉토리 읽어와서 날짜와 알파벳 순으로 정렬
            //sgDevice->Row = 0;


        }

        private void FormDevice_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        private void SaveDeviceLog(int _Idx, string _sBf , string _sAt = "")
        {
            string sTemp = "";
            switch (_Idx)
            {
                case 1:
                    sTemp = "# NEW DEVICE NAME     : " + _sBf ;
                    Log.Trace(sTemp, ti.Dev);
                    break;//New.

                case 2:
                    sTemp = "# CURRENT DEVICE NAME : " + _sBf ;
                    Log.Trace(sTemp, ti.Dev);
                    sTemp = "# COPYED  DEVICE NAME : " + _sAt ;
                    Log.Trace(sTemp, ti.Dev);
                    break;//Copy.

                case 3:
                    sTemp = "# CURRENT DEVICE NAME : " + _sBf ;
                    Log.Trace(sTemp, ti.Dev);
                    sTemp = "# RENAME  DEVICE NAME : " + _sAt ;
                    Log.Trace(sTemp, ti.Dev);
                    break;//Rename.

                case 4:
                    sTemp = "# DELETE DEVICE NAME  : " + _sBf ;
                    Log.Trace(sTemp, ti.Dev);
                    break;//Delete

                case 0:
                    sTemp = "# CHANGED DEVICE NAME : " + _sBf ;
                    Log.Trace(sTemp, ti.Dev);
                    sTemp = "# CURRENT DEVICE NAME : " + _sAt ;
                    Log.Trace(sTemp, ti.Dev);
                    break;//Changed
            }
        }

        private void FormDevice_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FormDevice_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) tmUpdate.Enabled = true;
        }

        private void lvDevice_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
               return;
            }
 
            lvDevice.Columns[e.Column].Text = lvDevice.Columns[e.Column].Text.Replace(" ▼", "");
            lvDevice.Columns[e.Column].Text = lvDevice.Columns[e.Column].Text.Replace(" ▲", "");
  
 
            if (this.lvDevice.Sorting == SortOrder.Ascending || lvDevice.Sorting == SortOrder.None)
            {
                this.lvDevice.ListViewItemSorter = new ListviewItemComparer(e.Column, "desc");
                lvDevice.Sorting = SortOrder.Descending;
                lvDevice.Columns[e.Column].Text = lvDevice.Columns[e.Column].Text + " ▼";
            }
            else
            {
                this.lvDevice.ListViewItemSorter = new ListviewItemComparer(e.Column, "asc");
                lvDevice.Sorting = SortOrder.Ascending;
                lvDevice.Columns[e.Column].Text = lvDevice.Columns[e.Column].Text + " ▲";
            }
 
            lvDevice.Sort();

        }
    }

    class ListviewItemComparer : IComparer
    {
        private int col;
        public string sort = "asc";
        public ListviewItemComparer()
        {
            col = 0;
        }
 
        public ListviewItemComparer(int column, string sort)
        {
            col = column;
            this.sort = sort;
        }
 
        public int Compare(object x, object y)
        {
            if (sort == "asc")
            {
                return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }
            else
            {
                return String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
            }
        }
    }


}
