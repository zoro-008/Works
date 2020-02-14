namespace Control
{
    partial class FormKeyence
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormKeyence));
            this._grpBaseOperation = new System.Windows.Forms.GroupBox();
            this._pnlCommunicationDevice = new System.Windows.Forms.Panel();
            this._radUsb = new System.Windows.Forms.RadioButton();
            this._radEthernet = new System.Windows.Forms.RadioButton();
            this._grpEthernetSetting = new System.Windows.Forms.GroupBox();
            this._txtIpFirstSegment = new System.Windows.Forms.TextBox();
            this._txtIpFourthSegment = new System.Windows.Forms.TextBox();
            this._txtIpSecondSegment = new System.Windows.Forms.TextBox();
            this._txtIpThirdSegment = new System.Windows.Forms.TextBox();
            this._lblIpSeparator3 = new System.Windows.Forms.Label();
            this._lblIpSeparator2 = new System.Windows.Forms.Label();
            this._lblIpSeparator1 = new System.Windows.Forms.Label();
            this._txtPort = new System.Windows.Forms.TextBox();
            this._lblIpAddress = new System.Windows.Forms.Label();
            this._lblPort = new System.Windows.Forms.Label();
            this._btnEnd = new System.Windows.Forms.Button();
            this._btnStart = new System.Windows.Forms.Button();
            this._grpSettings = new System.Windows.Forms.GroupBox();
            this._lblStoragePoints = new System.Windows.Forms.Label();
            this._bntSetStoragePoints = new System.Windows.Forms.Button();
            this._bntGetStoragePoints = new System.Windows.Forms.Button();
            this._numStoragePoints = new System.Windows.Forms.NumericUpDown();
            this._grpControl = new System.Windows.Forms.GroupBox();
            this._chkOut16 = new System.Windows.Forms.CheckBox();
            this._chkOut12 = new System.Windows.Forms.CheckBox();
            this._chkOut8 = new System.Windows.Forms.CheckBox();
            this._chkOut4 = new System.Windows.Forms.CheckBox();
            this._chkOut15 = new System.Windows.Forms.CheckBox();
            this._chkOut11 = new System.Windows.Forms.CheckBox();
            this._chkOut7 = new System.Windows.Forms.CheckBox();
            this._chkOut3 = new System.Windows.Forms.CheckBox();
            this._chkOut14 = new System.Windows.Forms.CheckBox();
            this._chkOut10 = new System.Windows.Forms.CheckBox();
            this._chkOut6 = new System.Windows.Forms.CheckBox();
            this._chkOut2 = new System.Windows.Forms.CheckBox();
            this._chkOut13 = new System.Windows.Forms.CheckBox();
            this._chkOut9 = new System.Windows.Forms.CheckBox();
            this._chkOut5 = new System.Windows.Forms.CheckBox();
            this._chkOut1 = new System.Windows.Forms.CheckBox();
            this._btnTimingOff = new System.Windows.Forms.Button();
            this._btnTimingOn = new System.Windows.Forms.Button();
            this._btnAutoZeroOff = new System.Windows.Forms.Button();
            this._btnAutoZeroOn = new System.Windows.Forms.Button();
            this._btnReset = new System.Windows.Forms.Button();
            this._grpDataStorage = new System.Windows.Forms.GroupBox();
            this._btnClearStorage = new System.Windows.Forms.Button();
            this._numGetStorageCount = new System.Windows.Forms.NumericUpDown();
            this._lblGetStorageCount = new System.Windows.Forms.Label();
            this._numGetStorageStart = new System.Windows.Forms.NumericUpDown();
            this._lblGetStorageStart = new System.Windows.Forms.Label();
            this._btnGetStorageData = new System.Windows.Forms.Button();
            this._btnGetStorageStatus = new System.Windows.Forms.Button();
            this._btnStopStorage = new System.Windows.Forms.Button();
            this._btnStartStorage = new System.Windows.Forms.Button();
            this._grpStatSamp = new System.Windows.Forms.GroupBox();
            this._btnStatSampClear = new System.Windows.Forms.Button();
            this._lstStatChs = new System.Windows.Forms.ListBox();
            this._btnGetStatSamp = new System.Windows.Forms.Button();
            this._btnStatSampStop = new System.Windows.Forms.Button();
            this._btnStatSampStart = new System.Windows.Forms.Button();
            this._grpGetData = new System.Windows.Forms.GroupBox();
            this._btnGetMeasureData = new System.Windows.Forms.Button();
            this.btSaveMotr = new System.Windows.Forms.Button();
            this._grpBaseOperation.SuspendLayout();
            this._pnlCommunicationDevice.SuspendLayout();
            this._grpEthernetSetting.SuspendLayout();
            this._grpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numStoragePoints)).BeginInit();
            this._grpControl.SuspendLayout();
            this._grpDataStorage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numGetStorageCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._numGetStorageStart)).BeginInit();
            this._grpStatSamp.SuspendLayout();
            this._grpGetData.SuspendLayout();
            this.SuspendLayout();
            // 
            // _grpBaseOperation
            // 
            this._grpBaseOperation.Controls.Add(this._pnlCommunicationDevice);
            this._grpBaseOperation.Controls.Add(this._btnEnd);
            this._grpBaseOperation.Controls.Add(this._btnStart);
            this._grpBaseOperation.Location = new System.Drawing.Point(12, 12);
            this._grpBaseOperation.Name = "_grpBaseOperation";
            this._grpBaseOperation.Size = new System.Drawing.Size(539, 105);
            this._grpBaseOperation.TabIndex = 1;
            this._grpBaseOperation.TabStop = false;
            this._grpBaseOperation.Text = "Start/Finish";
            // 
            // _pnlCommunicationDevice
            // 
            this._pnlCommunicationDevice.Controls.Add(this._radUsb);
            this._pnlCommunicationDevice.Controls.Add(this._radEthernet);
            this._pnlCommunicationDevice.Controls.Add(this._grpEthernetSetting);
            this._pnlCommunicationDevice.Location = new System.Drawing.Point(148, 18);
            this._pnlCommunicationDevice.Name = "_pnlCommunicationDevice";
            this._pnlCommunicationDevice.Size = new System.Drawing.Size(382, 83);
            this._pnlCommunicationDevice.TabIndex = 2;
            // 
            // _radUsb
            // 
            this._radUsb.AutoSize = true;
            this._radUsb.Checked = true;
            this._radUsb.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._radUsb.Location = new System.Drawing.Point(10, 4);
            this._radUsb.Name = "_radUsb";
            this._radUsb.Size = new System.Drawing.Size(47, 16);
            this._radUsb.TabIndex = 0;
            this._radUsb.TabStop = true;
            this._radUsb.Text = "USB";
            this._radUsb.UseVisualStyleBackColor = true;
            // 
            // _radEthernet
            // 
            this._radEthernet.AutoSize = true;
            this._radEthernet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._radEthernet.Location = new System.Drawing.Point(71, 3);
            this._radEthernet.Name = "_radEthernet";
            this._radEthernet.Size = new System.Drawing.Size(69, 16);
            this._radEthernet.TabIndex = 1;
            this._radEthernet.Text = "Ethernet";
            this._radEthernet.UseVisualStyleBackColor = true;
            this._radEthernet.CheckedChanged += new System.EventHandler(this._radEthernet_CheckedChanged);
            // 
            // _grpEthernetSetting
            // 
            this._grpEthernetSetting.Controls.Add(this._txtIpFirstSegment);
            this._grpEthernetSetting.Controls.Add(this._txtIpFourthSegment);
            this._grpEthernetSetting.Controls.Add(this._txtIpSecondSegment);
            this._grpEthernetSetting.Controls.Add(this._txtIpThirdSegment);
            this._grpEthernetSetting.Controls.Add(this._lblIpSeparator3);
            this._grpEthernetSetting.Controls.Add(this._lblIpSeparator2);
            this._grpEthernetSetting.Controls.Add(this._lblIpSeparator1);
            this._grpEthernetSetting.Controls.Add(this._txtPort);
            this._grpEthernetSetting.Controls.Add(this._lblIpAddress);
            this._grpEthernetSetting.Controls.Add(this._lblPort);
            this._grpEthernetSetting.Enabled = false;
            this._grpEthernetSetting.Location = new System.Drawing.Point(66, 4);
            this._grpEthernetSetting.Name = "_grpEthernetSetting";
            this._grpEthernetSetting.Size = new System.Drawing.Size(308, 73);
            this._grpEthernetSetting.TabIndex = 2;
            this._grpEthernetSetting.TabStop = false;
            // 
            // _txtIpFirstSegment
            // 
            this._txtIpFirstSegment.Location = new System.Drawing.Point(136, 20);
            this._txtIpFirstSegment.Name = "_txtIpFirstSegment";
            this._txtIpFirstSegment.Size = new System.Drawing.Size(30, 21);
            this._txtIpFirstSegment.TabIndex = 1;
            this._txtIpFirstSegment.Text = "192";
            // 
            // _txtIpFourthSegment
            // 
            this._txtIpFourthSegment.Location = new System.Drawing.Point(266, 20);
            this._txtIpFourthSegment.Name = "_txtIpFourthSegment";
            this._txtIpFourthSegment.Size = new System.Drawing.Size(30, 21);
            this._txtIpFourthSegment.TabIndex = 7;
            this._txtIpFourthSegment.Text = "10";
            // 
            // _txtIpSecondSegment
            // 
            this._txtIpSecondSegment.Location = new System.Drawing.Point(180, 20);
            this._txtIpSecondSegment.Name = "_txtIpSecondSegment";
            this._txtIpSecondSegment.Size = new System.Drawing.Size(30, 21);
            this._txtIpSecondSegment.TabIndex = 3;
            this._txtIpSecondSegment.Text = "168";
            // 
            // _txtIpThirdSegment
            // 
            this._txtIpThirdSegment.Location = new System.Drawing.Point(223, 20);
            this._txtIpThirdSegment.Name = "_txtIpThirdSegment";
            this._txtIpThirdSegment.Size = new System.Drawing.Size(30, 21);
            this._txtIpThirdSegment.TabIndex = 5;
            this._txtIpThirdSegment.Text = "0";
            // 
            // _lblIpSeparator3
            // 
            this._lblIpSeparator3.AutoSize = true;
            this._lblIpSeparator3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._lblIpSeparator3.Location = new System.Drawing.Point(255, 23);
            this._lblIpSeparator3.Name = "_lblIpSeparator3";
            this._lblIpSeparator3.Size = new System.Drawing.Size(9, 12);
            this._lblIpSeparator3.TabIndex = 6;
            this._lblIpSeparator3.Text = ".";
            // 
            // _lblIpSeparator2
            // 
            this._lblIpSeparator2.AutoSize = true;
            this._lblIpSeparator2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._lblIpSeparator2.Location = new System.Drawing.Point(212, 23);
            this._lblIpSeparator2.Name = "_lblIpSeparator2";
            this._lblIpSeparator2.Size = new System.Drawing.Size(9, 12);
            this._lblIpSeparator2.TabIndex = 4;
            this._lblIpSeparator2.Text = ".";
            // 
            // _lblIpSeparator1
            // 
            this._lblIpSeparator1.AutoSize = true;
            this._lblIpSeparator1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._lblIpSeparator1.Location = new System.Drawing.Point(169, 23);
            this._lblIpSeparator1.Name = "_lblIpSeparator1";
            this._lblIpSeparator1.Size = new System.Drawing.Size(9, 12);
            this._lblIpSeparator1.TabIndex = 2;
            this._lblIpSeparator1.Text = ".";
            // 
            // _txtPort
            // 
            this._txtPort.Location = new System.Drawing.Point(136, 46);
            this._txtPort.Name = "_txtPort";
            this._txtPort.Size = new System.Drawing.Size(68, 21);
            this._txtPort.TabIndex = 9;
            this._txtPort.Text = "24683";
            // 
            // _lblIpAddress
            // 
            this._lblIpAddress.AutoSize = true;
            this._lblIpAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._lblIpAddress.Location = new System.Drawing.Point(17, 23);
            this._lblIpAddress.Name = "_lblIpAddress";
            this._lblIpAddress.Size = new System.Drawing.Size(67, 12);
            this._lblIpAddress.TabIndex = 0;
            this._lblIpAddress.Text = "IP Address";
            // 
            // _lblPort
            // 
            this._lblPort.AutoSize = true;
            this._lblPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._lblPort.Location = new System.Drawing.Point(17, 49);
            this._lblPort.Name = "_lblPort";
            this._lblPort.Size = new System.Drawing.Size(105, 12);
            this._lblPort.TabIndex = 8;
            this._lblPort.Text = "TCP Port Number";
            // 
            // _btnEnd
            // 
            this._btnEnd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._btnEnd.Location = new System.Drawing.Point(7, 47);
            this._btnEnd.Name = "_btnEnd";
            this._btnEnd.Size = new System.Drawing.Size(134, 23);
            this._btnEnd.TabIndex = 1;
            this._btnEnd.Text = "Finish";
            this._btnEnd.UseVisualStyleBackColor = true;
            this._btnEnd.Click += new System.EventHandler(this._btnEnd_Click);
            // 
            // _btnStart
            // 
            this._btnStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._btnStart.Location = new System.Drawing.Point(7, 18);
            this._btnStart.Name = "_btnStart";
            this._btnStart.Size = new System.Drawing.Size(134, 23);
            this._btnStart.TabIndex = 0;
            this._btnStart.Text = "Start";
            this._btnStart.UseVisualStyleBackColor = true;
            this._btnStart.Click += new System.EventHandler(this._btnStart_Click);
            // 
            // _grpSettings
            // 
            this._grpSettings.Controls.Add(this._lblStoragePoints);
            this._grpSettings.Controls.Add(this._bntSetStoragePoints);
            this._grpSettings.Controls.Add(this._bntGetStoragePoints);
            this._grpSettings.Controls.Add(this._numStoragePoints);
            this._grpSettings.Location = new System.Drawing.Point(445, 123);
            this._grpSettings.Name = "_grpSettings";
            this._grpSettings.Size = new System.Drawing.Size(205, 114);
            this._grpSettings.TabIndex = 9;
            this._grpSettings.TabStop = false;
            this._grpSettings.Text = "Settings(Program No.0)";
            // 
            // _lblStoragePoints
            // 
            this._lblStoragePoints.AutoSize = true;
            this._lblStoragePoints.Location = new System.Drawing.Point(7, 15);
            this._lblStoragePoints.Name = "_lblStoragePoints";
            this._lblStoragePoints.Size = new System.Drawing.Size(86, 12);
            this._lblStoragePoints.TabIndex = 0;
            this._lblStoragePoints.Text = "Storage points";
            // 
            // _bntSetStoragePoints
            // 
            this._bntSetStoragePoints.ForeColor = System.Drawing.SystemColors.ControlText;
            this._bntSetStoragePoints.Location = new System.Drawing.Point(7, 59);
            this._bntSetStoragePoints.Name = "_bntSetStoragePoints";
            this._bntSetStoragePoints.Size = new System.Drawing.Size(191, 23);
            this._bntSetStoragePoints.TabIndex = 2;
            this._bntSetStoragePoints.Text = "Set";
            this._bntSetStoragePoints.Click += new System.EventHandler(this._bntSetStoragePoints_Click);
            // 
            // _bntGetStoragePoints
            // 
            this._bntGetStoragePoints.ForeColor = System.Drawing.SystemColors.ControlText;
            this._bntGetStoragePoints.Location = new System.Drawing.Point(7, 30);
            this._bntGetStoragePoints.Name = "_bntGetStoragePoints";
            this._bntGetStoragePoints.Size = new System.Drawing.Size(191, 23);
            this._bntGetStoragePoints.TabIndex = 1;
            this._bntGetStoragePoints.Text = "Get";
            this._bntGetStoragePoints.Click += new System.EventHandler(this._bntGetStoragePoints_Click);
            // 
            // _numStoragePoints
            // 
            this._numStoragePoints.Location = new System.Drawing.Point(92, 88);
            this._numStoragePoints.Maximum = new decimal(new int[] {
            400000,
            0,
            0,
            0});
            this._numStoragePoints.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._numStoragePoints.Name = "_numStoragePoints";
            this._numStoragePoints.Size = new System.Drawing.Size(106, 21);
            this._numStoragePoints.TabIndex = 3;
            this._numStoragePoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._numStoragePoints.Value = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            // 
            // _grpControl
            // 
            this._grpControl.Controls.Add(this._chkOut16);
            this._grpControl.Controls.Add(this._chkOut12);
            this._grpControl.Controls.Add(this._chkOut8);
            this._grpControl.Controls.Add(this._chkOut4);
            this._grpControl.Controls.Add(this._chkOut15);
            this._grpControl.Controls.Add(this._chkOut11);
            this._grpControl.Controls.Add(this._chkOut7);
            this._grpControl.Controls.Add(this._chkOut3);
            this._grpControl.Controls.Add(this._chkOut14);
            this._grpControl.Controls.Add(this._chkOut10);
            this._grpControl.Controls.Add(this._chkOut6);
            this._grpControl.Controls.Add(this._chkOut2);
            this._grpControl.Controls.Add(this._chkOut13);
            this._grpControl.Controls.Add(this._chkOut9);
            this._grpControl.Controls.Add(this._chkOut5);
            this._grpControl.Controls.Add(this._chkOut1);
            this._grpControl.Controls.Add(this._btnTimingOff);
            this._grpControl.Controls.Add(this._btnTimingOn);
            this._grpControl.Controls.Add(this._btnAutoZeroOff);
            this._grpControl.Controls.Add(this._btnAutoZeroOn);
            this._grpControl.Controls.Add(this._btnReset);
            this._grpControl.Location = new System.Drawing.Point(12, 184);
            this._grpControl.Name = "_grpControl";
            this._grpControl.Size = new System.Drawing.Size(205, 293);
            this._grpControl.TabIndex = 7;
            this._grpControl.TabStop = false;
            this._grpControl.Text = "Control";
            // 
            // _chkOut16
            // 
            this._chkOut16.AutoSize = true;
            this._chkOut16.Location = new System.Drawing.Point(80, 176);
            this._chkOut16.Name = "_chkOut16";
            this._chkOut16.Size = new System.Drawing.Size(61, 16);
            this._chkOut16.TabIndex = 15;
            this._chkOut16.Text = "OUT16";
            this._chkOut16.UseVisualStyleBackColor = true;
            // 
            // _chkOut12
            // 
            this._chkOut12.AutoSize = true;
            this._chkOut12.Location = new System.Drawing.Point(80, 132);
            this._chkOut12.Name = "_chkOut12";
            this._chkOut12.Size = new System.Drawing.Size(61, 16);
            this._chkOut12.TabIndex = 11;
            this._chkOut12.Text = "OUT12";
            this._chkOut12.UseVisualStyleBackColor = true;
            // 
            // _chkOut8
            // 
            this._chkOut8.AutoSize = true;
            this._chkOut8.Location = new System.Drawing.Point(80, 88);
            this._chkOut8.Name = "_chkOut8";
            this._chkOut8.Size = new System.Drawing.Size(55, 16);
            this._chkOut8.TabIndex = 7;
            this._chkOut8.Text = "OUT8";
            this._chkOut8.UseVisualStyleBackColor = true;
            // 
            // _chkOut4
            // 
            this._chkOut4.AutoSize = true;
            this._chkOut4.Location = new System.Drawing.Point(80, 44);
            this._chkOut4.Name = "_chkOut4";
            this._chkOut4.Size = new System.Drawing.Size(55, 16);
            this._chkOut4.TabIndex = 3;
            this._chkOut4.Text = "OUT4";
            this._chkOut4.UseVisualStyleBackColor = true;
            // 
            // _chkOut15
            // 
            this._chkOut15.AutoSize = true;
            this._chkOut15.Location = new System.Drawing.Point(12, 176);
            this._chkOut15.Name = "_chkOut15";
            this._chkOut15.Size = new System.Drawing.Size(61, 16);
            this._chkOut15.TabIndex = 14;
            this._chkOut15.Text = "OUT15";
            this._chkOut15.UseVisualStyleBackColor = true;
            // 
            // _chkOut11
            // 
            this._chkOut11.AutoSize = true;
            this._chkOut11.Location = new System.Drawing.Point(12, 132);
            this._chkOut11.Name = "_chkOut11";
            this._chkOut11.Size = new System.Drawing.Size(61, 16);
            this._chkOut11.TabIndex = 10;
            this._chkOut11.Text = "OUT11";
            this._chkOut11.UseVisualStyleBackColor = true;
            // 
            // _chkOut7
            // 
            this._chkOut7.AutoSize = true;
            this._chkOut7.Location = new System.Drawing.Point(12, 88);
            this._chkOut7.Name = "_chkOut7";
            this._chkOut7.Size = new System.Drawing.Size(55, 16);
            this._chkOut7.TabIndex = 6;
            this._chkOut7.Text = "OUT7";
            this._chkOut7.UseVisualStyleBackColor = true;
            // 
            // _chkOut3
            // 
            this._chkOut3.AutoSize = true;
            this._chkOut3.Location = new System.Drawing.Point(12, 44);
            this._chkOut3.Name = "_chkOut3";
            this._chkOut3.Size = new System.Drawing.Size(55, 16);
            this._chkOut3.TabIndex = 2;
            this._chkOut3.Text = "OUT3";
            this._chkOut3.UseVisualStyleBackColor = true;
            // 
            // _chkOut14
            // 
            this._chkOut14.AutoSize = true;
            this._chkOut14.Location = new System.Drawing.Point(80, 154);
            this._chkOut14.Name = "_chkOut14";
            this._chkOut14.Size = new System.Drawing.Size(61, 16);
            this._chkOut14.TabIndex = 13;
            this._chkOut14.Text = "OUT14";
            this._chkOut14.UseVisualStyleBackColor = true;
            // 
            // _chkOut10
            // 
            this._chkOut10.AutoSize = true;
            this._chkOut10.Location = new System.Drawing.Point(80, 110);
            this._chkOut10.Name = "_chkOut10";
            this._chkOut10.Size = new System.Drawing.Size(61, 16);
            this._chkOut10.TabIndex = 9;
            this._chkOut10.Text = "OUT10";
            this._chkOut10.UseVisualStyleBackColor = true;
            // 
            // _chkOut6
            // 
            this._chkOut6.AutoSize = true;
            this._chkOut6.Location = new System.Drawing.Point(80, 66);
            this._chkOut6.Name = "_chkOut6";
            this._chkOut6.Size = new System.Drawing.Size(55, 16);
            this._chkOut6.TabIndex = 5;
            this._chkOut6.Text = "OUT6";
            this._chkOut6.UseVisualStyleBackColor = true;
            // 
            // _chkOut2
            // 
            this._chkOut2.AutoSize = true;
            this._chkOut2.Location = new System.Drawing.Point(80, 22);
            this._chkOut2.Name = "_chkOut2";
            this._chkOut2.Size = new System.Drawing.Size(55, 16);
            this._chkOut2.TabIndex = 1;
            this._chkOut2.Text = "OUT2";
            this._chkOut2.UseVisualStyleBackColor = true;
            // 
            // _chkOut13
            // 
            this._chkOut13.AutoSize = true;
            this._chkOut13.Location = new System.Drawing.Point(12, 154);
            this._chkOut13.Name = "_chkOut13";
            this._chkOut13.Size = new System.Drawing.Size(61, 16);
            this._chkOut13.TabIndex = 12;
            this._chkOut13.Text = "OUT13";
            this._chkOut13.UseVisualStyleBackColor = true;
            // 
            // _chkOut9
            // 
            this._chkOut9.AutoSize = true;
            this._chkOut9.Location = new System.Drawing.Point(12, 110);
            this._chkOut9.Name = "_chkOut9";
            this._chkOut9.Size = new System.Drawing.Size(55, 16);
            this._chkOut9.TabIndex = 8;
            this._chkOut9.Text = "OUT9";
            this._chkOut9.UseVisualStyleBackColor = true;
            // 
            // _chkOut5
            // 
            this._chkOut5.AutoSize = true;
            this._chkOut5.Location = new System.Drawing.Point(12, 66);
            this._chkOut5.Name = "_chkOut5";
            this._chkOut5.Size = new System.Drawing.Size(55, 16);
            this._chkOut5.TabIndex = 4;
            this._chkOut5.Text = "OUT5";
            this._chkOut5.UseVisualStyleBackColor = true;
            // 
            // _chkOut1
            // 
            this._chkOut1.AutoSize = true;
            this._chkOut1.Checked = true;
            this._chkOut1.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chkOut1.Location = new System.Drawing.Point(12, 22);
            this._chkOut1.Name = "_chkOut1";
            this._chkOut1.Size = new System.Drawing.Size(55, 16);
            this._chkOut1.TabIndex = 0;
            this._chkOut1.Text = "OUT1";
            this._chkOut1.UseVisualStyleBackColor = true;
            // 
            // _btnTimingOff
            // 
            this._btnTimingOff.Location = new System.Drawing.Point(105, 231);
            this._btnTimingOff.Name = "_btnTimingOff";
            this._btnTimingOff.Size = new System.Drawing.Size(93, 23);
            this._btnTimingOff.TabIndex = 19;
            this._btnTimingOff.Text = "TIMING OFF";
            this._btnTimingOff.Click += new System.EventHandler(this._btnTimingOff_Click);
            // 
            // _btnTimingOn
            // 
            this._btnTimingOn.Location = new System.Drawing.Point(7, 231);
            this._btnTimingOn.Name = "_btnTimingOn";
            this._btnTimingOn.Size = new System.Drawing.Size(93, 23);
            this._btnTimingOn.TabIndex = 18;
            this._btnTimingOn.Text = "TIMING ON";
            this._btnTimingOn.Click += new System.EventHandler(this._btnTimingOn_Click);
            // 
            // _btnAutoZeroOff
            // 
            this._btnAutoZeroOff.Location = new System.Drawing.Point(105, 202);
            this._btnAutoZeroOff.Name = "_btnAutoZeroOff";
            this._btnAutoZeroOff.Size = new System.Drawing.Size(93, 23);
            this._btnAutoZeroOff.TabIndex = 17;
            this._btnAutoZeroOff.Text = "ZERO OFF";
            this._btnAutoZeroOff.Click += new System.EventHandler(this._btnAutoZeroOff_Click);
            // 
            // _btnAutoZeroOn
            // 
            this._btnAutoZeroOn.Location = new System.Drawing.Point(7, 202);
            this._btnAutoZeroOn.Name = "_btnAutoZeroOn";
            this._btnAutoZeroOn.Size = new System.Drawing.Size(93, 23);
            this._btnAutoZeroOn.TabIndex = 16;
            this._btnAutoZeroOn.Text = "ZERO ON";
            this._btnAutoZeroOn.Click += new System.EventHandler(this._btnAutoZeroOn_Click);
            // 
            // _btnReset
            // 
            this._btnReset.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._btnReset.Location = new System.Drawing.Point(7, 260);
            this._btnReset.Name = "_btnReset";
            this._btnReset.Size = new System.Drawing.Size(191, 23);
            this._btnReset.TabIndex = 20;
            this._btnReset.Text = "RESET";
            this._btnReset.UseVisualStyleBackColor = true;
            this._btnReset.Click += new System.EventHandler(this._btnReset_Click);
            // 
            // _grpDataStorage
            // 
            this._grpDataStorage.Controls.Add(this._btnClearStorage);
            this._grpDataStorage.Controls.Add(this._numGetStorageCount);
            this._grpDataStorage.Controls.Add(this._lblGetStorageCount);
            this._grpDataStorage.Controls.Add(this._numGetStorageStart);
            this._grpDataStorage.Controls.Add(this._lblGetStorageStart);
            this._grpDataStorage.Controls.Add(this._btnGetStorageData);
            this._grpDataStorage.Controls.Add(this._btnGetStorageStatus);
            this._grpDataStorage.Controls.Add(this._btnStopStorage);
            this._grpDataStorage.Controls.Add(this._btnStartStorage);
            this._grpDataStorage.Location = new System.Drawing.Point(224, 123);
            this._grpDataStorage.Name = "_grpDataStorage";
            this._grpDataStorage.Size = new System.Drawing.Size(205, 354);
            this._grpDataStorage.TabIndex = 8;
            this._grpDataStorage.TabStop = false;
            this._grpDataStorage.Text = "Data storage";
            // 
            // _btnClearStorage
            // 
            this._btnClearStorage.ForeColor = System.Drawing.SystemColors.ControlText;
            this._btnClearStorage.Location = new System.Drawing.Point(7, 47);
            this._btnClearStorage.Name = "_btnClearStorage";
            this._btnClearStorage.Size = new System.Drawing.Size(191, 23);
            this._btnClearStorage.TabIndex = 2;
            this._btnClearStorage.Text = "Clear";
            this._btnClearStorage.Click += new System.EventHandler(this._btnClearStorage_Click);
            // 
            // _numGetStorageCount
            // 
            this._numGetStorageCount.Location = new System.Drawing.Point(138, 159);
            this._numGetStorageCount.Maximum = new decimal(new int[] {
            45000,
            0,
            0,
            0});
            this._numGetStorageCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._numGetStorageCount.Name = "_numGetStorageCount";
            this._numGetStorageCount.Size = new System.Drawing.Size(61, 21);
            this._numGetStorageCount.TabIndex = 8;
            this._numGetStorageCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._numGetStorageCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // _lblGetStorageCount
            // 
            this._lblGetStorageCount.AutoSize = true;
            this._lblGetStorageCount.Location = new System.Drawing.Point(90, 161);
            this._lblGetStorageCount.Name = "_lblGetStorageCount";
            this._lblGetStorageCount.Size = new System.Drawing.Size(38, 12);
            this._lblGetStorageCount.TabIndex = 7;
            this._lblGetStorageCount.Text = "Count";
            // 
            // _numGetStorageStart
            // 
            this._numGetStorageStart.Location = new System.Drawing.Point(138, 134);
            this._numGetStorageStart.Maximum = new decimal(new int[] {
            399999,
            0,
            0,
            0});
            this._numGetStorageStart.Name = "_numGetStorageStart";
            this._numGetStorageStart.Size = new System.Drawing.Size(61, 21);
            this._numGetStorageStart.TabIndex = 6;
            this._numGetStorageStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _lblGetStorageStart
            // 
            this._lblGetStorageStart.AutoSize = true;
            this._lblGetStorageStart.Location = new System.Drawing.Point(59, 136);
            this._lblGetStorageStart.Name = "_lblGetStorageStart";
            this._lblGetStorageStart.Size = new System.Drawing.Size(65, 12);
            this._lblGetStorageStart.TabIndex = 5;
            this._lblGetStorageStart.Text = "Start index";
            // 
            // _btnGetStorageData
            // 
            this._btnGetStorageData.ForeColor = System.Drawing.SystemColors.ControlText;
            this._btnGetStorageData.Location = new System.Drawing.Point(7, 105);
            this._btnGetStorageData.Name = "_btnGetStorageData";
            this._btnGetStorageData.Size = new System.Drawing.Size(191, 23);
            this._btnGetStorageData.TabIndex = 4;
            this._btnGetStorageData.Text = "Get storage data";
            this._btnGetStorageData.Click += new System.EventHandler(this._btnGetStorageData_Click);
            // 
            // _btnGetStorageStatus
            // 
            this._btnGetStorageStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this._btnGetStorageStatus.Location = new System.Drawing.Point(7, 76);
            this._btnGetStorageStatus.Name = "_btnGetStorageStatus";
            this._btnGetStorageStatus.Size = new System.Drawing.Size(191, 23);
            this._btnGetStorageStatus.TabIndex = 3;
            this._btnGetStorageStatus.Text = "Get status";
            this._btnGetStorageStatus.Click += new System.EventHandler(this._btnGetStorageStatus_Click);
            // 
            // _btnStopStorage
            // 
            this._btnStopStorage.Location = new System.Drawing.Point(105, 18);
            this._btnStopStorage.Name = "_btnStopStorage";
            this._btnStopStorage.Size = new System.Drawing.Size(93, 23);
            this._btnStopStorage.TabIndex = 1;
            this._btnStopStorage.Text = "Stop";
            this._btnStopStorage.Click += new System.EventHandler(this._btnStopStorage_Click);
            // 
            // _btnStartStorage
            // 
            this._btnStartStorage.Location = new System.Drawing.Point(7, 18);
            this._btnStartStorage.Name = "_btnStartStorage";
            this._btnStartStorage.Size = new System.Drawing.Size(93, 23);
            this._btnStartStorage.TabIndex = 0;
            this._btnStartStorage.Text = "Start";
            this._btnStartStorage.Click += new System.EventHandler(this._btnStartStorage_Click);
            // 
            // _grpStatSamp
            // 
            this._grpStatSamp.Controls.Add(this._btnStatSampClear);
            this._grpStatSamp.Controls.Add(this._lstStatChs);
            this._grpStatSamp.Controls.Add(this._btnGetStatSamp);
            this._grpStatSamp.Controls.Add(this._btnStatSampStop);
            this._grpStatSamp.Controls.Add(this._btnStatSampStart);
            this._grpStatSamp.Location = new System.Drawing.Point(445, 250);
            this._grpStatSamp.Name = "_grpStatSamp";
            this._grpStatSamp.Size = new System.Drawing.Size(205, 227);
            this._grpStatSamp.TabIndex = 10;
            this._grpStatSamp.TabStop = false;
            this._grpStatSamp.Text = "Statistics";
            // 
            // _btnStatSampClear
            // 
            this._btnStatSampClear.ForeColor = System.Drawing.SystemColors.ControlText;
            this._btnStatSampClear.Location = new System.Drawing.Point(105, 81);
            this._btnStatSampClear.Name = "_btnStatSampClear";
            this._btnStatSampClear.Size = new System.Drawing.Size(93, 23);
            this._btnStatSampClear.TabIndex = 4;
            this._btnStatSampClear.Text = "Clear";
            this._btnStatSampClear.Click += new System.EventHandler(this._btnStatSampClear_Click);
            // 
            // _lstStatChs
            // 
            this._lstStatChs.FormattingEnabled = true;
            this._lstStatChs.ItemHeight = 12;
            this._lstStatChs.Items.AddRange(new object[] {
            "Statistics Ch1",
            "Statistics Ch2"});
            this._lstStatChs.Location = new System.Drawing.Point(7, 18);
            this._lstStatChs.Name = "_lstStatChs";
            this._lstStatChs.Size = new System.Drawing.Size(180, 28);
            this._lstStatChs.TabIndex = 0;
            // 
            // _btnGetStatSamp
            // 
            this._btnGetStatSamp.ForeColor = System.Drawing.SystemColors.ControlText;
            this._btnGetStatSamp.Location = new System.Drawing.Point(7, 81);
            this._btnGetStatSamp.Name = "_btnGetStatSamp";
            this._btnGetStatSamp.Size = new System.Drawing.Size(93, 23);
            this._btnGetStatSamp.TabIndex = 3;
            this._btnGetStatSamp.Text = "Get";
            this._btnGetStatSamp.Click += new System.EventHandler(this._btnGetStatSamp_Click);
            // 
            // _btnStatSampStop
            // 
            this._btnStatSampStop.Location = new System.Drawing.Point(105, 52);
            this._btnStatSampStop.Name = "_btnStatSampStop";
            this._btnStatSampStop.Size = new System.Drawing.Size(93, 23);
            this._btnStatSampStop.TabIndex = 2;
            this._btnStatSampStop.Text = "Stop";
            this._btnStatSampStop.Click += new System.EventHandler(this._btnStatSampStop_Click);
            // 
            // _btnStatSampStart
            // 
            this._btnStatSampStart.Location = new System.Drawing.Point(7, 52);
            this._btnStatSampStart.Name = "_btnStatSampStart";
            this._btnStatSampStart.Size = new System.Drawing.Size(93, 23);
            this._btnStatSampStart.TabIndex = 1;
            this._btnStatSampStart.Text = "Start";
            this._btnStatSampStart.Click += new System.EventHandler(this._btnStatSampStart_Click);
            // 
            // _grpGetData
            // 
            this._grpGetData.Controls.Add(this._btnGetMeasureData);
            this._grpGetData.Location = new System.Drawing.Point(12, 123);
            this._grpGetData.Name = "_grpGetData";
            this._grpGetData.Size = new System.Drawing.Size(205, 55);
            this._grpGetData.TabIndex = 6;
            this._grpGetData.TabStop = false;
            this._grpGetData.Text = "Measurement";
            // 
            // _btnGetMeasureData
            // 
            this._btnGetMeasureData.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._btnGetMeasureData.Location = new System.Drawing.Point(7, 18);
            this._btnGetMeasureData.Name = "_btnGetMeasureData";
            this._btnGetMeasureData.Size = new System.Drawing.Size(191, 23);
            this._btnGetMeasureData.TabIndex = 0;
            this._btnGetMeasureData.Text = "Get last measurement value";
            this._btnGetMeasureData.UseVisualStyleBackColor = true;
            this._btnGetMeasureData.Click += new System.EventHandler(this._btnGetMeasureData_Click);
            // 
            // btSaveMotr
            // 
            this.btSaveMotr.BackColor = System.Drawing.Color.Transparent;
            this.btSaveMotr.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btSaveMotr.Image = ((System.Drawing.Image)(resources.GetObject("btSaveMotr.Image")));
            this.btSaveMotr.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSaveMotr.Location = new System.Drawing.Point(556, 19);
            this.btSaveMotr.Name = "btSaveMotr";
            this.btSaveMotr.Size = new System.Drawing.Size(94, 98);
            this.btSaveMotr.TabIndex = 82;
            this.btSaveMotr.Tag = "5";
            this.btSaveMotr.Text = "SAVE";
            this.btSaveMotr.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btSaveMotr.UseVisualStyleBackColor = false;
            this.btSaveMotr.Click += new System.EventHandler(this.btSaveMotr_Click);
            // 
            // FormKeyence
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 488);
            this.Controls.Add(this.btSaveMotr);
            this.Controls.Add(this._grpSettings);
            this.Controls.Add(this._grpControl);
            this.Controls.Add(this._grpDataStorage);
            this.Controls.Add(this._grpStatSamp);
            this.Controls.Add(this._grpGetData);
            this.Controls.Add(this._grpBaseOperation);
            this.Name = "FormKeyence";
            this.Text = "FormKeyence";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormKeyence_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormKeyence_FormClosed);
            this._grpBaseOperation.ResumeLayout(false);
            this._pnlCommunicationDevice.ResumeLayout(false);
            this._pnlCommunicationDevice.PerformLayout();
            this._grpEthernetSetting.ResumeLayout(false);
            this._grpEthernetSetting.PerformLayout();
            this._grpSettings.ResumeLayout(false);
            this._grpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numStoragePoints)).EndInit();
            this._grpControl.ResumeLayout(false);
            this._grpControl.PerformLayout();
            this._grpDataStorage.ResumeLayout(false);
            this._grpDataStorage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numGetStorageCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._numGetStorageStart)).EndInit();
            this._grpStatSamp.ResumeLayout(false);
            this._grpGetData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _grpBaseOperation;
        private System.Windows.Forms.Panel _pnlCommunicationDevice;
        private System.Windows.Forms.RadioButton _radUsb;
        private System.Windows.Forms.RadioButton _radEthernet;
        private System.Windows.Forms.GroupBox _grpEthernetSetting;
        private System.Windows.Forms.TextBox _txtIpFirstSegment;
        private System.Windows.Forms.TextBox _txtIpFourthSegment;
        private System.Windows.Forms.TextBox _txtIpSecondSegment;
        private System.Windows.Forms.TextBox _txtIpThirdSegment;
        private System.Windows.Forms.Label _lblIpSeparator3;
        private System.Windows.Forms.Label _lblIpSeparator2;
        private System.Windows.Forms.Label _lblIpSeparator1;
        private System.Windows.Forms.TextBox _txtPort;
        private System.Windows.Forms.Label _lblIpAddress;
        private System.Windows.Forms.Label _lblPort;
        private System.Windows.Forms.Button _btnEnd;
        private System.Windows.Forms.Button _btnStart;
        private System.Windows.Forms.GroupBox _grpSettings;
        private System.Windows.Forms.Label _lblStoragePoints;
        private System.Windows.Forms.Button _bntSetStoragePoints;
        private System.Windows.Forms.Button _bntGetStoragePoints;
        private System.Windows.Forms.NumericUpDown _numStoragePoints;
        private System.Windows.Forms.GroupBox _grpControl;
        private System.Windows.Forms.CheckBox _chkOut16;
        private System.Windows.Forms.CheckBox _chkOut12;
        private System.Windows.Forms.CheckBox _chkOut8;
        private System.Windows.Forms.CheckBox _chkOut4;
        private System.Windows.Forms.CheckBox _chkOut15;
        private System.Windows.Forms.CheckBox _chkOut11;
        private System.Windows.Forms.CheckBox _chkOut7;
        private System.Windows.Forms.CheckBox _chkOut3;
        private System.Windows.Forms.CheckBox _chkOut14;
        private System.Windows.Forms.CheckBox _chkOut10;
        private System.Windows.Forms.CheckBox _chkOut6;
        private System.Windows.Forms.CheckBox _chkOut2;
        private System.Windows.Forms.CheckBox _chkOut13;
        private System.Windows.Forms.CheckBox _chkOut9;
        private System.Windows.Forms.CheckBox _chkOut5;
        private System.Windows.Forms.CheckBox _chkOut1;
        private System.Windows.Forms.Button _btnTimingOff;
        private System.Windows.Forms.Button _btnTimingOn;
        private System.Windows.Forms.Button _btnAutoZeroOff;
        private System.Windows.Forms.Button _btnAutoZeroOn;
        private System.Windows.Forms.Button _btnReset;
        private System.Windows.Forms.GroupBox _grpDataStorage;
        private System.Windows.Forms.Button _btnClearStorage;
        private System.Windows.Forms.NumericUpDown _numGetStorageCount;
        private System.Windows.Forms.Label _lblGetStorageCount;
        private System.Windows.Forms.NumericUpDown _numGetStorageStart;
        private System.Windows.Forms.Label _lblGetStorageStart;
        private System.Windows.Forms.Button _btnGetStorageData;
        private System.Windows.Forms.Button _btnGetStorageStatus;
        private System.Windows.Forms.Button _btnStopStorage;
        private System.Windows.Forms.Button _btnStartStorage;
        private System.Windows.Forms.GroupBox _grpStatSamp;
        private System.Windows.Forms.Button _btnStatSampClear;
        private System.Windows.Forms.ListBox _lstStatChs;
        private System.Windows.Forms.Button _btnGetStatSamp;
        private System.Windows.Forms.Button _btnStatSampStop;
        private System.Windows.Forms.Button _btnStatSampStart;
        private System.Windows.Forms.GroupBox _grpGetData;
        private System.Windows.Forms.Button _btnGetMeasureData;
        public  System.Windows.Forms.Button btSaveMotr;
    }
}