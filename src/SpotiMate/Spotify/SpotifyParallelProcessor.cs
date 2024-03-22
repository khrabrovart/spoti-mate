using Flurl.Http;

namespace SpotiMate.Spotify;

public class SpotifyParallelProcessor
{
    private const int Parallelism = 10;
    private const int DelayMs = 1000;

    public async Task<IReadOnlyCollection<TItem>> GetAll<TResponse, TItem>(
        IFlurlRequest request,
        int limit,
        Func<TResponse, IEnumerable<TItem>> selector)
    {
        var results = new List<TItem>();

        while (true)
        {
            var tasks = new List<Task<TResponse>>();

            for (var i = 0; i < Parallelism; i++)
            {
                var offset = results.Count + i * limit;

                var task = request
                    .SetQueryParams(new { limit, offset })
                    .GetJsonAsync<TResponse>();

                tasks.Add(task);
            }

            var responses = await Task.WhenAll(tasks);
            var items = responses.SelectMany(selector).ToArray();

            results.AddRange(items);

            if (items.Length < Parallelism * limit)
            {
                break;
            }
            
            await Task.Delay(DelayMs);
        }

        return results;
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
            var responses = await Task.WhenAll(tasks);
            
            if (responses.Any(r => !r))
            {
                return false;
            }
            
            await Task.Delay(DelayMs);
        }
        
        return true;
    }
}