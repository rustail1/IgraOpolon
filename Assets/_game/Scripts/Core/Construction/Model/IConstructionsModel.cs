using System;
using System.Collections.Generic;

namespace Game
{
    public interface IConstructionsModel
    {
        event Action<IConstructionModel> ConstructionAdded;

        event Action<ConstructionType> ConstructionBuildRequested;

        IReadOnlyList<IConstructionModel> Constructions { get; }

        void AddConstruction(IConstructionModel constructionModel);

        void RequestConstructionBuild(ConstructionType constructionType);

        bool TryGetConstruction(ConstructionType constructionType, out IConstructionModel constructionModel);
    }
}
