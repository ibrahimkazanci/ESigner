namespace SignatureHelper
{
    partial class Main
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
            this.label1 = new System.Windows.Forms.Label();
            this.tBoxERecetePath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rbString = new System.Windows.Forms.RadioButton();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.btnAddParalelSignature = new System.Windows.Forms.Button();
            this.btnAddSerialSignature = new System.Windows.Forms.Button();
            this.btnAddTimeStamp = new System.Windows.Forms.Button();
            this.btnVerify = new System.Windows.Forms.Button();
            this.lbSertifikaSahibi = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tmrPinKoduTemizle = new System.Windows.Forms.Timer(this.components);
            this.tmrGiris = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cbDosyayaYaz = new System.Windows.Forms.CheckBox();
            this.lblHedef = new System.Windows.Forms.Label();
            this.tBoxSignedERecetePath = new System.Windows.Forms.TextBox();
            this.lbTCKimlikNo = new System.Windows.Forms.Label();
            this.txtXML = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbPinKodu = new System.Windows.Forms.TextBox();
            this.btnEReceteImzala = new System.Windows.Forms.Button();
            this.lbKartTipi = new System.Windows.Forms.Label();
            this.txtBilgi = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnImzalaSQL = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(15, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Kaynak: E-Reçete Dosyası";
            // 
            // tBoxERecetePath
            // 
            this.tBoxERecetePath.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tBoxERecetePath.Location = new System.Drawing.Point(18, 117);
            this.tBoxERecetePath.Name = "tBoxERecetePath";
            this.tBoxERecetePath.ReadOnly = true;
            this.tBoxERecetePath.Size = new System.Drawing.Size(277, 20);
            this.tBoxERecetePath.TabIndex = 2;
            this.tBoxERecetePath.Text = "eRecete.xml";
            this.tBoxERecetePath.TextChanged += new System.EventHandler(this.tBoxERecetePath_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbFile);
            this.groupBox1.Controls.Add(this.rbString);
            this.groupBox1.Controls.Add(this.btnSelectFile);
            this.groupBox1.Controls.Add(this.tBoxERecetePath);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 146);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Reçete İmzalama İşlemi ";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // rbFile
            // 
            this.rbFile.AutoSize = true;
            this.rbFile.Checked = true;
            this.rbFile.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbFile.Location = new System.Drawing.Point(20, 19);
            this.rbFile.Name = "rbFile";
            this.rbFile.Size = new System.Drawing.Size(209, 19);
            this.rbFile.TabIndex = 13;
            this.rbFile.TabStop = true;
            this.rbFile.Text = "Kaynak Olarak; Bir Dosyayı Kullan";
            this.rbFile.UseVisualStyleBackColor = true;
            this.rbFile.CheckedChanged += new System.EventHandler(this.rbFile_CheckedChanged);
            // 
            // rbString
            // 
            this.rbString.AutoSize = true;
            this.rbString.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbString.Location = new System.Drawing.Point(20, 42);
            this.rbString.Name = "rbString";
            this.rbString.Size = new System.Drawing.Size(247, 19);
            this.rbString.TabIndex = 12;
            this.rbString.Text = "Kaynak Olarak; Gelen String Veriyi Kullan";
            this.rbString.UseVisualStyleBackColor = true;
            this.rbString.CheckedChanged += new System.EventHandler(this.rbString_CheckedChanged);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSelectFile.Location = new System.Drawing.Point(301, 115);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(30, 24);
            this.btnSelectFile.TabIndex = 3;
            this.btnSelectFile.Text = "...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // btnAddParalelSignature
            // 
            this.btnAddParalelSignature.Enabled = false;
            this.btnAddParalelSignature.Location = new System.Drawing.Point(242, 344);
            this.btnAddParalelSignature.Name = "btnAddParalelSignature";
            this.btnAddParalelSignature.Size = new System.Drawing.Size(66, 47);
            this.btnAddParalelSignature.TabIndex = 9;
            this.btnAddParalelSignature.Text = "Paralel İmzala";
            this.btnAddParalelSignature.UseVisualStyleBackColor = true;
            this.btnAddParalelSignature.Visible = false;
            this.btnAddParalelSignature.Click += new System.EventHandler(this.btnAddParalelSignature_Click);
            // 
            // btnAddSerialSignature
            // 
            this.btnAddSerialSignature.Enabled = false;
            this.btnAddSerialSignature.Location = new System.Drawing.Point(176, 344);
            this.btnAddSerialSignature.Name = "btnAddSerialSignature";
            this.btnAddSerialSignature.Size = new System.Drawing.Size(60, 47);
            this.btnAddSerialSignature.TabIndex = 8;
            this.btnAddSerialSignature.Text = "Seri İmza Ekle";
            this.btnAddSerialSignature.UseVisualStyleBackColor = true;
            this.btnAddSerialSignature.Visible = false;
            this.btnAddSerialSignature.Click += new System.EventHandler(this.btnAddSerialSignature_Click);
            // 
            // btnAddTimeStamp
            // 
            this.btnAddTimeStamp.Enabled = false;
            this.btnAddTimeStamp.Location = new System.Drawing.Point(76, 344);
            this.btnAddTimeStamp.Name = "btnAddTimeStamp";
            this.btnAddTimeStamp.Size = new System.Drawing.Size(94, 47);
            this.btnAddTimeStamp.TabIndex = 7;
            this.btnAddTimeStamp.Text = "Zaman Damgalı İmzaya Dönüştür";
            this.btnAddTimeStamp.UseVisualStyleBackColor = true;
            this.btnAddTimeStamp.Visible = false;
            this.btnAddTimeStamp.Click += new System.EventHandler(this.btnAddTimeStamp_Click);
            // 
            // btnVerify
            // 
            this.btnVerify.Location = new System.Drawing.Point(12, 344);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(58, 47);
            this.btnVerify.TabIndex = 6;
            this.btnVerify.Text = "Doğrula";
            this.btnVerify.UseVisualStyleBackColor = true;
            this.btnVerify.Visible = false;
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // lbSertifikaSahibi
            // 
            this.lbSertifikaSahibi.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.lbSertifikaSahibi.Location = new System.Drawing.Point(412, 19);
            this.lbSertifikaSahibi.Name = "lbSertifikaSahibi";
            this.lbSertifikaSahibi.Size = new System.Drawing.Size(379, 139);
            this.lbSertifikaSahibi.TabIndex = 13;
            this.lbSertifikaSahibi.Text = "Sertifika ve Sahiplik Bilgisi: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(12, 436);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(445, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Bu imza 5070 sayılı elektronik imza kanununa göre güvenli elektronik imzadır.";
            // 
            // tmrPinKoduTemizle
            // 
            this.tmrPinKoduTemizle.Interval = 30000;
            this.tmrPinKoduTemizle.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tmrGiris
            // 
            this.tmrGiris.Interval = 500;
            this.tmrGiris.Tick += new System.EventHandler(this.tmrGiris_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 420);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(667, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "PIN kodunu girdikten sonra 30 s. içerisinde İmzalama işlemini yapınız. Aksi takdi" +
    "rde güvenliğiniz için PIN kodunu tekrar girmeniz gerekecektir.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(408, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(281, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "İmzalama İşleminde Kullanılacak Sertifika Bilgisi:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.cbDosyayaYaz);
            this.groupBox2.Controls.Add(this.lblHedef);
            this.groupBox2.Controls.Add(this.tBoxSignedERecetePath);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.groupBox2.Location = new System.Drawing.Point(12, 222);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(386, 117);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " İmzalanmış Reçete ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label6.Location = new System.Drawing.Point(281, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "\"c:\\imzali.xml\" gibi...";
            this.label6.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.NavajoWhite;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.textBox1.Location = new System.Drawing.Point(11, 18);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(362, 26);
            this.textBox1.TabIndex = 22;
            this.textBox1.Text = "İmzalanmış veri parametre olarak geri döndürülecektir. İsterseniz alttaki seçeneğ" +
    "i işaretleyip, imzalanmış veriyi dosyaya da saklayabilirsiniz.";
            // 
            // cbDosyayaYaz
            // 
            this.cbDosyayaYaz.AutoSize = true;
            this.cbDosyayaYaz.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cbDosyayaYaz.Location = new System.Drawing.Point(19, 50);
            this.cbDosyayaYaz.Name = "cbDosyayaYaz";
            this.cbDosyayaYaz.Size = new System.Drawing.Size(196, 19);
            this.cbDosyayaYaz.TabIndex = 13;
            this.cbDosyayaYaz.Text = "İmzalanan reçeteyi dosyaya yaz";
            this.cbDosyayaYaz.UseVisualStyleBackColor = true;
            // 
            // lblHedef
            // 
            this.lblHedef.AutoSize = true;
            this.lblHedef.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblHedef.Location = new System.Drawing.Point(15, 70);
            this.lblHedef.Name = "lblHedef";
            this.lblHedef.Size = new System.Drawing.Size(356, 15);
            this.lblHedef.TabIndex = 12;
            this.lblHedef.Text = "BES İmzalı E-Reçetenin Kaydedileceği Dosyanın Adı ve Konumu";
            this.lblHedef.Visible = false;
            // 
            // tBoxSignedERecetePath
            // 
            this.tBoxSignedERecetePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tBoxSignedERecetePath.Location = new System.Drawing.Point(16, 87);
            this.tBoxSignedERecetePath.Name = "tBoxSignedERecetePath";
            this.tBoxSignedERecetePath.Size = new System.Drawing.Size(257, 20);
            this.tBoxSignedERecetePath.TabIndex = 11;
            this.tBoxSignedERecetePath.Visible = false;
            // 
            // lbTCKimlikNo
            // 
            this.lbTCKimlikNo.AccessibleName = "lbTCKimlikNo";
            this.lbTCKimlikNo.AutoSize = true;
            this.lbTCKimlikNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbTCKimlikNo.Location = new System.Drawing.Point(15, 400);
            this.lbTCKimlikNo.Name = "lbTCKimlikNo";
            this.lbTCKimlikNo.Size = new System.Drawing.Size(71, 13);
            this.lbTCKimlikNo.TabIndex = 17;
            this.lbTCKimlikNo.Text = "TC Kimlik No:";
            this.lbTCKimlikNo.Visible = false;
            // 
            // txtXML
            // 
            this.txtXML.BackColor = System.Drawing.Color.Cornsilk;
            this.txtXML.Location = new System.Drawing.Point(411, 175);
            this.txtXML.Multiline = true;
            this.txtXML.Name = "txtXML";
            this.txtXML.ReadOnly = true;
            this.txtXML.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtXML.Size = new System.Drawing.Size(380, 164);
            this.txtXML.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label7.Location = new System.Drawing.Point(408, 162);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "İmzalanacak Veri:";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button2.Location = new System.Drawing.Point(321, 170);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 39);
            this.button2.TabIndex = 28;
            this.button2.Text = "Kart Listele";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_2);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button1.Location = new System.Drawing.Point(240, 170);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 39);
            this.button1.TabIndex = 27;
            this.button1.Text = "Kartları Kaydet";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(22, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 15);
            this.label4.TabIndex = 26;
            this.label4.Text = "PIN Kodu";
            // 
            // tbPinKodu
            // 
            this.tbPinKodu.AcceptsReturn = true;
            this.tbPinKodu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbPinKodu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tbPinKodu.Location = new System.Drawing.Point(23, 186);
            this.tbPinKodu.Name = "tbPinKodu";
            this.tbPinKodu.PasswordChar = '*';
            this.tbPinKodu.Size = new System.Drawing.Size(64, 21);
            this.tbPinKodu.TabIndex = 24;
            // 
            // btnEReceteImzala
            // 
            this.btnEReceteImzala.BackColor = System.Drawing.Color.Coral;
            this.btnEReceteImzala.Font = new System.Drawing.Font("Arial Black", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnEReceteImzala.Location = new System.Drawing.Point(100, 170);
            this.btnEReceteImzala.Name = "btnEReceteImzala";
            this.btnEReceteImzala.Size = new System.Drawing.Size(123, 39);
            this.btnEReceteImzala.TabIndex = 25;
            this.btnEReceteImzala.Text = "İmzala";
            this.btnEReceteImzala.UseVisualStyleBackColor = false;
            this.btnEReceteImzala.Click += new System.EventHandler(this.btnEReceteImzala_Click);
            // 
            // lbKartTipi
            // 
            this.lbKartTipi.AccessibleName = "lbTCKimlikNo";
            this.lbKartTipi.AutoSize = true;
            this.lbKartTipi.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbKartTipi.Location = new System.Drawing.Point(214, 400);
            this.lbKartTipi.Name = "lbKartTipi";
            this.lbKartTipi.Size = new System.Drawing.Size(49, 13);
            this.lbKartTipi.TabIndex = 29;
            this.lbKartTipi.Text = "Kart Tipi:";
            this.lbKartTipi.Visible = false;
            // 
            // txtBilgi
            // 
            this.txtBilgi.BackColor = System.Drawing.Color.Cornsilk;
            this.txtBilgi.Location = new System.Drawing.Point(411, 353);
            this.txtBilgi.Multiline = true;
            this.txtBilgi.Name = "txtBilgi";
            this.txtBilgi.ReadOnly = true;
            this.txtBilgi.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBilgi.Size = new System.Drawing.Size(380, 60);
            this.txtBilgi.TabIndex = 30;
            this.txtBilgi.TextChanged += new System.EventHandler(this.txtBilgi_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label8.Location = new System.Drawing.Point(409, 340);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "Bilgi:";
            // 
            // btnImzalaSQL
            // 
            this.btnImzalaSQL.BackColor = System.Drawing.Color.Coral;
            this.btnImzalaSQL.Font = new System.Drawing.Font("Arial Black", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnImzalaSQL.Location = new System.Drawing.Point(314, 352);
            this.btnImzalaSQL.Name = "btnImzalaSQL";
            this.btnImzalaSQL.Size = new System.Drawing.Size(81, 39);
            this.btnImzalaSQL.TabIndex = 32;
            this.btnImzalaSQL.Text = "İmzala";
            this.btnImzalaSQL.UseVisualStyleBackColor = false;
            this.btnImzalaSQL.Visible = false;
            this.btnImzalaSQL.Click += new System.EventHandler(this.btnImzalaSQL_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 452);
            this.Controls.Add(this.btnEReceteImzala);
            this.Controls.Add(this.btnImzalaSQL);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtBilgi);
            this.Controls.Add(this.lbKartTipi);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbPinKodu);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtXML);
            this.Controls.Add(this.lbTCKimlikNo);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnVerify);
            this.Controls.Add(this.btnAddParalelSignature);
            this.Controls.Add(this.btnAddSerialSignature);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnAddTimeStamp);
            this.Controls.Add(this.lbSertifikaSahibi);
            this.Controls.Add(this.groupBox1);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Healthy HBYS E-İmza Yazılımı   -   bilgi@hbys.web.tr  /  ibrahimkazanci@gmail.com" +
    "  /  505 376 54 02";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tBoxERecetePath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Button btnVerify;
        private System.Windows.Forms.Button btnAddTimeStamp;
        private System.Windows.Forms.Button btnAddParalelSignature;
        private System.Windows.Forms.Button btnAddSerialSignature;
        private System.Windows.Forms.Label lbSertifikaSahibi;
        private System.Windows.Forms.RadioButton rbFile;
        private System.Windows.Forms.RadioButton rbString;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer tmrPinKoduTemizle;
        private System.Windows.Forms.Timer tmrGiris;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox cbDosyayaYaz;
        private System.Windows.Forms.Label lblHedef;
        private System.Windows.Forms.TextBox tBoxSignedERecetePath;
        private System.Windows.Forms.Label lbTCKimlikNo;
        public System.Windows.Forms.TextBox txtXML;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbPinKodu;
        private System.Windows.Forms.Button btnEReceteImzala;
        private System.Windows.Forms.Label lbKartTipi;
        public System.Windows.Forms.TextBox txtBilgi;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnImzalaSQL;
    }
}

