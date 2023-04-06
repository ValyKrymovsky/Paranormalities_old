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
    [SerializeField] private AudioSource source;
    [SerializeField] private float minPitch = .8f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float volume = 1f;
    [SerializeField] private groundTypes ground;

    // footsteps on wood surface //
    [SerializeField] private List<AudioClip> footsteps_wood_walk;
    [SerializeField] private List<AudioClip> footsteps_wood_sprint;

    // footsteps on concrete surface //
    [SerializeField] private List<AudioClip> footsteps_concrete_walk;
    [SerializeField] private List<AudioClip> footsteps_concrete_sprint;

    // footsteps on grass surface //
    [SerializeField] private List<AudioClip> footsteps_grass_walk;
    [SerializeField] private List<AudioClip> footsteps_grass_sprint;
    private List<AudioClip> playedFootsteps;
    private moveAction currentAction;
    

    [Header("Noise")]
    [SerializeField] private float noise;
    [SerializeField] private float defaultNoise = 1;
    [SerializeField] private float sprintMultiplier = 1.75f;
    [SerializeField] private float sneakMultiplier = .45f;

    private Dictionary<groundTypes, float> groundNoiseMultiplier = new Dictionary<groundTypes, float>();

    private void Awake() {
        p_movement = GetComponent<P_Movement>();
        ch_controller = GetComponent<CharacterController>();
        playedFootsteps = new List<AudioClip>();
        groundNoiseMultiplier.Add(groundTypes.concrete, 1f);
        groundNoiseMultiplier.Add(groundTypes.grass, .75f);
        groundNoiseMultiplier.Add(groundTypes.wood, 1.3f);
    }

    public float GetNoise()
    {
        return noise;
    }

    public void SetNoise(float _value)
    {
        noise = _value;
    }

    public void PlaySound(AudioClip _clip)
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;
        source.volume = volume;
        source.PlayOneShot(_clip);
    }

    public void PlaySound(AudioClip _clip, float _volume)
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;
        source.volume = _volume;
        source.PlayOneShot(_clip);
    }

    public AudioClip SelectClip(List<AudioClip> _clips)
    {
        AudioClip selectedClip = null;

        if (playedFootsteps.Count >= footsteps_concrete_walk.Count)
        {
            playedFootsteps.Clear();
        }
        selectedClip = _clips[Random.Range(0, _clips.Count)];

        while (playedFootsteps.Contains(selectedClip))
        {
            selectedClip = _clips[Random.Range(0, _clips.Count)];

            if (!playedFootsteps.Contains(selectedClip))
            {
                break;
            }
        }

        playedFootsteps.Add(selectedClip);

        return selectedClip;
    }

    private IEnumerator walkFootsteps(moveAction _moveAction, float _speed)
    {
        yield return new WaitForSeconds(.2f);
        while (true)
        {
            if (ground == groundTypes.concrete)
            {
                PlaySound(SelectClip(footsteps_concrete_walk), .7f);
            }
            else if (ground == groundTypes.wood)
            {
                PlaySound(SelectClip(footsteps_wood_walk), .8f);
            }
            else
            {
                PlaySound(SelectClip(footsteps_grass_walk), .4f);
            }
            yield return new WaitForSeconds(_speed);
        }
    }

    private IEnumerator sprintFootsteps(moveAction _moveAction, float _speed)
    {
        yield return new WaitForSeconds(.2f);
        while (true)
        {
            if (ground == groundTypes.concrete)
            {
                PlaySound(SelectClip(footsteps_concrete_sprint), .7f);
            }
            else if (ground == groundTypes.wood)
            {
                PlaySound(SelectClip(footsteps_wood_sprint), .8f);
            }
            else
            {
                PlaySound(SelectClip(footsteps_grass_sprint), .4f);
            }
            yield return new WaitForSeconds(_speed);
        }
    }

    private IEnumerator sneakFootsteps(moveAction _moveAction, float _speed)
    {
        yield return new WaitForSeconds(.2f);
        while (true)
        {
           if (ground == groundTypes.concrete)
            {
                PlaySound(SelectClip(footsteps_concrete_walk), .7f);
            }
            else if (ground == groundTypes.wood)
            {
                PlaySound(SelectClip(footsteps_wood_walk), .8f);
            }
            else
            {
                PlaySound(SelectClip(footsteps_grass_walk), .4f);
            }
            yield return new WaitForSeconds(_speed);
        }
    }

    private void Update()
    {
        currentAction = p_movement.GetMoveAction();
        
        if (currentAction == moveAction.walk)
        {
            GetGroundType();
            if (sprintCoroutine != null)
            {
                StopCoroutine(sprintCoroutine);
                sprintCoroutine = null;
                playedFootsteps.Clear();
            }

            if (sneakCoroutine != null)
            {
                StopCoroutine(sneakCoroutine);
                sneakCoroutine = null;
                playedFootsteps.Clear();
            }

            if (walkCoroutine == null)
            {
                playedFootsteps.Clear();
                walkCoroutine = StartCoroutine(walkFootsteps(currentAction, .6f));
            }
            noise = defaultNoise * groundNoiseMultiplier[GetGroundType()];
        }
        else if (currentAction == moveAction.sprint)
        {
            GetGroundType();
            if (walkCoroutine != null)
            {
                StopCoroutine(walkCoroutine);
                walkCoroutine = null;
                playedFootsteps.Clear();
            }

            if (sneakCoroutine != null)
            {
                StopCoroutine(sneakCoroutine);
                sneakCoroutine = null;
                playedFootsteps.Clear();
            }

            if (sprintCoroutine == null)
            {
                playedFootsteps.Clear();
                sprintCoroutine = StartCoroutine(sprintFootsteps(currentAction, .35f));
            }
            noise = defaultNoise * sprintMultiplier * groundNoiseMultiplier[GetGroundType()];
        }
        else if (currentAction == moveAction.sneak)
        {
            GetGroundType();
            if (walkCoroutine != null)
            {
                StopCoroutine(walkCoroutine);
                walkCoroutine = null;
                playedFootsteps.Clear();
            }

            if (sprintCoroutine != null)
            {
                StopCoroutine(sprintCoroutine);
                sprintCoroutine = null;
                playedFootsteps.Clear();
            }

            if (sneakCoroutine == null)
            {
                playedFootsteps.Clear();
                sneakCoroutine = StartCoroutine(sneakFootsteps(currentAction, 1f));
            }
            noise = defaultNoise * sneakMultiplier * groundNoiseMultiplier[GetGroundType()];
        }
        else
        {
            if (walkCoroutine != null)
            {
                StopCoroutine(walkCoroutine);
                walkCoroutine = null;
                playedFootsteps.Clear();
            }

            if (sprintCoroutine != null)
            {
                StopCoroutine(sprintCoroutine);
                sprintCoroutine = null;
                playedFootsteps.Clear();
            }

            if (sneakCoroutine != null)
            {
                StopCoroutine(sneakCoroutine);
                sneakCoroutine = null;
                playedFootsteps.Clear();
            }

            noise = 0;
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


    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, noise * 5);
    }
}
