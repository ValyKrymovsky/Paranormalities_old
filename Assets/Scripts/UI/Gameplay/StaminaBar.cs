using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private P_Stamina p_stamina;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = p_stamina.getMaxStamina();
    }

    public void Update()
    {
        slider.value = p_stamina.getCurrentStamina();
    }
}
