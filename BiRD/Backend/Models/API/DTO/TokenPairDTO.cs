using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.DTO
{
    public class TokenPairDTO
    {
        public int SecurityLevel { get; set; }
        public Guid MachineUid { get; set; }
    }

}
