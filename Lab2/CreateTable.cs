using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2
{
    public static class CreateTable
    {
        public static IAmazonDynamoDB client =new AmazonDynamoDBClient(RegionEndpoint.USEast1);

        public static void createTable()
        {
            List<string> currentTables = client.ListTables().TableNames;
            if (!currentTables.Contains("Users"))
            {
                CreateTableRequest createRequest = new CreateTableRequest
                {
                    TableName = "Users",
                    AttributeDefinitions = new List<AttributeDefinition>()
                    {
                    new AttributeDefinition
                    {
                        AttributeName = "Name",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Password",
                        AttributeType = "S"
                    }
                },
                    KeySchema = new List<KeySchemaElement>()
                    {
                    new KeySchemaElement
                    {
                        AttributeName = "Name",
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "Password",
                        KeyType = "RANGE"
                    }
                },
                };

                createRequest.ProvisionedThroughput = new ProvisionedThroughput(1, 1);

                CreateTableResponse createResponse;
                try
                {
                    createResponse = client.CreateTable(createRequest);

                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error: failed to create the new table; " + ex.Message);
                    return;
                }
            }
        }
    }
}
