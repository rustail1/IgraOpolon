using Game;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    [HideInInspector] public Vector3 Direction;
    [HideInInspector] public float Velocity;
    [HideInInspector] public int Damage;
    private void Update()
    {
        transform.position += Direction * Velocity * Time.deltaTime;
    }
    private void OnCollisionEnter(Collision collision)
    {
        var target = collision.gameObject.GetComponent<CharacterView>();

        if (target != null) CharacterCombatSystem.Instance?.ApplyDamage(target, Damage);
        Destroy(gameObject);
    }
}
