using System;
using System.Collections.Generic;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.cms;
using tr.gov.tubitak.uekae.esya.api.asn.pkixtsp;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.asn.util;

/**
 * Get times from signature.
 * @author orcun.ertugrul
 *
 */

namespace tr.gov.tubitak.uekae.esya.api.cades.example.validation
{
    [TestFixture]
    public class GetTime
    {
        /**
	 * Gets signature time stamp. It indicates when the sign was created.
	 * @throws Exception
	 */

        [Test]
        public void testSignatureTS()
        {
            byte[] input = AsnIO.dosyadanOKU(TestConstants.getDirectory() + @"\testVerileri\EST-1.p7s");
            BaseSignedData bs = new BaseSignedData(input);
            EST estSign = (EST)bs.getSignerList()[0];
            DateTime? time = estSign.getTime();
            Console.WriteLine(time.ToString());
        }

        /**
         * Gets signing time attribute time. It indicates the declared time when the signature is created. 
         * @throws Exception
         */

        [Test]
        public void testSigningTme()
        {
            byte[] input = AsnIO.dosyadanOKU(TestConstants.getDirectory() + @"\testVerileri\BES-2.p7s");
            
            BaseSignedData bs = new BaseSignedData(input);
            List<EAttribute> attrs = bs.getSignerList()[0].getSignedAttribute(AttributeOIDs.id_signingTime);
            ETime time = new ETime(attrs[0].getValue(0));
            Console.WriteLine((object) time.getTime().Value.ToLocalTime());
        }

        /**
         * Gets archive time stamp. It indicated then signature is converted to ESA.
         * @throws Exception
         */

        [Test]
        public void testarchiveTimestamp()
        {
            byte[] input = AsnIO.dosyadanOKU(TestConstants.getDirectory() + @"\testVerileri\ESA-1.p7s");
            BaseSignedData bs = new BaseSignedData(input);
            List<EAttribute> attrs = bs.getSignerList()[0].getUnsignedAttribute(AttributeOIDs.id_aa_ets_archiveTimestamp);
            List<EAttribute> attrsV2 =
                bs.getSignerList()[0].getUnsignedAttribute(AttributeOIDs.id_aa_ets_archiveTimestampV2);
            attrs.AddRange(attrsV2);
            foreach (EAttribute attribute in attrs)
            {
                EContentInfo contentInfo = new EContentInfo(attribute.getValue(0));
                ESignedData sd = new ESignedData(contentInfo.getContent());
                ETSTInfo tstInfo = new ETSTInfo(sd.getEncapsulatedContentInfo().getContent());
                Console.WriteLine((string) tstInfo.getTime().ToString());
            }
        }


    }
}