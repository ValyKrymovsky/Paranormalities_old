using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum groundTypes
{
    concrete,
    wood,
    grass
}

public class P_SoundSystem : MonoBehaviour
{
    private P_Movement p_movement;
    private CharacterController ch_controller;
    private Coroutine walkCoroutine;
    private Coroutine sprintCoroutine;
    private Coroutine sneakCoroutine;


    [Header("Audio")]
    [SerializeField] 
    private AudioSource source;
    [SerializeField]
    private float minPitch = .8f;
    [SerializeField]
    private float maxPitch = 1.2f;
    [SerializeField]
    private float volume = 1f;
    [SerializeField]
    private groundTypes ground;

    [SerializeField]
    private List<AudioClip> footsteps_wood_walk;
    [SerializeField]
    private List<AudioClip> footsteps_wood_sprint;

    [SerializeField]
    private List<AudioClip> footsteps_concrete_walk;
    [SerializeField]
    private List<AudioClip> footsteps_concrete_sprint;

    [SerializeField]
    private List<AudioClip> footsteps_grass_walk;
    [SerializeField]
    private List<AudioClip> footsteps_grass_sprint;
    [SerializeField]
    private List<AudioClip> currentFootstepsList;
    private List<AudioClip> playedFootsteps;
    private moveAction currentAction;
    

    [SerializeField, Header("Noise")]
    private float noise;
    [SerializeField]
    private float defaultNoise = 1;
    [SerializeField]
    private float sprintMultiplier = 1.75f;
    [SerializeField]
    private float sneakMultiplier = .45f;

    private Dictionary<groundTypes, float> groundNoiseMultiplier = new Dictionary<groundTypes, float>();

    private void Awake() {
        p_movement = GetComponent<P_Movement>();
        ch_controller = GetComponent<CharacterController>();
        playedFootsteps = new List<AudioClip>();
        groundNoiseMultiplier.Add(groundTypes.concrete, 1f);
        groundNoiseMultiplier.Add(groundTypes.grass, .75f);
        groundNoiseMultiplier.Add(groundTypes.wood, 1.3f);
    }

    private void Update() {
        GetGroundType();
    }

    public float GetNoise()
    {
        return noise;
    }

    public void SetNoise(float _value)
    {
        noise = _value;
    }

    public void PlaySound()
    {
        if (!source.isPlaying || source.time >(source.clip.length * .5f))
        {
            AudioClip clip = SelectClip();
            if (clip != null)
            {
                float randomPitch = Random.Range(minPitch, maxPitch);
                source.pitch = randomPitch;
                source.volume = volume;
                source.clip = clip;
                source.Play();
                if (GetGroundType() == groundTypes.concrete)
                {
                    if (GetMoveAction() == moveAction.sprint)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(0).Value * sprintMultiplier;
                    }
                    else if (GetMoveAction() == moveAction.sneak)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(0).Value * sneakMultiplier;
                    }
                    else if (GetMoveAction() == moveAction.walk)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(0).Value;
                    }
                }
                else if (GetGroundType() == groundTypes.grass)
                {
                    if (GetMoveAction() == moveAction.sprint)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(1).Value * sprintMultiplier;
                    }
                    else if (GetMoveAction() == moveAction.sneak)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(1).Value * sneakMultiplier;
                    }
                    else if (GetMoveAction() == moveAction.walk)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(1).Value;
                    }
                }
                else if (GetGroundType() == groundTypes.wood)
                {
                    if (GetMoveAction() == moveAction.sprint)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(2).Value * sprintMultiplier;
                    }
                    else if (GetMoveAction() == moveAction.sneak)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(2).Value * sneakMultiplier;
                    }
                    else if (GetMoveAction() == moveAction.walk)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(2).Value;
                    }
                }

                SpawnNoiseObject(NoiseObjectGlobalClass.noiseObject, gameObject, GameObject.Find("Noise Objects"), gameObject.transform.position, noise);
            }
        }
    }

    private AudioClip SelectClip()
    {
        AudioClip selectedClip = null;

        if (GetAudioList() != null)
        {
            if (!currentFootstepsList.Equals(GetAudioList()))
            {
                playedFootsteps.Clear();
                currentFootstepsList = GetAudioList();
            }

            if (playedFootsteps.Count >= GetAudioList().Count )
            {
                playedFootsteps.Clear();
            }

            selectedClip = GetAudioList()[Random.Range(0, GetAudioList().Count)];

            while (playedFootsteps.Contains(selectedClip))
            {
                selectedClip = GetAudioList()[Random.Range(0, GetAudioList().Count)];

                if (!playedFootsteps.Contains(selectedClip))
                {
                    break;
                }
            }

            playedFootsteps.Add(selectedClip);

            return selectedClip;
        }
        else
        {
            return selectedClip;
        }
    }

    private groundTypes GetGroundType()
    {
        switch(p_movement.GetLayer())
        {
            case 8:
                ground = groundTypes.concrete;
                return ground;
            
            case 9:
                ground = groundTypes.wood;
                return ground;
            
            case 10:
                ground = groundTypes.grass;
                return ground;
        }
        
        return ground;
    }

    private moveAction GetMoveAction()
    {
        currentAction = p_movement.GetMoveAction();
        return currentAction;
    }

    private List<AudioClip> GetAudioList()
    {
        if (GetGroundType() == groundTypes.concrete)
        {
            if (GetMoveAction() == moveAction.walk || GetMoveAction() == moveAction.sneak)
            {
                return footsteps_concrete_walk;
            }
            else if (GetMoveAction() == moveAction.sprint)
            {
                return footsteps_concrete_sprint;
            }
            else
            {
                return null;
            }
        }
        else if (GetGroundType() == groundTypes.wood)
        {
            if (GetMoveAction() == moveAction.walk || GetMoveAction() == moveAction.sneak)
            {
                return footsteps_wood_walk;
            }
            else if (GetMoveAction() == moveAction.sprint)
            {
                return footsteps_wood_sprint;
            }
            else
            {
                return null;
            }
        }
        else if (GetGroundType() == groundTypes.grass)
        {
            if (GetMoveAction() == moveAction.walk || GetMoveAction() == moveAction.sneak)
            {
                return footsteps_grass_walk;
            }
            else if (GetMoveAction() == moveAction.sprint)
            {
                return footsteps_grass_sprint;
            }
            else
            {
                return null;
            }
        }

        return null;
    }


    public static void SpawnNoiseObject(GameObject _noiseObject, GameObject _instantiator, GameObject _parent, Vector3 _position, float _noise)
    {
        GameObject noiseObject = Instantiate(_noiseObject, _position, _instantiator.transform.rotation, _parent.transform);
        NoiseSystem noiseSys = noiseObject.GetComponent<NoiseSystem>();

        noiseSys.SetNoise(_noise);
        noiseSys.SetParent(_instantiator);
        noiseSys.tag = "Noise";
    }
}
