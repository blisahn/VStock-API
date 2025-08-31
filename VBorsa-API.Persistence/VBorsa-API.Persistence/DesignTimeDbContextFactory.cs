using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VBorsa_API.Persistence.Context;

namespace VBorsa_API.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<VBorsaDbContext>
{
    private readonly IConfiguration _configuration;

    public DesignTimeDbContextFactory()
    {
    }

    public DesignTimeDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public VBorsaDbContext CreateDbContext(string[] args)
    {
        // var appSettingsFullPath =
        //   _configuration.GetConnectionString("DefaultConnection");

        // IConfiguration configuration = new ConfigurationBuilder()
        //     .AddJsonFile(appSettingsFullPath)
        //     .Build();
        //
        // var connectionString = _configuration.GetConnectionString("DefaultConnection");
        // Console.WriteLine(connectionString);
        var optionsBuilder = new DbContextOptionsBuilder<VBorsaDbContext>();
        // Console.WriteLine("Connection string: "+_configuration.GetConnectionString("DefaultConnection"));
        optionsBuilder.UseSqlServer(
            "Server=AMOS\\SQLEXPRESS;Database=VBorsaDB;User ID=amos;Password=Amos123!;TrustServerCertificate=true");

        return new VBorsaDbContext(optionsBuilder.Options);
    }
}