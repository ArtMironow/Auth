using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Entities.DataTransferObjects;
using DAL.Auth.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Auth.Features
{
    public class JwtHandler
    {
        private readonly IConfigurationSection _jwtSettings;
        private readonly IConfigurationSection _goolgeSettings;
        private readonly UserManager<User> _userManager;

        public JwtHandler(IConfiguration configuration, UserManager<User> userManager)
        {
            _jwtSettings = configuration.GetSection("JwtSettings");
            _goolgeSettings = configuration.GetSection("GoogleAuthSettings");
            _userManager = userManager;
        }

        public SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value);
            var secret = new SymmetricSecurityKey(key)
;

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<List<Claim>> GetClaims(User user)
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                return claims;
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email)
                };

                return claims;
            }
        }

        public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _goolgeSettings.GetSection("clientId").Value }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<FacebookAccountDto?> VerifyFacebookToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var httpClient = new HttpClient { BaseAddress = new Uri("https://graph.facebook.com/v2.9/%22") };
                var response = await httpClient.GetAsync($"me?access_token={externalAuth.IdToken}&fields=id,name,email," +
                                                         $"first_name,last_name,age_range,birthday,gender,locale,picture");
                var result = await response.Content.ReadAsStringAsync();
                var facebookAccountDto = JsonConvert.DeserializeObject<FacebookAccountDto>(result);

                return facebookAccountDto;
            }
                
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GenerateToken(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return token;
        }
    }
}
