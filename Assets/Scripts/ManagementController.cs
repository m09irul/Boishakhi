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
        dateString = DateTime.Today.ToString("yyyy-MM-dd");

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
        // Create a dictionary to hold the final data
        var data = new Dictionary<string, object>
        {
            [dateString] = new Dictionary<string, object>
            {
                ["Baki"] = bakiField.text,
                ["Expense"] = new List<object>(),
                ["Purchase"] = new List<object>()
            }
        };

        //populate expense
        ((List<object>)((Dictionary<string, object>)data[dateString])["Expense"]).Add(new Dictionary<string, object>
        {
            ["Shanto"] = new Dictionary<string, object>
            {
                ["FromCash"] = expenseFields[0].text,
                ["FromFund"] = ""
            }
        });
        ((List<object>)((Dictionary<string, object>)data[dateString])["Expense"]).Add(new Dictionary<string, object>
        {
            ["Dokan"] = new Dictionary<string, object>
            {
                ["FromCash"] = expenseFields[1].text,
                ["FromFund"] = ""
            }
        });

        // Loop through each expense field and add the data to the dictionary
        for (int i = 2; i < expenseFields.Count - 2; i += 3)
        {
            ((List<object>)((Dictionary<string, object>)data[dateString])["Expense"]).Add(new Dictionary<string, object>
            {
                [expenseFields[i].text] = new Dictionary<string, object>
                {
                    ["FromCash"] = expenseFields[i + 1].text,
                    ["FromFund"] = expenseFields[i + 2].text
                }
            });
        }

        //populate purchase

        // Loop through each purchase field and add the data to the dictionary
        for (int i = 0; i < purchaseFields.Count - 2; i += 3)
        {
            ((List<object>)((Dictionary<string, object>)data[dateString])["Purchase"]).Add(new Dictionary<string, object>
            {
                [purchaseFields[i].text] = new Dictionary<string, object>
                {
                    ["FromCash"] = purchaseFields[i + 1].text,
                    ["FromFund"] = purchaseFields[i + 2].text
                }
            });
        }

        // Convert the data into a JSON string
        string json = JsonConvert.SerializeObject(data);

        // Output the JSON string
        print(json);
    }
    void GenerateCigarJson()
    {
        // Create a dictionary to hold the final data
        var data = new Dictionary<string, object>
        {
            [dateString] = new Dictionary<string, object>
            {
                ["Cash_in"] = new List<object>(),
                ["Cash_out"] = new List<object>()
            }
        };

        //populate Cash_in
        ((List<object>)((Dictionary<string, object>)data[dateString])["Cash_in"]).Add(new Dictionary<string, object>
        {
            ["SellCigar"] = cigarCashInText.text
        });

        // Loop through each expense field and add the data to the dictionary
        for (int i = 0; i < cigarCashInFields.Count - 1; i += 2)
        {
            ((List<object>)((Dictionary<string, object>)data[dateString])["Cash_in"]).Add(new Dictionary<string, object>
            {
                [cigarCashInFields[i].text] = cigarCashInFields[i + 1].text
            });
        }

        //populate Cash_out
        ((List<object>)((Dictionary<string, object>)data[dateString])["Cash_out"]).Add(new Dictionary<string, object>
        {
            ["BuyCigar"] = cigarCashOutFields[0].text
        });

        // Loop through each expense field and add the data to the dictionary
        for (int i = 1; i < cigarCashOutFields.Count - 1; i += 2)
        {
            ((List<object>)((Dictionary<string, object>)data[dateString])["Cash_out"]).Add(new Dictionary<string, object>
            {
                [cigarCashOutFields[i].text] = cigarCashOutFields[i + 1].text
            });
        }

        // Convert the data into a JSON string
        string json = JsonConvert.SerializeObject(data);

        // Output the JSON string
        print(json);



    }
    void GenerateShantoJson()
    {
        // Create a dictionary to hold the final data
        var data = new Dictionary<string, object>
        {
            [dateString] = new Dictionary<string, object>
            {
                ["Cash_in"] = new List<object>(),
                ["Cash_out"] = new List<object>()
            }
        };

        //populate Cash_in
        ((List<object>)((Dictionary<string, object>)data[dateString])["Cash_in"]).Add(new Dictionary<string, object>
        {
            ["Shanto"] = S_D_expenseTexts[0].text,
        });
        ((List<object>)((Dictionary<string, object>)data[dateString])["Cash_in"]).Add(new Dictionary<string, object>
        {
            ["Dokan"] = S_D_expenseTexts[1].text
        });

        // Loop through each expense field and add the data to the dictionary
        for (int i = 0; i < shantoCashInFields.Count - 1; i += 2)
        {
            ((List<object>)((Dictionary<string, object>)data[dateString])["Cash_in"]).Add(new Dictionary<string, object>
            {
                [shantoCashInFields[i].text] = shantoCashInFields[i + 1].text
            });
        }

        //populate Cash_out

        // Loop through each expense field and add the data to the dictionary
        for (int i = 0; i < shantoCashOutFields.Count - 1; i += 2)
        {
            ((List<object>)((Dictionary<string, object>)data[dateString])["Cash_out"]).Add(new Dictionary<string, object>
            {
                [shantoCashOutFields[i].text] = shantoCashOutFields[i + 1].text
            });
        }

        // Convert the data into a JSON string
        string json = JsonConvert.SerializeObject(data);

        // Output the JSON string
        print(json);
    }
}
