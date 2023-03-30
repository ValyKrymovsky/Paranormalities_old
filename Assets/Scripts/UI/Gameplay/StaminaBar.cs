using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private P_Stamina p_stamina;
    
    [Header("Opacity")]
    [Range(0, 1)]
    [SerializeField] private float staminaOpacity;
    [SerializeField] private float opacityFadeTime;
    private CanvasGroup staminaUIGroup;
    private bool active;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = p_stamina.GetMaxStamina();
        staminaUIGroup = GetComponent<CanvasGroup>();
        active = false;
        staminaUIGroup.alpha = 0;
    }

    public void Update()
    {
        slider.value = p_stamina.GetStamina();

        if (p_stamina.GetStamina() != p_stamina.GetMaxStamina() && !active)
        {
            StaminaFadeIn();
        }
        else if (p_stamina.GetStamina() == p_stamina.GetMaxStamina() && active)
        {
            StaminaFadeOut();
        }
    }

    public void StaminaFadeIn()
    {
        staminaUIGroup.DOFade(staminaOpacity, opacityFadeTime);
        active = true;
    }

    public void StaminaFadeOut()
    {
        staminaUIGroup.DOFade(0f, opacityFadeTime);
        active = false;
    }
}
