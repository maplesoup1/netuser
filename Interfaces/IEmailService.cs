using System.Threading.Tasks;

namespace Titube.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmail(string email, string code);
    }
}