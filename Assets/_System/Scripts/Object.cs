using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public float mass;
    public float radius;
    public float SOIdistance;

    private void Start()
    {
        transform.localScale = Vector3.one * (radius / 23454.8f) * StarSystem.singleton.scale * StarSystem.singleton.AuToUnityUnits;
    }
}
