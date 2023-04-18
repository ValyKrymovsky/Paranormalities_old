using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

public class GradeSystem : MonoBehaviour, IInteractable
{
    [SerializeField, Separator("Computers", true)]
    private List<GameObject> computers;

    private void Awake()
    {
        computers = GameObject.FindGameObjectsWithTag("Computer").ToList();

        foreach (GameObject computer in computers)
        {
            computer.tag = "interact";
        }
    }

    public void Interact()
    {
        bool interacted = false;
        foreach (GameObject computer in computers)
        {
            if (computer.TryGetComponent(out IInteractable interactObj))
            {
                interacted = computer.GetComponent<Computer>().GetInteracted();

                if (!interacted)
                {
                    break;
                }
            }
        }

        if (interacted)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
