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

    
    }
}


