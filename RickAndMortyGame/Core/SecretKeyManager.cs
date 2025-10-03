using System;
using System.Security.Cryptography;

namespace RickAndMortyGame.Core
{
    public class SecretKeyManager
    {
        private const int KeyByteLength = 32;

        public byte[] GenerateKey()
        {
            byte[] key = new byte[KeyByteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return key;
        }
    }
}
