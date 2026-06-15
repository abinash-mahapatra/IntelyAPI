using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace IntelyAPI.Model
{
    public class AuthenticationHelper
    {
        ConfigurationManager _configuration;
        public AuthenticationHelper(ConfigurationManager configuration)
        {
            _configuration = configuration;
        }
        public string Login()
        {
            return GenerateJWT();
        }

        private string GenerateJWT()
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudiance"],
            claims: null,
            expires: System.DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
