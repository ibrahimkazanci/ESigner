using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using log4net;
using log4net.Config;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.common;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.signature.util;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.config;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.resolver;

namespace tr.gov.tubitak.uekae.esya.api.xmlsignature.example.utils
{
    /**
     * Base class for sample codes
     * @author: suleyman.uslu
     */
    public class SampleBase
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static String ROOT_DIR;           // root directory of project
        protected static String CONFIG;             // config file path
        protected static String BASE_DIR;           // base directory where signatures created
        protected static String POLICY_FILE;        // certificate validation policy file path
        protected static String POLICY_FILE_CRL;    // path of policy file without OCSP
        protected static String PFX_FILE;           // PFX file path
        protected static String PFX_PASS;           // PFX password
        protected static String PIN;                // smart card PIN
        protected static String LICENSE;            // license file path

        protected static ECertificate CERTIFICATE;  // certificate of PFX
        protected static BaseSigner SIGNER;         // signer of pfx

        protected static bool IS_QUALIFIED;         // true if qualified certificates to be used

        protected static OfflineResolver POLICY_RESOLVER;   // policy resolver for profile examples

        public static readonly int[] OID_POLICY_P2 = new int[] { 2, 16, 792, 1, 61, 0, 1, 5070, 3, 1, 1 };
        public static readonly int[] OID_POLICY_P3 = new int[] { 2, 16, 792, 1, 61, 0, 1, 5070, 3, 2, 1 };
        public static readonly int[] OID_POLICY_P4 = new int[] { 2, 16, 792, 1, 61, 0, 1, 5070, 3, 3, 1 };

        private static readonly String ENVELOPE_XML =
                        "<envelope>\n" +
                        "  <data id=\"data1\">\n" +
                        "    <item>Item 1</item>\n" +
                        "    <item>Item 2</item>\n" +
                        "    <item>Item 3</item>\n" +
                        "  </data>\n" +
                        "</envelope>\n";

        /**
         * Initialize paths and other variables
         */
        static SampleBase()
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
            POLICY_FILE = ROOT_DIR + "/config/certval-policy-test.xml";
            POLICY_FILE_CRL = ROOT_DIR + "/config/certval-policy-test-crl.xml";
            LICENSE = ROOT_DIR + "/lisans/lisans.xml";
            CONFIG = ROOT_DIR + "/config/xmlsignature-config.xml";

            Directory.SetCurrentDirectory(ROOT_DIR);

            PFX_FILE = ROOT_DIR + "/sertifika deposu/522277_test1@kamusm.gov.tr.pfx";
            PFX_PASS = "522277";
            PfxSigner signer = new PfxSigner(SignatureAlg.RSA_SHA256.getName(), PFX_FILE, PFX_PASS);
            CERTIFICATE = signer.getSignersCertificate();
            SIGNER = signer;

            PIN = "12345";

            IS_QUALIFIED = true;

            POLICY_RESOLVER = new OfflineResolver();
            POLICY_RESOLVER.register("urn:oid:2.16.792.1.61.0.1.5070.3.1.1", ROOT_DIR + "/config/profiller/Elektronik_Imza_Kullanim_Profilleri_Rehberi.pdf", "text/plain");
            POLICY_RESOLVER.register("urn:oid:2.16.792.1.61.0.1.5070.3.2.1", ROOT_DIR + "/config/profiller/Elektronik_Imza_Kullanim_Profilleri_Rehberi.pdf", "text/plain");
            POLICY_RESOLVER.register("urn:oid:2.16.792.1.61.0.1.5070.3.3.1", ROOT_DIR + "/config/profiller/Elektronik_Imza_Kullanim_Profilleri_Rehberi.pdf", "text/plain");

            logger.Debug("Base directory: " + BASE_DIR);
            loadLicense();
        }

        public static Context createContext()
        {
            Context context = new Context(BASE_DIR);
            context.Config = new Config(CONFIG);
            return context;
        }

        public static void loadLicense()
        {
            logger.Debug("License is being loaded from: " + LICENSE);
            LicenseUtil.setLicenseXml(new FileStream(LICENSE, FileMode.Open, FileAccess.Read));
        }

        /**
         * Creates sample envelope XML that will contain signature inside
         */
        public XmlDocument newEnvelope()
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(ENVELOPE_XML);
                MemoryStream ms = new MemoryStream(bytes);

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                XmlReader reader = XmlReader.Create(ms);
                doc.Load(reader);

                return doc;
            }
            catch (Exception x)
            {
                // we shouldn't be here if ENVELOPE_XML is valid
                Console.WriteLine(x.StackTrace);
            }
            throw new ESYAException("Cant construct envelope xml ");
        }

        /**
         * Creates sample envelope XML that will contain signature inside
         * by reading the given file name in base directory
         */
        public XmlDocument newEnvelope(String file)
        {
            try
            {
                //logger.Debug(BASE_DIR + file);
                byte[] bytes = File.ReadAllBytes(BASE_DIR + file);
                MemoryStream ms = new MemoryStream(bytes);

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                XmlReader reader = XmlReader.Create(ms);
                doc.Load(reader);

                return doc;
            }
            catch (Exception x)
            {
                // we shouldn't be here if ENVELOPE_XML is valid
                Console.WriteLine(x.StackTrace);
            }
            throw new ESYAException("Cant construct envelope xml ");
        }

        /**
         * Reads an XML document into XmlDocument format
         */
        public XmlDocument parseDoc(String uri)
        {
            byte[] bytes = File.ReadAllBytes(BASE_DIR + uri);
            MemoryStream ms = new MemoryStream(bytes);

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            XmlReader reader = XmlReader.Create(ms);
            doc.Load(reader);

            return doc;
        }

        /**
         * Gets the signature by searching for tag in an XML document
         */
        public XMLSignature readSignature(XmlDocument aDocument, Context aContext)
        {
            // get the signature in enveloped signature format
            XmlNode signatureElement = aDocument.GetElementsByTagName("ds:Signature").Item(0);

            // return the XML signature created with signature element
            return new XMLSignature((XmlElement)signatureElement, aContext);
        }

        /**
         * Gets the signature by searching for tag in an XML document
         */
        public XMLSignature readSignature(XmlDocument aDocument, Context aContext, int item)
        {
            // get the signature in parallel signature format
            XmlNode signatureElement = ((XmlElement)aDocument.GetElementsByTagName("signatures").Item(0)).GetElementsByTagName("ds:Signature").Item(item);

            // return the XML signature created with signature element
            return new XMLSignature((XmlElement)signatureElement, aContext);
        }
    }
}
