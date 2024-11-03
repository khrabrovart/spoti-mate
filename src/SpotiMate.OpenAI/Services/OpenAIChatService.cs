using OpenAI.Chat;

namespace SpotiMate.OpenAI.Services;

public class OpenAIChatService : IOpenAIChatService
{
    public async Task<string> Complete(string apiKey, string systemMessage, string userMessage)
    {
        var chat = new ChatClient("gpt-3.5-turbo", apiKey);

        var systemChatMessage = ChatMessage.CreateSystemMessage(systemMessage);
        var userChatMessage = ChatMessage.CreateUserMessage(userMessage);

        var completion = await chat.CompleteChatAsync(systemChatMessage, userChatMessage);

        return completion.Value.Content[0].Text;
    }
}
