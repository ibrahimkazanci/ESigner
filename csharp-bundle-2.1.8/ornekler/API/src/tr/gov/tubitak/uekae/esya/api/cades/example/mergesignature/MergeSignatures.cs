using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Com.Objsys.Asn1.Runtime;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.cms;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.crypto.exceptions;

namespace tr.gov.tubitak.uekae.esya.api.cades.example.mergesignature
{
    /**
    * Merges two parallel signature.
    * @author orcun.ertugrul
    *
    */
    [TestFixture]
    public class MergeSignatures
    {
        [Test]
        public void testCombineTwoSignatures()
        {
            //First Signature
            BaseSignedData bs1 = new BaseSignedData();
            ISignable content = new SignableByteArray(ASCIIEncoding.ASCII.GetBytes("test"));
            bs1.addContent(content);

            Dictionary<string, object> params_ = new Dictionary<string, object>();

            params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;
            bool checkQCStatement = TestConstants.getCheckQCStatement();

            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
            BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);

            bs1.addSigner(ESignatureType.TYPE_BES, cert, signer, null, params_);

            //Second Signature
            BaseSignedData bs2 = new BaseSignedData();
            bs2.addContent(content);
            bs2.addSigner(ESignatureType.TYPE_BES, cert, signer, null, params_);
            SmartCardManager.getInstance().logout();

            //Merge Signatures
            BaseSignedData[] toBeMerged = new BaseSignedData[] { bs1, bs2 };
            BaseSignedData mergedSignature = mergeSignatures(toBeMerged, content);
            Console.WriteLine((int)mergedSignature.getAllSigners().Count);

        }


        private BaseSignedData mergeSignatures(BaseSignedData[] toBeMerged, ISignable content)
        {
            BaseSignedData combined = new BaseSignedData(toBeMerged[0].getEncoded());
            for (int i = 1; i < toBeMerged.Length; i++)
            {
                BaseSignedData bs = toBeMerged[i];
                List<Signer> signers = bs.getSignerList();


                foreach (Signer aSigner in signers)
                {
                    ESignerInfo signerInfo = aSigner.getSignerInfo();
                    //Check the correct document is signed.
                    if (checkMessageDigestAttr(signerInfo, content))
                    {
                        combined.getSignedData().addSignerInfo(signerInfo);
                        CMSSignatureUtil.addCerIfNotExist(combined.getSignedData(), aSigner.getSignerCertificate());
                        CMSSignatureUtil.addDigestAlgIfNotExist(combined.getSignedData(), aSigner.getSignerInfo().getDigestAlgorithm());
                    }
                    else
                        throw new CMSSignatureException("İmzalanan içerik aynı değil");
                }
            }
            //new signature file
            return combined;
        }

        private bool checkMessageDigestAttr(ESignerInfo aSignerInfo, ISignable content)
        {
            EAttribute attr = aSignerInfo.getSignedAttribute(MessageDigestAttr.OID)[0];
            Asn1OctetString octetS = new Asn1OctetString();
            try
            {
                Asn1DerDecodeBuffer decBuf = new Asn1DerDecodeBuffer(attr.getValue(0));
                octetS.Decode(decBuf);
            }
            catch (Exception tEx)
            {
                throw new CMSSignatureException("Mesaj özeti çözülemedi.", tEx);
            }

            DigestAlg digestAlg = DigestAlg.fromOID(aSignerInfo.getDigestAlgorithm().getAlgorithm().mValue);
            try
            {
                byte[] contentDigest = content.getMessageDigest(digestAlg);
                return Enumerable.SequenceEqual<byte>(octetS.mValue, contentDigest);
            }
            catch (CryptoException e)
            {
                throw new CMSSignatureException("Mesaj özeti hesaplanamadı.", e);
            }
            catch (IOException e)
            {
                throw new CMSSignatureException("İmzalanan dosya okunamadı.", e);
            }
        }
    }
}
