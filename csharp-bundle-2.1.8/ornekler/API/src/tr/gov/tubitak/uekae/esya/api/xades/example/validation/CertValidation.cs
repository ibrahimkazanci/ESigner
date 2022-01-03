using System;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.certificate.validation;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.check.certificate;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation
{
    /**
     * Provides validation functions for certificates
     * @author: suleyman.uslu
     */
    public class CertValidation : SampleBase
    {
        /**
         * Validates given certificate
         */
        public static Boolean validateCertificate(ECertificate certificate)
        {
            try
            {
                // generate policy which going to be used in validation
                ValidationPolicy policy = new ValidationPolicy();
                String policyPath = ROOT_DIR + "/config/certval-policy-test.xml";
                policy = PolicyReader.readValidationPolicy(policyPath);

                // generate validation system
                ValidationSystem vs = CertificateValidation.createValidationSystem(policy);
                vs.setBaseValidationTime(DateTime.UtcNow);

                // validate certificate
                CertificateStatusInfo csi = CertificateValidation.validateCertificate(vs, certificate);

                // return true if certificate is valid, false otherwise
                if (csi.getCertificateStatus() != CertificateStatus.VALID)
                    return false;
                else if (csi.getCertificateStatus() == CertificateStatus.VALID)
                    return true;
            }
            catch(Exception e)
            {
                throw new Exception("An error occured while validating certificate", e);
            }
            return false;
        }

    }
}
