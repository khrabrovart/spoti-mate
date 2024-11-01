using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Authorization;
using SpotiMate.Spotify.Objects;

namespace SpotiMate.Services;

public class SearchService : ISearchService
{
    private readonly ISpotifySearchApi _spotifySearchApi;

    public SearchService(ISpotifySearchApi spotifySearchApi)
    {
        _spotifySearchApi = spotifySearchApi;
    }

    public async Task SearchTracks()
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, "Resources", "playlist.txt");
        var playlist = await File.ReadAllLinesAsync(filePath);

        var notFound = new List<string>();

        foreach (var track in playlist)
        {
            var nameParts = track.Split(" - ");

            var artist = nameParts[0];
            var title = nameParts[1];

            var q = $"artist:{artist} track:{title}";

            var searchResponse = await _spotifySearchApi.SearchTracks(q);

            var foundTrack = searchResponse.Tracks.Items.FirstOrDefault();

            CliPrint.PrintInfo(track);

            if (foundTrack == null)
            {
                CliPrint.PrintWarning("Track not found");
                notFound.Add(track);
            }
            else
            {
                CliPrint.PrintInfo(ComposeTrackString(foundTrack));
            }

            Console.WriteLine();
        }

        CliPrint.PrintInfo($"Tracks not found: {notFound.Count}");
    }

    private static string ComposeTrackString(TrackObject track)
    {
        if (track == null)
        {
            return "Track not found";
        }

        var artists = string.Join(", ", track.Artists.Select(a => a.Name));

        return $"{artists} - {track.Name}";
    }
}
