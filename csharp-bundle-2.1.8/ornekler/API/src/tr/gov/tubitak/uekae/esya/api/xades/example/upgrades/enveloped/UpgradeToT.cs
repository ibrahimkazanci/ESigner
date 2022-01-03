using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using log4net.Config;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.upgrades.enveloped
{
    /**
     * Provides functions for upgrade of enveloped BES to type T
     * @author suleyman.uslu
     */
    [TestFixture]
    public class UpgradeToT : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "t_from_bes_enveloped.xml";

        /**
         * Upgrade enveloped BES to type T
         */
        [Test]
        public void upgradeBESToT()
        {
            // create context with working dir
            Context context = createContext();

            // parse the previously created enveloped signature
            XmlDocument document = parseDoc("enveloped.xml");

            // get the signature in DOM document
            XMLSignature signature = readSignature(document, context);

            ValidationResult vr = signature.verify();

            // upgrade the signature to type T
            signature.upgrade(tr.gov.tubitak.uekae.esya.api.signature.SignatureType.ES_T);

            // writing enveloped XML to the file
            document.Save(BASE_DIR + SIGNATURE_FILENAME);
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
