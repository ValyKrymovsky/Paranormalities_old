using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum groundTypes
{
    Concrete,
    Wood,
    Grass
}

public class P_SoundSystem : MonoBehaviour
{
    private P_Movement p_movement;
    private CharacterController ch_controller;
    private Coroutine soundCoroutine;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private float minPitch = .8f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private AudioClip[] footsteps_wood;
    [SerializeField] private AudioClip[] footsteps_concrete;
    [SerializeField] private AudioClip[] footsteps_grass;
    private List<AudioClip> playedFootsteps;
    private moveAction currentAction;
    private groundTypes ground;

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

    private IEnumerator soundEmittor()
    {
        while (true)
        {
            // PlaySound()
        }
    }

    private void Update()
    {
        currentAction = p_movement.GetMoveAction();


    }

    private void CheckLayer()
    {
        if (p_movement.IsGrounded())
        {
            Ray ray = new Ray(gameObject.transform.position, transform.up * -1);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, ch_controller.height / 2))
            {
                Debug.Log(hitInfo.transform.gameObject.layer);
            }
        }
    }
}
