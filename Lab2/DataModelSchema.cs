using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey]
        public string Name { get; set; }
        [DynamoDBRangeKey(AttributeName = "Password")]
        public string Password { get; set; }
    }

    [DynamoDBTable("BookShelf")]
    public class BookShelf
    {
        [DynamoDBHashKey]
        public string BookName { get; set; }
        [DynamoDBRangeKey]
        public string UserName { get; set; }
        public int Bookmark { get; set; }
        public string Author { get; set; }
        public DateTime Accessed { get; set; }
        public string Key { get; set; }
    }
}
