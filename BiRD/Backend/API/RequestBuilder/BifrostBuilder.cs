using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiRD.Backend.API.RequestBuilder
{
    class BifrostBuilder : BaseRequestBuilder
    {   
        public BifrostBuilder(bool useTestEndpoint = false, int port = 0)
        {
            if (!useTestEndpoint)
                _baseURL = "bifrostremote.com";
            else
                _baseURL = "localhost";

            if (useTestEndpoint && port != 0)
                _port = port; 
        }
    }
}
