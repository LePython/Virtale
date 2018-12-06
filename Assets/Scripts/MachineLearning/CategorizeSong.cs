using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunPythonScript;
using System.Threading;

public class CategorizeSong : MonoBehaviour
{
    private Thread workerThread = null;

    private string dataPath;
    private string songName = "CategorizeSong.py";
    private string error;

    private string output = "";

    RunPythonScript.MLModelLoadingManager kmeansModel;
    // Start is called before the first frame update
    private void Awake() {
        dataPath = Application.dataPath;
    }
    void Start()
    {

        kmeansModel = new RunPythonScript.MLModelLoadingManager();
        

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("l"))
        {
            Debug.Log("Starting a new thread");
            workerThread = new Thread(new ThreadStart(() => kmeansModel.ExecutePythonScript(dataPath + "/MLData/CategorizeSong.py", out error, out output)));
            workerThread.Start();
        }
        if(!workerThread.IsAlive)
        {
            Debug.Log(output);
        }
    }
}
