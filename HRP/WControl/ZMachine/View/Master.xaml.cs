using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using COMMON;
using SML;
using System.ComponentModel;

namespace Machine.View
{
    /// <summary>
    /// Master.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Master : Window
    {
        private const string sFormText = "Form Master ";

        private bool bLoad = false;

        public Master()
        {
            InitializeComponent();

            UpdateMstOptn(true);
            OM.LoadMstOptn();
        }

        public void AddListView()
        {
            SequenceData.GetInstance().Clear();
            for (int i = (int)pi.MAX_PART-1; i >= 0 ; i--) //0부터 시작하면 파트 순서가 역순으로 쌓여서 뒤에서부터 돌린다.
            {
                SequenceData.GetInstance().Add(new SequenceData()
                {
                    No       = i.ToString()                               ,
                    PartName = SEQ.m_Part[i].GetPartName     ()           ,
                    ToStart  = SEQ.m_Part[i].GetToStartStep  ().ToString(),
                    Seq      = SEQ.m_Part[i].GetCrntCycleName().ToString(),
                    Cycle    = SEQ.m_Part[i].GetCycleStep    ().ToString(),
                    ToStop   = SEQ.m_Part[i].GetToStopStep   ().ToString(),
                    Home     = SEQ.m_Part[i].GetHomeStep     ().ToString()
                });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddListView();

            listView.ItemsSource = SequenceData.GetInstance();

            bLoad = true;
        }

        public void UpdateMstOptn(bool bTable)
        {
            if(bTable)
            {
                CConfig.ValToCon(cbDebug       , ref OM.MstOptn.bDebugMode  );
                CConfig.ValToCon(cbIdlerun     , ref OM.MstOptn.bIdleRun    );
                //CConfig.ValToCon(tbTrgOfs      , ref OM.MstOptn.dTrgOfs     );
                //CConfig.ValToCon(cbMarkAlign   , ref OM.MstOptn.bMarkAlgin  );
                //cbDebug        .Checked = OM.MstOptn.bDebugMode   ;
                //cbIdlerun      .Checked = OM.MstOptn.bIdleRun     ;
                //               
                //tbTrgOfs       .Text    = OM.MstOptn.dTrgOfs.ToString()  ;
            }
            else{
                CConfig.ConToVal(cbDebug       , ref OM.MstOptn.bDebugMode  );
                CConfig.ConToVal(cbIdlerun     , ref OM.MstOptn.bIdleRun    );
                //CConfig.ConToVal(tbTrgOfs      , ref OM.MstOptn.dTrgOfs     );
                //CConfig.ConToVal(cbMarkAlign   , ref OM.MstOptn.bMarkAlgin  );
                //OM.MstOptn.bDebugMode     = cbDebug  .Checked   ;
                //OM.MstOptn.bIdleRun       = cbIdlerun.Checked   ;
                //                         
                //OM.MstOptn.dTrgOfs        = CConfig.StrToDoubleDef(tbTrgOfs.Text, 0.0);
                
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "Save Button Clicked", ForContext.Frm);

            UpdateMstOptn(false);
            OM.SaveMstOptn();
        }
        
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!bLoad) return;
            if(listView.SelectedItem == null) return;
            int iPartNo = listView.SelectedIndex;
            if(!SequenceData.GetInstance()[iPartNo].Enabled) { SequenceData.GetInstance()[iPartNo].Enabled = true ; }
            else                                             { SequenceData.GetInstance()[iPartNo].Enabled = false; }
            listView.SelectedItem = null; 
        }

        private void BtPartReset_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "Part Reset Button Clicked", ForContext.Frm);

            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                if (SequenceData.GetInstance()[i].Enabled)
                {
                    SEQ.m_Part[i].Reset();
                }
            }
        }
        
        private void BtPartAutorun_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "Part Autorun Button Clicked", ForContext.Frm);

            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                if (SequenceData.GetInstance()[i].Enabled)
                {
                    SEQ.m_Part[i].Autorun();
                }
            }
        }

        private void BtAllReset_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "All Reset Button Clicked", ForContext.Frm);

            SEQ.Reset();
        }

        private void BtAllCheck_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                if(!SequenceData.GetInstance()[i].Enabled) SequenceData.GetInstance()[i].Enabled = true ;
                else                                       SequenceData.GetInstance()[i].Enabled = false;
            }
        }
    }

    public class SequenceData : INotifyPropertyChanged
    {
        private string _no      ;
        private bool   _enabled ;
        private string _partName;
        private string _toStart ;
        private string _seq     ;
        private string _cycle   ;
        private string _toStop  ;
        private string _home    ;

        public string No      { get { return _no      ; } set{ _no       = value; OnPropertyChanged("No"      ) ;} }
        public bool   Enabled { get { return _enabled ; } set{ _enabled  = value; OnPropertyChanged("Enabled" ) ;} }
        public string PartName{ get { return _partName; } set{ _partName = value; OnPropertyChanged("PartName") ;} }
        public string ToStart { get { return _toStart ; } set{ _toStart  = value; OnPropertyChanged("ToStart" ) ;} }
        public string Seq     { get { return _seq     ; } set{ _seq      = value; OnPropertyChanged("Seq"     ) ;} }
        public string Cycle   { get { return _cycle   ; } set{ _cycle    = value; OnPropertyChanged("Cycle"   ) ;} }
        public string ToStop  { get { return _toStop  ; } set{ _toStop   = value; OnPropertyChanged("ToStop"  ) ;} }
        public string Home    { get { return _home    ; } set{ _home     = value; OnPropertyChanged("Home"    ) ;} }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        //private static List<Listdata> instance;
        private static ObservableCollection<SequenceData> instance;
        
        //public static List<Listdata> GetInstance()
        public static ObservableCollection<SequenceData> GetInstance()
        {
            if (instance == null) { instance = new ObservableCollection<SequenceData>();}
            return instance;
        }
    }
}
