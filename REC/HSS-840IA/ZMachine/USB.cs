using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using COMMON;
using System.Management;
using SML;

namespace Machine
{
    public class USBSerialNumber   //USB 정보를 가져와서 출력해주는 클래스
    {
        public System.Windows.Forms.Timer tmUpdate;
        public ListBox listBox1 = new ListBox();
        protected CDelayTimer m_tmDelay; 
        public USBSerialNumber()
        {
            listBox1.Items.Clear();
            var Devices = GetDevices();
            string sTemp = "";
            foreach (var d in Devices)
            {
                if(d.Description == "FS Ergo")
                {
                    sTemp += "FS Ergo";

                }
            }

            listBox1.Items.Add(sTemp);

            tmUpdate = new System.Windows.Forms.Timer();
            tmUpdate.Interval = 100;
            tmUpdate.Tick += new EventHandler(tmUpdate_Tick); //
            tmUpdate.Enabled = true;
        }
        
        
        // USB 정보 가져오는 부분 
        private List<USBDeviceInfo> GetDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new USBDeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description")
                ));
            }

            collection.Dispose();
            return devices;
        }

        private int GetDevicesCnt(string _sName)
        {
            int iCount = 0;
            ManagementObjectCollection collection;
            var query = string.Format("select * from Win32_PnPEntity");

            using (var searcher = new ManagementObjectSearcher(query))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                if (_sName == (string)device.GetPropertyValue("Description"))
                {
                    iCount++;
                }
                //devices.Append(
                ////(string)device.GetPropertyValue("DeviceID"),
                ////(string)device.GetPropertyValue("PNPDeviceID"),
                //(string)device.GetPropertyValue("Description"));

            }

            collection.Dispose();
            return iCount;
        }

        public void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;// DateTime.Now;
        }

        //public void Reset()
        //{
        //    tmUpdate = new System.Windows.Forms.Timer();
        //    tmUpdate.Interval = 100;
        //    tmUpdate.Tick += new EventHandler(tmUpdate_Tick); //
        //    tmUpdate.Enabled = true;
        //
        //    if (ML.CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Fwd))
        //    {
        //        SEQ.XRAY.bLeftCnct = true;
        //    }
        //    if (ML.CL_Complete(ci.XRAY_RightUSBFwBw, fb.Fwd))
        //    {
        //        SEQ.XRAY.bRightCnct = true;
        //    }
        //    Delay(500);
        //    if (ML.CL_Complete(ci.XRAY_LeftUSBFwBw, fb.Bwd))
        //    {
        //        SEQ.XRAY.bLeftCnct = false;
        //    }
        //    if (ML.CL_Complete(ci.XRAY_RightUSBFwBw, fb.Bwd))
        //    {
        //        SEQ.XRAY.bRightCnct = false;
        //    }
        //}

        
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            if (SEQ.XRYD.bDeviceChange)
            {
                if (ML.IO_GetY(yi.XRAY_LeftUSBFwBw, true))
                {
                    SEQ.XRYD.bLeftCnct  = true ;
                }
                else
                {
                    SEQ.XRYD.bLeftCnct = false;
                }

                if (ML.IO_GetY(yi.XRAY_RightUSBFwBw, true))
                {
                    SEQ.XRYD.bRightCnct = true ;
                }
                else
                {
                    SEQ.XRYD.bRightCnct = false;
                }
                SEQ.XRYD.bDeviceChange = false;
            }

            else if (SEQ.XRYE.bDeviceChange)
            {
                if (ML.IO_GetY(yi.XRAY_LeftUSBFwBw, true))
                {
                    SEQ.XRYE.bLeftCnct = true;
                }
                else
                {
                    SEQ.XRYE.bLeftCnct = false;
                }

                if (ML.IO_GetY(yi.XRAY_RightUSBFwBw, true))
                {
                    SEQ.XRYE.bRightCnct = true;
                }
                else
                {
                    SEQ.XRYE.bRightCnct = false;
                }
                SEQ.XRYE.bDeviceChange = false;
            }

            tmUpdate.Enabled = true;
        }

    }
    class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
    }

   

}
