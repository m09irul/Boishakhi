using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace InteractiveCalculator
{
    public class Calculator : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _displayText;

        [SerializeField]
        private int _maxDisplayLength = 12;

        public TextMeshProUGUI historyText; // Reference to the UI text element for history display

        public List<decimal> inputHistory = new List<decimal>();
        public List<string> operatorHistory = new List<string>();
        private bool lastInputWasOperator;

        public enum DisplayMode { value, input };
        public DisplayMode CurrentDisplayMode;

        public decimal input;
        public decimal value;
        public int InputDecimals { get; private set; }
        public bool UsingDecimals;
        private bool lastPressWasEquals;
        private string currentOperator;
        //public bool isOn;
        public bool isCalculatorSelected;
        void Start()
        {
            OnPressedClearAll();
            isCalculatorSelected = true;
        }
        private void Update()
        {
            if (isCalculatorSelected)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
                    OnPressedNumber(0);
                else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    OnPressedNumber(1);
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    OnPressedNumber(2);
                else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                    OnPressedNumber(3);
                else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                    OnPressedNumber(4);
                else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                    OnPressedNumber(5);
                else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                    OnPressedNumber(6);
                else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                    OnPressedNumber(7);
                else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                    OnPressedNumber(8);
                else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                    OnPressedNumber(9);
                else if (Input.GetKeyDown(KeyCode.Backspace))
                    OnPressedDelete();
                else if (Input.GetKeyDown(KeyCode.Escape))
                    OnPressedClearAll();

                else if (Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.KeypadPeriod))
                    OnPressedOperator(".");
                else if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                    OnPressedOperator("+");
                else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                    OnPressedOperator("-");
                else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                    OnPressedOperator("*");
                else if (Input.GetKeyDown(KeyCode.KeypadDivide))
                    OnPressedOperator("/");
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    OnPressedOperator("=");
            }
        }
        public void OnPressedClearEntry()
        {
            input = 0;
            InputDecimals = 0;
            UsingDecimals = false;
            UpdateDisplay();
        }

        public void OnPressedClearAll()
        {
            //isOn = true;
            value = 0;
            currentOperator = "+";
            CurrentDisplayMode = DisplayMode.value;

            OnPressedClearEntry();

            inputHistory.Clear();
            operatorHistory.Clear();
            OnPressedCheckHistory();
        }

/*        public void OnPressedOff()
        {
            OnPressedClearAll();
            isOn = false;
            UpdateDisplay();
        }

        public void OnPressedOn()
        {
            isOn = true;
            UpdateDisplay();
        }*/
        public void OnPressedDelete()
        {
            if (CurrentDisplayMode == DisplayMode.input && input > 0)
            {
                if(UsingDecimals)
                    InputDecimals = Mathf.Max(0, InputDecimals - 1);

                // Remove the last whole digit
                string numberAsString = input.ToString();
                string strippedNumberAsString;
                if (numberAsString.Length > 1)
                    strippedNumberAsString = numberAsString.Substring(0, numberAsString.Length - 1);
                else
                    strippedNumberAsString = "0";

                input = decimal.Parse(strippedNumberAsString);


                UpdateDisplay();
            }
        }
        public void OnPressedCheckHistory()
        {
            string historyDisplay = "";

            for (int i = 0; i < inputHistory.Count; i++)
            {
                historyDisplay += inputHistory[i].ToString() + "\n";
                if (i < operatorHistory.Count)
                {
                    historyDisplay += operatorHistory[i] + "\n";
                }
            }

            historyText.text = historyDisplay;
        }
        public void OnPressedOperator(string op)
        {
            if (lastPressWasEquals)
            {
                inputHistory.RemoveAt(inputHistory.Count - 1);
            }

            switch (op)
            {
                case "+":
                case "-":
                case "÷":
                case "/":
                case "*":
                case "x":
                case "X":
                    if (CurrentDisplayMode == DisplayMode.input)
                        OnPressedEquals();
                    currentOperator = op;
                    CurrentDisplayMode = DisplayMode.value;
                    lastPressWasEquals = false;
                    break;
                case "√":
                case "sqrt":
                    if (CurrentDisplayMode == DisplayMode.input)
                        OnPressedEquals();
                    currentOperator = op;
                    CurrentDisplayMode = DisplayMode.value;
                    lastPressWasEquals = false;
                    OnPressedEquals();
                    break;
                case "%":
                    if (CurrentDisplayMode == DisplayMode.input)
                    {
                        input /= 100;
                        OnPressedEquals();
                    }
                    currentOperator = op;
                    CurrentDisplayMode = DisplayMode.value;
                    lastPressWasEquals = false;
                    break;
                case "=":
                    if (CurrentDisplayMode == DisplayMode.input)
                        OnPressedEquals();
                    currentOperator = op;
                    lastPressWasEquals = false;
                    OnPressedEquals();
                    CurrentDisplayMode = DisplayMode.value;
                    break;
                default:
                    Debug.LogError("Unknown operator '" + op + "'");
                    break;
            }

            if (lastInputWasOperator)
            {
                operatorHistory.RemoveAt(operatorHistory.Count - 1);
            }

            operatorHistory.Add(op);
            lastInputWasOperator = true;
            CurrentDisplayMode = DisplayMode.value;

            OnPressedCheckHistory();
        }

        public void OnPressedNumber(int number)
        {
            if (lastPressWasEquals)
            {
                OnPressedClearAll();
            }

            lastInputWasOperator = false;
            if (CurrentDisplayMode == DisplayMode.value)
                input = 0;
            if (lastPressWasEquals)
                value = 0;

            if (!UsingDecimals)
            {
                input *= 10;
                input += number;
            }
            else
            {
                input += (decimal)number / (decimal)Mathf.Pow(10, ++InputDecimals);
            }

            CurrentDisplayMode = DisplayMode.input;
            lastPressWasEquals = false;
            UpdateDisplay();
        }

        public void OnPressedDecimal()
        {
            UsingDecimals = true;
            lastPressWasEquals = false;

            if (CurrentDisplayMode != DisplayMode.input)
            {
                input = 0;
                InputDecimals = 0;
                CurrentDisplayMode = DisplayMode.input;
            }

            UpdateDisplay();
        }

        public void OnPressedPi()
        {
            input = 3.14159265358979323m;
            UsingDecimals = true;
            InputDecimals = 12;
            lastPressWasEquals = false;
            CurrentDisplayMode = DisplayMode.input;
            UpdateDisplay();
        }

        public void OnPressedSwapSign()
        {
            if (CurrentDisplayMode == DisplayMode.input)
                input *= -1;
            else
                value *= -1;

            UpdateDisplay();
        }

        public void OnPressedEquals()
        {
            inputHistory.Add(input);
            try
            {
                switch (currentOperator)
                {
                    case "-":
                        value -= input;
                        break;
                    case "+":
                        value += input;
                        break;
                    case "÷":
                    case "/":
                        value /= input;
                        break;
                    case "*":
                    case "x":
                    case "X":
                        value *= input;
                        break;
                    case "√":
                    case "sqrt":
                        value = (decimal) Mathf.Sqrt((float) value);
                        break;
                    case "=":
                        inputHistory.RemoveAt(inputHistory.Count - 1);
                        inputHistory.Add(value);
                        break;
                    default:
                        Debug.LogError("Unknown operator '" + currentOperator + "'");
                        break;
                }

                // Clear input and update display

                lastPressWasEquals = true;
                UsingDecimals = false;
                InputDecimals = 0;
                CurrentDisplayMode = DisplayMode.value;
                UpdateDisplay();
            }
            catch
            {
                _displayText.text = "Error";
            }
        }

        private void UpdateDisplay()
        {
            //_displayText.gameObject.SetActive(true);
            
            //if (!isOn) return;

            if (CurrentDisplayMode == DisplayMode.input)
            {
                _displayText.text = FormatDecimal(input, InputDecimals);
                return;
            }

            _displayText.text = FormatDecimal(Normalize(value), DigitsAfterDecimal(value));
        }

        string FormatDecimal(decimal val, int decimalDigits)
        {
            string str = val.ToString($"N{decimalDigits}", new CultureInfo("en-US"));
            if (str.Length <= _maxDisplayLength)
                return str;

            if (val < (decimal)Mathf.Pow(10, _maxDisplayLength - 3) && val > -(decimal)Mathf.Pow(10, _maxDisplayLength - 4))
                return str.Substring(0, _maxDisplayLength);

            return val.ToString($"G7", new CultureInfo("en-US"));
        }
        static int DigitsAfterDecimal(decimal value)
        {
            return ((SqlDecimal)value).Scale;
        }

        public static decimal Normalize(decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }
    }
}