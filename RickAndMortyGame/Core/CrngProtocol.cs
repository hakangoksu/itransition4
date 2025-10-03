using System;
using System.Security.Cryptography;
using System.Text;
using RickAndMortyGame.Common;

namespace RickAndMortyGame.Core
{

    public class CrngProtocol
    {
        private readonly SecretKeyManager _keyManager;

        public CrngProtocol(SecretKeyManager keyManager)
        {
            _keyManager = keyManager;
        }

        public CrngResult Generate(int range)
        {
            if (range <= 0) throw new ArgumentOutOfRangeException(nameof(range), "Range must be positive.");

            byte[] secretKey = _keyManager.GenerateKey();

            int mortyValue = GenerateSecureRandomInt(range);
            byte[] mortyValueBytes = BitConverter.GetBytes(mortyValue);

            byte[] hmacBytes;
            using (var hmac = new HMACSHA256(secretKey))
            {
                hmacBytes = hmac.ComputeHash(mortyValueBytes);
            }

            Console.WriteLine($"\n[CRNG Protocol] Morty has generated their secret value.");
            Console.WriteLine($"[CRNG Protocol] HMAC Proof: {BitConverter.ToString(hmacBytes).Replace("-", "").ToLower()}");

            int rickValue = GetRickInputValue(range);

            int finalResult = (mortyValue + rickValue) % range;

            return new CrngResult(finalResult, mortyValue, rickValue, secretKey);
        }

        private int GenerateSecureRandomInt(int maxExclusive)
        {
            return RandomNumberGenerator.GetInt32(maxExclusive);
        }

        private int GetRickInputValue(int range)
        {
            int rickValue = -1;
            while (rickValue < 0)
            {
                Console.Write($"[CRNG Protocol] Rick, please provide your secret contribution (integer 0 to {range - 1}): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int parsedValue) && parsedValue >= 0 && parsedValue < range)
                {
                    rickValue = parsedValue;
                }
                else
                {
                    Console.WriteLine($"[CRNG Protocol] Invalid input. Please enter an integer between 0 and {range - 1}.");
                }
            }
            return rickValue;
        }

        public void RevealProtocol(CrngResult result)
        {
            Console.WriteLine("\n--- CRNG Protocol Verification ---");
            Console.WriteLine($"Secret Key: {BitConverter.ToString(result.SecretKey).Replace("-", "").ToLower()}");
            Console.WriteLine($"Morty's Original Value: {result.MortyValue}");
            Console.WriteLine($"Rick's Value: {result.RickValue}");
            Console.WriteLine($"Final Result (Hiding Box): {result.FinalResult}");
            Console.WriteLine("----------------------------------");
        }
    }
}
