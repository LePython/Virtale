using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEngine.AudioManager
{
    public static class AudioFileGetter
    {
        /// <summary>
        /// Gets all files with certain extensions from the specified directory and stores them in a IEnumerable<FileInfo>
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if(extensions == null)
                Debug.LogError("There are no file extensions.");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }

    }
}