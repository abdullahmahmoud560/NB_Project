using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NB_Project.Model
{
    public class User
    {
        [Column("UserID")]
        public int Id { get; set; }
        public string? fullName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? phoneNumber { get; set; }
        public string? Password { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Town { get; set; }
        public string refreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public ICollection<Car>? cars { get; set; }
    }
}
