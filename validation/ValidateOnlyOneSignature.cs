using System;
using System.Collections.Generic;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.asn.util;

namespace tr.gov.tubitak.uekae.esya.api.cades.example.validation
{
    [TestFixture]
    public class ValidateOnlyOneSignature
    {
        [Test]
        public void testValidateOnlyASignature()
        {
            byte[] sign = AsnIO.dosyadanOKU(TestConstants.getDirectory() + @"\testVerileri\counterSignatures.p7s");

            Dictionary<string, object> params_ = new Dictionary<string, object>();
            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();

            BaseSignedData bs = new BaseSignedData(sign);

            CertificateRevocationInfoCollector collector = new CertificateRevocationInfoCollector();
            collector._extractAll(bs.getSignedData(),params_);

            List<Signer> sis = bs.getSignerList();


            foreach (Signer si in sis)
            {
                SignatureValidator sv = new SignatureValidator(sign);
                sv.setCertificates(collector.getAllCertificates());
                sv.setCRLs(collector.getAllCRLs());
                sv.setOCSPs(collector.getAllBasicOCSPResponses());
                SignatureValidationResult svr = new SignatureValidationResult();
                params_[AllEParameters.P_PARENT_SIGNER_INFO] = si.getSignerInfo();
                sv.verify(svr, si.getCounterSigners()[0], true, params_);

                Console.WriteLine((object) svr);
            }


        }
    }
}
