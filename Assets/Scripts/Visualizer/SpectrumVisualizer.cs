using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioAnalyzer;
public class SpectrumVisualizer : MonoBehaviour {


    #region Serialized variables

    // Default spectrum prefab
    [SerializeField]
    private Transform defaultSpectrumPrefab;

    [SerializeField]
    private float distanceBetweenNodes;

    [SerializeField, Range(1,10)]
    private float nodesMovementStrength;

    #endregion

    #region Private Variables

    private static int spectrumNodeSize = 8;

    private Transform[] spectrumNodes = new Transform[spectrumNodeSize];

    [SerializeField]
    private AudioSpectrum audioSpectrum;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (!defaultSpectrumPrefab)
        {
            Debug.LogError("There is no default prefab set in Spectrum Visualizer. GameObject will be disabled.");
            enabled = false;
            return;
        }
        if(!audioSpectrum)
        {
            Debug.LogError("There is no AudioSpectrum object assigned to the Spectrum Visualizer. GameObject will be disabled.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        CreateVisualizer();
    }

    //TODO Create a coroutine instead of using an Update function, 
    //because the visualizer might not even need to be updated the whole time
    private void Update() {
        UpdateSpectrum();
    }

    #endregion


    #region Private Methods

    // Instantiate the spectrum nodes
    private void CreateVisualizer()
    {
        for (int i = 0; i < spectrumNodeSize; i++)
        {
            Transform newCube = Instantiate(defaultSpectrumPrefab);
            newCube.name = "Spectrum " + i;
            newCube.transform.SetParent(gameObject.transform, true);
            newCube.transform.localPosition = new Vector3(distanceBetweenNodes*i*newCube.transform.localScale.x, 0f, 0f);
            spectrumNodes[i] = newCube;
        }
    }
        /// <summary>
    /// Update the spectrum meshes: 
    /// Scales the spectrum meshes according to the frequency of current sample
    /// </summary>
    private void UpdateSpectrum()
    {
        for (int i = 0; i < SpectrumNodes.Length; i++)
        {
            float scaleFactor = audioSpectrum.GetSpectrum(AudioSpectrum.AudioDataReturnType.DefaultBands)[i] * nodesMovementStrength;
            scaleFactor = Mathf.Lerp(SpectrumNodes[i].localScale.y, scaleFactor, 0.25f);
            Vector3 newSize = DefaultSpectrumPrefab.localScale + new Vector3(0f, Mathf.Abs(scaleFactor), 0f);
            SpectrumNodes[i].transform.localScale = newSize;
        }
    }

    // Scale the z value of spectrum to 1f
    private void ResetSpectrum()
    {
        for (int i = 0; i < SpectrumNodes.Length; i++)
        {
            float scaleFactor = Mathf.Lerp(SpectrumNodes[i].localScale.y, 1f, 0.2f);
            Vector3 newSize = DefaultSpectrumPrefab.localScale + new Vector3(0f, Mathf.Abs(scaleFactor), 0f);
            SpectrumNodes[i].localScale = newSize;
        }
    }
    #endregion

    #region Properties

    // Get the spectrum nodes
    public Transform[] SpectrumNodes
    {
        get
        {
            return spectrumNodes;
        }
    }

    public Transform DefaultSpectrumPrefab { get => defaultSpectrumPrefab; set => defaultSpectrumPrefab = value; }

    #endregion

}
