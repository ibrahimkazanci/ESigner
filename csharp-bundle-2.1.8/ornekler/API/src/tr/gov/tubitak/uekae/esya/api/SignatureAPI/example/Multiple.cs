using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.signature;

namespace tr.gov.tubitak.uekae.esya.api.SignatureAPI.example
{
public class Multiple {

    [Test]
	    public void createParallelToExisting()  {
	        Context c = Constants.createContext();
	        SignatureContainer sc = Constants.readSignatureContainer("bes_enveloping", c);
	        
	        bool checkQCStatement = Constants.getCheckQCStatement();		
			//Get qualified or non-qualified certificate.
			ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
			BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);        
	        
	        Signature s = sc.createSignature(cert);
	        s.addContent(Constants.getContent(), true);
	        s.sign(signer);
	        SmartCardManager.getInstance().logout();	        
	        Constants.dosyaYaz(sc, "parallel_bes");
	    }

        [Test]
        public void createParallelToExistingDetached()
        {
            Context c = Constants.createContext();
            c.setData(null);
            SignatureContainer sc = Constants.readSignatureContainer("bes_detached", c);

            bool checkQCStatement = Constants.getCheckQCStatement();
            //Get qualified or non-qualified certificate.
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
            BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);

            Signature s = sc.createSignature(cert);
            s.addContent(Constants.getContent(), false);
            s.sign(signer);
            SmartCardManager.getInstance().logout();
            Constants.dosyaYaz(sc, "parallel_bes_detached");
        }

	     [Test]
	    public void createSerialToExisting()  {
	        Context c = Constants.createContext();
	        SignatureContainer sc = Constants.readSignatureContainer("bes_enveloping", c);
	        
	        bool checkQCStatement = Constants.getCheckQCStatement();		
			//Get qualified or non-qualified certificate.
			ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
			BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert); 
			
	        Signature cs = sc.getSignatures()[0].createCounterSignature(cert);
	        cs.sign(signer);
	        SmartCardManager.getInstance().logout();	        
	        Constants.dosyaYaz(sc, "serial_bes");
	    }

	     [Test]
	    public void createSerialToSerial()  {
	        Context c = Constants.createContext();
	        SignatureContainer sc = Constants.readSignatureContainer("serial_bes", c);
	        Signature s = sc.getSignatures()[0];
	        Signature counter = s.getCounterSignatures()[0];
	        
	        bool checkQCStatement = Constants.getCheckQCStatement();		
			//Get qualified or non-qualified certificate.
			ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
			BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert); 
			
	        Signature counterOfCounter = counter.createCounterSignature(cert);
	        counterOfCounter.sign(signer);
	        SmartCardManager.getInstance().logout();	        
	        Constants.dosyaYaz(sc, "serial_to_serial_bes");

	    }

	     [Test]
	    public void validateSerial()  {
	        ContainerValidationResult cvr = Validation.validateSignature("serial_bes");
            Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
	    }

	     [Test]
	    public void validateParallel()  {
	        ContainerValidationResult cvr = Validation.validateSignature("parallel_bes");
            Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
	    }

         [Test]
         public void validateParallelDetached()
         {
             ContainerValidationResult cvr = Validation.validateSignature("parallel_bes_detached");
             Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
         }

	     [Test]
	    public void validateSerialToSerial()  {
	        ContainerValidationResult cvr = Validation.validateSignature("serial_to_serial_bes");
            Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
	    }

}

}
