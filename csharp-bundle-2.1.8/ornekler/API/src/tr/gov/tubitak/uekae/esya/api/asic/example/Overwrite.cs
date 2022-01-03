using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.sigpackage;

namespace tr.gov.tubitak.uekae.esya.api.asic.example
{
    /**
     * Adds new signatures to given containers, new extended signature
     * will have the same name with the old one
     * @author suleyman.uslu
     */
    public class Overwrite : ASiCBase
    {
        [Test]
        public void update_CAdES()
        {
            SignaturePackage sp = read(PackageType.ASiC_E, SignatureFormat.CAdES, SignatureType.ES_BES);
            String filename = fileName(PackageType.ASiC_E, SignatureFormat.CAdES, SignatureType.ES_BES) + "-upgraded.asice";
            FileInfo toUpgrade = new FileInfo(filename);

            // create a copy tu update package
            sp.write(new FileStream(filename, FileMode.OpenOrCreate));

            // add signature
            SignaturePackage sp2 = SignaturePackageFactory.readPackage(createContext(), toUpgrade);
            SignatureContainer sc = sp2.createContainer();
            Signature s = sc.createSignature(CERTIFICATE);
            s.addContent(sp.getDatas()[0], false);
            s.sign(SIGNER);

            // write onto read file!
            sp2.write();

            // read again to verify
            SignaturePackage sp3 = SignaturePackageFactory.readPackage(createContext(), new FileInfo(filename));
            PackageValidationResult pvr = sp3.verifyAll();
            Console.WriteLine(pvr);

            Assert.True(pvr.getResultType() == PackageValidationResultType.ALL_VALID);
        }
    }
}
