using System.ComponentModel.DataAnnotations;

namespace Titube.Dtos
{
    public enum UserRoleDto
    {
        Regular = 0,
        Admin = 1
    }

    public class UserUpdateDto
    {
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public UserRoleDto? Role { get; set; }
    }
}