using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormVersion : Form
    {
        public FormVersion()
        {
            InitializeComponent();
            SetVer();
        }

        private string GetVer(string _sName)
        {
            //Version vVer ;
            string sVer ;
            string sName = _sName;
            try { 
                //vVer = System.Reflection.AssemblyName.GetAssemblyName(sName).Version ;
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(sName);
                sVer = fvi.FileVersion ;
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message ;
            }
            return sVer;
        }

        private void SetVer()
        {
            string sVer;
            string sText = tbVer.Text;
            tbVer.Clear();

            List<string> lst = new List<string>();

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo d = new DirectoryInfo(sExeFolder + "\\Dll\\");       //Assuming Test is your Folder
            
            FileInfo[] Files = d.GetFiles();
            foreach(FileInfo file in Files )
            {
                string sExtension = file.Extension;
                if(sExtension == ".dll") lst.Add(file.FullName);
            }

            tbVer.AppendText("All Version \n" );
            sVer = Application.ProductVersion + "_" + Assembly.GetEntryAssembly().Location + "\r\n";
            tbVer.AppendText(sVer);
            Log.Trace(sVer);

            string sTest = Assembly.GetEntryAssembly().FullName;

            for(int i=0; i<lst.Count; i++)
            {
                sVer = GetVer(lst[i]) + "_" + lst[i] + "\r\n";
                tbVer.AppendText(sVer);
                Log.Trace(sVer);
            }

            tbVer.AppendText("\n" );

            tbVer.Text += sText;

        }

        private void tbVer_TextChanged(object sender, EventArgs e)
        {

        }

        /*
            All Version 
            1.0.0.1_D:\Works\HVI-540RI\Bin\Control.exe
            4, 0, 3, 3_D:\Works\HVI-540RI\Bin\Dll\AXL.dll
            1.0.0.1_D:\Works\HVI-540RI\Bin\Dll\Common.dll
            4, 0, 1, 1_D:\Works\HVI-540RI\Bin\Dll\EzBasicAxl.dll
            1.0.0.1_D:\Works\HVI-540RI\Bin\Dll\MotionAXL.dll
            1.0.0.1_D:\Works\HVI-540RI\Bin\Dll\MotionInterface.dll
            1.0.0.1_D:\Works\HVI-540RI\Bin\Dll\MotionNMC2.dll
            1.0.0.1_D:\Works\HVI-540RI\Bin\Dll\SML.dll
            
            Control Version
            1.0.0.0
            Added FormVer
            
            SML Version
            1.0.0.0
         */
    }
}
