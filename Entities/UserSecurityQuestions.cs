using System;
using System.Collections.Generic;

namespace DocumentTracking.API.Entities
{
    public partial class UserSecurityQuestions
    {
        public UserSecurityQuestions()
        {
            UserSecureAccounts = new HashSet<UserSecureAccounts>();
        }

        public Guid Id { get; set; }
        public string SecurityQuestion { get; set; }

        public virtual ICollection<UserSecureAccounts> UserSecureAccounts { get; set; }
    }
}
