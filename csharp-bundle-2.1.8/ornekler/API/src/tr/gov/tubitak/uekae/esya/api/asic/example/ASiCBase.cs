using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.config;
using tr.gov.tubitak.uekae.esya.api.signature.sigpackage;
using tr.gov.tubitak.uekae.esya.api.signature.util;

namespace tr.gov.tubitak.uekae.esya.api.asic.example
{
    /**
     * Provides required variables and functions for ASiC examples
     * @author suleyman.uslu
     */
    public class ASiCBase
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static String ROOT_DIR;           // root directory of project
        protected static String BASE_DIR;           // base directory where signatures created
        protected static String LICENSE;            // license file path

        protected static FileInfo DATA_FILE;        // file to be signed

        protected static ECertificate CERTIFICATE;  // certificate
        protected static BaseSigner SIGNER;         // signer

        protected static bool IS_QUALIFIED = true;         // gets only qualified certificates in smart card
        protected static String PIN = "12345";

        static ASiCBase()
        {
            String dir = Directory.GetCurrentDirectory();

            ROOT_DIR = Directory.GetParent(dir).Parent.Parent.Parent.FullName;

            if (dir.Contains("x86") || dir.Contains("x64"))
            {
                ROOT_DIR = Directory.GetParent(ROOT_DIR).FullName;
            }

            XmlConfigurator.Configure(new FileInfo(ROOT_DIR + "/config/log4net.xml"));
            logger.Debug("Root directory: " + ROOT_DIR);

            BASE_DIR = ROOT_DIR + "/testVerileri/";
            DATA_FILE = new FileInfo(BASE_DIR + "sample.txt");
            LICENSE = ROOT_DIR + "/lisans/lisans.xml";
            loadLicense();


            try
            {
                CERTIFICATE = SmartCardManager.getInstance().getSignatureCertificate(IS_QUALIFIED, !IS_QUALIFIED);
                SIGNER = SmartCardManager.getInstance().getSigner(PIN, CERTIFICATE);
            }
            catch (Exception e)
            {
                throw new SignatureException("Error in smart card operations", e);
            }

            /*
            String PFX_FILE = ROOT_DIR + "/sertifika deposu/522277_test1@kamusm.gov.tr.pfx";
            String PFX_PASS = "522277";
            PfxSigner signer = new PfxSigner(SignatureAlg.RSA_SHA256.getName(), PFX_FILE, PFX_PASS);
            CERTIFICATE = signer.getSignersCertificate();
            SIGNER = signer;
            */


        }

        /**
         * Creates an appropriate file name for ASiC signatures
         * @param packageType    package type of the signature, ASiC_S or ASiC_E
         * @param format         format of the signature, CAdES or XAdES
         * @param type           type of the signature, BES etc.
         * @return file name of associated signature as string
         */
        protected String fileName(PackageType packageType, SignatureFormat format, SignatureType type)
        {
            String fileName = BASE_DIR + packageType + "-" + format + "-" +type;
            switch (packageType){
                case PackageType.ASiC_S : return fileName + ".asics";
                case PackageType.ASiC_E : return fileName + ".asice";
            }
            return null;
        }

        /**
         * Reads an ASiC signature
         * @param packageType     type of the ASiC signature to be read, ASiC_S or ASiC_E
         * @param format          format of the ASiC signature to be read, CAdES or XAdES
         * @param type            type of the ASiC signature to be read, BES etc.
         * @return signature package of ASiC signature
         * @throws Exception
         */
        protected SignaturePackage read(PackageType packageType, SignatureFormat format, SignatureType type)
        {
            Context c = createContext();
            FileInfo f = new FileInfo(fileName(packageType, format, type));
            return SignaturePackageFactory.readPackage(c, f);
        }

        /**
         * Creates context for signature creation and validation
         * @return created context
         */
        public static Context createContext()
        {
            Uri uri = new Uri(BASE_DIR);
            Context context = new Context(new Uri(uri.AbsoluteUri));
            context.setConfig(new Config(ROOT_DIR + "/config/esya-signature-config.xml"));
            return context;
        }

        /**
         * Loads given license file
         */
        public static void loadLicense()
        {
            logger.Debug("License is being loaded from: " + LICENSE);
            LicenseUtil.setLicenseXml(new FileStream(LICENSE, FileMode.Open, FileAccess.Read));
        }
    }
}
