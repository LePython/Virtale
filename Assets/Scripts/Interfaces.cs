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

public interface IMLSharpPython
{
    //void ExecutePythonScript(string filePythonScript, out string standardError, out string output);
    void ExecutePythonScript(string filePythonScript, out string standardError, out string output);
}
