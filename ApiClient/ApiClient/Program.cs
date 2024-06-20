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
