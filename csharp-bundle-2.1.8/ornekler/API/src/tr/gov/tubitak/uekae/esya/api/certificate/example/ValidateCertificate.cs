using System;
using System.IO;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.certificate.validation;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.check.certificate;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;

namespace tr.gov.tubitak.uekae.esya.api.cades.example.validation
{
    [TestFixture]
    public class ValidateCertificate
    {
        private static String POLICY_FILE_NES=TestConstants.getDirectory()+@"\config\certval-policy.xml";
        private static String POLICY_FILE_MM=TestConstants.getDirectory()+@"\config\certval-policy-malimuhur.xml";

        public ValidationPolicy getPolicy(String fileName)
        {
            return PolicyReader.readValidationPolicy(new FileStream(fileName, FileMode.Open));
        }
        
        [Test]
        public void testValidateNESCertificate()
        {
            TestConstants.setLicence();
            bool QCStatement = true; //Qualified certificate
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(QCStatement, !QCStatement);

            ValidationSystem vs = CertificateValidation.createValidationSystem(getPolicy(POLICY_FILE_NES));
            vs.setBaseValidationTime(DateTime.UtcNow);
            CertificateStatusInfo csi = CertificateValidation.validateCertificate(vs, cert);
            if (csi.getCertificateStatus() != CertificateStatus.VALID)
                throw new Exception("Not Verified");
            
        }
        [Test]
        public void testValidateMMCertificate()
        {
            TestConstants.setLicence();
            bool QCStatement = false; //Unqualified certificate
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(QCStatement, !QCStatement);

            ValidationSystem vs = CertificateValidation.createValidationSystem(getPolicy(POLICY_FILE_MM));
            vs.setBaseValidationTime(DateTime.UtcNow);
            CertificateStatusInfo csi = CertificateValidation.validateCertificate(vs, cert);
            if (csi.getCertificateStatus() != CertificateStatus.VALID)
                throw new Exception("Not Verified");

        }
    }
}
