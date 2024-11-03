using System.Net;

namespace SpotiMate.Spotify.Models;

public class ApiResponse<TResponse> : ApiResponse
{
    public TResponse Data { get; set; }
}

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }

    public bool IsError { get; set; }

    public string Error { get; set; }

    public int? RetryAfter { get; set; }
}
