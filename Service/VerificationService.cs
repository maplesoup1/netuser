using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Titube.Interfaces;

namespace Titube.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IDistributedCache _cache;
        private readonly IEmailService _emailService;

        public VerificationService(IDistributedCache cache, IEmailService emailService)
        {
            _cache = cache;
            _emailService = emailService;
        }

        public async Task<bool> SendVerificationCode(string email)
        {
            string code = GenerateRandomCode();
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            string cacheKey = $"verification_code:{email}";
            await _cache.SetStringAsync(cacheKey, code, cacheOptions);
            return await _emailService.SendVerificationEmail(email, code);
        }

        public async Task<bool> VerifyCode(string email, string submittedCode)
        {
            string cacheKey = $"verification_code:{email}";
            string storedCode = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(storedCode))
                return false;

            bool isValid = storedCode == submittedCode;

            if (isValid)
            {
                await _cache.RemoveAsync(cacheKey);
            }

            return isValid;
        }

        private string GenerateRandomCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}