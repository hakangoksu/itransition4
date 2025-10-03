using System;

namespace RickAndMortyGame.Common
{
    public class CrngResult
    {
        public int FinalResult { get; }
        public int MortyValue { get; }
        public int RickValue { get; }
        public byte[] SecretKey { get; }

        public CrngResult(int finalResult, int mortyValue, int rickValue, byte[] secretKey)
        {
            FinalResult = finalResult;
            MortyValue = mortyValue;
            RickValue = rickValue;
            SecretKey = secretKey;
        }
    }
}
