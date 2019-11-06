using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float timeBeforeFadeStart = 5;
    public float timeUntilFadeComplete = 5;
    private float liveTime = 0;
    public UnityEngine.UI.Text text;
    Color baseColor;
    Color invisColor;

    // Start is called before the first frame update
    void Start()
    {
        invisColor = baseColor = text.color;
        invisColor.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        liveTime += Time.deltaTime;
        if (liveTime >= timeBeforeFadeStart)
        {
            //begin fade
            float interpolationTime = (liveTime - timeBeforeFadeStart) / timeUntilFadeComplete;
            text.color = Color.Lerp(baseColor, invisColor, interpolationTime);
        }
    }

    public void ResetFade()
    {
        liveTime = 0;
        text.color = baseColor;
    }
}
