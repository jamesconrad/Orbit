using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLevel : MonoBehaviour
{
    public void GenerateNewLevel()
    {
        CustomEventSystem.Instance.TriggerEvent(CustomEventSystem.EventType.NewLevel, GravityGroup.Instance.GravityObjects[0].name);
        return;
    }
}
