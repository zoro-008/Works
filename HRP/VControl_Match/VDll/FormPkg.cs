using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDll.Pakage;

namespace VDll
{
    public partial class FormPkg : Form
    {
        //public List<IPkg> lsPkg ;
        int iVisionID ; 

        public FormPkg(int _iVisionID)
        {
            InitializeComponent();
            iVisionID = _iVisionID ;

            InitPkgList();
            InitPkgType();
            UpdatePkgList();
        }

        void InitPkgList()
        {
            //Error ListView
            lvPkg.Clear();
            lvPkg.View = View.Details;
            //lvPkg.LabelEdit = true;
            lvPkg.AllowColumnReorder = false;
            lvPkg.FullRowSelect = true;
            lvPkg.MultiSelect = false ;
            lvPkg.GridLines = true;
            lvPkg.Sorting = SortOrder.None;

            lvPkg.Columns.Add("No"    , 40 , HorizontalAlignment.Left);
            lvPkg.Columns.Add("Name"  , 140, HorizontalAlignment.Left);
            lvPkg.Columns.Add("Type"  , 140, HorizontalAlignment.Left);
        }

        void InitPkgType()
        {
            cbPkgType.Items.Clear();

            //foreach ( items in dic)
            foreach(KeyValuePair<string, Type> type in GV.DynamicPkgMaker.Types)
            {                
                cbPkgType.Items.Add(type.Key);
            }
            if(cbPkgType.Items.Count > 0) cbPkgType.SelectedIndex = 0 ;
        }

        void UpdatePkgList()
        {
            //lvPkg.Clear();

            lvPkg.Items.Clear();

            int iPkgCnt = GV.Visions[iVisionID].Pkgs.Count ;
            ListViewItem[] liPkg = new ListViewItem[iPkgCnt];
            for (int i = 0; i < liPkg.Length ; i++)
            {
                liPkg[i] = new ListViewItem(i.ToString());
                liPkg[i].SubItems.Add(((CErrorObject)GV.Visions[iVisionID].Pkgs[i]).Name);
                liPkg[i].SubItems.Add(GV.Visions[iVisionID].Pkgs[i].GetType().Name);
                //liPkg[i].UseItemStyleForSubItems = false;
                lvPkg.Items.Add(liPkg[i]);
            }
        }


        private void btAdd_Click(object sender, EventArgs e)
        {
            if(tbName.Text =="") 
            {
                MessageBox.Show("Name is Empty!" , "Error");
                return ;
            }
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                if(Pkg.GetName() == tbName.Text) 
                {
                    MessageBox.Show("Same Named Pkg is Exist!" , "Error");
                    return ;
                }
            }

            IPkg NewPkg = GV.DynamicPkgMaker.New(cbPkgType.Text , new object[]{tbName.Text , iVisionID});

            NewPkg.Init();

            GV.Visions[iVisionID].Pkgs.Add(NewPkg);
            UpdatePkgList();

        }

        private void btInsert_Click(object sender, EventArgs e)
        {
            if(lvPkg.SelectedIndices.Count == 0) return ;

            if(lvPkg.Items.Count <= 0) {
                btAdd_Click( sender,  e);
            }

            if(tbName.Text =="") 
            {
                MessageBox.Show("Name is Empty!" , "Error");
                return ;
            }
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                if(Pkg.GetName() == tbName.Text) 
                {
                    MessageBox.Show("Same Named Pkg is Exist!" , "Error");
                    return ;
                }
            }

            IPkg NewPkg = GV.DynamicPkgMaker.New(cbPkgType.Text , new object[]{tbName.Text , iVisionID});
            if(NewPkg == null) {
                MessageBox.Show("Inserting Pkg Failed!");
                return ;
            }
            
            NewPkg.Init();

            GV.Visions[iVisionID].Pkgs.Insert(lvPkg.SelectedIndices[0] , NewPkg); 
            UpdatePkgList();
        }

        private void btDel_Click(object sender, EventArgs e)
        {
            if(cbPkgType.Items.Count <= 0) return ;
            if(lvPkg.SelectedIndices.Count == 0) return ;

            //sun 나중에 디스포즈 구현하자.
            GV.Visions[iVisionID].Pkgs[lvPkg.SelectedIndices[0]].Close();
            //lsPkg[cbPkgType.SelectedIndex].Dispose();
            

            GV.Visions[iVisionID].Pkgs.RemoveAt(lvPkg.SelectedIndices[0]); 
            UpdatePkgList();
        }

        private void btUp_Click(object sender, EventArgs e)
        {
            if(lvPkg.Items.Count <= 1) return ; //1개면 의미 없음.
            if(lvPkg.SelectedIndices.Count == 0) return ;
            if(lvPkg.SelectedIndices[0] < 1) return ; //더 낮출데가 없음.
            

            int iSel = lvPkg.SelectedIndices[0] ;
            IPkg Pkg = GV.Visions[iVisionID].Pkgs[iSel-1] ;
            GV.Visions[iVisionID].Pkgs[iSel-1] = GV.Visions[iVisionID].Pkgs[iSel];
            GV.Visions[iVisionID].Pkgs[iSel] = Pkg ;
            UpdatePkgList();

            lvPkg.Items[iSel-1].Focused = true;
            lvPkg.Items[iSel-1].Selected = true;
        }

        private void btDown_Click(object sender, EventArgs e)
        {
            if(lvPkg.Items.Count <= 1) return ; //1개면 의미 없음.
            if(lvPkg.SelectedIndices.Count == 0) return ;
            if(lvPkg.SelectedIndices[0] >= lvPkg.Items.Count -1) return ; //더 낮출데가 없음.

            int iSel = lvPkg.SelectedIndices[0] ;
            IPkg Pkg = GV.Visions[iVisionID].Pkgs[iSel+1] ;
            GV.Visions[iVisionID].Pkgs[iSel+1] = GV.Visions[iVisionID].Pkgs[iSel];
            GV.Visions[iVisionID].Pkgs[iSel] = Pkg ;
            UpdatePkgList();

            lvPkg.Items[iSel+1].Focused = true;
            lvPkg.Items[iSel+1].Selected = true;
        }

        private void btRename_Click(object sender, EventArgs e)
        {

        }
        
    }
}
