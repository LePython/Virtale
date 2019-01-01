using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace RunPythonScript
{
    public class MLModelLoadingManager : IMLSharpPython
    {
        //C:/Users/me/AppData/Local/Programs/Python/Python36
        //C:/Windows/py.exe

        private bool isProcessingDone = false;
        private string filePythonExePath = @"C:/Users/me/AppData/Local/Programs/Python/Python36/python.exe";

        /// <summary>
        /// Without arguments
        /// /// </summary>
        /// <param name="filePythonScript"></param>
        /// <param name="standardError"></param>
        /// <param name="output"></param>
        public void ExecutePythonScript(string filePythonScript, out string standardError, out string output)
        {
            standardError = string.Empty;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = filePythonExePath,
                Arguments = string.Format("{0}", filePythonScript),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            Process process = Process.Start(startInfo);
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(process_Exited);

            using (StreamReader reader = process.StandardOutput)
            {
                standardError = process.StandardError.ReadToEnd();
                string result = reader.ReadToEnd();
                output = result;
            }
            process.WaitForExit();
        }
        void process_Exited(object sender, EventArgs e)
        {
            isProcessingDone = true;
        }

        public bool IsProcessingDone
        {
            get => isProcessingDone;
            set => value = isProcessingDone;
        }
    }

}
