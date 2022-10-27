using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;
using Microsoft.Azure.Cosmos.Table.Queryable;
namespace Bitcoin.swa
{
    public static class RatesApi
    {
        [FunctionName("RatesApi")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, [Table("RatesTable", "Bitcoin", Connection = "RatesConnection")] 
            CloudTable table,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var query = table.CreateQuery<TableData>();
            
            

            var latestRates = (await table.ExecuteQuerySegmentedAsync(query, null)).ToList();
            
            if (latestRates.Any()){
                float bcfloat = (float)latestRates.First().BitCoinRate/100;
                float sekfloat = (float)latestRates.First().SEKRate/10000;
                var result = new ApiResponse(bcfloat, sekfloat, bcfloat * sekfloat);
                return new OkObjectResult(result);
            }
            return new OkResult();
            
        }
    }
        public record ApiResponse(float BCRate, float SEKRate, float SEKperBC);

    public class TableData : TableEntity
    {
        public int BitCoinRate {get; set;}
        public int SEKRate {get; set;}
        
        
    }
}
