using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Stats : MonoBehaviour
{
    private TextMeshProUGUI fps;

    [SerializeField] private float updateRate;
    private float time;
    private int frameCount;
    public bool active;
    void Start()
    {
        fps = GetComponent<TextMeshProUGUI>();
        active = false;
        fps.enabled = false;
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        if (((int)context.phase) == 2)
        {
            active = !active;
            if (active)
            {
                fps.enabled = true;
            }
            else
            {
                fps.enabled = false;
            }
        }
        
        
    }

    void Update()
    {
        if (active)
        {
            time += Time.deltaTime;

            frameCount++;

            if (time >= updateRate)
            {
                int frameRate = Mathf.RoundToInt(frameCount/time);
                fps.text = frameRate.ToString() + " fps";

                time -= updateRate;
                frameCount = 0;
            }
        }
        
    }
}
