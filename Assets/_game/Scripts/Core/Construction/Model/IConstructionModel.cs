using R3;
using UnityEngine;

namespace Game
{
    public interface IConstructionModel
    {
        ConstructionType ConstructionType { get; }

        ConstructionSettings Settings { get; }

        GameObject View { get; }

        ReadOnlyReactiveProperty<int> Level { get; }

        ReadOnlyReactiveProperty<float> BuildElapsedTime { get; }

        ReadOnlyReactiveProperty<bool> IsBuilt { get; }

        void AdvanceBuild(float deltaTime);

        void Upgrade();
    }
}
