
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;


namespace Lab3MVC.Models
{
    [DynamoDBTable("Movie")]
    public class Movie
    {
        [DynamoDBHashKey]
        public string MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string Actors { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }

        public S3Link MovieImage { get; set; }

        public S3Link MovieVideo { get; set; }

        public List<Rating> Ratings { get; set; } = new List<Rating>();

    }
}
