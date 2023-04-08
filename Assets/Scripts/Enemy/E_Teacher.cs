using UnityEngine;

public class E_Teacher : MonoBehaviour, IEnemy
{
    [Header("Noise scan")]
    [SerializeField] private float noiseScanRadius;
    
    public void FindPlayer()
    {
        throw new System.NotImplementedException();
    }

    public Transform Listen()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, noiseScanRadius);

        return transform;
    }

    public void MoveTo(Transform _location)
    {
        throw new System.NotImplementedException();
    }

    public void ScanForPlayer()
    {
        throw new System.NotImplementedException();
    }
}
