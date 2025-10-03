using System;

namespace RickAndMortyGame.GameCore
{
    public class GameStatistics
    {
        private int _totalGames = 0;
        private int _rickWinsOnKeep = 0;
        private int _rickWinsOnSwitch = 0;
        private int _rickLosses = 0;

        public void RecordOutcome(bool rickWon, bool switchedChoice)
        {
            _totalGames++;
            if (rickWon)
            {
                if (switchedChoice)
                {
                    _rickWinsOnSwitch++;
                }
                else
                {
                    _rickWinsOnKeep++;
                }
            }
            else
            {
                _rickLosses++;
            }
        }

        public double ExperimentalWinProbabilityKeep => _totalGames > 0 ? (double)_rickWinsOnKeep / TotalGames : 0.0;

        public double ExperimentalWinProbabilitySwitch => _totalGames > 0 ? (double)_rickWinsOnSwitch / TotalGames : 0.0;

        public int TotalGames => _totalGames;

        public string GetSummary(IMorty morty, int boxesCount)
        {
            double theoKeep = morty.CalculateTheoreticalWinProbability(boxesCount, false);
            double theoSwitch = morty.CalculateTheoreticalWinProbability(boxesCount, true);

            string summary = $"\n============================================\n";
            summary += $"             GAME SUMMARY REPORT\n";
            summary += $"============================================\n";
            summary += $"Total Rounds Played: {_totalGames}\n";
            summary += $"Total Rick Wins (Keep): {_rickWinsOnKeep}\n";
            summary += $"Total Rick Wins (Switch): {_rickWinsOnSwitch}\n";
            summary += $"Total Morty Wins (Rick Loss): {_rickLosses}\n";
            summary += $"--------------------------------------------\n";
            summary += $"EXPERIMENTAL PROBABILITIES:\n";
            summary += $"- Win probability (Keep): {ExperimentalWinProbabilityKeep:P2}\n";
            summary += $"- Win probability (Switch): {ExperimentalWinProbabilitySwitch:P2}\n";
            summary += $"--------------------------------------------\n";
            summary += $"THEORETICAL PROBABILITIES (Calculated by {morty.Name}):\n";
            summary += $"- Win probability (Keep): {theoKeep:P2} (1/{boxesCount})\n";
            summary += $"- Win probability (Switch): {theoSwitch:P2} ({boxesCount - 1}/{boxesCount})\n";
            summary += $"============================================\n";
            return summary;
        }
    }
}
