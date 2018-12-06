using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunPythonScript;
public class CategorizeSong : MonoBehaviour
{
    private string songName = "CategorizeSong.py";
    private string error;

    RunPythonScript.MLModelLoadingManager kmeansModel;
    // Start is called before the first frame update
    void Start()
    {

        kmeansModel = new RunPythonScript.MLModelLoadingManager();
        string outputText = kmeansModel.ExecutePythonScript(Application.dataPath + "/MLData/CategorizeSong.py", out error);

        Debug.Log(outputText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
