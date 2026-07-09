using UnityEngine;
using TMPro;
public class ParticleComponent : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float angleDegrees;
    [SerializeField] private float speed;
    public TextMeshProUGUI textLabel;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    void Update()
    {
        float x = Mathf.Cos(angleDegrees * Mathf.Deg2Rad);
        float y = Mathf.Sin(angleDegrees * Mathf.Deg2Rad);

        transform.position += new Vector3(x, y) * speed * Time.deltaTime;
    }
}
