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
    [SerializeField] private AudioClip[] footsteps_wood_walk;
    [SerializeField] private AudioClip[] footsteps_wood_sprint;

    // footsteps on concrete surface //
    [SerializeField] private AudioClip[] footsteps_concrete_walk;
    [SerializeField] private AudioClip[] footsteps_concrete_sprint;

    // footsteps on grass surface //
    [SerializeField] private AudioClip[] footsteps_grass_walk;
    [SerializeField] private AudioClip[] footsteps_grass_sprint;
    private List<AudioClip> playedFootsteps;
    private moveAction currentAction;
    

    [Header("Noise")]
    [SerializeField] private float noise;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float sneakMultiplier = .75f;

    private void Awake() {
        p_movement = GetComponent<P_Movement>();
        ch_controller = GetComponent<CharacterController>();
        playedFootsteps = new List<AudioClip>();
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

    private IEnumerator walkFootsteps(moveAction _moveAction, float _speed)
    {
        yield return new WaitForSeconds(.2f);
        while (true)
        {
            if (ground == groundTypes.concrete)
            {
                AudioClip selectedClip = null;

                if (_moveAction == moveAction.walk)
                {
                    if (playedFootsteps.Count >= footsteps_concrete_walk.Length)
                    {
                        playedFootsteps.Clear();
                    }
                    selectedClip = footsteps_concrete_walk[Random.Range(0, footsteps_concrete_walk.Length - 1)];
                }
                
                PlaySound(selectedClip);
                playedFootsteps.Add(selectedClip);
            }
            else if (ground == groundTypes.wood)
            {
                AudioClip selectedClip = null;

                if (_moveAction == moveAction.walk)
                {
                    if (playedFootsteps.Count >= footsteps_wood_walk.Length)
                    {
                        playedFootsteps.Clear();
                    }
                    selectedClip = footsteps_wood_walk[Random.Range(0, footsteps_wood_walk.Length - 1)];
                }
                
                PlaySound(selectedClip);
                playedFootsteps.Add(selectedClip);
            }
            else
            {
                AudioClip selectedClip = null;

                if (_moveAction == moveAction.walk)
                {
                    if (playedFootsteps.Count >= footsteps_grass_walk.Length)
                    {
                        playedFootsteps.Clear();
                    }
                    selectedClip = footsteps_grass_walk[Random.Range(0, footsteps_grass_walk.Length - 1)];
                }
                
                PlaySound(selectedClip);
                playedFootsteps.Add(selectedClip);
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
                AudioClip selectedClip = null;
                if (playedFootsteps.Count >= footsteps_concrete_sprint.Length)
                {
                    playedFootsteps.Clear();
                }
                selectedClip = footsteps_concrete_sprint[Random.Range(0, footsteps_concrete_sprint.Length - 1)];

                PlaySound(selectedClip);
                playedFootsteps.Add(selectedClip);
            }
            else if (ground == groundTypes.wood)
            {
                AudioClip selectedClip = null;
                if (playedFootsteps.Count >= footsteps_wood_sprint.Length)
                {
                    playedFootsteps.Clear();
                }
                selectedClip = footsteps_wood_sprint[Random.Range(0, footsteps_wood_sprint.Length - 1)];

                PlaySound(selectedClip);
                playedFootsteps.Add(selectedClip);
            }
            else
            {
                AudioClip selectedClip = null;
                if (playedFootsteps.Count >= footsteps_grass_sprint.Length)
                {
                    playedFootsteps.Clear();
                }
                selectedClip = footsteps_grass_sprint[Random.Range(0, footsteps_grass_sprint.Length - 1)];

                PlaySound(selectedClip);
                playedFootsteps.Add(selectedClip);
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
                AudioClip selectedClip = null;
                if (playedFootsteps.Count >= footsteps_concrete_walk.Length)
                {
                    playedFootsteps.Clear();
                }
                selectedClip = footsteps_concrete_walk[Random.Range(0, footsteps_concrete_walk.Length - 1)];

                PlaySound(selectedClip, .7f);
                playedFootsteps.Add(selectedClip);
            }
            else if (ground == groundTypes.wood)
            {
                AudioClip selectedClip = null;
                if (playedFootsteps.Count >= footsteps_wood_walk.Length)
                {
                    playedFootsteps.Clear();
                }
                selectedClip = footsteps_wood_walk[Random.Range(0, footsteps_wood_walk.Length - 1)];

                PlaySound(selectedClip, .7f);
                playedFootsteps.Add(selectedClip);
            }
            else
            {
                AudioClip selectedClip = null;
                if (playedFootsteps.Count >= footsteps_grass_walk.Length)
                {
                    playedFootsteps.Clear();
                }
                selectedClip = footsteps_grass_walk[Random.Range(0, footsteps_grass_walk.Length - 1)];

                PlaySound(selectedClip, .7f);
                playedFootsteps.Add(selectedClip);
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
                walkCoroutine = StartCoroutine(walkFootsteps(currentAction, .6f));
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
                sprintCoroutine = StartCoroutine(sprintFootsteps(currentAction, .35f));
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
                sneakCoroutine = StartCoroutine(sneakFootsteps(currentAction, 1f));
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
