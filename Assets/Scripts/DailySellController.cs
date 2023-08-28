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

    private void Start()
    {
        // Format the date as a string
        dateString = DateTime.Today.ToString("yyyy-MM-dd");
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
        {
            cashOutText[0].text = "0";
        }

        if (decimal.TryParse(fields[3].text, out decimal cash_inDokan) && decimal.TryParse(fields[2].text, out decimal priceDokan))
        {
            cashOutText[1].text = (cash_inDokan - priceDokan).ToString();
        }
        else
        {
            cashOutText[1].text = "0";
        }
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
    public void SellToOutside()
    {
        priceTitleText.text = "`vg:";
        dokanSellPanel.SetActive(true);
        UpdateSellToInfo("Outside");
        CleanInputFields();
    }
    public void SellToHotel()
    {
        priceTitleText.text = "†nv‡Uj:";
        dokanSellPanel.SetActive(false);
        UpdateSellToInfo("Hotel");
        CleanInputFields();
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
            sellToButton.onClick.RemoveListener(() => SellToOutside());
            sellToButton.onClick.AddListener(() => SellToHotel());

            sellToButton.GetComponentInChildren<TextMeshProUGUI>().text = "†nv‡Uj wewµ"; // hotel bikri
        }
        else if (sellToInfo.Equals("Hotel"))
        {
            sellToButton.onClick.RemoveListener(() => SellToHotel());
            sellToButton.onClick.AddListener(() => SellToOutside());

            sellToButton.GetComponentInChildren<TextMeshProUGUI>().text = "†`vKvb wewµ"; // dokan bikri
        }
    }

    /// <summary>
    /// 0 = cigar
    /// 1 = dokan
    /// </summary>
    /// <param name="index"></param>
    public void SubmitSellData(int index)
    {
        string jsonPrefId = "";
        string existingJson = "";
        string id = "";
        string price = "";
        string cash_in = "";
        string cash_out = "";

        if (index == 0)
        {
            // Get existing JSON data from PlayerPrefs
            jsonPrefId = "CigarSell";
            id = "CigarSellId";
            price = fields[0].text;
            cash_in = fields[1].text;
            cash_out = cashOutText[0].text;
        }
        else if (index == 1)
        {
            // Get existing JSON data from PlayerPrefs
            jsonPrefId = "DokanSell";
            id = "DokanSellId";
            price = fields[2].text;
            cash_in = fields[3].text;
            cash_out = cashOutText[1].text;
        }

        string time = DateTime.Now.ToString("HH:mm:ss");

        existingJson = PlayerPrefs.GetString(jsonPrefId, "{}");
        JObject jsonData = JObject.Parse(existingJson);
        JArray dateArray;

        // Check if current date exists in JSON data
        if (jsonData[dateString] != null)
        {
            // Append new data to existing date array
            dateArray = (JArray)jsonData[dateString];
        }
        else
        {
            PlayerPrefs.SetInt(id, 1);

            // Create new date array and append to JSON data
            dateArray = new JArray();

        }

        JObject newData = new JObject(
                new JProperty("id", PlayerPrefs.GetInt(id, 1)),
                new JProperty("sellTo", sellToInfo),
                new JProperty("price", price),
                new JProperty("cash_in", cash_in),
                new JProperty("cash_out", cash_out),
                new JProperty("time", time)
            );
        dateArray.Add(newData);
        jsonData[dateString] = dateArray;

        // Increment ID in PlayerPrefs
        PlayerPrefs.SetInt(id, PlayerPrefs.GetInt(id, 1) + 1);

        //get total sell price
        int sum = 0;
        foreach (var item in jsonData[dateString])
        {
            if (int.TryParse((string)item["price"], out int tmpPrice))
            {
                sum += tmpPrice;
            }
        }

        // Store updated JSON data in PlayerPrefs
        string json = jsonData.ToString();
        Debug.Log(json);

        PlayerPrefs.SetString(jsonPrefId, json);

        UpdateTotalSellUI(sum.ToString(), index);
        CleanInputFields();

    }
    void UpdateTotalSellUI(string data, int index)
    {
        totalSellText[index].text = data;  
    }
    public void OnSellListClicked(int index)
    {
        //clean the container
        for (int i = 0; i < sellDataScrollbarViewport.childCount; i++)
        {
            sellDataScrollbarViewport.GetChild(i).gameObject.SetActive(false);
        }

        string jsonPrefId = "";
        string existingJson = "";

        if (index == 0)
        {
            jsonPrefId = "CigarSell";
            sellListTitleText.text = "†ePv-‡Kbv wj÷(wmMv‡iU)";
        }

        else if (index == 1)
        {
            
            jsonPrefId = "DokanSell";
            sellListTitleText.text = "†ePv-‡Kbv wj÷(†`vKvb)";
        }

        // Get existing JSON data from PlayerPrefs
        existingJson = PlayerPrefs.GetString(jsonPrefId, "{}");

        JObject jsonData = JObject.Parse(existingJson);

        // Check if current date exists in JSON data
        if (jsonData[dateString] != null)
        {
            JArray myArray = (JArray)jsonData[dateString];

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
