namespace OmnitakITSupportMVC
{
    using Microsoft.EntityFrameworkCore;

    public class OmnitakContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SupportTeam> SupportTeams { get; set; }

        public OmnitakContext(DbContextOptions<OmnitakContext> options) : base(options)
        {
        }

        // Remove or comment out OnConfiguring if using DI
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlServer("name=ConnectionStrings:DefaultConnection");
        // }
    }

    public class User { public int UserID { get; set; } public string FullName { get; set; } }
    public class Ticket { public int TicketID { get; set; } public string Title { get; set; } }
    public class Role { public int RoleID { get; set; } public string RoleName { get; set; } }
    public class SupportTeam { public int TeamID { get; set; } public string TeamName { get; set; } }
}
