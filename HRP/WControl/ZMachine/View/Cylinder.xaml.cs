using COMMON;
using SML;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Machine.View
{
    /// <summary>
    /// Cylinder.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Cylinder : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        public ci                 m_iActrId   ;
        public string             m_sActrName ;
        public EN_MOVE_DIRECTION  m_iType     ;
        public bool               m_bPreCmd   ;
        public string             m_sCaptionFw;
        public string             m_sCaptionBw;
        //public dgCheckSafe        m_CheckSafe;
        public bool[] bActivate = new bool[(int)ci.MAX_ACTR];

        public int iFwd = 0;
        public int iBwd = 0;

        public string sFwd = "";
        public string sBwd = "";


        public const int Up    = 0,
                         Down  = 1,
                         Left  = 2,
                         Right = 3,
                         FWD   = 4,
                         BWD   = 5,
                         CW    = 6,
                         CCW   = 7,
                         OPEN  = 8,
                         CLOSE = 9;

        private bool bLoad = false;

        public Cylinder()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }

        private void BtRepeat_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.TraceListView(m_sActrName + "_" + sText + " Button Clicked", ForContext.Frm);            
            
            int iSelNo = Convert.ToInt32(TbCylNo.Text);
            int iRptTm = CConfig.StrToIntDef(Tbmsec.Text);
            if (TbCylNo.Text != "" && !bActivate[iSelNo])
            {
                ML.CL_GoRpt(iRptTm, iSelNo);
                bActivate[iSelNo] = true;
            }
            else if(bActivate[iSelNo])
            {
                ML.CL_StopRpt();
                bActivate[iSelNo] = false;
            }
        }

        private void BtFwd_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.TraceListView(m_sActrName + "_" + sText + " Button Clicked", ForContext.Frm);
            
            ML.CL_Move(m_iActrId, fb.Fwd);
            //TODO :: if (SEQ.LODR.CheckSafe(m_iActrId, fb.Fwd , true) && 
            //TODO ::     SEQ.PREB.CheckSafe(m_iActrId, fb.Bwd , true) &&
            //TODO ::     SEQ.VSNZ.CheckSafe(m_iActrId, fb.Bwd , true) &&
            //TODO ::     SEQ.PSTB.CheckSafe(m_iActrId, fb.Bwd , true) &&
            //TODO ::     SEQ.ULDR.CheckSafe(m_iActrId, fb.Fwd , true)) ML.CL_Move(m_iActrId, fb.Fwd);
        }

        private void BtBwd_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.TraceListView(m_sActrName + "_" + sText + " Button Clicked", ForContext.Frm);

            ML.CL_Move(m_iActrId, fb.Bwd);
            //TODO :: if (SEQ.LODR.CheckSafe(m_iActrId, fb.Bwd , true) &&
            //TODO ::     SEQ.PREB.CheckSafe(m_iActrId, fb.Bwd , true) &&
            //TODO ::     SEQ.VSNZ.CheckSafe(m_iActrId, fb.Bwd , true) &&
            //TODO ::     SEQ.PSTB.CheckSafe(m_iActrId, fb.Bwd , true) &&
            //TODO ::     SEQ.ULDR.CheckSafe(m_iActrId, fb.Bwd , true)) ML.CL_Move(m_iActrId, fb.Bwd);

        }

        public void SetConfig(ci _iActrId, string _sTitle, EN_MOVE_DIRECTION _iActrType, Frame _wcParent /*, dgCheckSafe _CheckSafe*/)
        {
            m_sActrName = _sTitle;
            //bActivate = new bool [SML.CL]
            if(m_sActrName == null) return;
            m_sActrName = m_sActrName.Replace("_", " ");
            bLoad = true;
        
            m_iActrId = _iActrId;      //실린더 넘버
            TbCylNo.Text = ((int)m_iActrId).ToString();
            TbCylName.Text = m_sActrName;//실린더 이름
            m_iType = _iActrType;      
        
            m_bPreCmd = true;
            //tmUpdate.Enabled = true;

            switch (m_iType)
            {
                default: iFwd = Left; sFwd = "LEFT"; iBwd = Right; sBwd = "RIGHT"; break;

                case EN_MOVE_DIRECTION.LR: sBwd = "LEFT"  ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "RIGHT" ; BtFwd.Click += (BtFwd_Click); break;
                case EN_MOVE_DIRECTION.RL: sBwd = "RIGHT" ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "LEFT"  ; BtFwd.Click += (BtFwd_Click); break; 
                case EN_MOVE_DIRECTION.BF: sBwd = "BWD"   ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "FWD"   ; BtFwd.Click += (BtFwd_Click); break;
                case EN_MOVE_DIRECTION.FB: sBwd = "FWD"   ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "BWD"   ; BtFwd.Click += (BtFwd_Click); break;
                case EN_MOVE_DIRECTION.UD: sBwd = "UP"    ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "DN"    ; BtFwd.Click += (BtFwd_Click); break;
                case EN_MOVE_DIRECTION.DU: sBwd = "DN"    ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "UP"    ; BtFwd.Click += (BtFwd_Click); break;
                case EN_MOVE_DIRECTION.CA: sBwd = "CW"    ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "CCW"   ; BtFwd.Click += (BtFwd_Click); break;
                case EN_MOVE_DIRECTION.AC: sBwd = "CCW"   ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "CW"    ; BtFwd.Click += (BtFwd_Click); break;
                case EN_MOVE_DIRECTION.OC: sBwd = "OPEN"  ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "CLOSE" ; BtFwd.Click += (BtFwd_Click); break;
                case EN_MOVE_DIRECTION.CO: sBwd = "CLOSE" ; BtBwd.Click += (BtBwd_Click);
                                           sFwd = "OPEN"  ; BtFwd.Click += (BtFwd_Click); break;
            }

            BtFwd.Content = sFwd;
            BtBwd.Content = sBwd;
            //lbFwd.Text = sFwd; 
            //lbBwd.Text = sBwd;

            if(_wcParent != null) _wcParent.Content = this;

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if(!bLoad) return;

            bool bCmd     = ML.CL_GetCmd  (m_iActrId        ) == 0 ? true : false;
            bool bErr     = ML.CL_Err     (m_iActrId        );
            bool bDone    = ML.CL_Complete(m_iActrId        );
            bool bDoneFwd = ML.CL_Complete(m_iActrId, fb.Fwd);
            bool bDoneBwd = ML.CL_Complete(m_iActrId, fb.Bwd);

            //if (bDone)
            //{
            //    lbFwd.BackColor = bCmd ? Color.ForestGreen    : SystemColors.Control;
            //    lbBwd.BackColor = bCmd ? SystemColors.Control : Color.ForestGreen   ;
            //}
            if (bErr)
            {
                BtBwd.Background = Brushes.Red;
                BtFwd.Background = Brushes.Red;
            }
            

            if (bCmd != m_bPreCmd)
            {
                if ((int)ML.CL_GetCmd(m_iActrId) == 0) { BtBwd.Background = Brushes.ForestGreen; BtFwd.Background = Brushes.Silver; }
                else                                   { BtFwd.Background = Brushes.ForestGreen; BtBwd.Background = Brushes.Silver; }
            }

            m_bPreCmd = bCmd;


        }


    }
}
