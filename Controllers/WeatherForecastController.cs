using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace WebApplication1.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        CosmosClient client = new CosmosClient("");
        Database database = await client.CreateDatabaseIfNotExistsAsync("MyDatabaseName");
        Container container = await database.CreateContainerIfNotExistsAsync(
            "MyContainerName",
            "/partitionKeyPath");

        // Create an item
        dynamic testItem = new { id = "MyTestItemId", partitionKeyPath = "MyTestPkValue", details = "it's working", status = "done" };
        ItemResponse<dynamic> createResponse = await container.CreateItemAsync(testItem);

        // Query for an item
        using (FeedIterator<dynamic> feedIterator = container.GetItemQueryIterator<dynamic>(
            "select * from T where T.status = 'done'"))
        {
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<dynamic> response = await feedIterator.ReadNextAsync();
                foreach (var item in response)
                {
                    Console.WriteLine(item);
                }
            }
        }

        Console.WriteLine("Hello, World!");

        return Ok();
    }
}
