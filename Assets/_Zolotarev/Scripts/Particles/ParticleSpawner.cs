using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] private ParticleComponent objectToSpawn;

    public void Spawn(Transform spawnPosition, string text, Color color)
    {
        var instance = Instantiate(objectToSpawn, spawnPosition.position, Quaternion.identity, spawnPosition);

        if (!string.IsNullOrEmpty(text) && instance.textLabel != null)
        {
            instance.textLabel.SetText(text);
            instance.textLabel.color = color;
        }
    }
}