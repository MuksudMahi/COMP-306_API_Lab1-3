using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3MVC.Services
{
    public interface IDynamoDBService
    {
        void CreateDynamoDBTable();
    }
}
