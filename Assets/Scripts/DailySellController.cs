using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InteractiveCalculator;
using TMPro;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using System.IO;

public class DailySellController : MonoBehaviour
{
    /// <summary>
    /// 0 = price field
    /// 1 = deposit field
    /// </summary>
    public TMP_InputField[] fields;
    public TextMeshProUGUI returnText;
    public Calculator calculator;
    public GameObject sellDataContainerPrefab;
    public Transform sellDataScrollbarViewport;
    string dateString;
    public void BringCalcDataToInputField(int index)
    {
        if (index == 0)
        {
            fields[0].text = calculator.value.ToString();
        }
        else if (index == 1)
        {
            fields[1].text = calculator.value.ToString();
        }
    }
    private void Start()
    {
        // Format the date as a string
        dateString = DateTime.Today.ToString("yyyy-MM-dd");
    }
    private void Update()
    {
        if (fields[0].isFocused || fields[1].isFocused)
        {
            calculator.isCalculatorSelected = false;
        }
        else
            calculator.isCalculatorSelected = true;

        if (decimal.TryParse(fields[1].text, out decimal depositAmount) && decimal.TryParse(fields[0].text, out decimal price))
        {
            returnText.text = (depositAmount - price).ToString();
        }
        else
        {
            returnText.text = "0";
        }
    }
    public void SubmitSellData()
    {
        // Get existing JSON data from PlayerPrefs
        string existingJson = PlayerPrefs.GetString("MyObject", "{}");
        JObject jsonData = JObject.Parse(existingJson);

        // Check if current date exists in JSON data
        if (jsonData[dateString] != null)
        {
            // Append new data to existing date array
            JArray dateArray = (JArray)jsonData[dateString];
            JObject newData = new JObject(
                new JProperty("id", PlayerPrefs.GetInt("ID", 1)),
                new JProperty("price", fields[0].text),
                new JProperty("cash_in", fields[1].text),
                new JProperty("cash_out", returnText.text)
            );
            dateArray.Add(newData);
        }
        else
        {
            PlayerPrefs.SetInt("ID", 1);

            // Create new date array and append to JSON data
            JArray dateArray = new JArray();
            JObject newData = new JObject(
                new JProperty("id", PlayerPrefs.GetInt("ID", 1)),
                new JProperty("price", fields[0].text),
                new JProperty("cash_in", fields[1].text),
                new JProperty("cash_out", returnText.text)
            );
            dateArray.Add(newData);
            jsonData[dateString] = dateArray;
        }

        // Increment ID in PlayerPrefs
        PlayerPrefs.SetInt("ID", PlayerPrefs.GetInt("ID", 1) + 1);

        // Store updated JSON data in PlayerPrefs
        string json = jsonData.ToString();
        Debug.Log(json);
        PlayerPrefs.SetString("MyObject", json);
    }
    public void OnSellListClicked()
    {
        // Get existing JSON data from PlayerPrefs
        string existingJson = PlayerPrefs.GetString("MyObject", "{}");

        JObject jsonData = JObject.Parse(existingJson);

        // Check if current date exists in JSON data
        if (jsonData[dateString] != null)
        {
            JArray myArray = (JArray)jsonData[dateString];

            for (int i = myArray.Count - 1; i >= 0; i--)
            {
                if (myArray.Count - i <= sellDataScrollbarViewport.childCount)
                {
                    var tmp = sellDataScrollbarViewport.GetChild(myArray.Count - i - 1);
                    tmp.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = myArray[i]["id"].ToString() + "               " +
                        myArray[i]["price"].ToString() + "               " +
                        myArray[i]["cash_in"].ToString() + "               " +
                        myArray[i]["cash_out"].ToString() + "               " +
                        dateString;
                }
                else
                {
                    var tmp = Instantiate(sellDataContainerPrefab, sellDataScrollbarViewport);
                    tmp.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = myArray[i]["id"].ToString() + "               " +
                        myArray[i]["price"].ToString() + "               " +
                        myArray[i]["cash_in"].ToString() + "               " +
                        myArray[i]["cash_out"].ToString() + "               " +
                        dateString;
                }
                
                
            }
        }

        
    }

}
