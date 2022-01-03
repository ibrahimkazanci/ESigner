using System;
using MobileSignatureClient.SignatureServiceStub;
using tr.gov.tubitak.uekae.esya.api.asn.cms;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.infra.mobile;
using tr.gov.tubitak.uekae.esya.asn.cms;

namespace MobileSignatureClient
{
    public class EMSSPClientConnector : MSSPClientConnector
    {
        SignatureServiceStub.Service1SoapClient signatureServiceClient = new Service1SoapClient();
        public void setCertificateInitials(UserIdentifier aUserID)
        {
            PhoneNumberAndOperator phoneNumberAndOperator = (PhoneNumberAndOperator) aUserID;
            signatureServiceClient.setCertificateInitials(phoneNumberAndOperator.getPhoneNumber(), (int)phoneNumberAndOperator.getOperator());
        }

        public byte[] sign(byte[] dataToBeSigned, SigningMode aMode, UserIdentifier aUserID, ECertificate aSigningCert, string informativeText, string aSigningAlg)
        {
            if(aMode!=SigningMode.SIGNHASH)
            {
                throw new Exception("Unsuported signing mode. Only SIGNHASH supported.");
            }

            PhoneNumberAndOperator phoneNumberAndOperator = (PhoneNumberAndOperator)aUserID;
            string dataTobeSigned64 = Convert.ToBase64String(dataToBeSigned);
            string signatureBase64 = signatureServiceClient.SignHash(dataTobeSigned64, informativeText, phoneNumberAndOperator.getPhoneNumber(), (int) phoneNumberAndOperator.getOperator());
           if(signatureBase64!=null)
           {
               return Convert.FromBase64String(signatureBase64);
           }
            return null;
        }

        public ECertificate getSigningCert()
        {
            return new ECertificate(Convert.FromBase64String(signatureServiceClient.getSigningCert()));
        }

        public SigningCertificate getSigningCertAttr()
        {
            return new ESigningCertificate(Convert.FromBase64String(signatureServiceClient.getSigningCertAttr())).getObject();
        }

        public SigningCertificateV2 getSigningCertAttrv2()
        {
            return new ESigningCertificateV2(Convert.FromBase64String(signatureServiceClient.getSigningCertAttrv2())).getObject();
        }

        public ESignerIdentifier getSignerIdentifier()
        {
            return new ESignerIdentifier(Convert.FromBase64String(signatureServiceClient.getSignerIdentifier()));
        }

        public DigestAlg getDigestAlg()
        {
            return DigestAlg.fromName(signatureServiceClient.getDigestAlg());
        }
    }
}