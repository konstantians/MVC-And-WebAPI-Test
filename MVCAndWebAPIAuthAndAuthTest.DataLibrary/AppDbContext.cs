using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCAndWebAPIAuthAndAuthTest.SharedModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    //used for migrations
    public AppDbContext()
    {

    }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //if the application runs use this
        if (_configuration != null)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultData"),
                options => options.EnableRetryOnFailure());
        }
        //otherwise this is used for migrations, because the configuration can not be instantiated without the application running
        else
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=MVCAndWebAPIDataDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False",
                options => options.EnableRetryOnFailure());
        }
    }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().HasKey(post => post.Id);

        base.OnModelCreating(modelBuilder);
    }
}
