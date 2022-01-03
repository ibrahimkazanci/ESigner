using System;
using System.IO;
using System.Web.Services;
using tr.gov.tubitak.uekae.esya.api.asn.cms;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.infra.mobile;
using tr.gov.tubitak.uekae.esya.api.webservice.mssclient.provider;
using tr.gov.tubitak.uekae.esya.api.webservice.mssclient.transaction.signature;
using tr.gov.tubitak.uekae.esya.api.webservice.mssclient.wrapper;

namespace MobilSignatureService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://localhost/webservices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        static MSSParams mobilParams = new MSSParams("http://MImzaTubitakBilgem", "*******", "www.turkcelltech.com");
		 /*
        mobilParams.SetMsspSignatureQueryUrl("https://msign.turkcell.com.tr/MSSP2/services/MSS_Signature");
        //Test ortamı için "http://85.235.93.165/MSSP2/services/MSS_Signature"
        mobilParams.SetMsspProfileQueryUrl("https://msign.turkcell.com.tr/MSSP2/services/MSS_ProfileQueryPort");
        //Test ortamı için "http://85.235.93.165/MSSP2/services/MSS_ProfileQueryPort "
         */
        EMSSPRequestHandler msspRequestHandler = new EMSSPRequestHandler(mobilParams);

        private void loadLicense()
        {
            //write license path below
            FileStream fileStream = new FileStream("C:/Users/int2/Desktop/lisans.xml", FileMode.Open);
            String p = "1.3.15";
            LicenseUtil.setLicenseXml(fileStream, p);
            fileStream.Close();
        }

        [WebMethod]
        public void setCertificateInitials(String phoneNumber, int iOperator)
        {
            loadLicense();
            Operator mobileOperator = (Operator) iOperator;
            PhoneNumberAndOperator phoneNumberAndOperator = new PhoneNumberAndOperator(phoneNumber, mobileOperator);
            msspRequestHandler.setCertificateInitials(phoneNumberAndOperator);
        }

        [WebMethod]
        public string SignHash(String hashForSign64, String displayText,String phoneNumber, int iOperator)
        {
            loadLicense();
            Operator mobileOperator = (Operator)iOperator;
            PhoneNumberAndOperator phoneNumberAndOperator = new PhoneNumberAndOperator(phoneNumber, mobileOperator);

            byte[] dataForSign = Convert.FromBase64String(hashForSign64);
            byte[] signedData = msspRequestHandler.Sign(dataForSign, SigningMode.SIGNHASH, phoneNumberAndOperator, displayText, null);
            return Convert.ToBase64String(signedData);
        }

        [WebMethod]
        public string getSigningCert()
        {
            loadLicense();
            return Convert.ToBase64String(msspRequestHandler.getSigningCert().getEncoded());
        }

        [WebMethod]
        public string getSigningCertAttr()
        {
            loadLicense();
            ESigningCertificate sc = new ESigningCertificate(msspRequestHandler.getSigningCertAttr());
            return Convert.ToBase64String(sc.getEncoded());
        }

        [WebMethod]
        public string getSigningCertAttrv2()
        {
            loadLicense();
            ESigningCertificateV2 sc2 = new ESigningCertificateV2(msspRequestHandler.getSigningCertAttrv2());
            return Convert.ToBase64String(sc2.getEncoded());
        }

        [WebMethod]
        public string getSignerIdentifier()
        {
            loadLicense();
            return Convert.ToBase64String(msspRequestHandler.getSignerIdentifier().getEncoded());
        }

        [WebMethod]
        public string getDigestAlg()
        {
            loadLicense();
            return msspRequestHandler.getDigestAlg().getName();
        }
    }
}