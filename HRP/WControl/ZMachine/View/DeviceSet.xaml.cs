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
using System.Reflection;
using COMMON;
using static Machine.View.MotorMove;
using Machine.View.DataMan;

namespace Machine.View
{
    /// <summary>
    /// DeviceSet.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeviceSet : UserControl
    {
        private const string sFormText = "Form DeviceSet ";

        public DeviceSet()
        {
            InitializeComponent();
            MainWindow parant;

            //Machine.View.Oper.DM.SetMask(ri.LODR, Machine.View.Oper.DM.ARAY[ri.PLDR]);


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Output
            IO_0.SetConfig(yi.ETC_058    , yi.ETC_058    .ToString(), null);
            IO_0.SetConfig(yi.ETC_LightOn, yi.ETC_LightOn.ToString(), null);

            //Jog
            JogMOVE_X.SetConfig(mi.TBL_XMove, SML.MOTION_DIR.RightLeft, null );
            JogMOVE_Y.SetConfig(mi.TBL_YMove, SML.MOTION_DIR.BwdFwd   , null );

            //Input
            IO_1.SetConfig(xi.ETC_91     , xi.ETC_DoorRt .ToString(), null);

            //Cylinder
            Cyl_0.SetConfig(ci.LODR_GrpRtrCwCCw,ML.CL_GetName(ci.LODR_GrpRtrCwCCw),ML.CL_GetDirType(ci.LODR_GrpRtrCwCCw), null);
            Cyl_1.SetConfig(ci.LODR_GuideOpCl  ,ML.CL_GetName(ci.LODR_GuideOpCl  ),ML.CL_GetDirType(ci.LODR_GuideOpCl  ), null);

            PM.Init();

            PstnDisp();
            //PM.Save("NONE");

            PM.Load(OM.GetCrntDev());
            PM.UpdatePstn(true);
            UpdateDevInfo(true);

            MOVE_X.SetConfig(mi.TBL_XMove, null);
            MOVE_Y.SetConfig(mi.TBL_YMove, null);

            //frame_Copy3.Content = mpv;
        }


        public void PstnDisp()
        {
            PM.SetPara((int)mi.TBL_XMove, (int)pv.LODR_YIndxWait   , "Wait   ", true, true, false);
            PM.SetPara((int)mi.TBL_XMove, (int)pv.LODR_YIndxWork   , "Work   ", true, true, false);

            PM.SetPara((int)mi.TBL_YMove, (int)pv.LODR_XPshrWait   , "Wait   ", true, true, false);
            PM.SetPara((int)mi.TBL_YMove, (int)pv.LODR_XPshrWorkStt, "WorkStt", true, true, false);
            PM.SetPara((int)mi.TBL_YMove, (int)pv.LODR_XPshrWorkEnd, "WorkEnd", true, true, false);
            PM.SetPara((int)mi.TBL_YMove, (int)pv.LODR_XPshrBackOfs, "BackOfs", true, true, false);
        }

        private void TcPart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PM.UpdatePstn(true);
            PM.Load(OM.GetCrntDev());
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(tbColCount , ref OM.DevInfo.iColCount);
                CConfig.ValToCon(tbRowCount , ref OM.DevInfo.iRowCount );
                CConfig.ValToCon(tbVacuumOn , ref OM.DevInfo.iVacuumOn );
                CConfig.ValToCon(tbVacuumOff, ref OM.DevInfo.iVacuumOff);
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                CConfig.ConToVal(tbColCount , ref OM.DevInfo.iColCount);
                CConfig.ConToVal(tbRowCount , ref OM.DevInfo.iRowCount );
                CConfig.ConToVal(tbVacuumOn , ref OM.DevInfo.iVacuumOn );
                CConfig.ConToVal(tbVacuumOff, ref OM.DevInfo.iVacuumOff);

                //Auto Log
                Type type = DevInfo.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < f.Length; i++)
                {
                    Trace(f[i].Name, f[i].GetValue(DevInfo).ToString(), f[i].GetValue(OM.DevInfo).ToString());
                }

                UpdateDevInfo(true);
            }
        }

        //--------------------------------------------------
        // Log
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
        // Log
        //--------------------------------------------------

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string sText = ((TextBlock)sender).Text;
            Log.TraceListView(sFormText + "Save Button Clicked", ForContext.Frm);

            if (!Log.ShowMessageModal("Confirm", "Are you Sure?")) return;

            if(tcMain.SelectedIndex == 0)
            {
                UpdateDevInfo(false);
                OM.SaveJobFile(OM.GetCrntDev());
            }
            else
            {
                MOVE_X.Update();

                PM.UpdatePstn(false);
                PM.Save(OM.GetCrntDev());
                PM.UpdatePstn(true);
            }


            ////Loader.
            DM.SetMaxColRow(ri.LODR, OM.DevInfo.iColCount, OM.DevInfo.iRowCount);
            DM.SetMaxColRow(ri.PLDR, OM.DevInfo.iColCount, OM.DevInfo.iRowCount);
            //

            //ArrayPos.TPara PosPara ;//= new ArrayPos.TPara();
            //PosPara.dColGrGap  = OM.DevInfo.dColGrGap  ;            
            //if(!OM .StripPos.SetPara(PosPara))
            //{
            //    Log.ShowMessage("Strip Position Err" , OM .StripPos.Error);
            //}

            //InvalidateVisual();
        }

        private void JogMOVE_X_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
