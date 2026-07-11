using System;
using System.Collections.Generic;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public class ConstructionsModel : GameModel<ConstructionsModel, IConstructionsModel>, IConstructionsModel
    {
        private readonly List<IConstructionModel> _constructions = new();

        public event Action<IConstructionModel> ConstructionAdded;

        public event Action<ConstructionType> ConstructionBuildRequested;

        public IReadOnlyList<IConstructionModel> Constructions => _constructions;

        public void AddConstruction(IConstructionModel constructionModel)
        {
            if (!constructionModel.EnoughWorkers()) return;

            CurrenciesModel.Instance.Followers.Value -= constructionModel.NeedWorkers(); 
            _constructions.Add(constructionModel);
            ConstructionAdded?.Invoke(constructionModel);
        }

        public void RequestConstructionBuild(ConstructionType constructionType)
        {
            ConstructionBuildRequested?.Invoke(constructionType);
        }

        public bool TryGetConstruction(
            ConstructionType constructionType,
            out IConstructionModel constructionModel)
        {
            foreach (var construction in _constructions)
            {
                if (construction.ConstructionType == constructionType)
                {
                    constructionModel = construction;

                    return true;
                }
            }

            constructionModel = null;

            return false;
        }
    }
}
