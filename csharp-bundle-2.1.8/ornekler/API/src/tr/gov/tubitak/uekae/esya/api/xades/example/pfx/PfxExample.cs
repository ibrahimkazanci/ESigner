using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.pfx
{
    /**
     * Creating electronic signature using pfx
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class PfxExample : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "pfxexample.xml";

        /**
         * Creates detached BES using pfx
         */
        [Test]
        public void createDetachedBesWithPfx()
        {
            try {
                // create context with working dir
                Context context = createContext();

                // create signature according to context,
                // with default type (XADES_BES)
                XMLSignature signature = new XMLSignature(context);

                // add document as reference, but do not embed it
                // into the signature (embed=false)
                signature.addDocument("./sample.txt", "text/plain", false);

                // add signer's certificate
                signature.addKeyInfo(CERTIFICATE);

                // sign the document
                signature.sign(SIGNER);

                signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
            }
            catch (XMLSignatureException x){
                // cant create signature
                Console.WriteLine(x.StackTrace);
            }
            catch (IOException x){
                // probably couldn't write to the file
                Console.WriteLine(x.StackTrace);
            }
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
