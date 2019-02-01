using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollector : MonoBehaviour
{
    void Update()
    {
        if (Time.frameCount % 30 == 0)
        {
            System.GC.Collect();
        }
    }
}
