using System;
using System.Collections.Generic;
using System.IO;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.api.signature.certval;
using tr.gov.tubitak.uekae.esya.asn.util;

namespace tr.gov.tubitak.uekae.esya.api.cades.example.validation
{
    [TestFixture]
    public class ValidationUtil
    {
        [Test]
        public void testValidation1()
        {
            byte[] input =
                AsnIO.dosyadanOKU(TestConstants.getDirectory() +
                                  @"\testVerileri\ESXLong-1.p7s");

            SignedDataValidationResult sdvr = validate(input, null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());

            BaseSignedData bs = new BaseSignedData(input);
            Console.WriteLine((object) bs.getSignerList()[0].getSignerCertificate());
        }


        public static SignedDataValidationResult validate(byte[] signature, ISignable externalContent)
        {
            Dictionary<string, object> params_ = new Dictionary<string, object>();
            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
            if (externalContent != null)
                params_[EParameters.P_EXTERNAL_CONTENT] = externalContent;

            //Use only reference and their corresponding value to validate signature
            params_[EParameters.P_FORCE_STRICT_REFERENCE_USE] =true;
		    //Ignore grace period which means allow usage of CRL published before signature time 
            //params_[EParameters.P_IGNORE_GRACE] = true;

            //Use multiple policies if you want to use different policies to validate different types of certificate
           // CertValidationPolicies certificateValidationPolicies = new CertValidationPolicies();
           // certificateValidationPolicies.register(CertValidationPolicies.CertificateType.DEFAULT.ToString(), TestConstants.getPolicy());
           // ValidationPolicy maliMuhurPolicy = PolicyReader.readValidationPolicy(new FileStream(TestConstants.getDirectory()+"/config/certval-policy-malimuhur.xml", FileMode.Open, FileAccess.Read));
           // certificateValidationPolicies.register(CertValidationPolicies.CertificateType.MaliMuhurCertificate.ToString(), maliMuhurPolicy);
           // params_[EParameters.P_CERT_VALIDATION_POLICIES]= certificateValidationPolicies;

            SignedDataValidation sdv = new SignedDataValidation();
            SignedDataValidationResult sdvr = sdv.verify(signature, params_);

            return sdvr;
        }
    }
}