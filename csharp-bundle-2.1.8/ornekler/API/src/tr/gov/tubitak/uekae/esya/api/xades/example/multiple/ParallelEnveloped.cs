using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.resolver;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.multiple
{
    /**
     * Parallel signature enveloped sample
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class ParallelEnveloped : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "parallel_enveloped.xml";

        /**
         * Creates two signatures in a document, that signs same inner data
         */
        [Test]
        public void createParallelEnveloped()
        {
            Context context = createContext();

            SignedDocument signatures = new SignedDocument(context);

            Document doc = Resolver.resolve("./sample.xml", context);
            String fragment = signatures.addDocument(doc);

            XMLSignature signature1 = signatures.createSignature();

            // add document as inner reference
            signature1.addDocument("#" + fragment, "text/xml", false);

            signature1.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

            // add certificate to show who signed the document
            // arrange the parameters whether the certificate is qualified or not
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);
            signature1.addKeyInfo(cert);

            // now sign it by using smart card
            // specifiy the PIN before sign
            signature1.sign(SmartCardManager.getInstance().getSigner(PIN, cert));

            XMLSignature signature2 = signatures.createSignature();

            // add document as inner reference
            signature2.addDocument("#" + fragment, "text/plain", false);

            signature2.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

            // add certificate to show who signed the document
            signature2.addKeyInfo(cert);

            // now sign it by using smart card
            // specifiy the PIN before sign
            signature2.sign(SmartCardManager.getInstance().getSigner(PIN, cert));

            // write combined document
            signatures.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
        }

        [Test]
        public void validate()
        {
            Validation.validateParallel(SIGNATURE_FILENAME);
        }
    }
}
