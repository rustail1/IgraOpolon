using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public sealed class OutpostTowerView : MonoBehaviour
    {
        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Stats")]
        public int MaxHealth { get; private set; } = 100;

        [field: SerializeField]
        [field: MinValue(0)]
        [field: BoxGroup("Stats")]
        public int Attack { get; private set; } = 10;

        [field: SerializeField]
        [field: MinValue(0)]
        [field: BoxGroup("Stats")]
        public int Defense { get; private set; }

        public int Health { get; private set; }

        private void Awake()
        {
            Health = MaxHealth;
        }

        public void ApplyDamage(int damage)
        {
            var damageAfterDefense = Mathf.Max(damage - Defense, 0);
            Health = Mathf.Max(Health - damageAfterDefense, 0);
        }
    }
}
