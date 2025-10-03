using System;
using System.Collections.Generic;
using System.Linq;
using RickAndMortyGame.GameCore;

namespace RickAndMortyGame.Morties
{
    public class LazyMorty : IMorty
    {
        public string Name => "Lazy Morty";

        public string GetMortyWinPhrase() => "Ugh... I won? Okay, whatever, I'm going back to bed.";
        public string GetRickWinPhrase() => "I'm too tired to care, Rick. Just take your stupid win.";
        public string GetHideGunPhrase() => "It's hidden... somewhere. Can we be done now?";
        public string GetDecisionPhrase() => "I'll just pick the first available one. Too much work to think.";

        public double CalculateTheoreticalWinProbability(int boxesCount, bool shouldSwitch)
        {
            if (!shouldSwitch) return 1.0 / boxesCount;
            return (boxesCount - 1.0) / boxesCount;
        }

        public List<int> RevealBoxes(int boxesCount, int portalGunBoxIndex, int rickSelectedBoxIndex, IGameCore gameCore)
        {
            var candidatesForReveal = Enumerable.Range(0, boxesCount)
                .Where(i => i != rickSelectedBoxIndex)
                .Where(i => i != portalGunBoxIndex)
                .ToList();

            int boxToKeepSecret;

            if (rickSelectedBoxIndex == portalGunBoxIndex)
            {
                boxToKeepSecret = candidatesForReveal.Min();
            }
            else
            {
                boxToKeepSecret = portalGunBoxIndex;

            }

            var boxesToReveal = Enumerable.Range(0, boxesCount)
                .Where(i => i != portalGunBoxIndex) 
                .Where(i => i != rickSelectedBoxIndex) 
                .Where(i => i != boxToKeepSecret) 
                .ToList();

            return boxesToReveal;
        }
    }
}