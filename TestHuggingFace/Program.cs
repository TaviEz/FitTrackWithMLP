using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;

class Program
{
    static async Task Main()
    {
        var token = Environment.GetEnvironmentVariable("HF_TOKEN");
        var url = "https://router.huggingface.co/v1";

        var api = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = token,
            BaseDomain = url
        });

        var prompt = "You are a personal trainer.  \r\nCreate a 3-day workout for a 23-year-old woman (54kg) beginner aiming gain muscle and stay lean." +
            "\r\n- List only exercises per day (no descriptions)\r\n- List 3 meals per day without descriptions. Make individual meal plans for each day. " +
            "\r\nFormat using markdown with clear line breaks.";

        var completionRequest = new ChatCompletionCreateRequest
        {
            Model = "openai/gpt-oss-20b:fireworks-ai", // Example model
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(prompt)
            },
            Stream = true // Enables streaming responses
        };

        await foreach (var completion in api.ChatCompletion.CreateCompletionAsStream(completionRequest))
        {
            if (completion.Successful)
            {
                Console.Write(completion.Choices.FirstOrDefault()?.Message.Content);
            }
            else
            {
                Console.WriteLine($"Error: {completion.Error?.Message}");
            }
        }
    }
}
