using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public static class AddUsers
    {
        public static IAmazonDynamoDB client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        public static void RunDataModelSample()
        {
            DynamoDBContext context = new DynamoDBContext(client);
            Console.WriteLine("Creating movie");
            User user1 = new User
            {
                Name = "Admin",
                Password="admin"
            };
            context.Save(user1);
        }

    }
}
