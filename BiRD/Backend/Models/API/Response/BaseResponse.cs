using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiRD.Backend.Models.API.Response
{
    class BaseResponse
    {
        public HttpStatusCode StatusCode;
        public string ResponseBody;
        public List<KeyValuePair<string, string>> Headers = new List<KeyValuePair<string, string>>();
    }
}
