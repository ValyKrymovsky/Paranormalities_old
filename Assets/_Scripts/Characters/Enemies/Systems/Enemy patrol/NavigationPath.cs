using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using DG.Tweening;

namespace MyCode.Enemy.Systems
{
    public class NavigationPath : MonoBehaviour
    {
        [Space]
        [Separator("Navigation Points")]
        [Space]

        [Header("Points")]
        [SerializeField] private NavigationPathPoint[] _navigationPoints;
        [Space]

        [Space]
        [Separator("Path Properties")]
        [Space]
        [SerializeField] private PathMode _pathMode;

        // Points
        public NavigationPathPoint[] NavigationPoints { get => _navigationPoints; set => _navigationPoints = value; }

        // Path
        public PathMode PathMode { get => _pathMode; set => _pathMode = value; }

        private void Awake()
        {

        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        public NavigationPathPoint NextPathPoint(NavigationPathPoint point)
        {
            for(int i = 0; i < _navigationPoints.Length; i++)
            {
                if (point != _navigationPoints[i]) continue;

                if (i == _navigationPoints.Length - 1) return null;

                return _navigationPoints[i + 1];
            }

            return null;
        }
    }
}
