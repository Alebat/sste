namespace WS_STE
{
    partial class MainForm
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
            this.panelData = new System.Windows.Forms.Panel();
            this.radioButtonFemale = new System.Windows.Forms.RadioButton();
            this.radioButtonMale = new System.Windows.Forms.RadioButton();
            this.maskedTextBoxBirthDate = new System.Windows.Forms.MaskedTextBox();
            this.buttonNext = new System.Windows.Forms.Button();
            this.textBoxNotes = new System.Windows.Forms.TextBox();
            this.textBoxPlace = new System.Windows.Forms.TextBox();
            this.textBoxSurname = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.panelNotice = new System.Windows.Forms.Panel();
            this.labelNotice = new System.Windows.Forms.Label();
            this.panelSound = new System.Windows.Forms.Panel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.timerSpacebar = new System.Windows.Forms.Timer(this.components);
            this.panelFix = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panelRating = new System.Windows.Forms.Panel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.panelConnect = new System.Windows.Forms.Panel();
            this.panelBattery = new System.Windows.Forms.Panel();
            this.panelBatCharge = new System.Windows.Forms.Panel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelQuality = new System.Windows.Forms.Label();
            this.panelImgConn = new System.Windows.Forms.Panel();
            this.panelData.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.panelNotice.SuspendLayout();
            this.panelSound.SuspendLayout();
            this.panelFix.SuspendLayout();
            this.panelRating.SuspendLayout();
            this.panelConnect.SuspendLayout();
            this.panelBattery.SuspendLayout();
            this.panelBatCharge.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelData
            // 
            this.panelData.Controls.Add(this.radioButtonFemale);
            this.panelData.Controls.Add(this.radioButtonMale);
            this.panelData.Controls.Add(this.maskedTextBoxBirthDate);
            this.panelData.Controls.Add(this.buttonNext);
            this.panelData.Controls.Add(this.textBoxNotes);
            this.panelData.Controls.Add(this.textBoxPlace);
            this.panelData.Controls.Add(this.textBoxSurname);
            this.panelData.Controls.Add(this.textBoxName);
            this.panelData.Controls.Add(this.label7);
            this.panelData.Controls.Add(this.label6);
            this.panelData.Controls.Add(this.label8);
            this.panelData.Controls.Add(this.label5);
            this.panelData.Controls.Add(this.label4);
            this.panelData.Controls.Add(this.label3);
            this.panelData.Controls.Add(this.label1);
            this.panelData.Enabled = false;
            this.panelData.Location = new System.Drawing.Point(367, 12);
            this.panelData.Name = "panelData";
            this.panelData.Size = new System.Drawing.Size(354, 259);
            this.panelData.TabIndex = 0;
            this.panelData.Visible = false;
            this.panelData.EnabledChanged += new System.EventHandler(this.panelData_EnabledChanged);
            // 
            // radioButtonFemale
            // 
            this.radioButtonFemale.AutoSize = true;
            this.radioButtonFemale.Location = new System.Drawing.Point(184, 124);
            this.radioButtonFemale.Name = "radioButtonFemale";
            this.radioButtonFemale.Size = new System.Drawing.Size(31, 17);
            this.radioButtonFemale.TabIndex = 7;
            this.radioButtonFemale.TabStop = true;
            this.radioButtonFemale.Text = "F";
            this.radioButtonFemale.UseVisualStyleBackColor = true;
            // 
            // radioButtonMale
            // 
            this.radioButtonMale.AutoSize = true;
            this.radioButtonMale.Location = new System.Drawing.Point(93, 124);
            this.radioButtonMale.Name = "radioButtonMale";
            this.radioButtonMale.Size = new System.Drawing.Size(34, 17);
            this.radioButtonMale.TabIndex = 6;
            this.radioButtonMale.TabStop = true;
            this.radioButtonMale.Text = "M";
            this.radioButtonMale.UseVisualStyleBackColor = true;
            // 
            // maskedTextBoxBirthDate
            // 
            this.maskedTextBoxBirthDate.Location = new System.Drawing.Point(93, 97);
            this.maskedTextBoxBirthDate.Mask = "00/00/0000";
            this.maskedTextBoxBirthDate.Name = "maskedTextBoxBirthDate";
            this.maskedTextBoxBirthDate.Size = new System.Drawing.Size(256, 20);
            this.maskedTextBoxBirthDate.TabIndex = 5;
            this.maskedTextBoxBirthDate.ValidatingType = typeof(System.DateTime);
            // 
            // buttonNext
            // 
            this.buttonNext.FlatAppearance.BorderSize = 2;
            this.buttonNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonNext.Location = new System.Drawing.Point(290, 0);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(61, 24);
            this.buttonNext.TabIndex = 10;
            this.buttonNext.Text = "Next >";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // textBoxNotes
            // 
            this.textBoxNotes.AcceptsReturn = true;
            this.textBoxNotes.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxNotes.Location = new System.Drawing.Point(93, 181);
            this.textBoxNotes.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxNotes.Multiline = true;
            this.textBoxNotes.Name = "textBoxNotes";
            this.textBoxNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxNotes.Size = new System.Drawing.Size(256, 59);
            this.textBoxNotes.TabIndex = 9;
            // 
            // textBoxPlace
            // 
            this.textBoxPlace.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxPlace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxPlace.Location = new System.Drawing.Point(93, 151);
            this.textBoxPlace.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxPlace.Name = "textBoxPlace";
            this.textBoxPlace.Size = new System.Drawing.Size(256, 20);
            this.textBoxPlace.TabIndex = 8;
            // 
            // textBoxSurname
            // 
            this.textBoxSurname.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxSurname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSurname.Location = new System.Drawing.Point(93, 68);
            this.textBoxSurname.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxSurname.Name = "textBoxSurname";
            this.textBoxSurname.Size = new System.Drawing.Size(256, 20);
            this.textBoxSurname.TabIndex = 4;
            // 
            // textBoxName
            // 
            this.textBoxName.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxName.Location = new System.Drawing.Point(93, 43);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxName.Multiline = true;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(256, 20);
            this.textBoxName.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(5, 181);
            this.label7.Margin = new System.Windows.Forms.Padding(5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "Notes:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 154);
            this.label6.Margin = new System.Windows.Forms.Padding(5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "Place:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(5, 124);
            this.label8.Margin = new System.Windows.Forms.Padding(5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "Gender:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(5, 98);
            this.label5.Margin = new System.Windows.Forms.Padding(5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Date of Birth:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(5, 72);
            this.label4.Margin = new System.Windows.Forms.Padding(5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Surname:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 44);
            this.label3.Margin = new System.Windows.Forms.Padding(5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Name:";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(354, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "Experiment\'s data";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelTitle
            // 
            this.panelTitle.Controls.Add(this.buttonExit);
            this.panelTitle.Controls.Add(this.buttonNew);
            this.panelTitle.Controls.Add(this.label10);
            this.panelTitle.Enabled = false;
            this.panelTitle.Location = new System.Drawing.Point(12, 12);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(337, 206);
            this.panelTitle.TabIndex = 0;
            this.panelTitle.Visible = false;
            this.panelTitle.EnabledChanged += new System.EventHandler(this.panelStart_EnabledChanged);
            // 
            // buttonExit
            // 
            this.buttonExit.FlatAppearance.BorderSize = 2;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonExit.Location = new System.Drawing.Point(120, 128);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(85, 29);
            this.buttonExit.TabIndex = 2;
            this.buttonExit.Text = "Exit (Alt + F4)";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonNew
            // 
            this.buttonNew.FlatAppearance.BorderSize = 2;
            this.buttonNew.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonNew.Location = new System.Drawing.Point(110, 56);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(105, 48);
            this.buttonNew.TabIndex = 1;
            this.buttonNew.Text = "New";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Top;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Margin = new System.Windows.Forms.Padding(5);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(337, 34);
            this.label10.TabIndex = 0;
            this.label10.Text = "Stress Sound Experiment";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelNotice
            // 
            this.panelNotice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.panelNotice.Controls.Add(this.labelNotice);
            this.panelNotice.Enabled = false;
            this.panelNotice.Location = new System.Drawing.Point(16, 224);
            this.panelNotice.Name = "panelNotice";
            this.panelNotice.Size = new System.Drawing.Size(336, 212);
            this.panelNotice.TabIndex = 1;
            this.panelNotice.Visible = false;
            this.panelNotice.EnabledChanged += new System.EventHandler(this.panelNotice_EnabledChanged);
            // 
            // labelNotice
            // 
            this.labelNotice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.labelNotice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNotice.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotice.Location = new System.Drawing.Point(0, 0);
            this.labelNotice.Name = "labelNotice";
            this.labelNotice.Size = new System.Drawing.Size(336, 212);
            this.labelNotice.TabIndex = 0;
            this.labelNotice.Text = "Qui appariranno gli avvisi informativi per l\'utente.";
            this.labelNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelSound
            // 
            this.panelSound.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelSound.Controls.Add(this.linkLabel2);
            this.panelSound.Enabled = false;
            this.panelSound.Location = new System.Drawing.Point(504, 301);
            this.panelSound.Name = "panelSound";
            this.panelSound.Size = new System.Drawing.Size(123, 95);
            this.panelSound.TabIndex = 1;
            this.panelSound.Visible = false;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Enabled = false;
            this.linkLabel2.Location = new System.Drawing.Point(39, 41);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(46, 13);
            this.linkLabel2.TabIndex = 0;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "SOUND";
            this.linkLabel2.Visible = false;
            // 
            // timerSpacebar
            // 
            this.timerSpacebar.Interval = 5000;
            this.timerSpacebar.Tick += new System.EventHandler(this.timerSpacebar_Tick);
            // 
            // panelFix
            // 
            this.panelFix.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelFix.Controls.Add(this.linkLabel1);
            this.panelFix.Enabled = false;
            this.panelFix.Location = new System.Drawing.Point(367, 301);
            this.panelFix.Name = "panelFix";
            this.panelFix.Size = new System.Drawing.Size(131, 95);
            this.panelFix.TabIndex = 1;
            this.panelFix.Visible = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Enabled = false;
            this.linkLabel1.Location = new System.Drawing.Point(52, 41);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(23, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "FIX";
            this.linkLabel1.Visible = false;
            // 
            // panelRating
            // 
            this.panelRating.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelRating.Controls.Add(this.linkLabel3);
            this.panelRating.Enabled = false;
            this.panelRating.Location = new System.Drawing.Point(638, 301);
            this.panelRating.Name = "panelRating";
            this.panelRating.Size = new System.Drawing.Size(126, 95);
            this.panelRating.TabIndex = 1;
            this.panelRating.Visible = false;
            this.panelRating.EnabledChanged += new System.EventHandler(this.panelRating_EnabledChanged);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Enabled = false;
            this.linkLabel3.Location = new System.Drawing.Point(39, 41);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(48, 13);
            this.linkLabel3.TabIndex = 0;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "RATING";
            this.linkLabel3.Visible = false;
            // 
            // panelConnect
            // 
            this.panelConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.panelConnect.Controls.Add(this.panelBattery);
            this.panelConnect.Controls.Add(this.label2);
            this.panelConnect.Controls.Add(this.labelQuality);
            this.panelConnect.Controls.Add(this.panelImgConn);
            this.panelConnect.Enabled = false;
            this.panelConnect.Location = new System.Drawing.Point(727, 12);
            this.panelConnect.Name = "panelConnect";
            this.panelConnect.Size = new System.Drawing.Size(333, 206);
            this.panelConnect.TabIndex = 1;
            this.panelConnect.Visible = false;
            this.panelConnect.EnabledChanged += new System.EventHandler(this.panelConnect_EnabledChanged);
            // 
            // panelBattery
            // 
            this.panelBattery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBattery.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelBattery.Controls.Add(this.panelBatCharge);
            this.panelBattery.Controls.Add(this.linkLabel4);
            this.panelBattery.Location = new System.Drawing.Point(264, 3);
            this.panelBattery.Name = "panelBattery";
            this.panelBattery.Size = new System.Drawing.Size(66, 85);
            this.panelBattery.TabIndex = 2;
            this.panelBattery.Visible = false;
            // 
            // panelBatCharge
            // 
            this.panelBatCharge.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelBatCharge.Controls.Add(this.linkLabel5);
            this.panelBatCharge.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBatCharge.Location = new System.Drawing.Point(0, 0);
            this.panelBatCharge.Name = "panelBatCharge";
            this.panelBatCharge.Size = new System.Drawing.Size(66, 30);
            this.panelBatCharge.TabIndex = 1;
            // 
            // linkLabel5
            // 
            this.linkLabel5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Enabled = false;
            this.linkLabel5.Location = new System.Drawing.Point(8, 10);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(52, 13);
            this.linkLabel5.TabIndex = 0;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "CHARGE";
            this.linkLabel5.Visible = false;
            // 
            // linkLabel4
            // 
            this.linkLabel4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Enabled = false;
            this.linkLabel4.Location = new System.Drawing.Point(3, 47);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(57, 13);
            this.linkLabel4.TabIndex = 0;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "BATTERY";
            this.linkLabel4.Visible = false;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(333, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Barra Spaziatrice quando OK";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelQuality
            // 
            this.labelQuality.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQuality.Location = new System.Drawing.Point(0, 171);
            this.labelQuality.Name = "labelQuality";
            this.labelQuality.Size = new System.Drawing.Size(333, 35);
            this.labelQuality.TabIndex = 1;
            this.labelQuality.Text = "Signal Quality %";
            this.labelQuality.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelImgConn
            // 
            this.panelImgConn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panelImgConn.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelImgConn.Location = new System.Drawing.Point(0, 0);
            this.panelImgConn.Name = "panelImgConn";
            this.panelImgConn.Size = new System.Drawing.Size(333, 149);
            this.panelImgConn.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1072, 461);
            this.Controls.Add(this.panelRating);
            this.Controls.Add(this.panelFix);
            this.Controls.Add(this.panelSound);
            this.Controls.Add(this.panelConnect);
            this.Controls.Add(this.panelNotice);
            this.Controls.Add(this.panelTitle);
            this.Controls.Add(this.panelData);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.panelData.ResumeLayout(false);
            this.panelData.PerformLayout();
            this.panelTitle.ResumeLayout(false);
            this.panelNotice.ResumeLayout(false);
            this.panelSound.ResumeLayout(false);
            this.panelSound.PerformLayout();
            this.panelFix.ResumeLayout(false);
            this.panelFix.PerformLayout();
            this.panelRating.ResumeLayout(false);
            this.panelRating.PerformLayout();
            this.panelConnect.ResumeLayout(false);
            this.panelBattery.ResumeLayout(false);
            this.panelBattery.PerformLayout();
            this.panelBatCharge.ResumeLayout(false);
            this.panelBatCharge.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelData;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.TextBox textBoxNotes;
        private System.Windows.Forms.TextBox textBoxPlace;
        private System.Windows.Forms.TextBox textBoxSurname;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panelNotice;
        private System.Windows.Forms.Label labelNotice;
        private System.Windows.Forms.Panel panelSound;
        private System.Windows.Forms.Timer timerSpacebar;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Panel panelFix;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panelRating;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.Panel panelConnect;
        private System.Windows.Forms.Label labelQuality;
        private System.Windows.Forms.Panel panelImgConn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelBattery;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.Panel panelBatCharge;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxBirthDate;
        private System.Windows.Forms.RadioButton radioButtonFemale;
        private System.Windows.Forms.RadioButton radioButtonMale;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;

    }
}