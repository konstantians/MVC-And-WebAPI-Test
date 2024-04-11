using Microsoft.EntityFrameworkCore;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.InternalModels.NoSqlModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary;

public class NoSqlAppDbContext : DbContext
{
    public NoSqlAppDbContext(DbContextOptions<NoSqlAppDbContext> options) : base(options)
    {

    }

    public DbSet<NoSqlPostDataModel> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NoSqlPostDataModel>().HasKey("Guid");
        modelBuilder.Entity<NoSqlPostDataModel>().ToContainer("MVCAPITest_Posts").HasPartitionKey(post => post.Guid);

        base.OnModelCreating(modelBuilder);
    }
}
