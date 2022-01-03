using System;
using System.IO;
using System.Runtime.CompilerServices;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using tr.gov.tubitak.uekae.esya.api.infra.tsclient;

/**
 * Examples gets the test constants from that class. Default UGTestConstants is set.
 * @author orcun.ertugrul
 *
 */

namespace tr.gov.tubitak.uekae.esya.api.cades.example.testconstants
{
    public class TestConstants
    {
        private static String DIRECTORY;
        
        private static readonly String PIN = "12345";

        private static ValidationPolicy POLICY;

        private static String POLICY_FILE;

        protected static String LICENCE_FILE;

        static TestConstants()
        {
            String dir = Directory.GetCurrentDirectory();

            DIRECTORY = Directory.GetParent(dir).Parent.Parent.Parent.FullName;
            if (dir.Contains("x86") || dir.Contains("x64"))
            {
                DIRECTORY = Directory.GetParent(DIRECTORY).FullName;
            }

            POLICY_FILE = dir + @"\certval-policy.xml";
            LICENCE_FILE = dir + @"\lisans\lisans.xml";
            setLicence();

           // String dir = Directory.GetCurrentDirectory();

           //// DIRECTORY = Directory.GetParent(dir).Parent.Parent.Parent.FullName;
           // DIRECTORY = Directory.GetParent(dir).Parent.Parent.FullName;
           // //if (dir.Contains("x86") || dir.Contains("x64"))
           // //{
           // //    DIRECTORY = Directory.GetParent(DIRECTORY).FullName; 
           // //}

           // POLICY_FILE = DIRECTORY + @"\config\certval-policy-test.xml";
           // LICENCE_FILE = DIRECTORY + @"\lisans\lisans.xml";
           // setLicence();
        }

        public static String getDirectory()
        {
            return DIRECTORY;
        }
        public static void setLicence()
        {
            using (Stream license = new FileStream(LICENCE_FILE, FileMode.Open, FileAccess.Read))
            {
                LicenseUtil.setLicenseXml(license);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ValidationPolicy getPolicy()
        {
            if (POLICY == null)
            {
                try
                {
                    POLICY = PolicyReader.readValidationPolicy(new FileStream(POLICY_FILE, FileMode.Open));
                    //For UEKAE Test Environment, we add our test roots.
                    //Dictionary<String, Object> parameters = new Dictionary<String, Object>();
                    //parameters["dizin"] = DIRECTORY + @"\sertifika deposu\test kok sertifika\";
                    //POLICY.bulmaPolitikasiAl().addTrustedCertificateFinder("tr.gov.tubitak.uekae.esya.api.certificate.validation.find.certificate.trusted.TrustedCertificateFinderFromFileSystem",
                    //        parameters);
                }
                catch (FileNotFoundException e)
                {
                    throw new SystemException("Policy file could not be found", e);
                }
            }
            return POLICY;
        }

        public static TSSettings getTSSettings()
        {
            //for getting test TimeStamp or qualified TimeStamp account, mail to bilgi@kamusm.gov.tr.
            //This configuration, user ID (2) and password (PASSWORD), is invalid. 
            return new TSSettings("http://tzd.kamusm.gov.tr", 2, "12345678", DigestAlg.SHA256);
        }

        //To-Do Get PIN from user
        public static String getPIN()
        {
            return PIN;
        }
        public static bool getCheckQCStatement()
        {
            //return false;   //Unqualified 
            return true;   //Qualified
        }
        public static bool validateCertificate()
        {
            return false;
        }
       

    }
}