using System;
using System.IO;
using NUnit.Framework;
using log4net.Config;
using tr.gov.tubitak.uekae.esya.api.common;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation
{
    /**
     * Sample for signing a document only if certificate is valid
     * @author suleyman.uslu
     */
    [TestFixture]
    public class CertValidationBeforeSignPfx : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "validate_before_sign.xml";


        [TestFixtureSetUp]
        public static void setUp()
        {
            XmlConfigurator.Configure(new FileInfo(ROOT_DIR + "config\\log4net.xml"));
            loadLicense();
        }
        /**
         * Creates BES with only valid certificates
         */
        [Test]
        public void createBESWithCertificateCheck()
        {
            try
            {
                // check validity of signing certificate
                bool valid = CertValidation.validateCertificate(CERTIFICATE);

                if(valid)
                {
                    // create context with working directory
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

                    // add certificate to show who signed the document
                    signature.addKeyInfo(CERTIFICATE);

                    // now sign it by using signer
                    signature.sign(SIGNER);

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

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
