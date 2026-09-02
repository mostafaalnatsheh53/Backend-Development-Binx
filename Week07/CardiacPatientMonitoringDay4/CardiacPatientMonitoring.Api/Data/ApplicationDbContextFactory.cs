using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CardiacPatientMonitoring.Api.Data;

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"))
            .Options;

        return new ApplicationDbContext(options);
    }
}