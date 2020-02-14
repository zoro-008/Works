using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Machine
{
    public partial class FormVer : Form
    {
        public FormVer()
        {
            InitializeComponent();
            
            SetVer();
            SetUpdate();
        }

        private void richTextBox1_VisibleChanged(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen ;
        }

        private void SetVer()
        {
            string sName ;
            Version vVer ;
            string sVer ;

            sName = "SCDll.dll" ;
            try { 
                vVer = System.Reflection.AssemblyName.GetAssemblyName(sName).Version ;
                sVer = vVer.ToString();
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message ;
            }
            tbVer.AppendText(sName + "(ver:" + sVer +")\r\n" );


            sName = "MotionInterface.dll" ;
            try { 
                vVer = System.Reflection.AssemblyName.GetAssemblyName(sName).Version ;
                sVer = vVer.ToString();
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message ;
            }
            tbVer.AppendText(sName + "(ver:" + sVer +")\r\n" );


            sName = "AjinAXL.dll" ;
            try { 
                vVer = System.Reflection.AssemblyName.GetAssemblyName(sName).Version ;
                sVer = vVer.ToString();
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message ;
            }
            tbVer.AppendText(sName + "(ver:" + sVer +")\r\n" );


            sName = "MotionPaix.dll" ;
            try { 
                vVer = System.Reflection.AssemblyName.GetAssemblyName(sName).Version ;
                sVer = vVer.ToString();
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message ;
            }
            tbVer.AppendText(sName + "(ver:" + sVer +")\r\n" );


            sName = "SML2.dll" ;
            try { 
                vVer = System.Reflection.AssemblyName.GetAssemblyName(sName).Version ;
                sVer = vVer.ToString();
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message ;
            }
            tbVer.AppendText(sName + "(ver:" + sVer +")\r\n" );


            sName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            try { 
                sVer = System.Windows.Forms.Application.ProductVersion;
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message ;
            }
            tbVer.AppendText(sName + "(ver:" + sVer +")\r\n" );

            tbVer.AppendText("-------------------------------------\r\n\r\n" );
        }

        private void SetUpdate()
        {
            tbVer.AppendText("2017_11_03_SUN\r\n");
            tbVer.AppendText("    SML2.dll(ver 1.2.1) - Added all axis speed ratio\r\n");
            tbVer.AppendText("    OSW-10a(ver 1.0.1) - Added FormVer\r\n");
            tbVer.AppendText("                         Added Index Overload Err when Index BarcodeCycle & Index Out Cycle\r\n");
            tbVer.AppendText("                         Added Index Gripper Tray detect Sensor Error at the every index cycle start\r\n");
            tbVer.AppendText("                         Change error name from 'CycleTimeout' to 'tray check sensor not detected tray' when barcode place failed on tray.\r\n");
            //
            tbVer.AppendText("    OSW-10a(ver 1.0.2) - Budle Run function added \r\n");
            tbVer.AppendText("2017_11_30_SUN\r\n");
            tbVer.AppendText("                        Send VIT File to the Oracle when Manaul LotEnded  \r\n");
            //
            tbVer.AppendText("2018_01_18_SUN\r\n");
            tbVer.AppendText("    OSW-10a(ver 1.0.3) - Added DMC2 String Compare Charicter Count Limit \r\n");

            tbVer.AppendText("2018_01_25_SUN\r\n");
            tbVer.AppendText("    OSW-10a(ver 1.0.4) - Seperated LogOn ID and LotOpen ID \r\n");
            tbVer.AppendText("                         showing messege box if there is empty editbox in lotopen form \r\n");

            tbVer.AppendText("2018_02_09_SUN\r\n");
            tbVer.AppendText("    OSW-10a(ver 1.0.5) - Added Barcode Attach Skip option  \r\n");

            tbVer.AppendText("2018_02_21_SUN\r\n");
            tbVer.AppendText("    OSW-10a(ver 1.0.6) - Added Log about barcode reading 111  \r\n");
            tbVer.AppendText("                         Check Vision Result File Delete \r\n");

            tbVer.AppendText("2018_03_05_SUN\r\n");
            tbVer.AppendText("    OSW-10a(ver 1.0.7) - <List Item 3> Changed 'Open Lot' to 'Start Lot'  \r\n");
            tbVer.AppendText("                         <List Item 2> Auto All Homing when lot start \r\n");
            tbVer.AppendText("                         <List Item 5> Added tray gripper detect sensor error on first vision position  \r\n");

            tbVer.AppendText("2018_03_05_SUN\r\n");
            tbVer.AppendText("    OSW-10a(ver 1.0.8) - <List Item 10> Added permition of DMC2 char count over option   \r\n");

            tbVer.AppendText("2018_08_08_SUN\r\n");
            tbVer.AppendText("    OSW-10a(ver 1.0.9) - Added DMC1Grouping Ignore Option   \r\n");

        }
    }
}
