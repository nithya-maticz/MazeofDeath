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
    public GameObject charPage;
    public TMP_Text levelText;
    public static string character;
    public int CharacterIndex;
    public int currentIndex;
    public GameObject maleAniChar;
    public GameObject femaleAniChar;
    public GameObject maleIcon;
    public GameObject femaleIcon;
    public GameObject maleName;
    public GameObject femaleName;
    //public GameObject femaleiChar;
    //public GameObject femaleAniChar;




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
            LoadingPage.SetActive(true);
            StartCoroutine(LoadLobbyWithDelay());
        }
        if(OnTutorial)
        {
            levelText.text = "Maze Level 1";
            LobbyPage.SetActive(true);
            LoadingPage.SetActive(false);
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
        if(!OnTutorial)
        {
            Fade();
            OnTutorial = true;
            SceneManager.LoadScene("Story");
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
       
    }
   public void lobbyPage()
    {
        Fade();
        LoginPage.SetActive(false);
        charPage.SetActive(true);
       // 
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
   

    public void Character(int index)
    {
        currentIndex = index;
    }

    public void selectfun()
    {
        Fade();
        Debug.Log(currentIndex); 
        LobbyPage.SetActive(true);
        if(currentIndex==1)
        {
            maleAniChar.SetActive(true);
            femaleAniChar.SetActive(false);
            femaleName.SetActive(false);
            femaleIcon.SetActive(false);
        }
        else if (currentIndex == 2)
        {
            maleAniChar.SetActive(false);
            femaleAniChar.SetActive(true);
            femaleName.SetActive(true);
            femaleIcon.SetActive(true);
            maleName.SetActive(false);
            maleIcon.SetActive(false);
        }

    }

}
