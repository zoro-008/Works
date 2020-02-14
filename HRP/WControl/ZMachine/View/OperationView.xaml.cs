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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Machine.View
{
    /// <summary>
    /// OperationView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OperationView : UserControl
    {
        OutputView   ov ;
        MotorView    mv ;
        InputView    iv ;
        CylinderView cv ;
        MotorPosView mpv;
        public OperationView()
        {
            InitializeComponent();

            ov = new OutputView();
            mv = new MotorView();
            iv = new InputView();
            cv = new CylinderView();

            mpv = new MotorPosView();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //ov = new OutputView();
            ov.SetConfig(yi.ETC_058,yi.ETC_058.ToString(), frame);

            
            //mv = new MotorView();
            mv.SetConfig(mi.LODR_XPckr,SML.MOTION_DIR.CwCcw,frame_Copy);

            //iv = new InputView();
            iv.SetConfig(xi.ETC_91,xi.ETC_DoorRt.ToString(), frame_Copy1);

            
            

            //cylinderview = new CylinderView();
            cylinderview.SetConfig(ci.LODR_GrpRtrCwCCw,"TEST cylinderview",ML.CL_GetDirType(ci.LODR_GrpRtrCwCCw), null);

            //cv = new CylinderView();
            cv.SetConfig(ci.LODR_GrpRtrCwCCw,ML.CL_GetName(ci.LODR_GrpRtrCwCCw),ML.CL_GetDirType(ci.LODR_GrpRtrCwCCw), frame_Copy2);

            //mpv

            PM.Init();
            PM.Para[(int)mi.LODR_YIndx].Add(new PositionPara() { 
                sName     = "aswedf" ,
                uPstnNo   = 0 ,
                dPos      = 123.1  ,
                dMin      = 12.1   ,
                dMax      = 12.2   ,
                iSpeed    = 24     ,
                bGo       = false  ,
                bInput    = false  ,
                bCommon   = false
            });
            PM.Para[(int)mi.LODR_YIndx].Add(new PositionPara() { 
                sName     = "as213df" ,
                uPstnNo   = 1 ,
                dPos      = 123.1  ,
                dMin      = 12.1   ,
                dMax      = 12.2   ,
                iSpeed    = 24     ,
                bGo       = false  ,
                bInput    = false  ,
                bCommon   = false
            });
            PM.Save("NONE");
            PM.UpdatePstn(true);
            mpv.SetConfig(0,frame_Copy3); //SetConfig(0);
            //frame_Copy3.Content = mpv;
            
        }
    }



}
