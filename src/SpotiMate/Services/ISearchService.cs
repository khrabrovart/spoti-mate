namespace SpotiMate.Services;

public interface ISearchService
{
    Task<string[]> SearchTracks(string[] trackNames, string openAIApiKey);
}
