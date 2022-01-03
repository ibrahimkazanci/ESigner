using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.attributes
{
    /**
     * BES with SignerRole attribute sample
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class SignerRoleAttribute : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "signer_role.xml";

        /**
         * Creates detached BES with SignerRole attribute
         */
        [Test]
        public void createBESWithSignerRole()
        {
            try
            {
                // create context with working directory
                Context context = createContext();

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

                // add signer role
                signature.QualifyingProperties.SignedSignatureProperties.SignerRole =
                        new SignerRole(context, new ClaimedRole[] { new ClaimedRole(context, "Manager") });

                // now sign it by using smart card
                // specifiy the PIN before sign
                signature.sign(SmartCardManager.getInstance().getSigner(PIN, cert));

                signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
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
