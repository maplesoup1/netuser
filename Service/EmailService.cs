using System;
using System.Threading.Tasks;
using Resend;
using Titube.Interfaces;

namespace Titube.Services
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task<bool> SendVerificationEmail(string email, string code)
        {
            try
            {
                var message = new EmailMessage
                {
                    From = "onboarding@resend.dev",
                    To = new[] { email },
                    Subject = "Verification code",
                    HtmlBody = $"<p>你的验证码是：<strong>{code}</strong>,5分钟内有效。</p>"
                };

                var response = await _resend.EmailSendAsync(message);
            
                return response != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}