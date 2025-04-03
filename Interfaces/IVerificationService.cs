namespace Titube.Interfaces
{
    public interface IVerificationService
    {
        Task<bool> SendVerificationCode(string email);
        Task<bool> VerifyCode(string email, string submittedCode);
    }
}