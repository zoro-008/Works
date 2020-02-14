namespace SML2
{
    partial class FormPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPassword));
            this.pnChange = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNewPass = new System.Windows.Forms.TextBox();
            this.tbOldPass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btChange = new System.Windows.Forms.Button();
            this.btEnter = new System.Windows.Forms.Button();
            this.btEsc = new System.Windows.Forms.Button();
            this.pnInput = new System.Windows.Forms.Panel();
            this.lbInputPass = new System.Windows.Forms.Label();
            this.tbInputPass = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pnChange.SuspendLayout();
            this.pnInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnChange
            // 
            this.pnChange.BackColor = System.Drawing.SystemColors.Control;
            this.pnChange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnChange.Controls.Add(this.label3);
            this.pnChange.Controls.Add(this.label2);
            this.pnChange.Controls.Add(this.tbNewPass);
            this.pnChange.Controls.Add(this.tbOldPass);
            this.pnChange.Controls.Add(this.label1);
            this.pnChange.Location = new System.Drawing.Point(2, 4);
            this.pnChange.Name = "pnChange";
            this.pnChange.Size = new System.Drawing.Size(330, 108);
            this.pnChange.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.Navy;
            this.label3.Location = new System.Drawing.Point(3, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "New Password";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(3, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Old Password";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbNewPass
            // 
            this.tbNewPass.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbNewPass.Location = new System.Drawing.Point(122, 66);
            this.tbNewPass.Name = "tbNewPass";
            this.tbNewPass.Size = new System.Drawing.Size(200, 29);
            this.tbNewPass.TabIndex = 2;
            // 
            // tbOldPass
            // 
            this.tbOldPass.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbOldPass.Location = new System.Drawing.Point(122, 31);
            this.tbOldPass.Name = "tbOldPass";
            this.tbOldPass.Size = new System.Drawing.Size(200, 29);
            this.tbOldPass.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(-1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(330, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "   PASSWORD Change";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btChange
            // 
            this.btChange.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btChange.Image = ((System.Drawing.Image)(resources.GetObject("btChange.Image")));
            this.btChange.Location = new System.Drawing.Point(5, 115);
            this.btChange.Name = "btChange";
            this.btChange.Size = new System.Drawing.Size(105, 43);
            this.btChange.TabIndex = 1;
            this.btChange.Text = "Change";
            this.btChange.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btChange.UseVisualStyleBackColor = true;
            this.btChange.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btChange_MouseDown);
            // 
            // btEnter
            // 
            this.btEnter.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btEnter.Image = ((System.Drawing.Image)(resources.GetObject("btEnter.Image")));
            this.btEnter.Location = new System.Drawing.Point(115, 115);
            this.btEnter.Name = "btEnter";
            this.btEnter.Size = new System.Drawing.Size(105, 43);
            this.btEnter.TabIndex = 2;
            this.btEnter.Text = "Enter";
            this.btEnter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btEnter.UseVisualStyleBackColor = true;
            this.btEnter.Click += new System.EventHandler(this.btEnter_Click);
            // 
            // btEsc
            // 
            this.btEsc.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btEsc.Image = ((System.Drawing.Image)(resources.GetObject("btEsc.Image")));
            this.btEsc.Location = new System.Drawing.Point(224, 115);
            this.btEsc.Name = "btEsc";
            this.btEsc.Size = new System.Drawing.Size(105, 43);
            this.btEsc.TabIndex = 3;
            this.btEsc.Text = "Esc";
            this.btEsc.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btEsc.UseVisualStyleBackColor = true;
            this.btEsc.Click += new System.EventHandler(this.btEsc_Click);
            // 
            // pnInput
            // 
            this.pnInput.BackColor = System.Drawing.SystemColors.Control;
            this.pnInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnInput.Controls.Add(this.lbInputPass);
            this.pnInput.Controls.Add(this.tbInputPass);
            this.pnInput.Controls.Add(this.label6);
            this.pnInput.Location = new System.Drawing.Point(2, 4);
            this.pnInput.Name = "pnInput";
            this.pnInput.Size = new System.Drawing.Size(330, 108);
            this.pnInput.TabIndex = 5;
            // 
            // lbInputPass
            // 
            this.lbInputPass.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbInputPass.ForeColor = System.Drawing.Color.Navy;
            this.lbInputPass.Location = new System.Drawing.Point(14, 34);
            this.lbInputPass.Name = "lbInputPass";
            this.lbInputPass.Size = new System.Drawing.Size(278, 23);
            this.lbInputPass.TabIndex = 3;
            this.lbInputPass.Text = "INPUT PASSWORD";
            this.lbInputPass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbInputPass
            // 
            this.tbInputPass.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbInputPass.Location = new System.Drawing.Point(13, 59);
            this.tbInputPass.Name = "tbInputPass";
            this.tbInputPass.Size = new System.Drawing.Size(302, 29);
            this.tbInputPass.TabIndex = 1;
            this.tbInputPass.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbInputPass_KeyDown);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.Location = new System.Drawing.Point(-1, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(330, 23);
            this.label6.TabIndex = 0;
            this.label6.Text = "   PASSWORD Check";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(334, 162);
            this.Controls.Add(this.btEsc);
            this.Controls.Add(this.btEnter);
            this.Controls.Add(this.btChange);
            this.Controls.Add(this.pnInput);
            this.Controls.Add(this.pnChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormPassword";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FormPassword_Shown);
            this.pnChange.ResumeLayout(false);
            this.pnChange.PerformLayout();
            this.pnInput.ResumeLayout(false);
            this.pnInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnChange;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNewPass;
        private System.Windows.Forms.TextBox tbOldPass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btChange;
        private System.Windows.Forms.Button btEnter;
        private System.Windows.Forms.Button btEsc;
        private System.Windows.Forms.Panel pnInput;
        private System.Windows.Forms.Label lbInputPass;
        private System.Windows.Forms.TextBox tbInputPass;
        private System.Windows.Forms.Label label6;
    }
}