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
        private string filePythonExePath = "C:/Users/me/AppData/Local/Programs/Python/Python36/python.exe";

        /// <summary>
        /// Without arguments
        /// /// </summary>
        /// <param name="filePythonScript"></param>
        /// <param name="standardError"></param>
        /// <param name="output"></param>
        public void ExecutePythonScript(string filePythonScript, out string standardError, out string output)
        {
            standardError = string.Empty;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filePythonExePath;
            startInfo.Arguments = string.Format("\"{0}\"", filePythonScript);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    standardError = process.StandardError.ReadToEnd();
                    string result = reader.ReadToEnd();
                    output = result;
                }
            }
        }

        /// <summary>
        /// Run the model with args
        /// </summary>
        /// <param name="filePythonScript"></param>
        /// <param name="args"></param>
        /// <param name="standardError"></param>
        /// <param name="output"></param>
        public void ExecutePythonScript(string filePythonScript, string args, out string standardError, out string output)
        {
            standardError = string.Empty;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filePythonExePath;
            startInfo.Arguments = string.Format("\"{0}\"", filePythonScript, args);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    standardError = process.StandardError.ReadToEnd();
                    string result = reader.ReadToEnd();
                    output = result;
                }
            }
        }
    }

}
