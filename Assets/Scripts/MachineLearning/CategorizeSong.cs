using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunPythonScript;
using System.Threading;
using UnityEngine.Events;

public class CategorizeSong : MonoBehaviour
{
    delegate void Categorize();
    Categorize categorizeDelegate;
    private Thread workerThread = null;

    private string songRelativePath = "";
    public UnityEvent OnCategorizeFinished;
    private string dataPath;
    private string error;
    private string output = null;

    RunPythonScript.MLModelLoadingManager kmeansModel;
    RunPythonScript.LoadIronPythonMLModel kmeansIPModel;

    // Start is called before the first frame update
    private void Awake() {
        dataPath = Application.dataPath;
    }
    void Start()
    {

        kmeansModel = new RunPythonScript.MLModelLoadingManager();
        kmeansIPModel = new RunPythonScript.LoadIronPythonMLModel();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("l"))
        {
            Debug.Log("Starting a new thread to Categorize song");

            songRelativePath = "Adele - Hello.mp3";

            // Assigns the script to the delegate
            //categorizeDelegate = RunTheMLModelTest;

            // Starts a new Thread with a newly assigned delegate for categorizing
            //workerThread = new Thread(new ThreadStart(() => RunTheMLModel()));
            //workerThread.Start();
            
            output = kmeansModel.ExecutePythonScript(dataPath + "/MLData/test.py", out error, out output);

            // Waits for thread and analysis to end
            //StartCoroutine(WaitForCategorizationEnd());
        }
        
        Debug.Log(output);
    }

    /// <summary>
    /// If there is no output, then wait till there is one.
    /// If there is output, write it down the console
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForCategorizationEnd()
    {
        while(output == null)
        {
#if UNITY_EDITOR
            Debug.Log("Categorizing...");
#endif
            yield return new WaitForSeconds(.1f);
        }

        workerThread.Abort();
        Debug.Log("Your song group is: " + output);

        OnCategorizeFinished.Invoke();

        yield break;
    }

    #region Delegate Methods
    /// <summary>
    /// Runs the model with given argmuents by accessing the
    /// function from MLMOdelLoadingManager
    /// </summary>
    private void RunTheMLModelTest()
    {
        kmeansModel.ExecutePythonScript(dataPath + "/MLData/CategorizeSongTest.py", out error, out output);
    }
    private void RunTheMLModel()
    {
        if(SongRelativePath)
        {
            kmeansModel.ExecutePythonScript(dataPath + "/MLData/test.py", out error, out output);
        }
        else
        {
            Debug.LogError("There is no audio path assigned to the Categorizer. Do it in script before the start of a new thread");
        }
    }

    private void RunIronPythonModel()
    {
        if(SongRelativePath)
        {
            output = kmeansIPModel.PatchParameter("CategorizeSong.py", "Adele - Hello.mp3");
        }
        else
        {
            Debug.LogError("There is no audio path assigned to the Categorizer. Do it in script before the start of a new thread");
        }
    }
    #endregion

    #region Properties

    // Return true only if there is a path given
    public bool SongRelativePath
    {
        get{
            bool songExist = (songRelativePath == null || songRelativePath == "") ? false : true;
            return songExist;
            // if(songRelativePath == null || songRelativePath == "")
            // {
            //     return false;
            // }else
            // {
            //     return true;
            // }
        }
    }

    #endregion

}
