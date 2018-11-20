using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.AudioAnalyzer
{
    public class AudioSpectrum : MonoBehaviour 
    {
        public enum AudioDataReturnType
        {
            LowRange,
            HighRange,
            DefaultBands,
        }
        #region Private Variables
        AudioDataReturnType audioFilter;

        private static int sampleSize = 1024;

        private float[] audioSpectrumRawData = new float[sampleSize];

        private bool isAudioSourceAttached = false;

        #endregion

        #region Serialized Variables

        [SerializeField, Tooltip("In case of no Audio Source, your audio listener will be use automatically.")]
        private AudioSource audioSource;

        #endregion

        #region Unity Methods

        private void Awake() 
        {
            if(audioSource)
                isAudioSourceAttached = true;
        }

        private void Start() 
        {

        }
        
        // TODO Create a coroutine instead of an Update function
        private void Update() 
        {
            // If audio source is not attached, then use audio listener as an audio input for getting spectrum data
            if(!isAudioSourceAttached)
            {
                AudioListener.GetSpectrumData(audioSpectrumRawData, 0, FFTWindow.BlackmanHarris);
                return;
            }
            audioSource.GetSpectrumData(audioSpectrumRawData, 0, FFTWindow.BlackmanHarris);
        }

        #endregion
        #region Private Methods
        /// <summary>
        /// Get spectrum data as a float array. Resource intensive. Use it only once in a frame and save it into an array for further use
        /// </summary>
        /// <param name="filter"> How to filter the spectrum data </param>
        /// <returns></returns>
        public float[] GetSpectrum(AudioDataReturnType filter)
        {
            switch (filter)
            {
                case AudioDataReturnType.LowRange:
                    return AudioSpectrumAnalyzer.GetLowFrequencyRangeSpectrum(audioSpectrumRawData);
                case AudioDataReturnType.HighRange:
                    return AudioSpectrumAnalyzer.GetHighFrequencyRangeSpectrum(audioSpectrumRawData);
                case AudioDataReturnType.DefaultBands:
                    return AudioSpectrumAnalyzer.DivideIntoFrequencyBands(audioSpectrumRawData);
                default:
                    return audioSpectrumRawData;
            }
        }
        #endregion
    }
}
