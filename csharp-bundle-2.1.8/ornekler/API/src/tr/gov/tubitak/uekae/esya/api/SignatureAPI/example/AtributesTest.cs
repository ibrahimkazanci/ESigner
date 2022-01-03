using System;
using System.Collections.Generic;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.attribute;

namespace tr.gov.tubitak.uekae.esya.api.SignatureAPI.example
{
    public class AtributesTest
    {
        [Test]
        public void testSignatureTimestamp()
        {
            SignatureContainer sc = Constants.readSignatureContainer("upgrade_BES_T");
            List<TimestampInfo> tsInfo = sc.getSignatures()[0].getTimestampInfo(TimestampType.SIGNATURE_TIMESTAMP);
            Assert.True(tsInfo.Count == 1, "Bir zaman damgası olmalı");
            Console.WriteLine(tsInfo[0].getTSTInfo().getTime());
        }

        [Test]
        public void testSigAndRefsTimestamp()
        {
            SignatureContainer sc = Constants.readSignatureContainer("upgrade_BES_X1");
            List<TimestampInfo> tsInfo = sc.getSignatures()[0].getTimestampInfo(TimestampType.SIG_AND_REFERENCES_TIMESTAMP);
            Assert.True(tsInfo.Count == 1, "Bir sigAndRefs zaman damgası olmalı");
            Console.WriteLine(tsInfo[0].getTSTInfo().getTime());
        }

        [Test]
        public void testReferences()
        {
            SignatureContainer sc = Constants.readSignatureContainer("upgrade_BES_C");
            CertValidationReferences refs = sc.getSignatures()[0].getCertValidationReferences();
            Assert.True(refs.getCrlReferences().Count > 0, "Bir sil referansı olmalı");
            Assert.True(refs.getCertificateReferences().Count > 0, "Bir sertifika referansı olmalı");
        }

        [Test]
        public void testValues()
        {
            SignatureContainer sc = Constants.readSignatureContainer("upgrade_BES_xL");
            CertValidationValues values = sc.getSignatures()[0].getCertValidationValues();
            Assert.True(values.getCrls().Count > 0, "Bir sil olmalı");
            Assert.True(values.getCertificates().Count > 0, "Bir sertifika olmalı");
        }

        [Test]
        public void testSignatureAlg()
        {
            SignatureContainer sc = Constants.readSignatureContainer("upgrade_BES_xL");
            SignatureAlg alg = (SignatureAlg)sc.getSignatures()[0].getSignatureAlg();
            Assert.True(alg.Equals(SignatureAlg.RSA_SHA256), "Imza alg RSA-SHA256 olmalı");
        }
    }
}
