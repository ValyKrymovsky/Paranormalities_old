using MyBox;
using System;
using UnityEngine;

namespace MyCode.Enemy.Systems
{
    [Serializable]
    public class NavigationPathPoint : MonoBehaviour
    {

        [Space]
        [Separator("Path")]
        [Space]

        [SerializeField] private NavigationPath _mainPath;
        [Space]

        [Space]
        [Separator("Junction")]
        [Space]

        [SerializeField] private JunctionMode _junctionMode;
        [Space]

        [Tooltip("Only works if Junction Mode is set to Single or Multi")]
        [SerializeField] private NavigationPath[] _junctionPaths;
        [Space]

        [Space]
        [Separator("Properties")]
        [Space]

        [SerializeField] private bool _delay;
        [SerializeField, ConditionalField("_delay")] private float _delayTime;
        [SerializeField, ConditionalField("_delay")] private bool _randomDelay;
        [SerializeField, ConditionalField("_randomDelay")] private float _randomDelayTimeMin;
        [SerializeField, ConditionalField("_randomDelay")] private float _randomDelayTimeMax;


        // Path
        public NavigationPath MainPath { get => _mainPath; set => _mainPath = value; }

        // Junction
        public JunctionMode JunctionMode { get => _junctionMode; set => _junctionMode = value; }
        public NavigationPath[] JunctionPaths { get => _junctionPaths; set => _junctionPaths = value; }

        // Delay
        public bool Delay { get => _delay; set => _delay = value; }
        public float DelayTime { get => _delayTime; set => _delayTime = value; }
        public bool RandomDelay { get => _randomDelay; set => _randomDelay = value; }
        public float RandomDelayTimeMin { get => _randomDelayTimeMin; set => _randomDelayTimeMin = value; }
        public float RandomDelayTimeMax { get => _randomDelayTimeMax; set => _randomDelayTimeMax = value; }


        private void OnEnable()
        {
            _mainPath = transform.parent.GetComponent<NavigationPath>();
        }

        private void OnDisable()
        {
            _mainPath = null;
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "NavigationIcon.png", true);
        }
    }
}

