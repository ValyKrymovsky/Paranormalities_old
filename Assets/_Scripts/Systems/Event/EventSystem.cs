using UnityEngine;
using MyBox;
using System;

public class EventSystem : MonoBehaviour
{
    [Space]
    [Separator("Player")]
    [Space]

    [Header("Player properties")]
    [Space]
    [SerializeField] private LayerMask _playerLayer;

    private Collider _collider;

    public event Action OnEventStart;


    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.tag.Equals("Player")) OnEventStart?.Invoke();
    }

}
