using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Login : MonoBehaviour
{
    public Button managementButton;
    public Button sellButton;
    public Button backButton;
    public Button submitButton;

    public TMP_InputField passwordField;

    public GameObject loginPanel;
    public GameObject managementPanel;
    public GameObject sellPanel;

    private void Start()
    {
        managementButton.onClick.AddListener(()=> OnManagementButtonClicked());
        sellButton.onClick.AddListener(()=> OnSellButtonClicked());
        backButton.onClick.AddListener(()=> OnBackButtonClicked());
        submitButton.onClick.AddListener(()=> OnSubmitButtonClicked());
    }
    public void OnManagementButtonClicked()
    {
        loginPanel.SetActive(true);

        passwordField.text = "";
        passwordField.gameObject.SetActive(true);

        managementButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
    }
    public void OnSellButtonClicked()
    {
        sellPanel.SetActive(true);
        managementPanel.SetActive(false);
        gameObject.SetActive(false);
    }
    public void OnBackButtonClicked()
    {
        loginPanel.SetActive(false);

        passwordField.text = "";
        passwordField.gameObject.SetActive(false);

        managementButton.gameObject.SetActive(true);
        sellButton.gameObject.SetActive(true);
    }
    public void OnSubmitButtonClicked()
    {
        if (passwordField.text == "B0ishakhi")
        {
            managementPanel.SetActive(true);
            sellPanel.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
