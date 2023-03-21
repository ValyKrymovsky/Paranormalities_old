using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar : MonoBehaviour
{
    [SerializeField] private GameObject slotTemplate;
    private P_Inventory inventory;
    private List<GameObject> slotList;

    private void Awake() {
        for (int i = 0; i < inventory.inventory.GetInventorySize(); i++)
        {
            Instantiate(slotTemplate, this.gameObject.transform);
        }
    }
}
