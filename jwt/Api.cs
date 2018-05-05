using JWT.Algorithms;
using JWT.Builder;
using System;
using System.IO;
using System.Linq;
using System.Security;

namespace jwt_api
{
    public class Api
    {
        //Coger esto del servidor PHP
        //private const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";

        private const string secretUrl = "https://qbz28b-user.freehosting.host/api.php?action=secret";

        private static T ManipulateSecret<T>(Func<string, T> manipulation)
        {
            //return manipulation(GetSecureString(BReq.HttpGetStream(secretUrl)));
            return manipulation(BReq.HttpGet(secretUrl));
        }

        public static string GetToken()
        {
            return ManipulateSecret((secret) =>
            {
                var token = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(secret)
                    .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
                    .AddClaim("claim2", "claim2-value")
                    .Build();

                //secret.Clear();
                //secret.Dispose();

                return token;
            });
        }

        public static SecureString GetSecureString(Stream sm)
        {
            SecureString ret = new SecureString();
            sm.ReadFully(sm.Length).Select(x => (char)x).ForEach(y => ret.AppendChar(y));

            sm.Close();
            sm.Dispose();

            return ret;
        }
    }
}