using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public static bool OnTutorial;
    public bool IsNewUser;

    [Header("Pages")]
    public GameObject LoadingPage;
    public GameObject StoryPage;
    public GameObject LoginPage;
    public GameObject LobbyPage;
    public TMP_Text levelText;
    public static string character;
    public int CharacterIndex;
    public int currentIndex;


    [Header("Fader")]
    public Animator Fader;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (!OnTutorial)
        {
           // LoadingPage.SetActive(true);
           // StartCoroutine(LoadLobbyWithDelay());
        }
        if(OnTutorial)
        {
            levelText.text = "Maze Level 1";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadLobbyWithDelay()
    {
        yield return new WaitForSeconds(2f);
        OnLoadLobby();
    }

    void OnLoadLobby()
    {
        if (IsNewUser)
        {
            LoadingPage.SetActive(false);
            VideoController.Instance.PlayNextVideo();
        }
        else
        {
            Fade();
            LoadingPage.SetActive(false);
            LoginPage.SetActive(true);
        }
    }

    public void Fade()
    {
        Fader.gameObject.SetActive(true);
        Fader.SetTrigger("Fade");
    }
    public void PlayGame()
    {
        Fade();
        OnTutorial = true;
        SceneManager.LoadScene("Story");
    }
   public void lobbyPage()
    {
        Fade();
        LoginPage.SetActive(false);
        LobbyPage.SetActive(true);
     }

    public void NextCharacterSelection()
    {
        character = "female";
        CharacterIndex = 1;
    }
    public void previousCharacterSelection()
    {
        character = "male";
        CharacterIndex = 2;
    }
    public void SelectFunction()
    {
        currentIndex = CharacterIndex;
    }

    public void Character(string _name)
    {
        
    }

}
