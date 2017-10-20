using System;
using System.Net.Http;
using Accipitridae;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sending test request!");
            /*var gen = new HawkGenerator("dh37fgj492je","werxhqb98rpaxn39848xrunpaw3489ruxnpa98w4rxn");
            var str = gen.GetMac(new Uri("http://example.com:8000/resource/1?b=1&a=2"),"GET","\nsome-app-ext-data","j4h3g2",1353832234);
            Console.WriteLine(str);
        */
            
            var hawkHandler = new HawkAuthHandler("d74s3nz2873n","werxhqb98rpaxn39848xrunpaw3489ruxnpa98w4rxn");
            var client = new HttpClient(hawkHandler);
            var res=client.GetAsync("http://localhost:8000/hello");
            res.Wait();
            Console.WriteLine(res.Result.StatusCode);

            res=client.PostAsync("http://localhost:8000/hello",new StringContent("This is a test!"));
            res.Wait();
            Console.WriteLine(res.Result.StatusCode);
        
            res=client.PutAsync("http://localhost:8000/hello",new StringContent("This is a test!"));
            res.Wait();
            Console.WriteLine(res.Result.StatusCode);

            res=client.DeleteAsync("http://localhost:8000/hello");
            res.Wait();
            Console.WriteLine(res.Result.StatusCode);

        }
    }
}
