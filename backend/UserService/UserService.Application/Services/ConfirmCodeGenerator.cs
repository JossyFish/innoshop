using System.Security.Cryptography;
using UserService.Application.Interfaces;

namespace UserService.Application.Services
{
    public class ConfirmCodeGenerator : IConfirmCodeGenerator
    {
        public ConfirmCodeGenerator() { }

        public string Generate()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var randomNumber = new byte[4];
                rng.GetBytes(randomNumber);
                var randomValue = BitConverter.ToUInt32(randomNumber, 0) % 1000000;

                return (randomValue == 0 ? 1 : randomValue).ToString("D6");
            }
        }
    }
}
