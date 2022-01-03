using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
//using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
//using tr.gov.tubitak.uekae.esya.api.signature.certval;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.validation
{
    /**
     * Provides validating functions for xml signatures.
     * @author suleyman.uslu
     */
    public class Validation : SampleBase
    {
        /**
         * Generic validate function. Validates known types of xml signature.
         * @param fileName name of the signature file to be validated
         */
        public static void validate(String fileName)
        {
            Context context = createContext();

            /* optional - specifying policy from code
            // generate policy to be used in certificate validation
            ValidationPolicy policy = PolicyReader.readValidationPolicy(POLICY_FILE);

            CertValidationPolicies policies = new CertValidationPolicies();
            // null means default
            policies.register(null,policy);

            context.Config.ValidationConfig.setCertValidationPolicies(policies);
            */
            
            // add external resolver to resolve policies
            context.addExternalResolver(POLICY_RESOLVER);

            XMLSignature signature = XMLSignature.parse(
                                        new FileDocument(new FileInfo(BASE_DIR + fileName)),
                                        context) ;

            // no params, use the certificate in key info
            ValidationResult result = signature.verify();
            Console.WriteLine(result.toXml());
            Assert.True(result.getType() == ValidationResultType.VALID,"Cant verify " + fileName);

            UnsignedSignatureProperties usp = signature.QualifyingProperties.UnsignedSignatureProperties;
            if (usp!=null){
                IList<XMLSignature> counterSignatures = usp.AllCounterSignatures;
                foreach (XMLSignature counterSignature in counterSignatures){
                    ValidationResult counterResult = signature.verify();

                    Console.WriteLine(counterResult.toXml());

                    Assert.True(counterResult.getType() == ValidationResultType.VALID,
                        "Cant verify counter signature" + fileName + " : "+counterSignature.Id);

                }
            }

        }

        /**
         * Validate function for parallel signatures
         * @param fileName name of the signature file to be validated
         */
        public static void validateParallel(String fileName)
        {
            Context context = createContext();

            /* optional - specifying policy from code
            // generate policy to be used in certificate validation
            ValidationPolicy policy = PolicyReader.readValidationPolicy(POLICY_FILE);

            CertValidationPolicies policies = new CertValidationPolicies();
            // null means default
            policies.register(null, policy);

            context.Config.ValidationConfig.setCertValidationPolicies(policies);
            */

            // add external resolver to resolve policies
            context.addExternalResolver(POLICY_RESOLVER);

            List<XMLSignature> xmlSignatures = new List<XMLSignature>();

            // get signature as signed document in order to be able to validate parallel ones
            SignedDocument sd = new SignedDocument(new FileDocument(new FileInfo(BASE_DIR + fileName)), context);

            xmlSignatures.AddRange(sd.getRootSignatures());

            foreach (var xmlSignature in xmlSignatures)
            {
                // no params, use the certificate in key info
                ValidationResult result = xmlSignature.verify();
                Console.WriteLine(result.toXml());
                Assert.True(result.getType() == ValidationResultType.VALID, "Cant verify " + fileName);

                UnsignedSignatureProperties usp = xmlSignature.QualifyingProperties.UnsignedSignatureProperties;
                if (usp != null)
                {
                    IList<XMLSignature> counterSignatures = usp.AllCounterSignatures;
                    foreach (XMLSignature counterSignature in counterSignatures)
                    {
                        ValidationResult counterResult = xmlSignature.verify();

                        Console.WriteLine(counterResult.toXml());

                        Assert.True(counterResult.getType() == ValidationResultType.VALID,
                            "Cant verify counter signature" + fileName + " : " + counterSignature.Id);

                    }
                }
            }

        }
    }
}
