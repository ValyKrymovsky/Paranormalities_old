using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using MyBox;

public class E_Hearing : MonoBehaviour
{
    /*
    [Separator("Hearing", true)]
    public float hearingRadius = 10f;
    [SerializeField]
    private LayerMask noiseLayerMask;
    public (GameObject noiseObject, float volume) currentNoiseObject;
    public bool heardNoise;
    public bool changedCurrentNoise;
    private int noiseQueueSize = 5;
    public List<(GameObject noiseObject, float volume, int id)> noiseQueue;
    public List<(GameObject noiseObject, float volume, int id)> investigatedNoiseList;

    private void Awake()
    {
        noiseQueue = new List<(GameObject noiseObject, float volume, int id)>();
        investigatedNoiseList = new List<(GameObject noiseObject, float volume, int id)>();
    }



    public IEnumerator Listen()
    {

        List<(GameObject noiseObject, float volume, int id)> allNoisesInRange;

        while (true)
        {
            allNoisesInRange = GetAllNoisesInRange();

            AddNewNoisesToQueue(allNoisesInRange);
            noiseQueue = SortNoisesInQueue();

            // Debug.Log("Listening");

            yield return new WaitForSeconds(.1f);
        }
        




    }

    public (GameObject noiseObject, float volume, int id) GetClosestNoise()
    {
        List<(GameObject noiseObject, float volume, int id)> allNoises = GetAllNoises();

        (GameObject noiseObject, float volume, int id) nearestNoise = (null, 0, 0);
        int iterationIndex = 0;

        foreach((GameObject noiseObject, float volume, int id) noise in allNoises)
        {
            if (iterationIndex == 0)
            {
                nearestNoise = noise;
            }

            float distanceToNearestNoise = Vector3.Distance(transform.position, nearestNoise.noiseObject.transform.position);
            float distanceToNoise = Vector3.Distance(transform.position, noise.noiseObject.transform.position);

            if (distanceToNoise < distanceToNearestNoise)
            {
                nearestNoise = noise;
            }

            iterationIndex++;
        }

        return nearestNoise;
    }

    public List<(GameObject noiseObject, float volume, int id)> GetAllNoises()
    {
        List<(GameObject noiseObject, float volume, int id)> allNoises = NoiseSystem.GetAllNoises();

        return allNoises;
    }
    
    public List<(GameObject noiseObject, float volume, int id)> GetAllNoisesInRange()
    {
        List<(GameObject noiseObject, float volume, int id)> allNoises = GetAllNoises();

        List<(GameObject noiseObject, float volume, int id)> allNoisesInRange = new List<(GameObject noiseObject, float volume, int id)>();

        foreach((GameObject noiseObject, float volume, int id) noise in allNoises)
        {
            if (Vector3.Distance(transform.position, noise.noiseObject.transform.position) - hearingRadius - noise.volume < 0)
            {
                allNoisesInRange.Add(noise);
            }
        }

        return allNoisesInRange;
    }

    public List<(GameObject noiseObject, float volume, int id)> AddNewNoisesToQueue(List<(GameObject noiseObject, float volume, int id)> noiseList)
    {
        foreach((GameObject noiseObject, float volume, int id) noise in noiseList)
        {
            if (!noiseQueue.Contains(noise) && !investigatedNoiseList.Contains(noise))
            {
                noiseQueue.Add(noise);
                noise.noiseObject.GetComponent<NoiseSystem>().investigating = true;
            }
        }

        return noiseQueue;
    }

    public List<(GameObject noiseObject, float volume, int id)> SortNoisesInQueue()
    {
        // noiseQueue.OrderBy(n => n.volume).ThenBy(n => Vector3.Distance(transform.position, n.noiseObject.transform.position));

        var orderedQueue = noiseQueue.OrderBy(noise => Vector3.Distance(transform.position, noise.noiseObject.transform.position)).ToList();

        return orderedQueue;
    }

    public IEnumerator investigatedNoiseListCleaner()
    {
        while (true)
        {
            foreach((GameObject noiseObject, float volume, int id) noise in investigatedNoiseList)
            {
                if (noise.noiseObject == null)
                {
                    investigatedNoiseList.Remove(noise);
                }
            }
        }
        
    }
    */
}
