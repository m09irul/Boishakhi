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
using System.Linq;

public class DailySellController : MonoBehaviour
{
    /// <summary>
    /// 0 = price field
    /// 1 = deposit field
    /// </summary>
    public TMP_InputField[] fields;
    public TextMeshProUGUI[] cashOutText;
    public TextMeshProUGUI[] totalSellText;
    public TextMeshProUGUI sellListTitleText;
    public TextMeshProUGUI priceTitleText;
    public Calculator calculator;
    public GameObject sellDataContainerPrefab;
    public GameObject dokanSellPanel;
    public Transform sellDataScrollbarViewport;
    string dateString, sellToInfo;
    public Button sellToButton;

    string mainJsonPref = "";
    string tmpJsonPref = "";
    string existingJson = "";
    string id = "";
    string price = "";
    string cash_in = "";
    string cash_out = "";

    private bool sellToOutside = true;

    public void OnStart()
    {
        CleanCalcAndFields();

        sellToButton.onClick.AddListener(() =>
        {
            if (sellToOutside)
                SellToOutside();
            else
                SellToHotel();
        });

        SubmitSellData(0);
        SubmitSellData(1);
    }
    public void OnEnable()
    {
        // Format the date as a string
        dateString = MainController.instance.GetToday();

        // Define an array of keys to update
        string[] keysToUpdate = new string[] {
        StringManager.DOKAN_SELL_MAIN,
        StringManager.DOKAN_SELL_TMP,
        StringManager.CIGAR_SELL_MAIN,
        StringManager.CIGAR_SELL_TMP
        };

        // Iterate over the keys and update their values
        foreach (string key in keysToUpdate)
        {
            PlayerPrefs.SetString(key, UpdateJsonForOldDates(PlayerPrefs.GetString(key, "{}")));
        }
    }

    private void Update()
    {
        bool isAnyFieldFocused = false;
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].isFocused)
            {
                isAnyFieldFocused = true;
                break;
            }

            if (string.IsNullOrEmpty(fields[i].text))
                fields[i].text = "0";
        }
        calculator.isCalculatorSelected = !isAnyFieldFocused;

        if (decimal.TryParse(fields[1].text, out decimal cash_inCigar) && decimal.TryParse(fields[0].text, out decimal priceCigar))
        {
            cashOutText[0].text = (cash_inCigar - priceCigar).ToString();
        }
        else
            cashOutText[0].text = "0";
        if (decimal.TryParse(fields[3].text, out decimal cash_inDokan) && decimal.TryParse(fields[2].text, out decimal priceDokan))
        {
            cashOutText[1].text = (cash_inDokan - priceDokan).ToString();
        }
        else
            cashOutText[1].text = "0";
    }

    public void BringCalcDataToInputField(int index)
    {
        if (index == 0)
        {
            fields[0].text = calculator.value.ToString();
        }
        else if (index == 1)
        {
            fields[2].text = calculator.value.ToString();
        }
    }
    public void SellTo(bool outside)
    {
        if (outside)
        {
            priceTitleText.text = "`vg:";
            dokanSellPanel.SetActive(true);
        }
        else
        {
            priceTitleText.text = "†nv‡Uj:";
            dokanSellPanel.SetActive(false);
        }
        UpdateSellToInfo(outside ? "Outside" : "Hotel");
        CleanInputFields();
    }

    public void SellToOutside()
    {
        SellTo(true);
    }

    public void SellToHotel()
    {
        SellTo(false);
    }
    void CleanInputFields()
    {
        foreach (var item in fields)
        {
            item.text = "0";
        }
    }
    public void UpdateSellToInfo(string sellTo)
    {
        sellToInfo = sellTo;

        if (sellToInfo.Equals("Outside"))
        {
            sellToButton.GetComponentInChildren<TextMeshProUGUI>().text = "†nv‡Uj wewµ"; // hotel bikri
            sellToOutside = false;
        }
        else if (sellToInfo.Equals("Hotel"))
        {
            sellToButton.GetComponentInChildren<TextMeshProUGUI>().text = "†`vKvb wewµ"; // dokan bikri
            sellToOutside = true;
        }
    }

    private (string mainJsonPref, string tmpJsonPref, string id, string price, string cash_in, string cash_out) GetValues(int index)
    {
        if (index == 0)
        {
            return (
                StringManager.CIGAR_SELL_MAIN,
                StringManager.CIGAR_SELL_TMP,
                StringManager.CIGAR_SELL_ID,
                fields[0].text,
                fields[1].text,
                cashOutText[0].text
            );
        }
        else
        {
            return (
                StringManager.DOKAN_SELL_MAIN,
                StringManager.DOKAN_SELL_TMP,
                StringManager.DOKAN_SELL_ID,
                fields[2].text,
                fields[3].text,
                cashOutText[1].text
            );
        }
    }

    private JObject GetJsonData(string key)
    {
        return JObject.Parse(PlayerPrefs.GetString(key, "{}"));
    }
    /// <summary>
    /// 0 = cigar
    /// 1 = dokan
    /// </summary>
    /// <param name="index"></param>
    public void SubmitSellData(int index)
    {

        // Get existing JSON data from PlayerPrefs
        var values = GetValues(index);
        mainJsonPref = values.mainJsonPref;
        tmpJsonPref = values.tmpJsonPref;
        id = values.id;
        price = values.price;
        cash_in = values.cash_in;
        cash_out = values.cash_out;

        string time = DateTime.Now.ToString("HH:mm:ss");

        
        existingJson = PlayerPrefs.GetString(tmpJsonPref, "{}");
        JObject tmpJsonData = GetJsonData(tmpJsonPref);
        JObject mainJsonData = GetJsonData(mainJsonPref);

        // Check if current date exists in JSON data

        JArray tmpDateArray; 
        JArray mainDateArray;

        // Check if current date exists in JSON data
        if (tmpJsonData[dateString] != null)
        {
            // Append new data to existing date array
            tmpDateArray = (JArray)tmpJsonData[dateString]["Data"];
            mainDateArray = (JArray)mainJsonData[dateString]["Data"];
        }
        else
        {
            PlayerPrefs.SetInt(id, 1);

            // Create new date array and append to JSON data
            tmpDateArray = new JArray();
            mainDateArray = new JArray();

            tmpJsonData[dateString] = new JObject();
            mainJsonData[dateString] = new JObject();

            tmpJsonData[dateString]["Data"] = tmpDateArray;
            mainJsonData[dateString]["Data"] = mainDateArray;
        }

        JObject newData = new JObject(
                new JProperty("id", PlayerPrefs.GetInt(id, 1)),
                new JProperty("sellTo", sellToInfo),
                new JProperty("price", price),
                new JProperty("cash_in", cash_in),
                new JProperty("cash_out", cash_out),
                new JProperty("time", time)
            );

        tmpDateArray.Add(newData);
        mainDateArray.Add(newData);

        // Increment ID in PlayerPrefs
        PlayerPrefs.SetInt(id, PlayerPrefs.GetInt(id, 1) + 1);

        //get total sell price
        mainJsonData[dateString]["Total Sell"] = (Convert.ToDouble(tmpJsonData[dateString]["Total Sell"]) + Convert.ToDouble(price)).ToString();
        tmpJsonData[dateString]["Total Sell"] = (Convert.ToDouble(tmpJsonData[dateString]["Total Sell"]) + Convert.ToDouble(price)).ToString();

        // Store updated JSON data in PlayerPrefs
        PlayerPrefs.SetString(tmpJsonPref, tmpJsonData.ToString());
        PlayerPrefs.SetString(mainJsonPref, mainJsonData.ToString());

        UpdateTotalSellUI(mainJsonData[dateString]["Total Sell"].ToString(), index);

        StartCoroutine(MainController.instance.PostRequest(PlayerPrefs.GetString(tmpJsonPref, "{}"), index + 1, UpdateJsonOnEachSell));

        CleanCalcAndFields(index);
    }
    string UpdateJsonForOldDates(string jsonString)
    {
        // Parse the JSON string into a JObject
        JObject jsonData = JObject.Parse(jsonString);

        // Get the current date
        DateTime currentDate = DateTime.Now;

        // Iterate over the keys of the JObject
        foreach (var key in jsonData.Properties().ToList())
        {
            // Parse the date from the key
            DateTime date = DateTime.Parse(key.Name);

            // Calculate the difference between the current date and the date in the JSON object
            TimeSpan difference = currentDate - date;

            // If the difference is greater than 4 days, remove the key-value pair from the JObject
            if (difference.TotalDays > 4)
            {
                jsonData.Remove(key.Name);
            }
        }

        // Convert the modified JObject back to a JSON string
        string updatedJsonString = jsonData.ToString();

        return updatedJsonString;

    }
    void UpdateJsonOnEachSell(string respose)
    {
        JObject jObj1 = JObject.Parse(respose);
        JObject jObj2 = JObject.Parse(PlayerPrefs.GetString(tmpJsonPref, "{}"));

        var data1 = jObj1[dateString]["Data"].Children().Select(d => d.ToString()).ToHashSet();
        var data2 = jObj2[dateString]["Data"].Children().Select(d => d.ToString());

        var newData = new JArray(data2.Except(data1).Select(d => JObject.Parse(d)));

        jObj2[dateString]["Data"] = newData;

        PlayerPrefs.SetString(tmpJsonPref, jObj2.ToString());
    }
    void UpdateTotalSellUI(string data, int index)
    {
        totalSellText[index].text = data;
    }

    private void CleanCalcAndFields(int index = -1)
    {
        int start = index == -1 ? 0 : (index + 1) * index;
        int end = index == -1 ? fields.Length - 1 : (index + 1) + index;

        for (int i = start; i <= end; i++)
        {
            fields[i].text = "0";
        }


        calculator.OnPressedClearAll();
    }

    public void OnSellListClicked(int index)
    {
        //clean the container
        for (int i = 0; i < sellDataScrollbarViewport.childCount; i++)
        {
            sellDataScrollbarViewport.GetChild(i).gameObject.SetActive(false);
        }

        if (index == 0)
        {
            mainJsonPref = StringManager.CIGAR_SELL_MAIN;
            sellListTitleText.text = "†ePv-‡Kbv wj÷(wmMv‡iU)";
        }

        else if (index == 1)
        {

            mainJsonPref = StringManager.DOKAN_SELL_MAIN;
            sellListTitleText.text = "†ePv-‡Kbv wj÷(†`vKvb)";
        }

        // Get existing JSON data from PlayerPrefs
        existingJson = PlayerPrefs.GetString(mainJsonPref, "{}");

        JObject jsonData = JObject.Parse(existingJson);

        // Check if current date exists in JSON data
        if (jsonData[dateString] != null)
        {
            JArray myArray = (JArray)jsonData[dateString]["Data"];

            GameObject tmp;

            for (int i = myArray.Count - 1; i >= 0; i--)
            {
                if (myArray.Count - i <= sellDataScrollbarViewport.childCount)
                {
                    tmp = sellDataScrollbarViewport.GetChild(myArray.Count - i - 1).gameObject;
                    tmp.SetActive(true);
                }

                else
                    tmp = Instantiate(sellDataContainerPrefab, sellDataScrollbarViewport);

                tmp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = myArray[i]["id"].ToString();
                tmp.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = myArray[i]["price"].ToString();
                tmp.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = myArray[i]["cash_in"].ToString();
                tmp.gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = myArray[i]["cash_out"].ToString();
                tmp.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = dateString + "    " + myArray[i]["time"].ToString();
            }
        }

        
    }

}
