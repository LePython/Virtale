using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocation : MonoBehaviour
{
    [SerializeField]
    private Vector3 newLocation;

    public void ChangeLocation()
    {
        gameObject.transform.position = newLocation;
    }
}
