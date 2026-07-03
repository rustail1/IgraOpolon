using System;
using System.Collections.Generic;

namespace Game
{
    public interface IOutpostsModel
    {
        event Action<OutpostView, OutpostTeam> Captured;

        void Initialize(LevelView levelView, OutpostsSettings settings);

        void Tick(float deltaTime);

        void AddCharacter(OutpostView outpostView, CharacterView characterView);

        void RemoveCharacter(OutpostView outpostView, CharacterView characterView);

        IReadOnlyList<CharacterView> TakeCharacters(OutpostView outpostView, OutpostTeam team);

        OutpostTeam GetOwner(OutpostView outpostView);
    }
}
