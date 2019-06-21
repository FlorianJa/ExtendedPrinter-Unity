#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Web.Http;
using System.Net.Http;
using WebSocketSharp;
using Windows.Storage.Streams;
using System.Net.Http.Headers;

namespace OctoprintClient

{
    /// <summary>
    /// is the base Class connecting your project to different parts of Octoprint.
    /// </summary>
    public class OctoprintConnectionUWP : OctoprintConnection
    {

        public WebSocketSharp.WebSocket WebSocket;

        /// <summary>
        /// Creates a <see cref="T:OctoprintClient.OctoprintConnection"/> 
        /// </summary>
        /// <param name="eP">The endpoint Address like "http://192.168.1.2/"</param>
        /// <param name="aK">The Api Key of the User account you want to use. You can get this in the user settings</param>

        public OctoprintConnectionUWP(string eP, string aK) : base(eP, aK)
        {
            Position = new OctoprintPos(this);
            Files = new OctoprintFileTracker(this);
            Jobs = new OctoprintJobTracker(this);
            Printer = new OctoprintPrinter(this);

            WebSocket = new WebSocket(GetWebsocketurl());
            WebSocket.ConnectAsync();
            WebSocket.OnMessage += webSocket_OnMessage;
        }

        private void webSocket_OnMessage(object sender, MessageEventArgs e)
        {
            ParseData(e.Data);
        }


        /// <summary>
        /// A Get request for any String using your Account
        /// </summary>
        /// <returns>The result as a String, doesn't handle Exceptions</returns>
        /// <param name="location">The url sub-address like "http://192.168.1.2/<paramref name="location"/>"</param>
        internal override string Get(string location)
        {
            string strResponseValue = string.Empty;
            Debug.WriteLine("This was searched:");
            Debug.WriteLine(EndPoint + location + "?apikey=" + ApiKey);

            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            //var headers = 
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", ApiKey); 
            //headers.Add("X-Api-Key", ApiKey);
            Uri requestUri = new Uri(EndPoint + location);
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                //Send the GET request
                httpResponse = httpClient.GetAsync(requestUri).AsTask().GetAwaiter().GetResult();
                //httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = httpResponse.Content.ReadAsStringAsync().AsTask().GetAwaiter().GetResult();
                strResponseValue = httpResponseBody;
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            return strResponseValue;
        }

        /// <summary>
        /// Posts a string with the rights of your Account to a given <paramref name="location"/>..
        /// </summary>
        /// <returns>The Result if any exists. Doesn't handle exceptions</returns>
        /// <param name="location">The url sub-address like "http://192.168.1.2/<paramref name="location"/>"</param>
        /// <param name="arguments">The string to post tp the address</param>
        internal override string PostString(string location, string arguments)
        {
            string strResponseValue = string.Empty;
            Debug.WriteLine("This was searched:");
            Debug.WriteLine(EndPoint + location + "?apikey=" + ApiKey);

            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-Api-Key", ApiKey);
            Uri requestUri = new Uri(EndPoint + location);
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                HttpStringContent arg = new HttpStringContent(arguments);
                //Send the GET request
                httpResponse = httpClient.PostAsync(requestUri, arg).AsTask().GetAwaiter().GetResult();
                //httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = httpResponse.Content.ReadAsStringAsync().AsTask().GetAwaiter().GetResult();
                strResponseValue = httpResponseBody;
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            return strResponseValue;
        }

        /// <summary>
        /// Posts a JSON object as a string, uses JObject from Newtonsoft.Json to a given <paramref name="location"/>.
        /// </summary>
        /// <returns>The Result if any exists. Doesn't handle exceptions</returns>
        /// <param name="location">The url sub-address like "http://192.168.1.2/<paramref name="location"/>"</param>
        /// <param name="arguments">The Newtonsoft Jobject to post tp the address</param>
        internal override string PostJson(string location, JObject arguments)
        {
            string strResponseValue = string.Empty;
            Debug.WriteLine("This was searched:");
            Debug.WriteLine(EndPoint + location + "?apikey=" + ApiKey);
            String argumentString = string.Empty;
            argumentString = JsonConvert.SerializeObject(arguments);

            //var content = new StringContent(arguments.ToString(), Encoding.UTF8, "application/json");


            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-Api-Key", ApiKey);
            //headers.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
            Uri requestUri = new Uri(EndPoint + location);
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                HttpStringContent arg = new HttpStringContent(argumentString, UnicodeEncoding.Utf8, "application/json");
                //arg.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //Send the GET request
                httpResponse = httpClient.PostAsync(requestUri, arg).AsTask().GetAwaiter().GetResult();
                //httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = httpResponse.Content.ReadAsStringAsync().AsTask().GetAwaiter().GetResult();
                strResponseValue = httpResponseBody;
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            return strResponseValue;
        }

        /// <summary>
        /// Posts a Delete request to a given <paramref name="location"/>
        /// </summary>
        /// <returns>The Result if any, shouldn't return anything.</returns>
        /// <param name="location">The url sub-address like "http://192.168.1.2/<paramref name="location"/>"</param>
        internal override string Delete(string location)
        {
            string strResponseValue = string.Empty;
            Debug.WriteLine("This was deleted:");
            Debug.WriteLine(EndPoint + location + "?apikey=" + ApiKey);

            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-Api-Key", ApiKey);
            Uri requestUri = new Uri(EndPoint + location);
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                //Send the GET request
                httpResponse = httpClient.DeleteAsync(requestUri).AsTask().GetAwaiter().GetResult();
                //httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = httpResponse.Content.ReadAsStringAsync().AsTask().GetAwaiter().GetResult();
                strResponseValue = httpResponseBody;
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            return strResponseValue;
        }

        /// <summary>
        /// Posts a multipart reqest to a given <paramref name="location"/>
        /// </summary>
        /// <returns>The Result if any.</returns>
        /// <param name="packagestring">A packagestring should be generated elsewhere and input here as a String</param>
        /// <param name="location">The url sub-address like "http://192.168.1.2/<paramref name="location"/>"</param>
        internal override string PostMultipart(string fileData, string location, string path = "")
        {

string strResponseValue = string.Empty;
            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-Api-Key", ApiKey);
            Uri requestUri = new Uri(EndPoint + location);
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            HttpMultipartFormDataContent multipartContent = new HttpMultipartFormDataContent();
            multipartContent.Add(new HttpStringContent(fileData), "file", "customMove.gcode");
            multipartContent.Add(new HttpStringContent("true"),"select");
            multipartContent.Add(new HttpStringContent("true"),"print");
            if(path != "") multipartContent.Add(new HttpStringContent(path),"path");
            try
            {
                
                //Send the GET request
                Windows.Web.Http.HttpResponseMessage response = httpClient.PostAsync(requestUri, multipartContent).AsTask().GetAwaiter().GetResult();

                httpResponseBody = httpResponse.Content.ReadAsStringAsync().AsTask().GetAwaiter().GetResult();
                strResponseValue = httpResponseBody;
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            return strResponseValue;

        }
    }
}

#endif