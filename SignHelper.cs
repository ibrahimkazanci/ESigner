using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.certificate.validation;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.check.certificate;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.common;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.common.util.bag;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.smartcard.pkcs11;
using tr.gov.tubitak.uekae.esya.api.smartcard.gui;
using tr.gov.tubitak.uekae.esya.api.smartcard.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.config;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades;
using utils;
using log4net;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.keyinfo.x509;
using Context = tr.gov.tubitak.uekae.esya.api.xmlsignature.Context;
using XMLSignature = tr.gov.tubitak.uekae.esya.api.xmlsignature.XMLSignature;
using XMLSignatureException = tr.gov.tubitak.uekae.esya.api.xmlsignature.XMLSignatureException;
using ProgramNameSpace;
using ESigner;
using System.Data.SqlClient;
using System.Data;

namespace SignatureHelper
{
    class SignHelper
    {
        public IBaseSmartCard BaseSmartCard;
        public List<ECertificate> ListSertifikaListesi;
        private static SignHelper mSCManager;
        private static readonly ILog LOGGER = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool isValidCertificate(ECertificate certificate)
        {
            try
            {
                ValidationPolicy validationPolicy = CertValidationPolicyManager.getInstance().getValidationPolicy();
                ValidationSystem vs = CertificateValidation.createValidationSystem(validationPolicy);
                vs.setBaseValidationTime(DateTime.UtcNow);
                CertificateStatusInfo csi = CertificateValidation.validateCertificate(vs, certificate);
                CertificateStatus certificateStatus = csi.getCertificateStatus();
                string statusText = certificateStatus.textAl();
                //System.Console.WriteLine("Doğrulama Sonucu");
                //System.Console.WriteLine(statusText);
                //System.Console.WriteLine(csi.checkResultsToString());
                //System.Console.WriteLine(csi.getDetailedMessage());
                Program.HataMesaji = "\n\r" + csi.checkResultsToString() + "\n\r" + csi.getDetailedMessage();

                return (certificateStatus == CertificateStatus.VALID);
            }
            catch (Exception exc)
            {
                // System.Console.WriteLine(exc.Message);
                Program.HataMesaji = exc.Message;
                return false;
            }
        }

        public static int askOption(Control aParent, Icon aIcon, String[] aSecenekList, String aBaslik,
                                    String[] aOptions)
        {
            SlotList sl = new SlotList(null, aIcon, aSecenekList, aBaslik);
            DialogResult result = sl.ShowDialog();
            if (result != DialogResult.OK)
                return -1;
            return sl.getSelectedIndex();
        }
        

        public static void MesajiIsle(string Mesaj, byte Fatal)
        {
            if (Program.ParamOto == "0")
            {
                if (Fatal == 1) { Program.Hata = 1; }
                MessageBox.Show(Mesaj);
            }
            else
            {
                System.Console.WriteLine(Mesaj);
                if (Fatal == 1)
                {
                    Program.Hata = 1;
                    Environment.Exit(1);
                }
            }
        }


        public string eReceteImzala(string eReceteSourceFilePath, string eReceteDestFilePath, string GelenXML, Boolean DosyayaYaz)
        {
            string retSignedXmlPath = null;
            try
            {
                // create context with working dir
                string currentDirectory = Directory.GetCurrentDirectory();
                Context context = new Context(currentDirectory);
                // create signature according to context,
                // with default type (XADES_BES)
                // context.Config = new Config(Assembly.GetExecutingAssembly().GetManifestResourceStream("xmlsignature-config.xml"));
                XMLSignature signature = new XMLSignature(context);
                signature.SigningTime = DateTime.Now;
                if (GelenXML != "")
                {
                    InMemoryDocument inMMDoc = new InMemoryDocument(System.Text.Encoding.UTF8.GetBytes(GelenXML), "", null, null);
                    signature.addDocument(inMMDoc);
                }
                else
                {
                    // add document as reference, and keep BASE64 version of data
                    // in an <Object tag, in a way that reference points to
                    // that <Object
                    // (embed=true)                                 null
                    signature.addDocument(eReceteSourceFilePath, "text/plain", true);
                }
                // bu kismin alternatifi TestEnvelopingSignature dosyasinda var
                // if (Program.ParamTCKimlikNo != "")
                if (Program.ParamOto == "1")
                {
                    SmartCardManagerKimlikNodanSec smc = SmartCardManagerKimlikNodanSec.getInstance(0); // 0 server version
                    // sanirim smc nesnesi getInstance icinde uygun karta gore olusuyor... altta masaustu icin de uygula... 10.12.2015
                    if (Program.KartOkuyucuYok == 1) return null;
                    ECertificate signingCert = smc.getSignatureCertificate(true, false);

                    //İlk parameter Kart Pin
                    BaseSigner baseSigner = smc.getSigner(Program.PinKodu, signingCert); // "12345"
                    bool validCertificate = isValidCertificate(signingCert);
                    if (!validCertificate)
                    {
                        MesajiIsle("İmza atılmak istenen sertifika geçerli değil." + Program.HataMesaji, 1);
                        return null;
                    }
                    // add certificate to show who signed the document
                    signature.addKeyInfo(signingCert);
                    //Signer Oluşturma
                    signature.sign(baseSigner);
                }
                else
                {
                    // 1 desktop version
                    // imzalama oncesi kartta instance acma
                    SmartCardManagerKimlikNodanSec smc = SmartCardManagerKimlikNodanSec.getInstance(1); // 1 desktop version
                    // smc nesnesi getInstance icinde uygun karta gore olusacak
                    if (Program.KartOkuyucuYok == 1) return null;
                    ECertificate signingCert = smc.getSignatureCertificate(true, false);

                    //İlk parameter Kart Pin
                    BaseSigner baseSigner = smc.getSigner(Program.PinKodu, signingCert); // "12345"
                    if (baseSigner == null) return null;
                    bool validCertificate = isValidCertificate(signingCert);
                    if (!validCertificate)
                    {
                        MesajiIsle("İmza atılmak istenen sertifika geçerli değil." + Program.HataMesaji, 1);
                        return null;
                    }

                    if (Program.SertifikaBilgisi != "Sertifika ve Sahiplik Bilgisi: " + signingCert.ToString())
                    {
                        if (Program.SertifikaBilgisi == "Sertifika ve Sahiplik Bilgisi: ")
                        {
                            MesajiIsle("Akıllı kartı, imza ekranına girmeden evvel takınız." + Program.HataMesaji, 1);
                            return null;
                        }
                        MesajiIsle("Akıllı kart, imza ekranına girildikten sonra değiştirilmiş, işlemi kart değiştirmeden yapınız." + Program.HataMesaji, 1);
                        return null;
                    }
                    // add certificate to show who signed the document
                    signature.addKeyInfo(signingCert);
                    //Signer Oluşturma
                    signature.sign(baseSigner);


                    // eski yontem
                    //if (Program.KartOkuyucuYok == 1) return null;
                    //ECertificate signingCert = smc.getSignatureCertificate(true, false);


                    //BaseSigner baseSigner = smc.getSigner(Program.PinKodu, signingCert); // "12345"
                    //bool validCertificate = isValidCertificate(signingCert);
                    //if (!validCertificate)
                    //{
                    //    MesajiIsle("İmza atılmak istenen sertifika geçerli değil." + Program.HataMesaji, 1);
                    //    return null;
                    //}
                    //// add certificate to show who signed the document
                    //signature.addKeyInfo(signingCert);
                    ////Signer Oluşturma
                    //signature.sign(baseSigner);
                }


                //FileInfo sourceFileInfo = new FileInfo(eReceteSourceFilePath);
                //string destDirPath = sourceFileInfo.Directory.FullName;
                if (DosyayaYaz)
                {
                    retSignedXmlPath = eReceteDestFilePath.Replace(".xml", ".xsig");
                    FileStream signatureFileStream = new FileStream(retSignedXmlPath, FileMode.Create);

                    signature.write(signatureFileStream);
                    signatureFileStream.Close();

                    // mesaji main'de button click sonundan buraya aldim
                    if (retSignedXmlPath != null)
                    {
                        // tBoxSignedERecetePath.Text = signedFilePath;
                        MesajiIsle(retSignedXmlPath + " konumunda imzalı E-Reçete oluşturuldu.", 0);
                    }
                }
                else
                {
                    // MesajiIsle("E-Reçete imza verisi hazır", 0);
                }
                // return retSignedXmlPath;

                return signature.Document.OuterXml;
            }
            catch (XMLSignatureRuntimeException exc)
            {
                // cant create signature
                MesajiIsle("Hata Oluştu." + exc.Message, 1);

            }
            catch (XMLSignatureException exc)
            {
                // cant create signature
                MesajiIsle("Hata Oluştu." + exc.Message, 1);
            }
            catch (Exception exc)
            {
                // probably couldn't write to the file
                MesajiIsle("Hata Oluştu." + exc.Message, 1);
            }
            // return retSignedXmlPath;
            return "";
        }

        public static SqlConnection SQLConUpdate = new SqlConnection();
        private static string sql;
        public static SqlConnection SQLConBrowse = new SqlConnection();
        public static SqlDataReader reader;

        public static void BaglanVeriAl(object sender, EventArgs e)
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
            // sql olustur 
            String SqlCumlesi = "";
            SqlCumlesi = "select * from EgitimlerDetay where FisNo = @FisNo and isnull(Durum,0) <> 1";

            // dbye baglan
            SQLConBrowse.ConnectionString = "server=" + Program.ParamSQLServer + ";user=" + Program.ParamSQLUser + ";pwd=" + Program.ParamSQLPassword + ";database=" + Program.ParamSQLDB;
            SqlCommand qryVeriOku = new SqlCommand(SqlCumlesi, SQLConBrowse);
            try
            {
                SQLConBrowse.Open();
                qryVeriOku.Parameters.Add("@FisNo", SqlDbType.Float).Value = Program.KayitBilgisi[3];
                reader = qryVeriOku.ExecuteReader();
                //Main.txtBilgi.Text += "Bağlandı\r\n";
            }
            catch (Exception Exception1)
            {
                MessageBox.Show($"Kayıtlar alınamadı: {Exception1.Message}");
            }
        }

        public static void ImzaliXMLKaydet(double FisNo, int SiraNo, string EgitimAltKodu, string Tablo, string ImzaliXML)
        {
            // var connetionString = "Data Source=EVOPC18\\PMSMART;Initial Catalog=NORTHWND;User ID=test;Password=test";
            if (Tablo == "EgitimlerDetay")
            {
                sql = "UPDATE EgitimlerDetay SET ImzaliXML = @ImzaliXML where FisNo = @FisNo and EgitimAltKodu = @EgitimAltKodu ";
            }
            if (Tablo == "KurumRiskFisD")
            {
                sql = "UPDATE KurumRiskFisD SET ImzaliXML = @ImzaliXML where FisNo = @FisNo and SiraNo = @SiraNo ";
            }

            try
            {
                using (SQLConUpdate)
                {
                    using (var command = new SqlCommand(sql, SQLConUpdate))
                    {
                        command.Parameters.Add("@FisNo", SqlDbType.Float).Value = FisNo;
                        command.Parameters.Add("@ImzaliXML", SqlDbType.Char).Value = ImzaliXML;
                        if (Tablo == "EgitimlerDetay")
                        {
                            //
                            command.Parameters.Add("@EgitimAltKodu", SqlDbType.Char).Value = EgitimAltKodu; // iptal basarili ise ilgili kaydin durumunu da gonderilmemise cek
                        }
                        else
                        {
                            command.Parameters.Add("@SiraNo", SqlDbType.Int).Value = 1; // gonderim basarili ise ilgili kaydin durumunu gonderildi yap
                        }
                        // repeat for all variables....
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception Exception1)
            {
                MessageBox.Show($"Kayıt güncellenemedi: {Exception1.Message}");
            }

        }

        public static string Imzala()
        {
            try
            {
                // create context with working dir
                string currentDirectory = Directory.GetCurrentDirectory();
                Context context = new Context(currentDirectory);

                // imzalama oncesi kartta instance acma
                SmartCardManagerKimlikNodanSec smc = SmartCardManagerKimlikNodanSec.getInstance(1); // 1 desktop version
                                                                                                    // smc nesnesi getInstance icinde uygun karta gore olusacak
                if (Program.KartOkuyucuYok == 1) return null;
                ECertificate signingCert = smc.getSignatureCertificate(true, false);

                bool validCertificate = isValidCertificate(signingCert);
                if (!validCertificate)
                {
                    MesajiIsle("İmza atılmak istenen sertifika geçerli değil." + Program.HataMesaji, 1);
                    return null;
                }

                if (Program.SertifikaBilgisi != "Sertifika ve Sahiplik Bilgisi: " + signingCert.ToString())
                {
                    if (Program.SertifikaBilgisi == "Sertifika ve Sahiplik Bilgisi: ")
                    {
                        MesajiIsle("Akıllı kartı, imza ekranına girmeden evvel takınız." + Program.HataMesaji, 1);
                        return null;
                    }
                    MesajiIsle("Akıllı kart, imza ekranına girildikten sonra değiştirilmiş, işlemi kart değiştirmeden yapınız." + Program.HataMesaji, 1);
                    return null;
                }

                BaglanVeriAl(null, new EventArgs());

                if (reader.HasRows)
                {
                    // create signature according to context with default type (XADES_BES)
                    string ImzalanacakXML, EgitimAltKodu;
                    byte[] signStream;
                    signStream = System.Text.Encoding.UTF8.GetBytes("");

                    XMLSignature signature = new XMLSignature(context);

                    // otomatik hepsini gonder
                    SQLConUpdate.ConnectionString = "server=" + Program.ParamSQLServer + ";user=" + Program.ParamSQLUser + ";pwd=" + Program.ParamSQLPassword + ";database=" + Program.ParamSQLDB;
                    SQLConUpdate.Open();
                    while (reader.Read())
                    {
                        ImzalanacakXML = reader["XML"].ToString().Trim();
                        EgitimAltKodu = reader["EgitimAltKodu"].ToString().Trim();
                        //SiraNo = (double)reader["SiraNo"];
                        if (ImzalanacakXML != "")
                        {
                            InMemoryDocument inMMDoc = new InMemoryDocument(System.Text.Encoding.UTF8.GetBytes(ImzalanacakXML), "", null, null);
                            signature.addDocument(inMMDoc);
                        }
                        // add certificate to show who signed the document
                        signature.SigningTime = DateTime.Now;
                        signature.addKeyInfo(signingCert);
                        //Signer Oluşturma
                        //İlk parameter Kart Pin
                        BaseSigner baseSigner = smc.getSigner(Program.PinKodu, signingCert); // "12345"
                        if (baseSigner == null) return null;
                        signature.sign(baseSigner);
                        if (Program.Hata == 1) { Environment.Exit(1); }
                        // ONEMLI, base64 olarak console ciktisi veriyorum. ikazanci
                        signStream = System.Text.Encoding.UTF8.GetBytes(signature.Document.OuterXml);
                        // hata varsa parametreyi geri dondurmeden evvel cik
                        if (Program.Hata == 1) { Environment.Exit(1); }
                        if (signature.Document.OuterXml != "")
                        {
                            ImzaliXMLKaydet(Convert.ToDouble(Program.KayitBilgisi[3]), 0, EgitimAltKodu, "EgitimlerDetay", System.Convert.ToBase64String(signStream));
                        }
                        //baseSigner = null;
                        //signature = null;
                        //signingCert = null;
                        smc.logout();
                        smc = null;
                    }
                    SQLConUpdate.Close();
                    return signature.Document.OuterXml;
                }


            }
            catch (XMLSignatureRuntimeException exc)
            {
                // cant create signature
                MesajiIsle("Hata Oluştu." + exc.Message, 1);

            }
            catch (XMLSignatureException exc)
            {
                // cant create signature
                MesajiIsle("Hata Oluştu." + exc.Message, 1);
            }
            catch (Exception exc)
            {
                // probably couldn't write to the file
                MesajiIsle("Hata Oluştu." + exc.Message, 1);
            }
            return "";
        }

        public string upgradeToEST(String signedEReceteFilePath)
        {
            LisansHelper.loadLicense();
            String retSignedXmlPath = null;
            try
            {
                //create context with working dir
                string currentDirectory = Directory.GetCurrentDirectory();
                Context context = new Context(currentDirectory);
                XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(signedEReceteFilePath)), context);
                signature.upgradeToXAdES_T();
                FileInfo sourceFileInfo = new FileInfo(signedEReceteFilePath);
                string destDirPath = sourceFileInfo.Directory.FullName;
                retSignedXmlPath = destDirPath + "/" + sourceFileInfo.Name.Replace(".xsig", "_EST.xsig");
                FileStream signatureFileStream = new FileStream(retSignedXmlPath, FileMode.Create);
                signature.write(signatureFileStream);
                signatureFileStream.Close();
            }
            catch (XMLSignatureException exc)
            {
                // cant create signature
                MesajiIsle("Hata Oluştu." + exc.Message,1);
            }
            catch (Exception exc)
            {
                // probably couldn't write to the file
                MesajiIsle("Hata Oluştu." + exc.Message,1);
            }
            return retSignedXmlPath;
        }

        public string verifySignature(String signedEReceteFilePath)
        {
            String retStr = null;
            try
            {
                //create context with working dir
                string currentDirectory = Directory.GetCurrentDirectory();
                Context context = new Context(currentDirectory);

                XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(signedEReceteFilePath)), context);

                ValidationResult vr = signature.verify();
                System.Console.Out.WriteLine(vr.toXml());
                retStr = vr.toXml();
                //Seri imzalar için
                /*
                UnsignedSignatureProperties usp = signature.QualifyingProperties.UnsignedSignatureProperties;
                if (usp != null)
                {
                    IList<XMLSignature> allCounterSignatures = usp.AllCounterSignatures;
                    foreach (XMLSignature counterSignature in allCounterSignatures)
                    {
                        ValidationResult counterResult = counterSignature.verify();
                        System.Console.Out.WriteLine(counterResult.toXml());
                    }
                }*/
            }
            catch (XMLSignatureException exc)
            {
                // cant create signature
                MesajiIsle("Hata Oluştu." + exc.Message,1);
            }
            catch (Exception exc)
            {
                // probably couldn't write to the file
                MesajiIsle("Hata Oluştu." + exc.Message,1);
            }
            return retStr;
        }

        public string addSerialSignature(String signedEReceteFilePath)
        {
            LisansHelper.loadLicense();
            String retSignedXmlPath = null;
            try
            {
                //create context with working dir
                string currentDirectory = Directory.GetCurrentDirectory();
                Context context = new Context(currentDirectory);
                XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(signedEReceteFilePath)), context);

                // create counter signature
                XMLSignature counterSignature = signature.createCounterSignature();
                counterSignature.SigningTime = DateTime.Now;

                // sign
                // add certificate to show who signed the document
                KeyOrSmartCardSignManager keyOrSmartCardSignManager = KeyOrSmartCardSignManager.Instance;
                ECertificate signingCert = keyOrSmartCardSignManager.getSigningCertificate();

                counterSignature.addKeyInfo(signingCert);
                // now sign it by using smart card
                // now sign it
                BaseSigner baseSigner = keyOrSmartCardSignManager.getSigner(signingCert);
                counterSignature.sign(baseSigner);


                FileInfo sourceFileInfo = new FileInfo(signedEReceteFilePath);
                string destDirPath = sourceFileInfo.Directory.FullName;
                retSignedXmlPath = destDirPath + "/" + sourceFileInfo.Name.Replace(".xsig", "_Counter.xsig");
                FileStream signatureFileStream = new FileStream(retSignedXmlPath, FileMode.Create);
                signature.write(signatureFileStream);
                signatureFileStream.Close();
            }
            catch (XMLSignatureException exc)
            {
                // cant create signature
                MesajiIsle("Hata Oluştu." + exc.Message,1);
            }
            catch (Exception exc)
            {
                // probably couldn't write to the file
                MesajiIsle("Hata Oluştu." + exc.Message,1);
            }
            return retSignedXmlPath;
        }
        public string createParalelSignature(string eReceteSourceFilePath)
        {
            string retSignedXmlPath = null;
            //Load license from file
            //LisansHelper.loadFreeLicenseBase64();
            LisansHelper.loadLicense();
            try
            {
                // create context with working dir
                string currentDirectory = Directory.GetCurrentDirectory();
                Context context = new Context(currentDirectory);
                SignedDocument signatures = new SignedDocument(context);


                //First Signature
                XMLSignature signature1 = signatures.createSignature();
                signature1.SigningTime = DateTime.Now;
                signature1.addDocument(eReceteSourceFilePath, null, true);
                KeyOrSmartCardSignManager keyOrSmartCardSignManager = KeyOrSmartCardSignManager.Instance;
                ECertificate signingCert = keyOrSmartCardSignManager.getSigningCertificate();
                bool validCertificate = isValidCertificate(signingCert);
                if (!validCertificate)
                {
                    MesajiIsle("İmza atılmak istenen sertifika geçerli değil." + Program.HataMesaji,1);
                    return null;
                }
                // add certificate to show who signed the document
                signature1.addKeyInfo(signingCert);
                BaseSigner smartCardSigner = keyOrSmartCardSignManager.getSigner(signingCert);
                signature1.sign(smartCardSigner);

                //Second Signature
                XMLSignature signature2 = signatures.createSignature();
                signature2.SigningTime = DateTime.Now;
                signature2.addDocument(eReceteSourceFilePath, null, true);
                signature2.addKeyInfo(signingCert);
                signature2.sign(smartCardSigner);

                FileInfo sourceFileInfo = new FileInfo(eReceteSourceFilePath);
                string destDirPath = sourceFileInfo.Directory.FullName;
                retSignedXmlPath = destDirPath + "/" + sourceFileInfo.Name.Replace(".xml", "_PARALEL.xsig");
                FileStream signatureFileStream = new FileStream(retSignedXmlPath, FileMode.Create);
                signatures.write(signatureFileStream);
                signatureFileStream.Close();
                return retSignedXmlPath;
            }
            catch (XMLSignatureException exc)
            {
                // cant create signature
                MesajiIsle("Hata Oluştu." + exc.Message,1);
            }
            catch (Exception exc)
            {
                // probably couldn't write to the file
                MesajiIsle("Hata Oluştu." + exc.Message,1);
            }
            return retSignedXmlPath;
        }
    }
}