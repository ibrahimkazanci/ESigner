using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.impl;
using tr.gov.tubitak.uekae.esya.api.signature.sigpackage;

namespace tr.gov.tubitak.uekae.esya.api.asic.example
{
    /**
     * Generates ASiC-S and -E signatures with CAdES and XAdES BES
     * @author suleyman.uslu
     */
    public class Generate : ASiCBase
    {
        public void createBES(PackageType packageType, SignatureFormat format)
        {
            Context c = createContext();

            SignaturePackage signaturePackage = SignaturePackageFactory.createPackage(c, packageType, format);

            // add into zip
            Signable inPackage = signaturePackage.addData(new SignableFile(DATA_FILE, "text/plain"), "sample.txt");

            SignatureContainer container = signaturePackage.createContainer();

            Signature signature = container.createSignature(CERTIFICATE);

            // pass document in ZIP to signature
            signature.addContent(inPackage, false);

            signature.sign(SIGNER);

            String filename = fileName(packageType, format, SignatureType.ES_BES);
            signaturePackage.write(new FileStream(filename, FileMode.OpenOrCreate));

            // read it back
            signaturePackage = SignaturePackageFactory.readPackage(c, new FileInfo(filename));

            // validate
            PackageValidationResult pvr = signaturePackage.verifyAll();

            // output results
            Console.WriteLine(pvr);
            Assert.True(pvr.getResultType() == PackageValidationResultType.ALL_VALID);
        }

        [Test]
        public void createASiC_S_CAdES()
        {
            createBES(PackageType.ASiC_S, SignatureFormat.CAdES);
        }

        [Test]
        public void createASiC_E_CAdES()
        {
            createBES(PackageType.ASiC_E, SignatureFormat.CAdES);
        }

        [Test]
        public void createASiC_S_XAdES()
        {
            createBES(PackageType.ASiC_S, SignatureFormat.XAdES);
        }

        [Test]
        public void createASiC_E_XAdES()
        {
            createBES(PackageType.ASiC_E, SignatureFormat.XAdES);
        }
    }
}
