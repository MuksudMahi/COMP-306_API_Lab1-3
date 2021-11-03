using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3MVC.Models
{
    [DynamoDBTable("Users")]
    public class Movie
    {
        [DynamoDBHashKey(AttributeName = "name")]

        public string Name { get; set; }
    }
}
