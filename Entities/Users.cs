using System;
using System.Collections.Generic;

namespace DocumentTracking.API.Entities
{
    public partial class Users
    {
        public Users()
        {
            AttachmentsCreatedByNavigation = new HashSet<Attachments>();
            AttachmentsModifiedByNavigation = new HashSet<Attachments>();
            Departments = new HashSet<Departments>();
            DiscussionsMessageByNavigation = new HashSet<Discussions>();
            DiscussionsUser = new HashSet<Discussions>();
            Divisions = new HashSet<Divisions>();
            DocumentContents = new HashSet<DocumentContents>();
            DocumentHistoryMinutedByNavigation = new HashSet<DocumentHistory>();
            DocumentHistoryRecievedByNavigation = new HashSet<DocumentHistory>();
            DocumentHistoryRouteFromNavigation = new HashSet<DocumentHistory>();
            DocumentHistoryRouteToNavigation = new HashSet<DocumentHistory>();
            DocumentSources = new HashSet<DocumentSources>();
            DocumentTypes = new HashSet<DocumentTypes>();
            Documents = new HashSet<Documents>();
            Minutes = new HashSet<Minutes>();
            OfficeSecretariats = new HashSet<OfficeSecretariats>();
            Offices = new HashSet<Offices>();
            RegistersCreatedByNavigation = new HashSet<Registers>();
            RegistersModifiedByNavigation = new HashSet<Registers>();
            UserSecureAccounts = new HashSet<UserSecureAccounts>();
        }

        public Guid Id { get; set; }
        public string Lpno { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public string PhoneNo { get; set; }
        public Guid? DepartmentId { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public bool? IsActive { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? RankId { get; set; }
        public Guid? UserGroupId { get; set; }
        public bool? IsAccessGranted { get; set; }
        public bool? IsFirstLogin { get; set; }
        public bool? IsLocked { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Gender { get; set; }

        public virtual Departments Department { get; set; }
        public virtual OfficeDesignations Designation { get; set; }
        public virtual OfficeRanks Rank { get; set; }
        public virtual UserGroups UserGroup { get; set; }
        public virtual ICollection<Attachments> AttachmentsCreatedByNavigation { get; set; }
        public virtual ICollection<Attachments> AttachmentsModifiedByNavigation { get; set; }
        public virtual ICollection<Departments> Departments { get; set; }
        public virtual ICollection<Discussions> DiscussionsMessageByNavigation { get; set; }
        public virtual ICollection<Discussions> DiscussionsUser { get; set; }
        public virtual ICollection<Divisions> Divisions { get; set; }
        public virtual ICollection<DocumentContents> DocumentContents { get; set; }
        public virtual ICollection<DocumentHistory> DocumentHistoryMinutedByNavigation { get; set; }
        public virtual ICollection<DocumentHistory> DocumentHistoryRecievedByNavigation { get; set; }
        public virtual ICollection<DocumentHistory> DocumentHistoryRouteFromNavigation { get; set; }
        public virtual ICollection<DocumentHistory> DocumentHistoryRouteToNavigation { get; set; }
        public virtual ICollection<DocumentSources> DocumentSources { get; set; }
        public virtual ICollection<DocumentTypes> DocumentTypes { get; set; }
        public virtual ICollection<Documents> Documents { get; set; }
        public virtual ICollection<Minutes> Minutes { get; set; }
        public virtual ICollection<OfficeSecretariats> OfficeSecretariats { get; set; }
        public virtual ICollection<Offices> Offices { get; set; }
        public virtual ICollection<Registers> RegistersCreatedByNavigation { get; set; }
        public virtual ICollection<Registers> RegistersModifiedByNavigation { get; set; }
        public virtual ICollection<UserSecureAccounts> UserSecureAccounts { get; set; }
    }
}
