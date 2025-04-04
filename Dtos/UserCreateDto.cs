namespace Titube.Dtos
{
    public class UserCreateDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string VerificationCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}


