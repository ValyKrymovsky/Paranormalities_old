using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;

namespace MyCode.Systems
{
    public class NoiseSystem : MonoBehaviour
    {
        [Separator("Noise", true)]
        [SerializeField]
        private float noise;
        [SerializeField]
        private int randomId;

        [Separator("Age", true)]
        [SerializeField]
        private float age = 0;
        [SerializeField]
        private float maxAge = 100;
        public bool investigating;
        private Coroutine agingCoroutine;

        [Separator("Parent info", true)]
        [SerializeField]
        private GameObject parentObject;

        private void Start()
        {
            randomId = Random.Range(1000, 10000);
            agingCoroutine = StartCoroutine(StartAging());
            investigating = false;
        }

        private void Update()
        {
            if (age > maxAge)
            {
                if (agingCoroutine != null)
                {
                    StopCoroutine(agingCoroutine);
                    agingCoroutine = null;
                }

                if (!investigating)
                {
                    Object.Destroy(gameObject);
                }
            }
        }

        public float GetAge()
        {
            return age;
        }

        public void SetAge(float _age)
        {
            age = _age;
        }

        public float GetMaxAge()
        {
            return maxAge;
        }

        public void SetMaxAge(float _maxAge)
        {
            maxAge = _maxAge;
        }

        public float GetNoise()
        {
            return noise;
        }

        public void SetNoise(float _noise)
        {
            noise = _noise;
        }

        public void SetParent(GameObject _parent)
        {
            parentObject = _parent;
        }

        private IEnumerator StartAging()
        {
            while (age < maxAge)
            {
                age += .1f;
                yield return null;
            }

        }

        public static (GameObject noiseObject, float volume, int id) GetEarliestNoise()
        {
            GameObject[] noiseArray;
            List<float> ageList = new List<float>();
            noiseArray = GameObject.FindGameObjectsWithTag("Noise");

            foreach (GameObject noiseObject in noiseArray)
            {
                if (noiseObject.TryGetComponent(out NoiseSystem systemObject))
                {
                    ageList.Add(systemObject.age);
                }
                else
                {
                    continue;
                }
            }

            foreach (GameObject noiseObject in noiseArray)
            {
                if (noiseObject.TryGetComponent(out NoiseSystem systemObject))
                {
                    if (systemObject.age == ageList.Min())
                    {
                        return (noiseObject, systemObject.noise, systemObject.randomId);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            return (null, 0, 0);
        }

        public static List<(GameObject noiseObject, float volume, int id)> GetAllNoises()
        {
            GameObject[] noiseArray;
            noiseArray = GameObject.FindGameObjectsWithTag("Noise");

            List<(GameObject noiseObject, float volume, int id)> allNoises = new List<(GameObject noiseObject, float volume, int id)>();

            foreach (GameObject noiseObject in noiseArray)
            {
                if (noiseObject.TryGetComponent(out NoiseSystem systemObject))
                {
                    allNoises.Add((noiseObject, systemObject.noise, systemObject.randomId));
                }
                else
                {
                    continue;
                }
            }

            return allNoises;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, noise);
        }
    }
}
