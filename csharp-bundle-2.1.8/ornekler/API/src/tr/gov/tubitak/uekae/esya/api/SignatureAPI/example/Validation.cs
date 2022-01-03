

using System;
using System.IO;
using tr.gov.tubitak.uekae.esya.api.signature;

namespace tr.gov.tubitak.uekae.esya.api.SignatureAPI.example
{
public class Validation {
	
    public static ContainerValidationResult validateSignature(String fileName)  {
        return validateSignature(fileName, Constants.createContext());
    }
    public static ContainerValidationResult validateSignature(String fileName, Context c)  {

        FileStream fis = new FileStream(Constants.getPath(fileName), FileMode.Open, FileAccess.Read);

        SignatureContainer container = SignatureFactory.readContainer(fis, c);

        fis.Close();

        ContainerValidationResult cvr = container.verifyAll();
        debugCVR(cvr);

        return cvr;
    }
    public static void debugCVR(ContainerValidationResult cvr){
        Console.WriteLine("--------------------------");
        Console.WriteLine(cvr);
        /*
        System.out.println(cvr.getResultType());
        int index = 0;
        for (SignatureValidationResult svr : cvr.getAllResults().values()){
            index++;
            System.out.println("Signature "+index);
            System.out.println(svr.getResultType());
            debugDetails(svr.getDetails(), 1);
        } */
        Console.WriteLine("--------------------------");
    }     
}
}
