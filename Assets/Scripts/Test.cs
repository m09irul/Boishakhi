using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{
    // The URL of the deployed web app
    private string url = "https://script.google.com/macros/s/AKfycbxWKVj0LQ6Hsht8czyg_QwUWriguiCbZ5CM0780LKZL6VbQDUN5qfL2f2KXtuWxMyrV5g/exec";

    // Start is called before the first frame update
    void Start()
    {

        // Send the data as a POST request to the web app's URL
       // StartCoroutine(PostRequest(url, PlayerPrefs.GetString("MyObject", "{}")));
        
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
    public IEnumerator PostRequest(string json)
    {
        // Create a new UnityWebRequest and set the method to POST
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
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
        }

        //StartCoroutine(GetRequest(url));
    }

}