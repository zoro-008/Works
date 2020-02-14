namespace Control
{
    partial class FormMotion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMotion));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pgMotrPara = new System.Windows.Forms.PropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.S = new System.Windows.Forms.PropertyGrid();
            this.label3 = new System.Windows.Forms.Label();
            this.lbTrgPos = new System.Windows.Forms.Label();
            this.lbEncPos = new System.Windows.Forms.Label();
            this.lbCmdPos = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.lbStop = new System.Windows.Forms.Label();
            this.lbInpos = new System.Windows.Forms.Label();
            this.lbServo = new System.Windows.Forms.Label();
            this.lbHomeDone = new System.Windows.Forms.Label();
            this.lbBreakOff = new System.Windows.Forms.Label();
            this.lbAlram = new System.Windows.Forms.Label();
            this.lbDirP = new System.Windows.Forms.Label();
            this.lbDirN = new System.Windows.Forms.Label();
            this.lbZPhase = new System.Windows.Forms.Label();
            this.lbLimitP = new System.Windows.Forms.Label();
            this.lbHome = new System.Windows.Forms.Label();
            this.lbLimitN = new System.Windows.Forms.Label();
            this.btServoOffAll = new System.Windows.Forms.Button();
            this.btServoOnAll = new System.Windows.Forms.Button();
            this.btServoOff = new System.Windows.Forms.Button();
            this.btServoOn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btAllStop = new System.Windows.Forms.Button();
            this.btHome = new System.Windows.Forms.Button();
            this.btClearPos = new System.Windows.Forms.Button();
            this.btReset = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.btSaveMotr = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnMotionIO = new System.Windows.Forms.Panel();
            this.panel23 = new System.Windows.Forms.Panel();
            this.lbY5 = new System.Windows.Forms.Label();
            this.lbY4 = new System.Windows.Forms.Label();
            this.lbY3 = new System.Windows.Forms.Label();
            this.lbY2 = new System.Windows.Forms.Label();
            this.lbY1 = new System.Windows.Forms.Label();
            this.lbY = new System.Windows.Forms.Label();
            this.panel22 = new System.Windows.Forms.Panel();
            this.lbX5 = new System.Windows.Forms.Label();
            this.lbX4 = new System.Windows.Forms.Label();
            this.lbX3 = new System.Windows.Forms.Label();
            this.lbX2 = new System.Windows.Forms.Label();
            this.lbX1 = new System.Windows.Forms.Label();
            this.lbX = new System.Windows.Forms.Label();
            this.btRepeat = new System.Windows.Forms.Button();
            this.btStop2 = new System.Windows.Forms.Button();
            this.btGo2ndPos = new System.Windows.Forms.Button();
            this.btGo1stPos = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btJogP = new System.Windows.Forms.Button();
            this.btJogN = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnMotionIO.SuspendLayout();
            this.panel23.SuspendLayout();
            this.panel22.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1243, 29);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Z MOTOR SETTING";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pgMotrPara);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(363, 738);
            this.panel2.TabIndex = 3;
            // 
            // pgMotrPara
            // 
            this.pgMotrPara.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgMotrPara.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pgMotrPara.HelpVisible = false;
            this.pgMotrPara.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgMotrPara.Location = new System.Drawing.Point(0, 28);
            this.pgMotrPara.Name = "pgMotrPara";
            this.pgMotrPara.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgMotrPara.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.pgMotrPara.Size = new System.Drawing.Size(363, 710);
            this.pgMotrPara.TabIndex = 6;
            this.pgMotrPara.ToolbarVisible = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.DimGray;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(363, 28);
            this.label1.TabIndex = 5;
            this.label1.Text = "MOTOR PARAMETER";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.S);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(363, 29);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(363, 738);
            this.panel3.TabIndex = 20;
            // 
            // S
            // 
            this.S.Dock = System.Windows.Forms.DockStyle.Fill;
            this.S.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.S.HelpVisible = false;
            this.S.LineColor = System.Drawing.SystemColors.ControlDark;
            this.S.Location = new System.Drawing.Point(0, 28);
            this.S.Name = "S";
            this.S.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.S.Size = new System.Drawing.Size(363, 710);
            this.S.TabIndex = 6;
            this.S.ToolbarVisible = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.DimGray;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(363, 28);
            this.label3.TabIndex = 5;
            this.label3.Text = "MOTOR SUB PARAMETER";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbTrgPos
            // 
            this.lbTrgPos.BackColor = System.Drawing.Color.Silver;
            this.lbTrgPos.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbTrgPos.Location = new System.Drawing.Point(862, 210);
            this.lbTrgPos.Name = "lbTrgPos";
            this.lbTrgPos.Size = new System.Drawing.Size(370, 24);
            this.lbTrgPos.TabIndex = 80;
            this.lbTrgPos.Text = "0.0";
            this.lbTrgPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbEncPos
            // 
            this.lbEncPos.BackColor = System.Drawing.Color.Silver;
            this.lbEncPos.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbEncPos.Location = new System.Drawing.Point(862, 181);
            this.lbEncPos.Name = "lbEncPos";
            this.lbEncPos.Size = new System.Drawing.Size(370, 24);
            this.lbEncPos.TabIndex = 79;
            this.lbEncPos.Text = "0.0";
            this.lbEncPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCmdPos
            // 
            this.lbCmdPos.BackColor = System.Drawing.Color.Silver;
            this.lbCmdPos.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbCmdPos.Location = new System.Drawing.Point(862, 152);
            this.lbCmdPos.Name = "lbCmdPos";
            this.lbCmdPos.Size = new System.Drawing.Size(370, 24);
            this.lbCmdPos.TabIndex = 78;
            this.lbCmdPos.Text = "0.0";
            this.lbCmdPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label54
            // 
            this.label54.BackColor = System.Drawing.Color.LightGray;
            this.label54.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label54.Location = new System.Drawing.Point(738, 210);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(120, 24);
            this.label54.TabIndex = 77;
            this.label54.Text = "TRG POS";
            this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label53
            // 
            this.label53.BackColor = System.Drawing.Color.LightGray;
            this.label53.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label53.Location = new System.Drawing.Point(738, 181);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(120, 24);
            this.label53.TabIndex = 76;
            this.label53.Text = "ENC POS";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label52
            // 
            this.label52.BackColor = System.Drawing.Color.LightGray;
            this.label52.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label52.Location = new System.Drawing.Point(738, 152);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(120, 24);
            this.label52.TabIndex = 75;
            this.label52.Text = "CMD POS";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStop
            // 
            this.lbStop.BackColor = System.Drawing.Color.Silver;
            this.lbStop.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbStop.Location = new System.Drawing.Point(1112, 123);
            this.lbStop.Name = "lbStop";
            this.lbStop.Size = new System.Drawing.Size(120, 24);
            this.lbStop.TabIndex = 74;
            this.lbStop.Text = "STOP";
            this.lbStop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInpos
            // 
            this.lbInpos.BackColor = System.Drawing.Color.Silver;
            this.lbInpos.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbInpos.Location = new System.Drawing.Point(987, 123);
            this.lbInpos.Name = "lbInpos";
            this.lbInpos.Size = new System.Drawing.Size(120, 24);
            this.lbInpos.TabIndex = 73;
            this.lbInpos.Text = "INPOS";
            this.lbInpos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbServo
            // 
            this.lbServo.BackColor = System.Drawing.Color.Silver;
            this.lbServo.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbServo.Location = new System.Drawing.Point(862, 123);
            this.lbServo.Name = "lbServo";
            this.lbServo.Size = new System.Drawing.Size(120, 24);
            this.lbServo.TabIndex = 72;
            this.lbServo.Text = "SERVO";
            this.lbServo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbHomeDone
            // 
            this.lbHomeDone.BackColor = System.Drawing.Color.Silver;
            this.lbHomeDone.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbHomeDone.Location = new System.Drawing.Point(737, 123);
            this.lbHomeDone.Name = "lbHomeDone";
            this.lbHomeDone.Size = new System.Drawing.Size(120, 24);
            this.lbHomeDone.TabIndex = 71;
            this.lbHomeDone.Text = "HOME DONE";
            this.lbHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbBreakOff
            // 
            this.lbBreakOff.BackColor = System.Drawing.Color.Silver;
            this.lbBreakOff.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbBreakOff.Location = new System.Drawing.Point(1112, 94);
            this.lbBreakOff.Name = "lbBreakOff";
            this.lbBreakOff.Size = new System.Drawing.Size(120, 24);
            this.lbBreakOff.TabIndex = 70;
            this.lbBreakOff.Text = "BREAK OFF";
            this.lbBreakOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbAlram
            // 
            this.lbAlram.BackColor = System.Drawing.Color.Silver;
            this.lbAlram.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbAlram.Location = new System.Drawing.Point(987, 94);
            this.lbAlram.Name = "lbAlram";
            this.lbAlram.Size = new System.Drawing.Size(120, 24);
            this.lbAlram.TabIndex = 69;
            this.lbAlram.Text = "ALRAM";
            this.lbAlram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDirP
            // 
            this.lbDirP.BackColor = System.Drawing.Color.Silver;
            this.lbDirP.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbDirP.Location = new System.Drawing.Point(862, 94);
            this.lbDirP.Name = "lbDirP";
            this.lbDirP.Size = new System.Drawing.Size(119, 24);
            this.lbDirP.TabIndex = 68;
            this.lbDirP.Text = "DIR +";
            this.lbDirP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDirN
            // 
            this.lbDirN.BackColor = System.Drawing.Color.Silver;
            this.lbDirN.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbDirN.Location = new System.Drawing.Point(737, 94);
            this.lbDirN.Name = "lbDirN";
            this.lbDirN.Size = new System.Drawing.Size(120, 24);
            this.lbDirN.TabIndex = 67;
            this.lbDirN.Text = "DIR -";
            this.lbDirN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbZPhase
            // 
            this.lbZPhase.BackColor = System.Drawing.Color.Silver;
            this.lbZPhase.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbZPhase.Location = new System.Drawing.Point(1112, 65);
            this.lbZPhase.Name = "lbZPhase";
            this.lbZPhase.Size = new System.Drawing.Size(120, 24);
            this.lbZPhase.TabIndex = 66;
            this.lbZPhase.Text = "Z PHASE";
            this.lbZPhase.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLimitP
            // 
            this.lbLimitP.BackColor = System.Drawing.Color.Silver;
            this.lbLimitP.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbLimitP.Location = new System.Drawing.Point(987, 65);
            this.lbLimitP.Name = "lbLimitP";
            this.lbLimitP.Size = new System.Drawing.Size(120, 24);
            this.lbLimitP.TabIndex = 65;
            this.lbLimitP.Text = "LIMIT +";
            this.lbLimitP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbHome
            // 
            this.lbHome.BackColor = System.Drawing.Color.Silver;
            this.lbHome.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbHome.Location = new System.Drawing.Point(862, 65);
            this.lbHome.Name = "lbHome";
            this.lbHome.Size = new System.Drawing.Size(120, 24);
            this.lbHome.TabIndex = 64;
            this.lbHome.Text = "HOME";
            this.lbHome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLimitN
            // 
            this.lbLimitN.BackColor = System.Drawing.Color.Silver;
            this.lbLimitN.Font = new System.Drawing.Font("맑은 고딕", 10F);
            this.lbLimitN.Location = new System.Drawing.Point(737, 65);
            this.lbLimitN.Name = "lbLimitN";
            this.lbLimitN.Size = new System.Drawing.Size(120, 24);
            this.lbLimitN.TabIndex = 63;
            this.lbLimitN.Text = "LIMIT -";
            this.lbLimitN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btServoOffAll
            // 
            this.btServoOffAll.BackColor = System.Drawing.Color.Transparent;
            this.btServoOffAll.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btServoOffAll.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btServoOffAll.Location = new System.Drawing.Point(902, 517);
            this.btServoOffAll.Name = "btServoOffAll";
            this.btServoOffAll.Size = new System.Drawing.Size(165, 63);
            this.btServoOffAll.TabIndex = 60;
            this.btServoOffAll.Tag = "5";
            this.btServoOffAll.Text = "SERVO OFF ALL";
            this.btServoOffAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btServoOffAll.UseVisualStyleBackColor = false;
            this.btServoOffAll.Click += new System.EventHandler(this.btServoOffAll_Click);
            // 
            // btServoOnAll
            // 
            this.btServoOnAll.BackColor = System.Drawing.Color.Transparent;
            this.btServoOnAll.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btServoOnAll.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btServoOnAll.Location = new System.Drawing.Point(731, 517);
            this.btServoOnAll.Name = "btServoOnAll";
            this.btServoOnAll.Size = new System.Drawing.Size(165, 63);
            this.btServoOnAll.TabIndex = 59;
            this.btServoOnAll.Tag = "5";
            this.btServoOnAll.Text = "SERVO ON ALL";
            this.btServoOnAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btServoOnAll.UseVisualStyleBackColor = false;
            this.btServoOnAll.Click += new System.EventHandler(this.btServoOnAll_Click);
            // 
            // btServoOff
            // 
            this.btServoOff.BackColor = System.Drawing.Color.Transparent;
            this.btServoOff.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btServoOff.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btServoOff.Location = new System.Drawing.Point(902, 448);
            this.btServoOff.Name = "btServoOff";
            this.btServoOff.Size = new System.Drawing.Size(165, 63);
            this.btServoOff.TabIndex = 57;
            this.btServoOff.Tag = "5";
            this.btServoOff.Text = "SERVO OFF";
            this.btServoOff.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btServoOff.UseVisualStyleBackColor = false;
            this.btServoOff.Click += new System.EventHandler(this.btServoOff_Click);
            // 
            // btServoOn
            // 
            this.btServoOn.BackColor = System.Drawing.Color.Transparent;
            this.btServoOn.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btServoOn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btServoOn.Location = new System.Drawing.Point(731, 448);
            this.btServoOn.Name = "btServoOn";
            this.btServoOn.Size = new System.Drawing.Size(165, 63);
            this.btServoOn.TabIndex = 56;
            this.btServoOn.Tag = "5";
            this.btServoOn.Text = "SERVO ON";
            this.btServoOn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btServoOn.UseVisualStyleBackColor = false;
            this.btServoOn.Click += new System.EventHandler(this.btServoOn_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.DimGray;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
            this.label5.Location = new System.Drawing.Point(732, 276);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(507, 28);
            this.label5.TabIndex = 62;
            this.label5.Text = "MOTOR CONTROL";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.DimGray;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
            this.label4.Location = new System.Drawing.Point(731, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(507, 28);
            this.label4.TabIndex = 61;
            this.label4.Text = "MOTOR STATUS";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btAllStop
            // 
            this.btAllStop.BackColor = System.Drawing.Color.Transparent;
            this.btAllStop.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btAllStop.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAllStop.Location = new System.Drawing.Point(1073, 448);
            this.btAllStop.Name = "btAllStop";
            this.btAllStop.Size = new System.Drawing.Size(165, 63);
            this.btAllStop.TabIndex = 58;
            this.btAllStop.Tag = "5";
            this.btAllStop.Text = "ALL STOP";
            this.btAllStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btAllStop.UseVisualStyleBackColor = false;
            this.btAllStop.Click += new System.EventHandler(this.btAllStop_Click);
            // 
            // btHome
            // 
            this.btHome.BackColor = System.Drawing.Color.Transparent;
            this.btHome.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btHome.Image = ((System.Drawing.Image)(resources.GetObject("btHome.Image")));
            this.btHome.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btHome.Location = new System.Drawing.Point(1073, 379);
            this.btHome.Name = "btHome";
            this.btHome.Size = new System.Drawing.Size(165, 63);
            this.btHome.TabIndex = 55;
            this.btHome.Tag = "5";
            this.btHome.Text = "HOME";
            this.btHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btHome.UseVisualStyleBackColor = false;
            this.btHome.Click += new System.EventHandler(this.btHome_Click);
            // 
            // btClearPos
            // 
            this.btClearPos.BackColor = System.Drawing.Color.Transparent;
            this.btClearPos.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btClearPos.Image = ((System.Drawing.Image)(resources.GetObject("btClearPos.Image")));
            this.btClearPos.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btClearPos.Location = new System.Drawing.Point(902, 379);
            this.btClearPos.Name = "btClearPos";
            this.btClearPos.Size = new System.Drawing.Size(165, 63);
            this.btClearPos.TabIndex = 54;
            this.btClearPos.Tag = "5";
            this.btClearPos.Text = "CLEAR POS";
            this.btClearPos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btClearPos.UseVisualStyleBackColor = false;
            this.btClearPos.Click += new System.EventHandler(this.btClearPos_Click);
            // 
            // btReset
            // 
            this.btReset.BackColor = System.Drawing.Color.Transparent;
            this.btReset.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btReset.Image = ((System.Drawing.Image)(resources.GetObject("btReset.Image")));
            this.btReset.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btReset.Location = new System.Drawing.Point(731, 379);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(165, 63);
            this.btReset.TabIndex = 53;
            this.btReset.Tag = "5";
            this.btReset.Text = "RESET";
            this.btReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btReset.UseVisualStyleBackColor = false;
            this.btReset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btReset_MouseDown);
            this.btReset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btReset_MouseUp);
            // 
            // btStop
            // 
            this.btStop.BackColor = System.Drawing.Color.Transparent;
            this.btStop.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btStop.Image = ((System.Drawing.Image)(resources.GetObject("btStop.Image")));
            this.btStop.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btStop.Location = new System.Drawing.Point(1073, 310);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(165, 63);
            this.btStop.TabIndex = 52;
            this.btStop.Tag = "5";
            this.btStop.Text = "STOP";
            this.btStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btStop.UseVisualStyleBackColor = false;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // btSaveMotr
            // 
            this.btSaveMotr.BackColor = System.Drawing.Color.Transparent;
            this.btSaveMotr.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btSaveMotr.Image = ((System.Drawing.Image)(resources.GetObject("btSaveMotr.Image")));
            this.btSaveMotr.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSaveMotr.Location = new System.Drawing.Point(1073, 691);
            this.btSaveMotr.Name = "btSaveMotr";
            this.btSaveMotr.Size = new System.Drawing.Size(166, 63);
            this.btSaveMotr.TabIndex = 81;
            this.btSaveMotr.Tag = "5";
            this.btSaveMotr.Text = "SAVE";
            this.btSaveMotr.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btSaveMotr.UseVisualStyleBackColor = false;
            this.btSaveMotr.Click += new System.EventHandler(this.btSaveMotr_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnMotionIO
            // 
            this.pnMotionIO.Controls.Add(this.panel23);
            this.pnMotionIO.Controls.Add(this.panel22);
            this.pnMotionIO.Location = new System.Drawing.Point(732, 237);
            this.pnMotionIO.Name = "pnMotionIO";
            this.pnMotionIO.Size = new System.Drawing.Size(507, 29);
            this.pnMotionIO.TabIndex = 82;
            // 
            // panel23
            // 
            this.panel23.Controls.Add(this.lbY5);
            this.panel23.Controls.Add(this.lbY4);
            this.panel23.Controls.Add(this.lbY3);
            this.panel23.Controls.Add(this.lbY2);
            this.panel23.Controls.Add(this.lbY1);
            this.panel23.Controls.Add(this.lbY);
            this.panel23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel23.Location = new System.Drawing.Point(253, 0);
            this.panel23.Name = "panel23";
            this.panel23.Padding = new System.Windows.Forms.Padding(3);
            this.panel23.Size = new System.Drawing.Size(254, 29);
            this.panel23.TabIndex = 54;
            // 
            // lbY5
            // 
            this.lbY5.BackColor = System.Drawing.Color.Silver;
            this.lbY5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbY5.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbY5.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbY5.Location = new System.Drawing.Point(208, 3);
            this.lbY5.Name = "lbY5";
            this.lbY5.Size = new System.Drawing.Size(40, 23);
            this.lbY5.TabIndex = 37;
            this.lbY5.Tag = "4";
            this.lbY5.Text = "5";
            this.lbY5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbY5.DoubleClick += new System.EventHandler(this.lbY1_DoubleClick);
            // 
            // lbY4
            // 
            this.lbY4.BackColor = System.Drawing.Color.Silver;
            this.lbY4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbY4.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbY4.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbY4.Location = new System.Drawing.Point(168, 3);
            this.lbY4.Name = "lbY4";
            this.lbY4.Size = new System.Drawing.Size(40, 23);
            this.lbY4.TabIndex = 36;
            this.lbY4.Tag = "3";
            this.lbY4.Text = "4";
            this.lbY4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbY4.DoubleClick += new System.EventHandler(this.lbY1_DoubleClick);
            // 
            // lbY3
            // 
            this.lbY3.BackColor = System.Drawing.Color.Silver;
            this.lbY3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbY3.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbY3.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbY3.Location = new System.Drawing.Point(128, 3);
            this.lbY3.Name = "lbY3";
            this.lbY3.Size = new System.Drawing.Size(40, 23);
            this.lbY3.TabIndex = 35;
            this.lbY3.Tag = "2";
            this.lbY3.Text = "3";
            this.lbY3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbY3.DoubleClick += new System.EventHandler(this.lbY1_DoubleClick);
            // 
            // lbY2
            // 
            this.lbY2.BackColor = System.Drawing.Color.Silver;
            this.lbY2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbY2.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbY2.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbY2.Location = new System.Drawing.Point(88, 3);
            this.lbY2.Name = "lbY2";
            this.lbY2.Size = new System.Drawing.Size(40, 23);
            this.lbY2.TabIndex = 34;
            this.lbY2.Tag = "1";
            this.lbY2.Text = "2";
            this.lbY2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbY2.DoubleClick += new System.EventHandler(this.lbY1_DoubleClick);
            // 
            // lbY1
            // 
            this.lbY1.BackColor = System.Drawing.Color.Silver;
            this.lbY1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbY1.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbY1.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbY1.Location = new System.Drawing.Point(48, 3);
            this.lbY1.Name = "lbY1";
            this.lbY1.Size = new System.Drawing.Size(40, 23);
            this.lbY1.TabIndex = 33;
            this.lbY1.Tag = "0";
            this.lbY1.Text = "1";
            this.lbY1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbY1.Click += new System.EventHandler(this.lbY1_Click);
            this.lbY1.DoubleClick += new System.EventHandler(this.lbY1_DoubleClick);
            // 
            // lbY
            // 
            this.lbY.BackColor = System.Drawing.Color.Silver;
            this.lbY.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbY.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbY.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbY.Location = new System.Drawing.Point(3, 3);
            this.lbY.Name = "lbY";
            this.lbY.Size = new System.Drawing.Size(45, 23);
            this.lbY.TabIndex = 32;
            this.lbY.Text = "Y";
            this.lbY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel22
            // 
            this.panel22.Controls.Add(this.lbX5);
            this.panel22.Controls.Add(this.lbX4);
            this.panel22.Controls.Add(this.lbX3);
            this.panel22.Controls.Add(this.lbX2);
            this.panel22.Controls.Add(this.lbX1);
            this.panel22.Controls.Add(this.lbX);
            this.panel22.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel22.Location = new System.Drawing.Point(0, 0);
            this.panel22.Name = "panel22";
            this.panel22.Padding = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.panel22.Size = new System.Drawing.Size(253, 29);
            this.panel22.TabIndex = 53;
            // 
            // lbX5
            // 
            this.lbX5.BackColor = System.Drawing.Color.Silver;
            this.lbX5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbX5.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbX5.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbX5.Location = new System.Drawing.Point(211, 3);
            this.lbX5.Name = "lbX5";
            this.lbX5.Size = new System.Drawing.Size(40, 23);
            this.lbX5.TabIndex = 37;
            this.lbX5.Tag = "4";
            this.lbX5.Text = "5";
            this.lbX5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbX4
            // 
            this.lbX4.BackColor = System.Drawing.Color.Silver;
            this.lbX4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbX4.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbX4.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbX4.Location = new System.Drawing.Point(171, 3);
            this.lbX4.Name = "lbX4";
            this.lbX4.Size = new System.Drawing.Size(40, 23);
            this.lbX4.TabIndex = 36;
            this.lbX4.Tag = "3";
            this.lbX4.Text = "4";
            this.lbX4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbX3
            // 
            this.lbX3.BackColor = System.Drawing.Color.Silver;
            this.lbX3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbX3.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbX3.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbX3.Location = new System.Drawing.Point(131, 3);
            this.lbX3.Name = "lbX3";
            this.lbX3.Size = new System.Drawing.Size(40, 23);
            this.lbX3.TabIndex = 35;
            this.lbX3.Tag = "2";
            this.lbX3.Text = "3";
            this.lbX3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbX2
            // 
            this.lbX2.BackColor = System.Drawing.Color.Silver;
            this.lbX2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbX2.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbX2.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbX2.Location = new System.Drawing.Point(91, 3);
            this.lbX2.Name = "lbX2";
            this.lbX2.Size = new System.Drawing.Size(40, 23);
            this.lbX2.TabIndex = 34;
            this.lbX2.Tag = "1";
            this.lbX2.Text = "2";
            this.lbX2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbX1
            // 
            this.lbX1.BackColor = System.Drawing.Color.Silver;
            this.lbX1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbX1.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbX1.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbX1.Location = new System.Drawing.Point(51, 3);
            this.lbX1.Name = "lbX1";
            this.lbX1.Size = new System.Drawing.Size(40, 23);
            this.lbX1.TabIndex = 33;
            this.lbX1.Tag = "0";
            this.lbX1.Text = "1";
            this.lbX1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbX
            // 
            this.lbX.BackColor = System.Drawing.Color.Silver;
            this.lbX.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbX.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbX.Font = new System.Drawing.Font("맑은 고딕", 11F);
            this.lbX.Location = new System.Drawing.Point(6, 3);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(45, 23);
            this.lbX.TabIndex = 32;
            this.lbX.Text = "X";
            this.lbX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btRepeat
            // 
            this.btRepeat.BackColor = System.Drawing.Color.Transparent;
            this.btRepeat.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btRepeat.Image = ((System.Drawing.Image)(resources.GetObject("btRepeat.Image")));
            this.btRepeat.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btRepeat.Location = new System.Drawing.Point(730, 691);
            this.btRepeat.Name = "btRepeat";
            this.btRepeat.Size = new System.Drawing.Size(165, 63);
            this.btRepeat.TabIndex = 87;
            this.btRepeat.Tag = "5";
            this.btRepeat.Text = "REPEAT";
            this.btRepeat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btRepeat.UseVisualStyleBackColor = false;
            this.btRepeat.Click += new System.EventHandler(this.btRepeat_Click);
            // 
            // btStop2
            // 
            this.btStop2.BackColor = System.Drawing.Color.Transparent;
            this.btStop2.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btStop2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btStop2.Location = new System.Drawing.Point(1072, 622);
            this.btStop2.Name = "btStop2";
            this.btStop2.Size = new System.Drawing.Size(165, 63);
            this.btStop2.TabIndex = 86;
            this.btStop2.Tag = "5";
            this.btStop2.Text = "STOP";
            this.btStop2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btStop2.UseVisualStyleBackColor = false;
            this.btStop2.Click += new System.EventHandler(this.btStop2_Click);
            // 
            // btGo2ndPos
            // 
            this.btGo2ndPos.BackColor = System.Drawing.Color.Transparent;
            this.btGo2ndPos.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btGo2ndPos.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGo2ndPos.Location = new System.Drawing.Point(901, 622);
            this.btGo2ndPos.Name = "btGo2ndPos";
            this.btGo2ndPos.Size = new System.Drawing.Size(165, 63);
            this.btGo2ndPos.TabIndex = 85;
            this.btGo2ndPos.Tag = "5";
            this.btGo2ndPos.Text = "GO 2ND POS";
            this.btGo2ndPos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btGo2ndPos.UseVisualStyleBackColor = false;
            this.btGo2ndPos.Click += new System.EventHandler(this.btGo2ndPos_Click);
            // 
            // btGo1stPos
            // 
            this.btGo1stPos.BackColor = System.Drawing.Color.Transparent;
            this.btGo1stPos.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btGo1stPos.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGo1stPos.Location = new System.Drawing.Point(730, 622);
            this.btGo1stPos.Name = "btGo1stPos";
            this.btGo1stPos.Size = new System.Drawing.Size(165, 63);
            this.btGo1stPos.TabIndex = 84;
            this.btGo1stPos.Tag = "5";
            this.btGo1stPos.Text = "GO 1ST POS";
            this.btGo1stPos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btGo1stPos.UseVisualStyleBackColor = false;
            this.btGo1stPos.Click += new System.EventHandler(this.btGo1stPos_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.DimGray;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
            this.label6.Location = new System.Drawing.Point(731, 591);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(507, 28);
            this.label6.TabIndex = 83;
            this.label6.Text = "MOTOR REPEAT CONTROL";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btJogP
            // 
            this.btJogP.BackColor = System.Drawing.Color.Transparent;
            this.btJogP.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btJogP.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btJogP.Location = new System.Drawing.Point(902, 310);
            this.btJogP.Name = "btJogP";
            this.btJogP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btJogP.Size = new System.Drawing.Size(165, 63);
            this.btJogP.TabIndex = 88;
            this.btJogP.Tag = "5";
            this.btJogP.Text = "DOWN JOG+";
            this.btJogP.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btJogP.UseVisualStyleBackColor = false;
            this.btJogP.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btJogP_MouseDown);
            this.btJogP.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btJogP_MouseUp);
            // 
            // btJogN
            // 
            this.btJogN.BackColor = System.Drawing.Color.Transparent;
            this.btJogN.Font = new System.Drawing.Font("맑은 고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btJogN.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btJogN.Location = new System.Drawing.Point(732, 310);
            this.btJogN.Name = "btJogN";
            this.btJogN.Size = new System.Drawing.Size(164, 63);
            this.btJogN.TabIndex = 89;
            this.btJogN.Tag = "5";
            this.btJogN.Text = "UP JOG-";
            this.btJogN.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btJogN.UseVisualStyleBackColor = false;
            this.btJogN.Click += new System.EventHandler(this.btJogN_Click);
            this.btJogN.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btJogN_MouseDown);
            this.btJogN.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btJogN_MouseUp);
            // 
            // FormMotion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1243, 767);
            this.Controls.Add(this.btJogN);
            this.Controls.Add(this.btJogP);
            this.Controls.Add(this.btRepeat);
            this.Controls.Add(this.btStop2);
            this.Controls.Add(this.btGo2ndPos);
            this.Controls.Add(this.btGo1stPos);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pnMotionIO);
            this.Controls.Add(this.btSaveMotr);
            this.Controls.Add(this.lbTrgPos);
            this.Controls.Add(this.lbEncPos);
            this.Controls.Add(this.lbCmdPos);
            this.Controls.Add(this.label54);
            this.Controls.Add(this.label53);
            this.Controls.Add(this.label52);
            this.Controls.Add(this.lbStop);
            this.Controls.Add(this.lbInpos);
            this.Controls.Add(this.lbServo);
            this.Controls.Add(this.lbHomeDone);
            this.Controls.Add(this.lbBreakOff);
            this.Controls.Add(this.lbAlram);
            this.Controls.Add(this.lbDirP);
            this.Controls.Add(this.lbDirN);
            this.Controls.Add(this.lbZPhase);
            this.Controls.Add(this.lbLimitP);
            this.Controls.Add(this.lbHome);
            this.Controls.Add(this.lbLimitN);
            this.Controls.Add(this.btServoOffAll);
            this.Controls.Add(this.btServoOnAll);
            this.Controls.Add(this.btServoOff);
            this.Controls.Add(this.btServoOn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btAllStop);
            this.Controls.Add(this.btHome);
            this.Controls.Add(this.btClearPos);
            this.Controls.Add(this.btReset);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FormMotion";
            this.Text = "FormMotion";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMotion_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FormMotion_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.pnMotionIO.ResumeLayout(false);
            this.panel23.ResumeLayout(false);
            this.panel22.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PropertyGrid pgMotrPara;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PropertyGrid S;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbTrgPos;
        private System.Windows.Forms.Label lbEncPos;
        private System.Windows.Forms.Label lbCmdPos;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label lbStop;
        private System.Windows.Forms.Label lbInpos;
        private System.Windows.Forms.Label lbServo;
        private System.Windows.Forms.Label lbHomeDone;
        private System.Windows.Forms.Label lbBreakOff;
        private System.Windows.Forms.Label lbAlram;
        private System.Windows.Forms.Label lbDirP;
        private System.Windows.Forms.Label lbDirN;
        private System.Windows.Forms.Label lbZPhase;
        private System.Windows.Forms.Label lbLimitP;
        private System.Windows.Forms.Label lbHome;
        private System.Windows.Forms.Label lbLimitN;
        private System.Windows.Forms.Button btServoOffAll;
        private System.Windows.Forms.Button btServoOnAll;
        private System.Windows.Forms.Button btServoOff;
        private System.Windows.Forms.Button btServoOn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btAllStop;
        private System.Windows.Forms.Button btHome;
        private System.Windows.Forms.Button btClearPos;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Button btSaveMotr;
        private System.Windows.Forms.Panel pnMotionIO;
        private System.Windows.Forms.Panel panel23;
        private System.Windows.Forms.Label lbY5;
        private System.Windows.Forms.Label lbY4;
        private System.Windows.Forms.Label lbY3;
        private System.Windows.Forms.Label lbY2;
        private System.Windows.Forms.Label lbY1;
        private System.Windows.Forms.Label lbY;
        private System.Windows.Forms.Panel panel22;
        private System.Windows.Forms.Label lbX5;
        private System.Windows.Forms.Label lbX4;
        private System.Windows.Forms.Label lbX3;
        private System.Windows.Forms.Label lbX2;
        private System.Windows.Forms.Label lbX1;
        private System.Windows.Forms.Label lbX;
        private System.Windows.Forms.Button btRepeat;
        private System.Windows.Forms.Button btStop2;
        private System.Windows.Forms.Button btGo2ndPos;
        private System.Windows.Forms.Button btGo1stPos;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btJogP;
        private System.Windows.Forms.Button btJogN;
    }
}