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
        string ExecutePythonScript(string filePythonScript, out string standardError);
    }
}