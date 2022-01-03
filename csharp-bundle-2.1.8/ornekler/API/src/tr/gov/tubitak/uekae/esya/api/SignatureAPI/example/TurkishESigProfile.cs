using System;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.profile;
namespace tr.gov.tubitak.uekae.esya.api.SignatureAPI.example
{
public class TurkishESigProfile {

     [Test]
    public void createP1() 
    {
        SignatureContainer container = SignatureFactory.createContainer(Constants.signatureFormat, Constants.createContext());
		
        bool checkQCStatement = Constants.getCheckQCStatement();		
		//Get qualified or non-qualified certificate.
		ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
		BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);
		
        Signature signature = container.createSignature(cert);
        signature.addContent(Constants.getContent(), false);
        signature.setSigningTime(DateTime.UtcNow);

        signature.sign(signer);
        SmartCardManager.getInstance().logout();        
        Constants.dosyaYaz(container, "turkish_profile_p1_bes");
    }

     [Test]
    public void createP2() 
    {
        SignatureContainer container = SignatureFactory.createContainer(Constants.signatureFormat, Constants.createContext());
		
        bool checkQCStatement = Constants.getCheckQCStatement();		
		//Get qualified or non-qualified certificate.
		ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
		BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);
	       
        Signature signature = container.createSignature(cert);
        signature.addContent(Constants.getContent(), false);
        signature.setSigningTime(DateTime.UtcNow);
        signature.setSignaturePolicy(TurkishESigProfiles.SIG_POLICY_ID_P2v1);

        signature.sign(signer);
        signature.upgrade(SignatureType.ES_T);
        SmartCardManager.getInstance().logout();        
        Constants.dosyaYaz(container, "turkish_profile_p2_t");
    }

     [Test]
    public void createP3() 
    {
        Context context = Constants.createContext();
        context.getConfig().setCertificateValidationPolicies(Constants.getCRLOnlyPolicies());

        SignatureContainer container = SignatureFactory.createContainer(Constants.signatureFormat, context);
		
        bool checkQCStatement = Constants.getCheckQCStatement();		
		//Get qualified or non-qualified certificate.
		ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
		BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);
	
        Signature signature = container.createSignature(cert);
        signature.addContent(Constants.getContent(), false);
        signature.setSigningTime(DateTime.UtcNow);
        signature.setSignaturePolicy(TurkishESigProfiles.SIG_POLICY_ID_P3v1);

        signature.sign(signer);
        signature.upgrade(SignatureType.ES_XL);
        SmartCardManager.getInstance().logout();        
        Constants.dosyaYaz(container, "turkish_profile_p3_xl_crl");
    }

     [Test]
    public void createP4() 
    {
        Context context = Constants.createContext();
        context.getConfig().setCertificateValidationPolicies(Constants.getOCSPOnlyPolicies());

        SignatureContainer container = SignatureFactory.createContainer(Constants.signatureFormat, context);
		
        bool checkQCStatement = Constants.getCheckQCStatement();		
		//Get qualified or non-qualified certificate.
		ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
		BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);
	
        Signature signature = container.createSignature(cert);
        signature.addContent(Constants.getContent(), false);
        signature.setSigningTime(DateTime.UtcNow);
        signature.setSignaturePolicy(TurkishESigProfiles.SIG_POLICY_ID_P4v1);

        signature.sign(signer);
        signature.upgrade(SignatureType.ES_XL);
        SmartCardManager.getInstance().logout();       
        Constants.dosyaYaz(container, "turkish_profile_p4_xl_ocsp");
    }

     [Test]
    public void createP4WithA() 
    {
        SignatureContainer sc = Constants.readSignatureContainer("turkish_profile_p4_xl_ocsp");
        sc.getSignatures()[0].upgrade(SignatureType.ES_A);
        Constants.dosyaYaz(sc, "turkish_profile_p4_a");
    }

     [Test]
    public void createP4WithAWithA() 
    {
        SignatureContainer sc = Constants.readSignatureContainer("turkish_profile_p4_a", Constants.createContext());
        sc.getSignatures()[0].addArchiveTimestamp();
        Constants.dosyaYaz(sc, "turkish_profile_p4_aa");
    }

     [Test]
    public void validateP1() 
    {
        ContainerValidationResult cvr = Validation.validateSignature("turkish_profile_p1_bes");
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

     [Test]
    public void validateP2() 
    {
        ContainerValidationResult cvr = Validation.validateSignature("turkish_profile_p2_t");
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

     [Test]
    public void 
         validateP3() 

    {
        ContainerValidationResult cvr = Validation.validateSignature("turkish_profile_p3_xl_crl");
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

     [Test]
    public void validateP4() 
    {
        ContainerValidationResult cvr = Validation.validateSignature("turkish_profile_p4_xl_ocsp");
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

     [Test]
    public void validateP4A() 
    {
        ContainerValidationResult cvr = Validation.validateSignature("turkish_profile_p4_a");
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

     [Test]
    public void validateP4AA() 
    {
        ContainerValidationResult cvr = Validation.validateSignature("turkish_profile_p4_aa");
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

}
}
