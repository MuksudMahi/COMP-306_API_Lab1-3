using Amazon.DynamoDBv2.DataModel;


namespace Lab3MVC.Models
{
    [DynamoDBTable("User")]
    public class Users
    {
        [DynamoDBHashKey]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
