using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.sigpackage;

namespace tr.gov.tubitak.uekae.esya.api.asic.example
{
    /**
     * Upgrades given ASiC signatures
     * @author suleyman.uslu
     */
    public class Upgrade : ASiCBase
    {
        public void upgrade(PackageType packageType, SignatureFormat format,
                            SignatureType current, SignatureType next)
        {
            Context c = createContext();//new Context();
            SignaturePackage signaturePackage = SignaturePackageFactory.readPackage(c, new FileInfo(fileName(packageType, format, current)));
            SignatureContainer sc = signaturePackage.getContainers()[0];
            Signature signature = sc.getSignatures()[0];

            // upgrade
            signature.upgrade(next);

            signaturePackage.write(new FileStream(fileName(packageType, format, next), FileMode.OpenOrCreate));

            // validate
            PackageValidationResult pvr = signaturePackage.verifyAll();

            // output results
            Console.WriteLine(pvr);
            Assert.True(pvr.getResultType() == PackageValidationResultType.ALL_VALID);
        }

        [Test]
        public void upgrade_BES_T_X_E()
        {
            upgrade(PackageType.ASiC_E, SignatureFormat.XAdES, SignatureType.ES_BES, SignatureType.ES_T);
        }

        [Test]
        public void upgrade_BES_T_C_E()
        {
            upgrade(PackageType.ASiC_E, SignatureFormat.CAdES, SignatureType.ES_BES, SignatureType.ES_T);
        }
    }
}
