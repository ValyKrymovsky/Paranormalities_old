using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [field: SerializeField] public PlayerCameraData CameraData { get; private set; }
    [field: SerializeField] public PlayerMovementData MovementData { get; private set; }
    [field: SerializeField] public PlayerHealthData HealthData { get; private set; }
    [field: SerializeField] public PlayerStaminaData StaminaData { get; private set; }
    [field: SerializeField] public PlayerInventoryData InventoryData { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance.transform.root.gameObject);
        }
    }
}
