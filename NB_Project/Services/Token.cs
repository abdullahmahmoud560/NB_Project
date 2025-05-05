using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NB_Project.ApplicationDbContext;
using NB_Project.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NB_Project.Services
{
    public class Token
    {
        private readonly IConfiguration _configuration;
        private readonly DB _db;

        public Token(IConfiguration configuration , DB db)
        {
            _configuration = configuration;
            _db = db;
        }

        public string GenerateAccessToken(User infoStudnet)
        {
            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:secretKey"]!));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim("id", infoStudnet.Id.ToString()),
                    new Claim("fullName", infoStudnet.fullName!),
                    new Claim("email", infoStudnet.Email!),
                    new Claim("phone", infoStudnet.phoneNumber!),
                    new Claim("country", infoStudnet.Country!),
                    new Claim("city", infoStudnet.City!),
                    new Claim("town", infoStudnet.Town!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var tokeOptions = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(15),
                    signingCredentials: signinCredentials
                );

                return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GenerateRefreshToken(User infoStudnet)
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var refreshToken = Convert.ToBase64String(randomNumber);
                var studnet = await _db.users.Where(l=>l.Id == infoStudnet.Id).FirstOrDefaultAsync();
                if (studnet != null) { 
                    studnet.refreshToken = refreshToken;
                    studnet.TokenCreated = DateTime.UtcNow;
                    studnet.TokenExpires = DateTime.UtcNow.AddDays(7);
                    _db.Update(studnet);
                    await _db.SaveChangesAsync();
                }
                return refreshToken;
            }
        }

    }
}
