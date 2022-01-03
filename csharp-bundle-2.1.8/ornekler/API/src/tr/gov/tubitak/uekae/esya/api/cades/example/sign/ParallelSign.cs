using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using tr.gov.tubitak.uekae.esya.api.cades.example.validation;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.smartcard.pkcs11;
using tr.gov.tubitak.uekae.esya.asn.util;

/**
 * Creates content info structures that has several paralel signatures
 * @author orcun.ertugrul
 */

namespace tr.gov.tubitak.uekae.esya.api.cades.example.sign
{
    [TestFixture]
    public class ParallelSign
    {
        public readonly int SIGNATURE_COUNT = 5;

        /**
         * Creates a signature structure that has two different signers. Each signer has signatures as much as SIGNATURE_COUNT
         * variable.
         * @throws Exception
         */


        [Test]
        public void testParallelSign()
        {
            bool checkQCStatement = TestConstants.getCheckQCStatement();

            SmartCardManager scm = SmartCardManager.getInstance();
            //Get Non-Qualified certificate.
            ECertificate cert = scm.getSignatureCertificate(checkQCStatement, !checkQCStatement);
            BaseSigner signer = scm.getSigner(TestConstants.getPIN(), cert);

            byte[] signature = null;
            byte[] content = Encoding.ASCII.GetBytes("test");
            for (int i = 0; i < SIGNATURE_COUNT; i++)
            {
                //adds a paralel signature of signer
                signature = sign(content, signature, signer, cert);
            }

            try
            {
                scm.logout();
            }
            catch (SmartCardException sce)
            {
                Console.WriteLine((object) sce);
            }
            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            AsnIO.dosyayaz(signature, di.FullName + @"\paralelSignatures.p7s");

            

            SignedDataValidationResult sdvr = ValidationUtil.validate(signature, null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }

        [Test]
        public void testParallelSignWith2Signer()
        {
            bool checkQCStatement = TestConstants.getCheckQCStatement();

            //Wants user to select two cards for parallel signatures. 
            Console.WriteLine("Select card - 1");
            //SmartCard - 1
            SmartCardManager scm1 = SmartCardManager.getInstance();
            //Get Non-Qualified certificate.
            ECertificate cert1 = scm1.getSignatureCertificate(checkQCStatement, !checkQCStatement);
            BaseSigner signer1 = scm1.getSigner(TestConstants.getPIN(), cert1);


            //reset to select second card.
            SmartCardManager.reset();


            Console.WriteLine("Select card - 2");
            //SmartCard - 2 
            SmartCardManager scm2 = SmartCardManager.getInstance();
            //Get Non-Qualified certificate.
            ECertificate cert2 = scm2.getSignatureCertificate(checkQCStatement, !checkQCStatement);
            BaseSigner signer2 = scm2.getSigner(TestConstants.getPIN(), cert2);

            SmartCardManager.reset();


            byte[] signature = null;
            byte[] content = Encoding.ASCII.GetBytes("test");
            for (int i = 0; i < SIGNATURE_COUNT; i++)
            {
                //adds a paralel signature of signer one
                signature = sign(content, signature, signer1, cert1);
                //adds a paralel signature of signer two
                signature = sign(content, signature, signer2, cert2);
            }

            try
            {
                scm1.logout();
                scm2.logout();
            }
            catch (SmartCardException sce)
            {
                Console.WriteLine((object) sce);
            }

            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            AsnIO.dosyayaz(signature, di.FullName + @"\paralelSignatures2.p7s");


            SignedDataValidationResult sdvr = ValidationUtil.validate(signature, null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }

        /**
         * adds a parallel signature to a signature structure.
         * @param content
         * @param sign
         * @param signer
         * @param cert
         * @return
         * @throws Exception
         */

        private byte[] sign(byte[] content, byte[] sign, BaseSigner signer, ECertificate cert)
        {
            BaseSignedData bs;

            Dictionary<string, object> params_ = new Dictionary<string, object>();

            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
            

            //If sign is null, there is no signature before.
            if (sign == null)
            {
                bs = new BaseSignedData();
                bs.addContent(new SignableByteArray(content));
            }
            else
            {
                bs = new BaseSignedData(sign);
            }

            //add signer
            bs.addSigner(ESignatureType.TYPE_BES, cert, signer, null, params_);

            //returns content info
            return bs.getEncoded();
        }
    }
}