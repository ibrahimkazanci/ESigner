using System;
using System.Text;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using tr.gov.tubitak.uekae.esya.api.cades.example.validation;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.infra.tsclient;

/**
 * Test SignerManager class runs correctly.
 * @author orcun.ertugrul
 *
 */

namespace tr.gov.tubitak.uekae.esya.api.cades.example.util
{
    [TestFixture]
    public class TestSignerManager
    {
        /**
	 * Tests EST sign
	 * @throws Exception
	 */

        [Test]
        public void testEsxLong()
        {
            byte[] sign = null;
            SignableByteArray sba = new SignableByteArray(Encoding.ASCII.GetBytes("test"));
            ValidationPolicy policy = TestConstants.getPolicy();
            TSSettings tsSettings = TestConstants.getTSSettings();
            DigestAlg digestAlg = DigestAlg.SHA1;

            for (int i = 0; i < 5; i++)
            {
                sign = SignerManager.addSigner(sba, sign);
            }

            SignedDataValidationResult sdvr = ValidationUtil.validate(sign, sba);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }

        /**
         * Tests BES sign
         * @throws Exception
         */

        [Test]
        public void testBESSign()
        {
            byte[] sign = null;
            SignableByteArray sba = new SignableByteArray(Encoding.ASCII.GetBytes("test"));
            
            DateTime? signingTime = DateTime.UtcNow;

            for (int i = 0; i < 5; i++)
            {
                sign = SignerManager.addSignerWithSigningTimeAttr(sba, sign, signingTime);
            }

            SignedDataValidationResult sdvr = ValidationUtil.validate(sign, sba);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }
    }
}