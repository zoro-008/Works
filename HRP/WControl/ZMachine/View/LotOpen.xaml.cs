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
using COMMON;
using SML;

namespace Machine.View
{
    /// <summary>
    /// LotOpen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LotOpen : Window
    {
        private const string sFormText = "Form Lot Open ";

        public LotOpen()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbDevice.Text = OM.GetCrntDev();
        }

        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "Close Button Clicked", ForContext.Frm);
            this.Close();
        }

        private void BtLotOpen_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "Lot Open Button Clicked", ForContext.Frm);

            if (tbLotNo.Text == "") return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");

            string LotNo = tbLotNo.Text.Trim();
            string Device = tbDevice.Text.Trim();

            LOT.TLot Lot;

            Lot.sLotNo = tbLotNo.Text.Trim();
         
            LOT.LotOpen(Lot);

             
            //Machine.View.Oper.LotInfoList.New();

            //DM.ARAY[ri.STT].SetStat(cs.None);
            //DM.ARAY[ri.PRE].SetStat(cs.None);
            //DM.ARAY[ri.ADJ].SetStat(cs.None);
            //DM.ARAY[ri.PST].SetStat(cs.None);

            //FrmOperation.

            //Log.Trace("LotOpen", "Try");

            //CDelayTimer TimeOut = new CDelayTimer();
            //TimeOut.Clear();
            //SEQ.Visn.SendLotStart(Lot.sLotNo);
            //while(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.LotStart  )){
            //    Thread.Sleep(1);
            //    if(TimeOut.OnDelay(5000)) { 
            //        SM.ER_SetErr(ei.VSN_ComErr,"Lot Start TimeOut");
            //        break;
            //    }
            //}
            //DM.ARAY[ri.LODR].SetStat(cs.Unknown);

            //DM.ARAY[ri.BPCK].SetStat(cs.None );      

            Close();

        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (LOT.GetLotOpen())
            {
                tbLotNo.Text = LOT.GetLotNo();
                //tbWorkCount.Text = OM.EqpStat.iTotalWorkCount.ToString();
                tbLotNo    .IsEnabled = false;
                //tbWorkCount.Enabled = false;
                btLotOpen  .IsEnabled = false;
                //btRework   .Enabled = false;
            }
            else
            {
                tbLotNo    .IsEnabled = true;
                //tbWorkCount.Enabled = true;
                btLotOpen  .IsEnabled = true;
                //btRework   .Enabled = true;
            }
        }
    }
}
