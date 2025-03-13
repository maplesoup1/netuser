using System.ComponentModel.DataAnnotations;

namespace Titube.Entities
{
    public class User
    {   
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        
        public DateTime CreatedAt { get; set; }
    }
}