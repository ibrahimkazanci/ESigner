using System;
using System.IO;
using System.Text;
using log4net.Config;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.certval;
using tr.gov.tubitak.uekae.esya.api.signature.config;
using tr.gov.tubitak.uekae.esya.api.signature.impl;
namespace tr.gov.tubitak.uekae.esya.api.SignatureAPI.example
{
public class Constants {
	
	public static SignatureFormat signatureFormat=SignatureFormat.CAdES;
    public static String ROOT;
    public static String LICENCE_FILE;
	private static readonly String PIN = "12345";

    //Comment tr.gov.tubitak.uekae.esya.api.certificate.validation.check.certificate.revocation.RevocationFromOCSPChecker for POLICY_CRL_ONLY.
    private static readonly String POLICY_CRL_ONLY;
    private static readonly String POLICY_OCSP_ONLY;
    static CertValidationPolicies crlPolicies = new CertValidationPolicies();
    static CertValidationPolicies ocspPolicies = new CertValidationPolicies();

    static Constants()
    {
        
        String dir = Directory.GetCurrentDirectory();

        ROOT = Directory.GetParent(dir).Parent.Parent.Parent.FullName; 
        if (dir.Contains("x86") || dir.Contains("x64"))
        {
            ROOT = Directory.GetParent(ROOT).FullName; 
        }

        XmlConfigurator.Configure(new FileInfo(ROOT + "/config/log4net.xml"));
        //logger.Debug("Root directory: " + ROOT);

        LICENCE_FILE = ROOT + @"\lisans\lisans.xml";
        POLICY_CRL_ONLY = ROOT + @"\config\certval-policy-test.xml";
        POLICY_OCSP_ONLY = ROOT + @"\config\certval-policy-test.xml";
        setLicence();
        crlPolicies.register(null, PolicyReader.readValidationPolicy(POLICY_CRL_ONLY));
        ocspPolicies.register(null, PolicyReader.readValidationPolicy(POLICY_OCSP_ONLY));
  }
    public static void setLicence()
    {
        using (Stream license = new FileStream(LICENCE_FILE, FileMode.Open, FileAccess.Read))
        {
            LicenseUtil.setLicenseXml(license);
        }
    }
    public static Context createContext(){
        Uri uri = new Uri(ROOT + @"\testVerileri");
        Context c = new Context(new Uri(uri.AbsoluteUri));
        c.setConfig(new Config(ROOT + @"\config\esya-signature-config.xml"));
        c.setData(getContent()); // for detached CAdES signatures validation
        return c;
    }
    public static Signable getContent()
    {
        return new SignableBytes(Encoding.ASCII.GetBytes("test data"), "data.txt", "text/plain");
    }
	public static bool getCheckQCStatement()
	{
		//return false;   //Unqualified 
		return true;   //Qualified 
	}
	//Get PIN from user
	public static String getPIN()
	{
		return PIN;
	}
	
    public static String getPath(String fileName){
        String ext = "";
        switch (signatureFormat)
        {
            case SignatureFormat.XAdES: ext = ".xml"; break;
            case SignatureFormat.CAdES: ext = ".p7s"; break;
            default: throw new SystemException();
        }

        return ROOT + "/testVerileri/" + fileName + ext;
    }	
    public static void dosyaYaz(SignatureContainer sc, String fileName) {

        FileStream fis = new FileStream(getPath(fileName), FileMode.OpenOrCreate);
        sc.write(fis);
        fis.Close();
    }
    
    public static SignatureContainer readSignatureContainer(String fileName)  {
        return readSignatureContainer(fileName, createContext());
    }
    public static SignatureContainer readSignatureContainer(String fileName, Context c)  {
        FileStream fis = new FileStream(getPath(fileName), FileMode.Open, FileAccess.Read);
        SignatureContainer container = SignatureFactory.readContainer(fis, c);
        fis.Close();
        return container;
    }
    public static CertValidationPolicies getCRLOnlyPolicies()
    {
        return crlPolicies;
    }

    public static CertValidationPolicies getOCSPOnlyPolicies()
    {
        return ocspPolicies;
    }
}
}

