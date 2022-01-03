using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using tr.gov.tubitak.uekae.esya.api.smartcard.pkcs11;
using tr.gov.tubitak.uekae.esya.asn.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;
using utils;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using ProgramNameSpace;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using Context = tr.gov.tubitak.uekae.esya.api.xmlsignature.Context;
using XMLSignature = tr.gov.tubitak.uekae.esya.api.xmlsignature.XMLSignature;
using XMLSignatureException = tr.gov.tubitak.uekae.esya.api.xmlsignature.XMLSignatureException;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.signature;
using ContextCades = tr.gov.tubitak.uekae.esya.api.signature.Context;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using tr.gov.tubitak.uekae.esya.api.cades.example.validation;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using NUnit.Framework;
using System.Security.Cryptography;
using tr.gov.tubitak.uekae.esya.api.common.util;

//using tr.gov.tubitak.uekae.esya.api.xmlsignature;
// using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades;
//using api_smartcard.src.tr.gov.tubitak.uekae.esya.api.smartcard.winscard;

// oto parametreleri: "wwwww" "" "" "" "" "" "" "" "1" "12345" "11111111111" "2" "ibrahim kazancı" "127.0.0.1"
// kontroller sf 15
// sf 17 pin kodu degistirme imkani
// sf 20 pin bloke mesaji geliyor mu


namespace SignatureHelper
{
    //[comvisible(true)]

    //public interface UserGui
    //{
    //    string createEnvelopedBes(string configxml, String lisanspath, String pinNo, bool nitelikli, string inputXML, string inputXMLFilename, string outputXMLPath);
    //}

    public partial class Main : Form
    {  // dll embed icin
        Dictionary<string, Assembly> _libs = new Dictionary<string, Assembly>();
        public Main()
        {    // dll embed icin
             // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            InitializeComponent();
            //if (GetNetworkTime() > new DateTime(2016, 01, 05, 0, 0, 0, DateTimeKind.Utc))
            //{
            //    // 1;
            //}
            //else if (Program.himm == 1)
            //{ Program.himm = 0; }
            if (Program.ParamXML != "")
            {
                if (Program.ParamXML.StartsWith("FisTipi"))
                {
                    rbString.Checked = true;
                }
                else
                {
                    rbString.Checked = true;
                    try
                    {
                        txtXML.Text = System.Xml.Linq.XDocument.Parse(Program.ParamXML).ToString(); // Program.ParamXML;
                    }
                    catch { txtXML.Text = txtXML.Text; }
                }

                tBoxERecetePath.Visible = false;
                btnSelectFile.Visible = false;
                label1.Visible = false;
            }
            else if (Program.ParamPath != "")
            {
                rbFile.Checked = true;
                try
                {
                    if (File.Exists(tBoxERecetePath.Text))
                    {
                        txtXML.Text = System.Xml.Linq.XDocument.Parse(DosyaRead(Program.ParamPath)).ToString(); // DosyaRead(Program.ParamPath);
                    }
                    else
                    {
                        txtXML.Text = Program.ParamPath + " İmza atılacak dosya bulunamadı, dosyayı seçiniz.";
                    }
                }
                catch { txtXML.Text = Program.ParamPath + " İmza atılacak dosya bulunamadı, dosyayı seçiniz."; }
                tBoxERecetePath.Visible = true;
                btnSelectFile.Visible = true;
                label1.Visible = true;
            }
            else if (tBoxERecetePath.Text != "")
            {
                rbFile.Checked = true;
                try
                {
                    if (File.Exists(tBoxERecetePath.Text))
                    {
                        txtXML.Text = System.Xml.Linq.XDocument.Parse(DosyaRead(tBoxERecetePath.Text)).ToString(); // DosyaRead(tBoxERecetePath.Text);
                    }
                    else
                    {
                        txtXML.Text = tBoxERecetePath.Text + " İmza atılacak dosya bulunamadı, dosyayı seçiniz.";
                    }
                }
                catch
                {
                    txtXML.Text = tBoxERecetePath.Text + " İmza atılacak dosya bulunamadı, dosyayı seçiniz.";
                }

                tBoxERecetePath.Visible = true;
                btnSelectFile.Visible = true;
                label1.Visible = true;
            }
            tmrGiris.Enabled = true;
            //LisansHelper.loadFreeLicense();
            //KartveOkuyucuKontrol();
            if (Program.ParamPin != "")
            {
                if ((Program.ParamPin.Length > 3) && (Program.ParamPin.Length < 10))
                {
                    //if ((Program.ParamPin.Length != 4) && (Program.ParamCardType == "SAFESIGN"))
                    //{
                    //    MesajiIsle("SAFESIGN için Pin kodu sorunlu (4 karakter olmalı): " + Program.ParamPin, 0);
                    //}
                    //else
                    //{
                    if (Program.ParamPin.All(char.IsDigit))
                    {
                        tbPinKodu.Text = Program.ParamPin;
                        tBoxERecetePath.Visible = false;
                        btnSelectFile.Visible = false;
                        label1.Visible = false;
                    }
                    else
                    {
                        MesajiIsle("Pin kodu sorunlu: " + Program.ParamPin, 0);
                    }
                    //}
                }
                else
                {
                    MesajiIsle("Pin kodu fazla uzun veya fazla kısa: " + Program.ParamPin, 0);
                }
            }
            else
            {
                if (Program.ParamOto == "1")
                {
                    MesajiIsle("Pin kodu boş: " + Program.ParamPin, 0);
                }
            }

        }
        public static DateTime GetNetworkTime()
        {
            try
            {
                //default Windows time server
                const string ntpServer = "time.windows.com";

                // NTP message size - 16 bytes of the digest (RFC 2030)
                var ntpData = new byte[48];

                //Setting the Leap Indicator, Version Number and Mode values
                ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

                var addresses = Dns.GetHostEntry(ntpServer).AddressList;

                //The UDP port number assigned to NTP is 123
                var ipEndPoint = new IPEndPoint(addresses[0], 123);
                //NTP uses UDP
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                socket.Connect(ipEndPoint);

                socket.SendTimeout = 3000;
                //Stops code hang if NTP is blocked
                socket.ReceiveTimeout = 3000;

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();

                //Offset to get to the "Transmit Timestamp" field (time at which the reply 
                //departed the server for the client, in 64-bit timestamp format."
                const byte serverReplyTime = 40;

                //Get the seconds part
                ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

                //Get the seconds fraction
                ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

                //Convert From big-endian to little-endian
                intPart = SwapEndianness(intPart);
                fractPart = SwapEndianness(fractPart);

                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

                //**UTC** time
                var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

                return networkDateTime.ToLocalTime();
            }

            catch (Exception e)
            {
                MesajiIsle("Gerçek Tarih Saat alınamadı: " + e.Message, 1);
                return new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

        }

        // stackoverflow.com/a/3294698/162671
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }


        public static void MesajiIsle(string Mesaj, byte Fatal)
        {
            if (Program.ParamOto == "0")
            {
                if (Fatal == 1) { Program.Hata = 1; }
                if (Mesaj != "") { MessageBox.Show(Mesaj); }
            }
            else
            {
                if (Mesaj != "") { System.Console.WriteLine(Mesaj); }
                if (Fatal == 1)
                {
                    Program.Hata = 1;
                    Environment.Exit(0);
                }
            }
        }

        public bool KartveOkuyucuKontrol()
        {
            // ilk giriste terminal sayisini degiskene kaydet, sertifikayi goster ve degiskene kaydet
            String[] terminals = SmartOp.getCardTerminals();
            Program.TerminalSayisi = terminals.Length;

            if (terminals == null || terminals.Length == 0)
            {
                MesajiIsle("Kart takılı bir kart okuyucu bulunamadı. E-İmza programına girmeden evvel imza için kullanacağınız kartı takmalısınız.", 1);
                // eskiden bundan sonra cikartmiyordum, eimza kısmında kart değiştigi veya sonradan takıldığı ortaya cikiyordu. 
                // orada uyari alip cikiyordu (karti programa girdikten sonra degistirmeyin veya onceden takiniz gibi...)
                // Bir tus konulup kartlari Oku diye girdikten sonra manuel olarak kart bilgisi almasi saglanabilir ama gerekli oldugunu sanmiyorum.
                //MessageBox.Show("Kart takılı kart okuyucu bulunamadı", "", MessageBoxButtons.OK,
                //             System.Windows.Forms.MessageBoxIcon.Error,
                //             System.Windows.Forms.MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return false;
                // throw new SmartCardException("Kart takılı kart okuyucu bulunamadı");
            }
            SmartCardManager scmgr = SmartCardManager.getInstance();
            {
                ECertificate signingCert = scmgr.getSignatureCertificate(true, false);
                lbSertifikaSahibi.Text = "Sertifika ve Sahiplik Bilgisi: " + signingCert.ToString();
                Program.SertifikaBilgisi = "Sertifika ve Sahiplik Bilgisi: " + signingCert.ToString();
                //lbTCKimlikNo.Text = TerminalSayisi 
            }

            return true;
        }

        public string DosyaRead(string Path)
        {

            if (Path == "") return "";
            try
            {
                StreamReader streamRead = new StreamReader(Path);
                string text = streamRead.ReadToEnd();
                streamRead.Close();
                return text;
            }
            catch
            {
                MesajiIsle("Dosya Okunamadı: " + Path, 0);
                return "";
            }
        }
        // dll embed icin
        //// dll handler
        //System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    string keyName = new AssemblyName(args.Name).Name;

        //    // If DLL is loaded then don't load it again just return
        //    if (_libs.ContainsKey(keyName)) return _libs[keyName];

        //    // using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DllResourceName("itextsharp.dll")))  // <-- To find out the Namespace name go to Your Project >> Properties >> Application >> Default namespace
        //    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EsyaXMLSignatureExample." + keyName + ".dll"))  // <-- To find out the Namespace name go to Your Project >> Properties >> Application >> Default namespace
        //    {
        //        byte[] buffer = new BinaryReader(stream).ReadBytes((int)stream.Length);
        //        Assembly assembly = Assembly.Load(buffer);
        //        _libs[keyName] = assembly;
        //        return assembly;
        //    }
        //}

        // dll embed icin
        //private string DllResourceName(string ddlName)
        //{
        //    string resourceName = string.Empty;
        //    foreach (string name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
        //    {
        //        if (name.EndsWith(ddlName))
        //        {
        //            resourceName = name;
        //            break;
        //        }
        //    }
        //    return resourceName;
        //}

        private void DosyaKontrolu()
        {
            if (Program.ParamOto != "1")
            {
                if (txtXML.Text.Contains("<ereceteBilgisi>") && txtXML.Text.Contains("</ereceteBilgisi>") && txtXML.Text.Contains("<takipNo>") && txtXML.Text.Contains("<doktorTcKimlikNo>"))
                {
                    Program.ImzalanacakVeriTipi = "erecete";
                    //    if (MessageBox.Show("E-Reçete bilgisini 'Güvenli Elektronik İmza' ile imzalamak istediğinizden emin misiniz?", "Dikkat", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //    { }
                    //    else MesajiIsle("", 1);
                }
                else
                {
                    if (txtXML.Text.Contains("<") && txtXML.Text.Contains("</") && txtXML.Text.Contains(">"))
                    {
                        Program.ImzalanacakVeriTipi = "xml";
                        //if (MessageBox.Show("İmzalanacak bilgi, bir XML ve e-Reçete değil. 'Güvenli Elektronik İmza' ile imzalamak istediğinizden emin misiniz?", "Dikkat", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        //{ }
                        //else MesajiIsle("", 1);
                    }
                    else
                    {
                        if (txtXML.Text.Contains("<egitimBildirim>") || Program.ParamXML.StartsWith("FisTipi"))
                        {
                            Program.ImzalanacakVeriTipi = "xml";
                        }
                        else
                        {
                            if (MessageBox.Show("İmzalanacak dosya e-Reçete değil. Yine de 'Güvenli Elektronik İmza' ile imzalamak istediğinizden emin misiniz?", "Dikkat", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                Program.ImzalanacakVeriTipi = "dosya";
                            }
                            else MesajiIsle("", 1);
                        }
                    }

                }
            }
        }
        public static SqlConnection SQLConUpdate = new SqlConnection();
        private string sql;
        public static SqlConnection SQLConBrowse = new SqlConnection();
        public static SqlDataReader reader;

        public void BaglanVeriAl(object sender, EventArgs e)
        {
            if (Program.ParamSQLUser == "")
            {
                MessageBox.Show("SQL Server bağlantı bilgileri eksik (SQLUser). Kayıt işlemi yapılamadı.", "");
                return;
            }
            if (Program.ParamSQLPassword == "")
            {
                MessageBox.Show("SQL Server bağlantı bilgileri eksik(SQLPassword). Kayıt işlemi yapılamadı.", "");
                return;
            }
            if (Program.ParamSQLDB == "")
            {
                MessageBox.Show("SQL Server bağlantı bilgileri eksik (veritabanı). Kayıt işlemi yapılamadı.", "");
                return;
            }
            // dbye baglan
            SQLConBrowse.Close();
            SQLConBrowse.ConnectionString = "server=" + Program.ParamSQLServer + ";user=" + Program.ParamSQLUser + ";pwd=" + Program.ParamSQLPassword + ";database=" + Program.ParamSQLDB;

            // sql olustur 
            String SqlCumlesi = "";
            if (Program.KayitBilgisi[1] == "Egitim") // fistipi
            {
                if (Convert.ToDouble(Program.KayitBilgisi[3]) != 0)
                {   // tek kayit cagrilmasi
                    SqlCumlesi = "select * from EgitimlerDetay where FisNo = @FisNo and isnull(Durum,0) <> 1";
                }
                else
                {   // o kisinin imzalayacagi tum egitimler
                    SqlCumlesi = "select * from EgitimlerDetay where XMLCreateUser = @XMLCreateUser and isnull(Durum,0) <> 1 and isnull(XML, '') <> '' ";
                }
            }
            else
            {
                if (Convert.ToDouble(Program.KayitBilgisi[3]) != 0)
                {   // tek kayit cagrilmasi
                    SqlCumlesi = "SELECT * FROM IBYSGonderimleri WHERE FisNo = @FisNo and isnull(Durum,0) <> 1 and isnull(XML, '') <> '' ";
                }
                else
                {   // o kisinin imzalayacagi tum kayitlar
                    SqlCumlesi = "SELECT * FROM IBYSGonderimleri WHERE XMLCreateUser = @XMLCreateUser and isnull(Durum,0) <> 1 and isnull(XML, '') <> '' ";
                }
            }

            SqlCommand qryVeriOku = new SqlCommand(SqlCumlesi, SQLConBrowse);
            try
            {
                // txtBilgi.Text += SQLConBrowse.ConnectionString + "\r\n";
                SQLConBrowse.Open();
                if (Convert.ToDouble(Program.KayitBilgisi[3]) != 0)
                {
                    qryVeriOku.Parameters.Add("@FisNo", SqlDbType.Float).Value = Program.KayitBilgisi[3];
                }
                else
                {
                    qryVeriOku.Parameters.Add("@XMLCreateUser", SqlDbType.Char).Value = Program.KayitBilgisi[7];
                }
                reader = qryVeriOku.ExecuteReader();
                txtBilgi.Text += "Bağlandı\r\n";
            }
            catch (Exception Exception1)
            {
                MessageBox.Show($"Kayıtlar alınamadı: {Exception1.Message}");
            }
        }

        //public void ImzaliXMLKaydet(double FisNo, int SiraNo, string EgitimAltKodu, string Tablo, string ImzaliXML, string Hash256)
        public void ImzaliXMLKaydet(double FisNo, int SiraNo, string EgitimAltKodu, string Tablo, string ImzaliXML, string FisTipi)
        {
            // var connetionString = "Data Source=EVOPC18\\PMSMART;Initial Catalog=NORTHWND;User ID=test;Password=test";
            // if (Tablo == "EgitimlerDetay")
            if (FisTipi == "Egitim")
            {
                sql = "UPDATE EgitimlerDetay SET ImzaliXML = @ImzaliXML where FisNo = @FisNo and EgitimAltKodu = @EgitimAltKodu ";
            }
            else
            { 
                sql = "UPDATE IBYSGonderimleri SET ImzaliXML = @ImzaliXML, EImzaUser = @EImzaUser where FisTuru = @FisTipi and FisNo = @FisNo ";
            }

            try
            {
                SQLConUpdate.Close();
                SQLConUpdate.ConnectionString = "server=" + Program.ParamSQLServer + ";user=" + Program.ParamSQLUser + ";pwd=" + Program.ParamSQLPassword + ";database=" + Program.ParamSQLDB;
                SQLConUpdate.Open();
                using (SQLConUpdate)
                {
                    using (var command = new SqlCommand(sql, SQLConUpdate))
                    {
                        command.Parameters.Add("@FisNo", SqlDbType.Float).Value = FisNo;
                        command.Parameters.Add("@ImzaliXML", SqlDbType.Char).Value = ImzaliXML;
                        if (Tablo == "EgitimlerDetay")
                        {
                            //
                            command.Parameters.Add("@EgitimAltKodu", SqlDbType.Char).Value = EgitimAltKodu;
                            // command.Parameters.Add("@Hash256", SqlDbType.Char).Value = Hash256; artik gerek yok, imza atarken yasanan sorunlardan dolayi test icin koymustuk
                        }
                        else
                        {
                            if (Tablo == "IBYSGonderimleri")
                            {
                                command.Parameters.Add("@FisTipi", SqlDbType.Char).Value = FisTipi;
                                command.Parameters.Add("@EImzaUser", SqlDbType.Char).Value = Program.KayitBilgisi[7]; // kullanicikodu
                            }
                        }
                        // repeat for all variables....
                        command.ExecuteNonQuery();
                        if (Tablo == "EgitimlerDetay")
                        {
                            txtBilgi.Text += EgitimAltKodu + " kodlu eğitim imzalandı.\n";
                        }
                        else
                        {
                            txtBilgi.Text += FisNo + " nolu " + FisTipi + " kaydı imzalandı.\n";
                        }
                        //MessageBox.Show(txtBilgi.Text);
                    }
                }
            }
            catch (Exception Exception1)
            {
                MessageBox.Show($"Kayıt güncellenemedi: {Exception1.Message}");
            }

        }

        private void btnEReceteImzala_Click(object sender, EventArgs e)
        {
            SignHelper signHelper = new SignHelper();
            // string signedFilePath = signHelper.eReceteImzala(tBoxERecetePath.Text, tBoxSignedERecetePath.Text);
            // dosyayiBase64Yaz(signedFilePath);
            if (Program.ParamXML == "" && Program.ParamPath == "" && tBoxERecetePath.Text == "")
            {
                MesajiIsle("EReçete verisi string olarak da path olarak da gelmedi, kaynak dosyayı seçiniz!", 0);
            }
            if (rbString.Checked && Program.ParamXML == "")
            {
                MesajiIsle("EReçete verisi string olarak gelmedi, kaynak dosya üzerinden işlem yapınız!", 0);
                rbFile.Checked = true;
                if (Program.ParamOto == "0") { return; }
            }
            if (Program.Hata == 1) { Environment.Exit(1); }
            if (tbPinKodu.Text == "")
            {
                MesajiIsle("Pin kodunu giriniz!", 0);
                if (Program.ParamOto == "0") { return; }
                if (Program.ParamOto == "1") { Environment.Exit(1); }
            }
            if (Program.Hata == 1) { Environment.Exit(1); }
            // dosya yapisi kontrolu
            Program.PinKodu = tbPinKodu.Text;
            string XML;
            byte[] signStream;
            byte[] signedDocument;
            //// eger FisNo ile geldiyse veritabanindan ilgili fisnoya ait kayitlari getir dongu icinde XML'leri al ve imzala
            //if (Program.ParamXML.StartsWith("FisTipi"))
            //{
            //    XML = SignHelper.Imzala();
            //    System.Console.WriteLine("esignerbase64:" + XML + ":esignerbase64");
            //    if (XML != "") Application.Exit();
            //}
            // eger FisNo ile geldiyse veritabanindan ilgili fisnoya ait kayitlari getir dongu icinde XML'leri al ve imzala
            if (Program.ParamXML.StartsWith("FisTipi"))
            {
                BaglanVeriAl(null, new EventArgs());

                if (reader.HasRows)
                {
                    // create signature according to context with default type (XADES_BES)
                    DosyaKontrolu();
                    if (Program.Hata == 1) { Environment.Exit(1); }
                    string ImzalanacakXML, EgitimAltKodu, Imzalanacak_Sha256Hash_Base64;
                    signStream = System.Text.Encoding.UTF8.GetBytes("");
                    signedDocument = System.Text.Encoding.UTF8.GetBytes("");

                    // otomatik hepsini gonder
                    // Main.ActiveForm.Enabled = false;
                    try
                    {
                        while (reader.Read())
                        {
                            tmrGiris_Tick(btnImzalaSQL, new EventArgs());
                            ImzalanacakXML = reader["XML"].ToString().Trim();
                            if (Program.KayitBilgisi[1] == "Egitim") // fistipi
                            {
                                EgitimAltKodu = reader["EgitimAltKodu"].ToString().Trim();
                            }
                            else
                            {
                                EgitimAltKodu = "";
                            }


                            BaseSignedData bs = new BaseSignedData();

                            //var crypt = new System.Security.Cryptography.SHA256Managed();
                            //byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(ImzalanacakXML));
                            //var hash = new System.Text.StringBuilder();
                            //ISignable content = new SignableByteArray(crypto);

                            // ISignable content = new SignableByteArray(ASCIIEncoding.ASCII.GetBytes(ImzalanacakXML));
                            // 2-a XML SHA-256 ile hashlenmelidir
                            // 2-b Hash sonucunda dönen değer, byte olmalıdır
                            // byte[] Imzalanacak_Sha256Hash = ComputeSha256HashToByteArray(ImzalanacakXML);


                            // Imzalanacak_Sha256Hash = sha256Hash2(ImzalanacakXML);
                            // Imzalanacak_Sha256Hash = System.Text.Encoding.ASCII.GetBytes("5e3f2acc9090be3472470e6f78446733054d36111bcd747be0a17391ae73bea0");
                            // test
                            // byte[] hashBytes = Encoding.UTF8.GetBytes(ImzalanacakXML);
                            // 2-Hash sonucunda dönen değer String’e çevirilmemelidir

                            //SHA256 Sha256 = SHA256Managed.Create();
                            // Imzalanacak_Sha256Hash = Sha256.ComputeHash(hashBytes);
                            // string crypt = Encoding.Default.GetString(Imzalanacak_Sha256Hash);
                            // test sonu



                            // sadece bakanliga mail ile bildirmek icin base64 hali
                            // Imzalanacak_Sha256Hash_Base64 = System.Convert.ToBase64String(Imzalanacak_Sha256Hash);  // ******** OK
                            // sadece bakanliga mail ile bildirmek icin base64 hali sonu

                            ISignable content = new SignableByteArray(Encoding.UTF8.GetBytes(ImzalanacakXML));
                            // sonra da sunu dene...
                            // ISignable content = new SignableByteArray(Encoding.UTF8.GetBytes(Imzalanacak_Sha256Hash_Base64));
                            // sonra da sunu...
                            // ISignable content = new SignableByteArray(Encoding.UTF8.GetBytes(Imzalanacak_Sha256Hash_Base64));
                            //   ISignable content = new SignableByteArray(Imzalanacak_Sha256Hash);
                            // 3-a İmza’ya SHA’lanmış değer gönderilmelidir.XML gönderilmemelidir
                            // 3-c Ayrık imza olmalıdır
                            bs.addContent(content, false);

                            Dictionary<string, object> params_ = new Dictionary<string, object>();

                            //List<IAttribute> optionalAttributes = new List<IAttribute>();
                            //optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));
                            //if the user does not want certificate validation at generating signature,he can add 
                            //P_VALIDATE_CERTIFICATE_BEFORE_SIGNING parameter with its value set to false
                            params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

                            //necessary for certificate validation.By default,certificate validation is done 
                            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();

                            //By default, QC statement is checked,and signature wont be created if it is not a 
                            //qualified certificate. 

                            bool checkQCStatement = TestConstants.getCheckQCStatement();
                            //Get qualified or non-qualified certificate.
                            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);

                            //BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);
                            BaseSigner signer = SmartCardManager.getInstance().getSigner(tbPinKodu.Text, cert);

                            //add signer
                            //Since the specified attributes are mandatory for bes,null is given as parameter 
                            //for optional attributes

                            try
                            {
                                // 3-b İmza CADES ile atılmalıdır
                                bs.addSigner(ESignatureType.TYPE_BES, cert, signer, null, params_);
                            }
                            catch (CertificateValidationException cve)
                            {
                                Console.WriteLine((string)cve.getCertStatusInfo().getDetailedMessage());

                            }

                            SmartCardManager.getInstance().logout();
                            // bs.detachContent();
                            // 3-d İmza değeri byte olarak dönmelidir.
                            signedDocument = bs.getEncoded();
                            var b64 = Base64.Encode(signedDocument);
                            //write the contentinfo to file
                            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
                            AsnIO.dosyayaz(signedDocument, di.FullName + @"\BES-1.p7s");

                            SignedDataValidationResult sdvr = ValidationUtil.validate(signedDocument, null);

                            //Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
                            Application.DoEvents();
                            bs = null;
                            cert = null;
                            signStream = signedDocument;

                            if (signStream.Length > 0)
                            {
                                if (Program.KayitBilgisi[1] == "Egitim")
                                {   // egitim 
                                    ImzaliXMLKaydet((Double)(reader["FisNo"]), 0, EgitimAltKodu, "EgitimlerDetay", Encoding.UTF8.GetString(b64), Program.KayitBilgisi[1]);
                                }
                                else
                                {   // diger gonderimler
                                    ImzaliXMLKaydet((Double)(reader["FisNo"]), 0, EgitimAltKodu, "IBYSGonderimleri", Encoding.UTF8.GetString(b64), Program.KayitBilgisi[1]);
                                }
                            }
                            // SmartCardManager = null; SmartCardManager create edilmeden direk kullaniliyor.
                            // Belki ayri nesne olarak create edip sonra null yapmak gerekebilir
                        }
                    }
                    finally
                    {
                        if (Main.ActiveForm != null) { Main.ActiveForm.Enabled = true; }
                    }
                    SQLConUpdate.Close();
                    //                    System.Console.WriteLine("esignerbase64:" + System.Convert.ToBase64String(signStream) + ":esignerbase64");
                    System.Console.WriteLine("esignerbase64:" + Encoding.UTF8.GetString(Base64.Encode(signedDocument)) + ":esignerbase64");
                    if (System.Convert.ToBase64String(signStream) != "") Application.Exit();
                }
            }
            else
            {
                DosyaKontrolu();
                if (Program.Hata == 1) { Environment.Exit(1); }
                if (Program.ImzalanacakVeriTipi == "xml")
                    XML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + signHelper.eReceteImzala(tBoxERecetePath.Text, tBoxSignedERecetePath.Text, Program.ParamXML, cbDosyayaYaz.Checked);
                else
                    XML = signHelper.eReceteImzala(tBoxERecetePath.Text, tBoxSignedERecetePath.Text, Program.ParamXML, cbDosyayaYaz.Checked);
                if (Program.Hata == 1) { Environment.Exit(1); }
                // dosyaya yazilacaksa eReceteImzala icinde imzalanmis xsig dosyasi olusuyor 
                // sonra bu dosya alttaki prosedurde okunup base64'e cevriliyor ve sonu _64.txt olacak sekilde kaydediliyor.
                if (cbDosyayaYaz.Checked) dosyayiBase64Yaz(tBoxSignedERecetePath.Text.Replace(".xml", ".xsig"));
                // ONEMLI, base64 olarak console ciktisi veriyorum. ikazanci
                signStream = System.Text.Encoding.UTF8.GetBytes(XML);
                // hata varsa parametreyi geri dondurmeden evvel cik
                if (Program.Hata == 1) { Environment.Exit(1); }
                if (cbDosyayaYaz.Checked) WriteByteArrayToFile(System.Convert.ToBase64String(signStream), Path.GetDirectoryName(tBoxSignedERecetePath.Text), Path.GetFileName(tBoxSignedERecetePath.Text) + ".xsig");
                System.Console.WriteLine("esignerbase64:" + System.Convert.ToBase64String(signStream) + ":esignerbase64");
                if (XML != "") Application.Exit();
            }
        }


        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static byte[] ComputeSha256HashToByteArray(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return bytes;
            }
        }

        //public static string sha256Hash(string data)
        //{
        //    // create buffer and specify encoding format (here utf8)
        //    IBuffer input = CryptographicBuffer.ConvertStringToBinary(data,
        //    BinaryStringEncoding.Utf8);

        //    // select algorithm
        //    var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA256");
        //    IBuffer hashed = hasher.HashData(input);

        //    // return hash in base64 format
        //    return CryptographicBuffer.EncodeToBase64String(hashed);
        //}

        static byte[] sha256Hash2(string rawData)
        {
            byte[] hashBytes = Encoding.UTF8.GetBytes(rawData);
            SHA256 sha256 = SHA256Managed.Create();
            return sha256.ComputeHash(hashBytes);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            //tBoxERecetePath.Text = currentDirectory + "\\eRecete.xml";
            //tBoxSignedERecetePath.Text = currentDirectory + "\\eReceteBES.xml";
            if (Program.ParamPath != "")
            { tBoxERecetePath.Text = Program.ParamPath; }
            else tBoxERecetePath.Text = "eRecete.xml";
            //tBoxERecetePath.Text = Program.ParamPath +"eRecete.xml";
            tBoxSignedERecetePath.Text = currentDirectory + "\\eReceteImzali.xml";
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = Directory.GetCurrentDirectory();
            DialogResult result = fd.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                tBoxERecetePath.Text = fd.FileName;
                txtXML.Text = DosyaRead(tBoxERecetePath.Text);
            }
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            SignHelper signHelper = new SignHelper();
            string signedERecetePath = tBoxSignedERecetePath.Text;
            string verifyResultStr = signHelper.verifySignature(signedERecetePath);
            MesajiIsle(verifyResultStr, 0);
        }

        private void btnAddTimeStamp_Click(object sender, EventArgs e)
        {
            SignHelper signHelper = new SignHelper();
            string signedERecetePath = tBoxSignedERecetePath.Text;
            string estFilePath = signHelper.upgradeToEST(signedERecetePath);
            if (estFilePath != null)
            {
                tBoxSignedERecetePath.Text = estFilePath;
                MesajiIsle(estFilePath + " konumunda zaman damgalı imzalı E-Reçete oluşturuldu.", 0);
            }
        }

        private void btnAddSerialSignature_Click(object sender, EventArgs e)
        {
            SignHelper signHelper = new SignHelper();
            string signedERecetePath = tBoxSignedERecetePath.Text;
            string estFilePath = signHelper.addSerialSignature(signedERecetePath);
            if (estFilePath != null)
            {
                tBoxSignedERecetePath.Text = estFilePath;
                MesajiIsle(estFilePath + " konumunda seri imzalı E-Reçete oluşturuldu.", 0);
            }
        }

        private void btnAddParalelSignature_Click(object sender, EventArgs e)
        {
            SignHelper signHelper = new SignHelper();
            string eRecetePath = tBoxERecetePath.Text;
            string estFilePath = signHelper.createParalelSignature(eRecetePath);
            if (estFilePath != null)
            {
                tBoxSignedERecetePath.Text = estFilePath;
                MesajiIsle(estFilePath + " konumunda paralel imzalı E-Reçete oluşturuldu.", 0);
            }
        }

        void dosyayiBase64Yaz(string filePath)
        {
            string base64 = dosyadanBase64Oku(filePath);
            // create a writer and open the file
            TextWriter tw = new StreamWriter(filePath + "_64.txt");

            // write a line of text to the file
            tw.WriteLine(base64);

            // close the stream
            tw.Close();
            // ONEMLI, base64 olarak output veriyorum. ikazanci
            //System.Console.WriteLine(base64);
        }

        private string dosyadanBase64Oku(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] signStream = AsnIO.streamOku(fileStream);
            string base64Signature = System.Convert.ToBase64String(signStream);
            return base64Signature;
        }

        // http://stackoverflow.com/questions/472906/converting-a-string-to-byte-array adresindeki kod
        static byte[] GetBytesx(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        // pdf vs dosyalari imzalanmis (veya sadece base64) veriden dosya olarak yazma
        void WriteByteArrayToFile(string inFileByteArrayStream, string filelocation, string fileName)
        {

            byte[] data = Convert.FromBase64String(inFileByteArrayStream);
            if (Directory.Exists(filelocation))
            {
                filelocation = filelocation + '\\' + fileName;
                using (FileStream Writer = new System.IO.FileStream(filelocation, FileMode.Create, FileAccess.Write))
                {

                    Writer.Write(data, 0, data.Length);
                }
            }
            else
            {
                throw new System.Exception("File location not found");
            }

        }

        private void rbString_CheckedChanged(object sender, EventArgs e)
        {
            if (rbString.Checked)
            {
                tBoxERecetePath.Visible = false;
                btnSelectFile.Visible = false;
                label1.Visible = false;
                txtXML.Text = Program.ParamXML;
            }
            else
            {
                tBoxERecetePath.Visible = true;
                btnSelectFile.Visible = true;
                label1.Visible = true;
                if (tBoxERecetePath.Text != "") { txtXML.Text = DosyaRead(tBoxERecetePath.Text); }
            }

        }

        private void rbFile_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Program.ParamOto == "0")
            {
                // manuel ise belli sure sonra pinkodunu sifirlayabilirsin
                tbPinKodu.Text = "";
                tmrPinKoduTemizle.Enabled = false;
            }
        }

        private void tbPinKodu_TextChanged(object sender, EventArgs e)
        {
            tmrPinKoduTemizle.Enabled = true;
        }

        private void tBoxERecetePath_TextChanged(object sender, EventArgs e)
        {
            //  txtXML.Text = DosyaRead(tBoxERecetePath.Text);
        }

        private void tbPinKodu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnEReceteImzala_Click(this, new EventArgs());
            }
        }

        private void tmrOtoImzala_Tick(object sender, EventArgs e)
        {
        }

        private void tmrGiris_Tick(object sender, EventArgs e)
        {
            //if (Program.himm == 1)
            //{
            //    Application.Exit();
            //}
            tmrGiris.Enabled = false;
            //            LisansHelper.loadFreeLicense();
            LisansHelper.loadLicense();
            if (Program.ParamSlotID == "") if (KartveOkuyucuKontrol() == false) Environment.Exit(0);
            if (Program.ParamOto == "1") { btnEReceteImzala_Click(btnEReceteImzala, new EventArgs()); };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String[] terminals = SmartOp.getCardTerminals();
            if (terminals == null || terminals.Length == 0)
            {
                MesajiIsle("Kart takılı bir kart okuyucu bulunamadı-Kart Kontrol İşlemi", 1);
                return;
            }
            // eger tum kartlari kendisi kontrol edip oto. imzalayacaksa askoption calistirma,
            // for dongusunde tum kartlari ve kartlardaki tum sertifikalari gezip bunlari arraya, sonrasinda da tabloya at
            // imzalama asamasinda tckimlikno uyusmamasi gibi bir durum olursa ancak o zaman tum kartlari bir kez daha
            // dolasarak db tablodaki kart ve sertifikalarin slot nolarini yenile

            // ***********************
            // alt satirda hem kartlar hem sertifikalar hem ltrm class dolduruluyor 
            SmartCardManagerTumunuOku scmgr = SmartCardManagerTumunuOku.getInstanceTumunuOku();
            // ***********************
            {
                // sql olustur 
                String SqlCumlesi = "";
                SqlCumlesi = "Delete AkilliKartlar ";

                for (int i = 0; i < Program.ltrm.Count; i++)
                {
                    SqlCumlesi += " insert into AkilliKartlar (FisNo, TerminalAdi,TCKimlikNo,AdiSoyadi,InsertDate, KartTipi, SlotID) values ('" +
                       Convert.ToString(i + 1) + "','" + Program.ltrm[i].TerminalAdi + "','" + Program.ltrm[i].TCKimlikNo + "','" + Program.ltrm[i].AdiSoyadi + "', getdate(), '" + Program.ltrm[i].KartTipi + "', '" + Convert.ToInt64(Program.ltrm[i].SlotID) + "' )";
                }
                if (Program.ltrm.Count > 0)
                {
                    if (Program.ParamSQLServer == "")
                    {
                        MesajiIsle("SQL Server bağlantı bilgileri eksik. Kayıt işlemi yapılamadı.", 0);
                        //return;
                    }
                    if (Program.ParamSQLUser == "")
                    {
                        MesajiIsle("SQL Server bağlantı bilgileri eksik. Kayıt işlemi yapılamadı.", 0);
                        return;
                    }
                    if (Program.ParamSQLPassword == "")
                    {
                        MesajiIsle("SQL Server bağlantı bilgileri eksik. Kayıt işlemi yapılamadı.", 0);
                        return;
                    }
                    // dbye kaydet
                    SqlConnection SQLFormVeriBaglantisi = new SqlConnection();
                    SQLFormVeriBaglantisi.ConnectionString = "server=" + Program.ParamSQLServer + ";user=" + Program.ParamSQLUser + ";pwd=" + Program.ParamSQLPassword + ";database="+ Program.ParamSQLDB+"; ";
                    SQLFormVeriBaglantisi.Open();
                    SqlCommand qryVeriKaydet = new SqlCommand(SqlCumlesi, SQLFormVeriBaglantisi);
                    qryVeriKaydet.ExecuteNonQuery();
                    SQLFormVeriBaglantisi.Close();
                    MessageBox.Show("Kayıt işlemi Tamamlandı", "Kayıt Bilgisi");
                }
            }

            return;
        }

        private void cbDosyayaYaz_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDosyayaYaz.Checked)
            {
                tBoxSignedERecetePath.Visible = true;
                lblHedef.Visible = true;
            }
            else
            {
                tBoxSignedERecetePath.Visible = false;
                lblHedef.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            String[] terminals = SmartOp.getCardTerminals();
            if (terminals == null || terminals.Length == 0)
            {
                MesajiIsle("Kart takılı bir kart okuyucu bulunamadı-Kart Kontrol İşlemi", 1);
                return;
            }
            // eger tum kartlari kendisi kontrol edip oto. imzalayacaksa askoption calistirma,
            // for dongusunde tum kartlari ve kartlardaki tum sertifikalari gezip bunlari arraya, sonrasinda da tabloya at
            // imzalama asamasinda tckimlikno uyusmamasi gibi bir durum olursa ancak o zaman tum kartlari bir kez daha
            // dolasarak db tablodaki kart ve sertifikalarin slot nolarini yenile

            // ***********************
            // alt satirda hem kartlar hem sertifikalar hem ltrm classi dolduruluyor 
            SmartCardManagerTumunuOku scmgr = SmartCardManagerTumunuOku.getInstanceTumunuOku();
            // ***********************
            {
                // olustur 
                string Mesaj = "";
                for (int i = 0; i < Program.ltrm.Count; i++)
                {
                    Mesaj += "\r\nKart No: " + Convert.ToString(i + 1) + ", TerminalAdı: " + Program.ltrm[i].TerminalAdi + ", TCKimlikNo: " + Program.ltrm[i].TCKimlikNo + ", AdSoyad: " + Program.ltrm[i].AdiSoyadi + ", KartTipi: " + Program.ltrm[i].KartTipi + ", SlotID: " + Program.ltrm[i].SlotID;
                }
                if (Program.ltrm.Count > 0)
                {

                    MessageBox.Show("Kartlar okundu: " + Mesaj, "Kayıt Bilgisi");
                }
            }

            return;


        }

        private void button3_Click_1(object sender, EventArgs e)
        {
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
        }

        private void btnImzalaSQL_Click(object sender, EventArgs e)
        {
            if (Program.ParamXML == "" && Program.ParamPath == "" && tBoxERecetePath.Text == "")
            {
                MesajiIsle("EReçete verisi string olarak da path olarak da gelmedi, kaynak dosyayı seçiniz!", 0);
            }
            if (rbString.Checked && Program.ParamXML == "")
            {
                MesajiIsle("EReçete verisi string olarak gelmedi, kaynak dosya üzerinden işlem yapınız!", 0);
                rbFile.Checked = true;
                if (Program.ParamOto == "0") { return; }
            }
            if (Program.Hata == 1) { Environment.Exit(0); }
            if (tbPinKodu.Text == "")
            {
                MesajiIsle("Pin kodunu giriniz!", 0);
                if (Program.ParamOto == "0") { return; }
            }
            if (Program.Hata == 1) { Environment.Exit(0); }

            Program.PinKodu = tbPinKodu.Text;
            byte[] signStream;
            // eger FisNo ile geldiyse veritabanindan ilgili fisnoya ait kayitlari getir dongu icinde XML'leri al ve imzala
            if (Program.ParamXML.StartsWith("FisTipi"))
            {
                BaglanVeriAl(null, new EventArgs());

                if (reader.HasRows)
                {
                    // create signature according to context with default type (XADES_BES)
                    DosyaKontrolu();
                    string ImzalanacakXML, EgitimAltKodu;
                    signStream = System.Text.Encoding.UTF8.GetBytes("");


                    // otomatik hepsini gonder
                    Program.mSCManager = null;
                    while (reader.Read())
                    {
                        tmrGiris_Tick(btnImzalaSQL, new EventArgs());
                        ImzalanacakXML = reader["XML"].ToString().Trim();
                        EgitimAltKodu = reader["EgitimAltKodu"].ToString().Trim();
                        //SiraNo = (double)reader["SiraNo"];
                        XMLSignature signature = new XMLSignature();
                        if (ImzalanacakXML != "")
                        {
                            InMemoryDocument inMMDoc = new InMemoryDocument(System.Text.Encoding.UTF8.GetBytes(ImzalanacakXML), "", null, null);
                            signature.addDocument(inMMDoc);
                        }
                        // add certificate to show who signed the document
                        signature.SigningTime = DateTime.Now;
                        //Signer Oluşturma
                        Program.mSCManager = SmartCardManagerKimlikNodanSec.getInstance(1); // 0 server version
                        //SmartCardManagerKimlikNodanSec smc = SmartCardManagerKimlikNodanSec.getInstance(1); // 0 server version

                        if (Program.KartOkuyucuYok == 1) Environment.Exit(0);
                        ECertificate signingCert = Program.mSCManager.getSignatureCertificate(true, false);

                        //İlk parameter Kart Pin
                        BaseSigner baseSigner = Program.mSCManager.getSigner(Program.PinKodu, signingCert); // "12345"
                        if (baseSigner == null) Environment.Exit(0);
                        bool validCertificate = SignHelper.isValidCertificate(signingCert);
                        if (!validCertificate)
                        {
                            MesajiIsle("İmza atılmak istenen sertifika geçerli değil." + Program.HataMesaji, 1);
                            Environment.Exit(0);
                        };
                        // add certificate to show who signed the document
                        signature.addKeyInfo(signingCert);
                        //Signer Oluşturma
                        signature.sign(baseSigner);

                        if (Program.Hata == 1) { Environment.Exit(0); }
                        // ONEMLI, base64 olarak console ciktisi veriyorum. ikazanci
                        signStream = System.Text.Encoding.UTF8.GetBytes(signature.Document.OuterXml);
                        // hata varsa parametreyi geri dondurmeden evvel cik
                        if (Program.Hata == 1) { Environment.Exit(0); }
                        if (signature.Document.OuterXml != "")
                        {
                            ImzaliXMLKaydet(Convert.ToDouble(Program.KayitBilgisi[3]), 0, EgitimAltKodu, "EgitimlerDetay", System.Convert.ToBase64String(signStream), Program.KayitBilgisi[1]);
                        }
                        baseSigner = null;
                        signature = null;
                        signingCert = null;
                        Program.mSCManager.logout();
                        Program.mSCManager = null;
                    }
                    SQLConUpdate.Close();
                    System.Console.WriteLine("esignerbase64:" + System.Convert.ToBase64String(signStream) + ":esignerbase64");
                    if (System.Convert.ToBase64String(signStream) != "") Application.Exit();
                }
            }

            if (Program.Hata == 1) { Environment.Exit(0); }
        }

        private void txtBilgi_TextChanged(object sender, EventArgs e)
        {
            txtBilgi.SelectionStart = txtBilgi.Text.Length;
            txtBilgi.ScrollToCaret();
        }
    }

    class terminaller
    {
        public string TerminalAdi { set; get; }
        public string AdiSoyadi { set; get; }
        public string TCKimlikNo { set; get; }
        public string KartTipi { set; get; }
        public string SlotID { set; get; }
    }
    //if (i > 0) WinFormsExtensions.AppendLine(txtXML, "");
    //WinFormsExtensions.AppendLine(txtXML, ltrm[i].TerminalAdi);
    //WinFormsExtensions.AppendLine(txtXML, ltrm[i].AdiSoyadi);
    //WinFormsExtensions.AppendLine(txtXML, ltrm[i].TCKimlikNo);

}
//                            // imza konteyneri yarat
//                            SignatureContainer container = SignatureFactory.createContainer(tr.gov.tubitak.uekae.esya.api.signature.SignatureFormat.CAdES, ContextCades);

//ECertificate signingCert = Program.mSCManager.getSignatureCertificate(true, false);
//// konteyner içinde imza nesnesi oluştur
//tr.gov.tubitak.uekae.esya.api.signature.Signature CadesSignature = container.createSignature(signingCert);

//BaseSigner baseSigner = Program.mSCManager.getSigner(Program.PinKodu, signingCert); // "12345"
//                                                                                    // imzalanacak içerik ekle
//string filename = Path.GetFileName(tBoxSignedERecetePath.Text) + ".txt";
//string dirname = Path.GetFileName(tBoxSignedERecetePath.Text);
//                                WriteByteArrayToFile(System.Convert.ToBase64String(Encoding.UTF8.GetBytes(Program.ParamXML)), Path.GetDirectoryName(dirname), filename);
//                                FileInfo file = new FileInfo(filename);
//CadesSignature.addContent(new tr.gov.tubitak.uekae.esya.api.signature.impl.SignableFile(file, "text/plain"), false);

//                                // imzala
//                                CadesSignature.sign(baseSigner);

//                                // imzayı yaz
//                                // container.write(new FileOutputStream(fileName));
//                                if (Program.Hata == 1) { Environment.Exit(1); }
//                                // ONEMLI, base64 olarak console ciktisi veriyorum. ikazanci
//                                signStream = System.Text.Encoding.UTF8.GetBytes(CadesSignature.ToString());
//                                // hata varsa parametreyi geri dondurmeden evvel cik
//                                if (Program.Hata == 1) { Environment.Exit(1); }
//                                if (CadesSignature.ToString() != "")
//                                {
//                                    ImzaliXMLKaydet((Double)(reader["FisNo"]), 0, EgitimAltKodu, "EgitimlerDetay", System.Convert.ToBase64String(signStream));
//                                }
//                                Application.DoEvents();
//                                baseSigner = null;
//                                signature = null;
//                                signingCert = null;

//                            }
//                            else
//                            {
//                                ECertificate signingCert = Program.mSCManager.getSignatureCertificate(true, false);

////İlk parameter Kart Pin
//BaseSigner baseSigner = Program.mSCManager.getSigner(Program.PinKodu, signingCert); // "12345"
//                                if (baseSigner == null) Environment.Exit(1);
//                                bool validCertificate = SignHelper.isValidCertificate(signingCert);
//                                if (!validCertificate)
//                                {
//                                    MesajiIsle("İmza atılmak istenen sertifika geçerli değil." + Program.HataMesaji, 1);
//Environment.Exit(1);
//                                }
//                                // add certificate to show who signed the document
//                                signature.addKeyInfo(signingCert);
//                                //Signer Oluşturma
//                                signature.sign(baseSigner);

//                                if (Program.Hata == 1) { Environment.Exit(1); }
//                                // ONEMLI, base64 olarak console ciktisi veriyorum. ikazanci
//                                signStream = System.Text.Encoding.UTF8.GetBytes(signature.Document.OuterXml);
//                                // hata varsa parametreyi geri dondurmeden evvel cik
//                                if (Program.Hata == 1) { Environment.Exit(1); }
//                                if (signature.Document.OuterXml != "")
//                                {
//                                    ImzaliXMLKaydet((Double)(reader["FisNo"]), 0, EgitimAltKodu, "EgitimlerDetay", System.Convert.ToBase64String(signStream));
//                                }
//                                Application.DoEvents();
//                                baseSigner = null;
//                                signature = null;
//                                signingCert = null;
//                                Program.mSCManager.logout();
//                                Program.mSCManager = null;
//                            }



// 1111 hatasi veren sekil

//BaseSignedData bs = new BaseSignedData();

////var crypt = new System.Security.Cryptography.SHA256Managed();
////byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(ImzalanacakXML));
////var hash = new System.Text.StringBuilder();
////ISignable content = new SignableByteArray(crypto);

//// ISignable content = new SignableByteArray(ASCIIEncoding.ASCII.GetBytes(ImzalanacakXML));
//// 2-a XML SHA-256 ile hashlenmelidir
//// 2-b Hash sonucunda dönen değer, byte olmalıdır
//byte[] Imzalanacak_Sha256Hash = ComputeSha256HashToByteArray(ImzalanacakXML);
//Imzalanacak_Sha256Hash = sha256Hash2(ImzalanacakXML);
//// 2-Hash sonucunda dönen değer String’e çevirilmemelidir
//// sadece bakanliga mail ile bildirmek icin base64 hali
//Imzalanacak_Sha256Hash_Base64 = System.Convert.ToBase64String(Imzalanacak_Sha256Hash);
//                            // sadece bakanliga mail ile bildirmek icin base64 hali sonu

//                            // ISignable content = new SignableByteArray(Encoding.UTF8.GetBytes(ImzalanacakXML));
//                            ISignable content = new SignableByteArray(Imzalanacak_Sha256Hash);
//// 3-a İmza’ya SHA’lanmış değer gönderilmelidir.XML gönderilmemelidir
//// 3-c Ayrık imza olmalıdır
//bs.addContent(content, false);

//                            Dictionary<string, object> params_ = new Dictionary<string, object>();

////List<IAttribute> optionalAttributes = new List<IAttribute>();
////optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));
////if the user does not want certificate validation at generating signature,he can add 
////P_VALIDATE_CERTIFICATE_BEFORE_SIGNING parameter with its value set to false
//params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

//                            //necessary for certificate validation.By default,certificate validation is done 
//                            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();

//                            //By default, QC statement is checked,and signature wont be created if it is not a 
//                            //qualified certificate. 

//                            bool checkQCStatement = TestConstants.getCheckQCStatement();
////Get qualified or non-qualified certificate.
//ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);

////BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);
//BaseSigner signer = SmartCardManager.getInstance().getSigner(tbPinKodu.Text, cert);

//                            //add signer
//                            //Since the specified attributes are mandatory for bes,null is given as parameter 
//                            //for optional attributes

//                            try
//                            {
//                                // 3-b İmza CADES ile atılmalıdır
//                                bs.addSigner(ESignatureType.TYPE_BES, cert, signer, null, params_);
//                            }
//                            catch (CertificateValidationException cve)
//                            {
//                                Console.WriteLine((string) cve.getCertStatusInfo().getDetailedMessage());

//                            }

//                            SmartCardManager.getInstance().logout();
//bs.detachContent();
//                            // 3-d İmza değeri byte olarak dönmelidir.
//                            byte[] signedDocument = bs.getEncoded();

////write the contentinfo to file
//DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
//AsnIO.dosyayaz(signedDocument, di.FullName + @"\BES-1.p7s");

//                            SignedDataValidationResult sdvr = ValidationUtil.validate(signedDocument, null);

////Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
//Application.DoEvents();
//                            bs = null;
//                            cert = null;
//                            signStream = signedDocument;







// **************** 1111 hatasi alinan hali ****************** //
//private void btnEReceteImzala_Click(object sender, EventArgs e)
//{
//    SignHelper signHelper = new SignHelper();
//    // string signedFilePath = signHelper.eReceteImzala(tBoxERecetePath.Text, tBoxSignedERecetePath.Text);
//    // dosyayiBase64Yaz(signedFilePath);
//    if (Program.ParamXML == "" && Program.ParamPath == "" && tBoxERecetePath.Text == "")
//    {
//        MesajiIsle("EReçete verisi string olarak da path olarak da gelmedi, kaynak dosyayı seçiniz!", 0);
//    }
//    if (rbString.Checked && Program.ParamXML == "")
//    {
//        MesajiIsle("EReçete verisi string olarak gelmedi, kaynak dosya üzerinden işlem yapınız!", 0);
//        rbFile.Checked = true;
//        if (Program.ParamOto == "0") { return; }
//    }
//    if (Program.Hata == 1) { Environment.Exit(1); }
//    if (tbPinKodu.Text == "")
//    {
//        MesajiIsle("Pin kodunu giriniz!", 0);
//        if (Program.ParamOto == "0") { return; }
//        if (Program.ParamOto == "1") { Environment.Exit(1); }
//    }
//    if (Program.Hata == 1) { Environment.Exit(1); }
//    // dosya yapisi kontrolu
//    Program.PinKodu = tbPinKodu.Text;
//    string XML;
//    byte[] signStream;
//    //// eger FisNo ile geldiyse veritabanindan ilgili fisnoya ait kayitlari getir dongu icinde XML'leri al ve imzala
//    //if (Program.ParamXML.StartsWith("FisTipi"))
//    //{
//    //    XML = SignHelper.Imzala();
//    //    System.Console.WriteLine("esignerbase64:" + XML + ":esignerbase64");
//    //    if (XML != "") Application.Exit();
//    //}
//    // eger FisNo ile geldiyse veritabanindan ilgili fisnoya ait kayitlari getir dongu icinde XML'leri al ve imzala
//    if (Program.ParamXML.StartsWith("FisTipi"))
//    {
//        BaglanVeriAl(null, new EventArgs());

//        if (reader.HasRows)
//        {
//            // create signature according to context with default type (XADES_BES)
//            DosyaKontrolu();
//            if (Program.Hata == 1) { Environment.Exit(1); }
//            string ImzalanacakXML, EgitimAltKodu, Imzalanacak_Sha256Hash_Base64;
//            signStream = System.Text.Encoding.UTF8.GetBytes("");

//            // otomatik hepsini gonder
//            // Main.ActiveForm.Enabled = false;
//            try
//            {
//                while (reader.Read())
//                {
//                    tmrGiris_Tick(btnImzalaSQL, new EventArgs());
//                    ImzalanacakXML = reader["XML"].ToString().Trim();
//                    EgitimAltKodu = reader["EgitimAltKodu"].ToString().Trim();

//                    BaseSignedData bs = new BaseSignedData();

//                    //var crypt = new System.Security.Cryptography.SHA256Managed();
//                    //byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(ImzalanacakXML));
//                    //var hash = new System.Text.StringBuilder();
//                    //ISignable content = new SignableByteArray(crypto);

//                    // ISignable content = new SignableByteArray(ASCIIEncoding.ASCII.GetBytes(ImzalanacakXML));
//                    // 2-a XML SHA-256 ile hashlenmelidir
//                    // 2-b Hash sonucunda dönen değer, byte olmalıdır
//                    byte[] Imzalanacak_Sha256Hash = ComputeSha256HashToByteArray(ImzalanacakXML);
//                    Imzalanacak_Sha256Hash = sha256Hash2(ImzalanacakXML);
//                    // 2-Hash sonucunda dönen değer String’e çevirilmemelidir
//                    // sadece bakanliga mail ile bildirmek icin base64 hali
//                    Imzalanacak_Sha256Hash_Base64 = System.Convert.ToBase64String(Imzalanacak_Sha256Hash);
//                    // sadece bakanliga mail ile bildirmek icin base64 hali sonu

//                    // ISignable content = new SignableByteArray(Encoding.UTF8.GetBytes(ImzalanacakXML));
//                    ISignable content = new SignableByteArray(Imzalanacak_Sha256Hash);
//                    // 3-a İmza’ya SHA’lanmış değer gönderilmelidir.XML gönderilmemelidir
//                    // 3-c Ayrık imza olmalıdır
//                    bs.addContent(content, false);

//                    Dictionary<string, object> params_ = new Dictionary<string, object>();

//                    //List<IAttribute> optionalAttributes = new List<IAttribute>();
//                    //optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));
//                    //if the user does not want certificate validation at generating signature,he can add 
//                    //P_VALIDATE_CERTIFICATE_BEFORE_SIGNING parameter with its value set to false
//                    params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

//                    //necessary for certificate validation.By default,certificate validation is done 
//                    params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();

//                    //By default, QC statement is checked,and signature wont be created if it is not a 
//                    //qualified certificate. 

//                    bool checkQCStatement = TestConstants.getCheckQCStatement();
//                    //Get qualified or non-qualified certificate.
//                    ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);

//                    //BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);
//                    BaseSigner signer = SmartCardManager.getInstance().getSigner(tbPinKodu.Text, cert);

//                    //add signer
//                    //Since the specified attributes are mandatory for bes,null is given as parameter 
//                    //for optional attributes

//                    try
//                    {
//                        // 3-b İmza CADES ile atılmalıdır
//                        bs.addSigner(ESignatureType.TYPE_BES, cert, signer, null, params_);
//                    }
//                    catch (CertificateValidationException cve)
//                    {
//                        Console.WriteLine((string)cve.getCertStatusInfo().getDetailedMessage());

//                    }

//                    SmartCardManager.getInstance().logout();
//                    bs.detachContent();
//                    // 3-d İmza değeri byte olarak dönmelidir.
//                    byte[] signedDocument = bs.getEncoded();

//                    //write the contentinfo to file
//                    DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
//                    AsnIO.dosyayaz(signedDocument, di.FullName + @"\BES-1.p7s");

//                    SignedDataValidationResult sdvr = ValidationUtil.validate(signedDocument, null);

//                    //Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
//                    Application.DoEvents();
//                    bs = null;
//                    cert = null;
//                    signStream = signedDocument;
//                    if (signStream.Length > 0)
//                    {
//                        ImzaliXMLKaydet((Double)(reader["FisNo"]), 0, EgitimAltKodu, "EgitimlerDetay", System.Convert.ToBase64String(signStream));
//                    }
//                    // SmartCardManager = null; SmartCardManager create edilmeden direk kullaniliyor.
//                    // Belki ayri nesne olarak create edip sonra null yapmak gerekebilir
//                }
//            }
//            finally
//            {
//                if (Main.ActiveForm != null) { Main.ActiveForm.Enabled = true; }
//            }
//            SQLConUpdate.Close();
//            //                    System.Console.WriteLine("esignerbase64:" + System.Convert.ToBase64String(signStream) + ":esignerbase64");
//            System.Console.WriteLine("esignerbase64:" + System.Convert.ToString(signStream) + ":esignerbase64");
//            if (System.Convert.ToBase64String(signStream) != "") Application.Exit();
//        }
//    }
//    else
//    {
//        DosyaKontrolu();
//        if (Program.Hata == 1) { Environment.Exit(1); }
//        if (Program.ImzalanacakVeriTipi == "xml")
//            XML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + signHelper.eReceteImzala(tBoxERecetePath.Text, tBoxSignedERecetePath.Text, Program.ParamXML, cbDosyayaYaz.Checked);
//        else
//            XML = signHelper.eReceteImzala(tBoxERecetePath.Text, tBoxSignedERecetePath.Text, Program.ParamXML, cbDosyayaYaz.Checked);
//        if (Program.Hata == 1) { Environment.Exit(1); }
//        // dosyaya yazilacaksa eReceteImzala icinde imzalanmis xsig dosyasi olusuyor 
//        // sonra bu dosya alttaki prosedurde okunup base64'e cevriliyor ve sonu _64.txt olacak sekilde kaydediliyor.
//        if (cbDosyayaYaz.Checked) dosyayiBase64Yaz(tBoxSignedERecetePath.Text.Replace(".xml", ".xsig"));
//        // ONEMLI, base64 olarak console ciktisi veriyorum. ikazanci
//        signStream = System.Text.Encoding.UTF8.GetBytes(XML);
//        // hata varsa parametreyi geri dondurmeden evvel cik
//        if (Program.Hata == 1) { Environment.Exit(1); }
//        if (cbDosyayaYaz.Checked) WriteByteArrayToFile(System.Convert.ToBase64String(signStream), Path.GetDirectoryName(tBoxSignedERecetePath.Text), Path.GetFileName(tBoxSignedERecetePath.Text) + ".xsig");
//        System.Console.WriteLine("esignerbase64:" + System.Convert.ToBase64String(signStream) + ":esignerbase64");
//        if (XML != "") Application.Exit();
//    }
//}

//static string ComputeSha256Hash(string rawData)
//{
//    // Create a SHA256   
//    using (SHA256 sha256Hash = SHA256.Create())
//    {
//        // ComputeHash - returns byte array  
//        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

//        // Convert byte array to a string   
//        StringBuilder builder = new StringBuilder();
//        for (int i = 0; i < bytes.Length; i++)
//        {
//            builder.Append(bytes[i].ToString("x2"));
//        }
//        return builder.ToString();
//    }
//}

//static byte[] ComputeSha256HashToByteArray(string rawData)
//{
//    // Create a SHA256   
//    using (SHA256 sha256Hash = SHA256.Create())
//    {
//        // ComputeHash - returns byte array  
//        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

//        // Convert byte array to a string   
//        StringBuilder builder = new StringBuilder();
//        for (int i = 0; i < bytes.Length; i++)
//        {
//            builder.Append(bytes[i].ToString("x2"));
//        }
//        return bytes;
//    }
//}


// eimza rapor gonderim esignerda kullanilan deneme parametresi:
// "^<eraporBilgisi^>  ^<tesisKodu^>10343044^</tesisKodu^>  ^<raporTakipNo^>289246797^</raporTakipNo^>  ^<hastaTcKimlikNo^>39469570182^</hastaTcKimlikNo^>  ^<raporNo^>26038^</raporNo^>  ^<raporTarihi^>04.03.2019^</raporTarihi^>  ^<protokolNo^>201900020794^</protokolNo^>  ^<aciklama/^>  ^<klinikTani/^>  ^<raporDuzenlemeTuru^>2^</raporDuzenlemeTuru^>  ^<takipNo^>2TUX4J9^</takipNo^>  ^<raporOnayDurumu/^>  ^<eraporKisiBilgisi^>  ^<tcKimlikNo^>39469570182^</tcKimlikNo^>    ^<adi^>NECLA^</adi^>    ^<soyadi^>YILDIZ^</soyadi^>  ^</eraporKisiBilgisi^>  ^<eraporTeshisBilgisi^>    ^<raporTeshisKodu^>205^</raporTeshisKodu^>    ^<baslangicTarihi^>04.03.2019^</baslangicTarihi^>    ^<bitisTarihi^>27.02.2020^</bitisTarihi^>    ^<taniBilgisi^>      ^<kodu^>G62.9^</kodu^>      ^<adi^>^</adi^>    ^</taniBilgisi^>  ^</eraporTeshisBilgisi^>  ^<eraporDoktorBilgisi^>    ^<tcKimlikNo^>19601471324^</tcKimlikNo^>    ^<bransKodu^>1300^</bransKodu^>    ^<sertifikaKodu^>0^</sertifikaKodu^>  ^</eraporDoktorBilgisi^>  ^<eraporEtkinMaddeBilgisi^>    ^<etkinMaddeKodu^>SGKF39^</etkinMaddeKodu^>    ^<kullanimDoz1^>3^</kullanimDoz1^>    ^<kullanimDoz2^>600^</kullanimDoz2^>    ^<kullanimDozBirimi^>3^</kullanimDozBirimi^>    ^<kullanimDozPeriyot^>1^</kullanimDozPeriyot^>    ^<kullanimDozPeriyotBirimi^>3^</kullanimDozPeriyotBirimi^>    ^<etkinMaddeBilgisi^>      ^<kodu^>SGKF39^</kodu^>      ^<adi^>GABAPENTIN  Ağızdan katı^</adi^>      ^<icerikMiktari/^>      ^<formu^>^</formu^>    ^</etkinMaddeBilgisi^>  ^</eraporEtkinMaddeBilgisi^>  ^<eraporAciklamaBilgisi^>    ^<aciklama^>Etken Madde:GABAPENTIN  Ağızdan katı Doz:3x600&Polinöropati, tanımlanmamış^</aciklama^>  ^</eraporAciklamaBilgisi^> ^</eraporBilgisi^>" "" "" "" "" "" "" "" "" "" "" "" "0" "" "" "" "" "78.188.44.147,4283""
