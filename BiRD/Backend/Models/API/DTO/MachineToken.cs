using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BifrostApi.Models
{
    public partial class MachineToken
    {
        public Guid Uid { get; set; }
        public string Token { get; set; }

        public bool Active { get; set; }

        public int CreateDate { get; set; }
        public Guid MachineUid { get; set; }

        public virtual Machine Machine { get; set; }
    }
}
