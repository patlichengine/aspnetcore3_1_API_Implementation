using System;
using System.Collections.Generic;

namespace DocumentTracking.API.Entities
{
    public partial class UserGroups
    {
        public UserGroups()
        {
            Users = new HashSet<Users>();
        }

        public Guid Id { get; set; }
        public string GroupTitle { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
