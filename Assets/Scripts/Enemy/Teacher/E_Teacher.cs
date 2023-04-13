using System.Collections;
using UnityEngine;
using MyBox;

public class E_Teacher : MonoBehaviour
{
    [SerializeField, Separator("Damage", true)]
    private float damage = 25f;

    E_TeacherController controller;
    [SerializeField]
    P_Health p_health;

    Coroutine dealDamageCoroutine = null;

    private void Awake() {
        controller = GetComponent<E_TeacherController>();
        
    }

    private void Start()
    {
        p_health = controller.player.GetComponent<P_Health>();
    }

    private void Update()
    {
        controller.playerInRange = controller.PlayerInCloseRange();
        Transform noise = controller.Listen();

        if (controller.playerInRange)
        {
            if (dealDamageCoroutine == null)
            {
                dealDamageCoroutine = StartCoroutine(DealDamage(damage));
            }
        }

        if (controller.playerInSight || controller.playerInRange)
        {
            controller.StopPatroling();
            controller.MoveTo(controller.player.transform.position);
        }
        else if (noise != null && !(controller.playerInRange || controller.playerInSight))
        {
            controller.StopPatroling();
            controller.MoveTo(noise.position);
            if (dealDamageCoroutine != null)
            {
                StopCoroutine(dealDamageCoroutine);
                dealDamageCoroutine = null;
            }
        }
        else
        {
            if (controller.patrolCoroutine == null)
            {
                controller.patrolCoroutine = StartCoroutine(controller.Patrol());
            }
            if (dealDamageCoroutine != null)
            {
                StopCoroutine(dealDamageCoroutine);
                dealDamageCoroutine = null;
            }
        }
    }

    private IEnumerator DealDamage(float _damage)
    {
        while (true)
        {
            p_health.DealDamage(_damage);
            yield return new WaitForSeconds(3f);
        }

    }

}
