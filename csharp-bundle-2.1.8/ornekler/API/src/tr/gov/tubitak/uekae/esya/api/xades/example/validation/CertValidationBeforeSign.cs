using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.common;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation
{
    /**
     * Sample for signing a document with smart card only if certificate is valid
     * @author suleyman.uslu
     */
    public class CertValidationBeforeSign : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "validate_before_sign_smartcard.xml";

        /**
         * Creates BES using smart card with only valid certificates
         */
        [Test]
        public void createBESWithCertificateCheckSmartcard()
        {
            try
            {
                // false-true gets non-qualified certificates while true-false gets qualified ones
                ECertificate certificate = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);

                // check validity of signing certificate
                bool valid = CertValidation.validateCertificate(certificate);

                if(valid)
                {
                    // create context with working dir
                    Context context = new Context(BASE_DIR);

                    /* optional - specifying policy from code
                    // generate policy to be used in certificate validation
                    ValidationPolicy policy = PolicyReader.readValidationPolicy(POLICY_FILE);

                    CertValidationPolicies policies = new CertValidationPolicies();
                    // null means default
                    policies.register(null,policy);

                    context.Config.ValidationConfig.setCertValidationPolicies(policies);
                    */

                    // create signature according to context,
                    // with default type (XADES_BES)
                    XMLSignature signature = new XMLSignature(context);

                    // add document as reference, but do not embed it
                    // into the signature (embed=false)
                    signature.addDocument("./sample.txt", "text/plain", false);

                    signature.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

                    // add certificate to show who signed the document
                    signature.addKeyInfo(certificate);

                    // now sign it by using smart card
                    signature.sign(SmartCardManager.getInstance().getSigner(PIN, certificate));

                    signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
                }
                else
                {
                    throw new ESYAException("Certificate " + CERTIFICATE.ToString() + " is not a valid certificate!");
                }
            }
            catch (XMLSignatureException e)
            {
                // cannot create signature
                Assert.Fail("Error while signing" + e.StackTrace);
            }
            catch (Exception e)
            {
                // probably could not write to file
                Assert.Fail("Error while signing" + e.StackTrace);
            }
        }
    }
}
