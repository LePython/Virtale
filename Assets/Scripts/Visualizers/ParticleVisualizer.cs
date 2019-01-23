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

        public enum ParticleVisualizationType
        {
            SimulationSpeed,
            VelocityOverTime
        }

        #region Private Variables

        // TODO Create additional class to this and to spectrum analyzer, inherit from ParticleSystem
        // Dust Particle System and it's features
        private ParticleSystem pSystem;
        private ParticleSystem.MainModule pSystemMainSettings;

        [SerializeField]
        private ParticleVisualizationType particleVisualizationType;

        [SerializeField]
        private int particleSpeed = 3;

        [SerializeField]
        private Color particleSystemColor;


        [SerializeField]
        private SpectrumAnalyzer spectrumAnalyzer;

        [SerializeField]
        private SpectrumAnalyzer.AudioDataReturnType spectrumDataType;

        #endregion

        #region Unity Methods

        void Awake()
        {
            pSystem = GetComponent<ParticleSystem>();
            pSystemMainSettings = pSystem.main;

            if(!spectrumAnalyzer)
            {
                Debug.LogError("There is no SpectrumAnalyzer script assigned to the Particle Visualizer. GameObject will be disabled.");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            StartVisualizerCoroutine();
        }

        #endregion

        #region Private Methods
        private void StartVisualizerCoroutine()
        {
            switch(particleVisualizationType)
            {
                case ParticleVisualizationType.SimulationSpeed:
                    StartCoroutine(VisualizeBySimulationSpeed());
                    break;
                case ParticleVisualizationType.VelocityOverTime:
                    StartCoroutine(VisualizeByVelocity());
                    break;
                default:
                    break;
            }
        }
        private IEnumerator VisualizeByVelocity()
        {
            while (AudioPlaybackManager.musicPlaybackState == AudioPlaybackManager.PlaybackState.Play)
            {
                float[] lowRangeArray = spectrumAnalyzer.GetSpectrum(spectrumDataType);
                float scaleFactor = 1f + 10 * particleSpeed * (SpectrumAnalyzer.GetArrayAverage(lowRangeArray));
                
                // Set particle velocity based on low frequency average
                ParticleSystem.VelocityOverLifetimeModule particleVelocity = pSystem.velocityOverLifetime;
                particleVelocity.speedModifierMultiplier = scaleFactor;

                // Set particle color brightness based on lower frequencies
                Color finalColor = particleSystemColor * pSystemMainSettings.simulationSpeed;
                //dustParticleMaterial.SetColor("_Color", finalColor);
                yield return null;
            }
            // If song isn't playing, set simulation speed to default
            pSystemMainSettings.simulationSpeed = 1;

            while(AudioPlaybackManager.musicPlaybackState == AudioPlaybackManager.PlaybackState.Pause)
            {
                yield return null;
            }

            StartCoroutine(VisualizeByVelocity());

            yield break;
        }
        private IEnumerator VisualizeBySimulationSpeed()
        {
            while (AudioPlaybackManager.musicPlaybackState == AudioPlaybackManager.PlaybackState.Play)
            {
                float[] lowRangeArray = spectrumAnalyzer.GetSpectrum(spectrumDataType);
                float scaleFactor = 1f + 10 * particleSpeed * (SpectrumAnalyzer.GetArrayAverage(lowRangeArray));
                // Set simulation speed of particle system to beatScaleFactor
                pSystemMainSettings.simulationSpeed = Mathf.Lerp(pSystemMainSettings.simulationSpeed,scaleFactor, 0.15f);

                // Set particle color brightness based on lower frequencies
                Color finalColor = particleSystemColor * pSystemMainSettings.simulationSpeed;
                //dustParticleMaterial.SetColor("_Color", finalColor);
                yield return null;
            }
            // If song isn't playing, set simulation speed to default
            pSystemMainSettings.simulationSpeed = 1;

            while(AudioPlaybackManager.musicPlaybackState == AudioPlaybackManager.PlaybackState.Pause)
            {
                yield return null;
            }

            StartCoroutine(VisualizeBySimulationSpeed());

            yield break;
        }
        #endregion
    }
   
}