using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalObjectSpawner : MonoBehaviour
{
    private Vector3 clickStart;
    private Vector3 clickStop;
    private bool isDragging = false;
    OrbitalObject newOrbiter;
    public LineRenderer thrustLine;
    public LineRenderer simulatedLine;
    private float lineSimulationInterval = 0.017f;//roughly once every 60s
    private float lineSimulationTimer;
    public Transform Container;

    private void Start()
    {
        thrustLine.enabled = false;
        simulatedLine.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            clickStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickStart.z = 0;
            GameObject newObject = Instantiate(Resources.Load("Orbiter")) as GameObject;
            newObject.name = newObject.GetInstanceID().ToString();
            newObject.transform.position = clickStart;
            newObject.transform.SetParent(Container);
            //newObject.GetComponent<Renderer>().material.color = Random.ColorHSV(0, 1, 0.8f, 0.95f);
            newOrbiter = newObject.GetComponent<OrbitalObject>();
            newOrbiter.SetColor(Random.ColorHSV(0, 1, 0.8f, 0.8f));
            thrustLine.enabled = true;
            thrustLine.SetPosition(0, clickStart);
            simulatedLine.enabled = true;
            lineSimulationTimer = 0;
        }

        if (isDragging)
        {
            clickStop = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickStop.z = 0;
            thrustLine.SetPosition(1, clickStop);
            thrustLine.endWidth = ((clickStart - clickStop) * 0.1f).magnitude;

            lineSimulationTimer -= Time.deltaTime;
            if (lineSimulationTimer <= 0)
            {
                lineSimulationTimer = lineSimulationInterval;
                Vector3[] simulatedPoints = OrbitalObject.Simulate(clickStart, (clickStart - clickStop) * 0.1f, 1, 100, 0);
                simulatedLine.positionCount = simulatedPoints.Length;
                simulatedLine.SetPositions(simulatedPoints);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            newOrbiter.SetForce((clickStart - clickStop) * 0.1f);
            newOrbiter.Unfreeze();
            isDragging = false;
            thrustLine.enabled = false;
            simulatedLine.enabled = false;
        }
    }
}
