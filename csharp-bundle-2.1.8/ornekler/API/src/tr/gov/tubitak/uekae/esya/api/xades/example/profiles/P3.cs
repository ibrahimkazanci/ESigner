using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.signature.certval;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.profiles
{
    /**
     * Profile 3 sample
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class P3 : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "p3.xml";

        /**
         * Creates T type signature according to the profile 3 specifications. It must be
         * upgraded to XL type using the second function (upgradeP3). If 'use-validation-
         * data-published-after-creation' is true in xml signature config file, after signing,
         * at least one new CRL for signing certificate must be published before upgrade.
         * @throws Exception
         */
        [Test]
        public void createP3()
        {
            try
            {
                // create context with working directory
                Context context = createContext();

                // add resolver to resolve policies
                context.addExternalResolver(POLICY_RESOLVER);

                // create signature according to context,
                // with default type (XADES_BES)
                XMLSignature signature = new XMLSignature(context);

                // add document as reference, but do not embed it
                // into the signature (embed=false)
                signature.addDocument("./sample.txt", "text/plain", false);

                signature.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

                // add certificate to show who signed the document
                // arrange the parameters whether the certificate is qualified or not
                ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);
                signature.addKeyInfo(cert);

                // add signing time
                signature.SigningTime = DateTime.Now;

                // set policy info defined and required by profile
                signature.setPolicyIdentifier(OID_POLICY_P3,
                        "Uzun Dönemli ve SİL Kontrollü Güvenli Elektronik İmza Politikası",
                        "http://www.eimza.gov.tr/EimzaPolitikalari/216792161015070321.pdf"
                );

                // now sign it by using smart card
                // specifiy the PIN before sign
                signature.sign(SmartCardManager.getInstance().getSigner(PIN, cert));

                // upgrade to T
                signature.upgrade(tr.gov.tubitak.uekae.esya.api.signature.SignatureType.ES_T);

                signature.write(new FileStream(BASE_DIR + "p3_temp.xml", FileMode.OpenOrCreate));
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

        /**
         * Upgrades temporary T type signature to XL to create a signature with
         * profile 3 specifications. Do not run this upgrade code to be able to
         * validate using 'use-validation-data-published-after-creation' true
         * just afer creating temporary signature in the above function (createP3).
         * Also, revocation data must be CRL instead of OCSP in this profile.
         * @throws Exception
         */
        [Test]
        public void upgradeP3()
        {
            // create context with working dir
            Context context = createContext();

            // set policy such that it only works with CRL
            CertValidationPolicies policies = new CertValidationPolicies();
            policies.register(null, PolicyReader.readValidationPolicy(POLICY_FILE_CRL));

            context.Config.ValidationConfig.setCertValidationPolicies(policies);

            // add resolver to resolve policies
            context.addExternalResolver(POLICY_RESOLVER);

            // read temporary signature
            XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(BASE_DIR + "p3_temp.xml")),context);

            // upgrade to XL
            signature.upgrade(tr.gov.tubitak.uekae.esya.api.signature.SignatureType.ES_XL);

            signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
