using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(
        fileName = "EnemyWaveTable",
        menuName = "Game/Enemy Wave Table")]
    public sealed class EnemyWaveTable : ScriptableObject
    {
        [SerializeField]
        private CharactersTable _charactersTable;

        public IReadOnlyList<CharacterSettings> GetWave(
            LineCode lineCode,
            float elapsedTime)
        {
            if (_charactersTable == null)
            {
                return Array.Empty<CharacterSettings>();
            }

            var units = _charactersTable.Characters
                .OrderBy(x => x.GoldCost)
                .ToArray();

            if (units.Length == 0)
            {
                return Array.Empty<CharacterSettings>();
            }

            CharacterSettings weak = units[0];
            CharacterSettings medium = units[Mathf.Min(1, units.Length - 1)];
            CharacterSettings heavy = units[Mathf.Min(2, units.Length - 1)];

            var result = new List<CharacterSettings>();

            // Первые 60 секунд
            if (elapsedTime < 60f)
            {
                switch (lineCode)
                {
                    case LineCode.Mid:
                        result.Add(weak);
                        result.Add(weak);
                        break;

                    case LineCode.Top:
                        result.Add(weak);
                        break;
                }
            }
            // 60-120 секунд
            else if (elapsedTime < 120f)
            {
                switch (lineCode)
                {
                    case LineCode.Top:
                        result.Add(weak);
                        result.Add(medium);
                        break;

                    case LineCode.Mid:
                        result.Add(weak);
                        result.Add(weak);
                        result.Add(medium);
                        break;

                    case LineCode.Bot:
                        result.Add(medium);
                        break;
                }
            }
            // После 120 секунд
            else
            {
                switch (lineCode)
                {
                    case LineCode.Top:
                        result.Add(heavy);
                        result.Add(medium);
                        break;

                    case LineCode.Mid:
                        result.Add(heavy);
                        result.Add(heavy);
                        result.Add(medium);
                        break;

                    case LineCode.Bot:
                        result.Add(heavy);
                        result.Add(heavy);
                        break;
                }
            }

            return result;
        }
    }
}