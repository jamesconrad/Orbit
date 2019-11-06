using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventSystem : MonoBehaviour
{
    public enum EventType { Collision, OrbitStable, OrbiterDestroyed, NewLevel, COUNT };
    public delegate void Callback(string message);
    private List<List<Callback>> m_callbacks = new List<List<Callback>>();
    //singleton definiton
    public static CustomEventSystem Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < (int)EventType.COUNT; i++)
        {
            m_callbacks.Add(new List<Callback>());
        }
    }

    public void TriggerEvent(EventType e, string message)
    {
        foreach (Callback callback in m_callbacks[(int)e])
        {
            callback(message);
        }
    }

    public void AddCallback(EventType e, Callback callback)
    {
        m_callbacks[(int)e].Add(callback);
    }
}
