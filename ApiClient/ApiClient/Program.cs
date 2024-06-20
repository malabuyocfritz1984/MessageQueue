using System.Diagnostics;
using System.Text;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static readonly string apiUrl = "https://localhost:5001/request";
    private static readonly int totalRequests = 5000;
    private static readonly int maxDegreeOfParallelism = 50;

    static async Task Main(string[] args)
    {
        var tasks = new List<Task>();

        //Dont use semaphoe slim here use Parallels.Foreach with a max maxDegreeOfParallelism
        using (var semaphore = new SemaphoreSlim(maxDegreeOfParallelism))
        {
            for (int i = 0; i < totalRequests; i++)
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var stopwatch = Stopwatch.StartNew();
                        var requestId = await MakeRequest(i);
                        stopwatch.Stop();
                        Console.WriteLine($"Request {i} took {stopwatch.ElapsedMilliseconds} ms, Request ID: {requestId}");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }
            await Task.WhenAll(tasks);
        }
    }

    static async Task<string> MakeRequest(int i)
    {
        var payload = $"Request {i}";
        var content = new StringContent($"\"{payload}\"", Encoding.UTF8, "application/json");
        var response = await client.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}


/*
 1. Creat a new Class with interface "ApiClient" this will use the httpClient and call the api we created
 2. Inject the ApiClient in the program class using AddHttpClient
 3. Convert the Program to use dependency injection
 */
