using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using tr.gov.tubitak.uekae.esya.api.cades.example.validation;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.profile;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.infra.tsclient;
using tr.gov.tubitak.uekae.esya.asn.util;

namespace tr.gov.tubitak.uekae.esya.api.cades.example.sign.profil
{
    [TestFixture]
    class P2
    {

        [Test]
        public void testCreateP2()
        {
            BaseSignedData bs = new BaseSignedData();

            ISignable content = new SignableByteArray(Encoding.ASCII.GetBytes("test"));
            bs.addContent(content);

             List<IAttribute> optionalAttributes = new List<IAttribute>();
            optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));
            optionalAttributes.Add(new SignaturePolicyIdentifierAttr(TurkishESigProfile.P2_1));

            Dictionary<String, Object> params_ = new Dictionary<String, Object>();

            //if the user does not want certificate validation,he can add 
            //P_VALIDATE_CERTIFICATE_BEFORE_SIGNING parameter with its value set to false
            params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

            bool checkQCStatement = TestConstants.getCheckQCStatement();
            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
            
            //necessary for getting signature time stamp.
            TSSettings tsSettings = TestConstants.getTSSettings();
            params_[EParameters.P_TSS_INFO] = tsSettings;


            //Get qualified or non-qualified certificate.
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement,
                                                                                       !checkQCStatement);
            BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);

            //add signer
            bs.addSigner(ESignatureType.TYPE_EST, cert, signer, optionalAttributes, params_);

            SmartCardManager.getInstance().logout();

            byte[] signedDocument = bs.getEncoded();

            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            AsnIO.dosyayaz(signedDocument, di.FullName + @"\P2.p7s");

            SignedDataValidationResult sdvr = ValidationUtil.validate(signedDocument, null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
            
        }
    }
}
