using System;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace jwt_api
{
    public class Api
    {
        //Coger esto del servidor PHP
        //private const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";

        private const string secretUrl = "https://qbz28b-user.freehosting.host/api.php?action=secret";

        private static T ManipulateSecret<T>(Func<SecureString, T> manipulation)
        {
            ConnPreCheck();

            return manipulation(GetSecureString(BReq.HttpGetStream(secretUrl)));
            //return manipulation(BReq.HttpGet(secretUrl));
        }

        private static void ConnPreCheck()
        {
            //Pre-check
            string rootUrl = new Uri(secretUrl).GetLeftPart(UriPartial.Authority);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rootUrl);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                if (!CheckTrustedConnection(request, response))
                    throw new SecurityException("Somebody is trying to sniffing over the network!");
        }

        public static string GetToken()
        {
            return ManipulateSecret((secret) =>
            {
                //https://www.codeproject.com/Tips/1208535/Create-and-Consume-JWT-Tokens-in-Csharp
                /*var token = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(secret.ToString())
                    .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
                    .AddClaim("claim2", "claim2-value")
                    .Build();*/

                secret.Clear();
                secret.Dispose();

                return ""; // token;
            });
        }

        public static SecureString GetSecureString(Stream sm)
        {
            SecureString ret = new SecureString();
            //sm.ReadFully(sm.Length)
            using (StreamReader sr = new StreamReader(sm))
                sr.ReadToEnd().ForEach(y => ret.AppendChar(y));

            sm.Close();
            sm.Dispose();

            return ret;
        }

        private static bool CheckTrustedConnection(HttpWebRequest request, HttpWebResponse response)
        {
            //retrieve the ssl cert and assign it to an X509Certificate object
            X509Certificate cert = request.ServicePoint.Certificate;

            //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
            X509Certificate2 cert2 = new X509Certificate2(cert);

            //Console.WriteLine(cert2.ToString());
            //Console.WriteLine("Is Valid?: {0}", cert2.Verify());

            return cert2.Verify();
        }

        /*

                if (dumpLocalCerts)
                {
                    var store = new X509Store(StoreLocation.CurrentUser); //StoreLocation.LocalMachine fails too
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    var certificates = store.Certificates;

                    Console.WriteLine(certificates.Count);
                    foreach (var certificate in certificates)
                    {
                        //var friendlyName = certificate.FriendlyName;
                        Console.WriteLine(certificate.ToString());
                        Console.WriteLine("\n{0}\n", new string('-', 20));
                    }
                }

         */
    }
}