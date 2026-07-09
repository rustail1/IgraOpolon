using UnityEngine;

namespace Game
{
    public class Tower : MonoBehaviour
    {
        public OutpostTeam Team;
        public int Damage = 100;
        public float ShootCoolDown = 1f;
        public float AttackRange = 100f;
        public float ProjectileVelocity = 10f;


        private float _lastShootTime;
        public GameObject ProjectilePrefab;
        private void Update()
        {
            if (Time.time < _lastShootTime + ShootCoolDown) return;


            CharacterView target = FindNearestEnemy();

            if (target == null) return;


            _lastShootTime = Time.time;

            transform.LookAt(target.transform);
            Shoot(target);
        }

        private CharacterView FindNearestEnemy()
        {
            CharacterView target = null;
            float bestDistance = AttackRange;

            var characters = FindObjectsByType<CharacterView>();

            foreach (var character in characters)
            {
                if (character == null || character.Team == Team)
                    continue;

                float distance = (character.transform.position - transform.position).magnitude;

                if (distance < bestDistance && character.Health > 0)
                {
                    bestDistance = distance;
                    target = character;
                    break;
                }
            }

            return target;
        }

        private void Shoot(CharacterView enemy)
        {
            if (enemy == null) return;

            if(ProjectilePrefab?.GetComponent<TowerProjectile>() != null)
            {
                var projectile = Instantiate(ProjectilePrefab, transform).GetComponent<TowerProjectile>();
                projectile.Direction = (enemy.transform.position - transform.position).normalized;
                projectile.Velocity = ProjectileVelocity;
            }
            CharacterCombatSystem.Instance?.ApplyDamage(enemy, Damage);
        }
    }
}