using System;
using RickAndMortyGame.Core;
using RickAndMortyGame.GameCore;

namespace RickAndMortyGame
{

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ArgumentProcessor.GameConfiguration config = ArgumentProcessor.Process(args);

                SecretKeyManager keyManager = new SecretKeyManager();
                CrngProtocol crngProtocol = new CrngProtocol(keyManager);

                IMorty morty = (IMorty)Activator.CreateInstance(config.MortyType);

                GameRunner runner = new GameRunner(config.BoxCount, morty, crngProtocol);
                runner.Run();
            }
            catch (Exception ex)
            {

                string errorType = ex is ArgumentException || ex is FileNotFoundException ? "Configuration Error" : "Runtime Error";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n--- {errorType} ---");
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine("\nUSAGE EXAMPLE:");
                Console.WriteLine("dotnet run -- <Boxes> <MortyType>");
                Console.WriteLine("Example: dotnet run -- 3 EvilMorty");
                Console.ResetColor();
            }
        }
    }
}
