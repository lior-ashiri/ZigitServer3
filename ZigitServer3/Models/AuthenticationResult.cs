using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ZigitServer3.Models
{
    public class AuthenticationResult
    {
        public string token { get; set; }
        public Dictionary<string, string> personalDetails { get; set; }
        public AuthenticationResult(Dictionary<string, string> fullDictionary, IConfiguration _config)
        {
            this.token = fullDictionary["token"]; //createJWT(fullDictionary, _config);



            this.personalDetails = new Dictionary<string, string>();
            personalDetails.Add("name", fullDictionary["name"]);
            personalDetails.Add("joinedAt", fullDictionary["joinedAt"]);
            personalDetails.Add("Team", fullDictionary["Team"]);
            personalDetails.Add("Avatar", fullDictionary["Avatar"]);


        }

        private string createJWT(Dictionary<string, string> claimsData, IConfiguration _config)
        {
            var builder = new JwtSecurityTokenHandler
            {
                SetDefaultTimesOnTokenCreation = false
            };

            var symetricSecret = _config["Jwt:Key"];

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symetricSecret));

            var claims = claimsData.Select(attr => new Claim(attr.Key, attr.Value));


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = "localhost",
                Issuer = "localhost",
                Expires = new DateTime(DateTime.Now.Ticks).AddDays(1),
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };
            var jwt = builder.CreateJwtSecurityToken(tokenDescriptor);
            return builder.WriteToken(jwt);
        }
    }
}


