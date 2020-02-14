using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log
{
    public partial class LogVersion : Form
    {
        public LogVersion()
        {
            InitializeComponent();

            this.TopMost = true;

            //파일 버전, 수정한날짜 보여줄때 필요한 부분
            string sExeFolder = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            string FileName = Path.GetFileName(sExeFolder);
            FileInfo File = new FileInfo(FileName);
            //파일 버전 보여주는 부분
            string sFileVersion = System.Windows.Forms.Application.ProductVersion;
            lbVer.Text = "Version " + sFileVersion;
            //수정한 날짜 보여주는 부분
            double Age = File.LastWriteTime.ToOADate();
            string Date = DateTime.FromOADate(Age).ToString("yyyy MM-dd hh:mm:ss");
            lbDate.Text = Date;

        }
    }
}
