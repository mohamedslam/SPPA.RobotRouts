using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SPPA.Domain;

namespace SPPA.Database;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private readonly string _connectionString;

    public DesignTimeDbContextFactory()
    {
        _connectionString = new ConfigurationBuilder()
                   .SetBasePath(GetWebProjectPath())
                   .AddJsonFile("appsettings.json")
                   .AddJsonFile("appsettings.Development.json", optional: true)
                   .Build()
                   .Get<AppSettings>().Database!.Connection!;
    }

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var efOptions = new DbContextOptionsBuilder<ApplicationDbContext>();
        efOptions.UseNpgsql(_connectionString, npgOptions =>
        {
            //npgOptions.UseNodaTime();
        });

        //var logger = LoggerFactory.Create(options => options.AddConsole())
        //                          .CreateLogger<ApplicationDbContext>();

        return new ApplicationDbContext(efOptions.Options);
    }

    private static string GetWebProjectPath() =>
        Path.Combine(Directory.GetCurrentDirectory(), "../SPPA.Web");
}

