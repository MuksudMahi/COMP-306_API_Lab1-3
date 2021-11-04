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
        [Route("putitem")]
        public IActionResult AddNewEntry([FromQuery] int id, string replyDateTime, double price)
        {
            _dynamoDBService.AddNewEntry(id, replyDateTime, price);
            return Ok();
        }
        [Route("getitems")]
        public async Task<IActionResult> GetItems([FromQuery] int? id)
        {
            var response = await _dynamoDBService.GetItems(id);
            return Ok(response);
        }
        [HttpPut]
        [Route("updateitem")]
        public async Task<IActionResult> UpdateItem([FromQuery] int id, double price)
        {
            var response = await _dynamoDBService.UpdateItem(id, price);
            return Ok(response);
        }
    }
}
