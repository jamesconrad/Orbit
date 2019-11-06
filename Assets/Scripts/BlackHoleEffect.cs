using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleEffect : MonoBehaviour
{
    [Tooltip("Length of one rings animation")]
    public float ringTimeLength;
    public Transform[] rings;
    private float time;
    private Material[] ringMats;
    private float[] ringTime;

    public Vector3 startSize;
    public Vector3 stopSize;
    public Color startColor;
    public Color stopColor;

    public bool reset = false;

    private void Start()
    {
        ringMats = new Material[rings.Length];
        ringTime = new float[rings.Length];
        for (int i = 0; i < rings.Length; i++)
        {
            ringMats[i] = rings[i].GetComponent<Renderer>().material;
            ringTime[i] = i * ringTimeLength - (i * ringTimeLength / rings.Length);
        }
    }

    void Update()
    {
        if (reset)
            Start();
        time += Time.deltaTime;


        for (int i = 0; i < rings.Length; i++)
        {
            float startTime = ringTime[i];
            float stopTime = startTime + ringTimeLength;
            if (time >= startTime && time <= stopTime)
            {
                float t = (time - startTime) * (1.0f / ringTimeLength);
                rings[i].localScale = Vector3.Lerp(startSize, stopSize, t);
                ringMats[i].color = Color.Lerp(startColor, stopColor, t);
            }
            else if (stopTime < time)
            {
                ringTime[i] += ringTimeLength;
            }
        }
    }
}
