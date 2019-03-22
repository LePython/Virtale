using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandOutsideCameraView : MonoBehaviour
{
    

    [SerializeField]
    private GameObject objectToObserveForChanges;

    [SerializeField]
    private UnityEvent WhenObjectDisabled;

    public void StartObjectActiveStateObservation()
    {
        StartCoroutine("ObjectActiveStateObservation");
    }
    private IEnumerator ObjectActiveStateObservation()
    {
        while(objectToObserveForChanges.activeSelf == true)
        {
            yield return new WaitForSeconds(0.2f);
        }

        WhenObjectDisabled.Invoke();

        yield break;
    }
}
