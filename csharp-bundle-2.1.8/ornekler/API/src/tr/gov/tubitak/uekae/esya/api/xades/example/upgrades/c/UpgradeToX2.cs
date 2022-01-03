using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace xmlsig.samples.upgrades.c
{
    /**
     * Provides upgrade function from C to X2
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class UpgradeToX2 : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "x2_from_c.xml";

        /**
         * Upgrades C to X2. C needs to be provided before upgrade process.
         * It can be created in formats.C.
         */
        [Test]
        public void upgradeCToX2()
        {
            // create context with working dir
            Context context = createContext();

            // read signature to be upgraded
            XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(BASE_DIR + "c.xml")), context);

            // upgrade to X2
            signature.upgrade(tr.gov.tubitak.uekae.esya.api.signature.SignatureType.ES_X_Type2);

            signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
