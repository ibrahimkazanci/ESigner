using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.config;

namespace tr.gov.tubitak.uekae.esya.api.SignatureAPI.example
{
public class Bes {

	 [Test]
    public void createDetached()
	 {
        SignatureContainer container = SignatureFactory.createContainer(Constants.signatureFormat, Constants.createContext());
		
        bool checkQCStatement = Constants.getCheckQCStatement();		
		//Get qualified or non-qualified certificate.
		ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
		BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);
		
        Signature signature = container.createSignature(cert);

        signature.addContent(Constants.getContent(), false);

        signature.sign(signer);
        SmartCardManager.getInstance().logout();
        Constants.dosyaYaz(container,"bes_detached");
    }
     [Test]
    public void createEnveloping() 
    {
        SignatureContainer container = SignatureFactory.createContainer(Constants.signatureFormat, Constants.createContext());
        //container.getContext().setConfig(new Config(Constants.ROOT + @"\config\esya-signature-config.xml"));
        bool checkQCStatement = Constants.getCheckQCStatement();		
		//Get qualified or non-qualified certificate.
		ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
		BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);        
        
        Signature signature = container.createSignature(cert);

        signature.addContent(Constants.getContent(), true);

        signature.sign(signer);
        SmartCardManager.getInstance().logout();
        Constants.dosyaYaz(container, "bes_enveloping");
    }

     [Test]
    public void validateDetached() 
    {
        Context c = Constants.createContext();
        ContainerValidationResult cvr = Validation.validateSignature("bes_detached", c);
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
        Assert.AreEqual(1, cvr.getSignatureValidationResults().Count);
    }

     [Test]
    public void validateEnveloping() 
    {
        ContainerValidationResult cvr = Validation.validateSignature("bes_enveloping");
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
        Assert.AreEqual(1, cvr.getSignatureValidationResults().Count);
    }
	
}
}
