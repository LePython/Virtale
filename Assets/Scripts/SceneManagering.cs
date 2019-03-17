using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagering : MonoBehaviour
{
    [SerializeField]
    private int scene;

    public void ChangeScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeScene(scene);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        UnityEngine.AudioManager.AudioPlaybackManager.musicPlaybackState = UnityEngine.AudioManager.AudioPlaybackManager.PlaybackState.Pause;
        UnityEngine.AudioManager.AudioPlaybackManager.SongNumber = 0;
    }
}
