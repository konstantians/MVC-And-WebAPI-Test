using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SocialMediaApp.Authentication;

public class AppIdentityDbContext : IdentityDbContext
{
    private readonly IConfiguration _configuration;

    //used for migrations
    public AppIdentityDbContext()
    {

    }

    //used when the application is running
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //if the application runs use this
        if (_configuration != null)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultAuthentication"),
                options => options.EnableRetryOnFailure());
        }
        //otherwise this is used for migrations, because the configuration can not be instantiated without the application running
        else
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=MVCAndWebAPIAuthDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False",
                options => options.EnableRetryOnFailure());
        }
    }
}