using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
//using CoreScanner;

namespace Machine
{
    public class DS_6878
    {
        Image imgCapturedImage;
        //CCoreScannerClass m_pCoreScanner;
        bool m_bSuccessOpen;//Is open success
        XmlReader m_xml;
        bool m_bIgnoreIndexChange;
        short m_nNumberOfTypes;
        short[] m_arScannerTypes;
        bool[] m_arSelectedTypes;
        int m_nTotalScanners;
        static ushort[] m_arParamsList; //Parameter information list 
        static ushort[] m_arViewFindParamsList; //Parameter information list - viewFinder
        int m_nRsmAttributeCount;
        long m_nResultLineCount;
        int[] m_nArTotalScannersInType;//total scanners in types of SCANNER_TYPES_SNAPI,SCANNER_TYPES_SSI,SCANNER_TYPES_IBMHID,SCANNER_TYPES_NIXMODB,SCANNER_TYPES_HIDKB
        //DocCapMessage m_docCapMsg = new DocCapMessage();
        byte[] m_wavFile;
        List<string> claimlist = new List<string>();
        List<string> scanrdisablelist = new List<string>();

        /// <summary>
        /// Calls Open command
        /// </summary>
        //private void Connect()
        //{
        //    if (m_bSuccessOpen)
        //    {
        //        return;
        //    }
        //    int appHandle = 0;
        //    GetSelectedScannerTypes();
        //    int status = STATUS_FALSE;
        //
        //    try
        //    {
        //        m_pCoreScanner.Open(appHandle, m_arScannerTypes, m_nNumberOfTypes, out status);
        //        DisplayResult(status, "OPEN");
        //        if (STATUS_SUCCESS == status)
        //        {
        //            m_bSuccessOpen = true;
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        MessageBox.Show("Error OPEN - " + exp.Message, APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        if (STATUS_SUCCESS == status)
        //        {
        //            SetControls();
        //        }
        //    }
        //}
    }
}
