using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using MyBox;

public class E_Teacher : MonoBehaviour
{
    [Separator("Patrol", true)]
    [SerializeField]
    public bool useLocationalPatrol;
    [SerializeField]
    public List<GameObject> locationalParentPatrolPoints;
    [SerializeField]
    public List<GameObject> parentPatrolPoints;
    [SerializeField]
    public GameObject currentParentPatrolPoint;
    [SerializeField]
    public List<GameObject> childPatrolPoints;
    [SerializeField]
    public GameObject currentChildPatrolPoint;

    [SerializeField]
    public bool patroling = false;
    [SerializeField]
    public bool newParentPatrolPoint = false;
    [SerializeField]
    public bool nextParentPatrolPoint = false;
    [SerializeField]
    public bool nextChildPatrolPoint = false;
    [SerializeField]
    public bool movingToPatrolPoint = false;

    [SerializeField]
    public float distanceToChildPatrolPoint;

    [Separator("Navigation", true)]
    [SerializeField]
    public GameObject player;
    [SerializeField]
    public NavMeshAgent navigationAgent;
    [SerializeField]
    public Vector3 locationToMove;

    [Separator("Hearing", true)]
    [SerializeField] private float noiseScanRadius = 10f;

    [Separator("Sight", true)]
    [SerializeField]
    public float sightDistance = 5f;
    [SerializeField, Range(0, 360)]
    public float sightAngle = 75f;
    [SerializeField]
    public bool playerInSight;
    [SerializeField]
    public bool playerInRange;
    public Coroutine sightCheck;
    public Coroutine patrolCoroutine;
    [SerializeField]
    public LayerMask playerMask;
    [SerializeField]
    public LayerMask obstacleMask;

    [Separator("Damage", true)]
    [SerializeField]
    private float damage = 25f;
    [SerializeField]
    MyCode.Player.P_Health p_health;
    private Coroutine dealDamageCoroutine = null;
    
    private void Awake()
    {
        navigationAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        p_health = player.GetComponent<MyCode.Player.P_Health>();
    }

    private void Start()
    {
        UpdateParentPatrolPoints();
        sightCheck = StartCoroutine(CheckSightCoroutine());
    }

    private void Update()
    {
        playerInRange = PlayerInCloseRange();
        Transform noise = Listen();

        if (playerInRange && !p_health.IsDead())
        {
            if (dealDamageCoroutine == null)
            {
                dealDamageCoroutine = StartCoroutine(DealDamage(damage));
            }
        }

        if ((playerInSight || playerInRange) && !p_health.IsDead())
        {
            StopPatroling();
            MoveTo(player.transform.position);
        }
        else if (noise != null && !(playerInRange || playerInSight))
        {
            StopPatroling();
            MoveTo(noise.position);
            if (dealDamageCoroutine != null)
            {
                StopCoroutine(dealDamageCoroutine);
                dealDamageCoroutine = null;
            }
        }
        else
        {
            if (patrolCoroutine == null)
            {
                patrolCoroutine = StartCoroutine(Patrol());
            }
            if (dealDamageCoroutine != null)
            {
                StopCoroutine(dealDamageCoroutine);
                dealDamageCoroutine = null;
            }
        }
    }

    private IEnumerator DealDamage(float _damage)
    {
        while (true)
        {
            p_health.DealDamage(_damage);
            yield return new WaitForSeconds(3f);
        }

    }

    public void UpdateParentPatrolPoints() {
        parentPatrolPoints = GetAllParentPatrolPoints();
    }

    public IEnumerator Patrol() {
        while (true)
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
                    MoveTo(transform.position);
                    yield return new WaitForSeconds(1f);
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
                        MoveTo(transform.position);
                        yield return new WaitForSeconds(1f);
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
            yield return null;
        }
    }

    public void StopPatroling() {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }

        patroling = false;
        newParentPatrolPoint = false;
        nextParentPatrolPoint = false;
        nextChildPatrolPoint = false;
        movingToPatrolPoint = false;

        currentParentPatrolPoint = null;
        childPatrolPoints = null;
        currentChildPatrolPoint = null;
    }

    public Transform FindPlayer() {
        if (player != null)
        {
            return player.transform;
        }
        return null;
    }

    public Transform Listen() {
        (GameObject noiseObject, float volume, int id) earliestNoise = NoiseSystem.GetEarliestNoise();

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

    public List<GameObject> GetAllParentPatrolPoints() {
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

    public GameObject RandomParentPatrolPoint() {
        int index = UnityEngine.Random.Range(0, parentPatrolPoints.Count);
        return parentPatrolPoints[index];
    }

    public GameObject ParentPatrolPointNearLocation(Vector3 _location) {
        float lowestDistance = 100;
        int parentIndex = 0;
        int index = 0;
        foreach (GameObject parentPoint in parentPatrolPoints)
        {
            float distance = Vector3.Distance(_location, parentPoint.transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                parentIndex = index;
            }
            index++;
        }
        return parentPatrolPoints[parentIndex];
    }

    public GameObject NextParentPatrolPoint() {
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

    public GameObject PreviousParentPatrolPoint() {
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

    public List<GameObject> GetAllChildPatrolPoints(GameObject _parentPatrolPoint) {
        if (_parentPatrolPoint.TryGetComponent(out EnemyPatrolSystem patrolSystem))
        {
            List<GameObject> allChildPatrolPoints = patrolSystem.GetAllChildPatrolPoints();
            return allChildPatrolPoints;
        }
        return null;
    }

    public GameObject RandomChildPatrolPoint() {
        int index = UnityEngine.Random.Range(0, childPatrolPoints.Count);
        return childPatrolPoints[index];
    }

    public GameObject NextChildPatrolPoint() {
        try {
            GameObject nextChildPatrolPoint = childPatrolPoints[childPatrolPoints.IndexOf(currentChildPatrolPoint) + 1];
            return nextChildPatrolPoint;
        }
        catch (ArgumentOutOfRangeException) {
            return null; 
        }
    }
    
    public GameObject PreviousChildPatrolPoint() {
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

    public void MoveTo(Vector3 _location) {
        locationToMove = _location;
        navigationAgent.SetDestination(_location);
    }

    public IEnumerator CheckSightCoroutine() {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            CheckSight();
        }
    }

    public void CheckSight() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightDistance, playerMask);

        if (hitColliders.Length != 0)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < sightAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    playerInSight = true;
                }
                else
                {
                    playerInSight = false;
                }
            }
            else
            {
                playerInSight = false;
            }
        }
        else
        {
            playerInSight = false;
        }
    }

    public bool PlayerInCloseRange() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2, playerMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
