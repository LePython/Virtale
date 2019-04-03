using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

/// <summary>
/// The ServerHandler class handles the communication between the
/// server and the client. The GET and POST requests are helping
/// the communication to occur.
/// </summary>
public class ServerHandler : MonoBehaviour
{
    public delegate void GetRequestEnd();

    public struct AudioAnalyze
    {
        public string url;
        public string format;
        public string key;
    }

    public struct AudioData
    {
        public string name;
        public string featuregroup;
    }

    #region Private Variables

    public GetRequestEnd OnRequestEnd;

    private string getRequestOutput;

    private List<AudioData> requestedAudioList = new List<AudioData>();

    #endregion

    #region Parameters

    public List<AudioData> RequestedAudioList { get => (requestedAudioList.Count <= 0) ? null : requestedAudioList; }
    public string GetRequestOutput { get => getRequestOutput; set => getRequestOutput = value; }

    #endregion

    #region Enumerators
    /// Underneath the "Enumerators" all enumerators for communication with the server are stored.
    /// This means a new Coroutine has to be started to run them.


    /// <summary>
    /// Get request from the specified uri
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public IEnumerator GetRequest(string uri)
    {

        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if(request.isNetworkError)
        {
            Debug.Log("Error on GetRequest: " + request.error);
        }else
        {
            Debug.Log("Request successful: " + request.downloadHandler.text);
            getRequestOutput = request.downloadHandler.text;
            Debug.Log(getRequestOutput);
        }
    }
    /// <summary>
    /// This enumerator achieves the same thing as the original one.
    /// The main difference is that you can call a function after every
    /// get request by giving a function as a second parameter of GetRequest().
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="endRequest"></param>
    /// <returns></returns>
    public IEnumerator GetRequest(string uri, GetRequestEnd endRequest)
    {

        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if(request.isNetworkError)
        {
            Debug.Log("Error on GetRequest: " + request.error);
        }else
        {
            Debug.Log("Request successful: " + request.downloadHandler.text);
            GetRequestOutput = request.downloadHandler.text;
        }
        endRequest();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jsonBody"></param>
    /// <returns></returns>
    public IEnumerator PostRequest(string uri, string jsonBody)
    {
        UnityWebRequest request = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Form upload complete!" + request.downloadHandler.text);
        }
    }

    #endregion

    /// <summary>
    /// If the get request output is not zero, convert the string json array to the AudioData type list
    /// </summary>
    public void GetAudioList()
    {
        if(!string.IsNullOrEmpty(GetRequestOutput))
        {
            requestedAudioList = JsonConvert.DeserializeObject<List<AudioData>>(GetRequestOutput);
        }
    }

    
}
