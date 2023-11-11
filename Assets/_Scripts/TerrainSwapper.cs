using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerrainSwapper : MonoBehaviour
{
    public TerrainCollider _terrainCollider;
    public Terrain terrain;
    public TerrainData oldTerrainData;
    public TerrainData newTerrainData;

    public InputActionReference input;

    private void Awake()
    {
        oldTerrainData = _terrainCollider.terrainData;
    }

    private void OnEnable()
    {
        input.action.Enable();
        input.action.performed += SwapTerrain;
    }

    private void OnDisable()
    {
        input.action.Disable();
        input.action.performed -= SwapTerrain;
    }

    private void SwapTerrain(InputAction.CallbackContext ctx)
    {
        Debug.Log("Swapped terrain");
        _terrainCollider.terrainData = newTerrainData;
        terrain.terrainData = newTerrainData;
    }
}
