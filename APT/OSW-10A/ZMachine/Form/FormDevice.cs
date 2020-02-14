﻿using COMMON;
using SML2;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormDevice : Form
    {
        //public static FormDevice FrmDevice ;

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
            //int iLevel = (int)FormPassword.GetLevel();
            //switch (iLevel)
            //{
            //    case (int)EN_LEVEL.Operator: btSetting.Enabled = false; break;
            //    case (int)EN_LEVEL.Engineer: btSetting.Enabled = true; break;
            //    case (int)EN_LEVEL.Master: btSetting.Enabled = true; break;
            //    default: break;
            //}
            int iLevel = (int)SML.FrmLogOn.GetLevel();
            switch (iLevel)
            {
                case (int)EN_LEVEL.Operator: btSetting.Enabled = false; break;
                case (int)EN_LEVEL.Engineer: btSetting.Enabled = true ; break;
                case (int)EN_LEVEL.Master  : btSetting.Enabled = true ; break;
                case (int)EN_LEVEL.Control : btSetting.Enabled = true ; break;
                default: break;
            }

            tmUpdate.Enabled = true;
        }

        private void btSetting_Click(object sender, EventArgs e)    //DeviceSet Form 띄움
        {
            this.Hide();
            FrmMain.FrmDeviceSet.Show();
            FrmMain.FrmDeviceSet.UpdateDevOptn(true);
            FrmMain.FrmDeviceSet.UpdateDevInfo(true);
            PM.UpdatePstn(true);
            
            PM.Load(OM.GetCrntDev());
            
        }

        private void btDownload_Click(object sender, EventArgs e)             //HRM-930B 참고
        {
            if (tbFromName.Text == "") return;
   
            //Check Running Status
//            bool bAllArayNone = DM.ARAY[(int)ri.SLD].CheckAllStat(cs.None);

            if (LOT.GetLotOpen())
            {
                //Log.ShowMessage("Error", "자재나 메거진이 남아 있으면 잡파일을 바꿀수 없습니다.");
                Log.ShowMessage("Error", "Cannot change the working files.");
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
            //FrmMain.FrmDeviceSet.
            OM.LoadJobFile(sName);                                           pbStatus.Minimum  = 0;
            SEQ.Visn.SendJobChange(sName);                                   pbStatus.Minimum  = 30;
            PM.Load(sName);                                                  pbStatus.Value    = 70;
            CConfig Config = new CConfig();
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + sName + "\\TrayMask.ini";
            Config.Load(sDevOptnPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            DM.ARAY[ri.MASK].Load(Config, true);
            
            pbStatus.Value    = 100;

            CDelayTimer TimeOut = new CDelayTimer();
            TimeOut.Clear();
            while(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.JobChange  )){
                Thread.Sleep(1);
                if(TimeOut.OnDelay(5000)) { 
                    SM.ER_SetErr(ei.VSN_ComErr,"JobFile Change TimeOut");
                    break;
                }
            }

            lbSelDevice.Text = "";
            tbFromName.Text = "";
            lbCrntDevice.Text = sName;

            PM.UpdatePstn(true);

            
            //OM.TrayMask.SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY);
            //OM.TrayMask.SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY);
            //OM.SaveTrayMask();


            DM.ARAY[ri.SPLR].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.IDXR].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.IDXF].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.PCKR].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.TRYF].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.TRYG].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.OUTZ].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.STCK].SetMaxColRow(1                        , OM.DevInfo.iTRAY_StackingCnt);
            DM.ARAY[ri.BARZ].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.INSP].SetMaxColRow(1                        , OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.PSTC].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.MASK].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
//            DM.ARAY[ri.BPCK].SetMaxColRow(1                        , 1                           );

            DM.ARAY[ri.TRYF].SetStat(cs.Empty);
            DM.ARAY[ri.TRYG].SetStat(cs.Good );
            DM.ARAY[ri.STCK].SetStat(cs.Empty);
            DM.ARAY[ri.INSP].SetStat(cs.Good );


            DM.ARAY[ri.IDXR].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.IDXF].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.TRYF].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.TRYG].SetMask(DM.ARAY[ri.MASK]);

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
            Log.Trace("JobFile", (tbToName.Text + " is Maked"));
        }

        private void btCopy_Click(object sender, EventArgs e)
        {
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
                if (Log.ShowMessageModal("Confirm", "Do you want copy " + "to " + tbToName.Text+ " From " + tbFromName.Text+ "?") != DialogResult.Yes) return;
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
            Log.Trace("JobFile", (tbToName.Text + " is Maked by Copy"));
        }

        private void btRename_Click(object sender, EventArgs e)
        {
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
            Log.Trace("JobFile", (tbToName.Text + " is Maked by Rename"));
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
             
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
