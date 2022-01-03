using System;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.signature;

namespace tr.gov.tubitak.uekae.esya.api.SignatureAPI.example
{
public class Upgrade {

	public void upgrade(String fileNameRead, String fileNameWrite, SignatureType convertToType)	
	{
	    SignatureContainer sc = Constants.readSignatureContainer(fileNameRead);
	    Signature signature = sc.getSignatures()[0];
	    signature.upgrade(convertToType);
	    Constants.dosyaYaz(sc, fileNameWrite);
    	
	    SignatureContainer read = Constants.readSignatureContainer(fileNameWrite);
	    Signature readSignature = read.getSignatures()[0];
	    Assert.AreEqual(convertToType, readSignature.getSignatureType());
	    ContainerValidationResult cvr = read.verifyAll();
	    Console.WriteLine(cvr);
	    Assert.AreEqual(ContainerValidationResultType.ALL_VALID, cvr.getResultType());
	}


	 [Test]
	public void upgradeBesToT()  {
        upgrade("bes_detached", "upgrade_BES_T", SignatureType.ES_T);
	}
	
	 [Test]
	public void upgradeBesToC()  {
        upgrade("bes_detached", "upgrade_BES_C", SignatureType.ES_C);
	}
	
	 [Test]
	public void upgradeBesToX1()  {
        upgrade("bes_detached", "upgrade_BES_X1", SignatureType.ES_X_Type1);
	}
	
	 [Test]
	public void upgradeBesToXL()  {
        upgrade("bes_detached", "upgrade_BES_xL", SignatureType.ES_XL_Type1);
	}
	
	 [Test]
	public void upgradeTToC()  {
        upgrade("upgrade_BES_T", "upgrade_T_C", SignatureType.ES_C);
	}
	
	 [Test]
	public void upgradeTToX1()  {
        upgrade("upgrade_BES_T", "upgrade_T_X1", SignatureType.ES_X_Type1);
	}
	
	 [Test]
	public void upgradeTToXL()  {
        upgrade("upgrade_BES_T", "upgrade_T_XL", SignatureType.ES_XL_Type1);
	}
	
	 [Test]
	public void upgradeCToXL()  {
        upgrade("upgrade_T_C", "upgrade_C_XL", SignatureType.ES_XL_Type1);
	}


}

}
