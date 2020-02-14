using SMDll2;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Machine
{
    
    public partial class FormMain : Form
    {
        public FormOperation FrmOperation ;
        public FormVision    FrmVision    ;
        public FormDevice    FrmDevice    ;
        public FormDeviceSet FrmDeviceSet ;
        public FormSPC       FrmSPC       ;
        public FormOption    FrmOption    ;
        public FormPassword  FrmPassword  ;
        public FormMaster    FrmMaster    ;
         
        public int m_iActivePage = 0;

        public FormMain()
        {
            //this.TopMost = true;
            SEQ.Init();
            OM.LoadLastInfo();
            //VC.Init();

            InitializeComponent();

            m_iActivePage = 0;


            FrmOperation = new FormOperation(pnBase);
            FrmVision    = new FormVision   (pnBase);
            FrmDevice    = new FormDevice   (this  );
            FrmDeviceSet = new FormDeviceSet(pnBase);
            FrmSPC       = new FormSPC      (pnBase);
            FrmOption    = new FormOption   (pnBase);

            FrmMaster    = new FormMaster();


            FrmDevice.TopLevel = false;
            FrmDevice.Parent = pnBase;
   
            lbDevice.Text = OM.GetCrntDev().ToString();

            lbLotNo.Text  = LOT.GetLotNo();
            lbName.Text   = OM.EqpOptn.sModelName;

            //USB 현미경 프로그램 키는거            
            EmbededExe.CameraInit();
            EmbededExe.SetCamParent(FrmVision.Handle);

            ShowPage(0);//Operation.

            tmUpdate.Enabled = true;

            

        }

        public void ShowPage(int _iPageIdx)
        {
            FrmDeviceSet.UpdateDevInfo(true);
            FrmDeviceSet.UpdateDevOptn(true);
            PM.UpdatePstn(true);

            switch (_iPageIdx)
            {
                case 0: FrmOperation.Show();
                        
                        break;        
                case 1: FrmVision   .Show();      
                        EmbededExe.SetCamParent(FrmVision.Handle);
                        break;
                case 2: FrmDevice   .Show();          break;
                case 3: FrmOption   .Show();          break;
                case 4: FrmSPC      .Show();
                        FrmSPC      .ShowUpdate();    break;
                case 5: SM.SetDllMainWin(ref pnBase); break;
                default: FrmOperation.Show();         break;
            }
          
        }
        private void HidePage(int _iPageIdx)
        {
            switch (_iPageIdx)
            {
                case 0: FrmOperation.Hide();          break;
                case 1: FrmVision   .Hide();          break;
                case 2: FrmDevice   .Hide();          
                        FrmDeviceSet.Hide();          break;
                case 3: FrmOption   .Hide();          break;
                case 4: FrmSPC      .Hide();          break;
                case 5: SM.HideDllMainWin();          break;
                default: FrmOperation.Hide();         break;
            }
        }

        bool bMsgEnd = false;
        bool bRecvEnd = false;
        int iExitCnt = 0;
        DateTime tDateTime;
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            //tm tmUpdate.Enabled = false ;
            double dLtTotalTime = SEQ.ASY.dLtTotalTime;
            double dRtTotalTime = SEQ.ASY.dRtTotalTime;
            double dAssyTotalTime = SEQ.ASY.dAssyTotalTime;

            
            DateTime tLtTotalTime   = DateTime.FromOADate(dLtTotalTime);
            DateTime tRtTotalTime   = DateTime.FromOADate(dRtTotalTime);
            DateTime tAssyTotalTime = DateTime.FromOADate(dAssyTotalTime);

            label2.Text = tLtTotalTime.ToString("ss:fff");
            label5.Text = tRtTotalTime.ToString("ss:fff");
            label4.Text = tAssyTotalTime.ToString("ss:fff");

            lbMainThreadTime.Text = string.Format("{0:0.000ms}", SEQ._dMainThreadCycleTime);
            lbDevice.Text = OM.GetCrntDev().ToString();
            lbLotNo.Text = LOT.GetLotNo();

            btExit.Enabled = !SEQ._bRun ;
            
            //Set Sequence State.
            if(!OM.MstOptn.bDebugMode){
            
                switch(SEQ._iSeqStat) {
                    default        :
                    break ;
                    case EN_SEQ_STAT.ssInit    : lbStat.Text = "INIT"      ; lbStat.ForeColor = Color.Blue   ;
                    break ;
                    case EN_SEQ_STAT.ssError   : lbStat.Text = "ERROR"     ; lbStat.ForeColor = SEQ._bFlick ? Color.Yellow : Color.Red ;
                    break ;
                    case EN_SEQ_STAT.ssRunning : lbStat.Text = "RUNNING"   ; lbStat.ForeColor = Color.Lime   ;
                    break ;
                    case EN_SEQ_STAT.ssStop    : lbStat.Text = "STOP"      ; lbStat.ForeColor = Color.Black  ;
                    break ;
                    case EN_SEQ_STAT.ssWorkEnd : lbStat.Text = "LOTEND"    ; lbStat.ForeColor = Color.Gray   ;
                    break ;
                }
            }
            else {
                switch(SEQ._iSeqStat) {
                    default                    :                                                                                                  break;
                    case EN_SEQ_STAT.ssInit    : lbStat.Text = "DEBUG INIT"    ; lbStat.ForeColor = Color.Blue                                  ; break;
                    case EN_SEQ_STAT.ssError   : lbStat.Text = "DEBUG ERROR"   ; lbStat.ForeColor = SEQ._bFlick ? Color.Yellow : Color.Red      ; break;
                    case EN_SEQ_STAT.ssRunning : lbStat.Text = "DEBUG RUNNING" ; lbStat.ForeColor = Color.Lime                                  ; break;
                    case EN_SEQ_STAT.ssStop    : lbStat.Text = "DEBUG STOP"    ; lbStat.ForeColor = Color.Black                                 ; break;
                    case EN_SEQ_STAT.ssWorkEnd : lbStat.Text = "DEBUG LOTEND"  ; lbStat.ForeColor = Color.Gray                                  ; break;
                }
            }
            
            //접근 레벨 Operator에서 Option/Util 버튼 비활성화
            if (FormPassword.GetLevel() == EN_LEVEL.lvOperator)
            {
                //btSpc.Enabled    = false;
                btUtil.Enabled      = false;
                btOption.Enabled = false;
            }

            else
            {
                //btSpc.Enabled    = true;
                btUtil.Enabled      = true;
                btOption.Enabled = true;
            }
            if (bMsgEnd && VC.GetVisnSendMsg() == "OK") bRecvEnd = true;
            if (iExitCnt >= 2) bRecvEnd = true;

            lbVisnCnt.Text = "Rcv:" + VC.GetVisnRecvViewMsg();
            lbLTorque.Text = "Left  Torque:" + SM.MT.GetIntStat((int)mi.PCK_TL,"Torque");
            lbRTorque.Text = "Right Torque:" + SM.MT.GetIntStat((int)mi.PCK_TR, "Torque");

            if((SEQ._iStep == EN_SEQ_STEP.scToStart || 
                (MM.GetManNo() == mc.AllHome || MM.GetManNo() == mc.ASY_Home)) &&
                EmbededExe.GetHPSHandle() < 0)
            {
                //EmbededExe.SetCamParent(FrmVision.Handle);
            }

            lbBfDegree.Text = SEQ.ASY.dBfThetaEncPos.ToString() ;
            lbAtDegree.Text = SEQ.ASY.dAtThetaEncPos.ToString();
            lbMovePos .Text = SEQ.ASY.dMovePos      .ToString();
            //tm tmUpdate.Enabled = true  ;

        }

        private void btExit_Click(object sender, EventArgs e)
        {
            if (EmbededExe.GetHPSHandle() > 0)
            {
                VC.SendVisnMsg(VC.sm.End);
                bMsgEnd = true;
                iExitCnt++;
                if (!bRecvEnd) return;
            }

            SEQ.Close();
            FrmOperation.Close();
            FrmDevice.Close();
            FrmDeviceSet.Close();
            FrmMaster.Close();
            FrmOption.Close();


            Close();
        }

        private void btOperation_Click(object sender, EventArgs e)
        {
            HidePage(m_iActivePage);
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            m_iActivePage = iBtnTag;
            ShowPage(m_iActivePage);
        }




        private void lbName_Click(object sender, EventArgs e)
        {
            if (FormPassword.GetLevel() == EN_LEVEL.lvMaster)
            {
                if (FrmMaster.IsDisposed)
                {
                    FrmMaster = new FormMaster();
                }
                FrmMaster.Show();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        //public struct COPYDATASTRUCT
        //{
        //    public IntPtr dwData;
        //    public int cbData;
        //    [MarshalAs(UnmanagedType.LPStr)]
        //    public string lpData;
        //}

        /*
         * public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
            public void Clear()
            {
                dwData = (IntPtr)mc.NONE;
                cbData = 0;
                lpData = "";
            }
        }
         */

        public const int WM_COPYDATA = 0x4A;

        /*
        ///SendMessage Receive
        protected override void WndProc(ref Message m)
        {      
            const int WM_COPYDATA = 0x4A;
            VC.COPYDATASTRUCT DataMsg;
            object obj; Message mm;
            //try
            //{
                switch (m.Msg)
                {
                    default:
                        base.WndProc(ref m);
                        break;
                    
                    case WM_COPYDATA:
                        mm = m ;
                        obj = mm.GetLParam(typeof(VC.COPYDATASTRUCT));
                        DataMsg = (VC.COPYDATASTRUCT)obj;
                        VC.SetReceivedMsg((VC.COPYDATASTRUCT)m.GetLParam(typeof(VC.COPYDATASTRUCT)));

                    return ;                    
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
                
            //}
        }
        */

        VC.COPYDATASTRUCTSUB CopyDataSub;
        VC.COPYDATASTRUCT    CopyData;
        ///SendMessage Receive
        protected override void WndProc(ref Message m)
        {
            const int WM_COPYDATA = 0x4A;
            //try
            //{
            switch (m.Msg)
            {
                default:
                    base.WndProc(ref m);
                    break;

                case WM_COPYDATA:
                    //카피데이터를 할때 c#에서 구조체에 String이 들어가서 사이즈가 달라져서 이렇게 함.
                    //사이즈가 달라져서 비전결과값이 올때 뒷쪽에 쓰레기가 들어갔는데 그중에 멀티바이트에서 유니코드
                    //맵핑 안되는 놈이 있으면 뻑나서 이런 방식으로 바꿈.
                    CopyDataSub = (VC.COPYDATASTRUCTSUB)m.GetLParam(typeof(VC.COPYDATASTRUCTSUB));
                    CopyData.cbData = CopyDataSub.cbData;
                    CopyData.dwData = CopyDataSub.dwData;
                    CopyData.lpData = "";

                    byte[] bTemp = new byte[CopyDataSub.cbData];

                    Marshal.Copy(CopyDataSub.lpData, bTemp, 0 , CopyDataSub.cbData);
                    CopyData.lpData = System.Text.Encoding.Default.GetString(bTemp);

                    VC.SetReceivedMsg(CopyData);
                    return;
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);

            //}
        }


        private void button2_Click(object sender, EventArgs e)
        {
            DM.ARAY[(int)ri.FRNT].SetStat(0, 0, cs.Unkwn);
            DM.ARAY[(int)ri.FRNT].SetStat(0, 1, cs.Work);
            DM.ARAY[(int)ri.FRNT].SetStat(0, 2, cs.Work);
            DM.ARAY[(int)ri.FRNT].SetStat(0, 3, cs.Work);
            DM.ARAY[(int)ri.FRNT].SetStat(0, 4, cs.Work);
            DM.ARAY[(int)ri.FRNT].SetStat(0, 5, cs.Work);
            
        }
        
    }
}
