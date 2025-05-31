using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Object
{
    private void Start()
    {
        transform.localScale = Vector3.one * (radius / 23454.8f) * StarSystem.singleton.starScale * StarSystem.singleton.AuToUnityUnits;
    }
}
