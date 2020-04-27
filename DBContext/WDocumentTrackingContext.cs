using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DocumentTracking.API.Entities;
using Microsoft.Extensions.Configuration;

namespace DocumentTracking.API.DBContext
{
    public partial class WDocumentTrackingContext : DbContext
    {
        public WDocumentTrackingContext()
        {
        }

        public WDocumentTrackingContext(DbContextOptions<WDocumentTrackingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuditTrail> AuditTrail { get; set; }
        
        public virtual DbSet<UserGroups> UserGroups { get; set; }
        public virtual DbSet<UserSecureAccounts> UserSecureAccounts { get; set; }
        public virtual DbSet<UserSecurityQuestions> UserSecurityQuestions { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        

        public IConfiguration Configuration { get; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                
                var connection = Configuration.GetConnectionString("DocTrackingConnection");
                optionsBuilder.UseSqlServer(connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<AuditTrail>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserGroups>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.GroupTitle).HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                entity.Property(e => e.IsDefault)
                    .HasColumnName("isDefault")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsSuperAdmin).HasColumnName("isSuperAdmin");
            });

            modelBuilder.Entity<UserSecureAccounts>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.SecurityQuestionId).HasColumnName("SecurityQuestionID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.SecurityQuestion)
                    .WithMany(p => p.UserSecureAccounts)
                    .HasForeignKey(d => d.SecurityQuestionId)
                    .HasConstraintName("FK_UserSecureAccounts_UserSecurityQuestions");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSecureAccounts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserSecureAccounts_Users");
            });

            modelBuilder.Entity<UserSecurityQuestions>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.SecurityQuestion).HasMaxLength(300);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.DesignationId).HasColumnName("DesignationID");

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.Gender).HasMaxLength(7);

                entity.Property(e => e.IsAccessGranted)
                    .HasColumnName("isAccessGranted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsFirstLogin)
                    .HasColumnName("isFirstLogin")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsLocked)
                    .HasColumnName("isLocked")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Lpno)
                    .HasColumnName("LPNo")
                    .HasMaxLength(20);

                entity.Property(e => e.OtherNames).HasMaxLength(150);

                entity.Property(e => e.PhoneNo).HasMaxLength(50);

                entity.Property(e => e.RankId).HasColumnName("RankID");

                entity.Property(e => e.Surname).HasMaxLength(50);

                entity.Property(e => e.UserGroupId).HasColumnName("UserGroupID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Users_Departments");

                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DesignationId)
                    .HasConstraintName("FK_Users_OfficeDesignations");

                entity.HasOne(d => d.Rank)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RankId)
                    .HasConstraintName("FK_Users_OfficeRanks");

                entity.HasOne(d => d.UserGroup)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserGroupId)
                    .HasConstraintName("FK_Users_UserGroups");
            });

            

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
