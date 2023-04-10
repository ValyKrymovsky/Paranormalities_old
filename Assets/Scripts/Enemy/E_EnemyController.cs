using UnityEngine;
using System.Collections.Generic;
interface IEnemy
{
    Transform FindPlayer();
    Transform Listen();
    List<GameObject> GetAllParentPatrolPoints();
    GameObject RandomParentPatrolPoint();
    GameObject NextParentPatrolPoint();
    GameObject PreviousParentPatrolPoint();
    List<GameObject> GetAllChildPatrolPoints(GameObject _parentPatrolPoint);
    GameObject RandomChildPatrolPoint();
    GameObject NextChildPatrolPoint();
    GameObject PreviousChildPatrolPoint();
    void MoveTo(Vector3 _location);
}

public class E_EnemyController : MonoBehaviour
{
}
