using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BifrostApi.Models
{
    public partial class User
    {
        public User()
        {
            Machines = new HashSet<Machine>();
        }

        // We mismatch the general schema where Guids are named uid, due to the required IUser interface requiring a ID value of T, where in our case T is a Guid
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid UserGroupUid { get; set; }
        public bool Deleted { get; set; }
        public virtual UserGroup UserGroup { get; set; }
        public virtual ICollection<Machine> Machines { get; set; }

    }
}
