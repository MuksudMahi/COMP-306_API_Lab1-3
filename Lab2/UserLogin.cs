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
    public static class UserLogin
    {
        public static User getUser(String userName, string password)
        {
            IAmazonDynamoDB client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            DynamoDBContext context = new DynamoDBContext(client);
            User user = context.Load<User>(userName, password);
            return user;

        }
    }
}
