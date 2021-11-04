using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Lab3MVC.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab3MVC.Services
{
    public class DynamoDBService : IDynamoDBService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private static readonly string tableName="TempTableNew";
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

        public async Task AddNewEntry(int id, string replyDateTime, double price)
        {
            var queryRequest = RequestBuilder(id, replyDateTime, price);
            await PutItemAsync(queryRequest);
        }

  
        private PutItemRequest RequestBuilder(int id, string replyDateTime, double price)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue{N=id.ToString()}},
                {"ReplyDateTime", new AttributeValue{N=replyDateTime}},
                {"Price", new AttributeValue{N=price.ToString(CultureInfo.InvariantCulture)}}
            };
            return new PutItemRequest
            {
                TableName = tableName,
                Item = item
            };
        }

        private async Task PutItemAsync(PutItemRequest queryRequest)
        {
            await _dynamoDBClient.PutItemAsync(queryRequest);
        }

        public async Task<DynamoDBTableItems> GetItems(int? id)
        {
            var queryRequest = RequestBuilder(id);
            var result = await ScanAsync(queryRequest);

            return new DynamoDBTableItems
            {
                Items = result.Items.Select(Map).ToList()
            };
        }

        private Item Map(Dictionary<string, AttributeValue> result)
        {
            return new Item
            {
                Id = Convert.ToInt32(result["Id"].N),
                ReplyDateTime = result["ReplyDateTime"].N,
                Price = Convert.ToDouble(result["Price"].N)
            };
        }

        private async Task<ScanResponse> ScanAsync(ScanRequest queryRequest)
        {
            var response = await _dynamoDBClient.ScanAsync(queryRequest);
            return response;
        }

        private ScanRequest RequestBuilder(int? id)
        {
            if (id.HasValue == false)
            {
                return new ScanRequest
                {
                    TableName = tableName
                };
            }
            else
            {
                return new ScanRequest
                {
                    TableName = tableName,
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {
                            ":v_id", new AttributeValue {N=id.ToString() }
                        }
                    },
                    FilterExpression = "Id= :v_id",
                    ProjectionExpression = "Id, ReplyDateTime, Price"
                };
            }
        }

        public async Task<Item> UpdateItem(int id, double price)
        {
            var response = await GetItems(id);
            var currprice = response.Items.Select(i => i.Price).FirstOrDefault();
            var replyDateTime = response.Items.Select(d => d.ReplyDateTime).FirstOrDefault();
            var request = UpdateRequestBuilder(id, price, currprice, replyDateTime);
            var result = await UpdateItemAsync(request);
            return new Item
            {
                Id = Convert.ToInt32(result.Attributes["Id"].N),
                ReplyDateTime = result.Attributes["ReplyDateTime"].N,
                Price = Convert.ToDouble(result.Attributes["Price"].N)
            };
        }

        private async Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request)
        {
            var response = await _dynamoDBClient.UpdateItemAsync(request);
            return response;
        }

        private UpdateItemRequest UpdateRequestBuilder(int id, double price, double currprice, string replyDateTime)
        {
            var request = new UpdateItemRequest
            {
                Key = new Dictionary<string, AttributeValue>
                {
                    {
                        "Id", new AttributeValue
                        {
                            N = id.ToString()
                        }
                    },
                    {
                        "ReplyDateTime", new AttributeValue
                        {
                            N = replyDateTime
                        }
                    }
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {
                        "#r", "Price"
                    },

                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":newprice", new AttributeValue
                        {
                            N=price.ToString()
                        }

                    },
                    {
                        ":currprice", new AttributeValue
                        {
                            N=currprice.ToString()
                        }
                    }

                },
                UpdateExpression = "SET #r = :newprice",
                ConditionExpression = "#r = :currprice",
                TableName=tableName,
                ReturnValues="ALL_NEW"
            };
            return request;
        }
    }
}
