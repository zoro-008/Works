using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormManual : Form
    {
        ValveDisplay ValveDisp;

        public FormManual(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;
            this.Left = 0 ;
            this.Top  = 0 ;

            //this.Dock = DockStyle.Fill;
            

            //Scable Setting
            //this.Dock = DockStyle.Fill;
            //int  _iWidth  = _pnBase.Width;
            //int  _iHeight = _pnBase.Height;
            //
            //const int  iWidth  = 1920;
            //const int  iHeight = 920;
            //
            //float widthRatio  = _iWidth   / (float)iWidth;// this.ClientSize.Width;//1280f;
            //float heightRatio = _iHeight  / (float)iHeight;//.ClientSize.Height; //863f ;
            //
            //SizeF scale = new SizeF(widthRatio, heightRatio);
            //this.Scale(scale);

            //foreach (Control control in this.Controls)
            //{
            //control.Scale(scale); 
            //control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            //}

            ValveDisp = new ValveDisplay(pnValves);

            MakeDoubleBuffered(pnValves, true);
        }

        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void FormManual_Load(object sender, EventArgs e)
        {
            //pnManualMain.Dock = DockStyle.None ;
            //pnManualMain.Left = 0 ;
            //pnManualMain.Top  = 0 ;
            //pnManualMain.Width = Parent.Width ;
            //pnManualMain.Height = Parent.Height ;
        }

        private void pnManualMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FormManual_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = Visible ;
            //label1.Text = pnValves.Width.ToString() + "," + pnValves.Height.ToString() ;
            //pnValves.Size = new Size(782, 679);
        }

        private void pnValves_Resize(object sender, EventArgs e)
        {
            int i = 0 ;
            i++;
        }

        private void pnValves_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            ValveDisp.Paint(g);
            
        }

        private void pnValves_MouseClick(object sender, MouseEventArgs e)
        {
            ValveDisp.Click(e.X, e.Y);
        }

        private void pnValves_MouseMove(object sender, MouseEventArgs e)
        {
            lbMousePoint.Text = e.X.ToString() + " : " + e.Y.ToString() + " W:" + Width.ToString() + " H:" + Height.ToString();

            string sTip = ValveDisp.Move(e.X, e.Y) ;

            if(sTip != "")
            {
                ttValve.Active = true ;
                ttValve.SetToolTip(pnValves , sTip);
            }
            else
            {
                 ttValve.Active = false ;
            }
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            pnValves.Invalidate();

            lbBarcode.Text = SEQ.Bar.GetReadingText();
             
            btSyrActivate.Enabled = !SEQ.SyringePump.GetBusy(cbSyrID.SelectedIndex);
        }


        private void btTankEmpty_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button ;
            if(btn == null)return ;

            //Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            int iTag = Convert.ToInt32(btn.Tag);

            if (iTag == 0)
            {
                SEQ.CHA.TankCP1toFCMWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btTankCP1toFCMWST.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray; 
            }
            else if(iTag == 1)
            {
                SEQ.CHA.FCMWSTtoMAINWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 2)
            {
                SEQ.CHA.MainWtoExtW(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 10)
            {
                SEQ.CHA.TankCP2toMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            
            }
            else if(iTag == 20)
            {
                
                SEQ.CHA.TankCP3toMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 30)
            {
                
                SEQ.CHA.TankCSFtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 40)
            {
                
                SEQ.CHA.TankCSRtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 50)
            {
                
                SEQ.CHA.TankSULFtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 60)
            {
                
                SEQ.CHA.TankFBtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 70)
            {
                
                SEQ.CHA.Tank4DLtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 80)
            {
                
                SEQ.CHA.TankRETtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 90)
            {
                
                SEQ.CHA.TankNRtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            
        }

        private void btActivate_Click(object sender, EventArgs e)
        {
            if (cbSyrFunction.SelectedIndex == 0) //ZR
            {
                SEQ.SyringePump.YR(cbSyrID.SelectedIndex);
            }
            else if (cbSyrFunction.SelectedIndex == 1) //AbsMove 
            {
                SEQ.SyringePump.AbsMove(cbSyrID.SelectedIndex , (VALVE_POS)cbSyrValvePos1.SelectedIndex , decimal.ToInt32(tbSyrPos.Value) , decimal.ToInt32(tbSyrSpeed.Value)) ;
            }
            else if (cbSyrFunction.SelectedIndex == 2) //DispIncPos
            {
                SEQ.SyringePump.DispIncPos(cbSyrID.SelectedIndex , (VALVE_POS)cbSyrValvePos1.SelectedIndex , decimal.ToInt32(tbSyrPos.Value) , decimal.ToInt32(tbSyrSpeed.Value)) ;
            }
            else if (cbSyrFunction.SelectedIndex == 3) //PickupIncPos
            {
                SEQ.SyringePump.PickupIncPos(cbSyrID.SelectedIndex , (VALVE_POS)cbSyrValvePos1.SelectedIndex , decimal.ToInt32(tbSyrPos.Value) , decimal.ToInt32(tbSyrSpeed.Value)) ;
            }
            else if (cbSyrFunction.SelectedIndex == 4) //PickupAndDispInc
            {
                SEQ.SyringePump.PickupAndDispInc(cbSyrID.SelectedIndex , (VALVE_POS)cbSyrValvePos1.SelectedIndex , (VALVE_POS)cbSyrValvePos2.SelectedIndex , decimal.ToInt32(tbSyrPos.Value) , decimal.ToInt32(tbSyrSpeed.Value), decimal.ToInt32(tbSyrTimes.Value)) ;
            }
            else if (cbSyrFunction.SelectedIndex == 5) //PickupAndDispInc
            {
                SEQ.SyringePump.DispAndPickupInc(cbSyrID.SelectedIndex, (VALVE_POS)cbSyrValvePos1.SelectedIndex, (VALVE_POS)cbSyrValvePos2.SelectedIndex, decimal.ToInt32(tbSyrPos.Value), decimal.ToInt32(tbSyrSpeed.Value), decimal.ToInt32(tbSyrTimes.Value));
            }
        }

        private void cbFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSyrFunction.SelectedIndex == 0) //YR
            {
                cbSyrValvePos1.Enabled = false ;
                cbSyrValvePos2.Enabled = false ;
                tbSyrPos.Enabled = false ;
                tbSyrSpeed.Enabled = false ;
                tbSyrTimes.Enabled = false ;
            }
            else if(cbSyrFunction.SelectedIndex == 4|| cbSyrFunction.SelectedIndex == 5)
            {
                cbSyrValvePos1.Enabled = true;
                cbSyrValvePos2.Enabled = true;
                tbSyrPos.Enabled = true;
                tbSyrSpeed.Enabled = true;
                tbSyrTimes.Enabled = true;
            }
            else
            {
                cbSyrValvePos1.Enabled = true ;
                cbSyrValvePos2.Enabled = false ;
                tbSyrPos.Enabled = true ;
                tbSyrSpeed.Enabled = true ;
                tbSyrTimes.Enabled = false;
            }

        }

        private void btFcmActivate_Click(object sender, EventArgs e)
        {
            if (cbFcmFunc.SelectedIndex == 0) //ReqStatus
            {
                SEQ.FcmTester.SendReqStatus();
            }
            else if (cbFcmFunc.SelectedIndex == 1) //Test
            {
                SEQ.FcmTester.SendTestSeq((TCPIP_NewOpticsFCM.ETestSeq)(cbFcmTestSub.SelectedIndex+1) , tbFcmTestSub.Text);
            }
            else if (cbFcmFunc.SelectedIndex == 2) //SubTest
            {
                     if(cbFcmSubTestSub.SelectedIndex <  4) SEQ.FcmTester.SendTestSub((TCPIP_NewOpticsFCM.ETest)(cbFcmSubTestSub.SelectedIndex+1) );
                else if(cbFcmSubTestSub.SelectedIndex == 4) SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.DC_TEST);
                else if(cbFcmSubTestSub.SelectedIndex == 5) SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_1_TEST);
                else if(cbFcmSubTestSub.SelectedIndex == 6) SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_2_TEST);

            }
            else if (cbFcmFunc.SelectedIndex == 3) //Hemo
            {
                SEQ.FcmTester.SendHemog(tbFcmHemoSub.Text );
            }
            else 
            {
                SEQ.FcmTester.SendPing();
            }
        }

        private void cbFcmFunc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFcmFunc.SelectedIndex == 0) //ReqStatus
            {
                cbFcmTestSub   .Enabled = false ;
                cbFcmSubTestSub.Enabled = false ;
                tbFcmHemoSub   .Enabled = false ;
                tbFcmTestSub   .Enabled = false ;
            }
            else if (cbFcmFunc.SelectedIndex == 1) //Test
            {
                cbFcmTestSub   .Enabled = true  ;
                cbFcmSubTestSub.Enabled = false ;
                tbFcmHemoSub   .Enabled = false ;
                tbFcmTestSub   .Enabled = true  ;
            }
            else if (cbFcmFunc.SelectedIndex == 2) //SubTest
            {
                cbFcmTestSub   .Enabled = false ;
                cbFcmSubTestSub.Enabled = true  ;
                tbFcmHemoSub   .Enabled = false ;
                tbFcmTestSub   .Enabled = false ;
            }
            else if (cbFcmFunc.SelectedIndex == 3) //Hemo
            {
                cbFcmTestSub   .Enabled = false ;
                cbFcmSubTestSub.Enabled = false ;
                tbFcmHemoSub   .Enabled = true  ;
                tbFcmTestSub   .Enabled = false ;
            }
            else if (cbFcmFunc.SelectedIndex == 4) //Ping
            {
                cbFcmTestSub   .Enabled = false ;
                cbFcmSubTestSub.Enabled = false ;
                tbFcmHemoSub   .Enabled = false ;
                tbFcmTestSub   .Enabled = false ;
            }

        }

        private void btTankActivate_Click(object sender, EventArgs e)
        {
            SEQ.CHA.iTime = Decimal.ToInt32(tbTankTime.Value) ;

            SEQ.CHA.iTankID = cbTankID.SelectedIndex;
            //     if (cbTankID.SelectedIndex == 0) SEQ.CHA.Tank = SEQ.CHA.TankCP1  ;
            //else if (cbTankID.SelectedIndex == 1) SEQ.CHA.Tank = SEQ.CHA.TankCP2  ;
            //else if (cbTankID.SelectedIndex == 2) SEQ.CHA.Tank = SEQ.CHA.TankCP3  ;
            //else if (cbTankID.SelectedIndex == 3) SEQ.CHA.Tank = SEQ.CHA.TankCSF  ;
            //else if (cbTankID.SelectedIndex == 4) SEQ.CHA.Tank = SEQ.CHA.TankCSR  ;
            //else if (cbTankID.SelectedIndex == 5) SEQ.CHA.Tank = SEQ.CHA.TankSULF ;
            //else if (cbTankID.SelectedIndex == 6) SEQ.CHA.Tank = SEQ.CHA.TankFB   ;
            //else if (cbTankID.SelectedIndex == 7) SEQ.CHA.Tank = SEQ.CHA.Tank4DL  ;
            //else if (cbTankID.SelectedIndex == 8) SEQ.CHA.Tank = SEQ.CHA.TankRET  ;
            //else if (cbTankID.SelectedIndex == 9) SEQ.CHA.Tank = SEQ.CHA.TankNR   ;

            MM.SetManCycle(mc.CHA_CycleTimeSupply);

        }

        private void cbChamberID_SelectedIndexChanged(object sender, EventArgs e)
        {
            //셋업시에만 쓰고 위험해서 없앰.
            //SEQ.CHA.iManFillCmbID = cbChamberID.SelectedIndex ; 
        }


        private void btSpectro_Click(object sender, EventArgs e)
        {
            

            SEQ.Spec.Aquire();
            SEQ.Spec.CalPeakVal();

            lbSpectro.Text = SEQ.Spec.GetCalHemoglobin(OM.DevInfo.iCmb2Blk , OM.DevInfo.dCmb2SpecAngle).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SEQ.CHA.InitValve();
        }

        private void btBarcode_Click(object sender, EventArgs e)
        {
            SEQ.Bar.Read();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SEQ.Bar.Stop();
        }
    }
}
