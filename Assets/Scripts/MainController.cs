using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text;
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
    public ManagementController managementController;
    public DailySellController dailySellController;

    // The URL of the deployed web app
    private string url = "https://script.google.com/macros/s/AKfycbylK6ALrw5nvNaIVDHjtK6nHUQsbemGbQtr62nwpxel8781LW9UX2_TdIpN8a-mk-I/exec";

    private void Start()
    {
        dailySellController.OnStart();
        managementController.OnStart();
    }
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
    public IEnumerator GetRequest(string param, Action<string> callBack)
    {
        // Append the parameter to the URL
        var tmpUrl = url + "?" + param;
        // Create a new UnityWebRequest and set the method to GET
        var request = new UnityWebRequest(tmpUrl, "GET");
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

            callBack(request.downloadHandler.text);  
        }
    }
    public IEnumerator PostRequest(string json, int intValue, Action<string> successCallBack = null, Action<string> errorCallBack = null)
    {
        // Create a new UnityWebRequest and set the method to POST
        var request = new UnityWebRequest(url, "POST");
        string requestBody = "{\"jsonData\":" + json + ",\"intValue\":" + intValue + "}";
        print(requestBody);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            errorCallBack(request.error);
        }
        else
        {
            successCallBack(request.downloadHandler.text);
        }
    }
    public void OnExitApplication()
    {
        Application.Quit(0);
    }
}