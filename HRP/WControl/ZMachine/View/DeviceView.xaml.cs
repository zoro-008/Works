using COMMON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Threading;

namespace Machine.View
{
    /// <summary>
    /// DeviceView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeviceView : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        public DeviceView()
        {
            InitializeComponent();

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);

            //디렉토리 검사후 리스트뷰에 추가
            AddDirectoryInfo();

            //처음에 한번만 연결해주고 List 대신에 ObservableCollection 사용함
            listView.ItemsSource = null;
            listView.ItemsSource = Listdata.GetInstance();

            //리프레시 기타 방법
            //listView.ItemsSource = null;
            //listView.ItemsSource = Listdata.GetInstance();

            //이것도 리플레시 해주는데 ObservableCollection 이걸로 사용함
            //ICollectionView view;
            //view = System.Windows.Data.CollectionViewSource.GetDefaultView(Listdata.GetInstance());

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //tbDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //throw new NotImplementedException();
        }
        
        public void AddDirectoryInfo()
        {
            Listdata.GetInstance().Clear();
            string sPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\JobFile\\";
            DirectoryInfo Info = new DirectoryInfo(sPath);

            if (Info.Exists)
            {
                DirectoryInfo[] Dir = Info.GetDirectories();

                int iNo = 1;
                foreach (DirectoryInfo info in Dir)
                {
                    Listdata.GetInstance().Add(new Listdata() { No = iNo, Name = info.Name, Date = info.LastWriteTime });
                    //Listdata.GetInstance().Add(new Listdata() { No = iNo, Name = "asdf", Date = "fdgg" });
                    iNo++;
                }
            }
           
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var item = sender as ListView;
            //Listdata listdata = (Listdata)item.SelectedItem[0];
        }

        private void btCreate_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace("DeviceView " + sText + " Button Clicked", ForContext.Frm);

            string sPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\JobFile\\" + tbToName.Text; //Application.StartupPath + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sPath);
            if (di.Exists == false)
            {
                //if (Log.ShowMessageModal("Confirm", "JOB FILE을 생성하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want to Create JOB FILE?") != System.Windows.Forms.DialogResult.Yes) return;
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

            AddDirectoryInfo();
        }

        private void btCopy_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace("DeviceView " + sText + " Button Clicked", ForContext.Frm);

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
            sFromPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\JobFile\\" + tbFromName.Text;
            sToPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sToPath);

            if (di.Exists == false)
            {
                //if (Log.ShowMessageModal("Confirm", tbToName.Text + " 을(를) " + tbFromName.Text + " 로 부터 복사 생성 하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want copy " + "to " + tbToName.Text + " From " + tbFromName.Text + "?") != System.Windows.Forms.DialogResult.Yes) return;
                CopyFolder(sFromPath, sToPath);
            }

            if (di.Exists == true)
            {
                //Log.ShowMessage("Warning", "같은 이름의 JOB FILE이 존재합니다.");
                Log.ShowMessage("Warning", "File with the same name exists.");
                return;
            }
            
            SaveDeviceLog(2, tbFromName.Text, tbToName.Text);
            AddDirectoryInfo();
        }

        private void btRename_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace("DeviceView " + sText + " Button Clicked", ForContext.Frm);

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
            sFromPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\JobFile\\" + tbFromName.Text;
            sToPath   = System.AppDomain.CurrentDomain.BaseDirectory + "\\JobFile\\" + tbToName.Text;
            DirectoryInfo di = new DirectoryInfo(sToPath);

            if (di.Exists == false)
            {
                //if (Log.ShowMessageModal("Confirm", "잡파일 이름을 변경하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want to change the JOB FILE Name?") != System.Windows.Forms.DialogResult.Yes) return;
                RenameFolder(sFromPath, sToPath);
            }

            if (di.Exists == true)
            {
                //Log.ShowMessage("Warning", "같은 이름의 JOB FILE이 존재합니다.");
                Log.ShowMessage("Warning", "File with the same name exists.");
                return;
            }

            //OM.SetCrntDev(tbToName.Text);
            
            //Trace Log.
            //Log.Trace("JobFile", (tbToName.Text + " is Maked by Rename"));
            SaveDeviceLog(3, tbFromName.Text, tbToName.Text);

            AddDirectoryInfo();
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace("DeviceView " + sText + " Button Clicked", ForContext.Frm);

            //Check None Name.
            if (tbFromName.Text == "")
            {   //아무것도 선택되지 않은 경우.
                //Log.ShowMessage("Error", "선택된 JOB FILE 이름이 없습니다." );
                Log.ShowMessage("Error", "The selected JOB FILE name is missing.");
                return;
            }

            string sPath;

            sPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\JobFile\\" + tbFromName.Text;

            DirectoryInfo di = new DirectoryInfo(sPath);

            if (di.Exists == true)
            {
                //if (Log.ShowMessageModal("Confirm", "선택된 JOB FILE을 삭제하시겠습니까?") != DialogResult.Yes) return;
                if (Log.ShowMessageModal("Confirm", "Do you want to delete the selected JOB FILE?") != System.Windows.Forms.DialogResult.Yes) return;
                DeleteFolder(sPath);
            }

            if (tbFromName.Text == OM.GetCrntDev())
            {
                //Log.ShowMessage("Warning", "현재 사용중인 JOB FILE은 삭제할 수 없습니다." );
                Log.ShowMessage("Warning", "JOB FILE currently in use cannot be deleted.");
                return;
            }

            //Trace Log.
            //Log.Trace("JobFile", (tbFromName.Text + " is Deleted"));
            SaveDeviceLog(4, tbFromName.Text);

            //pbStatus.Minimum = 0;
            //UserFile.GridSearchDir(ExtractFilePath(Application->ExeName) + "JobFile", sgDevice, 1, true);     // 디렉토리 읽어와서 날짜와 알파벳 순으로 정렬
            //sgDevice->Row = 0;

            AddDirectoryInfo();
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
                string name = System.IO.Path.GetFileName(file);
                string dest = System.IO.Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            foreach (string folder in folders)
            {
                string name = tbToName.Text;
                string dest = System.IO.Path.Combine(destFolder, name);
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

        private void SaveDeviceLog(int _Idx, string _sBf , string _sAt = "")
        {
            string sTemp = "";
            switch (_Idx)
            {
                case 1:
                    sTemp = "# NEW DEVICE NAME     : " + _sBf ;
                    Log.Trace(sTemp, ForContext.Dev);
                    break;//New.

                case 2:
                    sTemp = "# CURRENT DEVICE NAME : " + _sBf ;
                    Log.Trace(sTemp, ForContext.Dev);
                    sTemp = "# COPYED  DEVICE NAME : " + _sAt ;
                    Log.Trace(sTemp, ForContext.Dev);
                    break;//Copy.

                case 3:
                    sTemp = "# CURRENT DEVICE NAME : " + _sBf ;
                    Log.Trace(sTemp, ForContext.Dev);
                    sTemp = "# RENAME  DEVICE NAME : " + _sAt ;
                    Log.Trace(sTemp, ForContext.Dev);
                    break;//Rename.

                case 4:
                    sTemp = "# DELETE DEVICE NAME  : " + _sBf ;
                    Log.Trace(sTemp, ForContext.Dev);
                    break;//Delete

                case 0:
                    sTemp = "# CHANGED DEVICE NAME : " + _sBf ;
                    Log.Trace(sTemp, ForContext.Dev);
                    sTemp = "# CURRENT DEVICE NAME : " + _sAt ;
                    Log.Trace(sTemp, ForContext.Dev);
                    break;//Changed
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }

        private void btDownload_Click(object sender, RoutedEventArgs e)
        {
            var bt = sender as Button;
            bt.IsEnabled = false;
            pbDownload.Value=0;

            bt.IsEnabled = true ;
        }
    }

    public class Listdata
    {
        public int       No   { get; set; }
        public string    Name { get; set; }
        public DateTime  Date { get; set; }
        //public string    Date { get; set; }
 
        //private static List<Listdata> instance;
        private static ObservableCollection<Listdata> instance;
 
        //public static List<Listdata> GetInstance()
        public static ObservableCollection<Listdata> GetInstance()
        {
            //if (instance == null)
            //    instance = new List<Listdata>();
            if (instance == null)
                instance = new ObservableCollection<Listdata>();
 
            return instance;
        }
    }
}
