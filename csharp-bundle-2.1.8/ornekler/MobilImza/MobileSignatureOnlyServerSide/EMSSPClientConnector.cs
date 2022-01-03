using System;
using tr.gov.tubitak.uekae.esya.api.asn.cms;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.common;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.crypto.util;
using tr.gov.tubitak.uekae.esya.api.infra.mobile;
using tr.gov.tubitak.uekae.esya.api.webservice.mssclient.wrapper;
using tr.gov.tubitak.uekae.esya.asn.cms;

namespace MobileSignatureOnlyServerSide
{
    class EMSSPClientConnector : MSSPClientConnector
    {

        private EMSSPRequestHandler msspRequestHandler;
        public EMSSPClientConnector()
        {
            MSSParams mobilParams = new MSSParams("http://MImzaTubitakBilgem", "******", "www.turkcelltech.com");
            /*
            mobilParams.SetMsspSignatureQueryUrl("https://msign.turkcell.com.tr/MSSP2/services/MSS_Signature");
            //Test ortamı için "http://85.235.93.165/MSSP2/services/MSS_Signature"
            mobilParams.SetMsspProfileQueryUrl("https://msign.turkcell.com.tr/MSSP2/services/MSS_ProfileQueryPort");
            //Test ortamı için "http://85.235.93.165/MSSP2/services/MSS_ProfileQueryPort "
             */
            msspRequestHandler = new EMSSPRequestHandler(mobilParams);
        }

        public void setCertificateInitials(UserIdentifier aUserID)
        {
            PhoneNumberAndOperator phoneNumberAndOperator = (PhoneNumberAndOperator)aUserID;
            msspRequestHandler.setCertificateInitials(phoneNumberAndOperator);
        }

        public String calculateFingerPrintValue(byte[] dataToBeSigned)
        {
            String retFingerPrintStr = "";
            byte[] digestForSign = null;
            try
            {
                digestForSign = DigestUtil.digest(DigestAlg.SHA1, dataToBeSigned);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return null;
            }
            return StringUtil.ToString(digestForSign);
        }

        public byte[] sign(byte[] dataToBeSigned, SigningMode aMode, UserIdentifier aUserID, ECertificate aSigningCert, string informativeText, string aSigningAlg)
        {
            String fingerPrintStr = calculateFingerPrintValue(dataToBeSigned);
            if (fingerPrintStr == null)
            {
                throw new ESYAException("Parmak izi değeri hesaplanamadı.");
            }

            PhoneNumberAndOperator phoneNumberAndOperator = (PhoneNumberAndOperator)aUserID;
            return msspRequestHandler.Sign(dataToBeSigned, SigningMode.SIGNHASH, phoneNumberAndOperator, informativeText, aSigningAlg);
        }

        public ECertificate getSigningCert()
        {
            return msspRequestHandler.getSigningCert();
        }

        public SigningCertificate getSigningCertAttr()
        {
            return msspRequestHandler.getSigningCertAttr();
        }

        public SigningCertificateV2 getSigningCertAttrv2()
        {
            return msspRequestHandler.getSigningCertAttrv2();
        }

        public ESignerIdentifier getSignerIdentifier()
        {
            return msspRequestHandler.getSignerIdentifier();
        }

        public DigestAlg getDigestAlg()
        {
            return msspRequestHandler.getDigestAlg();
        }
    }
}
