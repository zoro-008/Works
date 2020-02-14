namespace Machine
{
    partial class FormOperation
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOperation));
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.pnDataMap = new System.Windows.Forms.Panel();
            this.pnDownSide = new System.Windows.Forms.Panel();
            this.pnOper = new System.Windows.Forms.Panel();
            this.btOperator = new System.Windows.Forms.Button();
            this.btHome = new System.Windows.Forms.Button();
            this.btReset = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.btStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pnOperMan = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btAllHome = new System.Windows.Forms.Button();
            this.btManual1 = new System.Windows.Forms.Button();
            this.btManual3 = new System.Windows.Forms.Button();
            this.btManual2 = new System.Windows.Forms.Button();
            this.btManual6 = new System.Windows.Forms.Button();
            this.btManual4 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.tcVision = new System.Windows.Forms.TabControl();
            this.tcVisiond = new System.Windows.Forms.TabPage();
            this.pnVision1 = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnVision2 = new System.Windows.Forms.Panel();
            this.pnOperation = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnDataMap.SuspendLayout();
            this.pnDownSide.SuspendLayout();
            this.pnOper.SuspendLayout();
            this.pnOperMan.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tcVision.SuspendLayout();
            this.tcVisiond.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.pnOperation.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmUpdate
            // 
            this.tmUpdate.Interval = 30;
            this.tmUpdate.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnDataMap
            // 
            this.pnDataMap.BackColor = System.Drawing.SystemColors.Control;
            this.pnDataMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnDataMap.Controls.Add(this.pnDownSide);
            this.pnDataMap.Controls.Add(this.button4);
            this.pnDataMap.Controls.Add(this.button3);
            this.pnDataMap.Controls.Add(this.button2);
            this.pnDataMap.Controls.Add(this.button1);
            this.pnDataMap.Controls.Add(this.pnOperMan);
            this.pnDataMap.Controls.Add(this.tcVision);
            this.pnDataMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnDataMap.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pnDataMap.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnDataMap.Location = new System.Drawing.Point(0, 0);
            this.pnDataMap.Name = "pnDataMap";
            this.pnDataMap.Size = new System.Drawing.Size(1280, 844);
            this.pnDataMap.TabIndex = 10;
            // 
            // pnDownSide
            // 
            this.pnDownSide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnDownSide.Controls.Add(this.pnOper);
            this.pnDownSide.Location = new System.Drawing.Point(1035, 604);
            this.pnDownSide.Name = "pnDownSide";
            this.pnDownSide.Size = new System.Drawing.Size(250, 352);
            this.pnDownSide.TabIndex = 11;
            // 
            // pnOper
            // 
            this.pnOper.BackColor = System.Drawing.SystemColors.Control;
            this.pnOper.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnOper.Controls.Add(this.btOperator);
            this.pnOper.Controls.Add(this.btHome);
            this.pnOper.Controls.Add(this.btReset);
            this.pnOper.Controls.Add(this.btStop);
            this.pnOper.Controls.Add(this.btStart);
            this.pnOper.Controls.Add(this.label1);
            this.pnOper.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnOper.Location = new System.Drawing.Point(0, 0);
            this.pnOper.Name = "pnOper";
            this.pnOper.Size = new System.Drawing.Size(250, 352);
            this.pnOper.TabIndex = 18;
            // 
            // btOperator
            // 
            this.btOperator.BackColor = System.Drawing.SystemColors.Control;
            this.btOperator.Dock = System.Windows.Forms.DockStyle.Top;
            this.btOperator.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btOperator.ForeColor = System.Drawing.Color.DarkKhaki;
            this.btOperator.Image = ((System.Drawing.Image)(resources.GetObject("btOperator.Image")));
            this.btOperator.Location = new System.Drawing.Point(0, 284);
            this.btOperator.Name = "btOperator";
            this.btOperator.Size = new System.Drawing.Size(248, 65);
            this.btOperator.TabIndex = 8;
            this.btOperator.Text = "  LOG IN";
            this.btOperator.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btOperator.UseVisualStyleBackColor = false;
            this.btOperator.Click += new System.EventHandler(this.btOperator_Click_1);
            // 
            // btHome
            // 
            this.btHome.BackColor = System.Drawing.SystemColors.Control;
            this.btHome.Dock = System.Windows.Forms.DockStyle.Top;
            this.btHome.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btHome.ForeColor = System.Drawing.Color.LightSeaGreen;
            this.btHome.Location = new System.Drawing.Point(0, 219);
            this.btHome.Name = "btHome";
            this.btHome.Size = new System.Drawing.Size(248, 65);
            this.btHome.TabIndex = 7;
            this.btHome.Text = "  HOME";
            this.btHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btHome.UseVisualStyleBackColor = false;
            this.btHome.Click += new System.EventHandler(this.btAllHome_Click_1);
            // 
            // btReset
            // 
            this.btReset.BackColor = System.Drawing.SystemColors.Control;
            this.btReset.Dock = System.Windows.Forms.DockStyle.Top;
            this.btReset.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btReset.ForeColor = System.Drawing.Color.Gold;
            this.btReset.Image = ((System.Drawing.Image)(resources.GetObject("btReset.Image")));
            this.btReset.Location = new System.Drawing.Point(0, 154);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(248, 65);
            this.btReset.TabIndex = 6;
            this.btReset.Text = "  RESET";
            this.btReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btReset.UseVisualStyleBackColor = false;
            this.btReset.Click += new System.EventHandler(this.btReset_Click_1);
            // 
            // btStop
            // 
            this.btStop.BackColor = System.Drawing.SystemColors.Control;
            this.btStop.Dock = System.Windows.Forms.DockStyle.Top;
            this.btStop.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btStop.ForeColor = System.Drawing.Color.Red;
            this.btStop.Image = ((System.Drawing.Image)(resources.GetObject("btStop.Image")));
            this.btStop.Location = new System.Drawing.Point(0, 89);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(248, 65);
            this.btStop.TabIndex = 3;
            this.btStop.Text = "  STOP";
            this.btStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btStop.UseVisualStyleBackColor = false;
            this.btStop.Click += new System.EventHandler(this.btStop_Click_1);
            // 
            // btStart
            // 
            this.btStart.BackColor = System.Drawing.SystemColors.Control;
            this.btStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.btStart.Font = new System.Drawing.Font("맑은 고딕", 25F, System.Drawing.FontStyle.Bold);
            this.btStart.ForeColor = System.Drawing.Color.Green;
            this.btStart.Image = ((System.Drawing.Image)(resources.GetObject("btStart.Image")));
            this.btStart.Location = new System.Drawing.Point(0, 24);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(248, 65);
            this.btStart.TabIndex = 2;
            this.btStart.Text = "  START";
            this.btStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btStart.UseVisualStyleBackColor = false;
            this.btStart.Click += new System.EventHandler(this.btStart_Click_1);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightCoral;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "OPERATION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1109, 114);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 33);
            this.button4.TabIndex = 56;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1028, 114);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 33);
            this.button3.TabIndex = 55;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1109, 85);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 54;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button2_MouseDown);
            this.button2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button2_MouseUp);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1028, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 53;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button1_MouseDown);
            this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button1_MouseUp);
            // 
            // pnOperMan
            // 
            this.pnOperMan.AllowDrop = true;
            this.pnOperMan.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.pnOperMan.BackColor = System.Drawing.SystemColors.Control;
            this.pnOperMan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnOperMan.Controls.Add(this.tabControl1);
            this.pnOperMan.Controls.Add(this.label10);
            this.pnOperMan.Location = new System.Drawing.Point(1039, 132);
            this.pnOperMan.Name = "pnOperMan";
            this.pnOperMan.Size = new System.Drawing.Size(247, 352);
            this.pnOperMan.TabIndex = 52;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabControl1.ItemSize = new System.Drawing.Size(70, 30);
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(245, 326);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage1.Controls.Add(this.flowLayoutPanel1);
            this.tabPage1.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(237, 288);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "HOME";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btAllHome);
            this.flowLayoutPanel1.Controls.Add(this.btManual1);
            this.flowLayoutPanel1.Controls.Add(this.btManual3);
            this.flowLayoutPanel1.Controls.Add(this.btManual2);
            this.flowLayoutPanel1.Controls.Add(this.btManual6);
            this.flowLayoutPanel1.Controls.Add(this.btManual4);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(231, 282);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // btAllHome
            // 
            this.btAllHome.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btAllHome.Location = new System.Drawing.Point(1, 1);
            this.btAllHome.Margin = new System.Windows.Forms.Padding(1);
            this.btAllHome.Name = "btAllHome";
            this.btAllHome.Size = new System.Drawing.Size(230, 44);
            this.btAllHome.TabIndex = 21;
            this.btAllHome.Tag = "1";
            this.btAllHome.Text = "ALL HOME";
            this.btAllHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btAllHome.UseVisualStyleBackColor = true;
            this.btAllHome.Click += new System.EventHandler(this.btManual1_Click);
            // 
            // btManual1
            // 
            this.btManual1.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btManual1.Location = new System.Drawing.Point(1, 47);
            this.btManual1.Margin = new System.Windows.Forms.Padding(1);
            this.btManual1.Name = "btManual1";
            this.btManual1.Size = new System.Drawing.Size(230, 44);
            this.btManual1.TabIndex = 22;
            this.btManual1.Tag = "2";
            this.btManual1.Text = "One Shot Triger";
            this.btManual1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btManual1.UseVisualStyleBackColor = true;
            this.btManual1.Click += new System.EventHandler(this.btManual1_Click);
            // 
            // btManual3
            // 
            this.btManual3.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btManual3.Location = new System.Drawing.Point(1, 93);
            this.btManual3.Margin = new System.Windows.Forms.Padding(1);
            this.btManual3.Name = "btManual3";
            this.btManual3.Size = new System.Drawing.Size(230, 44);
            this.btManual3.TabIndex = 17;
            this.btManual3.Tag = "4";
            this.btManual3.Text = "Vision Zone Home";
            this.btManual3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btManual3.UseVisualStyleBackColor = true;
            this.btManual3.Visible = false;
            // 
            // btManual2
            // 
            this.btManual2.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btManual2.Location = new System.Drawing.Point(1, 139);
            this.btManual2.Margin = new System.Windows.Forms.Padding(1);
            this.btManual2.Name = "btManual2";
            this.btManual2.Size = new System.Drawing.Size(230, 44);
            this.btManual2.TabIndex = 16;
            this.btManual2.Tag = "3";
            this.btManual2.Text = "Marking Zone Home";
            this.btManual2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btManual2.UseVisualStyleBackColor = true;
            this.btManual2.Visible = false;
            // 
            // btManual6
            // 
            this.btManual6.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btManual6.Location = new System.Drawing.Point(1, 185);
            this.btManual6.Margin = new System.Windows.Forms.Padding(1);
            this.btManual6.Name = "btManual6";
            this.btManual6.Size = new System.Drawing.Size(230, 44);
            this.btManual6.TabIndex = 20;
            this.btManual6.Tag = "7";
            this.btManual6.Text = "Unloader Zone Home";
            this.btManual6.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btManual6.UseVisualStyleBackColor = true;
            this.btManual6.Visible = false;
            // 
            // btManual4
            // 
            this.btManual4.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.btManual4.Location = new System.Drawing.Point(1, 231);
            this.btManual4.Margin = new System.Windows.Forms.Padding(1);
            this.btManual4.Name = "btManual4";
            this.btManual4.Size = new System.Drawing.Size(230, 44);
            this.btManual4.TabIndex = 23;
            this.btManual4.Tag = "5";
            this.btManual4.Text = "Marking NG Home";
            this.btManual4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btManual4.UseVisualStyleBackColor = true;
            this.btManual4.Visible = false;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.LightCoral;
            this.label10.Dock = System.Windows.Forms.DockStyle.Top;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(245, 24);
            this.label10.TabIndex = 0;
            this.label10.Text = "OPERATION MANUAL";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tcVision
            // 
            this.tcVision.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tcVision.Controls.Add(this.tcVisiond);
            this.tcVision.Controls.Add(this.tabPage2);
            this.tcVision.Dock = System.Windows.Forms.DockStyle.Left;
            this.tcVision.Location = new System.Drawing.Point(0, 0);
            this.tcVision.Name = "tcVision";
            this.tcVision.SelectedIndex = 0;
            this.tcVision.Size = new System.Drawing.Size(1020, 842);
            this.tcVision.TabIndex = 51;
            // 
            // tcVisiond
            // 
            this.tcVisiond.BackColor = System.Drawing.SystemColors.Control;
            this.tcVisiond.Controls.Add(this.pnVision1);
            this.tcVisiond.Location = new System.Drawing.Point(4, 27);
            this.tcVisiond.Name = "tcVisiond";
            this.tcVisiond.Padding = new System.Windows.Forms.Padding(3);
            this.tcVisiond.Size = new System.Drawing.Size(1012, 811);
            this.tcVisiond.TabIndex = 1;
            this.tcVisiond.Text = "Vision1";
            // 
            // pnVision1
            // 
            this.pnVision1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnVision1.Location = new System.Drawing.Point(3, 3);
            this.pnVision1.Name = "pnVision1";
            this.pnVision1.Size = new System.Drawing.Size(1006, 805);
            this.pnVision1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pnVision2);
            this.tabPage2.Location = new System.Drawing.Point(4, 27);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1012, 811);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Vision2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnVision2
            // 
            this.pnVision2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnVision2.Location = new System.Drawing.Point(3, 3);
            this.pnVision2.Name = "pnVision2";
            this.pnVision2.Size = new System.Drawing.Size(1006, 805);
            this.pnVision2.TabIndex = 2;
            // 
            // pnOperation
            // 
            this.pnOperation.Controls.Add(this.pnDataMap);
            this.pnOperation.Location = new System.Drawing.Point(0, 0);
            this.pnOperation.Name = "pnOperation";
            this.pnOperation.Size = new System.Drawing.Size(1280, 844);
            this.pnOperation.TabIndex = 21;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // FormOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1280, 844);
            this.Controls.Add(this.pnOperation);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOperation";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "FormOperation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOperation_FormClosing);
            this.Shown += new System.EventHandler(this.FormOperation_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormOperation_VisibleChanged);
            this.pnDataMap.ResumeLayout(false);
            this.pnDownSide.ResumeLayout(false);
            this.pnOper.ResumeLayout(false);
            this.pnOperMan.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tcVision.ResumeLayout(false);
            this.tcVisiond.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.pnOperation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.Panel pnDataMap;
        private System.Windows.Forms.TabControl tcVision;
        private System.Windows.Forms.Panel pnOper;
        private System.Windows.Forms.Button btOperator;
        private System.Windows.Forms.Button btHome;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnOperation;
        private System.Windows.Forms.Panel pnDownSide;
        private System.Windows.Forms.TabPage tcVisiond;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnOperMan;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btAllHome;
        private System.Windows.Forms.Button btManual1;
        private System.Windows.Forms.Button btManual3;
        private System.Windows.Forms.Button btManual2;
        private System.Windows.Forms.Button btManual6;
        private System.Windows.Forms.Button btManual4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel pnVision1;
        private System.Windows.Forms.Panel pnVision2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
    }
}