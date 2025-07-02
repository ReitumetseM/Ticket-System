using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using OmnitakSupportHub.Models;

namespace OmnitakSupportHub
{
    public class OmnitakContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SupportTeam> SupportTeams { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketTimeline> TicketTimelines { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<KnowledgeBase> KnowledgeBase { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Department> Departments { get; set; }

        public OmnitakContext(DbContextOptions<OmnitakContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1) Explicit primary keys
            modelBuilder.Entity<User>().HasKey(u => u.UserID);
            modelBuilder.Entity<Role>().HasKey(r => r.RoleID);
            modelBuilder.Entity<SupportTeam>().HasKey(st => st.TeamID);
            modelBuilder.Entity<Ticket>().HasKey(t => t.TicketID);
            modelBuilder.Entity<TicketTimeline>().HasKey(tt => tt.TimelineID);
            modelBuilder.Entity<ChatMessage>().HasKey(cm => cm.MessageID);
            modelBuilder.Entity<KnowledgeBase>().HasKey(kb => kb.ArticleID);
            modelBuilder.Entity<Feedback>().HasKey(f => f.FeedbackID);
            modelBuilder.Entity<PasswordReset>().HasKey(pr => pr.Token);
            modelBuilder.Entity<AuditLog>().HasKey(al => al.LogID);

            /// 2) Configure specific relationships BEFORE setting global delete behavior

            // 2a) Self-referencing User relationship (configure first)
            modelBuilder.Entity<User>()
                .HasOne(u => u.ApprovedByUser)
                .WithMany(u => u.ApprovedUsers)
                .HasForeignKey(u => u.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // 2b) TicketTimeline → Ticket (client‐side cascade only)
            modelBuilder.Entity<TicketTimeline>()
                .HasOne(tt => tt.Ticket)
                .WithMany(t => t.TicketTimelines)
                .HasForeignKey(tt => tt.TicketID)
                .OnDelete(DeleteBehavior.ClientCascade);

            // 2c) Other relationships that need special handling
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Team)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TeamID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupportTeam>()
                .HasOne(st => st.TeamLead)
                .WithMany(u => u.LeadTeams)
                .HasForeignKey(st => st.TeamLeadID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Team)
                .WithMany(st => st.Tickets)
                .HasForeignKey(t => t.TeamID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Ticket)
                .WithMany(t => t.ChatMessages)
                .HasForeignKey(cm => cm.TicketID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(cm => cm.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KnowledgeBase>()
                .HasOne(kb => kb.CreatedByUser)
                .WithMany(u => u.CreatedArticles)
                .HasForeignKey(kb => kb.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KnowledgeBase>()
                .HasOne(kb => kb.LastUpdatedByUser)
                .WithMany(u => u.UpdatedArticles)
                .HasForeignKey(kb => kb.LastUpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Ticket)
                .WithMany(t => t.Feedbacks)
                .HasForeignKey(f => f.TicketID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PasswordReset>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.PasswordResets)
                .HasForeignKey(pr => pr.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // 3) NOW set global delete behavior for any remaining relationships
            foreach (var fk in modelBuilder.Model
                                        .GetEntityTypes()
                                        .SelectMany(e => e.GetForeignKeys())
                                        .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // 4) Indexes, seed & base call
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<SupportTeam>()
                .HasIndex(st => st.TeamName)
                .IsUnique();

            SeedData(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Roles with enhanced permissions for approval workflow
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleID = 1,
                    RoleName = "Administrator",
                    Description = "Full system access including user approval and system management",
                    CanApproveUsers = true,
                    CanManageTickets = true,
                    CanViewAllTickets = true,
                    CanManageKnowledgeBase = true,
                    CanViewReports = true,
                    CanManageTeams = true,
                    IsSystemRole = true
                },
                new Role
                {
                    RoleID = 2,
                    RoleName = "Support Manager",
                    Description = "Can manage support teams and approve users",
                    CanApproveUsers = true,
                    CanManageTickets = true,
                    CanViewAllTickets = true,
                    CanManageKnowledgeBase = true,
                    CanViewReports = true,
                    CanManageTeams = true,
                    IsSystemRole = true
                },
                new Role
                {
                    RoleID = 3,
                    RoleName = "Support Agent",
                    Description = "Can manage and resolve assigned tickets",
                    CanApproveUsers = false,
                    CanManageTickets = true,
                    CanViewAllTickets = true,
                    CanManageKnowledgeBase = true,
                    CanViewReports = false,
                    CanManageTeams = false,
                    IsSystemRole = true
                },
                new Role
                {
                    RoleID = 4,
                    RoleName = "End User",
                    Description = "Can create and view own tickets",
                    CanApproveUsers = false,
                    CanManageTickets = false,
                    CanViewAllTickets = false,
                    CanManageKnowledgeBase = false,
                    CanViewReports = false,
                    CanManageTeams = false,
                    IsSystemRole = true
                }
            );

            // Seed Support Teams
            modelBuilder.Entity<SupportTeam>().HasData(
                new SupportTeam
                {
                    TeamID = 1,
                    TeamName = "IT Support",
                    Description = "General IT support and troubleshooting",
                    Specialization = "Hardware and Software Support"
                },
                new SupportTeam
                {
                    TeamID = 2,
                    TeamName = "Network Team",
                    Description = "Network infrastructure and connectivity",
                    Specialization = "Network Infrastructure"
                },
                new SupportTeam
                {
                    TeamID = 3,
                    TeamName = "Security Team",
                    Description = "Information security and compliance",
                    Specialization = "Cybersecurity"
                },
                new SupportTeam
                {
                    TeamID = 4,
                    TeamName = "Application Support",
                    Description = "Business application support and maintenance",
                    Specialization = "Software Applications"
                }
            );

            // Seed Admin User - WITHOUT self-reference to avoid circular dependency
            string adminPasswordHash = HashPassword("SuperSecure!2025");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    Email = "admin@omnitak.com",
                    PasswordHash = adminPasswordHash,
                    HashAlgorithm = "SHA256",
                    FullName = "System Administrator",
                    DepartmentId = 1,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsApproved = true,
                    IsActive = true,
                    RoleID = 1,
                    TeamID = 1,
                    ApprovedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ApprovedBy = null // Set to null to avoid self-reference during seeding
                }
            );
                modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, Name = "IT", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { DepartmentId = 2, Name = "HR", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { DepartmentId = 3, Name = "Finance", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { DepartmentId = 4, Name = "Operations", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { DepartmentId = 5, Name = "Marketing", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { DepartmentId = 6, Name = "Sales", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Changed to match AuthService: salt + password
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes("OmnitakSalt2024" + password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}