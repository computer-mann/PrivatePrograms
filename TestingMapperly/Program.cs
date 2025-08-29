using Riok.Mapperly.Abstractions;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace TestingMapperly
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GenerateFiveYearToken());
        }

    public static string GenerateFiveYearToken()
    {
        var rsa = new RSAParameters
        {
            Exponent = Convert.FromBase64String("AQAB"),
            Modulus = Convert.FromBase64String(
                "tAHAfvtmGBng322TqUXF/Aar7726jFELj73lywuCvpGsh3JTpImuoSYsJxy5GZCRF7ppIIbsJBmWwSiesYfxWxBsfnpOmAHU3OTMDt383mf0USdqq/F0yFxBL9IQuDdvhlPfFcTrWEL0U2JsAzUjt9AfsPHNQbiEkOXlIwtNkqMP2naynW8y4WbaGG1n2NohyN6nfNb42KoNSR83nlbBJSwcc3heE3muTt3ZvbpguanyfFXeoP6yyqatnymWp/C0aQBEI5kDahOU641aDiSagG7zX1WaF9+hwfWCbkMDKYxeSWUkQOUOdfUQ89CQS5wrLpcU0D0xf7/SrRdY2TRHvQ==")
        };

        var securityKey = new RsaSecurityKey(rsa)
        {
            KeyId = "IdentityServerLicensekey/7ceadbb78130469e8806891025414f16"
        };

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = "https://duendesoftware.com",
            Audience = "IdentityServer",
            Expires = DateTime.UtcNow.AddYears(5),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}
}
