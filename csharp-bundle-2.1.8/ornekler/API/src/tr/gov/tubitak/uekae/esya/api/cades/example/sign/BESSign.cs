using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using tr.gov.tubitak.uekae.esya.api.cades.example.validation;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.asn.util;

/**
 * This class shows creations BES type signature.
 * @author orcun.ertugrul
 *
 */
namespace tr.gov.tubitak.uekae.esya.api.cades.example.sign
{
    [TestFixture]
    public class BESSign
    {
        /**
	 * creates BES type signature and validate it.
	 * @throws Exception
	 */
        [Test]
        public static void testSimpleSign()
        {
            BaseSignedData bs = new BaseSignedData();
            ISignable content = new SignableByteArray(ASCIIEncoding.ASCII.GetBytes("test"));
            bs.addContent(content);

            Dictionary<string, object> params_ = new Dictionary<string, object>();
            //if the user does not want certificate validation at generating signature,he can add 
            //P_VALIDATE_CERTIFICATE_BEFORE_SIGNING parameter with its value set to false
            params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

            //necessary for certificate validation.By default,certificate validation is done 
            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();

            //By default, QC statement is checked,and signature wont be created if it is not a 
            //qualified certificate. 

            bool checkQCStatement = TestConstants.getCheckQCStatement();
            
            //Get qualified or non-qualified certificate.
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
            BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);

            //add signer
            //Since the specified attributes are mandatory for bes,null is given as parameter 
            //for optional attributes
            try
            {
                bs.addSigner(ESignatureType.TYPE_BES, cert, signer, null, params_);
            }
            catch (CertificateValidationException cve)
            {
                Console.WriteLine((string) cve.getCertStatusInfo().getDetailedMessage());
                
            }
            

            SmartCardManager.getInstance().logout();

            byte[] signedDocument = bs.getEncoded();

            //write the contentinfo to file
            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory()+@"\testVerileri");
            AsnIO.dosyayaz(signedDocument, di.FullName + @"\BES-1.p7s");
            
            SignedDataValidationResult sdvr = ValidationUtil.validate(signedDocument, null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }

        /**
         * creates BES type signature with signing time attribute and validate it.
         * @throws Exception
         */
        [Test]
        public static void testSigningTimeAttrSign()
        {
            BaseSignedData bs = new BaseSignedData();
            ISignable content = new SignableByteArray(ASCIIEncoding.ASCII.GetBytes("test"));
            bs.addContent(content);

            //Since SigningTime attribute is optional,add it to optional attributes list
            List<IAttribute> optionalAttributes = new List<IAttribute>();
            optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));

            Dictionary<string, object> params_ = new Dictionary<string, object>();
            ValidationPolicy policy = TestConstants.getPolicy();

            //necessary for certificate validation.By default,certificate validation is done 
            params_[EParameters.P_CERT_VALIDATION_POLICY] = policy;

            //if the user does not want certificate validation,he can add 
            //P_VALIDATE_CERTIFICATE_BEFORE_SIGNING parameter with its value set to false
            params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

            //By default, QC statement is checked,and signature wont be created if it is not a 
            //qualified certificate.
            bool checkQCStatement = TestConstants.getCheckQCStatement();
            
            //Get qualified or non-qualified certificate.
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
            BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);


            //add signer
            //Since the specified attributes are mandatory for bes,null is given as parameter 
            //for optional attributes
            bs.addSigner(ESignatureType.TYPE_BES, cert, signer, optionalAttributes, params_);

            SmartCardManager.getInstance().logout();

            byte[] signedDocument = bs.getEncoded();

            //write the contentinfo to file
            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            AsnIO.dosyayaz(signedDocument, di.FullName + @"\BES-2.p7s");
           

            SignedDataValidationResult sdvr = ValidationUtil.validate(signedDocument, null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }
    }
}
