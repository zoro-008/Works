using COMMON;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormSPC_Sub : Form
    {
        public static CArray SPCARAY = new CArray();

        

        public FormSPC_Sub()
        {
            

            InitializeComponent();

            // 아이콘 리스트 만들기
            ImageList myimageList = new ImageList();
            //myimageList.Images.Add(Properties.Resources.Folder); 
            tvDirectory.ImageList = myimageList;

            //SPC.LOT.LoadDataMap();

            MakeDoubleBuffered(pnDataMap, true);

            SPCARAY.SetParent(pnDataMap);
            //SPCARAY.Name = DM.ARAY[ri.SPC].Name;

            //Array 색깔
            SPCARAY.SetDisp(cs.Unknown, "UnKnown", Color.Aqua);
            SPCARAY.SetVisible(cs.Unknown, true);

            InitlvFile();
        }

        //SpcLog 폴더에서 DataMap 폴더까지의 경로
        string sRootPath = System.IO.Directory.GetParent(SPC.LOT.Folder).Parent.FullName.ToString() + "\\DataMap";
        //TreeView 초기화
        public void InitDirTreeView(string _sDay, string _sLotNo)
        {
            try
            {
                //계속 갱신해야되서 처음에 Clear
                tvDirectory.Nodes.Clear();
                
                // TreeView에 사용할 ImageList 정의
                //Tree 앞에 아래 정의한 이미지 사용
                //ImageList imgList = new ImageList();
                //imgList.Images.Add(Properties.Resources.Folder);
                //tvDirectory.ImageList = imgList;

                //Form SPC에서 건네받은 랏 넘버로 폴더 검색할때 쓰는 변수들
                string[] sDataMapFolder = Directory.GetDirectories(sRootPath);

                //폴더 검색 및 TreeView에 폴더명 표기
                foreach (string drive in sDataMapFolder)
                {
                    DirectoryInfo dir = new DirectoryInfo(drive);
                    TreeNode root = new TreeNode(drive.Substring(drive.LastIndexOf('\\') + 1),0,0);
                    //FormSPC에서 해당 랏 정보 클릭했을 때 랏넘버 가져와서 트리뷰에 해당 랏 넘버 폴더에
                    //포커스 위치하도록 할때 트리 노드 네임이 할당 돼야 검색이 되서 여기서 넣음
                    root.Name = drive.Substring(drive.LastIndexOf('\\') + 1);
                    tvDirectory.Nodes.Add(root);
                    //랏넘버 폴더 아래 하위 폴더들 추가
                    GetDirectoryNodes(root, dir, true);
                }

                //트리뷰 포커스
                TreeNode[] NodeDay = tvDirectory.Nodes.Find(_sDay  .ToString(), true);
                TreeNode[] NodeLot = NodeDay[0] .Nodes.Find(_sLotNo.ToString(), true);
                TreeNode[] NodeMgz = NodeLot[0] .Nodes.Find(1      .ToString(), true);
                tvDirectory.ExpandAll();
                tvDirectory.SelectedNode = NodeMgz[0];
                tvDirectory.Select();
                //tvDirectory.SelectedNode.EnsureVisible();
                //tvDirectory.Focus();
                
            }
            catch (Exception _e)
            {
                Log.ShowMessage("Error", _e.ToString());
            }
            
        }

        public void UpdateResultPanel(string _sDate , string _sLot)
        {
            lbLotNo.Text = _sLot ;
            //일단 초기화.
            lbCnt0.Text = 0.ToString();
            lbCnt1.Text = 0.ToString();
            lbCnt2.Text = 0.ToString();
            lbCnt3.Text = 0.ToString();
            lbCnt4.Text = 0.ToString();
            lbCnt5.Text = 0.ToString();
            lbCnt6.Text = 0.ToString();
            lbCnt7.Text = 0.ToString();
            lbCnt8.Text = 0.ToString();
            lbCnt9.Text = 0.ToString();
            lbCntA.Text = 0.ToString();
            lbCntB.Text = 0.ToString();
            lbCntC.Text = 0.ToString();
            lbCntD.Text = 0.ToString();
            lbCntE.Text = 0.ToString();
            lbCntF.Text = 0.ToString();
            lbCntG.Text = 0.ToString();
            lbCntH.Text = 0.ToString();
            lbCntI.Text = 0.ToString();
            lbCntJ.Text = 0.ToString();
            lbCntK.Text = 0.ToString();
            lbCntL.Text = 0.ToString();
            lbCntGood .Text = 0.ToString();
            lbCntTotal.Text = 0.ToString();


            int[] Rslts = new int [(int)cs.MAX_CHIP_STAT];
            if(!DateTime.TryParseExact(_sDate , "yyyyMMdd", new CultureInfo("ko-KR") , DateTimeStyles.None , out DateTime Date))
            {
                return ;
            }

            SPC.MAP.LoadDataCnt(Date.ToOADate() , _sLot , ref Rslts );
            
            int iSum = 0 ;
            //lbCnt0.Text     = Rslts[(int)cs.Rslt0].ToString(); iSum += Rslts[(int)cs.Rslt0] ;
            //lbCnt1.Text     = Rslts[(int)cs.Rslt1].ToString(); iSum += Rslts[(int)cs.Rslt1] ;
            //lbCnt2.Text     = Rslts[(int)cs.Rslt2].ToString(); iSum += Rslts[(int)cs.Rslt2] ;
            //lbCnt3.Text     = Rslts[(int)cs.Rslt3].ToString(); iSum += Rslts[(int)cs.Rslt3] ;
            //lbCnt4.Text     = Rslts[(int)cs.Rslt4].ToString(); iSum += Rslts[(int)cs.Rslt4] ;
            //lbCnt5.Text     = Rslts[(int)cs.Rslt5].ToString(); iSum += Rslts[(int)cs.Rslt5] ;
            //lbCnt6.Text     = Rslts[(int)cs.Rslt6].ToString(); iSum += Rslts[(int)cs.Rslt6] ;
            //lbCnt7.Text     = Rslts[(int)cs.Rslt7].ToString(); iSum += Rslts[(int)cs.Rslt7] ;
            //lbCnt8.Text     = Rslts[(int)cs.Rslt8].ToString(); iSum += Rslts[(int)cs.Rslt8] ;
            //lbCnt9.Text     = Rslts[(int)cs.Rslt9].ToString(); iSum += Rslts[(int)cs.Rslt9] ;
            //lbCntA.Text     = Rslts[(int)cs.RsltA].ToString(); iSum += Rslts[(int)cs.RsltA] ;
            //lbCntB.Text     = Rslts[(int)cs.RsltB].ToString(); iSum += Rslts[(int)cs.RsltB] ;
            //lbCntC.Text     = Rslts[(int)cs.RsltC].ToString(); iSum += Rslts[(int)cs.RsltC] ;
            //lbCntD.Text     = Rslts[(int)cs.RsltD].ToString(); iSum += Rslts[(int)cs.RsltD] ;
            //lbCntE.Text     = Rslts[(int)cs.RsltE].ToString(); iSum += Rslts[(int)cs.RsltE] ;
            //lbCntF.Text     = Rslts[(int)cs.RsltF].ToString(); iSum += Rslts[(int)cs.RsltF] ;
            //lbCntG.Text     = Rslts[(int)cs.RsltG].ToString(); iSum += Rslts[(int)cs.RsltG] ;
            //lbCntH.Text     = Rslts[(int)cs.RsltH].ToString(); iSum += Rslts[(int)cs.RsltH] ;
            //lbCntI.Text     = Rslts[(int)cs.RsltI].ToString(); iSum += Rslts[(int)cs.RsltI] ;
            //lbCntJ.Text     = Rslts[(int)cs.RsltJ].ToString(); iSum += Rslts[(int)cs.RsltJ] ;
            //lbCntK.Text     = Rslts[(int)cs.RsltK].ToString(); iSum += Rslts[(int)cs.RsltK] ;
            //lbCntL.Text     = Rslts[(int)cs.RsltL].ToString(); iSum += Rslts[(int)cs.RsltL] ;

            //string sTemp = Rslts[(int)cs.Good ].ToString();
            //
            //lbCntGood .Text = Rslts[(int)cs.Good ].ToString(); iSum += Rslts[(int)cs.Good ] ;  
           
            lbCntTotal.Text = iSum.ToString();
        }

        //tvDirectory 트리뷰 하위 디렉토리 추가
        public void GetDirectoryNodes(TreeNode _root, DirectoryInfo _dirs, bool _isLoop)
        {
            try
            {
                DirectoryInfo[] DIRS = _dirs.GetDirectories();
                foreach (DirectoryInfo dir in DIRS)
                {
                    TreeNode child = new TreeNode(dir.Name, 0, 0);
                    _root.Nodes.Add(child);
                    //FormSPC에서 해당 랏 정보 클릭했을 때 랏넘버 가져와서 트리뷰에 해당 랏 넘버 폴더에
                    //포커스 위치하도록 할때 트리 노드 네임이 할당 돼야 검색이 되서 여기서 넣음
                    child.Name = child.Text;
                    if (_isLoop) GetDirectoryNodes(child, dir, false);
                }
            }
            catch (Exception _e)
            {
                Log.ShowMessage("Error", _e.ToString());
            }
        }

       

        //트리뷰 옆에 리스트뷰 초기화
        public void InitlvFile() //오퍼레이션 창용.
        {
            lvFile.Clear();
            lvFile.View = View.Details;
            lvFile.LabelEdit = true;
            lvFile.AllowColumnReorder = true;
            lvFile.FullRowSelect = true;
            lvFile.GridLines = true;
            //lvFile.Sorting = SortOrder.Descending;
            lvFile.Scrollable = true;
        
            lvFile.Columns.Add("Name"         ,  lvFile.Width / 2                   , HorizontalAlignment.Left);   
            lvFile.Columns.Add("Modified date", (int)((lvFile.Width / 2 - 25) * 0.8), HorizontalAlignment.Left);   
            lvFile.Columns.Add("Size"         , (int)((lvFile.Width / 2 - 25) * 0.2), HorizontalAlignment.Left);   

            var PropDayInfo = lvFile.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropDayInfo.SetValue(lvFile, true, null);
        
            if (lvFile == null) return;
        }

        //우측에 하위 폴더&파일들을 리스트 형태로 보여주기
        private void ViewDirectoryList(string path)
        {
            string curPath = path;
        
            try
            {
                lvFile.Items.Clear();    // 전에 있던 목록들을 지우고 새로 나열함.
        
                string[] directories = Directory.GetDirectories(curPath);
        
                string[] files = Directory.GetFiles(curPath);
        
                foreach (string file in files)    // 파일 나열
                {
                    FileInfo info = new FileInfo(file);
                    ListViewItem item = new ListViewItem(new string[]
                    {
                info.Name, info.LastWriteTime.ToString(), ((info.Length/1000)+1).ToString()+"KB"    // 파일의 상세 내용 표시.
                    });
                    
                    lvFile.Items.Add(item);
                }
                
                //lvFile.Sorting = SortOrder.Ascending;
                //lvFile.ListViewItemSorter = new MyListViewComparer(0, SortOrder.Ascending);
                //lvFile.Sort();

            }
            catch (Exception ex)
            {
                Console.WriteLine("ViewDirectoryList : " + ex.Message);
            }
        }

        string sSelectedNode = "";
        string sSelectedDate = "";
        string sSelectedLot  = "";
        string sSelectedMgz  = "";
        private void tvDirectory_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.FullPath == null)
            {
                Console.WriteLine("empth node.FullPath");
                return;
            }
            //string path = "";
            string[] sDataMapFolder = Directory.GetDirectories(sRootPath);
            //폴더 검색 및 TreeView에 폴더명 표기
            //foreach (string drive in sDataMapFolder)
            //{
            //    //랏넘버 폴더 이름으로 가장 상위 Tree 이름 입력
            //    DirectoryInfo dir = new DirectoryInfo(drive);
            //}

            sSelectedNode = sRootPath + "\\" + e.Node.FullPath;

            string [] NodeInfos = e.Node.FullPath.Split('\\');
            if(NodeInfos.Length == 1)
            {
                sSelectedDate = NodeInfos[0] ;
                sSelectedLot  = "" ;
                sSelectedMgz  = "" ;
            }
            else if(NodeInfos.Length == 2)
            {
                sSelectedDate = NodeInfos[0] ;
                sSelectedLot  = NodeInfos[1] ;
                sSelectedMgz  = "" ;
            }
            else if(NodeInfos.Length == 3)
            {
                sSelectedDate = NodeInfos[0] ;
                sSelectedLot  = NodeInfos[1] ;
                sSelectedMgz  = NodeInfos[2] ;
            }

            if(sSelectedDate != "" && sSelectedLot != "")
            {
                UpdateResultPanel(sSelectedDate , sSelectedLot);
            }
            

            //sSelectedLot  = sRootPath + "\\" + e.Node.Parent.FullPath ;
            string[] directories = Directory.GetDirectories(sSelectedNode);
            string[] files = Directory.GetFiles(sSelectedNode, "*.ini");
            
            ViewDirectoryList(sSelectedNode);
        }

        private void FormSPC_Sub_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
            if (this.Visible)
            {
                tvDirectory.Refresh();

                //SPCARAY.SetDisp(cs.Good , "Good"               , Color.Lime                            ); SPCARAY.SetVisible(cs.Good , false);
                //SPCARAY.SetDisp(cs.Rslt0, OM.CmnOptn.sRsltName0, Color.FromArgb(OM.CmnOptn.iRsltColor0)); SPCARAY.SetVisible(cs.Rslt0, false);
                //SPCARAY.SetDisp(cs.Rslt1, OM.CmnOptn.sRsltName1, Color.FromArgb(OM.CmnOptn.iRsltColor1)); SPCARAY.SetVisible(cs.Rslt1, false);
                //SPCARAY.SetDisp(cs.Rslt2, OM.CmnOptn.sRsltName2, Color.FromArgb(OM.CmnOptn.iRsltColor2)); SPCARAY.SetVisible(cs.Rslt2, false);
                //SPCARAY.SetDisp(cs.Rslt3, OM.CmnOptn.sRsltName3, Color.FromArgb(OM.CmnOptn.iRsltColor3)); SPCARAY.SetVisible(cs.Rslt3, false);
                //SPCARAY.SetDisp(cs.Rslt4, OM.CmnOptn.sRsltName4, Color.FromArgb(OM.CmnOptn.iRsltColor4)); SPCARAY.SetVisible(cs.Rslt4, false);
                //SPCARAY.SetDisp(cs.Rslt5, OM.CmnOptn.sRsltName5, Color.FromArgb(OM.CmnOptn.iRsltColor5)); SPCARAY.SetVisible(cs.Rslt5, false);
                //SPCARAY.SetDisp(cs.Rslt6, OM.CmnOptn.sRsltName6, Color.FromArgb(OM.CmnOptn.iRsltColor6)); SPCARAY.SetVisible(cs.Rslt6, false);
                //SPCARAY.SetDisp(cs.Rslt7, OM.CmnOptn.sRsltName7, Color.FromArgb(OM.CmnOptn.iRsltColor7)); SPCARAY.SetVisible(cs.Rslt7, false);
                //SPCARAY.SetDisp(cs.Rslt8, OM.CmnOptn.sRsltName8, Color.FromArgb(OM.CmnOptn.iRsltColor8)); SPCARAY.SetVisible(cs.Rslt8, false);
                //SPCARAY.SetDisp(cs.Rslt9, OM.CmnOptn.sRsltName9, Color.FromArgb(OM.CmnOptn.iRsltColor9)); SPCARAY.SetVisible(cs.Rslt9, false);
                //SPCARAY.SetDisp(cs.RsltA, OM.CmnOptn.sRsltNameA, Color.FromArgb(OM.CmnOptn.iRsltColorA)); SPCARAY.SetVisible(cs.RsltA, false);
                //SPCARAY.SetDisp(cs.RsltB, OM.CmnOptn.sRsltNameB, Color.FromArgb(OM.CmnOptn.iRsltColorB)); SPCARAY.SetVisible(cs.RsltB, false);
                //SPCARAY.SetDisp(cs.RsltC, OM.CmnOptn.sRsltNameC, Color.FromArgb(OM.CmnOptn.iRsltColorC)); SPCARAY.SetVisible(cs.RsltC, false);
                //SPCARAY.SetDisp(cs.RsltD, OM.CmnOptn.sRsltNameD, Color.FromArgb(OM.CmnOptn.iRsltColorD)); SPCARAY.SetVisible(cs.RsltD, false);
                //SPCARAY.SetDisp(cs.RsltE, OM.CmnOptn.sRsltNameE, Color.FromArgb(OM.CmnOptn.iRsltColorE)); SPCARAY.SetVisible(cs.RsltE, false);
                //SPCARAY.SetDisp(cs.RsltF, OM.CmnOptn.sRsltNameF, Color.FromArgb(OM.CmnOptn.iRsltColorF)); SPCARAY.SetVisible(cs.RsltF, false);
                //SPCARAY.SetDisp(cs.RsltG, OM.CmnOptn.sRsltNameG, Color.FromArgb(OM.CmnOptn.iRsltColorG)); SPCARAY.SetVisible(cs.RsltG, false);
                //SPCARAY.SetDisp(cs.RsltH, OM.CmnOptn.sRsltNameH, Color.FromArgb(OM.CmnOptn.iRsltColorH)); SPCARAY.SetVisible(cs.RsltH, false);
                //SPCARAY.SetDisp(cs.RsltI, OM.CmnOptn.sRsltNameI, Color.FromArgb(OM.CmnOptn.iRsltColorI)); SPCARAY.SetVisible(cs.RsltI, false);
                //SPCARAY.SetDisp(cs.RsltJ, OM.CmnOptn.sRsltNameJ, Color.FromArgb(OM.CmnOptn.iRsltColorJ)); SPCARAY.SetVisible(cs.RsltJ, false);
                //SPCARAY.SetDisp(cs.RsltK, OM.CmnOptn.sRsltNameK, Color.FromArgb(OM.CmnOptn.iRsltColorK)); SPCARAY.SetVisible(cs.RsltK, false);
                //SPCARAY.SetDisp(cs.RsltL, OM.CmnOptn.sRsltNameL, Color.FromArgb(OM.CmnOptn.iRsltColorL)); SPCARAY.SetVisible(cs.RsltL, false);
                SPCARAY.SetDisp(cs.Empty   , "Empty"   ,Color.Silver  ); SPCARAY.SetVisible(cs.Empty  , false);
                SPCARAY.SetDisp(cs.Unknown , "Unknown" ,Color.Aqua    ); SPCARAY.SetVisible(cs.Unknown, false);
                SPCARAY.SetDisp(cs.None    , "None"    ,Color.White   ); SPCARAY.SetVisible(cs.None   , false);
                //결과값 디스플레이어.
                //pnC0.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor0); lbC0.Text = OM.CmnOptn.sRsltName0 ; 
                //pnC1.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor1); lbC1.Text = OM.CmnOptn.sRsltName1 ; 
                //pnC2.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor2); lbC2.Text = OM.CmnOptn.sRsltName2 ; 
                //pnC3.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor3); lbC3.Text = OM.CmnOptn.sRsltName3 ; 
                //pnC4.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor4); lbC4.Text = OM.CmnOptn.sRsltName4 ; 
                //pnC5.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor5); lbC5.Text = OM.CmnOptn.sRsltName5 ; 
                //pnC6.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor6); lbC6.Text = OM.CmnOptn.sRsltName6 ; 
                //pnC7.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor7); lbC7.Text = OM.CmnOptn.sRsltName7 ; 
                //pnC8.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor8); lbC8.Text = OM.CmnOptn.sRsltName8 ; 
                //pnC9.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor9); lbC9.Text = OM.CmnOptn.sRsltName9 ; 
                //pnCA.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorA); lbCA.Text = OM.CmnOptn.sRsltNameA ; 
                //pnCB.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorB); lbCB.Text = OM.CmnOptn.sRsltNameB ; 
                //pnCC.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorC); lbCC.Text = OM.CmnOptn.sRsltNameC ; 
                //pnCD.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorD); lbCD.Text = OM.CmnOptn.sRsltNameD ; 
                //pnCE.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorE); lbCE.Text = OM.CmnOptn.sRsltNameE ; 
                //pnCF.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorF); lbCF.Text = OM.CmnOptn.sRsltNameF ; 
                //pnCG.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorG); lbCG.Text = OM.CmnOptn.sRsltNameG ; 
                //pnCH.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorH); lbCH.Text = OM.CmnOptn.sRsltNameH ; 
                //pnCI.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorI); lbCI.Text = OM.CmnOptn.sRsltNameI ; 
                //pnCJ.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorJ); lbCJ.Text = OM.CmnOptn.sRsltNameJ ; 
                //pnCK.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorK); lbCK.Text = OM.CmnOptn.sRsltNameK ; 
                //pnCL.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorL); lbCL.Text = OM.CmnOptn.sRsltNameL ; 

                DispDataMap();

                pnCGood.BackColor = Color.Lime;  //lbCntGood.Text = "Good" ; 
                lbCTotal.Text = "Total" ; 

                tvDirectory.Focus();
            }
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            pnDataMap.Refresh();
            SPCARAY.UpdateAray();

            tmUpdate.Enabled = true;
        }

        private void FormSPC_Sub_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            this.Hide(); //x버튼 눌렀을때 폼 숨기기 위해 이렇게 씀
            e.Cancel = true;
        }

        private void FormSPC_Sub_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
            
        }

        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void lvFile_Click(object sender, EventArgs e)
        {

        }


        private void button1_Click_1(object sender, EventArgs e)
        {


        }

        private void DispDataMap()
        {
            if(lvFile.SelectedItems.Count < 1) return;
            string sPath = sSelectedNode + "\\" + lvFile.SelectedItems[0].Text ;
  
            SPC.MAP.LoadDataMap(sPath, SPCARAY);

            int iTemp1 = SPCARAY.MaxCol;
            int iTemp2 = SPCARAY.MaxRow;
        }

        private void lvFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            DispDataMap();
        }
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


}
