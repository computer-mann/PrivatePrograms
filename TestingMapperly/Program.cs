using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Riok.Mapperly.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestingMapperly
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var jsonData = """
                {
                  "message": "Token generated successfully.",
                  "code": 200,
                  "data": {
                    "phoneNumbers": null,
                    "tokenData": {
                      "email": "bruce@hubtel.com",
                      "role": "{\"id\":\"40cb8da110934a489428fdbd97982fe3\",\"name\":\"User Care, Staff Portal, Transaction Monitoring & Compliance\",\"description\":\"User Care, Staff Portal, Transaction Monitoring & Compliance\",\"permissions\":[\"ucmp.supervisor.read\",\"ucmp.supervisor.create\",\"ucmp.supervisor.update\",\"ucmp.supervisor.delete\"]}",
                      "cities": "[]",
                      "zones": "[]",
                      "userName": "bruce@hubtel.com"
                    }
                  },
                  "subCode": null,
                  "errors": null,
                  "isSuccessful": true
                }
                """;

            var obj = JsonConvert.DeserializeObject<EmailLookUpResponse>(jsonData,new JsonSerializerSettings
            {
                
            });

            Console.WriteLine(obj?.Data?.TokenData["email"] ?? "No email found");   
        }
    }


    public class EmailLookUpData
    {
        [JsonPropertyName("phoneNumbers")]
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
        [JsonPropertyName("tokenData")]
        public Dictionary<string, string> TokenData { get; set; } = new();
    }
    public class PhoneNumber
    {
        [Required]
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }
        [Required]
        [JsonPropertyName("number")]
        public string Number { get; set; }
    }
    public class EmailLookUpResponse : LookUpResponse
    {
        [JsonPropertyName("data")]
        public EmailLookUpData Data { get; set; }
    }
    public abstract class LookUpResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
