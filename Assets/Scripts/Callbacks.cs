using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callbacks : MonoBehaviour
{
    public Transform Container;

    private Transform cameraTarget;
    public float coTime;
    private Vector3 cameraStart;
    public int numStableOrbits;
    public int numStableOrbitsRequired;

    public UnityEngine.UI.Text stableOrbitText;
    public UnityEngine.UI.Text stableOrbitCounter;

    // Start is called before the first frame update
    void Start()
    {
        //CustomEventSystem.Instance.AddCallback(CustomEventSystem.EventType.Collision, OnCollisionEvent);
        CustomEventSystem.Instance.AddCallback(CustomEventSystem.EventType.NewLevel, GenerateRandomLevel);
        CustomEventSystem.Instance.AddCallback(CustomEventSystem.EventType.OrbitStable, OnOrbitStable);
        CustomEventSystem.Instance.AddCallback(CustomEventSystem.EventType.OrbiterDestroyed, OnOrbiterDestroy);
    }

    public void OnCollisionEvent(string message)
    {
        if (message.Contains("Random Level"))
        {
            GenerateRandomLevel(message);
        }
    }

    public void OnLevelCompleteEvent(string message)
    {
        GenerateRandomLevel(message);
    }

    public void OnOrbitStable(string message)
    {
        numStableOrbits++;
        stableOrbitCounter.text = (numStableOrbitsRequired - numStableOrbits).ToString();
        if (numStableOrbits >= numStableOrbitsRequired)
            GenerateRandomLevel(message);
    }

    public void OnOrbiterDestroy(string message)
    {
        if (message.Equals("stable"))
        {
            numStableOrbits--;
            stableOrbitCounter.text = (numStableOrbitsRequired - numStableOrbits).ToString();
        }
    }

    private void GenerateRandomLevel(string target)
    {
        cameraTarget = GravityGroup.Instance.GravityObjects[0].transform;
        //cameraTarget = GameObject.Find(target).transform;
        cameraStart = Camera.main.transform.position;
        GravityGroup.Instance.ClearGroup();
        coTime = 1;
        StartCoroutine("GenerateRandomLevelEnumerator");
    }

    private IEnumerator GenerateRandomLevelEnumerator()
    {
        for (int j = 0; j < 1000; j++)
        {
            
            Camera.main.transform.position = Vector3.Lerp(cameraTarget.position, cameraStart, coTime);
            cameraTarget.localScale = Vector3.Lerp(new Vector3(25,25,1), cameraTarget.localScale, coTime);
            coTime -= Time.deltaTime/2;
            if (coTime <= 0.5)
            {
                //set bg to faded current color
                //Color col = cameraTarget.GetComponent<Renderer>().material.color;
                //Color.RGBToHSV(col, out float h, out float s, out float v);
                //Camera.main.backgroundColor = Color.HSVToRGB(h, 0.25f, v);

                //clear existing objects
                //while (Container.childCount > 0)
                //    Destroy(Container.GetChild(0));
                foreach (Transform child in Container)
                    Destroy(child.gameObject);

                //reset cam pos
                Camera.main.transform.position = new Vector3(0, 0, -10);

                int newPlanets = Random.Range(1, 3);
                numStableOrbitsRequired = Random.Range(2, 5);
                //re-enable stableorbit texts;
                stableOrbitCounter.text = numStableOrbitsRequired.ToString();
                stableOrbitCounter.enabled = true;
                stableOrbitText.enabled = true;
                stableOrbitText.GetComponent<FadeOut>().ResetFade();

                for (int i = 0; i < newPlanets; i++)
                {
                    float mass = Random.Range(50, 200);
                    //Color color = Random.ColorHSV(0, 1, 0.5f, 0.75f, 1, 1, 0, 0);
                    Color color = Color.black;
                    GameObject planet = Instantiate(Resources.Load("Planet")) as GameObject;
                    planet.GetComponent<GravitySource>().PostConstruct(mass, color);
                    planet.name += i;
                    planet.transform.SetParent(Container);
                    planet.transform.position = (Vector2)Random.insideUnitSphere * 4;
                }
                yield break;//quit coroutine
            }
            yield return null;
        }
    }
}
