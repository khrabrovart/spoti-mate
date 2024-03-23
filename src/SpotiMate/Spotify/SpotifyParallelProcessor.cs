using Flurl.Http;
using SpotiMate.Cli;
using SpotiMate.Spotify.Objects;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify;

public class SpotifyParallelProcessor
{
    private const int Parallelism = 10;
    private const int RegularDelay = 1000;
    
    public async Task<IReadOnlyCollection<TItem>> GetAll<TResponse, TItem>(Func<IFlurlRequest> requestBuilder, int limit) 
        where TResponse : ISpotifyPageResponse<TItem>
        where TItem : ISpotifyObject
    {
        var initResponse = await MakeRequest<TResponse>(requestBuilder(), limit, 0);
        
        var total = initResponse.Total;
        var initItems = initResponse.Items;

        var remainingRequestsCount = (int)Math.Ceiling((total - initItems.Length) / (double)limit);
        var tasks = new List<Task<TResponse>>();
        
        for (var i = 0; i < remainingRequestsCount; i++)
        {
            tasks.Add(MakeRequest<TResponse>(requestBuilder(), limit, initItems.Length + i * limit));
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

    private static async Task<TResponse> MakeRequest<TResponse>(IFlurlRequest request, int limit, int offset)
    {
        while (true)
        {
            try
            {
                return await request
                    .SetQueryParams(new { limit, offset })
                    .GetJsonAsync<TResponse>();
            }
            catch (FlurlHttpException ex)
            {
                switch (ex.StatusCode)
                {
                    case 429:
                    {
                        if (ex.Call.Response.Headers.TryGetFirst("Retry-After", out var ra))
                        {
                            var delay = int.Parse(ra) + 1;
                            await Task.Delay(delay * 1000);
                        }
                    
                        continue;
                    }
                    
                    case 500:
                    {
                        CliPrint.PrintError("Internal server error. Retrying...");
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
        IEnumerable<TItem> items,
        int chunkSize,
        Func<IEnumerable<TItem>, Task<bool>> action)
    {
        var chunks = items.Chunk(chunkSize);
        var batches = chunks.Chunk(Parallelism);
        
        foreach (var batch in batches)
        {
            var tasks = batch.Select(async c => await action(c));

            try
            {

                var responses = await Task.WhenAll(tasks);

                if (responses.Any(r => !r))
                {
                    return false;
                }
            }
            catch (FlurlHttpException ex)
            {
                if (ex.StatusCode == 429)
                {
                    CliPrint.PrintError("Too many requests.");
                }
                
                return false;
            }

            await Task.Delay(RegularDelay);
        }
        
        return true;
    }
}