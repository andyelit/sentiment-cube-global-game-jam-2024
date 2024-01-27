using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Events;

public class APICallExample : MonoBehaviour
{
    public string apiKey = "RkzqStmQ0edqXis0Nbq2Zg==ydwnbSR9hA16FLFj";
    public string apiURL = "https://api.api-ninjas.com/v1/sentiment";

    [System.Serializable]
    public class RandomWordReponse
    {
        public string word;
    }

    [System.Serializable]
    public class SentimentResponse
    {
        public float score;
        public string text;
        public string sentiment;
    }

    public IEnumerator GetRandomWord(UnityAction<string> OnComplete)
    {
        string _apiURL = "https://api.api-ninjas.com/v1/randomword";

        UnityWebRequest request = UnityWebRequest.Get(_apiURL);
        request.SetRequestHeader("X-Api-Key", apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            OnComplete?.Invoke(request.downloadHandler.text);
        }
    }

    public IEnumerator GetSentiment(string text, UnityAction<string> OnComplete)
    {
        string _apiURL = $"{apiURL}?text={UnityWebRequest.EscapeURL(text)}";

        UnityWebRequest request = UnityWebRequest.Get(_apiURL);
        request.SetRequestHeader("X-Api-Key", apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);            

            OnComplete?.Invoke(request.downloadHandler.text);
        }
    }
}
