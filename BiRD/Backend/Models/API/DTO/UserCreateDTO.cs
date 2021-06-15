using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.DTO
{
    public class UserCreateDTO
    {
        public string name;
        public string email;
        public string username;
        public string unencryptedPassword;
    }
}
