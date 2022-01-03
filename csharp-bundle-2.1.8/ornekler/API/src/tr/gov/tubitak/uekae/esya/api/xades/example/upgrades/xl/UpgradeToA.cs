﻿using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation;

namespace xmlsig.samples.upgrades.xl
{
    /**
     * Provides upgrade function from XL to A
     * @author: suleyman.uslu
     */
    [TestFixture]
    public class UpgradeToA : SampleBase
    {
        public static readonly String SIGNATURE_FILENAME = "a_from_xl.xml";

        /**
         * Upgrades XL to A. XL needs to be provided before upgrade process.
         * It can be created in formats.XL.
         */
        [Test]
        public void upgradeXLToA()
        {
            // create context with working dir
            Context context = createContext();

            // read signature to be upgraded
            XMLSignature signature = XMLSignature.parse(new FileDocument(new FileInfo(BASE_DIR + "xl.xml")), context);

            // upgrade to A
            signature.upgrade(tr.gov.tubitak.uekae.esya.api.signature.SignatureType.ES_A);

            signature.write(new FileStream(BASE_DIR + SIGNATURE_FILENAME, FileMode.OpenOrCreate));
        }

        [Test]
        public void validate()
        {
            Validation.validate(SIGNATURE_FILENAME);
        }
    }
}
