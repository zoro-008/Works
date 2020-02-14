using Machine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Control
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Machine.Properties.Resources.Culture = new System.Globalization.CultureInfo("ko-KR");
            //     if(ConfigurationManager.AppSettings["Language"] == "Ko") Machine.Properties.Resources.Culture = new System.Globalization.CultureInfo("ko-KR");
            //else if(ConfigurationManager.AppSettings["Language"] == "En") Machine.Properties.Resources.Culture = new System.Globalization.CultureInfo("en-US");
        }
    }
}
