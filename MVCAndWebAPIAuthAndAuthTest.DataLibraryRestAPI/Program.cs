using Microsoft.EntityFrameworkCore;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Logic;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        IConfiguration configuration = builder.Configuration;

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        if (configuration["DatabaseInUse"] is null || configuration["DatabaseInUse"] == "SqlServer")
        {
            builder.Services.AddDbContext<SqlAppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlData"))
            );
        
            builder.Services.AddScoped<IPostDataAccess, SqlPostDataAccess>();
        }
        else
        {
            builder.Services.AddCosmos<NoSqlAppDbContext>(connectionString: configuration["CosmosDbConnectionString"]!,
                databaseName: "GlobalDb");

            builder.Services.AddScoped<IPostDataAccess, NoSqlPostDataAccess>();
        }
        
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("https://localhost:7189")
                       .AllowAnyHeader()
                       .AllowAnyMethod();

                builder.WithOrigins("https://apipartoftest.azurewebsites.net/")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}