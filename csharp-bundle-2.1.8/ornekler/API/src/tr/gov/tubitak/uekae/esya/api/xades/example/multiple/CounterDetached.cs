using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.structures;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.resolver;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.multiple
{
    /**
     * Counter signature sample
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class CounterDetached : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "counter_detached.xml";

        /**
         * Adds counter signature to a detached one
         * @throws Exception
         */
        [Test]
        public void signCounterDetached()
        {
            Context context = createContext();

            // read previously created signature, you need to run Detached first
            Document doc = Resolver.resolve(Detached.SIGNATURE_FILENAME, context);
            XMLSignature signature = XMLSignature.parse(doc, context);

            // create counter signature
            XMLSignature counterSignature = signature.createCounterSignature();

            signature.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

            // add certificate to show who signed the document
            // arrange the parameters whether the certificate is qualified or not
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);
            counterSignature.addKeyInfo(cert);

            // now sign it by using smart card
            // specifiy the PIN before sign
            counterSignature.sign(SmartCardManager.getInstance().getSigner(PIN, cert));

            // signature contains itself and counter signature
            signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
