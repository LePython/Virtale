using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioManager;
using UnityEngine.AudioAnalyzer;

namespace UnityEngine.Visualizers
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleVisualizer : MonoBehaviour {


        #region Private Variables

        // TODO Create additional class to this and to spectrum analyzer, inherit from ParticleSystem
        // Dust Particle System and it's features
        [Header("Particle System")]
        [SerializeField]
        private ParticleSystem            dustParticleSystem;
        private ParticleSystem.MainModule dustParticleSystemMain;
        private ParticleSystemRenderer    dustParticleSystemRenderer;
        private Material                  dustParticleMaterial;

        [SerializeField]
        private int dustParticleSpeed = 3;

        [SerializeField]
        private Color particleSystemColor;

        // Equalizer scale multiplier
        private int scaleMultiplier = 10;

        [SerializeField]
        private SpectrumAnalyzer spectrumAnalyzer;

        [SerializeField]
        private SpectrumAnalyzer.AudioDataReturnType spectrumDataType;

        #endregion

        #region Unity Methods

        void Awake()
        {
            dustParticleSystem = GetComponent<ParticleSystem>();
            if(!dustParticleSystem)
            {
                Debug.LogError("There is no Particle system attached to " + nameof(dustParticleSystem));
            }
            if(!spectrumAnalyzer)
            {
                Debug.LogError("There is no SpectrumAnalyzer script assigned to the Particle Visualizer. GameObject will be disabled.");
                enabled = false;
                return;
            }
        }
        // Use this for initialization
        void Start ()
        {

            Initialize();

        }
        
        // Update is called once per frame
        void Update ()
        {
            if (AudioPlaybackManager.musicPlaybackState == AudioPlaybackManager.PlaybackState.Play)
            {
                float[] lowRangeArray = spectrumAnalyzer.GetSpectrum(spectrumDataType);
                float scaleFactor = 1f + 10 * dustParticleSpeed * (SpectrumAnalyzer.GetArrayAverage(lowRangeArray));
                // Set simulation speed of particle system to beatScaleFactor
                dustParticleSystemMain.simulationSpeed = Mathf.Lerp(dustParticleSystemMain.simulationSpeed,scaleFactor, 0.15f);

                // Set particle color brightness based on lower frequencies
                Color finalColor = particleSystemColor * dustParticleSystemMain.simulationSpeed;
                //dustParticleMaterial.SetColor("_Color", finalColor);
            }
            else
            {
                // If song isn't playing, set simulation speed to default
                dustParticleSystemMain.simulationSpeed = 1;
            }
        }

        #endregion

        #region Privately Defined Methods

        // Initialize all components
        private void Initialize()
        {
            dustParticleSystemMain = dustParticleSystem.main;
            dustParticleSystemRenderer = dustParticleSystem.GetComponent<ParticleSystemRenderer>();
            dustParticleMaterial = dustParticleSystemRenderer.material;
        }

        #endregion
    }
   
}