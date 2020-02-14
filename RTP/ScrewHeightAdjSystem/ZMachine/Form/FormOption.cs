using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using System.IO;
using System.Reflection;
using SML;
using System.Collections.Generic;

namespace Machine
{
    public partial class FormOption : Form
    {
        //FormMain FrmMain;
        private const string sFormText = "Form Option ";
        public int iPreWorkMode  = 0;
        public int iCrntWorkMode = 0;

        public FormOption(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            UpdateComOptn(true);
            UpdateEqpOptn(true);

            OM.LoadCmnOptn();

            //Scable Setting
            this.Dock = DockStyle.Fill;
            int  _iWidth  = _pnBase.Width;
            int  _iHeight = _pnBase.Height;
            
            const int  iWidth  = 1280;
            const int  iHeight = 863;

            float widthRatio  = _iWidth   / (float)iWidth;// this.ClientSize.Width;//1280f;
            float heightRatio = _iHeight  / (float)iHeight;//.ClientSize.Height; //863f ;

            SizeF scale = new SizeF(widthRatio, heightRatio);
            //this.Scale(scale);

            foreach (Control control in this.Controls)
            {
                control.Scale(scale); 
                //control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            //Check Running Status.
            if (SEQ._bRun) 
            {
                Log.ShowMessage("Warning", "Can't Save during Autorunning!");
                return;
            }

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
            ((Button)sender).Enabled = false;
            
            UpdateComOptn(false);
            OM.SaveCmnOptn();

            UpdateEqpOptn(false);
            OM.SaveEqpOptn();
            
            //비전 결과 색 이름 표기
            //SetDisp(ri.RLT1);SetDisp(ri.RLT2);SetDisp(ri.RLT3);
            //SetDisp(ri.WRK1);SetDisp(ri.WRK2);SetDisp(ri.WRK3);
            //SetDisp(ri.PSTB);
            //SetDisp(ri.SPC );
            
            ((Button)sender).Enabled = true;
        } 

                    
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(cbLoadingStop  , ref OM.CmnOptn.bLoadStop     );
                CConfig.ValToCon(cbIgnrDoor     , ref OM.CmnOptn.bIgnrDoor     );
                CConfig.ValToCon(tbFindBoltWork1, ref OM.CmnOptn.dFindBoltWork1);
                CConfig.ValToCon(tbFindBoltWork2, ref OM.CmnOptn.dFindBoltWork2);
                CConfig.ValToCon(tbFindBoltWork3, ref OM.CmnOptn.dFindBoltWork3);

            }
            else 
            {
                OM.CCmnOptn PreCmnOptn = OM.CmnOptn;
                CConfig.ConToVal(cbLoadingStop  , ref OM.CmnOptn.bLoadStop     );
                CConfig.ConToVal(cbIgnrDoor     , ref OM.CmnOptn.bIgnrDoor     );
                CConfig.ConToVal(tbFindBoltWork1, ref OM.CmnOptn.dFindBoltWork1);
                CConfig.ConToVal(tbFindBoltWork2, ref OM.CmnOptn.dFindBoltWork2);
                CConfig.ConToVal(tbFindBoltWork3, ref OM.CmnOptn.dFindBoltWork3);

                //Auto Log
                Type type = PreCmnOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++)
                {
                    if(f[i].GetValue(PreCmnOptn) != f[i].GetValue(OM.CmnOptn))Trace(f[i].Name + " Changed", f[i].GetValue(PreCmnOptn).ToString() , f[i].GetValue(OM.CmnOptn).ToString());
                    else                                                      Trace(f[i].Name             , f[i].GetValue(PreCmnOptn).ToString() , f[i].GetValue(OM.CmnOptn).ToString());
                }

                UpdateComOptn(true);
            }
        }


        public void UpdateEqpOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(tbEquipName  , ref OM.EqpOptn.sEquipName     );
                CConfig.ValToCon(tbEquipSerial, ref OM.EqpOptn.sEquipSerial   );
            }
            else 
            {
                OM.CEqpOptn EqpOptn = OM.EqpOptn;

                CConfig.ConToVal(tbEquipName  , ref OM.EqpOptn.sEquipName     );
                CConfig.ConToVal(tbEquipSerial, ref OM.EqpOptn.sEquipSerial   );

                //Auto Log
                Type type = EqpOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(EqpOptn).ToString(), f[i].GetValue(OM.EqpOptn).ToString());
                }

                UpdateComOptn(true);
            }
        }

        private void FormOption_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void FormOption_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = Visible;
            if (Visible)
            {
                UpdateComOptn(true);
                UpdateEqpOptn(true);
            }
        }

        private void Trace(string sText, string _s1, string _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1 + " -> " + _s2,ForContext.Dev);
        }
        private void Trace(string sText, int _s1, int _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }
        private void Trace(string sText, double _s1, double _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }
        private void Trace(string sText, bool _s1, bool _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {

        }
    }

          //  public bool   bLoadStop           ; //로더 구간 작업 대기
          //  public bool   bIgnrDoor           ; //도어 스킵.
          //
          //  public int    iShakeCnt           ; //쉐이크 카운트.
          //  public bool   bAutoQc             ; //오토큐씨 적용여부.
          //  public int    iQcStartHour        ; //오토큐씨 시작시간.
          //  public int    iQcStartMin         ; //오토큐씨 시작분.
          //  public int    iUnFreezeMin        ; //해동시간 분.
          //
          //
          //  public int    iBloodChamberVol    ; //챔버에 넣는 피의 양.
          //  public int    iBloodChamberSpd    ; //챔버에 넣는 피공급 스피드. 수치가 아니고 인덱스임 1~40번까지 있고 해당 스피드 는 헤밀톤 클래스 참조.
          //
          //  //챔버1 용량.
          //  public int    iCmb1Cp2Time        ;
          ////public int    iCmb1TankTime       ;
          ////public int    iCmb1SylnPos        ;
          ////public int    iCmb1SylSpdCode     ;
          //  public int    iCmb1Cp3Time        ;
          //
          //  public int    iCmb1DCSylPos       ; //DC에 들어가는 CP2 실린지.
          //  public int    iCmb1DCSylSpdCode   ;
          //
          //  //챔버2 용량.
          //  public int    iCmb2Cp2Time        ;
          //  public int    iCmb2TankTime       ;
          ////public int    iCmb2SylnPos        ;
          ////public int    iCmb2SylSpdCode     ;
          //  public int    iCmb2Cp3Time        ;
          //
          //  //챔버3 용량.
          ////public int    iCmb3Cp2Time        ;
          //  public int    iCmb3TankTime       ;
          //  public int    iCmb3SylnPos        ;
          //  public int    iCmb3SylSpdCode     ;
          //  public int    iCmb3Cp3Time        ;
          //
          //  public int    iCmb3FCMSylPos      ; //FCM에 들어가는 실린지.
          //  public int    iCmb3FCMSylSpdCode  ;
          //
          //
          //  //챔버4 용량.
          ////public int    iCmb4Cp2Time        ;
          //  public int    iCmb4TankTime       ;
          //  public int    iCmb4SylnPos        ;
          //  public int    iCmb4SylSpdCode     ;
          //  public int    iCmb4Cp3Time        ;
          //
          //  public int    iCmb4FCMSylPos      ; //FCM에 들어가는 실린지.
          //  public int    iCmb4FCMSylSpdCode  ;
          //
          //  //챔버5 용량.
          ////public int    iCmb5Cp2Time        ;
          //  public int    iCmb5TankTime       ;
          //  public int    iCmb5SylnPos        ;
          //  public int    iCmb5SylSpdCode     ;
          //  public int    iCmb5Cp3Time        ;
          //
          //  public int    iCmb5FCMSylPos      ; //FCM에 들어가는 실린지.
          //  public int    iCmb5FCMSylSpdCode  ;
          //
          //  //챔버6 용량.
          ////public int    iCmb6Cp2Time        ;
          //  public int    iCmb6TankTime       ;
          //  public int    iCmb6SylnPos        ;
          //  public int    iCmb6SylSpdCode     ;
          //  public int    iCmb6Cp3Time        ;
          //
          //  public int    iCmb6FCMSylPos      ; //FCM에 들어가는 실린지.
          //  public int    iCmb6FCMSylSpdCode  ;
}
