using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleUniformGrid : MonoBehaviour
{
    ParticleSystem pSystem;

    [SerializeField]
    private Vector3 bounds = new Vector3(25f, 25f, 25f);

    [SerializeField]
    private Vector3Int resolution = new Vector3Int(10, 10, 10);

    [SerializeField]
    private float radius;

    private void OnEnable()
    {
        pSystem = GetComponent<ParticleSystem>();

        Vector3 scale;
        Vector3 boundsHalf = bounds / 2.0f;
 
        scale.x = bounds.x / resolution.x;
        scale.y = bounds.y / resolution.y;
        scale.z = bounds.z / resolution.z;
 
        ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
 
        for (int i = 0; i < resolution.x; i++)
        {
            for (int j = 0; j < resolution.y; j++)
            {
                for (int k = 0; k < resolution.z; k++)
                {
                    Vector3 position;
 
                    position.x = radius * Mathf.Cos(2*Mathf.PI/10 * i);
                    position.y = (j * scale.y) - boundsHalf.y;
                    position.z = radius * Mathf.Sin(2*Mathf.PI/10 * k);
                     
                    ep.position = position;
                    pSystem.Emit(ep, 1);
                }
            }
        }
    }
}
