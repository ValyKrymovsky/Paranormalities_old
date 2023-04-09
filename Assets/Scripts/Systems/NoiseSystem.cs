using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GlobalNoiseSystem
{
    public static GameObject noiseObject;
}

public class NoiseSystem : MonoBehaviour
{
    [SerializeField, Header("Noise")]
    private float noise;

    [SerializeField, Header("Age")]
    private float age = 0;
    [SerializeField]
    private float maxAge = 100;
    private Coroutine agingCoroutine;

    [SerializeField, Header("Parent info")]
    private GameObject parentObject;
    
    private void Start() {
        agingCoroutine = StartCoroutine(StartAging());
    }

    private void Update()
    {
        if (age > maxAge)
        {
            StopCoroutine(agingCoroutine);
            agingCoroutine = null;
            Object.Destroy(gameObject);
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


    public static (GameObject, float) GetEarliestNoise()
    {
        GameObject[] noiseList;
        List<float> ageList = new List<float>();
        noiseList = GameObject.FindGameObjectsWithTag("Noise");

        foreach (GameObject noiseObject in noiseList)
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

        foreach (GameObject noiseObject in noiseList)
        {
            if (noiseObject.TryGetComponent(out NoiseSystem systemObject))
            {
                if (systemObject.age == ageList.Min())
                {
                    return (noiseObject, systemObject.noise);
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

        return (null, 0);
    }
}