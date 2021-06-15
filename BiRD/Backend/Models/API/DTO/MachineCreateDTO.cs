using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.DTO
{
    public class MachineCreateDTO
    {
        public string Name { get; set; }
        public Guid UserUid { get; set; }
        public string IPAddress { get; set; }
    }
}
