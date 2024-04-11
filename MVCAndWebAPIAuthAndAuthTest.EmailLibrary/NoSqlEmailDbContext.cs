using Microsoft.EntityFrameworkCore;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.InternalModels.NoSqlModels;

namespace MVCAndWebAPIAuthAndAuthTest.EmailLibrary;

public class NoSqlEmailDbContext : DbContext
{
    public NoSqlEmailDbContext(DbContextOptions<NoSqlEmailDbContext> options) : base(options)
    {
        
    }

    public DbSet<NoSqlEmailModel> Emails { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NoSqlEmailModel>().ToContainer("MVCAPITest_Emails").HasPartitionKey(email => email.Id);

        base.OnModelCreating(modelBuilder);
    }
}
