﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMMON;
using System.IO;
using System.Reflection;
using SML2;

namespace Machine
{
    public partial class FormDevice : Form
    {
        public static FormDevice FrmDevice ;

        FormMain FrmMain;
        //CArray[] Aray;

        public FormDevice(FormMain _FrmMain)
        {
            
            InitializeComponent();

            tmUpdate.Enabled = true;

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

        

        public int    m_iRowCount = 70;
        public string m_sParaFolderPath;

        public int    _iRowCount { get { return m_iRowCount; } }


      

        private void InitDevice()
        {
            

            //Device ListView
            lvDevice.Clear();
            lvDevice.View = View.Details;
            lvDevice.FullRowSelect = true;
            lvDevice.GridLines = true;
        
            lvDevice.Columns.Add("", 30, HorizontalAlignment.Left);
            lvDevice.Columns.Add("", 340, HorizontalAlignment.Left);
            lvDevice.Columns.Add("", 260, HorizontalAlignment.Left);

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

            
            tmUpdate.Enabled = true;
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
            tbFromName .Text = lvDevice.Items[iDeviceSel].SubItems[1].Text;
           
        }

        //Folder Copy하는 함수
        public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            string[] files   = Directory.GetFiles      (sourceFolder);
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
            int iLevel = (int)FormPassword.GetLevel();
            switch (iLevel)
            {
                case (int)EN_LEVEL.Operator: btSetting.Enabled = false; break;
                case (int)EN_LEVEL.Engineer: btSetting.Enabled = true; break;
                case (int)EN_LEVEL.Master: btSetting.Enabled = true; break;
                default: break;
            }

            tmUpdate.Enabled = true;
        }

        private void btSetting_Click(object sender, EventArgs e)    //DeviceSet Form 띄움
        {
            this.Hide();
            FrmMain.FrmDeviceSet.Show();
            FrmMain.FrmDeviceSet.UpdateNodePos(true);
            FrmMain.FrmDeviceSet.UpdateDevOptn(true);
            FrmMain.FrmDeviceSet.UpdateDevInfo(true);
            //FrmMain.ShowPage(6);
        }

        private void btDownload_Click(object sender, EventArgs e)             //HRM-930B 참고
        {
            if (tbFromName.Text == "") return;

            //Check Running Status
//            bool bAllArayNone = DM.ARAY[(int)ri.SLD].CheckAllStat(cs.None);

            //if (LOT.GetLotOpen())
            //{
            //    if (!bAllArayNone)
            //    {
            //        FM_MsgOk("Error", "자제나 메거진이 남아 있으면 잡파일을 바꿀수 없습니다.");
            //        return;
            //    }
            //}

            if (lvDevice.SelectedIndices.Count <= 0) return;
            int iDeviceSel = lvDevice.SelectedIndices[0];

            string sName = lvDevice.Items[iDeviceSel].SubItems[1].Text;
            string sTemp = "DOWNLOAD JOB FILE. (" + lbSelDevice.Text + ")";

            if (OM.GetCrntDev() == sName)
            {
                Log.ShowMessage( "ERROR", "현재 잡파일과 같은 잡파일 입니다.");
                return;
            }
            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            OM.LoadJobFile(sName);                                           pbStatus.Minimum = 0;
            PM.Load(sName);                                                  pbStatus.Value    = 70;
            pbStatus.Value = 100;

            lbSelDevice.Text = "";
            tbFromName.Text = "";
            lbCrntDevice.Text = sName;

            PM.UpdatePstn(true);

            //            DM.ARAY[(int)ri.SLD].SetMaxColRow(OM.DevInfo.iSTGColCnt, OM.DevInfo.iSTGRowCnt); 


            //VSN_L.SendJobChange(sName);
            //VSN_R.SendJobChange(sName);



            //세이브버튼 누를때 , 디바이스셑 크리에트할때 , 디바이스 체인지 누를때...
            //Rs232_DisprFt.SetPTV(OM.DevOptn.dDspPrsPres , 10 , OM.DevOptn.dDspVacPres);
            //Rs232_DisprRr.SetPTV(OM.DevOptn.dDspPrsPres , 10 , OM.DevOptn.dDspVacPres);
            //CDelayTimer TimeOut;
            //Rs232_DisprFt.SetLoadCh(OM.DevOptn.iDspChFt + 1);
            //TimeOut.Clear();
            //while (!Rs232_DisprFt.GetMsgEnd())
            //{ //메세지 다 주고 받을때까지 기다림.
            //    Sleep(1);
            //    if (TimeOut.OnDelay(true, 1000))
            //    {
            //        FM_MsgOk("Error", "프론트 디스펜서 채널 로드 통신타임아웃");
            //        break;
            //    }
            //}
            //if (Rs232_DisprFt.GetErrMsg() != "")
            //{
            //    FM_MsgOk("Disp Ft Error", Rs232_DisprFt.GetErrMsg().c_str());
            //}
            //
            //
            //Rs232_DisprRr.SetLoadCh(OM.DevOptn.iDspChRr + 1);
            //TimeOut.Clear();
            //while (!Rs232_DisprRr.GetMsgEnd())
            //{ //메세지 다 주고 받을때까지 기다림.
            //    Sleep(1);
            //    if (TimeOut.OnDelay(true, 1000))
            //    {
            //        FM_MsgOk("Error", "리어 디스펜서 채널 로드 통신타임아웃");
            //        break;
            //    }
            //}
            //if (Rs232_DisprRr.GetErrMsg() != "")
            //{
            //    FM_MsgOk("Disp Rr Error", Rs232_DisprRr.GetErrMsg().c_str());
            //}
            //

            //            DM.ARAY[(int)ri.SLD].SetStat(cs.None);
            DM.LoadMap();

        }

        private void btNew_Click(object sender, EventArgs e)
        {
            string sPath;
            sPath = Application.StartupPath + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sPath);
            if (di.Exists == false)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to Create JOB FILE?") != DialogResult.Yes) return;
                di.Create();
            }

            if (tbToName.Text == "")
            {  
                Log.ShowMessage("ERROR", "입력 된 JOB FILE 이름이 없습니다.");
                return;
            }

            if (di.Exists == true)
            {
                Log.ShowMessage("Warning", "같은 이름의 File이 존재합니다.");
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
            Log.Trace("JobFile", (tbToName.Text + " is Maked"));
        }

        private void btCopy_Click(object sender, EventArgs e)
        {
            if (tbFromName.Text == "")
            {   
                Log.ShowMessage("Error", "선택된 JOB FILE 이름이 없습니다.");
                return;
            }
            
            if (tbToName.Text == "")
            {
                Log.ShowMessage("Error", "입력된 JOB FILE 이름이 없습니다.");
                return;
            }

            string sFromPath;
            string sToPath;
            sFromPath = Application.StartupPath + "\\JobFile\\" + tbFromName.Text;
            sToPath = Application.StartupPath + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sToPath);

            if (di.Exists == false)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want copy " + "to " + tbToName.Text+ " From " + tbFromName.Text+ "?") != DialogResult.Yes) return;
                CopyFolder(sFromPath, sToPath);
            }
            
            if (di.Exists == true)
            {
                Log.ShowMessage("Warning", "같은 이름의 JOB FILE이 존재합니다.");
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
            Log.Trace("JobFile", (tbToName.Text + " is Maked by Copy"));
        }

        private void btRename_Click(object sender, EventArgs e)
        {
            if (tbFromName.Text == "")
            {  
                Log.ShowMessage("Error", "선택된 JOB FILE 이름이 없습니다.");
                return;
            }

            if (tbToName.Text == "")
            {  
                Log.ShowMessage("Error", "입력된 JOB FILE 이름이 없습니다.");
                return;
            }
            
            string sFromPath;
            string sToPath;
            sFromPath = Application.StartupPath + "\\JobFile\\" + tbFromName.Text;
            sToPath = Application.StartupPath + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sToPath);

            if (di.Exists == false)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to change the JOB FILE Name?") != DialogResult.Yes) return;

                RenameFolder(sFromPath, sToPath);
            }
            
            if (di.Exists == true)
            {
                Log.ShowMessage("Warning", "같은 이름의 JOB FILE이 존재합니다.");
                return;
            }

            OM.SetCrntDev(tbToName.Text);
            DispDevice();

            //Trace Log.
            Log.Trace("JobFile", (tbToName.Text + " is Maked by Rename"));
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
             
            //Check None Name.
            if (tbFromName.Text == "")
            {   //아무것도 선택되지 않은 경우.
                Log.ShowMessage("Error", "선택된 JOB FILE 이름이 없습니다." );
                return;
            }

            string sPath;
            
            sPath = Application.StartupPath + "\\JobFile\\" + tbFromName.Text;

            DirectoryInfo di = new DirectoryInfo(sPath);

            if (di.Exists == true)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to delete the selected JOB FILE?") != DialogResult.Yes) return;
                DeleteFolder(sPath);
            }
            
            if (tbFromName.Text == OM.GetCrntDev())
            {
                Log.ShowMessage("Warning", "현재 사용중인 JOB FILE은 삭제할 수 없습니다." );
                return;
            }

            DispDevice();
            
            //Trace Log.
            Log.Trace("JobFile", (tbFromName.Text + " is Deleted"));
         

            //pbStatus.Minimum = 0;
            //UserFile.GridSearchDir(ExtractFilePath(Application->ExeName) + "JobFile", sgDevice, 1, true);     // 디렉토리 읽어와서 날짜와 알파벳 순으로 정렬
            //sgDevice->Row = 0;


        }

        private void FormDevice_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

       



        
    }
}
