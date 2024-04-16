using Microsoft.Azure.Cosmos;
using Microsoft.Data.SqlClient;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices;

public static class ResetDatabaseService
{
    public static async Task DefaultResetDatabaseActions()
    {
        //The authentication database is always sql server
        ResetSqlAuthenticationDatabase();

        string dataDatabaseMode = Environment.GetEnvironmentVariable("DataDatabaseInUse")!;
        if (dataDatabaseMode == "SqlServer")
            ResetSqlDataDatabase();
        else
            await ResetNoSqlDataDatabase();

        string emailDatabaseMode = Environment.GetEnvironmentVariable("EmailDatabaseInUse")!;
        if (emailDatabaseMode == "SqlServer")
            ResetSqlEmailDatabase();
        else
            await ResetNoSqlEmailDatabase();
    }

    public static void ResetSqlEmailDatabase()
    {
        ResetSqlDatabaseHelper(environmentConnectionString: "SqlEmail",
            tables: new string[] { "dbo.Emails" },
            consoleMessage: "All documents deleted from Sql email database.");
    }

    public static void ResetSqlDataDatabase()
    {
        ResetSqlDatabaseHelper(environmentConnectionString: "SqlData",
            tables: new string[] { "dbo.Posts" },
            consoleMessage: "All documents deleted from Sql data database.");
    }

    public static void ResetSqlAuthenticationDatabase()
    {
        ResetSqlDatabaseHelper(environmentConnectionString: "SqlAuthentication", 
            tables: new string[] { "dbo.AspNetRoleClaims", "dbo.AspNetUserRoles", "dbo.AspNetUserClaims",
            "dbo.AspNetUserLogins", "dbo.AspNetRoles", "dbo.AspNetUsers", "dbo.AspNetUserTokens"}, 
            consoleMessage: "All documents deleted from Sql authentication database.");
    }

    private static void ResetSqlDatabaseHelper(string environmentConnectionString, string[] tables, string consoleMessage)
    {
        string connectionString = Environment.GetEnvironmentVariable(environmentConnectionString)!;

        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        foreach (string table in tables)
        {
            string deleteQuery = $"DELETE FROM {table}";
            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                deleteCommand.ExecuteNonQuery();
        }

        connection.Close();

        Console.WriteLine(consoleMessage);
    }

    public static async Task ResetNoSqlDataDatabase()
    {
        string cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString")!;

        CosmosClient cosmosClient = new CosmosClient(cosmosDbConnectionString);
        Database database = cosmosClient.GetDatabase("GlobalDb");
        Container container = database.GetContainer("MVCAPITest_Posts");

        //all the documents of the container
        FeedIterator<dynamic> resultSetIterator = container.GetItemQueryIterator<dynamic>("SELECT * FROM c");

        while (resultSetIterator.HasMoreResults)
        {
            //a batch of the documents that are loaded from resultSetIterator
            FeedResponse<dynamic> response = await resultSetIterator.ReadNextAsync();

            foreach (var document in response)
            {   
                await container.DeleteItemAsync<dynamic>(id: document.id.ToString(), 
                    partitionKey: new PartitionKey(document.Guid.ToString()));
            }
        }

        Console.WriteLine("All documents deleted from NoSql data database.");
    }

    public static async Task ResetNoSqlEmailDatabase()
    {
        string cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString")!;

        CosmosClient cosmosClient = new CosmosClient(cosmosDbConnectionString);
        Database database = cosmosClient.GetDatabase("GlobalDb");
        Container container = database.GetContainer("MVCAPITest_Emails");

        //all the documents of the container
        FeedIterator<dynamic> resultSetIterator = container.GetItemQueryIterator<dynamic>("SELECT * FROM c");

        while (resultSetIterator.HasMoreResults)
        {
            //a batch of the documents that are loaded from resultSetIterator
            FeedResponse<dynamic> response = await resultSetIterator.ReadNextAsync();

            foreach (var document in response)
            {
                await container.DeleteItemAsync<dynamic>(id: document.id.ToString(),
                    partitionKey: new PartitionKey(document.Id.ToString()));
            }
        }

        Console.WriteLine("All documents deleted from NoSql email database.");
    }
}
