using RickAndMortyGame.Common;

namespace RickAndMortyGame.GameCore
{
    public interface IGameCore
    {
        CrngResult RequestCrngValue(int range);

        void RequestProtocolReveal(CrngResult result);
    }
}
