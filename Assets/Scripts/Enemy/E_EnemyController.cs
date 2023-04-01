using UnityEngine;

interface IEnemy
{
    void ScanForPlayer();
    void FindPlayer();
    void Listen();
    void MoveTo(Transform _location);
}

public class E_EnemyController : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
