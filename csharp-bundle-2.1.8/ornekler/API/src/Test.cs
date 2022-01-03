using System;
using tr.gov.tubitak.uekae.esya.api.cades.example.convert;
using tr.gov.tubitak.uekae.esya.api.cades.example.sign;

/*
 * Projede testconstants dizini içerisindeki TestConstants.cs dosyasıda
 * akılı kart pin ayarları bulunmaktadır kendi ayarlarınıza göre düzenleyiniz.
 */

namespace NETAPI_TEST
{
    class Test
    {

        static void Main(string[] args)
        {
            CADES_TEST();
        }
        public static void CADES_TEST()
        {
            Console.WriteLine("------------------BESImza at------------------------");
            BESSign.testSimpleSign();

            Console.WriteLine("------------------ESTImza at------------------------");
            ESTSign.testEstSign();

            Console.WriteLine("------------------Long Imza at------------------");
            ESXLongSign.testEsxlongSign();

            Console.WriteLine("------------------BES TO EST yap------------------");
            Converts.testConvertBES_1();


            Console.WriteLine("finished!"); 
        }
    }
}
