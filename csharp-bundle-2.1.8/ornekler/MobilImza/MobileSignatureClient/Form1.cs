using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.infra.mobile;
using tr.gov.tubitak.uekae.esya.asn.util;

namespace MobileSignatureClient
{
    public partial class Form1 : Form
    {
        private void loadLicense()
        {
            //write license path below
            FileStream fileStream = new FileStream("../../../../lisans/lisans.xml", FileMode.Open, FileAccess.Read);
            String p = "1.3.15";
            LicenseUtil.setLicenseXml(fileStream, p);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
            string fileName = openFileDialog1.FileName;
            if(fileName != null)
            {
                txtFilePath.Text = fileName;
            }
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            loadLicense();
            String filePath = txtFilePath.Text;
            byte[] contentData = AsnIO.dosyadanOKU(filePath);

            String phoneNumber = txtPhoneNumber.Text;
            int mobileOperator = cBoxOperator.SelectedIndex;

            
            BaseSignedData bs = new BaseSignedData();
            bs.addContent(new SignableByteArray(contentData));

            Dictionary<String, Object> params_ = new Dictionary<String, Object>();
            //write policy path below
            ValidationPolicy readValidationPolicy = PolicyReader.readValidationPolicy(new FileStream("../../../../ornek politika/policy.xml", FileMode.Open, FileAccess.Read));

            ValidationPolicy validationPolicy = new ValidationPolicy();
            params_[EParameters.P_CERT_VALIDATION_POLICY] = readValidationPolicy;
            //In real system, validate certificate by giving parameter "true" instead of "false"
            params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

            
            PhoneNumberAndOperator phoneNumberAndOperator = new PhoneNumberAndOperator(phoneNumber,(Operator) mobileOperator);
            EMSSPClientConnector emsspClientConnector = new EMSSPClientConnector();
            emsspClientConnector.setCertificateInitials(phoneNumberAndOperator);

            ECertificate signerCert = null;
            MobileSigner mobileSigner = new MobileSigner(emsspClientConnector, phoneNumberAndOperator, signerCert, "Dosya imzalanacak",SignatureAlg.RSA_SHA1.getName());
            bs.addSigner(ESignatureType.TYPE_BES,signerCert,mobileSigner, null, params_);

            AsnIO.dosyayaz(bs.getEncoded(), "ServisImzali.dat");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}