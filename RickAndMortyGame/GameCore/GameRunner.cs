using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using RickAndMortyGame.Common;
using RickAndMortyGame.Core;

namespace RickAndMortyGame.GameCore
{
    public class GameRunner : IGameCore
    {
        private readonly int _boxesCount;
        private readonly IMorty _morty;
        private readonly CrngProtocol _crngProtocol;
        private readonly GameStatistics _stats = new GameStatistics();

        private CrngResult _lastCrngResult;

        public GameRunner(int boxesCount, IMorty morty, CrngProtocol crngProtocol)
        {
            _boxesCount = boxesCount;
            _morty = morty;
            _crngProtocol = crngProtocol;

            Console.WriteLine($"\n--- Game Initialized ---");
            Console.WriteLine($"Boxes (N): {_boxesCount}");
            Console.WriteLine($"Morty AI: {_morty.Name}");
            Console.WriteLine($"------------------------\n");
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine($"\n==================== NEW ROUND ====================");
                Console.WriteLine($"Boxes 0 to {_boxesCount - 1}.");

                Console.WriteLine("Morty is using the CRNG protocol to hide Rick's portal gun...");
                _lastCrngResult = RequestCrngValue(_boxesCount);
                int portalGunBoxIndex = _lastCrngResult.FinalResult;
                Console.WriteLine($"Portal gun hidden (Box: ??). CRNG completed.");

                _morty.TalkMorty(_morty.GetHideGunPhrase());

                int rickSelectedBoxIndex = GetRickInput("Rick, choose your initial box (0 to {0}): ", _boxesCount);
                int finalChosenBoxIndex = rickSelectedBoxIndex;
                bool switchedChoice = false;

                if (_morty.ShouldRevealBoxes(_boxesCount, portalGunBoxIndex, rickSelectedBoxIndex))
                {
                    List<int> revealedBoxes = _morty.RevealBoxes(_boxesCount, portalGunBoxIndex, rickSelectedBoxIndex, this);

                    if (revealedBoxes.Count != _boxesCount - 2)
                    {
                        Console.WriteLine($"\n[ERROR] Morty failed to reveal exactly N-2 ({_boxesCount - 2}) boxes. Round void.");
                        _lastCrngResult = null;
                        continue;
                    }

                    string revealedBoxesStr = string.Join(", ", revealedBoxes);
                    int remainingBoxIndex = Enumerable.Range(0, _boxesCount)
                        .Except(revealedBoxes)
                        .Single(i => i != rickSelectedBoxIndex);

                    Console.WriteLine($"\nMorty reveals {_boxesCount - 2} boxes that do not contain the portal gun: [{revealedBoxesStr}]");
                    Console.WriteLine($"Remaining choices: Your initial box ({rickSelectedBoxIndex}) and the new box ({remainingBoxIndex}).");


                    _morty.TalkMorty(_morty.GetDecisionPhrase());

                    bool keep = GetRickDecision("Rick, do you want to [K]eep your box or [S]witch to the other? (K/S): ", remainingBoxIndex);
                    if (!keep)
                    {
                        finalChosenBoxIndex = remainingBoxIndex;
                        switchedChoice = true;
                    }
                    Console.WriteLine($"Rick chooses box: {finalChosenBoxIndex} (Action: {(switchedChoice ? "Switch" : "Keep")})");
                }

                Thread.Sleep(1000);
                bool rickWon = finalChosenBoxIndex == portalGunBoxIndex;
                Console.WriteLine($"\n--- RESULT ---");
                Console.WriteLine($"Portal gun was in box: {portalGunBoxIndex}");
                Console.WriteLine($"Rick's final box: {finalChosenBoxIndex}");

                Thread.Sleep(1000);
                if (rickWon)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("RICK WINS!");
                    Console.ResetColor();
                    _morty.TalkMorty(_morty.GetRickWinPhrase());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("MORTY WINS!");
                    Console.ResetColor();

                    _morty.TalkMorty(_morty.GetMortyWinPhrase());
                }

                _stats.RecordOutcome(rickWon, switchedChoice);
                Thread.Sleep(1000);
                RequestProtocolReveal(_lastCrngResult);

                if (!GetRickDecision("Do you want to play [A]nother round or [E]xit and see the summary? (A/E): ", exit: true))
                {
                    break;
                }
            }

            Console.Write(_stats.GetSummary(_morty, _boxesCount));
        }

        private int GetRickInput(string prompt, int maxExclusive)
        {
            int input = -1;
            while (input < 0 || input >= maxExclusive)
            {
                Console.Write(string.Format(prompt, maxExclusive - 1));
                string line = Console.ReadLine();
                if (int.TryParse(line, out input) && input >= 0 && input < maxExclusive)
                {
                    return input;
                }
                Console.WriteLine($"Invalid input. Please enter an integer between 0 and {maxExclusive - 1}.");
            }
            return input;
        }

        private bool GetRickDecision(string prompt, int remainingBox = -1, bool reveal = false, bool exit = false)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim().ToUpper();

                if (remainingBox != -1) // Keep or Switch (K/S)
                {
                    if (input == "K") return true; // Keep
                    if (input == "S") return false; // Switch
                }
                else if (reveal) // Reveal secrets (Y/N)
                {
                    if (input == "Y") return true;
                    if (input == "N") return false;
                }
                else if (exit) // Another round or Exit (A/E)
                {
                    if (input == "A") return true; // Play Another
                    if (input == "E") return false; // Exit
                }

                Console.WriteLine("Invalid option. Please try again.");
            }
        }

        public CrngResult RequestCrngValue(int range)
        {
            return _crngProtocol.Generate(range);
        }

        public void RequestProtocolReveal(CrngResult result)
        {
            _crngProtocol.RevealProtocol(result);
        }
    }
}
