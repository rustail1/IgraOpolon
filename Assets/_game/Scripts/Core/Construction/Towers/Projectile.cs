using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Vector3 Direction;
    [HideInInspector] public float Velocity;
    [HideInInspector] public int Damage;
    
    private void Update()
    {
        transform.position += Direction * Velocity * Time.deltaTime;
        if (transform.position.y < 0) Destroy(gameObject);
    }
}
