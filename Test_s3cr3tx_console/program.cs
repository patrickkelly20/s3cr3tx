using System;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Tests3cr3tx
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is a test app for s3cr3tx!");
            Console.WriteLine("Please enter your email address: ");
            string strEmail = Console.ReadLine();
            Console.WriteLine("Please enter your APIToken: ");
            string strAPIToken = Console.ReadLine();
            Console.WriteLine("Please enter your Authorization Code: ");
            string strAuth = Console.ReadLine();
            Console.WriteLine("enter E to encrypt or D to decrypt: ");
            string strEoD = Console.ReadLine();
            Console.WriteLine("Please enter your input: ");
            string strInput = Console.ReadLine();

            string strResult = @"";
            strResult = getResult(strEmail, strAPIToken, strAuth, strEoD, strInput);
            Console.WriteLine(@"Your output is:");
            Console.WriteLine(strResult);


        }

        private static string getResult(string Email,string strToken,string strAuth,string strEoD,string strInput)
        {
            try
            {
                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(@"https://s3cr3tx.com/api/Values");
                
                WebClient wc = new WebClient();
                //wc.Credentials.GetCredential();
                wc.BaseAddress = @"https://s3cr3tx.com/api/Values";
                WebHeaderCollection webHeader = new WebHeaderCollection();
                webHeader.Add(@"Email:" + Email);
                webHeader.Add(@"AuthCode:" + strAuth);
                webHeader.Add(@"APIToken:" + strToken);
                webHeader.Add(@"Input:" + strInput);
                webHeader.Add(@"EorD:" + strEoD);
                webHeader.Add(@"Def:" + @"z");
                wc.Headers = webHeader;
                string result = wc.DownloadString(@"https://s3cr3tx.com/api/Values");
                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().ToString());
                Console.ReadLine();
                return ex.GetBaseException().ToString();
            }
        }

    }
}
