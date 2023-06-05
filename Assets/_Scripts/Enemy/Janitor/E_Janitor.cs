using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(NavMeshAgent))]
public class E_Janitor : MonoBehaviour
{
    [Separator("Speed", true)]

    [SerializeField]
    private float patrolSpeed;
    [SerializeField]
    private float chaseSpeed;

    [Separator("Attack", true)]

    [SerializeField]
    private float damageAmount;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float attackCooldown;
    [SerializeField]
    private bool inAttackRange;
    private Coroutine attackCoroutine;
    private P_Health p_health;

    [Separator("Navigation", true)]

    [Header("Player")]
    [SerializeField]
    private GameObject player;
    private P_Movement p_movement;
    private P_Camera p_camera;
    private Vector3 playerInputDirection;
    private movementDirection playerMovementDirection;

    [Header("Navigation")]
    [SerializeField]
    private NavMeshAgent navigationAgent;
    private Vector3 locationToMove;
    [Header("Interception")]
    private Vector3 interceptionPoint;
    private Vector3 secondInterceptionPoint;
    private float interceptionsPointsCheckDistance = .25f;
    private bool interceptionPointReached;
    [SerializeField]
    private float interceptionPointMaxDistance;

    [Header("Patrol")]
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

    [Separator("Hearing", true)]
    [SerializeField]
    private bool movingToNoise;
    private E_Hearing e_hearing;
    private Coroutine listenCoroutine;

    [Separator("Sight", true)]
    
    public float sightDistance = 5f;
    [Range(0, 360)]
    public float sightAngle = 75f;
    public bool playerInSight;
    public Coroutine sightCheck;
    public Coroutine patrolCoroutine;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    

    public void Awake()
    {
        navigationAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        p_movement = player.GetComponent<P_Movement>();
        p_camera = player.GetComponent<P_Camera>();
        p_health = player.GetComponent<P_Health>();

        e_hearing = GetComponent<E_Hearing>();
        
        navigationAgent.speed = patrolSpeed;
    }

    private void Start()
    {
        playerMovementDirection = p_movement.GetMovementDirection();
        UpdateParentPatrolPoints();
        sightCheck = StartCoroutine(CheckSightCoroutine());
        listenCoroutine = StartCoroutine(e_hearing.Listen());
    }

    private void Update()
    {
        inAttackRange = PlayerInAttackRange();
        
        // foreach((GameObject noiseObject, float volume, int id) noiseObject in e_hearing.noiseQueue)
        // {
        //     Debug.Log(noiseObject);
        // }

        // Debug.Log("\r\n ---------------------------- \r\n");

        if (e_hearing.noiseQueue.Count != 0)
        {
            (GameObject noiseObject, float volume, int id) noise = e_hearing.noiseQueue[0];
            Debug.Log(noise);
            navigationAgent.SetDestination(noise.noiseObject.transform.position);

            Debug.DrawLine(transform.position, noise.noiseObject.transform.position);

            if (Vector3.Distance(transform.position, noise.noiseObject.transform.position) <= 1)
            {
                noise.noiseObject.GetComponent<NoiseSystem>().investigating = false;
                e_hearing.investigatedNoiseList.Add(noise);
                e_hearing.noiseQueue.RemoveAt(0);
                Debug.Log("reached noise!");
            }
        }
        


        // if (playerInSight)
        // {
        //     StopPatroling();
        //     CalculateInterceptionPoint();
        //     movingToNoise = false;
        // }
        // else if (e_hearing.noiseQueue.Count != 0 && !movingToNoise)
        // {
        //     // foreach((GameObject noiseObject, float volume) noiseObject in e_hearing.GetAllNoisesInRange())
        //     // {
        //     //     Debug.Log(noiseObject);
        //     // }

        //     // Debug.Log("\r\n ---------------------------- \r\n");

        //     navigationAgent.SetDestination(e_hearing.noiseQueue[0].noiseObject.transform.position);
            
        // }
        // else if()

        // if (inAttackRange && !p_health.IsDead())
        // {
        //     if (attackCoroutine == null)
        //     {
        //         attackCoroutine = StartCoroutine(Attack());
        //     }
        // }

        // if ((playerInSight || playerInRange) && !p_health.IsDead())
        // {
        //     StopPatroling();
        //     MoveTo(player.transform.position);
        // }
        // else if (noise != null && !(playerInRange || playerInSight))
        // {
        //     StopPatroling();
        //     MoveTo(noise.position);
        //     if (dealDamageCoroutine != null)
        //     {
        //         StopCoroutine(dealDamageCoroutine);
        //         dealDamageCoroutine = null;
        //     }
        // }
        // else
        // {
        //     if (patrolCoroutine == null)
        //     {
        //         patrolCoroutine = StartCoroutine(Patrol());
        //     }
        //     if (dealDamageCoroutine != null)
        //     {
        //         StopCoroutine(dealDamageCoroutine);
        //         dealDamageCoroutine = null;
        //     }
        // }
    }

    private void CalculateInterceptionPoint()
    {
        inAttackRange = Vector3.Distance(player.transform.position, transform.position) > attackRange ? false : true;

        Vector3 directionToEnemy = transform.position - p_movement.transform.position;

        movementDirection playerMovementDirectionLocal = p_movement.GetMovementDirection();

        if (!inAttackRange)
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            if (playerMovementDirectionLocal == movementDirection.none)
            {
                float attackDistanceMultiplier = attackRange/Vector3.Distance(player.transform.position, transform.position);
                interceptionPoint = player.transform.position + (directionToEnemy * (attackDistanceMultiplier * .8f));
                Debug.DrawLine(transform.position, interceptionPoint, Color.green);

                if (playerMovementDirection != movementDirection.none)
                {
                    playerMovementDirection = movementDirection.none;
                    interceptionPointReached = false;
                }
            }
            else
            {
                float angleToEnemy = GetAgnleFromPlayer(player);

                // Debug.Log(angleToEnemy);
                
                if (angleToEnemy <= 180)
                {
                    Vector3 playerMoveVector = new Vector3(p_movement.GetFinalPlayerDirectionToMoveTo().x, 0, p_movement.GetFinalPlayerDirectionToMoveTo().z);
                    float distanceEnemyPlayer = Vector3.Distance(transform.position, p_movement.transform.position) > interceptionPointMaxDistance ? interceptionPointMaxDistance : Vector3.Distance(transform.position, p_movement.transform.position);
                    float secondInterceptionPointDistance = (distanceEnemyPlayer / 1.5f) * (p_movement.GetCurrentSpeed() / 2) * (1 - (angleToEnemy / 360));
                    secondInterceptionPointDistance = secondInterceptionPointDistance > interceptionPointMaxDistance ? interceptionPointMaxDistance : secondInterceptionPointDistance;
                    secondInterceptionPointDistance = secondInterceptionPointDistance < attackRange ? attackRange : secondInterceptionPointDistance;

                    secondInterceptionPoint = player.transform.position + playerMoveVector.normalized * secondInterceptionPointDistance;

                    if (Vector3.Distance(interceptionPoint, secondInterceptionPoint) >= interceptionsPointsCheckDistance)
                    {
                        interceptionPoint = secondInterceptionPoint;
                        p_movement.directionSphere.transform.position = interceptionPoint;
                    }


                    if (playerMovementDirectionLocal != playerMovementDirection)
                    {
                        float interceptionPointDistance = (distanceEnemyPlayer / 2) * (1 + (1 - (angleToEnemy / 360) * 2));
                        interceptionPointDistance = interceptionPointDistance > interceptionPointMaxDistance ? interceptionPointMaxDistance : interceptionPointDistance;
                        interceptionPointDistance = interceptionPointDistance < attackRange ? attackRange : interceptionPointDistance;

                        interceptionPoint = player.transform.position + playerMoveVector.normalized * interceptionPointDistance;
                        p_movement.directionSphere.transform.position = interceptionPoint;
                        Debug.DrawLine(transform.position, interceptionPoint, Color.blue);
                        playerMovementDirection = playerMovementDirectionLocal;
                        interceptionPointReached = false;
                    }

                    if (Vector3.Distance(transform.position, interceptionPoint) < .5f)
                    {
                        interceptionPointReached = true;
                        float attackDistanceMultiplier = attackRange/Vector3.Distance(player.transform.position, transform.position);
                        interceptionPoint = player.transform.position + (directionToEnemy * (attackDistanceMultiplier * .8f));
                    }

                    if (interceptionPointReached)
                    {
                        float attackDistanceMultiplier = attackRange/Vector3.Distance(player.transform.position, transform.position);
                        interceptionPoint = player.transform.position + (directionToEnemy * (attackDistanceMultiplier * .8f));
                        Debug.DrawLine(transform.position, interceptionPoint, Color.blue);
                    }
                }
                else
                {
                    float attackDistanceMultiplier = attackRange/Vector3.Distance(player.transform.position, transform.position);
                    interceptionPoint = player.transform.position + (directionToEnemy * (attackDistanceMultiplier * .8f));
                    Debug.DrawLine(transform.position, interceptionPoint, Color.red);
                    interceptionPointReached = false;
                }
            }

            
        }
        else
        {
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(Attack());
            }

            float attackDistanceMultiplier = attackRange/Vector3.Distance(player.transform.position, transform.position);
            interceptionPoint = player.transform.position + (directionToEnemy * (attackDistanceMultiplier * .8f));
            Debug.DrawLine(transform.position, interceptionPoint, Color.red);
            interceptionPointReached = false;

            transform.LookAt(player.gameObject.GetComponentInChildren<Camera>().transform.position);
        }

        navigationAgent.SetDestination(interceptionPoint);
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            Debug.Log("Attack");
            yield return new WaitForSeconds(attackCooldown);
        }
        
    }

    private float GetAgnleFromPlayer(GameObject player)
    {
        float enemyDotProduct;
        float angleFromPlayer;
        Vector3 directionFromPlayerToEnemy = transform.position - p_movement.transform.position;

        if (p_movement.GetMovementDirection() == movementDirection.none)
        {
            Vector3 playerForwardVector = p_movement.transform.TransformDirection(Vector3.forward);
            enemyDotProduct = Vector3.Dot(playerForwardVector.normalized, directionFromPlayerToEnemy.normalized);
        }
        else
        {
            Vector3 playerMoveVector = new Vector3(p_movement.GetFinalPlayerDirectionToMoveTo().x, 0, p_movement.GetFinalPlayerDirectionToMoveTo().z);
            enemyDotProduct = Vector3.Dot(playerMoveVector.normalized, directionFromPlayerToEnemy.normalized);
        }

        angleFromPlayer = Mathf.InverseLerp(1, -1, enemyDotProduct) * 360;

        return angleFromPlayer;
    }

    public void UpdateParentPatrolPoints() {
        parentPatrolPoints = GetAllParentPatrolPoints();
    }

    public IEnumerator Patrol()
    {
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
                    navigationAgent.SetDestination(transform.position);
                    yield return new WaitForSeconds(1f);
                    currentChildPatrolPoint = childPatrolPoints[0];
                    navigationAgent.SetDestination(currentChildPatrolPoint.transform.position);
                    newParentPatrolPoint = false;
                    movingToPatrolPoint = true;
                    nextChildPatrolPoint = false;
                }
                else
                {
                    if (NextChildPatrolPoint() != null)
                    {
                        navigationAgent.SetDestination(transform.position);
                        yield return new WaitForSeconds(1f);
                        currentChildPatrolPoint = NextChildPatrolPoint();
                        navigationAgent.SetDestination(currentChildPatrolPoint.transform.position);
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

    public void StopPatroling()
    {
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

    public Transform FindPlayer()
    {
        if (player != null)
        {
            return player.transform;
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

    public GameObject ParentPatrolPointNearLocation(Vector3 _location)
    {
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
        try {
            GameObject nextChildPatrolPoint = childPatrolPoints[childPatrolPoints.IndexOf(currentChildPatrolPoint) + 1];
            return nextChildPatrolPoint;
        }
        catch (ArgumentOutOfRangeException) {
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

    public IEnumerator CheckSightCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            playerInSight = PlayerInSight();
        }
    }

    public bool PlayerInSight()
    {
        if (!p_health.IsDead())
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightDistance, playerMask);

            if (hitColliders.Length != 0)
            {
                Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToPlayer) < sightAngle / 2)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                    if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                    {
                        // playerInSight = true;
                        return true;
                    }
                    else
                    {
                        // playerInSight = false;
                        return false;
                    }
                }
                else
                {
                    // playerInSight = false;
                    return false;
                }
            }
            else
            {
                // playerInSight = false;
                return false;
            }
        }
        return false;
    }

    public bool PlayerInAttackRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, e_hearing.hearingRadius);
    }
}