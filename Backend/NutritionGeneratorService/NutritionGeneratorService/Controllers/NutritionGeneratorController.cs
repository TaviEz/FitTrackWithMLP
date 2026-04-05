using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NutritionGeneratorService.DTOs;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using static OpenAI.ObjectModels.StaticValues.AssistantsStatics.MessageStatics;

namespace NutritionGeneratorService.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NutritionGeneratorController : Controller
    {
        private readonly IConfiguration _config;
        private readonly OpenAIService _api;
        public NutritionGeneratorController(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            var huggingFaceToken = _config["hf_token_secret"] ?? string.Empty;
            var huggingFaceUrl = _config["HuggingFaceSettings"] ?? string.Empty;
            _api = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = huggingFaceToken,
                BaseDomain = huggingFaceUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> GenerateNutritionPlan([FromBody] UserDetailsDto dto)
        {
            var fullResponse = new StringBuilder();
            // Use string interpolation to inject DTO properties
            var prompt = $@"You are a personal trainer.  
                Create a meal plan for a week for a {dto.Age}-year-old {dto.Gender} ({dto.Weight}kg, {dto.Height}cm) 
                with an activity level of {dto.ActivityLevel} aiming to reach their fitness goals.
                - List 3 meals per day without descriptions. Make individual meal plans for each day.";

            //content = """You are a personal trainer.  
            //Create a 3 - day workout for a 23 - year - old woman(54kg) beginner aiming gain muscle and stay lean.
            //- List only exercises per day(no descriptions)
            //- List 3 meals per day without descriptions.Make individual meal plans for each day.  
            //Format using markdown with clear line breaks."""

            var completionRequest = new ChatCompletionCreateRequest
            {
                Model = "openai/gpt-oss-20b:fireworks-ai", // Example model
                Messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(prompt)
            },
                Stream = true // Enables streaming responses
            };

            try
            {
                await foreach (var completion in _api.ChatCompletion.CreateCompletionAsStream(completionRequest))
                {
                    if (completion.Successful)
                    {
                        var content = completion.Choices.FirstOrDefault()?.Message.Content;
                        if (!string.IsNullOrEmpty(content))
                        {
                            fullResponse.Append(content);
                        }
                    }
                    else
                    {
                        return BadRequest($"AI Error: {completion.Error?.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }

            var phrases = fullResponse.ToString().Split("\n", StringSplitOptions.RemoveEmptyEntries);

            return Ok(new { Text = phrases });
        }
    }
}
