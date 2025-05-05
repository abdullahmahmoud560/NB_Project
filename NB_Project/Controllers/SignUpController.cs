using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NB_Project.ApplicationDbContext;
using NB_Project.Model;
using NB_Project.Services;

namespace NB_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly DB _db;
       
        public SignUpController(DB db)
        {
            _db = db;
        }

        [HttpPost("SignUp-User")]
        public async Task<IActionResult> signUp(SignUpDTO signUp)
        {
            if(ModelState.IsValid && signUp != null)
            {
                if (await _db.users.AnyAsync(s => s.Email == signUp.Email || s.phoneNumber == signUp.PhoneNumber))
                {
                    return BadRequest(new { message = "Email or PhoneNumber already exists" });
                }
                var user = new User()
               {
                   fullName = signUp.fullName,
                   Email = signUp.Email,
                   Password = BCrypt.Net.BCrypt.HashPassword(signUp.Password),
                   phoneNumber = signUp.PhoneNumber,
                   Country = signUp.Country,
                   City = signUp.City,
                   Town = signUp.Town,
               };
                if(signUp.Password != signUp.confirmPassword)
                {
                    return BadRequest(new { message = "Password and Confirm Password do not match"});
                }
                await _db.users.AddAsync(user);
                await _db.SaveChangesAsync();
                return Ok(new { message = "Student registered successfully" });
            }
            return BadRequest("Invalid data");
        }

    }
}
