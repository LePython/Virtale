using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.AudioAnalyzer
{
    public class SpectrumAnalyzer : MonoBehaviour
    {

        public enum AudioDataReturnType
        {
            LowRange,
            HighRange,
            DefaultBands,
            CustomBands
        }

        #region Private Variables

        // Samples to analyze every frame
        private static int sampleSize = 1024;

        // Divided data into frequency bands
        private static int spectrumNodeSize = 8;

        private float[] audioSpectrumRawData = new float[sampleSize];

        [SerializeField]
        private AudioSource audioSource;

        private void Awake() {
            audioSource = GetComponent<AudioSource>();
            if(!audioSource)
            {
                Debug.LogError("There is no Audio Source attached to the script.");
                enabled = false;
            }
        }

        private void Update() {
            audioSource.GetSpectrumData(audioSpectrumRawData, 0, FFTWindow.Hamming);
        }

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
                    return GetLowFrequencyRangeSpectrum();
                case AudioDataReturnType.HighRange:
                    return GetHighFrequencyRangeSpectrum();
                case AudioDataReturnType.DefaultBands:
                    return DivideIntoFrequencyBands();
                default:
                    return AudioSpectrumRawData;
            }
        }
        public float[] GetDividedSpectrum <T>(AudioDataReturnType filter, List<T> objs)
        {
            switch (filter)
            {
                case AudioDataReturnType.CustomBands:
                    return DivideIntoFrequencyBands(objs.Count);
                default:
                    return AudioSpectrumRawData;
            }
        }

        /// <summary>
        /// Assigns low range values to a new array, ignoring pass band and high range
        /// </summary>
        /// <returns></returns>
        private float[] GetLowFrequencyRangeSpectrum()
        {

            int granularity = 22050 / sampleSize;

            // Frequency to stop assigning values to the array
            int cutoffFrequency = 300;
            int samplesNeeded = cutoffFrequency / granularity;

            // Create new array to set the filtered values to
            float[] filteredSignal = new float[samplesNeeded];

            for (int i = 0; i < samplesNeeded; i++)
            {
                filteredSignal[i] = audioSpectrumRawData[i];
            }

            return filteredSignal;
        }
        /// <summary>
        /// Assigns high range values to a new array, ignoring low range and pass band
        /// </summary>
        /// <returns></returns>
        private float[] GetHighFrequencyRangeSpectrum()
        {

            int granularity = 22050 / sampleSize;

            // Frequency to start setting values to the array
            int cutoffFrequency = 5000;
            int samplesNeeded = cutoffFrequency / granularity;

            // Create new array to set the filtered values to
            float[] filteredSignal = new float[sampleSize - samplesNeeded];

            for (int i = samplesNeeded; i < sampleSize; i++)
            {
                filteredSignal[i - samplesNeeded] = audioSpectrumRawData[i];
            }
            return filteredSignal;
        }
        /// <summary>
        /// Filters the signal with a Butterworth Filter
        /// </summary>
        /// <returns> Returns the filtered signal as a float array instead of double</returns>
        private float[] GetFilteredSpectrumData()
        {
            // Cast float array to double array
            double[] spectrumDatad = Array.ConvertAll(audioSpectrumRawData, item => (double)item);
            double[] spectrumDataOut = Butterworth(spectrumDatad, 0.10, 1);
            float[] spectrumDatafloat = Array.ConvertAll(spectrumDataOut, item => (float)item);

            return DivideIntoFrequencyBands(spectrumDatafloat);
        }

        //--------------------------------------------------------------------------
        // This function returns the data filtered. Converted to C# 2 July 2014.
        // Original source written in VBA for Microsoft Excel, 2000 by Sam Van
        // Wassenbergh (University of Antwerp), 6 june 2007.
        //--------------------------------------------------------------------------
        private double[] Butterworth(double[] indata, double deltaTimeinsec, double CutOff)
        {
            if (indata == null) return null;
            if (CutOff == 0) return indata;

            double Samplingrate = 1 / deltaTimeinsec;
            long dF2 = indata.Length - 1;        // The data range is set with dF2
            double[] Dat2 = new double[dF2 + 4]; // Array with 4 extra points front and back
            double[] data = indata; // Ptr., changes passed data

            // Copy indata to Dat2
            for (long r = 0; r < dF2; r++)
            {
                Dat2[2 + r] = indata[r];
            }
            Dat2[1] = Dat2[0] = indata[0];
            Dat2[dF2 + 3] = Dat2[dF2 + 2] = indata[dF2];

            double wc = Mathf.Tan((float)CutOff * Mathf.PI / (float)Samplingrate);
            double k1 = 1.414213562 * wc; // Sqrt(2) * wc
            double k2 = wc * wc;
            double a = k2 / (1 + k1 + k2);
            double b = 2 * a;
            double c = a;
            double k3 = b / k2;
            double d = -2 * a + k3;
            double e = 1 - (2 * a) - k3;

            // RECURSIVE TRIGGERS - ENABLE filter is performed (first, last points constant)
            double[] DatYt = new double[dF2 + 4];
            DatYt[1] = DatYt[0] = indata[0];
            for (long s = 2; s < dF2 + 2; s++)
            {
                DatYt[s] = a * Dat2[s] + b * Dat2[s - 1] + c * Dat2[s - 2]
                           + d * DatYt[s - 1] + e * DatYt[s - 2];
            }
            DatYt[dF2 + 3] = DatYt[dF2 + 2] = DatYt[dF2 + 1];

            // FORWARD filter
            double[] DatZt = new double[dF2 + 2];
            DatZt[dF2] = DatYt[dF2 + 2];
            DatZt[dF2 + 1] = DatYt[dF2 + 3];
            for (long t = -dF2 + 1; t <= 0; t++)
            {
                DatZt[-t] = a * DatYt[-t + 2] + b * DatYt[-t + 3] + c * DatYt[-t + 4]
                            + d * DatZt[-t + 1] + e * DatZt[-t + 2];
            }

            // Calculated points copied for return
            for (long p = 0; p < dF2; p++)
            {
                data[p] = DatZt[p];
            }

            return data;
        }

        /// <summary>
        /// Divides the spectrum data into 8 frequency bands.
        /// The bands are the average of that band's frequency range
        /// </summary>
        /// <param name="data"> Data to process</param>
        /// <param name="divideTimes"> How many times to divide the data ( 8 minimum )</param>
        private float[] DivideIntoFrequencyBands(float[] data)
        {

            float[] endData = new float[SpectrumNodeSize];

            int currentSample = 0;

            for (int i = 0; i < SpectrumNodeSize; i++)
            {

                int band = CalculateBandsAuto(i);

                float averageOfFrequencyBand = 0;

                for (int j = 0; j < band; j++)
                {
                    averageOfFrequencyBand += data[currentSample] * (currentSample + 1);
                    currentSample++;
                }
                averageOfFrequencyBand /= band;
                endData[i] = averageOfFrequencyBand;
            }

            return endData;
        }
        // Divides data into frequency bands for a preffered lsit size
        private float[] DivideIntoFrequencyBands(int listSize)
        {
            List<float> endData = new List<float>();

            int currentSample = 0;

            int samples = (sampleSize - 256)/listSize;

            for(int i = 0; i < listSize; i ++)
            {
                float averageOfFrequencyBand = 0;

                for(int band = 0; band < samples; band++ )
                {
                    averageOfFrequencyBand += audioSpectrumRawData[currentSample] * (currentSample + 1);
                    currentSample++;
                }
                averageOfFrequencyBand /= samples;
                endData.Add(averageOfFrequencyBand);
            }
            // for (int i = 0; i < 8; i++)
            // {

            //     int band = CalculateBandsAuto(i);
            //     //Debug.Log(band);
            //     float averageOfFrequencyBand = 0;


            //     for(int a = 1; a <= listSize/8; a++)
            //     {
            //         for (int j = 0; j < band/(listSize/8); j++)
            //         {
            //             averageOfFrequencyBand += audioSpectrumRawData[currentSample] * (currentSample + 1);
            //             currentSample++;
            //         }
            //         averageOfFrequencyBand /= band;
            //         endData.Add(averageOfFrequencyBand);
            //     }
            // }

            return endData.ToArray();
        }
        // Automatically assigns 8 bands
        private float[] DivideIntoFrequencyBands()
        {

            float[] endData = new float[spectrumNodeSize];

            int currentSample = 0;

            for (int i = 0; i < spectrumNodeSize; i++)
            {

                int band = CalculateBandsAuto(i);
                float averageOfFrequencyBand = 0;

                for (int j = 0; j < band; j++)
                {
                    averageOfFrequencyBand += audioSpectrumRawData[currentSample] * (currentSample + 1);
                    currentSample++;
                }
                averageOfFrequencyBand /= band;
                endData[i] = averageOfFrequencyBand;
            }

            return endData;
        }

        public static float GetArrayAverage(float[] data)
        {
            float endValue = 0f;
            for (int i = 0; i < data.Length; i++)
            {
                endValue += data[i];
            }
            endValue /= data.Length;
            return endValue;
        }
        // Calculates bands with the preferred node size
        // Works only with 1024 sample size
        // maximum size is 32, minimum is 8
        // size must be a number of 2^n
        private int CalculateBands(int n, int size)
        {
            int band = 0;
            if(size == 8)
            {
                band = (sampleSize/ 512 * 2) * (int)Mathf.Pow(2, n);
            }
            else if(size == 16)
            {
                band = ((sampleSize/ 512 * 2) * (int)Mathf.Pow(1.31732f, n));
            }
            else if(size == 32)
            {
                band = ((sampleSize/ 512 * 2) * (int)Mathf.Pow(1.11166f, n));
            }

            return band;
        }
        #endregion

        #region Lambda Expressions
        // Add ((sampleSize/512) * 2) instead of 2*.. if you wish to have variable sample size( 1024 samples on default )
        // Works only with spectrum node count of 8
        public static Func<int, int> CalculateBandsAuto = band => ((sampleSize / 512) * 2) * (int)Mathf.Pow(2, band);
        #endregion

        #region Properties


        public int SpectrumNodeSize
        {
            get
            {
                return spectrumNodeSize;
            }

            set
            {
                spectrumNodeSize = value;
            }
        }

        public float[] AudioSpectrumRawData
        {
            get
            {
                return audioSpectrumRawData;
            }
        }
        #endregion
    }
}

