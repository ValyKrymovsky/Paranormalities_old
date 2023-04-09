using UnityEngine;
using UnityEngine.AI;

public class E_Teacher : MonoBehaviour, IEnemy
{
    [SerializeField, Header("Player")]
    private GameObject player;

    [SerializeField, Header("Navigation")]
    private NavMeshAgent navigationAgent;

    [Header("Noise scan")]
    [SerializeField] private float noiseScanRadius = 10f;
    
    private void Awake() {
        navigationAgent = GetComponent<NavMeshAgent>();
    }


    public Transform FindPlayer()
    {
        if (player != null)
        {
            return player.transform;
        }
        return null;
    }

    public Transform Listen()
    {
        (GameObject, float) earliestNoise = NoiseSystem.GetEarliestNoise();

        if (earliestNoise.Item1 != null)
        {
            bool noiseInRange = Vector3.Distance(transform.position, earliestNoise.Item1.transform.position) - noiseScanRadius - earliestNoise.Item2 < 0 ? true : false;
            if (noiseInRange)
            {
                return earliestNoise.Item1.transform;
            }
            return null;
        }
        return null;
        
    }

    public void MoveTo(Transform _location)
    {
        navigationAgent.SetDestination(_location.position);
    }

    private void Update() {
        Transform noiseLocation = Listen();
        if (noiseLocation != null)
        {
            MoveTo(noiseLocation);
        }
    }
}
