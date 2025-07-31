using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public TMP_InputField userName;
    public TMP_InputField email;
    public TMP_InputField age;
    public TMP_Text usernameErrortxt;
    public TMP_Text emailErrortxt;
    public TMP_Text ageErrortxt;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoginUser()
    {
        string u_name = userName.text.Trim();
        string u_email = email.text.Trim();
        string u_age = age.text.Trim();

        usernameErrortxt.text = "";
        emailErrortxt.text = "";
        ageErrortxt.text = "";

        bool hasError = false;

        // Username validation
        if (string.IsNullOrEmpty(u_name))
        {
            usernameErrortxt.text = "Enter user name";
            hasError = true;
        }

        // Email validation
        if (string.IsNullOrEmpty(u_email))
        {
            emailErrortxt.text = "Enter email";
            hasError = true;
        }
        else if (!IsValidEmail(u_email))
        {
            emailErrortxt.text = "Invalid email format";
            hasError = true;
        }

        // Age validation
        if (!int.TryParse(u_age, out int parsedAge))
        {
            ageErrortxt.text = "Age must be a number.";
           
            hasError = true;
        }
        else if (parsedAge <= 0)
        {
            ageErrortxt.text = "Age must be a positive number.";
            
            hasError = true;
        }

        if (!hasError)
        {
            Debug.Log("Login Success");
            LobbyManager.Instance.TutorialScene();
            // Add further login logic here
        }
    }

    private bool IsValidEmail(string email)
    {
        // Basic email validation using System.Text.RegularExpressions
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return System.Text.RegularExpressions.Regex.IsMatch(email, pattern);
    }
}


