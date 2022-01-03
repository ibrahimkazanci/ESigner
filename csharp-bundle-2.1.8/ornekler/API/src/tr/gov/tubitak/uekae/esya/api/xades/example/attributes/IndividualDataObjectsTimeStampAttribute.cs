using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades.timestamp;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.attributes
{
    /**
     * BES with IndividualDataObjectsTimeStamp attribute sample
     * @author: suleyman.uslu
     */
    [TestFixture]
    class IndividualDataObjectsTimeStampAttribute : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "individual_data_objects_timestamp.xml";

        /**
         * Creates detached BES with IndividualDataObjectsTimeStamp attribute
         */
        [Test]
        public void createBESWithIndividualDataObjectsTimeStamp()
        {
            try
            {
                // create context with working directory
                Context context = createContext();

                // create signature according to context,
                // with default type (XADES_BES)
                XMLSignature signature = new XMLSignature(context);

                // add document into the signature and get the reference
                String ref1 = "#" + signature.addDocument("./sample.txt", "text/plain", true);

                // add another object
                String objId2 = signature.addPlainObject("Test Data 1", "text/plain", null);
                String ref2 = "#" + signature.addDocument("#" + objId2, null, false);

                signature.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

                // add certificate to show who signed the document
                // arrange the parameters whether the certificate is qualified or not
                ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);
                signature.addKeyInfo(cert);

                // create new individual data objects timestamp structure
                IndividualDataObjectsTimeStamp timestamp = new IndividualDataObjectsTimeStamp(context);

                // add objects to timestamp structure
                timestamp.addInclude(new Include(context, ref1, true));
                timestamp.addInclude(new Include(context, ref2, true));

                // get encapsulated timestamp to individual data objects timestamp
                timestamp.addEncapsulatedTimeStamp(signature);

                // add individual data objects timestamp to signature
                signature.QualifyingProperties.SignedProperties.createOrGetSignedDataObjectProperties().
                        addIndividualDataObjectsTimeStamp(timestamp);

                // optional - add timestamp validation data
                signature.addTimeStampValidationData(timestamp, DateTime.UtcNow);

                // now sign it by using smart card
                // specifiy the PIN before sign
                signature.sign(SmartCardManager.getInstance().getSigner(PIN, cert));

                signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
            }
            catch (XMLSignatureException e)
            {
                // cannot create signature
                Assert.Fail("Error while signing" + e.StackTrace);
            }
            catch (Exception e)
            {
                // probably could not write to file
                Assert.Fail("Error while signing" + e.StackTrace);
            }
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
