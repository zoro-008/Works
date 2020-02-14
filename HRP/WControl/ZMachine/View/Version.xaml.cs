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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using COMMON;
using SML;

namespace Machine.View
{
    /// <summary>
    /// Version.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Version : Window
    {
        public Version()
        {
            InitializeComponent();
            SetVer();
        }

        private string GetVer(string _sName)
        {
            //Version vVer ;
            string sVer;
            string sName = _sName;
            try
            {
                //vVer = System.Reflection.AssemblyName.GetAssemblyName(sName).Version ;
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(sName);
                sVer = fvi.FileVersion;
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message;
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

            FileInfo[] Files = d.GetFiles("*",SearchOption.AllDirectories);
            foreach (FileInfo file in Files)
            {
                string sExtension = file.Extension;
                if (sExtension == ".dll") lst.Add(file.FullName);
            }

            tbVer.AppendText("All Version \n");
            //sVer = Application.ProductVersion + "_" + Assembly.GetEntryAssembly().Location + "\r\n";
            sVer = System.Windows.Forms.Application.ProductVersion + "_" + Assembly.GetEntryAssembly().Location + "\r\n";
            tbVer.AppendText(sVer);
            Log.Trace(sVer);

            string sTest = Assembly.GetEntryAssembly().FullName;

            for (int i = 0; i < lst.Count; i++)
            {
                sVer = GetVer(lst[i]) + "_" + lst[i] + "\r\n";
                tbVer.AppendText(sVer);
                Log.Trace(sVer);
            }

            tbVer.AppendText("\n");

            tbVer.Text += sText;

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
