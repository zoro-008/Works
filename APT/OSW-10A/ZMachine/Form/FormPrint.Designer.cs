namespace Machine
{
    partial class FormPrint
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblPause = new System.Windows.Forms.Label();
            this.lblLabelWaitFlag = new System.Windows.Forms.Label();
            this.lblRibionOut = new System.Windows.Forms.Label();
            this.lblHeadUp = new System.Windows.Forms.Label();
            this.lblPaperOut = new System.Windows.Forms.Label();
            this.textBoxDesc = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxQty = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxLotId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown1Toff = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMatNo = new System.Windows.Forms.TextBox();
            this.textBoxType = new System.Windows.Forms.TextBox();
            this.textBoxBinNo = new System.Windows.Forms.TextBox();
            this.textBoxTrayId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonSendData = new System.Windows.Forms.Button();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.tbYOffset = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lbStep = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1Toff)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblPause);
            this.groupBox1.Controls.Add(this.lblLabelWaitFlag);
            this.groupBox1.Controls.Add(this.lblRibionOut);
            this.groupBox1.Controls.Add(this.lblHeadUp);
            this.groupBox1.Controls.Add(this.lblPaperOut);
            this.groupBox1.Location = new System.Drawing.Point(10, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 81);
            this.groupBox1.TabIndex = 277;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Zebra Printer Current Status";
            // 
            // lblPause
            // 
            this.lblPause.AutoSize = true;
            this.lblPause.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPause.Location = new System.Drawing.Point(28, 24);
            this.lblPause.Name = "lblPause";
            this.lblPause.Size = new System.Drawing.Size(89, 19);
            this.lblPause.TabIndex = 266;
            this.lblPause.Text = "Pause flag";
            // 
            // lblLabelWaitFlag
            // 
            this.lblLabelWaitFlag.AutoSize = true;
            this.lblLabelWaitFlag.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lblLabelWaitFlag.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLabelWaitFlag.Location = new System.Drawing.Point(167, 51);
            this.lblLabelWaitFlag.Name = "lblLabelWaitFlag";
            this.lblLabelWaitFlag.Size = new System.Drawing.Size(119, 19);
            this.lblLabelWaitFlag.TabIndex = 265;
            this.lblLabelWaitFlag.Text = "Label Wait flag";
            // 
            // lblRibionOut
            // 
            this.lblRibionOut.AutoSize = true;
            this.lblRibionOut.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRibionOut.Location = new System.Drawing.Point(28, 51);
            this.lblRibionOut.Name = "lblRibionOut";
            this.lblRibionOut.Size = new System.Drawing.Size(122, 19);
            this.lblRibionOut.TabIndex = 263;
            this.lblRibionOut.Text = "Ribion Out flag";
            // 
            // lblHeadUp
            // 
            this.lblHeadUp.AutoSize = true;
            this.lblHeadUp.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeadUp.Location = new System.Drawing.Point(309, 24);
            this.lblHeadUp.Name = "lblHeadUp";
            this.lblHeadUp.Size = new System.Drawing.Size(107, 19);
            this.lblHeadUp.TabIndex = 262;
            this.lblHeadUp.Text = "Head Up flag";
            // 
            // lblPaperOut
            // 
            this.lblPaperOut.AutoSize = true;
            this.lblPaperOut.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaperOut.Location = new System.Drawing.Point(167, 24);
            this.lblPaperOut.Name = "lblPaperOut";
            this.lblPaperOut.Size = new System.Drawing.Size(117, 19);
            this.lblPaperOut.TabIndex = 261;
            this.lblPaperOut.Text = "Paper Out flag";
            // 
            // textBoxDesc
            // 
            this.textBoxDesc.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDesc.Location = new System.Drawing.Point(131, 437);
            this.textBoxDesc.Name = "textBoxDesc";
            this.textBoxDesc.Size = new System.Drawing.Size(163, 32);
            this.textBoxDesc.TabIndex = 301;
            this.textBoxDesc.Text = "R/FAIL OPTIC";
            this.textBoxDesc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(10, 437);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 29);
            this.label8.TabIndex = 300;
            this.label8.Text = "Desc";
            // 
            // textBoxQty
            // 
            this.textBoxQty.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxQty.Location = new System.Drawing.Point(131, 359);
            this.textBoxQty.Name = "textBoxQty";
            this.textBoxQty.Size = new System.Drawing.Size(163, 32);
            this.textBoxQty.TabIndex = 299;
            this.textBoxQty.Text = "000";
            this.textBoxQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(10, 359);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 29);
            this.label7.TabIndex = 298;
            this.label7.Text = "Qty";
            // 
            // textBoxLotId
            // 
            this.textBoxLotId.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLotId.Location = new System.Drawing.Point(131, 320);
            this.textBoxLotId.Name = "textBoxLotId";
            this.textBoxLotId.Size = new System.Drawing.Size(163, 32);
            this.textBoxLotId.TabIndex = 297;
            this.textBoxLotId.Text = "111111";
            this.textBoxLotId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(10, 320);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 29);
            this.label6.TabIndex = 296;
            this.label6.Text = "LOT ID";
            // 
            // numericUpDown1Toff
            // 
            this.numericUpDown1Toff.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1Toff.Location = new System.Drawing.Point(131, 476);
            this.numericUpDown1Toff.Maximum = new decimal(new int[] {
            125,
            0,
            0,
            0});
            this.numericUpDown1Toff.Minimum = new decimal(new int[] {
            125,
            0,
            0,
            -2147483648});
            this.numericUpDown1Toff.Name = "numericUpDown1Toff";
            this.numericUpDown1Toff.Size = new System.Drawing.Size(58, 32);
            this.numericUpDown1Toff.TabIndex = 295;
            this.numericUpDown1Toff.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 476);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 29);
            this.label1.TabIndex = 294;
            this.label1.Text = "Toff";
            // 
            // textBoxMatNo
            // 
            this.textBoxMatNo.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMatNo.Location = new System.Drawing.Point(131, 281);
            this.textBoxMatNo.Name = "textBoxMatNo";
            this.textBoxMatNo.Size = new System.Drawing.Size(163, 32);
            this.textBoxMatNo.TabIndex = 293;
            this.textBoxMatNo.Text = "000001";
            this.textBoxMatNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxType
            // 
            this.textBoxType.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxType.Location = new System.Drawing.Point(131, 242);
            this.textBoxType.Name = "textBoxType";
            this.textBoxType.Size = new System.Drawing.Size(163, 32);
            this.textBoxType.TabIndex = 292;
            this.textBoxType.Text = "Type";
            this.textBoxType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxBinNo
            // 
            this.textBoxBinNo.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxBinNo.Location = new System.Drawing.Point(131, 398);
            this.textBoxBinNo.Name = "textBoxBinNo";
            this.textBoxBinNo.Size = new System.Drawing.Size(163, 32);
            this.textBoxBinNo.TabIndex = 291;
            this.textBoxBinNo.Text = "0";
            this.textBoxBinNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxTrayId
            // 
            this.textBoxTrayId.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTrayId.Location = new System.Drawing.Point(131, 203);
            this.textBoxTrayId.Name = "textBoxTrayId";
            this.textBoxTrayId.Size = new System.Drawing.Size(163, 32);
            this.textBoxTrayId.TabIndex = 290;
            this.textBoxTrayId.Text = "Tray_Id";
            this.textBoxTrayId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 281);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 29);
            this.label3.TabIndex = 289;
            this.label3.Text = "Mat#";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 242);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 29);
            this.label2.TabIndex = 288;
            this.label2.Text = "Type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(10, 398);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 29);
            this.label5.TabIndex = 287;
            this.label5.Text = "Bin No";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 203);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 29);
            this.label4.TabIndex = 286;
            this.label4.Text = "Tray Id";
            // 
            // buttonSendData
            // 
            this.buttonSendData.Location = new System.Drawing.Point(15, 548);
            this.buttonSendData.Name = "buttonSendData";
            this.buttonSendData.Size = new System.Drawing.Size(204, 57);
            this.buttonSendData.TabIndex = 302;
            this.buttonSendData.Text = "Send Data";
            this.buttonSendData.UseVisualStyleBackColor = true;
            this.buttonSendData.Click += new System.EventHandler(this.buttonSendData_Click);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // tbYOffset
            // 
            this.tbYOffset.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbYOffset.Location = new System.Drawing.Point(131, 161);
            this.tbYOffset.Name = "tbYOffset";
            this.tbYOffset.Size = new System.Drawing.Size(163, 32);
            this.tbYOffset.TabIndex = 304;
            this.tbYOffset.Text = "48";
            this.tbYOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(10, 161);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(130, 29);
            this.label9.TabIndex = 303;
            this.label9.Text = "Y OFFSET";
            // 
            // lbStep
            // 
            this.lbStep.AutoSize = true;
            this.lbStep.Location = new System.Drawing.Point(26, 114);
            this.lbStep.Name = "lbStep";
            this.lbStep.Size = new System.Drawing.Size(41, 13);
            this.lbStep.TabIndex = 305;
            this.lbStep.Text = "label10";
            // 
            // FormPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 623);
            this.Controls.Add(this.lbStep);
            this.Controls.Add(this.tbYOffset);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.buttonSendData);
            this.Controls.Add(this.textBoxDesc);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxQty);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxLotId);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDown1Toff);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxMatNo);
            this.Controls.Add(this.textBoxType);
            this.Controls.Add(this.textBoxBinNo);
            this.Controls.Add(this.textBoxTrayId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormPrint";
            this.Text = "FormPrint";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1Toff)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblPause;
        private System.Windows.Forms.Label lblLabelWaitFlag;
        private System.Windows.Forms.Label lblRibionOut;
        private System.Windows.Forms.Label lblHeadUp;
        private System.Windows.Forms.Label lblPaperOut;
        private System.Windows.Forms.TextBox textBoxDesc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxQty;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxLotId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown1Toff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMatNo;
        private System.Windows.Forms.TextBox textBoxType;
        private System.Windows.Forms.TextBox textBoxBinNo;
        private System.Windows.Forms.TextBox textBoxTrayId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonSendData;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.TextBox tbYOffset;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbStep;
    }
}

