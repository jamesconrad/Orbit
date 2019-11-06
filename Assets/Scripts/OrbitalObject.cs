using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OrbitalObject : MonoBehaviour
{
    static float BIG_G = 0.0001f;
    public float m_mass;
    public float m_radius;
    public float liveTime = 0;
    private Vector3[] m_forces;
    private Vector3 m_finalForce;
    private Vector3 m_priorVelocity;
    private Vector3 m_initialForce;
    private bool frozen = true;
    public bool stable = false;
    private bool collided = false;

    private Rigidbody rb;

    public void SetForce(Vector3 force)
    {
        m_initialForce = force;
        m_initialForce.z = 0;
    }

    public void Unfreeze()
    {
        rb.AddRelativeForce(m_initialForce * (1.0f/Time.deltaTime), ForceMode.VelocityChange);
        //m_priorVelocity = rb.velocity = m_initialForce * (1.0f / Time.deltaTime);
        //m_finalForce = m_initialForce;
        frozen = false;
    }

    void Start()
    {
        frozen = true;
        rb = GetComponent<Rigidbody>();
        //m_finalForce = new Vector3(0.01f, 0, 0);
        m_forces = new Vector3[GravityGroup.Instance.GravityObjects.Count + 1];
    }

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
        TrailRenderer tr = GetComponent<TrailRenderer>();
        tr.startColor = color;
        tr.endColor = color;
    }

    void Update()
    {
        float fps = (1.0f / Time.deltaTime);
        liveTime += Time.deltaTime;
        if (frozen) return;
        //m_forces[0] = m_finalForce;
        m_forces[0] = new Vector3(0, 0, 0);
        if (collided)
        {
            m_forces[0] = rb.velocity / 60;
            collided = false;
        }


        float minDistance = 1000;
        for (int i = 1; i < GravityGroup.Instance.GravityObjects.Count + 1; i++)
        {
            GravitySource source = GravityGroup.Instance.GravityObjects[i-1];
            
            //skip ourselves if we are also a source to prevent some nan math
            //if (source.transform == transform)
            //{
            //    m_forces[i] = new Vector3(0, 0, 0);
            //    continue;
            //}

            Vector3 direction = source.transform.position - transform.position;
            float distance = direction.magnitude;
            if (minDistance > distance)
                minDistance = distance;

            //TODO: change this to decay the burn mass
            if (distance <= source.m_radius / 2 && !frozen)
            {
                GetComponent<Renderer>().enabled = false;
                transform.position = source.transform.position;
                frozen = true;
                Destroy(gameObject, 2.5f);
                CustomEventSystem.Instance.TriggerEvent(CustomEventSystem.EventType.Collision, source.name);
            }

            float gravitationalForce = BIG_G * (m_mass * source.m_mass / (distance * distance));
            m_forces[i] = direction.normalized * gravitationalForce;
            //i++;
        }

        //check for destruction
        if (minDistance > 10)
        {
            GetComponent<Renderer>().enabled = false;
            frozen = true;
            Destroy(gameObject, 2.5f);
        }

        if (stable == false && liveTime > 10)
        {
            stable = true;
            CustomEventSystem.Instance.TriggerEvent(CustomEventSystem.EventType.OrbitStable, transform.name);
        }

        //sum and apply all forces
        m_finalForce = new Vector3(0, 0, 0);
        foreach(Vector3 force in m_forces)
            m_finalForce += force;
        m_finalForce.z = 0;

        //rb.velocity = m_finalForce * (1.0f / Time.deltaTime);
        rb.AddRelativeForce(m_finalForce * fps, ForceMode.VelocityChange);
        //transform.position += m_finalForce;
        transform.rotation.SetLookRotation(m_finalForce.normalized, Vector3.forward);
        Debug.DrawRay(transform.position, transform.forward);
    }

    private void OnDestroy()
    {
        CustomEventSystem.Instance.TriggerEvent(CustomEventSystem.EventType.OrbiterDestroyed, stable ? "stable" : "unstable");
    }

    private void OnTriggerStay(Collider other)
    {
        rb.Sleep();
    }

    private void OnCollisionExit(Collision collision)
    {
        collided = true;
    }

    //skip is number of times to skip during a step, higher values is less accurate
    public static Vector3[] Simulate(Vector3 startPosition, Vector3 initalForce, float mass, int steps, int skip, float timeStep = 1.0f/60.0f)
    {
        Vector3[] result = new Vector3[steps + 1];
        result[0] = startPosition;
        skip++; //skip value of 0 means no skipping but we use this as a scalar so it must be incremented
        //setup vals
        Vector3[] forces = new Vector3[GravityGroup.Instance.GravityObjects.Count + 1];
        float fps = 60;
        Vector3 finalForce = initalForce;
        Vector3 position = startPosition;
        bool dead = false;
        for (int step = 0; step < steps; step++)
        {
            forces[0] = finalForce;
            int i = 1;
            //calculate and store the gravity force for each source
            foreach (GravitySource source in GravityGroup.Instance.GravityObjects)
            {
                Vector3 direction = source.transform.position - position;
                float distance = direction.magnitude;
                if (distance <= source.m_radius / 2)
                {
                    dead = true;
                    break;
                }
                float gravitationalForce = BIG_G * (mass * source.m_mass / (distance * distance));
                forces[i] = direction.normalized * gravitationalForce;
                i++;
            }
            if (dead)
            {
                for (int j = step + 1; j < result.Length; j++)
                    result[j] = position;
                break;
            }
            //sum and apply all forces
            finalForce = new Vector3(0, 0, 0);
            foreach (Vector3 force in forces)
                finalForce += force;
            finalForce.z = 0;
            finalForce *= timeStep * fps;

            position += finalForce * skip;
            result[step + 1] = position;
        }
        return result;
    }
}
