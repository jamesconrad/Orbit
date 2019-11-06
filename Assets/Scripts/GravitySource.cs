using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public float m_mass = 1;
    public float m_radius = 1;
    //public float m_multiplier = 1; source and oribter mass are different so this is unessicary
    private float coTime;
    private Renderer m_renderer;
    // Start is called before the first frame update
    void Start()
    {
        GravityGroup.Instance.GravityObjects.Add(this);
        m_renderer = GetComponent<Renderer>();
    }


    public void PostConstruct(float mass, Color color)
    {
        m_renderer = GetComponent<Renderer>();
        m_renderer.material.color = color;
        m_mass = mass;
        m_radius = mass / 100;
        transform.localScale = new Vector3(m_radius, m_radius, m_radius);
        coTime = 0;
        StartCoroutine("FadeIn");
    }

    private IEnumerable FadeIn()
    {
        coTime += Time.deltaTime * 2;
        Color c = m_renderer.material.color;
        c.a = Mathf.Clamp01(coTime);
        m_renderer.material.color = c;        

        yield return null;
    }
}
