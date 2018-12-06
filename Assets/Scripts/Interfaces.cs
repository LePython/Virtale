using UnityEngine;
using System.Collections;

public interface IMusicPlayer
{
    void PlayAudio();
    void PauseAudio();
    void NextSong();
    void LastSong();
    void SelectSong(int index);
}

namespace RunPythonScript
{
    public interface IMLSharpPython
    {
        void ExecutePythonScript(string filePythonScript, out string standardError, out string output);
    }
}