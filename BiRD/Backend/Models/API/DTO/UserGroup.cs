using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BifrostApi.Models
{
    public partial class UserGroup
    {
        public UserGroup()
        {
            InverseParentNavigation = new HashSet<UserGroup>();
            Users = new HashSet<User>();
        }

        public Guid Uid { get; set; }
        public string Name { get; set; }
        public Guid? ParentUid { get; set; }
        public bool Deleted { get; set; }
        public virtual UserGroup ParentNavigation { get; set; }
        public ICollection<UserGroup> InverseParentNavigation { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
