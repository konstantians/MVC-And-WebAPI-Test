using Microsoft.Azure.Cosmos;
using Microsoft.Data.SqlClient;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Tests.IntegrationTests.HelperMethods;
internal class ResetDatabaseHelperMethods
{
    public static async Task ResetNoSqlPostDatabase()
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

        Console.WriteLine("All documents deleted from NoSql post database.");
    }

    public static void ResetSqlPostDatabase()
    {
        string connectionString = Environment.GetEnvironmentVariable("SqlData")!;
        string[] tables = new string[] { "dbo.Posts" };

        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        foreach (string table in tables)
        {
            string deleteQuery = $"DELETE FROM {table}";
            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                deleteCommand.ExecuteNonQuery();
        }

        connection.Close();

        Console.WriteLine("All documents deleted from Sql post database.");
    }


}
