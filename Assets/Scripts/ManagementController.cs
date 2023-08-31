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
    public void OnStart()
    {
        // Format the date as a string
        dateString = MainController.instance.GetToday();

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

        UpdateTotalCashUI();
        
        UpdateCigarSellUI(GetTotalCigarSell());
    }

    string GetTotalCigarSell()
    {
        //get total sell data:
        JObject cigarSellData = JObject.Parse(PlayerPrefs.GetString(StringManager.CIGAR_SELL_MAIN, "{}"));
        print(cigarSellData);
        return cigarSellData[dateString]["Total Sell"] == null ? "0" : (string)cigarSellData[dateString]["Total Sell"];

    }
    public void UpdateTotalCashUI()
    {
        dokanCashTillToday.text = PlayerPrefs.GetString(StringManager.DOKAN_TOTAL_CASH, "0");
        cigarCashTillToday.text = PlayerPrefs.GetString(StringManager.CIGAR_TOTAL_CASH, "0");
        shantoCashTillToday.text = PlayerPrefs.GetString(StringManager.SHANTO_TOTAL_CASH, "0");

        StartCoroutine(MainController.instance.GetRequest($"myInt={4}&myDate={MainController.instance.GetToday()}", UpdateTotalCash));
    }
    void UpdateTotalCash(string respose)
    {
        // Parse the JSON data
        JObject data = JObject.Parse(respose);

        JProperty property = data.Properties().First();

        JArray elements = (JArray)property.Value;

        // Define an array of keys to update
        string[] keysToUpdate = new string[] {
        StringManager.DOKAN_TOTAL_CASH,
        StringManager.SHANTO_TOTAL_CASH,
        StringManager.SHANTO_HOTEL_TOTAL_CASH,
        StringManager.CIGAR_TOTAL_CASH,
        StringManager.OVREALL_TOTAL_CASH
        };

        PlayerPrefs.SetString(StringManager.HOTEL_TOTAL_CASH, elements[0].ToString());
        PlayerPrefs.SetString(StringManager.DOKAN_TOTAL_CASH, elements[1].ToString());
        PlayerPrefs.SetString(StringManager.SHANTO_TOTAL_CASH, elements[2].ToString());
        PlayerPrefs.SetString(StringManager.SHANTO_HOTEL_TOTAL_CASH, elements[3].ToString());
        PlayerPrefs.SetString(StringManager.CIGAR_TOTAL_CASH, elements[5].ToString());
        PlayerPrefs.SetString(StringManager.OVREALL_TOTAL_CASH, elements[7].ToString());

        dokanCashTillToday.text = PlayerPrefs.GetString(StringManager.DOKAN_TOTAL_CASH, "0");
        cigarCashTillToday.text = PlayerPrefs.GetString(StringManager.CIGAR_TOTAL_CASH, "0");
        shantoCashTillToday.text = PlayerPrefs.GetString(StringManager.SHANTO_TOTAL_CASH, "0");
    }
    void UpdateDokanCash(string respose)
    {
        // Parse the JSON data
        JObject jo = JObject.Parse(respose);

        // Access the 'Purchase' property
        JArray purchase = (JArray)jo["Purchase"];

        // Create a StringBuilder to store the formatted string
        StringBuilder sb = new StringBuilder();

        // Append the header to the StringBuilder
        sb.AppendLine("Purchase:");
        sb.AppendLine("Date\tItem\tValue");

        // Iterate over the elements of the 'Purchase' array
        foreach (JArray element in purchase)
        {
            // Access the values of the element
            string date = (string)element[0];
            string item = (string)element[1];
            string value = (string)element[2];

            // Extract only the date value from the date string
            date = date.Substring(0, 10);
            // Split the date into its components
            string[] dateParts = date.Split('/');

            // Reformat the date in day-month-year format
            date = $"{dateParts[2]}-{dateParts[1]}-{dateParts[0]}";

            // Append the values to the StringBuilder
            sb.AppendLine($"{date}\t{item}\t{value}");
        }

        // Get the formatted string from the StringBuilder
        string result = sb.ToString();

        // Print the result
        print(result);
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

        // Output the JSON string
        print(json);

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
            { "SellCigar", cigarCashInText.text }
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
            { "BuyCigar", cigarCashOutFields[0].text }
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
        string json = data.ToString();

        // Output the JSON string
        print(json);
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
        string json = data.ToString();

        // Output the JSON string
        print(json);
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

        string displayText = $"{dateString}:\n";
        displayText += "  Expense:\n";
        int expenseAmount = 0;
        foreach (JObject expense in expenses)
        {
            foreach (KeyValuePair<string, JToken> entry in expense)
            {
                JObject expenseData = (JObject)entry.Value;
                string fromCash = (string)expenseData["FromCash"];
                if (!string.IsNullOrEmpty(fromCash))
                {
                    displayText += "    " + entry.Key + ":" + fromCash + "\n";
                    expenseAmount += int.Parse(fromCash);
                }
            }
        }

        displayText += "  Purchase:\n";
        int purchaseAmount = 0;
        foreach (JObject purchase in purchases)
        {
            foreach (KeyValuePair<string, JToken> entry in purchase)
            {
                JObject purchaseData = (JObject)entry.Value;
                string fromCash = (string)purchaseData["FromCash"];
                if (!string.IsNullOrEmpty(fromCash))
                {
                    displayText += "    " + entry.Key + ":" + fromCash + "\n";
                    purchaseAmount += int.Parse(fromCash);
                }
            }
        }

        int bakiAmount = 0;
        if (!string.IsNullOrEmpty(baki))
        {
            displayText += "  Baki:" + baki + "\n";
            bakiAmount += int.Parse(baki);
        }

        int remainingCash = (sellToHotel + sellAmountExceptHotel + bakiAmount) - (expenseAmount + purchaseAmount);
        displayText += "<b><i>"+"  Cash:" + remainingCash + " </b></i > " + "\n";

             // Add remaining cash to JSON object
             dateData.Add("RemainingCash", remainingCash);
        // Add remaining cash to JSON object
        dateData.Add("Date", dateString);
        dateData.Add("SellToHotel", sellToHotel);
        // Convert dateData to JSON string
        string dateDataJsonString = dateData.ToString();

        displayText += "-------------------------\n";
        displayText += "SELL = " + (expenseAmount + purchaseAmount + remainingCash) + $"    Hotel({sellToHotel})";

        Debug.LogError("Add an Erorr popup if accessing without any sell");
        dailySellSummaryText.text = displayText;

    }
}
