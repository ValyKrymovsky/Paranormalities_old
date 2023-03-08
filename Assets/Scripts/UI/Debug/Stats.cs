using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    private TextMeshProUGUI fps;

    [SerializeField] private float updateRate;
    private float time;
    private int frameCount;
    void Start()
    {
        fps = GetComponent<TextMeshProUGUI>();
    }

    void Update()
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
