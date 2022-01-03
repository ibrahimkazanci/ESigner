using System;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;

/**
 * Matchs the signer and validation result
 * @author orcun.ertugrul
 */

namespace tr.gov.tubitak.uekae.esya.api.cades.example.validation
{
    public class SignerAndValidationResult
    {
        private readonly Signer signer;
        private readonly SignatureValidationResult validationResult;        

        public SignerAndValidationResult(Signer aSigner, SignatureValidationResult aValidationResult)
        {
            signer = aSigner;
            validationResult = aValidationResult;
        }

        //@Override
        public override String ToString()
        {
            return signer.getSignerCertificate().getSubject().getCommonNameAttribute();
        }

        public SignatureValidationResult getValidationResult()
        {
            return validationResult;
        }
    }
}