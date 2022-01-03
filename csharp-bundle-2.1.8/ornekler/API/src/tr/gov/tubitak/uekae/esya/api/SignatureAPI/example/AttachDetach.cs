using System;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.signature;
namespace tr.gov.tubitak.uekae.esya.api.SignatureAPI.example
{
public class AttachDetach {

    [Test]
    public void attachNewSignatureToExisting()
    {
        Context c = Constants.createContext();
        SignatureContainer sc = SignatureFactory.createContainer(Constants.signatureFormat, c);
        
        bool checkQCStatement = Constants.getCheckQCStatement();		
		//Get qualified or non-qualified certificate.
		ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
		BaseSigner signer = SmartCardManager.getInstance().getSigner(Constants.getPIN(), cert);
		
        Signature s1 = sc.createSignature(cert);
        s1.addContent(Constants.getContent(), false);
        s1.sign(signer);

        Signature s2 = sc.createSignature(cert);
        s2.addContent(Constants.getContent(), false);
        s2.sign(signer);
        SmartCardManager.getInstance().logout();
        SignatureContainer existing = Constants.readSignatureContainer("bes_enveloping", c);
        existing.addExternalSignature(s1);
        existing.addExternalSignature(s2);

        Constants.dosyaYaz(existing, "bes_multiple_attached");
    }

     [Test]
    public void detachSignature()  {
        SignatureContainer existing = Constants.readSignatureContainer("bes_multiple_attached");
        // remove third signature!
        existing.getSignatures()[2].detachFromParent();
        Constants.dosyaYaz(existing, "bes_multiple_detached");
    }

     [Test]
    public void detachCounterLeaf() {
        SignatureContainer existing = Constants.readSignatureContainer("serial_to_serial_bes");

        // first counter
        Signature cs = existing.getSignatures()[0].getCounterSignatures()[0];
        // counter of counter
        Signature cc = cs.getCounterSignatures()[0];

        cc.detachFromParent();
        Constants.dosyaYaz(existing, "bes_serial_detached_leaf");
    }

     [Test]
    public void detachCounterMiddle()  {
        SignatureContainer existing = Constants.readSignatureContainer("serial_to_serial_bes");

        // first counter
        Signature cs = existing.getSignatures()[0].getCounterSignatures()[0];

        cs.detachFromParent();
        Constants.dosyaYaz(existing, "bes_serial_detached_middle");
    }

    // attach to A
    // detach from A
    // detach serial
    // detach subserial

     [Test]
    public void validateAttached() {
        SignatureContainer sc = Constants.readSignatureContainer("bes_multiple_attached", Constants.createContext());
        Assert.AreEqual(3, sc.getSignatures().Count);
        ContainerValidationResult cvr = sc.verifyAll();
        Console.WriteLine(cvr);
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

     [Test]
    public void validateDetached()  {
        SignatureContainer sc = Constants.readSignatureContainer("bes_multiple_detached");
        Assert.AreEqual(2, sc.getSignatures().Count);
        ContainerValidationResult cvr = sc.verifyAll();
        Console.WriteLine(cvr);
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

     [Test]
    public void validateDetachedSerialLeaf() {
        SignatureContainer sc = Constants.readSignatureContainer("bes_serial_detached_leaf");
        Assert.AreEqual(1, sc.getSignatures().Count);
        Assert.AreEqual(1, sc.getSignatures()[0].getCounterSignatures().Count);
        Assert.AreEqual(0, sc.getSignatures()[0].getCounterSignatures()[0].getCounterSignatures().Count);
        ContainerValidationResult cvr = sc.verifyAll();
        Console.WriteLine(cvr);
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }

     [Test]
    public void validateDetachedSerialMiddle() {
        SignatureContainer sc = Constants.readSignatureContainer("bes_serial_detached_middle");
        Assert.AreEqual(1, sc.getSignatures().Count);
        Assert.AreEqual(0, sc.getSignatures()[0].getCounterSignatures().Count);
        ContainerValidationResult cvr = sc.verifyAll();
        Console.WriteLine(cvr);
        Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
    }
}
}
