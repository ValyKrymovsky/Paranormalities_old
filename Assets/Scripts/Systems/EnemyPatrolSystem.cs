using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolSystem : MonoBehaviour
{
    [SerializeField, Header("Patrol points")]
    private List<GameObject> childPatrolPoints = new List<GameObject>();

    private void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.CompareTag("Child Patrol Point"))
            {
                childPatrolPoints.Add(child.gameObject);
            }
        }
    }

    public List<GameObject> GetAllChildPatrolPoints()
    {
        return childPatrolPoints;
    }
}
