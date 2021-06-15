using BiRD.Backend.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiRD.Backend.API.RequestBuilder
{
    public class BaseRequestBuilder : IRequestBuilder
    {
        protected string _baseURL;
        protected string _testURL;
        protected int _port;
        protected bool _useHTTPS = true;
        protected string _endpoint;
        protected string _baseAPIPath;
        protected List<KeyValuePair<string, string>> _parameters = new List<KeyValuePair<string, string>>();
        
        public void AddParameter(string parameter, string value)
        {
            if (parameter != "" || value != "")
            {
                KeyValuePair<string, string> parameterEntry = new KeyValuePair<string, string>(parameter, value);

                _parameters.Add(parameterEntry);
            }
        }

        public List<KeyValuePair<string, string>> GetParameters()
        {
            if (_parameters == null || _parameters.Count == 0)
                return new List<KeyValuePair<string, string>>();

            return _parameters;
        }

        public Uri GetRequest()
        {
            UriBuilder builder = new UriBuilder();

            if (_useHTTPS)
                builder.Scheme = @"https:\\";
            else
                builder.Scheme = @"http:\\";

            builder.Host = _baseURL;
            builder.Path = _endpoint;
            
            if (_port != 0)
                builder.Port = _port;


            if (_parameters.Count == 0)
                return builder.Uri;

            if (_endpoint == "")
                throw new ArgumentException("Endpoint has not been set");

            string query = "?";

            for (int i = 0; i < _parameters.Count; i++)
            {
                query += _parameters[i].Key + "=" + _parameters[i].Value;
                query += "&";
            }

            // remove the last & from the query we dont have time to make an elegant fix, this works.
            query = query.Remove(query.Length - 1);


            builder.Query = query;

            return builder.Uri;
        }

        public void RemoveParameter(string parameter)
        {
            if (parameter != "")
            {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>(_parameters.Where(x => x.Key == parameter).ToList());

                if (parameters.Count == 1)
                {
                    var matchingParameter = parameters.FirstOrDefault();

                    _parameters.Remove(matchingParameter);
                }
            }
        }

        public void SetEncryption(Encryption encryptionMethod)
        {
            switch (encryptionMethod)
            {
                case Encryption.https:
                    {
                        _useHTTPS = true;
                        break;
                    }
                case Encryption.http:
                    {
                        _useHTTPS = false;
                        break;
                    }
            }
        }

        public void SetEndpoint(string endpoint)
        {
            if (endpoint != "")
            {
                _endpoint = endpoint;
            }
        }

        public void SetHeader()
        {

        }


    }
}
