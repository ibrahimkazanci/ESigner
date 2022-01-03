using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.upgrades.x1
{
    /**
     * Provides upgrade function from X1 to XL
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class UpgradeToXL : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "xl_from_x1.xml";

        /**
         * Upgrades X1 to XL. X1 needs to be provided before upgrade process.
         * It can be created in formats.X1.
         */
        [Test]
        public void upgradeX1ToXL()
        {
            // create context with working dir
            Context context = createContext();

            // read signature to be upgraded
            XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(BASE_DIR + "x1.xml")), context);

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
