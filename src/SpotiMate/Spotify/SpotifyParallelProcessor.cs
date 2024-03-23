using Flurl.Http;
using SpotiMate.Cli;
using SpotiMate.Spotify.Objects;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify;

public class SpotifyParallelProcessor
{
    private const int MaxRetries = 5;
    
    public async Task<IReadOnlyCollection<TItem>> GetAll<TResponse, TItem>(Func<IFlurlRequest> requestBuilder, int limit) 
        where TResponse : ISpotifyPageResponse<TItem>
        where TItem : ISpotifyObject
    {
        var initResponse = await WaitRequest(MakePageRequest<TResponse>(requestBuilder(), limit, 0));
        
        var total = initResponse.Total;
        var initItems = initResponse.Items;

        var remainingRequestsCount = (int)Math.Ceiling((total - initItems.Length) / (double)limit);
        var tasks = new List<Task<TResponse>>();
        
        for (var i = 0; i < remainingRequestsCount; i++)
        {
            tasks.Add(WaitRequest(
                MakePageRequest<TResponse>(requestBuilder(), limit, initItems.Length + i * limit)));
        }
        
        var responses = await Task.WhenAll(tasks);
        var results = new List<TItem>(total);
  
        results.AddRange(initItems);
        
        foreach (var response in responses)
        {
            results.AddRange(response.Items);
        }

        return results;
    }

    private static Task<TResponse> MakePageRequest<TResponse>(IFlurlRequest request, int limit, int offset)
    {
        return request
            .SetQueryParams(new { limit, offset })
            .GetJsonAsync<TResponse>();
    }

    private static async Task<TResponse> WaitRequest<TResponse>(Task<TResponse> requestTask)
    {
        var retries = 0;
        
        while (true)
        {
            try
            {
                return await requestTask;
            }
            catch (FlurlHttpException ex)
            {
                switch (ex.StatusCode)
                {
                    case 429:
                    {
                        if (retries++ >= MaxRetries)
                        {
                            throw new SpotifyClientException("Request limit exceeded. Too many retries. Giving up.");
                        }
                        
                        if (ex.Call.Response.Headers.TryGetFirst("Retry-After", out var ra))
                        {
                            var delay = int.Parse(ra) + 1;
                            await Task.Delay(delay * 1000);
                        }
                        else
                        {
                            CliPrint.PrintWarning("No Retry-After header found. Waiting for 10 seconds.");
                            await Task.Delay(10000);
                        }

                        continue;
                    }
                    
                    case 500:
                    {
                        if (retries++ >= MaxRetries)
                        {
                            throw new SpotifyClientException("Internal server error. Too many retries. Giving up.");
                        }
                        
                        await Task.Delay(1000);
                        
                        continue;
                    }
                    
                    default:
                        throw;
                }
            }
        }
    }

    public async Task<bool> ProcessAll<TItem>(
        Func<IEnumerable<TItem>, Task<IFlurlResponse>> requestMaker,
        IEnumerable<TItem> items,
        int chunkSize)
    {
        var chunks = items.Chunk(chunkSize);
        var tasks = chunks
            .Select(chunk => WaitRequest(requestMaker(chunk)))
            .ToArray();

        var responses = await Task.WhenAll(tasks);
        return responses.All(r => r.StatusCode is >= 200 and < 300);
    }
}