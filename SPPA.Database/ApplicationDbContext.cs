using Microsoft.EntityFrameworkCore;
using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.Orders.Files;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Entities.WorkSpaces;
using System.Reflection;
namespace SPPA.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Workspace> Workspaces { get; set; }


    #region Order
    public DbSet<Order> Orders { get; set; }

    #region Order files
    public DbSet<OrderFile> OrderFiles { get; set; }
    public DbSet<OrderFileElementRef> OrderFileElementRefs { get; set; }
    public DbSet<OrderFileAttribute> OrderFileAttributes { get; set; }
    public DbSet<OrderFileAttributeValue> OrderFileAttributeValues { get; set; }
    #endregion

    #endregion

    #region Users
    public DbSet<User> Users { get; set; }
    public DbSet<UserInvite> Invites { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    #endregion

    //private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
        Database.SetCommandTimeout(60 * 5);
        //_logger = this.GetService<ILogger<ApplicationDbContext>>(); ;
        // TODO
        //this.UseTimestamps();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<Enum>(x => x.HaveConversion<string>());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(this.GetType()));
    }

}
