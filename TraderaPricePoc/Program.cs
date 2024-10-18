using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Xml.Linq;

// Create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion("Azure OpenAI Deployment Name", "Azure OpenAI Endpoint", "Azure OpenAI Key");

//Alternative using OpenAI
//builder.AddOpenAIChatCompletion(
//         "OpenAI Model name",                  // OpenAI Model name, eg 'gpt-3.5-turbo'
//         "Azure OpenAI Key");     // OpenAI API Key

// Get First Page of Tradera for Xbox 360 and sort into Lowest, Highest and Average Price.
var listOfMaxBids = await PriceSuggestedAsync();
var averagePrice = (int)listOfMaxBids.Average();
var lowestPrice = (int)listOfMaxBids.Min();
var highestPrice = (int)listOfMaxBids.Max();

// Build the kernel
Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Create a history and store the conversation
var history = new ChatHistory();
//Bot should only tell prices if the user asks about Xbox 360 prices.
history.AddSystemMessage($"This information is hidden until the user asks about an Xbox 360. However, the average price they should pay is {averagePrice} SEK, the lowest you can find is {lowestPrice} SEK, and the highest price they should pay is {highestPrice} SEK.");

// Initiate chat
string? userInput;
do
{
    Console.Write("You > ");
    userInput = Console.ReadLine();

    //Add message to history, which is used when fetching response from Model
    history.AddUserMessage(userInput!);

    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        kernel: kernel);

    Console.WriteLine("GptPricePicker > " + result);

    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);

 async Task<List<int>> PriceSuggestedAsync()
{
    //Xbox360 first page
    var httpClient = new HttpClient();
    HttpResponseMessage response = await httpClient.GetAsync("https://api.tradera.com/v3/searchservice.asmx/Search?query=xbox360%20konsol&categoryId=302028&pageNumber=1&orderBy=Relevance&appid=YOURAPPID&appKey=YOURAPPKEY");

    if (response.IsSuccessStatusCode)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var listOfMaxBids = ParseMaxBid(responseContent);
        listOfMaxBids.RemoveAll(x => x < 100 || x > 3000);
        return listOfMaxBids;
    }

    throw new HttpRequestException($"Failed to Get from tradera, method PriceSuggestedAsync {response.StatusCode}");

}

 List<int> ParseMaxBid(string xmlResponse)
{
    // Load the XML string into an XDocument
    XDocument xmlDoc = XDocument.Parse(xmlResponse);

    //MaxBids to get current offerings
    List<int> listOfMaxBids = new();
    var xmlElements = xmlDoc.Descendants();

    listOfMaxBids = xmlElements
        .Skip(1) // Skip the first element, its junk from response
        .Where(element => element.ToString().Contains("MaxBid")) // Filter elements containing "MaxBid"
        .Select(element =>
        {
            XDocument item = XDocument.Parse(element.ToString());
            string maxBid = item.Descendants("{http://api.tradera.com}MaxBid").FirstOrDefault()?.Value!;
            return int.TryParse(maxBid, out int result) ? result : (int?)null;
        })
        .Where(maxBid => maxBid.HasValue) // Filter out null values
        .Select(maxBid => maxBid!.Value)
        .Distinct()
        .ToList();

    return listOfMaxBids;
}