using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] TextMeshProUGUI printBox;
    public TMP_InputField nameText;
    public TMP_InputField mailText;
    public TMP_InputField ageText;
    public int login;

    private void Start()
    {
        Instance = this;
        printBox.text = "";
        textBox.text = "";
        nameText.text = "";
        mailText.text = "";
        ageText.text = "";
    }

    public void DeleteLetter()
    {
        if(textBox.text.Length != 0) {
            textBox.text = textBox.text.Remove(textBox.text.Length - 1, 1);
        }
        if (login == 1)
        {
            nameText.text = nameText.text.Remove(nameText.text.Length - 1, 1);
        }
        else if (login == 2)
        {
            mailText.text = mailText.text.Remove(mailText.text.Length - 1, 1);
        }

        else if (login == 3)
        {
            ageText.text = ageText.text.Remove(ageText.text.Length - 1, 1);
        }

    }

    public void AddLetter(string letter)
    {
        if(login==1)
        {
            nameText.text = nameText.text + letter;
        }
        else if(login==2)
        {
            mailText.text = mailText.text + letter;
        }

        else if (login == 3)
        {
            ageText.text = ageText.text + letter;
        }

    }

    public void SubmitWord()
    {
        printBox.text = textBox.text;
        textBox.text = "";
        // Debug.Log("Text submitted successfully!");
    }

    public void AssignValue(int val)
    {
        login = val;
    }
}
