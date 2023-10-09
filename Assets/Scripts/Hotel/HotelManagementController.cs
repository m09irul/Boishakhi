using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InteractiveCalculator;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
public class HotelManagementController : MonoBehaviour
{
    public TextMeshProUGUI[] S_D_expenseTexts;

    public TextMeshProUGUI hotelCashTillToday;
    public TextMeshProUGUI shantoCashTillToday;
    [Space]
    public TextMeshProUGUI dailySellSummaryText;
    public TextMeshProUGUI dailyHotelSummaryText;
    public TextMeshProUGUI dailyShantoSummaryText;

    public GameObject expensePurchaseContainerPrefab;
    public GameObject cigarShantoContainerPrefab;

    public Button expenseAddButton;
    public Button purchaseAddButton;

    public Button shantoCashInAddButton;
    public Button shantoCashOutAddButton;

    [Space]
    public Transform expenseContainerParent;
    public Transform purchaseContainerParent;

    public Transform shantoCashInContainerParent;
    public Transform shantoCashOutContainerParent;

    public TMP_InputField bakiRecievedField;
    public TMP_InputField nicheField;
    public TextMeshProUGUI shabekText;
    public TMP_InputField cashField;

    public List<TMP_InputField> expenseFields;
    public List<TMP_InputField> purchaseFields;

    public List<TMP_InputField> shantoCashInFields;
    public List<TMP_InputField> shantoCashOutFields;

    public List<TMP_InputField> allInputFields;

    public Calculator calculator;
    string dateString;

    int newTmpHotelAmount;
    int newTmpShantoAmount;

    string hotelJsonToSubmit;
    string shantoJsonToSubmit;

    public GameObject successPanel, errorPanel, processingPanel, contactShantoPanel, alreadySubmittedPanel;

    [Space]
    public GameObject listPage;
    public TextMeshProUGUI dokanListText;
    public TextMeshProUGUI ShantoListText;
    public TextMeshProUGUI[] allCashTexts;

    public void OnStart()
    {
        expenseAddButton.onClick.AddListener(() => OnAddMoreFields(expensePurchaseContainerPrefab, expenseContainerParent, expenseFields));
        purchaseAddButton.onClick.AddListener(() => OnAddMoreFields(expensePurchaseContainerPrefab, purchaseContainerParent, purchaseFields));

        shantoCashInAddButton.onClick.AddListener(() => OnAddMoreFields(cigarShantoContainerPrefab, shantoCashInContainerParent, shantoCashInFields));
        shantoCashOutAddButton.onClick.AddListener(() => OnAddMoreFields(cigarShantoContainerPrefab, shantoCashOutContainerParent, shantoCashOutFields));

        allInputFields.Add(bakiRecievedField);
        allInputFields.Add(nicheField);
        allInputFields.Add(cashField);
        allInputFields.AddRange(expenseFields);
        allInputFields.AddRange(purchaseFields);
;
        allInputFields.AddRange(shantoCashInFields);
        allInputFields.AddRange(shantoCashOutFields);
    }
    public void OnEnable()
    {
        // Format the date as a string
        dateString = HotelMainController.instance.GetToday();

        UpdateTotalCashUI();
    }
    public void UpdateTotalCashUI()
    {
        hotelCashTillToday.text = PlayerPrefs.GetString(StringManager.HOTEL_TOTAL_CASH, "0");
        shantoCashTillToday.text = PlayerPrefs.GetString(StringManager.SHANTO_HOTEL_TOTAL_CASH, "0");

        StartCoroutine(HotelMainController.instance.GetRequest($"myInt={4}&myDate={HotelMainController.instance.GetToday()}", UpdateTotalCash, OnRequestError));
        StartCoroutine(HotelMainController.instance.GetRequest($"myInt={1}&myDate={HotelMainController.instance.GetToday()}", UpdateShabekCash, OnRequestError, false));
    }
    void UpdateTotalCash(string respose)
    {
        // Parse the JSON data
        JObject data = JObject.Parse(respose);

        JProperty property = data.Properties().First();

        JArray elements = (JArray)property.Value;

        // Define an array of keys to update
        string[] keysToUpdate = new string[] {
        StringManager.HOTEL_TOTAL_CASH,
        StringManager.DOKAN_TOTAL_CASH,
        StringManager.SHANTO_TOTAL_CASH,
        StringManager.SHANTO_HOTEL_TOTAL_CASH,
        null,
        StringManager.CIGAR_TOTAL_CASH,
        null,
        StringManager.OVREALL_TOTAL_CASH,
        StringManager.BAKI,
    };

        // Define a method to check if a value is valid
        bool IsValidValue(JToken value)
        {
            return value.Type != JTokenType.Null && value.ToString() != "" && value.ToString() != "#N/A";
        }

        // Update the PlayerPrefs with valid values only
        for (int i = 0; i < elements.Count; i++)
        {
            if (keysToUpdate[i] != null && IsValidValue(elements[i]))
                PlayerPrefs.SetString(keysToUpdate[i], elements[i].ToString());
        }

        hotelCashTillToday.text = PlayerPrefs.GetString(StringManager.HOTEL_TOTAL_CASH, "0");
        shantoCashTillToday.text = PlayerPrefs.GetString(StringManager.SHANTO_HOTEL_TOTAL_CASH, "0");
    }
    void UpdateShabekCash(string respose)
    {
        // Parse the JSON data
        JObject data = JObject.Parse(respose);

        JProperty property = data.Properties().First();

        // Define a method to check if a value is valid
        bool IsValidValue(JToken value)
        {
            return value.Type != JTokenType.Null && value.ToString() != "" && value.ToString() != "#N/A";
        }

        // Update the PlayerPrefs with valid values only
        if (property.Name == "Shabek" && IsValidValue(property.Value))
            PlayerPrefs.SetString(StringManager.HOTEL_SHABEK, property.Value.ToString());

        shabekText.text = PlayerPrefs.GetString(StringManager.HOTEL_SHABEK, "0");
    }
    private void Update()
    {
        bool isAnyFieldFocused = false;
        for (int i = 0; i < allInputFields.Count; i++)
        {
            if (allInputFields[i].isFocused)
            {
                isAnyFieldFocused = true;
                break;
            }

        }
        calculator.isCalculatorSelected = !isAnyFieldFocused;

        for (int i = 0; i < S_D_expenseTexts.Length; i++)
        {
            if (string.IsNullOrEmpty(expenseFields[i].text))
                expenseFields[i].text = "0";

            S_D_expenseTexts[i].text = expenseFields[i].text;
        }
    }
    public void OnAddMoreFields(GameObject prefab, Transform containerParent, List<TMP_InputField> listToAdd)
    {
        var tmp = Instantiate(prefab, containerParent);
        tmp.transform.SetSiblingIndex(0);
        var tmp2 = tmp.GetComponentsInChildren<TMP_InputField>();

        listToAdd.AddRange(tmp2);
        allInputFields.AddRange(listToAdd.Except(allInputFields));
    }
    public void GenerateJson()
    {
        GenerateHotelJson();
        GenerateShantoJson();
    }
    void GenerateHotelJson()
    {
        // Create a JObject to hold the final data
        var data = new JObject
        {
            { dateString, new JObject
                {
                    { "Baki", bakiRecievedField.text },
                    { "Niche", nicheField.text },
                    { "Cash", cashField.text },
                    { "Shabek", shabekText.text },
                    { "Expense", new JArray() },
                    { "Purchase", new JArray() }
                }
            }
        };

        // Populate expense
        ((JArray)data[dateString]["Expense"]).Add(new JObject
        {
            { "Shanto", new JObject
                {
                    { "FromCash", expenseFields[0].text },
                    { "FromFund", "" }
                }
            }
        });

        // Loop through each expense field and add the data to the JObject
        for (int i = 1; i < expenseFields.Count - 2; i += 3)
        {
            if (!string.IsNullOrEmpty(expenseFields[i].text) || !string.IsNullOrEmpty(expenseFields[i + 1].text) || !string.IsNullOrEmpty(expenseFields[i + 2].text))
            {
                ((JArray)data[dateString]["Expense"]).Add(new JObject
                {
                    { expenseFields[i].text, new JObject
                        {
                            { "FromCash", expenseFields[i + 1].text },
                            { "FromFund", expenseFields[i + 2].text }
                        }
                    }
                });
            }
        }

        // Populate purchase

        // Loop through each purchase field and add the data to the JObject
        for (int i = 0; i < purchaseFields.Count - 2; i += 3)
        {
            if (!string.IsNullOrEmpty(purchaseFields[i].text) || !string.IsNullOrEmpty(purchaseFields[i + 1].text) || !string.IsNullOrEmpty(purchaseFields[i + 2].text))
            {
                ((JArray)data[dateString]["Purchase"]).Add(new JObject
                {
                    { purchaseFields[i].text, new JObject
                        {
                            { "FromCash", purchaseFields[i + 1].text },
                            { "FromFund", purchaseFields[i + 2].text }
                        }
                    }
                });
            }
        }

        // Convert the data into a JSON string
        string json = data.ToString();

        ShowDailySellSummary(json);
    }

    void GenerateShantoJson()
    {
        // Create a JObject to hold the final data
        var data = new JObject
        {
            { dateString, new JObject
                {
                    { "Cash_in", new JArray() },
                    { "Cash_out", new JArray() }
                }
            }
        };

        // Populate Cash_in

        // Loop through each expense field and add the data to the JObject
        for (int i = 0; i < shantoCashInFields.Count - 1; i += 2)
        {
            if (!string.IsNullOrEmpty(shantoCashInFields[i].text) || !string.IsNullOrEmpty(shantoCashInFields[i + 1].text))
            {
                ((JArray)data[dateString]["Cash_in"]).Add(new JObject
                {
                    { shantoCashInFields[i].text, shantoCashInFields[i + 1].text }
                });
            }


        }

        // Populate Cash_out

        // Loop through each expense field and add the data to the JObject
        for (int i = 0; i < shantoCashOutFields.Count - 1; i += 2)
        {
            if (!string.IsNullOrEmpty(shantoCashOutFields[i].text) || !string.IsNullOrEmpty(shantoCashOutFields[i + 1].text))
            {
                ((JArray)data[dateString]["Cash_out"]).Add(new JObject
                {
                    { shantoCashOutFields[i].text, shantoCashOutFields[i + 1].text }
                });
            }
        }

        // Convert the data into a JSON string
        shantoJsonToSubmit = data.ToString();

        // Output the JSON string
        ShowDailyShantoSummary(shantoJsonToSubmit);
    }

    void ShowDailySellSummary(string jsonData)
    {
        //get total sell data:
        int sellAmount = 0;

        JObject data = JObject.Parse(jsonData);
        JObject dateData = (JObject)data[dateString];
        JArray expenses = (JArray)dateData["Expense"];
        JArray purchases = (JArray)dateData["Purchase"];
        string baki = (string)dateData["Baki"];
        string niche = (string)dateData["Niche"];
        string cash = (string)dateData["Cash"];
        string shabek = (string)dateData["Shabek"];

        string displayTextForDailySell = $"{dateString}:\n";
        string displayTextForHotelSell = $"{dateString}:    old:  {PlayerPrefs.GetString(StringManager.HOTEL_TOTAL_CASH, "0")}\n";

        displayTextForDailySell += "  Expense:\n";
        displayTextForHotelSell += "  Expense:\n";

        int expenseAmountFromCash = 0;
        int expenseAmountFromFund = 0;
        foreach (JObject expense in expenses)
        {
            foreach (KeyValuePair<string, JToken> entry in expense)
            {
                JObject expenseData = (JObject)entry.Value;

                string fromCash = (string)expenseData["FromCash"];
                string fromFund = (string)expenseData["FromFund"];

                if (!string.IsNullOrEmpty(fromCash) && !fromCash.Equals("0"))
                {
                    displayTextForDailySell += "    " + entry.Key + ":" + fromCash + "\n";
                    expenseAmountFromCash += int.Parse(fromCash);
                }
                if (!string.IsNullOrEmpty(fromFund) && !fromFund.Equals("0"))
                {
                    displayTextForHotelSell += "    " + entry.Key + ": (-)" + fromFund + "\n";
                    expenseAmountFromFund += int.Parse(fromFund);
                }
            }
        }

        displayTextForDailySell += "  Purchase:\n";
        displayTextForHotelSell += "  Purchase:\n";

        int purchaseAmountFromCash = 0;
        int purchaseAmountFromFund = 0;
        foreach (JObject purchase in purchases)
        {
            foreach (KeyValuePair<string, JToken> entry in purchase)
            {
                JObject purchaseData = (JObject)entry.Value;

                string fromCash = (string)purchaseData["FromCash"];
                string fromFund = (string)purchaseData["FromFund"];

                if (!string.IsNullOrEmpty(fromCash) && !fromCash.Equals("0"))
                {
                    displayTextForDailySell += "    " + entry.Key + ":" + fromCash + "\n";
                    purchaseAmountFromCash += int.Parse(fromCash);
                }
                if (!string.IsNullOrEmpty(fromFund) && !fromFund.Equals("0"))
                {
                    displayTextForHotelSell += "    " + entry.Key + ": (-)" + fromFund + "\n";
                    purchaseAmountFromFund += int.Parse(fromFund);
                }
            }
        }

        int bakiAmount = 0;
        if (!string.IsNullOrEmpty(baki))
        {
            bakiAmount += int.Parse(baki);
        }

        int nicheAmount = 0;
        if (!string.IsNullOrEmpty(niche))
        {
            nicheAmount += int.Parse(niche);
        }
        int cashAmount = 0;
        if (!string.IsNullOrEmpty(cash))
        {
            cashAmount += int.Parse(cash);
        }
        int shabekAmount = 0;
        if (!string.IsNullOrEmpty(shabek))
        {
            shabekAmount += int.Parse(shabek);
        }

        displayTextForDailySell += "<b><i>" + "  Niche:  " + nicheAmount + " </b></i > " + "\n";
        displayTextForDailySell += "<b><i>" + "  Closing Cash:  " + cashAmount + " </b></i > " + "\n";
        displayTextForDailySell += "<b><i>" + "  Opening Cash:  (-)" + shabekAmount + " </b></i > " + "\n";

        displayTextForHotelSell += "<b><i>" + "  Niche:  " + nicheAmount + " </b></i > " + "\n";
        displayTextForHotelSell += "<b><i>" + "  Baki:  " + bakiAmount + " </b></i > " + "\n";

        // Add remaining cash to JSON object
        dateData.Add("Date", dateString);

        // Convert dateData to JSON string
        hotelJsonToSubmit = dateData.ToString();
        //print(hotelJsonToSubmit);
        displayTextForDailySell += "-------------------------\n";
        
        sellAmount = (expenseAmountFromCash + purchaseAmountFromCash + nicheAmount - shabekAmount);
        displayTextForDailySell += "SELL = " + sellAmount;

        dailySellSummaryText.text = displayTextForDailySell;

        ShowDailyHotelSummary(displayTextForHotelSell, expenseAmountFromFund, purchaseAmountFromFund, nicheAmount, bakiAmount);

    }
    void ShowDailyHotelSummary(string data, int expenseAmount, int purchaseAmount, int nicheAmount, int bakiAmount)
    {
        int prevAmount = int.Parse(PlayerPrefs.GetString(StringManager.HOTEL_TOTAL_CASH, "0"));
        newTmpHotelAmount = prevAmount - (expenseAmount + purchaseAmount) + nicheAmount + bakiAmount;

        data += "-------------------------\n";
        data += $"Total =   {newTmpHotelAmount}";
        dailyHotelSummaryText.text = data;

    }
    void ShowDailyShantoSummary(string jsonData)
    {
        JObject data = JObject.Parse(jsonData);
        JObject dateData = (JObject)data[dateString];

        JArray cashIn = (JArray)dateData["Cash_in"];
        JArray cashOut = (JArray)dateData["Cash_out"];

        string displayText = $"{dateString}:    old:  {PlayerPrefs.GetString(StringManager.SHANTO_HOTEL_TOTAL_CASH, "0")}\n";
        displayText += "  Cash In:\n";
        int cashInAmount = 0;
        if (!string.IsNullOrEmpty(S_D_expenseTexts[0].text) && !S_D_expenseTexts[0].text.Equals("0"))
        {
            displayText += $"    Shanto  (+){S_D_expenseTexts[0].text}\n";
            cashInAmount += int.Parse(S_D_expenseTexts[0].text);
        }

        foreach (JObject cashInTransaction in cashIn)
        {
            foreach (var property in cashInTransaction.Properties())
            {
                if (!string.IsNullOrEmpty(property.Value.ToString()) && !property.Value.ToString().Equals("0"))
                {
                    displayText += $"    {property.Name}  (+){property.Value}\n";
                    cashInAmount += int.Parse(property.Value.ToString());
                }
            }
        }

        displayText += "  Cash Out:\n";
        int cashOutAmount = 0;
        foreach (JObject cashOutTransaction in cashOut)
        {
            foreach (var property in cashOutTransaction.Properties())
            {
                if (!string.IsNullOrEmpty(property.Value.ToString()) && !property.Value.ToString().Equals("0"))
                {
                    displayText += $"    {property.Name}  (-){property.Value}\n";
                    cashOutAmount += int.Parse(property.Value.ToString());
                }
            }
        }

        displayText += "-------------------------\n";

        int prevAmount = int.Parse(PlayerPrefs.GetString(StringManager.SHANTO_HOTEL_TOTAL_CASH, "0"));
        newTmpShantoAmount = prevAmount + cashInAmount - cashOutAmount;

        displayText += "Total:  " + (newTmpShantoAmount).ToString();

        dailyShantoSummaryText.text = displayText;
    }

    public void SubmitDatas()
    {
        if (PlayerPrefs.GetString(StringManager.ALREADY_SUBMITTED_TODAY, "0").Equals(dateString))
        {
            alreadySubmittedPanel.SetActive(true);
        }
        else
        {
            processingPanel.SetActive(true);

            StartCoroutine(HotelMainController.instance.PostRequest(hotelJsonToSubmit, 6, OnSuccessfulHotelSubmit, OnRequestError));
        }

    }
    void OnSuccessfulHotelSubmit(string msg)
    {
        if (msg.Equals("\"Done\""))
        {
            PlayerPrefs.SetString(StringManager.HOTEL_TOTAL_CASH, $"{newTmpHotelAmount}");

            StartCoroutine(HotelMainController.instance.PostRequest(shantoJsonToSubmit, 7, OnSuccessfulShantoSubmit, OnRequestError));
        }
        else
        {
            contactShantoPanel.SetActive(true);
            processingPanel.SetActive(false);
        }
    }
    void OnSuccessfulShantoSubmit(string msg)
    {
        if (msg.Equals("\"Done\""))
        {
            PlayerPrefs.SetString(StringManager.SHANTO_HOTEL_TOTAL_CASH, $"{newTmpShantoAmount}");
            PlayerPrefs.SetString(StringManager.ALREADY_SUBMITTED_TODAY, $"{dateString}");

            processingPanel.SetActive(false);
            successPanel.SetActive(true);
        }
        else
        {
            contactShantoPanel.SetActive(true);
            processingPanel.SetActive(false);
        }

    }
    void OnRequestError(string msg)
    {
        processingPanel.SetActive(false);
        errorPanel.SetActive(true);
    }
}
