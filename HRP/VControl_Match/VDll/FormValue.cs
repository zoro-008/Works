using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDll
{
    public partial class FormValue : Form
    {
        private struct CPara
        {
            public int         iVisionID ;
            public bool        bGlobal   ;
            public CVarTable   VarTable  ;
            public string[]    sTitle    ;
        }
        private CPara Para;

        public enum eType : uint
        {
            String         ,
            Double         ,
            Int32          , 
            Uint32         ,
            Boolean        ,
            Image          ,
        }

        public FormValue(int _iVisionID, bool _bGlobal = false)
        {
            InitializeComponent();

            //Init
            cbType.DataSource = Enum.GetValues(typeof(eType));

            Para.iVisionID = _iVisionID     ;
            Para.bGlobal   = _bGlobal       ;
            Para.sTitle    = new string []{"NO","NAME","COMMENT","TYPE","VALUE","MIN","MAX"};
            if(Para.bGlobal) Para.VarTable  = GV.GlobalTables;
            else             Para.VarTable  = GV.VarTables[_iVisionID];

            InitListView();
            //
            
            this.Text = Para.VarTable.FileName ;
            Update();

            tbName.Select();
            
        }

        #region Init
        private void InitListView()
        {
            //ListView
            lvValue.Clear();

            lvValue.Columns.Add(Para.sTitle[0], 30 , HorizontalAlignment.Left); //No
            lvValue.Columns.Add(Para.sTitle[1], 100, HorizontalAlignment.Left); //Name
            lvValue.Columns.Add(Para.sTitle[2], 200, HorizontalAlignment.Left); //Comment
            lvValue.Columns.Add(Para.sTitle[3], 100, HorizontalAlignment.Left); //Type
            lvValue.Columns.Add(Para.sTitle[4], 100, HorizontalAlignment.Left); //Value
            lvValue.Columns.Add(Para.sTitle[5], 50 , HorizontalAlignment.Left); //Min
            lvValue.Columns.Add(Para.sTitle[6], 50 , HorizontalAlignment.Left); //Max
        }
        #endregion

        #region Event
        private void btAdd_Click(object sender, EventArgs e)
        {
            int    iCnt     = lvValue.Items.Count                 ;
            //int    iNo      = iCnt + 1 ;// CConfig.StrToIntDef(tbName.Text,0)  ;
            string sName    = tbName.Text.Trim()                  ;
            string sComment = tbComment.Text                      ;
            double dMin     = CConfig.StrToDoubleDef(tbMin.Text,0);
            double dMax     = CConfig.StrToDoubleDef(tbMax.Text,0);
            string sText    = cbType.SelectedItem.ToString()      ;
            Type   tType    = Para.VarTable.GetType(sText)        ;

            //Check Condition
            if(sName == "") return;
            
            Para.VarTable.AddValue(tbName.Text,new CValue(sName,sComment,dMin,dMax,tType));
            Update();
        }
        
        private void btDelete_Click(object sender, EventArgs e)
        {
            string sName = "";
            ListView.SelectedListViewItemCollection Items = lvValue.SelectedItems;
            foreach (ListViewItem item in Items)
            {
                sName = item.SubItems[1].Text;
            }
            Para.VarTable.RemoveValue(sName);
            Update();
        }

        private void btDeleteAll_Click(object sender, EventArgs e)
        {
            Para.VarTable.Clear();
            Update();
        }

        private void btChange_Click(object sender, EventArgs e)
        {
            if (lvValue.SelectedIndices.Count <= 0) return;
            int iSel = lvValue.SelectedIndices[0];
            //Name
            string fromName = Para.VarTable.GetName(iSel); 
            string toName   = tbName.Text.Trim()                  ;
            //Value
            string sComment = tbComment.Text                      ;
            double dMin     = CConfig.StrToDoubleDef(tbMin.Text,0);
            double dMax     = CConfig.StrToDoubleDef(tbMax.Text,0);
            string sText    = cbType.SelectedItem.ToString()      ;
            Type   tType    = Para.VarTable.GetType(sText)        ;

            //Check Condition
            if(toName == "") return;

            if (fromName != toName)
            {
                Para.VarTable.RenameValue(fromName,toName);
            }
            Para.VarTable.SetValue(toName,new CValue(toName,sComment,dMin,dMax,tType));
            Update();
            
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            SaveTable();
        }

        private void lvValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvValue.SelectedIndices.Count <= 0) return;
            int iSel = lvValue.SelectedIndices[0];
            tbName.Text          = lvValue.Items[iSel].SubItems[1].Text;
            tbComment.Text       = lvValue.Items[iSel].SubItems[2].Text;
            cbType.SelectedItem  = lvValue.Items[iSel].SubItems[3].Text;
            tbMin.Text           = lvValue.Items[iSel].SubItems[5].Text;
            tbMax.Text           = lvValue.Items[iSel].SubItems[6].Text;
        }
        #endregion

        //로드
        //초기 진입시 로드
        public bool LoadTable(string _sPath = "")
        {
            Para.VarTable.Load(_sPath);
            Update();
            return true;
        }
        public bool SaveTable()
        {
            Para.VarTable.Save();
            return true;
        }

        //업데이트
        //이벤트들마다 뒷부분에 화면 갱신용
        private void Update()
        {
            string sName;
            CValue Value;

            InitListView();

            int iCount = Para.VarTable.GetCount();
            ListViewItem[] lvItem = new ListViewItem[iCount+1];// Para.sTitle.Count()];

            
            for(int i=0; i<iCount; i++)
            {
                sName = Para.VarTable.GetName(i); //느림
                Value = Para.VarTable.GetValue(sName);
                
                lvItem[i] = new ListViewItem((i+1).ToString());
                //lvItem[i].SubItems.Add(Value.No     .ToString()) ;
                lvItem[i].SubItems.Add(Value.Name   .ToString()) ;
                lvItem[i].SubItems.Add(Value.Comment.ToString()) ;
                lvItem[i].SubItems.Add(Value.Type   .ToString()) ;
                lvItem[i].SubItems.Add(Value.Value  .ToString()) ;
                lvItem[i].SubItems.Add(Value.Min    .ToString()) ;
                lvItem[i].SubItems.Add(Value.Max    .ToString()) ;

                lvValue.Items.Add(lvItem[i]);
                
            }
            lvValue.Refresh();

        }

        //타이머
        //화면 정리용
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            if (lvValue.SelectedIndices.Count > 0)
            {
                btDelete.Visible = true;
                btChange.Enabled = true;
            }
            else
            {
                btDelete.Visible = false;
                btChange.Enabled = false;
            }

            tmUpdate.Enabled = true;
        }
    }
}
