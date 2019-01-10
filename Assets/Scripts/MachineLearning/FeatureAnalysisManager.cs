using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.Events;
using Newtonsoft.Json;

namespace UnityEngine.ML
{
    [Serializable]
    public class SongData
    {
        public string Name;
        public int FeatureGroup;
    }
    public class FeatureAnalysisManager : MonoBehaviour
    {

        delegate void Categorize();
        Categorize categorizeDelegate;
        private Thread workerThread = null;
        public UnityEvent OnCategorizeFinished;
        private string dataPath;
        private string error;
        private string output = null;

        bool isDoneAnalyzing = false;
        MLModelLoader kmeansModel;

        [SerializeField]
        private List<SongData> analyzedSongList;

        public List<SongData> AnalyzedSongList { get => analyzedSongList; set => analyzedSongList = value; }

        #region Unity Methods
        // Start is called before the first frame update
        private void Awake() 
        {
            dataPath = Application.dataPath;
        }
        void Start()
        {

            kmeansModel = new MLModelLoader();

            analyzedSongList = GetFeatureList();

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown("l"))
            {
                Debug.Log("Starting a new thread to Categorize new songs");

                // Assigns the script to the delegate
                categorizeDelegate = RunTheMLModel;

                // Starts a new Thread with a newly assigned delegate for categorizing
                workerThread = new Thread(new ThreadStart(categorizeDelegate));
                workerThread.Start();

                // Waits for thread and analysis to end
                StartCoroutine(WaitForCategorizationEnd());
            }
            //Debug.Log(output);
        }
        #endregion
        /// <summary>
        /// Wait till the the worker thread is done processing
        /// When its done, stop the thread and execute OnCategorizeFinished method
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForCategorizationEnd()
        {
            while(kmeansModel.IsProcessingDone == false)
            {
    #if UNITY_EDITOR
                Debug.Log("Categorizing...");
    #endif

                yield return new WaitForSeconds(.1f);
            }

            kmeansModel.IsProcessingDone = false;

            workerThread.Abort();
            Debug.Log("Done processing.");

            OnCategorizeFinished.Invoke();

            yield break;
        }

        #region Delegate Methods
        /// <summary>
        /// Runs the model with given argmuents by accessing the
        /// function from MLModelLoadingManager
        /// </summary>
        private void RunTheMLModelTest()
        {
            kmeansModel.ExecutePythonScript(dataPath + "/MLData/CategorizeSongTest.py", out error, out output);
        }
        private void RunTheMLModel()
        {
            kmeansModel.ExecutePythonScript(dataPath + "/MLData/CategorizeSong.py", out error, out output);
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<SongData> GetFeatureList()
        {

            List<SongData> analyzedSongs = new List<SongData>();

            using (StreamReader reader = new StreamReader(dataPath + "/MLData/AnalyzedFeaturesList.json"))
            {
                
                string jsonFile = reader.ReadToEnd();
                analyzedSongs = JsonConvert.DeserializeObject<List<SongData>>(jsonFile);

            }
            
            return analyzedSongs;
        }

        #endregion


    }
}
