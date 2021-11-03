using Lab3MVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3MVC.Controllers
{
    [Produces("application/json")]
    [Route("api/DynamoDB")]
    [ApiController]
    public class DynamoDBController : ControllerBase
    {
        private readonly IDynamoDBService _dynamoDBService;
        public DynamoDBController(IDynamoDBService dynamoDBService)
        {
            _dynamoDBService = dynamoDBService;
        }

        [Route("createtable")]
        public IActionResult CreateDynamoDBTable()
        {
            _dynamoDBService.CreateDynamoDBTable();
            return Ok()
;        }
    }
}
