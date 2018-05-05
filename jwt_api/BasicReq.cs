using System.Text;
using System.Net;
using System.IO;
using System.Drawing;

namespace jwt_api
{
    /// <summary>
    /// A simple basic class for HTTP Requests.
    /// </summary>
    internal static class BReq
    {
        /// <summary>
        /// UserAgent to be used on the requests
        /// </summary>
        public static string UserAgent = @"Mozilla/5.0 (Windows; Windows NT 6.1) AppleWebKit/534.23 (KHTML, like Gecko) Chrome/11.0.686.3 Safari/534.23";

        /// <summary>
        /// Cookie Container that will handle all the cookies.
        /// </summary>
        private static CookieContainer cJar = new CookieContainer();

        /// <summary>
        /// Performs a basic HTTP GET request.
        /// </summary>
        /// <param name="url">The URL of the request.</param>
        /// <returns>HTML Content of the response.</returns>
        public static Stream HttpGetStream(string url, bool follow_redirect = true)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cJar;
            request.UserAgent = UserAgent;
            request.KeepAlive = false;
            if (follow_redirect)
                request.AllowAutoRedirect = false;
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (follow_redirect && (response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Found))
            {
                while (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Moved)
                {
                    response.Close();
                    request = (HttpWebRequest)WebRequest.Create(response.Headers["Location"]);
                    request.AllowAutoRedirect = false;
                    request.CookieContainer = cJar;
                    response = (HttpWebResponse)request.GetResponse();
                }
            }

            return response.GetResponseStream();
        }

        public static string HttpGet(string url, bool follow_redirect = true)
        {
            using (StreamReader sm = new StreamReader(HttpGetStream(url, follow_redirect)))
                return sm.ReadToEnd();
        }

        /// <summary>
        /// Performs a basic HTTP POST request
        /// </summary>
        /// <param name="url">The URL of the request.</param>
        /// <param name="post">POST Data to be passed.</param>
        /// <param name="refer">Referrer of the request</param>
        /// <returns>HTML Content of the response.</returns>
        public static string HttpPost(string url, string post, bool follow_redirect = true, string refer = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cJar;
            request.UserAgent = UserAgent;
            request.KeepAlive = false;
            request.Method = "POST";
            request.Referer = refer;
            if (follow_redirect)
                request.AllowAutoRedirect = false;

            byte[] postBytes = Encoding.ASCII.GetBytes(post);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (follow_redirect && (response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Found))
            {
                while (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Moved)
                {
                    response.Close();
                    request = (HttpWebRequest)HttpWebRequest.Create(response.Headers["Location"]);
                    request.AllowAutoRedirect = false;
                    request.CookieContainer = cJar;
                    response = (HttpWebResponse)request.GetResponse();
                }
            }
            StreamReader sr = new StreamReader(response.GetResponseStream());

            return sr.ReadToEnd();
        }

        /// <summary>
        /// Creates an HTML file from the string.
        /// </summary>
        /// <param name="html">HTML String.</param>
        public static void DebugHtml(string html)
        {
            StreamWriter sw = new StreamWriter("debug.html");
            sw.Write(html);
            sw.Close();
        }
    }
}