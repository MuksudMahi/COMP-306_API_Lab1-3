using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab3MVC.Services
{
    public class DynamoDBService : IDynamoDBService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private static readonly string tableName="TempTable";
        public DynamoDBService(IAmazonDynamoDB dynamoDBClient)
        {
            _dynamoDBClient = dynamoDBClient;
        }

        public void CreateDynamoDBTable()
        {
            try
            {
                CreateTable();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void CreateTable()
        {
            var request = new CreateTableRequest()
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "N"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "ReplyDateTime",
                        AttributeType = "N"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "ReplyDateTime",
                        KeyType = "Range"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                },
                TableName = tableName
            };
            var response = _dynamoDBClient.CreateTableAsync(request);

            WaitUntilTableIsReady(tableName);
        }

        private void WaitUntilTableIsReady(string tableName)
        {
            string status = null;
            do
            {
                Thread.Sleep(5000);
                try
                {
                    var res = _dynamoDBClient.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName=tableName
                    });
                    status = res.Result.Table.TableStatus;
                }
                catch (ResourceNotFoundException e)
                {

                    Console.WriteLine(e.Message);
                }
            } while (status!="ACTIVE");
            Console.WriteLine("Table Created");
        }
    }
}
