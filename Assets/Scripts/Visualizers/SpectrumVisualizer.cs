using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioAnalyzer;

namespace UnityEngine.Visualizers
{
    public class SpectrumVisualizer : MonoBehaviour {

        #region Private Variables

        private delegate void CreateSpectrumNodes();
        private delegate void UpdateSpectrumDel();
        UpdateSpectrumDel updateSpectrumDel;
        CreateSpectrumNodes createSpectrumNodes;
        
        private List<Transform> spectrumNodes = new List<Transform>();

        [SerializeField]
        private SpectrumAnalyzer spectrumAnalyzer;

        public SpectrumAnalyzer.AudioDataReturnType audioDataType;

        private bool updateSpectrum = false;

        #endregion

        #region Serialized variables
        
        [ConditionalField("audioDataType", SpectrumAnalyzer.AudioDataReturnType.CustomBands), SerializeField]
        private int spectrumNodeCount = 8;

        // Default spectrum prefab
        [SerializeField]
        private Transform defaultSpectrumPrefab;

        [ConditionalField("audioDataType", SpectrumAnalyzer.AudioDataReturnType.DefaultBands), SerializeField]
        private float distanceBetweenNodes;

        [SerializeField, Range(1,10)]
        private float nodesMovementStrength = 1f;

        [ConditionalField("audioDataType", SpectrumAnalyzer.AudioDataReturnType.CustomBands), SerializeField]
        private Color spectrumColor;

        [ConditionalField("audioDataType", SpectrumAnalyzer.AudioDataReturnType.CustomBands), SerializeField]
        private float radius;

        [SerializeField]
        private Vector3 spectrumNodeScale = new Vector3(1f, 1f, 1f);

        [ConditionalField("audioDataType", SpectrumAnalyzer.AudioDataReturnType.CustomBands), SerializeField]
        private Transform lookAtTargetObject;

        #endregion

        // Update Spectrum if music is playing
        // Coroutine is started if music is playing
        // otherwise it is stopped
        private IEnumerator SpectrumUpdater()
        {
            Debug.Log("Starting new Coroutine to update spectrum nodes");
            while(updateSpectrum)
            {
                updateSpectrumDel();
                
                yield return null;
            }
            yield break;
        }

        #region Unity Methods

        private void Awake()
        {
            if (!defaultSpectrumPrefab)
            {
                Debug.LogError("There is no default prefab set in Spectrum Visualizer. GameObject will be disabled.");
                enabled = false;
                return;
            }
            if(!spectrumAnalyzer)
            {
                Debug.LogError("There is no spectrumAnalyzer object assigned to the Spectrum Visualizer. GameObject will be disabled.");
                enabled = false;
                return;
            }
            // Check what to analyze and assign the method accordingly
            if(audioDataType == SpectrumAnalyzer.AudioDataReturnType.CustomBands)
            {
                updateSpectrumDel = UpdateDividedSpectrum;
                createSpectrumNodes = CreateCustomVisualizer;
            }else
            {
                updateSpectrumDel = UpdateSpectrum;
                createSpectrumNodes = CreateDefaultVisualizer;
            }
        }

        private void Start()
        {
            createSpectrumNodes();
        }

        #endregion


        #region Private Methods

        // Instantiate the spectrum nodes
        private void CreateDefaultVisualizer()
        {
            for (int i = 0; i < 8; i++)
            {
                Transform newCube = Instantiate(defaultSpectrumPrefab);
                newCube.name = "Spectrum " + i;
                newCube.transform.SetParent(gameObject.transform, true);
                newCube.transform.localPosition = new Vector3(distanceBetweenNodes*i*newCube.transform.localScale.x, 0f, 0f);
                newCube.transform.localScale = spectrumNodeScale;
                spectrumNodes.Add(newCube);
                
            }
        }

        private void CreateCustomVisualizer()
        {
            for (int i = 0; i < spectrumNodeCount; i++)
            {
                Transform newCube = Instantiate(defaultSpectrumPrefab);
                Material cubeMaterial = newCube.GetComponent<Renderer>().material;
                cubeMaterial.SetColor("_EmissionColor", spectrumColor);
                newCube.name = "Spectrum " + i;
                newCube.transform.SetParent(gameObject.transform, true);
                newCube.transform.localScale = spectrumNodeScale;
                spectrumNodes.Add(newCube);
            }
            CreateCircleFromObjects(spectrumNodes);
        }

        /// <summary>
        /// Update the spectrum meshes: 
        /// Scales the spectrum meshes according to the frequency of current sample
        /// </summary>
        private void UpdateSpectrum()
        {
            for (int i = 0; i < SpectrumNodes.Count; i++)
            {

                float scaleFactor = spectrumAnalyzer.GetSpectrum(audioDataType)[i] * nodesMovementStrength;

                scaleFactor = Mathf.Lerp(SpectrumNodes[i].localScale.y, scaleFactor, 0.25f);
                Vector3 newSize = spectrumNodeScale + new Vector3(0f, Mathf.Abs(scaleFactor), 0f);
                SpectrumNodes[i].transform.localScale = newSize;
            }
        }
        private void UpdateDividedSpectrum()
        {
            for (int i = 0; i < SpectrumNodes.Count; i++)
            {
                float scaleFactor = 0f;

                scaleFactor = spectrumAnalyzer.GetDividedSpectrum<Transform>(audioDataType, SpectrumNodes)[i] * nodesMovementStrength * 10;

                scaleFactor = Mathf.Lerp(SpectrumNodes[i].localScale.y, scaleFactor, 0.25f);

                Vector3 newSize = spectrumNodeScale + new Vector3(0f, Mathf.Abs(scaleFactor), 0f);
                SpectrumNodes[i].transform.localScale = newSize;
                //SpectrumNodes[i].transform.LookAt(lookAtTargetObject);
            }

        }

        // Scale the z value of spectrum to 1f
        private void ResetSpectrum()
        {
            for (int i = 0; i < SpectrumNodes.Count; i++)
            {
                float scaleFactor = Mathf.Lerp(SpectrumNodes[i].localScale.y, 1f, 0.2f);
                Vector3 newSize = spectrumNodeScale + new Vector3(0f, Mathf.Abs(scaleFactor), 0f);
                SpectrumNodes[i].localScale = newSize;
            }
        }
        /// <summary>
        /// Create a circle from the given object list and set the position accordingly
        /// </summary>
        /// <param name="objectList"></param>
        private void CreateCircleFromObjects (List<Transform> objectList)
        {
            // Den Umfang ausrechnen und danach diesen durch die Anzahl von Elemente teilen
            float abstand = 2 * Mathf.PI/objectList.Count;

            for(int i = 0; i < objectList.Count; i++)
            {
                float xCoordinate = radius * Mathf.Cos(abstand * i);
                float yCoordinate = radius * Mathf.Sin(abstand * i);

                objectList[i].localPosition = new Vector3(xCoordinate, 0f, yCoordinate);
                objectList[i].LookAt(lookAtTargetObject);
            }

        }
        // Checks if song is playing, if so, then starts a coroutine which updates spectrum
        public void CheckPlaybackState()
        {
            if(UnityEngine.AudioManager.AudioPlaybackManager.musicPlaybackState == UnityEngine.AudioManager.AudioPlaybackManager.PlaybackState.Play)
            {
                updateSpectrum = true;
                StartCoroutine(SpectrumUpdater());
            }else
            {
                updateSpectrum = false;
                StopCoroutine(SpectrumUpdater());
            }
        }
        #endregion

        #region Properties

        // Get the spectrum nodes
        public List<Transform> SpectrumNodes
        {
            get
            {
                return spectrumNodes;
            }
        }

        public Transform DefaultSpectrumPrefab { get => defaultSpectrumPrefab; set => defaultSpectrumPrefab = value; }

        #endregion

    }
   
}