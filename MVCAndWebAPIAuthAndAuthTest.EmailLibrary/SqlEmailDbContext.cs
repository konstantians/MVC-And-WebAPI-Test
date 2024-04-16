using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.InternalModels.SqlModels;

namespace MVCAndWebAPIAuthAndAuthTest.EmailLibrary;

public class SqlEmailDbContext : DbContext
{
    private readonly IConfiguration? _configuration;

    //used for migrations
    public SqlEmailDbContext()
    {
        
    }

    public SqlEmailDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //this is used for migrations, because the configuration can not be instantiated without the application running
        if (_configuration is null)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=D:\\Visual Studio\\LocalDbDatabases\\MVCAndWebAPIEmailDb.mdf;Initial Catalog=MVCAndWebAPIEmailDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                options => options.EnableRetryOnFailure());
            return;
        }

        //otherwise if the application runs check if it uses cosmos or sql server
        if (_configuration["DatabaseInUse"] is null || _configuration["DatabaseInUse"] != "SqlServer")
            throw new ArgumentException("The configuration for SQL is not valid");

        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SqlData"),
            options => options.EnableRetryOnFailure());
    }

    public DbSet<SqlEmailModel> Emails { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SqlEmailModel>().HasKey("Id");

        base.OnModelCreating(modelBuilder);
    }
}


