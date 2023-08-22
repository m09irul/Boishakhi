using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InteractiveCalculator;
using TMPro;

public class DailySellController : MonoBehaviour
{
    /// <summary>
    /// 0 = price field
    /// 1 = deposit field
    /// </summary>
    public TMP_InputField[] fields;
    public TextMeshProUGUI returnText;
    public Calculator calculator;
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
}
