using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScrollText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    [SerializeField]
    private float scrollSpeed;

    private RectTransform textMeshRectTransform;

    [SerializeField]
    private Transform handPosition;


    private void Awake() 
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMeshRectTransform = textMesh.GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScrollTextMesh());
    }

    IEnumerator ScrollTextMesh()
    {
        float width = textMesh.preferredWidth;
        Vector3 startPosition = handPosition.localPosition;

        float scrollPosition = 0;

        while (true)
        {
            if(textMesh.havePropertiesChanged)
            {
                width = textMesh.preferredWidth;

            }
            textMeshRectTransform.position = new Vector3(-scrollPosition % width/10, startPosition.y, startPosition.z);
            scrollPosition = scrollSpeed * Time.deltaTime;

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
