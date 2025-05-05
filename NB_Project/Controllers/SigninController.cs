using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NB_Project.ApplicationDbContext;
using NB_Project.Services;

namespace NB_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SigninController : ControllerBase
    {
        private readonly DB _db;
        private readonly Token _token;
        private readonly IConfiguration _configuration;

        public SigninController(DB db,Token token, IConfiguration configuration)
        {
            _db = db;
            _token = token;
            _configuration = configuration;
        }

        [HttpPost("SignIn-User")]
        public async Task<IActionResult> signIn(SignInDTO signIn)
        {
            try {
            if (ModelState.IsValid && signIn != null)
            {
                    var student = await _db.users.FirstOrDefaultAsync(s => s.Email == signIn.Email);

                    if (student != null && BCrypt.Net.BCrypt.Verify(signIn.Password, student.Password))
                    {
                        var refreshToken = await _token.GenerateRefreshToken(student);
                        var accessToken = _token.GenerateAccessToken(student);
                        return Ok(new { message = "Sign in successful" ,refreshToken = refreshToken , accessToken= accessToken });
                    }
                    return BadRequest(new { message = "Invalid email or password" });
                }
            return BadRequest("Invalid data");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> refreshToken(refreshTokenDTO refreshTokenDTO)
        {
            try
            {
                if (refreshTokenDTO.refreshToken != null)
                {
                    var student = await _db.users.FirstOrDefaultAsync(s => s.refreshToken == refreshTokenDTO.refreshToken);
                    if (student != null && student.TokenExpires > DateTime.UtcNow)
                    {
                        var newAccessToken = _token.GenerateAccessToken(student);
                        return Ok(new { message = "Refresh token successful", accessToken = newAccessToken });
                    }
                    return BadRequest(new { message = "Invalid refresh token" });
                }
                return BadRequest("Invalid data");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
