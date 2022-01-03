using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.multiple
{
    /**
     * Counter signature to a parallel signature sample
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class CounterParallel : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "counter_parallel.xml";

        /**
         * Adds counter signature to a parallel detached one
         * @throws Exception
         */
        [Test]
        public void signCounterParallel()
        {
            Context context = createContext();

            // read previously created signature
            SignedDocument signedDocument = new SignedDocument(new FileDocument(new FileInfo(BASE_DIR + ParallelDetached.SIGNATURE_FILENAME)),context);

            // get First signature
            XMLSignature signature = signedDocument.getSignature(0);

            // create counter signature
            XMLSignature counterSignature = signature.createCounterSignature();

            counterSignature.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

            // add certificate to show who signed the document
            // arrange the parameters whether the certificate is qualified or not
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);
            counterSignature.addKeyInfo(cert);

            // now sign it by using smart card
            // specifiy the PIN before sign
            counterSignature.sign(SmartCardManager.getInstance().getSigner(PIN, cert));

            // signed doc contains both previous signature and now a counter signature
            // in first signature
            signedDocument.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
        }

        [Test]
        public void validate()
        {
            Validation.validateParallel(SIGNATURE_FILENAME);
        }
    }
}
