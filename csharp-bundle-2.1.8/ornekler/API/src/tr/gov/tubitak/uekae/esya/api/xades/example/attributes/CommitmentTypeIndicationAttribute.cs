using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.attributes
{
    /**
     * BES with CommitmentTypeIndication attribute sample
     * @author: suleyman.uslu
     */
    [TestFixture]
    class CommitmentTypeIndicationAttribute : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "commitment_type_indication.xml";

        /**
         * Creates detached BES with CommitmentTypeIndication attribute
         */
        [Test]
        public void createBESWithCommitmentTypeIndication()
        {
            try
            {
                // create context with working directory
                Context context = createContext();

                // create signature according to context,
                // with default type (XADES_BES)
                XMLSignature signature = new XMLSignature(context);

                // add document
                String ref1 = "#" + signature.addDocument("./sample.txt", "text/plain", true);
                String objId2 = signature.addPlainObject("Test data 1.", "text/plain", null);
                String ref2 = "#" + signature.addDocument("#" + objId2, null, false);

                signature.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

                // add certificate to show who signed the document
                // arrange the parameters whether the certificate is qualified or not
                ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);
                signature.addKeyInfo(cert);

                // add commitment type indication
                signature.QualifyingProperties.SignedProperties.SignedDataObjectProperties.
                        addCommitmentTypeIndication(createTestCTI(context, ref1, ref2));

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

        private CommitmentTypeIndication createTestCTI(Context c, String ref1, String ref2)
        {
            CommitmentTypeId typeId = new CommitmentTypeId(
                    c,
                    new Identifier(c, "http://uri.etsi.org/01903/v1.2.2#ProofOfOrigin", null),
                    "Proof of origin indicates that the signer recognizes to have created, approved and sent the signed data object.",
                    //Arrays.asList(
                            new List<String>(new String[]{
                                "http://test.test/commitment1.txt</xades:DocumentationReference",
                                "file:///test/data/xml/commitment2.txt</xades:DocumentationReference",
                                "http://test.test/commitment3.txt</xades:DocumentationReference"})
            );

            CommitmentTypeQualifier q1 = new CommitmentTypeQualifier(c);
            q1.addContent("test commitment a");
            q1.addContent(getQualifierSampleContent());
            q1.addContent("test commitment b");
            CommitmentTypeQualifier q2 = new CommitmentTypeQualifier(c, "commitment 2");
            CommitmentTypeQualifier q3 = new CommitmentTypeQualifier(c, "commitment 2");

            return new CommitmentTypeIndication(c, typeId,
                    new List<String>(new String[]{ref1, ref2}), false,
                    new List<CommitmentTypeQualifier>(new CommitmentTypeQualifier[]{q1, q2, q3}));
        }

        private XmlElement getQualifierSampleContent()
        {
            return stringToElement(
                    "<xl:XadesLabs xmlns:xl=\"http://xadeslabs.com/xades\"> \n" +
                            "          <xl:Commitments type=\"ProofOfOrigin\">\n" +
                            "            <xl:Commitment>commitment 1</xl:Commitment>\n" +
                            "            <xl:Commitment>commitment 2</xl:Commitment>\n" +
                            "            <xl:Commitment>commitment 3</xl:Commitment>\n" +
                            "            <xl:Commitment>commitment 4</xl:Commitment>\n" +
                            "          </xl:Commitments>\n" +
                            "</xl:XadesLabs>");
        }

        private XmlElement stringToElement(String aStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(aStr);
            MemoryStream ms = new MemoryStream(bytes);

            XmlDocument doc = new XmlDocument();
            XmlReader reader = XmlReader.Create(ms);
            doc.Load(reader);

            return doc.DocumentElement;
        }
    }
}
