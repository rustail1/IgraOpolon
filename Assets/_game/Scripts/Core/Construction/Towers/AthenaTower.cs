using UnityEngine;

namespace Game
{
    public class AthenaTower : Tower
    {
        [Header("Aura")]
        public float Radius = 10f;
        public float DamageMultiplier = 1.5f;

        private CharacterView[] _buffer;

        private void Update()
        {
            _buffer = FindObjectsByType<CharacterView>();

            foreach (var character in _buffer)
            {
                if (character == null || character.Team != Team) continue;
                var model = character.Model;

                float sqrDistance = (character.transform.position - transform.position).sqrMagnitude;


                if (sqrDistance <= Radius * Radius)
                {
                    model.DamageMultiplier = DamageMultiplier;
                }
                else model.DamageMultiplier = 1f;
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}