using System;
using System.Collections.Generic;

namespace DocumentTracking.API.Entities
{
    public partial class UserSecureAccounts
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SecurityQuestionId { get; set; }
        public byte[] SecurityAnswer { get; set; }

        public virtual UserSecurityQuestions SecurityQuestion { get; set; }
        public virtual Users User { get; set; }
    }
}
