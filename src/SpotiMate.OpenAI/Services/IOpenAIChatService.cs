namespace SpotiMate.OpenAI.Services;

public interface IOpenAIChatService
{
    Task<string> Complete(string apiKey, string systemMessage, string userMessage);
}
