using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.InternalModels.SqlModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary;

public class SqlAppDbContext : DbContext
{
    private readonly IConfiguration? _configuration;

    //used for migrations
    public SqlAppDbContext()
    {

    }

    public SqlAppDbContext(DbContextOptions<SqlAppDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //this is used for migrations, because the configuration can not be instantiated without the application running
        if (_configuration is null)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=D:\\Visual Studio\\LocalDbDatabases\\MVCAndWebAPIDataDb.mdf;Initial Catalog=MVCAndWebAPIDataDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                options => options.EnableRetryOnFailure());
            return;
        }

        //otherwise if the application runs check if it uses cosmos or sql server
        if (_configuration["DatabaseInUse"] is null || _configuration["DatabaseInUse"] != "SqlServer")
            throw new ArgumentException("The configuration for SQL is not valid");

        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SqlData"),
            options => options.EnableRetryOnFailure());
    }

    internal DbSet<SqlPostDataModel> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SqlPostDataModel>().HasKey(post => post.Guid);

        base.OnModelCreating(modelBuilder);
    }
}
