using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.DTO
{
    public class MachineHierarchyDTO
    {
        public Guid user_uid;
        public string username;
        public Guid user_group_id;
        public Guid machine_uid;
        public string machine_ip;
        public string machine_lastonline;
    }
}
