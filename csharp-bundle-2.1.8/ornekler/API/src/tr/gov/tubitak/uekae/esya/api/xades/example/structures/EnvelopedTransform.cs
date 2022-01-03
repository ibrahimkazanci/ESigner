﻿using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.structures
{
    /**
     * Enveloped transform BES sample
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class EnvelopedTransform : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "enveloped_transform.xml";

        /**
         * Create enveloped transform BES
         */
        [Test]
        public void createEnvelopedTransform()
        {
            // here is our custom envelope xml
            XmlDocument envelopeDoc = newEnvelope();

            // create context with working dir
            Context context = createContext();

            // define where signature belongs to
            context.Document = envelopeDoc;

            // create signature according to context,
            // with default type (XADES_BES)
            XMLSignature signature = new XMLSignature(context, false);

            // attach signature to envelope
            envelopeDoc.DocumentElement.AppendChild(signature.Element);

            Transforms transforms = new Transforms(context);
            transforms.addTransform(new Transform(context, TransformType.ENVELOPED.Url));

            // add whole document(="") with envelope transform, with SHA256
            // and don't include it into signature(false)
            signature.addDocument("", "text/xml", transforms, DigestMethod.SHA_256, false);

            signature.SignedInfo.SignatureMethod = SignatureMethod.RSA_SHA256;

            // add certificate to show who signed the document
            // arrange the parameters whether the certificate is qualified or not
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);
            signature.addKeyInfo(cert);

            // now sign it by using smart card
            // specifiy the PIN before sign
            signature.sign(SmartCardManager.getInstance().getSigner(PIN, cert));

            signature.upgrade(api.signature.SignatureType.ES_A);

            // this time we dont use signature.write because we need to write
            // whole document instead of signature
            Stream s = new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate);
            /*if (!envelopeDoc.InnerXml.Contains(XmlUtil.XML_PREAMBLE_STR))
            {
                byte[] utf8Definition = XmlUtil.XML_PREAMBLE;
                s.Write(utf8Definition, 0, utf8Definition.Length);
            }*/
            envelopeDoc.Save(s);
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
