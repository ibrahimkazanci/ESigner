using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using log4net.Config;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.upgrades.parallel
{
    /**
     * Provides functions for upgrade of parallel BES to type T
     * @author suleyman.uslu
     */
    [TestFixture]
    public class UpgradeToT : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "t_from_bes_parallel.xml";

        /**
         * Upgrade parallel BES to type T
         */
        [Test]
        public void upgradeBESToT()
        {
            // create context with working dir
            Context context = createContext();

            // parse the previously created enveloped signature
            XmlDocument document = parseDoc("parallel_detached.xml");

            // get and upgrade the signature 1 in DOM document
            XMLSignature signature1 = readSignature(document, context,0 );
            signature1.upgrade(tr.gov.tubitak.uekae.esya.api.signature.SignatureType.ES_T);

            // get and upgrade the signature 2 in DOM document
            XMLSignature signature2 = readSignature(document, context, 1);
            signature2.upgrade(tr.gov.tubitak.uekae.esya.api.signature.SignatureType.ES_T);

            // writing enveloped XML to the file
            document.Save(BASE_DIR + SIGNATURE_FILENAME);
        }

        [Test]
        public void validate()
        {
            Validation.validateParallel(SIGNATURE_FILENAME);
        }
    }
}
