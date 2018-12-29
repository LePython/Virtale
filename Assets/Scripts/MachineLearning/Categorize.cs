using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunPythonScript;
public class Categorize : MonoBehaviour
{

    RunPythonScript.MLModelLoadingManager kmeansModel;
    string error;
    string output;

    // Start is called before the first frame update
    void Start()
    {
        kmeansModel = new RunPythonScript.MLModelLoadingManager();
        string otext = kmeansModel.ExecutePythonScript(Application.dataPath + "/MLData/CategorizeSong.py", out error, out output);    
        Debug.Log(otext);
    }
}
