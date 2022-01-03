using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace xmlsig.samples.upgrades.bes
{
    /**
     * Provides upgrade function from BES to C
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class UpgradeToC : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "c_from_bes.xml";

        /**
         * Upgrades BES to C. BES needs to be provided before upgrade process.
         * It can be created in formats.BES.
         */
        [Test]
        public void upgradeBESToC()
        {
            // create context with working dir
            Context context = createContext();

            // read signature to be upgraded
            XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(BASE_DIR + "bes.xml")), context);

            // upgrade to C
            signature.upgrade(tr.gov.tubitak.uekae.esya.api.signature.SignatureType.ES_C);

            signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
