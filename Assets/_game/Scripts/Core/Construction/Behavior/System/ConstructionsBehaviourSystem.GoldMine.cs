using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed partial class ConstructionsBehaviourSystem
    {
        private readonly List<GoldMineConstructionBehaviour> _goldMineBehaviours = new();

        private void InitializeGoldMineBehaviour(
            IConstructionModel constructionModel,
            GoldMineConstructionBehaviour behaviour)
        {
            behaviour.NextProductionTime = Time.time + behaviour.Production.Interval;
            _goldMineBehaviours.Add(behaviour);
        }

        public void OnUpdate()
        {
            foreach (var behaviour in _goldMineBehaviours)
            {
                if (Time.time < behaviour.NextProductionTime)
                {
                    continue;
                }

                _resourcesModel.Add(CurrencyType.Gold, behaviour.Production.Amount);
                behaviour.NextProductionTime = Time.time + behaviour.Production.Interval;
            }
        }
    }
}
