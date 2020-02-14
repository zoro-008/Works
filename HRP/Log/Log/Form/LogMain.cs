using COMMON;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Ionic.Zip;

namespace Log
{
    public partial class LogMain : Form
    {
        public LogOption  FrmLogOption;
        public LogVersion FrmLogVersion;
        public LogFind    FrmLogFind;

        const int m_iUpdateCount = 5;
        const int m_iUpdateTerm  = 500;
        const int m_iMaxArray    = 16;
        const int m_iPauseTime   = 60000;
        int m_iMaxListCnt  ;
        //const string m_sLs

        //ListView
        public String[] sListViewColText = { "TIME", "#", "TRACE INFO", "LINE", "FUNC NAME", "SOURCE PATH" };

        //Array List
        public MsgList[] lst;
        private ArrayList _pIgnrList = new ArrayList();

        private object lockObject = new object();

        private int m_iAddCount;            // ListView 에 Log를 추가한 횟수를 저장
        private double  m_dLastLogAddTick;  // 마지막으로 Log를 추가한 Tick를 저장

        int iDayCount;
        bool bPause;

        FileStream[] fs;
        StreamWriter[] sw;
        CTimer Timer;

        private int iTabIdx;

        #region WndProc Members
        public const Int32 WM_COPYDATA = 0x004A;
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public UInt32 cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        #endregion

        public LogMain()
        {
            InitializeComponent();

            this.TopMost = true;

            m_iMaxListCnt = Eqp.m_iMaxListCnt;
            //Fre Init
            Timer = new CTimer();
            //
                                    
            //notifyIcon
            ContextMenu contextMenu1 = new ContextMenu();
            //contextMenu1.MenuItems.Add(new MenuItem("OPEN"));
            //contextMenu1.MenuItems.Add(new MenuItem("EXIT", new EventHandler((s, ex) => this.Close())));
            contextMenu1.MenuItems.Add(new MenuItem("EXIT", new EventHandler(FormExit)));
            notifyIcon1.ContextMenu = contextMenu1;
            notifyIcon1.Visible = true;

            FrmLogOption = new LogOption(this);
            FrmLogVersion = new LogVersion();
            FrmLogFind = new LogFind(this);

            //m_iMaxArray = OM.LogInfo.iTagCnt;

            fs = new FileStream[m_iMaxArray];
            sw = new StreamWriter[m_iMaxArray];

            //OnMakeNewLog();

            _pIgnrList.Clear();
            lst = new MsgList[m_iMaxArray];
            for(int i = 0; i < m_iMaxArray; i++)
            {
                lst[i] = new MsgList();
                lst[i].Ary.Clear();
            }

            iDayCount = DateTime.Now.Day;

            tabControl1.TabPages.Clear();
            TabPage tp = new TabPage("DEFAULT"); tabControl1.TabPages.Add(tp); lst[0].Lsv.Parent = tp; lst[0].Lsv.Dock = DockStyle.Fill; 
            ListViewDispInit(OM.LogInfo.iTagCnt);

        }

        public int GetTabIdx()
        {
            int iIdx = tabControl1.SelectedIndex;
            if (iIdx >= 0 && iIdx < m_iMaxArray) return iIdx;
            return 0;
        }

        public void ListViewPageText(int _iCnt = 0)
        {
            for (int i = 1; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.TabPages[i].Text = OM.GetTag(i);
            }
        }
        public void ListViewDispInit(int _iCnt = 0)
        {
            int iNow = tabControl1.TabCount;

            if (_iCnt == 0)
            {
                for (int i = 1; i < iNow; i++)
                {
                    tabControl1.TabPages.RemoveAt(1);
                }
                return;
            }
            if (_iCnt == iNow - 1) return;
            if (_iCnt > iNow - 1)
            {
               for (int i = iNow; i < _iCnt + 1; i++)
               {
                   lst[i].Lsv.BeginUpdate();
                   TabPage tp = new TabPage(OM.GetTag(i));
                   //tp.Text = OM.GetTag(i); 
                   tabControl1.TabPages.Add(tp);
                   lst[i].Lsv.Parent = tp;
                   lst[i].Lsv.Dock = DockStyle.Fill; 
                   lst[i].Lsv.EndUpdate();
               }
            }
            else
            {
                for (int i = iNow - 1; i > _iCnt; i--)
                {
                    tabControl1.TabPages.RemoveAt(i);
                }
            }

        }
        private void oPTIONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogOption.Close();

            FrmLogOption = new LogOption(this);
            FrmLogOption.Show();
        }

        private void OnAddNewLog(String _sLog)
        {
            bool bSameMsg = false;
            string sLog1 = _sLog;
            string sLog2 = "";
            //string sTemp = "";
            //Add Time
            string sTime = DateTime.Now.ToString("MM-dd hh:mm:ss:fff,");
            sLog2 = sTime + sLog1;

            //if (sLog2.Contains("<EXIT>"))
            //{
            //    FormExit();
            //}

            //중복 삭제.
            int iIgnrSameMsgLine = OM.LogInfo.iIgnrSameMsgLine;

            while (_pIgnrList.Count > iIgnrSameMsgLine) { _pIgnrList.RemoveAt(0); }
            int iListCount = _pIgnrList.Count;

            if (iIgnrSameMsgLine > 0)
            {
                for (int j = 0; j < iListCount; j++)
                {
                    bSameMsg |= (string)_pIgnrList[j] == sLog1;
                }
            }
            if (bSameMsg) return;
            _pIgnrList.Add(sLog1);
            //

            lock (lockObject)
            {
                String[] sSplit = sLog2.Split(',');
                int iNo = int.TryParse(sSplit[1], out iNo) ? iNo : 0;
                if(iNo >= 0 && iNo < m_iMaxArray)
                { 
                    lst[iNo].Ary.Add(sLog2);
                    if (iNo != 0 && OM.GetWithAll(iNo))
                    {
                        lst[0].Ary.Add(sLog2);
                        if (sw[0] != null)
                        {
                            sw[0].WriteLine(sLog2);
                            sw[0].Flush(); //이거 음...
                        }
                        while (lst[0].Ary.Count > m_iMaxListCnt) {
                            lst[0].Ary.RemoveAt(0);
                        }
                    }
                    if (sw[iNo] != null)
                    {
                        sw[iNo].WriteLine(sLog2);
                        sw[iNo].Flush(); //이거 음...
                    }
                    while (lst[iNo].Ary.Count > m_iMaxListCnt) {
                        lst[iNo].Ary.RemoveAt(0);
                    }

                    if (sSplit[2].Contains("<EXIT>")) { FormExit(); }
                }
                //m_iAddCount++;
            }

            //// Max count 초과 Log 삭제
            //for (int i = 0; i < m_iMaxArray; i++)
            //{
            //    while (lst[i].Ary.Count > m_iMaxListCnt) {
            //        //lst[i].Ary.RemoveAt(lst[i].Ary.Count-1);
            //        lst[i].Ary.RemoveAt(0);
            //    }
            //}

            ////sTemp = (string)lst[0].Ary[i];
            //string[] sText = sLog2.Split(',');
            //int iText = int.TryParse(sText[1], out iText) ? iText : 0;

            //if (iText == 0 || OM.GetWithAll(iText))
            //{
            //    if (sw[0] != null)
            //    {
            //        sw[0].WriteLine(sLog2);
            //        sw[0].Flush(); //이거 음...
            //    }
            //}
                

            //for (int j = 1; j < m_iMaxArray; j++)
            //{
            //    if (j == iText)
            //    {
            //        if (sw[j] != null)
            //        {
            //            sw[j].WriteLine(sLog2);
            //            sw[j].Flush();
            //        }
            //    }
            //}
                

            // 파일 저장
            //sLog3 = sLog2 + "\r\n";
            //sw[0].WriteLine(sLog2);
            //for (int i = 0; i < m_iMaxArray; i++)
                //sw[i].WriteLine(sLog2);
            //sw.Flush();
            //sw.Close();
        }

        private bool OnMakeNewLog()
        {
            string sTime = DateTime.Now.ToString("yyyyMMdd");
            string sDir  = OM.LogInfo.sLogPath + @"\" + sTime + @"\";
            string[] sFile = new string[m_iMaxArray];

            if (!CIniFile.MakeFilePathFolder(sDir)) return false;
            
            sFile[0] = sDir + sTime + ".log";
            for (int i = 1; i < m_iMaxArray; i++)
                sFile[i] = sDir + sTime + "_" + OM.GetTag(i) + ".log";
            //if (!File.Exists(sFile)) File.Create(sFile);

            for (int i = 0; i < m_iMaxArray; i++)
            {
                if (sw[i] != null) sw[i].Close();
                if (fs[i] != null) fs[i].Close();
            }

            fs = new FileStream[m_iMaxArray];
            sw = new StreamWriter[m_iMaxArray];

            for (int i = 0; i < m_iMaxArray; i++)
            {
                if (i < OM.LogInfo.iTagCnt+1)
                {
                    fs[i] = new FileStream(sFile[i], FileMode.Append, FileAccess.Write);
                    sw[i] = new StreamWriter(fs[i]);
                }
                else
                {
                    fs[i] = null;
                    sw[i] = null;

                }
            }

            //BackUp
            //string sbuName = "BackUp";//Path.GetFileName(OM.LogInfo.sLogBuPath);
            
            string sZip = DateTime.Now.ToString("yyyyMMdd") + "_BackUp.zip";

            double d1, d2, d3;
            d1 = CTimer.GetTime();
            CreateSample(sDir + sZip, "", OM.LogInfo.sLogBuPath);
            d2 = CTimer.GetTime();
            d3 = d2 - d1;
            Debug.WriteLine(d3);

            //기존에 있던것들 지우기. 디렉토리 삭제
            DirectoryInfo di = new DirectoryInfo(OM.LogInfo.sLogPath);
             if (!di.Exists) di.Create();

             foreach (DirectoryInfo dir in di.GetDirectories())
             {
                if (dir.CreationTime <= DateTime.Now.AddMonths(-OM.LogInfo.iSaveMaxMonths))
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch(Exception e)
                    {
                        //Debug.WriteLine(e.Message);
                    }
                }
             }
             //foreach (FileInfo file in di.GetFiles()) //파일 삭제
             //{
             //    file.Delete();
             //}

            return true;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                OnAddNewLog(cds.lpData);
            }
            base.WndProc(ref m);
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            String sTemp;
            if(iDayCount != DateTime.Now.Day)
            {
                OnMakeNewLog();
                iDayCount = DateTime.Now.Day;
            }

            for(int i=0;i< m_iMaxArray;i++) {
                //ListViewItem item = lst[i].Lsv.Items[lst[i].Lsv.Items.Count - 1];
                //bool isNull = item.SubItems[0].Text == "" ;
                //if(isNull) lst[i].Lsv.Items[lst[i].Lsv.Items.Count - 1].EnsureVisible();
                lst[i].Lsv.Refresh();
            }
            //double dTimeus = CTimer.GetTime();

            //if (bPause)
            //{
            //    cLEARToolStripMenuItem.Text = "RUN";
            //    for(int i=0;i< m_iMaxArray;i++) lst[i].Lsv.VirtualMode = false;
            //}
            //else {
            //    cLEARToolStripMenuItem.Text = "PAUSE";
            //    for(int i=0;i< m_iMaxArray;i++) lst[i].Lsv.VirtualMode = true;
            //}

            //if (dTimeus - m_dLastLogAddTick > 30000) bPause = false;

            //if (!bPause && (m_iAddCount > m_iUpdateCount))// || (m_iAddCount > 0 && (dTimeus - m_dLastLogAddTick) > m_iUpdateTerm)))
            //{
            //    //for(int i=0;i< m_iMaxArray;i++) lst[i].Lsv.BeginUpdate();
            //    lock (lockObject) //EnterCriticalSection
            //    {
            //        for (int i = 0; i < lst[0].Ary.Count; i++)
            //        {
            //            sTemp = (String)lst[0].Ary[i];
            //            String[] sText = sTemp.Split(',');
            //            ListViewItem[] item = new ListViewItem[m_iMaxArray]; //클리어가 빠를까 이게 빠를까는 테스트 필요.
            //            for (int j = 0; j < m_iMaxArray; j++) item[j] = new ListViewItem(sText[0]);
            //            bool bFst = true;
            //            foreach (string s in sText)
            //            {
            //                if (!bFst)
            //                {
            //                    for (int j = 0; j < m_iMaxArray; j++) item[j].SubItems.Add(s);
            //                }
            //                bFst = false;
            //            }
            //            int iText = int.TryParse(sText[1], out iText) ? iText : 0;
            //            if (sText[2].Contains("<EXIT>")) { FormExit(); }

            //            if (iText == 0 || OM.GetWithAll(iText))
            //                 lst[0].Lsv.Items.Insert(0, item[0]);
            //            for (int j = 1; j < m_iMaxArray; j++)
            //            {
            //                if(j == iText) lst[j].Lsv.Items.Insert(0, item[j]);
            //            }
                        
            //        }
            //    }

            //    for (int i = 0; i < m_iMaxArray; i++)
            //    {
            //        if (lst[i].Lsv.Items.Count > 0) lst[i].Lsv.Items[0].EnsureVisible();
            //    }
            //    for (int i = 0; i < m_iMaxArray; i++) lst[i].Lsv.EndUpdate();

            //    // List View에서 Max count 초과 Log 삭제
            //    for (int i = 0; i < m_iMaxArray; i++)
            //    {
            //        lst[i].Lsv.BeginUpdate();
            //        while (lst[0].Lsv.Items.Count > m_iMaxListCnt)
            //        {
            //            lst[i].Lsv.Items.RemoveAt(lst[i].Lsv.Items.Count - 1);
            //        }
            //        lst[i].Lsv.EndUpdate();
            //    }

            //    // 제일 위로 Scroll 하도록 설정
            //    //lvLog->Scroll(0, -1000);

            //    m_iAddCount = 0;
            //    lst[0].Ary.Clear();

            //    m_dLastLogAddTick = CTimer.GetTime();
            //}

            timer1.Enabled = true;
        }

        private void LogMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (!bExit) e.Cancel = true; // 종료 이벤트를 취소 시킨다 this.Visible = false; // 폼을 표시하지 않는다;
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = true;
            //notifyIcon1.Visible = true;
            this.Hide();
        }


        private void LogMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                //notifyIcon1.Visible = true;
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                //notifyIcon1.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void eXITToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormExit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void LogMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //for (int i = 0; i < m_iMaxArray; i++)
            //{
                //if (sw[i] != null) sw[i].Close();
                //if (fs[i] != null) fs[i].Close();
            //}
        }

        private void FormExit()
        {
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void FormExit(object sender, EventArgs e)
        {
            FormExit();
        }

        private void ListClear()
        {
            for (int i = 0; i < m_iMaxArray; i++)
            {
                //lst[i].Lsv.BeginUpdate();
                //while (lst[i].Lsv.Items.Count > 0) { lst[i].Lsv.Items.RemoveAt(0); }
                //lst[i].Lsv.EndUpdate();
                lst[i].Ary.Clear();
            }
            while (_pIgnrList.Count > 0) { _pIgnrList.RemoveAt(0); }
        }

        private void eXITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListClear();
        }

        private void cLEARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bPause = !bPause;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int iIdx = tabControl1.SelectedIndex;
            if (iIdx >= 0 && iIdx < m_iMaxArray) lst[iIdx].SaveData();
        }

        private ListViewItem FindItem(string keyword, int startIndex)
        {
            for (int i = startIndex; i < lst[0].Lsv.Items.Count; i++)
            {
                ListViewItem item = lst[0].Lsv.Items[i];
                bool isContains = item.SubItems[2].Text.Contains(keyword);
                if (isContains) { return item; }
            }
            return null;
        }

        private void vERSIONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogVersion.Close();
            FrmLogVersion = new LogVersion();
            FrmLogVersion.Show();
        }

        private void fINDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogFind.Close();
            FrmLogFind = new LogFind(this);
            FrmLogFind.Show();
        }

        private void LogMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                FrmLogFind.Close();
                FrmLogFind = new LogFind(this);
                FrmLogFind.Show();
            }
            if (e.KeyCode == Keys.F3)
            {
                if(FrmLogFind.Visible)
                {
                    FrmLogFind.Find();
                }
            }
            if (e.KeyCode == Keys.F4)
            {
                if(FrmLogFind.Visible)
                {
                    FrmLogFind.FindNext();
                }
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                int iIdx = tabControl1.SelectedIndex;
                if (iIdx >= 0 && iIdx < m_iMaxArray) lst[iIdx].SaveData();
            }
            //if (e.Control && e.KeyCode == Keys.C)
            //{
            //    int iIdx = tabControl1.SelectedIndex;

            //    ListView.SelectedListViewItemCollection breakfast = lst[iIdx].Lsv.SelectedItems;
                
            //    string sTemp = "";
            //    if (breakfast.Count > 1000) return;
            //    foreach (ListViewItem item in breakfast)
            //    {
            //        for (int j = 0; j < item.SubItems.Count; j++) sTemp += item.SubItems[j].Text + ", ";
            //        sTemp += "\r\n";
            //        Clipboard.SetText(sTemp);
            //    }
            //}
            if (e.KeyCode == Keys.Escape)
            {
                //notifyIcon1.Visible = true;
                Hide();
            }
        }

        public void CreateSample(string outPathname, string password, string folderName)
        {

            FileStream fsOut = File.Create(outPathname);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(1); //0-9, 9 being the highest level of compression

            zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

            CompressFolder(folderName, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        // Recurses down the folder structure
        //
        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists) return;
            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }

        static bool bFst = true;
        private void LogMain_Shown(object sender, EventArgs e)
        {
            if (bFst)
            {
                //notifyIcon1.Visible = true;
                Hide();
                OnMakeNewLog();
                bFst = false;
            }

        }



        //public partial class LogMain : Form {}
    }

    class MyListViewComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public MyListViewComparer() { col = 0; order = SortOrder.Ascending; }
        public MyListViewComparer(int column, SortOrder order) { col = column; this.order = order; }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            if (order == SortOrder.Descending) returnVal *= -1;
            return returnVal;
        }
    }

    public class Eqp
    {
        public const int m_iMaxListCnt  = 2000;
    }    

    public class MsgList
    {
        public ArrayList Ary;
        public ListView Lsv;
        
        public String[] sListViewColText = { "TIME", "#", "TRACE INFO", "LINE", "FUNC NAME", "SOURCE PATH" };
        private int iSortCol = -1;
        
        //const int m_iMaxListCnt  = 2000;

        public MsgList()
        {
            Ary = new ArrayList();
            Lsv = new ListView();

            Lsv.ColumnClick += new ColumnClickEventHandler(ColumnClick);
            Lsv.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(Lsv_RetrieveVirtualItem);

            Lsv.Clear();
            Lsv.View = View.Details;
            Lsv.LabelEdit = false;
            Lsv.AllowColumnReorder = true;
            Lsv.FullRowSelect = true;
            Lsv.GridLines = true;
            //Lsv.Sorting = SortOrder.Descending;
            Lsv.Scrollable = true;
            //Need to Find
            //Lsv.MultiSelect   = false;
            Lsv.HideSelection   = false;
            Lsv.VirtualMode     = true ;
            Lsv.VirtualListSize = Eqp.m_iMaxListCnt ;

            
            

            Lsv.Columns.Add(sListViewColText[0], 130, HorizontalAlignment.Left); //Day Time
            Lsv.Columns.Add(sListViewColText[1], 30,  HorizontalAlignment.Left); //Tag
            Lsv.Columns.Add(sListViewColText[2], 400, HorizontalAlignment.Left);
            Lsv.Columns.Add(sListViewColText[3], 40,  HorizontalAlignment.Left);
            Lsv.Columns.Add(sListViewColText[4], 100, HorizontalAlignment.Left);
            Lsv.Columns.Add(sListViewColText[5], 600, HorizontalAlignment.Left);

            var PropLotInfo = Lsv.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(Lsv, true, null);

        }

        private void ColumnClick(object sender, ColumnClickEventArgs e)
        {
            return;
            //if (e.Column == 0) return;
            if (e.Column != iSortCol)
            {
                iSortCol = e.Column;
                Lsv.Sorting = SortOrder.Ascending;
                Lsv.Columns[iSortCol].Text = sListViewColText[iSortCol] + " ▲";

            }
            else
            {
                if (Lsv.Sorting == SortOrder.Ascending)
                {
                    Lsv.Sorting = SortOrder.Descending;
                    Lsv.Columns[iSortCol].Text = sListViewColText[iSortCol] + " ▼";
                }
                else
                {
                    Lsv.Sorting = SortOrder.Ascending;
                    Lsv.Columns[iSortCol].Text = sListViewColText[iSortCol] + " ▲";
                }
            }

            Lsv.Sort();
            this.Lsv.ListViewItemSorter = new MyListViewComparer(e.Column, Lsv.Sorting);
        }


        private void Lsv_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            
            int i = e.ItemIndex ;//Lsv.VirtualListSize - e.ItemIndex - 1 ;
            if (i >= Ary.Count) {// || Ary.Count == 0) {
                ListViewItem lTemp = new ListViewItem(""); 	// create a listviewitem object
                lTemp.SubItems.Add("");
                lTemp.SubItems.Add("");
                lTemp.SubItems.Add("");
                lTemp.SubItems.Add("");
                lTemp.SubItems.Add("");
                e.Item = lTemp; 		// assign item to event argument's item-property
                return;
            }

            ListViewItem lvi = new ListViewItem(); 	// create a listviewitem object
            string sTemp = "";
            sTemp = (String)Ary[Ary.Count - 1 - i];
            String[] sText = sTemp.Split(',');
            lvi = new ListViewItem(sText[0]);

            bool bFst = true;
            foreach (string s in sText)
            {
                if (!bFst)
                {
                    lvi.SubItems.Add(s);
                }
                bFst = false;
            }
            e.Item = lvi; 		// assign item to event argument's item-property
        }

        public void SaveData()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "csv File|*.csv";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1: for (int i = 0; i < Lsv.Items.Count; i++)
                    {
                        string sTmp = "";
                        for (int j = 0; j < Lsv.Items[i].SubItems.Count; j++) sTmp += Lsv.Items[i].SubItems[j].Text + ", ";
                            sTmp += "\n";
                        Byte[] Bytes = Encoding.UTF8.GetBytes(sTmp);
                        fs.Write(Bytes, 0, Bytes.Length);
                    }
                    break;
                }
                fs.Close();
            }
        }


    }
}
