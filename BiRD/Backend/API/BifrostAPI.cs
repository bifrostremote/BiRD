using BifrostApi.Models;
using BifrostApi.Models.DTO;
using BiRD.Backend.API.RequestBuilder;
using BiRD.Backend.Enums;
using BiRD.Backend.Models.API.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BiRD.Backend.API
{
    public class BifrostAPI
    {

        private CookieContainer _cookies;
        private static BifrostAPI _cachedInstance = null;
        public event EventHandler ResponseMessageRecieved;
        private bool _useTestServer = false;
        public class ResponseMessageReceivedArgs : EventArgs
        {
            public string Message;
        }

        private BifrostAPI()
        {

        }

        public static BifrostAPI GetInstance()
        {
            if (_cachedInstance == null)
                return new BifrostAPI();
            else
                return _cachedInstance;
        }

        public void UseTestServer()
        {
            _useTestServer = true;
        }

        public void UseLiveServer()
        {
            _useTestServer = false;
        }

        public string Login(string username, string password)
        {
            BifrostBuilder builder = new BifrostBuilder(_useTestServer, 5001);
            CookieContainer cookieJar = new CookieContainer();

            builder.SetEndpoint("Auth/Authenticate");
            builder.SetEncryption(Encryption.https);
            builder.AddParameter("username", username);
            builder.AddParameter("password", password);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(builder.GetRequest().ToString());

            request.Method = "POST";
            request.CookieContainer = cookieJar;

            BaseResponse response = ReadResponse(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _cookies = request.CookieContainer;

                return response.ResponseBody;
            }

            return "";
        }

        public string GenerateWordToken(TokenPairDTO dto)
        {
            BifrostBuilder builder = new BifrostBuilder(_useTestServer, 5001);
            CookieContainer cookieJar = new CookieContainer();

            builder.SetEndpoint("Token/Word");
            builder.SetEncryption(Encryption.https);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(builder.GetRequest().ToString());

            request.Method = "POST";
            request.CookieContainer = cookieJar;

            // prepare data for transmission
            string data = JsonConvert.SerializeObject(dto);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(data);

            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;

            // Prepare to write data.
            Stream stream = request.GetRequestStream();

            // Write data to request body
            stream.Write(bytes, 0, bytes.Length);

            stream.Close();


            BaseResponse response = ReadResponse(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _cookies = request.CookieContainer;

                return response.ResponseBody;
            }

            return "";
        }

        public string GetMachineIp(string token)
        {
            BifrostBuilder builder = new BifrostBuilder(_useTestServer, 5001);
            CookieContainer cookieJar = new CookieContainer();

            builder.SetEndpoint("Token");
            builder.SetEncryption(Encryption.https);
            builder.AddParameter("token", token);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(builder.GetRequest().ToString());

            request.Method = "GET";
            request.CookieContainer = cookieJar;

            BaseResponse response = ReadResponse(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _cookies = request.CookieContainer;

                return response.ResponseBody;
            }

            return "";
        }

        public void Logout()
        {
            // Invalidate cookies, we should probably invalidate the session serverside proper but this works for the time being. 
            _cookies = null;
        }

        public List<Machine> GetMachines(Guid userUid)
        {

            BifrostBuilder builder = new BifrostBuilder(_useTestServer, 5001);

            builder.SetEndpoint("Machine");
            builder.SetEncryption(Encryption.https);
            builder.AddParameter("userUid", userUid.ToString());

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(builder.GetRequest().ToString());

            request.Method = "GET";
            request.CookieContainer = _cookies;

            BaseResponse response = ReadResponse(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var machines = JsonConvert.DeserializeObject<List<Machine>>(response.ResponseBody);

                return machines;
            }

            return new List<Machine>();
        }

        public Guid EnrollMachine(MachineCreateDTO dto)
        {
            BifrostBuilder builder = new BifrostBuilder(_useTestServer, 5001);
            CookieContainer cookieJar = new CookieContainer();

            builder.SetEndpoint("Machine");
            builder.SetEncryption(Encryption.https);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(builder.GetRequest().ToString());

            request.Method = "POST";
            request.CookieContainer = cookieJar;

            // prepare data for transmission
            string data = JsonConvert.SerializeObject(dto);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(data);

            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;

            // Prepare to write data.
            Stream stream = request.GetRequestStream();

            // Write data to request body
            stream.Write(bytes, 0, bytes.Length);

            stream.Close();


            BaseResponse response = ReadResponse(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _cookies = request.CookieContainer;

                return new Guid(response.ResponseBody);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ArgumentException(response.ResponseBody);
            }

            return Guid.Empty;

        }

        private BaseResponse ReadResponse(WebRequest request)
        {
            string responseFromAPI = "";
            BaseResponse resp = new BaseResponse();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var headerkeys = response.Headers.AllKeys;

                    for (int i = 0; i < headerkeys.Length; i++)
                    {
                        var keyvalue = new KeyValuePair<string, string>(headerkeys[i], response.Headers.GetValues(i).FirstOrDefault());

                        resp.Headers.Add(keyvalue);
                    }

                    resp.StatusCode = response.StatusCode;
                    resp.ResponseBody = GetResponse(response);


                }
            }
            catch (WebException wex)
            {
                var response = (HttpWebResponse)wex.Response;
                if (response == null)
                    return resp;

                resp.ResponseBody = GetResponse(response);
                resp.StatusCode = response.StatusCode;
            }
            return resp;
        }

        private string GetResponse(HttpWebResponse response)
        {
            string message = "";
            if (response != null)
            {
                using (var datastream = response.GetResponseStream())
                {
                    if (datastream != null)
                    {
                        using (var reader = new StreamReader(datastream))
                        {
                            message = reader.ReadToEnd();
                        }
                        datastream.Close();
                    }
                }

            }
            return message;
        }
    }
}
