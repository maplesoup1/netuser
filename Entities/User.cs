using System.ComponentModel.DataAnnotations;

namespace Titube.Entities
{
    public enum UserRole
    {
        Regular = 0,
        Admin = 1
    }
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        public UserRole Role { get; set; } = UserRole.Regular;
        public DateTime CreatedAt { get; set; }
    }
}