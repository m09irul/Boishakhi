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

public class ManagementController : MonoBehaviour
{
    public TextMeshProUGUI[] S_D_expenseTexts;
    public TextMeshProUGUI cigarCashInText;

    public TextMeshProUGUI dokanCashTillToday;
    public TextMeshProUGUI cigarCashTillToday;
    public TextMeshProUGUI shantoCashTillToday;
    [Space]
    public TextMeshProUGUI dailySellSummaryText;
    public TextMeshProUGUI dailyDokanSummaryText;
    public TextMeshProUGUI dailyCigarSummaryText;
    public TextMeshProUGUI dailyShantoSummaryText;

    public GameObject expensePurchaseContainerPrefab;
    public GameObject cigarShantoContainerPrefab;

    public Button expenseAddButton;
    public Button purchaseAddButton;
    public Button cigarCashInAddButton;
    public Button cigarCashOutAddButton;
    public Button shantoCashInAddButton;
    public Button shantoCashOutAddButton;

    [Space]
    public Transform expenseContainerParent;
    public Transform purchaseContainerParent;
    public Transform cigarCashInContainerParent;
    public Transform cigarCashOutContainerParent;
    public Transform shantoCashInContainerParent;
    public Transform shantoCashOutContainerParent;

    public TMP_InputField bakiField;
    
    public List<TMP_InputField> expenseFields;
    public List<TMP_InputField> purchaseFields;
    public List<TMP_InputField> cigarCashInFields;
    public List<TMP_InputField> cigarCashOutFields;
    public List<TMP_InputField> shantoCashInFields;
    public List<TMP_InputField> shantoCashOutFields;

    public List<TMP_InputField> allInputFields;

    public Calculator calculator;
    string dateString;

    int newTmpDokanAmount;
    int newTmpCigarAmount;
    int newTmpShantoAmount;

    string dokanJsonToSubmit;
    string cigarJsonToSubmit;
    string shantoJsonToSubmit;

    public GameObject successPanel, errorPanel, processingPanel, contactShantoPanel, alreadySubmittedPanel;

    [Space]
    public GameObject listPage;
    public TextMeshProUGUI dokanListText;
    public TextMeshProUGUI cigarListText;
    public TextMeshProUGUI ShantoListText;
    public TextMeshProUGUI[] allCashTexts;

    public void OnStart()
    {
        expenseAddButton.onClick.AddListener(()=>OnAddMoreFields(expensePurchaseContainerPrefab, expenseContainerParent, expenseFields));
        purchaseAddButton.onClick.AddListener(()=>OnAddMoreFields(expensePurchaseContainerPrefab, purchaseContainerParent, purchaseFields));

        cigarCashInAddButton.onClick.AddListener(()=>OnAddMoreFields(cigarShantoContainerPrefab, cigarCashInContainerParent, cigarCashInFields));
        cigarCashOutAddButton.onClick.AddListener(()=>OnAddMoreFields(cigarShantoContainerPrefab, cigarCashOutContainerParent, cigarCashOutFields));
        shantoCashInAddButton.onClick.AddListener(()=>OnAddMoreFields(cigarShantoContainerPrefab, shantoCashInContainerParent, shantoCashInFields));
        shantoCashOutAddButton.onClick.AddListener(()=>OnAddMoreFields(cigarShantoContainerPrefab, shantoCashOutContainerParent, shantoCashOutFields));

        allInputFields.Add(bakiField);
        allInputFields.AddRange(expenseFields);
        allInputFields.AddRange(purchaseFields);
        allInputFields.AddRange(cigarCashInFields);
        allInputFields.AddRange(cigarCashOutFields);
        allInputFields.AddRange(shantoCashInFields);
        allInputFields.AddRange(shantoCashOutFields);

        
    }
    public void OnEnable()
    {
        // Format the date as a string
        dateString = MainController.instance.GetToday();
        
        UpdateTotalCashUI();
        
        UpdateCigarSellUI(GetTotalCigarSell());
    }
    string GetTotalCigarSell()
    {
        //get total sell data:
        JObject cigarSellData = JObject.Parse(PlayerPrefs.GetString(StringManager.CIGAR_SELL_MAIN, "{}"));
        if (cigarSellData[dateString] != null)
            return cigarSellData[dateString]["Total Sell"] == null ? "0" : (string)cigarSellData[dateString]["Total Sell"];
        else
            return "0";

    }
    public void UpdateTotalCashUI()
    {
        dokanCashTillToday.text = PlayerPrefs.GetString(StringManager.DOKAN_TOTAL_CASH, "0");
        cigarCashTillToday.text = PlayerPrefs.GetString(StringManager.CIGAR_TOTAL_CASH, "0");
        shantoCashTillToday.text = PlayerPrefs.GetString(StringManager.SHANTO_TOTAL_CASH, "0");

        StartCoroutine(MainController.instance.GetRequest($"myInt={4}&myDate={MainController.instance.GetToday()}", UpdateTotalCash, OnRequestError));
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
        StringManager.OVREALL_TOTAL_CASH
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

        dokanCashTillToday.text = PlayerPrefs.GetString(StringManager.DOKAN_TOTAL_CASH, "0");
        cigarCashTillToday.text = PlayerPrefs.GetString(StringManager.CIGAR_TOTAL_CASH, "0");
        shantoCashTillToday.text = PlayerPrefs.GetString(StringManager.SHANTO_TOTAL_CASH, "0");
    }
    public void OnListButtonClicked()
    {
        processingPanel.SetActive(true);
        StartCoroutine(MainController.instance.GetRequest($"myInt={1}&myDate={MainController.instance.GetToday()}", UpdateDokanCash, OnRequestError));
        StartCoroutine(MainController.instance.GetRequest($"myInt={2}&myDate={MainController.instance.GetToday()}", UpdateCigarCash, OnRequestError));
        StartCoroutine(MainController.instance.GetRequest($"myInt={3}&myDate={MainController.instance.GetToday()}", UpdateShantoCash, OnRequestError));
    }
    void UpdateDokanCash(string response)
    {
        
        if (processingPanel.activeInHierarchy)
            processingPanel.SetActive(false);
        // Parse the JSON data
        JObject jo = JObject.Parse(response);

        // Access the 'Purchase' property
        JArray purchase = (JArray)jo["Purchase"];
        JArray expense = (JArray)jo["Expense"];

        // Create a StringBuilder to store the formatted string
        StringBuilder sb = new StringBuilder();

        // Access the 'PrevAmount' property
        JArray prevAmount = (JArray)jo["PrevAmount"];

        // Get the previous amount value from the 6th column of the 'PrevAmount' array
        int previousAmount = (int)prevAmount[1];

        // Append the header to the StringBuilder
        sb.AppendLine("Previous Amount: " + previousAmount);

        // Create a dictionary to store the data grouped by date
        Dictionary<string, List<string>> dataByDate = new Dictionary<string, List<string>>();

        // Iterate over the elements of the 'Purchase' array
        foreach (JArray element in purchase)
        {
            // Access the values of the element
            string date = (string)element[0];
            string item = (string)element[1];
            string value = (string)element[2];

            /*// Extract only the date value from the date string
            date = date.Substring(0, 10);

            // Reformat the date in day-month-year format
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
                date = dateTime.ToString("yyyy-MM-dd");*/

            // Append the values to the StringBuilder
            if (!string.IsNullOrEmpty(value) && value != "0")
            {
                int value2Int = int.Parse(value);
                string line = $"{date}   {item}   (-){value}   {previousAmount - value2Int}";
                previousAmount += value2Int;

                // Add the line to the dictionary, grouped by date
                if (!dataByDate.ContainsKey(date))
                    dataByDate[date] = new List<string>();
                dataByDate[date].Add(line);
            }
        }

        // Iterate over the elements of the 'Expense' array
        foreach (JArray element in expense)
        {
            // Access the values of the element
            string date = (string)element[0];
            string item = (string)element[1];
            string value = (string)element[2];

            /*// Extract only the date value from the date string
            date = date.Substring(0, 10);

            // Reformat the date in day-month-year format
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
                date = dateTime.ToString("yyyy-MM-dd");*/

            // Append the values to the StringBuilder
            if (!string.IsNullOrEmpty(value) && value != "0" && !item.Equals("Shanto") && !item.Equals("Rent") && !item.Equals("Dokan"))
            {
                int value2Int = int.Parse(value);
                string line = $"{date}   {item}   (-){value}   {previousAmount - value2Int}";
                previousAmount += value2Int;

                // Add the line to the dictionary, grouped by date
                if (!dataByDate.ContainsKey(date))
                    dataByDate[date] = new List<string>();
                dataByDate[date].Add(line);
            }
        }

        // Iterate over the dates in ascending order and append all lines for each date to the StringBuilder
        foreach (var kvp in dataByDate.OrderBy(kvp => DateTime.Parse(kvp.Key)))
        {
            if (kvp.Value.Count > 0)
            {
                foreach (string line in kvp.Value)
                    sb.AppendLine(line);
            }
        }

        sb.AppendLine("-----------------------");
        // Get the formatted string from the StringBuilder
        string result = sb.ToString();

        // Print the result
        dokanListText.text = result;
    }

    void UpdateCigarCash(string response)
    {
        if (processingPanel.activeInHierarchy)
            processingPanel.SetActive(false);
        // Parse the JSON data
        JObject jo = JObject.Parse(response);

        // Access the 'Cigar' property
        JArray cigar = (JArray)jo["Cigar"];

        // Access the 'PrevAmount' property
        JArray prevAmount = (JArray)jo["PrevAmount"];

        // Get the previous amount value from the 6th column of the 'PrevAmount' array
        int previousAmount = (int)prevAmount[5];

        // Create a StringBuilder to store the formatted string
        StringBuilder sb = new StringBuilder();

        // Append the header to the StringBuilder
        sb.AppendLine("Previous Amount: " + previousAmount);

        // Create a dictionary to store the data grouped by date
        Dictionary<string, List<string>> dataByDate = new Dictionary<string, List<string>>();

        // Iterate over the elements of the 'Cigar' array
        for (int i = 0; i < cigar.Count; i++)
        {
            // Access the values of the element
            JArray element = (JArray)cigar[i];
            string date = (string)element[0];
            string cashInVal = (string)element[1];
            string cashInNote = (string)element[2];
            string cashOutVal = (string)element[3];
            string cashOutNote = (string)element[4];

            /*// Extract only the date value from the date string
            date = date.Substring(0, 10);

            // Reformat the date in day-month-year format
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
                date = dateTime.ToString("yyyy-MM-dd");*/

            // Append the values to the StringBuilder
            if (!string.IsNullOrEmpty(cashInVal) && cashInVal != "0")
            {
                int value2Int = int.Parse(cashInVal);
                string line = $"{date}   {cashInNote}   (+){value2Int}   {previousAmount + value2Int}";
                previousAmount += value2Int;

                // Add the line to the dictionary, grouped by date
                if (!dataByDate.ContainsKey(date))
                    dataByDate[date] = new List<string>();
                dataByDate[date].Add(line);
            }
            if (!string.IsNullOrEmpty(cashOutVal) && cashOutVal != "0")
            {
                int amount4Int = int.Parse(cashOutVal);
                string line = $"{date}   {cashOutNote}   (-){amount4Int}   {previousAmount - amount4Int}";
                previousAmount -= amount4Int;

                // Add the line to the dictionary, grouped by date
                if (!dataByDate.ContainsKey(date))
                    dataByDate[date] = new List<string>();
                dataByDate[date].Add(line);
            }
        }

        // Iterate over all lines for each date and append them to the StringBuilder without appending dates as headers.
        foreach (var kvp in dataByDate.OrderBy(kvp => DateTime.Parse(kvp.Key)))
        {
            if (kvp.Value.Count > 0)
            {
                foreach (string line in kvp.Value)
                    sb.AppendLine(line);
            }
        }

        sb.AppendLine("-----------------------");
        // Get the formatted string from the StringBuilder
        string result = sb.ToString();

        // Print the result
        cigarListText.text = result;
    }

    void UpdateShantoCash(string response)
    {
        if (processingPanel.activeInHierarchy)
            processingPanel.SetActive(false);
        // Parse the JSON data
        JObject jo = JObject.Parse(response);

        // Access the 'Cigar' property
        JArray cigar = (JArray)jo["Shanto"];

        // Access the 'PrevAmount' property
        JArray prevAmount = (JArray)jo["PrevAmount"];

        // Get the previous amount value from the 6th column of the 'PrevAmount' array
        int previousAmount = (int)prevAmount[2];

        // Create a StringBuilder to store the formatted string
        StringBuilder sb = new StringBuilder();

        // Append the header to the StringBuilder
        sb.AppendLine("Previous Amount: " + previousAmount);

        // Create a dictionary to store the data grouped by date
        Dictionary<string, List<string>> dataByDate = new Dictionary<string, List<string>>();

        // Iterate over the elements of the 'Cigar' array
        for (int i = 0; i < cigar.Count; i++)
        {
            // Access the values of the element
            JArray element = (JArray)cigar[i];
            string date = (string)element[0];
            string cashInVal = (string)element[1];
            string cashInNote = (string)element[2];
            string cashOutVal = (string)element[3];
            string cashOutNote = (string)element[4];

            /*// Extract only the date value from the date string
            date = date.Substring(0, 10);

            // Reformat the date in day-month-year format
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
                date = dateTime.ToString("yyyy-MM-dd");*/

            // Append the values to the StringBuilder
            if (!string.IsNullOrEmpty(cashInVal) && cashInVal != "0")
            {
                int value2Int = int.Parse(cashInVal);
                string line = $"{date}   {cashInNote}   (+){value2Int}   {previousAmount + value2Int}";
                previousAmount += value2Int;

                // Add the line to the dictionary, grouped by date
                if (!dataByDate.ContainsKey(date))
                    dataByDate[date] = new List<string>();
                dataByDate[date].Add(line);
            }
            if (!string.IsNullOrEmpty(cashOutVal) && cashOutVal != "0")
            {
                int amount4Int = int.Parse(cashOutVal);
                string line = $"{date}   {cashOutNote}   (-){amount4Int}   {previousAmount - amount4Int}";
                previousAmount -= amount4Int;

                // Add the line to the dictionary, grouped by date
                if (!dataByDate.ContainsKey(date))
                    dataByDate[date] = new List<string>();
                dataByDate[date].Add(line);
            }
        }

        // Iterate over all lines for each date and append them to the StringBuilder without appending dates as headers.
        foreach (var kvp in dataByDate.OrderBy(kvp => DateTime.Parse(kvp.Key)))
        {
            if (kvp.Value.Count > 0)
            {
                foreach (string line in kvp.Value)
                    sb.AppendLine(line);
            }
        }

        sb.AppendLine("-----------------------");
        // Get the formatted string from the StringBuilder
        string result = sb.ToString();

        // Print the result
        ShantoListText.text = result;
    }


    void UpdateCigarSellUI(string val)
    {
        cigarCashInText.text = val;
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
        var tmp2 = tmp.GetComponentsInChildren<TMP_InputField>();

        listToAdd.AddRange(tmp2);
        allInputFields.AddRange(listToAdd.Except(allInputFields));
    }
    public void GenerateJson()
    {
        GenerateDokanJson();
        GenerateCigarJson();
        GenerateShantoJson();
    }
    void GenerateDokanJson()
    {
        // Create a JObject to hold the final data
        var data = new JObject
        {
            { dateString, new JObject
                {
                    { "Baki", bakiField.text },
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
        ((JArray)data[dateString]["Expense"]).Add(new JObject
        {
            { "Dokan", new JObject
                {
                    { "FromCash", expenseFields[1].text },
                    { "FromFund", "" }
                }
            }
        });

        // Loop through each expense field and add the data to the JObject
        for (int i = 2; i < expenseFields.Count - 2; i += 3)
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

    void GenerateCigarJson()
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
        ((JArray)data[dateString]["Cash_in"]).Add(new JObject
        {
            { "Sell", cigarCashInText.text }
        });

        // Loop through each expense field and add the data to the JObject
        for (int i = 0; i < cigarCashInFields.Count - 1; i += 2)
        {
            if (!string.IsNullOrEmpty(cigarCashInFields[i].text) || !string.IsNullOrEmpty(cigarCashInFields[i + 1].text))
            {
                ((JArray)data[dateString]["Cash_in"]).Add(new JObject
                {
                    { cigarCashInFields[i].text, cigarCashInFields[i + 1].text }
                });
            }
        }

        // Populate Cash_out
        ((JArray)data[dateString]["Cash_out"]).Add(new JObject
        {
            { "Buy", cigarCashOutFields[0].text }
        });

        // Loop through each expense field and add the data to the JObject
        for (int i = 1; i < cigarCashOutFields.Count - 1; i += 2)
        {
            if (!string.IsNullOrEmpty(cigarCashOutFields[i].text) || !string.IsNullOrEmpty(cigarCashOutFields[i + 1].text))
            {
                ((JArray)data[dateString]["Cash_out"]).Add(new JObject
                {
                    { cigarCashOutFields[i].text, cigarCashOutFields[i + 1].text }
                });
            }
        }

        // Convert the data into a JSON string
        cigarJsonToSubmit = data.ToString();

        // Output the JSON string
        ShowDailyCigarSummary(cigarJsonToSubmit);
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
        JObject dokanSellData = JObject.Parse(PlayerPrefs.GetString(StringManager.DOKAN_SELL_MAIN, "{}"));
     
        int sellAmountExceptHotel = 0;
        int sellToHotel = 0;
        foreach (var item in dokanSellData[dateString]["Data"])
        {
            if (int.TryParse((string)item["price"], out int tmpPrice))
            {
                if ((string)item["sellTo"] == "Hotel")
                {
                    sellToHotel += tmpPrice;
                }
                else
                {
                    sellAmountExceptHotel += tmpPrice;
                }
            }
        }

        JObject data = JObject.Parse(jsonData);
        JObject dateData = (JObject)data[dateString];
        JArray expenses = (JArray)dateData["Expense"];
        JArray purchases = (JArray)dateData["Purchase"];
        string baki = (string)dateData["Baki"];

        string displayTextForDailySell = $"{dateString}:\n";
        string displayTextForDokanSell = $"{dateString}:    old:  {PlayerPrefs.GetString(StringManager.DOKAN_TOTAL_CASH, "0")}\n";

        displayTextForDailySell += "  Expense:\n";
        displayTextForDokanSell += "  Expense:\n";

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
                    displayTextForDokanSell += "    " + entry.Key + ": (-)" + fromFund + "\n";
                    expenseAmountFromFund += int.Parse(fromFund);
                }
            }
        }

        displayTextForDailySell += "  Purchase:\n";
        displayTextForDokanSell += "  Purchase:\n";

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
                    displayTextForDokanSell += "    " + entry.Key + ": (-)" + fromFund + "\n";
                    purchaseAmountFromFund += int.Parse(fromFund);
                }
            }
        }

        int bakiAmount = 0;
        if (!string.IsNullOrEmpty(baki))
        {
            displayTextForDailySell += "  Baki:" + baki + "\n";
            bakiAmount += int.Parse(baki);
        }

        int remainingCash = (sellToHotel + sellAmountExceptHotel + bakiAmount) - (expenseAmountFromCash + purchaseAmountFromCash);
        displayTextForDailySell += "<b><i>"+"  Cash:  " + remainingCash + " </b></i > " + "\n";
        displayTextForDokanSell += "<b><i>"+"  Cash In:  " + remainingCash + " </b></i > " + "\n";

        // Add remaining cash to JSON object
        dateData.Add("RemainingCash", remainingCash);

        // Add remaining cash to JSON object
        dateData.Add("Date", dateString);
        dateData.Add("SellToHotel", sellToHotel);
        // Convert dateData to JSON string
        dokanJsonToSubmit = dateData.ToString();

        displayTextForDailySell += "-------------------------\n";
        displayTextForDailySell += "SELL = " + (expenseAmountFromCash + purchaseAmountFromCash + remainingCash) + $"    Hotel({sellToHotel})";

        dailySellSummaryText.text = displayTextForDailySell;

        ShowDailyDokanSummary(displayTextForDokanSell, expenseAmountFromFund, purchaseAmountFromFund, remainingCash);

    }
    void ShowDailyDokanSummary(string data, int expenseAmount, int purchaseAmount, int remainingCash)
    {
        int prevAmount = int.Parse(PlayerPrefs.GetString(StringManager.DOKAN_TOTAL_CASH, "0"));

        newTmpDokanAmount = prevAmount - (expenseAmount + purchaseAmount) + remainingCash;

        data += "-------------------------\n";
        data += $"Total =   {newTmpDokanAmount}";
        dailyDokanSummaryText.text = data;

    }
    void ShowDailyCigarSummary(string jsonData)
    {
        JObject data = JObject.Parse(jsonData);
        JObject dateData = (JObject)data[dateString];

        JArray cashIn = (JArray)dateData["Cash_in"];
        JArray cashOut = (JArray)dateData["Cash_out"];

        string displayText = $"{dateString}:    old:  {PlayerPrefs.GetString(StringManager.CIGAR_TOTAL_CASH,"0")}\n";
        displayText += "  Cash In:\n";
        int cashInAmount = 0;
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
                    displayText += $"    {property.Name} (-){property.Value}\n";
                    cashOutAmount += int.Parse(property.Value.ToString());
                }    
            }
        }  

        displayText += "-------------------------\n";

        int prevAmount = int.Parse(PlayerPrefs.GetString(StringManager.CIGAR_TOTAL_CASH, "0"));
        newTmpCigarAmount = prevAmount + cashInAmount - cashOutAmount;

        displayText += "Total:  " + (newTmpCigarAmount).ToString();

        dailyCigarSummaryText.text = displayText;

    }
    void ShowDailyShantoSummary(string jsonData)
    {
        JObject data = JObject.Parse(jsonData);
        JObject dateData = (JObject)data[dateString];

        JArray cashIn = (JArray)dateData["Cash_in"];
        JArray cashOut = (JArray)dateData["Cash_out"];

        string displayText = $"{dateString}:    old:  {PlayerPrefs.GetString(StringManager.SHANTO_TOTAL_CASH,"0")}\n";
        displayText += "  Cash In:\n";
        int cashInAmount = 0;
        if (!string.IsNullOrEmpty(S_D_expenseTexts[0].text) && !S_D_expenseTexts[0].text.Equals("0"))
        {
            displayText += $"    Shanto  (+){S_D_expenseTexts[0].text}\n";
            cashInAmount += int.Parse(S_D_expenseTexts[0].text);
        }
        if (!string.IsNullOrEmpty(S_D_expenseTexts[1].text) && !S_D_expenseTexts[1].text.Equals("0"))
        {
            displayText += $"    Dokan  (+){S_D_expenseTexts[1].text}\n";
            cashInAmount += int.Parse(S_D_expenseTexts[1].text);
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

        int prevAmount = int.Parse(PlayerPrefs.GetString(StringManager.SHANTO_TOTAL_CASH, "0"));
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

            StartCoroutine(MainController.instance.PostRequest(dokanJsonToSubmit, 3, OnSuccessfulDokanSubmit, OnRequestError));
        }
        
    }
    void OnSuccessfulDokanSubmit(string msg)
    {
        if (msg.Equals("\"Done\""))
        {
            PlayerPrefs.SetString(StringManager.DOKAN_TOTAL_CASH, $"{newTmpDokanAmount}");

            StartCoroutine(MainController.instance.PostRequest(cigarJsonToSubmit, 4, OnSuccessfulCigarSubmit, OnRequestError));
        }
        else
        {
            contactShantoPanel.SetActive(true);
            processingPanel.SetActive(false);
        }
    }
    void OnSuccessfulCigarSubmit(string msg)
    {
        if (msg.Equals("\"Done\""))
        {
            PlayerPrefs.SetString(StringManager.CIGAR_TOTAL_CASH, $"{newTmpCigarAmount}");
            
            StartCoroutine(MainController.instance.PostRequest(shantoJsonToSubmit, 5, OnSuccessfulShantoSubmit, OnRequestError));
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
            PlayerPrefs.SetString(StringManager.SHANTO_TOTAL_CASH, $"{newTmpShantoAmount}");
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
