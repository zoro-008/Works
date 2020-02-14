using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control
{
    public partial class FormKeyence : Form
    {
        FormMain FrmMain;

        public FormKeyence(FormMain _FrmMain)
        {
            InitializeComponent();
            FrmMain = _FrmMain;
            _lstStatChs.SelectedIndex = 0;
                        
            Update(true);
            //Start();
            //Reset();

        }

        private void _btnStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        public bool Start()
        {
            if (_radUsb.Checked)
            {
                int rc = NativeMethods.LS9IF_UsbOpen();
                if (!CheckReturnCode(rc)) return false;
            }
            else
            {
                LS9IF_ETHERNET_CONFIG config = new LS9IF_ETHERNET_CONFIG();

                try
                {
                    config.abyIpAddress = new byte[]{ byte.Parse(_txtIpFirstSegment.Text),
                                                        byte.Parse(_txtIpSecondSegment.Text),
                                                        byte.Parse(_txtIpThirdSegment.Text),
                                                        byte.Parse(_txtIpFourthSegment.Text)};
                    config.wPortNo = ushort.Parse(_txtPort.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                    return false;
                }

                int rc = NativeMethods.LS9IF_EthernetOpen(ref config);
                if (!CheckReturnCode(rc)) return false;
            }

            OutPutResult(_grpBaseOperation.Text, _btnStart.Text, "OK");
            return true;
        }
        private void _btnEnd_Click(object sender, EventArgs e)
        {
            End();
        }

        public bool End()
        {
            int rc = NativeMethods.LS9IF_CommClose();
            if (!CheckReturnCode(rc)) return false;

            OutPutResult(_grpBaseOperation.Text, _btnEnd.Text, "OK");
            return true;
        }

        private void _radEthernet_CheckedChanged(object sender, EventArgs e)
        {
            _grpEthernetSetting.Enabled = _radEthernet.Checked;
        }

        private void _btnGetMeasureData_Click(object sender, EventArgs e)
        {
            LS9IF_MEASURE_DATA stMeasureData = new LS9IF_MEASURE_DATA();

            int rc = NativeMethods.LS9IF_GetMeasurementValue(ref stMeasureData);
            if (!CheckReturnCode(rc)) return;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < stMeasureData.stMesureValue.Length; i++)
            {
                stringBuilder.Append("OUT" + (i + 1).ToString());
                stringBuilder.Append("갌" + stMeasureData.stMesureValue[i].fValue.ToString("0.##########"));
                stringBuilder.AppendLine();
            }

            OutPutResult(_grpGetData.Text, _btnGetMeasureData.Text, stringBuilder.ToString());
        }

        private void _btnAutoZeroOn_Click(object sender, EventArgs e)
        {
            uint dwOut = GetTargetOutBits();
            int rc = NativeMethods.LS9IF_AutoZero(1, dwOut);
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpControl.Text, _btnAutoZeroOn.Text, "OK");
        }

        private void _btnAutoZeroOff_Click(object sender, EventArgs e)
        {
            uint dwOut = GetTargetOutBits();
            int rc = NativeMethods.LS9IF_AutoZero(0, dwOut);
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpControl.Text, _btnAutoZeroOff.Text, "OK");
        }

        private void _btnTimingOn_Click(object sender, EventArgs e)
        {
            uint dwOut = GetTargetOutBits();
            int rc = NativeMethods.LS9IF_Timing(1, dwOut);
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpControl.Text, _btnTimingOn.Text, "OK");
        }

        private void _btnTimingOff_Click(object sender, EventArgs e)
        {
            uint dwOut = GetTargetOutBits();
            int rc = NativeMethods.LS9IF_Timing(0, dwOut);
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpControl.Text, _btnTimingOff.Text, "OK");
        }

        private void _btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        public bool Reset()
        {
            uint dwOut = GetTargetOutBits();
            int rc = NativeMethods.LS9IF_Reset(dwOut);
            if (!CheckReturnCode(rc)) return false;

            OutPutResult(_grpControl.Text, _btnReset.Text, "OK");
            return true;
        }

        private uint GetTargetOutBits()
        {
            uint outBits = 0;
            outBits += _chkOut1.Checked ? 1u : 0;
            outBits += _chkOut2.Checked ? 2u : 0;
            outBits += _chkOut3.Checked ? 4u : 0;
            outBits += _chkOut4.Checked ? 8u : 0;
            outBits += _chkOut5.Checked ? 16u : 0;
            outBits += _chkOut6.Checked ? 32u : 0;
            outBits += _chkOut7.Checked ? 64u : 0;
            outBits += _chkOut8.Checked ? 128u : 0;
            outBits += _chkOut9.Checked ? 256u : 0;
            outBits += _chkOut10.Checked ? 512u : 0;
            outBits += _chkOut11.Checked ? 1024u : 0;
            outBits += _chkOut12.Checked ? 2048u : 0;
            outBits += _chkOut13.Checked ? 4096u : 0;
            outBits += _chkOut14.Checked ? 8192u : 0;
            outBits += _chkOut15.Checked ? 16384u : 0;
            outBits += _chkOut16.Checked ? 32768u : 0;

            return outBits;
        }

        private void _btnStartStorage_Click(object sender, EventArgs e)
        {
            int rc = NativeMethods.LS9IF_StartStorage();
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpDataStorage.Text, _btnStartStorage.Text, "OK");
        }

        private void _btnStopStorage_Click(object sender, EventArgs e)
        {
            int rc = NativeMethods.LS9IF_StopStorage();
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpDataStorage.Text, _btnStopStorage.Text, "OK");
        }

        private void _btnClearStorage_Click(object sender, EventArgs e)
        {
            int rc = NativeMethods.LS9IF_ClearMemory();
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpDataStorage.Text, _btnClearStorage.Text, "OK");
        }

        private void _btnGetStorageStatus_Click(object sender, EventArgs e)
        {
            LS9IF_STORAGE_INFO stStorageInfo = new LS9IF_STORAGE_INFO();

            int rc = NativeMethods.LS9IF_GetStorageStatus(ref stStorageInfo);
            if (!CheckReturnCode(rc)) return;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Status:" + stStorageInfo.byStatus.ToString());
            stringBuilder.AppendLine("Version:" + stStorageInfo.wStrageVer.ToString());
            stringBuilder.AppendLine("Number:" + stStorageInfo.dwStorageCnt.ToString());

            OutPutResult(_grpDataStorage.Text, _btnGetStorageStatus.Text, stringBuilder.ToString());
        }

        private void _btnGetStorageData_Click(object sender, EventArgs e)
        {
            uint readStart = (uint)_numGetStorageStart.Value;
            uint storageCount = (uint)_numGetStorageCount.Value;

            LS9IF_STORAGE_DATA[] stStorageData = new LS9IF_STORAGE_DATA[storageCount];

            int rc = NativeMethods.LS9IF_GetStorageData(readStart, storageCount, stStorageData);
            if (!CheckReturnCode(rc)) return;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < storageCount; i++)
            {
                stringBuilder.AppendLine("***Index:" + (readStart + i).ToString() + "***");

                string dateTimeString;
                try
                {
                    DateTime datetime = new DateTime(
                                       2000 + stStorageData[i].byYear
                                       , stStorageData[i].byMonth
                                       , stStorageData[i].byDay
                                       , stStorageData[i].byHour
                                       , stStorageData[i].byMinute
                                       , stStorageData[i].bySecond
                                       , stStorageData[i].byMillsecond
                                        );
                    dateTimeString = datetime.ToString();
                }
                catch (Exception)
                {
                    dateTimeString = "Invalid Date";
                }

                stringBuilder.AppendLine(dateTimeString);
                stringBuilder.AppendLine(" DST:" + stStorageData[i].byDstFlg.ToString());
                stringBuilder.AppendLine(" Pulse count:" + stStorageData[i].dwPulseCnt.ToString());

                for (int j = 0; j < stStorageData[i].stStorageValue.Length; j++)
                {
                    stringBuilder.Append(" OUT" + (j + 1).ToString());
                    stringBuilder.Append("-> Data info갌0x" + stStorageData[i].stStorageValue[j].byDataInfo.ToString("x2"));
                    stringBuilder.Append(", Judge갌0x" + stStorageData[i].stStorageValue[j].byJudge.ToString("x2"));
                    stringBuilder.Append(", Value갌" + stStorageData[i].stStorageValue[j].fValue.ToString("0.##########"));
                    stringBuilder.AppendLine();
                }
            }

            OutPutResult(_grpDataStorage.Text, _btnGetStorageData.Text, stringBuilder.ToString());
        }

        private void _bntGetStoragePoints_Click(object sender, EventArgs e)
        {
            byte byDepth = 0x02;
            LS9IF_TARGET_SETTING stTargetSetting = new LS9IF_TARGET_SETTING();
            stTargetSetting.byType = 0x10;
            stTargetSetting.byCategory = 0x05;
            stTargetSetting.byItem = 0x02;
            stTargetSetting.byTarget = 0x00;

            byte[] pbyDatas = new byte[128];
            int dwDataSize = 0;

            int rc = NativeMethods.LS9IF_GetSetting(byDepth, stTargetSetting, pbyDatas, ref dwDataSize);
            if (!CheckReturnCode(rc)) return;

            int storagePoints = BitConverter.ToInt32(pbyDatas, 0);

            OutPutResult(_grpSettings.Text, _bntGetStoragePoints.Text, storagePoints.ToString());
        }

        private void _bntSetStoragePoints_Click(object sender, EventArgs e)
        {
            byte byDepth = 0x02;
            LS9IF_TARGET_SETTING stTargetSetting = new LS9IF_TARGET_SETTING();
            stTargetSetting.byType = 0x10;
            stTargetSetting.byCategory = 0x05;
            stTargetSetting.byItem = 0x02;
            stTargetSetting.byTarget = 0x00;
            uint storagePoints = (uint)_numStoragePoints.Value;
            byte[] pbyDatas = BitConverter.GetBytes(storagePoints);

            uint pdwError = 0;

            int rc = NativeMethods.LS9IF_SetSetting(byDepth, stTargetSetting, pbyDatas.Length, pbyDatas, ref pdwError);
            if (!CheckReturnCode(rc)) return;

            string result;
            if (pdwError == 0)
            {
                result= "OK";
            }
            else
            {
                result = "Error code갌0x" + pdwError.ToString("x8");
            }

            OutPutResult(_grpSettings.Text, _bntSetStoragePoints.Text,result);
        }

        private void _btnStatSampStart_Click(object sender, EventArgs e)
        {
            byte statCh = (byte)_lstStatChs.SelectedIndex;

            int rc = NativeMethods.LS9IF_StatSampStart(statCh);
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpStatSamp.Text, _btnStatSampStart.Text, "OK");
        }

        private void _btnStatSampStop_Click(object sender, EventArgs e)
        {
            byte statCh = (byte)_lstStatChs.SelectedIndex;

            int rc = NativeMethods.LS9IF_StatSampStop(statCh);
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpStatSamp.Text, _btnStatSampStop.Text, "OK");
        }

        private void _btnGetStatSamp_Click(object sender, EventArgs e)
        {
            byte byStatChs = (byte)_lstStatChs.SelectedIndex;

            LS9IF_STAT_SAMP stStatSamp = new LS9IF_STAT_SAMP();

            int rc = NativeMethods.LS9IF_GetStatSamp(byStatChs, ref stStatSamp);
            if (!CheckReturnCode(rc)) return;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Status:" + stStatSamp.byStatus.ToString());
            stringBuilder.AppendLine("Average:" + stStatSamp.fAverage.ToString("0.##########"));
            stringBuilder.AppendLine("Max:" + stStatSamp.fMaximum.ToString("0.##########"));
            stringBuilder.AppendLine("Min:" + stStatSamp.fMinimum.ToString("0.##########"));
            stringBuilder.AppendLine("Max-Min:" + stStatSamp.fMax_Min.ToString("0.##########"));
            stringBuilder.AppendLine("Standard deviation:" + stStatSamp.fStdDeviation.ToString("0.##########"));
            stringBuilder.AppendLine("Parameter:" + stStatSamp.dwDenominator.ToString());
            stringBuilder.AppendLine("Number of HH:" + stStatSamp.dwHH_Count.ToString());
            stringBuilder.AppendLine("Number of HI:" + stStatSamp.dwHI_Count.ToString());
            stringBuilder.AppendLine("Number of GO:" + stStatSamp.dwGO_Count.ToString());
            stringBuilder.AppendLine("Number of LO:" + stStatSamp.dwLO_Count.ToString());
            stringBuilder.AppendLine("Number of LL:" + stStatSamp.dwLL_Count.ToString());

            OutPutResult(_grpStatSamp.Text, _btnGetStatSamp.Text, stringBuilder.ToString());
        }

        private void _btnStatSampClear_Click(object sender, EventArgs e)
        {
            byte statCh = (byte)_lstStatChs.SelectedIndex;

            int rc = NativeMethods.LS9IF_StatSampClear(statCh);
            if (!CheckReturnCode(rc)) return;

            OutPutResult(_grpStatSamp.Text, _btnStatSampClear.Text, "OK");

        }

        private bool CheckReturnCode(int rc)
        {
            if (rc != (int)Rc.Ok)
            {
                     if(rc == (int)Rc.ErrNotOpen  ) Log.ShowMessage("ErrNotOpen  ", "Keyence Connect Fail (Check the cable)", 1000);
                else if(rc == (int)Rc.ErrSend     ) Log.ShowMessage("ErrSend     ", "Keyence Communication Fail (Check the cable)", 1000);
                else if(rc == (int)Rc.ErrReceive  ) Log.ShowMessage("ErrReceive  ", "Keyence Communication Fail (Check the cable)", 1000);
                else if(rc == (int)Rc.ErrTimeout  ) Log.ShowMessage("ErrTimeout  ", "Keyence Communication Fail (Check the cable)", 1000);
                else if(rc == (int)Rc.ErrNoMemory ) Log.ShowMessage("ErrNoMemory ", "Keyence Communication Fail (Check the cable)", 1000);
                else if(rc == (int)Rc.ErrParameter) Log.ShowMessage("ErrParameter", "Keyence Communication Fail (Check the cable)", 1000);
                else if(rc == (int)Rc.ErrRecvFmt  ) Log.ShowMessage("ErrRecvFmt  ", "Keyence Communication Fail (Check the cable)", 1000);
                else if(rc == (int)Rc.ErrOpenYet  ) Log.ShowMessage("ErrOpenYet  ", "Keyence Communication Fail (Check the cable)", 1000);
                else                                Log.ShowMessage("ErrStart    ", "Keyence Communication Fail (Check the cable)", 1000);
                
                //MessageBox.Show(this, "Error: 0x" + rc.ToString("x4"), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void OutPutResult(string group, string function, string result)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt", CultureInfo.InvariantCulture));
            output.AppendLine("<" + group + " : " + function + ">");
            output.Append(result);
            FrmMain._txtOutput.Text = output.ToString();
        }

        private void FormKeyence_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = true;
            this.Hide();
        }

        private void btSaveMotr_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace("Form Keyence " + sText + " Button Click", 1);

            Update(false);
            OM.SaveCmnOptn();
        }


        public void Update(bool bToTable)
        {
            if (bToTable)
            {
                _txtIpFirstSegment.Text  = OM.CmnOptn.KysAdd1.ToString() ;
                _txtIpSecondSegment.Text = OM.CmnOptn.KysAdd2.ToString() ;
                _txtIpThirdSegment.Text  = OM.CmnOptn.KysAdd3.ToString() ;
                _txtIpFourthSegment.Text = OM.CmnOptn.KysAdd4.ToString() ;
                _txtPort.Text            = OM.CmnOptn.KysPort.ToString() ;
                _chkOut1.Checked         = OM.CmnOptn.bOut1              ;
                _chkOut2.Checked         = OM.CmnOptn.bOut2              ;
                _chkOut3.Checked         = OM.CmnOptn.bOut3              ;
                _chkOut4.Checked         = OM.CmnOptn.bOut4              ;
                _radEthernet.Checked     = OM.CmnOptn.bEthernet          ;

            }
            else 
            {
                OM.CmnOptn.KysAdd1            = CConfig.StrToIntDef(_txtIpFirstSegment.Text  ,192   );
                OM.CmnOptn.KysAdd2            = CConfig.StrToIntDef(_txtIpSecondSegment.Text ,168   );
                OM.CmnOptn.KysAdd3            = CConfig.StrToIntDef(_txtIpThirdSegment.Text  ,0     );
                OM.CmnOptn.KysAdd4            = CConfig.StrToIntDef(_txtIpFourthSegment.Text ,10    );
                OM.CmnOptn.KysPort            = CConfig.StrToIntDef(_txtPort.Text            , 24683);
                OM.CmnOptn.bOut1              = _chkOut1.Checked          ;
                OM.CmnOptn.bOut2              = _chkOut2.Checked          ;
                OM.CmnOptn.bOut3              = _chkOut3.Checked          ;
                OM.CmnOptn.bOut4              = _chkOut4.Checked          ;
                OM.CmnOptn.bEthernet          = _radEthernet.Checked      ;

                Update(true);
            }
        
        }

        private void FormKeyence_FormClosed(object sender, FormClosedEventArgs e)
        {
            End();
        }

        
    }
}
