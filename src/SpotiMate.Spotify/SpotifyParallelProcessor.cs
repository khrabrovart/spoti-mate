using Flurl.Http;
using SpotiMate.Spotify.Objects;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify;

public class SpotifyParallelProcessor
{
    private const int MaxRetries = 10;
    private const int DelayBeforeSubsequentRequestMs = 500;

    private const int Default429DelayMs = 60000;
    private const int Default500DelayMs = 10000;
    
    public async Task<TItem[]> GetAll<TResponse, TItem>(Func<IFlurlRequest> requestBuilder, int pageSize)
        where TResponse : ISpotifyPageResponse<TItem>
        where TItem : ISpotifyObject
    {
        var initRequest = MakePageRequest<TResponse>(requestBuilder(), pageSize, 0);
        var initResponse = await WaitRequestWithRetries(initRequest);
        
        var total = initResponse.Total;
        var initResponseItems = initResponse.Items;

        var remainingRequestsCount = (int)Math.Ceiling((total - initResponseItems.Length) / (double)pageSize);
        var tasks = new List<Task<TResponse>>();

        for (var i = 0; i < remainingRequestsCount; i++)
        {
            tasks.Add(WaitRequestWithRetries(
                MakePageRequest<TResponse>(requestBuilder(), pageSize, initResponseItems.Length + i * pageSize)));

            await Task.Delay(DelayBeforeSubsequentRequestMs);
        }

        var responses = await Task.WhenAll(tasks);
        var results = new List<TItem>(total);

        results.AddRange(initResponseItems);
        
        foreach (var response in responses)
        {
            results.AddRange(response.Items);
        }

        return results.Count == total ? results.ToArray() : null;
    }
    
    public async Task<bool> ProcessAll<TItem>(
        Func<IEnumerable<TItem>, Task<IFlurlResponse>> makeRequest,
        IEnumerable<TItem> items,
        int chunkSize)
    {
        var chunks = items.Chunk(chunkSize).ToArray();
        var tasks = new List<Task<IFlurlResponse>>(chunks.Length);

        foreach (var chunk in chunks)
        {
            tasks.Add(WaitRequestWithRetries(makeRequest(chunk)));
            await Task.Delay(DelayBeforeSubsequentRequestMs);
        }

        var responses = await Task.WhenAll(tasks);
        return responses.All(r => r.StatusCode is >= 200 and < 300);
    }

    private static Task<TResponse> MakePageRequest<TResponse>(IFlurlRequest request, int limit, int offset)
    {
        return request
            .SetQueryParams(new { limit, offset })
            .GetJsonAsync<TResponse>();
    }

    private static async Task<TResponse> WaitRequestWithRetries<TResponse>(Task<TResponse> requestTask)
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
                            throw new SpotifyClientException("Request limit exceeded. Too many retries.");
                        }
                        
                        if (ex.Call.Response.Headers.TryGetFirst("Retry-After", out var retryAfter))
                        {
                            var delay = int.Parse(retryAfter) + 1;

                            //.PrintWarning(
                                //$"Request limit exceeded. Waiting for {retryAfter} seconds before retrying based on Retry-After header.");

                            await Task.Delay(delay * 1000);
                        }
                        else
                        {
                            //CliPrint.PrintWarning("No Retry-After header found. Waiting for 1 minute before retrying.");
                            await Task.Delay(Default429DelayMs);
                        }

                        continue;
                    }
                    
                    case 500:
                    {
                        if (retries++ >= MaxRetries)
                        {
                            throw new SpotifyClientException("Internal server error. Too many retries.");
                        }

                        //CliPrint.PrintWarning("Internal server error. Waiting for 10 seconds before retrying.");
                        await Task.Delay(Default500DelayMs);
                        
                        continue;
                    }
                    
                    default:
                        throw;
                }
            }
        }
    }
}
