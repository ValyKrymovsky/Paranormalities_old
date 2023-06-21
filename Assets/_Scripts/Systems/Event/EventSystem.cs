using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum useSource
{
    ThisObject,
    OtherSource
}

public enum useAnimation
{
    yes,
    no
}

public class EventSystem : MonoBehaviour
{
    private BoxCollider boxCollider;
    [SerializeField] private GameObject triggerObject;
    [SerializeField] private bool collided;

    [Header("Animation")]
    [SerializeField] private useAnimation animationOption;
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationClip animationClip;

    [Header("Audio")]
    [SerializeField] private useSource sourceOption;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();

        if (animationOption == useAnimation.yes)
        {
            anim = triggerObject.GetComponent<Animator>();
        }
        
        if (sourceOption == useSource.ThisObject)
        {
            source = GetComponent<AudioSource>();
        }
        else
        {
            source = triggerObject.GetComponent<AudioSource>();
        }

        collided = false;
    }

    private void playAudio(AudioSource _source, AudioClip _clip)
    {
        _source.PlayOneShot(_clip);
    }

    private void playAnimation(AnimationClip _animation)
    {
        anim.Play(_animation.name, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && !collided)
        {
            collided = true;
            Debug.Log("Player entered collision box");
            if (animationClip != null && anim != null)
            {
                playAnimation(animationClip);
                Debug.Log("Animation played");
            }

            if (source != null && clip != null)
            {
                playAudio(source, clip);
                Debug.Log("Audio played");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            collided = false;
        }
        
    }

}
