using System.Collections.Generic;
using R3;

namespace Game
{
    public interface IPlayerSquadsModel
    {
        ReadOnlyReactiveProperty<int> Revision { get; }

        IReadOnlyList<CharacterSettings> GetQueuedCharacters(LineCode lineCode);

        bool TryQueueCharacter(LineCode lineCode, CharacterSettings characterSettings);

        bool TryRemoveQueuedCharacter(LineCode lineCode, int characterIndex);

        IReadOnlyList<CharacterSettings> TakeQueuedCharacters(LineCode lineCode);
    }
}
