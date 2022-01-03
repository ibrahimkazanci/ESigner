using System;
using System.Collections.Generic;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.common.crypto;

namespace tr.gov.tubitak.uekae.esya.api.cades.example.util
{
    public class SignerManager
    {
        /**
	 * Adds a parallel ESXLong signature with external content.
	 * @param aContent
	 * @param aSign
	 * @return
	 * @throws CMSSignatureException
	 * @throws SmartCardException
	 * @throws ESYAException
	 */
        public static byte[] addSigner(ISignable aContent, byte[] aSign)
        {
            BaseSignedData bs = null;
            Dictionary<string, object> params_ = new Dictionary<string, object>();

            if (aSign != null)
            {
                bs = new BaseSignedData(aSign);
                params_[EParameters.P_EXTERNAL_CONTENT] = aContent;
            }
            else
            {
                bs = new BaseSignedData();
                bs.addContent(aContent, false);
            }

            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
            params_[EParameters.P_TSS_INFO] = TestConstants.getTSSettings();

            bool qualifiedCert = TestConstants.getCheckQCStatement();
            

            //To-Do Get Card PIN from user.
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(qualifiedCert, !qualifiedCert);
            BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);

            bs.addSigner(ESignatureType.TYPE_ESXLong, cert, signer, null, params_);

            SmartCardManager.getInstance().logout();

            return bs.getEncoded();
        }


        //add signer as external content
        //returns new sign
        public static byte[] addSignerWithSigningTimeAttr(ISignable aContent, byte[] aSign, DateTime? aSigningTime)
        {
            BaseSignedData bs = null;
            Dictionary<String, Object> params_ = new Dictionary<String, Object>();

            if (aSign != null)
            {
                bs = new BaseSignedData(aSign);
                params_[EParameters.P_EXTERNAL_CONTENT] = aContent;
            }
            else
            {
                bs = new BaseSignedData();
                bs.addContent(aContent, false);
            }

            List<IAttribute> optionalAttributes = new List<IAttribute>();
            optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));

            params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
            if (aSigningTime != null)
                params_[EParameters.P_SIGNING_TIME] = aSigningTime;

            bool qualifiedCert = TestConstants.getCheckQCStatement();
           
            //To-Do Get Card PIN from user.
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(qualifiedCert, !qualifiedCert);
            BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);

            bs.addSigner(ESignatureType.TYPE_BES, cert, signer, optionalAttributes, params_);

            return bs.getEncoded();
        }
        
    }
}
