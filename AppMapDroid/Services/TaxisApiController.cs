using System;
using System.Collections.Generic;
//using System.Web;
//using System.Web.Http;
//using System.Net.Http;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace Taxi
{
    /// <summary>
    /// WebAPI Controller for HelloWorld app
    /// </summary>
    public static class taxisController
    {
        #region WebAPI Calls
        /// <summary>
        /// Returns a Hello World message via the WebAPI
        /// </summary>
        /// <returns></returns>       
        public static string POST(string url, Dictionary<string, string> parameters)
        {
            //Enums.BookingResponse             
            string authTokenString = string.Empty;
            NameValueCollection postFieldNameValue = new NameValueCollection();
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                postFieldNameValue.Add(parameter.Key, parameter.Value);
            }
            string postData = GetPostStringFrom(postFieldNameValue);
			WebRequest.DefaultWebProxy = null;


            // Change URL and use Web.config parameters
            WebRequest request = WebRequest.Create("http://dev.mob1taxi.net/CustomerService/" + url + "?" + postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-orm-urlencoded";
            Stream dataStream = request.GetRequestStream();
            dataStream.Close();

            WebResponse response = request.GetResponse();
            if (((HttpWebResponse)response).StatusCode.Equals(HttpStatusCode.OK))
            {
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                authTokenString = responseFromServer;
                reader.Close();
                dataStream.Close();
                response.Close();
                try
                {
                    return authTokenString;

                }
                catch (Exception ex)
                {
                    return ex.Message;
                };
            }
            return null;
        }

        private static string GetPostStringFrom(NameValueCollection postFieldNameValue)
        {
            //throw new NotImplementedException();
            List<string> items = new List<string>();

            foreach (String name in postFieldNameValue)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(postFieldNameValue[name])));

            return String.Join("&", items.ToArray());
        }

        #endregion
    }
}