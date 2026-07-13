using UnityEngine;

public class GodHand : MonoBehaviour
{
    private bool isPunching = false;
    [HideInInspector] public bool IsPunched = false;
    [SerializeField] private Rigidbody island;
    [SerializeField] private float punchAngle = 75f;
    private Quaternion targetRotation;

    public static GodHand Instance;
    void Awake()
    {
        Instance = this;
        targetRotation = transform.rotation;
        island.isKinematic = true;
    }
    void Update()
    {
        if (isPunching)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 30f * Time.deltaTime);  
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isPunching = false;
                IsPunched = true;
            }
        }
    }

    public void Punch()
    {
        targetRotation = Quaternion.Euler(punchAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        island.isKinematic = false;
        isPunching = true;
    }
}