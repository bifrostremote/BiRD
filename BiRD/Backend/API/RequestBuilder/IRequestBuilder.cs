using BiRD.Backend.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiRD.Backend.API.RequestBuilder
{
    interface IRequestBuilder
    {
        void SetEncryption(Encryption encryptionMethod);
        void SetEndpoint(string endpoint);
        void AddParameter(string parameter, string value);
        void RemoveParameter(string parameter);
        Uri GetRequest();
        List<KeyValuePair<string, string>> GetParameters();
    }
}
