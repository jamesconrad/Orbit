using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGroup : MonoBehaviour
{
    //contianer of all objects with a gravity source
    public List<GravitySource> GravityObjects;

    //singleton definiton
    public static GravityGroup Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GravityObjects = new List<GravitySource>();
    }

    public void ClearGroup()
    {
        GravityObjects.Clear();
    }
}
