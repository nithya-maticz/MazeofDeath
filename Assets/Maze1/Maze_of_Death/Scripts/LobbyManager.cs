using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public static bool OnTutorial;
    public static bool OnRetry;
    public bool IsNewUser;

    [Header("Pages")]
    public GameObject LoadingPage;
    public GameObject StoryPage;
    public GameObject LoginPage;
    public GameObject LobbyPage;
    public GameObject charPage;
    public TMP_Text levelText;

    [Header("Character Selection")]
    public static int currentIndex;
   

    [Header("Lobby")]
    public GameObject maleAniChar;
    public GameObject femaleAniChar;
    public Image lobbyCharIcon;
    public Image lobbyCharName;
    public Sprite lobbyMaleIcon;
    public Sprite lobbyMaleName;
    public Sprite lobbyFemaleIcon;
    public Sprite lobbyFemaleName;

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
        print("ssss");
        if (!OnTutorial)
        {
           LoadingPage.SetActive(true);
          StartCoroutine(LoadLobbyWithDelay());
        }
        if(OnTutorial)
        {

            if(OnRetry)
            {
                LobbyPage.SetActive(true);
                changeLobby();
            }
            else
            {
                OnRetry = true;
                LoginPage.SetActive(true);
                
            }
               
           
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
    public void LoginPageVisible()
    {
        LoginPage.SetActive(true);
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
           // 
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
            SceneManager.LoadScene("Game Level");
       
       
    }
   public void TutorialScene()
    {
        Fade();
       
        charPage.SetActive(true);
        //
        // 
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
        changeLobby();

    }
    public void changeLobby()
    {
        if (currentIndex == 1)
        {
            maleAniChar.SetActive(true);
            femaleAniChar.SetActive(false);
            lobbyCharIcon.sprite = lobbyMaleIcon;
            lobbyCharName.sprite = lobbyMaleName;

        }
        else if (currentIndex == 2)
        {
            maleAniChar.SetActive(false);
            femaleAniChar.SetActive(true);
            lobbyCharIcon.sprite = lobbyFemaleIcon;
            lobbyCharName.sprite = lobbyFemaleName;
        }
    }

}
