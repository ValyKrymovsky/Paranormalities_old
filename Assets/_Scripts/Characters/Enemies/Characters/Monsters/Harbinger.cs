using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode.Enemy.Systems;
using UnityEngine.AI;
using System.Linq;
using System;
using MyBox;

namespace MyCode.Enemy
{
    public class Harbinger : MonoBehaviour
    {
        public EnemyState enemyState = EnemyState.Patrolling;
        public PatrolState patrolState;
        private NavMeshAgent _agent;
        private Coroutine _patrolCoroutine;

        [SerializeField] private NavigationPathPoint[] _visitedPoints = new NavigationPathPoint[3];
        [SerializeField] private int _visitedPointsIndex = 0;

        [SerializeField] private NavigationPath[] _visitedPaths = new NavigationPath[2];
        [SerializeField] private int _visitedPathsIndex = 0;

        [SerializeField] private NavigationPath _currentNavPath;
        [SerializeField] private NavigationPathPoint _currentNavPathPoint;

        [SerializeField] private LayerMask _navigationMask;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _patrolCoroutine = StartCoroutine(Patrol());
        }

        private IEnumerator Patrol()
        {
            while (true)
            {
                switch(patrolState)
                {
                    case PatrolState.Decision:
                        if (_currentNavPath == null)
                        {
                            _currentNavPathPoint = FindNextPoint();
                            _currentNavPath = _currentNavPathPoint.MainPath;
                            patrolState = PatrolState.Patrol;
                            _agent.SetDestination(_currentNavPathPoint.transform.position);

                            _visitedPointsIndex = _visitedPointsIndex >= _visitedPoints.Length ? 0 : _visitedPointsIndex;
                            _visitedPoints[_visitedPointsIndex] = _currentNavPathPoint;
                            _visitedPointsIndex++;

                            _visitedPathsIndex = _visitedPathsIndex >= _visitedPaths.Length ? 0 : _visitedPathsIndex;
                            _visitedPaths[_visitedPathsIndex] = _currentNavPath;
                            _visitedPathsIndex++;
                            break;
                        }

                        NavigationPathPoint nextPoint = _currentNavPath.NextPathPoint(_currentNavPathPoint);
                        
                        if (nextPoint != null)
                        {
                            _currentNavPathPoint = nextPoint;
                            _agent.SetDestination(nextPoint.transform.position);
                            patrolState = PatrolState.Patrol;
                            break;
                        }

                        _currentNavPathPoint = FindNextPoint();
                        _currentNavPath = _currentNavPathPoint.MainPath;
                        patrolState = PatrolState.Patrol;
                        _agent.SetDestination(_currentNavPathPoint.transform.position);

                        _visitedPointsIndex = _visitedPointsIndex >= _visitedPoints.Length ? 0 : _visitedPointsIndex;
                        _visitedPoints[_visitedPointsIndex] = _currentNavPathPoint;
                        _visitedPointsIndex++;

                        _visitedPathsIndex = _visitedPathsIndex >= _visitedPaths.Length ? 0 : _visitedPathsIndex;
                        _visitedPaths[_visitedPathsIndex] = _currentNavPath;
                        _visitedPathsIndex++;

                        break;

                    case PatrolState.Patrol:
                        if (!ReachedPoint(_currentNavPathPoint.transform.position)) break;

                        patrolState = PatrolState.Decision;
                        break;
                }

                yield return new WaitForSeconds(.1f);
            }
        }

        private bool ReachedPoint(Vector3 point)
        {
            if ((point - transform.position).sqrMagnitude / 2 <= 1) return true;

            return false;
        }

        private NavigationPathPoint FindNextPoint()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 7.5f, _navigationMask);
            List<NavigationPathPoint> pathPoints = new List<NavigationPathPoint>(); 

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent<NavigationPathPoint>(out NavigationPathPoint point))
                {
                    if (point.MainPath.NavigationPoints.IndexOfItem(point) == point.MainPath.NavigationPoints.Length - 1) continue;
                    pathPoints.Add(point);
                }
            }
            /*
            for (int i = 0; i < _visitedPoints.Length; i++)
            {
                if (pathPoints.Contains(_visitedPoints[i])) pathPoints.Remove(_visitedPoints[i]);
            }
            */
            for (int i = 0; i < _visitedPaths.Length; i++)
            {
                if (_visitedPaths[i] == null) continue;
                if (!_visitedPaths[i].NavigationPoints.Intersect(pathPoints).Any()) continue;

                pathPoints.RemoveAll(p => _visitedPaths[i].NavigationPoints.Contains(p));
                /*
                foreach (NavigationPathPoint point in pathPoints.Where(p => _visitedPaths[i].NavigationPoints.Contains(p)))
                { 
                    pathPoints.Remove(point);
                }
                */
            }

            pathPoints.OrderBy(point => (point.transform.position - transform.position).sqrMagnitude / 2);
            int randomCap = pathPoints.Count < 3 ? pathPoints.Count : 3;
            return pathPoints[UnityEngine.Random.Range(0, randomCap)];
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 7.5f);
        }
    }
}

