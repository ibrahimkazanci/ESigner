using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.upgrades.c
{
    /**
     * Provides upgrade function from C to XL
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class UpgradeToXL : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "xl_from_c.xml";

        /**
         * Upgrades C to XL. C needs to be provided before upgrade process.
         * It can be created in formats.C.
         */
        [Test]
        public void upgradeCToXL()
        {
            // create context with working dir
            Context context = createContext();

            // read signature to be upgraded
            XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(BASE_DIR + "c.xml")), context);

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
