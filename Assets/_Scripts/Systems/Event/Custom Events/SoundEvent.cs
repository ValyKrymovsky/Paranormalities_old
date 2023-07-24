using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteamAudio;
using MyBox;
using System.Diagnostics;

[RequireComponent(typeof(EventSystem))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SteamAudioSource))]
public class SoundEvent : MonoBehaviour
{
    [Space]
    [Separator("Audio", true)]
    [Space]

    [Header("Audio properties")]
    [Space]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private SteamAudioSource _steamAudioSource;
    [SerializeField] private AudioClip _audioClip;

    [Space]
    [SerializeField] SoundPlayMode _playMode;
    [SerializeField, Tooltip("Set only if Play Mode is set to limited")] private int _playCount;
    private int _playState;

    [Space]
    [Separator("Event system")]
    [Space]

    [Header("event system")]
    [Space]
    [SerializeField] private EventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = GetComponent<EventSystem>();
        _audioSource = GetComponent<AudioSource>();
        _steamAudioSource = GetComponent<SteamAudioSource>();
    }

    private void OnEnable()
    {
        _eventSystem.OnEventStart += PlayAudioClip;
    }

    private void OnDisable()
    {
        _eventSystem.OnEventStart -= PlayAudioClip;
    }

    private void PlayAudioClip()
    {
        switch (_playMode)
        {
            case SoundPlayMode.once:
                if (_playState != 0) break;

                _audioSource.PlayOneShot(_audioClip);
                _playState += 1;
                break;

            case SoundPlayMode.limited:
                if (_playState >= _playCount) break;

                _audioSource.PlayOneShot(_audioClip);
                _playState += 1;
                break;

            case SoundPlayMode.unlimited:
                _audioSource.PlayOneShot(_audioClip);
                break;

            case SoundPlayMode.continuous:
                _audioSource.loop = true;
                _audioSource.clip = _audioClip;

                _audioSource.Play();
                break;
        }
    }
}
