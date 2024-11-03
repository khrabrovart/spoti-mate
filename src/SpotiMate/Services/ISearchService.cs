namespace SpotiMate.Services;

public interface ISearchService
{
    Task<bool> SearchAndSaveTracks(string[] trackNames, string addToPlaylistId, string openAIApiKey);
}
