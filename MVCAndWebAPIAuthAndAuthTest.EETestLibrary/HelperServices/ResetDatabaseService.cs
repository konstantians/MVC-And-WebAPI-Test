using Microsoft.Azure.Cosmos;
using Microsoft.Data.SqlClient;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices;

public static class ResetDatabaseService
{
    public static void ResetSqlDataDatabase()
    {
        string dataConnectionString = Environment.GetEnvironmentVariable("SqlData")!;

        using SqlConnection dataConnection = new SqlConnection(dataConnectionString);
        dataConnection.Open();

        string[] dataTableNames = new string[] { "dbo.Posts" };

        foreach (string tableName in dataTableNames)
        {
            string deleteQuery = $"DELETE FROM {tableName}";
            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, dataConnection))
                deleteCommand.ExecuteNonQuery();
        }

        dataConnection.Close();

        Console.WriteLine("All documents deleted from Sql data database.");
    }

    public static void ResetSqlAuthenticationDatabase()
    {
        string authConnectionString = Environment.GetEnvironmentVariable("SqlAuthentication")!;

        using SqlConnection connection = new SqlConnection(authConnectionString);
        connection.Open();

        string[] tableNames = new string[] { "dbo.AspNetRoleClaims", "dbo.AspNetUserRoles", "dbo.AspNetUserClaims",
            "dbo.AspNetUserLogins", "dbo.AspNetRoles", "dbo.AspNetUsers", "dbo.AspNetUserTokens"};

        foreach (string tableName in tableNames)
        {
            string deleteQuery = $"DELETE FROM {tableName}";
            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                deleteCommand.ExecuteNonQuery();
        }

        connection.Close();

        Console.WriteLine("All documents deleted from Sql authentication database.");
    }

    public static async Task ResetNoSqlDatabase()
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

        Console.WriteLine("All documents deleted from NoSql database.");
    }
}
