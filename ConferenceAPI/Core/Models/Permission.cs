using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class Permission
    {
        public Permission()
        {
            UserPermissions = new HashSet<UserPermission>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}
