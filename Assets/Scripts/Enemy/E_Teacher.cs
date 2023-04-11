using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;

public class E_Teacher : MonoBehaviour, IEnemy
{
    [SerializeField, Separator("Player", true)]
    private GameObject player;

    [SerializeField, Separator("Patrol", true)]
    private bool useLocationalPatrol;
    [SerializeField]
    private List<GameObject> locationalParentPatrolPoints;
    [SerializeField]
    private List<GameObject> parentPatrolPoints;
    [SerializeField]
    private GameObject currentParentPatrolPoint;
    [SerializeField]
    private List<GameObject> childPatrolPoints;
    [SerializeField]
    private GameObject currentChildPatrolPoint;

    [SerializeField]
    private bool patroling = false;
    [SerializeField]
    private bool newParentPatrolPoint = false;
    [SerializeField]
    private bool nextParentPatrolPoint = false;
    [SerializeField]
    private bool nextChildPatrolPoint = false;
    [SerializeField]
    private bool movingToPatrolPoint = false;

    [SerializeField]
    private float distanceToChildPatrolPoint;

    [SerializeField, Separator("Navigation", true)]
    private NavMeshAgent navigationAgent;

    [Separator("Noise scan", true)]
    [SerializeField] private float noiseScanRadius = 10f;
    
    private void Awake() {
        navigationAgent = GetComponent<NavMeshAgent>();
    }

    private void Start() {
        UpdateParentPatrolPoints();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void UpdateParentPatrolPoints()
    {
        parentPatrolPoints = GetAllParentPatrolPoints();
    }

    private void Update() {
        Patrol();
    }

    private void Patrol()
    {
        if (patroling)
        {
            if (currentParentPatrolPoint == null || nextParentPatrolPoint)
            {
                currentParentPatrolPoint = RandomParentPatrolPoint();
                childPatrolPoints = GetAllChildPatrolPoints(currentParentPatrolPoint);
                newParentPatrolPoint = true;
                nextParentPatrolPoint = false;
            }

            if (!movingToPatrolPoint)
            {
                if (newParentPatrolPoint)
                {
                    currentChildPatrolPoint = childPatrolPoints[0];
                    MoveTo(currentChildPatrolPoint.transform.position);
                    newParentPatrolPoint = false;
                    movingToPatrolPoint = true;
                    nextChildPatrolPoint = false;
                }
                else
                {
                    if (NextChildPatrolPoint() != null)
                    {
                        currentChildPatrolPoint = NextChildPatrolPoint();
                        MoveTo(currentChildPatrolPoint.transform.position);
                        movingToPatrolPoint = true;
                        nextChildPatrolPoint = false;
                    }
                    else
                    {
                        nextParentPatrolPoint = true;
                    }
                }
            }
            else
            {
                distanceToChildPatrolPoint = Vector3.Distance(transform.position, currentChildPatrolPoint.transform.position);
                if (distanceToChildPatrolPoint < 2f)
                {
                    movingToPatrolPoint = false;
                    nextChildPatrolPoint = true;
                }
            }
        }
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

    public List<GameObject> GetAllParentPatrolPoints()
    {
        if (useLocationalPatrol)
        {
            List<GameObject> allParentPatrolPoints = locationalParentPatrolPoints;
            return allParentPatrolPoints;
        }
        else
        {
            GameObject[] allPatrolPointsArray = GameObject.FindGameObjectsWithTag("Parent Patrol Point");
            List<GameObject> allPatrolPoints = allPatrolPointsArray.ToList();
            return allPatrolPoints;
        }
    }

    public GameObject RandomParentPatrolPoint()
    {
        int index = UnityEngine.Random.Range(0, parentPatrolPoints.Count);
        return parentPatrolPoints[index];
    }

    public GameObject NextParentPatrolPoint()
    {
        try
        {
            GameObject nextParentPatrolPoint = parentPatrolPoints[parentPatrolPoints.IndexOf(currentParentPatrolPoint) + 1];
            return nextParentPatrolPoint;
        }
        catch (ArgumentOutOfRangeException)
        {
            GameObject nextParentPatrolPoint = parentPatrolPoints[0];
            return nextParentPatrolPoint;
        }
        
    }

    public GameObject PreviousParentPatrolPoint()
    {
        try
        {
            GameObject nextParentPatrolPoint = parentPatrolPoints[parentPatrolPoints.IndexOf(currentParentPatrolPoint) - 1];
            return nextParentPatrolPoint;
        }
        catch (ArgumentOutOfRangeException)
        {
            GameObject nextParentPatrolPoint = parentPatrolPoints[parentPatrolPoints.Count - 1];
            return nextParentPatrolPoint;
        }
    }

    public List<GameObject> GetAllChildPatrolPoints(GameObject _parentPatrolPoint)
    {
        if (_parentPatrolPoint.TryGetComponent(out EnemyPatrolSystem patrolSystem))
        {
            List<GameObject> allChildPatrolPoints = patrolSystem.GetAllChildPatrolPoints();
            return allChildPatrolPoints;
        }
        return null;
    }

    public GameObject RandomChildPatrolPoint()
    {
        int index = UnityEngine.Random.Range(0, childPatrolPoints.Count);
        return childPatrolPoints[index];
    }

    public GameObject NextChildPatrolPoint()
    {
        try
        {
            GameObject nextChildPatrolPoint = childPatrolPoints[childPatrolPoints.IndexOf(currentChildPatrolPoint) + 1];
            return nextChildPatrolPoint;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }
    
    public GameObject PreviousChildPatrolPoint()
    {
        try
        {
            GameObject nextChildPatrolPoint = childPatrolPoints[childPatrolPoints.IndexOf(currentChildPatrolPoint) - 1];
            return nextChildPatrolPoint;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    public void MoveTo(Vector3 _location)
    {
        navigationAgent.SetDestination(_location);
    }
}
