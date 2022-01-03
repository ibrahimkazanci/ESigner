using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.upgrades.x2
{
    /**
     * Provides upgrade function from X2 to XL
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class UpgradeToXL : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "xl_from_x2.xml";

        /**
         * Upgrades X2 to XL. X2 needs to be provided before upgrade process.
         * It can be created in formats.X2.
         */
        [Test]
        public void upgradeX2ToXL()
        {
            // create context with working dir
            Context context = createContext();

            // read signature to be upgraded
            XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(BASE_DIR + "x2.xml")), context);

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
