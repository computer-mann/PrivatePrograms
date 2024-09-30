using Riok.Mapperly.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestingMapperly
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var json = "[{\"data\":{\"message\":\"adsds\",\"roomChannel\":\"music\"},\"time\":\"2024-09-25T15:37:17.582Z\",\"clientId\":\"Vk36bUdjH_lmXYh5AGBx\",\"source\":{\"platform\":\"web\",\"appName\":\"web-hubtel-chat\",\"appVersion\":\"1.0.0\",\"clientId\":\"Vk36bUdjH_lmXYh5AGBx\",\"userName\":\"prince\",\"channel\":\"music\"}}]";
            List<ChatPayload> chatPayloads = JsonSerializer.Deserialize<List<ChatPayload>>(json);

            // Access the data
            foreach (var payload in chatPayloads)
            {
                Console.WriteLine($"Message: {payload.Data.Message}");
                Console.WriteLine($"RoomChannel: {payload.Data.RoomChannel}");
                Console.WriteLine($"UserName: {payload.Source.UserName}");
            }
            Console.WriteLine(json);
            Console.WriteLine();
            var fold = new ChatPayload
            {
                ClientId = "Vk36bUdjH_lmXYh5AGBx",
                Data = new ChatData
                {
                    Message = "adsds",
                    RoomChannel = "music"
                },
                Source = new Source
                {
                    Platform = "web",
                    AppName = "web-hubtel-chat",
                    AppVersion = "1.0.0",
                    ClientId = "Vk36bUdjH_lmXYh5AGBx",
                    UserName = "prince",
                    Channel = "music"
                },
                Time = DateTime.Parse("2024-09-25T15:37:17.582Z")
            };

            Console.WriteLine(JsonSerializer.Serialize(fold));
        }


        public class ChatPayload
        {
            [JsonPropertyName("data")]
            public ChatData Data { get; set; }
            [JsonPropertyName("time")]
            public DateTime Time { get; set; }
            [JsonPropertyName("clientId")]
            public string ClientId { get; set; }
            [JsonPropertyName("source")]
            public Source Source { get; set; }
        }

        public class ChatData
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }
            [JsonPropertyName("roomChannel")]
            public string RoomChannel { get; set; }
        }

        public class Source
        {
            [JsonPropertyName("platform")]
            public string Platform { get; set; }
            [JsonPropertyName("appName")]
            public string AppName { get; set; }
            [JsonPropertyName("appVersion")]
            public string AppVersion { get; set; }
            [JsonPropertyName("clientId")]
            public string ClientId { get; set; }
            [JsonPropertyName("userName")]
            public string UserName { get; set; }
            [JsonPropertyName("channel")]
            public string Channel { get; set; }
        }
    }
}
