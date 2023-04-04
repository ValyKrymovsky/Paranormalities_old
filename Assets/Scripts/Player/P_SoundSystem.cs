using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float defaultNoise = 5;
    [SerializeField] private float sprintMultiplier = 1.75f;
    [SerializeField] private float sneakMultiplier = .45f;

    private void Awake() {
        p_movement = GetComponent<P_Movement>();
        ch_controller = GetComponent<CharacterController>();
        playedFootsteps = new List<AudioClip>();
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
        source.PlayOneShot(_clip);
    }

    public void PlaySound(AudioClip _clip, float _volume)
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;
        source.volume = _volume;
        source.PlayOneShot(_clip);
    }

    public AudioClip SelectClip(List<AudioClip> _clips, groundTypes _ground, moveAction _action)
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
                PlaySound(SelectClip(footsteps_concrete_walk, groundTypes.concrete, moveAction.walk));
            }
            else if (ground == groundTypes.wood)
            {
                PlaySound(SelectClip(footsteps_wood_walk, groundTypes.wood, moveAction.walk));
            }
            else
            {
                PlaySound(SelectClip(footsteps_grass_walk, groundTypes.grass, moveAction.walk));
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
                PlaySound(SelectClip(footsteps_concrete_sprint, groundTypes.concrete, moveAction.sprint));
            }
            else if (ground == groundTypes.wood)
            {
                PlaySound(SelectClip(footsteps_wood_sprint, groundTypes.wood, moveAction.sprint));
            }
            else
            {
                PlaySound(SelectClip(footsteps_grass_sprint, groundTypes.grass, moveAction.sprint));
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
                PlaySound(SelectClip(footsteps_concrete_walk, groundTypes.concrete, moveAction.sneak));
            }
            else if (ground == groundTypes.wood)
            {
                PlaySound(SelectClip(footsteps_wood_walk, groundTypes.wood, moveAction.sneak));
            }
            else
            {
                PlaySound(SelectClip(footsteps_grass_walk, groundTypes.grass, moveAction.sneak));
            }
            yield return new WaitForSeconds(_speed);
        }
    }

    private void Update()
    {
        currentAction = p_movement.GetMoveAction();
        
        if (currentAction == moveAction.walk)
        {
            CheckLayer();
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
                noise = defaultNoise;
            }
        }
        else if (currentAction == moveAction.sprint)
        {
            CheckLayer();
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
                noise = defaultNoise * sprintMultiplier;
            }
        }
        else if (currentAction == moveAction.sneak)
        {
            CheckLayer();
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
                noise = defaultNoise * sneakMultiplier;
            }
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

    private void CheckLayer()
    {
        switch(p_movement.GetLayer())
        {
            case 8:
                ground = groundTypes.concrete;
                break;
            
            case 9:
                ground = groundTypes.wood;
                break;
            
            case 10:
                ground = groundTypes.grass;
                break;
        }
    }
}
