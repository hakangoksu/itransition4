using System;
using System.Collections.Generic;

namespace RickAndMortyGame.GameCore
{
    public interface IMorty
    {
        string Name { get; }

        double CalculateTheoreticalWinProbability(int boxesCount, bool shouldSwitch);

        List<int> RevealBoxes(int boxesCount, int portalGunBoxIndex, int rickSelectedBoxIndex, IGameCore gameCore);

        bool ShouldRevealBoxes(int boxesCount, int portalGunBoxIndex, int rickSelectedBoxIndex) => true;

        void TalkMorty(string MortyText)
        {
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"{MortyText}");

            Console.ResetColor();
            Thread.Sleep(1500);
        }

        string GetMortyWinPhrase();

        string GetRickWinPhrase();

        string GetDecisionPhrase();

        string GetHideGunPhrase();

    }
}
