using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace SpotiMate.SpotifyWeb;

public interface ISpotifyPlaylistWebParser
{
    Task<string[]> GetTrackIds(string playlistId);
}

public class SpotifyPlaylistWebParser : ISpotifyPlaylistWebParser
{
    private const string PlaylistTrackXPath = "//*[@data-testid=\"playlist-tracklist\"]//*[@data-testid=\"internal-track-link\"]";

    public async Task<string[]> GetTrackIds(string playlistId)
    {
        var chromeService = ChromeDriverService.CreateDefaultService();
        chromeService.HideCommandPromptWindow = true;
        chromeService.SuppressInitialDiagnosticInformation = true;

        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--window-size=1200,1000");

        using var driver = new ChromeDriver(chromeService, options);

        var url = $"https://open.spotify.com/playlist/{playlistId}";
        await driver.Navigate().GoToUrlAsync(url);

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        wait.Until(d => d.FindElements(By.XPath(PlaylistTrackXPath)).Count > 0);

        await Task.Delay(1000);

        var actions = new Actions(driver);
        var htmlContainer = new HtmlDocument();
        var allTrackIds = new HashSet<string>();
        var lastElementText = string.Empty;

        while (true)
        {
            ParseAndSaveTrackIds(htmlContainer, driver.PageSource, allTrackIds);

            var linkElements = driver.FindElements(By.XPath(PlaylistTrackXPath));
            var lastLinkElement = linkElements.Last();

            if (lastLinkElement.Text == lastElementText)
            {
                break;
            }

            lastElementText = lastLinkElement.Text;

            actions.MoveToElement(lastLinkElement).Perform();
        }

        return allTrackIds.ToArray();
    }

    private static void ParseAndSaveTrackIds(HtmlDocument doc, string html, HashSet<string> trackIdsCollection)
    {
        doc.LoadHtml(html);

        var trackLinks = doc.DocumentNode.SelectNodes(PlaylistTrackXPath);

        if (trackLinks == null || trackLinks.Count == 0)
        {
            return;
        }

        var trackIds = trackLinks
            .Select(x => x.GetAttributeValue("href", string.Empty).Split('/').LastOrDefault())
            .Where(href => !string.IsNullOrEmpty(href));

        trackIdsCollection.UnionWith(trackIds);
    }
}
