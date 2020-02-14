using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using SML;

namespace Machine
{
    public interface IPaintable
    {
        void IPaint(Graphics _g);
        
    }

    public interface IClickable 
    {
        bool IClick(int _iX , int _iY);
    }

    public interface IToolTip
    {
        string IMove(int _iX , int _iY);
    }

    public class ValveDisplay
    {
        //Panel Zero Point
        Point ZeroPoint = new Point{ X = 0, Y = 0};
        
        static object BackImg = null; //Panel BackgroundImage 저장하는 놈
        static Panel  pnManual      ; //Update 할때 필요

        public IPaintable[] objs = new IPaintable[96];
        public ValveDisplay(Panel _pnBase)
        {
            pnManual = _pnBase;

            //3Way Valve
            objs[ 0] = new Valve3Way   (141,  69, yi.TankCP1_VcAr , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP1_VcAr ).iAdd) +"_A"); 
            objs[ 1] = new Valve3Way   (191,  69, yi.TankCP2_VcAr , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP2_VcAr ).iAdd) +"_A"); 
            objs[ 2] = new Valve3Way   (241,  69, yi.TankCP3_VcAr , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP3_VcAr ).iAdd) +"_A"); 
            objs[ 3] = new Valve3Way   (311,  69, yi.TankCSF_VcAr , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCSF_VcAr ).iAdd) +"_A"); 
            objs[ 4] = new Valve3Way   (361,  69, yi.TankCSR_VcAr , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCSR_VcAr ).iAdd) +"_A"); 
            objs[ 5] = new Valve3Way   (431,  69, yi.TankSULF_VcAr, string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankSULF_VcAr).iAdd) +"_A"); 
            objs[ 6] = new Valve3Way   (491,  69, yi.TankFB_VcAr  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankFB_VcAr  ).iAdd) +"_A"); 
            objs[ 7] = new Valve3Way   (541,  69, yi.Tank4DL_VcAr , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Tank4DL_VcAr ).iAdd) +"_A"); 
            objs[ 8] = new Valve3Way   (601,  69, yi.TankRET_VcAr , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankRET_VcAr ).iAdd) +"_A"); 
            objs[ 9] = new Valve3Way   (661,  69, yi.TankNR_VcAr  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankNR_VcAr  ).iAdd) +"_A"); 
            objs[10] = new Valve3Way   ( 50, 352, yi.Cmb1Air_InVt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb1Air_InVt ).iAdd) +"_D"); 
            objs[11] = new Valve3Way   ( 80, 352, yi.Cmb2Air_InVt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb2Air_InVt ).iAdd) +"_D"); 
            objs[12] = new Valve3Way   (110, 352, yi.Cmb3Air_InVt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb3Air_InVt ).iAdd) +"_D"); 
            objs[13] = new Valve3Way   (140, 352, yi.Cmb4Air_InVt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb4Air_InVt ).iAdd) +"_D"); 
            objs[14] = new Valve3Way   (170, 352, yi.Cmb5Air_InVt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb5Air_InVt ).iAdd) +"_D"); 
            objs[15] = new Valve3Way   (200, 352, yi.Cmb6Air_InVt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb6Air_InVt ).iAdd) +"_D"); 
            objs[16] = new Valve3Way   (291, 502, yi.Cmb1Out_VcOt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb1Out_VcOt ).iAdd) +"_E"); 
            objs[17] = new Valve3Way   (341, 502, yi.Cmb2Out_VcOt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb2Out_VcOt ).iAdd) +"_E"); 
            objs[18] = new Valve3Way   (391, 502, yi.Cmb3Out_VcOt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb3Out_VcOt ).iAdd) +"_E"); 
            objs[19] = new Valve3Way   (441, 502, yi.Cmb4Out_VcOt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb4Out_VcOt ).iAdd) +"_E"); 
            objs[20] = new Valve3Way   (491, 502, yi.Cmb5Out_VcOt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb5Out_VcOt ).iAdd) +"_E"); 
            objs[21] = new Valve3Way   (541, 502, yi.Cmb6Out_VcOt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb6Out_VcOt ).iAdd) +"_E"); 
            objs[22] = new Valve3Way   ( 53, 462, yi.Dc1Air_InVt  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Dc1Air_InVt  ).iAdd) +"_D"); 
            objs[23] = new Valve3Way   ( 82, 462, yi.Dc2Air_InVt  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Dc2Air_InVt  ).iAdd) +"_D"); 
            objs[24] = new Valve3Way   (657, 521, yi.WastAir_InVc , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.WastAir_InVc ).iAdd) +"_F"); 

            //2Way Valve
            objs[25] = new Valve2Way   (115, 112, yi.TankCP1_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP1_InSt ).iAdd) +"_B");
            objs[26] = new Valve2Way   (165, 112, yi.TankCP2_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP2_InSt ).iAdd) +"_B");
            objs[27] = new Valve2Way   (215, 112, yi.TankCP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP3_InSt ).iAdd) +"_B");
            objs[28] = new Valve2Way   (285, 112, yi.TankCSF_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCSF_InSt ).iAdd) +"_B");
            objs[29] = new Valve2Way   (335, 112, yi.TankCSR_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCSR_InSt ).iAdd) +"_B");
            objs[30] = new Valve2Way   (405, 112, yi.TankSULF_InSt, string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankSULF_InSt).iAdd) +"_B");
            objs[31] = new Valve2Way   (465, 112, yi.TankFB_InSt  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankFB_InSt  ).iAdd) +"_B");
            objs[32] = new Valve2Way   (555, 112, yi.Tank4DL_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Tank4DL_InSt ).iAdd) +"_B");
            objs[33] = new Valve2Way   (615, 112, yi.TankRET_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankRET_InSt ).iAdd) +"_B");
            objs[34] = new Valve2Way   (675, 112, yi.TankNR_InSt  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankNR_InSt  ).iAdd) +"_B");
            objs[35] = new Valve2Way   (115, 182, yi.TankCP1_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP1_OtSt ).iAdd) +"_C");
            objs[36] = new Valve2Way   (165, 182, yi.TankCP2_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP2_OtSt ).iAdd) +"_C");
            objs[37] = new Valve2Way   (215, 182, yi.TankCP3_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCP3_OtSt ).iAdd) +"_C");
            objs[38] = new Valve2Way   (285, 182, yi.TankCSF_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCSF_OtSt ).iAdd) +"_C");
            objs[39] = new Valve2Way   (335, 182, yi.TankCSR_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankCSR_OtSt ).iAdd) +"_C");
            objs[40] = new Valve2Way   (405, 182, yi.TankSULF_OtSt, string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankSULF_OtSt).iAdd) +"_C");
            objs[41] = new Valve2Way   (465, 182, yi.TankFB_OtSt  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankFB_OtSt  ).iAdd) +"_C");
            objs[42] = new Valve2Way   (535, 182, yi.Tank4DL_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Tank4DL_OtSt ).iAdd) +"_C");
            objs[43] = new Valve2Way   (595, 182, yi.TankRET_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankRET_OtSt ).iAdd) +"_C");
            objs[44] = new Valve2Way   (655, 182, yi.TankNR_OtSt  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.TankNR_OtSt  ).iAdd) +"_C");
            objs[45] = new Valve2Way   (298, 310, yi.Cmb2CP2_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb2CP2_InSt ).iAdd) +"_G");
            objs[46] = new Valve2Way   (245, 352, yi.ClenCP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.ClenCP3_InSt ).iAdd) +"_G");
            objs[47] = new Valve2Way   (275, 352, yi.Cmb1CP2_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb1CP2_InSt ).iAdd) +"_G");
            objs[48] = new Valve2Way   (295, 352, yi.Cmb1CP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb1CP3_InSt ).iAdd) +"_G");
            objs[49] = new Valve2Way   (325, 352, yi.Cmb2CP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb2CP3_InSt ).iAdd) +"_G");
            objs[50] = new Valve2Way   (375, 352, yi.Cmb3CP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb3CP3_InSt ).iAdd) +"_G");
            objs[51] = new Valve2Way   (425, 352, yi.Cmb4CP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb4CP3_InSt ).iAdd) +"_G");
            objs[52] = new Valve2Way   (475, 352, yi.Cmb5CP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb5CP3_InSt ).iAdd) +"_G");
            objs[53] = new Valve2Way   (525, 352, yi.Cmb6CP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb6CP3_InSt ).iAdd) +"_G");
            objs[54] = new Valve2Way   (208, 492, yi.NidlCP3_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.NidlCP3_InSt ).iAdd) +"_G");
            objs[55] = new Valve2Way   (246, 492, yi.ClenOut_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.ClenOut_OtSt ).iAdd) +"_K");
            objs[56] = new Valve2Way   (333, 542, yi.Cmb2Out_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb2Out_OtSt ).iAdd) +"_J");
            objs[57] = new Valve2Way   ( 58, 524, yi.Dcc5CP2_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Dcc5CP2_InSt ).iAdd) +"_I");
            objs[58] = new Valve2Way   ( 58, 554, yi.Dcc4CSR_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Dcc4CSR_InSt ).iAdd) +"_I");
            objs[59] = new Valve2Way   ( 58, 584, yi.Dcc3CSf_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Dcc3CSf_InSt ).iAdd) +"_I");
            objs[60] = new Valve2Way   ( 58, 614, yi.Dcc2Vcm_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Dcc2Vcm_InSt ).iAdd) +"_I");
            objs[61] = new Valve2Way   ( 58, 634, yi.Dcc1Vcm_InSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Dcc1Vcm_InSt ).iAdd) +"_I");
            objs[62] = new Valve2Way   (169, 633, yi.NidlOut_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.NidlOut_OtSt ).iAdd) +"_H");
            objs[63] = new Valve2Way   (337, 632, yi.HGBOut_OtSt  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.HGBOut_OtSt  ).iAdd) +"_H");
            objs[64] = new Valve2Way   (575, 602, yi.FCMOut_OtSt  , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.FCMOut_OtSt  ).iAdd) +"_H");
            objs[65] = new Valve2Way   (638, 602, yi.FCMWOut_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.FCMWOut_OtSt ).iAdd) +"_H");
            objs[66] = new Valve2Way   (690, 553, yi.WastOut_OtSt , string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.WastOut_OtSt ).iAdd) +"_H");

            //2Way Pinch Valve
            objs[67] = new Valve2Pinch (279, 541, yi.Cmb1Out_OtSt ,string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb1Out_OtSt ).iAdd) );
            objs[68] = new Valve2Pinch (379, 541, yi.Cmb3Out_OtSt ,string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb3Out_OtSt ).iAdd) );
            objs[69] = new Valve2Pinch (429, 541, yi.Cmb4Out_OtSt ,string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb4Out_OtSt ).iAdd) );
            objs[70] = new Valve2Pinch (479, 541, yi.Cmb5Out_OtSt ,string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb5Out_OtSt ).iAdd) );
            objs[71] = new Valve2Pinch (529, 541, yi.Cmb6Out_OtSt ,string.Format("Y{0:X3}", SM.DIO.GetOutputPara((int)yi.Cmb6Out_OtSt ).iAdd) );

            //Vacuum
            objs[72] = new ValveVacuum ( 14,  50, yi.ETC_MainPumpOff);
            objs[73] = new ValveVacuum (619, 519, yi.ETC_MainPumpOff);
            
            //Air
            objs[74] = new ValveAir    ( 14,  20, yi.ETC_MainPumpOff);
            objs[75] = new ValveAir    ( 14, 331, yi.ETC_MainPumpOff);
            objs[76] = new ValveAir    ( 14, 477, yi.ETC_MainPumpOff);
            objs[77] = new ValveAir    (650, 491, yi.ETC_MainPumpOff);

            //Sylinge
            objs[78] = new ValveSylinge(159, 471, PumpID.Blood);
            objs[79] = new ValveSylinge(293, 606, PumpID.DC   );
            objs[80] = new ValveSylinge(380, 606, PumpID.FCM  );
            objs[81] = new ValveSylinge(600, 456, PumpID.NR   );
            objs[82] = new ValveSylinge(620, 456, PumpID.RET  );
            objs[83] = new ValveSylinge(640, 456, PumpID.FDS  );

            //Tank
            objs[84] = new ValveTank   (114, 141, xi.CP_1NotFull    );
            objs[85] = new ValveTank   (164, 141, xi.CP_2NotFull    );
            objs[86] = new ValveTank   (214, 141, xi.CP_3NotFull    );
            objs[87] = new ValveTank   (284, 141, xi.CS_FNotFull    );
            objs[88] = new ValveTank   (334, 141, xi.CS_RNotFull    );
            objs[89] = new ValveTank   (404, 141, xi.SULFNotFull    );
            objs[90] = new ValveTank   (464, 141, xi.FBNotFull      );
            objs[91] = new ValveTank   (534, 141, xi.FDLNotFull     );
            objs[92] = new ValveTank   (594, 141, xi.RETNotFull     );
            objs[93] = new ValveTank   (654, 141, xi.NRNotFull      );
            objs[94] = new ValveTank   (604, 596, xi.FCMWasteNotFull);
            objs[95] = new ValveTank   (650, 550, xi.WasteNotFull   );
        }
        
        public void Paint(Graphics _g)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].IPaint(_g);
            }
        }

        public void Click(int _iX , int _iY)
        {
            // 여기서 포문으로 돌림.
            IClickable ClickObj ;
            for(int i = 0 ; i < objs.Length ; i++)
            {
                ClickObj = objs[i] as IClickable ;
                if(ClickObj == null) continue ;
                ClickObj.IClick(_iX , _iY);
            }
        }

        public string Move(int _iX , int _iY)
        {
            IToolTip ClickObj ;
            string sToolTip = "";
            for(int i = 0 ; i < objs.Length ; i++)
            {
                ClickObj = objs[i] as IToolTip ;
                if(ClickObj == null) continue ;
                sToolTip = ClickObj.IMove(_iX , _iY);
                if(sToolTip != "")return sToolTip ;
            }
            return "";
        }
    }

    public class Valve3Way : IPaintable, IClickable , IToolTip
    {
        int iOriX ;
        int iOriY ;
        string sCmt = "";

        yi iValve3WayOut;
        Rectangle Click3WayRange = new Rectangle();

        public Valve3Way(int _iOriX, int _iOriY, yi _iIO , string _sCmt)
        {
            iOriX = _iOriX;
            iOriY = _iOriY;
            sCmt = _sCmt ;
            iValve3WayOut = _iIO;
        }

        public void IPaint(Graphics _g) //인터페이스 상속
        {
            Rectangle Roi = Rectangle.Round(_g.ClipBounds) ;
            using (Pen RP = new Pen(Color.Red, 2))
            {
                Point[] Triangle1 = new Point[3];
                Triangle1[0].X = iOriX     ;
                Triangle1[0].Y = iOriY     ;
                Triangle1[1].X = iOriX + 11;
                Triangle1[1].Y = iOriY     ;
                Triangle1[2].X = iOriX + 6 ;
                Triangle1[2].Y = iOriY + 9 ;

                Point[] Triangle2 = new Point[3];
                Triangle2[0].X = iOriX - 4 ;
                Triangle2[0].Y = iOriY + 4 ;
                Triangle2[1].X = iOriX - 4 ;
                Triangle2[1].Y = iOriY + 15;
                Triangle2[2].X = iOriX + 4 ;
                Triangle2[2].Y = iOriY + 9 ;

                using (SolidBrush BrushLime = new SolidBrush(Color.Lime))
                using (SolidBrush BrushNone = new SolidBrush(Color.Transparent))
                using (SolidBrush BrushRed = new SolidBrush(Color.Red))
                {                    
                    bool bValue = ML.IO_GetY(iValve3WayOut);
                    if (bValue)//IO_GetY
                    {
                        _g.FillPolygon(BrushLime, Triangle1);
                        _g.FillPolygon(BrushNone, Triangle2);
                    }

                    else
                    {
                        _g.FillPolygon(BrushNone, Triangle1);
                        _g.FillPolygon(BrushRed , Triangle2);
                    }
                    
                }
            }
        }

        public bool IClick(int _iX , int _iY) //인터페이스 상속
        {
            
            Click3WayRange.X      = iOriX - 5;
            Click3WayRange.Y      = iOriY - 5;
            Click3WayRange.Width  = iOriX + 15;
            Click3WayRange.Height = iOriY + 18;

            if ((_iX > Click3WayRange.X && _iX < Click3WayRange.Width) && (_iY > Click3WayRange.Y && _iY < Click3WayRange.Height))
            {
                ML.IO_SetY(iValve3WayOut, !ML.IO_GetY(iValve3WayOut));
                return true;
            }
            return false ;
        }

        public string IMove(int _iX , int _iY)
        {
            Click3WayRange.X      = iOriX - 5;
            Click3WayRange.Y      = iOriY - 5;
            Click3WayRange.Width  = iOriX + 15;
            Click3WayRange.Height = iOriY + 18;

            if ((_iX > Click3WayRange.X && _iX < Click3WayRange.Width) && (_iY > Click3WayRange.Y && _iY < Click3WayRange.Height))
            {
                
                return sCmt;
            }
            return "" ;
        }
    }

    public class Valve2Way : IPaintable, IClickable, IToolTip
    {
        int iOriX ;
        int iOriY ;
        string sCmt ="";
        yi iValve2WayOut;
        Rectangle Click2WayRange = new Rectangle();

        public Valve2Way(int _iOriX, int _iOriY, yi _iIO , string _sCmt)
        {
            iOriX         = _iOriX;
            iOriY         = _iOriY;
            sCmt          = _sCmt ;
            iValve2WayOut = _iIO  ;
        }

        public void IPaint(Graphics _g)
        {
            Rectangle Roi = Rectangle.Round(_g.ClipBounds);
            using (Pen RP = new Pen(Color.Red, 2))
            {
                Point[] Triangle1 = new Point[3];
                Triangle1[0].X = iOriX     ;
                Triangle1[0].Y = iOriY     ;
                Triangle1[1].X = iOriX + 11;
                Triangle1[1].Y = iOriY     ;
                Triangle1[2].X = iOriX + 6 ;
                Triangle1[2].Y = iOriY + 9 ;

                using (SolidBrush BrushLime = new SolidBrush(Color.Lime))
                {
                    using (SolidBrush BrushNone = new SolidBrush(Color.Transparent))
                    {
                        if (ML.IO_GetY(iValve2WayOut))//IO_GetY
                        {
                            _g.FillPolygon(BrushLime, Triangle1);
                        }
                        else
                        {
                            _g.FillPolygon(BrushNone, Triangle1);
                        }
                    }
                }
            }
        }

        public bool IClick(int _iX, int _iY)
        {
            Click2WayRange.X      = iOriX - 5 ;
            Click2WayRange.Y      = iOriY - 5 ;
            Click2WayRange.Width  = iOriX + 15;
            Click2WayRange.Height = iOriY + 18;

            if ((_iX > Click2WayRange.X && _iX < Click2WayRange.Width) && (_iY > Click2WayRange.Y && _iY < Click2WayRange.Height))
            {
                ML.IO_SetY(iValve2WayOut, !ML.IO_GetY(iValve2WayOut));
                return true;
            }
            return false;
        }

        public string IMove(int _iX , int _iY)
        {
            Click2WayRange.X      = iOriX - 5 ;
            Click2WayRange.Y      = iOriY - 5 ;
            Click2WayRange.Width  = iOriX + 15;
            Click2WayRange.Height = iOriY + 18;

            if ((_iX > Click2WayRange.X && _iX < Click2WayRange.Width) && (_iY > Click2WayRange.Y && _iY < Click2WayRange.Height))
            {
              
                return sCmt;
            }
            return "";
        }
    }

    public class Valve2Pinch : IPaintable, IClickable, IToolTip
    {
        int iOriX ;
        int iOriY ;
        string sCmt = "";
        yi iValve2PinchOut;
        Rectangle Click2PinchRange = new Rectangle();

        public Valve2Pinch(int _iOriX, int _iOriY, yi _iIO , string _sCmt)
        {
            sCmt = _sCmt ;
            iOriX           = _iOriX ;
            iOriY           = _iOriY ;
            iValve2PinchOut = _iIO   ;
        }

        public void IPaint(Graphics _g)
        {
            Rectangle Roi = Rectangle.Round(_g.ClipBounds);
            using (Pen RP = new Pen(Color.Red, 2))
            {
                Point[] Triangle1 = new Point[3];
                Triangle1[0].X = iOriX     ;
                Triangle1[0].Y = iOriY     ;
                Triangle1[1].X = iOriX + 11;
                Triangle1[1].Y = iOriY     ;
                Triangle1[2].X = iOriX + 6 ;
                Triangle1[2].Y = iOriY + 9 ;

                using (SolidBrush BrushLime = new SolidBrush(Color.Lime))
                {
                    using (SolidBrush BrushNone = new SolidBrush(Color.Transparent))
                    {
                        if (ML.IO_GetY(iValve2PinchOut))//IO_GetY
                        {
                            _g.FillPolygon(BrushLime, Triangle1);
                        }
                        else
                        {
                            _g.FillPolygon(BrushNone, Triangle1);
                        }
                    }
                }
            }
        }

        public bool IClick(int _iX, int _iY)
        {
            Click2PinchRange.X      = iOriX - 5 ;
            Click2PinchRange.Y      = iOriY     ;
            Click2PinchRange.Width  = iOriX + 15;
            Click2PinchRange.Height = iOriY + 23;

            if ((_iX > Click2PinchRange.X && _iX < Click2PinchRange.Width) && (_iY > Click2PinchRange.Y && _iY < Click2PinchRange.Height))
            {
                ML.IO_SetY(iValve2PinchOut, !ML.IO_GetY(iValve2PinchOut));
                return true;
            }
            return false;
        }

        public string IMove(int _iX , int _iY)
        {
            Click2PinchRange.X      = iOriX - 5 ;
            Click2PinchRange.Y      = iOriY     ;
            Click2PinchRange.Width  = iOriX + 15;
            Click2PinchRange.Height = iOriY + 23;

            if ((_iX > Click2PinchRange.X && _iX < Click2PinchRange.Width) && (_iY > Click2PinchRange.Y && _iY < Click2PinchRange.Height))
            {
                return sCmt;
            }
            return "";
        }
    }

    public class ValveVacuum : IPaintable, IClickable
    {
        int iOriX ;
        int iOriY ;

        yi  iVacOff;
        Rectangle ClickVacuumRange = new Rectangle();
    
        public ValveVacuum(int _iOriX, int _iOriY, yi _iIO)
        {
            iOriX   = _iOriX ;
            iOriY   = _iOriY ;
            iVacOff = _iIO   ;
        }
    
        public void IPaint(Graphics _g)
        {
            Rectangle Roi = Rectangle.Round(_g.ClipBounds);
            using (Pen RP = new Pen(Color.Red, 2))
            {
                Rectangle CircleSize = new Rectangle();
                CircleSize.X      = iOriX;
                CircleSize.Y      = iOriY;
                CircleSize.Width  = 10;
                CircleSize.Height = 10;

                using (SolidBrush BrushLime = new SolidBrush(Color.Lime))
                {
                    using (SolidBrush BrushNone = new SolidBrush(Color.Transparent))
                    {
                        if (!ML.IO_GetY(iVacOff))//IO_GetY
                        {
                            _g.FillEllipse(BrushLime, CircleSize);
                        }
                        else
                        {
                            _g.FillEllipse(BrushNone, CircleSize);
                        }
                    }
                }
            }
        }
    
        public bool IClick(int _iX, int _iY)
        {
            ClickVacuumRange.X      = iOriX - 5 ;
            ClickVacuumRange.Y      = iOriY     ;
            ClickVacuumRange.Width  = iOriX + 15;
            ClickVacuumRange.Height = iOriY + 23;
    
            if ((_iX > ClickVacuumRange.X && _iX < ClickVacuumRange.Width) && (_iY > ClickVacuumRange.Y && _iY < ClickVacuumRange.Height))
            {
                ML.IO_SetY(iVacOff, !ML.IO_GetY(iVacOff));
                return true;
            }
            return false;
        }
    }

    public class ValveAir : IPaintable, IClickable
    {
        int iOriX ;
        int iOriY ;
        
        yi iAirOff;
        Rectangle ClickAirRange = new Rectangle();

        public ValveAir(int _iOriX, int _iOriY, yi _iIO)
        {
            iOriX    = _iOriX;
            iOriY    = _iOriY;
            iAirOff  = _iIO  ;
        }

        public void IPaint(Graphics _g)
        {
            Rectangle Roi = Rectangle.Round(_g.ClipBounds);
            using (Pen RP = new Pen(Color.Red, 2))
            {
                Rectangle CircleSize = new Rectangle();
                CircleSize.X      = iOriX;
                CircleSize.Y      = iOriY;
                CircleSize.Width  = 10   ;
                CircleSize.Height = 10   ;

                using (SolidBrush BrushLime = new SolidBrush(Color.Lime))
                {
                    using (SolidBrush BrushNone = new SolidBrush(Color.Transparent))
                    {
                        if (!ML.IO_GetY(iAirOff))//IO_GetY
                        {
                            _g.FillEllipse(BrushLime, CircleSize);
                        }
                        else
                        {
                            _g.FillEllipse(BrushNone, CircleSize);
                        }
                    }
                }
            }
        }

        public bool IClick(int _iX, int _iY)
        {
            ClickAirRange.X      = iOriX - 5 ;
            ClickAirRange.Y      = iOriY     ;
            ClickAirRange.Width  = iOriX + 15;
            ClickAirRange.Height = iOriY + 23;

            if ((_iX > ClickAirRange.X && _iX < ClickAirRange.Width) && (_iY > ClickAirRange.Y && _iY < ClickAirRange.Height))
            {
                ML.IO_SetY(iAirOff, !ML.IO_GetY(iAirOff));
                return true;
            }
            return false;
        }
    }

    public class ValveSylinge : IPaintable
    {
        int iOriX ;
        int iOriY ;
        
        PumpID iSylPumpId;

        public ValveSylinge(int _iOriX, int _iOriY, PumpID _iPumpId)
        {
            iOriX      = _iOriX  ;
            iOriY      = _iOriY  ;
            iSylPumpId = _iPumpId;
        }

        public void IPaint(Graphics _g)
        {
            Rectangle Roi = Rectangle.Round(_g.ClipBounds);
            using (Pen RP = new Pen(Color.Red, 2))
            {
                Rectangle CircleSize = new Rectangle();
                CircleSize.X      = iOriX;
                CircleSize.Y      = iOriY;
                CircleSize.Width  = 10   ;
                CircleSize.Height = 10   ;

                using (SolidBrush BrushLime = new SolidBrush(Color.Lime))
                {
                    using (SolidBrush BrushNone = new SolidBrush(Color.Red))//Transparent))
                    {
                        if (SEQ.SyringePump.GetBusy((int)iSylPumpId))
                        {
                            _g.FillEllipse(BrushLime, CircleSize);
                        }
                        else
                        {
                            _g.FillEllipse(BrushNone, CircleSize);
                        }
                    }
                }
            }
        }
    }

    public class ValveTank : IPaintable
    {
        int iOriX ;
        int iOriY ;
        
        xi iTankOut;

        public ValveTank(int _iOriX, int _iOriY, xi _iIO)
        {
            iOriX    = _iOriX ;
            iOriY    = _iOriY ;
            iTankOut = _iIO   ;
        }

        public void IPaint(Graphics _g)
        {
            Rectangle Roi = Rectangle.Round(_g.ClipBounds);
            using (Pen RP = new Pen(Color.Red, 2))
            {
                Rectangle CircleSize = new Rectangle();
                CircleSize.X      = iOriX;
                CircleSize.Y      = iOriY;
                CircleSize.Width  = 10   ;
                CircleSize.Height = 10   ;

                using (SolidBrush BrushLime = new SolidBrush(Color.Lime))
                {
                    using (SolidBrush BrushNone = new SolidBrush(Color.Transparent))
                    {
                        if (!ML.IO_GetX(iTankOut))
                        {
                            _g.FillEllipse(BrushLime, CircleSize);
                        }
                        else
                        {
                            _g.FillEllipse(BrushNone, CircleSize); 
                        }
                    }
                }
            }
        }
    }

    
}
