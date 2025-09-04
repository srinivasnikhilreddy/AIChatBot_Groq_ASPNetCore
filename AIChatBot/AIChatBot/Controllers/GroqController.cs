using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using AIChatBot.Contexts;
using AIChatBot.Models;

namespace AIChatBot.Controllers
{
    [Route("[controller]")]
    public class GroqController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly GroqSettings groqSettings;
        private readonly ChatDbContext dbContext;
        public GroqController(HttpClient httpClient, IOptions<GroqSettings> groqSettings, ChatDbContext dbContext)
        {
            this.httpClient = httpClient;
            this.groqSettings = groqSettings.Value;
            this.dbContext = dbContext;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("chat")]
        public async Task GetAIResponse([FromBody] string input)
        {
            if(string.IsNullOrWhiteSpace(input)){
                Response.StatusCode = 400;
                await Response.WriteAsync("Input cannot be empty.");
                return;
            }

            var userMsg = new ChatHistory { Role = "user", Content = input };
            dbContext.ChatHistories.Add(userMsg);
            await dbContext.SaveChangesAsync();

            var recentMessages = dbContext.ChatHistories
                .OrderByDescending(c => c.CreatedAt)
                .Take(10)
                .Select(c => new { role = c.Role, content = c.Content, c.CreatedAt })
                .ToList();

            // Always add system prompt at the beginning
            var messages = new List<object>
            {
                new { role = "system", content = "You are a helpful assistant." }
            };

            // Insert history in chronological order (oldest → newest)
            messages.AddRange(recentMessages.OrderBy(c => c.CreatedAt).Select(c => new { c.role, c.content }));
            messages.Add(new { role = "user", content = input });

            var requestData = new
            {
                model = "llama-3.1-8b-instant",
                messages = messages,
                stream = true
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
            {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", groqSettings.ApiKey);

            Response.ContentType = "text/plain"; // use plain text instead of SSE(Server Side Events)

            try
            {
                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);

                var fullAssistantReply = new StringBuilder();

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if(string.IsNullOrWhiteSpace(line)) continue;

                    if(line.StartsWith("data: "))
                    {
                        var json = line.Substring("data: ".Length);
                        if(json == "[DONE]") break;

                        using var doc = JsonDocument.Parse(json);
                        if(doc.RootElement.GetProperty("choices")[0].TryGetProperty("delta", out var delta))
                        {
                            if(delta.TryGetProperty("content", out var contentToken))
                            {
                                var token = contentToken.GetString();
                                if(!string.IsNullOrEmpty(token))
                                {
                                    // Send token to client
                                    await Response.WriteAsync(token);
                                    await Response.Body.FlushAsync();

                                    // Append to assistant's full reply
                                    fullAssistantReply.Append(token);
                                }
                            }
                        }
                    }
                }
                // Save assistant reply after streaming is complete
                if(fullAssistantReply.Length > 0)
                {
                    var assistantMessage = new ChatHistory
                    {
                        Role = "assistant",
                        Content = fullAssistantReply.ToString()
                    };
                    dbContext.ChatHistories.Add(assistantMessage);
                    await dbContext.SaveChangesAsync();
                }

                await Response.Body.FlushAsync();
            }catch (Exception ex){
                Response.StatusCode = 500;
                await Response.WriteAsync($"Error: {ex.Message}");
            }
        }

        /*//one-shot response from api: problem? user has to wait till api respond with the complete response
        [HttpPost("chat")]
        public async Task<IActionResult> GetAIResponse([FromBody] string input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return BadRequest("Input cannot be empty.");

            var requestData = new
            {
                model = "llama-3.1-8b-instant", //change if it deprecated
                messages = new object[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = input }
                },
                max_tokens = 200
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", groqSettings.ApiKey);

            try{
                var response = await httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

                if(!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);

                var aiResponse = doc.RootElement
                                     .GetProperty("choices")[0]
                                     .GetProperty("message")
                                     .GetProperty("content")
                                     .GetString();

                return Ok(new { response = aiResponse });
            }catch(HttpRequestException ex){
                return StatusCode(500, $"Request error: {ex.Message}");
            }
        }*/
    }
}
