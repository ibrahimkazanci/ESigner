using System;
using System.Collections.Generic;
using System.IO;
using tr.gov.tubitak.uekae.esya.api.cades.example.testconstants;
using tr.gov.tubitak.uekae.esya.api.cades.example.validation;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.asn.util;

/**
 * Conversion to ESA. ESA signature can not be created directly. They must be converted from other signature types.
 * Firstly run sign operations in order to create signatures to be converted. 
 * @author orcun.ertugrul
 *
 */

namespace tr.gov.tubitak.uekae.esya.api.cades.example.convert
{
    [TestFixture]
    public class Converts
    {
        private static String mp3File = "D:\\mp3\\yabancı\\dido\\a.mp3";
        private static String movieFile = "D:\\share\\film\\Life\\Life S01E01 Challenges of Life.mkv";

        private readonly static String signatureofHugeFile = TestConstants.getDirectory() +
                                                      "\\testVerileri\\HugeExternalContent.p7s";

        private readonly static String signatureofSmallFile = TestConstants.getDirectory() +
                                                       "\\testVerileri\\SmallExternalContent.p7s";

        /**
         * Converting BES signature to ESA
         * @throws Exception
         */

        [Test]
        public static void testConvertBES_1()
        {

            byte[] content = AsnIO.dosyadanOKU(TestConstants.getDirectory() + @"\testVerileri\BES-1.p7s");

            BaseSignedData bs = new BaseSignedData(content);

            Dictionary<String, Object> parameters = new Dictionary<String, Object>();

            //Several time stamps are needed while converting to ESA; so time stamps settings must be given.
            parameters[EParameters.P_TSS_INFO] = TestConstants.getTSSettings();

            parameters[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();

            bs.getSignerList()[0].convert(ESignatureType.TYPE_ESA, parameters);

            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            AsnIO.dosyayaz(bs.getEncoded(), di.FullName + @"\ESA-1.p7s");
            

            SignedDataValidationResult sdvr = ValidationUtil.validate(bs.getEncoded(), null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }

        /**
         * Converting XLong signature to ESA.
         * @throws Exception
         */

        [Test]
        public static void testConvertXLong_2()
        {
            byte[] content =
                AsnIO.dosyadanOKU(TestConstants.getDirectory() + @"\testVerileri\ESXLong-1.p7s");
            BaseSignedData bs = new BaseSignedData(content);

            Dictionary<String, Object> parameters = new Dictionary<String, Object>();

            //Archive time stamp is added to signature, so time stamp settings are needed.
            parameters[EParameters.P_TSS_INFO] = TestConstants.getTSSettings();

            parameters[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();

            bs.getSignerList()[0].convert(ESignatureType.TYPE_ESA, parameters);

            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            AsnIO.dosyayaz(bs.getEncoded(), di.FullName + @"\ESA-2.p7s");

           

            SignedDataValidationResult sdvr = ValidationUtil.validate(bs.getEncoded(), null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }

        /**
         * Converting external signature to ESA.
         * @throws Exception
         */

        [Test]
        public static void testConvertExternalContentSignature_3()
        {
            FileInfo file = new FileInfo(mp3File);
            ISignable signable = new SignableFile(file, 2048);

            byte[] content = AsnIO.dosyadanOKU(signatureofSmallFile);
            BaseSignedData bs = new BaseSignedData(content);

            Dictionary<String, Object> parameters = new Dictionary<String, Object>();

            //Archive time stamp is added to signature, so time stamp settings are needed.
            parameters[EParameters.P_TSS_INFO] = TestConstants.getTSSettings();

            parameters[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
            parameters[EParameters.P_EXTERNAL_CONTENT] = signable;

            bs.getSignerList()[0].convert(ESignatureType.TYPE_ESA, parameters);

            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            AsnIO.dosyayaz(bs.getEncoded(), di.FullName + @"\ESA-3.p7s");            

            SignedDataValidationResult sdvr = ValidationUtil.validate(bs.getEncoded(), signable);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }

        /**
         * Converting external signature of a huge file to ESA.
         * @throws Exception
         */

        [Test]
        public static void testConvertHugeExternalContentSignature_4()
        {
            FileInfo file = new FileInfo(movieFile);
            ISignable signable = new SignableFile(file, 2048);

            byte[] content = AsnIO.dosyadanOKU(signatureofHugeFile);
            BaseSignedData bs = new BaseSignedData(content);

            Dictionary<String, Object> parameters = new Dictionary<String, Object>();

            //Archive time stamp is added to signature, so time stamp settings are needed.
            parameters[EParameters.P_TSS_INFO] = TestConstants.getTSSettings();

            parameters[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
            parameters[EParameters.P_EXTERNAL_CONTENT] = signable;

            bs.getSignerList()[0].convert(ESignatureType.TYPE_ESA, parameters);

            DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            AsnIO.dosyayaz(bs.getEncoded(), di.FullName + @"\ESA-4.p7s");
          

            SignedDataValidationResult sdvr = ValidationUtil.validate(bs.getEncoded(), signable);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }
    }
}