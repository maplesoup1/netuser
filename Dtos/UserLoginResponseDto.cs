namespace Titube.Dtos
{
    public class UserLoginResponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; } = null;
    }
}