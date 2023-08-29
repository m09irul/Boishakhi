using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MainController : MonoBehaviour
{
    public static MainController instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }
    // The URL of the deployed web app
    private string url = "https://script.google.com/macros/s/AKfycbxWKVj0LQ6Hsht8czyg_QwUWriguiCbZ5CM0780LKZL6VbQDUN5qfL2f2KXtuWxMyrV5g/exec";

    public string GetToday()
    {
        DateTime now = DateTime.Now;

        if (now.Hour < 3)
        {
            return now.AddDays(-1).Date.ToString("yyyy-MM-dd");
        }
        else
        {
            return now.Date.ToString("yyyy-MM-dd");
        }
    }
    IEnumerator GetRequest(string url)
    {
        // Create a new UnityWebRequest and set the method to GET
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }
    public IEnumerator PostRequest(string json, int intValue, Action<string> callBack)
    {
        // Create a new UnityWebRequest and set the method to POST
        var request = new UnityWebRequest(url, "POST");
        string requestBody = "{\"jsonData\":" + json + ",\"intValue\":" + intValue + "}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            callBack(request.downloadHandler.text);
        }

        //StartCoroutine(GetRequest(url));
    }

}