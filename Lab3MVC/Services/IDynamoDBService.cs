using Lab3MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3MVC.Services
{
    public interface IDynamoDBService
    {
        void CreateDynamoDBTable();
        Task AddNewEntry(int id, string replyDateTime, double price);
        Task<DynamoDBTableItems> GetItems(int? id);
        Task<Item> UpdateItem(int id, double price);
    }
}
