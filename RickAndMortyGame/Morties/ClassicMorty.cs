using System;
using System.Collections.Generic;
using System.Linq;
using RickAndMortyGame.GameCore;

namespace RickAndMortyGame.Morties
{
    public class ClassicMorty : IMorty
    {
        public string Name => "Classic Morty";
        private readonly Random _rand = new Random();

        public string GetMortyWinPhrase() => "Aww jeez, Rick, I won! I guess I'm not so useless after all.";
        public string GetRickWinPhrase() => "G-geez, Rick, you got lucky. N-next time you won't be so slick.";
        public string GetHideGunPhrase() => "Rick! The portal gun is gone! …Uh, don’t be mad, I may have dropped it in the toilet.";
        public string GetDecisionPhrase() => "I'll just... randomly pick one, Rick. Hope it's the right one.";

        public double CalculateTheoreticalWinProbability(int boxesCount, bool shouldSwitch)
        {
            if (!shouldSwitch) return 1.0 / boxesCount;
            return (boxesCount - 1.0) / boxesCount;
        }

        public List<int> RevealBoxes(int boxesCount, int portalGunBoxIndex, int rickSelectedBoxIndex, IGameCore gameCore)
        {
            var validRevealIndices = Enumerable.Range(0, boxesCount)
                .Where(i => i != rickSelectedBoxIndex && i != portalGunBoxIndex)
                .ToList();

            int boxToKeepSecret;

            if (rickSelectedBoxIndex == portalGunBoxIndex)
            {
                int randomIndex = _rand.Next(validRevealIndices.Count);
                boxToKeepSecret = validRevealIndices[randomIndex];
            }
            else
            {
                boxToKeepSecret = portalGunBoxIndex;
                int _ = _rand.Next(boxesCount);
            }

            var boxesToReveal = Enumerable.Range(0, boxesCount)
                .Where(i => i != portalGunBoxIndex)  
                .Where(i => i != rickSelectedBoxIndex) 
                .Where(i => i != boxToKeepSecret) 
                .ToList();

            int n = boxesToReveal.Count;
            while (n > 1)
            {
                n--;
                int k = _rand.Next(n + 1);

                int value = boxesToReveal[k];
                boxesToReveal[k] = boxesToReveal[n];
                boxesToReveal[n] = value;
            }

            return boxesToReveal;
        }
    }
}