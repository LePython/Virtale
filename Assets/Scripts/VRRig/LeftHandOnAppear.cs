using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeftHandOnAppear : MonoBehaviour
{
    
    [SerializeField]
    private UnityEvent OnLeftHandAppear;

    private void OnEnable() 
    {
        OnLeftHandAppear.Invoke();
    }
}
