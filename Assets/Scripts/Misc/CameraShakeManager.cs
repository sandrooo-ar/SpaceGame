using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    private CinemachineImpulseSource impulseSource;

    [Header("Shake Settings")]
    [SerializeField] private float forceVariation = 0.3f;   // Randomize strength
    [SerializeField] private Vector3 directionVariation = new Vector3(0.2f, 0.2f, 0.2f); // Randomize direction

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void CameraShake(float globalShakeForce)
    {
        // Randomize force
        float randomizedForce = globalShakeForce * Random.Range(1f - forceVariation, 1f + forceVariation);

        // Randomize direction
        Vector3 randomizedDir = new Vector3(
            Random.Range(-directionVariation.x, directionVariation.x),
            Random.Range(-directionVariation.y, directionVariation.y),
            Random.Range(-directionVariation.z, directionVariation.z)
        );

        impulseSource.m_DefaultVelocity = randomizedDir.normalized;

        impulseSource.GenerateImpulseWithForce(randomizedForce);
    }
}
