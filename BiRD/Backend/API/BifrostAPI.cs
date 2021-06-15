using BifrostApi.Models;
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

namespace BiRD.Backend.API
{
    public class BifrostAPI
    {

        private CookieContainer _cookies;
        private static BifrostAPI _cachedInstance = null;
        public event EventHandler ResponseMessageRecieved;
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

        public bool Login(string username, string password)
        {
            BifrostBuilder builder = new BifrostBuilder(true, 5001);
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

                return true;
            }

            return false;
        }

        public void Logout()
        {
            // Invalidate cookies, we should probably invalidate the session serverside proper but this works for the time being. 
            _cookies = null;
        }

        public List<Machine> GetMachine(Guid userUid)
        {

            BifrostBuilder builder = new BifrostBuilder(true, 5001);

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

        private BaseResponse ReadResponse(WebRequest request)
        {
            string responseFromAPI = "";
            BaseResponse resp = new BaseResponse();


            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var headerkeys = response.Headers.AllKeys;

                for (int i = 0; i < headerkeys.Length; i++)
                {
                    var keyvalue = new KeyValuePair<string, string>(headerkeys[i], response.Headers.GetValues(i).FirstOrDefault());

                    resp.Headers.Add(keyvalue);
                }

                resp.StatusCode = response.StatusCode;

                using (var datastream = response.GetResponseStream())
                {
                    if (datastream != null)
                    {
                        using (var reader = new StreamReader(datastream))
                        {
                            resp.ResponseBody = reader.ReadToEnd();
                        }
                        datastream.Close();
                    }
                }
            }
            return resp;
        }
    }
}
