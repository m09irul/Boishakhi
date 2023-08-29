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

public class ManagementController : MonoBehaviour
{
    public MainController test;

    public TextMeshProUGUI[] S_D_expenseTexts;
    public TextMeshProUGUI cigarCashInText;

    public TextMeshProUGUI dokanCashTillToday;
    public TextMeshProUGUI cigarCashTillToday;
    public TextMeshProUGUI shantoCashTillToday;

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
    private void Start()
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

        UpdateCigarSellUI(GetTotalCigarSell());
    }

    string GetTotalCigarSell()
    {
        //get total sell data:
        JObject cigarSellData = JObject.Parse(PlayerPrefs.GetString("CigarSell", "{}"));
        print(cigarSellData);
        return cigarSellData[dateString]["Total Sell"] == null ? "0" : (string)cigarSellData[dateString]["Total Sell"];

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
        /*string myDokanJson = PlayerPrefs.GetString("MyDokan", "{}");
        string myCigarJson = PlayerPrefs.GetString("MyCigar", "{}");
        string myShantoJson = PlayerPrefs.GetString("MyShanto", "{}");*/
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
        JObject dokanSellData = JObject.Parse(PlayerPrefs.GetString("DokanSell", "{}"));
     
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

        string displayText = "Expense:\n";
        int expenseAmount = 0;
        foreach (JObject expense in expenses)
        {
            foreach (KeyValuePair<string, JToken> entry in expense)
            {
                JObject expenseData = (JObject)entry.Value;
                string fromCash = (string)expenseData["FromCash"];
                if (!string.IsNullOrEmpty(fromCash))
                {
                    displayText += entry.Key + ":" + fromCash + "\n";
                    expenseAmount += int.Parse(fromCash);
                }
            }
        }

        displayText += "Purchase:\n";
        int purchaseAmount = 0;
        foreach (JObject purchase in purchases)
        {
            foreach (KeyValuePair<string, JToken> entry in purchase)
            {
                JObject purchaseData = (JObject)entry.Value;
                string fromCash = (string)purchaseData["FromCash"];
                if (!string.IsNullOrEmpty(fromCash))
                {
                    displayText += entry.Key + ":" + fromCash + "\n";
                    purchaseAmount += int.Parse(fromCash);
                }
            }
        }

        int bakiAmount = 0;
        if (!string.IsNullOrEmpty(baki))
        {
            displayText += "Baki:" + baki + "\n";
            bakiAmount += int.Parse(baki);
        }

        int remainingCash = (sellToHotel + sellAmountExceptHotel + bakiAmount) - (expenseAmount + purchaseAmount);
        displayText += "Cash:" + remainingCash + "\n";

        // Add remaining cash to JSON object
        dateData.Add("RemainingCash", remainingCash);
        // Add remaining cash to JSON object
        dateData.Add("Date", dateString);
        dateData.Add("SellToHotel", sellToHotel);
        // Convert dateData to JSON string
        string dateDataJsonString = dateData.ToString();

        displayText += "-------------------------\n";
        displayText += "Sum = " + (expenseAmount + purchaseAmount + bakiAmount + remainingCash) + $"    Hotel({sellToHotel})";
        displayText += sellToHotel + sellAmountExceptHotel;
        print(displayText);
        print(dateDataJsonString);
        Debug.LogError("Add an Erorr popup if accessing without any sell");
        //textDisplay.text = displayText;

        //StartCoroutine(test.PostRequest(dateDataJsonString));

    }
}
