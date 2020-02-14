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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDll
{
    public partial class FormRecipe : Form
    {
        public string sFolder = "";
        public string sRecipe = "";

        public delegate string dgRecipeName(string _sName = "") ;
        public                 dgRecipeName RecipeName ;

        public delegate bool dgLoadRecipe(string _sName);
        public event         dgLoadRecipe LoadRecipe;

        public FormRecipe(string _sFolder, dgRecipeName _sRecipeName, dgLoadRecipe _LoadRecipe)
        {
            InitializeComponent();

            sFolder     = _sFolder    ;
            RecipeName  = _sRecipeName;
            LoadRecipe  = _LoadRecipe ;

            DispDevice();
            
        }

        public void DispDevice()
        {
            lvDevice.Items.Clear();

            InitDevice();

            lbCrntDevice.Text = RecipeName();
            sRecipe           = RecipeName();
        }

        private void InitDevice()
        {
            //Device ListView
            lvDevice.Clear();
            lvDevice.View = View.Details;
            lvDevice.HeaderStyle = ColumnHeaderStyle.None;
            lvDevice.FullRowSelect = true;
            lvDevice.GridLines = true;

            lvDevice.Columns.Add("", 25, HorizontalAlignment.Left);
            //lvDevice.Columns.Add("", 210, HorizontalAlignment.Left);
            lvDevice.Columns.Add("", (int)(lvDevice.Width/2), HorizontalAlignment.Left);
            //lvDevice.Columns.Add("", 170, HorizontalAlignment.Left);
            lvDevice.Columns.Add("", (int)(lvDevice.Width/2.5), HorizontalAlignment.Left);

            string sPath = Application.StartupPath + "\\JobFile\\";
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

        private void btNew_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(this.GetType().Name + " " + sText + " Button Click", 1);

            string sPath = sFolder + tbToName.Text;
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
            Log.Trace(this.GetType().Name + " "  + sText + " Button Click", 1);

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

            string sFromPath = sFolder + tbFromName.Text;
            string sToPath   = sFolder + tbToName  .Text;
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
            Log.Trace(this.GetType().Name + " " + sText + " Button Click", 1);

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

            string sFromPath = sFolder + tbFromName.Text;
            string sToPath   = sFolder + tbToName  .Text;
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

            //OM.SetCrntDev(tbToName.Text);
            RecipeName(tbToName.Text);
            
            sRecipe = tbToName.Text;
            RecipeName(sRecipe);
            DispDevice();

        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(this.GetType().Name + " " + sText + " Button Click", 1);

            //Check None Name.
            if (tbFromName.Text == "")
            {   //아무것도 선택되지 않은 경우.
                //Log.ShowMessage("Error", "선택된 JOB FILE 이름이 없습니다." );
                Log.ShowMessage("Error", "The selected JOB FILE name is missing.");
                return;
            }

            string sPath = sFolder + tbFromName.Text;
            DirectoryInfo di = new DirectoryInfo(sPath);

            if (di.Exists == true)
            {
                //if (Log.ShowMessageModal("Confirm", "선택된 JOB FILE을 삭제하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want to delete the selected JOB FILE?") != DialogResult.Yes) return;
                DeleteFolder(sPath);
            }

            if (tbFromName.Text == sRecipe)
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
            Log.Trace(this.GetType().Name + " " + sText + " Button Click", 1);

            if (tbFromName.Text == "") return;

            //if(OM.DevInfo.sMrkData != "")SEQ.Com[0].SendMsg(OM.DevInfo.sMrkData);
            if (lvDevice.SelectedIndices.Count <= 0) return;
            int iDeviceSel = lvDevice.SelectedIndices[0];

            string sName = lvDevice.Items[iDeviceSel].SubItems[1].Text;
            string sTemp = "DOWNLOAD JOB FILE. (" + lbSelDevice.Text + ")";

            if (sRecipe == sName)
            {
                //Log.ShowMessage( "ERROR", "현재 잡파일과 같은 잡파일 입니다.");
                Log.ShowMessage("ERROR", "This is the current file with the same file.");
                return;
            }
            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            lbSelDevice.Text = "";
            tbFromName.Text = "";
            lbCrntDevice.Text = sName;

            sRecipe = sName;
            RecipeName(sRecipe);
            LoadRecipe(sName);
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

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void FormRecipe_VisibleChanged(object sender, EventArgs e)
        {
            timer1.Enabled = Visible;
        }

        private void lvDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDevice.SelectedIndices.Count <= 0) return;
            int iDeviceSel = lvDevice.SelectedIndices[0];

            lbSelDevice.Text = lvDevice.Items[iDeviceSel].SubItems[1].Text;
            tbFromName.Text = lvDevice.Items[iDeviceSel].SubItems[1].Text;
        }

        private void FormRecipe_Resize(object sender, EventArgs e)
        {
            InitDevice();
        }
    }
}
